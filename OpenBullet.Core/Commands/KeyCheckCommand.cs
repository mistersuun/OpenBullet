using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.KeyChecking;
using OpenBullet.Core.Models;
using OpenBullet.Core.Parsing;

namespace OpenBullet.Core.Commands;

/// <summary>
/// KEYCHECK command implementation for response analysis and status determination
/// </summary>
public class KeyCheckCommand : IScriptCommand
{
    private readonly ILogger<KeyCheckCommand> _logger;
    private readonly IKeyChecker _keyChecker;
    private readonly IScriptParser _scriptParser;

    public string CommandName => "KEYCHECK";
    public string Description => "Analyzes response data using key chains to determine bot status (Success, Failure, Ban, Retry, Custom)";

    public KeyCheckCommand(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILogger<KeyCheckCommand>>() 
                 ?? throw new ArgumentException("ILogger<KeyCheckCommand> not found in service provider");
        _keyChecker = serviceProvider.GetService<IKeyChecker>() 
                    ?? throw new ArgumentException("IKeyChecker not found in service provider");
        _scriptParser = serviceProvider.GetService<IScriptParser>() 
                      ?? throw new ArgumentException("IScriptParser not found in service provider");
    }

    public async Task<CommandResult> ExecuteAsync(ScriptInstruction instruction, BotData botData)
    {
        ArgumentNullException.ThrowIfNull(instruction);
        ArgumentNullException.ThrowIfNull(botData);

        try
        {
            _logger.LogTrace("Executing KEYCHECK command on line {LineNumber}", instruction.LineNumber);

            // Parse key chains from sub-instructions
            var keyChains = ParseKeyChains(instruction.SubInstructions, botData);

            if (keyChains.Count == 0)
            {
                _logger.LogWarning("KEYCHECK command has no key chains defined");
                return new CommandResult 
                { 
                    Success = false, 
                    ErrorMessage = "No key chains defined in KEYCHECK command" 
                };
            }

            // Evaluate key chains
            var keyCheckResult = _keyChecker.EvaluateKeyChains(keyChains, botData);

            // Apply result to bot data
            keyCheckResult.ApplyToBotData(botData);

            // Create command result
            var commandResult = new CommandResult
            {
                Success = keyCheckResult.Success || keyCheckResult.Status != BotStatus.Error,
                ErrorMessage = keyCheckResult.ErrorMessage,
                NewStatus = keyCheckResult.Status,
                CustomStatus = keyCheckResult.CustomStatus
            };

            // Add metadata to command result
            foreach (var metadata in keyCheckResult.Metadata)
            {
                commandResult.CapturedData[$"KeyCheck_{metadata.Key}"] = metadata.Value;
            }

            // Handle special flow control cases
            if (keyCheckResult.ShouldBanProxy)
            {
                commandResult.CapturedData["BanProxy"] = true;
                botData.AddLogEntry("KEYCHECK: Proxy should be banned");
            }

            if (keyCheckResult.ShouldRetry)
            {
                commandResult.CapturedData["ShouldRetry"] = true;
                botData.AddLogEntry("KEYCHECK: Bot should retry");
            }

            _logger.LogTrace("KEYCHECK command completed with status {Status}", keyCheckResult.Status);
            return commandResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "KEYCHECK command failed on line {LineNumber}", instruction.LineNumber);
            botData.Status = BotStatus.Error;
            botData.AddLogEntry($"KEYCHECK failed: {ex.Message}");
            return new CommandResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public CommandValidationResult ValidateInstruction(ScriptInstruction instruction)
    {
        var result = new CommandValidationResult { IsValid = true };

        try
        {
            // Check if there are sub-instructions (key chains)
            if (instruction.SubInstructions.Count == 0)
            {
                result.IsValid = false;
                result.Errors.Add("KEYCHECK command must contain KEYCHAIN blocks");
                return result;
            }

            // Validate each key chain
            foreach (var subInstruction in instruction.SubInstructions)
            {
                ValidateKeyChain(subInstruction, result);
            }

            // Check for at least one success or failure chain
            var hasSuccessChain = instruction.SubInstructions.Any(si => 
                si.CommandName.Equals("KEYCHAIN", StringComparison.OrdinalIgnoreCase) &&
                si.Arguments.Count > 0 &&
                si.Arguments[0].Equals("Success", StringComparison.OrdinalIgnoreCase));

            var hasFailureChain = instruction.SubInstructions.Any(si => 
                si.CommandName.Equals("KEYCHAIN", StringComparison.OrdinalIgnoreCase) &&
                si.Arguments.Count > 0 &&
                si.Arguments[0].Equals("Failure", StringComparison.OrdinalIgnoreCase));

            if (!hasSuccessChain && !hasFailureChain)
            {
                result.Warnings.Add("KEYCHECK should have at least one Success or Failure key chain");
            }

            return result;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Validation error: {ex.Message}");
            return result;
        }
    }

    private void ValidateKeyChain(ScriptInstruction keyChainInstruction, CommandValidationResult result)
    {
        if (!keyChainInstruction.CommandName.Equals("KEYCHAIN", StringComparison.OrdinalIgnoreCase))
        {
            result.Warnings.Add($"Unknown sub-command in KEYCHECK: {keyChainInstruction.CommandName}");
            return;
        }

        // Validate KEYCHAIN arguments: STATUS [LOGIC]
        if (keyChainInstruction.Arguments.Count == 0)
        {
            result.Errors.Add("KEYCHAIN must specify a status (Success, Failure, Ban, Retry, Custom)");
            result.IsValid = false;
            return;
        }

        var status = keyChainInstruction.Arguments[0];
        var validStatuses = new[] { "Success", "Failure", "Ban", "Retry", "Custom" };
        if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            result.Errors.Add($"Invalid KEYCHAIN status: {status}. Valid statuses: {string.Join(", ", validStatuses)}");
            result.IsValid = false;
        }

        // Validate logic if specified
        if (keyChainInstruction.Arguments.Count >= 2)
        {
            var logic = keyChainInstruction.Arguments[1];
            if (!logic.Equals("OR", StringComparison.OrdinalIgnoreCase) && 
                !logic.Equals("AND", StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add($"Invalid KEYCHAIN logic: {logic}. Valid values: OR, AND");
                result.IsValid = false;
            }
        }

        // Validate KEY sub-instructions
        if (keyChainInstruction.SubInstructions.Count == 0)
        {
            result.Warnings.Add($"KEYCHAIN {status} has no KEY statements");
        }

        foreach (var keyInstruction in keyChainInstruction.SubInstructions)
        {
            ValidateKey(keyInstruction, result);
        }
    }

    private void ValidateKey(ScriptInstruction keyInstruction, CommandValidationResult result)
    {
        if (!keyInstruction.CommandName.Equals("KEY", StringComparison.OrdinalIgnoreCase))
        {
            result.Warnings.Add($"Unknown sub-command in KEYCHAIN: {keyInstruction.CommandName}");
            return;
        }

        // Validate KEY arguments: SOURCE CONDITION VALUE
        if (keyInstruction.Arguments.Count < 3)
        {
            result.Errors.Add("KEY must have SOURCE, CONDITION, and VALUE arguments");
            result.IsValid = false;
            return;
        }

        var source = keyInstruction.Arguments[0];
        var condition = keyInstruction.Arguments[1];
        var value = keyInstruction.Arguments[2];

        // Validate source
        if (string.IsNullOrWhiteSpace(source))
        {
            result.Errors.Add("KEY source cannot be empty");
            result.IsValid = false;
        }

        // Validate condition
        var validConditions = Enum.GetNames<KeyCondition>();
        if (!validConditions.Contains(condition, StringComparer.OrdinalIgnoreCase))
        {
            result.Errors.Add($"Invalid KEY condition: {condition}. Valid conditions: {string.Join(", ", validConditions)}");
            result.IsValid = false;
        }

        // Validate value for regex conditions
        if (condition.Equals("MatchesRegex", StringComparison.OrdinalIgnoreCase) ||
            condition.Equals("DoesNotMatchRegex", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var _ = new System.Text.RegularExpressions.Regex(value);
            }
            catch
            {
                result.Warnings.Add($"KEY regex pattern may be invalid: {value}");
            }
        }

        // Validate numeric values for numeric conditions
        var numericConditions = new[] { "GreaterThan", "LessThan", "GreaterThanOrEqualTo", "LessThanOrEqualTo" };
        if (numericConditions.Contains(condition, StringComparer.OrdinalIgnoreCase))
        {
            if (!double.TryParse(value, out _))
            {
                result.Warnings.Add($"KEY numeric condition '{condition}' expects a numeric value, got: {value}");
            }
        }
    }

    private List<KeyChain> ParseKeyChains(List<ScriptInstruction> subInstructions, BotData botData)
    {
        var keyChains = new List<KeyChain>();

        foreach (var instruction in subInstructions)
        {
            if (!instruction.CommandName.Equals("KEYCHAIN", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var keyChain = ParseKeyChain(instruction, botData);
            if (keyChain != null)
            {
                keyChains.Add(keyChain);
            }
        }

        return keyChains;
    }

    private KeyChain? ParseKeyChain(ScriptInstruction instruction, BotData botData)
    {
        if (instruction.Arguments.Count == 0)
        {
            _logger.LogWarning("KEYCHAIN instruction has no arguments");
            return null;
        }

        var statusText = instruction.Arguments[0];
        var logic = instruction.Arguments.Count >= 2 ? 
            instruction.Arguments[1] : "OR";

        // Parse status
        if (!Enum.TryParse<BotStatus>(statusText, true, out var status))
        {
            _logger.LogWarning("Invalid KEYCHAIN status: {Status}", statusText);
            return null;
        }

        // Parse logic
        if (!Enum.TryParse<KeyChainLogic>(logic, true, out var chainLogic))
        {
            _logger.LogWarning("Invalid KEYCHAIN logic: {Logic}, defaulting to OR", logic);
            chainLogic = KeyChainLogic.OR;
        }

        var keyChain = new KeyChain
        {
            Status = status,
            Logic = chainLogic,
            Name = statusText
        };

        // Handle custom status
        if (status == BotStatus.Custom && instruction.Arguments.Count >= 3)
        {
            keyChain.CustomStatus = SubstituteVariables(instruction.Arguments[2], botData);
        }

        // Handle special properties
        if (status == BotStatus.Ban)
        {
            keyChain.BanProxy = true;
        }
        else if (status == BotStatus.Retry)
        {
            keyChain.Retry = true;
        }

        // Parse keys
        foreach (var keyInstruction in instruction.SubInstructions)
        {
            if (keyInstruction.CommandName.Equals("KEY", StringComparison.OrdinalIgnoreCase))
            {
                var key = ParseKey(keyInstruction, botData);
                if (key != null)
                {
                    keyChain.Keys.Add(key);
                }
            }
        }

        return keyChain;
    }

    private Key? ParseKey(ScriptInstruction instruction, BotData botData)
    {
        if (instruction.Arguments.Count < 3)
        {
            _logger.LogWarning("KEY instruction has insufficient arguments");
            return null;
        }

        var source = SubstituteVariables(instruction.Arguments[0], botData);
        var conditionText = instruction.Arguments[1];
        var value = SubstituteVariables(instruction.Arguments[2], botData);

        // Parse condition
        if (!Enum.TryParse<KeyCondition>(conditionText, true, out var condition))
        {
            _logger.LogWarning("Invalid KEY condition: {Condition}", conditionText);
            return null;
        }

        // Check for case sensitivity parameter
        var caseSensitive = false;
        if (instruction.Parameters.TryGetValue("CaseSensitive", out var caseSensitiveValue))
        {
            caseSensitive = Convert.ToBoolean(caseSensitiveValue);
        }

        return new Key
        {
            Source = source,
            Condition = condition,
            Value = value,
            CaseSensitive = caseSensitive,
            Name = $"{source}_{condition}_{value}"
        };
    }

    private string SubstituteVariables(string input, BotData botData)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var variables = botData.Variables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return _scriptParser.SubstituteVariables(input, variables);
    }
}

/// <summary>
/// KEYCHECK command extensions and utilities
/// </summary>
public static class KeyCheckCommandExtensions
{
    /// <summary>
    /// Creates a KEYCHECK instruction from key chains
    /// </summary>
    public static ScriptInstruction CreateKeyCheckInstruction(IEnumerable<KeyChain> keyChains)
    {
        var instruction = new ScriptInstruction
        {
            CommandName = "KEYCHECK",
            Arguments = new List<string>(),
            SubInstructions = new List<ScriptInstruction>()
        };

        foreach (var keyChain in keyChains)
        {
            var keyChainInstruction = new ScriptInstruction
            {
                CommandName = "KEYCHAIN",
                Arguments = new List<string> { keyChain.Status.ToString(), keyChain.Logic.ToString() },
                SubInstructions = new List<ScriptInstruction>()
            };

            if (keyChain.Status == BotStatus.Custom && !string.IsNullOrEmpty(keyChain.CustomStatus))
            {
                keyChainInstruction.Arguments.Add(keyChain.CustomStatus);
            }

            foreach (var key in keyChain.Keys)
            {
                var keyInstruction = new ScriptInstruction
                {
                    CommandName = "KEY",
                    Arguments = new List<string> { key.Source, key.Condition.ToString(), key.Value },
                    Parameters = new Dictionary<string, object>()
                };

                if (key.CaseSensitive)
                {
                    keyInstruction.Parameters["CaseSensitive"] = true;
                }

                keyChainInstruction.SubInstructions.Add(keyInstruction);
            }

            instruction.SubInstructions.Add(keyChainInstruction);
        }

        return instruction;
    }

    /// <summary>
    /// Creates a simple success/failure key check
    /// </summary>
    public static ScriptInstruction CreateSimpleKeyCheck(
        string successPattern, 
        string failurePattern, 
        string source = "<SOURCE>",
        KeyCondition condition = KeyCondition.Contains)
    {
        var keyChains = new List<KeyChain>();

        // Success chain
        if (!string.IsNullOrEmpty(successPattern))
        {
            var successChain = KeyChain.Success();
            successChain.Keys.Add(Key.Contains(source, successPattern));
            keyChains.Add(successChain);
        }

        // Failure chain
        if (!string.IsNullOrEmpty(failurePattern))
        {
            var failureChain = KeyChain.Failure();
            failureChain.Keys.Add(Key.Contains(source, failurePattern));
            keyChains.Add(failureChain);
        }

        return CreateKeyCheckInstruction(keyChains);
    }
}
