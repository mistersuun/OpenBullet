// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.DateTimeUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class DateTimeUtilities
{
  public static DateTime GetContextNow(DateTimeKind kind)
  {
    if (kind == DateTimeKind.Unspecified)
      return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
    return kind != DateTimeKind.Utc ? DateTime.Now : DateTime.UtcNow;
  }

  public static bool IsSameDate(DateTime? date1, DateTime? date2)
  {
    return date1.HasValue && date2.HasValue && date1.Value.Date == date2.Value.Date;
  }
}
