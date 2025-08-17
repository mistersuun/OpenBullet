using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 9 Tests: BotRunner Implementation
/// </summary>
public class Step9_BotRunnerTests : IDisposable
{
    private readonly Mock<ILogger<BotRunner>> _loggerMock;
    private readonly Mock<OpenBullet.Core.Execution.IScriptEngine> _scriptEngineMock;
    private readonly BotRunner _botRunner;
    private readonly ConfigModel _testConfig;

    public Step9_BotRunnerTests()
    {
        _loggerMock = new Mock<ILogger<BotRunner>>();
        _scriptEngineMock = new Mock<OpenBullet.Core.Execution.IScriptEngine>();
        _botRunner = new BotRunner(_loggerMock.Object, _scriptEngineMock.Object);

        _testConfig = new ConfigModel
        {
            Name = "TestConfig",
            Script = "REQUEST GET \"https://example.com\""
        };
    }

    [Fact]
    public void BotRunner_Can_Be_Created()
    {
        // Act & Assert
        _botRunner.Should().NotBeNull();
        _botRunner.Should().BeAssignableTo<IBotRunner>();
    }

    [Fact]
    public async Task RunAsync_With_Successful_Script_Should_Return_Success_Result()
    {
        // Arrange
        var dataLine = "test:password123";
        var scriptResult = new ScriptExecutionResult
        {
            Success = true,
            Status = BotStatus.Success,
            ExecutionTime = TimeSpan.FromMilliseconds(500),
            CommandsExecuted = 3,
            Variables = new Dictionary<string, object> { ["USER"] = "test", ["PASS"] = "password123" },
            CapturedData = new Dictionary<string, object> { ["TOKEN"] = "abc123" },
            Logs = new List<string> { "Request sent", "Response received", "Login successful" }
        };

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(scriptResult);

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Status.Should().Be(BotStatus.Success);
        result.DataLine.Should().Be(dataLine);
        result.ConfigName.Should().Be("TestConfig");
        result.Variables.Should().ContainKey("USER");
        result.Variables["USER"].Should().Be("test");
        result.CapturedData.Should().ContainKey("TOKEN");
        result.CapturedData["TOKEN"].Should().Be("abc123");
        result.Logs.Should().HaveCount(3);
        result.ExecutionTime.Should().BeGreaterThan(TimeSpan.Zero);
        result.BotId.Should().NotBeEmpty();
        result.Metadata.Should().ContainKey("CommandsExecuted");
        result.Metadata["CommandsExecuted"].Should().Be(3);
    }

    [Fact]
    public async Task RunAsync_With_Failed_Script_Should_Return_Failed_Result()
    {
        // Arrange
        var dataLine = "invalid:data";
        var scriptResult = new ScriptExecutionResult
        {
            Success = false,
            Status = BotStatus.Failure,
            ExecutionTime = TimeSpan.FromMilliseconds(200),
            CommandsExecuted = 1,
            ErrorMessage = "Invalid credentials",
            Logs = new List<string> { "Login failed" }
        };

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(scriptResult);

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Status.Should().Be(BotStatus.Failure);
        result.ErrorMessage.Should().Be("Invalid credentials");
        result.DataLine.Should().Be(dataLine);
        result.Logs.Should().HaveCount(1);
        result.Logs[0].Should().Be("Login failed");
    }

    [Fact]
    public async Task RunAsync_With_Custom_Status_Should_Return_Custom_Result()
    {
        // Arrange
        var dataLine = "premium:user123";
        var scriptResult = new ScriptExecutionResult
        {
            Success = true,
            Status = BotStatus.Custom,
            CustomStatus = "Premium User",
            ExecutionTime = TimeSpan.FromMilliseconds(750),
            CommandsExecuted = 5,
            Variables = new Dictionary<string, object> { ["ACCOUNT_TYPE"] = "premium" },
            CapturedData = new Dictionary<string, object> { ["BALANCE"] = "1000.50" }
        };

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(scriptResult);

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Status.Should().Be(BotStatus.Custom);
        result.CustomStatus.Should().Be("Premium User");
        result.Variables.Should().ContainKey("ACCOUNT_TYPE");
        result.CapturedData.Should().ContainKey("BALANCE");
    }

    [Fact]
    public async Task RunAsync_With_Cancellation_Should_Return_Cancelled_Result()
    {
        // Arrange
        var dataLine = "test:data";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine, cancellationTokenSource.Token);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Status.Should().Be(BotStatus.Error);
        result.ErrorMessage.Should().Be("Bot execution was cancelled");
        result.DataLine.Should().Be(dataLine);
    }

    [Fact]
    public async Task RunAsync_With_Exception_Should_Return_Error_Result()
    {
        // Arrange
        var dataLine = "test:data";
        var exception = new InvalidOperationException("Script engine failed");

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Status.Should().Be(BotStatus.Error);
        result.ErrorMessage.Should().Be("Script engine failed");
        result.Exception.Should().BeSameAs(exception);
        result.DataLine.Should().Be(dataLine);
    }

    [Fact]
    public async Task RunAsync_Should_Set_Timing_Information()
    {
        // Arrange
        var dataLine = "test:data";
        var startTime = DateTime.UtcNow;
        var scriptResult = new ScriptExecutionResult
        {
            Success = true,
            Status = BotStatus.Success,
            ExecutionTime = TimeSpan.FromMilliseconds(100)
        };

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(scriptResult);

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.StartTime.Should().BeOnOrAfter(startTime);
        result.EndTime.Should().BeOnOrAfter(result.StartTime);
        result.ExecutionTime.Should().BeGreaterThan(TimeSpan.Zero);
        result.ExecutionTime.Should().BeLessThan(TimeSpan.FromSeconds(5)); // Should be reasonably fast
    }

    [Fact]
    public async Task RunAsync_Should_Include_HTTP_Metadata_When_Available()
    {
        // Arrange
        var dataLine = "test:data";
        var scriptResult = new ScriptExecutionResult
        {
            Success = true,
            Status = BotStatus.Success,
            ExecutionTime = TimeSpan.FromMilliseconds(300)
        };

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((ConfigModel config, BotData botData, CancellationToken ct) =>
                        {
                            // Simulate HTTP response data being set
                            botData.ResponseCode = 200;
                            botData.Address = "https://example.com";
                            botData.Source = "<html>Welcome</html>";
                            return scriptResult;
                        });

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.Metadata.Should().ContainKey("LastResponseCode");
        result.Metadata["LastResponseCode"].Should().Be(200);
        result.Metadata.Should().ContainKey("LastAddress");
        result.Metadata["LastAddress"].Should().Be("https://example.com");
        result.Metadata.Should().ContainKey("SourceLength");
        result.Metadata["SourceLength"].Should().Be(20);
    }

    [Fact]
    public async Task RunAsync_Should_Include_Proxy_Metadata_When_Available()
    {
        // Arrange
        var dataLine = "test:data";
        var scriptResult = new ScriptExecutionResult
        {
            Success = true,
            Status = BotStatus.Success
        };

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((ConfigModel config, BotData botData, CancellationToken ct) =>
                        {
                            // Simulate proxy being set
                            botData.Proxy = new ProxyInfo
                            {
                                Host = "proxy.example.com",
                                Port = 8080,
                                Type = ProxyType.Http
                            };
                            return scriptResult;
                        });

        // Act
        var result = await _botRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.Metadata.Should().ContainKey("ProxyUsed");
        result.Metadata["ProxyUsed"].Should().Be("proxy.example.com:8080");
        result.Metadata.Should().ContainKey("ProxyType");
        result.Metadata["ProxyType"].Should().Be("Http");
    }

    [Fact]
    public void BotResult_GetCapturedValue_Should_Return_Correct_Type()
    {
        // Arrange
        var result = new BotResult();
        result.CapturedData["StringValue"] = "test";
        result.CapturedData["IntValue"] = 123;
        result.CapturedData["BoolValue"] = true;

        // Act & Assert
        result.GetCapturedValue<string>("StringValue").Should().Be("test");
        result.GetCapturedValue<int>("IntValue").Should().Be(123);
        result.GetCapturedValue<bool>("BoolValue").Should().Be(true);
        result.GetCapturedValue<string>("NonExistent").Should().BeNull();
        result.GetCapturedValue<int>("StringValue").Should().Be(0); // Type mismatch returns default
    }

    [Fact]
    public void BotResult_GetVariableValue_Should_Return_Correct_Type()
    {
        // Arrange
        var result = new BotResult();
        result.Variables["USER"] = "testuser";
        result.Variables["COUNT"] = 42;
        result.Variables["ENABLED"] = false;

        // Act & Assert
        result.GetVariableValue<string>("USER").Should().Be("testuser");
        result.GetVariableValue<int>("COUNT").Should().Be(42);
        result.GetVariableValue<bool>("ENABLED").Should().Be(false);
        result.GetVariableValue<string>("MISSING").Should().BeNull();
    }

    [Fact]
    public void BotResult_Properties_Should_Work_Correctly()
    {
        // Arrange
        var result = new BotResult
        {
            CapturedData = new Dictionary<string, object> { ["token"] = "abc" },
            Variables = new Dictionary<string, object> { ["user"] = "test" },
            ExecutionTime = TimeSpan.FromMilliseconds(500)
        };
        result.Metadata["CommandsExecuted"] = 10;

        // Act & Assert
        result.HasCapturedData.Should().BeTrue();
        result.HasVariables.Should().BeTrue();
        result.GetExecutionRate().Should().Be(20.0); // 10 commands / 0.5 seconds = 20 commands/second

        var emptyResult = new BotResult();
        emptyResult.HasCapturedData.Should().BeFalse();
        emptyResult.HasVariables.Should().BeFalse();
        emptyResult.GetExecutionRate().Should().Be(0);
    }

    [Fact]
    public void BotResult_GetSummary_Should_Format_Correctly()
    {
        // Arrange
        var result = new BotResult
        {
            BotId = "bot123",
            Status = BotStatus.Success,
            ExecutionTime = TimeSpan.FromMilliseconds(1500)
        };

        var customResult = new BotResult
        {
            BotId = "bot456",
            Status = BotStatus.Custom,
            CustomStatus = "Premium User",
            ExecutionTime = TimeSpan.FromMilliseconds(750)
        };

        // Act
        var summary = result.GetSummary();
        var customSummary = customResult.GetSummary();

        // Assert
        summary.Should().Be("Bot bot123: Success in 1500ms");
        customSummary.Should().Be("Bot bot456: Premium User in 750ms");
    }

    [Fact]
    public void BotResult_GetFormattedCapturedData_Should_Convert_To_Strings()
    {
        // Arrange
        var result = new BotResult();
        result.CapturedData["username"] = "testuser";
        result.CapturedData["balance"] = 1234.56;
        result.CapturedData["isActive"] = true;
        result.CapturedData["nullValue"] = null!;

        // Act
        var formatted = result.GetFormattedCapturedData();

        // Assert
        formatted.Should().HaveCount(4);
        formatted["username"].Should().Be("testuser");
        formatted["balance"].Should().Be("1234.56");
        formatted["isActive"].Should().Be("True");
        formatted["nullValue"].Should().Be("");
    }

    [Fact]
    public void EnhancedBotRunner_Can_Be_Created()
    {
        // Arrange
        var enhancedLoggerMock = new Mock<ILogger<EnhancedBotRunner>>();
        
        // Act
        var enhancedRunner = new EnhancedBotRunner(enhancedLoggerMock.Object, _scriptEngineMock.Object);

        // Assert
        enhancedRunner.Should().NotBeNull();
        enhancedRunner.Should().BeAssignableTo<BotRunner>();
    }

    [Fact]
    public async Task EnhancedBotRunner_Should_Add_Analysis_Metadata()
    {
        // Arrange
        var enhancedLoggerMock = new Mock<ILogger<EnhancedBotRunner>>();
        var enhancedRunner = new EnhancedBotRunner(enhancedLoggerMock.Object, _scriptEngineMock.Object);
        
        var dataLine = "test:data";
        var scriptResult = new ScriptExecutionResult
        {
            Success = true,
            Status = BotStatus.Success,
            ExecutionTime = TimeSpan.FromMilliseconds(100),
            CommandsExecuted = 5,
            Variables = new Dictionary<string, object> { ["test"] = "value" },
            CapturedData = new Dictionary<string, object> { ["token"] = "abc", ["balance"] = "100" },
            Logs = new List<string> { "Info: Starting", "Warning: Slow response", "Error: Timeout" }
        };

        _scriptEngineMock.Setup(se => se.ExecuteAsync(It.IsAny<ConfigModel>(), It.IsAny<BotData>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(scriptResult);

        // Act
        var result = await enhancedRunner.RunAsync(_testConfig, dataLine);

        // Assert
        result.Should().NotBeNull();
        result.Metadata.Should().ContainKey("ExecutionRate");
        result.Metadata["ExecutionRate"].Should().Be(50.0); // 5 commands / 0.1 seconds

        result.Metadata.Should().ContainKey("CapturedDataKeys");
        var capturedKeys = result.Metadata["CapturedDataKeys"] as List<string>;
        capturedKeys.Should().Contain("token").And.Contain("balance");

        result.Metadata.Should().ContainKey("ErrorLogCount");
        result.Metadata["ErrorLogCount"].Should().Be(1);
        result.Metadata.Should().ContainKey("WarningLogCount");
        result.Metadata["WarningLogCount"].Should().Be(1);
    }

    [Theory]
    [InlineData(BotStatus.Success, true)]
    [InlineData(BotStatus.Custom, true)]
    [InlineData(BotStatus.Failure, false)]
    [InlineData(BotStatus.Error, false)]
    [InlineData(BotStatus.Ban, false)]
    [InlineData(BotStatus.Retry, false)]
    public void BotStatusExtensions_IsSuccess_Should_Work_Correctly(BotStatus status, bool expectedIsSuccess)
    {
        // Act & Assert
        status.IsSuccess().Should().Be(expectedIsSuccess);
    }

    [Theory]
    [InlineData(BotStatus.Error, true)]
    [InlineData(BotStatus.Success, false)]
    [InlineData(BotStatus.Failure, false)]
    [InlineData(BotStatus.Custom, false)]
    public void BotStatusExtensions_IsError_Should_Work_Correctly(BotStatus status, bool expectedIsError)
    {
        // Act & Assert
        status.IsError().Should().Be(expectedIsError);
    }

    [Theory]
    [InlineData(BotStatus.Retry, true)]
    [InlineData(BotStatus.Ban, true)]
    [InlineData(BotStatus.Success, false)]
    [InlineData(BotStatus.Failure, false)]
    [InlineData(BotStatus.Error, false)]
    public void BotStatusExtensions_ShouldRetry_Should_Work_Correctly(BotStatus status, bool expectedShouldRetry)
    {
        // Act & Assert
        status.ShouldRetry().Should().Be(expectedShouldRetry);
    }

    [Fact]
    public void DictionaryExtensions_AddRange_Should_Add_All_Items()
    {
        // Arrange
        var dict1 = new Dictionary<string, object> { ["key1"] = "value1", ["key2"] = 123 };
        var dict2 = new Dictionary<string, object> { ["key3"] = "value3", ["key2"] = 456 }; // key2 will be overwritten

        // Act
        dict1.AddRange(dict2);

        // Assert
        dict1.Should().HaveCount(3);
        dict1["key1"].Should().Be("value1");
        dict1["key2"].Should().Be(456); // Overwritten value
        dict1["key3"].Should().Be("value3");
    }

    public void Dispose()
    {
        // No resources to dispose in this test class
    }
}
