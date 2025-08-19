// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.SR
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Globalization;
using System.Resources;

#nullable disable
namespace Microsoft.Windows.Controls;

internal static class SR
{
  private static ResourceManager _resourceManager = new ResourceManager("ExceptionStringTable", typeof (SR).Assembly);

  internal static string Get(SRID id) => SR._resourceManager.GetString(id.String);

  internal static string Get(SRID id, params object[] args)
  {
    string format = SR._resourceManager.GetString(id.String);
    if (format != null && args != null && args.Length > 0)
      format = string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    return format;
  }
}
