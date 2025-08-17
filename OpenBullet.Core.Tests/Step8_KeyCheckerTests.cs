using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.KeyChecking;
using OpenBullet.Core.Models;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 8 Tests: KeyChecker Implementation
/// </summary>
public class Step8_KeyCheckerTests : IDisposable
{
    private readonly Mock<ILogger<KeyChecker>> _loggerMock;
    private readonly KeyChecker _keyChecker;
    private readonly BotData _botData;

    public Step8_KeyCheckerTests()
    {
        _loggerMock = new Mock<ILogger<KeyChecker>>();
        _keyChecker = new KeyChecker(_loggerMock.Object);

        // Create test bot data
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        var cancellationToken = new CancellationTokenSource().Token;
        _botData = new BotData("test:data", config, logger, cancellationToken);
        _botData.Source = "Welcome to the dashboard! Login successful.";
        _botData.Address = "https://example.com/dashboard";
        _botData.ResponseCode = 200;
        _botData.SetVariable("USER_TYPE", "premium");
        _botData.SetCapture("TOKEN", "abc123");
    }

    [Fact]
    public void KeyChecker_Can_Be_Created()
    {
        // Act & Assert
        _keyChecker.Should().NotBeNull();
        _keyChecker.Should().BeAssignableTo<IKeyChecker>();
    }

    [Fact]
    public void EvaluateKey_With_Contains_Condition_Should_Work()
    {
        // Arrange
        var key = Key.Contains("<SOURCE>", "Welcome");

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKey_With_Contains_Case_Insensitive_Should_Work()
    {
        // Arrange
        var key = Key.Contains("<SOURCE>", "WELCOME", caseSensitive: false);

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKey_With_Contains_Case_Sensitive_Should_Work()
    {
        // Arrange
        var key = Key.Contains("<SOURCE>", "WELCOME", caseSensitive: true);

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EvaluateKey_With_DoesNotContain_Condition_Should_Work()
    {
        // Arrange
        var key = Key.DoesNotContain("<SOURCE>", "Error");

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKey_With_EqualTo_Condition_Should_Work()
    {
        // Arrange
        var key = Key.EqualTo("<RESPONSECODE>", "200");

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKey_With_MatchesRegex_Condition_Should_Work()
    {
        // Arrange
        var key = Key.MatchesRegex("<SOURCE>", @"Welcome.*dashboard");

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKey_With_Variable_Reference_Should_Work()
    {
        // Arrange
        var key = Key.EqualTo("<USER_TYPE>", "premium");

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKey_With_Captured_Data_Reference_Should_Work()
    {
        // Arrange
        var key = Key.EqualTo("<TOKEN>", "abc123");

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKeyChain_With_OR_Logic_Should_Work()
    {
        // Arrange
        var keyChain = new KeyChain
        {
            Logic = KeyChainLogic.OR,
            Keys = new List<Key>
            {
                Key.Contains("<SOURCE>", "NonExistent"),
                Key.Contains("<SOURCE>", "Welcome"),
                Key.Contains("<SOURCE>", "AnotherNonExistent")
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChain(keyChain, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKeyChain_With_AND_Logic_Should_Work()
    {
        // Arrange
        var keyChain = new KeyChain
        {
            Logic = KeyChainLogic.AND,
            Keys = new List<Key>
            {
                Key.Contains("<SOURCE>", "Welcome"),
                Key.Contains("<SOURCE>", "dashboard"),
                Key.Contains("<SOURCE>", "successful")
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChain(keyChain, _botData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKeyChain_With_AND_Logic_Should_Fail_If_Any_Key_Fails()
    {
        // Arrange
        var keyChain = new KeyChain
        {
            Logic = KeyChainLogic.AND,
            Keys = new List<Key>
            {
                Key.Contains("<SOURCE>", "Welcome"),
                Key.Contains("<SOURCE>", "NonExistent"), // This will fail
                Key.Contains("<SOURCE>", "successful")
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChain(keyChain, _botData);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EvaluateKeyChains_With_Success_Chain_Should_Return_Success()
    {
        // Arrange
        var keyChains = new List<KeyChain>
        {
            new()
            {
                Status = BotStatus.Success,
                Logic = KeyChainLogic.OR,
                Keys = new List<Key>
                {
                    Key.Contains("<SOURCE>", "Welcome"),
                    Key.Contains("<SOURCE>", "dashboard")
                }
            },
            new()
            {
                Status = BotStatus.Failure,
                Logic = KeyChainLogic.OR,
                Keys = new List<Key>
                {
                    Key.Contains("<SOURCE>", "Error"),
                    Key.Contains("<SOURCE>", "Failed")
                }
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChains(keyChains, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Status.Should().Be(BotStatus.Success);
        result.MatchedKeyChain.Should().NotBeNull();
        result.MatchedKeyChain!.Status.Should().Be(BotStatus.Success);
    }

    [Fact]
    public void EvaluateKeyChains_With_No_Matches_Should_Return_Failure()
    {
        // Arrange
        var keyChains = new List<KeyChain>
        {
            new()
            {
                Status = BotStatus.Success,
                Logic = KeyChainLogic.AND,
                Keys = new List<Key>
                {
                    Key.Contains("<SOURCE>", "NonExistent1"),
                    Key.Contains("<SOURCE>", "NonExistent2")
                }
            },
            new()
            {
                Status = BotStatus.Failure,
                Logic = KeyChainLogic.AND,
                Keys = new List<Key>
                {
                    Key.Contains("<SOURCE>", "NonExistent3"),
                    Key.Contains("<SOURCE>", "NonExistent4")
                }
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChains(keyChains, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Status.Should().Be(BotStatus.Failure);
        result.MatchedKeyChain.Should().BeNull();
    }

    [Fact]
    public void EvaluateKeyChains_With_Ban_Chain_Should_Set_Ban_Flag()
    {
        // Arrange
        _botData.ResponseCode = 429;
        
        var keyChains = new List<KeyChain>
        {
            new()
            {
                Status = BotStatus.Ban,
                Logic = KeyChainLogic.AND,
                BanProxy = true,
                Keys = new List<Key>
                {
                    Key.EqualTo("<RESPONSECODE>", "429")
                }
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChains(keyChains, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Status.Should().Be(BotStatus.Ban);
        result.ShouldBanProxy.Should().BeTrue();
    }

    [Fact]
    public void EvaluateKeyChains_With_Custom_Status_Should_Set_Custom_Status()
    {
        // Arrange
        var keyChains = new List<KeyChain>
        {
            new()
            {
                Status = BotStatus.Custom,
                CustomStatus = "Premium User",
                Logic = KeyChainLogic.OR,
                Keys = new List<Key>
                {
                    Key.EqualTo("<USER_TYPE>", "premium")
                }
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChains(keyChains, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Status.Should().Be(BotStatus.Custom);
        result.CustomStatus.Should().Be("Premium User");
    }

    [Fact]
    public void GetSourceValue_With_Special_Sources_Should_Return_Correct_Values()
    {
        // Act & Assert
        _keyChecker.GetSourceValue("<SOURCE>", _botData).Should().Be(_botData.Source);
        _keyChecker.GetSourceValue("<ADDRESS>", _botData).Should().Be(_botData.Address);
        _keyChecker.GetSourceValue("<RESPONSECODE>", _botData).Should().Be(_botData.ResponseCode.ToString());
        _keyChecker.GetSourceValue("<STATUS>", _botData).Should().Be(_botData.Status.ToString());
        _keyChecker.GetSourceValue("<CUSTOMSTATUS>", _botData).Should().Be(_botData.CustomStatus);
    }

    [Fact]
    public void GetSourceValue_With_Variable_Reference_Should_Return_Variable_Value()
    {
        // Act
        var result = _keyChecker.GetSourceValue("<USER_TYPE>", _botData);

        // Assert
        result.Should().Be("premium");
    }

    [Fact]
    public void GetSourceValue_With_Literal_Value_Should_Return_As_Is()
    {
        // Act
        var result = _keyChecker.GetSourceValue("literal_value", _botData);

        // Assert
        result.Should().Be("literal_value");
    }

    [Fact]
    public void GetSourceValue_With_NonExistent_Variable_Should_Return_Empty()
    {
        // Act
        var result = _keyChecker.GetSourceValue("<NONEXISTENT>", _botData);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(KeyCondition.StartsWith, "Welcome to the dashboard!", "Welcome", true)]
    [InlineData(KeyCondition.StartsWith, "Welcome to the dashboard!", "welcome", false)] // Case sensitive
    [InlineData(KeyCondition.EndsWith, "Welcome to the dashboard!", "dashboard!", true)]
    [InlineData(KeyCondition.EndsWith, "Welcome to the dashboard!", "DASHBOARD!", false)] // Case sensitive
    [InlineData(KeyCondition.IsEmpty, "", "", true)]
    [InlineData(KeyCondition.IsEmpty, "not empty", "", false)]
    [InlineData(KeyCondition.IsNotEmpty, "not empty", "", true)]
    [InlineData(KeyCondition.IsNotEmpty, "", "", false)]
    [InlineData(KeyCondition.IsNumeric, "123", "", true)]
    [InlineData(KeyCondition.IsNumeric, "123.45", "", true)]
    [InlineData(KeyCondition.IsNumeric, "not a number", "", false)]
    [InlineData(KeyCondition.IsNotNumeric, "not a number", "", true)]
    [InlineData(KeyCondition.IsNotNumeric, "123", "", false)]
    public void EvaluateKey_With_Various_Conditions_Should_Work(KeyCondition condition, string sourceValue, string targetValue, bool expected)
    {
        // Arrange
        _botData.Source = sourceValue;
        var key = new Key
        {
            Source = "<SOURCE>",
            Condition = condition,
            Value = targetValue,
            CaseSensitive = true // Use case sensitive for these tests
        };

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("100", "50", true)]
    [InlineData("50", "100", false)]
    [InlineData("100", "100", false)]
    [InlineData("not a number", "50", false)]
    public void EvaluateKey_With_GreaterThan_Condition_Should_Work(string sourceValue, string targetValue, bool expected)
    {
        // Arrange
        _botData.Source = sourceValue;
        var key = new Key
        {
            Source = "<SOURCE>",
            Condition = KeyCondition.GreaterThan,
            Value = targetValue
        };

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello", "5", true)]
    [InlineData("hello", "10", false)]
    [InlineData("hello", "3", false)]
    [InlineData("hello world", "11", true)]
    public void EvaluateKey_With_LengthEqualTo_Condition_Should_Work(string sourceValue, string targetLength, bool expected)
    {
        // Arrange
        _botData.Source = sourceValue;
        var key = new Key
        {
            Source = "<SOURCE>",
            Condition = KeyCondition.LengthEqualTo,
            Value = targetLength
        };

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void EvaluateKey_With_Invalid_Regex_Should_Return_False()
    {
        // Arrange
        var key = Key.MatchesRegex("<SOURCE>", "[invalid");

        // Act
        var result = _keyChecker.EvaluateKey(key, _botData);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EvaluateKeyChains_Should_Include_Evaluations()
    {
        // Arrange
        var keyChains = new List<KeyChain>
        {
            new()
            {
                Status = BotStatus.Success,
                Logic = KeyChainLogic.OR,
                Keys = new List<Key>
                {
                    Key.Contains("<SOURCE>", "Welcome"),
                    Key.Contains("<SOURCE>", "Test")
                }
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChains(keyChains, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Evaluations.Should().HaveCount(2);
        result.Evaluations[0].Result.Should().BeTrue();
        result.Evaluations[0].ActualValue.Should().Be(_botData.Source);
        result.Evaluations[0].ExpectedValue.Should().Be("Welcome");
        result.Evaluations[1].Result.Should().BeFalse();
        result.Evaluations[1].ExpectedValue.Should().Be("Test");
    }

    [Fact]
    public void EvaluateKeyChains_Should_Include_Metadata()
    {
        // Arrange
        var keyChains = new List<KeyChain>
        {
            new()
            {
                Status = BotStatus.Success,
                Logic = KeyChainLogic.OR,
                Keys = new List<Key>
                {
                    Key.Contains("<SOURCE>", "Welcome")
                }
            }
        };

        // Act
        var result = _keyChecker.EvaluateKeyChains(keyChains, _botData);

        // Assert
        result.Should().NotBeNull();
        result.Metadata.Should().ContainKey("TotalKeyChains");
        result.Metadata.Should().ContainKey("TotalKeys");
        result.Metadata.Should().ContainKey("EvaluationCount");
        result.Metadata.Should().ContainKey("MatchedChain");
        result.Metadata["TotalKeyChains"].Should().Be(1);
        result.Metadata["TotalKeys"].Should().Be(1);
    }

    [Fact]
    public void KeyCheckResult_ApplyToBotData_Should_Update_BotData()
    {
        // Arrange
        var result = new KeyCheckResult
        {
            Success = true,
            Status = BotStatus.Success,
            CustomStatus = "Test Status",
            MatchedKeyChain = new KeyChain { Name = "Success" },
            Evaluations = new List<KeyEvaluation> { new() }
        };

        // Act
        result.ApplyToBotData(_botData);

        // Assert
        _botData.Status.Should().Be(BotStatus.Success);
        _botData.CustomStatus.Should().Be("Test Status");
        _botData.Variables.Should().ContainKey("_KEYCHECK_EVALUATIONS");
        _botData.Variables.Should().ContainKey("_KEYCHECK_MATCHED_CHAIN");
        _botData.Variables["_KEYCHECK_EVALUATIONS"].Should().Be(1);
        _botData.Variables["_KEYCHECK_MATCHED_CHAIN"].Should().Be("Success");
    }

    [Fact]
    public void KeyChain_Factory_Methods_Should_Create_Correct_Chains()
    {
        // Act
        var successChain = KeyChain.Success();
        var failureChain = KeyChain.Failure();
        var banChain = KeyChain.Ban();
        var retryChain = KeyChain.CreateRetry();
        var customChain = KeyChain.Custom("Test Custom");

        // Assert
        successChain.Status.Should().Be(BotStatus.Success);
        successChain.Logic.Should().Be(KeyChainLogic.OR);
        
        failureChain.Status.Should().Be(BotStatus.Failure);
        
        banChain.Status.Should().Be(BotStatus.Ban);
        banChain.BanProxy.Should().BeTrue();
        
        retryChain.Status.Should().Be(BotStatus.Retry);
        retryChain.Retry.Should().BeTrue();
        
        customChain.Status.Should().Be(BotStatus.Custom);
        customChain.CustomStatus.Should().Be("Test Custom");
    }

    [Fact]
    public void Key_Factory_Methods_Should_Create_Correct_Keys()
    {
        // Act
        var containsKey = Key.Contains("<SOURCE>", "test");
        var equalToKey = Key.EqualTo("<SOURCE>", "test");
        var doesNotContainKey = Key.DoesNotContain("<SOURCE>", "test");
        var regexKey = Key.MatchesRegex("<SOURCE>", @"\d+");

        // Assert
        containsKey.Condition.Should().Be(KeyCondition.Contains);
        containsKey.Source.Should().Be("<SOURCE>");
        containsKey.Value.Should().Be("test");
        
        equalToKey.Condition.Should().Be(KeyCondition.EqualTo);
        
        doesNotContainKey.Condition.Should().Be(KeyCondition.DoesNotContain);
        
        regexKey.Condition.Should().Be(KeyCondition.MatchesRegex);
        regexKey.Value.Should().Be(@"\d+");
    }

    [Fact]
    public void KeyChainBuilder_Should_Create_Chains_Fluently()
    {
        // Act
        var chain = KeyChainBuilder.Success()
            .WithLogic(KeyChainLogic.AND)
            .WithName("Test Success Chain")
            .AddKey("<SOURCE>", KeyCondition.Contains, "Welcome")
            .AddKey("<SOURCE>", KeyCondition.Contains, "dashboard")
            .Build();

        // Assert
        chain.Should().NotBeNull();
        chain.Status.Should().Be(BotStatus.Success);
        chain.Logic.Should().Be(KeyChainLogic.AND);
        chain.Name.Should().Be("Test Success Chain");
        chain.Keys.Should().HaveCount(2);
        chain.Keys[0].Condition.Should().Be(KeyCondition.Contains);
        chain.Keys[0].Value.Should().Be("Welcome");
        chain.Keys[1].Value.Should().Be("dashboard");
    }

    [Fact]
    public void KeyChainBuilder_Custom_Should_Create_Custom_Chain()
    {
        // Act
        var chain = KeyChainBuilder.Custom("Premium User")
            .WithBanProxy()
            .AddKey("<USER_TYPE>", KeyCondition.EqualTo, "premium")
            .Build();

        // Assert
        chain.Should().NotBeNull();
        chain.Status.Should().Be(BotStatus.Custom);
        chain.CustomStatus.Should().Be("Premium User");
        chain.BanProxy.Should().BeTrue();
        chain.Keys.Should().HaveCount(1);
    }

    public void Dispose()
    {
        // No resources to dispose in this test class
    }
}
