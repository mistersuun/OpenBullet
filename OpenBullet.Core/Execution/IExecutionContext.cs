using System.Net;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using OpenBullet.Core.Variables;

namespace OpenBullet.Core.Execution;

/// <summary>
/// Interface for script execution context
/// </summary>
public interface IExecutionContext
{
    /// <summary>
    /// Unique identifier for this execution context
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Bot data containing input data and execution state
    /// </summary>
    BotData BotData { get; }

    /// <summary>
    /// Variable manager for this execution
    /// </summary>
    IVariableManager VariableManager { get; }

    /// <summary>
    /// HTTP client service for web requests
    /// </summary>
    IHttpClientService HttpClient { get; }

    /// <summary>
    /// Cancellation token for stopping execution
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Current execution status
    /// </summary>
    ExecutionStatus Status { get; set; }

    /// <summary>
    /// Execution start time
    /// </summary>
    DateTime StartTime { get; }

    /// <summary>
    /// Current execution time
    /// </summary>
    TimeSpan ElapsedTime { get; }

    /// <summary>
    /// Proxy information if using proxy
    /// </summary>
    ProxyInfo? Proxy { get; set; }

    /// <summary>
    /// Cookie container for HTTP requests
    /// </summary>
    CookieContainer Cookies { get; }

    /// <summary>
    /// Custom headers for HTTP requests
    /// </summary>
    Dictionary<string, string> Headers { get; }

    /// <summary>
    /// Last HTTP response source
    /// </summary>
    string ResponseSource { get; set; }

    /// <summary>
    /// Last HTTP response code
    /// </summary>
    int ResponseCode { get; set; }

    /// <summary>
    /// Last HTTP response address
    /// </summary>
    string ResponseAddress { get; set; }

    /// <summary>
    /// Execution log entries
    /// </summary>
    IList<LogEntry> Log { get; }

    /// <summary>
    /// Captured data during execution
    /// </summary>
    Dictionary<string, object> CapturedData { get; }

    /// <summary>
    /// Custom status message
    /// </summary>
    string CustomStatus { get; set; }

    /// <summary>
    /// Whether to use proxy for requests
    /// </summary>
    bool UseProxy { get; set; }

    /// <summary>
    /// Execution statistics
    /// </summary>
    ExecutionStatistics Statistics { get; }

    /// <summary>
    /// Execution environment settings
    /// </summary>
    ExecutionEnvironment Environment { get; }

    /// <summary>
    /// Adds a log entry
    /// </summary>
    void AddLog(string message, LogLevel level = LogLevel.Info);

    /// <summary>
    /// Adds a log entry with exception
    /// </summary>
    void AddLog(string message, Exception exception, LogLevel level = LogLevel.Error);

    /// <summary>
    /// Sets a variable
    /// </summary>
    void SetVariable(string name, object? value);

    /// <summary>
    /// Gets a variable
    /// </summary>
    T? GetVariable<T>(string name);

    /// <summary>
    /// Sets captured data
    /// </summary>
    void SetCapture(string name, object value);

    /// <summary>
    /// Gets captured data
    /// </summary>
    T? GetCapture<T>(string name);

    /// <summary>
    /// Updates HTTP response information
    /// </summary>
    void UpdateResponse(string source, int statusCode, string address);

    /// <summary>
    /// Creates a checkpoint for rollback
    /// </summary>
    ExecutionCheckpoint CreateCheckpoint(string name);

    /// <summary>
    /// Restores from a checkpoint
    /// </summary>
    void RestoreCheckpoint(ExecutionCheckpoint checkpoint);

    /// <summary>
    /// Validates the execution context
    /// </summary>
    ContextValidationResult Validate();

    /// <summary>
    /// Event fired when status changes
    /// </summary>
    event EventHandler<ExecutionStatusChangedEventArgs>? StatusChanged;

    /// <summary>
    /// Event fired when a variable is captured
    /// </summary>
    event EventHandler<DataCapturedEventArgs>? DataCaptured;
}

/// <summary>
/// Execution status enumeration
/// </summary>
public enum ExecutionStatus
{
    NotStarted,
    Running,
    Paused,
    Completed,
    Failed,
    Cancelled,
    Retrying
}

/// <summary>
/// Log entry for execution context
/// </summary>
public class LogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Message { get; set; } = string.Empty;
    public LogLevel Level { get; set; } = LogLevel.Info;
    public Exception? Exception { get; set; }
    public string Source { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
}

/// <summary>
/// Log level enumeration
/// </summary>
public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
    Critical = 5
}

/// <summary>
/// Execution statistics
/// </summary>
public class ExecutionStatistics
{
    public int HttpRequestCount;
    public int VariableSetCount { get; set; }
    public int VariableGetCount { get; set; }
    public int DataCaptureCount { get; set; }
    public int LogEntryCount { get; set; }
    public long TotalResponseTime { get; set; }
    public long AverageResponseTime => HttpRequestCount > 0 ? TotalResponseTime / HttpRequestCount : 0;
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public DateTime LastHttpRequest { get; set; }
    public Dictionary<string, int> CommandCounts { get; set; } = new();

    // Extended properties for comprehensive statistics
    public int TotalScriptsExecuted { get; set; }
    public int TotalCommandsExecuted { get; set; }
    public int SuccessfulExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public TimeSpan AverageExecutionTime => TotalScriptsExecuted > 0 ? new TimeSpan(TotalExecutionTime.Ticks / TotalScriptsExecuted) : TimeSpan.Zero;
    public Dictionary<string, int> CommandExecutionCounts { get; set; } = new();
    public Dictionary<string, TimeSpan> CommandExecutionTimes { get; set; } = new();
    public Dictionary<string, int> StatusCounts { get; set; } = new();
    public Dictionary<string, object> CustomMetrics { get; set; } = new();
    public double SuccessRate => TotalScriptsExecuted > 0 ? (double)SuccessfulExecutions / TotalScriptsExecuted : 0.0;
    public double CommandsPerMinute => TotalExecutionTime.TotalMinutes > 0 ? TotalCommandsExecuted / TotalExecutionTime.TotalMinutes : 0.0;
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime LastExecutionTime { get; set; }

    /// <summary>
    /// Update statistics with data from a script execution result
    /// </summary>
    public void UpdateWith(ScriptExecutionResult result)
    {
        TotalScriptsExecuted++;
        TotalCommandsExecuted += result.CommandsExecuted;
        TotalExecutionTime = TotalExecutionTime.Add(result.ExecutionTime);
        
        if (result.Success)
        {
            SuccessfulExecutions++;
        }
        else
        {
            FailedExecutions++;
        }
        
        // Update status counts
        StatusCounts[result.Status.ToString()] = StatusCounts.GetValueOrDefault(result.Status.ToString(), 0) + 1;
        
        // Update command execution counts
        foreach (var cmdResult in result.CommandResults)
        {
            CommandExecutionCounts[cmdResult.CommandName] = CommandExecutionCounts.GetValueOrDefault(cmdResult.CommandName, 0) + 1;
            CommandExecutionTimes[cmdResult.CommandName] = CommandExecutionTimes.GetValueOrDefault(cmdResult.CommandName, TimeSpan.Zero).Add(cmdResult.ExecutionTime);
        }
        
        LastExecutionTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Update statistics with data from another statistics object
    /// </summary>
    public void UpdateWith(ExecutionStatistics other)
    {
        HttpRequestCount += other.HttpRequestCount;
        VariableSetCount += other.VariableSetCount;
        VariableGetCount += other.VariableGetCount;
        DataCaptureCount += other.DataCaptureCount;
        LogEntryCount += other.LogEntryCount;
        TotalResponseTime += other.TotalResponseTime;
        ErrorCount += other.ErrorCount;
        WarningCount += other.WarningCount;
        TotalScriptsExecuted += other.TotalScriptsExecuted;
        TotalCommandsExecuted += other.TotalCommandsExecuted;
        SuccessfulExecutions += other.SuccessfulExecutions;
        FailedExecutions += other.FailedExecutions;
        TotalExecutionTime = TotalExecutionTime.Add(other.TotalExecutionTime);
        
        // Merge dictionaries
        foreach (var kvp in other.CommandCounts)
        {
            CommandCounts[kvp.Key] = CommandCounts.GetValueOrDefault(kvp.Key, 0) + kvp.Value;
        }
        foreach (var kvp in other.CommandExecutionCounts)
        {
            CommandExecutionCounts[kvp.Key] = CommandExecutionCounts.GetValueOrDefault(kvp.Key, 0) + kvp.Value;
        }
        foreach (var kvp in other.CommandExecutionTimes)
        {
            CommandExecutionTimes[kvp.Key] = CommandExecutionTimes.GetValueOrDefault(kvp.Key, TimeSpan.Zero).Add(kvp.Value);
        }
        foreach (var kvp in other.StatusCounts)
        {
            StatusCounts[kvp.Key] = StatusCounts.GetValueOrDefault(kvp.Key, 0) + kvp.Value;
        }
        foreach (var kvp in other.CustomMetrics)
        {
            CustomMetrics[kvp.Key] = kvp.Value;
        }
        
        LastHttpRequest = other.LastHttpRequest > LastHttpRequest ? other.LastHttpRequest : LastHttpRequest;
        LastExecutionTime = other.LastExecutionTime > LastExecutionTime ? other.LastExecutionTime : LastExecutionTime;
    }

    /// <summary>
    /// Reset all statistics to initial values
    /// </summary>
    public void Reset()
    {
        HttpRequestCount = 0;
        VariableSetCount = 0;
        VariableGetCount = 0;
        DataCaptureCount = 0;
        LogEntryCount = 0;
        TotalResponseTime = 0;
        ErrorCount = 0;
        WarningCount = 0;
        TotalScriptsExecuted = 0;
        TotalCommandsExecuted = 0;
        SuccessfulExecutions = 0;
        FailedExecutions = 0;
        TotalExecutionTime = TimeSpan.Zero;
        
        CommandCounts.Clear();
        CommandExecutionCounts.Clear();
        CommandExecutionTimes.Clear();
        StatusCounts.Clear();
        CustomMetrics.Clear();
        
        StartTime = DateTime.UtcNow;
        LastHttpRequest = DateTime.MinValue;
        LastExecutionTime = DateTime.MinValue;
    }
}

/// <summary>
/// Execution environment settings
/// </summary>
public class ExecutionEnvironment
{
    public string UserAgent { get; set; } = "OpenBullet/2.0";
    public int RequestTimeout { get; set; } = 10000; // milliseconds
    public int MaxRedirects { get; set; } = 8;
    public bool IgnoreSSLErrors { get; set; } = false;
    public bool AutoRedirect { get; set; } = true;
    public string AcceptLanguage { get; set; } = "en-US,en;q=0.9";
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
    public bool EnableLogging { get; set; } = true;
    public LogLevel LogLevel { get; set; } = LogLevel.Info;
}

/// <summary>
/// Execution checkpoint for rollback
/// </summary>
public class ExecutionCheckpoint
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ExecutionStatus Status { get; set; }
    public Dictionary<string, object> CapturedData { get; set; } = new();
    public string ResponseSource { get; set; } = string.Empty;
    public int ResponseCode { get; set; }
    public string ResponseAddress { get; set; } = string.Empty;
    public VariableSnapshot VariableSnapshot { get; set; } = null!;
    public string CustomStatus { get; set; } = string.Empty;
}

/// <summary>
/// Context validation result
/// </summary>
public class ContextValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Execution status changed event arguments
/// </summary>
public class ExecutionStatusChangedEventArgs : EventArgs
{
    public string ContextId { get; set; } = string.Empty;
    public ExecutionStatus OldStatus { get; set; }
    public ExecutionStatus NewStatus { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Message { get; set; }
}

/// <summary>
/// Data captured event arguments
/// </summary>
public class DataCapturedEventArgs : EventArgs
{
    public string Name { get; set; } = string.Empty;
    public object Value { get; set; } = null!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = string.Empty;
}
