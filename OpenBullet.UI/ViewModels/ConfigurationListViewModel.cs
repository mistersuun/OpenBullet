using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.UI.Services;
using System.Collections.ObjectModel;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Configuration list view model for managing automation configurations
/// </summary>
public partial class ConfigurationListViewModel : ListViewModelBase<ConfigurationEntity>, INavigationAware
{
    private readonly IConfigurationStorage _configurationStorage;
    private readonly IFileService _fileService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _selectedCategory = "All";

    [ObservableProperty]
    private ObservableCollection<string> _categories = new();

    [ObservableProperty]
    private string _sortColumn = "Name";

    [ObservableProperty]
    private bool _sortDescending = false;

    public ConfigurationListViewModel(
        ILogger<ConfigurationListViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        INavigationService navigationService,
        IConfigurationStorage configurationStorage,
        IFileService fileService)
        : base(logger, dialogService, notificationService)
    {
        _navigationService = navigationService;
        _configurationStorage = configurationStorage;
        _fileService = fileService;
        
        Title = "Configurations";
        LoadCategoriesAsync();
    }

    public void OnNavigatedTo()
    {
        _ = Task.Run(RefreshAsync);
    }

    public void OnNavigatedFrom()
    {
        // Cleanup if needed
    }

    protected override async Task<(IList<ConfigurationEntity> items, int totalCount)> LoadItemsAsync(int page, int pageSize, string searchText)
    {
        try
        {
            var category = SelectedCategory == "All" ? null : SelectedCategory;
            var pagedResult = await _configurationStorage.GetPagedAsync(page, pageSize, category, searchText);
            
            var items = pagedResult.Items.ToList();
            
            // Apply sorting
            items = SortConfigurations(items);
            
            return (items, pagedResult.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configurations");
            return (new List<ConfigurationEntity>(), 0);
        }
    }

    private List<ConfigurationEntity> SortConfigurations(List<ConfigurationEntity> items)
    {
        return SortColumn switch
        {
            "Name" => SortDescending 
                ? items.OrderByDescending(c => c.Name).ToList()
                : items.OrderBy(c => c.Name).ToList(),
            "Category" => SortDescending
                ? items.OrderByDescending(c => c.Category).ToList()
                : items.OrderBy(c => c.Category).ToList(),
            "Author" => SortDescending
                ? items.OrderByDescending(c => c.Author).ToList()
                : items.OrderBy(c => c.Author).ToList(),
            "CreatedAt" => SortDescending
                ? items.OrderByDescending(c => c.CreatedAt).ToList()
                : items.OrderBy(c => c.CreatedAt).ToList(),
            "UsageCount" => SortDescending
                ? items.OrderByDescending(c => c.UsageCount).ToList()
                : items.OrderBy(c => c.UsageCount).ToList(),
            _ => items
        };
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var allConfigs = await _configurationStorage.GetAllAsync();
            var categories = allConfigs.Select(c => c.Category).Distinct().OrderBy(c => c).ToList();
            categories.Insert(0, "All");
            
            Categories = new ObservableCollection<string>(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");
        }
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        _ = Task.Run(async () =>
        {
            CurrentPage = 1;
            await LoadItemsAsync();
        });
    }

    [RelayCommand]
    private async Task CreateNewConfigurationAsync()
    {
        await ExecuteAsync(async () =>
        {
            var newConfig = new ConfigurationEntity
            {
                Name = "New Configuration",
                Description = "Created from dashboard",
                Script = "# New LoliScript configuration\n# Add your commands here\n\nREQUEST GET \"https://httpbin.org/get\"",
                Category = "General",
                Author = Environment.UserName,
                Version = "1.0.0",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _configurationStorage.SaveAsync(newConfig);
            
            // Navigate to configuration detail view for editing
            _navigationService.NavigateTo<ConfigurationDetailViewModel>(newConfig.Id);
            
            ShowSuccess($"Configuration '{newConfig.Name}' created successfully");
            await LoadItemsAsync();
            
        }, "Creating new configuration...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task EditConfigurationAsync()
    {
        if (SelectedItem == null) return;
        
        // For now, show basic configuration details and allow simple script editing
        var message = $"Configuration: {SelectedItem.Name}\n" +
                     $"Category: {SelectedItem.Category}\n" +
                     $"Author: {SelectedItem.Author}\n" +
                     $"Version: {SelectedItem.Version}\n\n" +
                     $"Script Preview:\n{SelectedItem.Script.Split('\n').Take(5).Aggregate((a, b) => a + "\n" + b)}...";
        
        ShowInfo(message);
        
        // TODO: Full configuration editor will be implemented in Step 13 with syntax highlighting
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DuplicateConfigurationAsync()
    {
        if (SelectedItem == null) return;

        await ExecuteAsync(async () =>
        {
            var duplicate = new ConfigurationEntity
            {
                Name = $"{SelectedItem.Name} (Copy)",
                Description = SelectedItem.Description,
                Script = SelectedItem.Script,
                Category = SelectedItem.Category,
                Author = SelectedItem.Author,
                Version = "1.0.0",
                SettingsJson = SelectedItem.SettingsJson,
                RequiredPluginsJson = SelectedItem.RequiredPluginsJson,
                TagsJson = SelectedItem.TagsJson,
                IsActive = true
            };

            await _configurationStorage.SaveAsync(duplicate);
            ShowSuccess($"Configuration '{duplicate.Name}' created successfully");
            await LoadItemsAsync();
            
        }, "Duplicating configuration...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task DeleteConfigurationAsync()
    {
        if (SelectedItem == null) return;

        var message = $"Are you sure you want to delete configuration '{SelectedItem.Name}'?\n\nThis action cannot be undone.";
        if (!ShowConfirmation(message, "Delete Configuration"))
            return;

        await ExecuteAsync(async () =>
        {
            var success = await _configurationStorage.DeleteAsync(SelectedItem.Id);
            if (success)
            {
                ShowSuccess($"Configuration '{SelectedItem.Name}' deleted successfully");
                await LoadItemsAsync();
            }
            else
            {
                ShowWarning("Failed to delete configuration");
            }
        }, "Deleting configuration...");
    }

    [RelayCommand]
    private async Task ImportConfigurationAsync()
    {
        var filePath = _dialogService.ShowOpenFileDialog(
            "OpenBullet Configuration (*.anom)|*.anom|All files (*.*)|*.*",
            "Import Configuration");

        if (string.IsNullOrEmpty(filePath)) return;

        await ExecuteAsync(async () =>
        {
            // TODO: Import logic will be enhanced in Step 13 with full parsing
            var content = await _fileService.ReadTextFileAsync(filePath);
            var fileName = _fileService.GetFileNameWithoutExtension(filePath);
            
            var config = new ConfigurationEntity
            {
                Name = fileName,
                Script = content,
                Category = "Imported",
                Author = "Unknown",
                Version = "1.0.0",
                IsActive = true
            };

            await _configurationStorage.SaveAsync(config);
            ShowSuccess($"Configuration '{config.Name}' imported successfully");
            await LoadItemsAsync();
            await LoadCategoriesAsync();
            
        }, "Importing configuration...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task ExportConfigurationAsync()
    {
        if (SelectedItem == null) return;

        var filePath = _dialogService.ShowSaveFileDialog(
            "OpenBullet Configuration (*.anom)|*.anom|All files (*.*)|*.*",
            "Export Configuration",
            $"{SelectedItem.Name}.anom");

        if (string.IsNullOrEmpty(filePath)) return;

        await ExecuteAsync(async () =>
        {
            await _fileService.WriteTextFileAsync(filePath, SelectedItem.Script);
            ShowSuccess($"Configuration exported to '{filePath}'");
            
        }, "Exporting configuration...");
    }

    [RelayCommand]
    private async Task ExportSelectedConfigurationsAsync()
    {
        if (!SelectedItems.Any())
        {
            ShowWarning("Please select configurations to export");
            return;
        }

        var folderPath = _dialogService.ShowOpenFolderDialog("Select Export Folder");
        if (string.IsNullOrEmpty(folderPath)) return;

        await ExecuteAsync(async () =>
        {
            var exportedCount = 0;
            foreach (var config in SelectedItems)
            {
                var filePath = _fileService.CombinePaths(folderPath, $"{config.Name}.anom");
                await _fileService.WriteTextFileAsync(filePath, config.Script);
                exportedCount++;
            }
            
            ShowSuccess($"{exportedCount} configurations exported successfully");
            
        }, $"Exporting {SelectedItems.Count} configurations...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task ViewUsageStatsAsync()
    {
        if (SelectedItem == null) return;

        await ExecuteAsync(async () =>
        {
            var stats = await _configurationStorage.GetUsageStatsAsync(SelectedItem.Id);
            
            var statsMessage = $"""
                Configuration Usage Statistics
                
                Configuration: {SelectedItem.Name}
                Author: {SelectedItem.Author}
                Version: {SelectedItem.Version}
                Category: {SelectedItem.Category}
                
                Usage Statistics:
                • Total Jobs: {stats.TotalJobs}
                • Successful Jobs: {stats.SuccessfulJobs}
                • Success Rate: {(stats.TotalJobs > 0 ? (double)stats.SuccessfulJobs / stats.TotalJobs * 100 : 0):F1}%
                • Last Used: {stats.LastUsed?.ToString("g") ?? "Never"}
                • Average Success Rate: {stats.AverageSuccessRate?.ToString("F1") ?? "N/A"}%
                • Average Execution Time: {stats.AverageExecutionTime?.ToString(@"mm\:ss") ?? "N/A"}
                
                Created: {SelectedItem.CreatedAt:g}
                Updated: {SelectedItem.UpdatedAt:g}
                """;
            
            _dialogService.ShowInformation(statsMessage, "Usage Statistics");
            
        }, "Loading usage statistics...");
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
            SortDescending = false;
        }

        await LoadItemsAsync();
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task ToggleActiveStatusAsync()
    {
        if (SelectedItem == null) return;

        await ExecuteAsync(async () =>
        {
            SelectedItem.IsActive = !SelectedItem.IsActive;
            await _configurationStorage.SaveAsync(SelectedItem);
            
            var status = SelectedItem.IsActive ? "activated" : "deactivated";
            ShowSuccess($"Configuration '{SelectedItem.Name}' {status}");
            
        }, "Updating configuration status...");
    }

    [RelayCommand]
    private async Task BulkDeleteAsync()
    {
        if (!SelectedItems.Any())
        {
            ShowWarning("Please select configurations to delete");
            return;
        }

        var message = $"Are you sure you want to delete {SelectedItems.Count} selected configurations?\n\nThis action cannot be undone.";
        if (!ShowConfirmation(message, "Bulk Delete"))
            return;

        await ExecuteAsync(async () =>
        {
            var deletedCount = 0;
            var failedCount = 0;

            foreach (var config in SelectedItems.ToList())
            {
                try
                {
                    var success = await _configurationStorage.DeleteAsync(config.Id);
                    if (success)
                        deletedCount++;
                    else
                        failedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete configuration {ConfigName}", config.Name);
                    failedCount++;
                }
            }

            if (deletedCount > 0)
            {
                ShowSuccess($"{deletedCount} configurations deleted successfully");
            }
            
            if (failedCount > 0)
            {
                ShowWarning($"{failedCount} configurations could not be deleted");
            }

            await LoadItemsAsync();
            ClearSelection();
            
        }, $"Deleting {SelectedItems.Count} configurations...");
    }

    public override async Task InitializeAsync()
    {
        await LoadCategoriesAsync();
        await base.InitializeAsync();
    }
}
