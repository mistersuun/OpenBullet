// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.QueryScanFilter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class QueryScanFilter : PathFilter
{
  public QueryExpression Expression { get; set; }

  public override IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch)
  {
    foreach (JToken t1 in current)
    {
      if (t1 is JContainer jcontainer)
      {
        foreach (JToken t2 in jcontainer.DescendantsAndSelf())
        {
          if (this.Expression.IsMatch(root, t2))
            yield return t2;
        }
      }
      else if (this.Expression.IsMatch(root, t1))
        yield return t1;
    }
  }
}
