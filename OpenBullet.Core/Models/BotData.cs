using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Logging;

namespace OpenBullet.Core.Models;

/// <summary>
/// Represents the execution context and data for a bot instance
/// </summary>
public class BotData
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string DataLine { get; set; } = string.Empty;
    public ConcurrentDictionary<string, object> Variables { get; } = new();
    public ConcurrentDictionary<string, object> CapturedData { get; } = new();
    public CookieContainer Cookies { get; } = new();
    public ProxyInfo? Proxy { get; set; }
    public BotStatus Status { get; set; } = BotStatus.None;
    public string CustomStatus { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int ResponseCode { get; set; }
    public CancellationToken CancellationToken { get; set; }
    public List<string> Log { get; } = new();
    public DateTime StartTime { get; set; }
    public long ExecutionTime { get; set; }

    public BotData(string dataLine, ConfigModel config, ILogger logger, CancellationToken cancellationToken)
    {
        DataLine = dataLine;
        CancellationToken = cancellationToken;
        StartTime = DateTime.UtcNow;
        // Store config and logger references if needed
    }

    public void SetVariable(string name, object value)
    {
        Variables[name] = value;
    }

    public T? GetVariable<T>(string name)
    {
        if (Variables.TryGetValue(name, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    public void SetCapture(string name, object value)
    {
        CapturedData[name] = value;
    }

    public void AddLogEntry(string message)
    {
        Log.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}

/// <summary>
/// Bot execution status enumeration
/// </summary>
public enum BotStatus
{
    None,
    Success,
    Failure,
    Retry,
    Ban,
    Error,
    Custom
}

/// <summary>
/// Proxy information model
/// </summary>
public class ProxyInfo
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public ProxyType Type { get; set; } = ProxyType.Http;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Uses { get; set; }
    public DateTime LastUsed { get; set; }
    public bool IsBanned { get; set; }

    public override string ToString() => $"{Host}:{Port}";
}

/// <summary>
/// Proxy type enumeration
/// </summary>
public enum ProxyType
{
    Http,
    Socks4,
    Socks5
}
