using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.UI.Services;
using System.Collections.ObjectModel;
using System.IO;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Job list view model for managing automation jobs
/// </summary>
public partial class JobListViewModel : ListViewModelBase<JobEntity>, INavigationAware
{
    private readonly IJobStorage _jobStorage;
    private readonly IConfigurationStorage _configurationStorage;
    private readonly IResultStorage _resultStorage;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private JobStatus? _selectedStatus = null;

    [ObservableProperty]
    private ObservableCollection<JobStatusFilter> _statusFilters = new();

    [ObservableProperty]
    private string _selectedConfigurationId = "All";

    [ObservableProperty]
    private ObservableCollection<ConfigurationFilter> _configurationFilters = new();

    [ObservableProperty]
    private DateTime? _dateFrom;

    [ObservableProperty]
    private DateTime? _dateTo;

    [ObservableProperty]
    private string _sortColumn = "CreatedAt";

    [ObservableProperty]
    private bool _sortDescending = true;

    public JobListViewModel(
        ILogger<JobListViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        INavigationService navigationService,
        IJobStorage jobStorage,
        IConfigurationStorage configurationStorage,
        IResultStorage resultStorage)
        : base(logger, dialogService, notificationService)
    {
        _navigationService = navigationService;
        _jobStorage = jobStorage;
        _configurationStorage = configurationStorage;
        _resultStorage = resultStorage;
        
        Title = "Jobs";
        InitializeFilters();
    }

    public void OnNavigatedTo()
    {
        _ = Task.Run(RefreshAsync);
    }

    public void OnNavigatedFrom()
    {
        // Cleanup if needed
    }

    private void InitializeFilters()
    {
        StatusFilters = new ObservableCollection<JobStatusFilter>
        {
            new("All", null),
            new("Running", JobStatus.Running),
            new("Completed", JobStatus.Completed),
            new("Failed", JobStatus.Failed),
            new("Cancelled", JobStatus.Cancelled),
            new("Created", JobStatus.Created)
        };

        LoadConfigurationFiltersAsync();
    }

    private async Task LoadConfigurationFiltersAsync()
    {
        try
        {
            var configs = await _configurationStorage.GetAllAsync();
            var filters = new List<ConfigurationFilter> { new("All Configurations", "All") };
            
            filters.AddRange(configs.Select(c => new ConfigurationFilter(c.Name, c.Id)));
            
            ConfigurationFilters = new ObservableCollection<ConfigurationFilter>(filters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration filters");
        }
    }

    protected override async Task<(IList<JobEntity> items, int totalCount)> LoadItemsAsync(int page, int pageSize, string searchText)
    {
        try
        {
            var filter = new JobFilter
            {
                Status = SelectedStatus,
                ConfigurationId = SelectedConfigurationId == "All" ? null : SelectedConfigurationId,
                StartDateFrom = DateFrom,
                StartDateTo = DateTo,
                SearchTerm = string.IsNullOrWhiteSpace(searchText) ? null : searchText
            };

            var pagedResult = await _jobStorage.GetPagedAsync(page, pageSize, filter);
            var items = pagedResult.Items.ToList();
            
            // Apply sorting
            items = SortJobs(items);
            
            return (items, pagedResult.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load jobs");
            return (new List<JobEntity>(), 0);
        }
    }

    private List<JobEntity> SortJobs(List<JobEntity> items)
    {
        return SortColumn switch
        {
            "Name" => SortDescending 
                ? items.OrderByDescending(j => j.Name).ToList()
                : items.OrderBy(j => j.Name).ToList(),
            "Status" => SortDescending
                ? items.OrderByDescending(j => j.Status).ToList()
                : items.OrderBy(j => j.Status).ToList(),
            "CreatedAt" => SortDescending
                ? items.OrderByDescending(j => j.CreatedAt).ToList()
                : items.OrderBy(j => j.CreatedAt).ToList(),
            "StartedAt" => SortDescending
                ? items.OrderByDescending(j => j.StartedAt).ToList()
                : items.OrderBy(j => j.StartedAt).ToList(),
            "Progress" => SortDescending
                ? items.OrderByDescending(j => j.ProgressPercentage).ToList()
                : items.OrderBy(j => j.ProgressPercentage).ToList(),
            "SuccessRate" => SortDescending
                ? items.OrderByDescending(j => j.SuccessRate).ToList()
                : items.OrderBy(j => j.SuccessRate).ToList(),
            _ => items
        };
    }

    partial void OnSelectedStatusChanged(JobStatus? value)
    {
        _ = Task.Run(async () =>
        {
            CurrentPage = 1;
            await LoadItemsAsync();
        });
    }

    partial void OnSelectedConfigurationIdChanged(string value)
    {
        _ = Task.Run(async () =>
        {
            CurrentPage = 1;
            await LoadItemsAsync();
        });
    }

    partial void OnDateFromChanged(DateTime? value)
    {
        _ = Task.Run(async () =>
        {
            CurrentPage = 1;
            await LoadItemsAsync();
        });
    }

    partial void OnDateToChanged(DateTime? value)
    {
        _ = Task.Run(async () =>
        {
            CurrentPage = 1;
            await LoadItemsAsync();
        });
    }

    [RelayCommand]
    private async Task CreateNewJobAsync()
    {
        try
        {
            // Create a new job entity with default values
            var newJob = new JobEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"New Job {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                Description = "Job created from the Jobs page",
                Status = JobStatus.Created,
                CreatedAt = DateTime.Now,
                ConcurrentBots = 10,
                UseProxies = false,
                TotalItems = 0,
                ProcessedItems = 0,
                SuccessfulItems = 0,
                FailedItems = 0,
                ErrorItems = 0,
                ConfigurationId = "" // Will need to be set in detail view
            };

            // Save the new job
            await _jobStorage.SaveAsync(newJob);
            
            // Navigate to job detail view for editing
            _navigationService.NavigateTo<JobDetailViewModel>(newJob.Id);
            
            ShowSuccess($"Created new job: {newJob.Name}");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to create new job: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task ViewJobDetailsAsync()
    {
        if (SelectedItem == null) return;
        
        try
        {
            // Navigate to job detail view with the selected job ID
            _navigationService.NavigateTo<JobDetailViewModel>(SelectedItem.Id);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to open job details: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task ViewJobResultsAsync()
    {
        if (SelectedItem == null) return;

        await ExecuteAsync(async () =>
        {
            var results = await _resultStorage.GetByJobIdAsync(SelectedItem.Id);
            var stats = await _resultStorage.GetStatisticsAsync(SelectedItem.Id);
            
            var resultsMessage = $"""
                Job Results: {SelectedItem.Name}
                
                Total Results: {stats.TotalResults:N0}
                Successful: {stats.SuccessfulResults:N0}
                Failed: {stats.FailedResults:N0}
                Success Rate: {(stats.TotalResults > 0 ? (double)stats.SuccessfulResults / stats.TotalResults * 100 : 0):F1}%
                
                Average Execution Time: {stats.AverageExecutionTime.TotalSeconds:F1}s
                
                Status Distribution:
                """;

            foreach (var statusCount in stats.StatusCounts)
            {
                resultsMessage += $"• {statusCount.Key}: {statusCount.Value:N0}\n";
            }

            if (stats.CapturedDataSummary.Any())
            {
                resultsMessage += "\nCaptured Data Fields:\n";
                foreach (var field in stats.CapturedDataSummary.Take(10))
                {
                    resultsMessage += $"• {field.Key}: {field.Value:N0} records\n";
                }
            }
            
            _dialogService.ShowInformation(resultsMessage, "Job Results");
            
        }, "Loading job results...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DeleteJobAsync()
    {
        if (SelectedItem == null) return;

        var message = $"Are you sure you want to delete job '{SelectedItem.Name}'?\n\nThis will also delete all associated results and cannot be undone.";
        if (!ShowConfirmation(message, "Delete Job"))
            return;

        await ExecuteAsync(async () =>
        {
            var success = await _jobStorage.DeleteAsync(SelectedItem.Id);
            if (success)
            {
                ShowSuccess($"Job '{SelectedItem.Name}' deleted successfully");
                await LoadItemsAsync();
            }
            else
            {
                ShowWarning("Failed to delete job");
            }
        }, "Deleting job...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task ExportJobResultsAsync()
    {
        if (SelectedItem == null) return;

        var filePath = _dialogService.ShowSaveFileDialog(
            "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|All files (*.*)|*.*",
            "Export Job Results",
            $"{SelectedItem.Name}_results.csv");

        if (string.IsNullOrEmpty(filePath)) return;

        await ExecuteAsync(async () =>
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var format = extension switch
            {
                ".csv" => ExportFormat.CSV,
                ".json" => ExportFormat.JSON,
                ".xml" => ExportFormat.XML,
                _ => ExportFormat.CSV
            };

            var success = await _resultStorage.ExportAsync(SelectedItem.Id, filePath, format);
            if (success)
            {
                ShowSuccess($"Job results exported to '{filePath}'");
            }
            else
            {
                ShowWarning("Failed to export job results");
            }
            
        }, "Exporting job results...");
    }

    [RelayCommand]
    private async Task CleanupOldJobsAsync()
    {
        var input = _dialogService.ShowInput(
            "Enter the number of days to keep completed jobs (older jobs will be deleted):",
            "Cleanup Old Jobs",
            "30");

        if (string.IsNullOrEmpty(input) || !int.TryParse(input, out var days) || days < 1)
            return;

        var message = $"This will delete all completed jobs older than {days} days.\n\nContinue?";
        if (!ShowConfirmation(message, "Cleanup Confirmation"))
            return;

        await ExecuteAsync(async () =>
        {
            var retentionPeriod = TimeSpan.FromDays(days);
            var deletedCount = await _jobStorage.CleanupOldJobsAsync(retentionPeriod, false);
            
            ShowSuccess($"{deletedCount} old jobs cleaned up successfully");
            await LoadItemsAsync();
            
        }, "Cleaning up old jobs...");
    }

    [RelayCommand]
    private async Task SetSortAsync(string column)
    {
        if (SortColumn == column)
        {
            SortDescending = !SortDescending;
        }
        else
        {
            SortColumn = column;
            SortDescending = column == "CreatedAt" || column == "StartedAt"; // Default descending for dates
        }

        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        SelectedStatus = null;
        SelectedConfigurationId = "All";
        DateFrom = null;
        DateTo = null;
        SearchText = string.Empty;
        
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task BulkDeleteAsync()
    {
        if (!SelectedItems.Any())
        {
            ShowWarning("Please select jobs to delete");
            return;
        }

        var message = $"Are you sure you want to delete {SelectedItems.Count} selected jobs and all their results?\n\nThis action cannot be undone.";
        if (!ShowConfirmation(message, "Bulk Delete"))
            return;

        await ExecuteAsync(async () =>
        {
            var deletedCount = 0;
            var failedCount = 0;

            foreach (var job in SelectedItems.ToList())
            {
                try
                {
                    var success = await _jobStorage.DeleteAsync(job.Id);
                    if (success)
                        deletedCount++;
                    else
                        failedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete job {JobName}", job.Name);
                    failedCount++;
                }
            }

            if (deletedCount > 0)
            {
                ShowSuccess($"{deletedCount} jobs deleted successfully");
            }
            
            if (failedCount > 0)
            {
                ShowWarning($"{failedCount} jobs could not be deleted");
            }

            await LoadItemsAsync();
            ClearSelection();
            
        }, $"Deleting {SelectedItems.Count} jobs...");
    }

    public override async Task InitializeAsync()
    {
        await LoadConfigurationFiltersAsync();
        await base.InitializeAsync();
    }
}

/// <summary>
/// Job status filter item
/// </summary>
public class JobStatusFilter
{
    public string Name { get; }
    public JobStatus? Status { get; }

    public JobStatusFilter(string name, JobStatus? status)
    {
        Name = name;
        Status = status;
    }

    public override string ToString() => Name;
}

/// <summary>
/// Configuration filter item
/// </summary>
public class ConfigurationFilter
{
    public string Name { get; }
    public string Id { get; }

    public ConfigurationFilter(string name, string id)
    {
        Name = name;
        Id = id;
    }

    public override string ToString() => Name;
}
