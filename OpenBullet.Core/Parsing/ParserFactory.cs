using Microsoft.Extensions.Logging;
using OpenBullet.Core.Parsing.Parsers;
using System.Collections.Concurrent;

namespace OpenBullet.Core.Parsing;

/// <summary>
/// Factory for creating data parsers
/// </summary>
public class ParserFactory : IParserFactory
{
    private readonly ILogger<ParserFactory> _logger;
    private readonly ConcurrentDictionary<ParseType, Func<IDataParser>> _parserFactories = new();
    private readonly ConcurrentDictionary<ParseType, IDataParser> _singletonParsers = new();

    public ParserFactory(ILogger<ParserFactory> logger)
    {
        _logger = logger;
        RegisterDefaultParsers();
    }

    public IDataParser CreateParser(ParseType parseType)
    {
        try
        {
            // Check for singleton parser first
            if (_singletonParsers.TryGetValue(parseType, out var singletonParser))
            {
                return singletonParser;
            }

            // Check for factory method
            if (_parserFactories.TryGetValue(parseType, out var factory))
            {
                var parser = factory();
                _logger.LogTrace("Created parser for type {ParseType}", parseType);
                return parser;
            }

            throw new NotSupportedException($"Parser type {parseType} is not supported");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create parser for type {ParseType}", parseType);
            throw;
        }
    }

    public IEnumerable<ParseType> GetAvailableTypes()
    {
        var types = new HashSet<ParseType>();
        types.UnionWith(_parserFactories.Keys);
        types.UnionWith(_singletonParsers.Keys);
        return types.OrderBy(t => t.ToString());
    }

    public void RegisterParser(ParseType parseType, IDataParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);

        _singletonParsers[parseType] = parser;
        _logger.LogDebug("Registered singleton parser for type {ParseType}", parseType);
    }

    /// <summary>
    /// Registers a parser factory method
    /// </summary>
    public void RegisterParserFactory(ParseType parseType, Func<IDataParser> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _parserFactories[parseType] = factory;
        _logger.LogDebug("Registered parser factory for type {ParseType}", parseType);
    }

    /// <summary>
    /// Registers a parser type that will be created using reflection
    /// </summary>
    public void RegisterParserType<T>(ParseType parseType) where T : IDataParser, new()
    {
        _parserFactories[parseType] = () => new T();
        _logger.LogDebug("Registered parser type {TypeName} for {ParseType}", typeof(T).Name, parseType);
    }

    private void RegisterDefaultParsers()
    {
        // Register factory methods for default parsers
        RegisterParserFactory(ParseType.LeftRight, () => new LeftRightParser());
        RegisterParserFactory(ParseType.Regex, () => new RegexParser());
        RegisterParserFactory(ParseType.CSS, () => new CssParser());
        RegisterParserFactory(ParseType.Json, () => new JsonParser());

        // Register enhanced parsers as well
        RegisterParserFactory(ParseType.LeftRight, () => new EnhancedLeftRightParser());
        RegisterParserFactory(ParseType.Regex, () => new AdvancedRegexParser());
        RegisterParserFactory(ParseType.CSS, () => new EnhancedCssParser());
        RegisterParserFactory(ParseType.Json, () => new EnhancedJsonParser());

        _logger.LogInformation("Registered {Count} default parser types", _parserFactories.Count);
    }

    /// <summary>
    /// Gets parser information for debugging
    /// </summary>
    public ParserInfo GetParserInfo(ParseType parseType)
    {
        var info = new ParserInfo
        {
            ParseType = parseType,
            IsAvailable = _parserFactories.ContainsKey(parseType) || _singletonParsers.ContainsKey(parseType),
            IsSingleton = _singletonParsers.ContainsKey(parseType)
        };

        if (info.IsAvailable)
        {
            try
            {
                var parser = CreateParser(parseType);
                info.ParserTypeName = parser.GetType().Name;
                info.SupportsValidation = true; // All our parsers support validation
            }
            catch (Exception ex)
            {
                info.Error = ex.Message;
            }
        }

        return info;
    }

    /// <summary>
    /// Validates a pattern for a specific parser type
    /// </summary>
    public bool ValidatePattern(ParseType parseType, string pattern)
    {
        try
        {
            var parser = CreateParser(parseType);
            return parser.IsValidPattern(pattern);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets suggested patterns for a parser type
    /// </summary>
    public List<string> GetSuggestedPatterns(ParseType parseType)
    {
        return parseType switch
        {
            ParseType.LeftRight => new List<string>
            {
                "\"left\" \"right\"",
                "start|end",
                "<div> </div>"
            },
            ParseType.Regex => new List<string>
            {
                @"\d+",
                @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}",
                @"https?://[^\s]+",
                @"(\w+)=(\w+)"
            },
            ParseType.CSS => new List<string>
            {
                "div.className",
                "#elementId",
                "input[name='fieldName']",
                "a[href]",
                "table tr td:nth-child(2)"
            },
            ParseType.Json => new List<string>
            {
                "$.propertyName",
                "$[0]",
                "$.array[*].property",
                "$..recursiveProperty",
                "$[?(@.property == 'value')]"
            },
            _ => new List<string>()
        };
    }
}

/// <summary>
/// Parser information for debugging and metadata
/// </summary>
public class ParserInfo
{
    public ParseType ParseType { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsSingleton { get; set; }
    public string ParserTypeName { get; set; } = string.Empty;
    public bool SupportsValidation { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Parser factory extensions
/// </summary>
public static class ParserFactoryExtensions
{
    /// <summary>
    /// Parses data using the factory
    /// </summary>
    public static ParseResult Parse(this IParserFactory factory, ParseType parseType, string input, string pattern, ParseOptions? options = null)
    {
        var parser = factory.CreateParser(parseType);
        return parser.Parse(input, pattern, options ?? new ParseOptions());
    }

    /// <summary>
    /// Validates a pattern using the factory
    /// </summary>
    public static bool ValidatePattern(this IParserFactory factory, ParseType parseType, string pattern)
    {
        try
        {
            var parser = factory.CreateParser(parseType);
            return parser.IsValidPattern(pattern);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets parser capabilities
    /// </summary>
    public static Dictionary<ParseType, List<string>> GetAllCapabilities(this IParserFactory factory)
    {
        var capabilities = new Dictionary<ParseType, List<string>>();

        foreach (var parseType in factory.GetAvailableTypes())
        {
            var suggestions = factory is ParserFactory pf ? pf.GetSuggestedPatterns(parseType) : new List<string>();
            capabilities[parseType] = suggestions;
        }

        return capabilities;
    }
}
