using Microsoft.Extensions.Logging;
using OpenBullet.Core.Models;
using System.Text.RegularExpressions;

namespace OpenBullet.Core.Services;

/// <summary>
/// Advanced LoliScript syntax highlighting service with comprehensive language support
/// </summary>
public class SyntaxHighlightingService : ISyntaxHighlightingService
{
    private readonly ILogger<SyntaxHighlightingService> _logger;
    private readonly Dictionary<string, ColorScheme> _colorSchemes;
    private readonly Dictionary<string, SyntaxHighlightingRules> _highlightingRules;
    private readonly string[] _loliScriptCommands;
    private bool _isInitialized;

    public bool IsInitialized => _isInitialized;
    
    public string[] SupportedLanguages => new[] { "LoliScript", "JavaScript", "Python", "C#", "Generic" };

    public SyntaxHighlightingService(ILogger<SyntaxHighlightingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _colorSchemes = new Dictionary<string, ColorScheme>();
        _highlightingRules = new Dictionary<string, SyntaxHighlightingRules>();
        
        // LoliScript commands for syntax highlighting
        _loliScriptCommands = new[]
        {
            "REQUEST", "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS",
            "PARSE", "LR", "CSS", "XPATH", "REGEX", "JSON",
            "KEYCHECK", "SUCCESS", "FAIL", "BAN", "RETRY", "CUSTOM",
            "SET", "CAP", "LOG", "CLOG",
            "FUNCTION", "RETURN", "CALL",
            "IF", "ELSE", "ENDIF", "WHILE", "ENDWHILE", "FOR", "ENDFOR",
            "TRY", "CATCH", "ENDTRY",
            "JUMP", "LABEL", "BLOCK", "ENDBLOCK",
            "UTILITY", "TRANSLATE", "HASH", "HMAC", "AES", "RSA",
            "RECAPTCHA", "HCAPTCHA", "CAPTCHA"
        };

        Initialize();
    }

    private void Initialize()
    {
        try
        {
            InitializeColorSchemes();
            InitializeHighlightingRules();
            _isInitialized = true;
            _logger.LogInformation("SyntaxHighlightingService initialized successfully with {CommandCount} LoliScript commands", _loliScriptCommands.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize SyntaxHighlightingService");
            _isInitialized = false;
        }
    }

    private void InitializeColorSchemes()
    {
        // Dark theme (default)
        _colorSchemes["Dark"] = new ColorScheme
        {
            CommandColor = "#569CD6",     // Blue like VS Code keywords
            StringColor = "#CE9178",      // Orange like VS Code strings
            CommentColor = "#6A9955",     // Green like VS Code comments
            VariableColor = "#9CDCFE",    // Light blue like VS Code variables
            NumberColor = "#B5CEA8",      // Light green like VS Code numbers
            LabelColor = "#DCDCAA",       // Yellow like VS Code functions
            OperatorColor = "#D4D4D4"     // Light gray like VS Code operators
        };

        // Light theme
        _colorSchemes["Light"] = new ColorScheme
        {
            CommandColor = "#0000FF",     // Blue
            StringColor = "#A31515",      // Red
            CommentColor = "#008000",     // Green
            VariableColor = "#001080",    // Dark blue
            NumberColor = "#098658",      // Dark green
            LabelColor = "#795E26",       // Brown
            OperatorColor = "#000000"     // Black
        };

        // High Contrast theme
        _colorSchemes["HighContrast"] = new ColorScheme
        {
            CommandColor = "#00FFFF",     // Cyan
            StringColor = "#FFFF00",      // Yellow
            CommentColor = "#00FF00",     // Bright green
            VariableColor = "#FFFFFF",    // White
            NumberColor = "#FF00FF",      // Magenta
            LabelColor = "#FFA500",       // Orange
            OperatorColor = "#C0C0C0"     // Light gray
        };
    }

    private void InitializeHighlightingRules()
    {
        // LoliScript highlighting rules
        _highlightingRules["LoliScript"] = new SyntaxHighlightingRules
        {
            Language = "LoliScript",
            Keywords = _loliScriptCommands,
            Operators = new[] { "=", "==", "!=", ">", "<", ">=", "<=", "+", "-", "*", "/", "%", "&&", "||", "!" },
            StringDelimiters = new[] { "\"", "'" },
            CommentDelimiters = new[] { "#", "//" },
            BlockStartKeywords = new[] { "IF", "WHILE", "FOR", "TRY", "FUNCTION", "BLOCK" },
            BlockEndKeywords = new[] { "ENDIF", "ENDWHILE", "ENDFOR", "ENDTRY", "RETURN", "ENDBLOCK" },
            TokenPatterns = new Dictionary<TokenType, string>
            {
                [TokenType.Command] = @"\b(" + string.Join("|", _loliScriptCommands) + @")\b",
                [TokenType.String] = @"""([^""\\]|\\.)*""|'([^'\\]|\\.)*'",
                [TokenType.Number] = @"\b\d+(\.\d+)?\b",
                [TokenType.Variable] = @"<[^>]+>|\$[a-zA-Z_][a-zA-Z0-9_]*",
                [TokenType.Comment] = @"#.*$|//.*$",
                [TokenType.Label] = @"^[a-zA-Z_][a-zA-Z0-9_]*:",
                [TokenType.Operator] = @"[=!<>+\-*/%&|]+",
                [TokenType.Whitespace] = @"\s+"
            }
        };

        // Generic rules for other languages
        _highlightingRules["Generic"] = new SyntaxHighlightingRules
        {
            Language = "Generic",
            Keywords = new[] { "if", "else", "while", "for", "function", "return", "var", "let", "const" },
            Operators = new[] { "=", "==", "!=", ">", "<", ">=", "<=", "+", "-", "*", "/", "%" },
            StringDelimiters = new[] { "\"", "'" },
            CommentDelimiters = new[] { "//", "#" },
            TokenPatterns = new Dictionary<TokenType, string>
            {
                [TokenType.String] = @"""([^""\\]|\\.)*""|'([^'\\]|\\.)*'",
                [TokenType.Number] = @"\b\d+(\.\d+)?\b",
                [TokenType.Comment] = @"//.*$|#.*$",
                [TokenType.Whitespace] = @"\s+"
            }
        };
    }

    public List<SyntaxToken> TokenizeScript(string script)
        {
            if (string.IsNullOrEmpty(script))
                return new List<SyntaxToken>();

            var tokens = new List<SyntaxToken>();
        var rules = GetHighlightingRules("LoliScript");
            var lines = script.Split('\n');

        try
        {
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                var position = 0;

                while (position < line.Length)
                {
                    var token = GetNextToken(line, position, lineIndex + 1, rules);
                    if (token != null)
                    {
                        tokens.Add(token);
                        position = token.StartPosition + token.Length;
                    }
                    else
                    {
                        // Skip unknown character
                        position++;
                    }
                }
            }

            _logger.LogDebug("Tokenized script into {TokenCount} tokens", tokens.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tokenizing script");
        }

        return tokens;
    }

    private SyntaxToken? GetNextToken(string line, int startPosition, int lineNumber, SyntaxHighlightingRules rules)
    {
        if (startPosition >= line.Length)
            return null;

        // Skip whitespace
        if (char.IsWhiteSpace(line[startPosition]))
        {
            int length = 1;
            while (startPosition + length < line.Length && char.IsWhiteSpace(line[startPosition + length]))
                length++;

            return new SyntaxToken
            {
                Type = TokenType.Whitespace,
                Value = line.Substring(startPosition, length),
                StartPosition = startPosition,
                Length = length,
                LineNumber = lineNumber
            };
        }

        // Check each token type pattern
        foreach (var pattern in rules.TokenPatterns)
        {
            var regex = new Regex(pattern.Value, RegexOptions.IgnoreCase);
            var match = regex.Match(line, startPosition);

            if (match.Success && match.Index == startPosition)
            {
                return new SyntaxToken
                {
                    Type = pattern.Key,
                    Value = match.Value,
                    StartPosition = startPosition,
                    Length = match.Length,
                    LineNumber = lineNumber
                };
            }
        }

        // Default to single character if no pattern matches
        return new SyntaxToken
        {
            Type = TokenType.Whitespace,
            Value = line[startPosition].ToString(),
            StartPosition = startPosition,
            Length = 1,
            LineNumber = lineNumber
        };
    }

    public SyntaxHighlightResult HighlightSyntax(string script)
    {
        var result = new SyntaxHighlightResult();
        
        if (string.IsNullOrEmpty(script))
            return result;

        try
        {
            var tokens = TokenizeScript(script);
            result.Tokens = tokens;

            // Generate formatted text with color codes (can be extended for HTML/RTF output)
            var formattedLines = new List<string>();
            var lines = script.Split('\n');
            var scheme = GetColorScheme("Dark");

            for (int i = 0; i < lines.Length; i++)
            {
                var lineTokens = tokens.Where(t => t.LineNumber == i + 1).OrderBy(t => t.StartPosition);
                var formattedLine = FormatLineWithColors(lines[i], lineTokens, scheme);
                formattedLines.Add(formattedLine);
            }

            result.FormattedText = string.Join("\n", formattedLines);
            _logger.LogDebug("Highlighted syntax for script with {LineCount} lines", lines.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error highlighting syntax");
            result.FormattedText = script; // Fallback to original text
        }

        return result;
    }

    private string FormatLineWithColors(string line, IEnumerable<SyntaxToken> tokens, ColorScheme scheme)
    {
        // For now, return the original line (this could be extended to generate HTML/RTF)
        // In a real implementation, you'd wrap tokens with color formatting
        return line;
    }

    public ColorScheme GetColorScheme(string themeName)
    {
        if (_colorSchemes.TryGetValue(themeName, out var scheme))
            return scheme;

        _logger.LogWarning("Color scheme '{ThemeName}' not found, returning default Dark theme", themeName);
        return _colorSchemes["Dark"];
    }

    public string[] GetAvailableThemes()
    {
        return _colorSchemes.Keys.ToArray();
    }

    public SyntaxValidationResult ValidateSyntax(string script)
    {
        var result = new SyntaxValidationResult { IsValid = true };

        if (string.IsNullOrEmpty(script))
            return result;

        try
        {
            var lines = script.Split('\n');
            var errors = new List<SyntaxError>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("//"))
                    continue;

                // Basic validation rules
                ValidateLineStructure(line, i + 1, errors);
                ValidateCommandSyntax(line, i + 1, errors);
                ValidateStringQuotes(line, i + 1, errors);
            }

            result.Errors = errors;
            result.IsValid = errors.Count == 0;

            _logger.LogDebug("Validated syntax: {ErrorCount} errors found", errors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating syntax");
            result.Errors.Add(new SyntaxError
            {
                LineNumber = 1,
                Column = 1,
                Message = "Validation failed due to internal error",
                Severity = ErrorSeverity.Critical
            });
            result.IsValid = false;
        }

        return result;
    }

    private void ValidateLineStructure(string line, int lineNumber, List<SyntaxError> errors)
    {
        // Check for basic structural issues
        if (line.Contains('\t'))
        {
            errors.Add(new SyntaxError
            {
                LineNumber = lineNumber,
                Column = line.IndexOf('\t') + 1,
                Message = "Use spaces instead of tabs for indentation",
                Severity = ErrorSeverity.Warning
            });
        }
    }

    private void ValidateCommandSyntax(string line, int lineNumber, List<SyntaxError> errors)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        var command = parts[0].ToUpper();
        
        // Check if it's a known command
        if (!_loliScriptCommands.Contains(command) && !line.Contains(':') && !line.StartsWith("//") && !line.StartsWith("#"))
        {
            errors.Add(new SyntaxError
            {
                LineNumber = lineNumber,
                Column = 1,
                Message = $"Unknown command: {command}",
                Severity = ErrorSeverity.Error
            });
        }

        // Validate command-specific syntax
        switch (command)
        {
            case "REQUEST":
                if (parts.Length < 3)
                {
                    errors.Add(new SyntaxError
                    {
                        LineNumber = lineNumber,
                        Column = 1,
                        Message = "REQUEST command requires method and URL",
                        Severity = ErrorSeverity.Error
                    });
                }
                break;

            case "PARSE":
                if (parts.Length < 4)
                {
                    errors.Add(new SyntaxError
                    {
                        LineNumber = lineNumber,
                        Column = 1,
                        Message = "PARSE command requires parser type, pattern, and variable name",
                        Severity = ErrorSeverity.Error
                    });
                }
                break;

            case "KEYCHECK":
                if (parts.Length < 4)
                {
                    errors.Add(new SyntaxError
                    {
                        LineNumber = lineNumber,
                        Column = 1,
                        Message = "KEYCHECK command requires variable, condition, and key",
                        Severity = ErrorSeverity.Error
                    });
                }
                break;
        }
    }

    private void ValidateStringQuotes(string line, int lineNumber, List<SyntaxError> errors)
    {
        // Check for unmatched quotes
        int doubleQuotes = line.Count(c => c == '"');
        int singleQuotes = line.Count(c => c == '\'');

        if (doubleQuotes % 2 != 0)
        {
            errors.Add(new SyntaxError
            {
                LineNumber = lineNumber,
                Column = line.LastIndexOf('"') + 1,
                Message = "Unmatched double quote",
                Severity = ErrorSeverity.Error
            });
        }

        if (singleQuotes % 2 != 0)
        {
            errors.Add(new SyntaxError
            {
                LineNumber = lineNumber,
                Column = line.LastIndexOf('\'') + 1,
                Message = "Unmatched single quote",
                Severity = ErrorSeverity.Error
            });
        }
    }

    public SyntaxHighlightingRules GetHighlightingRules(string language)
    {
        if (_highlightingRules.TryGetValue(language, out var rules))
            return rules;

        _logger.LogWarning("Highlighting rules for '{Language}' not found, returning Generic rules", language);
        return _highlightingRules["Generic"];
    }
}
