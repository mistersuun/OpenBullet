// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.ThicknessSideRemovalConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class ThicknessSideRemovalConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    Thickness thickness = (Thickness) value;
    switch (int.Parse((string) parameter))
    {
      case 0:
        thickness.Left = 0.0;
        break;
      case 1:
        thickness.Top = 0.0;
        break;
      case 2:
        thickness.Right = 0.0;
        break;
      case 3:
        thickness.Bottom = 0.0;
        break;
      default:
        throw new InvalidContentException("parameter should be from 0 to 3 to specify the side to remove.");
    }
    return (object) thickness;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
