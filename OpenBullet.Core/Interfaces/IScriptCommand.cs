using OpenBullet.Core.Models;

namespace OpenBullet.Core.Interfaces;

/// <summary>
/// Interface for script commands
/// </summary>
public interface IScriptCommand
{
    /// <summary>
    /// Command name (e.g., "REQUEST", "PARSE", "KEYCHECK")
    /// </summary>
    string CommandName { get; }

    /// <summary>
    /// Command description
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Execute the command
    /// </summary>
    Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData);

    /// <summary>
    /// Validate command syntax
    /// </summary>
    CommandValidationResult ValidateInstruction(ScriptInstruction instruction);
}

/// <summary>
/// Represents a parsed script instruction
/// </summary>
public class ScriptInstruction
{
    public string CommandName { get; set; } = string.Empty;
    public List<string> Arguments { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
    public int LineNumber { get; set; }
    public string RawLine { get; set; } = string.Empty;
    public string? Label { get; set; }
    public List<ScriptInstruction> SubInstructions { get; set; } = new();

    public string GetArgument(int index, string defaultValue = "")
    {
        return index < Arguments.Count ? Arguments[index] : defaultValue;
    }

    public T GetParameter<T>(string key, T defaultValue = default!)
    {
        if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }
}

/// <summary>
/// Result of command execution
/// </summary>
public class CommandResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public FlowControl FlowControl { get; set; } = FlowControl.Continue;
    public Dictionary<string, object> Variables { get; set; } = new();
    public Dictionary<string, object> CapturedData { get; set; } = new();
    public BotStatus? NewStatus { get; set; }
    public string? CustomStatus { get; set; }
}

/// <summary>
/// Flow control enumeration
/// </summary>
public enum FlowControl
{
    Continue,
    Stop,
    Jump,
    Break,
    Return,
    Retry
}

/// <summary>
/// Command validation result
/// </summary>
public class CommandValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
