using Microsoft.Extensions.Logging;
using OpenBullet.Core.Services;
using FluentAssertions;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Tests for Step 13 Block 1: Auto-Completion Foundation
/// </summary>
public class Step13_Block1_AutoCompletionTests
{
    private readonly AutoCompletionProvider _autoCompletionProvider;

    public Step13_Block1_AutoCompletionTests()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _autoCompletionProvider = new AutoCompletionProvider(loggerFactory.CreateLogger<AutoCompletionProvider>());
    }

    [Fact]
    public void AutoCompletionProvider_Should_Be_Initialized()
    {
        // Arrange & Act
        var provider = new AutoCompletionProvider();

        // Assert
        provider.Should().NotBeNull();
    }

    [Fact]
    public void GetCompletions_Should_Return_Command_Suggestions_For_REQUEST()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "REQ",
            Position = 3,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeNull();
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text == "REQUEST");
    }

    [Fact]
    public void GetCompletions_Should_Return_Command_Suggestions_For_PAR()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "PAR",
            Position = 3,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeNull();
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text == "PARSE");
    }

    [Fact]
    public void GetCompletions_Should_Return_Command_Suggestions_For_KEY()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "KEY",
            Position = 3,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeNull();
        completions.Should().NotBeEmpty();
        completions.Should().Contain(c => c.Text == "KEYCHECK");
    }

    [Fact]
    public void GetCompletions_Should_Return_Variable_Suggestions_When_Available()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "<VAR",
            Position = 4,
            LineNumber = 1,
            AvailableVariables = new[] { "VAR1", "VAR2", "VARIABLE" }
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeNull();
        completions.Should().NotBeEmpty();
        completions.Where(c => c.Type == CompletionType.Variable).Should().NotBeEmpty();
    }

    [Fact]
    public void GetCompletions_Should_Handle_Empty_Context()
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = "",
            Position = 0,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeNull();
        // Empty context should return empty completions (correct behavior)
        completions.Should().BeEmpty();
    }

    [Fact]
    public void GetCompletions_Should_Handle_Null_Context()
    {
        // Arrange
        CompletionContext? context = null;

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context!);

        // Assert
        completions.Should().NotBeNull();
        completions.Should().BeEmpty();
    }

    [Theory]
    [InlineData("REQUEST ")]
    [InlineData("PARSE ")]
    [InlineData("KEYCHECK ")]
    public void GetCompletions_Should_Handle_Complete_Commands_With_Space(string commandText)
    {
        // Arrange
        var context = new CompletionContext
        {
            Text = commandText,
            Position = commandText.Length,
            LineNumber = 1
        };

        // Act
        var completions = _autoCompletionProvider.GetCompletions(context);

        // Assert
        completions.Should().NotBeNull();
        // Should provide parameter suggestions or other contextual completions
    }

    [Fact]
    public void CompletionContext_Should_Have_All_Required_Properties()
    {
        // Arrange & Act
        var context = new CompletionContext
        {
            Text = "REQUEST GET",
            Position = 11,
            LineNumber = 1,
            CurrentCommand = "REQUEST",
            AvailableVariables = new[] { "VAR1", "VAR2" }
        };

        // Assert
        context.Text.Should().Be("REQUEST GET");
        context.Position.Should().Be(11);
        context.LineNumber.Should().Be(1);
        context.CurrentCommand.Should().Be("REQUEST");
        context.AvailableVariables.Should().NotBeNull();
        context.AvailableVariables!.Length.Should().Be(2);
    }

    [Fact]
    public void CompletionItem_Should_Have_All_Required_Properties()
    {
        // Arrange & Act
        var item = new CompletionItem
        {
            Text = "REQUEST",
            Type = CompletionType.Command,
            Description = "HTTP request command",
            InsertText = "REQUEST GET \"\"",
            Priority = 10
        };

        // Assert
        item.Text.Should().Be("REQUEST");
        item.Type.Should().Be(CompletionType.Command);
        item.Description.Should().Be("HTTP request command");
        item.InsertText.Should().Be("REQUEST GET \"\"");
        item.Priority.Should().Be(10);
    }

    [Fact]
    public void Block1_Integration_Should_Be_Complete()
    {
        // This test verifies that Block 1 implementation is complete
        
        // Arrange
        var provider = new AutoCompletionProvider();

        // Act & Assert
        
        // 1. AutoCompletionProvider should be initialized
        provider.Should().NotBeNull();
        
        // 2. Should provide command completions for partial commands
        var commandContext = new CompletionContext
        {
            Text = "REQ",
            Position = 3,
            LineNumber = 1
        };
        var commandCompletions = provider.GetCompletions(commandContext);
        commandCompletions.Should().NotBeEmpty();
        commandCompletions.Should().Contain(c => c.Type == CompletionType.Command);
        
        // 3. Should handle basic error cases
        var emptyCompletions = provider.GetCompletions(null!);
        emptyCompletions.Should().NotBeNull();
        emptyCompletions.Should().BeEmpty();
        
        // Block 1 implementation is complete!
    }
}
