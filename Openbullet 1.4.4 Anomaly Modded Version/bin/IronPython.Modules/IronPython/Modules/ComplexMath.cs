// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.ComplexMath
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Utils;
using System;
using System.Numerics;

#nullable disable
namespace IronPython.Modules;

public class ComplexMath
{
  public const double pi = 3.1415926535897931;
  public const double e = 2.7182818284590451;
  public const string __doc__ = "Provides access to functions for operating on complex numbers";

  public static Complex cos(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (double.IsNaN(complexNum.Imaginary()))
      return new Complex(double.NaN, double.NaN);
    return !double.IsInfinity(complexNum.Real) ? new Complex(Math.Cos(complexNum.Real) * Math.Cosh(complexNum.Imaginary()), -(Math.Sin(complexNum.Real) * Math.Sinh(complexNum.Imaginary()))) : throw PythonOps.ValueError("math domain error");
  }

  public static Complex sin(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (double.IsNaN(complexNum.Imaginary()))
      return new Complex(double.NaN, double.NaN);
    return !double.IsInfinity(complexNum.Real) ? new Complex(Math.Sin(complexNum.Real) * Math.Cosh(complexNum.Imaginary()), Math.Cos(complexNum.Real) * Math.Sinh(complexNum.Imaginary())) : throw PythonOps.ValueError("math domain error");
  }

  public static Complex tan(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (double.IsPositiveInfinity(complexNum.Imaginary()))
      return Complex.ImaginaryOne;
    return double.IsNegativeInfinity(complexNum.Imaginary()) ? new Complex(0.0, -1.0) : ComplexMath.sin((object) complexNum) / ComplexMath.cos((object) complexNum);
  }

  public static Complex cosh(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (double.IsNaN(complexNum.Real))
      return new Complex(double.NaN, double.NaN);
    return !double.IsInfinity(complexNum.Imaginary()) ? new Complex(Math.Cosh(complexNum.Real) * Math.Cos(complexNum.Imaginary()), Math.Sinh(complexNum.Real) * Math.Sin(complexNum.Imaginary())) : throw PythonOps.ValueError("math domain error");
  }

  public static Complex sinh(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (double.IsNaN(complexNum.Real))
      return new Complex(double.NaN, double.NaN);
    return !double.IsInfinity(complexNum.Imaginary()) ? new Complex(Math.Sinh(complexNum.Real) * Math.Cos(complexNum.Imaginary()), Math.Cosh(complexNum.Real) * Math.Sin(complexNum.Imaginary())) : throw PythonOps.ValueError("math domain error");
  }

  public static Complex tanh(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (double.IsPositiveInfinity(complexNum.Real))
      return Complex.One;
    return double.IsNegativeInfinity(complexNum.Real) ? new Complex(-1.0, 0.0) : ComplexMath.sinh((object) complexNum) / ComplexMath.cosh((object) complexNum);
  }

  public static Complex acos(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    double num1 = MathUtils.Hypot(complexNum.Real + 1.0, complexNum.Imaginary());
    double num2 = MathUtils.Hypot(complexNum.Real - 1.0, complexNum.Imaginary());
    double num3 = 0.5 * (num1 + num2);
    double real = Math.Acos(0.5 * (num1 - num2));
    double num4 = Math.Log(num3 + Math.Sqrt(num3 + 1.0) * Math.Sqrt(num3 - 1.0));
    double imaginary = complexNum.Imaginary() >= 0.0 ? num4 : -num4;
    return new Complex(real, imaginary);
  }

  public static Complex asin(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    double num1 = MathUtils.Hypot(complexNum.Real + 1.0, complexNum.Imaginary());
    double num2 = MathUtils.Hypot(complexNum.Real - 1.0, complexNum.Imaginary());
    double num3 = 0.5 * (num1 + num2);
    double real = Math.Asin(0.5 * (num1 - num2));
    double num4 = Math.Log(num3 + Math.Sqrt(num3 + 1.0) * Math.Sqrt(num3 - 1.0));
    double imaginary = complexNum.Imaginary() >= 0.0 ? num4 : -num4;
    return new Complex(real, imaginary);
  }

  public static Complex atan(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    Complex imaginaryOne = Complex.ImaginaryOne;
    return imaginaryOne * (Complex) 0.5 * (ComplexMath.log((object) (imaginaryOne + complexNum)) - ComplexMath.log((object) (imaginaryOne - complexNum)));
  }

  public static Complex acosh(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    return ComplexMath.log((object) (complexNum + ComplexMath.sqrt((object) (complexNum + (Complex) 1)) * ComplexMath.sqrt((object) (complexNum - (Complex) 1))));
  }

  public static Complex asinh(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (complexNum.IsZero())
      return MathUtils.MakeImaginary(complexNum.Imaginary());
    Complex complex = (Complex) 1 / complexNum;
    return ComplexMath.log((object) complexNum) + ComplexMath.log((object) ((Complex) 1 + ComplexMath.sqrt((object) (complex * complex + (Complex) 1))));
  }

  public static Complex atanh(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    return (ComplexMath.log((object) ((Complex) 1 + complexNum)) - ComplexMath.log((object) ((Complex) 1 - complexNum))) * (Complex) 0.5;
  }

  public static Complex log(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    double d = !complexNum.IsZero() ? complexNum.Abs() : throw PythonOps.ValueError("math domain error");
    double angle = ComplexMath.GetAngle(complexNum);
    return new Complex(Math.Log(d), angle);
  }

  public static Complex log(object x, object logBase)
  {
    return ComplexMath.log(x) / ComplexMath.log(logBase);
  }

  public static Complex log10(object x) => ComplexMath.log(x, (object) 10);

  public static Complex exp(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (complexNum.Imaginary() == 0.0)
    {
      if (double.IsPositiveInfinity(complexNum.Real))
        return new Complex(double.PositiveInfinity, 0.0);
      double num = Math.Exp(complexNum.Real);
      return !double.IsInfinity(num) ? new Complex(num, 0.0) : throw PythonOps.OverflowError("math range error");
    }
    if (double.IsNegativeInfinity(complexNum.Real))
      return Complex.Zero;
    if (double.IsNaN(complexNum.Real))
      return new Complex(double.NaN, double.NaN);
    if (double.IsNaN(complexNum.Imaginary()))
      return new Complex(double.IsInfinity(complexNum.Real) ? double.PositiveInfinity : double.NaN, double.NaN);
    double d1 = !double.IsInfinity(complexNum.Imaginary()) ? Math.Cos(complexNum.Imaginary()) : throw PythonOps.ValueError("math domain error");
    double num1 = d1 <= 0.0 ? (d1 >= 0.0 ? 0.0 : -Math.Exp(complexNum.Real + Math.Log(-d1))) : Math.Exp(complexNum.Real + Math.Log(d1));
    double d2 = Math.Sin(complexNum.Imaginary());
    double num2 = d2 <= 0.0 ? (d2 >= 0.0 ? 0.0 : -Math.Exp(complexNum.Real + Math.Log(-d2))) : Math.Exp(complexNum.Real + Math.Log(d2));
    if ((double.IsInfinity(num1) || double.IsInfinity(num2)) && !double.IsInfinity(complexNum.Real))
      throw PythonOps.OverflowError("math range error");
    return new Complex(num1, num2);
  }

  public static Complex sqrt(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    if (complexNum.Imaginary() == 0.0)
      return complexNum.Real >= 0.0 ? MathUtils.MakeReal(Math.Sqrt(complexNum.Real)) : MathUtils.MakeImaginary(Math.Sqrt(-complexNum.Real));
    double num = complexNum.Abs() + complexNum.Real;
    return new Complex(Math.Sqrt(0.5 * num), complexNum.Imaginary() / Math.Sqrt(2.0 * num));
  }

  public static double phase(object x) => ComplexMath.GetAngle(ComplexMath.GetComplexNum(x));

  public static PythonTuple polar(object x)
  {
    Complex complexNum = ComplexMath.GetComplexNum(x);
    double[] o = new double[2]
    {
      complexNum.Abs(),
      ComplexMath.GetAngle(complexNum)
    };
    return !double.IsInfinity(o[0]) || ComplexMath.IsInfinity(complexNum) ? new PythonTuple((object) o) : throw PythonOps.OverflowError("math range error");
  }

  public static Complex rect(double r, double theta)
  {
    if (r == 0.0)
      return Complex.Zero;
    if (theta == 0.0)
      return new Complex(r, 0.0);
    if (double.IsNaN(r))
      return new Complex(double.NaN, double.NaN);
    if (double.IsNaN(theta))
      return new Complex(double.IsInfinity(r) ? double.PositiveInfinity : double.NaN, double.NaN);
    return !double.IsInfinity(theta) ? new Complex(r * Math.Cos(theta), r * Math.Sin(theta)) : throw PythonOps.ValueError("math domain error");
  }

  public static bool isinf(object x) => ComplexMath.IsInfinity(ComplexMath.GetComplexNum(x));

  public static bool isnan(object x) => ComplexMath.IsNaN(ComplexMath.GetComplexNum(x));

  private static bool IsInfinity(Complex num)
  {
    return double.IsInfinity(num.Real) || double.IsInfinity(num.Imaginary());
  }

  private static bool IsNaN(Complex num) => double.IsNaN(num.Real) || double.IsNaN(num.Imaginary());

  private static double GetAngle(Complex num)
  {
    if (ComplexMath.IsNaN(num))
      return double.NaN;
    if (double.IsPositiveInfinity(num.Real))
    {
      if (double.IsPositiveInfinity(num.Imaginary()))
        return Math.PI / 4.0;
      return double.IsNegativeInfinity(num.Imaginary()) ? -1.0 * Math.PI / 4.0 : 0.0;
    }
    if (double.IsNegativeInfinity(num.Real))
    {
      if (double.IsPositiveInfinity(num.Imaginary()))
        return 3.0 * Math.PI / 4.0;
      return double.IsNegativeInfinity(num.Imaginary()) ? -3.0 * Math.PI / 4.0 : (double) DoubleOps.Sign(num.Imaginary()) * Math.PI;
    }
    if (num.Real != 0.0)
      return Math.Atan2(num.Imaginary(), num.Real);
    return num.Imaginary() != 0.0 ? Math.PI / 2.0 * (double) Math.Sign(num.Imaginary()) : (DoubleOps.IsPositiveZero(num.Real) ? 0.0 : Math.PI) * (double) DoubleOps.Sign(num.Imaginary());
  }

  private static Complex GetComplexNum(object num)
  {
    return num != null ? Converter.ConvertToComplex(num) : throw new NullReferenceException("The input was null");
  }
}
