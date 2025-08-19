// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.ProxySocket
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Net;
using System.Net.Sockets;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket;

public class ProxySocket : Socket
{
  private object m_State;
  private IPEndPoint m_ProxyEndPoint;
  private ProxyTypes m_ProxyType;
  private string m_ProxyUser;
  private string m_ProxyPass;
  private AsyncCallback CallBack;
  private IAsyncProxyResult m_AsyncResult;
  private Exception m_ToThrow;
  private int m_RemotePort;

  public ProxySocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
    : this(addressFamily, socketType, protocolType, "")
  {
  }

  public ProxySocket(
    AddressFamily addressFamily,
    SocketType socketType,
    ProtocolType protocolType,
    string proxyUsername)
    : this(addressFamily, socketType, protocolType, proxyUsername, "")
  {
  }

  public ProxySocket(
    AddressFamily addressFamily,
    SocketType socketType,
    ProtocolType protocolType,
    string proxyUsername,
    string proxyPassword)
    : base(addressFamily, socketType, protocolType)
  {
    this.ProxyUser = proxyUsername;
    this.ProxyPass = proxyPassword;
    this.ToThrow = (Exception) new InvalidOperationException();
  }

  public new void Connect(EndPoint remoteEP)
  {
    if (remoteEP == null)
      throw new ArgumentNullException("<remoteEP> cannot be null.");
    if (this.ProtocolType != ProtocolType.Tcp || this.ProxyType == ProxyTypes.None || this.ProxyEndPoint == null)
    {
      base.Connect(remoteEP);
    }
    else
    {
      base.Connect((EndPoint) this.ProxyEndPoint);
      if (this.ProxyType == ProxyTypes.Https)
        new HttpsHandler((Socket) this, this.ProxyUser, this.ProxyPass).Negotiate((IPEndPoint) remoteEP);
      else if (this.ProxyType == ProxyTypes.Socks4)
      {
        new Socks4Handler((Socket) this, this.ProxyUser).Negotiate((IPEndPoint) remoteEP);
      }
      else
      {
        if (this.ProxyType != ProxyTypes.Socks5)
          return;
        new Socks5Handler((Socket) this, this.ProxyUser, this.ProxyPass).Negotiate((IPEndPoint) remoteEP);
      }
    }
  }

  public new void Connect(string host, int port)
  {
    if (host == null)
      throw new ArgumentNullException("<host> cannot be null.");
    if (port <= 0 || port > (int) ushort.MaxValue)
      throw new ArgumentException("Invalid port.");
    if (this.ProtocolType != ProtocolType.Tcp || this.ProxyType == ProxyTypes.None || this.ProxyEndPoint == null)
    {
      base.Connect((EndPoint) new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port));
    }
    else
    {
      base.Connect((EndPoint) this.ProxyEndPoint);
      if (this.ProxyType == ProxyTypes.Https)
        new HttpsHandler((Socket) this, this.ProxyUser, this.ProxyPass).Negotiate(host, port);
      else if (this.ProxyType == ProxyTypes.Socks4)
      {
        new Socks4Handler((Socket) this, this.ProxyUser).Negotiate(host, port);
      }
      else
      {
        if (this.ProxyType != ProxyTypes.Socks5)
          return;
        new Socks5Handler((Socket) this, this.ProxyUser, this.ProxyPass).Negotiate(host, port);
      }
    }
  }

  public new IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
  {
    if (remoteEP == null)
      throw new ArgumentNullException();
    if (this.ProtocolType != ProtocolType.Tcp || this.ProxyType == ProxyTypes.None || this.ProxyEndPoint == null)
      return base.BeginConnect(remoteEP, callback, state);
    this.CallBack = callback;
    if (this.ProxyType == ProxyTypes.Https)
    {
      this.AsyncResult = new HttpsHandler((Socket) this, this.ProxyUser, this.ProxyPass).BeginNegotiate((IPEndPoint) remoteEP, new HandShakeComplete(this.OnHandShakeComplete), this.ProxyEndPoint);
      return (IAsyncResult) this.AsyncResult;
    }
    if (this.ProxyType == ProxyTypes.Socks4)
    {
      this.AsyncResult = new Socks4Handler((Socket) this, this.ProxyUser).BeginNegotiate((IPEndPoint) remoteEP, new HandShakeComplete(this.OnHandShakeComplete), this.ProxyEndPoint);
      return (IAsyncResult) this.AsyncResult;
    }
    if (this.ProxyType != ProxyTypes.Socks5)
      return (IAsyncResult) null;
    this.AsyncResult = new Socks5Handler((Socket) this, this.ProxyUser, this.ProxyPass).BeginNegotiate((IPEndPoint) remoteEP, new HandShakeComplete(this.OnHandShakeComplete), this.ProxyEndPoint);
    return (IAsyncResult) this.AsyncResult;
  }

  public new IAsyncResult BeginConnect(
    string host,
    int port,
    AsyncCallback callback,
    object state)
  {
    if (host == null)
      throw new ArgumentNullException();
    if (port <= 0 || port > (int) ushort.MaxValue)
      throw new ArgumentException();
    this.CallBack = callback;
    if (this.ProtocolType != ProtocolType.Tcp || this.ProxyType == ProxyTypes.None || this.ProxyEndPoint == null)
    {
      this.RemotePort = port;
      this.AsyncResult = this.BeginDns(host, new HandShakeComplete(this.OnHandShakeComplete));
      return (IAsyncResult) this.AsyncResult;
    }
    if (this.ProxyType == ProxyTypes.Https)
    {
      this.AsyncResult = new HttpsHandler((Socket) this, this.ProxyUser, this.ProxyPass).BeginNegotiate(host, port, new HandShakeComplete(this.OnHandShakeComplete), this.ProxyEndPoint);
      return (IAsyncResult) this.AsyncResult;
    }
    if (this.ProxyType == ProxyTypes.Socks4)
    {
      this.AsyncResult = new Socks4Handler((Socket) this, this.ProxyUser).BeginNegotiate(host, port, new HandShakeComplete(this.OnHandShakeComplete), this.ProxyEndPoint);
      return (IAsyncResult) this.AsyncResult;
    }
    if (this.ProxyType != ProxyTypes.Socks5)
      return (IAsyncResult) null;
    this.AsyncResult = new Socks5Handler((Socket) this, this.ProxyUser, this.ProxyPass).BeginNegotiate(host, port, new HandShakeComplete(this.OnHandShakeComplete), this.ProxyEndPoint);
    return (IAsyncResult) this.AsyncResult;
  }

  public new void EndConnect(IAsyncResult asyncResult)
  {
    if (asyncResult == null)
      throw new ArgumentNullException();
    if (!(asyncResult is IAsyncProxyResult))
    {
      base.EndConnect(asyncResult);
    }
    else
    {
      if (!asyncResult.IsCompleted)
        asyncResult.AsyncWaitHandle.WaitOne();
      if (this.ToThrow != null)
        throw this.ToThrow;
    }
  }

  internal IAsyncProxyResult BeginDns(string host, HandShakeComplete callback)
  {
    try
    {
      Dns.BeginGetHostEntry(host, new AsyncCallback(this.OnResolved), (object) this);
      return new IAsyncProxyResult();
    }
    catch
    {
      throw new SocketException();
    }
  }

  private void OnResolved(IAsyncResult asyncResult)
  {
    try
    {
      base.BeginConnect((EndPoint) new IPEndPoint(Dns.EndGetHostEntry(asyncResult).AddressList[0], this.RemotePort), new AsyncCallback(this.OnConnect), this.State);
    }
    catch (Exception ex)
    {
      this.OnHandShakeComplete(ex);
    }
  }

  private void OnConnect(IAsyncResult asyncResult)
  {
    try
    {
      base.EndConnect(asyncResult);
      this.OnHandShakeComplete((Exception) null);
    }
    catch (Exception ex)
    {
      this.OnHandShakeComplete(ex);
    }
  }

  private void OnHandShakeComplete(Exception error)
  {
    if (error != null)
      this.Close();
    this.ToThrow = error;
    this.AsyncResult.Reset();
    if (this.CallBack == null)
      return;
    this.CallBack((IAsyncResult) this.AsyncResult);
  }

  public IPEndPoint ProxyEndPoint
  {
    get => this.m_ProxyEndPoint;
    set => this.m_ProxyEndPoint = value;
  }

  public ProxyTypes ProxyType
  {
    get => this.m_ProxyType;
    set => this.m_ProxyType = value;
  }

  private object State
  {
    get => this.m_State;
    set => this.m_State = value;
  }

  public string ProxyUser
  {
    get => this.m_ProxyUser;
    set => this.m_ProxyUser = value != null ? value : throw new ArgumentNullException();
  }

  public string ProxyPass
  {
    get => this.m_ProxyPass;
    set => this.m_ProxyPass = value != null ? value : throw new ArgumentNullException();
  }

  private IAsyncProxyResult AsyncResult
  {
    get => this.m_AsyncResult;
    set => this.m_AsyncResult = value;
  }

  private Exception ToThrow
  {
    get => this.m_ToThrow;
    set => this.m_ToThrow = value;
  }

  private int RemotePort
  {
    get => this.m_RemotePort;
    set => this.m_RemotePort = value;
  }
}
