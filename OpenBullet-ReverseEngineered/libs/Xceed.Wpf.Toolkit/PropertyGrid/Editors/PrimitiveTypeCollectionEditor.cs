// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.PrimitiveTypeCollectionEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class PrimitiveTypeCollectionEditor : TypeEditor<PrimitiveTypeCollectionControl>
{
  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    this.Editor.BorderThickness = new Thickness(0.0);
    this.Editor.Content = (object) "(Collection)";
  }

  protected override void SetValueDependencyProperty()
  {
    this.ValueProperty = PrimitiveTypeCollectionControl.ItemsSourceProperty;
  }

  protected override PrimitiveTypeCollectionControl CreateEditor()
  {
    return (PrimitiveTypeCollectionControl) new PropertyGridEditorPrimitiveTypeCollectionControl();
  }

  protected override void ResolveValueBinding(PropertyItem propertyItem)
  {
    Type propertyType = propertyItem.PropertyType;
    this.Editor.ItemsSourceType = propertyType;
    if (propertyType.BaseType == typeof (Array))
    {
      this.Editor.ItemType = propertyType.GetElementType();
    }
    else
    {
      Type[] genericArguments = propertyType.GetGenericArguments();
      if (genericArguments.Length != 0)
        this.Editor.ItemType = genericArguments[0];
    }
    base.ResolveValueBinding(propertyItem);
  }
}
