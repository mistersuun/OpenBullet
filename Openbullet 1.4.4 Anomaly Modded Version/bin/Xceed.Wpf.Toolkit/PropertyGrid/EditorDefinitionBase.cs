// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.EditorDefinitionBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public abstract class EditorDefinitionBase : PropertyDefinitionBase
{
  internal EditorDefinitionBase()
  {
  }

  internal FrameworkElement GenerateEditingElementInternal(PropertyItemBase propertyItem)
  {
    return this.GenerateEditingElement(propertyItem);
  }

  protected virtual FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
  {
    return (FrameworkElement) null;
  }

  internal void UpdateProperty(
    FrameworkElement element,
    DependencyProperty elementProp,
    DependencyProperty definitionProperty)
  {
    object obj1 = this.GetValue(definitionProperty);
    object obj2 = this.ReadLocalValue(definitionProperty);
    object obj3 = element.GetValue(elementProp);
    bool flag = false;
    object unsetValue = DependencyProperty.UnsetValue;
    if (obj2 == unsetValue)
      return;
    if (obj3 != null && obj1 != null)
      flag = !obj3.GetType().IsValueType || !obj1.GetType().IsValueType ? obj1 == element.GetValue(elementProp) : obj3.Equals(obj1);
    if (!flag)
      element.SetValue(elementProp, obj1);
    else
      element.ClearValue(elementProp);
  }
}
