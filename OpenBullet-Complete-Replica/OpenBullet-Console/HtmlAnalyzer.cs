using System;
using System.IO;
using System.Linq;

namespace OpenBullet_Console
{
    /// <summary>
    /// Scientific HTML analyzer that examines saved Amazon response files
    /// to identify exact validation patterns for valid vs invalid accounts
    /// </summary>
    public static class HtmlAnalyzer
    {
        public static void AnalyzeResponseFiles(bool debugMode = true)
        {
            try
            {
                if (debugMode)
                {
                    Console.WriteLine("🔬 ===== HTML PATTERN ANALYSIS =====");
                    Console.WriteLine("📂 Analyzing saved Amazon response files...");
                }

                var resultsDir = "Results/";
                if (!Directory.Exists(resultsDir))
                {
                    Console.WriteLine("❌ Results directory not found");
                    return;
                }

                // Get recent response files for analysis
                var responseFiles = Directory.GetFiles(resultsDir, "*_response_*.html")
                    .Where(f => f.Contains("20250820"))
                    .OrderByDescending(File.GetLastWriteTime)
                    .Take(10)
                    .ToList();

                if (debugMode)
                {
                    Console.WriteLine($"📊 Found {responseFiles.Count} recent response files");
                }

                foreach (var file in responseFiles)
                {
                    AnalyzeResponseFile(file, debugMode);
                }
            }
            catch (Exception ex)
            {
                if (debugMode)
                {
                    Console.WriteLine($"❌ Analysis error: {ex.Message}");
                }
            }
        }

        private static void AnalyzeResponseFile(string filePath, bool debugMode)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var fileSize = new FileInfo(filePath).Length;
                var content = File.ReadAllText(filePath);

                // Extract phone number from filename
                var phoneNumber = ExtractPhoneFromFilename(fileName);
                var isKnownValid = IsKnownValidNumber(phoneNumber);
                var isKnownInvalid = IsKnownInvalidNumber(phoneNumber);

                if (debugMode)
                {
                    Console.WriteLine($"\n📄 ANALYZING: {fileName}");
                    Console.WriteLine($"📏 File size: {fileSize:N0} bytes");
                    Console.WriteLine($"📱 Phone: {phoneNumber}");
                    Console.WriteLine($"✅ Known valid: {isKnownValid}");
                    Console.WriteLine($"❌ Known invalid: {isKnownInvalid}");
                }

                // Check for validation patterns
                var patterns = AnalyzeValidationPatterns(content, phoneNumber);

                if (debugMode)
                {
                    Console.WriteLine("🔍 PATTERNS FOUND:");
                    foreach (var pattern in patterns)
                    {
                        Console.WriteLine($"   {pattern}");
                    }
                }

                // Compare against expected result
                if (isKnownValid && patterns.Any(p => p.Contains("INVALID")))
                {
                    Console.WriteLine($"🚨 MISMATCH: {phoneNumber} is known valid but patterns suggest invalid!");
                }
                else if (isKnownInvalid && patterns.Any(p => p.Contains("VALID")))
                {
                    Console.WriteLine($"🚨 MISMATCH: {phoneNumber} is known invalid but patterns suggest valid!");
                }
            }
            catch (Exception ex)
            {
                if (debugMode)
                {
                    Console.WriteLine($"❌ File analysis error: {ex.Message}");
                }
            }
        }

        private static string[] AnalyzeValidationPatterns(string content, string phoneNumber)
        {
            var patterns = new List<string>();

            // Pattern 1: Error messages (INVALID indicators)
            if (content.Contains("Incorrect phone number"))
                patterns.Add("❌ INVALID: 'Incorrect phone number'");

            if (content.Contains("We cannot find an account with that mobile number"))
                patterns.Add("❌ INVALID: 'We cannot find an account'");

            if (content.Contains("Password reset required"))
                patterns.Add("❌ INVALID: 'Password reset required'");

            if (content.Contains("BlacklistPasswordReverificationApplication"))
                patterns.Add("❌ INVALID: 'BlacklistPasswordReverification'");

            // Pattern 2: Success indicators (VALID indicators)  
            if (content.Contains("id=\"ap_password\""))
                patterns.Add("✅ VALID: Password field present");

            if (content.Contains("Enter your password"))
                patterns.Add("✅ VALID: 'Enter your password' prompt");

            if (content.Contains($"value=\"+{phoneNumber}\"") || 
                content.Contains($"value=\"+1{phoneNumber}\"") ||
                content.Contains($"value=\"{phoneNumber}\""))
                patterns.Add("✅ VALID: Phone number properly formatted in form");

            // Pattern 3: Error alerts
            if (content.Contains("auth-error-message-box"))
                patterns.Add("❌ INVALID: Error message box present");

            if (content.Contains("a-alert-error"))
                patterns.Add("❌ INVALID: Alert error class present");

            // Pattern 4: Blocking indicators
            if (content.Contains("Please Enable Cookies to Continue"))
                patterns.Add("🚫 BLOCKED: Cookie warning");

            if (content.Contains("security check"))
                patterns.Add("🚫 BLOCKED: Security check");

            if (patterns.Count == 0)
                patterns.Add("⚠️ UNCLEAR: No clear validation patterns found");

            return patterns.ToArray();
        }

        private static string ExtractPhoneFromFilename(string fileName)
        {
            // Extract phone number from filename like "debug_response_15142955315_20250820_081930.html"
            try
            {
                var parts = fileName.Split('_');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] == "response" && i + 1 < parts.Length)
                    {
                        return parts[i + 1];
                    }
                }
            }
            catch { }
            return "unknown";
        }

        private static bool IsKnownValidNumber(string phoneNumber)
        {
            var knownValid = new[] { "5142955315", "15142955315", "souljrcam@gmail.com", "souleydop@gmail.com" };
            return knownValid.Contains(phoneNumber);
        }

        private static bool IsKnownInvalidNumber(string phoneNumber)
        {
            var knownInvalid = new[] { "11111111111", "0000000", "22222222", "1111111111", "2222222222" };
            return knownInvalid.Contains(phoneNumber);
        }
    }
}

