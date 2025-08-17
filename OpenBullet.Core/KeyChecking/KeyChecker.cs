using Microsoft.Extensions.Logging;
using OpenBullet.Core.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OpenBullet.Core.KeyChecking;

/// <summary>
/// Implementation of key checking operations
/// </summary>
public class KeyChecker : IKeyChecker
{
    private readonly ILogger<KeyChecker> _logger;
    private readonly KeyCheckConfiguration _configuration;

    public KeyChecker(ILogger<KeyChecker> logger, KeyCheckConfiguration? configuration = null)
    {
        _logger = logger;
        _configuration = configuration ?? new KeyCheckConfiguration();
    }

    public KeyCheckResult EvaluateKeyChains(List<KeyChain> keyChains, BotData botData)
    {
        var result = new KeyCheckResult();
        var evaluations = new List<KeyEvaluation>();

        try
        {
            _logger.LogTrace("Evaluating {Count} key chains for bot {BotId}", keyChains.Count, botData.Id);

            foreach (var keyChain in keyChains)
            {
                var chainResult = EvaluateKeyChain(keyChain, botData);
                
                // Collect evaluations for this chain
                foreach (var key in keyChain.Keys)
                {
                    var keyEvaluation = EvaluateKeyWithDetails(key, botData);
                    evaluations.Add(keyEvaluation);
                }

                if (chainResult)
                {
                    result.Success = true;
                    result.Status = keyChain.Status;
                    result.CustomStatus = keyChain.CustomStatus;
                    result.MatchedKeyChain = keyChain;
                    result.ShouldBanProxy = keyChain.BanProxy && _configuration.EnableBanProxy;
                    result.ShouldRetry = keyChain.Retry && _configuration.EnableRetry;

                    _logger.LogDebug("Key chain '{ChainName}' matched with status {Status}", 
                        keyChain.Name, keyChain.Status);

                    if (_configuration.StopOnFirstMatch)
                    {
                        break;
                    }
                }
            }

            result.Evaluations = evaluations;

            // If no key chain matched, default to failure
            if (!result.Success)
            {
                result.Status = BotStatus.Failure;
                _logger.LogDebug("No key chains matched, defaulting to Failure status");
            }

            // Add metadata
            result.Metadata["TotalKeyChains"] = keyChains.Count;
            result.Metadata["TotalKeys"] = keyChains.SelectMany(kc => kc.Keys).Count();
            result.Metadata["EvaluationCount"] = evaluations.Count;
            result.Metadata["MatchedChain"] = result.MatchedKeyChain?.Name ?? "None";

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating key chains for bot {BotId}", botData.Id);
            result.Success = false;
            result.Status = BotStatus.Error;
            result.ErrorMessage = ex.Message;
            result.Evaluations = evaluations;
            return result;
        }
    }

    public bool EvaluateKeyChain(KeyChain keyChain, BotData botData)
    {
        if (keyChain.Keys.Count == 0)
        {
            _logger.LogWarning("Key chain '{ChainName}' has no keys", keyChain.Name);
            return false;
        }

        var keyResults = keyChain.Keys.Select(key => EvaluateKey(key, botData)).ToList();

        return keyChain.Logic switch
        {
            KeyChainLogic.OR => keyResults.Any(r => r),
            KeyChainLogic.AND => keyResults.All(r => r),
            _ => throw new ArgumentException($"Unknown key chain logic: {keyChain.Logic}")
        };
    }

    public bool EvaluateKey(Key key, BotData botData)
    {
        try
        {
            var sourceValue = GetSourceValue(key.Source, botData);
            var targetValue = SubstituteVariables(key.Value, botData);

            return EvaluateCondition(sourceValue, key.Condition, targetValue, key.CaseSensitive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating key {KeyName} with condition {Condition}", 
                key.Name, key.Condition);
            return false;
        }
    }

    public string GetSourceValue(string source, BotData botData)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;

        // Handle special source values
        return source.ToUpperInvariant() switch
        {
            "<SOURCE>" => botData.Source,
            "<ADDRESS>" => botData.Address,
            "<RESPONSECODE>" => botData.ResponseCode.ToString(),
            "<STATUS>" => botData.Status.ToString(),
            "<CUSTOMSTATUS>" => botData.CustomStatus,
            _ => GetVariableOrLiteralValue(source, botData)
        };
    }

    private string GetVariableOrLiteralValue(string source, BotData botData)
    {
        // Check if it's a variable reference
        if (source.StartsWith('<') && source.EndsWith('>'))
        {
            var variableName = source[1..^1]; // Remove < and >
            
            if (botData.Variables.TryGetValue(variableName, out var value))
            {
                return value?.ToString() ?? string.Empty;
            }
            
            if (botData.CapturedData.TryGetValue(variableName, out var capturedValue))
            {
                return capturedValue?.ToString() ?? string.Empty;
            }

            _logger.LogWarning("Variable '{VariableName}' not found, using empty string", variableName);
            return string.Empty;
        }

        // Return as literal value
        return source;
    }

    private string SubstituteVariables(string input, BotData botData)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = input;

        // Replace variable references
        var regex = new Regex(@"<([^>]+)>", RegexOptions.Compiled);
        var matches = regex.Matches(input);

        foreach (Match match in matches)
        {
            var variableName = match.Groups[1].Value;
            var variableValue = string.Empty;

            if (botData.Variables.TryGetValue(variableName, out var value))
            {
                variableValue = value?.ToString() ?? string.Empty;
            }
            else if (botData.CapturedData.TryGetValue(variableName, out var capturedValue))
            {
                variableValue = capturedValue?.ToString() ?? string.Empty;
            }
            else
            {
                variableValue = GetSourceValue($"<{variableName}>", botData);
            }

            result = result.Replace(match.Value, variableValue);
        }

        return result;
    }

    private bool EvaluateCondition(string sourceValue, KeyCondition condition, string targetValue, bool caseSensitive)
    {
        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        
        try
        {
            return condition switch
            {
                KeyCondition.Contains => sourceValue.Contains(targetValue, comparison),
                KeyCondition.DoesNotContain => !sourceValue.Contains(targetValue, comparison),
                KeyCondition.EqualTo => string.Equals(sourceValue, targetValue, comparison),
                KeyCondition.NotEqualTo => !string.Equals(sourceValue, targetValue, comparison),
                KeyCondition.StartsWith => sourceValue.StartsWith(targetValue, comparison),
                KeyCondition.EndsWith => sourceValue.EndsWith(targetValue, comparison),
                KeyCondition.IsEmpty => string.IsNullOrEmpty(sourceValue),
                KeyCondition.IsNotEmpty => !string.IsNullOrEmpty(sourceValue),
                KeyCondition.MatchesRegex => EvaluateRegex(sourceValue, targetValue, caseSensitive),
                KeyCondition.DoesNotMatchRegex => !EvaluateRegex(sourceValue, targetValue, caseSensitive),
                KeyCondition.IsNumeric => IsNumeric(sourceValue),
                KeyCondition.IsNotNumeric => !IsNumeric(sourceValue),
                KeyCondition.LengthEqualTo => EvaluateLengthCondition(sourceValue.Length, targetValue, "=="),
                KeyCondition.LengthGreaterThan => EvaluateLengthCondition(sourceValue.Length, targetValue, ">"),
                KeyCondition.LengthLessThan => EvaluateLengthCondition(sourceValue.Length, targetValue, "<"),
                KeyCondition.GreaterThan => EvaluateNumericCondition(sourceValue, targetValue, ">"),
                KeyCondition.LessThan => EvaluateNumericCondition(sourceValue, targetValue, "<"),
                KeyCondition.GreaterThanOrEqualTo => EvaluateNumericCondition(sourceValue, targetValue, ">="),
                KeyCondition.LessThanOrEqualTo => EvaluateNumericCondition(sourceValue, targetValue, "<="),
                _ => throw new ArgumentException($"Unknown key condition: {condition}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating condition {Condition} with source '{Source}' and target '{Target}'", 
                condition, sourceValue, targetValue);
            return false;
        }
    }

    private bool EvaluateRegex(string input, string pattern, bool caseSensitive)
    {
        try
        {
            var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            var regex = new Regex(pattern, options | RegexOptions.Compiled);
            return regex.IsMatch(input);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid regex pattern: {Pattern}", pattern);
            return false;
        }
    }

    private bool IsNumeric(string value)
    {
        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
    }

    private bool EvaluateNumericCondition(string sourceValue, string targetValue, string operatorSymbol)
    {
        if (!double.TryParse(sourceValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var sourceNum) ||
            !double.TryParse(targetValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var targetNum))
        {
            return false;
        }

        return operatorSymbol switch
        {
            ">" => sourceNum > targetNum,
            "<" => sourceNum < targetNum,
            ">=" => sourceNum >= targetNum,
            "<=" => sourceNum <= targetNum,
            "==" => Math.Abs(sourceNum - targetNum) < double.Epsilon,
            _ => false
        };
    }

    private bool EvaluateLengthCondition(int sourceLength, string targetValue, string operatorSymbol)
    {
        if (!int.TryParse(targetValue, out var targetLength))
        {
            return false;
        }

        return operatorSymbol switch
        {
            ">" => sourceLength > targetLength,
            "<" => sourceLength < targetLength,
            ">=" => sourceLength >= targetLength,
            "<=" => sourceLength <= targetLength,
            "==" => sourceLength == targetLength,
            _ => false
        };
    }

    private KeyEvaluation EvaluateKeyWithDetails(Key key, BotData botData)
    {
        var evaluation = new KeyEvaluation
        {
            Key = key,
            Source = key.Source,
            Condition = key.Condition.ToString(),
            ExpectedValue = key.Value
        };

        var startTime = DateTime.UtcNow;

        try
        {
            evaluation.ActualValue = GetSourceValue(key.Source, botData);
            evaluation.Result = EvaluateKey(key, botData);
        }
        catch (Exception ex)
        {
            evaluation.Result = false;
            evaluation.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error in detailed key evaluation for key {KeyName}", key.Name);
        }
        finally
        {
            evaluation.EvaluationTime = DateTime.UtcNow - startTime;
        }

        return evaluation;
    }
}

/// <summary>
/// Key checker extensions and utilities
/// </summary>
public static class KeyCheckerExtensions
{
    /// <summary>
    /// Applies the key check result to bot data
    /// </summary>
    public static void ApplyToBotData(this KeyCheckResult result, BotData botData)
    {
        if (result.Success)
        {
            botData.Status = result.Status;
            
            if (!string.IsNullOrEmpty(result.CustomStatus))
            {
                botData.CustomStatus = result.CustomStatus;
            }

            // Add log entry
            var statusText = result.Status == BotStatus.Custom ? result.CustomStatus : result.Status.ToString();
            botData.AddLogEntry($"KEYCHECK: {statusText}");

            // Store evaluation metadata if needed
            if (result.Evaluations.Count > 0)
            {
                botData.SetVariable("_KEYCHECK_EVALUATIONS", result.Evaluations.Count);
                botData.SetVariable("_KEYCHECK_MATCHED_CHAIN", result.MatchedKeyChain?.Name ?? "None");
            }
        }
        else
        {
            botData.Status = BotStatus.Error;
            botData.AddLogEntry($"KEYCHECK Error: {result.ErrorMessage ?? "Unknown error"}");
        }
    }

    /// <summary>
    /// Creates a simple success key chain
    /// </summary>
    public static KeyChain CreateSuccessChain(this IKeyChecker checker, params Key[] keys)
    {
        var chain = KeyChain.Success();
        chain.Keys.AddRange(keys);
        return chain;
    }

    /// <summary>
    /// Creates a simple failure key chain
    /// </summary>
    public static KeyChain CreateFailureChain(this IKeyChecker checker, params Key[] keys)
    {
        var chain = KeyChain.Failure();
        chain.Keys.AddRange(keys);
        return chain;
    }

    /// <summary>
    /// Creates a ban key chain
    /// </summary>
    public static KeyChain CreateBanChain(this IKeyChecker checker, params Key[] keys)
    {
        var chain = KeyChain.Ban();
        chain.Keys.AddRange(keys);
        return chain;
    }
}

/// <summary>
/// Builder for creating key chains fluently
/// </summary>
public class KeyChainBuilder
{
    private readonly KeyChain _keyChain;

    public KeyChainBuilder(BotStatus status)
    {
        _keyChain = new KeyChain { Status = status };
    }

    public KeyChainBuilder WithLogic(KeyChainLogic logic)
    {
        _keyChain.Logic = logic;
        return this;
    }

    public KeyChainBuilder WithName(string name)
    {
        _keyChain.Name = name;
        return this;
    }

    public KeyChainBuilder WithCustomStatus(string customStatus)
    {
        _keyChain.CustomStatus = customStatus;
        _keyChain.Status = BotStatus.Custom;
        return this;
    }

    public KeyChainBuilder WithBanProxy(bool banProxy = true)
    {
        _keyChain.BanProxy = banProxy;
        return this;
    }

    public KeyChainBuilder WithRetry(bool retry = true)
    {
        _keyChain.Retry = retry;
        return this;
    }

    public KeyChainBuilder AddKey(Key key)
    {
        _keyChain.Keys.Add(key);
        return this;
    }

    public KeyChainBuilder AddKey(string source, KeyCondition condition, string value, bool caseSensitive = false)
    {
        _keyChain.Keys.Add(new Key
        {
            Source = source,
            Condition = condition,
            Value = value,
            CaseSensitive = caseSensitive
        });
        return this;
    }

    public KeyChain Build() => _keyChain;

    // Static factory methods
    public static KeyChainBuilder Success() => new(BotStatus.Success);
    public static KeyChainBuilder Failure() => new(BotStatus.Failure);
    public static KeyChainBuilder Ban() => new(BotStatus.Ban);
    public static KeyChainBuilder Retry() => new(BotStatus.Retry);
    public static KeyChainBuilder Custom(string status) => new(BotStatus.Custom) { _keyChain = { CustomStatus = status } };
}
