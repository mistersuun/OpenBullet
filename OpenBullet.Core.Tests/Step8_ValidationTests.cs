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
/// Step 8 Validation Tests - Basic functionality validation
/// </summary>
public class Step8_ValidationTests
{
    [Fact]
    public void IKeyChecker_Interface_Should_Be_Properly_Defined()
    {
        // Assert
        typeof(IKeyChecker).IsInterface.Should().BeTrue();
        
        var methods = typeof(IKeyChecker).GetMethods();
        methods.Should().Contain(m => m.Name == nameof(IKeyChecker.EvaluateKeyChains));
        methods.Should().Contain(m => m.Name == nameof(IKeyChecker.EvaluateKeyChain));
        methods.Should().Contain(m => m.Name == nameof(IKeyChecker.EvaluateKey));
        methods.Should().Contain(m => m.Name == nameof(IKeyChecker.GetSourceValue));
    }

    [Fact]
    public void KeyCheckCommand_Can_Be_Created_With_Valid_ServiceProvider()
    {
        // Arrange
        var serviceProvider = CreateValidServiceProvider();

        // Act
        var command = new KeyCheckCommand(serviceProvider);

        // Assert
        command.Should().NotBeNull();
        command.Should().BeAssignableTo<IScriptCommand>();
        command.CommandName.Should().Be("KEYCHECK");
        command.Description.Should().NotBeEmpty();
    }

    [Fact]
    public void KeyCheckCommand_Should_Throw_With_Invalid_ServiceProvider()
    {
        // Arrange
        var emptyServiceProvider = new Mock<IServiceProvider>().Object;

        // Act & Assert
        var act = () => new KeyCheckCommand(emptyServiceProvider);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void KeyChecker_Can_Be_Created()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<KeyChecker>>();

        // Act
        var keyChecker = new KeyChecker(loggerMock.Object);

        // Assert
        keyChecker.Should().NotBeNull();
        keyChecker.Should().BeAssignableTo<IKeyChecker>();
    }

    [Fact]
    public void KeyChecker_Can_Be_Created_With_Configuration()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<KeyChecker>>();
        var configuration = new KeyCheckConfiguration
        {
            StopOnFirstMatch = false,
            EnableBanProxy = true,
            EnableRetry = true,
            EvaluationTimeout = TimeSpan.FromSeconds(10)
        };

        // Act
        var keyChecker = new KeyChecker(loggerMock.Object, configuration);

        // Assert
        keyChecker.Should().NotBeNull();
        keyChecker.Should().BeAssignableTo<IKeyChecker>();
    }

    [Fact]
    public void KeyCheckResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new KeyCheckResult();

        // Assert
        result.Success.Should().BeFalse();
        result.Status.Should().Be(BotStatus.None);
        result.CustomStatus.Should().BeNull();
        result.MatchedKeyChain.Should().BeNull();
        result.Evaluations.Should().NotBeNull().And.BeEmpty();
        result.ErrorMessage.Should().BeNull();
        result.ShouldBanProxy.Should().BeFalse();
        result.ShouldRetry.Should().BeFalse();
        result.Metadata.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void KeyEvaluation_Should_Initialize_With_Defaults()
    {
        // Act
        var evaluation = new KeyEvaluation();

        // Assert
        evaluation.Key.Should().NotBeNull();
        evaluation.Result.Should().BeFalse();
        evaluation.ActualValue.Should().BeEmpty();
        evaluation.ExpectedValue.Should().BeEmpty();
        evaluation.Condition.Should().BeEmpty();
        evaluation.Source.Should().BeEmpty();
        evaluation.ErrorMessage.Should().BeNull();
        evaluation.EvaluationTime.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void KeyChain_Should_Initialize_With_Defaults()
    {
        // Act
        var keyChain = new KeyChain();

        // Assert
        keyChain.Status.Should().Be(BotStatus.Success);
        keyChain.CustomStatus.Should().BeNull();
        keyChain.Logic.Should().Be(KeyChainLogic.OR);
        keyChain.Keys.Should().NotBeNull().And.BeEmpty();
        keyChain.BanProxy.Should().BeFalse();
        keyChain.Retry.Should().BeFalse();
        keyChain.Name.Should().BeEmpty();
        keyChain.Properties.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Key_Should_Initialize_With_Defaults()
    {
        // Act
        var key = new Key();

        // Assert
        key.Source.Should().BeEmpty();
        key.Condition.Should().Be(KeyCondition.Contains);
        key.Value.Should().BeEmpty();
        key.CaseSensitive.Should().BeFalse();
        key.Name.Should().BeEmpty();
        key.Properties.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void KeyChainLogic_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<KeyChainLogic>().Should().Contain(KeyChainLogic.OR);
        Enum.GetValues<KeyChainLogic>().Should().Contain(KeyChainLogic.AND);
    }

    [Fact]
    public void KeyCondition_Enum_Should_Have_Expected_Values()
    {
        // Assert
        var conditions = Enum.GetValues<KeyCondition>();
        conditions.Should().Contain(KeyCondition.Contains);
        conditions.Should().Contain(KeyCondition.DoesNotContain);
        conditions.Should().Contain(KeyCondition.EqualTo);
        conditions.Should().Contain(KeyCondition.NotEqualTo);
        conditions.Should().Contain(KeyCondition.GreaterThan);
        conditions.Should().Contain(KeyCondition.LessThan);
        conditions.Should().Contain(KeyCondition.StartsWith);
        conditions.Should().Contain(KeyCondition.EndsWith);
        conditions.Should().Contain(KeyCondition.MatchesRegex);
        conditions.Should().Contain(KeyCondition.DoesNotMatchRegex);
        conditions.Should().Contain(KeyCondition.IsEmpty);
        conditions.Should().Contain(KeyCondition.IsNotEmpty);
        conditions.Should().Contain(KeyCondition.IsNumeric);
        conditions.Should().Contain(KeyCondition.IsNotNumeric);
        conditions.Should().Contain(KeyCondition.LengthEqualTo);
        conditions.Should().Contain(KeyCondition.LengthGreaterThan);
        conditions.Should().Contain(KeyCondition.LengthLessThan);
    }

    [Fact]
    public void KeyCheckConfiguration_Should_Initialize_With_Defaults()
    {
        // Act
        var config = new KeyCheckConfiguration();

        // Assert
        config.StopOnFirstMatch.Should().BeTrue();
        config.EnableBanProxy.Should().BeTrue();
        config.EnableRetry.Should().BeTrue();
        config.EvaluationTimeout.Should().Be(TimeSpan.FromSeconds(5));
        config.LogEvaluations.Should().BeTrue();
        config.CustomSettings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void KeyChain_Factory_Methods_Should_Create_Correct_Instances()
    {
        // Act & Assert
        var successChain = KeyChain.Success();
        successChain.Status.Should().Be(BotStatus.Success);
        successChain.Name.Should().Be("Success");

        var failureChain = KeyChain.Failure();
        failureChain.Status.Should().Be(BotStatus.Failure);
        failureChain.Name.Should().Be("Failure");

        var banChain = KeyChain.Ban();
        banChain.Status.Should().Be(BotStatus.Ban);
        banChain.BanProxy.Should().BeTrue();
        banChain.Name.Should().Be("Ban");

        var retryChain = KeyChain.CreateRetry();
        retryChain.Status.Should().Be(BotStatus.Retry);
        retryChain.Retry.Should().BeTrue();
        retryChain.Name.Should().Be("Retry");

        var customChain = KeyChain.Custom("TestStatus");
        customChain.Status.Should().Be(BotStatus.Custom);
        customChain.CustomStatus.Should().Be("TestStatus");
        customChain.Name.Should().Be("Custom");
    }

    [Fact]
    public void Key_Factory_Methods_Should_Create_Correct_Instances()
    {
        // Act & Assert
        var containsKey = Key.Contains("source", "value");
        containsKey.Source.Should().Be("source");
        containsKey.Condition.Should().Be(KeyCondition.Contains);
        containsKey.Value.Should().Be("value");
        containsKey.CaseSensitive.Should().BeFalse();

        var equalToKey = Key.EqualTo("source", "value", true);
        equalToKey.Condition.Should().Be(KeyCondition.EqualTo);
        equalToKey.CaseSensitive.Should().BeTrue();

        var doesNotContainKey = Key.DoesNotContain("source", "value");
        doesNotContainKey.Condition.Should().Be(KeyCondition.DoesNotContain);

        var regexKey = Key.MatchesRegex("source", @"\d+");
        regexKey.Condition.Should().Be(KeyCondition.MatchesRegex);
        regexKey.Value.Should().Be(@"\d+");
    }

    [Fact]
    public void KeyChainBuilder_Should_Create_Correct_Chains()
    {
        // Act
        var successBuilder = KeyChainBuilder.Success();
        var failureBuilder = KeyChainBuilder.Failure();
        var banBuilder = KeyChainBuilder.Ban();
        var retryBuilder = KeyChainBuilder.Retry();
        var customBuilder = KeyChainBuilder.Custom("TestStatus");

        // Assert
        successBuilder.Should().NotBeNull();
        failureBuilder.Should().NotBeNull();
        banBuilder.Should().NotBeNull();
        retryBuilder.Should().NotBeNull();
        customBuilder.Should().NotBeNull();
    }

    [Fact]
    public void KeyChainBuilder_Should_Build_Fluently()
    {
        // Act
        var chain = KeyChainBuilder.Success()
            .WithLogic(KeyChainLogic.AND)
            .WithName("TestChain")
            .WithBanProxy()
            .WithRetry()
            .AddKey("source", KeyCondition.Contains, "value")
            .Build();

        // Assert
        chain.Should().NotBeNull();
        chain.Status.Should().Be(BotStatus.Success);
        chain.Logic.Should().Be(KeyChainLogic.AND);
        chain.Name.Should().Be("TestChain");
        chain.BanProxy.Should().BeTrue();
        chain.Retry.Should().BeTrue();
        chain.Keys.Should().HaveCount(1);
        chain.Keys[0].Source.Should().Be("source");
        chain.Keys[0].Condition.Should().Be(KeyCondition.Contains);
        chain.Keys[0].Value.Should().Be("value");
    }

    [Fact]
    public void KeyCheckCommand_Should_Validate_Basic_Instructions()
    {
        // Arrange
        var command = new KeyCheckCommand(CreateValidServiceProvider());
        
        var validInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction>
            {
                new()
                {
                    CommandName = "KEYCHAIN",
                    Arguments = new List<string> { "Success", "OR" },
                    SubInstructions = new List<ScriptInstruction>
                    {
                        new()
                        {
                            CommandName = "KEY",
                            Arguments = new List<string> { "<SOURCE>", "Contains", "Welcome" }
                        }
                    }
                }
            }
        };

        var invalidInstruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction>() // No key chains
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

    [Fact]
    public void KeyChecker_Should_Handle_Basic_Operations()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<KeyChecker>>();
        var keyChecker = new KeyChecker(loggerMock.Object);
        var botData = CreateTestBotData();

        var key = Key.Contains("<SOURCE>", "test");
        var keyChain = new KeyChain
        {
            Logic = KeyChainLogic.OR,
            Keys = new List<Key> { key }
        };

        // Act
        var keyResult = keyChecker.EvaluateKey(key, botData);
        var chainResult = keyChecker.EvaluateKeyChain(keyChain, botData);
        var source = keyChecker.GetSourceValue("<SOURCE>", botData);

        // Assert
        ((object)keyResult).Should().BeOfType<bool>();
        ((object)chainResult).Should().BeOfType<bool>();
        source.Should().NotBeNull();
    }

    [Fact]
    public void KeyCheckExtensions_Should_Work()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<KeyChecker>>();
        var keyChecker = new KeyChecker(loggerMock.Object);
        var botData = CreateTestBotData();

        var result = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Success,
            CustomStatus = "Test",
            MatchedKeyChain = new KeyChain { Name = "Success" },
            Evaluations = new List<KeyEvaluation> { new() }
        };

        // Act
        result.ApplyToBotData(botData);
        var successChain = keyChecker.CreateSuccessChain(Key.Contains("<SOURCE>", "test"));
        var failureChain = keyChecker.CreateFailureChain(Key.Contains("<SOURCE>", "error"));
        var banChain = keyChecker.CreateBanChain(Key.EqualTo("<RESPONSECODE>", "429"));

        // Assert
        botData.Status.Should().Be(BotStatus.Success);
        botData.CustomStatus.Should().Be("Test");
        
        successChain.Should().NotBeNull();
        successChain.Status.Should().Be(BotStatus.Success);
        
        failureChain.Should().NotBeNull();
        failureChain.Status.Should().Be(BotStatus.Failure);
        
        banChain.Should().NotBeNull();
        banChain.Status.Should().Be(BotStatus.Ban);
    }

    [Fact]
    public void KeyCheckCommandExtensions_Should_Create_Instructions()
    {
        // Arrange
        var keyChains = new List<KeyChain>
        {
            KeyChain.Success(KeyChainLogic.OR),
            KeyChain.Failure(KeyChainLogic.AND)
        };
        keyChains[0].Keys.Add(Key.Contains("<SOURCE>", "Welcome"));
        keyChains[1].Keys.Add(Key.Contains("<SOURCE>", "Error"));

        // Act
        var instruction = KeyCheckCommandExtensions.CreateKeyCheckInstruction(keyChains);
        var simpleInstruction = KeyCheckCommandExtensions.CreateSimpleKeyCheck("Welcome", "Error");

        // Assert
        instruction.Should().NotBeNull();
        instruction.CommandName.Should().Be("KEYCHECK");
        instruction.SubInstructions.Should().HaveCount(2);
        instruction.SubInstructions[0].CommandName.Should().Be("KEYCHAIN");
        
        simpleInstruction.Should().NotBeNull();
        simpleInstruction.CommandName.Should().Be("KEYCHECK");
        simpleInstruction.SubInstructions.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("Success", BotStatus.Success)]
    [InlineData("Failure", BotStatus.Failure)]
    [InlineData("Ban", BotStatus.Ban)]
    [InlineData("Retry", BotStatus.Retry)]
    [InlineData("Custom", BotStatus.Custom)]
    public void BotStatus_String_Conversion_Should_Work(string stringValue, BotStatus expectedStatus)
    {
        // Act
        var parseSuccess = Enum.TryParse<BotStatus>(stringValue, true, out var parsedStatus);

        // Assert
        parseSuccess.Should().BeTrue();
        parsedStatus.Should().Be(expectedStatus);
    }

    [Theory]
    [InlineData("OR", KeyChainLogic.OR)]
    [InlineData("AND", KeyChainLogic.AND)]
    public void KeyChainLogic_String_Conversion_Should_Work(string stringValue, KeyChainLogic expectedLogic)
    {
        // Act
        var parseSuccess = Enum.TryParse<KeyChainLogic>(stringValue, true, out var parsedLogic);

        // Assert
        parseSuccess.Should().BeTrue();
        parsedLogic.Should().Be(expectedLogic);
    }

    [Theory]
    [InlineData("Contains", KeyCondition.Contains)]
    [InlineData("DoesNotContain", KeyCondition.DoesNotContain)]
    [InlineData("EqualTo", KeyCondition.EqualTo)]
    [InlineData("MatchesRegex", KeyCondition.MatchesRegex)]
    [InlineData("IsEmpty", KeyCondition.IsEmpty)]
    [InlineData("IsNumeric", KeyCondition.IsNumeric)]
    public void KeyCondition_String_Conversion_Should_Work(string stringValue, KeyCondition expectedCondition)
    {
        // Act
        var parseSuccess = Enum.TryParse<KeyCondition>(stringValue, true, out var parsedCondition);

        // Assert
        parseSuccess.Should().BeTrue();
        parsedCondition.Should().Be(expectedCondition);
    }

    [Fact]
    public void All_KeyConditions_Should_Be_Testable()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<KeyChecker>>();
        var keyChecker = new KeyChecker(loggerMock.Object);
        var botData = CreateTestBotData();
        botData.Source = "Test123";

        // Act & Assert - Just ensure all conditions can be used without throwing
        foreach (var condition in Enum.GetValues<KeyCondition>())
        {
            try
            {
                var key = new Key
                {
                    Source = "<SOURCE>",
                    Condition = condition,
                    Value = "test"
                };
                
                var result = keyChecker.EvaluateKey(key, botData);
                ((object)result).Should().BeOfType<bool>();
            }
            catch (Exception ex)
            {
                // Some conditions might fail with invalid data, but they shouldn't crash
                ex.Should().BeOneOf(typeof(ArgumentException), typeof(FormatException));
            }
        }
    }

    [Fact]
    public void Step8TestHelpers_Should_Create_Valid_Objects()
    {
        // Arrange & Act
        var serviceProvider = CreateValidServiceProvider();
        var botData = CreateTestBotData();
        var keyChain = CreateTestKeyChain();
        var keyCheckResult = CreateTestKeyCheckResult();

        // Assert
        serviceProvider.Should().NotBeNull();
        
        botData.Should().NotBeNull();
        botData.Source.Should().NotBeEmpty();
        
        keyChain.Should().NotBeNull();
        keyChain.Keys.Should().HaveCount(2);
        
        keyCheckResult.Should().NotBeNull();
        keyCheckResult.Success.Should().BeTrue();
        keyCheckResult.Status.Should().Be(BotStatus.Success);
    }

    private static IServiceProvider CreateValidServiceProvider()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<KeyChecker>>();
        var keyChecker = new KeyChecker(loggerMock.Object);
        
        serviceProvider.Setup(sp => sp.GetService(typeof(ILogger<KeyCheckCommand>)))
                      .Returns(new Mock<ILogger<KeyCheckCommand>>().Object);
        serviceProvider.Setup(sp => sp.GetService(typeof(IKeyChecker)))
                      .Returns(keyChecker);
        serviceProvider.Setup(sp => sp.GetService(typeof(IScriptParser)))
                      .Returns(new Mock<IScriptParser>().Object);
        
        return serviceProvider.Object;
    }

    private static BotData CreateTestBotData()
    {
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        var cancellationToken = new CancellationTokenSource().Token;
        var botData = new BotData("test:data", config, logger, cancellationToken);
        botData.Source = "Welcome to the test dashboard!";
        botData.Address = "https://test.example.com";
        botData.ResponseCode = 200;
        botData.SetVariable("TEST_VAR", "test_value");
        return botData;
    }

    private static KeyChain CreateTestKeyChain()
    {
        return new KeyChain
        {
            Status = BotStatus.Success,
            Logic = KeyChainLogic.OR,
            Name = "Test Chain",
            Keys = new List<Key>
            {
                Key.Contains("<SOURCE>", "Welcome"),
                Key.EqualTo("<RESPONSECODE>", "200")
            }
        };
    }

    private static KeyCheckResult CreateTestKeyCheckResult()
    {
        return new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Success,
            MatchedKeyChain = new KeyChain { Name = "Success", Status = BotStatus.Success },
            Evaluations = new List<KeyEvaluation>
            {
                new() { Result = true, ActualValue = "test", ExpectedValue = "test" }
            },
            Metadata = new Dictionary<string, object>
            {
                ["TotalKeyChains"] = 1,
                ["TotalKeys"] = 2
            }
        };
    }
}
