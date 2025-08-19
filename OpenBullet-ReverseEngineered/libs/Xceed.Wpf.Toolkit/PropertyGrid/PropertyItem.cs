// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

[TemplatePart(Name = "content", Type = typeof (ContentControl))]
public class PropertyItem : CustomPropertyItem
{
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (PropertyItem), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IsInvalidProperty = DependencyProperty.Register(nameof (IsInvalid), typeof (bool), typeof (PropertyItem), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(PropertyItem.OnIsInvalidChanged)));

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(PropertyItem.IsReadOnlyProperty);
    set => this.SetValue(PropertyItem.IsReadOnlyProperty, (object) value);
  }

  public bool IsInvalid
  {
    get => (bool) this.GetValue(PropertyItem.IsInvalidProperty);
    internal set => this.SetValue(PropertyItem.IsInvalidProperty, (object) value);
  }

  private static void OnIsInvalidChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is PropertyItem propertyItem))
      return;
    propertyItem.OnIsInvalidChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsInvalidChanged(bool oldValue, bool newValue)
  {
    BindingExpression bindingExpression = this.GetBindingExpression(CustomPropertyItem.ValueProperty);
    if (newValue)
      Validation.MarkInvalid((BindingExpressionBase) bindingExpression, new ValidationError((ValidationRule) new PropertyItem.InvalidValueValidationRule(), (object) bindingExpression)
      {
        ErrorContent = (object) "Value could not be converted."
      });
    else
      Validation.ClearInvalid((BindingExpressionBase) bindingExpression);
  }

  public PropertyDescriptor PropertyDescriptor { get; internal set; }

  public string PropertyName
  {
    get
    {
      return this.DescriptorDefinition == null ? (string) null : this.DescriptorDefinition.PropertyName;
    }
  }

  public Type PropertyType
  {
    get => this.PropertyDescriptor == null ? (Type) null : this.PropertyDescriptor.PropertyType;
  }

  internal DescriptorPropertyDefinitionBase DescriptorDefinition { get; private set; }

  public object Instance { get; internal set; }

  protected override string GetPropertyItemName() => this.PropertyName;

  protected override Type GetPropertyItemType() => this.PropertyType;

  protected override void OnIsExpandedChanged(bool oldValue, bool newValue)
  {
    if (!newValue || !this.IsLoaded)
      return;
    this.GenerateExpandedPropertyItems();
  }

  protected override object OnCoerceValueChanged(object baseValue)
  {
    this.SetRedInvalidBorder(this.GetBindingExpression(CustomPropertyItem.ValueProperty));
    return baseValue;
  }

  protected override void OnValueChanged(object oldValue, object newValue)
  {
    base.OnValueChanged(oldValue, newValue);
    if (newValue != null || this.DescriptorDefinition == null || this.DescriptorDefinition.DefaultValue == null)
      return;
    this.SetCurrentValue(CustomPropertyItem.ValueProperty, this.DescriptorDefinition.DefaultValue);
  }

  internal void SetRedInvalidBorder(BindingExpression be)
  {
    if (be == null || !(be.DataItem is DescriptorPropertyDefinitionBase))
      return;
    DescriptorPropertyDefinitionBase dataItem = be.DataItem as DescriptorPropertyDefinitionBase;
    if (!Validation.GetHasError((DependencyObject) dataItem))
      return;
    ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors((DependencyObject) dataItem);
    Validation.MarkInvalid((BindingExpressionBase) be, errors[0]);
  }

  private void OnDefinitionContainerHelperInvalidated(object sender, EventArgs e)
  {
    if (this.ContainerHelper != null)
      this.ContainerHelper.ClearHelper();
    ObjectContainerHelperBase containerHelper = this.DescriptorDefinition.CreateContainerHelper((IPropertyContainer) this);
    this.ContainerHelper = (ContainerHelperBase) containerHelper;
    if (!this.IsExpanded)
      return;
    containerHelper.GenerateProperties();
  }

  private void Init(DescriptorPropertyDefinitionBase definition)
  {
    if (definition == null)
      throw new ArgumentNullException(nameof (definition));
    if (this.ContainerHelper != null)
      this.ContainerHelper.ClearHelper();
    this.DescriptorDefinition = definition;
    this.ContainerHelper = (ContainerHelperBase) definition.CreateContainerHelper((IPropertyContainer) this);
    definition.ContainerHelperInvalidated += new EventHandler(this.OnDefinitionContainerHelperInvalidated);
    this.Loaded += new RoutedEventHandler(this.PropertyItem_Loaded);
  }

  private void GenerateExpandedPropertyItems()
  {
    if (!this.IsExpanded || !(this.ContainerHelper is ObjectContainerHelperBase containerHelper))
      return;
    containerHelper.GenerateProperties();
  }

  private void PropertyItem_Loaded(object sender, RoutedEventArgs e)
  {
    this.GenerateExpandedPropertyItems();
  }

  internal PropertyItem(DescriptorPropertyDefinitionBase definition)
    : base(definition.IsPropertyGridCategorized, !definition.PropertyType.IsArray)
  {
    this.Init(definition);
  }

  private class InvalidValueValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      return new ValidationResult(false, (object) null);
    }
  }
}
