using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json; // RESTORED - using exact original version

namespace OpenBullet.Native
{
    /// <summary>
    /// Config parser that uses the discovered RuriLib.Config API
    /// Constructor: Config(ConfigSettings settings, String script)
    /// </summary>
    public class ConfigParser
    {
        public static object LoadConfig(string filePath)
        {
            try
            {
                Console.WriteLine($"🔧 Loading config using discovered RuriLib API: {Path.GetFileName(filePath)}");
                
                // Step 1: Read the .anom file
                var configContent = File.ReadAllText(filePath);
                Console.WriteLine($"📄 Config content: {configContent.Length} characters");
                
                // Step 2: Parse [SETTINGS] section
                var settingsJson = ExtractSettingsSection(configContent);
                var settings = ParseConfigSettings(settingsJson);
                
                // Step 3: Parse [SCRIPT] section  
                var script = ExtractScriptSection(configContent);
                
                // Step 4: Create RuriLib.Config using discovered constructor
                var config = CreateRuriLibConfig(settings, script);
                
                Console.WriteLine("✅ Config loaded successfully using RuriLib!");
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Config loading failed: {ex.Message}");
                throw;
            }
        }
        
        private static string ExtractSettingsSection(string configContent)
        {
            try
            {
                var settingsMatch = Regex.Match(configContent, @"\[SETTINGS\]\s*(\{.*?\})", RegexOptions.Singleline);
                if (settingsMatch.Success)
                {
                    var settingsJson = settingsMatch.Groups[1].Value;
                    Console.WriteLine($"✅ Extracted [SETTINGS]: {settingsJson.Length} chars");
                    return settingsJson;
                }
                
                Console.WriteLine("⚠️ No [SETTINGS] section found");
                return "{}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Settings extraction failed: {ex.Message}");
                return "{}";
            }
        }
        
        private static string ExtractScriptSection(string configContent)
        {
            try
            {
                var scriptStart = configContent.IndexOf("[SCRIPT]");
                if (scriptStart >= 0)
                {
                    var script = configContent.Substring(scriptStart + "[SCRIPT]".Length).Trim();
                    Console.WriteLine($"✅ Extracted [SCRIPT]: {script.Length} chars");
                    return script;
                }
                
                Console.WriteLine("⚠️ No [SCRIPT] section found");
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Script extraction failed: {ex.Message}");
                return "";
            }
        }
        
        private static object ParseConfigSettings(string settingsJson)
        {
            try
            {
                // Load RuriLib assembly
                var ruriLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
                var assembly = Assembly.LoadFrom(ruriLibPath);
                
                // Find ConfigSettings type
                var configSettingsType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == "ConfigSettings" && t.IsPublic);
                
                if (configSettingsType == null)
                {
                    Console.WriteLine("❌ ConfigSettings type not found");
                    return null;
                }
                
                Console.WriteLine($"✅ Found ConfigSettings type: {configSettingsType.FullName}");
                
                // Try to create a ConfigSettings instance
                var configSettings = Activator.CreateInstance(configSettingsType);
                
                // RESTORED: Use exact original JsonConvert.DeserializeObject with matching Newtonsoft.Json version
                var settingsDict = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(settingsJson);
                
                if (settingsDict != null)
                {
                    foreach (var kvp in settingsDict)
                    {
                        var property = configSettingsType.GetProperty(kvp.Key);
                        if (property != null && property.CanWrite)
                        {
                            try
                            {
                                // Convert the value to the property type
                                var convertedValue = Convert.ChangeType(kvp.Value, property.PropertyType);
                                property.SetValue(configSettings, convertedValue);
                                Console.WriteLine($"   ✅ Set {kvp.Key} = {kvp.Value}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"   ⚠️ Failed to set {kvp.Key}: {ex.Message}");
                            }
                        }
                    }
                }
                
                Console.WriteLine("✅ ConfigSettings created and populated");
                return configSettings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ConfigSettings parsing failed: {ex.Message}");
                return null;
            }
        }
        
        private static object CreateRuriLibConfig(object settings, string script)
        {
            try
            {
                // Load RuriLib assembly
                var ruriLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
                var assembly = Assembly.LoadFrom(ruriLibPath);
                
                // Find Config type
                var configType = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == "Config" && t.IsPublic);
                
                if (configType == null)
                {
                    Console.WriteLine("❌ Config type not found");
                    return null;
                }
                
                // Find the constructor: Config(ConfigSettings settings, String script)
                var constructor = configType.GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == 2);
                
                if (constructor == null)
                {
                    Console.WriteLine("❌ Config constructor not found");
                    return null;
                }
                
                Console.WriteLine("🔨 Creating RuriLib.Config using discovered constructor...");
                
                // Create the Config instance
                var config = constructor.Invoke(new object[] { settings, script });
                
                Console.WriteLine("✅ RuriLib.Config created successfully!");
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Config creation failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                }
                return null;
            }
        }
        
        /// <summary>
        /// MANUAL JSON PARSER - Bypasses Newtonsoft.Json dependency issues
        /// Parses basic JSON settings without external dependencies
        /// </summary>
        private static Dictionary<string, object> ParseSettingsManually(string settingsJson)
        {
            try
            {
                Console.WriteLine("🔧 ParseSettingsManually - Bypassing Newtonsoft.Json dependency");
                var settings = new Dictionary<string, object>();
                
                if (string.IsNullOrEmpty(settingsJson))
                {
                    Console.WriteLine("⚠️ Settings JSON is empty - using defaults");
                    return CreateDefaultSettings();
                }
                
                // Remove braces and split by commas
                string cleanJson = settingsJson.Trim().Trim('{', '}');
                var pairs = cleanJson.Split(',');
                
                foreach (var pair in pairs)
                {
                    try
                    {
                        var keyValue = pair.Split(':');
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim().Trim('"');
                            string value = keyValue[1].Trim().Trim('"');
                            
                            // Convert values to appropriate types
                            if (bool.TryParse(value, out bool boolValue))
                            {
                                settings[key] = boolValue;
                            }
                            else if (int.TryParse(value, out int intValue))
                            {
                                settings[key] = intValue;
                            }
                            else if (double.TryParse(value, out double doubleValue))
                            {
                                settings[key] = doubleValue;
                            }
                            else
                            {
                                settings[key] = value; // String value
                            }
                            
                            Console.WriteLine($"   {key}: {settings[key]} ({settings[key].GetType().Name})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Failed to parse setting pair: {pair} - {ex.Message}");
                    }
                }
                
                Console.WriteLine($"✅ Manual JSON parsing completed: {settings.Count} settings");
                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Manual JSON parsing failed: {ex.Message}");
                return CreateDefaultSettings();
            }
        }
        
        /// <summary>
        /// Creates default ConfigSettings values based on decompiled source analysis
        /// </summary>
        private static Dictionary<string, object> CreateDefaultSettings()
        {
            Console.WriteLine("🔧 Creating default ConfigSettings based on decompiled source");
            
            return new Dictionary<string, object>
            {
                ["Name"] = "Amazon Phone Checker",
                ["Author"] = "saisu",
                ["Version"] = "1.1.5",
                ["IgnoreResponseErrors"] = true,      // From decompiled ConfigSettings.cs
                ["MaxRedirects"] = 8,                 // From decompiled ConfigSettings.cs  
                ["NeedsProxies"] = false,            // From decompiled ConfigSettings.cs
                ["OnlySocks"] = false,               // From decompiled ConfigSettings.cs
                ["OnlySsl"] = false,                 // From decompiled ConfigSettings.cs
                ["SuggestedBots"] = 1,               // From decompiled ConfigSettings.cs
                ["MaxCPM"] = 0,                      // From decompiled ConfigSettings.cs
                ["EncodeData"] = false,              // From decompiled ConfigSettings.cs
                ["AllowedWordlist1"] = "",           // From decompiled ConfigSettings.cs
                ["AllowedWordlist2"] = ""            // From decompiled ConfigSettings.cs
            };
        }
    }
}
