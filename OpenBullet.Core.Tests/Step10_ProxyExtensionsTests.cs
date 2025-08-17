using FluentAssertions;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;
using Xunit;
using ProxyInfo = OpenBullet.Core.Proxies.ProxyInfo;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 10 Tests: ProxyExtensions Implementation
/// </summary>
public class Step10_ProxyExtensionsTests
{
    [Fact]
    public void ToBasicProxy_Should_Convert_Enhanced_To_Basic()
    {
        // Arrange
        var enhancedProxy = new ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Type = ProxyType.Http,
            Username = "user",
            Password = "pass",
            Uses = 10,
            LastUsed = DateTime.UtcNow.AddMinutes(-5),
            IsBanned = false,
            SuccessfulRequests = 8,
            FailedRequests = 2
        };

        // Act
        var basicProxy = enhancedProxy.ToBasicProxy();

        // Assert
        basicProxy.Should().NotBeNull();
        basicProxy.Host.Should().Be("test.proxy.com");
        basicProxy.Port.Should().Be(8080);
        basicProxy.Type.Should().Be(ProxyType.Http);
        basicProxy.Username.Should().Be("user");
        basicProxy.Password.Should().Be("pass");
        basicProxy.Uses.Should().Be(10);
        basicProxy.IsBanned.Should().BeFalse();
    }

    [Fact]
    public void ToEnhancedProxy_Should_Convert_Basic_To_Enhanced()
    {
        // Arrange
        var basicProxy = new Models.ProxyInfo
        {
            Host = "test.proxy.com",
            Port = 8080,
            Type = ProxyType.Http,
            Username = "user",
            Password = "pass",
            Uses = 5,
            LastUsed = DateTime.UtcNow.AddMinutes(-3),
            IsBanned = true
        };

        // Act
        var enhancedProxy = basicProxy.ToEnhancedProxy();

        // Assert
        enhancedProxy.Should().NotBeNull();
        enhancedProxy.Host.Should().Be("test.proxy.com");
        enhancedProxy.Port.Should().Be(8080);
        enhancedProxy.Type.Should().Be(ProxyType.Http);
        enhancedProxy.Username.Should().Be("user");
        enhancedProxy.Password.Should().Be("pass");
        enhancedProxy.Uses.Should().Be(5);
        enhancedProxy.IsBanned.Should().BeTrue();
        
        // Enhanced properties should have default values
        enhancedProxy.Health.Should().Be(ProxyHealth.Unknown);
        enhancedProxy.SuccessfulRequests.Should().Be(0);
        enhancedProxy.FailedRequests.Should().Be(0);
    }

    [Theory]
    [InlineData("proxy.com", 8080, ProxyType.Http, true)]
    [InlineData("", 8080, ProxyType.Http, false)]          // Empty host
    [InlineData("proxy.com", 0, ProxyType.Http, false)]     // Invalid port
    [InlineData("proxy.com", 65536, ProxyType.Http, false)] // Port too high
    [InlineData("proxy.com", 8080, (ProxyType)999, false)]  // Invalid type
    public void IsValid_Enhanced_Should_Validate_Correctly(string host, int port, ProxyType type, bool expectedValid)
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = host,
            Port = port,
            Type = type
        };

        // Act
        var isValid = proxy.IsValid();

        // Assert
        isValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("proxy.com", 8080, ProxyType.Http, true)]
    [InlineData("", 8080, ProxyType.Http, false)]          // Empty host
    [InlineData("proxy.com", 0, ProxyType.Http, false)]     // Invalid port
    [InlineData("proxy.com", 65536, ProxyType.Http, false)] // Port too high
    [InlineData("proxy.com", 8080, (ProxyType)999, false)]  // Invalid type
    public void IsValid_Basic_Should_Validate_Correctly(string host, int port, ProxyType type, bool expectedValid)
    {
        // Arrange
        var proxy = new Models.ProxyInfo
        {
            Host = host,
            Port = port,
            Type = type
        };

        // Act
        var isValid = proxy.IsValid();

        // Assert
        isValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("proxy.com", 8080, ProxyType.Http, "", "", false, "http://proxy.com:8080")]
    [InlineData("proxy.com", 8080, ProxyType.Http, "user", "pass", false, "http://proxy.com:8080")]
    [InlineData("proxy.com", 8080, ProxyType.Http, "user", "pass", true, "http://user:pass@proxy.com:8080")]
    [InlineData("proxy.com", 1080, ProxyType.Socks5, "user", "pass", true, "socks5://user:pass@proxy.com:1080")]
    public void ToDisplayString_Enhanced_Should_Format_Correctly(string host, int port, ProxyType type, 
        string username, string password, bool includeCredentials, string expected)
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = host,
            Port = port,
            Type = type,
            Username = username,
            Password = password
        };

        // Act
        var displayString = proxy.ToDisplayString(includeCredentials);

        // Assert
        displayString.Should().Be(expected);
    }

    [Theory]
    [InlineData("proxy.com", 8080, ProxyType.Http, "", "", false, "http://proxy.com:8080")]
    [InlineData("proxy.com", 8080, ProxyType.Http, "user", "pass", false, "http://proxy.com:8080")]
    [InlineData("proxy.com", 8080, ProxyType.Http, "user", "pass", true, "http://user:pass@proxy.com:8080")]
    [InlineData("proxy.com", 1080, ProxyType.Socks5, "user", "pass", true, "socks5://user:pass@proxy.com:1080")]
    public void ToDisplayString_Basic_Should_Format_Correctly(string host, int port, ProxyType type, 
        string username, string password, bool includeCredentials, string expected)
    {
        // Arrange
        var proxy = new Models.ProxyInfo
        {
            Host = host,
            Port = port,
            Type = type,
            Username = username,
            Password = password
        };

        // Act
        var displayString = proxy.ToDisplayString(includeCredentials);

        // Assert
        displayString.Should().Be(expected);
    }

    [Theory]
    [InlineData(ProxyHealth.Healthy, 100.0, 500, 95.0)]    // High performance
    [InlineData(ProxyHealth.Slow, 95.0, 8000, 67.6)]       // Slow response
    [InlineData(ProxyHealth.Unreliable, 80.0, 2000, 56.0)] // Unreliable
    [InlineData(ProxyHealth.Dead, 50.0, 1000, 20.0)]       // Dead proxy
    [InlineData(ProxyHealth.Unknown, 0.0, 500, 50.0)]      // Unknown/untested
    public void GetPerformanceScore_Should_Calculate_Correctly(ProxyHealth health, double successRate, 
        int responseTimeMs, double expectedScore)
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Health = health,
            SuccessfulRequests = (int)(successRate * 10),
            FailedRequests = (int)((100 - successRate) * 10),
            AverageResponseTime = TimeSpan.FromMilliseconds(responseTimeMs)
        };

        if (proxy.TotalRequests == 0)
        {
            proxy.SuccessfulRequests = 1;
            proxy.FailedRequests = 1;
        }

        // Act
        var score = proxy.GetPerformanceScore();

        // Assert
        score.Should().BeApproximately(expectedScore, 0.5);
    }

    [Theory]
    [InlineData(ProxyHealth.Healthy, 95.0, 2500, true)]   // High priority
    [InlineData(ProxyHealth.Healthy, 95.0, 4000, false)]  // Slow response, not high priority
    [InlineData(ProxyHealth.Healthy, 90.0, 2500, false)]  // Lower success rate
    [InlineData(ProxyHealth.Slow, 100.0, 1000, false)]    // Not healthy
    public void IsHighPriority_Should_Determine_Priority_Correctly(ProxyHealth health, double successRate, 
        int responseTimeMs, bool expectedHighPriority)
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Health = health,
            SuccessfulRequests = (int)(successRate * 10),
            FailedRequests = (int)((100 - successRate) * 10),
            AverageResponseTime = TimeSpan.FromMilliseconds(responseTimeMs)
        };

        // Act
        var isHighPriority = proxy.IsHighPriority();

        // Assert
        isHighPriority.Should().Be(expectedHighPriority);
    }

    [Theory]
    [InlineData(true, 500, 200, null)]                          // Success
    [InlineData(false, 1000, 500, "Server error")]             // Failure
    [InlineData(false, 30000, 407, "Proxy auth required")]     // Should ban
    [InlineData(false, 70000, 200, "Timeout")]                 // Timeout should ban
    public void CreateUsageResult_Should_Create_Correct_Result(bool success, int responseTimeMs, 
        int responseCode, string? errorMessage)
    {
        // Act
        var result = ProxyExtensions.CreateUsageResult(
            success, 
            TimeSpan.FromMilliseconds(responseTimeMs), 
            responseCode, 
            errorMessage);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().Be(success);
        result.ResponseTime.Should().Be(TimeSpan.FromMilliseconds(responseTimeMs));
        result.ResponseCode.Should().Be(responseCode);
        result.ErrorMessage.Should().Be(errorMessage);
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Check ban logic
        var shouldBan = responseCode == 407 || responseCode == 429 || 
                       (responseCode >= 500 && responseCode < 600) ||
                       responseTimeMs > 60000;
        result.ShouldBan.Should().Be(shouldBan);
    }

    [Fact]
    public void CreateSuccessResult_Should_Create_Success_Result()
    {
        // Arrange
        var responseTime = TimeSpan.FromMilliseconds(300);

        // Act
        var result = ProxyExtensions.CreateSuccessResult(responseTime, 201);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ResponseTime.Should().Be(responseTime);
        result.ResponseCode.Should().Be(201);
        result.ShouldBan.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void CreateFailureResult_Should_Create_Failure_Result()
    {
        // Arrange
        var responseTime = TimeSpan.FromMilliseconds(1500);
        var exception = new InvalidOperationException("Test error");

        // Act
        var result = ProxyExtensions.CreateFailureResult(responseTime, 500, "Server error", exception);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ResponseTime.Should().Be(responseTime);
        result.ResponseCode.Should().Be(500);
        result.ErrorMessage.Should().Be("Server error");
        result.Exception.Should().BeSameAs(exception);
        result.ShouldBan.Should().BeTrue(); // 500 should trigger ban
        result.BanReason.Should().Contain("Server error");
    }

    [Theory]
    [InlineData("127.0.0.1:8080", true, null)]
    [InlineData("http://proxy.example.com:3128", true, null)]
    [InlineData("socks5://user:pass@proxy.example.com:1080", true, null)]
    [InlineData("proxy.example.com:8080", true, null)]
    [InlineData("", false, "Proxy string is empty")]
    [InlineData("invalid-proxy", false, "Invalid proxy format")]
    [InlineData("proxy.com:99999", false, "Invalid port number: 99999")]
    [InlineData("proxy.com:abc", false, "Invalid port number: abc")]
    public void ValidateProxy_Should_Validate_Proxy_Strings(string proxyString, bool expectedValid, string? expectedError)
    {
        // Act
        var result = ProxyExtensions.ValidateProxy(proxyString);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().Be(expectedValid);
        
        if (expectedError != null)
        {
            result.ErrorMessage.Should().Contain(expectedError);
        }
    }

    [Fact]
    public void ValidateProxyList_Should_Validate_Multiple_Proxies()
    {
        // Arrange
        var proxyStrings = new List<string>
        {
            "127.0.0.1:8080",              // Valid
            "http://proxy.com:3128",       // Valid
            "invalid-proxy",               // Invalid
            "",                            // Invalid
            "socks5://user:pass@proxy2.com:1080" // Valid
        };

        // Act
        var result = ProxyExtensions.ValidateProxyList(proxyStrings);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse(); // Has invalid proxies
        result.TotalProxies.Should().Be(5);
        result.ValidProxies.Should().Be(3);
        result.InvalidProxies.Should().Be(2);
        result.SuccessRate.Should().Be(60.0);
        result.ValidationErrors.Should().HaveCount(2);
    }

    [Fact]
    public void ValidateProxyList_With_All_Valid_Proxies_Should_Pass()
    {
        // Arrange
        var proxyStrings = new List<string>
        {
            "127.0.0.1:8080",
            "http://proxy.com:3128",
            "socks5://user:pass@proxy2.com:1080"
        };

        // Act
        var result = ProxyExtensions.ValidateProxyList(proxyStrings);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.TotalProxies.Should().Be(3);
        result.ValidProxies.Should().Be(3);
        result.InvalidProxies.Should().Be(0);
        result.SuccessRate.Should().Be(100.0);
        result.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateProxyList_With_Empty_List_Should_Pass()
    {
        // Arrange
        var proxyStrings = new List<string>();

        // Act
        var result = ProxyExtensions.ValidateProxyList(proxyStrings);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.TotalProxies.Should().Be(0);
        result.ValidProxies.Should().Be(0);
        result.InvalidProxies.Should().Be(0);
        result.SuccessRate.Should().Be(0.0);
        result.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateProxyList_With_Null_Should_Handle_Gracefully()
    {
        // Act
        var result = ProxyExtensions.ValidateProxyList(null!);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.TotalProxies.Should().Be(0);
        result.ValidProxies.Should().Be(0);
        result.InvalidProxies.Should().Be(0);
    }

    [Fact]
    public void ToBasicProxy_With_Null_Should_Throw()
    {
        // Act & Assert
        var act = () => ((ProxyInfo)null!).ToBasicProxy();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToEnhancedProxy_With_Null_Should_Throw()
    {
        // Act & Assert
        var act = () => ((Models.ProxyInfo)null!).ToEnhancedProxy();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ProxyInfo_Equals_Should_Work_Correctly()
    {
        // Arrange
        var proxy1 = new ProxyInfo { Host = "proxy.com", Port = 8080, Type = ProxyType.Http };
        var proxy2 = new ProxyInfo { Host = "proxy.com", Port = 8080, Type = ProxyType.Http };
        var proxy3 = new ProxyInfo { Host = "proxy.com", Port = 3128, Type = ProxyType.Http };
        var sameIdProxy = new ProxyInfo { Id = proxy1.Id, Host = "different.com", Port = 9999 };

        // Act & Assert
        proxy1.Equals(proxy2).Should().BeTrue();   // Same host/port/type
        proxy1.Equals(sameIdProxy).Should().BeTrue(); // Same ID
        proxy1.Equals(proxy3).Should().BeFalse();  // Different port
        proxy1.Equals(null).Should().BeFalse();
        proxy1!.Equals("not a proxy").Should().BeFalse();
    }

    [Fact]
    public void ProxyInfo_GetHashCode_Should_Be_Consistent()
    {
        // Arrange
        var proxy1 = new ProxyInfo { Host = "proxy.com", Port = 8080, Type = ProxyType.Http };
        var proxy2 = new ProxyInfo { Host = "proxy.com", Port = 8080, Type = ProxyType.Http };

        // Act & Assert
        proxy1.GetHashCode().Should().Be(proxy2.GetHashCode());
    }

    [Theory]
    [InlineData(ProxyHealth.Healthy, 100.0, 1000, ProxyHealth.Healthy)]
    [InlineData(ProxyHealth.Unknown, 95.0, 11000, ProxyHealth.Slow)]
    [InlineData(ProxyHealth.Unknown, 85.0, 3000, ProxyHealth.Unreliable)]
    [InlineData(ProxyHealth.Unknown, 50.0, 1000, ProxyHealth.Dead)]
    public void ProxyTestResult_DetermineHealth_Should_Set_Health_Correctly(ProxyHealth initialHealth, 
        double successRate, int responseTimeMs, ProxyHealth expectedHealth)
    {
        // Arrange
        var result = new Proxies.ProxyTestResult
        {
            DeterminedHealth = initialHealth,
            Success = successRate >= 50.0,
            SuccessRate = successRate,
            ResponseTime = TimeSpan.FromMilliseconds(responseTimeMs)
        };

        // Act
        var determinedHealth = result.DetermineHealth();

        // Assert
        determinedHealth.Should().Be(expectedHealth);
        result.DeterminedHealth.Should().Be(expectedHealth);
    }

    [Fact]
    public void ProxyPoolTestResult_Properties_Should_Calculate_Correctly()
    {
        // Arrange
        var result = new ProxyPoolTestResult
        {
            TotalProxies = 10,
            TestedProxies = 8,
            HealthyProxies = 5,
            SlowProxies = 2,
            UnreliableProxies = 1,
            DeadProxies = 0
        };

        // Act & Assert
        result.HealthRate.Should().Be(62.5); // 5 healthy out of 8 tested
        result.UsableProxies.Should().Be(8); // healthy + slow + unreliable
        result.UsableRate.Should().Be(100.0); // all tested proxies are usable
    }
}
