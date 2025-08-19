// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonBinaryAscii
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonBinaryAscii
{
  public const string __doc__ = "Provides functions for converting between binary data encoded in various formats and ASCII.";
  private const int MAXLINESIZE = 76;
  private static readonly object _ErrorKey = new object();
  private static readonly object _IncompleteKey = new object();
  private const int IgnoreByte = -1;
  private const int EmptyByte = -2;
  private const int PadByte = -3;
  private const int InvalidByte = -4;
  private const int NoMoreBytes = -5;

  private static Exception Error(CodeContext context, params object[] args)
  {
    return PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState(PythonBinaryAscii._ErrorKey), args);
  }

  private static Exception Incomplete(CodeContext context, params object[] args)
  {
    return PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState(PythonBinaryAscii._IncompleteKey), args);
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException(PythonBinaryAscii._ErrorKey, dict, "Error", "binascii");
    context.EnsureModuleException(PythonBinaryAscii._IncompleteKey, dict, "Incomplete", "binascii");
  }

  private static int UuDecFunc(char val)
  {
    if (val > ' ' && val < '`')
      return (int) val - 32 /*0x20*/;
    if (val <= '\r')
    {
      if (val != '\n' && val != '\r')
        goto label_6;
    }
    else if (val != ' ' && val != '`')
      goto label_6;
    return -2;
label_6:
    return -4;
  }

  public static string a2b_uu(CodeContext context, string data)
  {
    if (data == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (data.Length < 1)
      return new string(char.MinValue, 32 /*0x20*/);
    int num = ((int) data[0] + 32 /*0x20*/) % 64 /*0x40*/;
    int length = (num * 4 + 2) / 3;
    string data1 = (string) null;
    if (data.Length - 1 > length)
    {
      data1 = data.Substring(1 + length);
      data = data.Substring(1, length);
    }
    else
      data = data.Substring(1);
    StringBuilder stringBuilder = PythonBinaryAscii.DecodeWorker(context, data, true, new PythonBinaryAscii.DecodeByte(PythonBinaryAscii.UuDecFunc));
    if (data1 == null)
      stringBuilder.Append(char.MinValue, num - stringBuilder.Length);
    else
      PythonBinaryAscii.ProcessSuffix(context, data1, new PythonBinaryAscii.DecodeByte(PythonBinaryAscii.UuDecFunc));
    return stringBuilder.ToString();
  }

  public static string b2a_uu(CodeContext context, string data)
  {
    if (data == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    StringBuilder stringBuilder = data.Length <= 45 ? PythonBinaryAscii.EncodeWorker(data, ' ', (PythonBinaryAscii.EncodeChar) (val => (char) (32 /*0x20*/ + val % 64 /*0x40*/))) : throw PythonBinaryAscii.Error(context, (object) "At most 45 bytes at once");
    stringBuilder.Insert(0, ((char) (32 /*0x20*/ + data.Length)).ToString());
    stringBuilder.Append('\n');
    return stringBuilder.ToString();
  }

  private static int Base64DecFunc(char val)
  {
    if (val >= 'A' && val <= 'Z')
      return (int) val - 65;
    if (val >= 'a' && val <= 'z')
      return (int) val - 97 + 26;
    if (val >= '0' && val <= '9')
      return (int) val - 48 /*0x30*/ + 52;
    switch (val)
    {
      case '+':
        return 62;
      case '/':
        return 63 /*0x3F*/;
      case '=':
        return -3;
      default:
        return -1;
    }
  }

  public static object a2b_base64(CodeContext context, [BytesConversion] string data)
  {
    data = data != null ? PythonBinaryAscii.RemovePrefix(context, data, new PythonBinaryAscii.DecodeByte(PythonBinaryAscii.Base64DecFunc)) : throw PythonOps.TypeError("expected string, got NoneType");
    return data.Length == 0 ? (object) string.Empty : (object) PythonBinaryAscii.DecodeWorker(context, data, false, new PythonBinaryAscii.DecodeByte(PythonBinaryAscii.Base64DecFunc)).ToString();
  }

  public static object b2a_base64([BytesConversion] string data)
  {
    switch (data)
    {
      case null:
        throw PythonOps.TypeError("expected string, got NoneType");
      case "":
        return (object) string.Empty;
      default:
        StringBuilder stringBuilder = PythonBinaryAscii.EncodeWorker(data, '=', new PythonBinaryAscii.EncodeChar(PythonBinaryAscii.EncodeValue));
        stringBuilder.Append('\n');
        return (object) stringBuilder.ToString();
    }
  }

  private static char EncodeValue(int val)
  {
    if (val < 26)
      return (char) (65 + val);
    if (val < 52)
      return (char) (97 + val - 26);
    if (val < 62)
      return (char) (48 /*0x30*/ + val - 52);
    if (val == 62)
      return '+';
    if (val == 63 /*0x3F*/)
      return '/';
    throw new InvalidOperationException($"Bad int val: {val}");
  }

  public static object a2b_qp(object data) => throw new NotImplementedException();

  [LightThrowing]
  public static object a2b_qp(object data, object header)
  {
    return LightExceptions.Throw((Exception) new NotImplementedException());
  }

  private static void to_hex(char ch, StringBuilder s, int index)
  {
    int num1 = (int) ch;
    int num2 = (int) ch / 16 /*0x10*/;
    s.Append("0123456789ABCDEF"[num2 % 16 /*0x10*/]);
    s.Append("0123456789ABCDEF"[num1 % 16 /*0x10*/]);
  }

  [Documentation("b2a_qp(data, quotetabs=0, istext=1, header=0) -> s; \r\n Encode a string using quoted-printable encoding. \r\n\r\nOn encoding, when istext is set, newlines are not encoded, and white \r\nspace at end of lines is.  When istext is not set, \\\\r and \\\\n (CR/LF) are \r\nboth encoded.  When quotetabs is set, space and tabs are encoded.")]
  public static object b2a_qp(string data, int quotetabs = 0, int istext = 1, int header = 0)
  {
    bool flag1 = data.Contains("\r\n");
    int num1 = 0;
    int num2 = 0;
    int index1 = 0;
    bool flag2 = quotetabs != 0;
    bool flag3 = header != 0;
    bool flag4 = istext != 0;
    while (index1 < data.Length)
    {
      if (data[index1] > '~' || data[index1] == '=' || flag3 && data[index1] == '_' || data[index1] == '.' && num1 == 0 && (data[index1 + 1] == '\n' || data[index1 + 1] == '\r' || data[index1 + 1] == char.MinValue) || !flag4 && (data[index1] == '\r' || data[index1] == '\n') || (data[index1] == '\t' || data[index1] == ' ') && index1 + 1 == data.Length || data[index1] < '!' && data[index1] != '\r' && data[index1] != '\n' && (flag2 || !flag2 && data[index1] != '\t' && data[index1] != ' '))
      {
        if (num1 + 3 >= 76)
        {
          num1 = 0;
          if (flag1)
            num2 += 3;
          else
            num2 += 2;
        }
        num1 += 3;
        num2 += 3;
        ++index1;
      }
      else if (flag4 && (data[index1] == '\n' || index1 + 1 < data.Length && data[index1] == '\r' && data[index1 + 1] == '\n'))
      {
        num1 = 0;
        if (index1 > 0 && (data[index1 - 1] == ' ' || data[index1 - 1] == '\t'))
          num2 += 2;
        if (flag1)
          num2 += 2;
        else
          ++num2;
        if (data[index1] == '\r')
          index1 += 2;
        else
          ++index1;
      }
      else
      {
        if (index1 + 1 != data.Length && data[index1 + 1] != '\n' && num1 + 1 >= 76)
        {
          num1 = 0;
          if (flag1)
            num2 += 3;
          else
            num2 += 2;
        }
        ++num1;
        ++num2;
        ++index1;
      }
    }
    StringBuilder s1 = new StringBuilder();
    int num3;
    int num4 = num3 = 0;
    int index2 = num3;
    int index3 = num3;
    while (index3 < data.Length)
    {
      if (data[index3] > '~' || data[index3] == '=' || flag3 && data[index3] == '_' || data[index3] == '.' && num4 == 0 && (data[index3 + 1] == '\n' || data[index3 + 1] == '\r' || data[index3 + 1] == char.MinValue) || !flag4 && (data[index3] == '\r' || data[index3] == '\n') || (data[index3] == '\t' || data[index3] == ' ') && index3 + 1 == data.Length || data[index3] < '!' && data[index3] != '\r' && data[index3] != '\n' && (flag2 || !flag2 && data[index3] != '\t' && data[index3] != ' '))
      {
        if (num4 + 3 >= 76)
        {
          s1.Append('=');
          if (flag1)
            s1.Append('\r');
          s1.Append('\n');
          num4 = 0;
        }
        s1.Append('=');
        PythonBinaryAscii.to_hex(data[index3], s1, index2);
        index2 += 2;
        ++index3;
        num4 += 3;
      }
      else if (flag4 && (data[index3] == '\n' || index3 + 1 < data.Length && data[index3] == '\r' && data[index3 + 1] == '\n'))
      {
        num4 = 0;
        if (index2 != 0 && (s1[index2 - 1] == ' ' || s1[index2 - 1] == '\t'))
        {
          int num5 = (int) s1[index2 - 1];
          s1[index2 - 1] = '=';
          StringBuilder s2 = s1;
          int index4 = index2;
          PythonBinaryAscii.to_hex((char) num5, s2, index4);
          index2 += 2;
        }
        if (flag1)
          s1[index2++] = '\r';
        s1[index2++] = '\n';
        if (data[index3] == '\r')
          index3 += 2;
        else
          ++index3;
      }
      else
      {
        if (index3 + 1 != data.Length && data[index3 + 1] != '\n' && num4 + 1 >= 76)
        {
          StringBuilder stringBuilder1 = s1;
          int index5 = index2;
          int num6 = index5 + 1;
          stringBuilder1[index5] = '=';
          if (flag1)
            s1[num6++] = '\r';
          StringBuilder stringBuilder2 = s1;
          int index6 = num6;
          index2 = index6 + 1;
          stringBuilder2[index6] = '\n';
          num4 = 0;
        }
        ++num4;
        if (flag3 && data[index3] == ' ')
        {
          s1[index2++] = '_';
          ++index3;
        }
        else
          s1[index2++] = data[index3++];
      }
    }
    return (object) s1.ToString();
  }

  public static object a2b_hqx(object data) => throw new NotImplementedException();

  public static object rledecode_hqx(object data) => throw new NotImplementedException();

  public static object rlecode_hqx(object data) => throw new NotImplementedException();

  public static object b2a_hqx(object data) => throw new NotImplementedException();

  public static object crc_hqx(object data, object crc) => throw new NotImplementedException();

  [Documentation("crc32(string[, value]) -> string\n\nComputes a CRC (Cyclic Redundancy Check) checksum of string.")]
  public static int crc32(string buffer, int baseValue = 0)
  {
    byte[] buffer1 = buffer.MakeByteArray();
    return (int) PythonBinaryAscii.crc32(buffer1, 0, buffer1.Length, (uint) baseValue);
  }

  [Documentation("crc32(string[, value]) -> string\n\nComputes a CRC (Cyclic Redundancy Check) checksum of string.")]
  public static int crc32(string buffer, uint baseValue)
  {
    byte[] buffer1 = buffer.MakeByteArray();
    return (int) PythonBinaryAscii.crc32(buffer1, 0, buffer1.Length, baseValue);
  }

  [Documentation("crc32(byte_array[, value]) -> string\n\nComputes a CRC (Cyclic Redundancy Check) checksum of byte_array.")]
  public static int crc32(byte[] buffer, int baseValue = 0)
  {
    return (int) PythonBinaryAscii.crc32(buffer, 0, buffer.Length, (uint) baseValue);
  }

  [Documentation("crc32(byte_array[, value]) -> string\n\nComputes a CRC (Cyclic Redundancy Check) checksum of byte_array.")]
  public static int crc32(byte[] buffer, uint baseValue)
  {
    return (int) PythonBinaryAscii.crc32(buffer, 0, buffer.Length, baseValue);
  }

  internal static uint crc32(byte[] buffer, int offset, int count, uint baseValue)
  {
    uint num = baseValue ^ uint.MaxValue;
    for (int index1 = offset; index1 < offset + count; ++index1)
    {
      num ^= (uint) buffer[index1];
      for (int index2 = 0; index2 < 8; ++index2)
      {
        if (((int) num & 1) != 0)
          num = num >> 1 ^ 3988292384U;
        else
          num >>= 1;
      }
    }
    return num ^ uint.MaxValue;
  }

  public static string b2a_hex(string data)
  {
    StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
    for (int index = 0; index < data.Length; ++index)
      stringBuilder.AppendFormat("{0:x2}", (object) (int) data[index]);
    return stringBuilder.ToString();
  }

  public static string hexlify(string data) => PythonBinaryAscii.b2a_hex(data);

  public static Bytes hexlify(MemoryView data) => PythonBinaryAscii.hexlify(data.tobytes());

  public static Bytes hexlify(Bytes data)
  {
    byte[] bytes = new byte[data.Count * 2];
    for (int index = 0; index < data.Count; ++index)
    {
      bytes[index * 2] = PythonBinaryAscii.ToHex((int) data._bytes[index] >> 4);
      bytes[index * 2 + 1] = PythonBinaryAscii.ToHex((int) data._bytes[index] & 15);
    }
    return Bytes.Make(bytes);
  }

  public static Bytes hexlify(ByteArray data)
  {
    byte[] bytes = new byte[data.Count * 2];
    for (int index = 0; index < data.Count; ++index)
    {
      bytes[index * 2] = PythonBinaryAscii.ToHex((int) data._bytes[index] >> 4);
      bytes[index * 2 + 1] = PythonBinaryAscii.ToHex((int) data._bytes[index] & 15);
    }
    return Bytes.Make(bytes);
  }

  private static byte ToHex(int p) => p >= 10 ? (byte) (97 + p - 10) : (byte) (48 /*0x30*/ + p);

  public static string hexlify([NotNull] PythonBuffer data)
  {
    return PythonBinaryAscii.hexlify(data.ToString());
  }

  public static object a2b_hex(CodeContext context, [BytesConversion] string data)
  {
    if (data == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    StringBuilder stringBuilder = (data.Length & 1) == 0 ? new StringBuilder(data.Length / 2) : throw PythonOps.TypeError("Odd-length string");
    for (int index = 0; index < data.Length; index += 2)
    {
      byte num1 = !char.IsDigit(data[index]) ? (byte) ((int) char.ToUpper(data[index]) - 65 + 10) : (byte) ((uint) data[index] - 48U /*0x30*/);
      byte num2 = !char.IsDigit(data[index + 1]) ? (byte) ((int) char.ToUpper(data[index + 1]) - 65 + 10) : (byte) ((uint) data[index + 1] - 48U /*0x30*/);
      stringBuilder.Append((char) ((uint) num1 * 16U /*0x10*/ + (uint) num2));
    }
    return (object) stringBuilder.ToString();
  }

  public static object unhexlify(CodeContext context, [BytesConversion] string hexstr)
  {
    return PythonBinaryAscii.a2b_hex(context, hexstr);
  }

  private static StringBuilder EncodeWorker(
    string data,
    char empty,
    PythonBinaryAscii.EncodeChar encFunc)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < data.Length; index += 3)
    {
      switch (data.Length - index)
      {
        case 1:
          int num1 = ((int) data[index] & (int) byte.MaxValue) << 16 /*0x10*/;
          stringBuilder.Append(encFunc(num1 >> 18 & 63 /*0x3F*/));
          stringBuilder.Append(encFunc(num1 >> 12 & 63 /*0x3F*/));
          stringBuilder.Append(empty);
          stringBuilder.Append(empty);
          break;
        case 2:
          int num2 = ((int) data[index] & (int) byte.MaxValue) << 16 /*0x10*/ | ((int) data[index + 1] & (int) byte.MaxValue) << 8;
          stringBuilder.Append(encFunc(num2 >> 18 & 63 /*0x3F*/));
          stringBuilder.Append(encFunc(num2 >> 12 & 63 /*0x3F*/));
          stringBuilder.Append(encFunc(num2 >> 6 & 63 /*0x3F*/));
          stringBuilder.Append(empty);
          break;
        default:
          int num3 = ((int) data[index] & (int) byte.MaxValue) << 16 /*0x10*/ | ((int) data[index + 1] & (int) byte.MaxValue) << 8 | (int) data[index + 2] & (int) byte.MaxValue;
          stringBuilder.Append(encFunc(num3 >> 18 & 63 /*0x3F*/));
          stringBuilder.Append(encFunc(num3 >> 12 & 63 /*0x3F*/));
          stringBuilder.Append(encFunc(num3 >> 6 & 63 /*0x3F*/));
          stringBuilder.Append(encFunc(num3 & 63 /*0x3F*/));
          break;
      }
    }
    return stringBuilder;
  }

  private static int NextVal(
    CodeContext context,
    string data,
    ref int index,
    PythonBinaryAscii.DecodeByte decFunc)
  {
    while (index < data.Length)
    {
      int num = decFunc(data[index++]);
      switch (num)
      {
        case -4:
          throw PythonBinaryAscii.Error(context, (object) "Illegal char");
        case -2:
          return 0;
        case -1:
          continue;
        default:
          return num;
      }
    }
    return -5;
  }

  private static int CountPadBytes(
    CodeContext context,
    string data,
    int bound,
    ref int index,
    PythonBinaryAscii.DecodeByte decFunc)
  {
    int num1 = -3;
    int num2 = 0;
    while ((bound < 0 || num2 < bound) && (num1 = PythonBinaryAscii.NextVal(context, data, ref index, decFunc)) == -3)
      ++num2;
    if (num1 != -3 && num1 != -5)
      --index;
    return num2;
  }

  private static int GetVal(
    CodeContext context,
    string data,
    int align,
    bool bounded,
    ref int index,
    PythonBinaryAscii.DecodeByte decFunc)
  {
    int val;
    do
    {
      val = PythonBinaryAscii.NextVal(context, data, ref index, decFunc);
      switch (val)
      {
        case -5:
          goto label_6;
        case -3:
          switch (align)
          {
            case 0:
            case 1:
              PythonBinaryAscii.CountPadBytes(context, data, -1, ref index, decFunc);
              continue;
            case 2:
              continue;
            default:
              goto label_5;
          }
        case -2:
          goto label_9;
        default:
          goto label_10;
      }
    }
    while (PythonBinaryAscii.CountPadBytes(context, data, 1, ref index, decFunc) <= 0);
    return -5;
label_5:
    return -5;
label_6:
    if (bounded || align == 0)
      return -5;
    throw PythonBinaryAscii.Error(context, (object) "Incorrect padding");
label_9:
    return 0;
label_10:
    return val;
  }

  private static StringBuilder DecodeWorker(
    CodeContext context,
    string data,
    bool bounded,
    PythonBinaryAscii.DecodeByte decFunc)
  {
    StringBuilder stringBuilder = new StringBuilder();
    int index = 0;
    while (index < data.Length)
    {
      int val1 = PythonBinaryAscii.GetVal(context, data, 0, bounded, ref index, decFunc);
      if (val1 >= 0)
      {
        int val2 = PythonBinaryAscii.GetVal(context, data, 1, bounded, ref index, decFunc);
        if (val2 >= 0)
        {
          int val3 = PythonBinaryAscii.GetVal(context, data, 2, bounded, ref index, decFunc);
          if (val3 < 0)
          {
            int num = val1 << 18 | val2 << 12;
            stringBuilder.Append((char) (num >> 16 /*0x10*/ & (int) byte.MaxValue));
            break;
          }
          int val4 = PythonBinaryAscii.GetVal(context, data, 3, bounded, ref index, decFunc);
          if (val4 < 0)
          {
            int num = val1 << 18 | val2 << 12 | val3 << 6;
            stringBuilder.Append((char) (num >> 16 /*0x10*/ & (int) byte.MaxValue));
            stringBuilder.Append((char) (num >> 8 & (int) byte.MaxValue));
            break;
          }
          int num1 = val1 << 18 | val2 << 12 | val3 << 6 | val4;
          stringBuilder.Append((char) (num1 >> 16 /*0x10*/ & (int) byte.MaxValue));
          stringBuilder.Append((char) (num1 >> 8 & (int) byte.MaxValue));
          stringBuilder.Append((char) (num1 & (int) byte.MaxValue));
        }
        else
          break;
      }
      else
        break;
    }
    return stringBuilder;
  }

  private static string RemovePrefix(
    CodeContext context,
    string data,
    PythonBinaryAscii.DecodeByte decFunc)
  {
    int num1;
    for (num1 = 0; num1 < data.Length; ++num1)
    {
      int num2 = decFunc(data[num1]);
      if (num2 == -4)
        throw PythonBinaryAscii.Error(context, (object) "Illegal char");
      if (num2 >= 0)
        break;
    }
    return num1 != 0 ? data.Substring(num1) : data;
  }

  private static void ProcessSuffix(
    CodeContext context,
    string data,
    PythonBinaryAscii.DecodeByte decFunc)
  {
    for (int index = 0; index < data.Length; ++index)
    {
      int num = decFunc(data[index]);
      if (num >= 0 || num == -4)
        throw PythonBinaryAscii.Error(context, (object) "Trailing garbage");
    }
  }

  private delegate char EncodeChar(int val);

  private delegate int DecodeByte(char val);
}
