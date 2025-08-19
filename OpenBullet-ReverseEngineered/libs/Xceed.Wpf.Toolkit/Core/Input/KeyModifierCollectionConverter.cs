// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Input.KeyModifierCollectionConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Security;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Input;

public sealed class KeyModifierCollectionConverter : TypeConverter
{
  private static readonly TypeConverter _keyModifierConverter = TypeDescriptor.GetConverter(typeof (KeyModifier));

  public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return KeyModifierCollectionConverter._keyModifierConverter.CanConvertFrom(typeDescriptorContext, type);
  }

  public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return type == typeof (InstanceDescriptor) || type == typeof (KeyModifierCollection) || type == typeof (string);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext typeDescriptorContext,
    CultureInfo cultureInfo,
    object value)
  {
    KeyModifierCollection modifierCollection = new KeyModifierCollection();
    string str1 = value as string;
    if (value == null || str1 != null && str1.Trim() == string.Empty)
    {
      modifierCollection.Add(KeyModifier.None);
    }
    else
    {
      string str2 = str1;
      char[] separator = new char[4]{ '+', ' ', '|', ',' };
      foreach (string str3 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        modifierCollection.Add((KeyModifier) KeyModifierCollectionConverter._keyModifierConverter.ConvertFrom(typeDescriptorContext, cultureInfo, (object) str3));
      if (modifierCollection.Count == 0)
        modifierCollection.Add(KeyModifier.None);
    }
    return (object) modifierCollection;
  }

  public override object ConvertTo(
    ITypeDescriptorContext typeDescriptorContext,
    CultureInfo cultureInfo,
    object value,
    Type destinationType)
  {
    if (value == null || ((Collection<KeyModifier>) value).Count == 0)
    {
      if (destinationType == typeof (InstanceDescriptor))
      {
        object obj = (object) null;
        try
        {
          obj = KeyModifierCollectionConverter.ConstructInstanceDescriptor();
        }
        catch (SecurityException ex)
        {
        }
        return obj;
      }
      if (destinationType == typeof (string))
        return KeyModifierCollectionConverter._keyModifierConverter.ConvertTo(typeDescriptorContext, cultureInfo, (object) KeyModifier.None, destinationType);
    }
    if (!(destinationType == typeof (string)))
      return (object) null;
    string empty = string.Empty;
    foreach (KeyModifier keyModifier in (Collection<KeyModifier>) value)
    {
      if (empty != string.Empty)
        empty += "+";
      empty += (string) KeyModifierCollectionConverter._keyModifierConverter.ConvertTo(typeDescriptorContext, cultureInfo, (object) keyModifier, destinationType);
    }
    return (object) empty;
  }

  private static object ConstructInstanceDescriptor()
  {
    return (object) new InstanceDescriptor((MemberInfo) typeof (KeyModifierCollection).GetConstructor(new Type[0]), (ICollection) new object[0]);
  }
}
