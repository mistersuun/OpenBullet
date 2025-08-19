// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Indentation.CSharp;

public class CSharpIndentationStrategy : DefaultIndentationStrategy
{
  private string indentationString = "\t";

  public CSharpIndentationStrategy()
  {
  }

  public CSharpIndentationStrategy(TextEditorOptions options)
  {
    this.IndentationString = options.IndentationString;
  }

  public string IndentationString
  {
    get => this.indentationString;
    set
    {
      this.indentationString = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Indentation string must not be null or empty");
    }
  }

  public void Indent(IDocumentAccessor document, bool keepEmptyLines)
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    new IndentationReformatter().Reformat(document, new IndentationSettings()
    {
      IndentString = this.IndentationString,
      LeaveEmptyLines = keepEmptyLines
    });
  }

  public override void IndentLine(TextDocument document, DocumentLine line)
  {
    int lineNumber = line.LineNumber;
    TextDocumentAccessor document1 = new TextDocumentAccessor(document, lineNumber, lineNumber);
    this.Indent((IDocumentAccessor) document1, false);
    if (document1.Text.Length != 0)
      return;
    base.IndentLine(document, line);
  }

  public override void IndentLines(TextDocument document, int beginLine, int endLine)
  {
    this.Indent((IDocumentAccessor) new TextDocumentAccessor(document, beginLine, endLine), true);
  }
}
