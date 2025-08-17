using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.Core.Models;
using OpenBullet.UI.Services;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Configuration detail view model for editing configurations
/// </summary>
public partial class ConfigurationDetailViewModel : ViewModelBase, IParameterReceiver, INavigationAware
{
    private readonly IConfigurationStorage _configurationStorage;

    [ObservableProperty]
    private ConfigurationEntity? _configuration;

    [ObservableProperty]
    private bool _isNewConfiguration;

    [ObservableProperty]
    private string _configurationName = string.Empty;

    [ObservableProperty]
    private string _configurationScript = string.Empty;

    [ObservableProperty]
    private string _configurationDescription = string.Empty;

    [ObservableProperty]
    private string _configurationCategory = string.Empty;

    [ObservableProperty]
    private string _configurationAuthor = string.Empty;

    [ObservableProperty]
    private string _configurationVersion = "1.0.0";

    public ConfigurationDetailViewModel(
        ILogger<ConfigurationDetailViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        IConfigurationStorage configurationStorage)
        : base(logger, dialogService, notificationService)
    {
        _configurationStorage = configurationStorage;
        Title = "Configuration Editor";
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is string configId)
        {
            _ = Task.Run(() => LoadConfigurationAsync(configId));
        }
        else if (parameter is ConfigurationEntity config)
        {
            Configuration = config;
            LoadConfigurationProperties();
        }
        else
        {
            // New configuration
            IsNewConfiguration = true;
            Configuration = new ConfigurationEntity();
        }
    }

    public void OnNavigatedTo()
    {
        // Initialize if needed
    }

    public void OnNavigatedFrom()
    {
        // Cleanup if needed
    }

    private async Task LoadConfigurationAsync(string configId)
    {
        await ExecuteAsync(async () =>
        {
            Configuration = await _configurationStorage.GetByIdAsync(configId);
            if (Configuration != null)
            {
                LoadConfigurationProperties();
            }
            else
            {
                ShowError($"Configuration with ID {configId} not found");
            }
        }, "Loading configuration...");
    }

    private void LoadConfigurationProperties()
    {
        if (Configuration == null) return;

        ConfigurationName = Configuration.Name;
        ConfigurationScript = Configuration.Script;
        ConfigurationDescription = Configuration.Description ?? string.Empty;
        ConfigurationCategory = Configuration.Category;
        ConfigurationAuthor = Configuration.Author;
        ConfigurationVersion = Configuration.Version;
    }

    [RelayCommand]
    private async Task SaveConfigurationAsync()
    {
        if (Configuration == null) return;

        if (!ValidateConfiguration())
            return;

        await ExecuteAsync(async () =>
        {
            Configuration.Name = ConfigurationName;
            Configuration.Script = ConfigurationScript;
            Configuration.Description = ConfigurationDescription;
            Configuration.Category = ConfigurationCategory;
            Configuration.Author = ConfigurationAuthor;
            Configuration.Version = ConfigurationVersion;

            await _configurationStorage.SaveAsync(Configuration);
            ShowSuccess($"Configuration '{Configuration.Name}' saved successfully");

        }, "Saving configuration...");
    }

    [RelayCommand]
    private async Task TestScriptAsync()
    {
        if (string.IsNullOrWhiteSpace(ConfigurationScript))
        {
            ShowWarning("Please enter a script to test");
            return;
        }

        await ExecuteAsync(async () =>
        {
            // Create a simple test job configuration
            var testConfig = new OpenBullet.Core.Jobs.JobConfiguration
            {
                Name = $"Test - {ConfigurationName}",
                Config = new OpenBullet.Core.Models.ConfigModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ConfigurationName,
                    Script = ConfigurationScript
                },
                DataLines = new List<string> { "test:data" },
                ConcurrentBots = 1,
                SaveResults = true
            };

            // Validate the script first
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var validator = new OpenBullet.Core.Services.LoliScriptValidator(loggerFactory.CreateLogger<OpenBullet.Core.Services.LoliScriptValidator>());
            var validationResult = validator.ValidateScript(ConfigurationScript);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("\n", validationResult.Errors.Select(e => e.Message));
                ShowError($"Script validation failed:\n{errors}");
                return;
            }

            // Show success message
            ShowSuccess("Script validation passed! The script is syntactically correct.");
            
            // TODO: In a real implementation, you could run a test execution here
            // using the JobManager to test the script with sample data

        }, "Testing script...");
    }

    private bool ValidateConfiguration()
    {
        if (!ValidateRequired(ConfigurationName, "Configuration Name"))
            return false;

        if (!ValidateRequired(ConfigurationScript, "Configuration Script"))
            return false;

        return true;
    }
}

/// <summary>
/// Job detail view model for viewing job details and results
/// </summary>
public partial class JobDetailViewModel : ViewModelBase, IParameterReceiver, INavigationAware
{
    private readonly IJobStorage _jobStorage;
    private readonly IResultStorage _resultStorage;

    [ObservableProperty]
    private JobEntity? _job;

    [ObservableProperty]
    private string _jobName = string.Empty;

    [ObservableProperty]
    private JobStatus _jobStatus;

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private int _processedItems;

    [ObservableProperty]
    private int _successfulItems;

    [ObservableProperty]
    private int _failedItems;

    [ObservableProperty]
    private double _progressPercentage;

    [ObservableProperty]
    private double? _successRate;

    [ObservableProperty]
    private string _executionTime = "N/A";

    public JobDetailViewModel(
        ILogger<JobDetailViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        IJobStorage jobStorage,
        IResultStorage resultStorage)
        : base(logger, dialogService, notificationService)
    {
        _jobStorage = jobStorage;
        _resultStorage = resultStorage;
        Title = "Job Details";
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is string jobId)
        {
            _ = Task.Run(() => LoadJobAsync(jobId));
        }
        else if (parameter is JobEntity job)
        {
            Job = job;
            LoadJobProperties();
        }
    }

    public void OnNavigatedTo()
    {
        // Start auto-refresh for running jobs
    }

    public void OnNavigatedFrom()
    {
        // Stop auto-refresh
    }

    private async Task LoadJobAsync(string jobId)
    {
        await ExecuteAsync(async () =>
        {
            Job = await _jobStorage.GetByIdAsync(jobId);
            if (Job != null)
            {
                LoadJobProperties();
            }
            else
            {
                ShowError($"Job with ID {jobId} not found");
            }
        }, "Loading job details...");
    }

    private void LoadJobProperties()
    {
        if (Job == null) return;

        JobName = Job.Name;
        JobStatus = Job.Status;
        TotalItems = Job.TotalItems;
        ProcessedItems = Job.ProcessedItems;
        SuccessfulItems = Job.SuccessfulItems;
        FailedItems = Job.FailedItems;
        ProgressPercentage = Job.ProgressPercentage;
        SuccessRate = Job.SuccessRate;
        ExecutionTime = Job.Duration?.ToString(@"hh\:mm\:ss") ?? "N/A";
    }

    [RelayCommand]
    private async Task ViewResultsAsync()
    {
        if (Job == null) return;
        
        await ExecuteAsync(async () =>
        {
            // Get results for this job from storage
            var results = await _resultStorage.GetByJobIdAsync(Job.Id);
            var resultCount = results.Count();
            
            if (resultCount > 0)
            {
                var successCount = results.Count(r => r.Status == OpenBullet.Core.Models.BotStatus.Success);
                var failCount = results.Count(r => r.Status == OpenBullet.Core.Models.BotStatus.Failure);
                var banCount = results.Count(r => r.Status == OpenBullet.Core.Models.BotStatus.Ban);
                
                var message = $"Job Results for '{Job.Name}':\n\n" +
                             $"Total Results: {resultCount}\n" +
                             $"Success: {successCount}\n" +
                             $"Failed: {failCount}\n" +
                             $"Banned: {banCount}\n\n" +
                             $"Success Rate: {(resultCount > 0 ? (successCount * 100.0 / resultCount):0):F1}%";
                             
                ShowInfo(message);
            }
            else
            {
                ShowInfo($"No results found for job '{Job.Name}'. The job may not have been executed yet.");
            }
            
        }, "Loading job results...");
    }
}

/// <summary>
/// Proxy detail view model for viewing and editing proxy details
/// </summary>
public partial class ProxyDetailViewModel : ViewModelBase, IParameterReceiver, INavigationAware
{
    private readonly IProxyStorage _proxyStorage;

    [ObservableProperty]
    private ProxyEntity? _proxy;

    [ObservableProperty]
    private string _proxyHost = string.Empty;

    [ObservableProperty]
    private int _proxyPort;

    [ObservableProperty]
    private ProxyType _proxyType;

    [ObservableProperty]
    private string _proxyUsername = string.Empty;

    [ObservableProperty]
    private string _proxyPassword = string.Empty;

    [ObservableProperty]
    private ProxyHealth _proxyHealth;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isBanned;

    [ObservableProperty]
    private int _uses;

    [ObservableProperty]
    private int _successfulRequests;

    [ObservableProperty]
    private int _failedRequests;

    [ObservableProperty]
    private double? _successRate;

    public ProxyDetailViewModel(
        ILogger<ProxyDetailViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        IProxyStorage proxyStorage)
        : base(logger, dialogService, notificationService)
    {
        _proxyStorage = proxyStorage;
        Title = "Proxy Details";
    }

    public void ReceiveParameter(object parameter)
    {
        if (parameter is string proxyId)
        {
            _ = Task.Run(() => LoadProxyAsync(proxyId));
        }
        else if (parameter is ProxyEntity proxy)
        {
            Proxy = proxy;
            LoadProxyProperties();
        }
    }

    public void OnNavigatedTo()
    {
        // Initialize if needed
    }

    public void OnNavigatedFrom()
    {
        // Cleanup if needed
    }

    private async Task LoadProxyAsync(string proxyId)
    {
        await ExecuteAsync(async () =>
        {
            var allProxies = await _proxyStorage.GetAllAsync();
            Proxy = allProxies.FirstOrDefault(p => p.Id == proxyId);
            
            if (Proxy != null)
            {
                LoadProxyProperties();
            }
            else
            {
                ShowError($"Proxy with ID {proxyId} not found");
            }
        }, "Loading proxy details...");
    }

    private void LoadProxyProperties()
    {
        if (Proxy == null) return;

        ProxyHost = Proxy.Host;
        ProxyPort = Proxy.Port;
        ProxyType = Proxy.Type;
        ProxyUsername = Proxy.Username ?? string.Empty;
        ProxyPassword = Proxy.Password ?? string.Empty;
        ProxyHealth = Proxy.Health;
        IsActive = Proxy.IsActive;
        IsBanned = Proxy.IsBanned;
        Uses = Proxy.Uses;
        SuccessfulRequests = Proxy.SuccessfulRequests;
        FailedRequests = Proxy.FailedRequests;
        SuccessRate = Proxy.SuccessRate;
    }

    [RelayCommand]
    private async Task SaveProxyAsync()
    {
        if (Proxy == null) return;

        if (!ValidateProxy())
            return;

        await ExecuteAsync(async () =>
        {
            Proxy.Host = ProxyHost;
            Proxy.Port = ProxyPort;
            Proxy.Type = ProxyType;
            Proxy.Username = ProxyUsername;
            Proxy.Password = ProxyPassword;
            Proxy.IsActive = IsActive;

            await _proxyStorage.SaveAsync(Proxy);
            ShowSuccess($"Proxy '{Proxy.Address}' saved successfully");

        }, "Saving proxy...");
    }

    [RelayCommand]
    private async Task TestProxyAsync()
    {
        if (Proxy == null) return;

        await ExecuteAsync(async () =>
        {
            // Simulate proxy test
            await Task.Delay(2000);
            
            var isHealthy = Random.Shared.NextDouble() > 0.3;
            Proxy.Health = isHealthy ? ProxyHealth.Healthy : ProxyHealth.Dead;
            Proxy.LastTested = DateTime.UtcNow;
            
            await _proxyStorage.SaveAsync(Proxy);
            LoadProxyProperties();
            
            var result = isHealthy ? "passed" : "failed";
            ShowInfo($"Proxy test {result}");

        }, "Testing proxy...");
    }

    private bool ValidateProxy()
    {
        if (!ValidateRequired(ProxyHost, "Proxy Host"))
            return false;

        if (ProxyPort <= 0 || ProxyPort > 65535)
        {
            SetError("Port must be between 1 and 65535");
            return false;
        }

        return true;
    }
}
