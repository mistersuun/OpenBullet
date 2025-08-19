// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.Line
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;
using System.Drawing;

#nullable disable
namespace RuriLib.LS;

public class Line
{
  private Point p1;
  private Point p2;
  private Random rand;

  public Line(Point p1, Point p2)
  {
    this.p1 = p1;
    this.p2 = p2;
    this.rand = new Random();
  }

  public Point[] getPoints(int quantity)
  {
    Point[] points = new Point[quantity];
    int num1 = this.p2.Y - this.p1.Y;
    int num2 = this.p2.X - this.p1.X;
    double num3 = (double) (this.p2.Y - this.p1.Y) / (double) (this.p2.X - this.p1.X);
    --quantity;
    for (double index = 0.0; index < (double) quantity; ++index)
    {
      double a1 = num3 == 0.0 ? 0.0 : (double) num1 * (index / (double) quantity);
      double a2 = num3 == 0.0 ? (double) num2 * (index / (double) quantity) : a1 / num3;
      points[(int) index] = new Point((int) Math.Round(a2) + this.p1.X, (int) Math.Round(a1) + this.p1.Y);
    }
    points[quantity] = this.p2;
    return points;
  }

  public Point[] getOffsets(int quantity)
  {
    Point[] offsets = new Point[quantity];
    int num1 = this.p2.Y - this.p1.Y;
    int num2 = this.p2.X - this.p1.X;
    double num3 = (double) (this.p2.Y - this.p1.Y) / (double) (this.p2.X - this.p1.X);
    --quantity;
    for (double index = 0.0; index < (double) quantity; ++index)
    {
      double a1 = num3 == 0.0 ? 0.0 : (double) num1 * (index / (double) quantity);
      double a2 = num3 == 0.0 ? (double) num2 * (index / (double) quantity) : a1 / num3;
      offsets[(int) index] = new Point((int) Math.Round(a2), (int) Math.Round(a1));
    }
    offsets[quantity] = this.p2;
    return offsets;
  }

  private static double Distance(double x1, double y1, double x2, double y2)
  {
    return Math.Sqrt(Math.Pow(x2 - x1, 2.0) + Math.Pow(y2 - y1, 2.0));
  }

  private static double Hypot(double x, double y) => Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0));

  public Point[] HumanWindMouse(
    double xs,
    double ys,
    double xe,
    double ye,
    double gravity,
    double wind,
    double targetArea)
  {
    double x = 0.0;
    double y = 0.0;
    double num1 = 0.0;
    double num2 = 0.0;
    double num3 = Math.Sqrt(2.0);
    double num4 = Math.Sqrt(3.0);
    double num5 = Math.Sqrt(5.0);
    int a1 = (int) Line.Distance(Math.Round(xs), Math.Round(ys), Math.Round(xe), Math.Round(ye));
    uint num6 = (uint) (Environment.TickCount + 10000);
    List<Point> pointList = new List<Point>();
    while ((long) Environment.TickCount <= (long) num6)
    {
      double num7 = Line.Hypot(xs - xe, ys - ye);
      wind = Math.Min(wind, num7);
      if (num7 < 1.0)
        num7 = 1.0;
      double num8 = Math.Round(Math.Round((double) a1) * 0.3) / 7.0;
      if (num8 > 25.0)
        num8 = 25.0;
      if (num8 < 5.0)
        num8 = 5.0;
      if ((double) this.rand.Next(6) == 1.0)
        num8 = 2.0;
      double a2 = num8 > Math.Round(num7) ? Math.Round(num7) : num8;
      if (num7 >= targetArea)
      {
        num1 = num1 / num4 + ((double) this.rand.Next((int) (Math.Round(wind) * 2.0 + 1.0)) - wind) / num5;
        num2 = num2 / num4 + ((double) this.rand.Next((int) (Math.Round(wind) * 2.0 + 1.0)) - wind) / num5;
      }
      else
      {
        num1 /= num3;
        num2 /= num3;
      }
      double num9 = x + num1;
      double num10 = y + num2;
      x = num9 + gravity * (xe - xs) / num7;
      y = num10 + gravity * (ye - ys) / num7;
      if (Line.Hypot(x, y) > a2)
      {
        double num11 = a2 / 2.0 + (double) this.rand.Next((int) (Math.Round(a2) / 2.0));
        double num12 = Math.Sqrt(x * x + y * y);
        x = x / num12 * num11;
        y = y / num12 * num11;
      }
      int num13 = (int) Math.Round(xs);
      int num14 = (int) Math.Round(ys);
      xs += x;
      ys += y;
      if ((double) num13 != Math.Round(xs) || (double) num14 != Math.Round(ys))
        pointList.Add(new Point((int) Math.Round(xs), (int) Math.Round(ys)));
      if (Line.Hypot(xs - xe, ys - ye) < 1.0)
        break;
    }
    if (Math.Round(xe) != Math.Round(xs) || Math.Round(ye) != Math.Round(ys))
      pointList.Add(new Point((int) Math.Round(xe), (int) Math.Round(ye)));
    return pointList.ToArray();
  }
}
