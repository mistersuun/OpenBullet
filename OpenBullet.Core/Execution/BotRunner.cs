using Microsoft.Extensions.Logging;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using System.Diagnostics;

namespace OpenBullet.Core.Execution;

/// <summary>
/// Implementation of bot runner for executing individual bot instances
/// </summary>
public class BotRunner : IBotRunner
{
    private readonly ILogger<BotRunner> _logger;
    private readonly IScriptEngine _scriptEngine;

    public event EventHandler<BotStatusEventArgs>? StatusChanged;

    public BotRunner(ILogger<BotRunner> logger, IScriptEngine scriptEngine)
    {
        _logger = logger;
        _scriptEngine = scriptEngine;
    }

    public async Task<BasicBotResult> ExecuteAsync(BotData data, ConfigModel config)
    {
        try
        {
            var result = await RunAsync(config, data.DataLine, CancellationToken.None);
            return new BasicBotResult
            {
                Status = result.Status,
                CustomStatus = result.CustomStatus ?? string.Empty,
                CapturedData = result.CapturedData,
                DataLine = result.DataLine
            };
        }
        catch (Exception ex)
        {
            return new BasicBotResult
            {
                Status = BotStatus.Error,
                CustomStatus = ex.Message,
                DataLine = data.DataLine
            };
        }
    }

    public void Stop()
    {
        // Implementation for stopping bot execution
        _logger.LogInformation("Bot execution stopped");
    }

    public async Task<BotResult> RunAsync(ConfigModel config, string dataLine, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var botId = Guid.NewGuid().ToString()[..8];

        try
        {
            _logger.LogDebug("Starting bot {BotId} with config {ConfigName} and data line {DataLine}", 
                botId, config.Name, dataLine.Length > 50 ? dataLine[..50] + "..." : dataLine);

            // Create bot data
            var botData = new BotData(dataLine, config, _logger, cancellationToken)
            {
                StartTime = DateTime.UtcNow
            };

            // Execute script
            var executionResult = await _scriptEngine.ExecuteAsync(config, botData, cancellationToken);

            // Create bot result
            var result = new BotResult
            {
                BotId = botId,
                DataLine = dataLine,
                ConfigName = config.Name,
                Status = executionResult.Status,
                CustomStatus = executionResult.CustomStatus ?? string.Empty,
                ExecutionTime = executionResult.ExecutionTime, // Use script execution time instead of stopwatch
                Success = executionResult.Success,
                Variables = new Dictionary<string, object>(executionResult.Variables),
                CapturedData = new Dictionary<string, object>(executionResult.CapturedData),
                Logs = new List<string>(executionResult.Logs),
                ErrorMessage = executionResult.ErrorMessage,
                Exception = executionResult.Exception,
                StartTime = botData.StartTime,
                EndTime = DateTime.UtcNow
            };

            // Add execution metadata
            result.Metadata["CommandsExecuted"] = executionResult.CommandsExecuted;
            result.Metadata["ScriptExecutionTime"] = executionResult.ExecutionTime.TotalMilliseconds;
            result.Metadata["TotalLogEntries"] = executionResult.Logs.Count;

            // Add HTTP-specific metadata if available
            if (botData.ResponseCode > 0)
            {
                result.Metadata["LastResponseCode"] = botData.ResponseCode;
                result.Metadata["LastAddress"] = botData.Address;
                result.Metadata["SourceLength"] = botData.Source?.Length ?? 0;
            }

            // Add proxy information if available
            if (botData.Proxy != null)
            {
                result.Metadata["ProxyUsed"] = botData.Proxy.ToString();
                result.Metadata["ProxyType"] = botData.Proxy.Type.ToString();
            }

            _logger.LogDebug("Bot {BotId} completed in {ExecutionTime}ms with status {Status}", 
                botId, result.ExecutionTime.TotalMilliseconds, result.Status);

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Bot {BotId} was cancelled", botId);
            return new BotResult
            {
                BotId = botId,
                DataLine = dataLine,
                ConfigName = config.Name,
                Status = BotStatus.Error,
                Success = false,
                ErrorMessage = "Bot execution was cancelled",
                ExecutionTime = stopwatch.Elapsed,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot {BotId} failed with exception", botId);
            return new BotResult
            {
                BotId = botId,
                DataLine = dataLine,
                ConfigName = config.Name,
                Status = BotStatus.Error,
                Success = false,
                ErrorMessage = ex.Message,
                Exception = ex,
                ExecutionTime = stopwatch.Elapsed,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow
            };
        }
    }
}

/// <summary>
/// Result of bot execution
/// </summary>
public class BotResult
{
    public string BotId { get; set; } = string.Empty;
    public string DataLine { get; set; } = string.Empty;
    public string ConfigName { get; set; } = string.Empty;
    public BotStatus Status { get; set; } = BotStatus.None;
    public string CustomStatus { get; set; } = string.Empty;
    public bool Success { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public Dictionary<string, object> Variables { get; set; } = new();
    public Dictionary<string, object> CapturedData { get; set; } = new();
    public List<string> Logs { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets a captured value by name
    /// </summary>
    public T? GetCapturedValue<T>(string name)
    {
        if (CapturedData.TryGetValue(name, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Gets a variable value by name
    /// </summary>
    public T? GetVariableValue<T>(string name)
    {
        if (Variables.TryGetValue(name, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Checks if the result contains captured data
    /// </summary>
    public bool HasCapturedData => CapturedData.Count > 0;

    /// <summary>
    /// Checks if the result contains variables
    /// </summary>
    public bool HasVariables => Variables.Count > 0;

    /// <summary>
    /// Gets the execution rate (commands per second)
    /// </summary>
    public double GetExecutionRate()
    {
        if (Metadata.TryGetValue("CommandsExecuted", out var commandsObj) && 
            commandsObj is int commands && 
            ExecutionTime.TotalSeconds > 0)
        {
            return commands / ExecutionTime.TotalSeconds;
        }
        return 0;
    }

    /// <summary>
    /// Creates a summary string of the result
    /// </summary>
    public string GetSummary()
    {
        var status = Status == BotStatus.Custom && !string.IsNullOrEmpty(CustomStatus) 
            ? CustomStatus 
            : Status.ToString();
        
        return $"Bot {BotId}: {status} in {ExecutionTime.TotalMilliseconds:F0}ms";
    }

    /// <summary>
    /// Gets all captured data as formatted strings
    /// </summary>
    public Dictionary<string, string> GetFormattedCapturedData()
    {
        return CapturedData.ToDictionary(
            kvp => kvp.Key, 
            kvp => kvp.Value?.ToString() ?? string.Empty
        );
    }
}

/// <summary>
/// Enhanced bot runner with additional features
/// </summary>
public class EnhancedBotRunner : BotRunner
{
    private readonly ILogger<EnhancedBotRunner> _enhancedLogger;

    public EnhancedBotRunner(
        ILogger<EnhancedBotRunner> logger, 
        IScriptEngine scriptEngine) : base(logger, scriptEngine)
    {
        _enhancedLogger = logger;
    }

    public new async Task<BotResult> RunAsync(ConfigModel config, string dataLine, CancellationToken cancellationToken = default)
    {
        // Pre-execution validation
        var validationResult = ValidateConfiguration(config);
        if (!validationResult.IsValid)
        {
            _enhancedLogger.LogWarning("Configuration validation failed for {ConfigName}: {Errors}", 
                config.Name, string.Join(", ", validationResult.Errors.Select(e => e.Message)));
        }

        // Execute with enhanced monitoring
        var result = await base.RunAsync(config, dataLine, cancellationToken);

        // Post-execution analysis
        AnalyzeResult(result);

        return result;
    }

    private ScriptValidationResult ValidateConfiguration(ConfigModel config)
    {
        var result = new ScriptValidationResult { IsValid = true };

        // Basic validation
        if (string.IsNullOrWhiteSpace(config.Name))
        {
            result.Errors.Add(new ValidationError
            {
                CommandName = "CONFIG",
                Message = "Configuration name is required",
                Severity = ValidationSeverity.Warning
            });
        }

        if (string.IsNullOrWhiteSpace(config.Script))
        {
            result.Errors.Add(new ValidationError
            {
                CommandName = "CONFIG",
                Message = "Configuration script is empty",
                Severity = ValidationSeverity.Critical
            });
            result.IsValid = false;
        }

        return result;
    }

    private void AnalyzeResult(BotResult result)
    {
        // Performance analysis
        var executionRate = result.GetExecutionRate();
        if (executionRate > 0)
        {
            result.Metadata["ExecutionRate"] = executionRate;
            
            if (executionRate < 1.0)
            {
                _enhancedLogger.LogInformation("Slow execution detected for bot {BotId}: {Rate:F2} commands/sec", 
                    result.BotId, executionRate);
            }
        }

        // Data analysis
        if (result.HasCapturedData)
        {
            result.Metadata["CapturedDataKeys"] = result.CapturedData.Keys.ToList();
            _enhancedLogger.LogTrace("Bot {BotId} captured {Count} data items: {Keys}", 
                result.BotId, result.CapturedData.Count, string.Join(", ", result.CapturedData.Keys));
        }

        // Log analysis
        if (result.Logs.Count > 0)
        {
            var errorLogs = result.Logs.Count(log => log.Contains("Error") || log.Contains("Failed"));
            var warningLogs = result.Logs.Count(log => log.Contains("Warning"));
            
            result.Metadata["ErrorLogCount"] = errorLogs;
            result.Metadata["WarningLogCount"] = warningLogs;
            
            if (errorLogs > 0)
            {
                _enhancedLogger.LogWarning("Bot {BotId} has {ErrorCount} error log entries", 
                    result.BotId, errorLogs);
            }
        }
    }
}
