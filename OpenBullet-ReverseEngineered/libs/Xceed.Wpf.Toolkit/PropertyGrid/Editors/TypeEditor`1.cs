// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.TypeEditor`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public abstract class TypeEditor<T> : ITypeEditor where T : FrameworkElement, new()
{
  protected T Editor { get; set; }

  protected DependencyProperty ValueProperty { get; set; }

  public virtual FrameworkElement ResolveEditor(PropertyItem propertyItem)
  {
    this.Editor = this.CreateEditor();
    this.SetValueDependencyProperty();
    this.SetControlProperties(propertyItem);
    this.ResolveValueBinding(propertyItem);
    return (FrameworkElement) this.Editor;
  }

  protected virtual T CreateEditor() => new T();

  protected virtual IValueConverter CreateValueConverter() => (IValueConverter) null;

  protected virtual void ResolveValueBinding(PropertyItem propertyItem)
  {
    BindingOperations.SetBinding((DependencyObject) this.Editor, this.ValueProperty, (BindingBase) new Binding("Value")
    {
      Source = (object) propertyItem,
      UpdateSourceTrigger = ((object) this.Editor is InputBase ? UpdateSourceTrigger.PropertyChanged : UpdateSourceTrigger.Default),
      Mode = (propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay),
      Converter = this.CreateValueConverter()
    });
  }

  protected virtual void SetControlProperties(PropertyItem propertyItem)
  {
  }

  protected abstract void SetValueDependencyProperty();
}
