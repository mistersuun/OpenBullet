// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextUtilities
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Globalization;
using System.Text;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public static class TextUtilities
{
  private static readonly string[] c0Table = new string[32 /*0x20*/]
  {
    "NUL",
    "SOH",
    "STX",
    "ETX",
    "EOT",
    "ENQ",
    "ACK",
    "BEL",
    "BS",
    "HT",
    "LF",
    "VT",
    "FF",
    "CR",
    "SO",
    "SI",
    "DLE",
    "DC1",
    "DC2",
    "DC3",
    "DC4",
    "NAK",
    "SYN",
    "ETB",
    "CAN",
    "EM",
    "SUB",
    "ESC",
    "FS",
    "GS",
    "RS",
    "US"
  };
  private static readonly string[] delAndC1Table = new string[33]
  {
    "DEL",
    "PAD",
    "HOP",
    "BPH",
    "NBH",
    "IND",
    "NEL",
    "SSA",
    "ESA",
    "HTS",
    "HTJ",
    "VTS",
    "PLD",
    "PLU",
    "RI",
    "SS2",
    "SS3",
    "DCS",
    "PU1",
    "PU2",
    "STS",
    "CCH",
    "MW",
    "SPA",
    "EPA",
    "SOS",
    "SGCI",
    "SCI",
    "CSI",
    "ST",
    "OSC",
    "PM",
    "APC"
  };

  public static int FindNextNewLine(ITextSource text, int offset, out string newLineType)
  {
    if (text == null)
      throw new ArgumentNullException(nameof (text));
    if (offset < 0 || offset > text.TextLength)
      throw new ArgumentOutOfRangeException(nameof (offset), (object) offset, "offset is outside of text source");
    SimpleSegment simpleSegment = NewLineFinder.NextNewLine(text, offset);
    if (simpleSegment == SimpleSegment.Invalid)
    {
      newLineType = (string) null;
      return -1;
    }
    newLineType = simpleSegment.Length != 2 ? (text.GetCharAt(simpleSegment.Offset) != '\n' ? "\r" : "\n") : "\r\n";
    return simpleSegment.Offset;
  }

  public static bool IsNewLine(string newLine)
  {
    return newLine == "\r\n" || newLine == "\n" || newLine == "\r";
  }

  public static string NormalizeNewLines(string input, string newLine)
  {
    if (input == null)
      return (string) null;
    if (!TextUtilities.IsNewLine(newLine))
      throw new ArgumentException("newLine must be one of the known newline sequences");
    SimpleSegment simpleSegment = NewLineFinder.NextNewLine(input, 0);
    if (simpleSegment == SimpleSegment.Invalid)
      return input;
    StringBuilder stringBuilder = new StringBuilder(input.Length);
    int num = 0;
    do
    {
      stringBuilder.Append(input, num, simpleSegment.Offset - num);
      stringBuilder.Append(newLine);
      num = simpleSegment.EndOffset;
      simpleSegment = NewLineFinder.NextNewLine(input, num);
    }
    while (simpleSegment != SimpleSegment.Invalid);
    stringBuilder.Append(input, num, input.Length - num);
    return stringBuilder.ToString();
  }

  public static string GetNewLineFromDocument(IDocument document, int lineNumber)
  {
    IDocumentLine documentLine = document.GetLineByNumber(lineNumber);
    if (documentLine.DelimiterLength == 0)
    {
      documentLine = documentLine.PreviousLine;
      if (documentLine == null)
        return Environment.NewLine;
    }
    return document.GetText(documentLine.Offset + documentLine.Length, documentLine.DelimiterLength);
  }

  public static string GetControlCharacterName(char controlCharacter)
  {
    int index = (int) controlCharacter;
    if (index < TextUtilities.c0Table.Length)
      return TextUtilities.c0Table[index];
    return index >= (int) sbyte.MaxValue && index <= 159 ? TextUtilities.delAndC1Table[index - (int) sbyte.MaxValue] : index.ToString("x4", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static ISegment GetWhitespaceAfter(ITextSource textSource, int offset)
  {
    if (textSource == null)
      throw new ArgumentNullException(nameof (textSource));
    int offset1;
    for (offset1 = offset; offset1 < textSource.TextLength; ++offset1)
    {
      switch (textSource.GetCharAt(offset1))
      {
        case '\t':
        case ' ':
          continue;
        default:
          goto label_6;
      }
    }
label_6:
    return (ISegment) new SimpleSegment(offset, offset1 - offset);
  }

  public static ISegment GetWhitespaceBefore(ITextSource textSource, int offset)
  {
    if (textSource == null)
      throw new ArgumentNullException(nameof (textSource));
    int offset1;
    for (offset1 = offset - 1; offset1 >= 0; --offset1)
    {
      switch (textSource.GetCharAt(offset1))
      {
        case '\t':
        case ' ':
          continue;
        default:
          goto label_6;
      }
    }
label_6:
    int offset2 = offset1 + 1;
    return (ISegment) new SimpleSegment(offset2, offset - offset2);
  }

  public static ISegment GetLeadingWhitespace(TextDocument document, DocumentLine documentLine)
  {
    if (documentLine == null)
      throw new ArgumentNullException(nameof (documentLine));
    return TextUtilities.GetWhitespaceAfter((ITextSource) document, documentLine.Offset);
  }

  public static ISegment GetTrailingWhitespace(TextDocument document, DocumentLine documentLine)
  {
    if (documentLine == null)
      throw new ArgumentNullException(nameof (documentLine));
    ISegment whitespaceBefore = TextUtilities.GetWhitespaceBefore((ITextSource) document, documentLine.EndOffset);
    return whitespaceBefore.Offset == documentLine.Offset ? (ISegment) new SimpleSegment(documentLine.EndOffset, 0) : whitespaceBefore;
  }

  public static ISegment GetSingleIndentationSegment(
    ITextSource textSource,
    int offset,
    int indentationSize)
  {
    if (textSource == null)
      throw new ArgumentNullException(nameof (textSource));
    int offset1;
    for (offset1 = offset; offset1 < textSource.TextLength; ++offset1)
    {
      switch (textSource.GetCharAt(offset1))
      {
        case '\t':
          if (offset1 == offset)
            return (ISegment) new SimpleSegment(offset, 1);
          goto label_9;
        case ' ':
          if (offset1 - offset < indentationSize)
            continue;
          goto label_9;
        default:
          goto label_9;
      }
    }
label_9:
    return (ISegment) new SimpleSegment(offset, offset1 - offset);
  }

  public static CharacterClass GetCharacterClass(char c)
  {
    switch (c)
    {
      case '\n':
      case '\r':
        return CharacterClass.LineTerminator;
      case '_':
        return CharacterClass.IdentifierPart;
      default:
        return TextUtilities.GetCharacterClass(char.GetUnicodeCategory(c));
    }
  }

  private static CharacterClass GetCharacterClass(char highSurrogate, char lowSurrogate)
  {
    return char.IsSurrogatePair(highSurrogate, lowSurrogate) ? TextUtilities.GetCharacterClass(char.GetUnicodeCategory(highSurrogate.ToString() + lowSurrogate.ToString(), 0)) : CharacterClass.Other;
  }

  private static CharacterClass GetCharacterClass(UnicodeCategory c)
  {
    switch (c)
    {
      case UnicodeCategory.UppercaseLetter:
      case UnicodeCategory.LowercaseLetter:
      case UnicodeCategory.TitlecaseLetter:
      case UnicodeCategory.ModifierLetter:
      case UnicodeCategory.OtherLetter:
      case UnicodeCategory.DecimalDigitNumber:
        return CharacterClass.IdentifierPart;
      case UnicodeCategory.NonSpacingMark:
      case UnicodeCategory.SpacingCombiningMark:
      case UnicodeCategory.EnclosingMark:
        return CharacterClass.CombiningMark;
      case UnicodeCategory.SpaceSeparator:
      case UnicodeCategory.LineSeparator:
      case UnicodeCategory.ParagraphSeparator:
      case UnicodeCategory.Control:
        return CharacterClass.Whitespace;
      default:
        return CharacterClass.Other;
    }
  }

  public static int GetNextCaretPosition(
    ITextSource textSource,
    int offset,
    LogicalDirection direction,
    CaretPositioningMode mode)
  {
    if (textSource == null)
      throw new ArgumentNullException(nameof (textSource));
    switch (mode)
    {
      case CaretPositioningMode.Normal:
      case CaretPositioningMode.WordBorder:
      case CaretPositioningMode.WordStart:
      case CaretPositioningMode.WordStartOrSymbol:
      case CaretPositioningMode.WordBorderOrSymbol:
      case CaretPositioningMode.EveryCodepoint:
        if (direction != LogicalDirection.Backward && direction != LogicalDirection.Forward)
          throw new ArgumentException("Invalid LogicalDirection: " + (object) direction, nameof (direction));
        int textLength = textSource.TextLength;
        if (textLength <= 0)
          return TextUtilities.IsNormal(mode) && (offset > 0 && direction == LogicalDirection.Backward || offset < 0 && direction == LogicalDirection.Forward) ? 0 : -1;
        int offset1;
        while (true)
        {
          offset1 = direction == LogicalDirection.Backward ? offset - 1 : offset + 1;
          if (offset1 >= 0 && offset1 <= textLength)
          {
            if (offset1 == 0)
            {
              if (TextUtilities.IsNormal(mode) || !char.IsWhiteSpace(textSource.GetCharAt(0)))
                goto label_14;
            }
            else if (offset1 == textLength)
            {
              if (mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol && (TextUtilities.IsNormal(mode) || !char.IsWhiteSpace(textSource.GetCharAt(textLength - 1))))
                goto label_17;
            }
            else
            {
              char charAt1 = textSource.GetCharAt(offset1 - 1);
              char charAt2 = textSource.GetCharAt(offset1);
              if (!char.IsSurrogatePair(charAt1, charAt2))
              {
                CharacterClass characterClass1 = TextUtilities.GetCharacterClass(charAt1);
                CharacterClass characterClass2 = TextUtilities.GetCharacterClass(charAt2);
                if (char.IsLowSurrogate(charAt1) && offset1 >= 2)
                  characterClass1 = TextUtilities.GetCharacterClass(textSource.GetCharAt(offset1 - 2), charAt1);
                if (char.IsHighSurrogate(charAt2) && offset1 + 1 < textLength)
                  characterClass2 = TextUtilities.GetCharacterClass(charAt2, textSource.GetCharAt(offset1 + 1));
                if (TextUtilities.StopBetweenCharacters(mode, characterClass1, characterClass2))
                  goto label_24;
              }
            }
            offset = offset1;
          }
          else
            break;
        }
        return -1;
label_14:
        return offset1;
label_17:
        return offset1;
label_24:
        return offset1;
      default:
        throw new ArgumentException("Unsupported CaretPositioningMode: " + (object) mode, nameof (mode));
    }
  }

  private static bool IsNormal(CaretPositioningMode mode)
  {
    return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint;
  }

  private static bool StopBetweenCharacters(
    CaretPositioningMode mode,
    CharacterClass charBefore,
    CharacterClass charAfter)
  {
    if (mode == CaretPositioningMode.EveryCodepoint)
      return true;
    if (charAfter == CharacterClass.CombiningMark)
      return false;
    if (mode == CaretPositioningMode.Normal)
      return true;
    if (charBefore == charAfter)
    {
      if (charBefore == CharacterClass.Other && (mode == CaretPositioningMode.WordBorderOrSymbol || mode == CaretPositioningMode.WordStartOrSymbol))
        return true;
    }
    else if (mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol || charAfter != CharacterClass.Whitespace && charAfter != CharacterClass.LineTerminator)
      return true;
    return false;
  }
}
