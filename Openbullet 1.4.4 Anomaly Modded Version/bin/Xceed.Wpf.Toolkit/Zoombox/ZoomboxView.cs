// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Zoombox.ZoomboxView
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Zoombox;

[TypeConverter(typeof (ZoomboxViewConverter))]
public class ZoomboxView
{
  private static readonly ZoomboxView _empty = new ZoomboxView(ZoomboxViewKind.Empty);
  private static readonly ZoomboxView _fill = new ZoomboxView(ZoomboxViewKind.Fill);
  private static readonly ZoomboxView _fit = new ZoomboxView(ZoomboxViewKind.Fit);
  private static readonly ZoomboxView _center = new ZoomboxView(ZoomboxViewKind.Center);
  private double _kindHeight = -1.0;
  private double _x = double.NaN;
  private double _y = double.NaN;
  private double _scaleWidth = double.NaN;

  public ZoomboxView()
  {
  }

  public ZoomboxView(double scale) => this.Scale = scale;

  public ZoomboxView(Point position) => this.Position = position;

  public ZoomboxView(double scale, Point position)
  {
    this.Position = position;
    this.Scale = scale;
  }

  public ZoomboxView(Rect region) => this.Region = region;

  public ZoomboxView(double x, double y)
    : this(new Point(x, y))
  {
  }

  public ZoomboxView(double scale, double x, double y)
    : this(scale, new Point(x, y))
  {
  }

  public ZoomboxView(double x, double y, double width, double height)
    : this(new Rect(x, y, width, height))
  {
  }

  public static ZoomboxView Empty => ZoomboxView._empty;

  public static ZoomboxView Fill => ZoomboxView._fill;

  public static ZoomboxView Fit => ZoomboxView._fit;

  public static ZoomboxView Center => ZoomboxView._center;

  public ZoomboxViewKind ViewKind
  {
    get => this._kindHeight > 0.0 ? ZoomboxViewKind.Region : (ZoomboxViewKind) this._kindHeight;
  }

  public Point Position
  {
    get
    {
      if (this.ViewKind != ZoomboxViewKind.Absolute)
        throw new InvalidOperationException(ErrorMessages.GetMessage("PositionOnlyAccessibleOnAbsolute"));
      return new Point(this._x, this._y);
    }
    set
    {
      if (this.ViewKind != ZoomboxViewKind.Absolute && this.ViewKind != ZoomboxViewKind.Empty)
        throw new InvalidOperationException(string.Format(ErrorMessages.GetMessage("ZoomboxViewAlreadyInitialized"), (object) this.ViewKind.ToString()));
      this._x = value.X;
      this._y = value.Y;
      this._kindHeight = -5.0;
    }
  }

  public double Scale
  {
    get
    {
      if (this.ViewKind != ZoomboxViewKind.Absolute)
        throw new InvalidOperationException(ErrorMessages.GetMessage("ScaleOnlyAccessibleOnAbsolute"));
      return this._scaleWidth;
    }
    set
    {
      if (this.ViewKind != ZoomboxViewKind.Absolute && this.ViewKind != ZoomboxViewKind.Empty)
        throw new InvalidOperationException(string.Format(ErrorMessages.GetMessage("ZoomboxViewAlreadyInitialized"), (object) this.ViewKind.ToString()));
      this._scaleWidth = value;
      this._kindHeight = -5.0;
    }
  }

  public Rect Region
  {
    get
    {
      if (this._kindHeight < 0.0)
        throw new InvalidOperationException(ErrorMessages.GetMessage("RegionOnlyAccessibleOnRegionalView"));
      return new Rect(this._x, this._y, this._scaleWidth, this._kindHeight);
    }
    set
    {
      if (this.ViewKind != ZoomboxViewKind.Region && this.ViewKind != ZoomboxViewKind.Empty)
        throw new InvalidOperationException(string.Format(ErrorMessages.GetMessage("ZoomboxViewAlreadyInitialized"), (object) this.ViewKind.ToString()));
      if (value.IsEmpty)
        return;
      this._x = value.X;
      this._y = value.Y;
      this._scaleWidth = value.Width;
      this._kindHeight = value.Height;
    }
  }

  public override int GetHashCode()
  {
    return this._x.GetHashCode() ^ this._y.GetHashCode() ^ this._scaleWidth.GetHashCode() ^ this._kindHeight.GetHashCode();
  }

  public override bool Equals(object o)
  {
    bool flag = false;
    if ((object) (o as ZoomboxView) != null)
    {
      ZoomboxView zoomboxView = (ZoomboxView) o;
      if (this.ViewKind == zoomboxView.ViewKind)
      {
        switch (this.ViewKind)
        {
          case ZoomboxViewKind.Absolute:
            flag = DoubleHelper.AreVirtuallyEqual(this._scaleWidth, zoomboxView._scaleWidth) && DoubleHelper.AreVirtuallyEqual(this.Position, zoomboxView.Position);
            break;
          case ZoomboxViewKind.Region:
            flag = DoubleHelper.AreVirtuallyEqual(this.Region, zoomboxView.Region);
            break;
          default:
            flag = true;
            break;
        }
      }
    }
    return flag;
  }

  public override string ToString()
  {
    switch (this.ViewKind)
    {
      case ZoomboxViewKind.Absolute:
        return $"ZoomboxView: Scale = {this._scaleWidth.ToString("f")}; Position = ({this._x.ToString("f")}, {this._y.ToString("f")})";
      case ZoomboxViewKind.Fit:
        return "ZoomboxView: Fit";
      case ZoomboxViewKind.Fill:
        return "ZoomboxView: Fill";
      case ZoomboxViewKind.Center:
        return "ZoomboxView: Center";
      case ZoomboxViewKind.Empty:
        return "ZoomboxView: Empty";
      case ZoomboxViewKind.Region:
        return $"ZoomboxView: Region = ({this._x.ToString("f")}, {this._y.ToString("f")}, {this._scaleWidth.ToString("f")}, {this._kindHeight.ToString("f")})";
      default:
        return base.ToString();
    }
  }

  private ZoomboxView(ZoomboxViewKind viewType) => this._kindHeight = (double) viewType;

  public static bool operator ==(ZoomboxView v1, ZoomboxView v2)
  {
    if ((object) v1 == null)
      return (object) v2 == null;
    return (object) v2 == null ? (object) v1 == null : v1.Equals((object) v2);
  }

  public static bool operator !=(ZoomboxView v1, ZoomboxView v2) => !(v1 == v2);
}
