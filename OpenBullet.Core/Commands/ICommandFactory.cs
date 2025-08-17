using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;

namespace OpenBullet.Core.Commands;

/// <summary>
/// Factory for creating script command instances
/// </summary>
public interface ICommandFactory
{
    /// <summary>
    /// Creates a command instance by name
    /// </summary>
    IScriptCommand? CreateCommand(string commandName);

    /// <summary>
    /// Gets all available command names
    /// </summary>
    IEnumerable<string> GetAvailableCommands();

    /// <summary>
    /// Registers a command type
    /// </summary>
    void RegisterCommand<T>(string commandName) where T : class, IScriptCommand;

    /// <summary>
    /// Registers a command instance
    /// </summary>
    void RegisterCommand(string commandName, IScriptCommand command);

    /// <summary>
    /// Checks if a command is registered
    /// </summary>
    bool IsCommandRegistered(string commandName);

    /// <summary>
    /// Gets command metadata
    /// </summary>
    CommandMetadata? GetCommandMetadata(string commandName);
}

/// <summary>
/// Command metadata for documentation and validation
/// </summary>
public class CommandMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<CommandParameter> Parameters { get; set; } = new();
    public List<string> Examples { get; set; } = new();
    public string Syntax { get; set; } = string.Empty;
    public bool RequiresVariableSubstitution { get; set; } = true;
    public bool CanHaveSubcommands { get; set; } = false;
}

/// <summary>
/// Command parameter metadata
/// </summary>
public class CommandParameter
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ParameterType Type { get; set; } = ParameterType.String;
    public bool Required { get; set; } = true;
    public object? DefaultValue { get; set; }
    public List<string> AllowedValues { get; set; } = new();
    public string? Pattern { get; set; }
}

/// <summary>
/// Parameter type enumeration
/// </summary>
public enum ParameterType
{
    String,
    Integer,
    Boolean,
    Url,
    HttpMethod,
    ContentType,
    FilePath,
    Variable,
    Enum
}
