namespace OpenBullet.Core.Parsing;

/// <summary>
/// Interface for data parsing operations
/// </summary>
public interface IDataParser
{
    /// <summary>
    /// Parse data using the specified method
    /// </summary>
    ParseResult Parse(string input, string pattern, ParseOptions options);

    /// <summary>
    /// Gets the parser type
    /// </summary>
    ParseType ParserType { get; }

    /// <summary>
    /// Validates the parsing pattern
    /// </summary>
    bool IsValidPattern(string pattern);
}

/// <summary>
/// Parse result container
/// </summary>
public class ParseResult
{
    public bool Success { get; set; }
    public List<string> Matches { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets the first match or default value
    /// </summary>
    public string GetFirstMatch(string defaultValue = "")
    {
        return Matches.Count > 0 ? Matches[0] : defaultValue;
    }

    /// <summary>
    /// Gets all matches as a joined string
    /// </summary>
    public string GetAllMatches(string separator = ", ")
    {
        return string.Join(separator, Matches);
    }
}

/// <summary>
/// Parse options for data extraction
/// </summary>
public class ParseOptions
{
    public bool Recursive { get; set; } = false;
    public bool IgnoreCase { get; set; } = false;
    public bool Multiline { get; set; } = false;
    public int MaxMatches { get; set; } = 0; // 0 = unlimited
    public string? AttributeName { get; set; } // For CSS/HTML parsing
    public string? JsonPath { get; set; } // For JSON parsing
    public string? LeftDelimiter { get; set; } // For LR parsing
    public string? RightDelimiter { get; set; } // For LR parsing
    public Dictionary<string, object> CustomOptions { get; set; } = new();
}

/// <summary>
/// Supported parsing types
/// </summary>
public enum ParseType
{
    LeftRight,  // LR - Simple left/right delimiter extraction
    Regex,      // REGEX - Regular expression pattern matching
    CSS,        // CSS - CSS selector for HTML/XML
    Json,       // JSON - JSON path extraction
    XPath       // XPATH - XPath for XML parsing
}

/// <summary>
/// Extension methods for ParseType enum
/// </summary>
public static class ParseTypeExtensions
{
    /// <summary>
    /// Converts LoliScript abbreviation to ParseType enum
    /// </summary>
    public static bool TryParseFromString(string value, out ParseType parseType)
    {
        parseType = default;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.ToUpperInvariant() switch
        {
            "LR" => (parseType = ParseType.LeftRight) == ParseType.LeftRight,
            "LEFTRIGHT" => (parseType = ParseType.LeftRight) == ParseType.LeftRight,
            "REGEX" => (parseType = ParseType.Regex) == ParseType.Regex,
            "CSS" => (parseType = ParseType.CSS) == ParseType.CSS,
            "JSON" => (parseType = ParseType.Json) == ParseType.Json,
            "XPATH" => (parseType = ParseType.XPath) == ParseType.XPath,
            _ => false
        };
    }

    /// <summary>
    /// Gets the LoliScript abbreviation for a ParseType
    /// </summary>
    public static string ToLoliScriptString(this ParseType parseType)
    {
        return parseType switch
        {
            ParseType.LeftRight => "LR",
            ParseType.Regex => "REGEX",
            ParseType.CSS => "CSS",
            ParseType.Json => "JSON",
            ParseType.XPath => "XPATH",
            _ => parseType.ToString()
        };
    }
}

/// <summary>
/// Interface for parser factory
/// </summary>
public interface IParserFactory
{
    /// <summary>
    /// Creates a parser for the specified type
    /// </summary>
    IDataParser CreateParser(ParseType parseType);

    /// <summary>
    /// Gets all available parser types
    /// </summary>
    IEnumerable<ParseType> GetAvailableTypes();

    /// <summary>
    /// Registers a custom parser
    /// </summary>
    void RegisterParser(ParseType parseType, IDataParser parser);
}

/// <summary>
/// Parse validation result
/// </summary>
public class ParseValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}
