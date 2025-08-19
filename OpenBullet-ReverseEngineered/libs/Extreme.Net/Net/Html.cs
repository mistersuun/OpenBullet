// Decompiled with JetBrains decompiler
// Type: Extreme.Net.Html
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

#nullable disable
namespace Extreme.Net;

public static class Html
{
  private static readonly Dictionary<string, string> _htmlMnemonics = new Dictionary<string, string>()
  {
    {
      "apos",
      "'"
    },
    {
      "quot",
      "\""
    },
    {
      "amp",
      "&"
    },
    {
      "lt",
      "<"
    },
    {
      "gt",
      ">"
    }
  };

  public static string ReplaceEntities(this string str)
  {
    return string.IsNullOrEmpty(str) ? string.Empty : new Regex("(\\&(?<text>\\w{1,4})\\;)|(\\&#(?<code>\\w{1,4})\\;)", RegexOptions.Compiled).Replace(str, (MatchEvaluator) (match =>
    {
      if (match.Groups["text"].Success)
      {
        string str1;
        if (Html._htmlMnemonics.TryGetValue(match.Groups["text"].Value, out str1))
          return str1;
      }
      else if (match.Groups["code"].Success)
        return ((char) int.Parse(match.Groups["code"].Value)).ToString();
      return match.Value;
    }));
  }

  public static string ReplaceUnicode(this string str)
  {
    return string.IsNullOrEmpty(str) ? string.Empty : new Regex("\\\\u(?<code>[0-9a-f]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(str, (MatchEvaluator) (match => ((char) int.Parse(match.Groups["code"].Value, NumberStyles.HexNumber)).ToString()));
  }

  public static string Substring(
    this string str,
    string left,
    int startIndex,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    if (string.IsNullOrEmpty(str))
      return string.Empty;
    switch (left)
    {
      case null:
        throw new ArgumentNullException(nameof (left));
      case "":
        throw ExceptionHelper.EmptyString(nameof (left));
      default:
        if (startIndex < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
        if (startIndex >= str.Length)
          throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
        int num = str.IndexOf(left, startIndex, comparsion);
        if (num == -1)
          return string.Empty;
        int startIndex1 = num + left.Length;
        int length = str.Length - startIndex1;
        return str.Substring(startIndex1, length);
    }
  }

  public static string Substring(this string str, string left, StringComparison comparsion = StringComparison.Ordinal)
  {
    return str.Substring(left, 0, comparsion);
  }

  public static string ClearSteamSpecialChars(this string str)
  {
    string[] strArray = new string[8]
    {
      "™",
      "★",
      "☆",
      "★",
      "\\u2605",
      "\\u2122",
      "™",
      "★"
    };
    foreach (string oldValue in strArray)
      str = str.Replace(oldValue, "");
    return str;
  }

  public static string Substring(
    this string str,
    string left,
    string right,
    int startIndex,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    if (string.IsNullOrEmpty(str))
      return string.Empty;
    switch (left)
    {
      case null:
        throw new ArgumentNullException(nameof (left));
      case "":
        throw ExceptionHelper.EmptyString(nameof (left));
      default:
        switch (right)
        {
          case null:
            throw new ArgumentNullException(nameof (right));
          case "":
            throw ExceptionHelper.EmptyString(nameof (right));
          default:
            if (startIndex < 0)
              throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
            if (startIndex >= str.Length)
              throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            int num1 = str.IndexOf(left, startIndex, comparsion);
            if (num1 == -1)
              return string.Empty;
            int startIndex1 = num1 + left.Length;
            int num2 = str.IndexOf(right, startIndex1, comparsion);
            if (num2 == -1)
              return string.Empty;
            int length = num2 - startIndex1;
            return str.Substring(startIndex1, length);
        }
    }
  }

  public static string Substring(
    this string str,
    string left,
    string right,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    return str.Substring(left, right, 0, comparsion);
  }

  public static string LastSubstring(
    this string str,
    string left,
    int startIndex,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    if (string.IsNullOrEmpty(str))
      return string.Empty;
    switch (left)
    {
      case null:
        throw new ArgumentNullException(nameof (left));
      case "":
        throw ExceptionHelper.EmptyString(nameof (left));
      default:
        if (startIndex < 0)
          throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
        if (startIndex >= str.Length)
          throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
        int num = str.LastIndexOf(left, startIndex, comparsion);
        if (num == -1)
          return string.Empty;
        int startIndex1 = num + left.Length;
        int length = str.Length - startIndex1;
        return str.Substring(startIndex1, length);
    }
  }

  public static string LastSubstring(this string str, string left, StringComparison comparsion = StringComparison.Ordinal)
  {
    return string.IsNullOrEmpty(str) ? string.Empty : str.LastSubstring(left, str.Length - 1, comparsion);
  }

  public static string LastSubstring(
    this string str,
    string left,
    string right,
    int startIndex,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    if (string.IsNullOrEmpty(str))
      return string.Empty;
    switch (left)
    {
      case null:
        throw new ArgumentNullException(nameof (left));
      case "":
        throw ExceptionHelper.EmptyString(nameof (left));
      default:
        switch (right)
        {
          case null:
            throw new ArgumentNullException(nameof (right));
          case "":
            throw ExceptionHelper.EmptyString(nameof (right));
          default:
            if (startIndex < 0)
              throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
            if (startIndex >= str.Length)
              throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            int num1 = str.LastIndexOf(left, startIndex, comparsion);
            if (num1 == -1)
              return string.Empty;
            int startIndex1 = num1 + left.Length;
            int num2 = str.IndexOf(right, startIndex1, comparsion);
            if (num2 == -1)
              return num1 == 0 ? string.Empty : str.LastSubstring(left, right, num1 - 1, comparsion);
            int length = num2 - startIndex1;
            return str.Substring(startIndex1, length);
        }
    }
  }

  public static string LastSubstring(
    this string str,
    string left,
    string right,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    return string.IsNullOrEmpty(str) ? string.Empty : str.LastSubstring(left, right, str.Length - 1, comparsion);
  }

  public static string[] Substrings(
    this string str,
    string left,
    string right,
    int startIndex,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    if (string.IsNullOrEmpty(str))
      return new string[0];
    switch (left)
    {
      case null:
        throw new ArgumentNullException(nameof (left));
      case "":
        throw ExceptionHelper.EmptyString(nameof (left));
      default:
        switch (right)
        {
          case null:
            throw new ArgumentNullException(nameof (right));
          case "":
            throw ExceptionHelper.EmptyString(nameof (right));
          default:
            if (startIndex < 0)
              throw ExceptionHelper.CanNotBeLess<int>(nameof (startIndex), 0);
            if (startIndex >= str.Length)
              throw new ArgumentOutOfRangeException(nameof (startIndex), Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
            int startIndex1 = startIndex;
            List<string> stringList = new List<string>();
            while (true)
            {
              int num1 = str.IndexOf(left, startIndex1, comparsion);
              if (num1 != -1)
              {
                int startIndex2 = num1 + left.Length;
                int num2 = str.IndexOf(right, startIndex2, comparsion);
                if (num2 != -1)
                {
                  int length = num2 - startIndex2;
                  stringList.Add(str.Substring(startIndex2, length));
                  startIndex1 = num2 + right.Length;
                }
                else
                  break;
              }
              else
                break;
            }
            return stringList.ToArray();
        }
    }
  }

  public static string[] Substrings(
    this string str,
    string left,
    string right,
    StringComparison comparsion = StringComparison.Ordinal)
  {
    return str.Substrings(left, right, 0, comparsion);
  }
}
