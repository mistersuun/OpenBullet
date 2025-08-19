// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonCodecs
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonCodecs
{
  public const string __doc__ = "Provides access to various codecs (ASCII, UTF7, UTF8, etc...)";
  internal const int EncoderIndex = 0;
  internal const int DecoderIndex = 1;
  internal const int StreamReaderIndex = 2;
  internal const int StreamWriterIndex = 3;
  private static Encoding utf32BeEncoding;

  public static object ascii_decode(CodeContext context, object input, string errors = "strict")
  {
    return (object) PythonCodecs.DoDecode(context, "ascii", PythonAsciiEncoding.Instance, input, errors, true);
  }

  public static object ascii_encode(object input, string errors = "strict")
  {
    return (object) PythonCodecs.DoEncode(PythonAsciiEncoding.Instance, input, errors);
  }

  public static object charbuffer_encode(string input)
  {
    throw PythonOps.NotImplementedError(nameof (charbuffer_encode));
  }

  public static EncodingMap charmap_build(string decoding_table)
  {
    if (decoding_table.Length != 256 /*0x0100*/)
      throw PythonOps.TypeError("charmap_build expected 256 character string");
    EncodingMap encodingMap = new EncodingMap();
    for (int index = 0; index < decoding_table.Length; ++index)
      encodingMap.Mapping[(int) decoding_table[index]] = (char) index;
    return encodingMap;
  }

  public static PythonTuple charmap_encode(
    CodeContext context,
    [BytesConversion] string input,
    string errors,
    [NotNull] EncodingMap map)
  {
    return PythonCodecs.CharmapDecodeWorker(context, input, errors, (Encoding) new EncodingMapEncoding(map, errors), false);
  }

  public static PythonTuple charmap_encode(
    CodeContext context,
    [BytesConversion] string input,
    string errors = "strict",
    IDictionary<object, object> map = null)
  {
    Encoding e = map != null ? (Encoding) new CharmapEncoding(map, errors) : (Encoding) null;
    return PythonCodecs.CharmapDecodeWorker(context, input, errors, e, false);
  }

  public static PythonTuple charmap_decode(
    CodeContext context,
    [BytesConversion] string input,
    string errors,
    [NotNull] string map)
  {
    EncodingMap map1 = new EncodingMap();
    for (int index = 0; index < map.Length; ++index)
      map1.Mapping[index] = map[index];
    return PythonCodecs.CharmapDecodeWorker(context, input, errors, (Encoding) new EncodingMapEncoding(map1, errors), true);
  }

  public static PythonTuple charmap_decode(
    CodeContext context,
    [BytesConversion] string input,
    string errors = "strict",
    IDictionary<object, object> map = null)
  {
    Encoding e = map != null ? (Encoding) new CharmapEncoding(map, errors) : (Encoding) null;
    return PythonCodecs.CharmapDecodeWorker(context, input, errors, e, true);
  }

  private static PythonTuple CharmapDecodeWorker(
    CodeContext context,
    string input,
    string errors,
    Encoding e,
    bool isDecode)
  {
    if (input.Length == 0)
      return PythonTuple.MakeTuple((object) string.Empty, (object) 0);
    string encoding = "charmap";
    if (e == null)
    {
      e = Encoding.GetEncoding("iso-8859-1");
      encoding = "latin-1";
    }
    string str = isDecode ? StringOps.DoDecode(context, input, errors, encoding, e) : StringOps.DoEncode(context, input, errors, encoding, e);
    return PythonTuple.MakeTuple((object) str, (object) str.Length);
  }

  public static object decode(CodeContext context, object obj, string encoding = null, string errors = "strict")
  {
    if (encoding == null)
      encoding = context.LanguageContext.DefaultEncoding.EncodingName;
    PythonTuple pythonTuple = PythonCodecs.lookup(context, encoding);
    return PythonOps.GetIndex(context, PythonCalls.Call(context, pythonTuple[1], obj, (object) errors), (object) 0);
  }

  public static object encode(CodeContext context, object obj, string encoding = null, string errors = "strict")
  {
    if (encoding == null)
      encoding = context.LanguageContext.DefaultEncoding.EncodingName;
    PythonTuple pythonTuple = PythonCodecs.lookup(context, encoding);
    return PythonOps.GetIndex(context, PythonCalls.Call(context, pythonTuple[0], obj, (object) errors), (object) 0);
  }

  public static object escape_decode(string text, string errors = "strict")
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < text.Length; ++index)
    {
      if (text[index] == '\\')
      {
        if (index == text.Length - 1)
          throw PythonOps.ValueError("\\ at end of string");
        switch (text[++index])
        {
          case '\n':
            continue;
          case '\\':
            stringBuilder.Append('\\');
            continue;
          case 'a':
            stringBuilder.Append('\a');
            continue;
          case 'b':
            stringBuilder.Append('\b');
            continue;
          case 'f':
            stringBuilder.Append('\f');
            continue;
          case 'n':
            stringBuilder.Append('\n');
            continue;
          case 'r':
            stringBuilder.Append('\r');
            continue;
          case 't':
            stringBuilder.Append('\t');
            continue;
          case 'v':
            stringBuilder.Append('\v');
            continue;
          case 'x':
            int val1;
            int val2;
            if (index >= text.Length - 2 || !PythonCodecs.CharToInt(text[index], out val1) || !PythonCodecs.CharToInt(text[index + 1], out val2))
            {
              switch (errors)
              {
                case "strict":
                  if (index >= text.Length - 2)
                    throw PythonOps.ValueError("invalid character value");
                  throw PythonOps.ValueError("invalid hexadecimal digit");
                case "replace":
                  stringBuilder.Append("?");
                  --index;
                  while (index < text.Length - 1)
                    stringBuilder.Append(text[index++]);
                  continue;
                default:
                  throw PythonOps.ValueError("decoding error; unknown error handling code: " + errors);
              }
            }
            else
            {
              stringBuilder.Append(val1 * 16 /*0x10*/ + val2);
              index += 2;
              continue;
            }
          default:
            stringBuilder.Append("\\" + text[index].ToString());
            continue;
        }
      }
      else
        stringBuilder.Append(text[index]);
    }
    return (object) PythonTuple.MakeTuple((object) stringBuilder.ToString(), (object) text.Length);
  }

  private static bool CharToInt(char ch, out int val)
  {
    if (char.IsDigit(ch))
    {
      val = (int) ch - 48 /*0x30*/;
      return true;
    }
    ch = char.ToUpper(ch);
    if (ch >= 'A' && ch <= 'F')
    {
      val = (int) ch - 65 + 10;
      return true;
    }
    val = 0;
    return false;
  }

  public static PythonTuple escape_encode(string text, string errors = "strict")
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < text.Length; ++index)
    {
      switch (text[index])
      {
        case '\t':
          stringBuilder.Append("\\t");
          break;
        case '\n':
          stringBuilder.Append("\\n");
          break;
        case '\r':
          stringBuilder.Append("\\r");
          break;
        case '\'':
          stringBuilder.Append("\\'");
          break;
        case '\\':
          stringBuilder.Append("\\\\");
          break;
        default:
          if (text[index] < ' ' || text[index] >= '\u007F')
          {
            stringBuilder.AppendFormat("\\x{0:x2}", (object) (int) text[index]);
            break;
          }
          stringBuilder.Append(text[index]);
          break;
      }
    }
    return PythonTuple.MakeTuple((object) stringBuilder.ToString(), (object) stringBuilder.Length);
  }

  public static object latin_1_decode(CodeContext context, object input, string errors = "strict")
  {
    return (object) PythonCodecs.DoDecode(context, (string) null, Encoding.GetEncoding("iso-8859-1"), input, errors, true);
  }

  public static object latin_1_encode(object input, string errors = "strict")
  {
    return (object) PythonCodecs.DoEncode(Encoding.GetEncoding("iso-8859-1"), input, errors);
  }

  public static PythonTuple lookup(CodeContext context, string encoding)
  {
    return PythonOps.LookupEncoding(context, encoding);
  }

  [LightThrowing]
  public static object lookup_error(CodeContext context, string name)
  {
    return PythonOps.LookupEncodingError(context, name);
  }

  [PythonHidden(PlatformsAttribute.PlatformFamily.Unix)]
  public static PythonTuple mbcs_decode(
    CodeContext context,
    string input,
    string errors = "strict",
    bool final = false)
  {
    return PythonTuple.MakeTuple((object) StringOps.decode(context, input, (object) Encoding.GetEncoding(0), "replace"), (object) Builtin.len(input));
  }

  [PythonHidden(PlatformsAttribute.PlatformFamily.Unix)]
  public static PythonTuple mbcs_encode(CodeContext context, string input, string errors = "strict")
  {
    return PythonTuple.MakeTuple((object) StringOps.encode(context, input, (object) Encoding.GetEncoding(0), "replace"), (object) Builtin.len(input));
  }

  public static PythonTuple raw_unicode_escape_decode(
    CodeContext context,
    object input,
    string errors = "strict")
  {
    return PythonTuple.MakeTuple((object) StringOps.decode(context, Converter.ConvertToString(input), (object) "raw-unicode-escape", errors), (object) Builtin.len(input));
  }

  public static PythonTuple raw_unicode_escape_encode(
    CodeContext context,
    object input,
    string errors = "strict")
  {
    return PythonTuple.MakeTuple((object) StringOps.encode(context, Converter.ConvertToString(input), (object) "raw-unicode-escape", errors), (object) Builtin.len(input));
  }

  public static PythonTuple readbuffer_encode([BytesConversion] string input, string errors = null)
  {
    return PythonTuple.MakeTuple((object) input, (object) input.Length);
  }

  public static void register(CodeContext context, object search_function)
  {
    PythonOps.RegisterEncoding(context, search_function);
  }

  public static void register_error(CodeContext context, string name, object handler)
  {
    PythonOps.RegisterEncodingError(context, name, handler);
  }

  public static PythonTuple unicode_escape_decode(string input)
  {
    throw PythonOps.NotImplementedError(nameof (unicode_escape_decode));
  }

  public static PythonTuple unicode_escape_encode(string input)
  {
    throw PythonOps.NotImplementedError(nameof (unicode_escape_encode));
  }

  public static PythonTuple unicode_internal_decode(
    CodeContext context,
    object input,
    string errors = "strict")
  {
    return PythonCodecs.utf_16_decode(context, input, errors);
  }

  public static PythonTuple unicode_internal_encode(object input, [Optional] string errors)
  {
    PythonTuple pythonTuple = PythonCodecs.DoEncode(Encoding.Unicode, input, errors, false);
    return PythonTuple.MakeTuple(pythonTuple[0], (object) ((int) pythonTuple[1] * 2));
  }

  public static PythonTuple utf_16_be_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.DoDecode(context, "utf16", Encoding.BigEndianUnicode, input, errors, final);
  }

  public static PythonTuple utf_16_be_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(Encoding.BigEndianUnicode, input, errors);
  }

  public static PythonTuple utf_16_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.DoDecode(context, "utf16", Encoding.Unicode, input, errors, final);
  }

  public static PythonTuple utf_16_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(Encoding.Unicode, input, errors, true);
  }

  public static PythonTuple utf_16_ex_decode(CodeContext context, object input, string errors = "strict")
  {
    return PythonCodecs.utf_16_ex_decode(context, input, errors, (object) null, (object) null);
  }

  public static PythonTuple utf_16_ex_decode(
    CodeContext context,
    object input,
    string errors,
    object unknown1,
    object unknown2)
  {
    byte[] preamble1 = Encoding.Unicode.GetPreamble();
    byte[] preamble2 = Encoding.BigEndianUnicode.GetPreamble();
    string str = Converter.ConvertToString(input);
    bool flag = true;
    if (str.Length > preamble1.Length)
    {
      for (int index = 0; index < preamble1.Length; ++index)
      {
        if ((int) (byte) str[index] != (int) preamble1[index])
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return PythonTuple.MakeTuple((object) string.Empty, (object) preamble1.Length, (object) -1);
      flag = true;
    }
    if (str.Length > preamble2.Length)
    {
      for (int index = 0; index < preamble2.Length; ++index)
      {
        if ((int) (byte) str[index] != (int) preamble2[index])
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return PythonTuple.MakeTuple((object) string.Empty, (object) preamble2.Length, (object) 1);
    }
    PythonTuple pythonTuple = PythonCodecs.utf_16_decode(context, input, errors);
    return PythonTuple.MakeTuple(pythonTuple[0], pythonTuple[1], (object) 0);
  }

  public static PythonTuple utf_16_le_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.utf_16_decode(context, input, errors, final);
  }

  public static PythonTuple utf_16_le_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(Encoding.Unicode, input, errors);
  }

  public static PythonTuple utf_7_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.DoDecode(context, "utf7", Encoding.UTF7, input, errors, final);
  }

  public static PythonTuple utf_7_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(Encoding.UTF7, input, errors);
  }

  public static PythonTuple utf_8_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.DoDecode(context, "utf8", Encoding.UTF8, input, errors, final);
  }

  public static PythonTuple utf_8_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(Encoding.UTF8, input, errors);
  }

  public static PythonTuple utf_32_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.DoDecode(context, "utf32", Encoding.UTF32, input, errors, final);
  }

  public static PythonTuple utf_32_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(Encoding.UTF32, input, errors, true);
  }

  public static PythonTuple utf_32_ex_decode(CodeContext context, object input, string errors = "strict")
  {
    return PythonCodecs.utf_32_ex_decode(context, input, errors, (object) null, (object) null);
  }

  public static PythonTuple utf_32_ex_decode(
    CodeContext context,
    object input,
    string errors,
    object byteorder,
    object final)
  {
    byte[] preamble = Encoding.UTF32.GetPreamble();
    string str = Converter.ConvertToString(input);
    bool flag = true;
    if (str.Length > preamble.Length)
    {
      for (int index = 0; index < preamble.Length; ++index)
      {
        if ((int) (byte) str[index] != (int) preamble[index])
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return PythonTuple.MakeTuple((object) string.Empty, (object) preamble.Length, (object) -1);
    }
    PythonTuple pythonTuple = PythonCodecs.utf_32_decode(context, input, errors);
    return PythonTuple.MakeTuple(pythonTuple[0], pythonTuple[1], (object) 0);
  }

  public static PythonTuple utf_32_le_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.utf_32_decode(context, input, errors, final);
  }

  public static PythonTuple utf_32_le_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(Encoding.UTF32, input, errors);
  }

  private static Encoding UTF32BE
  {
    get
    {
      if (PythonCodecs.utf32BeEncoding == null)
        PythonCodecs.utf32BeEncoding = (Encoding) new UTF32Encoding(true, true);
      return PythonCodecs.utf32BeEncoding;
    }
  }

  public static PythonTuple utf_32_be_decode(
    CodeContext context,
    object input,
    string errors = "strict",
    bool final = false)
  {
    return PythonCodecs.DoDecode(context, "utf32", PythonCodecs.UTF32BE, input, errors, final);
  }

  public static PythonTuple utf_32_be_encode(object input, string errors = "strict")
  {
    return PythonCodecs.DoEncode(PythonCodecs.UTF32BE, input, errors);
  }

  private static PythonTuple DoDecode(
    CodeContext context,
    string encodingName,
    Encoding encoding,
    object input,
    string errors,
    bool final)
  {
    string result;
    if (!Converter.TryConvertToString(input, out result))
      result = input is Bytes bytes ? bytes.ToString() : throw PythonOps.TypeErrorForBadInstance("argument 1 must be string, got {0}", input);
    int numBytes;
    return PythonTuple.MakeTuple((object) StringOps.DoDecode(context, result, errors, encodingName, encoding, final, out numBytes), (object) numBytes);
  }

  private static PythonTuple DoEncode(Encoding encoding, object input, string errors)
  {
    return PythonCodecs.DoEncode(encoding, input, errors, false);
  }

  private static PythonTuple DoEncode(
    Encoding encoding,
    object input,
    string errors,
    bool includePreamble)
  {
    string result;
    if (!Converter.TryConvertToString(input, out result))
      throw PythonOps.TypeErrorForBadInstance("cannot decode {0}", input);
    StringBuilder stringBuilder = new StringBuilder();
    encoding = (Encoding) encoding.Clone();
    encoding.EncoderFallback = EncoderFallback.ExceptionFallback;
    if (includePreamble)
    {
      foreach (char ch in encoding.GetPreamble())
        stringBuilder.Append(ch);
    }
    foreach (char ch in encoding.GetBytes(result))
      stringBuilder.Append(ch);
    return PythonTuple.MakeTuple((object) stringBuilder.ToString(), (object) result.Length);
  }
}
