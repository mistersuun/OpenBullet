using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Models;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Execution;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 1 Tests: Core Interfaces and Basic Models
/// </summary>
public class Step1_CoreInterfacesTests
{
    [Fact]
    public void BotData_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var config = new ConfigModel { Name = "Test Config" };
        var logger = new Mock<ILogger>().Object;
        var cancellationToken = CancellationToken.None;
        var botData = new BotData("test-data-line", config, logger, cancellationToken);

        // Assert
        botData.Id.Should().NotBeNullOrEmpty();
        botData.DataLine.Should().Be("test-data-line");
        botData.Variables.Should().NotBeNull().And.BeEmpty();
        botData.CapturedData.Should().NotBeNull().And.BeEmpty();
        botData.Cookies.Should().NotBeNull();
        botData.Status.Should().Be(BotStatus.None);
        botData.CustomStatus.Should().BeEmpty();
        botData.Source.Should().BeEmpty();
        botData.Log.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void BotData_SetVariable_Should_Store_Variable()
    {
        // Arrange
        var config = new ConfigModel { Name = "Test Config" };
        var logger = new Mock<ILogger>().Object;
        var botData = new BotData("test-data", config, logger, CancellationToken.None);
        const string variableName = "testVar";
        const string variableValue = "testValue";

        // Act
        botData.SetVariable(variableName, variableValue);

        // Assert
        botData.Variables.Should().ContainKey(variableName);
        botData.Variables[variableName].Should().Be(variableValue);
    }

    [Fact]
    public void BotData_GetVariable_Should_Return_Correct_Type()
    {
        // Arrange
        var config = new ConfigModel { Name = "Test Config" };
        var logger = new Mock<ILogger>().Object;
        var botData = new BotData("test-data", config, logger, CancellationToken.None);
        const string variableName = "testVar";
        const int variableValue = 42;
        botData.SetVariable(variableName, variableValue);

        // Act
        var result = botData.GetVariable<int>(variableName);

        // Assert
        result.Should().Be(variableValue);
    }

    [Fact]
    public void BotData_GetVariable_Should_Return_Default_For_Missing_Variable()
    {
        // Arrange
        var config = new ConfigModel { Name = "Test Config" };
        var logger = new Mock<ILogger>().Object;
        var botData = new BotData("test-data", config, logger, CancellationToken.None);

        // Act
        var result = botData.GetVariable<string>("nonExistentVar");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void BotData_SetCapture_Should_Store_Captured_Data()
    {
        // Arrange
        var config = new ConfigModel { Name = "Test Config" };
        var logger = new Mock<ILogger>().Object;
        var botData = new BotData("test-data", config, logger, CancellationToken.None);
        const string captureName = "username";
        const string captureValue = "testuser";

        // Act
        botData.SetCapture(captureName, captureValue);

        // Assert
        botData.CapturedData.Should().ContainKey(captureName);
        botData.CapturedData[captureName].Should().Be(captureValue);
    }

    [Fact]
    public void BotData_AddLogEntry_Should_Add_Timestamped_Entry()
    {
        // Arrange
        var config = new ConfigModel { Name = "Test Config" };
        var logger = new Mock<ILogger>().Object;
        var botData = new BotData("test-data", config, logger, CancellationToken.None);
        const string message = "Test log message";

        // Act
        botData.AddLogEntry(message);

        // Assert
        botData.Log.Should().HaveCount(1);
        botData.Log[0].Should().Contain(message);
        botData.Log[0].Should().MatchRegex(@"^\[\d{2}:\d{2}:\d{2}\] Test log message$");
    }

    [Fact]
    public void ProxyInfo_ToString_Should_Return_Host_Port_Format()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "127.0.0.1",
            Port = 8080
        };

        // Act
        var result = proxy.ToString();

        // Assert
        result.Should().Be("127.0.0.1:8080");
    }

    [Fact]
    public void ConfigModel_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var config = new ConfigModel();

        // Assert
        config.Id.Should().NotBeNullOrEmpty();
        config.Name.Should().BeEmpty();
        config.Author.Should().BeEmpty();
        config.Version.Should().Be("1.0.0");
        config.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        config.Script.Should().BeEmpty();
        config.Settings.Should().NotBeNull();
    }

    [Fact]
    public void ConfigSettings_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var settings = new ConfigSettings();

        // Assert
        settings.SuggestedBots.Should().Be(1);
        settings.MaxCPM.Should().Be(0);
        settings.IgnoreResponseErrors.Should().BeFalse();
        settings.MaxRedirects.Should().Be(8);
        settings.NeedsProxies.Should().BeFalse();
        settings.DataRules.Should().NotBeNull().And.BeEmpty();
        settings.CustomInputs.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ScriptInstruction_GetArgument_Should_Return_Correct_Value()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            Arguments = { "arg1", "arg2", "arg3" }
        };

        // Act & Assert
        instruction.GetArgument(0).Should().Be("arg1");
        instruction.GetArgument(1).Should().Be("arg2");
        instruction.GetArgument(5, "default").Should().Be("default");
    }

    [Fact]
    public void ScriptInstruction_GetParameter_Should_Return_Correct_Type()
    {
        // Arrange
        var instruction = new ScriptInstruction();
        instruction.Parameters["testParam"] = 42;

        // Act & Assert
        instruction.GetParameter<int>("testParam").Should().Be(42);
        instruction.GetParameter<string>("nonExistent", "default").Should().Be("default");
    }

    [Fact]
    public void BotResult_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var result = new BotResult();

        // Assert
        result.Status.Should().Be(BotStatus.None);
        result.CustomStatus.Should().BeEmpty();
        result.CapturedData.Should().NotBeNull().And.BeEmpty();
        result.DataLine.Should().BeEmpty();
        result.Logs.Should().NotBeNull().And.BeEmpty();
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void CommandResult_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var result = new CommandResult();

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.FlowControl.Should().Be(FlowControl.Continue);
        result.Variables.Should().NotBeNull().And.BeEmpty();
        result.CapturedData.Should().NotBeNull().And.BeEmpty();
        result.NewStatus.Should().BeNull();
        result.CustomStatus.Should().BeNull();
    }
}

/// <summary>
/// Test utilities for Step 1
/// </summary>
public static class Step1TestHelpers
{
    public static BotData CreateTestBotData(string dataLine = "test:pass")
    {
        var config = new ConfigModel { Name = "Test Config" };
        var logger = new Mock<ILogger>().Object;
        var botData = new BotData(dataLine, config, logger, CancellationToken.None);
        return botData;
    }

    public static ConfigModel CreateTestConfig(string name = "TestConfig")
    {
        return new ConfigModel
        {
            Name = name,
            Author = "TestAuthor",
            Script = "## Test script\nPRINT Hello World",
            Settings = new ConfigSettings
            {
                SuggestedBots = 10,
                NeedsProxies = true
            }
        };
    }

    public static ScriptInstruction CreateTestInstruction(string command, params string[] args)
    {
        return new ScriptInstruction
        {
            CommandName = command,
            Arguments = args.ToList(),
            LineNumber = 1,
            RawLine = $"{command} {string.Join(" ", args)}"
        };
    }
}
