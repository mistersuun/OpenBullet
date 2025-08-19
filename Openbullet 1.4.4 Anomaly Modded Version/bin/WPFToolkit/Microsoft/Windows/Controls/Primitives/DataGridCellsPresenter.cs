// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

public class DataGridCellsPresenter : ItemsControl
{
  private object _item;
  private Microsoft.Windows.Controls.ContainerTracking<Microsoft.Windows.Controls.DataGridCell> _cellTrackingRoot;
  private Panel _internalItemsHost;

  static DataGridCellsPresenter()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridCellsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridCellsPresenter)));
    ItemsControl.ItemsPanelProperty.OverrideMetadata(typeof (DataGridCellsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) new ItemsPanelTemplate(new FrameworkElementFactory(typeof (Microsoft.Windows.Controls.DataGridCellsPanel)))));
    UIElement.FocusableProperty.OverrideMetadata(typeof (DataGridCellsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    FrameworkElement.HeightProperty.OverrideMetadata(typeof (DataGridCellsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridCellsPresenter.OnNotifyHeightPropertyChanged), new CoerceValueCallback(DataGridCellsPresenter.OnCoerceHeight)));
    FrameworkElement.MinHeightProperty.OverrideMetadata(typeof (DataGridCellsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridCellsPresenter.OnNotifyHeightPropertyChanged), new CoerceValueCallback(DataGridCellsPresenter.OnCoerceMinHeight)));
    VirtualizingStackPanel.IsVirtualizingProperty.OverrideMetadata(typeof (DataGridCellsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridCellsPresenter.OnIsVirtualizingPropertyChanged), new CoerceValueCallback(DataGridCellsPresenter.OnCoerceIsVirtualizingProperty)));
    VirtualizingStackPanel.VirtualizationModeProperty.OverrideMetadata(typeof (DataGridCellsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) VirtualizationMode.Recycling));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    if (dataGridRowOwner != null)
    {
      dataGridRowOwner.CellsPresenter = this;
      this.Item = dataGridRowOwner.Item;
    }
    this.SyncProperties(false);
  }

  internal void SyncProperties(bool forcePrepareCells)
  {
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.DataGridOwner;
    if (dataGridOwner == null)
      return;
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.HeightProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.MinHeightProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, VirtualizingStackPanel.IsVirtualizingProperty);
    this.NotifyPropertyChanged((DependencyObject) this, new DependencyPropertyChangedEventArgs(Microsoft.Windows.Controls.DataGrid.CellStyleProperty, (object) null, (object) null), NotificationTarget.Cells);
    if (!(this.ItemsSource is Microsoft.Windows.Controls.MultipleCopiesCollection itemsSource))
      return;
    ObservableCollection<Microsoft.Windows.Controls.DataGridColumn> columns = dataGridOwner.Columns;
    int count1 = columns.Count;
    int count2 = itemsSource.Count;
    int num = 0;
    if (count1 != count2)
    {
      itemsSource.SyncToCount(count1);
      num = Math.Min(count1, count2);
    }
    else if (forcePrepareCells)
      num = count1;
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    bool flag = false;
    for (int index = 0; index < num; ++index)
    {
      Microsoft.Windows.Controls.DataGridCell dataGridCell = (Microsoft.Windows.Controls.DataGridCell) this.ItemContainerGenerator.ContainerFromIndex(index);
      if (dataGridCell != null)
      {
        dataGridCell.PrepareCell(dataGridRowOwner.Item, (ItemsControl) this, dataGridRowOwner);
        if (!flag && !DoubleUtil.AreClose(dataGridCell.ActualWidth, columns[index].Width.DisplayValue))
        {
          this.InvalidateDataGridCellsPanelMeasureAndArrange();
          flag = true;
        }
      }
    }
    if (!flag)
    {
      for (int index = num; index < count1; ++index)
      {
        Microsoft.Windows.Controls.DataGridCell dataGridCell = (Microsoft.Windows.Controls.DataGridCell) this.ItemContainerGenerator.ContainerFromIndex(index);
        if (dataGridCell != null && !DoubleUtil.AreClose(dataGridCell.ActualWidth, columns[index].Width.DisplayValue))
        {
          this.InvalidateDataGridCellsPanelMeasureAndArrange();
          flag = true;
          break;
        }
      }
    }
    if (flag || !this.InvalidateCellsPanelOnColumnChange())
      return;
    this.InvalidateDataGridCellsPanelMeasureAndArrange();
  }

  private bool InvalidateCellsPanelOnColumnChange()
  {
    if (this.InternalItemsHost == null)
      return false;
    bool isVirtualizing = VirtualizingStackPanel.GetIsVirtualizing((DependencyObject) this);
    List<Microsoft.Windows.Controls.RealizedColumnsBlock> realizedColumnsBlockList = (List<Microsoft.Windows.Controls.RealizedColumnsBlock>) null;
    if (isVirtualizing && !this.DataGridOwner.InternalColumns.RebuildRealizedColumnsBlockListForVirtualizedRows)
      realizedColumnsBlockList = this.DataGridOwner.InternalColumns.RealizedColumnsBlockListForVirtualizedRows;
    else if (!isVirtualizing && !this.DataGridOwner.InternalColumns.RebuildRealizedColumnsBlockListForNonVirtualizedRows)
      realizedColumnsBlockList = this.DataGridOwner.InternalColumns.RealizedColumnsBlockListForNonVirtualizedRows;
    if (realizedColumnsBlockList == null)
      return true;
    IList children = (IList) this.InternalItemsHost.Children;
    int index1 = 0;
    int index2 = 0;
    int count1 = children.Count;
    int count2 = realizedColumnsBlockList.Count;
    int count3 = this.DataGridOwner.Columns.Count;
    for (int index3 = 0; index3 < count3; ++index3)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (index1 < count2)
      {
        Microsoft.Windows.Controls.RealizedColumnsBlock realizedColumnsBlock = realizedColumnsBlockList[index1];
        if (realizedColumnsBlock.StartIndex <= index3 && index3 <= realizedColumnsBlock.EndIndex)
        {
          flag1 = true;
          if (index3 == realizedColumnsBlock.EndIndex)
            ++index1;
        }
      }
      if (index2 < count1)
      {
        Microsoft.Windows.Controls.DataGridCell dataGridCell = children[index2] as Microsoft.Windows.Controls.DataGridCell;
        if (this.DataGridOwner.Columns[index3] == dataGridCell.Column)
        {
          flag2 = true;
          ++index2;
        }
      }
      bool flag3 = index1 == count2;
      bool flag4 = index2 == count1;
      if (flag2 != flag1 || flag3 != flag4)
        return true;
      if (flag3)
        break;
    }
    return false;
  }

  private static object OnCoerceHeight(DependencyObject d, object baseValue)
  {
    DataGridCellsPresenter baseObject = d as DataGridCellsPresenter;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, FrameworkElement.HeightProperty, (DependencyObject) baseObject.DataGridOwner, Microsoft.Windows.Controls.DataGrid.RowHeightProperty);
  }

  private static object OnCoerceMinHeight(DependencyObject d, object baseValue)
  {
    DataGridCellsPresenter baseObject = d as DataGridCellsPresenter;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, FrameworkElement.MinHeightProperty, (DependencyObject) baseObject.DataGridOwner, Microsoft.Windows.Controls.DataGrid.MinRowHeightProperty);
  }

  public object Item
  {
    get => this._item;
    internal set
    {
      if (this._item == value)
        return;
      object oldItem = this._item;
      this._item = value;
      this.OnItemChanged(oldItem, this._item);
    }
  }

  protected virtual void OnItemChanged(object oldItem, object newItem)
  {
    ObservableCollection<Microsoft.Windows.Controls.DataGridColumn> columns = this.Columns;
    if (columns == null)
      return;
    if (!(this.ItemsSource is Microsoft.Windows.Controls.MultipleCopiesCollection itemsSource))
      this.ItemsSource = (IEnumerable) new Microsoft.Windows.Controls.MultipleCopiesCollection(newItem, columns.Count);
    else
      itemsSource.CopiedItem = newItem;
  }

  protected override bool IsItemItsOwnContainerOverride(object item) => item is Microsoft.Windows.Controls.DataGridCell;

  internal bool IsItemItsOwnContainerInternal(object item)
  {
    return this.IsItemItsOwnContainerOverride(item);
  }

  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new Microsoft.Windows.Controls.DataGridCell();
  }

  protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
  {
    Microsoft.Windows.Controls.DataGridCell dataGridCell = (Microsoft.Windows.Controls.DataGridCell) element;
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    if (dataGridCell.RowOwner != dataGridRowOwner)
      dataGridCell.Tracker.StartTracking(ref this._cellTrackingRoot);
    dataGridCell.PrepareCell(item, (ItemsControl) this, dataGridRowOwner);
  }

  protected override void ClearContainerForItemOverride(DependencyObject element, object item)
  {
    Microsoft.Windows.Controls.DataGridCell dataGridCell = (Microsoft.Windows.Controls.DataGridCell) element;
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    if (dataGridCell.RowOwner == dataGridRowOwner)
      dataGridCell.Tracker.StopTracking(ref this._cellTrackingRoot);
    dataGridCell.ClearCell(dataGridRowOwner);
  }

  protected internal virtual void OnColumnsChanged(
    ObservableCollection<Microsoft.Windows.Controls.DataGridColumn> columns,
    NotifyCollectionChangedEventArgs e)
  {
    if (!(this.ItemsSource is Microsoft.Windows.Controls.MultipleCopiesCollection itemsSource))
      return;
    itemsSource.MirrorCollectionChange(e);
  }

  private static void OnNotifyHeightPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridCellsPresenter) d).NotifyPropertyChanged(d, e, NotificationTarget.CellsPresenter);
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
    if (Microsoft.Windows.Controls.DataGridHelper.ShouldNotifyCellsPresenter(target))
    {
      if (e.Property == Microsoft.Windows.Controls.DataGridColumn.WidthProperty || e.Property == Microsoft.Windows.Controls.DataGridColumn.DisplayIndexProperty)
      {
        if (((Microsoft.Windows.Controls.DataGridColumn) d).IsVisible)
          this.InvalidateDataGridCellsPanelMeasureAndArrange();
      }
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.FrozenColumnCountProperty || e.Property == Microsoft.Windows.Controls.DataGridColumn.VisibilityProperty || e.Property == Microsoft.Windows.Controls.DataGrid.CellsPanelHorizontalOffsetProperty || e.Property == Microsoft.Windows.Controls.DataGrid.HorizontalScrollOffsetProperty || string.Compare(propertyName, "ViewportWidth", StringComparison.Ordinal) == 0 || string.Compare(propertyName, "DelayedColumnWidthComputation", StringComparison.Ordinal) == 0)
        this.InvalidateDataGridCellsPanelMeasureAndArrange();
      else if (string.Compare(propertyName, "RealizedColumnsBlockListForNonVirtualizedRows", StringComparison.Ordinal) == 0)
        this.InvalidateDataGridCellsPanelMeasureAndArrange(false);
      else if (string.Compare(propertyName, "RealizedColumnsBlockListForVirtualizedRows", StringComparison.Ordinal) == 0)
        this.InvalidateDataGridCellsPanelMeasureAndArrange(true);
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.RowHeightProperty || e.Property == FrameworkElement.HeightProperty)
        Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.HeightProperty);
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.MinRowHeightProperty || e.Property == FrameworkElement.MinHeightProperty)
        Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.MinHeightProperty);
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.EnableColumnVirtualizationProperty)
        Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, VirtualizingStackPanel.IsVirtualizingProperty);
    }
    if (!Microsoft.Windows.Controls.DataGridHelper.ShouldNotifyCells(target) && !Microsoft.Windows.Controls.DataGridHelper.ShouldRefreshCellContent(target))
      return;
    for (Microsoft.Windows.Controls.ContainerTracking<Microsoft.Windows.Controls.DataGridCell> containerTracking = this._cellTrackingRoot; containerTracking != null; containerTracking = containerTracking.Next)
      containerTracking.Container.NotifyPropertyChanged(d, propertyName, e, target);
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    if (dataGridRowOwner == null)
      return base.MeasureOverride(availableSize);
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner.DataGridOwner;
    if (dataGridOwner == null)
      return base.MeasureOverride(availableSize);
    if (!Microsoft.Windows.Controls.DataGridHelper.IsGridLineVisible(dataGridOwner, true))
      return base.MeasureOverride(availableSize);
    double gridLineThickness = dataGridOwner.HorizontalGridLineThickness;
    Size size = base.MeasureOverride(Microsoft.Windows.Controls.DataGridHelper.SubtractFromSize(availableSize, gridLineThickness, true));
    size.Height += gridLineThickness;
    return size;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    if (dataGridRowOwner == null)
      return base.ArrangeOverride(finalSize);
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner.DataGridOwner;
    if (dataGridOwner == null)
      return base.ArrangeOverride(finalSize);
    if (!Microsoft.Windows.Controls.DataGridHelper.IsGridLineVisible(dataGridOwner, true))
      return base.ArrangeOverride(finalSize);
    double gridLineThickness = dataGridOwner.HorizontalGridLineThickness;
    Size size = base.ArrangeOverride(Microsoft.Windows.Controls.DataGridHelper.SubtractFromSize(finalSize, gridLineThickness, true));
    size.Height += gridLineThickness;
    return size;
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    base.OnRender(drawingContext);
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    if (dataGridRowOwner == null)
      return;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner.DataGridOwner;
    if (dataGridOwner == null || !Microsoft.Windows.Controls.DataGridHelper.IsGridLineVisible(dataGridOwner, true))
      return;
    double gridLineThickness = dataGridOwner.HorizontalGridLineThickness;
    drawingContext.DrawRectangle(dataGridOwner.HorizontalGridLinesBrush, (Pen) null, new Rect(new Size(this.RenderSize.Width, gridLineThickness))
    {
      Y = this.RenderSize.Height - gridLineThickness
    });
  }

  private static void OnIsVirtualizingPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridCellsPresenter d1 = (DataGridCellsPresenter) d;
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) d1, VirtualizingStackPanel.IsVirtualizingProperty);
    if (e.OldValue == d1.GetValue(VirtualizingStackPanel.IsVirtualizingProperty))
      return;
    d1.InvalidateDataGridCellsPanelMeasureAndArrange();
  }

  private static object OnCoerceIsVirtualizingProperty(DependencyObject d, object baseValue)
  {
    DataGridCellsPresenter baseObject = d as DataGridCellsPresenter;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, VirtualizingStackPanel.IsVirtualizingProperty, (DependencyObject) baseObject.DataGridOwner, Microsoft.Windows.Controls.DataGrid.EnableColumnVirtualizationProperty);
  }

  private void InvalidateDataGridCellsPanelMeasureAndArrange()
  {
    if (this._internalItemsHost == null)
      return;
    this._internalItemsHost.InvalidateMeasure();
    this._internalItemsHost.InvalidateArrange();
  }

  private void InvalidateDataGridCellsPanelMeasureAndArrange(bool withColumnVirtualization)
  {
    if (withColumnVirtualization != VirtualizingStackPanel.GetIsVirtualizing((DependencyObject) this))
      return;
    this.InvalidateDataGridCellsPanelMeasureAndArrange();
  }

  internal Panel InternalItemsHost
  {
    get => this._internalItemsHost;
    set => this._internalItemsHost = value;
  }

  internal void ScrollCellIntoView(int index)
  {
    if (!(this.InternalItemsHost is Microsoft.Windows.Controls.DataGridCellsPanel internalItemsHost))
      return;
    internalItemsHost.InternalBringIndexIntoView(index);
  }

  private Microsoft.Windows.Controls.DataGrid DataGridOwner => this.DataGridRowOwner?.DataGridOwner;

  internal Microsoft.Windows.Controls.DataGridRow DataGridRowOwner
  {
    get => Microsoft.Windows.Controls.DataGridHelper.FindParent<Microsoft.Windows.Controls.DataGridRow>((FrameworkElement) this);
  }

  private ObservableCollection<Microsoft.Windows.Controls.DataGridColumn> Columns
  {
    get => this.DataGridRowOwner?.DataGridOwner?.Columns;
  }

  internal Microsoft.Windows.Controls.ContainerTracking<Microsoft.Windows.Controls.DataGridCell> CellTrackingRoot
  {
    get => this._cellTrackingRoot;
  }
}
