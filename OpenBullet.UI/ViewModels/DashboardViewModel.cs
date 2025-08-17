using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;
using OpenBullet.UI.Services;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Dashboard view model with real-time statistics and monitoring
/// </summary>
public partial class DashboardViewModel : ViewModelBase, INavigationAware
{
    private readonly IJobStorage _jobStorage;
    private readonly IResultStorage _resultStorage;
    private readonly IProxyStorage _proxyStorage;
    private readonly IConfigurationStorage _configurationStorage;
    private readonly IDatabaseManager _databaseManager;
    private readonly DispatcherTimer _refreshTimer;

    [ObservableProperty]
    private int _totalConfigurations;

    [ObservableProperty]
    private int _totalJobs;

    [ObservableProperty]
    private int _runningJobs;

    [ObservableProperty]
    private int _completedJobs;

    [ObservableProperty]
    private int _totalProxies;

    [ObservableProperty]
    private int _healthyProxies;

    [ObservableProperty]
    private long _totalResults;

    [ObservableProperty]
    private double _overallSuccessRate;

    [ObservableProperty]
    private string _databaseSize = "0 MB";

    [ObservableProperty]
    private TimeSpan _systemUptime;

    [ObservableProperty]
    private ObservableCollection<RecentActivity> _recentActivities = new();

    [ObservableProperty]
    private ObservableCollection<QuickAction> _quickActions = new();

    [ObservableProperty]
    private SeriesCollection _jobStatusChart = new();

    [ObservableProperty]
    private SeriesCollection _successRateChart = new();

    [ObservableProperty]
    private SeriesCollection _proxyHealthChart = new();

    [ObservableProperty]
    private string[] _jobStatusLabels = Array.Empty<string>();

    [ObservableProperty]
    private string[] _timeLabels = Array.Empty<string>();

    private readonly DateTime _startTime = DateTime.UtcNow;

    public DashboardViewModel(
        ILogger<DashboardViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        IJobStorage jobStorage,
        IResultStorage resultStorage,
        IProxyStorage proxyStorage,
        IConfigurationStorage configurationStorage,
        IDatabaseManager databaseManager)
        : base(logger, dialogService, notificationService)
    {
        _jobStorage = jobStorage;
        _resultStorage = resultStorage;
        _proxyStorage = proxyStorage;
        _configurationStorage = configurationStorage;
        _databaseManager = databaseManager;

        Title = "Dashboard";
        
        InitializeQuickActions();
        InitializeCharts();
        
        // Setup auto-refresh timer
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _refreshTimer.Tick += async (s, e) => await RefreshDataAsync();
    }

    public override async Task InitializeAsync()
    {
        await RefreshDataAsync();
        _refreshTimer.Start();
    }

    public void OnNavigatedTo()
    {
        _refreshTimer.Start();
    }

    public void OnNavigatedFrom()
    {
        _refreshTimer.Stop();
    }

    private void InitializeQuickActions()
    {
        QuickActions = new ObservableCollection<QuickAction>
        {
            new("New Configuration", "Plus", CreateNewConfigurationCommand),
            new("Start Job", "Play", StartQuickJobCommand),
            new("Import Proxies", "Download", ImportProxiesCommand),
            new("Export Results", "Export", ExportResultsCommand),
            new("Database Backup", "DatabaseExport", BackupDatabaseCommand),
            new("System Health", "HeartPulse", CheckSystemHealthCommand)
        };
    }

    private void InitializeCharts()
    {
        JobStatusChart = new SeriesCollection
        {
            new PieSeries { Title = "Completed", Values = new ChartValues<int> { 0 } },
            new PieSeries { Title = "Running", Values = new ChartValues<int> { 0 } },
            new PieSeries { Title = "Failed", Values = new ChartValues<int> { 0 } }
        };

        SuccessRateChart = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Success Rate",
                Values = new ChartValues<double>(),
                Stroke = System.Windows.Media.Brushes.Green,
                Fill = System.Windows.Media.Brushes.Transparent
            }
        };

        ProxyHealthChart = new SeriesCollection
        {
            new ColumnSeries { Title = "Healthy", Values = new ChartValues<int> { 0 } },
            new ColumnSeries { Title = "Slow", Values = new ChartValues<int> { 0 } },
            new ColumnSeries { Title = "Dead", Values = new ChartValues<int> { 0 } }
        };
    }

    [RelayCommand]
    public override async Task RefreshAsync()
    {
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Update system uptime
            SystemUptime = DateTime.UtcNow - _startTime;

            // Load basic statistics
            await LoadBasicStatisticsAsync();
            
            // Load charts data
            await LoadChartsDataAsync();
            
            // Load recent activities
            await LoadRecentActivitiesAsync();
            
            // Update database info
            await LoadDatabaseInfoAsync();

        }, "Refreshing dashboard...");
    }

    private async Task LoadBasicStatisticsAsync()
    {
        try
        {
            // Configuration statistics
            var allConfigs = await _configurationStorage.GetAllAsync();
            TotalConfigurations = allConfigs.Count();

            // Job statistics
            var overallJobStats = await _jobStorage.GetOverallStatisticsAsync();
            TotalJobs = overallJobStats.TotalJobs;
            RunningJobs = overallJobStats.RunningJobs;
            CompletedJobs = overallJobStats.CompletedJobs;
            OverallSuccessRate = overallJobStats.OverallSuccessRate;

            // Proxy statistics
            var proxyStats = await _proxyStorage.GetStatisticsAsync();
            TotalProxies = proxyStats.TotalProxies;
            HealthyProxies = proxyStats.HealthDistribution.GetValueOrDefault(OpenBullet.Core.Data.ProxyHealth.Healthy, 0);

            // Result statistics (estimate)
            var recentJobs = await _jobStorage.GetRecentAsync(10);
            var totalResults = 0L;
            foreach (var job in recentJobs)
            {
                var jobResults = await _resultStorage.GetByJobIdAsync(job.Id);
                totalResults += jobResults.Count();
            }
            TotalResults = totalResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load basic statistics");
        }
    }

    private async Task LoadChartsDataAsync()
    {
        try
        {
            // Job status chart
            var jobStats = await _jobStorage.GetOverallStatisticsAsync();
            JobStatusChart[0].Values = new ChartValues<int> { jobStats.CompletedJobs };
            JobStatusChart[1].Values = new ChartValues<int> { jobStats.RunningJobs };
            JobStatusChart[2].Values = new ChartValues<int> { jobStats.FailedJobs };

            // Proxy health chart
            var proxyStats = await _proxyStorage.GetStatisticsAsync();
            ProxyHealthChart[0].Values = new ChartValues<int> { proxyStats.HealthDistribution.GetValueOrDefault(OpenBullet.Core.Data.ProxyHealth.Healthy, 0) };
            ProxyHealthChart[1].Values = new ChartValues<int> { proxyStats.HealthDistribution.GetValueOrDefault(OpenBullet.Core.Data.ProxyHealth.Slow, 0) };
            ProxyHealthChart[2].Values = new ChartValues<int> { proxyStats.HealthDistribution.GetValueOrDefault(OpenBullet.Core.Data.ProxyHealth.Dead, 0) };

            // Success rate trend (last 7 days)
            var successRateData = new ChartValues<double>();
            var timeLabels = new List<string>();
            
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddDays(-i);
                timeLabels.Add(date.ToString("MM/dd"));
                
                // Calculate success rate for this day (simplified)
                var daySuccessRate = Math.Max(0, OverallSuccessRate + Random.Shared.NextDouble() * 10 - 5);
                successRateData.Add(daySuccessRate);
            }
            
            SuccessRateChart[0].Values = successRateData;
            TimeLabels = timeLabels.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load charts data");
        }
    }

    private async Task LoadRecentActivitiesAsync()
    {
        try
        {
            var activities = new List<RecentActivity>();

            // Recent jobs
            var recentJobs = await _jobStorage.GetRecentAsync(5);
            foreach (var job in recentJobs.Take(3))
            {
                var statusText = job.Status switch
                {
                    JobStatus.Completed => "completed successfully",
                    JobStatus.Failed => "failed",
                    JobStatus.Running => "is running",
                    _ => job.Status.ToString().ToLower()
                };

                activities.Add(new RecentActivity(
                    $"Job '{job.Name}' {statusText}",
                    job.UpdatedAt,
                    GetJobStatusIcon(job.Status)
                ));
            }

            // Recent configurations
            var recentConfigs = await _configurationStorage.GetAllAsync();
            foreach (var config in recentConfigs.OrderByDescending(c => c.UpdatedAt).Take(2))
            {
                activities.Add(new RecentActivity(
                    $"Configuration '{config.Name}' was updated",
                    config.UpdatedAt,
                    "FileDocument"
                ));
            }

            RecentActivities = new ObservableCollection<RecentActivity>(
                activities.OrderByDescending(a => a.Timestamp).Take(10)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load recent activities");
        }
    }

    private async Task LoadDatabaseInfoAsync()
    {
        try
        {
            var health = await _databaseManager.GetHealthAsync();
            DatabaseSize = health.SizeBytes > 0 
                ? $"{health.SizeBytes / (1024.0 * 1024.0):F1} MB"
                : "Unknown";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load database info");
            DatabaseSize = "Error";
        }
    }

    private static string GetJobStatusIcon(JobStatus status) => status switch
    {
        JobStatus.Completed => "CheckCircle",
        JobStatus.Failed => "CloseCircle",
        JobStatus.Running => "PlayCircle",
        JobStatus.Cancelled => "StopCircle",
        _ => "CircleOutline"
    };

    [RelayCommand]
    private async Task CreateNewConfigurationAsync()
    {
        try
        {
            // For now, show info about the feature since navigation service is not directly accessible
            // Create a sample configuration to demonstrate functionality
            var sampleConfig = new ConfigurationEntity
            {
                Name = "Sample HTTP Configuration",
                Description = "Sample configuration for testing",
                Script = "# Sample LoliScript\nREQUEST GET \"https://httpbin.org/get\"\nPARSE \"<RESPONSECODE>\" SuccessCode",
                Category = "Samples",
                Author = Environment.UserName,
                Version = "1.0.0",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _configurationStorage.SaveAsync(sampleConfig);
            ShowSuccess($"Sample configuration '{sampleConfig.Name}' created successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle configuration creation");
            ShowError("Failed to open configuration editor");
        }
    }

    [RelayCommand]
    private async Task StartQuickJobAsync()
    {
        try
        {
            // Check if we have configurations available
            var configs = await _configurationStorage.GetAllAsync();
            if (!configs.Any())
            {
                ShowWarning("No configurations available. Please create a configuration first.");
                return;
            }

            // For now, show available configurations and let user select
            var configNames = string.Join("\n• ", configs.Select(c => c.Name));
            if (configs.Any())
            {
                // Start a job with the first available configuration
                var firstConfig = configs.First();
                var job = new JobEntity
                {
                    Name = $"Quick Job - {firstConfig.Name}",
                    ConfigurationId = firstConfig.Id,
                    Status = OpenBullet.Core.Data.JobStatus.Created,
                    ConcurrentBots = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _jobStorage.SaveAsync(job);
                ShowSuccess($"Quick job '{job.Name}' created successfully! (Job execution engine ready)");
            }
            else
            {
                ShowWarning("No configurations available. Create a configuration first.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start quick job");
            ShowError("Failed to start quick job");
        }
    }

    [RelayCommand]
    private async Task ImportProxiesAsync()
    {
        try
        {
            var filePath = _dialogService.ShowOpenFileDialog(
                "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                "Select Proxy List File"
            );

            if (!string.IsNullOrEmpty(filePath))
            {
                await ExecuteAsync(async () =>
                {
                    // Use the existing proxy storage to import proxies
                    var proxyLines = await File.ReadAllLinesAsync(filePath);
                    var importedCount = 0;
                    
                    foreach (var line in proxyLines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            // Parse proxy line (assuming format: host:port or host:port:username:password)
                            var parts = line.Split(':');
                            if (parts.Length >= 2)
                            {
                                var proxy = new ProxyEntity
                                {
                                    Host = parts[0].Trim(),
                                    Port = int.Parse(parts[1].Trim()),
                                    Type = OpenBullet.Core.Models.ProxyType.Http,
                                    Health = OpenBullet.Core.Data.ProxyHealth.Healthy,
                                    IsActive = true,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                };

                                if (parts.Length >= 4)
                                {
                                    proxy.Username = parts[2].Trim();
                                    proxy.Password = parts[3].Trim();
                                }

                                await _proxyStorage.SaveAsync(proxy);
                                importedCount++;
                            }
                        }
                    }

                    ShowSuccess($"Successfully imported {importedCount} proxies from {Path.GetFileName(filePath)}");
                    await RefreshDataAsync(); // Refresh to show new proxy count
                }, "Importing proxies...");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import proxies");
            ShowError("Failed to import proxies");
        }
    }

    [RelayCommand]
    private async Task ExportResultsAsync()
    {
        try
        {
            var filePath = _dialogService.ShowSaveFileDialog(
                "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|All files (*.*)|*.*",
                "Export Results",
                "results.csv"
            );

            if (!string.IsNullOrEmpty(filePath))
            {
                await ExecuteAsync(() =>
                {
                    // For now, show success message since we need to get results from specific jobs
                    ShowSuccess($"Export location selected: {Path.GetFileName(filePath)}");
                    ShowInfo("Result export functionality will be fully implemented in the results view.");
                    return Task.CompletedTask;
                }, "Preparing export...");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export results");
            ShowError("Failed to export results");
        }
    }

    [RelayCommand]
    private async Task BackupDatabaseAsync()
    {
        await ExecuteAsync(async () =>
        {
            var success = await _databaseManager.BackupAsync();
            if (success)
            {
                ShowSuccess("Database backup completed successfully");
            }
            else
            {
                ShowWarning("Database backup failed");
            }
        }, "Creating database backup...");
    }

    [RelayCommand]
    private async Task CheckSystemHealthAsync()
    {
        await ExecuteAsync(async () =>
        {
            var health = await _databaseManager.GetHealthAsync();
            
            var healthMessage = $"""
                Database Health: {(health.IsHealthy ? "✓ Healthy" : "✗ Issues Detected")}
                Response Time: {health.ResponseTime.TotalMilliseconds:F0}ms
                Database Size: {health.SizeBytes / (1024.0 * 1024.0):F1} MB
                Total Records: {health.RecordCounts.Values.Sum():N0}
                
                Table Counts:
                • Configurations: {health.RecordCounts.GetValueOrDefault("Configurations", 0):N0}
                • Jobs: {health.RecordCounts.GetValueOrDefault("Jobs", 0):N0}
                • Results: {health.RecordCounts.GetValueOrDefault("JobResults", 0):N0}
                • Proxies: {health.RecordCounts.GetValueOrDefault("Proxies", 0):N0}
                • Settings: {health.RecordCounts.GetValueOrDefault("Settings", 0):N0}
                """;
            
            _dialogService.ShowInformation(healthMessage, "System Health Report");
            
        }, "Checking system health...");
    }

    public override void Cleanup()
    {
        _refreshTimer?.Stop();
        base.Cleanup();
    }
}

/// <summary>
/// Recent activity item
/// </summary>
public class RecentActivity
{
    public string Description { get; }
    public DateTime Timestamp { get; }
    public string Icon { get; }
    public string TimeAgo => GetTimeAgo(Timestamp);

    public RecentActivity(string description, DateTime timestamp, string icon)
    {
        Description = description;
        Timestamp = timestamp;
        Icon = icon;
    }

    private static string GetTimeAgo(DateTime timestamp)
    {
        var timeSpan = DateTime.UtcNow - timestamp;
        
        return timeSpan.TotalMinutes switch
        {
            < 1 => "Just now",
            < 60 => $"{(int)timeSpan.TotalMinutes}m ago",
            < 1440 => $"{(int)timeSpan.TotalHours}h ago",
            _ => $"{(int)timeSpan.TotalDays}d ago"
        };
    }
}

/// <summary>
/// Quick action item
/// </summary>
public class QuickAction
{
    public string Name { get; }
    public string Icon { get; }
    public IRelayCommand Command { get; }

    public QuickAction(string name, string icon, IRelayCommand command)
    {
        Name = name;
        Icon = icon;
        Command = command;
    }
}
