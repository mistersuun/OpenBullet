// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.IntegerUpDownEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.ComponentModel;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class IntegerUpDownEditor : NumericUpDownEditor<IntegerUpDown, int?>
{
  protected override IntegerUpDown CreateEditor()
  {
    return (IntegerUpDown) new PropertyGridEditorIntegerUpDown();
  }

  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    base.SetControlProperties(propertyItem);
    this.SetMinMaxFromRangeAttribute(propertyItem.PropertyDescriptor, TypeDescriptor.GetConverter(typeof (int)));
  }
}
