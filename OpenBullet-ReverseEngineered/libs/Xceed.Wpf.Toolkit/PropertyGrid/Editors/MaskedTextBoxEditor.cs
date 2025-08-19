// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.MaskedTextBoxEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class MaskedTextBoxEditor : TypeEditor<MaskedTextBox>
{
  public string Mask { get; set; }

  public Type ValueDataType { get; set; }

  protected override MaskedTextBox CreateEditor()
  {
    return (MaskedTextBox) new PropertyGridEditorMaskedTextBox();
  }

  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    this.Editor.BorderThickness = new Thickness(0.0);
    this.Editor.ValueDataType = this.ValueDataType;
    this.Editor.Mask = this.Mask;
  }

  protected override void SetValueDependencyProperty()
  {
    this.ValueProperty = ValueRangeTextBox.ValueProperty;
  }
}
