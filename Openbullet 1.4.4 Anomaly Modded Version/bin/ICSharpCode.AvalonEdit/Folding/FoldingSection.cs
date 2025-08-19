// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Folding.FoldingSection
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Text;

#nullable disable
namespace ICSharpCode.AvalonEdit.Folding;

public sealed class FoldingSection : TextSegment
{
  private readonly FoldingManager manager;
  private bool isFolded;
  internal CollapsedLineSection[] collapsedSections;
  private string title;

  public bool IsFolded
  {
    get => this.isFolded;
    set
    {
      if (this.isFolded == value)
        return;
      this.isFolded = value;
      this.ValidateCollapsedLineSections();
      this.manager.Redraw(this);
    }
  }

  internal void ValidateCollapsedLineSections()
  {
    if (!this.isFolded)
    {
      this.RemoveCollapsedLineSection();
    }
    else
    {
      DocumentLine lineByOffset1 = this.manager.document.GetLineByOffset(this.StartOffset.CoerceValue(0, this.manager.document.TextLength));
      DocumentLine lineByOffset2 = this.manager.document.GetLineByOffset(this.EndOffset.CoerceValue(0, this.manager.document.TextLength));
      if (lineByOffset1 == lineByOffset2)
      {
        this.RemoveCollapsedLineSection();
      }
      else
      {
        if (this.collapsedSections == null)
          this.collapsedSections = new CollapsedLineSection[this.manager.textViews.Count];
        DocumentLine nextLine = lineByOffset1.NextLine;
        for (int index = 0; index < this.collapsedSections.Length; ++index)
        {
          CollapsedLineSection collapsedSection = this.collapsedSections[index];
          if (collapsedSection == null || collapsedSection.Start != nextLine || collapsedSection.End != lineByOffset2)
          {
            collapsedSection?.Uncollapse();
            this.collapsedSections[index] = this.manager.textViews[index].CollapseLines(nextLine, lineByOffset2);
          }
        }
      }
    }
  }

  protected override void OnSegmentChanged()
  {
    this.ValidateCollapsedLineSections();
    base.OnSegmentChanged();
    if (!this.IsConnectedToCollection)
      return;
    this.manager.Redraw(this);
  }

  public string Title
  {
    get => this.title;
    set
    {
      if (!(this.title != value))
        return;
      this.title = value;
      if (!this.IsFolded)
        return;
      this.manager.Redraw(this);
    }
  }

  public string TextContent
  {
    get => this.manager.document.GetText(this.StartOffset, this.EndOffset - this.StartOffset);
  }

  [Obsolete]
  public string TooltipText
  {
    get
    {
      DocumentLine lineByOffset1 = this.manager.document.GetLineByOffset(this.StartOffset);
      DocumentLine lineByOffset2 = this.manager.document.GetLineByOffset(this.EndOffset);
      StringBuilder stringBuilder = new StringBuilder();
      DocumentLine documentLine = lineByOffset1;
      ISegment leadingWhitespace1 = TextUtilities.GetLeadingWhitespace(this.manager.document, lineByOffset1);
      for (; documentLine != lineByOffset2.NextLine; documentLine = documentLine.NextLine)
      {
        ISegment leadingWhitespace2 = TextUtilities.GetLeadingWhitespace(this.manager.document, documentLine);
        if (documentLine == lineByOffset1 && documentLine == lineByOffset2)
          stringBuilder.Append(this.manager.document.GetText(this.StartOffset, this.EndOffset - this.StartOffset));
        else if (documentLine == lineByOffset1)
        {
          if (documentLine.EndOffset - this.StartOffset > 0)
            stringBuilder.AppendLine(this.manager.document.GetText(this.StartOffset, documentLine.EndOffset - this.StartOffset).TrimStart());
        }
        else if (documentLine == lineByOffset2)
        {
          if (leadingWhitespace1.Length <= leadingWhitespace2.Length)
            stringBuilder.Append(this.manager.document.GetText(documentLine.Offset + leadingWhitespace1.Length, this.EndOffset - documentLine.Offset - leadingWhitespace1.Length));
          else
            stringBuilder.Append(this.manager.document.GetText(documentLine.Offset + leadingWhitespace2.Length, this.EndOffset - documentLine.Offset - leadingWhitespace2.Length));
        }
        else if (leadingWhitespace1.Length <= leadingWhitespace2.Length)
          stringBuilder.AppendLine(this.manager.document.GetText(documentLine.Offset + leadingWhitespace1.Length, documentLine.Length - leadingWhitespace1.Length));
        else
          stringBuilder.AppendLine(this.manager.document.GetText(documentLine.Offset + leadingWhitespace2.Length, documentLine.Length - leadingWhitespace2.Length));
      }
      return stringBuilder.ToString();
    }
  }

  public object Tag { get; set; }

  internal FoldingSection(FoldingManager manager, int startOffset, int endOffset)
  {
    this.manager = manager;
    this.StartOffset = startOffset;
    this.Length = endOffset - startOffset;
  }

  private void RemoveCollapsedLineSection()
  {
    if (this.collapsedSections == null)
      return;
    foreach (CollapsedLineSection collapsedSection in this.collapsedSections)
    {
      if (collapsedSection != null && collapsedSection.Start != null)
        collapsedSection.Uncollapse();
    }
    this.collapsedSections = (CollapsedLineSection[]) null;
  }
}
