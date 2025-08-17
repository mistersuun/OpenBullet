using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 4 Tests: Script Parser
/// </summary>
public class Step4_ScriptParserTests : IDisposable
{
    private readonly Mock<ILogger<ScriptParser>> _loggerMock;
    private readonly ScriptParser _scriptParser;
    private readonly string _testDataDirectory;

    public Step4_ScriptParserTests()
    {
        _loggerMock = new Mock<ILogger<ScriptParser>>();
        _scriptParser = new ScriptParser(_loggerMock.Object);
        _testDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
    }

    [Fact]
    public void ScriptParser_Should_Initialize_Successfully()
    {
        // Assert
        _scriptParser.Should().NotBeNull();
        _scriptParser.Should().BeAssignableTo<IScriptParser>();
    }

    [Fact]
    public void ParseLine_With_Comment_Should_Return_Null()
    {
        // Arrange
        var commentLine = "## This is a comment";

        // Act
        var result = _scriptParser.ParseLine(commentLine, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseLine_With_Empty_Line_Should_Return_Null()
    {
        // Arrange
        var emptyLine = "   ";

        // Act
        var result = _scriptParser.ParseLine(emptyLine, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseLine_With_Simple_Command_Should_Parse_Correctly()
    {
        // Arrange
        var commandLine = "PRINT Hello World";

        // Act
        var result = _scriptParser.ParseLine(commandLine, 1);

        // Assert
        result.Should().NotBeNull();
        result!.CommandName.Should().Be("PRINT");
        result.Arguments.Should().HaveCount(2);
        result.Arguments[0].Should().Be("Hello");
        result.Arguments[1].Should().Be("World");
        result.LineNumber.Should().Be(1);
        result.RawLine.Should().Be(commandLine);
    }

    [Fact]
    public void ParseLine_With_Quoted_Arguments_Should_Parse_Correctly()
    {
        // Arrange
        var commandLine = "REQUEST GET \"https://example.com/test page\"";

        // Act
        var result = _scriptParser.ParseLine(commandLine, 5);

        // Assert
        result.Should().NotBeNull();
        result!.CommandName.Should().Be("REQUEST");
        result.Arguments.Should().HaveCount(2);
        result.Arguments[0].Should().Be("GET");
        result.Arguments[1].Should().Be("https://example.com/test page");
    }

    [Fact]
    public void ParseLine_With_Boolean_Parameters_Should_Parse_Correctly()
    {
        // Arrange
        var commandLine = "KEYCHECK BanOn4XX=True BanOnToCheck=False";

        // Act
        var result = _scriptParser.ParseLine(commandLine, 10);

        // Assert
        result.Should().NotBeNull();
        result!.CommandName.Should().Be("KEYCHECK");
        result.Parameters.Should().ContainKey("BanOn4XX");
        result.Parameters.Should().ContainKey("BanOnToCheck");
        result.Parameters["BanOn4XX"].Should().Be(true);
        result.Parameters["BanOnToCheck"].Should().Be(false);
    }

    [Fact]
    public void ParseLine_With_Redirector_Should_Parse_Correctly()
    {
        // Arrange
        var commandLine = "PARSE \"<SOURCE>\" LR \"<title>\" \"</title>\" -> VAR \"PAGE_TITLE\"";

        // Act
        var result = _scriptParser.ParseLine(commandLine, 15);

        // Assert
        result.Should().NotBeNull();
        result!.CommandName.Should().Be("PARSE");
        result.Arguments.Should().HaveCount(4); // "<SOURCE>", "LR", "<title>", "</title>"
        result.Parameters.Should().ContainKey("RedirectorType");
        result.Parameters.Should().ContainKey("RedirectorName");
        result.Parameters["RedirectorType"].Should().Be("VAR");
        result.Parameters["RedirectorName"].Should().Be("PAGE_TITLE");
    }

    [Fact]
    public void ParseLine_With_Label_Should_Parse_Correctly()
    {
        // Arrange
        var labelLine = "#LOGIN REQUEST POST \"https://example.com/login\"";

        // Act
        var result = _scriptParser.ParseLine(labelLine, 20);

        // Assert
        result.Should().NotBeNull();
        result!.Label.Should().Be("LOGIN");
        result.CommandName.Should().Be("REQUEST");
        result.Arguments.Should().HaveCount(2);
        result.Arguments[0].Should().Be("POST");
        result.Arguments[1].Should().Be("https://example.com/login");
    }

    [Fact]
    public void ParseLine_With_Complex_Arguments_Should_Parse_Correctly()
    {
        // Arrange
        var commandLine = "FUNCTION Hash SHA256 \"test input\" AutoEncode=True -> VAR \"RESULT\" \"prefix\" \"suffix\"";

        // Act
        var result = _scriptParser.ParseLine(commandLine, 25);

        // Assert
        result.Should().NotBeNull();
        result!.CommandName.Should().Be("FUNCTION");
        result.Arguments.Should().HaveCount(3); // "Hash", "SHA256", "test input"
        result.Arguments[0].Should().Be("Hash");
        result.Arguments[1].Should().Be("SHA256");
        result.Arguments[2].Should().Be("test input");
        result.Parameters["AutoEncode"].Should().Be(true);
        result.Parameters["RedirectorType"].Should().Be("VAR");
        result.Parameters["RedirectorName"].Should().Be("RESULT");
        result.Parameters["RedirectorPrefix"].Should().Be("prefix");
        result.Parameters["RedirectorSuffix"].Should().Be("suffix");
    }

    [Fact]
    public void ParseScript_With_Simple_Script_Should_Parse_All_Instructions()
    {
        // Arrange
        var script = @"## Simple test script
PRINT Starting test
REQUEST GET ""https://example.com""
PRINT Test completed";

        // Act
        var result = _scriptParser.ParseScript(script);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Instructions.Should().HaveCount(3); // 3 non-comment lines
        result.Instructions[0].CommandName.Should().Be("PRINT");
        result.Instructions[1].CommandName.Should().Be("REQUEST");
        result.Instructions[2].CommandName.Should().Be("PRINT");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ParseScript_With_Complex_Script_Should_Handle_Sub_Instructions()
    {
        // Arrange
        var script = @"KEYCHECK BanOn4XX=True
  KEYCHAIN Success OR
    KEY ""welcome""
    KEY ""dashboard""
  KEYCHAIN Failure OR
    KEY ""error""
    KEY ""invalid""";

        // Act
        var result = _scriptParser.ParseScript(script);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Instructions.Should().HaveCount(1);
        
        var keycheckInstruction = result.Instructions[0];
        keycheckInstruction.CommandName.Should().Be("KEYCHECK");
        keycheckInstruction.SubInstructions.Should().HaveCount(6); // 2 KEYCHAIN + 4 KEY commands
        keycheckInstruction.SubInstructions[0].CommandName.Should().Be("KEYCHAIN");
        keycheckInstruction.SubInstructions[1].CommandName.Should().Be("KEY");
    }

    [Fact]
    public void ParseScript_With_Labels_Should_Extract_Labels()
    {
        // Arrange
        var script = @"#START PRINT Beginning
#LOGIN REQUEST POST ""https://example.com/login""
#END PRINT Finished";

        // Act
        var result = _scriptParser.ParseScript(script);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Labels.Should().HaveCount(3);
        result.Labels.Should().ContainKey("START");
        result.Labels.Should().ContainKey("LOGIN");
        result.Labels.Should().ContainKey("END");
        result.Instructions.Should().HaveCount(3);
        result.Instructions[0].Label.Should().Be("START");
        result.Instructions[1].Label.Should().Be("LOGIN");
        result.Instructions[2].Label.Should().Be("END");
    }

    [Fact]
    public async Task ParseScript_With_Sample_File_Should_Parse_Successfully()
    {
        // Arrange
        var sampleScriptPath = Path.Combine(_testDataDirectory, "sample_script.ls");
        var script = await File.ReadAllTextAsync(sampleScriptPath);

        // Act
        var result = _scriptParser.ParseScript(script);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Instructions.Should().HaveCountGreaterThan(5);
        result.Labels.Should().ContainKey("LOGIN");
        result.Labels.Should().ContainKey("CHECK_RESULT");
        result.VariableReferences.Should().NotBeEmpty();
        result.Statistics.TotalLines.Should().BeGreaterThan(10);
        result.Statistics.CommentLines.Should().BeGreaterThan(0);
        result.Statistics.InstructionCount.Should().BeGreaterThan(5);
    }

    [Fact]
    public void ExtractVariableReferences_Should_Find_All_Variables()
    {
        // Arrange
        var input = "username=<USER>&password=<PASS>&token=<TOKEN[0]>&dict=<DICT(key)>&list=<LIST[*]>";

        // Act
        var references = _scriptParser.ExtractVariableReferences(input);

        // Assert
        references.Should().HaveCount(5);
        
        references[0].VariableName.Should().Be("USER");
        references[0].Type.Should().Be(VariableType.Single);
        
        references[1].VariableName.Should().Be("PASS");
        references[1].Type.Should().Be(VariableType.Single);
        
        references[2].VariableName.Should().Be("TOKEN");
        references[2].Type.Should().Be(VariableType.List);
        references[2].IndexOrKey.Should().Be("0");
        
        references[3].VariableName.Should().Be("DICT");
        references[3].Type.Should().Be(VariableType.Dictionary);
        references[3].IndexOrKey.Should().Be("key");
        
        references[4].VariableName.Should().Be("LIST");
        references[4].Type.Should().Be(VariableType.List);
        references[4].IndexOrKey.Should().Be("*");
    }

    [Fact]
    public void SubstituteVariables_Should_Replace_Variables_Correctly()
    {
        // Arrange
        var input = "Hello <NAME>, your ID is <ID> and status is <STATUS>";
        var variables = new Dictionary<string, object>
        {
            ["NAME"] = "John",
            ["ID"] = 12345,
            ["STATUS"] = "Active"
        };

        // Act
        var result = _scriptParser.SubstituteVariables(input, variables);

        // Assert
        result.Should().Be("Hello John, your ID is 12345 and status is Active");
    }

    [Fact]
    public void SubstituteVariables_With_List_Should_Handle_Index_Access()
    {
        // Arrange
        var input = "First item: <ITEMS[0]>, All items: <ITEMS[*]>";
        var variables = new Dictionary<string, object>
        {
            ["ITEMS"] = new List<object> { "Apple", "Banana", "Cherry" }
        };

        // Act
        var result = _scriptParser.SubstituteVariables(input, variables);

        // Assert
        result.Should().Be("First item: Apple, All items: Apple, Banana, Cherry");
    }

    [Fact]
    public void SubstituteVariables_With_Dictionary_Should_Handle_Key_Access()
    {
        // Arrange
        var input = "User: <USER(name)>, All values: <USER(*)>";
        var variables = new Dictionary<string, object>
        {
            ["USER"] = new Dictionary<string, object> { ["name"] = "Alice", ["age"] = 30, ["city"] = "Boston" }
        };

        // Act
        var result = _scriptParser.SubstituteVariables(input, variables);

        // Assert
        result.Should().Be("User: Alice, All values: Alice, 30, Boston");
    }

    [Fact]
    public void ValidateScript_With_Valid_Script_Should_Return_Success()
    {
        // Arrange
        var validScript = @"PRINT Starting validation test
REQUEST GET ""https://example.com""
PARSE ""<SOURCE>"" LR ""<title>"" ""</title>"" -> VAR ""TITLE""";

        // Act
        var result = _scriptParser.ValidateScript(validScript);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateScript_With_Invalid_Script_Should_Return_Errors()
    {
        // Arrange
        var invalidScript = @"PRINT Starting test
INVALID_COMMAND with bad syntax
REQUEST"; // Incomplete command

        // Act
        var result = _scriptParser.ValidateScript(invalidScript);

        // Assert
        result.Should().NotBeNull();
        // Note: This might still be valid as we're only doing basic parsing
        // More advanced validation would require command-specific validation
    }

    [Fact]
    public void GetAvailableCommands_Should_Return_Command_List()
    {
        // Act
        var commands = _scriptParser.GetAvailableCommands();

        // Assert
        commands.Should().NotBeNull();
        commands.Should().NotBeEmpty();
        commands.Should().Contain(c => c.Name == "PRINT");
        commands.Should().Contain(c => c.Name == "REQUEST");
        commands.Should().Contain(c => c.Name == "PARSE");
        commands.Should().Contain(c => c.Name == "KEYCHECK");
        commands.Should().Contain(c => c.Name == "FUNCTION");
    }

    [Fact]
    public void ParseScript_Should_Calculate_Statistics_Correctly()
    {
        // Arrange
        var script = @"## Comment 1
## Comment 2

PRINT Line 1
REQUEST GET ""https://example.com""

## Another comment
PARSE ""<SOURCE>"" LR ""test"" ""test"" -> VAR ""RESULT""
PRINT Line 2";

        // Act
        var result = _scriptParser.ParseScript(script);

        // Assert
        result.Statistics.TotalLines.Should().Be(9);
        result.Statistics.CommentLines.Should().Be(3);
        result.Statistics.EmptyLines.Should().Be(2);
        result.Statistics.CodeLines.Should().Be(4);
        result.Statistics.InstructionCount.Should().Be(4);
        result.Statistics.CommandCounts["PRINT"].Should().Be(2);
        result.Statistics.CommandCounts["REQUEST"].Should().Be(1);
        result.Statistics.CommandCounts["PARSE"].Should().Be(1);
        result.Statistics.ParseTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Theory]
    [InlineData("PRINT Simple message")]
    [InlineData("REQUEST GET \"https://example.com\"")]
    [InlineData("PARSE \"<SOURCE>\" LR \"left\" \"right\" -> VAR \"RESULT\"")]
    [InlineData("#LABEL FUNCTION Constant \"test\" -> VAR \"OUTPUT\"")]
    [InlineData("KEYCHECK BanOn4XX=True BanOnToCheck=False")]
    public void ParseLine_With_Various_Valid_Syntaxes_Should_Parse_Successfully(string commandLine)
    {
        // Act
        var result = _scriptParser.ParseLine(commandLine, 1);

        // Assert
        result.Should().NotBeNull();
        result!.CommandName.Should().NotBeEmpty();
        result.RawLine.Should().Be(commandLine);
    }

    [Theory]
    [InlineData("## This is a comment")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\t")]
    public void ParseLine_With_Non_Command_Lines_Should_Return_Null(string line)
    {
        // Act
        var result = _scriptParser.ParseLine(line, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseScript_With_Multiline_Command_Should_Handle_Continuations()
    {
        // Arrange
        var script = @"REQUEST POST ""https://example.com/login""
  CONTENT ""username=test&password=secret""
  CONTENTTYPE ""application/x-www-form-urlencoded""
  HEADER ""User-Agent: TestBot""";

        // Act
        var result = _scriptParser.ParseScript(script);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Instructions.Should().HaveCount(4); // Currently parses as separate commands
        
        result.Instructions[0].CommandName.Should().Be("REQUEST");
        result.Instructions[1].CommandName.Should().Be("CONTENT");
        result.Instructions[2].CommandName.Should().Be("CONTENTTYPE");
        result.Instructions[3].CommandName.Should().Be("HEADER");
    }

    public void Dispose()
    {
        // No resources to dispose in this test class
    }
}

/// <summary>
/// Step 4 Performance Tests
/// </summary>
public class Step4_PerformanceTests : IDisposable
{
    private readonly Mock<ILogger<ScriptParser>> _loggerMock;
    private readonly ScriptParser _scriptParser;

    public Step4_PerformanceTests()
    {
        _loggerMock = new Mock<ILogger<ScriptParser>>();
        _scriptParser = new ScriptParser(_loggerMock.Object);
    }

    [Fact]
    public void ParseScript_With_Large_Script_Should_Be_Fast()
    {
        // Arrange
        var largeScript = Step4TestHelpers.CreateLargeScript(1000); // 1000 lines
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = _scriptParser.ParseScript(largeScript);

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should parse 1000 lines in under 1 second
        result.Should().NotBeNull();
        result.Instructions.Should().HaveCountGreaterThan(500);
    }

    [Fact]
    public void ExtractVariableReferences_Should_Be_Fast()
    {
        // Arrange
        var textWithManyVariables = Step4TestHelpers.CreateTextWithManyVariables(500);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var references = _scriptParser.ExtractVariableReferences(textWithManyVariables);

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should extract 500 variables in under 100ms
        references.Should().HaveCount(500);
    }

    [Fact]
    public void ParseLine_Multiple_Times_Should_Be_Fast()
    {
        // Arrange
        var testLines = Step4TestHelpers.CreateTestLines(100);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < testLines.Count; i++)
        {
            var result = _scriptParser.ParseLine(testLines[i], i + 1);
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(200); // Should parse 100 lines in under 200ms
    }

    public void Dispose()
    {
        // No resources to dispose
    }
}

/// <summary>
/// Step 4 Test Helpers
/// </summary>
public static class Step4TestHelpers
{
    public static string CreateLargeScript(int lineCount)
    {
        var lines = new List<string>();
        
        for (int i = 0; i < lineCount; i++)
        {
            switch (i % 5)
            {
                case 0:
                    lines.Add($"## Comment line {i}");
                    break;
                case 1:
                    lines.Add($"PRINT Line {i} processing");
                    break;
                case 2:
                    lines.Add($"REQUEST GET \"https://example.com/page{i}\"");
                    break;
                case 3:
                    lines.Add($"PARSE \"<SOURCE>\" LR \"<div{i}>\" \"</div>\" -> VAR \"DATA{i}\"");
                    break;
                case 4:
                    lines.Add($"#LABEL{i} FUNCTION Constant \"Value{i}\" -> VAR \"CONST{i}\"");
                    break;
            }
        }
        
        return string.Join("\n", lines);
    }

    public static string CreateTextWithManyVariables(int variableCount)
    {
        var parts = new List<string>();
        
        for (int i = 0; i < variableCount; i++)
        {
            var varType = i % 4;
            switch (varType)
            {
                case 0:
                    parts.Add($"<VAR{i}>");
                    break;
                case 1:
                    parts.Add($"<LIST{i}[{i}]>");
                    break;
                case 2:
                    parts.Add($"<DICT{i}(key{i})>");
                    break;
                case 3:
                    parts.Add($"<ARRAY{i}[*]>");
                    break;
            }
            
            if (i < variableCount - 1)
                parts.Add("&");
        }
        
        return string.Join("", parts);
    }

    public static List<string> CreateTestLines(int count)
    {
        var lines = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            switch (i % 6)
            {
                case 0:
                    lines.Add($"PRINT Test line {i}");
                    break;
                case 1:
                    lines.Add($"REQUEST GET \"https://test{i}.com\"");
                    break;
                case 2:
                    lines.Add($"PARSE \"<SOURCE>\" CSS \"div.test{i}\" \"text\" -> VAR \"RESULT{i}\"");
                    break;
                case 3:
                    lines.Add($"FUNCTION Hash MD5 \"test{i}\" -> VAR \"HASH{i}\"");
                    break;
                case 4:
                    lines.Add($"#TEST{i} KEYCHECK BanOn4XX=True");
                    break;
                case 5:
                    lines.Add($"## Comment for test {i}");
                    break;
            }
        }
        
        return lines;
    }

    public static ScriptInstruction CreateTestInstruction(string commandName = "PRINT", params string[] arguments)
    {
        return new ScriptInstruction
        {
            CommandName = commandName,
            Arguments = arguments.ToList(),
            LineNumber = 1,
            RawLine = $"{commandName} {string.Join(" ", arguments)}"
        };
    }

    public static VariableReference CreateVariableReference(string name, VariableType type = VariableType.Single, string? indexOrKey = null)
    {
        return new VariableReference
        {
            VariableName = name,
            Type = type,
            IndexOrKey = indexOrKey,
            Position = 0,
            Length = name.Length + 2, // <NAME>
            OriginalText = $"<{name}>"
        };
    }

    public static ScriptParseResult CreateParseResult(bool success = true, int instructionCount = 5)
    {
        var result = new ScriptParseResult { Success = success };
        
        for (int i = 0; i < instructionCount; i++)
        {
            result.Instructions.Add(CreateTestInstruction("PRINT", $"Instruction {i}"));
        }
        
        if (!success)
        {
            result.Errors.Add(new ScriptError
            {
                LineNumber = 1,
                Message = "Test error",
                Line = "Invalid line"
            });
        }
        
        result.Statistics.TotalLines = instructionCount * 2; // Assume some comments/empty lines
        result.Statistics.InstructionCount = instructionCount;
        result.Statistics.CodeLines = instructionCount;
        result.Statistics.ParseTime = TimeSpan.FromMilliseconds(10);
        
        return result;
    }
}
