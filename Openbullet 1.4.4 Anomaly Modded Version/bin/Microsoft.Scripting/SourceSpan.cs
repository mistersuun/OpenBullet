// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceSpan
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;
using System.Globalization;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public readonly struct SourceSpan : IEquatable<SourceSpan>
{
  public static readonly SourceSpan None = new SourceSpan(SourceLocation.None, SourceLocation.None);
  public static readonly SourceSpan Invalid = new SourceSpan(SourceLocation.Invalid, SourceLocation.Invalid);

  public SourceSpan(SourceLocation start, SourceLocation end)
  {
    SourceSpan.ValidateLocations(start, end);
    this.Start = start;
    this.End = end;
  }

  private static void ValidateLocations(SourceLocation start, SourceLocation end)
  {
    if (start.IsValid && end.IsValid)
    {
      if (start > end)
        throw new ArgumentException("Start and End must be well ordered");
    }
    else if (start.IsValid || end.IsValid)
      throw new ArgumentException("Start and End must both be valid or both invalid");
  }

  public SourceLocation Start { get; }

  public SourceLocation End { get; }

  public int Length
  {
    get
    {
      SourceLocation sourceLocation = this.End;
      int index1 = sourceLocation.Index;
      sourceLocation = this.Start;
      int index2 = sourceLocation.Index;
      return index1 - index2;
    }
  }

  public bool IsValid => this.Start.IsValid && this.End.IsValid;

  public static bool operator ==(SourceSpan left, SourceSpan right)
  {
    return left.Start == right.Start && left.End == right.End;
  }

  public static bool operator !=(SourceSpan left, SourceSpan right)
  {
    return left.Start != right.Start || left.End != right.End;
  }

  public bool Equals(SourceSpan other) => this.Start == other.Start && this.End == other.End;

  public override bool Equals(object obj) => obj is SourceSpan other && this.Equals(other);

  public override string ToString()
  {
    SourceLocation sourceLocation = this.Start;
    string str1 = sourceLocation.ToString();
    sourceLocation = this.End;
    string str2 = sourceLocation.ToString();
    return $"{str1} - {str2}";
  }

  public override int GetHashCode()
  {
    SourceLocation sourceLocation = this.Start;
    int column = sourceLocation.Column;
    sourceLocation = this.End;
    int num1 = sourceLocation.Column << 7;
    int num2 = column ^ num1;
    sourceLocation = this.Start;
    int num3 = sourceLocation.Line << 14;
    int num4 = num2 ^ num3;
    sourceLocation = this.End;
    int num5 = sourceLocation.Line << 23;
    return num4 ^ num5;
  }

  internal string ToDebugString()
  {
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}-{1}", (object) this.Start.ToDebugString(), (object) this.End.ToDebugString());
  }
}
