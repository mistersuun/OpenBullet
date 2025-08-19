// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.SingleOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class SingleOps
{
  [SpecialName]
  public static bool LessThan(float x, float y) => (double) x < (double) y;

  [SpecialName]
  public static bool LessThanOrEqual(float x, float y)
  {
    return (double) x == (double) y ? !float.IsNaN(x) : (double) x < (double) y;
  }

  [SpecialName]
  public static bool GreaterThan(float x, float y) => (double) x > (double) y;

  [SpecialName]
  public static bool GreaterThanOrEqual(float x, float y)
  {
    return (double) x == (double) y ? !float.IsNaN(x) : (double) x > (double) y;
  }

  [SpecialName]
  public static bool Equals(float x, float y)
  {
    return (double) x == (double) y ? !float.IsNaN(x) : (double) x == (double) y;
  }

  [SpecialName]
  public static bool NotEquals(float x, float y) => !SingleOps.Equals(x, y);

  [SpecialName]
  public static float Mod(float x, float y) => (float) DoubleOps.Mod((double) x, (double) y);

  [SpecialName]
  public static float Power(float x, float y) => (float) DoubleOps.Power((double) x, (double) y);

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.Single ? (object) 0.0f : cls.CreateInstance(context);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, object x)
  {
    if (cls != TypeCache.Single)
      return cls.CreateInstance(context, x);
    switch (x)
    {
      case string _:
        return SingleOps.ParseFloat((string) x);
      case Extensible<string> _:
        return SingleOps.ParseFloat(((Extensible<string>) x).Value);
      case char ch:
        return SingleOps.ParseFloat(ScriptingRuntimeHelpers.CharToString(ch));
      default:
        double result;
        if (Converter.TryConvertToDouble(x, out result))
          return (object) (float) result;
        object o = !(x is Complex) ? PythonOps.CallWithContext(context, PythonOps.GetBoundAttr(context, x, "__float__")) : throw PythonOps.TypeError("can't convert complex to Single; use abs(z)");
        return o is double num ? (object) (float) num : throw PythonOps.TypeError("__float__ returned non-float (type %s)", (object) DynamicHelpers.GetPythonType(o));
    }
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, IList<byte> s)
  {
    object o1;
    if (!(s is IPythonObject o2) || !PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, (object) o2, "__float__", out o1))
      o1 = SingleOps.ParseFloat(s.MakeString());
    if (!(o1 is double))
      throw PythonOps.TypeError("__float__ returned non-float (type %s)", (object) DynamicHelpers.GetPythonType(o1));
    return cls == TypeCache.Single ? (object) (float) o1 : cls.CreateInstance(context, (object) (float) o1);
  }

  private static object ParseFloat(string x)
  {
    try
    {
      return (object) (float) LiteralParser.ParseFloat(x);
    }
    catch (FormatException ex)
    {
      throw PythonOps.ValueError("invalid literal for Single(): {0}", (object) x);
    }
  }

  public static string __str__(CodeContext context, float x)
  {
    return new StringFormatter(context, "%.6g", (object) x)
    {
      _TrailingZeroAfterWholeFloat = true
    }.Format();
  }

  public static string __repr__(CodeContext context, float self)
  {
    return SingleOps.__str__(context, self);
  }

  public static string __format__(CodeContext context, float self, [NotNull] string formatSpec)
  {
    return DoubleOps.__format__(context, (double) self, formatSpec);
  }

  public static int __hash__(float x) => DoubleOps.__hash__((double) x);

  public static double __float__(float x) => (double) x;

  [SpecialName]
  public static float Plus(float x) => x;

  [SpecialName]
  public static float Negate(float x) => -x;

  [SpecialName]
  public static float Abs(float x) => Math.Abs(x);

  public static bool __nonzero__(float x) => (double) x != 0.0;

  public static object __trunc__(float x)
  {
    return (double) x >= 2147483648.0 || (double) x <= (double) int.MinValue ? (object) (BigInteger) x : (object) (int) x;
  }

  [SpecialName]
  public static float Add(float x, float y) => x + y;

  [SpecialName]
  public static float Subtract(float x, float y) => x - y;

  [SpecialName]
  public static float Multiply(float x, float y) => x * y;

  [SpecialName]
  public static float Divide(float x, float y) => SingleOps.TrueDivide(x, y);

  [SpecialName]
  public static float TrueDivide(float x, float y)
  {
    if ((double) y == 0.0)
      throw PythonOps.ZeroDivisionError();
    return x / y;
  }

  [SpecialName]
  public static float FloorDivide(float x, float y)
  {
    return (double) y != 0.0 ? (float) Math.Floor((double) x / (double) y) : throw PythonOps.ZeroDivisionError();
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(float x)
  {
    return (double) sbyte.MinValue <= (double) x && (double) x <= (double) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(float x)
  {
    return 0.0 <= (double) x && (double) x <= (double) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(float x)
  {
    return (double) short.MinValue <= (double) x && (double) x <= (double) short.MaxValue ? (short) x : throw Converter.CannotConvertOverflow("Int16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(float x)
  {
    return 0.0 <= (double) x && (double) x <= (double) ushort.MaxValue ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(float x)
  {
    return (double) int.MinValue <= (double) x && (double) x <= 2147483648.0 ? (int) x : throw Converter.CannotConvertOverflow("Int32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(float x)
  {
    return 0.0 <= (double) x && (double) x <= 4294967296.0 ? (uint) x : throw Converter.CannotConvertOverflow("UInt32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(float x)
  {
    return (double) long.MinValue <= (double) x && (double) x <= (double) long.MaxValue ? (long) x : throw Converter.CannotConvertOverflow("Int64", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(float x)
  {
    return 0.0 <= (double) x && (double) x <= 1.8446744073709552E+19 ? (ulong) x : throw Converter.CannotConvertOverflow("UInt64", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(float x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static float Getreal(float x) => x;

  [PropertyMethod]
  [SpecialName]
  public static float Getimag(float x) => 0.0f;

  public static float conjugate(float x) => x;
}
