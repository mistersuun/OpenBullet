# Extreme.Net.dll Documentation

## Overview
Extreme.Net is a comprehensive HTTP client library for .NET that provides advanced networking capabilities including proxy support, SSL/TLS handling, and extensive request/response management.

## Purpose in OpenBullet
- Execute HTTP/HTTPS requests with full control
- Manage proxy connections (HTTP, SOCKS4, SOCKS5)
- Handle cookies and sessions
- Support for various authentication methods

## Key Components

### Core Classes

#### `HttpRequest`
- **Purpose**: Main HTTP client for making web requests
- **Key Properties**:
  - `Proxy` - Set proxy for requests
  - `Cookies` - Cookie container management
  - `UserAgent` - Custom user agent string
  - `KeepAlive` - Connection persistence
  - `AllowAutoRedirect` - Redirect handling
  - `ConnectTimeout` - Connection timeout settings
  - `ReadWriteTimeout` - Data transfer timeout
- **Key Methods**:
  - `Get()` - HTTP GET request
  - `Post()` - HTTP POST request
  - `Raw()` - Send raw HTTP request
  - `AddHeader()` - Add custom headers

#### `HttpResponse`
- **Purpose**: Represents HTTP response data
- **Key Properties**:
  - `StatusCode` - HTTP status code
  - `ContentType` - Response content type
  - `ContentLength` - Response size
  - `Cookies` - Response cookies
  - `Headers` - Response headers
  - `Body` - Response body content
- **Methods**:
  - `ToString()` - Get response as string
  - `ToBytes()` - Get response as byte array
  - `Save()` - Save response to file

### Proxy Support

#### `ProxyClient` (Abstract Base)
- **Purpose**: Base class for all proxy implementations
- **Derived Classes**:
  - `HttpProxyClient` - HTTP/HTTPS proxy
  - `Socks4ProxyClient` - SOCKS4 proxy
  - `Socks4AProxyClient` - SOCKS4A proxy
  - `Socks5ProxyClient` - SOCKS5 proxy

#### Proxy Configuration
```csharp
// HTTP Proxy
var httpProxy = new HttpProxyClient("proxy.server.com", 8080);
httpProxy.Username = "user";
httpProxy.Password = "pass";

// SOCKS5 Proxy
var socks5Proxy = new Socks5ProxyClient("socks.server.com", 1080);
socks5Proxy.Username = "user";
socks5Proxy.Password = "pass";

request.Proxy = httpProxy;
```

### Content Types

#### `StringContent`
- **Purpose**: String-based request content
- **Usage**: Text data, JSON, XML
```csharp
var content = new StringContent("{"key":"value"}", Encoding.UTF8, "application/json");
```

#### `BytesContent`
- **Purpose**: Binary request content
- **Usage**: File uploads, binary data

#### `MultipartContent`
- **Purpose**: Multipart form data
- **Usage**: File uploads with form fields
```csharp
var multipart = new MultipartContent();
multipart.Add(new StringContent("value"), "field");
multipart.Add(new FileContent("file.jpg"), "upload");
```

#### `FormUrlEncodedContent`
- **Purpose**: URL-encoded form data
- **Usage**: Standard form submissions
```csharp
var formData = new FormUrlEncodedContent(new Dictionary<string, string> {
    ["username"] = "user",
    ["password"] = "pass"
});
```

## Implementation Examples

### Basic GET Request
```csharp
using (var request = new HttpRequest())
{
    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
    request.AddHeader("Accept", "text/html,application/xhtml+xml");
    
    var response = request.Get("https://example.com");
    string html = response.ToString();
}
```

### POST with Form Data
```csharp
using (var request = new HttpRequest())
{
    request.UserAgent = HttpHelper.RandomUserAgent();
    request.Referer = "https://example.com/login";
    
    var postData = new RequestParams();
    postData["username"] = "testuser";
    postData["password"] = "testpass";
    postData["remember"] = "1";
    
    var response = request.Post("https://example.com/login", postData);
    
    if (response.StatusCode == HttpStatusCode.OK)
    {
        // Handle successful login
    }
}
```

### Session Management
```csharp
using (var request = new HttpRequest())
{
    request.Cookies = new CookieStorage();
    
    // Initial request to get session
    request.Get("https://example.com");
    
    // Subsequent request with session
    var response = request.Get("https://example.com/account");
    
    // Cookies are automatically managed
}
```

### Proxy Rotation
```csharp
public class ProxyRotator
{
    private List<ProxyClient> proxies;
    private int currentIndex = 0;
    
    public ProxyClient GetNext()
    {
        var proxy = proxies[currentIndex];
        currentIndex = (currentIndex + 1) % proxies.Count;
        return proxy;
    }
}

// Usage
request.Proxy = proxyRotator.GetNext();
```

## Advanced Features

### Custom Headers
```csharp
request.AddHeader("X-Requested-With", "XMLHttpRequest");
request.AddHeader("X-CSRF-Token", csrfToken);
request.AddHeader("Accept-Language", "en-US,en;q=0.9");
```

### SSL/TLS Configuration
```csharp
request.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
request.ServerCertificateValidation = (sender, cert, chain, errors) => true; // Ignore cert errors
```

### Timeout Management
```csharp
request.ConnectTimeout = 10000; // 10 seconds
request.ReadWriteTimeout = 30000; // 30 seconds
request.KeepAliveTimeout = 60000; // 60 seconds
```

### Download Progress
```csharp
request.DownloadProgressChanged += (sender, e) =>
{
    Console.WriteLine($"Downloaded: {e.BytesReceived}/{e.TotalBytesToReceive}");
};
```

## Error Handling

### Common Exceptions
- `HttpException` - HTTP-related errors
- `ProxyException` - Proxy connection failures
- `NetException` - Network-level errors

### Error Handling Pattern
```csharp
try
{
    var response = request.Get(url);
}
catch (HttpException ex) when (ex.Status == HttpExceptionStatus.ConnectFailure)
{
    // Handle connection failure
}
catch (ProxyException ex)
{
    // Handle proxy error
}
catch (Exception ex)
{
    // Handle other errors
}
```

## Performance Optimization

### Connection Pooling
```csharp
request.KeepAlive = true;
request.MaximumConnectionsLimit = 10;
request.KeepAliveTimeout = 300000; // 5 minutes
```

### Response Streaming
```csharp
using (var response = request.Get(url))
using (var stream = response.ToStream())
{
    // Process stream without loading entire response
}
```

## Integration with OpenBullet

### Bot Configuration
```csharp
public class BotHttpClient
{
    private HttpRequest request;
    
    public BotHttpClient(BotData data)
    {
        request = new HttpRequest();
        request.Proxy = data.Proxy;
        request.Cookies = data.Cookies;
        request.UserAgent = data.UserAgent;
    }
}
```

### Response Analysis
```csharp
var response = request.Post(url, data);

// Check for success patterns
if (response.ToString().Contains("Welcome"))
{
    return BotStatus.Success;
}
else if (response.StatusCode == HttpStatusCode.Forbidden)
{
    return BotStatus.Ban;
}
else
{
    return BotStatus.Fail;
}
```

## Best Practices
1. Always dispose HttpRequest objects
2. Reuse connections with KeepAlive
3. Implement proper timeout values
4. Handle proxy failures gracefully
5. Rotate user agents and proxies
6. Implement retry logic for transient failures

## Limitations
- No built-in JavaScript execution
- Limited WebSocket support
- No HTTP/2 support in older versions
- Manual cookie parsing sometimes required

## Dependencies
- .NET Framework 4.0+
- System.Net.Http (for some features)
- No external dependencies

## Security Considerations
- Validate SSL certificates in production
- Sanitize user input in requests
- Protect proxy credentials
- Implement rate limiting
- Log security-relevant events