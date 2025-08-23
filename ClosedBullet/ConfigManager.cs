using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ClosedBullet
{
    public class ConfigManager
    {
        public class AnomConfig
        {
            public string Name { get; set; }
            public string FilePath { get; set; }
            public string Author { get; set; }
            public string Version { get; set; }
            public DateTime LastModified { get; set; }
            
            // Request settings
            public string Method { get; set; } = "POST";
            public string Url { get; set; }
            public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
            public string PostData { get; set; }
            public bool FollowRedirects { get; set; } = true;
            public int Timeout { get; set; } = 30000;
            
            // Patterns
            public List<string> SuccessKeys { get; set; } = new List<string>();
            public List<string> FailureKeys { get; set; } = new List<string>();
            public List<string> BanKeys { get; set; } = new List<string>();
            public List<string> RetryKeys { get; set; } = new List<string>();
            public List<string> CustomKeys { get; set; } = new List<string>();
            
            // Capture patterns
            public Dictionary<string, string> CapturePatterns { get; set; } = new Dictionary<string, string>();
            
            // Script blocks
            public List<ScriptBlock> Scripts { get; set; } = new List<ScriptBlock>();
            
            public override string ToString()
            {
                return $"{Name} v{Version} by {Author}";
            }
        }
        
        public class ScriptBlock
        {
            public string Type { get; set; } // REQUEST, PARSE, KEYCHECK, etc.
            public string Content { get; set; }
            public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        }
        
        private List<AnomConfig> configs = new List<AnomConfig>();
        private AnomConfig activeConfig;
        
        public event EventHandler<string> ConfigStatusChanged;
        
        public List<AnomConfig> LoadedConfigs => configs;
        public AnomConfig ActiveConfig => activeConfig;
        
        // Load .anom configuration file
        public AnomConfig LoadConfig(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Config file not found: {filePath}");
                }
                
                var content = File.ReadAllText(filePath);
                var config = ParseAnomConfig(content);
                config.FilePath = filePath;
                config.Name = Path.GetFileNameWithoutExtension(filePath);
                config.LastModified = File.GetLastWriteTime(filePath);
                
                // Add to loaded configs if not already present
                var existing = configs.FirstOrDefault(c => c.FilePath == filePath);
                if (existing != null)
                {
                    configs.Remove(existing);
                }
                configs.Add(config);
                
                OnConfigStatusChanged($"Loaded config: {config.Name}");
                return config;
            }
            catch (Exception ex)
            {
                OnConfigStatusChanged($"Failed to load config: {ex.Message}");
                return null;
            }
        }
        
        // Parse .anom configuration content
        private AnomConfig ParseAnomConfig(string content)
        {
            var config = new AnomConfig();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            string currentSection = "";
            var scriptBlock = new ScriptBlock();
            bool inScriptBlock = false;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Skip comments
                if (trimmedLine.StartsWith("#") || trimmedLine.StartsWith("//"))
                    continue;
                
                // Detect sections
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2).ToUpper();
                    continue;
                }
                
                // Parse based on section
                switch (currentSection)
                {
                    case "SETTINGS":
                        ParseSettingsLine(config, trimmedLine);
                        break;
                        
                    case "SCRIPT":
                        ParseScriptLine(config, trimmedLine, ref scriptBlock, ref inScriptBlock);
                        break;
                        
                    case "REQUEST":
                        ParseRequestLine(config, trimmedLine);
                        break;
                        
                    case "HEADERS":
                        ParseHeaderLine(config, trimmedLine);
                        break;
                        
                    case "KEYS":
                    case "KEYCHECK":
                        ParseKeyCheckLine(config, trimmedLine);
                        break;
                        
                    case "CAPTURE":
                        ParseCaptureLine(config, trimmedLine);
                        break;
                }
            }
            
            return config;
        }
        
        private void ParseSettingsLine(AnomConfig config, string line)
        {
            if (line.Contains("="))
            {
                var parts = line.Split(new[] { '=' }, 2);
                var key = parts[0].Trim().ToUpper();
                var value = parts[1].Trim();
                
                switch (key)
                {
                    case "AUTHOR":
                        config.Author = value;
                        break;
                    case "VERSION":
                        config.Version = value;
                        break;
                    case "TIMEOUT":
                        int.TryParse(value, out int timeout);
                        config.Timeout = timeout;
                        break;
                    case "FOLLOWREDIRECTS":
                        config.FollowRedirects = value.ToLower() == "true";
                        break;
                }
            }
        }
        
        private void ParseRequestLine(AnomConfig config, string line)
        {
            if (line.StartsWith("METHOD", StringComparison.OrdinalIgnoreCase))
            {
                config.Method = ExtractValue(line, "METHOD");
            }
            else if (line.StartsWith("URL", StringComparison.OrdinalIgnoreCase))
            {
                config.Url = ExtractValue(line, "URL");
            }
            else if (line.StartsWith("POST", StringComparison.OrdinalIgnoreCase))
            {
                config.PostData = ExtractValue(line, "POST");
            }
        }
        
        private void ParseHeaderLine(AnomConfig config, string line)
        {
            if (line.Contains(":"))
            {
                var parts = line.Split(new[] { ':' }, 2);
                config.Headers[parts[0].Trim()] = parts[1].Trim();
            }
        }
        
        private void ParseKeyCheckLine(AnomConfig config, string line)
        {
            // Parse patterns like: KEYCHECK BanOn "captcha"
            var match = Regex.Match(line, @"KEYCHECK\s+(\w+)\s+""([^""]+)""", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var type = match.Groups[1].Value.ToUpper();
                var pattern = match.Groups[2].Value;
                
                switch (type)
                {
                    case "SUCCESSON":
                    case "SUCCESS":
                        config.SuccessKeys.Add(pattern);
                        break;
                    case "FAILUREON":
                    case "FAILURE":
                    case "FAIL":
                        config.FailureKeys.Add(pattern);
                        break;
                    case "BANON":
                    case "BAN":
                        config.BanKeys.Add(pattern);
                        break;
                    case "RETRYON":
                    case "RETRY":
                        config.RetryKeys.Add(pattern);
                        break;
                    case "CUSTOM":
                        config.CustomKeys.Add(pattern);
                        break;
                }
            }
        }
        
        private void ParseCaptureLine(AnomConfig config, string line)
        {
            // Parse patterns like: CAPTURE email BETWEEN "email":" AND "
            var match = Regex.Match(line, @"CAPTURE\s+(\w+)\s+BETWEEN\s+""([^""]+)""\s+AND\s+""([^""]+)""", 
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var name = match.Groups[1].Value;
                var start = match.Groups[2].Value;
                var end = match.Groups[3].Value;
                config.CapturePatterns[name] = $"{start}|{end}";
            }
        }
        
        private void ParseScriptLine(AnomConfig config, string line, ref ScriptBlock scriptBlock, ref bool inScriptBlock)
        {
            // Handle script blocks
            if (line.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase))
            {
                inScriptBlock = true;
                scriptBlock = new ScriptBlock();
                scriptBlock.Type = ExtractValue(line, "BEGIN");
            }
            else if (line.StartsWith("END", StringComparison.OrdinalIgnoreCase))
            {
                if (inScriptBlock)
                {
                    config.Scripts.Add(scriptBlock);
                    inScriptBlock = false;
                }
            }
            else if (inScriptBlock)
            {
                scriptBlock.Content += line + Environment.NewLine;
            }
        }
        
        private string ExtractValue(string line, string key)
        {
            var pattern = $@"{key}\s+(.+)";
            var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim().Trim('"') : "";
        }
        
        // Set active configuration
        public void SetActiveConfig(AnomConfig config)
        {
            activeConfig = config;
            OnConfigStatusChanged($"Active config: {config.Name}");
        }
        
        // Save configuration to file
        public void SaveConfig(AnomConfig config, string filePath = null)
        {
            try
            {
                filePath = filePath ?? config.FilePath;
                
                var content = GenerateAnomContent(config);
                File.WriteAllText(filePath, content);
                
                config.FilePath = filePath;
                config.LastModified = DateTime.Now;
                
                OnConfigStatusChanged($"Config saved: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                OnConfigStatusChanged($"Failed to save config: {ex.Message}");
            }
        }
        
        // Generate .anom content from config
        private string GenerateAnomContent(AnomConfig config)
        {
            var sb = new StringBuilder();
            
            // Settings section
            sb.AppendLine("[SETTINGS]");
            sb.AppendLine($"Author = {config.Author ?? "OpenBullet Copy"}");
            sb.AppendLine($"Version = {config.Version ?? "1.0"}");
            sb.AppendLine($"Timeout = {config.Timeout}");
            sb.AppendLine($"FollowRedirects = {config.FollowRedirects}");
            sb.AppendLine();
            
            // Request section
            if (!string.IsNullOrEmpty(config.Url))
            {
                sb.AppendLine("[REQUEST]");
                sb.AppendLine($"METHOD {config.Method}");
                sb.AppendLine($"URL {config.Url}");
                if (!string.IsNullOrEmpty(config.PostData))
                {
                    sb.AppendLine($"POST {config.PostData}");
                }
                sb.AppendLine();
            }
            
            // Headers section
            if (config.Headers.Count > 0)
            {
                sb.AppendLine("[HEADERS]");
                foreach (var header in config.Headers)
                {
                    sb.AppendLine($"{header.Key}: {header.Value}");
                }
                sb.AppendLine();
            }
            
            // KeyCheck section
            if (config.SuccessKeys.Count > 0 || config.FailureKeys.Count > 0 || config.BanKeys.Count > 0)
            {
                sb.AppendLine("[KEYCHECK]");
                foreach (var key in config.SuccessKeys)
                {
                    sb.AppendLine($"KEYCHECK SuccessOn \"{key}\"");
                }
                foreach (var key in config.FailureKeys)
                {
                    sb.AppendLine($"KEYCHECK FailureOn \"{key}\"");
                }
                foreach (var key in config.BanKeys)
                {
                    sb.AppendLine($"KEYCHECK BanOn \"{key}\"");
                }
                foreach (var key in config.RetryKeys)
                {
                    sb.AppendLine($"KEYCHECK RetryOn \"{key}\"");
                }
                sb.AppendLine();
            }
            
            // Capture section
            if (config.CapturePatterns.Count > 0)
            {
                sb.AppendLine("[CAPTURE]");
                foreach (var capture in config.CapturePatterns)
                {
                    var parts = capture.Value.Split('|');
                    if (parts.Length == 2)
                    {
                        sb.AppendLine($"CAPTURE {capture.Key} BETWEEN \"{parts[0]}\" AND \"{parts[1]}\"");
                    }
                }
                sb.AppendLine();
            }
            
            // Script section
            if (config.Scripts.Count > 0)
            {
                sb.AppendLine("[SCRIPT]");
                foreach (var script in config.Scripts)
                {
                    sb.AppendLine($"BEGIN {script.Type}");
                    sb.AppendLine(script.Content);
                    sb.AppendLine($"END {script.Type}");
                    sb.AppendLine();
                }
            }
            
            return sb.ToString();
        }
        
        // Create default Amazon config
        public AnomConfig CreateDefaultAmazonConfig()
        {
            var config = new AnomConfig
            {
                Name = "Amazon Checker",
                Author = "OpenBullet Copy",
                Version = "1.0",
                Method = "POST",
                Url = "https://www.amazon.ca/ap/signin",
                Timeout = 30000,
                FollowRedirects = true
            };
            
            // Headers
            config.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            config.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            config.Headers["Accept-Language"] = "en-US,en;q=0.9";
            config.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            
            // Success patterns
            config.SuccessKeys.Add("Enter your password");
            config.SuccessKeys.Add("ap_password");
            config.SuccessKeys.Add("Send OTP");
            config.SuccessKeys.Add("two-step verification");
            
            // Failure patterns
            config.FailureKeys.Add("We cannot find an account");
            config.FailureKeys.Add("We can't find an account");
            config.FailureKeys.Add("Wrong or Invalid");
            config.FailureKeys.Add("Create your Amazon account");
            
            // Ban patterns
            config.BanKeys.Add("captcha");
            config.BanKeys.Add("CAPTCHA");
            config.BanKeys.Add("automated access");
            config.BanKeys.Add("robot");
            
            return config;
        }
        
        // Test configuration with a sample input
        public ValidationResult TestConfig(AnomConfig config, string testInput)
        {
            try
            {
                // This would use the actual validation engine with the config
                // For now, return a test result
                var result = new ValidationResult
                {
                    PhoneNumber = testInput,
                    Status = "TEST",
                    Timestamp = DateTime.Now
                };
                
                OnConfigStatusChanged($"Config test completed for: {testInput}");
                return result;
            }
            catch (Exception ex)
            {
                OnConfigStatusChanged($"Config test failed: {ex.Message}");
                return null;
            }
        }
        
        // Export config to JSON
        public void ExportConfigToJson(AnomConfig config, string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(filePath, json);
                OnConfigStatusChanged($"Config exported to JSON: {filePath}");
            }
            catch (Exception ex)
            {
                OnConfigStatusChanged($"Export failed: {ex.Message}");
            }
        }
        
        // Import config from JSON
        public AnomConfig ImportConfigFromJson(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<AnomConfig>(json);
                configs.Add(config);
                OnConfigStatusChanged($"Config imported from JSON: {config.Name}");
                return config;
            }
            catch (Exception ex)
            {
                OnConfigStatusChanged($"Import failed: {ex.Message}");
                return null;
            }
        }
        
        protected virtual void OnConfigStatusChanged(string status)
        {
            ConfigStatusChanged?.Invoke(this, status);
        }
    }
}