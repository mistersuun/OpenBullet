// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightedInlineBuilder
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Obsolete("Use RichText / RichTextModel instead")]
public sealed class HighlightedInlineBuilder
{
  private readonly string text;
  private List<int> stateChangeOffsets = new List<int>();
  private List<HighlightingColor> stateChanges = new List<HighlightingColor>();

  private static HighlightingBrush MakeBrush(Brush b)
  {
    return b is SolidColorBrush brush ? (HighlightingBrush) new SimpleHighlightingBrush(brush) : (HighlightingBrush) null;
  }

  private int GetIndexForOffset(int offset)
  {
    if (offset < 0 || offset > this.text.Length)
      throw new ArgumentOutOfRangeException(nameof (offset));
    int index = this.stateChangeOffsets.BinarySearch(offset);
    if (index < 0)
    {
      index = ~index;
      if (offset < this.text.Length)
      {
        this.stateChanges.Insert(index, this.stateChanges[index - 1].Clone());
        this.stateChangeOffsets.Insert(index, offset);
      }
    }
    return index;
  }

  public HighlightedInlineBuilder(string text)
  {
    this.text = text != null ? text : throw new ArgumentNullException(nameof (text));
    this.stateChangeOffsets.Add(0);
    this.stateChanges.Add(new HighlightingColor());
  }

  public HighlightedInlineBuilder(RichText text)
  {
    this.text = text != null ? text.Text : throw new ArgumentNullException(nameof (text));
    this.stateChangeOffsets.AddRange((IEnumerable<int>) text.stateChangeOffsets);
    this.stateChanges.AddRange((IEnumerable<HighlightingColor>) text.stateChanges);
  }

  private HighlightedInlineBuilder(string text, List<int> offsets, List<HighlightingColor> states)
  {
    this.text = text;
    this.stateChangeOffsets = offsets;
    this.stateChanges = states;
  }

  public string Text => this.text;

  public void SetHighlighting(int offset, int length, HighlightingColor color)
  {
    if (color == null)
      throw new ArgumentNullException(nameof (color));
    if (color.Foreground == null && color.Background == null && !color.FontStyle.HasValue && !color.FontWeight.HasValue && !color.Underline.HasValue)
      return;
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].MergeWith(color);
  }

  public void SetForeground(int offset, int length, Brush brush)
  {
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    HighlightingBrush highlightingBrush = HighlightedInlineBuilder.MakeBrush(brush);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].Foreground = highlightingBrush;
  }

  public void SetBackground(int offset, int length, Brush brush)
  {
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    HighlightingBrush highlightingBrush = HighlightedInlineBuilder.MakeBrush(brush);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].Background = highlightingBrush;
  }

  public void SetFontWeight(int offset, int length, FontWeight weight)
  {
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].FontWeight = new FontWeight?(weight);
  }

  public void SetFontStyle(int offset, int length, FontStyle style)
  {
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].FontStyle = new FontStyle?(style);
  }

  public Run[] CreateRuns() => this.ToRichText().CreateRuns();

  public RichText ToRichText()
  {
    return new RichText(this.text, this.stateChangeOffsets.ToArray(), this.stateChanges.Select<HighlightingColor, HighlightingColor>(new Func<HighlightingColor, HighlightingColor>(FreezableHelper.GetFrozenClone<HighlightingColor>)).ToArray<HighlightingColor>());
  }

  public HighlightedInlineBuilder Clone()
  {
    return new HighlightedInlineBuilder(this.text, this.stateChangeOffsets.ToList<int>(), this.stateChanges.Select<HighlightingColor, HighlightingColor>((Func<HighlightingColor, HighlightingColor>) (sc => sc.Clone())).ToList<HighlightingColor>());
  }
}
