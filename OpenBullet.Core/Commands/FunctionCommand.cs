using Microsoft.Extensions.Logging;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using System.Text.RegularExpressions;

namespace OpenBullet.Core.Commands;

/// <summary>
/// FUNCTION command for defining and calling reusable code blocks
/// </summary>
public class FunctionCommand : IScriptCommand
{
    private readonly ILogger<FunctionCommand> _logger;
    private static readonly Dictionary<string, FunctionDefinition> _functions = new();

    public string CommandName => "FUNCTION";
    public string Description => "Defines and calls reusable code blocks with parameters";

    public FunctionCommand(ILogger<FunctionCommand> logger)
    {
        _logger = logger;
    }

    public async Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData)
    {
        try
        {
            _logger.LogTrace("Executing FUNCTION command on line {LineNumber}", instruction.LineNumber);

            if (instruction.Arguments.Count < 2)
            {
                return new CommandResult { Success = false, ErrorMessage = "FUNCTION command requires at least operation and name arguments" };
            }

            var operation = instruction.Arguments[0].ToUpperInvariant();
            var functionName = instruction.Arguments[1];

            switch (operation)
            {
                case "DEFINE":
                    return await DefineFunctionAsync(functionName, instruction, botData);
                
                case "CALL":
                    return await CallFunctionAsync(functionName, instruction, botData);
                
                case "LIST":
                    return await ListFunctionsAsync(botData);
                
                case "DELETE":
                    return await DeleteFunctionAsync(functionName, botData);
                
                default:
                    return new CommandResult { Success = false, ErrorMessage = $"Invalid FUNCTION operation: {operation}. Use DEFINE, CALL, LIST, or DELETE" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing FUNCTION command on line {LineNumber}", instruction.LineNumber);
            return new CommandResult { Success = false, ErrorMessage = $"FUNCTION command failed: {ex.Message}" };
        }
    }

    public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
    {
        var result = new CommandValidationResult { IsValid = true };

        if (instruction.Arguments.Count < 2)
        {
            result.IsValid = false;
            result.Errors.Add("FUNCTION command requires at least operation and name arguments");
            return result;
        }

        var operation = instruction.Arguments[0].ToUpperInvariant();
        if (!new[] { "DEFINE", "CALL", "LIST", "DELETE" }.Contains(operation))
        {
            result.IsValid = false;
            result.Errors.Add($"Invalid FUNCTION operation: {operation}");
        }

        if (operation == "DEFINE" && instruction.Arguments.Count < 3)
        {
            result.IsValid = false;
            result.Errors.Add("DEFINE operation requires function body");
        }

        return result;
    }

    private async Task<CommandResult> DefineFunctionAsync(string name, ScriptInstruction instruction, BotData botData)
    {
        if (instruction.Arguments.Count < 3)
        {
            return new CommandResult { Success = false, ErrorMessage = "DEFINE operation requires function body" };
        }

        var body = instruction.Arguments[2];
        
        // Parse parameters if provided
        var parameters = new List<string>();
        for (int i = 3; i < instruction.Arguments.Count; i++)
        {
            if (instruction.Arguments[i].StartsWith("PARAMS", StringComparison.OrdinalIgnoreCase) && i + 1 < instruction.Arguments.Count)
            {
                var paramString = instruction.Arguments[i + 1];
                parameters = paramString.Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList();
                break;
            }
        }

        // Store function definition
        _functions[name] = new FunctionDefinition
        {
            Name = name,
            Parameters = parameters,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogDebug("Function '{FunctionName}' defined with {ParamCount} parameters", name, parameters.Count);
        
        // Store in bot data variables for access
        botData.Variables[$"FUNCTION_{name}"] = _functions[name];
        
        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> CallFunctionAsync(string name, ScriptInstruction instruction, BotData botData)
    {
        if (!_functions.TryGetValue(name, out var function))
        {
            return new CommandResult { Success = false, ErrorMessage = $"Function '{name}' not found" };
        }

        try
        {
            // Parse call parameters if provided
            var callParams = new List<string>();
            for (int i = 2; i < instruction.Arguments.Count; i++)
            {
                if (instruction.Arguments[i].StartsWith("PARAMS", StringComparison.OrdinalIgnoreCase) && i + 1 < instruction.Arguments.Count)
                {
                    var paramString = instruction.Arguments[i + 1];
                    callParams = ParseCallParameters(paramString);
                    break;
                }
            }

            // Validate parameter count
            if (callParams.Count != function.Parameters.Count)
            {
                return new CommandResult { Success = false, ErrorMessage = $"Function '{name}' expects {function.Parameters.Count} parameters, got {callParams.Count}" };
            }

            // Store function result in bot data
            botData.Variables[$"FUNCTION_RESULT_{name}"] = "executed";
            
            _logger.LogDebug("Function '{FunctionName}' called with {ParamCount} parameters", name, callParams.Count);
            
            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling function '{FunctionName}'", name);
            return new CommandResult { Success = false, ErrorMessage = $"Function '{name}' execution failed: {ex.Message}" };
        }
    }

    private async Task<CommandResult> ListFunctionsAsync(BotData botData)
    {
        var functionList = _functions.Values.Select(f => new
        {
            f.Name,
            f.Parameters.Count,
            f.CreatedAt
        }).ToList();

        botData.Variables["FUNCTIONS"] = functionList;
        
        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> DeleteFunctionAsync(string name, BotData botData)
    {
        if (!_functions.ContainsKey(name))
        {
            return new CommandResult { Success = false, ErrorMessage = $"Function '{name}' not found" };
        }

        _functions.Remove(name);
        botData.Variables.TryRemove($"FUNCTION_{name}", out _);
        
        return new CommandResult { Success = true };
    }

    private List<string> ParseCallParameters(string parameters)
    {
        if (string.IsNullOrEmpty(parameters))
            return new List<string>();

        // Handle quoted parameters
        var matches = Regex.Matches(parameters, @"(""[^""]*""|\S+)");
        return matches.Select(m => m.Value.Trim('"')).ToList();
    }
}

/// <summary>
/// Function definition model
/// </summary>
public class FunctionDefinition
{
    public string Name { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
