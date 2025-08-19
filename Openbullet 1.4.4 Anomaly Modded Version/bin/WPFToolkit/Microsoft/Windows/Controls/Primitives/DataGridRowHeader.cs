// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.DataGridRowHeader
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

[TemplatePart(Name = "PART_TopHeaderGripper", Type = typeof (Thumb))]
[TemplatePart(Name = "PART_BottomHeaderGripper", Type = typeof (Thumb))]
public class DataGridRowHeader : ButtonBase
{
  private const string TopHeaderGripperTemplateName = "PART_TopHeaderGripper";
  private const string BottomHeaderGripperTemplateName = "PART_BottomHeaderGripper";
  public static readonly DependencyProperty SeparatorBrushProperty = DependencyProperty.Register(nameof (SeparatorBrush), typeof (Brush), typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty SeparatorVisibilityProperty = DependencyProperty.Register(nameof (SeparatorVisibility), typeof (Visibility), typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Visible));
  private static readonly DependencyPropertyKey IsRowSelectedPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsRowSelected), typeof (bool), typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, (PropertyChangedCallback) null, new CoerceValueCallback(DataGridRowHeader.OnCoerceIsRowSelected)));
  public static readonly DependencyProperty IsRowSelectedProperty = DataGridRowHeader.IsRowSelectedPropertyKey.DependencyProperty;
  private Thumb _topGripper;
  private Thumb _bottomGripper;

  static DataGridRowHeader()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridRowHeader)));
    ContentControl.ContentProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridRowHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridRowHeader.OnCoerceContent)));
    ContentControl.ContentTemplateProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridRowHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridRowHeader.OnCoerceContentTemplate)));
    ContentControl.ContentTemplateSelectorProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridRowHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridRowHeader.OnCoerceContentTemplateSelector)));
    FrameworkElement.StyleProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridRowHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridRowHeader.OnCoerceStyle)));
    FrameworkElement.WidthProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridRowHeader.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridRowHeader.OnCoerceWidth)));
    ButtonBase.ClickModeProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) ClickMode.Press));
    UIElement.FocusableProperty.OverrideMetadata(typeof (DataGridRowHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DataGridRowHeaderAutomationPeer(this);
  }

  public Brush SeparatorBrush
  {
    get => (Brush) this.GetValue(DataGridRowHeader.SeparatorBrushProperty);
    set => this.SetValue(DataGridRowHeader.SeparatorBrushProperty, (object) value);
  }

  public Visibility SeparatorVisibility
  {
    get => (Visibility) this.GetValue(DataGridRowHeader.SeparatorVisibilityProperty);
    set => this.SetValue(DataGridRowHeader.SeparatorVisibilityProperty, (object) value);
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    Size size = base.MeasureOverride(availableSize);
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.DataGridOwner;
    if (dataGridOwner == null)
      return size;
    if (DoubleUtil.IsNaN(dataGridOwner.RowHeaderWidth) && size.Width > dataGridOwner.RowHeaderActualWidth)
      dataGridOwner.RowHeaderActualWidth = size.Width;
    return new Size(dataGridOwner.RowHeaderActualWidth, size.Height);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    Microsoft.Windows.Controls.DataGridRow parentRow = this.ParentRow;
    if (parentRow != null)
    {
      parentRow.RowHeader = this;
      this.SyncProperties();
    }
    this.HookupGripperEvents();
  }

  internal void SyncProperties()
  {
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.StyleProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateSelectorProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.WidthProperty);
    this.CoerceValue(DataGridRowHeader.IsRowSelectedProperty);
    this.OnCanUserResizeRowsChanged();
  }

  private static void OnNotifyPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridRowHeader) d).NotifyPropertyChanged(d, e);
  }

  internal void NotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (e.Property == Microsoft.Windows.Controls.DataGridRow.HeaderProperty || e.Property == ContentControl.ContentProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.RowHeaderStyleProperty || e.Property == Microsoft.Windows.Controls.DataGridRow.HeaderStyleProperty || e.Property == FrameworkElement.StyleProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.StyleProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.RowHeaderTemplateProperty || e.Property == Microsoft.Windows.Controls.DataGridRow.HeaderTemplateProperty || e.Property == ContentControl.ContentTemplateProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.RowHeaderTemplateSelectorProperty || e.Property == Microsoft.Windows.Controls.DataGridRow.HeaderTemplateSelectorProperty || e.Property == ContentControl.ContentTemplateSelectorProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentControl.ContentTemplateSelectorProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.RowHeaderWidthProperty || e.Property == FrameworkElement.WidthProperty)
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, FrameworkElement.WidthProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGridRow.IsSelectedProperty)
      this.CoerceValue(DataGridRowHeader.IsRowSelectedProperty);
    else if (e.Property == Microsoft.Windows.Controls.DataGrid.CanUserResizeRowsProperty)
    {
      this.OnCanUserResizeRowsChanged();
    }
    else
    {
      if (e.Property != Microsoft.Windows.Controls.DataGrid.RowHeaderActualWidthProperty)
        return;
      this.InvalidateMeasure();
      this.InvalidateArrange();
      if (!(this.Parent is UIElement parent))
        return;
      parent.InvalidateMeasure();
      parent.InvalidateArrange();
    }
  }

  private static object OnCoerceContent(DependencyObject d, object baseValue)
  {
    DataGridRowHeader baseObject = d as DataGridRowHeader;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentControl.ContentProperty, (DependencyObject) baseObject.ParentRow, Microsoft.Windows.Controls.DataGridRow.HeaderProperty);
  }

  private static object OnCoerceContentTemplate(DependencyObject d, object baseValue)
  {
    DataGridRowHeader baseObject = d as DataGridRowHeader;
    Microsoft.Windows.Controls.DataGridRow parentRow = baseObject.ParentRow;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = parentRow?.DataGridOwner;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentControl.ContentTemplateProperty, (DependencyObject) parentRow, Microsoft.Windows.Controls.DataGridRow.HeaderTemplateProperty, (DependencyObject) dataGridOwner, Microsoft.Windows.Controls.DataGrid.RowHeaderTemplateProperty);
  }

  private static object OnCoerceContentTemplateSelector(DependencyObject d, object baseValue)
  {
    DataGridRowHeader baseObject = d as DataGridRowHeader;
    Microsoft.Windows.Controls.DataGridRow parentRow = baseObject.ParentRow;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = parentRow?.DataGridOwner;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentControl.ContentTemplateSelectorProperty, (DependencyObject) parentRow, Microsoft.Windows.Controls.DataGridRow.HeaderTemplateSelectorProperty, (DependencyObject) dataGridOwner, Microsoft.Windows.Controls.DataGrid.RowHeaderTemplateSelectorProperty);
  }

  private static object OnCoerceStyle(DependencyObject d, object baseValue)
  {
    DataGridRowHeader baseObject = d as DataGridRowHeader;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, FrameworkElement.StyleProperty, (DependencyObject) baseObject.ParentRow, Microsoft.Windows.Controls.DataGridRow.HeaderStyleProperty, (DependencyObject) baseObject.DataGridOwner, Microsoft.Windows.Controls.DataGrid.RowHeaderStyleProperty);
  }

  private static object OnCoerceWidth(DependencyObject d, object baseValue)
  {
    DataGridRowHeader baseObject = d as DataGridRowHeader;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, FrameworkElement.WidthProperty, (DependencyObject) baseObject.DataGridOwner, Microsoft.Windows.Controls.DataGrid.RowHeaderWidthProperty);
  }

  [Bindable(true)]
  [Category("Appearance")]
  public bool IsRowSelected => (bool) this.GetValue(DataGridRowHeader.IsRowSelectedProperty);

  private static object OnCoerceIsRowSelected(DependencyObject d, object baseValue)
  {
    Microsoft.Windows.Controls.DataGridRow parentRow = ((DataGridRowHeader) d).ParentRow;
    return parentRow != null ? (object) parentRow.IsSelected : baseValue;
  }

  protected override void OnClick()
  {
    base.OnClick();
    if (Mouse.Captured == this)
      this.ReleaseMouseCapture();
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.DataGridOwner;
    Microsoft.Windows.Controls.DataGridRow parentRow = this.ParentRow;
    if (dataGridOwner == null || parentRow == null)
      return;
    dataGridOwner.HandleSelectionForRowHeaderAndDetailsInput(parentRow, true);
  }

  private void HookupGripperEvents()
  {
    this.UnhookGripperEvents();
    this._topGripper = this.GetTemplateChild("PART_TopHeaderGripper") as Thumb;
    this._bottomGripper = this.GetTemplateChild("PART_BottomHeaderGripper") as Thumb;
    if (this._topGripper != null)
    {
      this._topGripper.DragStarted += new DragStartedEventHandler(this.OnRowHeaderGripperDragStarted);
      this._topGripper.DragDelta += new DragDeltaEventHandler(this.OnRowHeaderResize);
      this._topGripper.DragCompleted += new DragCompletedEventHandler(this.OnRowHeaderGripperDragCompleted);
      this._topGripper.MouseDoubleClick += new MouseButtonEventHandler(this.OnGripperDoubleClicked);
      this.SetTopGripperVisibility();
    }
    if (this._bottomGripper == null)
      return;
    this._bottomGripper.DragStarted += new DragStartedEventHandler(this.OnRowHeaderGripperDragStarted);
    this._bottomGripper.DragDelta += new DragDeltaEventHandler(this.OnRowHeaderResize);
    this._bottomGripper.DragCompleted += new DragCompletedEventHandler(this.OnRowHeaderGripperDragCompleted);
    this._bottomGripper.MouseDoubleClick += new MouseButtonEventHandler(this.OnGripperDoubleClicked);
    this.SetBottomGripperVisibility();
  }

  private void UnhookGripperEvents()
  {
    if (this._topGripper != null)
    {
      this._topGripper.DragStarted -= new DragStartedEventHandler(this.OnRowHeaderGripperDragStarted);
      this._topGripper.DragDelta -= new DragDeltaEventHandler(this.OnRowHeaderResize);
      this._topGripper.DragCompleted -= new DragCompletedEventHandler(this.OnRowHeaderGripperDragCompleted);
      this._topGripper.MouseDoubleClick -= new MouseButtonEventHandler(this.OnGripperDoubleClicked);
      this._topGripper = (Thumb) null;
    }
    if (this._bottomGripper == null)
      return;
    this._bottomGripper.DragStarted -= new DragStartedEventHandler(this.OnRowHeaderGripperDragStarted);
    this._bottomGripper.DragDelta -= new DragDeltaEventHandler(this.OnRowHeaderResize);
    this._bottomGripper.DragCompleted -= new DragCompletedEventHandler(this.OnRowHeaderGripperDragCompleted);
    this._bottomGripper.MouseDoubleClick -= new MouseButtonEventHandler(this.OnGripperDoubleClicked);
    this._bottomGripper = (Thumb) null;
  }

  private void SetTopGripperVisibility()
  {
    if (this._topGripper == null)
      return;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.DataGridOwner;
    Microsoft.Windows.Controls.DataGridRow parentRow = this.ParentRow;
    if (dataGridOwner != null && parentRow != null && dataGridOwner.CanUserResizeRows && dataGridOwner.Items.Count > 1 && !object.ReferenceEquals(parentRow.Item, dataGridOwner.Items[0]))
      this._topGripper.Visibility = Visibility.Visible;
    else
      this._topGripper.Visibility = Visibility.Collapsed;
  }

  private void SetBottomGripperVisibility()
  {
    if (this._bottomGripper == null)
      return;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = this.DataGridOwner;
    if (dataGridOwner != null && dataGridOwner.CanUserResizeRows)
      this._bottomGripper.Visibility = Visibility.Visible;
    else
      this._bottomGripper.Visibility = Visibility.Collapsed;
  }

  private Microsoft.Windows.Controls.DataGridRow PreviousRow
  {
    get
    {
      Microsoft.Windows.Controls.DataGridRow parentRow = this.ParentRow;
      if (parentRow != null)
      {
        Microsoft.Windows.Controls.DataGrid dataGridOwner = parentRow.DataGridOwner;
        if (dataGridOwner != null)
        {
          int num = dataGridOwner.ItemContainerGenerator.IndexFromContainer((DependencyObject) parentRow);
          if (num > 0)
            return (Microsoft.Windows.Controls.DataGridRow) dataGridOwner.ItemContainerGenerator.ContainerFromIndex(num - 1);
        }
      }
      return (Microsoft.Windows.Controls.DataGridRow) null;
    }
  }

  private Microsoft.Windows.Controls.DataGridRow RowToResize(object gripper)
  {
    return gripper != this._bottomGripper ? this.PreviousRow : this.ParentRow;
  }

  private void OnRowHeaderGripperDragStarted(object sender, DragStartedEventArgs e)
  {
    Microsoft.Windows.Controls.DataGridRow resize = this.RowToResize(sender);
    if (resize == null)
      return;
    resize.OnRowResizeStarted();
    e.Handled = true;
  }

  private void OnRowHeaderResize(object sender, DragDeltaEventArgs e)
  {
    Microsoft.Windows.Controls.DataGridRow resize = this.RowToResize(sender);
    if (resize == null)
      return;
    resize.OnRowResize(e.VerticalChange);
    e.Handled = true;
  }

  private void OnRowHeaderGripperDragCompleted(object sender, DragCompletedEventArgs e)
  {
    Microsoft.Windows.Controls.DataGridRow resize = this.RowToResize(sender);
    if (resize == null)
      return;
    resize.OnRowResizeCompleted(e.Canceled);
    e.Handled = true;
  }

  private void OnGripperDoubleClicked(object sender, MouseButtonEventArgs e)
  {
    Microsoft.Windows.Controls.DataGridRow resize = this.RowToResize(sender);
    if (resize == null)
      return;
    resize.OnRowResizeReset();
    e.Handled = true;
  }

  private void OnCanUserResizeRowsChanged()
  {
    this.SetTopGripperVisibility();
    this.SetBottomGripperVisibility();
  }

  internal Microsoft.Windows.Controls.DataGridRow ParentRow
  {
    get => Microsoft.Windows.Controls.DataGridHelper.FindParent<Microsoft.Windows.Controls.DataGridRow>((FrameworkElement) this);
  }

  private Microsoft.Windows.Controls.DataGrid DataGridOwner => this.ParentRow?.DataGridOwner;
}
