// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridCell
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridCell : ContentControl, IProvideDataGridColumn
{
  private static readonly DependencyPropertyKey ColumnPropertyKey = DependencyProperty.RegisterReadOnly(nameof (Column), typeof (DataGridColumn), typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridCell.OnColumnChanged)));
  public static readonly DependencyProperty ColumnProperty = DataGridCell.ColumnPropertyKey.DependencyProperty;
  public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(nameof (IsEditing), typeof (bool), typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridCell.OnIsEditingChanged)));
  private static readonly DependencyPropertyKey IsReadOnlyPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsReadOnly), typeof (bool), typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridCell.OnNotifyIsReadOnlyChanged), new CoerceValueCallback(DataGridCell.OnCoerceIsReadOnly)));
  public static readonly DependencyProperty IsReadOnlyProperty = DataGridCell.IsReadOnlyPropertyKey.DependencyProperty;
  public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridCell.OnIsSelectedChanged)));
  public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (DataGridCell));
  public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (DataGridCell));
  private DataGridRow _owner;
  private ContainerTracking<DataGridCell> _tracker;
  private bool _syncingIsSelected;

  static DataGridCell()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridCell)));
    FrameworkElement.StyleProperty.OverrideMetadata(typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridCell.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridCell.OnCoerceStyle)));
    UIElement.ClipProperty.OverrideMetadata(typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridCell.OnCoerceClip)));
    KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Local));
    UIElement.SnapsToDevicePixelsProperty.OverrideMetadata(typeof (DataGridCell), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.AffectsArrange));
    EventManager.RegisterClassHandler(typeof (DataGridCell), UIElement.MouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(DataGridCell.OnAnyMouseLeftButtonDownThunk), true);
    EventManager.RegisterClassHandler(typeof (DataGridCell), UIElement.LostFocusEvent, (Delegate) new RoutedEventHandler(DataGridCell.OnAnyLostFocus), true);
    EventManager.RegisterClassHandler(typeof (DataGridCell), UIElement.GotFocusEvent, (Delegate) new RoutedEventHandler(DataGridCell.OnAnyGotFocus), true);
  }

  public DataGridCell() => this._tracker = new ContainerTracking<DataGridCell>(this);

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DataGridCellAutomationPeer(this);
  }

  internal void PrepareCell(object item, ItemsControl cellsPresenter, DataGridRow ownerRow)
  {
    this.PrepareCell(item, ownerRow, cellsPresenter.ItemContainerGenerator.IndexFromContainer((DependencyObject) this));
  }

  internal void PrepareCell(object item, DataGridRow ownerRow, int index)
  {
    this._owner = ownerRow;
    DataGrid dataGridOwner = this._owner.DataGridOwner;
    if (dataGridOwner != null)
    {
      if (index >= 0 && index < dataGridOwner.Columns.Count)
      {
        DataGridColumn column = dataGridOwner.Columns[index];
        this.Column = column;
        this.TabIndex = column.DisplayIndex;
      }
      if (this.IsEditing)
        this.IsEditing = false;
      else if (!(this.Content is FrameworkElement))
      {
        this.BuildVisualTree();
        if (!this.NeedsVisualTree)
          this.Content = item;
      }
      this.SyncIsSelected(dataGridOwner.SelectedCellsInternal.Contains(this));
    }
    DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.StyleProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridCell.IsReadOnlyProperty);
    this.CoerceValue(UIElement.ClipProperty);
  }

  internal void ClearCell(DataGridRow ownerRow) => this._owner = (DataGridRow) null;

  internal ContainerTracking<DataGridCell> Tracker => this._tracker;

  public DataGridColumn Column
  {
    get => (DataGridColumn) this.GetValue(DataGridCell.ColumnProperty);
    internal set => this.SetValue(DataGridCell.ColumnPropertyKey, (object) value);
  }

  private static void OnColumnChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is DataGridCell dataGridCell))
      return;
    dataGridCell.OnColumnChanged((DataGridColumn) e.OldValue, (DataGridColumn) e.NewValue);
  }

  protected virtual void OnColumnChanged(DataGridColumn oldColumn, DataGridColumn newColumn)
  {
    this.Content = (object) null;
    DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.StyleProperty);
    DataGridHelper.TransferProperty((DependencyObject) this, DataGridCell.IsReadOnlyProperty);
  }

  private static void OnNotifyPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridCell) d).NotifyPropertyChanged(d, string.Empty, e, NotificationTarget.Cells);
  }

  private static void OnNotifyIsReadOnlyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridCell cell = (DataGridCell) d;
    DataGrid dataGridOwner = cell.DataGridOwner;
    if ((bool) e.NewValue && dataGridOwner != null)
      dataGridOwner.CancelEdit(cell);
    CommandManager.InvalidateRequerySuggested();
    cell.NotifyPropertyChanged(d, string.Empty, e, NotificationTarget.Cells);
  }

  internal void NotifyPropertyChanged(
    DependencyObject d,
    string propertyName,
    DependencyPropertyChangedEventArgs e,
    NotificationTarget target)
  {
    if (d is DataGridColumn dataGridColumn && dataGridColumn != this.Column)
      return;
    if (DataGridHelper.ShouldNotifyCells(target))
    {
      if (e.Property == DataGridColumn.WidthProperty)
        DataGridHelper.OnColumnWidthChanged((IProvideDataGridColumn) this, e);
      else if (e.Property == DataGrid.CellStyleProperty || e.Property == DataGridColumn.CellStyleProperty || e.Property == FrameworkElement.StyleProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.StyleProperty);
      else if (e.Property == DataGrid.IsReadOnlyProperty || e.Property == DataGridColumn.IsReadOnlyProperty || e.Property == DataGridCell.IsReadOnlyProperty)
        DataGridHelper.TransferProperty((DependencyObject) this, DataGridCell.IsReadOnlyProperty);
      else if (e.Property == DataGridColumn.DisplayIndexProperty)
        this.TabIndex = dataGridColumn.DisplayIndex;
    }
    if (!DataGridHelper.ShouldRefreshCellContent(target) || dataGridColumn == null || !this.NeedsVisualTree)
      return;
    if (!string.IsNullOrEmpty(propertyName))
    {
      dataGridColumn.RefreshCellContent((FrameworkElement) this, propertyName);
    }
    else
    {
      if (e.Property == null)
        return;
      dataGridColumn.RefreshCellContent((FrameworkElement) this, e.Property.Name);
    }
  }

  private static object OnCoerceStyle(DependencyObject d, object baseValue)
  {
    DataGridCell baseObject = d as DataGridCell;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, FrameworkElement.StyleProperty, (DependencyObject) baseObject.Column, DataGridColumn.CellStyleProperty, (DependencyObject) baseObject.DataGridOwner, DataGrid.CellStyleProperty);
  }

  internal void BuildVisualTree()
  {
    if (!this.NeedsVisualTree)
      return;
    DataGridColumn column = this.Column;
    if (column == null)
      return;
    DataGridRow rowOwner = this.RowOwner;
    if (rowOwner != null)
    {
      BindingGroup bindingGroup = rowOwner.BindingGroup;
      if (bindingGroup != null)
        this.RemoveBindingExpressions(bindingGroup, this.Content as DependencyObject);
    }
    this.Content = (object) column.BuildVisualTree(this.IsEditing, this.RowDataItem, this);
  }

  private void RemoveBindingExpressions(BindingGroup bindingGroup, DependencyObject element)
  {
    if (element == null)
      return;
    Collection<BindingExpressionBase> bindingExpressions = bindingGroup.BindingExpressions;
    LocalValueEnumerator localValueEnumerator = element.GetLocalValueEnumerator();
    while (localValueEnumerator.MoveNext())
    {
      if (localValueEnumerator.Current.Value is BindingExpression objA)
      {
        for (int index = 0; index < bindingExpressions.Count; ++index)
        {
          if (object.ReferenceEquals((object) objA, (object) bindingExpressions[index]))
            bindingExpressions.RemoveAt(index--);
        }
      }
    }
    foreach (object child in LogicalTreeHelper.GetChildren(element))
      this.RemoveBindingExpressions(bindingGroup, child as DependencyObject);
  }

  public bool IsEditing
  {
    get => (bool) this.GetValue(DataGridCell.IsEditingProperty);
    set => this.SetValue(DataGridCell.IsEditingProperty, (object) value);
  }

  private static void OnIsEditingChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    ((DataGridCell) sender).OnIsEditingChanged((bool) e.NewValue);
  }

  protected virtual void OnIsEditingChanged(bool isEditing)
  {
    if (this.IsKeyboardFocusWithin && !this.IsKeyboardFocused)
      this.Focus();
    this.BuildVisualTree();
  }

  public bool IsReadOnly => (bool) this.GetValue(DataGridCell.IsReadOnlyProperty);

  private static object OnCoerceIsReadOnly(DependencyObject d, object baseValue)
  {
    DataGridCell dataGridCell = d as DataGridCell;
    DataGridColumn column = dataGridCell.Column;
    DataGrid dataGridOwner = dataGridCell.DataGridOwner;
    return DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) column, (object) column.IsReadOnly, DataGridColumn.IsReadOnlyProperty, (DependencyObject) dataGridOwner, DataGrid.IsReadOnlyProperty);
  }

  private static void OnAnyLostFocus(object sender, RoutedEventArgs e)
  {
    DataGridCell visualParent = DataGridHelper.FindVisualParent<DataGridCell>(e.OriginalSource as UIElement);
    if (visualParent == null || visualParent != sender)
      return;
    DataGrid dataGridOwner = visualParent.DataGridOwner;
    if (dataGridOwner == null || visualParent.IsKeyboardFocusWithin || dataGridOwner.FocusedCell != visualParent)
      return;
    dataGridOwner.FocusedCell = (DataGridCell) null;
  }

  private static void OnAnyGotFocus(object sender, RoutedEventArgs e)
  {
    DataGridCell visualParent = DataGridHelper.FindVisualParent<DataGridCell>(e.OriginalSource as UIElement);
    if (visualParent == null || visualParent != sender)
      return;
    DataGrid dataGridOwner = visualParent.DataGridOwner;
    if (dataGridOwner == null)
      return;
    dataGridOwner.FocusedCell = visualParent;
  }

  internal void BeginEdit(RoutedEventArgs e)
  {
    this.IsEditing = true;
    this.Column?.BeginEdit(this.Content as FrameworkElement, e);
    this.RaisePreparingCellForEdit(e);
  }

  internal void CancelEdit()
  {
    this.Column?.CancelEdit(this.Content as FrameworkElement);
    this.IsEditing = false;
  }

  internal bool CommitEdit()
  {
    bool flag = true;
    DataGridColumn column = this.Column;
    if (column != null)
      flag = column.CommitEdit(this.Content as FrameworkElement);
    if (flag)
      this.IsEditing = false;
    return flag;
  }

  private void RaisePreparingCellForEdit(RoutedEventArgs editingEventArgs)
  {
    DataGrid dataGridOwner = this.DataGridOwner;
    if (dataGridOwner == null)
      return;
    FrameworkElement editingElement = this.EditingElement;
    DataGridPreparingCellForEditEventArgs e = new DataGridPreparingCellForEditEventArgs(this.Column, this.RowOwner, editingEventArgs, editingElement);
    dataGridOwner.OnPreparingCellForEdit(e);
  }

  internal FrameworkElement EditingElement => this.Content as FrameworkElement;

  public bool IsSelected
  {
    get => (bool) this.GetValue(DataGridCell.IsSelectedProperty);
    set => this.SetValue(DataGridCell.IsSelectedProperty, (object) value);
  }

  private static void OnIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    DataGridCell cell = (DataGridCell) sender;
    bool newValue = (bool) e.NewValue;
    if (!cell._syncingIsSelected)
      cell.DataGridOwner?.CellIsSelectedChanged(cell, newValue);
    cell.RaiseSelectionChangedEvent(newValue);
  }

  internal void SyncIsSelected(bool isSelected)
  {
    bool syncingIsSelected = this._syncingIsSelected;
    this._syncingIsSelected = true;
    try
    {
      this.IsSelected = isSelected;
    }
    finally
    {
      this._syncingIsSelected = syncingIsSelected;
    }
  }

  private void RaiseSelectionChangedEvent(bool isSelected)
  {
    if (isSelected)
      this.OnSelected(new RoutedEventArgs(DataGridCell.SelectedEvent, (object) this));
    else
      this.OnUnselected(new RoutedEventArgs(DataGridCell.UnselectedEvent, (object) this));
  }

  public event RoutedEventHandler Selected
  {
    add => this.AddHandler(DataGridCell.SelectedEvent, (Delegate) value);
    remove => this.RemoveHandler(DataGridCell.SelectedEvent, (Delegate) value);
  }

  protected virtual void OnSelected(RoutedEventArgs e) => this.RaiseEvent(e);

  public event RoutedEventHandler Unselected
  {
    add => this.AddHandler(DataGridCell.UnselectedEvent, (Delegate) value);
    remove => this.RemoveHandler(DataGridCell.UnselectedEvent, (Delegate) value);
  }

  protected virtual void OnUnselected(RoutedEventArgs e) => this.RaiseEvent(e);

  protected override Size MeasureOverride(Size constraint)
  {
    if (!DataGridHelper.IsGridLineVisible(this.DataGridOwner, false))
      return base.MeasureOverride(constraint);
    double gridLineThickness = this.DataGridOwner.VerticalGridLineThickness;
    Size size = base.MeasureOverride(DataGridHelper.SubtractFromSize(constraint, gridLineThickness, false));
    size.Width += gridLineThickness;
    return size;
  }

  protected override Size ArrangeOverride(Size arrangeSize)
  {
    if (!DataGridHelper.IsGridLineVisible(this.DataGridOwner, false))
      return base.ArrangeOverride(arrangeSize);
    double gridLineThickness = this.DataGridOwner.VerticalGridLineThickness;
    Size size = base.ArrangeOverride(DataGridHelper.SubtractFromSize(arrangeSize, gridLineThickness, false));
    size.Width += gridLineThickness;
    return size;
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    base.OnRender(drawingContext);
    if (!DataGridHelper.IsGridLineVisible(this.DataGridOwner, false))
      return;
    double gridLineThickness = this.DataGridOwner.VerticalGridLineThickness;
    drawingContext.DrawRectangle(this.DataGridOwner.VerticalGridLinesBrush, (Pen) null, new Rect(new Size(gridLineThickness, this.RenderSize.Height))
    {
      X = this.RenderSize.Width - gridLineThickness
    });
  }

  private static void OnAnyMouseLeftButtonDownThunk(object sender, MouseButtonEventArgs e)
  {
    ((DataGridCell) sender).OnAnyMouseLeftButtonDown(e);
  }

  private void OnAnyMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    bool keyboardFocusWithin = this.IsKeyboardFocusWithin;
    bool flag = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
    if (keyboardFocusWithin && !flag && !e.Handled && !this.IsEditing && !this.IsReadOnly && this.IsSelected)
    {
      DataGrid dataGridOwner = this.DataGridOwner;
      if (dataGridOwner == null)
        return;
      dataGridOwner.HandleSelectionForCellInput(this, false, true, false);
      dataGridOwner.BeginEdit((RoutedEventArgs) e);
      e.Handled = true;
    }
    else
    {
      if (keyboardFocusWithin && this.IsSelected && !flag)
        return;
      if (!keyboardFocusWithin)
        this.Focus();
      this.DataGridOwner?.HandleSelectionForCellInput(this, Mouse.Captured == null, true, true);
      e.Handled = true;
    }
  }

  protected override void OnTextInput(TextCompositionEventArgs e)
  {
    this.SendInputToColumn((InputEventArgs) e);
  }

  protected override void OnKeyDown(KeyEventArgs e) => this.SendInputToColumn((InputEventArgs) e);

  private void SendInputToColumn(InputEventArgs e) => this.Column?.OnInput(e);

  private static object OnCoerceClip(DependencyObject d, object baseValue)
  {
    DataGridCell cell = (DataGridCell) d;
    Geometry geometry1 = baseValue as Geometry;
    Geometry frozenClipForCell = DataGridHelper.GetFrozenClipForCell((IProvideDataGridColumn) cell);
    if (frozenClipForCell != null)
    {
      if (geometry1 == null)
        return (object) frozenClipForCell;
      geometry1 = (Geometry) new CombinedGeometry(GeometryCombineMode.Intersect, geometry1, frozenClipForCell);
    }
    return (object) geometry1;
  }

  internal DataGrid DataGridOwner
  {
    get
    {
      return this._owner != null ? this._owner.DataGridOwner ?? ItemsControl.ItemsControlFromItemContainer((DependencyObject) this._owner) as DataGrid : (DataGrid) null;
    }
  }

  private Panel ParentPanel => this.VisualParent as Panel;

  internal DataGridRow RowOwner => this._owner;

  internal object RowDataItem
  {
    get
    {
      DataGridRow rowOwner = this.RowOwner;
      return rowOwner != null ? rowOwner.Item : this.DataContext;
    }
  }

  private DataGridCellsPresenter CellsPresenter
  {
    get
    {
      return ItemsControl.ItemsControlFromItemContainer((DependencyObject) this) as DataGridCellsPresenter;
    }
  }

  private bool NeedsVisualTree
  {
    get => this.ContentTemplate == null && this.ContentTemplateSelector == null;
  }
}
