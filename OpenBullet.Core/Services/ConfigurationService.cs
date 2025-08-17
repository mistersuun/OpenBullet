using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenBullet.Core.Models;

namespace OpenBullet.Core.Services;

/// <summary>
/// Configuration service implementation for .anom file management
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Regex _sectionRegex;

    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _sectionRegex = new Regex(@"^\[([^\]]+)\]$", RegexOptions.Compiled);
    }

    public async Task<ConfigModel> LoadConfigAsync(string filePath)
    {
        _logger.LogDebug("Loading configuration from {FilePath}", filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Configuration file not found: {filePath}");
        }

        if (!IsValidConfigFile(filePath))
        {
            throw new InvalidOperationException($"File is not a valid .anom configuration: {filePath}");
        }

        try
        {
            var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            var config = ParseConfigContent(content, Path.GetFileName(filePath));
            
            // Set file metadata
            var fileInfo = new FileInfo(filePath);
            config.LastModified = fileInfo.LastWriteTime;

            _logger.LogDebug("Successfully loaded configuration '{Name}' by {Author}", config.Name, config.Author);
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration from {FilePath}", filePath);
            throw;
        }
    }

    public async Task SaveConfigAsync(ConfigModel config, string filePath)
    {
        _logger.LogDebug("Saving configuration '{Name}' to {FilePath}", config.Name, filePath);

        try
        {
            var content = SerializeConfig(config);
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
            
            // Update last modified time
            config.LastModified = DateTime.UtcNow;

            _logger.LogDebug("Successfully saved configuration to {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save configuration to {FilePath}", filePath);
            throw;
        }
    }

    public async Task<ConfigValidationResult> ValidateConfigAsync(string filePath)
    {
        var result = new ConfigValidationResult();

        try
        {
            if (!File.Exists(filePath))
            {
                result.Errors.Add(new ConfigError
                {
                    Type = ConfigErrorType.FileNotFound,
                    Message = $"Configuration file not found: {filePath}"
                });
                return result;
            }

            var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            result.ParsedConfig = ValidateAndParseContent(content, result);
            result.IsValid = result.Errors.Count == 0;

            _logger.LogDebug("Validation completed for {FilePath}: {IsValid} (Errors: {ErrorCount}, Warnings: {WarningCount})", 
                filePath, result.IsValid, result.Errors.Count, result.Warnings.Count);
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ConfigError
            {
                Type = ConfigErrorType.ParseError,
                Message = $"Unexpected error during validation: {ex.Message}"
            });
            _logger.LogError(ex, "Validation failed for {FilePath}", filePath);
        }

        return result;
    }

    public async Task<IEnumerable<ConfigInfo>> GetConfigsFromDirectoryAsync(string directoryPath)
    {
        _logger.LogDebug("Scanning directory for configurations: {DirectoryPath}", directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            return Enumerable.Empty<ConfigInfo>();
        }

        var configs = new List<ConfigInfo>();
        var anomFiles = Directory.GetFiles(directoryPath, "*.anom", SearchOption.AllDirectories);

        foreach (var filePath in anomFiles)
        {
            try
            {
                var configInfo = await CreateConfigInfoAsync(filePath);
                configs.Add(configInfo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process config file {FilePath}", filePath);
                
                // Add invalid config info
                configs.Add(new ConfigInfo
                {
                    FilePath = filePath,
                    Name = Path.GetFileNameWithoutExtension(filePath),
                    IsValid = false,
                    LastModified = File.GetLastWriteTime(filePath),
                    FileSize = new FileInfo(filePath).Length
                });
            }
        }

        _logger.LogDebug("Found {Count} configuration files in {DirectoryPath}", configs.Count, directoryPath);
        return configs;
    }

    public bool IsValidConfigFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        if (!File.Exists(filePath))
            return false;

        if (!Path.GetExtension(filePath).Equals(".anom", StringComparison.OrdinalIgnoreCase))
            return false;

        try
        {
            // Quick validation - check if file contains required sections
            var content = File.ReadAllText(filePath);
            return content.Contains("[SETTINGS]") && content.Contains("[SCRIPT]");
        }
        catch
        {
            return false;
        }
    }

    public ConfigModel ParseConfigContent(string content, string? fileName = null)
    {
        var config = new ConfigModel();
        
        if (!string.IsNullOrEmpty(fileName))
        {
            config.Name = Path.GetFileNameWithoutExtension(fileName);
        }

        var sections = ParseSections(content);

        // Parse SETTINGS section
        if (sections.TryGetValue(ConfigSections.Settings, out var settingsContent))
        {
            try
            {
                var settingsJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsContent);
                if (settingsJson != null)
                {
                    ParseSettingsFromJson(config, settingsJson);
                }
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                throw new InvalidOperationException($"Invalid JSON in SETTINGS section: {ex.Message}", ex);
            }
        }

        // Parse SCRIPT section
        if (sections.TryGetValue(ConfigSections.Script, out var scriptContent))
        {
            config.Script = scriptContent.Trim();
        }

        return config;
    }

    public string SerializeConfig(ConfigModel config)
    {
        var sb = new StringBuilder();

        // SETTINGS section
        sb.AppendLine("[SETTINGS]");
        var settingsJson = CreateSettingsJson(config);
        var formattedJson = JsonConvert.SerializeObject(settingsJson, Formatting.Indented);
        sb.AppendLine(formattedJson);
        sb.AppendLine();

        // SCRIPT section
        sb.AppendLine("[SCRIPT]");
        sb.AppendLine(config.Script);

        return sb.ToString();
    }

    private Dictionary<string, string> ParseSections(string content)
    {
        var sections = new Dictionary<string, string>();
        var lines = content.Split('\n', StringSplitOptions.None);
        
        string? currentSection = null;
        var currentContent = new StringBuilder();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            var sectionMatch = _sectionRegex.Match(trimmedLine);

            if (sectionMatch.Success)
            {
                // Save previous section
                if (currentSection != null)
                {
                    sections[currentSection] = currentContent.ToString().Trim();
                }

                // Start new section
                currentSection = sectionMatch.Groups[1].Value;
                currentContent.Clear();
            }
            else if (currentSection != null)
            {
                currentContent.AppendLine(line);
            }
        }

        // Save last section
        if (currentSection != null)
        {
            sections[currentSection] = currentContent.ToString().Trim();
        }

        // Normalize line endings in SCRIPT section to use \n consistently
        if (sections.ContainsKey("SCRIPT"))
        {
            sections["SCRIPT"] = sections["SCRIPT"].Replace("\r\n", "\n");
        }

        return sections;
    }

    private void ParseSettingsFromJson(ConfigModel config, Dictionary<string, object> settingsJson)
    {
        // Basic config properties
        if (settingsJson.TryGetValue("Name", out var name))
            config.Name = name.ToString() ?? string.Empty;

        if (settingsJson.TryGetValue("Author", out var author))
            config.Author = author.ToString() ?? string.Empty;

        if (settingsJson.TryGetValue("Version", out var version))
            config.Version = version.ToString() ?? string.Empty;

        if (settingsJson.TryGetValue("AdditionalInfo", out var info))
            config.AdditionalInfo = info.ToString() ?? string.Empty;

        if (settingsJson.TryGetValue("LastModified", out var lastModified) && 
            DateTime.TryParse(lastModified.ToString(), out var parsedDate))
        {
            config.LastModified = parsedDate;
        }

        // Parse settings
        var settings = config.Settings;
        
        if (settingsJson.TryGetValue("SuggestedBots", out var suggestedBots) && 
            int.TryParse(suggestedBots.ToString(), out var botsInt))
            settings.SuggestedBots = botsInt;

        if (settingsJson.TryGetValue("MaxCPM", out var maxCpm) && 
            int.TryParse(maxCpm.ToString(), out var cpmInt))
            settings.MaxCPM = cpmInt;

        if (settingsJson.TryGetValue("IgnoreResponseErrors", out var ignoreErrors) && 
            bool.TryParse(ignoreErrors.ToString(), out var ignoreErrorsBool))
            settings.IgnoreResponseErrors = ignoreErrorsBool;

        if (settingsJson.TryGetValue("MaxRedirects", out var maxRedirects) && 
            int.TryParse(maxRedirects.ToString(), out var redirectsInt))
            settings.MaxRedirects = redirectsInt;

        if (settingsJson.TryGetValue("NeedsProxies", out var needsProxies) && 
            bool.TryParse(needsProxies.ToString(), out var needsProxiesBool))
            settings.NeedsProxies = needsProxiesBool;

        if (settingsJson.TryGetValue("OnlySocks", out var onlySocks) && 
            bool.TryParse(onlySocks.ToString(), out var onlySocksBool))
            settings.OnlySocks = onlySocksBool;

        if (settingsJson.TryGetValue("OnlySsl", out var onlySsl) && 
            bool.TryParse(onlySsl.ToString(), out var onlySslBool))
            settings.OnlySsl = onlySslBool;

        // Parse arrays
        if (settingsJson.TryGetValue("DataRules", out var dataRules) && dataRules is Newtonsoft.Json.Linq.JArray rulesArray)
        {
            settings.DataRules = rulesArray.ToObject<List<string>>() ?? new List<string>();
        }

        if (settingsJson.TryGetValue("CustomInputs", out var customInputs) && customInputs is Newtonsoft.Json.Linq.JArray inputsArray)
        {
            settings.CustomInputs = inputsArray.ToObject<List<string>>() ?? new List<string>();
        }

        // Parse other boolean and numeric settings
        ParseBooleanSetting(settingsJson, "EncodeData", val => settings.EncodeData = val);
        ParseBooleanSetting(settingsJson, "Base64", val => settings.Base64 = val);
        ParseBooleanSetting(settingsJson, "Grayscale", val => settings.Grayscale = val);
        ParseBooleanSetting(settingsJson, "RemoveLines", val => settings.RemoveLines = val);
        ParseBooleanSetting(settingsJson, "RemoveNoise", val => settings.RemoveNoise = val);
        ParseBooleanSetting(settingsJson, "Dilate", val => settings.Dilate = val);
        ParseBooleanSetting(settingsJson, "Transparent", val => settings.Transparent = val);
        ParseBooleanSetting(settingsJson, "OnlyShow", val => settings.OnlyShow = val);
        ParseBooleanSetting(settingsJson, "ContrastGamma", val => settings.ContrastGamma = val);
        ParseBooleanSetting(settingsJson, "ForceHeadless", val => settings.ForceHeadless = val);
        ParseBooleanSetting(settingsJson, "AlwaysOpen", val => settings.AlwaysOpen = val);
        ParseBooleanSetting(settingsJson, "AlwaysQuit", val => settings.AlwaysQuit = val);
        ParseBooleanSetting(settingsJson, "DisableNotifications", val => settings.DisableNotifications = val);
        ParseBooleanSetting(settingsJson, "RandomUA", val => settings.RandomUA = val);
        ParseBooleanSetting(settingsJson, "LoliSave", val => settings.LoliSave = val);

        ParseDoubleSetting(settingsJson, "Threshold", val => settings.Threshold = val);
        ParseDoubleSetting(settingsJson, "DiffKeep", val => settings.DiffKeep = val);
        ParseDoubleSetting(settingsJson, "DiffHide", val => settings.DiffHide = val);
        ParseDoubleSetting(settingsJson, "Contrast", val => settings.Contrast = val);
        ParseDoubleSetting(settingsJson, "Gamma", val => settings.Gamma = val);
        ParseDoubleSetting(settingsJson, "Brightness", val => settings.Brightness = val);

        ParseIntSetting(settingsJson, "MaxProxyUses", val => settings.MaxProxyUses = val);
        ParseIntSetting(settingsJson, "RemoveLinesMin", val => settings.RemoveLinesMin = val);
        ParseIntSetting(settingsJson, "RemoveLinesMax", val => settings.RemoveLinesMax = val);

        ParseStringSetting(settingsJson, "AllowedWordlist1", val => settings.AllowedWordlist1 = val);
        ParseStringSetting(settingsJson, "AllowedWordlist2", val => settings.AllowedWordlist2 = val);
        ParseStringSetting(settingsJson, "CaptchaUrl", val => settings.CaptchaUrl = val);
        ParseStringSetting(settingsJson, "CustomUserAgent", val => settings.CustomUserAgent = val);
        ParseStringSetting(settingsJson, "CustomCMDArgs", val => settings.CustomCMDArgs = val);
    }

    private static void ParseBooleanSetting(Dictionary<string, object> json, string key, Action<bool> setter)
    {
        if (json.TryGetValue(key, out var value) && bool.TryParse(value.ToString(), out var parsed))
        {
            setter(parsed);
        }
    }

    private static void ParseDoubleSetting(Dictionary<string, object> json, string key, Action<double> setter)
    {
        if (json.TryGetValue(key, out var value) && double.TryParse(value.ToString(), out var parsed))
        {
            setter(parsed);
        }
    }

    private static void ParseIntSetting(Dictionary<string, object> json, string key, Action<int> setter)
    {
        if (json.TryGetValue(key, out var value) && int.TryParse(value.ToString(), out var parsed))
        {
            setter(parsed);
        }
    }

    private static void ParseStringSetting(Dictionary<string, object> json, string key, Action<string> setter)
    {
        if (json.TryGetValue(key, out var value))
        {
            setter(value.ToString() ?? string.Empty);
        }
    }

    private Dictionary<string, object> CreateSettingsJson(ConfigModel config)
    {
        var settings = new Dictionary<string, object>
        {
            ["Name"] = config.Name,
            ["Author"] = config.Author,
            ["Version"] = config.Version,
            ["LastModified"] = config.LastModified.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"),
            ["AdditionalInfo"] = config.AdditionalInfo,
            
            // Core settings
            ["SuggestedBots"] = config.Settings.SuggestedBots,
            ["MaxCPM"] = config.Settings.MaxCPM,
            ["IgnoreResponseErrors"] = config.Settings.IgnoreResponseErrors,
            ["MaxRedirects"] = config.Settings.MaxRedirects,
            ["NeedsProxies"] = config.Settings.NeedsProxies,
            ["OnlySocks"] = config.Settings.OnlySocks,
            ["OnlySsl"] = config.Settings.OnlySsl,
            ["MaxProxyUses"] = config.Settings.MaxProxyUses,
            ["EncodeData"] = config.Settings.EncodeData,
            ["AllowedWordlist1"] = config.Settings.AllowedWordlist1,
            ["AllowedWordlist2"] = config.Settings.AllowedWordlist2,
            ["DataRules"] = config.Settings.DataRules,
            ["CustomInputs"] = config.Settings.CustomInputs,
            
            // Captcha settings
            ["CaptchaUrl"] = config.Settings.CaptchaUrl,
            ["Base64"] = config.Settings.Base64,
            ["Grayscale"] = config.Settings.Grayscale,
            ["RemoveLines"] = config.Settings.RemoveLines,
            ["RemoveNoise"] = config.Settings.RemoveNoise,
            ["Dilate"] = config.Settings.Dilate,
            ["Threshold"] = config.Settings.Threshold,
            ["DiffKeep"] = config.Settings.DiffKeep,
            ["DiffHide"] = config.Settings.DiffHide,
            ["Transparent"] = config.Settings.Transparent,
            ["OnlyShow"] = config.Settings.OnlyShow,
            ["ContrastGamma"] = config.Settings.ContrastGamma,
            ["Contrast"] = config.Settings.Contrast,
            ["Gamma"] = config.Settings.Gamma,
            ["Brightness"] = config.Settings.Brightness,
            ["RemoveLinesMin"] = config.Settings.RemoveLinesMin,
            ["RemoveLinesMax"] = config.Settings.RemoveLinesMax,
            
            // Browser settings
            ["ForceHeadless"] = config.Settings.ForceHeadless,
            ["AlwaysOpen"] = config.Settings.AlwaysOpen,
            ["AlwaysQuit"] = config.Settings.AlwaysQuit,
            ["DisableNotifications"] = config.Settings.DisableNotifications,
            ["CustomUserAgent"] = config.Settings.CustomUserAgent,
            ["RandomUA"] = config.Settings.RandomUA,
            ["CustomCMDArgs"] = config.Settings.CustomCMDArgs,
            
            // Legacy
            ["LoliSave"] = config.Settings.LoliSave
        };

        return settings;
    }

    private ConfigModel ValidateAndParseContent(string content, ConfigValidationResult result)
    {
        try
        {
            var config = ParseConfigContent(content);

            // Validate required sections
            if (!content.Contains("[SETTINGS]"))
            {
                result.Errors.Add(new ConfigError
                {
                    Type = ConfigErrorType.MissingSection,
                    Section = "SETTINGS",
                    Message = "Missing required [SETTINGS] section"
                });
            }

            if (!content.Contains("[SCRIPT]"))
            {
                result.Errors.Add(new ConfigError
                {
                    Type = ConfigErrorType.MissingSection,
                    Section = "SCRIPT",
                    Message = "Missing required [SCRIPT] section"
                });
            }

            // Validate script content
            if (string.IsNullOrWhiteSpace(config.Script))
            {
                result.Warnings.Add(new ConfigWarning
                {
                    Type = ConfigWarningType.SuggestedImprovement,
                    Section = "SCRIPT",
                    Message = "Script section is empty"
                });
            }

            // Validate settings
            if (config.Settings.SuggestedBots <= 0)
            {
                result.Warnings.Add(new ConfigWarning
                {
                    Type = ConfigWarningType.SuggestedImprovement,
                    Section = "SETTINGS",
                    Message = "SuggestedBots should be greater than 0"
                });
            }

            return config;
        }
        catch (Exception ex)
        {
            result.Errors.Add(new ConfigError
            {
                Type = ConfigErrorType.ParseError,
                Message = $"Failed to parse configuration: {ex.Message}"
            });
            return new ConfigModel();
        }
    }

    private async Task<ConfigInfo> CreateConfigInfoAsync(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var configInfo = new ConfigInfo
        {
            FilePath = filePath,
            LastModified = fileInfo.LastWriteTime,
            FileSize = fileInfo.Length
        };

        try
        {
            var validation = await ValidateConfigAsync(filePath);
            configInfo.IsValid = validation.IsValid;

            if (validation.ParsedConfig != null)
            {
                configInfo.Name = validation.ParsedConfig.Name;
                configInfo.Author = validation.ParsedConfig.Author;
                configInfo.Version = validation.ParsedConfig.Version;
            }
            else
            {
                configInfo.Name = Path.GetFileNameWithoutExtension(filePath);
            }
        }
        catch
        {
            configInfo.IsValid = false;
            configInfo.Name = Path.GetFileNameWithoutExtension(filePath);
        }

        return configInfo;
    }
}
