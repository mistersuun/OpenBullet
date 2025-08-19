// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.Authentication.AuthNone
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Net.Sockets;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket.Authentication;

internal sealed class AuthNone(Socket server) : AuthMethod(server)
{
  public override void Authenticate()
  {
  }

  public override void BeginAuthenticate(HandShakeComplete callback) => callback((Exception) null);
}
