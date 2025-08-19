// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.UInt32Ops
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

public static class UInt32Ops
{
  [StaticExtensionMethod]
  public static object __new__(PythonType cls) => UInt32Ops.__new__(cls, (object) 0U);

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, object value)
  {
    if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (uint)))
      throw PythonOps.TypeError("UInt32.__new__: first argument must be UInt32 type.");
    if (value is IConvertible convertible)
    {
      switch (convertible.GetTypeCode())
      {
        case TypeCode.SByte:
          return (object) (uint) (sbyte) value;
        case TypeCode.Byte:
          return (object) (uint) (byte) value;
        case TypeCode.Int16:
          return (object) (uint) (short) value;
        case TypeCode.UInt16:
          return (object) (uint) (ushort) value;
        case TypeCode.Int32:
          return (object) (uint) (int) value;
        case TypeCode.UInt32:
          return (object) (uint) value;
        case TypeCode.Int64:
          return (object) (uint) (long) value;
        case TypeCode.UInt64:
          return (object) (uint) (ulong) value;
        case TypeCode.Single:
          return (object) (uint) (float) value;
        case TypeCode.Double:
          return (object) (uint) (double) value;
      }
    }
    if (value is string)
      return (object) uint.Parse((string) value);
    if (value is BigInteger bigInteger)
      return (object) (uint) bigInteger;
    if (value is Extensible<BigInteger>)
      return (object) (uint) ((Extensible<BigInteger>) value).Value;
    return value is Extensible<double> ? (object) (uint) ((Extensible<double>) value).Value : throw PythonOps.ValueError("invalid value for UInt32.__new__");
  }

  [SpecialName]
  public static uint Plus(uint x) => x;

  [SpecialName]
  public static object Negate(uint x) => Int64Ops.Negate((long) x);

  [SpecialName]
  public static uint Abs(uint x) => x;

  [SpecialName]
  public static object OnesComplement(uint x) => (object) Int64Ops.OnesComplement((long) x);

  public static bool __nonzero__(uint x) => x > 0U;

  public static string __repr__(uint x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static uint __trunc__(uint x) => x;

  public static int __hash__(uint x) => (int) x;

  public static int __index__(uint x) => (int) x;

  [SpecialName]
  public static object Add(uint x, uint y)
  {
    long num = (long) x + (long) y;
    return 0L <= num && num <= (long) uint.MaxValue ? (object) (uint) num : (object) num;
  }

  [SpecialName]
  public static object Add(uint x, int y) => Int64Ops.Add((long) x, (long) y);

  [SpecialName]
  public static object Add(int x, uint y) => Int64Ops.Add((long) x, (long) y);

  [SpecialName]
  public static object Subtract(uint x, uint y)
  {
    long num = (long) x - (long) y;
    return 0L <= num && num <= (long) uint.MaxValue ? (object) (uint) num : (object) num;
  }

  [SpecialName]
  public static object Subtract(uint x, int y) => Int64Ops.Subtract((long) x, (long) y);

  [SpecialName]
  public static object Subtract(int x, uint y) => Int64Ops.Subtract((long) x, (long) y);

  [SpecialName]
  public static object Multiply(uint x, uint y)
  {
    long num = (long) x * (long) y;
    return 0L <= num && num <= (long) uint.MaxValue ? (object) (uint) num : (object) num;
  }

  [SpecialName]
  public static object Multiply(uint x, int y) => Int64Ops.Multiply((long) x, (long) y);

  [SpecialName]
  public static object Multiply(int x, uint y) => Int64Ops.Multiply((long) x, (long) y);

  [SpecialName]
  public static object Divide(uint x, uint y) => (object) UInt32Ops.FloorDivide(x, y);

  [SpecialName]
  public static object Divide(uint x, int y) => Int64Ops.Divide((long) x, (long) y);

  [SpecialName]
  public static object Divide(int x, uint y) => Int64Ops.Divide((long) x, (long) y);

  [SpecialName]
  public static double TrueDivide(uint x, uint y) => DoubleOps.TrueDivide((double) x, (double) y);

  [SpecialName]
  public static double TrueDivide(uint x, int y) => Int64Ops.TrueDivide((long) x, (long) y);

  [SpecialName]
  public static double TrueDivide(int x, uint y) => Int64Ops.TrueDivide((long) x, (long) y);

  [SpecialName]
  public static uint FloorDivide(uint x, uint y) => x / y;

  [SpecialName]
  public static object FloorDivide(uint x, int y) => Int64Ops.FloorDivide((long) x, (long) y);

  [SpecialName]
  public static object FloorDivide(int x, uint y) => Int64Ops.FloorDivide((long) x, (long) y);

  [SpecialName]
  public static uint Mod(uint x, uint y) => x % y;

  [SpecialName]
  public static long Mod(uint x, int y) => Int64Ops.Mod((long) x, (long) y);

  [SpecialName]
  public static long Mod(int x, uint y) => Int64Ops.Mod((long) x, (long) y);

  [SpecialName]
  public static object Power(uint x, uint y) => Int32Ops.Power((int) x, (int) y);

  [SpecialName]
  public static object Power(uint x, int y) => Int64Ops.Power((long) x, (long) y);

  [SpecialName]
  public static object Power(int x, uint y) => Int64Ops.Power((long) x, (long) y);

  [SpecialName]
  public static object LeftShift(uint x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static uint RightShift(uint x, [NotNull] BigInteger y)
  {
    return (uint) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static uint BitwiseAnd(uint x, uint y) => x & y;

  [SpecialName]
  public static long BitwiseAnd(uint x, int y) => Int64Ops.BitwiseAnd((long) x, (long) y);

  [SpecialName]
  public static long BitwiseAnd(int x, uint y) => Int64Ops.BitwiseAnd((long) x, (long) y);

  [SpecialName]
  public static uint BitwiseOr(uint x, uint y) => x | y;

  [SpecialName]
  public static long BitwiseOr(uint x, int y) => Int64Ops.BitwiseOr((long) x, (long) y);

  [SpecialName]
  public static long BitwiseOr(int x, uint y) => Int64Ops.BitwiseOr((long) x, (long) y);

  [SpecialName]
  public static uint ExclusiveOr(uint x, uint y) => x ^ y;

  [SpecialName]
  public static long ExclusiveOr(uint x, int y) => Int64Ops.ExclusiveOr((long) x, (long) y);

  [SpecialName]
  public static long ExclusiveOr(int x, uint y) => Int64Ops.ExclusiveOr((long) x, (long) y);

  [SpecialName]
  public static int Compare(uint x, uint y)
  {
    if ((int) x == (int) y)
      return 0;
    return x <= y ? -1 : 1;
  }

  [SpecialName]
  public static int Compare(uint x, int y) => Int64Ops.Compare((long) x, (long) y);

  [SpecialName]
  public static int Compare(int x, uint y) => Int64Ops.Compare((long) x, (long) y);

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(uint x)
  {
    return x <= (uint) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(uint x)
  {
    return 0U <= x && x <= (uint) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(uint x)
  {
    return x <= (uint) short.MaxValue ? (short) x : throw Converter.CannotConvertOverflow("Int16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(uint x)
  {
    return 0U <= x && x <= (uint) ushort.MaxValue ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(uint x)
  {
    return x <= (uint) int.MaxValue ? (int) x : throw Converter.CannotConvertOverflow("Int32", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(uint x) => (long) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(uint x) => (ulong) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(uint x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(uint x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static uint Getreal(uint x) => x;

  [PropertyMethod]
  [SpecialName]
  public static uint Getimag(uint x) => 0;

  public static uint conjugate(uint x) => x;

  [PropertyMethod]
  [SpecialName]
  public static uint Getnumerator(uint x) => x;

  [PropertyMethod]
  [SpecialName]
  public static uint Getdenominator(uint x) => 1;

  public static string __hex__(uint value) => BigIntegerOps.__hex__((BigInteger) value);

  public static int bit_length(uint value) => MathUtils.BitLengthUnsigned(value);
}
