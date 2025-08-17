using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenBullet.Core.Parsing.Parsers;

/// <summary>
/// JSON parser for extracting data using JSON paths
/// </summary>
public class JsonParser : IDataParser
{
    public ParseType ParserType => ParseType.Json;

    public ParseResult Parse(string input, string pattern, ParseOptions options)
    {
        var result = new ParseResult();

        try
        {
            if (string.IsNullOrEmpty(input))
            {
                result.ErrorMessage = "Input JSON is empty";
                return result;
            }

            if (string.IsNullOrEmpty(pattern))
            {
                result.ErrorMessage = "JSON path pattern is empty";
                return result;
            }

            // Parse JSON
            JToken jsonToken;
            try
            {
                jsonToken = JToken.Parse(input);
            }
            catch (JsonReaderException ex)
            {
                result.ErrorMessage = $"Invalid JSON: {ex.Message}";
                return result;
            }

            // Extract values using JSON path
            var matches = ExtractFromJsonPath(jsonToken, pattern, options);
            result.Matches = matches;
            result.Success = matches.Count > 0;

            // Add metadata
            result.Metadata["JsonPath"] = pattern;
            result.Metadata["JsonType"] = jsonToken.Type.ToString();
            
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"JSON parsing error: {ex.Message}";
            return result;
        }
    }

    public bool IsValidPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        try
        {
            // Test with a simple JSON object
            var testJson = JObject.Parse(@"{""test"": ""value""}");
            testJson.SelectTokens(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private List<string> ExtractFromJsonPath(JToken jsonToken, string pattern, ParseOptions options)
    {
        var matches = new List<string>();

        try
        {
            var tokens = jsonToken.SelectTokens(pattern);
            var maxMatches = options.MaxMatches > 0 ? options.MaxMatches : int.MaxValue;

            foreach (var token in tokens.Take(maxMatches))
            {
                var value = ExtractValueFromToken(token, options);
                if (value != null)
                {
                    matches.Add(value);
                }
            }
        }
        catch (Exception ex)
        {
            // Add error information but don't fail completely
            if (matches.Count == 0)
            {
                throw new InvalidOperationException($"JSON path extraction failed: {ex.Message}");
            }
        }

        return matches;
    }

    private string? ExtractValueFromToken(JToken token, ParseOptions options)
    {
        // Handle different token types
        switch (token.Type)
        {
            case JTokenType.String:
            case JTokenType.Integer:
            case JTokenType.Float:
            case JTokenType.Boolean:
            case JTokenType.Date:
            case JTokenType.Guid:
            case JTokenType.TimeSpan:
            case JTokenType.Uri:
                return token.Value<string>();

            case JTokenType.Null:
                return options.CustomOptions.TryGetValue("IncludeNull", out var includeNull) && 
                       includeNull is bool includeNullBool && includeNullBool ? "null" : null;

            case JTokenType.Array:
                return options.CustomOptions.TryGetValue("ArrayFormat", out var arrayFormat) && 
                       arrayFormat is string format ? 
                       FormatArray(token as JArray, format) : 
                       token.ToString(Formatting.None);

            case JTokenType.Object:
                return options.CustomOptions.TryGetValue("ObjectFormat", out var objectFormat) && 
                       objectFormat is string objFormat ? 
                       FormatObject(token as JObject, objFormat) : 
                       token.ToString(Formatting.None);

            default:
                return token.ToString();
        }
    }

    private string FormatArray(JArray? array, string format)
    {
        if (array == null) return string.Empty;

        return format.ToLowerInvariant() switch
        {
            "comma" => string.Join(", ", array.Select(t => t.ToString())),
            "json" => array.ToString(Formatting.None),
            "values" => string.Join("|", array.Select(t => t.Type == JTokenType.String ? t.Value<string>() : t.ToString())),
            "count" => array.Count.ToString(),
            _ => array.ToString(Formatting.None)
        };
    }

    private string FormatObject(JObject? obj, string format)
    {
        if (obj == null) return string.Empty;

        return format.ToLowerInvariant() switch
        {
            "keys" => string.Join(", ", obj.Properties().Select(p => p.Name)),
            "values" => string.Join(", ", obj.Properties().Select(p => p.Value.ToString())),
            "json" => obj.ToString(Formatting.None),
            "count" => obj.Properties().Count().ToString(),
            _ => obj.ToString(Formatting.None)
        };
    }
}

/// <summary>
/// Enhanced JSON parser with additional features
/// </summary>
public class EnhancedJsonParser : JsonParser
{
    public new ParseResult Parse(string input, string pattern, ParseOptions options)
    {
        var result = base.Parse(input, pattern, options);

        if (result.Success)
        {
            // Apply enhancements
            result = ApplyEnhancements(result, input, pattern, options);
        }

        return result;
    }

    private ParseResult ApplyEnhancements(ParseResult result, string input, string pattern, ParseOptions options)
    {
        try
        {
            // Extract JSON structure information
            if (options.CustomOptions.TryGetValue("IncludeStructure", out var includeStructure) && 
                includeStructure is bool includeStructureBool && includeStructureBool)
            {
                var structure = AnalyzeJsonStructure(input);
                result.Metadata["JsonStructure"] = structure;
            }

            // Extract multiple paths if requested
            if (options.CustomOptions.TryGetValue("MultiplePaths", out var multiplePaths) && 
                multiplePaths is string[] paths && paths.Length > 0)
            {
                var multiResults = ExtractMultiplePaths(input, paths, options);
                result.Metadata["MultiplePaths"] = multiResults;
            }

            // Validate extracted values if requested
            if (options.CustomOptions.ContainsKey("ValidateTypes"))
            {
                var typeValidation = ValidateExtractedTypes(result.Matches, options);
                result.Metadata["TypeValidation"] = typeValidation;
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Metadata["EnhancementError"] = ex.Message;
            return result;
        }
    }

    private JsonStructureInfo AnalyzeJsonStructure(string input)
    {
        try
        {
            var token = JToken.Parse(input);
            return AnalyzeToken(token);
        }
        catch
        {
            return new JsonStructureInfo { Type = "Invalid", IsValid = false };
        }
    }

    private JsonStructureInfo AnalyzeToken(JToken token)
    {
        var info = new JsonStructureInfo
        {
            Type = token.Type.ToString(),
            IsValid = true
        };

        switch (token.Type)
        {
            case JTokenType.Object:
                var obj = token as JObject;
                info.PropertyCount = obj?.Properties().Count() ?? 0;
                info.Properties = obj?.Properties().Select(p => p.Name).ToList() ?? new List<string>();
                break;

            case JTokenType.Array:
                var array = token as JArray;
                info.ItemCount = array?.Count ?? 0;
                if (array?.Count > 0)
                {
                    info.ArrayItemType = array[0].Type.ToString();
                }
                break;

            case JTokenType.Property:
                var prop = token as JProperty;
                info.PropertyName = prop?.Name ?? string.Empty;
                if (prop?.Value != null)
                {
                    info.ValueType = prop.Value.Type.ToString();
                }
                break;
        }

        return info;
    }

    private Dictionary<string, List<string>> ExtractMultiplePaths(string input, string[] paths, ParseOptions options)
    {
        var results = new Dictionary<string, List<string>>();

        try
        {
            var jsonToken = JToken.Parse(input);

            foreach (var path in paths)
            {
                try
                {
                    var pathMatches = ExtractFromJsonPath(jsonToken, path, options);
                    results[path] = pathMatches;
                }
                catch (Exception ex)
                {
                    results[path] = new List<string> { $"Error: {ex.Message}" };
                }
            }
        }
        catch
        {
            // Return empty results on parsing error
        }

        return results;
    }

    private List<string> ExtractFromJsonPath(JToken jsonToken, string pattern, ParseOptions options)
    {
        // Use the base class method through reflection or reimplementation
        var matches = new List<string>();

        try
        {
            var tokens = jsonToken.SelectTokens(pattern);
            var maxMatches = options.MaxMatches > 0 ? options.MaxMatches : int.MaxValue;

            foreach (var token in tokens.Take(maxMatches))
            {
                var value = ExtractValueFromToken(token, options);
                if (value != null)
                {
                    matches.Add(value);
                }
            }
        }
        catch
        {
            // Ignore errors for enhancement features
        }

        return matches;
    }

    private string? ExtractValueFromToken(JToken token, ParseOptions options)
    {
        // Duplicate the base implementation for access
        switch (token.Type)
        {
            case JTokenType.String:
            case JTokenType.Integer:
            case JTokenType.Float:
            case JTokenType.Boolean:
                return token.Value<string>();
            default:
                return token.ToString();
        }
    }

    private Dictionary<string, object> ValidateExtractedTypes(List<string> matches, ParseOptions options)
    {
        var validation = new Dictionary<string, object>
        {
            ["TotalMatches"] = matches.Count,
            ["NumericCount"] = 0,
            ["BooleanCount"] = 0,
            ["StringCount"] = 0,
            ["EmptyCount"] = 0
        };

        foreach (var match in matches)
        {
            if (string.IsNullOrEmpty(match))
            {
                validation["EmptyCount"] = (int)validation["EmptyCount"] + 1;
            }
            else if (bool.TryParse(match, out _))
            {
                validation["BooleanCount"] = (int)validation["BooleanCount"] + 1;
            }
            else if (double.TryParse(match, out _))
            {
                validation["NumericCount"] = (int)validation["NumericCount"] + 1;
            }
            else
            {
                validation["StringCount"] = (int)validation["StringCount"] + 1;
            }
        }

        return validation;
    }
}

/// <summary>
/// JSON structure information
/// </summary>
public class JsonStructureInfo
{
    public string Type { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public int PropertyCount { get; set; }
    public int ItemCount { get; set; }
    public List<string> Properties { get; set; } = new();
    public string PropertyName { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public string ArrayItemType { get; set; } = string.Empty;
}

/// <summary>
/// JSON path helper utilities
/// </summary>
public static class JsonPathHelper
{
    /// <summary>
    /// Common JSON path expressions
    /// </summary>
    public static class Common
    {
        public const string Root = "$";
        public const string AllProperties = "$.*";
        public const string AllArrayItems = "$[*]";
        public const string FirstItem = "$[0]";
        public const string LastItem = "$[-1]";
        public const string RecursiveSearch = "$..";
    }

    /// <summary>
    /// Builds property path
    /// </summary>
    public static string Property(string propertyName)
    {
        return $"$.{propertyName}";
    }

    /// <summary>
    /// Builds array index path
    /// </summary>
    public static string ArrayIndex(int index)
    {
        return $"$[{index}]";
    }

    /// <summary>
    /// Builds nested property path
    /// </summary>
    public static string NestedProperty(params string[] properties)
    {
        return "$." + string.Join(".", properties);
    }

    /// <summary>
    /// Builds recursive search path
    /// </summary>
    public static string RecursiveProperty(string propertyName)
    {
        return $"$..{propertyName}";
    }

    /// <summary>
    /// Builds filtered array path
    /// </summary>
    public static string FilteredArray(string filterExpression)
    {
        return $"$[?({filterExpression})]";
    }
}
