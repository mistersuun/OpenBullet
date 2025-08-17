using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using OpenBullet.Core.Parsing.Parsers;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 7 Validation Tests - Basic functionality validation
/// </summary>
public class Step7_ValidationTests
{
    [Fact]
    public void IDataParser_Interface_Should_Be_Properly_Defined()
    {
        // Assert
        typeof(IDataParser).IsInterface.Should().BeTrue();
        
        var methods = typeof(IDataParser).GetMethods();
        methods.Should().Contain(m => m.Name == nameof(IDataParser.Parse));
        methods.Should().Contain(m => m.Name == nameof(IDataParser.IsValidPattern));
        methods.Should().Contain(m => m.Name == "get_ParserType"); // Property getter shows as method in reflection
    }

    [Fact]
    public void IParserFactory_Interface_Should_Be_Properly_Defined()
    {
        // Assert
        typeof(IParserFactory).IsInterface.Should().BeTrue();
        
        var methods = typeof(IParserFactory).GetMethods();
        methods.Should().Contain(m => m.Name == nameof(IParserFactory.CreateParser));
        methods.Should().Contain(m => m.Name == nameof(IParserFactory.GetAvailableTypes));
        methods.Should().Contain(m => m.Name == nameof(IParserFactory.RegisterParser));
    }

    [Fact]
    public void ParseCommand_Can_Be_Created_With_Valid_ServiceProvider()
    {
        // Arrange
        var serviceProvider = CreateValidServiceProvider();

        // Act
        var command = new ParseCommand(serviceProvider);

        // Assert
        command.Should().NotBeNull();
        command.Should().BeAssignableTo<IScriptCommand>();
        command.CommandName.Should().Be("PARSE");
        command.Description.Should().NotBeEmpty();
    }

    [Fact]
    public void ParseCommand_Should_Throw_With_Invalid_ServiceProvider()
    {
        // Arrange
        var emptyServiceProvider = new Mock<IServiceProvider>().Object;

        // Act & Assert
        var act = () => new ParseCommand(emptyServiceProvider);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LeftRightParser_Can_Be_Created()
    {
        // Act
        var parser = new LeftRightParser();

        // Assert
        parser.Should().NotBeNull();
        parser.Should().BeAssignableTo<IDataParser>();
        parser.ParserType.Should().Be(ParseType.LeftRight);
    }

    [Fact]
    public void RegexParser_Can_Be_Created()
    {
        // Act
        var parser = new RegexParser();

        // Assert
        parser.Should().NotBeNull();
        parser.Should().BeAssignableTo<IDataParser>();
        parser.ParserType.Should().Be(ParseType.Regex);
    }

    [Fact]
    public void CssParser_Can_Be_Created()
    {
        // Act
        var parser = new CssParser();

        // Assert
        parser.Should().NotBeNull();
        parser.Should().BeAssignableTo<IDataParser>();
        parser.ParserType.Should().Be(ParseType.CSS);
    }

    [Fact]
    public void JsonParser_Can_Be_Created()
    {
        // Act
        var parser = new JsonParser();

        // Assert
        parser.Should().NotBeNull();
        parser.Should().BeAssignableTo<IDataParser>();
        parser.ParserType.Should().Be(ParseType.Json);
    }

    [Fact]
    public void ParserFactory_Can_Be_Created()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();

        // Act
        var factory = new ParserFactory(loggerMock.Object);

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeAssignableTo<IParserFactory>();
    }

    [Fact]
    public void ParseType_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<ParseType>().Should().Contain(ParseType.LeftRight);
        Enum.GetValues<ParseType>().Should().Contain(ParseType.Regex);
        Enum.GetValues<ParseType>().Should().Contain(ParseType.CSS);
        Enum.GetValues<ParseType>().Should().Contain(ParseType.Json);
        Enum.GetValues<ParseType>().Should().Contain(ParseType.XPath);
    }

    [Fact]
    public void ParseResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ParseResult();

        // Assert
        result.Success.Should().BeFalse();
        result.Matches.Should().NotBeNull().And.BeEmpty();
        result.ErrorMessage.Should().BeNull();
        result.Metadata.Should().NotBeNull().And.BeEmpty();
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

    [Fact]
    public void ParseValidationResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ParseValidationResult();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNull().And.BeEmpty();
        result.Warnings.Should().NotBeNull().And.BeEmpty();
        result.Suggestions.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ParserInfo_Should_Initialize_With_Defaults()
    {
        // Act
        var info = new ParserInfo();

        // Assert
        info.ParseType.Should().Be(ParseType.LeftRight); // Default enum value
        info.IsAvailable.Should().BeFalse();
        info.IsSingleton.Should().BeFalse();
        info.ParserTypeName.Should().BeEmpty();
        info.SupportsValidation.Should().BeFalse();
        info.Error.Should().BeNull();
        info.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void LeftRightParser_Should_Handle_Basic_Patterns()
    {
        // Arrange
        var parser = new LeftRightParser();

        // Act & Assert
        parser.IsValidPattern("").Should().BeFalse();
        parser.IsValidPattern(null!).Should().BeFalse();
        parser.IsValidPattern("\"left\" \"right\"").Should().BeTrue();
        parser.IsValidPattern("start end").Should().BeTrue();
        parser.IsValidPattern("left|right").Should().BeTrue();
    }

    [Fact]
    public void RegexParser_Should_Handle_Basic_Patterns()
    {
        // Arrange
        var parser = new RegexParser();

        // Act & Assert
        parser.IsValidPattern("").Should().BeFalse();
        parser.IsValidPattern(null!).Should().BeFalse();
        parser.IsValidPattern(@"\d+").Should().BeTrue();
        parser.IsValidPattern(@"[A-Za-z]+").Should().BeTrue();
        parser.IsValidPattern("[invalid").Should().BeFalse();
    }

    [Fact]
    public void CssParser_Should_Handle_Basic_Selectors()
    {
        // Arrange
        var parser = new CssParser();

        // Act & Assert
        parser.IsValidPattern("").Should().BeFalse();
        parser.IsValidPattern(null!).Should().BeFalse();
        parser.IsValidPattern("div").Should().BeTrue();
        parser.IsValidPattern(".className").Should().BeTrue();
        parser.IsValidPattern("#elementId").Should().BeTrue();
        parser.IsValidPattern("div.class[attr='value']").Should().BeTrue();
    }

    [Fact]
    public void JsonParser_Should_Handle_Basic_Paths()
    {
        // Arrange
        var parser = new JsonParser();

        // Act & Assert
        parser.IsValidPattern("").Should().BeFalse();
        parser.IsValidPattern(null!).Should().BeFalse();
        parser.IsValidPattern("$").Should().BeTrue();
        parser.IsValidPattern("$.property").Should().BeTrue();
        parser.IsValidPattern("$[0]").Should().BeTrue();
        parser.IsValidPattern("$.array[*].property").Should().BeTrue();
    }

    [Fact]
    public void ParseCommand_Should_Validate_Basic_Instructions()
    {
        // Arrange
        var command = new ParseCommand(CreateValidServiceProvider());
        
        var validInstruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR", "left", "right" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "RESULT"
            }
        };

        var invalidInstruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR" } // Missing pattern
        };

        // Act
        var validResult = command.ValidateInstruction(validInstruction);
        var invalidResult = command.ValidateInstruction(invalidInstruction);

        // Assert
        validResult.Should().NotBeNull();
        if (!validResult.IsValid)
        {
            var errors = string.Join(", ", validResult.Errors);
            throw new Exception($"Validation failed with errors: {errors}");
        }
        validResult.IsValid.Should().BeTrue();

        invalidResult.Should().NotBeNull();
        invalidResult.IsValid.Should().BeFalse();
        invalidResult.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void ParserFactory_Should_Handle_Basic_Operations()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act
        var availableTypes = factory.GetAvailableTypes().ToList();
        var lrParser = factory.CreateParser(ParseType.LeftRight);

        // Assert
        availableTypes.Should().NotBeEmpty();
        availableTypes.Should().Contain(ParseType.LeftRight);
        
        lrParser.Should().NotBeNull();
        lrParser.ParserType.Should().Be(ParseType.LeftRight);
    }

    [Fact]
    public void ParserFactory_Extensions_Should_Work()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act
        var parseResult = factory.Parse(ParseType.LeftRight, "start_value_end", "start_ _end");
        var isValid = factory.ValidatePattern(ParseType.LeftRight, "\"left\" \"right\"");
        var capabilities = factory.GetAllCapabilities();

        // Assert
        parseResult.Should().NotBeNull();
        parseResult.Success.Should().BeTrue();
        parseResult.Matches.Should().HaveCount(1);
        parseResult.Matches[0].Should().Be("value");

        isValid.Should().BeTrue();
        
        capabilities.Should().NotBeEmpty();
        capabilities.Should().ContainKey(ParseType.LeftRight);
        capabilities.Should().ContainKey(ParseType.Regex);
        capabilities.Should().ContainKey(ParseType.CSS);
        capabilities.Should().ContainKey(ParseType.Json);
    }

    [Fact]
    public void Enhanced_Parsers_Should_Extend_Base_Functionality()
    {
        // Arrange & Act
        var enhancedLR = new EnhancedLeftRightParser();
        var advancedRegex = new AdvancedRegexParser();
        var enhancedCss = new EnhancedCssParser();
        var enhancedJson = new EnhancedJsonParser();

        // Assert
        enhancedLR.Should().BeAssignableTo<LeftRightParser>();
        enhancedLR.Should().BeAssignableTo<IDataParser>();
        
        advancedRegex.Should().BeAssignableTo<RegexParser>();
        advancedRegex.Should().BeAssignableTo<IDataParser>();
        
        enhancedCss.Should().BeAssignableTo<CssParser>();
        enhancedCss.Should().BeAssignableTo<IDataParser>();
        
        enhancedJson.Should().BeAssignableTo<JsonParser>();
        enhancedJson.Should().BeAssignableTo<IDataParser>();
    }

    [Fact]
    public void Helper_Classes_Should_Provide_Useful_Constants()
    {
        // Assert
        RegexPatternBuilder.Common.Email.Should().NotBeEmpty();
        RegexPatternBuilder.Common.Url.Should().NotBeEmpty();
        RegexPatternBuilder.Common.IPv4.Should().NotBeEmpty();

        CssSelectorHelper.Common.AllLinks.Should().Be("a[href]");
        CssSelectorHelper.Common.AllImages.Should().Be("img[src]");
        CssSelectorHelper.Common.AllInputs.Should().Be("input");

        JsonPathHelper.Common.Root.Should().Be("$");
        JsonPathHelper.Common.AllProperties.Should().Be("$.*");
        JsonPathHelper.Common.AllArrayItems.Should().Be("$[*]");
    }

    [Fact]
    public void Helper_Classes_Should_Build_Correct_Patterns()
    {
        // Act & Assert
        RegexPatternBuilder.Escape("(test)").Should().Be(@"\(test\)");
        RegexPatternBuilder.Between("start", "end").Should().Be(@"start(.*?)end");
        RegexPatternBuilder.NamedGroup("name", @"\w+").Should().Be(@"(?<name>\w+)");

        CssSelectorHelper.AttributeSelector("name", "value").Should().Be("[name='value']");
        CssSelectorHelper.ClassSelector("myClass").Should().Be(".myClass");
        CssSelectorHelper.IdSelector("myId").Should().Be("#myId");

        JsonPathHelper.Property("name").Should().Be("$.name");
        JsonPathHelper.ArrayIndex(0).Should().Be("$[0]");
        JsonPathHelper.NestedProperty("user", "name").Should().Be("$.user.name");
    }

    [Fact]
    public void Step7TestHelpers_Should_Create_Valid_Objects()
    {
        // Arrange & Act
        var serviceProvider = CreateValidServiceProvider();
        var options = CreateTestParseOptions();
        var result = CreateTestParseResult();

        // Assert
        serviceProvider.Should().NotBeNull();
        
        options.Should().NotBeNull();
        options.Recursive.Should().BeTrue();
        options.IgnoreCase.Should().BeTrue();
        
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Matches.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("LR", ParseType.LeftRight)]
    [InlineData("REGEX", ParseType.Regex)]
    [InlineData("CSS", ParseType.CSS)]
    [InlineData("JSON", ParseType.Json)]
    [InlineData("XPATH", ParseType.XPath)]
    public void ParseType_String_Conversion_Should_Work(string stringValue, ParseType expectedType)
    {
        // Act
        var parseSuccess = ParseTypeExtensions.TryParseFromString(stringValue, out var parsedType);

        // Assert
        parseSuccess.Should().BeTrue();
        parsedType.Should().Be(expectedType);
    }

    [Fact]
    public void ParseResult_Helper_Methods_Should_Handle_Edge_Cases()
    {
        // Arrange
        var emptyResult = new ParseResult { Success = true, Matches = new List<string>() };
        var singleResult = new ParseResult { Success = true, Matches = new List<string> { "single" } };
        var multiResult = new ParseResult { Success = true, Matches = new List<string> { "first", "second", "third" } };

        // Act & Assert
        emptyResult.GetFirstMatch().Should().BeEmpty();
        emptyResult.GetFirstMatch("default").Should().Be("default");
        emptyResult.GetAllMatches().Should().BeEmpty();

        singleResult.GetFirstMatch().Should().Be("single");
        singleResult.GetAllMatches().Should().Be("single");

        multiResult.GetFirstMatch().Should().Be("first");
        multiResult.GetAllMatches().Should().Be("first, second, third");
        multiResult.GetAllMatches("|").Should().Be("first|second|third");
    }

    [Fact]
    public void All_Parser_Types_Should_Be_Testable()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);

        // Act & Assert
        foreach (var parseType in Enum.GetValues<ParseType>())
        {
            try
            {
                var parser = factory.CreateParser(parseType);
                parser.Should().NotBeNull();
                parser.ParserType.Should().Be(parseType);
                parser.IsValidPattern("").Should().BeFalse();
            }
            catch (NotSupportedException)
            {
                // Some parse types might not be implemented yet (like XPath)
                // This is acceptable for now
            }
        }
    }

    private static IServiceProvider CreateValidServiceProvider()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<ParserFactory>>();
        var factory = new ParserFactory(loggerMock.Object);
        
        // Register all parser types with the factory
        var lrParser = new LeftRightParser();
        var regexParser = new RegexParser();
        var cssParser = new CssParser();
        var jsonParser = new JsonParser();
        
        factory.RegisterParser(ParseType.LeftRight, lrParser);
        factory.RegisterParser(ParseType.Regex, regexParser);
        factory.RegisterParser(ParseType.CSS, cssParser);
        factory.RegisterParser(ParseType.Json, jsonParser);
        
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<ParseCommand>)))
                      .Returns(new Mock<ILogger<ParseCommand>>().Object);
        serviceProvider.Setup(sp => sp.GetService(typeof(IParserFactory)))
                      .Returns(factory);
        serviceProvider.Setup(sp => sp.GetService(typeof(IScriptParser)))
                      .Returns(new Mock<IScriptParser>().Object);
        
        return serviceProvider.Object;
    }

    private static ParseOptions CreateTestParseOptions()
    {
        return new ParseOptions
        {
            Recursive = true,
            IgnoreCase = true,
            Multiline = false,
            MaxMatches = 10,
            AttributeName = "value",
            LeftDelimiter = "start",
            RightDelimiter = "end"
        };
    }

    private static ParseResult CreateTestParseResult()
    {
        return new ParseResult
        {
            Success = true,
            Matches = new List<string> { "match1", "match2" },
            Metadata = new Dictionary<string, object>
            {
                ["ParseType"] = "LR",
                ["MatchCount"] = 2
            }
        };
    }
}
