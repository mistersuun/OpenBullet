// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.StringExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace AngleSharp.Text;

public static class StringExtensions
{
  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool Has(this string value, char chr, int index = 0)
  {
    return value != null && value.Length > index && (int) value[index] == (int) chr;
  }

  internal static string GetCompatiblity(this QuirksMode mode)
  {
    return CustomAttributeExtensions.GetCustomAttribute<DomDescriptionAttribute>((MemberInfo) IntrospectionExtensions.GetTypeInfo(typeof (QuirksMode)).GetDeclaredField(mode.ToString()))?.Description ?? "CSS1Compat";
  }

  public static string HtmlLower(this string value)
  {
    int length = value.Length;
    for (int index1 = 0; index1 < length; ++index1)
    {
      char c = value[index1];
      if (c.IsUppercaseAscii())
      {
        char[] chArray = new char[length];
        for (int index2 = 0; index2 < index1; ++index2)
          chArray[index2] = value[index2];
        chArray[index1] = char.ToLowerInvariant(c);
        for (int index3 = index1 + 1; index3 < length; ++index3)
        {
          char lowerInvariant = value[index3];
          if (lowerInvariant.IsUppercaseAscii())
            lowerInvariant = char.ToLowerInvariant(lowerInvariant);
          chArray[index3] = lowerInvariant;
        }
        return new string(chArray);
      }
    }
    return value;
  }

  public static Sandboxes ParseSecuritySettings(this string value, bool allowFullscreen = false)
  {
    string[] list = value.SplitSpaces();
    Sandboxes securitySettings = Sandboxes.Navigation | Sandboxes.Plugins | Sandboxes.DocumentDomain;
    if (!list.Contains("allow-popups", StringComparison.OrdinalIgnoreCase))
      securitySettings |= Sandboxes.AuxiliaryNavigation;
    if (!list.Contains("allow-top-navigation", StringComparison.OrdinalIgnoreCase))
      securitySettings |= Sandboxes.TopLevelNavigation;
    if (!list.Contains("allow-same-origin", StringComparison.OrdinalIgnoreCase))
      securitySettings |= Sandboxes.Origin;
    if (!list.Contains("allow-forms", StringComparison.OrdinalIgnoreCase))
      securitySettings |= Sandboxes.Forms;
    if (!list.Contains("allow-pointer-lock", StringComparison.OrdinalIgnoreCase))
      securitySettings |= Sandboxes.PointerLock;
    if (!list.Contains("allow-scripts", StringComparison.OrdinalIgnoreCase))
      securitySettings = securitySettings | Sandboxes.Scripts | Sandboxes.AutomaticFeatures;
    if (!list.Contains("allow-presentation", StringComparison.OrdinalIgnoreCase))
      securitySettings |= Sandboxes.Presentation;
    if (!allowFullscreen)
      securitySettings |= Sandboxes.Fullscreen;
    return securitySettings;
  }

  public static T ToEnum<T>(this string value, T defaultValue) where T : struct, IComparable
  {
    T result;
    return !string.IsNullOrEmpty(value) && Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
  }

  public static double ToDouble(this string value, double defaultValue = 0.0)
  {
    double result;
    return !string.IsNullOrEmpty(value) && double.TryParse(value, NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result) ? result : defaultValue;
  }

  public static int ToInteger(this string value, int defaultValue = 0)
  {
    int result;
    return !string.IsNullOrEmpty(value) && int.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : defaultValue;
  }

  public static uint ToInteger(this string value, uint defaultValue = 0)
  {
    uint result;
    return !string.IsNullOrEmpty(value) && uint.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : defaultValue;
  }

  public static bool ToBoolean(this string value, bool defaultValue = false)
  {
    bool result;
    return !string.IsNullOrEmpty(value) && bool.TryParse(value, out result) ? result : defaultValue;
  }

  public static string ReplaceFirst(this string text, string search, string replace)
  {
    int length = text.IndexOf(search);
    return length < 0 ? text : text.Substring(0, length) + replace + text.Substring(length + search.Length);
  }

  public static string CollapseAndStrip(this string str)
  {
    char[] chArray = new char[str.Length];
    bool flag = true;
    int length = 0;
    for (int index = 0; index < str.Length; ++index)
    {
      if (str[index].IsSpaceCharacter())
      {
        if (!flag)
        {
          flag = true;
          chArray[length++] = ' ';
        }
      }
      else
      {
        flag = false;
        chArray[length++] = str[index];
      }
    }
    if (flag && length > 0)
      --length;
    return new string(chArray, 0, length);
  }

  public static string Collapse(this string str)
  {
    List<char> charList = new List<char>();
    bool flag = false;
    for (int index = 0; index < str.Length; ++index)
    {
      if (str[index].IsSpaceCharacter())
      {
        if (!flag)
        {
          charList.Add(' ');
          flag = true;
        }
      }
      else
      {
        flag = false;
        charList.Add(str[index]);
      }
    }
    return new string(charList.ToArray());
  }

  public static bool Contains(this string[] list, string element, StringComparison comparison = StringComparison.Ordinal)
  {
    for (int index = 0; index < list.Length; ++index)
    {
      if (list[index].Equals(element, comparison))
        return true;
    }
    return false;
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool Is(this string current, string other)
  {
    return string.Equals(current, other, StringComparison.Ordinal);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool Isi(this string current, string other)
  {
    return string.Equals(current, other, StringComparison.OrdinalIgnoreCase);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsOneOf(this string element, string item1, string item2)
  {
    return element.Is(item1) || element.Is(item2);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsOneOf(this string element, string item1, string item2, string item3)
  {
    return element.Is(item1) || element.Is(item2) || element.Is(item3);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsOneOf(
    this string element,
    string item1,
    string item2,
    string item3,
    string item4)
  {
    return element.Is(item1) || element.Is(item2) || element.Is(item3) || element.Is(item4);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool IsOneOf(
    this string element,
    string item1,
    string item2,
    string item3,
    string item4,
    string item5)
  {
    return element.Is(item1) || element.Is(item2) || element.Is(item3) || element.Is(item4) || element.Is(item5);
  }

  public static string StripLineBreaks(this string str)
  {
    char[] charArray = str.ToCharArray();
    int num = 0;
    int length = charArray.Length;
    int index = 0;
    while (index < length)
    {
      charArray[index] = charArray[index + num];
      if (charArray[index].IsLineBreak())
      {
        ++num;
        --length;
      }
      else
        ++index;
    }
    return new string(charArray, 0, length);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static string StripLeadingTrailingSpaces(this string str)
  {
    return str.ToCharArray().StripLeadingTrailingSpaces();
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static string StripLeadingTrailingSpaces(this char[] array)
  {
    int startIndex = 0;
    int index = array.Length - 1;
    while (startIndex < array.Length && array[startIndex].IsSpaceCharacter())
      ++startIndex;
    while (index > startIndex && array[index].IsSpaceCharacter())
      --index;
    return new string(array, startIndex, 1 + index - startIndex);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static string[] SplitWithoutTrimming(this string str, char c)
  {
    return str.ToCharArray().SplitWithoutTrimming(c);
  }

  public static string[] SplitWithoutTrimming(this char[] chars, char c)
  {
    List<string> stringList = new List<string>();
    int startIndex = 0;
    for (int index = 0; index < chars.Length; ++index)
    {
      if ((int) chars[index] == (int) c)
      {
        if (index > startIndex)
          stringList.Add(new string(chars, startIndex, index - startIndex));
        startIndex = index + 1;
      }
    }
    if (chars.Length > startIndex)
      stringList.Add(new string(chars, startIndex, chars.Length - startIndex));
    return stringList.ToArray();
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static string[] SplitCommas(this string str) => str.SplitWithTrimming(',');

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static bool HasHyphen(this string str, string value, StringComparison comparison = StringComparison.Ordinal)
  {
    if (string.Equals(str, value, comparison))
      return true;
    return str.Length > value.Length && str.StartsWith(value, comparison) && str[value.Length] == '-';
  }

  public static string[] SplitSpaces(this string str)
  {
    List<string> stringList = new List<string>();
    List<char> charList = new List<char>();
    char[] charArray = str.ToCharArray();
    for (int index = 0; index <= charArray.Length; ++index)
    {
      if (index == charArray.Length || charArray[index].IsSpaceCharacter())
      {
        if (charList.Count > 0)
        {
          string str1 = charList.ToArray().StripLeadingTrailingSpaces();
          if (str1.Length != 0)
            stringList.Add(str1);
          charList.Clear();
        }
      }
      else
        charList.Add(charArray[index]);
    }
    return stringList.ToArray();
  }

  public static string[] SplitWithTrimming(this string str, char c)
  {
    List<string> stringList = new List<string>();
    List<char> charList = new List<char>();
    char[] charArray = str.ToCharArray();
    for (int index = 0; index <= charArray.Length; ++index)
    {
      if (index == charArray.Length || (int) charArray[index] == (int) c)
      {
        if (charList.Count > 0)
        {
          string str1 = charList.ToArray().StripLeadingTrailingSpaces();
          if (str1.Length != 0)
            stringList.Add(str1);
          charList.Clear();
        }
      }
      else
        charList.Add(charArray[index]);
    }
    return stringList.ToArray();
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static int FromHex(this string s)
  {
    return int.Parse(s, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static int FromDec(this string s)
  {
    return int.Parse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static string HtmlEncode(this string value, Encoding encoding) => value;

  public static string CssString(this string value)
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    sb.Append('"');
    if (!string.IsNullOrEmpty(value))
    {
      for (int index = 0; index < value.Length; ++index)
      {
        char ch = value[index];
        switch (ch)
        {
          case char.MinValue:
            throw new DomException(DomError.InvalidCharacter);
          case '"':
          case '\\':
            sb.Append('\\').Append(ch);
            break;
          default:
            if (ch.IsInRange(1, 31 /*0x1F*/) || ch == '{')
            {
              sb.Append('\\').Append(ch.ToHex()).Append(index + 1 != value.Length ? " " : "");
              break;
            }
            sb.Append(ch);
            break;
        }
      }
    }
    sb.Append('"');
    return sb.ToPool();
  }

  public static string CssFunction(this string value, string argument) => $"{value}({argument})";

  public static string UrlEncode(this byte[] content)
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    for (int index = 0; index < content.Length; ++index)
    {
      char c = (char) content[index];
      switch (c)
      {
        case ' ':
          sb.Append('+');
          break;
        case '*':
        case '-':
        case '.':
        case '_':
        case '~':
          sb.Append(c);
          break;
        default:
          if (!c.IsAlphanumericAscii())
          {
            sb.Append('%').Append(content[index].ToString("X2"));
            break;
          }
          goto case '*';
      }
    }
    return sb.ToPool();
  }

  public static byte[] UrlDecode(this string value)
  {
    MemoryStream memoryStream = new MemoryStream();
    for (int index = 0; index < value.Length; ++index)
    {
      char ch = value[index];
      switch (ch)
      {
        case '%':
          if (index + 2 >= value.Length)
            throw new FormatException();
          int num1;
          byte num2 = (byte) (16 /*0x10*/ * value[num1 = index + 1].FromHex() + value[index = num1 + 1].FromHex());
          memoryStream.WriteByte(num2);
          break;
        case '+':
          byte num3 = 32 /*0x20*/;
          memoryStream.WriteByte(num3);
          break;
        default:
          byte num4 = (byte) ch;
          memoryStream.WriteByte(num4);
          break;
      }
    }
    return memoryStream.ToArray();
  }

  public static string NormalizeLineEndings(this string value)
  {
    if (string.IsNullOrEmpty(value))
      return value;
    StringBuilder sb = StringBuilderPool.Obtain();
    bool flag1 = false;
    for (int index = 0; index < value.Length; ++index)
    {
      char ch = value[index];
      bool flag2 = ch == '\n';
      if (flag1 && !flag2)
        sb.Append('\n');
      else if (!flag1 & flag2)
        sb.Append('\r');
      flag1 = ch == '\r';
      sb.Append(ch);
    }
    if (flag1)
      sb.Append('\n');
    return sb.ToPool();
  }

  public static string ToEncodingType(this string encType)
  {
    return !encType.Isi(MimeTypeNames.Plain) && !encType.Isi(MimeTypeNames.MultipartForm) && !encType.Isi(MimeTypeNames.ApplicationJson) ? (string) null : encType.ToLowerInvariant();
  }

  public static string ToFormMethod(this string method)
  {
    return !method.Isi(FormMethodNames.Get) && !method.Isi(FormMethodNames.Post) && !method.Isi(FormMethodNames.Dialog) ? (string) null : method.ToLowerInvariant();
  }
}
