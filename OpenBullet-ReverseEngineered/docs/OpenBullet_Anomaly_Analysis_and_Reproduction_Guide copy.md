# OpenBullet 1.4.4 Anomaly Modded Version - Complete Analysis & Reproduction Guide

## Table of Contents
1. [Overview](#overview)
2. [Architecture Analysis](#architecture-analysis)
3. [Core Components](#core-components)
4. [Dependencies & Libraries](#dependencies--libraries)
5. [Configuration System](#configuration-system)
6. [LoliScript Language](#loliscript-language)
7. [Database Structure](#database-structure)
8. [Execution Flow](#execution-flow)
9. [Reproduction Guide](#reproduction-guide)
10. [Technical Implementation Details](#technical-implementation-details)

## Overview

OpenBullet 1.4.4 Anomaly is a sophisticated multi-threaded web automation tool designed for credential checking, data scraping, and web testing. This modded version includes enhanced features and custom modifications beyond the standard OpenBullet release.

**Key Features:**
- Multi-threaded execution engine
- Custom LoliScript scripting language
- Proxy rotation and management
- CAPTCHA solving integration
- Web browser automation (Selenium)
- Configuration management system
- Real-time monitoring and statistics
- Database-backed storage
- Plugin architecture

## Architecture Analysis

### Application Structure
```
OpenBullet.exe (Main Application)
├── bin/ (Dependencies & Libraries)
├── Configs/ (Configuration Files)
├── Wordlists/ (Input Data)
├── DB/ (Database Files)
├── Settings/ (Application Settings)
├── Captchas/ (CAPTCHA Processing)
├── Screenshots/ (Browser Screenshots)
├── Sounds/ (Audio Notifications)
├── ChromeExtensions/ (Browser Extensions)
└── x64/, x86/ (Platform-specific libraries)
```

### Technology Stack
- **Platform**: .NET Framework (C#/WPF)
- **Database**: LiteDB (NoSQL document database)
- **Scripting**: Custom LoliScript + IronPython support
- **UI Framework**: WPF with AvalonEdit
- **Web Automation**: Selenium WebDriver
- **HTTP Client**: Custom HTTP library (Leaf.xNet)
- **JSON Processing**: Newtonsoft.Json
- **HTML Parsing**: AngleSharp

## Core Components

### 1. Main Executable (`OpenBullet.exe`)
- Primary application entry point
- WPF-based user interface
- Multi-threading coordinator
- Configuration manager

### 2. Configuration Files (`.anom` format)
**Structure Example:**
```
[SETTINGS]
{
  "Name": "Amazon Phone Checker",
  "Author": "saisu",
  "Version": "1.3.6 [Anomaly]",
  "SuggestedBots": 100,
  "NeedsProxies": true,
  "MaxRedirects": 8
}

[SCRIPT]
#POST REQUEST POST "https://www.amazon.ca/ap/signin" 
  CONTENT "email=<USER>&password=<PASS>&..."
  HEADER "User-Agent: Mozilla/5.0..."
  
KEYCHECK 
  KEYCHAIN Failure OR 
    KEY "No account found"
    KEY "Incorrect phone number"
  KEYCHAIN Success OR 
    KEY "Sign-In"
```

### 3. Settings Management
**OBSettings.json** - OpenBullet Application Settings:
```json
{
  "General": {
    "DisplayLoliScriptOnLoad": false,
    "RecommendedBots": true,
    "LiveConfigUpdates": false
  },
  "Themes": {
    "BackgroundMain": "#222",
    "ForegroundGood": "#adff2f",
    "ForegroundBad": "#ff6347"
  },
  "Sounds": {
    "EnableSounds": false,
    "OnHitSound": "rifle_hit.wav"
  }
}
```

**RLSettings.json** - RuriLib Engine Settings:
```json
{
  "General": {
    "WaitTime": 100,
    "RequestTimeout": 10,
    "MaxHits": 0
  },
  "Proxies": {
    "ConcurrentUse": false,
    "ShuffleOnStart": false,
    "ReloadInterval": 0
  },
  "Captchas": {
    "CurrentService": 0,
    "Timeout": 120
  },
  "Selenium": {
    "Browser": 0,
    "Headless": false,
    "PageLoadTimeout": 60
  }
}
```

## Dependencies & Libraries

### Core Libraries
1. **RuriLib.dll** - Main execution engine
2. **Leaf.xNet.dll** - HTTP client library
3. **AngleSharp.dll** - HTML/CSS parsing
4. **LiteDB.dll** - NoSQL database
5. **Newtonsoft.Json.dll** - JSON serialization

### Scripting Support
1. **IronPython.dll** - Python scripting support
2. **Microsoft.Scripting.dll** - Dynamic language runtime
3. **Jint.dll** - JavaScript engine

### UI Components
1. **ICSharpCode.AvalonEdit.dll** - Code editor
2. **Xceed.Wpf.Toolkit.dll** - Extended WPF controls
3. **WPFToolkit.dll** - Additional WPF components

### Web Automation
1. **WebDriver.dll** - Selenium WebDriver
2. **WebDriver.Support.dll** - Selenium utilities

### CAPTCHA & OCR
1. **Tesseract.dll** - OCR engine
2. **liblept*.dll** - Image processing libraries

### Security & Networking
1. **ProxySocket.dll** - Proxy connection handling
2. **System.Net.Http.dll** - HTTP client
3. **System.Security.Cryptography.*.dll** - Encryption libraries

## Configuration System

### Wordlist Types (Environment.ini)
```ini
[WLTYPE]
Name=MailPass
Regex=^*.@.*:.*$
Separator=:
Slices=USER,PASS

[WLTYPE]
Name=UserPass
Regex=^*.:.*$
Separator=:
Slices=USER,PASS

[CUSTOMKC]
Name=FREE
Color=OrangeRed

[EXPFORMAT]
Format=<DATA>:<PROXY>:<CAPTURE>
```

### Configuration Elements
- **Settings Section**: Metadata, author info, execution parameters
- **Script Section**: LoliScript code for automation logic
- **Custom Inputs**: Dynamic input fields
- **Data Rules**: Input validation and processing rules

## LoliScript Language

### Syntax Overview
LoliScript is a domain-specific language designed for web automation:

```loliscript
# Comments start with #
FUNCTION Constant "Hello World" -> VAR "greeting"

# HTTP Requests
REQUEST GET "https://example.com"
  HEADER "User-Agent: OpenBullet"
  -> STRING

# Data Parsing
PARSE "<SOURCE>" LR "left_bound" "right_bound" -> CAP "result"

# Key Checking
KEYCHECK
  KEYCHAIN Success OR
    KEY "Welcome"
    KEY "Login successful"
  KEYCHAIN Failure OR
    KEY "Error"
    KEY "Invalid"
```

### Available Commands
1. **FUNCTION** - Execute built-in functions
2. **REQUEST** - HTTP/HTTPS requests
3. **PARSE** - Data extraction (Regex, CSS, JSON, LR)
4. **KEYCHECK** - Response validation
5. **UTILITY** - Helper functions
6. **CAPTCHA/RECAPTCHA** - CAPTCHA solving
7. **NAVIGATE** - Browser navigation
8. **BROWSERACTION** - Browser interactions

### Syntax Highlighting
The application includes custom syntax highlighting defined in `LSHighlighting.xshd`:
- **Green**: FUNCTION commands
- **Blue**: KEYCHECK commands
- **Plum**: KEYCHAIN commands
- **Yellow**: Variables and data
- **Gray**: Comments

## Database Structure

### LiteDB Implementation
- **File**: `OpenBullet.db` (8KB base size)
- **Type**: NoSQL document database
- **Backup**: `OpenBullet-BackupCopy.db`

### Stored Data
- Configuration metadata
- Execution results
- Proxy lists
- Hit/fail statistics
- User preferences
- Session data

## Execution Flow

### 1. Application Startup
```
OpenBullet.exe Launch
├── Load Settings (OBSettings.json, RLSettings.json)
├── Initialize Database (LiteDB)
├── Load Configurations
├── Setup UI Theme
└── Ready for Operations
```

### 2. Configuration Execution
```
Load .anom Config
├── Parse Settings Section
├── Validate Script Section
├── Load Wordlist Data
├── Setup Proxy Rotation
├── Initialize Threads
└── Execute LoliScript
    ├── HTTP Requests
    ├── Data Parsing
    ├── Key Checking
    └── Result Classification
```

### 3. Result Processing
```
Script Execution Result
├── SUCCESS → Save to Hits
├── FAILURE → Increment Fails
├── RETRY → Queue for Retry
├── BAN → Handle Proxy Ban
└── CUSTOM → Process Custom Status
```

## Reproduction Guide

### Prerequisites
1. **.NET Framework 4.7.2+**
2. **Visual Studio 2019+** or **JetBrains Rider**
3. **Windows 10/11** (primary platform)

### Development Environment Setup

#### 1. Create Base Project Structure
```csharp
// Solution Structure
OpenBullet.sln
├── OpenBullet (WPF Application)
├── RuriLib (Class Library)
├── OpenBullet.Updater (Console Application)
└── PluginFramework (Class Library)
```

#### 2. Install Required NuGet Packages
```xml
<PackageReference Include="AngleSharp" Version="0.14.0" />
<PackageReference Include="LiteDB" Version="5.0.11" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
<PackageReference Include="Selenium.WebDriver" Version="4.1.0" />
<PackageReference Include="IronPython" Version="2.7.11" />
<PackageReference Include="AvalonEdit" Version="6.1.3.50" />
<PackageReference Include="Extended.Wpf.Toolkit" Version="4.2.0" />
```

#### 3. Core Application Architecture

**Main Window (WPF)**
```csharp
public partial class MainWindow : Window
{
    private RunnerManager runnerManager;
    private ConfigManager configManager;
    private DatabaseService dbService;
    
    public MainWindow()
    {
        InitializeComponent();
        InitializeServices();
        LoadSettings();
        SetupTheme();
    }
    
    private void InitializeServices()
    {
        dbService = new DatabaseService("OpenBullet.db");
        configManager = new ConfigManager();
        runnerManager = new RunnerManager();
    }
}
```

**Configuration Manager**
```csharp
public class ConfigManager
{
    public Config LoadConfig(string path)
    {
        var content = File.ReadAllText(path);
        var config = new Config();
        
        // Parse [SETTINGS] section
        config.Settings = ParseSettingsSection(content);
        
        // Parse [SCRIPT] section
        config.Script = ParseScriptSection(content);
        
        return config;
    }
    
    private ConfigSettings ParseSettingsSection(string content)
    {
        var settingsMatch = Regex.Match(content, @"\[SETTINGS\]\s*({.*?})", 
            RegexOptions.Singleline);
        
        if (settingsMatch.Success)
        {
            return JsonConvert.DeserializeObject<ConfigSettings>(
                settingsMatch.Groups[1].Value);
        }
        
        return new ConfigSettings();
    }
}
```

**LoliScript Parser**
```csharp
public class LoliScriptParser
{
    public List<LoliScriptCommand> ParseScript(string script)
    {
        var commands = new List<LoliScriptCommand>();
        var lines = script.Split('\n');
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;
                
            var command = ParseCommand(line.Trim());
            if (command != null)
                commands.Add(command);
        }
        
        return commands;
    }
    
    private LoliScriptCommand ParseCommand(string line)
    {
        var parts = line.Split(' ', 2);
        var commandType = parts[0].ToUpper();
        
        switch (commandType)
        {
            case "REQUEST":
                return ParseRequestCommand(parts[1]);
            case "KEYCHECK":
                return ParseKeyCheckCommand(parts[1]);
            case "PARSE":
                return ParseParseCommand(parts[1]);
            // ... other commands
        }
        
        return null;
    }
}
```

**Execution Engine**
```csharp
public class ExecutionEngine
{
    public async Task<BotResult> ExecuteConfig(Config config, 
        DataLine data, ProxyInfo proxy)
    {
        var context = new ExecutionContext
        {
            Data = data,
            Proxy = proxy,
            Variables = new Dictionary<string, object>(),
            Captures = new Dictionary<string, string>()
        };
        
        foreach (var command in config.Script)
        {
            var result = await ExecuteCommand(command, context);
            
            if (result.ShouldStop)
                break;
        }
        
        return ClassifyResult(context);
    }
    
    private async Task<CommandResult> ExecuteCommand(
        LoliScriptCommand command, ExecutionContext context)
    {
        switch (command.Type)
        {
            case CommandType.Request:
                return await ExecuteHttpRequest(command, context);
            case CommandType.Parse:
                return ExecuteDataParsing(command, context);
            case CommandType.KeyCheck:
                return ExecuteKeyCheck(command, context);
            // ... other command types
        }
        
        return CommandResult.Continue();
    }
}
```

#### 4. Key Components Implementation

**HTTP Client (Custom Implementation)**
```csharp
public class CustomHttpClient
{
    private readonly HttpClient httpClient;
    private readonly ProxyInfo proxy;
    
    public async Task<HttpResponse> SendAsync(HttpRequest request)
    {
        var httpRequestMessage = BuildHttpRequestMessage(request);
        
        if (proxy != null)
            SetupProxy(httpRequestMessage);
            
        var response = await httpClient.SendAsync(httpRequestMessage);
        
        return new HttpResponse
        {
            StatusCode = (int)response.StatusCode,
            Content = await response.Content.ReadAsStringAsync(),
            Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.First())
        };
    }
}
```

**Data Parsing Engine**
```csharp
public class DataParser
{
    public string ParseLR(string input, string left, string right, bool recursive = false)
    {
        var startIndex = input.IndexOf(left);
        if (startIndex == -1) return string.Empty;
        
        startIndex += left.Length;
        var endIndex = input.IndexOf(right, startIndex);
        if (endIndex == -1) return string.Empty;
        
        return input.Substring(startIndex, endIndex - startIndex);
    }
    
    public string ParseRegex(string input, string pattern, string output)
    {
        var regex = new Regex(pattern);
        var match = regex.Match(input);
        
        if (!match.Success) return string.Empty;
        
        return regex.Replace(output, m => match.Groups[m.Groups[1].Value].Value);
    }
    
    public string ParseCSS(string html, string selector, string attribute)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = context.OpenAsync(req => req.Content(html)).Result;
        
        var element = document.QuerySelector(selector);
        
        return attribute == "text" ? element?.TextContent : element?.GetAttribute(attribute);
    }
}
```

#### 5. Database Layer
```csharp
public class DatabaseService
{
    private readonly LiteDatabase database;
    
    public DatabaseService(string connectionString)
    {
        database = new LiteDatabase(connectionString);
    }
    
    public void SaveConfig(Config config)
    {
        var collection = database.GetCollection<Config>("configs");
        collection.Upsert(config);
    }
    
    public void SaveResult(BotResult result)
    {
        var collection = database.GetCollection<BotResult>("results");
        collection.Insert(result);
    }
    
    public List<Config> GetConfigs()
    {
        var collection = database.GetCollection<Config>("configs");
        return collection.FindAll().ToList();
    }
}
```

#### 6. Proxy Management
```csharp
public class ProxyManager
{
    private readonly List<ProxyInfo> proxies = new List<ProxyInfo>();
    private readonly object lockObject = new object();
    private int currentIndex = 0;
    
    public ProxyInfo GetNext()
    {
        lock (lockObject)
        {
            if (proxies.Count == 0) return null;
            
            var proxy = proxies[currentIndex];
            currentIndex = (currentIndex + 1) % proxies.Count;
            
            return proxy;
        }
    }
    
    public void BanProxy(ProxyInfo proxy)
    {
        lock (lockObject)
        {
            proxies.Remove(proxy);
        }
    }
    
    public void LoadFromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            if (TryParseProxy(line, out var proxy))
                proxies.Add(proxy);
        }
    }
}
```

### Building the "Anomaly" Modifications

#### 1. Enhanced UI Features
```csharp
// Custom theme system
public class ThemeManager
{
    public void ApplyTheme(Theme theme)
    {
        Application.Current.Resources["BackgroundMain"] = theme.BackgroundMain;
        Application.Current.Resources["ForegroundGood"] = theme.ForegroundGood;
        // ... apply other theme properties
    }
}

// Enhanced statistics display
public class StatsManager
{
    public void UpdateStats(BotResult result)
    {
        switch (result.Status)
        {
            case ResultStatus.Success:
                Hits++;
                break;
            case ResultStatus.Failure:
                Fails++;
                break;
            // ... handle other statuses
        }
        
        UpdateUI();
    }
}
```

#### 2. Advanced Configuration Features
```csharp
// Live config updates
public class LiveConfigManager
{
    private FileSystemWatcher watcher;
    
    public void StartWatching(string configPath)
    {
        watcher = new FileSystemWatcher(Path.GetDirectoryName(configPath))
        {
            Filter = "*.anom",
            EnableRaisingEvents = true
        };
        
        watcher.Changed += OnConfigChanged;
    }
    
    private void OnConfigChanged(object sender, FileSystemEventArgs e)
    {
        // Reload config and update running instances
        ReloadConfig(e.FullPath);
    }
}
```

#### 3. Enhanced Captcha Integration
```csharp
public class CaptchaService
{
    public async Task<string> SolveCaptcha(CaptchaType type, string imageData)
    {
        switch (type)
        {
            case CaptchaType.ReCaptcha:
                return await SolveReCaptcha(imageData);
            case CaptchaType.Image:
                return await SolveImageCaptcha(imageData);
            // ... other captcha types
        }
        
        return string.Empty;
    }
}
```

### Deployment Structure
```
Release/
├── OpenBullet.exe
├── OpenBullet.exe.config
├── bin/
│   ├── [All DLL dependencies]
├── Configs/
├── Wordlists/
├── DB/
├── Settings/
│   ├── OBSettings.json
│   ├── RLSettings.json
│   └── Environment.ini
├── Sounds/
│   ├── rifle_hit.wav
│   └── rifle_reload.wav
└── [Platform-specific directories]
```

## Technical Implementation Details

### Multi-Threading Architecture
- **UI Thread**: Main application interface
- **Worker Threads**: Configuration execution (configurable count)
- **Background Threads**: Proxy management, statistics updates
- **Thread Pool**: HTTP requests and parsing operations

### Memory Management
- **Connection Pooling**: HTTP client reuse
- **Object Pooling**: Frequent object reuse
- **Garbage Collection**: Optimized for high-throughput scenarios

### Security Considerations
- **Input Validation**: All user inputs sanitized
- **Proxy Validation**: Proxy connectivity testing
- **Rate Limiting**: Built-in request throttling
- **Error Handling**: Comprehensive exception management

### Performance Optimizations
- **Async/Await**: Non-blocking operations
- **Connection Reuse**: HTTP keep-alive
- **Memory Streams**: Efficient data handling
- **Compiled Regexes**: Pre-compiled pattern matching

## Conclusion

This analysis provides a comprehensive understanding of OpenBullet 1.4.4 Anomaly's architecture and implementation. The modded version enhances the base OpenBullet with improved UI, advanced configuration management, and additional automation features.

**Key Takeaways for Reproduction:**
1. **Architecture**: Multi-layered .NET application with clear separation of concerns
2. **Language**: Custom LoliScript DSL for automation logic
3. **Database**: LiteDB for lightweight, embedded data storage
4. **Threading**: Sophisticated multi-threading for performance
5. **Extensibility**: Plugin architecture for custom functionality

The reproduction requires understanding of .NET development, HTTP protocols, web automation principles, and the specific domain of credential checking/web testing tools.

---

**Disclaimer**: This analysis is for educational and security research purposes only. Always ensure compliance with applicable laws and terms of service when developing or using web automation tools.
