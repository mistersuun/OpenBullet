using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;

namespace OpenBullet.Core.Parsing;

/// <summary>
/// LoliScript parser implementation
/// </summary>
public class ScriptParser : IScriptParser
{
    private readonly ILogger<ScriptParser> _logger;
    private readonly Regex _variableRegex;
    private readonly Regex _commentRegex;
    private readonly Regex _labelRegex;
    private readonly Regex _booleanParamRegex;
    private readonly Regex _redirectorRegex;
    private readonly Dictionary<string, CommandInfo> _commands;

    public ScriptParser(ILogger<ScriptParser> logger)
    {
        _logger = logger;
        
        // Initialize regex patterns
        _variableRegex = new Regex(@"<([A-Za-z_][A-Za-z0-9_]*(?:\[[^\]]*\]|\([^)]*\)|\{[^}]*\})?)>", RegexOptions.Compiled);
        _commentRegex = new Regex(@"^\s*##.*$", RegexOptions.Compiled);
        _labelRegex = new Regex(@"^\s*#([A-Za-z_][A-Za-z0-9_]*)\s+(.+)$", RegexOptions.Compiled);
        _booleanParamRegex = new Regex(@"([A-Za-z_][A-Za-z0-9_]*)=(True|False)", RegexOptions.Compiled);
        _redirectorRegex = new Regex(@"->\s*(VAR|CAP)\s+""([^""]+)""(?:\s+""([^""]*)""\s+""([^""]*)"")?", RegexOptions.Compiled);
        
        // Initialize command definitions
        _commands = InitializeCommands();
    }

    public ScriptParseResult ParseScript(string script)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ScriptParseResult { Success = true };
        
        try
        {
            _logger.LogDebug("Starting script parsing");
            
            var lines = script.Split('\n', StringSplitOptions.None);
            var context = new InstructionContext();
            
            // First pass: identify structure and collect labels
            var processedLines = PreprocessLines(lines, result);
            
            // Second pass: parse instructions
            ParseInstructions(processedLines, result, context);
            
            // Calculate statistics
            CalculateStatistics(lines, result);
            
            stopwatch.Stop();
            result.Statistics.ParseTime = stopwatch.Elapsed;
            
            _logger.LogDebug("Script parsing completed in {Time}ms with {Instructions} instructions and {Errors} errors", 
                stopwatch.ElapsedMilliseconds, result.Instructions.Count, result.Errors.Count);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new ScriptError
            {
                LineNumber = 0,
                Message = $"Unexpected parsing error: {ex.Message}",
                Line = string.Empty
            });
            _logger.LogError(ex, "Script parsing failed");
        }

        return result;
    }

    public ScriptInstruction? ParseLine(string line, int lineNumber)
    {
        try
        {
            var trimmedLine = line.Trim();
            var lineType = GetLineType(trimmedLine);

            switch (lineType)
            {
                case LineType.Empty:
                case LineType.Comment:
                    return null;

                case LineType.Label:
                    return ParseLabelLine(trimmedLine, lineNumber);

                case LineType.Command:
                    return ParseCommandLine(trimmedLine, lineNumber);

                default:
                    return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse line {LineNumber}: {Line}", lineNumber, line);
            return null;
        }
    }

    public ScriptValidationResult ValidateScript(string script)
    {
        var result = new ScriptValidationResult { IsValid = true };
        
        try
        {
            var parseResult = ParseScript(script);
            result.IsValid = parseResult.Success;
            result.Errors = parseResult.Errors;
            result.Warnings = parseResult.Warnings;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add(new ScriptError
            {
                Message = $"Validation error: {ex.Message}",
                LineNumber = 0,
                Line = string.Empty
            });
        }

        return result;
    }

    public List<VariableReference> ExtractVariableReferences(string input)
    {
        var references = new List<VariableReference>();
        var matches = _variableRegex.Matches(input);

        foreach (Match match in matches)
        {
            var reference = ParseVariableReference(match);
            references.Add(reference);
        }

        return references;
    }

    public string SubstituteVariables(string input, Dictionary<string, object> variables)
    {
        return _variableRegex.Replace(input, match =>
        {
            var varRef = ParseVariableReference(match);
            
            if (variables.TryGetValue(varRef.VariableName, out var value))
            {
                return FormatVariableValue(value, varRef);
            }
            
            // Return original if variable not found
            return match.Value;
        });
    }

    public List<CommandInfo> GetAvailableCommands()
    {
        return _commands.Values.ToList();
    }

    private List<ProcessedLine> PreprocessLines(string[] lines, ScriptParseResult result)
    {
        var processedLines = new List<ProcessedLine>();
        var currentInstruction = new List<string>();
        var currentLineNumber = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmedLine = line.Trim();
            var lineType = GetLineType(trimmedLine);

            if (lineType == LineType.Continuation && currentInstruction.Count > 0)
            {
                // Continuation of previous instruction
                currentInstruction.Add(line);
            }
            else
            {
                // Finish current instruction if exists
                if (currentInstruction.Count > 0)
                {
                    processedLines.Add(new ProcessedLine
                    {
                        LineNumber = currentLineNumber,
                        Type = GetLineType(currentInstruction[0].Trim()),
                        Content = string.Join("\n", currentInstruction)
                    });
                }

                // Start new instruction
                currentInstruction.Clear();
                currentInstruction.Add(line);
                currentLineNumber = i + 1;

                // Handle labels
                if (lineType == LineType.Label)
                {
                    var labelMatch = _labelRegex.Match(trimmedLine);
                    if (labelMatch.Success)
                    {
                        var labelName = labelMatch.Groups[1].Value;
                        result.Labels[labelName] = i + 1;
                    }
                }
            }
        }

        // Add final instruction
        if (currentInstruction.Count > 0)
        {
            processedLines.Add(new ProcessedLine
            {
                LineNumber = currentLineNumber,
                Type = GetLineType(currentInstruction[0].Trim()),
                Content = string.Join("\n", currentInstruction)
            });
        }

        return processedLines;
    }

    private void ParseInstructions(List<ProcessedLine> lines, ScriptParseResult result, InstructionContext context)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            
            try
            {
                var instruction = ParseProcessedLine(line, context);
                if (instruction != null)
                {
                    // Handle sub-instructions for complex commands
                    if (IsComplexCommand(instruction.CommandName))
                    {
                        ParseSubInstructions(lines, ref i, instruction, result, context);
                    }
                    
                    result.Instructions.Add(instruction);
                    
                    // Extract variable references
                    var varRefs = ExtractVariableReferences(instruction.RawLine);
                    result.VariableReferences.AddRange(varRefs);
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ScriptError
                {
                    LineNumber = line.LineNumber,
                    Message = ex.Message,
                    Line = line.Content
                });
                result.Success = false;
            }
        }
    }

    private ScriptInstruction? ParseProcessedLine(ProcessedLine line, InstructionContext context)
    {
        var trimmedContent = line.Content.Trim();
        var lineType = line.Type;

        switch (lineType)
        {
            case LineType.Empty:
            case LineType.Comment:
                return null;

            case LineType.Label:
                return ParseLabelLine(trimmedContent, line.LineNumber);

            case LineType.Command:
                return ParseCommandLine(trimmedContent, line.LineNumber);

            default:
                return null;
        }
    }

    private ScriptInstruction ParseLabelLine(string line, int lineNumber)
    {
        var match = _labelRegex.Match(line);
        if (!match.Success)
        {
            throw new ArgumentException($"Invalid label syntax: {line}");
        }

        var labelName = match.Groups[1].Value;
        var commandPart = match.Groups[2].Value;

        var instruction = ParseCommandLine(commandPart, lineNumber);
        instruction.Label = labelName;
        instruction.RawLine = line; // Preserve the original line with label
        
        return instruction;
    }

    private ScriptInstruction ParseCommandLine(string line, int lineNumber)
    {
        var instruction = new ScriptInstruction
        {
            LineNumber = lineNumber,
            RawLine = line
        };

        // Parse redirector first (and remove from line)
        var redirectorMatch = _redirectorRegex.Match(line);
        if (redirectorMatch.Success)
        {
            instruction.Parameters["RedirectorType"] = redirectorMatch.Groups[1].Value; // VAR or CAP
            instruction.Parameters["RedirectorName"] = redirectorMatch.Groups[2].Value;
            
            if (redirectorMatch.Groups[3].Success)
                instruction.Parameters["RedirectorPrefix"] = redirectorMatch.Groups[3].Value;
                
            if (redirectorMatch.Groups[4].Success)
                instruction.Parameters["RedirectorSuffix"] = redirectorMatch.Groups[4].Value;
            
            // Remove redirector from line for further parsing
            line = line.Substring(0, redirectorMatch.Index).Trim();
        }

        // Parse boolean parameters (and remove from line)
        var booleanMatches = _booleanParamRegex.Matches(line);
        foreach (Match match in booleanMatches)
        {
            var paramName = match.Groups[1].Value;
            var paramValue = bool.Parse(match.Groups[2].Value);
            instruction.Parameters[paramName] = paramValue;
        }

        // Remove boolean parameters from line
        line = _booleanParamRegex.Replace(line, "").Trim();

        // Split into command and arguments
        var parts = SplitCommandLine(line);
        if (parts.Count == 0)
        {
            throw new ArgumentException($"Empty command line: {instruction.RawLine}");
        }

        instruction.CommandName = parts[0].ToUpper();
        instruction.Arguments = parts.Skip(1).ToList();

        return instruction;
    }

    private List<string> SplitCommandLine(string line)
    {
        var parts = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;
        var escapeNext = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (escapeNext)
            {
                current.Append(c);
                escapeNext = false;
                continue;
            }

            if (c == '\\')
            {
                escapeNext = true;
                continue;
            }

            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (!inQuotes && char.IsWhiteSpace(c))
            {
                if (current.Length > 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
        {
            parts.Add(current.ToString());
        }

        return parts;
    }

    private void ParseSubInstructions(List<ProcessedLine> lines, ref int currentIndex, ScriptInstruction parentInstruction, ScriptParseResult result, InstructionContext context)
    {
        // Look ahead for indented sub-instructions
        for (int i = currentIndex + 1; i < lines.Count; i++)
        {
            var line = lines[i];
            var trimmedContent = line.Content.TrimStart();
            
            // Check if line is indented (sub-instruction)
            if (line.Content.Length > trimmedContent.Length && !string.IsNullOrWhiteSpace(trimmedContent))
            {
                try
                {
                    var subInstruction = ParseCommandLine(trimmedContent, line.LineNumber);
                    parentInstruction.SubInstructions.Add(subInstruction);
                    currentIndex = i; // Update current index
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ScriptError
                    {
                        LineNumber = line.LineNumber,
                        Message = $"Error parsing sub-instruction: {ex.Message}",
                        Line = line.Content
                    });
                }
            }
            else
            {
                break; // No more sub-instructions
            }
        }
    }

    private bool IsComplexCommand(string commandName)
    {
        return commandName switch
        {
            "KEYCHECK" => true,
            "MOUSEACTION" => true,
            "IF" => true,
            "WHILE" => true,
            _ => false
        };
    }

    private LineType GetLineType(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return LineType.Empty;

        if (_commentRegex.IsMatch(line))
            return LineType.Comment;

        if (_labelRegex.IsMatch(line))
            return LineType.Label;

        if (char.IsWhiteSpace(line[0]))
            return LineType.Continuation;

        return LineType.Command;
    }

    private VariableReference ParseVariableReference(Match match)
    {
        var fullText = match.Groups[1].Value;
        var reference = new VariableReference
        {
            Position = match.Index,
            Length = match.Length,
            OriginalText = match.Value
        };

        // Parse variable type and name
        if (fullText.Contains('['))
        {
            // List variable: VAR[index] or VAR[*]
            var bracketIndex = fullText.IndexOf('[');
            reference.VariableName = fullText.Substring(0, bracketIndex);
            reference.Type = VariableType.List;
            
            var endBracket = fullText.IndexOf(']', bracketIndex);
            if (endBracket > bracketIndex)
            {
                reference.IndexOrKey = fullText.Substring(bracketIndex + 1, endBracket - bracketIndex - 1);
            }
        }
        else if (fullText.Contains('('))
        {
            // Dictionary variable by key: VAR(key)
            var parenIndex = fullText.IndexOf('(');
            reference.VariableName = fullText.Substring(0, parenIndex);
            reference.Type = VariableType.Dictionary;
            
            var endParen = fullText.IndexOf(')', parenIndex);
            if (endParen > parenIndex)
            {
                reference.IndexOrKey = fullText.Substring(parenIndex + 1, endParen - parenIndex - 1);
            }
        }
        else if (fullText.Contains('{'))
        {
            // Dictionary variable by value: VAR{value}
            var braceIndex = fullText.IndexOf('{');
            reference.VariableName = fullText.Substring(0, braceIndex);
            reference.Type = VariableType.Dictionary;
            
            var endBrace = fullText.IndexOf('}', braceIndex);
            if (endBrace > braceIndex)
            {
                reference.IndexOrKey = fullText.Substring(braceIndex + 1, endBrace - braceIndex - 1);
            }
        }
        else
        {
            // Simple variable: VAR
            reference.VariableName = fullText;
            reference.Type = VariableType.Single;
        }

        return reference;
    }

    private string FormatVariableValue(object value, VariableReference varRef)
    {
        switch (varRef.Type)
        {
            case VariableType.Single:
                return value?.ToString() ?? string.Empty;

            case VariableType.List:
                if (value is IList<object> list)
                {
                    if (varRef.IndexOrKey == "*")
                    {
                        return string.Join(", ", list);
                    }
                    else if (int.TryParse(varRef.IndexOrKey, out var index) && index >= 0 && index < list.Count)
                    {
                        return list[index]?.ToString() ?? string.Empty;
                    }
                }
                break;

            case VariableType.Dictionary:
                if (value is IDictionary<string, object> dict)
                {
                    if (varRef.IndexOrKey == "*")
                    {
                        return string.Join(", ", dict.Values);
                    }
                    else if (varRef.IndexOrKey != null && dict.TryGetValue(varRef.IndexOrKey, out var dictValue))
                    {
                        return dictValue?.ToString() ?? string.Empty;
                    }
                }
                break;
        }

        return varRef.OriginalText; // Return original if can't resolve
    }

    private void CalculateStatistics(string[] lines, ScriptParseResult result)
    {
        result.Statistics.TotalLines = lines.Length;
        
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                result.Statistics.EmptyLines++;
            }
            else if (_commentRegex.IsMatch(trimmed))
            {
                result.Statistics.CommentLines++;
            }
            else
            {
                result.Statistics.CodeLines++;
            }
        }

        result.Statistics.LabelCount = result.Labels.Count;
        result.Statistics.InstructionCount = result.Instructions.Count;

        // Count commands
        foreach (var instruction in result.Instructions)
        {
            var commandName = instruction.CommandName;
            if (result.Statistics.CommandCounts.ContainsKey(commandName))
            {
                result.Statistics.CommandCounts[commandName]++;
            }
            else
            {
                result.Statistics.CommandCounts[commandName] = 1;
            }
        }
    }

    private Dictionary<string, CommandInfo> InitializeCommands()
    {
        var commands = new Dictionary<string, CommandInfo>();

        // Basic commands
        commands["PRINT"] = new CommandInfo
        {
            Name = "PRINT",
            Description = "Outputs text to the debugger log",
            Syntax = "PRINT TEXT",
            Category = "General",
            Examples = { "PRINT Hello, World!", "PRINT Variable value: <VAR>" }
        };

        commands["REQUEST"] = new CommandInfo
        {
            Name = "REQUEST",
            Description = "Sends an HTTP request",
            Syntax = "REQUEST METHOD \"URL\" [OPTIONS]",
            Category = "HTTP",
            Parameters =
            {
                new ParameterInfo { Name = "METHOD", Type = ParameterType.Enum, Required = true, AllowedValues = { "GET", "POST", "PUT", "DELETE" } },
                new ParameterInfo { Name = "URL", Type = ParameterType.String, Required = true },
                new ParameterInfo { Name = "AutoRedirect", Type = ParameterType.Boolean, Required = false, DefaultValue = "True" }
            },
            Examples = { "REQUEST GET \"https://example.com\"", "REQUEST POST \"https://api.example.com/login\" AutoRedirect=False" }
        };

        commands["PARSE"] = new CommandInfo
        {
            Name = "PARSE",
            Description = "Extracts data from the response",
            Syntax = "PARSE \"TARGET\" TYPE \"PATTERN\" [OPTIONS] -> VAR/CAP \"NAME\"",
            Category = "Data",
            Parameters =
            {
                new ParameterInfo { Name = "TARGET", Type = ParameterType.String, Required = true },
                new ParameterInfo { Name = "TYPE", Type = ParameterType.Enum, Required = true, AllowedValues = { "LR", "CSS", "JSON", "REGEX" } },
                new ParameterInfo { Name = "PATTERN", Type = ParameterType.String, Required = true },
                new ParameterInfo { Name = "Recursive", Type = ParameterType.Boolean, Required = false, DefaultValue = "False" }
            },
            Examples = { "PARSE \"<SOURCE>\" LR \"<title>\" \"</title>\" -> VAR \"TITLE\"", "PARSE \"<SOURCE>\" CSS \"input[name='token']\" \"value\" -> VAR \"TOKEN\"" }
        };

        commands["KEYCHECK"] = new CommandInfo
        {
            Name = "KEYCHECK",
            Description = "Checks for specific keys in the response to determine success/failure",
            Syntax = "KEYCHECK [OPTIONS]\\n  KEYCHAIN TYPE MODE\\n    KEY \"STRING\" [CONDITION]",
            Category = "Logic",
            Parameters =
            {
                new ParameterInfo { Name = "BanOn4XX", Type = ParameterType.Boolean, Required = false, DefaultValue = "True" },
                new ParameterInfo { Name = "BanOnToCheck", Type = ParameterType.Boolean, Required = false, DefaultValue = "False" }
            },
            Examples = { "KEYCHECK\\n  KEYCHAIN Success OR\\n    KEY \"Welcome\"\\n  KEYCHAIN Failure OR\\n    KEY \"Invalid\"" }
        };

        commands["FUNCTION"] = new CommandInfo
        {
            Name = "FUNCTION",
            Description = "Executes a utility function",
            Syntax = "FUNCTION Name [ARGUMENTS] [\"INPUT\"] [-> VAR/CAP \"NAME\"]",
            Category = "Utility",
            Examples = { "FUNCTION Constant \"Hello\" -> VAR \"GREETING\"", "FUNCTION Hash SHA256 \"<PASSWORD>\" -> VAR \"HASHED\"" }
        };

        return commands;
    }

    private class ProcessedLine
    {
        public int LineNumber { get; set; }
        public LineType Type { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
