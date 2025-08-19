// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.ItemsSourceAttributeEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class ItemsSourceAttributeEditor : TypeEditor<ComboBox>
{
  private readonly ItemsSourceAttribute _attribute;

  public ItemsSourceAttributeEditor(ItemsSourceAttribute attribute) => this._attribute = attribute;

  protected override void SetValueDependencyProperty()
  {
    this.ValueProperty = Selector.SelectedValueProperty;
  }

  protected override ComboBox CreateEditor() => (ComboBox) new PropertyGridEditorComboBox();

  protected override void ResolveValueBinding(PropertyItem propertyItem)
  {
    this.SetItemsSource();
    base.ResolveValueBinding(propertyItem);
  }

  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    this.Editor.DisplayMemberPath = "DisplayName";
    this.Editor.SelectedValuePath = "Value";
    if (propertyItem == null)
      return;
    this.Editor.IsEnabled = !propertyItem.IsReadOnly;
  }

  private void SetItemsSource() => this.Editor.ItemsSource = this.CreateItemsSource();

  private IEnumerable CreateItemsSource()
  {
    return (IEnumerable) (Activator.CreateInstance(this._attribute.Type) as IItemsSource).GetValues();
  }
}
