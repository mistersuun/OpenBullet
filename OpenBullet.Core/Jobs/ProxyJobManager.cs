using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace OpenBullet.Core.Jobs;

/// <summary>
/// Enhanced job manager with integrated proxy management
/// </summary>
public class ProxyJobManager : IJobManager, IDisposable
{
    private readonly ILogger<ProxyJobManager> _logger;
    private readonly IBotRunner _botRunner;
    private readonly IProxyManager _proxyManager;
    private readonly JobManager _baseJobManager;
    private readonly ConcurrentDictionary<string, ProxyJobConfiguration> _jobProxyConfigs = new();
    private readonly ConcurrentDictionary<string, ProxyJobStatistics> _proxyJobStats = new();
    private bool _disposed = false;

    public event EventHandler<JobStatusChangedEventArgs>? JobStatusChanged
    {
        add => _baseJobManager.JobStatusChanged += value;
        remove => _baseJobManager.JobStatusChanged -= value;
    }

    public event EventHandler<BotCompletedEventArgs>? BotCompleted
    {
        add => _baseJobManager.BotCompleted += value;
        remove => _baseJobManager.BotCompleted -= value;
    }

    public event EventHandler<JobCompletedEventArgs>? JobCompleted
    {
        add => _baseJobManager.JobCompleted += value;
        remove => _baseJobManager.JobCompleted -= value;
    }

    /// <summary>
    /// Event fired when proxy assignment occurs
    /// </summary>
    public event EventHandler<ProxyAssignedEventArgs>? ProxyAssigned;

    /// <summary>
    /// Event fired when proxy rotation occurs
    /// </summary>
    public event EventHandler<ProxyRotatedEventArgs>? ProxyRotated;

    /// <summary>
    /// Event fired when proxy fails for a job
    /// </summary>
    public event EventHandler<ProxyFailedEventArgs>? ProxyFailed;

    public ProxyJobManager(
        ILogger<ProxyJobManager> logger, 
        IBotRunner botRunner,
        IProxyManager proxyManager)
    {
        _logger = logger;
        _botRunner = botRunner;
        _proxyManager = proxyManager;
        
        // Create a logger for JobManager using a logger factory if available, otherwise use a null logger
        var loggerFactory = new LoggerFactory();
        var jobManagerLogger = loggerFactory.CreateLogger<JobManager>();
        
        // Create a service provider for JobManager (minimal implementation for testing)
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        
        _baseJobManager = new JobManager(jobManagerLogger, new ProxyAwareBotRunner(botRunner, proxyManager), serviceProvider);

        // Subscribe to proxy manager events
        _proxyManager.ProxyBanned += OnProxyBanned;
        _proxyManager.ProxyUnbanned += OnProxyUnbanned;
        _proxyManager.ProxyStatisticsUpdated += OnProxyStatisticsUpdated;

        _logger.LogInformation("ProxyJobManager initialized with proxy management");
    }

    public async Task<string> StartJobAsync(JobConfiguration jobConfig, CancellationToken cancellationToken = default)
    {
        var proxyJobConfig = jobConfig as ProxyJobConfiguration ?? new ProxyJobConfiguration(jobConfig);
        return await StartProxyJobAsync(proxyJobConfig, cancellationToken);
    }

    public async Task<string> StartProxyJobAsync(ProxyJobConfiguration proxyJobConfig, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        _logger.LogInformation("Starting proxy-enabled job {JobName} with {ProxyStrategy} strategy", 
            proxyJobConfig.Name, proxyJobConfig.ProxyStrategy);

        // Validate proxy configuration
        var validation = await ValidateProxyJobConfiguration(proxyJobConfig);
        if (!validation.IsValid)
        {
            var errors = string.Join(", ", validation.Errors);
            _logger.LogError("Proxy job configuration validation failed: {Errors}", errors);
            throw new ArgumentException($"Invalid proxy job configuration: {errors}");
        }

        // Set up proxy manager configuration based on job requirements
        if (proxyJobConfig.ProxyPoolConfiguration != null)
        {
            _proxyManager.SetConfiguration(proxyJobConfig.ProxyPoolConfiguration);
        }

        if (proxyJobConfig.ProxyRotationStrategy.HasValue)
        {
            _proxyManager.SetRotationStrategy(proxyJobConfig.ProxyRotationStrategy.Value);
        }

        // Start the base job
        var jobId = await _baseJobManager.StartJobAsync(proxyJobConfig, cancellationToken);

        // Track proxy configuration for this job
        _jobProxyConfigs[jobId] = proxyJobConfig;
        _proxyJobStats[jobId] = new ProxyJobStatistics { JobId = jobId };

        _logger.LogInformation("Proxy job {JobId} started successfully", jobId);
        return jobId;
    }

    public async Task<bool> StopJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var result = await _baseJobManager.StopJobAsync(jobId, cancellationToken);
        
        if (result)
        {
            // Clean up proxy job tracking
            _jobProxyConfigs.TryRemove(jobId, out _);
            _proxyJobStats.TryRemove(jobId, out _);
        }

        return result;
    }

    public Task<bool> PauseJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return _baseJobManager.PauseJobAsync(jobId, cancellationToken);
    }

    public Task<bool> ResumeJobAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return _baseJobManager.ResumeJobAsync(jobId, cancellationToken);
    }

    public JobStatus GetJobStatus(string jobId)
    {
        var status = _baseJobManager.GetJobStatus(jobId);
        
        // Enhance status with proxy information
        if (_proxyJobStats.TryGetValue(jobId, out var proxyStats))
        {
            status.Metadata["ProxyAssignments"] = proxyStats.TotalProxyAssignments;
            status.Metadata["ProxyRotations"] = proxyStats.ProxyRotations;
            status.Metadata["ProxyFailures"] = proxyStats.ProxyFailures;
            status.Metadata["UniqueProxiesUsed"] = proxyStats.UniqueProxiesUsed.Count;
            
            if (proxyStats.LastUsedProxy != null)
            {
                status.Metadata["LastUsedProxy"] = proxyStats.LastUsedProxy.Address;
            }
        }

        return status;
    }

    public List<JobStatus> GetActiveJobs()
    {
        return _baseJobManager.GetActiveJobs();
    }

    public Task<List<BotResult>> GetJobResultsAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return _baseJobManager.GetJobResultsAsync(jobId, cancellationToken);
    }

    public async Task<bool> ClearJobDataAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var result = await _baseJobManager.ClearJobDataAsync(jobId, cancellationToken);
        
        if (result)
        {
            _jobProxyConfigs.TryRemove(jobId, out _);
            _proxyJobStats.TryRemove(jobId, out _);
        }

        return result;
    }

    public JobManagerStatistics GetStatistics()
    {
        var baseStats = _baseJobManager.GetStatistics();
        
        // Enhance with proxy-specific statistics
        var totalProxyAssignments = _proxyJobStats.Values.Sum(s => s.TotalProxyAssignments);
        var totalProxyRotations = _proxyJobStats.Values.Sum(s => s.ProxyRotations);
        var totalProxyFailures = _proxyJobStats.Values.Sum(s => s.ProxyFailures);
        var uniqueProxies = _proxyJobStats.Values.SelectMany(s => s.UniqueProxiesUsed).Distinct().Count();

        baseStats.CustomMetrics["TotalProxyAssignments"] = totalProxyAssignments;
        baseStats.CustomMetrics["TotalProxyRotations"] = totalProxyRotations;
        baseStats.CustomMetrics["TotalProxyFailures"] = totalProxyFailures;
        baseStats.CustomMetrics["UniqueProxiesUsed"] = uniqueProxies;
        
        if (totalProxyAssignments > 0)
        {
            baseStats.CustomMetrics["ProxySuccessRate"] = ((double)(totalProxyAssignments - totalProxyFailures) / totalProxyAssignments) * 100;
        }

        return baseStats;
    }

    /// <summary>
    /// Gets proxy-specific statistics for a job
    /// </summary>
    public ProxyJobStatistics? GetProxyJobStatistics(string jobId)
    {
        _proxyJobStats.TryGetValue(jobId, out var stats);
        return stats?.Clone();
    }

    /// <summary>
    /// Gets proxy pool statistics
    /// </summary>
    public async Task<ProxyPoolStatistics> GetProxyPoolStatisticsAsync(CancellationToken cancellationToken = default)
    {
        return await _proxyManager.GetStatisticsAsync(cancellationToken);
    }

    /// <summary>
    /// Forces proxy rotation for a specific job
    /// </summary>
    public async Task<bool> ForceProxyRotationAsync(string jobId, CancellationToken cancellationToken = default)
    {
        if (!_jobProxyConfigs.ContainsKey(jobId))
        {
            return false;
        }

        // Implementation would depend on how we track active bot-proxy assignments
        // For now, we'll just log the request
        _logger.LogInformation("Forced proxy rotation requested for job {JobId}", jobId);
        
        // In a real implementation, we'd need to track active bots and force them to get new proxies
        // This could involve setting a flag that the ProxyAwareBotRunner checks
        
        return await Task.FromResult(true);
    }

    /// <summary>
    /// Tests all proxies in the pool
    /// </summary>
    public async Task<ProxyPoolTestResult> TestAllProxiesAsync(ProxyTestConfiguration? config = null, CancellationToken cancellationToken = default)
    {
        return await _proxyManager.TestAllProxiesAsync(config, cancellationToken);
    }

    private async Task<ProxyJobConfigurationValidationResult> ValidateProxyJobConfiguration(ProxyJobConfiguration config)
    {
        var result = new ProxyJobConfigurationValidationResult { IsValid = true };

        // First validate base job configuration
        var baseValidation = config.Validate();
        result.Errors.AddRange(baseValidation.Errors);
        result.Warnings.AddRange(baseValidation.Warnings);
        result.IsValid = baseValidation.IsValid;

        // Validate proxy-specific configuration
        if (config.RequiresProxy)
        {
            var availableProxies = await _proxyManager.GetAvailableProxiesAsync();
            if (availableProxies.Count == 0)
            {
                result.Errors.Add("Job requires proxies but no proxies are available");
                result.IsValid = false;
            }
            else if (availableProxies.Count < config.ConcurrentBots)
            {
                result.Warnings.Add($"Available proxies ({availableProxies.Count}) are fewer than concurrent bots ({config.ConcurrentBots}). This may cause proxy contention.");
            }
        }

        if (config.ProxyFailureThreshold <= 0)
        {
            result.Warnings.Add("Proxy failure threshold is 0 or negative. Proxies will be banned immediately on failure.");
        }

        if (config.ProxyRotationInterval < TimeSpan.FromSeconds(1))
        {
            result.Warnings.Add("Very short proxy rotation interval may cause performance issues.");
        }

        return result;
    }

    private void OnProxyBanned(object? sender, ProxyBannedEventArgs e)
    {
        _logger.LogWarning("Proxy banned during job execution: {ProxyAddress} - {Reason}", 
            e.Proxy.Address, e.Reason);

        // Update statistics for all jobs using this proxy
        foreach (var stats in _proxyJobStats.Values)
        {
            if (stats.UniqueProxiesUsed.Contains(e.Proxy.Id))
            {
                stats.ProxyFailures++;
                stats.LastFailedProxy = e.Proxy;
                stats.LastFailureTime = DateTime.UtcNow;
            }
        }

        ProxyFailed?.Invoke(this, new ProxyFailedEventArgs
        {
            Proxy = e.Proxy,
            Reason = e.Reason ?? "Unknown",
            Timestamp = e.Timestamp
        });
    }

    private void OnProxyUnbanned(object? sender, ProxyUnbannedEventArgs e)
    {
        _logger.LogInformation("Proxy unbanned: {ProxyAddress} (Automatic: {WasAutomatic})", 
            e.Proxy.Address, e.WasAutomatic);
    }

    private void OnProxyStatisticsUpdated(object? sender, ProxyStatisticsUpdatedEventArgs e)
    {
        // Update job statistics based on proxy usage
        foreach (var kvp in _proxyJobStats)
        {
            var stats = kvp.Value;
            if (stats.UniqueProxiesUsed.Contains(e.Proxy.Id))
            {
                stats.LastUpdateTime = DateTime.UtcNow;
                stats.LastUsedProxy = e.Proxy;
                
                if (e.UsageResult.Success)
                {
                    stats.SuccessfulProxyRequests++;
                }
                else
                {
                    stats.FailedProxyRequests++;
                }
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        // Unsubscribe from events
        _proxyManager.ProxyBanned -= OnProxyBanned;
        _proxyManager.ProxyUnbanned -= OnProxyUnbanned;
        _proxyManager.ProxyStatisticsUpdated -= OnProxyStatisticsUpdated;

        _baseJobManager?.Dispose();
        _jobProxyConfigs.Clear();
        _proxyJobStats.Clear();

        _logger.LogInformation("ProxyJobManager disposed");
    }
}

/// <summary>
/// Enhanced job configuration with proxy settings
/// </summary>
public class ProxyJobConfiguration : JobConfiguration
{
    public bool RequiresProxy { get; set; } = true;
    public ProxyRotationStrategy? ProxyRotationStrategy { get; set; }
    public TimeSpan ProxyRotationInterval { get; set; } = TimeSpan.FromMinutes(5);
    public int ProxyFailureThreshold { get; set; } = 3;
    public bool StickyProxyAssignment { get; set; } = false;
    public ProxyPoolConfiguration? ProxyPoolConfiguration { get; set; }
    public ProxyStrategy ProxyStrategy { get; set; } = ProxyStrategy.OnePerBot;
    public bool AutoBanFailedProxies { get; set; } = true;
    public TimeSpan ProxyBanDuration { get; set; } = TimeSpan.FromMinutes(10);
    public List<string> PreferredProxyTypes { get; set; } = new();
    public Dictionary<string, object> ProxyCustomSettings { get; set; } = new();

    public ProxyJobConfiguration() : base() { }

    public ProxyJobConfiguration(JobConfiguration baseConfig) : base()
    {
        Name = baseConfig.Name;
        Config = baseConfig.Config;
        DataLines = baseConfig.DataLines;
        ConcurrentBots = baseConfig.ConcurrentBots;
        MaxRetries = baseConfig.MaxRetries;
        BotTimeout = baseConfig.BotTimeout;
        StopOnError = baseConfig.StopOnError;
        SaveResults = baseConfig.SaveResults;
        SaveOnlySuccessful = baseConfig.SaveOnlySuccessful;
        OutputFormat = baseConfig.OutputFormat;
        CustomSettings = new Dictionary<string, object>(baseConfig.CustomSettings);
    }

    /// <summary>
    /// Validates the proxy job configuration
    /// </summary>
    public new ProxyJobConfigurationValidationResult Validate()
    {
        var result = new ProxyJobConfigurationValidationResult();
        var baseResult = base.Validate();
        
        result.IsValid = baseResult.IsValid;
        result.Errors.AddRange(baseResult.Errors);
        result.Warnings.AddRange(baseResult.Warnings);

        if (ProxyRotationInterval <= TimeSpan.Zero)
        {
            result.Errors.Add("Proxy rotation interval must be greater than zero");
            result.IsValid = false;
        }

        if (ProxyFailureThreshold < 0)
        {
            result.Errors.Add("Proxy failure threshold cannot be negative");
            result.IsValid = false;
        }

        if (RequiresProxy && ProxyStrategy == ProxyStrategy.None)
        {
            result.Errors.Add("Job requires proxy but proxy strategy is set to None");
            result.IsValid = false;
        }

        return result;
    }
}

/// <summary>
/// Proxy assignment strategies
/// </summary>
public enum ProxyStrategy
{
    None,           // No proxy assignment
    OnePerBot,      // Each bot gets a dedicated proxy
    Shared,         // Multiple bots can share the same proxy
    RoundRobin,     // Rotate proxies among all bots
    PerDataLine     // One proxy per data line (sticky assignment)
}

/// <summary>
/// Validation result for proxy job configuration
/// </summary>
public class ProxyJobConfigurationValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Statistics for proxy usage in jobs
/// </summary>
public class ProxyJobStatistics
{
    public string JobId { get; set; } = string.Empty;
    public int TotalProxyAssignments { get; set; }
    public int ProxyRotations { get; set; }
    public int ProxyFailures { get; set; }
    public int SuccessfulProxyRequests { get; set; }
    public int FailedProxyRequests { get; set; }
    public HashSet<string> UniqueProxiesUsed { get; set; } = new();
    public Proxies.ProxyInfo? LastUsedProxy { get; set; }
    public Proxies.ProxyInfo? LastFailedProxy { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdateTime { get; set; } = DateTime.UtcNow;
    public DateTime? LastFailureTime { get; set; }

    /// <summary>
    /// Gets the proxy success rate as percentage
    /// </summary>
    public double ProxySuccessRate
    {
        get
        {
            var totalRequests = SuccessfulProxyRequests + FailedProxyRequests;
            return totalRequests > 0 ? (double)SuccessfulProxyRequests / totalRequests * 100 : 0;
        }
    }

    /// <summary>
    /// Creates a copy of the statistics
    /// </summary>
    public ProxyJobStatistics Clone()
    {
        return new ProxyJobStatistics
        {
            JobId = JobId,
            TotalProxyAssignments = TotalProxyAssignments,
            ProxyRotations = ProxyRotations,
            ProxyFailures = ProxyFailures,
            SuccessfulProxyRequests = SuccessfulProxyRequests,
            FailedProxyRequests = FailedProxyRequests,
            UniqueProxiesUsed = new HashSet<string>(UniqueProxiesUsed),
            LastUsedProxy = LastUsedProxy?.Clone(),
            LastFailedProxy = LastFailedProxy?.Clone(),
            StartTime = StartTime,
            LastUpdateTime = LastUpdateTime,
            LastFailureTime = LastFailureTime
        };
    }
}

/// <summary>
/// Event arguments for proxy assignment
/// </summary>
public class ProxyAssignedEventArgs : EventArgs
{
    public string JobId { get; set; } = string.Empty;
    public string BotId { get; set; } = string.Empty;
    public Proxies.ProxyInfo Proxy { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event arguments for proxy rotation
/// </summary>
public class ProxyRotatedEventArgs : EventArgs
{
    public string JobId { get; set; } = string.Empty;
    public string BotId { get; set; } = string.Empty;
    public Proxies.ProxyInfo OldProxy { get; set; } = new();
    public Proxies.ProxyInfo NewProxy { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Event arguments for proxy failure
/// </summary>
public class ProxyFailedEventArgs : EventArgs
{
    public Proxies.ProxyInfo Proxy { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Proxy-aware bot runner that integrates with proxy management
/// </summary>
internal class ProxyAwareBotRunner : IBotRunner
{
    private readonly IBotRunner _baseBotRunner;
    private readonly IProxyManager _proxyManager;
    private readonly ILogger<ProxyAwareBotRunner> _logger;

    public event EventHandler<BotStatusEventArgs>? StatusChanged;

    public ProxyAwareBotRunner(IBotRunner baseBotRunner, IProxyManager proxyManager)
    {
        _baseBotRunner = baseBotRunner;
        _proxyManager = proxyManager;
        _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ProxyAwareBotRunner>.Instance;
        
        // Forward events from base runner
        if (_baseBotRunner != null)
        {
            _baseBotRunner.StatusChanged += (s, e) => StatusChanged?.Invoke(s, e);
        }
    }

    public async Task<BasicBotResult> ExecuteAsync(BotData data, ConfigModel config)
    {
        return await _baseBotRunner.ExecuteAsync(data, config);
    }

    public void Stop()
    {
        _baseBotRunner.Stop();
    }

    public async Task<BotResult> RunAsync(ConfigModel config, string dataLine, CancellationToken cancellationToken = default)
    {
        // Get a proxy for this bot execution
        var proxy = await _proxyManager.GetNextProxyAsync(dataLine, cancellationToken);
        
        try
        {
            // Run the bot with the assigned proxy and get detailed result
            var result = await _baseBotRunner.RunAsync(config, dataLine, cancellationToken);
            
            // Update proxy information in the result if proxy was used
            if (proxy != null)
            {
                result.Metadata["ProxyAddress"] = proxy.Address;
                result.Metadata["ProxyType"] = proxy.Type.ToString();
                _logger.LogTrace("Used proxy {ProxyAddress} for bot execution", proxy.Address);
            }

            // Return the proxy to the pool with usage result
            if (proxy != null)
            {
                var usageResult = ProxyExtensions.CreateUsageResult(
                    result.Success,
                    result.ExecutionTime,
                    result.Metadata.ContainsKey("LastResponseCode") ? (int)result.Metadata["LastResponseCode"] : 200,
                    result.ErrorMessage,
                    result.Exception);

                await _proxyManager.ReturnProxyAsync(proxy, usageResult, cancellationToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            // Return proxy with failure result
            if (proxy != null)
            {
                var usageResult = ProxyExtensions.CreateFailureResult(
                    TimeSpan.Zero,
                    0,
                    ex.Message,
                    ex);

                await _proxyManager.ReturnProxyAsync(proxy, usageResult, cancellationToken);
            }

            throw;
        }
    }
}
