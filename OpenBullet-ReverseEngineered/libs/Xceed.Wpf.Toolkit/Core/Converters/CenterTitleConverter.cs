// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.CenterTitleConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class CenterTitleConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    double width = ((Size) values[0]).Width;
    double num = (double) values[1];
    ColumnDefinitionCollection definitionCollection = (ColumnDefinitionCollection) values[2];
    double actualWidth1 = definitionCollection[2].ActualWidth;
    double actualWidth2 = definitionCollection[3].ActualWidth;
    if (width + actualWidth2 * 2.0 < num)
      return (object) 1;
    return width < actualWidth1 ? (object) 2 : (object) 3;
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
