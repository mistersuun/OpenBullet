// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.EllipseHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class EllipseHelper
{
  public static Point PointOfRadialIntersection(Rect ellipseRect, double angle)
  {
    double num1 = ellipseRect.Width / 2.0;
    double num2 = ellipseRect.Height / 2.0;
    double num3 = angle * Math.PI / 180.0;
    return RectHelper.Center(ellipseRect) + new Vector(num1 * Math.Cos(num3), num2 * Math.Sin(num3));
  }

  public static double RadialDistanceFromCenter(Rect ellipseRect, double angle)
  {
    double num1 = ellipseRect.Width / 2.0;
    double num2 = ellipseRect.Height / 2.0;
    double num3 = angle * Math.PI / 180.0;
    double num4 = Math.Sin(num3);
    double num5 = Math.Cos(num3);
    return Math.Sqrt(num1 * num1 * num2 * num2 / (num1 * num1 * num4 * num4 + num2 * num2 * num5 * num5));
  }
}
