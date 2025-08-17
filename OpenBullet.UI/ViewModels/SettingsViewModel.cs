using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenBullet.Core.Data;
using OpenBullet.UI.Services;
using System.Collections.ObjectModel;

namespace OpenBullet.UI.ViewModels;

/// <summary>
/// Settings view model for managing application configuration
/// </summary>
public partial class SettingsViewModel : ViewModelBase, INavigationAware
{
    private readonly ISettingsStorage _settingsStorage;
    private readonly IThemeService _themeService;
    private readonly IDatabaseManager _databaseManager;
    private readonly IOptions<UIConfiguration> _uiConfig;

    [ObservableProperty]
    private ObservableCollection<SettingCategory> _categories = new();

    [ObservableProperty]
    private SettingCategory? _selectedCategory;

    [ObservableProperty]
    private ObservableCollection<SettingItem> _currentSettings = new();

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    // General Settings
    [ObservableProperty]
    private int _maxConcurrentJobs = 5;

    [ObservableProperty]
    private int _defaultTimeout = 30;

    [ObservableProperty]
    private bool _enableLogging = true;

    [ObservableProperty]
    private string _logLevel = "Information";

    [ObservableProperty]
    private int _autoSaveInterval = 300;

    // UI Settings
    [ObservableProperty]
    private string _currentTheme = "Dark";

    [ObservableProperty]
    private string _primaryColor = "DeepPurple";

    [ObservableProperty]
    private bool _enableAnimations = true;

    [ObservableProperty]
    private bool _showNotifications = true;

    [ObservableProperty]
    private int _autoRefreshInterval = 5000;

    [ObservableProperty]
    private bool _minimizeToTray = true;

    [ObservableProperty]
    private bool _startMinimized = false;

    // Proxy Settings
    [ObservableProperty]
    private int _proxyHealthCheckInterval = 300;

    [ObservableProperty]
    private bool _autoProxyBanEnabled = true;

    [ObservableProperty]
    private int _maxFailuresBeforeBan = 3;

    [ObservableProperty]
    private int _proxyBanDuration = 600;

    // Export Settings
    [ObservableProperty]
    private string _defaultExportFormat = "JSON";

    [ObservableProperty]
    private bool _includeMetadataInExport = true;

    [ObservableProperty]
    private int _maxRecentConfigs = 10;

    [ObservableProperty]
    private int _maxRecentJobs = 20;

    // Database Settings
    [ObservableProperty]
    private bool _autoBackupEnabled = true;

    [ObservableProperty]
    private int _backupIntervalHours = 24;

    [ObservableProperty]
    private int _maxBackupCount = 7;

    [ObservableProperty]
    private string _databaseProvider = "SQLite";

    public SettingsViewModel(
        ILogger<SettingsViewModel> logger,
        IDialogService dialogService,
        INotificationService notificationService,
        ISettingsStorage settingsStorage,
        IThemeService themeService,
        IDatabaseManager databaseManager,
        IOptions<UIConfiguration> uiConfig)
        : base(logger, dialogService, notificationService)
    {
        _settingsStorage = settingsStorage;
        _themeService = themeService;
        _databaseManager = databaseManager;
        _uiConfig = uiConfig;
        
        Title = "Settings";
        InitializeCategories();
    }

    public void OnNavigatedTo()
    {
        _ = Task.Run(LoadSettingsAsync);
    }

    public void OnNavigatedFrom()
    {
        if (HasUnsavedChanges)
        {
            var save = ShowConfirmation("You have unsaved changes. Save before leaving?", "Unsaved Changes");
            if (save)
            {
                _ = Task.Run(SaveSettingsAsync);
            }
        }
    }

    private void InitializeCategories()
    {
        Categories = new ObservableCollection<SettingCategory>
        {
            new("General", "Cog", "Basic application settings"),
            new("User Interface", "Palette", "Theme and UI preferences"),
            new("Proxy Management", "ServerNetwork", "Proxy pool configuration"),
            new("Export/Import", "Export", "Data export and import settings"),
            new("Database", "Database", "Database and backup settings"),
            new("Advanced", "SettingsHelper", "Advanced configuration options")
        };

        SelectedCategory = Categories.First();
    }

    partial void OnSelectedCategoryChanged(SettingCategory? value)
    {
        if (value != null)
        {
            LoadCategorySettings(value.Name);
        }
    }

    private void LoadCategorySettings(string categoryName)
    {
        CurrentSettings.Clear();

        switch (categoryName)
        {
            case "General":
                LoadGeneralSettings();
                break;
            case "User Interface":
                LoadUISettings();
                break;
            case "Proxy Management":
                LoadProxySettings();
                break;
            case "Export/Import":
                LoadExportSettings();
                break;
            case "Database":
                LoadDatabaseSettings();
                break;
            case "Advanced":
                LoadAdvancedSettings();
                break;
        }
    }

    private void LoadGeneralSettings()
    {
        CurrentSettings = new ObservableCollection<SettingItem>
        {
            new("Max Concurrent Jobs", "Maximum number of jobs that can run simultaneously", MaxConcurrentJobs, 1, 50),
            new("Default Timeout", "Default timeout for operations in seconds", DefaultTimeout, 5, 300),
            new("Enable Logging", "Enable application logging", EnableLogging),
            new("Log Level", "Minimum log level", LogLevel, new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical" }),
            new("Auto Save Interval", "Auto save interval in seconds", AutoSaveInterval, 60, 3600)
        };
    }

    private void LoadUISettings()
    {
        CurrentSettings = new ObservableCollection<SettingItem>
        {
            new("Theme", "Application color theme", CurrentTheme, new[] { "Light", "Dark" }),
            new("Primary Color", "Primary theme color", PrimaryColor, _themeService.AvailablePrimaryColors.ToArray()),
            new("Enable Animations", "Enable UI animations", EnableAnimations),
            new("Show Notifications", "Show toast notifications", ShowNotifications),
            new("Auto Refresh Interval", "Auto refresh interval in milliseconds", AutoRefreshInterval, 1000, 30000),
            new("Minimize to Tray", "Minimize to system tray instead of closing", MinimizeToTray),
            new("Start Minimized", "Start application minimized", StartMinimized)
        };
    }

    private void LoadProxySettings()
    {
        CurrentSettings = new ObservableCollection<SettingItem>
        {
            new("Health Check Interval", "Proxy health check interval in seconds", ProxyHealthCheckInterval, 60, 3600),
            new("Auto Ban Enabled", "Automatically ban failed proxies", AutoProxyBanEnabled),
            new("Max Failures Before Ban", "Maximum failures before banning proxy", MaxFailuresBeforeBan, 1, 20),
            new("Ban Duration", "Proxy ban duration in seconds", ProxyBanDuration, 60, 86400)
        };
    }

    private void LoadExportSettings()
    {
        CurrentSettings = new ObservableCollection<SettingItem>
        {
            new("Default Export Format", "Default format for data export", DefaultExportFormat, new[] { "JSON", "CSV", "XML" }),
            new("Include Metadata", "Include metadata in exports", IncludeMetadataInExport),
            new("Max Recent Configs", "Maximum recent configurations to remember", MaxRecentConfigs, 5, 50),
            new("Max Recent Jobs", "Maximum recent jobs to show", MaxRecentJobs, 10, 100)
        };
    }

    private void LoadDatabaseSettings()
    {
        CurrentSettings = new ObservableCollection<SettingItem>
        {
            new("Auto Backup Enabled", "Enable automatic database backups", AutoBackupEnabled),
            new("Backup Interval", "Backup interval in hours", BackupIntervalHours, 1, 168),
            new("Max Backup Count", "Maximum number of backups to keep", MaxBackupCount, 1, 30),
            new("Database Provider", "Database provider type", DatabaseProvider, new[] { "SQLite", "LiteDB", "InMemory" })
        };
    }

    private void LoadAdvancedSettings()
    {
        CurrentSettings = new ObservableCollection<SettingItem>
        {
            new("Debug Mode", "Enable debug mode features", false),
            new("Performance Monitoring", "Enable performance monitoring", true),
            new("Crash Reporting", "Enable automatic crash reporting", true),
            new("Telemetry", "Enable anonymous usage telemetry", false)
        };
    }

    private async Task LoadSettingsAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Load General settings
            MaxConcurrentJobs = await _settingsStorage.GetValueAsync("General.MaxConcurrentJobs", 5);
            DefaultTimeout = await _settingsStorage.GetValueAsync("General.DefaultTimeout", 30);
            EnableLogging = await _settingsStorage.GetValueAsync("General.EnableLogging", true);
            LogLevel = await _settingsStorage.GetValueAsync("General.LogLevel", "Information");
            AutoSaveInterval = await _settingsStorage.GetValueAsync("General.AutoSaveInterval", 300);

            // Load UI settings
            CurrentTheme = _themeService.CurrentTheme;
            PrimaryColor = _themeService.CurrentPrimaryColor;
            EnableAnimations = await _settingsStorage.GetValueAsync("UI.EnableAnimations", true);
            ShowNotifications = await _settingsStorage.GetValueAsync("UI.ShowNotifications", true);
            AutoRefreshInterval = await _settingsStorage.GetValueAsync("UI.AutoRefreshInterval", 5000);
            MinimizeToTray = await _settingsStorage.GetValueAsync("UI.MinimizeToTray", true);
            StartMinimized = await _settingsStorage.GetValueAsync("UI.StartMinimized", false);

            // Load Proxy settings
            ProxyHealthCheckInterval = await _settingsStorage.GetValueAsync("Proxy.HealthCheckInterval", 300);
            AutoProxyBanEnabled = await _settingsStorage.GetValueAsync("Proxy.AutoBanEnabled", true);
            MaxFailuresBeforeBan = await _settingsStorage.GetValueAsync("Proxy.MaxFailuresBeforeBan", 3);
            ProxyBanDuration = await _settingsStorage.GetValueAsync("Proxy.BanDuration", 600);

            // Load Export settings
            DefaultExportFormat = await _settingsStorage.GetValueAsync("Export.DefaultFormat", "JSON");
            IncludeMetadataInExport = await _settingsStorage.GetValueAsync("Export.IncludeMetadata", true);
            MaxRecentConfigs = await _settingsStorage.GetValueAsync("UI.MaxRecentConfigs", 10);
            MaxRecentJobs = await _settingsStorage.GetValueAsync("UI.MaxRecentJobs", 20);

            // Load Database settings
            AutoBackupEnabled = await _settingsStorage.GetValueAsync("Backup.AutoBackupEnabled", true);
            BackupIntervalHours = await _settingsStorage.GetValueAsync("Backup.BackupInterval", 24);
            MaxBackupCount = await _settingsStorage.GetValueAsync("Backup.MaxBackupCount", 7);

            HasUnsavedChanges = false;
            LoadCategorySettings(SelectedCategory?.Name ?? "General");
            
        }, "Loading settings...");
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Save General settings
            await _settingsStorage.SetValueAsync("General.MaxConcurrentJobs", MaxConcurrentJobs, "Maximum concurrent jobs", "General");
            await _settingsStorage.SetValueAsync("General.DefaultTimeout", DefaultTimeout, "Default timeout seconds", "General");
            await _settingsStorage.SetValueAsync("General.EnableLogging", EnableLogging, "Enable logging", "General");
            await _settingsStorage.SetValueAsync("General.LogLevel", LogLevel, "Log level", "General");
            await _settingsStorage.SetValueAsync("General.AutoSaveInterval", AutoSaveInterval, "Auto save interval", "General");

            // Save UI settings
            await _settingsStorage.SetValueAsync("UI.EnableAnimations", EnableAnimations, "Enable animations", "UI");
            await _settingsStorage.SetValueAsync("UI.ShowNotifications", ShowNotifications, "Show notifications", "UI");
            await _settingsStorage.SetValueAsync("UI.AutoRefreshInterval", AutoRefreshInterval, "Auto refresh interval", "UI");
            await _settingsStorage.SetValueAsync("UI.MinimizeToTray", MinimizeToTray, "Minimize to tray", "UI");
            await _settingsStorage.SetValueAsync("UI.StartMinimized", StartMinimized, "Start minimized", "UI");

            // Save Proxy settings
            await _settingsStorage.SetValueAsync("Proxy.HealthCheckInterval", ProxyHealthCheckInterval, "Health check interval", "Proxy");
            await _settingsStorage.SetValueAsync("Proxy.AutoBanEnabled", AutoProxyBanEnabled, "Auto ban enabled", "Proxy");
            await _settingsStorage.SetValueAsync("Proxy.MaxFailuresBeforeBan", MaxFailuresBeforeBan, "Max failures before ban", "Proxy");
            await _settingsStorage.SetValueAsync("Proxy.BanDuration", ProxyBanDuration, "Ban duration", "Proxy");

            // Save Export settings
            await _settingsStorage.SetValueAsync("Export.DefaultFormat", DefaultExportFormat, "Default export format", "Export");
            await _settingsStorage.SetValueAsync("Export.IncludeMetadata", IncludeMetadataInExport, "Include metadata", "Export");
            await _settingsStorage.SetValueAsync("UI.MaxRecentConfigs", MaxRecentConfigs, "Max recent configs", "UI");
            await _settingsStorage.SetValueAsync("UI.MaxRecentJobs", MaxRecentJobs, "Max recent jobs", "UI");

            // Save Database settings
            await _settingsStorage.SetValueAsync("Backup.AutoBackupEnabled", AutoBackupEnabled, "Auto backup enabled", "Backup");
            await _settingsStorage.SetValueAsync("Backup.BackupInterval", BackupIntervalHours, "Backup interval hours", "Backup");
            await _settingsStorage.SetValueAsync("Backup.MaxBackupCount", MaxBackupCount, "Max backup count", "Backup");

            // Apply theme changes
            if (CurrentTheme != _themeService.CurrentTheme || PrimaryColor != _themeService.CurrentPrimaryColor)
            {
                _themeService.ApplyTheme(CurrentTheme, PrimaryColor);
            }

            HasUnsavedChanges = false;
            ShowSuccess("Settings saved successfully");
            
        }, "Saving settings...");
    }

    [RelayCommand]
    private async Task ResetToDefaultsAsync()
    {
        if (!ShowConfirmation("This will reset all settings to their default values. Continue?", "Reset Settings"))
            return;

        await ExecuteAsync(async () =>
        {
            await _settingsStorage.ResetToDefaultsAsync();
            await LoadSettingsAsync();
            ShowSuccess("Settings reset to defaults");
            
        }, "Resetting settings...");
    }

    [RelayCommand]
    private async Task ExportSettingsAsync()
    {
        var filePath = _dialogService.ShowSaveFileDialog(
            "JSON files (*.json)|*.json|All files (*.*)|*.*",
            "Export Settings",
            "openbullet-settings.json");

        if (string.IsNullOrEmpty(filePath)) return;

        await ExecuteAsync(async () =>
        {
            var success = await _settingsStorage.ExportAsync(filePath);
            if (success)
            {
                ShowSuccess($"Settings exported to '{filePath}'");
            }
            else
            {
                ShowWarning("Failed to export settings");
            }
            
        }, "Exporting settings...");
    }

    [RelayCommand]
    private async Task ImportSettingsAsync()
    {
        var filePath = _dialogService.ShowOpenFileDialog(
            "JSON files (*.json)|*.json|All files (*.*)|*.*",
            "Import Settings");

        if (string.IsNullOrEmpty(filePath)) return;

        if (!ShowConfirmation("This will overwrite current settings. Continue?", "Import Settings"))
            return;

        await ExecuteAsync(async () =>
        {
            var result = await _settingsStorage.ImportAsync(filePath, true);
            
            var message = $"Import Results:\n\n" +
                         $"• Imported: {result.ImportedCount}\n" +
                         $"• Skipped: {result.SkippedCount}\n" +
                         $"• Errors: {result.ErrorCount}";

            if (result.Errors.Any())
            {
                message += $"\n\nErrors:\n{string.Join("\n", result.Errors.Take(5))}";
            }

            _dialogService.ShowInformation(message, "Import Results");
            
            if (result.ImportedCount > 0)
            {
                await LoadSettingsAsync();
            }
            
        }, "Importing settings...");
    }

    [RelayCommand]
    private async Task TestDatabaseConnectionAsync()
    {
        await ExecuteAsync(async () =>
        {
            var health = await _databaseManager.GetHealthAsync();
            
            var message = $"Database Connection Test\n\n" +
                         $"Status: {(health.IsHealthy ? "✓ Connected" : "✗ Failed")}\n" +
                         $"Response Time: {health.ResponseTime.TotalMilliseconds:F0}ms\n" +
                         $"Database Size: {health.SizeBytes / (1024.0 * 1024.0):F1} MB\n" +
                         $"Version: {health.Version}";

            if (health.Issues.Any())
            {
                message += $"\n\nIssues:\n{string.Join("\n", health.Issues)}";
            }

            _dialogService.ShowInformation(message, "Database Test");
            
        }, "Testing database connection...");
    }

    // Property change handlers to track unsaved changes
    partial void OnMaxConcurrentJobsChanged(int value) => HasUnsavedChanges = true;
    partial void OnDefaultTimeoutChanged(int value) => HasUnsavedChanges = true;
    partial void OnEnableLoggingChanged(bool value) => HasUnsavedChanges = true;
    partial void OnLogLevelChanged(string value) => HasUnsavedChanges = true;
    partial void OnCurrentThemeChanged(string value) => HasUnsavedChanges = true;
    partial void OnPrimaryColorChanged(string value) => HasUnsavedChanges = true;
    partial void OnEnableAnimationsChanged(bool value) => HasUnsavedChanges = true;
    partial void OnShowNotificationsChanged(bool value) => HasUnsavedChanges = true;
    partial void OnProxyHealthCheckIntervalChanged(int value) => HasUnsavedChanges = true;
    partial void OnAutoProxyBanEnabledChanged(bool value) => HasUnsavedChanges = true;
    partial void OnDefaultExportFormatChanged(string value) => HasUnsavedChanges = true;
    partial void OnAutoBackupEnabledChanged(bool value) => HasUnsavedChanges = true;

    public override async Task InitializeAsync()
    {
        await LoadSettingsAsync();
    }
}

/// <summary>
/// Setting category model
/// </summary>
public class SettingCategory
{
    public string Name { get; }
    public string Icon { get; }
    public string Description { get; }

    public SettingCategory(string name, string icon, string description)
    {
        Name = name;
        Icon = icon;
        Description = description;
    }

    public override string ToString() => Name;
}

/// <summary>
/// Setting item model for dynamic settings display
/// </summary>
public class SettingItem
{
    public string Name { get; }
    public string Description { get; }
    public object Value { get; set; }
    public SettingType Type { get; }
    public object? MinValue { get; }
    public object? MaxValue { get; }
    public string[]? Options { get; }

    public SettingItem(string name, string description, bool value)
    {
        Name = name;
        Description = description;
        Value = value;
        Type = SettingType.Boolean;
    }

    public SettingItem(string name, string description, int value, int minValue, int maxValue)
    {
        Name = name;
        Description = description;
        Value = value;
        Type = SettingType.Integer;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public SettingItem(string name, string description, string value, string[] options)
    {
        Name = name;
        Description = description;
        Value = value;
        Type = SettingType.Choice;
        Options = options;
    }

    public SettingItem(string name, string description, string value)
    {
        Name = name;
        Description = description;
        Value = value;
        Type = SettingType.String;
    }
}

/// <summary>
/// Setting type enumeration
/// </summary>
public enum SettingType
{
    Boolean,
    Integer,
    String,
    Choice
}
