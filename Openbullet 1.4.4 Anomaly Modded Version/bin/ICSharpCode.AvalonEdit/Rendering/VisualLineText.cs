// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineText
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class VisualLineText : VisualLineElement
{
  private VisualLine parentVisualLine;

  public VisualLine ParentVisualLine => this.parentVisualLine;

  public VisualLineText(VisualLine parentVisualLine, int length)
    : base(length, length)
  {
    this.parentVisualLine = parentVisualLine != null ? parentVisualLine : throw new ArgumentNullException(nameof (parentVisualLine));
  }

  protected virtual VisualLineText CreateInstance(int length)
  {
    return new VisualLineText(this.parentVisualLine, length);
  }

  public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
  {
    if (context == null)
      throw new ArgumentNullException(nameof (context));
    int num = startVisualColumn - this.VisualColumn;
    StringSegment text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + this.RelativeTextOffset + num, this.DocumentLength - num);
    return (TextRun) new TextCharacters(text.Text, text.Offset, text.Count, (System.Windows.Media.TextFormatting.TextRunProperties) this.TextRunProperties);
  }

  public override bool IsWhitespace(int visualColumn)
  {
    return char.IsWhiteSpace(this.parentVisualLine.Document.GetCharAt(visualColumn - this.VisualColumn + this.parentVisualLine.FirstDocumentLine.Offset + this.RelativeTextOffset));
  }

  public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(
    int visualColumnLimit,
    ITextRunConstructionContext context)
  {
    if (context == null)
      throw new ArgumentNullException(nameof (context));
    int length = visualColumnLimit - this.VisualColumn;
    StringSegment text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + this.RelativeTextOffset, length);
    CharacterBufferRange characterBufferRange = new CharacterBufferRange(text.Text, text.Offset, text.Count);
    return new TextSpan<CultureSpecificCharacterBufferRange>(characterBufferRange.Length, new CultureSpecificCharacterBufferRange(this.TextRunProperties.CultureInfo, characterBufferRange));
  }

  public override bool CanSplit => true;

  public override void Split(
    int splitVisualColumn,
    IList<VisualLineElement> elements,
    int elementIndex)
  {
    if (splitVisualColumn <= this.VisualColumn || splitVisualColumn >= this.VisualColumn + this.VisualLength)
      throw new ArgumentOutOfRangeException(nameof (splitVisualColumn), (object) splitVisualColumn, $"Value must be between {(object) (this.VisualColumn + 1)} and {(object) (this.VisualColumn + this.VisualLength - 1)}");
    if (elements == null)
      throw new ArgumentNullException(nameof (elements));
    if (elements[elementIndex] != this)
      throw new ArgumentException("Invalid elementIndex - couldn't find this element at the index");
    int num = splitVisualColumn - this.VisualColumn;
    VisualLineText instance = this.CreateInstance(this.DocumentLength - num);
    this.SplitHelper((VisualLineElement) this, (VisualLineElement) instance, splitVisualColumn, num + this.RelativeTextOffset);
    elements.Insert(elementIndex + 1, (VisualLineElement) instance);
  }

  public override int GetRelativeOffset(int visualColumn)
  {
    return this.RelativeTextOffset + visualColumn - this.VisualColumn;
  }

  public override int GetVisualColumn(int relativeTextOffset)
  {
    return this.VisualColumn + relativeTextOffset - this.RelativeTextOffset;
  }

  public override int GetNextCaretPosition(
    int visualColumn,
    LogicalDirection direction,
    CaretPositioningMode mode)
  {
    int num = this.parentVisualLine.StartOffset + this.RelativeTextOffset;
    int nextCaretPosition = TextUtilities.GetNextCaretPosition((ITextSource) this.parentVisualLine.Document, num + visualColumn - this.VisualColumn, direction, mode);
    return nextCaretPosition < num || nextCaretPosition > num + this.DocumentLength ? -1 : this.VisualColumn + nextCaretPosition - num;
  }
}
