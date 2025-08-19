// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.EnumComboBoxEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class EnumComboBoxEditor : ComboBoxEditor
{
  protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
  {
    return (IEnumerable) EnumComboBoxEditor.GetValues(propertyItem.PropertyType);
  }

  private static object[] GetValues(Type enumType)
  {
    List<object> objectList = new List<object>();
    if (enumType != (Type) null)
    {
      foreach (FieldInfo fieldInfo in ((IEnumerable<FieldInfo>) enumType.GetFields()).Where<FieldInfo>((Func<FieldInfo, bool>) (x => x.IsLiteral)))
      {
        object[] customAttributes = fieldInfo.GetCustomAttributes(typeof (BrowsableAttribute), false);
        if (customAttributes.Length != 1 || ((BrowsableAttribute) customAttributes[0]).Browsable)
          objectList.Add(fieldInfo.GetValue((object) enumType));
      }
    }
    return objectList.ToArray();
  }
}
