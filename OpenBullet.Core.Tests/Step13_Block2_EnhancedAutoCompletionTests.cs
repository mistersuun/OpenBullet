using Microsoft.Extensions.Logging;
using OpenBullet.Core.Services;
using FluentAssertions;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Tests for Step 13 Block 2: Enhanced Auto-Completion
/// Tests variable suggestions, parameter hints, and context-aware completions
/// </summary>
public class Step13_Block2_EnhancedAutoCompletionTests
{
    private readonly AutoCompletionProvider _autoCompletionProvider;

    public Step13_Block2_EnhancedAutoCompletionTests()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _autoCompletionProvider = new AutoCompletionProvider(loggerFactory.CreateLogger<AutoCompletionProvider>());
    }

    [Fact]
    public void GetCompletions_Should_ProvideVariableSuggestions_WhenInVariableContext()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "REQUEST GET \"<SOU",
            Position = 18,
            LineNumber = 1,
            AvailableVariables = new[] { "EMAIL", "PASSWORD", "TOKEN" }
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Type == CompletionType.Variable);
        completions.Should().Contain(c => c.Text == "SOURCE");
        completions.Should().Contain(c => c.Text == "EMAIL");
        
        // Check priorities
        var sourceCompletion = completions.First(c => c.Text == "SOURCE");
        sourceCompletion.Priority.Should().BeGreaterThan(5);
    }

    [Fact]
    public void GetCompletions_Should_ProvideHTTPMethodSuggestions_ForRequestCommand()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "REQUEST G",
            Position = 9,
            LineNumber = 1,
            CurrentCommand = "REQUEST"
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text == "GET");
        completions.Should().Contain(c => c.Text == "POST");
        completions.Should().Contain(c => c.Text == "PUT");
        
        var getCompletion = completions.First(c => c.Text == "GET");
        getCompletion.Description.Should().Contain("HTTP GET method");
        getCompletion.Priority.Should().Be(10);
    }

    [Fact]
    public void GetCompletions_Should_ProvideURLSuggestions_ForRequestCommand()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "REQUEST GET \"",
            Position = 13,
            LineNumber = 1,
            CurrentCommand = "REQUEST"
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text.Contains("https://"));
        completions.Should().Contain(c => c.Text.Contains("http://"));
        
        var httpsCompletion = completions.First(c => c.Text.Contains("https://"));
        httpsCompletion.Description.Should().Contain("HTTPS URL template");
    }

    [Fact]
    public void GetCompletions_Should_ProvideRegexPatterns_ForParseCommand()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "PARSE \"<title",
            Position = 13,
            LineNumber = 1,
            CurrentCommand = "PARSE"
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text.Contains("<title>(.*?)</title>"));
        completions.Should().Contain(c => c.Text.Contains("token"));
        
        var titleCompletion = completions.First(c => c.Text.Contains("<title>"));
        titleCompletion.Description.Should().Contain("Extract page title");
    }

    [Fact]
    public void GetCompletions_Should_ProvideKeycheckValues_ForKeycheckCommand()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "KEYCHECK \"SUC",
            Position = 14,
            LineNumber = 1,
            CurrentCommand = "KEYCHECK"
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text.Contains("SUCCESS"));
        completions.Should().Contain(c => c.Text.Contains("FAILURE"));
        completions.Should().Contain(c => c.Text.Contains("BAN"));
        
        var successCompletion = completions.First(c => c.Text.Contains("SUCCESS"));
        successCompletion.Description.Should().Contain("successful result");
    }

    [Fact]
    public void GetCompletions_Should_ProvideVariableAssignment_WithArrowOperator()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "REQUEST GET \"https://example.com\" ",
            Position = 35,
            LineNumber = 1,
            CurrentCommand = "REQUEST"
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text.Contains("-> VAR"));
        
        var arrowCompletion = completions.First(c => c.Text.Contains("-> VAR"));
        arrowCompletion.Description.Should().Contain("Assign result to variable");
    }

    [Fact]
    public void GetCompletions_Should_ProvideCommonVariablePatterns()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "REQUEST GET \"<em",
            Position = 16,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text == "EMAIL");
        
        var emailCompletion = completions.First(c => c.Text == "EMAIL");
        emailCompletion.Description.Should().Contain("Captured email address");
        emailCompletion.InsertText.Should().Be("<EMAIL>");
    }

    [Fact]
    public void GetCompletions_Should_PrioritizeCompletions_Correctly()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "REQUEST ",
            Position = 8,
            LineNumber = 1,
            CurrentCommand = "REQUEST",
            AvailableVariables = new[] { "MY_VAR" }
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        
        // Built-in variables should have highest priority (10)
        var builtinCompletions = completions.Where(c => c.Type == CompletionType.Variable && c.Priority == 10);
        builtinCompletions.Should().NotBeEmpty();
        
        // User variables should have medium-high priority (8)
        var userVarCompletions = completions.Where(c => c.Type == CompletionType.Variable && c.Priority == 8);
        userVarCompletions.Should().NotBeEmpty();
        
        // HTTP methods should have highest priority (10)
        var httpMethodCompletions = completions.Where(c => c.Type == CompletionType.Parameter && c.Priority == 10);
        httpMethodCompletions.Should().NotBeEmpty();
    }

    [Fact]
    public void GetCompletions_Should_HandleVariableBrackets_Correctly()
    {
        // Arrange - cursor is inside variable brackets
        var context = new CompletionContext
        {
            Text = "REQUEST GET \"<SOU",
            Position = 17,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        var variableCompletions = completions.Where(c => c.Type == CompletionType.Variable);
        variableCompletions.Should().NotBeEmpty();
        
        // Inside brackets, should not add additional brackets
        var sourceCompletion = variableCompletions.FirstOrDefault(c => c.Text == "SOURCE");
        sourceCompletion?.InsertText.Should().Be("SOURCE"); // Not "<SOURCE>"
    }

    [Fact]
    public void GetCompletions_Should_HandleVariableBrackets_WhenOutsideContext()
    {
        // Arrange - cursor is outside variable context
        var context = new CompletionContext
        {
            Text = "REQUEST GET SOU",
            Position = 15,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        var variableCompletions = completions.Where(c => c.Type == CompletionType.Variable);
        variableCompletions.Should().NotBeEmpty();
        
        // Outside brackets, should add brackets
        var sourceCompletion = variableCompletions.FirstOrDefault(c => c.Text == "SOURCE");
        sourceCompletion?.InsertText.Should().Be("<SOURCE>");
    }

    [Theory]
    [InlineData("REQ", "REQUEST")]
    [InlineData("PAR", "PARSE")]
    [InlineData("KEY", "KEYCHECK")]
    public void GetCompletions_Should_ProvideCommandCompletions_ForPartialInput(string input, string expectedCommand)
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = input,
            Position = input.Length,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text == expectedCommand && c.Type == CompletionType.Command);
    }
}

