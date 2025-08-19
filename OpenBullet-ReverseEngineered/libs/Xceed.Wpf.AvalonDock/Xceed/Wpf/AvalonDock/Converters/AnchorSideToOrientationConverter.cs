// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Converters.AnchorSideToOrientationConverter
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Converters;

[ValueConversion(typeof (AnchorSide), typeof (Orientation))]
public class AnchorSideToOrientationConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    switch ((AnchorSide) value)
    {
      case AnchorSide.Left:
      case AnchorSide.Right:
        return (object) Orientation.Vertical;
      default:
        return (object) Orientation.Horizontal;
    }
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
