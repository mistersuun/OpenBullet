using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Commands;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.KeyChecking;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 8 Tests: KEYCHECK Command Implementation
/// </summary>
public class Step8_KeyCheckCommandTests : IDisposable
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<KeyCheckCommand>> _loggerMock;
    private readonly Mock<IKeyChecker> _keyCheckerMock;
    private readonly Mock<IScriptParser> _scriptParserMock;
    private readonly KeyCheckCommand _keyCheckCommand;
    private readonly BotData _botData;

    public Step8_KeyCheckCommandTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<KeyCheckCommand>>();
        _keyCheckerMock = new Mock<IKeyChecker>();
        _scriptParserMock = new Mock<IScriptParser>();

        // Setup service provider
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<KeyCheckCommand>)))
                           .Returns(_loggerMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IKeyChecker)))
                           .Returns(_keyCheckerMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IScriptParser)))
                           .Returns(_scriptParserMock.Object);

        _keyCheckCommand = new KeyCheckCommand(_serviceProviderMock.Object);

        // Create test bot data
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        var cancellationToken = new CancellationTokenSource().Token;
        _botData = new BotData("test:data", config, logger, cancellationToken);
        _botData.Source = "Welcome to the dashboard! Login successful.";
        _botData.Address = "https://example.com/dashboard";
        _botData.ResponseCode = 200;

        // Setup script parser default behavior
        _scriptParserMock.Setup(sp => sp.SubstituteVariables(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                        .Returns<string, Dictionary<string, object>>((input, _) => input);
    }

    [Fact]
    public void KeyCheckCommand_Should_Have_Correct_Properties()
    {
        // Assert
        _keyCheckCommand.CommandName.Should().Be("KEYCHECK");
        _keyCheckCommand.Description.Should().NotBeEmpty();
        _keyCheckCommand.Should().BeAssignableTo<IScriptCommand>();
    }

    [Fact]
    public async Task ExecuteAsync_With_Success_KeyChain_Should_Set_Success_Status()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "Welcome" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction },
            LineNumber = 1
        };

        var keyCheckResult = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Success,
            MatchedKeyChain = new KeyChain { Name = "Success", Status = BotStatus.Success }
        };

        _keyCheckerMock.Setup(kc => kc.EvaluateKeyChains(It.IsAny<List<KeyChain>>(), _botData))
                      .Returns(keyCheckResult);

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.NewStatus.Should().Be(BotStatus.Success);
        _botData.Status.Should().Be(BotStatus.Success);
        
        _keyCheckerMock.Verify(kc => kc.EvaluateKeyChains(
            It.Is<List<KeyChain>>(chains => chains.Count == 1 && chains[0].Status == BotStatus.Success), 
            _botData), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_Failure_KeyChain_Should_Set_Failure_Status()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Failure", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "Error" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction },
            LineNumber = 1
        };

        var keyCheckResult = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Failure,
            MatchedKeyChain = new KeyChain { Name = "Failure", Status = BotStatus.Failure }
        };

        _keyCheckerMock.Setup(kc => kc.EvaluateKeyChains(It.IsAny<List<KeyChain>>(), _botData))
                      .Returns(keyCheckResult);

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.NewStatus.Should().Be(BotStatus.Failure);
        _botData.Status.Should().Be(BotStatus.Failure);
    }

    [Fact]
    public async Task ExecuteAsync_With_Ban_KeyChain_Should_Set_Ban_Status_And_Flag()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Ban", "AND" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<RESPONSECODE>", "EqualTo", "429" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction },
            LineNumber = 1
        };

        var keyCheckResult = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Ban,
            ShouldBanProxy = true,
            MatchedKeyChain = new KeyChain { Name = "Ban", Status = BotStatus.Ban, BanProxy = true }
        };

        _keyCheckerMock.Setup(kc => kc.EvaluateKeyChains(It.IsAny<List<KeyChain>>(), _botData))
                      .Returns(keyCheckResult);

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.NewStatus.Should().Be(BotStatus.Ban);
        result.CapturedData.Should().ContainKey("BanProxy");
        result.CapturedData["BanProxy"].Should().Be(true);
        _botData.Status.Should().Be(BotStatus.Ban);
    }

    [Fact]
    public async Task ExecuteAsync_With_Custom_Status_Should_Set_Custom_Status()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Custom", "OR", "Premium User" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "Premium" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction },
            LineNumber = 1
        };

        var keyCheckResult = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Custom,
            CustomStatus = "Premium User",
            MatchedKeyChain = new KeyChain { Name = "Custom", Status = BotStatus.Custom, CustomStatus = "Premium User" }
        };

        _keyCheckerMock.Setup(kc => kc.EvaluateKeyChains(It.IsAny<List<KeyChain>>(), _botData))
                      .Returns(keyCheckResult);

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.NewStatus.Should().Be(BotStatus.Custom);
        result.CustomStatus.Should().Be("Premium User");
        _botData.Status.Should().Be(BotStatus.Custom);
        _botData.CustomStatus.Should().Be("Premium User");
    }

    [Fact]
    public async Task ExecuteAsync_With_Multiple_KeyChains_Should_Evaluate_All()
    {
        // Arrange
        var successChain = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "Welcome" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var failureChain = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Failure", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "Error" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { successChain, failureChain },
            LineNumber = 1
        };

        var keyCheckResult = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Success,
            MatchedKeyChain = new KeyChain { Name = "Success", Status = BotStatus.Success }
        };

        _keyCheckerMock.Setup(kc => kc.EvaluateKeyChains(It.IsAny<List<KeyChain>>(), _botData))
                      .Returns(keyCheckResult);

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        
        _keyCheckerMock.Verify(kc => kc.EvaluateKeyChains(
            It.Is<List<KeyChain>>(chains => chains.Count == 2), 
            _botData), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_Variable_Substitution_Should_Replace_Variables()
    {
        // Arrange
        _botData.SetVariable("ERROR_TEXT", "Invalid credentials");

        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Failure", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "<ERROR_TEXT>" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction },
            LineNumber = 1
        };

        _scriptParserMock.Setup(sp => sp.SubstituteVariables("<ERROR_TEXT>", It.IsAny<Dictionary<string, object>>()))
                        .Returns("Invalid credentials");

        var keyCheckResult = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Failure,
            MatchedKeyChain = new KeyChain { Name = "Failure", Status = BotStatus.Failure }
        };

        _keyCheckerMock.Setup(kc => kc.EvaluateKeyChains(It.IsAny<List<KeyChain>>(), _botData))
                      .Returns(keyCheckResult);

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _scriptParserMock.Verify(sp => sp.SubstituteVariables("<ERROR_TEXT>", It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_With_No_KeyChains_Should_Return_Error()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction>(), // No key chains
            LineNumber = 1
        };

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("No key chains defined in KEYCHECK command");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Handle_KeyChecker_Exception()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "Welcome" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction },
            LineNumber = 1
        };

        _keyCheckerMock.Setup(kc => kc.EvaluateKeyChains(It.IsAny<List<KeyChain>>(), _botData))
                      .Throws(new InvalidOperationException("Test exception"));

        // Act
        var result = await _keyCheckCommand.ExecuteAsync(instruction, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Test exception");
        _botData.Status.Should().Be(BotStatus.Error);
    }

    [Fact]
    public void ValidateInstruction_With_Valid_Instruction_Should_Pass()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "Welcome" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateInstruction_With_No_KeyChains_Should_Fail()
    {
        // Arrange
        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction>() // No key chains
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("KEYCHECK command must contain KEYCHAIN blocks");
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_Status_Should_Fail()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "InvalidStatus", "OR" },
            SubInstructions = new List<ScriptInstruction>()
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Invalid KEYCHAIN status: InvalidStatus"));
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_Logic_Should_Fail()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", "INVALID_LOGIC" },
            SubInstructions = new List<ScriptInstruction>()
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Invalid KEYCHAIN logic: INVALID_LOGIC"));
    }

    [Fact]
    public void ValidateInstruction_With_Invalid_Key_Should_Fail()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "InvalidCondition", "Welcome" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Invalid KEY condition: InvalidCondition"));
    }

    [Fact]
    public void ValidateInstruction_With_Insufficient_Key_Arguments_Should_Fail()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains" }, // Missing value
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("KEY must have SOURCE, CONDITION, and VALUE arguments");
    }

    [Fact]
    public void ValidateInstruction_Without_Success_Or_Failure_Chain_Should_Warn()
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Ban", "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<RESPONSECODE>", "EqualTo", "429" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Warnings.Should().Contain("KEYCHECK should have at least one Success or Failure key chain");
    }

    [Theory]
    [InlineData("Success")]
    [InlineData("Failure")]
    [InlineData("Ban")]
    [InlineData("Retry")]
    [InlineData("Custom")]
    public void ValidateInstruction_With_Valid_Statuses_Should_Pass(string status)
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { status, "OR" },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "test" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("OR")]
    [InlineData("AND")]
    public void ValidateInstruction_With_Valid_Logic_Should_Pass(string logic)
    {
        // Arrange
        var keyChainInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHAIN",
            Arguments = new List<string> { "Success", logic },
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { "<SOURCE>", "Contains", "test" },
                    Parameters = new Dictionary<string, object>()
                }
            }
        };

        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction> { keyChainInstruction }
        };

        // Act
        var result = _keyCheckCommand.ValidateInstruction(instruction);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    public void Dispose()
    {
        // No resources to dispose in this test class
    }
}
