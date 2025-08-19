// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGrid
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGrid : MultiSelector
{
  private const string ItemsPanelPartName = "PART_RowsPresenter";
  public static readonly DependencyProperty CanUserResizeColumnsProperty = DependencyProperty.Register(nameof (CanUserResizeColumns), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnNotifyColumnAndColumnHeaderPropertyChanged)));
  public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(nameof (ColumnWidth), typeof (DataGridLength), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridLength.SizeToHeader));
  public static readonly DependencyProperty MinColumnWidthProperty = DependencyProperty.Register(nameof (MinColumnWidth), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 20.0, new PropertyChangedCallback(DataGrid.OnColumnSizeConstraintChanged)), new ValidateValueCallback(DataGrid.ValidateMinColumnWidth));
  public static readonly DependencyProperty MaxColumnWidthProperty = DependencyProperty.Register(nameof (MaxColumnWidth), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.PositiveInfinity, new PropertyChangedCallback(DataGrid.OnColumnSizeConstraintChanged)), new ValidateValueCallback(DataGrid.ValidateMaxColumnWidth));
  public static readonly DependencyProperty GridLinesVisibilityProperty = DependencyProperty.Register(nameof (GridLinesVisibility), typeof (DataGridGridLinesVisibility), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridGridLinesVisibility.All, new PropertyChangedCallback(DataGrid.OnNotifyGridLinePropertyChanged)));
  public static readonly DependencyProperty HorizontalGridLinesBrushProperty = DependencyProperty.Register(nameof (HorizontalGridLinesBrush), typeof (Brush), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Black, new PropertyChangedCallback(DataGrid.OnNotifyGridLinePropertyChanged)));
  public static readonly DependencyProperty VerticalGridLinesBrushProperty = DependencyProperty.Register(nameof (VerticalGridLinesBrush), typeof (Brush), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Black, new PropertyChangedCallback(DataGrid.OnNotifyGridLinePropertyChanged)));
  public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register(nameof (RowStyle), typeof (Style), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnRowStyleChanged)));
  public static readonly DependencyProperty RowValidationErrorTemplateProperty = DependencyProperty.Register(nameof (RowValidationErrorTemplate), typeof (ControlTemplate), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyRowPropertyChanged)));
  public static readonly DependencyProperty RowStyleSelectorProperty = DependencyProperty.Register(nameof (RowStyleSelector), typeof (StyleSelector), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnRowStyleSelectorChanged)));
  public static readonly DependencyProperty RowBackgroundProperty = DependencyProperty.Register(nameof (RowBackground), typeof (Brush), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyRowPropertyChanged)));
  public static readonly DependencyProperty AlternatingRowBackgroundProperty = DependencyProperty.Register(nameof (AlternatingRowBackground), typeof (Brush), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyDataGridAndRowPropertyChanged)));
  public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register(nameof (RowHeight), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(DataGrid.OnNotifyCellsPresenterPropertyChanged)));
  public static readonly DependencyProperty MinRowHeightProperty = DependencyProperty.Register(nameof (MinRowHeight), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, new PropertyChangedCallback(DataGrid.OnNotifyCellsPresenterPropertyChanged)));
  public static readonly DependencyProperty RowHeaderWidthProperty = DependencyProperty.Register(nameof (RowHeaderWidth), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(DataGrid.OnNotifyRowHeaderWidthPropertyChanged)));
  private static readonly DependencyPropertyKey RowHeaderActualWidthPropertyKey = DependencyProperty.RegisterReadOnly(nameof (RowHeaderActualWidth), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, new PropertyChangedCallback(DataGrid.OnNotifyRowHeaderPropertyChanged)));
  public static readonly DependencyProperty RowHeaderActualWidthProperty = DataGrid.RowHeaderActualWidthPropertyKey.DependencyProperty;
  public static readonly DependencyProperty ColumnHeaderHeightProperty = DependencyProperty.Register(nameof (ColumnHeaderHeight), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(DataGrid.OnNotifyColumnHeaderPropertyChanged)));
  public static readonly DependencyProperty HeadersVisibilityProperty = DependencyProperty.Register(nameof (HeadersVisibility), typeof (DataGridHeadersVisibility), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridHeadersVisibility.All));
  public static readonly DependencyProperty CellStyleProperty = DependencyProperty.Register(nameof (CellStyle), typeof (Style), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyColumnAndCellPropertyChanged)));
  public static readonly DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register(nameof (ColumnHeaderStyle), typeof (Style), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyColumnAndColumnHeaderPropertyChanged)));
  public static readonly DependencyProperty RowHeaderStyleProperty = DependencyProperty.Register(nameof (RowHeaderStyle), typeof (Style), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyRowAndRowHeaderPropertyChanged)));
  public static readonly DependencyProperty RowHeaderTemplateProperty = DependencyProperty.Register(nameof (RowHeaderTemplate), typeof (DataTemplate), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyRowAndRowHeaderPropertyChanged)));
  public static readonly DependencyProperty RowHeaderTemplateSelectorProperty = DependencyProperty.Register(nameof (RowHeaderTemplateSelector), typeof (DataTemplateSelector), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyRowAndRowHeaderPropertyChanged)));
  public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) ScrollBarVisibility.Auto));
  public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) ScrollBarVisibility.Auto));
  internal static readonly DependencyProperty HorizontalScrollOffsetProperty = DependencyProperty.Register(nameof (HorizontalScrollOffset), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, new PropertyChangedCallback(DataGrid.OnNotifyHorizontalOffsetPropertyChanged)));
  public static readonly RoutedCommand BeginEditCommand = new RoutedCommand("BeginEdit", typeof (DataGrid));
  public static readonly RoutedCommand CommitEditCommand = new RoutedCommand("CommitEdit", typeof (DataGrid));
  public static readonly RoutedCommand CancelEditCommand = new RoutedCommand("CancelEdit", typeof (DataGrid));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGrid.OnIsReadOnlyChanged)));
  public static readonly DependencyProperty CurrentItemProperty = DependencyProperty.Register(nameof (CurrentItem), typeof (object), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnCurrentItemChanged)));
  public static readonly DependencyProperty CurrentColumnProperty = DependencyProperty.Register(nameof (CurrentColumn), typeof (DataGridColumn), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnCurrentColumnChanged)));
  public static readonly DependencyProperty CurrentCellProperty = DependencyProperty.Register(nameof (CurrentCell), typeof (DataGridCellInfo), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridCellInfo.Unset, new PropertyChangedCallback(DataGrid.OnCurrentCellChanged)));
  public static readonly DependencyProperty CanUserAddRowsProperty = DependencyProperty.Register(nameof (CanUserAddRows), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnCanUserAddRowsChanged), new CoerceValueCallback(DataGrid.OnCoerceCanUserAddRows)));
  public static readonly DependencyProperty CanUserDeleteRowsProperty = DependencyProperty.Register(nameof (CanUserDeleteRows), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnCanUserDeleteRowsChanged), new CoerceValueCallback(DataGrid.OnCoerceCanUserDeleteRows)));
  public static readonly DependencyProperty RowDetailsVisibilityModeProperty = DependencyProperty.Register(nameof (RowDetailsVisibilityMode), typeof (DataGridRowDetailsVisibilityMode), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridRowDetailsVisibilityMode.VisibleWhenSelected, new PropertyChangedCallback(DataGrid.OnNotifyRowAndDetailsPropertyChanged)));
  public static readonly DependencyProperty AreRowDetailsFrozenProperty = DependencyProperty.Register(nameof (AreRowDetailsFrozen), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty RowDetailsTemplateProperty = DependencyProperty.Register(nameof (RowDetailsTemplate), typeof (DataTemplate), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyRowAndDetailsPropertyChanged)));
  public static readonly DependencyProperty RowDetailsTemplateSelectorProperty = DependencyProperty.Register(nameof (RowDetailsTemplateSelector), typeof (DataTemplateSelector), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyRowAndDetailsPropertyChanged)));
  public static readonly DependencyProperty CanUserResizeRowsProperty = DependencyProperty.Register(nameof (CanUserResizeRows), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnNotifyRowHeaderPropertyChanged)));
  public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof (SelectionMode), typeof (DataGridSelectionMode), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridSelectionMode.Extended, new PropertyChangedCallback(DataGrid.OnSelectionModeChanged)));
  public static readonly DependencyProperty SelectionUnitProperty = DependencyProperty.Register(nameof (SelectionUnit), typeof (DataGridSelectionUnit), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridSelectionUnit.FullRow, new PropertyChangedCallback(DataGrid.OnSelectionUnitChanged)));
  public static readonly DependencyProperty CanUserSortColumnsProperty = DependencyProperty.Register(nameof (CanUserSortColumns), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnCanUserSortColumnsPropertyChanged), new CoerceValueCallback(DataGrid.OnCoerceCanUserSortColumns)));
  public static readonly DependencyProperty AutoGenerateColumnsProperty = DependencyProperty.Register(nameof (AutoGenerateColumns), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnAutoGenerateColumnsPropertyChanged)));
  public static readonly DependencyProperty FrozenColumnCountProperty = DependencyProperty.Register(nameof (FrozenColumnCount), typeof (int), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0, new PropertyChangedCallback(DataGrid.OnFrozenColumnCountPropertyChanged), new CoerceValueCallback(DataGrid.OnCoerceFrozenColumnCount)), new ValidateValueCallback(DataGrid.ValidateFrozenColumnCount));
  private static readonly DependencyPropertyKey NonFrozenColumnsViewportHorizontalOffsetPropertyKey = DependencyProperty.RegisterReadOnly(nameof (NonFrozenColumnsViewportHorizontalOffset), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
  public static readonly DependencyProperty NonFrozenColumnsViewportHorizontalOffsetProperty = DataGrid.NonFrozenColumnsViewportHorizontalOffsetPropertyKey.DependencyProperty;
  public static readonly DependencyProperty EnableRowVirtualizationProperty = DependencyProperty.Register(nameof (EnableRowVirtualization), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnEnableRowVirtualizationChanged)));
  public static readonly DependencyProperty EnableColumnVirtualizationProperty = DependencyProperty.Register(nameof (EnableColumnVirtualization), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGrid.OnEnableColumnVirtualizationChanged)));
  public static readonly DependencyProperty CanUserReorderColumnsProperty = DependencyProperty.Register(nameof (CanUserReorderColumns), typeof (bool), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(DataGrid.OnNotifyColumnPropertyChanged)));
  public static readonly DependencyProperty DragIndicatorStyleProperty = DependencyProperty.Register(nameof (DragIndicatorStyle), typeof (Style), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGrid.OnNotifyColumnPropertyChanged)));
  public static readonly DependencyProperty DropLocationIndicatorStyleProperty = DependencyProperty.Register(nameof (DropLocationIndicatorStyle), typeof (Style), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ClipboardCopyModeProperty = DependencyProperty.Register(nameof (ClipboardCopyMode), typeof (DataGridClipboardCopyMode), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) DataGridClipboardCopyMode.ExcludeHeader, new PropertyChangedCallback(DataGrid.OnClipboardCopyModeChanged)));
  internal static readonly DependencyProperty CellsPanelActualWidthProperty = DependencyProperty.Register(nameof (CellsPanelActualWidth), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, new PropertyChangedCallback(DataGrid.CellsPanelActualWidthChanged)));
  private static readonly DependencyPropertyKey CellsPanelHorizontalOffsetPropertyKey = DependencyProperty.RegisterReadOnly(nameof (CellsPanelHorizontalOffset), typeof (double), typeof (DataGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, new PropertyChangedCallback(DataGrid.OnNotifyHorizontalOffsetPropertyChanged)));
  public static readonly DependencyProperty CellsPanelHorizontalOffsetProperty = DataGrid.CellsPanelHorizontalOffsetPropertyKey.DependencyProperty;
  private static ComponentResourceKey _focusBorderBrushKey;
  private static IValueConverter _headersVisibilityConverter;
  private static IValueConverter _rowDetailsScrollingConverter;
  private static object _newItemPlaceholder = new object();
  private DataGridColumnCollection _columns;
  private ContainerTracking<DataGridRow> _rowTrackingRoot;
  private Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter _columnHeadersPresenter;
  private DataGridCell _currentCellContainer;
  private DataGridCell _pendingCurrentCellContainer;
  private SelectedCellsCollection _selectedCells;
  private DataGridCellInfo? _selectionAnchor;
  private bool _isDraggingSelection;
  private bool _isRowDragging;
  private Panel _internalItemsHost;
  private ScrollViewer _internalScrollHost;
  private ScrollContentPresenter _internalScrollContentPresenter;
  private DispatcherTimer _autoScrollTimer;
  private bool _hasAutoScrolled;
  private VirtualizedCellInfoCollection _pendingSelectedCells;
  private VirtualizedCellInfoCollection _pendingUnselectedCells;
  private bool _measureNeverInvoked = true;
  private bool _updatingSelectedCells;
  private Visibility _placeholderVisibility = Visibility.Collapsed;
  private Point _dragPoint;
  private List<int> _groupingSortDescriptionIndices;
  private bool _ignoreSortDescriptionsChange;
  private bool _sortingStarted;
  private ObservableCollection<ValidationRule> _rowValidationRules;
  private BindingGroup _rowValidationBindingGroup;
  private object _editingRowItem;
  private int _editingRowIndex = -1;
  private bool _hasCellValidationError;
  private bool _hasRowValidationError;
  private IEnumerable _cachedItemsSource;
  private DataGridItemAttachedStorage _itemAttachedStorage = new DataGridItemAttachedStorage();
  private bool _viewportWidthChangeNotificationPending;
  private double _originalViewportWidth;
  private double _finalViewportWidth;
  private DataGridCell _focusedCell;

  static DataGrid()
  {
    Type type = typeof (DataGrid);
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGrid)));
    FrameworkElementFactory root = new FrameworkElementFactory(typeof (Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter));
    root.SetValue(FrameworkElement.NameProperty, (object) "PART_RowsPresenter");
    ItemsControl.ItemsPanelProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) new ItemsPanelTemplate(root)));
    VirtualizingStackPanel.IsVirtualizingProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) true, (PropertyChangedCallback) null, new CoerceValueCallback(DataGrid.OnCoerceIsVirtualizingProperty)));
    VirtualizingStackPanel.VirtualizationModeProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) VirtualizationMode.Recycling));
    ItemsControl.ItemContainerStyleProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGrid.OnCoerceItemContainerStyle)));
    ItemsControl.ItemContainerStyleSelectorProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGrid.OnCoerceItemContainerStyleSelector)));
    ItemsControl.ItemsSourceProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGrid.OnCoerceItemsSourceProperty)));
    ItemsControl.AlternationCountProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) 0, (PropertyChangedCallback) null, new CoerceValueCallback(DataGrid.OnCoerceAlternationCount)));
    UIElement.IsEnabledProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGrid.OnIsEnabledChanged)));
    Selector.IsSynchronizedWithCurrentItemProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGrid.OnCoerceIsSynchronizedWithCurrentItem)));
    Control.IsTabStopProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Contained));
    KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Once));
    CommandManager.RegisterClassInputBinding(type, new InputBinding((ICommand) DataGrid.BeginEditCommand, (InputGesture) new KeyGesture(Key.F2)));
    CommandManager.RegisterClassCommandBinding(type, new CommandBinding((ICommand) DataGrid.BeginEditCommand, new ExecutedRoutedEventHandler(DataGrid.OnExecutedBeginEdit), new CanExecuteRoutedEventHandler(DataGrid.OnCanExecuteBeginEdit)));
    CommandManager.RegisterClassCommandBinding(type, new CommandBinding((ICommand) DataGrid.CommitEditCommand, new ExecutedRoutedEventHandler(DataGrid.OnExecutedCommitEdit), new CanExecuteRoutedEventHandler(DataGrid.OnCanExecuteCommitEdit)));
    CommandManager.RegisterClassInputBinding(type, new InputBinding((ICommand) DataGrid.CancelEditCommand, (InputGesture) new KeyGesture(Key.Escape)));
    CommandManager.RegisterClassCommandBinding(type, new CommandBinding((ICommand) DataGrid.CancelEditCommand, new ExecutedRoutedEventHandler(DataGrid.OnExecutedCancelEdit), new CanExecuteRoutedEventHandler(DataGrid.OnCanExecuteCancelEdit)));
    CommandManager.RegisterClassCommandBinding(type, new CommandBinding((ICommand) DataGrid.SelectAllCommand, new ExecutedRoutedEventHandler(DataGrid.OnExecutedSelectAll), new CanExecuteRoutedEventHandler(DataGrid.OnCanExecuteSelectAll)));
    CommandManager.RegisterClassCommandBinding(type, new CommandBinding((ICommand) DataGrid.DeleteCommand, new ExecutedRoutedEventHandler(DataGrid.OnExecutedDelete), new CanExecuteRoutedEventHandler(DataGrid.OnCanExecuteDelete)));
    CommandManager.RegisterClassCommandBinding(typeof (DataGrid), new CommandBinding((ICommand) ApplicationCommands.Copy, new ExecutedRoutedEventHandler(DataGrid.OnExecutedCopy), new CanExecuteRoutedEventHandler(DataGrid.OnCanExecuteCopy)));
    EventManager.RegisterClassHandler(typeof (DataGrid), UIElement.MouseUpEvent, (Delegate) new MouseButtonEventHandler(DataGrid.OnAnyMouseUpThunk), true);
  }

  public DataGrid()
  {
    this._columns = new DataGridColumnCollection(this);
    this._columns.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnColumnsChanged);
    this._rowValidationRules = new ObservableCollection<ValidationRule>();
    this._rowValidationRules.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnRowValidationRulesChanged);
    this._selectedCells = new SelectedCellsCollection(this);
    ((INotifyCollectionChanged) this.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnItemsCollectionChanged);
    ((INotifyCollectionChanged) this.Items.SortDescriptions).CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnItemsSortDescriptionsChanged);
    this.Items.GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnItemsGroupDescriptionsChanged);
    this.InternalColumns.InvalidateColumnWidthsComputation();
    this.CellsPanelHorizontalOffsetComputationPending = false;
  }

  public ObservableCollection<DataGridColumn> Columns
  {
    get => (ObservableCollection<DataGridColumn>) this._columns;
  }

  internal DataGridColumnCollection InternalColumns => this._columns;

  public bool CanUserResizeColumns
  {
    get => (bool) this.GetValue(DataGrid.CanUserResizeColumnsProperty);
    set => this.SetValue(DataGrid.CanUserResizeColumnsProperty, (object) value);
  }

  public DataGridLength ColumnWidth
  {
    get => (DataGridLength) this.GetValue(DataGrid.ColumnWidthProperty);
    set => this.SetValue(DataGrid.ColumnWidthProperty, (object) value);
  }

  public double MinColumnWidth
  {
    get => (double) this.GetValue(DataGrid.MinColumnWidthProperty);
    set => this.SetValue(DataGrid.MinColumnWidthProperty, (object) value);
  }

  public double MaxColumnWidth
  {
    get => (double) this.GetValue(DataGrid.MaxColumnWidthProperty);
    set => this.SetValue(DataGrid.MaxColumnWidthProperty, (object) value);
  }

  private static void OnColumnSizeConstraintChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.Columns);
  }

  private static bool ValidateMinColumnWidth(object v)
  {
    double d = (double) v;
    return d >= 0.0 && !DoubleUtil.IsNaN(d) && !double.IsPositiveInfinity(d);
  }

  private static bool ValidateMaxColumnWidth(object v)
  {
    double num = (double) v;
    return num >= 0.0 && !DoubleUtil.IsNaN(num);
  }

  private void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        this.UpdateDataGridReference(e.NewItems, false);
        DataGrid.UpdateColumnSizeConstraints(e.NewItems);
        break;
      case NotifyCollectionChangedAction.Remove:
        this.UpdateDataGridReference(e.OldItems, true);
        break;
      case NotifyCollectionChangedAction.Replace:
        this.UpdateDataGridReference(e.OldItems, true);
        this.UpdateDataGridReference(e.NewItems, false);
        DataGrid.UpdateColumnSizeConstraints(e.NewItems);
        break;
      case NotifyCollectionChangedAction.Reset:
        this._selectedCells.Clear();
        break;
    }
    if (this.InternalColumns.DisplayIndexMapInitialized)
      this.CoerceValue(DataGrid.FrozenColumnCountProperty);
    bool flag = DataGrid.HasVisibleColumns(e.OldItems) | DataGrid.HasVisibleColumns(e.NewItems) | e.Action == NotifyCollectionChangedAction.Reset;
    if (flag)
      this.InternalColumns.InvalidateColumnRealization(true);
    this.UpdateColumnsOnRows(e);
    if (!flag || e.Action == NotifyCollectionChangedAction.Move)
      return;
    this.InternalColumns.InvalidateColumnWidthsComputation();
  }

  internal void UpdateDataGridReference(IList list, bool clear)
  {
    int count = list.Count;
    for (int index = 0; index < count; ++index)
    {
      DataGridColumn dataGridColumn = (DataGridColumn) list[index];
      if (clear)
      {
        if (dataGridColumn.DataGridOwner == this)
          dataGridColumn.DataGridOwner = (DataGrid) null;
      }
      else
      {
        if (dataGridColumn.DataGridOwner != null && dataGridColumn.DataGridOwner != this)
          dataGridColumn.DataGridOwner.Columns.Remove(dataGridColumn);
        dataGridColumn.DataGridOwner = this;
      }
    }
  }

  private static void UpdateColumnSizeConstraints(IList list)
  {
    int count = list.Count;
    for (int index = 0; index < count; ++index)
      ((DataGridColumn) list[index]).SyncProperties();
  }

  private static bool HasVisibleColumns(IList columns)
  {
    if (columns != null && columns.Count > 0)
    {
      foreach (DataGridColumn column in (IEnumerable) columns)
      {
        if (column.IsVisible)
          return true;
      }
    }
    return false;
  }

  public DataGridColumn ColumnFromDisplayIndex(int displayIndex)
  {
    if (displayIndex < 0 || displayIndex >= this.Columns.Count)
      throw new ArgumentOutOfRangeException(nameof (displayIndex), (object) displayIndex, SR.Get(SRID.DataGrid_DisplayIndexOutOfRange));
    return this.InternalColumns.ColumnFromDisplayIndex(displayIndex);
  }

  public event EventHandler<DataGridColumnEventArgs> ColumnDisplayIndexChanged;

  protected internal virtual void OnColumnDisplayIndexChanged(DataGridColumnEventArgs e)
  {
    if (this.ColumnDisplayIndexChanged == null)
      return;
    this.ColumnDisplayIndexChanged((object) this, e);
  }

  internal List<int> DisplayIndexMap => this.InternalColumns.DisplayIndexMap;

  internal void ValidateDisplayIndex(DataGridColumn column, int displayIndex)
  {
    this.InternalColumns.ValidateDisplayIndex(column, displayIndex);
  }

  internal int ColumnIndexFromDisplayIndex(int displayIndex)
  {
    return displayIndex >= 0 && displayIndex < this.DisplayIndexMap.Count ? this.DisplayIndexMap[displayIndex] : -1;
  }

  internal Microsoft.Windows.Controls.Primitives.DataGridColumnHeader ColumnHeaderFromDisplayIndex(
    int displayIndex)
  {
    int index = this.ColumnIndexFromDisplayIndex(displayIndex);
    return index != -1 && this.ColumnHeadersPresenter != null && this.ColumnHeadersPresenter.ItemContainerGenerator != null ? (Microsoft.Windows.Controls.Primitives.DataGridColumnHeader) this.ColumnHeadersPresenter.ItemContainerGenerator.ContainerFromIndex(index) : (Microsoft.Windows.Controls.Primitives.DataGridColumnHeader) null;
  }

  private static void OnNotifyCellsPresenterPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.CellsPresenter);
  }

  private static void OnNotifyColumnAndCellPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.Cells | NotificationTarget.Columns);
  }

  private static void OnNotifyColumnPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.Columns);
  }

  private static void OnNotifyColumnAndColumnHeaderPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.Columns | NotificationTarget.ColumnHeaders);
  }

  private static void OnNotifyColumnHeaderPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.ColumnHeaders);
  }

  private static void OnNotifyHeaderPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.ColumnHeaders | NotificationTarget.RowHeaders);
  }

  private static void OnNotifyDataGridAndRowPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.DataGrid | NotificationTarget.Rows);
  }

  private static void OnNotifyGridLinePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue == e.NewValue)
      return;
    ((ItemsControl) d).OnItemTemplateChanged((DataTemplate) null, (DataTemplate) null);
  }

  private static void OnNotifyRowPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.Rows);
  }

  private static void OnNotifyRowHeaderPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.RowHeaders);
  }

  private static void OnNotifyRowAndRowHeaderPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.RowHeaders | NotificationTarget.Rows);
  }

  private static void OnNotifyRowAndDetailsPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.DetailsPresenter | NotificationTarget.Rows);
  }

  private static void OnNotifyHorizontalOffsetPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.CellsPresenter | NotificationTarget.ColumnCollection | NotificationTarget.ColumnHeadersPresenter);
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
    if (DataGridHelper.ShouldNotifyDataGrid(target) && e.Property == DataGrid.AlternatingRowBackgroundProperty)
      this.CoerceValue(ItemsControl.AlternationCountProperty);
    if (DataGridHelper.ShouldNotifyRowSubtree(target))
    {
      for (ContainerTracking<DataGridRow> containerTracking = this._rowTrackingRoot; containerTracking != null; containerTracking = containerTracking.Next)
        containerTracking.Container.NotifyPropertyChanged(d, propertyName, e, target);
    }
    if (DataGridHelper.ShouldNotifyColumnCollection(target) || DataGridHelper.ShouldNotifyColumns(target))
      this.InternalColumns.NotifyPropertyChanged(d, propertyName, e, target);
    if (!DataGridHelper.ShouldNotifyColumnHeadersPresenter(target) && !DataGridHelper.ShouldNotifyColumnHeaders(target) || this.ColumnHeadersPresenter == null)
      return;
    this.ColumnHeadersPresenter.NotifyPropertyChanged(d, propertyName, e, target);
  }

  internal void UpdateColumnsOnVirtualizedCellInfoCollections(
    NotifyCollectionChangedAction action,
    int oldDisplayIndex,
    DataGridColumn oldColumn,
    int newDisplayIndex)
  {
    using (this.UpdateSelectedCells())
      this._selectedCells.OnColumnsChanged(action, oldDisplayIndex, oldColumn, newDisplayIndex, this.SelectedItems);
  }

  internal Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter ColumnHeadersPresenter
  {
    get => this._columnHeadersPresenter;
    set => this._columnHeadersPresenter = value;
  }

  protected override void OnTemplateChanged(
    ControlTemplate oldTemplate,
    ControlTemplate newTemplate)
  {
    base.OnTemplateChanged(oldTemplate, newTemplate);
    this.ColumnHeadersPresenter = (Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter) null;
  }

  public DataGridGridLinesVisibility GridLinesVisibility
  {
    get => (DataGridGridLinesVisibility) this.GetValue(DataGrid.GridLinesVisibilityProperty);
    set => this.SetValue(DataGrid.GridLinesVisibilityProperty, (object) value);
  }

  public Brush HorizontalGridLinesBrush
  {
    get => (Brush) this.GetValue(DataGrid.HorizontalGridLinesBrushProperty);
    set => this.SetValue(DataGrid.HorizontalGridLinesBrushProperty, (object) value);
  }

  public Brush VerticalGridLinesBrush
  {
    get => (Brush) this.GetValue(DataGrid.VerticalGridLinesBrushProperty);
    set => this.SetValue(DataGrid.VerticalGridLinesBrushProperty, (object) value);
  }

  internal double HorizontalGridLineThickness => 1.0;

  internal double VerticalGridLineThickness => 1.0;

  protected override bool IsItemItsOwnContainerOverride(object item) => item is DataGridRow;

  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new DataGridRow();
  }

  protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
  {
    base.PrepareContainerForItemOverride(element, item);
    DataGridRow row = (DataGridRow) element;
    if (row.DataGridOwner != this)
    {
      row.Tracker.StartTracking(ref this._rowTrackingRoot);
      this.EnsureInternalScrollControls();
    }
    row.PrepareRow(item, this);
    this.OnLoadingRow(new DataGridRowEventArgs(row));
  }

  protected override void ClearContainerForItemOverride(DependencyObject element, object item)
  {
    base.ClearContainerForItemOverride(element, item);
    DataGridRow row = (DataGridRow) element;
    if (row.DataGridOwner == this)
      row.Tracker.StopTracking(ref this._rowTrackingRoot);
    this.OnUnloadingRow(new DataGridRowEventArgs(row));
    row.ClearRow(this);
  }

  private void UpdateColumnsOnRows(NotifyCollectionChangedEventArgs e)
  {
    for (ContainerTracking<DataGridRow> containerTracking = this._rowTrackingRoot; containerTracking != null; containerTracking = containerTracking.Next)
      containerTracking.Container.OnColumnsChanged((ObservableCollection<DataGridColumn>) this._columns, e);
  }

  public Style RowStyle
  {
    get => (Style) this.GetValue(DataGrid.RowStyleProperty);
    set => this.SetValue(DataGrid.RowStyleProperty, (object) value);
  }

  private static void OnRowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    d.CoerceValue(ItemsControl.ItemContainerStyleProperty);
  }

  private static object OnCoerceItemContainerStyle(DependencyObject d, object baseValue)
  {
    return !DataGridHelper.IsDefaultValue(d, DataGrid.RowStyleProperty) ? d.GetValue(DataGrid.RowStyleProperty) : baseValue;
  }

  public ControlTemplate RowValidationErrorTemplate
  {
    get => (ControlTemplate) this.GetValue(DataGrid.RowValidationErrorTemplateProperty);
    set => this.SetValue(DataGrid.RowValidationErrorTemplateProperty, (object) value);
  }

  public ObservableCollection<ValidationRule> RowValidationRules => this._rowValidationRules;

  private void OnRowValidationRulesChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    BindingGroup objA = this.ItemBindingGroup;
    if (objA == null)
    {
      this.ItemBindingGroup = objA = new BindingGroup();
      this._rowValidationBindingGroup = objA;
    }
    if (this._rowValidationBindingGroup == null)
      return;
    if (object.ReferenceEquals((object) objA, (object) this._rowValidationBindingGroup))
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          IEnumerator enumerator1 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
              this._rowValidationBindingGroup.ValidationRules.Add((ValidationRule) enumerator1.Current);
            break;
          }
          finally
          {
            if (enumerator1 is IDisposable disposable)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Remove:
          IEnumerator enumerator2 = e.OldItems.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
              this._rowValidationBindingGroup.ValidationRules.Remove((ValidationRule) enumerator2.Current);
            break;
          }
          finally
          {
            if (enumerator2 is IDisposable disposable)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Replace:
          foreach (ValidationRule oldItem in (IEnumerable) e.OldItems)
            this._rowValidationBindingGroup.ValidationRules.Remove(oldItem);
          IEnumerator enumerator3 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator3.MoveNext())
              this._rowValidationBindingGroup.ValidationRules.Add((ValidationRule) enumerator3.Current);
            break;
          }
          finally
          {
            if (enumerator3 is IDisposable disposable)
              disposable.Dispose();
          }
        case NotifyCollectionChangedAction.Reset:
          this._rowValidationBindingGroup.ValidationRules.Clear();
          break;
      }
    }
    else
      this._rowValidationBindingGroup = (BindingGroup) null;
  }

  public StyleSelector RowStyleSelector
  {
    get => (StyleSelector) this.GetValue(DataGrid.RowStyleSelectorProperty);
    set => this.SetValue(DataGrid.RowStyleSelectorProperty, (object) value);
  }

  private static void OnRowStyleSelectorChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    d.CoerceValue(ItemsControl.ItemContainerStyleSelectorProperty);
  }

  private static object OnCoerceItemContainerStyleSelector(DependencyObject d, object baseValue)
  {
    return !DataGridHelper.IsDefaultValue(d, DataGrid.RowStyleSelectorProperty) ? d.GetValue(DataGrid.RowStyleSelectorProperty) : baseValue;
  }

  private static object OnCoerceIsSynchronizedWithCurrentItem(DependencyObject d, object baseValue)
  {
    return ((DataGrid) d).SelectionUnit == DataGridSelectionUnit.Cell ? (object) false : baseValue;
  }

  public Brush RowBackground
  {
    get => (Brush) this.GetValue(DataGrid.RowBackgroundProperty);
    set => this.SetValue(DataGrid.RowBackgroundProperty, (object) value);
  }

  public Brush AlternatingRowBackground
  {
    get => (Brush) this.GetValue(DataGrid.AlternatingRowBackgroundProperty);
    set => this.SetValue(DataGrid.AlternatingRowBackgroundProperty, (object) value);
  }

  private static object OnCoerceAlternationCount(DependencyObject d, object baseValue)
  {
    return (int) baseValue < 2 && ((DataGrid) d).AlternatingRowBackground != null ? (object) 2 : baseValue;
  }

  public double RowHeight
  {
    get => (double) this.GetValue(DataGrid.RowHeightProperty);
    set => this.SetValue(DataGrid.RowHeightProperty, (object) value);
  }

  public double MinRowHeight
  {
    get => (double) this.GetValue(DataGrid.MinRowHeightProperty);
    set => this.SetValue(DataGrid.MinRowHeightProperty, (object) value);
  }

  internal Visibility PlaceholderVisibility => this._placeholderVisibility;

  public event EventHandler<DataGridRowEventArgs> LoadingRow;

  public event EventHandler<DataGridRowEventArgs> UnloadingRow;

  protected virtual void OnLoadingRow(DataGridRowEventArgs e)
  {
    if (this.LoadingRow != null)
      this.LoadingRow((object) this, e);
    DataGridRow row = e.Row;
    if (row.DetailsVisibility != Visibility.Visible || row.DetailsPresenter == null)
      return;
    Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(DataGrid.DelayedOnLoadingRowDetails), DispatcherPriority.Loaded, (object) row);
  }

  internal static object DelayedOnLoadingRowDetails(object arg)
  {
    DataGridRow row = (DataGridRow) arg;
    row.DataGridOwner?.OnLoadingRowDetailsWrapper(row);
    return (object) null;
  }

  protected virtual void OnUnloadingRow(DataGridRowEventArgs e)
  {
    if (this.UnloadingRow != null)
      this.UnloadingRow((object) this, e);
    this.OnUnloadingRowDetailsWrapper(e.Row);
  }

  public double RowHeaderWidth
  {
    get => (double) this.GetValue(DataGrid.RowHeaderWidthProperty);
    set => this.SetValue(DataGrid.RowHeaderWidthProperty, (object) value);
  }

  public double RowHeaderActualWidth
  {
    get => (double) this.GetValue(DataGrid.RowHeaderActualWidthProperty);
    internal set => this.SetValue(DataGrid.RowHeaderActualWidthPropertyKey, (object) value);
  }

  public double ColumnHeaderHeight
  {
    get => (double) this.GetValue(DataGrid.ColumnHeaderHeightProperty);
    set => this.SetValue(DataGrid.ColumnHeaderHeightProperty, (object) value);
  }

  public DataGridHeadersVisibility HeadersVisibility
  {
    get => (DataGridHeadersVisibility) this.GetValue(DataGrid.HeadersVisibilityProperty);
    set => this.SetValue(DataGrid.HeadersVisibilityProperty, (object) value);
  }

  private static void OnNotifyRowHeaderWidthPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGrid dataGrid = (DataGrid) d;
    double newValue = (double) e.NewValue;
    dataGrid.RowHeaderActualWidth = DoubleUtil.IsNaN(newValue) ? 0.0 : newValue;
    DataGrid.OnNotifyRowHeaderPropertyChanged(d, e);
  }

  private void ResetRowHeaderActualWidth()
  {
    if (!DoubleUtil.IsNaN(this.RowHeaderWidth))
      return;
    this.RowHeaderActualWidth = 0.0;
  }

  public void SetDetailsVisibilityForItem(object item, Visibility detailsVisibility)
  {
    this._itemAttachedStorage.SetValue(item, DataGridRow.DetailsVisibilityProperty, (object) detailsVisibility);
    DataGridRow dataGridRow = (DataGridRow) this.ItemContainerGenerator.ContainerFromItem(item);
    if (dataGridRow == null)
      return;
    dataGridRow.DetailsVisibility = detailsVisibility;
  }

  public Visibility GetDetailsVisibilityForItem(object item)
  {
    object visibilityForItem;
    if (this._itemAttachedStorage.TryGetValue(item, DataGridRow.DetailsVisibilityProperty, out visibilityForItem))
      return (Visibility) visibilityForItem;
    DataGridRow dataGridRow = (DataGridRow) this.ItemContainerGenerator.ContainerFromItem(item);
    if (dataGridRow != null)
      return dataGridRow.DetailsVisibility;
    switch (this.RowDetailsVisibilityMode)
    {
      case DataGridRowDetailsVisibilityMode.Visible:
        return Visibility.Visible;
      case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
        return !this.SelectedItems.Contains(item) ? Visibility.Collapsed : Visibility.Visible;
      default:
        return Visibility.Collapsed;
    }
  }

  public void ClearDetailsVisibilityForItem(object item)
  {
    this._itemAttachedStorage.ClearValue(item, DataGridRow.DetailsVisibilityProperty);
    this.ItemContainerGenerator.ContainerFromItem(item)?.ClearValue(DataGridRow.DetailsVisibilityProperty);
  }

  internal DataGridItemAttachedStorage ItemAttachedStorage => this._itemAttachedStorage;

  private bool ShouldSelectRowHeader
  {
    get
    {
      return this._selectionAnchor.HasValue && this.SelectedItems.Contains(this._selectionAnchor.Value.Item) && this.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
    }
  }

  public Style CellStyle
  {
    get => (Style) this.GetValue(DataGrid.CellStyleProperty);
    set => this.SetValue(DataGrid.CellStyleProperty, (object) value);
  }

  public Style ColumnHeaderStyle
  {
    get => (Style) this.GetValue(DataGrid.ColumnHeaderStyleProperty);
    set => this.SetValue(DataGrid.ColumnHeaderStyleProperty, (object) value);
  }

  public Style RowHeaderStyle
  {
    get => (Style) this.GetValue(DataGrid.RowHeaderStyleProperty);
    set => this.SetValue(DataGrid.RowHeaderStyleProperty, (object) value);
  }

  public DataTemplate RowHeaderTemplate
  {
    get => (DataTemplate) this.GetValue(DataGrid.RowHeaderTemplateProperty);
    set => this.SetValue(DataGrid.RowHeaderTemplateProperty, (object) value);
  }

  public DataTemplateSelector RowHeaderTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(DataGrid.RowHeaderTemplateSelectorProperty);
    set => this.SetValue(DataGrid.RowHeaderTemplateSelectorProperty, (object) value);
  }

  public static ComponentResourceKey FocusBorderBrushKey
  {
    get
    {
      if (DataGrid._focusBorderBrushKey == null)
        DataGrid._focusBorderBrushKey = new ComponentResourceKey(typeof (DataGrid), (object) nameof (FocusBorderBrushKey));
      return DataGrid._focusBorderBrushKey;
    }
  }

  public static IValueConverter HeadersVisibilityConverter
  {
    get
    {
      if (DataGrid._headersVisibilityConverter == null)
        DataGrid._headersVisibilityConverter = (IValueConverter) new DataGridHeadersVisibilityToVisibilityConverter();
      return DataGrid._headersVisibilityConverter;
    }
  }

  public static IValueConverter RowDetailsScrollingConverter
  {
    get
    {
      if (DataGrid._rowDetailsScrollingConverter == null)
        DataGrid._rowDetailsScrollingConverter = (IValueConverter) new BooleanToSelectiveScrollingOrientationConverter();
      return DataGrid._rowDetailsScrollingConverter;
    }
  }

  public ScrollBarVisibility HorizontalScrollBarVisibility
  {
    get => (ScrollBarVisibility) this.GetValue(DataGrid.HorizontalScrollBarVisibilityProperty);
    set => this.SetValue(DataGrid.HorizontalScrollBarVisibilityProperty, (object) value);
  }

  public ScrollBarVisibility VerticalScrollBarVisibility
  {
    get => (ScrollBarVisibility) this.GetValue(DataGrid.VerticalScrollBarVisibilityProperty);
    set => this.SetValue(DataGrid.VerticalScrollBarVisibilityProperty, (object) value);
  }

  public void ScrollIntoView(object item)
  {
    if (item == null)
      throw new ArgumentNullException(nameof (item));
    if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
      this.ScrollRowIntoView(item);
    else
      this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Delegate) new DispatcherOperationCallback(this.OnScrollIntoView), item);
  }

  public void ScrollIntoView(object item, DataGridColumn column)
  {
    if (column == null)
    {
      this.ScrollIntoView(item);
    }
    else
    {
      if (!column.IsVisible)
        return;
      if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
      {
        if (item == null)
          this.ScrollColumnIntoView(column);
        else
          this.ScrollCellIntoView(item, column);
      }
      else
        this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Delegate) new DispatcherOperationCallback(this.OnScrollIntoView), (object) new object[2]
        {
          item,
          (object) column
        });
    }
  }

  private object OnScrollIntoView(object arg)
  {
    if (arg is object[] objArray)
    {
      if (objArray[0] != null)
        this.ScrollCellIntoView(objArray[0], (DataGridColumn) objArray[1]);
      else
        this.ScrollColumnIntoView((DataGridColumn) objArray[1]);
    }
    else
      this.ScrollRowIntoView(arg);
    return (object) null;
  }

  private void ScrollColumnIntoView(DataGridColumn column)
  {
    if (this._rowTrackingRoot == null)
      return;
    DataGridRow container = this._rowTrackingRoot.Container;
    if (container == null)
      return;
    int index = this._columns.IndexOf(column);
    container.ScrollCellIntoView(index);
  }

  private void ScrollRowIntoView(object item)
  {
    if (this.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement frameworkElement)
    {
      frameworkElement.BringIntoView();
    }
    else
    {
      if (this.IsGrouping)
        return;
      int index = this.Items.IndexOf(item);
      if (index < 0 || !(this.InternalItemsHost is Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter internalItemsHost))
        return;
      internalItemsHost.InternalBringIndexIntoView(index);
    }
  }

  private void ScrollCellIntoView(object item, DataGridColumn column)
  {
    if (!column.IsVisible)
      return;
    if (!(this.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow dataGridRow))
    {
      this.ScrollRowIntoView(item);
      this.UpdateLayout();
      dataGridRow = this.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
    }
    else
      dataGridRow.BringIntoView();
    if (dataGridRow == null)
      return;
    int index = this._columns.IndexOf(column);
    dataGridRow.ScrollCellIntoView(index);
  }

  protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e)
  {
    if (!this.IsMouseCaptured)
      this.StopAutoScroll();
    base.OnIsMouseCapturedChanged(e);
  }

  private new static TimeSpan AutoScrollTimeout
  {
    get => TimeSpan.FromMilliseconds((double) NativeMethods.GetDoubleClickTime() * 0.8);
  }

  private void StartAutoScroll()
  {
    if (this._autoScrollTimer != null)
      return;
    this._hasAutoScrolled = false;
    this._autoScrollTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
    this._autoScrollTimer.Interval = DataGrid.AutoScrollTimeout;
    this._autoScrollTimer.Tick += new EventHandler(this.OnAutoScrollTimeout);
    this._autoScrollTimer.Start();
  }

  private void StopAutoScroll()
  {
    if (this._autoScrollTimer == null)
      return;
    this._autoScrollTimer.Stop();
    this._autoScrollTimer = (DispatcherTimer) null;
    this._hasAutoScrolled = false;
  }

  private void OnAutoScrollTimeout(object sender, EventArgs e)
  {
    if (Mouse.LeftButton == MouseButtonState.Pressed)
      this.DoAutoScroll();
    else
      this.StopAutoScroll();
  }

  private bool DoAutoScroll()
  {
    DataGrid.RelativeMousePositions relativeMousePosition = this.RelativeMousePosition;
    if (relativeMousePosition != DataGrid.RelativeMousePositions.Over)
    {
      DataGridCell cellNearMouse = this.GetCellNearMouse();
      if (cellNearMouse != null)
      {
        DataGridColumn column = cellNearMouse.Column;
        object rowDataItem = cellNearMouse.RowDataItem;
        if (DataGrid.IsMouseToLeft(relativeMousePosition))
        {
          int displayIndex = column.DisplayIndex;
          if (displayIndex > 0)
            column = this.ColumnFromDisplayIndex(displayIndex - 1);
        }
        else if (DataGrid.IsMouseToRight(relativeMousePosition))
        {
          int displayIndex = column.DisplayIndex;
          if (displayIndex < this._columns.Count - 1)
            column = this.ColumnFromDisplayIndex(displayIndex + 1);
        }
        if (DataGrid.IsMouseAbove(relativeMousePosition))
        {
          int num = this.Items.IndexOf(rowDataItem);
          if (num > 0)
            rowDataItem = this.Items[num - 1];
        }
        else if (DataGrid.IsMouseBelow(relativeMousePosition))
        {
          int num = this.Items.IndexOf(rowDataItem);
          if (num < this.Items.Count - 1)
            rowDataItem = this.Items[num + 1];
        }
        if (this._isRowDragging)
        {
          this.ScrollRowIntoView(rowDataItem);
          DataGridRow row = (DataGridRow) this.ItemContainerGenerator.ContainerFromItem(rowDataItem);
          if (row != null)
          {
            this._hasAutoScrolled = true;
            this.HandleSelectionForRowHeaderAndDetailsInput(row, false);
            this.CurrentItem = rowDataItem;
            return true;
          }
        }
        else
        {
          this.ScrollCellIntoView(rowDataItem, column);
          DataGridCell cell = this.TryFindCell(rowDataItem, column);
          if (cell != null)
          {
            this._hasAutoScrolled = true;
            this.HandleSelectionForCellInput(cell, false, true, true);
            cell.Focus();
            return true;
          }
        }
      }
    }
    return false;
  }

  protected override bool HandlesScrolling => true;

  internal Panel InternalItemsHost
  {
    get => this._internalItemsHost;
    set
    {
      if (this._internalItemsHost == value)
        return;
      this._internalItemsHost = value;
      if (this._internalItemsHost == null)
        return;
      this.EnsureInternalScrollControls();
    }
  }

  internal ScrollViewer InternalScrollHost
  {
    get
    {
      this.EnsureInternalScrollControls();
      return this._internalScrollHost;
    }
  }

  internal ScrollContentPresenter InternalScrollContentPresenter
  {
    get
    {
      this.EnsureInternalScrollControls();
      return this._internalScrollContentPresenter;
    }
  }

  private void EnsureInternalScrollControls()
  {
    if (this._internalScrollContentPresenter == null)
    {
      if (this._internalItemsHost != null)
        this._internalScrollContentPresenter = DataGridHelper.FindVisualParent<ScrollContentPresenter>((UIElement) this._internalItemsHost);
      else if (this._rowTrackingRoot != null)
        this._internalScrollContentPresenter = DataGridHelper.FindVisualParent<ScrollContentPresenter>((UIElement) this._rowTrackingRoot.Container);
      if (this._internalScrollContentPresenter != null)
        this._internalScrollContentPresenter.SizeChanged += new SizeChangedEventHandler(this.OnInternalScrollContentPresenterSizeChanged);
    }
    if (this._internalScrollHost != null)
      return;
    if (this._internalItemsHost != null)
      this._internalScrollHost = DataGridHelper.FindVisualParent<ScrollViewer>((UIElement) this._internalItemsHost);
    else if (this._rowTrackingRoot != null)
      this._internalScrollHost = DataGridHelper.FindVisualParent<ScrollViewer>((UIElement) this._rowTrackingRoot.Container);
    if (this._internalScrollHost == null)
      return;
    this.SetBinding(DataGrid.HorizontalScrollOffsetProperty, (BindingBase) new Binding("ContentHorizontalOffset")
    {
      Source = (object) this._internalScrollHost
    });
  }

  private void CleanUpInternalScrollControls()
  {
    BindingOperations.ClearBinding((DependencyObject) this, DataGrid.HorizontalScrollOffsetProperty);
    this._internalScrollHost = (ScrollViewer) null;
    if (this._internalScrollContentPresenter == null)
      return;
    this._internalScrollContentPresenter.SizeChanged -= new SizeChangedEventHandler(this.OnInternalScrollContentPresenterSizeChanged);
    this._internalScrollContentPresenter = (ScrollContentPresenter) null;
  }

  private void OnInternalScrollContentPresenterSizeChanged(object sender, SizeChangedEventArgs e)
  {
    if (this._internalScrollContentPresenter == null || this._internalScrollContentPresenter.CanContentScroll)
      return;
    this.OnViewportSizeChanged(e.PreviousSize, e.NewSize);
  }

  internal void OnViewportSizeChanged(Size oldSize, Size newSize)
  {
    if (this.InternalColumns.ColumnWidthsComputationPending || DoubleUtil.AreClose(newSize.Width - oldSize.Width, 0.0))
      return;
    this._finalViewportWidth = newSize.Width;
    if (this._viewportWidthChangeNotificationPending)
      return;
    this._originalViewportWidth = oldSize.Width;
    this.Dispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(this.OnDelayedViewportWidthChanged), DispatcherPriority.Loaded, (object) this);
    this._viewportWidthChangeNotificationPending = true;
  }

  private object OnDelayedViewportWidthChanged(object args)
  {
    if (!this._viewportWidthChangeNotificationPending)
      return (object) null;
    double availableSpaceChange = this._finalViewportWidth - this._originalViewportWidth;
    if (!DoubleUtil.AreClose(availableSpaceChange, 0.0))
    {
      this.NotifyPropertyChanged((DependencyObject) this, "ViewportWidth", new DependencyPropertyChangedEventArgs(), NotificationTarget.CellsPresenter | NotificationTarget.ColumnCollection | NotificationTarget.ColumnHeadersPresenter);
      double newTotalAvailableSpace = this._finalViewportWidth - this.CellsPanelHorizontalOffset;
      this.InternalColumns.RedistributeColumnWidthsOnAvailableSpaceChange(availableSpaceChange, newTotalAvailableSpace);
    }
    this._viewportWidthChangeNotificationPending = false;
    return (object) null;
  }

  internal double HorizontalScrollOffset
  {
    get => (double) this.GetValue(DataGrid.HorizontalScrollOffsetProperty);
  }

  public static RoutedUICommand DeleteCommand => ApplicationCommands.Delete;

  private static void OnCanExecuteBeginEdit(object sender, CanExecuteRoutedEventArgs e)
  {
    ((DataGrid) sender).OnCanExecuteBeginEdit(e);
  }

  private static void OnExecutedBeginEdit(object sender, ExecutedRoutedEventArgs e)
  {
    ((DataGrid) sender).OnExecutedBeginEdit(e);
  }

  protected virtual void OnCanExecuteBeginEdit(CanExecuteRoutedEventArgs e)
  {
    bool flag = !this.IsReadOnly && this.CurrentCellContainer != null && !this.IsEditingCurrentCell && !this.IsCurrentCellReadOnly && !this.HasCellValidationError;
    if (flag && this.HasRowValidationError)
    {
      DataGridCell cellOrCurrentCell = this.GetEventCellOrCurrentCell((RoutedEventArgs) e);
      flag = cellOrCurrentCell != null && this.IsAddingOrEditingRowItem(cellOrCurrentCell.RowDataItem);
    }
    if (flag)
    {
      e.CanExecute = true;
      e.Handled = true;
    }
    else
      e.ContinueRouting = true;
  }

  protected virtual void OnExecutedBeginEdit(ExecutedRoutedEventArgs e)
  {
    DataGridCell currentCellContainer = this.CurrentCellContainer;
    if (currentCellContainer != null && !currentCellContainer.IsReadOnly && !currentCellContainer.IsEditing)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      List<int> columnIndexRanges = (List<int>) null;
      int rowIndex1 = -1;
      object obj = (object) null;
      bool flag4 = this.EditableItems.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning;
      if (this.IsNewItemPlaceholder(currentCellContainer.RowDataItem))
      {
        if (this.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
        {
          this.UnselectItem(CollectionView.NewItemPlaceholder);
          flag2 = true;
        }
        else
        {
          rowIndex1 = this.Items.IndexOf(currentCellContainer.RowDataItem);
          flag3 = rowIndex1 >= 0 && this._selectedCells.Intersects(rowIndex1, out columnIndexRanges);
        }
        obj = this.AddNewItem();
        this.CurrentItem = obj;
        currentCellContainer = this.CurrentCellContainer;
        if (this.CurrentCellContainer == null)
        {
          this.UpdateLayout();
          currentCellContainer = this.CurrentCellContainer;
          if (currentCellContainer != null && !currentCellContainer.IsKeyboardFocusWithin)
            currentCellContainer.Focus();
        }
        if (flag2)
          this.SelectItem(obj);
        else if (flag3)
        {
          using (this.UpdateSelectedCells())
          {
            int rowIndex2 = rowIndex1;
            if (flag4)
            {
              this._selectedCells.RemoveRegion(rowIndex1, 0, 1, this.Columns.Count);
              ++rowIndex2;
            }
            int index = 0;
            for (int count = columnIndexRanges.Count; index < count; index += 2)
              this._selectedCells.AddRegion(rowIndex2, columnIndexRanges[index], 1, columnIndexRanges[index + 1]);
          }
        }
        flag1 = true;
      }
      RoutedEventArgs parameter = e.Parameter as RoutedEventArgs;
      DataGridBeginningEditEventArgs e1 = (DataGridBeginningEditEventArgs) null;
      if (currentCellContainer != null)
      {
        e1 = new DataGridBeginningEditEventArgs(currentCellContainer.Column, currentCellContainer.RowOwner, parameter);
        this.OnBeginningEdit(e1);
      }
      if (currentCellContainer == null || e1.Cancel)
      {
        if (flag2)
          this.UnselectItem(obj);
        else if (flag3 && flag4)
          this._selectedCells.RemoveRegion(rowIndex1 + 1, 0, 1, this.Columns.Count);
        if (flag1)
        {
          this.CancelRowItem();
          this.UpdateNewItemPlaceholder(false);
          this.SetCurrentItemToPlaceholder();
        }
        if (flag2)
          this.SelectItem(CollectionView.NewItemPlaceholder);
        else if (flag3)
        {
          int index = 0;
          for (int count = columnIndexRanges.Count; index < count; index += 2)
            this._selectedCells.AddRegion(rowIndex1, columnIndexRanges[index], 1, columnIndexRanges[index + 1]);
        }
      }
      else
      {
        if (!flag1 && !this.IsEditingRowItem)
        {
          this.EditRowItem(currentCellContainer.RowDataItem);
          currentCellContainer.RowOwner.BindingGroup?.BeginEdit();
          this._editingRowItem = currentCellContainer.RowDataItem;
          this._editingRowIndex = this.Items.IndexOf(this._editingRowItem);
        }
        currentCellContainer.BeginEdit(parameter);
        currentCellContainer.RowOwner.IsEditing = true;
      }
    }
    CommandManager.InvalidateRequerySuggested();
    e.Handled = true;
  }

  private static void OnCanExecuteCommitEdit(object sender, CanExecuteRoutedEventArgs e)
  {
    ((DataGrid) sender).OnCanExecuteCommitEdit(e);
  }

  private static void OnExecutedCommitEdit(object sender, ExecutedRoutedEventArgs e)
  {
    ((DataGrid) sender).OnExecutedCommitEdit(e);
  }

  private DataGridCell GetEventCellOrCurrentCell(RoutedEventArgs e)
  {
    UIElement originalSource = e.OriginalSource as UIElement;
    return originalSource != this && originalSource != null ? DataGridHelper.FindVisualParent<DataGridCell>(originalSource) : this.CurrentCellContainer;
  }

  private bool CanEndEdit(CanExecuteRoutedEventArgs e, bool commit)
  {
    DataGridCell cellOrCurrentCell = this.GetEventCellOrCurrentCell((RoutedEventArgs) e);
    if (cellOrCurrentCell == null)
      return false;
    DataGridEditingUnit editingUnit = this.GetEditingUnit(e.Parameter);
    IEditableCollectionView editableItems = this.EditableItems;
    object rowDataItem = cellOrCurrentCell.RowDataItem;
    if (cellOrCurrentCell.IsEditing)
      return true;
    if (editingUnit != DataGridEditingUnit.Row || this.HasCellValidationError)
      return false;
    if (editableItems.IsAddingNew && editableItems.CurrentAddItem == rowDataItem)
      return true;
    return editableItems.IsEditingItem && (commit || editableItems.CanCancelEdit || this.HasRowValidationError) && editableItems.CurrentEditItem == rowDataItem;
  }

  protected virtual void OnCanExecuteCommitEdit(CanExecuteRoutedEventArgs e)
  {
    if (this.CanEndEdit(e, true))
    {
      e.CanExecute = true;
      e.Handled = true;
    }
    else
      e.ContinueRouting = true;
  }

  protected virtual void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
  {
    DataGridCell currentCellContainer = this.CurrentCellContainer;
    bool flag1 = true;
    if (currentCellContainer != null)
    {
      DataGridEditingUnit editingUnit = this.GetEditingUnit(e.Parameter);
      bool flag2 = false;
      if (currentCellContainer.IsEditing)
      {
        DataGridCellEditEndingEventArgs e1 = new DataGridCellEditEndingEventArgs(currentCellContainer.Column, currentCellContainer.RowOwner, currentCellContainer.EditingElement, DataGridEditAction.Commit);
        this.OnCellEditEnding(e1);
        flag2 = e1.Cancel;
        if (!flag2)
        {
          flag1 = currentCellContainer.CommitEdit();
          this.HasCellValidationError = !flag1;
        }
      }
      if (flag1 && !flag2 && (editingUnit == DataGridEditingUnit.Row && this.IsAddingOrEditingRowItem(currentCellContainer.RowDataItem) || !this.EditableItems.CanCancelEdit && this.IsEditingItem(currentCellContainer.RowDataItem)))
      {
        DataGridRowEditEndingEventArgs e2 = new DataGridRowEditEndingEventArgs(currentCellContainer.RowOwner, DataGridEditAction.Commit);
        this.OnRowEditEnding(e2);
        if (!e2.Cancel)
        {
          BindingGroup bindingGroup = currentCellContainer.RowOwner.BindingGroup;
          if (bindingGroup != null)
          {
            this.Dispatcher.Invoke((Delegate) new DispatcherOperationCallback(DataGrid.DoNothing), DispatcherPriority.DataBind, (object) bindingGroup);
            flag1 = bindingGroup.CommitEdit();
          }
          this.HasRowValidationError = !flag1;
          if (flag1)
            this.CommitRowItem();
        }
      }
      if (flag1)
        this.UpdateRowEditing(currentCellContainer);
      CommandManager.InvalidateRequerySuggested();
    }
    e.Handled = true;
  }

  private static object DoNothing(object arg) => (object) null;

  private DataGridEditingUnit GetEditingUnit(object parameter)
  {
    if (parameter != null && parameter is DataGridEditingUnit editingUnit)
      return editingUnit;
    return !this.IsEditingCurrentCell ? DataGridEditingUnit.Row : DataGridEditingUnit.Cell;
  }

  public event EventHandler<DataGridRowEditEndingEventArgs> RowEditEnding;

  protected virtual void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
  {
    if (this.RowEditEnding != null)
      this.RowEditEnding((object) this, e);
    if (!AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked) || !(UIElementAutomationPeer.FromElement((UIElement) this) is Microsoft.Windows.Automation.Peers.DataGridAutomationPeer gridAutomationPeer))
      return;
    gridAutomationPeer.RaiseAutomationRowInvokeEvents(e.Row);
  }

  public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;

  protected virtual void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
  {
    if (this.CellEditEnding != null)
      this.CellEditEnding((object) this, e);
    if (!AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked) || !(UIElementAutomationPeer.FromElement((UIElement) this) is Microsoft.Windows.Automation.Peers.DataGridAutomationPeer gridAutomationPeer))
      return;
    gridAutomationPeer.RaiseAutomationCellInvokeEvents(e.Column, e.Row);
  }

  private static void OnCanExecuteCancelEdit(object sender, CanExecuteRoutedEventArgs e)
  {
    ((DataGrid) sender).OnCanExecuteCancelEdit(e);
  }

  private static void OnExecutedCancelEdit(object sender, ExecutedRoutedEventArgs e)
  {
    ((DataGrid) sender).OnExecutedCancelEdit(e);
  }

  protected virtual void OnCanExecuteCancelEdit(CanExecuteRoutedEventArgs e)
  {
    if (this.CanEndEdit(e, false))
    {
      e.CanExecute = true;
      e.Handled = true;
    }
    else
      e.ContinueRouting = true;
  }

  protected virtual void OnExecutedCancelEdit(ExecutedRoutedEventArgs e)
  {
    DataGridCell currentCellContainer = this.CurrentCellContainer;
    if (currentCellContainer != null)
    {
      DataGridEditingUnit editingUnit = this.GetEditingUnit(e.Parameter);
      bool flag1 = false;
      if (currentCellContainer.IsEditing)
      {
        DataGridCellEditEndingEventArgs e1 = new DataGridCellEditEndingEventArgs(currentCellContainer.Column, currentCellContainer.RowOwner, currentCellContainer.EditingElement, DataGridEditAction.Cancel);
        this.OnCellEditEnding(e1);
        flag1 = e1.Cancel;
        if (!flag1)
        {
          currentCellContainer.CancelEdit();
          this.HasCellValidationError = false;
        }
      }
      IEditableCollectionView editableItems = this.EditableItems;
      bool flag2 = this.IsEditingItem(currentCellContainer.RowDataItem) && !editableItems.CanCancelEdit;
      if (!flag1 && (this.CanCancelAddingOrEditingRowItem(editingUnit, currentCellContainer.RowDataItem) || flag2))
      {
        bool flag3 = true;
        if (!flag2)
        {
          DataGridRowEditEndingEventArgs e2 = new DataGridRowEditEndingEventArgs(currentCellContainer.RowOwner, DataGridEditAction.Cancel);
          this.OnRowEditEnding(e2);
          flag3 = !e2.Cancel;
        }
        if (flag3)
        {
          if (flag2)
            editableItems.CommitEdit();
          else
            this.CancelRowItem();
          BindingGroup bindingGroup = currentCellContainer.RowOwner.BindingGroup;
          if (bindingGroup != null)
          {
            bindingGroup.CancelEdit();
            bindingGroup.UpdateSources();
          }
        }
      }
      this.UpdateRowEditing(currentCellContainer);
      if (!currentCellContainer.RowOwner.IsEditing)
        this.HasRowValidationError = false;
      CommandManager.InvalidateRequerySuggested();
    }
    e.Handled = true;
  }

  private static void OnCanExecuteDelete(object sender, CanExecuteRoutedEventArgs e)
  {
    ((DataGrid) sender).OnCanExecuteDelete(e);
  }

  private static void OnExecutedDelete(object sender, ExecutedRoutedEventArgs e)
  {
    ((DataGrid) sender).OnExecutedDelete(e);
  }

  protected virtual void OnCanExecuteDelete(CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = this.CanUserDeleteRows && this.DataItemsSelected > 0 && (this._currentCellContainer == null || !this._currentCellContainer.IsEditing);
    e.Handled = true;
  }

  protected virtual void OnExecutedDelete(ExecutedRoutedEventArgs e)
  {
    if (this.DataItemsSelected > 0)
    {
      bool flag = false;
      bool isEditingRowItem = this.IsEditingRowItem;
      if (isEditingRowItem || this.IsAddingNewItem)
      {
        if (this.CancelEdit(DataGridEditingUnit.Row) && isEditingRowItem)
          flag = true;
      }
      else
        flag = true;
      if (flag)
      {
        int count = this.SelectedItems.Count;
        int index1 = -1;
        object currentItem = this.CurrentItem;
        if (this.SelectedItems.Contains(currentItem))
        {
          int val2 = this.Items.IndexOf(currentItem);
          if (this._selectionAnchor.HasValue)
          {
            int num = this.Items.IndexOf(this._selectionAnchor.Value.Item);
            if (num >= 0 && num < val2)
              val2 = num;
          }
          index1 = Math.Min(this.Items.Count - count - 1, val2);
        }
        ArrayList arrayList = new ArrayList((ICollection) this.SelectedItems);
        using (this.UpdateSelectedCells())
        {
          bool updatingSelectedItems = this.IsUpdatingSelectedItems;
          if (!updatingSelectedItems)
            this.BeginUpdateSelectedItems();
          try
          {
            this._selectedCells.ClearFullRows(this.SelectedItems);
            this.SelectedItems.Clear();
          }
          finally
          {
            if (!updatingSelectedItems)
              this.EndUpdateSelectedItems();
          }
        }
        for (int index2 = 0; index2 < count; ++index2)
        {
          object obj = arrayList[index2];
          if (obj != CollectionView.NewItemPlaceholder)
            this.EditableItems.Remove(obj);
        }
        if (index1 >= 0)
        {
          this.CurrentItem = this.Items[index1];
          DataGridCell currentCellContainer = this.CurrentCellContainer;
          if (currentCellContainer != null)
          {
            this._selectionAnchor = new DataGridCellInfo?();
            this.HandleSelectionForCellInput(currentCellContainer, false, false, false);
          }
        }
      }
    }
    e.Handled = true;
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(DataGrid.IsReadOnlyProperty);
    set => this.SetValue(DataGrid.IsReadOnlyProperty, (object) value);
  }

  private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if ((bool) e.NewValue)
      ((DataGrid) d).CancelAnyEdit();
    CommandManager.InvalidateRequerySuggested();
    d.CoerceValue(DataGrid.CanUserAddRowsProperty);
    d.CoerceValue(DataGrid.CanUserDeleteRowsProperty);
    DataGrid.OnNotifyColumnAndCellPropertyChanged(d, e);
  }

  public object CurrentItem
  {
    get => this.GetValue(DataGrid.CurrentItemProperty);
    set => this.SetValue(DataGrid.CurrentItemProperty, value);
  }

  private static void OnCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    DataGrid owner = (DataGrid) d;
    DataGridCellInfo currentCell = owner.CurrentCell;
    object newValue = e.NewValue;
    if (currentCell.Item == newValue)
      return;
    owner.CurrentCell = DataGridCellInfo.CreatePossiblyPartialCellInfo(newValue, currentCell.Column, owner);
  }

  public DataGridColumn CurrentColumn
  {
    get => (DataGridColumn) this.GetValue(DataGrid.CurrentColumnProperty);
    set => this.SetValue(DataGrid.CurrentColumnProperty, (object) value);
  }

  private static void OnCurrentColumnChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGrid owner = (DataGrid) d;
    DataGridCellInfo currentCell = owner.CurrentCell;
    DataGridColumn newValue = (DataGridColumn) e.NewValue;
    if (currentCell.Column == newValue)
      return;
    owner.CurrentCell = DataGridCellInfo.CreatePossiblyPartialCellInfo(currentCell.Item, newValue, owner);
  }

  public DataGridCellInfo CurrentCell
  {
    get => (DataGridCellInfo) this.GetValue(DataGrid.CurrentCellProperty);
    set => this.SetValue(DataGrid.CurrentCellProperty, (object) value);
  }

  private static void OnCurrentCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    DataGrid dataGrid = (DataGrid) d;
    DataGridCellInfo oldValue = (DataGridCellInfo) e.OldValue;
    DataGridCellInfo newValue = (DataGridCellInfo) e.NewValue;
    if (dataGrid.CurrentItem != newValue.Item)
      dataGrid.CurrentItem = newValue.Item;
    if (dataGrid.CurrentColumn != newValue.Column)
      dataGrid.CurrentColumn = newValue.Column;
    if (dataGrid._currentCellContainer != null)
    {
      if ((dataGrid.IsAddingNewItem || dataGrid.IsEditingRowItem) && oldValue.Item != newValue.Item)
        dataGrid.EndEdit(DataGrid.CommitEditCommand, dataGrid._currentCellContainer, DataGridEditingUnit.Row, true);
      else if (dataGrid._currentCellContainer.IsEditing)
        dataGrid.EndEdit(DataGrid.CommitEditCommand, dataGrid._currentCellContainer, DataGridEditingUnit.Cell, true);
    }
    dataGrid._currentCellContainer = (DataGridCell) null;
    if (newValue.IsValid && dataGrid.IsKeyboardFocusWithin)
    {
      DataGridCell currentCellContainer = dataGrid._pendingCurrentCellContainer;
      if (currentCellContainer == null)
      {
        currentCellContainer = dataGrid.CurrentCellContainer;
        if (currentCellContainer == null)
        {
          dataGrid.ScrollCellIntoView(newValue.Item, newValue.Column);
          currentCellContainer = dataGrid.CurrentCellContainer;
        }
      }
      if (currentCellContainer != null && !currentCellContainer.IsKeyboardFocusWithin)
        currentCellContainer.Focus();
    }
    dataGrid.OnCurrentCellChanged(EventArgs.Empty);
  }

  public event EventHandler<EventArgs> CurrentCellChanged;

  protected virtual void OnCurrentCellChanged(EventArgs e)
  {
    if (this.CurrentCellChanged == null)
      return;
    this.CurrentCellChanged((object) this, e);
  }

  private void UpdateCurrentCell(DataGridCell cell, bool isFocusWithinCell)
  {
    if (isFocusWithinCell)
    {
      this.CurrentCellContainer = cell;
    }
    else
    {
      if (this.IsKeyboardFocusWithin)
        return;
      this.CurrentCellContainer = (DataGridCell) null;
    }
  }

  private DataGridCell CurrentCellContainer
  {
    get
    {
      if (this._currentCellContainer == null)
      {
        DataGridCellInfo currentCell = this.CurrentCell;
        if (currentCell.IsValid)
          this._currentCellContainer = this.TryFindCell(currentCell);
      }
      return this._currentCellContainer;
    }
    set
    {
      if (this._currentCellContainer == value || value != null && value == this._pendingCurrentCellContainer)
        return;
      this._pendingCurrentCellContainer = value;
      if (value == null)
        this.ClearValue(DataGrid.CurrentCellProperty);
      else
        this.CurrentCell = new DataGridCellInfo(value);
      this._pendingCurrentCellContainer = (DataGridCell) null;
      this._currentCellContainer = value;
      CommandManager.InvalidateRequerySuggested();
    }
  }

  private bool IsEditingCurrentCell
  {
    get
    {
      DataGridCell currentCellContainer = this.CurrentCellContainer;
      return currentCellContainer != null && currentCellContainer.IsEditing;
    }
  }

  private bool IsCurrentCellReadOnly
  {
    get
    {
      DataGridCell currentCellContainer = this.CurrentCellContainer;
      return currentCellContainer != null && currentCellContainer.IsReadOnly;
    }
  }

  public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit;

  protected virtual void OnBeginningEdit(DataGridBeginningEditEventArgs e)
  {
    if (this.BeginningEdit != null)
      this.BeginningEdit((object) this, e);
    if (!AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked) || !(UIElementAutomationPeer.FromElement((UIElement) this) is Microsoft.Windows.Automation.Peers.DataGridAutomationPeer gridAutomationPeer))
      return;
    gridAutomationPeer.RaiseAutomationCellInvokeEvents(e.Column, e.Row);
  }

  public event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;

  protected internal virtual void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
  {
    if (this.PreparingCellForEdit == null)
      return;
    this.PreparingCellForEdit((object) this, e);
  }

  public bool BeginEdit() => this.BeginEdit((RoutedEventArgs) null);

  public bool BeginEdit(RoutedEventArgs editingEventArgs)
  {
    if (!this.IsReadOnly)
    {
      DataGridCell currentCellContainer = this.CurrentCellContainer;
      if (currentCellContainer != null)
      {
        if (!currentCellContainer.IsEditing && DataGrid.BeginEditCommand.CanExecute((object) editingEventArgs, (IInputElement) currentCellContainer))
          DataGrid.BeginEditCommand.Execute((object) editingEventArgs, (IInputElement) currentCellContainer);
        return currentCellContainer.IsEditing;
      }
    }
    return false;
  }

  public bool CancelEdit()
  {
    if (this.IsEditingCurrentCell)
      return this.CancelEdit(DataGridEditingUnit.Cell);
    return !this.IsEditingRowItem && !this.IsAddingNewItem || this.CancelEdit(DataGridEditingUnit.Row);
  }

  internal bool CancelEdit(DataGridCell cell)
  {
    DataGridCell currentCellContainer = this.CurrentCellContainer;
    return currentCellContainer == null || currentCellContainer != cell || !currentCellContainer.IsEditing || this.CancelEdit(DataGridEditingUnit.Cell);
  }

  public bool CancelEdit(DataGridEditingUnit editingUnit)
  {
    return this.EndEdit(DataGrid.CancelEditCommand, this.CurrentCellContainer, editingUnit, true);
  }

  private void CancelAnyEdit()
  {
    if (this.IsAddingNewItem || this.IsEditingRowItem)
    {
      this.CancelEdit(DataGridEditingUnit.Row);
    }
    else
    {
      if (!this.IsEditingCurrentCell)
        return;
      this.CancelEdit(DataGridEditingUnit.Cell);
    }
  }

  public bool CommitEdit()
  {
    if (this.IsEditingCurrentCell)
      return this.CommitEdit(DataGridEditingUnit.Cell, true);
    return !this.IsEditingRowItem && !this.IsAddingNewItem || this.CommitEdit(DataGridEditingUnit.Row, true);
  }

  public bool CommitEdit(DataGridEditingUnit editingUnit, bool exitEditingMode)
  {
    return this.EndEdit(DataGrid.CommitEditCommand, this.CurrentCellContainer, editingUnit, exitEditingMode);
  }

  private bool CommitAnyEdit()
  {
    if (this.IsAddingNewItem || this.IsEditingRowItem)
      return this.CommitEdit(DataGridEditingUnit.Row, true);
    return !this.IsEditingCurrentCell || this.CommitEdit(DataGridEditingUnit.Cell, true);
  }

  private bool EndEdit(
    RoutedCommand command,
    DataGridCell cellContainer,
    DataGridEditingUnit editingUnit,
    bool exitEditMode)
  {
    bool flag1 = true;
    bool flag2 = true;
    if (cellContainer != null)
    {
      if (command.CanExecute((object) editingUnit, (IInputElement) cellContainer))
        command.Execute((object) editingUnit, (IInputElement) cellContainer);
      flag1 = !cellContainer.IsEditing;
      flag2 = !this.IsEditingRowItem && !this.IsAddingNewItem;
    }
    if (!exitEditMode)
    {
      if (editingUnit == DataGridEditingUnit.Cell)
      {
        if (cellContainer == null)
          return false;
        if (flag1)
          return this.BeginEdit((RoutedEventArgs) null);
      }
      else
      {
        if (flag2)
        {
          object rowDataItem = cellContainer.RowDataItem;
          if (rowDataItem != null)
          {
            this.EditRowItem(rowDataItem);
            return this.IsEditingRowItem;
          }
        }
        return false;
      }
    }
    if (!flag1)
      return false;
    return editingUnit == DataGridEditingUnit.Cell || flag2;
  }

  private bool HasCellValidationError
  {
    get => this._hasCellValidationError;
    set
    {
      if (this._hasCellValidationError == value)
        return;
      this._hasCellValidationError = value;
      CommandManager.InvalidateRequerySuggested();
    }
  }

  private bool HasRowValidationError
  {
    get => this._hasRowValidationError;
    set
    {
      if (this._hasRowValidationError == value)
        return;
      this._hasRowValidationError = value;
      CommandManager.InvalidateRequerySuggested();
    }
  }

  internal DataGridCell FocusedCell
  {
    get => this._focusedCell;
    set
    {
      if (this._focusedCell == value)
        return;
      if (this._focusedCell != null)
        this.UpdateCurrentCell(this._focusedCell, false);
      this._focusedCell = value;
      if (this._focusedCell == null)
        return;
      this.UpdateCurrentCell(this._focusedCell, true);
    }
  }

  public bool CanUserAddRows
  {
    get => (bool) this.GetValue(DataGrid.CanUserAddRowsProperty);
    set => this.SetValue(DataGrid.CanUserAddRowsProperty, (object) value);
  }

  private static void OnCanUserAddRowsChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).UpdateNewItemPlaceholder(false);
  }

  private static object OnCoerceCanUserAddRows(DependencyObject d, object baseValue)
  {
    return (object) DataGrid.OnCoerceCanUserAddOrDeleteRows((DataGrid) d, (bool) baseValue, true);
  }

  private static bool OnCoerceCanUserAddOrDeleteRows(
    DataGrid dataGrid,
    bool baseValue,
    bool canUserAddRowsProperty)
  {
    return (!baseValue || !dataGrid.IsReadOnly && dataGrid.IsEnabled && (!canUserAddRowsProperty || dataGrid.EditableItems.CanAddNew) && (canUserAddRowsProperty || dataGrid.EditableItems.CanRemove)) && baseValue;
  }

  public bool CanUserDeleteRows
  {
    get => (bool) this.GetValue(DataGrid.CanUserDeleteRowsProperty);
    set => this.SetValue(DataGrid.CanUserDeleteRowsProperty, (object) value);
  }

  private static void OnCanUserDeleteRowsChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    CommandManager.InvalidateRequerySuggested();
  }

  private static object OnCoerceCanUserDeleteRows(DependencyObject d, object baseValue)
  {
    return (object) DataGrid.OnCoerceCanUserAddOrDeleteRows((DataGrid) d, (bool) baseValue, false);
  }

  public event InitializingNewItemEventHandler InitializingNewItem;

  protected virtual void OnInitializingNewItem(InitializingNewItemEventArgs e)
  {
    if (this.InitializingNewItem == null)
      return;
    this.InitializingNewItem((object) this, e);
  }

  private object AddNewItem()
  {
    this.UpdateNewItemPlaceholder(true);
    object newItem = this.EditableItems.AddNew();
    if (newItem != null)
      this.OnInitializingNewItem(new InitializingNewItemEventArgs(newItem));
    CommandManager.InvalidateRequerySuggested();
    return newItem;
  }

  private void EditRowItem(object rowItem)
  {
    this.EditableItems.EditItem(rowItem);
    CommandManager.InvalidateRequerySuggested();
  }

  private void CommitRowItem()
  {
    if (this.IsEditingRowItem)
    {
      this.EditableItems.CommitEdit();
    }
    else
    {
      this.EditableItems.CommitNew();
      this.UpdateNewItemPlaceholder(false);
    }
  }

  private void CancelRowItem()
  {
    if (this.IsEditingRowItem)
    {
      this.EditableItems.CancelEdit();
    }
    else
    {
      object currentAddItem = this.EditableItems.CurrentAddItem;
      bool flag1 = currentAddItem == this.CurrentItem;
      bool flag2 = this.SelectedItems.Contains(currentAddItem);
      bool flag3 = false;
      List<int> columnIndexRanges = (List<int>) null;
      int rowIndex1 = -1;
      if (flag2)
      {
        this.UnselectItem(currentAddItem);
      }
      else
      {
        rowIndex1 = this.Items.IndexOf(currentAddItem);
        flag3 = rowIndex1 >= 0 && this._selectedCells.Intersects(rowIndex1, out columnIndexRanges);
      }
      this.EditableItems.CancelNew();
      this.UpdateNewItemPlaceholder(false);
      if (flag1)
        this.CurrentItem = CollectionView.NewItemPlaceholder;
      if (flag2)
      {
        this.SelectItem(CollectionView.NewItemPlaceholder);
      }
      else
      {
        if (!flag3)
          return;
        using (this.UpdateSelectedCells())
        {
          int rowIndex2 = rowIndex1;
          if (this.EditableItems.NewItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning)
          {
            this._selectedCells.RemoveRegion(rowIndex1, 0, 1, this.Columns.Count);
            --rowIndex2;
          }
          int index = 0;
          for (int count = columnIndexRanges.Count; index < count; index += 2)
            this._selectedCells.AddRegion(rowIndex2, columnIndexRanges[index], 1, columnIndexRanges[index + 1]);
        }
      }
    }
  }

  private void UpdateRowEditing(DataGridCell cell)
  {
    if (this.IsAddingOrEditingRowItem(cell.RowDataItem))
      return;
    cell.RowOwner.IsEditing = false;
    this._editingRowItem = (object) null;
    this._editingRowIndex = -1;
  }

  private IEditableCollectionView EditableItems => (IEditableCollectionView) this.Items;

  private bool IsAddingNewItem => this.EditableItems.IsAddingNew;

  private bool IsEditingRowItem => this.EditableItems.IsEditingItem;

  private bool IsAddingOrEditingRowItem(object item)
  {
    if (this.IsEditingItem(item))
      return true;
    return this.IsAddingNewItem && this.EditableItems.CurrentAddItem == item;
  }

  private bool CanCancelAddingOrEditingRowItem(DataGridEditingUnit editingUnit, object item)
  {
    if (editingUnit != DataGridEditingUnit.Row)
      return false;
    if (this.IsEditingItem(item) && this.EditableItems.CanCancelEdit)
      return true;
    return this.IsAddingNewItem && this.EditableItems.CurrentAddItem == item;
  }

  private bool IsEditingItem(object item)
  {
    return this.IsEditingRowItem && this.EditableItems.CurrentEditItem == item;
  }

  private void UpdateNewItemPlaceholder(bool isAddingNewItem)
  {
    IEditableCollectionView editableItems = this.EditableItems;
    bool baseValue = this.CanUserAddRows;
    if (DataGridHelper.IsDefaultValue((DependencyObject) this, DataGrid.CanUserAddRowsProperty))
      baseValue = DataGrid.OnCoerceCanUserAddOrDeleteRows(this, baseValue, true);
    if (!isAddingNewItem)
    {
      if (baseValue)
      {
        if (editableItems.NewItemPlaceholderPosition == NewItemPlaceholderPosition.None)
          editableItems.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
        this._placeholderVisibility = Visibility.Visible;
      }
      else
      {
        if (editableItems.NewItemPlaceholderPosition != NewItemPlaceholderPosition.None)
          editableItems.NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;
        this._placeholderVisibility = Visibility.Collapsed;
      }
    }
    else
      this._placeholderVisibility = Visibility.Collapsed;
    this.ItemContainerGenerator.ContainerFromItem(CollectionView.NewItemPlaceholder)?.CoerceValue(UIElement.VisibilityProperty);
  }

  private void SetCurrentItemToPlaceholder()
  {
    switch (this.EditableItems.NewItemPlaceholderPosition)
    {
      case NewItemPlaceholderPosition.AtBeginning:
        if (this.Items.Count <= 0)
          break;
        this.CurrentItem = this.Items[0];
        break;
      case NewItemPlaceholderPosition.AtEnd:
        int count = this.Items.Count;
        if (count <= 0)
          break;
        this.CurrentItem = this.Items[count - 1];
        break;
    }
  }

  private int DataItemsCount
  {
    get
    {
      int count = this.Items.Count;
      if (this.HasNewItemPlaceholder)
        --count;
      return count;
    }
  }

  private int DataItemsSelected
  {
    get
    {
      int count = this.SelectedItems.Count;
      if (this.HasNewItemPlaceholder && this.SelectedItems.Contains(CollectionView.NewItemPlaceholder))
        --count;
      return count;
    }
  }

  private bool HasNewItemPlaceholder
  {
    get => this.EditableItems.NewItemPlaceholderPosition != NewItemPlaceholderPosition.None;
  }

  private bool IsNewItemPlaceholder(object item)
  {
    return item == CollectionView.NewItemPlaceholder || item == DataGrid.NewItemPlaceholder;
  }

  public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
  {
    get
    {
      return (DataGridRowDetailsVisibilityMode) this.GetValue(DataGrid.RowDetailsVisibilityModeProperty);
    }
    set => this.SetValue(DataGrid.RowDetailsVisibilityModeProperty, (object) value);
  }

  public bool AreRowDetailsFrozen
  {
    get => (bool) this.GetValue(DataGrid.AreRowDetailsFrozenProperty);
    set => this.SetValue(DataGrid.AreRowDetailsFrozenProperty, (object) value);
  }

  public DataTemplate RowDetailsTemplate
  {
    get => (DataTemplate) this.GetValue(DataGrid.RowDetailsTemplateProperty);
    set => this.SetValue(DataGrid.RowDetailsTemplateProperty, (object) value);
  }

  public DataTemplateSelector RowDetailsTemplateSelector
  {
    get => (DataTemplateSelector) this.GetValue(DataGrid.RowDetailsTemplateSelectorProperty);
    set => this.SetValue(DataGrid.RowDetailsTemplateSelectorProperty, (object) value);
  }

  public event EventHandler<DataGridRowDetailsEventArgs> LoadingRowDetails;

  public event EventHandler<DataGridRowDetailsEventArgs> UnloadingRowDetails;

  public event EventHandler<DataGridRowDetailsEventArgs> RowDetailsVisibilityChanged;

  internal void OnLoadingRowDetailsWrapper(DataGridRow row)
  {
    if (row == null || row.DetailsLoaded || row.DetailsVisibility != Visibility.Visible || row.DetailsPresenter == null)
      return;
    this.OnLoadingRowDetails(new DataGridRowDetailsEventArgs(row, row.DetailsPresenter.DetailsElement));
    row.DetailsLoaded = true;
  }

  internal void OnUnloadingRowDetailsWrapper(DataGridRow row)
  {
    if (row == null || !row.DetailsLoaded || row.DetailsPresenter == null)
      return;
    this.OnUnloadingRowDetails(new DataGridRowDetailsEventArgs(row, row.DetailsPresenter.DetailsElement));
    row.DetailsLoaded = false;
  }

  protected virtual void OnLoadingRowDetails(DataGridRowDetailsEventArgs e)
  {
    if (this.LoadingRowDetails == null)
      return;
    this.LoadingRowDetails((object) this, e);
  }

  protected virtual void OnUnloadingRowDetails(DataGridRowDetailsEventArgs e)
  {
    if (this.UnloadingRowDetails == null)
      return;
    this.UnloadingRowDetails((object) this, e);
  }

  protected internal virtual void OnRowDetailsVisibilityChanged(DataGridRowDetailsEventArgs e)
  {
    if (this.RowDetailsVisibilityChanged != null)
      this.RowDetailsVisibilityChanged((object) this, e);
    this.OnLoadingRowDetailsWrapper(e.Row);
  }

  public bool CanUserResizeRows
  {
    get => (bool) this.GetValue(DataGrid.CanUserResizeRowsProperty);
    set => this.SetValue(DataGrid.CanUserResizeRowsProperty, (object) value);
  }

  public IList<DataGridCellInfo> SelectedCells => (IList<DataGridCellInfo>) this._selectedCells;

  internal SelectedCellsCollection SelectedCellsInternal => this._selectedCells;

  public event SelectedCellsChangedEventHandler SelectedCellsChanged;

  internal void OnSelectedCellsChanged(
    NotifyCollectionChangedAction action,
    VirtualizedCellInfoCollection oldItems,
    VirtualizedCellInfoCollection newItems)
  {
    DataGridSelectionMode selectionMode = this.SelectionMode;
    DataGridSelectionUnit selectionUnit = this.SelectionUnit;
    if (!this.IsUpdatingSelectedCells && selectionUnit == DataGridSelectionUnit.FullRow)
      throw new InvalidOperationException(SR.Get(SRID.DataGrid_CannotSelectCell));
    if (oldItems != null)
    {
      if (this._pendingSelectedCells != null)
        VirtualizedCellInfoCollection.Xor(this._pendingSelectedCells, oldItems);
      if (this._pendingUnselectedCells == null)
        this._pendingUnselectedCells = oldItems;
      else
        this._pendingUnselectedCells.Union(oldItems);
    }
    if (newItems != null)
    {
      if (this._pendingUnselectedCells != null)
        VirtualizedCellInfoCollection.Xor(this._pendingUnselectedCells, newItems);
      if (this._pendingSelectedCells == null)
        this._pendingSelectedCells = newItems;
      else
        this._pendingSelectedCells.Union(newItems);
    }
    if (this.IsUpdatingSelectedCells)
      return;
    using (this.UpdateSelectedCells())
    {
      if (selectionMode == DataGridSelectionMode.Single && action == NotifyCollectionChangedAction.Add && this._selectedCells.Count > 1)
      {
        this._selectedCells.RemoveAllButOne(newItems[0]);
      }
      else
      {
        if (action != NotifyCollectionChangedAction.Remove || oldItems == null || selectionUnit != DataGridSelectionUnit.CellOrRowHeader)
          return;
        bool updatingSelectedItems = this.IsUpdatingSelectedItems;
        if (!updatingSelectedItems)
          this.BeginUpdateSelectedItems();
        try
        {
          object obj1 = (object) null;
          foreach (DataGridCellInfo oldItem in oldItems)
          {
            object obj2 = oldItem.Item;
            if (obj2 != obj1)
            {
              obj1 = obj2;
              if (this.SelectedItems.Contains(obj2))
                this.SelectedItems.Remove(obj2);
            }
          }
        }
        finally
        {
          if (!updatingSelectedItems)
            this.EndUpdateSelectedItems();
        }
      }
    }
  }

  private void NotifySelectedCellsChanged()
  {
    if ((this._pendingSelectedCells == null || this._pendingSelectedCells.Count <= 0) && (this._pendingUnselectedCells == null || this._pendingUnselectedCells.Count <= 0))
      return;
    SelectedCellsChangedEventArgs e = new SelectedCellsChangedEventArgs(this, this._pendingSelectedCells, this._pendingUnselectedCells);
    int count1 = this._selectedCells.Count;
    int count2 = this._pendingUnselectedCells != null ? this._pendingUnselectedCells.Count : 0;
    int count3 = this._pendingSelectedCells != null ? this._pendingSelectedCells.Count : 0;
    int num = count1 - count3 + count2;
    this._pendingSelectedCells = (VirtualizedCellInfoCollection) null;
    this._pendingUnselectedCells = (VirtualizedCellInfoCollection) null;
    this.OnSelectedCellsChanged(e);
    if (num != 0 && count1 != 0)
      return;
    CommandManager.InvalidateRequerySuggested();
  }

  protected virtual void OnSelectedCellsChanged(SelectedCellsChangedEventArgs e)
  {
    if (this.SelectedCellsChanged != null)
      this.SelectedCellsChanged((object) this, e);
    if (!AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && !AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) && !AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection) || !(UIElementAutomationPeer.FromElement((UIElement) this) is Microsoft.Windows.Automation.Peers.DataGridAutomationPeer gridAutomationPeer))
      return;
    gridAutomationPeer.RaiseAutomationCellSelectedEvent(e);
  }

  public static RoutedUICommand SelectAllCommand => ApplicationCommands.SelectAll;

  private static void OnCanExecuteSelectAll(object sender, CanExecuteRoutedEventArgs e)
  {
    DataGrid dataGrid = (DataGrid) sender;
    e.CanExecute = dataGrid.SelectionMode == DataGridSelectionMode.Extended && dataGrid.IsEnabled;
    e.Handled = true;
  }

  private static void OnExecutedSelectAll(object sender, ExecutedRoutedEventArgs e)
  {
    DataGrid dataGrid = (DataGrid) sender;
    if (dataGrid.SelectionUnit == DataGridSelectionUnit.Cell)
      dataGrid.SelectAllCells();
    else
      dataGrid.SelectAllRows();
    e.Handled = true;
  }

  private void SelectAllRows()
  {
    int count1 = this.Items.Count;
    int count2 = this._columns.Count;
    if (count2 <= 0 || count1 <= 0)
      return;
    using (this.UpdateSelectedCells())
    {
      this._selectedCells.AddRegion(0, 0, count1, count2);
      this.SelectAll();
    }
  }

  internal void SelectOnlyThisCell(DataGridCellInfo currentCellInfo)
  {
    using (this.UpdateSelectedCells())
    {
      this._selectedCells.Clear();
      this._selectedCells.Add(currentCellInfo);
    }
  }

  public void SelectAllCells()
  {
    if (this.SelectionUnit == DataGridSelectionUnit.FullRow)
    {
      this.SelectAllRows();
    }
    else
    {
      int count1 = this.Items.Count;
      int count2 = this._columns.Count;
      if (count1 <= 0 || count2 <= 0)
        return;
      using (this.UpdateSelectedCells())
      {
        if (this._selectedCells.Count > 0)
          this._selectedCells.Clear();
        this._selectedCells.AddRegion(0, 0, count1, count2);
      }
    }
  }

  public void UnselectAllCells()
  {
    using (this.UpdateSelectedCells())
    {
      this._selectedCells.Clear();
      if (this.SelectionUnit == DataGridSelectionUnit.Cell)
        return;
      this.UnselectAll();
    }
  }

  public DataGridSelectionMode SelectionMode
  {
    get => (DataGridSelectionMode) this.GetValue(DataGrid.SelectionModeProperty);
    set => this.SetValue(DataGrid.SelectionModeProperty, (object) value);
  }

  private static void OnSelectionModeChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGrid dataGrid = (DataGrid) d;
    DataGridSelectionMode newValue = (DataGridSelectionMode) e.NewValue;
    bool flag = newValue == DataGridSelectionMode.Single;
    DataGridSelectionUnit selectionUnit = dataGrid.SelectionUnit;
    if (flag && selectionUnit == DataGridSelectionUnit.Cell)
    {
      using (dataGrid.UpdateSelectedCells())
        dataGrid._selectedCells.RemoveAllButOne();
    }
    dataGrid.CanSelectMultipleItems = newValue != DataGridSelectionMode.Single;
    if (!flag || selectionUnit != DataGridSelectionUnit.CellOrRowHeader)
      return;
    if (dataGrid.SelectedItems.Count > 0)
    {
      using (dataGrid.UpdateSelectedCells())
        dataGrid._selectedCells.RemoveAllButOneRow(dataGrid.Items.IndexOf(dataGrid.SelectedItems[0]));
    }
    else
    {
      using (dataGrid.UpdateSelectedCells())
        dataGrid._selectedCells.RemoveAllButOne();
    }
  }

  public DataGridSelectionUnit SelectionUnit
  {
    get => (DataGridSelectionUnit) this.GetValue(DataGrid.SelectionUnitProperty);
    set => this.SetValue(DataGrid.SelectionUnitProperty, (object) value);
  }

  private static void OnSelectionUnitChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGrid dataGrid = (DataGrid) d;
    DataGridSelectionUnit oldValue = (DataGridSelectionUnit) e.OldValue;
    if (oldValue != DataGridSelectionUnit.Cell)
      dataGrid.UnselectAll();
    if (oldValue != DataGridSelectionUnit.FullRow)
    {
      using (dataGrid.UpdateSelectedCells())
        dataGrid._selectedCells.Clear();
    }
    dataGrid.CoerceValue(Selector.IsSynchronizedWithCurrentItemProperty);
  }

  protected override void OnSelectionChanged(SelectionChangedEventArgs e)
  {
    if (!this.IsUpdatingSelectedCells)
    {
      using (this.UpdateSelectedCells())
      {
        int count1 = e.RemovedItems.Count;
        for (int index = 0; index < count1; ++index)
          this.UpdateSelectionOfCellsInRow(e.RemovedItems[index], false);
        int count2 = e.AddedItems.Count;
        for (int index = 0; index < count2; ++index)
          this.UpdateSelectionOfCellsInRow(e.AddedItems[index], true);
      }
    }
    CommandManager.InvalidateRequerySuggested();
    if ((AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection)) && UIElementAutomationPeer.FromElement((UIElement) this) is Microsoft.Windows.Automation.Peers.DataGridAutomationPeer gridAutomationPeer)
      gridAutomationPeer.RaiseAutomationSelectionEvents(e);
    base.OnSelectionChanged(e);
  }

  private void UpdateIsSelected()
  {
    this.UpdateIsSelected(this._pendingUnselectedCells, false);
    this.UpdateIsSelected(this._pendingSelectedCells, true);
  }

  private void UpdateIsSelected(VirtualizedCellInfoCollection cells, bool isSelected)
  {
    if (cells == null)
      return;
    int count1 = cells.Count;
    if (count1 <= 0)
      return;
    bool flag = false;
    if (count1 > 750)
    {
      int num = 0;
      int count2 = this._columns.Count;
      for (ContainerTracking<DataGridRow> containerTracking = this._rowTrackingRoot; containerTracking != null; containerTracking = containerTracking.Next)
      {
        num += count2;
        if (num >= count1)
          break;
      }
      flag = count1 > num;
    }
    if (flag)
    {
      for (ContainerTracking<DataGridRow> containerTracking1 = this._rowTrackingRoot; containerTracking1 != null; containerTracking1 = containerTracking1.Next)
      {
        Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = containerTracking1.Container.CellsPresenter;
        if (cellsPresenter != null)
        {
          for (ContainerTracking<DataGridCell> containerTracking2 = cellsPresenter.CellTrackingRoot; containerTracking2 != null; containerTracking2 = containerTracking2.Next)
          {
            DataGridCell container = containerTracking2.Container;
            DataGridCellInfo cell = new DataGridCellInfo(container);
            if (cells.Contains(cell))
              container.SyncIsSelected(isSelected);
          }
        }
      }
    }
    else
    {
      foreach (DataGridCellInfo cell in cells)
        this.TryFindCell(cell)?.SyncIsSelected(isSelected);
    }
  }

  private void UpdateSelectionOfCellsInRow(object rowItem, bool isSelected)
  {
    int rowIndex = this.Items.IndexOf(rowItem);
    if (rowIndex < 0)
      return;
    int count = this._columns.Count;
    if (count <= 0)
      return;
    if (isSelected)
      this._selectedCells.AddRegion(rowIndex, 0, 1, count);
    else
      this._selectedCells.RemoveRegion(rowIndex, 0, 1, count);
  }

  internal void CellIsSelectedChanged(DataGridCell cell, bool isSelected)
  {
    if (this.IsUpdatingSelectedCells)
      return;
    DataGridCellInfo cell1 = new DataGridCellInfo(cell);
    if (isSelected)
    {
      this._selectedCells.AddValidatedCell(cell1);
    }
    else
    {
      if (!this._selectedCells.Contains(cell1))
        return;
      this._selectedCells.Remove(cell1);
    }
  }

  internal void HandleSelectionForCellInput(
    DataGridCell cell,
    bool startDragging,
    bool allowsExtendSelect,
    bool allowsMinimalSelect)
  {
    if (this.SelectionUnit == DataGridSelectionUnit.FullRow)
      this.MakeFullRowSelection(cell.RowDataItem, allowsExtendSelect, allowsMinimalSelect);
    else
      this.MakeCellSelection(new DataGridCellInfo(cell), allowsExtendSelect, allowsMinimalSelect);
    if (!startDragging)
      return;
    this.BeginDragging();
  }

  internal void HandleSelectionForRowHeaderAndDetailsInput(DataGridRow row, bool startDragging)
  {
    object dataItem = row.Item;
    if (!this._isDraggingSelection && this._columns.Count > 0)
    {
      if (!this.IsKeyboardFocusWithin)
        this.Focus();
      if (this.CurrentCell.Item != dataItem)
        this.CurrentCell = new DataGridCellInfo(dataItem, this.ColumnFromDisplayIndex(0), this);
      else if (this._currentCellContainer != null && this._currentCellContainer.IsEditing)
        this.EndEdit(DataGrid.CommitEditCommand, this._currentCellContainer, DataGridEditingUnit.Cell, true);
    }
    if (!this.CanSelectRows)
      return;
    this.MakeFullRowSelection(dataItem, true, true);
    if (!startDragging)
      return;
    this.BeginRowDragging();
  }

  private void BeginRowDragging()
  {
    this.BeginDragging();
    this._isRowDragging = true;
  }

  private void BeginDragging()
  {
    if (!Mouse.Capture((IInputElement) this, CaptureMode.SubTree))
      return;
    this._isDraggingSelection = true;
    this._dragPoint = Mouse.GetPosition((IInputElement) this);
  }

  private void EndDragging()
  {
    this.StopAutoScroll();
    if (Mouse.Captured == this)
      this.ReleaseMouseCapture();
    this._isDraggingSelection = false;
    this._isRowDragging = false;
  }

  private void MakeFullRowSelection(
    object dataItem,
    bool allowsExtendSelect,
    bool allowsMinimalSelect)
  {
    bool flag1 = allowsExtendSelect && this.ShouldExtendSelection;
    bool flag2 = allowsMinimalSelect && DataGrid.ShouldMinimallyModifySelection;
    using (this.UpdateSelectedCells())
    {
      bool updatingSelectedItems = this.IsUpdatingSelectedItems;
      if (!updatingSelectedItems)
        this.BeginUpdateSelectedItems();
      try
      {
        if (flag1)
        {
          if (this._columns.Count <= 0)
            return;
          ItemCollection items = this.Items;
          int rowIndex1 = items.IndexOf(this._selectionAnchor.Value.Item);
          int num1 = items.IndexOf(dataItem);
          if (rowIndex1 > num1)
          {
            int num2 = rowIndex1;
            rowIndex1 = num1;
            num1 = num2;
          }
          if (rowIndex1 < 0 || num1 < 0)
            return;
          IList selectedItems = this.SelectedItems;
          int count = selectedItems.Count;
          if (!flag2)
          {
            bool flag3 = false;
            for (int index = 0; index < count; ++index)
            {
              object obj = selectedItems[index];
              int num3 = items.IndexOf(obj);
              if (num3 < rowIndex1 || num1 < num3)
              {
                selectedItems.RemoveAt(index);
                if (!flag3)
                {
                  this._selectedCells.Clear();
                  flag3 = true;
                }
              }
            }
          }
          else
          {
            int num4 = items.IndexOf(this.CurrentCell.Item);
            int rowIndex2 = -1;
            int num5 = -1;
            if (num4 < rowIndex1)
            {
              rowIndex2 = num4;
              num5 = rowIndex1 - 1;
            }
            else if (num4 > num1)
            {
              rowIndex2 = num1 + 1;
              num5 = num4;
            }
            if (rowIndex2 >= 0 && num5 >= 0)
            {
              for (int index = 0; index < count; ++index)
              {
                object obj = selectedItems[index];
                int num6 = items.IndexOf(obj);
                if (rowIndex2 <= num6 && num6 <= num5)
                  selectedItems.RemoveAt(index);
              }
              this._selectedCells.RemoveRegion(rowIndex2, 0, num5 - rowIndex2 + 1, this.Columns.Count);
            }
          }
          IEnumerator enumerator = ((IEnumerable) items).GetEnumerator();
          for (int index = 0; index <= num1 && enumerator.MoveNext(); ++index)
          {
            if (index >= rowIndex1)
              selectedItems.Add(enumerator.Current);
          }
          this._selectedCells.AddRegion(rowIndex1, 0, num1 - rowIndex1 + 1, this._columns.Count);
        }
        else
        {
          if (flag2 && this.SelectedItems.Contains(dataItem))
          {
            this.UnselectItem(dataItem);
          }
          else
          {
            if (!flag2 || !this.CanSelectMultipleItems)
            {
              if (this._selectedCells.Count > 0)
                this._selectedCells.Clear();
              if (this.SelectedItems.Count > 0)
                this.SelectedItems.Clear();
            }
            if (this._editingRowIndex >= 0 && this._editingRowItem == dataItem)
            {
              int count = this._columns.Count;
              if (count > 0)
                this._selectedCells.AddRegion(this._editingRowIndex, 0, 1, count);
              this.SelectItem(dataItem, false);
            }
            else
              this.SelectItem(dataItem);
          }
          this._selectionAnchor = new DataGridCellInfo?(new DataGridCellInfo(dataItem, this.ColumnFromDisplayIndex(0), this));
        }
      }
      finally
      {
        if (!updatingSelectedItems)
          this.EndUpdateSelectedItems();
      }
    }
  }

  private void MakeCellSelection(
    DataGridCellInfo cellInfo,
    bool allowsExtendSelect,
    bool allowsMinimalSelect)
  {
    bool flag1 = allowsExtendSelect && this.ShouldExtendSelection;
    bool flag2 = allowsMinimalSelect && DataGrid.ShouldMinimallyModifySelection;
    using (this.UpdateSelectedCells())
    {
      int displayIndex1 = cellInfo.Column.DisplayIndex;
      if (flag1)
      {
        ItemCollection items = this.Items;
        int val1 = items.IndexOf(this._selectionAnchor.Value.Item);
        int val2_1 = items.IndexOf(cellInfo.Item);
        if (this._editingRowIndex >= 0)
        {
          if (this._selectionAnchor.Value.Item == this._editingRowItem)
            val1 = this._editingRowIndex;
          if (cellInfo.Item == this._editingRowItem)
            val2_1 = this._editingRowIndex;
        }
        int displayIndex2 = this._selectionAnchor.Value.Column.DisplayIndex;
        int val2_2 = displayIndex1;
        if (val1 < 0 || val2_1 < 0 || displayIndex2 < 0 || val2_2 < 0)
          return;
        int rowCount1 = Math.Abs(val2_1 - val1) + 1;
        int columnCount1 = Math.Abs(val2_2 - displayIndex2) + 1;
        if (!flag2)
        {
          if (this.SelectedItems.Count > 0)
            this.UnselectAll();
          this._selectedCells.Clear();
        }
        else
        {
          int val2_3 = items.IndexOf(this.CurrentCell.Item);
          if (this._editingRowIndex >= 0 && this._editingRowItem == this.CurrentCell.Item)
            val2_3 = this._editingRowIndex;
          int displayIndex3 = this.CurrentCell.Column.DisplayIndex;
          int rowIndex = Math.Min(val1, val2_3);
          int rowCount2 = Math.Abs(val2_3 - val1) + 1;
          int columnIndex = Math.Min(displayIndex2, displayIndex3);
          int columnCount2 = Math.Abs(displayIndex3 - displayIndex2) + 1;
          this._selectedCells.RemoveRegion(rowIndex, columnIndex, rowCount2, columnCount2);
          if (this.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader)
          {
            int num1 = rowIndex;
            int num2 = rowIndex + rowCount2 - 1;
            if (columnCount2 <= columnCount1)
            {
              if (rowCount2 > rowCount1)
              {
                int num3 = rowCount2 - rowCount1;
                num1 = rowIndex == val2_3 ? val2_3 : val2_3 - num3 + 1;
                num2 = num1 + num3 - 1;
              }
              else
                num2 = num1 - 1;
            }
            for (int index = num1; index <= num2; ++index)
            {
              object obj = this.Items[index];
              if (this.SelectedItems.Contains(obj))
                this.SelectedItems.Remove(obj);
            }
          }
        }
        this._selectedCells.AddRegion(Math.Min(val1, val2_1), Math.Min(displayIndex2, val2_2), rowCount1, columnCount1);
      }
      else
      {
        bool flag3 = this._selectedCells.Contains(cellInfo);
        bool flag4 = this._editingRowIndex >= 0 && this._editingRowItem == cellInfo.Item;
        if (!flag3 && flag4)
          flag3 = this._selectedCells.Contains(this._editingRowIndex, displayIndex1);
        if (flag2 && flag3)
        {
          if (flag4)
            this._selectedCells.RemoveRegion(this._editingRowIndex, displayIndex1, 1, 1);
          else
            this._selectedCells.Remove(cellInfo);
          if (this.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader && this.SelectedItems.Contains(cellInfo.Item))
            this.SelectedItems.Remove(cellInfo.Item);
        }
        else
        {
          if (!flag2 || !this.CanSelectMultipleItems)
          {
            if (this.SelectedItems.Count > 0)
              this.UnselectAll();
            this._selectedCells.Clear();
          }
          if (flag4)
            this._selectedCells.AddRegion(this._editingRowIndex, displayIndex1, 1, 1);
          else
            this._selectedCells.AddValidatedCell(cellInfo);
        }
        this._selectionAnchor = new DataGridCellInfo?(cellInfo);
      }
    }
  }

  private void SelectItem(object item) => this.SelectItem(item, true);

  private void SelectItem(object item, bool selectCells)
  {
    if (selectCells)
    {
      using (this.UpdateSelectedCells())
      {
        int rowIndex = this.Items.IndexOf(item);
        int count = this._columns.Count;
        if (rowIndex >= 0)
        {
          if (count > 0)
            this._selectedCells.AddRegion(rowIndex, 0, 1, count);
        }
      }
    }
    this.UpdateSelectedItems(item, true);
  }

  private void UnselectItem(object item)
  {
    using (this.UpdateSelectedCells())
    {
      int rowIndex = this.Items.IndexOf(item);
      int count = this._columns.Count;
      if (rowIndex >= 0)
      {
        if (count > 0)
          this._selectedCells.RemoveRegion(rowIndex, 0, 1, count);
      }
    }
    this.UpdateSelectedItems(item, false);
  }

  private void UpdateSelectedItems(object item, bool add)
  {
    bool updatingSelectedItems = this.IsUpdatingSelectedItems;
    if (!updatingSelectedItems)
      this.BeginUpdateSelectedItems();
    try
    {
      if (add)
        this.SelectedItems.Add(item);
      else
        this.SelectedItems.Remove(item);
    }
    finally
    {
      if (!updatingSelectedItems)
        this.EndUpdateSelectedItems();
    }
  }

  private IDisposable UpdateSelectedCells()
  {
    return (IDisposable) new DataGrid.ChangingSelectedCellsHelper(this);
  }

  private void BeginUpdateSelectedCells() => this._updatingSelectedCells = true;

  private void EndUpdateSelectedCells()
  {
    this.UpdateIsSelected();
    this._updatingSelectedCells = false;
    this.NotifySelectedCellsChanged();
  }

  private bool IsUpdatingSelectedCells => this._updatingSelectedCells;

  private bool ShouldExtendSelection
  {
    get
    {
      if (!this.CanSelectMultipleItems || !this._selectionAnchor.HasValue)
        return false;
      return this._isDraggingSelection || (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
    }
  }

  private static bool ShouldMinimallyModifySelection
  {
    get => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
  }

  private bool CanSelectRows
  {
    get
    {
      switch (this.SelectionUnit)
      {
        case DataGridSelectionUnit.Cell:
          return false;
        case DataGridSelectionUnit.FullRow:
        case DataGridSelectionUnit.CellOrRowHeader:
          return true;
        default:
          return false;
      }
    }
  }

  private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    this._currentCellContainer = (DataGridCell) null;
    using (this.UpdateSelectedCells())
      this._selectedCells.OnItemsCollectionChanged(e, this.SelectedItems);
    if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
    {
      foreach (object oldItem in (IEnumerable) e.OldItems)
        this._itemAttachedStorage.ClearItem(oldItem);
    }
    else
    {
      if (e.Action != NotifyCollectionChangedAction.Reset)
        return;
      this._itemAttachedStorage.Clear();
    }
  }

  private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    d.CoerceValue(DataGrid.CanUserAddRowsProperty);
    d.CoerceValue(DataGrid.CanUserDeleteRowsProperty);
    if (!(bool) e.NewValue)
      ((DataGrid) d).UnselectAllCells();
    CommandManager.InvalidateRequerySuggested();
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    switch (e.Key)
    {
      case Key.Tab:
        this.OnTabKeyDown(e);
        break;
      case Key.Return:
        this.OnEnterKeyDown(e);
        break;
      case Key.Prior:
      case Key.Next:
        this.OnPageUpOrDownKeyDown(e);
        break;
      case Key.End:
      case Key.Home:
        this.OnHomeOrEndKeyDown(e);
        break;
      case Key.Left:
      case Key.Up:
      case Key.Right:
      case Key.Down:
        this.OnArrowKeyDown(e);
        break;
    }
    if (e.Handled)
      return;
    base.OnKeyDown(e);
  }

  private static FocusNavigationDirection KeyToTraversalDirection(Key key)
  {
    switch (key)
    {
      case Key.Left:
        return FocusNavigationDirection.Left;
      case Key.Up:
        return FocusNavigationDirection.Up;
      case Key.Right:
        return FocusNavigationDirection.Right;
      default:
        return FocusNavigationDirection.Down;
    }
  }

  private void OnArrowKeyDown(KeyEventArgs e)
  {
    DataGridCell currentCellContainer = this.CurrentCellContainer;
    if (currentCellContainer == null)
      return;
    e.Handled = true;
    bool isEditing = currentCellContainer.IsEditing;
    ContentElement focusedElement1 = !(Keyboard.FocusedElement is UIElement focusedElement2) ? Keyboard.FocusedElement as ContentElement : (ContentElement) null;
    if (focusedElement2 == null && focusedElement1 == null)
      return;
    bool flag1 = e.OriginalSource == currentCellContainer;
    if (flag1)
    {
      KeyboardNavigationMode directionalNavigation = KeyboardNavigation.GetDirectionalNavigation((DependencyObject) this);
      if (directionalNavigation == KeyboardNavigationMode.Once)
      {
        DependencyObject dependencyObject = this.PredictFocus(DataGrid.KeyToTraversalDirection(e.Key));
        if (dependencyObject == null || this.IsAncestorOf(dependencyObject))
          return;
        Keyboard.Focus(dependencyObject as IInputElement);
        return;
      }
      int displayIndex1 = this.CurrentColumn.DisplayIndex;
      object currentItem = this.CurrentItem;
      int num = this.Items.IndexOf(currentItem);
      if (this._editingRowIndex >= 0 && currentItem == this._editingRowItem)
        num = this._editingRowIndex;
      int displayIndex2 = displayIndex1;
      int index = num;
      bool flag2 = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
      Key key = e.Key;
      if (this.FlowDirection == FlowDirection.RightToLeft)
      {
        if (key == Key.Left)
          key = Key.Right;
        else if (key == Key.Right)
          key = Key.Left;
      }
      switch (key - 23)
      {
        case Key.None:
          if (flag2)
          {
            displayIndex2 = this.InternalColumns.FirstVisibleDisplayIndex;
            break;
          }
          --displayIndex2;
          while (displayIndex2 >= 0 && !this.ColumnFromDisplayIndex(displayIndex2).IsVisible)
            --displayIndex2;
          if (displayIndex2 < 0)
          {
            switch (directionalNavigation)
            {
              case KeyboardNavigationMode.Cycle:
                displayIndex2 = this.InternalColumns.LastVisibleDisplayIndex;
                break;
              case KeyboardNavigationMode.Contained:
                return;
              default:
                this.MoveFocus(new TraversalRequest(e.Key == Key.Left ? FocusNavigationDirection.Left : FocusNavigationDirection.Right));
                return;
            }
          }
          else
            break;
          break;
        case Key.Cancel:
          if (flag2)
          {
            index = 0;
            break;
          }
          --index;
          if (index < 0)
          {
            switch (directionalNavigation)
            {
              case KeyboardNavigationMode.Cycle:
                index = this.Items.Count - 1;
                break;
              case KeyboardNavigationMode.Contained:
                return;
              default:
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                return;
            }
          }
          else
            break;
          break;
        case Key.Back:
          if (flag2)
          {
            displayIndex2 = Math.Max(0, this.InternalColumns.LastVisibleDisplayIndex);
            break;
          }
          ++displayIndex2;
          int count = this.Columns.Count;
          while (displayIndex2 < count && !this.ColumnFromDisplayIndex(displayIndex2).IsVisible)
            ++displayIndex2;
          if (displayIndex2 >= this.Columns.Count)
          {
            switch (directionalNavigation)
            {
              case KeyboardNavigationMode.Cycle:
                displayIndex2 = this.InternalColumns.FirstVisibleDisplayIndex;
                break;
              case KeyboardNavigationMode.Contained:
                return;
              default:
                this.MoveFocus(new TraversalRequest(e.Key == Key.Left ? FocusNavigationDirection.Left : FocusNavigationDirection.Right));
                return;
            }
          }
          else
            break;
          break;
        default:
          if (flag2)
          {
            index = Math.Max(0, this.Items.Count - 1);
            break;
          }
          ++index;
          if (index >= this.Items.Count)
          {
            switch (directionalNavigation)
            {
              case KeyboardNavigationMode.Cycle:
                index = 0;
                break;
              case KeyboardNavigationMode.Contained:
                return;
              default:
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                return;
            }
          }
          else
            break;
          break;
      }
      DataGridColumn column = this.ColumnFromDisplayIndex(displayIndex2);
      object obj = this.Items[index];
      this.ScrollCellIntoView(obj, column);
      DataGridCell cell = this.TryFindCell(obj, column);
      if (cell == null || cell == currentCellContainer || !cell.Focus())
        return;
    }
    TraversalRequest request = new TraversalRequest(DataGrid.KeyToTraversalDirection(e.Key));
    if (!flag1 && (focusedElement2 == null || !focusedElement2.MoveFocus(request)) && (focusedElement1 == null || !focusedElement1.MoveFocus(request)))
      return;
    this.SelectAndEditOnFocusMove(e, currentCellContainer, isEditing, true, true);
  }

  private void OnTabKeyDown(KeyEventArgs e)
  {
    DataGridCell currentCellContainer = this.CurrentCellContainer;
    if (currentCellContainer == null)
      return;
    bool isEditing = currentCellContainer.IsEditing;
    bool flag = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
    ContentElement focusedElement1 = !(Keyboard.FocusedElement is UIElement focusedElement2) ? Keyboard.FocusedElement as ContentElement : (ContentElement) null;
    if (focusedElement2 == null && focusedElement1 == null)
      return;
    e.Handled = true;
    TraversalRequest request = new TraversalRequest(flag ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
    request.Wrapped = true;
    if ((focusedElement2 == null || !focusedElement2.MoveFocus(request)) && (focusedElement1 == null || !focusedElement1.MoveFocus(request)))
      return;
    if (isEditing && flag && Keyboard.FocusedElement == currentCellContainer)
      currentCellContainer.MoveFocus(request);
    if (this.IsGrouping && isEditing)
    {
      DataGridCell andEditOnFocusMove = this.GetCellForSelectAndEditOnFocusMove();
      if (andEditOnFocusMove != null && andEditOnFocusMove.RowDataItem == currentCellContainer.RowDataItem)
      {
        DataGridCell cell = this.TryFindCell(andEditOnFocusMove.RowDataItem, andEditOnFocusMove.Column);
        if (cell == null)
        {
          this.UpdateLayout();
          cell = this.TryFindCell(andEditOnFocusMove.RowDataItem, andEditOnFocusMove.Column);
        }
        if (cell != null && cell != andEditOnFocusMove)
          cell.Focus();
      }
    }
    this.SelectAndEditOnFocusMove(e, currentCellContainer, isEditing, false, true);
  }

  private void OnEnterKeyDown(KeyEventArgs e)
  {
    DataGridCell currentCellContainer = this.CurrentCellContainer;
    if (currentCellContainer == null || this._columns.Count <= 0)
      return;
    e.Handled = true;
    DataGridColumn column = currentCellContainer.Column;
    if (!this.CommitAnyEdit() || (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
      return;
    bool flag = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
    int count = this.Items.Count;
    int index = Math.Max(0, Math.Min(count - 1, this.Items.IndexOf(currentCellContainer.RowDataItem) + (flag ? -1 : 1)));
    if (index >= count)
      return;
    object obj = this.Items[index];
    this.ScrollIntoView(obj, column);
    if (this.CurrentCell.Item != obj)
    {
      this.CurrentCell = new DataGridCellInfo(obj, column, this);
      this.SelectAndEditOnFocusMove(e, currentCellContainer, false, false, true);
    }
    else
      this.CurrentCellContainer?.Focus();
  }

  private DataGridCell GetCellForSelectAndEditOnFocusMove()
  {
    if (!(Keyboard.FocusedElement is DataGridCell andEditOnFocusMove) && this.CurrentCellContainer != null && this.CurrentCellContainer.IsKeyboardFocusWithin)
      andEditOnFocusMove = this.CurrentCellContainer;
    return andEditOnFocusMove;
  }

  private void SelectAndEditOnFocusMove(
    KeyEventArgs e,
    DataGridCell oldCell,
    bool wasEditing,
    bool allowsExtendSelect,
    bool ignoreControlKey)
  {
    DataGridCell andEditOnFocusMove = this.GetCellForSelectAndEditOnFocusMove();
    if (andEditOnFocusMove == null || andEditOnFocusMove.DataGridOwner != this)
      return;
    if (ignoreControlKey || (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
    {
      if (this.ShouldSelectRowHeader && allowsExtendSelect)
        this.HandleSelectionForRowHeaderAndDetailsInput(andEditOnFocusMove.RowOwner, false);
      else
        this.HandleSelectionForCellInput(andEditOnFocusMove, false, allowsExtendSelect, false);
    }
    if (!wasEditing || andEditOnFocusMove.IsEditing || oldCell.RowDataItem != andEditOnFocusMove.RowDataItem)
      return;
    this.BeginEdit((RoutedEventArgs) e);
  }

  private void OnHomeOrEndKeyDown(KeyEventArgs e)
  {
    if (this._columns.Count <= 0 || this.Items.Count <= 0)
      return;
    e.Handled = true;
    bool flag = e.Key == Key.Home;
    object obj = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control ? this.Items[flag ? 0 : this.Items.Count - 1] : this.CurrentItem;
    DataGridColumn column = this.ColumnFromDisplayIndex(flag ? this.InternalColumns.FirstVisibleDisplayIndex : this.InternalColumns.LastVisibleDisplayIndex);
    this.ScrollCellIntoView(obj, column);
    DataGridCell cell = this.TryFindCell(obj, column);
    if (cell == null)
      return;
    cell.Focus();
    if (this.ShouldSelectRowHeader)
      this.HandleSelectionForRowHeaderAndDetailsInput(cell.RowOwner, false);
    else
      this.HandleSelectionForCellInput(cell, false, true, false);
  }

  private void OnPageUpOrDownKeyDown(KeyEventArgs e)
  {
    ScrollViewer internalScrollHost = this.InternalScrollHost;
    if (internalScrollHost == null)
      return;
    object currentItem = this.CurrentItem;
    DataGridColumn currentColumn = this.CurrentColumn;
    int num1 = this.Items.IndexOf(currentItem);
    if (num1 < 0)
      return;
    int num2 = Math.Max(1, (int) internalScrollHost.ViewportHeight - 1);
    object obj = this.Items[Math.Max(0, Math.Min(e.Key == Key.Prior ? num1 - num2 : num1 + num2, this.Items.Count - 1))];
    if (currentColumn == null)
    {
      this.ScrollRowIntoView(obj);
      this.CurrentItem = obj;
    }
    else
    {
      this.ScrollCellIntoView(obj, currentColumn);
      DataGridCell cell = this.TryFindCell(obj, currentColumn);
      if (cell == null)
        return;
      cell.Focus();
      if (this.ShouldSelectRowHeader)
        this.HandleSelectionForRowHeaderAndDetailsInput(cell.RowOwner, false);
      else
        this.HandleSelectionForCellInput(cell, false, true, false);
    }
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    if (!this._isDraggingSelection)
      return;
    if (e.LeftButton == MouseButtonState.Pressed)
    {
      Point position = Mouse.GetPosition((IInputElement) this);
      if (DoubleUtil.AreClose(position, this._dragPoint))
        return;
      this._dragPoint = position;
      DataGrid.RelativeMousePositions relativeMousePosition = this.RelativeMousePosition;
      if (relativeMousePosition == DataGrid.RelativeMousePositions.Over)
      {
        if (this._isRowDragging)
        {
          DataGridRow mouseOverRow = DataGrid.MouseOverRow;
          if (mouseOverRow == null || mouseOverRow.Item == this.CurrentItem)
            return;
          this.HandleSelectionForRowHeaderAndDetailsInput(mouseOverRow, false);
          this.CurrentItem = mouseOverRow.Item;
          e.Handled = true;
        }
        else
        {
          DataGridCell cell = DataGrid.MouseOverCell;
          if (cell == null && DataGrid.MouseOverRow != null)
            cell = this.GetCellNearMouse();
          if (cell == null || cell == this.CurrentCellContainer)
            return;
          this.HandleSelectionForCellInput(cell, false, true, true);
          cell.Focus();
          e.Handled = true;
        }
      }
      else if (this._isRowDragging && DataGrid.IsMouseToLeftOrRightOnly(relativeMousePosition))
      {
        DataGridRow rowNearMouse = this.GetRowNearMouse();
        if (rowNearMouse == null || rowNearMouse.Item == this.CurrentItem)
          return;
        this.HandleSelectionForRowHeaderAndDetailsInput(rowNearMouse, false);
        this.CurrentItem = rowNearMouse.Item;
        e.Handled = true;
      }
      else if (this._hasAutoScrolled)
      {
        if (!this.DoAutoScroll())
          return;
        e.Handled = true;
      }
      else
        this.StartAutoScroll();
    }
    else
      this.EndDragging();
  }

  private static void OnAnyMouseUpThunk(object sender, MouseButtonEventArgs e)
  {
    ((DataGrid) sender).OnAnyMouseUp(e);
  }

  private void OnAnyMouseUp(MouseButtonEventArgs e) => this.EndDragging();

  protected override void OnContextMenuOpening(ContextMenuEventArgs e)
  {
    cell = (DataGridCell) null;
    dataGridRowHeader = (Microsoft.Windows.Controls.Primitives.DataGridRowHeader) null;
    UIElement reference = e.OriginalSource as UIElement;
    while (true)
    {
      switch (reference)
      {
        case null:
        case DataGridCell cell:
        case Microsoft.Windows.Controls.Primitives.DataGridRowHeader dataGridRowHeader:
          goto label_3;
        default:
          reference = VisualTreeHelper.GetParent((DependencyObject) reference) as UIElement;
          continue;
      }
    }
label_3:
    if (cell != null && !cell.IsSelected && !cell.IsKeyboardFocusWithin)
    {
      cell.Focus();
      this.HandleSelectionForCellInput(cell, false, true, true);
    }
    if (dataGridRowHeader == null)
      return;
    DataGridRow parentRow = dataGridRowHeader.ParentRow;
    if (parentRow == null)
      return;
    this.HandleSelectionForRowHeaderAndDetailsInput(parentRow, false);
  }

  private DataGridRow GetRowNearMouse()
  {
    Panel internalItemsHost = this.InternalItemsHost;
    if (internalItemsHost != null)
    {
      for (int index = internalItemsHost.Children.Count - 1; index >= 0; --index)
      {
        if (internalItemsHost.Children[index] is DataGridRow child)
        {
          Point position = Mouse.GetPosition((IInputElement) child);
          Rect rect = new Rect(new Point(), child.RenderSize);
          if (position.Y >= rect.Top && position.Y <= rect.Bottom)
            return child;
        }
      }
    }
    return (DataGridRow) null;
  }

  private DataGridCell GetCellNearMouse()
  {
    Panel internalItemsHost = this.InternalItemsHost;
    if (internalItemsHost == null)
      return (DataGridCell) null;
    Rect itemsHostBounds = new Rect(new Point(), internalItemsHost.RenderSize);
    double num = double.PositiveInfinity;
    DataGridCell cellNearMouse = (DataGridCell) null;
    bool isMouseInCorner = DataGrid.IsMouseInCorner(this.RelativeMousePosition);
    for (int index = internalItemsHost.Children.Count - 1; index >= 0; --index)
    {
      if (internalItemsHost.Children[index] is DataGridRow child)
      {
        Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = child.CellsPresenter;
        if (cellsPresenter != null)
        {
          for (ContainerTracking<DataGridCell> containerTracking = cellsPresenter.CellTrackingRoot; containerTracking != null; containerTracking = containerTracking.Next)
          {
            DataGridCell container = containerTracking.Container;
            double distance;
            if (DataGrid.CalculateCellDistance((FrameworkElement) container, child, internalItemsHost, itemsHostBounds, isMouseInCorner, out distance) && (cellNearMouse == null || distance < num))
            {
              num = distance;
              cellNearMouse = container;
            }
          }
          Microsoft.Windows.Controls.Primitives.DataGridRowHeader rowHeader = child.RowHeader;
          double distance1;
          if (rowHeader != null && DataGrid.CalculateCellDistance((FrameworkElement) rowHeader, child, internalItemsHost, itemsHostBounds, isMouseInCorner, out distance1) && (cellNearMouse == null || distance1 < num))
          {
            DataGridCell cell = child.TryGetCell(this.DisplayIndexMap[0]);
            if (cell != null)
            {
              num = distance1;
              cellNearMouse = cell;
            }
          }
        }
      }
    }
    return cellNearMouse;
  }

  private static bool CalculateCellDistance(
    FrameworkElement cell,
    DataGridRow rowOwner,
    Panel itemsHost,
    Rect itemsHostBounds,
    bool isMouseInCorner,
    out double distance)
  {
    GeneralTransform ancestor = cell.TransformToAncestor((Visual) itemsHost);
    Rect rect1 = new Rect(new Point(), cell.RenderSize);
    if (itemsHostBounds.Contains(ancestor.TransformBounds(rect1)))
    {
      Point position1 = Mouse.GetPosition((IInputElement) cell);
      if (isMouseInCorner)
      {
        Vector vector = new Vector(position1.X - rect1.Width * 0.5, position1.Y - rect1.Height * 0.5);
        distance = vector.Length;
        return true;
      }
      Point position2 = Mouse.GetPosition((IInputElement) rowOwner);
      Rect rect2 = new Rect(new Point(), rowOwner.RenderSize);
      if (position1.X >= rect1.Left && position1.X <= rect1.Right)
      {
        distance = position2.Y < rect2.Top || position2.Y > rect2.Bottom ? Math.Abs(position1.Y - rect1.Top) : 0.0;
        return true;
      }
      if (position2.Y >= rect2.Top && position2.Y <= rect2.Bottom)
      {
        distance = Math.Abs(position1.X - rect1.Left);
        return true;
      }
    }
    distance = double.PositiveInfinity;
    return false;
  }

  private static DataGridRow MouseOverRow
  {
    get => DataGridHelper.FindVisualParent<DataGridRow>(Mouse.DirectlyOver as UIElement);
  }

  private static DataGridCell MouseOverCell
  {
    get => DataGridHelper.FindVisualParent<DataGridCell>(Mouse.DirectlyOver as UIElement);
  }

  private DataGrid.RelativeMousePositions RelativeMousePosition
  {
    get
    {
      DataGrid.RelativeMousePositions relativeMousePosition = DataGrid.RelativeMousePositions.Over;
      Panel internalItemsHost = this.InternalItemsHost;
      if (internalItemsHost != null)
      {
        Point position = Mouse.GetPosition((IInputElement) internalItemsHost);
        Rect rect = new Rect(new Point(), internalItemsHost.RenderSize);
        if (position.X < rect.Left)
          relativeMousePosition |= DataGrid.RelativeMousePositions.Left;
        else if (position.X > rect.Right)
          relativeMousePosition |= DataGrid.RelativeMousePositions.Right;
        if (position.Y < rect.Top)
          relativeMousePosition |= DataGrid.RelativeMousePositions.Above;
        else if (position.Y > rect.Bottom)
          relativeMousePosition |= DataGrid.RelativeMousePositions.Below;
      }
      return relativeMousePosition;
    }
  }

  private static bool IsMouseToLeft(DataGrid.RelativeMousePositions position)
  {
    return (position & DataGrid.RelativeMousePositions.Left) == DataGrid.RelativeMousePositions.Left;
  }

  private static bool IsMouseToRight(DataGrid.RelativeMousePositions position)
  {
    return (position & DataGrid.RelativeMousePositions.Right) == DataGrid.RelativeMousePositions.Right;
  }

  private static bool IsMouseAbove(DataGrid.RelativeMousePositions position)
  {
    return (position & DataGrid.RelativeMousePositions.Above) == DataGrid.RelativeMousePositions.Above;
  }

  private static bool IsMouseBelow(DataGrid.RelativeMousePositions position)
  {
    return (position & DataGrid.RelativeMousePositions.Below) == DataGrid.RelativeMousePositions.Below;
  }

  private static bool IsMouseToLeftOrRightOnly(DataGrid.RelativeMousePositions position)
  {
    return position == DataGrid.RelativeMousePositions.Left || position == DataGrid.RelativeMousePositions.Right;
  }

  private static bool IsMouseInCorner(DataGrid.RelativeMousePositions position)
  {
    return position != DataGrid.RelativeMousePositions.Over && position != DataGrid.RelativeMousePositions.Above && position != DataGrid.RelativeMousePositions.Below && position != DataGrid.RelativeMousePositions.Left && position != DataGrid.RelativeMousePositions.Right;
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DataGridAutomationPeer(this);
  }

  private DataGridCell TryFindCell(DataGridCellInfo info)
  {
    return this.TryFindCell(info.Item, info.Column);
  }

  internal DataGridCell TryFindCell(object item, DataGridColumn column)
  {
    DataGridRow dataGridRow = (DataGridRow) this.ItemContainerGenerator.ContainerFromItem(item);
    int index = this._columns.IndexOf(column);
    return dataGridRow != null && index >= 0 ? dataGridRow.TryGetCell(index) : (DataGridCell) null;
  }

  public bool CanUserSortColumns
  {
    get => (bool) this.GetValue(DataGrid.CanUserSortColumnsProperty);
    set => this.SetValue(DataGrid.CanUserSortColumnsProperty, (object) value);
  }

  private static object OnCoerceCanUserSortColumns(DependencyObject d, object baseValue)
  {
    DataGrid d1 = (DataGrid) d;
    return DataGridHelper.IsPropertyTransferEnabled((DependencyObject) d1, DataGrid.CanUserSortColumnsProperty) && DataGridHelper.IsDefaultValue((DependencyObject) d1, DataGrid.CanUserSortColumnsProperty) && !d1.Items.CanSort ? (object) false : baseValue;
  }

  private static void OnCanUserSortColumnsPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridHelper.TransferProperty(d, DataGrid.CanUserSortColumnsProperty);
    DataGrid.OnNotifyColumnPropertyChanged(d, e);
  }

  public event DataGridSortingEventHandler Sorting;

  protected virtual void OnSorting(DataGridSortingEventArgs eventArgs)
  {
    eventArgs.Handled = false;
    if (this.Sorting != null)
      this.Sorting((object) this, eventArgs);
    if (eventArgs.Handled)
      return;
    this.DefaultSort(eventArgs.Column, (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift);
  }

  internal void PerformSort(DataGridColumn sortColumn)
  {
    if (!this.CanUserSortColumns || !sortColumn.CanUserSort || !this.CommitAnyEdit())
      return;
    this.PrepareForSort(sortColumn);
    this.OnSorting(new DataGridSortingEventArgs(sortColumn));
    if (!this.Items.NeedsRefresh)
      return;
    try
    {
      this.Items.Refresh();
    }
    catch (InvalidOperationException ex)
    {
      this.Items.SortDescriptions.Clear();
      throw new InvalidOperationException(SR.Get(SRID.DataGrid_ProbableInvalidSortDescription), (Exception) ex);
    }
  }

  private void PrepareForSort(DataGridColumn sortColumn)
  {
    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift || this.Columns == null)
      return;
    foreach (DataGridColumn column in (Collection<DataGridColumn>) this.Columns)
    {
      if (column != sortColumn)
        column.SortDirection = new ListSortDirection?();
    }
  }

  private void DefaultSort(DataGridColumn column, bool clearExistingSortDescriptions)
  {
    ListSortDirection direction = ListSortDirection.Ascending;
    ListSortDirection? sortDirection = column.SortDirection;
    if (sortDirection.HasValue && sortDirection.Value == ListSortDirection.Ascending)
      direction = ListSortDirection.Descending;
    string sortMemberPath = column.SortMemberPath;
    if (string.IsNullOrEmpty(sortMemberPath))
      return;
    int index1 = -1;
    if (clearExistingSortDescriptions)
    {
      this.Items.SortDescriptions.Clear();
    }
    else
    {
      for (int index2 = 0; index2 < this.Items.SortDescriptions.Count; ++index2)
      {
        if (string.Compare(this.Items.SortDescriptions[index2].PropertyName, sortMemberPath, StringComparison.Ordinal) == 0 && (this.GroupingSortDescriptionIndices == null || !this.GroupingSortDescriptionIndices.Contains(index2)))
        {
          index1 = index2;
          break;
        }
      }
    }
    SortDescription sortDescription = new SortDescription(sortMemberPath, direction);
    try
    {
      if (index1 >= 0)
        this.Items.SortDescriptions[index1] = sortDescription;
      else
        this.Items.SortDescriptions.Add(sortDescription);
      if (!clearExistingSortDescriptions)
      {
        if (this._sortingStarted)
          goto label_19;
      }
      this.RegenerateGroupingSortDescriptions();
      this._sortingStarted = true;
    }
    catch (InvalidOperationException ex)
    {
      this.Items.SortDescriptions.Clear();
      throw new InvalidOperationException(SR.Get(SRID.DataGrid_InvalidSortDescription), (Exception) ex);
    }
label_19:
    column.SortDirection = new ListSortDirection?(direction);
  }

  private List<int> GroupingSortDescriptionIndices
  {
    get => this._groupingSortDescriptionIndices;
    set => this._groupingSortDescriptionIndices = value;
  }

  private void OnItemsSortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (this._ignoreSortDescriptionsChange || this.GroupingSortDescriptionIndices == null)
      return;
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        int index1 = 0;
        for (int count = this.GroupingSortDescriptionIndices.Count; index1 < count; ++index1)
        {
          if (this.GroupingSortDescriptionIndices[index1] >= e.NewStartingIndex)
          {
            List<int> descriptionIndices;
            int index2;
            (descriptionIndices = this.GroupingSortDescriptionIndices)[index2 = index1] = descriptionIndices[index2] + 1;
          }
        }
        break;
      case NotifyCollectionChangedAction.Remove:
        int index3 = 0;
        for (int count = this.GroupingSortDescriptionIndices.Count; index3 < count; ++index3)
        {
          if (this.GroupingSortDescriptionIndices[index3] > e.OldStartingIndex)
          {
            List<int> descriptionIndices;
            int index4;
            (descriptionIndices = this.GroupingSortDescriptionIndices)[index4 = index3] = descriptionIndices[index4] - 1;
          }
          else if (this.GroupingSortDescriptionIndices[index3] == e.OldStartingIndex)
          {
            this.GroupingSortDescriptionIndices.RemoveAt(index3);
            --index3;
            --count;
          }
        }
        break;
      case NotifyCollectionChangedAction.Replace:
        this.GroupingSortDescriptionIndices.Remove(e.OldStartingIndex);
        break;
      case NotifyCollectionChangedAction.Reset:
        this.GroupingSortDescriptionIndices.Clear();
        break;
    }
  }

  private void RemoveGroupingSortDescriptions()
  {
    if (this.GroupingSortDescriptionIndices == null)
      return;
    bool descriptionsChange = this._ignoreSortDescriptionsChange;
    this._ignoreSortDescriptionsChange = true;
    try
    {
      int index = 0;
      for (int count = this.GroupingSortDescriptionIndices.Count; index < count; ++index)
        this.Items.SortDescriptions.RemoveAt(this.GroupingSortDescriptionIndices[index] - index);
      this.GroupingSortDescriptionIndices.Clear();
    }
    finally
    {
      this._ignoreSortDescriptionsChange = descriptionsChange;
    }
  }

  private static bool CanConvertToSortDescription(PropertyGroupDescription propertyGroupDescription)
  {
    return propertyGroupDescription != null && propertyGroupDescription.Converter == null && propertyGroupDescription.StringComparison == StringComparison.Ordinal;
  }

  private void AddGroupingSortDescriptions()
  {
    bool descriptionsChange = this._ignoreSortDescriptionsChange;
    this._ignoreSortDescriptionsChange = true;
    try
    {
      int index = 0;
      foreach (GroupDescription groupDescription in (Collection<GroupDescription>) this.Items.GroupDescriptions)
      {
        PropertyGroupDescription propertyGroupDescription = groupDescription as PropertyGroupDescription;
        if (DataGrid.CanConvertToSortDescription(propertyGroupDescription))
        {
          SortDescription sortDescription = new SortDescription(propertyGroupDescription.PropertyName, ListSortDirection.Ascending);
          this.Items.SortDescriptions.Insert(index, sortDescription);
          if (this.GroupingSortDescriptionIndices == null)
            this.GroupingSortDescriptionIndices = new List<int>();
          this.GroupingSortDescriptionIndices.Add(index++);
        }
      }
    }
    finally
    {
      this._ignoreSortDescriptionsChange = descriptionsChange;
    }
  }

  private void RegenerateGroupingSortDescriptions()
  {
    this.RemoveGroupingSortDescriptions();
    this.AddGroupingSortDescriptions();
  }

  private void OnItemsGroupDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (!this._sortingStarted)
      return;
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        if (!DataGrid.CanConvertToSortDescription(e.NewItems[0] as PropertyGroupDescription))
          break;
        this.RegenerateGroupingSortDescriptions();
        break;
      case NotifyCollectionChangedAction.Remove:
        if (!DataGrid.CanConvertToSortDescription(e.OldItems[0] as PropertyGroupDescription))
          break;
        this.RegenerateGroupingSortDescriptions();
        break;
      case NotifyCollectionChangedAction.Replace:
        if (!DataGrid.CanConvertToSortDescription(e.OldItems[0] as PropertyGroupDescription) && !DataGrid.CanConvertToSortDescription(e.NewItems[0] as PropertyGroupDescription))
          break;
        this.RegenerateGroupingSortDescriptions();
        break;
      case NotifyCollectionChangedAction.Reset:
        this.RemoveGroupingSortDescriptions();
        break;
    }
  }

  public event EventHandler AutoGeneratedColumns;

  public event EventHandler<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn;

  public bool AutoGenerateColumns
  {
    get => (bool) this.GetValue(DataGrid.AutoGenerateColumnsProperty);
    set => this.SetValue(DataGrid.AutoGenerateColumnsProperty, (object) value);
  }

  protected virtual void OnAutoGeneratedColumns(EventArgs e)
  {
    if (this.AutoGeneratedColumns == null)
      return;
    this.AutoGeneratedColumns((object) this, e);
  }

  protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
  {
    if (this.AutoGeneratingColumn == null)
      return;
    this.AutoGeneratingColumn((object) this, e);
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    if (this._measureNeverInvoked)
    {
      this._measureNeverInvoked = false;
      if (this.AutoGenerateColumns)
        this.AddAutoColumns();
      this.InternalColumns.InitializeDisplayIndexMap();
      this.CoerceValue(DataGrid.FrozenColumnCountProperty);
      this.CoerceValue(DataGrid.CanUserAddRowsProperty);
      this.CoerceValue(DataGrid.CanUserDeleteRowsProperty);
      this.UpdateNewItemPlaceholder(false);
    }
    else if (this.DeferAutoGeneration && this.AutoGenerateColumns)
      this.AddAutoColumns();
    return base.MeasureOverride(availableSize);
  }

  private void ClearSortDescriptionsOnItemsSourceChange()
  {
    this.Items.SortDescriptions.Clear();
    this._sortingStarted = false;
    this.GroupingSortDescriptionIndices?.Clear();
    foreach (DataGridColumn column in (Collection<DataGridColumn>) this.Columns)
      column.SortDirection = new ListSortDirection?();
  }

  private static object OnCoerceItemsSourceProperty(DependencyObject d, object baseValue)
  {
    DataGrid dataGrid = (DataGrid) d;
    if (baseValue != dataGrid._cachedItemsSource && dataGrid._cachedItemsSource != null)
      dataGrid.ClearSortDescriptionsOnItemsSourceChange();
    return baseValue;
  }

  protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
  {
    base.OnItemsSourceChanged(oldValue, newValue);
    if (newValue == null)
      this.ClearSortDescriptionsOnItemsSourceChange();
    this._cachedItemsSource = newValue;
    using (this.UpdateSelectedCells())
      this._selectedCells.RestoreOnlyFullRows(this.SelectedItems);
    if (this.AutoGenerateColumns)
      this.RegenerateAutoColumns();
    this.InternalColumns.RefreshAutoWidthColumns = true;
    this.InternalColumns.InvalidateColumnWidthsComputation();
    this.CoerceValue(DataGrid.CanUserAddRowsProperty);
    this.CoerceValue(DataGrid.CanUserDeleteRowsProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGrid.CanUserSortColumnsProperty);
    this.ResetRowHeaderActualWidth();
    this.UpdateNewItemPlaceholder(false);
    this.HasCellValidationError = false;
    this.HasRowValidationError = false;
  }

  private bool DeferAutoGeneration { get; set; }

  protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
  {
    base.OnItemsChanged(e);
    if (e.Action == NotifyCollectionChangedAction.Add)
    {
      if (!this.DeferAutoGeneration)
        return;
      this.AddAutoColumns();
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
    {
      if (!this.HasRowValidationError && !this.HasCellValidationError)
        return;
      foreach (object oldItem in (IEnumerable) e.OldItems)
      {
        if (this.IsAddingOrEditingRowItem(oldItem))
        {
          this.HasRowValidationError = false;
          this.HasCellValidationError = false;
          break;
        }
      }
    }
    else
    {
      if (e.Action != NotifyCollectionChangedAction.Reset)
        return;
      this.ResetRowHeaderActualWidth();
      this.HasRowValidationError = false;
      this.HasCellValidationError = false;
    }
  }

  private void AddAutoColumns()
  {
    if (this.DataItemsCount == 0)
    {
      this.DeferAutoGeneration = true;
    }
    else
    {
      if (this._measureNeverInvoked)
        return;
      DataGrid.GenerateColumns((IItemProperties) this.Items, this, (Collection<DataGridColumn>) null);
      this.DeferAutoGeneration = false;
      this.OnAutoGeneratedColumns(EventArgs.Empty);
    }
  }

  private void DeleteAutoColumns()
  {
    if (!this.DeferAutoGeneration && !this._measureNeverInvoked)
    {
      for (int index = this.Columns.Count - 1; index >= 0; --index)
      {
        if (this.Columns[index].IsAutoGenerated)
          this.Columns.RemoveAt(index);
      }
    }
    else
      this.DeferAutoGeneration = false;
  }

  private void RegenerateAutoColumns()
  {
    this.DeleteAutoColumns();
    this.AddAutoColumns();
  }

  public static Collection<DataGridColumn> GenerateColumns(IItemProperties itemProperties)
  {
    if (itemProperties == null)
      throw new ArgumentNullException(nameof (itemProperties));
    Collection<DataGridColumn> columnCollection = new Collection<DataGridColumn>();
    DataGrid.GenerateColumns(itemProperties, (DataGrid) null, columnCollection);
    return columnCollection;
  }

  private static void GenerateColumns(
    IItemProperties iItemProperties,
    DataGrid dataGrid,
    Collection<DataGridColumn> columnCollection)
  {
    ReadOnlyCollection<ItemPropertyInfo> itemProperties = iItemProperties.ItemProperties;
    if (itemProperties == null || itemProperties.Count <= 0)
      return;
    foreach (ItemPropertyInfo itemPropertyInfo in itemProperties)
    {
      DataGridColumn defaultColumn = DataGridColumn.CreateDefaultColumn(itemPropertyInfo);
      if (dataGrid != null)
      {
        DataGridAutoGeneratingColumnEventArgs e = new DataGridAutoGeneratingColumnEventArgs(defaultColumn, itemPropertyInfo);
        dataGrid.OnAutoGeneratingColumn(e);
        if (!e.Cancel && e.Column != null)
        {
          e.Column.IsAutoGenerated = true;
          dataGrid.Columns.Add(e.Column);
        }
      }
      else
        columnCollection.Add(defaultColumn);
    }
  }

  private static void OnAutoGenerateColumnsPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    bool newValue = (bool) e.NewValue;
    DataGrid dataGrid = (DataGrid) d;
    if (newValue)
      dataGrid.AddAutoColumns();
    else
      dataGrid.DeleteAutoColumns();
  }

  public int FrozenColumnCount
  {
    get => (int) this.GetValue(DataGrid.FrozenColumnCountProperty);
    set => this.SetValue(DataGrid.FrozenColumnCountProperty, (object) value);
  }

  private static object OnCoerceFrozenColumnCount(DependencyObject d, object baseValue)
  {
    DataGrid dataGrid = (DataGrid) d;
    return (int) baseValue > dataGrid.Columns.Count ? (object) dataGrid.Columns.Count : baseValue;
  }

  private static void OnFrozenColumnCountPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.CellsPresenter | NotificationTarget.ColumnCollection | NotificationTarget.ColumnHeadersPresenter);
  }

  private static bool ValidateFrozenColumnCount(object value) => (int) value >= 0;

  public double NonFrozenColumnsViewportHorizontalOffset
  {
    get => (double) this.GetValue(DataGrid.NonFrozenColumnsViewportHorizontalOffsetProperty);
    internal set
    {
      this.SetValue(DataGrid.NonFrozenColumnsViewportHorizontalOffsetPropertyKey, (object) value);
    }
  }

  public override void OnApplyTemplate()
  {
    this.CleanUpInternalScrollControls();
    base.OnApplyTemplate();
  }

  public bool EnableRowVirtualization
  {
    get => (bool) this.GetValue(DataGrid.EnableRowVirtualizationProperty);
    set => this.SetValue(DataGrid.EnableRowVirtualizationProperty, (object) value);
  }

  private static void OnEnableRowVirtualizationChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGrid dataGrid = (DataGrid) d;
    dataGrid.CoerceValue(VirtualizingStackPanel.IsVirtualizingProperty);
    Panel internalItemsHost = dataGrid.InternalItemsHost;
    if (internalItemsHost == null)
      return;
    internalItemsHost.InvalidateMeasure();
    internalItemsHost.InvalidateArrange();
  }

  private static object OnCoerceIsVirtualizingProperty(DependencyObject d, object baseValue)
  {
    return !DataGridHelper.IsDefaultValue(d, DataGrid.EnableRowVirtualizationProperty) ? d.GetValue(DataGrid.EnableRowVirtualizationProperty) : baseValue;
  }

  public bool EnableColumnVirtualization
  {
    get => (bool) this.GetValue(DataGrid.EnableColumnVirtualizationProperty);
    set => this.SetValue(DataGrid.EnableColumnVirtualizationProperty, (object) value);
  }

  private static void OnEnableColumnVirtualizationChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.CellsPresenter | NotificationTarget.ColumnCollection | NotificationTarget.ColumnHeadersPresenter);
  }

  public bool CanUserReorderColumns
  {
    get => (bool) this.GetValue(DataGrid.CanUserReorderColumnsProperty);
    set => this.SetValue(DataGrid.CanUserReorderColumnsProperty, (object) value);
  }

  public Style DragIndicatorStyle
  {
    get => (Style) this.GetValue(DataGrid.DragIndicatorStyleProperty);
    set => this.SetValue(DataGrid.DragIndicatorStyleProperty, (object) value);
  }

  public Style DropLocationIndicatorStyle
  {
    get => (Style) this.GetValue(DataGrid.DropLocationIndicatorStyleProperty);
    set => this.SetValue(DataGrid.DropLocationIndicatorStyleProperty, (object) value);
  }

  public event EventHandler<DataGridColumnReorderingEventArgs> ColumnReordering;

  public event EventHandler<DragStartedEventArgs> ColumnHeaderDragStarted;

  public event EventHandler<DragDeltaEventArgs> ColumnHeaderDragDelta;

  public event EventHandler<DragCompletedEventArgs> ColumnHeaderDragCompleted;

  public event EventHandler<DataGridColumnEventArgs> ColumnReordered;

  protected internal virtual void OnColumnHeaderDragStarted(DragStartedEventArgs e)
  {
    if (this.ColumnHeaderDragStarted == null)
      return;
    this.ColumnHeaderDragStarted((object) this, e);
  }

  protected internal virtual void OnColumnReordering(DataGridColumnReorderingEventArgs e)
  {
    if (this.ColumnReordering == null)
      return;
    this.ColumnReordering((object) this, e);
  }

  protected internal virtual void OnColumnHeaderDragDelta(DragDeltaEventArgs e)
  {
    if (this.ColumnHeaderDragDelta == null)
      return;
    this.ColumnHeaderDragDelta((object) this, e);
  }

  protected internal virtual void OnColumnHeaderDragCompleted(DragCompletedEventArgs e)
  {
    if (this.ColumnHeaderDragCompleted == null)
      return;
    this.ColumnHeaderDragCompleted((object) this, e);
  }

  protected internal virtual void OnColumnReordered(DataGridColumnEventArgs e)
  {
    if (this.ColumnReordered == null)
      return;
    this.ColumnReordered((object) this, e);
  }

  private static void OnClipboardCopyModeChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    CommandManager.InvalidateRequerySuggested();
  }

  public DataGridClipboardCopyMode ClipboardCopyMode
  {
    get => (DataGridClipboardCopyMode) this.GetValue(DataGrid.ClipboardCopyModeProperty);
    set => this.SetValue(DataGrid.ClipboardCopyModeProperty, (object) value);
  }

  private static void OnCanExecuteCopy(object target, CanExecuteRoutedEventArgs args)
  {
    ((DataGrid) target).OnCanExecuteCopy(args);
  }

  protected virtual void OnCanExecuteCopy(CanExecuteRoutedEventArgs args)
  {
    args.CanExecute = this.ClipboardCopyMode != DataGridClipboardCopyMode.None && this._selectedCells.Count > 0;
    args.Handled = true;
  }

  private static void OnExecutedCopy(object target, ExecutedRoutedEventArgs args)
  {
    ((DataGrid) target).OnExecutedCopy(args);
  }

  protected virtual void OnExecutedCopy(ExecutedRoutedEventArgs args)
  {
    if (this.ClipboardCopyMode == DataGridClipboardCopyMode.None)
      throw new NotSupportedException(SR.Get(SRID.ClipboardCopyMode_Disabled));
    args.Handled = true;
    Collection<string> collection = new Collection<string>((IList<string>) new string[4]
    {
      DataFormats.Html,
      DataFormats.Text,
      DataFormats.UnicodeText,
      DataFormats.CommaSeparatedValue
    });
    Dictionary<string, StringBuilder> dictionary = new Dictionary<string, StringBuilder>(collection.Count);
    foreach (string key in collection)
      dictionary[key] = new StringBuilder();
    int minColumnDisplayIndex;
    int maxColumnDisplayIndex;
    int minRowIndex;
    int maxRowIndex;
    if (this._selectedCells.GetSelectionRange(out minColumnDisplayIndex, out maxColumnDisplayIndex, out minRowIndex, out maxRowIndex))
    {
      if (this.ClipboardCopyMode == DataGridClipboardCopyMode.IncludeHeader)
      {
        DataGridRowClipboardEventArgs args1 = new DataGridRowClipboardEventArgs((object) null, minColumnDisplayIndex, maxColumnDisplayIndex, true);
        this.OnCopyingRowClipboardContent(args1);
        foreach (string str in collection)
          dictionary[str].Append(args1.FormatClipboardCellValues(str));
      }
      for (int index = minRowIndex; index <= maxRowIndex; ++index)
      {
        object obj = this.Items[index];
        if (this._selectedCells.Intersects(index))
        {
          DataGridRowClipboardEventArgs args2 = new DataGridRowClipboardEventArgs(obj, minColumnDisplayIndex, maxColumnDisplayIndex, false, index);
          this.OnCopyingRowClipboardContent(args2);
          foreach (string str in collection)
            dictionary[str].Append(args2.FormatClipboardCellValues(str));
        }
      }
    }
    ClipboardHelper.GetClipboardContentForHtml(dictionary[DataFormats.Html]);
    try
    {
      DataObject data = new DataObject();
      foreach (string str in collection)
        data.SetData(str, (object) dictionary[str].ToString(), false);
      Clipboard.SetDataObject((object) data);
    }
    catch (SecurityException ex)
    {
      TextBox textBox = new TextBox();
      textBox.Text = dictionary[DataFormats.Text].ToString();
      textBox.SelectAll();
      textBox.Copy();
    }
  }

  protected virtual void OnCopyingRowClipboardContent(DataGridRowClipboardEventArgs args)
  {
    if (args.IsColumnHeadersRow)
    {
      for (int columnDisplayIndex = args.StartColumnDisplayIndex; columnDisplayIndex <= args.EndColumnDisplayIndex; ++columnDisplayIndex)
      {
        DataGridColumn column = this.ColumnFromDisplayIndex(columnDisplayIndex);
        if (column.IsVisible)
          args.ClipboardRowContent.Add(new DataGridClipboardCellContent(args.Item, column, column.Header));
      }
    }
    else
    {
      int rowIndex = args.RowIndexHint;
      if (rowIndex < 0)
        rowIndex = this.Items.IndexOf(args.Item);
      if (this._selectedCells.Intersects(rowIndex))
      {
        for (int columnDisplayIndex = args.StartColumnDisplayIndex; columnDisplayIndex <= args.EndColumnDisplayIndex; ++columnDisplayIndex)
        {
          DataGridColumn column = this.ColumnFromDisplayIndex(columnDisplayIndex);
          if (column.IsVisible)
          {
            object content = (object) null;
            if (this._selectedCells.Contains(rowIndex, columnDisplayIndex))
              content = column.OnCopyingCellClipboardContent(args.Item);
            args.ClipboardRowContent.Add(new DataGridClipboardCellContent(args.Item, column, content));
          }
        }
      }
    }
    if (this.CopyingRowClipboardContent == null)
      return;
    this.CopyingRowClipboardContent((object) this, args);
  }

  public event EventHandler<DataGridRowClipboardEventArgs> CopyingRowClipboardContent;

  internal double CellsPanelActualWidth
  {
    get => (double) this.GetValue(DataGrid.CellsPanelActualWidthProperty);
    set => this.SetValue(DataGrid.CellsPanelActualWidthProperty, (object) value);
  }

  private static void CellsPanelActualWidthChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (DoubleUtil.AreClose((double) e.OldValue, (double) e.NewValue))
      return;
    ((DataGrid) d).NotifyPropertyChanged(d, e, NotificationTarget.ColumnHeadersPresenter);
  }

  public double CellsPanelHorizontalOffset
  {
    get => (double) this.GetValue(DataGrid.CellsPanelHorizontalOffsetProperty);
    private set => this.SetValue(DataGrid.CellsPanelHorizontalOffsetPropertyKey, (object) value);
  }

  private bool CellsPanelHorizontalOffsetComputationPending { get; set; }

  internal void QueueInvalidateCellsPanelHorizontalOffset()
  {
    if (this.CellsPanelHorizontalOffsetComputationPending)
      return;
    this.Dispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(this.InvalidateCellsPanelHorizontalOffset), DispatcherPriority.Loaded, (object) this);
    this.CellsPanelHorizontalOffsetComputationPending = true;
  }

  private object InvalidateCellsPanelHorizontalOffset(object args)
  {
    if (!this.CellsPanelHorizontalOffsetComputationPending)
      return (object) null;
    IProvideDataGridColumn cellOrColumnHeader = this.GetAnyCellOrColumnHeader();
    this.CellsPanelHorizontalOffset = cellOrColumnHeader == null ? (double.IsNaN(this.RowHeaderWidth) ? 0.0 : this.RowHeaderWidth) : DataGridHelper.GetParentCellsPanelHorizontalOffset(cellOrColumnHeader);
    this.CellsPanelHorizontalOffsetComputationPending = false;
    return (object) null;
  }

  internal IProvideDataGridColumn GetAnyCellOrColumnHeader()
  {
    if (this._rowTrackingRoot != null)
    {
      for (ContainerTracking<DataGridRow> containerTracking1 = this._rowTrackingRoot; containerTracking1 != null; containerTracking1 = containerTracking1.Next)
      {
        if (containerTracking1.Container.IsVisible)
        {
          Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter cellsPresenter = containerTracking1.Container.CellsPresenter;
          if (cellsPresenter != null)
          {
            for (ContainerTracking<DataGridCell> containerTracking2 = cellsPresenter.CellTrackingRoot; containerTracking2 != null; containerTracking2 = containerTracking2.Next)
            {
              if (containerTracking2.Container.IsVisible)
                return (IProvideDataGridColumn) containerTracking2.Container;
            }
          }
        }
      }
    }
    if (this.ColumnHeadersPresenter != null)
    {
      for (ContainerTracking<Microsoft.Windows.Controls.Primitives.DataGridColumnHeader> containerTracking = this.ColumnHeadersPresenter.HeaderTrackingRoot; containerTracking != null; containerTracking = containerTracking.Next)
      {
        if (containerTracking.Container.IsVisible)
          return (IProvideDataGridColumn) containerTracking.Container;
      }
    }
    return (IProvideDataGridColumn) null;
  }

  internal double GetViewportWidthForColumns()
  {
    return this.InternalScrollHost == null ? 0.0 : this.InternalScrollHost.ViewportWidth - this.CellsPanelHorizontalOffset;
  }

  internal static object NewItemPlaceholder => DataGrid._newItemPlaceholder;

  private class ChangingSelectedCellsHelper : IDisposable
  {
    private DataGrid _dataGrid;
    private bool _wasUpdatingSelectedCells;

    internal ChangingSelectedCellsHelper(DataGrid dataGrid)
    {
      this._dataGrid = dataGrid;
      this._wasUpdatingSelectedCells = this._dataGrid.IsUpdatingSelectedCells;
      if (this._wasUpdatingSelectedCells)
        return;
      this._dataGrid.BeginUpdateSelectedCells();
    }

    public void Dispose()
    {
      GC.SuppressFinalize((object) this);
      if (this._wasUpdatingSelectedCells)
        return;
      this._dataGrid.EndUpdateSelectedCells();
    }
  }

  [Flags]
  private enum RelativeMousePositions
  {
    Over = 0,
    Above = 1,
    Below = 2,
    Left = 4,
    Right = 8,
  }
}
