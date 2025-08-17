using Microsoft.Extensions.Logging;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace OpenBullet.Core.Proxies;

/// <summary>
/// Implementation of proxy management with rotation, health monitoring, and statistics
/// </summary>
public class ProxyManager : IProxyManager, IDisposable
{
    private readonly ILogger<ProxyManager> _logger;
    private readonly IHttpClientService _httpClientService;
    private readonly ConcurrentDictionary<string, ProxyInfo> _proxies = new();
    private readonly ConcurrentQueue<string> _rotationQueue = new();
    private readonly SemaphoreSlim _poolSemaphore = new(1, 1);
    private readonly Timer _healthCheckTimer;
    private readonly Timer _unbanTimer;
    private readonly Random _random = new();
    
    private ProxyPoolConfiguration _configuration = new();
    private ProxyRotationStrategy _rotationStrategy = ProxyRotationStrategy.RoundRobin;
    private int _roundRobinIndex = 0;
    private bool _disposed = false;

    public event EventHandler<ProxyBannedEventArgs>? ProxyBanned;
    public event EventHandler<ProxyUnbannedEventArgs>? ProxyUnbanned;
    public event EventHandler<ProxyStatisticsUpdatedEventArgs>? ProxyStatisticsUpdated;

    public ProxyManager(ILogger<ProxyManager> logger, IHttpClientService httpClientService)
    {
        _logger = logger;
        _httpClientService = httpClientService;

        // Initialize timers
        _healthCheckTimer = new Timer(PerformHealthCheck, null, Timeout.Infinite, Timeout.Infinite);
        _unbanTimer = new Timer(PerformAutoUnban, null, Timeout.Infinite, Timeout.Infinite);

        _logger.LogInformation("ProxyManager initialized");
    }

    public async Task<int> LoadProxiesFromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (!File.Exists(filePath))
        {
            _logger.LogError("Proxy file not found: {FilePath}", filePath);
            throw new FileNotFoundException($"Proxy file not found: {filePath}");
        }

        try
        {
            _logger.LogInformation("Loading proxies from file: {FilePath}", filePath);

            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
            var validLines = lines.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"));

            return await LoadProxiesFromListAsync(validLines, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load proxies from file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<int> LoadProxiesFromListAsync(IEnumerable<string> proxyStrings, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            var loadedCount = 0;
            var totalCount = 0;

            foreach (var proxyString in proxyStrings)
            {
                totalCount++;
                
                if (string.IsNullOrWhiteSpace(proxyString))
                    continue;

                try
                {
                    var proxy = ParseProxyString(proxyString.Trim());
                    if (proxy != null)
                    {
                        var existing = _proxies.Values.FirstOrDefault(p => 
                            p.Host == proxy.Host && p.Port == proxy.Port && p.Type == proxy.Type);
                        
                        if (existing == null)
                        {
                            _proxies[proxy.Id] = proxy;
                            _rotationQueue.Enqueue(proxy.Id);
                            loadedCount++;
                        }
                        else
                        {
                            _logger.LogTrace("Duplicate proxy skipped: {ProxyAddress}", proxy.Address);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse proxy string: {ProxyString}", proxyString);
                }
            }

            _logger.LogInformation("Loaded {LoadedCount} proxies out of {TotalCount} entries", loadedCount, totalCount);

            // Start health checking and auto-unban if proxies were loaded
            if (loadedCount > 0)
            {
                StartPeriodicTasks();
            }

            return loadedCount;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<bool> AddProxyAsync(ProxyInfo proxy, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (proxy == null)
        {
            throw new ArgumentNullException(nameof(proxy));
        }

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            var existing = _proxies.Values.FirstOrDefault(p => 
                p.Host == proxy.Host && p.Port == proxy.Port && p.Type == proxy.Type);

            if (existing != null)
            {
                _logger.LogWarning("Proxy already exists: {ProxyAddress}", proxy.Address);
                return false;
            }

            if (string.IsNullOrEmpty(proxy.Id))
            {
                proxy.Id = Guid.NewGuid().ToString();
            }

            _proxies[proxy.Id] = proxy;
            _rotationQueue.Enqueue(proxy.Id);

            _logger.LogDebug("Added proxy: {ProxyAddress}", proxy.Address);

            // Start periodic tasks if this is the first proxy
            if (_proxies.Count == 1)
            {
                StartPeriodicTasks();
            }

            return true;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<bool> RemoveProxyAsync(ProxyInfo proxy, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (proxy == null)
        {
            throw new ArgumentNullException(nameof(proxy));
        }

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            var removed = _proxies.TryRemove(proxy.Id, out var removedProxy);
            if (removed && removedProxy != null)
            {
                _logger.LogDebug("Removed proxy: {ProxyAddress}", removedProxy.Address);
            }

            return removed;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<ProxyInfo?> GetNextProxyAsync(string? assignedTo = null, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            var availableProxies = _proxies.Values.Where(p => p.IsAvailable).ToList();
            
            if (availableProxies.Count == 0)
            {
                _logger.LogWarning("No available proxies in pool");
                return null;
            }

            ProxyInfo? selectedProxy = null;

            switch (_rotationStrategy)
            {
                case ProxyRotationStrategy.RoundRobin:
                    selectedProxy = GetNextRoundRobinProxy(availableProxies);
                    break;

                case ProxyRotationStrategy.Random:
                    selectedProxy = GetRandomProxy(availableProxies);
                    break;

                case ProxyRotationStrategy.LeastUsed:
                    selectedProxy = GetLeastUsedProxy(availableProxies);
                    break;

                case ProxyRotationStrategy.HealthBased:
                    selectedProxy = GetHealthBasedProxy(availableProxies);
                    break;

                case ProxyRotationStrategy.ResponseTimeBased:
                    selectedProxy = GetResponseTimeBasedProxy(availableProxies);
                    break;

                case ProxyRotationStrategy.Sticky:
                    selectedProxy = GetStickyProxy(availableProxies, assignedTo);
                    break;

                default:
                    selectedProxy = GetNextRoundRobinProxy(availableProxies);
                    break;
            }

            if (selectedProxy != null)
            {
                selectedProxy.Uses++;
                selectedProxy.LastUsed = DateTime.UtcNow;
                selectedProxy.AssignedTo = assignedTo;

                _logger.LogTrace("Selected proxy: {ProxyAddress} (Strategy: {Strategy})", 
                    selectedProxy.Address, _rotationStrategy);
            }

            return selectedProxy?.Clone();
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<bool> ReturnProxyAsync(ProxyInfo proxy, ProxyUsageResult result, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (proxy == null || result == null)
        {
            return false;
        }

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (!_proxies.TryGetValue(proxy.Id, out var managedProxy))
            {
                _logger.LogWarning("Attempted to return unknown proxy: {ProxyAddress}", proxy.Address);
                return false;
            }

            // Update statistics
            if (result.Success)
            {
                managedProxy.SuccessfulRequests++;
            }
            else
            {
                managedProxy.FailedRequests++;
            }

            // Update average response time
            var totalTime = managedProxy.AverageResponseTime.Ticks * (managedProxy.TotalRequests - 1) + result.ResponseTime.Ticks;
            managedProxy.AverageResponseTime = new TimeSpan(totalTime / managedProxy.TotalRequests);

            // Update health based on recent performance
            UpdateProxyHealth(managedProxy);

            // Check if proxy should be banned
            if (result.ShouldBan || ShouldAutoBan(managedProxy))
            {
                var banReason = result.BanReason ?? "Auto-ban due to poor performance";
                await BanProxyInternalAsync(managedProxy, _configuration.AutoBanTimeout, banReason);
            }

            managedProxy.AssignedTo = null;

            // Fire statistics updated event
            ProxyStatisticsUpdated?.Invoke(this, new ProxyStatisticsUpdatedEventArgs
            {
                Proxy = managedProxy.Clone(),
                UsageResult = result,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogTrace("Updated proxy statistics: {ProxyAddress} (Success: {Success}, ResponseTime: {ResponseTime}ms)", 
                proxy.Address, result.Success, result.ResponseTime.TotalMilliseconds);

            return true;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<bool> BanProxyAsync(ProxyInfo proxy, TimeSpan? banDuration = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (proxy == null)
        {
            return false;
        }

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_proxies.TryGetValue(proxy.Id, out var managedProxy))
            {
                return await BanProxyInternalAsync(managedProxy, banDuration, reason);
            }

            return false;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<bool> UnbanProxyAsync(ProxyInfo proxy, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (proxy == null)
        {
            return false;
        }

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_proxies.TryGetValue(proxy.Id, out var managedProxy))
            {
                return await UnbanProxyInternalAsync(managedProxy, false);
            }

            return false;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<ProxyTestResult> TestProxyAsync(ProxyInfo proxy, ProxyTestConfiguration? config = null, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        if (proxy == null)
        {
            throw new ArgumentNullException(nameof(proxy));
        }

        config ??= new ProxyTestConfiguration();
        var result = new ProxyTestResult { Proxy = proxy.Clone() };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogTrace("Testing proxy: {ProxyAddress}", proxy.Address);

            var httpConfig = new HttpClientConfiguration
            {
                Timeout = config.Timeout,
                UserAgent = config.UserAgent,
                IgnoreSslErrors = !config.ValidateSsl
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, config.TestUrl);
            foreach (var header in config.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await _httpClientService.SendAsync(requestMessage, proxy, httpConfig);

            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
            result.ResponseCode = (int)response.StatusCode;
            result.ResponseContent = response.Content;
            result.Success = response.IsSuccessStatusCode;

            if (!string.IsNullOrEmpty(config.ExpectedContent) && 
                (response.Content?.Contains(config.ExpectedContent) != true))
            {
                result.Success = false;
                result.ErrorMessage = $"Response does not contain expected content: {config.ExpectedContent}";
            }

            result.DetermineHealth();

            _logger.LogTrace("Proxy test completed: {ProxyAddress} - Success: {Success}, ResponseTime: {ResponseTime}ms", 
                proxy.Address, result.Success, result.ResponseTime.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.Exception = ex;
            result.DeterminedHealth = ProxyHealth.Dead;

            _logger.LogTrace(ex, "Proxy test failed: {ProxyAddress}", proxy.Address);

            return result;
        }
    }

    public async Task<ProxyPoolTestResult> TestAllProxiesAsync(ProxyTestConfiguration? config = null, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        config ??= new ProxyTestConfiguration();
        var result = new ProxyPoolTestResult
        {
            TestStartTime = DateTime.UtcNow
        };

        var allProxies = await GetAllProxiesAsync(cancellationToken);
        result.TotalProxies = allProxies.Count;

        if (allProxies.Count == 0)
        {
            _logger.LogWarning("No proxies to test");
            result.TestEndTime = DateTime.UtcNow;
            return result;
        }

        _logger.LogInformation("Testing {ProxyCount} proxies", allProxies.Count);

        var stopwatch = Stopwatch.StartNew();
        var testTasks = allProxies.Select(async proxy =>
        {
            try
            {
                var testResult = await TestProxyAsync(proxy, config, cancellationToken);
                result.Results.Add(testResult);

                // Update proxy health in pool
                if (_proxies.TryGetValue(proxy.Id, out var managedProxy))
                {
                    managedProxy.Health = testResult.DeterminedHealth;
                    managedProxy.LastTestedAt = DateTime.UtcNow;
                }

                return testResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing proxy {ProxyAddress}", proxy.Address);
                return new ProxyTestResult
                {
                    Proxy = proxy,
                    Success = false,
                    ErrorMessage = ex.Message,
                    Exception = ex,
                    DeterminedHealth = ProxyHealth.Dead
                };
            }
        });

        await Task.WhenAll(testTasks);
        stopwatch.Stop();

        // Calculate results
        result.TestedProxies = result.Results.Count;
        result.HealthyProxies = result.Results.Count(r => r.DeterminedHealth == ProxyHealth.Healthy);
        result.SlowProxies = result.Results.Count(r => r.DeterminedHealth == ProxyHealth.Slow);
        result.UnreliableProxies = result.Results.Count(r => r.DeterminedHealth == ProxyHealth.Unreliable);
        result.DeadProxies = result.Results.Count(r => r.DeterminedHealth == ProxyHealth.Dead);
        result.TotalTestTime = stopwatch.Elapsed;
        result.TestEndTime = DateTime.UtcNow;

        var successfulTests = result.Results.Where(r => r.Success).ToList();
        if (successfulTests.Count > 0)
        {
            var averageTicks = successfulTests.Average(r => r.ResponseTime.Ticks);
            result.AverageResponseTime = new TimeSpan((long)averageTicks);
        }

        _logger.LogInformation("Proxy testing completed: {HealthyCount} healthy, {SlowCount} slow, {UnreliableCount} unreliable, {DeadCount} dead out of {TotalCount} proxies", 
            result.HealthyProxies, result.SlowProxies, result.UnreliableProxies, result.DeadProxies, result.TotalProxies);

        return result;
    }

    public async Task<List<ProxyInfo>> GetAllProxiesAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            return _proxies.Values.Select(p => p.Clone()).ToList();
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<List<ProxyInfo>> GetAvailableProxiesAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            return _proxies.Values.Where(p => p.IsAvailable).Select(p => p.Clone()).ToList();
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<List<ProxyInfo>> GetBannedProxiesAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            return _proxies.Values.Where(p => p.IsCurrentlyBanned).Select(p => p.Clone()).ToList();
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<ProxyPoolStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            var allProxies = _proxies.Values.ToList();
            var stats = new ProxyPoolStatistics
            {
                TotalProxies = allProxies.Count,
                AvailableProxies = allProxies.Count(p => p.IsAvailable),
                BannedProxies = allProxies.Count(p => p.IsCurrentlyBanned),
                HealthyProxies = allProxies.Count(p => p.Health == ProxyHealth.Healthy),
                SlowProxies = allProxies.Count(p => p.Health == ProxyHealth.Slow),
                UnreliableProxies = allProxies.Count(p => p.Health == ProxyHealth.Unreliable),
                DeadProxies = allProxies.Count(p => p.Health == ProxyHealth.Dead),
                TotalRequests = allProxies.Sum(p => (long)p.TotalRequests),
                SuccessfulRequests = allProxies.Sum(p => (long)p.SuccessfulRequests),
                FailedRequests = allProxies.Sum(p => (long)p.FailedRequests),
                LastUpdated = DateTime.UtcNow
            };

            // Calculate average response time
            var proxiesWithRequests = allProxies.Where(p => p.TotalRequests > 0).ToList();
            if (proxiesWithRequests.Count > 0)
            {
                var weightedResponseTime = proxiesWithRequests
                    .Sum(p => p.AverageResponseTime.Ticks * p.TotalRequests);
                var totalRequests = proxiesWithRequests.Sum(p => p.TotalRequests);
                stats.AverageResponseTime = new TimeSpan(weightedResponseTime / totalRequests);
            }

            // Calculate overall success rate
            stats.OverallSuccessRate = stats.TotalRequests > 0 
                ? (double)stats.SuccessfulRequests / stats.TotalRequests * 100 
                : 0;

            // Group by type and health
            stats.ProxiesByType = allProxies.GroupBy(p => p.Type)
                .ToDictionary(g => g.Key, g => g.Count());
            stats.ProxiesByHealth = allProxies.GroupBy(p => p.Health)
                .ToDictionary(g => g.Key, g => g.Count());

            return stats;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<bool> ClearPoolAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);

        await _poolSemaphore.WaitAsync(cancellationToken);
        try
        {
            var count = _proxies.Count;
            _proxies.Clear();
            _rotationQueue.Clear();
            _roundRobinIndex = 0;

            _logger.LogInformation("Cleared proxy pool ({ProxyCount} proxies removed)", count);
            return true;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public void SetRotationStrategy(ProxyRotationStrategy strategy)
    {
        _rotationStrategy = strategy;
        _logger.LogInformation("Proxy rotation strategy changed to: {Strategy}", strategy);
    }

    public void SetConfiguration(ProxyPoolConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _rotationStrategy = configuration.RotationStrategy;

        // Update timer intervals
        UpdateTimerIntervals();

        _logger.LogInformation("Proxy pool configuration updated");
    }

    private ProxyInfo? ParseProxyString(string proxyString)
    {
        if (string.IsNullOrWhiteSpace(proxyString))
            return null;

        try
        {
            var proxy = new ProxyInfo();

            // Pattern for: [type://][username:password@]host:port
            var pattern = @"^(?:(?<type>https?|socks[45]?)://)?(?:(?<username>[^:@]+):(?<password>[^@]+)@)?(?<host>[^:]+):(?<port>\d+)$";
            var match = Regex.Match(proxyString, pattern, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                // Try simple host:port format
                var parts = proxyString.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out var port) && port > 0 && port <= 65535)
                {
                    proxy.Host = parts[0].Trim();
                    proxy.Port = port;
                    proxy.Type = ProxyType.Http; // Default type
                    return proxy;
                }
                return null;
            }

            proxy.Host = match.Groups["host"].Value;
            if (!int.TryParse(match.Groups["port"].Value, out var proxyPort) || proxyPort <= 0 || proxyPort > 65535)
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

            return proxy;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse proxy string: {ProxyString}", proxyString);
            return null;
        }
    }

    private ProxyInfo? GetNextRoundRobinProxy(List<ProxyInfo> availableProxies)
    {
        if (availableProxies.Count == 0)
            return null;

        var index = Interlocked.Increment(ref _roundRobinIndex) % availableProxies.Count;
        return availableProxies[index];
    }

    private ProxyInfo? GetRandomProxy(List<ProxyInfo> availableProxies)
    {
        if (availableProxies.Count == 0)
            return null;

        var index = _random.Next(availableProxies.Count);
        return availableProxies[index];
    }

    private ProxyInfo? GetLeastUsedProxy(List<ProxyInfo> availableProxies)
    {
        if (availableProxies.Count == 0)
            return null;

        return availableProxies.OrderBy(p => p.Uses).ThenBy(p => p.LastUsed).First();
    }

    private ProxyInfo? GetHealthBasedProxy(List<ProxyInfo> availableProxies)
    {
        if (availableProxies.Count == 0)
            return null;

        // Prioritize by health, then by least used
        return availableProxies
            .OrderBy(p => (int)p.Health)
            .ThenBy(p => p.Uses)
            .First();
    }

    private ProxyInfo? GetResponseTimeBasedProxy(List<ProxyInfo> availableProxies)
    {
        if (availableProxies.Count == 0)
            return null;

        // Prioritize by response time, then by least used
        return availableProxies
            .OrderBy(p => p.AverageResponseTime)
            .ThenBy(p => p.Uses)
            .First();
    }

    private ProxyInfo? GetStickyProxy(List<ProxyInfo> availableProxies, string? assignedTo)
    {
        if (availableProxies.Count == 0)
            return null;

        if (!string.IsNullOrEmpty(assignedTo))
        {
            // Try to find a proxy previously assigned to this entity
            var stickyProxy = availableProxies.FirstOrDefault(p => p.AssignedTo == assignedTo);
            if (stickyProxy != null)
                return stickyProxy;
        }

        // Fall back to least used
        return GetLeastUsedProxy(availableProxies);
    }

    private void UpdateProxyHealth(ProxyInfo proxy)
    {
        if (proxy.TotalRequests == 0)
        {
            proxy.Health = ProxyHealth.Unknown;
            return;
        }

        var successRate = proxy.SuccessRate;
        var responseTime = proxy.AverageResponseTime;

        if (successRate < _configuration.MinSuccessRate)
        {
            proxy.Health = ProxyHealth.Dead;
        }
        else if (responseTime > _configuration.MaxResponseTime)
        {
            proxy.Health = ProxyHealth.Slow;
        }
        else if (successRate < 90.0 || responseTime > TimeSpan.FromSeconds(5))
        {
            proxy.Health = ProxyHealth.Unreliable;
        }
        else
        {
            proxy.Health = ProxyHealth.Healthy;
        }
    }

    private bool ShouldAutoBan(ProxyInfo proxy)
    {
        // Check consecutive failures
        if (proxy.FailedRequests >= _configuration.MaxFailuresBeforeBan && 
            proxy.TotalRequests >= _configuration.MaxFailuresBeforeBan)
        {
            var recentFailureRate = proxy.FailedRequests / (double)proxy.TotalRequests;
            if (recentFailureRate > 0.8) // 80% failure rate
                return true;
        }

        // Check health status
        if (proxy.Health == ProxyHealth.Dead)
            return true;

        return false;
    }

    private async Task<bool> BanProxyInternalAsync(ProxyInfo proxy, TimeSpan? banDuration, string? reason)
    {
        proxy.IsBanned = true;
        proxy.BannedUntil = banDuration.HasValue ? DateTime.UtcNow.Add(banDuration.Value) : null;
        proxy.BanReason = reason ?? "Manual ban";

        ProxyBanned?.Invoke(this, new ProxyBannedEventArgs
        {
            Proxy = proxy.Clone(),
            Reason = reason,
            BanDuration = banDuration,
            Timestamp = DateTime.UtcNow
        });

        _logger.LogDebug("Banned proxy: {ProxyAddress} (Reason: {Reason}, Duration: {Duration})", 
            proxy.Address, reason, banDuration?.ToString() ?? "Permanent");

        return await Task.FromResult(true);
    }

    private async Task<bool> UnbanProxyInternalAsync(ProxyInfo proxy, bool wasAutomatic)
    {
        proxy.IsBanned = false;
        proxy.BannedUntil = null;
        proxy.BanReason = null;

        ProxyUnbanned?.Invoke(this, new ProxyUnbannedEventArgs
        {
            Proxy = proxy.Clone(),
            WasAutomatic = wasAutomatic,
            Timestamp = DateTime.UtcNow
        });

        _logger.LogDebug("Unbanned proxy: {ProxyAddress} (Automatic: {WasAutomatic})", 
            proxy.Address, wasAutomatic);

        return await Task.FromResult(true);
    }

    private void StartPeriodicTasks()
    {
        if (_configuration.HealthCheckInterval > TimeSpan.Zero)
        {
            _healthCheckTimer.Change(_configuration.HealthCheckInterval, _configuration.HealthCheckInterval);
        }

        if (_configuration.AutoUnbanEnabled && _configuration.AutoUnbanInterval > TimeSpan.Zero)
        {
            _unbanTimer.Change(_configuration.AutoUnbanInterval, _configuration.AutoUnbanInterval);
        }
    }

    private void UpdateTimerIntervals()
    {
        _healthCheckTimer.Change(_configuration.HealthCheckInterval, _configuration.HealthCheckInterval);
        
        if (_configuration.AutoUnbanEnabled)
        {
            _unbanTimer.Change(_configuration.AutoUnbanInterval, _configuration.AutoUnbanInterval);
        }
        else
        {
            _unbanTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    private async void PerformHealthCheck(object? state)
    {
        try
        {
            if (_disposed || _proxies.IsEmpty)
                return;

            _logger.LogTrace("Performing periodic health check");

            var proxiesToTest = _proxies.Values
                .Where(p => !p.IsCurrentlyBanned && 
                           (p.LastTestedAt == null || 
                            DateTime.UtcNow - p.LastTestedAt > _configuration.HealthCheckInterval))
                .Take(10) // Limit concurrent health checks
                .ToList();

            if (proxiesToTest.Count > 0)
            {
                var testConfig = new ProxyTestConfiguration
                {
                    Timeout = TimeSpan.FromSeconds(5) // Shorter timeout for health checks
                };

                var testTasks = proxiesToTest.Select(async proxy =>
                {
                    try
                    {
                        var result = await TestProxyAsync(proxy, testConfig);
                        
                        await _poolSemaphore.WaitAsync();
                        try
                        {
                            if (_proxies.TryGetValue(proxy.Id, out var managedProxy))
                            {
                                managedProxy.Health = result.DeterminedHealth;
                                managedProxy.LastTestedAt = DateTime.UtcNow;

                                if (result.DeterminedHealth == ProxyHealth.Dead && !managedProxy.IsBanned)
                                {
                                    await BanProxyInternalAsync(managedProxy, _configuration.AutoBanTimeout, "Health check failed");
                                }
                            }
                        }
                        finally
                        {
                            _poolSemaphore.Release();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogTrace(ex, "Health check failed for proxy {ProxyAddress}", proxy.Address);
                    }
                });

                await Task.WhenAll(testTasks);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during periodic health check");
        }
    }

    private async void PerformAutoUnban(object? state)
    {
        try
        {
            if (_disposed || _proxies.IsEmpty)
                return;

            _logger.LogTrace("Performing periodic auto-unban check");

            var proxiesToUnban = _proxies.Values
                .Where(p => p.IsBanned && 
                           p.BannedUntil.HasValue && 
                           p.BannedUntil.Value <= DateTime.UtcNow)
                .ToList();

            foreach (var proxy in proxiesToUnban)
            {
                await UnbanProxyInternalAsync(proxy, true);
            }

            if (proxiesToUnban.Count > 0)
            {
                _logger.LogDebug("Auto-unbanned {Count} proxies", proxiesToUnban.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during periodic auto-unban");
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _healthCheckTimer?.Dispose();
        _unbanTimer?.Dispose();
        _poolSemaphore?.Dispose();

        _logger.LogInformation("ProxyManager disposed");
    }
}
