// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.ProxyException
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket;

[Serializable]
public class ProxyException(string message) : Exception(message)
{
  public ProxyException()
    : this("An error occured while talking to the proxy server.")
  {
  }

  public ProxyException(int socks5Error)
    : this(ProxyException.Socks5ToString(socks5Error))
  {
  }

  public static string Socks5ToString(int socks5Error)
  {
    switch (socks5Error)
    {
      case 0:
        return "Connection succeeded.";
      case 1:
        return "General SOCKS server failure.";
      case 2:
        return "Connection not allowed by ruleset.";
      case 3:
        return "Network unreachable.";
      case 4:
        return "Host unreachable.";
      case 5:
        return "Connection refused.";
      case 6:
        return "TTL expired.";
      case 7:
        return "Command not supported.";
      case 8:
        return "Address type not supported.";
      default:
        return "Unspecified SOCKS error.";
    }
  }
}
