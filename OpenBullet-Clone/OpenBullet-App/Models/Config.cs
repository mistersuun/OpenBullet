using System;
using System.Collections.Generic;

namespace OpenBullet.Models
{
    public class Config
    {
        public ConfigSettings Settings { get; set; } = new ConfigSettings();
        public string Script { get; set; } = "";
        public List<CustomInput> CustomInputs { get; set; } = new List<CustomInput>();
        public List<DataRule> DataRules { get; set; } = new List<DataRule>();
    }
    
    public class ConfigSettings
    {
        public string Name { get; set; } = "";
        public int SuggestedBots { get; set; } = 1;
        public int MaxCPM { get; set; } = 0;
        public DateTime LastModified { get; set; } = DateTime.Now;
        public string AdditionalInfo { get; set; } = "";
        public string Author { get; set; } = "";
        public string Version { get; set; } = "1.0";
        public bool LoliSave { get; set; } = false;
        public bool IgnoreResponseErrors { get; set; } = false;
        public int MaxRedirects { get; set; } = 8;
        public bool NeedsProxies { get; set; } = false;
        public bool OnlySocks { get; set; } = false;
        public bool OnlySsl { get; set; } = false;
        public int MaxProxyUses { get; set; } = 0;
        public bool EncodeData { get; set; } = false;
        public string AllowedWordlist1 { get; set; } = "";
        public string AllowedWordlist2 { get; set; } = "";
        public List<DataRule> DataRules { get; set; } = new List<DataRule>();
        public List<CustomInput> CustomInputs { get; set; } = new List<CustomInput>();
        public string CaptchaUrl { get; set; } = "";
        public string Base64 { get; set; } = "";
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
        public bool ForceHeadless { get; set; } = false;
        public bool AlwaysOpen { get; set; } = false;
        public bool AlwaysQuit { get; set; } = false;
        public bool DisableNotifications { get; set; } = false;
        public string CustomUserAgent { get; set; } = "";
        public bool RandomUA { get; set; } = false;
        public string CustomCMDArgs { get; set; } = "";
    }
    
    public class CustomInput
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string DefaultValue { get; set; } = "";
        public bool IsRequired { get; set; } = false;
    }
    
    public class DataRule
    {
        public string Name { get; set; } = "";
        public string Pattern { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}


