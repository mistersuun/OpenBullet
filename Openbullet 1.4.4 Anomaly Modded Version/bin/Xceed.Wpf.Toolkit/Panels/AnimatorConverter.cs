// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.AnimatorConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public sealed class AnimatorConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return type == typeof (string);
  }

  public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return type == typeof (IterativeAnimator) || type == typeof (DoubleAnimator);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext typeDescriptorContext,
    CultureInfo cultureInfo,
    object value)
  {
    IterativeAnimator iterativeAnimator = (IterativeAnimator) null;
    if (value is string)
    {
      switch (value as string)
      {
        case "BackEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.BackEaseIn;
          break;
        case "BackEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.BackEaseInOut;
          break;
        case "BackEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.BackEaseOut;
          break;
        case "BounceEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.BounceEaseIn;
          break;
        case "BounceEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.BounceEaseInOut;
          break;
        case "BounceEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.BounceEaseOut;
          break;
        case "CircEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.CircEaseIn;
          break;
        case "CircEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.CircEaseInOut;
          break;
        case "CircEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.CircEaseOut;
          break;
        case "CubicEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.CubicEaseIn;
          break;
        case "CubicEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.CubicEaseInOut;
          break;
        case "CubicEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.CubicEaseOut;
          break;
        case "ElasticEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.ElasticEaseIn;
          break;
        case "ElasticEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.ElasticEaseInOut;
          break;
        case "ElasticEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.ElasticEaseOut;
          break;
        case "ExpoEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.ExpoEaseIn;
          break;
        case "ExpoEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.ExpoEaseInOut;
          break;
        case "ExpoEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.ExpoEaseOut;
          break;
        case "Linear":
          iterativeAnimator = (IterativeAnimator) Animators.Linear;
          break;
        case "QuadEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.QuadEaseIn;
          break;
        case "QuadEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.QuadEaseInOut;
          break;
        case "QuadEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.QuadEaseOut;
          break;
        case "QuartEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.QuartEaseIn;
          break;
        case "QuartEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.QuartEaseInOut;
          break;
        case "QuartEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.QuartEaseOut;
          break;
        case "QuintEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.QuintEaseIn;
          break;
        case "QuintEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.QuintEaseInOut;
          break;
        case "QuintEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.QuintEaseOut;
          break;
        case "SineEaseIn":
          iterativeAnimator = (IterativeAnimator) Animators.SineEaseIn;
          break;
        case "SineEaseInOut":
          iterativeAnimator = (IterativeAnimator) Animators.SineEaseInOut;
          break;
        case "SineEaseOut":
          iterativeAnimator = (IterativeAnimator) Animators.SineEaseOut;
          break;
      }
    }
    return (object) iterativeAnimator;
  }
}
