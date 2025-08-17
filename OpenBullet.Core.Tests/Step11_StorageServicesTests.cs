using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Data;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Jobs;
using OpenBullet.Core.Models;
using System.Diagnostics;
using Xunit;
using JobStatus = OpenBullet.Core.Data.JobStatus;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 11 Tests: Storage Services Implementation Tests
/// </summary>
public class Step11_StorageServicesTests : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationStorage _configurationStorage;
    private readonly IJobStorage _jobStorage;
    private readonly IResultStorage _resultStorage;
    private readonly IProxyStorage _proxyStorage;
    private readonly ISettingsStorage _settingsStorage;

    public Step11_StorageServicesTests()
    {
        var services = new ServiceCollection();
        
        var options = new DatabaseOptions
        {
            Provider = DatabaseProvider.InMemory,
            ConnectionString = $"InMemoryStorageTest_{Guid.NewGuid():N}",
            EnableLogging = false
        };

        services.AddOpenBulletDatabase(options);
        services.AddLogging(builder => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning));

        _serviceProvider = services.BuildServiceProvider();
        _configurationStorage = _serviceProvider.GetRequiredService<IConfigurationStorage>();
        _jobStorage = _serviceProvider.GetRequiredService<IJobStorage>();
        _resultStorage = _serviceProvider.GetRequiredService<IResultStorage>();
        _proxyStorage = _serviceProvider.GetRequiredService<IProxyStorage>();
        _settingsStorage = _serviceProvider.GetRequiredService<ISettingsStorage>();
    }

    #region Configuration Storage Tests

    [Fact]
    public async Task ConfigurationStorage_CRUD_Operations_Should_Work()
    {
        // Arrange
        var config = new ConfigurationEntity
        {
            Name = "Test Configuration",
            Description = "Test description",
            Script = "REQUEST GET \"https://example.com\"",
            Category = "Testing",
            Author = "Test Author",
            Version = "1.0.0"
        };

        // Act - Create
        var saved = await _configurationStorage.SaveAsync(config);

        // Assert - Create
        saved.Should().NotBeNull();
        saved.Id.Should().NotBeEmpty();

        // Act - Read
        var retrieved = await _configurationStorage.GetByIdAsync(saved.Id);

        // Assert - Read
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test Configuration");

        // Act - Update
        retrieved.Description = "Updated description";
        var updated = await _configurationStorage.SaveAsync(retrieved);

        // Assert - Update
        updated.Description.Should().Be("Updated description");

        // Act - Delete
        var deleted = await _configurationStorage.DeleteAsync(updated.Id);

        // Assert - Delete
        deleted.Should().BeTrue();
    }

    [Fact]
    public async Task ConfigurationStorage_CreateFromModel_Should_Work()
    {
        // Arrange
        var configModel = new ConfigModel
        {
            Name = "Model Test",
            Script = "REQUEST GET \"https://api.example.com\"",
            Author = "Model Author",
            Version = "1.5.0"
        };

        // Act
        var created = await _configurationStorage.CreateFromModelAsync(configModel);

        // Assert
        created.Should().NotBeNull();
        created.Name.Should().Be("Model Test");
        created.Script.Should().Be("REQUEST GET \"https://api.example.com\"");
        created.Author.Should().Be("Model Author");
        created.Version.Should().Be("1.5.0");

        // Verify it's actually saved
        var retrieved = await _configurationStorage.GetByIdAsync(created.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Model Test");
    }

    [Fact]
    public async Task ConfigurationStorage_Search_And_Filter_Should_Work()
    {
        // Arrange
        var configs = new List<ConfigurationEntity>
        {
            new() { Name = "Web Scraper", Category = "Scraping", Description = "Scrapes web data" },
            new() { Name = "API Client", Category = "API", Description = "Calls REST API" },
            new() { Name = "Data Parser", Category = "Scraping", Description = "Parses HTML data" },
            new() { Name = "File Processor", Category = "Processing", Description = "Processes files" }
        };

        foreach (var config in configs)
        {
            await _configurationStorage.SaveAsync(config);
        }

        // Act & Assert - Get by category
        var scrapingConfigs = await _configurationStorage.GetByCategoryAsync("Scraping");
        scrapingConfigs.Should().HaveCount(2);
        scrapingConfigs.Should().OnlyContain(c => c.Category == "Scraping");

        // Act & Assert - Search by name
        var webConfigs = await _configurationStorage.SearchAsync("Web");
        webConfigs.Should().HaveCount(1);
        webConfigs.First().Name.Should().Be("Web Scraper");

        // Act & Assert - Search by description
        var dataConfigs = await _configurationStorage.SearchAsync("data");
        dataConfigs.Should().HaveCount(2); // "Scrapes web data" and "Parses HTML data"

        // Act & Assert - Paged results
        var pagedResult = await _configurationStorage.GetPagedAsync(1, 2);
        pagedResult.Items.Should().HaveCount(2);
        pagedResult.TotalCount.Should().Be(4);
        pagedResult.TotalPages.Should().Be(2);

        // Act & Assert - Filtered paged results
        var filteredPaged = await _configurationStorage.GetPagedAsync(1, 10, "Scraping");
        filteredPaged.Items.Should().HaveCount(2);
        filteredPaged.Items.Should().OnlyContain(c => c.Category == "Scraping");
    }

    [Fact]
    public async Task ConfigurationStorage_Usage_Stats_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Stats Test",
            Script = "REQUEST GET \"https://example.com\""
        });

        // Act - Update usage
        await _configurationStorage.UpdateUsageAsync(config.Id, true);
        await _configurationStorage.UpdateUsageAsync(config.Id, true);
        await _configurationStorage.UpdateUsageAsync(config.Id, false);

        // Act - Get stats
        var stats = await _configurationStorage.GetUsageStatsAsync(config.Id);

        // Assert
        stats.Should().NotBeNull();
        // Note: These stats come from related jobs, which we haven't created yet
        // In a real scenario with jobs, we would test the actual statistics
    }

    #endregion

    #region Job Storage Tests

    [Fact]
    public async Task JobStorage_CRUD_Operations_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Job Test Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = new JobEntity
        {
            Name = "Test Job",
            ConfigurationId = config.Id,
            Status = JobStatus.Created,
            TotalItems = 100,
            ConcurrentBots = 5
        };

        // Act - Create
        var saved = await _jobStorage.SaveAsync(job);

        // Assert - Create
        saved.Should().NotBeNull();
        saved.Id.Should().NotBeEmpty();
        saved.ConfigurationId.Should().Be(config.Id);

        // Act - Update status
        await _jobStorage.UpdateStatusAsync(saved.Id, JobStatus.Running);

        // Act - Read
        var retrieved = await _jobStorage.GetByIdAsync(saved.Id);

        // Assert - Read
        retrieved.Should().NotBeNull();
        retrieved!.Status.Should().Be(JobStatus.Running);
        retrieved.StartedAt.Should().NotBeNull(); // Should be set when status changes to Running

        // Act - Update progress
        await _jobStorage.UpdateProgressAsync(saved.Id, 50, 45, 5);

        var afterProgress = await _jobStorage.GetByIdAsync(saved.Id);

        // Assert - Update progress
        afterProgress!.ProcessedItems.Should().Be(50);
        afterProgress.SuccessfulItems.Should().Be(45);
        afterProgress.FailedItems.Should().Be(5);
        afterProgress.SuccessRate.Should().Be(90.0); // 45/50 * 100

        // Act - Complete job
        var duration = TimeSpan.FromMinutes(10);
        await _jobStorage.CompleteJobAsync(saved.Id, JobStatus.Completed, duration);

        var completed = await _jobStorage.GetByIdAsync(saved.Id);

        // Assert - Complete job
        completed!.Status.Should().Be(JobStatus.Completed);
        completed.Duration.Should().Be(duration);
        completed.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task JobStorage_CreateFromConfiguration_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Source Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var jobConfig = new JobConfiguration
        {
            Name = "Test Job from Config",
            DataLines = new List<string> { "data1", "data2", "data3" },
            ConcurrentBots = 3,
            CustomSettings = { ["TestSetting"] = "TestValue" }
        };

        // Act
        var created = await _jobStorage.CreateFromConfigurationAsync(config.Id, jobConfig);

        // Assert
        created.Should().NotBeNull();
        created.Name.Should().Be("Test Job from Config");
        created.ConfigurationId.Should().Be(config.Id);
        created.TotalItems.Should().Be(3);
        created.ConcurrentBots.Should().Be(3);
        created.Status.Should().Be(JobStatus.Created);

        // Verify data lines are serialized
        created.DataLinesJson.Should().NotBeNullOrEmpty();
        created.SettingsJson.Should().NotBeNullOrEmpty();
        created.SettingsJson.Should().Contain("TestSetting");
    }

    [Fact]
    public async Task JobStorage_Query_Methods_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Query Test Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var jobs = new List<JobEntity>
        {
            new() { Name = "Job 1", ConfigurationId = config.Id, Status = JobStatus.Completed, StartedAt = DateTime.UtcNow.AddHours(-3) },
            new() { Name = "Job 2", ConfigurationId = config.Id, Status = JobStatus.Running, StartedAt = DateTime.UtcNow.AddHours(-2) },
            new() { Name = "Job 3", ConfigurationId = config.Id, Status = JobStatus.Failed, StartedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Name = "Job 4", ConfigurationId = config.Id, Status = JobStatus.Running, StartedAt = DateTime.UtcNow.AddMinutes(-30) }
        };

        foreach (var job in jobs)
        {
            await _jobStorage.SaveAsync(job);
        }

        // Act & Assert - Get recent jobs
        var recent = await _jobStorage.GetRecentAsync(3);
        recent.Should().HaveCount(3);

        // Act & Assert - Get running jobs
        var running = await _jobStorage.GetRunningAsync();
        running.Should().HaveCount(2);
        running.Should().OnlyContain(j => j.Status == JobStatus.Running);

        // Act & Assert - Get with filter
        var filter = new JobFilter
        {
            Status = JobStatus.Completed,
            StartDateFrom = DateTime.UtcNow.AddHours(-4)
        };

        var filtered = await _jobStorage.GetPagedAsync(1, 10, filter);
        filtered.Items.Should().HaveCount(1);
        filtered.Items.First().Status.Should().Be(JobStatus.Completed);
    }

    [Fact]
    public async Task JobStorage_Statistics_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Stats Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = await _jobStorage.SaveAsync(new JobEntity
        {
            Name = "Stats Job",
            ConfigurationId = config.Id,
            Status = JobStatus.Completed,
            ProcessedItems = 100,
            SuccessfulItems = 85,
            FailedItems = 15,
            Duration = TimeSpan.FromMinutes(10)
        });

        // Create some job results for statistics
        var results = new List<JobResultEntity>
        {
            new() { JobId = job.Id, DataLine = "data1", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(1) },
            new() { JobId = job.Id, DataLine = "data2", Status = BotStatus.Failure, Success = false, ExecutionTime = TimeSpan.FromSeconds(2) },
            new() { JobId = job.Id, DataLine = "data3", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(1.5) }
        };

        foreach (var result in results)
        {
            await _resultStorage.SaveAsync(result);
        }

        // Act
        var jobStats = await _jobStorage.GetStatisticsAsync(job.Id);
        var overallStats = await _jobStorage.GetOverallStatisticsAsync();

        // Assert - Job statistics
        jobStats.Should().NotBeNull();
        jobStats.JobId.Should().Be(job.Id);
        jobStats.Duration.Should().Be(TimeSpan.FromMinutes(10));
        jobStats.StatusCounts.Should().ContainKey(BotStatus.Success);
        jobStats.StatusCounts[BotStatus.Success].Should().Be(2);
        jobStats.StatusCounts[BotStatus.Failure].Should().Be(1);

        // Assert - Overall statistics
        overallStats.Should().NotBeNull();
        overallStats.TotalJobs.Should().BeGreaterThan(0);
        overallStats.CompletedJobs.Should().BeGreaterThan(0);
    }

    #endregion

    #region Result Storage Tests

    [Fact]
    public async Task ResultStorage_CRUD_Operations_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Result Test Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = await _jobStorage.SaveAsync(new JobEntity
        {
            Name = "Result Test Job",
            ConfigurationId = config.Id
        });

        var result = new JobResultEntity
        {
            JobId = job.Id,
            DataLine = "test:data",
            Status = BotStatus.Success,
            Success = true,
            ExecutionTime = TimeSpan.FromMilliseconds(500),
            Variables = { ["TestVar"] = "TestValue" },
            CapturedData = { ["Email"] = "test@example.com" }
        };

        // Act - Create
        var saved = await _resultStorage.SaveAsync(result);

        // Assert - Create
        saved.Should().NotBeNull();
        saved.JobId.Should().Be(job.Id);
        saved.Variables.Should().ContainKey("TestVar");
        saved.CapturedData.Should().ContainKey("Email");

        // Act - Read by job
        var jobResults = await _resultStorage.GetByJobIdAsync(job.Id);

        // Assert - Read by job
        jobResults.Should().HaveCount(1);
        jobResults.First().DataLine.Should().Be("test:data");
    }

    [Fact]
    public async Task ResultStorage_CreateFromBotResult_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Bot Result Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = await _jobStorage.SaveAsync(new JobEntity
        {
            Name = "Bot Result Job",
            ConfigurationId = config.Id
        });

        var botResult = new BotResult
        {
            DataLine = "bot:test:data",
            Status = BotStatus.Success,
            CustomStatus = "Custom Success",
            Success = true,
            ExecutionTime = TimeSpan.FromSeconds(2),
            Variables = { ["BotVar"] = "BotValue" },
            CapturedData = { ["Username"] = "testuser" },
            Logs = { "Starting bot", "Request sent", "Response received" },
            ErrorMessage = null,
            Metadata = 
            {
                ["LastResponseCode"] = 200,
                ["LastAddress"] = "https://example.com/success",
                ["ProxyUsed"] = "proxy.example.com:8080",
                ["ResponseTime"] = 1500L
            }
        };

        // Act
        var created = await _resultStorage.CreateFromBotResultAsync(job.Id, botResult);

        // Assert
        created.Should().NotBeNull();
        created.JobId.Should().Be(job.Id);
        created.DataLine.Should().Be("bot:test:data");
        created.Status.Should().Be(BotStatus.Success);
        created.CustomStatus.Should().Be("Custom Success");
        created.Success.Should().BeTrue();
        created.Variables.Should().ContainKey("BotVar");
        created.CapturedData.Should().ContainKey("Username");
        created.Logs.Should().HaveCount(3);
        created.ResponseCode.Should().Be(200);
        created.FinalUrl.Should().Be("https://example.com/success");
        created.ProxyUsed.Should().Be("proxy.example.com:8080");
        created.ResponseTime.Should().Be(1500L);
    }

    [Fact]
    public async Task ResultStorage_Filtering_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Filter Test Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = await _jobStorage.SaveAsync(new JobEntity
        {
            Name = "Filter Test Job",
            ConfigurationId = config.Id
        });

        var results = new List<JobResultEntity>
        {
            new() { JobId = job.Id, DataLine = "success:data", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(1), ResponseCode = 200 },
            new() { JobId = job.Id, DataLine = "failure:data", Status = BotStatus.Failure, Success = false, ExecutionTime = TimeSpan.FromSeconds(2), ResponseCode = 404, ErrorMessage = "Not found" },
            new() { JobId = job.Id, DataLine = "retry:data", Status = BotStatus.Retry, Success = false, ExecutionTime = TimeSpan.FromSeconds(3), ResponseCode = 500 },
            new() { JobId = job.Id, DataLine = "ban:data", Status = BotStatus.Ban, Success = false, ExecutionTime = TimeSpan.FromSeconds(0.5), ResponseCode = 429 }
        };

        await _resultStorage.SaveRangeAsync(results);

        // Act & Assert - Get successful results
        var successful = await _resultStorage.GetSuccessfulAsync(job.Id);
        successful.Should().HaveCount(1);
        successful.First().Status.Should().Be(BotStatus.Success);

        // Act & Assert - Get failed results
        var failed = await _resultStorage.GetFailedAsync(job.Id);
        failed.Should().HaveCount(3);
        failed.Should().OnlyContain(r => !r.Success);

        // Act & Assert - Filtered paged results
        var filter = new ResultFilter
        {
            Status = BotStatus.Failure,
            ResponseCodeFrom = 400,
            ResponseCodeTo = 499
        };

        var filtered = await _resultStorage.GetPagedAsync(job.Id, 1, 10, filter);
        filtered.Items.Should().HaveCount(1);
        filtered.Items.First().ResponseCode.Should().Be(404);

        // Act & Assert - Search by text
        var searchFilter = new ResultFilter { SearchTerm = "success" };
        var searched = await _resultStorage.GetPagedAsync(job.Id, 1, 10, searchFilter);
        searched.Items.Should().HaveCount(1);
        searched.Items.First().DataLine.Should().Contain("success");
    }

    [Fact]
    public async Task ResultStorage_CapturedData_Operations_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Captured Data Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = await _jobStorage.SaveAsync(new JobEntity
        {
            Name = "Captured Data Job",
            ConfigurationId = config.Id
        });

        var results = new List<JobResultEntity>
        {
            new() 
            { 
                JobId = job.Id, 
                DataLine = "user1:pass1", 
                Status = BotStatus.Success, 
                CapturedData = { ["Email"] = "user1@example.com", ["Balance"] = "100.50", ["Level"] = "Premium" }
            },
            new() 
            { 
                JobId = job.Id, 
                DataLine = "user2:pass2", 
                Status = BotStatus.Success, 
                CapturedData = { ["Email"] = "user2@example.com", ["Balance"] = "50.25", ["Level"] = "Basic" }
            },
            new() 
            { 
                JobId = job.Id, 
                DataLine = "user3:pass3", 
                Status = BotStatus.Success, 
                CapturedData = { ["Email"] = "user3@test.org", ["Balance"] = "200.00", ["Level"] = "Premium" }
            }
        };

        await _resultStorage.SaveRangeAsync(results);

        // Act & Assert - Get captured data keys
        var keys = await _resultStorage.GetCapturedDataKeysAsync(job.Id);
        keys.Should().Contain("Email", "Balance", "Level");

        // Act & Assert - Get captured data values
        var emailValues = await _resultStorage.GetCapturedDataValuesAsync(job.Id, "Email");
        emailValues.Should().HaveCount(3);
        emailValues.Should().Contain("user1@example.com", "user2@example.com", "user3@test.org");

        var levelValues = await _resultStorage.GetCapturedDataValuesAsync(job.Id, "Level");
        levelValues.Should().HaveCount(2); // "Premium" and "Basic"
        levelValues.Should().Contain("Premium", "Basic");

        // Act & Assert - Search by captured data
        var premiumUsers = await _resultStorage.SearchByCapturedDataAsync(job.Id, "Level", "Premium");
        premiumUsers.Should().HaveCount(2);
        premiumUsers.Should().OnlyContain(r => r.CapturedData["Level"].ToString() == "Premium");

        var testOrgUsers = await _resultStorage.SearchByCapturedDataAsync(job.Id, "Email", "test.org");
        testOrgUsers.Should().HaveCount(1);
        testOrgUsers.First().DataLine.Should().Be("user3:pass3");
    }

    [Fact]
    public async Task ResultStorage_Statistics_Should_Work()
    {
        // Arrange
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Result Stats Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = await _jobStorage.SaveAsync(new JobEntity
        {
            Name = "Result Stats Job",
            ConfigurationId = config.Id
        });

        var results = new List<JobResultEntity>
        {
            new() { JobId = job.Id, DataLine = "data1", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(1), CapturedData = { ["Type"] = "A" } },
            new() { JobId = job.Id, DataLine = "data2", Status = BotStatus.Success, Success = true, ExecutionTime = TimeSpan.FromSeconds(2), CapturedData = { ["Type"] = "B" } },
            new() { JobId = job.Id, DataLine = "data3", Status = BotStatus.Failure, Success = false, ExecutionTime = TimeSpan.FromSeconds(0.5), CapturedData = { ["Type"] = "A" } },
            new() { JobId = job.Id, DataLine = "data4", Status = BotStatus.Retry, Success = false, ExecutionTime = TimeSpan.FromSeconds(1.5) }
        };

        await _resultStorage.SaveRangeAsync(results);

        // Act
        var stats = await _resultStorage.GetStatisticsAsync(job.Id);

        // Assert
        stats.Should().NotBeNull();
        stats.TotalResults.Should().Be(4);
        stats.SuccessfulResults.Should().Be(2);
        stats.FailedResults.Should().Be(2);
        stats.StatusCounts.Should().ContainKey(BotStatus.Success);
        stats.StatusCounts[BotStatus.Success].Should().Be(2);
        stats.StatusCounts[BotStatus.Failure].Should().Be(1);
        stats.StatusCounts[BotStatus.Retry].Should().Be(1);
        stats.CapturedDataSummary.Should().ContainKey("Type");
        stats.CapturedDataSummary["Type"].Should().Be(3); // 3 results have "Type" captured data
        stats.AverageExecutionTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    #endregion

    #region Proxy Storage Tests

    [Fact]
    public async Task ProxyStorage_CRUD_Operations_Should_Work()
    {
        // Arrange
        var proxy = new ProxyEntity
        {
            Host = "proxy.example.com",
            Port = 8080,
            Type = ProxyType.Http,
            Username = "user",
            Password = "pass",
            Source = "Test"
        };

        // Act - Create
        var saved = await _proxyStorage.SaveAsync(proxy);

        // Assert - Create
        saved.Should().NotBeNull();
        saved.Address.Should().Be("proxy.example.com:8080");

        // Act - Update statistics
        await _proxyStorage.UpdateStatisticsAsync(saved.Id, true, TimeSpan.FromMilliseconds(500));
        await _proxyStorage.UpdateStatisticsAsync(saved.Id, true, TimeSpan.FromMilliseconds(300));
        await _proxyStorage.UpdateStatisticsAsync(saved.Id, false, TimeSpan.FromMilliseconds(1000));

        // Act - Read
        var allProxies = await _proxyStorage.GetAllAsync();
        var retrieved = allProxies.FirstOrDefault(p => p.Id == saved.Id);

        // Assert - Update statistics effect
        retrieved.Should().NotBeNull();
        retrieved!.SuccessfulRequests.Should().Be(2);
        retrieved.FailedRequests.Should().Be(1);
        retrieved.TotalRequests.Should().Be(3);
        retrieved.SuccessRate.Should().BeApproximately(66.67, 0.1);
        retrieved.AverageResponseTimeMs.Should().BeGreaterThan(0);

        // Act - Ban proxy
        await _proxyStorage.BanProxyAsync(saved.Id, TimeSpan.FromMinutes(10), "Test ban");

        var allProxiesAfterBan = await _proxyStorage.GetAllAsync();
        var banned = allProxiesAfterBan.FirstOrDefault(p => p.Id == saved.Id);

        // Assert - Ban proxy
        banned!.IsBanned.Should().BeTrue();
        banned.BanReason.Should().Be("Test ban");
        banned.BannedUntil.Should().NotBeNull();
        banned.IsCurrentlyBanned.Should().BeTrue();

        // Act - Unban proxy
        await _proxyStorage.UnbanProxyAsync(saved.Id);

        var allProxiesAfterUnban = await _proxyStorage.GetAllAsync();
        var unbanned = allProxiesAfterUnban.FirstOrDefault(p => p.Id == saved.Id);

        // Assert - Unban proxy
        unbanned!.IsBanned.Should().BeFalse();
        unbanned.BanReason.Should().BeNull();
        unbanned.BannedUntil.Should().BeNull();
        unbanned.IsCurrentlyBanned.Should().BeFalse();
    }

    [Fact]
    public async Task ProxyStorage_ImportFromStrings_Should_Work()
    {
        // Arrange
        var proxyStrings = new List<string>
        {
            "127.0.0.1:8080",
            "http://proxy1.example.com:3128",
            "socks5://user:pass@proxy2.example.com:1080",
            "invalid-proxy-string",
            "proxy3.example.com:8888"
        };

        // Act
        var result = await _proxyStorage.ImportFromStringsAsync(proxyStrings, "Test Import");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ImportedCount.Should().Be(4); // 4 valid proxy strings
        result.ErrorCount.Should().Be(1); // 1 invalid string
        result.SkippedCount.Should().Be(0);
        result.Errors.Should().HaveCount(1);

        // Verify proxies were actually imported
        var allProxies = await _proxyStorage.GetAllAsync();
        var importedProxies = allProxies.Where(p => p.Source == "Test Import").ToList();
        importedProxies.Should().HaveCount(4);

        // Verify specific proxy types
        importedProxies.Should().Contain(p => p.Host == "127.0.0.1" && p.Port == 8080 && p.Type == ProxyType.Http);
        importedProxies.Should().Contain(p => p.Host == "proxy1.example.com" && p.Port == 3128 && p.Type == ProxyType.Http);
        importedProxies.Should().Contain(p => p.Host == "proxy2.example.com" && p.Port == 1080 && p.Type == ProxyType.Socks5 && p.Username == "user");
        importedProxies.Should().Contain(p => p.Host == "proxy3.example.com" && p.Port == 8888 && p.Type == ProxyType.Http);
    }

    [Fact]
    public async Task ProxyStorage_Filtering_Should_Work()
    {
        // Arrange
        var proxies = new List<ProxyEntity>
        {
            new() { Host = "proxy1.com", Port = 8080, Type = ProxyType.Http, Health = ProxyHealth.Healthy, Country = "US", SuccessRate = 95.0 },
            new() { Host = "proxy2.com", Port = 3128, Type = ProxyType.Http, Health = ProxyHealth.Slow, Country = "UK", SuccessRate = 85.0 },
            new() { Host = "proxy3.com", Port = 1080, Type = ProxyType.Socks5, Health = ProxyHealth.Dead, Country = "DE", SuccessRate = 20.0 },
            new() { Host = "proxy4.com", Port = 8080, Type = ProxyType.Socks4, Health = ProxyHealth.Healthy, Country = "US", SuccessRate = 98.0, IsBanned = true }
        };

        await _proxyStorage.SaveRangeAsync(proxies);

        // Act & Assert - Get available proxies (not banned, not dead)
        var available = await _proxyStorage.GetAvailableAsync();
        available.Should().HaveCount(2); // proxy1 and proxy2 (proxy3 is dead, proxy4 is banned)

        // Act & Assert - Filter by type
        var httpProxies = await _proxyStorage.GetPagedAsync(1, 10, new ProxyFilter { Type = ProxyType.Http });
        httpProxies.Items.Should().HaveCount(2);
        httpProxies.Items.Should().OnlyContain(p => p.Type == ProxyType.Http);

        // Act & Assert - Filter by health
        var healthyProxies = await _proxyStorage.GetPagedAsync(1, 10, new ProxyFilter { Health = ProxyHealth.Healthy });
        healthyProxies.Items.Should().HaveCount(2);
        healthyProxies.Items.Should().OnlyContain(p => p.Health == ProxyHealth.Healthy);

        // Act & Assert - Filter by country and success rate
        var usHighPerformance = await _proxyStorage.GetPagedAsync(1, 10, new ProxyFilter 
        { 
            Country = "US", 
            MinSuccessRate = 90.0 
        });
        usHighPerformance.Items.Should().HaveCount(2); // Both US proxies have >90% success rate
        usHighPerformance.Items.Should().OnlyContain(p => p.Country == "US" && p.SuccessRate >= 90.0);

        // Act & Assert - Filter banned proxies
        var bannedProxies = await _proxyStorage.GetPagedAsync(1, 10, new ProxyFilter { IsBanned = true });
        bannedProxies.Items.Should().HaveCount(1);
        bannedProxies.Items.First().Host.Should().Be("proxy4.com");
    }

    [Fact]
    public async Task ProxyStorage_Statistics_Should_Work()
    {
        // Arrange
        var proxies = new List<ProxyEntity>
        {
            new() { Host = "stats1.com", Port = 8080, Type = ProxyType.Http, Health = ProxyHealth.Healthy, IsActive = true, SuccessfulRequests = 100, FailedRequests = 10 },
            new() { Host = "stats2.com", Port = 3128, Type = ProxyType.Http, Health = ProxyHealth.Slow, IsActive = true, SuccessfulRequests = 50, FailedRequests = 20 },
            new() { Host = "stats3.com", Port = 1080, Type = ProxyType.Socks5, Health = ProxyHealth.Dead, IsActive = false, SuccessfulRequests = 5, FailedRequests = 50 },
            new() { Host = "stats4.com", Port = 8080, Type = ProxyType.Socks4, Health = ProxyHealth.Healthy, IsActive = true, IsBanned = true, SuccessfulRequests = 80, FailedRequests = 5 }
        };

        await _proxyStorage.SaveRangeAsync(proxies);

        // Act
        var stats = await _proxyStorage.GetStatisticsAsync();

        // Assert
        stats.Should().NotBeNull();
        stats.TotalProxies.Should().Be(4);
        stats.ActiveProxies.Should().Be(3);
        stats.BannedProxies.Should().Be(1);

        stats.HealthDistribution.Should().ContainKey(ProxyHealth.Healthy);
        stats.HealthDistribution[ProxyHealth.Healthy].Should().Be(2);
        stats.HealthDistribution[ProxyHealth.Slow].Should().Be(1);
        stats.HealthDistribution[ProxyHealth.Dead].Should().Be(1);

        stats.TypeDistribution.Should().ContainKey(ProxyType.Http);
        stats.TypeDistribution[ProxyType.Http].Should().Be(2);
        stats.TypeDistribution[ProxyType.Socks5].Should().Be(1);
        stats.TypeDistribution[ProxyType.Socks4].Should().Be(1);

        stats.OverallSuccessRate.Should().BeGreaterThan(0);
        stats.AverageResponseTime.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task ProxyStorage_Cleanup_Should_Work()
    {
        // Arrange
        var oldProxy = new ProxyEntity
        {
            Host = "old.proxy.com",
            Port = 8080,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastUsed = DateTime.UtcNow.AddDays(-8)
        };

        var deadProxy = new ProxyEntity
        {
            Host = "dead.proxy.com",
            Port = 8080,
            Health = ProxyHealth.Dead
        };

        var activeProxy = new ProxyEntity
        {
            Host = "active.proxy.com",
            Port = 8080,
            Health = ProxyHealth.Healthy,
            LastUsed = DateTime.UtcNow
        };

        await _proxyStorage.SaveRangeAsync(new[] { oldProxy, deadProxy, activeProxy });

        // Act - Cleanup old proxies (older than 5 days)
        var cleanedOld = await _proxyStorage.CleanupAsync(TimeSpan.FromDays(5), false);

        // Assert - Old proxy cleanup
        cleanedOld.Should().Be(1); // Only the old proxy should be removed

        // Act - Cleanup dead proxies
        var cleanedDead = await _proxyStorage.CleanupAsync(null, true);

        // Assert - Dead proxy cleanup
        cleanedDead.Should().Be(1); // Only the dead proxy should be removed

        // Verify active proxy remains
        var remaining = await _proxyStorage.GetAllAsync();
        remaining.Should().HaveCount(1);
        remaining.First().Host.Should().Be("active.proxy.com");
    }

    #endregion

    #region Settings Storage Tests

    [Fact]
    public async Task SettingsStorage_CRUD_Operations_Should_Work()
    {
        // Act - Set values
        await _settingsStorage.SetValueAsync("TestString", "Hello World", "Test string setting", "Test");
        await _settingsStorage.SetValueAsync("TestNumber", 42, "Test number setting", "Test");
        await _settingsStorage.SetValueAsync("TestBoolean", true, "Test boolean setting", "Test");
        await _settingsStorage.SetValueAsync("TestObject", new { Name = "Test", Value = 123 }, "Test object setting", "Test");

        // Act - Get values
        var stringValue = await _settingsStorage.GetValueAsync<string>("TestString");
        var numberValue = await _settingsStorage.GetValueAsync<int>("TestNumber");
        var boolValue = await _settingsStorage.GetValueAsync<bool>("TestBoolean");
        var objectValue = await _settingsStorage.GetValueAsync<object>("TestObject");

        // Assert - Values retrieved correctly
        stringValue.Should().Be("Hello World");
        numberValue.Should().Be(42);
        boolValue.Should().BeTrue();
        objectValue.Should().NotBeNull();

        // Act - Get non-existent value with default
        var defaultValue = await _settingsStorage.GetValueAsync("NonExistent", "DefaultValue");
        defaultValue.Should().Be("DefaultValue");

        // Act - Get settings by category
        var testSettings = await _settingsStorage.GetByCategoryAsync("Test");
        testSettings.Should().HaveCount(4);
        testSettings.Should().OnlyContain(s => s.Category == "Test");

        // Act - Update existing setting
        await _settingsStorage.SetValueAsync("TestString", "Updated Hello", "Updated description", "Test");
        var updatedValue = await _settingsStorage.GetValueAsync<string>("TestString");
        updatedValue.Should().Be("Updated Hello");

        // Act - Delete setting
        var deleted = await _settingsStorage.DeleteAsync("TestBoolean");
        deleted.Should().BeTrue();

        var afterDelete = await _settingsStorage.GetValueAsync<bool?>("TestBoolean");
        afterDelete.Should().BeNull();
    }

    [Fact]
    public async Task SettingsStorage_Complex_Types_Should_Work()
    {
        // Arrange
        var complexObject = new
        {
            Name = "Complex Object",
            Values = new[] { 1, 2, 3, 4, 5 },
            Nested = new
            {
                Property1 = "Nested Value",
                Property2 = DateTime.UtcNow,
                Property3 = true
            },
            Dictionary = new Dictionary<string, string>
            {
                ["Key1"] = "Value1",
                ["Key2"] = "Value2"
            }
        };

        var list = new List<string> { "Item1", "Item2", "Item3" };
        var dictionary = new Dictionary<string, int> { ["A"] = 1, ["B"] = 2, ["C"] = 3 };

        // Act - Set complex types
        await _settingsStorage.SetValueAsync("ComplexObject", complexObject, "Complex object test", "Complex");
        await _settingsStorage.SetValueAsync("ListObject", list, "List object test", "Complex");
        await _settingsStorage.SetValueAsync("DictionaryObject", dictionary, "Dictionary object test", "Complex");

        // Act - Get complex types
        var retrievedComplex = await _settingsStorage.GetValueAsync<object>("ComplexObject");
        var retrievedList = await _settingsStorage.GetValueAsync<List<string>>("ListObject");
        var retrievedDict = await _settingsStorage.GetValueAsync<Dictionary<string, int>>("DictionaryObject");

        // Assert
        retrievedComplex.Should().NotBeNull();
        retrievedList.Should().BeEquivalentTo(list);
        retrievedDict.Should().BeEquivalentTo(dictionary);
    }

    [Fact]
    public async Task SettingsStorage_Categories_Should_Work()
    {
        // Arrange - Create settings in different categories
        await _settingsStorage.SetValueAsync("General.Setting1", "Value1", "General setting 1", "General");
        await _settingsStorage.SetValueAsync("General.Setting2", "Value2", "General setting 2", "General");
        await _settingsStorage.SetValueAsync("Security.ApiKey", "secret123", "API Key", "Security");
        await _settingsStorage.SetValueAsync("Security.Timeout", 30, "Security timeout", "Security");
        await _settingsStorage.SetValueAsync("UI.Theme", "Dark", "UI theme", "UI");

        // Act - Get by category
        var generalSettings = await _settingsStorage.GetByCategoryAsync("General");
        var securitySettings = await _settingsStorage.GetByCategoryAsync("Security");
        var uiSettings = await _settingsStorage.GetByCategoryAsync("UI");

        // Assert
        generalSettings.Should().HaveCount(2);
        generalSettings.Should().OnlyContain(s => s.Category == "General");

        securitySettings.Should().HaveCount(2);
        securitySettings.Should().OnlyContain(s => s.Category == "Security");
        securitySettings.Should().Contain(s => s.Key == "Security.ApiKey");

        uiSettings.Should().HaveCount(1);
        uiSettings.First().Key.Should().Be("UI.Theme");

        // Act - Get all settings
        var allSettings = await _settingsStorage.GetAllAsync();
        allSettings.Should().HaveCountGreaterOrEqualTo(5);
    }

    #endregion

    [Fact]
    public async Task Performance_Test_Large_Dataset()
    {
        // This test verifies that the storage services can handle larger datasets efficiently
        var stopwatch = Stopwatch.StartNew();

        // Create a configuration for the test
        var config = await _configurationStorage.SaveAsync(new ConfigurationEntity
        {
            Name = "Performance Test Config",
            Script = "REQUEST GET \"https://example.com\""
        });

        var job = await _jobStorage.SaveAsync(new JobEntity
        {
            Name = "Performance Test Job",
            ConfigurationId = config.Id,
            TotalItems = 1000
        });

        // Create 1000 results
        var results = new List<JobResultEntity>();
        for (int i = 1; i <= 1000; i++)
        {
            results.Add(new JobResultEntity
            {
                JobId = job.Id,
                DataLine = $"data{i}:test",
                Status = i % 3 == 0 ? BotStatus.Success : (i % 2 == 0 ? BotStatus.Failure : BotStatus.Retry),
                Success = i % 3 == 0,
                ExecutionTime = TimeSpan.FromMilliseconds(i),
                CapturedData = { ["Index"] = i.ToString(), ["Type"] = (i % 2 == 0 ? "Even" : "Odd") }
            });

            // Batch insert every 100 items
            if (i % 100 == 0)
            {
                await _resultStorage.SaveRangeAsync(results.TakeLast(100));
                results.RemoveRange(results.Count - 100, 100);
            }
        }

        stopwatch.Stop();

        // Verify the data was inserted
        var allResults = await _resultStorage.GetByJobIdAsync(job.Id);
        allResults.Should().HaveCount(1000);

        // Performance should be reasonable (adjust threshold as needed)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000, "Large dataset operations should complete within 10 seconds");

        // Test pagination performance
        var pageStopwatch = Stopwatch.StartNew();
        var pagedResults = await _resultStorage.GetPagedAsync(job.Id, 1, 50);
        pageStopwatch.Stop();

        pagedResults.Items.Should().HaveCount(50);
        pagedResults.TotalCount.Should().Be(1000);
        pageStopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Pagination should be fast");

        // Test filtering performance
        var filterStopwatch = Stopwatch.StartNew();
        var filteredResults = await _resultStorage.GetPagedAsync(job.Id, 1, 100, new ResultFilter { Status = BotStatus.Success });
        filterStopwatch.Stop();

        filteredResults.Items.Should().OnlyContain(r => r.Status == BotStatus.Success);
        filterStopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Filtering should be fast");
    }

    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    // Helper method to get a repository by ID (would need to be implemented in actual storage)
    private async Task<ProxyEntity?> GetByIdAsync(string id)
    {
        var all = await _proxyStorage.GetAllAsync();
        return all.FirstOrDefault(p => p.Id == id);
    }
}
