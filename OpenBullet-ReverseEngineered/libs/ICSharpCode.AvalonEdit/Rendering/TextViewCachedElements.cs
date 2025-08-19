// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.TextViewCachedElements
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class TextViewCachedElements : IDisposable
{
  private TextFormatter formatter;
  private Dictionary<string, TextLine> nonPrintableCharacterTexts;

  public TextLine GetTextForNonPrintableCharacter(string text, ITextRunConstructionContext context)
  {
    if (this.nonPrintableCharacterTexts == null)
      this.nonPrintableCharacterTexts = new Dictionary<string, TextLine>();
    TextLine printableCharacter;
    if (!this.nonPrintableCharacterTexts.TryGetValue(text, out printableCharacter))
    {
      VisualLineElementTextRunProperties properties = new VisualLineElementTextRunProperties(context.GlobalTextRunProperties);
      properties.SetForegroundBrush(context.TextView.NonPrintableCharacterBrush);
      if (this.formatter == null)
        this.formatter = TextFormatterFactory.Create((DependencyObject) context.TextView);
      printableCharacter = FormattedTextElement.PrepareText(this.formatter, text, (TextRunProperties) properties);
      this.nonPrintableCharacterTexts[text] = printableCharacter;
    }
    return printableCharacter;
  }

  public void Dispose()
  {
    if (this.nonPrintableCharacterTexts != null)
    {
      foreach (TextLine textLine in this.nonPrintableCharacterTexts.Values)
        textLine.Dispose();
    }
    if (this.formatter == null)
      return;
    this.formatter.Dispose();
  }
}
