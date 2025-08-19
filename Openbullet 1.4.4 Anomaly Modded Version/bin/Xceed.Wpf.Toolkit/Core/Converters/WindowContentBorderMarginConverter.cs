// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.WindowContentBorderMarginConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class WindowContentBorderMarginConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    double num = (double) values[0];
    double bottom = (double) values[1];
    switch ((string) parameter)
    {
      case "0":
        return (object) new Thickness(num, 0.0, num, bottom);
      case "1":
        return (object) new Thickness(0.0, 0.0, num, bottom);
      case "2":
        return (object) new Thickness(0.0, 0.0, num, 0.0);
      default:
        throw new NotSupportedException("'parameter' for WindowContentBorderMarginConverter is not valid.");
    }
  }

  public object[] ConvertBack(
    object value,
    Type[] targetTypes,
    object parameter,
    CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
