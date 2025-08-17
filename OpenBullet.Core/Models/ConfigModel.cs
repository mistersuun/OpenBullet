namespace OpenBullet.Core.Models;

/// <summary>
/// Configuration model representing a .anom file
/// </summary>
public class ConfigModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string AdditionalInfo { get; set; } = string.Empty;
    
    // Script content
    public string Script { get; set; } = string.Empty;
    
    // Settings from [SETTINGS] section
    public ConfigSettings Settings { get; set; } = new();
}

/// <summary>
/// Configuration settings from the [SETTINGS] section
/// </summary>
public class ConfigSettings
{
    public int SuggestedBots { get; set; } = 1;
    public int MaxCPM { get; set; } = 0;
    public bool IgnoreResponseErrors { get; set; } = false;
    public int MaxRedirects { get; set; } = 8;
    public bool NeedsProxies { get; set; } = false;
    public bool OnlySocks { get; set; } = false;
    public bool OnlySsl { get; set; } = false;
    public int MaxProxyUses { get; set; } = 0;
    public bool EncodeData { get; set; } = false;
    public string AllowedWordlist1 { get; set; } = string.Empty;
    public string AllowedWordlist2 { get; set; } = string.Empty;
    public List<string> DataRules { get; set; } = new();
    public List<string> CustomInputs { get; set; } = new();
    
    // Captcha settings
    public string CaptchaUrl { get; set; } = string.Empty;
    public bool Base64 { get; set; } = false;
    public bool Grayscale { get; set; } = false;
    public bool RemoveLines { get; set; } = false;
    public bool RemoveNoise { get; set; } = false;
    public bool Dilate { get; set; } = false;
    public double Threshold { get; set; } = 1.0;
    public double DiffKeep { get; set; } = 0.0;
    public double DiffHide { get; set; } = 0.0;
    public bool Transparent { get; set; } = false;
    public bool OnlyShow { get; set; } = false;
    public bool ContrastGamma { get; set; } = false;
    public double Contrast { get; set; } = 1.0;
    public double Gamma { get; set; } = 1.0;
    public double Brightness { get; set; } = 1.0;
    public int RemoveLinesMin { get; set; } = 0;
    public int RemoveLinesMax { get; set; } = 0;
    
    // Browser settings
    public bool ForceHeadless { get; set; } = false;
    public bool AlwaysOpen { get; set; } = false;
    public bool AlwaysQuit { get; set; } = false;
    public bool DisableNotifications { get; set; } = false;
    public string CustomUserAgent { get; set; } = string.Empty;
    public bool RandomUA { get; set; } = false;
    public string CustomCMDArgs { get; set; } = string.Empty;
    
    // Legacy
    public bool LoliSave { get; set; } = false;
}
