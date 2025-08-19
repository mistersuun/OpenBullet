// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextLocationConverter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public class TextLocationConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
  {
    return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
  }

  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    return destinationType == typeof (TextLocation) || base.CanConvertTo(context, destinationType);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value)
  {
    if (value is string)
    {
      string[] strArray = ((string) value).Split(';', ',');
      if (strArray.Length == 2)
        return (object) new TextLocation(int.Parse(strArray[0], (IFormatProvider) culture), int.Parse(strArray[1], (IFormatProvider) culture));
    }
    return base.ConvertFrom(context, culture, value);
  }

  public override object ConvertTo(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value,
    Type destinationType)
  {
    if (!(value is TextLocation) || !(destinationType == typeof (string)))
      return base.ConvertTo(context, culture, value, destinationType);
    TextLocation textLocation = (TextLocation) value;
    return (object) $"{textLocation.Line.ToString((IFormatProvider) culture)};{textLocation.Column.ToString((IFormatProvider) culture)}";
  }
}
