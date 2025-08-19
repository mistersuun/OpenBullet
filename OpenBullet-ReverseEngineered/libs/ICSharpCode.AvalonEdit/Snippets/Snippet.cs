// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.Snippet
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class Snippet : SnippetContainerElement
{
  public void Insert(TextArea textArea)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
    int num = textArea.Caret.Offset;
    if (surroundingSegment != null)
      num = surroundingSegment.Offset + TextUtilities.GetWhitespaceAfter((ITextSource) textArea.Document, surroundingSegment.Offset).Length;
    InsertionContext context = new InsertionContext(textArea, num);
    using (context.Document.RunUpdate())
    {
      if (surroundingSegment != null)
        textArea.Document.Remove(num, surroundingSegment.EndOffset - num);
      this.Insert(context);
      context.RaiseInsertionCompleted(EventArgs.Empty);
    }
  }
}
