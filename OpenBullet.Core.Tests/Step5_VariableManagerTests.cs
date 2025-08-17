using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Parsing;
using OpenBullet.Core.Variables;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 5 Tests: Variable Manager
/// </summary>
public class Step5_VariableManagerTests : IDisposable
{
    private readonly Mock<ILogger<VariableManager>> _loggerMock;
    private readonly VariableManager _variableManager;

    public Step5_VariableManagerTests()
    {
        _loggerMock = new Mock<ILogger<VariableManager>>();
        _variableManager = new VariableManager(_loggerMock.Object);
    }

    [Fact]
    public void VariableManager_Should_Initialize_Successfully()
    {
        // Assert
        _variableManager.Should().NotBeNull();
        _variableManager.Should().BeAssignableTo<IVariableManager>();
    }

    [Fact]
    public void SetVariable_Should_Store_Variable_In_Local_Scope()
    {
        // Arrange
        const string varName = "testVar";
        const string varValue = "testValue";

        // Act
        _variableManager.SetVariable(varName, varValue, VariableScope.Local);

        // Assert
        _variableManager.HasVariable(varName, VariableScope.Local).Should().BeTrue();
        _variableManager.GetVariable<string>(varName, VariableScope.Local).Should().Be(varValue);
    }

    [Fact]
    public void SetVariable_Should_Store_Variable_In_Global_Scope()
    {
        // Arrange
        const string varName = "globalVar";
        const int varValue = 42;

        // Act
        _variableManager.SetVariable(varName, varValue, VariableScope.Global);

        // Assert
        _variableManager.HasVariable(varName, VariableScope.Global).Should().BeTrue();
        _variableManager.GetVariable<int>(varName, VariableScope.Global).Should().Be(varValue);
    }

    [Fact]
    public void GetVariable_With_Any_Scope_Should_Check_Local_First()
    {
        // Arrange
        const string varName = "conflictVar";
        const string localValue = "localValue";
        const string globalValue = "globalValue";

        // Act
        _variableManager.SetVariable(varName, globalValue, VariableScope.Global);
        _variableManager.SetVariable(varName, localValue, VariableScope.Local);

        var result = _variableManager.GetVariable<string>(varName, VariableScope.Any);

        // Assert
        result.Should().Be(localValue); // Local should override global
    }

    [Fact]
    public void GetVariable_Should_Return_Default_For_Missing_Variable()
    {
        // Arrange
        const string missingVar = "nonExistentVar";

        // Act
        var stringResult = _variableManager.GetVariable<string>(missingVar);
        var intResult = _variableManager.GetVariable<int>(missingVar);

        // Assert
        stringResult.Should().BeNull();
        intResult.Should().Be(0);
    }

    [Fact]
    public void GetVariable_Should_Convert_Types_When_Possible()
    {
        // Arrange
        const string varName = "numberVar";
        const int intValue = 123;

        _variableManager.SetVariable(varName, intValue);

        // Act
        var stringResult = _variableManager.GetVariable<string>(varName);
        var intResult = _variableManager.GetVariable<int>(varName);
        var doubleResult = _variableManager.GetVariable<double>(varName);

        // Assert
        stringResult.Should().Be("123");
        intResult.Should().Be(123);
        doubleResult.Should().Be(123.0);
    }

    [Fact]
    public void HasVariable_Should_Return_Correct_Results()
    {
        // Arrange
        const string localVar = "localVar";
        const string globalVar = "globalVar";

        _variableManager.SetVariable(localVar, "value", VariableScope.Local);
        _variableManager.SetVariable(globalVar, "value", VariableScope.Global);

        // Assert
        _variableManager.HasVariable(localVar, VariableScope.Local).Should().BeTrue();
        _variableManager.HasVariable(localVar, VariableScope.Global).Should().BeFalse();
        _variableManager.HasVariable(localVar, VariableScope.Any).Should().BeTrue();

        _variableManager.HasVariable(globalVar, VariableScope.Local).Should().BeFalse();
        _variableManager.HasVariable(globalVar, VariableScope.Global).Should().BeTrue();
        _variableManager.HasVariable(globalVar, VariableScope.Any).Should().BeTrue();

        _variableManager.HasVariable("nonExistent", VariableScope.Any).Should().BeFalse();
    }

    [Fact]
    public void RemoveVariable_Should_Remove_From_Correct_Scope()
    {
        // Arrange
        const string varName = "removeVar";
        _variableManager.SetVariable(varName, "value", VariableScope.Local);

        // Act
        var removed = _variableManager.RemoveVariable(varName, VariableScope.Local);

        // Assert
        removed.Should().BeTrue();
        _variableManager.HasVariable(varName, VariableScope.Local).Should().BeFalse();
    }

    [Fact]
    public void RemoveVariable_Should_Return_False_For_Missing_Variable()
    {
        // Act
        var removed = _variableManager.RemoveVariable("nonExistent", VariableScope.Local);

        // Assert
        removed.Should().BeFalse();
    }

    [Fact]
    public void GetAllVariables_Should_Return_All_Variables_In_Scope()
    {
        // Arrange - Clear any leftover variables from previous tests
        _variableManager.ClearVariables(VariableScope.Local);
        _variableManager.ClearVariables(VariableScope.Global);
        
        _variableManager.SetVariable("local1", "value1", VariableScope.Local);
        _variableManager.SetVariable("local2", "value2", VariableScope.Local);
        _variableManager.SetVariable("global1", "value3", VariableScope.Global);

        // Act
        var localVars = _variableManager.GetAllVariables(VariableScope.Local);
        var globalVars = _variableManager.GetAllVariables(VariableScope.Global);
        var allVars = _variableManager.GetAllVariables(VariableScope.Any);

        // Assert
        localVars.Should().HaveCount(2);
        localVars.Should().ContainKey("local1");
        localVars.Should().ContainKey("local2");

        globalVars.Should().HaveCount(1);
        globalVars.Should().ContainKey("global1");

        allVars.Should().HaveCount(3);
        allVars.Should().ContainKey("local1");
        allVars.Should().ContainKey("local2");
        allVars.Should().ContainKey("global1");
    }

    [Fact]
    public void ClearVariables_Should_Clear_Correct_Scope()
    {
        // Arrange
        _variableManager.SetVariable("local1", "value1", VariableScope.Local);
        _variableManager.SetVariable("global1", "value2", VariableScope.Global);

        // Act
        _variableManager.ClearVariables(VariableScope.Local);

        // Assert
        _variableManager.HasVariable("local1", VariableScope.Local).Should().BeFalse();
        _variableManager.HasVariable("global1", VariableScope.Global).Should().BeTrue();
    }

    [Fact]
    public void SetList_And_GetList_Should_Work_Correctly()
    {
        // Arrange
        const string listName = "testList";
        var listValues = new List<object?> { "item1", "item2", 42 };

        // Act
        _variableManager.SetList(listName, listValues);
        var retrievedList = _variableManager.GetList(listName);

        // Assert
        retrievedList.Should().NotBeNull();
        retrievedList.Should().HaveCount(3);
        retrievedList.Should().BeEquivalentTo(listValues);
    }

    [Fact]
    public void AddToList_Should_Add_Item_To_Existing_List()
    {
        // Arrange
        const string listName = "testList";
        var initialList = new List<object?> { "item1" };
        _variableManager.SetList(listName, initialList);

        // Act
        _variableManager.AddToList(listName, "item2");
        var retrievedList = _variableManager.GetList(listName);

        // Assert
        retrievedList.Should().HaveCount(2);
        retrievedList.Should().Contain("item1");
        retrievedList.Should().Contain("item2");
    }

    [Fact]
    public void AddToList_Should_Create_List_If_Not_Exists()
    {
        // Arrange
        const string listName = "newList";

        // Act
        _variableManager.AddToList(listName, "firstItem");
        var retrievedList = _variableManager.GetList(listName);

        // Assert
        retrievedList.Should().NotBeNull();
        retrievedList.Should().HaveCount(1);
        retrievedList.Should().Contain("firstItem");
    }

    [Fact]
    public void SetDictionary_And_GetDictionary_Should_Work_Correctly()
    {
        // Arrange
        const string dictName = "testDict";
        var dictValues = new Dictionary<string, object?>
        {
            ["key1"] = "value1",
            ["key2"] = 42,
            ["key3"] = true
        };

        // Act
        _variableManager.SetDictionary(dictName, dictValues);
        var retrievedDict = _variableManager.GetDictionary(dictName);

        // Assert
        retrievedDict.Should().NotBeNull();
        retrievedDict.Should().HaveCount(3);
        retrievedDict.Should().BeEquivalentTo(dictValues);
    }

    [Fact]
    public void SetDictionaryValue_Should_Set_Value_In_Existing_Dictionary()
    {
        // Arrange
        const string dictName = "testDict";
        var initialDict = new Dictionary<string, object?> { ["key1"] = "value1" };
        _variableManager.SetDictionary(dictName, initialDict);

        // Act
        _variableManager.SetDictionaryValue(dictName, "key2", "value2");
        var retrievedDict = _variableManager.GetDictionary(dictName);

        // Assert
        retrievedDict.Should().HaveCount(2);
        retrievedDict.Should().ContainKey("key1");
        retrievedDict.Should().ContainKey("key2");
        retrievedDict!["key2"].Should().Be("value2");
    }

    [Fact]
    public void SetDictionaryValue_Should_Create_Dictionary_If_Not_Exists()
    {
        // Arrange
        const string dictName = "newDict";

        // Act
        _variableManager.SetDictionaryValue(dictName, "key1", "value1");
        var retrievedDict = _variableManager.GetDictionary(dictName);

        // Assert
        retrievedDict.Should().NotBeNull();
        retrievedDict.Should().HaveCount(1);
        retrievedDict.Should().ContainKey("key1");
        retrievedDict!["key1"].Should().Be("value1");
    }

    [Fact]
    public void ResolveVariableReference_Should_Handle_Simple_Variable()
    {
        // Arrange
        const string varName = "simpleVar";
        const string varValue = "simpleValue";
        _variableManager.SetVariable(varName, varValue);

        var reference = new VariableReference
        {
            VariableName = varName,
            Type = VariableType.Single
        };

        // Act
        var result = _variableManager.ResolveVariableReference(reference);

        // Assert
        result.Should().Be(varValue);
    }

    [Fact]
    public void ResolveVariableReference_Should_Handle_List_Variable()
    {
        // Arrange
        const string listName = "testList";
        var listValues = new List<object?> { "item0", "item1", "item2" };
        _variableManager.SetList(listName, listValues);

        var indexReference = new VariableReference
        {
            VariableName = listName,
            Type = VariableType.List,
            IndexOrKey = "1"
        };

        var allReference = new VariableReference
        {
            VariableName = listName,
            Type = VariableType.List,
            IndexOrKey = "*"
        };

        // Act
        var indexResult = _variableManager.ResolveVariableReference(indexReference);
        var allResult = _variableManager.ResolveVariableReference(allReference);

        // Assert
        indexResult.Should().Be("item1");
        allResult.Should().Be("item0, item1, item2");
    }

    [Fact]
    public void ResolveVariableReference_Should_Handle_Dictionary_Variable()
    {
        // Arrange
        const string dictName = "testDict";
        var dictValues = new Dictionary<string, object?>
        {
            ["name"] = "John",
            ["age"] = 30
        };
        _variableManager.SetDictionary(dictName, dictValues);

        var keyReference = new VariableReference
        {
            VariableName = dictName,
            Type = VariableType.Dictionary,
            IndexOrKey = "name"
        };

        var allReference = new VariableReference
        {
            VariableName = dictName,
            Type = VariableType.Dictionary,
            IndexOrKey = "*"
        };

        // Act
        var keyResult = _variableManager.ResolveVariableReference(keyReference);
        var allResult = _variableManager.ResolveVariableReference(allReference);

        // Assert
        keyResult.Should().Be("John");
        allResult?.ToString().Should().Contain("John");
        allResult?.ToString().Should().Contain("30");
    }

    [Fact]
    public void GetVariableMetadata_Should_Return_Metadata()
    {
        // Arrange
        const string varName = "testVar";
        _variableManager.SetVariable(varName, "value");

        // Act
        var metadata = _variableManager.GetVariableMetadata(varName);

        // Assert
        metadata.Should().NotBeNull();
        metadata!.Name.Should().Be(varName);
        metadata.Scope.Should().Be(VariableScope.Local);
        metadata.ValueType.Should().Be(typeof(string));
        metadata.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void GetStatistics_Should_Return_Correct_Statistics()
    {
        // Arrange - Clear any leftover variables from previous tests
        _variableManager.ClearVariables(VariableScope.Local);
        _variableManager.ClearVariables(VariableScope.Global);
        
        _variableManager.SetVariable("local1", "value1", VariableScope.Local);
        _variableManager.SetVariable("local2", 42, VariableScope.Local);
        _variableManager.SetVariable("global1", "value3", VariableScope.Global);
        
        var list = new List<object?> { "item1", "item2" };
        _variableManager.SetList("list1", list);
        
        var dict = new Dictionary<string, object?> { ["key"] = "value" };
        _variableManager.SetDictionary("dict1", dict);

        // Act
        var statistics = _variableManager.GetStatistics();

        // Assert
        statistics.LocalVariableCount.Should().Be(4); // local1, local2, list1, dict1
        statistics.GlobalVariableCount.Should().Be(1);
        statistics.TotalVariableCount.Should().Be(5);
        statistics.TypeCounts.Should().ContainKey("String");
        statistics.TypeCounts.Should().ContainKey("Int32");
    }

    [Fact]
    public void CreateSnapshot_And_RestoreSnapshot_Should_Work()
    {
        // Arrange
        _variableManager.SetVariable("var1", "value1", VariableScope.Local);
        _variableManager.SetVariable("var2", "value2", VariableScope.Global);

        // Act
        var snapshot = _variableManager.CreateSnapshot();
        
        // Modify variables
        _variableManager.SetVariable("var1", "modified", VariableScope.Local);
        _variableManager.RemoveVariable("var2", VariableScope.Global);
        
        // Restore
        _variableManager.RestoreSnapshot(snapshot);

        // Assert
        _variableManager.GetVariable<string>("var1").Should().Be("value1");
        _variableManager.GetVariable<string>("var2").Should().Be("value2");
    }

    [Fact]
    public void VariableChanged_Event_Should_Be_Fired()
    {
        // Arrange
        var eventFired = false;
        VariableChangedEventArgs? capturedArgs = null;

        _variableManager.VariableChanged += (sender, args) =>
        {
            eventFired = true;
            capturedArgs = args;
        };

        // Act
        _variableManager.SetVariable("testVar", "testValue");

        // Assert
        eventFired.Should().BeTrue();
        capturedArgs.Should().NotBeNull();
        capturedArgs!.VariableName.Should().Be("testVar");
        capturedArgs.NewValue.Should().Be("testValue");
        capturedArgs.ChangeType.Should().Be(VariableChangeType.Created);
    }

    [Theory]
    [InlineData(VariableScope.Local)]
    [InlineData(VariableScope.Global)]
    [InlineData(VariableScope.Any)]
    public void SetVariable_Should_Throw_For_Empty_Name(VariableScope scope)
    {
        // Act & Assert
        var act = () => _variableManager.SetVariable("", "value", scope);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetVariable_Should_Throw_For_Any_Scope()
    {
        // Act & Assert
        var act = () => _variableManager.SetVariable("test", "value", VariableScope.Any);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task Concurrent_Access_Should_Be_Thread_Safe()
    {
        // Arrange
        const int threadCount = 10;
        const int operationsPerThread = 100;
        var tasks = new List<Task>();

        // Act
        for (int t = 0; t < threadCount; t++)
        {
            var threadId = t;
            tasks.Add(Task.Run(() =>
            {
                for (int i = 0; i < operationsPerThread; i++)
                {
                    var varName = $"thread{threadId}_var{i}";
                    _variableManager.SetVariable(varName, i, VariableScope.Local);
                    var value = _variableManager.GetVariable<int>(varName);
                    value.Should().Be(i);
                }
            }));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        var statistics = _variableManager.GetStatistics();
        statistics.LocalVariableCount.Should().Be(threadCount * operationsPerThread);
    }

    public void Dispose()
    {
        _variableManager?.Dispose();
    }
}

/// <summary>
/// Step 5 Test Helpers
/// </summary>
public static class Step5TestHelpers
{
    public static VariableReference CreateVariableReference(string name, VariableType type, string? indexOrKey = null)
    {
        return new VariableReference
        {
            VariableName = name,
            Type = type,
            IndexOrKey = indexOrKey,
            Position = 0,
            Length = name.Length + 2,
            OriginalText = $"<{name}>"
        };
    }

    public static VariableManager CreateVariableManager()
    {
        var loggerMock = new Mock<ILogger<VariableManager>>();
        return new VariableManager(loggerMock.Object);
    }

    public static void PopulateWithTestData(IVariableManager manager)
    {
        manager.SetVariable("USER", "testuser", VariableScope.Local);
        manager.SetVariable("PASS", "testpass", VariableScope.Local);
        manager.SetVariable("TOKEN", "abc123", VariableScope.Global);
        
        var items = new List<object?> { "apple", "banana", "cherry" };
        manager.SetList("ITEMS", items);
        
        var user = new Dictionary<string, object?>
        {
            ["name"] = "John Doe",
            ["age"] = 30,
            ["admin"] = true
        };
        manager.SetDictionary("USER_DATA", user);
    }

    public static VariableStatistics CreateTestStatistics()
    {
        return new VariableStatistics
        {
            LocalVariableCount = 10,
            GlobalVariableCount = 5,
            ListVariableCount = 2,
            DictionaryVariableCount = 1,
            SimpleVariableCount = 12,
            MemoryUsageBytes = 1024,
            TypeCounts = new Dictionary<string, int>
            {
                ["String"] = 8,
                ["Int32"] = 4,
                ["Boolean"] = 3
            }
        };
    }

    public static VariableSnapshot CreateTestSnapshot()
    {
        return new VariableSnapshot
        {
            Description = "Test snapshot",
            LocalVariables = new Dictionary<string, object?>
            {
                ["local1"] = "value1",
                ["local2"] = 42
            },
            GlobalVariables = new Dictionary<string, object?>
            {
                ["global1"] = "globalValue"
            }
        };
    }
}
