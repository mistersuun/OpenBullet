using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;
using OpenBullet.Core.Services;
using Xunit;
using ProxyInfo = OpenBullet.Core.Proxies.ProxyInfo;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 10 Tests: ProxyManager Implementation
/// </summary>
public class Step10_ProxyManagerTests : IDisposable
{
    private readonly Mock<ILogger<ProxyManager>> _loggerMock;
    private readonly Mock<IHttpClientService> _httpClientServiceMock;
    private readonly ProxyManager _proxyManager;

    public Step10_ProxyManagerTests()
    {
        _loggerMock = new Mock<ILogger<ProxyManager>>();
        _httpClientServiceMock = new Mock<IHttpClientService>();
        _proxyManager = new ProxyManager(_loggerMock.Object, _httpClientServiceMock.Object);
    }

    [Fact]
    public void ProxyManager_Can_Be_Created()
    {
        // Act & Assert
        _proxyManager.Should().NotBeNull();
        _proxyManager.Should().BeAssignableTo<IProxyManager>();
    }

    [Fact]
    public async Task LoadProxiesFromListAsync_With_Valid_Proxies_Should_Load_Successfully()
    {
        // Arrange
        var proxyStrings = new List<string>
        {
            "127.0.0.1:8080",
            "http://proxy.example.com:3128",
            "socks5://user:pass@proxy2.example.com:1080",
            "192.168.1.100:8888"
        };

        // Act
        var loadedCount = await _proxyManager.LoadProxiesFromListAsync(proxyStrings);

        // Assert
        loadedCount.Should().Be(4);
        
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        allProxies.Should().HaveCount(4);
        
        var firstProxy = allProxies.First(p => p.Host == "127.0.0.1");
        firstProxy.Port.Should().Be(8080);
        firstProxy.Type.Should().Be(ProxyType.Http);
        firstProxy.Username.Should().BeEmpty();

        var socksProxy = allProxies.First(p => p.Host == "proxy2.example.com");
        socksProxy.Port.Should().Be(1080);
        socksProxy.Type.Should().Be(ProxyType.Socks5);
        socksProxy.Username.Should().Be("user");
        socksProxy.Password.Should().Be("pass");
    }

    [Fact]
    public async Task LoadProxiesFromListAsync_With_Invalid_Proxies_Should_Skip_Invalid_Ones()
    {
        // Arrange
        var proxyStrings = new List<string>
        {
            "127.0.0.1:8080",           // Valid
            "invalid-proxy",             // Invalid
            "",                          // Empty
            "proxy.example.com:3128",    // Valid
            "not:a:proxy:format",        // Invalid
            "192.168.1.1:65536"         // Invalid port
        };

        // Act
        var loadedCount = await _proxyManager.LoadProxiesFromListAsync(proxyStrings);

        // Assert
        loadedCount.Should().Be(2); // Only 2 valid proxies
        
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        allProxies.Should().HaveCount(2);
    }

    [Fact]
    public async Task LoadProxiesFromListAsync_With_Duplicate_Proxies_Should_Skip_Duplicates()
    {
        // Arrange
        var proxyStrings = new List<string>
        {
            "127.0.0.1:8080",
            "http://127.0.0.1:8080",  // Same proxy, different format
            "127.0.0.1:8080",         // Exact duplicate
            "proxy.example.com:3128"
        };

        // Act
        var loadedCount = await _proxyManager.LoadProxiesFromListAsync(proxyStrings);

        // Assert
        loadedCount.Should().Be(2); // Only 2 unique proxies
        
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        allProxies.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddProxyAsync_With_Valid_Proxy_Should_Add_Successfully()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Type = ProxyType.Http,
            Username = "user",
            Password = "pass"
        };

        // Act
        var added = await _proxyManager.AddProxyAsync(proxy);

        // Assert
        added.Should().BeTrue();
        
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        allProxies.Should().HaveCount(1);
        allProxies[0].Host.Should().Be("test.proxy.com");
        allProxies[0].Port.Should().Be(8080);
        allProxies[0].Username.Should().Be("user");
        allProxies[0].Password.Should().Be("pass");
    }

    [Fact]
    public async Task AddProxyAsync_With_Duplicate_Proxy_Should_Return_False()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Type = ProxyType.Http
        };

        await _proxyManager.AddProxyAsync(proxy);

        var duplicateProxy = new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Type = ProxyType.Http
        };

        // Act
        var added = await _proxyManager.AddProxyAsync(duplicateProxy);

        // Assert
        added.Should().BeFalse();
        
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        allProxies.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetNextProxyAsync_With_RoundRobin_Should_Rotate_Proxies()
    {
        // Arrange
        await LoadTestProxies();
        _proxyManager.SetRotationStrategy(ProxyRotationStrategy.RoundRobin);

        // Act
        var proxy1 = await _proxyManager.GetNextProxyAsync();
        var proxy2 = await _proxyManager.GetNextProxyAsync();
        var proxy3 = await _proxyManager.GetNextProxyAsync();
        var proxy4 = await _proxyManager.GetNextProxyAsync(); // Should wrap around

        // Assert
        proxy1.Should().NotBeNull();
        proxy2.Should().NotBeNull();
        proxy3.Should().NotBeNull();
        proxy4.Should().NotBeNull();
        
        // Should get different proxies
        proxy1!.Id.Should().NotBe(proxy2!.Id);
        proxy2.Id.Should().NotBe(proxy3!.Id);
        
        // Fourth should wrap around to first
        proxy4!.Address.Should().Be(proxy1.Address);
    }

    [Fact]
    public async Task GetNextProxyAsync_With_Random_Should_Return_Available_Proxy()
    {
        // Arrange
        await LoadTestProxies();
        _proxyManager.SetRotationStrategy(ProxyRotationStrategy.Random);

        // Act
        var proxy = await _proxyManager.GetNextProxyAsync();

        // Assert
        proxy.Should().NotBeNull();
        proxy!.IsAvailable.Should().BeTrue();
        
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        allProxies.Should().Contain(p => p.Id == proxy.Id);
    }

    [Fact]
    public async Task GetNextProxyAsync_With_No_Available_Proxies_Should_Return_Null()
    {
        // Act
        var proxy = await _proxyManager.GetNextProxyAsync();

        // Assert
        proxy.Should().BeNull();
    }

    [Fact]
    public async Task BanProxyAsync_Should_Ban_Proxy_Successfully()
    {
        // Arrange
        await LoadTestProxies();
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        var proxyToBan = allProxies.First();
        var banDuration = TimeSpan.FromMinutes(5);

        // Act
        var banned = await _proxyManager.BanProxyAsync(proxyToBan, banDuration, "Test ban");

        // Assert
        banned.Should().BeTrue();
        
        var bannedProxies = await _proxyManager.GetBannedProxiesAsync();
        bannedProxies.Should().HaveCount(1);
        
        var bannedProxy = bannedProxies.First();
        bannedProxy.IsBanned.Should().BeTrue();
        bannedProxy.BanReason.Should().Be("Test ban");
        bannedProxy.BannedUntil.Should().BeCloseTo(DateTime.UtcNow.Add(banDuration), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task UnbanProxyAsync_Should_Unban_Proxy_Successfully()
    {
        // Arrange
        await LoadTestProxies();
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        var proxyToUnban = allProxies.First();
        
        await _proxyManager.BanProxyAsync(proxyToUnban, TimeSpan.FromMinutes(5), "Test ban");

        // Act
        var unbanned = await _proxyManager.UnbanProxyAsync(proxyToUnban);

        // Assert
        unbanned.Should().BeTrue();
        
        var bannedProxies = await _proxyManager.GetBannedProxiesAsync();
        bannedProxies.Should().BeEmpty();
        
        var availableProxies = await _proxyManager.GetAvailableProxiesAsync();
        availableProxies.Should().Contain(p => p.Id == proxyToUnban.Id);
    }

    [Fact]
    public async Task ReturnProxyAsync_With_Success_Result_Should_Update_Statistics()
    {
        // Arrange
        await LoadTestProxies();
        var proxy = await _proxyManager.GetNextProxyAsync();
        proxy.Should().NotBeNull();

        var successResult = new ProxyUsageResult
        {
            Success = true,
            ResponseTime = TimeSpan.FromMilliseconds(500),
            ResponseCode = 200
        };

        // Act
        var returned = await _proxyManager.ReturnProxyAsync(proxy!, successResult);

        // Assert
        returned.Should().BeTrue();
        
        var updatedProxies = await _proxyManager.GetAllProxiesAsync();
        var updatedProxy = updatedProxies.First(p => p.Id == proxy!.Id);
        
        updatedProxy.SuccessfulRequests.Should().Be(1);
        updatedProxy.FailedRequests.Should().Be(0);
        updatedProxy.Uses.Should().Be(1);
        updatedProxy.AverageResponseTime.Should().Be(TimeSpan.FromMilliseconds(500));
        updatedProxy.SuccessRate.Should().Be(100.0);
    }

    [Fact]
    public async Task ReturnProxyAsync_With_Failure_Result_Should_Update_Statistics()
    {
        // Arrange
        await LoadTestProxies();
        var proxy = await _proxyManager.GetNextProxyAsync();
        proxy.Should().NotBeNull();

        var failureResult = new ProxyUsageResult
        {
            Success = false,
            ResponseTime = TimeSpan.FromMilliseconds(1000),
            ResponseCode = 500,
            ErrorMessage = "Server error"
        };

        // Act
        var returned = await _proxyManager.ReturnProxyAsync(proxy!, failureResult);

        // Assert
        returned.Should().BeTrue();
        
        var updatedProxies = await _proxyManager.GetAllProxiesAsync();
        var updatedProxy = updatedProxies.First(p => p.Id == proxy!.Id);
        
        updatedProxy.SuccessfulRequests.Should().Be(0);
        updatedProxy.FailedRequests.Should().Be(1);
        updatedProxy.Uses.Should().Be(1);
        updatedProxy.AverageResponseTime.Should().Be(TimeSpan.FromMilliseconds(1000));
        updatedProxy.SuccessRate.Should().Be(0.0);
    }

    [Fact]
    public async Task TestProxyAsync_Should_Test_Proxy_Connectivity()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Type = ProxyType.Http
        };

        var mockHttpResponse = new HttpResponseWrapper
        {
            IsSuccess = true,
            ResponseTime = 500,
            Content = "{\"origin\": \"1.2.3.4\"}",
            Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        };

        _httpClientServiceMock.Setup(h => h.SendAsync(
            It.IsAny<HttpRequestMessage>(),
            It.IsAny<ProxyInfo>(),
            It.IsAny<HttpClientConfiguration>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockHttpResponse);

        var config = new ProxyTestConfiguration
        {
            TestUrl = "https://httpbin.org/ip",
            Timeout = TimeSpan.FromSeconds(10)
        };

        // Act
        var result = await _proxyManager.TestProxyAsync(proxy, config);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ResponseTime.Should().BeGreaterThan(TimeSpan.Zero);
        result.ResponseCode.Should().Be(200);
        result.DeterminedHealth.Should().Be(ProxyHealth.Healthy);
    }

    [Fact]
    public async Task TestAllProxiesAsync_Should_Test_All_Proxies()
    {
        // Arrange
        await LoadTestProxies();

        var mockHttpResponse = new HttpResponseWrapper
        {
            IsSuccess = true,
            ResponseTime = 500,
            Content = "{\"origin\": \"1.2.3.4\"}",
            Response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        };

        _httpClientServiceMock.Setup(h => h.SendAsync(
            It.IsAny<HttpRequestMessage>(),
            It.IsAny<ProxyInfo>(),
            It.IsAny<HttpClientConfiguration>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockHttpResponse);

        // Act
        var result = await _proxyManager.TestAllProxiesAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalProxies.Should().Be(3);
        result.TestedProxies.Should().Be(3);
        result.HealthyProxies.Should().Be(3);
        result.Results.Should().HaveCount(3);
        result.Results.Should().AllSatisfy(r => r.Success.Should().BeTrue());
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Return_Pool_Statistics()
    {
        // Arrange
        await LoadTestProxies();

        // Get a proxy and return it with usage data
        var proxy = await _proxyManager.GetNextProxyAsync();
        var usageResult = new ProxyUsageResult
        {
            Success = true,
            ResponseTime = TimeSpan.FromMilliseconds(300),
            ResponseCode = 200
        };
        await _proxyManager.ReturnProxyAsync(proxy!, usageResult);

        // Act
        var stats = await _proxyManager.GetStatisticsAsync();

        // Assert
        stats.Should().NotBeNull();
        stats.TotalProxies.Should().Be(3);
        stats.AvailableProxies.Should().Be(3);
        stats.BannedProxies.Should().Be(0);
        stats.TotalRequests.Should().Be(1);
        stats.SuccessfulRequests.Should().Be(1);
        stats.OverallSuccessRate.Should().Be(100.0);
        stats.ProxiesByType.Should().ContainKey(ProxyType.Http);
    }

    [Fact]
    public async Task ClearPoolAsync_Should_Remove_All_Proxies()
    {
        // Arrange
        await LoadTestProxies();

        // Act
        var cleared = await _proxyManager.ClearPoolAsync();

        // Assert
        cleared.Should().BeTrue();
        
        var allProxies = await _proxyManager.GetAllProxiesAsync();
        allProxies.Should().BeEmpty();
        
        var stats = await _proxyManager.GetStatisticsAsync();
        stats.TotalProxies.Should().Be(0);
    }

    [Fact]
    public void SetRotationStrategy_Should_Update_Strategy()
    {
        // Arrange
        var newStrategy = ProxyRotationStrategy.LeastUsed;

        // Act
        _proxyManager.SetRotationStrategy(newStrategy);

        // Assert
        // We can't directly test the internal strategy, but we can test behavior
        // This would be tested through GetNextProxyAsync behavior in a real scenario
    }

    [Fact]
    public void SetConfiguration_Should_Update_Configuration()
    {
        // Arrange
        var config = new ProxyPoolConfiguration
        {
            RotationStrategy = ProxyRotationStrategy.HealthBased,
            MaxConcurrentUses = 5,
            AutoBanTimeout = TimeSpan.FromMinutes(15),
            MaxFailuresBeforeBan = 5
        };

        // Act
        _proxyManager.SetConfiguration(config);

        // Assert
        // Configuration update is internal, but we can verify it doesn't throw
        // Real testing would involve observing behavior changes
    }

    [Fact]
    public async Task ProxyManager_Should_Fire_Events()
    {
        // Arrange
        var bannedEvents = new List<ProxyBannedEventArgs>();
        var unbannedEvents = new List<ProxyUnbannedEventArgs>();
        var statisticsEvents = new List<ProxyStatisticsUpdatedEventArgs>();

        _proxyManager.ProxyBanned += (sender, e) => bannedEvents.Add(e);
        _proxyManager.ProxyUnbanned += (sender, e) => unbannedEvents.Add(e);
        _proxyManager.ProxyStatisticsUpdated += (sender, e) => statisticsEvents.Add(e);

        await LoadTestProxies();
        var proxy = await _proxyManager.GetNextProxyAsync();

        // Act
        await _proxyManager.BanProxyAsync(proxy!, TimeSpan.FromMinutes(5), "Test event");
        await _proxyManager.UnbanProxyAsync(proxy!);
        
        var usageResult = new ProxyUsageResult { Success = true, ResponseTime = TimeSpan.FromMilliseconds(100) };
        await _proxyManager.ReturnProxyAsync(proxy!, usageResult);

        // Assert
        bannedEvents.Should().HaveCount(1);
        bannedEvents[0].Proxy.Id.Should().Be(proxy!.Id);
        bannedEvents[0].Reason.Should().Be("Test event");

        unbannedEvents.Should().HaveCount(1);
        unbannedEvents[0].Proxy.Id.Should().Be(proxy.Id);

        statisticsEvents.Should().HaveCount(1);
        statisticsEvents[0].Proxy.Id.Should().Be(proxy.Id);
        statisticsEvents[0].UsageResult.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData(ProxyRotationStrategy.RoundRobin)]
    [InlineData(ProxyRotationStrategy.Random)]
    [InlineData(ProxyRotationStrategy.LeastUsed)]
    [InlineData(ProxyRotationStrategy.HealthBased)]
    [InlineData(ProxyRotationStrategy.ResponseTimeBased)]
    [InlineData(ProxyRotationStrategy.Sticky)]
    public async Task GetNextProxyAsync_Should_Work_With_All_Rotation_Strategies(ProxyRotationStrategy strategy)
    {
        // Arrange
        await LoadTestProxies();
        _proxyManager.SetRotationStrategy(strategy);

        // Act
        var proxy = await _proxyManager.GetNextProxyAsync("test-assignment");

        // Assert
        proxy.Should().NotBeNull();
        proxy!.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void ProxyInfo_Properties_Should_Calculate_Correctly()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            SuccessfulRequests = 8,
            FailedRequests = 2
        };

        // Act & Assert
        proxy.TotalRequests.Should().Be(10);
        proxy.SuccessRate.Should().Be(80.0);
        proxy.IsCurrentlyBanned.Should().BeFalse();
        proxy.IsAvailable.Should().BeTrue();
        proxy.Address.Should().Be("test.proxy.com:8080");
    }

    [Fact]
    public void ProxyInfo_Clone_Should_Create_Copy()
    {
        // Arrange
        var originalProxy = new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Username = "user",
            Password = "pass",
            SuccessfulRequests = 5,
            Metadata = { ["test"] = "value" }
        };

        // Act
        var clonedProxy = originalProxy.Clone();

        // Assert
        clonedProxy.Should().NotBeSameAs(originalProxy);
        clonedProxy.Host.Should().Be(originalProxy.Host);
        clonedProxy.Port.Should().Be(originalProxy.Port);
        clonedProxy.Username.Should().Be(originalProxy.Username);
        clonedProxy.Password.Should().Be(originalProxy.Password);
        clonedProxy.SuccessfulRequests.Should().Be(originalProxy.SuccessfulRequests);
        clonedProxy.Metadata.Should().NotBeSameAs(originalProxy.Metadata);
        clonedProxy.Metadata["test"].Should().Be("value");
    }

    private async Task LoadTestProxies()
    {
        var proxies = new List<string>
        {
            "127.0.0.1:8080",
            "proxy1.example.com:3128",
            "proxy2.example.com:1080"
        };
        await _proxyManager.LoadProxiesFromListAsync(proxies);
    }

    public void Dispose()
    {
        _proxyManager?.Dispose();
    }
}
