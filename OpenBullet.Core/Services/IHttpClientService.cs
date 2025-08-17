using System.Net;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;

namespace OpenBullet.Core.Services;

/// <summary>
/// Interface for HTTP client operations
/// </summary>
public interface IHttpClientService
{
    /// <summary>
    /// Sends an HTTP request with optional proxy support
    /// </summary>
    Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Models.ProxyInfo? proxy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an HTTP request with optional enhanced proxy support
    /// </summary>
    Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Proxies.ProxyInfo? proxy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an HTTP request with configuration and proxy support
    /// </summary>
    Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Models.ProxyInfo? proxy, HttpClientConfiguration config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an HTTP request with configuration and enhanced proxy support
    /// </summary>
    Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Proxies.ProxyInfo? proxy, HttpClientConfiguration config, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a configured HTTP client
    /// </summary>
    HttpClient CreateClient(HttpClientConfiguration? config = null, Models.ProxyInfo? proxy = null);

    /// <summary>
    /// Creates a configured HTTP client with enhanced proxy support
    /// </summary>
    HttpClient CreateClient(HttpClientConfiguration? config, Proxies.ProxyInfo? proxy);

    /// <summary>
    /// Tests a proxy for connectivity
    /// </summary>
    Task<ProxyTestResult> TestProxyAsync(Models.ProxyInfo proxy, string testUrl = "https://httpbin.org/ip", TimeSpan? timeout = null);

    /// <summary>
    /// Tests an enhanced proxy for connectivity
    /// </summary>
    Task<Proxies.ProxyTestResult> TestProxyAsync(Proxies.ProxyInfo proxy, ProxyTestConfiguration? config = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets global HTTP configuration
    /// </summary>
    void SetGlobalConfiguration(HttpClientConfiguration configuration);
}

/// <summary>
/// HTTP response wrapper with additional metadata
/// </summary>
public class HttpResponseWrapper : IDisposable
{
    public HttpResponseMessage Response { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public byte[] ContentBytes { get; set; } = Array.Empty<byte>();
    public long ResponseTime { get; set; }
    public Models.ProxyInfo? UsedProxy { get; set; }
    public DateTime RequestTime { get; set; }
    public string FinalUrl { get; set; } = string.Empty;
    public CookieCollection Cookies { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Additional properties for compatibility
    public int StatusCode => Response?.StatusCode != null ? (int)Response.StatusCode : 0;
    public bool IsSuccessStatusCode => Response?.IsSuccessStatusCode ?? false;
    public string? RequestUri => Response?.RequestMessage?.RequestUri?.ToString();
    public Exception? Exception { get; set; }

    public void Dispose()
    {
        Response?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// HTTP client configuration
/// </summary>
public class HttpClientConfiguration
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    public int MaxRedirects { get; set; } = 8;
    public bool FollowRedirects { get; set; } = true;
    public bool IgnoreSSLErrors { get; set; } = false;
    public string UserAgent { get; set; } = "OpenBullet/2.0";
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
    public CookieContainer CookieContainer { get; set; } = new();
    public bool UseCookies { get; set; } = true;
    public bool AutoDecompression { get; set; } = true;
    public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;
    public int MaxConnectionsPerServer { get; set; } = 50;
    public bool UseHttp2 { get; set; } = true;
    
    // Compatibility properties
    public bool IgnoreSslErrors { get => IgnoreSSLErrors; set => IgnoreSSLErrors = value; }
    public bool AllowAutoRedirect { get => FollowRedirects; set => FollowRedirects = value; }
}

/// <summary>
/// Proxy test result
/// </summary>
public class ProxyTestResult
{
    public bool IsWorking { get; set; }
    public long ResponseTime { get; set; }
    public string? ErrorMessage { get; set; }
    public string? PublicIP { get; set; }
    public string? Country { get; set; }
    public Models.ProxyInfo TestedProxy { get; set; } = null!;
}
