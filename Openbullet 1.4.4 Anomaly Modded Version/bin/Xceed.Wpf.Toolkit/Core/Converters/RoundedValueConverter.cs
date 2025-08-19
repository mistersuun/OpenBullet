// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.RoundedValueConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class RoundedValueConverter : IValueConverter
{
  private int _precision;

  public int Precision
  {
    get => this._precision;
    set => this._precision = value;
  }

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    switch (value)
    {
      case double num:
        return (object) Math.Round(num, this._precision);
      case Point point:
        return (object) new Point(Math.Round(point.X, this._precision), Math.Round(((Point) value).Y, this._precision));
      default:
        return value;
    }
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return value;
  }
}
