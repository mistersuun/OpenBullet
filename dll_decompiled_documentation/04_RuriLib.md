# RuriLib.dll Documentation

## Overview
RuriLib is the core automation framework library of OpenBullet, providing the block-based scripting system, pattern matching engine, and bot orchestration capabilities. It's the heart of OpenBullet's automation functionality.

## Purpose in OpenBullet
- Provides the block-based automation system
- Implements LoliScript/LoliCode scripting languages
- Manages bot data and state
- Handles pattern matching and result analysis
- Integrates all other libraries into a cohesive framework

## Key Components

### Core Data Structure

#### `BotData`
- **Purpose**: Central data container for bot execution
- **Key Properties**:
  - `Variables` - Dictionary of runtime variables
  - `Proxy` - Current proxy configuration
  - `Cookies` - Cookie storage for session
  - `ResponseSource` - Last HTTP response content
  - `ResponseCode` - Last HTTP status code
  - `Status` - Current bot status (SUCCESS, FAIL, BAN, RETRY, ERROR)
  - `CaptchaService` - CAPTCHA solving service
  - `Log` - Execution log entries
  - `GlobalVariables` - Shared variables across bots
- **Methods**:
  - `Reset()` - Clear all data
  - `Clone()` - Create deep copy
  - `Export()` - Export to JSON

### Block System

#### `BlockBase` (Abstract)
- **Purpose**: Base class for all automation blocks
- **Derived Blocks**:
  - `BlockRequest` - HTTP requests
  - `BlockKeycheck` - Pattern matching
  - `BlockFunction` - Data manipulation
  - `BlockParse` - Data extraction
  - `BlockCaptcha` - CAPTCHA solving
  - `BlockBypassCF` - Cloudflare bypass
  - `BlockTCP` - TCP connections
  - `BlockWebSocket` - WebSocket operations
  - `BlockSelenium` - Browser automation

#### `BlockRequest`
- **Purpose**: Execute HTTP requests
- **Key Properties**:
  - `Url` - Target URL
  - `Method` - HTTP method (GET, POST, etc.)
  - `PostData` - Request body
  - `ContentType` - Content type header
  - `CustomHeaders` - Additional headers
  - `MultipartBoundary` - For multipart forms
  - `RequestTimeout` - Timeout settings
- **Features**:
  - Automatic cookie handling
  - Proxy support
  - Response parsing
  - Error handling

#### `BlockKeycheck`
- **Purpose**: Analyze responses and determine bot status
- **Key Components**:
  - `KeyChain` - List of key checking rules
  - `KeyType` - Check type (SUCCESS, FAILURE, BAN, RETRY, CUSTOM)
  - `Condition` - Matching condition (CONTAINS, REGEX, etc.)
  - `CustomType` - Custom status type
- **Pattern Matching**:
```csharp
// Example keycheck configuration
KEYCHECK BanOn "Access Denied"
KEYCHECK BanOn "403 Forbidden"
KEYCHECK SuccessOn "Welcome"
KEYCHECK SuccessOn "Dashboard"
KEYCHECK FailureOn "Invalid credentials"
```

### Scripting Languages

#### LoliScript
- **Purpose**: Simple scripting language for OpenBullet
- **Syntax Example**:
```loliscript
REQUEST POST "https://example.com/login" 
  CONTENT "username=<USER>&password=<PASS>"
  CONTENTTYPE "application/x-www-form-urlencoded"
  HEADER "Accept: */*"
  HEADER "Referer: https://example.com"

KEYCHECK 
  KEYCHAIN Success OR
    KEY "Welcome back"
    KEY "dashboard"
  KEYCHAIN Failure OR
    KEY "Invalid password"
    KEY "Account not found"
```

#### LoliCode (C#)
- **Purpose**: C# scripting for advanced automation
- **Example**:
```csharp
// LoliCode block
string token = data.Variables["TOKEN"];
var response = await HttpRequest.Post("https://api.example.com/verify", 
    new { token = token });

if (response.Contains("valid"))
{
    data.Status = BotStatus.Success;
    data.Variables["ACCOUNT_STATUS"] = "VALID";
}
```

### Configuration System

#### `Config`
- **Purpose**: Store and manage bot configurations
- **Properties**:
  - `Settings` - Configuration settings
  - `Blocks` - List of automation blocks
  - `Variables` - Predefined variables
  - `CustomInputs` - User-defined inputs
  - `Proxies` - Proxy configuration
- **Methods**:
  - `Save()` - Save to .anom file
  - `Load()` - Load from .anom file
  - `Verify()` - Validate configuration

### Runner System

#### `Runner`
- **Purpose**: Execute bot configurations
- **Types**:
  - `RunnerViewModel` - Single bot execution
  - `ProxyCheckRunner` - Proxy validation
  - `MultiRunner` - Parallel bot execution
- **Key Methods**:
  - `Start()` - Begin execution
  - `Stop()` - Stop execution
  - `Pause()` - Pause execution
  - `Resume()` - Resume execution

#### `MultiRunner`
- **Purpose**: Manage multiple bot instances
- **Properties**:
  - `Bots` - List of bot instances
  - `ThreadCount` - Number of concurrent bots
  - `ProxyMode` - Proxy rotation mode
  - `DataPool` - Input data management
- **Events**:
  - `OnBotCompleted` - Bot finished
  - `OnProgress` - Progress update
  - `OnResult` - Result available

## Implementation Examples

### Basic Bot Configuration
```csharp
var config = new Config();
config.Settings.Name = "Example Bot";

// Add request block
var requestBlock = new BlockRequest
{
    Url = "https://example.com/api",
    Method = HttpMethod.POST,
    PostData = "{\"user\":\"<USER>\",\"pass\":\"<PASS>\"}"
};
config.Blocks.Add(requestBlock);

// Add keycheck block
var keycheckBlock = new BlockKeycheck();
keycheckBlock.KeyChains.Add(new KeyChain
{
    Type = KeycheckType.Success,
    Keys = new List<Key> { new Key("success", Condition.Contains) }
});
config.Blocks.Add(keycheckBlock);
```

### Custom Block Implementation
```csharp
public class CustomBlock : BlockBase
{
    public override string Label => "CUSTOM";
    
    public override BlockResult Process(BotData data)
    {
        try
        {
            // Custom processing logic
            var input = data.Variables["INPUT"];
            var result = ProcessInput(input);
            
            data.Variables["OUTPUT"] = result;
            
            return BlockResult.Continue;
        }
        catch (Exception ex)
        {
            data.Log.Add($"Error: {ex.Message}");
            return BlockResult.Error;
        }
    }
}
```

### Variable Management
```csharp
// Setting variables
data.Variables["TOKEN"] = ExtractToken(response);
data.Variables["USER_ID"] = ParseUserId(response);

// Using variables in requests
var url = $"https://api.example.com/user/{data.Variables["USER_ID"]}";

// Global variables (shared across bots)
data.GlobalVariables["TOTAL_SUCCESS"] = successCount;
```

### CAPTCHA Integration
```csharp
var captchaBlock = new BlockCaptcha
{
    Type = CaptchaType.ReCaptchaV2,
    SiteKey = "6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-",
    SiteUrl = "https://example.com",
    Service = CaptchaService.TwoCaptcha,
    ApiKey = "YOUR_API_KEY"
};

// Automatic solving
var solution = captchaBlock.Process(data);
data.Variables["G_RECAPTCHA_RESPONSE"] = solution;
```

### Selenium Integration
```csharp
var seleniumBlock = new BlockSelenium
{
    Action = SeleniumAction.Navigate,
    Url = "https://example.com",
    Browser = SeleniumBrowser.Chrome,
    Headless = true
};

// Execute browser automation
seleniumBlock.Process(data);

// Extract data from page
var extractBlock = new BlockSelenium
{
    Action = SeleniumAction.GetText,
    Selector = "div.username",
    OutputVariable = "USERNAME"
};
```

## Advanced Features

### Debugger Support
```csharp
public class Debugger
{
    public BotData Data { get; set; }
    public List<BlockBase> Blocks { get; set; }
    public int CurrentBlock { get; set; }
    
    public void Step()
    {
        var block = Blocks[CurrentBlock];
        var result = block.Process(Data);
        
        LogBlockExecution(block, result);
        CurrentBlock++;
    }
    
    public void SetBreakpoint(int blockIndex)
    {
        // Breakpoint logic
    }
}
```

### Custom Functions
```csharp
// Register custom function
Functions.Register("CUSTOM_HASH", (input) => 
{
    return ComputeCustomHash(input);
});

// Use in LoliScript
// FUNCTION CustomHash "<INPUT>" -> VAR "HASH"
```

### Wordlist Management
```csharp
public class WordlistManager
{
    public List<string> LoadWordlist(string path)
    {
        return File.ReadAllLines(path).ToList();
    }
    
    public IEnumerable<DataPair> ParseCombolist(string path, string separator = ":")
    {
        foreach (var line in File.ReadLines(path))
        {
            var parts = line.Split(separator);
            if (parts.Length >= 2)
            {
                yield return new DataPair(parts[0], parts[1]);
            }
        }
    }
}
```

## Integration Points

### HTTP Libraries
- Uses Extreme.Net or Leaf.xNet for HTTP operations
- Configurable HTTP client selection

### Parsing Libraries
- Integrates AngleSharp for HTML parsing
- Supports JSON parsing with Newtonsoft.Json

### Browser Automation
- Selenium WebDriver integration
- Puppeteer support (optional)

### CAPTCHA Services
- 2Captcha
- Anti-Captcha
- CapMonster
- Custom solver integration

## Best Practices
1. Use appropriate block types for tasks
2. Implement proper error handling
3. Optimize keycheck patterns
4. Manage variables efficiently
5. Use debugging for development
6. Implement logging for production
7. Handle rate limiting properly
8. Clean up resources after execution

## Configuration File Format (.anom)
```
[SETTINGS]
Name=Example Config
Author=User
Version=1.0

[SCRIPT]
REQUEST POST "https://example.com/login"
  CONTENT "user=<USER>&pass=<PASS>"
  HEADER "User-Agent: Mozilla/5.0"

KEYCHECK
  KEYCHAIN Success OR
    KEY "success":true"
    KEY "authenticated"
  KEYCHAIN Failure OR
    KEY "error"
    KEY "invalid"
```

## Security Considerations
- Sanitize user inputs
- Protect API keys and credentials
- Implement rate limiting
- Log security events
- Validate configuration files
- Use secure proxy connections
- Handle sensitive data properly