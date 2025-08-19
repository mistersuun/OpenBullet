// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.TextViewPosition
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit;

public struct TextViewPosition : IEquatable<TextViewPosition>, IComparable<TextViewPosition>
{
  private int line;
  private int column;
  private int visualColumn;
  private bool isAtEndOfLine;

  public TextLocation Location
  {
    get => new TextLocation(this.line, this.column);
    set
    {
      this.line = value.Line;
      this.column = value.Column;
    }
  }

  public int Line
  {
    get => this.line;
    set => this.line = value;
  }

  public int Column
  {
    get => this.column;
    set => this.column = value;
  }

  public int VisualColumn
  {
    get => this.visualColumn;
    set => this.visualColumn = value;
  }

  public bool IsAtEndOfLine
  {
    get => this.isAtEndOfLine;
    set => this.isAtEndOfLine = value;
  }

  public TextViewPosition(int line, int column, int visualColumn)
  {
    this.line = line;
    this.column = column;
    this.visualColumn = visualColumn;
    this.isAtEndOfLine = false;
  }

  public TextViewPosition(int line, int column)
    : this(line, column, -1)
  {
  }

  public TextViewPosition(TextLocation location, int visualColumn)
  {
    this.line = location.Line;
    this.column = location.Column;
    this.visualColumn = visualColumn;
    this.isAtEndOfLine = false;
  }

  public TextViewPosition(TextLocation location)
    : this(location, -1)
  {
  }

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[TextViewPosition Line={0} Column={1} VisualColumn={2} IsAtEndOfLine={3}]", (object) this.line, (object) this.column, (object) this.visualColumn, (object) this.isAtEndOfLine);
  }

  public override bool Equals(object obj) => obj is TextViewPosition other && this.Equals(other);

  public override int GetHashCode()
  {
    return (this.isAtEndOfLine ? 115817 : 0) + 1000000007 * this.Line.GetHashCode() + 1000000009 * this.Column.GetHashCode() + 1000000021 * this.VisualColumn.GetHashCode();
  }

  public bool Equals(TextViewPosition other)
  {
    return this.Line == other.Line && this.Column == other.Column && this.VisualColumn == other.VisualColumn && this.IsAtEndOfLine == other.IsAtEndOfLine;
  }

  public static bool operator ==(TextViewPosition left, TextViewPosition right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(TextViewPosition left, TextViewPosition right)
  {
    return !left.Equals(right);
  }

  public int CompareTo(TextViewPosition other)
  {
    int num1 = this.Location.CompareTo(other.Location);
    if (num1 != 0)
      return num1;
    int num2 = this.visualColumn.CompareTo(other.visualColumn);
    if (num2 != 0)
      return num2;
    if (this.isAtEndOfLine && !other.isAtEndOfLine)
      return -1;
    return !this.isAtEndOfLine && other.isAtEndOfLine ? 1 : 0;
  }
}
