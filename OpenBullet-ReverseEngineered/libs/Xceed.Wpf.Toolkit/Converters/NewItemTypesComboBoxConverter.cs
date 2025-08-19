// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Converters.NewItemTypesComboBoxConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Converters;

public class NewItemTypesComboBoxConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values.Length != 2)
      throw new ArgumentException("The 'values' argument should contain 2 objects.");
    if (values[1] != null)
      return values[1].GetType().IsGenericType && (object) ((IEnumerable<Type>) values[1].GetType().GetGenericArguments()).First<Type>().GetType() != null ? values[1] : throw new ArgumentException("The 'value' argument is not of the correct type.");
    if (values[0] == null)
      return (object) null;
    if ((object) values[0].GetType() == null)
      throw new ArgumentException("The 'value' argument is not of the correct type.");
    List<Type> typeList = new List<Type>();
    Type listItemType = ListUtilities.GetListItemType((Type) values[0]);
    if (listItemType != (Type) null)
      typeList.Add(listItemType);
    return (object) typeList;
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
