using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace OpenBullet.Core.Parsing.Parsers;

/// <summary>
/// CSS selector parser for HTML/XML document parsing
/// </summary>
public class CssParser : IDataParser
{
    public ParseType ParserType => ParseType.CSS;

    public ParseResult Parse(string input, string pattern, ParseOptions options)
    {
        var result = new ParseResult();

        try
        {
            if (string.IsNullOrEmpty(input))
            {
                result.ErrorMessage = "Input HTML is empty";
                return result;
            }

            if (string.IsNullOrEmpty(pattern))
            {
                result.ErrorMessage = "CSS selector pattern is empty";
                return result;
            }

            // Parse HTML document
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = context.OpenAsync(req => req.Content(input)).Result;

            // Execute CSS selector
            var elements = document.QuerySelectorAll(pattern);

            // Extract values based on options
            var matches = ExtractFromElements(elements, options);
            result.Matches = matches;
            result.Success = true; // Success means parsing completed without errors, not that matches were found

            // Add metadata
            result.Metadata["Selector"] = pattern;
            result.Metadata["ElementCount"] = elements.Length;
            result.Metadata["AttributeName"] = options.AttributeName ?? "text";

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"CSS parsing error: {ex.Message}";
            return result;
        }
    }

    public bool IsValidPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        try
        {
            // Try to parse with a dummy document to validate selector
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = context.OpenAsync(req => req.Content("<html></html>")).Result;
            
            document.QuerySelectorAll(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private List<string> ExtractFromElements(IHtmlCollection<IElement> elements, ParseOptions options)
    {
        var matches = new List<string>();
        var maxMatches = options.MaxMatches > 0 ? options.MaxMatches : int.MaxValue;
        var attributeName = options.AttributeName?.ToLowerInvariant();

        foreach (var element in elements.Take(maxMatches))
        {
            string? value = null;

            // Determine what to extract
            if (string.IsNullOrEmpty(attributeName) || attributeName == "text")
            {
                // Extract text content
                value = element.TextContent;
            }
            else if (attributeName == "html" || attributeName == "innerhtml")
            {
                // Extract inner HTML
                value = element.InnerHtml;
            }
            else if (attributeName == "outerhtml")
            {
                // Extract outer HTML
                value = element.OuterHtml;
            }
            else
            {
                // Extract specific attribute
                value = element.GetAttribute(attributeName);
            }

            if (!string.IsNullOrEmpty(value))
            {
                matches.Add(value);
            }
            else if (options.CustomOptions.TryGetValue("IncludeEmpty", out var includeEmpty) && 
                     includeEmpty is bool includeEmptyBool && includeEmptyBool)
            {
                matches.Add(string.Empty);
            }
        }

        return matches;
    }
}

/// <summary>
/// Enhanced CSS parser with additional features
/// </summary>
public class EnhancedCssParser : CssParser
{
    public new ParseResult Parse(string input, string pattern, ParseOptions options)
    {
        var result = base.Parse(input, pattern, options);

        if (result.Success)
        {
            // Apply post-processing
            result = ApplyEnhancements(result, input, pattern, options);
        }

        return result;
    }

    private ParseResult ApplyEnhancements(ParseResult result, string input, string pattern, ParseOptions options)
    {
        try
        {
            // Extract multiple attributes if requested
            if (options.CustomOptions.TryGetValue("MultipleAttributes", out var multiAttr) && 
                multiAttr is string[] attributes && attributes.Length > 0)
            {
                var multiResults = ExtractMultipleAttributes(input, pattern, attributes, options);
                result.Metadata["MultipleAttributes"] = multiResults;
            }

            // Get element information if requested
            if (options.CustomOptions.TryGetValue("IncludeElementInfo", out var includeInfo) && 
                includeInfo is bool includeInfoBool && includeInfoBool)
            {
                var elementInfo = GetElementInformation(input, pattern, options);
                result.Metadata["ElementInfo"] = elementInfo;
            }

            // Apply filtering
            if (options.CustomOptions.ContainsKey("FilterEmpty") || 
                options.CustomOptions.ContainsKey("FilterPattern"))
            {
                result.Matches = ApplyFiltering(result.Matches, options);
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Metadata["EnhancementError"] = ex.Message;
            return result;
        }
    }

    private Dictionary<string, List<string>> ExtractMultipleAttributes(string input, string pattern, string[] attributes, ParseOptions options)
    {
        var results = new Dictionary<string, List<string>>();

        try
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = context.OpenAsync(req => req.Content(input)).Result;
            var elements = document.QuerySelectorAll(pattern);

            foreach (var attr in attributes)
            {
                results[attr] = new List<string>();
                
                foreach (var element in elements)
                {
                    string? value = null;

                    if (attr.ToLowerInvariant() == "text")
                    {
                        value = element.TextContent;
                    }
                    else if (attr.ToLowerInvariant() == "html")
                    {
                        value = element.InnerHtml;
                    }
                    else
                    {
                        value = element.GetAttribute(attr);
                    }

                    results[attr].Add(value ?? string.Empty);
                }
            }
        }
        catch
        {
            // Ignore errors in enhancement processing
        }

        return results;
    }

    private List<ElementInfo> GetElementInformation(string input, string pattern, ParseOptions options)
    {
        var elementInfos = new List<ElementInfo>();

        try
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = context.OpenAsync(req => req.Content(input)).Result;
            var elements = document.QuerySelectorAll(pattern);

            foreach (var element in elements)
            {
                var info = new ElementInfo
                {
                    TagName = element.TagName,
                    Id = element.Id,
                    ClassName = element.ClassName,
                    AttributeCount = element.Attributes.Length,
                    ChildElementCount = element.Children.Length,
                    TextLength = element.TextContent.Length,
                    Path = GetElementPath(element)
                };

                elementInfos.Add(info);
            }
        }
        catch
        {
            // Ignore errors in enhancement processing
        }

        return elementInfos;
    }

    private string GetElementPath(IElement element)
    {
        var path = new List<string>();
        var current = element;

        while (current != null && current.TagName != "HTML")
        {
            var tagName = current.TagName.ToLower();
            var index = GetElementIndex(current);
            
            if (index > 0)
            {
                path.Insert(0, $"{tagName}[{index}]");
            }
            else
            {
                path.Insert(0, tagName);
            }

            current = current.ParentElement;
        }

        return string.Join(" > ", path);
    }

    private int GetElementIndex(IElement element)
    {
        if (element.ParentElement == null)
            return 0;

        var siblings = element.ParentElement.Children.Where(e => e.TagName == element.TagName);
        return siblings.ToList().IndexOf(element) + 1;
    }

    private List<string> ApplyFiltering(List<string> matches, ParseOptions options)
    {
        var filtered = new List<string>(matches);

        // Filter empty values
        if (options.CustomOptions.TryGetValue("FilterEmpty", out var filterEmpty) && 
            filterEmpty is bool filterEmptyBool && filterEmptyBool)
        {
            filtered = filtered.Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
        }

        // Apply regex filter
        if (options.CustomOptions.TryGetValue("FilterPattern", out var filterPattern) && 
            filterPattern is string pattern && !string.IsNullOrEmpty(pattern))
        {
            try
            {
                var regex = new System.Text.RegularExpressions.Regex(pattern);
                filtered = filtered.Where(m => regex.IsMatch(m)).ToList();
            }
            catch
            {
                // Ignore invalid regex patterns
            }
        }

        return filtered;
    }
}

/// <summary>
/// Element information for enhanced CSS parsing
/// </summary>
public class ElementInfo
{
    public string TagName { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int AttributeCount { get; set; }
    public int ChildElementCount { get; set; }
    public int TextLength { get; set; }
    public string Path { get; set; } = string.Empty;
}

/// <summary>
/// CSS selector helper utilities
/// </summary>
public static class CssSelectorHelper
{
    /// <summary>
    /// Common CSS selectors
    /// </summary>
    public static class Common
    {
        public const string AllLinks = "a[href]";
        public const string AllImages = "img[src]";
        public const string AllInputs = "input";
        public const string AllForms = "form";
        public const string AllTables = "table";
        public const string AllDivs = "div";
        public const string AllSpans = "span";
        public const string AllButtons = "button, input[type='button'], input[type='submit']";
        public const string MetaTags = "meta";
        public const string Scripts = "script[src]";
        public const string StyleSheets = "link[rel='stylesheet']";
    }

    /// <summary>
    /// Builds attribute selector
    /// </summary>
    public static string AttributeSelector(string attribute, string value = "")
    {
        return string.IsNullOrEmpty(value) ? $"[{attribute}]" : $"[{attribute}='{value}']";
    }

    /// <summary>
    /// Builds class selector
    /// </summary>
    public static string ClassSelector(string className)
    {
        return $".{className}";
    }

    /// <summary>
    /// Builds ID selector
    /// </summary>
    public static string IdSelector(string id)
    {
        return $"#{id}";
    }

    /// <summary>
    /// Builds nth-child selector
    /// </summary>
    public static string NthChild(int n)
    {
        return $":nth-child({n})";
    }

    /// <summary>
    /// Builds contains text selector (pseudo-selector)
    /// </summary>
    public static string ContainsText(string text)
    {
        return $":contains('{text}')";
    }
}
