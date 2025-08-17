using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Jobs;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;
using OpenBullet.Core.Services;
using Xunit;
using ProxyInfo = OpenBullet.Core.Proxies.ProxyInfo;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 10 Validation Tests - Basic functionality validation
/// </summary>
public class Step10_ValidationTests
{
    [Fact]
    public void IProxyManager_Interface_Should_Be_Properly_Defined()
    {
        // Assert
        typeof(IProxyManager).IsInterface.Should().BeTrue();
        
        var methods = typeof(IProxyManager).GetMethods();
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.LoadProxiesFromFileAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.LoadProxiesFromListAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.AddProxyAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.RemoveProxyAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.GetNextProxyAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.ReturnProxyAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.BanProxyAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.UnbanProxyAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.TestProxyAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.TestAllProxiesAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.GetStatisticsAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.ClearPoolAsync));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.SetRotationStrategy));
        methods.Should().Contain(m => m.Name == nameof(IProxyManager.SetConfiguration));
    }

    [Fact]
    public void ProxyManager_Can_Be_Created_With_Valid_Dependencies()
    {
        // Arrange
        var logger = new Mock<ILogger<ProxyManager>>();
        var httpClientService = new Mock<IHttpClientService>();

        // Act
        using var proxyManager = new ProxyManager(logger.Object, httpClientService.Object);

        // Assert
        proxyManager.Should().NotBeNull();
        proxyManager.Should().BeAssignableTo<IProxyManager>();
    }

    [Fact]
    public void ProxyJobManager_Can_Be_Created_With_Valid_Dependencies()
    {
        // Arrange
        var logger = new Mock<ILogger<ProxyJobManager>>();
        var botRunner = new Mock<OpenBullet.Core.Interfaces.IBotRunner>();
        var proxyManager = new Mock<IProxyManager>();

        // Act
        using var proxyJobManager = new ProxyJobManager(logger.Object, botRunner.Object, proxyManager.Object);

        // Assert
        proxyJobManager.Should().NotBeNull();
        proxyJobManager.Should().BeAssignableTo<OpenBullet.Core.Jobs.IJobManager>();
    }

    [Fact]
    public void ProxyInfo_Should_Initialize_With_Defaults()
    {
        // Act
        var proxy = new ProxyInfo();

        // Assert
        proxy.Id.Should().NotBeEmpty();
        proxy.Host.Should().BeEmpty();
        proxy.Port.Should().Be(0);
        proxy.Type.Should().Be(ProxyType.Http);
        proxy.Username.Should().BeEmpty();
        proxy.Password.Should().BeEmpty();
        proxy.Uses.Should().Be(0);
        proxy.IsBanned.Should().BeFalse();
        proxy.BannedUntil.Should().BeNull();
        proxy.BanReason.Should().BeNull();
        proxy.AverageResponseTime.Should().Be(TimeSpan.Zero);
        proxy.SuccessfulRequests.Should().Be(0);
        proxy.FailedRequests.Should().Be(0);
        proxy.Health.Should().Be(ProxyHealth.Unknown);
        proxy.AssignedTo.Should().BeNull();
        proxy.Metadata.Should().NotBeNull().And.BeEmpty();
        proxy.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ProxyHealth_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Unknown);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Healthy);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Slow);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Unreliable);
        Enum.GetValues<ProxyHealth>().Should().Contain(ProxyHealth.Dead);
    }

    [Fact]
    public void ProxyRotationStrategy_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<ProxyRotationStrategy>().Should().Contain(ProxyRotationStrategy.RoundRobin);
        Enum.GetValues<ProxyRotationStrategy>().Should().Contain(ProxyRotationStrategy.Random);
        Enum.GetValues<ProxyRotationStrategy>().Should().Contain(ProxyRotationStrategy.LeastUsed);
        Enum.GetValues<ProxyRotationStrategy>().Should().Contain(ProxyRotationStrategy.HealthBased);
        Enum.GetValues<ProxyRotationStrategy>().Should().Contain(ProxyRotationStrategy.ResponseTimeBased);
        Enum.GetValues<ProxyRotationStrategy>().Should().Contain(ProxyRotationStrategy.Sticky);
    }

    [Fact]
    public void ProxyStrategy_Enum_Should_Have_Expected_Values()
    {
        // Assert
        Enum.GetValues<ProxyStrategy>().Should().Contain(ProxyStrategy.None);
        Enum.GetValues<ProxyStrategy>().Should().Contain(ProxyStrategy.OnePerBot);
        Enum.GetValues<ProxyStrategy>().Should().Contain(ProxyStrategy.Shared);
        Enum.GetValues<ProxyStrategy>().Should().Contain(ProxyStrategy.RoundRobin);
        Enum.GetValues<ProxyStrategy>().Should().Contain(ProxyStrategy.PerDataLine);
    }

    [Fact]
    public void ProxyUsageResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ProxyUsageResult();

        // Assert
        result.Success.Should().BeFalse();
        result.ResponseTime.Should().Be(TimeSpan.Zero);
        result.ResponseCode.Should().Be(0);
        result.ErrorMessage.Should().BeNull();
        result.Exception.Should().BeNull();
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.ShouldBan.Should().BeFalse();
        result.BanReason.Should().BeNull();
        result.Metadata.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ProxyTestConfiguration_Should_Initialize_With_Defaults()
    {
        // Act
        var config = new ProxyTestConfiguration();

        // Assert
        config.TestUrl.Should().Be("https://httpbin.org/ip");
        config.Timeout.Should().Be(TimeSpan.FromSeconds(10));
        config.MaxRetries.Should().Be(1);
        config.ExpectedContent.Should().BeEmpty();
        config.UserAgent.Should().StartWith("Mozilla/5.0");
        config.ValidateSsl.Should().BeFalse();
        config.Headers.Should().NotBeNull().And.BeEmpty();
        config.TestDns.Should().BeTrue();
        config.DnsTestHosts.Should().Contain("google.com").And.Contain("cloudflare.com");
    }

    [Fact]
    public void ProxyTestResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new Proxies.ProxyTestResult();

        // Assert
        result.Proxy.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ResponseTime.Should().Be(TimeSpan.Zero);
        result.ResponseCode.Should().Be(0);
        result.ResponseContent.Should().BeNull();
        result.ErrorMessage.Should().BeNull();
        result.Exception.Should().BeNull();
        result.TestedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.DeterminedHealth.Should().Be(ProxyHealth.Unknown);
        result.TestMetadata.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ProxyPoolTestResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ProxyPoolTestResult();

        // Assert
        result.TotalProxies.Should().Be(0);
        result.TestedProxies.Should().Be(0);
        result.HealthyProxies.Should().Be(0);
        result.SlowProxies.Should().Be(0);
        result.UnreliableProxies.Should().Be(0);
        result.DeadProxies.Should().Be(0);
        result.TotalTestTime.Should().Be(TimeSpan.Zero);
        result.AverageResponseTime.Should().Be(TimeSpan.Zero);
        result.Results.Should().NotBeNull().And.BeEmpty();
        result.TestStartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.HealthRate.Should().Be(0);
        result.UsableRate.Should().Be(0);
    }

    [Fact]
    public void ProxyPoolConfiguration_Should_Initialize_With_Defaults()
    {
        // Act
        var config = new ProxyPoolConfiguration();

        // Assert
        config.RotationStrategy.Should().Be(ProxyRotationStrategy.RoundRobin);
        config.MaxConcurrentUses.Should().Be(1);
        config.AutoBanTimeout.Should().Be(TimeSpan.FromMinutes(10));
        config.MaxFailuresBeforeBan.Should().Be(3);
        config.HealthCheckInterval.Should().Be(TimeSpan.FromMinutes(5));
        config.AutoUnbanEnabled.Should().BeTrue();
        config.AutoUnbanInterval.Should().Be(TimeSpan.FromHours(1));
        config.MaxResponseTime.Should().Be(TimeSpan.FromSeconds(30));
        config.MinSuccessRate.Should().Be(70.0);
        config.RemoveDeadProxies.Should().BeFalse();
        config.MaxProxyRetries.Should().Be(2);
        config.LoadBalanceEnabled.Should().BeTrue();
        config.CustomSettings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ProxyPoolStatistics_Should_Initialize_With_Defaults()
    {
        // Act
        var stats = new ProxyPoolStatistics();

        // Assert
        stats.TotalProxies.Should().Be(0);
        stats.AvailableProxies.Should().Be(0);
        stats.BannedProxies.Should().Be(0);
        stats.HealthyProxies.Should().Be(0);
        stats.SlowProxies.Should().Be(0);
        stats.UnreliableProxies.Should().Be(0);
        stats.DeadProxies.Should().Be(0);
        stats.TotalRequests.Should().Be(0);
        stats.SuccessfulRequests.Should().Be(0);
        stats.FailedRequests.Should().Be(0);
        stats.AverageResponseTime.Should().Be(TimeSpan.Zero);
        stats.OverallSuccessRate.Should().Be(0);
        stats.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        stats.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        stats.ProxiesByType.Should().NotBeNull().And.BeEmpty();
        stats.ProxiesByHealth.Should().NotBeNull().And.BeEmpty();
        stats.CustomMetrics.Should().NotBeNull().And.BeEmpty();
        stats.AvailabilityRate.Should().Be(0);
        stats.BanRate.Should().Be(0);
        stats.HealthRate.Should().Be(0);
    }

    [Fact]
    public void ProxyJobConfiguration_Should_Initialize_With_Defaults()
    {
        // Act
        var config = new ProxyJobConfiguration();

        // Assert
        config.RequiresProxy.Should().BeTrue();
        config.ProxyRotationStrategy.Should().BeNull();
        config.ProxyRotationInterval.Should().Be(TimeSpan.FromMinutes(5));
        config.ProxyFailureThreshold.Should().Be(3);
        config.StickyProxyAssignment.Should().BeFalse();
        config.ProxyPoolConfiguration.Should().BeNull();
        config.ProxyStrategy.Should().Be(ProxyStrategy.OnePerBot);
        config.AutoBanFailedProxies.Should().BeTrue();
        config.ProxyBanDuration.Should().Be(TimeSpan.FromMinutes(10));
        config.PreferredProxyTypes.Should().NotBeNull().And.BeEmpty();
        config.ProxyCustomSettings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ProxyJobStatistics_Should_Initialize_With_Defaults()
    {
        // Act
        var stats = new ProxyJobStatistics();

        // Assert
        stats.JobId.Should().BeEmpty();
        stats.TotalProxyAssignments.Should().Be(0);
        stats.ProxyRotations.Should().Be(0);
        stats.ProxyFailures.Should().Be(0);
        stats.SuccessfulProxyRequests.Should().Be(0);
        stats.FailedProxyRequests.Should().Be(0);
        stats.UniqueProxiesUsed.Should().NotBeNull().And.BeEmpty();
        stats.LastUsedProxy.Should().BeNull();
        stats.LastFailedProxy.Should().BeNull();
        stats.StartTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        stats.LastUpdateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        stats.LastFailureTime.Should().BeNull();
        stats.ProxySuccessRate.Should().Be(0);
    }

    [Fact]
    public void Event_Arguments_Should_Initialize_With_Defaults()
    {
        // Act
        var proxyBannedArgs = new ProxyBannedEventArgs();
        var proxyUnbannedArgs = new ProxyUnbannedEventArgs();
        var proxyStatisticsUpdatedArgs = new ProxyStatisticsUpdatedEventArgs();
        var proxyAssignedArgs = new ProxyAssignedEventArgs();
        var proxyRotatedArgs = new ProxyRotatedEventArgs();
        var proxyFailedArgs = new ProxyFailedEventArgs();

        // Assert
        proxyBannedArgs.Proxy.Should().NotBeNull();
        proxyBannedArgs.Reason.Should().BeNull();
        proxyBannedArgs.BanDuration.Should().BeNull();
        proxyBannedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        proxyUnbannedArgs.Proxy.Should().NotBeNull();
        proxyUnbannedArgs.WasAutomatic.Should().BeFalse();
        proxyUnbannedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        proxyStatisticsUpdatedArgs.Proxy.Should().NotBeNull();
        proxyStatisticsUpdatedArgs.UsageResult.Should().NotBeNull();
        proxyStatisticsUpdatedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        proxyAssignedArgs.JobId.Should().BeEmpty();
        proxyAssignedArgs.BotId.Should().BeEmpty();
        proxyAssignedArgs.Proxy.Should().NotBeNull();
        proxyAssignedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        proxyRotatedArgs.JobId.Should().BeEmpty();
        proxyRotatedArgs.BotId.Should().BeEmpty();
        proxyRotatedArgs.OldProxy.Should().NotBeNull();
        proxyRotatedArgs.NewProxy.Should().NotBeNull();
        proxyRotatedArgs.Reason.Should().BeEmpty();
        proxyRotatedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        proxyFailedArgs.Proxy.Should().NotBeNull();
        proxyFailedArgs.Reason.Should().BeEmpty();
        proxyFailedArgs.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ProxyValidationResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ProxyValidationResult();

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ProxyListValidationResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ProxyListValidationResult();

        // Assert
        result.IsValid.Should().BeFalse();
        result.TotalProxies.Should().Be(0);
        result.ValidProxies.Should().Be(0);
        result.InvalidProxies.Should().Be(0);
        result.ValidationErrors.Should().NotBeNull().And.BeEmpty();
        result.SuccessRate.Should().Be(0);
    }

    [Fact]
    public void ProxyJobConfigurationValidationResult_Should_Initialize_With_Defaults()
    {
        // Act
        var result = new ProxyJobConfigurationValidationResult();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeNull().And.BeEmpty();
        result.Warnings.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void All_Components_Should_Be_Testable_Together()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ProxyManager>>();
        var httpClientServiceMock = new Mock<IHttpClientService>();
        var jobLoggerMock = new Mock<ILogger<ProxyJobManager>>();
        var botRunnerMock = new Mock<OpenBullet.Core.Interfaces.IBotRunner>();

        // Act - Create all components
        using var proxyManager = new ProxyManager(loggerMock.Object, httpClientServiceMock.Object);
        using var proxyJobManager = new ProxyJobManager(jobLoggerMock.Object, botRunnerMock.Object, proxyManager);

        // Assert - All components should be properly initialized
        proxyManager.Should().NotBeNull().And.BeAssignableTo<IProxyManager>();
        proxyJobManager.Should().NotBeNull().And.BeAssignableTo<OpenBullet.Core.Jobs.IJobManager>();

        // Verify basic functionality
        proxyManager.SetRotationStrategy(ProxyRotationStrategy.Random);
        var config = new ProxyPoolConfiguration { MaxConcurrentUses = 10 };
        proxyManager.SetConfiguration(config);
    }

    [Fact]
    public void Step10TestHelpers_Should_Create_Valid_Objects()
    {
        // Arrange & Act
        var proxy = CreateTestProxy();
        var config = CreateTestProxyJobConfig();
        var usageResult = CreateTestUsageResult();

        // Assert
        proxy.Should().NotBeNull();
        proxy.Host.Should().NotBeEmpty();
        proxy.Port.Should().BeGreaterThan(0);
        proxy.Type.Should().BeOneOf(Enum.GetValues<ProxyType>());

        config.Should().NotBeNull();
        config.Name.Should().NotBeEmpty();
        config.RequiresProxy.Should().BeTrue();
        config.Validate().IsValid.Should().BeTrue();

        usageResult.Should().NotBeNull();
        usageResult.Success.Should().BeTrue();
        usageResult.ResponseTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Theory]
    [InlineData(ProxyRotationStrategy.RoundRobin, ProxyStrategy.OnePerBot, ProxyHealth.Healthy)]
    [InlineData(ProxyRotationStrategy.Random, ProxyStrategy.Shared, ProxyHealth.Slow)]
    [InlineData(ProxyRotationStrategy.LeastUsed, ProxyStrategy.RoundRobin, ProxyHealth.Unreliable)]
    [InlineData(ProxyRotationStrategy.HealthBased, ProxyStrategy.PerDataLine, ProxyHealth.Dead)]
    [InlineData(ProxyRotationStrategy.ResponseTimeBased, ProxyStrategy.None, ProxyHealth.Unknown)]
    public void Enum_Combinations_Should_Be_Valid(ProxyRotationStrategy rotationStrategy, ProxyStrategy strategy, ProxyHealth health)
    {
        // Act & Assert
        Enum.IsDefined(typeof(ProxyRotationStrategy), rotationStrategy).Should().BeTrue();
        Enum.IsDefined(typeof(ProxyStrategy), strategy).Should().BeTrue();
        Enum.IsDefined(typeof(ProxyHealth), health).Should().BeTrue();
        
        // Should be able to create objects with these values
        var config = new ProxyJobConfiguration
        {
            ProxyRotationStrategy = rotationStrategy,
            ProxyStrategy = strategy
        };
        config.Should().NotBeNull();
        
        var proxy = new ProxyInfo { Health = health };
        proxy.Should().NotBeNull();
    }

    [Fact]
    public void Complex_Integration_Should_Work()
    {
        // Arrange
        var proxy = CreateTestProxy();
        var basicProxy = proxy.ToBasicProxy();
        var enhancedProxy = basicProxy.ToEnhancedProxy();
        var usageResult = ProxyExtensions.CreateSuccessResult(TimeSpan.FromMilliseconds(500));

        // Act & Assert - Chain of operations should work
        proxy.IsValid().Should().BeTrue();
        basicProxy.IsValid().Should().BeTrue();
        enhancedProxy.IsValid().Should().BeTrue();
        
        enhancedProxy.Host.Should().Be(proxy.Host);
        enhancedProxy.Port.Should().Be(proxy.Port);
        
        usageResult.Success.Should().BeTrue();
        usageResult.ShouldBan.Should().BeFalse();
    }

    private static ProxyInfo CreateTestProxy()
    {
        return new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Type = ProxyType.Http,
            Username = "user",
            Password = "pass",
            Health = ProxyHealth.Healthy
        };
    }

    private static ProxyJobConfiguration CreateTestProxyJobConfig()
    {
        return new ProxyJobConfiguration
        {
            Name = "TestProxyJob",
            Config = new OpenBullet.Core.Models.ConfigModel
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
    }

    private static ProxyUsageResult CreateTestUsageResult()
    {
        return ProxyExtensions.CreateSuccessResult(
            TimeSpan.FromMilliseconds(300),
            200);
    }
}
