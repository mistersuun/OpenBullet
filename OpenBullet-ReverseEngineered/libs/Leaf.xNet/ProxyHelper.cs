// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.ProxyHelper
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet;

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
      case ProxyType.HTTP:
        return port != 0 ? (ProxyClient) new HttpProxyClient(host, port, username, password) : (ProxyClient) new HttpProxyClient(host);
      case ProxyType.Socks4:
        return port != 0 ? (ProxyClient) new Socks4ProxyClient(host, port, username) : (ProxyClient) new Socks4ProxyClient(host);
      case ProxyType.Socks4A:
        return port != 0 ? (ProxyClient) new Socks4AProxyClient(host, port, username) : (ProxyClient) new Socks4AProxyClient(host);
      case ProxyType.Socks5:
        return port != 0 ? (ProxyClient) new Socks5ProxyClient(host, port, username, password) : (ProxyClient) new Socks5ProxyClient(host);
      default:
        throw new InvalidOperationException();
    }
  }
}
