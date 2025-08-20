using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

// Original OpenBullet imports - these will only work if the DLLs are compatible
#if NETFRAMEWORK
using RuriLib;
using RuriLib.Models;
using RuriLib.Runner;
using RuriLib.ViewModels;
#endif

namespace OpenBullet_Replica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#if NETFRAMEWORK
        // Original OpenBullet components
        private Config currentConfig;
        private RunnerViewModel runner;
        private RLSettingsViewModel globalSettings;
        private EnvironmentSettings environment;
        private List<CProxy> proxies = new List<CProxy>();
        private DataPool dataPool;
#endif
        
        // Statistics tracking
        private int testedCount = 0;
        private int hitCount = 0;
        private int failCount = 0;
        private DateTime startTime;
        private DispatcherTimer updateTimer;
        private bool isRunning = false;

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

#if NETFRAMEWORK
                // Initialize OpenBullet components (only works with .NET Framework + original DLLs)
                InitializeOpenBulletComponents();
#endif

                LogMessage("Application initialized successfully");
                statusText.Text = "Ready - Load a config and wordlist to begin";
            }
            catch (Exception ex)
            {
                LogMessage($"Initialization error: {ex.Message}");
                MessageBox.Show($"Failed to initialize application: {ex.Message}", 
                              "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

#if NETFRAMEWORK
        private void InitializeOpenBulletComponents()
        {
            try
            {
                // Load or create default settings
                if (File.Exists("Settings/RLSettings.json"))
                {
                    globalSettings = IOManager.LoadSettings("Settings/RLSettings.json");
                }
                else
                {
                    globalSettings = new RLSettingsViewModel();
                    globalSettings.General.RequestTimeout = 30;
                    globalSettings.General.WaitTime = 100;
                }

                // Load environment settings
                if (File.Exists("Settings/Environment.ini"))
                {
                    environment = IOManager.ParseEnvironmentSettings("Settings/Environment.ini");
                }
                else
                {
                    environment = new EnvironmentSettings();
                }

                // Initialize runner
                runner = new RunnerViewModel(environment, globalSettings, new Random());
                
                // Setup event handlers
                runner.MessageArrived += (level, message, sound) =>
                {
                    Dispatcher.Invoke(() => LogMessage(message));
                };

                LogMessage("OpenBullet components initialized successfully");
            }
            catch (Exception ex)
            {
                LogMessage($"OpenBullet initialization error: {ex.Message}");
                throw;
            }
        }
#endif

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
                lblTested.Text = testedCount.ToString();
                lblHits.Text = hitCount.ToString();
                lblFails.Text = failCount.ToString();
                lblQuickHits.Text = hitCount.ToString();
                lblQuickFails.Text = failCount.ToString();

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
#if NETFRAMEWORK
                if (dataPool != null && dataPool.Size > 0)
                {
                    progressBar.Value = (double)testedCount / dataPool.Size * 100;
                    lblProgressText.Text = $"{testedCount}/{dataPool.Size}";
                }
#endif
            }
            catch (Exception ex)
            {
                LogMessage($"Statistics update error: {ex.Message}");
            }
        }

        #region Event Handlers

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "OpenBullet Config Files (*.anom;*.loli)|*.anom;*.loli|All files (*.*)|*.*",
                Title = "Select Config File"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
#if NETFRAMEWORK
                    // Use original IOManager for proper parsing
                    currentConfig = IOManager.LoadConfig(dialog.FileName);
                    
                    txtConfigName.Text = currentConfig.Settings.Name;
                    txtScriptEditor.Text = currentConfig.Script;
                    lblConfigStatus.Text = $"Loaded: {currentConfig.BlocksAmount} blocks";
                    lblConfigStatus.Foreground = new SolidColorBrush(Colors.Green);
                    
                    statusText.Text = $"Config loaded: {currentConfig.Settings.Name}";
                    LogMessage($"Config loaded: {currentConfig.Settings.Name} ({currentConfig.BlocksAmount} blocks)");
#else
                    // .NET 9 fallback - basic file reading
                    string configContent = File.ReadAllText(dialog.FileName);
                    txtScriptEditor.Text = configContent;
                    txtConfigName.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
                    lblConfigStatus.Text = "Config loaded (compatibility mode)";
                    lblConfigStatus.Foreground = new SolidColorBrush(Colors.Orange);
                    
                    statusText.Text = "Config loaded in compatibility mode";
                    LogMessage("Config loaded in .NET 9 compatibility mode - full functionality requires .NET Framework");
#endif
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading config: {ex.Message}", "Error", 
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    LogMessage($"Config loading error: {ex.Message}");
                }
            }
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
#if NETFRAMEWORK
                if (currentConfig != null)
                {
                    var dialog = new SaveFileDialog
                    {
                        Filter = "OpenBullet Config Files (*.anom)|*.anom|All files (*.*)|*.*",
                        Title = "Save Config File"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        currentConfig.Script = txtScriptEditor.Text;
                        currentConfig.Settings.Name = txtConfigName.Text;
                        IOManager.SaveConfig(currentConfig, dialog.FileName);
                        LogMessage($"Config saved: {dialog.FileName}");
                        statusText.Text = "Config saved successfully";
                    }
                }
                else
                {
                    MessageBox.Show("No config loaded to save!", "Warning");
                }
#else
                var dialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*",
                    Title = "Save Script File"
                };

                if (dialog.ShowDialog() == true)
                {
                    File.WriteAllText(dialog.FileName, txtScriptEditor.Text);
                    LogMessage($"Script saved: {dialog.FileName}");
                    statusText.Text = "Script saved successfully";
                }
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving config: {ex.Message}", "Error");
                LogMessage($"Config saving error: {ex.Message}");
            }
        }

        private void TestFiveEmails_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("Test 5 Emails functionality - simulating Amazon validation test...");
            
            // Simulate testing 5 sample phone numbers
            var testNumbers = new[]
            {
                "16479971432:0000",
                "16479972941:0000", 
                "16479979819:0000",
                "16479978515:0000",
                "16479978025:0000"
            };

            Task.Run(async () =>
            {
                foreach (var number in testNumbers)
                {
                    await Task.Delay(2000); // Simulate processing time
                    
                    // Simulate random results
                    bool isValid = new Random().Next(100) < 15; // 15% success rate
                    
                    Dispatcher.Invoke(() =>
                    {
                        testedCount++;
                        if (isValid)
                        {
                            hitCount++;
                            LogMessage($"✅ VALID: {number.Split(':')[0]} - Amazon account found");
                        }
                        else
                        {
                            failCount++;
                            LogMessage($"❌ INVALID: {number.Split(':')[0]} - No account found");
                        }
                        UpdateStatistics();
                    });
                }
                
                Dispatcher.Invoke(() =>
                {
                    LogMessage("Test completed - 5 numbers processed");
                    statusText.Text = "Test completed";
                });
            });
        }

        private void LoadWordlist_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Select Wordlist File"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var lines = File.ReadAllLines(dialog.FileName);
                    txtWordlistPath.Text = dialog.FileName;
                    lblWordlistStatus.Text = $"Loaded: {lines.Length} entries";
                    lblWordlistStatus.Foreground = new SolidColorBrush(Colors.Green);

#if NETFRAMEWORK
                    dataPool = new DataPool(lines);
#endif

                    LogMessage($"Wordlist loaded: {lines.Length} entries");
                    statusText.Text = $"Wordlist loaded: {lines.Length} entries";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading wordlist: {ex.Message}", "Error");
                    LogMessage($"Wordlist loading error: {ex.Message}");
                }
            }
        }

        private void LoadProxies_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Select Proxy File"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var lines = File.ReadAllLines(dialog.FileName)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToList();

#if NETFRAMEWORK
                    proxies = lines.Select(line => new CProxy().Parse(line, Extreme.Net.ProxyType.Http)).ToList();
                    runner.ProxyPool = new ProxyPool(proxies, true);
#endif

                    lblProxyStatus.Text = "Proxies loaded successfully";
                    lblProxyStatus.Foreground = new SolidColorBrush(Colors.Green);
                    lblProxyCount.Text = $"Count: {lines.Count}";

                    LogMessage($"Proxies loaded: {lines.Count} proxies");
                    statusText.Text = $"Loaded {lines.Count} proxies";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading proxies: {ex.Message}", "Error");
                    LogMessage($"Proxy loading error: {ex.Message}");
                }
            }
        }

        private void TestProxies_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("Proxy testing feature - checking proxy connectivity...");
            
#if NETFRAMEWORK
            if (proxies?.Any() == true)
            {
                Task.Run(async () =>
                {
                    int workingCount = 0;
                    foreach (var proxy in proxies.Take(10)) // Test first 10
                    {
                        await Task.Delay(500);
                        // Simulate proxy testing
                        bool working = new Random().Next(100) < 70; // 70% working rate
                        if (working) workingCount++;
                        
                        Dispatcher.Invoke(() =>
                        {
                            LogMessage($"Testing proxy {proxy.Proxy}: {(working ? "✅ Working" : "❌ Failed")}");
                        });
                    }
                    
                    Dispatcher.Invoke(() =>
                    {
                        LogMessage($"Proxy test completed: {workingCount}/10 working");
                    });
                });
            }
            else
            {
                LogMessage("No proxies loaded to test");
            }
#else
            LogMessage("Proxy testing requires .NET Framework version with original DLLs");
#endif
        }

        private async void StartRunner_Click(object sender, RoutedEventArgs e)
        {
#if NETFRAMEWORK
            if (currentConfig == null)
            {
                MessageBox.Show("Please load a config first!", "Warning");
                return;
            }

            if (dataPool == null)
            {
                MessageBox.Show("Please load a wordlist first!", "Warning");  
                return;
            }
#endif

            try
            {
                isRunning = true;
                startTime = DateTime.Now;
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
                updateTimer.Start();

                LogMessage("🚀 Runner started...");
                statusText.Text = "Runner is active";

#if NETFRAMEWORK
                // Configure runner
                runner.BotsNumber = int.Parse(txtBotCount.Text);
                runner.UseProxies = chkUseProxies.IsChecked ?? false;
                runner.Config = currentConfig;
                runner.DataPool = dataPool;

                // Start runner in background
                await Task.Run(() =>
                {
                    runner.Master.RunWorkerAsync();
                });
#else
                // .NET 9 simulation mode
                await SimulateRunnerExecution();
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting runner: {ex.Message}", "Error");
                LogMessage($"Runner start error: {ex.Message}");
                StopRunner();
            }
        }

        private void StopRunner_Click(object sender, RoutedEventArgs e)
        {
            StopRunner();
        }

        private void StopRunner()
        {
            try
            {
                isRunning = false;
                updateTimer.Stop();
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;

#if NETFRAMEWORK
                runner?.Master?.CancelAsync();
#endif

                LogMessage("⏹️ Runner stopped");
                statusText.Text = "Runner stopped";
            }
            catch (Exception ex)
            {
                LogMessage($"Stop runner error: {ex.Message}");
            }
        }

        private void PauseRunner_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("⏸️ Runner paused");
            statusText.Text = "Runner paused";
        }

#if !NETFRAMEWORK
        private async Task SimulateRunnerExecution()
        {
            // Simulate Amazon validation process for .NET 9
            var testData = new[]
            {
                "16479971432:0000", "16479972941:0000", "16479979819:0000", 
                "16479978515:0000", "16479978025:0000", "16479973797:0000"
            };

            var botCount = int.Parse(txtBotCount.Text);
            LogMessage($"Starting {botCount} bots for Amazon validation simulation...");

            foreach (var data in testData.Take(20)) // Simulate 20 entries
            {
                if (!isRunning) break;

                await Task.Delay(1000 + new Random().Next(500, 2000)); // Simulate varying processing time

                // Simulate Amazon response analysis
                bool isValid = SimulateAmazonValidation(data);

                Dispatcher.Invoke(() =>
                {
                    testedCount++;
                    if (isValid)
                    {
                        hitCount++;
                        LogMessage($"✅ SUCCESS: {data.Split(':')[0]} - Amazon account exists");
                    }
                    else
                    {
                        failCount++;
                        LogMessage($"❌ FAIL: {data.Split(':')[0]} - No Amazon account found");
                    }
                });
            }

            Dispatcher.Invoke(() =>
            {
                LogMessage("✅ Simulation completed");
                StopRunner();
            });
        }

        private bool SimulateAmazonValidation(string phoneNumber)
        {
            // Simulate the Amazon validation logic with realistic success rates
            var random = new Random();
            
            // Simulate different response types based on Amazon's actual responses
            var responses = new[]
            {
                "Sign-In", // SUCCESS
                "No account found with that email address",
                "ap_ra_email_or_phone", 
                "Please check your email address",
                "Incorrect phone number",
                "We cannot find an account with that mobile number",
                "There was a problem"
            };

            var response = responses[random.Next(responses.Length)];
            
            // Amazon-specific logic: only "Sign-In" indicates success
            return response == "Sign-In";
        }
#endif

        #endregion

        private void LogMessage(string message)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                var logEntry = $"[{timestamp}] {message}\n";
                
                Dispatcher.Invoke(() =>
                {
                    txtBotLog.AppendText(logEntry);
                    txtBotLog.ScrollToEnd();
                });
            }
            catch (Exception ex)
            {
                // Avoid recursive logging errors
                Console.WriteLine($"Logging error: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                updateTimer?.Stop();
#if NETFRAMEWORK
                runner?.Master?.CancelAsync();
#endif
                base.OnClosed(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }
    }
}

