// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonStruct
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class PythonStruct
{
  public const string __doc__ = null;
  public const string __version__ = "0.2";
  public const int _PY_STRUCT_FLOAT_COERCE = 0;
  public const int _PY_STRUCT_OVERFLOW_MASKING = 0;
  public const int _PY_STRUCT_RANGE_CHECKING = 0;
  private const int MAX_CACHE_SIZE = 1024 /*0x0400*/;
  private static CacheDict<string, PythonStruct.Struct> _cache = new CacheDict<string, PythonStruct.Struct>(1024 /*0x0400*/);

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException((object) "structerror", dict, "error", "struct");
  }

  private static int GetNativeSize(PythonStruct.FormatType c)
  {
    switch (c)
    {
      case PythonStruct.FormatType.PadByte:
      case PythonStruct.FormatType.Bool:
      case PythonStruct.FormatType.Char:
      case PythonStruct.FormatType.SignedChar:
      case PythonStruct.FormatType.UnsignedChar:
      case PythonStruct.FormatType.CString:
      case PythonStruct.FormatType.PascalString:
        return 1;
      case PythonStruct.FormatType.Short:
      case PythonStruct.FormatType.UnsignedShort:
        return 2;
      case PythonStruct.FormatType.Int:
      case PythonStruct.FormatType.UnsignedInt:
      case PythonStruct.FormatType.UnsignedLong:
      case PythonStruct.FormatType.Float:
        return 4;
      case PythonStruct.FormatType.LongLong:
      case PythonStruct.FormatType.UnsignedLongLong:
      case PythonStruct.FormatType.Double:
        return 8;
      case PythonStruct.FormatType.Pointer:
        return IntPtr.Size;
      default:
        throw new InvalidOperationException(c.ToString());
    }
  }

  private static PythonStruct.Struct GetStructFromCache(CodeContext context, [NotNull] string fmt)
  {
    PythonStruct.Struct structFromCache;
    bool flag;
    lock (PythonStruct._cache)
      flag = PythonStruct._cache.TryGetValue(fmt, out structFromCache);
    if (!flag)
      structFromCache = new PythonStruct.Struct(context, fmt);
    return structFromCache;
  }

  [Documentation("Clear the internal cache.")]
  public static void _clearcache()
  {
    PythonStruct._cache = new CacheDict<string, PythonStruct.Struct>(1024 /*0x0400*/);
  }

  [Documentation("int(x[, base]) -> integer\n\nConvert a string or number to an integer, if possible.  A floating point\nargument will be truncated towards zero (this does not include a string\nrepresentation of a floating point number!)  When converting a string, use\nthe optional base.  It is an error to supply a base when converting a\nnon-string.  If base is zero, the proper base is guessed based on the\nstring content.  If the argument is outside the integer range a\nlong object will be returned instead.")]
  public static int calcsize(CodeContext context, [NotNull] string fmt)
  {
    return PythonStruct.GetStructFromCache(context, fmt).size;
  }

  [Documentation("Return string containing values v1, v2, ... packed according to fmt.")]
  public static string pack(CodeContext context, [BytesConversion, NotNull] string fmt, params object[] values)
  {
    return PythonStruct.GetStructFromCache(context, fmt).pack(context, values);
  }

  [Documentation("Pack the values v1, v2, ... according to fmt.\nWrite the packed bytes into the writable buffer buf starting at offset.")]
  public static void pack_into(
    CodeContext context,
    [BytesConversion, NotNull] string fmt,
    [NotNull] ArrayModule.array buffer,
    int offset,
    params object[] args)
  {
    PythonStruct.GetStructFromCache(context, fmt).pack_into(context, buffer, offset, args);
  }

  public static void pack_into(
    CodeContext context,
    [BytesConversion, NotNull] string fmt,
    [NotNull] ByteArray buffer,
    int offset,
    params object[] args)
  {
    PythonStruct.GetStructFromCache(context, fmt).pack_into(context, buffer, offset, args);
  }

  [Documentation("Unpack the string containing packed C structure data, according to fmt.\nRequires len(string) == calcsize(fmt).")]
  public static PythonTuple unpack(CodeContext context, [BytesConversion, NotNull] string fmt, [NotNull] string @string)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack(context, @string);
  }

  [Documentation("Unpack the string containing packed C structure data, according to fmt.\nRequires len(string) == calcsize(fmt).")]
  public static PythonTuple unpack(CodeContext context, [BytesConversion, NotNull] string fmt, [BytesConversion, NotNull] IList<byte> @string)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack(context, @string);
  }

  [Documentation("Unpack the string containing packed C structure data, according to fmt.\nRequires len(string) == calcsize(fmt).")]
  public static PythonTuple unpack(CodeContext context, [BytesConversion, NotNull] string fmt, [NotNull] ArrayModule.array buffer)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack(context, buffer);
  }

  [Documentation("Unpack the string containing packed C structure data, according to fmt.\nRequires len(string) == calcsize(fmt).")]
  public static PythonTuple unpack(CodeContext context, [BytesConversion, NotNull] string fmt, [NotNull] PythonBuffer buffer)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack(context, buffer);
  }

  [Documentation("Unpack the buffer, containing packed C structure data, according to\nfmt, starting at offset. Requires len(buffer[offset:]) >= calcsize(fmt).")]
  public static PythonTuple unpack_from(
    CodeContext context,
    [BytesConversion, NotNull] string fmt,
    [NotNull] string buffer,
    int offset = 0)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack_from(context, buffer, offset);
  }

  [Documentation("Unpack the buffer, containing packed C structure data, according to\nfmt, starting at offset. Requires len(buffer[offset:]) >= calcsize(fmt).")]
  public static PythonTuple unpack_from(
    CodeContext context,
    [BytesConversion, NotNull] string fmt,
    [BytesConversion, NotNull] IList<byte> buffer,
    int offset = 0)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack_from(context, buffer, offset);
  }

  [Documentation("Unpack the buffer, containing packed C structure data, according to\nfmt, starting at offset. Requires len(buffer[offset:]) >= calcsize(fmt).")]
  public static PythonTuple unpack_from(
    CodeContext context,
    [BytesConversion, NotNull] string fmt,
    [NotNull] ArrayModule.array buffer,
    int offset = 0)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack_from(context, buffer, offset);
  }

  [Documentation("Unpack the buffer, containing packed C structure data, according to\nfmt, starting at offset. Requires len(buffer[offset:]) >= calcsize(fmt).")]
  public static PythonTuple unpack_from(
    CodeContext context,
    [BytesConversion, NotNull] string fmt,
    [NotNull] PythonBuffer buffer,
    int offset = 0)
  {
    return PythonStruct.GetStructFromCache(context, fmt).unpack_from(context, buffer, offset);
  }

  private static void WriteShort(StringBuilder res, bool fLittleEndian, short val)
  {
    if (fLittleEndian)
    {
      res.Append((char) ((uint) val & (uint) byte.MaxValue));
      res.Append((char) ((int) val >> 8 & (int) byte.MaxValue));
    }
    else
    {
      res.Append((char) ((int) val >> 8 & (int) byte.MaxValue));
      res.Append((char) ((uint) val & (uint) byte.MaxValue));
    }
  }

  private static void WriteUShort(StringBuilder res, bool fLittleEndian, ushort val)
  {
    if (fLittleEndian)
    {
      res.Append((char) ((uint) val & (uint) byte.MaxValue));
      res.Append((char) ((int) val >> 8 & (int) byte.MaxValue));
    }
    else
    {
      res.Append((char) ((int) val >> 8 & (int) byte.MaxValue));
      res.Append((char) ((uint) val & (uint) byte.MaxValue));
    }
  }

  private static void WriteInt(StringBuilder res, bool fLittleEndian, int val)
  {
    if (fLittleEndian)
    {
      res.Append((char) (val & (int) byte.MaxValue));
      res.Append((char) (val >> 8 & (int) byte.MaxValue));
      res.Append((char) (val >> 16 /*0x10*/ & (int) byte.MaxValue));
      res.Append((char) (val >> 24 & (int) byte.MaxValue));
    }
    else
    {
      res.Append((char) (val >> 24 & (int) byte.MaxValue));
      res.Append((char) (val >> 16 /*0x10*/ & (int) byte.MaxValue));
      res.Append((char) (val >> 8 & (int) byte.MaxValue));
      res.Append((char) (val & (int) byte.MaxValue));
    }
  }

  private static void WriteUInt(StringBuilder res, bool fLittleEndian, uint val)
  {
    if (fLittleEndian)
    {
      res.Append((char) (val & (uint) byte.MaxValue));
      res.Append((char) (val >> 8 & (uint) byte.MaxValue));
      res.Append((char) (val >> 16 /*0x10*/ & (uint) byte.MaxValue));
      res.Append((char) (val >> 24 & (uint) byte.MaxValue));
    }
    else
    {
      res.Append((char) (val >> 24 & (uint) byte.MaxValue));
      res.Append((char) (val >> 16 /*0x10*/ & (uint) byte.MaxValue));
      res.Append((char) (val >> 8 & (uint) byte.MaxValue));
      res.Append((char) (val & (uint) byte.MaxValue));
    }
  }

  private static void WritePointer(StringBuilder res, bool fLittleEndian, IntPtr val)
  {
    if (IntPtr.Size == 4)
      PythonStruct.WriteInt(res, fLittleEndian, val.ToInt32());
    else
      PythonStruct.WriteLong(res, fLittleEndian, val.ToInt64());
  }

  private static void WriteFloat(StringBuilder res, bool fLittleEndian, float val)
  {
    byte[] bytes = BitConverter.GetBytes(val);
    if (fLittleEndian)
    {
      res.Append((char) bytes[0]);
      res.Append((char) bytes[1]);
      res.Append((char) bytes[2]);
      res.Append((char) bytes[3]);
    }
    else
    {
      res.Append((char) bytes[3]);
      res.Append((char) bytes[2]);
      res.Append((char) bytes[1]);
      res.Append((char) bytes[0]);
    }
  }

  private static void WriteLong(StringBuilder res, bool fLittleEndian, long val)
  {
    if (fLittleEndian)
    {
      res.Append((char) ((ulong) val & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 8) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 16 /*0x10*/) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 24) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 32 /*0x20*/) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 40) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 48 /*0x30*/) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 56) & (ulong) byte.MaxValue));
    }
    else
    {
      res.Append((char) ((ulong) (val >> 56) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 48 /*0x30*/) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 40) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 32 /*0x20*/) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 24) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 16 /*0x10*/) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) (val >> 8) & (ulong) byte.MaxValue));
      res.Append((char) ((ulong) val & (ulong) byte.MaxValue));
    }
  }

  private static void WriteULong(StringBuilder res, bool fLittleEndian, ulong val)
  {
    if (fLittleEndian)
    {
      res.Append((char) (val & (ulong) byte.MaxValue));
      res.Append((char) (val >> 8 & (ulong) byte.MaxValue));
      res.Append((char) (val >> 16 /*0x10*/ & (ulong) byte.MaxValue));
      res.Append((char) (val >> 24 & (ulong) byte.MaxValue));
      res.Append((char) (val >> 32 /*0x20*/ & (ulong) byte.MaxValue));
      res.Append((char) (val >> 40 & (ulong) byte.MaxValue));
      res.Append((char) (val >> 48 /*0x30*/ & (ulong) byte.MaxValue));
      res.Append((char) (val >> 56 & (ulong) byte.MaxValue));
    }
    else
    {
      res.Append((char) (val >> 56 & (ulong) byte.MaxValue));
      res.Append((char) (val >> 48 /*0x30*/ & (ulong) byte.MaxValue));
      res.Append((char) (val >> 40 & (ulong) byte.MaxValue));
      res.Append((char) (val >> 32 /*0x20*/ & (ulong) byte.MaxValue));
      res.Append((char) (val >> 24 & (ulong) byte.MaxValue));
      res.Append((char) (val >> 16 /*0x10*/ & (ulong) byte.MaxValue));
      res.Append((char) (val >> 8 & (ulong) byte.MaxValue));
      res.Append((char) (val & (ulong) byte.MaxValue));
    }
  }

  private static void WriteDouble(StringBuilder res, bool fLittleEndian, double val)
  {
    byte[] bytes = BitConverter.GetBytes(val);
    if (fLittleEndian)
    {
      res.Append((char) bytes[0]);
      res.Append((char) bytes[1]);
      res.Append((char) bytes[2]);
      res.Append((char) bytes[3]);
      res.Append((char) bytes[4]);
      res.Append((char) bytes[5]);
      res.Append((char) bytes[6]);
      res.Append((char) bytes[7]);
    }
    else
    {
      res.Append((char) bytes[7]);
      res.Append((char) bytes[6]);
      res.Append((char) bytes[5]);
      res.Append((char) bytes[4]);
      res.Append((char) bytes[3]);
      res.Append((char) bytes[2]);
      res.Append((char) bytes[1]);
      res.Append((char) bytes[0]);
    }
  }

  private static void WriteString(StringBuilder res, int len, string val)
  {
    for (int index = 0; index < val.Length && index < len; ++index)
      res.Append(val[index]);
    for (int length = val.Length; length < len; ++length)
      res.Append(char.MinValue);
  }

  private static void WritePascalString(StringBuilder res, int len, string val)
  {
    int num = Math.Min((int) byte.MaxValue, Math.Min(val.Length, len));
    res.Append((char) num);
    for (int index = 0; index < val.Length && index < len; ++index)
      res.Append(val[index]);
    for (int length = val.Length; length < len; ++length)
      res.Append(char.MinValue);
  }

  internal static bool GetBoolValue(CodeContext context, int index, object[] args)
  {
    object obj = PythonStruct.GetValue(context, index, args);
    object result;
    if (Converter.TryConvert(obj, typeof (bool), out result))
      return (bool) result;
    throw PythonStruct.Error(context, "expected bool value got " + obj.ToString());
  }

  internal static char GetCharValue(CodeContext context, int index, object[] args)
  {
    switch (PythonStruct.GetValue(context, index, args))
    {
      case string str when str.Length == 1:
        return str[0];
      case IList<byte> byteList when byteList.Count == 1:
        return (char) byteList[0];
      default:
        throw PythonStruct.Error(context, "char format requires string of length 1");
    }
  }

  internal static sbyte GetSByteValue(CodeContext context, int index, object[] args)
  {
    object obj = PythonStruct.GetValue(context, index, args);
    sbyte result;
    if (Converter.TryConvertToSByte(obj, out result))
      return result;
    throw PythonStruct.Error(context, "expected sbyte value got " + obj.ToString());
  }

  internal static byte GetByteValue(CodeContext context, int index, object[] args)
  {
    object obj = PythonStruct.GetValue(context, index, args);
    byte result1;
    if (Converter.TryConvertToByte(obj, out result1))
      return result1;
    char result2;
    if (Converter.TryConvertToChar(obj, out result2))
      return (byte) result2;
    throw PythonStruct.Error(context, "expected byte value got " + obj.ToString());
  }

  internal static short GetShortValue(CodeContext context, int index, object[] args)
  {
    short result;
    if (Converter.TryConvertToInt16(PythonStruct.GetValue(context, index, args), out result))
      return result;
    throw PythonStruct.Error(context, "expected short value");
  }

  internal static ushort GetUShortValue(CodeContext context, int index, object[] args)
  {
    ushort result;
    if (Converter.TryConvertToUInt16(PythonStruct.GetValue(context, index, args), out result))
      return result;
    throw PythonStruct.Error(context, "expected ushort value");
  }

  internal static int GetIntValue(CodeContext context, int index, object[] args)
  {
    int result;
    if (Converter.TryConvertToInt32(PythonStruct.GetValue(context, index, args), out result))
      return result;
    throw PythonStruct.Error(context, "expected int value");
  }

  internal static uint GetULongValue(CodeContext context, int index, object[] args, string type)
  {
    object ulongValue = PythonStruct.GetValue(context, index, args);
    switch (ulongValue)
    {
      case int _:
        PythonStruct.CheckRange(context, (int) ulongValue, type);
        return (uint) (int) ulongValue;
      case BigInteger _:
        PythonStruct.CheckRange(context, (BigInteger) ulongValue, type);
        return (uint) (BigInteger) ulongValue;
      case Extensible<int> _:
        PythonStruct.CheckRange(context, ((Extensible<int>) ulongValue).Value, type);
        return (uint) ((Extensible<int>) ulongValue).Value;
      case Extensible<BigInteger> _:
        PythonStruct.CheckRange(context, ((Extensible<BigInteger>) ulongValue).Value, type);
        return (uint) ((Extensible<BigInteger>) ulongValue).Value;
      default:
        object val;
        if (PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, ulongValue, "__int__", out val) && val is int)
        {
          PythonStruct.CheckRange(context, (int) val, type);
          return (uint) (int) val;
        }
        uint result;
        if (Converter.TryConvertToUInt32(ulongValue, out result))
          return result;
        throw PythonStruct.Error(context, "cannot convert argument to integer");
    }
  }

  private static void CheckRange(CodeContext context, int val, string type)
  {
    if (val >= 0)
      return;
    PythonStruct.OutOfRange(context, type);
  }

  private static void CheckRange(CodeContext context, BigInteger bi, string type)
  {
    if (!(bi < 0L) && !(bi > (long) uint.MaxValue))
      return;
    PythonStruct.OutOfRange(context, type);
  }

  private static void OutOfRange(CodeContext context, string type)
  {
    throw PythonStruct.Error(context, $"integer out of range for '{(type == "unsigned long" ? (object) "L" : (object) "I")}' format code");
  }

  internal static IntPtr GetPointer(CodeContext context, int index, object[] args)
  {
    object obj = PythonStruct.GetValue(context, index, args);
    if (IntPtr.Size == 4)
    {
      uint result;
      if (Converter.TryConvertToUInt32(obj, out result))
        return new IntPtr((long) result);
    }
    else
    {
      long result;
      if (Converter.TryConvertToInt64(obj, out result))
        return new IntPtr(result);
    }
    throw PythonStruct.Error(context, "expected pointer value");
  }

  internal static long GetLongValue(CodeContext context, int index, object[] args)
  {
    long result;
    if (Converter.TryConvertToInt64(PythonStruct.GetValue(context, index, args), out result))
      return result;
    throw PythonStruct.Error(context, "expected long value");
  }

  internal static ulong GetULongLongValue(CodeContext context, int index, object[] args)
  {
    ulong result;
    if (Converter.TryConvertToUInt64(PythonStruct.GetValue(context, index, args), out result))
      return result;
    throw PythonStruct.Error(context, "expected ulong value");
  }

  internal static double GetDoubleValue(CodeContext context, int index, object[] args)
  {
    double result;
    if (Converter.TryConvertToDouble(PythonStruct.GetValue(context, index, args), out result))
      return result;
    throw PythonStruct.Error(context, "expected double value");
  }

  internal static string GetStringValue(CodeContext context, int index, object[] args)
  {
    string result;
    if (Converter.TryConvertToString(PythonStruct.GetValue(context, index, args), out result))
      return result;
    throw PythonStruct.Error(context, "expected string value");
  }

  internal static object GetValue(CodeContext context, int index, object[] args)
  {
    if (index >= args.Length)
      throw PythonStruct.Error(context, "not enough arguments");
    return args[index];
  }

  internal static bool CreateBoolValue(CodeContext context, ref int index, string data)
  {
    return PythonStruct.ReadData(context, ref index, data) > char.MinValue;
  }

  internal static char CreateCharValue(CodeContext context, ref int index, string data)
  {
    return PythonStruct.ReadData(context, ref index, data);
  }

  internal static short CreateShortValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    byte num1 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num2 = (byte) PythonStruct.ReadData(context, ref index, data);
    return fLittleEndian ? (short) ((int) num2 << 8 | (int) num1) : (short) ((int) num1 << 8 | (int) num2);
  }

  internal static ushort CreateUShortValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    byte num1 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num2 = (byte) PythonStruct.ReadData(context, ref index, data);
    return fLittleEndian ? (ushort) ((uint) num2 << 8 | (uint) num1) : (ushort) ((uint) num1 << 8 | (uint) num2);
  }

  internal static float CreateFloatValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    byte[] numArray = new byte[4];
    if (fLittleEndian)
    {
      numArray[0] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[1] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[2] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[3] = (byte) PythonStruct.ReadData(context, ref index, data);
    }
    else
    {
      numArray[3] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[2] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[1] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[0] = (byte) PythonStruct.ReadData(context, ref index, data);
    }
    float single = BitConverter.ToSingle(numArray, 0);
    if (context.LanguageContext.FloatFormat == FloatFormat.Unknown && (float.IsNaN(single) || float.IsInfinity(single)))
      throw PythonOps.ValueError("can't unpack IEEE 754 special value on non-IEEE platform");
    return single;
  }

  internal static int CreateIntValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    byte num1 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num2 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num3 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num4 = (byte) PythonStruct.ReadData(context, ref index, data);
    return fLittleEndian ? (int) num4 << 24 | (int) num3 << 16 /*0x10*/ | (int) num2 << 8 | (int) num1 : (int) num1 << 24 | (int) num2 << 16 /*0x10*/ | (int) num3 << 8 | (int) num4;
  }

  internal static uint CreateUIntValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    byte num1 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num2 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num3 = (byte) PythonStruct.ReadData(context, ref index, data);
    byte num4 = (byte) PythonStruct.ReadData(context, ref index, data);
    return fLittleEndian ? (uint) ((int) num4 << 24 | (int) num3 << 16 /*0x10*/ | (int) num2 << 8) | (uint) num1 : (uint) ((int) num1 << 24 | (int) num2 << 16 /*0x10*/ | (int) num3 << 8) | (uint) num4;
  }

  internal static long CreateLongValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    long num1 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    long num2 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    long num3 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    long num4 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    long num5 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    long num6 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    long num7 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    long num8 = (long) (byte) PythonStruct.ReadData(context, ref index, data);
    return fLittleEndian ? num8 << 56 | num7 << 48 /*0x30*/ | num6 << 40 | num5 << 32 /*0x20*/ | num4 << 24 | num3 << 16 /*0x10*/ | num2 << 8 | num1 : num1 << 56 | num2 << 48 /*0x30*/ | num3 << 40 | num4 << 32 /*0x20*/ | num5 << 24 | num6 << 16 /*0x10*/ | num7 << 8 | num8;
  }

  internal static ulong CreateULongValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    ulong num1 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    ulong num2 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    ulong num3 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    ulong num4 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    ulong num5 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    ulong num6 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    ulong num7 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    ulong num8 = (ulong) (byte) PythonStruct.ReadData(context, ref index, data);
    return fLittleEndian ? (ulong) ((long) num8 << 56 | (long) num7 << 48 /*0x30*/ | (long) num6 << 40 | (long) num5 << 32 /*0x20*/ | (long) num4 << 24 | (long) num3 << 16 /*0x10*/ | (long) num2 << 8) | num1 : (ulong) ((long) num1 << 56 | (long) num2 << 48 /*0x30*/ | (long) num3 << 40 | (long) num4 << 32 /*0x20*/ | (long) num5 << 24 | (long) num6 << 16 /*0x10*/ | (long) num7 << 8) | num8;
  }

  internal static double CreateDoubleValue(
    CodeContext context,
    ref int index,
    bool fLittleEndian,
    string data)
  {
    byte[] numArray = new byte[8];
    if (fLittleEndian)
    {
      numArray[0] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[1] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[2] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[3] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[4] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[5] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[6] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[7] = (byte) PythonStruct.ReadData(context, ref index, data);
    }
    else
    {
      numArray[7] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[6] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[5] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[4] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[3] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[2] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[1] = (byte) PythonStruct.ReadData(context, ref index, data);
      numArray[0] = (byte) PythonStruct.ReadData(context, ref index, data);
    }
    double d = BitConverter.ToDouble(numArray, 0);
    if (context.LanguageContext.DoubleFormat == FloatFormat.Unknown && (double.IsNaN(d) || double.IsInfinity(d)))
      throw PythonOps.ValueError("can't unpack IEEE 754 special value on non-IEEE platform");
    return d;
  }

  internal static string CreateString(CodeContext context, ref int index, int count, string data)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index1 = 0; index1 < count; ++index1)
      stringBuilder.Append(PythonStruct.ReadData(context, ref index, data));
    return stringBuilder.ToString();
  }

  internal static string CreatePascalString(
    CodeContext context,
    ref int index,
    int count,
    string data)
  {
    int num1 = (int) PythonStruct.ReadData(context, ref index, data);
    StringBuilder stringBuilder = new StringBuilder();
    for (int index1 = 0; index1 < num1; ++index1)
      stringBuilder.Append(PythonStruct.ReadData(context, ref index, data));
    for (int index2 = num1; index2 < count; ++index2)
    {
      int num2 = (int) PythonStruct.ReadData(context, ref index, data);
    }
    return stringBuilder.ToString();
  }

  private static char ReadData(CodeContext context, ref int index, string data)
  {
    if (index >= data.Length)
      throw PythonStruct.Error(context, "not enough data while reading");
    return data[index++];
  }

  internal static int Align(int length, int size) => length + (size - 1) & ~(size - 1);

  private static Exception Error(CodeContext context, string msg)
  {
    return PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState((object) "structerror"), (object) msg);
  }

  [PythonType]
  [Documentation("Represents a compiled struct pattern")]
  public class Struct : IWeakReferenceable
  {
    private string _formatString;
    private PythonStruct.Format[] _formats;
    private bool _isStandardized;
    private bool _isLittleEndian;
    private int _encodingCount = -1;
    private int _encodingSize = -1;
    private WeakRefTracker _tracker;

    private void Initialize(PythonStruct.Struct s)
    {
      this._formatString = s._formatString;
      this._formats = s._formats;
      this._isStandardized = s._isStandardized;
      this._isLittleEndian = s._isLittleEndian;
      this._encodingCount = s._encodingCount;
      this._encodingSize = s._encodingSize;
      this._tracker = s._tracker;
    }

    internal Struct(CodeContext context, [NotNull] string fmt) => this.__init__(context, fmt);

    [Documentation("creates a new uninitialized struct object - all arguments are ignored")]
    public Struct(params object[] args)
    {
    }

    [Documentation("creates a new uninitialized struct object - all arguments are ignored")]
    public Struct([ParamDictionary] IDictionary<object, object> kwArgs, params object[] args)
    {
    }

    [Documentation("initializes or re-initializes the compiled struct object with a new format")]
    public void __init__(CodeContext context, [NotNull] string fmt)
    {
      ContractUtils.RequiresNotNull((object) fmt, nameof (fmt));
      this._formatString = fmt;
      PythonStruct.Struct s;
      bool flag;
      lock (PythonStruct._cache)
        flag = PythonStruct._cache.TryGetValue(this._formatString, out s);
      if (flag)
        this.Initialize(s);
      else
        this.Compile(context, fmt);
    }

    [Documentation("gets the current format string for the compiled Struct")]
    public string format => this._formatString;

    [Documentation("returns a string consisting of the values serialized according to the format of the struct object")]
    public string pack(CodeContext context, params object[] values)
    {
      if (values.Length != this._encodingCount)
        throw PythonStruct.Error(context, $"pack requires exactly {this._encodingCount} arguments");
      int num1 = 0;
      StringBuilder res = new StringBuilder(this._encodingSize);
      for (int index1 = 0; index1 < this._formats.Length; ++index1)
      {
        PythonStruct.Format format = this._formats[index1];
        if (!this._isStandardized)
        {
          int nativeSize = format.NativeSize;
          int num2 = PythonStruct.Align(res.Length, nativeSize) - res.Length;
          for (int index2 = 0; index2 < num2; ++index2)
            res.Append(char.MinValue);
        }
        switch (format.Type)
        {
          case PythonStruct.FormatType.PadByte:
            res.Append(char.MinValue, format.Count);
            break;
          case PythonStruct.FormatType.Bool:
            res.Append(PythonStruct.GetBoolValue(context, num1++, values) ? '\u0001' : char.MinValue);
            break;
          case PythonStruct.FormatType.Char:
            for (int index3 = 0; index3 < format.Count; ++index3)
              res.Append(PythonStruct.GetCharValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.SignedChar:
            for (int index4 = 0; index4 < format.Count; ++index4)
              res.Append((char) (byte) PythonStruct.GetSByteValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.UnsignedChar:
            for (int index5 = 0; index5 < format.Count; ++index5)
              res.Append((char) PythonStruct.GetByteValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.Short:
            for (int index6 = 0; index6 < format.Count; ++index6)
              PythonStruct.WriteShort(res, this._isLittleEndian, PythonStruct.GetShortValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.UnsignedShort:
            for (int index7 = 0; index7 < format.Count; ++index7)
              PythonStruct.WriteUShort(res, this._isLittleEndian, PythonStruct.GetUShortValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.Int:
            for (int index8 = 0; index8 < format.Count; ++index8)
              PythonStruct.WriteInt(res, this._isLittleEndian, PythonStruct.GetIntValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.UnsignedInt:
            for (int index9 = 0; index9 < format.Count; ++index9)
              PythonStruct.WriteUInt(res, this._isLittleEndian, PythonStruct.GetULongValue(context, num1++, values, "unsigned int"));
            break;
          case PythonStruct.FormatType.UnsignedLong:
            for (int index10 = 0; index10 < format.Count; ++index10)
              PythonStruct.WriteUInt(res, this._isLittleEndian, PythonStruct.GetULongValue(context, num1++, values, "unsigned long"));
            break;
          case PythonStruct.FormatType.Float:
            for (int index11 = 0; index11 < format.Count; ++index11)
              PythonStruct.WriteFloat(res, this._isLittleEndian, (float) PythonStruct.GetDoubleValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.LongLong:
            for (int index12 = 0; index12 < format.Count; ++index12)
              PythonStruct.WriteLong(res, this._isLittleEndian, PythonStruct.GetLongValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.UnsignedLongLong:
            for (int index13 = 0; index13 < format.Count; ++index13)
              PythonStruct.WriteULong(res, this._isLittleEndian, PythonStruct.GetULongLongValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.Double:
            for (int index14 = 0; index14 < format.Count; ++index14)
              PythonStruct.WriteDouble(res, this._isLittleEndian, PythonStruct.GetDoubleValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.CString:
            PythonStruct.WriteString(res, format.Count, PythonStruct.GetStringValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.PascalString:
            PythonStruct.WritePascalString(res, format.Count - 1, PythonStruct.GetStringValue(context, num1++, values));
            break;
          case PythonStruct.FormatType.Pointer:
            for (int index15 = 0; index15 < format.Count; ++index15)
              PythonStruct.WritePointer(res, this._isLittleEndian, PythonStruct.GetPointer(context, num1++, values));
            break;
          default:
            throw PythonStruct.Error(context, "bad format string");
        }
      }
      return res.ToString();
    }

    [Documentation("Stores the deserialized data into the provided array")]
    public void pack_into(
      CodeContext context,
      [NotNull] ArrayModule.array buffer,
      int offset,
      params object[] args)
    {
      byte[] byteArray = buffer.ToByteArray();
      if (offset + this.size > byteArray.Length)
        throw PythonStruct.Error(context, $"pack_into requires a buffer of at least {this.size} bytes");
      string str = this.pack(context, args);
      for (int index = 0; index < str.Length; ++index)
        byteArray[index + offset] = (byte) str[index];
      buffer.Clear();
      buffer.FromStream((Stream) new MemoryStream(byteArray));
    }

    public void pack_into(CodeContext context, [NotNull] ByteArray buffer, int offset, params object[] args)
    {
      IList<byte> bytes = (IList<byte>) buffer._bytes;
      if (offset + this.size > bytes.Count)
        throw PythonStruct.Error(context, $"pack_into requires a buffer of at least {this.size} bytes");
      string str = this.pack(context, args);
      for (int index = 0; index < str.Length; ++index)
        bytes[index + offset] = (byte) str[index];
    }

    [Documentation("deserializes the string using the structs specified format")]
    public PythonTuple unpack(CodeContext context, [NotNull] string @string)
    {
      if (@string.Length != this.size)
        throw PythonStruct.Error(context, $"unpack requires a string argument of length {this.size}");
      string data = @string;
      int index1 = 0;
      object[] objArray = new object[this._encodingCount];
      int num = 0;
      for (int index2 = 0; index2 < this._formats.Length; ++index2)
      {
        PythonStruct.Format format = this._formats[index2];
        if (!this._isStandardized)
        {
          int nativeSize = format.NativeSize;
          if (nativeSize > 0)
            index1 = PythonStruct.Align(index1, nativeSize);
        }
        switch (format.Type)
        {
          case PythonStruct.FormatType.PadByte:
            index1 += format.Count;
            break;
          case PythonStruct.FormatType.Bool:
            for (int index3 = 0; index3 < format.Count; ++index3)
              objArray[num++] = (object) PythonStruct.CreateBoolValue(context, ref index1, data);
            break;
          case PythonStruct.FormatType.Char:
            for (int index4 = 0; index4 < format.Count; ++index4)
              objArray[num++] = (object) PythonStruct.CreateCharValue(context, ref index1, data).ToString();
            break;
          case PythonStruct.FormatType.SignedChar:
            for (int index5 = 0; index5 < format.Count; ++index5)
              objArray[num++] = (object) (int) (sbyte) PythonStruct.CreateCharValue(context, ref index1, data);
            break;
          case PythonStruct.FormatType.UnsignedChar:
            for (int index6 = 0; index6 < format.Count; ++index6)
              objArray[num++] = (object) (int) PythonStruct.CreateCharValue(context, ref index1, data);
            break;
          case PythonStruct.FormatType.Short:
            for (int index7 = 0; index7 < format.Count; ++index7)
              objArray[num++] = (object) (int) PythonStruct.CreateShortValue(context, ref index1, this._isLittleEndian, data);
            break;
          case PythonStruct.FormatType.UnsignedShort:
            for (int index8 = 0; index8 < format.Count; ++index8)
              objArray[num++] = (object) (int) PythonStruct.CreateUShortValue(context, ref index1, this._isLittleEndian, data);
            break;
          case PythonStruct.FormatType.Int:
            for (int index9 = 0; index9 < format.Count; ++index9)
              objArray[num++] = (object) PythonStruct.CreateIntValue(context, ref index1, this._isLittleEndian, data);
            break;
          case PythonStruct.FormatType.UnsignedInt:
          case PythonStruct.FormatType.UnsignedLong:
            for (int index10 = 0; index10 < format.Count; ++index10)
              objArray[num++] = BigIntegerOps.__int__((BigInteger) PythonStruct.CreateUIntValue(context, ref index1, this._isLittleEndian, data));
            break;
          case PythonStruct.FormatType.Float:
            for (int index11 = 0; index11 < format.Count; ++index11)
              objArray[num++] = (object) (double) PythonStruct.CreateFloatValue(context, ref index1, this._isLittleEndian, data);
            break;
          case PythonStruct.FormatType.LongLong:
            for (int index12 = 0; index12 < format.Count; ++index12)
              objArray[num++] = BigIntegerOps.__int__((BigInteger) PythonStruct.CreateLongValue(context, ref index1, this._isLittleEndian, data));
            break;
          case PythonStruct.FormatType.UnsignedLongLong:
            for (int index13 = 0; index13 < format.Count; ++index13)
              objArray[num++] = BigIntegerOps.__int__((BigInteger) PythonStruct.CreateULongValue(context, ref index1, this._isLittleEndian, data));
            break;
          case PythonStruct.FormatType.Double:
            for (int index14 = 0; index14 < format.Count; ++index14)
              objArray[num++] = (object) PythonStruct.CreateDoubleValue(context, ref index1, this._isLittleEndian, data);
            break;
          case PythonStruct.FormatType.CString:
            objArray[num++] = (object) PythonStruct.CreateString(context, ref index1, format.Count, data);
            break;
          case PythonStruct.FormatType.PascalString:
            objArray[num++] = (object) PythonStruct.CreatePascalString(context, ref index1, format.Count - 1, data);
            break;
          case PythonStruct.FormatType.Pointer:
            for (int index15 = 0; index15 < format.Count; ++index15)
              objArray[num++] = IntPtr.Size != 4 ? BigIntegerOps.__int__((BigInteger) PythonStruct.CreateLongValue(context, ref index1, this._isLittleEndian, data)) : (object) PythonStruct.CreateIntValue(context, ref index1, this._isLittleEndian, data);
            break;
        }
      }
      return PythonTuple.MakeTuple(objArray);
    }

    public PythonTuple unpack(CodeContext context, [BytesConversion, NotNull] IList<byte> @string)
    {
      return this.unpack(context, @string.MakeString());
    }

    public PythonTuple unpack(CodeContext context, [NotNull] ArrayModule.array buffer)
    {
      return this.unpack(context, ((IList<byte>) buffer.ToByteArray()).MakeString());
    }

    public PythonTuple unpack(CodeContext context, [NotNull] PythonBuffer buffer)
    {
      return this.unpack(context, buffer.ToString());
    }

    [Documentation("reads the current format from the specified string")]
    public PythonTuple unpack_from(CodeContext context, [NotNull] string buffer, int offset = 0)
    {
      if (buffer.Length - offset < this.size)
        throw PythonStruct.Error(context, $"unpack_from requires a buffer of at least {this.size} bytes");
      return this.unpack(context, buffer.Substring(offset, this.size));
    }

    [Documentation("reads the current format from the specified array")]
    public PythonTuple unpack_from(CodeContext context, [BytesConversion, NotNull] IList<byte> buffer, int offset = 0)
    {
      return this.unpack_from(context, buffer.MakeString(), offset);
    }

    [Documentation("reads the current format from the specified array")]
    public PythonTuple unpack_from(CodeContext context, [NotNull] ArrayModule.array buffer, int offset = 0)
    {
      return this.unpack_from(context, ((IList<byte>) buffer.ToByteArray()).MakeString(), offset);
    }

    [Documentation("reads the current format from the specified buffer object")]
    public PythonTuple unpack_from(CodeContext context, [NotNull] PythonBuffer buffer, int offset = 0)
    {
      return this.unpack_from(context, buffer.ToString(), offset);
    }

    [Documentation("gets the number of bytes that the serialized string will occupy or are required to deserialize the data")]
    public int size => this._encodingSize;

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._tracker;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      return Interlocked.CompareExchange<WeakRefTracker>(ref this._tracker, value, (WeakRefTracker) null) == null;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value) => this._tracker = value;

    private void Compile(CodeContext context, string fmt)
    {
      List<PythonStruct.Format> formatList = new List<PythonStruct.Format>();
      int count = 1;
      bool flag1 = BitConverter.IsLittleEndian;
      bool flag2 = false;
      for (int index = 0; index < fmt.Length; ++index)
      {
        switch (fmt[index])
        {
          case '\t':
          case ' ':
            continue;
          case '!':
          case '>':
            if (index != 0)
              throw PythonStruct.Error(context, "unexpected byte order");
            flag1 = false;
            flag2 = true;
            continue;
          case '<':
            if (index != 0)
              throw PythonStruct.Error(context, "unexpected byte order");
            flag1 = true;
            flag2 = true;
            continue;
          case '=':
            if (index != 0)
              throw PythonStruct.Error(context, "unexpected byte order");
            flag2 = true;
            continue;
          case '?':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.Bool, count));
            count = 1;
            continue;
          case '@':
            if (index != 0)
              throw PythonStruct.Error(context, "unexpected byte order");
            continue;
          case 'B':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.UnsignedChar, count));
            count = 1;
            continue;
          case 'H':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.UnsignedShort, count));
            count = 1;
            continue;
          case 'I':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.UnsignedInt, count));
            count = 1;
            continue;
          case 'L':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.UnsignedLong, count));
            count = 1;
            continue;
          case 'P':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.Pointer, count));
            count = 1;
            continue;
          case 'Q':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.UnsignedLongLong, count));
            count = 1;
            continue;
          case 'b':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.SignedChar, count));
            count = 1;
            continue;
          case 'c':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.Char, count));
            count = 1;
            continue;
          case 'd':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.Double, count));
            count = 1;
            continue;
          case 'f':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.Float, count));
            count = 1;
            continue;
          case 'h':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.Short, count));
            count = 1;
            continue;
          case 'i':
          case 'l':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.Int, count));
            count = 1;
            continue;
          case 'p':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.PascalString, count));
            count = 1;
            continue;
          case 'q':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.LongLong, count));
            count = 1;
            continue;
          case 's':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.CString, count));
            count = 1;
            continue;
          case 'x':
            formatList.Add(new PythonStruct.Format(PythonStruct.FormatType.PadByte, count));
            count = 1;
            continue;
          default:
            if (!char.IsDigit(fmt[index]))
              throw PythonStruct.Error(context, "bad format string");
            count = 0;
            for (; char.IsDigit(fmt[index]); ++index)
              count = count * 10 + ((int) fmt[index] - 48 /*0x30*/);
            if (char.IsWhiteSpace(fmt[index]))
              PythonStruct.Error(context, "white space not allowed between count and format");
            --index;
            continue;
        }
      }
      this._formats = formatList.ToArray();
      this._isStandardized = flag2;
      this._isLittleEndian = flag1;
      this._encodingSize = this._encodingCount = 0;
      for (int index = 0; index < this._formats.Length; ++index)
      {
        if (this._formats[index].Type != PythonStruct.FormatType.PadByte)
        {
          if (this._formats[index].Type != PythonStruct.FormatType.CString && this._formats[index].Type != PythonStruct.FormatType.PascalString)
            this._encodingCount += this._formats[index].Count;
          else
            ++this._encodingCount;
        }
        if (!this._isStandardized)
          this._encodingSize = PythonStruct.Align(this._encodingSize, this._formats[index].NativeSize);
        this._encodingSize += PythonStruct.GetNativeSize(this._formats[index].Type) * this._formats[index].Count;
      }
      lock (PythonStruct._cache)
        PythonStruct._cache.Add(fmt, this);
    }

    internal static PythonStruct.Struct Create(string format)
    {
      PythonStruct.Struct @struct = new PythonStruct.Struct(new object[0]);
      @struct.__init__(DefaultContext.Default, format);
      return @struct;
    }
  }

  private enum FormatType
  {
    None,
    PadByte,
    Bool,
    Char,
    SignedChar,
    UnsignedChar,
    Short,
    UnsignedShort,
    Int,
    UnsignedInt,
    UnsignedLong,
    Float,
    LongLong,
    UnsignedLongLong,
    Double,
    CString,
    PascalString,
    Pointer,
  }

  private struct Format(PythonStruct.FormatType type, int count)
  {
    public PythonStruct.FormatType Type = type;
    public int Count = count;

    public int NativeSize => PythonStruct.GetNativeSize(this.Type);
  }
}
