// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.RichTextModel
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public sealed class RichTextModel
{
  private List<int> stateChangeOffsets = new List<int>();
  private List<HighlightingColor> stateChanges = new List<HighlightingColor>();

  private int GetIndexForOffset(int offset)
  {
    int index = offset >= 0 ? this.stateChangeOffsets.BinarySearch(offset) : throw new ArgumentOutOfRangeException(nameof (offset));
    if (index < 0)
    {
      index = ~index;
      this.stateChanges.Insert(index, this.stateChanges[index - 1].Clone());
      this.stateChangeOffsets.Insert(index, offset);
    }
    return index;
  }

  private int GetIndexForOffsetUseExistingSegment(int offset)
  {
    int useExistingSegment = offset >= 0 ? this.stateChangeOffsets.BinarySearch(offset) : throw new ArgumentOutOfRangeException(nameof (offset));
    if (useExistingSegment < 0)
      useExistingSegment = ~useExistingSegment - 1;
    return useExistingSegment;
  }

  private int GetEnd(int index)
  {
    return index + 1 < this.stateChangeOffsets.Count ? this.stateChangeOffsets[index + 1] : int.MaxValue;
  }

  public RichTextModel()
  {
    this.stateChangeOffsets.Add(0);
    this.stateChanges.Add(new HighlightingColor());
  }

  internal RichTextModel(int[] stateChangeOffsets, HighlightingColor[] stateChanges)
  {
    this.stateChangeOffsets.AddRange((IEnumerable<int>) stateChangeOffsets);
    this.stateChanges.AddRange((IEnumerable<HighlightingColor>) stateChanges);
  }

  public void UpdateOffsets(TextChangeEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    this.UpdateOffsets(new Func<int, AnchorMovementType, int>(e.GetNewOffset));
  }

  public void UpdateOffsets(OffsetChangeMap change)
  {
    if (change == null)
      throw new ArgumentNullException(nameof (change));
    this.UpdateOffsets(new Func<int, AnchorMovementType, int>(change.GetNewOffset));
  }

  public void UpdateOffsets(OffsetChangeMapEntry change)
  {
    this.UpdateOffsets(new Func<int, AnchorMovementType, int>(change.GetNewOffset));
  }

  private void UpdateOffsets(Func<int, AnchorMovementType, int> updateOffset)
  {
    int index1 = 1;
    int index2 = 1;
    for (; index1 < this.stateChangeOffsets.Count; ++index1)
    {
      int num = updateOffset(this.stateChangeOffsets[index1], AnchorMovementType.Default);
      if (num == this.stateChangeOffsets[index2 - 1])
      {
        this.stateChanges[index2 - 1] = this.stateChanges[index1];
      }
      else
      {
        this.stateChangeOffsets[index2] = num;
        this.stateChanges[index2] = this.stateChanges[index1];
        ++index2;
      }
    }
    this.stateChangeOffsets.RemoveRange(index2, this.stateChangeOffsets.Count - index2);
    this.stateChanges.RemoveRange(index2, this.stateChanges.Count - index2);
  }

  internal void Append(int offset, int[] newOffsets, HighlightingColor[] newColors)
  {
    while (this.stateChangeOffsets.Count > 0 && this.stateChangeOffsets.Last<int>() <= offset)
    {
      this.stateChangeOffsets.RemoveAt(this.stateChangeOffsets.Count - 1);
      this.stateChanges.RemoveAt(this.stateChanges.Count - 1);
    }
    for (int index = 0; index < newOffsets.Length; ++index)
    {
      this.stateChangeOffsets.Add(offset + newOffsets[index]);
      this.stateChanges.Add(newColors[index]);
    }
  }

  public HighlightingColor GetHighlightingAt(int offset)
  {
    return this.stateChanges[this.GetIndexForOffsetUseExistingSegment(offset)].Clone();
  }

  public void ApplyHighlighting(int offset, int length, HighlightingColor color)
  {
    if (color == null || color.IsEmptyForMerge)
      return;
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].MergeWith(color);
  }

  public void SetHighlighting(int offset, int length, HighlightingColor color)
  {
    if (length <= 0)
      return;
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    this.stateChanges[indexForOffset1] = color != null ? color.Clone() : new HighlightingColor();
    this.stateChanges.RemoveRange(indexForOffset1 + 1, indexForOffset2 - (indexForOffset1 + 1));
    this.stateChangeOffsets.RemoveRange(indexForOffset1 + 1, indexForOffset2 - (indexForOffset1 + 1));
  }

  public void SetForeground(int offset, int length, HighlightingBrush brush)
  {
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].Foreground = brush;
  }

  public void SetBackground(int offset, int length, HighlightingBrush brush)
  {
    int indexForOffset1 = this.GetIndexForOffset(offset);
    int indexForOffset2 = this.GetIndexForOffset(offset + length);
    for (int index = indexForOffset1; index < indexForOffset2; ++index)
      this.stateChanges[index].Background = brush;
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

  public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
  {
    int index = this.GetIndexForOffsetUseExistingSegment(offset);
    int pos = offset;
    int endOffset = offset + length;
    while (pos < endOffset)
    {
      int endPos = Math.Min(endOffset, this.GetEnd(index));
      yield return new HighlightedSection()
      {
        Offset = pos,
        Length = endPos - pos,
        Color = this.stateChanges[index].Clone()
      };
      pos = endPos;
      ++index;
    }
  }

  public Run[] CreateRuns(ITextSource textSource)
  {
    Run[] runs = new Run[this.stateChanges.Count];
    for (int index = 0; index < runs.Length; ++index)
    {
      int stateChangeOffset = this.stateChangeOffsets[index];
      int num = index + 1 < this.stateChangeOffsets.Count ? this.stateChangeOffsets[index + 1] : textSource.TextLength;
      Run r = new Run(textSource.GetText(stateChangeOffset, num - stateChangeOffset));
      HighlightingColor stateChange = this.stateChanges[index];
      RichText.ApplyColorToTextElement((TextElement) r, stateChange);
      runs[index] = r;
    }
    return runs;
  }
}
