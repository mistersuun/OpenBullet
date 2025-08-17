using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.Core.Services;
using OpenBullet.UI.Services;
using OpenBullet.UI.ViewModels;
using OpenBullet.UI.Views;
using Serilog;
using System.IO;
using System.Windows;

namespace OpenBullet.UI;

/// <summary>
/// Main application class with dependency injection and hosting
/// </summary>
public partial class App : Application
{
    private IHost? _host;
    
    /// <summary>
    /// Global service provider for accessing services from UI controls
    /// </summary>
    public static IServiceProvider? ServiceProvider { get; private set; }

    public App()
    {
        // Handle UI thread exceptions
        this.DispatcherUnhandledException += (sender, e) =>
        {
            Log.Fatal(e.Exception, "Unhandled UI thread exception: {ExceptionType} - {Message}", 
                e.Exception.GetType().Name, e.Exception.Message);
            e.Handled = true;
        };

        // Handle application domain exceptions
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "Unhandled application domain exception: {ExceptionType} - {Message}", 
                e.ExceptionObject?.GetType().Name, e.ExceptionObject?.ToString());
        };
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            // Configure Serilog with more detailed logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Change to Debug for more detailed logging
                .WriteTo.File("Logs/openbullet-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application startup initiated");

            // Ensure Logs directory exists
            Directory.CreateDirectory("Logs");
            Log.Information("Logs directory ensured");

            // Build host with dependency injection
            Log.Information("Building host with dependency injection...");
            _host = CreateHostBuilder(e.Args).Build();
            Log.Information("Host built successfully");
            
            // Set global service provider for UI controls
            ServiceProvider = _host.Services;

            // Log basic service information for debugging
            LogServiceInfo(_host.Services);
            
            // Initialize database
            Log.Information("Initializing database...");
            await _host.Services.InitializeDatabaseAsync();
            Log.Information("Database initialized successfully");
            
            // Start the host
            Log.Information("Starting host...");
            await _host.StartAsync();
            Log.Information("Host started successfully");

            // Show main window
            Log.Information("Getting MainWindow service...");
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            Log.Information("MainWindow service retrieved successfully");
            
            Log.Information("Getting MainWindowViewModel service...");
            var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
            Log.Information("MainWindowViewModel service retrieved successfully");
            
            Log.Information("Setting MainWindow DataContext...");
            mainWindow.DataContext = mainWindowViewModel;
            Log.Information("MainWindow DataContext set successfully");
            
            Log.Information("Showing main window...");
            mainWindow.Show();
            Log.Information("Main window shown successfully");

            base.OnStartup(e);
            Log.Information("Application startup completed successfully");
            
            // Keep the application running
            Log.Information("Application is now running - keeping reference to main window");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed with exception: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
            
            // Log the full stack trace for debugging
            if (ex.StackTrace != null)
            {
                Log.Fatal("Stack trace: {StackTrace}", ex.StackTrace);
            }
            
            // Log inner exception if present
            if (ex.InnerException != null)
            {
                Log.Fatal("Inner exception: {InnerExceptionType} - {InnerExceptionMessage}", 
                    ex.InnerException.GetType().Name, ex.InnerException.Message);
            }
            
            MessageBox.Show($"Application failed to start: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
        finally
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services, context.Configuration);
            });

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            // Ensure database directory exists
            var databaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(databaseDirectory);
            
            // Database configuration
            var databaseOptions = new DatabaseOptions
            {
                Provider = DatabaseProvider.SQLite,
                ConnectionString = $"Data Source={Path.Combine(databaseDirectory, "openbullet.db")}",
                EnableLogging = true,
                AutoMigrate = true,
                BackupDirectory = Path.Combine(databaseDirectory, "Backups"),
                BackupInterval = TimeSpan.FromHours(6),
                MaxBackupCount = 10
            };

            // Register database services
            services.AddOpenBulletDatabase(databaseOptions);

            // Register UI services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IFileService, FileService>();
            
            // Register editor services
            services.AddSingleton<ISyntaxHighlightingService, SyntaxHighlightingService>();
            services.AddSingleton<AutoCompletionProvider>();
            
            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ConfigurationListViewModel>();
            services.AddTransient<ConfigurationDetailViewModel>();
            services.AddTransient<JobListViewModel>();
            services.AddTransient<JobDetailViewModel>();
            services.AddTransient<ProxyListViewModel>();
            services.AddTransient<ProxyDetailViewModel>();
            services.AddTransient<SettingsViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
            services.AddTransient<DashboardView>();
            services.AddTransient<ConfigurationListView>();
            services.AddTransient<ConfigurationDetailView>();
            services.AddTransient<JobListView>();
            services.AddTransient<JobDetailView>();
            services.AddTransient<ProxyListView>();
            services.AddTransient<ProxyDetailView>();
            services.AddTransient<SettingsView>();

            // Register managers and factories
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<IViewFactory, ViewFactory>();

            // Configuration
            services.Configure<UIConfiguration>(configuration.GetSection("UI"));
            services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
        }
        catch (Exception ex)
        {
            // Log the error but re-throw it so the calling code can handle it
            throw new InvalidOperationException($"Failed to configure services: {ex.Message}", ex);
        }
    }

    private void LogServiceInfo(IServiceProvider serviceProvider)
    {
        try
        {
            Log.Information("Logging basic service information...");
            
            // Test key services to see if they're available
            var navigationService = serviceProvider.GetService<INavigationService>();
            Log.Information("NavigationService available: {Available}", navigationService != null);
            
            var dialogService = serviceProvider.GetService<IDialogService>();
            Log.Information("DialogService available: {Available}", dialogService != null);
            
            var mainWindowVM = serviceProvider.GetService<MainWindowViewModel>();
            Log.Information("MainWindowViewModel available: {Available}", mainWindowVM != null);
            
            var mainWindow = serviceProvider.GetService<MainWindow>();
            Log.Information("MainWindow available: {Available}", mainWindow != null);
            
            Log.Information("Basic service availability check completed");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to log service information");
        }
    }
}

/// <summary>
/// UI configuration options
/// </summary>
public class UIConfiguration
{
    public string Theme { get; set; } = "Dark";
    public string PrimaryColor { get; set; } = "DeepPurple";
    public string AccentColor { get; set; } = "Lime";
    public bool EnableAnimations { get; set; } = true;
    public bool ShowNotifications { get; set; } = true;
    public int AutoRefreshInterval { get; set; } = 5000; // milliseconds
    public bool MinimizeToTray { get; set; } = true;
    public bool StartMinimized { get; set; } = false;
    public string DefaultExportFormat { get; set; } = "JSON";
    public int MaxRecentConfigs { get; set; } = 10;
    public int MaxRecentJobs { get; set; } = 20;
}
