// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.LineManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal sealed class LineManager
{
  private readonly TextDocument document;
  private readonly DocumentLineTree documentLineTree;
  private ILineTracker[] lineTrackers;

  internal void UpdateListOfLineTrackers()
  {
    this.lineTrackers = this.document.LineTrackers.ToArray<ILineTracker>();
  }

  public LineManager(DocumentLineTree documentLineTree, TextDocument document)
  {
    this.document = document;
    this.documentLineTree = documentLineTree;
    this.UpdateListOfLineTrackers();
    this.Rebuild();
  }

  public void Rebuild()
  {
    DocumentLine documentLine = this.documentLineTree.GetByNumber(1);
    for (DocumentLine nextLine = documentLine.NextLine; nextLine != null; nextLine = nextLine.NextLine)
    {
      nextLine.isDeleted = true;
      nextLine.parent = nextLine.left = nextLine.right = (DocumentLine) null;
    }
    documentLine.ResetLine();
    SimpleSegment simpleSegment = NewLineFinder.NextNewLine((ITextSource) this.document, 0);
    List<DocumentLine> documentLines = new List<DocumentLine>();
    int offset = 0;
    for (; simpleSegment != SimpleSegment.Invalid; simpleSegment = NewLineFinder.NextNewLine((ITextSource) this.document, offset))
    {
      documentLine.TotalLength = simpleSegment.Offset + simpleSegment.Length - offset;
      documentLine.DelimiterLength = simpleSegment.Length;
      offset = simpleSegment.Offset + simpleSegment.Length;
      documentLines.Add(documentLine);
      documentLine = new DocumentLine(this.document);
    }
    documentLine.TotalLength = this.document.TextLength - offset;
    documentLines.Add(documentLine);
    this.documentLineTree.RebuildTree(documentLines);
    foreach (ILineTracker lineTracker in this.lineTrackers)
      lineTracker.RebuildDocument();
  }

  public void Remove(int offset, int length)
  {
    if (length == 0)
      return;
    DocumentLine byOffset1 = this.documentLineTree.GetByOffset(offset);
    int offset1 = byOffset1.Offset;
    if (offset > offset1 + byOffset1.Length)
    {
      this.SetLineLength(byOffset1, byOffset1.TotalLength - 1);
      this.Remove(offset, length - 1);
    }
    else if (offset + length < offset1 + byOffset1.TotalLength)
    {
      this.SetLineLength(byOffset1, byOffset1.TotalLength - length);
    }
    else
    {
      int num1 = offset1 + byOffset1.TotalLength - offset;
      DocumentLine byOffset2 = this.documentLineTree.GetByOffset(offset + length);
      if (byOffset2 == byOffset1)
      {
        this.SetLineLength(byOffset1, byOffset1.TotalLength - length);
      }
      else
      {
        int num2 = byOffset2.Offset + byOffset2.TotalLength - (offset + length);
        DocumentLine nextLine = byOffset1.NextLine;
        DocumentLine lineToRemove;
        do
        {
          lineToRemove = nextLine;
          nextLine = nextLine.NextLine;
          this.RemoveLine(lineToRemove);
        }
        while (lineToRemove != byOffset2);
        this.SetLineLength(byOffset1, byOffset1.TotalLength - num1 + num2);
      }
    }
  }

  private void RemoveLine(DocumentLine lineToRemove)
  {
    foreach (ILineTracker lineTracker in this.lineTrackers)
      lineTracker.BeforeRemoveLine(lineToRemove);
    this.documentLineTree.RemoveLine(lineToRemove);
  }

  public void Insert(int offset, ITextSource text)
  {
    DocumentLine line = this.documentLineTree.GetByOffset(offset);
    int offset1 = line.Offset;
    if (offset > offset1 + line.Length)
    {
      this.SetLineLength(line, line.TotalLength - 1);
      line = this.SetLineLength(this.InsertLineAfter(line, 1), 1);
    }
    SimpleSegment simpleSegment = NewLineFinder.NextNewLine(text, 0);
    if (simpleSegment == SimpleSegment.Invalid)
    {
      this.SetLineLength(line, line.TotalLength + text.TextLength);
    }
    else
    {
      int offset2 = 0;
      for (; simpleSegment != SimpleSegment.Invalid; simpleSegment = NewLineFinder.NextNewLine(text, offset2))
      {
        int num1 = offset + simpleSegment.Offset + simpleSegment.Length;
        int offset3 = line.Offset;
        int num2 = offset3 + line.TotalLength - (offset + offset2);
        line = this.SetLineLength(this.InsertLineAfter(this.SetLineLength(line, num1 - offset3), num2), num2);
        offset2 = simpleSegment.Offset + simpleSegment.Length;
      }
      if (offset2 == text.TextLength)
        return;
      this.SetLineLength(line, line.TotalLength + text.TextLength - offset2);
    }
  }

  private DocumentLine InsertLineAfter(DocumentLine line, int length)
  {
    DocumentLine newLine = this.documentLineTree.InsertLineAfter(line, length);
    foreach (ILineTracker lineTracker in this.lineTrackers)
      lineTracker.LineInserted(line, newLine);
    return newLine;
  }

  private DocumentLine SetLineLength(DocumentLine line, int newTotalLength)
  {
    if (newTotalLength - line.TotalLength != 0)
    {
      foreach (ILineTracker lineTracker in this.lineTrackers)
        lineTracker.SetLineLength(line, newTotalLength);
      line.TotalLength = newTotalLength;
      DocumentLineTree.UpdateAfterChildrenChange(line);
    }
    if (newTotalLength == 0)
    {
      line.DelimiterLength = 0;
    }
    else
    {
      int offset = line.Offset;
      switch (this.document.GetCharAt(offset + newTotalLength - 1))
      {
        case '\n':
          if (newTotalLength >= 2 && this.document.GetCharAt(offset + newTotalLength - 2) == '\r')
          {
            line.DelimiterLength = 2;
            break;
          }
          if (newTotalLength == 1 && offset > 0 && this.document.GetCharAt(offset - 1) == '\r')
          {
            DocumentLine previousLine = line.PreviousLine;
            this.RemoveLine(line);
            return this.SetLineLength(previousLine, previousLine.TotalLength + 1);
          }
          line.DelimiterLength = 1;
          break;
        case '\r':
          line.DelimiterLength = 1;
          break;
        default:
          line.DelimiterLength = 0;
          break;
      }
    }
    return line;
  }

  public void ChangeComplete(DocumentChangeEventArgs e)
  {
    foreach (ILineTracker lineTracker in this.lineTrackers)
      lineTracker.ChangeComplete(e);
  }
}
