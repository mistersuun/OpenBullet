// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.SelectionLayer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class SelectionLayer : Layer, IWeakEventListener
{
  private readonly TextArea textArea;

  public SelectionLayer(TextArea textArea)
    : base(textArea.TextView, KnownLayer.Selection)
  {
    this.IsHitTestVisible = false;
    this.textArea = textArea;
    WeakEventManagerBase<TextViewWeakEventManager.VisualLinesChanged, TextView>.AddListener(this.textView, (IWeakEventListener) this);
    WeakEventManagerBase<TextViewWeakEventManager.ScrollOffsetChanged, TextView>.AddListener(this.textView, (IWeakEventListener) this);
  }

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (!(managerType == typeof (TextViewWeakEventManager.VisualLinesChanged)) && !(managerType == typeof (TextViewWeakEventManager.ScrollOffsetChanged)))
      return false;
    this.InvalidateVisual();
    return true;
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    base.OnRender(drawingContext);
    Pen selectionBorder = this.textArea.SelectionBorder;
    BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder();
    backgroundGeometryBuilder.AlignToWholePixels = true;
    backgroundGeometryBuilder.BorderThickness = selectionBorder != null ? selectionBorder.Thickness : 0.0;
    backgroundGeometryBuilder.ExtendToFullWidthAtLineEnd = this.textArea.Selection.EnableVirtualSpace;
    backgroundGeometryBuilder.CornerRadius = this.textArea.SelectionCornerRadius;
    foreach (SelectionSegment segment in this.textArea.Selection.Segments)
      backgroundGeometryBuilder.AddSegment(this.textView, (ISegment) segment);
    Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
    if (geometry == null)
      return;
    drawingContext.DrawGeometry(this.textArea.SelectionBrush, selectionBorder, geometry);
  }
}
