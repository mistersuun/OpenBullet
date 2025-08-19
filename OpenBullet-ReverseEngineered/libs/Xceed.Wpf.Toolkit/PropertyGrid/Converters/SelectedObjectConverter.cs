// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Converters.SelectedObjectConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Converters;

public class SelectedObjectConverter : IValueConverter
{
  private const string ValidParameterMessage = "parameter must be one of the following strings: 'Type', 'TypeName', 'SelectedObjectName'";

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (parameter == null)
      throw new ArgumentNullException(nameof (parameter));
    if (!(parameter is string))
      throw new ArgumentException("parameter must be one of the following strings: 'Type', 'TypeName', 'SelectedObjectName'");
    if (this.CompareParam(parameter, "Type"))
      return this.ConvertToType(value, culture);
    if (this.CompareParam(parameter, "TypeName"))
      return this.ConvertToTypeName(value, culture);
    if (this.CompareParam(parameter, "SelectedObjectName"))
      return this.ConvertToSelectedObjectName(value, culture);
    throw new ArgumentException("parameter must be one of the following strings: 'Type', 'TypeName', 'SelectedObjectName'");
  }

  private bool CompareParam(object parameter, string parameterValue)
  {
    return string.Compare((string) parameter, parameterValue, true) == 0;
  }

  private object ConvertToType(object value, CultureInfo culture)
  {
    return value == null ? (object) null : (object) value.GetType();
  }

  private object ConvertToTypeName(object value, CultureInfo culture)
  {
    if (value == null)
      return (object) string.Empty;
    Type type = value.GetType();
    if (type.GetInterface("ICustomTypeProvider", true) != (Type) null)
      type = type.GetMethod("GetCustomType").Invoke(value, (object[]) null) as Type;
    DisplayNameAttribute displayNameAttribute = type.GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault<DisplayNameAttribute>();
    return displayNameAttribute != null ? (object) displayNameAttribute.DisplayName : (object) type.Name;
  }

  private object ConvertToSelectedObjectName(object value, CultureInfo culture)
  {
    if (value == null)
      return (object) string.Empty;
    foreach (PropertyInfo property in value.GetType().GetProperties())
    {
      if (property.Name == "Name")
        return property.GetValue(value, (object[]) null);
    }
    return (object) string.Empty;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
