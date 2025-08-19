// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceLocation
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Globalization;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public readonly struct SourceLocation : IEquatable<SourceLocation>
{
  public static readonly SourceLocation None = new SourceLocation(0, 16707566 /*0xFEEFEE*/, 0, true);
  public static readonly SourceLocation Invalid = new SourceLocation(0, 0, 0, true);
  public static readonly SourceLocation MinValue = new SourceLocation(0, 1, 1);

  public SourceLocation(int index, int line, int column)
  {
    SourceLocation.ValidateLocation(index, line, column);
    this.Index = index;
    this.Line = line;
    this.Column = column;
  }

  private static void ValidateLocation(int index, int line, int column)
  {
    if (index < 0)
      throw SourceLocation.ErrorOutOfRange((object) nameof (index), (object) 0);
    if (line < 1)
      throw SourceLocation.ErrorOutOfRange((object) nameof (line), (object) 1);
    if (column < 1)
      throw SourceLocation.ErrorOutOfRange((object) nameof (column), (object) 1);
  }

  private static Exception ErrorOutOfRange(object p0, object p1)
  {
    return (Exception) new ArgumentOutOfRangeException($"{p0} must be greater than or equal to {p1}");
  }

  private SourceLocation(int index, int line, int column, bool noChecks)
  {
    this.Index = index;
    this.Line = line;
    this.Column = column;
  }

  public int Index { get; }

  public int Line { get; }

  public int Column { get; }

  public static bool operator ==(SourceLocation left, SourceLocation right)
  {
    return left.Index == right.Index && left.Line == right.Line && left.Column == right.Column;
  }

  public static bool operator !=(SourceLocation left, SourceLocation right)
  {
    return left.Index != right.Index || left.Line != right.Line || left.Column != right.Column;
  }

  public static bool operator <(SourceLocation left, SourceLocation right)
  {
    return left.Index < right.Index;
  }

  public static bool operator >(SourceLocation left, SourceLocation right)
  {
    return left.Index > right.Index;
  }

  public static bool operator <=(SourceLocation left, SourceLocation right)
  {
    return left.Index <= right.Index;
  }

  public static bool operator >=(SourceLocation left, SourceLocation right)
  {
    return left.Index >= right.Index;
  }

  public static int Compare(SourceLocation left, SourceLocation right)
  {
    if (left < right)
      return -1;
    return left > right ? 1 : 0;
  }

  public bool IsValid => this.Line != 0 && this.Column != 0;

  public bool Equals(SourceLocation other)
  {
    return other.Index == this.Index && other.Line == this.Line && other.Column == this.Column;
  }

  public override bool Equals(object obj) => obj is SourceLocation other && this.Equals(other);

  public override int GetHashCode() => this.Line << 16 /*0x10*/ ^ this.Column;

  public override string ToString() => $"({(object) this.Line},{(object) this.Column})";

  internal string ToDebugString()
  {
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "({0},{1},{2})", (object) this.Index, (object) this.Line, (object) this.Column);
  }
}
