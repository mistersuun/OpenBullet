// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.ObjectContainerHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

internal class ObjectContainerHelper : ObjectContainerHelperBase
{
  private object _selectedObject;

  public ObjectContainerHelper(IPropertyContainer propertyContainer, object selectedObject)
    : base(propertyContainer)
  {
    this._selectedObject = selectedObject;
  }

  private object SelectedObject => this._selectedObject;

  protected override string GetDefaultPropertyName()
  {
    return this.SelectedObject == null ? (string) null : ObjectContainerHelperBase.GetDefaultPropertyName(this.SelectedObject);
  }

  protected override void GenerateSubPropertiesCore(
    Action<IEnumerable<PropertyItem>> updatePropertyItemsCallback)
  {
    List<PropertyItem> propertyItemList = new List<PropertyItem>();
    if (this.SelectedObject != null)
    {
      try
      {
        List<PropertyDescriptor> propertyDescriptorList = new List<PropertyDescriptor>();
        foreach (PropertyDescriptor propertyDescriptor in ObjectContainerHelperBase.GetPropertyDescriptors(this.SelectedObject, this.PropertyContainer.HideInheritedProperties))
        {
          PropertyDefinition propertyDefinition = this.GetPropertyDefinition(propertyDescriptor);
          bool? nullable = this.PropertyContainer.IsPropertyVisible(propertyDescriptor);
          bool flag;
          if (nullable.HasValue)
          {
            flag = nullable.Value;
          }
          else
          {
            DisplayAttribute attribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(propertyDescriptor);
            if (attribute != null)
            {
              bool? autoGenerateField = attribute.GetAutoGenerateField();
              flag = this.PropertyContainer.AutoGenerateProperties && (autoGenerateField.HasValue && autoGenerateField.Value || !autoGenerateField.HasValue);
            }
            else
              flag = propertyDescriptor.IsBrowsable && this.PropertyContainer.AutoGenerateProperties;
            if (propertyDefinition != null)
              flag = propertyDefinition.IsBrowsable ?? flag;
          }
          if (flag)
          {
            PropertyItem propertyItem = this.CreatePropertyItem(propertyDescriptor, propertyDefinition);
            if (propertyItem != null)
              propertyItemList.Add(propertyItem);
          }
        }
      }
      catch (Exception ex)
      {
      }
    }
    updatePropertyItemsCallback((IEnumerable<PropertyItem>) propertyItemList);
  }

  private PropertyItem CreatePropertyItem(
    PropertyDescriptor property,
    PropertyDefinition propertyDef)
  {
    DescriptorPropertyDefinition propertyDefinition = new DescriptorPropertyDefinition(property, this.SelectedObject, this.PropertyContainer);
    propertyDefinition.InitProperties();
    this.InitializeDescriptorDefinition((DescriptorPropertyDefinitionBase) propertyDefinition, propertyDef);
    PropertyItem propertyItem = new PropertyItem((DescriptorPropertyDefinitionBase) propertyDefinition);
    propertyItem.Instance = this.SelectedObject;
    propertyItem.CategoryOrder = this.GetCategoryOrder((object) propertyDefinition.CategoryValue);
    propertyItem.WillRefreshPropertyGrid = this.GetWillRefreshPropertyGrid(property);
    return propertyItem;
  }

  private int GetCategoryOrder(object categoryValue)
  {
    if (categoryValue == null)
      return int.MaxValue;
    int categoryOrder = int.MaxValue;
    object selectedObject = this.SelectedObject;
    CategoryOrderAttribute categoryOrderAttribute = (selectedObject != null ? (IEnumerable<CategoryOrderAttribute>) selectedObject.GetType().GetCustomAttributes(typeof (CategoryOrderAttribute), true) : (IEnumerable<CategoryOrderAttribute>) new CategoryOrderAttribute[0]).FirstOrDefault<CategoryOrderAttribute>((Func<CategoryOrderAttribute, bool>) (a => object.Equals((object) a.CategoryValue, categoryValue)));
    if (categoryOrderAttribute != null)
      categoryOrder = categoryOrderAttribute.Order;
    return categoryOrder;
  }
}
