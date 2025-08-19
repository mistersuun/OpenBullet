// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.StringTextSource
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

[Serializable]
public class StringTextSource : ITextSource
{
  public static readonly StringTextSource Empty = new StringTextSource(string.Empty);
  private readonly string text;
  private readonly ITextSourceVersion version;

  public StringTextSource(string text)
  {
    this.text = text != null ? text : throw new ArgumentNullException(nameof (text));
  }

  public StringTextSource(string text, ITextSourceVersion version)
  {
    this.text = text != null ? text : throw new ArgumentNullException(nameof (text));
    this.version = version;
  }

  public ITextSourceVersion Version => this.version;

  public int TextLength => this.text.Length;

  public string Text => this.text;

  public ITextSource CreateSnapshot() => (ITextSource) this;

  public ITextSource CreateSnapshot(int offset, int length)
  {
    return (ITextSource) new StringTextSource(this.text.Substring(offset, length));
  }

  public TextReader CreateReader() => (TextReader) new StringReader(this.text);

  public TextReader CreateReader(int offset, int length)
  {
    return (TextReader) new StringReader(this.text.Substring(offset, length));
  }

  public void WriteTextTo(TextWriter writer) => writer.Write(this.text);

  public void WriteTextTo(TextWriter writer, int offset, int length)
  {
    writer.Write(this.text.Substring(offset, length));
  }

  public char GetCharAt(int offset) => this.text[offset];

  public string GetText(int offset, int length) => this.text.Substring(offset, length);

  public string GetText(ISegment segment)
  {
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    return this.text.Substring(segment.Offset, segment.Length);
  }

  public int IndexOf(char c, int startIndex, int count) => this.text.IndexOf(c, startIndex, count);

  public int IndexOfAny(char[] anyOf, int startIndex, int count)
  {
    return this.text.IndexOfAny(anyOf, startIndex, count);
  }

  public int IndexOf(
    string searchText,
    int startIndex,
    int count,
    StringComparison comparisonType)
  {
    return this.text.IndexOf(searchText, startIndex, count, comparisonType);
  }

  public int LastIndexOf(char c, int startIndex, int count)
  {
    return this.text.LastIndexOf(c, startIndex + count - 1, count);
  }

  public int LastIndexOf(
    string searchText,
    int startIndex,
    int count,
    StringComparison comparisonType)
  {
    return this.text.LastIndexOf(searchText, startIndex + count - 1, count, comparisonType);
  }
}
