using Microsoft.Extensions.Logging;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Jobs;
using OpenBullet.Core.Models;
using System.Globalization;
using System.Text;

namespace OpenBullet.Core.Data;

/// <summary>
/// Configuration storage service implementation
/// </summary>
public class ConfigurationStorage : IConfigurationStorage
{
    private readonly IOpenBulletContext _context;
    private readonly ILogger<ConfigurationStorage> _logger;

    public ConfigurationStorage(IOpenBulletContext context, ILogger<ConfigurationStorage> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ConfigurationEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Configurations.GetAllAsync(cancellationToken);
    }

    public async Task<ConfigurationEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Configurations.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<ConfigurationEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Configurations.FindAsync(c => c.Category == category, cancellationToken);
    }

    public async Task<IEnumerable<ConfigurationEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _context.Configurations.GetAllAsync(cancellationToken);
        }
        
        var lowerTerm = searchTerm.ToLowerInvariant();
        return await _context.Configurations.FindAsync(
            c => c.Name.ToLower().Contains(lowerTerm) || 
                 (c.Description != null && c.Description.ToLower().Contains(lowerTerm)),
            cancellationToken);
    }

    public async Task<PagedResult<ConfigurationEntity>> GetPagedAsync(int pageNumber, int pageSize, string? category = null, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(searchTerm))
        {
            var lowerTerm = searchTerm.ToLowerInvariant();
            return await _context.Configurations.GetPagedAsync(pageNumber, pageSize,
                c => c.Category == category && 
                     (c.Name.ToLower().Contains(lowerTerm) || 
                      (c.Description != null && c.Description.ToLower().Contains(lowerTerm))),
                cancellationToken);
        }
        else if (!string.IsNullOrEmpty(category))
        {
            return await _context.Configurations.GetPagedAsync(pageNumber, pageSize,
                c => c.Category == category, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerTerm = searchTerm.ToLowerInvariant();
            return await _context.Configurations.GetPagedAsync(pageNumber, pageSize,
                c => c.Name.ToLower().Contains(lowerTerm) || 
                     (c.Description != null && c.Description.ToLower().Contains(lowerTerm)),
                cancellationToken);
        }

        return await _context.Configurations.GetPagedAsync(pageNumber, pageSize, cancellationToken: cancellationToken);
    }

    public async Task<ConfigurationEntity> SaveAsync(ConfigurationEntity configuration, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Configurations.GetByIdAsync(configuration.Id, cancellationToken);
        
        if (existing == null)
        {
            return await _context.Configurations.AddAsync(configuration, cancellationToken);
        }
        else
        {
            return await _context.Configurations.UpdateAsync(configuration, cancellationToken);
        }
    }

    public async Task<ConfigurationEntity> CreateFromModelAsync(ConfigModel config, CancellationToken cancellationToken = default)
    {
        var entity = new ConfigurationEntity();
        entity.FromConfigModel(config);
        entity.CreatedAt = DateTime.UtcNow;
        
        return await _context.Configurations.AddAsync(entity, cancellationToken);
    }

    public async Task<ConfigurationEntity> UpdateFromModelAsync(string id, ConfigModel config, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Configurations.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            throw new InvalidOperationException($"Configuration with ID {id} not found");
        }

        entity.FromConfigModel(config);
        return await _context.Configurations.UpdateAsync(entity, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Configurations.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<ConfigurationUsageStats> GetUsageStatsAsync(string id, CancellationToken cancellationToken = default)
    {
        var jobs = await _context.Jobs.FindAsync(j => j.ConfigurationId == id, cancellationToken);
        var jobList = jobs.ToList();

        return new ConfigurationUsageStats
        {
            TotalJobs = jobList.Count,
            SuccessfulJobs = jobList.Count(j => j.Status == JobStatus.Completed),
            LastUsed = jobList.MaxBy(j => j.StartedAt)?.StartedAt,
            AverageSuccessRate = jobList.Where(j => j.SuccessRate.HasValue).Average(j => j.SuccessRate),
            AverageExecutionTime = jobList.Where(j => j.Duration.HasValue).Any() 
                ? TimeSpan.FromTicks((long)jobList.Where(j => j.Duration.HasValue).Average(j => j.Duration!.Value.Ticks))
                : null
        };
    }

    public async Task UpdateUsageAsync(string id, bool success, CancellationToken cancellationToken = default)
    {
        var config = await _context.Configurations.GetByIdAsync(id, cancellationToken);
        if (config != null)
        {
            config.UsageCount++;
            config.LastUsed = DateTime.UtcNow;
            await _context.Configurations.UpdateAsync(config, cancellationToken);
        }
    }

    public async Task<bool> ExportAsync(IEnumerable<string> configurationIds, string filePath, ExportFormat format, CancellationToken cancellationToken = default)
    {
        try
        {
            var configs = new List<ConfigurationEntity>();
            foreach (var id in configurationIds)
            {
                var config = await _context.Configurations.GetByIdAsync(id, cancellationToken);
                if (config != null)
                {
                    configs.Add(config);
                }
            }

            return await ExportHelper.ExportConfigurationsAsync(configs, filePath, format, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export configurations to {FilePath}", filePath);
            return false;
        }
    }

    public async Task<ImportResult> ImportAsync(string filePath, bool overwriteExisting = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var configs = await ExportHelper.ImportConfigurationsAsync(filePath, cancellationToken);
            var result = new ImportResult { Success = true };

            foreach (var config in configs)
            {
                try
                {
                    var existing = await _context.Configurations.GetSingleAsync(c => c.Name == config.Name, cancellationToken);
                    
                    if (existing != null && !overwriteExisting)
                    {
                        result.SkippedCount++;
                        result.Warnings.Add($"Configuration '{config.Name}' already exists and was skipped");
                        continue;
                    }

                    if (existing != null && overwriteExisting)
                    {
                        config.Id = existing.Id;
                        config.CreatedAt = existing.CreatedAt;
                        await _context.Configurations.UpdateAsync(config, cancellationToken);
                    }
                    else
                    {
                        await _context.Configurations.AddAsync(config, cancellationToken);
                    }

                    result.ImportedCount++;
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Failed to import '{config.Name}': {ex.Message}");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import configurations from {FilePath}", filePath);
            return new ImportResult
            {
                Success = false,
                Errors = { ex.Message }
            };
        }
    }
}

/// <summary>
/// Job storage service implementation
/// </summary>
public class JobStorage : IJobStorage
{
    private readonly IOpenBulletContext _context;
    private readonly ILogger<JobStorage> _logger;

    public JobStorage(IOpenBulletContext context, ILogger<JobStorage> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<JobEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Jobs.GetOrderedAsync(j => j.CreatedAt, true, cancellationToken);
    }

    public async Task<JobEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Jobs.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PagedResult<JobEntity>> GetPagedAsync(int pageNumber, int pageSize, JobFilter? filter = null, CancellationToken cancellationToken = default)
    {
        if (filter == null)
        {
            return await _context.Jobs.GetPagedAsync(pageNumber, pageSize, cancellationToken: cancellationToken);
        }

        return await _context.Jobs.GetPagedAsync(pageNumber, pageSize, j =>
            (filter.Status == null || j.Status == filter.Status) &&
            (filter.ConfigurationId == null || j.ConfigurationId == filter.ConfigurationId) &&
            (filter.StartDateFrom == null || j.StartedAt >= filter.StartDateFrom) &&
            (filter.StartDateTo == null || j.StartedAt <= filter.StartDateTo) &&
            (filter.UseProxies == null || j.UseProxies == filter.UseProxies) &&
            (filter.SearchTerm == null || j.Name.ToLower().Contains(filter.SearchTerm.ToLower())),
            cancellationToken);
    }

    public async Task<IEnumerable<JobEntity>> GetRecentAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var allJobs = await _context.Jobs.GetOrderedAsync(j => j.CreatedAt, true, cancellationToken);
        return allJobs.Take(count);
    }

    public async Task<IEnumerable<JobEntity>> GetRunningAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Jobs.FindAsync(j => 
            j.Status == JobStatus.Running || 
            j.Status == JobStatus.Starting, 
            cancellationToken);
    }

    public async Task<JobEntity> SaveAsync(JobEntity job, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Jobs.GetByIdAsync(job.Id, cancellationToken);
        
        if (existing == null)
        {
            return await _context.Jobs.AddAsync(job, cancellationToken);
        }
        else
        {
            return await _context.Jobs.UpdateAsync(job, cancellationToken);
        }
    }

    public async Task<JobEntity> CreateFromConfigurationAsync(string configurationId, JobConfiguration jobConfig, CancellationToken cancellationToken = default)
    {
        var entity = new JobEntity
        {
            Name = jobConfig.Name,
            ConfigurationId = configurationId,
            Status = JobStatus.Created,
            TotalItems = jobConfig.DataLines.Count,
            ConcurrentBots = jobConfig.ConcurrentBots,
            DataLinesJson = Newtonsoft.Json.JsonConvert.SerializeObject(jobConfig.DataLines),
            SettingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(jobConfig.CustomSettings),
            CreatedAt = DateTime.UtcNow
        };

        return await _context.Jobs.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateStatusAsync(string id, JobStatus status, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.GetByIdAsync(id, cancellationToken);
        if (job != null)
        {
            job.Status = status;
            job.ErrorMessage = errorMessage;
            
            if (status == JobStatus.Running && job.StartedAt == null)
            {
                job.StartedAt = DateTime.UtcNow;
            }

            await _context.Jobs.UpdateAsync(job, cancellationToken);
        }
    }

    public async Task UpdateProgressAsync(string id, int processedItems, int successfulItems, int failedItems, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.GetByIdAsync(id, cancellationToken);
        if (job != null)
        {
            job.ProcessedItems = processedItems;
            job.SuccessfulItems = successfulItems;
            job.FailedItems = failedItems;
            job.SuccessRate = processedItems > 0 ? (double)successfulItems / processedItems * 100 : 0;

            if (job.StartedAt.HasValue)
            {
                var elapsed = DateTime.UtcNow - job.StartedAt.Value;
                job.ItemsPerMinute = elapsed.TotalMinutes > 0 ? processedItems / elapsed.TotalMinutes : 0;
            }

            await _context.Jobs.UpdateAsync(job, cancellationToken);
        }
    }

    public async Task CompleteJobAsync(string id, JobStatus finalStatus, TimeSpan duration, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.GetByIdAsync(id, cancellationToken);
        if (job != null)
        {
            job.Status = finalStatus;
            job.CompletedAt = DateTime.UtcNow;
            job.Duration = duration;
            job.ErrorMessage = errorMessage;

            await _context.Jobs.UpdateAsync(job, cancellationToken);
        }
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        // Delete all job results first
        await _context.JobResults.DeleteAsync(r => r.JobId == id, cancellationToken);
        
        // Then delete the job
        return await _context.Jobs.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<JobStatistics> GetStatisticsAsync(string id, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs.GetByIdAsync(id, cancellationToken);
        if (job == null)
        {
            return new JobStatistics { JobId = id };
        }

        var results = await _context.JobResults.FindAsync(r => r.JobId == id, cancellationToken);
        var resultList = results.ToList();

        var stats = new JobStatistics
        {
            JobId = id,
            Duration = job.Duration,
            ItemsPerMinute = job.ItemsPerMinute
        };

        // Count results by status
        foreach (var result in resultList)
        {
            if (!stats.StatusCounts.ContainsKey(result.Status))
            {
                stats.StatusCounts[result.Status] = 0;
            }
            stats.StatusCounts[result.Status]++;
        }

        // Count proxy usage
        var proxyUsage = resultList
            .Where(r => !string.IsNullOrEmpty(r.ProxyUsed))
            .GroupBy(r => r.ProxyUsed!)
            .ToDictionary(g => g.Key, g => g.Count());
        
        stats.ProxyUsage = proxyUsage;

        // Calculate average response time
        var responseTimes = resultList
            .Where(r => r.ResponseTime.HasValue)
            .Select(r => r.ResponseTime!.Value)
            .ToList();

        if (responseTimes.Count > 0)
        {
            stats.AverageResponseTime = TimeSpan.FromMilliseconds(responseTimes.Average());
        }

        return stats;
    }

    public async Task<OverallJobStatistics> GetOverallStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var allJobs = await _context.Jobs.GetAllAsync(cancellationToken);
        var jobList = allJobs.ToList();

        return new OverallJobStatistics
        {
            TotalJobs = jobList.Count,
            RunningJobs = jobList.Count(j => j.IsRunning),
            CompletedJobs = jobList.Count(j => j.Status == JobStatus.Completed),
            FailedJobs = jobList.Count(j => j.Status == JobStatus.Failed),
            TotalItemsProcessed = jobList.Sum(j => j.ProcessedItems),
            OverallSuccessRate = jobList.Where(j => j.SuccessRate.HasValue).Any() 
                ? jobList.Where(j => j.SuccessRate.HasValue).Average(j => j.SuccessRate!.Value)
                : 0,
            TotalExecutionTime = jobList.Where(j => j.Duration.HasValue).Any()
                ? TimeSpan.FromTicks(jobList.Where(j => j.Duration.HasValue).Sum(j => j.Duration!.Value.Ticks))
                : TimeSpan.Zero
        };
    }

    public async Task<int> CleanupOldJobsAsync(TimeSpan olderThan, bool keepResults = false, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow - olderThan;
        var oldJobs = await _context.Jobs.FindAsync(j => 
            j.CompletedAt.HasValue && 
            j.CompletedAt.Value < cutoffDate, 
            cancellationToken);

        var count = 0;
        foreach (var job in oldJobs)
        {
            if (!keepResults)
            {
                await _context.JobResults.DeleteAsync(r => r.JobId == job.Id, cancellationToken);
            }
            
            if (await _context.Jobs.DeleteAsync(job, cancellationToken))
            {
                count++;
            }
        }

        _logger.LogInformation("Cleaned up {Count} old jobs", count);
        return count;
    }
}

/// <summary>
/// Export/Import helper class
/// </summary>
public static partial class ExportHelper
{
    public static async Task<bool> ExportConfigurationsAsync(IEnumerable<ConfigurationEntity> configs, string filePath, ExportFormat format, CancellationToken cancellationToken = default)
    {
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            switch (format)
            {
                case ExportFormat.JSON:
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(configs, Newtonsoft.Json.Formatting.Indented);
                    await File.WriteAllTextAsync(filePath, json, cancellationToken);
                    break;

                case ExportFormat.CSV:
                    await ExportConfigurationsToCsvAsync(configs, filePath, cancellationToken);
                    break;

                case ExportFormat.XML:
                    await ExportConfigurationsToXmlAsync(configs, filePath, cancellationToken);
                    break;

                default:
                    throw new ArgumentException($"Unsupported export format: {format}");
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<List<ConfigurationEntity>> ImportConfigurationsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Import file not found: {filePath}");
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var content = await File.ReadAllTextAsync(filePath, cancellationToken);

        return extension switch
        {
            ".json" => Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigurationEntity>>(content) ?? new List<ConfigurationEntity>(),
            ".csv" => await ImportConfigurationsFromCsvAsync(filePath, cancellationToken),
            ".xml" => await ImportConfigurationsFromXmlAsync(filePath, cancellationToken),
            _ => throw new ArgumentException($"Unsupported import format: {extension}")
        };
    }

    private static async Task ExportConfigurationsToCsvAsync(IEnumerable<ConfigurationEntity> configs, string filePath, CancellationToken cancellationToken)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(configs, cancellationToken);
    }

    private static async Task ExportConfigurationsToXmlAsync(IEnumerable<ConfigurationEntity> configs, string filePath, CancellationToken cancellationToken)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<ConfigurationEntity>));
        await using var stream = new FileStream(filePath, FileMode.Create);
        serializer.Serialize(stream, configs.ToList());
    }

    private static async Task<List<ConfigurationEntity>> ImportConfigurationsFromCsvAsync(string filePath, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
        
        var records = csv.GetRecordsAsync<ConfigurationEntity>(cancellationToken);
        var result = new List<ConfigurationEntity>();
        
        await foreach (var record in records)
        {
            result.Add(record);
        }
        
        return result;
    }

    private static async Task<List<ConfigurationEntity>> ImportConfigurationsFromXmlAsync(string filePath, CancellationToken cancellationToken)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<ConfigurationEntity>));
        await using var stream = new FileStream(filePath, FileMode.Open);
        var result = serializer.Deserialize(stream) as List<ConfigurationEntity>;
        return result ?? new List<ConfigurationEntity>();
    }
}
