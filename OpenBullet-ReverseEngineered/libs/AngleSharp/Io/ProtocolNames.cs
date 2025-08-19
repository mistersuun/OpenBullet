// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.ProtocolNames
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Io;

public static class ProtocolNames
{
  public static readonly string Http = "http";
  public static readonly string Https = "https";
  public static readonly string Ftp = "ftp";
  public static readonly string JavaScript = "javascript";
  public static readonly string Data = "data";
  public static readonly string Mailto = "mailto";
  public static readonly string File = "file";
  public static readonly string Ws = "ws";
  public static readonly string Wss = "wss";
  public static readonly string Telnet = "telnet";
  public static readonly string Ssh = "ssh";
  public static readonly string Gopher = "gopher";
  public static readonly string Blob = "blob";
  private static readonly string[] RelativeProtocols = new string[7]
  {
    ProtocolNames.Http,
    ProtocolNames.Https,
    ProtocolNames.Ftp,
    ProtocolNames.File,
    ProtocolNames.Ws,
    ProtocolNames.Wss,
    ProtocolNames.Gopher
  };
  private static readonly string[] OriginalableProtocols = new string[6]
  {
    ProtocolNames.Http,
    ProtocolNames.Https,
    ProtocolNames.Ftp,
    ProtocolNames.Ws,
    ProtocolNames.Wss,
    ProtocolNames.Gopher
  };

  public static bool IsRelative(string protocol)
  {
    return StringExtensions.Contains(ProtocolNames.RelativeProtocols, protocol);
  }

  public static bool IsOriginable(string protocol)
  {
    return StringExtensions.Contains(ProtocolNames.OriginalableProtocols, protocol);
  }
}
