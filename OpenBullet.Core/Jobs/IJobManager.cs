using OpenBullet.Core.Execution;
using OpenBullet.Core.Models;
using OpenBullet.Core.Interfaces;

namespace OpenBullet.Core.Jobs;

/// <summary>
/// Interface for job management and execution
/// </summary>
public interface IJobManager
{
    /// <summary>
    /// Starts a new job
    /// </summary>
    Task<string> StartJobAsync(JobConfiguration jobConfig, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops a running job
    /// </summary>
    Task<bool> StopJobAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses a running job
    /// </summary>
    Task<bool> PauseJobAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes a paused job
    /// </summary>
    Task<bool> ResumeJobAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets job status and statistics
    /// </summary>
    JobStatus GetJobStatus(string jobId);

    /// <summary>
    /// Gets all active jobs
    /// </summary>
    List<JobStatus> GetActiveJobs();

    /// <summary>
    /// Gets job results
    /// </summary>
    Task<List<BotResult>> GetJobResultsAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears completed job data
    /// </summary>
    Task<bool> ClearJobDataAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overall manager statistics
    /// </summary>
    JobManagerStatistics GetStatistics();

    /// <summary>
    /// Event for job status changes
    /// </summary>
    event EventHandler<JobStatusChangedEventArgs>? JobStatusChanged;

    /// <summary>
    /// Event for bot completion
    /// </summary>
    event EventHandler<BotCompletedEventArgs>? BotCompleted;

    /// <summary>
    /// Event for job completion
    /// </summary>
    event EventHandler<JobCompletedEventArgs>? JobCompleted;
}

/// <summary>
/// Configuration for job execution
/// </summary>
public class JobConfiguration
{
    public string Name { get; set; } = string.Empty;
    public ConfigModel Config { get; set; } = new();
    public List<string> DataLines { get; set; } = new();
    public int ConcurrentBots { get; set; } = 1;
    public int MaxRetries { get; set; } = 0;
    public TimeSpan BotTimeout { get; set; } = TimeSpan.FromMinutes(5);
    public bool StopOnError { get; set; } = false;
    public bool SaveResults { get; set; } = true;
    public bool SaveOnlySuccessful { get; set; } = false;
    public string? OutputFormat { get; set; }
    public Dictionary<string, object> CustomSettings { get; set; } = new();

    /// <summary>
    /// Validates the job configuration
    /// </summary>
    public JobConfigurationValidationResult Validate()
    {
        var result = new JobConfigurationValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(Name))
        {
            result.Errors.Add("Job name is required");
            result.IsValid = false;
        }

        if (Config == null)
        {
            result.Errors.Add("Configuration is required");
            result.IsValid = false;
        }
        else if (string.IsNullOrWhiteSpace(Config.Script))
        {
            result.Errors.Add("Configuration script is empty");
            result.IsValid = false;
        }

        if (DataLines.Count == 0)
        {
            result.Errors.Add("At least one data line is required");
            result.IsValid = false;
        }

        if (ConcurrentBots <= 0)
        {
            result.Errors.Add("Concurrent bots must be greater than zero");
            result.IsValid = false;
        }
        else if (ConcurrentBots > 1000)
        {
            result.Warnings.Add("High concurrent bot count may cause performance issues");
        }

        if (BotTimeout <= TimeSpan.Zero)
        {
            result.Errors.Add("Bot timeout must be greater than zero");
            result.IsValid = false;
        }
        else if (BotTimeout > TimeSpan.FromHours(1))
        {
            result.Warnings.Add("Very long bot timeout may cause jobs to hang");
        }

        return result;
    }
}

/// <summary>
/// Validation result for job configuration
/// </summary>
public class JobConfigurationValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Status of a job execution
/// </summary>
public class JobStatus
{
    public string JobId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public JobState State { get; set; } = JobState.Created;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public int TotalDataLines { get; set; }
    public int ProcessedDataLines { get; set; }
    public int SuccessfulBots { get; set; }
    public int FailedBots { get; set; }
    public int RetryBots { get; set; }
    public int BannedBots { get; set; }
    public int ErrorBots { get; set; }
    public int CustomStatusBots { get; set; }
    public int ActiveBots { get; set; }
    public double ProgressPercentage { get; set; }
    public double BotsPerMinute { get; set; }
    public TimeSpan EstimatedTimeRemaining { get; set; }
    public string? LastErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets the completion percentage
    /// </summary>
    public double GetCompletionPercentage()
    {
        return TotalDataLines > 0 ? (double)ProcessedDataLines / TotalDataLines * 100 : 0;
    }

    /// <summary>
    /// Gets the success rate percentage
    /// </summary>
    public double GetSuccessRate()
    {
        var completedBots = ProcessedDataLines;
        return completedBots > 0 ? (double)SuccessfulBots / completedBots * 100 : 0;
    }

    /// <summary>
    /// Checks if the job is running
    /// </summary>
    public bool IsRunning => State == JobState.Running;

    /// <summary>
    /// Checks if the job is completed
    /// </summary>
    public bool IsCompleted => State == JobState.Completed || State == JobState.Cancelled || State == JobState.Failed;

    /// <summary>
    /// Updates progress statistics
    /// </summary>
    public void UpdateProgress()
    {
        ProgressPercentage = GetCompletionPercentage();
        
        // Calculate ElapsedTime if not already set
        if (StartTime != default)
        {
            var currentElapsedTime = ElapsedTime;
            if (currentElapsedTime == TimeSpan.Zero)
            {
                var endTime = EndTime ?? DateTime.UtcNow;
                currentElapsedTime = endTime - StartTime;
            }
            
            if (currentElapsedTime.TotalMinutes > 0)
            {
                BotsPerMinute = ProcessedDataLines / currentElapsedTime.TotalMinutes;
                
                var remainingBots = TotalDataLines - ProcessedDataLines;
                if (BotsPerMinute > 0)
                {
                    EstimatedTimeRemaining = TimeSpan.FromMinutes(remainingBots / BotsPerMinute);
                }
            }
        }
    }
}

/// <summary>
/// Job execution states
/// </summary>
public enum JobState
{
    Created,
    Starting,
    Running,
    Paused,
    Stopping,
    Completed,
    Cancelled,
    Failed
}

/// <summary>
/// Statistics for the job manager
/// </summary>
public class JobManagerStatistics
{
    public int TotalJobsCreated { get; set; }
    public int ActiveJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int FailedJobs { get; set; }
    public int CancelledJobs { get; set; }
    public long TotalBotsExecuted { get; set; }
    public long TotalSuccessfulBots { get; set; }
    public long TotalFailedBots { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public double AverageBotsPerMinute { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? LastJobStartTime { get; set; }
    public Dictionary<string, object> CustomMetrics { get; set; } = new();

    /// <summary>
    /// Gets overall success rate
    /// </summary>
    public double GetOverallSuccessRate()
    {
        return TotalBotsExecuted > 0 ? (double)TotalSuccessfulBots / TotalBotsExecuted * 100 : 0;
    }

    /// <summary>
    /// Gets job completion rate
    /// </summary>
    public double GetJobCompletionRate()
    {
        var totalJobs = CompletedJobs + FailedJobs + CancelledJobs;
        return totalJobs > 0 ? (double)CompletedJobs / totalJobs * 100 : 0;
    }
}

/// <summary>
/// Event arguments for job status changes
/// </summary>
public class JobStatusChangedEventArgs : EventArgs
{
    public string JobId { get; set; } = string.Empty;
    public JobState OldState { get; set; }
    public JobState NewState { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Message { get; set; }
}

/// <summary>
/// Event arguments for bot completion
/// </summary>
public class BotCompletedEventArgs : EventArgs
{
    public string JobId { get; set; } = string.Empty;
    public BasicBotResult Result { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event arguments for job completion
/// </summary>
public class JobCompletedEventArgs : EventArgs
{
    public string JobId { get; set; } = string.Empty;
    public JobStatus FinalStatus { get; set; } = new();
    public List<BasicBotResult> Results { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool WasSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}
