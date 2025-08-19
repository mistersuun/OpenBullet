// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Converters.UriSourceToBitmapImageConverter
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Converters;

public class UriSourceToBitmapImageConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null)
      return Binding.DoNothing;
    return (object) new Image()
    {
      Source = (ImageSource) new BitmapImage((Uri) value)
    };
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
