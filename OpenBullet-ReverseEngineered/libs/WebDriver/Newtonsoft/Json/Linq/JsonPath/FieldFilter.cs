// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.FieldFilter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class FieldFilter : PathFilter
{
  public string Name { get; set; }

  public override IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch)
  {
    foreach (JToken t in current)
    {
      if (t is JObject o)
      {
        if (this.Name != null)
        {
          JToken jtoken = o[this.Name];
          if (jtoken != null)
            yield return jtoken;
          else if (errorWhenNoMatch)
            throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this.Name));
        }
        else
        {
          foreach (KeyValuePair<string, JToken> keyValuePair in o)
            yield return keyValuePair.Value;
        }
      }
      else if (errorWhenNoMatch)
        throw new JsonException("Property '{0}' not valid on {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) (this.Name ?? "*"), (object) t.GetType().Name));
      o = (JObject) null;
    }
  }
}
