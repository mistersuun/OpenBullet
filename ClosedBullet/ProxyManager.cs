using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Threading;
using Leaf.xNet;

namespace ClosedBullet
{
    public enum ProxyType
    {
        HTTP,
        SOCKS4,
        SOCKS4A,
        SOCKS5
    }

    public class ProxyInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public ProxyType Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAlive { get; set; }
        public int FailCount { get; set; }
        public DateTime LastUsed { get; set; }
        public long ResponseTime { get; set; }
        public string Country { get; set; }
        
        public string ToProxyString()
        {
            var auth = !string.IsNullOrEmpty(Username) ? $"{Username}:{Password}@" : "";
            return $"{Type.ToString().ToLower()}://{auth}{Host}:{Port}";
        }
        
        public override string ToString()
        {
            return $"{Host}:{Port} ({Type}) - {Country ?? "Unknown"}";
        }
    }

    public class ProxyManager
    {
        private List<ProxyInfo> proxies = new List<ProxyInfo>();
        private List<ProxyInfo> workingProxies = new List<ProxyInfo>();
        private List<ProxyInfo> deadProxies = new List<ProxyInfo>();
        private int currentProxyIndex = 0;
        private readonly object lockObject = new object();
        private bool rotationEnabled = false;
        private int maxFailures = 3;
        
        public event EventHandler<string> ProxyStatusChanged;
        
        public int TotalProxies => proxies.Count;
        public int WorkingProxies => workingProxies.Count;
        public int DeadProxies => deadProxies.Count;
        public bool RotationEnabled 
        { 
            get => rotationEnabled;
            set => rotationEnabled = value;
        }
        
        public ProxyManager()
        {
            // Initialize with no proxies (direct connection)
        }
        
        // Load proxies from file
        public void LoadProxiesFromFile(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    
                    var proxy = ParseProxyLine(line.Trim());
                    if (proxy != null)
                    {
                        proxies.Add(proxy);
                    }
                }
                
                OnProxyStatusChanged($"Loaded {proxies.Count} proxies from file");
            }
            catch (Exception ex)
            {
                OnProxyStatusChanged($"Error loading proxies: {ex.Message}");
            }
        }
        
        // Parse proxy line in various formats
        private ProxyInfo ParseProxyLine(string line)
        {
            try
            {
                ProxyInfo proxy = new ProxyInfo();
                
                // Format 1: type://user:pass@host:port
                if (line.Contains("://"))
                {
                    var parts = line.Split(new[] { "://" }, StringSplitOptions.None);
                    proxy.Type = ParseProxyType(parts[0]);
                    
                    var remaining = parts[1];
                    
                    // Check for authentication
                    if (remaining.Contains("@"))
                    {
                        var authParts = remaining.Split('@');
                        var credentials = authParts[0].Split(':');
                        proxy.Username = credentials[0];
                        proxy.Password = credentials.Length > 1 ? credentials[1] : "";
                        remaining = authParts[1];
                    }
                    
                    var hostPort = remaining.Split(':');
                    proxy.Host = hostPort[0];
                    proxy.Port = int.Parse(hostPort[1]);
                }
                // Format 2: host:port:user:pass
                else if (line.Count(c => c == ':') >= 3)
                {
                    var parts = line.Split(':');
                    proxy.Host = parts[0];
                    proxy.Port = int.Parse(parts[1]);
                    proxy.Username = parts[2];
                    proxy.Password = parts.Length > 3 ? parts[3] : "";
                    proxy.Type = ProxyType.HTTP; // Default
                }
                // Format 3: host:port
                else if (line.Contains(":"))
                {
                    var parts = line.Split(':');
                    proxy.Host = parts[0];
                    proxy.Port = int.Parse(parts[1]);
                    proxy.Type = ProxyType.HTTP; // Default
                }
                else
                {
                    return null;
                }
                
                proxy.IsAlive = true;
                return proxy;
            }
            catch
            {
                return null;
            }
        }
        
        private ProxyType ParseProxyType(string type)
        {
            switch (type.ToLower())
            {
                case "http":
                case "https":
                    return ProxyType.HTTP;
                case "socks4":
                    return ProxyType.SOCKS4;
                case "socks4a":
                    return ProxyType.SOCKS4A;
                case "socks5":
                    return ProxyType.SOCKS5;
                default:
                    return ProxyType.HTTP;
            }
        }
        
        // Get next proxy for rotation
        public ProxyInfo GetNextProxy()
        {
            if (!rotationEnabled || workingProxies.Count == 0)
                return null;
            
            lock (lockObject)
            {
                if (workingProxies.Count == 0)
                {
                    // Reset all proxies if all are dead
                    workingProxies = new List<ProxyInfo>(proxies);
                    deadProxies.Clear();
                }
                
                var proxy = workingProxies[currentProxyIndex % workingProxies.Count];
                currentProxyIndex++;
                proxy.LastUsed = DateTime.Now;
                return proxy;
            }
        }
        
        // Configure HttpRequest with proxy
        public void ConfigureRequest(HttpRequest request, ProxyInfo proxy = null)
        {
            if (proxy == null && rotationEnabled)
            {
                proxy = GetNextProxy();
            }
            
            if (proxy != null)
            {
                try
                {
                    switch (proxy.Type)
                    {
                        case ProxyType.HTTP:
                            request.Proxy = new HttpProxyClient(proxy.Host, proxy.Port, proxy.Username, proxy.Password);
                            break;
                        case ProxyType.SOCKS4:
                            request.Proxy = new Socks4ProxyClient(proxy.Host, proxy.Port, proxy.Username);
                            break;
                        case ProxyType.SOCKS4A:
                            request.Proxy = new Socks4AProxyClient(proxy.Host, proxy.Port, proxy.Username);
                            break;
                        case ProxyType.SOCKS5:
                            request.Proxy = new Socks5ProxyClient(proxy.Host, proxy.Port, proxy.Username, proxy.Password);
                            break;
                    }
                    
                    request.ConnectTimeout = 10000; // 10 seconds for proxy connections
                }
                catch (Exception ex)
                {
                    OnProxyStatusChanged($"Proxy configuration error: {ex.Message}");
                    MarkProxyAsDead(proxy);
                }
            }
        }
        
        // Mark proxy as dead
        public void MarkProxyAsDead(ProxyInfo proxy)
        {
            if (proxy == null) return;
            
            lock (lockObject)
            {
                proxy.FailCount++;
                
                if (proxy.FailCount >= maxFailures)
                {
                    proxy.IsAlive = false;
                    workingProxies.Remove(proxy);
                    if (!deadProxies.Contains(proxy))
                    {
                        deadProxies.Add(proxy);
                    }
                    
                    OnProxyStatusChanged($"Proxy marked as dead: {proxy.Host}:{proxy.Port}");
                }
            }
        }
        
        // Mark proxy as working
        public void MarkProxyAsWorking(ProxyInfo proxy, long responseTime)
        {
            if (proxy == null) return;
            
            lock (lockObject)
            {
                proxy.IsAlive = true;
                proxy.FailCount = 0;
                proxy.ResponseTime = responseTime;
                
                if (!workingProxies.Contains(proxy))
                {
                    workingProxies.Add(proxy);
                    deadProxies.Remove(proxy);
                }
            }
        }
        
        // Test all proxies
        public async System.Threading.Tasks.Task TestAllProxiesAsync(Action<int, int> progressCallback = null)
        {
            int tested = 0;
            int working = 0;
            
            var tasks = proxies.Select(async proxy =>
            {
                bool isWorking = await TestProxyAsync(proxy);
                
                lock (lockObject)
                {
                    tested++;
                    if (isWorking)
                    {
                        working++;
                        if (!workingProxies.Contains(proxy))
                            workingProxies.Add(proxy);
                    }
                    else
                    {
                        if (!deadProxies.Contains(proxy))
                            deadProxies.Add(proxy);
                    }
                    
                    progressCallback?.Invoke(tested, proxies.Count);
                }
                
                return isWorking;
            }).ToArray();
            
            await System.Threading.Tasks.Task.WhenAll(tasks);
            
            OnProxyStatusChanged($"Proxy test complete: {working}/{proxies.Count} working");
        }
        
        // Test single proxy
        private async System.Threading.Tasks.Task<bool> TestProxyAsync(ProxyInfo proxy)
        {
            try
            {
                using (var request = new HttpRequest())
                {
                    ConfigureRequest(request, proxy);
                    request.ConnectTimeout = 5000;
                    request.ReadWriteTimeout = 5000;
                    
                    var startTime = DateTime.Now;
                    var response = await System.Threading.Tasks.Task.Run(() => 
                        request.Get("http://httpbin.org/ip"));
                    
                    if (response.StatusCode == Leaf.xNet.HttpStatusCode.OK)
                    {
                        proxy.ResponseTime = (long)(DateTime.Now - startTime).TotalMilliseconds;
                        proxy.IsAlive = true;
                        
                        // Try to parse country from response if available
                        var content = response.ToString();
                        if (content.Contains("origin"))
                        {
                            // Extract IP and potentially geolocate
                            proxy.Country = "Unknown";
                        }
                        
                        return true;
                    }
                }
            }
            catch
            {
                proxy.IsAlive = false;
            }
            
            return false;
        }
        
        // Clear all proxies
        public void ClearProxies()
        {
            lock (lockObject)
            {
                proxies.Clear();
                workingProxies.Clear();
                deadProxies.Clear();
                currentProxyIndex = 0;
            }
        }
        
        // Export working proxies
        public void ExportWorkingProxies(string filePath)
        {
            try
            {
                var lines = workingProxies.Select(p => p.ToProxyString());
                File.WriteAllLines(filePath, lines);
                OnProxyStatusChanged($"Exported {workingProxies.Count} working proxies");
            }
            catch (Exception ex)
            {
                OnProxyStatusChanged($"Export error: {ex.Message}");
            }
        }
        
        protected virtual void OnProxyStatusChanged(string status)
        {
            ProxyStatusChanged?.Invoke(this, status);
        }
    }
}