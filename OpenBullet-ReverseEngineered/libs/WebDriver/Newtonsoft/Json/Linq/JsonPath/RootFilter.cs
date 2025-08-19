// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.RootFilter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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
