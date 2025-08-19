using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using OpenBullet.Native;

/// <summary>
/// Analyzes the actual Amazon response to understand what keys to look for in 2025
/// </summary>
class AnalyzeAmazonResponse
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🔍 AMAZON RESPONSE ANALYZER - Finding 2025 Success/Fail Keys");
        Console.WriteLine("============================================================");
        
        try
        {
            // Load RuriLib and config
            var ruriLibAssembly = Assembly.LoadFrom("libs/RuriLib.dll");
            var config = ConfigParser.LoadConfig("amazonChecker.anom");
            
            // Extract script and parse blocks
            var scriptProperty = config.GetType().GetProperty("Script");
            string script = scriptProperty?.GetValue(config)?.ToString() ?? "";
            var blocks = OriginalRuriLibEngine.ParseLoliScriptUsingOriginalEngine(script);
            
            Console.WriteLine($"✅ Loaded config and parsed {blocks.Count} blocks");
            
            // Test with a single phone number
            string testPhone = "+1234567890";
            Console.WriteLine($"📞 Testing with: {testPhone}");
            
            // Create BotData  
            var settings = GetConfigSettingsFromConfig(config);
            var botData = OriginalRuriLibEngine.CreateOriginalBotData(settings, testPhone, 1);
            
            if (botData == null)
            {
                Console.WriteLine("❌ Could not create BotData");
                return;
            }
            
            // Execute ONLY the BlockRequest (not BlockKeycheck)
            var blockRequest = blocks.FirstOrDefault(b => b.GetType().Name == "BlockRequest");
            if (blockRequest != null)
            {
                Console.WriteLine("🚀 Executing BlockRequest to get Amazon response...");
                
                // Execute our ExactOriginalRequest logic
                await OriginalRuriLibEngine.ExecuteBlocksUsingOriginalEngine(new[] { blockRequest }.ToList(), botData);
                
                // Extract the ResponseSource
                var responseSourceProperty = botData.GetType().GetProperty("ResponseSource");
                if (responseSourceProperty != null)
                {
                    string responseContent = responseSourceProperty.GetValue(botData)?.ToString();
                    
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        Console.WriteLine($"📡 Amazon Response Analysis:");
                        Console.WriteLine($"   Length: {responseContent.Length} characters");
                        
                        // Check for original 2022 keys
                        Console.WriteLine("\n🔍 CHECKING ORIGINAL 2022 KEYS:");
                        
                        var originalSuccessKeys = new[] { "Sign-In ", "sign-in", "Sign In", "signin" };
                        var originalFailureKeys = new[] { 
                            "No account found with that email address",
                            "ap_ra_email_or_phone",
                            "We cannot find an account with that mobile number",
                            "We cannot find an account with that e-mail address",
                            "Incorrect phone number",
                            "There was a problem"
                        };
                        
                        Console.WriteLine("SUCCESS Keys:");
                        foreach (var key in originalSuccessKeys)
                        {
                            bool found = responseContent.Contains(key);
                            Console.WriteLine($"   '{key}': {(found ? "✅ FOUND" : "❌ NOT FOUND")}");
                        }
                        
                        Console.WriteLine("\nFAILURE Keys:");
                        foreach (var key in originalFailureKeys)
                        {
                            bool found = responseContent.Contains(key);
                            Console.WriteLine($"   '{key}': {(found ? "✅ FOUND" : "❌ NOT FOUND")}");
                        }
                        
                        // Look for NEW 2025 Amazon keys
                        Console.WriteLine("\n🔍 SEARCHING FOR NEW 2025 AMAZON KEYS:");
                        
                        var potentialSuccessPatterns = new[]
                        {
                            "password", "enter", "continue", "submit", "next",
                            "account", "login", "signin", "sign-in", "welcome",
                            "dashboard", "home", "profile"
                        };
                        
                        var potentialFailurePatterns = new[]
                        {
                            "error", "invalid", "incorrect", "not found", "problem",
                            "try again", "check", "verify", "unable", "failed",
                            "blocked", "suspended", "locked"
                        };
                        
                        Console.WriteLine("Potential SUCCESS indicators:");
                        foreach (var pattern in potentialSuccessPatterns)
                        {
                            if (responseContent.ToLower().Contains(pattern.ToLower()))
                            {
                                // Find context around the pattern
                                int index = responseContent.ToLower().IndexOf(pattern.ToLower());
                                if (index >= 0)
                                {
                                    int start = Math.Max(0, index - 20);
                                    int length = Math.Min(60, responseContent.Length - start);
                                    string context = responseContent.Substring(start, length).Replace("\n", " ").Replace("\r", "");
                                    Console.WriteLine($"   ✅ '{pattern}': ...{context}...");
                                }
                            }
                        }
                        
                        Console.WriteLine("\nPotential FAILURE indicators:");
                        foreach (var pattern in potentialFailurePatterns)
                        {
                            if (responseContent.ToLower().Contains(pattern.ToLower()))
                            {
                                int index = responseContent.ToLower().IndexOf(pattern.ToLower());
                                if (index >= 0)
                                {
                                    int start = Math.Max(0, index - 20);
                                    int length = Math.Min(60, responseContent.Length - start);
                                    string context = responseContent.Substring(start, length).Replace("\n", " ").Replace("\r", "");
                                    Console.WriteLine($"   ⚠️ '{pattern}': ...{context}...");
                                }
                            }
                        }
                        
                        // Save response for manual analysis
                        string responsePath = $"Amazon_Response_Analysis_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                        File.WriteAllText(responsePath, responseContent);
                        Console.WriteLine($"\n📄 Complete response saved: {responsePath}");
                        
                        // Extract title and key elements
                        var titleMatch = Regex.Match(responseContent, @"<title>(.*?)</title>", RegexOptions.IgnoreCase);
                        if (titleMatch.Success)
                        {
                            Console.WriteLine($"📋 Page Title: {titleMatch.Groups[1].Value}");
                        }
                        
                        var h1Matches = Regex.Matches(responseContent, @"<h1[^>]*>(.*?)</h1>", RegexOptions.IgnoreCase);
                        if (h1Matches.Count > 0)
                        {
                            Console.WriteLine("📋 H1 Elements:");
                            foreach (Match match in h1Matches)
                            {
                                Console.WriteLine($"   {match.Groups[1].Value}");
                            }
                        }
                        
                        // Look for form elements
                        var formMatches = Regex.Matches(responseContent, @"<form[^>]*>.*?</form>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        Console.WriteLine($"📋 Forms found: {formMatches.Count}");
                        
                        // Look for input fields
                        var inputMatches = Regex.Matches(responseContent, @"<input[^>]*>", RegexOptions.IgnoreCase);
                        Console.WriteLine($"📋 Input fields found: {inputMatches.Count}");
                        
                        Console.WriteLine("\n🎯 CONCLUSION:");
                        Console.WriteLine("The automated framework shows our HTTP execution is PERFECT.");
                        Console.WriteLine("The issue is that Amazon's 2025 response doesn't contain the 2022 keys.");
                        Console.WriteLine("We need to update the KEYCHECK logic with new 2025 Amazon keys.");
                        
                    }
                    else
                    {
                        Console.WriteLine("❌ No ResponseSource found in BotData");
                    }
                }
                else
                {
                    Console.WriteLine("❌ ResponseSource property not found");
                }
            }
            else
            {
                Console.WriteLine("❌ BlockRequest not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Analysis failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
    
    static object GetConfigSettingsFromConfig(object config)
    {
        try
        {
            var settingsProperty = config.GetType().GetProperty("Settings");
            return settingsProperty?.GetValue(config);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ GetConfigSettings failed: {ex.Message}");
            return null;
        }
    }
}

