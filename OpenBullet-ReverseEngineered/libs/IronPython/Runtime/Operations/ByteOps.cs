// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ByteOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class ByteOps
{
  internal static byte ToByteChecked(this int item)
  {
    try
    {
      return checked ((byte) item);
    }
    catch (OverflowException ex)
    {
      throw PythonOps.ValueError("byte must be in range(0, 256)");
    }
  }

  internal static byte ToByteChecked(this BigInteger item)
  {
    int ret;
    if (item.AsInt32(out ret))
      return ret.ToByteChecked();
    throw PythonOps.ValueError("byte must be in range(0, 256)");
  }

  internal static byte ToByteChecked(this double item)
  {
    try
    {
      return checked ((byte) item);
    }
    catch (OverflowException ex)
    {
      throw PythonOps.ValueError("byte must be in range(0, 256)");
    }
  }

  internal static bool IsSign(this byte ch) => ch == (byte) 43 || ch == (byte) 45;

  internal static byte ToUpper(this byte p)
  {
    if (p >= (byte) 97 && p <= (byte) 122)
      p -= (byte) 32 /*0x20*/;
    return p;
  }

  internal static byte ToLower(this byte p)
  {
    if (p >= (byte) 65 && p <= (byte) 90)
      p += (byte) 32 /*0x20*/;
    return p;
  }

  internal static bool IsLower(this byte p) => p >= (byte) 97 && p <= (byte) 122;

  internal static bool IsUpper(this byte p) => p >= (byte) 65 && p <= (byte) 90;

  internal static bool IsDigit(this byte b) => b >= (byte) 48 /*0x30*/ && b <= (byte) 57;

  internal static bool IsLetter(this byte b) => b.IsLower() || b.IsUpper();

  internal static bool IsWhiteSpace(this byte b)
  {
    return b == (byte) 32 /*0x20*/ || b == (byte) 9 || b == (byte) 10 || b == (byte) 13 || b == (byte) 12 || b == (byte) 11;
  }

  internal static void AppendJoin(object value, int index, List<byte> byteList)
  {
    switch (value)
    {
      case IList<byte> collection:
        byteList.AddRange((IEnumerable<byte>) collection);
        break;
      case string s:
        byteList.AddRange((IEnumerable<byte>) s.MakeByteArray());
        break;
      default:
        throw PythonOps.TypeError("sequence item {0}: expected bytes or byte array, {1} found", (object) index.ToString(), (object) PythonOps.GetPythonTypeName(value));
    }
  }

  internal static IList<byte> CoerceBytes(object obj)
  {
    return obj is IList<byte> byteList ? byteList : throw PythonOps.TypeError("expected string, got {0} Type", (object) PythonTypeOps.GetName(obj));
  }

  internal static List<byte> GetBytes(ICollection bytes)
  {
    return ByteOps.GetBytes(bytes, new Func<object, byte>(ByteOps.GetByte));
  }

  internal static List<byte> GetBytes(ICollection bytes, Func<object, byte> conversion)
  {
    List<byte> bytes1 = new List<byte>(bytes.Count);
    foreach (object obj in (IEnumerable) bytes)
      bytes1.Add(conversion(obj));
    return bytes1;
  }

  internal static byte GetByteStringOk(object o)
  {
    switch (o)
    {
      case string str:
        return str.Length == 1 ? ((int) str[0]).ToByteChecked() : throw PythonOps.TypeError("an integer or string of size 1 is required");
      case Extensible<string> extensible:
        if (extensible.Value.Length == 1)
          return ((int) extensible.Value[0]).ToByteChecked();
        throw PythonOps.TypeError("an integer or string of size 1 is required");
      default:
        return ByteOps.GetByteListOk(o);
    }
  }

  internal static byte GetByteListOk(object o)
  {
    if (!(o is IList<byte> byteList))
      return ByteOps.GetByte(o);
    return byteList.Count == 1 ? byteList[0] : throw PythonOps.ValueError("an integer or string of size 1 is required");
  }

  internal static byte GetByte(object o)
  {
    switch (o)
    {
      case int num1:
        return num1.ToByteChecked();
      case BigInteger bigInteger:
        return bigInteger.ToByteChecked();
      case double num2:
        return num2.ToByteChecked();
      case Extensible<int> extensible1:
        return extensible1.Value.ToByteChecked();
      case Extensible<BigInteger> extensible2:
        return extensible2.Value.ToByteChecked();
      case Extensible<double> extensible3:
        return extensible3.Value.ToByteChecked();
      case byte num3:
        return num3;
      case sbyte num4:
        return ((int) num4).ToByteChecked();
      case char ch:
        return ((int) ch).ToByteChecked();
      case short num5:
        return ((int) num5).ToByteChecked();
      case ushort num6:
        return ((int) num6).ToByteChecked();
      case uint num7:
        return ((BigInteger) num7).ToByteChecked();
      case float num8:
        return ((double) num8).ToByteChecked();
      default:
        int index;
        if (Converter.TryConvertToIndex(o, out index))
          return index.ToByteChecked();
        if (o is string str && str.Length == 1)
          return ((int) str[0]).ToByteChecked();
        throw PythonOps.TypeError("an integer or string of size 1 is required");
    }
  }

  [StaticExtensionMethod]
  public static object __new__(PythonType cls) => ByteOps.__new__(cls, (object) (byte) 0);

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, object value)
  {
    if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (byte)))
      throw PythonOps.TypeError("Byte.__new__: first argument must be Byte type.");
    if (value is IConvertible convertible)
    {
      switch (convertible.GetTypeCode())
      {
        case TypeCode.SByte:
          return (object) (byte) (sbyte) value;
        case TypeCode.Byte:
          return (object) (byte) value;
        case TypeCode.Int16:
          return (object) (byte) (short) value;
        case TypeCode.UInt16:
          return (object) (byte) (ushort) value;
        case TypeCode.Int32:
          return (object) (byte) (int) value;
        case TypeCode.UInt32:
          return (object) (byte) (uint) value;
        case TypeCode.Int64:
          return (object) (byte) (long) value;
        case TypeCode.UInt64:
          return (object) (byte) (ulong) value;
        case TypeCode.Single:
          return (object) (byte) (float) value;
        case TypeCode.Double:
          return (object) (byte) (double) value;
      }
    }
    if (value is string)
      return (object) byte.Parse((string) value);
    if (value is BigInteger bigInteger)
      return (object) (byte) bigInteger;
    if (value is Extensible<BigInteger>)
      return (object) (byte) ((Extensible<BigInteger>) value).Value;
    return value is Extensible<double> ? (object) (byte) ((Extensible<double>) value).Value : throw PythonOps.ValueError("invalid value for Byte.__new__");
  }

  [SpecialName]
  public static byte Plus(byte x) => x;

  [SpecialName]
  public static object Negate(byte x) => Int16Ops.Negate((short) x);

  [SpecialName]
  public static byte Abs(byte x) => x;

  [SpecialName]
  public static object OnesComplement(byte x) => (object) Int16Ops.OnesComplement((short) x);

  public static bool __nonzero__(byte x) => x > (byte) 0;

  public static string __repr__(byte x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static byte __trunc__(byte x) => x;

  public static int __hash__(byte x) => (int) x;

  public static int __index__(byte x) => (int) x;

  [SpecialName]
  public static object Add(byte x, byte y)
  {
    short num = (short) ((int) x + (int) y);
    return (short) 0 <= num && num <= (short) byte.MaxValue ? (object) (byte) num : (object) num;
  }

  [SpecialName]
  public static object Add(byte x, sbyte y) => Int16Ops.Add((short) x, (short) y);

  [SpecialName]
  public static object Add(sbyte x, byte y) => Int16Ops.Add((short) x, (short) y);

  [SpecialName]
  public static object Subtract(byte x, byte y)
  {
    short num = (short) ((int) x - (int) y);
    return (short) 0 <= num && num <= (short) byte.MaxValue ? (object) (byte) num : (object) num;
  }

  [SpecialName]
  public static object Subtract(byte x, sbyte y) => Int16Ops.Subtract((short) x, (short) y);

  [SpecialName]
  public static object Subtract(sbyte x, byte y) => Int16Ops.Subtract((short) x, (short) y);

  [SpecialName]
  public static object Multiply(byte x, byte y)
  {
    short num = (short) ((int) x * (int) y);
    return (short) 0 <= num && num <= (short) byte.MaxValue ? (object) (byte) num : (object) num;
  }

  [SpecialName]
  public static object Multiply(byte x, sbyte y) => Int16Ops.Multiply((short) x, (short) y);

  [SpecialName]
  public static object Multiply(sbyte x, byte y) => Int16Ops.Multiply((short) x, (short) y);

  [SpecialName]
  public static object Divide(byte x, byte y) => (object) ByteOps.FloorDivide(x, y);

  [SpecialName]
  public static object Divide(byte x, sbyte y) => Int16Ops.Divide((short) x, (short) y);

  [SpecialName]
  public static object Divide(sbyte x, byte y) => Int16Ops.Divide((short) x, (short) y);

  [SpecialName]
  public static double TrueDivide(byte x, byte y) => DoubleOps.TrueDivide((double) x, (double) y);

  [SpecialName]
  public static double TrueDivide(byte x, sbyte y) => Int16Ops.TrueDivide((short) x, (short) y);

  [SpecialName]
  public static double TrueDivide(sbyte x, byte y) => Int16Ops.TrueDivide((short) x, (short) y);

  [SpecialName]
  public static byte FloorDivide(byte x, byte y) => (byte) ((uint) x / (uint) y);

  [SpecialName]
  public static object FloorDivide(byte x, sbyte y) => Int16Ops.FloorDivide((short) x, (short) y);

  [SpecialName]
  public static object FloorDivide(sbyte x, byte y) => Int16Ops.FloorDivide((short) x, (short) y);

  [SpecialName]
  public static byte Mod(byte x, byte y) => (byte) ((uint) x % (uint) y);

  [SpecialName]
  public static short Mod(byte x, sbyte y) => Int16Ops.Mod((short) x, (short) y);

  [SpecialName]
  public static short Mod(sbyte x, byte y) => Int16Ops.Mod((short) x, (short) y);

  [SpecialName]
  public static object Power(byte x, byte y) => Int32Ops.Power((int) x, (int) y);

  [SpecialName]
  public static object Power(byte x, sbyte y) => Int16Ops.Power((short) x, (short) y);

  [SpecialName]
  public static object Power(sbyte x, byte y) => Int16Ops.Power((short) x, (short) y);

  [SpecialName]
  public static object LeftShift(byte x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static byte RightShift(byte x, [NotNull] BigInteger y)
  {
    return (byte) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static object LeftShift(byte x, int y) => Int32Ops.LeftShift((int) x, y);

  [SpecialName]
  public static byte RightShift(byte x, int y) => (byte) Int32Ops.RightShift((int) x, y);

  [SpecialName]
  public static byte BitwiseAnd(byte x, byte y) => (byte) ((uint) x & (uint) y);

  [SpecialName]
  public static short BitwiseAnd(byte x, sbyte y) => Int16Ops.BitwiseAnd((short) x, (short) y);

  [SpecialName]
  public static short BitwiseAnd(sbyte x, byte y) => Int16Ops.BitwiseAnd((short) x, (short) y);

  [SpecialName]
  public static byte BitwiseOr(byte x, byte y) => (byte) ((uint) x | (uint) y);

  [SpecialName]
  public static short BitwiseOr(byte x, sbyte y) => Int16Ops.BitwiseOr((short) x, (short) y);

  [SpecialName]
  public static short BitwiseOr(sbyte x, byte y) => Int16Ops.BitwiseOr((short) x, (short) y);

  [SpecialName]
  public static byte ExclusiveOr(byte x, byte y) => (byte) ((uint) x ^ (uint) y);

  [SpecialName]
  public static short ExclusiveOr(byte x, sbyte y) => Int16Ops.ExclusiveOr((short) x, (short) y);

  [SpecialName]
  public static short ExclusiveOr(sbyte x, byte y) => Int16Ops.ExclusiveOr((short) x, (short) y);

  [SpecialName]
  public static int Compare(byte x, byte y)
  {
    if ((int) x == (int) y)
      return 0;
    return (int) x <= (int) y ? -1 : 1;
  }

  [SpecialName]
  public static int Compare(byte x, sbyte y) => Int16Ops.Compare((short) x, (short) y);

  [SpecialName]
  public static int Compare(sbyte x, byte y) => Int16Ops.Compare((short) x, (short) y);

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(byte x)
  {
    return x <= (byte) 127 /*0x7F*/ ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(byte x) => (short) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(byte x) => (ushort) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(byte x) => (int) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(byte x) => (uint) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(byte x) => (long) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(byte x) => (ulong) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(byte x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(byte x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static byte Getreal(byte x) => x;

  [PropertyMethod]
  [SpecialName]
  public static byte Getimag(byte x) => 0;

  public static byte conjugate(byte x) => x;

  [PropertyMethod]
  [SpecialName]
  public static byte Getnumerator(byte x) => x;

  [PropertyMethod]
  [SpecialName]
  public static byte Getdenominator(byte x) => 1;

  public static string __hex__(byte value) => BigIntegerOps.__hex__((BigInteger) value);

  public static int bit_length(byte value) => MathUtils.BitLength((int) value);
}
