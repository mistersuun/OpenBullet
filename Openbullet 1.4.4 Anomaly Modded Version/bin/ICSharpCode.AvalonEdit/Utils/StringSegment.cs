// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.StringSegment
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public struct StringSegment : IEquatable<StringSegment>
{
  private readonly string text;
  private readonly int offset;
  private readonly int count;

  public StringSegment(string text, int offset, int count)
  {
    if (text == null)
      throw new ArgumentNullException(nameof (text));
    if (offset < 0 || offset > text.Length)
      throw new ArgumentOutOfRangeException(nameof (offset));
    if (offset + count > text.Length)
      throw new ArgumentOutOfRangeException(nameof (count));
    this.text = text;
    this.offset = offset;
    this.count = count;
  }

  public StringSegment(string text)
  {
    this.text = text != null ? text : throw new ArgumentNullException(nameof (text));
    this.offset = 0;
    this.count = text.Length;
  }

  public string Text => this.text;

  public int Offset => this.offset;

  public int Count => this.count;

  public override bool Equals(object obj) => obj is StringSegment other && this.Equals(other);

  public bool Equals(StringSegment other)
  {
    return object.ReferenceEquals((object) this.text, (object) other.text) && this.offset == other.offset && this.count == other.count;
  }

  public override int GetHashCode() => this.text.GetHashCode() ^ this.offset ^ this.count;

  public static bool operator ==(StringSegment left, StringSegment right) => left.Equals(right);

  public static bool operator !=(StringSegment left, StringSegment right) => !left.Equals(right);
}
