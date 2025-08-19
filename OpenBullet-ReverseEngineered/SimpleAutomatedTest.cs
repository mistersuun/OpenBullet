using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Simple automated test that directly tests BlockRequest execution
/// and analyzes BAN status without complex dependencies
/// </summary>
public class SimpleAutomatedTest
{
    private static List<string> phoneNumbers = new List<string>
    {
        "+1234567890", "+1555123456", "+1987654321"
    };

    public static async Task<bool> RunTest()
    {
        Console.WriteLine("🚀 SIMPLE AUTOMATED TEST - BAN Status Analysis");
        Console.WriteLine("==============================================");
        
        int hits = 0, fails = 0, errors = 0, banCount = 0;
        
        try
        {
            // Load RuriLib
            Console.WriteLine("🔧 Loading RuriLib.dll...");
            var ruriLibAssembly = Assembly.LoadFrom("libs/RuriLib.dll");
            
            // Load config manually (bypass ConfigParser)
            Console.WriteLine("📄 Loading amazonChecker.anom...");
            string configContent = File.ReadAllText("amazonChecker.anom");
            
            // Extract script section manually
            var scriptStart = configContent.IndexOf("[SCRIPT]");
            if (scriptStart == -1)
            {
                Console.WriteLine("❌ [SCRIPT] section not found");
                return false;
            }
            
            string script = configContent.Substring(scriptStart + 8).Trim();
            Console.WriteLine($"✅ Script extracted: {script.Length} characters");
            
            // Test BlockParser directly
            Console.WriteLine("🔧 Testing original BlockParser...");
            var blockParserType = ruriLibAssembly.GetType("RuriLib.LS.BlockParser");
            if (blockParserType == null)
            {
                Console.WriteLine("❌ BlockParser type not found");
                return false;
            }
            
            var parseMethod = blockParserType.GetMethod("Parse", new[] { typeof(string) });
            if (parseMethod == null)
            {
                Console.WriteLine("❌ Parse method not found");
                return false;
            }
            
            // Parse the complete LoliScript
            var blocks = new List<object>();
            
            // Split into individual block lines
            var lines = script.Split('\n');
            string currentBlock = "";
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                
                // Handle comment-prefixed commands
                if (trimmed.StartsWith("#"))
                {
                    if (trimmed.Contains("REQUEST"))
                    {
                        // Extract REQUEST command
                        var match = System.Text.RegularExpressions.Regex.Match(trimmed, @"#[^ ]* (.+)");
                        if (match.Success)
                        {
                            string blockLine = match.Groups[1].Value;
                            
                            // Find related content lines
                            List<string> relatedLines = new List<string> { blockLine };
                            
                            // Look ahead for CONTENT, CONTENTTYPE, HEADER lines
                            for (int i = Array.IndexOf(lines, line) + 1; i < lines.Length; i++)
                            {
                                var nextLine = lines[i].Trim();
                                if (nextLine.StartsWith("CONTENT") || nextLine.StartsWith("CONTENTTYPE") || nextLine.StartsWith("HEADER"))
                                {
                                    relatedLines.Add(nextLine);
                                }
                                else if (!string.IsNullOrEmpty(nextLine) && !nextLine.StartsWith(" ") && !nextLine.StartsWith("\t"))
                                {
                                    break;
                                }
                            }
                            
                            // Combine into single block line
                            string fullBlockLine = string.Join(" ", relatedLines);
                            
                            try
                            {
                                var requestBlock = parseMethod.Invoke(null, new object[] { fullBlockLine });
                                if (requestBlock != null)
                                {
                                    blocks.Add(requestBlock);
                                    Console.WriteLine($"✅ BlockRequest created: {requestBlock.GetType().Name}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"❌ BlockRequest parsing failed: {ex.Message}");
                            }
                        }
                    }
                }
                else if (trimmed.StartsWith("KEYCHECK"))
                {
                    // Handle KEYCHECK block
                    List<string> keycheckLines = new List<string> { trimmed };
                    
                    // Collect all KEYCHECK related lines
                    for (int i = Array.IndexOf(lines, line) + 1; i < lines.Length; i++)
                    {
                        var nextLine = lines[i].Trim();
                        if (nextLine.StartsWith("KEYCHAIN") || nextLine.StartsWith("KEY") || nextLine.StartsWith(" "))
                        {
                            keycheckLines.Add(nextLine);
                        }
                        else if (!string.IsNullOrEmpty(nextLine))
                        {
                            break;
                        }
                    }
                    
                    string fullKeycheckBlock = string.Join(" ", keycheckLines);
                    
                    try
                    {
                        var keycheckBlock = parseMethod.Invoke(null, new object[] { fullKeycheckBlock });
                        if (keycheckBlock != null)
                        {
                            blocks.Add(keycheckBlock);
                            Console.WriteLine($"✅ BlockKeycheck created: {keycheckBlock.GetType().Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ BlockKeycheck parsing failed: {ex.Message}");
                    }
                }
            }
            
            Console.WriteLine($"🎯 Total blocks created: {blocks.Count}");
            
            if (blocks.Count == 0)
            {
                Console.WriteLine("❌ No blocks created - parsing failed");
                return false;
            }
            
            // Test each phone number
            foreach (var phoneNumber in phoneNumbers)
            {
                try
                {
                    Console.WriteLine($"\n📞 Testing phone number: {phoneNumber}");
                    
                    // Create minimal BotData for testing
                    var botDataType = ruriLibAssembly.GetType("RuriLib.BotData");
                    if (botDataType == null)
                    {
                        Console.WriteLine("❌ BotData type not found");
                        errors++;
                        continue;
                    }
                    
                    // Try simple constructor first
                    object botData = null;
                    var constructors = botDataType.GetConstructors();
                    
                    // Find a constructor we can use
                    foreach (var ctor in constructors)
                    {
                        var parameters = ctor.GetParameters();
                        if (parameters.Length == 0)
                        {
                            botData = ctor.Invoke(new object[0]);
                            break;
                        }
                    }
                    
                    if (botData == null)
                    {
                        Console.WriteLine("❌ Could not create BotData");
                        errors++;
                        continue;
                    }
                    
                    // Set basic properties
                    try
                    {
                        var dataProperty = botData.GetType().GetProperty("Data");
                        if (dataProperty != null)
                        {
                            // Create CData object for the phone number
                            var cDataType = ruriLibAssembly.GetType("RuriLib.Models.CData");
                            if (cDataType != null)
                            {
                                var cDataInstance = Activator.CreateInstance(cDataType);
                                var cDataDataProperty = cDataType.GetProperty("Data");
                                if (cDataDataProperty != null)
                                {
                                    cDataDataProperty.SetValue(cDataInstance, phoneNumber);
                                    dataProperty.SetValue(botData, cDataInstance);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Could not set Data property: {ex.Message}");
                    }
                    
                    Console.WriteLine($"✅ BotData created for {phoneNumber}");
                    
                    // Execute blocks
                    foreach (var block in blocks)
                    {
                        try
                        {
                            var blockType = block.GetType().Name;
                            Console.WriteLine($"🔧 Executing {blockType}...");
                            
                            var processMethod = block.GetType().GetMethod("Process");
                            if (processMethod != null)
                            {
                                processMethod.Invoke(block, new object[] { botData });
                                Console.WriteLine($"✅ {blockType} executed");
                            }
                        }
                        catch (Exception blockEx)
                        {
                            Console.WriteLine($"❌ Block execution failed: {blockEx.Message}");
                        }
                    }
                    
                    // Check final status
                    var statusProperty = botData.GetType().GetProperty("Status");
                    if (statusProperty != null)
                    {
                        var status = statusProperty.GetValue(botData)?.ToString();
                        Console.WriteLine($"📊 Final Status: {status}");
                        
                        switch (status?.ToUpper())
                        {
                            case "SUCCESS":
                                hits++;
                                Console.WriteLine($"🎉 SUCCESS: {phoneNumber}");
                                break;
                            case "FAIL":
                                fails++;
                                Console.WriteLine($"❌ FAIL: {phoneNumber}");
                                break;
                            case "BAN":
                                banCount++;
                                Console.WriteLine($"🚫 BAN: {phoneNumber}");
                                break;
                            default:
                                errors++;
                                Console.WriteLine($"⚠️ UNKNOWN: {phoneNumber} - {status}");
                                break;
                        }
                    }
                    else
                    {
                        errors++;
                        Console.WriteLine($"⚠️ No status property found for {phoneNumber}");
                    }
                }
                catch (Exception ex)
                {
                    errors++;
                    Console.WriteLine($"❌ Error testing {phoneNumber}: {ex.Message}");
                }
            }
            
            // Final analysis
            Console.WriteLine("\n📊 FINAL TEST RESULTS:");
            Console.WriteLine("=======================");
            Console.WriteLine($"Total Tested: {phoneNumbers.Count}");
            Console.WriteLine($"Hits: {hits}");
            Console.WriteLine($"Fails: {fails}");
            Console.WriteLine($"Errors: {errors}");
            Console.WriteLine($"BAN Status: {banCount}");
            Console.WriteLine($"Blocks Created: {blocks.Count}");
            
            if (banCount == 0 && errors == 0)
            {
                Console.WriteLine("🎉 SUCCESS! BAN STATUS ELIMINATED!");
                return true;
            }
            else if (banCount > 0)
            {
                Console.WriteLine("🚫 BAN status still present - anti-detection needed");
            }
            else if (errors > 0)
            {
                Console.WriteLine("❌ Execution errors - technical fixes needed");
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test failed: {ex.Message}");
            return false;
        }
    }
    
    public static async Task Main(string[] args)
    {
        try
        {
            // Change to the correct directory
            Directory.SetCurrentDirectory(@"C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered");
            
            bool success = await RunTest();
            
            if (success)
            {
                Console.WriteLine("\n🎉 AUTOMATED TEST SUCCESSFUL!");
            }
            else
            {
                Console.WriteLine("\n⚠️ Issues identified - manual fixes needed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Main failed: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}

