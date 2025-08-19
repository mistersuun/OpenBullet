// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.ObjectTypeToNameConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class ObjectTypeToNameConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null)
      return (object) null;
    if ((object) (value as Type) != null)
    {
      DisplayNameAttribute displayNameAttribute = ((Type) value).GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault<DisplayNameAttribute>();
      return displayNameAttribute == null ? (object) ((Type) value).Name : (object) displayNameAttribute.DisplayName;
    }
    Type type = value.GetType();
    string str = value.ToString();
    if (!string.IsNullOrEmpty(str) && !(str == type.UnderlyingSystemType.ToString()))
      return value;
    DisplayNameAttribute displayNameAttribute1 = type.GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault<DisplayNameAttribute>();
    return displayNameAttribute1 == null ? (object) type.Name : (object) displayNameAttribute1.DisplayName;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
