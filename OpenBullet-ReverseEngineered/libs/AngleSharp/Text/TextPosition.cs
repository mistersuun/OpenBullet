// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.TextPosition
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Text;

public struct TextPosition(ushort line, ushort column, int position) : 
  IEquatable<TextPosition>,
  IComparable<TextPosition>
{
  public static readonly TextPosition Empty;
  private readonly ushort _line = line;
  private readonly ushort _column = column;
  private readonly int _position = position;

  public int Line => (int) this._line;

  public int Column => (int) this._column;

  public int Position => this._position;

  public int Index => this._position - 1;

  public TextPosition Shift(int columns)
  {
    return new TextPosition(this._line, (ushort) ((uint) this._column + (uint) columns), this._position + columns);
  }

  public TextPosition After(char chr)
  {
    ushort line = this._line;
    ushort num1 = this._column;
    if (chr == '\n')
    {
      ++line;
      num1 = (ushort) 0;
    }
    ushort num2;
    return new TextPosition(line, num2 = (ushort) ((uint) num1 + 1U), this._position + 1);
  }

  public TextPosition After(string str)
  {
    ushort line = this._line;
    ushort column = this._column;
    foreach (char ch in str)
    {
      if (ch == '\n')
      {
        ++line;
        column = (ushort) 0;
      }
      ++column;
    }
    return new TextPosition(line, column, this._position + str.Length);
  }

  public override string ToString() => $"Ln {this._line}, Col {this._column}, Pos {this._position}";

  public override int GetHashCode()
  {
    return this._position ^ ((int) this._line | (int) this._column) + (int) this._line;
  }

  public override bool Equals(object obj) => obj is TextPosition other && this.Equals(other);

  public bool Equals(TextPosition other)
  {
    return this._position == other._position && (int) this._column == (int) other._column && (int) this._line == (int) other._line;
  }

  public static bool operator >(TextPosition a, TextPosition b) => a._position > b._position;

  public static bool operator <(TextPosition a, TextPosition b) => a._position < b._position;

  public int CompareTo(TextPosition other)
  {
    if (this.Equals(other))
      return 0;
    return !(this > other) ? -1 : 1;
  }
}
