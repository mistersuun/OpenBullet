// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineTextSource
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Globalization;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class VisualLineTextSource : TextSource, ITextRunConstructionContext
{
  private string cachedString;
  private int cachedStringOffset;

  public VisualLineTextSource(VisualLine visualLine) => this.VisualLine = visualLine;

  public VisualLine VisualLine { get; private set; }

  public TextView TextView { get; set; }

  public TextDocument Document { get; set; }

  public TextRunProperties GlobalTextRunProperties { get; set; }

  public override TextRun GetTextRun(int textSourceCharacterIndex)
  {
    try
    {
      foreach (VisualLineElement element in this.VisualLine.Elements)
      {
        if (textSourceCharacterIndex >= element.VisualColumn && textSourceCharacterIndex < element.VisualColumn + element.VisualLength)
        {
          int num = textSourceCharacterIndex - element.VisualColumn;
          TextRun textRun = element.CreateTextRun(textSourceCharacterIndex, (ITextRunConstructionContext) this);
          if (textRun == null)
            throw new ArgumentNullException(element.GetType().Name + ".CreateTextRun");
          if (textRun.Length == 0)
            throw new ArgumentException("The returned TextRun must not have length 0.", element.GetType().Name + ".Length");
          if (num + textRun.Length > element.VisualLength)
            throw new ArgumentException("The returned TextRun is too long.", element.GetType().Name + ".CreateTextRun");
          if (textRun is InlineObjectRun inlineObject)
          {
            inlineObject.VisualLine = this.VisualLine;
            this.VisualLine.hasInlineObjects = true;
            this.TextView.AddInlineObject(inlineObject);
          }
          return textRun;
        }
      }
      return this.TextView.Options.ShowEndOfLine && textSourceCharacterIndex == this.VisualLine.VisualLength ? this.CreateTextRunForNewLine() : (TextRun) new TextEndOfParagraph(1);
    }
    catch (Exception ex)
    {
      throw;
    }
  }

  private TextRun CreateTextRunForNewLine()
  {
    string text = "";
    DocumentLine lastDocumentLine = this.VisualLine.LastDocumentLine;
    if (lastDocumentLine.DelimiterLength == 2)
      text = "¶";
    else if (lastDocumentLine.DelimiterLength == 1)
    {
      switch (this.Document.GetCharAt(lastDocumentLine.Offset + lastDocumentLine.Length))
      {
        case '\n':
          text = "\\n";
          break;
        case '\r':
          text = "\\r";
          break;
        default:
          text = "?";
          break;
      }
    }
    return (TextRun) new FormattedTextRun(new FormattedTextElement(this.TextView.cachedElements.GetTextForNonPrintableCharacter(text, (ITextRunConstructionContext) this), 0), this.GlobalTextRunProperties);
  }

  public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(
    int textSourceCharacterIndexLimit)
  {
    try
    {
      foreach (VisualLineElement element in this.VisualLine.Elements)
      {
        if (textSourceCharacterIndexLimit > element.VisualColumn && textSourceCharacterIndexLimit <= element.VisualColumn + element.VisualLength)
        {
          TextSpan<CultureSpecificCharacterBufferRange> precedingText = element.GetPrecedingText(textSourceCharacterIndexLimit, (ITextRunConstructionContext) this);
          if (precedingText != null)
          {
            int num = textSourceCharacterIndexLimit - element.VisualColumn;
            if (precedingText.Length > num)
              throw new ArgumentException("The returned TextSpan is too long.", element.GetType().Name + ".GetPrecedingText");
            return precedingText;
          }
          break;
        }
      }
      CharacterBufferRange empty = CharacterBufferRange.Empty;
      return new TextSpan<CultureSpecificCharacterBufferRange>(empty.Length, new CultureSpecificCharacterBufferRange((CultureInfo) null, empty));
    }
    catch (Exception ex)
    {
      throw;
    }
  }

  public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(
    int textSourceCharacterIndex)
  {
    throw new NotSupportedException();
  }

  public StringSegment GetText(int offset, int length)
  {
    if (this.cachedString != null && offset >= this.cachedStringOffset && offset + length <= this.cachedStringOffset + this.cachedString.Length)
      return new StringSegment(this.cachedString, offset - this.cachedStringOffset, length);
    this.cachedStringOffset = offset;
    return new StringSegment(this.cachedString = this.Document.GetText(offset, length));
  }
}
