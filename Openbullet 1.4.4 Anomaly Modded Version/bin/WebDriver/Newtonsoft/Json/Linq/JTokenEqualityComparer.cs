// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JTokenEqualityComparer
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Linq;

internal class JTokenEqualityComparer : IEqualityComparer<JToken>
{
  public bool Equals(JToken x, JToken y) => JToken.DeepEquals(x, y);

  public int GetHashCode(JToken obj) => obj == null ? 0 : obj.GetDeepHashCode();
}
