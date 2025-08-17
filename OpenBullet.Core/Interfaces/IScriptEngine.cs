using OpenBullet.Core.Models;

namespace OpenBullet.Core.Interfaces;

/// <summary>
/// Interface for script execution engine
/// </summary>
public interface IScriptEngine
{
    /// <summary>
    /// Executes a script with the provided bot data
    /// </summary>
    Task<ScriptResult> ExecuteAsync(string script, BotData botData);

    /// <summary>
    /// Validates script syntax
    /// </summary>
    ScriptValidationResult ValidateScript(string script);

    /// <summary>
    /// Registers a custom command
    /// </summary>
    void RegisterCommand(string commandName, IScriptCommand command);

    /// <summary>
    /// Gets all available commands
    /// </summary>
    IReadOnlyDictionary<string, IScriptCommand> GetCommands();
}

/// <summary>
/// Result of script execution
/// </summary>
public class ScriptResult
{
    public bool Success { get; set; }
    public BotStatus Status { get; set; } = BotStatus.None;
    public string CustomStatus { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public List<string> Log { get; set; } = new();
    public Dictionary<string, object> Variables { get; set; } = new();
    public Dictionary<string, object> CapturedData { get; set; } = new();
}

/// <summary>
/// Script validation result
/// </summary>
public class ScriptValidationResult
{
    public bool IsValid { get; set; }
    public List<ScriptError> Errors { get; set; } = new();
    public List<ScriptWarning> Warnings { get; set; } = new();
}

/// <summary>
/// Script error information
/// </summary>
public class ScriptError
{
    public int LineNumber { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
}

/// <summary>
/// Script warning information
/// </summary>
public class ScriptWarning
{
    public int LineNumber { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
}
