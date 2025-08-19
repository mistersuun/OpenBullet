// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HtmlRichTextWriter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

internal class HtmlRichTextWriter : RichTextWriter
{
  private readonly TextWriter htmlWriter;
  private readonly HtmlOptions options;
  private Stack<string> endTagStack = new Stack<string>();
  private bool spaceNeedsEscaping = true;
  private bool hasSpace;
  private bool needIndentation = true;
  private int indentationLevel;
  private static readonly char[] specialChars = new char[4]
  {
    ' ',
    '\t',
    '\r',
    '\n'
  };

  public HtmlRichTextWriter(TextWriter htmlWriter, HtmlOptions options = null)
  {
    this.htmlWriter = htmlWriter != null ? htmlWriter : throw new ArgumentNullException(nameof (htmlWriter));
    this.options = options ?? new HtmlOptions();
  }

  public override Encoding Encoding => this.htmlWriter.Encoding;

  public override void Flush()
  {
    this.FlushSpace(true);
    this.htmlWriter.Flush();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
      this.FlushSpace(true);
    base.Dispose(disposing);
  }

  private void FlushSpace(bool nextIsWhitespace)
  {
    if (!this.hasSpace)
      return;
    if (this.spaceNeedsEscaping || nextIsWhitespace)
      this.htmlWriter.Write("&nbsp;");
    else
      this.htmlWriter.Write(' ');
    this.hasSpace = false;
    this.spaceNeedsEscaping = true;
  }

  private void WriteIndentation()
  {
    if (!this.needIndentation)
      return;
    for (int index = 0; index < this.indentationLevel; ++index)
      this.WriteChar('\t');
    this.needIndentation = false;
  }

  public override void Write(char value)
  {
    this.WriteIndentation();
    this.WriteChar(value);
  }

  private void WriteChar(char c)
  {
    bool nextIsWhitespace = char.IsWhiteSpace(c);
    this.FlushSpace(nextIsWhitespace);
    switch (c)
    {
      case '\t':
        for (int index = 0; index < this.options.TabSize; ++index)
          this.htmlWriter.Write("&nbsp;");
        goto case '\r';
      case '\n':
        this.htmlWriter.Write("<br/>");
        this.needIndentation = true;
        goto case '\r';
      case '\r':
        if (c == ' ')
          break;
        this.spaceNeedsEscaping = nextIsWhitespace;
        break;
      case ' ':
        if (this.spaceNeedsEscaping)
        {
          this.htmlWriter.Write("&nbsp;");
          goto case '\r';
        }
        this.hasSpace = true;
        goto case '\r';
      default:
        WebUtility.HtmlEncode(c.ToString(), this.htmlWriter);
        goto case '\r';
    }
  }

  public override void Write(string value)
  {
    int num1 = 0;
    do
    {
      int num2 = value.IndexOfAny(HtmlRichTextWriter.specialChars, num1);
      if (num2 < 0)
      {
        this.WriteSimpleString(value.Substring(num1));
        break;
      }
      if (num2 > num1)
        this.WriteSimpleString(value.Substring(num1, num2 - num1));
      this.WriteChar(value[num1]);
      num1 = num2 + 1;
    }
    while (num1 < value.Length);
  }

  private void WriteIndentationAndSpace()
  {
    this.WriteIndentation();
    this.FlushSpace(false);
  }

  private void WriteSimpleString(string value)
  {
    if (value.Length == 0)
      return;
    this.WriteIndentationAndSpace();
    WebUtility.HtmlEncode(value, this.htmlWriter);
  }

  public override void Indent() => ++this.indentationLevel;

  public override void Unindent()
  {
    if (this.indentationLevel == 0)
      throw new NotSupportedException();
    --this.indentationLevel;
  }

  protected override void BeginUnhandledSpan() => this.endTagStack.Push((string) null);

  public override void EndSpan() => this.htmlWriter.Write(this.endTagStack.Pop());

  public override void BeginSpan(Color foregroundColor)
  {
    this.BeginSpan(new HighlightingColor()
    {
      Foreground = (HighlightingBrush) new SimpleHighlightingBrush(foregroundColor)
    });
  }

  public override void BeginSpan(FontFamily fontFamily) => this.BeginUnhandledSpan();

  public override void BeginSpan(FontStyle fontStyle)
  {
    this.BeginSpan(new HighlightingColor()
    {
      FontStyle = new FontStyle?(fontStyle)
    });
  }

  public override void BeginSpan(FontWeight fontWeight)
  {
    this.BeginSpan(new HighlightingColor()
    {
      FontWeight = new FontWeight?(fontWeight)
    });
  }

  public override void BeginSpan(HighlightingColor highlightingColor)
  {
    this.WriteIndentationAndSpace();
    if (this.options.ColorNeedsSpanForStyling(highlightingColor))
    {
      this.htmlWriter.Write("<span");
      this.options.WriteStyleAttributeForColor(this.htmlWriter, highlightingColor);
      this.htmlWriter.Write('>');
      this.endTagStack.Push("</span>");
    }
    else
      this.endTagStack.Push((string) null);
  }

  public override void BeginHyperlinkSpan(Uri uri)
  {
    this.WriteIndentationAndSpace();
    this.htmlWriter.Write($"<a href=\"{WebUtility.HtmlEncode(uri.ToString())}\">");
    this.endTagStack.Push("</a>");
  }
}
