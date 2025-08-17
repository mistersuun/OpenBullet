# Architecture Documentation - OpenBullet Recreation

## 🏗️ **System Architecture Overview**

### **High-Level Architecture**
```
┌─────────────────────────────────────────────────────────────┐
│                    OpenBullet.UI (WPF)                     │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────────────┐   │
│  │  Dashboard  │ │ Config Edit │ │    Job Manager      │   │
│  │   & Charts  │ │   & Syntax  │ │   & Monitoring      │   │
│  └─────────────┘ └─────────────┘ └─────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ MVVM + DI
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   OpenBullet.Core                          │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────────────┐   │
│  │  Scripting  │ │  HTTP/Proxy │ │   Job Execution     │   │
│  │   Engine    │ │   Manager   │ │      Engine         │   │
│  └─────────────┘ └─────────────┘ └─────────────────────┘   │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────────────┐   │
│  │   Commands  │ │  Variables  │ │    Data Storage     │   │
│  │ (REQ/PARSE) │ │   Manager   │ │   (EF Core/DB)      │   │
│  └─────────────┘ └─────────────┘ └─────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ Repository Pattern
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Database Layer                        │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────────────┐   │
│  │   SQLite    │ │   LiteDB    │ │    In-Memory DB     │   │
│  │  (Primary)  │ │ (Optional)  │ │     (Testing)       │   │
│  └─────────────┘ └─────────────┘ └─────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔧 **Core Components Architecture**

### **1. Scripting Engine**
```csharp
┌─────────────────────────────────────────────────────────────┐
│                    Scripting Engine                        │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    │
│  │ Script      │    │ Variable    │    │ Command     │    │
│  │ Parser      │ ── │ Manager     │ ── │ Executor    │    │
│  └─────────────┘    └─────────────┘    └─────────────┘    │
│         │                   │                   │         │
│         │                   │                   │         │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    │
│  │ Tokenizer   │    │ Data        │    │ Command     │    │
│  │ & Lexer     │    │ Context     │    │ Registry    │    │
│  └─────────────┘    └─────────────┘    └─────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

**Key Classes**:
- `ScriptParser`: Tokenizes and parses LoliScript
- `VariableManager`: Manages variable lifecycle and scoping
- `DataContext`: Execution context with variable storage
- `CommandRegistry`: Dynamic command discovery and execution

### **2. HTTP & Proxy Management**
```csharp
┌─────────────────────────────────────────────────────────────┐
│                  HTTP & Proxy System                       │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    │
│  │ HTTP Client │ ── │ Proxy Pool  │ ── │ Health      │    │
│  │ Service     │    │ Manager     │    │ Monitor     │    │
│  └─────────────┘    └─────────────┘    └─────────────┘    │
│         │                   │                   │         │
│         │                   │                   │         │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    │
│  │ Request     │    │ Rotation    │    │ Ban         │    │
│  │ Pipeline    │    │ Strategy    │    │ Manager     │    │
│  └─────────────┘    └─────────────┘    └─────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

**Key Features**:
- **Request Pipeline**: Interceptors for logging, retries, caching
- **Proxy Rotation**: Multiple strategies (Round-Robin, Health-based, etc.)
- **Health Monitoring**: Automatic proxy testing and recovery
- **Ban Management**: Intelligent proxy banning with recovery

### **3. Job Execution Engine**
```csharp
┌─────────────────────────────────────────────────────────────┐
│                   Job Execution Engine                     │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    │
│  │ Job         │ ── │ Bot Pool    │ ── │ Thread      │    │
│  │ Manager     │    │ Manager     │    │ Scheduler   │    │
│  └─────────────┘    └─────────────┘    └─────────────┘    │
│         │                   │                   │         │
│         │                   │                   │         │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    │
│  │ Progress    │    │ Bot Engine  │    │ Result      │    │
│  │ Tracker     │    │ (Individual)│    │ Collector   │    │
│  └─────────────┘    └─────────────┘    └─────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

**Execution Flow**:
1. **Job Creation**: Configure job parameters and target data
2. **Bot Pool**: Create and manage bot instances
3. **Thread Scheduling**: Distribute work across available threads
4. **Progress Tracking**: Real-time monitoring and statistics
5. **Result Collection**: Aggregate and store execution results

---

## 🏛️ **Design Patterns and Principles**

### **1. Repository Pattern**
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize);
    Task SaveAsync(T entity);
    Task<bool> DeleteAsync(string id);
}

// Implementation provides abstraction over data access
public class ConfigurationRepository : IRepository<ConfigurationEntity>
{
    private readonly SqliteContext _context;
    // Implementation details...
}
```

### **2. Factory Pattern**
```csharp
public interface ICommandFactory
{
    ICommand CreateCommand(string commandType);
    IEnumerable<string> GetAvailableCommands();
}

public class CommandFactory : ICommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _commands;
    
    public ICommand CreateCommand(string commandType)
    {
        if (_commands.TryGetValue(commandType, out var type))
        {
            return (ICommand)_serviceProvider.GetService(type);
        }
        throw new NotSupportedException($"Command '{commandType}' not supported");
    }
}
```

### **3. Strategy Pattern**
```csharp
public interface IProxyRotationStrategy
{
    ProxyInfo? GetNextProxy(IEnumerable<ProxyInfo> proxies);
    void RecordUsage(ProxyInfo proxy, bool success);
}

public class HealthBasedRotationStrategy : IProxyRotationStrategy
{
    public ProxyInfo? GetNextProxy(IEnumerable<ProxyInfo> proxies)
    {
        return proxies
            .Where(p => p.Health == ProxyHealth.Healthy)
            .OrderBy(p => p.Uses)
            .FirstOrDefault();
    }
}
```

### **4. Observer Pattern**
```csharp
public interface IProgressObserver
{
    void OnProgressUpdated(ProgressInfo progress);
    void OnJobCompleted(JobResult result);
    void OnError(Exception error);
}

public class JobProgressPublisher
{
    private readonly List<IProgressObserver> _observers = new();
    
    public void Subscribe(IProgressObserver observer) => _observers.Add(observer);
    public void Unsubscribe(IProgressObserver observer) => _observers.Remove(observer);
    
    protected virtual void NotifyProgress(ProgressInfo progress)
    {
        foreach (var observer in _observers)
            observer.OnProgressUpdated(progress);
    }
}
```

---

## 🔌 **Dependency Injection Architecture**

### **Service Registration Pattern**
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenBulletCore(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Core Services
        services.AddSingleton<IHttpClientService, HttpClientService>();
        services.AddSingleton<IProxyManager, ProxyManager>();
        services.AddSingleton<IScriptParser, ScriptParser>();
        services.AddSingleton<IVariableManager, VariableManager>();
        
        // Command Registration
        services.AddTransient<RequestCommand>();
        services.AddTransient<ParseCommand>();
        services.AddTransient<KeycheckCommand>();
        
        // Factories
        services.AddSingleton<ICommandFactory, CommandFactory>();
        services.AddSingleton<IBotEngineFactory, BotEngineFactory>();
        
        // Database
        services.AddOpenBulletDatabase(configuration);
        
        return services;
    }
}
```

### **Lifetime Management**
- **Singleton**: Services that maintain state (HttpClientService, ProxyManager)
- **Transient**: Commands and lightweight objects
- **Scoped**: Per-job or per-request services (in future web API)

---

## 💾 **Data Architecture**

### **Entity Relationship Diagram**
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Configuration  │    │      Job        │    │   JobResult     │
│                 │    │                 │    │                 │
│ + Id            │    │ + Id            │    │ + Id            │
│ + Name          │ ┌──│ + ConfigId      │ ┌──│ + JobId         │
│ + Script        │ │  │ + Name          │ │  │ + Data          │
│ + Category      │ │  │ + Status        │ │  │ + Success       │
│ + Author        │ │  │ + Progress      │ │  │ + Timestamp     │
│ + Version       │ │  │ + CreatedAt     │ │  │ + Duration      │
└─────────────────┘ │  └─────────────────┘ │  └─────────────────┘
                    │                      │
                    └──────────────────────┘
                    
┌─────────────────┐    ┌─────────────────┐
│     Proxy       │    │    Setting      │
│                 │    │                 │
│ + Id            │    │ + Id            │
│ + Host          │    │ + Key           │
│ + Port          │    │ + Value         │
│ + Type          │    │ + Category      │
│ + Health        │    │ + Description   │
│ + Uses          │    │ + CreatedAt     │
│ + IsBanned      │    │ + UpdatedAt     │
└─────────────────┘    └─────────────────┘
```

### **Database Migrations**
```csharp
public class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Configurations",
            columns: table => new
            {
                Id = table.Column<string>(nullable: false),
                Name = table.Column<string>(maxLength: 200, nullable: false),
                Script = table.Column<string>(nullable: false),
                Category = table.Column<string>(maxLength: 100, nullable: false),
                // ... other columns
            });
            
        // Indexes for performance
        migrationBuilder.CreateIndex(
            name: "IX_Configurations_Category",
            table: "Configurations",
            column: "Category");
    }
}
```

---

## 🚦 **Performance Architecture**

### **Concurrency Management**
```csharp
public class ConcurrencyManager
{
    private readonly SemaphoreSlim _jobSemaphore;
    private readonly SemaphoreSlim _httpSemaphore;
    private readonly ConcurrentQueue<BotEngine> _botPool;
    
    public ConcurrencyManager(int maxConcurrentJobs, int maxHttpConnections)
    {
        _jobSemaphore = new SemaphoreSlim(maxConcurrentJobs);
        _httpSemaphore = new SemaphoreSlim(maxHttpConnections);
        _botPool = new ConcurrentQueue<BotEngine>();
    }
}
```

### **Memory Management**
- **Object Pooling**: Reuse BotEngine instances to reduce GC pressure
- **Lazy Loading**: Load configurations and data on-demand
- **Streaming**: Process large datasets without loading into memory
- **Cache Management**: LRU cache for frequently accessed data

### **Performance Monitoring**
```csharp
public class PerformanceMetrics
{
    public int ActiveJobs { get; set; }
    public int TotalRequests { get; set; }
    public double AverageResponseTime { get; set; }
    public long MemoryUsage { get; set; }
    public int ProxyHealthScore { get; set; }
}
```

---

## 🔐 **Security Architecture**

### **Data Protection**
- **Sensitive Data Encryption**: Proxy credentials and user data
- **Configuration Validation**: Prevent malicious script execution
- **Input Sanitization**: All user inputs are validated and sanitized
- **Secure Storage**: Use Windows Data Protection API for secrets

### **Network Security**
- **SSL/TLS Validation**: Configurable certificate validation
- **Proxy Authentication**: Secure proxy credential management
- **Rate Limiting**: Built-in request throttling and delays
- **IP Filtering**: Configurable IP whitelisting/blacklisting

### **Audit and Logging**
```csharp
public class AuditLogger
{
    public void LogJobStart(string jobId, string userId) { }
    public void LogConfigurationAccess(string configId, string action) { }
    public void LogProxyUsage(string proxyId, bool success) { }
    public void LogSecurityEvent(string eventType, string details) { }
}
```

---

## 🔄 **Extension Architecture**

### **Plugin System (Future)**
```csharp
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    IEnumerable<ICommand> GetCommands();
    void Initialize(IServiceProvider serviceProvider);
}

public class PluginManager
{
    public void LoadPlugin(string assemblyPath) { }
    public void UnloadPlugin(string pluginName) { }
    public IEnumerable<IPlugin> GetLoadedPlugins() { }
}
```

### **Command Extension**
```csharp
[Command("CUSTOM")]
public class CustomCommand : ICommand
{
    public async Task<CommandResult> ExecuteAsync(
        CommandContext context, 
        CancellationToken cancellationToken)
    {
        // Custom command implementation
        return CommandResult.Success();
    }
}
```

---

## 📊 **Monitoring and Observability**

### **Logging Architecture**
```csharp
public static class LoggingSetup
{
    public static IServiceCollection AddOpenBulletLogging(
        this IServiceCollection services)
    {
        return services.AddLogging(builder =>
        {
            builder.AddSerilog(new LoggerConfiguration()
                .WriteTo.File("logs/openbullet-.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger());
        });
    }
}
```

### **Metrics Collection**
- **Job Metrics**: Success rates, execution times, error counts
- **System Metrics**: Memory usage, CPU utilization, thread counts
- **Network Metrics**: Request rates, response times, proxy performance
- **User Metrics**: Feature usage, configuration complexity, error patterns

**This architecture provides a solid foundation for building a scalable, maintainable, and extensible automation platform that can grow with user needs and technological advances.**
