// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.SimpleSegment
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal struct SimpleSegment : IEquatable<SimpleSegment>, ISegment
{
  public static readonly SimpleSegment Invalid = new SimpleSegment(-1, -1);
  public readonly int Offset;
  public readonly int Length;

  public static SimpleSegment GetOverlap(ISegment segment1, ISegment segment2)
  {
    int offset = Math.Max(segment1.Offset, segment2.Offset);
    int num = Math.Min(segment1.EndOffset, segment2.EndOffset);
    return num < offset ? SimpleSegment.Invalid : new SimpleSegment(offset, num - offset);
  }

  int ISegment.Offset => this.Offset;

  int ISegment.Length => this.Length;

  public int EndOffset => this.Offset + this.Length;

  public SimpleSegment(int offset, int length)
  {
    this.Offset = offset;
    this.Length = length;
  }

  public SimpleSegment(ISegment segment)
  {
    this.Offset = segment.Offset;
    this.Length = segment.Length;
  }

  public override int GetHashCode() => this.Offset + 10301 * this.Length;

  public override bool Equals(object obj) => obj is SimpleSegment other && this.Equals(other);

  public bool Equals(SimpleSegment other)
  {
    return this.Offset == other.Offset && this.Length == other.Length;
  }

  public static bool operator ==(SimpleSegment left, SimpleSegment right) => left.Equals(right);

  public static bool operator !=(SimpleSegment left, SimpleSegment right) => !left.Equals(right);

  public override string ToString()
  {
    return $"[Offset={this.Offset.ToString((IFormatProvider) CultureInfo.InvariantCulture)}, Length={this.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture)}]";
  }
}
