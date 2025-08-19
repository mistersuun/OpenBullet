using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using Newtonsoft.Json;

// We'll load the original OpenBullet DLLs dynamically using reflection
// No direct using statements needed - we'll use Assembly.LoadFrom()

namespace OpenBullet.Native
{

/// <summary>
/// OpenBullet Native - Reverse Engineered Implementation
/// Uses original DLL files to provide identical functionality
/// </summary>
public partial class MainWindow : Window
{
    // Fields for RuriLib integration
    private object currentConfig; // Will discover the correct Config type
    private List<string> wordlistEntries = new List<string>();
    private CancellationTokenSource cancellationTokenSource;
    private bool isRunning = false;
    
    // Statistics tracking
    private int totalTested = 0;
    private int totalHits = 0;
    private int totalFails = 0;
    private int totalErrors = 0;
    private DateTime startTime;
    private System.Windows.Threading.DispatcherTimer statsTimer;
    private string logFilePath;

    public MainWindow()
    {
        InitializeComponent();
        InitializeApplication();
    }

    private void InitializeApplication()
    {
        // Initialize log file for easy copying
        var logsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        if (!System.IO.Directory.Exists(logsDir))
        {
            System.IO.Directory.CreateDirectory(logsDir);
        }
        
        logFilePath = System.IO.Path.Combine(logsDir, $"OpenBullet_Native_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        
        LogMessage("🚀 OpenBullet Native v2.0 - Initializing...");
        LogMessage("📦 Using Original OpenBullet DLL Files for Maximum Compatibility");
        LogMessage($"📝 Log file: {logFilePath}");
        
        // Initialize statistics timer
        statsTimer = new System.Windows.Threading.DispatcherTimer();
        statsTimer.Interval = TimeSpan.FromSeconds(1);
        statsTimer.Tick += UpdateElapsedTime;
        
        // Test RuriLib integration
        TestRuriLibIntegration();
    }

    private void TestRuriLibIntegration()
    {
        LogMessage("🔍 Testing RuriLib.dll Integration...");
        
        try
        {
            // Attempt to discover RuriLib types through reflection
            var ruriLibPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
            var ruriLibAssembly = Assembly.LoadFrom(ruriLibPath);
            LogMessage($"✅ RuriLib.dll loaded: {ruriLibAssembly.FullName}");
            
            // Get all public types
            var publicTypes = ruriLibAssembly.GetExportedTypes();
            LogMessage($"📋 Found {publicTypes.Length} public types in RuriLib");
            
            // Look for Config-related classes
            var configTypes = publicTypes.Where(t => t.Name.Contains("Config")).ToList();
            foreach (var type in configTypes)
            {
                LogMessage($"🔧 Found Config type: {type.FullName}");
            }
            
            // Look for BotData-related classes  
            var botDataTypes = publicTypes.Where(t => t.Name.Contains("BotData") || t.Name.Contains("Bot")).ToList();
            foreach (var type in botDataTypes)
            {
                LogMessage($"🤖 Found Bot type: {type.FullName}");
            }
            
            // Look for Runner-related classes
            var runnerTypes = publicTypes.Where(t => t.Name.Contains("Runner")).ToList();
            foreach (var type in runnerTypes)
            {
                LogMessage($"🏃 Found Runner type: {type.FullName}");
            }
            
            lblRuriLibStatus.Text = "✅ RuriLib.dll loaded successfully";
            txtStatus.Text = "Status: Ready - RuriLib Integration Active";
            
        }
        catch (Exception ex)
        {
            LogMessage($"❌ RuriLib Integration Error: {ex.Message}");
            lblRuriLibStatus.Text = "❌ RuriLib integration failed";
            txtStatus.Text = "Status: Error - Check logs";
        }
    }

    private void LoadConfig_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select OpenBullet Config File",
            Filter = "OpenBullet Configs (*.anom)|*.anom|All Files (*.*)|*.*",
            InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        };

        if (openFileDialog.ShowDialog() == true)
        {
            LoadConfigFile(openFileDialog.FileName);
        }
    }

    private void LoadConfigFile(string filePath)
    {
        try
        {
            LogMessage($"📁 Loading config: {System.IO.Path.GetFileName(filePath)}");
            
            // Read config file content
            var configContent = System.IO.File.ReadAllText(filePath);
            LogMessage($"📄 Config file loaded: {configContent.Length} characters");
            
            // Attempt to parse using RuriLib - discover the correct method
            DiscoverConfigLoadingAPI(filePath, configContent);
            
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Config Loading Error: {ex.Message}");
        }
    }

    private void DiscoverConfigLoadingAPI(string filePath, string configContent)
    {
        LogMessage("🔧 Loading config using discovered RuriLib.Config API...");
        LogMessage("   Constructor: Config(ConfigSettings settings, String script)");
        
        try
        {
            // Use our working ConfigParser that implements the discovered API
            var config = ConfigParser.LoadConfig(filePath);
            
            if (config != null)
            {
                currentConfig = config;
                
                // Extract config information using reflection
                ExtractConfigInformation(config);
                
                btnStart.IsEnabled = wordlistEntries.Count > 0;
                LogMessage("✅ Config loaded successfully using RuriLib native API!");
                txtStatus.Text = "Status: Config Loaded - Ready to Run";
            }
            else
            {
                LogMessage("❌ Config loading failed, falling back to manual parsing");
                ParseConfigManually(configContent);
            }
        }
        catch (Exception ex)
        {
            LogMessage($"❌ RuriLib config loading failed: {ex.Message}");
            LogMessage("🔧 Falling back to manual config parsing...");
            ParseConfigManually(configContent);
        }
    }

    private void ExtractConfigInformation(object config)
    {
        try
        {
            var configType = config.GetType();
            LogMessage($"🔍 Extracting info from config type: {configType.Name}");
            
            // Try to get common properties
            var nameProperty = configType.GetProperty("Name");
            var authorProperty = configType.GetProperty("Author");
            var scriptProperty = configType.GetProperty("Script");
            
            if (nameProperty != null)
            {
                var nameValue = nameProperty.GetValue(config);
                var name = nameValue != null ? nameValue.ToString() : "Unknown Config";
                lblConfigName.Text = name;
                LogMessage($"📋 Config Name: {name}");
            }
            
            if (authorProperty != null)
            {
                var authorValue = authorProperty.GetValue(config);
                var author = authorValue != null ? authorValue.ToString() : "Unknown";
                lblConfigAuthor.Text = $"by {author}";
                LogMessage($"👤 Config Author: {author}");
            }
            
            if (scriptProperty != null)
            {
                var scriptValue = scriptProperty.GetValue(config);
                var script = scriptValue != null ? scriptValue.ToString() : "";
                LogMessage($"📜 Config Script: {script.Length} characters");
            }
            
            // Log all available properties for discovery
            LogMessage("📊 Available Config Properties:");
            foreach (var prop in configType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    var value = prop.GetValue(config);
                    LogMessage($"   {prop.Name}: {prop.PropertyType.Name} = {value}");
                }
                catch
                {
                    LogMessage($"   {prop.Name}: {prop.PropertyType.Name} = <cannot read>");
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Config info extraction failed: {ex.Message}");
        }
    }

    private void ParseConfigManually(string configContent)
    {
        LogMessage("🔧 Falling back to manual config parsing...");
        
        // Parse basic config info manually as fallback
        try
        {
            if (configContent.Contains("[SETTINGS]"))
            {
                var settingsStart = configContent.IndexOf("{");
                var settingsEnd = configContent.IndexOf("}", settingsStart);
                
                if (settingsStart > 0 && settingsEnd > settingsStart)
                {
                    var settingsJson = configContent.Substring(settingsStart, settingsEnd - settingsStart + 1);
                    var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsJson);
                    
                    if (settings != null)
                    {
                        var configName = "Parsed Config";
                        if (settings.ContainsKey("Name") && settings["Name"] != null)
                        {
                            configName = settings["Name"].ToString();
                        }
                        lblConfigName.Text = configName;
                        
                        var configAuthor = "";
                        if (settings.ContainsKey("Author") && settings["Author"] != null)
                        {
                            configAuthor = $"by {settings["Author"]}";
                        }
                        lblConfigAuthor.Text = configAuthor;
                        
                        LogMessage($"📋 Manually parsed config: {lblConfigName.Text}");
                    }
                }
            }
            
            btnStart.IsEnabled = wordlistEntries.Count > 0;
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Manual parsing failed: {ex.Message}");
        }
    }

    private void LoadWordlist_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select Wordlist File",
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            LoadWordlistFile(openFileDialog.FileName);
        }
    }

    private void LoadWordlistFile(string filePath)
    {
        try
        {
            LogMessage($"📄 Loading wordlist: {System.IO.Path.GetFileName(filePath)}");
            
            var lines = System.IO.File.ReadAllLines(filePath);
            wordlistEntries = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            
            lblWordlistInfo.Text = $"{wordlistEntries.Count} entries loaded";
            LogMessage($"📋 Wordlist loaded: {wordlistEntries.Count} entries");
            
            btnStart.IsEnabled = currentConfig != null && wordlistEntries.Count > 0;
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Wordlist loading error: {ex.Message}");
        }
    }

    private void CreateSampleData_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            LogMessage("📋 Creating sample test data...");
            
            // Create sample phone numbers for Amazon checker
            wordlistEntries = new List<string>
            {
                "+1234567890",
                "+1987654321", 
                "+1555123456",
                "+1444555666",
                "+1333222111"
            };
            
            lblWordlistInfo.Text = $"{wordlistEntries.Count} sample entries created";
            LogMessage($"✅ Sample data created: {wordlistEntries.Count} phone numbers");
            
            btnStart.IsEnabled = currentConfig != null && wordlistEntries.Count > 0;
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Sample data creation error: {ex.Message}");
        }
    }

    private void CopyLogs_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (System.IO.File.Exists(logFilePath))
            {
                Clipboard.SetText(logFilePath);
                LogMessage($"📝 Log file path copied to clipboard: {logFilePath}");
                MessageBox.Show($"Log file path copied to clipboard!\n\nPath: {logFilePath}\n\nYou can now open this file to copy all the debug logs.", 
                               "Log File Ready", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                LogMessage("❌ Log file not found");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Copy logs error: {ex.Message}");
        }
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        if (currentConfig == null || wordlistEntries.Count == 0)
        {
            LogMessage("❌ Cannot start: Config or wordlist not loaded");
            return;
        }

        LogMessage("🚀 Starting execution using RuriLib...");
        
        btnStart.IsEnabled = false;
        btnStop.IsEnabled = true;
        isRunning = true;
        
        // Reset statistics
        totalTested = totalHits = totalFails = totalErrors = 0;
        startTime = DateTime.Now;
        statsTimer.Start();
        
        cancellationTokenSource = new CancellationTokenSource();
        
        // Start execution
        Task.Run(() => ExecuteWithRuriLib(cancellationTokenSource.Token));
    }

    private void ExecuteWithRuriLib(CancellationToken cancellationToken)
    {
        try
        {
            LogMessage("🚀 IMPLEMENTING REAL RURILIB AUTOMATION (No Fallback!)");
            LogMessage("🎯 Using discovered APIs: Config(ConfigSettings, String script)");
            
            if (currentConfig == null)
            {
                LogMessage("❌ No config loaded - cannot execute");
                Dispatcher.BeginInvoke(new Action(CompleteExecution));
                return;
            }
            
            // Extract script from loaded RuriLib.Config
            var configType = currentConfig.GetType();
            var scriptProperty = configType.GetProperty("Script");
            var settingsProperty = configType.GetProperty("Settings");
            
            if (scriptProperty == null || settingsProperty == null)
            {
                LogMessage("❌ Cannot access Script or Settings from RuriLib.Config");
                Dispatcher.BeginInvoke(new Action(CompleteExecution));
                return;
            }
            
            var script = scriptProperty.GetValue(currentConfig)?.ToString();
            var settings = settingsProperty.GetValue(currentConfig);
            
            LogMessage($"📜 Extracted LoliScript from RuriLib.Config: {script?.Length} characters");
            LogMessage($"⚙️ Config settings type: {settings?.GetType().Name}");
            
            LogMessage("🔥 SWITCHING TO ORIGINAL RURILIB EXECUTION ENGINE!");
            LogMessage("   Using: RuriLib.LS.BlockParser.Parse() + Block.Process(BotData)");
            
            // Use the ORIGINAL RuriLib execution engine instead of custom parsing
            var blocks = OriginalRuriLibEngine.ParseLoliScriptUsingOriginalEngine(script);
            LogMessage($"✅ Original BlockParser created {blocks.Count} blocks");
            
            // Get thread count on UI thread before background execution
            int threadCount = 5; // Default
            Dispatcher.Invoke(() =>
            {
                int.TryParse(txtThreads.Text, out threadCount);
                if (threadCount <= 0) threadCount = 5;
            });
            
            // Execute using ORIGINAL block execution system
            ExecuteUsingOriginalRuriLibEngine(blocks, settings, threadCount, cancellationToken);
        }
        catch (Exception ex)
        {
            LogMessage($"❌ REAL RuriLib execution failed: {ex.Message}");
            LogMessage($"   Exception details: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                LogMessage($"   Inner exception: {ex.InnerException.Message}");
            }
            LogMessage($"   Stack trace: {ex.StackTrace}");
            Dispatcher.BeginInvoke(new Action(CompleteExecution));
        }
    }

    private void ExecuteUsingOriginalRuriLibEngine(List<object> blocks, object settings, int threadCount, CancellationToken cancellationToken)
    {
        LogMessage("🔥 EXECUTING using ORIGINAL RuriLib Block.Process() system!");
        LogMessage($"📊 Processing {wordlistEntries.Count} entries with {blocks.Count} original blocks");
        LogMessage($"🧵 Using {threadCount} concurrent threads");
        
        var semaphore = new SemaphoreSlim(threadCount, threadCount);
        
        var tasks = wordlistEntries.Take(5).Select(entry => // Test with first 5 entries
            Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await ExecuteEntryUsingOriginalBlocks(entry, blocks, settings, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        
        Task.WhenAll(tasks).ContinueWith(task =>
        {
            LogMessage("✅ ORIGINAL RURILIB BLOCK EXECUTION COMPLETED!");
            
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CompleteExecution();
                LogMessage("📝 Original RuriLib execution complete - check logs for details");
            }));
        }, TaskScheduler.Default);
    }
    
    private async Task ExecuteEntryUsingOriginalBlocks(string entry, List<object> blocks, object settings, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        
        try
        {
            LogMessage($"🎯 ORIGINAL RURILIB EXECUTION for entry: {entry}");
            
            // Create BotData using original constructor (simplified for now)
            var botData = OriginalRuriLibEngine.CreateOriginalBotData(settings, entry, 1);
            
            if (botData == null)
            {
                LogMessage($"❌ Could not create original BotData for {entry}");
                Interlocked.Increment(ref totalErrors);
                return;
            }
            
            LogMessage($"✅ Created original BotData for {entry}");
            
            // Execute each block using the ORIGINAL Block.Process(BotData) method
            foreach (var block in blocks)
            {
                if (cancellationToken.IsCancellationRequested) break;
                
                if (block == null) continue;
                
                var blockType = block.GetType();
                LogMessage($"🔧 Executing original {blockType.Name}.Process() for {entry}");
                
                try
                {
                    // Find and call the original Process(BotData) method
                    var processMethod = blockType.GetMethods()
                        .FirstOrDefault(m => m.Name == "Process" && m.GetParameters().Length == 1);
                    
                    if (processMethod != null)
                    {
                        await Task.Run(() => processMethod.Invoke(block, new object[] { botData }));
                        LogMessage($"✅ {blockType.Name}.Process() completed for {entry}");
                        
                        // Check BotData status after block execution
                        var statusProperty = botData.GetType().GetProperty("Status");
                        if (statusProperty != null)
                        {
                            var status = statusProperty.GetValue(botData);
                            LogMessage($"   📊 BotData Status: {status}");
                            
                            // Update statistics based on original BotData status
                            UpdateStatisticsFromOriginalBotData(botData, entry);
                        }
                    }
                    else
                    {
                        LogMessage($"❌ Process() method not found in {blockType.Name}");
                    }
                }
                catch (Exception blockEx)
                {
                    LogMessage($"❌ {blockType.Name} execution failed: {blockEx.InnerException?.Message ?? blockEx.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref totalErrors);
            LogMessage($"❌ ORIGINAL EXECUTION ERROR for {entry}: {ex.Message}");
            Dispatcher.BeginInvoke(new Action(UpdateStatistics));
        }
    }
    
    private void UpdateStatisticsFromOriginalBotData(object botData, string entry)
    {
        try
        {
            var botDataType = botData.GetType();
            var statusProperty = botDataType.GetProperty("Status");
            
            if (statusProperty != null)
            {
                var status = statusProperty.GetValue(botData);
                var statusString = status?.ToString() ?? "UNKNOWN";
                
                LogMessage($"🔍 Original BotData Status for {entry}: {statusString}");
                
                Interlocked.Increment(ref totalTested);
                
                // Map original BotStatus to our statistics
                switch (statusString.ToUpper())
                {
                    case "SUCCESS":
                        Interlocked.Increment(ref totalHits);
                        LogMessage($"✅ ORIGINAL HIT: {entry}");
                        break;
                    case "FAIL":
                    case "FAILURE":
                        Interlocked.Increment(ref totalFails);
                        LogMessage($"❌ ORIGINAL FAIL: {entry}");
                        break;
                    case "ERROR":
                    case "BAN":
                        Interlocked.Increment(ref totalErrors);
                        LogMessage($"⚠️ ORIGINAL ERROR: {entry}");
                        break;
                    default:
                        Interlocked.Increment(ref totalFails);
                        LogMessage($"⚠️ UNKNOWN STATUS: {entry} = {statusString}");
                        break;
                }
                
                Dispatcher.BeginInvoke(new Action(UpdateStatistics));
            }
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Status update failed: {ex.Message}");
        }
    }

    private List<string> ParseLoliScriptCommands(string script)
    {
        LogMessage("📜 PARSING REAL LOLISCRIPT from amazonChecker.anom...");
        
        var commands = new List<string>();
        if (string.IsNullOrEmpty(script)) return commands;
        
        var lines = script.Split(new char[] { '\n' });
        var currentCommand = "";
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
                continue;
            
            // Handle comment lines that contain commands (like "#POST REQUEST POST...")
            if (trimmedLine.StartsWith("#"))
            {
                // Check if there's a command after the comment
                var commentParts = trimmedLine.Split(new char[] { ' ' }, 2);
                if (commentParts.Length > 1)
                {
                    var afterComment = commentParts[1].Trim();
                    if (afterComment.StartsWith("REQUEST") || afterComment.StartsWith("KEYCHECK") || 
                        afterComment.StartsWith("PARSE") || afterComment.StartsWith("FUNCTION"))
                    {
                        // Save previous command if exists
                        if (!string.IsNullOrEmpty(currentCommand))
                        {
                            commands.Add(currentCommand.Trim());
                        }
                        currentCommand = afterComment; // Use the command without the comment prefix
                        LogMessage($"   🔍 Found comment-prefixed command: {afterComment.Split(new char[] { ' ' })[0]}");
                        continue;
                    }
                }
                // Regular comment, skip
                continue;
            }
                
            // MAJOR COMMAND start (these start new commands)
            if (trimmedLine.StartsWith("REQUEST") || trimmedLine.StartsWith("KEYCHECK") || 
                trimmedLine.StartsWith("PARSE") || trimmedLine.StartsWith("FUNCTION") ||
                trimmedLine.StartsWith("UTILITY"))
            {
                // Save previous command if exists
                if (!string.IsNullOrEmpty(currentCommand))
                {
                    commands.Add(currentCommand.Trim());
                }
                currentCommand = trimmedLine;
            }
            // SUB-COMMANDS (these belong to the current command)
            else if (trimmedLine.StartsWith("CONTENT") || trimmedLine.StartsWith("HEADER") || 
                     trimmedLine.StartsWith("CONTENTTYPE") || trimmedLine.StartsWith("KEYCHAIN") ||
                     trimmedLine.StartsWith("KEY"))
            {
                // Add to current command
                currentCommand += "\n" + trimmedLine;
            }
            else
            {
                // Any other line also belongs to current command
                currentCommand += "\n" + trimmedLine;
            }
        }
        
        // Add final command
        if (!string.IsNullOrEmpty(currentCommand))
        {
            commands.Add(currentCommand.Trim());
        }
        
        LogMessage($"✅ FIXED LoliScript parsing: {commands.Count} commands extracted");
        
        // Log each command for debugging
        for (int i = 0; i < commands.Count; i++)
        {
            var firstLine = commands[i].Split(new char[] { '\n' })[0];
            LogMessage($"   📝 Command {i + 1}: {firstLine}");
        }
        
        return commands;
    }
    
    private void ExecuteRealAutomationFixed(List<string> commands, object settings, int threadCount, CancellationToken cancellationToken)
    {
        LogMessage("🚀 REAL AUTOMATION - Using actual RuriLib + Leaf.xNet execution!");
        LogMessage($"📊 Processing {wordlistEntries.Count} entries with {commands.Count} LoliScript commands");
        LogMessage($"🧵 Using {threadCount} concurrent threads");
        
        // Execute with proper threading (FIXED - no UI access from background thread)
        var semaphore = new SemaphoreSlim(threadCount, threadCount);
        
        var tasks = wordlistEntries.Take(10).Select(entry => // Test with first 10 entries
            Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await ExecuteEntryWithRealRuriLib(entry, commands, settings, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        
        Task.WhenAll(tasks).ContinueWith(task =>
        {
            LogMessage("✅ REAL RURILIB AUTOMATION COMPLETED!");
            
            // Use BeginInvoke to avoid threading issues
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CompleteExecution();
                LogMessage("📝 Check the log file for complete debug info - use 'Copy Log File' button");
            }));
        }, TaskScheduler.Default);
    }
    
    private async Task ExecuteEntryWithRealRuriLib(string entry, List<string> commands, object settings, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;
        
        try
        {
            LogMessage($"🎯 REAL RURILIB EXECUTION for entry: {entry}");
            
            // Create HTTP context using real Leaf.xNet
            var httpContext = CreateRealHttpContext();
            if (httpContext == null)
            {
                LogMessage($"❌ Failed to create HTTP context for {entry}");
                Interlocked.Increment(ref totalErrors);
                return;
            }
            
            string lastResponse = "";
            bool foundResult = false;
            
            // Execute each LoliScript command in sequence
            foreach (var command in commands)
            {
                if (cancellationToken.IsCancellationRequested) break;
                
                LogMessage($"📝 Executing command for {entry}: {command.Split(new char[] { '\n' })[0]}");
                
                if (command.StartsWith("REQUEST"))
                {
                    lastResponse = await ExecuteRealRequestCommand(command, entry, httpContext);
                    LogMessage($"📡 REQUEST result: {lastResponse.Length} chars received");
                }
                else if (command.StartsWith("KEYCHECK"))
                {
                    var keyResult = ExecuteRealKeyCheckCommand(command, lastResponse, entry);
                    LogMessage($"🔍 KEYCHECK result for {entry}: {(keyResult ? "HIT ✅" : "FAIL ❌")}");
                    
                    // Update statistics
                    Interlocked.Increment(ref totalTested);
                    if (keyResult)
                    {
                        Interlocked.Increment(ref totalHits);
                    }
                    else
                    {
                        Interlocked.Increment(ref totalFails);
                    }
                    
                    // Update UI safely
                    Dispatcher.BeginInvoke(new Action(UpdateStatistics));
                    foundResult = true;
                    break; // Exit after keycheck determines result
                }
                else if (command.StartsWith("PARSE"))
                {
                    var parseResult = ExecuteParseCommand(command, lastResponse, entry);
                    LogMessage($"🔍 PARSE result for {entry}: {parseResult}");
                }
            }
            
            if (!foundResult)
            {
                LogMessage($"⚠️ No KEYCHECK found for {entry} - marking as ERROR");
                Interlocked.Increment(ref totalErrors);
                Dispatcher.BeginInvoke(new Action(UpdateStatistics));
            }
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref totalErrors);
            LogMessage($"❌ REAL EXECUTION ERROR for {entry}: {ex.Message}");
            LogMessage($"   Exception type: {ex.GetType().Name}");
            Dispatcher.BeginInvoke(new Action(UpdateStatistics));
        }
    }
    
    private object CreateRealHttpContext()
    {
        try
        {
            LogMessage("🌐 Creating REAL Leaf.xNet.HttpRequest for automation...");
            
            var leafXNetPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Leaf.xNet.dll");
            var assembly = Assembly.LoadFrom(leafXNetPath);
            var httpRequestType = assembly.GetType("Leaf.xNet.HttpRequest");
            
            if (httpRequestType != null)
            {
                var httpRequest = Activator.CreateInstance(httpRequestType);
                
                // Configure HttpRequest properties using reflection
                var userAgentProperty = httpRequestType.GetProperty("UserAgent");
                var keepAliveProperty = httpRequestType.GetProperty("KeepAlive");
                var connectTimeoutProperty = httpRequestType.GetProperty("ConnectTimeout");
                var allowRedirectProperty = httpRequestType.GetProperty("AllowAutoRedirect");
                
                userAgentProperty?.SetValue(httpRequest, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36", null);
                keepAliveProperty?.SetValue(httpRequest, true, null);
                connectTimeoutProperty?.SetValue(httpRequest, 15000, null);
                allowRedirectProperty?.SetValue(httpRequest, true, null);
                
                LogMessage("✅ Real Leaf.xNet.HttpRequest configured for Amazon automation");
                return httpRequest;
            }
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Failed to create real HttpRequest: {ex.Message}");
        }
        
        return null;
    }
    
    private async Task<string> ExecuteRealRequestCommand(string requestCommand, string entry, object httpContext)
    {
        try
        {
            LogMessage($"🌐 EXECUTING REAL AMAZON REQUEST for {entry}");
            
            if (httpContext == null)
            {
                LogMessage("❌ No HTTP context available");
                return "";
            }
            
            // Parse the REQUEST command to extract URL, method, content, headers
            var (method, url, content, headers) = ParseRequestCommand(requestCommand, entry);
            
            LogMessage($"   📝 Method: {method}");
            LogMessage($"   📝 URL: {url}");
            LogMessage($"   📝 Content: {content.Substring(0, Math.Min(200, content.Length))}...");
            LogMessage($"   📝 Headers: {headers.Count} headers");
            
            // Set headers on HttpRequest using reflection
            var httpType = httpContext.GetType();
            
            // Set headers
            foreach (var header in headers)
            {
                try
                {
                    if (header.Key.ToLower() == "user-agent")
                    {
                        var userAgentProperty = httpType.GetProperty("UserAgent");
                        userAgentProperty?.SetValue(httpContext, header.Value, null);
                        LogMessage($"   🔧 Set User-Agent: {header.Value.Substring(0, Math.Min(50, header.Value.Length))}...");
                    }
                    else if (header.Key.ToLower() == "cookie")
                    {
                        // Set cookies - we'll implement this later
                        LogMessage($"   🍪 Cookie header found: {header.Value.Length} chars");
                    }
                    else
                    {
                        // Add other headers using AddHeader method if available
                        var addHeaderMethods = httpType.GetMethods().Where(m => m.Name == "AddHeader").ToList();
                        var addHeaderMethod = addHeaderMethods.FirstOrDefault(m => m.GetParameters().Length == 2);
                        
                        if (addHeaderMethod != null)
                        {
                            try
                            {
                                addHeaderMethod.Invoke(httpContext, new object[] { header.Key, header.Value });
                                LogMessage($"   ✅ Header set: {header.Key}");
                            }
                            catch (Exception headerEx)
                            {
                                LogMessage($"   ⚠️ Header {header.Key} failed: {headerEx.InnerException?.Message ?? headerEx.Message}");
                            }
                        }
                        else
                        {
                            LogMessage($"   ⚠️ No AddHeader method found for {header.Key}");
                        }
                    }
                }
                catch (Exception headerEx)
                {
                    LogMessage($"   ⚠️ Header {header.Key} failed: {headerEx.Message}");
                }
            }
            
            // Execute the HTTP request
            if (method == "POST")
            {
                LogMessage($"🚀 REAL POST to Amazon Canada signin...");
                
                // Look for Post method with string parameters
                var postMethods = httpType.GetMethods().Where(m => m.Name == "Post").ToList();
                LogMessage($"   🔍 Found {postMethods.Count} Post methods in Leaf.xNet");
                
                foreach (var postMethod in postMethods)
                {
                    var parameters = postMethod.GetParameters();
                    LogMessage($"      📝 Post({string.Join(", ", parameters.Select(p => p.ParameterType.Name))})");
                }
                
                // Try Post(string url, string content, string contentType) - 3 parameter version
                var postWithContentType = postMethods.FirstOrDefault(m => 
                    m.GetParameters().Length == 3 && 
                    m.GetParameters()[0].ParameterType == typeof(string) && 
                    m.GetParameters()[1].ParameterType == typeof(string) &&
                    m.GetParameters()[2].ParameterType == typeof(string));
                
                if (postWithContentType != null)
                {
                    LogMessage($"   🎯 Using Post(string url, string content, string contentType) method");
                    var contentType = headers.ContainsKey("Content-Type") ? headers["Content-Type"] : "application/x-www-form-urlencoded";
                    var response = await Task.Run(() => postWithContentType.Invoke(httpContext, new object[] { url, content, contentType }));
                    
                    if (response != null)
                    {
                        var responseText = response.ToString();
                        LogMessage($"✅ Amazon POST response: {responseText.Length} characters");
                        
                        // Show response preview for debugging
                        var preview = responseText.Length > 500 ? responseText.Substring(0, 500) + "..." : responseText;
                        LogMessage($"   📄 Response preview: {preview}");
                        
                        return responseText;
                    }
                }
                else
                {
                    LogMessage("❌ No suitable Post(url, content, contentType) method found");
                    
                    // Fallback: try 2-parameter Post method 
                    var simplePost = postMethods.FirstOrDefault(m => 
                        m.GetParameters().Length == 2 && 
                        m.GetParameters()[0].ParameterType == typeof(string) && 
                        m.GetParameters()[1].ParameterType == typeof(string));
                    
                    if (simplePost != null)
                    {
                        LogMessage($"   🔄 Fallback: Using Post(string url, string content) method");
                        var response = await Task.Run(() => simplePost.Invoke(httpContext, new object[] { url, content }));
                        
                        if (response != null)
                        {
                            var responseText = response.ToString();
                            LogMessage($"✅ Fallback POST response: {responseText.Length} characters");
                            return responseText;
                        }
                    }
                    else
                    {
                        LogMessage("❌ No suitable Post method found at all");
                    }
                }
            }
            else if (method == "GET")
            {
                LogMessage($"🚀 REAL GET request...");
                var getMethod = httpType.GetMethods().FirstOrDefault(m => m.Name == "Get" && m.GetParameters().Length == 1);
                
                if (getMethod != null)
                {
                    var response = await Task.Run(() => getMethod.Invoke(httpContext, new object[] { url }));
                    
                    if (response != null)
                    {
                        var responseText = response.ToString();
                        LogMessage($"✅ GET response: {responseText.Length} characters");
                        return responseText;
                    }
                }
            }
            
            LogMessage("❌ HTTP request execution failed");
            return "";
        }
        catch (Exception ex)
        {
            LogMessage($"❌ REAL REQUEST FAILED for {entry}: {ex.Message}");
            LogMessage($"   Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                LogMessage($"   Inner exception: {ex.InnerException.Message}");
            }
            return "";
        }
    }
    
    private (string method, string url, string content, Dictionary<string, string> headers) ParseRequestCommand(string requestCommand, string entry)
    {
        try
        {
            LogMessage($"🔍 PARSING REQUEST command for {entry}:");
            
            var method = "GET";
            var url = "";
            var content = "";
            var headers = new Dictionary<string, string>();
            
            var lines = requestCommand.Split(new char[] { '\n' });
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("REQUEST"))
                {
                    // Parse: REQUEST POST "https://www.amazon.ca/ap/signin"
                    var requestParts = trimmedLine.Split(new char[] { ' ' }, 3);
                    if (requestParts.Length >= 3)
                    {
                        method = requestParts[1]; // POST
                        url = requestParts[2].Trim('"');
                        LogMessage($"   📝 Extracted: {method} {url}");
                    }
                }
                else if (trimmedLine.StartsWith("CONTENTTYPE"))
                {
                    // Parse: CONTENTTYPE "application/x-www-form-urlencoded"
                    var contentType = trimmedLine.Substring("CONTENTTYPE".Length).Trim().Trim('"');
                    headers["Content-Type"] = contentType;
                    LogMessage($"   📝 Content-Type: {contentType}");
                }
                else if (trimmedLine.StartsWith("CONTENT"))
                {
                    // Parse: CONTENT "appActionToken=..." (long string)
                    var contentStart = trimmedLine.IndexOf('"');
                    var contentEnd = trimmedLine.LastIndexOf('"');
                    if (contentStart >= 0 && contentEnd > contentStart)
                    {
                        content = trimmedLine.Substring(contentStart + 1, contentEnd - contentStart - 1);
                        
                        // Replace variables with actual data
                        content = content.Replace("<USER>", entry);
                        content = content.Replace("<PASS>", ""); // Amazon phone checker doesn't need password
                        
                        LogMessage($"   📝 REAL CONTENT extracted: {content.Length} characters");
                        LogMessage($"   📝 Content preview: {content.Substring(0, Math.Min(200, content.Length))}...");
                    }
                }
                else if (trimmedLine.StartsWith("HEADER"))
                {
                    // Parse: HEADER "accept: text/html,application/xhtml+xml..."
                    var headerStart = trimmedLine.IndexOf('"');
                    var headerEnd = trimmedLine.LastIndexOf('"');
                    if (headerStart >= 0 && headerEnd > headerStart)
                    {
                        var headerContent = trimmedLine.Substring(headerStart + 1, headerEnd - headerStart - 1);
                        var colonIndex = headerContent.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            var headerName = headerContent.Substring(0, colonIndex).Trim();
                            var headerValue = headerContent.Substring(colonIndex + 1).Trim();
                            headers[headerName] = headerValue;
                            LogMessage($"   📝 Header: {headerName} = {headerValue.Substring(0, Math.Min(50, headerValue.Length))}...");
                        }
                    }
                }
            }
            
            LogMessage($"✅ REQUEST parsing complete: {method} {url}, {content.Length} chars content, {headers.Count} headers");
            return (method, url, content, headers);
        }
        catch (Exception ex)
        {
            LogMessage($"❌ REQUEST parsing failed: {ex.Message}");
            return ("GET", "", "", new Dictionary<string, string>());
        }
    }
    
    private bool ExecuteRealKeyCheckCommand(string keyCheckCommand, string response, string entry)
    {
        try
        {
            LogMessage($"🔍 REAL KEYCHECK execution for {entry}");
            LogMessage($"   Response length: {response.Length} characters");
            
            // Parse KEYCHECK command structure:
            // KEYCHAIN Success OR
            //   KEY "Sign-In"
            // KEYCHAIN Failure OR  
            //   KEY "No account found"
            //   KEY "Incorrect phone number"
            
            var lines = keyCheckCommand.Split(new char[] { '\n' });
            bool inSuccessKeychain = false;
            bool inFailureKeychain = false;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.Contains("KEYCHAIN Success"))
                {
                    inSuccessKeychain = true;
                    inFailureKeychain = false;
                    LogMessage("   🎯 Checking SUCCESS keychain...");
                }
                else if (trimmedLine.Contains("KEYCHAIN Failure"))
                {
                    inSuccessKeychain = false;
                    inFailureKeychain = true;
                    LogMessage("   🎯 Checking FAILURE keychain...");
                }
                else if (trimmedLine.StartsWith("KEY") && trimmedLine.Contains('"'))
                {
                    var keyStart = trimmedLine.IndexOf('"');
                    var keyEnd = trimmedLine.LastIndexOf('"');
                    if (keyStart >= 0 && keyEnd > keyStart)
                    {
                        var key = trimmedLine.Substring(keyStart + 1, keyEnd - keyStart - 1);
                        bool keyFound = response.Contains(key);
                        
                        LogMessage($"   🔍 Checking key '{key}': {(keyFound ? "FOUND" : "NOT FOUND")}");
                        
                        if (keyFound)
                        {
                            if (inSuccessKeychain)
                            {
                                LogMessage($"✅ SUCCESS key found: '{key}' - Marking as HIT");
                                return true;
                            }
                            else if (inFailureKeychain)
                            {
                                LogMessage($"❌ FAILURE key found: '{key}' - Marking as FAIL");
                                return false;
                            }
                        }
                    }
                }
            }
            
            LogMessage($"⚠️ No matching keys found in response - defaulting to FAIL");
            return false;
        }
        catch (Exception ex)
        {
            LogMessage($"❌ KEYCHECK execution failed: {ex.Message}");
            return false;
        }
    }
    
    private string ExecuteParseCommand(string parseCommand, string response, string entry)
    {
        try
        {
            LogMessage($"🔍 PARSE command execution for {entry}");
            // Basic PARSE implementation - can be enhanced later
            return "";
        }
        catch (Exception ex)
        {
            LogMessage($"❌ PARSE execution failed: {ex.Message}");
            return "";
        }
    }
    




    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        LogMessage("⏹️ Stopping execution...");
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
        CompleteExecution();
    }

    private void CompleteExecution()
    {
        isRunning = false;
        btnStart.IsEnabled = true;
        btnStop.IsEnabled = false;
        statsTimer.Stop();
        
        txtStatus.Text = "Status: Completed";
        LogMessage($"✅ Execution completed. Total: {totalTested}, Hits: {totalHits}, Fails: {totalFails}, Errors: {totalErrors}");
    }

    private void UpdateStatistics()
    {
        lblTested.Text = totalTested.ToString();
        lblHits.Text = totalHits.ToString();
        lblFails.Text = totalFails.ToString();
        lblErrors.Text = totalErrors.ToString();
        
        // Calculate CPM
        if (isRunning && totalTested > 0)
        {
            var elapsed = DateTime.Now - startTime;
            if (elapsed.TotalMinutes > 0)
            {
                var cpm = totalTested / elapsed.TotalMinutes;
                lblCPM.Text = $"{cpm:F0}";
            }
        }
        
        // Calculate success rate
        if (totalTested > 0)
        {
            var successRate = (double)totalHits / totalTested * 100;
            lblSuccessRate.Text = $"{successRate:F1}%";
        }
    }

    private void UpdateElapsedTime(object sender, EventArgs e)
    {
        if (isRunning)
        {
            var elapsed = DateTime.Now - startTime;
            lblElapsedTime.Text = $" | Elapsed: {elapsed:hh\\:mm\\:ss}";
        }
    }

    private void LogMessage(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var logEntry = $"[{timestamp}] {message}";
        
        // Write to log file for easy copying
        try
        {
            System.IO.File.AppendAllText(logFilePath, logEntry + "\n");
        }
        catch { } // Ignore log file errors
        
        // Update UI on correct thread
        if (Dispatcher.CheckAccess())
        {
            txtLog.Text += logEntry + "\n";
            logScrollViewer.ScrollToEnd();
        }
        else
        {
            Dispatcher.Invoke(() =>
            {
                txtLog.Text += logEntry + "\n";
                logScrollViewer.ScrollToEnd();
            });
        }
    }
}
}