using System.Text;

namespace OpenBullet.Core.Parsing.Parsers;

/// <summary>
/// Left-Right parser for simple string extraction between delimiters
/// </summary>
public class LeftRightParser : IDataParser
{
    public ParseType ParserType => ParseType.LeftRight;

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

            // Extract left and right delimiters from pattern or options
            var (leftDelim, rightDelim) = ExtractDelimiters(pattern, options);

            if (string.IsNullOrEmpty(leftDelim) || string.IsNullOrEmpty(rightDelim))
            {
                result.ErrorMessage = "Both left and right delimiters are required for LR parsing";
                return result;
            }

            // Perform extraction
            var matches = ExtractBetweenDelimiters(input, leftDelim, rightDelim, options);
            result.Matches = matches;
            result.Success = matches.Count > 0;

            // Add metadata
            result.Metadata["LeftDelimiter"] = leftDelim;
            result.Metadata["RightDelimiter"] = rightDelim;
            result.Metadata["MatchCount"] = matches.Count;

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"LR parsing error: {ex.Message}";
            return result;
        }
    }

    public bool IsValidPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        // Pattern should contain both left and right delimiters
        var parts = ParsePatternParts(pattern);
        return parts.leftDelim != null && parts.rightDelim != null;
    }

    private (string? leftDelim, string? rightDelim) ExtractDelimiters(string pattern, ParseOptions options)
    {
        // First try options
        if (!string.IsNullOrEmpty(options.LeftDelimiter) && !string.IsNullOrEmpty(options.RightDelimiter))
        {
            return (options.LeftDelimiter, options.RightDelimiter);
        }

        // Parse from pattern
        return ParsePatternParts(pattern);
    }

    private (string? leftDelim, string? rightDelim) ParsePatternParts(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return (null, null);

        // Try to parse different pattern formats:
        // 1. "leftDelim" "rightDelim" (quoted strings)
        // 2. leftDelim|rightDelim (pipe separated)
        // 3. leftDelim rightDelim (space separated)

        // Handle quoted strings
        var quotedMatches = System.Text.RegularExpressions.Regex.Matches(pattern, @"""([^""]*?)""");
        if (quotedMatches.Count >= 2)
        {
            return (quotedMatches[0].Groups[1].Value, quotedMatches[1].Groups[1].Value);
        }

        // Handle pipe separated
        if (pattern.Contains('|'))
        {
            var parts = pattern.Split('|', 2);
            if (parts.Length == 2)
            {
                return (parts[0].Trim(), parts[1].Trim());
            }
        }

        // Handle space separated (simple case)
        var spaceParts = pattern.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (spaceParts.Length == 2)
        {
            return (spaceParts[0], spaceParts[1]);
        }

        return (null, null);
    }

    private List<string> ExtractBetweenDelimiters(string input, string leftDelim, string rightDelim, ParseOptions options)
    {
        var matches = new List<string>();
        var comparison = options.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var searchInput = input;
        var startPos = 0;

        while (startPos < searchInput.Length)
        {
            // Find left delimiter
            var leftIndex = searchInput.IndexOf(leftDelim, startPos, comparison);
            if (leftIndex == -1)
                break;

            // Find right delimiter after left delimiter
            var searchStart = leftIndex + leftDelim.Length;
            var rightIndex = searchInput.IndexOf(rightDelim, searchStart, comparison);
            if (rightIndex == -1)
                break;

            // Extract content between delimiters
            var content = searchInput.Substring(searchStart, rightIndex - searchStart);
            matches.Add(content);

            // Check max matches limit
            if (options.MaxMatches > 0 && matches.Count >= options.MaxMatches)
                break;

            // Move past the right delimiter for next search
            if (options.Recursive)
            {
                startPos = leftIndex + leftDelim.Length; // Allow overlapping matches
            }
            else
            {
                startPos = rightIndex + rightDelim.Length; // Non-overlapping matches
            }
        }

        return matches;
    }
}

/// <summary>
/// Enhanced Left-Right parser with additional features
/// </summary>
public class EnhancedLeftRightParser : LeftRightParser
{
    public new ParseResult Parse(string input, string pattern, ParseOptions options)
    {
        var result = base.Parse(input, pattern, options);

        if (result.Success && result.Matches.Count > 0)
        {
            // Apply post-processing filters
            result.Matches = ApplyFilters(result.Matches, options);
            result.Metadata["FilteredMatchCount"] = result.Matches.Count;
        }

        return result;
    }

    private List<string> ApplyFilters(List<string> matches, ParseOptions options)
    {
        var filtered = new List<string>(matches);

        // Remove empty matches if specified
        if (options.CustomOptions.TryGetValue("RemoveEmpty", out var removeEmpty) && 
            removeEmpty is bool removeEmptyBool && removeEmptyBool)
        {
            filtered = filtered.Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
        }

        // Trim whitespace if specified
        if (options.CustomOptions.TryGetValue("TrimWhitespace", out var trimWhitespace) && 
            trimWhitespace is bool trimWhitespaceBool && trimWhitespaceBool)
        {
            filtered = filtered.Select(m => m.Trim()).ToList();
        }

        // Apply regex filter if specified
        if (options.CustomOptions.TryGetValue("RegexFilter", out var regexFilter) && 
            regexFilter is string regexPattern && !string.IsNullOrEmpty(regexPattern))
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(regexPattern);
                filtered = filtered.Where(m => regex.IsMatch(m)).ToList();
            }
            catch
            {
                // Ignore invalid regex patterns
            }
        }

        // Apply length filter if specified
        if (options.CustomOptions.TryGetValue("MinLength", out var minLength) && 
            minLength is int minLengthInt && minLengthInt > 0)
        {
            filtered = filtered.Where(m => m.Length >= minLengthInt).ToList();
        }

        if (options.CustomOptions.TryGetValue("MaxLength", out var maxLength) && 
            maxLength is int maxLengthInt && maxLengthInt > 0)
        {
            filtered = filtered.Where(m => m.Length <= maxLengthInt).ToList();
        }

        return filtered;
    }
}
