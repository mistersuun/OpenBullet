// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridHeadersVisibilityToVisibilityConverter
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Controls;

[Localizability(LocalizationCategory.NeverLocalize)]
internal sealed class DataGridHeadersVisibilityToVisibilityConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    bool flag = false;
    if (value is DataGridHeadersVisibility && parameter is DataGridHeadersVisibility headersVisibility)
    {
      switch ((DataGridHeadersVisibility) value)
      {
        case DataGridHeadersVisibility.Column:
          flag = headersVisibility == DataGridHeadersVisibility.Column || headersVisibility == DataGridHeadersVisibility.None;
          break;
        case DataGridHeadersVisibility.Row:
          flag = headersVisibility == DataGridHeadersVisibility.Row || headersVisibility == DataGridHeadersVisibility.None;
          break;
        case DataGridHeadersVisibility.All:
          flag = true;
          break;
      }
    }
    return (object) targetType == (object) typeof (Visibility) ? (object) (Visibility) (flag ? 0 : 2) : DependencyProperty.UnsetValue;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
