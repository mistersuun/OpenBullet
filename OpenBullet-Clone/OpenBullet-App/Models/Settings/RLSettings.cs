using System.Collections.Generic;

namespace OpenBullet.Models.Settings
{
    public class RLSettings
    {
        public GeneralRLSettings General { get; set; } = new GeneralRLSettings();
        public ProxySettings Proxies { get; set; } = new ProxySettings();
        public CaptchaSettings Captchas { get; set; } = new CaptchaSettings();
        public SeleniumSettings Selenium { get; set; } = new SeleniumSettings();
    }
    
    public class GeneralRLSettings
    {
        public int WaitTime { get; set; } = 100;
        public int RequestTimeout { get; set; } = 10;
        public int MaxHits { get; set; } = 0;
        public int BotsDisplayMode { get; set; } = 1;
        public bool EnableBotLog { get; set; } = false;
        public bool SaveLastSource { get; set; } = false;
        public bool WebhookEnabled { get; set; } = false;
        public string WebhookURL { get; set; } = "";
        public string WebhookUser { get; set; } = "Undefined";
    }
    
    public class ProxySettings
    {
        public bool ConcurrentUse { get; set; } = false;
        public bool NeverBan { get; set; } = false;
        public int BanLoopEvasion { get; set; } = 100;
        public bool ShuffleOnStart { get; set; } = false;
        public bool Reload { get; set; } = true;
        public int ReloadSource { get; set; } = 0;
        public string ReloadPath { get; set; } = "";
        public int ReloadType { get; set; } = 0;
        public int ReloadInterval { get; set; } = 0;
        public bool AlwaysGetClearance { get; set; } = false;
        public List<string> GlobalBanKeys { get; set; } = new List<string>();
        public List<string> GlobalRetryKeys { get; set; } = new List<string>();
        public List<string> RemoteProxySources { get; set; } = new List<string>();
    }
    
    public class CaptchaSettings
    {
        public int CurrentService { get; set; } = 0;
        public string AntiCapToken { get; set; } = "";
        public string ImageTypToken { get; set; } = "";
        public string DBCUser { get; set; } = "";
        public string DBCPass { get; set; } = "";
        public string TwoCapToken { get; set; } = "";
        public string RuCapToken { get; set; } = "";
        public string DCUser { get; set; } = "";
        public string DCPass { get; set; } = "";
        public string AZCapToken { get; set; } = "";
        public string SRUserId { get; set; } = "";
        public string SRToken { get; set; } = "";
        public string CIOToken { get; set; } = "";
        public string CDToken { get; set; } = "";
        public string CustomTwoCapToken { get; set; } = "";
        public string CustomTwoCapDomain { get; set; } = "example.com";
        public int CustomTwoCapPort { get; set; } = 80;
        public bool BypassBalanceCheck { get; set; } = false;
        public int Timeout { get; set; } = 120;
    }
    
    public class SeleniumSettings
    {
        public int Browser { get; set; } = 0;
        public bool Headless { get; set; } = false;
        public string FirefoxBinaryLocation { get; set; } = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
        public string ChromeBinaryLocation { get; set; } = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        public List<string> ChromeExtensions { get; set; } = new List<string>();
        public bool DrawMouseMovement { get; set; } = true;
        public int PageLoadTimeout { get; set; } = 60;
    }
}


