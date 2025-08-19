// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridRow
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridRow : Control
{
  public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof (Item), typeof (object), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyRowPropertyChanged)));
  public static readonly DependencyProperty ItemsPanelProperty = ItemsControl.ItemsPanelProperty.AddOwner(typeof (DataGridRow));
  public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof (Header), typeof (object), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyRowPropertyChanged)));
  public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(nameof (HeaderStyle), typeof (Style), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyRowAndRowHeaderPropertyChanged), new CoerceValueCallback(DataGridRow.OnCoerceHeaderStyle)));
  public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof (HeaderTemplate), typeof (DataTemplate), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyRowAndRowHeaderPropertyChanged), new CoerceValueCallback(DataGridRow.OnCoerceHeaderTemplate)));
  public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register(nameof (HeaderTemplateSelector), typeof (DataTemplateSelector), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyRowAndRowHeaderPropertyChanged), new CoerceValueCallback(DataGridRow.OnCoerceHeaderTemplateSelector)));
  public static readonly DependencyProperty ValidationErrorTemplateProperty = DependencyProperty.Register(nameof (ValidationErrorTemplate), typeof (ControlTemplate), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyRowPropertyChanged), new CoerceValueCallback(DataGridRow.OnCoerceValidationErrorTemplate)));
  public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(nameof (DetailsTemplate), typeof (DataTemplate), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyDetailsTemplatePropertyChanged), new CoerceValueCallback(DataGridRow.OnCoerceDetailsTemplate)));
  public static readonly DependencyProperty DetailsTemplateSelectorProperty = DependencyProperty.Register(nameof (DetailsTemplateSelector), typeof (DataTemplateSelector), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyDetailsTemplatePropertyChanged), new CoerceValueCallback(DataGridRow.OnCoerceDetailsTemplateSelector)));
  public static readonly DependencyProperty DetailsVisibilityProperty = DependencyProperty.Register(nameof (DetailsVisibility), typeof (Visibility), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Collapsed, new PropertyChangedCallback(DataGridRow.OnNotifyDetailsVisibilityChanged), new CoerceValueCallback(DataGridRow.OnCoerceDetailsVisibility)));
  public static readonly DependencyProperty AlternationIndexProperty = ItemsControl.AlternationIndexProperty.AddOwner(typeof (DataGridRow));
  public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, new PropertyChangedCallback(DataGridRow.OnIsSelectedChanged)));
  public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof (DataGridRow));
  public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof (DataGridRow));
  private static readonly DependencyPropertyKey IsEditingPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsEditing), typeof (bool), typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsEditingProperty = DataGridRow.IsEditingPropertyKey.DependencyProperty;
  internal bool _detailsLoaded;
  private DataGrid _owner;
  private Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter _cellsPresenter;
  private Microsoft.Windows.Controls.Primitives.DataGridDetailsPresenter _detailsPresenter;
  private Microsoft.Windows.Controls.Primitives.DataGridRowHeader _rowHeader;
  private ContainerTracking<DataGridRow> _tracker;
  private double _cellsPresenterResizeHeight;

  static DataGridRow()
  {
    UIElement.VisibilityProperty.OverrideMetadata(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridRow.OnCoerceVisibility)));
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridRow)));
    DataGridRow.ItemsPanelProperty.OverrideMetadata(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) new ItemsPanelTemplate(new FrameworkElementFactory(typeof (DataGridCellsPanel)))));
    UIElement.FocusableProperty.OverrideMetadata(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    Control.BackgroundProperty.OverrideMetadata(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridRow.OnNotifyRowPropertyChanged), new CoerceValueCallback(DataGridRow.OnCoerceBackground)));
    FrameworkElement.BindingGroupProperty.OverrideMetadata(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridRow.OnNotifyRowPropertyChanged)));
    UIElement.SnapsToDevicePixelsProperty.OverrideMetadata(typeof (DataGridRow), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.AffectsArrange));
  }

  public DataGridRow() => this._tracker = new ContainerTracking<DataGridRow>(this);

  public object Item
  {
    get => this.GetValue(DataGridRow.ItemProperty);
    set => this.SetValue(DataGridRow.ItemProperty, value);
  }

  protected virtual void OnItemChanged(object oldItem, object newItem)
  {
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    if (cellsPresenter != null)
      cellsPresenter.Item = newItem;
    if (!(UIElementAutomationPeer.FromElement((UIElement) this) is Microsoft.Windows.Automation.Peers.DataGridRowAutomationPeer rowAutomationPeer))
      return;
    rowAutomationPeer.UpdateEventSource();
  }

  public ItemsPanelTemplate ItemsPanel
  {
    get => (ItemsPanelTemplate) this.GetValue(DataGridRow.ItemsPanelProperty);
    set => this.SetValue(DataGridRow.ItemsPanelProperty, (object) value);
  }

  protected override void OnTemplateChanged(
    ControlTemplate oldTemplate,
    ControlTemplate newTemplate)
  {
    base.OnTemplateChanged(oldTemplate, newTemplate);
    this.CellsPresenter = (Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter) null;
    this.DetailsPresenter = (Microsoft.Windows.Controls.Primitives.DataGridDetailsPresenter) null;
  }

  public object Header
  {
    get => this.GetValue(DataGridRow.HeaderProperty);
    set => this.SetValue(DataGridRow.HeaderProperty, value);
  }

  protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
  {
  }

  public Style HeaderStyle
  {
    get => (Style) this.GetValue(DataGridRow.HeaderStyleProperty);
    set => this.SetValue(DataGridRow.HeaderStyleProperty, (object) value);
  }

  public DataTemplate HeaderTemplate
  {
    get => (DataTemplate) this.GetValue(DataGridRow.HeaderTemplateProperty);
    set => this.SetValue(DataGridRow.HeaderTemplateProperty, (object) value);
  }

  public DataTemplateSelector HeaderTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(DataGridRow.HeaderTemplateSelectorProperty);
    set => this.SetValue(DataGridRow.HeaderTemplateSelectorProperty, (object) value);
  }

  public ControlTemplate ValidationErrorTemplate
  {
    get => (ControlTemplate) this.GetValue(DataGridRow.ValidationErrorTemplateProperty);
    set => this.SetValue(DataGridRow.ValidationErrorTemplateProperty, (object) value);
  }

  public DataTemplate DetailsTemplate
  {
    get => (DataTemplate) this.GetValue(DataGridRow.DetailsTemplateProperty);
    set => this.SetValue(DataGridRow.DetailsTemplateProperty, (object) value);
  }

  public DataTemplateSelector DetailsTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(DataGridRow.DetailsTemplateSelectorProperty);
    set => this.SetValue(DataGridRow.DetailsTemplateSelectorProperty, (object) value);
  }

  public Visibility DetailsVisibility
  {
    get => (Visibility) this.GetValue(DataGridRow.DetailsVisibilityProperty);
    set => this.SetValue(DataGridRow.DetailsVisibilityProperty, (object) value);
  }

  internal bool DetailsLoaded
  {
    get => this._detailsLoaded;
    set => this._detailsLoaded = value;
  }

  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);
    if (e.Property != DataGridRow.AlternationIndexProperty)
      return;
    this.NotifyPropertyChanged((DependencyObject) this, e, NotificationTarget.Rows);
  }

  internal void PrepareRow(object item, DataGrid owningDataGrid)
  {
    bool flag = this._owner != owningDataGrid;
    bool forcePrepareCells = false;
    this._owner = owningDataGrid;
    if (this != item)
    {
      if (this.Item != item)
        this.Item = item;
      else
        forcePrepareCells = true;
    }
    if (this.IsEditing)
      this.IsEditing = false;
    if (flag)
      this.SyncProperties(forcePrepareCells);
    this.Dispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(this.DelayedValidateWithoutUpdate), DispatcherPriority.DataBind, (object) this.BindingGroup);
  }

  internal void ClearRow(DataGrid owningDataGrid)
  {
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    if (cellsPresenter != null)
      this.PersistAttachedItemValue((DependencyObject) cellsPresenter, FrameworkElement.HeightProperty);
    this.PersistAttachedItemValue((DependencyObject) this, DataGridRow.DetailsVisibilityProperty);
    this._owner = (DataGrid) null;
  }

  private void PersistAttachedItemValue(
    DependencyObject objectWithProperty,
    DependencyProperty property)
  {
    if (DependencyPropertyHelper.GetValueSource(objectWithProperty, property).BaseValueSource != BaseValueSource.Local)
      return;
    this._owner.ItemAttachedStorage.SetValue(this.Item, property, objectWithProperty.GetValue(property));
    objectWithProperty.ClearValue(property);
  }

  private void RestoreAttachedItemValue(
    DependencyObject objectWithProperty,
    DependencyProperty property)
  {
    object obj;
    if (!this._owner.ItemAttachedStorage.TryGetValue(this.Item, property, out obj))
      return;
    objectWithProperty.SetValue(property, obj);
  }

  internal ContainerTracking<DataGridRow> Tracker => this._tracker;

  internal void OnRowResizeStarted()
  {
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    if (cellsPresenter == null)
      return;
    this._cellsPresenterResizeHeight = cellsPresenter.Height;
  }

  internal void OnRowResize(double changeAmount)
  {
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    if (cellsPresenter == null)
      return;
    double num1 = cellsPresenter.ActualHeight + changeAmount;
    double num2 = Math.Max(this.RowHeader.DesiredSize.Height, this.MinHeight);
    if (DoubleUtil.LessThan(num1, num2))
      num1 = num2;
    double maxHeight = this.MaxHeight;
    if (DoubleUtil.GreaterThan(num1, maxHeight))
      num1 = maxHeight;
    cellsPresenter.Height = num1;
  }

  internal void OnRowResizeCompleted(bool canceled)
  {
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    if (cellsPresenter == null || !canceled)
      return;
    cellsPresenter.Height = this._cellsPresenterResizeHeight;
  }

  internal void OnRowResizeReset()
  {
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    if (cellsPresenter == null)
      return;
    cellsPresenter.ClearValue(FrameworkElement.HeightProperty);
    if (this._owner == null)
      return;
    this._owner.ItemAttachedStorage.ClearValue(this.Item, FrameworkElement.HeightProperty);
  }

  protected internal virtual void OnColumnsChanged(
    ObservableCollection<DataGridColumn> columns,
    NotifyCollectionChangedEventArgs e)
  {
    this.CellsPresenter?.OnColumnsChanged(columns, e);
  }

  private static object OnCoerceHeaderStyle(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridRow.HeaderStyleProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowHeaderStyleProperty);
  }

  private static object OnCoerceHeaderTemplate(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridRow.HeaderTemplateProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowHeaderTemplateProperty);
  }

  private static object OnCoerceHeaderTemplateSelector(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridRow.HeaderTemplateSelectorProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowHeaderTemplateSelectorProperty);
  }

  private static object OnCoerceBackground(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    object obj = baseValue;
    switch (baseObject.AlternationIndex)
    {
      case 0:
        obj = DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, Control.BackgroundProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowBackgroundProperty);
        break;
      case 1:
        obj = DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, Control.BackgroundProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.AlternatingRowBackgroundProperty);
        break;
    }
    return obj;
  }

  private static object OnCoerceValidationErrorTemplate(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridRow.ValidationErrorTemplateProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowValidationErrorTemplateProperty);
  }

  private static object OnCoerceDetailsTemplate(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridRow.DetailsTemplateProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowDetailsTemplateProperty);
  }

  private static object OnCoerceDetailsTemplateSelector(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridRow.DetailsTemplateSelectorProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowDetailsTemplateSelectorProperty);
  }

  private static object OnCoerceDetailsVisibility(DependencyObject d, object baseValue)
  {
    DataGridRow baseObject = (DataGridRow) d;
    object obj = DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, DataGridRow.DetailsVisibilityProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.RowDetailsVisibilityModeProperty);
    if (obj is DataGridRowDetailsVisibilityMode detailsVisibilityMode)
    {
      bool flag1 = baseObject.DetailsTemplate != null || baseObject.DetailsTemplateSelector != null;
      bool flag2 = baseObject.Item != CollectionView.NewItemPlaceholder;
      switch (detailsVisibilityMode)
      {
        case DataGridRowDetailsVisibilityMode.Collapsed:
          obj = (object) Visibility.Collapsed;
          break;
        case DataGridRowDetailsVisibilityMode.Visible:
          obj = (object) (Visibility) (!flag1 || !flag2 ? 2 : 0);
          break;
        case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
          obj = (object) (Visibility) (!baseObject.IsSelected || !flag1 || !flag2 ? 2 : 0);
          break;
        default:
          obj = (object) Visibility.Collapsed;
          break;
      }
    }
    return obj;
  }

  private static object OnCoerceVisibility(DependencyObject d, object baseValue)
  {
    DataGridRow dataGridRow = (DataGridRow) d;
    DataGrid dataGridOwner = dataGridRow.DataGridOwner;
    return dataGridRow.Item == CollectionView.NewItemPlaceholder && dataGridOwner != null ? (object) dataGridOwner.PlaceholderVisibility : baseValue;
  }

  private static void OnNotifyRowPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    (d as DataGridRow).NotifyPropertyChanged(d, e, NotificationTarget.Rows);
  }

  private static void OnNotifyRowAndRowHeaderPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    (d as DataGridRow).NotifyPropertyChanged(d, e, NotificationTarget.RowHeaders | NotificationTarget.Rows);
  }

  private static void OnNotifyDetailsTemplatePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridRow dataGridRow = (DataGridRow) d;
    dataGridRow.NotifyPropertyChanged((DependencyObject) dataGridRow, e, NotificationTarget.DetailsPresenter | NotificationTarget.Rows);
    if (!dataGridRow.DetailsLoaded || d.GetValue(e.Property) != e.NewValue)
      return;
    if (dataGridRow.DataGridOwner != null)
      dataGridRow.DataGridOwner.OnUnloadingRowDetailsWrapper(dataGridRow);
    if (e.NewValue == null)
      return;
    Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(DataGrid.DelayedOnLoadingRowDetails), DispatcherPriority.Loaded, (object) dataGridRow);
  }

  private static void OnNotifyDetailsVisibilityChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridRow dataGridRow = (DataGridRow) d;
    Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(DataGridRow.DelayedRowDetailsVisibilityChanged), DispatcherPriority.Loaded, (object) dataGridRow);
    dataGridRow.NotifyPropertyChanged(d, e, NotificationTarget.DetailsPresenter | NotificationTarget.Rows);
  }

  private static object DelayedRowDetailsVisibilityChanged(object arg)
  {
    DataGridRow row = (DataGridRow) arg;
    DataGrid dataGridOwner = row.DataGridOwner;
    FrameworkElement detailsElement = row.DetailsPresenter != null ? row.DetailsPresenter.DetailsElement : (FrameworkElement) null;
    if (dataGridOwner != null)
    {
      DataGridRowDetailsEventArgs e = new DataGridRowDetailsEventArgs(row, detailsElement);
      dataGridOwner.OnRowDetailsVisibilityChanged(e);
    }
    return (object) null;
  }

  internal Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter CellsPresenter
  {
    get => this._cellsPresenter;
    set => this._cellsPresenter = value;
  }

  internal Microsoft.Windows.Controls.Primitives.DataGridDetailsPresenter DetailsPresenter
  {
    get => this._detailsPresenter;
    set => this._detailsPresenter = value;
  }

  internal Microsoft.Windows.Controls.Primitives.DataGridRowHeader RowHeader
  {
    get => this._rowHeader;
    set => this._rowHeader = value;
  }

  internal void NotifyPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e,
    NotificationTarget target)
  {
    this.NotifyPropertyChanged(d, string.Empty, e, target);
  }

  internal void NotifyPropertyChanged(
    DependencyObject d,
    string propertyName,
    DependencyPropertyChangedEventArgs e,
    NotificationTarget target)
  {
    if (DataGridHelper.ShouldNotifyRows(target))
    {
      if (e.Property == DataGrid.RowBackgroundProperty || e.Property == DataGrid.AlternatingRowBackgroundProperty || e.Property == Control.BackgroundProperty || e.Property == DataGridRow.AlternationIndexProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, Control.BackgroundProperty);
      else if (e.Property == DataGrid.RowHeaderStyleProperty || e.Property == DataGridRow.HeaderStyleProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.HeaderStyleProperty);
      else if (e.Property == DataGrid.RowHeaderTemplateProperty || e.Property == DataGridRow.HeaderTemplateProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.HeaderTemplateProperty);
      else if (e.Property == DataGrid.RowHeaderTemplateSelectorProperty || e.Property == DataGridRow.HeaderTemplateSelectorProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.HeaderTemplateSelectorProperty);
      else if (e.Property == DataGrid.RowValidationErrorTemplateProperty || e.Property == DataGridRow.ValidationErrorTemplateProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.ValidationErrorTemplateProperty);
      else if (e.Property == DataGrid.RowDetailsTemplateProperty || e.Property == DataGridRow.DetailsTemplateProperty)
      {
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsTemplateProperty);
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsVisibilityProperty);
      }
      else if (e.Property == DataGrid.RowDetailsTemplateSelectorProperty || e.Property == DataGridRow.DetailsTemplateSelectorProperty)
      {
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsTemplateSelectorProperty);
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsVisibilityProperty);
      }
      else if (e.Property == DataGrid.RowDetailsVisibilityModeProperty || e.Property == DataGridRow.DetailsVisibilityProperty || e.Property == DataGridRow.IsSelectedProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsVisibilityProperty);
      else if (e.Property == DataGridRow.ItemProperty)
        this.OnItemChanged(e.OldValue, e.NewValue);
      else if (e.Property == DataGridRow.HeaderProperty)
        this.OnHeaderChanged(e.OldValue, e.NewValue);
      else if (e.Property == FrameworkElement.BindingGroupProperty)
        this.Dispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(this.DelayedValidateWithoutUpdate), DispatcherPriority.DataBind, e.NewValue);
    }
    if (DataGridHelper.ShouldNotifyDetailsPresenter(target) && this.DetailsPresenter != null)
      this.DetailsPresenter.NotifyPropertyChanged(d, e);
    if (DataGridHelper.ShouldNotifyCellsPresenter(target) || DataGridHelper.ShouldNotifyCells(target) || DataGridHelper.ShouldRefreshCellContent(target))
      this.CellsPresenter?.NotifyPropertyChanged(d, propertyName, e, target);
    if (!DataGridHelper.ShouldNotifyRowHeaders(target) || this.RowHeader == null)
      return;
    this.RowHeader.NotifyPropertyChanged(d, e);
  }

  private object DelayedValidateWithoutUpdate(object arg)
  {
    BindingGroup bindingGroup = (BindingGroup) arg;
    if (bindingGroup != null && bindingGroup.Items.Count > 0)
      bindingGroup.ValidateWithoutUpdate();
    return (object) null;
  }

  private void SyncProperties(bool forcePrepareCells)
  {
    DataGridHelper.TransferProperty((DependencyObject) this, Control.BackgroundProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.HeaderStyleProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.HeaderTemplateProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.HeaderTemplateSelectorProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.ValidationErrorTemplateProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsTemplateProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsTemplateSelectorProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridRow.DetailsVisibilityProperty);
    this.CoerceValue(UIElement.VisibilityProperty);
    this.RestoreAttachedItemValue((DependencyObject) this, DataGridRow.DetailsVisibilityProperty);
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    if (cellsPresenter != null)
    {
      cellsPresenter.SyncProperties(forcePrepareCells);
      this.RestoreAttachedItemValue((DependencyObject) cellsPresenter, FrameworkElement.HeightProperty);
    }
    if (this.DetailsPresenter != null)
      this.DetailsPresenter.SyncProperties();
    if (this.RowHeader == null)
      return;
    this.RowHeader.SyncProperties();
  }

  public int AlternationIndex => (int) this.GetValue(DataGridRow.AlternationIndexProperty);

  [Bindable(true)]
  [Category("Appearance")]
  public bool IsSelected
  {
    get => (bool) this.GetValue(DataGridRow.IsSelectedProperty);
    set => this.SetValue(DataGridRow.IsSelectedProperty, (object) value);
  }

  private static void OnIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    DataGridRow d = (DataGridRow) sender;
    bool newValue = (bool) e.NewValue;
    if (newValue && !d.IsSelectable)
      throw new InvalidOperationException(SR.Get(SRID.DataGridRow_CannotSelectRowWhenCells));
    DataGrid dataGridOwner = d.DataGridOwner;
    if (dataGridOwner != null && d.DataContext != null && UIElementAutomationPeer.FromElement((UIElement) dataGridOwner) is Microsoft.Windows.Automation.Peers.DataGridAutomationPeer gridAutomationPeer)
      gridAutomationPeer.GetOrCreateItemPeer(d.DataContext)?.RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, (object) (bool) e.OldValue, (object) newValue);
    d.NotifyPropertyChanged((DependencyObject) d, e, NotificationTarget.RowHeaders | NotificationTarget.Rows);
    d.RaiseSelectionChangedEvent(newValue);
  }

  private void RaiseSelectionChangedEvent(bool isSelected)
  {
    if (isSelected)
      this.OnSelected(new RoutedEventArgs(DataGridRow.SelectedEvent, (object) this));
    else
      this.OnUnselected(new RoutedEventArgs(DataGridRow.UnselectedEvent, (object) this));
  }

  public event RoutedEventHandler Selected
  {
    add => this.AddHandler(DataGridRow.SelectedEvent, (Delegate) value);
    remove => this.RemoveHandler(DataGridRow.SelectedEvent, (Delegate) value);
  }

  protected virtual void OnSelected(RoutedEventArgs e) => this.RaiseEvent(e);

  public event RoutedEventHandler Unselected
  {
    add => this.AddHandler(DataGridRow.UnselectedEvent, (Delegate) value);
    remove => this.RemoveHandler(DataGridRow.UnselectedEvent, (Delegate) value);
  }

  protected virtual void OnUnselected(RoutedEventArgs e) => this.RaiseEvent(e);

  private bool IsSelectable
  {
    get
    {
      DataGrid dataGridOwner = this.DataGridOwner;
      if (dataGridOwner == null)
        return true;
      DataGridSelectionUnit selectionUnit = dataGridOwner.SelectionUnit;
      return selectionUnit == DataGridSelectionUnit.FullRow || selectionUnit == DataGridSelectionUnit.CellOrRowHeader;
    }
  }

  public bool IsEditing
  {
    get => (bool) this.GetValue(DataGridRow.IsEditingProperty);
    internal set => this.SetValue(DataGridRow.IsEditingPropertyKey, (object) value);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DataGridRowAutomationPeer(this);
  }

  internal void ScrollCellIntoView(int index) => this.CellsPresenter?.ScrollCellIntoView(index);

  protected override Size ArrangeOverride(Size arrangeBounds)
  {
    this.DataGridOwner?.QueueInvalidateCellsPanelHorizontalOffset();
    return base.ArrangeOverride(arrangeBounds);
  }

  public int GetIndex()
  {
    DataGrid dataGridOwner = this.DataGridOwner;
    return dataGridOwner != null ? dataGridOwner.ItemContainerGenerator.IndexFromContainer((DependencyObject) this) : -1;
  }

  public static DataGridRow GetRowContainingElement(FrameworkElement element)
  {
    return DataGridHelper.FindVisualParent<DataGridRow>((UIElement) element);
  }

  internal DataGrid DataGridOwner => this._owner;

  internal bool DetailsPresenterDrawsGridLines
  {
    get
    {
      return this._detailsPresenter != null && this._detailsPresenter.Visibility == Visibility.Visible;
    }
  }

  internal DataGridCell TryGetCell(int index)
  {
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = this.CellsPresenter;
    return cellsPresenter != null ? cellsPresenter.ItemContainerGenerator.ContainerFromIndex(index) as DataGridCell : (DataGridCell) null;
  }
}
