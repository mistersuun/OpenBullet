// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.RichText
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class RichText
{
  public static readonly RichText Empty = new RichText(string.Empty);
  private readonly string text;
  internal readonly int[] stateChangeOffsets;
  internal readonly HighlightingColor[] stateChanges;

  public RichText(string text, RichTextModel model = null)
  {
    this.text = text != null ? text : throw new ArgumentNullException(nameof (text));
    if (model != null)
    {
      HighlightedSection[] array = model.GetHighlightedSections(0, text.Length).ToArray<HighlightedSection>();
      this.stateChangeOffsets = new int[array.Length];
      this.stateChanges = new HighlightingColor[array.Length];
      for (int index = 0; index < array.Length; ++index)
      {
        this.stateChangeOffsets[index] = array[index].Offset;
        this.stateChanges[index] = array[index].Color;
      }
    }
    else
    {
      this.stateChangeOffsets = new int[1];
      this.stateChanges = new HighlightingColor[1]
      {
        HighlightingColor.Empty
      };
    }
  }

  internal RichText(string text, int[] offsets, HighlightingColor[] states)
  {
    this.text = text;
    this.stateChangeOffsets = offsets;
    this.stateChanges = states;
  }

  public string Text => this.text;

  public int Length => this.text.Length;

  private int GetIndexForOffset(int offset)
  {
    if (offset < 0 || offset > this.text.Length)
      throw new ArgumentOutOfRangeException(nameof (offset));
    int indexForOffset = Array.BinarySearch<int>(this.stateChangeOffsets, offset);
    if (indexForOffset < 0)
      indexForOffset = ~indexForOffset - 1;
    return indexForOffset;
  }

  private int GetEnd(int index)
  {
    return index + 1 < this.stateChangeOffsets.Length ? this.stateChangeOffsets[index + 1] : this.text.Length;
  }

  public HighlightingColor GetHighlightingAt(int offset)
  {
    return this.stateChanges[this.GetIndexForOffset(offset)];
  }

  public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
  {
    int index = this.GetIndexForOffset(offset);
    int pos = offset;
    int endOffset = offset + length;
    while (pos < endOffset)
    {
      int endPos = Math.Min(endOffset, this.GetEnd(index));
      yield return new HighlightedSection()
      {
        Offset = pos,
        Length = endPos - pos,
        Color = this.stateChanges[index]
      };
      pos = endPos;
      ++index;
    }
  }

  public RichTextModel ToRichTextModel()
  {
    return new RichTextModel(this.stateChangeOffsets, ((IEnumerable<HighlightingColor>) this.stateChanges).Select<HighlightingColor, HighlightingColor>((Func<HighlightingColor, HighlightingColor>) (ch => ch.Clone())).ToArray<HighlightingColor>());
  }

  public override string ToString() => this.text;

  public Run[] CreateRuns()
  {
    Run[] runs = new Run[this.stateChanges.Length];
    for (int index = 0; index < runs.Length; ++index)
    {
      int stateChangeOffset = this.stateChangeOffsets[index];
      int num = index + 1 < this.stateChangeOffsets.Length ? this.stateChangeOffsets[index + 1] : this.text.Length;
      Run r = new Run(this.text.Substring(stateChangeOffset, num - stateChangeOffset));
      HighlightingColor stateChange = this.stateChanges[index];
      RichText.ApplyColorToTextElement((TextElement) r, stateChange);
      runs[index] = r;
    }
    return runs;
  }

  internal static void ApplyColorToTextElement(TextElement r, HighlightingColor state)
  {
    if (state.Foreground != null)
      r.Foreground = state.Foreground.GetBrush((ITextRunConstructionContext) null);
    if (state.Background != null)
      r.Background = state.Background.GetBrush((ITextRunConstructionContext) null);
    if (state.FontWeight.HasValue)
      r.FontWeight = state.FontWeight.Value;
    if (!state.FontStyle.HasValue)
      return;
    r.FontStyle = state.FontStyle.Value;
  }

  public string ToHtml(HtmlOptions options = null)
  {
    StringWriter htmlWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
    using (HtmlRichTextWriter htmlRichTextWriter = new HtmlRichTextWriter((TextWriter) htmlWriter, options))
      htmlRichTextWriter.Write(this);
    return htmlWriter.ToString();
  }

  public string ToHtml(int offset, int length, HtmlOptions options = null)
  {
    StringWriter htmlWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
    using (HtmlRichTextWriter htmlRichTextWriter = new HtmlRichTextWriter((TextWriter) htmlWriter, options))
      htmlRichTextWriter.Write(this, offset, length);
    return htmlWriter.ToString();
  }

  public RichText Substring(int offset, int length)
  {
    if (offset == 0 && length == this.Length)
      return this;
    string text = this.text.Substring(offset, length);
    RichTextModel richTextModel = this.ToRichTextModel();
    OffsetChangeMap change = new OffsetChangeMap(2);
    change.Add(new OffsetChangeMapEntry(offset + length, this.text.Length - offset - length, 0));
    change.Add(new OffsetChangeMapEntry(0, offset, 0));
    richTextModel.UpdateOffsets(change);
    return new RichText(text, richTextModel);
  }

  public static RichText Concat(params RichText[] texts)
  {
    if (texts == null || texts.Length == 0)
      return RichText.Empty;
    if (texts.Length == 1)
      return texts[0];
    string text = string.Concat(((IEnumerable<RichText>) texts).Select<RichText, string>((Func<RichText, string>) (txt => txt.text)));
    RichTextModel richTextModel = texts[0].ToRichTextModel();
    int length = texts[0].Length;
    for (int index = 1; index < texts.Length; ++index)
    {
      richTextModel.Append(length, texts[index].stateChangeOffsets, texts[index].stateChanges);
      length += texts[index].Length;
    }
    return new RichText(text, richTextModel);
  }

  public static RichText operator +(RichText a, RichText b) => RichText.Concat(a, b);

  public static implicit operator RichText(string text)
  {
    return text != null ? new RichText(text) : (RichText) null;
  }
}
