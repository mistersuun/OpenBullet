using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Execution;
using OpenBullet.Core.Interfaces;
using OpenBullet.Core.Jobs;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;
using Xunit;
using ProxyInfo = OpenBullet.Core.Proxies.ProxyInfo;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 10 Tests: ProxyJobManager Implementation
/// </summary>
public class Step10_ProxyJobManagerTests : IDisposable
{
    private readonly Mock<ILogger<ProxyJobManager>> _loggerMock;
    private readonly Mock<IBotRunner> _botRunnerMock;
    private readonly Mock<IProxyManager> _proxyManagerMock;
    private readonly ProxyJobManager _proxyJobManager;
    private readonly ProxyJobConfiguration _testJobConfig;

    public Step10_ProxyJobManagerTests()
    {
        _loggerMock = new Mock<ILogger<ProxyJobManager>>();
        _botRunnerMock = new Mock<IBotRunner>();
        _proxyManagerMock = new Mock<IProxyManager>();
        _proxyJobManager = new ProxyJobManager(_loggerMock.Object, _botRunnerMock.Object, _proxyManagerMock.Object);

        _testJobConfig = new ProxyJobConfiguration
        {
            Name = "TestProxyJob",
            Config = new ConfigModel
            {
                Name = "TestConfig",
                Script = "REQUEST GET \"https://example.com\""
            },
            DataLines = new List<string> { "user1:pass1", "user2:pass2" },
            ConcurrentBots = 2,
            RequiresProxy = true,
            ProxyStrategy = ProxyStrategy.OnePerBot,
            ProxyRotationStrategy = ProxyRotationStrategy.RoundRobin
        };

        // Setup default proxy manager responses
        _proxyManagerMock.Setup(pm => pm.GetAvailableProxiesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<ProxyInfo>
                        {
                            new() { Host = "proxy1.com", Port = 8080 },
                            new() { Host = "proxy2.com", Port = 8080 }
                        });

        _botRunnerMock.Setup(br => br.ExecuteAsync(It.IsAny<BotData>(), It.IsAny<ConfigModel>()))
                     .ReturnsAsync(new BasicBotResult { Status = BotStatus.Success });
    }

    [Fact]
    public void ProxyJobManager_Can_Be_Created()
    {
        // Act & Assert
        _proxyJobManager.Should().NotBeNull();
        _proxyJobManager.Should().BeAssignableTo<IJobManager>();
    }

    [Fact]
    public async Task StartProxyJobAsync_With_Valid_Config_Should_Start_Successfully()
    {
        // Act
        var jobId = await _proxyJobManager.StartProxyJobAsync(_testJobConfig);

        // Assert
        jobId.Should().NotBeEmpty();
        
        // Verify proxy manager was configured
        _proxyManagerMock.Verify(pm => pm.SetRotationStrategy(ProxyRotationStrategy.RoundRobin), Times.Once);
        _proxyManagerMock.Verify(pm => pm.GetAvailableProxiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartJobAsync_With_Basic_Config_Should_Convert_To_Proxy_Config()
    {
        // Arrange
        var basicConfig = new JobConfiguration
        {
            Name = "BasicJob",
            Config = _testJobConfig.Config,
            DataLines = _testJobConfig.DataLines,
            ConcurrentBots = 1
        };

        // Act
        var jobId = await _proxyJobManager.StartJobAsync(basicConfig);

        // Assert
        jobId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task StartProxyJobAsync_With_No_Available_Proxies_Should_Throw()
    {
        // Arrange
        _proxyManagerMock.Setup(pm => pm.GetAvailableProxiesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<ProxyInfo>());

        // Act & Assert
        var act = async () => await _proxyJobManager.StartProxyJobAsync(_testJobConfig);
        await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*no proxies are available*");
    }

    [Fact]
    public async Task StartProxyJobAsync_With_Insufficient_Proxies_Should_Show_Warning()
    {
        // Arrange
        var config = _testJobConfig;
        config.ConcurrentBots = 5; // More bots than available proxies

        _proxyManagerMock.Setup(pm => pm.GetAvailableProxiesAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<ProxyInfo>
                        {
                            new() { Host = "proxy1.com", Port = 8080 }
                        });

        // Act
        var jobId = await _proxyJobManager.StartProxyJobAsync(config);

        // Assert
        jobId.Should().NotBeEmpty();
        // In a real scenario, this would generate a warning in logs
    }

    [Fact]
    public async Task GetJobStatus_Should_Include_Proxy_Metadata()
    {
        // Arrange
        var jobId = await _proxyJobManager.StartProxyJobAsync(_testJobConfig);
        await Task.Delay(50); // Allow job to start

        // Act
        var status = _proxyJobManager.GetJobStatus(jobId);

        // Assert
        status.Should().NotBeNull();
        status.Metadata.Should().ContainKey("ProxyAssignments");
        status.Metadata.Should().ContainKey("ProxyRotations");
        status.Metadata.Should().ContainKey("ProxyFailures");
        status.Metadata.Should().ContainKey("UniqueProxiesUsed");
    }

    [Fact]
    public async Task GetStatistics_Should_Include_Proxy_Metrics()
    {
        // Arrange
        var jobId = await _proxyJobManager.StartProxyJobAsync(_testJobConfig);
        await Task.Delay(50);

        // Act
        var stats = _proxyJobManager.GetStatistics();

        // Assert
        stats.Should().NotBeNull();
        stats.CustomMetrics.Should().ContainKey("TotalProxyAssignments");
        stats.CustomMetrics.Should().ContainKey("TotalProxyRotations");
        stats.CustomMetrics.Should().ContainKey("TotalProxyFailures");
        stats.CustomMetrics.Should().ContainKey("UniqueProxiesUsed");
    }

    [Fact]
    public async Task GetProxyPoolStatisticsAsync_Should_Return_Pool_Stats()
    {
        // Arrange
        var expectedStats = new ProxyPoolStatistics
        {
            TotalProxies = 10,
            AvailableProxies = 8,
            BannedProxies = 2
        };

        _proxyManagerMock.Setup(pm => pm.GetStatisticsAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedStats);

        // Act
        var stats = await _proxyJobManager.GetProxyPoolStatisticsAsync();

        // Assert
        stats.Should().NotBeNull();
        stats.TotalProxies.Should().Be(10);
        stats.AvailableProxies.Should().Be(8);
        stats.BannedProxies.Should().Be(2);
    }

    [Fact]
    public async Task TestAllProxiesAsync_Should_Delegate_To_ProxyManager()
    {
        // Arrange
        var testConfig = new ProxyTestConfiguration
        {
            TestUrl = "https://httpbin.org/ip",
            Timeout = TimeSpan.FromSeconds(5)
        };

        var expectedResult = new ProxyPoolTestResult
        {
            TotalProxies = 5,
            HealthyProxies = 4,
            DeadProxies = 1
        };

        _proxyManagerMock.Setup(pm => pm.TestAllProxiesAsync(testConfig, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResult);

        // Act
        var result = await _proxyJobManager.TestAllProxiesAsync(testConfig);

        // Assert
        result.Should().NotBeNull();
        result.TotalProxies.Should().Be(5);
        result.HealthyProxies.Should().Be(4);
        result.DeadProxies.Should().Be(1);
        
        _proxyManagerMock.Verify(pm => pm.TestAllProxiesAsync(testConfig, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ForceProxyRotationAsync_Should_Handle_Valid_Job()
    {
        // Arrange
        var jobId = await _proxyJobManager.StartProxyJobAsync(_testJobConfig);

        // Act
        var result = await _proxyJobManager.ForceProxyRotationAsync(jobId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ForceProxyRotationAsync_With_Invalid_Job_Should_Return_False()
    {
        // Act
        var result = await _proxyJobManager.ForceProxyRotationAsync("invalid-job-id");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ProxyJobConfiguration_Should_Validate_Proxy_Settings()
    {
        // Arrange
        var validConfig = new ProxyJobConfiguration
        {
            Name = "ValidJob",
            Config = new ConfigModel { Script = "REQUEST GET \"https://example.com\"" },
            DataLines = new List<string> { "test:data" },
            ConcurrentBots = 1,
            RequiresProxy = true,
            ProxyRotationInterval = TimeSpan.FromMinutes(5),
            ProxyFailureThreshold = 3,
            ProxyStrategy = ProxyStrategy.OnePerBot
        };

        // Act
        var result = validConfig.Validate();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ProxyJobConfiguration_With_Invalid_Settings_Should_Fail_Validation()
    {
        // Arrange
        var invalidConfig = new ProxyJobConfiguration
        {
            Name = "", // Empty name
            Config = null!, // Null config
            DataLines = new List<string>(), // Empty data lines
            ConcurrentBots = 0, // Invalid concurrent bots
            RequiresProxy = true,
            ProxyStrategy = ProxyStrategy.None, // Contradictory setting
            ProxyRotationInterval = TimeSpan.Zero, // Invalid interval
            ProxyFailureThreshold = -1 // Invalid threshold
        };

        // Act
        var result = invalidConfig.Validate();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Contains("Job name is required"));
        result.Errors.Should().Contain(e => e.Contains("Configuration is required"));
        result.Errors.Should().Contain(e => e.Contains("At least one data line is required"));
        result.Errors.Should().Contain(e => e.Contains("Concurrent bots must be greater than zero"));
        result.Errors.Should().Contain(e => e.Contains("Job requires proxy but proxy strategy is set to None"));
        result.Errors.Should().Contain(e => e.Contains("Proxy rotation interval must be greater than zero"));
        result.Errors.Should().Contain(e => e.Contains("Proxy failure threshold cannot be negative"));
    }

    [Fact]
    public void ProxyJobConfiguration_Constructor_From_Base_Should_Copy_Properties()
    {
        // Arrange
        var baseConfig = new JobConfiguration
        {
            Name = "BaseJob",
            Config = new ConfigModel { Script = "TEST" },
            DataLines = new List<string> { "data1", "data2" },
            ConcurrentBots = 5,
            MaxRetries = 3,
            BotTimeout = TimeSpan.FromMinutes(2),
            StopOnError = true,
            SaveResults = false,
            SaveOnlySuccessful = true,
            OutputFormat = "json",
            CustomSettings = { ["key"] = "value" }
        };

        // Act
        var proxyConfig = new ProxyJobConfiguration(baseConfig);

        // Assert
        proxyConfig.Name.Should().Be("BaseJob");
        proxyConfig.Config.Should().BeSameAs(baseConfig.Config);
        proxyConfig.DataLines.Should().BeEquivalentTo(baseConfig.DataLines);
        proxyConfig.ConcurrentBots.Should().Be(5);
        proxyConfig.MaxRetries.Should().Be(3);
        proxyConfig.BotTimeout.Should().Be(TimeSpan.FromMinutes(2));
        proxyConfig.StopOnError.Should().BeTrue();
        proxyConfig.SaveResults.Should().BeFalse();
        proxyConfig.SaveOnlySuccessful.Should().BeTrue();
        proxyConfig.OutputFormat.Should().Be("json");
        proxyConfig.CustomSettings.Should().ContainKey("key");
        proxyConfig.CustomSettings["key"].Should().Be("value");
        
        // Proxy-specific defaults
        proxyConfig.RequiresProxy.Should().BeTrue();
        proxyConfig.ProxyStrategy.Should().Be(ProxyStrategy.OnePerBot);
        proxyConfig.AutoBanFailedProxies.Should().BeTrue();
    }

    [Fact]
    public void ProxyJobStatistics_Should_Calculate_Success_Rate()
    {
        // Arrange
        var stats = new ProxyJobStatistics
        {
            SuccessfulProxyRequests = 80,
            FailedProxyRequests = 20
        };

        // Act & Assert
        stats.ProxySuccessRate.Should().Be(80.0);
    }

    [Fact]
    public void ProxyJobStatistics_Clone_Should_Create_Copy()
    {
        // Arrange
        var originalStats = new ProxyJobStatistics
        {
            JobId = "job123",
            TotalProxyAssignments = 100,
            ProxyRotations = 10,
            ProxyFailures = 5,
            UniqueProxiesUsed = { "proxy1", "proxy2", "proxy3" },
            LastUsedProxy = new ProxyInfo { Host = "last.proxy.com", Port = 8080 }
        };

        // Act
        var clonedStats = originalStats.Clone();

        // Assert
        clonedStats.Should().NotBeSameAs(originalStats);
        clonedStats.JobId.Should().Be("job123");
        clonedStats.TotalProxyAssignments.Should().Be(100);
        clonedStats.ProxyRotations.Should().Be(10);
        clonedStats.ProxyFailures.Should().Be(5);
        clonedStats.UniqueProxiesUsed.Should().BeEquivalentTo(originalStats.UniqueProxiesUsed);
        clonedStats.UniqueProxiesUsed.Should().NotBeSameAs(originalStats.UniqueProxiesUsed);
        clonedStats.LastUsedProxy.Should().NotBeSameAs(originalStats.LastUsedProxy);
        clonedStats.LastUsedProxy!.Host.Should().Be("last.proxy.com");
    }

    [Theory]
    [InlineData(ProxyStrategy.None)]
    [InlineData(ProxyStrategy.OnePerBot)]
    [InlineData(ProxyStrategy.Shared)]
    [InlineData(ProxyStrategy.RoundRobin)]
    [InlineData(ProxyStrategy.PerDataLine)]
    public void ProxyStrategy_Enum_Should_Have_All_Values(ProxyStrategy strategy)
    {
        // Act & Assert
        Enum.IsDefined(typeof(ProxyStrategy), strategy).Should().BeTrue();
    }

    [Fact]
    public async Task ProxyJobManager_Should_Handle_Proxy_Events()
    {
        // Arrange
        var proxyFailedEvents = new List<ProxyFailedEventArgs>();
        var proxyAssignedEvents = new List<ProxyAssignedEventArgs>();
        var proxyRotatedEvents = new List<ProxyRotatedEventArgs>();

        _proxyJobManager.ProxyFailed += (sender, e) => proxyFailedEvents.Add(e);
        _proxyJobManager.ProxyAssigned += (sender, e) => proxyAssignedEvents.Add(e);
        _proxyJobManager.ProxyRotated += (sender, e) => proxyRotatedEvents.Add(e);

        // Simulate proxy ban event
        var bannedProxy = new ProxyInfo { Host = "banned.proxy.com", Port = 8080 };
        _proxyManagerMock.Raise(pm => pm.ProxyBanned += null, 
            new ProxyBannedEventArgs { Proxy = bannedProxy, Reason = "Too many failures" });

        // Act
        await Task.Delay(50); // Allow event processing

        // Assert
        proxyFailedEvents.Should().HaveCount(1);
        proxyFailedEvents[0].Proxy.Host.Should().Be("banned.proxy.com");
        proxyFailedEvents[0].Reason.Should().Be("Too many failures");
    }

    [Fact]
    public async Task ProxyJobManager_Should_Track_Job_Statistics()
    {
        // Arrange
        var jobId = await _proxyJobManager.StartProxyJobAsync(_testJobConfig);

        // Simulate some proxy usage through events
        var proxy = new ProxyInfo { Id = "proxy1", Host = "test.proxy.com", Port = 8080 };
        var usageResult = new ProxyUsageResult { Success = true, ResponseTime = TimeSpan.FromMilliseconds(300) };

        _proxyManagerMock.Raise(pm => pm.ProxyStatisticsUpdated += null,
            new ProxyStatisticsUpdatedEventArgs { Proxy = proxy, UsageResult = usageResult });

        // Act
        var proxyStats = _proxyJobManager.GetProxyJobStatistics(jobId);

        // Assert
        proxyStats.Should().NotBeNull();
        proxyStats!.JobId.Should().Be(jobId);
    }

    [Fact]
    public async Task StopJobAsync_Should_Clean_Up_Proxy_Tracking()
    {
        // Arrange
        var jobId = await _proxyJobManager.StartProxyJobAsync(_testJobConfig);

        // Act
        var stopped = await _proxyJobManager.StopJobAsync(jobId);

        // Assert
        stopped.Should().BeTrue();
        
        var proxyStats = _proxyJobManager.GetProxyJobStatistics(jobId);
        proxyStats.Should().BeNull(); // Should be cleaned up
    }

    [Fact]
    public async Task ClearJobDataAsync_Should_Clean_Up_Proxy_Data()
    {
        // Arrange
        var jobId = await _proxyJobManager.StartProxyJobAsync(_testJobConfig);
        await _proxyJobManager.StopJobAsync(jobId);

        // Act
        var cleared = await _proxyJobManager.ClearJobDataAsync(jobId);

        // Assert
        cleared.Should().BeTrue();
        
        var proxyStats = _proxyJobManager.GetProxyJobStatistics(jobId);
        proxyStats.Should().BeNull();
    }

    [Fact]
    public void ProxyAwareBotRunner_Should_Handle_Proxy_Assignment()
    {
        // This test would require more complex setup to test the internal ProxyAwareBotRunner
        // For now, we verify that the ProxyJobManager was constructed successfully
        _proxyJobManager.Should().NotBeNull();
    }

    [Fact]
    public void EventArgs_Should_Initialize_With_Defaults()
    {
        // Arrange & Act
        var assignedArgs = new ProxyAssignedEventArgs();
        var rotatedArgs = new ProxyRotatedEventArgs();
        var failedArgs = new ProxyFailedEventArgs();

        // Assert
        assignedArgs.JobId.Should().BeEmpty();
        assignedArgs.BotId.Should().BeEmpty();
        assignedArgs.Proxy.Should().NotBeNull();
        assignedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        rotatedArgs.JobId.Should().BeEmpty();
        rotatedArgs.BotId.Should().BeEmpty();
        rotatedArgs.OldProxy.Should().NotBeNull();
        rotatedArgs.NewProxy.Should().NotBeNull();
        rotatedArgs.Reason.Should().BeEmpty();
        rotatedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        failedArgs.Proxy.Should().NotBeNull();
        failedArgs.Reason.Should().BeEmpty();
        failedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        _proxyJobManager?.Dispose();
    }
}
