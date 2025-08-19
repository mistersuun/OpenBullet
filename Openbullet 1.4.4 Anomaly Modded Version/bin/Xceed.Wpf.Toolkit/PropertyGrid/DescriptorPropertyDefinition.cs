// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.DescriptorPropertyDefinition
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

internal class DescriptorPropertyDefinition : DescriptorPropertyDefinitionBase
{
  private object _selectedObject;
  private PropertyDescriptor _propertyDescriptor;
  private DependencyPropertyDescriptor _dpDescriptor;

  internal DescriptorPropertyDefinition(
    PropertyDescriptor propertyDescriptor,
    object selectedObject,
    IPropertyContainer propertyContainer)
    : base(propertyContainer.IsCategorized)
  {
    this.Init(propertyDescriptor, selectedObject);
  }

  internal override PropertyDescriptor PropertyDescriptor => this._propertyDescriptor;

  private object SelectedObject => this._selectedObject;

  internal override ObjectContainerHelperBase CreateContainerHelper(IPropertyContainer parent)
  {
    return (ObjectContainerHelperBase) new ObjectContainerHelper(parent, this.Value);
  }

  internal override void OnValueChanged(object oldValue, object newValue)
  {
    base.OnValueChanged(oldValue, newValue);
    this.RaiseContainerHelperInvalidated();
  }

  protected override BindingBase CreateValueBinding()
  {
    object selectedObject = this.SelectedObject;
    return (BindingBase) new Binding(this.PropertyDescriptor.Name)
    {
      Source = this.GetValueInstance(selectedObject),
      Mode = (this.PropertyDescriptor.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay),
      ValidatesOnDataErrors = true,
      ValidatesOnExceptions = true,
      ConverterCulture = CultureInfo.CurrentCulture
    };
  }

  protected override bool ComputeIsReadOnly() => this.PropertyDescriptor.IsReadOnly;

  internal override ITypeEditor CreateDefaultEditor(PropertyItem propertyItem)
  {
    return PropertyGridUtilities.CreateDefaultEditor(this.PropertyDescriptor.PropertyType, this.PropertyDescriptor.Converter, propertyItem);
  }

  protected override bool ComputeCanResetValue()
  {
    if (this.PropertyDescriptor.IsReadOnly)
      return false;
    object defaultValueAttribute = this.ComputeDefaultValueAttribute();
    return defaultValueAttribute != null ? !defaultValueAttribute.Equals(this.Value) : this.PropertyDescriptor.CanResetValue(this.SelectedObject);
  }

  protected override object ComputeAdvancedOptionsTooltip()
  {
    object tooltip;
    this.UpdateAdvanceOptionsForItem(this.SelectedObject as DependencyObject, this._dpDescriptor, out tooltip);
    return tooltip;
  }

  protected override string ComputeCategory()
  {
    DisplayAttribute attribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(this.PropertyDescriptor);
    return attribute == null || attribute.GetGroupName() == null ? this.PropertyDescriptor.Category : attribute.GetGroupName();
  }

  protected override string ComputeCategoryValue() => this.PropertyDescriptor.Category;

  protected override bool ComputeExpandableAttribute()
  {
    return (bool) this.ComputeExpandableAttributeForItem((object) this.PropertyDescriptor);
  }

  protected override object ComputeDefaultValueAttribute()
  {
    return this.ComputeDefaultValueAttributeForItem((object) this.PropertyDescriptor);
  }

  protected override bool ComputeIsExpandable() => this.Value != null;

  protected override IList<Type> ComputeNewItemTypes()
  {
    return (IList<Type>) this.ComputeNewItemTypesForItem((object) this.PropertyDescriptor);
  }

  protected override string ComputeDescription()
  {
    return (string) this.ComputeDescriptionForItem((object) this.PropertyDescriptor);
  }

  protected override int ComputeDisplayOrder(bool isPropertyGridCategorized)
  {
    this.IsPropertyGridCategorized = isPropertyGridCategorized;
    return (int) this.ComputeDisplayOrderForItem((object) this.PropertyDescriptor);
  }

  protected override void ResetValue()
  {
    this.PropertyDescriptor.ResetValue(this.SelectedObject);
    base.ResetValue();
  }

  internal override ITypeEditor CreateAttributeEditor()
  {
    EditorAttribute attribute1 = this.GetAttribute<EditorAttribute>();
    if (attribute1 != null)
    {
      Type type;
      try
      {
        type = Type.GetType(attribute1.EditorTypeName, (Func<AssemblyName, Assembly>) (name => ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Where<Assembly>((Func<Assembly, bool>) (l => l.FullName == name.FullName)).FirstOrDefault<Assembly>()), (Func<Assembly, string, bool, Type>) null, true);
      }
      catch (Exception ex)
      {
        type = Type.GetType(attribute1.EditorTypeName);
      }
      if (typeof (ITypeEditor).IsAssignableFrom(type) && type.GetConstructor(new Type[0]) != (ConstructorInfo) null && Activator.CreateInstance(type) is ITypeEditor instance)
        return instance;
    }
    ItemsSourceAttribute attribute2 = this.GetAttribute<ItemsSourceAttribute>();
    return attribute2 != null ? (ITypeEditor) new ItemsSourceAttributeEditor(attribute2) : (ITypeEditor) null;
  }

  private T GetAttribute<T>() where T : Attribute
  {
    return PropertyGridUtilities.GetAttribute<T>(this.PropertyDescriptor);
  }

  private void Init(PropertyDescriptor propertyDescriptor, object selectedObject)
  {
    if (propertyDescriptor == null)
      throw new ArgumentNullException(nameof (propertyDescriptor));
    if (selectedObject == null)
      throw new ArgumentNullException(nameof (selectedObject));
    this._propertyDescriptor = propertyDescriptor;
    this._selectedObject = selectedObject;
    this._dpDescriptor = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);
  }
}
