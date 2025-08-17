using OpenBullet.Core.Execution;
using OpenBullet.Core.Models;

namespace OpenBullet.Core.Data;

/// <summary>
/// Interface for configuration storage operations
/// </summary>
public interface IConfigurationStorage
{
    /// <summary>
    /// Gets all configurations
    /// </summary>
    Task<IEnumerable<ConfigurationEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a configuration by ID
    /// </summary>
    Task<ConfigurationEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets configurations by category
    /// </summary>
    Task<IEnumerable<ConfigurationEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches configurations by name or description
    /// </summary>
    Task<IEnumerable<ConfigurationEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets configurations with pagination
    /// </summary>
    Task<PagedResult<ConfigurationEntity>> GetPagedAsync(int pageNumber, int pageSize, string? category = null, string? searchTerm = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a configuration
    /// </summary>
    Task<ConfigurationEntity> SaveAsync(ConfigurationEntity configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a configuration from a ConfigModel
    /// </summary>
    Task<ConfigurationEntity> CreateFromModelAsync(ConfigModel config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a configuration from a ConfigModel
    /// </summary>
    Task<ConfigurationEntity> UpdateFromModelAsync(string id, ConfigModel config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a configuration
    /// </summary>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets configuration usage statistics
    /// </summary>
    Task<ConfigurationUsageStats> GetUsageStatsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates configuration usage statistics
    /// </summary>
    Task UpdateUsageAsync(string id, bool success, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports configurations to file
    /// </summary>
    Task<bool> ExportAsync(IEnumerable<string> configurationIds, string filePath, ExportFormat format, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports configurations from file
    /// </summary>
    Task<ImportResult> ImportAsync(string filePath, bool overwriteExisting = false, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for job storage operations
/// </summary>
public interface IJobStorage
{
    /// <summary>
    /// Gets all jobs
    /// </summary>
    Task<IEnumerable<JobEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a job by ID
    /// </summary>
    Task<JobEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets jobs with pagination
    /// </summary>
    Task<PagedResult<JobEntity>> GetPagedAsync(int pageNumber, int pageSize, JobFilter? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent jobs
    /// </summary>
    Task<IEnumerable<JobEntity>> GetRecentAsync(int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets running jobs
    /// </summary>
    Task<IEnumerable<JobEntity>> GetRunningAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a job
    /// </summary>
    Task<JobEntity> SaveAsync(JobEntity job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a job from job configuration
    /// </summary>
    Task<JobEntity> CreateFromConfigurationAsync(string configurationId, Jobs.JobConfiguration jobConfig, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates job status
    /// </summary>
    Task UpdateStatusAsync(string id, JobStatus status, string? errorMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates job progress
    /// </summary>
    Task UpdateProgressAsync(string id, int processedItems, int successfulItems, int failedItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a job
    /// </summary>
    Task CompleteJobAsync(string id, JobStatus finalStatus, TimeSpan duration, string? errorMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a job and its results
    /// </summary>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets job statistics
    /// </summary>
    Task<JobStatistics> GetStatisticsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overall job statistics
    /// </summary>
    Task<OverallJobStatistics> GetOverallStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up old completed jobs
    /// </summary>
    Task<int> CleanupOldJobsAsync(TimeSpan olderThan, bool keepResults = false, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for job result storage operations
/// </summary>
public interface IResultStorage
{
    /// <summary>
    /// Gets all results for a job
    /// </summary>
    Task<IEnumerable<JobResultEntity>> GetByJobIdAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets results with pagination
    /// </summary>
    Task<PagedResult<JobResultEntity>> GetPagedAsync(string jobId, int pageNumber, int pageSize, ResultFilter? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets successful results for a job
    /// </summary>
    Task<IEnumerable<JobResultEntity>> GetSuccessfulAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed results for a job
    /// </summary>
    Task<IEnumerable<JobResultEntity>> GetFailedAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a job result
    /// </summary>
    Task<JobResultEntity> SaveAsync(JobResultEntity result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves multiple job results
    /// </summary>
    Task<IEnumerable<JobResultEntity>> SaveRangeAsync(IEnumerable<JobResultEntity> results, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a result from BotResult
    /// </summary>
    Task<JobResultEntity> CreateFromBotResultAsync(string jobId, BotResult botResult, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes results for a job
    /// </summary>
    Task<int> DeleteByJobIdAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets result statistics for a job
    /// </summary>
    Task<ResultStatistics> GetStatisticsAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports results to file
    /// </summary>
    Task<bool> ExportAsync(string jobId, string filePath, ExportFormat format, ResultFilter? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches results by captured data
    /// </summary>
    Task<IEnumerable<JobResultEntity>> SearchByCapturedDataAsync(string jobId, string key, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets distinct captured data keys for a job
    /// </summary>
    Task<IEnumerable<string>> GetCapturedDataKeysAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets distinct captured data values for a key
    /// </summary>
    Task<IEnumerable<string>> GetCapturedDataValuesAsync(string jobId, string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for proxy storage operations
/// </summary>
public interface IProxyStorage
{
    /// <summary>
    /// Gets all proxies
    /// </summary>
    Task<IEnumerable<ProxyEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available (active and not banned) proxies
    /// </summary>
    Task<IEnumerable<ProxyEntity>> GetAvailableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets proxies with pagination
    /// </summary>
    Task<PagedResult<ProxyEntity>> GetPagedAsync(int pageNumber, int pageSize, ProxyFilter? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a proxy
    /// </summary>
    Task<ProxyEntity> SaveAsync(ProxyEntity proxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves multiple proxies
    /// </summary>
    Task<IEnumerable<ProxyEntity>> SaveRangeAsync(IEnumerable<ProxyEntity> proxies, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates proxies from proxy strings
    /// </summary>
    Task<ImportResult> ImportFromStringsAsync(IEnumerable<string> proxyStrings, string? source = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates proxy statistics
    /// </summary>
    Task UpdateStatisticsAsync(string id, bool success, TimeSpan responseTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bans a proxy
    /// </summary>
    Task BanProxyAsync(string id, TimeSpan? duration = null, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unbans a proxy
    /// </summary>
    Task UnbanProxyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a proxy
    /// </summary>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets proxy statistics
    /// </summary>
    Task<ProxyStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up dead or old proxies
    /// </summary>
    Task<int> CleanupAsync(TimeSpan? olderThan = null, bool removeDeadProxies = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports proxies to file
    /// </summary>
    Task<bool> ExportAsync(string filePath, ExportFormat format, ProxyFilter? filter = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for settings storage operations
/// </summary>
public interface ISettingsStorage
{
    /// <summary>
    /// Gets all settings
    /// </summary>
    Task<IEnumerable<SettingEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets settings by category
    /// </summary>
    Task<IEnumerable<SettingEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a setting value
    /// </summary>
    Task<T?> GetValueAsync<T>(string key, T? defaultValue = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a setting value
    /// </summary>
    Task SetValueAsync<T>(string key, T value, string? description = null, string category = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a setting
    /// </summary>
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports settings to file
    /// </summary>
    Task<bool> ExportAsync(string filePath, string? category = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports settings from file
    /// </summary>
    Task<ImportResult> ImportAsync(string filePath, bool overwriteExisting = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets settings to defaults
    /// </summary>
    Task ResetToDefaultsAsync(string? category = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Filters for different entity types
/// </summary>
public class JobFilter
{
    public JobStatus? Status { get; set; }
    public string? ConfigurationId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public bool? UseProxies { get; set; }
    public string? SearchTerm { get; set; }
}

public class ResultFilter
{
    public BotStatus? Status { get; set; }
    public bool? Success { get; set; }
    public string? ProxyUsed { get; set; }
    public int? ResponseCodeFrom { get; set; }
    public int? ResponseCodeTo { get; set; }
    public TimeSpan? ExecutionTimeFrom { get; set; }
    public TimeSpan? ExecutionTimeTo { get; set; }
    public string? SearchTerm { get; set; }
    public Dictionary<string, string> CapturedDataFilters { get; set; } = new();
}

public class ProxyFilter
{
    public ProxyType? Type { get; set; }
    public ProxyHealth? Health { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsBanned { get; set; }
    public string? Country { get; set; }
    public string? Source { get; set; }
    public double? MinSuccessRate { get; set; }
    public long? MaxResponseTime { get; set; }
}

/// <summary>
/// Statistics models
/// </summary>
public class ConfigurationUsageStats
{
    public int TotalJobs { get; set; }
    public int SuccessfulJobs { get; set; }
    public DateTime? LastUsed { get; set; }
    public double? AverageSuccessRate { get; set; }
    public TimeSpan? AverageExecutionTime { get; set; }
}

public class JobStatistics
{
    public string JobId { get; set; } = string.Empty;
    public TimeSpan? Duration { get; set; }
    public double? ItemsPerMinute { get; set; }
    public Dictionary<BotStatus, int> StatusCounts { get; set; } = new();
    public Dictionary<string, int> ProxyUsage { get; set; } = new();
    public TimeSpan? AverageResponseTime { get; set; }
}

public class OverallJobStatistics
{
    public int TotalJobs { get; set; }
    public int RunningJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int FailedJobs { get; set; }
    public long TotalItemsProcessed { get; set; }
    public double OverallSuccessRate { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
}

public class ResultStatistics
{
    public int TotalResults { get; set; }
    public int SuccessfulResults { get; set; }
    public int FailedResults { get; set; }
    public Dictionary<BotStatus, int> StatusCounts { get; set; } = new();
    public TimeSpan AverageExecutionTime { get; set; }
    public Dictionary<string, int> CapturedDataSummary { get; set; } = new();
}

public class ProxyStatistics
{
    public int TotalProxies { get; set; }
    public int ActiveProxies { get; set; }
    public int BannedProxies { get; set; }
    public Dictionary<ProxyHealth, int> HealthDistribution { get; set; } = new();
    public Dictionary<ProxyType, int> TypeDistribution { get; set; } = new();
    public double OverallSuccessRate { get; set; }
    public long AverageResponseTime { get; set; }
}

/// <summary>
/// Export formats
/// </summary>
public enum ExportFormat
{
    JSON,
    CSV,
    XML,
    Excel
}

/// <summary>
/// Import/Export results
/// </summary>
public class ImportResult
{
    public bool Success { get; set; }
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
