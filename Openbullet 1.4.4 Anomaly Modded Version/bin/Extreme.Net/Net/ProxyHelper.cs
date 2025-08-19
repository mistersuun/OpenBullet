// Decompiled with JetBrains decompiler
// Type: Extreme.Net.ProxyHelper
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;

#nullable disable
namespace Extreme.Net;

internal static class ProxyHelper
{
  public static ProxyClient CreateProxyClient(
    ProxyType proxyType,
    string host = null,
    int port = 0,
    string username = null,
    string password = null)
  {
    switch (proxyType)
    {
      case ProxyType.Http:
        return port == 0 ? (ProxyClient) new HttpProxyClient(host) : (ProxyClient) new HttpProxyClient(host, port, username, password);
      case ProxyType.Socks4:
        return port == 0 ? (ProxyClient) new Socks4ProxyClient(host) : (ProxyClient) new Socks4ProxyClient(host, port, username);
      case ProxyType.Socks4a:
        return port == 0 ? (ProxyClient) new Socks4aProxyClient(host) : (ProxyClient) new Socks4aProxyClient(host, port, username);
      case ProxyType.Socks5:
        return port == 0 ? (ProxyClient) new Socks5ProxyClient(host) : (ProxyClient) new Socks5ProxyClient(host, port, username, password);
      default:
        throw new InvalidOperationException();
    }
  }
}
