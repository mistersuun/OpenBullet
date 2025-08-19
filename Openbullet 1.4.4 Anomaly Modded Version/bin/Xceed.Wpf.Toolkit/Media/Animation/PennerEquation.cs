// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Media.Animation.PennerEquation
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;

#nullable disable
namespace Xceed.Wpf.Toolkit.Media.Animation;

public class PennerEquation : IterativeEquation<double>
{
  private readonly PennerEquation.PennerEquationDelegate _pennerImpl;

  internal PennerEquation(PennerEquation.PennerEquationDelegate pennerImpl)
  {
    this._pennerImpl = pennerImpl;
  }

  public override double Evaluate(TimeSpan currentTime, double from, double to, TimeSpan duration)
  {
    return this._pennerImpl(currentTime.TotalSeconds, from, to - from, duration.TotalSeconds);
  }

  internal delegate double PennerEquationDelegate(double t, double b, double c, double d);
}
