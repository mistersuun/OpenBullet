// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Converters.BoolToVisibilityConverter
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Converters;

[ValueConversion(typeof (bool), typeof (Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is bool && targetType == typeof (Visibility))
    {
      if ((bool) value)
        return (object) Visibility.Visible;
      return parameter != null && parameter is Visibility ? parameter : (object) Visibility.Collapsed;
    }
    if (value != null)
      return (object) Visibility.Visible;
    return parameter != null && parameter is Visibility ? parameter : (object) Visibility.Collapsed;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (!(value is Visibility) || !(targetType == typeof (bool)))
      throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
    return (Visibility) value == Visibility.Visible ? (object) true : (object) false;
  }
}
