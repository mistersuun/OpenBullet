using System.Text.RegularExpressions;

namespace OpenBullet.Core.Parsing.Parsers;

/// <summary>
/// Regular expression parser for pattern-based data extraction
/// </summary>
public class RegexParser : IDataParser
{
    public ParseType ParserType => ParseType.Regex;

    public ParseResult Parse(string input, string pattern, ParseOptions options)
    {
        var result = new ParseResult();

        try
        {
            if (string.IsNullOrEmpty(input))
            {
                result.ErrorMessage = "Input string is empty";
                return result;
            }

            if (string.IsNullOrEmpty(pattern))
            {
                result.ErrorMessage = "Regex pattern is empty";
                return result;
            }

            // Build regex options
            var regexOptions = BuildRegexOptions(options);

            // Create and execute regex
            var regex = new Regex(pattern, regexOptions);
            var matches = regex.Matches(input);

            // Extract results
            var extractedMatches = ExtractMatchResults(matches, options);
            result.Matches = extractedMatches;
            result.Success = extractedMatches.Count > 0;

            // Add metadata
            result.Metadata["Pattern"] = pattern;
            result.Metadata["RegexOptions"] = regexOptions.ToString();
            result.Metadata["MatchCount"] = matches.Count;
            result.Metadata["ExtractedCount"] = extractedMatches.Count;

            return result;
        }
        catch (ArgumentException ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Invalid regex pattern: {ex.Message}";
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Regex parsing error: {ex.Message}";
            return result;
        }
    }

    public bool IsValidPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        try
        {
            var regex = new Regex(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected RegexOptions BuildRegexOptions(ParseOptions options)
    {
        var regexOptions = RegexOptions.None;

        if (options.IgnoreCase)
            regexOptions |= RegexOptions.IgnoreCase;

        if (options.Multiline)
            regexOptions |= RegexOptions.Multiline;

        // Check for additional regex options in custom options
        if (options.CustomOptions.TryGetValue("Singleline", out var singleline) && 
            singleline is bool singlelineBool && singlelineBool)
        {
            regexOptions |= RegexOptions.Singleline;
        }

        if (options.CustomOptions.TryGetValue("ExplicitCapture", out var explicitCapture) && 
            explicitCapture is bool explicitCaptureBool && explicitCaptureBool)
        {
            regexOptions |= RegexOptions.ExplicitCapture;
        }

        if (options.CustomOptions.TryGetValue("Compiled", out var compiled) && 
            compiled is bool compiledBool && compiledBool)
        {
            regexOptions |= RegexOptions.Compiled;
        }

        return regexOptions;
    }

    private List<string> ExtractMatchResults(MatchCollection matches, ParseOptions options)
    {
        var results = new List<string>();
        var maxMatches = options.MaxMatches > 0 ? options.MaxMatches : int.MaxValue;

        foreach (Match match in matches.Take(maxMatches))
        {
            if (!match.Success)
                continue;

            // Determine what to extract from the match
            var extractedValue = ExtractFromMatch(match, options);
            if (extractedValue != null)
            {
                results.Add(extractedValue);
            }
        }

        return results;
    }

    private string? ExtractFromMatch(Match match, ParseOptions options)
    {
        // Check if a specific group is requested
        if (options.CustomOptions.TryGetValue("GroupName", out var groupName) && 
            groupName is string groupNameStr && !string.IsNullOrEmpty(groupNameStr))
        {
            var group = match.Groups[groupNameStr];
            return group.Success ? group.Value : null;
        }

        if (options.CustomOptions.TryGetValue("GroupIndex", out var groupIndex) && 
            groupIndex is int groupIndexInt && groupIndexInt >= 0 && groupIndexInt < match.Groups.Count)
        {
            var group = match.Groups[groupIndexInt];
            return group.Success ? group.Value : null;
        }

        // If no specific group requested, use the first capturing group or the entire match
        if (match.Groups.Count > 1)
        {
            // Return first capturing group (Groups[0] is the entire match)
            for (int i = 1; i < match.Groups.Count; i++)
            {
                var group = match.Groups[i];
                if (group.Success)
                {
                    return group.Value;
                }
            }
        }

        // Return the entire match
        return match.Value;
    }
}

/// <summary>
/// Advanced regex parser with additional features
/// </summary>
public class AdvancedRegexParser : RegexParser
{
    public new ParseResult Parse(string input, string pattern, ParseOptions options)
    {
        var result = base.Parse(input, pattern, options);

        if (result.Success && result.Matches.Count > 0)
        {
            // Apply advanced processing
            result = ApplyAdvancedProcessing(result, input, pattern, options);
        }

        return result;
    }

    private ParseResult ApplyAdvancedProcessing(ParseResult result, string input, string pattern, ParseOptions options)
    {
        try
        {
            // Extract named groups if requested
            if (options.CustomOptions.TryGetValue("ExtractNamedGroups", out var extractNamed) && 
                extractNamed is bool extractNamedBool && extractNamedBool)
            {
                var namedGroups = ExtractNamedGroups(input, pattern, options);
                result.Metadata["NamedGroups"] = namedGroups;
            }

            // Extract all groups if requested
            if (options.CustomOptions.TryGetValue("ExtractAllGroups", out var extractAll) && 
                extractAll is bool extractAllBool && extractAllBool)
            {
                var allGroups = ExtractAllGroups(input, pattern, options);
                result.Metadata["AllGroups"] = allGroups;
            }

            // Get match positions if requested
            if (options.CustomOptions.TryGetValue("IncludePositions", out var includePos) && 
                includePos is bool includePosBool && includePosBool)
            {
                var positions = GetMatchPositions(input, pattern, options);
                result.Metadata["MatchPositions"] = positions;
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Metadata["AdvancedProcessingError"] = ex.Message;
            return result;
        }
    }

    private Dictionary<string, List<string>> ExtractNamedGroups(string input, string pattern, ParseOptions options)
    {
        var namedGroups = new Dictionary<string, List<string>>();

        try
        {
            var regexOptions = BuildRegexOptions(options);
            var regex = new Regex(pattern, regexOptions);
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                foreach (string groupName in regex.GetGroupNames())
                {
                    if (groupName == "0") continue; // Skip the entire match group

                    var group = match.Groups[groupName];
                    if (group.Success)
                    {
                        if (!namedGroups.ContainsKey(groupName))
                        {
                            namedGroups[groupName] = new List<string>();
                        }
                        namedGroups[groupName].Add(group.Value);
                    }
                }
            }
        }
        catch
        {
            // Ignore errors in advanced processing
        }

        return namedGroups;
    }

    private List<List<string>> ExtractAllGroups(string input, string pattern, ParseOptions options)
    {
        var allGroups = new List<List<string>>();

        try
        {
            var regexOptions = BuildRegexOptions(options);
            var regex = new Regex(pattern, regexOptions);
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                var matchGroups = new List<string>();
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    var group = match.Groups[i];
                    matchGroups.Add(group.Success ? group.Value : "");
                }
                allGroups.Add(matchGroups);
            }
        }
        catch
        {
            // Ignore errors in advanced processing
        }

        return allGroups;
    }

    private List<(int Start, int Length, string Value)> GetMatchPositions(string input, string pattern, ParseOptions options)
    {
        var positions = new List<(int Start, int Length, string Value)>();

        try
        {
            var regexOptions = BuildRegexOptions(options);
            var regex = new Regex(pattern, regexOptions);
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                positions.Add((match.Index, match.Length, match.Value));
            }
        }
        catch
        {
            // Ignore errors in advanced processing
        }

        return positions;
    }

    protected new RegexOptions BuildRegexOptions(ParseOptions options)
    {
        // Use the base implementation
        return base.BuildRegexOptions(options);
    }
}

/// <summary>
/// Regex pattern builder helper
/// </summary>
public static class RegexPatternBuilder
{
    /// <summary>
    /// Builds common regex patterns
    /// </summary>
    public static class Common
    {
        public const string Email = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
        public const string Url = @"https?://[^\s/$.?#].[^\s]*";
        public const string IPv4 = @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b";
        public const string Phone = @"\b\d{3}[-.]?\d{3}[-.]?\d{4}\b";
        public const string CreditCard = @"\b(?:\d{4}[-\s]?){3}\d{4}\b";
        public const string JsonValue = @"""([^""\\]|\\.)*""";
        public const string HtmlTag = @"<[^>]+>";
        public const string HexColor = @"#[A-Fa-f0-9]{6}|#[A-Fa-f0-9]{3}";
    }

    /// <summary>
    /// Escapes special regex characters
    /// </summary>
    public static string Escape(string input)
    {
        return Regex.Escape(input);
    }

    /// <summary>
    /// Creates a pattern to match between two strings
    /// </summary>
    public static string Between(string left, string right)
    {
        return $"{Regex.Escape(left)}(.*?){Regex.Escape(right)}";
    }

    /// <summary>
    /// Creates a pattern with named capture group
    /// </summary>
    public static string NamedGroup(string name, string pattern)
    {
        return $"(?<{name}>{pattern})";
    }
}
