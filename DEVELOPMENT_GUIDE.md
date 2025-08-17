# Development Guide - OpenBullet Recreation

## 🚀 **Getting Started with Development**

### **Prerequisites**
- **Visual Studio 2022** (Community or higher) OR **VS Code** with C# extension
- **.NET 8.0 SDK** (latest version)
- **Git** for version control
- **Windows 10/11** (primary platform, future cross-platform support planned)

### **Quick Setup**
```bash
# Clone the repository
git clone [repository-url]
cd OpenBulletRecreation

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests to verify setup
dotnet test

# Start the UI application
cd OpenBullet.UI
dotnet run
```

---

## 🏗️ **Project Structure Understanding**

### **Solution Organization**
```
OpenBulletRecreation/
├── OpenBullet.Core/                 # ⚡ Core automation engine
│   ├── Commands/                    # LoliScript command implementations
│   │   ├── RequestCommand.cs        # HTTP request command
│   │   ├── ParseCommand.cs          # Data parsing command
│   │   └── KeycheckCommand.cs       # Condition evaluation
│   ├── Data/                        # 💾 Database and storage
│   │   ├── SqliteContext.cs         # EF Core database context
│   │   ├── Entities.cs              # Database entity models
│   │   ├── StorageServices.cs       # Repository implementations
│   │   └── DatabaseServiceProvider.cs
│   ├── Engines/                     # 🎮 Execution engines
│   │   ├── BotEngine.cs             # Individual bot execution
│   │   └── JobRunner.cs             # Job execution coordinator
│   ├── Jobs/                        # 📋 Job management
│   │   ├── JobManager.cs            # Job lifecycle management
│   │   └── ProxyJobManager.cs       # Proxy-job integration
│   ├── Models/                      # 📦 Core data models
│   │   ├── BotData.cs               # Bot execution context
│   │   ├── ProxyInfo.cs             # Proxy information model
│   │   └── ConfigInfo.cs            # Configuration model
│   ├── Proxies/                     # 🌐 Proxy management
│   │   ├── ProxyManager.cs          # Advanced proxy management
│   │   └── ProxyExtensions.cs       # Utility methods
│   ├── Scripting/                   # 📜 Script processing
│   │   ├── ScriptParser.cs          # LoliScript parser
│   │   └── VariableManager.cs       # Variable management
│   └── Services/                    # 🔧 Core services
│       ├── HttpClientService.cs     # HTTP client wrapper
│       └── ConfigurationLoader.cs   # Configuration loading
├── OpenBullet.Core.Tests/           # 🧪 Comprehensive test suite
│   ├── Step[X]_[Feature]Tests.cs    # Feature-specific tests
│   └── [Integration/Unit/Performance]Tests/
├── OpenBullet.UI/                   # 🖥️ WPF user interface
│   ├── ViewModels/                  # MVVM view models
│   │   ├── MainWindowViewModel.cs   # Main window logic
│   │   ├── DashboardViewModel.cs    # Dashboard statistics
│   │   └── [Feature]ViewModel.cs    # Feature-specific VMs
│   ├── Views/                       # WPF user controls
│   │   ├── MainWindow.xaml          # Main application window
│   │   ├── DashboardView.xaml       # Statistics dashboard
│   │   └── [Feature]View.xaml       # Feature-specific views
│   ├── Services/                    # UI-specific services
│   │   ├── NavigationService.cs     # View navigation
│   │   └── UIServices.cs            # Dialogs, themes, etc.
│   └── Styles/                      # UI styling
│       └── Colors.xaml              # Color definitions
└── Documentation/                   # 📚 Project documentation
    ├── PROJECT_OVERVIEW.md          # High-level project description
    ├── PROGRESS_SUMMARY.md          # Completed features summary
    ├── CURRENT_STATUS.md            # Current state and capabilities
    ├── NEXT_STEPS.md                # Future development roadmap
    ├── ARCHITECTURE.md              # Technical architecture
    └── DEVELOPMENT_GUIDE.md         # This file
```

---

## 🔧 **Development Workflow**

### **1. Understanding the Current State**
Before making changes, understand what's already implemented:

```bash
# Run all tests to see current functionality
dotnet test --logger:console

# Start the UI to see current features
cd OpenBullet.UI
dotnet run

# Review the dashboard and existing views
# Check database integration by creating test data
```

### **2. Feature Development Process**

#### **Step 1: Design and Planning**
1. **Read Documentation**: Review relevant `.md` files
2. **Understand Dependencies**: Check what interfaces and services are available
3. **Design Interfaces**: Define contracts before implementation
4. **Plan Tests**: Write test cases before implementation (TDD approach)

#### **Step 2: Implementation**
```csharp
// Example: Adding a new command
// 1. Create the command class
public class NewCommand : ICommand
{
    public async Task<CommandResult> ExecuteAsync(
        CommandContext context, 
        CancellationToken cancellationToken)
    {
        // Implementation
        return CommandResult.Success();
    }
}

// 2. Register in DI container
services.AddTransient<NewCommand>();

// 3. Add to command factory
_commands["NEW"] = typeof(NewCommand);

// 4. Write comprehensive tests
[Fact]
public async Task NewCommand_ShouldExecuteSuccessfully()
{
    // Arrange, Act, Assert
}
```

#### **Step 3: Testing and Validation**
```bash
# Run specific test category
dotnet test --filter Category=Commands

# Run with coverage (if tool installed)
dotnet test --collect:"XPlat Code Coverage"

# Performance testing for new features
dotnet test --filter Category=Performance
```

---

## 🧪 **Testing Strategy**

### **Test Categories**
1. **Unit Tests**: Individual component testing
2. **Integration Tests**: Service interaction testing
3. **Performance Tests**: Load and stress testing
4. **UI Tests**: User interface functionality
5. **Validation Tests**: End-to-end workflow testing

### **Test Structure Example**
```csharp
public class RequestCommandTests
{
    private readonly RequestCommand _command;
    private readonly Mock<IHttpClientService> _httpClientMock;
    private readonly TestContext _context;

    public RequestCommandTests()
    {
        _httpClientMock = new Mock<IHttpClientService>();
        _command = new RequestCommand(_httpClientMock.Object);
        _context = TestContext.Create();
    }

    [Theory]
    [InlineData("GET", "https://httpbin.org/get")]
    [InlineData("POST", "https://httpbin.org/post")]
    public async Task ExecuteAsync_ShouldPerformHttpRequest(string method, string url)
    {
        // Arrange
        _context.Variables["url"] = url;
        _context.Variables["method"] = method;

        // Act
        var result = await _command.ExecuteAsync(_context, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _httpClientMock.Verify(x => x.SendAsync(
            It.Is<HttpRequestMessage>(req => req.Method.Method == method)), 
            Times.Once);
    }
}
```

### **Performance Testing**
```csharp
[Fact]
[Trait("Category", "Performance")]
public async Task JobRunner_ShouldHandle1000ConcurrentBots()
{
    // Arrange
    var jobRunner = CreateJobRunner();
    var config = CreateTestConfiguration();
    var stopwatch = Stopwatch.StartNew();

    // Act
    var job = await jobRunner.StartJobAsync(config, botCount: 1000);
    await job.WaitForCompletionAsync();
    stopwatch.Stop();

    // Assert
    Assert.True(stopwatch.Elapsed < TimeSpan.FromMinutes(5));
    Assert.True(job.SuccessRate > 0.95);
}
```

---

## 🏗️ **Key Development Patterns**

### **1. Command Pattern Implementation**
```csharp
// All LoliScript commands follow this pattern
public interface ICommand
{
    Task<CommandResult> ExecuteAsync(
        CommandContext context, 
        CancellationToken cancellationToken);
}

// Command context provides execution environment
public class CommandContext
{
    public BotData BotData { get; set; }
    public IVariableManager Variables { get; set; }
    public IServiceProvider Services { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}
```

### **2. Repository Pattern for Data Access**
```csharp
// Generic repository interface
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize);
    Task SaveAsync(T entity);
    Task<bool> DeleteAsync(string id);
}

// Specific repository with additional methods
public interface IConfigurationStorage : IRepository<ConfigurationEntity>
{
    Task<IEnumerable<ConfigurationEntity>> GetByCategoryAsync(string category);
    Task<ConfigurationUsageStats> GetUsageStatsAsync(string configId);
}
```

### **3. MVVM Pattern in UI**
```csharp
// Base view model with common functionality
public abstract class ViewModelBase : ObservableObject
{
    protected async Task ExecuteAsync(Func<Task> operation, string busyMessage = "Please wait...")
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            BusyMessage = busyMessage;
            await operation();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

// Specific view model
public class DashboardViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _totalJobs;

    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            TotalJobs = await _jobStorage.GetCountAsync();
        }, "Refreshing dashboard...");
    }
}
```

---

## 📦 **Adding New Features**

### **Adding a New LoliScript Command**
1. **Create Command Class**:
```csharp
[Command("MYCOMMAND")]
public class MyCommand : ICommand
{
    private readonly ILogger<MyCommand> _logger;
    
    public MyCommand(ILogger<MyCommand> logger)
    {
        _logger = logger;
    }
    
    public async Task<CommandResult> ExecuteAsync(
        CommandContext context, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Extract parameters from context
            var param1 = context.Parameters.GetValueOrDefault("param1", "default");
            
            // Perform command logic
            var result = await ProcessAsync(param1);
            
            // Update variables
            context.Variables["result"] = result;
            
            return CommandResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing MYCOMMAND");
            return CommandResult.Failure(ex.Message);
        }
    }
}
```

2. **Register in DI Container**:
```csharp
// In ServiceCollectionExtensions
services.AddTransient<MyCommand>();
```

3. **Add to Command Factory**:
```csharp
// In CommandFactory constructor
_commands["MYCOMMAND"] = typeof(MyCommand);
```

4. **Write Tests**:
```csharp
public class MyCommandTests
{
    [Fact]
    public async Task MyCommand_ShouldExecuteSuccessfully()
    {
        // Test implementation
    }
}
```

### **Adding a New UI View**
1. **Create View Model**:
```csharp
public partial class MyFeatureViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _data = string.Empty;
    
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            Data = await _dataService.LoadAsync();
        });
    }
}
```

2. **Create XAML View**:
```xml
<UserControl x:Class="OpenBullet.UI.Views.MyFeatureView">
    <Grid>
        <TextBlock Text="{Binding Data}" />
        <Button Command="{Binding LoadDataCommand}" Content="Load Data" />
    </Grid>
</UserControl>
```

3. **Register in DI**:
```csharp
services.AddTransient<MyFeatureViewModel>();
services.AddTransient<MyFeatureView>();
```

4. **Add to Navigation**:
```csharp
// In ViewFactory
_viewModelToViewMap[typeof(MyFeatureViewModel)] = typeof(MyFeatureView);
```

---

## 🔍 **Debugging and Troubleshooting**

### **Common Development Issues**

#### **1. Dependency Injection Issues**
```csharp
// Problem: Service not registered
// Solution: Check DI registration in ConfigureServices

// Verify registration
services.AddTransient<IMyService, MyService>();

// Debug DI issues
public MyClass(IServiceProvider serviceProvider)
{
    var service = serviceProvider.GetService<IMyService>();
    if (service == null)
        throw new InvalidOperationException("IMyService not registered");
}
```

#### **2. Database Issues**
```bash
# Check database file
ls -la *.db

# Inspect database schema
sqlite3 openbullet.db ".schema"

# Reset database (development only)
rm openbullet.db
dotnet ef database update
```

#### **3. Test Failures**
```bash
# Run tests with detailed output
dotnet test --logger:console --verbosity detailed

# Run specific test
dotnet test --filter "FullyQualifiedName~MyTest"

# Debug test in VS
# Set breakpoint and use Test Explorer
```

### **Debugging Tools**
- **Visual Studio Debugger**: Full debugging support
- **Serilog Logs**: Check `Logs/` directory for application logs
- **Entity Framework Logging**: Enable to see SQL queries
- **Memory Profilers**: dotMemory, PerfView for memory issues
- **Performance Profilers**: dotTrace, Visual Studio Profiler

---

## 📋 **Code Quality Guidelines**

### **Coding Standards**
```csharp
// ✅ Good: Clear naming and single responsibility
public class ConfigurationValidator
{
    public ValidationResult ValidateConfiguration(ConfigurationEntity config)
    {
        if (string.IsNullOrEmpty(config.Name))
            return ValidationResult.Error("Name is required");
            
        if (string.IsNullOrEmpty(config.Script))
            return ValidationResult.Error("Script is required");
            
        return ValidationResult.Success();
    }
}

// ❌ Bad: Unclear naming and multiple responsibilities
public class Manager
{
    public void DoStuff(object thing)
    {
        // Unclear what this does
    }
}
```

### **Error Handling**
```csharp
// ✅ Good: Specific exception handling
public async Task<ConfigurationEntity?> LoadConfigurationAsync(string id)
{
    try
    {
        return await _repository.GetByIdAsync(id);
    }
    catch (FileNotFoundException ex)
    {
        _logger.LogWarning("Configuration file not found: {Id}", id);
        return null;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to load configuration: {Id}", id);
        throw new ConfigurationLoadException($"Failed to load configuration {id}", ex);
    }
}
```

### **Async/Await Best Practices**
```csharp
// ✅ Good: Proper async/await usage
public async Task<string> ProcessDataAsync(string input)
{
    var result = await _service.ProcessAsync(input);
    return result.ToUpperInvariant();
}

// ❌ Bad: Blocking async operations
public string ProcessData(string input)
{
    var result = _service.ProcessAsync(input).Result; // Deadlock risk
    return result.ToUpperInvariant();
}
```

---

## 🚀 **Performance Optimization**

### **Memory Management**
```csharp
// Use object pooling for frequently created objects
public class BotEnginePool
{
    private readonly ConcurrentQueue<BotEngine> _pool = new();
    
    public BotEngine Get()
    {
        if (_pool.TryDequeue(out var engine))
        {
            engine.Reset();
            return engine;
        }
        return new BotEngine();
    }
    
    public void Return(BotEngine engine)
    {
        _pool.Enqueue(engine);
    }
}
```

### **Database Optimization**
```csharp
// Use async methods and proper indexing
[Index(nameof(Category))]
[Index(nameof(CreatedAt))]
public class ConfigurationEntity
{
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Efficient querying
public async Task<IEnumerable<ConfigurationEntity>> GetRecentConfigurationsAsync(int count)
{
    return await _context.Configurations
        .OrderByDescending(c => c.CreatedAt)
        .Take(count)
        .AsNoTracking() // Read-only for better performance
        .ToListAsync();
}
```

---

## 🤝 **Contributing Guidelines**

### **Pull Request Process**
1. **Fork and Branch**: Create feature branch from main
2. **Implement Feature**: Follow coding standards and patterns
3. **Write Tests**: Ensure >80% code coverage for new code
4. **Update Documentation**: Update relevant `.md` files
5. **Submit PR**: Include detailed description and test results

### **Commit Message Format**
```
feat: add UTILITY command for string operations

- Implement REPLACE, SUBSTRING, SPLIT operations
- Add comprehensive test coverage
- Update command registry and documentation

Closes #123
```

### **Code Review Checklist**
- [ ] **Functionality**: Does the code work as intended?
- [ ] **Tests**: Are there comprehensive tests with good coverage?
- [ ] **Performance**: Any performance implications?
- [ ] **Security**: Any security considerations?
- [ ] **Documentation**: Is documentation updated?
- [ ] **Style**: Follows project coding standards?

**Happy coding! This platform has a solid foundation - let's build something amazing together! 🚀**
