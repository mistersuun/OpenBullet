using Microsoft.Extensions.Logging;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Models;
using System.Globalization;
using System.Text;

namespace OpenBullet.Core.Data;

/// <summary>
/// Result storage service implementation
/// </summary>
public class ResultStorage : IResultStorage
{
    private readonly IOpenBulletContext _context;
    private readonly ILogger<ResultStorage> _logger;

    public ResultStorage(IOpenBulletContext context, ILogger<ResultStorage> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<JobResultEntity>> GetByJobIdAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return await _context.JobResults.FindAsync(r => r.JobId == jobId, cancellationToken);
    }

    public async Task<PagedResult<JobResultEntity>> GetPagedAsync(string jobId, int pageNumber, int pageSize, ResultFilter? filter = null, CancellationToken cancellationToken = default)
    {
        if (filter == null)
        {
            return await _context.JobResults.GetPagedAsync(pageNumber, pageSize, 
                r => r.JobId == jobId, cancellationToken);
        }

        return await _context.JobResults.GetPagedAsync(pageNumber, pageSize, r =>
            r.JobId == jobId &&
            (filter.Status == null || r.Status == filter.Status) &&
            (filter.Success == null || r.Success == filter.Success) &&
            (filter.ProxyUsed == null || r.ProxyUsed == filter.ProxyUsed) &&
            (filter.ResponseCodeFrom == null || r.ResponseCode >= filter.ResponseCodeFrom) &&
            (filter.ResponseCodeTo == null || r.ResponseCode <= filter.ResponseCodeTo) &&
            (filter.ExecutionTimeFrom == null || r.ExecutionTime >= filter.ExecutionTimeFrom) &&
            (filter.ExecutionTimeTo == null || r.ExecutionTime <= filter.ExecutionTimeTo) &&
            (filter.SearchTerm == null || 
             r.DataLine.ToLower().Contains(filter.SearchTerm.ToLower()) ||
             (r.ErrorMessage != null && r.ErrorMessage.ToLower().Contains(filter.SearchTerm.ToLower()))),
            cancellationToken);
    }

    public async Task<IEnumerable<JobResultEntity>> GetSuccessfulAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return await _context.JobResults.FindAsync(r => r.JobId == jobId && r.Success, cancellationToken);
    }

    public async Task<IEnumerable<JobResultEntity>> GetFailedAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return await _context.JobResults.FindAsync(r => r.JobId == jobId && !r.Success, cancellationToken);
    }

    public async Task<JobResultEntity> SaveAsync(JobResultEntity result, CancellationToken cancellationToken = default)
    {
        var existing = await _context.JobResults.GetByIdAsync(result.Id, cancellationToken);
        
        if (existing == null)
        {
            return await _context.JobResults.AddAsync(result, cancellationToken);
        }
        else
        {
            return await _context.JobResults.UpdateAsync(result, cancellationToken);
        }
    }

    public async Task<IEnumerable<JobResultEntity>> SaveRangeAsync(IEnumerable<JobResultEntity> results, CancellationToken cancellationToken = default)
    {
        return await _context.JobResults.AddRangeAsync(results, cancellationToken);
    }

    public async Task<JobResultEntity> CreateFromBotResultAsync(string jobId, BotResult botResult, CancellationToken cancellationToken = default)
    {
        var entity = new JobResultEntity
        {
            JobId = jobId,
            DataLine = botResult.DataLine,
            Status = botResult.Status,
            CustomStatus = botResult.CustomStatus,
            Success = botResult.Success,
            ExecutionTime = botResult.ExecutionTime,
            Variables = botResult.Variables,
            CapturedData = botResult.CapturedData,
            Logs = botResult.Logs,
            ErrorMessage = botResult.ErrorMessage,
            CreatedAt = DateTime.UtcNow
        };

        // Extract metadata
        if (botResult.Metadata.ContainsKey("LastResponseCode"))
        {
            entity.ResponseCode = (int)botResult.Metadata["LastResponseCode"];
        }

        if (botResult.Metadata.ContainsKey("LastAddress"))
        {
            entity.FinalUrl = botResult.Metadata["LastAddress"]?.ToString();
        }

        if (botResult.Metadata.ContainsKey("ProxyUsed"))
        {
            entity.ProxyUsed = botResult.Metadata["ProxyUsed"]?.ToString();
        }

        if (botResult.Metadata.ContainsKey("ResponseTime"))
        {
            if (long.TryParse(botResult.Metadata["ResponseTime"]?.ToString(), out var responseTime))
            {
                entity.ResponseTime = responseTime;
            }
        }

        return await _context.JobResults.AddAsync(entity, cancellationToken);
    }

    public async Task<int> DeleteByJobIdAsync(string jobId, CancellationToken cancellationToken = default)
    {
        return await _context.JobResults.DeleteAsync(r => r.JobId == jobId, cancellationToken);
    }

    public async Task<ResultStatistics> GetStatisticsAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var results = await _context.JobResults.FindAsync(r => r.JobId == jobId, cancellationToken);
        var resultList = results.ToList();

        var stats = new ResultStatistics
        {
            TotalResults = resultList.Count,
            SuccessfulResults = resultList.Count(r => r.Success),
            FailedResults = resultList.Count(r => !r.Success)
        };

        // Count by status
        foreach (var result in resultList)
        {
            if (!stats.StatusCounts.ContainsKey(result.Status))
            {
                stats.StatusCounts[result.Status] = 0;
            }
            stats.StatusCounts[result.Status]++;
        }

        // Calculate average execution time
        if (resultList.Count > 0)
        {
            var averageTicks = resultList.Average(r => r.ExecutionTime.Ticks);
            stats.AverageExecutionTime = new TimeSpan((long)averageTicks);
        }

        // Summarize captured data
        foreach (var result in resultList)
        {
            foreach (var captured in result.CapturedData)
            {
                if (!stats.CapturedDataSummary.ContainsKey(captured.Key))
                {
                    stats.CapturedDataSummary[captured.Key] = 0;
                }
                stats.CapturedDataSummary[captured.Key]++;
            }
        }

        return stats;
    }

    public async Task<bool> ExportAsync(string jobId, string filePath, ExportFormat format, ResultFilter? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var results = filter == null
                ? await GetByJobIdAsync(jobId, cancellationToken)
                : (await GetPagedAsync(jobId, 1, int.MaxValue, filter, cancellationToken)).Items;

            return await ExportHelper.ExportResultsAsync(results, filePath, format, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export results for job {JobId} to {FilePath}", jobId, filePath);
            return false;
        }
    }

    public async Task<IEnumerable<JobResultEntity>> SearchByCapturedDataAsync(string jobId, string key, string value, CancellationToken cancellationToken = default)
    {
        var results = await _context.JobResults.FindAsync(r => r.JobId == jobId, cancellationToken);
        
        return results.Where(r =>
        {
            var capturedData = r.CapturedData;
            return capturedData.ContainsKey(key) && 
                   capturedData[key]?.ToString()?.Contains(value, StringComparison.OrdinalIgnoreCase) == true;
        });
    }

    public async Task<IEnumerable<string>> GetCapturedDataKeysAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var results = await _context.JobResults.FindAsync(r => r.JobId == jobId, cancellationToken);
        
        var keys = new HashSet<string>();
        foreach (var result in results)
        {
            foreach (var key in result.CapturedData.Keys)
            {
                keys.Add(key);
            }
        }

        return keys.OrderBy(k => k);
    }

    public async Task<IEnumerable<string>> GetCapturedDataValuesAsync(string jobId, string key, CancellationToken cancellationToken = default)
    {
        var results = await _context.JobResults.FindAsync(r => r.JobId == jobId, cancellationToken);
        
        var values = new HashSet<string>();
        foreach (var result in results)
        {
            if (result.CapturedData.ContainsKey(key))
            {
                var value = result.CapturedData[key]?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    values.Add(value);
                }
            }
        }

        return values.OrderBy(v => v);
    }
}

/// <summary>
/// Proxy storage service implementation
/// </summary>
public class ProxyStorage : IProxyStorage
{
    private readonly IOpenBulletContext _context;
    private readonly ILogger<ProxyStorage> _logger;

    public ProxyStorage(IOpenBulletContext context, ILogger<ProxyStorage> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ProxyEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Proxies.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProxyEntity>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        // Use the actual conditions instead of computed property
        return await _context.Proxies.FindAsync(p => 
            p.IsActive && 
            (!p.IsBanned || (p.BannedUntil.HasValue && p.BannedUntil < DateTime.UtcNow)) &&
            p.Health != ProxyHealth.Dead, 
            cancellationToken);
    }

    public async Task<PagedResult<ProxyEntity>> GetPagedAsync(int pageNumber, int pageSize, ProxyFilter? filter = null, CancellationToken cancellationToken = default)
    {
        if (filter == null)
        {
            return await _context.Proxies.GetPagedAsync(pageNumber, pageSize, cancellationToken: cancellationToken);
        }

        return await _context.Proxies.GetPagedAsync(pageNumber, pageSize, p =>
            (filter.Type == null || p.Type == filter.Type) &&
            (filter.Health == null || p.Health == filter.Health) &&
            (filter.IsActive == null || p.IsActive == filter.IsActive) &&
            (filter.IsBanned == null || p.IsBanned == filter.IsBanned) &&
            (filter.Country == null || p.Country == filter.Country) &&
            (filter.Source == null || p.Source == filter.Source) &&
            (filter.MinSuccessRate == null || p.SuccessRate >= filter.MinSuccessRate) &&
            (filter.MaxResponseTime == null || p.AverageResponseTimeMs <= filter.MaxResponseTime),
            cancellationToken);
    }

    public async Task<ProxyEntity> SaveAsync(ProxyEntity proxy, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Proxies.GetByIdAsync(proxy.Id, cancellationToken);
        
        if (existing == null)
        {
            return await _context.Proxies.AddAsync(proxy, cancellationToken);
        }
        else
        {
            return await _context.Proxies.UpdateAsync(proxy, cancellationToken);
        }
    }

    public async Task<IEnumerable<ProxyEntity>> SaveRangeAsync(IEnumerable<ProxyEntity> proxies, CancellationToken cancellationToken = default)
    {
        return await _context.Proxies.AddRangeAsync(proxies, cancellationToken);
    }

    public async Task<ImportResult> ImportFromStringsAsync(IEnumerable<string> proxyStrings, string? source = null, CancellationToken cancellationToken = default)
    {
        var result = new ImportResult { Success = true };
        
        foreach (var proxyString in proxyStrings)
        {
            try
            {
                var proxy = ParseProxyString(proxyString.Trim());
                if (proxy != null)
                {
                    proxy.Source = source;
                    
                    // Check for duplicates
                    var existing = await _context.Proxies.GetSingleAsync(p => 
                        p.Host == proxy.Host && p.Port == proxy.Port && p.Type == proxy.Type, 
                        cancellationToken);
                    
                    if (existing == null)
                    {
                        await _context.Proxies.AddAsync(proxy, cancellationToken);
                        result.ImportedCount++;
                    }
                    else
                    {
                        result.SkippedCount++;
                        result.Warnings.Add($"Proxy {proxy.Address} already exists and was skipped");
                    }
                }
                else
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Failed to parse proxy string: {proxyString}");
                }
            }
            catch (Exception ex)
            {
                result.ErrorCount++;
                result.Errors.Add($"Error importing proxy '{proxyString}': {ex.Message}");
            }
        }

        return result;
    }

    public async Task UpdateStatisticsAsync(string id, bool success, TimeSpan responseTime, CancellationToken cancellationToken = default)
    {
        var proxy = await _context.Proxies.GetByIdAsync(id, cancellationToken);
        if (proxy != null)
        {
            proxy.Uses++;
            proxy.LastUsed = DateTime.UtcNow;

            if (success)
            {
                proxy.SuccessfulRequests++;
            }
            else
            {
                proxy.FailedRequests++;
            }

            // Update average response time
            var totalRequests = proxy.TotalRequests;
            if (totalRequests > 1)
            {
                var totalTime = proxy.AverageResponseTimeMs * (totalRequests - 1) + responseTime.TotalMilliseconds;
                proxy.AverageResponseTimeMs = (long)(totalTime / totalRequests);
            }
            else
            {
                proxy.AverageResponseTimeMs = (long)responseTime.TotalMilliseconds;
            }

            // Update success rate
            proxy.SuccessRate = totalRequests > 0 ? (double)proxy.SuccessfulRequests / totalRequests * 100 : 0;

            // Update health based on recent performance
            UpdateProxyHealth(proxy);

            await _context.Proxies.UpdateAsync(proxy, cancellationToken);
        }
    }

    public async Task BanProxyAsync(string id, TimeSpan? duration = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        var proxy = await _context.Proxies.GetByIdAsync(id, cancellationToken);
        if (proxy != null)
        {
            proxy.IsBanned = true;
            proxy.BannedUntil = duration.HasValue ? DateTime.UtcNow.Add(duration.Value) : null;
            proxy.BanReason = reason ?? "Manual ban";

            await _context.Proxies.UpdateAsync(proxy, cancellationToken);
            _logger.LogInformation("Banned proxy {ProxyAddress} (Reason: {Reason}, Duration: {Duration})", 
                proxy.Address, reason, duration?.ToString() ?? "Permanent");
        }
    }

    public async Task UnbanProxyAsync(string id, CancellationToken cancellationToken = default)
    {
        var proxy = await _context.Proxies.GetByIdAsync(id, cancellationToken);
        if (proxy != null)
        {
            proxy.IsBanned = false;
            proxy.BannedUntil = null;
            proxy.BanReason = null;

            await _context.Proxies.UpdateAsync(proxy, cancellationToken);
            _logger.LogInformation("Unbanned proxy {ProxyAddress}", proxy.Address);
        }
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Proxies.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<ProxyStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var proxies = await _context.Proxies.GetAllAsync(cancellationToken);
        var proxyList = proxies.ToList();

        var stats = new ProxyStatistics
        {
            TotalProxies = proxyList.Count,
            ActiveProxies = proxyList.Count(p => p.IsActive),
            BannedProxies = proxyList.Count(p => p.IsCurrentlyBanned)
        };

        // Health distribution
        foreach (var proxy in proxyList)
        {
            if (!stats.HealthDistribution.ContainsKey(proxy.Health))
            {
                stats.HealthDistribution[proxy.Health] = 0;
            }
            stats.HealthDistribution[proxy.Health]++;
        }

        // Type distribution
        foreach (var proxy in proxyList)
        {
            if (!stats.TypeDistribution.ContainsKey(proxy.Type))
            {
                stats.TypeDistribution[proxy.Type] = 0;
            }
            stats.TypeDistribution[proxy.Type]++;
        }

        // Overall statistics
        var proxiesWithRequests = proxyList.Where(p => p.TotalRequests > 0).ToList();
        if (proxiesWithRequests.Count > 0)
        {
            var totalRequests = proxiesWithRequests.Sum(p => p.TotalRequests);
            var totalSuccessful = proxiesWithRequests.Sum(p => p.SuccessfulRequests);
            stats.OverallSuccessRate = totalRequests > 0 ? (double)totalSuccessful / totalRequests * 100 : 0;

            var weightedResponseTime = proxiesWithRequests.Sum(p => p.AverageResponseTimeMs * p.TotalRequests);
            stats.AverageResponseTime = totalRequests > 0 ? weightedResponseTime / totalRequests : 0;
        }

        return stats;
    }

    public async Task<int> CleanupAsync(TimeSpan? olderThan = null, bool removeDeadProxies = false, CancellationToken cancellationToken = default)
    {
        var count = 0;

        if (removeDeadProxies)
        {
            count += await _context.Proxies.DeleteAsync(p => p.Health == ProxyHealth.Dead, cancellationToken);
        }

        if (olderThan.HasValue)
        {
            var cutoffDate = DateTime.UtcNow - olderThan.Value;
            count += await _context.Proxies.DeleteAsync(p => 
                p.CreatedAt < cutoffDate && 
                (p.LastUsed == null || p.LastUsed < cutoffDate), 
                cancellationToken);
        }

        _logger.LogInformation("Cleaned up {Count} proxies", count);
        return count;
    }

    public async Task<bool> ExportAsync(string filePath, ExportFormat format, ProxyFilter? filter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var proxies = filter == null
                ? await GetAllAsync(cancellationToken)
                : (await GetPagedAsync(1, int.MaxValue, filter, cancellationToken)).Items;

            return await ExportHelper.ExportProxiesAsync(proxies, filePath, format, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export proxies to {FilePath}", filePath);
            return false;
        }
    }

    private static ProxyEntity? ParseProxyString(string proxyString)
    {
        if (string.IsNullOrWhiteSpace(proxyString))
            return null;

        try
        {
            var proxy = new ProxyEntity();

            // Pattern for: [type://][username:password@]host:port
            var pattern = @"^(?:(?<type>https?|socks[45]?)://)?(?:(?<username>[^:@]+):(?<password>[^@]+)@)?(?<host>[^:]+):(?<port>\d+)$";
            var match = System.Text.RegularExpressions.Regex.Match(proxyString, pattern, 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // Try simple host:port format
                var parts = proxyString.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var port))
                {
                    proxy.Host = parts[0].Trim();
                    proxy.Port = port;
                    proxy.Type = ProxyType.Http; // Default type
                    return proxy;
                }
                return null;
            }

            proxy.Host = match.Groups["host"].Value;
            if (!int.TryParse(match.Groups["port"].Value, out var proxyPort))
                return null;
            
            proxy.Port = proxyPort;
            proxy.Username = match.Groups["username"].Value;
            proxy.Password = match.Groups["password"].Value;

            // Parse proxy type
            var typeString = match.Groups["type"].Value.ToLowerInvariant();
            proxy.Type = typeString switch
            {
                "http" or "https" => ProxyType.Http,
                "socks4" => ProxyType.Socks4,
                "socks5" => ProxyType.Socks5,
                _ => ProxyType.Http
            };

            proxy.CreatedAt = DateTime.UtcNow;
            return proxy;
        }
        catch
        {
            return null;
        }
    }

    private static void UpdateProxyHealth(ProxyEntity proxy)
    {
        if (proxy.TotalRequests == 0)
        {
            proxy.Health = ProxyHealth.Unknown;
            return;
        }

        var successRate = proxy.SuccessRate ?? 0;
        var responseTime = proxy.AverageResponseTimeMs;

        if (successRate < 50.0)
        {
            proxy.Health = ProxyHealth.Dead;
        }
        else if (responseTime > 30000) // 30 seconds
        {
            proxy.Health = ProxyHealth.Slow;
        }
        else if (successRate < 90.0 || responseTime > 5000)
        {
            proxy.Health = ProxyHealth.Unreliable;
        }
        else
        {
            proxy.Health = ProxyHealth.Healthy;
        }
    }
}

/// <summary>
/// Settings storage service implementation
/// </summary>
public class SettingsStorage : ISettingsStorage
{
    private readonly IOpenBulletContext _context;
    private readonly ILogger<SettingsStorage> _logger;

    public SettingsStorage(IOpenBulletContext context, ILogger<SettingsStorage> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SettingEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Settings.GetOrderedAsync(s => s.Category, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<SettingEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Settings.FindAsync(s => s.Category == category, cancellationToken);
    }

    public async Task<T?> GetValueAsync<T>(string key, T? defaultValue = default, CancellationToken cancellationToken = default)
    {
        var setting = await _context.Settings.GetSingleAsync(s => s.Key == key, cancellationToken);
        
        if (setting == null)
        {
            return defaultValue;
        }

        return setting.GetValue<T>();
    }

    public async Task SetValueAsync<T>(string key, T value, string? description = null, string category = "General", CancellationToken cancellationToken = default)
    {
        var setting = await _context.Settings.GetSingleAsync(s => s.Key == key, cancellationToken);
        
        if (setting == null)
        {
            setting = new SettingEntity
            {
                Key = key,
                Description = description,
                Category = category,
                DataType = typeof(T).Name.ToLowerInvariant(),
                CreatedAt = DateTime.UtcNow
            };
            setting.SetValue(value);
            await _context.Settings.AddAsync(setting, cancellationToken);
        }
        else
        {
            setting.SetValue(value);
            if (description != null)
            {
                setting.Description = description;
            }
            await _context.Settings.UpdateAsync(setting, cancellationToken);
        }
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await _context.Settings.GetSingleAsync(s => s.Key == key, cancellationToken);
        if (setting == null)
            return false;

        return await _context.Settings.DeleteAsync(setting, cancellationToken);
    }

    public async Task<bool> ExportAsync(string filePath, string? category = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = category == null
                ? await GetAllAsync(cancellationToken)
                : await GetByCategoryAsync(category, cancellationToken);

            return await ExportHelper.ExportSettingsAsync(settings, filePath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export settings to {FilePath}", filePath);
            return false;
        }
    }

    public async Task<ImportResult> ImportAsync(string filePath, bool overwriteExisting = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = await ExportHelper.ImportSettingsAsync(filePath, cancellationToken);
            var result = new ImportResult { Success = true };

            foreach (var setting in settings)
            {
                try
                {
                    var existing = await _context.Settings.GetSingleAsync(s => s.Key == setting.Key, cancellationToken);
                    
                    if (existing != null && !overwriteExisting)
                    {
                        result.SkippedCount++;
                        result.Warnings.Add($"Setting '{setting.Key}' already exists and was skipped");
                        continue;
                    }

                    if (existing != null && overwriteExisting)
                    {
                        setting.Id = existing.Id;
                        setting.CreatedAt = existing.CreatedAt;
                        await _context.Settings.UpdateAsync(setting, cancellationToken);
                    }
                    else
                    {
                        await _context.Settings.AddAsync(setting, cancellationToken);
                    }

                    result.ImportedCount++;
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add($"Failed to import setting '{setting.Key}': {ex.Message}");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import settings from {FilePath}", filePath);
            return new ImportResult
            {
                Success = false,
                Errors = { ex.Message }
            };
        }
    }

    public async Task ResetToDefaultsAsync(string? category = null, CancellationToken cancellationToken = default)
    {
        var settings = category == null
            ? await GetAllAsync(cancellationToken)
            : await GetByCategoryAsync(category, cancellationToken);

        foreach (var setting in settings.Where(s => !string.IsNullOrEmpty(s.DefaultValue)))
        {
            setting.Value = setting.DefaultValue;
            await _context.Settings.UpdateAsync(setting, cancellationToken);
        }

        _logger.LogInformation("Reset settings to defaults for category: {Category}", category ?? "All");
    }
}

/// <summary>
/// Extended export helper methods
/// </summary>
public static partial class ExportHelper
{
    public static async Task<bool> ExportResultsAsync(IEnumerable<JobResultEntity> results, string filePath, ExportFormat format, CancellationToken cancellationToken = default)
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
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented);
                    await File.WriteAllTextAsync(filePath, json, cancellationToken);
                    break;

                case ExportFormat.CSV:
                    await ExportResultsToCsvAsync(results, filePath, cancellationToken);
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

    public static async Task<bool> ExportProxiesAsync(IEnumerable<ProxyEntity> proxies, string filePath, ExportFormat format, CancellationToken cancellationToken = default)
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
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(proxies, Newtonsoft.Json.Formatting.Indented);
                    await File.WriteAllTextAsync(filePath, json, cancellationToken);
                    break;

                case ExportFormat.CSV:
                    await ExportProxiesToCsvAsync(proxies, filePath, cancellationToken);
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

    public static async Task<bool> ExportSettingsAsync(IEnumerable<SettingEntity> settings, string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<List<SettingEntity>> ImportSettingsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var content = await File.ReadAllTextAsync(filePath, cancellationToken);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<SettingEntity>>(content) ?? new List<SettingEntity>();
    }

    private static async Task ExportResultsToCsvAsync(IEnumerable<JobResultEntity> results, string filePath, CancellationToken cancellationToken)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(results, cancellationToken);
    }

    private static async Task ExportProxiesToCsvAsync(IEnumerable<ProxyEntity> proxies, string filePath, CancellationToken cancellationToken)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(proxies, cancellationToken);
    }
}
