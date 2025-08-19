// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Converters.ListConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Converters;

internal class ListConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    return destinationType == typeof (string);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value)
  {
    if (value == null)
      return (object) null;
    string str1 = value as string;
    List<object> list = new List<object>();
    if (str1 == null && value != null)
    {
      list.Add(value);
    }
    else
    {
      if (str1 == null)
        return (object) null;
      string str2 = str1;
      char[] chArray = new char[1]{ ',' };
      foreach (string str3 in str2.Split(chArray))
        list.Add((object) str3.Trim());
    }
    return (object) new ReadOnlyCollection<object>((IList<object>) list);
  }

  public override object ConvertTo(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value,
    Type destinationType)
  {
    if (destinationType != typeof (string))
      throw new InvalidOperationException("Can only convert to string.");
    IList list = (IList) value;
    if (list == null)
      return (object) null;
    StringBuilder stringBuilder = new StringBuilder();
    bool flag = true;
    foreach (object obj in (IEnumerable) list)
    {
      if (obj == null)
        throw new InvalidOperationException("Property names cannot be null.");
      if (!(obj is string source))
        throw new InvalidOperationException("Does not support serialization of non-string property names.");
      if (source.Contains<char>(','))
        throw new InvalidOperationException("Property names cannot contain commas.");
      if (source.Trim().Length != source.Length)
        throw new InvalidOperationException("Property names cannot start or end with whitespace characters.");
      if (!flag)
        stringBuilder.Append(", ");
      flag = false;
      stringBuilder.Append(source);
    }
    return (object) stringBuilder.ToString();
  }
}
