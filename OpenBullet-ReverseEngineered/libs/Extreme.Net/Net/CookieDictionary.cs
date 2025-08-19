// Decompiled with JetBrains decompiler
// Type: Extreme.Net.CookieDictionary
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Extreme.Net;

public class CookieDictionary : Dictionary<string, string>
{
  public bool IsLocked { get; set; }

  public CookieDictionary(bool isLocked = false)
    : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    this.IsLocked = isLocked;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) this)
      stringBuilder.AppendFormat("{0}={1}; ", (object) keyValuePair.Key, (object) keyValuePair.Value);
    if (stringBuilder.Length > 0)
      stringBuilder.Remove(stringBuilder.Length - 2, 2);
    return stringBuilder.ToString();
  }
}
