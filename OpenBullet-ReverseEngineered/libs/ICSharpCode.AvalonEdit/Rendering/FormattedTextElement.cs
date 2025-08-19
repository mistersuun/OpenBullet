// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.FormattedTextElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class FormattedTextElement : VisualLineElement
{
  internal readonly FormattedText formattedText;
  internal string text;
  internal TextLine textLine;

  public FormattedTextElement(string text, int documentLength)
    : base(1, documentLength)
  {
    this.text = text != null ? text : throw new ArgumentNullException(nameof (text));
    this.BreakBefore = LineBreakCondition.BreakPossible;
    this.BreakAfter = LineBreakCondition.BreakPossible;
  }

  public FormattedTextElement(TextLine text, int documentLength)
    : base(1, documentLength)
  {
    this.textLine = text != null ? text : throw new ArgumentNullException(nameof (text));
    this.BreakBefore = LineBreakCondition.BreakPossible;
    this.BreakAfter = LineBreakCondition.BreakPossible;
  }

  public FormattedTextElement(FormattedText text, int documentLength)
    : base(1, documentLength)
  {
    this.formattedText = text != null ? text : throw new ArgumentNullException(nameof (text));
    this.BreakBefore = LineBreakCondition.BreakPossible;
    this.BreakAfter = LineBreakCondition.BreakPossible;
  }

  public LineBreakCondition BreakBefore { get; set; }

  public LineBreakCondition BreakAfter { get; set; }

  public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
  {
    if (this.textLine == null)
    {
      this.textLine = FormattedTextElement.PrepareText(TextFormatterFactory.Create((DependencyObject) context.TextView), this.text, (System.Windows.Media.TextFormatting.TextRunProperties) this.TextRunProperties);
      this.text = (string) null;
    }
    return (TextRun) new FormattedTextRun(this, (System.Windows.Media.TextFormatting.TextRunProperties) this.TextRunProperties);
  }

  public static TextLine PrepareText(
    TextFormatter formatter,
    string text,
    System.Windows.Media.TextFormatting.TextRunProperties properties)
  {
    if (formatter == null)
      throw new ArgumentNullException(nameof (formatter));
    if (text == null)
      throw new ArgumentNullException(nameof (text));
    if (properties == null)
      throw new ArgumentNullException(nameof (properties));
    return formatter.FormatLine((TextSource) new SimpleTextSource(text, properties), 0, 32000.0, (TextParagraphProperties) new VisualLineTextParagraphProperties()
    {
      defaultTextRunProperties = properties,
      textWrapping = TextWrapping.NoWrap,
      tabSize = 40.0
    }, (TextLineBreak) null);
  }
}
