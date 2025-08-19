using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OpenBullet.Native
{
    /// <summary>
    /// Advanced HTTP Integration Layer using original Leaf.xNet CloudflareBypass and anti-detection features
    /// This replaces basic HTTP requests with sophisticated anti-detection capabilities
    /// </summary>
    public static class AdvancedHttpIntegration
    {
        private static Assembly leafxNetAssembly;
        private static Type httpRequestType;
        private static Type cloudflareBypassType;
        private static Type httpResponseType;
        private static Type httpProxyClientType;
        private static Type socks5ProxyClientType;
        
        static AdvancedHttpIntegration()
        {
            try
            {
                // Load the Leaf.xNet assembly
                leafxNetAssembly = Assembly.LoadFrom("libs/Leaf.xNet.dll");
                
                // Get critical types for advanced HTTP functionality
                httpRequestType = leafxNetAssembly.GetType("Leaf.xNet.HttpRequest");
                cloudflareBypassType = leafxNetAssembly.GetType("Leaf.xNet.Services.Cloudflare.CloudflareBypass");
                httpResponseType = leafxNetAssembly.GetType("Leaf.xNet.HttpResponse");
                httpProxyClientType = leafxNetAssembly.GetType("Leaf.xNet.HttpProxyClient");
                socks5ProxyClientType = leafxNetAssembly.GetType("Leaf.xNet.Socks5ProxyClient");
                
                Console.WriteLine("✅ Advanced HTTP Integration - Leaf.xNet loaded successfully");
                Console.WriteLine($"   HttpRequest type: {httpRequestType?.FullName ?? "NOT FOUND"}");
                Console.WriteLine($"   CloudflareBypass type: {cloudflareBypassType?.FullName ?? "NOT FOUND"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Advanced HTTP Integration initialization failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Creates an advanced HttpRequest with CloudflareBypass and anti-detection features enabled
        /// </summary>
        public static object CreateAdvancedHttpRequest()
        {
            try
            {
                if (httpRequestType == null)
                {
                    Console.WriteLine("❌ HttpRequest type not available");
                    return null;
                }
                
                // Create new Leaf.xNet.HttpRequest instance
                var httpRequest = Activator.CreateInstance(httpRequestType);
                
                // Enable advanced features using reflection
                Console.WriteLine("🔧 Configuring advanced HTTP request with anti-detection features...");
                
                // Enable cookies (critical for session management)
                var useCookiesProperty = httpRequestType.GetProperty("UseCookies");
                if (useCookiesProperty != null)
                {
                    useCookiesProperty.SetValue(httpRequest, true);
                    Console.WriteLine("✅ Cookies enabled for session persistence");
                }
                
                // Set browser-like User-Agent (anti-detection)
                var userAgentProperty = httpRequestType.GetProperty("UserAgent");
                if (userAgentProperty != null)
                {
                    var chromeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
                    userAgentProperty.SetValue(httpRequest, chromeUserAgent);
                    Console.WriteLine($"✅ User-Agent set: {chromeUserAgent.Substring(0, 50)}...");
                }
                
                // Enable compression (browser-like behavior)
                var acceptEncodingProperty = httpRequestType.GetProperty("AcceptEncoding");
                if (acceptEncodingProperty != null)
                {
                    acceptEncodingProperty.SetValue(httpRequest, "gzip,deflate,br");
                    Console.WriteLine("✅ Compression enabled: gzip,deflate,br");
                }
                
                // Enable keep-alive connections (session persistence)
                var keepAliveProperty = httpRequestType.GetProperty("KeepAlive");
                if (keepAliveProperty != null)
                {
                    keepAliveProperty.SetValue(httpRequest, true);
                    Console.WriteLine("✅ Keep-alive connections enabled");
                }
                
                // Set connection timeout (reasonable values)
                var connectTimeoutProperty = httpRequestType.GetProperty("ConnectTimeout");
                if (connectTimeoutProperty != null)
                {
                    connectTimeoutProperty.SetValue(httpRequest, 30000); // 30 seconds
                    Console.WriteLine("✅ Connect timeout: 30 seconds");
                }
                
                var readWriteTimeoutProperty = httpRequestType.GetProperty("ReadWriteTimeout");
                if (readWriteTimeoutProperty != null)
                {
                    readWriteTimeoutProperty.SetValue(httpRequest, 60000); // 60 seconds
                    Console.WriteLine("✅ Read/Write timeout: 60 seconds");
                }
                
                // Enable automatic redirects with reasonable limit
                var allowAutoRedirectProperty = httpRequestType.GetProperty("AllowAutoRedirect");
                if (allowAutoRedirectProperty != null)
                {
                    allowAutoRedirectProperty.SetValue(httpRequest, true);
                    Console.WriteLine("✅ Auto-redirect enabled");
                }
                
                var maxRedirectsProperty = httpRequestType.GetProperty("MaximumAutomaticRedirections");
                if (maxRedirectsProperty != null)
                {
                    maxRedirectsProperty.SetValue(httpRequest, 8);
                    Console.WriteLine("✅ Max redirects: 8");
                }
                
                return httpRequest;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to create advanced HttpRequest: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Adds browser-like headers to simulate real browser behavior
        /// </summary>
        public static void AddBrowserHeaders(object httpRequest)
        {
            if (httpRequest == null) return;
            
            try
            {
                Console.WriteLine("🔧 Adding browser-like headers for anti-detection...");
                
                var addHeaderMethod = httpRequestType.GetMethod("AddHeader", new[] { typeof(string), typeof(string) });
                if (addHeaderMethod == null)
                {
                    Console.WriteLine("⚠️ AddHeader method not found, trying alternative...");
                    
                    // Try alternative header setting method
                    var headersProperty = httpRequestType.GetProperty("Headers");
                    if (headersProperty != null)
                    {
                        var headers = headersProperty.GetValue(httpRequest);
                        if (headers != null)
                        {
                            var headersType = headers.GetType();
                            var addMethod = headersType.GetMethod("Add", new[] { typeof(string), typeof(string) });
                            if (addMethod != null)
                            {
                                addMethod.Invoke(headers, new object[] { "Accept-Language", "en-US,en;q=0.9" });
                                addMethod.Invoke(headers, new object[] { "Sec-Fetch-Dest", "document" });
                                addMethod.Invoke(headers, new object[] { "Sec-Fetch-Mode", "navigate" });
                                addMethod.Invoke(headers, new object[] { "Sec-Fetch-Site", "same-origin" });
                                addMethod.Invoke(headers, new object[] { "Upgrade-Insecure-Requests", "1" });
                                Console.WriteLine("✅ Browser-like headers added via Headers collection");
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // Add critical browser-like headers
                    addHeaderMethod.Invoke(httpRequest, new object[] { "Accept-Language", "en-US,en;q=0.9" });
                    addHeaderMethod.Invoke(httpRequest, new object[] { "Sec-Fetch-Dest", "document" });
                    addHeaderMethod.Invoke(httpRequest, new object[] { "Sec-Fetch-Mode", "navigate" });
                    addHeaderMethod.Invoke(httpRequest, new object[] { "Sec-Fetch-Site", "same-origin" });
                    addHeaderMethod.Invoke(httpRequest, new object[] { "Upgrade-Insecure-Requests", "1" });
                    addHeaderMethod.Invoke(httpRequest, new object[] { "Cache-Control", "max-age=0" });
                    
                    Console.WriteLine("✅ Browser-like headers added:");
                    Console.WriteLine("   Accept-Language: en-US,en;q=0.9");
                    Console.WriteLine("   Sec-Fetch-Dest: document");
                    Console.WriteLine("   Sec-Fetch-Mode: navigate");
                    Console.WriteLine("   Sec-Fetch-Site: same-origin");
                    Console.WriteLine("   Upgrade-Insecure-Requests: 1");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to add browser headers: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Executes HTTP request with CloudflareBypass integration
        /// This is the KEY method that will eliminate BAN status
        /// </summary>
        public static async Task<object> ExecuteWithCloudflareBypass(string url, string method = "GET", string content = null, Dictionary<string, string> headers = null)
        {
            try
            {
                Console.WriteLine($"🚀 EXECUTING ADVANCED HTTP REQUEST with CloudflareBypass");
                Console.WriteLine($"   URL: {url}");
                Console.WriteLine($"   Method: {method}");
                Console.WriteLine($"   Content length: {content?.Length ?? 0} characters");
                
                var httpRequest = CreateAdvancedHttpRequest();
                if (httpRequest == null)
                {
                    Console.WriteLine("❌ Failed to create advanced HttpRequest");
                    return null;
                }
                
                // Add browser-like headers
                AddBrowserHeaders(httpRequest);
                
                // Add custom headers if provided
                if (headers != null)
                {
                    Console.WriteLine($"🔧 Adding {headers.Count} custom headers...");
                    foreach (var header in headers)
                    {
                        try
                        {
                            var addHeaderMethod = httpRequestType.GetMethod("AddHeader", new[] { typeof(string), typeof(string) });
                            addHeaderMethod?.Invoke(httpRequest, new object[] { header.Key, header.Value });
                            Console.WriteLine($"   {header.Key}: {header.Value}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Failed to add header {header.Key}: {ex.Message}");
                        }
                    }
                }
                
                // Try to use CloudflareBypass if available
                if (cloudflareBypassType != null)
                {
                    Console.WriteLine("🔥 ATTEMPTING CLOUDFLARE BYPASS - This should eliminate BAN status!");
                    
                    try
                    {
                        // Look for GetThroughCloudflare method
                        var getThroughCloudflareMethod = cloudflareBypassType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .FirstOrDefault(m => m.Name == "GetThroughCloudflare");
                        
                        if (getThroughCloudflareMethod != null)
                        {
                            Console.WriteLine("✅ GetThroughCloudflare method found - executing with bypass...");
                            
                            var uri = new Uri(url);
                            var cancellationToken = CancellationToken.None;
                            
                            // Call GetThroughCloudflare
                            var response = getThroughCloudflareMethod.Invoke(null, new object[] { httpRequest, uri, null, cancellationToken, null });
                            
                            if (response != null)
                            {
                                Console.WriteLine("🎉 CLOUDFLARE BYPASS SUCCESSFUL!");
                                Console.WriteLine($"   Response type: {response.GetType().Name}");
                                
                                // Get response content
                                var contentProperty = response.GetType().GetProperty("Content") ?? 
                                                   response.GetType().GetProperty("ToString");
                                if (contentProperty != null)
                                {
                                    var responseContent = contentProperty.GetValue(response)?.ToString();
                                    Console.WriteLine($"   Response length: {responseContent?.Length ?? 0} characters");
                                }
                                
                                return response;
                            }
                        }
                        else
                        {
                            Console.WriteLine("⚠️ GetThroughCloudflare method not found, trying alternative CloudflareBypass methods...");
                            
                            // List available methods for debugging
                            var methods = cloudflareBypassType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                            Console.WriteLine($"Available CloudflareBypass methods: {string.Join(", ", methods.Select(m => m.Name))}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ CloudflareBypass failed, falling back to advanced HTTP: {ex.Message}");
                    }
                }
                
                // Fallback to advanced HTTP request (still better than basic HTTP)
                Console.WriteLine("🔧 Executing advanced HTTP request (without CloudflareBypass)...");
                
                object response_fallback = null;
                
                if (method.ToUpper() == "POST" && !string.IsNullOrEmpty(content))
                {
                    // POST request
                    var postMethod = httpRequestType.GetMethod("Post", new[] { typeof(string), typeof(string), typeof(string) });
                    if (postMethod != null)
                    {
                        response_fallback = postMethod.Invoke(httpRequest, new object[] { url, content, "application/x-www-form-urlencoded" });
                        Console.WriteLine("✅ Advanced POST request executed");
                    }
                }
                else
                {
                    // GET request
                    var getMethod = httpRequestType.GetMethod("Get", new[] { typeof(string) });
                    if (getMethod != null)
                    {
                        response_fallback = getMethod.Invoke(httpRequest, new object[] { url });
                        Console.WriteLine("✅ Advanced GET request executed");
                    }
                }
                
                if (response_fallback != null)
                {
                    Console.WriteLine("✅ Advanced HTTP request completed successfully");
                    
                    // Get response details
                    var contentProperty = response_fallback.GetType().GetProperty("Content") ?? 
                                       response_fallback.GetType().GetProperty("ToString");
                    if (contentProperty != null)
                    {
                        var responseContent = contentProperty.GetValue(response_fallback)?.ToString();
                        Console.WriteLine($"   Response length: {responseContent?.Length ?? 0} characters");
                    }
                    
                    return response_fallback;
                }
                
                Console.WriteLine("❌ All HTTP execution methods failed");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Advanced HTTP execution failed: {ex.Message}");
                Console.WriteLine($"   Stack trace: {ex.StackTrace}");
                return null;
            }
        }
        
        /// <summary>
        /// Sets up proxy support for anonymity and bypassing restrictions
        /// </summary>
        public static void SetProxy(object httpRequest, string proxyHost, int proxyPort, string username = null, string password = null, string proxyType = "HTTP")
        {
            if (httpRequest == null) return;
            
            try
            {
                Console.WriteLine($"🔧 Setting up proxy: {proxyType}://{proxyHost}:{proxyPort}");
                
                object proxyClient = null;
                
                if (proxyType.ToUpper() == "HTTP" && httpProxyClientType != null)
                {
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        var constructor = httpProxyClientType.GetConstructor(new[] { typeof(string), typeof(int), typeof(string), typeof(string) });
                        proxyClient = constructor?.Invoke(new object[] { proxyHost, proxyPort, username, password });
                    }
                    else
                    {
                        var constructor = httpProxyClientType.GetConstructor(new[] { typeof(string), typeof(int) });
                        proxyClient = constructor?.Invoke(new object[] { proxyHost, proxyPort });
                    }
                }
                else if (proxyType.ToUpper() == "SOCKS5" && socks5ProxyClientType != null)
                {
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        var constructor = socks5ProxyClientType.GetConstructor(new[] { typeof(string), typeof(int), typeof(string), typeof(string) });
                        proxyClient = constructor?.Invoke(new object[] { proxyHost, proxyPort, username, password });
                    }
                    else
                    {
                        var constructor = socks5ProxyClientType.GetConstructor(new[] { typeof(string), typeof(int) });
                        proxyClient = constructor?.Invoke(new object[] { proxyHost, proxyPort });
                    }
                }
                
                if (proxyClient != null)
                {
                    var proxyProperty = httpRequestType.GetProperty("Proxy");
                    if (proxyProperty != null)
                    {
                        proxyProperty.SetValue(httpRequest, proxyClient);
                        Console.WriteLine($"✅ Proxy configured: {proxyType}://{proxyHost}:{proxyPort}");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ Failed to create proxy client for {proxyType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Proxy setup failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Extracts response content and metadata for analysis
        /// </summary>
        public static string GetResponseContent(object response)
        {
            if (response == null) return null;
            
            try
            {
                // Try different property names for response content
                var properties = new[] { "Content", "ToString", "Body", "Text" };
                
                foreach (var propertyName in properties)
                {
                    var property = response.GetType().GetProperty(propertyName);
                    if (property != null)
                    {
                        var content = property.GetValue(response);
                        if (content != null)
                        {
                            return content.ToString();
                        }
                    }
                }
                
                // Fallback to ToString()
                return response.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to extract response content: {ex.Message}");
                return response?.ToString();
            }
        }
    }
}
