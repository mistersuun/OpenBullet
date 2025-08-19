// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.Authentication.AuthUserPass
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Net.Sockets;
using System.Text;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket.Authentication;

internal sealed class AuthUserPass : AuthMethod
{
  private string m_Username;
  private string m_Password;

  public AuthUserPass(Socket server, string user, string pass)
    : base(server)
  {
    this.Username = user;
    this.Password = pass;
  }

  private byte[] GetAuthenticationBytes()
  {
    byte[] destinationArray = new byte[3 + this.Username.Length + this.Password.Length];
    destinationArray[0] = (byte) 1;
    destinationArray[1] = (byte) this.Username.Length;
    Array.Copy((Array) Encoding.ASCII.GetBytes(this.Username), 0, (Array) destinationArray, 2, this.Username.Length);
    destinationArray[this.Username.Length + 2] = (byte) this.Password.Length;
    Array.Copy((Array) Encoding.ASCII.GetBytes(this.Password), 0, (Array) destinationArray, this.Username.Length + 3, this.Password.Length);
    return destinationArray;
  }

  private int GetAuthenticationLength() => 3 + this.Username.Length + this.Password.Length;

  public override void Authenticate()
  {
    if (this.Server.Send(this.GetAuthenticationBytes()) < this.GetAuthenticationLength())
      throw new SocketException(10054);
    byte[] buffer = new byte[2];
    int num;
    for (int offset = 0; offset != 2; offset += num)
    {
      num = this.Server.Receive(buffer, offset, 2 - offset, SocketFlags.None);
      if (num == 0)
        throw new SocketException(10054);
    }
    if (buffer[1] != (byte) 0)
    {
      this.Server.Close();
      throw new ProxyException("Username/password combination rejected.");
    }
  }

  public override void BeginAuthenticate(HandShakeComplete callback)
  {
    this.CallBack = callback;
    this.Server.BeginSend(this.GetAuthenticationBytes(), 0, this.GetAuthenticationLength(), SocketFlags.None, new AsyncCallback(this.OnSent), (object) this.Server);
  }

  private void OnSent(IAsyncResult ar)
  {
    try
    {
      if (this.Server.EndSend(ar) < this.GetAuthenticationLength())
        throw new SocketException(10054);
      this.Buffer = new byte[2];
      this.Server.BeginReceive(this.Buffer, 0, 2, SocketFlags.None, new AsyncCallback(this.OnReceive), (object) this.Server);
    }
    catch (Exception ex)
    {
      this.CallBack(ex);
    }
  }

  private void OnReceive(IAsyncResult ar)
  {
    try
    {
      int num = this.Server.EndReceive(ar);
      if (num <= 0)
        throw new SocketException(10054);
      this.Received += num;
      if (this.Received == this.Buffer.Length)
      {
        if (this.Buffer[1] != (byte) 0)
          throw new ProxyException("Username/password combination not accepted.");
        this.CallBack((Exception) null);
      }
      else
        this.Server.BeginReceive(this.Buffer, this.Received, this.Buffer.Length - this.Received, SocketFlags.None, new AsyncCallback(this.OnReceive), (object) this.Server);
    }
    catch (Exception ex)
    {
      this.CallBack(ex);
    }
  }

  private string Username
  {
    get => this.m_Username;
    set => this.m_Username = value != null ? value : throw new ArgumentNullException();
  }

  private string Password
  {
    get => this.m_Password;
    set => this.m_Password = value != null ? value : throw new ArgumentNullException();
  }
}
