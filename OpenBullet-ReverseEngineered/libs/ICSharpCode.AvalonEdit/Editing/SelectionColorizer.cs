// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.SelectionColorizer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class SelectionColorizer : ColorizingTransformer
{
  private TextArea textArea;

  public SelectionColorizer(TextArea textArea)
  {
    this.textArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
  }

  protected override void Colorize(ITextRunConstructionContext context)
  {
    if (this.textArea.SelectionForeground == null)
      return;
    int offset = context.VisualLine.FirstDocumentLine.Offset;
    int num = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;
    foreach (SelectionSegment segment in this.textArea.Selection.Segments)
    {
      int startOffset = segment.StartOffset;
      int endOffset = segment.EndOffset;
      if (endOffset > offset && startOffset < num)
        this.ChangeVisualElements(startOffset >= offset ? context.VisualLine.ValidateVisualColumn(segment.StartOffset, segment.StartVisualColumn, this.textArea.Selection.EnableVirtualSpace) : 0, endOffset <= num ? context.VisualLine.ValidateVisualColumn(segment.EndOffset, segment.EndVisualColumn, this.textArea.Selection.EnableVirtualSpace) : (this.textArea.Selection.EnableVirtualSpace ? int.MaxValue : context.VisualLine.VisualLengthWithEndOfLineMarker), (Action<VisualLineElement>) (element => element.TextRunProperties.SetForegroundBrush(this.textArea.SelectionForeground)));
    }
  }
}
