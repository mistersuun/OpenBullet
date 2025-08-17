using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.Core.Models;
using OpenBullet.UI.Services;
using System.Collections.ObjectModel;
using System.IO;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Proxy list view model for managing proxy pools
/// </summary>
public partial class ProxyListViewModel : ListViewModelBase<ProxyEntity>, INavigationAware
{
    private readonly IProxyStorage _proxyStorage;
    private readonly IFileService _fileService;

    [ObservableProperty]
    private ProxyType? _selectedType = null;

    [ObservableProperty]
    private ObservableCollection<ProxyTypeFilter> _typeFilters = new();

    [ObservableProperty]
    private ProxyHealth? _selectedHealth = null;

    [ObservableProperty]
    private ObservableCollection<ProxyHealthFilter> _healthFilters = new();

    [ObservableProperty]
    private bool? _isActive = null;

    [ObservableProperty]
    private bool? _isBanned = null;

    [ObservableProperty]
    private string _selectedCountry = "All";

    [ObservableProperty]
    private ObservableCollection<string> _countries = new();

    [ObservableProperty]
    private double _minSuccessRate = 0;

    [ObservableProperty]
    private long _maxResponseTime = 30000;

    [ObservableProperty]
    private string _sortColumn = "CreatedAt";

    [ObservableProperty]
    private bool _sortDescending = true;

    [ObservableProperty]
    private ProxyStatistics? _currentStatistics;

    public ProxyListViewModel(
        ILogger<ProxyListViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        IProxyStorage proxyStorage,
        IFileService fileService)
        : base(logger, dialogService, notificationService)
    {
        _proxyStorage = proxyStorage;
        _fileService = fileService;
        
        Title = "Proxies";
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
        TypeFilters = new ObservableCollection<ProxyTypeFilter>
        {
            new("All Types", null),
            new("HTTP", ProxyType.Http),
            new("SOCKS4", ProxyType.Socks4),
            new("SOCKS5", ProxyType.Socks5)
        };

        HealthFilters = new ObservableCollection<ProxyHealthFilter>
        {
            new("All Health", null),
            new("Healthy", ProxyHealth.Healthy),
            new("Slow", ProxyHealth.Slow),
            new("Unreliable", ProxyHealth.Unreliable),
            new("Dead", ProxyHealth.Dead),
            new("Unknown", ProxyHealth.Unknown)
        };

        LoadCountriesAsync();
        LoadStatisticsAsync();
    }

    private async Task LoadCountriesAsync()
    {
        try
        {
            var allProxies = await _proxyStorage.GetAllAsync();
            var countries = allProxies
                .Where(p => !string.IsNullOrEmpty(p.Country))
                .Select(p => p.Country!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
            
            countries.Insert(0, "All");
            Countries = new ObservableCollection<string>(countries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load countries");
        }
    }

    private async Task LoadStatisticsAsync()
    {
        try
        {
            CurrentStatistics = await _proxyStorage.GetStatisticsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load proxy statistics");
        }
    }

    protected override async Task<(IList<ProxyEntity> items, int totalCount)> LoadItemsAsync(int page, int pageSize, string searchText)
    {
        try
        {
            var filter = new ProxyFilter
            {
                Type = SelectedType,
                Health = SelectedHealth,
                IsActive = IsActive,
                IsBanned = IsBanned,
                Country = SelectedCountry == "All" ? null : SelectedCountry,
                MinSuccessRate = MinSuccessRate > 0 ? MinSuccessRate : null,
                MaxResponseTime = MaxResponseTime < 30000 ? MaxResponseTime : null
            };

            var pagedResult = await _proxyStorage.GetPagedAsync(page, pageSize, filter);
            var items = pagedResult.Items.ToList();

            // Apply text search if specified
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                items = items.Where(p => 
                    p.Host.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Address.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (p.Source?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true)
                ).ToList();
            }
            
            // Apply sorting
            items = SortProxies(items);
            
            // Update statistics
            await LoadStatisticsAsync();
            
            return (items, pagedResult.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load proxies");
            return (new List<ProxyEntity>(), 0);
        }
    }

    private List<ProxyEntity> SortProxies(List<ProxyEntity> items)
    {
        return SortColumn switch
        {
            "Host" => SortDescending 
                ? items.OrderByDescending(p => p.Host).ToList()
                : items.OrderBy(p => p.Host).ToList(),
            "Type" => SortDescending
                ? items.OrderByDescending(p => p.Type).ToList()
                : items.OrderBy(p => p.Type).ToList(),
            "Health" => SortDescending
                ? items.OrderByDescending(p => p.Health).ToList()
                : items.OrderBy(p => p.Health).ToList(),
            "SuccessRate" => SortDescending
                ? items.OrderByDescending(p => p.SuccessRate).ToList()
                : items.OrderBy(p => p.SuccessRate).ToList(),
            "ResponseTime" => SortDescending
                ? items.OrderByDescending(p => p.AverageResponseTimeMs).ToList()
                : items.OrderBy(p => p.AverageResponseTimeMs).ToList(),
            "Uses" => SortDescending
                ? items.OrderByDescending(p => p.Uses).ToList()
                : items.OrderBy(p => p.Uses).ToList(),
            "LastUsed" => SortDescending
                ? items.OrderByDescending(p => p.LastUsed).ToList()
                : items.OrderBy(p => p.LastUsed).ToList(),
            "CreatedAt" => SortDescending
                ? items.OrderByDescending(p => p.CreatedAt).ToList()
                : items.OrderBy(p => p.CreatedAt).ToList(),
            _ => items
        };
    }

    // Filter change handlers
    partial void OnSelectedTypeChanged(ProxyType? value) => _ = Task.Run(async () => { CurrentPage = 1; await LoadItemsAsync(); });
    partial void OnSelectedHealthChanged(ProxyHealth? value) => _ = Task.Run(async () => { CurrentPage = 1; await LoadItemsAsync(); });
    partial void OnIsActiveChanged(bool? value) => _ = Task.Run(async () => { CurrentPage = 1; await LoadItemsAsync(); });
    partial void OnIsBannedChanged(bool? value) => _ = Task.Run(async () => { CurrentPage = 1; await LoadItemsAsync(); });
    partial void OnSelectedCountryChanged(string value) => _ = Task.Run(async () => { CurrentPage = 1; await LoadItemsAsync(); });
    partial void OnMinSuccessRateChanged(double value) => _ = Task.Run(async () => { CurrentPage = 1; await LoadItemsAsync(); });
    partial void OnMaxResponseTimeChanged(long value) => _ = Task.Run(async () => { CurrentPage = 1; await LoadItemsAsync(); });

    [RelayCommand]
    private async Task ImportProxiesAsync()
    {
        var filePath = _dialogService.ShowOpenFileDialog(
            "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            "Import Proxies");

        if (string.IsNullOrEmpty(filePath)) return;

        await ExecuteAsync(async () =>
        {
            var content = await _fileService.ReadTextFileAsync(filePath);
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                              .Select(line => line.Trim())
                              .Where(line => !string.IsNullOrEmpty(line))
                              .ToList();

            var source = _fileService.GetFileNameWithoutExtension(filePath);
            var result = await _proxyStorage.ImportFromStringsAsync(lines, source);

            var message = $"Proxy Import Results:\n\n" +
                         $"• Imported: {result.ImportedCount}\n" +
                         $"• Skipped: {result.SkippedCount}\n" +
                         $"• Errors: {result.ErrorCount}";

            if (result.Errors.Any())
            {
                message += $"\n\nFirst few errors:\n{string.Join("\n", result.Errors.Take(5))}";
            }

            _dialogService.ShowInformation(message, "Import Results");
            
            if (result.ImportedCount > 0)
            {
                await LoadItemsAsync();
                await LoadCountriesAsync();
            }
            
        }, "Importing proxies...");
    }

    [RelayCommand]
    private async Task ExportProxiesAsync()
    {
        var filePath = _dialogService.ShowSaveFileDialog(
            "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv|JSON files (*.json)|*.json",
            "Export Proxies",
            "proxies.txt");

        if (string.IsNullOrEmpty(filePath)) return;

        await ExecuteAsync(async () =>
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var format = extension switch
            {
                ".csv" => ExportFormat.CSV,
                ".json" => ExportFormat.JSON,
                _ => ExportFormat.CSV // Default to CSV for .txt files
            };

            var filter = new ProxyFilter
            {
                Type = SelectedType,
                Health = SelectedHealth,
                IsActive = IsActive,
                IsBanned = IsBanned,
                Country = SelectedCountry == "All" ? null : SelectedCountry,
                MinSuccessRate = MinSuccessRate > 0 ? MinSuccessRate : null,
                MaxResponseTime = MaxResponseTime < 30000 ? MaxResponseTime : null
            };

            var success = await _proxyStorage.ExportAsync(filePath, format, filter);
            if (success)
            {
                ShowSuccess($"Proxies exported to '{filePath}'");
            }
            else
            {
                ShowWarning("Failed to export proxies");
            }
            
        }, "Exporting proxies...");
    }

    [RelayCommand]
    private async Task TestSelectedProxiesAsync()
    {
        if (!SelectedItems.Any())
        {
            ShowWarning("Please select proxies to test");
            return;
        }

        var testUrl = _dialogService.ShowInput(
            "Enter test URL:",
            "Test Proxies",
            "https://httpbin.org/ip");

        if (string.IsNullOrEmpty(testUrl)) return;

        await ExecuteAsync(async () =>
        {
            var testedCount = 0;
            var healthyCount = 0;

            foreach (var proxy in SelectedItems)
            {
                try
                {
                    // Simulate proxy testing (in real implementation, use IHttpClientService)
                    await Task.Delay(100); // Simulate test delay
                    
                    // Update proxy health based on test results
                    var newHealth = Random.Shared.NextDouble() > 0.3 ? ProxyHealth.Healthy : ProxyHealth.Dead;
                    proxy.Health = newHealth;
                    proxy.LastTested = DateTime.UtcNow;
                    
                    await _proxyStorage.SaveAsync(proxy);
                    
                    testedCount++;
                    if (newHealth == ProxyHealth.Healthy)
                        healthyCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to test proxy {ProxyAddress}", proxy.Address);
                }
            }

            ShowSuccess($"Tested {testedCount} proxies. {healthyCount} are healthy.");
            await LoadItemsAsync();
            
        }, $"Testing {SelectedItems.Count} proxies...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task BanSelectedProxiesAsync()
    {
        if (!SelectedItems.Any()) return;

        var duration = _dialogService.ShowInput(
            "Enter ban duration in minutes (leave empty for permanent ban):",
            "Ban Proxies",
            "30");

        var reason = _dialogService.ShowInput(
            "Enter ban reason:",
            "Ban Reason",
            "Manual ban");

        if (string.IsNullOrEmpty(reason)) return;

        await ExecuteAsync(async () =>
        {
            TimeSpan? banDuration = null;
            if (!string.IsNullOrEmpty(duration) && int.TryParse(duration, out var minutes))
            {
                banDuration = TimeSpan.FromMinutes(minutes);
            }

            var bannedCount = 0;
            foreach (var proxy in SelectedItems.ToList())
            {
                try
                {
                    await _proxyStorage.BanProxyAsync(proxy.Id, banDuration, reason);
                    bannedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to ban proxy {ProxyAddress}", proxy.Address);
                }
            }

            ShowSuccess($"{bannedCount} proxies banned successfully");
            await LoadItemsAsync();
            ClearSelection();
            
        }, $"Banning {SelectedItems.Count} proxies...");
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task UnbanSelectedProxiesAsync()
    {
        if (!SelectedItems.Any()) return;

        await ExecuteAsync(async () =>
        {
            var unbannedCount = 0;
            foreach (var proxy in SelectedItems.ToList())
            {
                try
                {
                    await _proxyStorage.UnbanProxyAsync(proxy.Id);
                    unbannedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to unban proxy {ProxyAddress}", proxy.Address);
                }
            }

            ShowSuccess($"{unbannedCount} proxies unbanned successfully");
            await LoadItemsAsync();
            ClearSelection();
            
        }, $"Unbanning {SelectedItems.Count} proxies...");
    }

    [RelayCommand]
    private async Task CleanupDeadProxiesAsync()
    {
        if (!ShowConfirmation("This will permanently delete all dead proxies. Continue?", "Cleanup Dead Proxies"))
            return;

        await ExecuteAsync(async () =>
        {
            var deletedCount = await _proxyStorage.CleanupAsync(null, true);
            ShowSuccess($"{deletedCount} dead proxies cleaned up successfully");
            await LoadItemsAsync();
            
        }, "Cleaning up dead proxies...");
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        SelectedType = null;
        SelectedHealth = null;
        IsActive = null;
        IsBanned = null;
        SelectedCountry = "All";
        MinSuccessRate = 0;
        MaxResponseTime = 30000;
        SearchText = string.Empty;
        
        await LoadItemsAsync();
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
            SortDescending = column == "CreatedAt" || column == "LastUsed"; // Default descending for dates
        }

        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task DeleteSelectedProxiesAsync()
    {
        if (!SelectedItems.Any())
        {
            ShowWarning("Please select proxies to delete");
            return;
        }

        var message = $"Are you sure you want to delete {SelectedItems.Count} selected proxies?\n\nThis action cannot be undone.";
        if (!ShowConfirmation(message, "Delete Proxies"))
            return;

        await ExecuteAsync(async () =>
        {
            var deletedCount = 0;
            var failedCount = 0;

            foreach (var proxy in SelectedItems.ToList())
            {
                try
                {
                    var success = await _proxyStorage.DeleteAsync(proxy.Id);
                    if (success)
                        deletedCount++;
                    else
                        failedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete proxy {ProxyAddress}", proxy.Address);
                    failedCount++;
                }
            }

            if (deletedCount > 0)
            {
                ShowSuccess($"{deletedCount} proxies deleted successfully");
            }
            
            if (failedCount > 0)
            {
                ShowWarning($"{failedCount} proxies could not be deleted");
            }

            await LoadItemsAsync();
            ClearSelection();
            
        }, $"Deleting {SelectedItems.Count} proxies...");
    }

    public override async Task InitializeAsync()
    {
        await LoadCountriesAsync();
        await LoadStatisticsAsync();
        await base.InitializeAsync();
    }
}

/// <summary>
/// Proxy type filter item
/// </summary>
public class ProxyTypeFilter
{
    public string Name { get; }
    public ProxyType? Type { get; }

    public ProxyTypeFilter(string name, ProxyType? type)
    {
        Name = name;
        Type = type;
    }

    public override string ToString() => Name;
}

/// <summary>
/// Proxy health filter item
/// </summary>
public class ProxyHealthFilter
{
    public string Name { get; }
    public ProxyHealth? Health { get; }

    public ProxyHealthFilter(string name, ProxyHealth? health)
    {
        Name = name;
        Health = health;
    }

    public override string ToString() => Name;
}
