// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Folding.FoldingMarginMarker
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Folding;

internal sealed class FoldingMarginMarker : UIElement
{
  private const double MarginSizeFactor = 0.7;
  internal VisualLine VisualLine;
  internal FoldingSection FoldingSection;
  private bool isExpanded;

  public bool IsExpanded
  {
    get => this.isExpanded;
    set
    {
      if (this.isExpanded != value)
      {
        this.isExpanded = value;
        this.InvalidateVisual();
      }
      if (this.FoldingSection == null)
        return;
      this.FoldingSection.IsFolded = !value;
    }
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    base.OnMouseDown(e);
    if (e.Handled || e.ChangedButton != MouseButton.Left)
      return;
    this.IsExpanded = !this.IsExpanded;
    e.Handled = true;
  }

  protected override Size MeasureCore(Size availableSize)
  {
    double odd = PixelSnapHelpers.RoundToOdd(14.0 / 15.0 * (double) this.GetValue(TextBlock.FontSizeProperty), PixelSnapHelpers.GetPixelSize((Visual) this).Width);
    return new Size(odd, odd);
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    FoldingMargin visualParent = this.VisualParent as FoldingMargin;
    Pen pen1 = new Pen(visualParent.SelectedFoldingMarkerBrush, 1.0);
    Pen pen2 = new Pen(visualParent.FoldingMarkerBrush, 1.0);
    pen1.StartLineCap = pen2.StartLineCap = PenLineCap.Square;
    pen1.EndLineCap = pen2.EndLineCap = PenLineCap.Square;
    Size pixelSize = PixelSnapHelpers.GetPixelSize((Visual) this);
    Rect rectangle = new Rect(pixelSize.Width / 2.0, pixelSize.Height / 2.0, this.RenderSize.Width - pixelSize.Width, this.RenderSize.Height - pixelSize.Height);
    drawingContext.DrawRectangle(this.IsMouseDirectlyOver ? visualParent.SelectedFoldingMarkerBackgroundBrush : visualParent.FoldingMarkerBackgroundBrush, this.IsMouseDirectlyOver ? pen1 : pen2, rectangle);
    double x = rectangle.Left + rectangle.Width / 2.0;
    double y = rectangle.Top + rectangle.Height / 2.0;
    double num = PixelSnapHelpers.Round(rectangle.Width / 8.0, pixelSize.Width) + pixelSize.Width;
    drawingContext.DrawLine(pen1, new Point(rectangle.Left + num, y), new Point(rectangle.Right - num, y));
    if (this.isExpanded)
      return;
    drawingContext.DrawLine(pen1, new Point(x, rectangle.Top + num), new Point(x, rectangle.Bottom - num));
  }

  protected override void OnIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnIsMouseDirectlyOverChanged(e);
    this.InvalidateVisual();
  }
}
