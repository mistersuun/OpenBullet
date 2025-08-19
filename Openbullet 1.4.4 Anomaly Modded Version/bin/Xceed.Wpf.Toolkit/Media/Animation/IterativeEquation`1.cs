// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Media.Animation.IterativeEquation`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Xceed.Wpf.Toolkit.Media.Animation;

[TypeConverter(typeof (IterativeEquationConverter))]
public class IterativeEquation<T>
{
  private readonly IterativeAnimationEquationDelegate<T> _equation;

  public IterativeEquation(IterativeAnimationEquationDelegate<T> equation)
  {
    this._equation = equation;
  }

  internal IterativeEquation()
  {
  }

  public virtual T Evaluate(TimeSpan currentTime, T from, T to, TimeSpan duration)
  {
    return this._equation(currentTime, from, to, duration);
  }
}
