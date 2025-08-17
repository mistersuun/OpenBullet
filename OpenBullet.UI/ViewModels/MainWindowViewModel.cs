using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.UI.Services;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Main window view model with navigation and status management
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly IDatabaseManager _databaseManager;

    [ObservableProperty]
    private UserControl? _currentView;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private bool _isConnected = true;

    [ObservableProperty]
    private string _databaseStatus = "Unknown";

    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems = new();

    [ObservableProperty]
    private NavigationItem? _selectedNavigationItem;

    [ObservableProperty]
    private string _currentTheme = "Dark";

    [ObservableProperty]
    private string _windowTitle = "OpenBullet - Professional Web Automation Platform";

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        INavigationService navigationService,
        IThemeService themeService,
        IDatabaseManager databaseManager)
        : base(logger, dialogService, notificationService)
    {
        _navigationService = navigationService;
        _themeService = themeService;
        _databaseManager = databaseManager;
        
        Title = "OpenBullet Main";
        InitializeNavigation();
        
        // Subscribe to navigation changes
        _navigationService.CurrentViewChanged += OnCurrentViewChanged;
        _themeService.ThemeChanged += OnThemeChanged;
    }

    public override async Task InitializeAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Initialize navigation to dashboard
            _navigationService.NavigateTo<DashboardViewModel>();
            
            // Check database status
            await UpdateDatabaseStatusAsync();
            
            // Set initial status
            StatusText = "Application initialized successfully";
            
        }, "Initializing application...");
    }

    private void InitializeNavigation()
    {
        NavigationItems = new ObservableCollection<NavigationItem>
        {
            new("Dashboard", "ViewDashboard", typeof(DashboardViewModel), "Real-time monitoring and statistics"),
            new("Configurations", "FileDocumentMultiple", typeof(ConfigurationListViewModel), "Manage automation configurations"),
            new("Jobs", "PlayBoxMultiple", typeof(JobListViewModel), "Monitor and manage automation jobs"),
            new("Proxies", "ServerNetwork", typeof(ProxyListViewModel), "Manage proxy pools and health"),
            new("Settings", "Cog", typeof(SettingsViewModel), "Application settings and preferences")
        };

        SelectedNavigationItem = NavigationItems.First();
    }

    private void OnCurrentViewChanged(object? sender, NavigationEventArgs e)
    {
        CurrentView = e.CurrentView;
        
        // Update selected navigation item
        if (e.ViewModel != null)
        {
            var navItem = NavigationItems.FirstOrDefault(n => n.ViewModelType == e.ViewModel.GetType());
            if (navItem != null)
            {
                SelectedNavigationItem = navItem;
            }
        }
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        CurrentTheme = e.CurrentTheme;
    }

    private async Task UpdateDatabaseStatusAsync()
    {
        try
        {
            var health = await _databaseManager.GetHealthAsync();
            DatabaseStatus = health.IsHealthy ? "Connected" : "Error";
            IsConnected = health.IsHealthy;
            
            if (!health.IsHealthy)
            {
                ShowWarning("Database connection issues detected. Some features may not work properly.");
            }
        }
        catch (Exception ex)
        {
            DatabaseStatus = "Disconnected";
            IsConnected = false;
            _logger.LogError(ex, "Failed to check database status");
        }
    }

    partial void OnSelectedNavigationItemChanged(NavigationItem? value)
    {
        if (value != null && value.ViewModelType != null)
        {
            NavigateToView(value.ViewModelType);
        }
    }

    [RelayCommand]
    private void NavigateToView(Type viewModelType)
    {
        try
        {
            if (viewModelType == typeof(DashboardViewModel))
                _navigationService.NavigateTo<DashboardViewModel>();
            else if (viewModelType == typeof(ConfigurationListViewModel))
                _navigationService.NavigateTo<ConfigurationListViewModel>();
            else if (viewModelType == typeof(JobListViewModel))
                _navigationService.NavigateTo<JobListViewModel>();
            else if (viewModelType == typeof(ProxyListViewModel))
                _navigationService.NavigateTo<ProxyListViewModel>();
            else if (viewModelType == typeof(SettingsViewModel))
                _navigationService.NavigateTo<SettingsViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to {ViewModelType}", viewModelType.Name);
            HandleError(ex);
        }
    }

    [RelayCommand]
    private async Task RefreshStatusAsync()
    {
        await UpdateDatabaseStatusAsync();
        ShowInfo("Status refreshed");
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        var newTheme = CurrentTheme == "Dark" ? "Light" : "Dark";
        _themeService.ApplyTheme(newTheme, _themeService.CurrentPrimaryColor);
    }

    [RelayCommand]
    private async Task ShowAboutAsync()
    {
        var aboutMessage = """
            OpenBullet - Professional Web Automation Platform
            
            Version: 1.0.0
            Built with: .NET 8.0, WPF, Entity Framework Core
            
            A powerful automation platform for web testing,
            data extraction, and automated workflows.
            
            © 2024 OpenBullet Community
            """;

        _dialogService.ShowInformation(aboutMessage, "About OpenBullet");
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
    private async Task OptimizeDatabaseAsync()
    {
        if (!ShowConfirmation("This will optimize the database and may take some time. Continue?"))
            return;

        await ExecuteAsync(async () =>
        {
            await _databaseManager.OptimizeAsync();
            ShowSuccess("Database optimization completed");
            await UpdateDatabaseStatusAsync();
        }, "Optimizing database...");
    }

    [RelayCommand]
    private async Task ExitApplicationAsync()
    {
        if (ShowConfirmation("Are you sure you want to exit OpenBullet?", "Exit Application"))
        {
            System.Windows.Application.Current.Shutdown();
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        if (_navigationService.CanGoBack)
        {
            _navigationService.GoBack();
        }
    }

    [RelayCommand]
    private void GoForward()
    {
        if (_navigationService.CanGoForward)
        {
            _navigationService.GoForward();
        }
    }

    public bool CanGoBack => _navigationService.CanGoBack;
    public bool CanGoForward => _navigationService.CanGoForward;

    public override void Cleanup()
    {
        _navigationService.CurrentViewChanged -= OnCurrentViewChanged;
        _themeService.ThemeChanged -= OnThemeChanged;
        base.Cleanup();
    }
}

/// <summary>
/// Navigation item for the main menu
/// </summary>
public class NavigationItem
{
    public string Name { get; }
    public string Icon { get; }
    public Type? ViewModelType { get; }
    public string Description { get; }

    public NavigationItem(string name, string icon, Type? viewModelType, string description)
    {
        Name = name;
        Icon = icon;
        ViewModelType = viewModelType;
        Description = description;
    }

    public override string ToString() => Name;
}
