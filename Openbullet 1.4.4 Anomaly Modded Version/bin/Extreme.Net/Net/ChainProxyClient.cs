// Decompiled with JetBrains decompiler
// Type: Extreme.Net.ChainProxyClient
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

#nullable disable
namespace Extreme.Net;

public class ChainProxyClient : ProxyClient
{
  [ThreadStatic]
  private static Random _rand;
  private List<ProxyClient> _proxies = new List<ProxyClient>();

  private static Random Rand
  {
    get
    {
      if (ChainProxyClient._rand == null)
        ChainProxyClient._rand = new Random();
      return ChainProxyClient._rand;
    }
  }

  public bool EnableShuffle { get; set; }

  public List<ProxyClient> Proxies => this._proxies;

  public override string Host
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  public override int Port
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  public override string Username
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  public override string Password
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  public override int ConnectTimeout
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  public override int ReadWriteTimeout
  {
    get => throw new NotSupportedException();
    set => throw new NotSupportedException();
  }

  public ChainProxyClient(bool enableShuffle = false)
    : base(ProxyType.Chain)
  {
    this.EnableShuffle = enableShuffle;
  }

  public override TcpClient CreateConnection(
    string destinationHost,
    int destinationPort,
    TcpClient tcpClient = null)
  {
    if (this._proxies.Count == 0)
      throw new InvalidOperationException(Resources.InvalidOperationException_ChainProxyClient_NotProxies);
    List<ProxyClient> proxyClientList;
    if (this.EnableShuffle)
    {
      proxyClientList = this._proxies.ToList<ProxyClient>();
      for (int index1 = 0; index1 < proxyClientList.Count; ++index1)
      {
        int index2 = ChainProxyClient.Rand.Next(proxyClientList.Count);
        ProxyClient proxyClient = proxyClientList[index1];
        proxyClientList[index1] = proxyClientList[index2];
        proxyClientList[index2] = proxyClient;
      }
    }
    else
      proxyClientList = this._proxies;
    int index3 = proxyClientList.Count - 1;
    TcpClient tcpClient1 = tcpClient;
    for (int index4 = 0; index4 < index3; ++index4)
      tcpClient1 = proxyClientList[index4].CreateConnection(proxyClientList[index4 + 1].Host, proxyClientList[index4 + 1].Port, tcpClient1);
    return proxyClientList[index3].CreateConnection(destinationHost, destinationPort, tcpClient1);
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (ProxyClient proxy in this._proxies)
      stringBuilder.AppendLine(proxy.ToString());
    return stringBuilder.ToString();
  }

  public new virtual string ToExtendedString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (ProxyClient proxy in this._proxies)
      stringBuilder.AppendLine(proxy.ToExtendedString());
    return stringBuilder.ToString();
  }

  public void AddProxy(ProxyClient proxy)
  {
    if (proxy == null)
      throw new ArgumentNullException(nameof (proxy));
    this._proxies.Add(proxy);
  }

  public void AddHttpProxy(string proxyAddress)
  {
    this._proxies.Add((ProxyClient) HttpProxyClient.Parse(proxyAddress));
  }

  public void AddSocks4Proxy(string proxyAddress)
  {
    this._proxies.Add((ProxyClient) Socks4ProxyClient.Parse(proxyAddress));
  }

  public void AddSocks4aProxy(string proxyAddress)
  {
    this._proxies.Add((ProxyClient) Socks4aProxyClient.Parse(proxyAddress));
  }

  public void AddSocks5Proxy(string proxyAddress)
  {
    this._proxies.Add((ProxyClient) Socks5ProxyClient.Parse(proxyAddress));
  }
}
