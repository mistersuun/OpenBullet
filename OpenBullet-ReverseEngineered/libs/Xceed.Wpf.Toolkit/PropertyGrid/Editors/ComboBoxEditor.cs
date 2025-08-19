// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.ComboBoxEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public abstract class ComboBoxEditor : TypeEditor<ComboBox>
{
  protected override void SetValueDependencyProperty()
  {
    this.ValueProperty = Selector.SelectedItemProperty;
  }

  protected override ComboBox CreateEditor() => (ComboBox) new PropertyGridEditorComboBox();

  protected override void ResolveValueBinding(PropertyItem propertyItem)
  {
    this.SetItemsSource(propertyItem);
    base.ResolveValueBinding(propertyItem);
  }

  protected abstract IEnumerable CreateItemsSource(PropertyItem propertyItem);

  private void SetItemsSource(PropertyItem propertyItem)
  {
    this.Editor.ItemsSource = this.CreateItemsSource(propertyItem);
  }
}
