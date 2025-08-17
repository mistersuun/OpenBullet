using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using Xunit;
using IScriptEngine = OpenBullet.Core.Execution.IScriptEngine;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 9 Tests: ScriptEngine Implementation
/// </summary>
public class Step9_ScriptEngineTests : IDisposable
{
    private readonly Mock<ILogger<ScriptEngine>> _loggerMock;
    private readonly Mock<ICommandFactory> _commandFactoryMock;
    private readonly Mock<IScriptParser> _scriptParserMock;
    private readonly ScriptEngine _scriptEngine;
    private readonly ConfigModel _testConfig;
    private readonly BotData _testBotData;

    public Step9_ScriptEngineTests()
    {
        _loggerMock = new Mock<ILogger<ScriptEngine>>();
        _commandFactoryMock = new Mock<ICommandFactory>();
        _scriptParserMock = new Mock<IScriptParser>();

        _scriptEngine = new ScriptEngine(_loggerMock.Object, _commandFactoryMock.Object, _scriptParserMock.Object);

        _testConfig = new ConfigModel
        {
            Name = "TestConfig",
            Script = "REQUEST GET \"https://example.com\"\nKEYCHECK\n  KEYCHAIN Success\n    KEY \"<SOURCE>\" Contains \"Welcome\""
        };

        var logger = new Mock<ILogger>().Object;
        var cancellationToken = new CancellationTokenSource().Token;
        _testBotData = new BotData("test:data", _testConfig, logger, cancellationToken);
    }

    [Fact]
    public void ScriptEngine_Can_Be_Created()
    {
        // Act & Assert
        _scriptEngine.Should().NotBeNull();
        _scriptEngine.Should().BeAssignableTo<IScriptEngine>();
    }

    [Fact]
    public async Task ExecuteAsync_With_Empty_Script_Should_Return_Success()
    {
        // Arrange
        var config = new ConfigModel { Name = "Empty", Script = "" };
        _scriptParserMock.Setup(sp => sp.ParseScript("")).Returns(new ScriptParseResult { Success = true, Instructions = new List<ScriptInstruction>() });

        // Act
        var result = await _scriptEngine.ExecuteAsync(config, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Status.Should().Be(BotStatus.Success);
        result.CommandsExecuted.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_With_Single_Command_Should_Execute_Successfully()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com" },
            LineNumber = 1
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                  .ReturnsAsync(new CommandResult { Success = true });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = new List<ScriptInstruction> { instruction } });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CommandsExecuted.Should().Be(1);
        result.CommandResults.Should().HaveCount(1);
        result.CommandResults[0].Success.Should().BeTrue();
        result.CommandResults[0].CommandName.Should().Be("REQUEST");
    }

    [Fact]
    public async Task ExecuteAsync_With_Failed_Command_Should_Handle_Error()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://invalid-url" },
            LineNumber = 1
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                  .ReturnsAsync(new CommandResult 
                  { 
                      Success = false, 
                      ErrorMessage = "Connection failed" 
                  });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = new List<ScriptInstruction> { instruction } });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Connection failed");
        result.CommandsExecuted.Should().Be(1);
        result.CommandResults[0].Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_With_Unknown_Command_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "UNKNOWN_COMMAND",
            Arguments = new List<string>(),
            LineNumber = 1
        };

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = new List<ScriptInstruction> { instruction } });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("UNKNOWN_COMMAND"))
                          .Returns((IScriptCommand?)null);

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.CommandResults[0].ErrorMessage.Should().Contain("Unknown command");
    }

    [Fact]
    public async Task ExecuteAsync_With_Comments_Should_Skip_Comments()
    {
        // Arrange
        var instructions = new List<ScriptInstruction>
        {
            new() { CommandName = "#", Arguments = new List<string> { "This is a comment" }, LineNumber = 1 },
            new() { CommandName = "REQUEST", Arguments = new List<string> { "GET", "https://example.com" }, LineNumber = 2 }
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                  .ReturnsAsync(new CommandResult { Success = true });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = instructions });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CommandsExecuted.Should().Be(2); // Both comment and REQUEST are processed
        result.CommandResults[0].Success.Should().BeTrue(); // Comment should succeed
        result.CommandResults[1].Success.Should().BeTrue(); // REQUEST should succeed
    }

    [Fact]
    public async Task ExecuteAsync_With_Stop_Flow_Control_Should_Stop_Execution()
    {
        // Arrange
        var instructions = new List<ScriptInstruction>
        {
            new() { CommandName = "REQUEST", Arguments = new List<string> { "GET", "https://example.com" }, LineNumber = 1 },
            new() { CommandName = "STOP", Arguments = new List<string>(), LineNumber = 2 },
            new() { CommandName = "REQUEST", Arguments = new List<string> { "GET", "https://example2.com" }, LineNumber = 3 }
        };

        var mockRequestCommand = new Mock<IScriptCommand>();
        mockRequestCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                         .ReturnsAsync(new CommandResult { Success = true });

        var mockStopCommand = new Mock<IScriptCommand>();
        mockStopCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                      .ReturnsAsync(new CommandResult { Success = true, FlowControl = FlowControl.Stop });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = instructions });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockRequestCommand.Object);
        _commandFactoryMock.Setup(cf => cf.CreateCommand("STOP"))
                          .Returns(mockStopCommand.Object);

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CommandsExecuted.Should().Be(2); // Should stop after STOP command
        result.CommandResults.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_Statistics()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com" },
            LineNumber = 1
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                  .ReturnsAsync(new CommandResult { Success = true });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = new List<ScriptInstruction> { instruction } });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        var initialStats = _scriptEngine.GetExecutionStatistics();
        var initialScriptsExecuted = initialStats.TotalScriptsExecuted;

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        
        var updatedStats = _scriptEngine.GetExecutionStatistics();
        updatedStats.TotalScriptsExecuted.Should().Be(initialScriptsExecuted + 1);
        updatedStats.TotalCommandsExecuted.Should().BeGreaterThan(0);
        updatedStats.SuccessfulExecutions.Should().Be(initialScriptsExecuted + 1);
    }

    [Fact]
    public async Task ExecuteAsync_With_Cancellation_Should_Cancel_Gracefully()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com" },
            LineNumber = 1
        };

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = new List<ScriptInstruction> { instruction } });

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData, cancellationTokenSource.Token);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Script execution was cancelled");
    }

    [Fact]
    public void ValidateScript_With_Valid_Script_Should_Pass()
    {
        // Arrange
        var instructions = new List<ScriptInstruction>
        {
            new() { CommandName = "REQUEST", Arguments = new List<string> { "GET", "https://example.com" }, LineNumber = 1 }
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ValidateInstruction(It.IsAny<ScriptInstruction>()))
               .Returns(new CommandValidationResult { IsValid = true });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = instructions });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = _scriptEngine.ValidateScript(_testConfig);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.TotalCommands.Should().Be(1);
        result.CommandCounts.Should().ContainKey("REQUEST");
        result.CommandCounts["REQUEST"].Should().Be(1);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateScript_With_Empty_Script_Should_Fail()
    {
        // Arrange
        var config = new ConfigModel { Name = "Empty", Script = "" };

        // Act
        var result = _scriptEngine.ValidateScript(config);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Script is empty");
        result.Errors[0].Severity.Should().Be(ValidationSeverity.Critical);
    }

    [Fact]
    public void ValidateScript_With_Invalid_Command_Should_Fail()
    {
        // Arrange
        var instructions = new List<ScriptInstruction>
        {
            new() { CommandName = "INVALID_COMMAND", Arguments = new List<string>(), LineNumber = 1 }
        };

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = instructions });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("INVALID_COMMAND"))
                          .Returns((IScriptCommand?)null);

        // Act
        var result = _scriptEngine.ValidateScript(_testConfig);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Unknown command: INVALID_COMMAND");
    }

    [Fact]
    public void ValidateScript_With_Command_Validation_Errors_Should_Fail()
    {
        // Arrange
        var instructions = new List<ScriptInstruction>
        {
            new() { CommandName = "REQUEST", Arguments = new List<string>(), LineNumber = 1 } // Missing arguments
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ValidateInstruction(It.IsAny<ScriptInstruction>()))
               .Returns(new CommandValidationResult 
               { 
                   IsValid = false, 
                   Errors = new List<string> { "Missing required arguments" },
                   Warnings = new List<string> { "Consider adding timeout" }
               });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = instructions });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = _scriptEngine.ValidateScript(_testConfig);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("Missing required arguments");
        result.Warnings.Should().HaveCount(1);
        result.Warnings[0].Message.Should().Be("Consider adding timeout");
    }

    [Fact]
    public void ValidateScript_With_Labels_Should_Extract_Labels()
    {
        // Arrange
        var instructions = new List<ScriptInstruction>
        {
            new() { CommandName = "REQUEST", Arguments = new List<string> { "GET", "https://example.com" }, Label = "START", LineNumber = 1 },
            new() { CommandName = "JUMP", Arguments = new List<string> { "START" }, LineNumber = 2 }
        };

        var mockRequestCommand = new Mock<IScriptCommand>();
        mockRequestCommand.Setup(c => c.ValidateInstruction(It.IsAny<ScriptInstruction>()))
                         .Returns(new CommandValidationResult { IsValid = true });

        var mockJumpCommand = new Mock<IScriptCommand>();
        mockJumpCommand.Setup(c => c.ValidateInstruction(It.IsAny<ScriptInstruction>()))
                      .Returns(new CommandValidationResult { IsValid = true });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = instructions });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockRequestCommand.Object);
        _commandFactoryMock.Setup(cf => cf.CreateCommand("JUMP"))
                          .Returns(mockJumpCommand.Object);

        // Act
        var result = _scriptEngine.ValidateScript(_testConfig);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.DefinedLabels.Should().Contain("START");
        result.ReferencedLabels.Should().Contain("START");
    }

    [Fact]
    public void ValidateScript_With_Duplicate_Labels_Should_Report_Error()
    {
        // Arrange
        var instructions = new List<ScriptInstruction>
        {
            new() { CommandName = "REQUEST", Arguments = new List<string> { "GET", "https://example.com" }, Label = "DUPLICATE", LineNumber = 1 },
            new() { CommandName = "REQUEST", Arguments = new List<string> { "GET", "https://example2.com" }, Label = "DUPLICATE", LineNumber = 2 }
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ValidateInstruction(It.IsAny<ScriptInstruction>()))
               .Returns(new CommandValidationResult { IsValid = true });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = instructions });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = _scriptEngine.ValidateScript(_testConfig);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Duplicate label: DUPLICATE"));
    }

    [Fact]
    public void GetExecutionStatistics_Should_Return_Current_Statistics()
    {
        // Act
        var stats = _scriptEngine.GetExecutionStatistics();

        // Assert
        stats.Should().NotBeNull();
        stats.TotalScriptsExecuted.Should().BeGreaterOrEqualTo(0);
        stats.TotalCommandsExecuted.Should().BeGreaterOrEqualTo(0);
        stats.StartTime.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void ResetStatistics_Should_Clear_All_Statistics()
    {
        // Arrange
        var initialStats = _scriptEngine.GetExecutionStatistics();
        
        // Act
        _scriptEngine.ResetStatistics();
        var resetStats = _scriptEngine.GetExecutionStatistics();

        // Assert
        resetStats.TotalScriptsExecuted.Should().Be(0);
        resetStats.TotalCommandsExecuted.Should().Be(0);
        resetStats.SuccessfulExecutions.Should().Be(0);
        resetStats.FailedExecutions.Should().Be(0);
        resetStats.CommandExecutionCounts.Should().BeEmpty();
        resetStats.CommandExecutionTimes.Should().BeEmpty();
        resetStats.StatusCounts.Should().BeEmpty();
    }

    [Theory]
    [InlineData(FlowControl.Continue)]
    [InlineData(FlowControl.Stop)]
    [InlineData(FlowControl.Break)]
    public async Task ExecuteAsync_Should_Handle_All_Flow_Control_Types(FlowControl flowControl)
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "TEST",
            Arguments = new List<string>(),
            LineNumber = 1
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                  .ReturnsAsync(new CommandResult { Success = true, FlowControl = flowControl });

        _scriptParserMock.Setup(sp => sp.ParseScript(It.IsAny<string>())).Returns(new ScriptParseResult { Success = true, Instructions = new List<ScriptInstruction> { instruction } });
        _commandFactoryMock.Setup(cf => cf.CreateCommand("TEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = await _scriptEngine.ExecuteAsync(_testConfig, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.CommandResults.Should().HaveCount(1);
        result.CommandResults[0].FlowControl.Should().Be(flowControl);
    }

    [Fact]
    public async Task ExecuteInstructionAsync_Should_Execute_Single_Instruction()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com" },
            LineNumber = 1
        };

        var mockCommand = new Mock<IScriptCommand>();
        mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<ScriptInstruction>(), It.IsAny<BotData>()))
                  .ReturnsAsync(new CommandResult { Success = true });

        _commandFactoryMock.Setup(cf => cf.CreateCommand("REQUEST"))
                          .Returns(mockCommand.Object);

        // Act
        var result = await _scriptEngine.ExecuteInstructionAsync(instruction, _testBotData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CommandName.Should().Be("REQUEST");
        result.LineNumber.Should().Be(1);
        result.FlowControl.Should().Be(FlowControl.Continue);
    }

    public void Dispose()
    {
        // No resources to dispose in this test class
    }
}
