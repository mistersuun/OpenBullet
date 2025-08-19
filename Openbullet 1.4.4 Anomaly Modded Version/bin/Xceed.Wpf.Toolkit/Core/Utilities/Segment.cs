// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.Segment
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal struct Segment
{
  private bool _isP1Excluded;
  private bool _isP2Excluded;
  private Point _p1;
  private Point _p2;

  public Segment(Point point)
  {
    this._p1 = point;
    this._p2 = point;
    this._isP1Excluded = false;
    this._isP2Excluded = false;
  }

  public Segment(Point p1, Point p2)
  {
    this._p1 = p1;
    this._p2 = p2;
    this._isP1Excluded = false;
    this._isP2Excluded = false;
  }

  public Segment(Point p1, Point p2, bool excludeP1, bool excludeP2)
  {
    this._p1 = p1;
    this._p2 = p2;
    this._isP1Excluded = excludeP1;
    this._isP2Excluded = excludeP2;
  }

  public static Segment Empty
  {
    get
    {
      return new Segment(new Point(0.0, 0.0))
      {
        _isP1Excluded = true,
        _isP2Excluded = true
      };
    }
  }

  public Point P1 => this._p1;

  public Point P2 => this._p2;

  public bool IsP1Excluded => this._isP1Excluded;

  public bool IsP2Excluded => this._isP2Excluded;

  public bool IsEmpty
  {
    get
    {
      if (!DoubleHelper.AreVirtuallyEqual(this._p1, this._p2))
        return false;
      return this._isP1Excluded || this._isP2Excluded;
    }
  }

  public bool IsPoint => DoubleHelper.AreVirtuallyEqual(this._p1, this._p2);

  public double Length => (this.P2 - this.P1).Length;

  public double Slope
  {
    get
    {
      Point point1 = this.P2;
      double x1 = point1.X;
      point1 = this.P1;
      double x2 = point1.X;
      if (x1 == x2)
        return double.NaN;
      Point point2 = this.P2;
      double y1 = point2.Y;
      point2 = this.P1;
      double y2 = point2.Y;
      double num1 = y1 - y2;
      Point point3 = this.P2;
      double x3 = point3.X;
      point3 = this.P1;
      double x4 = point3.X;
      double num2 = x3 - x4;
      return num1 / num2;
    }
  }

  public bool Contains(Point point)
  {
    if (this.IsEmpty)
      return false;
    if (DoubleHelper.AreVirtuallyEqual(this._p1, point))
      return this._isP1Excluded;
    if (DoubleHelper.AreVirtuallyEqual(this._p2, point))
      return this._isP2Excluded;
    bool flag = false;
    if (DoubleHelper.AreVirtuallyEqual(this.Slope, new Segment(this._p1, point).Slope))
      flag = point.X >= Math.Min(this._p1.X, this._p2.X) && point.X <= Math.Max(this._p1.X, this._p2.X) && point.Y >= Math.Min(this._p1.Y, this._p2.Y) && point.Y <= Math.Max(this._p1.Y, this._p2.Y);
    return flag;
  }

  public bool Contains(Segment segment) => segment == this.Intersection(segment);

  public override bool Equals(object o)
  {
    if (!(o is Segment segment))
      return false;
    if (this.IsEmpty)
      return segment.IsEmpty;
    return DoubleHelper.AreVirtuallyEqual(this._p1, segment._p1) ? DoubleHelper.AreVirtuallyEqual(this._p2, segment._p2) && this._isP1Excluded == segment._isP1Excluded && this._isP2Excluded == segment._isP2Excluded : DoubleHelper.AreVirtuallyEqual(this._p1, segment._p2) && DoubleHelper.AreVirtuallyEqual(this._p2, segment._p1) && this._isP1Excluded == segment._isP2Excluded && this._isP2Excluded == segment._isP1Excluded;
  }

  public override int GetHashCode()
  {
    return this._p1.GetHashCode() ^ this._p2.GetHashCode() ^ this._isP1Excluded.GetHashCode() ^ this._isP2Excluded.GetHashCode();
  }

  public Segment Intersection(Segment segment)
  {
    if (this.IsEmpty || segment.IsEmpty)
      return Segment.Empty;
    if (this == segment)
      return new Segment(this._p1, this._p2, this._isP1Excluded, this._isP2Excluded);
    if (this.IsPoint)
      return !segment.Contains(this._p1) ? Segment.Empty : new Segment(this._p1);
    if (segment.IsPoint)
      return !this.Contains(segment._p1) ? Segment.Empty : new Segment(segment._p1);
    Point p1_1 = this._p1;
    Vector vector = this._p2 - this._p1;
    Point p1_2 = segment._p1;
    Vector vector2 = segment._p2 - segment._p1;
    Point point = p1_1;
    Vector vector1 = p1_2 - point;
    double num1 = Vector.CrossProduct(vector, vector2);
    if (!DoubleHelper.AreVirtuallyEqual(this.Slope, segment.Slope))
    {
      double num2 = Vector.CrossProduct(vector1, vector) / num1;
      if (num2 < 0.0 || num2 > 1.0)
        return Segment.Empty;
      double num3 = Vector.CrossProduct(vector1, vector2) / num1;
      return num3 < 0.0 || num3 > 1.0 ? Segment.Empty : new Segment(p1_1 + num3 * vector);
    }
    double num4 = Vector.CrossProduct(vector1, vector);
    if (num4 * num4 > 1E-06 * vector.LengthSquared * vector1.LengthSquared)
      return Segment.Empty;
    Segment segment1 = new Segment();
    Segment segment2 = new Segment(this._p1, this._p2);
    Segment segment3 = new Segment(segment._p1, segment._p2);
    bool flag1 = segment3.Contains(segment2._p1);
    bool flag2 = segment3.Contains(segment2._p2);
    if (flag1 & flag2)
    {
      segment1._p1 = this._p1;
      segment1._p2 = this._p2;
      segment1._isP1Excluded = this._isP1Excluded || !segment.Contains(this._p1);
      segment1._isP2Excluded = this._isP2Excluded || !segment.Contains(this._p2);
      return segment1;
    }
    bool flag3 = segment2.Contains(segment3._p1);
    bool flag4 = segment2.Contains(segment3._p2);
    if (flag3 & flag4)
    {
      segment1._p1 = segment._p1;
      segment1._p2 = segment._p2;
      segment1._isP1Excluded = segment._isP1Excluded || !this.Contains(segment._p1);
      segment1._isP2Excluded = segment._isP2Excluded || !this.Contains(segment._p2);
      return segment1;
    }
    if (flag1)
    {
      segment1._p1 = this._p1;
      segment1._isP1Excluded = this._isP1Excluded || !segment.Contains(this._p1);
    }
    else
    {
      segment1._p1 = this._p2;
      segment1._isP1Excluded = this._isP2Excluded || !segment.Contains(this._p2);
    }
    if (flag3)
    {
      segment1._p2 = segment._p1;
      segment1._isP2Excluded = segment._isP1Excluded || !this.Contains(segment._p1);
    }
    else
    {
      segment1._p2 = segment._p2;
      segment1._isP2Excluded = segment._isP2Excluded || !this.Contains(segment._p2);
    }
    return segment1;
  }

  public override string ToString()
  {
    string str1 = base.ToString();
    string str2;
    if (this.IsEmpty)
      str2 = str1 + ": {Empty}";
    else if (this.IsPoint)
      str2 = $"{str1}, Point: {this._p1.ToString()}";
    else
      str2 = $"{str1}: {this._p1.ToString()}{(this._isP1Excluded ? " (excl)" : " (incl)")} to {this._p2.ToString()}{(this._isP2Excluded ? " (excl)" : " (incl)")}";
    return str2;
  }

  public static bool operator ==(Segment s1, Segment s2)
  {
    if ((ValueType) s1 == null)
      return (ValueType) s2 == null;
    return (ValueType) s2 == null ? (ValueType) s1 == null : s1.Equals((object) s2);
  }

  public static bool operator !=(Segment s1, Segment s2) => !(s1 == s2);
}
