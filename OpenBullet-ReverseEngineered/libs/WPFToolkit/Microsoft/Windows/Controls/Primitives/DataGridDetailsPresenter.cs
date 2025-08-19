// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.DataGridDetailsPresenter
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

public class DataGridDetailsPresenter : ContentPresenter
{
  static DataGridDetailsPresenter()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridDetailsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridDetailsPresenter)));
    ContentPresenter.ContentTemplateProperty.OverrideMetadata(typeof (DataGridDetailsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridDetailsPresenter.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridDetailsPresenter.OnCoerceContentTemplate)));
    ContentPresenter.ContentTemplateSelectorProperty.OverrideMetadata(typeof (DataGridDetailsPresenter), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridDetailsPresenter.OnNotifyPropertyChanged), new CoerceValueCallback(DataGridDetailsPresenter.OnCoerceContentTemplateSelector)));
    EventManager.RegisterClassHandler(typeof (DataGridDetailsPresenter), UIElement.MouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(DataGridDetailsPresenter.OnAnyMouseLeftButtonDownThunk), true);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DataGridDetailsPresenterAutomationPeer(this);
  }

  private static object OnCoerceContentTemplate(DependencyObject d, object baseValue)
  {
    DataGridDetailsPresenter baseObject = d as DataGridDetailsPresenter;
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = baseObject.DataGridRowOwner;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner?.DataGridOwner;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentPresenter.ContentTemplateProperty, (DependencyObject) dataGridRowOwner, Microsoft.Windows.Controls.DataGridRow.DetailsTemplateProperty, (DependencyObject) dataGridOwner, Microsoft.Windows.Controls.DataGrid.RowDetailsTemplateProperty);
  }

  private static object OnCoerceContentTemplateSelector(DependencyObject d, object baseValue)
  {
    DataGridDetailsPresenter baseObject = d as DataGridDetailsPresenter;
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = baseObject.DataGridRowOwner;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner?.DataGridOwner;
    return Microsoft.Windows.Controls.DataGridHelper.GetCoercedTransferPropertyValue((DependencyObject) baseObject, baseValue, ContentPresenter.ContentTemplateSelectorProperty, (DependencyObject) dataGridRowOwner, Microsoft.Windows.Controls.DataGridRow.DetailsTemplateSelectorProperty, (DependencyObject) dataGridOwner, Microsoft.Windows.Controls.DataGrid.RowDetailsTemplateSelectorProperty);
  }

  protected override void OnVisualParentChanged(DependencyObject oldParent)
  {
    base.OnVisualParentChanged(oldParent);
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    if (dataGridRowOwner == null)
      return;
    dataGridRowOwner.DetailsPresenter = this;
    this.SyncProperties();
  }

  private bool IsInVisualSubTree(DependencyObject visual)
  {
    for (; visual != null; visual = VisualTreeHelper.GetParent(visual))
    {
      if (visual == this)
        return true;
    }
    return false;
  }

  private static void OnAnyMouseLeftButtonDownThunk(object sender, MouseButtonEventArgs e)
  {
    ((DataGridDetailsPresenter) sender).OnAnyMouseLeftButtonDown(e);
  }

  private void OnAnyMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (!this.IsInVisualSubTree(e.OriginalSource as DependencyObject))
      return;
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner?.DataGridOwner;
    if (dataGridOwner == null || dataGridRowOwner == null)
      return;
    if (dataGridOwner.CurrentCell.Item != dataGridRowOwner.Item)
      dataGridOwner.ScrollIntoView(dataGridRowOwner.Item, dataGridOwner.ColumnFromDisplayIndex(0));
    dataGridOwner.HandleSelectionForRowHeaderAndDetailsInput(dataGridRowOwner, Mouse.Captured == null);
  }

  internal FrameworkElement DetailsElement
  {
    get
    {
      return VisualTreeHelper.GetChildrenCount((DependencyObject) this) > 0 ? VisualTreeHelper.GetChild((DependencyObject) this, 0) as FrameworkElement : (FrameworkElement) null;
    }
  }

  internal void SyncProperties()
  {
    this.Content = this.DataGridRowOwner?.Item;
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentPresenter.ContentTemplateProperty);
    Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentPresenter.ContentTemplateSelectorProperty);
  }

  private static void OnNotifyPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((DataGridDetailsPresenter) d).NotifyPropertyChanged(d, e);
  }

  internal void NotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (e.Property == Microsoft.Windows.Controls.DataGrid.RowDetailsTemplateProperty || e.Property == Microsoft.Windows.Controls.DataGridRow.DetailsTemplateProperty || e.Property == ContentPresenter.ContentTemplateProperty)
    {
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentPresenter.ContentTemplateProperty);
    }
    else
    {
      if (e.Property != Microsoft.Windows.Controls.DataGrid.RowDetailsTemplateSelectorProperty && e.Property != Microsoft.Windows.Controls.DataGridRow.DetailsTemplateSelectorProperty && e.Property != ContentPresenter.ContentTemplateSelectorProperty)
        return;
      Microsoft.Windows.Controls.DataGridHelper.TransferProperty((DependencyObject) this, ContentPresenter.ContentTemplateSelectorProperty);
    }
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner.DataGridOwner;
    if (!dataGridRowOwner.DetailsPresenterDrawsGridLines || !Microsoft.Windows.Controls.DataGridHelper.IsGridLineVisible(dataGridOwner, true))
      return base.MeasureOverride(availableSize);
    double gridLineThickness = dataGridOwner.HorizontalGridLineThickness;
    Size size = base.MeasureOverride(Microsoft.Windows.Controls.DataGridHelper.SubtractFromSize(availableSize, gridLineThickness, true));
    size.Height += gridLineThickness;
    return size;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    Microsoft.Windows.Controls.DataGridRow dataGridRowOwner = this.DataGridRowOwner;
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner.DataGridOwner;
    if (!dataGridRowOwner.DetailsPresenterDrawsGridLines || !Microsoft.Windows.Controls.DataGridHelper.IsGridLineVisible(dataGridOwner, true))
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
    Microsoft.Windows.Controls.DataGrid dataGridOwner = dataGridRowOwner.DataGridOwner;
    if (!dataGridRowOwner.DetailsPresenterDrawsGridLines || !Microsoft.Windows.Controls.DataGridHelper.IsGridLineVisible(dataGridOwner, true))
      return;
    double gridLineThickness = dataGridOwner.HorizontalGridLineThickness;
    drawingContext.DrawRectangle(dataGridOwner.HorizontalGridLinesBrush, (Pen) null, new Rect(new Size(this.RenderSize.Width, gridLineThickness))
    {
      Y = this.RenderSize.Height - gridLineThickness
    });
  }

  private Microsoft.Windows.Controls.DataGrid DataGridOwner => this.DataGridRowOwner?.DataGridOwner;

  internal Microsoft.Windows.Controls.DataGridRow DataGridRowOwner
  {
    get => Microsoft.Windows.Controls.DataGridHelper.FindParent<Microsoft.Windows.Controls.DataGridRow>((FrameworkElement) this);
  }
}
