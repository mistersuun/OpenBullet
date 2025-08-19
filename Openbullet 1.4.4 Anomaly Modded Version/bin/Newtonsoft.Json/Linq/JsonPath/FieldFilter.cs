// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.FieldFilter
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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
    foreach (JToken jtoken1 in current)
    {
      if (jtoken1 is JObject jobject)
      {
        if (this.Name != null)
        {
          JToken jtoken2 = jobject[this.Name];
          if (jtoken2 != null)
            yield return jtoken2;
          else if (errorWhenNoMatch)
            throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) this.Name));
        }
        else
        {
          foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
            yield return keyValuePair.Value;
        }
      }
      else if (errorWhenNoMatch)
        throw new JsonException("Property '{0}' not valid on {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) (this.Name ?? "*"), (object) jtoken1.GetType().Name));
    }
  }
}
