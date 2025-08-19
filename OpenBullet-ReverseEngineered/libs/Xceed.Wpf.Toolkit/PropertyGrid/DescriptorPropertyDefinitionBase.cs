// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.DescriptorPropertyDefinitionBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

internal abstract class DescriptorPropertyDefinitionBase : DependencyObject
{
  private string _category;
  private string _categoryValue;
  private string _description;
  private string _displayName;
  private object _defaultValue;
  private int _displayOrder;
  private bool _expandableAttribute;
  private bool _isReadOnly;
  private IList<Type> _newItemTypes;
  private IEnumerable<CommandBinding> _commandBindings;
  public static readonly DependencyProperty AdvancedOptionsIconProperty = DependencyProperty.Register(nameof (AdvancedOptionsIcon), typeof (ImageSource), typeof (DescriptorPropertyDefinitionBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty AdvancedOptionsTooltipProperty = DependencyProperty.Register(nameof (AdvancedOptionsTooltip), typeof (object), typeof (DescriptorPropertyDefinitionBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty IsExpandableProperty = DependencyProperty.Register(nameof (IsExpandable), typeof (bool), typeof (DescriptorPropertyDefinitionBase), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (object), typeof (DescriptorPropertyDefinitionBase), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(DescriptorPropertyDefinitionBase.OnValueChanged)));

  internal abstract PropertyDescriptor PropertyDescriptor { get; }

  internal DescriptorPropertyDefinitionBase(bool isPropertyGridCategorized)
  {
    this.IsPropertyGridCategorized = isPropertyGridCategorized;
  }

  protected virtual string ComputeCategory() => (string) null;

  protected virtual string ComputeCategoryValue() => (string) null;

  protected virtual string ComputeDescription() => (string) null;

  protected virtual int ComputeDisplayOrder(bool isPropertyGridCategorized) => int.MaxValue;

  protected virtual bool ComputeExpandableAttribute() => false;

  protected virtual object ComputeDefaultValueAttribute() => (object) null;

  protected abstract bool ComputeIsExpandable();

  protected virtual IList<Type> ComputeNewItemTypes() => (IList<Type>) null;

  protected virtual bool ComputeIsReadOnly() => false;

  protected virtual bool ComputeCanResetValue() => false;

  protected virtual object ComputeAdvancedOptionsTooltip() => (object) null;

  protected virtual void ResetValue()
  {
    BindingOperations.GetBindingExpressionBase((DependencyObject) this, DescriptorPropertyDefinitionBase.ValueProperty)?.UpdateTarget();
  }

  protected abstract BindingBase CreateValueBinding();

  internal abstract ObjectContainerHelperBase CreateContainerHelper(IPropertyContainer parent);

  internal void RaiseContainerHelperInvalidated()
  {
    if (this.ContainerHelperInvalidated == null)
      return;
    this.ContainerHelperInvalidated((object) this, EventArgs.Empty);
  }

  internal virtual ITypeEditor CreateDefaultEditor(PropertyItem propertyItem) => (ITypeEditor) null;

  internal virtual ITypeEditor CreateAttributeEditor() => (ITypeEditor) null;

  internal void UpdateAdvanceOptionsForItem(
    DependencyObject dependencyObject,
    DependencyPropertyDescriptor dpDescriptor,
    out object tooltip)
  {
    tooltip = (object) StringConstants.Default;
    if (typeof (Style).IsAssignableFrom(this.PropertyType) | typeof (DynamicResourceExtension).IsAssignableFrom(this.PropertyType))
      tooltip = (object) StringConstants.Resource;
    else if (dependencyObject != null && dpDescriptor != null)
    {
      if (BindingOperations.GetBindingExpressionBase(dependencyObject, dpDescriptor.DependencyProperty) != null)
      {
        tooltip = (object) StringConstants.Databinding;
      }
      else
      {
        switch (DependencyPropertyHelper.GetValueSource(dependencyObject, dpDescriptor.DependencyProperty).BaseValueSource)
        {
          case BaseValueSource.Inherited:
          case BaseValueSource.DefaultStyle:
          case BaseValueSource.ImplicitStyleReference:
            tooltip = (object) StringConstants.Inheritance;
            break;
          case BaseValueSource.Style:
            tooltip = (object) StringConstants.StyleSetter;
            break;
          case BaseValueSource.Local:
            tooltip = (object) StringConstants.Local;
            break;
        }
      }
    }
    else
    {
      if (this.Value.Equals(this.DefaultValue))
        return;
      if (this.DefaultValue != null)
        tooltip = (object) StringConstants.Local;
      else if (this.PropertyType.IsValueType)
      {
        if (this.Value.Equals(Activator.CreateInstance(this.PropertyType)))
          return;
        tooltip = (object) StringConstants.Local;
      }
      else
      {
        if (this.Value == null)
          return;
        tooltip = (object) StringConstants.Local;
      }
    }
  }

  internal void UpdateAdvanceOptions()
  {
    this.AdvancedOptionsTooltip = this.ComputeAdvancedOptionsTooltip();
  }

  internal void UpdateIsExpandable()
  {
    this.IsExpandable = this.ComputeIsExpandable() && this.ExpandableAttribute;
  }

  internal void UpdateValueFromSource()
  {
    BindingOperations.GetBindingExpressionBase((DependencyObject) this, DescriptorPropertyDefinitionBase.ValueProperty)?.UpdateTarget();
  }

  internal object ComputeDescriptionForItem(object item)
  {
    PropertyDescriptor property = item as PropertyDescriptor;
    DisplayAttribute attribute1 = PropertyGridUtilities.GetAttribute<DisplayAttribute>(property);
    if (attribute1 != null)
      return (object) attribute1.GetDescription();
    DescriptionAttribute attribute2 = PropertyGridUtilities.GetAttribute<DescriptionAttribute>(property);
    return attribute2 == null ? (object) property.Description : (object) attribute2.Description;
  }

  internal object ComputeNewItemTypesForItem(object item)
  {
    NewItemTypesAttribute attribute = PropertyGridUtilities.GetAttribute<NewItemTypesAttribute>(item as PropertyDescriptor);
    return attribute == null ? (object) null : (object) attribute.Types;
  }

  internal object ComputeDisplayOrderForItem(object item)
  {
    PropertyDescriptor propertyDescriptor = item as PropertyDescriptor;
    DisplayAttribute attribute = PropertyGridUtilities.GetAttribute<DisplayAttribute>(this.PropertyDescriptor);
    if (attribute != null && attribute.GetOrder().HasValue)
      return (object) attribute.GetOrder();
    List<PropertyOrderAttribute> list = propertyDescriptor.Attributes.OfType<PropertyOrderAttribute>().ToList<PropertyOrderAttribute>();
    if (list.Count > 0)
    {
      this.ValidatePropertyOrderAttributes(list);
      if (this.IsPropertyGridCategorized)
      {
        PropertyOrderAttribute propertyOrderAttribute = list.FirstOrDefault<PropertyOrderAttribute>((Func<PropertyOrderAttribute, bool>) (x => x.UsageContext == UsageContextEnum.Categorized || x.UsageContext == UsageContextEnum.Both));
        if (propertyOrderAttribute != null)
          return (object) propertyOrderAttribute.Order;
      }
      else
      {
        PropertyOrderAttribute propertyOrderAttribute = list.FirstOrDefault<PropertyOrderAttribute>((Func<PropertyOrderAttribute, bool>) (x => x.UsageContext == UsageContextEnum.Alphabetical || x.UsageContext == UsageContextEnum.Both));
        if (propertyOrderAttribute != null)
          return (object) propertyOrderAttribute.Order;
      }
    }
    return (object) int.MaxValue;
  }

  internal object ComputeExpandableAttributeForItem(object item)
  {
    return (object) (PropertyGridUtilities.GetAttribute<ExpandableObjectAttribute>((PropertyDescriptor) item) != null);
  }

  internal int ComputeDisplayOrderInternal(bool isPropertyGridCategorized)
  {
    return this.ComputeDisplayOrder(isPropertyGridCategorized);
  }

  internal object GetValueInstance(object sourceObject)
  {
    if (sourceObject is ICustomTypeDescriptor customTypeDescriptor)
      sourceObject = customTypeDescriptor.GetPropertyOwner(this.PropertyDescriptor);
    return sourceObject;
  }

  internal object ComputeDefaultValueAttributeForItem(object item)
  {
    return PropertyGridUtilities.GetAttribute<DefaultValueAttribute>((PropertyDescriptor) item)?.Value;
  }

  private static void ExecuteResetValueCommand(object sender, ExecutedRoutedEventArgs e)
  {
    if (!(e.Parameter is PropertyItem propertyItem))
      propertyItem = sender as PropertyItem;
    if (propertyItem == null || propertyItem.DescriptorDefinition == null || !propertyItem.DescriptorDefinition.ComputeCanResetValue())
      return;
    propertyItem.DescriptorDefinition.ResetValue();
  }

  private static void CanExecuteResetValueCommand(object sender, CanExecuteRoutedEventArgs e)
  {
    if (!(e.Parameter is PropertyItem propertyItem))
      propertyItem = sender as PropertyItem;
    e.CanExecute = propertyItem != null && propertyItem.DescriptorDefinition != null && propertyItem.DescriptorDefinition.ComputeCanResetValue();
  }

  private string ComputeDisplayName()
  {
    DisplayAttribute attribute1 = PropertyGridUtilities.GetAttribute<DisplayAttribute>(this.PropertyDescriptor);
    string displayName = attribute1 != null ? attribute1.GetName() : this.PropertyDescriptor.DisplayName;
    ParenthesizePropertyNameAttribute attribute2 = PropertyGridUtilities.GetAttribute<ParenthesizePropertyNameAttribute>(this.PropertyDescriptor);
    if (attribute2 != null && attribute2.NeedParenthesis)
      displayName = $"({displayName})";
    return displayName;
  }

  private void ValidatePropertyOrderAttributes(List<PropertyOrderAttribute> list)
  {
    if (list.Count <= 0 || list.FirstOrDefault<PropertyOrderAttribute>((Func<PropertyOrderAttribute, bool>) (x => x.UsageContext == UsageContextEnum.Both)) == null)
      return;
    int count = list.Count;
  }

  public event EventHandler ContainerHelperInvalidated;

  public ImageSource AdvancedOptionsIcon
  {
    get
    {
      return (ImageSource) this.GetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsIconProperty);
    }
    set
    {
      this.SetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsIconProperty, (object) value);
    }
  }

  public object AdvancedOptionsTooltip
  {
    get => this.GetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsTooltipProperty);
    set => this.SetValue(DescriptorPropertyDefinitionBase.AdvancedOptionsTooltipProperty, value);
  }

  public bool IsExpandable
  {
    get => (bool) this.GetValue(DescriptorPropertyDefinitionBase.IsExpandableProperty);
    set => this.SetValue(DescriptorPropertyDefinitionBase.IsExpandableProperty, (object) value);
  }

  public string Category
  {
    get => this._category;
    internal set => this._category = value;
  }

  public string CategoryValue
  {
    get => this._categoryValue;
    internal set => this._categoryValue = value;
  }

  public IEnumerable<CommandBinding> CommandBindings => this._commandBindings;

  public string DisplayName
  {
    get => this._displayName;
    internal set => this._displayName = value;
  }

  public object DefaultValue
  {
    get => this._defaultValue;
    set => this._defaultValue = value;
  }

  public string Description
  {
    get => this._description;
    internal set => this._description = value;
  }

  public int DisplayOrder
  {
    get => this._displayOrder;
    internal set => this._displayOrder = value;
  }

  public bool IsReadOnly => this._isReadOnly;

  public IList<Type> NewItemTypes => this._newItemTypes;

  public string PropertyName => this.PropertyDescriptor.Name;

  public Type PropertyType => this.PropertyDescriptor.PropertyType;

  internal bool ExpandableAttribute
  {
    get => this._expandableAttribute;
    set
    {
      this._expandableAttribute = value;
      this.UpdateIsExpandable();
    }
  }

  internal bool IsPropertyGridCategorized { get; set; }

  public object Value
  {
    get => this.GetValue(DescriptorPropertyDefinitionBase.ValueProperty);
    set => this.SetValue(DescriptorPropertyDefinitionBase.ValueProperty, value);
  }

  private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    ((DescriptorPropertyDefinitionBase) o).OnValueChanged(e.OldValue, e.NewValue);
  }

  internal virtual void OnValueChanged(object oldValue, object newValue)
  {
    this.UpdateIsExpandable();
    this.UpdateAdvanceOptions();
    CommandManager.InvalidateRequerySuggested();
  }

  public virtual void InitProperties()
  {
    this._isReadOnly = this.ComputeIsReadOnly();
    this._category = this.ComputeCategory();
    this._categoryValue = this.ComputeCategoryValue();
    this._description = this.ComputeDescription();
    this._displayName = this.ComputeDisplayName();
    this._defaultValue = this.ComputeDefaultValueAttribute();
    this._displayOrder = this.ComputeDisplayOrder(this.IsPropertyGridCategorized);
    this._expandableAttribute = this.ComputeExpandableAttribute();
    this._newItemTypes = this.ComputeNewItemTypes();
    this._commandBindings = (IEnumerable<CommandBinding>) new CommandBinding[1]
    {
      new CommandBinding((ICommand) PropertyItemCommands.ResetValue, new ExecutedRoutedEventHandler(DescriptorPropertyDefinitionBase.ExecuteResetValueCommand), new CanExecuteRoutedEventHandler(DescriptorPropertyDefinitionBase.CanExecuteResetValueCommand))
    };
    BindingBase valueBinding = this.CreateValueBinding();
    BindingOperations.SetBinding((DependencyObject) this, DescriptorPropertyDefinitionBase.ValueProperty, valueBinding);
  }
}
