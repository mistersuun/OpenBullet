using OpenBullet.Core.Models;

namespace OpenBullet.Core.KeyChecking;

/// <summary>
/// Interface for key checking operations
/// </summary>
public interface IKeyChecker
{
    /// <summary>
    /// Evaluates all key chains and returns the result
    /// </summary>
    KeyCheckResult EvaluateKeyChains(List<KeyChain> keyChains, BotData botData);

    /// <summary>
    /// Evaluates a single key chain
    /// </summary>
    bool EvaluateKeyChain(KeyChain keyChain, BotData botData);

    /// <summary>
    /// Evaluates a single key condition
    /// </summary>
    bool EvaluateKey(Key key, BotData botData);

    /// <summary>
    /// Gets the value from the specified source
    /// </summary>
    string GetSourceValue(string source, BotData botData);
}

/// <summary>
/// Result of key checking operation
/// </summary>
public class KeyCheckResult
{
    public bool Success { get; set; }
    public BotStatus Status { get; set; } = BotStatus.None;
    public string? CustomStatus { get; set; }
    public KeyChain? MatchedKeyChain { get; set; }
    public List<KeyEvaluation> Evaluations { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public bool ShouldBanProxy { get; set; }
    public bool ShouldRetry { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Evaluation result for a single key
/// </summary>
public class KeyEvaluation
{
    public Key Key { get; set; } = new();
    public bool Result { get; set; }
    public string ActualValue { get; set; } = string.Empty;
    public string ExpectedValue { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public TimeSpan EvaluationTime { get; set; }
}

/// <summary>
/// Represents a key chain with multiple keys and logic
/// </summary>
public class KeyChain
{
    public BotStatus Status { get; set; } = BotStatus.Success;
    public string? CustomStatus { get; set; }
    public KeyChainLogic Logic { get; set; } = KeyChainLogic.OR;
    public List<Key> Keys { get; set; } = new();
    public bool BanProxy { get; set; } = false;
    public bool Retry { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Creates a success key chain
    /// </summary>
    public static KeyChain Success(KeyChainLogic logic = KeyChainLogic.OR) => new()
    {
        Status = BotStatus.Success,
        Logic = logic,
        Name = "Success"
    };

    /// <summary>
    /// Creates a failure key chain
    /// </summary>
    public static KeyChain Failure(KeyChainLogic logic = KeyChainLogic.OR) => new()
    {
        Status = BotStatus.Failure,
        Logic = logic,
        Name = "Failure"
    };

    /// <summary>
    /// Creates a ban key chain
    /// </summary>
    public static KeyChain Ban(KeyChainLogic logic = KeyChainLogic.OR) => new()
    {
        Status = BotStatus.Ban,
        Logic = logic,
        BanProxy = true,
        Name = "Ban"
    };

    /// <summary>
    /// Creates a retry key chain
    /// </summary>
    public static KeyChain CreateRetry(KeyChainLogic logic = KeyChainLogic.OR) => new()
    {
        Status = BotStatus.Retry,
        Logic = logic,
        Retry = true,
        Name = "Retry"
    };

    /// <summary>
    /// Creates a custom status key chain
    /// </summary>
    public static KeyChain Custom(string customStatus, KeyChainLogic logic = KeyChainLogic.OR) => new()
    {
        Status = BotStatus.Custom,
        CustomStatus = customStatus,
        Logic = logic,
        Name = "Custom"
    };
}

/// <summary>
/// Represents a single key condition
/// </summary>
public class Key
{
    public string Source { get; set; } = string.Empty;
    public KeyCondition Condition { get; set; } = KeyCondition.Contains;
    public string Value { get; set; } = string.Empty;
    public bool CaseSensitive { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Creates a contains key
    /// </summary>
    public static Key Contains(string source, string value, bool caseSensitive = false) => new()
    {
        Source = source,
        Condition = KeyCondition.Contains,
        Value = value,
        CaseSensitive = caseSensitive
    };

    /// <summary>
    /// Creates an equals key
    /// </summary>
    public static Key EqualTo(string source, string value, bool caseSensitive = false) => new()
    {
        Source = source,
        Condition = KeyCondition.EqualTo,
        Value = value,
        CaseSensitive = caseSensitive
    };

    /// <summary>
    /// Creates a not contains key
    /// </summary>
    public static Key DoesNotContain(string source, string value, bool caseSensitive = false) => new()
    {
        Source = source,
        Condition = KeyCondition.DoesNotContain,
        Value = value,
        CaseSensitive = caseSensitive
    };

    /// <summary>
    /// Creates a regex match key
    /// </summary>
    public static Key MatchesRegex(string source, string pattern, bool caseSensitive = false) => new()
    {
        Source = source,
        Condition = KeyCondition.MatchesRegex,
        Value = pattern,
        CaseSensitive = caseSensitive
    };
}

/// <summary>
/// Logic operators for key chains
/// </summary>
public enum KeyChainLogic
{
    OR,
    AND
}

/// <summary>
/// Key condition types
/// </summary>
public enum KeyCondition
{
    Contains,
    DoesNotContain,
    EqualTo,
    NotEqualTo,
    GreaterThan,
    LessThan,
    GreaterThanOrEqualTo,
    LessThanOrEqualTo,
    StartsWith,
    EndsWith,
    MatchesRegex,
    DoesNotMatchRegex,
    IsEmpty,
    IsNotEmpty,
    IsNumeric,
    IsNotNumeric,
    LengthEqualTo,
    LengthGreaterThan,
    LengthLessThan
}

/// <summary>
/// Key checking configuration
/// </summary>
public class KeyCheckConfiguration
{
    public bool StopOnFirstMatch { get; set; } = true;
    public bool EnableBanProxy { get; set; } = true;
    public bool EnableRetry { get; set; } = true;
    public TimeSpan EvaluationTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public bool LogEvaluations { get; set; } = true;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}
