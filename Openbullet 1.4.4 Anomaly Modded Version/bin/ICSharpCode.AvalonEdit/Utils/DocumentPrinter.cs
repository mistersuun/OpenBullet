// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.DocumentPrinter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public static class DocumentPrinter
{
  public static Block ConvertTextDocumentToBlock(IDocument document, IHighlighter highlighter)
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    Paragraph block = new Paragraph();
    block.TextAlignment = TextAlignment.Left;
    for (int lineNumber = 1; lineNumber <= document.LineCount; ++lineNumber)
    {
      if (lineNumber > 1)
        block.Inlines.Add((Inline) new LineBreak());
      IDocumentLine lineByNumber = document.GetLineByNumber(lineNumber);
      if (highlighter != null)
      {
        HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
        block.Inlines.AddRange((IEnumerable) highlightedLine.ToRichText().CreateRuns());
      }
      else
        block.Inlines.Add(document.GetText((ISegment) lineByNumber));
    }
    return (Block) block;
  }

  public static RichText ConvertTextDocumentToRichText(IDocument document, IHighlighter highlighter)
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    List<RichText> richTextList = new List<RichText>();
    for (int lineNumber = 1; lineNumber <= document.LineCount; ++lineNumber)
    {
      IDocumentLine lineByNumber = document.GetLineByNumber(lineNumber);
      if (lineNumber > 1)
        richTextList.Add((RichText) (lineByNumber.PreviousLine.DelimiterLength == 2 ? "\r\n" : "\n"));
      if (highlighter != null)
      {
        HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
        richTextList.Add(highlightedLine.ToRichText());
      }
      else
        richTextList.Add((RichText) document.GetText((ISegment) lineByNumber));
    }
    return RichText.Concat(richTextList.ToArray());
  }

  public static FlowDocument CreateFlowDocumentForEditor(ICSharpCode.AvalonEdit.TextEditor editor)
  {
    IHighlighter service = editor.TextArea.GetService(typeof (IHighlighter)) as IHighlighter;
    return new FlowDocument(DocumentPrinter.ConvertTextDocumentToBlock((IDocument) editor.Document, service))
    {
      FontFamily = editor.FontFamily,
      FontSize = editor.FontSize
    };
  }
}
