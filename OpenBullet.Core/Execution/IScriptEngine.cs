using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;

namespace OpenBullet.Core.Execution;

/// <summary>
/// Interface for script execution engine
/// </summary>
public interface IScriptEngine
{
    /// <summary>
    /// Executes a complete script
    /// </summary>
    Task<ScriptExecutionResult> ExecuteAsync(ConfigModel config, BotData botData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a single script instruction
    /// </summary>
    Task<CommandExecutionResult> ExecuteInstructionAsync(ScriptInstruction instruction, BotData botData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a script for syntax errors
    /// </summary>
    ScriptValidationResult ValidateScript(ConfigModel config);

    /// <summary>
    /// Gets script execution statistics
    /// </summary>
    ScriptExecutionStatistics GetExecutionStatistics();

    /// <summary>
    /// Resets execution statistics
    /// </summary>
    void ResetStatistics();
}

/// <summary>
/// Result of script execution
/// </summary>
public class ScriptExecutionResult
{
    public bool Success { get; set; }
    public BotStatus Status { get; set; } = BotStatus.None;
    public string? CustomStatus { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public int CommandsExecuted { get; set; }
    public List<CommandExecutionResult> CommandResults { get; set; } = new();
    public Dictionary<string, object> Variables { get; set; } = new();
    public Dictionary<string, object> CapturedData { get; set; } = new();
    public List<string> Logs { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Result of individual command execution
/// </summary>
public class CommandExecutionResult
{
    public bool Success { get; set; }
    public string CommandName { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public FlowControl FlowControl { get; set; } = FlowControl.Continue;
    public string? JumpLabel { get; set; }
    public Dictionary<string, object> OutputData { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Script validation result
/// </summary>
public class ScriptValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
    public int TotalCommands { get; set; }
    public Dictionary<string, int> CommandCounts { get; set; } = new();
    public List<string> UsedVariables { get; set; } = new();
    public List<string> DefinedLabels { get; set; } = new();
    public List<string> ReferencedLabels { get; set; } = new();
}

/// <summary>
/// Validation error details
/// </summary>
public class ValidationError
{
    public int LineNumber { get; set; }
    public string CommandName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
    public string? Suggestion { get; set; }
}

/// <summary>
/// Validation warning details
/// </summary>
public class ValidationWarning
{
    public int LineNumber { get; set; }
    public string CommandName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Suggestion { get; set; }
}

/// <summary>
/// Validation severity levels
/// </summary>
public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Script execution statistics tracking
/// </summary>
public class ScriptExecutionStatistics
{
    public int TotalScriptsExecuted { get; set; }
    public int TotalCommandsExecuted { get; set; }
    public int SuccessfulExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public TimeSpan AverageExecutionTime { get; set; }
    public Dictionary<string, int> CommandExecutionCounts { get; set; } = new();
    public Dictionary<string, TimeSpan> CommandExecutionTimes { get; set; } = new();
    public Dictionary<BotStatus, int> StatusCounts { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime? LastExecutionTime { get; set; }
    public Dictionary<string, object> CustomMetrics { get; set; } = new();

    /// <summary>
    /// Gets success rate as percentage
    /// </summary>
    public double SuccessRate => TotalScriptsExecuted > 0 ? (double)SuccessfulExecutions / TotalScriptsExecuted * 100 : 0;

    /// <summary>
    /// Gets commands per minute
    /// </summary>
    public double CommandsPerMinute => TotalExecutionTime.TotalMinutes > 0 ? TotalCommandsExecuted / TotalExecutionTime.TotalMinutes : 0;

    /// <summary>
    /// Updates statistics with a new execution result
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
        if (!StatusCounts.ContainsKey(result.Status))
        {
            StatusCounts[result.Status] = 0;
        }
        StatusCounts[result.Status]++;

        // Update command statistics
        foreach (var commandResult in result.CommandResults)
        {
            var commandName = commandResult.CommandName;
            
            if (!CommandExecutionCounts.ContainsKey(commandName))
            {
                CommandExecutionCounts[commandName] = 0;
                CommandExecutionTimes[commandName] = TimeSpan.Zero;
            }
            
            CommandExecutionCounts[commandName]++;
            CommandExecutionTimes[commandName] = CommandExecutionTimes[commandName].Add(commandResult.ExecutionTime);
        }

        // Update averages
        AverageExecutionTime = TotalScriptsExecuted > 0 ? 
            TimeSpan.FromTicks(TotalExecutionTime.Ticks / TotalScriptsExecuted) : 
            TimeSpan.Zero;

        LastExecutionTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets all statistics
    /// </summary>
    public void Reset()
    {
        TotalScriptsExecuted = 0;
        TotalCommandsExecuted = 0;
        SuccessfulExecutions = 0;
        FailedExecutions = 0;
        TotalExecutionTime = TimeSpan.Zero;
        AverageExecutionTime = TimeSpan.Zero;
        CommandExecutionCounts.Clear();
        CommandExecutionTimes.Clear();
        StatusCounts.Clear();
        CustomMetrics.Clear();
        StartTime = DateTime.UtcNow;
        LastExecutionTime = null;
    }
}

/// <summary>
/// Script execution context
/// </summary>
public class ScriptExecutionContext
{
    public ConfigModel Config { get; set; } = new();
    public BotData BotData { get; set; } = new("", new(), Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance, CancellationToken.None);
    public List<ScriptInstruction> Instructions { get; set; } = new();
    public int CurrentInstructionIndex { get; set; }
    public Dictionary<string, int> Labels { get; set; } = new();
    public Stack<int> CallStack { get; set; } = new();
    public bool ShouldStop { get; set; }
    public FlowControl LastFlowControl { get; set; } = FlowControl.Continue;
    public CancellationToken CancellationToken { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public List<CommandExecutionResult> CommandResults { get; set; } = new();

    /// <summary>
    /// Gets the current instruction
    /// </summary>
    public ScriptInstruction? GetCurrentInstruction()
    {
        return CurrentInstructionIndex >= 0 && CurrentInstructionIndex < Instructions.Count 
            ? Instructions[CurrentInstructionIndex] 
            : null;
    }

    /// <summary>
    /// Jumps to a specific label
    /// </summary>
    public bool JumpToLabel(string label)
    {
        if (Labels.TryGetValue(label, out var index))
        {
            CurrentInstructionIndex = index;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Moves to the next instruction
    /// </summary>
    public void MoveNext()
    {
        CurrentInstructionIndex++;
    }

    /// <summary>
    /// Checks if execution should continue
    /// </summary>
    public bool ShouldContinue()
    {
        return !ShouldStop && 
               !CancellationToken.IsCancellationRequested && 
               CurrentInstructionIndex < Instructions.Count;
    }
}
