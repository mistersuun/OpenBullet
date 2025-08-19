// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.DocumentLine
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Diagnostics;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public sealed class DocumentLine : IDocumentLine, ISegment
{
  internal DocumentLine left;
  internal DocumentLine right;
  internal DocumentLine parent;
  internal bool color;
  internal int nodeTotalCount;
  internal int nodeTotalLength;
  internal bool isDeleted;
  private int totalLength;
  private byte delimiterLength;

  internal void ResetLine()
  {
    this.totalLength = (int) (this.delimiterLength = (byte) 0);
    this.isDeleted = this.color = false;
    this.left = this.right = this.parent = (DocumentLine) null;
  }

  internal DocumentLine InitLineNode()
  {
    this.nodeTotalCount = 1;
    this.nodeTotalLength = this.TotalLength;
    return this;
  }

  internal DocumentLine LeftMost
  {
    get
    {
      DocumentLine leftMost = this;
      while (leftMost.left != null)
        leftMost = leftMost.left;
      return leftMost;
    }
  }

  internal DocumentLine RightMost
  {
    get
    {
      DocumentLine rightMost = this;
      while (rightMost.right != null)
        rightMost = rightMost.right;
      return rightMost;
    }
  }

  internal DocumentLine(TextDocument document)
  {
  }

  [Conditional("DEBUG")]
  private void DebugVerifyAccess()
  {
  }

  public bool IsDeleted => this.isDeleted;

  public int LineNumber
  {
    get
    {
      if (this.IsDeleted)
        throw new InvalidOperationException();
      return DocumentLineTree.GetIndexFromNode(this) + 1;
    }
  }

  public int Offset
  {
    get
    {
      if (this.IsDeleted)
        throw new InvalidOperationException();
      return DocumentLineTree.GetOffsetFromNode(this);
    }
  }

  public int EndOffset => this.Offset + this.Length;

  public int Length => this.totalLength - (int) this.delimiterLength;

  public int TotalLength
  {
    get => this.totalLength;
    internal set => this.totalLength = value;
  }

  public int DelimiterLength
  {
    get => (int) this.delimiterLength;
    internal set => this.delimiterLength = (byte) value;
  }

  public DocumentLine NextLine
  {
    get
    {
      if (this.right != null)
        return this.right.LeftMost;
      DocumentLine nextLine = this;
      DocumentLine documentLine;
      do
      {
        documentLine = nextLine;
        nextLine = nextLine.parent;
      }
      while (nextLine != null && nextLine.right == documentLine);
      return nextLine;
    }
  }

  public DocumentLine PreviousLine
  {
    get
    {
      if (this.left != null)
        return this.left.RightMost;
      DocumentLine previousLine = this;
      DocumentLine documentLine;
      do
      {
        documentLine = previousLine;
        previousLine = previousLine.parent;
      }
      while (previousLine != null && previousLine.left == documentLine);
      return previousLine;
    }
  }

  IDocumentLine IDocumentLine.NextLine => (IDocumentLine) this.NextLine;

  IDocumentLine IDocumentLine.PreviousLine => (IDocumentLine) this.PreviousLine;

  public override string ToString()
  {
    if (this.IsDeleted)
      return "[DocumentLine deleted]";
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[DocumentLine Number={0} Offset={1} Length={2}]", (object) this.LineNumber, (object) this.Offset, (object) this.Length);
  }
}
