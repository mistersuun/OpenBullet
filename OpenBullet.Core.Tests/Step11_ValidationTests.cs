using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Data;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Jobs;
using OpenBullet.Core.Models;
using System.Diagnostics;
using Xunit;
using JobStatus = OpenBullet.Core.Data.JobStatus;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 11 Validation Tests - Integration and validation testing
/// </summary>
public class Step11_ValidationTests : IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    public Step11_ValidationTests()
    {
        var services = new ServiceCollection();
        
        var options = new DatabaseOptions
        {
            Provider = DatabaseProvider.InMemory,
            ConnectionString = $"ValidationTest_{Guid.NewGuid():N}",
            EnableLogging = false,
            AutoMigrate = true
        };

        services.AddOpenBulletDatabase(options);
        services.AddLogging(builder => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));

        _serviceProvider = services.BuildServiceProvider();
        
        // Debug: Check what services are available
        var resultStorage = _serviceProvider.GetService<IResultStorage>();
        if (resultStorage == null)
        {
            Console.WriteLine("WARNING: IResultStorage service not found in service provider!");
            var availableServices = services.Select(s => s.ServiceType.Name).ToList();
            Console.WriteLine($"Available services: {string.Join(", ", availableServices)}");
        }
        else
        {
            Console.WriteLine("IResultStorage service found successfully");
        }
    }

    [Fact]
    public void All_Interfaces_Should_Be_Properly_Defined()
    {
        // Assert - Core interfaces exist
        typeof(IRepository<>).IsInterface.Should().BeTrue();
        typeof(IOpenBulletContext).IsInterface.Should().BeTrue();
        typeof(IDatabaseManager).IsInterface.Should().BeTrue();

        // Assert - Storage interfaces exist
        typeof(IConfigurationStorage).IsInterface.Should().BeTrue();
        typeof(IJobStorage).IsInterface.Should().BeTrue();
        typeof(IResultStorage).IsInterface.Should().BeTrue();
        typeof(IProxyStorage).IsInterface.Should().BeTrue();
        typeof(ISettingsStorage).IsInterface.Should().BeTrue();

        // Assert - Core interfaces have required methods
        var repositoryMethods = typeof(IRepository<>).GetMethods();
        repositoryMethods.Should().Contain(m => m.Name == "GetByIdAsync");
        repositoryMethods.Should().Contain(m => m.Name == "GetAllAsync");
        repositoryMethods.Should().Contain(m => m.Name == "AddAsync");
        repositoryMethods.Should().Contain(m => m.Name == "UpdateAsync");
        repositoryMethods.Should().Contain(m => m.Name == "DeleteAsync");
        repositoryMethods.Should().Contain(m => m.Name == "FindAsync");
        repositoryMethods.Should().Contain(m => m.Name == "CountAsync");
        repositoryMethods.Should().Contain(m => m.Name == "ExistsAsync");
        repositoryMethods.Should().Contain(m => m.Name == "GetPagedAsync");

        var contextMethods = typeof(IOpenBulletContext).GetMethods();
        contextMethods.Should().Contain(m => m.Name == "SaveChangesAsync");
        contextMethods.Should().Contain(m => m.Name == "BeginTransactionAsync");
        contextMethods.Should().Contain(m => m.Name == "BackupAsync");
        contextMethods.Should().Contain(m => m.Name == "RestoreAsync");
        contextMethods.Should().Contain(m => m.Name == "OptimizeAsync");
    }

    [Fact]
    public void All_Entity_Classes_Should_Be_Properly_Defined()
    {
        // Assert - Base entity
        typeof(BaseEntity).Should().BeAbstract();

        // Assert - Entity classes inherit from BaseEntity
        typeof(ConfigurationEntity).Should().BeDerivedFrom<BaseEntity>();
        typeof(JobEntity).Should().BeDerivedFrom<BaseEntity>();
        typeof(JobResultEntity).Should().BeDerivedFrom<BaseEntity>();
        typeof(ProxyEntity).Should().BeDerivedFrom<BaseEntity>();
        typeof(SettingEntity).Should().BeDerivedFrom<BaseEntity>();

        // Assert - Entities have required properties
        var configProps = typeof(ConfigurationEntity).GetProperties();
        configProps.Should().Contain(p => p.Name == "Name");
        configProps.Should().Contain(p => p.Name == "Script");
        configProps.Should().Contain(p => p.Name == "Category");
        configProps.Should().Contain(p => p.Name == "Author");
        configProps.Should().Contain(p => p.Name == "Version");

        var jobProps = typeof(JobEntity).GetProperties();
        jobProps.Should().Contain(p => p.Name == "Name");
        jobProps.Should().Contain(p => p.Name == "ConfigurationId");
        jobProps.Should().Contain(p => p.Name == "Status");
        jobProps.Should().Contain(p => p.Name == "TotalItems");
        jobProps.Should().Contain(p => p.Name == "ProcessedItems");

        var proxyProps = typeof(ProxyEntity).GetProperties();
        proxyProps.Should().Contain(p => p.Name == "Host");
        proxyProps.Should().Contain(p => p.Name == "Port");
        proxyProps.Should().Contain(p => p.Name == "Type");
        proxyProps.Should().Contain(p => p.Name == "Health");
    }

    [Fact]
    public void All_Enums_Should_Have_Correct_Values()
    {
        // Assert - DatabaseProvider enum
        Enum.GetValues<DatabaseProvider>().Should().HaveCount(3);
        Enum.GetValues<DatabaseProvider>().Should().Contain(DatabaseProvider.SQLite);
        Enum.GetValues<DatabaseProvider>().Should().Contain(DatabaseProvider.LiteDB);
        Enum.GetValues<DatabaseProvider>().Should().Contain(DatabaseProvider.InMemory);

        // Assert - JobStatus enum
        Enum.GetValues<JobStatus>().Should().Contain(JobStatus.Created);
        Enum.GetValues<JobStatus>().Should().Contain(JobStatus.Running);
        Enum.GetValues<JobStatus>().Should().Contain(JobStatus.Completed);
        Enum.GetValues<JobStatus>().Should().Contain(JobStatus.Failed);

        // Assert - ProxyHealth enum
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Unknown);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Healthy);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Slow);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Unreliable);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Dead);

        // Assert - ExportFormat enum
        Enum.GetValues<ExportFormat>().Should().Contain(ExportFormat.JSON);
        Enum.GetValues<ExportFormat>().Should().Contain(ExportFormat.CSV);
        Enum.GetValues<ExportFormat>().Should().Contain(ExportFormat.XML);
    }

    [Fact]
    public void Filter_Classes_Should_Initialize_Correctly()
    {
        // Assert - JobFilter
        var jobFilter = new JobFilter();
        jobFilter.Status.Should().BeNull();
        jobFilter.ConfigurationId.Should().BeNull();
        jobFilter.SearchTerm.Should().BeNull();

        // Assert - ResultFilter
        var resultFilter = new ResultFilter();
        resultFilter.Status.Should().BeNull();
        resultFilter.Success.Should().BeNull();
        resultFilter.CapturedDataFilters.Should().NotBeNull().And.BeEmpty();

        // Assert - ProxyFilter
        var proxyFilter = new ProxyFilter();
        proxyFilter.Type.Should().BeNull();
        proxyFilter.Health.Should().BeNull();
        proxyFilter.IsActive.Should().BeNull();
    }

    [Fact]
    public void Statistics_Classes_Should_Initialize_Correctly()
    {
        // Assert - ConfigurationUsageStats
        var configStats = new ConfigurationUsageStats();
        configStats.TotalJobs.Should().Be(0);
        configStats.SuccessfulJobs.Should().Be(0);

        // Assert - JobStatistics
        var jobStats = new JobStatistics();
        jobStats.JobId.Should().BeEmpty();
        jobStats.StatusCounts.Should().NotBeNull().And.BeEmpty();
        jobStats.ProxyUsage.Should().NotBeNull().And.BeEmpty();

        // Assert - ProxyStatistics
        var proxyStats = new ProxyStatistics();
        proxyStats.TotalProxies.Should().Be(0);
        proxyStats.OverallSuccessRate.Should().Be(0);
        proxyStats.HealthDistribution.Should().NotBeNull().And.BeEmpty();
        proxyStats.TypeDistribution.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ImportResult_Should_Initialize_Correctly()
    {
        // Arrange & Act
        var result = new ImportResult();

        // Assert
        result.Success.Should().BeFalse();
        result.ImportedCount.Should().Be(0);
        result.SkippedCount.Should().Be(0);
        result.ErrorCount.Should().Be(0);
        result.Errors.Should().NotBeNull().And.BeEmpty();
        result.Warnings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PagedResult_Should_Calculate_Properties_Correctly()
    {
        // Arrange
        var pagedResult = new PagedResult<string>
        {
            Items = new[] { "Item1", "Item2", "Item3", "Item4", "Item5" },
            TotalCount = 23,
            PageNumber = 2,
            PageSize = 5
        };

        // Assert
        pagedResult.TotalPages.Should().Be(5); // Ceiling(23 / 5) = 5
        pagedResult.HasPreviousPage.Should().BeTrue(); // Page 2 has previous
        pagedResult.HasNextPage.Should().BeTrue(); // Page 2 has next (not last page)

        // Test edge cases
        var firstPage = new PagedResult<string>
        {
            TotalCount = 10,
            PageNumber = 1,
            PageSize = 5
        };
        firstPage.HasPreviousPage.Should().BeFalse();
        firstPage.HasNextPage.Should().BeTrue();

        var lastPage = new PagedResult<string>
        {
            TotalCount = 10,
            PageNumber = 2,
            PageSize = 5
        };
        lastPage.HasPreviousPage.Should().BeTrue();
        lastPage.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Complete_Workflow_Integration_Test()
    {
        // This test validates that all components work together in a realistic scenario
        
        // Arrange - Initialize database
        var dbManager = _serviceProvider.GetRequiredService<IDatabaseManager>();
        await dbManager.InitializeAsync();

        var configStorage = _serviceProvider.GetRequiredService<IConfigurationStorage>();
        var jobStorage = _serviceProvider.GetRequiredService<IJobStorage>();
        var resultStorage = _serviceProvider.GetRequiredService<IResultStorage>();
        var proxyStorage = _serviceProvider.GetRequiredService<IProxyStorage>();
        var settingsStorage = _serviceProvider.GetRequiredService<ISettingsStorage>();

        // Step 1: Create and save a configuration
        var configModel = new ConfigModel
        {
            Name = "Integration Test Config",
            Script = "REQUEST GET \"https://api.example.com/users/<USER>\"",
            Author = "Integration Test",
            Version = "1.0.0"
        };

        var config = await configStorage.CreateFromModelAsync(configModel);
        config.Should().NotBeNull();

        // Step 2: Import some proxies
        var proxyStrings = new[]
        {
            "proxy1.example.com:8080",
            "socks5://user:pass@proxy2.example.com:1080",
            "proxy3.example.com:3128"
        };

        var importResult = await proxyStorage.ImportFromStringsAsync(proxyStrings, "Integration Test");
        importResult.Success.Should().BeTrue();
        importResult.ImportedCount.Should().Be(3);

        // Step 3: Create a job from the configuration
        var jobConfig = new JobConfiguration
        {
            Name = "Integration Test Job",
            Config = new ConfigModel 
            { 
                Id = config.Id, // Use the same ID as the stored configuration
                Script = configModel.Script,
                Name = configModel.Name,
                Author = configModel.Author,
                Version = configModel.Version
            },
            DataLines = new List<string> { "user1", "user2", "user3", "user4", "user5" },
            ConcurrentBots = 2,
            CustomSettings = { ["TestMode"] = true }
        };

        var job = await jobStorage.CreateFromConfigurationAsync(config.Id, jobConfig);
        job.Should().NotBeNull();
        job.TotalItems.Should().Be(5);

        // Step 4: Use actual JobManager to execute the job and update usage count
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<JobManager>();
        
        // Create a mock bot runner that returns our predefined results
        var botRunnerMock = new Mock<IBotRunner>();
        botRunnerMock.Setup(br => br.RunAsync(It.IsAny<ConfigModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Returns<ConfigModel, string, CancellationToken>((config, dataLine, token) =>
                    {
                        var result = dataLine switch
                        {
                            "user1" => new BotResult { DataLine = "user1", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(1), 
                                        CapturedData = { ["Email"] = "user1@example.com", ["ID"] = "1001" },
                                        Metadata = { ["LastResponseCode"] = 200, ["ProxyUsed"] = "proxy1.example.com:8080" } },
                            "user2" => new BotResult { DataLine = "user2", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(1.5), 
                                        CapturedData = { ["Email"] = "user2@example.com", ["ID"] = "1002" },
                                        Metadata = { ["LastResponseCode"] = 200, ["ProxyUsed"] = "proxy2.example.com:1080" } },
                            "user3" => new BotResult { DataLine = "user3", Status = BotStatus.Failure, Success = false, ExecutionTime = TimeSpan.FromSeconds(0.5), 
                                        ErrorMessage = "User not found", Metadata = { ["LastResponseCode"] = 404 } },
                            "user4" => new BotResult { DataLine = "user4", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(2), 
                                        CapturedData = { ["Email"] = "user4@example.com", ["ID"] = "1004" },
                                        Metadata = { ["LastResponseCode"] = 200, ["ProxyUsed"] = "proxy3.example.com:3128" } },
                            "user5" => new BotResult { DataLine = "user5", Status = BotStatus.Ban, Success = false, ExecutionTime = TimeSpan.FromSeconds(0.2), 
                                        ErrorMessage = "Rate limited", Metadata = { ["LastResponseCode"] = 429 } },
                            _ => new BotResult { DataLine = dataLine, Status = BotStatus.Error, Success = false, ErrorMessage = "Unknown data line" }
                        };
                        
                        return Task.FromResult(result);
                    });

        // Create JobManager with the mock bot runner
        var jobManager = new JobManager(logger, botRunnerMock.Object, _serviceProvider);
        
        // Start the job - this will execute bots and update usage count
        var actualJobId = await jobManager.StartJobAsync(jobConfig);
        
        // Wait for job to complete
        await Task.Delay(2000); // Give time for execution
        
                // Get the actual results from JobManager
        var actualResults = await jobManager.GetJobResultsAsync(actualJobId);
        
        // Step 5: Update job progress and complete
        await jobStorage.UpdateProgressAsync(job.Id, 5, 3, 2);
        await jobStorage.CompleteJobAsync(job.Id, JobStatus.Completed, TimeSpan.FromMinutes(5));
        
        // Step 6: Update proxy statistics based on actual results
        var availableProxies = await proxyStorage.GetAvailableAsync();
        foreach (var proxy in availableProxies)
        {
            var usageCount = actualResults.Count(r => 
                r.Metadata.ContainsKey("ProxyUsed") && 
                r.Metadata["ProxyUsed"]?.ToString()?.Contains(proxy.Host) == true);
            
            if (usageCount > 0)
            {
                var successCount = actualResults.Count(r => 
                    r.Success && 
                    r.Metadata.ContainsKey("ProxyUsed") && 
                    r.Metadata["ProxyUsed"]?.ToString()?.Contains(proxy.Host) == true);

                // Simulate multiple uses
                for (int i = 0; i < usageCount; i++)
                {
                    await proxyStorage.UpdateStatisticsAsync(proxy.Id, 
                        i < successCount, 
                        TimeSpan.FromMilliseconds(500 + i * 100));
                }
            }
        }

        // Step 7: Configure some settings
        await settingsStorage.SetValueAsync("IntegrationTest.MaxRetries", 3, "Max retry count", "Testing");
        await settingsStorage.SetValueAsync("IntegrationTest.EnableLogging", true, "Enable logging", "Testing");

        // Validation: Verify the complete workflow results
        
        // Verify configuration was saved correctly
        var savedConfig = await configStorage.GetByIdAsync(config.Id);
        savedConfig.Should().NotBeNull();
        savedConfig!.UsageCount.Should().BeGreaterThan(0);

        // Verify job was completed correctly using JobManager
        var jobStatus = jobManager.GetJobStatus(actualJobId);
        jobStatus.Should().NotBeNull();
        jobStatus.State.Should().Be(JobState.Completed);
        jobStatus.ProcessedDataLines.Should().Be(5);
        jobStatus.SuccessfulBots.Should().Be(3);

        // Verify results were saved correctly
        var allResults = await resultStorage.GetByJobIdAsync(actualJobId);
        allResults.Should().HaveCount(5);
        
        var successfulResults = await resultStorage.GetSuccessfulAsync(actualJobId);
        successfulResults.Should().HaveCount(3);

        var failedResults = await resultStorage.GetFailedAsync(actualJobId);
        failedResults.Should().HaveCount(2);

        // Verify captured data
        var capturedKeys = await resultStorage.GetCapturedDataKeysAsync(actualJobId);
        capturedKeys.Should().Contain("Email", "ID");

        var emailValues = await resultStorage.GetCapturedDataValuesAsync(actualJobId, "Email");
        emailValues.Should().HaveCount(3); // Only successful results have email

        // Verify proxy statistics were updated
        var proxyStats = await proxyStorage.GetStatisticsAsync();
        proxyStats.TotalProxies.Should().Be(3);
        proxyStats.OverallSuccessRate.Should().BeGreaterThan(0);

        // Verify settings were saved
        var maxRetries = await settingsStorage.GetValueAsync<int>("IntegrationTest.MaxRetries");
        maxRetries.Should().Be(3);

        var enableLogging = await settingsStorage.GetValueAsync<bool>("IntegrationTest.EnableLogging");
        enableLogging.Should().BeTrue();

        // Verify job statistics
        var jobManagerStats = jobManager.GetStatistics();
        jobManagerStats.TotalJobsCreated.Should().BeGreaterThan(0);
        jobManagerStats.TotalBotsExecuted.Should().BeGreaterThan(0);


        // Verify overall statistics
        var overallStats = await jobStorage.GetOverallStatisticsAsync();
        overallStats.TotalJobs.Should().BeGreaterThan(0);
        overallStats.CompletedJobs.Should().BeGreaterThan(0);

        // Verify database health
        var health = await dbManager.GetHealthAsync();
        health.IsHealthy.Should().BeTrue();
        health.RecordCounts["Configurations"].Should().BeGreaterThan(0);
        health.RecordCounts["Jobs"].Should().BeGreaterThan(0);
        health.RecordCounts["JobResults"].Should().BeGreaterThan(0);
        health.RecordCounts["Proxies"].Should().BeGreaterThan(0);
        health.RecordCounts["Settings"].Should().BeGreaterThan(0);

        // This validates that the entire database system works end-to-end
        // with all components properly integrated
    }

    [Fact]
    public async Task Error_Handling_And_Edge_Cases_Should_Work()
    {
        // Initialize database
        var dbManager = _serviceProvider.GetRequiredService<IDatabaseManager>();
        await dbManager.InitializeAsync();

        var configStorage = _serviceProvider.GetRequiredService<IConfigurationStorage>();
        var jobStorage = _serviceProvider.GetRequiredService<IJobStorage>();
        var resultStorage = _serviceProvider.GetRequiredService<IResultStorage>();
        var proxyStorage = _serviceProvider.GetRequiredService<IProxyStorage>();
        var settingsStorage = _serviceProvider.GetRequiredService<ISettingsStorage>();

        // Test 1: Handle non-existent IDs gracefully
        var nonExistentConfig = await configStorage.GetByIdAsync("non-existent-id");
        nonExistentConfig.Should().BeNull();

        var nonExistentJob = await jobStorage.GetByIdAsync("non-existent-id");
        nonExistentJob.Should().BeNull();

        // Test 2: Handle empty collections
        var emptyResults = await resultStorage.GetByJobIdAsync("non-existent-job");
        emptyResults.Should().BeEmpty();

        var emptyProxyImport = await proxyStorage.ImportFromStringsAsync(new string[0]);
        emptyProxyImport.ImportedCount.Should().Be(0);
        emptyProxyImport.Success.Should().BeTrue();

        // Test 3: Handle invalid data gracefully
        var invalidProxyStrings = new[] { "invalid", "", null, "not:a:valid:proxy:format:at:all" };
        var invalidImportResult = await proxyStorage.ImportFromStringsAsync(invalidProxyStrings.Where(s => s != null)!, "Error Test");
        invalidImportResult.ErrorCount.Should().BeGreaterThan(0);
        invalidImportResult.ImportedCount.Should().Be(0);

        // Test 4: Handle null/empty searches
        var emptySearch = await configStorage.SearchAsync("");
        emptySearch.Should().BeEmpty();

        var nullSearch = await configStorage.SearchAsync(null!);
        nullSearch.Should().BeEmpty();

        // Test 5: Handle edge case pagination
        var emptyPage = await configStorage.GetPagedAsync(1, 10);
        emptyPage.Items.Should().BeEmpty();
        emptyPage.TotalCount.Should().Be(0);
        emptyPage.TotalPages.Should().Be(0);
        emptyPage.HasPreviousPage.Should().BeFalse();
        emptyPage.HasNextPage.Should().BeFalse();

        // Test 6: Handle deletion of non-existent items
        var deleteNonExistent = await configStorage.DeleteAsync("non-existent");
        deleteNonExistent.Should().BeFalse();

        // Test 7: Handle setting operations with edge cases
        await settingsStorage.SetValueAsync("EdgeCase.NullValue", (string?)null);
        var nullValue = await settingsStorage.GetValueAsync<string>("EdgeCase.NullValue");
        nullValue.Should().BeNull();

        await settingsStorage.SetValueAsync("EdgeCase.EmptyString", "");
        var emptyValue = await settingsStorage.GetValueAsync<string>("EdgeCase.EmptyString");
        emptyValue.Should().Be("");

        // Test 8: Handle statistics for empty datasets
        var emptyJobStats = await jobStorage.GetStatisticsAsync("non-existent-job");
        emptyJobStats.JobId.Should().Be("non-existent-job");
        emptyJobStats.StatusCounts.Should().BeEmpty();

        var emptyResultStats = await resultStorage.GetStatisticsAsync("non-existent-job");
        emptyResultStats.TotalResults.Should().Be(0);
    }

    [Fact]
    public async Task Performance_With_Realistic_Dataset_Should_Be_Acceptable()
    {
        // This test validates performance with a more realistic dataset size
        var stopwatch = Stopwatch.StartNew();

        // Initialize database
        var dbManager = _serviceProvider.GetRequiredService<IDatabaseManager>();
        await dbManager.InitializeAsync();

        var configStorage = _serviceProvider.GetRequiredService<IConfigurationStorage>();
        var proxyStorage = _serviceProvider.GetRequiredService<IProxyStorage>();
        var settingsStorage = _serviceProvider.GetRequiredService<ISettingsStorage>();

        // Create realistic number of configurations (50)
        var configs = new List<ConfigurationEntity>();
        for (int i = 1; i <= 50; i++)
        {
            configs.Add(new ConfigurationEntity
            {
                Name = $"Performance Config {i:D3}",
                Description = $"Performance test configuration number {i}",
                Script = $"REQUEST GET \"https://api.example.com/endpoint/{i}\"",
                Category = $"Category{i % 5 + 1}",
                Author = $"Author{i % 3 + 1}",
                Version = $"1.{i % 10}.0",
                UsageCount = i * (i % 7),
                IsActive = i % 4 != 0
            });
        }

        // Batch insert configurations
        var configInsertTime = await MeasureTime(async () =>
        {
            foreach (var config in configs.Take(25))
            {
                await configStorage.SaveAsync(config);
            }
            // Simulate some being added later
            foreach (var config in configs.Skip(25))
            {
                await configStorage.SaveAsync(config);
            }
        });

        // Create realistic number of proxies (200)
        var proxyStrings = new List<string>();
        for (int i = 1; i <= 200; i++)
        {
            var proxyType = i % 3 == 0 ? "socks5" : "http";
            var hasAuth = i % 5 == 0;
            var auth = hasAuth ? $"user{i}:pass{i}@" : "";
            proxyStrings.Add($"{proxyType}://{auth}proxy{i:D3}.example.com:{8000 + i}");
        }

        var proxyInsertTime = await MeasureTime(async () =>
        {
            await proxyStorage.ImportFromStringsAsync(proxyStrings, "Performance Test");
        });

        // Create realistic number of settings (30)
        var settingInsertTime = await MeasureTime(async () =>
        {
            var categories = new[] { "General", "Security", "UI", "Network", "Advanced" };
            for (int i = 1; i <= 30; i++)
            {
                var category = categories[i % categories.Length];
                await settingsStorage.SetValueAsync($"{category}.Setting{i}", 
                    $"Value{i}", $"Description for setting {i}", category);
            }
        });

        // Test query performance
        var queryTime = await MeasureTime(async () =>
        {
            // Test various query patterns
            var allConfigs = await configStorage.GetAllAsync();
            var pagedConfigs = await configStorage.GetPagedAsync(1, 20);
            var searchResults = await configStorage.SearchAsync("Config");
            var categoryResults = await configStorage.GetByCategoryAsync("Category1");
            
            var availableProxies = await proxyStorage.GetAvailableAsync();
            var proxyStats = await proxyStorage.GetStatisticsAsync();
            
            var generalSettings = await settingsStorage.GetByCategoryAsync("General");
            var specificSetting = await settingsStorage.GetValueAsync<string>("General.Setting1");
            
            // Verify results are reasonable
            allConfigs.Should().HaveCount(50);
            pagedConfigs.Items.Should().HaveCount(20);
            pagedConfigs.TotalCount.Should().Be(50);
            searchResults.Should().HaveCount(50); // All contain "Config"
            availableProxies.Should().HaveCount(200);
            generalSettings.Should().HaveCountGreaterOrEqualTo(1);
        });

        stopwatch.Stop();

        // Assert performance is acceptable
        configInsertTime.Should().BeLessThan(TimeSpan.FromSeconds(10), "Configuration insertion should be fast");
        proxyInsertTime.Should().BeLessThan(TimeSpan.FromSeconds(5), "Proxy batch import should be fast");
        settingInsertTime.Should().BeLessThan(TimeSpan.FromSeconds(5), "Settings insertion should be fast");
        queryTime.Should().BeLessThan(TimeSpan.FromSeconds(3), "Queries should be fast");
        
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(30), "Entire performance test should complete within 30 seconds");

        // Verify database health after bulk operations
        var health = await dbManager.GetHealthAsync();
        health.IsHealthy.Should().BeTrue();
        health.ResponseTime.Should().BeLessThan(TimeSpan.FromSeconds(1), "Health check should be fast even after bulk operations");
    }

    [Fact]
    public void ExportHelper_Static_Methods_Should_Be_Available()
    {
        // These methods are used internally by storage services
        // We're just verifying they exist and are accessible
        
        var helperType = typeof(ExportHelper);
        helperType.Should().NotBeNull();
        
        var methods = helperType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        methods.Should().Contain(m => m.Name == "ExportConfigurationsAsync");
        methods.Should().Contain(m => m.Name == "ImportConfigurationsAsync");
    }

    [Fact]
    public void Database_Components_Should_Have_Proper_Inheritance()
    {
        // Verify inheritance hierarchy
        typeof(SqliteContext).Should().BeAssignableTo<IOpenBulletContext>();
        typeof(SqliteRepository<>).Should().BeAssignableTo(typeof(IRepository<>));
        typeof(DatabaseManager).Should().BeAssignableTo<IDatabaseManager>();
        
        typeof(ConfigurationStorage).Should().BeAssignableTo<IConfigurationStorage>();
        typeof(JobStorage).Should().BeAssignableTo<IJobStorage>();
        typeof(ResultStorage).Should().BeAssignableTo<IResultStorage>();
        typeof(ProxyStorage).Should().BeAssignableTo<IProxyStorage>();
        typeof(SettingsStorage).Should().BeAssignableTo<ISettingsStorage>();
    }

    [Fact]
    public void Model_Classes_Should_Have_Proper_Validation()
    {
        // Test that model classes have reasonable constraints and validation

        // DatabaseOptions validation
        var options = new DatabaseOptions();
        options.CommandTimeout.Should().BeGreaterThan(0);
        options.MaxBackupCount.Should().BeGreaterThan(0);
        options.BackupDirectory.Should().NotBeNullOrEmpty();

        // Entity constraints
        var config = new ConfigurationEntity();
        config.Id.Should().NotBeEmpty(); // Should auto-generate
        config.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        config.Metadata.Should().NotBeNull();

        var job = new JobEntity();
        job.Id.Should().NotBeEmpty();
        job.Status.Should().Be(JobStatus.Created); // Default status
        job.ProgressPercentage.Should().Be(0); // No items processed yet

        var result = new JobResultEntity();
        result.ExecutionTime.Should().Be(TimeSpan.Zero); // Default
        result.Variables.Should().NotBeNull();
        result.CapturedData.Should().NotBeNull();
        result.Logs.Should().NotBeNull();

        var proxy = new ProxyEntity();
        proxy.Health.Should().Be(ProxyHealth.Unknown); // Default
        proxy.IsAvailable.Should().BeTrue(); // Default state
        proxy.Address.Should().Be(":0"); // Empty host and 0 port

        var setting = new SettingEntity();
        setting.Category.Should().Be("General"); // Default category
        setting.DataType.Should().Be("string"); // Default type
    }

    private static async Task<TimeSpan> MeasureTime(Func<Task> operation)
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
