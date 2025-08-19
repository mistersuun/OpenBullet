// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.SelectionSegment
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public class SelectionSegment : ISegment
{
  private readonly int startOffset;
  private readonly int endOffset;
  private readonly int startVC;
  private readonly int endVC;

  public SelectionSegment(int startOffset, int endOffset)
  {
    this.startOffset = Math.Min(startOffset, endOffset);
    this.endOffset = Math.Max(startOffset, endOffset);
    this.startVC = this.endVC = -1;
  }

  public SelectionSegment(int startOffset, int startVC, int endOffset, int endVC)
  {
    if (startOffset < endOffset || startOffset == endOffset && startVC <= endVC)
    {
      this.startOffset = startOffset;
      this.startVC = startVC;
      this.endOffset = endOffset;
      this.endVC = endVC;
    }
    else
    {
      this.startOffset = endOffset;
      this.startVC = endVC;
      this.endOffset = startOffset;
      this.endVC = startVC;
    }
  }

  public int StartOffset => this.startOffset;

  public int EndOffset => this.endOffset;

  public int StartVisualColumn => this.startVC;

  public int EndVisualColumn => this.endVC;

  int ISegment.Offset => this.startOffset;

  public int Length => this.endOffset - this.startOffset;

  public override string ToString()
  {
    return $"[SelectionSegment StartOffset={this.startOffset}, EndOffset={this.endOffset}, StartVC={this.startVC}, EndVC={this.endVC}]";
  }
}
