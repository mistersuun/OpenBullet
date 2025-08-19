// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.PortNumbers
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Io;

internal static class PortNumbers
{
  private static readonly Dictionary<string, string> Ports = new Dictionary<string, string>()
  {
    {
      ProtocolNames.Http,
      "80"
    },
    {
      ProtocolNames.Https,
      "443"
    },
    {
      ProtocolNames.Ftp,
      "21"
    },
    {
      ProtocolNames.File,
      ""
    },
    {
      ProtocolNames.Ws,
      "80"
    },
    {
      ProtocolNames.Wss,
      "443"
    },
    {
      ProtocolNames.Gopher,
      "70"
    },
    {
      ProtocolNames.Telnet,
      "23"
    },
    {
      ProtocolNames.Ssh,
      "22"
    }
  };

  public static string GetDefaultPort(string protocol)
  {
    string defaultPort;
    PortNumbers.Ports.TryGetValue(protocol, out defaultPort);
    return defaultPort;
  }
}
