using OpenBullet.Core.Models;
using OpenBullet.Core.Execution;

namespace OpenBullet.Core.Interfaces;

/// <summary>
/// Interface for bot execution engine
/// </summary>
public interface IBotRunner
{
    /// <summary>
    /// Executes a bot with the provided data and configuration
    /// </summary>
    Task<BasicBotResult> ExecuteAsync(BotData data, ConfigModel config);

    /// <summary>
    /// Runs a bot with configuration and data line, returning detailed result
    /// </summary>
    Task<BotResult> RunAsync(ConfigModel config, string dataLine, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the bot execution
    /// </summary>
    void Stop();

    /// <summary>
    /// Event fired when bot status changes
    /// </summary>
    event EventHandler<BotStatusEventArgs>? StatusChanged;
}

/// <summary>
/// Basic result of bot execution
/// </summary>
public class BasicBotResult
{
    public BotStatus Status { get; set; }
    public string CustomStatus { get; set; } = string.Empty;
    public Dictionary<string, object> CapturedData { get; set; } = new();
    public string DataLine { get; set; } = string.Empty;
    public ProxyInfo? Proxy { get; set; }
    public long ExecutionTime { get; set; }
    public List<string> Log { get; set; } = new();
    public Exception? Exception { get; set; }
    
    // Additional properties for compatibility
    public bool Success => Status == BotStatus.Success;
    public string BotId { get; set; } = Guid.NewGuid().ToString();
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Event arguments for bot status changes
/// </summary>
public class BotStatusEventArgs : EventArgs
{
    public string BotId { get; set; } = string.Empty;
    public BotStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
}
