using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Parsing;
using OpenBullet.Core.Parsing.Parsers;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 7 Tests: Data Parsers
/// </summary>
public class Step7_ParsersTests : IDisposable
{
    [Fact]
    public void LeftRightParser_Should_Parse_Between_Delimiters()
    {
        // Arrange
        var parser = new LeftRightParser();
        var input = "<html><title>Test Page</title><body>Content</body></html>";
        var pattern = "<title> </title>";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(1);
        result.Matches[0].Should().Be("Test Page");
        result.Metadata.Should().ContainKey("LeftDelimiter");
        result.Metadata.Should().ContainKey("RightDelimiter");
    }

    [Fact]
    public void LeftRightParser_Should_Handle_Multiple_Matches()
    {
        // Arrange
        var parser = new LeftRightParser();
        var input = "<div>First</div><div>Second</div><div>Third</div>";
        var pattern = "<div> </div>";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(3);
        result.Matches[0].Should().Be("First");
        result.Matches[1].Should().Be("Second");
        result.Matches[2].Should().Be("Third");
    }

    [Fact]
    public void LeftRightParser_Should_Handle_Quoted_Delimiters()
    {
        // Arrange
        var parser = new LeftRightParser();
        var input = "start_value_here_end and start_another_value_end";
        var pattern = "\"start_\" \"_end\"";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(2);
        result.Matches[0].Should().Be("value_here");
        result.Matches[1].Should().Be("another_value");
    }

    [Fact]
    public void LeftRightParser_Should_Handle_Max_Matches()
    {
        // Arrange
        var parser = new LeftRightParser();
        var input = "<item>1</item><item>2</item><item>3</item><item>4</item>";
        var pattern = "<item> </item>";
        var options = new ParseOptions { MaxMatches = 2 };

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(2);
        result.Matches[0].Should().Be("1");
        result.Matches[1].Should().Be("2");
    }

    [Fact]
    public void LeftRightParser_Should_Handle_Case_Insensitive()
    {
        // Arrange
        var parser = new LeftRightParser();
        var input = "<TITLE>Test Page</TITLE>";
        var pattern = "<title> </title>";
        var options = new ParseOptions { IgnoreCase = true };

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(1);
        result.Matches[0].Should().Be("Test Page");
    }

    [Fact]
    public void RegexParser_Should_Parse_With_Regex_Pattern()
    {
        // Arrange
        var parser = new RegexParser();
        var input = "Contact us at test@example.com or admin@test.org for help";
        var pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(2);
        result.Matches[0].Should().Be("test@example.com");
        result.Matches[1].Should().Be("admin@test.org");
    }

    [Fact]
    public void RegexParser_Should_Extract_Capture_Groups()
    {
        // Arrange
        var parser = new RegexParser();
        var input = "Name: John Doe, Age: 30, City: Boston";
        var pattern = @"Name: ([^,]+), Age: (\d+), City: ([^,\s]+)";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(1);
        result.Matches[0].Should().Be("John Doe"); // First capturing group
    }

    [Fact]
    public void RegexParser_Should_Handle_Case_Insensitive()
    {
        // Arrange
        var parser = new RegexParser();
        var input = "HELLO world TEST";
        var pattern = @"hello|test";
        var options = new ParseOptions { IgnoreCase = true };

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(2);
        result.Matches[0].Should().Be("HELLO");
        result.Matches[1].Should().Be("TEST");
    }

    [Fact]
    public void RegexParser_Should_Handle_Invalid_Pattern()
    {
        // Arrange
        var parser = new RegexParser();
        var input = "test string";
        var pattern = "[invalid"; // Invalid regex
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid regex pattern");
    }

    [Fact]
    public void CssParser_Should_Parse_HTML_With_CSS_Selector()
    {
        // Arrange
        var parser = new CssParser();
        var input = "<html><head><title>Test Page</title></head><body><p class='content'>Hello World</p></body></html>";
        var pattern = "title";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(1);
        result.Matches[0].Should().Be("Test Page");
    }

    [Fact]
    public void CssParser_Should_Extract_Attributes()
    {
        // Arrange
        var parser = new CssParser();
        var input = "<html><body><input type='text' name='username' value='john' /><input type='password' name='password' /></body></html>";
        var pattern = "input[type='text']";
        var options = new ParseOptions { AttributeName = "value" };

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(1);
        result.Matches[0].Should().Be("john");
    }

    [Fact]
    public void CssParser_Should_Handle_Complex_Selectors()
    {
        // Arrange
        var parser = new CssParser();
        var input = "<table><tr><td>Cell1</td><td>Cell2</td></tr><tr><td>Cell3</td><td>Cell4</td></tr></table>";
        var pattern = "table tr:nth-child(2) td";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(2);
        result.Matches[0].Should().Be("Cell3");
        result.Matches[1].Should().Be("Cell4");
    }

    [Fact]
    public void CssParser_Should_Handle_Invalid_HTML()
    {
        // Arrange
        var parser = new CssParser();
        var input = "not html content";
        var pattern = "title";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().BeEmpty(); // No matches in non-HTML content
    }

    [Fact]
    public void JsonParser_Should_Parse_JSON_With_Path()
    {
        // Arrange
        var parser = new JsonParser();
        var input = @"{""user"":{""name"":""John"",""age"":30,""emails"":[""john@test.com"",""j.doe@example.org""]}}";
        var pattern = "$.user.name";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(1);
        result.Matches[0].Should().Be("John");
    }

    [Fact]
    public void JsonParser_Should_Parse_Array_Elements()
    {
        // Arrange
        var parser = new JsonParser();
        var input = @"{""items"":[""apple"",""banana"",""cherry""]}";
        var pattern = "$.items[*]";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(3);
        result.Matches[0].Should().Be("apple");
        result.Matches[1].Should().Be("banana");
        result.Matches[2].Should().Be("cherry");
    }

    [Fact]
    public void JsonParser_Should_Handle_Nested_Objects()
    {
        // Arrange
        var parser = new JsonParser();
        var input = @"{""data"":{""users"":[{""name"":""Alice"",""id"":1},{""name"":""Bob"",""id"":2}]}}";
        var pattern = "$.data.users[*].name";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(2);
        result.Matches[0].Should().Be("Alice");
        result.Matches[1].Should().Be("Bob");
    }

    [Fact]
    public void JsonParser_Should_Handle_Invalid_JSON()
    {
        // Arrange
        var parser = new JsonParser();
        var input = "{ invalid json }";
        var pattern = "$.test";
        var options = new ParseOptions();

        // Act
        var result = parser.Parse(input, pattern, options);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid JSON");
    }

    [Fact]
    public void ParserFactory_Should_Create_Parsers_For_All_Types()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act & Assert
        var lrParser = factory.CreateParser(ParseType.LeftRight);
        lrParser.Should().NotBeNull();
        lrParser.ParserType.Should().Be(ParseType.LeftRight);

        var regexParser = factory.CreateParser(ParseType.Regex);
        regexParser.Should().NotBeNull();
        regexParser.ParserType.Should().Be(ParseType.Regex);

        var cssParser = factory.CreateParser(ParseType.CSS);
        cssParser.Should().NotBeNull();
        cssParser.ParserType.Should().Be(ParseType.CSS);

        var jsonParser = factory.CreateParser(ParseType.Json);
        jsonParser.Should().NotBeNull();
        jsonParser.ParserType.Should().Be(ParseType.Json);
    }

    [Fact]
    public void ParserFactory_Should_Return_Available_Types()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act
        var availableTypes = factory.GetAvailableTypes();

        // Assert
        availableTypes.Should().NotBeEmpty();
        availableTypes.Should().Contain(ParseType.LeftRight);
        availableTypes.Should().Contain(ParseType.Regex);
        availableTypes.Should().Contain(ParseType.CSS);
        availableTypes.Should().Contain(ParseType.Json);
    }

    [Fact]
    public void ParserFactory_Should_Register_Custom_Parser()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);
        var customParser = new Mock<IDataParser>();
        customParser.Setup(p => p.ParserType).Returns(ParseType.XPath);

        // Act
        factory.RegisterParser(ParseType.XPath, customParser.Object);

        // Assert
        var availableTypes = factory.GetAvailableTypes();
        availableTypes.Should().Contain(ParseType.XPath);
        
        var createdParser = factory.CreateParser(ParseType.XPath);
        createdParser.Should().BeSameAs(customParser.Object);
    }

    [Fact]
    public void ParserFactory_Should_Validate_Patterns()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act & Assert
        factory.ValidatePattern(ParseType.LeftRight, "\"left\" \"right\"").Should().BeTrue();
        factory.ValidatePattern(ParseType.LeftRight, "").Should().BeFalse();
        
        factory.ValidatePattern(ParseType.Regex, @"\d+").Should().BeTrue();
        factory.ValidatePattern(ParseType.Regex, "[invalid").Should().BeFalse();
        
        factory.ValidatePattern(ParseType.CSS, "div.class").Should().BeTrue();
        factory.ValidatePattern(ParseType.CSS, "").Should().BeFalse();
        
        factory.ValidatePattern(ParseType.Json, "$.property").Should().BeTrue();
        factory.ValidatePattern(ParseType.Json, "").Should().BeFalse();
    }

    [Fact]
    public void ParserFactory_Should_Get_Parser_Info()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act
        var info = factory.GetParserInfo(ParseType.LeftRight);

        // Assert
        info.Should().NotBeNull();
        info.ParseType.Should().Be(ParseType.LeftRight);
        info.IsAvailable.Should().BeTrue();
        info.ParserTypeName.Should().NotBeEmpty();
        info.SupportsValidation.Should().BeTrue();
    }

    [Fact]
    public void ParserFactory_Should_Get_Suggested_Patterns()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act
        var lrSuggestions = factory.GetSuggestedPatterns(ParseType.LeftRight);
        var regexSuggestions = factory.GetSuggestedPatterns(ParseType.Regex);
        var cssSuggestions = factory.GetSuggestedPatterns(ParseType.CSS);
        var jsonSuggestions = factory.GetSuggestedPatterns(ParseType.Json);

        // Assert
        lrSuggestions.Should().NotBeEmpty();
        lrSuggestions.Should().Contain(s => s.Contains("\"left\" \"right\""));
        
        regexSuggestions.Should().NotBeEmpty();
        regexSuggestions.Should().Contain(s => s.Contains(@"\d+"));
        
        cssSuggestions.Should().NotBeEmpty();
        cssSuggestions.Should().Contain(s => s.Contains("div.className"));
        
        jsonSuggestions.Should().NotBeEmpty();
        jsonSuggestions.Should().Contain(s => s.Contains("$.propertyName"));
    }

    [Theory]
    [InlineData(ParseType.LeftRight)]
    [InlineData(ParseType.Regex)]
    [InlineData(ParseType.CSS)]
    [InlineData(ParseType.Json)]
    public void All_Parsers_Should_Implement_IsValidPattern(ParseType parseType)
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);
        var parser = factory.CreateParser(parseType);

        // Act & Assert
        parser.IsValidPattern("").Should().BeFalse();
        parser.IsValidPattern(null!).Should().BeFalse();
    }

    [Fact]
    public void ParseResult_Should_Have_Helper_Methods()
    {
        // Arrange
        var result = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "first", "second", "third" }
        };

        // Act & Assert
        result.GetFirstMatch().Should().Be("first");
        result.GetFirstMatch("default").Should().Be("first");
        result.GetAllMatches().Should().Be("first, second, third");
        result.GetAllMatches("|").Should().Be("first|second|third");

        // Test with empty results
        var emptyResult = new ParseResult { Success = true, Matches = new List<string>() };
        emptyResult.GetFirstMatch().Should().BeEmpty();
        emptyResult.GetFirstMatch("default").Should().Be("default");
        emptyResult.GetAllMatches().Should().BeEmpty();
    }

    [Fact]
    public void ParseOptions_Should_Initialize_With_Defaults()
    {
        // Act
        var options = new ParseOptions();

        // Assert
        options.Recursive.Should().BeFalse();
        options.IgnoreCase.Should().BeFalse();
        options.Multiline.Should().BeFalse();
        options.MaxMatches.Should().Be(0);
        options.AttributeName.Should().BeNull();
        options.JsonPath.Should().BeNull();
        options.LeftDelimiter.Should().BeNull();
        options.RightDelimiter.Should().BeNull();
        options.CustomOptions.Should().NotBeNull().And.BeEmpty();
    }

    public void Dispose()
    {
        // No resources to dispose in this test class
    }
}
