// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.ColumnRulerRenderer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class ColumnRulerRenderer : IBackgroundRenderer
{
  private Pen pen;
  private int column;
  private TextView textView;
  public static readonly Color DefaultForeground = Colors.LightGray;

  public ColumnRulerRenderer(TextView textView)
  {
    if (textView == null)
      throw new ArgumentNullException(nameof (textView));
    this.pen = new Pen((Brush) new SolidColorBrush(ColumnRulerRenderer.DefaultForeground), 1.0);
    this.pen.Freeze();
    this.textView = textView;
    this.textView.BackgroundRenderers.Add((IBackgroundRenderer) this);
  }

  public KnownLayer Layer => KnownLayer.Background;

  public void SetRuler(int column, Pen pen)
  {
    if (this.column != column)
    {
      this.column = column;
      this.textView.InvalidateLayer(this.Layer);
    }
    if (this.pen == pen)
      return;
    this.pen = pen;
    this.textView.InvalidateLayer(this.Layer);
  }

  public void Draw(TextView textView, DrawingContext drawingContext)
  {
    if (this.column < 1)
      return;
    double x = PixelSnapHelpers.PixelAlign(textView.WideSpaceWidth * (double) this.column, PixelSnapHelpers.GetPixelSize((Visual) textView).Width) - textView.ScrollOffset.X;
    Point point0 = new Point(x, 0.0);
    Point point1 = new Point(x, Math.Max(textView.DocumentHeight, textView.ActualHeight));
    drawingContext.DrawLine(this.pen, point0, point1);
  }
}
