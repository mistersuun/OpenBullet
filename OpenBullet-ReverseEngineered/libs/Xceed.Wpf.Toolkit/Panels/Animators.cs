// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.Animators
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using Xceed.Wpf.Toolkit.Media.Animation;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public static class Animators
{
  private static DoubleAnimator _backEaseIn;
  private static DoubleAnimator _backEaseInOut;
  private static DoubleAnimator _backEaseOut;
  private static DoubleAnimator _bounceEaseIn;
  private static DoubleAnimator _bounceEaseInOut;
  private static DoubleAnimator _bounceEaseOut;
  private static DoubleAnimator _circEaseIn;
  private static DoubleAnimator _circEaseInOut;
  private static DoubleAnimator _circEaseOut;
  private static DoubleAnimator _cubicEaseIn;
  private static DoubleAnimator _cubicEaseInOut;
  private static DoubleAnimator _cubicEaseOut;
  private static DoubleAnimator _elasticEaseIn;
  private static DoubleAnimator _elasticEaseInOut;
  private static DoubleAnimator _elasticEaseOut;
  private static DoubleAnimator _expoEaseIn;
  private static DoubleAnimator _expoEaseInOut;
  private static DoubleAnimator _expoEaseOut;
  private static DoubleAnimator _linear;
  private static DoubleAnimator _quadEaseIn;
  private static DoubleAnimator _quadEaseInOut;
  private static DoubleAnimator _quadEaseOut;
  private static DoubleAnimator _quartEaseIn;
  private static DoubleAnimator _quartEaseInOut;
  private static DoubleAnimator _quartEaseOut;
  private static DoubleAnimator _quintEaseIn;
  private static DoubleAnimator _quintEaseInOut;
  private static DoubleAnimator _quintEaseOut;
  private static DoubleAnimator _sineEaseIn;
  private static DoubleAnimator _sineEaseInOut;
  private static DoubleAnimator _sineEaseOut;

  public static DoubleAnimator BackEaseIn
  {
    get
    {
      if (Animators._backEaseIn == null)
        Animators._backEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.BackEaseIn);
      return Animators._backEaseIn;
    }
  }

  public static DoubleAnimator BackEaseInOut
  {
    get
    {
      if (Animators._backEaseInOut == null)
        Animators._backEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.BackEaseInOut);
      return Animators._backEaseInOut;
    }
  }

  public static DoubleAnimator BackEaseOut
  {
    get
    {
      if (Animators._backEaseOut == null)
        Animators._backEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.BackEaseOut);
      return Animators._backEaseOut;
    }
  }

  public static DoubleAnimator BounceEaseIn
  {
    get
    {
      if (Animators._bounceEaseIn == null)
        Animators._bounceEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.BounceEaseIn);
      return Animators._bounceEaseIn;
    }
  }

  public static DoubleAnimator BounceEaseInOut
  {
    get
    {
      if (Animators._bounceEaseInOut == null)
        Animators._bounceEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.BounceEaseInOut);
      return Animators._bounceEaseInOut;
    }
  }

  public static DoubleAnimator BounceEaseOut
  {
    get
    {
      if (Animators._bounceEaseOut == null)
        Animators._bounceEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.BounceEaseOut);
      return Animators._bounceEaseOut;
    }
  }

  public static DoubleAnimator CircEaseIn
  {
    get
    {
      if (Animators._circEaseIn == null)
        Animators._circEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.CircEaseIn);
      return Animators._circEaseIn;
    }
  }

  public static DoubleAnimator CircEaseInOut
  {
    get
    {
      if (Animators._circEaseInOut == null)
        Animators._circEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.CircEaseInOut);
      return Animators._circEaseInOut;
    }
  }

  public static DoubleAnimator CircEaseOut
  {
    get
    {
      if (Animators._circEaseOut == null)
        Animators._circEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.CircEaseOut);
      return Animators._circEaseOut;
    }
  }

  public static DoubleAnimator CubicEaseIn
  {
    get
    {
      if (Animators._cubicEaseIn == null)
        Animators._cubicEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.CubicEaseIn);
      return Animators._cubicEaseIn;
    }
  }

  public static DoubleAnimator CubicEaseInOut
  {
    get
    {
      if (Animators._cubicEaseInOut == null)
        Animators._cubicEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.CubicEaseInOut);
      return Animators._cubicEaseInOut;
    }
  }

  public static DoubleAnimator CubicEaseOut
  {
    get
    {
      if (Animators._cubicEaseOut == null)
        Animators._cubicEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.CubicEaseOut);
      return Animators._cubicEaseOut;
    }
  }

  public static DoubleAnimator ElasticEaseIn
  {
    get
    {
      if (Animators._elasticEaseIn == null)
        Animators._elasticEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.ElasticEaseIn);
      return Animators._elasticEaseIn;
    }
  }

  public static DoubleAnimator ElasticEaseInOut
  {
    get
    {
      if (Animators._elasticEaseInOut == null)
        Animators._elasticEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.ElasticEaseInOut);
      return Animators._elasticEaseInOut;
    }
  }

  public static DoubleAnimator ElasticEaseOut
  {
    get
    {
      if (Animators._elasticEaseOut == null)
        Animators._elasticEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.ElasticEaseOut);
      return Animators._elasticEaseOut;
    }
  }

  public static DoubleAnimator ExpoEaseIn
  {
    get
    {
      if (Animators._expoEaseIn == null)
        Animators._expoEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.ExpoEaseIn);
      return Animators._expoEaseIn;
    }
  }

  public static DoubleAnimator ExpoEaseInOut
  {
    get
    {
      if (Animators._expoEaseInOut == null)
        Animators._expoEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.ExpoEaseInOut);
      return Animators._expoEaseInOut;
    }
  }

  public static DoubleAnimator ExpoEaseOut
  {
    get
    {
      if (Animators._expoEaseOut == null)
        Animators._expoEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.ExpoEaseOut);
      return Animators._expoEaseOut;
    }
  }

  public static DoubleAnimator Linear
  {
    get
    {
      if (Animators._linear == null)
        Animators._linear = new DoubleAnimator((IterativeEquation<double>) PennerEquations.Linear);
      return Animators._linear;
    }
  }

  public static DoubleAnimator QuadEaseIn
  {
    get
    {
      if (Animators._quadEaseIn == null)
        Animators._quadEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuadEaseIn);
      return Animators._quadEaseIn;
    }
  }

  public static DoubleAnimator QuadEaseInOut
  {
    get
    {
      if (Animators._quadEaseInOut == null)
        Animators._quadEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuadEaseInOut);
      return Animators._quadEaseInOut;
    }
  }

  public static DoubleAnimator QuadEaseOut
  {
    get
    {
      if (Animators._quadEaseOut == null)
        Animators._quadEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuadEaseOut);
      return Animators._quadEaseOut;
    }
  }

  public static DoubleAnimator QuartEaseIn
  {
    get
    {
      if (Animators._quartEaseIn == null)
        Animators._quartEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuartEaseIn);
      return Animators._quartEaseIn;
    }
  }

  public static DoubleAnimator QuartEaseInOut
  {
    get
    {
      if (Animators._quartEaseInOut == null)
        Animators._quartEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuartEaseInOut);
      return Animators._quartEaseInOut;
    }
  }

  public static DoubleAnimator QuartEaseOut
  {
    get
    {
      if (Animators._quartEaseOut == null)
        Animators._quartEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuartEaseOut);
      return Animators._quartEaseOut;
    }
  }

  public static DoubleAnimator QuintEaseIn
  {
    get
    {
      if (Animators._quintEaseIn == null)
        Animators._quintEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuintEaseIn);
      return Animators._quintEaseIn;
    }
  }

  public static DoubleAnimator QuintEaseInOut
  {
    get
    {
      if (Animators._quintEaseInOut == null)
        Animators._quintEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuintEaseInOut);
      return Animators._quintEaseInOut;
    }
  }

  public static DoubleAnimator QuintEaseOut
  {
    get
    {
      if (Animators._quintEaseOut == null)
        Animators._quintEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.QuintEaseOut);
      return Animators._quintEaseOut;
    }
  }

  public static DoubleAnimator SineEaseIn
  {
    get
    {
      if (Animators._sineEaseIn == null)
        Animators._sineEaseIn = new DoubleAnimator((IterativeEquation<double>) PennerEquations.SineEaseIn);
      return Animators._sineEaseIn;
    }
  }

  public static DoubleAnimator SineEaseInOut
  {
    get
    {
      if (Animators._sineEaseInOut == null)
        Animators._sineEaseInOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.SineEaseInOut);
      return Animators._sineEaseInOut;
    }
  }

  public static DoubleAnimator SineEaseOut
  {
    get
    {
      if (Animators._sineEaseOut == null)
        Animators._sineEaseOut = new DoubleAnimator((IterativeEquation<double>) PennerEquations.SineEaseOut);
      return Animators._sineEaseOut;
    }
  }
}
