using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenBullet.Core.Models;
using OpenBullet.Core.Services;
using Xunit;

namespace OpenBullet.Core.Tests;

/// <summary>
/// Step 2 Tests: HTTP Client Service
/// </summary>
public class Step2_HttpClientServiceTests : IDisposable
{
    private readonly Mock<ILogger<HttpClientService>> _loggerMock;
    private readonly HttpClientService _httpClientService;

    public Step2_HttpClientServiceTests()
    {
        _loggerMock = new Mock<ILogger<HttpClientService>>();
        _httpClientService = new HttpClientService(_loggerMock.Object);
    }

    [Fact]
    public void HttpClientService_Should_Initialize_With_Default_Configuration()
    {
        // Act
        using var client = _httpClientService.CreateClient();

        // Assert
        client.Should().NotBeNull();
        client.Timeout.Should().Be(TimeSpan.FromSeconds(10));
        client.DefaultRequestHeaders.UserAgent.ToString().Should().Contain("OpenBullet");
    }

    [Fact]
    public void CreateClient_With_Custom_Configuration_Should_Apply_Settings()
    {
        // Arrange
        var config = new HttpClientConfiguration
        {
            Timeout = TimeSpan.FromSeconds(30),
            UserAgent = "Custom-Agent/1.0",
            MaxRedirects = 5,
            FollowRedirects = false
        };

        // Act
        using var client = _httpClientService.CreateClient(config);

        // Assert
        client.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        client.DefaultRequestHeaders.UserAgent.ToString().Should().Contain("Custom-Agent");
    }

    [Fact]
    public void CreateClient_With_Proxy_Should_Configure_Proxy()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "127.0.0.1",
            Port = 8080,
            Type = ProxyType.Http,
            Username = "user",
            Password = "pass"
        };

        // Act
        using var client = _httpClientService.CreateClient(proxy: proxy);

        // Assert
        client.Should().NotBeNull();
        // Note: We can't easily test proxy configuration without making actual requests
        // This would be better tested with integration tests
    }

    [Fact]
    public void SetGlobalConfiguration_Should_Update_Configuration()
    {
        // Arrange
        var newConfig = new HttpClientConfiguration
        {
            Timeout = TimeSpan.FromMinutes(5),
            UserAgent = "GlobalAgent/2.0"
        };

        // Act
        _httpClientService.SetGlobalConfiguration(newConfig);

        // Assert
        using var client = _httpClientService.CreateClient();
        client.Timeout.Should().Be(TimeSpan.FromMinutes(5));
        client.DefaultRequestHeaders.UserAgent.ToString().Should().Contain("GlobalAgent");
    }

    [Fact]
    public void SetGlobalConfiguration_With_Null_Should_Throw()
    {
        // Act & Assert
        var act = () => _httpClientService.SetGlobalConfiguration(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void HttpResponseWrapper_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var wrapper = new HttpResponseWrapper();

        // Assert
        wrapper.Content.Should().BeEmpty();
        wrapper.ContentBytes.Should().BeEmpty();
        wrapper.ResponseTime.Should().Be(0);
        wrapper.UsedProxy.Should().BeNull();
        wrapper.FinalUrl.Should().BeEmpty();
        wrapper.Headers.Should().NotBeNull().And.BeEmpty();
        wrapper.Cookies.Should().NotBeNull();
        wrapper.IsSuccess.Should().BeFalse();
        wrapper.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void HttpClientConfiguration_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var config = new HttpClientConfiguration();

        // Assert
        config.Timeout.Should().Be(TimeSpan.FromSeconds(10));
        config.MaxRedirects.Should().Be(8);
        config.FollowRedirects.Should().BeTrue();
        config.IgnoreSSLErrors.Should().BeFalse();
        config.UserAgent.Should().Be("OpenBullet/2.0");
        config.DefaultHeaders.Should().NotBeNull().And.BeEmpty();
        config.CookieContainer.Should().NotBeNull();
        config.UseCookies.Should().BeTrue();
        config.AutoDecompression.Should().BeTrue();
        config.DecompressionMethods.Should().Be(DecompressionMethods.GZip | DecompressionMethods.Deflate);
        config.MaxConnectionsPerServer.Should().Be(50);
        config.UseHttp2.Should().BeTrue();
    }

    [Fact]
    public void ProxyTestResult_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var result = new ProxyTestResult();

        // Assert
        result.IsWorking.Should().BeFalse();
        result.ResponseTime.Should().Be(0);
        result.ErrorMessage.Should().BeNull();
        result.PublicIP.Should().BeNull();
        result.Country.Should().BeNull();
        result.TestedProxy.Should().BeNull();
    }

    // Integration tests that would require actual HTTP calls
    // These would be marked as [Fact(Skip = "Integration test - requires internet")] in a real scenario

    [Fact(Skip = "Integration test - requires internet connection")]
    public async Task SendAsync_Should_Make_Successful_Request()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://httpbin.org/get");

        // Act
        var response = await _httpClientService.SendAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
        response.Content.Should().NotBeEmpty();
        response.ResponseTime.Should().BeGreaterThan(0);
        response.FinalUrl.Should().Contain("httpbin.org");
    }

    [Fact(Skip = "Integration test - requires internet connection")]
    public async Task GetAsync_Extension_Should_Work()
    {
        // Arrange
        const string url = "https://httpbin.org/get";

        // Act
        var response = await _httpClientService.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
        response.Content.Should().Contain("httpbin.org");
    }

    [Fact(Skip = "Integration test - requires internet connection")]
    public async Task PostFormAsync_Extension_Should_Work()
    {
        // Arrange
        const string url = "https://httpbin.org/post";
        var formData = new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key2"] = "value2"
        };

        // Act
        var response = await _httpClientService.PostFormAsync(url, formData);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
        response.Content.Should().Contain("key1");
        response.Content.Should().Contain("value1");
    }

    [Fact(Skip = "Integration test - requires internet connection")]
    public async Task PostJsonAsync_Extension_Should_Work()
    {
        // Arrange
        const string url = "https://httpbin.org/post";
        var data = new { name = "test", value = 123 };

        // Act
        var response = await _httpClientService.PostJsonAsync(url, data);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
        response.Content.Should().Contain("test");
        response.Content.Should().Contain("123");
    }

    [Fact(Skip = "Integration test - requires working proxy")]
    public async Task TestProxyAsync_With_Valid_Proxy_Should_Return_Success()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "127.0.0.1",
            Port = 8080,
            Type = ProxyType.Http
        };

        // Act
        var result = await _httpClientService.TestProxyAsync(proxy);

        // Assert
        result.Should().NotBeNull();
        result.TestedProxy.Should().Be(proxy);
        result.ResponseTime.Should().BeGreaterThan(0);
        // result.IsWorking would depend on proxy availability
    }

    [Fact]
    public async Task TestProxyAsync_With_Invalid_Proxy_Should_Return_Failure()
    {
        // Arrange
        var proxy = new ProxyInfo
        {
            Host = "invalid-proxy-host-12345.com",
            Port = 8080,
            Type = ProxyType.Http
        };

        // Act
        var result = await _httpClientService.TestProxyAsync(proxy, timeout: TimeSpan.FromSeconds(2));

        // Assert
        result.Should().NotBeNull();
        result.IsWorking.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        result.TestedProxy.Should().Be(proxy);
    }

    public void Dispose()
    {
        _httpClientService?.Dispose();
    }
}

/// <summary>
/// Mock HTTP tests using a test server
/// </summary>
public class MockHttpClientServiceTests : IDisposable
{
    private readonly TestHttpServer _testServer;
    private readonly HttpClientService _httpClientService;
    private readonly Mock<ILogger<HttpClientService>> _loggerMock;

    public MockHttpClientServiceTests()
    {
        _testServer = new TestHttpServer();
        _loggerMock = new Mock<ILogger<HttpClientService>>();
        _httpClientService = new HttpClientService(_loggerMock.Object);
    }

    [Fact]
    public async Task SendAsync_Should_Handle_Success_Response()
    {
        // Arrange
        const string expectedContent = "Hello, World!";
        _testServer.SetupResponse(HttpStatusCode.OK, expectedContent);
        
        var request = new HttpRequestMessage(HttpMethod.Get, _testServer.BaseUrl + "/test");

        // Act
        var response = await _httpClientService.SendAsync(request);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Content.Should().Be(expectedContent);
        response.ResponseTime.Should().BeGreaterThan(0);
        response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendAsync_Should_Handle_Error_Response()
    {
        // Arrange
        _testServer.SetupResponse(HttpStatusCode.NotFound, "Not Found");
        
        var request = new HttpRequestMessage(HttpMethod.Get, _testServer.BaseUrl + "/notfound");

        // Act
        var response = await _httpClientService.SendAsync(request);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Should().Be("Not Found");
    }

    [Fact]
    public async Task SendAsync_Should_Handle_Timeout()
    {
        // Arrange
        _testServer.SetupDelay(TimeSpan.FromSeconds(5));
        
        var config = new HttpClientConfiguration { Timeout = TimeSpan.FromSeconds(1) };
        _httpClientService.SetGlobalConfiguration(config);
        
        var request = new HttpRequestMessage(HttpMethod.Get, _testServer.BaseUrl + "/slow");

        // Act
        var response = await _httpClientService.SendAsync(request);

        // Assert
        response.IsSuccess.Should().BeFalse();
        response.ErrorMessage.Should().Contain("timeout");
    }

    public void Dispose()
    {
        _testServer?.Dispose();
        _httpClientService?.Dispose();
    }
}

/// <summary>
/// Simple test HTTP server for mocking HTTP responses
/// </summary>
public class TestHttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private HttpStatusCode _statusCode = HttpStatusCode.OK;
    private string _content = string.Empty;
    private TimeSpan _delay = TimeSpan.Zero;

    public string BaseUrl { get; }

    public TestHttpServer()
    {
        _listener = new HttpListener();
        BaseUrl = "http://localhost:8989/";
        _listener.Prefixes.Add(BaseUrl);
        _cancellationTokenSource = new CancellationTokenSource();
        
        _listener.Start();
        
        // Start background listener
        _ = Task.Run(HandleRequests, _cancellationTokenSource.Token);
    }

    public void SetupResponse(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    public void SetupDelay(TimeSpan delay)
    {
        _delay = delay;
    }

    private async Task HandleRequests()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested && _listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                
                // Apply delay if configured
                if (_delay > TimeSpan.Zero)
                {
                    await Task.Delay(_delay);
                }
                
                context.Response.StatusCode = (int)_statusCode;
                
                var buffer = System.Text.Encoding.UTF8.GetBytes(_content);
                context.Response.ContentLength64 = buffer.Length;
                
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                context.Response.Close();
            }
            catch (ObjectDisposedException)
            {
                // Expected when shutting down
                break;
            }
            catch (HttpListenerException)
            {
                // Expected when shutting down
                break;
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _listener?.Stop();
        _listener?.Close();
        _cancellationTokenSource?.Dispose();
    }
}

/// <summary>
/// Step 2 Test Helpers
/// </summary>
public static class Step2TestHelpers
{
    public static HttpRequestMessage CreateGetRequest(string url)
    {
        return new HttpRequestMessage(HttpMethod.Get, url);
    }

    public static HttpRequestMessage CreatePostRequest(string url, string content, string contentType = "application/json")
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(content, System.Text.Encoding.UTF8, contentType);
        return request;
    }

    public static ProxyInfo CreateTestProxy(string host = "127.0.0.1", int port = 8080)
    {
        return new ProxyInfo
        {
            Host = host,
            Port = port,
            Type = ProxyType.Http
        };
    }

    public static HttpClientConfiguration CreateTestConfiguration()
    {
        return new HttpClientConfiguration
        {
            Timeout = TimeSpan.FromSeconds(30),
            UserAgent = "TestAgent/1.0",
            FollowRedirects = true,
            MaxRedirects = 5,
            IgnoreSSLErrors = true
        };
    }
}
