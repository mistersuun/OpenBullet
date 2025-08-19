// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.SByteUpDownEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.ComponentModel;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

internal class SByteUpDownEditor : NumericUpDownEditor<SByteUpDown, sbyte?>
{
  protected override SByteUpDown CreateEditor()
  {
    return (SByteUpDown) new PropertyGridEditorSByteUpDown();
  }

  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    base.SetControlProperties(propertyItem);
    this.SetMinMaxFromRangeAttribute(propertyItem.PropertyDescriptor, TypeDescriptor.GetConverter(typeof (sbyte)));
  }
}
