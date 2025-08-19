// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.DoubleAnimator
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using Xceed.Wpf.Toolkit.Media.Animation;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public class DoubleAnimator : IterativeAnimator
{
  private readonly IterativeEquation<double> _equation;

  public DoubleAnimator(IterativeEquation<double> equation) => this._equation = equation;

  public override Rect GetInitialChildPlacement(
    UIElement child,
    Rect currentPlacement,
    Rect targetPlacement,
    AnimationPanel activeLayout,
    ref AnimationRate animationRate,
    out object placementArgs,
    out bool isDone)
  {
    isDone = animationRate.HasSpeed && animationRate.Speed <= 0.0 || animationRate.HasDuration && animationRate.Duration.Ticks == 0L;
    if (!isDone)
    {
      Vector vector = new Vector(currentPlacement.Left + currentPlacement.Width / 2.0, currentPlacement.Top + currentPlacement.Height / 2.0) - new Vector(targetPlacement.Left + targetPlacement.Width / 2.0, targetPlacement.Top + targetPlacement.Height / 2.0);
      animationRate = new AnimationRate(animationRate.HasDuration ? animationRate.Duration : TimeSpan.FromMilliseconds(vector.Length / animationRate.Speed));
    }
    placementArgs = (object) currentPlacement;
    return currentPlacement;
  }

  public override Rect GetNextChildPlacement(
    UIElement child,
    TimeSpan currentTime,
    Rect currentPlacement,
    Rect targetPlacement,
    AnimationPanel activeLayout,
    AnimationRate animationRate,
    ref object placementArgs,
    out bool isDone)
  {
    Rect nextChildPlacement = targetPlacement;
    isDone = true;
    if (this._equation != null)
    {
      Rect rect = (Rect) placementArgs;
      TimeSpan duration = animationRate.Duration;
      isDone = currentTime >= duration;
      if (!isDone)
        nextChildPlacement = new Rect(this._equation.Evaluate(currentTime, rect.Left, targetPlacement.Left, duration), this._equation.Evaluate(currentTime, rect.Top, targetPlacement.Top, duration), Math.Max(0.0, this._equation.Evaluate(currentTime, rect.Width, targetPlacement.Width, duration)), Math.Max(0.0, this._equation.Evaluate(currentTime, rect.Height, targetPlacement.Height, duration)));
    }
    return nextChildPlacement;
  }
}
