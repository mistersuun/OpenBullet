// Decompiled with JetBrains decompiler
// Type: LiteDB.StorageUnitHelper
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

internal class StorageUnitHelper
{
  public static long ParseFileSize(string size)
  {
    Match match = Regex.Match(size, "^(\\d+)\\s*([tgmk])?(b|byte|bytes)?$", RegexOptions.IgnoreCase);
    if (!match.Success)
      return 0;
    long int64 = Convert.ToInt64(match.Groups[1].Value);
    switch (match.Groups[2].Value.ToLower())
    {
      case "t":
        return int64 * 1024L /*0x0400*/ * 1024L /*0x0400*/ * 1024L /*0x0400*/ * 1024L /*0x0400*/;
      case "g":
        return int64 * 1024L /*0x0400*/ * 1024L /*0x0400*/ * 1024L /*0x0400*/;
      case "m":
        return int64 * 1024L /*0x0400*/ * 1024L /*0x0400*/;
      case "k":
        return int64 * 1024L /*0x0400*/;
      case "":
        return int64;
      default:
        return 0;
    }
  }

  public static string FormatFileSize(long byteCount)
  {
    string[] strArray = new string[5]
    {
      "B",
      "KB",
      "MB",
      "GB",
      "TB"
    };
    if (byteCount == 0L)
      return "0" + strArray[0];
    long num1;
    int int32 = Convert.ToInt32(Math.Floor(Math.Log((double) (num1 = Math.Abs(byteCount)), 1024.0)));
    double num2 = Math.Round((double) num1 / Math.Pow(1024.0, (double) int32), 1);
    return ((double) Math.Sign(byteCount) * num2).ToString() + strArray[int32];
  }
}
