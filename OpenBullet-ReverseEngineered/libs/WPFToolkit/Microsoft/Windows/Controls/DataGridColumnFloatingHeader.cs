// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridColumnFloatingHeader
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using MS.Internal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls;

[TemplatePart(Name = "PART_VisualBrushCanvas", Type = typeof (Canvas))]
internal class DataGridColumnFloatingHeader : Control
{
  private const string VisualBrushCanvasTemplateName = "PART_VisualBrushCanvas";
  private DataGridColumnHeader _referenceHeader;
  private Canvas _visualBrushCanvas;

  static DataGridColumnFloatingHeader()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DataGridColumnFloatingHeader), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DataGridColumnFloatingHeader)));
    FrameworkElement.WidthProperty.OverrideMetadata(typeof (DataGridColumnFloatingHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridColumnFloatingHeader.OnWidthChanged), new CoerceValueCallback(DataGridColumnFloatingHeader.OnCoerceWidth)));
    FrameworkElement.HeightProperty.OverrideMetadata(typeof (DataGridColumnFloatingHeader), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(DataGridColumnFloatingHeader.OnHeightChanged), new CoerceValueCallback(DataGridColumnFloatingHeader.OnCoerceHeight)));
  }

  private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    DataGridColumnFloatingHeader columnFloatingHeader = (DataGridColumnFloatingHeader) d;
    double newValue = (double) e.NewValue;
    if (columnFloatingHeader._visualBrushCanvas == null || DoubleUtil.IsNaN(newValue) || !(columnFloatingHeader._visualBrushCanvas.Background is VisualBrush background))
      return;
    Rect viewbox = background.Viewbox;
    background.Viewbox = new Rect(viewbox.X, viewbox.Y, newValue - columnFloatingHeader.GetVisualCanvasMarginX(), viewbox.Height);
  }

  private static object OnCoerceWidth(DependencyObject d, object baseValue)
  {
    double num = (double) baseValue;
    DataGridColumnFloatingHeader columnFloatingHeader = (DataGridColumnFloatingHeader) d;
    return columnFloatingHeader._referenceHeader != null && DoubleUtil.IsNaN(num) ? (object) (columnFloatingHeader._referenceHeader.ActualWidth + columnFloatingHeader.GetVisualCanvasMarginX()) : baseValue;
  }

  private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    DataGridColumnFloatingHeader columnFloatingHeader = (DataGridColumnFloatingHeader) d;
    double newValue = (double) e.NewValue;
    if (columnFloatingHeader._visualBrushCanvas == null || DoubleUtil.IsNaN(newValue) || !(columnFloatingHeader._visualBrushCanvas.Background is VisualBrush background))
      return;
    Rect viewbox = background.Viewbox;
    background.Viewbox = new Rect(viewbox.X, viewbox.Y, viewbox.Width, newValue - columnFloatingHeader.GetVisualCanvasMarginY());
  }

  private static object OnCoerceHeight(DependencyObject d, object baseValue)
  {
    double num = (double) baseValue;
    DataGridColumnFloatingHeader columnFloatingHeader = (DataGridColumnFloatingHeader) d;
    return columnFloatingHeader._referenceHeader != null && DoubleUtil.IsNaN(num) ? (object) (columnFloatingHeader._referenceHeader.ActualHeight + columnFloatingHeader.GetVisualCanvasMarginY()) : baseValue;
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._visualBrushCanvas = this.GetTemplateChild("PART_VisualBrushCanvas") as Canvas;
    this.UpdateVisualBrush();
  }

  internal DataGridColumnHeader ReferenceHeader
  {
    get => this._referenceHeader;
    set => this._referenceHeader = value;
  }

  private void UpdateVisualBrush()
  {
    if (this._referenceHeader == null || this._visualBrushCanvas == null)
      return;
    VisualBrush visualBrush = new VisualBrush((Visual) this._referenceHeader);
    visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
    double width1 = this.Width;
    double width2 = !DoubleUtil.IsNaN(width1) ? width1 - this.GetVisualCanvasMarginX() : this._referenceHeader.ActualWidth;
    double height1 = this.Height;
    double height2 = !DoubleUtil.IsNaN(height1) ? height1 - this.GetVisualCanvasMarginY() : this._referenceHeader.ActualHeight;
    Vector offset = VisualTreeHelper.GetOffset((Visual) this._referenceHeader);
    visualBrush.Viewbox = new Rect(offset.X, offset.Y, width2, height2);
    this._visualBrushCanvas.Background = (Brush) visualBrush;
  }

  internal void ClearHeader()
  {
    this._referenceHeader = (DataGridColumnHeader) null;
    if (this._visualBrushCanvas == null)
      return;
    this._visualBrushCanvas.Background = (Brush) null;
  }

  private double GetVisualCanvasMarginX()
  {
    double visualCanvasMarginX = 0.0;
    if (this._visualBrushCanvas != null)
    {
      Thickness margin = this._visualBrushCanvas.Margin;
      visualCanvasMarginX = visualCanvasMarginX + margin.Left + margin.Right;
    }
    return visualCanvasMarginX;
  }

  private double GetVisualCanvasMarginY()
  {
    double visualCanvasMarginY = 0.0;
    if (this._visualBrushCanvas != null)
    {
      Thickness margin = this._visualBrushCanvas.Margin;
      visualCanvasMarginY = visualCanvasMarginY + margin.Top + margin.Bottom;
    }
    return visualCanvasMarginY;
  }
}
