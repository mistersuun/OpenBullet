// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IUrlUtilities
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("URLUtils")]
[DomNoInterfaceObject]
public interface IUrlUtilities
{
  [DomName("href")]
  string Href { get; set; }

  [DomName("protocol")]
  string Protocol { get; set; }

  [DomName("host")]
  string Host { get; set; }

  [DomName("hostname")]
  string HostName { get; set; }

  [DomName("port")]
  string Port { get; set; }

  [DomName("pathname")]
  string PathName { get; set; }

  [DomName("search")]
  string Search { get; set; }

  [DomName("hash")]
  string Hash { get; set; }

  [DomName("username")]
  string UserName { get; set; }

  [DomName("password")]
  string Password { get; set; }

  [DomName("origin")]
  string Origin { get; }
}
