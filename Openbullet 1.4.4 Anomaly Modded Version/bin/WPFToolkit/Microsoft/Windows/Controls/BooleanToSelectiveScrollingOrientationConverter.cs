// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.BooleanToSelectiveScrollingOrientationConverter
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Controls;

[Localizability(LocalizationCategory.NeverLocalize)]
internal sealed class BooleanToSelectiveScrollingOrientationConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return value is bool && parameter is SelectiveScrollingOrientation scrollingOrientation && (bool) value ? (object) scrollingOrientation : (object) SelectiveScrollingOrientation.Both;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
