// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.SByteOps
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

public static class SByteOps
{
  [StaticExtensionMethod]
  public static object __new__(PythonType cls) => SByteOps.__new__(cls, (object) (sbyte) 0);

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, object value)
  {
    if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (sbyte)))
      throw PythonOps.TypeError("SByte.__new__: first argument must be SByte type.");
    if (value is IConvertible convertible)
    {
      switch (convertible.GetTypeCode())
      {
        case TypeCode.SByte:
          return (object) (sbyte) value;
        case TypeCode.Byte:
          return (object) (sbyte) (byte) value;
        case TypeCode.Int16:
          return (object) (sbyte) (short) value;
        case TypeCode.UInt16:
          return (object) (sbyte) (ushort) value;
        case TypeCode.Int32:
          return (object) (sbyte) (int) value;
        case TypeCode.UInt32:
          return (object) (sbyte) (uint) value;
        case TypeCode.Int64:
          return (object) (sbyte) (long) value;
        case TypeCode.UInt64:
          return (object) (sbyte) (ulong) value;
        case TypeCode.Single:
          return (object) (sbyte) (float) value;
        case TypeCode.Double:
          return (object) (sbyte) (double) value;
      }
    }
    if (value is string)
      return (object) sbyte.Parse((string) value);
    if (value is BigInteger bigInteger)
      return (object) (sbyte) bigInteger;
    if (value is Extensible<BigInteger>)
      return (object) (sbyte) ((Extensible<BigInteger>) value).Value;
    return value is Extensible<double> ? (object) (sbyte) ((Extensible<double>) value).Value : throw PythonOps.ValueError("invalid value for SByte.__new__");
  }

  [SpecialName]
  public static sbyte Plus(sbyte x) => x;

  [SpecialName]
  public static object Negate(sbyte x) => x == sbyte.MinValue ? (object) 128 /*0x80*/ : (object) -x;

  [SpecialName]
  public static object Abs(sbyte x)
  {
    if (x >= (sbyte) 0)
      return (object) x;
    return x == sbyte.MinValue ? (object) 128 /*0x80*/ : (object) -x;
  }

  [SpecialName]
  public static sbyte OnesComplement(sbyte x) => ~x;

  public static bool __nonzero__(sbyte x) => x != (sbyte) 0;

  public static string __repr__(sbyte x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static sbyte __trunc__(sbyte x) => x;

  public static int __hash__(sbyte x) => (int) x;

  public static int __index__(sbyte x) => (int) x;

  [SpecialName]
  public static object Add(sbyte x, sbyte y)
  {
    short num = (short) ((int) x + (int) y);
    return (short) sbyte.MinValue <= num && num <= (short) sbyte.MaxValue ? (object) (sbyte) num : (object) num;
  }

  [SpecialName]
  public static object Subtract(sbyte x, sbyte y)
  {
    short num = (short) ((int) x - (int) y);
    return (short) sbyte.MinValue <= num && num <= (short) sbyte.MaxValue ? (object) (sbyte) num : (object) num;
  }

  [SpecialName]
  public static object Multiply(sbyte x, sbyte y)
  {
    short num = (short) ((int) x * (int) y);
    return (short) sbyte.MinValue <= num && num <= (short) sbyte.MaxValue ? (object) (sbyte) num : (object) num;
  }

  [SpecialName]
  public static object Divide(sbyte x, sbyte y) => SByteOps.FloorDivide(x, y);

  [SpecialName]
  public static double TrueDivide(sbyte x, sbyte y) => DoubleOps.TrueDivide((double) x, (double) y);

  [SpecialName]
  public static object FloorDivide(sbyte x, sbyte y)
  {
    return y == (sbyte) -1 && x == sbyte.MinValue ? (object) 128 /*0x80*/ : (object) (sbyte) MathUtils.FloorDivideUnchecked((int) x, (int) y);
  }

  [SpecialName]
  public static sbyte Mod(sbyte x, sbyte y) => (sbyte) Int32Ops.Mod((int) x, (int) y);

  [SpecialName]
  public static object Power(sbyte x, sbyte y) => Int32Ops.Power((int) x, (int) y);

  [SpecialName]
  public static object LeftShift(sbyte x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static sbyte RightShift(sbyte x, [NotNull] BigInteger y)
  {
    return (sbyte) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static object LeftShift(sbyte x, int y) => Int32Ops.LeftShift((int) x, y);

  [SpecialName]
  public static sbyte RightShift(sbyte x, int y) => (sbyte) Int32Ops.RightShift((int) x, y);

  [SpecialName]
  public static sbyte BitwiseAnd(sbyte x, sbyte y) => (sbyte) ((int) x & (int) y);

  [SpecialName]
  public static sbyte BitwiseOr(sbyte x, sbyte y) => (sbyte) ((int) x | (int) y);

  [SpecialName]
  public static sbyte ExclusiveOr(sbyte x, sbyte y) => (sbyte) ((int) x ^ (int) y);

  [SpecialName]
  public static int Compare(sbyte x, sbyte y)
  {
    if ((int) x == (int) y)
      return 0;
    return (int) x <= (int) y ? -1 : 1;
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(sbyte x)
  {
    return x >= (sbyte) 0 ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(sbyte x) => (short) x;

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(sbyte x)
  {
    return x >= (sbyte) 0 ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(sbyte x) => (int) x;

  [ExplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(sbyte x)
  {
    return x >= (sbyte) 0 ? (uint) x : throw Converter.CannotConvertOverflow("UInt32", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(sbyte x) => (long) x;

  [ExplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(sbyte x)
  {
    return x >= (sbyte) 0 ? (ulong) x : throw Converter.CannotConvertOverflow("UInt64", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(sbyte x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(sbyte x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static sbyte Getreal(sbyte x) => x;

  [PropertyMethod]
  [SpecialName]
  public static sbyte Getimag(sbyte x) => 0;

  public static sbyte conjugate(sbyte x) => x;

  [PropertyMethod]
  [SpecialName]
  public static sbyte Getnumerator(sbyte x) => x;

  [PropertyMethod]
  [SpecialName]
  public static sbyte Getdenominator(sbyte x) => 1;

  public static string __hex__(sbyte value) => BigIntegerOps.__hex__((BigInteger) value);

  public static int bit_length(sbyte value) => MathUtils.BitLength((int) value);
}
