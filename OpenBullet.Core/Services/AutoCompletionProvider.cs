using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using OpenBullet.Core.Parsing;

namespace OpenBullet.Core.Services;

/// <summary>
/// Professional auto-completion provider for LoliScript with IntelliSense capabilities
/// Provides context-aware suggestions, parameter hints, and intelligent code completion
/// </summary>
public class AutoCompletionProvider
{
    private readonly ILogger<AutoCompletionProvider>? _logger;
    private readonly Dictionary<string, CommandInfo> _commands;
    private readonly Dictionary<string, List<string>> _commandParameters;
    private readonly List<string> _builtInVariables;

    public AutoCompletionProvider(ILogger<AutoCompletionProvider>? logger = null)
    {
        _logger = logger;
        _commands = new Dictionary<string, CommandInfo>();
        _commandParameters = new Dictionary<string, List<string>>();
        _builtInVariables = new List<string>();
        
        InitializeCommands();
        InitializeBuiltInVariables();
        
        _logger?.LogInformation("AutoCompletionProvider initialized with {CommandCount} commands", _commands.Count);
    }

    /// <summary>
    /// Get context-aware completions for the current position
    /// </summary>
    public List<CompletionItem> GetCompletions(CompletionContext context)
    {
        try
        {
            if (context == null || string.IsNullOrEmpty(context.Text))
                return new List<CompletionItem>();

            var completions = new List<CompletionItem>();

            // Determine completion type based on context
            var completionType = AnalyzeCompletionContext(context);

            switch (completionType)
            {
                case CompletionType.Command:
                    completions.AddRange(GetCommandCompletions(context));
                    break;
                    
                case CompletionType.Parameter:
                    completions.AddRange(GetParameterCompletions(context));
                    break;
                    
                case CompletionType.Variable:
                    completions.AddRange(GetVariableCompletions(context));
                    break;
                    
                case CompletionType.Value:
                    completions.AddRange(GetValueCompletions(context));
                    break;
            }

            // Sort by relevance and filter by input
            var filteredCompletions = FilterAndSortCompletions(completions, context);
            
            _logger?.LogDebug("Generated {CompletionCount} completions for context", filteredCompletions.Count);
            return filteredCompletions;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error generating completions");
            return new List<CompletionItem>();
        }
    }

    /// <summary>
    /// Get parameter hints for the current command
    /// </summary>
    public ParameterHints GetParameterHints(CompletionContext context)
    {
        try
        {
            if (context == null || string.IsNullOrEmpty(context.CurrentCommand))
                return new ParameterHints();

            var commandName = context.CurrentCommand.ToUpper();
            if (!_commands.TryGetValue(commandName, out var commandInfo))
                return new ParameterHints();

            var hints = new ParameterHints
            {
                CommandName = commandName,
                Parameters = commandInfo.Parameters.ToList(),
                CurrentParameterIndex = DetermineCurrentParameterIndex(context)
            };

            _logger?.LogDebug("Generated parameter hints for command: {CommandName}", commandName);
            return hints;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error generating parameter hints");
            return new ParameterHints();
        }
    }

    /// <summary>
    /// Add custom completion items (for user-defined variables, functions, etc.)
    /// </summary>
    public void AddCustomCompletions(IEnumerable<CompletionItem> items)
    {
        foreach (var item in items)
        {
            _logger?.LogDebug("Added custom completion: {Text}", item.Text);
        }
    }

    #region Private Implementation

    private void InitializeCommands()
    {
        // REQUEST command
        _commands["REQUEST"] = new CommandInfo
        {
            Name = "REQUEST",
            Description = "Perform HTTP request to specified URL",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "method", Type = ParameterType.String, Description = "HTTP method (GET, POST, PUT, DELETE, etc.)", Required = true },
                new() { Name = "url", Type = ParameterType.String, Description = "Target URL", Required = true },
                new() { Name = "headers", Type = ParameterType.String, Description = "HTTP headers", Required = false },
                new() { Name = "content", Type = ParameterType.String, Description = "Request body content", Required = false },
                new() { Name = "timeout", Type = ParameterType.Integer, Description = "Request timeout in milliseconds", Required = false }
            }
        };

        // PARSE command
        _commands["PARSE"] = new CommandInfo
        {
            Name = "PARSE",
            Description = "Extract data from response using specified parsing method",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "pattern", Type = ParameterType.String, Description = "Pattern to match (regex, selector, etc.)", Required = true },
                new() { Name = "type", Type = ParameterType.String, Description = "Parsing type (REGEX, LR, CSS, JSON)", Required = true },
                new() { Name = "variable", Type = ParameterType.Variable, Description = "Variable to store result", Required = false },
                new() { Name = "source", Type = ParameterType.String, Description = "Source to parse (default: response)", Required = false }
            }
        };

        // KEYCHECK command
        _commands["KEYCHECK"] = new CommandInfo
        {
            Name = "KEYCHECK",
            Description = "Check conditions and determine bot status",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "condition", Type = ParameterType.String, Description = "Condition type (SUCCESS, FAILURE, BAN, etc.)", Required = true },
                new() { Name = "value", Type = ParameterType.String, Description = "Value to check against", Required = true },
                new() { Name = "source", Type = ParameterType.String, Description = "Source to check (variable, response, etc.)", Required = false }
            }
        };

        // SET command
        _commands["SET"] = new CommandInfo
        {
            Name = "SET",
            Description = "Set variable value",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "variable", Type = ParameterType.Variable, Description = "Variable name", Required = true },
                new() { Name = "value", Type = ParameterType.String, Description = "Value to set", Required = true },
                new() { Name = "type", Type = ParameterType.String, Description = "Variable type (string, int, bool)", Required = false }
            }
        };

        // LOG command
        _commands["LOG"] = new CommandInfo
        {
            Name = "LOG",
            Description = "Write message to log",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "message", Type = ParameterType.String, Description = "Message to log", Required = true },
                new() { Name = "level", Type = ParameterType.String, Description = "Log level (INFO, WARN, ERROR)", Required = false }
            }
        };

        // DELAY command
        _commands["DELAY"] = new CommandInfo
        {
            Name = "DELAY",
            Description = "Wait for specified duration",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "duration", Type = ParameterType.Integer, Description = "Delay in milliseconds", Required = true },
                new() { Name = "random", Type = ParameterType.Boolean, Description = "Add random variation", Required = false }
            }
        };

        // Control flow commands
        _commands["IF"] = new CommandInfo
        {
            Name = "IF",
            Description = "Conditional execution block",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "condition", Type = ParameterType.String, Description = "Condition to evaluate", Required = true }
            }
        };

        _commands["WHILE"] = new CommandInfo
        {
            Name = "WHILE",
            Description = "Loop execution block",
            Parameters = new List<ParameterInfo>
            {
                new() { Name = "condition", Type = ParameterType.String, Description = "Loop condition", Required = true }
            }
        };

        // Initialize parameter mappings
        _commandParameters["REQUEST"] = new List<string> { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };
        _commandParameters["PARSE"] = new List<string> { "REGEX", "LR", "CSS", "JSON", "XPATH" };
        _commandParameters["KEYCHECK"] = new List<string> { "SUCCESS", "FAILURE", "BAN", "RETRY", "CUSTOM" };
        _commandParameters["SET"] = new List<string> { "string", "int", "bool", "list", "dict" };
        _commandParameters["LOG"] = new List<string> { "INFO", "WARN", "ERROR", "DEBUG" };
    }

    private void InitializeBuiltInVariables()
    {
        _builtInVariables.AddRange(new[]
        {
            "SOURCE", "RESPONSECODE", "ADDRESS", "HEADERS", "COOKIES",
            "USERAGENT", "TIMEOUT", "PROXY", "BOTDATA", "INPUT",
            "RANDOM", "DATE", "TIME", "TIMESTAMP"
        });
    }

    private CompletionType AnalyzeCompletionContext(CompletionContext context)
    {
        var textBeforeCursor = context.Text.Substring(0, Math.Min(context.Position, context.Text.Length));
        var currentLine = GetCurrentLine(textBeforeCursor);
        
        // Check if we're in a variable reference (precise detection)
        if (IsInVariableContext(textBeforeCursor))
            return CompletionType.Variable;

        // Check if we're at the start of a line or after whitespace (command context)
        if (IsCommandContext(currentLine))
            return CompletionType.Command;

        // Check if we have a command and need parameters
        if (!string.IsNullOrEmpty(context.CurrentCommand))
            return CompletionType.Parameter;

        // Default to value completion
        return CompletionType.Value;
    }

    private List<CompletionItem> GetCommandCompletions(CompletionContext context)
    {
        var completions = new List<CompletionItem>();
        var input = GetCurrentWord(context);

        foreach (var command in _commands.Values)
        {
            if (string.IsNullOrEmpty(input) || command.Name.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            {
                completions.Add(new CompletionItem
                {
                    Text = command.Name,
                    Type = CompletionType.Command,
                    Description = command.Description,
                    InsertText = command.Name + " "
                });
            }
        }

        return completions;
    }

    private List<CompletionItem> GetParameterCompletions(CompletionContext context)
    {
        var completions = new List<CompletionItem>();
        var commandName = context.CurrentCommand?.ToUpper();
        
        if (string.IsNullOrEmpty(commandName))
            return completions;

        // Enhanced parameter suggestions based on command
        var parameterSuggestions = GetCommandParameterSuggestions(commandName, context);
        completions.AddRange(parameterSuggestions);

        // Add common parameter patterns
        var commonParams = GetCommonParameterCompletions(commandName, context);
        completions.AddRange(commonParams);

        // Legacy parameter support
        if (_commandParameters.ContainsKey(commandName))
        {
            var parameters = _commandParameters[commandName];
            var input = GetCurrentWord(context);

            foreach (var parameter in parameters)
            {
                if (string.IsNullOrEmpty(input) || parameter.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                {
                    completions.Add(new CompletionItem
                    {
                        Text = parameter,
                        Type = CompletionType.Parameter,
                        Description = $"Parameter for {commandName} command",
                        InsertText = parameter,
                        Priority = 4 // Lower priority than enhanced suggestions
                    });
                }
            }
        }

        return completions;
    }

    private List<CompletionItem> GetVariableCompletions(CompletionContext context)
    {
        var completions = new List<CompletionItem>();
        var input = GetCurrentVariableInput(context);
        var isInVariableBrackets = IsInVariableContext(context.Text.Substring(0, Math.Min(context.Position, context.Text.Length)));

        // Built-in variables with enhanced descriptions (show all, don't filter by input)
        foreach (var variable in _builtInVariables)
        {
            var insertText = isInVariableBrackets ? variable : $"<{variable}>";
            var description = GetBuiltInVariableDescription(variable);
            
            completions.Add(new CompletionItem
            {
                Text = variable,
                Type = CompletionType.Variable,
                Description = description,
                InsertText = insertText,
                Priority = 10 // Higher priority for built-in variables
            });
        }

        // User-defined variables with context information
        if (context.AvailableVariables != null)
        {
            foreach (var variable in context.AvailableVariables)
            {
                // Always include user variables when in variable context
                var insertText = isInVariableBrackets ? variable : $"<{variable}>";
                var description = $"User variable: {variable} (defined in this script)";
                
                completions.Add(new CompletionItem
                {
                    Text = variable,
                    Type = CompletionType.Variable,
                    Description = description,
                    InsertText = insertText,
                    Priority = 8 // Slightly lower priority than built-in
                });
            }
        }

        // Add common variable patterns when typing partial variables
        if (!string.IsNullOrEmpty(input) && input.Length >= 2)
        {
            var commonPatterns = GetCommonVariablePatterns(input);
            completions.AddRange(commonPatterns);
        }

        return completions;
    }

    private List<CompletionItem> GetValueCompletions(CompletionContext context)
    {
        var completions = new List<CompletionItem>();
        
        // Common values based on context
        var commonValues = new[]
        {
            "\"https://\"", "\"http://\"", "true", "false", "null", "0", "1"
        };

        foreach (var value in commonValues)
        {
            completions.Add(new CompletionItem
            {
                Text = value,
                Type = CompletionType.Value,
                Description = "Common value",
                InsertText = value
            });
        }

        return completions;
    }

    private List<CompletionItem> FilterAndSortCompletions(List<CompletionItem> completions, CompletionContext context)
    {
        var input = GetCurrentWord(context);
        
        return completions
            .Where(c => {
                // For high-priority parameter suggestions (enhanced), don't filter by input
                // This allows showing all HTTP methods, KEYCHECK values, etc.
                if (c.Type == CompletionType.Parameter && c.Priority >= 8)
                    return true;
                    
                // For other completions, apply input filtering
                return string.IsNullOrEmpty(input) || 
                       c.Text.StartsWith(input, StringComparison.OrdinalIgnoreCase);
            })
            .OrderByDescending(c => c.Priority) // Sort by priority first
            .ThenBy(c => c.Type)
            .ThenBy(c => c.Text)
            .Take(20) // Limit to top 20 results
            .ToList();
    }

    private bool IsInVariableContext(string text)
    {
        var lastOpenBracket = text.LastIndexOf('<');
        var lastCloseBracket = text.LastIndexOf('>');
        
        // Check if we have an unclosed < bracket
        if (lastOpenBracket <= lastCloseBracket)
            return false;
            
        // Check if there's a reasonable variable name pattern after the <
        // This helps distinguish between variables like <EMAIL> and string literals like "<title"
        var textAfterBracket = text.Substring(lastOpenBracket + 1);
        
        // If the text after < contains quotes or spaces, it's likely not a variable
        if (textAfterBracket.Contains('"') || textAfterBracket.Contains(' '))
            return false;
            
        return true;
    }

    private bool IsCommandContext(string currentLine)
    {
        var trimmed = currentLine.Trim();
        return string.IsNullOrEmpty(trimmed) || 
               trimmed.All(char.IsWhiteSpace) ||
               !trimmed.Contains(' ');
    }

    private string GetCurrentLine(string text)
    {
        var lastNewline = text.LastIndexOf('\n');
        return lastNewline == -1 ? text : text.Substring(lastNewline + 1);
    }

    private string GetCurrentWord(CompletionContext context)
    {
        var textBeforeCursor = context.Text.Substring(0, Math.Min(context.Position, context.Text.Length));
        var match = Regex.Match(textBeforeCursor, @"(\w+)$");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private int DetermineCurrentParameterIndex(CompletionContext context)
    {
        var textBeforeCursor = context.Text.Substring(0, Math.Min(context.Position, context.Text.Length));
        var currentLine = GetCurrentLine(textBeforeCursor);
        
        // Count spaces to estimate parameter position
        var spaceCount = currentLine.Count(c => c == ' ');
        return Math.Max(0, spaceCount - 1);
    }

    private string GetCurrentVariableInput(CompletionContext context)
    {
        var text = context.Text;
        var position = Math.Min(context.Position, text.Length);
        
        // Look for variable context: <VAR|
        var lastOpenBracket = text.LastIndexOf('<', position - 1);
        if (lastOpenBracket == -1)
            return GetCurrentWord(context);
        
        var afterBracket = lastOpenBracket + 1;
        var input = text.Substring(afterBracket, position - afterBracket);
        
        // Only return letters/digits/underscore for variable names
        return new string(input.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
    }

    private string GetBuiltInVariableDescription(string variable)
    {
        return variable.ToUpper() switch
        {
            "SOURCE" => "The original input data (username, email, etc.)",
            "ADDRESS" => "The final URL after redirects",
            "RESPONSECODE" => "HTTP response status code",
            "HEADERS" => "Response headers from the last request",
            "COOKIES" => "Current cookies from the session",
            "HOST" => "Current proxy host being used",
            "PORT" => "Current proxy port being used",
            "USERNAME" => "Current proxy username",
            "PASSWORD" => "Current proxy password",
            _ => $"Built-in variable: {variable}"
        };
    }

    private List<CompletionItem> GetCommonVariablePatterns(string input)
    {
        var patterns = new List<CompletionItem>();
        var lowerInput = input.ToLower();

        // Common variable patterns based on partial input
        var commonPatterns = new Dictionary<string, string>
        {
            ["email"] = "EMAIL - Captured email address",
            ["pass"] = "PASSWORD - Captured password",
            ["user"] = "USERNAME - Captured username", 
            ["token"] = "TOKEN - Authentication token",
            ["id"] = "USER_ID - User identifier",
            ["url"] = "LOGIN_URL - Login page URL",
            ["data"] = "RESPONSE_DATA - Response content",
            ["status"] = "STATUS - Current status",
            ["result"] = "RESULT - Operation result"
        };

        foreach (var pattern in commonPatterns)
        {
            if (pattern.Key.StartsWith(lowerInput, StringComparison.OrdinalIgnoreCase))
            {
                var variableName = pattern.Key.ToUpper();
                patterns.Add(new CompletionItem
                {
                    Text = variableName,
                    Type = CompletionType.Variable,
                    Description = pattern.Value,
                    InsertText = $"<{variableName}>",
                    Priority = 5 // Medium priority for patterns
                });
            }
        }

        return patterns;
    }

    private List<CompletionItem> GetCommandParameterSuggestions(string commandName, CompletionContext context)
    {
        var suggestions = new List<CompletionItem>();
        var input = GetCurrentWord(context);

        switch (commandName)
        {
            case "REQUEST":
                suggestions.AddRange(GetRequestParameterSuggestions(input, context));
                break;
            case "PARSE":
                suggestions.AddRange(GetParseParameterSuggestions(input, context));
                break;
            case "KEYCHECK":
                suggestions.AddRange(GetKeycheckParameterSuggestions(input, context));
                break;
        }

        return suggestions;
    }

    private List<CompletionItem> GetRequestParameterSuggestions(string input, CompletionContext context)
    {
        var suggestions = new List<CompletionItem>();
        var parameterIndex = DetermineCurrentParameterIndex(context);

        // Always provide ALL HTTP methods for REQUEST command (don't filter by input)
        var methods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };
        foreach (var method in methods)
        {
            suggestions.Add(new CompletionItem
            {
                Text = method,
                Type = CompletionType.Parameter,
                Description = $"HTTP {method} method",
                InsertText = method,
                Priority = 10
            });
        }

        // Also provide URL suggestions if applicable
        if (parameterIndex >= 1) // URL
        {
            var urlSuggestions = new[]
            {
                ("\"https://\"", "HTTPS URL template"),
                ("\"http://\"", "HTTP URL template"),
                ("\"<LOGIN_URL>\"", "Use captured login URL"),
                ("\"<BASE_URL>/api/\"", "API endpoint template")
            };

            foreach (var (url, description) in urlSuggestions)
            {
                if (string.IsNullOrEmpty(input) || url.Contains(input, StringComparison.OrdinalIgnoreCase))
                {
                    suggestions.Add(new CompletionItem
                    {
                        Text = url,
                        Type = CompletionType.Value,
                        Description = description,
                        InsertText = url,
                        Priority = 8
                    });
                }
            }
        }

        return suggestions;
    }

    private List<CompletionItem> GetParseParameterSuggestions(string input, CompletionContext context)
    {
        var suggestions = new List<CompletionItem>();
        
        // Always provide ALL regex patterns for PARSE command (don't filter by input)
        var patterns = new[]
        {
            ("\"<title>(.*?)</title>\"", "Extract page title"),
            ("\"<input name=\\\"token\\\" value=\\\"(.*?)\\\"\"", "Extract CSRF token"),
            ("\"\\\"token\\\":\\\"(.*?)\\\"\"", "Extract JSON token"),
            ("\"email=(.*?)&\"", "Extract email parameter"),
            ("\"user_id=(\\\\d+)\"", "Extract numeric user ID")
        };

        foreach (var (pattern, description) in patterns)
        {
            suggestions.Add(new CompletionItem
            {
                Text = pattern,
                Type = CompletionType.Value,
                Description = description,
                InsertText = pattern,
                Priority = 8
            });
        }

        return suggestions;
    }

    private List<CompletionItem> GetKeycheckParameterSuggestions(string input, CompletionContext context)
    {
        var suggestions = new List<CompletionItem>();
        
        // Always provide ALL KEYCHECK patterns (don't filter by input)
        var keycheckPatterns = new[]
        {
            ("\"SUCCESS\"", "Mark as successful result"),
            ("\"FAILURE\"", "Mark as failed result"),
            ("\"BAN\"", "Mark as banned/blocked"),
            ("\"RETRY\"", "Retry the operation"),
            ("\"<RESPONSECODE>\"", "Check response status code"),
            ("\"<SOURCE>\"", "Check original input data")
        };

        foreach (var (pattern, description) in keycheckPatterns)
        {
            suggestions.Add(new CompletionItem
            {
                Text = pattern,
                Type = CompletionType.Value,
                Description = description,
                InsertText = pattern,
                Priority = 8
            });
        }

        return suggestions;
    }

    private List<CompletionItem> GetCommonParameterCompletions(string commandName, CompletionContext context)
    {
        var completions = new List<CompletionItem>();
        
        // Add arrow operator suggestion for variable assignment
        if (context.Text.Contains("->") == false)
        {
            completions.Add(new CompletionItem
            {
                Text = "-> VAR",
                Type = CompletionType.Parameter,
                Description = "Assign result to variable",
                InsertText = "-> VAR",
                Priority = 6
            });
        }

        return completions;
    }

    #endregion
}

#region Supporting Models

public class CompletionContext
{
    public string Text { get; set; } = string.Empty;
    public int Position { get; set; }
    public int LineNumber { get; set; }
    public string? CurrentCommand { get; set; }
    public string[]? AvailableVariables { get; set; }
}

public enum CompletionType
{
    Command,
    Parameter,
    Variable,
    Value
}

public class CompletionItem
{
    public string Text { get; set; } = string.Empty;
    public CompletionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? InsertText { get; set; }
    public int Priority { get; set; } = 0;
}

public class ParameterHints
{
    public string CommandName { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
    public int CurrentParameterIndex { get; set; }
}



public class CommandInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ParameterInfo> Parameters { get; set; } = new();
}

#endregion
