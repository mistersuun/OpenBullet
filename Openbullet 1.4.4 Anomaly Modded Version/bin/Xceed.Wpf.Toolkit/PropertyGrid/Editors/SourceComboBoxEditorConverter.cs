// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.SourceComboBoxEditorConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

internal class SourceComboBoxEditorConverter : IValueConverter
{
  private TypeConverter _typeConverter;

  internal SourceComboBoxEditorConverter(TypeConverter typeConverter)
  {
    this._typeConverter = typeConverter;
  }

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return this._typeConverter != null && this._typeConverter.CanConvertTo(typeof (string)) ? this._typeConverter.ConvertTo(value, typeof (string)) : value;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return this._typeConverter != null && this._typeConverter.CanConvertFrom(value.GetType()) ? this._typeConverter.ConvertFrom(value) : value;
  }
}
