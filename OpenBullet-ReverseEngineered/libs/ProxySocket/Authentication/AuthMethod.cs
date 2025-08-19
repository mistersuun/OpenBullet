// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.Authentication.AuthMethod
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Net.Sockets;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket.Authentication;

internal abstract class AuthMethod
{
  private byte[] m_Buffer;
  private Socket m_Server;
  protected HandShakeComplete CallBack;
  private int m_Received;

  public AuthMethod(Socket server) => this.Server = server;

  public abstract void Authenticate();

  public abstract void BeginAuthenticate(HandShakeComplete callback);

  protected Socket Server
  {
    get => this.m_Server;
    set => this.m_Server = value != null ? value : throw new ArgumentNullException();
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
}
