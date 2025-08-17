using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using OpenBullet.Core.Execution;

namespace OpenBullet.Core.Services;

/// <summary>
/// Professional LoliScript validator with comprehensive syntax and logical error detection
/// Provides real-time validation, error reporting, and code analysis
/// </summary>
public class LoliScriptValidator
{
    private readonly ILogger<LoliScriptValidator>? _logger;
    private readonly HashSet<string> _validCommands;
    private readonly Dictionary<string, CommandValidation> _commandValidations;
    private readonly Dictionary<string, Regex> _validationPatterns;

    public LoliScriptValidator(ILogger<LoliScriptValidator>? logger = null)
    {
        _logger = logger;
        _validCommands = new HashSet<string>();
        _commandValidations = new Dictionary<string, CommandValidation>();
        _validationPatterns = new Dictionary<string, Regex>();
        
        InitializeValidCommands();
        InitializeCommandValidations();
        InitializeValidationPatterns();
        
        _logger?.LogInformation("LoliScriptValidator initialized with {CommandCount} command validations", _commandValidations.Count);
    }

    /// <summary>
    /// Validate complete LoliScript and return comprehensive results
    /// </summary>
    public ValidationResult ValidateScript(string script)
    {
        try
        {
            if (string.IsNullOrEmpty(script))
            {
                return new ValidationResult { IsValid = true };
            }

            var result = new ValidationResult();
            var lines = script.Split('\n');
            var variableDefinitions = new HashSet<string>();
            var labels = new HashSet<string>();
            var flowControlStack = new Stack<string>();

            // First pass: collect variable definitions and labels
            CollectDefinitions(lines, variableDefinitions, labels);

            // Second pass: validate each line
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var lineNumber = i + 1;

                if (string.IsNullOrWhiteSpace(line) || IsComment(line))
                    continue;

                // Validate line syntax
                ValidateLine(line, lineNumber, result, variableDefinitions, labels, flowControlStack);
            }

            // Check for unclosed flow control blocks
            ValidateFlowControlBlocks(flowControlStack, result);

            // Analyze for logical issues
            AnalyzeLogicalIssues(lines, result, variableDefinitions);

            result.IsValid = result.Errors.Count == 0;
            
            _logger?.LogDebug("Validated script with {ErrorCount} errors and {WarningCount} warnings", 
                result.Errors.Count, result.Warnings.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error validating script");
            return new ValidationResult
            {
                IsValid = false,
                Errors = { new ValidationError { 
                    Message = $"Validation error: {ex.Message}", 
                    LineNumber = 0, 
                    CommandName = "UNKNOWN",
                    Severity = ValidationSeverity.Error 
                } }
            };
        }
    }

    /// <summary>
    /// Validate specific line for real-time feedback
    /// </summary>
    public LineValidationResult ValidateLine(string line, int lineNumber)
    {
        try
        {
            var result = new LineValidationResult { LineNumber = lineNumber };
            
            if (string.IsNullOrWhiteSpace(line) || IsComment(line))
            {
                result.IsValid = true;
                return result;
            }

            // Basic syntax validation
            var tokens = TokenizeLine(line);
            if (tokens.Count == 0)
            {
                result.Errors.Add(new ValidationError 
                { 
                    Message = "Invalid line syntax", 
                    LineNumber = lineNumber,
                    CommandName = "UNKNOWN",
                    Severity = ValidationSeverity.Error
                });
                return result;
            }

            var command = tokens[0].ToUpper();
            
            // Check if command exists
            if (!_validCommands.Contains(command))
            {
                result.Errors.Add(new ValidationError 
                { 
                    Message = $"Unknown command: {command}", 
                    LineNumber = lineNumber,
                    CommandName = command,
                    Severity = ValidationSeverity.Error,
                    Suggestion = "Check command spelling or refer to documentation"
                });
                return result;
            }

            // Validate command-specific syntax
            if (_commandValidations.TryGetValue(command, out var validation))
            {
                ValidateCommandSyntax(tokens, validation, result);
            }

            result.IsValid = result.Errors.Count == 0;
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error validating line {LineNumber}", lineNumber);
            return new LineValidationResult
            {
                LineNumber = lineNumber,
                IsValid = false,
                Errors = { new ValidationError { 
                    Message = $"Line validation error: {ex.Message}", 
                    LineNumber = lineNumber,
                    CommandName = "UNKNOWN",
                    Severity = ValidationSeverity.Error
                } }
            };
        }
    }

    #region Private Implementation

    private void InitializeValidCommands()
    {
        _validCommands.UnionWith(new[]
        {
            "REQUEST", "PARSE", "KEYCHECK", "SET", "LOG", "DELAY",
            "IF", "ELSE", "ENDIF", "WHILE", "ENDWHILE", "FOR", "ENDFOR",
            "FUNCTION", "ENDFUNCTION", "RETURN", "BREAK", "CONTINUE",
            "TRY", "CATCH", "ENDTRY", "UTILITY", "JUMP", "LABEL"
        });
    }

    private void InitializeCommandValidations()
    {
        // REQUEST command validation
        _commandValidations["REQUEST"] = new CommandValidation
        {
            MinParameters = 2,
            MaxParameters = 10,
            RequiredParameters = new[] { "method", "url" },
            ValidMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" }
        };

        // PARSE command validation
        _commandValidations["PARSE"] = new CommandValidation
        {
            MinParameters = 2,
            MaxParameters = 6,
            RequiredParameters = new[] { "pattern", "type" },
            ValidTypes = new[] { "REGEX", "LR", "CSS", "JSON", "XPATH" }
        };

        // KEYCHECK command validation
        _commandValidations["KEYCHECK"] = new CommandValidation
        {
            MinParameters = 2,
            MaxParameters = 4,
            RequiredParameters = new[] { "condition", "value" },
            ValidConditions = new[] { "SUCCESS", "FAILURE", "BAN", "RETRY", "CUSTOM" }
        };

        // SET command validation
        _commandValidations["SET"] = new CommandValidation
        {
            MinParameters = 2,
            MaxParameters = 3,
            RequiredParameters = new[] { "variable", "value" }
        };

        // Control flow validations
        _commandValidations["IF"] = new CommandValidation
        {
            MinParameters = 1,
            MaxParameters = 3,
            RequiredParameters = new[] { "condition" }
        };

        _commandValidations["WHILE"] = new CommandValidation
        {
            MinParameters = 1,
            MaxParameters = 3,
            RequiredParameters = new[] { "condition" }
        };
    }

    private void InitializeValidationPatterns()
    {
        // URL pattern
        _validationPatterns["URL"] = new Regex(
            @"^https?://[^\s/$.?#].[^\s]*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Variable pattern
        _validationPatterns["VARIABLE"] = new Regex(
            @"^[a-zA-Z_][a-zA-Z0-9_]*$",
            RegexOptions.Compiled);

        // String literal pattern
        _validationPatterns["STRING"] = new Regex(
            @"^""([^""\\]|\\.)*""$",
            RegexOptions.Compiled);

        // Number pattern
        _validationPatterns["NUMBER"] = new Regex(
            @"^\d+(\.\d+)?$",
            RegexOptions.Compiled);
    }

    private void CollectDefinitions(string[] lines, HashSet<string> variables, HashSet<string> labels)
    {
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || IsComment(trimmed))
                continue;

            var tokens = TokenizeLine(trimmed);
            if (tokens.Count == 0)
                continue;

            var command = tokens[0].ToUpper();

            // Collect variable definitions from SET and PARSE commands
            if (command == "SET" && tokens.Count >= 2)
            {
                variables.Add(tokens[1]);
            }
            else if (command == "PARSE" && trimmed.Contains("->"))
            {
                var arrowIndex = trimmed.IndexOf("->");
                var variablePart = trimmed.Substring(arrowIndex + 2).Trim();
                var variableName = variablePart.Split(' ')[0];
                variables.Add(variableName);
            }

            // Collect labels
            if (trimmed.Contains(':') && !trimmed.StartsWith("//") && !trimmed.StartsWith("#"))
            {
                var colonIndex = trimmed.IndexOf(':');
                if (colonIndex > 0)
                {
                    var labelName = trimmed.Substring(0, colonIndex).Trim();
                    labels.Add(labelName);
                }
            }
        }
    }

    private void ValidateLine(string line, int lineNumber, ValidationResult result, 
        HashSet<string> variables, HashSet<string> labels, Stack<string> flowControlStack)
    {
        var tokens = TokenizeLine(line);
        if (tokens.Count == 0)
            return;

        var command = tokens[0].ToUpper();

        // Validate command existence
        if (!_validCommands.Contains(command))
        {
            result.Errors.Add(new ValidationError
            {
                Message = $"Unknown command: {command}",
                LineNumber = lineNumber,
                CommandName = command,
                Severity = ValidationSeverity.Error,
                Suggestion = "Check command spelling or refer to documentation"
            });
            return;
        }

        // Validate command-specific syntax
        if (_commandValidations.TryGetValue(command, out var validation))
        {
            var lineResult = new LineValidationResult { LineNumber = lineNumber };
            ValidateCommandSyntax(tokens, validation, lineResult);
            result.Errors.AddRange(lineResult.Errors);
        }

        // Validate flow control structure
        ValidateFlowControl(command, flowControlStack, lineNumber, result);

        // Validate variable usage
        ValidateVariableUsage(line, variables, lineNumber, result);

        // Validate URLs if present
        ValidateUrls(tokens, lineNumber, result);
    }

    private void ValidateCommandSyntax(List<string> tokens, CommandValidation validation, LineValidationResult result)
    {
        var command = tokens[0].ToUpper();
        var parameterCount = tokens.Count - 1;

        // Check parameter count
        if (parameterCount < validation.MinParameters)
        {
            result.Errors.Add(new ValidationError
            {
                Message = $"{command} requires at least {validation.MinParameters} parameters, got {parameterCount}",
                LineNumber = result.LineNumber,
                CommandName = command,
                Severity = ValidationSeverity.Error,
                Suggestion = $"Add {validation.MinParameters - parameterCount} more parameter(s)"
            });
        }

        if (parameterCount > validation.MaxParameters)
        {
            result.Errors.Add(new ValidationError
            {
                Message = $"{command} accepts at most {validation.MaxParameters} parameters, got {parameterCount}",
                LineNumber = result.LineNumber,
                CommandName = command,
                Severity = ValidationSeverity.Error,
                Suggestion = $"Remove {parameterCount - validation.MaxParameters} parameter(s)"
            });
        }

        // Validate specific parameter values
        if (command == "REQUEST" && tokens.Count > 1)
        {
            var method = tokens[1].ToUpper();
            if (validation.ValidMethods != null && !validation.ValidMethods.Contains(method))
            {
                result.Errors.Add(new ValidationError
                {
                    Message = $"Invalid HTTP method: {method}",
                    LineNumber = result.LineNumber,
                    CommandName = command,
                    Severity = ValidationSeverity.Error,
                    Suggestion = "Use GET, POST, PUT, DELETE, PATCH, HEAD, or OPTIONS"
                });
            }
        }

        if (command == "PARSE" && tokens.Count > 2)
        {
            var type = tokens[2].ToUpper();
            if (validation.ValidTypes != null && !validation.ValidTypes.Contains(type))
            {
                result.Errors.Add(new ValidationError
                {
                    Message = $"Invalid parsing type: {type}",
                    LineNumber = result.LineNumber,
                    CommandName = command,
                    Severity = ValidationSeverity.Error,
                    Suggestion = "Use LR, REGEX, CSS, JSON, or XPATH"
                });
            }
        }

        if (command == "KEYCHECK" && tokens.Count > 1)
        {
            var condition = tokens[1].ToUpper();
            if (validation.ValidConditions != null && !validation.ValidConditions.Contains(condition))
            {
                result.Errors.Add(new ValidationError
                {
                    Message = $"Invalid keycheck condition: {condition}",
                    LineNumber = result.LineNumber,
                    CommandName = command,
                    Severity = ValidationSeverity.Error,
                    Suggestion = "Use SUCCESS, FAILURE, BAN, RETRY, or CUSTOM"
                });
            }
        }
    }

    private void ValidateFlowControl(string command, Stack<string> flowControlStack, int lineNumber, ValidationResult result)
    {
        switch (command)
        {
            case "IF":
            case "WHILE":
            case "FOR":
            case "TRY":
            case "FUNCTION":
                flowControlStack.Push(command);
                break;

            case "ENDIF":
                ValidateClosingBlock("IF", flowControlStack, lineNumber, result);
                break;

            case "ENDWHILE":
                ValidateClosingBlock("WHILE", flowControlStack, lineNumber, result);
                break;

            case "ENDFOR":
                ValidateClosingBlock("FOR", flowControlStack, lineNumber, result);
                break;

            case "ENDTRY":
                ValidateClosingBlock("TRY", flowControlStack, lineNumber, result);
                break;

            case "ENDFUNCTION":
                ValidateClosingBlock("FUNCTION", flowControlStack, lineNumber, result);
                break;

            case "ELSE":
            case "CATCH":
                if (flowControlStack.Count == 0 || 
                    (command == "ELSE" && flowControlStack.Peek() != "IF") ||
                    (command == "CATCH" && flowControlStack.Peek() != "TRY"))
                {
                    result.Errors.Add(new ValidationError
                    {
                        Message = $"{command} without matching opening block",
                        LineNumber = lineNumber,
                        CommandName = command,
                        Severity = ValidationSeverity.Error,
                        Suggestion = "Ensure proper block structure with opening statement"
                    });
                }
                break;
        }
    }

    private void ValidateClosingBlock(string expectedOpening, Stack<string> flowControlStack, int lineNumber, ValidationResult result)
    {
        if (flowControlStack.Count == 0)
        {
            result.Errors.Add(new ValidationError
            {
                Message = $"END{expectedOpening} without matching {expectedOpening}",
                LineNumber = lineNumber,
                CommandName = $"END{expectedOpening}",
                Severity = ValidationSeverity.Error,
                Suggestion = $"Add matching {expectedOpening} statement"
            });
        }
        else if (flowControlStack.Peek() != expectedOpening)
        {
            result.Errors.Add(new ValidationError
            {
                Message = $"Mismatched END{expectedOpening}, expected END{flowControlStack.Peek()}",
                LineNumber = lineNumber,
                CommandName = $"END{expectedOpening}",
                Severity = ValidationSeverity.Error,
                Suggestion = $"Use END{flowControlStack.Peek()} or fix block structure"
            });
        }
        else
        {
            flowControlStack.Pop();
        }
    }

    private void ValidateFlowControlBlocks(Stack<string> flowControlStack, ValidationResult result)
    {
        while (flowControlStack.Count > 0)
        {
            var unclosedBlock = flowControlStack.Pop();
            result.Errors.Add(new ValidationError
            {
                Message = $"Unclosed {unclosedBlock} block",
                LineNumber = 0,
                CommandName = unclosedBlock,
                Severity = ValidationSeverity.Error,
                Suggestion = $"Add END{unclosedBlock} statement to close block"
            });
        }
    }

    private void ValidateVariableUsage(string line, HashSet<string> variables, int lineNumber, ValidationResult result)
    {
        var variableMatches = Regex.Matches(line, @"<([^<>]+)>");
        foreach (Match match in variableMatches)
        {
            var variableName = match.Groups[1].Value;
            
            // Skip built-in variables
            if (IsBuiltInVariable(variableName))
                continue;

            if (!variables.Contains(variableName))
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Message = $"Variable '{variableName}' is used but never defined",
                    LineNumber = lineNumber,
                    CommandName = "VARIABLE",
                    Suggestion = $"Define variable '{variableName}' before using it"
                });
            }
        }
    }

    private void ValidateUrls(List<string> tokens, int lineNumber, ValidationResult result)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.StartsWith("\"http") && token.EndsWith("\""))
            {
                var url = token.Trim('"');
                if (!_validationPatterns["URL"].IsMatch(url))
                {
                    result.Warnings.Add(new ValidationWarning
                    {
                        Message = $"Invalid URL format: {url}",
                        LineNumber = lineNumber,
                        CommandName = "REQUEST",
                        Suggestion = "Check URL format (must include protocol like http:// or https://)"
                    });
                }
            }
        }
    }

    private void AnalyzeLogicalIssues(string[] lines, ValidationResult result, HashSet<string> variables)
    {
        bool hasSuccessKeycheck = false;
        int successLineNumber = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line) || IsComment(line))
                continue;

            var tokens = TokenizeLine(line);
            if (tokens.Count == 0)
                continue;

            var command = tokens[0].ToUpper();

            // Check for unreachable code after SUCCESS
            if (hasSuccessKeycheck && !IsComment(line) && command != "LABEL")
            {
                result.Warnings.Add(new ValidationWarning
                {
                    Message = "Unreachable code after SUCCESS keycheck",
                    LineNumber = i + 1,
                    CommandName = "KEYCHECK",
                    Suggestion = "Remove unreachable code or restructure logic"
                });
            }

            // Detect SUCCESS keycheck
            if (command == "KEYCHECK" && tokens.Count > 1 && tokens[1].ToUpper() == "SUCCESS")
            {
                hasSuccessKeycheck = true;
                successLineNumber = i + 1;
            }
        }
    }

    private List<string> TokenizeLine(string line)
    {
        var tokens = new List<string>();
        var inQuotes = false;
        var currentToken = string.Empty;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
                currentToken += c;
            }
            else if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (!string.IsNullOrEmpty(currentToken))
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
                }
            }
            else
            {
                currentToken += c;
            }
        }

        if (!string.IsNullOrEmpty(currentToken))
        {
            tokens.Add(currentToken);
        }

        return tokens;
    }

    private bool IsComment(string line)
    {
        var trimmed = line.Trim();
        return trimmed.StartsWith("//") || trimmed.StartsWith("#");
    }

    private bool IsBuiltInVariable(string variableName)
    {
        var builtInVariables = new[]
        {
            "SOURCE", "RESPONSECODE", "ADDRESS", "HEADERS", "COOKIES",
            "USERAGENT", "TIMEOUT", "PROXY", "BOTDATA", "INPUT",
            "RANDOM", "DATE", "TIME", "TIMESTAMP"
        };
        
        return builtInVariables.Contains(variableName.ToUpper());
    }

    #endregion
}

#region Supporting Models

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
}

public class LineValidationResult
{
    public int LineNumber { get; set; }
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
}



public class CommandValidation
{
    public int MinParameters { get; set; }
    public int MaxParameters { get; set; }
    public string[] RequiredParameters { get; set; } = Array.Empty<string>();
    public string[]? ValidMethods { get; set; }
    public string[]? ValidTypes { get; set; }
    public string[]? ValidConditions { get; set; }
}

#endregion
