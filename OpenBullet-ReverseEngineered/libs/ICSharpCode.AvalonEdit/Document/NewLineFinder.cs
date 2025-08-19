// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.NewLineFinder
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

internal static class NewLineFinder
{
  private static readonly char[] newline = new char[2]
  {
    '\r',
    '\n'
  };
  internal static readonly string[] NewlineStrings = new string[3]
  {
    "\r\n",
    "\r",
    "\n"
  };

  internal static SimpleSegment NextNewLine(string text, int offset)
  {
    int num = text.IndexOfAny(NewLineFinder.newline, offset);
    if (num < 0)
      return SimpleSegment.Invalid;
    return text[num] == '\r' && num + 1 < text.Length && text[num + 1] == '\n' ? new SimpleSegment(num, 2) : new SimpleSegment(num, 1);
  }

  internal static SimpleSegment NextNewLine(ITextSource text, int offset)
  {
    int textLength = text.TextLength;
    int offset1 = text.IndexOfAny(NewLineFinder.newline, offset, textLength - offset);
    if (offset1 < 0)
      return SimpleSegment.Invalid;
    return text.GetCharAt(offset1) == '\r' && offset1 + 1 < textLength && text.GetCharAt(offset1 + 1) == '\n' ? new SimpleSegment(offset1, 2) : new SimpleSegment(offset1, 1);
  }
}
