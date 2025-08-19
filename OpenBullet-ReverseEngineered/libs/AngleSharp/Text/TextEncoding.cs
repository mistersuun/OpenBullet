// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.TextEncoding
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Text;

public static class TextEncoding
{
  public static readonly Encoding Utf8 = (Encoding) new UTF8Encoding(false);
  public static readonly Encoding Utf16Be = (Encoding) new UnicodeEncoding(true, false);
  public static readonly Encoding Utf16Le = (Encoding) new UnicodeEncoding(false, false);
  public static readonly Encoding Utf32Le = TextEncoding.GetEncoding("UTF-32LE");
  public static readonly Encoding Utf32Be = TextEncoding.GetEncoding("UTF-32BE");
  public static readonly Encoding Gb18030 = TextEncoding.GetEncoding("GB18030");
  public static readonly Encoding Big5 = TextEncoding.GetEncoding("big5");
  public static readonly Encoding Windows874 = TextEncoding.GetEncoding("windows-874");
  public static readonly Encoding Windows1250 = TextEncoding.GetEncoding("windows-1250");
  public static readonly Encoding Windows1251 = TextEncoding.GetEncoding("windows-1251");
  public static readonly Encoding Windows1252 = TextEncoding.GetEncoding("windows-1252");
  public static readonly Encoding Windows1253 = TextEncoding.GetEncoding("windows-1253");
  public static readonly Encoding Windows1254 = TextEncoding.GetEncoding("windows-1254");
  public static readonly Encoding Windows1255 = TextEncoding.GetEncoding("windows-1255");
  public static readonly Encoding Windows1256 = TextEncoding.GetEncoding("windows-1256");
  public static readonly Encoding Windows1257 = TextEncoding.GetEncoding("windows-1257");
  public static readonly Encoding Windows1258 = TextEncoding.GetEncoding("windows-1258");
  public static readonly Encoding Latin2 = TextEncoding.GetEncoding("iso-8859-2");
  public static readonly Encoding Latin3 = TextEncoding.GetEncoding("iso-8859-3");
  public static readonly Encoding Latin4 = TextEncoding.GetEncoding("iso-8859-4");
  public static readonly Encoding Latin5 = TextEncoding.GetEncoding("iso-8859-5");
  public static readonly Encoding Latin13 = TextEncoding.GetEncoding("iso-8859-13");
  public static readonly Encoding UsAscii = TextEncoding.GetEncoding("us-ascii");
  public static readonly Encoding Korean = TextEncoding.GetEncoding("ks_c_5601-1987");
  private static readonly Dictionary<string, Encoding> encodings = TextEncoding.CreateEncodings();

  public static bool IsUnicode(this Encoding encoding)
  {
    return encoding == TextEncoding.Utf16Be || encoding == TextEncoding.Utf16Le;
  }

  public static Encoding Parse(string content)
  {
    string charset = string.Empty;
    int num1 = 0;
    for (int startIndex = num1; startIndex < content.Length - 7; ++startIndex)
    {
      if (content.Substring(startIndex).StartsWith(AttributeNames.Charset, StringComparison.OrdinalIgnoreCase))
      {
        num1 = startIndex + 7;
        break;
      }
    }
    if (num1 > 0 && num1 < content.Length)
    {
      for (int index = num1; index < content.Length - 1 && content[index].IsSpaceCharacter(); ++index)
        ++num1;
      if (content[num1] != '=')
        return TextEncoding.Parse(content.Substring(num1));
      int num2 = num1 + 1;
      for (int index = num2; index < content.Length && content[index].IsSpaceCharacter(); ++index)
        ++num2;
      if (num2 < content.Length)
      {
        if (content[num2] == '"')
        {
          content = content.Substring(num2 + 1);
          int length = content.IndexOf('"');
          if (length != -1)
            charset = content.Substring(0, length);
        }
        else if (content[num2] == '\'')
        {
          content = content.Substring(num2 + 1);
          int length = content.IndexOf('\'');
          if (length != -1)
            charset = content.Substring(0, length);
        }
        else
        {
          content = content.Substring(num2);
          int length = 0;
          for (int index = 0; index < content.Length && !content[index].IsSpaceCharacter() && content[index] != ';'; ++index)
            ++length;
          charset = content.Substring(0, length);
        }
      }
    }
    return !TextEncoding.IsSupported(charset) ? (Encoding) null : TextEncoding.Resolve(charset);
  }

  public static bool IsSupported(string charset) => TextEncoding.encodings.ContainsKey(charset);

  public static Encoding Resolve(string charset)
  {
    Encoding encoding;
    return charset != null && TextEncoding.encodings.TryGetValue(charset, out encoding) ? encoding : TextEncoding.Utf8;
  }

  private static Encoding GetEncoding(string name)
  {
    try
    {
      return Encoding.GetEncoding(name);
    }
    catch
    {
      return TextEncoding.Utf8;
    }
  }

  private static Dictionary<string, Encoding> CreateEncodings()
  {
    Dictionary<string, Encoding> encodings = new Dictionary<string, Encoding>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    encodings.Add("unicode-1-1-utf-8", TextEncoding.Utf8);
    encodings.Add("utf-8", TextEncoding.Utf8);
    encodings.Add("utf8", TextEncoding.Utf8);
    encodings.Add("utf-16be", TextEncoding.Utf16Be);
    encodings.Add("utf-16", TextEncoding.Utf16Le);
    encodings.Add("utf-16le", TextEncoding.Utf16Le);
    encodings.Add("dos-874", TextEncoding.Windows874);
    encodings.Add("iso-8859-11", TextEncoding.Windows874);
    encodings.Add("iso8859-11", TextEncoding.Windows874);
    encodings.Add("iso885911", TextEncoding.Windows874);
    encodings.Add("tis-620", TextEncoding.Windows874);
    encodings.Add("windows-874", TextEncoding.Windows874);
    encodings.Add("cp1250", TextEncoding.Windows1250);
    encodings.Add("windows-1250", TextEncoding.Windows1250);
    encodings.Add("x-cp1250", TextEncoding.Windows1250);
    encodings.Add("cp1251", TextEncoding.Windows1251);
    encodings.Add("windows-1251", TextEncoding.Windows1251);
    encodings.Add("x-cp1251", TextEncoding.Windows1251);
    encodings.Add("x-user-defined", TextEncoding.Windows1252);
    encodings.Add("ansi_x3.4-1968", TextEncoding.Windows1252);
    encodings.Add("ascii", TextEncoding.Windows1252);
    encodings.Add("cp1252", TextEncoding.Windows1252);
    encodings.Add("cp819", TextEncoding.Windows1252);
    encodings.Add("csisolatin1", TextEncoding.Windows1252);
    encodings.Add("ibm819", TextEncoding.Windows1252);
    encodings.Add("iso-8859-1", TextEncoding.Windows1252);
    encodings.Add("iso-ir-100", TextEncoding.Windows1252);
    encodings.Add("iso8859-1", TextEncoding.Windows1252);
    encodings.Add("iso88591", TextEncoding.Windows1252);
    encodings.Add("iso_8859-1", TextEncoding.Windows1252);
    encodings.Add("iso_8859-1:1987", TextEncoding.Windows1252);
    encodings.Add("l1", TextEncoding.Windows1252);
    encodings.Add("latin1", TextEncoding.Windows1252);
    encodings.Add("us-ascii", TextEncoding.Windows1252);
    encodings.Add("windows-1252", TextEncoding.Windows1252);
    encodings.Add("x-cp1252", TextEncoding.Windows1252);
    encodings.Add("cp1253", TextEncoding.Windows1253);
    encodings.Add("windows-1253", TextEncoding.Windows1253);
    encodings.Add("x-cp1253", TextEncoding.Windows1253);
    encodings.Add("cp1254", TextEncoding.Windows1254);
    encodings.Add("csisolatin5", TextEncoding.Windows1254);
    encodings.Add("iso-8859-9", TextEncoding.Windows1254);
    encodings.Add("iso-ir-148", TextEncoding.Windows1254);
    encodings.Add("iso8859-9", TextEncoding.Windows1254);
    encodings.Add("iso88599", TextEncoding.Windows1254);
    encodings.Add("iso_8859-9", TextEncoding.Windows1254);
    encodings.Add("iso_8859-9:1989", TextEncoding.Windows1254);
    encodings.Add("l5", TextEncoding.Windows1254);
    encodings.Add("latin5", TextEncoding.Windows1254);
    encodings.Add("windows-1254", TextEncoding.Windows1254);
    encodings.Add("x-cp1254", TextEncoding.Windows1254);
    encodings.Add("cp1255", TextEncoding.Windows1255);
    encodings.Add("windows-1255", TextEncoding.Windows1255);
    encodings.Add("x-cp1255", TextEncoding.Windows1255);
    encodings.Add("cp1256", TextEncoding.Windows1256);
    encodings.Add("windows-1256", TextEncoding.Windows1256);
    encodings.Add("x-cp1256", TextEncoding.Windows1256);
    encodings.Add("cp1257", TextEncoding.Windows1257);
    encodings.Add("windows-1257", TextEncoding.Windows1257);
    encodings.Add("x-cp1257", TextEncoding.Windows1257);
    encodings.Add("cp1258", TextEncoding.Windows1258);
    encodings.Add("windows-1258", TextEncoding.Windows1258);
    encodings.Add("x-cp1258", TextEncoding.Windows1258);
    Encoding encoding1 = TextEncoding.GetEncoding("macintosh");
    encodings.Add("csmacintosh", encoding1);
    encodings.Add("mac", encoding1);
    encodings.Add("macintosh", encoding1);
    encodings.Add("x-mac-roman", encoding1);
    Encoding encoding2 = TextEncoding.GetEncoding("x-mac-cyrillic");
    encodings.Add("x-mac-cyrillic", encoding2);
    encodings.Add("x-mac-ukrainian", encoding2);
    Encoding encoding3 = TextEncoding.GetEncoding("cp866");
    encodings.Add("866", encoding3);
    encodings.Add("cp866", encoding3);
    encodings.Add("csibm866", encoding3);
    encodings.Add("ibm866", encoding3);
    encodings.Add("csisolatin2", TextEncoding.Latin2);
    encodings.Add("iso-8859-2", TextEncoding.Latin2);
    encodings.Add("iso-ir-101", TextEncoding.Latin2);
    encodings.Add("iso8859-2", TextEncoding.Latin2);
    encodings.Add("iso88592", TextEncoding.Latin2);
    encodings.Add("iso_8859-2", TextEncoding.Latin2);
    encodings.Add("iso_8859-2:1987", TextEncoding.Latin2);
    encodings.Add("l2", TextEncoding.Latin2);
    encodings.Add("latin2", TextEncoding.Latin2);
    encodings.Add("csisolatin3", TextEncoding.Latin3);
    encodings.Add("iso-8859-3", TextEncoding.Latin3);
    encodings.Add("iso-ir-109", TextEncoding.Latin3);
    encodings.Add("iso8859-3", TextEncoding.Latin3);
    encodings.Add("iso88593", TextEncoding.Latin3);
    encodings.Add("iso_8859-3", TextEncoding.Latin3);
    encodings.Add("iso_8859-3:1988", TextEncoding.Latin3);
    encodings.Add("l3", TextEncoding.Latin3);
    encodings.Add("latin3", TextEncoding.Latin3);
    encodings.Add("csisolatin4", TextEncoding.Latin4);
    encodings.Add("iso-8859-4", TextEncoding.Latin4);
    encodings.Add("iso-ir-110", TextEncoding.Latin4);
    encodings.Add("iso8859-4", TextEncoding.Latin4);
    encodings.Add("iso88594", TextEncoding.Latin4);
    encodings.Add("iso_8859-4", TextEncoding.Latin4);
    encodings.Add("iso_8859-4:1988", TextEncoding.Latin4);
    encodings.Add("l4", TextEncoding.Latin4);
    encodings.Add("latin4", TextEncoding.Latin4);
    encodings.Add("csisolatincyrillic", TextEncoding.Latin5);
    encodings.Add("cyrillic", TextEncoding.Latin5);
    encodings.Add("iso-8859-5", TextEncoding.Latin5);
    encodings.Add("iso-ir-144", TextEncoding.Latin5);
    encodings.Add("iso8859-5", TextEncoding.Latin5);
    encodings.Add("iso88595", TextEncoding.Latin5);
    encodings.Add("iso_8859-5", TextEncoding.Latin5);
    encodings.Add("iso_8859-5:1988", TextEncoding.Latin5);
    Encoding encoding4 = TextEncoding.GetEncoding("iso-8859-6");
    encodings.Add("arabic", encoding4);
    encodings.Add("asmo-708", encoding4);
    encodings.Add("csiso88596e", encoding4);
    encodings.Add("csiso88596i", encoding4);
    encodings.Add("csisolatinarabic", encoding4);
    encodings.Add("ecma-114", encoding4);
    encodings.Add("iso-8859-6", encoding4);
    encodings.Add("iso-8859-6-e", encoding4);
    encodings.Add("iso-8859-6-i", encoding4);
    encodings.Add("iso-ir-127", encoding4);
    encodings.Add("iso8859-6", encoding4);
    encodings.Add("iso88596", encoding4);
    encodings.Add("iso_8859-6", encoding4);
    encodings.Add("iso_8859-6:1987", encoding4);
    Encoding encoding5 = TextEncoding.GetEncoding("iso-8859-7");
    encodings.Add("csisolatingreek", encoding5);
    encodings.Add("ecma-118", encoding5);
    encodings.Add("elot_928", encoding5);
    encodings.Add("greek", encoding5);
    encodings.Add("greek8", encoding5);
    encodings.Add("iso-8859-7", encoding5);
    encodings.Add("iso-ir-126", encoding5);
    encodings.Add("iso8859-7", encoding5);
    encodings.Add("iso88597", encoding5);
    encodings.Add("iso_8859-7", encoding5);
    encodings.Add("iso_8859-7:1987", encoding5);
    encodings.Add("sun_eu_greek", encoding5);
    Encoding encoding6 = TextEncoding.GetEncoding("iso-8859-8");
    encodings.Add("csiso88598e", encoding6);
    encodings.Add("csisolatinhebrew", encoding6);
    encodings.Add("hebrew", encoding6);
    encodings.Add("iso-8859-8", encoding6);
    encodings.Add("iso-8859-8-e", encoding6);
    encodings.Add("iso-ir-138", encoding6);
    encodings.Add("iso8859-8", encoding6);
    encodings.Add("iso88598", encoding6);
    encodings.Add("iso_8859-8", encoding6);
    encodings.Add("iso_8859-8:1988", encoding6);
    encodings.Add("visual", encoding6);
    Encoding encoding7 = TextEncoding.GetEncoding("iso-8859-8-i");
    encodings.Add("csiso88598i", encoding7);
    encodings.Add("iso-8859-8-i", encoding7);
    encodings.Add("logical", encoding7);
    Encoding encoding8 = TextEncoding.GetEncoding("iso-8859-13");
    encodings.Add("iso-8859-13", encoding8);
    encodings.Add("iso8859-13", encoding8);
    encodings.Add("iso885913", encoding8);
    Encoding encoding9 = TextEncoding.GetEncoding("iso-8859-15");
    encodings.Add("csisolatin9", encoding9);
    encodings.Add("iso-8859-15", encoding9);
    encodings.Add("iso8859-15", encoding9);
    encodings.Add("iso885915", encoding9);
    encodings.Add("iso_8859-15", encoding9);
    encodings.Add("l9", encoding9);
    Encoding encoding10 = TextEncoding.GetEncoding("koi8-r");
    encodings.Add("cskoi8r", encoding10);
    encodings.Add("koi", encoding10);
    encodings.Add("koi8", encoding10);
    encodings.Add("koi8-r", encoding10);
    encodings.Add("koi8_r", encoding10);
    encodings.Add("koi8-u", TextEncoding.GetEncoding("koi8-u"));
    Encoding encoding11 = TextEncoding.GetEncoding("x-cp20936");
    encodings.Add("chinese", encoding11);
    encodings.Add("csgb2312", encoding11);
    encodings.Add("csiso58gb231280", encoding11);
    encodings.Add("gb2312", encoding11);
    encodings.Add("gb_2312", encoding11);
    encodings.Add("gb_2312-80", encoding11);
    encodings.Add("gbk", encoding11);
    encodings.Add("iso-ir-58", encoding11);
    encodings.Add("x-gbk", encoding11);
    encodings.Add("hz-gb-2312", TextEncoding.GetEncoding("hz-gb-2312"));
    encodings.Add("gb18030", TextEncoding.Gb18030);
    encodings.Add("big5", TextEncoding.Big5);
    encodings.Add("big5-hkscs", TextEncoding.Big5);
    encodings.Add("cn-big5", TextEncoding.Big5);
    encodings.Add("csbig5", TextEncoding.Big5);
    encodings.Add("x-x-big5", TextEncoding.Big5);
    Encoding encoding12 = TextEncoding.GetEncoding("iso-2022-jp");
    encodings.Add("csiso2022jp", encoding12);
    encodings.Add("iso-2022-jp", encoding12);
    Encoding encoding13 = TextEncoding.GetEncoding("iso-2022-kr");
    encodings.Add("csiso2022kr", encoding13);
    encodings.Add("iso-2022-kr", encoding13);
    Encoding encoding14 = TextEncoding.GetEncoding("iso-2022-cn");
    encodings.Add("iso-2022-cn", encoding14);
    encodings.Add("iso-2022-cn-ext", encoding14);
    encodings.Add("shift_jis", TextEncoding.GetEncoding("shift_jis"));
    encodings.Add("euc-jp", TextEncoding.GetEncoding("euc-jp"));
    return encodings;
  }
}
