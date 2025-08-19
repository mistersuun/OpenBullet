// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.ArrayMultipleIndexFilter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class ArrayMultipleIndexFilter : PathFilter
{
  public List<int> Indexes { get; set; }

  public override IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch)
  {
    foreach (JToken t in current)
    {
      foreach (int index in this.Indexes)
      {
        JToken tokenIndex = PathFilter.GetTokenIndex(t, errorWhenNoMatch, index);
        if (tokenIndex != null)
          yield return tokenIndex;
      }
    }
  }
}
