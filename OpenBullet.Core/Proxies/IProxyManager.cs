using OpenBullet.Core.Models;

namespace OpenBullet.Core.Proxies;

/// <summary>
/// Interface for proxy management and rotation
/// </summary>
public interface IProxyManager
{
    /// <summary>
    /// Loads proxies from a file
    /// </summary>
    Task<int> LoadProxiesFromFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads proxies from a list of strings
    /// </summary>
    Task<int> LoadProxiesFromListAsync(IEnumerable<string> proxyStrings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a single proxy to the pool
    /// </summary>
    Task<bool> AddProxyAsync(ProxyInfo proxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a proxy from the pool
    /// </summary>
    Task<bool> RemoveProxyAsync(ProxyInfo proxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next available proxy based on rotation strategy
    /// </summary>
    Task<ProxyInfo?> GetNextProxyAsync(string? assignedTo = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a proxy to the pool and updates its statistics
    /// </summary>
    Task<bool> ReturnProxyAsync(ProxyInfo proxy, ProxyUsageResult result, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bans a proxy temporarily or permanently
    /// </summary>
    Task<bool> BanProxyAsync(ProxyInfo proxy, TimeSpan? banDuration = null, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unbans a proxy manually
    /// </summary>
    Task<bool> UnbanProxyAsync(ProxyInfo proxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests a proxy's connectivity and performance
    /// </summary>
    Task<ProxyTestResult> TestProxyAsync(ProxyInfo proxy, ProxyTestConfiguration? config = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests all proxies in the pool
    /// </summary>
    Task<ProxyPoolTestResult> TestAllProxiesAsync(ProxyTestConfiguration? config = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all proxies in the pool
    /// </summary>
    Task<List<ProxyInfo>> GetAllProxiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available (non-banned) proxies
    /// </summary>
    Task<List<ProxyInfo>> GetAvailableProxiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets banned proxies
    /// </summary>
    Task<List<ProxyInfo>> GetBannedProxiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets proxy pool statistics
    /// </summary>
    Task<ProxyPoolStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all proxies from the pool
    /// </summary>
    Task<bool> ClearPoolAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the proxy rotation strategy
    /// </summary>
    void SetRotationStrategy(ProxyRotationStrategy strategy);

    /// <summary>
    /// Sets proxy pool configuration
    /// </summary>
    void SetConfiguration(ProxyPoolConfiguration configuration);

    /// <summary>
    /// Event fired when a proxy is banned
    /// </summary>
    event EventHandler<ProxyBannedEventArgs>? ProxyBanned;

    /// <summary>
    /// Event fired when a proxy is unbanned
    /// </summary>
    event EventHandler<ProxyUnbannedEventArgs>? ProxyUnbanned;

    /// <summary>
    /// Event fired when proxy statistics are updated
    /// </summary>
    event EventHandler<ProxyStatisticsUpdatedEventArgs>? ProxyStatisticsUpdated;
}

/// <summary>
/// Enhanced proxy information with statistics and health tracking
/// </summary>
public class ProxyInfo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public ProxyType Type { get; set; } = ProxyType.Http;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Uses { get; set; }
    public DateTime LastUsed { get; set; }
    public bool IsBanned { get; set; }
    public DateTime? BannedUntil { get; set; }
    public string? BanReason { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTestedAt { get; set; }
    public ProxyHealth Health { get; set; } = ProxyHealth.Unknown;
    public string? AssignedTo { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets the success rate as percentage
    /// </summary>
    public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests * 100 : 0;

    /// <summary>
    /// Gets the total number of requests
    /// </summary>
    public int TotalRequests => SuccessfulRequests + FailedRequests;

    /// <summary>
    /// Checks if the proxy is currently banned
    /// </summary>
    public bool IsCurrentlyBanned => IsBanned && (BannedUntil == null || BannedUntil > DateTime.UtcNow);

    /// <summary>
    /// Checks if the proxy is available for use
    /// </summary>
    public bool IsAvailable => !IsCurrentlyBanned && Health != ProxyHealth.Dead;

    /// <summary>
    /// Gets the proxy address as string
    /// </summary>
    public string Address => $"{Host}:{Port}";

    /// <summary>
    /// Creates a copy of the proxy info
    /// </summary>
    public ProxyInfo Clone()
    {
        return new ProxyInfo
        {
            Id = Id,
            Host = Host,
            Port = Port,
            Type = Type,
            Username = Username,
            Password = Password,
            Uses = Uses,
            LastUsed = LastUsed,
            IsBanned = IsBanned,
            BannedUntil = BannedUntil,
            BanReason = BanReason,
            AverageResponseTime = AverageResponseTime,
            SuccessfulRequests = SuccessfulRequests,
            FailedRequests = FailedRequests,
            CreatedAt = CreatedAt,
            LastTestedAt = LastTestedAt,
            Health = Health,
            AssignedTo = AssignedTo,
            Metadata = new Dictionary<string, object>(Metadata)
        };
    }

    public override string ToString() => $"{Type}://{Address}";

    public override bool Equals(object? obj)
    {
        if (obj is ProxyInfo other)
        {
            return Id == other.Id || (Host == other.Host && Port == other.Port && Type == other.Type);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Host, Port, Type);
    }
}

/// <summary>
/// Proxy health status
/// </summary>
public enum ProxyHealth
{
    Unknown,
    Healthy,
    Slow,
    Unreliable,
    Dead
}

/// <summary>
/// Result of proxy usage
/// </summary>
public class ProxyUsageResult
{
    public bool Success { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public int ResponseCode { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool ShouldBan { get; set; }
    public string? BanReason { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Configuration for proxy testing
/// </summary>
public class ProxyTestConfiguration
{
    public string TestUrl { get; set; } = "https://httpbin.org/ip";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    public int MaxRetries { get; set; } = 1;
    public string ExpectedContent { get; set; } = string.Empty;
    public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
    public bool ValidateSsl { get; set; } = false;
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool TestDns { get; set; } = true;
    public List<string> DnsTestHosts { get; set; } = new() { "google.com", "cloudflare.com" };
}

/// <summary>
/// Result of proxy testing
/// </summary>
public class ProxyTestResult
{
    public ProxyInfo Proxy { get; set; } = new();
    public bool Success { get; set; }
    public double SuccessRate { get; set; } = 100.0; // Success rate percentage
    public TimeSpan ResponseTime { get; set; }
    public int ResponseCode { get; set; }
    public string? ResponseContent { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public DateTime TestedAt { get; set; } = DateTime.UtcNow;
    public ProxyHealth DeterminedHealth { get; set; } = ProxyHealth.Unknown;
    public Dictionary<string, object> TestMetadata { get; set; } = new();

    /// <summary>
    /// Determines proxy health based on test results
    /// </summary>
    public ProxyHealth DetermineHealth()
    {
        // Primary check: If success rate is too low, it's dead regardless of response time
        if (SuccessRate < 70.0)
        {
            DeterminedHealth = ProxyHealth.Dead;
        }
        // Secondary check: If response time is very slow, it's slow regardless of success rate
        else if (ResponseTime > TimeSpan.FromSeconds(10))
        {
            DeterminedHealth = ProxyHealth.Slow;
        }
        // Tertiary check: If success rate is mediocre or response time is moderate, it's unreliable
        else if (SuccessRate < 90.0 || ResponseTime > TimeSpan.FromSeconds(2))
        {
            DeterminedHealth = ProxyHealth.Unreliable;
        }
        // Otherwise it's healthy
        else
        {
            DeterminedHealth = ProxyHealth.Healthy;
        }

        return DeterminedHealth;
    }
}

/// <summary>
/// Result of testing all proxies in pool
/// </summary>
public class ProxyPoolTestResult
{
    public int TotalProxies { get; set; }
    public int TestedProxies { get; set; }
    public int HealthyProxies { get; set; }
    public int SlowProxies { get; set; }
    public int UnreliableProxies { get; set; }
    public int DeadProxies { get; set; }
    public TimeSpan TotalTestTime { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public List<ProxyTestResult> Results { get; set; } = new();
    public DateTime TestStartTime { get; set; } = DateTime.UtcNow;
    public DateTime TestEndTime { get; set; }

    /// <summary>
    /// Gets the health rate as percentage
    /// </summary>
    public double HealthRate => TestedProxies > 0 ? (double)HealthyProxies / TestedProxies * 100 : 0;

    /// <summary>
    /// Gets the total usable proxies (healthy + slow + unreliable)
    /// </summary>
    public int UsableProxies => HealthyProxies + SlowProxies + UnreliableProxies;

    /// <summary>
    /// Gets the usable rate as percentage
    /// </summary>
    public double UsableRate => TestedProxies > 0 ? (double)UsableProxies / TestedProxies * 100 : 0;
}

/// <summary>
/// Proxy rotation strategies
/// </summary>
public enum ProxyRotationStrategy
{
    RoundRobin,
    Random,
    LeastUsed,
    HealthBased,
    ResponseTimeBased,
    Sticky
}

/// <summary>
/// Proxy pool configuration
/// </summary>
public class ProxyPoolConfiguration
{
    public ProxyRotationStrategy RotationStrategy { get; set; } = ProxyRotationStrategy.RoundRobin;
    public int MaxConcurrentUses { get; set; } = 1;
    public TimeSpan AutoBanTimeout { get; set; } = TimeSpan.FromMinutes(10);
    public int MaxFailuresBeforeBan { get; set; } = 3;
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);
    public bool AutoUnbanEnabled { get; set; } = true;
    public TimeSpan AutoUnbanInterval { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan MaxResponseTime { get; set; } = TimeSpan.FromSeconds(30);
    public double MinSuccessRate { get; set; } = 70.0;
    public bool RemoveDeadProxies { get; set; } = false;
    public int MaxProxyRetries { get; set; } = 2;
    public bool LoadBalanceEnabled { get; set; } = true;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

/// <summary>
/// Proxy pool statistics
/// </summary>
public class ProxyPoolStatistics
{
    public int TotalProxies { get; set; }
    public int AvailableProxies { get; set; }
    public int BannedProxies { get; set; }
    public int HealthyProxies { get; set; }
    public int SlowProxies { get; set; }
    public int UnreliableProxies { get; set; }
    public int DeadProxies { get; set; }
    public long TotalRequests { get; set; }
    public long SuccessfulRequests { get; set; }
    public long FailedRequests { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public double OverallSuccessRate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public Dictionary<ProxyType, int> ProxiesByType { get; set; } = new();
    public Dictionary<ProxyHealth, int> ProxiesByHealth { get; set; } = new();
    public Dictionary<string, object> CustomMetrics { get; set; } = new();

    /// <summary>
    /// Gets the availability rate as percentage
    /// </summary>
    public double AvailabilityRate => TotalProxies > 0 ? (double)AvailableProxies / TotalProxies * 100 : 0;

    /// <summary>
    /// Gets the ban rate as percentage
    /// </summary>
    public double BanRate => TotalProxies > 0 ? (double)BannedProxies / TotalProxies * 100 : 0;

    /// <summary>
    /// Gets the health rate as percentage
    /// </summary>
    public double HealthRate => TotalProxies > 0 ? (double)HealthyProxies / TotalProxies * 100 : 0;
}

/// <summary>
/// Event arguments for proxy banned event
/// </summary>
public class ProxyBannedEventArgs : EventArgs
{
    public ProxyInfo Proxy { get; set; } = new();
    public string? Reason { get; set; }
    public TimeSpan? BanDuration { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event arguments for proxy unbanned event
/// </summary>
public class ProxyUnbannedEventArgs : EventArgs
{
    public ProxyInfo Proxy { get; set; } = new();
    public bool WasAutomatic { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event arguments for proxy statistics updated event
/// </summary>
public class ProxyStatisticsUpdatedEventArgs : EventArgs
{
    public ProxyInfo Proxy { get; set; } = new();
    public ProxyUsageResult UsageResult { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
