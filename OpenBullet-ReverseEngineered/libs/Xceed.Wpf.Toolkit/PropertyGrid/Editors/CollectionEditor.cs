// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.Editors.CollectionEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors;

public class CollectionEditor : TypeEditor<CollectionControlButton>
{
  protected override void SetValueDependencyProperty()
  {
    this.ValueProperty = CollectionControlButton.ItemsSourceProperty;
  }

  protected override CollectionControlButton CreateEditor()
  {
    return (CollectionControlButton) new PropertyGridEditorCollectionControl();
  }

  protected override void SetControlProperties(PropertyItem propertyItem)
  {
    if (!(propertyItem.ParentElement is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid parentElement))
      return;
    this.Editor.EditorDefinitions = parentElement.EditorDefinitions;
  }

  protected override void ResolveValueBinding(PropertyItem propertyItem)
  {
    Type propertyType = propertyItem.PropertyType;
    this.Editor.ItemsSourceType = propertyType;
    if (propertyType.BaseType == typeof (Array))
      this.Editor.NewItemTypes = (IList<Type>) new List<Type>()
      {
        propertyType.GetElementType()
      };
    else if (propertyItem.DescriptorDefinition != null && propertyItem.DescriptorDefinition.NewItemTypes != null && propertyItem.DescriptorDefinition.NewItemTypes.Count > 0)
    {
      this.Editor.NewItemTypes = propertyItem.DescriptorDefinition.NewItemTypes;
    }
    else
    {
      Type[] dictionaryItemsType = ListUtilities.GetDictionaryItemsType(propertyType);
      if (dictionaryItemsType != null && dictionaryItemsType.Length == 2)
      {
        this.Editor.NewItemTypes = (IList<Type>) new List<Type>()
        {
          ListUtilities.CreateEditableKeyValuePairType(dictionaryItemsType[0], dictionaryItemsType[1])
        };
      }
      else
      {
        Type listItemType = ListUtilities.GetListItemType(propertyType);
        if (listItemType != (Type) null)
        {
          this.Editor.NewItemTypes = (IList<Type>) new List<Type>()
          {
            listItemType
          };
        }
        else
        {
          Type collectionItemType = ListUtilities.GetCollectionItemType(propertyType);
          if (collectionItemType != (Type) null)
            this.Editor.NewItemTypes = (IList<Type>) new List<Type>()
            {
              collectionItemType
            };
        }
      }
    }
    base.ResolveValueBinding(propertyItem);
  }
}
