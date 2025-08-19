// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.HalfConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class HalfConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    double num1 = (double) value;
    double num2 = parameter != null ? double.Parse((string) parameter) : 0.0;
    return num2 != 0.0 ? (object) (Math.Max(0.0, num1 - num2) / 2.0) : (object) (num1 / 2.0);
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
