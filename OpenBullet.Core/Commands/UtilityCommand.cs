using Microsoft.Extensions.Logging;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Models;
using System.Text.RegularExpressions;

namespace OpenBullet.Core.Commands;

/// <summary>
/// UTILITY command for common operations like string manipulation, math, lists, etc.
/// </summary>
public class UtilityCommand : IScriptCommand
{
    private readonly ILogger<UtilityCommand> _logger;
    private readonly Random _random = new();

    public string CommandName => "UTILITY";
    public string Description => "Provides utility operations for strings, lists, math, dates, and encoding";

    public UtilityCommand(ILogger<UtilityCommand> logger)
    {
        _logger = logger;
    }

    public async Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData)
    {
        try
        {
            _logger.LogTrace("Executing UTILITY command on line {LineNumber}", instruction.LineNumber);

            if (instruction.Arguments.Count < 1)
            {
                return new CommandResult { Success = false, ErrorMessage = "UTILITY command requires operation argument" };
            }

            var operation = instruction.Arguments[0].ToUpperInvariant();
            var target = instruction.GetArgument(1, "");
            var value = instruction.GetArgument(2, "");
            var result = instruction.GetArgument(3, "");

            switch (operation)
            {
                case "REPLACE":
                    return await StringReplaceAsync(target, value, result, botData);
                
                case "SUBSTRING":
                    return await StringSubstringAsync(target, value, result, botData);
                
                case "SPLIT":
                    return await StringSplitAsync(target, value, result, botData);
                
                case "JOIN":
                    return await StringJoinAsync(target, value, result, botData);
                
                case "UPPER":
                    return await StringUpperAsync(target, result, botData);
                
                case "LOWER":
                    return await StringLowerAsync(target, result, botData);
                
                case "TRIM":
                    return await StringTrimAsync(target, result, botData);
                
                case "LENGTH":
                    return await StringLengthAsync(target, result, botData);
                
                case "ADD":
                    return await ListAddAsync(target, value, result, botData);
                
                case "REMOVE":
                    return await ListRemoveAsync(target, value, result, botData);
                
                case "SORT":
                    return await ListSortAsync(target, result, botData);
                
                case "SHUFFLE":
                    return await ListShuffleAsync(target, result, botData);
                
                case "CALCULATE":
                    return await MathCalculateAsync(target, result, botData);
                
                case "RANDOM":
                    return await MathRandomAsync(target, value, result, botData);
                
                case "ROUND":
                    return await MathRoundAsync(target, value, result, botData);
                
                case "NOW":
                    return await DateTimeNowAsync(result, botData);
                
                case "FORMAT":
                    return await DateTimeFormatAsync(target, value, result, botData);
                
                case "BASE64":
                    return await EncodingBase64Async(target, value, result, botData);
                
                case "URL":
                    return await EncodingUrlAsync(target, value, result, botData);
                
                case "HASH":
                    return await EncodingHashAsync(target, value, result, botData);
                
                default:
                    return new CommandResult { Success = false, ErrorMessage = $"Invalid UTILITY operation: {operation}" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing UTILITY command on line {LineNumber}", instruction.LineNumber);
            return new CommandResult { Success = false, ErrorMessage = $"UTILITY command failed: {ex.Message}" };
        }
    }

    public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
    {
        var result = new CommandValidationResult { IsValid = true };

        if (instruction.Arguments.Count < 1)
        {
            result.IsValid = false;
            result.Errors.Add("UTILITY command requires operation argument");
            return result;
        }

        var operation = instruction.Arguments[0].ToUpperInvariant();
        var validOperations = new[] { "REPLACE", "SUBSTRING", "SPLIT", "JOIN", "UPPER", "LOWER", "TRIM", "LENGTH", 
                                     "ADD", "REMOVE", "SORT", "SHUFFLE", "CALCULATE", "RANDOM", "ROUND", 
                                     "NOW", "FORMAT", "BASE64", "URL", "HASH" };

        if (!validOperations.Contains(operation))
        {
            result.IsValid = false;
            result.Errors.Add($"Invalid UTILITY operation: {operation}");
        }

        return result;
    }

    #region String Operations

    private async Task<CommandResult> StringReplaceAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for REPLACE operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var searchValue = botData.GetVariable<string>(value) ?? "";
        var replaceValue = botData.GetVariable<string>(result) ?? "";

        var replaced = targetValue.Replace(searchValue, replaceValue);
        botData.SetVariable(result, replaced);

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> StringSubstringAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for SUBSTRING operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var indexStr = botData.GetVariable<string>(value) ?? "";

        if (!int.TryParse(indexStr, out var index) || index < 0 || index >= targetValue.Length)
            return new CommandResult { Success = false, ErrorMessage = $"Invalid index for SUBSTRING: {indexStr}" };

        var substring = targetValue.Substring(index);
        botData.SetVariable(result, substring);

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> StringSplitAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for SPLIT operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var delimiter = botData.GetVariable<string>(value) ?? "";

        var parts = targetValue.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        botData.SetVariable(result, parts.ToList());

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> StringJoinAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for JOIN operation" };

        var targetValue = botData.GetVariable<object>(target);
        var delimiter = botData.GetVariable<string>(value) ?? "";

        if (targetValue is List<string> list)
        {
            var joined = string.Join(delimiter, list);
            botData.SetVariable(result, joined);
        }
        else
        {
            return new CommandResult { Success = false, ErrorMessage = "Target must be a list for JOIN operation" };
        }

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> StringUpperAsync(string target, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for UPPER operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var upper = targetValue.ToUpperInvariant();
        botData.SetVariable(result, upper);

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> StringLowerAsync(string target, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for LOWER operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var lower = targetValue.ToLowerInvariant();
        botData.SetVariable(result, lower);

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> StringTrimAsync(string target, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for TRIM operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var trimmed = targetValue.Trim();
        botData.SetVariable(result, trimmed);

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> StringLengthAsync(string target, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for LENGTH operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var length = targetValue.Length;
        botData.SetVariable(result, length);

        return new CommandResult { Success = true };
    }

    #endregion

    #region List Operations

    private async Task<CommandResult> ListAddAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for ADD operation" };

        var targetValue = botData.GetVariable<object>(target);
        var addValue = botData.GetVariable<object>(value);

        if (targetValue is List<string> list)
        {
            list.Add(addValue?.ToString() ?? "");
            botData.SetVariable(result, list);
        }
        else
        {
            return new CommandResult { Success = false, ErrorMessage = "Target must be a list for ADD operation" };
        }

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> ListRemoveAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for REMOVE operation" };

        var targetValue = botData.GetVariable<object>(target);
        var removeValue = botData.GetVariable<object>(value);

        if (targetValue is List<string> list)
        {
            var removed = list.Remove(removeValue?.ToString() ?? "");
            botData.SetVariable(result, list);
            
            return new CommandResult { Success = true };
        }
        else
        {
            return new CommandResult { Success = false, ErrorMessage = "Target must be a list for REMOVE operation" };
        }
    }

    private async Task<CommandResult> ListSortAsync(string target, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for SORT operation" };

        var targetValue = botData.GetVariable<object>(target);

        if (targetValue is List<string> list)
        {
            var sorted = list.OrderBy(x => x).ToList();
            botData.SetVariable(result, sorted);
        }
        else
        {
            return new CommandResult { Success = false, ErrorMessage = "Target must be a list for SORT operation" };
        }

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> ListShuffleAsync(string target, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for SHUFFLE operation" };

        var targetValue = botData.GetVariable<object>(target);

        if (targetValue is List<string> list)
        {
            var shuffled = list.OrderBy(x => _random.Next()).ToList();
            botData.SetVariable(result, shuffled);
        }
        else
        {
            return new CommandResult { Success = false, ErrorMessage = "Target must be a list for SHUFFLE operation" };
        }

        return new CommandResult { Success = true };
    }

    #endregion

    #region Math Operations

    private async Task<CommandResult> MathCalculateAsync(string target, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for CALCULATE operation" };

        var expression = botData.GetVariable<string>(target) ?? "";
        
        try
        {
            var calculated = EvaluateExpression(expression);
            botData.SetVariable(result, calculated);
            
            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"Failed to calculate expression: {ex.Message}" };
        }
    }

    private async Task<CommandResult> MathRandomAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for RANDOM operation" };

        var minStr = botData.GetVariable<string>(target) ?? "";
        var maxStr = botData.GetVariable<string>(value) ?? "";

        if (!int.TryParse(minStr, out var min) || !int.TryParse(maxStr, out var max))
            return new CommandResult { Success = false, ErrorMessage = "Target and value must be valid integers for RANDOM operation" };

        var random = _random.Next(min, max + 1);
        botData.SetVariable(result, random);

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> MathRoundAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for ROUND operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var decimals = botData.GetVariable<string>(value) ?? "0";

        if (!double.TryParse(targetValue, out var number) || !int.TryParse(decimals, out var decimalPlaces))
            return new CommandResult { Success = false, ErrorMessage = "Invalid values for ROUND operation" };

        var rounded = Math.Round(number, decimalPlaces);
        botData.SetVariable(result, rounded);

        return new CommandResult { Success = true };
    }

    #endregion

    #region DateTime Operations

    private async Task<CommandResult> DateTimeNowAsync(string result, BotData botData)
    {
        var now = DateTime.UtcNow;
        botData.SetVariable(result, now);

        return new CommandResult { Success = true };
    }

    private async Task<CommandResult> DateTimeFormatAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(value))
            return new CommandResult { Success = false, ErrorMessage = "Target and value are required for FORMAT operation" };

        var targetValue = botData.GetVariable<object>(target);
        var format = botData.GetVariable<string>(value) ?? "";

        if (targetValue is DateTime dateTime)
        {
            var formatted = dateTime.ToString(format);
            botData.SetVariable(result, formatted);
        }
        else
        {
            return new CommandResult { Success = false, ErrorMessage = "Target must be a DateTime for FORMAT operation" };
        }

        return new CommandResult { Success = true };
    }

    #endregion

    #region Encoding Operations

    private async Task<CommandResult> EncodingBase64Async(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for BASE64 operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var operation = botData.GetVariable<string>(value) ?? "encode";

        try
        {
            string encoded;
            if (operation.ToLowerInvariant() == "decode")
            {
                var bytes = Convert.FromBase64String(targetValue);
                encoded = System.Text.Encoding.UTF8.GetString(bytes);
            }
            else
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(targetValue);
                encoded = Convert.ToBase64String(bytes);
            }

            botData.SetVariable(result, encoded);
            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"Base64 {operation} failed: {ex.Message}" };
        }
    }

    private async Task<CommandResult> EncodingUrlAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for URL operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var operation = botData.GetVariable<string>(value) ?? "encode";

        try
        {
            string encoded;
            if (operation.ToLowerInvariant() == "decode")
            {
                encoded = Uri.UnescapeDataString(targetValue);
            }
            else
            {
                encoded = Uri.EscapeDataString(targetValue);
            }

            botData.SetVariable(result, encoded);
            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"URL {operation} failed: {ex.Message}" };
        }
    }

    private async Task<CommandResult> EncodingHashAsync(string target, string value, string result, BotData botData)
    {
        if (string.IsNullOrEmpty(target))
            return new CommandResult { Success = false, ErrorMessage = "Target is required for HASH operation" };

        var targetValue = botData.GetVariable<string>(target) ?? "";
        var algorithm = botData.GetVariable<string>(value) ?? "MD5";

        try
        {
            var hash = ComputeHash(targetValue, algorithm);
            botData.SetVariable(result, hash);
            
            return new CommandResult { Success = true };
        }
        catch (Exception ex)
        {
            return new CommandResult { Success = false, ErrorMessage = $"Hash computation failed: {ex.Message}" };
        }
    }

    #endregion

    #region Helper Methods

    private double EvaluateExpression(string expression)
    {
        // Simple expression evaluator - in production, use a proper math parser
        expression = expression.Replace(" ", "");
        
        // Handle basic operations
        if (expression.Contains("+"))
        {
            var parts = expression.Split('+');
            if (parts.Length == 2 && double.TryParse(parts[0], out var a) && double.TryParse(parts[1], out var b))
                return a + b;
        }
        else if (expression.Contains("-"))
        {
            var parts = expression.Split('-');
            if (parts.Length == 2 && double.TryParse(parts[0], out var a) && double.TryParse(parts[1], out var b))
                return a - b;
        }
        else if (expression.Contains("*"))
        {
            var parts = expression.Split('*');
            if (parts.Length == 2 && double.TryParse(parts[0], out var a) && double.TryParse(parts[1], out var b))
                return a * b;
        }
        else if (expression.Contains("/"))
        {
            var parts = expression.Split('/');
            if (parts.Length == 2 && double.TryParse(parts[0], out var a) && double.TryParse(parts[1], out var b))
                return a / b;
        }

        // Single number
        if (double.TryParse(expression, out var number))
            return number;

        throw new ArgumentException($"Cannot evaluate expression: {expression}");
    }

    private string ComputeHash(string input, string algorithm)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        
        return algorithm.ToUpperInvariant() switch
        {
            "MD5" => Convert.ToHexString(System.Security.Cryptography.MD5.HashData(bytes)).ToLowerInvariant(),
            "SHA1" => Convert.ToHexString(System.Security.Cryptography.SHA1.HashData(bytes)).ToLowerInvariant(),
            "SHA256" => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)).ToLowerInvariant(),
            _ => throw new ArgumentException($"Unsupported hash algorithm: {algorithm}")
        };
    }

    #endregion
}
