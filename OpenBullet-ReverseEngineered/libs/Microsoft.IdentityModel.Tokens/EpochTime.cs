// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.EpochTime
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public static class EpochTime
{
  public static readonly System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

  public static long GetIntDate(System.DateTime datetime)
  {
    System.DateTime dateTime = datetime;
    if (datetime.Kind != DateTimeKind.Utc)
      dateTime = datetime.ToUniversalTime();
    return dateTime.ToUniversalTime() <= EpochTime.UnixEpoch ? 0L : (long) (dateTime - EpochTime.UnixEpoch).TotalSeconds;
  }

  public static System.DateTime DateTime(long secondsSinceUnixEpoch)
  {
    if (secondsSinceUnixEpoch <= 0L)
      return EpochTime.UnixEpoch;
    return (double) secondsSinceUnixEpoch > TimeSpan.MaxValue.TotalSeconds ? DateTimeUtil.Add(EpochTime.UnixEpoch, TimeSpan.MaxValue).ToUniversalTime() : DateTimeUtil.Add(EpochTime.UnixEpoch, TimeSpan.FromSeconds((double) secondsSinceUnixEpoch)).ToUniversalTime();
  }
}
