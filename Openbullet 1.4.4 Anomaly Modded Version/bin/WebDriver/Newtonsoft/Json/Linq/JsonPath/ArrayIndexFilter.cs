// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.ArrayIndexFilter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class ArrayIndexFilter : PathFilter
{
  public int? Index { get; set; }

  public override IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch)
  {
    foreach (JToken t1 in current)
    {
      int? index = this.Index;
      if (index.HasValue)
      {
        JToken t2 = t1;
        int num = errorWhenNoMatch ? 1 : 0;
        index = this.Index;
        int valueOrDefault = index.GetValueOrDefault();
        JToken tokenIndex = PathFilter.GetTokenIndex(t2, num != 0, valueOrDefault);
        if (tokenIndex != null)
          yield return tokenIndex;
      }
      else if (t1 is JArray || t1 is JConstructor)
      {
        foreach (JToken jtoken in (IEnumerable<JToken>) t1)
          yield return jtoken;
      }
      else if (errorWhenNoMatch)
        throw new JsonException("Index * not valid on {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) t1.GetType().Name));
    }
  }
}
