// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.RopeTextSource
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.IO;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public sealed class RopeTextSource : ITextSource
{
  private readonly Rope<char> rope;
  private readonly ITextSourceVersion version;

  public RopeTextSource(Rope<char> rope)
  {
    this.rope = rope != null ? rope.Clone() : throw new ArgumentNullException(nameof (rope));
  }

  public RopeTextSource(Rope<char> rope, ITextSourceVersion version)
  {
    this.rope = rope != null ? rope.Clone() : throw new ArgumentNullException(nameof (rope));
    this.version = version;
  }

  public Rope<char> GetRope() => this.rope.Clone();

  public string Text => this.rope.ToString();

  public int TextLength => this.rope.Length;

  public char GetCharAt(int offset) => this.rope[offset];

  public string GetText(int offset, int length) => this.rope.ToString(offset, length);

  public string GetText(ISegment segment) => this.rope.ToString(segment.Offset, segment.Length);

  public TextReader CreateReader() => (TextReader) new RopeTextReader(this.rope);

  public TextReader CreateReader(int offset, int length)
  {
    return (TextReader) new RopeTextReader(this.rope.GetRange(offset, length));
  }

  public ITextSource CreateSnapshot() => (ITextSource) this;

  public ITextSource CreateSnapshot(int offset, int length)
  {
    return (ITextSource) new RopeTextSource(this.rope.GetRange(offset, length));
  }

  public int IndexOf(char c, int startIndex, int count) => this.rope.IndexOf(c, startIndex, count);

  public int IndexOfAny(char[] anyOf, int startIndex, int count)
  {
    return this.rope.IndexOfAny(anyOf, startIndex, count);
  }

  public int LastIndexOf(char c, int startIndex, int count)
  {
    return this.rope.LastIndexOf(c, startIndex, count);
  }

  public ITextSourceVersion Version => this.version;

  public int IndexOf(
    string searchText,
    int startIndex,
    int count,
    StringComparison comparisonType)
  {
    return this.rope.IndexOf(searchText, startIndex, count, comparisonType);
  }

  public int LastIndexOf(
    string searchText,
    int startIndex,
    int count,
    StringComparison comparisonType)
  {
    return this.rope.LastIndexOf(searchText, startIndex, count, comparisonType);
  }

  public void WriteTextTo(TextWriter writer) => this.rope.WriteTo(writer, 0, this.rope.Length);

  public void WriteTextTo(TextWriter writer, int offset, int length)
  {
    this.rope.WriteTo(writer, offset, length);
  }
}
