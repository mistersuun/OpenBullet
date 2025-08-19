// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextLocation
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

[TypeConverter(typeof (TextLocationConverter))]
[Serializable]
public struct TextLocation(int line, int column) : 
  IComparable<TextLocation>,
  IEquatable<TextLocation>
{
  public static readonly TextLocation Empty = new TextLocation(0, 0);
  private readonly int column = column;
  private readonly int line = line;

  public int Line => this.line;

  public int Column => this.column;

  public bool IsEmpty => this.column <= 0 && this.line <= 0;

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(Line {1}, Col {0})", (object) this.column, (object) this.line);
  }

  public override int GetHashCode() => 191 * this.column.GetHashCode() ^ this.line.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is TextLocation textLocation && textLocation == this;
  }

  public bool Equals(TextLocation other) => this == other;

  public static bool operator ==(TextLocation left, TextLocation right)
  {
    return left.column == right.column && left.line == right.line;
  }

  public static bool operator !=(TextLocation left, TextLocation right)
  {
    return left.column != right.column || left.line != right.line;
  }

  public static bool operator <(TextLocation left, TextLocation right)
  {
    if (left.line < right.line)
      return true;
    return left.line == right.line && left.column < right.column;
  }

  public static bool operator >(TextLocation left, TextLocation right)
  {
    if (left.line > right.line)
      return true;
    return left.line == right.line && left.column > right.column;
  }

  public static bool operator <=(TextLocation left, TextLocation right) => !(left > right);

  public static bool operator >=(TextLocation left, TextLocation right) => !(left < right);

  public int CompareTo(TextLocation other)
  {
    if (this == other)
      return 0;
    return this < other ? -1 : 1;
  }
}
