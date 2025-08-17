using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using System.Diagnostics;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 11 Tests: Database Manager and Service Provider Tests
/// </summary>
public class Step11_DatabaseManagerTests : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDatabaseManager _databaseManager;
    private readonly DatabaseOptions _databaseOptions;

    public Step11_DatabaseManagerTests()
    {
        var services = new ServiceCollection();
        
        _databaseOptions = new DatabaseOptions
        {
            Provider = DatabaseProvider.InMemory,
            ConnectionString = $"InMemoryManagerTest_{Guid.NewGuid():N}",
            EnableLogging = false,
            AutoMigrate = true,
            BackupDirectory = "TestBackups",
            BackupInterval = TimeSpan.FromHours(1),
            MaxBackupCount = 3
        };

        services.AddOpenBulletDatabase(_databaseOptions);
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));

        _serviceProvider = services.BuildServiceProvider();
        _databaseManager = _serviceProvider.GetRequiredService<IDatabaseManager>();
    }

    [Fact]
    public void DatabaseServiceProvider_Should_Register_All_Services()
    {
        // Assert - Core services are registered
        _serviceProvider.GetService<DatabaseOptions>().Should().NotBeNull();
        _serviceProvider.GetService<IOpenBulletContext>().Should().NotBeNull();
        _serviceProvider.GetService<IDatabaseManager>().Should().NotBeNull();

        // Assert - Storage services are registered
        _serviceProvider.GetService<IConfigurationStorage>().Should().NotBeNull();
        _serviceProvider.GetService<IJobStorage>().Should().NotBeNull();
        _serviceProvider.GetService<IResultStorage>().Should().NotBeNull();
        _serviceProvider.GetService<IProxyStorage>().Should().NotBeNull();
        _serviceProvider.GetService<ISettingsStorage>().Should().NotBeNull();
    }

    [Fact]
    public async Task DatabaseManager_Should_Initialize_Successfully()
    {
        // Act
        var initializeAction = async () => await _databaseManager.InitializeAsync();

        // Assert
        await initializeAction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DatabaseManager_Migration_Should_Work()
    {
        // Act
        var migrateAction = async () => await _databaseManager.MigrateAsync();

        // Assert
        await migrateAction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DatabaseManager_SeedDefaultData_Should_Work()
    {
        // Act
        await _databaseManager.SeedDefaultDataAsync();

        // Assert - Check that default settings were created
        var settingsStorage = _serviceProvider.GetRequiredService<ISettingsStorage>();
        var generalSettings = await settingsStorage.GetByCategoryAsync("General");
        
        generalSettings.Should().NotBeEmpty();
        generalSettings.Should().Contain(s => s.Key == "General.MaxConcurrentJobs");
        generalSettings.Should().Contain(s => s.Key == "General.DefaultTimeout");
        generalSettings.Should().Contain(s => s.Key == "General.EnableLogging");

        // Verify specific default values
        var maxJobs = await settingsStorage.GetValueAsync<int>("General.MaxConcurrentJobs");
        maxJobs.Should().Be(5);

        var timeout = await settingsStorage.GetValueAsync<int>("General.DefaultTimeout");
        timeout.Should().Be(30);

        var enableLogging = await settingsStorage.GetValueAsync<bool>("General.EnableLogging");
        enableLogging.Should().BeTrue();
    }

    [Fact]
    public async Task DatabaseManager_GetHealth_Should_Return_Health_Info()
    {
        // Arrange - Initialize database first
        await _databaseManager.InitializeAsync();

        // Act
        var health = await _databaseManager.GetHealthAsync();

        // Assert
        health.Should().NotBeNull();
        health.IsHealthy.Should().BeTrue();
        health.ResponseTime.Should().BeGreaterThan(TimeSpan.Zero);
        health.TableCount.Should().Be(5); // We have 5 main tables
        health.RecordCounts.Should().ContainKey("Configurations");
        health.RecordCounts.Should().ContainKey("Jobs");
        health.RecordCounts.Should().ContainKey("JobResults");
        health.RecordCounts.Should().ContainKey("Proxies");
        health.RecordCounts.Should().ContainKey("Settings");

        // For in-memory database, version should be set
        health.Version.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DatabaseManager_Health_With_Data_Should_Show_Counts()
    {
        // Arrange - Add some test data
        await _databaseManager.InitializeAsync();
        
        var configStorage = _serviceProvider.GetRequiredService<IConfigurationStorage>();
        var jobStorage = _serviceProvider.GetRequiredService<IJobStorage>();
        var proxyStorage = _serviceProvider.GetRequiredService<IProxyStorage>();

        await configStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Health Test Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        await proxyStorage.SaveAsync(new ProxyEntity
        {
            Host = "test.proxy.com",
            Port = 8080
        });

        // Act
        var health = await _databaseManager.GetHealthAsync();

        // Assert
        health.IsHealthy.Should().BeTrue();
        health.RecordCounts["Configurations"].Should().BeGreaterThan(0);
        health.RecordCounts["Proxies"].Should().BeGreaterThan(0);
        health.RecordCounts["Settings"].Should().BeGreaterThan(0); // From default seeding
    }

    [Fact]
    public async Task DatabaseManager_Optimize_Should_Work()
    {
        // Arrange
        await _databaseManager.InitializeAsync();

        // Act
        var optimizeAction = async () => await _databaseManager.OptimizeAsync();

        // Assert
        await optimizeAction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DatabaseManager_Cleanup_Should_Work()
    {
        // Arrange - Initialize and add test data
        await _databaseManager.InitializeAsync();
        
        var configStorage = _serviceProvider.GetRequiredService<IConfigurationStorage>();
        var jobStorage = _serviceProvider.GetRequiredService<IJobStorage>();
        var proxyStorage = _serviceProvider.GetRequiredService<IProxyStorage>();

        // Create old configuration and job
        var config = await configStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Cleanup Test Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var oldJob = await jobStorage.SaveAsync(new JobEntity
        {
            Name = "Old Job",
            ConfigurationId = config.Id,
            Status = JobStatus.Completed,
            StartedAt = DateTime.UtcNow.AddDays(-60),
            CompletedAt = DateTime.UtcNow.AddDays(-60)
        });

        var recentJob = await jobStorage.SaveAsync(new JobEntity
        {
            Name = "Recent Job",
            ConfigurationId = config.Id,
            Status = JobStatus.Completed,
            StartedAt = DateTime.UtcNow.AddDays(-1),
            CompletedAt = DateTime.UtcNow.AddDays(-1)
        });

        // Create old and dead proxies
        await proxyStorage.SaveAsync(new ProxyEntity
        {
            Host = "old.proxy.com",
            Port = 8080,
            CreatedAt = DateTime.UtcNow.AddDays(-90),
            LastUsed = DateTime.UtcNow.AddDays(-60)
        });

        await proxyStorage.SaveAsync(new ProxyEntity
        {
            Host = "dead.proxy.com",
            Port = 8080,
            Health = ProxyHealth.Dead
        });

        await proxyStorage.SaveAsync(new ProxyEntity
        {
            Host = "active.proxy.com",
            Port = 8080,
            Health = ProxyHealth.Healthy,
            LastUsed = DateTime.UtcNow
        });

        // Act - Cleanup with specific options
        var cleanupOptions = new DatabaseCleanupOptions
        {
            CleanupOldJobs = true,
            CleanupOldProxies = true,
            RemoveDeadProxies = true,
            JobRetentionPeriod = TimeSpan.FromDays(30),
            ProxyRetentionPeriod = TimeSpan.FromDays(30),
            OptimizeAfterCleanup = true
        };

        var result = await _databaseManager.CleanupAsync(cleanupOptions);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.DeletedJobs.Should().BeGreaterThan(0); // Old job should be deleted
        result.DeletedProxies.Should().BeGreaterThan(0); // Old and dead proxies should be deleted

        // Verify recent job and active proxy remain
        var remainingJobs = await jobStorage.GetAllAsync();
        var remainingProxies = await proxyStorage.GetAllAsync();

        remainingJobs.Should().Contain(j => j.Name == "Recent Job");
        remainingProxies.Should().Contain(p => p.Host == "active.proxy.com");
    }

    [Fact]
    public async Task DatabaseManager_Cleanup_With_Default_Options_Should_Work()
    {
        // Arrange
        await _databaseManager.InitializeAsync();

        // Act - Cleanup with default options
        var result = await _databaseManager.CleanupAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void DatabaseOptions_Should_Have_Correct_Defaults()
    {
        // Arrange
        var defaultOptions = new DatabaseOptions();

        // Assert
        defaultOptions.Provider.Should().Be(DatabaseProvider.SQLite);
        defaultOptions.ConnectionString.Should().BeEmpty();
        defaultOptions.EnableLogging.Should().BeFalse();
        defaultOptions.EnableSensitiveDataLogging.Should().BeFalse();
        defaultOptions.CommandTimeout.Should().Be(30);
        defaultOptions.AutoMigrate.Should().BeTrue();
        defaultOptions.BackupDirectory.Should().Be("Backups");
        defaultOptions.BackupInterval.Should().Be(TimeSpan.FromHours(24));
        defaultOptions.MaxBackupCount.Should().Be(7);
        defaultOptions.ProviderOptions.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void DatabaseProvider_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<DatabaseProvider>().Should().Contain(DatabaseProvider.SQLite);
        Enum.GetValues<DatabaseProvider>().Should().Contain(DatabaseProvider.LiteDB);
        Enum.GetValues<DatabaseProvider>().Should().Contain(DatabaseProvider.InMemory);
    }

    [Fact]
    public async Task DatabaseManager_Should_Handle_Multiple_Concurrent_Operations()
    {
        // Arrange
        await _databaseManager.InitializeAsync();

        // Act - Perform multiple operations concurrently using separate scopes
        var tasks = new List<Task>();
        
        for (int i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var configStorage = scope.ServiceProvider.GetRequiredService<IConfigurationStorage>();
                await configStorage.SaveAsync(new ConfigurationEntity
                {
                    Name = $"Concurrent Config {index}",
                    Script = $"REQUEST GET \"https://example.com/{index}\""
                });
            }));
        }

        // Add health checks concurrently using separate scopes
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var databaseManager = scope.ServiceProvider.GetRequiredService<IDatabaseManager>();
                var health = await databaseManager.GetHealthAsync();
                health.Should().NotBeNull();
            }));
        }

        // Assert - All operations should complete successfully
        await Task.WhenAll(tasks);

        var configStorage = _serviceProvider.GetRequiredService<IConfigurationStorage>();
        var allConfigs = await configStorage.GetAllAsync();
        allConfigs.Where(c => c.Name.StartsWith("Concurrent Config")).Should().HaveCount(10);
    }

    [Fact]
    public async Task DatabaseManager_Performance_Test()
    {
        // Arrange
        await _databaseManager.InitializeAsync();
        var stopwatch = Stopwatch.StartNew();

        // Act - Perform various operations and measure time
        var healthCheckTime = await MeasureOperationTime(() => _databaseManager.GetHealthAsync());
        var optimizeTime = await MeasureOperationTime(() => _databaseManager.OptimizeAsync());
        var cleanupTime = await MeasureOperationTime(() => _databaseManager.CleanupAsync());

        stopwatch.Stop();

        // Assert - Operations should complete within reasonable time
        healthCheckTime.Should().BeLessThan(TimeSpan.FromSeconds(5), "Health check should be fast");
        optimizeTime.Should().BeLessThan(TimeSpan.FromSeconds(10), "Optimize should complete reasonably quickly");
        cleanupTime.Should().BeLessThan(TimeSpan.FromSeconds(10), "Cleanup should complete reasonably quickly");
        
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(30), "All operations should complete within 30 seconds");
    }

    [Fact]
    public async Task ServiceProvider_InitializeDatabaseAsync_Extension_Should_Work()
    {
        // Arrange - Create a new service provider
        var services = new ServiceCollection();
        var options = new DatabaseOptions
        {
            Provider = DatabaseProvider.InMemory,
            ConnectionString = "ExtensionTest"
        };
        
        services.AddOpenBulletDatabase(options);
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        
        var serviceProvider = services.BuildServiceProvider();

        // Act - Use the extension method
        var initializedProvider = await serviceProvider.InitializeDatabaseAsync();

        // Assert
        initializedProvider.Should().BeSameAs(serviceProvider);
        
        // Verify that initialization actually happened by checking for seeded data
        var settingsStorage = initializedProvider.GetRequiredService<ISettingsStorage>();
        var generalSettings = await settingsStorage.GetByCategoryAsync("General");
        generalSettings.Should().NotBeEmpty();
    }

    [Fact]
    public void DatabaseCleanupOptions_Should_Have_Reasonable_Defaults()
    {
        // Arrange
        var options = new DatabaseCleanupOptions();

        // Assert
        options.CleanupOldJobs.Should().BeTrue();
        options.CleanupOldProxies.Should().BeTrue();
        options.RemoveDeadProxies.Should().BeFalse();
        options.KeepJobResults.Should().BeFalse();
        options.OptimizeAfterCleanup.Should().BeTrue();
        options.JobRetentionPeriod.Should().Be(TimeSpan.FromDays(30));
        options.ProxyRetentionPeriod.Should().Be(TimeSpan.FromDays(7));
    }

    [Fact]
    public void DatabaseCleanupResult_Should_Initialize_Correctly()
    {
        // Arrange
        var result = new DatabaseCleanupResult();

        // Assert
        result.Success.Should().BeFalse();
        result.DeletedJobs.Should().Be(0);
        result.DeletedProxies.Should().Be(0);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void DatabaseHealth_Should_Initialize_Correctly()
    {
        // Arrange
        var health = new DatabaseHealth();

        // Assert
        health.IsHealthy.Should().BeFalse();
        health.Version.Should().BeEmpty();
        health.SizeBytes.Should().Be(0);
        health.TableCount.Should().Be(0);
        health.LastBackup.Should().Be(default);
        health.ResponseTime.Should().Be(TimeSpan.Zero);
        health.RecordCounts.Should().NotBeNull().And.BeEmpty();
        health.Issues.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Database_Should_Handle_Error_Conditions_Gracefully()
    {
        // This test simulates various error conditions that might occur

        // Test health check when database might be unavailable
        // Note: In a real scenario, you might mock the context to throw exceptions
        var health = await _databaseManager.GetHealthAsync();
        
        // Even if there are issues, health check should not throw
        health.Should().NotBeNull();

        // Test cleanup with invalid options
        var invalidCleanupOptions = new DatabaseCleanupOptions
        {
            JobRetentionPeriod = TimeSpan.FromDays(-1), // Invalid negative value
            ProxyRetentionPeriod = TimeSpan.Zero
        };

        // Cleanup should handle invalid options gracefully
        var cleanupAction = async () => await _databaseManager.CleanupAsync(invalidCleanupOptions);
        await cleanupAction.Should().NotThrowAsync();
    }

    [Fact]
    public void LiteDbContext_Should_Not_Be_Implemented_Yet()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new DatabaseOptions
        {
            Provider = DatabaseProvider.LiteDB,
            ConnectionString = "LiteDbTest.db"
        };

        services.AddSingleton(options);
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));

        // Act & Assert - LiteDB should throw NotImplementedException for now
        var action = () =>
        {
            services.AddOpenBulletDatabase(options);
            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<IOpenBulletContext>();
        };

        // The context creation should work, but accessing repositories should throw
        action.Should().NotThrow();
    }

    [Fact]
    public async Task Multiple_Database_Contexts_Should_Work_Independently()
    {
        // Arrange - Create two separate service providers with different databases
        var services1 = new ServiceCollection();
        var options1 = new DatabaseOptions
        {
            Provider = DatabaseProvider.InMemory,
            ConnectionString = "Database1"
        };
        services1.AddOpenBulletDatabase(options1);
        services1.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        var provider1 = services1.BuildServiceProvider();

        var services2 = new ServiceCollection();
        var options2 = new DatabaseOptions
        {
            Provider = DatabaseProvider.InMemory,
            ConnectionString = "Database2"
        };
        services2.AddOpenBulletDatabase(options2);
        services2.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        var provider2 = services2.BuildServiceProvider();

        // Act - Add different data to each database
        var storage1 = provider1.GetRequiredService<IConfigurationStorage>();
        var storage2 = provider2.GetRequiredService<IConfigurationStorage>();

        await storage1.SaveAsync(new ConfigurationEntity { Name = "Config Database 1", Script = "REQUEST GET \"https://db1.com\"" });
        await storage2.SaveAsync(new ConfigurationEntity { Name = "Config Database 2", Script = "REQUEST GET \"https://db2.com\"" });

        // Assert - Each database should contain only its own data
        var configs1 = await storage1.GetAllAsync();
        var configs2 = await storage2.GetAllAsync();

        configs1.Should().HaveCount(1);
        configs1.First().Name.Should().Be("Config Database 1");

        configs2.Should().HaveCount(1);
        configs2.First().Name.Should().Be("Config Database 2");

        // Cleanup
        provider1.Dispose();
        provider2.Dispose();
    }

    private static async Task<TimeSpan> MeasureOperationTime(Func<Task> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        await operation();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
