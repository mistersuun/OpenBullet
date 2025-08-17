using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;

namespace OpenBullet.Core.Parsing;

/// <summary>
/// Interface for LoliScript parsing operations
/// </summary>
public interface IScriptParser
{
    /// <summary>
    /// Parses a complete script into instructions
    /// </summary>
    ScriptParseResult ParseScript(string script);

    /// <summary>
    /// Parses a single line into an instruction
    /// </summary>
    ScriptInstruction? ParseLine(string line, int lineNumber);

    /// <summary>
    /// Validates script syntax without full parsing
    /// </summary>
    ScriptValidationResult ValidateScript(string script);

    /// <summary>
    /// Extracts variable references from a string
    /// </summary>
    List<VariableReference> ExtractVariableReferences(string input);

    /// <summary>
    /// Substitutes variable references in a string
    /// </summary>
    string SubstituteVariables(string input, Dictionary<string, object> variables);

    /// <summary>
    /// Gets available commands for auto-completion
    /// </summary>
    List<CommandInfo> GetAvailableCommands();
}

/// <summary>
/// Result of script parsing operation
/// </summary>
public class ScriptParseResult
{
    public bool Success { get; set; }
    public List<ScriptInstruction> Instructions { get; set; } = new();
    public List<ScriptError> Errors { get; set; } = new();
    public List<ScriptWarning> Warnings { get; set; } = new();
    public Dictionary<string, int> Labels { get; set; } = new();
    public List<VariableReference> VariableReferences { get; set; } = new();
    public ParseStatistics Statistics { get; set; } = new();
}

/// <summary>
/// Variable reference in script
/// </summary>
public class VariableReference
{
    public string VariableName { get; set; } = string.Empty;
    public VariableType Type { get; set; } = VariableType.Single;
    public string? IndexOrKey { get; set; }
    public int Position { get; set; }
    public int Length { get; set; }
    public string OriginalText { get; set; } = string.Empty;
}

/// <summary>
/// Variable reference types
/// </summary>
public enum VariableType
{
    Single,      // <VAR>
    List,        // <VAR[0]> or <VAR[*]>
    Dictionary   // <VAR(key)> or <VAR{value}>
}

/// <summary>
/// Script parsing statistics
/// </summary>
public class ParseStatistics
{
    public int TotalLines { get; set; }
    public int CodeLines { get; set; }
    public int CommentLines { get; set; }
    public int EmptyLines { get; set; }
    public int LabelCount { get; set; }
    public int InstructionCount { get; set; }
    public Dictionary<string, int> CommandCounts { get; set; } = new();
    public TimeSpan ParseTime { get; set; }
}

/// <summary>
/// Command information for auto-completion
/// </summary>
public class CommandInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Syntax { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
    public List<string> Examples { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Parameter information for commands
/// </summary>
public class ParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ParameterType Type { get; set; }
    public bool Required { get; set; }
    public string? DefaultValue { get; set; }
    public List<string> AllowedValues { get; set; } = new();
}

/// <summary>
/// Parameter types for script commands
/// </summary>
public enum ParameterType
{
    String,
    Integer,
    Boolean,
    Enum,
    Variable,
    Redirector
}

/// <summary>
/// Instruction parsing context
/// </summary>
public class InstructionContext
{
    public int LineNumber { get; set; }
    public string OriginalLine { get; set; } = string.Empty;
    public LineType LineType { get; set; }
    public List<string> IndentedLines { get; set; } = new();
    public ScriptInstruction? ParentInstruction { get; set; }
    public int IndentLevel { get; set; }
    public Dictionary<string, int> Labels { get; set; } = new();
}

/// <summary>
/// Script line types
/// </summary>
public enum LineType
{
    Empty,
    Comment,
    Label,
    Command,
    Continuation
}
