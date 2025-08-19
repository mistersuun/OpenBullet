// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.StringUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class StringUtils
{
  public static Encoding DefaultEncoding => Encoding.Default;

  public static string GetSuffix(string str, char separator, bool includeSeparator)
  {
    ContractUtils.RequiresNotNull((object) str, nameof (str));
    int num = str.LastIndexOf(separator);
    return num == -1 ? (string) null : str.Substring(includeSeparator ? num : num + 1);
  }

  public static string GetLongestPrefix(string str, char separator, bool includeSeparator)
  {
    ContractUtils.RequiresNotNull((object) str, nameof (str));
    int num = str.LastIndexOf(separator);
    return num == -1 ? (string) null : str.Substring(0, includeSeparator || num == 0 ? num : num - 1);
  }

  public static int CountOf(string str, char c)
  {
    if (string.IsNullOrEmpty(str))
      return 0;
    int num = 0;
    for (int index = 0; index < str.Length; ++index)
    {
      if ((int) c == (int) str[index])
        ++num;
    }
    return num;
  }

  public static string[] Split(
    string str,
    string separator,
    int maxComponents,
    StringSplitOptions options)
  {
    ContractUtils.RequiresNotNull((object) str, nameof (str));
    return str.Split(new string[1]{ separator }, maxComponents, options);
  }

  public static string[] Split(
    string str,
    char[] separators,
    int maxComponents,
    StringSplitOptions options)
  {
    ContractUtils.RequiresNotNull((object) str, nameof (str));
    return str.Split(separators, maxComponents, options);
  }

  public static string SplitWords(string text, bool indentFirst, int lineWidth)
  {
    ContractUtils.RequiresNotNull((object) text, nameof (text));
    if (text.Length <= lineWidth || lineWidth <= 0)
      return indentFirst ? "    " + text : text;
    StringBuilder stringBuilder = new StringBuilder();
    int startIndex = 0;
    int count1 = lineWidth;
    while (startIndex != text.Length)
    {
      if (count1 >= lineWidth)
      {
        while (count1 != 0 && !char.IsWhiteSpace(text[startIndex + count1 - 1]))
          --count1;
      }
      if (stringBuilder.Length != 0)
        stringBuilder.Append(' ');
      if (indentFirst || stringBuilder.Length != 0)
        stringBuilder.Append("    ");
      if (count1 == 0)
      {
        int count2 = Math.Min(lineWidth, text.Length - startIndex);
        stringBuilder.Append(text, startIndex, count2);
        startIndex += count2;
      }
      else
      {
        stringBuilder.Append(text, startIndex, count1);
        startIndex += count1;
      }
      stringBuilder.AppendLine();
      count1 = Math.Min(lineWidth, text.Length - startIndex);
    }
    return stringBuilder.ToString();
  }

  public static string AddSlashes(string str)
  {
    ContractUtils.RequiresNotNull((object) str, nameof (str));
    StringBuilder stringBuilder = new StringBuilder(str.Length);
    for (int index = 0; index < str.Length; ++index)
    {
      switch (str[index])
      {
        case '\a':
          stringBuilder.Append("\\a");
          break;
        case '\b':
          stringBuilder.Append("\\b");
          break;
        case '\t':
          stringBuilder.Append("\\t");
          break;
        case '\n':
          stringBuilder.Append("\\n");
          break;
        case '\v':
          stringBuilder.Append("\\v");
          break;
        case '\f':
          stringBuilder.Append("\\f");
          break;
        case '\r':
          stringBuilder.Append("\\r");
          break;
        default:
          stringBuilder.Append(str[index]);
          break;
      }
    }
    return stringBuilder.ToString();
  }

  public static bool TryParseDouble(
    string s,
    NumberStyles style,
    IFormatProvider provider,
    out double result)
  {
    return double.TryParse(s, style, provider, out result);
  }

  public static bool TryParseInt32(string s, out int result) => int.TryParse(s, out result);

  public static bool TryParseDateTimeExact(
    string s,
    string format,
    IFormatProvider provider,
    DateTimeStyles style,
    out DateTime result)
  {
    return DateTime.TryParseExact(s, format, provider, style, out result);
  }

  public static bool TryParseDateTimeExact(
    string s,
    string[] formats,
    IFormatProvider provider,
    DateTimeStyles style,
    out DateTime result)
  {
    return DateTime.TryParseExact(s, formats, provider, style, out result);
  }

  public static bool TryParseDate(
    string s,
    IFormatProvider provider,
    DateTimeStyles style,
    out DateTime result)
  {
    return DateTime.TryParse(s, provider, style, out result);
  }

  public static CultureInfo GetCultureInfo(string name) => CultureInfo.GetCultureInfo(name);

  public static IEnumerable<string> Split(string str, string sep)
  {
    int end;
    int startIndex;
    for (startIndex = 0; (end = str.IndexOf(sep, startIndex)) != -1; startIndex = end + sep.Length)
      yield return str.Substring(startIndex, end - startIndex);
    yield return str.Substring(startIndex);
  }
}
