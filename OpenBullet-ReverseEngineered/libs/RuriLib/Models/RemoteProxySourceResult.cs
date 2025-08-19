// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.RemoteProxySourceResult
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System.Collections.Generic;

#nullable disable
namespace RuriLib.Models;

public class RemoteProxySourceResult
{
  public bool Successful { get; set; }

  public string Error { get; set; }

  public string Url { get; set; }

  public List<CProxy> Proxies { get; set; }
}
