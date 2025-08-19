// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.TextLayer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class TextLayer(TextView textView) : Layer(textView, KnownLayer.Text)
{
  internal int index;
  private List<VisualLineDrawingVisual> visuals = new List<VisualLineDrawingVisual>();

  internal void SetVisualLines(ICollection<VisualLine> visualLines)
  {
    foreach (VisualLineDrawingVisual visual in this.visuals)
    {
      if (visual.VisualLine.IsDisposed)
        this.RemoveVisualChild((Visual) visual);
    }
    this.visuals.Clear();
    foreach (VisualLine visualLine in (IEnumerable<VisualLine>) visualLines)
    {
      VisualLineDrawingVisual child = visualLine.Render();
      if (!child.IsAdded)
      {
        this.AddVisualChild((Visual) child);
        child.IsAdded = true;
      }
      this.visuals.Add(child);
    }
    this.InvalidateArrange();
  }

  protected override int VisualChildrenCount => this.visuals.Count;

  protected override Visual GetVisualChild(int index) => (Visual) this.visuals[index];

  protected override void ArrangeCore(Rect finalRect)
  {
    this.textView.ArrangeTextLayer((IList<VisualLineDrawingVisual>) this.visuals);
  }
}
