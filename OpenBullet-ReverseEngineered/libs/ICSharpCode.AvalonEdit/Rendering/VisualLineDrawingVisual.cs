// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineDrawingVisual
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class VisualLineDrawingVisual : DrawingVisual
{
  public readonly VisualLine VisualLine;
  public readonly double Height;
  internal bool IsAdded;

  public VisualLineDrawingVisual(VisualLine visualLine)
  {
    this.VisualLine = visualLine;
    DrawingContext drawingContext = this.RenderOpen();
    double y = 0.0;
    foreach (TextLine textLine in visualLine.TextLines)
    {
      textLine.Draw(drawingContext, new Point(0.0, y), InvertAxes.None);
      y += textLine.Height;
    }
    this.Height = y;
    drawingContext.Close();
  }

  protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
  {
    return (GeometryHitTestResult) null;
  }

  protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
  {
    return (HitTestResult) null;
  }
}
