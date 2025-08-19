// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.DocumentTextWriter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.IO;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public class DocumentTextWriter : TextWriter
{
  private readonly IDocument document;
  private int insertionOffset;

  public DocumentTextWriter(IDocument document, int insertionOffset)
  {
    this.insertionOffset = insertionOffset;
    this.document = document != null ? document : throw new ArgumentNullException(nameof (document));
    IDocumentLine documentLine = document.GetLineByOffset(insertionOffset);
    if (documentLine.DelimiterLength == 0)
      documentLine = documentLine.PreviousLine;
    if (documentLine == null)
      return;
    this.NewLine = document.GetText(documentLine.EndOffset, documentLine.DelimiterLength);
  }

  public int InsertionOffset
  {
    get => this.insertionOffset;
    set => this.insertionOffset = value;
  }

  public override void Write(char value)
  {
    this.document.Insert(this.insertionOffset, value.ToString());
    ++this.insertionOffset;
  }

  public override void Write(char[] buffer, int index, int count)
  {
    this.document.Insert(this.insertionOffset, new string(buffer, index, count));
    this.insertionOffset += count;
  }

  public override void Write(string value)
  {
    this.document.Insert(this.insertionOffset, value);
    this.insertionOffset += value.Length;
  }

  public override Encoding Encoding => Encoding.UTF8;
}
