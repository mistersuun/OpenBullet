// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.DoubleHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Runtime.InteropServices;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class DoubleHelper
{
  public static bool AreVirtuallyEqual(double d1, double d2)
  {
    if (double.IsPositiveInfinity(d1))
      return double.IsPositiveInfinity(d2);
    if (double.IsNegativeInfinity(d1))
      return double.IsNegativeInfinity(d2);
    if (DoubleHelper.IsNaN(d1))
      return DoubleHelper.IsNaN(d2);
    double num1 = d1 - d2;
    double num2 = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * 1E-15;
    return -num2 < num1 && num2 > num1;
  }

  public static bool AreVirtuallyEqual(Size s1, Size s2)
  {
    return DoubleHelper.AreVirtuallyEqual(s1.Width, s2.Width) && DoubleHelper.AreVirtuallyEqual(s1.Height, s2.Height);
  }

  public static bool AreVirtuallyEqual(Point p1, Point p2)
  {
    return DoubleHelper.AreVirtuallyEqual(p1.X, p2.X) && DoubleHelper.AreVirtuallyEqual(p1.Y, p2.Y);
  }

  public static bool AreVirtuallyEqual(Rect r1, Rect r2)
  {
    return DoubleHelper.AreVirtuallyEqual(r1.TopLeft, r2.TopLeft) && DoubleHelper.AreVirtuallyEqual(r1.BottomRight, r2.BottomRight);
  }

  public static bool AreVirtuallyEqual(Vector v1, Vector v2)
  {
    return DoubleHelper.AreVirtuallyEqual(v1.X, v2.X) && DoubleHelper.AreVirtuallyEqual(v1.Y, v2.Y);
  }

  public static bool AreVirtuallyEqual(Segment s1, Segment s2) => s1 == s2;

  public static bool IsNaN(double value)
  {
    DoubleHelper.NanUnion nanUnion = new DoubleHelper.NanUnion();
    nanUnion.DoubleValue = value;
    ulong num1 = nanUnion.UintValue & 18442240474082181120UL /*0xFFF0000000000000*/;
    ulong num2 = nanUnion.UintValue & 4503599627370495UL /*0x0FFFFFFFFFFFFF*/;
    return (num1 == 9218868437227405312UL /*0x7FF0000000000000*/ || num1 == 18442240474082181120UL /*0xFFF0000000000000*/) && num2 > 0UL;
  }

  [StructLayout(LayoutKind.Explicit)]
  private struct NanUnion
  {
    [FieldOffset(0)]
    internal double DoubleValue;
    [FieldOffset(0)]
    internal ulong UintValue;
  }
}
