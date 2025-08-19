// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.IterativeAnimator
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Media.Animation;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

[TypeConverter(typeof (AnimatorConverter))]
public abstract class IterativeAnimator
{
  private static readonly IterativeAnimator _default = (IterativeAnimator) new IterativeAnimator.DefaultAnimator();

  public static IterativeAnimator Default => IterativeAnimator._default;

  public abstract Rect GetInitialChildPlacement(
    UIElement child,
    Rect currentPlacement,
    Rect targetPlacement,
    AnimationPanel activeLayout,
    ref AnimationRate animationRate,
    out object placementArgs,
    out bool isDone);

  public abstract Rect GetNextChildPlacement(
    UIElement child,
    TimeSpan currentTime,
    Rect currentPlacement,
    Rect targetPlacement,
    AnimationPanel activeLayout,
    AnimationRate animationRate,
    ref object placementArgs,
    out bool isDone);

  private sealed class DefaultAnimator : IterativeAnimator
  {
    public override Rect GetInitialChildPlacement(
      UIElement child,
      Rect currentPlacement,
      Rect targetPlacement,
      AnimationPanel activeLayout,
      ref AnimationRate animationRate,
      out object placementArgs,
      out bool isDone)
    {
      throw new InvalidOperationException(ErrorMessages.GetMessage("DefaultAnimatorCantAnimate"));
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
      throw new InvalidOperationException(ErrorMessages.GetMessage("DefaultAnimatorCantAnimate"));
    }
  }
}
