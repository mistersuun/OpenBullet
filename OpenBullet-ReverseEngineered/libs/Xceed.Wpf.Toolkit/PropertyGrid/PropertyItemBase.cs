// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyItemBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

[TemplatePart(Name = "PART_PropertyItemsControl", Type = typeof (PropertyItemsControl))]
[TemplatePart(Name = "PART_ValueContainer", Type = typeof (ContentControl))]
public abstract class PropertyItemBase : Control, IPropertyContainer, INotifyPropertyChanged
{
  internal const string PART_ValueContainer = "PART_ValueContainer";
  private ContentControl _valueContainer;
  private ContainerHelperBase _containerHelper;
  private IPropertyContainer _parentNode;
  internal bool _isPropertyGridCategorized;
  internal bool _isSortedAlphabetically = true;
  public static readonly DependencyProperty AdvancedOptionsIconProperty = DependencyProperty.Register(nameof (AdvancedOptionsIcon), typeof (ImageSource), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty AdvancedOptionsTooltipProperty = DependencyProperty.Register(nameof (AdvancedOptionsTooltip), typeof (object), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof (Description), typeof (string), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(nameof (DisplayName), typeof (string), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty EditorProperty = DependencyProperty.Register(nameof (Editor), typeof (FrameworkElement), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(PropertyItemBase.OnEditorChanged)));
  public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.Register(nameof (HighlightedText), typeof (string), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof (IsExpanded), typeof (bool), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(PropertyItemBase.OnIsExpandedChanged)));
  public static readonly DependencyProperty IsExpandableProperty = DependencyProperty.Register(nameof (IsExpandable), typeof (bool), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(PropertyItemBase.OnIsSelectedChanged)));
  public static readonly DependencyProperty WillRefreshPropertyGridProperty = DependencyProperty.Register(nameof (WillRefreshPropertyGrid), typeof (bool), typeof (PropertyItemBase), (PropertyMetadata) new UIPropertyMetadata((object) false));
  internal static readonly RoutedEvent ItemSelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof (ItemSelectionChangedEvent), RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (PropertyItemBase));

  public ImageSource AdvancedOptionsIcon
  {
    get => (ImageSource) this.GetValue(PropertyItemBase.AdvancedOptionsIconProperty);
    set => this.SetValue(PropertyItemBase.AdvancedOptionsIconProperty, (object) value);
  }

  public object AdvancedOptionsTooltip
  {
    get => this.GetValue(PropertyItemBase.AdvancedOptionsTooltipProperty);
    set => this.SetValue(PropertyItemBase.AdvancedOptionsTooltipProperty, value);
  }

  public string Description
  {
    get => (string) this.GetValue(PropertyItemBase.DescriptionProperty);
    set => this.SetValue(PropertyItemBase.DescriptionProperty, (object) value);
  }

  public string DisplayName
  {
    get => (string) this.GetValue(PropertyItemBase.DisplayNameProperty);
    set => this.SetValue(PropertyItemBase.DisplayNameProperty, (object) value);
  }

  public FrameworkElement Editor
  {
    get => (FrameworkElement) this.GetValue(PropertyItemBase.EditorProperty);
    set => this.SetValue(PropertyItemBase.EditorProperty, (object) value);
  }

  private static void OnEditorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is PropertyItemBase propertyItemBase))
      return;
    propertyItemBase.OnEditorChanged((FrameworkElement) e.OldValue, (FrameworkElement) e.NewValue);
  }

  protected virtual void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
  {
  }

  public string HighlightedText
  {
    get => (string) this.GetValue(PropertyItemBase.HighlightedTextProperty);
    set => this.SetValue(PropertyItemBase.HighlightedTextProperty, (object) value);
  }

  public bool IsExpanded
  {
    get => (bool) this.GetValue(PropertyItemBase.IsExpandedProperty);
    set => this.SetValue(PropertyItemBase.IsExpandedProperty, (object) value);
  }

  private static void OnIsExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is PropertyItemBase propertyItemBase))
      return;
    propertyItemBase.OnIsExpandedChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsExpandedChanged(bool oldValue, bool newValue)
  {
  }

  public bool IsExpandable
  {
    get => (bool) this.GetValue(PropertyItemBase.IsExpandableProperty);
    set => this.SetValue(PropertyItemBase.IsExpandableProperty, (object) value);
  }

  public bool IsSelected
  {
    get => (bool) this.GetValue(PropertyItemBase.IsSelectedProperty);
    set => this.SetValue(PropertyItemBase.IsSelectedProperty, (object) value);
  }

  private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is PropertyItemBase propertyItemBase))
      return;
    propertyItemBase.OnIsSelectedChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
  {
    this.RaiseItemSelectionChangedEvent();
  }

  public FrameworkElement ParentElement => this.ParentNode as FrameworkElement;

  internal IPropertyContainer ParentNode
  {
    get => this._parentNode;
    set => this._parentNode = value;
  }

  internal ContentControl ValueContainer => this._valueContainer;

  public int Level { get; internal set; }

  public IList Properties
  {
    get
    {
      if (this._containerHelper == null)
        this._containerHelper = (ContainerHelperBase) new ObjectContainerHelper((IPropertyContainer) this, (object) null);
      return this._containerHelper.Properties;
    }
  }

  public Style PropertyContainerStyle
  {
    get => this.ParentNode == null ? (Style) null : this.ParentNode.PropertyContainerStyle;
  }

  internal ContainerHelperBase ContainerHelper
  {
    get => this._containerHelper;
    set
    {
      this._containerHelper = value != null ? value : throw new ArgumentNullException(nameof (value));
      this.RaisePropertyChanged<IList>((Expression<Func<IList>>) (() => this.Properties));
    }
  }

  public bool WillRefreshPropertyGrid
  {
    get => (bool) this.GetValue(PropertyItemBase.WillRefreshPropertyGridProperty);
    set => this.SetValue(PropertyItemBase.WillRefreshPropertyGridProperty, (object) value);
  }

  private void RaiseItemSelectionChangedEvent()
  {
    this.RaiseEvent(new RoutedEventArgs(PropertyItemBase.ItemSelectionChangedEvent));
  }

  public event PropertyChangedEventHandler PropertyChanged;

  internal void RaisePropertyChanged<TMember>(Expression<Func<TMember>> propertyExpression)
  {
    this.Notify<TMember>(this.PropertyChanged, propertyExpression);
  }

  internal void RaisePropertyChanged(string name) => this.Notify(this.PropertyChanged, name);

  static PropertyItemBase()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (PropertyItemBase), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (PropertyItemBase)));
  }

  internal PropertyItemBase()
  {
    this.GotFocus += new RoutedEventHandler(this.PropertyItemBase_GotFocus);
    this.RequestBringIntoView += new RequestBringIntoViewEventHandler(this.PropertyItemBase_RequestBringIntoView);
    this.AddHandler(PropertyItemsControl.PreparePropertyItemEvent, (Delegate) new PropertyItemEventHandler(this.OnPreparePropertyItemInternal));
    this.AddHandler(PropertyItemsControl.ClearPropertyItemEvent, (Delegate) new PropertyItemEventHandler(this.OnClearPropertyItemInternal));
  }

  private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
  {
    args.PropertyItem.Level = this.Level + 1;
    this._containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);
    args.Handled = true;
  }

  private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
  {
    this._containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
    args.PropertyItem.Level = 0;
    args.Handled = true;
  }

  private void PropertyItemBase_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
  {
    e.Handled = true;
  }

  protected virtual Type GetPropertyItemType() => (Type) null;

  protected virtual string GetPropertyItemName() => this.DisplayName;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._containerHelper.ChildrenItemsControl = (ItemsControl) (this.GetTemplateChild("PART_PropertyItemsControl") as PropertyItemsControl);
    this._valueContainer = this.GetTemplateChild("PART_ValueContainer") as ContentControl;
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    this.IsSelected = true;
    if (!this.IsKeyboardFocusWithin)
      this.Focus();
    e.Handled = true;
  }

  private void PropertyItemBase_GotFocus(object sender, RoutedEventArgs e)
  {
    this.IsSelected = true;
    e.Handled = true;
  }

  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);
    if (!ReflectionHelper.IsPublicInstanceProperty(this.GetType(), e.Property.Name) || !this.IsLoaded || this._parentNode == null || this._parentNode.ContainerHelper.IsCleaning)
      return;
    this.RaisePropertyChanged(e.Property.Name);
  }

  private PropertyDefinitionCollection GetPropertItemPropertyDefinitions()
  {
    if (this.ParentNode != null && this.ParentNode.PropertyDefinitions != null)
    {
      string propertyItemName = this.GetPropertyItemName();
      foreach (PropertyDefinition propertyDefinition in (Collection<PropertyDefinition>) this.ParentNode.PropertyDefinitions)
      {
        if (propertyDefinition.TargetProperties.Contains((object) propertyItemName))
          return propertyDefinition.PropertyDefinitions;
        Type propertyItemType = this.GetPropertyItemType();
        if (propertyItemType != (Type) null)
        {
          foreach (object targetProperty in (IEnumerable) propertyDefinition.TargetProperties)
          {
            Type type = targetProperty as Type;
            if (type != (Type) null && type.IsAssignableFrom(propertyItemType))
              return propertyDefinition.PropertyDefinitions;
          }
        }
      }
    }
    return (PropertyDefinitionCollection) null;
  }

  Style IPropertyContainer.PropertyContainerStyle => this.PropertyContainerStyle;

  EditorDefinitionCollection IPropertyContainer.EditorDefinitions
  {
    get
    {
      return this.ParentNode == null ? (EditorDefinitionCollection) null : this.ParentNode.EditorDefinitions;
    }
  }

  PropertyDefinitionCollection IPropertyContainer.PropertyDefinitions
  {
    get => this.GetPropertItemPropertyDefinitions();
  }

  ContainerHelperBase IPropertyContainer.ContainerHelper => this.ContainerHelper;

  bool IPropertyContainer.IsCategorized => this._isPropertyGridCategorized;

  bool IPropertyContainer.IsSortedAlphabetically => this._isSortedAlphabetically;

  bool IPropertyContainer.AutoGenerateProperties
  {
    get
    {
      if (this.ParentNode == null)
        return true;
      PropertyDefinitionCollection propertyDefinitions = this.GetPropertItemPropertyDefinitions();
      return propertyDefinitions == null || propertyDefinitions.Count == 0 || this.ParentNode.AutoGenerateProperties;
    }
  }

  bool IPropertyContainer.HideInheritedProperties => false;

  FilterInfo IPropertyContainer.FilterInfo => new FilterInfo();

  bool? IPropertyContainer.IsPropertyVisible(PropertyDescriptor pd)
  {
    return this._parentNode != null ? this._parentNode.IsPropertyVisible(pd) : new bool?();
  }
}
