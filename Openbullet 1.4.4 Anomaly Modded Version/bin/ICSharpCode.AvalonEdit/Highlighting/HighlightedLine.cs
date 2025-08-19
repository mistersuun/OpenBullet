// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightedLine
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightedLine
{
  public HighlightedLine(IDocument document, IDocumentLine documentLine)
  {
    this.Document = document != null ? document : throw new ArgumentNullException(nameof (document));
    this.DocumentLine = documentLine;
    this.Sections = (IList<HighlightedSection>) new NullSafeCollection<HighlightedSection>();
  }

  public IDocument Document { get; private set; }

  public IDocumentLine DocumentLine { get; private set; }

  public IList<HighlightedSection> Sections { get; private set; }

  public void ValidateInvariants()
  {
    HighlightedLine highlightedLine = this;
    int offset = highlightedLine.DocumentLine.Offset;
    int endOffset = highlightedLine.DocumentLine.EndOffset;
    for (int index1 = 0; index1 < highlightedLine.Sections.Count; ++index1)
    {
      HighlightedSection section1 = highlightedLine.Sections[index1];
      if (section1.Offset < offset || section1.Length < 0 || section1.Offset + section1.Length > endOffset)
        throw new InvalidOperationException("Section is outside line bounds");
      for (int index2 = index1 + 1; index2 < highlightedLine.Sections.Count; ++index2)
      {
        HighlightedSection section2 = highlightedLine.Sections[index2];
        if (section2.Offset < section1.Offset + section1.Length && (section2.Offset < section1.Offset || section2.Offset + section2.Length > section1.Offset + section1.Length))
          throw new InvalidOperationException("Sections are overlapping or incorrectly sorted.");
      }
    }
  }

  public void MergeWith(HighlightedLine additionalLine)
  {
    if (additionalLine == null)
      return;
    int index = 0;
    Stack<int> source = new Stack<int>();
    int endOffset = this.DocumentLine.EndOffset;
    source.Push(endOffset);
    foreach (HighlightedSection section1 in (IEnumerable<HighlightedSection>) additionalLine.Sections)
    {
      int offset = section1.Offset;
      for (; index < this.Sections.Count; ++index)
      {
        HighlightedSection section2 = this.Sections[index];
        if (section1.Offset >= section2.Offset)
        {
          while (section2.Offset > source.Peek())
            source.Pop();
          source.Push(section2.Offset + section2.Length);
        }
        else
          break;
      }
      Stack<int> insertionStack = new Stack<int>(source.Reverse<int>());
      int pos;
      for (pos = index; pos < this.Sections.Count; ++pos)
      {
        HighlightedSection section3 = this.Sections[pos];
        if (section1.Offset + section1.Length > section3.Offset)
        {
          this.Insert(ref pos, ref offset, section3.Offset, section1.Color, insertionStack);
          while (section3.Offset > insertionStack.Peek())
            insertionStack.Pop();
          insertionStack.Push(section3.Offset + section3.Length);
        }
        else
          break;
      }
      this.Insert(ref pos, ref offset, section1.Offset + section1.Length, section1.Color, insertionStack);
    }
  }

  private void Insert(
    ref int pos,
    ref int newSectionStart,
    int insertionEndPos,
    HighlightingColor color,
    Stack<int> insertionStack)
  {
    if (newSectionStart >= insertionEndPos)
      return;
    while (insertionStack.Peek() <= newSectionStart)
      insertionStack.Pop();
    while (insertionStack.Peek() < insertionEndPos)
    {
      int num = insertionStack.Pop();
      if (num > newSectionStart)
      {
        this.Sections.Insert(pos++, new HighlightedSection()
        {
          Offset = newSectionStart,
          Length = num - newSectionStart,
          Color = color
        });
        newSectionStart = num;
      }
    }
    if (insertionEndPos <= newSectionStart)
      return;
    this.Sections.Insert(pos++, new HighlightedSection()
    {
      Offset = newSectionStart,
      Length = insertionEndPos - newSectionStart,
      Color = color
    });
    newSectionStart = insertionEndPos;
  }

  internal void WriteTo(RichTextWriter writer)
  {
    int offset = this.DocumentLine.Offset;
    this.WriteTo(writer, offset, offset + this.DocumentLine.Length);
  }

  internal void WriteTo(RichTextWriter writer, int startOffset, int endOffset)
  {
    if (writer == null)
      throw new ArgumentNullException(nameof (writer));
    int offset = this.DocumentLine.Offset;
    int num1 = offset + this.DocumentLine.Length;
    if (startOffset < offset || startOffset > num1)
      throw new ArgumentOutOfRangeException(nameof (startOffset), (object) startOffset, $"Value must be between {(object) offset} and {(object) num1}");
    if (endOffset < startOffset || endOffset > num1)
      throw new ArgumentOutOfRangeException(nameof (endOffset), (object) endOffset, "Value must be between startOffset and " + (object) num1);
    ISegment segment2 = (ISegment) new SimpleSegment(startOffset, endOffset - startOffset);
    List<HighlightedLine.HtmlElement> htmlElementList = new List<HighlightedLine.HtmlElement>();
    for (int index = 0; index < this.Sections.Count; ++index)
    {
      HighlightedSection section = this.Sections[index];
      if (SimpleSegment.GetOverlap((ISegment) section, segment2).Length > 0)
      {
        htmlElementList.Add(new HighlightedLine.HtmlElement(section.Offset, index, false, section.Color));
        htmlElementList.Add(new HighlightedLine.HtmlElement(section.Offset + section.Length, index, true, section.Color));
      }
    }
    htmlElementList.Sort();
    IDocument document = this.Document;
    int num2 = startOffset;
    foreach (HighlightedLine.HtmlElement htmlElement in htmlElementList)
    {
      int val2 = Math.Min(htmlElement.Offset, endOffset);
      if (val2 > startOffset)
        document.WriteTextTo((TextWriter) writer, num2, val2 - num2);
      num2 = Math.Max(num2, val2);
      if (htmlElement.IsEnd)
        writer.EndSpan();
      else
        writer.BeginSpan(htmlElement.Color);
    }
    document.WriteTextTo((TextWriter) writer, num2, endOffset - num2);
  }

  public string ToHtml(HtmlOptions options = null)
  {
    StringWriter htmlWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
    using (HtmlRichTextWriter writer = new HtmlRichTextWriter((TextWriter) htmlWriter, options))
      this.WriteTo((RichTextWriter) writer);
    return htmlWriter.ToString();
  }

  public string ToHtml(int startOffset, int endOffset, HtmlOptions options = null)
  {
    StringWriter htmlWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
    using (HtmlRichTextWriter writer = new HtmlRichTextWriter((TextWriter) htmlWriter, options))
      this.WriteTo((RichTextWriter) writer, startOffset, endOffset);
    return htmlWriter.ToString();
  }

  public override string ToString() => $"[{this.GetType().Name} {this.ToHtml()}]";

  [Obsolete("Use ToRichText() instead")]
  public HighlightedInlineBuilder ToInlineBuilder()
  {
    HighlightedInlineBuilder inlineBuilder = new HighlightedInlineBuilder(this.Document.GetText((ISegment) this.DocumentLine));
    int offset = this.DocumentLine.Offset;
    foreach (HighlightedSection section in (IEnumerable<HighlightedSection>) this.Sections)
      inlineBuilder.SetHighlighting(section.Offset - offset, section.Length, section.Color);
    return inlineBuilder;
  }

  public RichTextModel ToRichTextModel()
  {
    RichTextModel richTextModel = new RichTextModel();
    int offset = this.DocumentLine.Offset;
    foreach (HighlightedSection section in (IEnumerable<HighlightedSection>) this.Sections)
      richTextModel.ApplyHighlighting(section.Offset - offset, section.Length, section.Color);
    return richTextModel;
  }

  public RichText ToRichText()
  {
    return new RichText(this.Document.GetText((ISegment) this.DocumentLine), this.ToRichTextModel());
  }

  private sealed class HtmlElement : IComparable<HighlightedLine.HtmlElement>
  {
    internal readonly int Offset;
    internal readonly int Nesting;
    internal readonly bool IsEnd;
    internal readonly HighlightingColor Color;

    public HtmlElement(int offset, int nesting, bool isEnd, HighlightingColor color)
    {
      this.Offset = offset;
      this.Nesting = nesting;
      this.IsEnd = isEnd;
      this.Color = color;
    }

    public int CompareTo(HighlightedLine.HtmlElement other)
    {
      int num = this.Offset.CompareTo(other.Offset);
      if (num != 0)
        return num;
      return this.IsEnd != other.IsEnd ? (this.IsEnd ? -1 : 1) : (this.IsEnd ? other.Nesting.CompareTo(this.Nesting) : this.Nesting.CompareTo(other.Nesting));
    }
  }
}
