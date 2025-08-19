// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Time.Time
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Globalization;

#nullable disable
namespace RuriLib.Functions.Time;

public static class Time
{
  public static long ToUnixTimeSeconds(this DateTime dateTime)
  {
    return (long) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
  }

  public static long ToUnixTimeMilliseconds(this DateTime dateTime)
  {
    return (long) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
  }

  public static DateTime ToDateTime(this string time, string format)
  {
    return DateTime.ParseExact(time, format, (IFormatProvider) new CultureInfo("en-US"), DateTimeStyles.AllowWhiteSpaces);
  }

  public static DateTime ToDateTime(this double unixTime)
  {
    if (unixTime < 10000000000.0)
      unixTime *= 1000.0;
    return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTime).ToUniversalTime();
  }

  public static string ToISO8601(this DateTime dateTime)
  {
    return dateTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ");
  }
}
