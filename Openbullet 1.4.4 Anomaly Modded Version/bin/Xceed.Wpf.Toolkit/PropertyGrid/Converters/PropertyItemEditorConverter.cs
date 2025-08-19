// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Converters.PropertyItemEditorConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Converters;

public class PropertyItemEditorConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values == null || values.Length != 2)
      return (object) null;
    object editor = values[0];
    bool? nullable1 = values[1] as bool?;
    if (editor == null || !nullable1.HasValue)
      return editor;
    Type type = editor.GetType();
    PropertyInfo property1 = type.GetProperty("IsReadOnly");
    if (property1 != (PropertyInfo) null)
    {
      if (!this.IsPropertySetLocally(editor, TextBoxBase.IsReadOnlyProperty))
        property1.SetValue(editor, (object) nullable1, (object[]) null);
    }
    else
    {
      PropertyInfo property2 = type.GetProperty("IsEnabled");
      if (property2 != (PropertyInfo) null && !this.IsPropertySetLocally(editor, UIElement.IsEnabledProperty))
      {
        PropertyInfo propertyInfo = property2;
        object obj = editor;
        bool? nullable2 = nullable1;
        // ISSUE: variable of a boxed type
        __Boxed<bool?> local = (ValueType) (nullable2.HasValue ? new bool?(!nullable2.GetValueOrDefault()) : new bool?());
        propertyInfo.SetValue(obj, (object) local, (object[]) null);
      }
    }
    return editor;
  }

  public object[] ConvertBack(
    object value,
    Type[] targetTypes,
    object parameter,
    CultureInfo culture)
  {
    throw new NotImplementedException();
  }

  private bool IsPropertySetLocally(object editor, DependencyProperty dp)
  {
    return dp != null && editor is DependencyObject dependencyObject && DependencyPropertyHelper.GetValueSource(dependencyObject, dp).BaseValueSource == BaseValueSource.Local;
  }
}
