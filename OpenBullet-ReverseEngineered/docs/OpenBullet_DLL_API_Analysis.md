# OpenBullet DLL Internal API Analysis

## Reverse Engineering Target: API Signatures and Integration Patterns

Based on analysis of the OpenBullet 1.4.4 Anomaly codebase structure, configuration files, and typical .NET automation framework patterns, this document outlines the expected internal API structures we need to discover and integrate.

## 🎯 RuriLib.dll - Core Engine API Analysis

### Expected Namespace Structure:
```csharp
RuriLib/
├── Config/          # Configuration management
├── BotData/         # Runtime data container  
├── Runner/          # Execution engine
├── LoliScript/      # Script parsing and execution
├── Functions/       # Built-in functions library
└── ViewModels/      # UI binding models
```

### Critical API Discovery Targets:

#### 1. Configuration Loading System
**Expected Pattern Based on `.anom` File Structure**:
```csharp
// Target API we need to reverse engineer:
namespace RuriLib
{
    public class Config
    {
        // Primary constructor - CRITICAL TO DISCOVER
        public Config(string name, string author, string version);
        
        // Configuration loading - HIGHEST PRIORITY
        public static Config LoadFromFile(string filePath);
        public void SaveToFile(string filePath);
        
        // Properties we know exist from .anom structure:
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Script { get; set; }           // LoliScript content
        public ConfigSettings Settings { get; set; } // Execution settings
        
        // Expected execution configuration:
        public bool NeedsProxies { get; set; }
        public int SuggestedBots { get; set; }
        public int MaxCPM { get; set; }
        public double LastModified { get; set; }
        public string RequiredPlugins { get; set; }
        public string Category { get; set; }
    }
    
    public class ConfigSettings
    {
        // Threading and execution settings
        public int MaxBots { get; set; }
        public int StartingPoint { get; set; }
        public bool IgnoreResponseErrors { get; set; }
        public int MaxRedirects { get; set; }
        public bool NeedsProxies { get; set; }
        public bool OnlySocks { get; set; }
        public bool OnlySsl { get; set; }
    }
}
```

#### 2. BotData Runtime Container
**Expected Pattern for Data Management**:
```csharp
// Critical runtime data container - MUST REVERSE ENGINEER
namespace RuriLib
{
    public class BotData
    {
        // Constructor signatures we need to discover:
        public BotData(ConfigSettings settings);
        public BotData(ConfigSettings settings, string data);
        public BotData(ConfigSettings settings, string data, IProxy proxy);
        
        // Data management - CRITICAL FOR WORDLIST INTEGRATION
        public string Data { get; set; }           // Current wordlist entry  
        public Dictionary<string, object> Variables { get; }  // Script variables
        public BotStatus Status { get; set; }      // Current execution status
        
        // HTTP client integration - HIGHEST PRIORITY
        public IWebClient WebClient { get; set; }  // HTTP client instance
        public ResponseData ResponseData { get; set; } // Last HTTP response
        
        // Proxy management
        public IProxy Proxy { get; set; }
        public bool UseProxy { get; set; }
        
        // Logging and debugging
        public List<string> Log { get; }
        public void LogBuffer(string message, LogLevel level);
        
        // Variable manipulation - CRITICAL FOR LOLISCRIPT
        public void SetVariable(string name, object value);
        public T GetVariable<T>(string name);
        public bool HasVariable(string name);
        
        // Status management
        public enum BotStatus { NONE, SUCCESS, FAIL, RETRY, ERROR, BAN, CUSTOM }
    }
    
    public class ResponseData
    {
        public string Source { get; set; }         // HTML content
        public string Address { get; set; }       // Final URL after redirects
        public Dictionary<string, string> Headers { get; set; }
        public List<Cookie> Cookies { get; set; }
        public int StatusCode { get; set; }
        public TimeSpan ResponseTime { get; set; }
    }
}
```

#### 3. LoliScript Execution Engine
**Expected Command Processing System**:
```csharp
// LoliScript parsing and execution - CORE FUNCTIONALITY
namespace RuriLib.LoliScript
{
    public class LoliScriptParser
    {
        // Main parsing method we need to discover:
        public static List<ILoliScriptCommand> ParseScript(string script, ConfigSettings settings);
        
        // Individual command parsers:
        public static RequestCommand ParseRequest(string line);
        public static KeyCheckCommand ParseKeyCheck(string line);
        public static ParseCommand ParseParse(string line);
        public static SetVarCommand ParseSetVar(string line);
        // ... other command parsers
    }
    
    public interface ILoliScriptCommand
    {
        // Command execution interface
        Task<bool> Execute(BotData data);
        string CommandName { get; }
        Dictionary<string, object> Parameters { get; }
    }
    
    // Specific command implementations we need to reverse engineer:
    public class RequestCommand : ILoliScriptCommand
    {
        public string Method { get; set; }         // GET, POST, etc.
        public string Url { get; set; }
        public string Content { get; set; }       // POST data
        public string ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int Timeout { get; set; }
        
        public Task<bool> Execute(BotData data);
    }
    
    public class KeyCheckCommand : ILoliScriptCommand
    {
        public List<KeyChain> KeyChains { get; set; }
        
        public class KeyChain
        {
            public string Key { get; set; }
            public KeyCheckCondition Condition { get; set; }  // CONTAINS, EQUALS, etc.
            public BotStatus ResultStatus { get; set; }       // SUCCESS, FAIL, CUSTOM
        }
        
        public enum KeyCheckCondition { CONTAINS, EQUALS, DOESNOTCONTAIN, MATCHES }
        
        public Task<bool> Execute(BotData data);
    }
}
```

#### 4. Execution Runner System
**Expected Threading and Execution Control**:
```csharp
// Main execution controller - CRITICAL FOR THREADING
namespace RuriLib
{
    public class Runner
    {
        // Constructor patterns to discover:
        public Runner(Config config, ProxyManager proxyManager);
        
        // Execution control - HIGHEST PRIORITY
        public async Task Start();
        public void Stop();
        public void Pause();
        public void Resume();
        
        // Threading configuration
        public int Bots { get; set; }              // Concurrent thread count
        public Queue<string> WordlistQueue { get; set; }  // Data queue
        
        // Progress tracking - CRITICAL FOR UI UPDATES
        public event EventHandler<BotData> OnResult;      // Hit/Fail events
        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler<string> OnStatusChanged;
        
        // Statistics - MUST INTEGRATE FOR UI
        public RunnerStats Stats { get; }
        
        // Proxy integration
        public ProxyManager ProxyManager { get; set; }
    }
    
    public class RunnerStats
    {
        public int Tested { get; set; }
        public int Hits { get; set; }
        public int Fails { get; set; }
        public int Errors { get; set; }
        public int Retries { get; set; }
        public double CPM { get; set; }
        public TimeSpan Elapsed { get; set; }
    }
    
    public class ProgressEventArgs
    {
        public int Tested { get; set; }
        public int Remaining { get; set; }
        public double Progress { get; set; }       // 0.0 to 1.0
    }
}
```

## 🌐 Leaf.xNet.dll - HTTP Client API Analysis

### Expected Namespace Structure:
```csharp
Leaf.xNet/
├── HttpRequest      # Main HTTP client
├── HttpResponse     # Response handling
├── ProxyClient/     # Proxy integration
├── Cookies/         # Cookie management
└── Extensions/      # Utility extensions
```

### Critical HTTP Integration APIs:

#### 1. HttpRequest Client
**Expected Core HTTP Functionality**:
```csharp
// Main HTTP client - CRITICAL FOR WEB REQUESTS
namespace Leaf.xNet
{
    public class HttpRequest : IDisposable
    {
        // Constructor patterns to discover:
        public HttpRequest();
        public HttpRequest(IProxyClient proxy);
        
        // HTTP method implementations - HIGHEST PRIORITY
        public HttpResponse Get(string address);
        public HttpResponse Post(string address, string content, string contentType);
        public HttpResponse Post(string address, byte[] content, string contentType);
        
        // Async versions (if available):
        public Task<HttpResponse> GetAsync(string address);
        public Task<HttpResponse> PostAsync(string address, string content, string contentType);
        
        // Configuration properties - CRITICAL FOR ANTI-DETECTION
        public string UserAgent { get; set; }
        public bool KeepAlive { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public int MaximumAutomaticRedirections { get; set; }
        public int ConnectTimeout { get; set; }
        public int ReadWriteTimeout { get; set; }
        
        // Proxy integration - MUST DISCOVER
        public IProxyClient Proxy { get; set; }
        
        // Cookie management - CRITICAL FOR SESSION HANDLING  
        public CookieContainer Cookies { get; set; }
        
        // Headers management
        public Dictionary<string, string> Headers { get; }
        public void AddHeader(string name, string value);
        public void RemoveHeader(string name);
        
        // Session management methods to discover:
        public void ResetSession();
        public void SetCookies(IEnumerable<Cookie> cookies);
    }
    
    public class HttpResponse
    {
        // Response data access - CRITICAL
        public string Content { get; }              // Response body as string
        public byte[] ContentBytes { get; }         // Response body as bytes
        public Stream ContentStream { get; }        // Response body as stream
        
        // Response metadata
        public Uri Address { get; }                 // Final URL after redirects
        public int StatusCode { get; }
        public string StatusDescription { get; }
        public Dictionary<string, string> Headers { get; }
        public long ContentLength { get; }
        public string ContentType { get; }
        
        // Cookie access
        public CookieCollection Cookies { get; }
        
        // Conversion methods to discover:
        public override string ToString();          // Get content as string
    }
}
```

#### 2. Proxy Integration System
**Expected Proxy Management APIs**:
```csharp
// Proxy handling - CRITICAL FOR ANONYMITY
namespace Leaf.xNet
{
    public interface IProxyClient
    {
        ProxyType Type { get; }
        string Host { get; set; }
        int Port { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
    
    public class HttpProxyClient : IProxyClient
    {
        public HttpProxyClient(string host, int port);
        public HttpProxyClient(string host, int port, string username, string password);
    }
    
    public class Socks4ProxyClient : IProxyClient
    {
        public Socks4ProxyClient(string host, int port);
        public Socks4ProxyClient(string host, int port, string username);
    }
    
    public class Socks5ProxyClient : IProxyClient  
    {
        public Socks5ProxyClient(string host, int port);
        public Socks5ProxyClient(string host, int port, string username, string password);
    }
    
    public enum ProxyType { HTTP, Socks4, Socks5 }
}
```

## 🔍 ProxySocket.dll - Socket Level Proxy API

### Expected Proxy Socket Management:
```csharp
// Socket-level proxy handling - ADVANCED PROXY FEATURES
namespace ProxySocket
{
    public class ProxySocket
    {
        // Socket-level proxy connection methods to discover:
        public static Socket ConnectToProxy(ProxySettings settings);
        public static bool TestProxy(string proxyString, int timeout);
        public static ProxyType DetectProxyType(string host, int port);
    }
    
    public class ProxySettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public ProxyType Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
    }
}
```

## 🖼️ Specialized Feature DLLs API Analysis

### Tesseract.dll - OCR Integration
```csharp
// CAPTCHA solving capabilities
namespace Tesseract
{
    public class TesseractEngine
    {
        public TesseractEngine(string dataPath, string language);
        public string Process(Bitmap image);
        public string Process(byte[] imageData);
    }
}
```

### WebDriver.dll - Browser Automation
```csharp
// Selenium integration for browser automation
namespace OpenQA.Selenium
{
    public class ChromeDriver : IWebDriver
    {
        public ChromeDriver(ChromeOptions options);
        public void Navigate().GoToUrl(string url);
        public IWebElement FindElement(By by);
        public string PageSource { get; }
        public byte[] GetScreenshot();
    }
}
```

### AngleSharp.dll - HTML Parsing
```csharp
// HTML/CSS parsing and manipulation
namespace AngleSharp
{
    public class HtmlDocument
    {
        public static HtmlDocument Parse(string html);
        public IElement QuerySelector(string selector);
        public IHtmlCollection<IElement> QuerySelectorAll(string selector);
    }
}
```

## Integration Discovery Methodology

### Phase 1: Static Analysis (ILSpy/dnSpy)
1. **Export complete namespace hierarchies**
2. **Map all public classes, methods, and properties**
3. **Identify constructor overloads and parameter requirements**
4. **Document event handlers and callback patterns**
5. **Extract enum definitions and constants**

### Phase 2: Dynamic Analysis (Runtime Debugging)
1. **Set breakpoints on critical API calls in original OpenBullet.exe**
2. **Trace execution flow during .anom config processing**
3. **Monitor parameter values and return data**
4. **Capture exception handling patterns**
5. **Profile memory usage and object lifecycles**

### Phase 3: API Validation Testing
1. **Create minimal test harnesses for each discovered API**
2. **Validate constructor patterns and initialization sequences**
3. **Test HTTP client session management and cookie handling**
4. **Verify LoliScript parsing and execution behavior**
5. **Compare results with original OpenBullet execution**

## Critical Integration Points

### 1. Configuration Loading Chain
```
.anom file → Config.LoadFromFile() → ConfigSettings → BotData initialization
```

### 2. LoliScript Execution Flow  
```
Script text → LoliScriptParser → Command objects → BotData.Execute() → Results
```

### 3. HTTP Request Processing
```
REQUEST command → HttpRequest setup → Proxy assignment → Execute → ResponseData
```

### 4. Result Processing Chain
```
KEYCHECK execution → Status assignment → Event triggering → Database storage
```

## Success Criteria for API Discovery

- [ ] **100% Constructor Compatibility**: All critical classes can be instantiated with correct parameters
- [ ] **Event Handler Integration**: All progress and result events properly connected
- [ ] **LoliScript Execution Parity**: Identical behavior for all command types  
- [ ] **HTTP Client Session Management**: Proper cookie and session handling
- [ ] **Proxy Integration**: Full support for HTTP/SOCKS4/SOCKS5 proxies
- [ ] **Anti-Detection Features**: User-agent rotation, timing, redirect handling
- [ ] **Database Operations**: LiteDB integration for result storage
- [ ] **Error Handling**: Proper exception management and recovery

This analysis provides the roadmap for discovering and integrating the internal APIs of all critical OpenBullet DLLs, ensuring our implementation achieves complete functional compatibility with the original system.
