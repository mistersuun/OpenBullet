// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.ScanFilter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class ScanFilter : PathFilter
{
  public string Name { get; set; }

  public override IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch)
  {
    foreach (JToken c in current)
    {
      if (this.Name == null)
        yield return c;
      JToken value = c;
      JContainer container = c as JContainer;
      while (true)
      {
        value = PathFilter.GetNextScanValue(c, (JToken) container, value);
        if (value != null)
        {
          if (value is JProperty jproperty)
          {
            if (jproperty.Name == this.Name)
              yield return jproperty.Value;
          }
          else if (this.Name == null)
            yield return value;
          container = value as JContainer;
        }
        else
          break;
      }
      value = (JToken) null;
    }
  }
}
