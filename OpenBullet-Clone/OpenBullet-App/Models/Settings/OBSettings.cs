using System.Collections.Generic;

namespace OpenBullet.Models.Settings
{
    public class OBSettings
    {
        public GeneralSettings General { get; set; } = new GeneralSettings();
        public SoundSettings Sounds { get; set; } = new SoundSettings();
        public SourceSettings Sources { get; set; } = new SourceSettings();
        public ThemeSettings Themes { get; set; } = new ThemeSettings();
    }
    
    public class GeneralSettings
    {
        public bool DisplayLoliScriptOnLoad { get; set; } = false;
        public bool RecommendedBots { get; set; } = true;
        public int StartingWidth { get; set; } = 800;
        public int StartingHeight { get; set; } = 620;
        public bool ChangeRunnerInterface { get; set; } = false;
        public bool DisableQuitWarning { get; set; } = false;
        public bool DisableNotSavedWarning { get; set; } = false;
        public string DefaultAuthor { get; set; } = "";
        public bool LiveConfigUpdates { get; set; } = false;
        public bool DisableHTMLView { get; set; } = false;
        public bool AlwaysOnTop { get; set; } = false;
        public bool AutoCreateRunner { get; set; } = false;
        public bool PersistDebuggerLog { get; set; } = false;
        public bool DisableSyntaxHelper { get; set; } = false;
        public bool DisplayCapturesLast { get; set; } = false;
        public bool DisableCopyPasteBlocks { get; set; } = false;
        public bool EnableLogging { get; set; } = false;
        public bool LogToFile { get; set; } = false;
        public int LogBufferSize { get; set; } = 10000;
        public bool BackupDB { get; set; } = true;
        public bool DisableRepo { get; set; } = false;
    }
    
    public class SoundSettings
    {
        public bool EnableSounds { get; set; } = false;
        public string OnHitSound { get; set; } = "rifle_hit.wav";
        public string OnReloadSound { get; set; } = "rifle_reload.wav";
    }
    
    public class SourceSettings
    {
        public List<string> Sources { get; set; } = new List<string>();
    }
    
    public class ThemeSettings
    {
        public string BackgroundMain { get; set; } = "#222";
        public string BackgroundSecondary { get; set; } = "#111";
        public string ForegroundMain { get; set; } = "#dcdcdc";
        public string ForegroundGood { get; set; } = "#adff2f";
        public string ForegroundBad { get; set; } = "#ff6347";
        public string ForegroundCustom { get; set; } = "#ff8c00";
        public string ForegroundRetry { get; set; } = "#ffff00";
        public string ForegroundToCheck { get; set; } = "#7fffd4";
        public string ForegroundMenuSelected { get; set; } = "#1e90ff";
        public bool UseImage { get; set; } = false;
        public string BackgroundImage { get; set; } = "";
        public int BackgroundImageOpacity { get; set; } = 100;
        public string BackgroundLogo { get; set; } = "";
        public bool EnableSnow { get; set; } = false;
        public int SnowAmount { get; set; } = 100;
        public bool AllowTransparency { get; set; } = false;
    }
}


