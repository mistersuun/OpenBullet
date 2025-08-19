// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.SingleCharacterElementGenerator
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class SingleCharacterElementGenerator : 
  VisualLineElementGenerator,
  IBuiltinElementGenerator
{
  public bool ShowSpaces { get; set; }

  public bool ShowTabs { get; set; }

  public bool ShowBoxForControlCharacters { get; set; }

  public SingleCharacterElementGenerator()
  {
    this.ShowSpaces = true;
    this.ShowTabs = true;
    this.ShowBoxForControlCharacters = true;
  }

  void IBuiltinElementGenerator.FetchOptions(TextEditorOptions options)
  {
    this.ShowSpaces = options.ShowSpaces;
    this.ShowTabs = options.ShowTabs;
    this.ShowBoxForControlCharacters = options.ShowBoxForControlCharacters;
  }

  public override int GetFirstInterestedOffset(int startOffset)
  {
    DocumentLine lastDocumentLine = this.CurrentContext.VisualLine.LastDocumentLine;
    StringSegment text = this.CurrentContext.GetText(startOffset, lastDocumentLine.EndOffset - startOffset);
    for (int index = 0; index < text.Count; ++index)
    {
      char c = text.Text[text.Offset + index];
      switch (c)
      {
        case '\t':
          if (this.ShowTabs)
            return startOffset + index;
          break;
        case ' ':
          if (this.ShowSpaces)
            return startOffset + index;
          break;
        default:
          if (this.ShowBoxForControlCharacters && char.IsControl(c))
            return startOffset + index;
          break;
      }
    }
    return -1;
  }

  public override VisualLineElement ConstructElement(int offset)
  {
    char charAt = this.CurrentContext.Document.GetCharAt(offset);
    if (this.ShowSpaces && charAt == ' ')
      return (VisualLineElement) new SingleCharacterElementGenerator.SpaceTextElement(this.CurrentContext.TextView.cachedElements.GetTextForNonPrintableCharacter("·", this.CurrentContext));
    if (this.ShowTabs && charAt == '\t')
      return (VisualLineElement) new SingleCharacterElementGenerator.TabTextElement(this.CurrentContext.TextView.cachedElements.GetTextForNonPrintableCharacter("»", this.CurrentContext));
    if (!this.ShowBoxForControlCharacters || !char.IsControl(charAt))
      return (VisualLineElement) null;
    VisualLineElementTextRunProperties properties = new VisualLineElementTextRunProperties(this.CurrentContext.GlobalTextRunProperties);
    properties.SetForegroundBrush((Brush) Brushes.White);
    return (VisualLineElement) new SingleCharacterElementGenerator.SpecialCharacterBoxElement(FormattedTextElement.PrepareText(TextFormatterFactory.Create((DependencyObject) this.CurrentContext.TextView), TextUtilities.GetControlCharacterName(charAt), (TextRunProperties) properties));
  }

  private sealed class SpaceTextElement : FormattedTextElement
  {
    public SpaceTextElement(TextLine textLine)
      : base(textLine, 1)
    {
      this.BreakBefore = LineBreakCondition.BreakPossible;
      this.BreakAfter = LineBreakCondition.BreakDesired;
    }

    public override int GetNextCaretPosition(
      int visualColumn,
      LogicalDirection direction,
      CaretPositioningMode mode)
    {
      return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint ? base.GetNextCaretPosition(visualColumn, direction, mode) : -1;
    }

    public override bool IsWhitespace(int visualColumn) => true;
  }

  private sealed class TabTextElement : VisualLineElement
  {
    internal readonly TextLine text;

    public TabTextElement(TextLine text)
      : base(2, 1)
    {
      this.text = text;
    }

    public override TextRun CreateTextRun(
      int startVisualColumn,
      ITextRunConstructionContext context)
    {
      if (startVisualColumn == this.VisualColumn)
        return (TextRun) new SingleCharacterElementGenerator.TabGlyphRun(this, (TextRunProperties) this.TextRunProperties);
      if (startVisualColumn == this.VisualColumn + 1)
        return (TextRun) new TextCharacters("\t", 0, 1, (TextRunProperties) this.TextRunProperties);
      throw new ArgumentOutOfRangeException(nameof (startVisualColumn));
    }

    public override int GetNextCaretPosition(
      int visualColumn,
      LogicalDirection direction,
      CaretPositioningMode mode)
    {
      return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint ? base.GetNextCaretPosition(visualColumn, direction, mode) : -1;
    }

    public override bool IsWhitespace(int visualColumn) => true;
  }

  private sealed class TabGlyphRun : TextEmbeddedObject
  {
    private readonly SingleCharacterElementGenerator.TabTextElement element;
    private TextRunProperties properties;

    public TabGlyphRun(
      SingleCharacterElementGenerator.TabTextElement element,
      TextRunProperties properties)
    {
      this.properties = properties != null ? properties : throw new ArgumentNullException(nameof (properties));
      this.element = element;
    }

    public override LineBreakCondition BreakBefore => LineBreakCondition.BreakPossible;

    public override LineBreakCondition BreakAfter => LineBreakCondition.BreakRestrained;

    public override bool HasFixedSize => true;

    public override CharacterBufferReference CharacterBufferReference
    {
      get => new CharacterBufferReference();
    }

    public override int Length => 1;

    public override TextRunProperties Properties => this.properties;

    public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
    {
      return new TextEmbeddedObjectMetrics(Math.Min(0.0, this.element.text.WidthIncludingTrailingWhitespace - 1.0), this.element.text.Height, this.element.text.Baseline);
    }

    public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
    {
      return new Rect(0.0, 0.0, Math.Min(0.0, this.element.text.WidthIncludingTrailingWhitespace - 1.0), this.element.text.Height);
    }

    public override void Draw(
      DrawingContext drawingContext,
      Point origin,
      bool rightToLeft,
      bool sideways)
    {
      origin.Y -= this.element.text.Baseline;
      this.element.text.Draw(drawingContext, origin, InvertAxes.None);
    }
  }

  private sealed class SpecialCharacterBoxElement(TextLine text) : FormattedTextElement(text, 1)
  {
    public override TextRun CreateTextRun(
      int startVisualColumn,
      ITextRunConstructionContext context)
    {
      return (TextRun) new SingleCharacterElementGenerator.SpecialCharacterTextRun((FormattedTextElement) this, (TextRunProperties) this.TextRunProperties);
    }
  }

  private sealed class SpecialCharacterTextRun(
    FormattedTextElement element,
    TextRunProperties properties) : FormattedTextRun(element, properties)
  {
    private static readonly SolidColorBrush darkGrayBrush = new SolidColorBrush(Color.FromArgb((byte) 200, (byte) 128 /*0x80*/, (byte) 128 /*0x80*/, (byte) 128 /*0x80*/));

    static SpecialCharacterTextRun()
    {
      SingleCharacterElementGenerator.SpecialCharacterTextRun.darkGrayBrush.Freeze();
    }

    public override void Draw(
      DrawingContext drawingContext,
      Point origin,
      bool rightToLeft,
      bool sideways)
    {
      Point origin1 = new Point(origin.X + 1.5, origin.Y);
      TextEmbeddedObjectMetrics embeddedObjectMetrics = base.Format(double.PositiveInfinity);
      Rect rectangle = new Rect(origin1.X - 0.5, origin1.Y - embeddedObjectMetrics.Baseline, embeddedObjectMetrics.Width + 2.0, embeddedObjectMetrics.Height);
      drawingContext.DrawRoundedRectangle((Brush) SingleCharacterElementGenerator.SpecialCharacterTextRun.darkGrayBrush, (Pen) null, rectangle, 2.5, 2.5);
      base.Draw(drawingContext, origin1, rightToLeft, sideways);
    }

    public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
    {
      TextEmbeddedObjectMetrics embeddedObjectMetrics = base.Format(remainingParagraphWidth);
      return new TextEmbeddedObjectMetrics(embeddedObjectMetrics.Width + 3.0, embeddedObjectMetrics.Height, embeddedObjectMetrics.Baseline);
    }

    public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
    {
      Rect boundingBox = base.ComputeBoundingBox(rightToLeft, sideways);
      boundingBox.Width += 3.0;
      return boundingBox;
    }
  }
}
