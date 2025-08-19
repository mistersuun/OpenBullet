// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.IntToThicknessConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class IntToThicknessConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    int num = 0;
    if (value != null)
      num = (int) value;
    return parameter != null && parameter.ToString().ToUpper() == "LEFT" ? (object) new Thickness((double) num, 0.0, 0.0, 0.0) : (object) new Thickness((double) num);
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
