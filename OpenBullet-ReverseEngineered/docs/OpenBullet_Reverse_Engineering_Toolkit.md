# OpenBullet Reverse Engineering Toolkit & Practical Guide

## Essential Reverse Engineering Tools for .NET Assemblies

### 🔧 Primary Decompilation Tools

#### 1. **ILSpy** (Free, Open Source) - RECOMMENDED STARTING POINT
**Download**: https://github.com/icsharpcode/ILSpy/releases
**Purpose**: General-purpose .NET decompiler with excellent export capabilities

**Key Features for Our Analysis**:
- Export entire assemblies to Visual Studio projects
- Cross-reference analysis between classes
- Search functionality across multiple assemblies
- Plugin architecture for custom analysis

**Specific Usage for OpenBullet DLLs**:
```powershell
# Command line usage for batch analysis:
ilspycmd.exe "RuriLib.dll" -o "RuriLib_Decompiled" -p -r
ilspycmd.exe "Leaf.xNet.dll" -o "LeafxNet_Decompiled" -p -r  
ilspycmd.exe "ProxySocket.dll" -o "ProxySocket_Decompiled" -p -r
```

**Analysis Workflow**:
1. Load all OpenBullet DLLs simultaneously for cross-reference analysis
2. Export each critical DLL to separate Visual Studio projects
3. Use "Find" feature to locate specific API patterns (constructors, events)
4. Generate namespace hierarchies and class diagrams

#### 2. **dnSpy** (Free, Advanced Debugger) - CRITICAL FOR RUNTIME ANALYSIS
**Download**: https://github.com/dnSpy/dnSpy/releases
**Purpose**: Advanced decompiler with debugging and runtime analysis capabilities

**Key Features for API Discovery**:
- Attach to running .NET processes
- Set breakpoints in decompiled code
- Modify IL code at runtime for testing
- Memory inspection and variable watching
- Assembly patching capabilities

**Specific Usage for OpenBullet Analysis**:
```powershell
# Attach to running OpenBullet.exe process:
# 1. Start original OpenBullet.exe
# 2. Launch dnSpy as Administrator  
# 3. Debug -> Attach to Process -> Select OpenBullet.exe
# 4. Load symbols for RuriLib.dll, Leaf.xNet.dll, etc.
```

**Runtime Analysis Strategy**:
1. Set breakpoints on key API calls (Config.LoadFromFile, Runner.Start)
2. Execute amazonChecker.anom to trace execution flow
3. Monitor parameter values and return types
4. Capture exception handling patterns
5. Profile memory allocations and object lifecycles

#### 3. **JetBrains dotPeek** (Free Professional Decompiler)
**Download**: https://www.jetbrains.com/decompiler/
**Purpose**: High-quality decompilation with Visual Studio integration

**Key Features**:
- Excellent code reconstruction quality
- Symbol server integration
- Export to Visual Studio projects with full project structure
- Built-in navigation and search capabilities

### 🔍 Specialized Analysis Tools

#### 4. **Reflexil** (IL Editor Plugin)
**Purpose**: Modify IL code directly within assemblies for testing and patching
**Integration**: Works as plugin for ILSpy or standalone

**Usage for API Testing**:
- Patch method calls to add logging
- Modify private members to public for testing
- Insert debugging code without recompilation
- Test API behavior modifications

#### 5. **Assembly Analyzer Tools**
```powershell
# .NET Framework SDK tools for metadata analysis:
ildasm.exe RuriLib.dll /out:RuriLib.il    # Disassemble to IL
peverify.exe RuriLib.dll                  # Verify assembly integrity  
sn.exe -v RuriLib.dll                     # Check strong name signing
```

## Practical Reverse Engineering Workflow

### 📋 Step-by-Step Analysis Process

#### Phase 1: Initial Discovery (Day 1-2)

**Step 1: Environment Setup**
```powershell
# Create analysis workspace:
mkdir "C:\OpenBullet_Analysis"
cd "C:\OpenBullet_Analysis"

# Create subdirectories for organized analysis:
mkdir "Decompiled_Source"
mkdir "API_Documentation" 
mkdir "Test_Harnesses"
mkdir "Runtime_Analysis"
mkdir "Integration_Tests"

# Copy all DLL files for analysis:
copy "..\OpenBullet\Openbullet 1.4.4 Anomaly Modded Version\bin\*.dll" "."
```

**Step 2: Mass Decompilation**
```powershell
# Batch decompile all critical DLLs using ILSpy:
ilspycmd.exe "RuriLib.dll" -o "Decompiled_Source\RuriLib" -p -r
ilspycmd.exe "Leaf.xNet.dll" -o "Decompiled_Source\LeafxNet" -p -r
ilspycmd.exe "ProxySocket.dll" -o "Decompiled_Source\ProxySocket" -p -r
ilspycmd.exe "AngleSharp.dll" -o "Decompiled_Source\AngleSharp" -p -r
ilspycmd.exe "LiteDB.dll" -o "Decompiled_Source\LiteDB" -p -r
```

**Step 3: Namespace and Class Mapping**
```powershell
# Generate namespace hierarchies:
# Use PowerShell to extract namespace structure from decompiled code
Get-ChildItem -Recurse "Decompiled_Source" -Filter "*.cs" | 
    ForEach-Object { 
        Select-String -Path $_.FullName -Pattern "^namespace " 
    } | 
    Sort-Object Line | 
    Out-File "API_Documentation\Namespace_Hierarchy.txt"

# Extract class definitions:
Get-ChildItem -Recurse "Decompiled_Source" -Filter "*.cs" |
    ForEach-Object {
        Select-String -Path $_.FullName -Pattern "^public class |^public interface |^public enum "
    } |
    Sort-Object Line |
    Out-File "API_Documentation\Public_Classes.txt"
```

#### Phase 2: Critical API Analysis (Day 3-5)

**Focus Areas for Deep Analysis**:

**RuriLib.dll Priority Analysis**:
1. **Config Management**:
   ```csharp
   // Search patterns in decompiled RuriLib code:
   // - "class Config"
   // - "LoadFromFile"
   // - "ConfigSettings"
   // - Constructor patterns with (string, string, string) parameters
   ```

2. **BotData Container**:
   ```csharp
   // Search patterns:
   // - "class BotData"
   // - "Variables" dictionary
   // - "Status" enumeration
   // - "SetVariable" and "GetVariable" methods
   ```

3. **LoliScript Parser**:
   ```csharp
   // Search patterns:
   // - "LoliScriptParser" or similar
   // - "ParseRequest", "ParseKeyCheck" methods
   // - "ILoliScriptCommand" interface
   // - Command execution methods
   ```

**Leaf.xNet.dll Priority Analysis**:
1. **HttpRequest Client**:
   ```csharp
   // Search patterns:
   // - "class HttpRequest"
   // - "Get(", "Post(" method signatures
   // - "Proxy" property setters
   // - "Cookies" management
   ```

2. **Session Management**:
   ```csharp
   // Search patterns:
   // - Cookie handling methods
   // - Session persistence
   // - "KeepAlive" mechanisms
   ```

#### Phase 3: Runtime Behavior Analysis (Day 6-8)

**dnSpy Runtime Analysis Strategy**:

1. **Prepare Original OpenBullet for Analysis**:
   ```powershell
   # Copy original OpenBullet to analysis directory:
   copy "..\OpenBullet\Openbullet 1.4.4 Anomaly Modded Version\*" "Runtime_Analysis\" -Recurse
   cd "Runtime_Analysis"
   
   # Launch OpenBullet in analysis mode:
   # 1. Start dnSpy as Administrator
   # 2. File -> Open -> Select all DLL files from bin\ folder
   # 3. Start OpenBullet.exe
   # 4. Debug -> Attach to Process -> OpenBullet.exe
   ```

2. **Critical Breakpoint Locations**:
   ```csharp
   // Set breakpoints at these locations in dnSpy:
   
   // Config loading:
   RuriLib.Config.LoadFromFile(string path)
   RuriLib.ConfigSettings..ctor()
   
   // Runner initialization:
   RuriLib.Runner..ctor(Config, ProxyManager)
   RuriLib.Runner.Start()
   
   // LoliScript execution:
   RuriLib.LoliScript.LoliScriptParser.ParseScript()
   RuriLib.RequestCommand.Execute()
   RuriLib.KeyCheckCommand.Execute()
   
   // HTTP operations:
   Leaf.xNet.HttpRequest.Post()
   Leaf.xNet.HttpRequest.Get()
   Leaf.xNet.HttpResponse..ctor()
   ```

3. **Execution Tracing Workflow**:
   ```powershell
   # In dnSpy, execute this test sequence:
   # 1. Load amazonChecker.anom config
   # 2. Load sample wordlist
   # 3. Start execution with 1 bot
   # 4. Trace through complete execution cycle
   # 5. Document all API calls and parameter values
   ```

#### Phase 4: API Integration Testing (Day 9-12)

**Create Minimal Test Harnesses**:

```csharp
// Example test harness structure for RuriLib integration:
// File: Test_Harnesses/RuriLib_Config_Test.cs
using System;
using RuriLib;

class ConfigTest
{
    static void Main()
    {
        try 
        {
            // Test config loading with discovered API:
            var config = Config.LoadFromFile("amazonChecker.anom");
            Console.WriteLine($"Loaded: {config.Name} by {config.Author}");
            
            // Test BotData creation with discovered constructor:
            var botData = new BotData(config.Settings, "test@example.com");
            Console.WriteLine($"BotData initialized with: {botData.Data}");
            
            // Test LoliScript parsing with discovered methods:
            var commands = LoliScriptParser.ParseScript(config.Script, config.Settings);
            Console.WriteLine($"Parsed {commands.Count} commands");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Integration test failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
```

```csharp
// Example test harness for Leaf.xNet integration:
// File: Test_Harnesses/LeafxNet_HTTP_Test.cs
using System;
using Leaf.xNet;

class HttpTest
{
    static void Main()
    {
        try
        {
            // Test HttpRequest with discovered API:
            using (var request = new HttpRequest())
            {
                // Configure with discovered properties:
                request.UserAgent = "Mozilla/5.0...";
                request.KeepAlive = true;
                request.AllowAutoRedirect = true;
                
                // Test GET request:
                var response = request.Get("https://www.amazon.ca/ap/signin");
                Console.WriteLine($"Response: {response.StatusCode} - {response.Content.Length} chars");
                
                // Test session management:
                Console.WriteLine($"Cookies received: {request.Cookies.Count}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HTTP test failed: {ex.Message}");
        }
    }
}
```

### 📊 Analysis Documentation Templates

#### API Discovery Document Template:
```markdown
# [DLL_NAME] API Analysis Results

## Namespace Structure
- Primary namespace: [namespace]
- Key classes discovered:
  - [Class1]: [Purpose]
  - [Class2]: [Purpose]

## Critical Constructors
### [ClassName]
- Constructor 1: `[ClassName]([parameters])`
  - Purpose: [description]
  - Parameters: [parameter details]
  - Usage example: [code]

## Key Methods
### [MethodName]
- Signature: `[return_type] [MethodName]([parameters])`
- Purpose: [description]
- Parameters: [parameter details]
- Return value: [description]
- Usage example: [code]

## Event Handlers
### [EventName]
- Signature: `event EventHandler<[EventArgs]> [EventName]`
- Trigger conditions: [when fired]
- Usage example: [code]

## Integration Notes
- Dependencies: [required assemblies/types]
- Initialization sequence: [step by step]
- Common usage patterns: [examples]
- Error handling: [exception types and handling]
```

### 🎯 Priority Analysis Checklist

#### RuriLib.dll - HIGHEST PRIORITY
- [ ] **Config.LoadFromFile()** - Configuration loading mechanism
- [ ] **BotData constructor patterns** - Runtime data container initialization  
- [ ] **Runner execution flow** - Threading and bot management
- [ ] **LoliScript parser methods** - Command parsing and execution
- [ ] **Event handling system** - Progress and result notifications
- [ ] **Variable management** - Data storage and retrieval
- [ ] **Status management** - SUCCESS/FAIL/ERROR handling

#### Leaf.xNet.dll - HIGH PRIORITY  
- [ ] **HttpRequest constructor and configuration** - HTTP client setup
- [ ] **GET/POST method signatures** - Request execution
- [ ] **Proxy integration** - Proxy assignment and configuration
- [ ] **Cookie management** - Session handling
- [ ] **Response processing** - Content and metadata access
- [ ] **Anti-detection features** - User-agent, timing, redirects

#### ProxySocket.dll - MEDIUM PRIORITY
- [ ] **Proxy validation methods** - Testing and verification
- [ ] **Socket-level proxy handling** - Low-level connection management
- [ ] **Timeout and error handling** - Connection reliability

## Success Metrics for Reverse Engineering

### Functional Compatibility Tests:
1. **Configuration Loading**: Load and parse any `.anom` file successfully
2. **LoliScript Execution**: Execute any LoliScript command with identical behavior
3. **HTTP Client Integration**: Maintain sessions, cookies, and anti-detection features
4. **Proxy Management**: Support HTTP/SOCKS4/SOCKS5 with validation and rotation
5. **Threading and Concurrency**: Match original performance characteristics
6. **Event System**: Receive real-time progress and result notifications
7. **Database Operations**: Store and retrieve results with full metadata

### Performance Benchmarks:
- **Memory Usage**: Within 10% of original OpenBullet
- **Execution Speed**: Match or exceed original CPM rates
- **Resource Utilization**: Efficient thread and connection management
- **Error Recovery**: Robust handling of network issues and exceptions

This toolkit provides the complete methodology and tools needed to reverse engineer all OpenBullet DLLs and create a functionally identical implementation using the original assemblies.
