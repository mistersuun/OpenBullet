// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class VisualLineElement
{
  protected VisualLineElement(int visualLength, int documentLength)
  {
    if (visualLength < 1)
      throw new ArgumentOutOfRangeException(nameof (visualLength), (object) visualLength, "Value must be at least 1");
    if (documentLength < 0)
      throw new ArgumentOutOfRangeException(nameof (documentLength), (object) documentLength, "Value must be at least 0");
    this.VisualLength = visualLength;
    this.DocumentLength = documentLength;
  }

  public int VisualLength { get; private set; }

  public int DocumentLength { get; private set; }

  public int VisualColumn { get; internal set; }

  public int RelativeTextOffset { get; internal set; }

  public VisualLineElementTextRunProperties TextRunProperties { get; private set; }

  public Brush BackgroundBrush { get; set; }

  internal void SetTextRunProperties(VisualLineElementTextRunProperties p)
  {
    this.TextRunProperties = p;
  }

  public abstract TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context);

  public virtual TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(
    int visualColumnLimit,
    ITextRunConstructionContext context)
  {
    return (TextSpan<CultureSpecificCharacterBufferRange>) null;
  }

  public virtual bool CanSplit => false;

  public virtual void Split(
    int splitVisualColumn,
    IList<VisualLineElement> elements,
    int elementIndex)
  {
    throw new NotSupportedException();
  }

  protected void SplitHelper(
    VisualLineElement firstPart,
    VisualLineElement secondPart,
    int splitVisualColumn,
    int splitRelativeTextOffset)
  {
    if (firstPart == null)
      throw new ArgumentNullException(nameof (firstPart));
    if (secondPart == null)
      throw new ArgumentNullException(nameof (secondPart));
    int num1 = splitVisualColumn - this.VisualColumn;
    int num2 = splitRelativeTextOffset - this.RelativeTextOffset;
    if (num1 <= 0 || num1 >= this.VisualLength)
      throw new ArgumentOutOfRangeException(nameof (splitVisualColumn), (object) splitVisualColumn, $"Value must be between {(object) (this.VisualColumn + 1)} and {(object) (this.VisualColumn + this.VisualLength - 1)}");
    if (num2 < 0 || num2 > this.DocumentLength)
      throw new ArgumentOutOfRangeException(nameof (splitRelativeTextOffset), (object) splitRelativeTextOffset, $"Value must be between {(object) this.RelativeTextOffset} and {(object) (this.RelativeTextOffset + this.DocumentLength)}");
    int visualLength = this.VisualLength;
    int documentLength = this.DocumentLength;
    int visualColumn = this.VisualColumn;
    int relativeTextOffset = this.RelativeTextOffset;
    firstPart.VisualColumn = visualColumn;
    secondPart.VisualColumn = visualColumn + num1;
    firstPart.RelativeTextOffset = relativeTextOffset;
    secondPart.RelativeTextOffset = relativeTextOffset + num2;
    firstPart.VisualLength = num1;
    secondPart.VisualLength = visualLength - num1;
    firstPart.DocumentLength = num2;
    secondPart.DocumentLength = documentLength - num2;
    if (firstPart.TextRunProperties == null)
      firstPart.TextRunProperties = this.TextRunProperties.Clone();
    if (secondPart.TextRunProperties == null)
      secondPart.TextRunProperties = this.TextRunProperties.Clone();
    firstPart.BackgroundBrush = this.BackgroundBrush;
    secondPart.BackgroundBrush = this.BackgroundBrush;
  }

  public virtual int GetVisualColumn(int relativeTextOffset)
  {
    return relativeTextOffset >= this.RelativeTextOffset + this.DocumentLength ? this.VisualColumn + this.VisualLength : this.VisualColumn;
  }

  public virtual int GetRelativeOffset(int visualColumn)
  {
    return visualColumn >= this.VisualColumn + this.VisualLength ? this.RelativeTextOffset + this.DocumentLength : this.RelativeTextOffset;
  }

  public virtual int GetNextCaretPosition(
    int visualColumn,
    LogicalDirection direction,
    CaretPositioningMode mode)
  {
    int visualColumn1 = this.VisualColumn;
    int nextCaretPosition = this.VisualColumn + this.VisualLength;
    if (direction == LogicalDirection.Backward)
    {
      if (visualColumn > nextCaretPosition && mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol)
        return nextCaretPosition;
      if (visualColumn > visualColumn1)
        return visualColumn1;
    }
    else
    {
      if (visualColumn < visualColumn1)
        return visualColumn1;
      if (visualColumn < nextCaretPosition && mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol)
        return nextCaretPosition;
    }
    return -1;
  }

  public virtual bool IsWhitespace(int visualColumn) => false;

  public virtual bool HandlesLineBorders => false;

  protected internal virtual void OnQueryCursor(QueryCursorEventArgs e)
  {
  }

  protected internal virtual void OnMouseDown(MouseButtonEventArgs e)
  {
  }

  protected internal virtual void OnMouseUp(MouseButtonEventArgs e)
  {
  }
}
