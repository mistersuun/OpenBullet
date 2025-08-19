// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Media.Animation.PennerEquations
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.Media.Animation;

public static class PennerEquations
{
  private static PennerEquation _backEaseIn;
  private static PennerEquation _backEaseInOut;
  private static PennerEquation _backEaseOut;
  private static PennerEquation _bounceEaseIn;
  private static PennerEquation _bounceEaseInOut;
  private static PennerEquation _bounceEaseOut;
  private static PennerEquation _circEaseIn;
  private static PennerEquation _circEaseInOut;
  private static PennerEquation _circEaseOut;
  private static PennerEquation _cubicEaseIn;
  private static PennerEquation _cubicEaseInOut;
  private static PennerEquation _cubicEaseOut;
  private static PennerEquation _elasticEaseIn;
  private static PennerEquation _elasticEaseInOut;
  private static PennerEquation _elasticEaseOut;
  private static PennerEquation _expoEaseIn;
  private static PennerEquation _expoEaseInOut;
  private static PennerEquation _expoEaseOut;
  private static PennerEquation _linear;
  private static PennerEquation _quadEaseIn;
  private static PennerEquation _quadEaseInOut;
  private static PennerEquation _quadEaseOut;
  private static PennerEquation _quartEaseIn;
  private static PennerEquation _quartEaseInOut;
  private static PennerEquation _quartEaseOut;
  private static PennerEquation _quintEaseIn;
  private static PennerEquation _quintEaseInOut;
  private static PennerEquation _quintEaseOut;
  private static PennerEquation _sineEaseIn;
  private static PennerEquation _sineEaseInOut;
  private static PennerEquation _sineEaseOut;

  public static PennerEquation BackEaseIn
  {
    get
    {
      if (PennerEquations._backEaseIn == null)
        PennerEquations._backEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.BackEaseInImpl));
      return PennerEquations._backEaseIn;
    }
  }

  public static PennerEquation BackEaseInOut
  {
    get
    {
      if (PennerEquations._backEaseInOut == null)
        PennerEquations._backEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.BackEaseInOutImpl));
      return PennerEquations._backEaseInOut;
    }
  }

  public static PennerEquation BackEaseOut
  {
    get
    {
      if (PennerEquations._backEaseOut == null)
        PennerEquations._backEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.BackEaseOutImpl));
      return PennerEquations._backEaseOut;
    }
  }

  public static PennerEquation BounceEaseIn
  {
    get
    {
      if (PennerEquations._bounceEaseIn == null)
        PennerEquations._bounceEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.BounceEaseInImpl));
      return PennerEquations._bounceEaseIn;
    }
  }

  public static PennerEquation BounceEaseInOut
  {
    get
    {
      if (PennerEquations._bounceEaseInOut == null)
        PennerEquations._bounceEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.BounceEaseInOutImpl));
      return PennerEquations._bounceEaseInOut;
    }
  }

  public static PennerEquation BounceEaseOut
  {
    get
    {
      if (PennerEquations._bounceEaseOut == null)
        PennerEquations._bounceEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.BounceEaseOutImpl));
      return PennerEquations._bounceEaseOut;
    }
  }

  public static PennerEquation CircEaseIn
  {
    get
    {
      if (PennerEquations._circEaseIn == null)
        PennerEquations._circEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.CircEaseInImpl));
      return PennerEquations._circEaseIn;
    }
  }

  public static PennerEquation CircEaseInOut
  {
    get
    {
      if (PennerEquations._circEaseInOut == null)
        PennerEquations._circEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.CircEaseInOutImpl));
      return PennerEquations._circEaseInOut;
    }
  }

  public static PennerEquation CircEaseOut
  {
    get
    {
      if (PennerEquations._circEaseOut == null)
        PennerEquations._circEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.CircEaseOutImpl));
      return PennerEquations._circEaseOut;
    }
  }

  public static PennerEquation CubicEaseIn
  {
    get
    {
      if (PennerEquations._cubicEaseIn == null)
        PennerEquations._cubicEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.CubicEaseInImpl));
      return PennerEquations._cubicEaseIn;
    }
  }

  public static PennerEquation CubicEaseInOut
  {
    get
    {
      if (PennerEquations._cubicEaseInOut == null)
        PennerEquations._cubicEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.CubicEaseInOutImpl));
      return PennerEquations._cubicEaseInOut;
    }
  }

  public static PennerEquation CubicEaseOut
  {
    get
    {
      if (PennerEquations._cubicEaseOut == null)
        PennerEquations._cubicEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.CubicEaseOutImpl));
      return PennerEquations._cubicEaseOut;
    }
  }

  public static PennerEquation ElasticEaseIn
  {
    get
    {
      if (PennerEquations._elasticEaseIn == null)
        PennerEquations._elasticEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.ElasticEaseInImpl));
      return PennerEquations._elasticEaseIn;
    }
  }

  public static PennerEquation ElasticEaseInOut
  {
    get
    {
      if (PennerEquations._elasticEaseInOut == null)
        PennerEquations._elasticEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.ElasticEaseInOutImpl));
      return PennerEquations._elasticEaseInOut;
    }
  }

  public static PennerEquation ElasticEaseOut
  {
    get
    {
      if (PennerEquations._elasticEaseOut == null)
        PennerEquations._elasticEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.ElasticEaseOutImpl));
      return PennerEquations._elasticEaseOut;
    }
  }

  public static PennerEquation ExpoEaseIn
  {
    get
    {
      if (PennerEquations._expoEaseIn == null)
        PennerEquations._expoEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.ExpoEaseInImpl));
      return PennerEquations._expoEaseIn;
    }
  }

  public static PennerEquation ExpoEaseInOut
  {
    get
    {
      if (PennerEquations._expoEaseInOut == null)
        PennerEquations._expoEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.ExpoEaseInOutImpl));
      return PennerEquations._expoEaseInOut;
    }
  }

  public static PennerEquation ExpoEaseOut
  {
    get
    {
      if (PennerEquations._expoEaseOut == null)
        PennerEquations._expoEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.ExpoEaseOutImpl));
      return PennerEquations._expoEaseOut;
    }
  }

  public static PennerEquation Linear
  {
    get
    {
      if (PennerEquations._linear == null)
        PennerEquations._linear = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.LinearImpl));
      return PennerEquations._linear;
    }
  }

  public static PennerEquation QuadEaseIn
  {
    get
    {
      if (PennerEquations._quadEaseIn == null)
        PennerEquations._quadEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuadEaseInImpl));
      return PennerEquations._quadEaseIn;
    }
  }

  public static PennerEquation QuadEaseInOut
  {
    get
    {
      if (PennerEquations._quadEaseInOut == null)
        PennerEquations._quadEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuadEaseInOutImpl));
      return PennerEquations._quadEaseInOut;
    }
  }

  public static PennerEquation QuadEaseOut
  {
    get
    {
      if (PennerEquations._quadEaseOut == null)
        PennerEquations._quadEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuadEaseOutImpl));
      return PennerEquations._quadEaseOut;
    }
  }

  public static PennerEquation QuartEaseIn
  {
    get
    {
      if (PennerEquations._quartEaseIn == null)
        PennerEquations._quartEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuartEaseInImpl));
      return PennerEquations._quartEaseIn;
    }
  }

  public static PennerEquation QuartEaseInOut
  {
    get
    {
      if (PennerEquations._quartEaseInOut == null)
        PennerEquations._quartEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuartEaseInOutImpl));
      return PennerEquations._quartEaseInOut;
    }
  }

  public static PennerEquation QuartEaseOut
  {
    get
    {
      if (PennerEquations._quartEaseOut == null)
        PennerEquations._quartEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuartEaseOutImpl));
      return PennerEquations._quartEaseOut;
    }
  }

  public static PennerEquation QuintEaseIn
  {
    get
    {
      if (PennerEquations._quintEaseIn == null)
        PennerEquations._quintEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuintEaseInImpl));
      return PennerEquations._quintEaseIn;
    }
  }

  public static PennerEquation QuintEaseInOut
  {
    get
    {
      if (PennerEquations._quintEaseInOut == null)
        PennerEquations._quintEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuintEaseInOutImpl));
      return PennerEquations._quintEaseInOut;
    }
  }

  public static PennerEquation QuintEaseOut
  {
    get
    {
      if (PennerEquations._quintEaseOut == null)
        PennerEquations._quintEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.QuintEaseOutImpl));
      return PennerEquations._quintEaseOut;
    }
  }

  public static PennerEquation SineEaseIn
  {
    get
    {
      if (PennerEquations._sineEaseIn == null)
        PennerEquations._sineEaseIn = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.SineEaseInImpl));
      return PennerEquations._sineEaseIn;
    }
  }

  public static PennerEquation SineEaseInOut
  {
    get
    {
      if (PennerEquations._sineEaseInOut == null)
        PennerEquations._sineEaseInOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.SineEaseInOutImpl));
      return PennerEquations._sineEaseInOut;
    }
  }

  public static PennerEquation SineEaseOut
  {
    get
    {
      if (PennerEquations._sineEaseOut == null)
        PennerEquations._sineEaseOut = new PennerEquation(new PennerEquation.PennerEquationDelegate(PennerEquations.SineEaseOutImpl));
      return PennerEquations._sineEaseOut;
    }
  }

  private static double BackEaseOutImpl(double t, double b, double c, double d)
  {
    return c * ((t = t / d - 1.0) * t * (2.70158 * t + 1.70158) + 1.0) + b;
  }

  private static double BackEaseInImpl(double t, double b, double c, double d)
  {
    return c * (t /= d) * t * (2.70158 * t - 1.70158) + b;
  }

  private static double BackEaseInOutImpl(double t, double b, double c, double d)
  {
    double num1 = 1.70158;
    double num2;
    double num3;
    return (t /= d / 2.0) < 1.0 ? c / 2.0 * (t * t * (((num2 = num1 * 1.525) + 1.0) * t - num2)) + b : c / 2.0 * ((t -= 2.0) * t * (((num3 = num1 * 1.525) + 1.0) * t + num3) + 2.0) + b;
  }

  private static double BounceEaseOutImpl(double t, double b, double c, double d)
  {
    if ((t /= d) < 4.0 / 11.0)
      return c * (121.0 / 16.0 * t * t) + b;
    if (t < 8.0 / 11.0)
      return c * (121.0 / 16.0 * (t -= 6.0 / 11.0) * t + 0.75) + b;
    return t < 10.0 / 11.0 ? c * (121.0 / 16.0 * (t -= 9.0 / 11.0) * t + 15.0 / 16.0) + b : c * (121.0 / 16.0 * (t -= 21.0 / 22.0) * t + 63.0 / 64.0) + b;
  }

  private static double BounceEaseInImpl(double t, double b, double c, double d)
  {
    return c - PennerEquations.BounceEaseOutImpl(d - t, 0.0, c, d) + b;
  }

  private static double BounceEaseInOutImpl(double t, double b, double c, double d)
  {
    return t < d / 2.0 ? PennerEquations.BounceEaseInImpl(t * 2.0, 0.0, c, d) * 0.5 + b : PennerEquations.BounceEaseOutImpl(t * 2.0 - d, 0.0, c, d) * 0.5 + c * 0.5 + b;
  }

  private static double CircEaseOutImpl(double t, double b, double c, double d)
  {
    return c * Math.Sqrt(1.0 - (t = t / d - 1.0) * t) + b;
  }

  private static double CircEaseInImpl(double t, double b, double c, double d)
  {
    return -c * (Math.Sqrt(1.0 - (t /= d) * t) - 1.0) + b;
  }

  private static double CircEaseInOutImpl(double t, double b, double c, double d)
  {
    return (t /= d / 2.0) < 1.0 ? -c / 2.0 * (Math.Sqrt(1.0 - t * t) - 1.0) + b : c / 2.0 * (Math.Sqrt(1.0 - (t -= 2.0) * t) + 1.0) + b;
  }

  private static double CubicEaseOutImpl(double t, double b, double c, double d)
  {
    return c * ((t = t / d - 1.0) * t * t + 1.0) + b;
  }

  private static double CubicEaseInImpl(double t, double b, double c, double d)
  {
    return c * (t /= d) * t * t + b;
  }

  private static double CubicEaseInOutImpl(double t, double b, double c, double d)
  {
    return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t * t + b : c / 2.0 * ((t -= 2.0) * t * t + 2.0) + b;
  }

  private static double ElasticEaseOutImpl(double t, double b, double c, double d)
  {
    if ((t /= d) == 1.0)
      return b + c;
    double num1 = d * 0.3;
    double num2 = num1 / 4.0;
    return c * Math.Pow(2.0, -10.0 * t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1) + c + b;
  }

  private static double ElasticEaseInImpl(double t, double b, double c, double d)
  {
    if ((t /= d) == 1.0)
      return b + c;
    double num1 = d * 0.3;
    double num2 = num1 / 4.0;
    return -(c * Math.Pow(2.0, 10.0 * --t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1)) + b;
  }

  private static double ElasticEaseInOutImpl(double t, double b, double c, double d)
  {
    if ((t /= d / 2.0) == 2.0)
      return b + c;
    double num1 = d * (9.0 / 20.0);
    double num2 = num1 / 4.0;
    return t < 1.0 ? -0.5 * (c * Math.Pow(2.0, 10.0 * --t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1)) + b : c * Math.Pow(2.0, -10.0 * --t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1) * 0.5 + c + b;
  }

  private static double ExpoEaseOutImpl(double t, double b, double c, double d)
  {
    return t != d ? c * (-Math.Pow(2.0, -10.0 * t / d) + 1.0) + b : b + c;
  }

  private static double ExpoEaseInImpl(double t, double b, double c, double d)
  {
    return t != 0.0 ? c * Math.Pow(2.0, 10.0 * (t / d - 1.0)) + b : b;
  }

  private static double ExpoEaseInOutImpl(double t, double b, double c, double d)
  {
    if (t == 0.0)
      return b;
    if (t == d)
      return b + c;
    return (t /= d / 2.0) < 1.0 ? c / 2.0 * Math.Pow(2.0, 10.0 * (t - 1.0)) + b : c / 2.0 * (-Math.Pow(2.0, -10.0 * --t) + 2.0) + b;
  }

  private static double LinearImpl(double t, double b, double c, double d) => c * (t / d) + b;

  private static double QuadEaseOutImpl(double t, double b, double c, double d)
  {
    return -c * (t /= d) * (t - 2.0) + b;
  }

  private static double QuadEaseInImpl(double t, double b, double c, double d)
  {
    return c * (t /= d) * t + b;
  }

  private static double QuadEaseInOutImpl(double t, double b, double c, double d)
  {
    return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t + b : -c / 2.0 * (--t * (t - 2.0) - 1.0) + b;
  }

  private static double QuartEaseOutImpl(double t, double b, double c, double d)
  {
    return -c * ((t = t / d - 1.0) * t * t * t - 1.0) + b;
  }

  private static double QuartEaseInImpl(double t, double b, double c, double d)
  {
    return c * (t /= d) * t * t * t + b;
  }

  private static double QuartEaseInOutImpl(double t, double b, double c, double d)
  {
    return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t * t * t + b : -c / 2.0 * ((t -= 2.0) * t * t * t - 2.0) + b;
  }

  private static double QuintEaseOutImpl(double t, double b, double c, double d)
  {
    return c * ((t = t / d - 1.0) * t * t * t * t + 1.0) + b;
  }

  private static double QuintEaseInImpl(double t, double b, double c, double d)
  {
    return c * (t /= d) * t * t * t * t + b;
  }

  private static double QuintEaseInOutImpl(double t, double b, double c, double d)
  {
    return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t * t * t * t + b : c / 2.0 * ((t -= 2.0) * t * t * t * t + 2.0) + b;
  }

  private static double SineEaseOutImpl(double t, double b, double c, double d)
  {
    return c * Math.Sin(t / d * (Math.PI / 2.0)) + b;
  }

  private static double SineEaseInImpl(double t, double b, double c, double d)
  {
    return -c * Math.Cos(t / d * (Math.PI / 2.0)) + c + b;
  }

  private static double SineEaseInOutImpl(double t, double b, double c, double d)
  {
    return (t /= d / 2.0) < 1.0 ? c / 2.0 * Math.Sin(Math.PI * t / 2.0) + b : -c / 2.0 * (Math.Cos(Math.PI * --t / 2.0) - 2.0) + b;
  }
}
