using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using OpenBullet.Core.Services;
using Xunit;
using ParameterType = OpenBullet.Core.Commands.ParameterType;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 6 Validation Tests - Basic functionality validation
/// </summary>
public class Step6_ValidationTests
{
    [Fact]
    public void ICommandFactory_Interface_Should_Be_Properly_Defined()
    {
        // Assert
        typeof(ICommandFactory).IsInterface.Should().BeTrue();
        
        var methods = typeof(ICommandFactory).GetMethods();
        methods.Should().Contain(m => m.Name == nameof(ICommandFactory.CreateCommand));
        methods.Should().Contain(m => m.Name == nameof(ICommandFactory.GetAvailableCommands));
        methods.Should().Contain(m => m.Name == nameof(ICommandFactory.RegisterCommand) && m.IsGenericMethod);
        methods.Should().Contain(m => m.Name == nameof(ICommandFactory.IsCommandRegistered));
        methods.Should().Contain(m => m.Name == nameof(ICommandFactory.GetCommandMetadata));
    }

    [Fact]
    public void CommandFactory_Can_Be_Created()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CommandFactory>>();
        var serviceProviderMock = new Mock<IServiceProvider>();

        // Act
        var factory = new CommandFactory(loggerMock.Object, serviceProviderMock.Object);

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeAssignableTo<ICommandFactory>();
    }

    [Fact]
    public void RequestCommand_Can_Be_Created_With_Valid_ServiceProvider()
    {
        // Arrange
        var serviceProvider = CreateValidServiceProvider();

        // Act
        var command = new RequestCommand(serviceProvider);

        // Assert
        command.Should().NotBeNull();
        command.Should().BeAssignableTo<IScriptCommand>();
        command.CommandName.Should().Be("REQUEST");
        command.Description.Should().NotBeEmpty();
    }

    [Fact]
    public void RequestCommand_Should_Throw_With_Invalid_ServiceProvider()
    {
        // Arrange
        var emptyServiceProvider = new Mock<IServiceProvider>().Object;

        // Act & Assert
        var act = () => new RequestCommand(emptyServiceProvider);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CommandMetadata_Should_Have_Correct_Structure()
    {
        // Act
        var metadata = new CommandMetadata();

        // Assert
        metadata.Should().NotBeNull();
        metadata.Name.Should().BeEmpty();
        metadata.Description.Should().BeEmpty();
        metadata.Category.Should().BeEmpty();
        metadata.Parameters.Should().NotBeNull();
        metadata.Examples.Should().NotBeNull();
        metadata.RequiresVariableSubstitution.Should().BeTrue();
        metadata.CanHaveSubcommands.Should().BeFalse();
    }

    [Fact]
    public void CommandParameter_Should_Have_Correct_Defaults()
    {
        // Act
        var parameter = new CommandParameter();

        // Assert
        parameter.Should().NotBeNull();
        parameter.Name.Should().BeEmpty();
        parameter.Type.Should().Be(ParameterType.String);
        parameter.Required.Should().BeTrue();
        parameter.DefaultValue.Should().BeNull();
        parameter.AllowedValues.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ParameterType_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.String);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.Integer);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.Boolean);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.Url);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.HttpMethod);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.ContentType);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.FilePath);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.Variable);
        Enum.GetValues<ParameterType>().Should().Contain(ParameterType.Enum);
    }

    [Fact]
    public void ScriptInstruction_Should_Have_Parameters_Property()
    {
        // Act
        var instruction = new ScriptInstruction();

        // Assert
        instruction.Should().NotBeNull();
        instruction.Parameters.Should().NotBeNull();
        instruction.Parameters.Should().BeEmpty();
    }

    [Fact]
    public void ScriptInstruction_GetParameter_Should_Work()
    {
        // Arrange
        var instruction = new ScriptInstruction();
        instruction.Parameters["TestKey"] = "TestValue";
        instruction.Parameters["IntKey"] = 42;
        instruction.Parameters["BoolKey"] = true;

        // Act & Assert
        instruction.GetParameter<string>("TestKey").Should().Be("TestValue");
        instruction.GetParameter<int>("IntKey").Should().Be(42);
        instruction.GetParameter<bool>("BoolKey").Should().BeTrue();
        instruction.GetParameter<string>("Missing").Should().BeNull();
        instruction.GetParameter<int>("Missing").Should().Be(0);
        instruction.GetParameter<string>("Missing", "default").Should().Be("default");
    }

    [Fact]
    public void CommandResult_Should_Initialize_With_Correct_Defaults()
    {
        // Act
        var result = new CommandResult();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.FlowControl.Should().Be(FlowControl.Continue);
        result.Variables.Should().NotBeNull().And.BeEmpty();
        result.CapturedData.Should().NotBeNull().And.BeEmpty();
        result.NewStatus.Should().BeNull();
        result.CustomStatus.Should().BeNull();
    }

    [Fact]
    public void CommandValidationResult_Should_Initialize_Correctly()
    {
        // Act
        var result = new CommandValidationResult();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNull().And.BeEmpty();
        result.Warnings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void FlowControl_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<FlowControl>().Should().Contain(FlowControl.Continue);
        Enum.GetValues<FlowControl>().Should().Contain(FlowControl.Stop);
        Enum.GetValues<FlowControl>().Should().Contain(FlowControl.Jump);
        Enum.GetValues<FlowControl>().Should().Contain(FlowControl.Break);
    }

    [Fact]
    public void RequestCommand_ValidateInstruction_Should_Handle_Basic_Cases()
    {
        // Arrange
        var command = new RequestCommand(CreateValidServiceProvider());
        
        var validInstruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com" }
        };

        var invalidInstruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET" } // Missing URL
        };

        // Act
        var validResult = command.ValidateInstruction(validInstruction);
        var invalidResult = command.ValidateInstruction(invalidInstruction);

        // Assert
        validResult.Should().NotBeNull();
        validResult.IsValid.Should().BeTrue();

        invalidResult.Should().NotBeNull();
        invalidResult.IsValid.Should().BeFalse();
        invalidResult.Errors.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("GET", true)]
    [InlineData("POST", true)]
    [InlineData("PUT", true)]
    [InlineData("DELETE", true)]
    [InlineData("PATCH", true)]
    [InlineData("HEAD", true)]
    [InlineData("OPTIONS", true)]
    [InlineData("INVALID", false)]
    [InlineData("", false)]
    public void RequestCommand_Should_Validate_HTTP_Methods_Correctly(string method, bool shouldBeValid)
    {
        // Arrange
        var command = new RequestCommand(CreateValidServiceProvider());
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { method, "https://example.com" }
        };

        // Act
        var result = command.ValidateInstruction(instruction);

        // Assert
        if (shouldBeValid)
        {
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }
    }

    [Fact]
    public void RequestCommand_Should_Validate_Parameter_Requirements()
    {
        // Arrange
        var command = new RequestCommand(CreateValidServiceProvider());

        var incompleteContentInstruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "POST", "https://example.com", "CONTENT" } // Missing content value
        };

        var incompleteHeaderInstruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string> { "GET", "https://example.com", "HEADER", "Authorization" } // Missing header value
        };

        // Act
        var contentResult = command.ValidateInstruction(incompleteContentInstruction);
        var headerResult = command.ValidateInstruction(incompleteHeaderInstruction);

        // Assert
        contentResult.IsValid.Should().BeFalse();
        contentResult.Errors.Should().Contain(e => e.Contains("CONTENT parameter requires a value"));

        headerResult.IsValid.Should().BeFalse();
        headerResult.Errors.Should().Contain(e => e.Contains("HEADER parameter requires name and value"));
    }

    [Fact]
    public void CommandFactory_Should_Register_And_Create_Commands()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CommandFactory>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var factory = new CommandFactory(loggerMock.Object, serviceProviderMock.Object);

        // Act
        factory.RegisterCommand<TestValidationCommand>("TEST");

        // Assert
        factory.IsCommandRegistered("TEST").Should().BeTrue();
        factory.GetAvailableCommands().Should().Contain("TEST");
    }

    [Fact]
    public void CommandFactory_Should_Handle_Case_Insensitive_Operations()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CommandFactory>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var factory = new CommandFactory(loggerMock.Object, serviceProviderMock.Object);

        factory.RegisterCommand<TestValidationCommand>("CaseTest");

        // Act & Assert
        factory.IsCommandRegistered("CASETEST").Should().BeTrue();
        factory.IsCommandRegistered("casetest").Should().BeTrue();
        factory.IsCommandRegistered("CaseTest").Should().BeTrue();
    }

    [Fact]
    public void Step6TestHelpers_Should_Create_Valid_Objects()
    {
        // Act
        var serviceProvider = Step6TestHelpers.CreateServiceProvider();
        var instruction = Step6TestHelpers.CreateRequestInstruction();
        var botData = Step6TestHelpers.CreateTestBotData();
        var successResponse = Step6TestHelpers.CreateSuccessResponse();
        var errorResponse = Step6TestHelpers.CreateErrorResponse();

        // Assert
        serviceProvider.Should().NotBeNull();
        
        instruction.Should().NotBeNull();
        instruction.CommandName.Should().Be("REQUEST");
        instruction.Arguments.Should().HaveCount(2);
        
        botData.Should().NotBeNull();
        botData.DataLine.Should().Be("test:data");
        
        successResponse.Should().NotBeNull();
        successResponse.IsSuccessStatusCode.Should().BeTrue();
        
        errorResponse.Should().NotBeNull();
        errorResponse.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public void Complex_Request_Instruction_Should_Parse_Correctly()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "REQUEST",
            Arguments = new List<string>
            {
                "POST",
                "https://api.example.com/login",
                "CONTENT",
                "username=<USER>&password=<PASS>",
                "CONTENTTYPE",
                "application/x-www-form-urlencoded",
                "HEADER",
                "Authorization",
                "Bearer token123",
                "Cookie",
                "sessionid",
                "abc123"
            },
            Parameters = new Dictionary<string, object>
            {
                ["AutoRedirect"] = false,
                ["Timeout"] = 30
            },
            SubInstructions = new List<ScriptInstruction>
            {
                new() { CommandName = "HEADER", Arguments = new List<string> { "X-API-Key", "secret" } }
            }
        };

        // Act
        var command = new RequestCommand(CreateValidServiceProvider());
        var validationResult = command.ValidateInstruction(instruction);

        // Assert
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
        
        instruction.Arguments.Should().Contain("POST");
        instruction.Arguments.Should().Contain("https://api.example.com/login");
        instruction.Parameters.Should().ContainKey("AutoRedirect");
        instruction.Parameters.Should().ContainKey("Timeout");
        instruction.SubInstructions.Should().HaveCount(1);
    }

    private static IServiceProvider CreateValidServiceProvider()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<RequestCommand>)))
                      .Returns(new Mock<ILogger<RequestCommand>>().Object);
        serviceProvider.Setup(sp => sp.GetService(typeof(IHttpClientService)))
                      .Returns(new Mock<IHttpClientService>().Object);
        serviceProvider.Setup(sp => sp.GetService(typeof(IScriptParser)))
                      .Returns(new Mock<IScriptParser>().Object);
        
        return serviceProvider.Object;
    }

    /// <summary>
    /// Test command for validation testing
    /// </summary>
    private class TestValidationCommand : IScriptCommand
    {
        public string CommandName => "TESTVALIDATION";
        public string Description => "Test validation command";

        public Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData)
        {
            return Task.FromResult(new CommandResult { Success = true });
        }

        public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
        {
            return new CommandValidationResult { IsValid = true };
        }
    }
}
