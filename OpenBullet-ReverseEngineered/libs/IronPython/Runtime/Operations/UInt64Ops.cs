// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.UInt64Ops
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class UInt64Ops
{
  [StaticExtensionMethod]
  public static object __new__(PythonType cls) => UInt64Ops.__new__(cls, (object) 0UL);

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, object value)
  {
    if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (ulong)))
      throw PythonOps.TypeError("UInt64.__new__: first argument must be UInt64 type.");
    if (value is IConvertible convertible)
    {
      switch (convertible.GetTypeCode())
      {
        case TypeCode.SByte:
          return (object) (ulong) (sbyte) value;
        case TypeCode.Byte:
          return (object) (ulong) (byte) value;
        case TypeCode.Int16:
          return (object) (ulong) (short) value;
        case TypeCode.UInt16:
          return (object) (ulong) (ushort) value;
        case TypeCode.Int32:
          return (object) (ulong) (int) value;
        case TypeCode.UInt32:
          return (object) (ulong) (uint) value;
        case TypeCode.Int64:
          return (object) (ulong) (long) value;
        case TypeCode.UInt64:
          return (object) (ulong) value;
        case TypeCode.Single:
          return (object) (ulong) (float) value;
        case TypeCode.Double:
          return (object) (ulong) (double) value;
      }
    }
    if (value is string)
      return (object) ulong.Parse((string) value);
    if (value is BigInteger bigInteger)
      return (object) (ulong) bigInteger;
    if (value is Extensible<BigInteger>)
      return (object) (ulong) ((Extensible<BigInteger>) value).Value;
    return value is Extensible<double> ? (object) (ulong) ((Extensible<double>) value).Value : throw PythonOps.ValueError("invalid value for UInt64.__new__");
  }

  [SpecialName]
  public static ulong Plus(ulong x) => x;

  [SpecialName]
  public static object Negate(ulong x) => BigIntegerOps.Negate((BigInteger) x);

  [SpecialName]
  public static ulong Abs(ulong x) => x;

  [SpecialName]
  public static object OnesComplement(ulong x)
  {
    return (object) BigIntegerOps.OnesComplement((BigInteger) x);
  }

  public static bool __nonzero__(ulong x) => x > 0UL;

  public static string __repr__(ulong x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static ulong __trunc__(ulong x) => x;

  public static int __hash__(ulong x)
  {
    int num = (int) (uint) x + (int) (uint) (x >> 32 /*0x20*/);
    return x < 0UL ? -num : num;
  }

  public static BigInteger __index__(ulong x) => (BigInteger) x;

  [SpecialName]
  public static object Add(ulong x, ulong y)
  {
    try
    {
      return (object) checked (x + y);
    }
    catch (OverflowException ex)
    {
      return (object) BigIntegerOps.Add((BigInteger) x, (BigInteger) y);
    }
  }

  [SpecialName]
  public static object Add(ulong x, long y)
  {
    return (object) BigIntegerOps.Add((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Add(long x, ulong y)
  {
    return (object) BigIntegerOps.Add((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Subtract(ulong x, ulong y)
  {
    try
    {
      return (object) checked (x - y);
    }
    catch (OverflowException ex)
    {
      return (object) BigIntegerOps.Subtract((BigInteger) x, (BigInteger) y);
    }
  }

  [SpecialName]
  public static object Subtract(ulong x, long y)
  {
    return (object) BigIntegerOps.Subtract((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Subtract(long x, ulong y)
  {
    return (object) BigIntegerOps.Subtract((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Multiply(ulong x, ulong y)
  {
    try
    {
      return (object) checked (x * y);
    }
    catch (OverflowException ex)
    {
      return (object) BigIntegerOps.Multiply((BigInteger) x, (BigInteger) y);
    }
  }

  [SpecialName]
  public static object Multiply(ulong x, long y)
  {
    return (object) BigIntegerOps.Multiply((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Multiply(long x, ulong y)
  {
    return (object) BigIntegerOps.Multiply((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Divide(ulong x, ulong y) => (object) UInt64Ops.FloorDivide(x, y);

  [SpecialName]
  public static object Divide(ulong x, long y)
  {
    return (object) BigIntegerOps.Divide((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Divide(long x, ulong y)
  {
    return (object) BigIntegerOps.Divide((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static double TrueDivide(ulong x, ulong y) => DoubleOps.TrueDivide((double) x, (double) y);

  [SpecialName]
  public static double TrueDivide(ulong x, long y)
  {
    return BigIntegerOps.TrueDivide((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static double TrueDivide(long x, ulong y)
  {
    return BigIntegerOps.TrueDivide((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static ulong FloorDivide(ulong x, ulong y) => x / y;

  [SpecialName]
  public static object FloorDivide(ulong x, long y)
  {
    return (object) BigIntegerOps.FloorDivide((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object FloorDivide(long x, ulong y)
  {
    return (object) BigIntegerOps.FloorDivide((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static ulong Mod(ulong x, ulong y) => x % y;

  [SpecialName]
  public static BigInteger Mod(ulong x, long y)
  {
    return BigIntegerOps.Mod((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static BigInteger Mod(long x, ulong y)
  {
    return BigIntegerOps.Mod((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Power(ulong x, ulong y)
  {
    return BigIntegerOps.Power((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Power(ulong x, long y)
  {
    return BigIntegerOps.Power((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Power(long x, ulong y)
  {
    return BigIntegerOps.Power((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object LeftShift(ulong x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static ulong RightShift(ulong x, [NotNull] BigInteger y)
  {
    return (ulong) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static ulong BitwiseAnd(ulong x, ulong y) => x & y;

  [SpecialName]
  public static BigInteger BitwiseAnd(ulong x, long y)
  {
    return BigIntegerOps.BitwiseAnd((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static BigInteger BitwiseAnd(long x, ulong y)
  {
    return BigIntegerOps.BitwiseAnd((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static ulong BitwiseOr(ulong x, ulong y) => x | y;

  [SpecialName]
  public static BigInteger BitwiseOr(ulong x, long y)
  {
    return BigIntegerOps.BitwiseOr((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static BigInteger BitwiseOr(long x, ulong y)
  {
    return BigIntegerOps.BitwiseOr((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static ulong ExclusiveOr(ulong x, ulong y) => x ^ y;

  [SpecialName]
  public static BigInteger ExclusiveOr(ulong x, long y)
  {
    return BigIntegerOps.ExclusiveOr((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static BigInteger ExclusiveOr(long x, ulong y)
  {
    return BigIntegerOps.ExclusiveOr((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static int Compare(ulong x, ulong y)
  {
    if ((long) x == (long) y)
      return 0;
    return x <= y ? -1 : 1;
  }

  [SpecialName]
  public static int Compare(ulong x, long y)
  {
    return BigIntegerOps.Compare((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static int Compare(long x, ulong y)
  {
    return BigIntegerOps.Compare((BigInteger) x, (BigInteger) y);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(ulong x)
  {
    return x <= (ulong) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(ulong x)
  {
    return 0UL <= x && x <= (ulong) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(ulong x)
  {
    return x <= (ulong) short.MaxValue ? (short) x : throw Converter.CannotConvertOverflow("Int16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(ulong x)
  {
    return 0UL <= x && x <= (ulong) ushort.MaxValue ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(ulong x)
  {
    return x <= (ulong) int.MaxValue ? (int) x : throw Converter.CannotConvertOverflow("Int32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(ulong x)
  {
    return 0UL <= x && x <= (ulong) uint.MaxValue ? (uint) x : throw Converter.CannotConvertOverflow("UInt32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(ulong x)
  {
    return x <= (ulong) long.MaxValue ? (long) x : throw Converter.CannotConvertOverflow("Int64", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(ulong x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(ulong x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static ulong Getreal(ulong x) => x;

  [PropertyMethod]
  [SpecialName]
  public static ulong Getimag(ulong x) => 0;

  public static ulong conjugate(ulong x) => x;

  [PropertyMethod]
  [SpecialName]
  public static ulong Getnumerator(ulong x) => x;

  [PropertyMethod]
  [SpecialName]
  public static ulong Getdenominator(ulong x) => 1;

  public static string __hex__(ulong value) => BigIntegerOps.__hex__((BigInteger) value);

  public static int bit_length(ulong value) => MathUtils.BitLengthUnsigned(value);
}
