using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Interfaces;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 6 Tests: Command Factory
/// </summary>
public class Step6_CommandFactoryTests : IDisposable
{
    private readonly Mock<ILogger<CommandFactory>> _loggerMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly CommandFactory _commandFactory;

    public Step6_CommandFactoryTests()
    {
        _loggerMock = new Mock<ILogger<CommandFactory>>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _commandFactory = new CommandFactory(_loggerMock.Object, _serviceProviderMock.Object);
    }

    [Fact]
    public void CommandFactory_Should_Initialize_With_Default_Commands()
    {
        // Assert
        _commandFactory.Should().NotBeNull();
        _commandFactory.Should().BeAssignableTo<ICommandFactory>();
        
        var availableCommands = _commandFactory.GetAvailableCommands().ToList();
        availableCommands.Should().Contain("REQUEST");
    }

    [Fact]
    public void GetAvailableCommands_Should_Return_Registered_Commands()
    {
        // Act
        var commands = _commandFactory.GetAvailableCommands().ToList();

        // Assert
        commands.Should().NotBeEmpty();
        commands.Should().Contain("REQUEST");
        commands.Should().BeInAscendingOrder();
    }

    [Fact]
    public void IsCommandRegistered_Should_Return_True_For_Existing_Command()
    {
        // Act & Assert
        _commandFactory.IsCommandRegistered("REQUEST").Should().BeTrue();
        _commandFactory.IsCommandRegistered("request").Should().BeTrue(); // Case insensitive
        _commandFactory.IsCommandRegistered("NONEXISTENT").Should().BeFalse();
        _commandFactory.IsCommandRegistered("").Should().BeFalse();
        _commandFactory.IsCommandRegistered(null!).Should().BeFalse();
    }

    [Fact]
    public void CreateCommand_Should_Return_Null_For_Unknown_Command()
    {
        // Act
        var command = _commandFactory.CreateCommand("UNKNOWN_COMMAND");

        // Assert
        command.Should().BeNull();
    }

    [Fact]
    public void CreateCommand_Should_Throw_For_Null_Or_Empty_Name()
    {
        // Act & Assert
        var act1 = () => _commandFactory.CreateCommand(null!);
        var act2 = () => _commandFactory.CreateCommand("");

        act1.Should().Throw<ArgumentException>();
        act2.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RegisterCommand_Generic_Should_Register_Command_Type()
    {
        // Act
        _commandFactory.RegisterCommand<TestCommand>("TEST");

        // Assert
        _commandFactory.IsCommandRegistered("TEST").Should().BeTrue();
        _commandFactory.GetAvailableCommands().Should().Contain("TEST");
    }

    [Fact]
    public void RegisterCommand_Instance_Should_Register_Command_Instance()
    {
        // Arrange
        var testCommand = new TestCommand();

        // Act
        _commandFactory.RegisterCommand("TESTINSTANCE", testCommand);

        // Assert
        _commandFactory.IsCommandRegistered("TESTINSTANCE").Should().BeTrue();
        _commandFactory.GetAvailableCommands().Should().Contain("TESTINSTANCE");
        
        var createdCommand = _commandFactory.CreateCommand("TESTINSTANCE");
        createdCommand.Should().BeSameAs(testCommand);
    }

    [Fact]
    public void RegisterCommand_Should_Throw_For_Invalid_Arguments()
    {
        // Arrange
        var testCommand = new TestCommand();

        // Act & Assert
        var act1 = () => _commandFactory.RegisterCommand<TestCommand>(null!);
        var act2 = () => _commandFactory.RegisterCommand<TestCommand>("");
        var act3 = () => _commandFactory.RegisterCommand("TEST", null!);
        var act4 = () => _commandFactory.RegisterCommand(null!, testCommand);

        act1.Should().Throw<ArgumentException>();
        act2.Should().Throw<ArgumentException>();
        act3.Should().Throw<ArgumentNullException>();
        act4.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetCommandMetadata_Should_Return_Metadata_For_Existing_Command()
    {
        // Act
        var metadata = _commandFactory.GetCommandMetadata("REQUEST");

        // Assert
        metadata.Should().NotBeNull();
        metadata!.Name.Should().Be("REQUEST");
        metadata.Description.Should().NotBeEmpty();
        metadata.Category.Should().Be("HTTP");
        metadata.Parameters.Should().NotBeEmpty();
        metadata.Examples.Should().NotBeEmpty();
    }

    [Fact]
    public void GetCommandMetadata_Should_Return_Null_For_Unknown_Command()
    {
        // Act
        var metadata = _commandFactory.GetCommandMetadata("UNKNOWN");

        // Assert
        metadata.Should().BeNull();
    }

    [Fact]
    public void GetCommandMetadata_Should_Return_Null_For_Invalid_Input()
    {
        // Act & Assert
        _commandFactory.GetCommandMetadata(null!).Should().BeNull();
        _commandFactory.GetCommandMetadata("").Should().BeNull();
    }

    [Fact]
    public void RegisterCommandMetadata_Should_Store_Metadata()
    {
        // Arrange
        var metadata = new CommandMetadata
        {
            Name = "CUSTOM",
            Description = "Custom test command",
            Category = "Test",
            Parameters = new List<CommandParameter>
            {
                new() { Name = "param1", Type = ParameterType.String, Required = true }
            }
        };

        // Act
        _commandFactory.RegisterCommandMetadata("CUSTOM", metadata);

        // Assert
        var retrievedMetadata = _commandFactory.GetCommandMetadata("CUSTOM");
        retrievedMetadata.Should().NotBeNull();
        retrievedMetadata.Should().BeSameAs(metadata);
    }

    [Fact]
    public void RegisterCommandMetadata_Should_Throw_For_Invalid_Arguments()
    {
        // Arrange
        var metadata = new CommandMetadata { Name = "TEST" };

        // Act & Assert
        var act1 = () => _commandFactory.RegisterCommandMetadata(null!, metadata);
        var act2 = () => _commandFactory.RegisterCommandMetadata("", metadata);
        var act3 = () => _commandFactory.RegisterCommandMetadata("TEST", null!);

        act1.Should().Throw<ArgumentException>();
        act2.Should().Throw<ArgumentException>();
        act3.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateCommand_Should_Handle_Construction_Errors()
    {
        // Arrange - Register a command type that will fail to construct
        _commandFactory.RegisterCommand<FailingTestCommand>("FAILING");

        // Act
        var command = _commandFactory.CreateCommand("FAILING");

        // Assert
        command.Should().BeNull(); // Should handle construction failure gracefully
    }

    [Fact]
    public void Case_Insensitive_Operations_Should_Work()
    {
        // Arrange
        _commandFactory.RegisterCommand<TestCommand>("CaseTest");

        // Act & Assert
        _commandFactory.IsCommandRegistered("CASETEST").Should().BeTrue();
        _commandFactory.IsCommandRegistered("casetest").Should().BeTrue();
        _commandFactory.IsCommandRegistered("CaseTest").Should().BeTrue();

        var command1 = _commandFactory.CreateCommand("CASETEST");
        var command2 = _commandFactory.CreateCommand("casetest");

        command1.Should().NotBeNull();
        command2.Should().NotBeNull();
    }

    [Fact]
    public async Task Concurrent_Access_Should_Be_Thread_Safe()
    {
        // Arrange
        const int threadCount = 10;
        const int operationsPerThread = 100;
        var tasks = new List<Task>();
        var commands = new List<IScriptCommand>();

        // Act
        for (int t = 0; t < threadCount; t++)
        {
            var threadId = t;
            tasks.Add(Task.Run(() =>
            {
                for (int i = 0; i < operationsPerThread; i++)
                {
                    var commandName = $"THREAD{threadId}_CMD{i}";
                    _commandFactory.RegisterCommand<TestCommand>(commandName);
                    
                    var command = _commandFactory.CreateCommand(commandName);
                    if (command != null)
                    {
                        lock (commands)
                        {
                            commands.Add(command);
                        }
                    }
                    
                    var isRegistered = _commandFactory.IsCommandRegistered(commandName);
                    isRegistered.Should().BeTrue();
                }
            }));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        var availableCommands = _commandFactory.GetAvailableCommands().ToList();
        availableCommands.Count.Should().BeGreaterThan(threadCount * operationsPerThread);
        commands.Count.Should().Be(threadCount * operationsPerThread);
    }

    public void Dispose()
    {
        // No resources to dispose in this test
    }

    /// <summary>
    /// Test command for testing purposes
    /// </summary>
    private class TestCommand : IScriptCommand
    {
        public string CommandName => "TEST";
        public string Description => "Test command for unit tests";

        public Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, OpenBullet.Core.Models.BotData botData)
        {
            return Task.FromResult(new CommandResult { Success = true });
        }

        public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
        {
            return new CommandValidationResult { IsValid = true };
        }
    }

    /// <summary>
    /// Test command that fails during construction
    /// </summary>
    private class FailingTestCommand : IScriptCommand
    {
        public string CommandName => "FAILING";
        public string Description => "Failing test command";

        public FailingTestCommand()
        {
            throw new InvalidOperationException("This command always fails to construct");
        }

        public Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, OpenBullet.Core.Models.BotData botData)
        {
            return Task.FromResult(new CommandResult { Success = false });
        }

        public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
        {
            return new CommandValidationResult { IsValid = false };
        }
    }
}

/// <summary>
/// Step 6 Command Metadata Tests
/// </summary>
public class Step6_CommandMetadataTests
{
    [Fact]
    public void CommandMetadata_Should_Initialize_With_Defaults()
    {
        // Act
        var metadata = new CommandMetadata();

        // Assert
        metadata.Name.Should().BeEmpty();
        metadata.Description.Should().BeEmpty();
        metadata.Category.Should().BeEmpty();
        metadata.Parameters.Should().NotBeNull().And.BeEmpty();
        metadata.Examples.Should().NotBeNull().And.BeEmpty();
        metadata.Syntax.Should().BeEmpty();
        metadata.RequiresVariableSubstitution.Should().BeTrue();
        metadata.CanHaveSubcommands.Should().BeFalse();
    }

    [Fact]
    public void CommandParameter_Should_Initialize_With_Defaults()
    {
        // Act
        var parameter = new CommandParameter();

        // Assert
        parameter.Name.Should().BeEmpty();
        parameter.Description.Should().BeEmpty();
        parameter.Type.Should().Be(ParameterType.String);
        parameter.Required.Should().BeTrue();
        parameter.DefaultValue.Should().BeNull();
        parameter.AllowedValues.Should().NotBeNull().And.BeEmpty();
        parameter.Pattern.Should().BeNull();
    }

    [Theory]
    [InlineData(ParameterType.String)]
    [InlineData(ParameterType.Integer)]
    [InlineData(ParameterType.Boolean)]
    [InlineData(ParameterType.Url)]
    [InlineData(ParameterType.HttpMethod)]
    [InlineData(ParameterType.ContentType)]
    [InlineData(ParameterType.FilePath)]
    [InlineData(ParameterType.Variable)]
    [InlineData(ParameterType.Enum)]
    public void ParameterType_Values_Should_Be_Available(ParameterType parameterType)
    {
        // Assert
        Enum.IsDefined(typeof(ParameterType), parameterType).Should().BeTrue();
    }

    [Fact]
    public void CommandMetadata_Can_Be_Configured()
    {
        // Arrange & Act
        var metadata = new CommandMetadata
        {
            Name = "TEST_COMMAND",
            Description = "Test command description",
            Category = "Testing",
            Syntax = "TEST_COMMAND <arg1> [arg2]",
            RequiresVariableSubstitution = false,
            CanHaveSubcommands = true,
            Parameters = new List<CommandParameter>
            {
                new()
                {
                    Name = "arg1",
                    Description = "First argument",
                    Type = ParameterType.String,
                    Required = true
                },
                new()
                {
                    Name = "arg2",
                    Description = "Second argument",
                    Type = ParameterType.Integer,
                    Required = false,
                    DefaultValue = 0,
                    AllowedValues = { "0", "1", "2" }
                }
            },
            Examples = { "TEST_COMMAND hello", "TEST_COMMAND hello 42" }
        };

        // Assert
        metadata.Name.Should().Be("TEST_COMMAND");
        metadata.Description.Should().Be("Test command description");
        metadata.Category.Should().Be("Testing");
        metadata.RequiresVariableSubstitution.Should().BeFalse();
        metadata.CanHaveSubcommands.Should().BeTrue();
        metadata.Parameters.Should().HaveCount(2);
        metadata.Examples.Should().HaveCount(2);

        var param1 = metadata.Parameters[0];
        param1.Name.Should().Be("arg1");
        param1.Required.Should().BeTrue();
        param1.Type.Should().Be(ParameterType.String);

        var param2 = metadata.Parameters[1];
        param2.Name.Should().Be("arg2");
        param2.Required.Should().BeFalse();
        param2.DefaultValue.Should().Be(0);
        param2.AllowedValues.Should().Contain("0", "1", "2");
    }
}
