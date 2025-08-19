// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ComplexOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class ComplexOps
{
  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.Complex ? (object) new Complex() : cls.CreateInstance(context);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, object real = null, object imag = null)
  {
    Complex self1 = new Complex();
    Complex self2 = self1;
    if (real == null && imag == null && cls == TypeCache.Complex)
      throw PythonOps.TypeError("argument must be a string or a number");
    if (imag != null)
    {
      if (real is string)
        throw PythonOps.TypeError("complex() can't take second arg if first is a string");
      self1 = !(imag is string) ? Converter.ConvertToComplex(imag) : throw PythonOps.TypeError("complex() second arg can't be a string");
    }
    switch (real)
    {
      case null:
        double real1 = self2.Real - self1.Imaginary();
        double imaginary = self2.Imaginary() + self1.Real;
        return cls == TypeCache.Complex ? (object) new Complex(real1, imaginary) : cls.CreateInstance(context, (object) real1, (object) imaginary);
      case string _:
        self2 = LiteralParser.ParseComplex((string) real);
        goto case null;
      case Extensible<string> _:
        self2 = LiteralParser.ParseComplex(((Extensible<string>) real).Value);
        goto case null;
      case Complex _:
        if (imag == null && cls == TypeCache.Complex)
          return real;
        self2 = (Complex) real;
        goto case null;
      default:
        self2 = Converter.ConvertToComplex(real);
        goto case null;
    }
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, double real)
  {
    return cls == TypeCache.Complex ? (object) new Complex(real, 0.0) : cls.CreateInstance(context, (object) real, (object) 0.0);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, double real, double imag)
  {
    return cls == TypeCache.Complex ? (object) new Complex(real, imag) : cls.CreateInstance(context, (object) real, (object) imag);
  }

  [PropertyMethod]
  [SpecialName]
  public static double Getreal(Complex self) => self.Real;

  [PropertyMethod]
  [SpecialName]
  public static double Getimag(Complex self) => self.Imaginary();

  [SpecialName]
  public static Complex Add(Complex x, Complex y) => x + y;

  [SpecialName]
  public static Complex Subtract(Complex x, Complex y) => x - y;

  [SpecialName]
  public static Complex Multiply(Complex x, Complex y) => x * y;

  [SpecialName]
  public static Complex Divide(Complex x, Complex y)
  {
    if (y.IsZero())
      throw new DivideByZeroException("complex division by zero");
    return x / y;
  }

  [SpecialName]
  public static Complex TrueDivide(Complex x, Complex y) => ComplexOps.Divide(x, y);

  [SpecialName]
  public static Complex op_Power(Complex x, Complex y)
  {
    if (x.IsZero())
    {
      if (y.Real < 0.0 || y.Imaginary() != 0.0)
        throw PythonOps.ZeroDivisionError("0.0 to a negative or complex power");
      return !y.IsZero() ? Complex.Zero : Complex.One;
    }
    if (y.Imaginary == 0.0)
    {
      int real = (int) y.Real;
      if (real >= 0 && y.Real == (double) real)
      {
        Complex one = Complex.One;
        if (real == 0)
          return one;
        Complex complex = x;
        for (; real != 0; real >>= 1)
        {
          if ((real & 1) != 0)
            one *= complex;
          complex *= complex;
        }
        return one;
      }
    }
    return x.Pow(y);
  }

  [PythonHidden(new PlatformID[] {})]
  public static Complex Power(Complex x, Complex y) => ComplexOps.op_Power(x, y);

  [SpecialName]
  public static Complex FloorDivide(CodeContext context, Complex x, Complex y)
  {
    return MathUtils.MakeReal(PythonOps.CheckMath(Math.Floor(ComplexOps.Divide(x, y).Real)));
  }

  [SpecialName]
  public static Complex Mod(CodeContext context, Complex x, Complex y)
  {
    Complex complex = ComplexOps.FloorDivide(context, x, y);
    return x - complex * y;
  }

  [SpecialName]
  public static PythonTuple DivMod(CodeContext context, Complex x, Complex y)
  {
    Complex complex = ComplexOps.FloorDivide(context, x, y);
    return PythonTuple.MakeTuple((object) complex, (object) (x - complex * y));
  }

  public static int __hash__(Complex x)
  {
    return x.Imaginary() == 0.0 ? DoubleOps.__hash__(x.Real) : x.GetHashCode();
  }

  public static bool __nonzero__(Complex x) => !x.IsZero();

  public static Complex conjugate(Complex x) => x.Conjugate();

  public static object __getnewargs__(CodeContext context, Complex self)
  {
    return (object) PythonTuple.MakeTuple(PythonOps.GetBoundAttr(context, (object) self, "real"), PythonOps.GetBoundAttr(context, (object) self, "imag"));
  }

  public static object __pos__(Complex x) => (object) x;

  public static object __coerce__(Complex x, object y)
  {
    Complex result;
    if (!Converter.TryConvertToComplex(y, out result))
      return (object) NotImplementedType.Value;
    if (double.IsInfinity(result.Real))
    {
      switch (y)
      {
        case BigInteger _:
        case Extensible<BigInteger> _:
          throw new OverflowException("long int too large to convert to float");
      }
    }
    return (object) PythonTuple.MakeTuple((object) x, (object) result);
  }

  public static string __str__(CodeContext context, Complex x)
  {
    if (x.Real == 0.0)
      return ComplexOps.FormatComplexValue(context, x.Imaginary()) + "j";
    if (x.Imaginary() < 0.0 || DoubleOps.IsNegativeZero(x.Imaginary()))
      return $"({ComplexOps.FormatComplexValue(context, x.Real)}{ComplexOps.FormatComplexValue(context, x.Imaginary())}j)";
    return $"({ComplexOps.FormatComplexValue(context, x.Real)}+{ComplexOps.FormatComplexValue(context, x.Imaginary())}j)";
  }

  public static string __repr__(CodeContext context, Complex x) => ComplexOps.__str__(context, x);

  public static double __float__(Complex self)
  {
    throw PythonOps.TypeError("can't convert complex to float; use abs(z)");
  }

  public static int __int__(Complex self)
  {
    throw PythonOps.TypeError(" can't convert complex to int; use int(abs(z))");
  }

  public static BigInteger __long__(Complex self)
  {
    throw PythonOps.TypeError("can't convert complex to long; use long(abs(z))");
  }

  private static string FormatComplexValue(CodeContext context, double x)
  {
    return new StringFormatter(context, "%.6g", (object) x).Format();
  }

  [SpecialName]
  public static double Abs(Complex x)
  {
    double d = x.Abs();
    if (double.IsInfinity(d) && !double.IsInfinity(x.Real) && !double.IsInfinity(x.Imaginary()))
      throw PythonOps.OverflowError("absolute value too large");
    return d;
  }

  [SpecialName]
  public static bool LessThan(Complex x, Complex y)
  {
    throw PythonOps.TypeError("complex is not an ordered type");
  }

  [SpecialName]
  public static bool LessThanOrEqual(Complex x, Complex y)
  {
    throw PythonOps.TypeError("complex is not an ordered type");
  }

  [SpecialName]
  public static bool GreaterThan(Complex x, Complex y)
  {
    throw PythonOps.TypeError("complex is not an ordered type");
  }

  [SpecialName]
  public static bool GreaterThanOrEqual(Complex x, Complex y)
  {
    throw PythonOps.TypeError("complex is not an ordered type");
  }
}
