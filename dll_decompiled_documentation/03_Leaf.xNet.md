# Leaf.xNet.dll Documentation

## Overview
Leaf.xNet is an enhanced HTTP client library that extends and improves upon Extreme.Net, providing additional features specifically designed for web automation, including CAPTCHA solving integration and advanced session management.

## Purpose in OpenBullet
- Advanced HTTP request handling with enhanced features
- Built-in CAPTCHA service integration
- Improved cookie and session management
- Better proxy handling and rotation
- Enhanced response processing

## Key Components

### Core Classes

#### `HttpRequest` (Enhanced)
- **Purpose**: Advanced HTTP client with automation-specific features
- **Enhanced Properties**:
  - `CaptchaSolver` - Integrated CAPTCHA solving
  - `ManualMode` - Fine-grained request control
  - `MiddleHeaders` - Headers sent between requests
  - `EnableMiddleHeaders` - Toggle middle headers
  - `Reconnect` - Force new connection
  - `Http2` - HTTP/2 support
- **Additional Methods**:
  - `AddField()` - Add form field
  - `AddFile()` - Add file upload
  - `ClearAllHeaders()` - Reset headers
  - `ParseCookies()` - Advanced cookie parsing

#### `HttpResponse` (Enhanced)
- **Purpose**: Enhanced response handling
- **Enhanced Features**:
  - Better encoding detection
  - Improved compression handling
  - Chunked transfer decoding
  - Stream processing support
- **New Properties**:
  - `LoadedContent` - Lazy-loaded content
  - `MessageBody` - Raw response body
  - `HasError` - Error detection

### CAPTCHA Integration

#### CAPTCHA Solver Support
```csharp
public interface ICaptchaSolver
{
    string SolveRecaptchaV2(string siteKey, string pageUrl);
    string SolveRecaptchaV3(string siteKey, string pageUrl, string action);
    string SolveHCaptcha(string siteKey, string pageUrl);
    string SolveImageCaptcha(byte[] imageData);
}
```

#### Implementation Example
```csharp
request.CaptchaSolver = new TwoCaptchaSolver("API_KEY");

// Automatic CAPTCHA solving
var response = request.Post(url, data); // CAPTCHA solved automatically if detected
```

### Advanced Cookie Management

#### `CookieStorage` (Enhanced)
- **Features**:
  - Domain-based cookie storage
  - Automatic expiration handling
  - Secure cookie support
  - HttpOnly flag support
- **Methods**:
  - `GetCookies()` - Get cookies for domain
  - `SetCookies()` - Set multiple cookies
  - `Clear()` - Clear all cookies
  - `Export()` - Export cookies to string
  - `Import()` - Import cookies from string

### Enhanced Proxy Features

#### Proxy Chain Support
```csharp
var proxyChain = new ProxyChain();
proxyChain.Add(new Socks5ProxyClient("proxy1.com", 1080));
proxyChain.Add(new HttpProxyClient("proxy2.com", 8080));
request.ProxyChain = proxyChain;
```

#### Smart Proxy Rotation
```csharp
public class SmartProxyRotator
{
    private Dictionary<string, int> proxyFailures;
    private List<ProxyClient> proxies;
    
    public ProxyClient GetBestProxy()
    {
        return proxies
            .OrderBy(p => proxyFailures.GetValueOrDefault(p.ToString(), 0))
            .First();
    }
    
    public void ReportFailure(ProxyClient proxy)
    {
        var key = proxy.ToString();
        proxyFailures[key] = proxyFailures.GetValueOrDefault(key, 0) + 1;
    }
}
```

## Implementation Examples

### Advanced Form Submission
```csharp
using (var request = new HttpRequest())
{
    request.UserAgent = HttpHelper.RandomUserAgent();
    request.EnableMiddleHeaders = true;
    
    // Get form page
    var formPage = request.Get("https://example.com/form");
    var token = ExtractToken(formPage.ToString());
    
    // Prepare multipart form
    var multipart = new MultipartContent();
    multipart.Add(new StringContent(token), "csrf_token");
    multipart.Add(new StringContent("John Doe"), "name");
    multipart.Add(new FileContent("document.pdf"), "file", "document.pdf");
    
    // Submit with automatic CAPTCHA solving
    request.CaptchaSolver = captchaSolver;
    var response = request.Post("https://example.com/submit", multipart);
}
```

### Session Persistence
```csharp
public class SessionManager
{
    private HttpRequest request;
    private CookieStorage cookies;
    
    public SessionManager()
    {
        request = new HttpRequest();
        cookies = new CookieStorage();
        request.Cookies = cookies;
    }
    
    public void SaveSession(string file)
    {
        File.WriteAllText(file, cookies.Export());
    }
    
    public void LoadSession(string file)
    {
        cookies.Import(File.ReadAllText(file));
    }
    
    public bool IsSessionValid()
    {
        var response = request.Get("https://example.com/account");
        return !response.ToString().Contains("login");
    }
}
```

### Manual Mode Request Control
```csharp
request.ManualMode = true;

// Build custom request
var rawRequest = @"GET /api/data HTTP/1.1
Host: api.example.com
User-Agent: CustomBot/1.0
X-Custom-Header: Value
Accept: application/json

";

var response = request.Raw(HttpMethod.GET, "https://api.example.com/api/data", 
    new StringContent(rawRequest));
```

### Response Stream Processing
```csharp
using (var response = request.Get("https://example.com/large-file"))
using (var stream = response.ToStream())
using (var reader = new StreamReader(stream))
{
    string line;
    while ((line = reader.ReadLine()) != null)
    {
        // Process line by line
        ProcessLine(line);
    }
}
```

## Advanced Features

### HTTP/2 Support
```csharp
request.Http2 = true;
request.EnableHttp2Push = true;
request.Http2MaxConcurrentStreams = 100;
```

### WebSocket Support
```csharp
var ws = request.ConnectWebSocket("wss://example.com/socket");
ws.OnMessage += (sender, e) => Console.WriteLine(e.Data);
ws.Send("Hello Server");
```

### Request Timing
```csharp
var stopwatch = Stopwatch.StartNew();
var response = request.Get(url);
stopwatch.Stop();

Console.WriteLine($"Request took: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"DNS Lookup: {request.DnsLookupTime}ms");
Console.WriteLine($"Connect Time: {request.ConnectTime}ms");
Console.WriteLine($"SSL Handshake: {request.SslHandshakeTime}ms");
```

### Content Decompression
```csharp
request.EnableAutoDecompression = true;
request.DecompressionMethods = DecompressionMethods.GZip | 
                                DecompressionMethods.Deflate | 
                                DecompressionMethods.Brotli;
```

## CAPTCHA Solving Integration

### 2Captcha Integration
```csharp
public class TwoCaptchaSolver : ICaptchaSolver
{
    private string apiKey;
    
    public string SolveRecaptchaV2(string siteKey, string pageUrl)
    {
        // Send CAPTCHA to 2Captcha
        var taskId = SendCaptcha(siteKey, pageUrl);
        
        // Poll for result
        return WaitForResult(taskId);
    }
}
```

### Anti-Captcha Integration
```csharp
public class AntiCaptchaSolver : ICaptchaSolver
{
    // Similar implementation for Anti-Captcha service
}
```

## Error Handling

### Enhanced Exception Types
- `HttpException` - HTTP protocol errors
- `ProxyException` - Proxy-related errors
- `CaptchaException` - CAPTCHA solving failures
- `ConnectionException` - Network connection issues

### Retry Logic
```csharp
public async Task<HttpResponse> RequestWithRetry(string url, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await request.GetAsync(url);
        }
        catch (HttpException ex) when (ex.Status == HttpExceptionStatus.ConnectFailure)
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay(1000 * (i + 1)); // Exponential backoff
        }
    }
    throw new Exception("Max retries exceeded");
}
```

## Performance Optimizations

### Connection Pooling
```csharp
request.KeepAlive = true;
request.MaximumConnectionsLimit = 20;
request.ServicePoint.ConnectionLeaseTimeout = 60000;
```

### Pipelining
```csharp
request.Pipelined = true;
request.MaxPipelinedRequests = 10;
```

### DNS Caching
```csharp
request.DnsCacheTimeout = 300; // 5 minutes
request.UseCustomDnsResolver = true;
request.DnsServers = new[] { "8.8.8.8", "8.8.4.4" };
```

## Integration with OpenBullet

### Block Integration
```csharp
public class LeafXNetRequestBlock : BlockBase
{
    public override BlockResult Execute(BotData data)
    {
        using (var request = new HttpRequest())
        {
            ConfigureRequest(request, data);
            
            var response = ExecuteRequest(request, data);
            
            data.ResponseCode = response.StatusCode;
            data.ResponseSource = response.ToString();
            
            return AnalyzeResponse(response, data);
        }
    }
}
```

## Best Practices
1. Reuse HttpRequest instances for session persistence
2. Implement CAPTCHA solving only when necessary
3. Use streaming for large responses
4. Enable compression for bandwidth optimization
5. Implement smart proxy rotation
6. Cache DNS results when possible
7. Use connection pooling for performance

## Dependencies
- .NET Framework 4.5+ or .NET Core 2.0+
- System.Net.Http (optional)
- Newtonsoft.Json (for some features)

## Security Considerations
- Validate SSL certificates in production
- Protect CAPTCHA service API keys
- Implement request rate limiting
- Log authentication attempts
- Sanitize user input in requests
- Use secure cookie storage