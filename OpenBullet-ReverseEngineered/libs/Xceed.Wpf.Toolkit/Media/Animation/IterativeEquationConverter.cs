// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Media.Animation.IterativeEquationConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace Xceed.Wpf.Toolkit.Media.Animation;

public class IterativeEquationConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return type == typeof (string);
  }

  public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return type == typeof (IterativeEquation<double>);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext typeDescriptorContext,
    CultureInfo cultureInfo,
    object value)
  {
    IterativeEquation<double> iterativeEquation = (IterativeEquation<double>) null;
    if (value is string)
    {
      switch (value as string)
      {
        case "BackEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.BackEaseIn;
          break;
        case "BackEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.BackEaseInOut;
          break;
        case "BackEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.BackEaseOut;
          break;
        case "BounceEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.BounceEaseIn;
          break;
        case "BounceEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.BounceEaseInOut;
          break;
        case "BounceEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.BounceEaseOut;
          break;
        case "CircEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.CircEaseIn;
          break;
        case "CircEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.CircEaseInOut;
          break;
        case "CircEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.CircEaseOut;
          break;
        case "CubicEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.CubicEaseIn;
          break;
        case "CubicEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.CubicEaseInOut;
          break;
        case "CubicEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.CubicEaseOut;
          break;
        case "ElasticEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.ElasticEaseIn;
          break;
        case "ElasticEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.ElasticEaseInOut;
          break;
        case "ElasticEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.ElasticEaseOut;
          break;
        case "ExpoEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.ExpoEaseIn;
          break;
        case "ExpoEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.ExpoEaseInOut;
          break;
        case "ExpoEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.ExpoEaseOut;
          break;
        case "Linear":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.Linear;
          break;
        case "QuadEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuadEaseIn;
          break;
        case "QuadEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuadEaseInOut;
          break;
        case "QuadEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuadEaseOut;
          break;
        case "QuartEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuartEaseIn;
          break;
        case "QuartEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuartEaseInOut;
          break;
        case "QuartEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuartEaseOut;
          break;
        case "QuintEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuintEaseIn;
          break;
        case "QuintEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuintEaseInOut;
          break;
        case "QuintEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.QuintEaseOut;
          break;
        case "SineEaseIn":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.SineEaseIn;
          break;
        case "SineEaseInOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.SineEaseInOut;
          break;
        case "SineEaseOut":
          iterativeEquation = (IterativeEquation<double>) PennerEquations.SineEaseOut;
          break;
      }
    }
    return (object) iterativeEquation;
  }
}
