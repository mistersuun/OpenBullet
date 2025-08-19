// Decompiled with JetBrains decompiler
// Type: MS.Internal.DoubleUtil
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Runtime.InteropServices;
using System.Windows;

#nullable disable
namespace MS.Internal;

internal static class DoubleUtil
{
  internal const double DBL_EPSILON = 2.2204460492503131E-16;
  internal const float FLT_MIN = 1.17549435E-38f;

  public static bool AreClose(double value1, double value2)
  {
    if (value1 == value2)
      return true;
    double num1 = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
    double num2 = value1 - value2;
    return -num1 < num2 && num1 > num2;
  }

  public static bool LessThan(double value1, double value2)
  {
    return value1 < value2 && !DoubleUtil.AreClose(value1, value2);
  }

  public static bool GreaterThan(double value1, double value2)
  {
    return value1 > value2 && !DoubleUtil.AreClose(value1, value2);
  }

  public static bool LessThanOrClose(double value1, double value2)
  {
    return value1 < value2 || DoubleUtil.AreClose(value1, value2);
  }

  public static bool GreaterThanOrClose(double value1, double value2)
  {
    return value1 > value2 || DoubleUtil.AreClose(value1, value2);
  }

  public static bool IsOne(double value) => Math.Abs(value - 1.0) < 2.2204460492503131E-15;

  public static bool AreClose(Point point1, Point point2)
  {
    return DoubleUtil.AreClose(point1.X, point2.X) && DoubleUtil.AreClose(point1.Y, point2.Y);
  }

  public static bool AreClose(Rect rect1, Rect rect2)
  {
    if (rect1.IsEmpty)
      return rect2.IsEmpty;
    return !rect2.IsEmpty && DoubleUtil.AreClose(rect1.X, rect2.X) && DoubleUtil.AreClose(rect1.Y, rect2.Y) && DoubleUtil.AreClose(rect1.Height, rect2.Height) && DoubleUtil.AreClose(rect1.Width, rect2.Width);
  }

  public static bool IsNaN(double value)
  {
    DoubleUtil.NanUnion nanUnion = new DoubleUtil.NanUnion();
    nanUnion.DoubleValue = value;
    ulong num1 = nanUnion.UintValue & 18442240474082181120UL /*0xFFF0000000000000*/;
    ulong num2 = nanUnion.UintValue & 4503599627370495UL /*0x0FFFFFFFFFFFFF*/;
    return (num1 == 9218868437227405312UL /*0x7FF0000000000000*/ || num1 == 18442240474082181120UL /*0xFFF0000000000000*/) && num2 != 0UL;
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
