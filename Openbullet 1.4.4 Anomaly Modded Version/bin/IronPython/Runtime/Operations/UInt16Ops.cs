// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.UInt16Ops
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

public static class UInt16Ops
{
  [StaticExtensionMethod]
  public static object __new__(PythonType cls) => UInt16Ops.__new__(cls, (object) (ushort) 0);

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, object value)
  {
    if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (ushort)))
      throw PythonOps.TypeError("UInt16.__new__: first argument must be UInt16 type.");
    if (value is IConvertible convertible)
    {
      switch (convertible.GetTypeCode())
      {
        case TypeCode.SByte:
          return (object) (ushort) (sbyte) value;
        case TypeCode.Byte:
          return (object) (ushort) (byte) value;
        case TypeCode.Int16:
          return (object) (ushort) (short) value;
        case TypeCode.UInt16:
          return (object) (ushort) value;
        case TypeCode.Int32:
          return (object) (ushort) (int) value;
        case TypeCode.UInt32:
          return (object) (ushort) (uint) value;
        case TypeCode.Int64:
          return (object) (ushort) (long) value;
        case TypeCode.UInt64:
          return (object) (ushort) (ulong) value;
        case TypeCode.Single:
          return (object) (ushort) (float) value;
        case TypeCode.Double:
          return (object) (ushort) (double) value;
      }
    }
    if (value is string)
      return (object) ushort.Parse((string) value);
    if (value is BigInteger bigInteger)
      return (object) (ushort) bigInteger;
    if (value is Extensible<BigInteger>)
      return (object) (ushort) ((Extensible<BigInteger>) value).Value;
    return value is Extensible<double> ? (object) (ushort) ((Extensible<double>) value).Value : throw PythonOps.ValueError("invalid value for UInt16.__new__");
  }

  [SpecialName]
  public static ushort Plus(ushort x) => x;

  [SpecialName]
  public static object Negate(ushort x) => Int32Ops.Negate((int) x);

  [SpecialName]
  public static ushort Abs(ushort x) => x;

  [SpecialName]
  public static object OnesComplement(ushort x) => (object) Int32Ops.OnesComplement((int) x);

  public static bool __nonzero__(ushort x) => x > (ushort) 0;

  public static string __repr__(ushort x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static ushort __trunc__(ushort x) => x;

  public static int __hash__(ushort x) => (int) x;

  public static int __index__(ushort x) => (int) x;

  [SpecialName]
  public static object Add(ushort x, ushort y)
  {
    int num = (int) x + (int) y;
    return 0 <= num && num <= (int) ushort.MaxValue ? (object) (ushort) num : (object) num;
  }

  [SpecialName]
  public static object Add(ushort x, short y) => Int32Ops.Add((int) x, (int) y);

  [SpecialName]
  public static object Add(short x, ushort y) => Int32Ops.Add((int) x, (int) y);

  [SpecialName]
  public static object Subtract(ushort x, ushort y)
  {
    int num = (int) x - (int) y;
    return 0 <= num && num <= (int) ushort.MaxValue ? (object) (ushort) num : (object) num;
  }

  [SpecialName]
  public static object Subtract(ushort x, short y) => Int32Ops.Subtract((int) x, (int) y);

  [SpecialName]
  public static object Subtract(short x, ushort y) => Int32Ops.Subtract((int) x, (int) y);

  [SpecialName]
  public static object Multiply(ushort x, ushort y)
  {
    int num = (int) x * (int) y;
    return 0 <= num && num <= (int) ushort.MaxValue ? (object) (ushort) num : (object) num;
  }

  [SpecialName]
  public static object Multiply(ushort x, short y) => Int32Ops.Multiply((int) x, (int) y);

  [SpecialName]
  public static object Multiply(short x, ushort y) => Int32Ops.Multiply((int) x, (int) y);

  [SpecialName]
  public static object Divide(ushort x, ushort y) => (object) UInt16Ops.FloorDivide(x, y);

  [SpecialName]
  public static object Divide(ushort x, short y) => Int32Ops.Divide((int) x, (int) y);

  [SpecialName]
  public static object Divide(short x, ushort y) => Int32Ops.Divide((int) x, (int) y);

  [SpecialName]
  public static double TrueDivide(ushort x, ushort y)
  {
    return DoubleOps.TrueDivide((double) x, (double) y);
  }

  [SpecialName]
  public static double TrueDivide(ushort x, short y) => Int32Ops.TrueDivide((int) x, (int) y);

  [SpecialName]
  public static double TrueDivide(short x, ushort y) => Int32Ops.TrueDivide((int) x, (int) y);

  [SpecialName]
  public static ushort FloorDivide(ushort x, ushort y) => (ushort) ((uint) x / (uint) y);

  [SpecialName]
  public static object FloorDivide(ushort x, short y) => Int32Ops.FloorDivide((int) x, (int) y);

  [SpecialName]
  public static object FloorDivide(short x, ushort y) => Int32Ops.FloorDivide((int) x, (int) y);

  [SpecialName]
  public static ushort Mod(ushort x, ushort y) => (ushort) ((uint) x % (uint) y);

  [SpecialName]
  public static int Mod(ushort x, short y) => Int32Ops.Mod((int) x, (int) y);

  [SpecialName]
  public static int Mod(short x, ushort y) => Int32Ops.Mod((int) x, (int) y);

  [SpecialName]
  public static object Power(ushort x, ushort y) => Int32Ops.Power((int) x, (int) y);

  [SpecialName]
  public static object Power(ushort x, short y) => Int32Ops.Power((int) x, (int) y);

  [SpecialName]
  public static object Power(short x, ushort y) => Int32Ops.Power((int) x, (int) y);

  [SpecialName]
  public static object LeftShift(ushort x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static ushort RightShift(ushort x, [NotNull] BigInteger y)
  {
    return (ushort) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static object LeftShift(ushort x, int y) => Int32Ops.LeftShift((int) x, y);

  [SpecialName]
  public static ushort RightShift(ushort x, int y) => (ushort) Int32Ops.RightShift((int) x, y);

  [SpecialName]
  public static ushort BitwiseAnd(ushort x, ushort y) => (ushort) ((uint) x & (uint) y);

  [SpecialName]
  public static int BitwiseAnd(ushort x, short y) => Int32Ops.BitwiseAnd((int) x, (int) y);

  [SpecialName]
  public static int BitwiseAnd(short x, ushort y) => Int32Ops.BitwiseAnd((int) x, (int) y);

  [SpecialName]
  public static ushort BitwiseOr(ushort x, ushort y) => (ushort) ((uint) x | (uint) y);

  [SpecialName]
  public static int BitwiseOr(ushort x, short y) => Int32Ops.BitwiseOr((int) x, (int) y);

  [SpecialName]
  public static int BitwiseOr(short x, ushort y) => Int32Ops.BitwiseOr((int) x, (int) y);

  [SpecialName]
  public static ushort ExclusiveOr(ushort x, ushort y) => (ushort) ((uint) x ^ (uint) y);

  [SpecialName]
  public static int ExclusiveOr(ushort x, short y) => Int32Ops.ExclusiveOr((int) x, (int) y);

  [SpecialName]
  public static int ExclusiveOr(short x, ushort y) => Int32Ops.ExclusiveOr((int) x, (int) y);

  [SpecialName]
  public static int Compare(ushort x, ushort y)
  {
    if ((int) x == (int) y)
      return 0;
    return (int) x <= (int) y ? -1 : 1;
  }

  [SpecialName]
  public static int Compare(ushort x, short y) => Int32Ops.Compare((int) x, (int) y);

  [SpecialName]
  public static int Compare(short x, ushort y) => Int32Ops.Compare((int) x, (int) y);

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(ushort x)
  {
    return x <= (ushort) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(ushort x)
  {
    return (ushort) 0 <= x && x <= (ushort) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(ushort x)
  {
    return x <= (ushort) short.MaxValue ? (short) x : throw Converter.CannotConvertOverflow("Int16", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(ushort x) => (int) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(ushort x) => (uint) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(ushort x) => (long) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(ushort x) => (ulong) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(ushort x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(ushort x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static ushort Getreal(ushort x) => x;

  [PropertyMethod]
  [SpecialName]
  public static ushort Getimag(ushort x) => 0;

  public static ushort conjugate(ushort x) => x;

  [PropertyMethod]
  [SpecialName]
  public static ushort Getnumerator(ushort x) => x;

  [PropertyMethod]
  [SpecialName]
  public static ushort Getdenominator(ushort x) => 1;

  public static string __hex__(ushort value) => BigIntegerOps.__hex__((BigInteger) value);

  public static int bit_length(ushort value) => MathUtils.BitLength((int) value);
}
