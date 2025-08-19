// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.Socks4Handler
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket;

internal sealed class Socks4Handler(Socket server, string user) : SocksHandler(server, user)
{
  private byte[] GetHostPortBytes(string host, int port)
  {
    if (host == null)
      throw new ArgumentNullException();
    if (port <= 0 || port > (int) ushort.MaxValue)
      throw new ArgumentException();
    byte[] destinationArray = new byte[10 + this.Username.Length + host.Length];
    destinationArray[0] = (byte) 4;
    destinationArray[1] = (byte) 1;
    Array.Copy((Array) this.PortToBytes(port), 0, (Array) destinationArray, 2, 2);
    destinationArray[4] = destinationArray[5] = destinationArray[6] = (byte) 0;
    destinationArray[7] = (byte) 1;
    Array.Copy((Array) Encoding.ASCII.GetBytes(this.Username), 0, (Array) destinationArray, 8, this.Username.Length);
    destinationArray[8 + this.Username.Length] = (byte) 0;
    Array.Copy((Array) Encoding.ASCII.GetBytes(host), 0, (Array) destinationArray, 9 + this.Username.Length, host.Length);
    destinationArray[9 + this.Username.Length + host.Length] = (byte) 0;
    return destinationArray;
  }

  private byte[] GetEndPointBytes(IPEndPoint remoteEP)
  {
    if (remoteEP == null)
      throw new ArgumentNullException();
    byte[] destinationArray = new byte[9 + this.Username.Length];
    destinationArray[0] = (byte) 4;
    destinationArray[1] = (byte) 1;
    Array.Copy((Array) this.PortToBytes(remoteEP.Port), 0, (Array) destinationArray, 2, 2);
    Array.Copy((Array) remoteEP.Address.GetAddressBytes(), 0, (Array) destinationArray, 4, 4);
    Array.Copy((Array) Encoding.ASCII.GetBytes(this.Username), 0, (Array) destinationArray, 8, this.Username.Length);
    destinationArray[8 + this.Username.Length] = (byte) 0;
    return destinationArray;
  }

  public override void Negotiate(string host, int port)
  {
    this.Negotiate(this.GetHostPortBytes(host, port));
  }

  public override void Negotiate(IPEndPoint remoteEP)
  {
    this.Negotiate(this.GetEndPointBytes(remoteEP));
  }

  private void Negotiate(byte[] connect)
  {
    if (connect == null)
      throw new ArgumentNullException();
    if (connect.Length < 2)
      throw new ArgumentException();
    if (this.Server.Send(connect) < connect.Length)
      throw new SocketException(10054);
    if (this.ReadBytes(8)[1] != (byte) 90)
    {
      this.Server.Close();
      throw new ProxyException("Negotiation failed.");
    }
  }

  public override IAsyncProxyResult BeginNegotiate(
    string host,
    int port,
    HandShakeComplete callback,
    IPEndPoint proxyEndPoint)
  {
    this.ProtocolComplete = callback;
    this.Buffer = this.GetHostPortBytes(host, port);
    this.Server.BeginConnect((EndPoint) proxyEndPoint, new AsyncCallback(this.OnConnect), (object) this.Server);
    this.AsyncResult = new IAsyncProxyResult();
    return this.AsyncResult;
  }

  public override IAsyncProxyResult BeginNegotiate(
    IPEndPoint remoteEP,
    HandShakeComplete callback,
    IPEndPoint proxyEndPoint)
  {
    this.ProtocolComplete = callback;
    this.Buffer = this.GetEndPointBytes(remoteEP);
    this.Server.BeginConnect((EndPoint) proxyEndPoint, new AsyncCallback(this.OnConnect), (object) this.Server);
    this.AsyncResult = new IAsyncProxyResult();
    return this.AsyncResult;
  }

  private void OnConnect(IAsyncResult ar)
  {
    try
    {
      this.Server.EndConnect(ar);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
      return;
    }
    try
    {
      this.Server.BeginSend(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnSent), (object) this.Server);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
    }
  }

  private void OnSent(IAsyncResult ar)
  {
    try
    {
      this.HandleEndSend(ar, this.Buffer.Length);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
      return;
    }
    try
    {
      this.Buffer = new byte[8];
      this.Received = 0;
      this.Server.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), (object) this.Server);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
    }
  }

  private void OnReceive(IAsyncResult ar)
  {
    try
    {
      this.HandleEndReceive(ar);
      if (this.Received == 8)
      {
        if (this.Buffer[1] == (byte) 90)
        {
          this.ProtocolComplete((Exception) null);
        }
        else
        {
          this.Server.Close();
          this.ProtocolComplete((Exception) new ProxyException("Negotiation failed."));
        }
      }
      else
        this.Server.BeginReceive(this.Buffer, this.Received, this.Buffer.Length - this.Received, SocketFlags.None, new AsyncCallback(this.OnReceive), (object) this.Server);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
    }
  }
}
