// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

[TemplatePart(Name = "PART_FillerColumnHeader", Type = typeof (DataGridColumnHeader))]
public class DataGridColumnHeadersPresenter : ItemsControl
{
  private const string ElementFillerColumnHeader = "PART_FillerColumnHeader";
  private Microsoft.Windows.Controls.ContainerTracking<DataGridColumnHeader> _headerTrackingRoot;
  private Microsoft.Windows.Controls.DataGrid _parentDataGrid;
  private bool _prepareColumnHeaderDragging;
  private bool _isColumnHeaderDragging;
  private DataGridColumnHeader _draggingSrcColumnHeader;
  private Point _columnHeaderDragStartPosition;
  private Point _columnHeaderDragStartRelativePosition;
  private Point _columnHeaderDragCurrentPosition;
  private Control _columnHeaderDropLocationIndicator;
  private Control _columnHeaderDragIndicator;
  private Panel _internalItemsHost;

  static DataGridColumnHeadersPresenter()
  {
    Type type = typeof (DataGridColumnHeadersPresenter);
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) type));
    UIElement.FocusableProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    FrameworkElementFactory root = new FrameworkElementFactory(typeof (Microsoft.Windows.Controls.DataGridCellsPanel));
    ItemsControl.ItemsPanelProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) new ItemsPanelTemplate(root)));
    VirtualizingStackPanel.IsVirtualizingProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(DataGridColumnHeadersPresenter.OnIsVirtualizingPropertyChanged), new CoerceValueCallback(DataGridColumnHeadersPresenter.OnCoerceIsVirtualizingProperty)));
    VirtualizingStackPanel.VirtualizationModeProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) VirtualizationMode.Recycling));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    Microsoft.Windows.Controls.DataGrid parentDataGrid = this.ParentDataGrid;
    if (parentDataGrid != null)
    {
      this.ItemsSource = (IEnumerable) new ColumnHeaderCollection(parentDataGrid.Columns);
      parentDataGrid.ColumnHeadersPresenter = this;
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, VirtualizingStackPanel.IsVirtualizingProperty);
      if (!(this.GetTemplateChild("PART_FillerColumnHeader") is DataGridColumnHeader templateChild))
        return;
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) templateChild, FrameworkElement.StyleProperty);
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) templateChild, FrameworkElement.HeightProperty);
    }
    else
      this.ItemsSource = (IEnumerable) null;
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DataGridColumnHeadersPresenterAutomationPeer(this);
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    Size size1 = availableSize with
    {
      Width = double.PositiveInfinity
    };
    Size size2 = base.MeasureOverride(size1);
    Size desiredSize;
    if (this._columnHeaderDragIndicator != null && this._isColumnHeaderDragging)
    {
      this._columnHeaderDragIndicator.Measure(size1);
      desiredSize = this._columnHeaderDragIndicator.DesiredSize;
      size2.Width = Math.Max(size2.Width, desiredSize.Width);
      size2.Height = Math.Max(size2.Height, desiredSize.Height);
    }
    if (this._columnHeaderDropLocationIndicator != null && this._isColumnHeaderDragging)
    {
      this._columnHeaderDropLocationIndicator.Measure(availableSize);
      desiredSize = this._columnHeaderDropLocationIndicator.DesiredSize;
      size2.Width = Math.Max(size2.Width, desiredSize.Width);
      size2.Height = Math.Max(size2.Height, desiredSize.Height);
    }
    size2.Width = Math.Min(availableSize.Width, size2.Width);
    return size2;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    UIElement child = VisualTreeHelper.GetChildrenCount((DependencyObject) this) > 0 ? VisualTreeHelper.GetChild((DependencyObject) this, 0) as UIElement : (UIElement) null;
    if (child != null)
    {
      Rect finalRect = new Rect(finalSize);
      Microsoft.Windows.Controls.DataGrid parentDataGrid = this.ParentDataGrid;
      if (parentDataGrid != null)
      {
        finalRect.X = -parentDataGrid.HorizontalScrollOffset;
        finalRect.Width = Math.Max(finalSize.Width, parentDataGrid.CellsPanelActualWidth);
      }
      child.Arrange(finalRect);
    }
    if (this._columnHeaderDragIndicator != null && this._isColumnHeaderDragging)
      this._columnHeaderDragIndicator.Arrange(new Rect(new Point(this._columnHeaderDragCurrentPosition.X - this._columnHeaderDragStartRelativePosition.X, 0.0), new Size(this._columnHeaderDragIndicator.Width, this._columnHeaderDragIndicator.Height)));
    if (this._columnHeaderDropLocationIndicator != null && this._isColumnHeaderDragging)
    {
      Point byCurrentPosition = this.FindColumnHeaderPositionByCurrentPosition(this._columnHeaderDragCurrentPosition, true);
      double width = this._columnHeaderDropLocationIndicator.Width;
      byCurrentPosition.X -= width * 0.5;
      this._columnHeaderDropLocationIndicator.Arrange(new Rect(byCurrentPosition, new Size(width, this._columnHeaderDropLocationIndicator.Height)));
    }
    return finalSize;
  }

  protected override Geometry GetLayoutClip(Size layoutSlotSize)
  {
    RectangleGeometry layoutClip = new RectangleGeometry(new Rect(this.RenderSize));
    layoutClip.Freeze();
    return (Geometry) layoutClip;
  }

  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new DataGridColumnHeader();
  }

  protected override bool IsItemItsOwnContainerOverride(object item)
  {
    return item is DataGridColumnHeader;
  }

  internal bool IsItemItsOwnContainerInternal(object item)
  {
    return this.IsItemItsOwnContainerOverride(item);
  }

  protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
  {
    if (!(element is DataGridColumnHeader container))
      return;
    Microsoft.Windows.Controls.DataGridColumn column = this.ColumnFromContainer(container);
    if (container.Column == null)
      container.Tracker.StartTracking(ref this._headerTrackingRoot);
    container.PrepareColumnHeader(item, column);
  }

  protected override void ClearContainerForItemOverride(DependencyObject element, object item)
  {
    DataGridColumnHeader gridColumnHeader = element as DataGridColumnHeader;
    base.ClearContainerForItemOverride(element, item);
    if (gridColumnHeader == null)
      return;
    gridColumnHeader.Tracker.StopTracking(ref this._headerTrackingRoot);
    gridColumnHeader.ClearHeader();
  }

  private Microsoft.Windows.Controls.DataGridColumn ColumnFromContainer(
    DataGridColumnHeader container)
  {
    return this.HeaderCollection.ColumnFromIndex(this.ItemContainerGenerator.IndexFromContainer((DependencyObject) container));
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
    Microsoft.Windows.Controls.DataGridColumn column = d as Microsoft.Windows.Controls.DataGridColumn;
    if (Microsoft.Windows.Controls.DataGridHelper.ShouldNotifyColumnHeadersPresenter(target))
    {
      if (e.Property == Microsoft.Windows.Controls.DataGridColumn.WidthProperty || e.Property == Microsoft.Windows.Controls.DataGridColumn.DisplayIndexProperty)
      {
        if (column.IsVisible)
          this.InvalidateDataGridCellsPanelMeasureAndArrange();
      }
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.FrozenColumnCountProperty || e.Property == Microsoft.Windows.Controls.DataGridColumn.VisibilityProperty || e.Property == Microsoft.Windows.Controls.DataGrid.CellsPanelHorizontalOffsetProperty || string.Compare(propertyName, "ViewportWidth", StringComparison.Ordinal) == 0 || string.Compare(propertyName, "DelayedColumnWidthComputation", StringComparison.Ordinal) == 0)
        this.InvalidateDataGridCellsPanelMeasureAndArrange();
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.HorizontalScrollOffsetProperty)
      {
        this.InvalidateArrange();
        this.InvalidateDataGridCellsPanelMeasureAndArrange();
      }
      else if (string.Compare(propertyName, "RealizedColumnsBlockListForNonVirtualizedRows", StringComparison.Ordinal) == 0)
        this.InvalidateDataGridCellsPanelMeasureAndArrange(false);
      else if (string.Compare(propertyName, "RealizedColumnsBlockListForVirtualizedRows", StringComparison.Ordinal) == 0)
        this.InvalidateDataGridCellsPanelMeasureAndArrange(true);
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.CellsPanelActualWidthProperty)
        this.InvalidateArrange();
      else if (e.Property == Microsoft.Windows.Controls.DataGrid.EnableColumnVirtualizationProperty)
        Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, VirtualizingStackPanel.IsVirtualizingProperty);
    }
    if (!Microsoft.Windows.Controls.DataGridHelper.ShouldNotifyColumnHeaders(target))
      return;
    if (e.Property == Microsoft.Windows.Controls.DataGridColumn.HeaderProperty)
    {
      if (this.HeaderCollection == null)
        return;
      this.HeaderCollection.NotifyHeaderPropertyChanged(column, e);
    }
    else
    {
      for (Microsoft.Windows.Controls.ContainerTracking<DataGridColumnHeader> containerTracking = this._headerTrackingRoot; containerTracking != null; containerTracking = containerTracking.Next)
        containerTracking.Container.NotifyPropertyChanged(d, e);
      if (!(d is Microsoft.Windows.Controls.DataGrid) || e.Property != Microsoft.Windows.Controls.DataGrid.ColumnHeaderStyleProperty && e.Property != Microsoft.Windows.Controls.DataGrid.ColumnHeaderHeightProperty || !(this.GetTemplateChild("PART_FillerColumnHeader") is DataGridColumnHeader templateChild))
        return;
      templateChild.NotifyPropertyChanged(d, e);
    }
  }

  private static void OnIsVirtualizingPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridColumnHeadersPresenter d1 = (DataGridColumnHeadersPresenter) d;
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) d1, VirtualizingStackPanel.IsVirtualizingProperty);
    if (e.OldValue == d1.GetValue(VirtualizingStackPanel.IsVirtualizingProperty))
      return;
    d1.InvalidateDataGridCellsPanelMeasureAndArrange();
  }

  private static object OnCoerceIsVirtualizingProperty(DependencyObject d, object baseValue)
  {
    DataGridColumnHeadersPresenter baseObject = d as DataGridColumnHeadersPresenter;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, VirtualizingStackPanel.IsVirtualizingProperty, (DependencyObject) baseObject.ParentDataGrid, Microsoft.Windows.Controls.DataGrid.EnableColumnVirtualizationProperty);
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

  protected override int VisualChildrenCount
  {
    get
    {
      int visualChildrenCount = base.VisualChildrenCount;
      if (this._columnHeaderDragIndicator != null)
        ++visualChildrenCount;
      if (this._columnHeaderDropLocationIndicator != null)
        ++visualChildrenCount;
      return visualChildrenCount;
    }
  }

  protected override Visual GetVisualChild(int index)
  {
    int visualChildrenCount = base.VisualChildrenCount;
    if (index == visualChildrenCount)
    {
      if (this._columnHeaderDragIndicator != null)
        return (Visual) this._columnHeaderDragIndicator;
      if (this._columnHeaderDropLocationIndicator != null)
        return (Visual) this._columnHeaderDropLocationIndicator;
    }
    return index == visualChildrenCount + 1 && this._columnHeaderDragIndicator != null && this._columnHeaderDropLocationIndicator != null ? (Visual) this._columnHeaderDropLocationIndicator : base.GetVisualChild(index);
  }

  internal void OnHeaderMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (this.ParentDataGrid == null)
      return;
    if (this._columnHeaderDragIndicator != null)
    {
      this.RemoveVisualChild((Visual) this._columnHeaderDragIndicator);
      this._columnHeaderDragIndicator = (Control) null;
    }
    if (this._columnHeaderDropLocationIndicator != null)
    {
      this.RemoveVisualChild((Visual) this._columnHeaderDropLocationIndicator);
      this._columnHeaderDropLocationIndicator = (Control) null;
    }
    DataGridColumnHeader headerByPosition = this.FindColumnHeaderByPosition(e.GetPosition((IInputElement) this));
    if (headerByPosition != null)
    {
      Microsoft.Windows.Controls.DataGridColumn column = headerByPosition.Column;
      if (!this.ParentDataGrid.CanUserReorderColumns || !column.CanUserReorder)
        return;
      this.PrepareColumnHeaderDrag(headerByPosition, e.GetPosition((IInputElement) this), e.GetPosition((IInputElement) headerByPosition));
    }
    else
    {
      this._isColumnHeaderDragging = false;
      this._prepareColumnHeaderDragging = false;
      this._draggingSrcColumnHeader = (DataGridColumnHeader) null;
      this.InvalidateArrange();
    }
  }

  internal void OnHeaderMouseMove(MouseEventArgs e)
  {
    if (e.LeftButton != MouseButtonState.Pressed || !this._prepareColumnHeaderDragging)
      return;
    this._columnHeaderDragCurrentPosition = e.GetPosition((IInputElement) this);
    if (!this._isColumnHeaderDragging)
    {
      if (!DataGridColumnHeadersPresenter.CheckStartColumnHeaderDrag(this._columnHeaderDragCurrentPosition, this._columnHeaderDragStartPosition))
        return;
      this.StartColumnHeaderDrag();
    }
    else
    {
      Visibility visibility = this.IsMousePositionValidForColumnDrag(2.0) ? Visibility.Visible : Visibility.Collapsed;
      if (this._columnHeaderDragIndicator != null)
        this._columnHeaderDragIndicator.Visibility = visibility;
      if (this._columnHeaderDropLocationIndicator != null)
        this._columnHeaderDropLocationIndicator.Visibility = visibility;
      this.InvalidateArrange();
      DragDeltaEventArgs e1 = new DragDeltaEventArgs(this._columnHeaderDragCurrentPosition.X - this._columnHeaderDragStartPosition.X, this._columnHeaderDragCurrentPosition.Y - this._columnHeaderDragStartPosition.Y);
      this._columnHeaderDragStartPosition = this._columnHeaderDragCurrentPosition;
      this.ParentDataGrid.OnColumnHeaderDragDelta(e1);
    }
  }

  internal void OnHeaderMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (this._isColumnHeaderDragging)
    {
      this._columnHeaderDragCurrentPosition = e.GetPosition((IInputElement) this);
      this.FinishColumnHeaderDrag(false);
    }
    else
      this.ClearColumnHeaderDragInfo();
  }

  internal void OnHeaderLostMouseCapture(MouseEventArgs e)
  {
    if (!this._isColumnHeaderDragging || Mouse.LeftButton != MouseButtonState.Pressed)
      return;
    this.FinishColumnHeaderDrag(true);
  }

  private void ClearColumnHeaderDragInfo()
  {
    this._isColumnHeaderDragging = false;
    this._prepareColumnHeaderDragging = false;
    this._draggingSrcColumnHeader = (DataGridColumnHeader) null;
    if (this._columnHeaderDragIndicator != null)
    {
      this.RemoveVisualChild((Visual) this._columnHeaderDragIndicator);
      this._columnHeaderDragIndicator = (Control) null;
    }
    if (this._columnHeaderDropLocationIndicator == null)
      return;
    this.RemoveVisualChild((Visual) this._columnHeaderDropLocationIndicator);
    this._columnHeaderDropLocationIndicator = (Control) null;
  }

  private void PrepareColumnHeaderDrag(DataGridColumnHeader header, Point pos, Point relativePos)
  {
    this._prepareColumnHeaderDragging = true;
    this._isColumnHeaderDragging = false;
    this._draggingSrcColumnHeader = header;
    this._columnHeaderDragStartPosition = pos;
    this._columnHeaderDragStartRelativePosition = relativePos;
  }

  private static bool CheckStartColumnHeaderDrag(Point currentPos, Point originalPos)
  {
    return DoubleUtil.GreaterThan(Math.Abs(currentPos.X - originalPos.X), SystemParameters.MinimumHorizontalDragDistance);
  }

  private bool IsMousePositionValidForColumnDrag(double dragFactor)
  {
    int nearestDisplayIndex = -1;
    return this.IsMousePositionValidForColumnDrag(dragFactor, out nearestDisplayIndex);
  }

  private bool IsMousePositionValidForColumnDrag(double dragFactor, out int nearestDisplayIndex)
  {
    nearestDisplayIndex = -1;
    bool flag = false;
    if (this._draggingSrcColumnHeader.Column != null)
      flag = this._draggingSrcColumnHeader.Column.IsFrozen;
    int num1 = 0;
    if (this.ParentDataGrid != null)
      num1 = this.ParentDataGrid.FrozenColumnCount;
    nearestDisplayIndex = this.FindDisplayIndexByPosition(this._columnHeaderDragCurrentPosition, true);
    if (flag && nearestDisplayIndex >= num1 || !flag && nearestDisplayIndex < num1)
      return false;
    double num2 = this._columnHeaderDragIndicator != null ? Math.Max(this._draggingSrcColumnHeader.RenderSize.Height, this._columnHeaderDragIndicator.Height) : this._draggingSrcColumnHeader.RenderSize.Height;
    return DoubleUtil.LessThanOrClose(-num2 * dragFactor, this._columnHeaderDragCurrentPosition.Y) && DoubleUtil.LessThanOrClose(this._columnHeaderDragCurrentPosition.Y, num2 * (dragFactor + 1.0));
  }

  private void StartColumnHeaderDrag()
  {
    this._columnHeaderDragStartPosition = this._columnHeaderDragCurrentPosition;
    this.ParentDataGrid.OnColumnHeaderDragStarted(new DragStartedEventArgs(this._columnHeaderDragStartPosition.X, this._columnHeaderDragStartPosition.Y));
    Microsoft.Windows.Controls.DataGridColumnReorderingEventArgs e = new Microsoft.Windows.Controls.DataGridColumnReorderingEventArgs(this._draggingSrcColumnHeader.Column);
    this._columnHeaderDragIndicator = this.CreateColumnHeaderDragIndicator();
    this._columnHeaderDropLocationIndicator = this.CreateColumnHeaderDropIndicator();
    e.DragIndicator = this._columnHeaderDragIndicator;
    e.DropLocationIndicator = this._columnHeaderDropLocationIndicator;
    this.ParentDataGrid.OnColumnReordering(e);
    if (!e.Cancel)
    {
      this._isColumnHeaderDragging = true;
      this._columnHeaderDragIndicator = e.DragIndicator;
      this._columnHeaderDropLocationIndicator = e.DropLocationIndicator;
      if (this._columnHeaderDragIndicator != null)
      {
        this.SetDefaultsOnDragIndicator();
        this.AddVisualChild((Visual) this._columnHeaderDragIndicator);
      }
      if (this._columnHeaderDropLocationIndicator != null)
      {
        this.SetDefaultsOnDropIndicator();
        this.AddVisualChild((Visual) this._columnHeaderDropLocationIndicator);
      }
      this._draggingSrcColumnHeader.SuppressClickEvent = true;
      this.InvalidateMeasure();
    }
    else
      this.FinishColumnHeaderDrag(true);
  }

  private Control CreateColumnHeaderDragIndicator()
  {
    return (Control) new Microsoft.Windows.Controls.DataGridColumnFloatingHeader()
    {
      ReferenceHeader = this._draggingSrcColumnHeader
    };
  }

  private void SetDefaultsOnDragIndicator()
  {
    Microsoft.Windows.Controls.DataGridColumn column = this._draggingSrcColumnHeader.Column;
    Style style = (Style) null;
    if (column != null)
      style = column.DragIndicatorStyle;
    this._columnHeaderDragIndicator.Style = style;
    this._columnHeaderDragIndicator.CoerceValue(FrameworkElement.WidthProperty);
    this._columnHeaderDragIndicator.CoerceValue(FrameworkElement.HeightProperty);
  }

  private Control CreateColumnHeaderDropIndicator()
  {
    return (Control) new Microsoft.Windows.Controls.DataGridColumnDropSeparator()
    {
      ReferenceHeader = this._draggingSrcColumnHeader
    };
  }

  private void SetDefaultsOnDropIndicator()
  {
    Style style = (Style) null;
    if (this.ParentDataGrid != null)
      style = this.ParentDataGrid.DropLocationIndicatorStyle;
    this._columnHeaderDropLocationIndicator.Style = style;
    this._columnHeaderDropLocationIndicator.CoerceValue(FrameworkElement.WidthProperty);
    this._columnHeaderDropLocationIndicator.CoerceValue(FrameworkElement.HeightProperty);
  }

  private void FinishColumnHeaderDrag(bool isCancel)
  {
    this._prepareColumnHeaderDragging = false;
    this._isColumnHeaderDragging = false;
    this._draggingSrcColumnHeader.SuppressClickEvent = false;
    if (this._columnHeaderDragIndicator != null)
    {
      this._columnHeaderDragIndicator.Visibility = Visibility.Collapsed;
      if (this._columnHeaderDragIndicator is Microsoft.Windows.Controls.DataGridColumnFloatingHeader headerDragIndicator)
        headerDragIndicator.ClearHeader();
      this.RemoveVisualChild((Visual) this._columnHeaderDragIndicator);
    }
    if (this._columnHeaderDropLocationIndicator != null)
    {
      this._columnHeaderDropLocationIndicator.Visibility = Visibility.Collapsed;
      if (this._columnHeaderDropLocationIndicator is Microsoft.Windows.Controls.DataGridColumnDropSeparator locationIndicator)
        locationIndicator.ReferenceHeader = (DataGridColumnHeader) null;
      this.RemoveVisualChild((Visual) this._columnHeaderDropLocationIndicator);
    }
    this.ParentDataGrid.OnColumnHeaderDragCompleted(new DragCompletedEventArgs(this._columnHeaderDragCurrentPosition.X - this._columnHeaderDragStartPosition.X, this._columnHeaderDragCurrentPosition.Y - this._columnHeaderDragStartPosition.Y, isCancel));
    this._draggingSrcColumnHeader.InvalidateArrange();
    if (!isCancel)
    {
      int nearestDisplayIndex = -1;
      bool flag = this.IsMousePositionValidForColumnDrag(2.0, out nearestDisplayIndex);
      Microsoft.Windows.Controls.DataGridColumn column = this._draggingSrcColumnHeader.Column;
      if (column != null && flag && nearestDisplayIndex != column.DisplayIndex)
      {
        column.DisplayIndex = nearestDisplayIndex;
        this.ParentDataGrid.OnColumnReordered(new Microsoft.Windows.Controls.DataGridColumnEventArgs(this._draggingSrcColumnHeader.Column));
      }
    }
    this._draggingSrcColumnHeader = (DataGridColumnHeader) null;
    this._columnHeaderDragIndicator = (Control) null;
    this._columnHeaderDropLocationIndicator = (Control) null;
  }

  private int FindDisplayIndexByPosition(Point startPos, bool findNearestColumn)
  {
    int displayIndex;
    this.FindDisplayIndexAndHeaderPosition(startPos, findNearestColumn, out displayIndex, out Point _, out DataGridColumnHeader _);
    return displayIndex;
  }

  private DataGridColumnHeader FindColumnHeaderByPosition(Point startPos)
  {
    DataGridColumnHeader header;
    this.FindDisplayIndexAndHeaderPosition(startPos, false, out int _, out Point _, out header);
    return header;
  }

  private Point FindColumnHeaderPositionByCurrentPosition(Point startPos, bool findNearestColumn)
  {
    Point headerPos;
    this.FindDisplayIndexAndHeaderPosition(startPos, findNearestColumn, out int _, out headerPos, out DataGridColumnHeader _);
    return headerPos;
  }

  private static double GetColumnEstimatedWidth(Microsoft.Windows.Controls.DataGridColumn column, double averageColumnWidth)
  {
    double columnEstimatedWidth = column.Width.DisplayValue;
    if (DoubleUtil.IsNaN(columnEstimatedWidth))
      columnEstimatedWidth = Math.Min(Math.Max(averageColumnWidth, column.MinWidth), column.MaxWidth);
    return columnEstimatedWidth;
  }

  private void FindDisplayIndexAndHeaderPosition(
    Point startPos,
    bool findNearestColumn,
    out int displayIndex,
    out Point headerPos,
    out DataGridColumnHeader header)
  {
    Point point = new Point(0.0, 0.0);
    headerPos = point;
    displayIndex = -1;
    header = (DataGridColumnHeader) null;
    if (startPos.X < 0.0)
    {
      if (!findNearestColumn)
        return;
      displayIndex = 0;
    }
    else
    {
      double num1 = 0.0;
      double num2 = 0.0;
      Microsoft.Windows.Controls.DataGrid parentDataGrid = this.ParentDataGrid;
      double averageColumnWidth = parentDataGrid.InternalColumns.AverageColumnWidth;
      bool flag = false;
      int displayIndex1;
      for (displayIndex1 = 0; displayIndex1 < parentDataGrid.Columns.Count; ++displayIndex1)
      {
        ++displayIndex;
        DataGridColumnHeader gridColumnHeader = parentDataGrid.ColumnHeaderFromDisplayIndex(displayIndex1);
        if (gridColumnHeader == null)
        {
          Microsoft.Windows.Controls.DataGridColumn column = parentDataGrid.ColumnFromDisplayIndex(displayIndex1);
          if (column.IsVisible)
          {
            num1 = num2;
            if (displayIndex1 >= parentDataGrid.FrozenColumnCount && !flag)
            {
              num1 -= parentDataGrid.HorizontalScrollOffset;
              flag = true;
            }
            num2 = num1 + DataGridColumnHeadersPresenter.GetColumnEstimatedWidth(column, averageColumnWidth);
          }
          else
            continue;
        }
        else
        {
          num1 = gridColumnHeader.TransformToAncestor((Visual) this).Transform(point).X;
          num2 = num1 + gridColumnHeader.RenderSize.Width;
        }
        if (!DoubleUtil.LessThanOrClose(startPos.X, num1))
        {
          if (DoubleUtil.GreaterThanOrClose(startPos.X, num1) && DoubleUtil.LessThanOrClose(startPos.X, num2))
          {
            if (findNearestColumn)
            {
              double num3 = (num1 + num2) * 0.5;
              if (DoubleUtil.GreaterThanOrClose(startPos.X, num3))
              {
                num1 = num2;
                ++displayIndex;
              }
              if (this._draggingSrcColumnHeader != null && this._draggingSrcColumnHeader.Column != null && this._draggingSrcColumnHeader.Column.DisplayIndex < displayIndex)
              {
                --displayIndex;
                break;
              }
              break;
            }
            header = gridColumnHeader;
            break;
          }
        }
        else
          break;
      }
      if (displayIndex1 == parentDataGrid.Columns.Count)
      {
        displayIndex = parentDataGrid.Columns.Count - 1;
        num1 = num2;
      }
      headerPos.X = num1;
    }
  }

  private ColumnHeaderCollection HeaderCollection => this.ItemsSource as ColumnHeaderCollection;

  internal Microsoft.Windows.Controls.DataGrid ParentDataGrid
  {
    get
    {
      if (this._parentDataGrid == null)
        this._parentDataGrid = Microsoft.Windows.Controls.DataGridHelper.FindParent<Microsoft.Windows.Controls.DataGrid>((FrameworkElement) this);
      return this._parentDataGrid;
    }
  }

  internal Microsoft.Windows.Controls.ContainerTracking<DataGridColumnHeader> HeaderTrackingRoot
  {
    get => this._headerTrackingRoot;
  }
}
