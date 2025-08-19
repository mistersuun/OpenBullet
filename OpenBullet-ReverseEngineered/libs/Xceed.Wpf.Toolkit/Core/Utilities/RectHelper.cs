// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.RectHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class RectHelper
{
  public static Point Center(Rect rect)
  {
    return new Point(rect.Left + rect.Width / 2.0, rect.Top + rect.Height / 2.0);
  }

  public static Point? GetNearestPointOfIntersectionBetweenRectAndSegment(
    Rect rect,
    Segment segment,
    Point point)
  {
    Point? result = new Point?();
    double distance = double.PositiveInfinity;
    Segment intersection1 = segment.Intersection(new Segment(rect.BottomLeft, rect.TopLeft));
    Segment intersection2 = segment.Intersection(new Segment(rect.TopLeft, rect.TopRight));
    Segment intersection3 = segment.Intersection(new Segment(rect.TopRight, rect.BottomRight));
    Segment intersection4 = segment.Intersection(new Segment(rect.BottomRight, rect.BottomLeft));
    RectHelper.AdjustResultForIntersectionWithSide(ref result, ref distance, intersection1, point);
    RectHelper.AdjustResultForIntersectionWithSide(ref result, ref distance, intersection2, point);
    RectHelper.AdjustResultForIntersectionWithSide(ref result, ref distance, intersection3, point);
    RectHelper.AdjustResultForIntersectionWithSide(ref result, ref distance, intersection4, point);
    return result;
  }

  public static Rect GetRectCenteredOnPoint(Point center, Size size)
  {
    return new Rect(new Point(center.X - size.Width / 2.0, center.Y - size.Height / 2.0), size);
  }

  private static void AdjustResultForIntersectionWithSide(
    ref Point? result,
    ref double distance,
    Segment intersection,
    Point point)
  {
    if (intersection.IsEmpty)
      return;
    if (intersection.Contains(point))
    {
      distance = 0.0;
      result = new Point?(point);
    }
    else
    {
      double val1 = PointHelper.DistanceBetween(point, intersection.P1);
      double val2 = double.PositiveInfinity;
      if (!intersection.IsPoint)
        val2 = PointHelper.DistanceBetween(point, intersection.P2);
      if (Math.Min(val1, val2) >= distance)
        return;
      if (val1 < val2)
      {
        distance = val1;
        result = new Point?(intersection.P1);
      }
      else
      {
        distance = val2;
        result = new Point?(intersection.P2);
      }
    }
  }
}
