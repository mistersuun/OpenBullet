// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.SocksHandler
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Net;
using System.Net.Sockets;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket;

internal abstract class SocksHandler
{
  private Socket m_Server;
  private string m_Username;
  private IAsyncProxyResult m_AsyncResult;
  private byte[] m_Buffer;
  private int m_Received;
  protected HandShakeComplete ProtocolComplete;

  public SocksHandler(Socket server, string user)
  {
    this.Server = server;
    this.Username = user;
  }

  protected byte[] PortToBytes(int port)
  {
    return new byte[2]
    {
      (byte) (port / 256 /*0x0100*/),
      (byte) (port % 256 /*0x0100*/)
    };
  }

  protected byte[] AddressToBytes(long address)
  {
    return new byte[4]
    {
      (byte) ((ulong) address % 256UL /*0x0100*/),
      (byte) ((ulong) (address / 256L /*0x0100*/) % 256UL /*0x0100*/),
      (byte) ((ulong) (address / 65536L /*0x010000*/) % 256UL /*0x0100*/),
      (byte) ((ulong) address / 16777216UL /*0x01000000*/)
    };
  }

  protected byte[] ReadBytes(int count)
  {
    byte[] buffer = count > 0 ? new byte[count] : throw new ArgumentException();
    int num;
    for (int offset = 0; offset != count; offset += num)
    {
      num = this.Server.Receive(buffer, offset, count - offset, SocketFlags.None);
      if (num == 0)
        throw new SocketException(10054);
    }
    return buffer;
  }

  protected void HandleEndReceive(IAsyncResult ar)
  {
    int num = this.Server.EndReceive(ar);
    if (num <= 0)
      throw new SocketException(10054);
    this.Received += num;
  }

  protected void HandleEndSend(IAsyncResult ar, int expectedLength)
  {
    if (this.Server.EndSend(ar) < expectedLength)
      throw new SocketException(10054);
  }

  protected Socket Server
  {
    get => this.m_Server;
    set => this.m_Server = value != null ? value : throw new ArgumentNullException();
  }

  protected string Username
  {
    get => this.m_Username;
    set => this.m_Username = value != null ? value : throw new ArgumentNullException();
  }

  protected IAsyncProxyResult AsyncResult
  {
    get => this.m_AsyncResult;
    set => this.m_AsyncResult = value;
  }

  protected byte[] Buffer
  {
    get => this.m_Buffer;
    set => this.m_Buffer = value;
  }

  protected int Received
  {
    get => this.m_Received;
    set => this.m_Received = value;
  }

  public abstract void Negotiate(string host, int port);

  public abstract void Negotiate(IPEndPoint remoteEP);

  public abstract IAsyncProxyResult BeginNegotiate(
    IPEndPoint remoteEP,
    HandShakeComplete callback,
    IPEndPoint proxyEndPoint);

  public abstract IAsyncProxyResult BeginNegotiate(
    string host,
    int port,
    HandShakeComplete callback,
    IPEndPoint proxyEndPoint);
}
