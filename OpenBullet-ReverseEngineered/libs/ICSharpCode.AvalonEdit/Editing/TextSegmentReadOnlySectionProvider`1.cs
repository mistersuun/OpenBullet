// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.TextSegmentReadOnlySectionProvider`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public class TextSegmentReadOnlySectionProvider<T> : IReadOnlySectionProvider where T : TextSegment
{
  private readonly TextSegmentCollection<T> segments;

  public TextSegmentCollection<T> Segments => this.segments;

  public TextSegmentReadOnlySectionProvider(TextDocument textDocument)
  {
    this.segments = new TextSegmentCollection<T>(textDocument);
  }

  public TextSegmentReadOnlySectionProvider(TextSegmentCollection<T> segments)
  {
    this.segments = segments != null ? segments : throw new ArgumentNullException(nameof (segments));
  }

  public virtual bool CanInsert(int offset)
  {
    foreach (T obj in this.segments.FindSegmentsContaining(offset))
    {
      TextSegment textSegment = (TextSegment) obj;
      if (textSegment.StartOffset < offset && offset < textSegment.EndOffset)
        return false;
    }
    return true;
  }

  public virtual IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
  {
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    if (segment.Length == 0 && this.CanInsert(segment.Offset))
    {
      yield return segment;
    }
    else
    {
      int readonlyUntil = segment.Offset;
      foreach (T overlappingSegment in this.segments.FindOverlappingSegments(segment))
      {
        TextSegment ts = (TextSegment) overlappingSegment;
        int start = ts.StartOffset;
        int end = start + ts.Length;
        if (start > readonlyUntil)
          yield return (ISegment) new SimpleSegment(readonlyUntil, start - readonlyUntil);
        if (end > readonlyUntil)
          readonlyUntil = end;
      }
      int endOffset = segment.EndOffset;
      if (readonlyUntil < endOffset)
        yield return (ISegment) new SimpleSegment(readonlyUntil, endOffset - readonlyUntil);
    }
  }
}
