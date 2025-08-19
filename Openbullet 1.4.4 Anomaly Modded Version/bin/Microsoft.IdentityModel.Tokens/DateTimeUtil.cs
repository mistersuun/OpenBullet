// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.DateTimeUtil
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public static class DateTimeUtil
{
  public static DateTime Add(DateTime time, TimeSpan timespan)
  {
    if (timespan == TimeSpan.Zero)
      return time;
    if (timespan > TimeSpan.Zero && DateTime.MaxValue - time <= timespan)
      return DateTimeUtil.GetMaxValue(time.Kind);
    return timespan < TimeSpan.Zero && DateTime.MinValue - time >= timespan ? DateTimeUtil.GetMinValue(time.Kind) : time + timespan;
  }

  public static DateTime GetMaxValue(DateTimeKind kind)
  {
    return kind == DateTimeKind.Unspecified ? new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc) : new DateTime(DateTime.MaxValue.Ticks, kind);
  }

  public static DateTime GetMinValue(DateTimeKind kind)
  {
    return kind == DateTimeKind.Unspecified ? new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc) : new DateTime(DateTime.MinValue.Ticks, kind);
  }

  public static DateTime? ToUniversalTime(DateTime? value)
  {
    return !value.HasValue || value.Value.Kind == DateTimeKind.Utc ? value : new DateTime?(DateTimeUtil.ToUniversalTime(value.Value));
  }

  public static DateTime ToUniversalTime(DateTime value)
  {
    return value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
  }
}
