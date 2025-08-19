// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.FormattedTextRun
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class FormattedTextRun : TextEmbeddedObject
{
  private readonly FormattedTextElement element;
  private TextRunProperties properties;

  public FormattedTextRun(FormattedTextElement element, TextRunProperties properties)
  {
    if (element == null)
      throw new ArgumentNullException(nameof (element));
    this.properties = properties != null ? properties : throw new ArgumentNullException(nameof (properties));
    this.element = element;
  }

  public FormattedTextElement Element => this.element;

  public override LineBreakCondition BreakBefore => this.element.BreakBefore;

  public override LineBreakCondition BreakAfter => this.element.BreakAfter;

  public override bool HasFixedSize => true;

  public override CharacterBufferReference CharacterBufferReference
  {
    get => new CharacterBufferReference();
  }

  public override int Length => this.element.VisualLength;

  public override TextRunProperties Properties => this.properties;

  public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
  {
    FormattedText formattedText = this.element.formattedText;
    if (formattedText != null)
      return new TextEmbeddedObjectMetrics(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height, formattedText.Baseline);
    TextLine textLine = this.element.textLine;
    return new TextEmbeddedObjectMetrics(textLine.WidthIncludingTrailingWhitespace, textLine.Height, textLine.Baseline);
  }

  public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
  {
    FormattedText formattedText = this.element.formattedText;
    if (formattedText != null)
      return new Rect(0.0, 0.0, formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
    TextLine textLine = this.element.textLine;
    return new Rect(0.0, 0.0, textLine.WidthIncludingTrailingWhitespace, textLine.Height);
  }

  public override void Draw(
    DrawingContext drawingContext,
    Point origin,
    bool rightToLeft,
    bool sideways)
  {
    if (this.element.formattedText != null)
    {
      origin.Y -= this.element.formattedText.Baseline;
      drawingContext.DrawText(this.element.formattedText, origin);
    }
    else
    {
      origin.Y -= this.element.textLine.Baseline;
      this.element.textLine.Draw(drawingContext, origin, InvertAxes.None);
    }
  }
}
