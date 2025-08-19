// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Converters.LayoutItemFromLayoutModelConverter
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Globalization;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Converters;

public class LayoutItemFromLayoutModelConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (!(value is LayoutContent content))
      return (object) null;
    if (content.Root == null)
      return (object) null;
    return content.Root.Manager == null ? (object) null : (object) content.Root.Manager.GetLayoutItemFromModel(content) ?? Binding.DoNothing;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
