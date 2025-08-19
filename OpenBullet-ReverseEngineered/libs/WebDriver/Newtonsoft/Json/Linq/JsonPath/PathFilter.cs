// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.PathFilter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal abstract class PathFilter
{
  public abstract IEnumerable<JToken> ExecuteFilter(
    JToken root,
    IEnumerable<JToken> current,
    bool errorWhenNoMatch);

  protected static JToken GetTokenIndex(JToken t, bool errorWhenNoMatch, int index)
  {
    JArray jarray = t as JArray;
    JConstructor jconstructor = t as JConstructor;
    if (jarray != null)
    {
      if (jarray.Count > index)
        return jarray[index];
      if (errorWhenNoMatch)
        throw new JsonException("Index {0} outside the bounds of JArray.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) index));
      return (JToken) null;
    }
    if (jconstructor != null)
    {
      if (jconstructor.Count > index)
        return jconstructor[(object) index];
      if (errorWhenNoMatch)
        throw new JsonException("Index {0} outside the bounds of JConstructor.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) index));
      return (JToken) null;
    }
    if (errorWhenNoMatch)
      throw new JsonException("Index {0} not valid on {1}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) index, (object) t.GetType().Name));
    return (JToken) null;
  }

  protected static JToken GetNextScanValue(JToken originalParent, JToken container, JToken value)
  {
    if (container != null && container.HasValues)
    {
      value = container.First;
    }
    else
    {
      while (value != null && value != originalParent && value == value.Parent.Last)
        value = (JToken) value.Parent;
      if (value == null || value == originalParent)
        return (JToken) null;
      value = value.Next;
    }
    return value;
  }
}
