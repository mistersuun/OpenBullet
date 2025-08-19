// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpRuleParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http;

internal static class HttpRuleParser
{
  private static readonly bool[] s_tokenChars = HttpRuleParser.CreateTokenChars();
  private const int maxNestedCount = 5;
  private static readonly string[] s_dateFormats = new string[15]
  {
    "ddd, d MMM yyyy H:m:s 'GMT'",
    "ddd, d MMM yyyy H:m:s",
    "d MMM yyyy H:m:s 'GMT'",
    "d MMM yyyy H:m:s",
    "ddd, d MMM yy H:m:s 'GMT'",
    "ddd, d MMM yy H:m:s",
    "d MMM yy H:m:s 'GMT'",
    "d MMM yy H:m:s",
    "dddd, d'-'MMM'-'yy H:m:s 'GMT'",
    "dddd, d'-'MMM'-'yy H:m:s",
    "ddd MMM d H:m:s yyyy",
    "ddd, d MMM yyyy H:m:s zzz",
    "ddd, d MMM yyyy H:m:s",
    "d MMM yyyy H:m:s zzz",
    "d MMM yyyy H:m:s"
  };
  internal const char CR = '\r';
  internal const char LF = '\n';
  internal const int MaxInt64Digits = 19;
  internal const int MaxInt32Digits = 10;
  internal static readonly Encoding DefaultHttpEncoding = Encoding.GetEncoding(28591);

  private static bool[] CreateTokenChars()
  {
    bool[] tokenChars = new bool[128 /*0x80*/];
    for (int index = 33; index < (int) sbyte.MaxValue; ++index)
      tokenChars[index] = true;
    tokenChars[40] = false;
    tokenChars[41] = false;
    tokenChars[60] = false;
    tokenChars[62] = false;
    tokenChars[64 /*0x40*/] = false;
    tokenChars[44] = false;
    tokenChars[59] = false;
    tokenChars[58] = false;
    tokenChars[92] = false;
    tokenChars[34] = false;
    tokenChars[47] = false;
    tokenChars[91] = false;
    tokenChars[93] = false;
    tokenChars[63 /*0x3F*/] = false;
    tokenChars[61] = false;
    tokenChars[123] = false;
    tokenChars[125] = false;
    return tokenChars;
  }

  internal static bool IsTokenChar(char character)
  {
    return character <= '\u007F' && HttpRuleParser.s_tokenChars[(int) character];
  }

  internal static int GetTokenLength(string input, int startIndex)
  {
    if (startIndex >= input.Length)
      return 0;
    for (int index = startIndex; index < input.Length; ++index)
    {
      if (!HttpRuleParser.IsTokenChar(input[index]))
        return index - startIndex;
    }
    return input.Length - startIndex;
  }

  internal static int GetWhitespaceLength(string input, int startIndex)
  {
    if (startIndex >= input.Length)
      return 0;
    int index = startIndex;
    while (index < input.Length)
    {
      switch (input[index])
      {
        case '\t':
        case ' ':
          ++index;
          continue;
        case '\r':
          if (index + 2 < input.Length && input[index + 1] == '\n')
          {
            switch (input[index + 2])
            {
              case '\t':
              case ' ':
                index += 3;
                continue;
            }
          }
          else
            break;
          break;
      }
      return index - startIndex;
    }
    return input.Length - startIndex;
  }

  internal static bool ContainsInvalidNewLine(string value)
  {
    return HttpRuleParser.ContainsInvalidNewLine(value, 0);
  }

  internal static bool ContainsInvalidNewLine(string value, int startIndex)
  {
    for (int index1 = startIndex; index1 < value.Length; ++index1)
    {
      if (value[index1] == '\r')
      {
        int index2 = index1 + 1;
        if (index2 < value.Length && value[index2] == '\n')
        {
          index1 = index2 + 1;
          if (index1 == value.Length)
            return true;
          switch (value[index1])
          {
            case '\t':
            case ' ':
              continue;
            default:
              return true;
          }
        }
      }
    }
    return false;
  }

  internal static int GetNumberLength(string input, int startIndex, bool allowDecimal)
  {
    int index = startIndex;
    bool flag = !allowDecimal;
    if (input[index] == '.')
      return 0;
    while (index < input.Length)
    {
      char ch = input[index];
      switch (ch)
      {
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          ++index;
          continue;
        default:
          if (!flag && ch == '.')
          {
            flag = true;
            ++index;
            continue;
          }
          goto label_7;
      }
    }
label_7:
    return index - startIndex;
  }

  internal static int GetHostLength(
    string input,
    int startIndex,
    bool allowToken,
    out string host)
  {
    host = (string) null;
    if (startIndex >= input.Length)
      return 0;
    int index = startIndex;
    bool flag = true;
    for (; index < input.Length; ++index)
    {
      char character = input[index];
      switch (character)
      {
        case '\t':
        case '\r':
        case ' ':
        case ',':
          goto label_7;
        case '/':
          return 0;
        default:
          flag = flag && HttpRuleParser.IsTokenChar(character);
          continue;
      }
    }
label_7:
    int length = index - startIndex;
    if (length == 0)
      return 0;
    string host1 = input.Substring(startIndex, length);
    if ((!allowToken || !flag) && !HttpRuleParser.IsValidHostName(host1))
      return 0;
    host = host1;
    return length;
  }

  internal static HttpParseResult GetCommentLength(string input, int startIndex, out int length)
  {
    int nestedCount = 0;
    return HttpRuleParser.GetExpressionLength(input, startIndex, '(', ')', true, ref nestedCount, out length);
  }

  internal static HttpParseResult GetQuotedStringLength(
    string input,
    int startIndex,
    out int length)
  {
    int nestedCount = 0;
    return HttpRuleParser.GetExpressionLength(input, startIndex, '"', '"', false, ref nestedCount, out length);
  }

  internal static HttpParseResult GetQuotedPairLength(string input, int startIndex, out int length)
  {
    length = 0;
    if (input[startIndex] != '\\')
      return HttpParseResult.NotParsed;
    if (startIndex + 2 > input.Length || input[startIndex + 1] > '\u007F')
      return HttpParseResult.InvalidFormat;
    length = 2;
    return HttpParseResult.Parsed;
  }

  internal static string DateToString(DateTimeOffset dateTime)
  {
    return dateTime.ToUniversalTime().ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal static bool TryStringToDate(string input, out DateTimeOffset result)
  {
    return DateTimeOffset.TryParseExact(input, HttpRuleParser.s_dateFormats, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out result);
  }

  private static HttpParseResult GetExpressionLength(
    string input,
    int startIndex,
    char openChar,
    char closeChar,
    bool supportsNesting,
    ref int nestedCount,
    out int length)
  {
    length = 0;
    if ((int) input[startIndex] != (int) openChar)
      return HttpParseResult.NotParsed;
    int num = startIndex + 1;
    while (num < input.Length)
    {
      int length1 = 0;
      if (num + 2 < input.Length && HttpRuleParser.GetQuotedPairLength(input, num, out length1) == HttpParseResult.Parsed)
      {
        num += length1;
      }
      else
      {
        if (supportsNesting && (int) input[num] == (int) openChar)
        {
          ++nestedCount;
          try
          {
            if (nestedCount > 5)
              return HttpParseResult.InvalidFormat;
            int length2 = 0;
            switch (HttpRuleParser.GetExpressionLength(input, num, openChar, closeChar, supportsNesting, ref nestedCount, out length2))
            {
              case HttpParseResult.Parsed:
                num += length2;
                break;
              case HttpParseResult.InvalidFormat:
                return HttpParseResult.InvalidFormat;
            }
          }
          finally
          {
            --nestedCount;
          }
        }
        if ((int) input[num] == (int) closeChar)
        {
          length = num - startIndex + 1;
          return HttpParseResult.Parsed;
        }
        ++num;
      }
    }
    return HttpParseResult.InvalidFormat;
  }

  private static bool IsValidHostName(string host)
  {
    return Uri.TryCreate($"http://u@{host}/", UriKind.Absolute, out Uri _);
  }
}
