// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.NumericUpDownEditor`2
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class NumericUpDownEditor<TEditor, TType> : UpDownEditor<TEditor, TType> where TEditor : UpDownBase<TType>, new()
{
  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    base.SetControlProperties(propertyItem);
    BindingOperations.SetBinding((DependencyObject) propertyItem, PropertyItem.IsInvalidProperty, (BindingBase) new Binding("IsInvalid")
    {
      Source = (object) this.Editor,
      UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
      Mode = BindingMode.TwoWay
    });
  }
}
