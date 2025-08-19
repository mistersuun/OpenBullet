// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Converters.EditorTimeSpanConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Converters;

public sealed class EditorTimeSpanConverter : IValueConverter
{
  public bool AllowNulls { get; set; }

  object IValueConverter.Convert(
    object value,
    Type targetType,
    object parameter,
    CultureInfo culture)
  {
    return this.AllowNulls && value == null ? (object) null : (object) (DateTime.Today + (value != null ? (TimeSpan) value : TimeSpan.Zero));
  }

  object IValueConverter.ConvertBack(
    object value,
    Type targetType,
    object parameter,
    CultureInfo culture)
  {
    return this.AllowNulls && value == null ? (object) null : (object) (value != null ? ((DateTime) value).TimeOfDay : TimeSpan.Zero);
  }
}
