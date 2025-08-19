// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.WindowControlBackgroundConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class WindowControlBackgroundConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    Brush brush = (Brush) values[0];
    double num = (double) values[1];
    if (brush != null && brush.ReadLocalValue(Brush.OpacityProperty) == DependencyProperty.UnsetValue)
    {
      brush = brush.Clone();
      brush.Opacity = num;
    }
    return (object) brush;
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
