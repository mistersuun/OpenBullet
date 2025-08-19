// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Indentation.CSharp.TextDocumentAccessor
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Indentation.CSharp;

public sealed class TextDocumentAccessor : IDocumentAccessor
{
  private readonly TextDocument doc;
  private readonly int minLine;
  private readonly int maxLine;
  private int num;
  private string text;
  private DocumentLine line;
  private bool lineDirty;

  public TextDocumentAccessor(TextDocument document)
  {
    this.doc = document != null ? document : throw new ArgumentNullException(nameof (document));
    this.minLine = 1;
    this.maxLine = this.doc.LineCount;
  }

  public TextDocumentAccessor(TextDocument document, int minLine, int maxLine)
  {
    this.doc = document != null ? document : throw new ArgumentNullException(nameof (document));
    this.minLine = minLine;
    this.maxLine = maxLine;
  }

  public bool IsReadOnly => this.num < this.minLine;

  public int LineNumber => this.num;

  public string Text
  {
    get => this.text;
    set
    {
      if (this.num < this.minLine)
        return;
      this.text = value;
      this.lineDirty = true;
    }
  }

  public bool MoveNext()
  {
    if (this.lineDirty)
    {
      this.doc.Replace((ISegment) this.line, this.text);
      this.lineDirty = false;
    }
    ++this.num;
    if (this.num > this.maxLine)
      return false;
    this.line = this.doc.GetLineByNumber(this.num);
    this.text = this.doc.GetText((ISegment) this.line);
    return true;
  }
}
