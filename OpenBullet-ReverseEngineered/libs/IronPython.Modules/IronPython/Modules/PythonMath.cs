// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonMath
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable disable
namespace IronPython.Modules;

public static class PythonMath
{
  public const string __doc__ = "Provides common mathematical functions.";
  public const double pi = 3.1415926535897931;
  public const double e = 2.7182818284590451;
  private const double degreesToRadians = 0.017453292519943295;
  private const int Bias = 1022;

  public static double degrees(double radians)
  {
    return PythonMath.Check(radians, radians / (Math.PI / 180.0));
  }

  public static double radians(double degrees)
  {
    return PythonMath.Check(degrees, degrees * (Math.PI / 180.0));
  }

  public static double fmod(double v, double w) => PythonMath.Check(v, w, v % w);

  private static double sum(List<double> partials)
  {
    int count = partials.Count;
    double num1 = 0.0;
    if (count == 0)
      return num1;
    double num2 = 0.0;
    while (count > 0)
    {
      double num3 = num1;
      double partial = partials[--count];
      num1 = num3 + partial;
      num2 = partial - (num1 - num3);
      if (num2 != 0.0)
        break;
    }
    if (count == 0 || (num2 >= 0.0 || partials[count - 1] >= 0.0) && (num2 <= 0.0 || partials[count - 1] <= 0.0))
      return num1;
    double num4 = num2 * 2.0;
    double num5 = num1 + num4;
    double num6 = num5 - num1;
    if (num4 == num6)
      num1 = num5;
    return num1;
  }

  public static double fsum(IEnumerable e)
  {
    List<double> partials = new List<double>();
    foreach (double num1 in e.Cast<object>().Select<object, double>((Func<object, double>) (o => Converter.ConvertToDouble(o))))
    {
      double num2 = num1;
      int index1 = 0;
      for (int index2 = 0; index2 < partials.Count; ++index2)
      {
        double num3 = partials[index2];
        if (Math.Abs(num2) < Math.Abs(num3))
        {
          double num4 = num2;
          num2 = num3;
          num3 = num4;
        }
        double num5 = num2 + num3;
        double num6 = num3 - (num5 - num2);
        if (num6 != 0.0)
          partials[index1++] = num6;
        num2 = num5;
      }
      partials.RemoveRange(index1, partials.Count - index1);
      partials.Add(num2);
    }
    return PythonMath.sum(partials);
  }

  public static PythonTuple frexp(double v)
  {
    if (double.IsInfinity(v) || double.IsNaN(v))
      return PythonTuple.MakeTuple((object) v, (object) 0.0);
    int e = 0;
    double m = 0.0;
    if (v == 0.0)
    {
      m = 0.0;
      e = 0;
    }
    else
    {
      byte[] bytes = BitConverter.GetBytes(v);
      if (!BitConverter.IsLittleEndian)
        throw new NotImplementedException();
      PythonMath.DecomposeLe(bytes, out m, out e);
    }
    return PythonTuple.MakeTuple((object) m, (object) e);
  }

  public static PythonTuple modf(double v)
  {
    if (double.IsInfinity(v))
      return PythonTuple.MakeTuple((object) 0.0, (object) v);
    double num = v % 1.0;
    v -= num;
    return PythonTuple.MakeTuple((object) num, (object) v);
  }

  public static double ldexp(double v, BigInteger w)
  {
    return v == 0.0 || double.IsInfinity(v) ? v : PythonMath.Check(v, v * Math.Pow(2.0, (double) w));
  }

  public static double hypot(double v, double w)
  {
    return double.IsInfinity(v) || double.IsInfinity(w) ? double.PositiveInfinity : PythonMath.Check(v, w, MathUtils.Hypot(v, w));
  }

  public static double pow(double v, double exp)
  {
    if (v == 1.0 || exp == 0.0)
      return 1.0;
    if (double.IsNaN(v) || double.IsNaN(exp))
      return double.NaN;
    if (v == 0.0)
    {
      if (exp > 0.0)
        return 0.0;
      throw PythonOps.ValueError("math domain error");
    }
    if (double.IsPositiveInfinity(exp))
    {
      if (v > 1.0 || v < -1.0)
        return double.PositiveInfinity;
      return v == -1.0 ? 1.0 : 0.0;
    }
    if (!double.IsNegativeInfinity(exp))
      return PythonMath.Check(v, exp, Math.Pow(v, exp));
    if (v > 1.0 || v < -1.0)
      return 0.0;
    return v == -1.0 ? 1.0 : double.PositiveInfinity;
  }

  public static double log(double v0)
  {
    return v0 > 0.0 ? PythonMath.Check(v0, Math.Log(v0)) : throw PythonOps.ValueError("math domain error");
  }

  public static double log(double v0, double v1)
  {
    if (v0 <= 0.0 || v1 == 0.0)
      throw PythonOps.ValueError("math domain error");
    if (v1 == 1.0)
      throw PythonOps.ZeroDivisionError("float division");
    return v1 == double.PositiveInfinity ? 0.0 : PythonMath.Check(Math.Log(v0, v1));
  }

  public static double log(BigInteger value)
  {
    return value.Sign > 0 ? value.Log() : throw PythonOps.ValueError("math domain error");
  }

  public static double log(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.log(result) : PythonMath.log(Converter.ConvertToBigInteger(value));
  }

  public static double log(BigInteger value, double newBase)
  {
    if (newBase <= 0.0 || value <= 0L)
      throw PythonOps.ValueError("math domain error");
    if (newBase == 1.0)
      throw PythonOps.ZeroDivisionError("float division");
    return newBase == double.PositiveInfinity ? 0.0 : PythonMath.Check(value.Log(newBase));
  }

  public static double log(object value, double newBase)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.log(result, newBase) : PythonMath.log(Converter.ConvertToBigInteger(value), newBase);
  }

  public static double log10(double v0)
  {
    return v0 > 0.0 ? PythonMath.Check(v0, Math.Log10(v0)) : throw PythonOps.ValueError("math domain error");
  }

  public static double log10(BigInteger value)
  {
    return value.Sign > 0 ? value.Log10() : throw PythonOps.ValueError("math domain error");
  }

  public static double log10(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.log10(result) : PythonMath.log10(Converter.ConvertToBigInteger(value));
  }

  public static double log1p(double v0)
  {
    if (double.IsPositiveInfinity(v0))
      return double.PositiveInfinity;
    double v0_1 = v0 + 1.0;
    return v0_1 == 1.0 ? v0 : PythonMath.log(v0_1) * v0 / (v0_1 - 1.0);
  }

  public static double log1p(BigInteger value) => PythonMath.log(value + BigInteger.One);

  public static double log1p(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.log1p(result) : PythonMath.log1p(Converter.ConvertToBigInteger(value));
  }

  public static double expm1(double v0)
  {
    return PythonMath.Check(v0, Math.Tanh(v0 / 2.0) * (Math.Exp(v0) + 1.0));
  }

  public static double asinh(double v0)
  {
    if (v0 == 0.0 || double.IsInfinity(v0))
      return v0;
    return Math.Abs(v0) > 1.0 ? (double) Math.Sign(v0) * (Math.Log(Math.Abs(v0)) + Math.Log(1.0 + MathUtils.Hypot(1.0, 1.0 / v0))) : Math.Log(v0 + MathUtils.Hypot(1.0, v0));
  }

  public static double asinh(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.asinh(result) : PythonMath.asinh((object) Converter.ConvertToBigInteger(value));
  }

  public static double acosh(double v0)
  {
    if (v0 < 1.0)
      throw PythonOps.ValueError("math domain error");
    if (double.IsPositiveInfinity(v0))
      return double.PositiveInfinity;
    double d = Math.Sqrt(v0 + 1.0);
    return Math.Log(d) + Math.Log(v0 / d + Math.Sqrt(v0 - 1.0));
  }

  public static double acosh(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.acosh(result) : PythonMath.acosh((object) Converter.ConvertToBigInteger(value));
  }

  public static double atanh(double v0)
  {
    if (v0 >= 1.0 || v0 <= -1.0)
      throw PythonOps.ValueError("math domain error");
    return v0 == 0.0 ? v0 : Math.Log((1.0 + v0) / (1.0 - v0)) * 0.5;
  }

  public static double atanh(BigInteger value)
  {
    if (value == 0L)
      return 0.0;
    throw PythonOps.ValueError("math domain error");
  }

  public static double atanh(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.atanh(result) : PythonMath.atanh(Converter.ConvertToBigInteger(value));
  }

  public static double atan2(double v0, double v1)
  {
    if (double.IsNaN(v0) || double.IsNaN(v1))
      return double.NaN;
    if (double.IsInfinity(v0))
    {
      if (double.IsPositiveInfinity(v1))
        return Math.PI / 4.0 * (double) Math.Sign(v0);
      return double.IsNegativeInfinity(v1) ? 3.0 * Math.PI / 4.0 * (double) Math.Sign(v0) : Math.PI / 2.0 * (double) Math.Sign(v0);
    }
    if (!double.IsInfinity(v1))
      return Math.Atan2(v0, v1);
    return v1 <= 0.0 ? Math.PI * (double) DoubleOps.Sign(v0) : 0.0;
  }

  public static double erf(double v0) => MathUtils.Erf(v0);

  public static double erfc(double v0) => MathUtils.ErfComplement(v0);

  public static object factorial(double v0)
  {
    if (v0 % 1.0 != 0.0)
      throw PythonOps.ValueError("factorial() only accepts integral values");
    if (v0 < 0.0)
      throw PythonOps.ValueError("factorial() not defined for negative values");
    BigInteger bigInteger1 = (BigInteger) 1;
    BigInteger bigInteger2 = (BigInteger) v0;
    while (bigInteger2 > BigInteger.One)
    {
      bigInteger1 *= bigInteger2;
      bigInteger2 -= BigInteger.One;
    }
    return bigInteger1 > (long) int.MaxValue ? (object) bigInteger1 : (object) (int) bigInteger1;
  }

  public static object factorial(BigInteger value)
  {
    if (value < 0L)
      throw PythonOps.ValueError("factorial() not defined for negative values");
    BigInteger bigInteger1 = (BigInteger) 1;
    BigInteger bigInteger2 = value;
    while (bigInteger2 > BigInteger.One)
    {
      bigInteger1 *= bigInteger2;
      bigInteger2 -= BigInteger.One;
    }
    return bigInteger1 > (long) int.MaxValue ? (object) bigInteger1 : (object) (int) bigInteger1;
  }

  public static object factorial(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) ? PythonMath.factorial(result) : PythonMath.factorial(Converter.ConvertToBigInteger(value));
  }

  public static double gamma(double v0) => PythonMath.Check(v0, MathUtils.Gamma(v0));

  public static double lgamma(double v0) => PythonMath.Check(v0, MathUtils.LogGamma(v0));

  public static object trunc(CodeContext context, object value)
  {
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "__trunc__", out ret))
      return PythonOps.CallWithContext(context, ret);
    throw PythonOps.AttributeError("__trunc__");
  }

  public static bool isinf(double v0) => double.IsInfinity(v0);

  public static bool isinf(BigInteger value) => false;

  public static bool isinf(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) && PythonMath.isinf(result);
  }

  public static bool isnan(double v0) => double.IsNaN(v0);

  public static bool isnan(BigInteger value) => false;

  public static bool isnan(object value)
  {
    double result;
    return Converter.TryConvertToDouble(value, out result) && PythonMath.isnan(result);
  }

  public static double copysign(double x, double y) => DoubleOps.CopySign(x, y);

  public static double copysign(object x, object y)
  {
    double result1;
    double result2;
    if (!Converter.TryConvertToDouble(x, out result1) || !Converter.TryConvertToDouble(y, out result2))
      throw PythonOps.TypeError("TypeError: a float is required");
    return DoubleOps.CopySign(result1, result2);
  }

  private static void SetExponentLe(byte[] v, int exp)
  {
    exp += 1022;
    ushort e = (ushort) ((int) PythonMath.LdExponentLe(v) & 32783 | exp << 4);
    PythonMath.StExponentLe(v, e);
  }

  private static int IntExponentLe(byte[] v)
  {
    return (((int) PythonMath.LdExponentLe(v) & 32752) >> 4) - 1022;
  }

  private static ushort LdExponentLe(byte[] v) => (ushort) ((uint) v[6] | (uint) v[7] << 8);

  private static long LdMantissaLe(byte[] v)
  {
    return (long) ((int) v[0] | (int) v[1] << 8 | (int) v[2] << 16 /*0x10*/ | (int) v[3] << 24 | (int) v[4] | (int) v[5] << 8 | ((int) v[6] & 15) << 16 /*0x10*/);
  }

  private static void StExponentLe(byte[] v, ushort e)
  {
    v[6] = (byte) e;
    v[7] = (byte) ((uint) e >> 8);
  }

  private static bool IsDenormalizedLe(byte[] v)
  {
    int num1 = (int) PythonMath.LdExponentLe(v);
    long num2 = PythonMath.LdMantissaLe(v);
    return (num1 & 32752) == 0 && num2 != 0L;
  }

  private static void DecomposeLe(byte[] v, out double m, out int e)
  {
    if (PythonMath.IsDenormalizedLe(v))
    {
      m = BitConverter.ToDouble(v, 0);
      m *= Math.Pow(2.0, 1022.0);
      v = BitConverter.GetBytes(m);
      e = PythonMath.IntExponentLe(v) - 1022;
    }
    else
      e = PythonMath.IntExponentLe(v);
    PythonMath.SetExponentLe(v, 0);
    m = BitConverter.ToDouble(v, 0);
  }

  private static double Check(double v) => PythonOps.CheckMath(v);

  private static double Check(double input, double output) => PythonOps.CheckMath(input, output);

  private static double Check(double in0, double in1, double output)
  {
    return PythonOps.CheckMath(in0, in1, output);
  }

  public static double cos(double v0) => PythonMath.Check(v0, Math.Cos(v0));

  public static double sin(double v0) => PythonMath.Check(v0, Math.Sin(v0));

  public static double tan(double v0) => PythonMath.Check(v0, Math.Tan(v0));

  public static double cosh(double v0) => PythonMath.Check(v0, Math.Cosh(v0));

  public static double sinh(double v0) => PythonMath.Check(v0, Math.Sinh(v0));

  public static double tanh(double v0) => PythonMath.Check(v0, Math.Tanh(v0));

  public static double acos(double v0) => PythonMath.Check(v0, Math.Acos(v0));

  public static double asin(double v0) => PythonMath.Check(v0, Math.Asin(v0));

  public static double atan(double v0) => PythonMath.Check(v0, Math.Atan(v0));

  public static double floor(double v0) => PythonMath.Check(v0, Math.Floor(v0));

  public static double ceil(double v0) => PythonMath.Check(v0, Math.Ceiling(v0));

  public static double fabs(double v0) => PythonMath.Check(v0, Math.Abs(v0));

  public static double sqrt(double v0) => PythonMath.Check(v0, Math.Sqrt(v0));

  public static double exp(double v0) => PythonMath.Check(v0, Math.Exp(v0));
}
