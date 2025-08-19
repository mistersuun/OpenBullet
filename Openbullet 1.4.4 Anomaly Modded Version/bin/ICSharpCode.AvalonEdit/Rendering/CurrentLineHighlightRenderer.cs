// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.CurrentLineHighlightRenderer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class CurrentLineHighlightRenderer : IBackgroundRenderer
{
  private int line;
  private TextView textView;
  public static readonly Color DefaultBackground = Color.FromArgb((byte) 22, (byte) 20, (byte) 220, (byte) 224 /*0xE0*/);
  public static readonly Color DefaultBorder = Color.FromArgb((byte) 52, (byte) 0, byte.MaxValue, (byte) 110);

  public int Line
  {
    get => this.line;
    set
    {
      if (this.line == value)
        return;
      this.line = value;
      this.textView.InvalidateLayer(this.Layer);
    }
  }

  public KnownLayer Layer => KnownLayer.Selection;

  public Brush BackgroundBrush { get; set; }

  public Pen BorderPen { get; set; }

  public CurrentLineHighlightRenderer(TextView textView)
  {
    if (textView == null)
      throw new ArgumentNullException(nameof (textView));
    this.BorderPen = new Pen((Brush) new SolidColorBrush(CurrentLineHighlightRenderer.DefaultBorder), 1.0);
    this.BorderPen.Freeze();
    this.BackgroundBrush = (Brush) new SolidColorBrush(CurrentLineHighlightRenderer.DefaultBackground);
    this.BackgroundBrush.Freeze();
    this.textView = textView;
    this.textView.BackgroundRenderers.Add((IBackgroundRenderer) this);
    this.line = 0;
  }

  public void Draw(TextView textView, DrawingContext drawingContext)
  {
    if (!this.textView.Options.HighlightCurrentLine)
      return;
    BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
    VisualLine visualLine = this.textView.GetVisualLine(this.line);
    if (visualLine == null)
      return;
    double y = visualLine.VisualTop - this.textView.ScrollOffset.Y;
    backgroundGeometryBuilder.AddRectangle(textView, new Rect(0.0, y, textView.ActualWidth, visualLine.Height));
    Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
    if (geometry == null)
      return;
    drawingContext.DrawGeometry(this.BackgroundBrush, this.BorderPen, geometry);
  }
}
