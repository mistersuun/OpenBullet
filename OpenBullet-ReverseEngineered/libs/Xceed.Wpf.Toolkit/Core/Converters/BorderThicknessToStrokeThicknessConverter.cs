// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.BorderThicknessToStrokeThicknessConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class BorderThicknessToStrokeThicknessConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    Thickness thickness = (Thickness) value;
    return (object) ((thickness.Bottom + thickness.Left + thickness.Right + thickness.Top) / 4.0);
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    int? nullable = (int?) value;
    int num = nullable.HasValue ? nullable.Value : 0;
    return (object) new Thickness((double) num, (double) num, (double) num, (double) num);
  }
}
