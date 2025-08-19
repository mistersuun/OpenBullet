// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Indentation;

public class DefaultIndentationStrategy : IIndentationStrategy
{
  public virtual void IndentLine(TextDocument document, DocumentLine line)
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    DocumentLine documentLine = line != null ? line.PreviousLine : throw new ArgumentNullException(nameof (line));
    if (documentLine == null)
      return;
    ISegment whitespaceAfter1 = TextUtilities.GetWhitespaceAfter((ITextSource) document, documentLine.Offset);
    string text = document.GetText(whitespaceAfter1);
    ISegment whitespaceAfter2 = TextUtilities.GetWhitespaceAfter((ITextSource) document, line.Offset);
    document.Replace(whitespaceAfter2.Offset, whitespaceAfter2.Length, text, OffsetChangeMappingType.RemoveAndInsert);
  }

  public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
  {
  }
}
