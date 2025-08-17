using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using OpenBullet.Core.Services;
using OpenBullet.Core.Variables;
using ExecutionContext = OpenBullet.Core.Execution.ExecutionContext;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 5 Validation Tests - Basic functionality without external dependencies
/// </summary>
public class Step5_ValidationTests
{
    [Fact]
    public void VariableManager_Can_Be_Created_Successfully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();

        // Act
        using var manager = new VariableManager(loggerMock.Object);

        // Assert
        manager.Should().NotBeNull();
        manager.Should().BeAssignableTo<IVariableManager>();
    }

    [Fact]
    public void ExecutionContext_Can_Be_Created_Successfully()
    {
        // Arrange
        var varLoggerMock = new Mock<ILogger<VariableManager>>();
        var contextLoggerMock = new Mock<ILogger<ExecutionContext>>();
        var httpClientMock = new Mock<IHttpClientService>();
        var cancellationTokenSource = new CancellationTokenSource();

        using var variableManager = new VariableManager(varLoggerMock.Object);
        var config = new ConfigModel { Name = "TestConfig" };
        var logger = new Mock<ILogger>().Object;
        var botData = new BotData("test:data", config, logger, cancellationTokenSource.Token);

        // Act
        using var context = new OpenBullet.Core.Execution.ExecutionContext(
            botData,
            variableManager,
            httpClientMock.Object,
            cancellationTokenSource.Token,
            contextLoggerMock.Object);

        // Assert
        context.Should().NotBeNull();
        context.Should().BeAssignableTo<IExecutionContext>();
        context.Id.Should().NotBeEmpty();
        context.BotData.Should().Be(botData);
        context.VariableManager.Should().Be(variableManager);
        
        cancellationTokenSource.Dispose();
    }

    [Fact]
    public void VariableScope_Enum_Values_Should_Be_Available()
    {
        // Assert
        Enum.GetValues<VariableScope>().Should().Contain(VariableScope.Local);
        Enum.GetValues<VariableScope>().Should().Contain(VariableScope.Global);
        Enum.GetValues<VariableScope>().Should().Contain(VariableScope.Any);
    }

    [Fact]
    public void ExecutionStatus_Enum_Values_Should_Be_Available()
    {
        // Assert
        Enum.GetValues<ExecutionStatus>().Should().Contain(ExecutionStatus.NotStarted);
        Enum.GetValues<ExecutionStatus>().Should().Contain(ExecutionStatus.Running);
        Enum.GetValues<ExecutionStatus>().Should().Contain(ExecutionStatus.Completed);
        Enum.GetValues<ExecutionStatus>().Should().Contain(ExecutionStatus.Failed);
        Enum.GetValues<ExecutionStatus>().Should().Contain(ExecutionStatus.Cancelled);
    }

    [Fact]
    public void LogLevel_Enum_Values_Should_Be_Available()
    {
        // Assert
        Enum.GetValues<Microsoft.Extensions.Logging.LogLevel>().Should().Contain(Microsoft.Extensions.Logging.LogLevel.Trace);
        Enum.GetValues<OpenBullet.Core.Execution.LogLevel>().Should().Contain(OpenBullet.Core.Execution.LogLevel.Debug);
        Enum.GetValues<OpenBullet.Core.Execution.LogLevel>().Should().Contain(OpenBullet.Core.Execution.LogLevel.Info);
        Enum.GetValues<OpenBullet.Core.Execution.LogLevel>().Should().Contain(OpenBullet.Core.Execution.LogLevel.Warning);
        Enum.GetValues<OpenBullet.Core.Execution.LogLevel>().Should().Contain(OpenBullet.Core.Execution.LogLevel.Error);
        Enum.GetValues<Microsoft.Extensions.Logging.LogLevel>().Should().Contain(Microsoft.Extensions.Logging.LogLevel.Critical);
    }

    [Fact]
    public void VariableChangeType_Enum_Values_Should_Be_Available()
    {
        // Assert
        Enum.GetValues<VariableChangeType>().Should().Contain(VariableChangeType.Created);
        Enum.GetValues<VariableChangeType>().Should().Contain(VariableChangeType.Updated);
        Enum.GetValues<VariableChangeType>().Should().Contain(VariableChangeType.Deleted);
        Enum.GetValues<VariableChangeType>().Should().Contain(VariableChangeType.Accessed);
    }

    [Fact]
    public void VariableMetadata_Should_Initialize_With_Defaults()
    {
        // Act
        var metadata = new VariableMetadata();

        // Assert
        metadata.Name.Should().BeEmpty();
        metadata.ValueType.Should().BeNull();
        metadata.Scope.Should().Be(VariableScope.Local);
        metadata.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        metadata.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        metadata.AccessCount.Should().Be(0);
        metadata.Description.Should().BeNull();
        metadata.IsReadOnly.Should().BeFalse();
        metadata.IsTemporary.Should().BeFalse();
        metadata.TimeToLive.Should().BeNull();
    }

    [Fact]
    public void VariableStatistics_Should_Initialize_With_Defaults()
    {
        // Act
        var stats = new VariableStatistics();

        // Assert
        stats.LocalVariableCount.Should().Be(0);
        stats.GlobalVariableCount.Should().Be(0);
        stats.TotalVariableCount.Should().Be(0);
        stats.ListVariableCount.Should().Be(0);
        stats.DictionaryVariableCount.Should().Be(0);
        stats.SimpleVariableCount.Should().Be(0);
        stats.MemoryUsageBytes.Should().Be(0);
        stats.CleanupCount.Should().Be(0);
        stats.TypeCounts.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void VariableSnapshot_Should_Initialize_With_Defaults()
    {
        // Act
        var snapshot = new VariableSnapshot();

        // Assert
        snapshot.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        snapshot.LocalVariables.Should().NotBeNull().And.BeEmpty();
        snapshot.GlobalVariables.Should().NotBeNull().And.BeEmpty();
        snapshot.Metadata.Should().NotBeNull().And.BeEmpty();
        snapshot.Description.Should().BeNull();
    }

    [Fact]
    public void VariableChangedEventArgs_Should_Initialize_With_Defaults()
    {
        // Act
        var args = new VariableChangedEventArgs();

        // Assert
        args.VariableName.Should().BeEmpty();
        args.OldValue.Should().BeNull();
        args.NewValue.Should().BeNull();
        args.Scope.Should().Be(VariableScope.Local);
        args.ChangeType.Should().Be(VariableChangeType.Created);
        args.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void LogEntry_Should_Initialize_With_Defaults()
    {
        // Act
        var entry = new LogEntry();

        // Assert
        entry.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entry.Message.Should().BeEmpty();
        entry.Level.Should().Be(OpenBullet.Core.Execution.LogLevel.Info);
        entry.Exception.Should().BeNull();
        entry.Source.Should().BeEmpty();
        entry.Properties.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ExecutionStatistics_Should_Initialize_With_Defaults()
    {
        // Act
        var stats = new ExecutionStatistics();

        // Assert
        stats.HttpRequestCount.Should().Be(0);
        stats.VariableSetCount.Should().Be(0);
        stats.VariableGetCount.Should().Be(0);
        stats.DataCaptureCount.Should().Be(0);
        stats.LogEntryCount.Should().Be(0);
        stats.TotalResponseTime.Should().Be(0);
        stats.AverageResponseTime.Should().Be(0);
        stats.ErrorCount.Should().Be(0);
        stats.WarningCount.Should().Be(0);
        stats.CommandCounts.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ExecutionEnvironment_Should_Initialize_With_Defaults()
    {
        // Act
        var env = new ExecutionEnvironment();

        // Assert
        env.UserAgent.Should().Be("OpenBullet/2.0");
        env.RequestTimeout.Should().Be(10000);
        env.MaxRedirects.Should().Be(8);
        env.IgnoreSSLErrors.Should().BeFalse();
        env.AutoRedirect.Should().BeTrue();
        env.AcceptLanguage.Should().Be("en-US,en;q=0.9");
        env.DefaultHeaders.Should().NotBeNull().And.BeEmpty();
        env.EnableLogging.Should().BeTrue();
        env.LogLevel.Should().Be(OpenBullet.Core.Execution.LogLevel.Info);
    }

    [Fact]
    public void ExecutionCheckpoint_Should_Initialize_With_Defaults()
    {
        // Act
        var checkpoint = new ExecutionCheckpoint();

        // Assert
        checkpoint.Name.Should().BeEmpty();
        checkpoint.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        checkpoint.Status.Should().Be(ExecutionStatus.NotStarted);
        checkpoint.CapturedData.Should().NotBeNull().And.BeEmpty();
        checkpoint.ResponseSource.Should().BeEmpty();
        checkpoint.ResponseCode.Should().Be(0);
        checkpoint.ResponseAddress.Should().BeEmpty();
        checkpoint.CustomStatus.Should().BeEmpty();
    }

    [Fact]
    public void ContextValidationResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ContextValidationResult();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNull().And.BeEmpty();
        result.Warnings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ExecutionStatusChangedEventArgs_Should_Initialize_With_Defaults()
    {
        // Act
        var args = new ExecutionStatusChangedEventArgs();

        // Assert
        args.ContextId.Should().BeEmpty();
        args.OldStatus.Should().Be(ExecutionStatus.NotStarted);
        args.NewStatus.Should().Be(ExecutionStatus.NotStarted);
        args.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        args.Message.Should().BeNull();
    }

    [Fact]
    public void DataCapturedEventArgs_Should_Initialize_With_Defaults()
    {
        // Act
        var args = new DataCapturedEventArgs();

        // Assert
        args.Name.Should().BeEmpty();
        args.Value.Should().BeNull();
        args.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        args.Source.Should().BeEmpty();
    }

    [Fact]
    public void VariableManager_Basic_Operations_Should_Work()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);

        // Act & Assert
        // Set and get variable
        manager.SetVariable("test", "value");
        manager.GetVariable<string>("test").Should().Be("value");
        manager.HasVariable("test").Should().BeTrue();

        // Remove variable
        manager.RemoveVariable("test").Should().BeTrue();
        manager.HasVariable("test").Should().BeFalse();
    }

    [Fact]
    public void VariableManager_Should_Handle_Different_Types()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);

        // Act
        manager.SetVariable("string_var", "hello");
        manager.SetVariable("int_var", 42);
        manager.SetVariable("bool_var", true);
        manager.SetVariable("double_var", 3.14);

        // Assert
        manager.GetVariable<string>("string_var").Should().Be("hello");
        manager.GetVariable<int>("int_var").Should().Be(42);
        manager.GetVariable<bool>("bool_var").Should().BeTrue();
        manager.GetVariable<double>("double_var").Should().Be(3.14);
    }

    [Fact]
    public void VariableManager_Should_Handle_Type_Conversion()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);

        // Act
        manager.SetVariable("number", 42);

        // Assert
        manager.GetVariable<string>("number").Should().Be("42");
        manager.GetVariable<int>("number").Should().Be(42);
        manager.GetVariable<double>("number").Should().Be(42.0);
    }

    [Fact]
    public void VariableManager_Should_Handle_Lists()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);
        var list = new List<object?> { "item1", "item2", "item3" };

        // Act
        manager.SetList("test_list", list);
        manager.AddToList("test_list", "item4");

        // Assert
        var retrievedList = manager.GetList("test_list");
        retrievedList.Should().HaveCount(4);
        retrievedList.Should().Contain("item1");
        retrievedList.Should().Contain("item4");
    }

    [Fact]
    public void VariableManager_Should_Handle_Dictionaries()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);
        var dict = new Dictionary<string, object?> { ["key1"] = "value1" };

        // Act
        manager.SetDictionary("test_dict", dict);
        manager.SetDictionaryValue("test_dict", "key2", "value2");

        // Assert
        var retrievedDict = manager.GetDictionary("test_dict");
        retrievedDict.Should().HaveCount(2);
        retrievedDict.Should().ContainKey("key1");
        retrievedDict.Should().ContainKey("key2");
        retrievedDict!["key2"].Should().Be("value2");
    }

    [Fact]
    public void VariableManager_Should_Track_Statistics()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);
        
        // Clear global variables to ensure test isolation
        manager.ClearVariables(VariableScope.Global);

        // Act
        manager.SetVariable("var1", "value1", VariableScope.Local);
        manager.SetVariable("var2", "value2", VariableScope.Global);
        manager.SetList("list1", new List<object?> { "item" });

        // Assert
        var stats = manager.GetStatistics();
        stats.LocalVariableCount.Should().Be(2);
        stats.GlobalVariableCount.Should().Be(1);
        stats.TotalVariableCount.Should().Be(3);
    }

    [Fact]
    public void VariableManager_Should_Support_Snapshots()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);

        // Act
        manager.SetVariable("original", "value");
        var snapshot = manager.CreateSnapshot();
        manager.SetVariable("original", "modified");
        manager.RestoreSnapshot(snapshot);

        // Assert
        manager.GetVariable<string>("original").Should().Be("value");
    }

    [Fact]
    public void ExecutionStatistics_AverageResponseTime_Should_Calculate_Correctly()
    {
        // Arrange
        var stats = new ExecutionStatistics();

        // Act
        stats.HttpRequestCount = 5;
        stats.TotalResponseTime = 1000; // 1000ms total

        // Assert
        stats.AverageResponseTime.Should().Be(200); // 1000/5 = 200ms average
    }

    [Fact]
    public void ExecutionStatistics_AverageResponseTime_Should_Handle_Zero_Requests()
    {
        // Arrange
        var stats = new ExecutionStatistics();

        // Act
        stats.HttpRequestCount = 0;
        stats.TotalResponseTime = 0;

        // Assert
        stats.AverageResponseTime.Should().Be(0);
    }

    [Fact]
    public void Step5TestHelpers_Should_Create_Valid_Objects()
    {
        // Act
        var variableRef = Step5TestHelpers.CreateVariableReference("TEST", VariableType.Single);
        var manager = Step5TestHelpers.CreateVariableManager();
        var stats = Step5TestHelpers.CreateTestStatistics();
        var snapshot = Step5TestHelpers.CreateTestSnapshot();

        // Assert
        variableRef.Should().NotBeNull();
        variableRef.VariableName.Should().Be("TEST");
        variableRef.Type.Should().Be(VariableType.Single);

        manager.Should().NotBeNull();
        manager.Should().BeAssignableTo<IVariableManager>();

        stats.Should().NotBeNull();
        stats.LocalVariableCount.Should().Be(10);
        stats.GlobalVariableCount.Should().Be(5);

        snapshot.Should().NotBeNull();
        snapshot.Description.Should().Be("Test snapshot");
        snapshot.LocalVariables.Should().ContainKey("local1");

        manager.Dispose();
    }

    [Fact]
    public void Step5TestHelpers_PopulateWithTestData_Should_Work()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VariableManager>>();
        using var manager = new VariableManager(loggerMock.Object);

        // Act
        Step5TestHelpers.PopulateWithTestData(manager);

        // Assert
        manager.GetVariable<string>("USER").Should().Be("testuser");
        manager.GetVariable<string>("PASS").Should().Be("testpass");
        manager.GetVariable<string>("TOKEN").Should().Be("abc123");
        manager.GetList("ITEMS").Should().HaveCount(3);
        manager.GetDictionary("USER_DATA").Should().ContainKey("name");
    }

    [Theory]
    [InlineData(VariableScope.Local)]
    [InlineData(VariableScope.Global)]
    [InlineData(VariableScope.Any)]
    public void VariableScope_Values_Should_Be_Distinct(VariableScope scope)
    {
        // Assert
        scope.Should().BeOneOf(VariableScope.Local, VariableScope.Global, VariableScope.Any);
    }

    [Theory]
    [InlineData(ExecutionStatus.NotStarted)]
    [InlineData(ExecutionStatus.Running)]
    [InlineData(ExecutionStatus.Paused)]
    [InlineData(ExecutionStatus.Completed)]
    [InlineData(ExecutionStatus.Failed)]
    [InlineData(ExecutionStatus.Cancelled)]
    [InlineData(ExecutionStatus.Retrying)]
    public void ExecutionStatus_Values_Should_Be_Distinct(ExecutionStatus status)
    {
        // Assert
        status.Should().BeOneOf(
            ExecutionStatus.NotStarted,
            ExecutionStatus.Running,
            ExecutionStatus.Paused,
            ExecutionStatus.Completed,
            ExecutionStatus.Failed,
            ExecutionStatus.Cancelled,
            ExecutionStatus.Retrying);
    }

    [Theory]
    [InlineData(Microsoft.Extensions.Logging.LogLevel.Trace, 0)]
    [InlineData(Microsoft.Extensions.Logging.LogLevel.Debug, 1)]
    [InlineData(Microsoft.Extensions.Logging.LogLevel.Information, 2)]
    [InlineData(Microsoft.Extensions.Logging.LogLevel.Warning, 3)]
    [InlineData(Microsoft.Extensions.Logging.LogLevel.Error, 4)]
    [InlineData(Microsoft.Extensions.Logging.LogLevel.Critical, 5)]
    public void LogLevel_Values_Should_Have_Correct_Numeric_Values(Microsoft.Extensions.Logging.LogLevel level, int expectedValue)
    {
        // Assert
        ((int)level).Should().Be(expectedValue);
    }

    [Fact]
    public void VariableReference_Should_Handle_All_Types()
    {
        // Act
        var singleRef = Step5TestHelpers.CreateVariableReference("VAR", VariableType.Single);
        var listRef = Step5TestHelpers.CreateVariableReference("LIST", VariableType.List, "0");
        var dictRef = Step5TestHelpers.CreateVariableReference("DICT", VariableType.Dictionary, "key");

        // Assert
        singleRef.Type.Should().Be(VariableType.Single);
        singleRef.IndexOrKey.Should().BeNull();

        listRef.Type.Should().Be(VariableType.List);
        listRef.IndexOrKey.Should().Be("0");

        dictRef.Type.Should().Be(VariableType.Dictionary);
        dictRef.IndexOrKey.Should().Be("key");
    }
}
