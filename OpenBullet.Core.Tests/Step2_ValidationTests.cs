using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 2 Validation Tests - Simple tests to verify implementation without external dependencies
/// </summary>
public class Step2_ValidationTests
{
    [Fact]
    public void HttpClientService_Can_Be_Created_Successfully()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HttpClientService>>();

        // Act
        var service = new HttpClientService(loggerMock.Object);

        // Assert
        service.Should().NotBeNull();
        service.Should().BeAssignableTo<IHttpClientService>();
        
        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void HttpClientService_CreateClient_Returns_Configured_Client()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HttpClientService>>();
        using var service = new HttpClientService(loggerMock.Object);
        
        var config = new HttpClientConfiguration
        {
            Timeout = TimeSpan.FromSeconds(15),
            UserAgent = "TestAgent/1.0",
            FollowRedirects = false,
            MaxRedirects = 3
        };

        // Act
        using var client = service.CreateClient(config);

        // Assert
        client.Should().NotBeNull();
        client.Timeout.Should().Be(TimeSpan.FromSeconds(15));
        client.DefaultRequestHeaders.UserAgent.ToString().Should().Contain("TestAgent");
    }

    [Fact]
    public void HttpClientService_CreateClient_With_Proxy_Does_Not_Throw()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HttpClientService>>();
        using var service = new HttpClientService(loggerMock.Object);
        
        var proxy = new ProxyInfo
        {
            Host = "proxy.test.com",
            Port = 8080,
            Type = ProxyType.Http,
            Username = "testuser",
            Password = "testpass"
        };

        // Act
        var act = () => service.CreateClient(proxy: proxy);

        // Assert
        act.Should().NotThrow();
        
        using var client = act();
        client.Should().NotBeNull();
    }

    [Fact]
    public void HttpResponseWrapper_Implements_IDisposable()
    {
        // Arrange & Act
        var wrapper = new HttpResponseWrapper();

        // Assert
        wrapper.Should().BeAssignableTo<IDisposable>();
        
        // Should not throw
        var act = () => wrapper.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void HttpClientService_Extension_Methods_Are_Available()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HttpClientService>>();
        IHttpClientService service = new HttpClientService(loggerMock.Object);
        const string testUrl = "https://example.com";

        // Act & Assert - These should compile and be available
        var getMethod = () => service.GetAsync(testUrl);
        var postFormMethod = () => service.PostFormAsync(testUrl, new Dictionary<string, string>());
        var postJsonMethod = () => service.PostJsonAsync(testUrl, new { test = "value" });

        getMethod.Should().NotBeNull();
        postFormMethod.Should().NotBeNull();
        postJsonMethod.Should().NotBeNull();
        
        // Cleanup
        ((HttpClientService)service).Dispose();
    }

    [Fact]
    public void ProxyTestResult_Properties_Work_Correctly()
    {
        // Arrange
        var proxy = Step2TestHelpers.CreateTestProxy();
        var result = new ProxyTestResult
        {
            IsWorking = true,
            ResponseTime = 150,
            ErrorMessage = null,
            PublicIP = "1.2.3.4",
            Country = "US",
            TestedProxy = proxy
        };

        // Act & Assert
        result.IsWorking.Should().BeTrue();
        result.ResponseTime.Should().Be(150);
        result.ErrorMessage.Should().BeNull();
        result.PublicIP.Should().Be("1.2.3.4");
        result.Country.Should().Be("US");
        result.TestedProxy.Should().Be(proxy);
        result.TestedProxy.ToString().Should().Be("127.0.0.1:8080");
    }

    [Fact]
    public void HttpClientConfiguration_Can_Be_Modified()
    {
        // Arrange
        var config = new HttpClientConfiguration();

        // Act
        config.Timeout = TimeSpan.FromMinutes(2);
        config.UserAgent = "Modified/2.0";
        config.DefaultHeaders["X-Custom"] = "CustomValue";
        config.IgnoreSSLErrors = true;
        config.UseHttp2 = false;

        // Assert
        config.Timeout.Should().Be(TimeSpan.FromMinutes(2));
        config.UserAgent.Should().Be("Modified/2.0");
        config.DefaultHeaders.Should().ContainKey("X-Custom");
        config.DefaultHeaders["X-Custom"].Should().Be("CustomValue");
        config.IgnoreSSLErrors.Should().BeTrue();
        config.UseHttp2.Should().BeFalse();
    }

    [Fact]
    public void HttpClientService_SetGlobalConfiguration_Updates_Future_Clients()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HttpClientService>>();
        using var service = new HttpClientService(loggerMock.Object);
        
        var newConfig = new HttpClientConfiguration
        {
            Timeout = TimeSpan.FromSeconds(45),
            UserAgent = "GlobalTest/3.0"
        };

        // Act
        service.SetGlobalConfiguration(newConfig);

        // Assert
        using var client = service.CreateClient();
        client.Timeout.Should().Be(TimeSpan.FromSeconds(45));
        client.DefaultRequestHeaders.UserAgent.ToString().Should().Contain("GlobalTest");
    }

    [Fact]
    public async Task HttpClientService_TestProxyAsync_Handles_Invalid_Url()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HttpClientService>>();
        using var service = new HttpClientService(loggerMock.Object);
        
        var proxy = new ProxyInfo
        {
            Host = "nonexistent-proxy-host-xyz.invalid",
            Port = 8080,
            Type = ProxyType.Http
        };

        // Act
        var result = await service.TestProxyAsync(proxy, "https://nonexistent-url-xyz.invalid", TimeSpan.FromSeconds(1));

        // Assert
        result.Should().NotBeNull();
        result.IsWorking.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        result.TestedProxy.Should().Be(proxy);
        result.ResponseTime.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("127.0.0.1", 8080, ProxyType.Http)]
    [InlineData("proxy.example.com", 3128, ProxyType.Http)]
    [InlineData("socks.example.com", 1080, ProxyType.Socks5)]
    public void ProxyInfo_Different_Configurations_Work(string host, int port, ProxyType type)
    {
        // Arrange & Act
        var proxy = new ProxyInfo
        {
            Host = host,
            Port = port,
            Type = type
        };

        // Assert
        proxy.Host.Should().Be(host);
        proxy.Port.Should().Be(port);
        proxy.Type.Should().Be(type);
        proxy.ToString().Should().Be($"{host}:{port}");
    }

    [Fact]
    public void Step2TestHelpers_Create_Valid_Objects()
    {
        // Act
        var getRequest = Step2TestHelpers.CreateGetRequest("https://test.com");
        var postRequest = Step2TestHelpers.CreatePostRequest("https://test.com", "{\"test\":\"data\"}", "application/json");
        var proxy = Step2TestHelpers.CreateTestProxy();
        var config = Step2TestHelpers.CreateTestConfiguration();

        // Assert
        getRequest.Method.Should().Be(HttpMethod.Get);
        getRequest.RequestUri.Should().Be(new Uri("https://test.com"));

        postRequest.Method.Should().Be(HttpMethod.Post);
        postRequest.Content.Should().NotBeNull();

        proxy.Host.Should().Be("127.0.0.1");
        proxy.Port.Should().Be(8080);
        proxy.Type.Should().Be(ProxyType.Http);

        config.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        config.UserAgent.Should().Be("TestAgent/1.0");
        config.FollowRedirects.Should().BeTrue();
        config.MaxRedirects.Should().Be(5);
        config.IgnoreSSLErrors.Should().BeTrue();
    }
}

/// <summary>
/// Performance validation tests
/// </summary>
public class Step2_PerformanceTests
{
    [Fact]
    public void HttpClientService_CreateClient_Should_Be_Fast()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HttpClientService>>();
        using var service = new HttpClientService(loggerMock.Object);
        var config = Step2TestHelpers.CreateTestConfiguration();

        // Act & Assert
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < 100; i++)
        {
            using var client = service.CreateClient(config);
            client.Should().NotBeNull();
        }
        
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should create 100 clients in under 1 second
    }

    [Fact]
    public void HttpClientConfiguration_Instantiation_Should_Be_Fast()
    {
        // Act & Assert
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < 1000; i++)
        {
            var config = new HttpClientConfiguration();
            config.Should().NotBeNull();
        }
        
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should create 1000 configs in under 100ms
    }
}
