using Microsoft.Extensions.Logging;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using System.Text.RegularExpressions;

namespace OpenBullet.Core.Commands;

/// <summary>
/// Flow control commands for IF/ELSE, WHILE, FOR, and other control structures
/// </summary>
public class FlowControlCommand : IScriptCommand
{
    private readonly ILogger<FlowControlCommand> _logger;

    public string CommandName => "FLOWCONTROL";
    public string Description => "Provides flow control structures like IF/ELSE, WHILE, FOR, and exception handling";

    public FlowControlCommand(ILogger<FlowControlCommand> logger)
    {
        _logger = logger;
    }

    public async Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData)
    {
        try
        {
            _logger.LogTrace("Executing FLOWCONTROL command on line {LineNumber}", instruction.LineNumber);

            if (instruction.Arguments.Count < 1)
            {
                return new CommandResult { Success = false, ErrorMessage = "FLOWCONTROL command requires operation argument" };
            }

            var operation = instruction.Arguments[0].ToUpperInvariant();
            var condition = instruction.GetArgument(1, "");
            var body = instruction.GetArgument(2, "");
            var elseBody = instruction.GetArgument(3, "");

            switch (operation)
            {
                case "IF":
                    return await ExecuteIfAsync(condition, body, elseBody, botData);
                
                case "WHILE":
                    return await ExecuteWhileAsync(condition, body, botData);
                
                case "FOR":
                    return await ExecuteForAsync(condition, body, botData);
                
                case "BREAK":
                    return await ExecuteBreakAsync(botData);
                
                case "CONTINUE":
                    return await ExecuteContinueAsync(botData);
                
                case "RETURN":
                    return await ExecuteReturnAsync(body, botData);
                
                case "TRY":
                    return await ExecuteTryAsync(body, elseBody, botData);
                
                default:
                    return new CommandResult { Success = false, ErrorMessage = $"Invalid flow control operation: {operation}. Use IF, WHILE, FOR, BREAK, CONTINUE, RETURN, or TRY" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing flow control command on line {LineNumber}", instruction.LineNumber);
            return new CommandResult { Success = false, ErrorMessage = $"Flow control command failed: {ex.Message}" };
        }
    }

    public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
    {
        var result = new CommandValidationResult { IsValid = true };

        if (instruction.Arguments.Count < 1)
        {
            result.IsValid = false;
            result.Errors.Add("FLOWCONTROL command requires operation argument");
            return result;
        }

        var operation = instruction.Arguments[0].ToUpperInvariant();
        var validOperations = new[] { "IF", "WHILE", "FOR", "BREAK", "CONTINUE", "RETURN", "TRY" };

        if (!validOperations.Contains(operation))
        {
            result.IsValid = false;
            result.Errors.Add($"Invalid flow control operation: {operation}");
        }

        if ((operation == "IF" || operation == "WHILE" || operation == "FOR") && instruction.Arguments.Count < 3)
        {
            result.IsValid = false;
            result.Errors.Add($"{operation} operation requires condition and body arguments");
        }

        return result;
    }

    #region IF/ELSE Logic

    private async Task<CommandResult> ExecuteIfAsync(string condition, string body, string elseBody, BotData botData)
    {
        if (string.IsNullOrEmpty(condition))
            return new CommandResult { Success = false, ErrorMessage = "Condition is required for IF command" };

        try
        {
            var conditionResult = EvaluateCondition(condition, botData);
            
            if (conditionResult)
            {
                if (!string.IsNullOrEmpty(body))
                {
                    return await ExecuteBodyAsync(body, botData);
                }
                return new CommandResult { Success = true };
            }
            else
            {
                if (!string.IsNullOrEmpty(elseBody))
                {
                    return await ExecuteBodyAsync(elseBody, botData);
                }
                return new CommandResult { Success = true };
            }
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"IF condition evaluation failed: {ex.Message}" };
        }
    }

    private bool EvaluateCondition(string condition, BotData botData)
    {
        // Simple condition evaluator - supports basic comparisons
        condition = condition.Trim();
        
        // Handle boolean variables
        if (botData.GetVariable<bool>(condition) is bool boolResult)
            return boolResult;
        
        if (botData.GetVariable<string>(condition) is string stringResult)
            return !string.IsNullOrEmpty(stringResult) && stringResult.ToLowerInvariant() != "false";

        // Handle comparison operators
        if (condition.Contains("=="))
        {
            var parts = condition.Split("==", 2);
            if (parts.Length == 2)
            {
                var left = GetVariableValue(parts[0].Trim(), botData);
                var right = GetVariableValue(parts[1].Trim(), botData);
                return left?.ToString() == right?.ToString();
            }
        }
        else if (condition.Contains("!="))
        {
            var parts = condition.Split("!=", 2);
            if (parts.Length == 2)
            {
                var left = GetVariableValue(parts[0].Trim(), botData);
                var right = GetVariableValue(parts[1].Trim(), botData);
                return left?.ToString() != right?.ToString();
            }
        }
        else if (condition.Contains(">"))
        {
            var parts = condition.Split(">", 2);
            if (parts.Length == 2)
            {
                var left = GetVariableValue(parts[0].Trim(), botData);
                var right = GetVariableValue(parts[1].Trim(), botData);
                if (double.TryParse(left?.ToString(), out var leftNum) && double.TryParse(right?.ToString(), out var rightNum))
                    return leftNum > rightNum;
            }
        }
        else if (condition.Contains("<"))
        {
            var parts = condition.Split("<", 2);
            if (parts.Length == 2)
            {
                var left = GetVariableValue(parts[0].Trim(), botData);
                var right = GetVariableValue(parts[1].Trim(), botData);
                if (double.TryParse(left?.ToString(), out var leftNum) && double.TryParse(right?.ToString(), out var rightNum))
                    return leftNum < rightNum;
            }
        }
        else if (condition.Contains(">="))
        {
            var parts = condition.Split(">=", 2);
            if (parts.Length == 2)
            {
                var left = GetVariableValue(parts[0].Trim(), botData);
                var right = GetVariableValue(parts[1].Trim(), botData);
                if (double.TryParse(left?.ToString(), out var leftNum) && double.TryParse(right?.ToString(), out var rightNum))
                    return leftNum >= rightNum;
            }
        }
        else if (condition.Contains("<="))
        {
            var parts = condition.Split("<=", 2);
            if (parts.Length == 2)
            {
                var left = GetVariableValue(parts[0].Trim(), botData);
                var right = GetVariableValue(parts[1].Trim(), botData);
                if (double.TryParse(left?.ToString(), out var leftNum) && double.TryParse(right?.ToString(), out var rightNum))
                    return leftNum <= rightNum;
            }
        }
        else if (condition.Contains("CONTAINS"))
        {
            var match = Regex.Match(condition, @"(\w+)\s+CONTAINS\s+""([^""]+)""");
            if (match.Success)
            {
                var variable = match.Groups[1].Value;
                var searchValue = match.Groups[2].Value;
                var variableValue = GetVariableValue(variable, botData)?.ToString() ?? "";
                return variableValue.Contains(searchValue);
            }
        }

        // Default: treat as boolean expression
        var finalValue = GetVariableValue(condition, botData);
        return finalValue != null && finalValue.ToString()?.ToLowerInvariant() != "false";
    }

    private object? GetVariableValue(string variableName, BotData botData)
    {
        // Remove common prefixes and clean up
        variableName = variableName.Trim();
        
        if (botData.GetVariable<object>(variableName) is object value)
            return value;
        
        // Try with common prefixes
        if (variableName.StartsWith("$"))
        {
            var cleanName = variableName.Substring(1);
            if (botData.GetVariable<object>(cleanName) is object prefixedValue)
                return prefixedValue;
        }
        
        return null;
    }

    #endregion

    #region WHILE Loop

    private async Task<CommandResult> ExecuteWhileAsync(string condition, string body, BotData botData)
    {
        if (string.IsNullOrEmpty(condition))
            return new CommandResult { Success = false, ErrorMessage = "Condition is required for WHILE command" };

        if (string.IsNullOrEmpty(body))
            return new CommandResult { Success = false, ErrorMessage = "Body is required for WHILE command" };

        var maxIterations = 1000; // Prevent infinite loops
        var iteration = 0;

        try
        {
            while (iteration < maxIterations)
            {
                // Check cancellation
                if (botData.CancellationToken.IsCancellationRequested)
                    break;

                // Evaluate condition
                if (!EvaluateCondition(condition, botData))
                    break;

                // Execute body
                var bodyResult = await ExecuteBodyAsync(body, botData);
                if (!bodyResult.Success)
                    return bodyResult;

                // Check for break/continue
                if (botData.GetVariable<bool>("__BREAK__") == true)
                {
                    botData.Variables.TryRemove("__BREAK__", out _);
                    break;
                }

                if (botData.GetVariable<bool>("__CONTINUE__") == true)
                {
                    botData.Variables.TryRemove("__CONTINUE__", out _);
                    iteration++;
                    continue;
                }

                iteration++;
            }

            if (iteration >= maxIterations)
            {
                _logger.LogWarning("WHILE loop exceeded maximum iterations ({MaxIterations})", maxIterations);
                return new CommandResult { Success = false, ErrorMessage = $"WHILE loop exceeded maximum iterations ({maxIterations})" };
            }

            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"WHILE loop execution failed: {ex.Message}" };
        }
    }

    #endregion

    #region FOR Loop

    private async Task<CommandResult> ExecuteForAsync(string condition, string body, BotData botData)
    {
        if (string.IsNullOrEmpty(condition))
            return new CommandResult { Success = false, ErrorMessage = "Condition is required for FOR command" };

        if (string.IsNullOrEmpty(body))
            return new CommandResult { Success = false, ErrorMessage = "Body is required for FOR command" };

        try
        {
            // Parse FOR condition: "variable,start,end" or "variable in collection"
            var parts = condition.Split(',');
            
            if (parts.Length == 3)
            {
                // Numeric FOR loop: FOR "i,1,10"
                return await ExecuteNumericForAsync(parts[0].Trim(), parts[1].Trim(), parts[2].Trim(), body, botData);
            }
            else if (condition.Contains(" in "))
            {
                // Collection FOR loop: FOR "item in list"
                var inParts = condition.Split(" in ", 2);
                if (inParts.Length == 2)
                {
                    return await ExecuteCollectionForAsync(inParts[0].Trim(), inParts[1].Trim(), body, botData);
                }
            }

            return new CommandResult { Success = false, ErrorMessage = $"Invalid FOR condition format: {condition}. Use 'variable,start,end' or 'variable in collection'" };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"FOR loop execution failed: {ex.Message}" };
        }
    }

    private async Task<CommandResult> ExecuteNumericForAsync(string variable, string start, string end, string body, BotData botData)
    {
        var startValue = GetVariableValue(start, botData)?.ToString() ?? start;
        var endValue = GetVariableValue(end, botData)?.ToString() ?? end;

        if (!int.TryParse(startValue, out var startNum) || !int.TryParse(endValue, out var endNum))
            return new CommandResult { Success = false, ErrorMessage = $"Invalid numeric values for FOR loop: start={startValue}, end={endValue}" };

        var maxIterations = Math.Abs(endNum - startNum) + 1;
        if (maxIterations > 1000)
            return new CommandResult { Success = false, ErrorMessage = $"FOR loop range too large: {maxIterations} iterations (max 1000)" };

        var iteration = 0;
        try
        {
            for (int i = startNum; startNum <= endNum ? i <= endNum : i >= endNum; i += startNum <= endNum ? 1 : -1)
            {
                // Check cancellation
                if (botData.CancellationToken.IsCancellationRequested)
                    break;

                // Set loop variable
                botData.SetVariable(variable, i);

                // Execute body
                var bodyResult = await ExecuteBodyAsync(body, botData);
                if (!bodyResult.Success)
                    return bodyResult;

                // Check for break/continue
                if (botData.GetVariable<bool>("__BREAK__") == true)
                {
                    botData.Variables.TryRemove("__BREAK__", out _);
                    break;
                }

                if (botData.GetVariable<bool>("__CONTINUE__") == true)
                {
                    botData.Variables.TryRemove("__CONTINUE__", out _);
                    iteration++;
                    continue;
                }

                iteration++;
            }

            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"FOR loop execution failed: {ex.Message}" };
        }
    }

    private async Task<CommandResult> ExecuteCollectionForAsync(string variable, string collection, string body, BotData botData)
    {
        var collectionValue = GetVariableValue(collection, botData);
        
        if (collectionValue is not List<string> list)
            return new CommandResult { Success = false, ErrorMessage = $"Collection '{collection}' is not a list" };

        var iteration = 0;
        try
        {
            foreach (var item in list)
            {
                // Check cancellation
                if (botData.CancellationToken.IsCancellationRequested)
                    break;

                // Set loop variable
                botData.SetVariable(variable, item);

                // Execute body
                var bodyResult = await ExecuteBodyAsync(body, botData);
                if (!bodyResult.Success)
                    return bodyResult;

                // Check for break/continue
                if (botData.GetVariable<bool>("__BREAK__") == true)
                {
                    botData.Variables.TryRemove("__BREAK__", out _);
                    break;
                }

                if (botData.GetVariable<bool>("__CONTINUE__") == true)
                {
                    botData.Variables.TryRemove("__CONTINUE__", out _);
                    iteration++;
                    continue;
                }

                iteration++;
            }

            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"FOR loop execution failed: {ex.Message}" };
        }
    }

    #endregion

    #region Control Commands

    private async Task<CommandResult> ExecuteBreakAsync(BotData botData)
    {
        botData.SetVariable("__BREAK__", true);
        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> ExecuteContinueAsync(BotData botData)
    {
        botData.SetVariable("__CONTINUE__", true);
        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> ExecuteReturnAsync(string body, BotData botData)
    {
        if (!string.IsNullOrEmpty(body))
        {
            var returnValue = GetVariableValue(body, botData);
            botData.SetVariable("__RETURN_VALUE__", returnValue);
        }
        
        botData.SetVariable("__RETURN__", true);
        return new CommandResult { Success = true };
    }

    #endregion

    #region Exception Handling

    private async Task<CommandResult> ExecuteTryAsync(string body, string catchBody, BotData botData)
    {
        if (string.IsNullOrEmpty(body))
            return new CommandResult { Success = false, ErrorMessage = "Body is required for TRY command" };

        try
        {
            var bodyResult = await ExecuteBodyAsync(body, botData);
            return bodyResult;
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(catchBody))
            {
                // Store exception info in variables
                botData.SetVariable("__EXCEPTION_MESSAGE__", ex.Message);
                botData.SetVariable("__EXCEPTION_TYPE__", ex.GetType().Name);
                
                // Execute catch body
                var catchResult = await ExecuteBodyAsync(catchBody, botData);
                return catchResult;
            }
            
            return new CommandResult { Success = false, ErrorMessage = $"TRY block failed: {ex.Message}" };
        }
    }

    #endregion

    #region Helper Methods

    private async Task<CommandResult> ExecuteBodyAsync(string body, BotData botData)
    {
        // TODO: Implement actual body execution
        // This would involve parsing the body and executing it line by line
        // For now, we'll just log the execution
        
        _logger.LogDebug("Executing flow control body: {Body}", body);
        
        // Store execution result
        botData.SetVariable("__LAST_EXECUTION_RESULT__", "executed");
        
        return new CommandResult { Success = true };
    }

    #endregion
}
