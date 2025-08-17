using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Parsing;

namespace OpenBullet.Core.Variables;

/// <summary>
/// Variable manager implementation for script execution
/// </summary>
public class VariableManager : IVariableManager, IDisposable
{
    private readonly ILogger<VariableManager> _logger;
    private readonly Dictionary<string, object?> _localVariables = new();
    private readonly Dictionary<string, VariableMetadata> _localMetadata = new();
    private readonly object _localLock = new();
    
    // Global variables are shared across all instances
    private static readonly ConcurrentDictionary<string, object?> _globalVariables = new();
    private static readonly ConcurrentDictionary<string, VariableMetadata> _globalMetadata = new();
    
    private readonly Timer _cleanupTimer;
    private bool _disposed = false;

    public event EventHandler<VariableChangedEventArgs>? VariableChanged;

    public VariableManager(ILogger<VariableManager> logger)
    {
        _logger = logger;
        
        // Setup cleanup timer to run every 5 minutes
        _cleanupTimer = new Timer(PerformCleanup, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        
        _logger.LogDebug("VariableManager initialized");
    }

    public void SetVariable(string name, object? value, VariableScope scope = VariableScope.Local)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        
        var oldValue = GetVariableValueWithoutEvent(name, scope);
        var changeType = HasVariable(name, scope) ? VariableChangeType.Updated : VariableChangeType.Created;

        switch (scope)
        {
            case VariableScope.Local:
                lock (_localLock)
                {
                    _localVariables[name] = value;
                    UpdateMetadata(name, value, VariableScope.Local);
                }
                break;
                
            case VariableScope.Global:
                _globalVariables[name] = value;
                UpdateMetadata(name, value, VariableScope.Global);
                break;
                
            default:
                throw new ArgumentException($"Cannot set variable with scope {scope}");
        }

        OnVariableChanged(new VariableChangedEventArgs
        {
            VariableName = name,
            OldValue = oldValue,
            NewValue = value,
            Scope = scope,
            ChangeType = changeType
        });

        _logger.LogTrace("Set variable {Name} = {Value} in {Scope} scope", name, value, scope);
    }

    public T? GetVariable<T>(string name, VariableScope scope = VariableScope.Any)
    {
        var value = GetVariable(name, scope);
        
        if (value == null)
            return default(T);
            
        try
        {
            // Handle type conversions
            if (typeof(T) == typeof(string))
                return (T)(object)value.ToString()!;
                
            if (value is T typedValue)
                return typedValue;
                
            // Try to convert using Convert class
            if (typeof(T).IsPrimitive || typeof(T) == typeof(decimal))
                return (T)Convert.ChangeType(value, typeof(T));
                
            // Try JSON deserialization for complex types
            if (value is string jsonString && !typeof(T).IsPrimitive)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(jsonString);
                }
                catch
                {
                    // Fall through to default
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to convert variable {Name} of type {ActualType} to {TargetType}", 
                name, value.GetType(), typeof(T));
        }
        
        return default(T);
    }

    public object? GetVariable(string name, VariableScope scope = VariableScope.Any)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        object? value = null;
        var found = false;

        switch (scope)
        {
            case VariableScope.Local:
                lock (_localLock)
                {
                    found = _localVariables.TryGetValue(name, out value);
                }
                break;
                
            case VariableScope.Global:
                found = _globalVariables.TryGetValue(name, out value);
                break;
                
            case VariableScope.Any:
                // Check local first, then global
                lock (_localLock)
                {
                    found = _localVariables.TryGetValue(name, out value);
                }
                if (!found)
                {
                    found = _globalVariables.TryGetValue(name, out value);
                }
                break;
        }

        if (found)
        {
            // Update access count
            IncrementAccessCount(name, scope);
            
            OnVariableChanged(new VariableChangedEventArgs
            {
                VariableName = name,
                NewValue = value,
                Scope = scope,
                ChangeType = VariableChangeType.Accessed
            });
        }

        return value;
    }

    public bool HasVariable(string name, VariableScope scope = VariableScope.Any)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return scope switch
        {
            VariableScope.Local => _localVariables.ContainsKey(name),
            VariableScope.Global => _globalVariables.ContainsKey(name),
            VariableScope.Any => _localVariables.ContainsKey(name) || _globalVariables.ContainsKey(name),
            _ => false
        };
    }

    public bool RemoveVariable(string name, VariableScope scope = VariableScope.Any)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var oldValue = GetVariableValueWithoutEvent(name, scope);
        var removed = false;

        switch (scope)
        {
            case VariableScope.Local:
                lock (_localLock)
                {
                    removed = _localVariables.Remove(name);
                    _localMetadata.Remove(name);
                }
                break;
                
            case VariableScope.Global:
                removed = _globalVariables.TryRemove(name, out _);
                _globalMetadata.TryRemove(name, out _);
                break;
                
            case VariableScope.Any:
                // Remove from both scopes
                lock (_localLock)
                {
                    var localRemoved = _localVariables.Remove(name);
                    _localMetadata.Remove(name);
                    removed = localRemoved;
                }
                var globalRemoved = _globalVariables.TryRemove(name, out _);
                _globalMetadata.TryRemove(name, out _);
                removed = removed || globalRemoved;
                break;
        }

        if (removed)
        {
            OnVariableChanged(new VariableChangedEventArgs
            {
                VariableName = name,
                OldValue = oldValue,
                Scope = scope,
                ChangeType = VariableChangeType.Deleted
            });
            
            _logger.LogTrace("Removed variable {Name} from {Scope} scope", name, scope);
        }

        return removed;
    }

    public Dictionary<string, object?> GetAllVariables(VariableScope scope)
    {
        return scope switch
        {
            VariableScope.Local => new Dictionary<string, object?>(_localVariables),
            VariableScope.Global => new Dictionary<string, object?>(_globalVariables),
            VariableScope.Any => MergeVariables(),
            _ => new Dictionary<string, object?>()
        };
    }

    public void ClearVariables(VariableScope scope)
    {
        switch (scope)
        {
            case VariableScope.Local:
                lock (_localLock)
                {
                    _localVariables.Clear();
                    _localMetadata.Clear();
                }
                break;
                
            case VariableScope.Global:
                _globalVariables.Clear();
                _globalMetadata.Clear();
                break;
                
            case VariableScope.Any:
                lock (_localLock)
                {
                    _localVariables.Clear();
                    _localMetadata.Clear();
                }
                _globalVariables.Clear();
                _globalMetadata.Clear();
                break;
        }
        
        _logger.LogDebug("Cleared variables in {Scope} scope", scope);
    }

    public object? ResolveVariableReference(VariableReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        var baseValue = GetVariable(reference.VariableName);
        if (baseValue == null)
            return null;

        return reference.Type switch
        {
            VariableType.Single => baseValue,
            VariableType.List => ResolveListReference(baseValue, reference.IndexOrKey),
            VariableType.Dictionary => ResolveDictionaryReference(baseValue, reference.IndexOrKey),
            _ => baseValue
        };
    }

    public void SetList(string name, IList<object?> values, VariableScope scope = VariableScope.Local)
    {
        SetVariable(name, values, scope);
    }

    public IList<object?>? GetList(string name, VariableScope scope = VariableScope.Any)
    {
        var value = GetVariable(name, scope);
        return value as IList<object?>;
    }

    public void AddToList(string name, object? value, VariableScope scope = VariableScope.Local)
    {
        var list = GetList(name, scope);
        if (list == null)
        {
            list = new List<object?>();
            SetList(name, list, scope);
        }
        
        list.Add(value);
        _logger.LogTrace("Added item to list {Name}: {Value}", name, value);
    }

    public void SetDictionary(string name, IDictionary<string, object?> values, VariableScope scope = VariableScope.Local)
    {
        SetVariable(name, values, scope);
    }

    public IDictionary<string, object?>? GetDictionary(string name, VariableScope scope = VariableScope.Any)
    {
        var value = GetVariable(name, scope);
        return value as IDictionary<string, object?>;
    }

    public void SetDictionaryValue(string name, string key, object? value, VariableScope scope = VariableScope.Local)
    {
        var dict = GetDictionary(name, scope);
        if (dict == null)
        {
            dict = new Dictionary<string, object?>();
            SetDictionary(name, dict, scope);
        }
        
        dict[key] = value;
        _logger.LogTrace("Set dictionary {Name}[{Key}] = {Value}", name, key, value);
    }

    public VariableMetadata? GetVariableMetadata(string name, VariableScope scope = VariableScope.Any)
    {
        return scope switch
        {
            VariableScope.Local => _localMetadata.TryGetValue(name, out var localMeta) ? localMeta : null,
            VariableScope.Global => _globalMetadata.TryGetValue(name, out var globalMeta) ? globalMeta : null,
            VariableScope.Any => _localMetadata.TryGetValue(name, out var anyMeta) ? anyMeta : 
                               _globalMetadata.TryGetValue(name, out anyMeta) ? anyMeta : null,
            _ => null
        };
    }

    public VariableStatistics GetStatistics()
    {
        var stats = new VariableStatistics
        {
            LocalVariableCount = _localVariables.Count,
            GlobalVariableCount = _globalVariables.Count,
            LastCleanup = DateTime.UtcNow,
            CleanupCount = 0
        };

        // Analyze variable types
        var allVariables = GetAllVariables(VariableScope.Any);
        foreach (var kvp in allVariables)
        {
            var value = kvp.Value;
            if (value == null) continue;

            var typeName = value.GetType().Name;
            stats.TypeCounts[typeName] = stats.TypeCounts.GetValueOrDefault(typeName, 0) + 1;

            if (value is IList<object?>)
                stats.ListVariableCount++;
            else if (value is IDictionary<string, object?>)
                stats.DictionaryVariableCount++;
            else
                stats.SimpleVariableCount++;
        }

        // Estimate memory usage (rough calculation)
        stats.MemoryUsageBytes = EstimateMemoryUsage(allVariables);

        return stats;
    }

    public VariableSnapshot CreateSnapshot()
    {
        var snapshot = new VariableSnapshot
        {
            LocalVariables = new Dictionary<string, object?>(_localVariables),
            GlobalVariables = new Dictionary<string, object?>(_globalVariables),
            Metadata = new Dictionary<string, VariableMetadata>()
        };

        // Copy metadata
        foreach (var kvp in _localMetadata)
        {
            snapshot.Metadata[kvp.Key] = CloneMetadata(kvp.Value);
        }
        
        foreach (var kvp in _globalMetadata)
        {
            if (!snapshot.Metadata.ContainsKey(kvp.Key))
            {
                snapshot.Metadata[kvp.Key] = CloneMetadata(kvp.Value);
            }
        }

        _logger.LogDebug("Created variable snapshot with {Count} variables", snapshot.LocalVariables.Count + snapshot.GlobalVariables.Count);
        return snapshot;
    }

    public void RestoreSnapshot(VariableSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        lock (_localLock)
        {
            _localVariables.Clear();
            _localMetadata.Clear();
            
            foreach (var kvp in snapshot.LocalVariables)
            {
                _localVariables[kvp.Key] = kvp.Value;
            }
        }

        _globalVariables.Clear();
        _globalMetadata.Clear();
        
        foreach (var kvp in snapshot.GlobalVariables)
        {
            _globalVariables[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in snapshot.Metadata)
        {
            if (kvp.Value.Scope == VariableScope.Local)
                _localMetadata[kvp.Key] = kvp.Value;
            else
                _globalMetadata[kvp.Key] = kvp.Value;
        }

        _logger.LogDebug("Restored variable snapshot with {Count} variables", 
            snapshot.LocalVariables.Count + snapshot.GlobalVariables.Count);
    }

    private Dictionary<string, object?> MergeVariables()
    {
        var merged = new Dictionary<string, object?>(_globalVariables);
        
        lock (_localLock)
        {
            foreach (var kvp in _localVariables)
            {
                merged[kvp.Key] = kvp.Value; // Local variables override global
            }
        }
        
        return merged;
    }

    private void UpdateMetadata(string name, object? value, VariableScope scope)
    {
        var metadata = GetVariableMetadata(name, scope) ?? new VariableMetadata
        {
            Name = name,
            Scope = scope,
            CreatedAt = DateTime.UtcNow
        };

        metadata.LastModified = DateTime.UtcNow;
        metadata.ValueType = value?.GetType();

        if (scope == VariableScope.Local)
        {
            _localMetadata[name] = metadata;
        }
        else
        {
            _globalMetadata[name] = metadata;
        }
    }

    private void IncrementAccessCount(string name, VariableScope scope)
    {
        var metadata = GetVariableMetadata(name, scope);
        if (metadata != null)
        {
            metadata.AccessCount++;
            metadata.LastModified = DateTime.UtcNow;
        }
    }

    private object? ResolveListReference(object baseValue, string? indexOrKey)
    {
        if (baseValue is not IList<object?> list)
            return null;

        if (indexOrKey == "*")
        {
            return string.Join(", ", list.Select(item => item?.ToString() ?? ""));
        }

        if (int.TryParse(indexOrKey, out var index) && index >= 0 && index < list.Count)
        {
            return list[index];
        }

        return null;
    }

    private object? ResolveDictionaryReference(object baseValue, string? indexOrKey)
    {
        if (baseValue is not IDictionary<string, object?> dict)
            return null;

        if (indexOrKey == "*")
        {
            return string.Join(", ", dict.Values.Select(item => item?.ToString() ?? ""));
        }

        if (indexOrKey != null && dict.TryGetValue(indexOrKey, out var value))
        {
            return value;
        }

        return null;
    }

    private long EstimateMemoryUsage(Dictionary<string, object?> variables)
    {
        long totalBytes = 0;
        
        foreach (var kvp in variables)
        {
            totalBytes += EstimateObjectSize(kvp.Key); // Key
            totalBytes += EstimateObjectSize(kvp.Value); // Value
        }
        
        return totalBytes;
    }

    private long EstimateObjectSize(object? obj)
    {
        if (obj == null) return 8; // Reference size
        
        return obj switch
        {
            string str => str.Length * 2 + 24, // UTF-16 + overhead
            int => 4,
            long => 8,
            double => 8,
            bool => 1,
            DateTime => 8,
            IList<object?> list => list.Count * 8 + 24, // Rough estimate
            IDictionary<string, object?> dict => dict.Count * 16 + 32, // Rough estimate
            _ => 32 // Default estimate for complex objects
        };
    }

    private VariableMetadata CloneMetadata(VariableMetadata original)
    {
        return new VariableMetadata
        {
            Name = original.Name,
            ValueType = original.ValueType,
            Scope = original.Scope,
            CreatedAt = original.CreatedAt,
            LastModified = original.LastModified,
            AccessCount = original.AccessCount,
            Description = original.Description,
            IsReadOnly = original.IsReadOnly,
            IsTemporary = original.IsTemporary,
            TimeToLive = original.TimeToLive
        };
    }

    private void PerformCleanup(object? state)
    {
        if (_disposed) return;

        try
        {
            var now = DateTime.UtcNow;
            var cleanupCount = 0;

            // Cleanup temporary variables with TTL
            var localToRemove = new List<string>();
            var globalToRemove = new List<string>();

            lock (_localLock)
            {
                foreach (var kvp in _localMetadata)
                {
                    var metadata = kvp.Value;
                    if (metadata.IsTemporary && metadata.TimeToLive.HasValue && 
                        now - metadata.CreatedAt > metadata.TimeToLive.Value)
                    {
                        localToRemove.Add(kvp.Key);
                    }
                }
                
                foreach (var key in localToRemove)
                {
                    _localVariables.Remove(key);
                    _localMetadata.Remove(key);
                    cleanupCount++;
                }
            }

            foreach (var kvp in _globalMetadata)
            {
                var metadata = kvp.Value;
                if (metadata.IsTemporary && metadata.TimeToLive.HasValue && 
                    now - metadata.CreatedAt > metadata.TimeToLive.Value)
                {
                    globalToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in globalToRemove)
            {
                _globalVariables.TryRemove(key, out _);
                _globalMetadata.TryRemove(key, out _);
                cleanupCount++;
            }

            if (cleanupCount > 0)
            {
                _logger.LogDebug("Cleaned up {Count} expired temporary variables", cleanupCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during variable cleanup");
        }
    }

    private void OnVariableChanged(VariableChangedEventArgs e)
    {
        try
        {
            VariableChanged?.Invoke(this, e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in variable changed event handler");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cleanupTimer?.Dispose();
            ClearVariables(VariableScope.Local);
            _disposed = true;
            
            _logger.LogDebug("VariableManager disposed");
        }
        
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets variable value without triggering Accessed event (for internal use)
    /// </summary>
    private object? GetVariableValueWithoutEvent(string name, VariableScope scope)
    {
        object? value = null;
        var found = false;

        switch (scope)
        {
            case VariableScope.Local:
                lock (_localLock)
                {
                    found = _localVariables.TryGetValue(name, out value);
                }
                break;
                
            case VariableScope.Global:
                found = _globalVariables.TryGetValue(name, out value);
                break;
                
            case VariableScope.Any:
                // Check local first, then global
                lock (_localLock)
                {
                    found = _localVariables.TryGetValue(name, out value);
                }
                if (!found)
                {
                    found = _globalVariables.TryGetValue(name, out value);
                }
                break;
        }

        return found ? value : null;
    }
}
