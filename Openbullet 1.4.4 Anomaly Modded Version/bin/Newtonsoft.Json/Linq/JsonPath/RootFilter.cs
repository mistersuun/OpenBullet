// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.RootFilter
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class RootFilter : PathFilter
{
  public static readonly RootFilter Instance = new RootFilter();

  private RootFilter()
  {
  }

  public override IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch)
  {
    return (IEnumerable<JToken>) new JToken[1]{ root };
  }
}
