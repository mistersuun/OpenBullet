// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Converters.AnchorableContextMenuHideVisibilityConverter
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Converters;

public class AnchorableContextMenuHideVisibilityConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    return ((IEnumerable<object>) values).Count<object>() == 2 && values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue && values[1] is bool && (bool) values[1] ? (object) Visibility.Collapsed : values[0];
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
