// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.SimpleSelection
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class SimpleSelection : Selection
{
  private readonly TextViewPosition start;
  private readonly TextViewPosition end;
  private readonly int startOffset;
  private readonly int endOffset;

  internal SimpleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
    : base(textArea)
  {
    this.start = start;
    this.end = end;
    this.startOffset = textArea.Document.GetOffset(start.Location);
    this.endOffset = textArea.Document.GetOffset(end.Location);
  }

  public override IEnumerable<SelectionSegment> Segments
  {
    get
    {
      return ExtensionMethods.Sequence<SelectionSegment>(new SelectionSegment(this.startOffset, this.start.VisualColumn, this.endOffset, this.end.VisualColumn));
    }
  }

  public override ISegment SurroundingSegment
  {
    get => (ISegment) new SelectionSegment(this.startOffset, this.endOffset);
  }

  public override void ReplaceSelectionWithText(string newText)
  {
    if (newText == null)
      throw new ArgumentNullException(nameof (newText));
    using (this.textArea.Document.RunUpdate())
    {
      ISegment[] deletableSegments = this.textArea.GetDeletableSegments(this.SurroundingSegment);
      for (int index = deletableSegments.Length - 1; index >= 0; --index)
      {
        if (index == deletableSegments.Length - 1)
        {
          if (deletableSegments[index].Offset == this.SurroundingSegment.Offset && deletableSegments[index].Length == this.SurroundingSegment.Length)
            newText = this.AddSpacesIfRequired(newText, this.start, this.end);
          if (string.IsNullOrEmpty(newText))
          {
            if (this.start.CompareTo(this.end) <= 0)
              this.textArea.Caret.Position = this.start;
            else
              this.textArea.Caret.Position = this.end;
          }
          else
            this.textArea.Caret.Offset = deletableSegments[index].EndOffset;
          this.textArea.Document.Replace(deletableSegments[index], newText);
        }
        else
          this.textArea.Document.Remove(deletableSegments[index]);
      }
      if (deletableSegments.Length == 0)
        return;
      this.textArea.ClearSelection();
    }
  }

  public override TextViewPosition StartPosition => this.start;

  public override TextViewPosition EndPosition => this.end;

  public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    int num1;
    int num2;
    if (this.startOffset <= this.endOffset)
    {
      num1 = e.GetNewOffset(this.startOffset, AnchorMovementType.Default);
      num2 = Math.Max(num1, e.GetNewOffset(this.endOffset, AnchorMovementType.BeforeInsertion));
    }
    else
    {
      num2 = e.GetNewOffset(this.endOffset, AnchorMovementType.Default);
      num1 = Math.Max(num2, e.GetNewOffset(this.startOffset, AnchorMovementType.BeforeInsertion));
    }
    return Selection.Create(this.textArea, new TextViewPosition(this.textArea.Document.GetLocation(num1), this.start.VisualColumn), new TextViewPosition(this.textArea.Document.GetLocation(num2), this.end.VisualColumn));
  }

  public override bool IsEmpty
  {
    get => this.startOffset == this.endOffset && this.start.VisualColumn == this.end.VisualColumn;
  }

  public override int Length => Math.Abs(this.endOffset - this.startOffset);

  public override Selection SetEndpoint(TextViewPosition endPosition)
  {
    return Selection.Create(this.textArea, this.start, endPosition);
  }

  public override Selection StartSelectionOrSetEndpoint(
    TextViewPosition startPosition,
    TextViewPosition endPosition)
  {
    if (this.textArea.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    return Selection.Create(this.textArea, this.start, endPosition);
  }

  public override int GetHashCode()
  {
    return this.startOffset * 27811 + this.endOffset + this.textArea.GetHashCode();
  }

  public override bool Equals(object obj)
  {
    return obj is SimpleSelection simpleSelection && this.start.Equals(simpleSelection.start) && this.end.Equals(simpleSelection.end) && this.startOffset == simpleSelection.startOffset && this.endOffset == simpleSelection.endOffset && this.textArea == simpleSelection.textArea;
  }

  public override string ToString()
  {
    return $"[SimpleSelection Start={(object) this.start} End={(object) this.end}]";
  }
}
