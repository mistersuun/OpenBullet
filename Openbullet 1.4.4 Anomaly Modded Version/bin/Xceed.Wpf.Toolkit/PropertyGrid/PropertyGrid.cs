// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

[TemplatePart(Name = "PART_DragThumb", Type = typeof (Thumb))]
[TemplatePart(Name = "PART_PropertyItemsControl", Type = typeof (PropertyItemsControl))]
[StyleTypedProperty(Property = "PropertyContainerStyle", StyleTargetType = typeof (PropertyItemBase))]
public class PropertyGrid : Control, ISupportInitialize, IPropertyContainer, INotifyPropertyChanged
{
  private const string PART_DragThumb = "PART_DragThumb";
  internal const string PART_PropertyItemsControl = "PART_PropertyItemsControl";
  private static readonly ComponentResourceKey SelectedObjectAdvancedOptionsMenuKey = new ComponentResourceKey(typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (object) "SelectedObjectAdvancedOptionsMenu");
  private Thumb _dragThumb;
  private bool _hasPendingSelectedObjectChanged;
  private int _initializationCount;
  private ContainerHelperBase _containerHelper;
  private PropertyDefinitionCollection _propertyDefinitions;
  private WeakEventListener<NotifyCollectionChangedEventArgs> _propertyDefinitionsListener;
  private WeakEventListener<NotifyCollectionChangedEventArgs> _editorDefinitionsListener;
  public static readonly DependencyProperty AdvancedOptionsMenuProperty = DependencyProperty.Register(nameof (AdvancedOptionsMenu), typeof (ContextMenu), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty AutoGeneratePropertiesProperty = DependencyProperty.Register(nameof (AutoGenerateProperties), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ShowDescriptionByTooltipProperty = DependencyProperty.Register(nameof (ShowDescriptionByTooltip), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ShowSummaryProperty = DependencyProperty.Register(nameof (ShowSummary), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty EditorDefinitionsProperty = DependencyProperty.Register(nameof (EditorDefinitions), typeof (EditorDefinitionCollection), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnEditorDefinitionsChanged)));
  public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof (Filter), typeof (string), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnFilterChanged)));
  public static readonly DependencyProperty FilterWatermarkProperty = DependencyProperty.Register(nameof (FilterWatermark), typeof (string), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) "Search"));
  public static readonly DependencyProperty HideInheritedPropertiesProperty = DependencyProperty.Register(nameof (HideInheritedProperties), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IsCategorizedProperty = DependencyProperty.Register(nameof (IsCategorized), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnIsCategorizedChanged)));
  public static readonly DependencyProperty IsMiscCategoryLabelHiddenProperty = DependencyProperty.Register(nameof (IsMiscCategoryLabelHidden), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IsScrollingToTopAfterRefreshProperty = DependencyProperty.Register(nameof (IsScrollingToTopAfterRefresh), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty IsVirtualizingProperty = DependencyProperty.Register(nameof (IsVirtualizing), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnIsVirtualizingChanged)));
  public static readonly DependencyProperty NameColumnWidthProperty = DependencyProperty.Register(nameof (NameColumnWidth), typeof (double), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) 150.0, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnNameColumnWidthChanged)));
  public static readonly DependencyProperty PropertyContainerStyleProperty = DependencyProperty.Register(nameof (PropertyContainerStyle), typeof (Style), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnPropertyContainerStyleChanged)));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(nameof (SelectedObject), typeof (object), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnSelectedObjectChanged)));
  public static readonly DependencyProperty SelectedObjectTypeProperty = DependencyProperty.Register(nameof (SelectedObjectType), typeof (Type), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnSelectedObjectTypeChanged)));
  public static readonly DependencyProperty SelectedObjectTypeNameProperty = DependencyProperty.Register(nameof (SelectedObjectTypeName), typeof (string), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty));
  public static readonly DependencyProperty SelectedObjectNameProperty = DependencyProperty.Register(nameof (SelectedObjectName), typeof (string), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnSelectedObjectNameChanged), new CoerceValueCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnCoerceSelectedObjectName)));
  private static readonly DependencyPropertyKey SelectedPropertyItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof (SelectedPropertyItem), typeof (PropertyItemBase), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnSelectedPropertyItemChanged)));
  public static readonly DependencyProperty SelectedPropertyItemProperty = Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyItemPropertyKey.DependencyProperty;
  public static readonly DependencyProperty SelectedPropertyProperty = DependencyProperty.Register(nameof (SelectedProperty), typeof (object), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.OnSelectedPropertyChanged)));
  public static readonly DependencyProperty ShowAdvancedOptionsProperty = DependencyProperty.Register(nameof (ShowAdvancedOptions), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ShowHorizontalScrollBarProperty = DependencyProperty.Register(nameof (ShowHorizontalScrollBar), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ShowPreviewProperty = DependencyProperty.Register(nameof (ShowPreview), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ShowSearchBoxProperty = DependencyProperty.Register(nameof (ShowSearchBox), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ShowSortOptionsProperty = DependencyProperty.Register(nameof (ShowSortOptions), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(nameof (ShowTitle), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty UpdateTextBoxSourceOnEnterKeyProperty = DependencyProperty.Register(nameof (UpdateTextBoxSourceOnEnterKey), typeof (bool), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly RoutedEvent PropertyValueChangedEvent = EventManager.RegisterRoutedEvent("PropertyValueChanged", RoutingStrategy.Bubble, typeof (PropertyValueChangedEventHandler), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid));
  public static readonly RoutedEvent SelectedPropertyItemChangedEvent = EventManager.RegisterRoutedEvent("SelectedPropertyItemChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<PropertyItemBase>), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid));
  public static readonly RoutedEvent SelectedObjectChangedEvent = EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<object>), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid));
  public static readonly RoutedEvent PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof (PropertyItemEventHandler), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid));
  public static readonly RoutedEvent ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof (PropertyItemEventHandler), typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid));

  public ContextMenu AdvancedOptionsMenu
  {
    get => (ContextMenu) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.AdvancedOptionsMenuProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.AdvancedOptionsMenuProperty, (object) value);
  }

  public bool AutoGenerateProperties
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.AutoGeneratePropertiesProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.AutoGeneratePropertiesProperty, (object) value);
  }

  public bool ShowDescriptionByTooltip
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowDescriptionByTooltipProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowDescriptionByTooltipProperty, (object) value);
  }

  public bool ShowSummary
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowSummaryProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowSummaryProperty, (object) value);
  }

  public EditorDefinitionCollection EditorDefinitions
  {
    get => (EditorDefinitionCollection) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.EditorDefinitionsProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.EditorDefinitionsProperty, (object) value);
  }

  private static void OnEditorDefinitionsChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnEditorDefinitionsChanged((EditorDefinitionCollection) e.OldValue, (EditorDefinitionCollection) e.NewValue);
  }

  protected virtual void OnEditorDefinitionsChanged(
    EditorDefinitionCollection oldValue,
    EditorDefinitionCollection newValue)
  {
    if (oldValue != null)
      CollectionChangedEventManager.RemoveListener((INotifyCollectionChanged) oldValue, (IWeakEventListener) this._editorDefinitionsListener);
    if (newValue != null)
      CollectionChangedEventManager.AddListener((INotifyCollectionChanged) newValue, (IWeakEventListener) this._editorDefinitionsListener);
    this.Notify<EditorDefinitionCollection>(this.PropertyChanged, (Expression<Func<EditorDefinitionCollection>>) (() => this.EditorDefinitions));
  }

  private void OnEditorDefinitionsCollectionChanged(
    object sender,
    NotifyCollectionChangedEventArgs e)
  {
    if (this._containerHelper == null)
      return;
    this._containerHelper.NotifyEditorDefinitionsCollectionChanged();
  }

  public string Filter
  {
    get => (string) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.FilterProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.FilterProperty, (object) value);
  }

  private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnFilterChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnFilterChanged(string oldValue, string newValue)
  {
    this.Notify<FilterInfo>(this.PropertyChanged, (Expression<Func<FilterInfo>>) (() => ((IPropertyContainer) this).FilterInfo));
  }

  public string FilterWatermark
  {
    get => (string) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.FilterWatermarkProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.FilterWatermarkProperty, (object) value);
  }

  public bool HideInheritedProperties
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.HideInheritedPropertiesProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.HideInheritedPropertiesProperty, (object) value);
  }

  public bool IsCategorized
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsCategorizedProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsCategorizedProperty, (object) value);
  }

  private static void OnIsCategorizedChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnIsCategorizedChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsCategorizedChanged(bool oldValue, bool newValue) => this.UpdateThumb();

  public bool IsMiscCategoryLabelHidden
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsMiscCategoryLabelHiddenProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsMiscCategoryLabelHiddenProperty, (object) value);
  }

  public bool IsScrollingToTopAfterRefresh
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsScrollingToTopAfterRefreshProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsScrollingToTopAfterRefreshProperty, (object) value);
  }

  public bool IsVirtualizing
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsVirtualizingProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsVirtualizingProperty, (object) value);
  }

  private static void OnIsVirtualizingChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnIsVirtualizingChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsVirtualizingChanged(bool oldValue, bool newValue)
  {
    this.UpdateContainerHelper();
  }

  public double NameColumnWidth
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.NameColumnWidthProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.NameColumnWidthProperty, (object) value);
  }

  private static void OnNameColumnWidthChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnNameColumnWidthChanged((double) e.OldValue, (double) e.NewValue);
  }

  protected virtual void OnNameColumnWidthChanged(double oldValue, double newValue)
  {
    if (this._dragThumb == null)
      return;
    ((TranslateTransform) this._dragThumb.RenderTransform).X = newValue;
  }

  public IList Properties
  {
    get => this._containerHelper == null ? (IList) null : this._containerHelper.Properties;
  }

  public Style PropertyContainerStyle
  {
    get => (Style) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyContainerStyleProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyContainerStyleProperty, (object) value);
  }

  private static void OnPropertyContainerStyleChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnPropertyContainerStyleChanged((Style) e.OldValue, (Style) e.NewValue);
  }

  protected virtual void OnPropertyContainerStyleChanged(Style oldValue, Style newValue)
  {
  }

  public PropertyDefinitionCollection PropertyDefinitions
  {
    get => this._propertyDefinitions;
    set
    {
      if (this._propertyDefinitions == value)
        return;
      PropertyDefinitionCollection propertyDefinitions = this._propertyDefinitions;
      this._propertyDefinitions = value;
      this.OnPropertyDefinitionsChanged(propertyDefinitions, value);
    }
  }

  protected virtual void OnPropertyDefinitionsChanged(
    PropertyDefinitionCollection oldValue,
    PropertyDefinitionCollection newValue)
  {
    if (oldValue != null)
      CollectionChangedEventManager.RemoveListener((INotifyCollectionChanged) oldValue, (IWeakEventListener) this._propertyDefinitionsListener);
    if (newValue != null)
      CollectionChangedEventManager.AddListener((INotifyCollectionChanged) newValue, (IWeakEventListener) this._propertyDefinitionsListener);
    this.Notify<PropertyDefinitionCollection>(this.PropertyChanged, (Expression<Func<PropertyDefinitionCollection>>) (() => this.PropertyDefinitions));
  }

  private void OnPropertyDefinitionsCollectionChanged(
    object sender,
    NotifyCollectionChangedEventArgs e)
  {
    if (this._containerHelper != null)
      this._containerHelper.NotifyPropertyDefinitionsCollectionChanged();
    if (!this.IsLoaded)
      return;
    this.UpdateContainerHelper();
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsReadOnlyProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.IsReadOnlyProperty, (object) value);
  }

  public object SelectedObject
  {
    get => this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectProperty, value);
  }

  private static void OnSelectedObjectChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnSelectedObjectChanged(e.OldValue, e.NewValue);
  }

  protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
  {
    if (this._initializationCount != 0)
    {
      this._hasPendingSelectedObjectChanged = true;
    }
    else
    {
      this.UpdateContainerHelper();
      this.RaiseEvent((RoutedEventArgs) new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectChangedEvent));
    }
  }

  public Type SelectedObjectType
  {
    get => (Type) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectTypeProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectTypeProperty, (object) value);
  }

  private static void OnSelectedObjectTypeChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnSelectedObjectTypeChanged((Type) e.OldValue, (Type) e.NewValue);
  }

  protected virtual void OnSelectedObjectTypeChanged(Type oldValue, Type newValue)
  {
  }

  public string SelectedObjectTypeName
  {
    get => (string) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectTypeNameProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectTypeNameProperty, (object) value);
  }

  public string SelectedObjectName
  {
    get => (string) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectNameProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectNameProperty, (object) value);
  }

  private static object OnCoerceSelectedObjectName(DependencyObject o, object baseValue)
  {
    return o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid && propertyGrid.SelectedObject is FrameworkElement && string.IsNullOrEmpty((string) baseValue) ? (object) "<no name>" : baseValue;
  }

  private static void OnSelectedObjectNameChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.SelectedObjectNameChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void SelectedObjectNameChanged(string oldValue, string newValue)
  {
  }

  public PropertyItemBase SelectedPropertyItem
  {
    get => (PropertyItemBase) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyItemProperty);
    internal set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyItemPropertyKey, (object) value);
  }

  private static void OnSelectedPropertyItemChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnSelectedPropertyItemChanged((PropertyItemBase) e.OldValue, (PropertyItemBase) e.NewValue);
  }

  protected virtual void OnSelectedPropertyItemChanged(
    PropertyItemBase oldValue,
    PropertyItemBase newValue)
  {
    if (oldValue != null)
      oldValue.IsSelected = false;
    if (newValue != null)
      newValue.IsSelected = true;
    this.SelectedProperty = newValue == null || this._containerHelper == null ? (object) null : this._containerHelper.ItemFromContainer(newValue);
    this.RaiseEvent((RoutedEventArgs) new RoutedPropertyChangedEventArgs<PropertyItemBase>(oldValue, newValue, Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyItemChangedEvent));
  }

  public object SelectedProperty
  {
    get => this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyProperty, value);
  }

  private static void OnSelectedPropertyChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    if (!(sender is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid propertyGrid))
      return;
    propertyGrid.OnSelectedPropertyChanged(args.OldValue, args.NewValue);
  }

  private void OnSelectedPropertyChanged(object oldValue, object newValue)
  {
    if (this._containerHelper == null || object.Equals(this._containerHelper.ItemFromContainer(this.SelectedPropertyItem), newValue))
      return;
    this.SelectedPropertyItem = this._containerHelper.ContainerFromItem(newValue);
  }

  public bool ShowAdvancedOptions
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowAdvancedOptionsProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowAdvancedOptionsProperty, (object) value);
  }

  public bool ShowHorizontalScrollBar
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowHorizontalScrollBarProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowHorizontalScrollBarProperty, (object) value);
  }

  public bool ShowPreview
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowPreviewProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowPreviewProperty, (object) value);
  }

  public bool ShowSearchBox
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowSearchBoxProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowSearchBoxProperty, (object) value);
  }

  public bool ShowSortOptions
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowSortOptionsProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowSortOptionsProperty, (object) value);
  }

  public bool ShowTitle
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowTitleProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ShowTitleProperty, (object) value);
  }

  public bool UpdateTextBoxSourceOnEnterKey
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.UpdateTextBoxSourceOnEnterKeyProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.UpdateTextBoxSourceOnEnterKeyProperty, (object) value);
  }

  static PropertyGrid()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid)));
  }

  public PropertyGrid()
  {
    this._propertyDefinitionsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(new Action<object, NotifyCollectionChangedEventArgs>(this.OnPropertyDefinitionsCollectionChanged));
    this._editorDefinitionsListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(new Action<object, NotifyCollectionChangedEventArgs>(this.OnEditorDefinitionsCollectionChanged));
    this.UpdateContainerHelper();
    this.SetCurrentValue(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.EditorDefinitionsProperty, (object) new EditorDefinitionCollection());
    this.PropertyDefinitions = new PropertyDefinitionCollection();
    this.PropertyValueChanged += new PropertyValueChangedEventHandler(this.PropertyGrid_PropertyValueChanged);
    this.AddHandler(PropertyItemBase.ItemSelectionChangedEvent, (Delegate) new RoutedEventHandler(this.OnItemSelectionChanged));
    this.AddHandler(PropertyItemsControl.PreparePropertyItemEvent, (Delegate) new PropertyItemEventHandler(this.OnPreparePropertyItemInternal));
    this.AddHandler(PropertyItemsControl.ClearPropertyItemEvent, (Delegate) new PropertyItemEventHandler(this.OnClearPropertyItemInternal));
    this.CommandBindings.Add(new CommandBinding((ICommand) PropertyGridCommands.ClearFilter, new ExecutedRoutedEventHandler(this.ClearFilter), new CanExecuteRoutedEventHandler(this.CanClearFilter)));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._dragThumb != null)
      this._dragThumb.DragDelta -= new DragDeltaEventHandler(this.DragThumb_DragDelta);
    this._dragThumb = this.GetTemplateChild("PART_DragThumb") as Thumb;
    if (this._dragThumb != null)
      this._dragThumb.DragDelta += new DragDeltaEventHandler(this.DragThumb_DragDelta);
    if (this._containerHelper != null)
      this._containerHelper.ChildrenItemsControl = (ItemsControl) (this.GetTemplateChild("PART_PropertyItemsControl") as PropertyItemsControl);
    TranslateTransform translateTransform = new TranslateTransform();
    translateTransform.X = this.NameColumnWidth;
    if (this._dragThumb != null)
      this._dragThumb.RenderTransform = (Transform) translateTransform;
    this.UpdateThumb();
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    TextBox originalSource = e.OriginalSource as TextBox;
    if (this.SelectedPropertyItem == null || e.Key != Key.Return || !this.UpdateTextBoxSourceOnEnterKey || originalSource == null || originalSource.AcceptsReturn)
      return;
    originalSource.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
  }

  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);
    if (!ReflectionHelper.IsPublicInstanceProperty(this.GetType(), e.Property.Name))
      return;
    this.Notify(this.PropertyChanged, e.Property.Name);
  }

  private void OnItemSelectionChanged(object sender, RoutedEventArgs args)
  {
    PropertyItemBase originalSource = (PropertyItemBase) args.OriginalSource;
    if (originalSource.IsSelected)
    {
      this.SelectedPropertyItem = originalSource;
    }
    else
    {
      if (originalSource != this.SelectedPropertyItem)
        return;
      this.SelectedPropertyItem = (PropertyItemBase) null;
    }
  }

  private void OnPreparePropertyItemInternal(object sender, PropertyItemEventArgs args)
  {
    if (this._containerHelper != null)
      this._containerHelper.PrepareChildrenPropertyItem(args.PropertyItem, args.Item);
    args.Handled = true;
  }

  private void OnClearPropertyItemInternal(object sender, PropertyItemEventArgs args)
  {
    if (this._containerHelper != null)
      this._containerHelper.ClearChildrenPropertyItem(args.PropertyItem, args.Item);
    args.Handled = true;
  }

  private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
  {
    this.NameColumnWidth = Math.Min(Math.Max(this.ActualWidth * 0.1, this.NameColumnWidth + e.HorizontalChange), this.ActualWidth * 0.9);
  }

  private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
  {
    if (!(e.OriginalSource is PropertyItem originalSource))
      return;
    if (originalSource.WillRefreshPropertyGrid)
      this.UpdateContainerHelper();
    if (!(originalSource.ParentNode is PropertyItem parentNode) || !parentNode.IsExpandable)
      return;
    this.RebuildEditor(parentNode);
  }

  private void ClearFilter(object sender, ExecutedRoutedEventArgs e) => this.Filter = string.Empty;

  private void CanClearFilter(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = !string.IsNullOrEmpty(this.Filter);
  }

  public double GetScrollPosition()
  {
    ScrollViewer scrollViewer = this.GetScrollViewer();
    return scrollViewer != null ? scrollViewer.VerticalOffset : 0.0;
  }

  public void ScrollToPosition(double position)
  {
    this.GetScrollViewer()?.ScrollToVerticalOffset(position);
  }

  public void ScrollToTop() => this.GetScrollViewer()?.ScrollToTop();

  public void ScrollToBottom() => this.GetScrollViewer()?.ScrollToBottom();

  public void CollapseAllProperties()
  {
    if (this._containerHelper == null)
      return;
    this._containerHelper.SetPropertiesExpansion(false);
  }

  public void ExpandAllProperties()
  {
    if (this._containerHelper == null)
      return;
    this._containerHelper.SetPropertiesExpansion(true);
  }

  public void ExpandProperty(string propertyName)
  {
    if (this._containerHelper == null)
      return;
    this._containerHelper.SetPropertiesExpansion(propertyName, true);
  }

  public void CollapseProperty(string propertyName)
  {
    if (this._containerHelper == null)
      return;
    this._containerHelper.SetPropertiesExpansion(propertyName, false);
  }

  private ScrollViewer GetScrollViewer()
  {
    return this._containerHelper != null && this._containerHelper.ChildrenItemsControl != null ? TreeHelper.FindChild<ScrollViewer>((DependencyObject) this._containerHelper.ChildrenItemsControl) : (ScrollViewer) null;
  }

  private void RebuildEditor(PropertyItem propertyItem)
  {
    FrameworkElement childrenEditorElement = (propertyItem.ContainerHelper as ObjectContainerHelperBase).GenerateChildrenEditorElement(propertyItem);
    if (childrenEditorElement == null)
      return;
    ContainerHelperBase.SetIsGenerated((DependencyObject) childrenEditorElement, true);
    propertyItem.Editor = childrenEditorElement;
    BindingExpression bindingExpression = propertyItem.GetBindingExpression(CustomPropertyItem.ValueProperty);
    if (bindingExpression == null)
      return;
    bindingExpression.UpdateSource();
    propertyItem.SetRedInvalidBorder(bindingExpression);
  }

  private void UpdateContainerHelper()
  {
    ItemsControl itemsControl = (ItemsControl) null;
    if (this._containerHelper != null)
    {
      itemsControl = this._containerHelper.ChildrenItemsControl;
      this._containerHelper.ClearHelper();
      if (this._containerHelper is ObjectContainerHelperBase)
        this.AdvancedOptionsMenu = (ContextMenu) null;
    }
    this._containerHelper = (ContainerHelperBase) new ObjectContainerHelper((IPropertyContainer) this, this.SelectedObject);
    ((ObjectContainerHelperBase) this._containerHelper).GenerateProperties();
    if (this._containerHelper != null)
      this._containerHelper.ChildrenItemsControl = itemsControl;
    if (this.IsScrollingToTopAfterRefresh)
      this.ScrollToTop();
    this.Notify<IList>(this.PropertyChanged, (Expression<Func<IList>>) (() => this.Properties));
  }

  private void UpdateThumb()
  {
    if (this._dragThumb == null)
      return;
    if (this.IsCategorized)
      this._dragThumb.Margin = new Thickness(6.0, 0.0, 0.0, 0.0);
    else
      this._dragThumb.Margin = new Thickness(-1.0, 0.0, 0.0, 0.0);
  }

  protected virtual Predicate<object> CreateFilter(string filter) => (Predicate<object>) null;

  public void Update()
  {
    if (this._containerHelper == null)
      return;
    this._containerHelper.UpdateValuesFromSource();
  }

  public event PropertyChangedEventHandler PropertyChanged;

  public event PropertyValueChangedEventHandler PropertyValueChanged
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyValueChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyValueChangedEvent, (Delegate) value);
  }

  public event RoutedPropertyChangedEventHandler<PropertyItemBase> SelectedPropertyItemChanged
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyItemChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedPropertyItemChangedEvent, (Delegate) value);
  }

  public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.SelectedObjectChangedEvent, (Delegate) value);
  }

  public event IsPropertyBrowsableHandler IsPropertyBrowsable;

  public event PropertyItemEventHandler PreparePropertyItem
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PreparePropertyItemEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PreparePropertyItemEvent, (Delegate) value);
  }

  public static void AddPreparePropertyItemHandler(
    UIElement element,
    PropertyItemEventHandler handler)
  {
    element.AddHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PreparePropertyItemEvent, (Delegate) handler);
  }

  public static void RemovePreparePropertyItemHandler(
    UIElement element,
    PropertyItemEventHandler handler)
  {
    element.RemoveHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PreparePropertyItemEvent, (Delegate) handler);
  }

  internal static void RaisePreparePropertyItemEvent(
    UIElement source,
    PropertyItemBase propertyItem,
    object item)
  {
    source.RaiseEvent((RoutedEventArgs) new PropertyItemEventArgs(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PreparePropertyItemEvent, (object) source, propertyItem, item));
  }

  public event PropertyItemEventHandler ClearPropertyItem
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ClearPropertyItemEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ClearPropertyItemEvent, (Delegate) value);
  }

  public static void AddClearPropertyItemHandler(
    UIElement element,
    PropertyItemEventHandler handler)
  {
    element.AddHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ClearPropertyItemEvent, (Delegate) handler);
  }

  public static void RemoveClearPropertyItemHandler(
    UIElement element,
    PropertyItemEventHandler handler)
  {
    element.RemoveHandler(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ClearPropertyItemEvent, (Delegate) handler);
  }

  internal static void RaiseClearPropertyItemEvent(
    UIElement source,
    PropertyItemBase propertyItem,
    object item)
  {
    source.RaiseEvent((RoutedEventArgs) new PropertyItemEventArgs(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.ClearPropertyItemEvent, (object) source, propertyItem, item));
  }

  public override void BeginInit()
  {
    base.BeginInit();
    ++this._initializationCount;
  }

  public override void EndInit()
  {
    base.EndInit();
    if (--this._initializationCount != 0)
      return;
    if (this._hasPendingSelectedObjectChanged)
    {
      this.UpdateContainerHelper();
      this._hasPendingSelectedObjectChanged = false;
    }
    if (this._containerHelper == null)
      return;
    this._containerHelper.OnEndInit();
  }

  FilterInfo IPropertyContainer.FilterInfo
  {
    get
    {
      return new FilterInfo()
      {
        Predicate = this.CreateFilter(this.Filter),
        InputString = this.Filter
      };
    }
  }

  ContainerHelperBase IPropertyContainer.ContainerHelper => this._containerHelper;

  bool IPropertyContainer.IsSortedAlphabetically => true;

  bool? IPropertyContainer.IsPropertyVisible(PropertyDescriptor pd)
  {
    IsPropertyBrowsableHandler propertyBrowsable = this.IsPropertyBrowsable;
    if (propertyBrowsable == null)
      return new bool?();
    IsPropertyBrowsableArgs e = new IsPropertyBrowsableArgs(pd);
    propertyBrowsable((object) this, e);
    return e.IsBrowsable;
  }
}
