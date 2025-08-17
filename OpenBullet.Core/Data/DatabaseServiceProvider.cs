using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenBullet.Core.Data;

/// <summary>
/// Database service provider for dependency injection and initialization
/// </summary>
public static class DatabaseServiceProvider
{
    /// <summary>
    /// Adds database services to the service collection
    /// </summary>
    public static IServiceCollection AddOpenBulletDatabase(this IServiceCollection services, DatabaseOptions options)
    {
        // Register database options
        services.AddSingleton(options);

        // Register database context based on provider
        switch (options.Provider)
        {
            case DatabaseProvider.SQLite:
                services.AddDbContext<SqliteContext>(contextOptions =>
                {
                    contextOptions.UseSqlite(options.ConnectionString);
                    if (options.EnableLogging)
                    {
                        contextOptions.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                        contextOptions.EnableSensitiveDataLogging(options.EnableSensitiveDataLogging);
                    }
                    // Note: CommandTimeout is set differently in .NET 8
                });
                services.AddScoped<IOpenBulletContext>(provider => provider.GetRequiredService<SqliteContext>());
                break;

            case DatabaseProvider.LiteDB:
                services.AddSingleton<LiteDbContext>();
                services.AddScoped<IOpenBulletContext>(provider => provider.GetRequiredService<LiteDbContext>());
                break;

            case DatabaseProvider.InMemory:
                services.AddDbContext<SqliteContext>(contextOptions =>
                {
                    contextOptions.UseInMemoryDatabase("OpenBulletInMemory");
                    if (options.EnableLogging)
                    {
                        contextOptions.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                    }
                });
                services.AddScoped<IOpenBulletContext>(provider => provider.GetRequiredService<SqliteContext>());
                break;

            default:
                throw new ArgumentException($"Unsupported database provider: {options.Provider}");
        }

        // Register storage services
        services.AddScoped<IConfigurationStorage, ConfigurationStorage>();
        services.AddScoped<IJobStorage, JobStorage>();
        services.AddScoped<IResultStorage, ResultStorage>();
        services.AddScoped<IProxyStorage, ProxyStorage>();
        services.AddScoped<ISettingsStorage, SettingsStorage>();

        // Register database management service
        services.AddScoped<IDatabaseManager, DatabaseManager>();

        return services;
    }

    /// <summary>
    /// Initializes the database with default settings
    /// </summary>
    public static async Task<IServiceProvider> InitializeDatabaseAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var databaseManager = scope.ServiceProvider.GetRequiredService<IDatabaseManager>();
        await databaseManager.InitializeAsync(cancellationToken);
        return serviceProvider;
    }
}

/// <summary>
/// Database manager interface
/// </summary>
public interface IDatabaseManager
{
    /// <summary>
    /// Initializes the database
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Migrates the database to the latest version
    /// </summary>
    Task MigrateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds the database with default data
    /// </summary>
    Task SeedDefaultDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets database health information
    /// </summary>
    Task<DatabaseHealth> GetHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Backs up the database
    /// </summary>
    Task<bool> BackupAsync(string? backupPath = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores the database from backup
    /// </summary>
    Task<bool> RestoreAsync(string backupPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes the database
    /// </summary>
    Task OptimizeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up old data
    /// </summary>
    Task<DatabaseCleanupResult> CleanupAsync(DatabaseCleanupOptions? options = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Database manager implementation
/// </summary>
public class DatabaseManager : IDatabaseManager
{
    private readonly IOpenBulletContext _context;
    private readonly DatabaseOptions _options;
    private readonly ILogger<DatabaseManager> _logger;

    public DatabaseManager(IOpenBulletContext context, DatabaseOptions options, ILogger<DatabaseManager> logger)
    {
        _context = context;
        _options = options;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing database with provider: {Provider}", _options.Provider);

        try
        {
            if (_options.AutoMigrate)
            {
                await MigrateAsync(cancellationToken);
            }

            await SeedDefaultDataAsync(cancellationToken);

            _logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize database");
            throw;
        }
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Migrating database to latest version");

        if (_context is SqliteContext sqliteContext)
        {
            // For new databases, use EnsureCreated instead of Migrate
            // Migrate requires existing migrations, EnsureCreated creates schema from current models
            if (_options.Provider == DatabaseProvider.SQLite)
            {
                // Check if database exists and has tables
                var databaseExists = await sqliteContext.Database.CanConnectAsync(cancellationToken);
                if (databaseExists)
                {
                    // Database exists, try to migrate if migrations exist
                    try
                    {
                        await sqliteContext.Database.MigrateAsync(cancellationToken);
                        _logger.LogInformation("Database migrated successfully");
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("pending changes"))
                    {
                        // No migrations exist, create schema from current models
                        _logger.LogInformation("No migrations found, creating database schema from current models");
                        await sqliteContext.Database.EnsureCreatedAsync(cancellationToken);
                        _logger.LogInformation("Database schema created successfully");
                    }
                }
                else
                {
                    // Database doesn't exist, create it with current schema
                    _logger.LogInformation("Database doesn't exist, creating new database with current schema");
                    await sqliteContext.Database.EnsureCreatedAsync(cancellationToken);
                    _logger.LogInformation("New database created successfully");
                }
            }
            else if (_options.Provider == DatabaseProvider.InMemory)
            {
                // InMemory databases don't require migrations
                await sqliteContext.Database.EnsureCreatedAsync(cancellationToken);
                _logger.LogInformation("InMemory database is ready");
            }
        }
        else if (_context is LiteDbContext liteDbContext)
        {
            // LiteDB doesn't require explicit migrations
            _logger.LogInformation("LiteDB database is ready");
        }

        _logger.LogInformation("Database migration completed");
    }

    public async Task SeedDefaultDataAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding default data");

        try
        {
            // Seed default settings
            await SeedDefaultSettingsAsync(cancellationToken);

            _logger.LogInformation("Default data seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed default data");
            throw;
        }
    }

    public async Task<DatabaseHealth> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var health = new DatabaseHealth();

        try
        {
            // Test basic connectivity
            var configCount = await _context.Configurations.CountAsync(cancellationToken: cancellationToken);
            var jobCount = await _context.Jobs.CountAsync(cancellationToken: cancellationToken);
            var resultCount = await _context.JobResults.CountAsync(cancellationToken: cancellationToken);
            var proxyCount = await _context.Proxies.CountAsync(cancellationToken: cancellationToken);
            var settingCount = await _context.Settings.CountAsync(cancellationToken: cancellationToken);

            stopwatch.Stop();

            health.IsHealthy = true;
            health.ResponseTime = stopwatch.Elapsed;
            health.TableCount = 5; // We have 5 main tables
            health.RecordCounts = new Dictionary<string, int>
            {
                ["Configurations"] = configCount,
                ["Jobs"] = jobCount,
                ["JobResults"] = resultCount,
                ["Proxies"] = proxyCount,
                ["Settings"] = settingCount
            };

            // Get database size and version info
            if (_context is SqliteContext)
            {
                health.Version = "SQLite";
                health.SizeBytes = GetSqliteDatabaseSize();
            }
            else if (_context is LiteDbContext)
            {
                health.Version = "LiteDB 5.0";
                health.SizeBytes = GetLiteDbDatabaseSize();
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            health.IsHealthy = false;
            health.ResponseTime = stopwatch.Elapsed;
            health.Issues.Add($"Health check failed: {ex.Message}");
            _logger.LogError(ex, "Database health check failed");
        }

        return health;
    }

    public async Task<bool> BackupAsync(string? backupPath = null, CancellationToken cancellationToken = default)
    {
        backupPath ??= GenerateBackupPath();

        try
        {
            return await _context.BackupAsync(backupPath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to backup database to {BackupPath}", backupPath);
            return false;
        }
    }

    public async Task<bool> RestoreAsync(string backupPath, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.RestoreAsync(backupPath, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore database from {BackupPath}", backupPath);
            return false;
        }
    }

    public async Task OptimizeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.OptimizeAsync(cancellationToken);
            _logger.LogInformation("Database optimization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to optimize database");
            throw;
        }
    }

    public async Task<DatabaseCleanupResult> CleanupAsync(DatabaseCleanupOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new DatabaseCleanupOptions();
        var result = new DatabaseCleanupResult();

        try
        {
            _logger.LogInformation("Starting database cleanup");

            // Clean up old jobs
            if (options.CleanupOldJobs && options.JobRetentionPeriod.HasValue)
            {
                var jobLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<JobStorage>.Instance;
                var jobStorage = new JobStorage(_context, jobLogger);
                result.DeletedJobs = await jobStorage.CleanupOldJobsAsync(options.JobRetentionPeriod.Value, options.KeepJobResults, cancellationToken);
            }

            // Clean up old proxies
            if (options.CleanupOldProxies)
            {
                var proxyLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ProxyStorage>.Instance;
                var proxyStorage = new ProxyStorage(_context, proxyLogger);
                result.DeletedProxies = await proxyStorage.CleanupAsync(options.ProxyRetentionPeriod, options.RemoveDeadProxies, cancellationToken);
            }

            // Optimize database after cleanup
            if (options.OptimizeAfterCleanup)
            {
                await OptimizeAsync(cancellationToken);
            }

            result.Success = true;
            _logger.LogInformation("Database cleanup completed: {DeletedJobs} jobs, {DeletedProxies} proxies removed", 
                result.DeletedJobs, result.DeletedProxies);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Database cleanup failed");
        }

        return result;
    }

    private async Task SeedDefaultSettingsAsync(CancellationToken cancellationToken)
    {
        var settingsStorage = new SettingsStorage(_context, _logger as ILogger<SettingsStorage>);

        var defaultSettings = new Dictionary<string, object>
        {
            ["General.MaxConcurrentJobs"] = 5,
            ["General.DefaultTimeout"] = 30,
            ["General.EnableLogging"] = true,
            ["General.LogLevel"] = "Information",
            ["General.AutoSaveInterval"] = 300, // 5 minutes
            ["Proxy.HealthCheckInterval"] = 300, // 5 minutes
            ["Proxy.AutoBanEnabled"] = true,
            ["Proxy.MaxFailuresBeforeBan"] = 3,
            ["Proxy.BanDuration"] = 600, // 10 minutes
            ["Export.DefaultFormat"] = "JSON",
            ["Export.IncludeMetadata"] = true,
            ["Backup.AutoBackupEnabled"] = true,
            ["Backup.BackupInterval"] = 86400, // 24 hours
            ["Backup.MaxBackupCount"] = 7
        };

        foreach (var setting in defaultSettings)
        {
            var existing = await settingsStorage.GetValueAsync<object>(setting.Key, cancellationToken: cancellationToken);
            if (existing == null)
            {
                await settingsStorage.SetValueAsync(setting.Key, setting.Value, 
                    $"Default {setting.Key} setting", GetSettingCategory(setting.Key), cancellationToken);
            }
        }
    }

    private static string GetSettingCategory(string key)
    {
        var parts = key.Split('.');
        return parts.Length > 1 ? parts[0] : "General";
    }

    private string GenerateBackupPath()
    {
        var backupDir = _options.BackupDirectory;
        if (!Directory.Exists(backupDir))
        {
            Directory.CreateDirectory(backupDir);
        }

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var extension = _options.Provider == DatabaseProvider.SQLite ? ".db" : ".bak";
        return Path.Combine(backupDir, $"openbullet_backup_{timestamp}{extension}");
    }

    private long GetSqliteDatabaseSize()
    {
        try
        {
            if (_context is SqliteContext sqliteContext)
            {
                var connectionString = sqliteContext.Database.GetConnectionString();
                var dbPath = ExtractDbPathFromConnectionString(connectionString);
                if (File.Exists(dbPath))
                {
                    return new FileInfo(dbPath).Length;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get SQLite database size");
        }

        return 0;
    }

    private long GetLiteDbDatabaseSize()
    {
        try
        {
            if (_context is LiteDbContext liteDbContext)
            {
                return liteDbContext.GetDatabaseSize();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get LiteDB database size");
        }

        return 0;
    }

    private static string ExtractDbPathFromConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "openbullet.db";

        var parts = connectionString.Split(';');
        var dataSourcePart = parts.FirstOrDefault(p => p.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));
        
        if (dataSourcePart != null)
        {
            return dataSourcePart.Split('=')[1].Trim();
        }

        return "openbullet.db";
    }
}

/// <summary>
/// Database cleanup options
/// </summary>
public class DatabaseCleanupOptions
{
    public bool CleanupOldJobs { get; set; } = true;
    public bool CleanupOldProxies { get; set; } = true;
    public bool RemoveDeadProxies { get; set; } = false;
    public bool KeepJobResults { get; set; } = false;
    public bool OptimizeAfterCleanup { get; set; } = true;
    public TimeSpan? JobRetentionPeriod { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan? ProxyRetentionPeriod { get; set; } = TimeSpan.FromDays(7);
}

/// <summary>
/// Database cleanup result
/// </summary>
public class DatabaseCleanupResult
{
    public bool Success { get; set; }
    public int DeletedJobs { get; set; }
    public int DeletedProxies { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// LiteDB context placeholder - would need LiteDB implementation
/// </summary>
public class LiteDbContext : IOpenBulletContext
{
    private readonly DatabaseOptions _options;
    private readonly ILogger<LiteDbContext> _logger;

    public LiteDbContext(DatabaseOptions options, ILogger<LiteDbContext> logger)
    {
        _options = options;
        _logger = logger;
        
        // TODO: Implement LiteDB repositories
        // Constructor should not throw, only repository access should throw
    }

    public IRepository<ConfigurationEntity> Configurations => throw new NotImplementedException();
    public IRepository<JobEntity> Jobs => throw new NotImplementedException();
    public IRepository<JobResultEntity> JobResults => throw new NotImplementedException();
    public IRepository<ProxyEntity> Proxies => throw new NotImplementedException();
    public IRepository<SettingEntity> Settings => throw new NotImplementedException();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<int> ExecuteSqlAsync(string sql, object[]? parameters = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> BackupAsync(string backupPath, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<bool> RestoreAsync(string backupPath, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task OptimizeAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public long GetDatabaseSize() => 0;
}
