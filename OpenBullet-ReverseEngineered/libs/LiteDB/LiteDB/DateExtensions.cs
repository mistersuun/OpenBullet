// Decompiled with JetBrains decompiler
// Type: LiteDB.DateExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

internal static class DateExtensions
{
  public static DateTime Truncate(this DateTime dt)
  {
    return dt == DateTime.MaxValue || dt == DateTime.MinValue ? dt : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, dt.Kind);
  }

  public static int MonthDifference(this DateTime startDate, DateTime endDate)
  {
    int num = endDate.Month + endDate.Year * 12 - (startDate.Month + startDate.Year * 12);
    double days = (double) (endDate - endDate.AddMonths(1)).Days;
    return Convert.ToInt32(Math.Truncate((double) num + (double) (startDate.Day - endDate.Day) / days));
  }

  public static int YearDifference(this DateTime startDate, DateTime endDate)
  {
    int num = endDate.Year - startDate.Year;
    if (startDate.Month == endDate.Month && endDate.Day < startDate.Day)
      --num;
    else if (endDate.Month < startDate.Month)
      --num;
    return num;
  }
}
