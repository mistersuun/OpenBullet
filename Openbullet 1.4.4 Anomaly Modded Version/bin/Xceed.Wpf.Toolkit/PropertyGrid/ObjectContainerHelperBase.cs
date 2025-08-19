// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.ObjectContainerHelperBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

internal abstract class ObjectContainerHelperBase : ContainerHelperBase
{
  private bool _isPreparingItemFlag;
  private PropertyItemCollection _propertyItemCollection;

  public ObjectContainerHelperBase(IPropertyContainer propertyContainer)
    : base(propertyContainer)
  {
    this._propertyItemCollection = new PropertyItemCollection(new ObservableCollection<PropertyItem>());
    this.UpdateFilter();
    this.UpdateCategorization(false);
  }

  public override IList Properties => (IList) this._propertyItemCollection;

  private PropertyItem DefaultProperty
  {
    get
    {
      PropertyItem defaultProperty = (PropertyItem) null;
      string defaultName = this.GetDefaultPropertyName();
      if (defaultName != null)
        defaultProperty = this._propertyItemCollection.FirstOrDefault<PropertyItem>((Func<PropertyItem, bool>) (prop => object.Equals((object) defaultName, (object) prop.PropertyDescriptor.Name)));
      return defaultProperty;
    }
  }

  protected PropertyItemCollection PropertyItems => this._propertyItemCollection;

  public override PropertyItemBase ContainerFromItem(object item)
  {
    if (item == null)
      return (PropertyItemBase) null;
    if (item is PropertyItem propertyItem)
      return (PropertyItemBase) propertyItem;
    string propertyStr = item as string;
    return propertyStr != null ? (PropertyItemBase) this.PropertyItems.FirstOrDefault<PropertyItem>((Func<PropertyItem, bool>) (prop => propertyStr == prop.PropertyDescriptor.Name)) : (PropertyItemBase) null;
  }

  public override object ItemFromContainer(PropertyItemBase container)
  {
    return !(container is PropertyItem propertyItem) ? (object) null : (object) propertyItem.PropertyDescriptor.Name;
  }

  public override void UpdateValuesFromSource()
  {
    foreach (PropertyItem propertyItem in (ReadOnlyCollection<PropertyItem>) this.PropertyItems)
    {
      propertyItem.DescriptorDefinition.UpdateValueFromSource();
      propertyItem.ContainerHelper.UpdateValuesFromSource();
    }
  }

  public void GenerateProperties()
  {
    if (this.PropertyItems.Count != 0)
      return;
    this.RegenerateProperties();
  }

  protected override void OnFilterChanged() => this.UpdateFilter();

  protected override void OnCategorizationChanged() => this.UpdateCategorization(true);

  protected override void OnAutoGeneratePropertiesChanged() => this.RegenerateProperties();

  protected override void OnHideInheritedPropertiesChanged() => this.RegenerateProperties();

  protected override void OnEditorDefinitionsChanged() => this.RegenerateProperties();

  protected override void OnPropertyDefinitionsChanged() => this.RegenerateProperties();

  protected internal override void SetPropertiesExpansion(bool isExpanded)
  {
    if (this.Properties.Count == 0)
      this.GenerateProperties();
    base.SetPropertiesExpansion(isExpanded);
  }

  protected internal override void SetPropertiesExpansion(string propertyName, bool isExpanded)
  {
    if (this.Properties.Count == 0)
      this.GenerateProperties();
    base.SetPropertiesExpansion(propertyName, isExpanded);
  }

  private void UpdateFilter()
  {
    FilterInfo filterInfo = this.PropertyContainer.FilterInfo;
    this.PropertyItems.FilterPredicate = filterInfo.Predicate ?? PropertyItemCollection.CreateFilter(filterInfo.InputString, (IList<PropertyItem>) this.PropertyItems, this.PropertyContainer);
  }

  private void UpdateCategorization(bool updateSubPropertiesCategorization)
  {
    this._propertyItemCollection.UpdateCategorization(this.ComputeCategoryGroupDescription(), this.PropertyContainer.IsCategorized, this.PropertyContainer.IsSortedAlphabetically);
    if (!updateSubPropertiesCategorization || this._propertyItemCollection.Count <= 0)
      return;
    foreach (PropertyItemBase propertyItem in (ReadOnlyCollection<PropertyItem>) this._propertyItemCollection)
    {
      if (propertyItem.Properties is PropertyItemCollection properties)
        properties.UpdateCategorization(this.ComputeCategoryGroupDescription(), this.PropertyContainer.IsCategorized, this.PropertyContainer.IsSortedAlphabetically);
    }
  }

  private GroupDescription ComputeCategoryGroupDescription()
  {
    return !this.PropertyContainer.IsCategorized ? (GroupDescription) null : (GroupDescription) new PropertyGroupDescription(PropertyItemCollection.CategoryPropertyName);
  }

  private string GetCategoryGroupingPropertyName()
  {
    return !(this.ComputeCategoryGroupDescription() is PropertyGroupDescription groupDescription) ? (string) null : groupDescription.PropertyName;
  }

  private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (!ObjectContainerHelperBase.IsItemOrderingProperty(e.PropertyName) && !(this.GetCategoryGroupingPropertyName() == e.PropertyName) || this.ChildrenItemsControl.ItemContainerGenerator.Status == GeneratorStatus.GeneratingContainers || this._isPreparingItemFlag)
      return;
    this.PropertyItems.RefreshView();
  }

  protected abstract string GetDefaultPropertyName();

  protected abstract void GenerateSubPropertiesCore(
    Action<IEnumerable<PropertyItem>> updatePropertyItemsCallback);

  private void RegenerateProperties()
  {
    this.GenerateSubPropertiesCore(new Action<IEnumerable<PropertyItem>>(this.UpdatePropertyItemsCallback));
  }

  private void UpdatePropertyItemsCallback(IEnumerable<PropertyItem> subProperties)
  {
    foreach (PropertyItem subProperty in subProperties)
      this.InitializePropertyItem(subProperty);
    foreach (PropertyItemBase propertyItem in (ReadOnlyCollection<PropertyItem>) this.PropertyItems)
      propertyItem.PropertyChanged -= new PropertyChangedEventHandler(this.OnChildrenPropertyChanged);
    this.PropertyItems.UpdateItems(subProperties);
    foreach (PropertyItemBase propertyItem in (ReadOnlyCollection<PropertyItem>) this.PropertyItems)
      propertyItem.PropertyChanged += new PropertyChangedEventHandler(this.OnChildrenPropertyChanged);
    if (!(this.PropertyContainer is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyContainer))
      return;
    propertyContainer.SelectedPropertyItem = (PropertyItemBase) this.DefaultProperty;
  }

  protected static List<PropertyDescriptor> GetPropertyDescriptors(
    object instance,
    bool hideInheritedProperties)
  {
    TypeConverter converter = TypeDescriptor.GetConverter(instance);
    PropertyDescriptorCollection source1 = converter == null || !converter.GetPropertiesSupported() ? (!(instance is ICustomTypeDescriptor) ? (!(instance.GetType().GetInterface("ICustomTypeProvider", true) != (Type) null) ? TypeDescriptor.GetProperties(instance.GetType()) : TypeDescriptor.GetProperties(instance.GetType().GetMethod("GetCustomType").Invoke(instance, (object[]) null) as Type)) : ((ICustomTypeDescriptor) instance).GetProperties()) : converter.GetProperties(instance);
    if (source1 == null)
      return (List<PropertyDescriptor>) null;
    IEnumerable<PropertyDescriptor> source2 = source1.Cast<PropertyDescriptor>();
    return hideInheritedProperties ? source2.Where<PropertyDescriptor>((Func<PropertyDescriptor, bool>) (p => p.ComponentType == instance.GetType())).ToList<PropertyDescriptor>() : source2.ToList<PropertyDescriptor>();
  }

  protected bool GetWillRefreshPropertyGrid(PropertyDescriptor propertyDescriptor)
  {
    if (propertyDescriptor == null)
      return false;
    RefreshPropertiesAttribute attribute = PropertyGridUtilities.GetAttribute<RefreshPropertiesAttribute>(propertyDescriptor);
    return attribute != null && attribute.RefreshProperties != 0;
  }

  internal void InitializeDescriptorDefinition(
    DescriptorPropertyDefinitionBase descriptorDef,
    PropertyDefinition propertyDefinition)
  {
    if (descriptorDef == null)
      throw new ArgumentNullException(nameof (descriptorDef));
    if (propertyDefinition == null || propertyDefinition == null)
      return;
    if (propertyDefinition.Category != null)
    {
      descriptorDef.Category = propertyDefinition.Category;
      descriptorDef.CategoryValue = propertyDefinition.Category;
    }
    if (propertyDefinition.Description != null)
      descriptorDef.Description = propertyDefinition.Description;
    if (propertyDefinition.DisplayName != null)
      descriptorDef.DisplayName = propertyDefinition.DisplayName;
    if (propertyDefinition.DisplayOrder.HasValue)
      descriptorDef.DisplayOrder = propertyDefinition.DisplayOrder.Value;
    if (!propertyDefinition.IsExpandable.HasValue)
      return;
    descriptorDef.ExpandableAttribute = propertyDefinition.IsExpandable.Value;
  }

  private void InitializePropertyItem(PropertyItem propertyItem)
  {
    DescriptorPropertyDefinitionBase pd = propertyItem.DescriptorDefinition;
    propertyItem.PropertyDescriptor = pd.PropertyDescriptor;
    propertyItem.IsReadOnly = pd.IsReadOnly;
    propertyItem.DisplayName = pd.DisplayName;
    propertyItem.Description = pd.Description;
    propertyItem.Category = pd.Category;
    propertyItem.PropertyOrder = pd.DisplayOrder;
    if (pd.PropertyDescriptor.Converter is ExpandableObjectConverter)
      propertyItem.IsExpandable = true;
    else
      this.SetupDefinitionBinding<bool>(propertyItem, PropertyItemBase.IsExpandableProperty, pd, (Expression<Func<bool>>) (() => pd.IsExpandable), BindingMode.OneWay);
    this.SetupDefinitionBinding<ImageSource>(propertyItem, PropertyItemBase.AdvancedOptionsIconProperty, pd, (Expression<Func<ImageSource>>) (() => pd.AdvancedOptionsIcon), BindingMode.OneWay);
    this.SetupDefinitionBinding<object>(propertyItem, PropertyItemBase.AdvancedOptionsTooltipProperty, pd, (Expression<Func<object>>) (() => pd.AdvancedOptionsTooltip), BindingMode.OneWay);
    this.SetupDefinitionBinding<object>(propertyItem, CustomPropertyItem.ValueProperty, pd, (Expression<Func<object>>) (() => pd.Value), BindingMode.TwoWay);
    if (pd.CommandBindings != null)
    {
      foreach (CommandBinding commandBinding in pd.CommandBindings)
        propertyItem.CommandBindings.Add(commandBinding);
    }
    if (pd.DefaultValue == null)
      return;
    object typeDefaultValue = this.GetTypeDefaultValue(propertyItem.PropertyType);
    if ((propertyItem.Value == null || !propertyItem.Value.Equals(typeDefaultValue)) && (propertyItem.Value != null || typeDefaultValue != propertyItem.Value))
      return;
    propertyItem.SetCurrentValue(CustomPropertyItem.ValueProperty, pd.DefaultValue);
  }

  private object GetTypeDefaultValue(Type type)
  {
    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
      type = type.GetProperty("Value").PropertyType;
    return !type.IsValueType ? (object) null : Activator.CreateInstance(type);
  }

  private void SetupDefinitionBinding<T>(
    PropertyItem propertyItem,
    DependencyProperty itemProperty,
    DescriptorPropertyDefinitionBase pd,
    Expression<Func<T>> definitionProperty,
    BindingMode bindingMode)
  {
    Binding binding = new Binding(ReflectionHelper.GetPropertyOrFieldName<T>(definitionProperty))
    {
      Source = (object) pd,
      Mode = bindingMode
    };
    propertyItem.SetBinding(itemProperty, (BindingBase) binding);
  }

  internal FrameworkElement GenerateChildrenEditorElement(PropertyItem propertyItem)
  {
    FrameworkElement childrenEditorElement = (FrameworkElement) null;
    DescriptorPropertyDefinitionBase descriptorDefinition = propertyItem.DescriptorDefinition;
    object obj = (object) null;
    Type propertyType = obj as Type;
    ITypeEditor typeEditor = (ITypeEditor) null ?? descriptorDefinition.CreateAttributeEditor();
    if (typeEditor != null)
      childrenEditorElement = typeEditor.ResolveEditor(propertyItem);
    if (childrenEditorElement == null && obj == null && propertyItem.PropertyDescriptor != null)
      childrenEditorElement = this.GenerateCustomEditingElement((object) propertyItem.PropertyDescriptor.Name, (PropertyItemBase) propertyItem);
    if (childrenEditorElement == null && propertyType == (Type) null)
      childrenEditorElement = this.GenerateCustomEditingElement(propertyItem.PropertyType, (PropertyItemBase) propertyItem);
    if (childrenEditorElement == null)
    {
      if (descriptorDefinition.IsReadOnly)
        typeEditor = (ITypeEditor) new TextBlockEditor();
      if (typeEditor == null)
        typeEditor = propertyType != (Type) null ? PropertyGridUtilities.CreateDefaultEditor(propertyType, (TypeConverter) null, propertyItem) : descriptorDefinition.CreateDefaultEditor(propertyItem);
      childrenEditorElement = typeEditor.ResolveEditor(propertyItem);
    }
    return childrenEditorElement;
  }

  internal PropertyDefinition GetPropertyDefinition(PropertyDescriptor descriptor)
  {
    PropertyDefinition propertyDefinition = (PropertyDefinition) null;
    PropertyDefinitionCollection propertyDefinitions = this.PropertyContainer.PropertyDefinitions;
    if (propertyDefinitions != null)
      propertyDefinition = propertyDefinitions[(object) descriptor.Name] ?? propertyDefinitions.GetRecursiveBaseTypes(descriptor.PropertyType);
    return propertyDefinition;
  }

  public override void PrepareChildrenPropertyItem(PropertyItemBase propertyItem, object item)
  {
    this._isPreparingItemFlag = true;
    base.PrepareChildrenPropertyItem(propertyItem, item);
    if (propertyItem.Editor == null)
    {
      FrameworkElement childrenEditorElement = this.GenerateChildrenEditorElement((PropertyItem) propertyItem);
      if (childrenEditorElement != null)
      {
        ContainerHelperBase.SetIsGenerated((DependencyObject) childrenEditorElement, true);
        propertyItem.Editor = childrenEditorElement;
      }
    }
    this._isPreparingItemFlag = false;
  }

  public override void ClearChildrenPropertyItem(PropertyItemBase propertyItem, object item)
  {
    if (propertyItem.Editor != null && ContainerHelperBase.GetIsGenerated((DependencyObject) propertyItem.Editor))
      propertyItem.Editor = (FrameworkElement) null;
    base.ClearChildrenPropertyItem(propertyItem, item);
  }

  public override Binding CreateChildrenDefaultBinding(PropertyItemBase propertyItem)
  {
    return new Binding("Value")
    {
      Mode = ((PropertyItem) propertyItem).IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
    };
  }

  protected static string GetDefaultPropertyName(object instance)
  {
    return ((DefaultPropertyAttribute) TypeDescriptor.GetAttributes(instance)[typeof (DefaultPropertyAttribute)])?.Name;
  }

  private static bool IsItemOrderingProperty(string propertyName)
  {
    return string.Equals(propertyName, PropertyItemCollection.DisplayNamePropertyName) || string.Equals(propertyName, PropertyItemCollection.CategoryOrderPropertyName) || string.Equals(propertyName, PropertyItemCollection.PropertyOrderPropertyName);
  }
}
