// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.HttpsHandler
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket;

internal sealed class HttpsHandler : SocksHandler
{
  private string m_Password;
  private int m_receivedNewlineChars;

  public HttpsHandler(Socket server)
    : this(server, "")
  {
  }

  public HttpsHandler(Socket server, string user)
    : this(server, user, "")
  {
  }

  public HttpsHandler(Socket server, string user, string pass)
    : base(server, user)
  {
    this.Password = pass;
  }

  private byte[] GetConnectBytes(string host, int port)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine($"CONNECT {host}:{port} HTTP/1.1");
    stringBuilder.AppendLine($"Host: {host}:{port}");
    if (!string.IsNullOrEmpty(this.Username))
    {
      string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{this.Username}:{this.Password}"));
      stringBuilder.AppendLine($"Proxy-Authorization: Basic {base64String}");
    }
    stringBuilder.AppendLine();
    return Encoding.ASCII.GetBytes(stringBuilder.ToString());
  }

  private void VerifyConnectHeader(byte[] buffer)
  {
    string str1 = Encoding.ASCII.GetString(buffer);
    string str2 = (str1.StartsWith("HTTP/1.1 ", StringComparison.OrdinalIgnoreCase) || str1.StartsWith("HTTP/1.0 ", StringComparison.OrdinalIgnoreCase)) && str1.EndsWith(" ") ? str1.Substring(9, 3) : throw new ProtocolViolationException();
    if (str2 != "200")
      throw new ProxyException("Invalid HTTP status. Code: " + str2);
  }

  public override void Negotiate(IPEndPoint remoteEP)
  {
    if (remoteEP == null)
      throw new ArgumentNullException();
    this.Negotiate(remoteEP.Address.ToString(), remoteEP.Port);
  }

  public override void Negotiate(string host, int port)
  {
    if (host == null)
      throw new ArgumentNullException();
    if (port <= 0 || port > (int) ushort.MaxValue || host.Length > (int) byte.MaxValue)
      throw new ArgumentException();
    byte[] connectBytes = this.GetConnectBytes(host, port);
    if (this.Server.Send(connectBytes, 0, connectBytes.Length, SocketFlags.None) < connectBytes.Length)
      throw new SocketException(10054);
    this.VerifyConnectHeader(this.ReadBytes(13));
    int num1 = 0;
    byte[] buffer = new byte[1];
    while (num1 < 4)
    {
      byte num2 = this.Server.Receive(buffer, 0, 1, SocketFlags.None) != 0 ? buffer[0] : throw new SocketException(10054);
      if ((int) num2 == (num1 % 2 == 0 ? 13 : 10))
        ++num1;
      else
        num1 = num2 == (byte) 13 ? 1 : 0;
    }
  }

  public override IAsyncProxyResult BeginNegotiate(
    IPEndPoint remoteEP,
    HandShakeComplete callback,
    IPEndPoint proxyEndPoint)
  {
    return this.BeginNegotiate(remoteEP.Address.ToString(), remoteEP.Port, callback, proxyEndPoint);
  }

  public override IAsyncProxyResult BeginNegotiate(
    string host,
    int port,
    HandShakeComplete callback,
    IPEndPoint proxyEndPoint)
  {
    this.ProtocolComplete = callback;
    this.Buffer = this.GetConnectBytes(host, port);
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
      this.Server.BeginSend(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnConnectSent), (object) null);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
    }
  }

  private void OnConnectSent(IAsyncResult ar)
  {
    try
    {
      this.HandleEndSend(ar, this.Buffer.Length);
      this.Buffer = new byte[13];
      this.Received = 0;
      this.Server.BeginReceive(this.Buffer, 0, 13, SocketFlags.None, new AsyncCallback(this.OnConnectReceive), (object) this.Server);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
    }
  }

  private void OnConnectReceive(IAsyncResult ar)
  {
    try
    {
      this.HandleEndReceive(ar);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
      return;
    }
    try
    {
      if (this.Received < 13)
      {
        this.Server.BeginReceive(this.Buffer, this.Received, 13 - this.Received, SocketFlags.None, new AsyncCallback(this.OnConnectReceive), (object) this.Server);
      }
      else
      {
        this.VerifyConnectHeader(this.Buffer);
        this.ReadUntilHeadersEnd(true);
      }
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
    }
  }

  private void ReadUntilHeadersEnd(bool readFirstByte)
  {
    while (this.Server.Available > 0 && this.m_receivedNewlineChars < 4)
    {
      if (!readFirstByte)
        readFirstByte = false;
      else if (this.Server.Receive(this.Buffer, 0, 1, SocketFlags.None) == 0)
        throw new SocketException(10054);
      if ((int) this.Buffer[0] == (this.m_receivedNewlineChars % 2 == 0 ? 13 : 10))
        ++this.m_receivedNewlineChars;
      else
        this.m_receivedNewlineChars = this.Buffer[0] == (byte) 13 ? 1 : 0;
    }
    if (this.m_receivedNewlineChars == 4)
      this.ProtocolComplete((Exception) null);
    else
      this.Server.BeginReceive(this.Buffer, 0, 1, SocketFlags.None, new AsyncCallback(this.OnEndHeadersReceive), (object) this.Server);
  }

  private void OnEndHeadersReceive(IAsyncResult ar)
  {
    try
    {
      this.HandleEndReceive(ar);
      this.ReadUntilHeadersEnd(false);
    }
    catch (Exception ex)
    {
      this.ProtocolComplete(ex);
    }
  }

  private string Password
  {
    get => this.m_Password;
    set => this.m_Password = value != null ? value : throw new ArgumentNullException();
  }
}
