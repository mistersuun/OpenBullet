namespace OpenBullet.Core.Models;

/// <summary>
/// Models for syntax highlighting and validation
/// </summary>

public enum TokenType
{
    Command,
    String,
    Number,
    Variable,
    Comment,
    Label,
    Operator,
    Whitespace
}

public class SyntaxToken
{
    public TokenType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public int StartPosition { get; set; }
    public int Length { get; set; }
    public int LineNumber { get; set; }
}

public class SyntaxHighlightResult
{
    public string FormattedText { get; set; } = string.Empty;
    public List<SyntaxToken> Tokens { get; set; } = new();
}

public class ColorScheme
{
    public string CommandColor { get; set; } = "#FF6B6B";
    public string StringColor { get; set; } = "#4ECDC4";
    public string CommentColor { get; set; } = "#95A5A6";
    public string VariableColor { get; set; } = "#F39C12";
    public string NumberColor { get; set; } = "#9B59B6";
    public string LabelColor { get; set; } = "#E74C3C";
    public string OperatorColor { get; set; } = "#ECF0F1";
}

public class SyntaxValidationResult
{
    public bool IsValid { get; set; }
    public List<SyntaxError> Errors { get; set; } = new();
}

public class SyntaxError
{
    public int LineNumber { get; set; }
    public int Column { get; set; }
    public string Message { get; set; } = string.Empty;
    public ErrorSeverity Severity { get; set; }
}

public enum ErrorSeverity
{
    Warning,
    Error,
    Critical
}

public class SyntaxHighlightingRules
{
    public string Language { get; set; } = string.Empty;
    public Dictionary<TokenType, string> TokenPatterns { get; set; } = new();
    public string[] Keywords { get; set; } = Array.Empty<string>();
    public string[] Operators { get; set; } = Array.Empty<string>();
    public string[] StringDelimiters { get; set; } = Array.Empty<string>();
    public string[] CommentDelimiters { get; set; } = Array.Empty<string>();
    public string[] BlockStartKeywords { get; set; } = Array.Empty<string>();
    public string[] BlockEndKeywords { get; set; } = Array.Empty<string>();
}

