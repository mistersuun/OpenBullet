// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.ShapeBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public abstract class ShapeBase : Shape
{
  private Pen _pen;

  static ShapeBase()
  {
    Shape.StrokeDashArrayProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeDashCapProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeDashOffsetProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeEndLineCapProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeLineJoinProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeMiterLimitProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeStartLineCapProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
    Shape.StrokeThicknessProperty.OverrideMetadata(typeof (ShapeBase), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(ShapeBase.OnStrokeChanged)));
  }

  internal bool IsPenEmptyOrUndefined
  {
    get
    {
      double strokeThickness = this.StrokeThickness;
      return this.Stroke == null || DoubleHelper.IsNaN(strokeThickness) || DoubleHelper.AreVirtuallyEqual(0.0, strokeThickness);
    }
  }

  protected abstract override Geometry DefiningGeometry { get; }

  internal new virtual Rect GetDefiningGeometryBounds() => this.DefiningGeometry.Bounds;

  internal new virtual Size GetNaturalSize()
  {
    Rect renderBounds = this.DefiningGeometry.GetRenderBounds(this.GetPen());
    return new Size(Math.Max(renderBounds.Right, 0.0), Math.Max(renderBounds.Bottom, 0.0));
  }

  internal new Pen GetPen()
  {
    if (this.IsPenEmptyOrUndefined)
      return (Pen) null;
    if (this._pen == null)
      this._pen = this.MakePen();
    return this._pen;
  }

  internal new double GetStrokeThickness()
  {
    return this.IsPenEmptyOrUndefined ? 0.0 : Math.Abs(this.StrokeThickness);
  }

  internal bool IsSizeEmptyOrUndefined(Size size)
  {
    return DoubleHelper.IsNaN(size.Width) || DoubleHelper.IsNaN(size.Height) || size.IsEmpty;
  }

  private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((ShapeBase) d)._pen = (Pen) null;
  }

  private Pen MakePen()
  {
    Pen pen = new Pen();
    pen.Brush = this.Stroke;
    pen.DashCap = this.StrokeDashCap;
    if (this.StrokeDashArray != null || this.StrokeDashOffset != 0.0)
      pen.DashStyle = new DashStyle((IEnumerable<double>) this.StrokeDashArray, this.StrokeDashOffset);
    pen.EndLineCap = this.StrokeEndLineCap;
    pen.LineJoin = this.StrokeLineJoin;
    pen.MiterLimit = this.StrokeMiterLimit;
    pen.StartLineCap = this.StrokeStartLineCap;
    pen.Thickness = Math.Abs(this.StrokeThickness);
    return pen;
  }
}
