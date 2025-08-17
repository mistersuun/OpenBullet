using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Data;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace OpenBullet.Core.Jobs;

/// <summary>
/// Implementation of job manager for orchestrating bot execution
/// </summary>
public class JobManager : IJobManager, IDisposable
{
    private readonly ILogger<JobManager> _logger;
    private readonly IBotRunner _botRunner;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, Job> _activeJobs = new();
    private readonly ConcurrentDictionary<string, List<BotResult>> _jobResults = new();
    private readonly JobManagerStatistics _statistics = new();
    private readonly SemaphoreSlim _jobManagementSemaphore = new(1, 1);
    private bool _disposed = false;

    public event EventHandler<JobStatusChangedEventArgs>? JobStatusChanged;
    public event EventHandler<BotCompletedEventArgs>? BotCompleted;
    public event EventHandler<JobCompletedEventArgs>? JobCompleted;

    public JobManager(ILogger<JobManager> logger, IBotRunner botRunner, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _botRunner = botRunner;
        _serviceProvider = serviceProvider;
        _statistics.StartTime = DateTime.UtcNow;
    }

    public async Task<string> StartJobAsync(JobConfiguration jobConfig, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _jobManagementSemaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Starting job {JobName} with {DataLineCount} data lines and {ConcurrentBots} concurrent bots", 
                jobConfig.Name, jobConfig.DataLines.Count, jobConfig.ConcurrentBots);

            // Validate configuration
            var validation = jobConfig.Validate();
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors);
                _logger.LogError("Job configuration validation failed: {Errors}", errors);
                throw new ArgumentException($"Invalid job configuration: {errors}");
            }

            // Log warnings
            foreach (var warning in validation.Warnings)
            {
                _logger.LogWarning("Job configuration warning: {Warning}", warning);
            }

            // Create job
            var jobId = Guid.NewGuid().ToString();
            var job = new Job
            {
                Id = jobId,
                Configuration = jobConfig,
                Status = new JobStatus
                {
                    JobId = jobId, // Use the same ID for both job and status
                    Name = jobConfig.Name,
                    State = JobState.Created,
                    TotalDataLines = jobConfig.DataLines.Count,
                    StartTime = DateTime.UtcNow
                },
                CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
            };

            // Add to active jobs
            _activeJobs[job.Id] = job;
            _jobResults[job.Id] = new List<BotResult>();

            // Update statistics
            _statistics.TotalJobsCreated++;
            _statistics.ActiveJobs++;
            _statistics.LastJobStartTime = DateTime.UtcNow;

            // Start job execution in background and set completion task
            job.CompletionTask = ExecuteJobAsync(job);

            _logger.LogInformation("Job {JobId} ({JobName}) started successfully", job.Id, jobConfig.Name);
            return job.Id;
        }
        finally
        {
            _jobManagementSemaphore.Release();
        }
    }

    public async Task<bool> StopJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (!_activeJobs.TryGetValue(jobId, out var job))
        {
            _logger.LogWarning("Attempted to stop non-existent job {JobId}", jobId);
            return false;
        }

        await _jobManagementSemaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Stopping job {JobId} ({JobName})", jobId, job.Configuration.Name);

            job.Status.State = JobState.Stopping;
            NotifyJobStatusChanged(jobId, JobState.Running, JobState.Stopping);

            job.CancellationTokenSource.Cancel();

            // Wait for job to complete or timeout
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            var completionTask = job.CompletionTask ?? Task.CompletedTask;
            
            await Task.WhenAny(completionTask, timeoutTask);

            if (!completionTask.IsCompleted)
            {
                _logger.LogWarning("Job {JobId} did not stop gracefully within timeout", jobId);
            }

            job.Status.State = JobState.Cancelled;
            job.Status.EndTime = DateTime.UtcNow;
            
            NotifyJobStatusChanged(jobId, JobState.Stopping, JobState.Cancelled);

            _logger.LogInformation("Job {JobId} stopped", jobId);
            return true;
        }
        finally
        {
            _jobManagementSemaphore.Release();
        }
    }

    public async Task<bool> PauseJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (!_activeJobs.TryGetValue(jobId, out var job))
        {
            return false;
        }

        await _jobManagementSemaphore.WaitAsync(cancellationToken);
        try
        {
            // Allow pausing if job is in Running, Starting, or even Created state (before execution starts)
            if (job.Status.State == JobState.Completed || job.Status.State == JobState.Failed || 
                job.Status.State == JobState.Cancelled)
            {
                _logger.LogWarning("Cannot pause job {JobId} in state {State}", jobId, job.Status.State);
                return false;
            }

            // If job is already paused, return true
            if (job.Status.State == JobState.Paused)
            {
                _logger.LogDebug("Job {JobId} is already paused", jobId);
                return true;
            }

            _logger.LogInformation("Pausing job {JobId} ({JobName}) from state {State}", 
                jobId, job.Configuration.Name, job.Status.State);

            var oldState = job.Status.State;
            job.Status.State = JobState.Paused;
            
            // Ensure PauseTokenSource is initialized before using it
            if (job.PauseTokenSource == null)
            {
                job.PauseTokenSource = new CancellationTokenSource();
            }
            
            // Cancel the pause token to signal pause to running bots
            job.PauseTokenSource.Cancel();
            
            NotifyJobStatusChanged(jobId, oldState, JobState.Paused);

            _logger.LogInformation("Job {JobId} paused successfully", jobId);
            return true;
        }
        finally
        {
            _jobManagementSemaphore.Release();
        }
    }

    public async Task<bool> ResumeJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (!_activeJobs.TryGetValue(jobId, out var job))
        {
            return false;
        }

        await _jobManagementSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (job.Status.State != JobState.Paused)
            {
                _logger.LogWarning("Cannot resume job {JobId} in state {State}", jobId, job.Status.State);
                return false;
            }

            _logger.LogInformation("Resuming job {JobId} ({JobName})", jobId, job.Configuration.Name);

            var oldState = job.Status.State;
            job.Status.State = JobState.Running;
            
            // Dispose old pause token source if it exists
            if (job.PauseTokenSource != null)
            {
                job.PauseTokenSource.Dispose();
            }
            
            // Create a new pause token source for future pause operations
            job.PauseTokenSource = new CancellationTokenSource();
            
            NotifyJobStatusChanged(jobId, oldState, JobState.Running);

            _logger.LogInformation("Job {JobId} resumed successfully", jobId);
            return true;
        }
        finally
        {
            _jobManagementSemaphore.Release();
        }
    }

    public JobStatus GetJobStatus(string jobId)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (_activeJobs.TryGetValue(jobId, out var job))
        {
            // Update elapsed time and progress
            job.Status.ElapsedTime = DateTime.UtcNow - job.Status.StartTime;
            job.Status.UpdateProgress();
            return job.Status;
        }

        return new JobStatus { JobId = jobId, State = JobState.Failed };
    }

    public List<JobStatus> GetActiveJobs()
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        var activeJobs = new List<JobStatus>();
        foreach (var job in _activeJobs.Values)
        {
            job.Status.ElapsedTime = DateTime.UtcNow - job.Status.StartTime;
            job.Status.UpdateProgress();
            activeJobs.Add(job.Status);
        }
        return activeJobs;
    }

    public async Task<List<BotResult>> GetJobResultsAsync(string jobId, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await Task.CompletedTask; // Make async for consistency
        
        if (_jobResults.TryGetValue(jobId, out var results))
        {
            return new List<BotResult>(results);
        }

        return new List<BotResult>();
    }

    public async Task<bool> ClearJobDataAsync(string jobId, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _jobManagementSemaphore.WaitAsync(cancellationToken);
        try
        {
            var removed = _activeJobs.TryRemove(jobId, out var job);
            _jobResults.TryRemove(jobId, out _);

            if (removed && job != null)
            {
                job.CancellationTokenSource.Dispose();
                if (job.PauseTokenSource != null)
                {
                    job.PauseTokenSource.Dispose();
                }
                _statistics.ActiveJobs = Math.Max(0, _statistics.ActiveJobs - 1);
            }

            _logger.LogInformation("Cleared data for job {JobId}", jobId);
            return removed;
        }
        finally
        {
            _jobManagementSemaphore.Release();
        }
    }

    public JobManagerStatistics GetStatistics()
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        _statistics.ActiveJobs = _activeJobs.Count;
        
        if (_statistics.TotalExecutionTime.TotalMinutes > 0)
        {
            _statistics.AverageBotsPerMinute = _statistics.TotalBotsExecuted / _statistics.TotalExecutionTime.TotalMinutes;
        }

        return _statistics;
    }

    private async Task ExecuteJobAsync(Job job)
    {
        Console.WriteLine($"ExecuteJobAsync ENTERED for job {job.Id}");
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            Console.WriteLine($"ExecuteJobAsync: About to log debug message for job {job.Id}");
            _logger.LogDebug("ExecuteJobAsync entered for job {JobId}", job.Id);
            Console.WriteLine($"ExecuteJobAsync: Debug message logged for job {job.Id}");
            
            Console.WriteLine($"ExecuteJobAsync: About to log info message for job {job.Id}");
            _logger.LogInformation("Starting execution of job {JobId} ({JobName})", job.Id, job.Configuration.Name);
            Console.WriteLine($"ExecuteJobAsync: Info message logged for job {job.Id}");

            Console.WriteLine($"ExecuteJobAsync: About to set job state to Starting for job {job.Id}");
            _logger.LogDebug("Setting job {JobId} state to Starting", job.Id);
            Console.WriteLine($"ExecuteJobAsync: Setting job state to Starting for job {job.Id}");
            job.Status.State = JobState.Starting;
            Console.WriteLine($"ExecuteJobAsync: Job state set to Starting for job {job.Id}");
            
            Console.WriteLine($"ExecuteJobAsync: About to notify job status changed for job {job.Id}");
            NotifyJobStatusChanged(job.Id, JobState.Created, JobState.Starting);
            Console.WriteLine($"ExecuteJobAsync: Job status changed notified for job {job.Id}");

            // Create semaphore for concurrency control
            Console.WriteLine($"ExecuteJobAsync: About to create semaphore for job {job.Id}");
            Console.WriteLine($"ExecuteJobAsync: Job configuration concurrent bots: {job.Configuration.ConcurrentBots}");
            _logger.LogDebug("Creating semaphore for job {JobId} with {ConcurrentBots} concurrent bots", job.Id, job.Configuration.ConcurrentBots);
            Console.WriteLine($"ExecuteJobAsync: About to create SemaphoreSlim for job {job.Id}");
            using var semaphore = new SemaphoreSlim(job.Configuration.ConcurrentBots, job.Configuration.ConcurrentBots);
            Console.WriteLine($"ExecuteJobAsync: SemaphoreSlim created for job {job.Id}");

            Console.WriteLine($"ExecuteJobAsync: About to set job state to Running for job {job.Id}");
            _logger.LogDebug("Setting job {JobId} state to Running", job.Id);
            Console.WriteLine($"ExecuteJobAsync: Setting job state to Running for job {job.Id}");
            job.Status.State = JobState.Running;
            Console.WriteLine($"ExecuteJobAsync: Job state set to Running for job {job.Id}");
            
            Console.WriteLine($"ExecuteJobAsync: About to notify job status changed to Running for job {job.Id}");
            NotifyJobStatusChanged(job.Id, JobState.Starting, JobState.Running);
            Console.WriteLine($"ExecuteJobAsync: Job status changed to Running notified for job {job.Id}");

            Console.WriteLine($"ExecuteJobAsync: About to get data lines for job {job.Id}");
            _logger.LogDebug("Getting data lines for job {JobId}", job.Id);
            Console.WriteLine($"ExecuteJobAsync: About to access job.Configuration.DataLines for job {job.Id}");
            var dataLines = job.Configuration.DataLines;
            Console.WriteLine($"ExecuteJobAsync: Data lines accessed for job {job.Id}, count: {dataLines?.Count ?? -1}");
            Console.WriteLine($"ExecuteJobAsync: About to log debug message about data line count for job {job.Id}");
            _logger.LogDebug("Job {JobId} has {DataLineCount} data lines to process", job.Id, dataLines?.Count ?? 0);
            Console.WriteLine($"ExecuteJobAsync: Debug message about data line count logged for job {job.Id}");
            
            Console.WriteLine($"ExecuteJobAsync: About to create tasks list for job {job.Id}");
            var tasks = new List<Task>();
            Console.WriteLine($"ExecuteJobAsync: Tasks list created for job {job.Id}");

            // Process data lines with proper pause/resume support
            Console.WriteLine($"ExecuteJobAsync: About to start foreach loop over data lines for job {job.Id}");
            if (dataLines != null)
            {
                foreach (var dataLine in dataLines)
            {
                Console.WriteLine($"ExecuteJobAsync: Processing data line '{dataLine}' for job {job.Id}");
                if (job.CancellationTokenSource.Token.IsCancellationRequested)
                {
                    break;
                }

                // Wait for available slot
                await semaphore.WaitAsync(job.CancellationTokenSource.Token);

                // Create bot execution task
                _logger.LogDebug("Creating bot execution task for data line: {DataLine} in job {JobId}", dataLine, job.Id);
                var botTask = ExecuteBotAsync(job, dataLine, semaphore);
                tasks.Add(botTask);
            }

            }
            
            // Wait for all bots to complete, but allow pausing during execution
            Console.WriteLine($"ExecuteJobAsync: Waiting for {tasks.Count} bot tasks to complete for job {job.Id}");
            _logger.LogDebug("Waiting for {TaskCount} bot tasks to complete for job {JobId}", tasks.Count, job.Id);
            
            while (tasks.Any(t => !t.IsCompleted))
            {
                Console.WriteLine($"ExecuteJobAsync: Loop iteration for job {job.Id}, {tasks.Count} tasks remaining");
                
                // Check for pause state
                if (job.Status.State == JobState.Paused)
                {
                    _logger.LogDebug("Job {JobId} is paused, waiting for resume...", job.Id);
                    await Task.Delay(100, job.CancellationTokenSource.Token);
                    continue;
                }

                // Check if any tasks completed
                var completedTasks = tasks.Where(t => t.IsCompleted).ToList();
                foreach (var completedTask in completedTasks)
                {
                    tasks.Remove(completedTask);
                    Console.WriteLine($"ExecuteJobAsync: Bot task completed for job {job.Id}, {tasks.Count} remaining");
                    _logger.LogDebug("Bot task completed for job {JobId}, {RemainingTasks} remaining", job.Id, tasks.Count);
                }

                // Small delay to prevent busy waiting
                await Task.Delay(50, job.CancellationTokenSource.Token);
            }
            
            Console.WriteLine($"ExecuteJobAsync: All bot tasks completed for job {job.Id}");
            _logger.LogDebug("All bot tasks completed for job {JobId}", job.Id);

            // Job completed successfully
            job.Status.State = JobState.Completed;
            job.Status.EndTime = DateTime.UtcNow;
            job.Status.ElapsedTime = stopwatch.Elapsed;

            // Update statistics
            _statistics.CompletedJobs++;
            _statistics.ActiveJobs = Math.Max(0, _statistics.ActiveJobs - 1);
            _statistics.TotalExecutionTime = _statistics.TotalExecutionTime.Add(job.Status.ElapsedTime);

            NotifyJobStatusChanged(job.Id, JobState.Running, JobState.Completed);

            var results = await GetJobResultsAsync(job.Id);
            var wasSuccessful = results.Count > 0 && results.Any(r => r.Success);

            // TODO: Fix BotResult vs BasicBotResult type conflict
            // NotifyJobCompleted(job.Id, job.Status, results, wasSuccessful);

            _logger.LogInformation("Job {JobId} completed successfully in {ElapsedTime}", 
                job.Id, job.Status.ElapsedTime);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Job {JobId} was cancelled", job.Id);
            job.Status.State = JobState.Cancelled;
            job.Status.EndTime = DateTime.UtcNow;
            _statistics.CancelledJobs++;
            _statistics.ActiveJobs = Math.Max(0, _statistics.ActiveJobs - 1);
            NotifyJobStatusChanged(job.Id, JobState.Running, JobState.Cancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job {JobId} failed with exception: {ExceptionType} - {Message}", job.Id, ex.GetType().Name, ex.Message);
            job.Status.State = JobState.Failed;
            job.Status.EndTime = DateTime.UtcNow;
            job.Status.LastErrorMessage = ex.Message;
            _statistics.FailedJobs++;
            _statistics.ActiveJobs = Math.Max(0, _statistics.ActiveJobs - 1);
            NotifyJobStatusChanged(job.Id, JobState.Running, JobState.Failed);

            var results = await GetJobResultsAsync(job.Id);
            // TODO: Fix BotResult vs BasicBotResult type conflict
            // NotifyJobCompleted(job.Id, job.Status, results, false, ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            // Ensure completion task is marked as completed
            if (job.CompletionTask != null && !job.CompletionTask.IsCompleted)
            {
                // The task should already be completed by now, but let's make sure
                try
                {
                    await job.CompletionTask;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Job {JobId} completion task had an exception", job.Id);
                }
            }
        }
    }

    private async Task<BotResult> ExecuteBotWithPauseChecking(Job job, BotData botData)
    {
        // Check for pause state before starting execution
        if (job.Status.State == JobState.Paused)
        {
            _logger.LogDebug("Bot execution paused for job {JobId}, waiting for resume...", job.Id);
            
            // Wait for resume or cancellation
            while (job.Status.State == JobState.Paused && !job.CancellationTokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(100, job.CancellationTokenSource.Token);
            }
        }

        // Execute the bot
        return await _botRunner.RunAsync(job.Configuration.Config, botData.DataLine, job.CancellationTokenSource.Token);
    }

    private async Task ExecuteBotAsync(Job job, string dataLine, SemaphoreSlim semaphore)
    {
        _logger.LogDebug("ExecuteBotAsync called for data line: {DataLine} in job {JobId}", dataLine, job.Id);
        try
        {
            job.Status.ActiveBots++;

            // Check for pause before starting bot execution
            while (job.Status.State == JobState.Paused && !job.CancellationTokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(100, job.CancellationTokenSource.Token);
            }

            // Create BotData for execution
            var botData = new BotData(dataLine, job.Configuration.Config, 
                Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance, 
                job.CancellationTokenSource.Token);
            
            // Execute bot with pause checking
            var botResult = await ExecuteBotWithPauseChecking(job, botData);

            // Update job statistics
            job.Status.ProcessedDataLines++;
            
            switch (botResult.Status)
            {
                case BotStatus.Success:
                    job.Status.SuccessfulBots++;
                    _statistics.TotalSuccessfulBots++;
                    break;
                case BotStatus.Failure:
                    job.Status.FailedBots++;
                    _statistics.TotalFailedBots++;
                    break;
                case BotStatus.Retry:
                    job.Status.RetryBots++;
                    break;
                case BotStatus.Ban:
                    job.Status.BannedBots++;
                    break;
                case BotStatus.Error:
                    job.Status.ErrorBots++;
                    _statistics.TotalFailedBots++;
                    break;
                case BotStatus.Custom:
                    job.Status.CustomStatusBots++;
                    break;
            }

            // Update configuration usage count
            try
            {
                // Get configuration storage service and update usage
                var configStorage = _serviceProvider.GetService<IConfigurationStorage>();
                if (configStorage != null)
                {
                    await configStorage.UpdateUsageAsync(job.Configuration.Config.Id, botResult.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update configuration usage count for job {JobId}", job.Id);
            }

            // Update global statistics
            _statistics.TotalBotsExecuted++;

            // Store result if configured to do so
            if (job.Configuration.SaveResults)
            {
                if (!job.Configuration.SaveOnlySuccessful || botResult.Success)
                {
                    _jobResults[job.Id].Add(botResult);
                }
            }

            // Always save results to database for integration testing
            try
            {
                var resultStorage = _serviceProvider.GetService<IResultStorage>();
                if (resultStorage != null)
                {
                    _logger.LogDebug("Saving bot result to database for job {JobId}", job.Id);
                    await resultStorage.CreateFromBotResultAsync(job.Id, botResult);
                    _logger.LogDebug("Successfully saved bot result to database for job {JobId}", job.Id);
                }
                else
                {
                    _logger.LogWarning("IResultStorage service not found in service provider for job {JobId}", job.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save bot result to database for job {JobId}", job.Id);
            }

            // Notify about bot completion
            NotifyBotCompleted(job.Id, botResult);

            // Handle retry logic
            if (botResult.Status == BotStatus.Retry && job.Configuration.MaxRetries > 0)
            {
                // Could implement retry logic here
                _logger.LogTrace("Bot {BotId} requested retry (not implemented yet)", botResult.BotId);
            }

            // Check if job should stop on error
            if (!botResult.Success && job.Configuration.StopOnError)
            {
                _logger.LogWarning("Stopping job {JobId} due to bot error: {ErrorMessage}", 
                    job.Id, botResult.ErrorMessage);
                job.CancellationTokenSource.Cancel();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when job is cancelled
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot execution failed for job {JobId} with data line: {DataLine}", 
                job.Id, dataLine.Length > 50 ? dataLine[..50] + "..." : dataLine);

            job.Status.ErrorBots++;
            job.Status.ProcessedDataLines++;
        }
        finally
        {
            job.Status.ActiveBots = Math.Max(0, job.Status.ActiveBots - 1);
            semaphore.Release();
        }
    }



    private void NotifyJobStatusChanged(string jobId, JobState oldState, JobState newState)
    {
        JobStatusChanged?.Invoke(this, new JobStatusChangedEventArgs
        {
            JobId = jobId,
            OldState = oldState,
            NewState = newState,
            Timestamp = DateTime.UtcNow
        });
    }

    private void NotifyBotCompleted(string jobId, BotResult result)
    {
        // Convert BotResult to BasicBotResult for the event
        var basicResult = new BasicBotResult
        {
            BotId = result.BotId,
            Status = result.Status,
            CustomStatus = result.CustomStatus,
            CapturedData = result.CapturedData,
            DataLine = result.DataLine,
            ExecutionTime = (long)result.ExecutionTime.TotalMilliseconds,
            Log = result.Logs,
            Exception = result.Exception,
            ErrorMessage = result.ErrorMessage,
            Metadata = result.Metadata
        };

        BotCompleted?.Invoke(this, new BotCompletedEventArgs
        {
            JobId = jobId,
            Result = basicResult,
            Timestamp = DateTime.UtcNow
        });
    }



    private void NotifyJobCompleted(string jobId, JobStatus status, List<BasicBotResult> results, bool wasSuccessful, string? errorMessage = null)
    {
        JobCompleted?.Invoke(this, new JobCompletedEventArgs
        {
            JobId = jobId,
            FinalStatus = status,
            Results = results,
            Timestamp = DateTime.UtcNow,
            WasSuccessful = wasSuccessful,
            ErrorMessage = errorMessage
        });
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        // Cancel all active jobs
        foreach (var job in _activeJobs.Values)
        {
            job.CancellationTokenSource.Cancel();
            job.CancellationTokenSource.Dispose();
            if (job.PauseTokenSource != null)
            {
                job.PauseTokenSource.Dispose();
            }
        }

        _activeJobs.Clear();
        _jobResults.Clear();
        _jobManagementSemaphore.Dispose();

        _logger.LogInformation("JobManager disposed");
    }
}

/// <summary>
/// Internal job representation
/// </summary>
internal class Job
{
    public string Id { get; set; } = string.Empty;
    public JobConfiguration Configuration { get; set; } = new();
    public JobStatus Status { get; set; } = new();
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public CancellationTokenSource? PauseTokenSource { get; set; } = new();
    public Task? CompletionTask { get; set; }
}
