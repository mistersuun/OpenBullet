using OpenBullet.Core.Models;

namespace OpenBullet.Core.Services;

/// <summary>
/// Interface for configuration file management
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Loads a configuration from a .anom file
    /// </summary>
    Task<ConfigModel> LoadConfigAsync(string filePath);

    /// <summary>
    /// Saves a configuration to a .anom file
    /// </summary>
    Task SaveConfigAsync(ConfigModel config, string filePath);

    /// <summary>
    /// Validates a configuration file
    /// </summary>
    Task<ConfigValidationResult> ValidateConfigAsync(string filePath);

    /// <summary>
    /// Gets all configurations from a directory
    /// </summary>
    Task<IEnumerable<ConfigInfo>> GetConfigsFromDirectoryAsync(string directoryPath);

    /// <summary>
    /// Checks if a file is a valid .anom configuration
    /// </summary>
    bool IsValidConfigFile(string filePath);

    /// <summary>
    /// Parses configuration content from string
    /// </summary>
    ConfigModel ParseConfigContent(string content, string? fileName = null);

    /// <summary>
    /// Converts configuration to .anom format string
    /// </summary>
    string SerializeConfig(ConfigModel config);
}

/// <summary>
/// Configuration validation result
/// </summary>
public class ConfigValidationResult
{
    public bool IsValid { get; set; }
    public List<ConfigError> Errors { get; set; } = new();
    public List<ConfigWarning> Warnings { get; set; } = new();
    public ConfigModel? ParsedConfig { get; set; }
}

/// <summary>
/// Configuration error information
/// </summary>
public class ConfigError
{
    public string Message { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string Section { get; set; } = string.Empty;
    public ConfigErrorType Type { get; set; }
}

/// <summary>
/// Configuration warning information
/// </summary>
public class ConfigWarning
{
    public string Message { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string Section { get; set; } = string.Empty;
    public ConfigWarningType Type { get; set; }
}

/// <summary>
/// Configuration error types
/// </summary>
public enum ConfigErrorType
{
    ParseError,
    InvalidJson,
    MissingSection,
    InvalidScript,
    FileNotFound,
    AccessDenied
}

/// <summary>
/// Configuration warning types
/// </summary>
public enum ConfigWarningType
{
    DeprecatedSetting,
    UnknownSetting,
    SuggestedImprovement,
    PerformanceWarning
}

/// <summary>
/// Configuration file information
/// </summary>
public class ConfigInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public long FileSize { get; set; }
    public bool IsValid { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Configuration file sections
/// </summary>
public static class ConfigSections
{
    public const string Settings = "SETTINGS";
    public const string Script = "SCRIPT";
}

/// <summary>
/// Configuration parsing options
/// </summary>
public class ConfigParsingOptions
{
    public bool ValidateScript { get; set; } = true;
    public bool ValidateSettings { get; set; } = true;
    public bool LoadMetadata { get; set; } = true;
    public bool StrictMode { get; set; } = false;
    public List<string> AllowedSections { get; set; } = new() { ConfigSections.Settings, ConfigSections.Script };
}
