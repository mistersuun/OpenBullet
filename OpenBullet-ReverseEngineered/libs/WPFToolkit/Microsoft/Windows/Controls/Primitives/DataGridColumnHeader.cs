// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.DataGridColumnHeader
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

[TemplatePart(Name = "PART_LeftHeaderGripper", Type = typeof (Thumb))]
[TemplatePart(Name = "PART_RightHeaderGripper", Type = typeof (Thumb))]
public class DataGridColumnHeader : ButtonBase, Microsoft.Windows.Controls.IProvideDataGridColumn
{
  private const string LeftHeaderGripperTemplateName = "PART_LeftHeaderGripper";
  private const string RightHeaderGripperTemplateName = "PART_RightHeaderGripper";
  public static readonly DependencyProperty SeparatorBrushProperty = DependencyProperty.Register(nameof (SeparatorBrush), typeof (Brush), typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty SeparatorVisibilityProperty = DependencyProperty.Register(nameof (SeparatorVisibility), typeof (Visibility), typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Visible));
  private static readonly DependencyPropertyKey DisplayIndexPropertyKey = DependencyProperty.RegisterReadOnly(nameof (DisplayIndex), typeof (int), typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1, new PropertyChangedCallback(DataGridColumnHeader.OnDisplayIndexChanged), new CoerceValueCallback(DataGridColumnHeader.OnCoerceDisplayIndex)));
  public static readonly DependencyProperty DisplayIndexProperty = DataGridColumnHeader.DisplayIndexPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey CanUserSortPropertyKey = DependencyProperty.RegisterReadOnly(nameof (CanUserSort), typeof (bool), typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, (PropertyChangedCallback) null, new CoerceValueCallback(DataGridColumnHeader.OnCoerceCanUserSort)));
  public static readonly DependencyProperty CanUserSortProperty = DataGridColumnHeader.CanUserSortPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey SortDirectionPropertyKey = DependencyProperty.RegisterReadOnly(nameof (SortDirection), typeof (ListSortDirection?), typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, (PropertyChangedCallback) null, new CoerceValueCallback(DataGridColumnHeader.OnCoerceSortDirection)));
  public static readonly DependencyProperty SortDirectionProperty = DataGridColumnHeader.SortDirectionPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsFrozenPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsFrozen), typeof (bool), typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, (PropertyChangedCallback) null, new CoerceValueCallback(DataGridColumnHeader.OnCoerceIsFrozen)));
  public static readonly DependencyProperty IsFrozenProperty = DataGridColumnHeader.IsFrozenPropertyKey.DependencyProperty;
  private Microsoft.Windows.Controls.DataGridColumn _column;
  private Microsoft.Windows.Controls.ContainerTracking<DataGridColumnHeader> _tracker;
  private DataGridColumnHeadersPresenter _parentPresenter;
  private Thumb _leftGripper;
  private Thumb _rightGripper;
  private bool _suppressClickEvent;

  static DataGridColumnHeader()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridColumnHeader)));
    ContentControl.ContentProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridColumnHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridColumnHeader.OnCoerceContent)));
    ContentControl.ContentTemplateProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumnHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridColumnHeader.OnCoerceContentTemplate)));
    ContentControl.ContentTemplateSelectorProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumnHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridColumnHeader.OnCoerceContentTemplateSelector)));
    ContentControl.ContentStringFormatProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumnHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridColumnHeader.OnCoerceStringFormat)));
    FrameworkElement.StyleProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(DataGridColumnHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridColumnHeader.OnCoerceStyle)));
    FrameworkElement.HeightProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridColumnHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridColumnHeader.OnCoerceHeight)));
    UIElement.FocusableProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    UIElement.ClipProperty.OverrideMetadata(typeof (DataGridColumnHeader), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(DataGridColumnHeader.OnCoerceClip)));
  }

  public DataGridColumnHeader()
  {
    this._tracker = new Microsoft.Windows.Controls.ContainerTracking<DataGridColumnHeader>(this);
  }

  public Microsoft.Windows.Controls.DataGridColumn Column => this._column;

  public Brush SeparatorBrush
  {
    get => (Brush) this.GetValue(DataGridColumnHeader.SeparatorBrushProperty);
    set => this.SetValue(DataGridColumnHeader.SeparatorBrushProperty, (object) value);
  }

  public Visibility SeparatorVisibility
  {
    get => (Visibility) this.GetValue(DataGridColumnHeader.SeparatorVisibilityProperty);
    set => this.SetValue(DataGridColumnHeader.SeparatorVisibilityProperty, (object) value);
  }

  internal void PrepareColumnHeader(object item, Microsoft.Windows.Controls.DataGridColumn column)
  {
    this._column = column;
    this.TabIndex = column.DisplayIndex;
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateSelectorProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentStringFormatProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.StyleProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.HeightProperty);
    this.CoerceValue(DataGridColumnHeader.CanUserSortProperty);
    this.CoerceValue(DataGridColumnHeader.SortDirectionProperty);
    this.CoerceValue(DataGridColumnHeader.IsFrozenProperty);
    this.CoerceValue(UIElement.ClipProperty);
    this.CoerceValue(DataGridColumnHeader.DisplayIndexProperty);
  }

  internal void ClearHeader() => this._column = (Microsoft.Windows.Controls.DataGridColumn) null;

  internal Microsoft.Windows.Controls.ContainerTracking<DataGridColumnHeader> Tracker
  {
    get => this._tracker;
  }

  public int DisplayIndex => (int) this.GetValue(DataGridColumnHeader.DisplayIndexProperty);

  private static object OnCoerceDisplayIndex(DependencyObject d, object baseValue)
  {
    Microsoft.Windows.Controls.DataGridColumn column = ((DataGridColumnHeader) d).Column;
    return column != null ? (object) column.DisplayIndex : (object) -1;
  }

  private static void OnDisplayIndexChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DataGridColumnHeader gridColumnHeader = (DataGridColumnHeader) d;
    Microsoft.Windows.Controls.DataGridColumn column = gridColumnHeader.Column;
    if (column == null)
      return;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = column.DataGridOwner;
    if (dataGridOwner == null)
      return;
    gridColumnHeader.SetLeftGripperVisibility();
    dataGridOwner.ColumnHeaderFromDisplayIndex(gridColumnHeader.DisplayIndex + 1)?.SetLeftGripperVisibility(column.CanUserResize);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.HookupGripperEvents();
  }

  private void HookupGripperEvents()
  {
    this.UnhookGripperEvents();
    this._leftGripper = this.GetTemplateChild("PART_LeftHeaderGripper") as Thumb;
    this._rightGripper = this.GetTemplateChild("PART_RightHeaderGripper") as Thumb;
    if (this._leftGripper != null)
    {
      this._leftGripper.DragStarted += new DragStartedEventHandler(this.OnColumnHeaderGripperDragStarted);
      this._leftGripper.DragDelta += new DragDeltaEventHandler(this.OnColumnHeaderResize);
      this._leftGripper.DragCompleted += new DragCompletedEventHandler(this.OnColumnHeaderGripperDragCompleted);
      this._leftGripper.MouseDoubleClick += new MouseButtonEventHandler(this.OnGripperDoubleClicked);
      this.SetLeftGripperVisibility();
    }
    if (this._rightGripper == null)
      return;
    this._rightGripper.DragStarted += new DragStartedEventHandler(this.OnColumnHeaderGripperDragStarted);
    this._rightGripper.DragDelta += new DragDeltaEventHandler(this.OnColumnHeaderResize);
    this._rightGripper.DragCompleted += new DragCompletedEventHandler(this.OnColumnHeaderGripperDragCompleted);
    this._rightGripper.MouseDoubleClick += new MouseButtonEventHandler(this.OnGripperDoubleClicked);
    this.SetRightGripperVisibility();
  }

  private void UnhookGripperEvents()
  {
    if (this._leftGripper != null)
    {
      this._leftGripper.DragStarted -= new DragStartedEventHandler(this.OnColumnHeaderGripperDragStarted);
      this._leftGripper.DragDelta -= new DragDeltaEventHandler(this.OnColumnHeaderResize);
      this._leftGripper.DragCompleted -= new DragCompletedEventHandler(this.OnColumnHeaderGripperDragCompleted);
      this._leftGripper.MouseDoubleClick -= new MouseButtonEventHandler(this.OnGripperDoubleClicked);
      this._leftGripper = (Thumb) null;
    }
    if (this._rightGripper == null)
      return;
    this._rightGripper.DragStarted -= new DragStartedEventHandler(this.OnColumnHeaderGripperDragStarted);
    this._rightGripper.DragDelta -= new DragDeltaEventHandler(this.OnColumnHeaderResize);
    this._rightGripper.DragCompleted -= new DragCompletedEventHandler(this.OnColumnHeaderGripperDragCompleted);
    this._rightGripper.MouseDoubleClick -= new MouseButtonEventHandler(this.OnGripperDoubleClicked);
    this._rightGripper = (Thumb) null;
  }

  private DataGridColumnHeader HeaderToResize(object gripper)
  {
    return gripper != this._rightGripper ? this.PreviousVisibleHeader : this;
  }

  private void OnColumnHeaderGripperDragStarted(object sender, DragStartedEventArgs e)
  {
    DataGridColumnHeader resize = this.HeaderToResize(sender);
    if (resize == null)
      return;
    if (resize.Column != null)
      resize.Column.DataGridOwner?.InternalColumns.OnColumnResizeStarted();
    e.Handled = true;
  }

  private void OnColumnHeaderResize(object sender, DragDeltaEventArgs e)
  {
    DataGridColumnHeader resize = this.HeaderToResize(sender);
    if (resize == null)
      return;
    DataGridColumnHeader.RecomputeColumnWidthsOnColumnResize(resize, e.HorizontalChange);
    e.Handled = true;
  }

  private static void RecomputeColumnWidthsOnColumnResize(
    DataGridColumnHeader header,
    double horizontalChange)
  {
    Microsoft.Windows.Controls.DataGridColumn column = header.Column;
    column?.DataGridOwner?.InternalColumns.RecomputeColumnWidthsOnColumnResize(column, horizontalChange, false);
  }

  private void OnColumnHeaderGripperDragCompleted(object sender, DragCompletedEventArgs e)
  {
    DataGridColumnHeader resize = this.HeaderToResize(sender);
    if (resize == null)
      return;
    if (resize.Column != null)
      resize.Column.DataGridOwner?.InternalColumns.OnColumnResizeCompleted(e.Canceled);
    e.Handled = true;
  }

  private void OnGripperDoubleClicked(object sender, MouseButtonEventArgs e)
  {
    DataGridColumnHeader resize = this.HeaderToResize(sender);
    if (resize == null || resize.Column == null)
      return;
    resize.Column.Width = Microsoft.Windows.Controls.DataGridLength.Auto;
    e.Handled = true;
  }

  private Microsoft.Windows.Controls.DataGridLength ColumnWidth
  {
    get => this.Column == null ? Microsoft.Windows.Controls.DataGridLength.Auto : this.Column.Width;
  }

  private double ColumnActualWidth
  {
    get => this.Column == null ? this.ActualWidth : this.Column.ActualWidth;
  }

  private static void OnNotifyPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridColumnHeader) d).NotifyPropertyChanged(d, e);
  }

  internal void NotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is Microsoft.Windows.Controls.DataGridColumn dataGridColumn && dataGridColumn != this.Column)
      return;
    if (e.Property == Microsoft.Windows.Controls.DataGridColumn.WidthProperty)
      Microsoft.Windows.Controls.DataGridHelper.OnColumnWidthChanged((Microsoft.Windows.Controls.IProvideDataGridColumn) this, e);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.HeaderProperty || e.Property == ContentControl.ContentProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.HeaderTemplateProperty || e.Property == ContentControl.ContentTemplateProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.HeaderTemplateSelectorProperty || e.Property == ContentControl.ContentTemplateSelectorProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateSelectorProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.HeaderStringFormatProperty || e.Property == ContentControl.ContentStringFormatProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentStringFormatProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.ColumnHeaderStyleProperty || e.Property == Microsoft.Windows.Controls.DataGridColumn.HeaderStyleProperty || e.Property == FrameworkElement.StyleProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.StyleProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.ColumnHeaderHeightProperty || e.Property == FrameworkElement.HeightProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.HeightProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.DisplayIndexProperty)
    {
      this.CoerceValue(DataGridColumnHeader.DisplayIndexProperty);
      this.TabIndex = dataGridColumn.DisplayIndex;
    }
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.CanUserResizeColumnsProperty)
      this.OnCanUserResizeColumnsChanged();
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.CanUserSortProperty)
      this.CoerceValue(DataGridColumnHeader.CanUserSortProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.SortDirectionProperty)
      this.CoerceValue(DataGridColumnHeader.SortDirectionProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.IsFrozenProperty)
      this.CoerceValue(DataGridColumnHeader.IsFrozenProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridColumn.CanUserResizeProperty)
    {
      this.OnCanUserResizeChanged();
    }
    else
    {
      if (e.Property != Microsoft.Windows.Controls.DataGridColumn.VisibilityProperty)
        return;
      this.OnColumnVisibilityChanged(e);
    }
  }

  private void OnCanUserResizeColumnsChanged()
  {
    if (this.Column.DataGridOwner == null)
      return;
    this.SetLeftGripperVisibility();
    this.SetRightGripperVisibility();
  }

  private void OnCanUserResizeChanged()
  {
    if (this.Column.DataGridOwner == null)
      return;
    this.SetNextHeaderLeftGripperVisibility(this.Column.CanUserResize);
    this.SetRightGripperVisibility();
  }

  private void SetLeftGripperVisibility()
  {
    if (this._leftGripper == null || this.Column == null)
      return;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.Column.DataGridOwner;
    bool canPreviousColumnResize = false;
    for (int displayIndex = this.DisplayIndex - 1; displayIndex >= 0; --displayIndex)
    {
      Microsoft.Windows.Controls.DataGridColumn dataGridColumn = dataGridOwner.ColumnFromDisplayIndex(displayIndex);
      if (dataGridColumn.IsVisible)
      {
        canPreviousColumnResize = dataGridColumn.CanUserResize;
        break;
      }
    }
    this.SetLeftGripperVisibility(canPreviousColumnResize);
  }

  private void SetLeftGripperVisibility(bool canPreviousColumnResize)
  {
    if (this._leftGripper == null || this.Column == null)
      return;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.Column.DataGridOwner;
    if (dataGridOwner != null && dataGridOwner.CanUserResizeColumns && canPreviousColumnResize)
      this._leftGripper.Visibility = Visibility.Visible;
    else
      this._leftGripper.Visibility = Visibility.Collapsed;
  }

  private void SetRightGripperVisibility()
  {
    if (this._rightGripper == null || this.Column == null)
      return;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.Column.DataGridOwner;
    if (dataGridOwner != null && dataGridOwner.CanUserResizeColumns && this.Column.CanUserResize)
      this._rightGripper.Visibility = Visibility.Visible;
    else
      this._rightGripper.Visibility = Visibility.Collapsed;
  }

  private void SetNextHeaderLeftGripperVisibility(bool canUserResize)
  {
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.Column.DataGridOwner;
    int count = dataGridOwner.Columns.Count;
    for (int displayIndex = this.DisplayIndex + 1; displayIndex < count; ++displayIndex)
    {
      if (dataGridOwner.ColumnFromDisplayIndex(displayIndex).IsVisible)
      {
        dataGridOwner.ColumnHeaderFromDisplayIndex(displayIndex)?.SetLeftGripperVisibility(canUserResize);
        break;
      }
    }
  }

  private void OnColumnVisibilityChanged(DependencyPropertyChangedEventArgs e)
  {
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.Column.DataGridOwner;
    if (dataGridOwner == null)
      return;
    bool flag1 = (Visibility) e.OldValue == Visibility.Visible;
    bool flag2 = (Visibility) e.NewValue == Visibility.Visible;
    if (flag1 == flag2)
      return;
    if (flag2)
    {
      this.SetLeftGripperVisibility();
      this.SetRightGripperVisibility();
      this.SetNextHeaderLeftGripperVisibility(this.Column.CanUserResize);
    }
    else
    {
      bool canUserResize = false;
      for (int displayIndex = this.DisplayIndex - 1; displayIndex >= 0; --displayIndex)
      {
        Microsoft.Windows.Controls.DataGridColumn dataGridColumn = dataGridOwner.ColumnFromDisplayIndex(displayIndex);
        if (dataGridColumn.IsVisible)
        {
          canUserResize = dataGridColumn.CanUserResize;
          break;
        }
      }
      this.SetNextHeaderLeftGripperVisibility(canUserResize);
    }
  }

  private static object OnCoerceContent(DependencyObject d, object baseValue)
  {
    DataGridColumnHeader baseObject = d as DataGridColumnHeader;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentControl.ContentProperty, (DependencyObject) baseObject.Column, Microsoft.Windows.Controls.DataGridColumn.HeaderProperty);
  }

  private static object OnCoerceContentTemplate(DependencyObject d, object baseValue)
  {
    DataGridColumnHeader baseObject = d as DataGridColumnHeader;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentControl.ContentTemplateProperty, (DependencyObject) baseObject.Column, Microsoft.Windows.Controls.DataGridColumn.HeaderTemplateProperty);
  }

  private static object OnCoerceContentTemplateSelector(DependencyObject d, object baseValue)
  {
    DataGridColumnHeader baseObject = d as DataGridColumnHeader;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentControl.ContentTemplateSelectorProperty, (DependencyObject) baseObject.Column, Microsoft.Windows.Controls.DataGridColumn.HeaderTemplateSelectorProperty);
  }

  private static object OnCoerceStringFormat(DependencyObject d, object baseValue)
  {
    DataGridColumnHeader baseObject = d as DataGridColumnHeader;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentControl.ContentStringFormatProperty, (DependencyObject) baseObject.Column, Microsoft.Windows.Controls.DataGridColumn.HeaderStringFormatProperty);
  }

  private static object OnCoerceStyle(DependencyObject d, object baseValue)
  {
    DataGridColumnHeader baseObject = (DataGridColumnHeader) d;
    Microsoft.Windows.Controls.DataGridColumn column = baseObject.Column;
    Microsoft.Windows.Controls.DataGrid grandParentObject = (Microsoft.Windows.Controls.DataGrid) null;
    if (column == null)
    {
      if (baseObject.TemplatedParent is DataGridColumnHeadersPresenter templatedParent)
        grandParentObject = templatedParent.ParentDataGrid;
    }
    else
      grandParentObject = column.DataGridOwner;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, FrameworkElement.StyleProperty, (DependencyObject) column, Microsoft.Windows.Controls.DataGridColumn.HeaderStyleProperty, (DependencyObject) grandParentObject, Microsoft.Windows.Controls.DataGrid.ColumnHeaderStyleProperty);
  }

  public bool CanUserSort => (bool) this.GetValue(DataGridColumnHeader.CanUserSortProperty);

  public ListSortDirection? SortDirection
  {
    get => (ListSortDirection?) this.GetValue(DataGridColumnHeader.SortDirectionProperty);
  }

  protected override void OnClick()
  {
    if (this.SuppressClickEvent)
      return;
    if (AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
      UIElementAutomationPeer.CreatePeerForElement((UIElement) this)?.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
    base.OnClick();
    if (this.Column == null || this.Column.DataGridOwner == null)
      return;
    this.Column.DataGridOwner.PerformSort(this.Column);
  }

  private static object OnCoerceHeight(DependencyObject d, object baseValue)
  {
    DataGridColumnHeader baseObject = (DataGridColumnHeader) d;
    Microsoft.Windows.Controls.DataGridColumn column = baseObject.Column;
    Microsoft.Windows.Controls.DataGrid parentObject = (Microsoft.Windows.Controls.DataGrid) null;
    if (column == null)
    {
      if (baseObject.TemplatedParent is DataGridColumnHeadersPresenter templatedParent)
        parentObject = templatedParent.ParentDataGrid;
    }
    else
      parentObject = column.DataGridOwner;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, FrameworkElement.HeightProperty, (DependencyObject) parentObject, Microsoft.Windows.Controls.DataGrid.ColumnHeaderHeightProperty);
  }

  private static object OnCoerceCanUserSort(DependencyObject d, object baseValue)
  {
    Microsoft.Windows.Controls.DataGridColumn column = ((DataGridColumnHeader) d).Column;
    return column != null ? (object) column.CanUserSort : baseValue;
  }

  private static object OnCoerceSortDirection(DependencyObject d, object baseValue)
  {
    Microsoft.Windows.Controls.DataGridColumn column = ((DataGridColumnHeader) d).Column;
    return column != null ? (object) column.SortDirection : baseValue;
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer(this);
  }

  internal void Invoke() => this.OnClick();

  public bool IsFrozen => (bool) this.GetValue(DataGridColumnHeader.IsFrozenProperty);

  private static object OnCoerceIsFrozen(DependencyObject d, object baseValue)
  {
    Microsoft.Windows.Controls.DataGridColumn column = ((DataGridColumnHeader) d).Column;
    return column != null ? (object) column.IsFrozen : baseValue;
  }

  private static object OnCoerceClip(DependencyObject d, object baseValue)
  {
    DataGridColumnHeader cell = (DataGridColumnHeader) d;
    Geometry geometry1 = baseValue as Geometry;
    Geometry frozenClipForCell = Microsoft.Windows.Controls.DataGridHelper.GetFrozenClipForCell((Microsoft.Windows.Controls.IProvideDataGridColumn) cell);
    if (frozenClipForCell != null)
    {
      if (geometry1 == null)
        return (object) frozenClipForCell;
      geometry1 = (Geometry) new CombinedGeometry(GeometryCombineMode.Intersect, geometry1, frozenClipForCell);
    }
    return (object) geometry1;
  }

  internal DataGridColumnHeadersPresenter ParentPresenter
  {
    get
    {
      if (this._parentPresenter == null)
        this._parentPresenter = ItemsControl.ItemsControlFromItemContainer((DependencyObject) this) as DataGridColumnHeadersPresenter;
      return this._parentPresenter;
    }
  }

  internal bool SuppressClickEvent
  {
    get => this._suppressClickEvent;
    set => this._suppressClickEvent = value;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonDown(e);
    if (this.ClickMode == ClickMode.Hover && e.ButtonState == MouseButtonState.Pressed)
      this.CaptureMouse();
    this.ParentPresenter.OnHeaderMouseLeftButtonDown(e);
    e.Handled = true;
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    base.OnMouseMove(e);
    this.ParentPresenter.OnHeaderMouseMove(e);
    e.Handled = true;
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonUp(e);
    if (this.ClickMode == ClickMode.Hover && this.IsMouseCaptured)
      this.ReleaseMouseCapture();
    this.ParentPresenter.OnHeaderMouseLeftButtonUp(e);
    e.Handled = true;
  }

  protected override void OnLostMouseCapture(MouseEventArgs e)
  {
    base.OnLostMouseCapture(e);
    this.ParentPresenter.OnHeaderLostMouseCapture(e);
    e.Handled = true;
  }

  Microsoft.Windows.Controls.DataGridColumn Microsoft.Windows.Controls.IProvideDataGridColumn.Column
  {
    get => this._column;
  }

  private Panel ParentPanel => this.VisualParent as Panel;

  private DataGridColumnHeader PreviousVisibleHeader
  {
    get
    {
      Microsoft.Windows.Controls.DataGridColumn column = this.Column;
      if (column != null)
      {
        Microsoft.Windows.Controls.DataGrid dataGridOwner = column.DataGridOwner;
        if (dataGridOwner != null)
        {
          for (int displayIndex = this.DisplayIndex - 1; displayIndex >= 0; --displayIndex)
          {
            if (dataGridOwner.ColumnFromDisplayIndex(displayIndex).IsVisible)
              return dataGridOwner.ColumnHeaderFromDisplayIndex(displayIndex);
          }
        }
      }
      return (DataGridColumnHeader) null;
    }
  }
}
