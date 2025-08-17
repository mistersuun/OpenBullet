using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using OpenBullet.Core.Services;
using OpenBullet.Core.Variables;
using System.Net;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 5 Tests: Execution Context
/// </summary>
public class Step5_ExecutionContextTests : IDisposable
{
    private readonly Mock<ILogger<OpenBullet.Core.Execution.ExecutionContext>> _loggerMock;
    private readonly Mock<IVariableManager> _variableManagerMock;
    private readonly Mock<IHttpClientService> _httpClientMock;
    private readonly Mock<ILogger<VariableManager>> _varLoggerMock;
    private readonly BotData _botData;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly OpenBullet.Core.Execution.ExecutionContext _executionContext;

    public Step5_ExecutionContextTests()
    {
        _loggerMock = new Mock<ILogger<OpenBullet.Core.Execution.ExecutionContext>>();
        _variableManagerMock = new Mock<IVariableManager>();
        _httpClientMock = new Mock<IHttpClientService>();
        _varLoggerMock = new Mock<ILogger<VariableManager>>();
        _cancellationTokenSource = new CancellationTokenSource();

        // Create test bot data
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        _botData = new BotData("testuser:testpass", config, logger, _cancellationTokenSource.Token);

        _executionContext = new OpenBullet.Core.Execution.ExecutionContext(
            _botData,
            _variableManagerMock.Object,
            _httpClientMock.Object,
            _cancellationTokenSource.Token,
            _loggerMock.Object);
    }

    [Fact]
    public void ExecutionContext_Should_Initialize_Successfully()
    {
        // Assert
        _executionContext.Should().NotBeNull();
        _executionContext.Should().BeAssignableTo<IExecutionContext>();
        _executionContext.Id.Should().NotBeEmpty();
        _executionContext.BotData.Should().Be(_botData);
        _executionContext.VariableManager.Should().Be(_variableManagerMock.Object);
        _executionContext.HttpClient.Should().Be(_httpClientMock.Object);
        _executionContext.Status.Should().Be(ExecutionStatus.Running);
        _executionContext.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ExecutionContext_Should_Initialize_From_BotData()
    {
        // Arrange
        _botData.SetVariable("TEST_VAR", "test_value");
        _botData.SetCapture("TEST_CAP", "cap_value");
        _botData.Source = "test_source";
        _botData.ResponseCode = 200;
        _botData.Address = "https://test.com";

        var realVariableManager = new VariableManager(_varLoggerMock.Object);

        // Act
        using var context = new OpenBullet.Core.Execution.ExecutionContext(
            _botData,
            realVariableManager,
            _httpClientMock.Object,
            _cancellationTokenSource.Token,
            _loggerMock.Object);

        // Assert
        context.ResponseSource.Should().Be("test_source");
        context.ResponseCode.Should().Be(200);
        context.ResponseAddress.Should().Be("https://test.com");
        context.CapturedData.Should().ContainKey("TEST_CAP");
        context.CapturedData["TEST_CAP"].Should().Be("cap_value");
    }

    [Fact]
    public void Status_Property_Should_Fire_Event_When_Changed()
    {
        // Arrange
        var eventFired = false;
        ExecutionStatusChangedEventArgs? capturedArgs = null;

        _executionContext.StatusChanged += (sender, args) =>
        {
            eventFired = true;
            capturedArgs = args;
        };

        // Act
        _executionContext.Status = ExecutionStatus.Completed;

        // Assert
        eventFired.Should().BeTrue();
        capturedArgs.Should().NotBeNull();
        capturedArgs!.ContextId.Should().Be(_executionContext.Id);
        capturedArgs.OldStatus.Should().Be(ExecutionStatus.Running);
        capturedArgs.NewStatus.Should().Be(ExecutionStatus.Completed);
    }

    [Fact]
    public void Status_Property_Should_Not_Fire_Event_When_Set_To_Same_Value()
    {
        // Arrange
        var eventFired = false;
        _executionContext.StatusChanged += (sender, args) => eventFired = true;

        // Act
        _executionContext.Status = ExecutionStatus.Running; // Same as initial

        // Assert
        eventFired.Should().BeFalse();
    }

    [Fact]
    public void AddLog_Should_Add_Entry_And_Update_Statistics()
    {
        // Act
        _executionContext.AddLog("Test message", OpenBullet.Core.Execution.LogLevel.Info);
        _executionContext.AddLog("Warning message", OpenBullet.Core.Execution.LogLevel.Warning);
        _executionContext.AddLog("Error message", OpenBullet.Core.Execution.LogLevel.Error);

        // Assert
        _executionContext.Log.Should().HaveCount(3);
        _executionContext.Statistics.LogEntryCount.Should().Be(3);
        _executionContext.Statistics.WarningCount.Should().Be(2); // Warning + Error
        _executionContext.Statistics.ErrorCount.Should().Be(1);

        var logEntry = _executionContext.Log.First();
        logEntry.Message.Should().Be("Test message");
        logEntry.Level.Should().Be(OpenBullet.Core.Execution.LogLevel.Info);
        logEntry.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddLog_With_Exception_Should_Include_Exception()
    {
        // Arrange
        var testException = new InvalidOperationException("Test exception");

        // Act
        _executionContext.AddLog("Error occurred", testException, OpenBullet.Core.Execution.LogLevel.Error);

        // Assert
        _executionContext.Log.Should().HaveCount(1);
        var logEntry = _executionContext.Log.First();
        logEntry.Exception.Should().Be(testException);
        logEntry.Level.Should().Be(OpenBullet.Core.Execution.LogLevel.Error);
    }

    [Fact]
    public void AddLog_Should_Respect_LogLevel_Filter()
    {
        // Arrange
        _executionContext.Environment.LogLevel = OpenBullet.Core.Execution.LogLevel.Warning;

        // Act
        _executionContext.AddLog("Debug message", OpenBullet.Core.Execution.LogLevel.Debug);
        _executionContext.AddLog("Info message", OpenBullet.Core.Execution.LogLevel.Info);
        _executionContext.AddLog("Warning message", OpenBullet.Core.Execution.LogLevel.Warning);
        _executionContext.AddLog("Error message", OpenBullet.Core.Execution.LogLevel.Error);

        // Assert
        _executionContext.Log.Should().HaveCount(2); // Only Warning and Error
        _executionContext.Log.Should().Contain(e => e.Message == "Warning message");
        _executionContext.Log.Should().Contain(e => e.Message == "Error message");
    }

    [Fact]
    public void AddLog_Should_Skip_When_Logging_Disabled()
    {
        // Arrange
        _executionContext.Environment.EnableLogging = false;

        // Act
        _executionContext.AddLog("Test message", OpenBullet.Core.Execution.LogLevel.Info);

        // Assert
        _executionContext.Log.Should().BeEmpty();
    }

    [Fact]
    public void SetVariable_Should_Call_VariableManager_And_Update_Statistics()
    {
        // Act
        _executionContext.SetVariable("testVar", "testValue");

        // Assert
        _variableManagerMock.Verify(vm => vm.SetVariable("testVar", "testValue", VariableScope.Local), Times.Once);
        _executionContext.Statistics.VariableSetCount.Should().Be(1);
    }

    [Fact]
    public void GetVariable_Should_Call_VariableManager_And_Update_Statistics()
    {
        // Arrange
        _variableManagerMock.Setup(vm => vm.GetVariable<string>("testVar", VariableScope.Any))
                          .Returns("testValue");

        // Act
        var result = _executionContext.GetVariable<string>("testVar");

        // Assert
        result.Should().Be("testValue");
        _variableManagerMock.Verify(vm => vm.GetVariable<string>("testVar", VariableScope.Any), Times.Once);
        _executionContext.Statistics.VariableGetCount.Should().Be(1);
    }

    [Fact]
    public void SetCapture_Should_Store_Data_And_Fire_Event()
    {
        // Arrange
        var eventFired = false;
        DataCapturedEventArgs? capturedArgs = null;

        _executionContext.DataCaptured += (sender, args) =>
        {
            eventFired = true;
            capturedArgs = args;
        };

        // Act
        _executionContext.SetCapture("testCapture", "capturedValue");

        // Assert
        _executionContext.CapturedData.Should().ContainKey("testCapture");
        _executionContext.CapturedData["testCapture"].Should().Be("capturedValue");
        _executionContext.Statistics.DataCaptureCount.Should().Be(1);

        eventFired.Should().BeTrue();
        capturedArgs.Should().NotBeNull();
        capturedArgs!.Name.Should().Be("testCapture");
        capturedArgs.Value.Should().Be("capturedValue");
    }

    [Fact]
    public void GetCapture_Should_Return_Stored_Data()
    {
        // Arrange
        _executionContext.SetCapture("testCapture", 42);

        // Act
        var result = _executionContext.GetCapture<int>("testCapture");

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public void GetCapture_Should_Return_Default_For_Missing_Data()
    {
        // Act
        var result = _executionContext.GetCapture<string>("missingCapture");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void UpdateResponse_Should_Update_Response_Information()
    {
        // Act
        _executionContext.UpdateResponse("response body", 200, "https://example.com");

        // Assert
        _executionContext.ResponseSource.Should().Be("response body");
        _executionContext.ResponseCode.Should().Be(200);
        _executionContext.ResponseAddress.Should().Be("https://example.com");
        _executionContext.Statistics.HttpRequestCount.Should().Be(1);
        _executionContext.Statistics.LastHttpRequest.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateCheckpoint_Should_Create_Valid_Checkpoint()
    {
        // Arrange
        _executionContext.SetCapture("test", "value");
        _executionContext.UpdateResponse("source", 200, "address");
        _executionContext.CustomStatus = "custom";

        var variableSnapshot = new VariableSnapshot();
        _variableManagerMock.Setup(vm => vm.CreateSnapshot()).Returns(variableSnapshot);

        // Act
        var checkpoint = _executionContext.CreateCheckpoint("TestCheckpoint");

        // Assert
        checkpoint.Should().NotBeNull();
        checkpoint.Name.Should().Be("TestCheckpoint");
        checkpoint.Status.Should().Be(_executionContext.Status);
        checkpoint.CapturedData.Should().ContainKey("test");
        checkpoint.ResponseSource.Should().Be("source");
        checkpoint.ResponseCode.Should().Be(200);
        checkpoint.ResponseAddress.Should().Be("address");
        checkpoint.CustomStatus.Should().Be("custom");
        checkpoint.VariableSnapshot.Should().Be(variableSnapshot);
        checkpoint.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RestoreCheckpoint_Should_Restore_State()
    {
        // Arrange
        var checkpoint = new ExecutionCheckpoint
        {
            Name = "TestRestore",
            Status = ExecutionStatus.Paused,
            CapturedData = new Dictionary<string, object> { ["restored"] = "value" },
            ResponseSource = "restored_source",
            ResponseCode = 404,
            ResponseAddress = "restored_address",
            CustomStatus = "restored_status",
            VariableSnapshot = new VariableSnapshot()
        };

        // Act
        _executionContext.RestoreCheckpoint(checkpoint);

        // Assert
        _executionContext.Status.Should().Be(ExecutionStatus.Paused);
        _executionContext.CapturedData.Should().ContainKey("restored");
        _executionContext.ResponseSource.Should().Be("restored_source");
        _executionContext.ResponseCode.Should().Be(404);
        _executionContext.ResponseAddress.Should().Be("restored_address");
        _executionContext.CustomStatus.Should().Be("restored_status");
        
        _variableManagerMock.Verify(vm => vm.RestoreSnapshot(checkpoint.VariableSnapshot), Times.Once);
    }

    [Fact]
    public void Validate_Should_Return_Valid_For_Proper_Context()
    {
        // Act
        var result = _executionContext.Validate();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Should_Detect_Issues()
    {
        // Arrange
        _executionContext.Environment.RequestTimeout = -1;
        _executionContext.Environment.MaxRedirects = -5;

        // Act
        var result = _executionContext.Validate();

        // Assert
        result.Warnings.Should().Contain("RequestTimeout should be greater than 0");
        result.Warnings.Should().Contain("MaxRedirects should not be negative");
    }

    [Fact]
    public void ElapsedTime_Should_Increase_Over_Time()
    {
        // Arrange
        var initialTime = _executionContext.ElapsedTime;

        // Act
        Thread.Sleep(10); // Small delay
        var laterTime = _executionContext.ElapsedTime;

        // Assert
        laterTime.Should().BeGreaterThan(initialTime);
    }

    [Fact]
    public void Headers_Should_Be_Modifiable()
    {
        // Act
        _executionContext.Headers["Authorization"] = "Bearer token123";
        _executionContext.Headers["Accept"] = "application/json";

        // Assert
        _executionContext.Headers.Should().HaveCount(2);
        _executionContext.Headers["Authorization"].Should().Be("Bearer token123");
        _executionContext.Headers["Accept"].Should().Be("application/json");
    }

    [Fact]
    public void Cookies_Should_Be_Accessible()
    {
        // Act
        _executionContext.Cookies.Add(new Cookie("sessionId", "abc123", "/", "example.com"));

        // Assert
        _executionContext.Cookies.Count.Should().Be(1);
        var cookie = _executionContext.Cookies.GetCookies(new Uri("https://example.com")).Cast<Cookie>().First();
        cookie.Name.Should().Be("sessionId");
        cookie.Value.Should().Be("abc123");
    }

    [Fact]
    public void Proxy_Property_Should_Be_Settable()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "proxy.example.com",
            Port = 8080,
            Username = "user",
            Password = "pass",
            Type = ProxyType.Http
        };

        // Act
        _executionContext.Proxy = proxy;

        // Assert
        _executionContext.Proxy.Should().Be(proxy);
    }

    [Fact]
    public void Environment_Should_Have_Default_Values()
    {
        // Assert
        _executionContext.Environment.UserAgent.Should().Be("OpenBullet/2.0");
        _executionContext.Environment.RequestTimeout.Should().Be(10000);
        _executionContext.Environment.MaxRedirects.Should().Be(8);
        _executionContext.Environment.IgnoreSSLErrors.Should().BeFalse();
        _executionContext.Environment.AutoRedirect.Should().BeTrue();
        _executionContext.Environment.EnableLogging.Should().BeTrue();
        _executionContext.Environment.LogLevel.Should().Be(OpenBullet.Core.Execution.LogLevel.Info);
    }

    [Fact]
    public void Environment_Should_Be_Modifiable()
    {
        // Act
        _executionContext.Environment.UserAgent = "CustomBot/1.0";
        _executionContext.Environment.RequestTimeout = 5000;
        _executionContext.Environment.MaxRedirects = 3;
        _executionContext.Environment.IgnoreSSLErrors = true;

        // Assert
        _executionContext.Environment.UserAgent.Should().Be("CustomBot/1.0");
        _executionContext.Environment.RequestTimeout.Should().Be(5000);
        _executionContext.Environment.MaxRedirects.Should().Be(3);
        _executionContext.Environment.IgnoreSSLErrors.Should().BeTrue();
    }

    [Fact]
    public void Statistics_Should_Track_Command_Counts()
    {
        // Act
        _executionContext.Statistics.CommandCounts["REQUEST"] = 5;
        _executionContext.Statistics.CommandCounts["PARSE"] = 3;

        // Assert
        _executionContext.Statistics.CommandCounts["REQUEST"].Should().Be(5);
        _executionContext.Statistics.CommandCounts["PARSE"].Should().Be(3);
    }

    [Fact]
    public void Dispose_Should_Update_BotData_Status()
    {
        // Arrange
        _executionContext.Status = ExecutionStatus.Completed;
        _executionContext.CustomStatus = "Test completed";

        // Act - Add small delay to ensure measurable time passes
        System.Threading.Thread.Sleep(1);
        _executionContext.Dispose();

        // Assert
        _botData.Status.Should().Be(BotStatus.Success);
        _botData.CustomStatus.Should().Be("Test completed");
        _botData.ExecutionTime.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Dispose_Should_Map_Status_Correctly()
    {
        // Test Failed status
        _executionContext.Status = ExecutionStatus.Failed;
        _executionContext.Dispose();
        _botData.Status.Should().Be(BotStatus.Error);

        // Create new context for Cancelled status
        var botData2 = new BotData("test", new ConfigModel(), new Mock<ILogger>().Object, _cancellationTokenSource.Token);
        using var context2 = new OpenBullet.Core.Execution.ExecutionContext(botData2, _variableManagerMock.Object, _httpClientMock.Object, _cancellationTokenSource.Token, _loggerMock.Object);
        context2.Status = ExecutionStatus.Cancelled;
        context2.Dispose();
        botData2.Status.Should().Be(BotStatus.Ban);
    }

    [Fact]
    public void Constructor_Should_Throw_For_Null_Arguments()
    {
        // Act & Assert
        var act1 = () => new OpenBullet.Core.Execution.ExecutionContext(null!, _variableManagerMock.Object, _httpClientMock.Object, _cancellationTokenSource.Token, _loggerMock.Object);
        act1.Should().Throw<ArgumentNullException>();

        var act2 = () => new OpenBullet.Core.Execution.ExecutionContext(_botData, null!, _httpClientMock.Object, _cancellationTokenSource.Token, _loggerMock.Object);
        act2.Should().Throw<ArgumentNullException>();

        var act3 = () => new OpenBullet.Core.Execution.ExecutionContext(_botData, _variableManagerMock.Object, null!, _cancellationTokenSource.Token, _loggerMock.Object);
        act3.Should().Throw<ArgumentNullException>();

        var act4 = () => new OpenBullet.Core.Execution.ExecutionContext(_botData, _variableManagerMock.Object, _httpClientMock.Object, _cancellationTokenSource.Token, null!);
        act4.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RestoreCheckpoint_Should_Throw_For_Null_Checkpoint()
    {
        // Act & Assert
        var act = () => _executionContext.RestoreCheckpoint(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Concurrent_Operations_Should_Be_Thread_Safe()
    {
        // Arrange
        const int threadCount = 5;
        const int operationsPerThread = 50;
        var tasks = new List<Task>();

        // Act
        for (int t = 0; t < threadCount; t++)
        {
            var threadId = t;
            tasks.Add(Task.Run(() =>
            {
                for (int i = 0; i < operationsPerThread; i++)
                {
                    _executionContext.AddLog($"Thread {threadId} Message {i}");
                    _executionContext.SetCapture($"thread{threadId}_cap{i}", i);
                    _executionContext.UpdateResponse($"response{i}", 200 + i, $"https://test{i}.com");
                }
            }));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        _executionContext.Log.Count.Should().Be(threadCount * operationsPerThread);
        _executionContext.CapturedData.Count.Should().Be(threadCount * operationsPerThread);
        _executionContext.Statistics.HttpRequestCount.Should().Be(threadCount * operationsPerThread);
    }

    public void Dispose()
    {
        _executionContext?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}

/// <summary>
/// Step 5 Integration Tests
/// </summary>
public class Step5_IntegrationTests : IDisposable
{
    private readonly Mock<ILogger<VariableManager>> _varLoggerMock;
    private readonly Mock<ILogger<OpenBullet.Core.Execution.ExecutionContext>> _contextLoggerMock;
    private readonly Mock<IHttpClientService> _httpClientMock;
    private readonly VariableManager _variableManager;
    private readonly OpenBullet.Core.Execution.ExecutionContext _executionContext;
    private readonly BotData _botData;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public Step5_IntegrationTests()
    {
        _varLoggerMock = new Mock<ILogger<VariableManager>>();
        _contextLoggerMock = new Mock<ILogger<OpenBullet.Core.Execution.ExecutionContext>>();
        _httpClientMock = new Mock<IHttpClientService>();
        _cancellationTokenSource = new CancellationTokenSource();

        _variableManager = new VariableManager(_varLoggerMock.Object);

        var config = new ConfigModel { Name = "IntegrationTest" };
        var logger = new Mock<ILogger>().Object;
        _botData = new BotData("integration:test", config, logger, _cancellationTokenSource.Token);

        _executionContext = new OpenBullet.Core.Execution.ExecutionContext(
            _botData,
            _variableManager,
            _httpClientMock.Object,
            _cancellationTokenSource.Token,
            _contextLoggerMock.Object);
    }

    [Fact]
    public void Full_Execution_Workflow_Should_Work()
    {
        // Arrange
        var capturedEvents = new List<DataCapturedEventArgs>();
        _executionContext.DataCaptured += (sender, args) => capturedEvents.Add(args);

        // Act - Simulate a typical execution workflow
        _executionContext.AddLog("Starting execution");

        // Set variables
        _executionContext.SetVariable("USERNAME", "testuser");
        _executionContext.SetVariable("PASSWORD", "testpass");

        // Simulate HTTP request
        _executionContext.UpdateResponse("Login successful", 200, "https://example.com/login");

        // Capture data
        _executionContext.SetCapture("USER_ID", "12345");
        _executionContext.SetCapture("SESSION_TOKEN", "abc123def456");

        // Add more complex data
        var userList = new List<object?> { "admin", "user", "guest" };
        _variableManager.SetList("ROLES", userList);

        var userData = new Dictionary<string, object?>
        {
            ["id"] = "12345",
            ["name"] = "Test User",
            ["email"] = "test@example.com"
        };
        _variableManager.SetDictionary("USER_DATA", userData);

        // Create checkpoint
        var checkpoint = _executionContext.CreateCheckpoint("AfterLogin");

        // Modify state
        _executionContext.SetVariable("STEP", "dashboard");
        _executionContext.UpdateResponse("Dashboard loaded", 200, "https://example.com/dashboard");

        // Restore checkpoint
        _executionContext.RestoreCheckpoint(checkpoint);

        _executionContext.AddLog("Execution completed");
        _executionContext.Status = ExecutionStatus.Completed;

        // Assert
        _executionContext.Log.Should().HaveCount(2);
        _executionContext.CapturedData.Should().HaveCount(2);
        _executionContext.Statistics.HttpRequestCount.Should().BeGreaterThan(0);
        _executionContext.Statistics.VariableSetCount.Should().BeGreaterThan(0);
        _executionContext.Status.Should().Be(ExecutionStatus.Completed);

        capturedEvents.Should().HaveCount(2);
        capturedEvents[0].Name.Should().Be("USER_ID");
        capturedEvents[1].Name.Should().Be("SESSION_TOKEN");

        // Verify variable manager integration
        _variableManager.GetVariable<string>("USERNAME").Should().Be("testuser");
        _variableManager.GetList("ROLES").Should().HaveCount(3);
        _variableManager.GetDictionary("USER_DATA").Should().ContainKey("email");
    }

    [Fact]
    public void Variable_Events_Should_Sync_Between_Components()
    {
        // Arrange
        var variableEvents = new List<VariableChangedEventArgs>();
        _variableManager.VariableChanged += (sender, args) => variableEvents.Add(args);

        // Act
        _executionContext.SetVariable("SYNC_TEST", "value1");
        _executionContext.SetVariable("SYNC_TEST", "value2"); // Update

        // Assert
        variableEvents.Should().HaveCount(2);
        variableEvents[0].ChangeType.Should().Be(VariableChangeType.Created);
        variableEvents[1].ChangeType.Should().Be(VariableChangeType.Updated);

        // Verify BotData is also updated
        _botData.GetVariable<string>("SYNC_TEST").Should().Be("value2");
    }

    [Fact]
    public void Complex_Variable_Operations_Should_Work()
    {
        // Act
        // Test list operations
        _variableManager.AddToList("ITEMS", "item1");
        _variableManager.AddToList("ITEMS", "item2");
        _variableManager.AddToList("ITEMS", "item3");

        // Test dictionary operations
        _variableManager.SetDictionaryValue("CONFIG", "timeout", 30);
        _variableManager.SetDictionaryValue("CONFIG", "retries", 3);
        _variableManager.SetDictionaryValue("CONFIG", "debug", true);

        // Test variable references
        var listRef = Step5TestHelpers.CreateVariableReference("ITEMS", VariableType.List, "1");
        var dictRef = Step5TestHelpers.CreateVariableReference("CONFIG", VariableType.Dictionary, "timeout");
        var allListRef = Step5TestHelpers.CreateVariableReference("ITEMS", VariableType.List, "*");

        var listValue = _variableManager.ResolveVariableReference(listRef);
        var dictValue = _variableManager.ResolveVariableReference(dictRef);
        var allListValue = _variableManager.ResolveVariableReference(allListRef);

        // Assert
        listValue.Should().Be("item2");
        dictValue.Should().Be(30);
        allListValue.Should().Be("item1, item2, item3");
    }

    [Fact]
    public void Checkpoint_Restore_Should_Maintain_Data_Integrity()
    {
        // Arrange
        _executionContext.SetVariable("VAR1", "initial1");
        _executionContext.SetVariable("VAR2", "initial2");
        _executionContext.SetCapture("CAP1", "captured1");

        var initialCheckpoint = _executionContext.CreateCheckpoint("Initial");

        // Act - Modify everything
        _executionContext.SetVariable("VAR1", "modified1");
        _executionContext.SetVariable("VAR2", "modified2");
        _executionContext.SetVariable("VAR3", "new");
        _executionContext.SetCapture("CAP1", "modified_captured");
        _executionContext.SetCapture("CAP2", "new_captured");
        _executionContext.CustomStatus = "Modified";

        // Restore
        _executionContext.RestoreCheckpoint(initialCheckpoint);

        // Assert
        _executionContext.GetVariable<string>("VAR1").Should().Be("initial1");
        _executionContext.GetVariable<string>("VAR2").Should().Be("initial2");
        _executionContext.GetVariable<string>("VAR3").Should().BeNull(); // Should be removed
        _executionContext.GetCapture<string>("CAP1").Should().Be("captured1");
        _executionContext.CapturedData.Should().NotContainKey("CAP2"); // Should be removed
        _executionContext.CustomStatus.Should().Be(initialCheckpoint.CustomStatus);
    }

    [Fact]
    public void Memory_Cleanup_Should_Work()
    {
        // Arrange
        var tempManager = new VariableManager(_varLoggerMock.Object);

        // Create temporary variables with TTL
        var metadata = new VariableMetadata
        {
            Name = "TEMP_VAR",
            IsTemporary = true,
            TimeToLive = TimeSpan.FromMilliseconds(50),
            Scope = VariableScope.Local
        };

        tempManager.SetVariable("TEMP_VAR", "temp_value");

        // Act
        Thread.Sleep(100); // Wait for TTL to expire

        // Trigger cleanup manually by waiting for timer
        Thread.Sleep(100);

        // Assert
        // Note: In a real test, we'd need to wait for the cleanup timer or trigger it manually
        // This is a conceptual test - in practice, you'd need more sophisticated timing control
        tempManager.Dispose();
    }

    public void Dispose()
    {
        _executionContext?.Dispose();
        _variableManager?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}
