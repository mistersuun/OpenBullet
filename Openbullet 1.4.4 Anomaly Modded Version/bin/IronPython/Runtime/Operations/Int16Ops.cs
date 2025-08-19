// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.Int16Ops
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

public static class Int16Ops
{
  [StaticExtensionMethod]
  public static object __new__(PythonType cls) => Int16Ops.__new__(cls, (object) (short) 0);

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, object value)
  {
    if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (short)))
      throw PythonOps.TypeError("Int16.__new__: first argument must be Int16 type.");
    if (value is IConvertible convertible)
    {
      switch (convertible.GetTypeCode())
      {
        case TypeCode.SByte:
          return (object) (short) (sbyte) value;
        case TypeCode.Byte:
          return (object) (short) (byte) value;
        case TypeCode.Int16:
          return (object) (short) value;
        case TypeCode.UInt16:
          return (object) (short) (ushort) value;
        case TypeCode.Int32:
          return (object) (short) (int) value;
        case TypeCode.UInt32:
          return (object) (short) (uint) value;
        case TypeCode.Int64:
          return (object) (short) (long) value;
        case TypeCode.UInt64:
          return (object) (short) (ulong) value;
        case TypeCode.Single:
          return (object) (short) (float) value;
        case TypeCode.Double:
          return (object) (short) (double) value;
      }
    }
    if (value is string)
      return (object) short.Parse((string) value);
    if (value is BigInteger bigInteger)
      return (object) (short) bigInteger;
    if (value is Extensible<BigInteger>)
      return (object) (short) ((Extensible<BigInteger>) value).Value;
    return value is Extensible<double> ? (object) (short) ((Extensible<double>) value).Value : throw PythonOps.ValueError("invalid value for Int16.__new__");
  }

  [SpecialName]
  public static short Plus(short x) => x;

  [SpecialName]
  public static object Negate(short x)
  {
    return x == short.MinValue ? (object) 32768 /*0x8000*/ : (object) -x;
  }

  [SpecialName]
  public static object Abs(short x)
  {
    if (x >= (short) 0)
      return (object) x;
    return x == short.MinValue ? (object) 32768 /*0x8000*/ : (object) -x;
  }

  [SpecialName]
  public static short OnesComplement(short x) => ~x;

  public static bool __nonzero__(short x) => x != (short) 0;

  public static string __repr__(short x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static short __trunc__(short x) => x;

  public static int __hash__(short x) => (int) x;

  public static int __index__(short x) => (int) x;

  [SpecialName]
  public static object Add(short x, short y)
  {
    int num = (int) x + (int) y;
    return (int) short.MinValue <= num && num <= (int) short.MaxValue ? (object) (short) num : (object) num;
  }

  [SpecialName]
  public static object Subtract(short x, short y)
  {
    int num = (int) x - (int) y;
    return (int) short.MinValue <= num && num <= (int) short.MaxValue ? (object) (short) num : (object) num;
  }

  [SpecialName]
  public static object Multiply(short x, short y)
  {
    int num = (int) x * (int) y;
    return (int) short.MinValue <= num && num <= (int) short.MaxValue ? (object) (short) num : (object) num;
  }

  [SpecialName]
  public static object Divide(short x, short y) => Int16Ops.FloorDivide(x, y);

  [SpecialName]
  public static double TrueDivide(short x, short y) => DoubleOps.TrueDivide((double) x, (double) y);

  [SpecialName]
  public static object FloorDivide(short x, short y)
  {
    return y == (short) -1 && x == short.MinValue ? (object) 32768 /*0x8000*/ : (object) (short) MathUtils.FloorDivideUnchecked((int) x, (int) y);
  }

  [SpecialName]
  public static short Mod(short x, short y) => (short) Int32Ops.Mod((int) x, (int) y);

  [SpecialName]
  public static object Power(short x, short y) => Int32Ops.Power((int) x, (int) y);

  [SpecialName]
  public static object LeftShift(short x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static short RightShift(short x, [NotNull] BigInteger y)
  {
    return (short) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static object LeftShift(short x, int y) => Int32Ops.LeftShift((int) x, y);

  [SpecialName]
  public static short RightShift(short x, int y) => (short) Int32Ops.RightShift((int) x, y);

  [SpecialName]
  public static short BitwiseAnd(short x, short y) => (short) ((int) x & (int) y);

  [SpecialName]
  public static short BitwiseOr(short x, short y) => (short) ((int) x | (int) y);

  [SpecialName]
  public static short ExclusiveOr(short x, short y) => (short) ((int) x ^ (int) y);

  [SpecialName]
  public static int Compare(short x, short y)
  {
    if ((int) x == (int) y)
      return 0;
    return (int) x <= (int) y ? -1 : 1;
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(short x)
  {
    return (short) sbyte.MinValue <= x && x <= (short) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(short x)
  {
    return (short) 0 <= x && x <= (short) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(short x)
  {
    return x >= (short) 0 ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(short x) => (int) x;

  [ExplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(short x)
  {
    return x >= (short) 0 ? (uint) x : throw Converter.CannotConvertOverflow("UInt32", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(short x) => (long) x;

  [ExplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(short x)
  {
    return x >= (short) 0 ? (ulong) x : throw Converter.CannotConvertOverflow("UInt64", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(short x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(short x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static short Getreal(short x) => x;

  [PropertyMethod]
  [SpecialName]
  public static short Getimag(short x) => 0;

  public static short conjugate(short x) => x;

  [PropertyMethod]
  [SpecialName]
  public static short Getnumerator(short x) => x;

  [PropertyMethod]
  [SpecialName]
  public static short Getdenominator(short x) => 1;

  public static string __hex__(short value) => BigIntegerOps.__hex__((BigInteger) value);

  public static int bit_length(short value) => MathUtils.BitLength((int) value);
}
