using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenBullet.Core.Models;
using OpenBullet.Core.Proxies;

namespace OpenBullet.Core.Services;

/// <summary>
/// HTTP client service implementation
/// </summary>
public class HttpClientService : IHttpClientService, IDisposable
{
    private readonly ILogger<HttpClientService> _logger;
    private readonly HttpClient _defaultClient;
    private HttpClientConfiguration _globalConfiguration;
    private readonly object _lockObject = new();

    public HttpClientService(ILogger<HttpClientService> logger)
    {
        _logger = logger;
        _globalConfiguration = new HttpClientConfiguration();
        _defaultClient = CreateClient();
    }

    public async Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Models.ProxyInfo? proxy = null, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var wrapper = new HttpResponseWrapper
        {
            RequestTime = DateTime.UtcNow,
            UsedProxy = proxy
        };

        try
        {
            using var client = CreateClient(_globalConfiguration, proxy);
            
            // Log request
            _logger.LogDebug("Sending {Method} request to {Url} via {Proxy}", 
                request.Method, request.RequestUri, proxy?.ToString() ?? "direct");

            wrapper.Response = await client.SendAsync(request, cancellationToken);
            stopwatch.Stop();
            wrapper.ResponseTime = stopwatch.ElapsedMilliseconds;

            // Read content
            wrapper.ContentBytes = await wrapper.Response.Content.ReadAsByteArrayAsync(cancellationToken);
            wrapper.Content = System.Text.Encoding.UTF8.GetString(wrapper.ContentBytes);
            
            // Extract final URL (after redirects)
            wrapper.FinalUrl = wrapper.Response.RequestMessage?.RequestUri?.ToString() ?? request.RequestUri?.ToString() ?? string.Empty;
            
            // Extract headers
            foreach (var header in wrapper.Response.Headers)
            {
                wrapper.Headers[header.Key] = string.Join(", ", header.Value);
            }
            
            // Extract cookies if cookie container is used
            if (client.DefaultRequestHeaders.Contains("Cookie"))
            {
                // Extract cookies from response headers
                if (wrapper.Response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        // Simple cookie parsing - would need more robust implementation
                        var parts = cookie.Split(';')[0].Split('=');
                        if (parts.Length == 2)
                        {
                            wrapper.Cookies.Add(new Cookie(parts[0].Trim(), parts[1].Trim()));
                        }
                    }
                }
            }

            wrapper.IsSuccess = wrapper.Response.IsSuccessStatusCode;
            
            _logger.LogDebug("Request completed in {Time}ms with status {Status}", 
                wrapper.ResponseTime, wrapper.Response.StatusCode);

            return wrapper;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            stopwatch.Stop();
            wrapper.ResponseTime = stopwatch.ElapsedMilliseconds;
            wrapper.ErrorMessage = "Request timeout";
            wrapper.IsSuccess = false;
            
            _logger.LogWarning("Request timeout after {Time}ms for {Url}", 
                wrapper.ResponseTime, request.RequestUri);
            
            return wrapper;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            wrapper.ResponseTime = stopwatch.ElapsedMilliseconds;
            wrapper.ErrorMessage = ex.Message;
            wrapper.IsSuccess = false;
            
            _logger.LogError(ex, "HTTP request error for {Url}: {Error}", 
                request.RequestUri, ex.Message);
            
            return wrapper;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            wrapper.ResponseTime = stopwatch.ElapsedMilliseconds;
            wrapper.ErrorMessage = ex.Message;
            wrapper.IsSuccess = false;
            
            _logger.LogError(ex, "Unexpected error during HTTP request to {Url}", 
                request.RequestUri);
            
            return wrapper;
        }
    }

    public HttpClient CreateClient(HttpClientConfiguration? config = null, Models.ProxyInfo? proxy = null)
    {
        config ??= _globalConfiguration;
        
        var handler = new HttpClientHandler();

        // Configure proxy
        if (proxy != null)
        {
            var webProxy = new WebProxy(proxy.Host, proxy.Port);
            
            if (!string.IsNullOrEmpty(proxy.Username))
            {
                webProxy.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }
            
            handler.Proxy = webProxy;
            handler.UseProxy = true;
        }

        // Configure SSL
        if (config.IgnoreSSLErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        }

        // Configure redirects
        if (config.FollowRedirects)
        {
            handler.AllowAutoRedirect = true;
            handler.MaxAutomaticRedirections = config.MaxRedirects;
        }
        else
        {
            handler.AllowAutoRedirect = false;
        }

        // Configure cookies
        if (config.UseCookies)
        {
            handler.CookieContainer = config.CookieContainer;
            handler.UseCookies = true;
        }
        else
        {
            handler.UseCookies = false;
        }

        // Configure decompression
        if (config.AutoDecompression)
        {
            handler.AutomaticDecompression = config.DecompressionMethods;
        }

        var client = new HttpClient(handler)
        {
            Timeout = config.Timeout
        };

        // Configure HTTP version
        if (config.UseHttp2)
        {
            client.DefaultRequestVersion = HttpVersion.Version20;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
        }

        // Set default headers
        client.DefaultRequestHeaders.UserAgent.ParseAdd(config.UserAgent);
        
        foreach (var header in config.DefaultHeaders)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        return client;
    }

    public async Task<ProxyTestResult> TestProxyAsync(Models.ProxyInfo proxy, string testUrl = "https://httpbin.org/ip", TimeSpan? timeout = null)
    {
        var result = new ProxyTestResult
        {
            TestedProxy = proxy
        };

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var config = new HttpClientConfiguration
            {
                Timeout = timeout ?? TimeSpan.FromSeconds(10),
                FollowRedirects = true
            };

            using var client = CreateClient(config, proxy);
            using var request = new HttpRequestMessage(HttpMethod.Get, testUrl);
            
            var response = await client.SendAsync(request);
            stopwatch.Stop();
            
            result.ResponseTime = stopwatch.ElapsedMilliseconds;
            result.IsWorking = response.IsSuccessStatusCode;

            if (result.IsWorking)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // Try to parse IP information from httpbin response
                try
                {
                    var json = JsonDocument.Parse(content);
                    if (json.RootElement.TryGetProperty("origin", out var ipProperty))
                    {
                        result.PublicIP = ipProperty.GetString();
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors
                }

                _logger.LogDebug("Proxy {Proxy} test successful in {Time}ms, IP: {IP}", 
                    proxy, result.ResponseTime, result.PublicIP);
            }
            else
            {
                result.ErrorMessage = $"HTTP {response.StatusCode}: {response.ReasonPhrase}";
                _logger.LogWarning("Proxy {Proxy} test failed: {Error}", proxy, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.ElapsedMilliseconds;
            result.IsWorking = false;
            result.ErrorMessage = ex.Message;
            
            _logger.LogError(ex, "Proxy {Proxy} test error: {Error}", proxy, ex.Message);
        }

        return result;
    }

    public void SetGlobalConfiguration(HttpClientConfiguration configuration)
    {
        lock (_lockObject)
        {
            _globalConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger.LogDebug("Global HTTP configuration updated");
        }
    }

    // Enhanced proxy overloads
    public async Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Proxies.ProxyInfo? proxy, CancellationToken cancellationToken = default)
    {
        return await SendAsync(request, proxy?.ToBasicProxy(), cancellationToken);
    }

    public async Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Models.ProxyInfo? proxy, HttpClientConfiguration config, CancellationToken cancellationToken = default)
    {
        var currentConfig = _globalConfiguration;
        SetGlobalConfiguration(config);
        try
        {
            return await SendAsync(request, proxy, cancellationToken);
        }
        finally
        {
            SetGlobalConfiguration(currentConfig);
        }
    }

    public async Task<HttpResponseWrapper> SendAsync(HttpRequestMessage request, Proxies.ProxyInfo? proxy, HttpClientConfiguration config, CancellationToken cancellationToken = default)
    {
        return await SendAsync(request, proxy?.ToBasicProxy(), config, cancellationToken);
    }

    public HttpClient CreateClient(HttpClientConfiguration? config, Proxies.ProxyInfo? proxy)
    {
        return CreateClient(config, proxy?.ToBasicProxy());
    }

    public async Task<Proxies.ProxyTestResult> TestProxyAsync(Proxies.ProxyInfo proxy, ProxyTestConfiguration? config = null, CancellationToken cancellationToken = default)
    {
        config ??= new ProxyTestConfiguration();
        var result = new Proxies.ProxyTestResult { Proxy = proxy };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogTrace("Testing enhanced proxy: {ProxyAddress}", proxy.Address);

            var httpConfig = new HttpClientConfiguration
            {
                Timeout = config.Timeout,
                UserAgent = config.UserAgent,
                IgnoreSslErrors = !config.ValidateSsl
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, config.TestUrl);
            foreach (var header in config.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await SendAsync(requestMessage, proxy, httpConfig, cancellationToken);

            stopwatch.Stop();
            result.ResponseTime = TimeSpan.FromMilliseconds(response.ResponseTime);
            result.ResponseCode = response.Response?.StatusCode is not null ? (int)response.Response.StatusCode : 0;
            result.ResponseContent = response.Content;
            result.Success = response.IsSuccess;

            if (!string.IsNullOrEmpty(config.ExpectedContent) && 
                (response.Content?.Contains(config.ExpectedContent) != true))
            {
                result.Success = false;
                result.ErrorMessage = $"Response does not contain expected content: {config.ExpectedContent}";
            }

            result.DetermineHealth();

            _logger.LogTrace("Enhanced proxy test completed: {ProxyAddress} - Success: {Success}, ResponseTime: {ResponseTime}ms", 
                proxy.Address, result.Success, result.ResponseTime.TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ResponseTime = stopwatch.Elapsed;
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.Exception = ex;
            result.DeterminedHealth = ProxyHealth.Dead;

            _logger.LogTrace(ex, "Enhanced proxy test failed: {ProxyAddress}", proxy.Address);

            return result;
        }
    }

    public void Dispose()
    {
        _defaultClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// HTTP client service extensions
/// </summary>
public static class HttpClientServiceExtensions
{
    /// <summary>
    /// Sends a GET request
    /// </summary>
    public static async Task<HttpResponseWrapper> GetAsync(this IHttpClientService service, string url, Models.ProxyInfo? proxy = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await service.SendAsync(request, proxy, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with content
    /// </summary>
    public static async Task<HttpResponseWrapper> PostAsync(this IHttpClientService service, string url, HttpContent content, Models.ProxyInfo? proxy = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        return await service.SendAsync(request, proxy, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with form data
    /// </summary>
    public static async Task<HttpResponseWrapper> PostFormAsync(this IHttpClientService service, string url, Dictionary<string, string> formData, Models.ProxyInfo? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(formData);
        return await service.PostAsync(url, content, proxy, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with JSON content
    /// </summary>
    public static async Task<HttpResponseWrapper> PostJsonAsync(this IHttpClientService service, string url, object data, Models.ProxyInfo? proxy = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        return await service.PostAsync(url, content, proxy, cancellationToken);
    }
}
