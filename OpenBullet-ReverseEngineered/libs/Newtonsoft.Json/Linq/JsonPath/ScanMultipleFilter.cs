// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.ScanMultipleFilter
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class ScanMultipleFilter : PathFilter
{
  public List<string> Names { get; set; }

  public override IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch)
  {
    foreach (JToken c in current)
    {
      JToken value = c;
      while (true)
      {
        value = PathFilter.GetNextScanValue(c, (JToken) (value as JContainer), value);
        if (value != null)
        {
          if (value is JProperty property)
          {
            foreach (string name in this.Names)
            {
              if (property.Name == name)
                yield return property.Value;
            }
          }
          property = (JProperty) null;
        }
        else
          break;
      }
      value = (JToken) null;
    }
  }
}
