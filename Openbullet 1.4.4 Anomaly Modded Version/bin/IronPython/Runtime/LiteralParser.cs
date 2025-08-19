// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.LiteralParser
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

public static class LiteralParser
{
  private static char[] signs = new char[2]{ '+', '-' };

  public static string ParseString(string text, bool isRaw, bool isUni)
  {
    return LiteralParser.ParseString(text.ToCharArray(), 0, text.Length, isRaw, isUni, false);
  }

  public static string ParseString(
    char[] text,
    int start,
    int length,
    bool isRaw,
    bool isUni,
    bool normalizeLineEndings)
  {
    if (isRaw && !isUni && !normalizeLineEndings)
      return new string(text, start, length);
    StringBuilder stringBuilder1 = (StringBuilder) null;
    int start1 = start;
    int num1 = start + length;
    while (start1 < num1)
    {
      char ch1 = text[start1++];
      if (!isRaw | isUni && ch1 == '\\')
      {
        if (stringBuilder1 == null)
        {
          stringBuilder1 = new StringBuilder(length);
          stringBuilder1.Append(text, start, start1 - start - 1);
        }
        if (start1 >= num1)
        {
          if (!isRaw)
            throw PythonOps.ValueError("Trailing \\ in string");
          stringBuilder1.Append('\\');
          break;
        }
        char ch2 = text[start1++];
        int utf32;
        switch (ch2)
        {
          case 'U':
          case 'u':
            int length1 = ch2 == 'u' ? 4 : 8;
            int b = 16 /*0x10*/;
            if (isUni)
            {
              if (LiteralParser.TryParseInt(text, start1, length1, b, out utf32))
              {
                if (utf32 < 0 || utf32 > 1114111)
                  throw PythonOps.StandardError("'unicodeescape' codec can't decode bytes in position {0}: illegal Unicode character", (object) start1);
                if (utf32 < 65536 /*0x010000*/)
                  stringBuilder1.Append((char) utf32);
                else
                  stringBuilder1.Append(char.ConvertFromUtf32(utf32));
                start1 += length1;
                continue;
              }
              throw PythonOps.UnicodeEncodeError("'unicodeescape' codec can't decode bytes in position {0}: truncated \\uXXXX escape", (object) start1);
            }
            stringBuilder1.Append('\\');
            stringBuilder1.Append(ch2);
            continue;
          default:
            if (isRaw)
            {
              stringBuilder1.Append('\\');
              stringBuilder1.Append(ch2);
              continue;
            }
            switch (ch2)
            {
              case '\n':
                continue;
              case '\r':
                if (start1 < num1 && text[start1] == '\n')
                {
                  ++start1;
                  continue;
                }
                continue;
              case '"':
                stringBuilder1.Append('"');
                continue;
              case '\'':
                stringBuilder1.Append('\'');
                continue;
              case '0':
              case '1':
              case '2':
              case '3':
              case '4':
              case '5':
              case '6':
              case '7':
                utf32 = (int) ch2 - 48 /*0x30*/;
                int num2;
                if (start1 < num1 && LiteralParser.HexValue(text[start1], out num2) && num2 < 8)
                {
                  utf32 = utf32 * 8 + num2;
                  ++start1;
                  if (start1 < num1 && LiteralParser.HexValue(text[start1], out num2) && num2 < 8)
                  {
                    utf32 = utf32 * 8 + num2;
                    ++start1;
                  }
                }
                stringBuilder1.Append((char) utf32);
                continue;
              case 'N':
                unicodedata.PerformModuleReload((PythonContext) null, (IDictionary) null);
                if (start1 < num1 && text[start1] == '{')
                {
                  ++start1;
                  StringBuilder stringBuilder2 = new StringBuilder();
                  bool flag = false;
                  while (start1 < num1)
                  {
                    char ch3 = text[start1++];
                    if (ch3 != '}')
                    {
                      stringBuilder2.Append(ch3);
                    }
                    else
                    {
                      flag = true;
                      break;
                    }
                  }
                  if (flag)
                  {
                    if (stringBuilder2.Length != 0)
                    {
                      try
                      {
                        string str = unicodedata.lookup(stringBuilder2.ToString());
                        stringBuilder1.Append(str);
                        continue;
                      }
                      catch (KeyNotFoundException ex)
                      {
                        throw PythonOps.StandardError("'unicodeescape' codec can't decode bytes in position {0}: unknown Unicode character name", (object) start1);
                      }
                    }
                  }
                  throw PythonOps.StandardError("'unicodeescape' codec can't decode bytes in position {0}: malformed \\N character escape", (object) start1);
                }
                throw PythonOps.StandardError("'unicodeescape' codec can't decode bytes in position {0}: malformed \\N character escape", (object) start1);
              case '\\':
                stringBuilder1.Append('\\');
                continue;
              case 'a':
                stringBuilder1.Append('\a');
                continue;
              case 'b':
                stringBuilder1.Append('\b');
                continue;
              case 'f':
                stringBuilder1.Append('\f');
                continue;
              case 'n':
                stringBuilder1.Append('\n');
                continue;
              case 'r':
                stringBuilder1.Append('\r');
                continue;
              case 't':
                stringBuilder1.Append('\t');
                continue;
              case 'v':
                stringBuilder1.Append('\v');
                continue;
              case 'x':
                if (LiteralParser.TryParseInt(text, start1, 2, 16 /*0x10*/, out utf32))
                {
                  stringBuilder1.Append((char) utf32);
                  start1 += 2;
                  continue;
                }
                break;
            }
            stringBuilder1.Append("\\");
            stringBuilder1.Append(ch2);
            continue;
        }
      }
      else if (ch1 == '\r' & normalizeLineEndings)
      {
        if (stringBuilder1 == null)
        {
          stringBuilder1 = new StringBuilder(length);
          stringBuilder1.Append(text, start, start1 - start - 1);
        }
        if (start1 < text.Length && text[start1] == '\n')
          ++start1;
        stringBuilder1.Append('\n');
      }
      else
        stringBuilder1?.Append(ch1);
    }
    return stringBuilder1 != null ? stringBuilder1.ToString() : new string(text, start, length);
  }

  internal static List<byte> ParseBytes(
    char[] text,
    int start,
    int length,
    bool isRaw,
    bool normalizeLineEndings)
  {
    List<byte> bytes = new List<byte>(length);
    int start1 = start;
    int num1 = start + length;
    while (start1 < num1)
    {
      char ch1 = text[start1++];
      if (!isRaw && ch1 == '\\')
      {
        char ch2 = start1 < num1 ? text[start1++] : throw PythonOps.ValueError("Trailing \\ in string");
        int num2;
        switch (ch2)
        {
          case '\n':
            continue;
          case '\r':
            if (start1 < num1 && text[start1] == '\n')
            {
              ++start1;
              continue;
            }
            continue;
          case '"':
            bytes.Add((byte) 34);
            continue;
          case '\'':
            bytes.Add((byte) 39);
            continue;
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
            num2 = (int) ch2 - 48 /*0x30*/;
            int num3;
            if (start1 < num1 && LiteralParser.HexValue(text[start1], out num3) && num3 < 8)
            {
              num2 = num2 * 8 + num3;
              ++start1;
              if (start1 < num1 && LiteralParser.HexValue(text[start1], out num3) && num3 < 8)
              {
                num2 = num2 * 8 + num3;
                ++start1;
              }
            }
            bytes.Add((byte) num2);
            continue;
          case '\\':
            bytes.Add((byte) 92);
            continue;
          case 'a':
            bytes.Add((byte) 7);
            continue;
          case 'b':
            bytes.Add((byte) 8);
            continue;
          case 'f':
            bytes.Add((byte) 12);
            continue;
          case 'n':
            bytes.Add((byte) 10);
            continue;
          case 'r':
            bytes.Add((byte) 13);
            continue;
          case 't':
            bytes.Add((byte) 9);
            continue;
          case 'v':
            bytes.Add((byte) 11);
            continue;
          case 'x':
            if (LiteralParser.TryParseInt(text, start1, 2, 16 /*0x10*/, out num2))
            {
              bytes.Add((byte) num2);
              start1 += 2;
              continue;
            }
            break;
        }
        bytes.Add((byte) 92);
        bytes.Add((byte) ch2);
      }
      else if (ch1 == '\r' & normalizeLineEndings)
      {
        if (start1 < text.Length && text[start1] == '\n')
          ++start1;
        bytes.Add((byte) 10);
      }
      else
        bytes.Add((byte) ch1);
    }
    return bytes;
  }

  private static bool HexValue(char ch, out int value)
  {
    switch (ch)
    {
      case '0':
      case '٠':
        value = 0;
        break;
      case '1':
      case '١':
        value = 1;
        break;
      case '2':
      case '٢':
        value = 2;
        break;
      case '3':
      case '٣':
        value = 3;
        break;
      case '4':
      case '٤':
        value = 4;
        break;
      case '5':
      case '٥':
        value = 5;
        break;
      case '6':
      case '٦':
        value = 6;
        break;
      case '7':
      case '٧':
        value = 7;
        break;
      case '8':
      case '٨':
        value = 8;
        break;
      case '9':
      case '٩':
        value = 9;
        break;
      default:
        if (ch >= 'a' && ch <= 'z')
        {
          value = (int) ch - 97 + 10;
          break;
        }
        if (ch >= 'A' && ch <= 'Z')
        {
          value = (int) ch - 65 + 10;
          break;
        }
        value = -1;
        return false;
    }
    return true;
  }

  private static int HexValue(char ch)
  {
    int num;
    if (!LiteralParser.HexValue(ch, out num))
      throw new ValueErrorException("bad char for integer value: " + ch.ToString());
    return num;
  }

  private static int CharValue(char ch, int b)
  {
    int num = LiteralParser.HexValue(ch);
    return num < b ? num : throw new ValueErrorException($"bad char for the integer value: '{ch}' (base {b})");
  }

  private static bool ParseInt(string text, int b, out int ret)
  {
    ret = 0;
    long num1 = 1;
    for (int index = text.Length - 1; index >= 0; --index)
    {
      long num2 = (long) ret + num1 * (long) LiteralParser.CharValue(text[index], b);
      if ((long) int.MinValue > num2 || num2 > (long) int.MaxValue)
        return false;
      ret = (int) num2;
      num1 *= (long) b;
      if ((long) int.MinValue > num1 || num1 > (long) int.MaxValue)
        return false;
    }
    return true;
  }

  private static bool TryParseInt(char[] text, int start, int length, int b, out int value)
  {
    value = 0;
    if (start + length > text.Length)
      return false;
    int index1 = start;
    for (int index2 = start + length; index1 < index2; ++index1)
    {
      int num;
      if (!LiteralParser.HexValue(text[index1], out num) || num >= b)
        return false;
      value = value * b + num;
    }
    return true;
  }

  public static object ParseInteger(string text, int b)
  {
    int ret;
    if (!LiteralParser.ParseInt(text, b, out ret))
    {
      BigInteger bigInteger = LiteralParser.ParseBigInteger(text, b);
      if (!bigInteger.AsInt32(out ret))
        return (object) bigInteger;
    }
    return ScriptingRuntimeHelpers.Int32ToObject(ret);
  }

  public static object ParseIntegerSign(string text, int b, int start = 0)
  {
    int length = text.Length;
    int b1 = b;
    int start1 = start;
    if (start < 0 || start > length)
      throw new ArgumentOutOfRangeException(nameof (start));
    short sign = 1;
    if (b < 0 || b == 1 || b > 36)
      throw new ValueErrorException("base must be >= 2 and <= 36");
    LiteralParser.ParseIntegerStart(text, ref b, ref start, length, ref sign);
    int num1 = 0;
    try
    {
      int num2 = start;
      for (; start < length; ++start)
      {
        int num3;
        if (LiteralParser.HexValue(text[start], out num3))
        {
          if (num3 >= b)
          {
            if (text[start] != 'l')
            {
              if (text[start] != 'L')
                throw new ValueErrorException($"invalid literal for int() with base {b}: {StringOps.__repr__(text)}");
              goto label_16;
            }
            goto label_16;
          }
          num1 = checked (num1 * b + (int) sign * num3);
        }
        else
          goto label_16;
      }
      if (num2 == start)
        throw new ValueErrorException($"invalid literal for int() with base {b}: {StringOps.__repr__(text)}");
    }
    catch (OverflowException ex)
    {
      return (object) LiteralParser.ParseBigIntegerSign(text, b1, start1);
    }
label_16:
    LiteralParser.ParseIntegerEnd(text, start, length);
    return ScriptingRuntimeHelpers.Int32ToObject(num1);
  }

  private static void ParseIntegerStart(
    string text,
    ref int b,
    ref int start,
    int end,
    ref short sign)
  {
    while (start < end && char.IsWhiteSpace(text, start))
      ++start;
    if (start < end)
    {
      switch (text[start])
      {
        case '+':
          ++start;
          break;
        case '-':
          sign = (short) -1;
          goto case '+';
      }
    }
    while (start < end && char.IsWhiteSpace(text, start))
      ++start;
    if (b != 0)
      return;
    if (start < end && text[start] == '0')
    {
      if (++start < end)
      {
        switch (text[start])
        {
          case 'B':
          case 'b':
            ++start;
            b = 2;
            break;
          case 'O':
          case 'o':
            b = 8;
            ++start;
            break;
          case 'X':
          case 'x':
            ++start;
            b = 16 /*0x10*/;
            break;
        }
      }
      if (b != 0)
        return;
      --start;
      b = 8;
    }
    else
      b = 10;
  }

  private static void ParseIntegerEnd(string text, int start, int end)
  {
    while (start < end && char.IsWhiteSpace(text, start))
      ++start;
    if (start < end)
      throw new ValueErrorException("invalid integer number literal");
  }

  public static BigInteger ParseBigInteger(string text, int b)
  {
    BigInteger zero = BigInteger.Zero;
    BigInteger one = BigInteger.One;
    int index1 = text.Length - 1;
    if (text[index1] == 'l' || text[index1] == 'L')
      --index1;
    int num1 = 7;
    if (b <= 10)
      num1 = 9;
    while (index1 >= 0)
    {
      int num2 = 1;
      uint num3 = 0;
      for (int index2 = 0; index2 < num1 && index1 >= 0; ++index2)
      {
        num3 = (uint) ((ulong) (LiteralParser.CharValue(text[index1--], b) * num2) + (ulong) num3);
        num2 *= b;
      }
      zero += one * (BigInteger) num3;
      if (index1 >= 0)
        one *= (BigInteger) num2;
    }
    return zero;
  }

  public static BigInteger ParseBigIntegerSign(string text, int b, int start = 0)
  {
    int length = text.Length;
    if (start < 0 || start > length)
      throw new ArgumentOutOfRangeException(nameof (start));
    short sign = 1;
    if (b < 0 || b == 1 || b > 36)
      throw new ValueErrorException("base must be >= 2 and <= 36");
    LiteralParser.ParseIntegerStart(text, ref b, ref start, length, ref sign);
    BigInteger bigInteger = BigInteger.Zero;
    int num1 = start;
    for (; start < length; ++start)
    {
      int num2;
      if (LiteralParser.HexValue(text[start], out num2))
      {
        if (num2 >= b)
        {
          if (text[start] != 'l' && text[start] != 'L')
            throw new ValueErrorException($"invalid literal for long() with base {b}: {StringOps.__repr__(text)}");
          goto label_13;
        }
        bigInteger = bigInteger * (BigInteger) b + (BigInteger) num2;
      }
      else
        goto label_13;
    }
    if (start == num1)
      throw new ValueErrorException($"invalid literal for long() with base {b}: {StringOps.__repr__(text)}");
label_13:
    if (start < length && (text[start] == 'l' || text[start] == 'L'))
      ++start;
    LiteralParser.ParseIntegerEnd(text, start, length);
    return sign >= (short) 0 ? bigInteger : -bigInteger;
  }

  public static double ParseFloat(string text)
  {
    try
    {
      return text == null || text.Length <= 0 || text[text.Length - 1] != char.MinValue ? LiteralParser.ParseFloatNoCatch(text) : throw PythonOps.ValueError("null byte in float literal");
    }
    catch (OverflowException ex)
    {
      return text.lstrip().StartsWith("-") ? double.NegativeInfinity : double.PositiveInfinity;
    }
  }

  private static double ParseFloatNoCatch(string text)
  {
    string str = LiteralParser.ReplaceUnicodeDigits(text);
    switch (str.ToLowerAsciiTriggered().lstrip())
    {
      case "nan":
      case "+nan":
      case "-nan":
        return double.NaN;
      case "inf":
      case "+inf":
        return double.PositiveInfinity;
      case "-inf":
        return double.NegativeInfinity;
      default:
        double num = double.Parse(str, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture);
        return num != 0.0 || !text.lstrip().StartsWith("-") ? num : -0.0;
    }
  }

  private static string ReplaceUnicodeDigits(string text)
  {
    StringBuilder stringBuilder = (StringBuilder) null;
    for (int index = 0; index < text.Length; ++index)
    {
      if (text[index] >= '٠' && text[index] <= '٩')
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder(text);
        stringBuilder[index] = (char) ((int) text[index] - 1632 + 48 /*0x30*/);
      }
    }
    if (stringBuilder != null)
      text = stringBuilder.ToString();
    return text;
  }

  private static Exception ExnMalformed()
  {
    return PythonOps.ValueError("complex() arg is a malformed string");
  }

  public static Complex ParseComplex(string s)
  {
    string text1 = s.Trim().ToLower();
    if (string.IsNullOrEmpty(text1) || text1.IndexOf(' ') != -1)
      throw LiteralParser.ExnMalformed();
    if (text1.StartsWith("(") && text1.EndsWith(")"))
      text1 = text1.Substring(1, text1.Length - 2);
    try
    {
      int length = text1.Length;
      string text2;
      string text3;
      if (text1[length - 1] == 'j')
      {
        int num1 = text1.LastIndexOfAny(LiteralParser.signs);
        int num2 = 0;
        while (num1 > 0 && text1[num1 - 1] == 'e')
        {
          if (num2 == 2)
            throw LiteralParser.ExnMalformed();
          num1 = text1.Substring(0, num1 - 1).LastIndexOfAny(LiteralParser.signs);
          ++num2;
        }
        if (num1 < 0)
          return MathUtils.MakeImaginary(length == 1 ? 1.0 : LiteralParser.ParseFloatNoCatch(text1.Substring(0, length - 1)));
        text2 = text1.Substring(0, num1);
        text3 = text1.Substring(num1, length - num1 - 1);
        if (text3.Length == 1)
          text3 += "1";
      }
      else
      {
        string[] strArray = text1.Split('j');
        if (strArray.Length == 1)
          return MathUtils.MakeReal(LiteralParser.ParseFloatNoCatch(text1));
        text2 = strArray.Length == 2 ? strArray[1] : throw LiteralParser.ExnMalformed();
        text3 = strArray[0];
        if (!text2.StartsWith("+") && !text2.StartsWith("-"))
          throw LiteralParser.ExnMalformed();
      }
      return new Complex(string.IsNullOrEmpty(text2) ? 0.0 : LiteralParser.ParseFloatNoCatch(text2), LiteralParser.ParseFloatNoCatch(text3));
    }
    catch (OverflowException ex)
    {
      throw PythonOps.ValueError("complex() literal too large to convert");
    }
    catch
    {
      throw LiteralParser.ExnMalformed();
    }
  }

  public static Complex ParseImaginary(string text)
  {
    try
    {
      return MathUtils.MakeImaginary(double.Parse(text.Substring(0, text.Length - 1), (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat));
    }
    catch (OverflowException ex)
    {
      return new Complex(0.0, double.PositiveInfinity);
    }
  }
}
