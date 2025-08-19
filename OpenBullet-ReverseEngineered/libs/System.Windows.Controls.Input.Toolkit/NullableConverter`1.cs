// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.NullableConverter`1
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace System.Windows.Controls;

public class NullableConverter<T> : TypeConverter where T : struct
{
  public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
  {
    return (object) sourceType == (object) typeof (T) || (object) sourceType == (object) typeof (string);
  }

  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    return (object) destinationType == (object) typeof (T);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value)
  {
    string a = value as string;
    if (value is T obj)
      return (object) new T?(obj);
    return string.IsNullOrEmpty(a) || string.Equals(a, "Auto", StringComparison.OrdinalIgnoreCase) ? (object) new T?() : (object) new T?((T) Convert.ChangeType(value, typeof (T), (IFormatProvider) culture));
  }

  public override object ConvertTo(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value,
    Type destinationType)
  {
    if (value == null)
      return (object) string.Empty;
    return (object) destinationType == (object) typeof (string) ? (object) value.ToString() : base.ConvertTo(context, culture, value, destinationType);
  }
}
