// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.DocumentColorizingTransformer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Linq;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class DocumentColorizingTransformer : ColorizingTransformer
{
  private DocumentLine currentDocumentLine;
  private int firstLineStart;
  private int currentDocumentLineStartOffset;
  private int currentDocumentLineEndOffset;

  protected ITextRunConstructionContext CurrentContext { get; private set; }

  protected override void Colorize(ITextRunConstructionContext context)
  {
    this.CurrentContext = context != null ? context : throw new ArgumentNullException(nameof (context));
    this.currentDocumentLine = context.VisualLine.FirstDocumentLine;
    this.firstLineStart = this.currentDocumentLineStartOffset = this.currentDocumentLine.Offset;
    this.currentDocumentLineEndOffset = this.currentDocumentLineStartOffset + this.currentDocumentLine.Length;
    int num = this.currentDocumentLineStartOffset + this.currentDocumentLine.TotalLength;
    if (context.VisualLine.FirstDocumentLine == context.VisualLine.LastDocumentLine)
    {
      this.ColorizeLine(this.currentDocumentLine);
    }
    else
    {
      this.ColorizeLine(this.currentDocumentLine);
      foreach (VisualLineElement visualLineElement in context.VisualLine.Elements.ToArray<VisualLineElement>())
      {
        int offset = this.firstLineStart + visualLineElement.RelativeTextOffset;
        if (offset >= num)
        {
          this.currentDocumentLine = context.Document.GetLineByOffset(offset);
          this.currentDocumentLineStartOffset = this.currentDocumentLine.Offset;
          this.currentDocumentLineEndOffset = this.currentDocumentLineStartOffset + this.currentDocumentLine.Length;
          num = this.currentDocumentLineStartOffset + this.currentDocumentLine.TotalLength;
          this.ColorizeLine(this.currentDocumentLine);
        }
      }
    }
    this.currentDocumentLine = (DocumentLine) null;
    this.CurrentContext = (ITextRunConstructionContext) null;
  }

  protected abstract void ColorizeLine(DocumentLine line);

  protected void ChangeLinePart(int startOffset, int endOffset, Action<VisualLineElement> action)
  {
    if (startOffset < this.currentDocumentLineStartOffset || startOffset > this.currentDocumentLineEndOffset)
      throw new ArgumentOutOfRangeException(nameof (startOffset), (object) startOffset, $"Value must be between {(object) this.currentDocumentLineStartOffset} and {(object) this.currentDocumentLineEndOffset}");
    if (endOffset < startOffset || endOffset > this.currentDocumentLineEndOffset)
      throw new ArgumentOutOfRangeException(nameof (endOffset), (object) endOffset, $"Value must be between {(object) startOffset} and {(object) this.currentDocumentLineEndOffset}");
    VisualLine visualLine = this.CurrentContext.VisualLine;
    int visualColumn1 = visualLine.GetVisualColumn(startOffset - this.firstLineStart);
    int visualColumn2 = visualLine.GetVisualColumn(endOffset - this.firstLineStart);
    if (visualColumn1 >= visualColumn2)
      return;
    this.ChangeVisualElements(visualColumn1, visualColumn2, action);
  }
}
