# ProxySocket.dll Documentation

## Overview
ProxySocket.dll provides low-level socket proxy support for .NET applications, enabling TCP/UDP connections through SOCKS4, SOCKS4a, and SOCKS5 proxies at the socket level.

## Purpose in OpenBullet
- Low-level proxy implementation
- Socket-based proxy connections
- SOCKS protocol support
- Direct TCP/UDP proxy tunneling
- Bypass HTTP-level proxy limitations

## Key Components

### Core Proxy Classes

#### `ProxySocket`
- **Purpose**: Socket wrapper with proxy support
- **Key Properties**:
  - `ProxyType` - Type of proxy (SOCKS4/4a/5)
  - `ProxyEndPoint` - Proxy server endpoint
  - `ProxyUser` - Authentication username
  - `ProxyPass` - Authentication password
- **Key Methods**:
  - `Connect()` - Connect through proxy
  - `Send()` - Send data through proxy
  - `Receive()` - Receive data through proxy

```csharp
var proxySocket = new ProxySocket(ProxyTypes.Socks5, "proxy.server.com", 1080);
proxySocket.ProxyUser = "username";
proxySocket.ProxyPass = "password";
proxySocket.Connect("target.server.com", 443);
```

### SOCKS Implementations

#### `Socks4Socket`
```csharp
public class Socks4Socket : ProxySocket
{
    public void ConnectSocks4(string host, int port)
    {
        // SOCKS4 protocol implementation
        byte[] request = BuildSocks4Request(host, port);
        Send(request);
        
        byte[] response = new byte[8];
        Receive(response);
        
        if (response[1] != 0x5A) // Request granted
            throw new ProxyException("SOCKS4 connection failed");
    }
}
```

#### `Socks5Socket`
```csharp
public class Socks5Socket : ProxySocket
{
    public void ConnectSocks5(string host, int port)
    {
        // Authentication
        SendAuthenticationRequest();
        
        // Connection request
        SendConnectionRequest(host, port);
        
        // Verify connection
        VerifyConnection();
    }
    
    private void SendAuthenticationRequest()
    {
        if (!string.IsNullOrEmpty(ProxyUser))
        {
            // Username/password authentication
            byte[] auth = BuildAuthRequest(ProxyUser, ProxyPass);
            Send(auth);
            VerifyAuthResponse();
        }
    }
}
```

## Implementation Examples

### Basic Proxy Connection
```csharp
public class ProxyConnection
{
    public Socket ConnectThroughProxy(string proxyHost, int proxyPort, 
                                     string targetHost, int targetPort)
    {
        var proxy = new ProxySocket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
        
        proxy.ProxyType = ProxyTypes.Socks5;
        proxy.ProxyEndPoint = new IPEndPoint(
            IPAddress.Parse(proxyHost), proxyPort);
        
        try
        {
            proxy.Connect(targetHost, targetPort);
            return proxy;
        }
        catch (ProxyException ex)
        {
            Console.WriteLine($"Proxy error: {ex.Message}");
            throw;
        }
    }
}
```

### SSL/TLS Over Proxy
```csharp
public class SecureProxyConnection
{
    public SslStream CreateSecureConnection(ProxySocket proxySocket, string host)
    {
        var networkStream = new NetworkStream(proxySocket);
        var sslStream = new SslStream(networkStream, false, 
            ValidateServerCertificate);
        
        sslStream.AuthenticateAsClient(host);
        return sslStream;
    }
    
    private bool ValidateServerCertificate(object sender, 
        X509Certificate certificate, X509Chain chain, 
        SslPolicyErrors sslPolicyErrors)
    {
        // Certificate validation logic
        return sslPolicyErrors == SslPolicyErrors.None;
    }
}
```

## Integration with OpenBullet
- Provides low-level proxy support for HTTP clients
- Enables proxy chaining
- Supports authentication methods
- Direct socket control for custom protocols

## Best Practices
1. Always dispose socket connections
2. Implement connection pooling
3. Handle proxy authentication properly
4. Use appropriate timeout values
5. Implement retry logic for failed connections

## Limitations
- No HTTP proxy support (SOCKS only)
- Manual SSL/TLS implementation required
- No built-in connection pooling
- Limited error reporting

## Dependencies
- .NET Framework 4.0+
- System.Net.Sockets