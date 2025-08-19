// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.Int64Ops
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

public static class Int64Ops
{
  [StaticExtensionMethod]
  public static object __new__(PythonType cls) => Int64Ops.__new__(cls, (object) 0L);

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, object value)
  {
    if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (long)))
      throw PythonOps.TypeError("Int64.__new__: first argument must be Int64 type.");
    if (value is IConvertible convertible)
    {
      switch (convertible.GetTypeCode())
      {
        case TypeCode.SByte:
          return (object) (long) (sbyte) value;
        case TypeCode.Byte:
          return (object) (long) (byte) value;
        case TypeCode.Int16:
          return (object) (long) (short) value;
        case TypeCode.UInt16:
          return (object) (long) (ushort) value;
        case TypeCode.Int32:
          return (object) (long) (int) value;
        case TypeCode.UInt32:
          return (object) (long) (uint) value;
        case TypeCode.Int64:
          return (object) (long) value;
        case TypeCode.UInt64:
          return (object) (long) (ulong) value;
        case TypeCode.Single:
          return (object) (long) (float) value;
        case TypeCode.Double:
          return (object) (long) (double) value;
      }
    }
    if (value is string)
      return (object) long.Parse((string) value);
    if (value is BigInteger bigInteger)
      return (object) (long) bigInteger;
    if (value is Extensible<BigInteger>)
      return (object) (long) ((Extensible<BigInteger>) value).Value;
    return value is Extensible<double> ? (object) (long) ((Extensible<double>) value).Value : throw PythonOps.ValueError("invalid value for Int64.__new__");
  }

  [SpecialName]
  public static long Plus(long x) => x;

  [SpecialName]
  public static object Negate(long x)
  {
    return x == long.MinValue ? (object) -(BigInteger) long.MinValue : (object) -x;
  }

  [SpecialName]
  public static object Abs(long x)
  {
    if (x >= 0L)
      return (object) x;
    return x == long.MinValue ? (object) -(BigInteger) long.MinValue : (object) -x;
  }

  [SpecialName]
  public static long OnesComplement(long x) => ~x;

  public static bool __nonzero__(long x) => x != 0L;

  public static string __repr__(long x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static long __trunc__(long x) => x;

  public static int __hash__(long x)
  {
    long num1 = x;
    if (num1 < 0L)
      num1 *= -1L;
    int num2 = (int) (uint) num1 + (int) (uint) (num1 >> 32 /*0x20*/);
    return x < 0L ? -num2 : num2;
  }

  public static BigInteger __index__(long x) => (BigInteger) x;

  [SpecialName]
  public static object Add(long x, long y)
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
  public static object Subtract(long x, long y)
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
  public static object Multiply(long x, long y)
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
  public static object Divide(long x, long y) => Int64Ops.FloorDivide(x, y);

  [SpecialName]
  public static double TrueDivide(long x, long y) => DoubleOps.TrueDivide((double) x, (double) y);

  [SpecialName]
  public static object FloorDivide(long x, long y)
  {
    return y == -1L && x == long.MinValue ? (object) -(BigInteger) long.MinValue : (object) MathUtils.FloorDivideUnchecked(x, y);
  }

  [SpecialName]
  public static long Mod(long x, long y)
  {
    return (long) BigIntegerOps.Mod((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Power(long x, long y) => BigIntegerOps.Power((BigInteger) x, (BigInteger) y);

  [SpecialName]
  public static object LeftShift(long x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static long RightShift(long x, [NotNull] BigInteger y)
  {
    return (long) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static long BitwiseAnd(long x, long y) => x & y;

  [SpecialName]
  public static long BitwiseOr(long x, long y) => x | y;

  [SpecialName]
  public static long ExclusiveOr(long x, long y) => x ^ y;

  [SpecialName]
  public static int Compare(long x, long y)
  {
    if (x == y)
      return 0;
    return x <= y ? -1 : 1;
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(long x)
  {
    return (long) sbyte.MinValue <= x && x <= (long) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(long x)
  {
    return 0L <= x && x <= (long) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(long x)
  {
    return (long) short.MinValue <= x && x <= (long) short.MaxValue ? (short) x : throw Converter.CannotConvertOverflow("Int16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(long x)
  {
    return 0L <= x && x <= (long) ushort.MaxValue ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(long x)
  {
    return (long) int.MinValue <= x && x <= (long) int.MaxValue ? (int) x : throw Converter.CannotConvertOverflow("Int32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(long x)
  {
    return 0L <= x && x <= (long) uint.MaxValue ? (uint) x : throw Converter.CannotConvertOverflow("UInt32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(long x)
  {
    return x >= 0L ? (ulong) x : throw Converter.CannotConvertOverflow("UInt64", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(long x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(long x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static long Getreal(long x) => x;

  [PropertyMethod]
  [SpecialName]
  public static long Getimag(long x) => 0;

  public static long conjugate(long x) => x;

  [PropertyMethod]
  [SpecialName]
  public static long Getnumerator(long x) => x;

  [PropertyMethod]
  [SpecialName]
  public static long Getdenominator(long x) => 1;

  public static string __hex__(long value) => BigIntegerOps.__hex__((BigInteger) value);

  public static int bit_length(long value) => MathUtils.BitLength(value);
}
