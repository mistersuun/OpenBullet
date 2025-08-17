using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using OpenBullet.Core.Variables;

namespace OpenBullet.Core.Execution;

/// <summary>
/// Execution context implementation
/// </summary>
public class ExecutionContext : IExecutionContext, IDisposable
{
    private readonly ILogger<ExecutionContext> _logger;
    private readonly Stopwatch _stopwatch;
    private readonly object _lockObject = new();
    private bool _disposed = false;

    public string Id { get; }
    public BotData BotData { get; }
    public IVariableManager VariableManager { get; }
    public IHttpClientService HttpClient { get; }
    public CancellationToken CancellationToken { get; }
    
    private ExecutionStatus _status = ExecutionStatus.NotStarted;
    public ExecutionStatus Status 
    { 
        get => _status;
        set
        {
            if (_status != value)
            {
                var oldStatus = _status;
                _status = value;
                OnStatusChanged(new ExecutionStatusChangedEventArgs
                {
                    ContextId = Id,
                    OldStatus = oldStatus,
                    NewStatus = value
                });
            }
        }
    }

    public DateTime StartTime { get; }
    public TimeSpan ElapsedTime => _stopwatch.Elapsed;
    public ProxyInfo? Proxy { get; set; }
    public CookieContainer Cookies { get; }
    public Dictionary<string, string> Headers { get; }
    public string ResponseSource { get; set; } = string.Empty;
    public int ResponseCode { get; set; }
    public string ResponseAddress { get; set; } = string.Empty;
    public IList<LogEntry> Log { get; }
    public Dictionary<string, object> CapturedData { get; }
    public string CustomStatus { get; set; } = string.Empty;
    public bool UseProxy { get; set; } = true;
    public ExecutionStatistics Statistics { get; }
    public ExecutionEnvironment Environment { get; }

    public event EventHandler<ExecutionStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<DataCapturedEventArgs>? DataCaptured;

    public ExecutionContext(
        BotData botData,
        IVariableManager variableManager,
        IHttpClientService httpClient,
        CancellationToken cancellationToken,
        ILogger<ExecutionContext> logger)
    {
        Id = Guid.NewGuid().ToString();
        BotData = botData ?? throw new ArgumentNullException(nameof(botData));
        VariableManager = variableManager ?? throw new ArgumentNullException(nameof(variableManager));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        CancellationToken = cancellationToken;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        StartTime = DateTime.UtcNow;
        _stopwatch = Stopwatch.StartNew();
        
        Cookies = new CookieContainer();
        Headers = new Dictionary<string, string>();
        Log = new List<LogEntry>();
        CapturedData = new Dictionary<string, object>();
        Statistics = new ExecutionStatistics();
        Environment = new ExecutionEnvironment();

        // Initialize with bot data
        InitializeFromBotData();
        
        // Subscribe to variable manager events
        VariableManager.VariableChanged += OnVariableChanged;

        Status = ExecutionStatus.Running;
        
        _logger.LogDebug("ExecutionContext {Id} created for data line: {DataLine}", Id, botData.DataLine);
    }

    public void AddLog(string message, LogLevel level = LogLevel.Info)
    {
        if (!Environment.EnableLogging || level < Environment.LogLevel)
            return;

        lock (_lockObject)
        {
            var entry = new LogEntry
            {
                Message = message,
                Level = level,
                Source = "ExecutionContext"
            };
            
            Log.Add(entry);
            Statistics.LogEntryCount++;
            
            if (level >= LogLevel.Warning)
                Statistics.WarningCount++;
            if (level >= LogLevel.Error)
                Statistics.ErrorCount++;
        }

        // Also add to BotData log for compatibility
        BotData.AddLogEntry($"[{level}] {message}");

        _logger.Log(MapLogLevel(level), "Context {Id}: {Message}", Id, message);
    }

    public void AddLog(string message, Exception exception, LogLevel level = LogLevel.Error)
    {
        if (!Environment.EnableLogging || level < Environment.LogLevel)
            return;

        lock (_lockObject)
        {
            var entry = new LogEntry
            {
                Message = message,
                Level = level,
                Exception = exception,
                Source = "ExecutionContext"
            };
            
            Log.Add(entry);
            Statistics.LogEntryCount++;
            Statistics.ErrorCount++;
        }

        BotData.AddLogEntry($"[{level}] {message}: {exception.Message}");

        _logger.Log(MapLogLevel(level), exception, "Context {Id}: {Message}", Id, message);
    }

    public void SetVariable(string name, object? value)
    {
        VariableManager.SetVariable(name, value, VariableScope.Local);
        Statistics.VariableSetCount++;
        
        // Also update BotData for compatibility
        BotData.SetVariable(name, value);
    }

    public T? GetVariable<T>(string name)
    {
        Statistics.VariableGetCount++;
        return VariableManager.GetVariable<T>(name);
    }

    public void SetCapture(string name, object value)
    {
        lock (_lockObject)
        {
            CapturedData[name] = value;
            Statistics.DataCaptureCount++;
        }

        // Also update BotData for compatibility
        BotData.SetCapture(name, value);

        OnDataCaptured(new DataCapturedEventArgs
        {
            Name = name,
            Value = value,
            Source = "ExecutionContext"
        });

        _logger.LogTrace("Context {Id}: Captured {Name} = {Value}", Id, name, value);
    }

    public T? GetCapture<T>(string name)
    {
        lock (_lockObject)
        {
            if (CapturedData.TryGetValue(name, out var value) && value is T typedValue)
            {
                return typedValue;
            }
        }
        
        return default(T);
    }

    public void UpdateResponse(string source, int statusCode, string address)
    {
        ResponseSource = source ?? string.Empty;
        ResponseCode = statusCode;
        ResponseAddress = address ?? string.Empty;
        Interlocked.Increment(ref Statistics.HttpRequestCount);
        Statistics.LastHttpRequest = DateTime.UtcNow;

        // Update BotData for compatibility
        BotData.Source = ResponseSource;
        BotData.ResponseCode = ResponseCode;
        BotData.Address = ResponseAddress;

        _logger.LogTrace("Context {Id}: Updated response - Status: {StatusCode}, Address: {Address}", 
            Id, statusCode, address);
    }

    public ExecutionCheckpoint CreateCheckpoint(string name)
    {
        var checkpoint = new ExecutionCheckpoint
        {
            Name = name,
            Status = Status,
            CapturedData = new Dictionary<string, object>(CapturedData),
            ResponseSource = ResponseSource,
            ResponseCode = ResponseCode,
            ResponseAddress = ResponseAddress,
            CustomStatus = CustomStatus,
            VariableSnapshot = VariableManager.CreateSnapshot()
        };

        _logger.LogDebug("Context {Id}: Created checkpoint '{Name}'", Id, name);
        return checkpoint;
    }

    public void RestoreCheckpoint(ExecutionCheckpoint checkpoint)
    {
        ArgumentNullException.ThrowIfNull(checkpoint);

        Status = checkpoint.Status;
        CustomStatus = checkpoint.CustomStatus;
        ResponseSource = checkpoint.ResponseSource;
        ResponseCode = checkpoint.ResponseCode;
        ResponseAddress = checkpoint.ResponseAddress;

        lock (_lockObject)
        {
            CapturedData.Clear();
            foreach (var kvp in checkpoint.CapturedData)
            {
                CapturedData[kvp.Key] = kvp.Value;
            }
        }

        VariableManager.RestoreSnapshot(checkpoint.VariableSnapshot);

        _logger.LogDebug("Context {Id}: Restored checkpoint '{Name}'", Id, checkpoint.Name);
    }

    public ContextValidationResult Validate()
    {
        var result = new ContextValidationResult { IsValid = true };

        // Validate required components
        if (BotData == null)
        {
            result.Errors.Add("BotData is null");
            result.IsValid = false;
        }

        if (VariableManager == null)
        {
            result.Errors.Add("VariableManager is null");
            result.IsValid = false;
        }

        if (HttpClient == null)
        {
            result.Errors.Add("HttpClient is null");
            result.IsValid = false;
        }

        // Validate data line
        if (string.IsNullOrEmpty(BotData?.DataLine))
        {
            result.Warnings.Add("BotData.DataLine is empty");
        }

        // Validate environment settings
        if (Environment.RequestTimeout <= 0)
        {
            result.Warnings.Add("RequestTimeout should be greater than 0");
        }

        if (Environment.MaxRedirects < 0)
        {
            result.Warnings.Add("MaxRedirects should not be negative");
        }

        // Check for cancellation
        if (CancellationToken.IsCancellationRequested)
        {
            result.Warnings.Add("Execution has been cancelled");
        }

        return result;
    }

    private void InitializeFromBotData()
    {
        // Initialize variables from BotData
        foreach (var kvp in BotData.Variables)
        {
            VariableManager.SetVariable(kvp.Key, kvp.Value, VariableScope.Local);
        }

        // Initialize captured data
        foreach (var kvp in BotData.CapturedData)
        {
            CapturedData[kvp.Key] = kvp.Value;
        }

        // Initialize response state
        ResponseSource = BotData.Source;
        ResponseCode = BotData.ResponseCode;
        ResponseAddress = BotData.Address;

        // Initialize cookies from BotData
        // Copy cookies from BotData - CookieContainer doesn't support direct enumeration
        if (BotData.Cookies.Count > 0)
        {
            // TODO: Implement proper cookie copying from CookieContainer
            // For now, skip cookie copying since CookieContainer doesn't support enumeration
            _logger.LogDebug("Skipping cookie copy - CookieContainer enumeration not supported");
        }

        // Set proxy if available
        Proxy = BotData.Proxy;
    }

    private void OnVariableChanged(object? sender, VariableChangedEventArgs e)
    {
        if (e.ChangeType == VariableChangeType.Created || e.ChangeType == VariableChangeType.Updated)
        {
            // Sync back to BotData for compatibility
            if (e.NewValue != null)
            {
                BotData.SetVariable(e.VariableName, e.NewValue);
            }
        }
    }

    private void OnStatusChanged(ExecutionStatusChangedEventArgs e)
    {
        try
        {
            StatusChanged?.Invoke(this, e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in status changed event handler");
        }
    }

    private void OnDataCaptured(DataCapturedEventArgs e)
    {
        try
        {
            DataCaptured?.Invoke(this, e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in data captured event handler");
        }
    }

    private Microsoft.Extensions.Logging.LogLevel MapLogLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            LogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => Microsoft.Extensions.Logging.LogLevel.Information
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _stopwatch.Stop();
            
            // Unsubscribe from events
            if (VariableManager != null)
            {
                VariableManager.VariableChanged -= OnVariableChanged;
            }
            
            // Update BotData with final state
            BotData.Status = Status switch
            {
                ExecutionStatus.Completed => BotStatus.Success,
                ExecutionStatus.Failed => BotStatus.Error,
                ExecutionStatus.Cancelled => BotStatus.Ban,
                _ => BotStatus.None
            };
            
            BotData.ExecutionTime = _stopwatch.ElapsedMilliseconds;
            BotData.CustomStatus = CustomStatus;

            _disposed = true;
            Status = ExecutionStatus.Completed;
            
            _logger.LogDebug("ExecutionContext {Id} disposed after {ElapsedTime}ms", Id, _stopwatch.ElapsedMilliseconds);
        }
        
        GC.SuppressFinalize(this);
    }
}
