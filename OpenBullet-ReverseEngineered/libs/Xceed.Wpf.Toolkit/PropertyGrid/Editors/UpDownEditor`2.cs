// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.UpDownEditor`2
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class UpDownEditor<TEditor, TType> : TypeEditor<TEditor> where TEditor : UpDownBase<TType>, new()
{
  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    this.Editor.TextAlignment = TextAlignment.Left;
  }

  protected override void SetValueDependencyProperty()
  {
    this.ValueProperty = UpDownBase<TType>.ValueProperty;
  }

  internal void SetMinMaxFromRangeAttribute(
    PropertyDescriptor propertyDescriptor,
    TypeConverter converter)
  {
    if (propertyDescriptor == null)
      return;
    RangeAttribute attribute = PropertyGridUtilities.GetAttribute<RangeAttribute>(propertyDescriptor);
    if (attribute == null)
      return;
    this.Editor.Maximum = (TType) converter.ConvertFrom((object) attribute.Maximum.ToString());
    this.Editor.Minimum = (TType) converter.ConvertFrom((object) attribute.Minimum.ToString());
  }
}
