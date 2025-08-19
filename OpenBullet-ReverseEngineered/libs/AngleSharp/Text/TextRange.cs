// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.TextRange
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Diagnostics;

#nullable disable
namespace AngleSharp.Text;

[DebuggerStepThrough]
public struct TextRange(TextPosition start, TextPosition end) : 
  IEquatable<TextRange>,
  IComparable<TextRange>
{
  private readonly TextPosition _start = start;
  private readonly TextPosition _end = end;

  public TextPosition Start => this._start;

  public TextPosition End => this._end;

  public override string ToString() => $"({this._start}) -- ({this._end})";

  public override int GetHashCode()
  {
    TextPosition textPosition = this._end;
    int hashCode1 = textPosition.GetHashCode();
    textPosition = this._start;
    int hashCode2 = textPosition.GetHashCode();
    return hashCode1 ^ hashCode2;
  }

  public override bool Equals(object obj)
  {
    TextRange? nullable = obj as TextRange?;
    return nullable.HasValue && this.Equals(nullable.Value);
  }

  public bool Equals(TextRange other)
  {
    return this._start.Equals(other._start) && this._end.Equals(other._end);
  }

  public static bool operator >(TextRange a, TextRange b) => a._start > b._end;

  public static bool operator <(TextRange a, TextRange b) => a._end < b._start;

  public int CompareTo(TextRange other)
  {
    if (this > other)
      return 1;
    return other > this ? -1 : 0;
  }
}
