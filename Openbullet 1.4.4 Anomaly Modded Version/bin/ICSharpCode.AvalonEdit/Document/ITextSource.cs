// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.ITextSource
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.IO;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public interface ITextSource
{
  ITextSourceVersion Version { get; }

  ITextSource CreateSnapshot();

  ITextSource CreateSnapshot(int offset, int length);

  TextReader CreateReader();

  TextReader CreateReader(int offset, int length);

  int TextLength { get; }

  string Text { get; }

  char GetCharAt(int offset);

  string GetText(int offset, int length);

  string GetText(ISegment segment);

  void WriteTextTo(TextWriter writer);

  void WriteTextTo(TextWriter writer, int offset, int length);

  int IndexOf(char c, int startIndex, int count);

  int IndexOfAny(char[] anyOf, int startIndex, int count);

  int IndexOf(string searchText, int startIndex, int count, StringComparison comparisonType);

  int LastIndexOf(char c, int startIndex, int count);

  int LastIndexOf(string searchText, int startIndex, int count, StringComparison comparisonType);
}
