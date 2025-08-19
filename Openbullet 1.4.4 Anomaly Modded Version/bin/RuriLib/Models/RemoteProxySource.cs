// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.RemoteProxySource
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using Newtonsoft.Json;

#nullable disable
namespace RuriLib.Models;

public class RemoteProxySource
{
  public RemoteProxySource(int id) => this.Id = id;

  public int Id { get; set; }

  public bool Active { get; set; } = true;

  public string Url { get; set; } = "";

  public ProxyType Type { get; set; }

  [JsonIgnore]
  public bool TypeInitialized { get; set; }

  public string Pattern { get; set; } = "([0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+:[0-9]+)";

  public string Output { get; set; } = "[1]";
}
