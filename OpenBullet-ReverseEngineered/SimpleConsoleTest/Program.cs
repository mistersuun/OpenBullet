using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleConsoleTest
{
    /// <summary>
    /// Simple console test to analyze Amazon's real response in 2025
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("🚀 Simple Amazon Response Analyzer - Console Test");
                Console.WriteLine("=" + new string('=', 60));
                Console.WriteLine($"📅 Current Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine();
                
                // Load original RuriLib.dll from current directory
                Console.WriteLine("📦 Loading original RuriLib.dll...");
                Console.WriteLine($"   Current directory: {Environment.CurrentDirectory}");
                
                string ruriLibPath = Path.Combine(Environment.CurrentDirectory, "RuriLib.dll");
                if (!File.Exists(ruriLibPath))
                {
                    Console.WriteLine($"❌ RuriLib.dll not found at: {ruriLibPath}");
                    return;
                }
                
                Assembly ruriLibAssembly = Assembly.LoadFrom(ruriLibPath);
                Console.WriteLine("✅ RuriLib.dll loaded successfully");
                
                // Load amazonChecker.anom config
                Console.WriteLine("📄 Loading amazonChecker.anom config...");
                string configPath = Path.Combine(Environment.CurrentDirectory, "amazonChecker.anom");
                
                if (!File.Exists(configPath))
                {
                    Console.WriteLine($"❌ Config file not found: {configPath}");
                    Console.WriteLine("Searching for config files...");
                    
                    var configFiles = Directory.GetFiles("..", "amazonChecker.anom", SearchOption.AllDirectories);
                    if (configFiles.Length > 0)
                    {
                        configPath = configFiles[0];
                        Console.WriteLine($"✅ Found config: {configPath}");
                    }
                    else
                    {
                        Console.WriteLine("❌ No amazonChecker.anom found. Exiting...");
                        return;
                    }
                }
                
                string configContent = File.ReadAllText(configPath);
                Console.WriteLine($"✅ Config loaded: {configContent.Length} characters");
                
                // Extract LoliScript from config
                string loliScript = ExtractLoliScript(configContent);
                Console.WriteLine($"📜 LoliScript extracted: {loliScript.Length} characters");
                Console.WriteLine();
                
                // Parse blocks using original RuriLib
                Console.WriteLine("🔧 Parsing blocks using original RuriLib...");
                var blocks = ParseBlocksWithOriginalRuriLib(ruriLibAssembly, loliScript);
                Console.WriteLine($"✅ Parsed {blocks.Count} blocks");
                
                // Test 5 phone numbers
                var testNumbers = new[]
                {
                    "+1234567890",
                    "+1987654321", 
                    "+1555123456",
                    "+1444555666",
                    "+1333222111"
                };
                
                Console.WriteLine($"\n🧪 Testing {testNumbers.Length} phone numbers against Amazon:");
                Console.WriteLine("=" + new string('=', 60));
                
                for (int i = 0; i < testNumbers.Length; i++)
                {
                    string phoneNumber = testNumbers[i];
                    Console.WriteLine($"\n📱 Test {i+1}/5: {phoneNumber}");
                    Console.WriteLine(new string('-', 40));
                    
                    try
                    {
                        await TestSinglePhoneNumber(ruriLibAssembly, blocks, phoneNumber, i + 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Test failed: {ex.Message}");
                        Console.WriteLine($"   Stack: {ex.StackTrace?.Split('\n')[0]}");
                    }
                }
                
                Console.WriteLine("\n🎯 Analysis Complete!");
                Console.WriteLine("=" + new string('=', 60));
                Console.WriteLine("💾 Check saved response files: Amazon_Response_Test_*.html");
                Console.WriteLine();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        /// <summary>
        /// Extract LoliScript from .anom config file
        /// </summary>
        static string ExtractLoliScript(string configContent)
        {
            // Find the script section between [SCRIPT] and [/SCRIPT]
            int scriptStart = configContent.IndexOf("[SCRIPT]");
            int scriptEnd = configContent.IndexOf("[/SCRIPT]");
            
            if (scriptStart == -1 || scriptEnd == -1)
            {
                Console.WriteLine("⚠️ No [SCRIPT] section found, using entire config as script");
                return configContent;
            }
            
            scriptStart += "[SCRIPT]".Length;
            string script = configContent.Substring(scriptStart, scriptEnd - scriptStart).Trim();
            
            Console.WriteLine($"📋 Script section found: {script.Length} characters");
            return script;
        }
        
        /// <summary>
        /// Parse blocks using the original RuriLib BlockParser
        /// </summary>
        static List<object> ParseBlocksWithOriginalRuriLib(Assembly ruriLibAssembly, string loliScript)
        {
            try
            {
                // Get the original BlockParser.Parse method
                Type blockParserType = ruriLibAssembly.GetType("RuriLib.LS.BlockParser");
                if (blockParserType == null)
                {
                    Console.WriteLine("❌ BlockParser not found in RuriLib");
                    return new List<object>();
                }
                
                MethodInfo parseMethod = blockParserType.GetMethod("Parse", 
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new[] { typeof(string) },
                    null);
                
                if (parseMethod == null)
                {
                    Console.WriteLine("❌ BlockParser.Parse method not found");
                    return new List<object>();
                }
                
                Console.WriteLine("✅ Found original BlockParser.Parse method");
                
                // Compress multi-line LoliScript like the original does
                string compressedScript = CompressLoliScript(loliScript);
                Console.WriteLine($"🗜️ Compressed script: {compressedScript.Length} characters");
                
                // Parse blocks
                var result = parseMethod.Invoke(null, new object[] { compressedScript });
                
                if (result is IList<object> blocks)
                {
                    Console.WriteLine($"✅ Successfully parsed {blocks.Count} blocks");
                    return new List<object>(blocks);
                }
                
                Console.WriteLine("❌ Parse result is not a block list");
                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Block parsing failed: {ex.Message}");
                return new List<object>();
            }
        }
        
        /// <summary>
        /// Compress multi-line LoliScript commands into single lines (like original)
        /// </summary>
        static string CompressLoliScript(string script)
        {
            var lines = script.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var compressedLines = new List<string>();
            
            string currentBlock = "";
            
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;
                    
                // If this is a new command (REQUEST, KEYCHECK, etc)
                if (IsCommandLine(trimmed))
                {
                    // Save previous block if exists
                    if (!string.IsNullOrEmpty(currentBlock))
                    {
                        compressedLines.Add(currentBlock.Trim());
                    }
                    
                    currentBlock = trimmed;
                }
                else
                {
                    // Continuation line (CONTENT, HEADER, etc)
                    currentBlock += " " + trimmed;
                }
            }
            
            // Save last block
            if (!string.IsNullOrEmpty(currentBlock))
            {
                compressedLines.Add(currentBlock.Trim());
            }
            
            return string.Join("\n", compressedLines);
        }
        
        /// <summary>
        /// Check if line starts a new LoliScript command
        /// </summary>
        static bool IsCommandLine(string line)
        {
            string[] commands = { "REQUEST", "POST", "GET", "KEYCHECK", "PARSE", "FUNCTION", "SET", "DELETE", "SAVE" };
            
            foreach (string cmd in commands)
            {
                if (line.StartsWith(cmd + " ", StringComparison.OrdinalIgnoreCase) || 
                    line.Equals(cmd, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Test a single phone number and capture Amazon's real response
        /// </summary>
        static async Task TestSinglePhoneNumber(Assembly ruriLibAssembly, List<object> blocks, string phoneNumber, int testNumber)
        {
            try
            {
                Console.WriteLine($"🎯 Creating original BotData for {phoneNumber}...");
                
                // Create original BotData using reflection
                Type botDataType = ruriLibAssembly.GetType("RuriLib.BotData");
                if (botDataType == null)
                {
                    Console.WriteLine("❌ BotData type not found");
                    return;
                }
                
                // Use 8-parameter constructor (like successful integration)
                var botDataConstructor = botDataType.GetConstructor(new[]
                {
                    typeof(string),        // data
                    ruriLibAssembly.GetType("RuriLib.Models.Proxy"),  // proxy
                    ruriLibAssembly.GetType("RuriLib.Interfaces.IProxyChecker"), // proxyChecker
                    ruriLibAssembly.GetType("RuriLib.ViewModels.ConfigViewModel"), // configModel
                    typeof(object),        // provider
                    typeof(bool),          // useProxies
                    ruriLibAssembly.GetType("RuriLib.Models.CVar"),  // wordlistType
                    typeof(string)         // wordlistName
                });
                
                if (botDataConstructor == null)
                {
                    Console.WriteLine("❌ BotData constructor not found");
                    return;
                }
                
                // Create BotData instance (pass nulls for unused parameters)
                object botData = botDataConstructor.Invoke(new object[] 
                { 
                    phoneNumber, 
                    null, null, null, null, 
                    false, 
                    null, 
                    "TestWordlist" 
                });
                
                Console.WriteLine($"✅ BotData created for {phoneNumber}");
                
                // Execute each block in order
                int blockIndex = 0;
                foreach (object block in blocks)
                {
                    blockIndex++;
                    string blockType = block.GetType().Name;
                    Console.WriteLine($"🔧 Executing Block {blockIndex}: {blockType}");
                    
                    try
                    {
                        // Get the Process method
                        MethodInfo processMethod = block.GetType().GetMethod("Process", 
                            new[] { botDataType });
                        
                        if (processMethod == null)
                        {
                            Console.WriteLine($"❌ No Process method found for {blockType}");
                            continue;
                        }
                        
                        // Execute the block
                        processMethod.Invoke(block, new object[] { botData });
                        Console.WriteLine($"✅ {blockType}.Process() completed");
                        
                        // For BlockRequest, capture the response
                        if (blockType.Contains("Request"))
                        {
                            await CaptureHttpResponse(botData, phoneNumber, testNumber);
                        }
                        
                        // For BlockKeycheck, show the result
                        if (blockType.Contains("Keycheck"))
                        {
                            ShowKeycheckResult(botData, phoneNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ {blockType} execution failed: {ex.Message}");
                    }
                }
                
                // Show final status
                ShowFinalStatus(botData, phoneNumber);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace?.Split('\n')[0]}");
            }
        }
        
        /// <summary>
        /// Capture HTTP response from BotData after BlockRequest
        /// </summary>
        static async Task CaptureHttpResponse(object botData, string phoneNumber, int testNumber)
        {
            try
            {
                // Get ResponseSource from BotData
                PropertyInfo responseSourceProp = botData.GetType().GetProperty("ResponseSource");
                string responseContent = responseSourceProp?.GetValue(botData)?.ToString() ?? "";
                
                if (!string.IsNullOrEmpty(responseContent))
                {
                    Console.WriteLine($"📄 HTTP Response captured: {responseContent.Length} characters");
                    
                    // Save full response for analysis
                    string responseFile = $"Amazon_Response_Test_{testNumber}_{phoneNumber.Replace("+", "")}.html";
                    File.WriteAllText(responseFile, responseContent);
                    Console.WriteLine($"💾 Response saved: {responseFile}");
                    
                    // Show response preview
                    string preview = responseContent.Length > 300 ? 
                        responseContent.Substring(0, 300) + "..." : responseContent;
                    Console.WriteLine($"📋 Response Preview:");
                    Console.WriteLine($"   {preview}");
                    
                    // Analyze for KEYCHECK patterns
                    AnalyzeResponseForKeycheck(responseContent, phoneNumber);
                }
                else
                {
                    Console.WriteLine("⚠️ No HTTP response captured");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Response capture failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Analyze response for KEYCHECK patterns to understand why it's BAN
        /// </summary>
        static void AnalyzeResponseForKeycheck(string responseContent, string phoneNumber)
        {
            Console.WriteLine($"\n🔍 KEYCHECK ANALYSIS for {phoneNumber}:");
            Console.WriteLine(new string('-', 30));
            
            // Check old 2022 keys from config
            var old2022Keys = new[]
            {
                "Sign-In ",
                "No account found with that email address1519",
                "ap_ra_email_or_phone", 
                "We cannot find an account with that mobile number"
            };
            
            Console.WriteLine("❌ OLD 2022 KEYCHECK KEYS:");
            bool anyOldKeyFound = false;
            foreach (var key in old2022Keys)
            {
                bool found = responseContent.Contains(key);
                Console.WriteLine($"   '{key}': {(found ? "✅ FOUND" : "❌ NOT FOUND")}");
                if (found) anyOldKeyFound = true;
            }
            
            if (!anyOldKeyFound)
            {
                Console.WriteLine("⚠️ NONE of the old 2022 keys found - this explains the BAN!");
            }
            
            // Look for potential new 2025 keys
            Console.WriteLine("\n🔍 POTENTIAL NEW 2025 KEYS:");
            var potentialKeys = new[] 
            { 
                "signin", "password", "continue", "submit", "error", "problem", 
                "invalid", "not found", "account", "email", "phone", "mobile"
            };
            
            foreach (var key in potentialKeys)
            {
                if (responseContent.ToLower().Contains(key.ToLower()))
                {
                    Console.WriteLine($"   ✅ Found: '{key}'");
                }
            }
            
            // Extract title and key elements
            try
            {
                var titleMatch = System.Text.RegularExpressions.Regex.Match(
                    responseContent, @"<title>(.*?)</title>", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (titleMatch.Success)
                {
                    Console.WriteLine($"📋 Page Title: '{titleMatch.Groups[1].Value}'");
                }
            }
            catch { }
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// Show KEYCHECK result from BotData
        /// </summary>
        static void ShowKeycheckResult(object botData, string phoneNumber)
        {
            try
            {
                PropertyInfo statusProp = botData.GetType().GetProperty("Status");
                object status = statusProp?.GetValue(botData);
                
                Console.WriteLine($"🎯 KEYCHECK Result for {phoneNumber}: {status ?? "UNKNOWN"}");
                
                if (status?.ToString() == "BAN")
                {
                    Console.WriteLine("❌ Status is BAN - old KEYCHECK keys don't match 2025 Amazon response");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not read KEYCHECK result: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Show final BotData status
        /// </summary>
        static void ShowFinalStatus(object botData, string phoneNumber)
        {
            try
            {
                PropertyInfo statusProp = botData.GetType().GetProperty("Status");
                object finalStatus = statusProp?.GetValue(botData);
                
                Console.WriteLine($"\n📊 FINAL STATUS for {phoneNumber}: {finalStatus ?? "UNKNOWN"}");
                
                if (finalStatus?.ToString() == "BAN")
                {
                    Console.WriteLine("🎯 CONCLUSION: BAN status caused by outdated KEYCHECK patterns");
                    Console.WriteLine("   ↳ Amazon's 2025 response doesn't contain the 2022 keys");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not read final status: {ex.Message}");
            }
        }
    }
}
