using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenBullet.Core.Models;

namespace OpenBullet.Core.Data;

/// <summary>
/// Base entity with common properties
/// </summary>
public abstract class BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    
    [NotMapped]
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    [Column("MetadataJson")]
    public string MetadataJson
    {
        get => Newtonsoft.Json.JsonConvert.SerializeObject(Metadata);
        set => Metadata = string.IsNullOrEmpty(value) 
            ? new Dictionary<string, object>() 
            : Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(value) ?? new();
    }
}

/// <summary>
/// Configuration entity for storing OpenBullet configurations
/// </summary>
[Table("Configurations")]
public class ConfigurationEntity : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string Script { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = "General";

    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Version { get; set; } = "1.0.0";

    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = false;

    [Column("SettingsJson")]
    public string? SettingsJson { get; set; }

    [Column("RequiredPluginsJson")]
    public string? RequiredPluginsJson { get; set; }

    [Column("TagsJson")]
    public string? TagsJson { get; set; }

    public int UsageCount { get; set; } = 0;
    public DateTime? LastUsed { get; set; }
    public double? SuccessRate { get; set; }

    // Navigation properties
    public virtual ICollection<JobEntity> Jobs { get; set; } = new List<JobEntity>();

    // Helper methods
    public ConfigModel ToConfigModel()
    {
        var config = new ConfigModel
        {
            Name = Name,
            Script = Script,
            Author = Author,
            Version = Version
        };

        if (!string.IsNullOrEmpty(SettingsJson))
        {
            config.Settings = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigSettings>(SettingsJson);
        }

        return config;
    }

    public void FromConfigModel(ConfigModel config)
    {
        Name = config.Name;
        Script = config.Script;
        Author = config.Author ?? string.Empty;
        Version = config.Version ?? "1.0.0";
        SettingsJson = config.Settings != null 
            ? Newtonsoft.Json.JsonConvert.SerializeObject(config.Settings) 
            : null;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Job entity for storing job execution information
/// </summary>
[Table("Jobs")]
public class JobEntity : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string ConfigurationId { get; set; } = string.Empty;

    public JobStatus Status { get; set; } = JobStatus.Created;

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration { get; set; }

    public int TotalItems { get; set; }
    public int ProcessedItems { get; set; }
    public int SuccessfulItems { get; set; }
    public int FailedItems { get; set; }
    public int ErrorItems { get; set; }

    public int ConcurrentBots { get; set; } = 1;
    public bool UseProxies { get; set; } = false;
    public int ProxiesUsed { get; set; } = 0;

    [Column("DataLinesJson")]
    public string? DataLinesJson { get; set; }

    [Column("SettingsJson")]
    public string? SettingsJson { get; set; }

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    public double? SuccessRate { get; set; }
    public double? ItemsPerMinute { get; set; }

    // Navigation properties
    public virtual ConfigurationEntity? Configuration { get; set; }
    public virtual ICollection<JobResultEntity> Results { get; set; } = new List<JobResultEntity>();

    // Helper properties
    [NotMapped]
    public double ProgressPercentage => TotalItems > 0 ? (double)ProcessedItems / TotalItems * 100 : 0;

    [NotMapped]
    public bool IsRunning => Status == JobStatus.Running || Status == JobStatus.Starting;

    [NotMapped]
    public bool IsCompleted => Status == JobStatus.Completed || Status == JobStatus.Failed || Status == JobStatus.Cancelled;
}

/// <summary>
/// Job result entity for storing individual bot execution results
/// </summary>
[Table("JobResults")]
public class JobResultEntity : BaseEntity
{
    [Required]
    public string JobId { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string DataLine { get; set; } = string.Empty;

    public BotStatus Status { get; set; } = BotStatus.None;

    [MaxLength(100)]
    public string? CustomStatus { get; set; }

    public bool Success { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    [Column("VariablesJson")]
    public string? VariablesJson { get; set; }

    [Column("CapturedDataJson")]
    public string? CapturedDataJson { get; set; }

    [Column("LogsJson")]
    public string? LogsJson { get; set; }

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    [MaxLength(100)]
    public string? ProxyUsed { get; set; }

    public int? ResponseCode { get; set; }
    public long? ResponseTime { get; set; }

    [MaxLength(200)]
    public string? FinalUrl { get; set; }

    // Navigation properties
    public virtual JobEntity? Job { get; set; }

    // Helper methods
    [NotMapped]
    public Dictionary<string, object> Variables
    {
        get 
        {
            if (_variables == null)
            {
                _variables = string.IsNullOrEmpty(VariablesJson) 
                    ? new Dictionary<string, object>() 
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(VariablesJson) ?? new();
            }
            return _variables;
        }
        set 
        { 
            _variables = value;
            VariablesJson = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
    private Dictionary<string, object>? _variables;

    [NotMapped]
    public Dictionary<string, object> CapturedData
    {
        get 
        {
            if (_capturedData == null)
            {
                _capturedData = string.IsNullOrEmpty(CapturedDataJson) 
                    ? new Dictionary<string, object>() 
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(CapturedDataJson) ?? new();
            }
            return _capturedData;
        }
        set 
        { 
            _capturedData = value;
            CapturedDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
    private Dictionary<string, object>? _capturedData;

    [NotMapped]
    public List<string> Logs
    {
        get 
        {
            if (_logs == null)
            {
                _logs = string.IsNullOrEmpty(LogsJson) 
                    ? new List<string>() 
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(LogsJson) ?? new();
            }
            return _logs;
        }
        set 
        { 
            _logs = value;
            LogsJson = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
    private List<string>? _logs;

    // Method to sync changes made directly to dictionaries/lists
    public void SyncJsonFields()
    {
        if (_variables != null)
            VariablesJson = Newtonsoft.Json.JsonConvert.SerializeObject(_variables);
        if (_capturedData != null)
            CapturedDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(_capturedData);
        if (_logs != null)
            LogsJson = Newtonsoft.Json.JsonConvert.SerializeObject(_logs);
    }
}

/// <summary>
/// Proxy entity for storing proxy information
/// </summary>
[Table("Proxies")]
public class ProxyEntity : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public ProxyType Type { get; set; } = ProxyType.Http;

    [MaxLength(100)]
    public string? Username { get; set; }

    [MaxLength(100)]
    public string? Password { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsBanned { get; set; } = false;
    public DateTime? BannedUntil { get; set; }

    [MaxLength(500)]
    public string? BanReason { get; set; }

    public int Uses { get; set; } = 0;
    public int SuccessfulRequests { get; set; } = 0;
    public int FailedRequests { get; set; } = 0;

    public long AverageResponseTimeMs { get; set; } = 0;
    public DateTime? LastUsed { get; set; }
    public DateTime? LastTested { get; set; }

    public ProxyHealth Health { get; set; } = ProxyHealth.Unknown;

    [MaxLength(50)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? Source { get; set; }

    public double? SuccessRate { get; set; }

    // Helper properties
    [NotMapped]
    public string Address => $"{Host}:{Port}";

    [NotMapped]
    public int TotalRequests => SuccessfulRequests + FailedRequests;

    [NotMapped]
    public bool IsCurrentlyBanned => IsBanned && (BannedUntil == null || BannedUntil > DateTime.UtcNow);

    [NotMapped]
    public bool IsAvailable => IsActive && !IsCurrentlyBanned && Health != ProxyHealth.Dead;
}

/// <summary>
/// Setting entity for storing application settings
/// </summary>
[Table("Settings")]
public class SettingEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    public string? Value { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Category { get; set; } = "General";

    [MaxLength(50)]
    public string DataType { get; set; } = "string";

    public bool IsReadOnly { get; set; } = false;
    public bool IsEncrypted { get; set; } = false;

    [MaxLength(100)]
    public string? DefaultValue { get; set; }

    [Column("ValidationRulesJson")]
    public string? ValidationRulesJson { get; set; }

    // Helper methods
    public T? GetValue<T>()
    {
        // Handle null values
        if (Value == null)
            return default;

        try
        {
            if (typeof(T) == typeof(string))
                return (T)(object)Value; // Return empty string as-is for string type

            // For non-string types, treat empty string as default
            if (string.IsNullOrEmpty(Value))
                return default;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Value);
        }
        catch
        {
            return default;
        }
    }

    public void SetValue<T>(T value)
    {
        if (value == null)
        {
            Value = null;
            return;
        }

        if (typeof(T) == typeof(string))
        {
            Value = value.ToString();
        }
        else
        {
            Value = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Job status enumeration for database storage
/// </summary>
public enum JobStatus
{
    Created = 0,
    Starting = 1,
    Running = 2,
    Paused = 3,
    Stopping = 4,
    Completed = 5,
    Failed = 6,
    Cancelled = 7
}

/// <summary>
/// Proxy health enumeration for database storage
/// </summary>
public enum ProxyHealth
{
    Unknown = 0,
    Healthy = 1,
    Slow = 2,
    Unreliable = 3,
    Dead = 4
}
