using OpenBullet.Core.Parsing;

namespace OpenBullet.Core.Variables;

/// <summary>
/// Interface for variable management during script execution
/// </summary>
public interface IVariableManager
{
    /// <summary>
    /// Sets a variable value
    /// </summary>
    void SetVariable(string name, object? value, VariableScope scope = VariableScope.Local);

    /// <summary>
    /// Gets a variable value
    /// </summary>
    T? GetVariable<T>(string name, VariableScope scope = VariableScope.Any);

    /// <summary>
    /// Gets a variable value as object
    /// </summary>
    object? GetVariable(string name, VariableScope scope = VariableScope.Any);

    /// <summary>
    /// Checks if a variable exists
    /// </summary>
    bool HasVariable(string name, VariableScope scope = VariableScope.Any);

    /// <summary>
    /// Removes a variable
    /// </summary>
    bool RemoveVariable(string name, VariableScope scope = VariableScope.Any);

    /// <summary>
    /// Gets all variables in a scope
    /// </summary>
    Dictionary<string, object?> GetAllVariables(VariableScope scope);

    /// <summary>
    /// Clears all variables in a scope
    /// </summary>
    void ClearVariables(VariableScope scope);

    /// <summary>
    /// Resolves a variable reference to its actual value
    /// </summary>
    object? ResolveVariableReference(VariableReference reference);

    /// <summary>
    /// Sets a list variable
    /// </summary>
    void SetList(string name, IList<object?> values, VariableScope scope = VariableScope.Local);

    /// <summary>
    /// Gets a list variable
    /// </summary>
    IList<object?>? GetList(string name, VariableScope scope = VariableScope.Any);

    /// <summary>
    /// Adds an item to a list variable
    /// </summary>
    void AddToList(string name, object? value, VariableScope scope = VariableScope.Local);

    /// <summary>
    /// Sets a dictionary variable
    /// </summary>
    void SetDictionary(string name, IDictionary<string, object?> values, VariableScope scope = VariableScope.Local);

    /// <summary>
    /// Gets a dictionary variable
    /// </summary>
    IDictionary<string, object?>? GetDictionary(string name, VariableScope scope = VariableScope.Any);

    /// <summary>
    /// Sets a dictionary value
    /// </summary>
    void SetDictionaryValue(string name, string key, object? value, VariableScope scope = VariableScope.Local);

    /// <summary>
    /// Gets variable metadata
    /// </summary>
    VariableMetadata? GetVariableMetadata(string name, VariableScope scope = VariableScope.Any);

    /// <summary>
    /// Gets variable statistics
    /// </summary>
    VariableStatistics GetStatistics();

    /// <summary>
    /// Creates a variable snapshot for debugging
    /// </summary>
    VariableSnapshot CreateSnapshot();

    /// <summary>
    /// Restores variables from a snapshot
    /// </summary>
    void RestoreSnapshot(VariableSnapshot snapshot);

    /// <summary>
    /// Event fired when a variable changes
    /// </summary>
    event EventHandler<VariableChangedEventArgs>? VariableChanged;
}

/// <summary>
/// Variable scope enumeration
/// </summary>
public enum VariableScope
{
    Local,    // Bot-specific variables
    Global,   // Shared across all bots
    Any       // Search both scopes (Local first, then Global)
}

/// <summary>
/// Variable metadata information
/// </summary>
public class VariableMetadata
{
    public string Name { get; set; } = string.Empty;
    public Type? ValueType { get; set; }
    public VariableScope Scope { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public int AccessCount { get; set; }
    public string? Description { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsTemporary { get; set; }
    public TimeSpan? TimeToLive { get; set; }
}

/// <summary>
/// Variable statistics
/// </summary>
public class VariableStatistics
{
    public int LocalVariableCount { get; set; }
    public int GlobalVariableCount { get; set; }
    public int TotalVariableCount => LocalVariableCount + GlobalVariableCount;
    public int ListVariableCount { get; set; }
    public int DictionaryVariableCount { get; set; }
    public int SimpleVariableCount { get; set; }
    public long MemoryUsageBytes { get; set; }
    public DateTime LastCleanup { get; set; }
    public int CleanupCount { get; set; }
    public Dictionary<string, int> TypeCounts { get; set; } = new();
}

/// <summary>
/// Variable snapshot for backup/restore
/// </summary>
public class VariableSnapshot
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object?> LocalVariables { get; set; } = new();
    public Dictionary<string, object?> GlobalVariables { get; set; } = new();
    public Dictionary<string, VariableMetadata> Metadata { get; set; } = new();
    public string? Description { get; set; }
}

/// <summary>
/// Variable changed event arguments
/// </summary>
public class VariableChangedEventArgs : EventArgs
{
    public string VariableName { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
    public VariableScope Scope { get; set; }
    public VariableChangeType ChangeType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Variable change types
/// </summary>
public enum VariableChangeType
{
    Created,
    Updated,
    Deleted,
    Accessed
}
