using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 7 Tests: PARSE Command Implementation
/// </summary>
public class Step7_ParseCommandTests : IDisposable
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<ParseCommand>> _loggerMock;
    private readonly Mock<IParserFactory> _parserFactoryMock;
    private readonly Mock<IScriptParser> _scriptParserMock;
    private readonly ParseCommand _parseCommand;
    private readonly BotData _botData;

    public Step7_ParseCommandTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<ParseCommand>>();
        _parserFactoryMock = new Mock<IParserFactory>();
        _scriptParserMock = new Mock<IScriptParser>();

        // Setup service provider
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<ParseCommand>)))
                           .Returns(_loggerMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IParserFactory)))
                           .Returns(_parserFactoryMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IScriptParser)))
                           .Returns(_scriptParserMock.Object);

        // Setup parser factory mock to return available types
        _parserFactoryMock.Setup(pf => pf.GetAvailableTypes())
                         .Returns(new[] { ParseType.LeftRight, ParseType.Regex, ParseType.CSS, ParseType.Json });

        // Setup mock parsers for validation and execution
        var lrParserMock = new Mock<IDataParser>();
        lrParserMock.Setup(p => p.IsValidPattern(It.IsAny<string>())).Returns(true);
        lrParserMock.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ParseOptions>()))
                   .Returns(new ParseResult { Success = true, Matches = new List<string> { "test_result" } });
        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.LeftRight))
                         .Returns(lrParserMock.Object);

        var regexParserMock = new Mock<IDataParser>();
        regexParserMock.Setup(p => p.IsValidPattern(It.IsAny<string>())).Returns(true);
        regexParserMock.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ParseOptions>()))
                      .Returns(new ParseResult { Success = true, Matches = new List<string> { "regex_result" } });
        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.Regex))
                         .Returns(regexParserMock.Object);

        var cssParserMock = new Mock<IDataParser>();
        cssParserMock.Setup(p => p.IsValidPattern(It.IsAny<string>())).Returns(true);
        cssParserMock.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ParseOptions>()))
                    .Returns(new ParseResult { Success = true, Matches = new List<string> { "css_result" } });
        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.CSS))
                         .Returns(cssParserMock.Object);

        var jsonParserMock = new Mock<IDataParser>();
        jsonParserMock.Setup(p => p.IsValidPattern(It.IsAny<string>())).Returns(true);
        jsonParserMock.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ParseOptions>()))
                     .Returns(new ParseResult { Success = true, Matches = new List<string> { "json_result" } });
        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.Json))
                         .Returns(jsonParserMock.Object);

        _parseCommand = new ParseCommand(_serviceProviderMock.Object);

        // Create test bot data
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        var cancellationToken = new CancellationTokenSource().Token;
        _botData = new BotData("test:data", config, logger, cancellationToken);
        _botData.Source = "<html><title>Test Page</title><body>Hello World</body></html>";

        // Setup script parser default behavior
        _scriptParserMock.Setup(sp => sp.SubstituteVariables(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                        .Returns<string, Dictionary<string, object>>((input, _) => input);

        // Setup parser factory to return available types
        _parserFactoryMock.Setup(pf => pf.GetAvailableTypes())
                         .Returns(new[] { ParseType.LeftRight, ParseType.Regex, ParseType.CSS, ParseType.Json });
    }

    [Fact]
    public void ParseCommand_Should_Have_Correct_Properties()
    {
        // Assert
        _parseCommand.CommandName.Should().Be("PARSE");
        _parseCommand.Description.Should().NotBeEmpty();
        _parseCommand.Should().BeAssignableTo<IScriptCommand>();
    }

    [Fact]
    public async Task ExecuteAsync_With_LR_Parser_Should_Extract_Data()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR", "<title>", "</title>" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "TITLE"
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "Test Page" }
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.LeftRight))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), "<title>", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.Variables.Should().ContainKey("TITLE");
        _botData.Variables["TITLE"].Should().Be("Test Page");
        
        mockParser.Verify(p => p.Parse(It.IsAny<string>(), "<title>", It.IsAny<ParseOptions>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_CSS_Parser_Should_Extract_Data()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "CSS", "title", "text" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "CAP",
                ["RedirectorName"] = "PAGE_TITLE"
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "Test Page" }
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.CSS))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), "title", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.CapturedData.Should().ContainKey("PAGE_TITLE");
        _botData.CapturedData["PAGE_TITLE"].Should().Be("Test Page");
    }

    [Fact]
    public async Task ExecuteAsync_With_Regex_Parser_Should_Extract_Data()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "REGEX", @"\d+" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "NUMBERS"
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "123", "456", "789" }
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.Regex))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), @"\d+", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.Variables.Should().ContainKey("NUMBERS");
        _botData.Variables["NUMBERS"].Should().Be("123|456|789"); // Multiple results joined
        _botData.Variables.Should().ContainKey("NUMBERS_ALL");
    }

    [Fact]
    public async Task ExecuteAsync_With_JSON_Parser_Should_Extract_Data()
    {
        // Arrange
        _botData.Source = @"{""user"":{""name"":""John"",""age"":30}}";
        
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "JSON", "$.user.name" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "USERNAME"
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "John" }
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.Json))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), "$.user.name", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.Variables.Should().ContainKey("USERNAME");
        _botData.Variables["USERNAME"].Should().Be("John");
    }

    [Fact]
    public async Task ExecuteAsync_With_Boolean_Parameters_Should_Apply_Options()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "REGEX", @"\w+" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "WORDS",
                ["Recursive"] = true,
                ["IgnoreCase"] = true,
                ["Multiline"] = false
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "word1", "word2" }
        };

        ParseOptions capturedOptions = null!;
        
        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.Regex))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), @"\w+", It.IsAny<ParseOptions>()))
                  .Callback<string, string, ParseOptions>((_, _, options) => capturedOptions = options)
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        capturedOptions.Should().NotBeNull();
        capturedOptions.Recursive.Should().BeTrue();
        capturedOptions.IgnoreCase.Should().BeTrue();
        capturedOptions.Multiline.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_With_Variable_Substitution_Should_Replace_Variables()
    {
        // Arrange
        _botData.SetVariable("SELECTOR", "title");
        
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "CSS", "<SELECTOR>" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "RESULT"
            },
            LineNumber = 1
        };

        _scriptParserMock.Setup(sp => sp.SubstituteVariables("<SELECTOR>", It.IsAny<Dictionary<string, object>>()))
                        .Returns("title");

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "Test Page" }
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.CSS))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), "title", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _scriptParserMock.Verify(sp => sp.SubstituteVariables("<SELECTOR>", It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_Prefix_And_Suffix_Should_Apply_Formatting()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR", "<title>", "</title>" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "TITLE",
                ["RedirectorPrefix"] = "Page: ",
                ["RedirectorSuffix"] = " - Site"
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string> { "Test Page" }
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.LeftRight))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), "<title>", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.Variables.Should().ContainKey("TITLE");
        _botData.Variables["TITLE"].Should().Be("Page: Test Page - Site");
    }

    [Fact]
    public async Task ExecuteAsync_With_Parse_Failure_Should_Handle_Error()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "REGEX", @"[invalid" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "RESULT"
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = false,
            ErrorMessage = "Invalid regex pattern"
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.Regex))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), @"[invalid", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid regex pattern");
    }

    [Fact]
    public async Task ExecuteAsync_With_No_Matches_Should_Set_Empty_Variable()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR", "notfound", "notfound" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "RESULT"
            },
            LineNumber = 1
        };

        var mockParser = new Mock<IDataParser>();
        var parseResult = new ParseResult
        {
            Success = true,
            Matches = new List<string>() // No matches
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.LeftRight))
                         .Returns(mockParser.Object);
        mockParser.Setup(p => p.Parse(It.IsAny<string>(), "notfound", It.IsAny<ParseOptions>()))
                  .Returns(parseResult);

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _botData.Variables.Should().ContainKey("RESULT");
        _botData.Variables["RESULT"].Should().Be(string.Empty);
    }

    [Fact]
    public void ValidateInstruction_With_Valid_Instruction_Should_Pass()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR", "<title>", "</title>" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "TITLE"
            }
        };

        var mockParser = new Mock<IDataParser>();
        mockParser.Setup(p => p.IsValidPattern("<title>")).Returns(true);
        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.LeftRight))
                         .Returns(mockParser.Object);

        // Act
        var result = _parseCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateInstruction_With_Missing_Arguments_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR" } // Missing pattern
        };

        // Act
        var result = _parseCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("PARSE command requires at least TARGET, TYPE, and PATTERN arguments");
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_Parse_Type_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "INVALID", "pattern" }
        };

        // Act
        var result = _parseCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Invalid parse type: INVALID"));
    }

    [Fact]
    public void ValidateInstruction_With_Unavailable_Parser_Type_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "XPATH", "//title" }
        };

        // Setup factory to not include XPATH
        _parserFactoryMock.Setup(pf => pf.GetAvailableTypes())
                         .Returns(new[] { ParseType.LeftRight, ParseType.Regex, ParseType.CSS, ParseType.Json });

        // Act
        var result = _parseCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Parser type XPATH is not available");
    }

    [Fact]
    public void ValidateInstruction_Without_Redirector_Should_Warn()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "LR", "<title>", "</title>" }
            // No redirector parameters
        };

        var mockParser = new Mock<IDataParser>();
        mockParser.Setup(p => p.IsValidPattern("<title>")).Returns(true);
        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.LeftRight))
                         .Returns(mockParser.Object);

        // Act
        var result = _parseCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Warnings.Should().Contain(w => w.Contains("should specify output destination"));
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_Boolean_Parameters_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "REGEX", @"\d+" },
            Parameters = new Dictionary<string, object>
            {
                ["Recursive"] = "invalid-boolean"
            }
        };

        // Act
        var result = _parseCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Recursive parameter must be true or false");
    }

    [Theory]
    [InlineData("LR")]
    [InlineData("REGEX")]
    [InlineData("CSS")]
    [InlineData("JSON")]
    public void ValidateInstruction_With_Valid_Parse_Types_Should_Pass(string parseType)
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", parseType, "test-pattern" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "RESULT"
            }
        };

        if (Enum.TryParse<ParseType>(parseType, true, out var type))
        {
            var mockParser = new Mock<IDataParser>();
            mockParser.Setup(p => p.IsValidPattern("test-pattern")).Returns(true);
            _parserFactoryMock.Setup(pf => pf.CreateParser(type))
                             .Returns(mockParser.Object);
        }

        // Act
        var result = _parseCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Handle_Exception_In_Execution()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "PARSE",
            Arguments = new List<string> { "<SOURCE>", "REGEX", @"\d+" },
            Parameters = new Dictionary<string, object>
            {
                ["RedirectorType"] = "VAR",
                ["RedirectorName"] = "RESULT"
            },
            LineNumber = 1
        };

        _parserFactoryMock.Setup(pf => pf.CreateParser(ParseType.Regex))
                         .Throws(new InvalidOperationException("Test exception"));

        // Act
        var result = await _parseCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Test exception");
        _botData.Status.Should().Be(BotStatus.Error);
    }

    public void Dispose()
    {
        // No resources to dispose in this test class
    }
}
