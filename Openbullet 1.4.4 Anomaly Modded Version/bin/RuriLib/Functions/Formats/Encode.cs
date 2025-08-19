// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Formats.Encode
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Text;

#nullable disable
namespace RuriLib.Functions.Formats;

public static class Encode
{
  public static string ToBase64(this string plainText)
  {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
  }

  public static string FromBase64(this string base64EncodedData)
  {
    string s = base64EncodedData.Replace(".", "");
    int num = s.Length % 4;
    if (num != 0)
      s = s.PadRight(s.Length + (4 - num), '=');
    return Encoding.UTF8.GetString(Convert.FromBase64String(s));
  }
}
