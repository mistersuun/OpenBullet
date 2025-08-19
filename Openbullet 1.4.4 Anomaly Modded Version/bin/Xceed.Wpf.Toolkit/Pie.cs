// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Pie
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public sealed class Pie : ShapeBase
{
  public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(nameof (EndAngle), typeof (double), typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) 360.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Pie.OnEndAngleChanged), new CoerceValueCallback(Pie.CoerceEndAngleValue)));
  public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof (Mode), typeof (PieMode), typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) PieMode.Manual, new PropertyChangedCallback(Pie.OnModeChanged)));
  public static readonly DependencyProperty SliceProperty = DependencyProperty.Register(nameof (Slice), typeof (double), typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Pie.OnSliceChanged), new CoerceValueCallback(Pie.CoerceSliceValue)), new ValidateValueCallback(Pie.ValidateSlice));
  public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(nameof (StartAngle), typeof (double), typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) 360.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Pie.OnStartAngleChanged)));
  public static readonly DependencyProperty SweepDirectionProperty = DependencyProperty.Register(nameof (SweepDirection), typeof (SweepDirection), typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) SweepDirection.Clockwise, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Pie.OnSweepDirectionChanged), new CoerceValueCallback(Pie.CoerceSweepDirectionValue)));
  private Rect _rect = Rect.Empty;
  private BitVector32 _cacheBits = new BitVector32(0);

  static Pie()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Pie)));
    Shape.StretchProperty.OverrideMetadata(typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) Stretch.Fill));
    Shape.StrokeLineJoinProperty.OverrideMetadata(typeof (Pie), (PropertyMetadata) new FrameworkPropertyMetadata((object) PenLineJoin.Round));
  }

  public double EndAngle
  {
    get => (double) this.GetValue(Pie.EndAngleProperty);
    set => this.SetValue(Pie.EndAngleProperty, (object) value);
  }

  private static void OnEndAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Pie) d).OnEndAngleChanged(e);
  }

  private void OnEndAngleChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.IsUpdatingEndAngle)
      return;
    if (!this.IsUpdatingStartAngle && !this.IsUpdatingSlice && !this.IsUpdatingSweepDirection && this.Mode == PieMode.Slice)
      throw new InvalidOperationException(ErrorMessages.GetMessage("EndAngleCannotBeSetDirectlyInSlice"));
    this.IsUpdatingEndAngle = true;
    try
    {
      if (this.Mode == PieMode.EndAngle)
        this.CoerceValue(Pie.SweepDirectionProperty);
      this.CoerceValue(Pie.SliceProperty);
    }
    finally
    {
      this.IsUpdatingEndAngle = false;
    }
  }

  private static object CoerceEndAngleValue(DependencyObject d, object value)
  {
    Pie pie = (Pie) d;
    if (pie.IsUpdatingSlice || pie.IsUpdatingSweepDirection || pie.IsUpdatingStartAngle && pie.Mode == PieMode.Slice)
    {
      double d2 = pie.StartAngle + (pie.SweepDirection == SweepDirection.Clockwise ? 1.0 : -1.0) * pie.Slice * 360.0;
      if (!DoubleHelper.AreVirtuallyEqual((double) value, d2))
        value = (object) d2;
    }
    return value;
  }

  public PieMode Mode
  {
    get => (PieMode) this.GetValue(Pie.ModeProperty);
    set => this.SetValue(Pie.ModeProperty, (object) value);
  }

  private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Pie) d).OnModeChanged(e);
  }

  private void OnModeChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.IsUpdatingMode)
      return;
    this.IsUpdatingMode = true;
    try
    {
      if (this.Mode != PieMode.EndAngle)
        return;
      this.CoerceValue(Pie.SweepDirectionProperty);
    }
    finally
    {
      this.IsUpdatingMode = false;
    }
  }

  public double Slice
  {
    get => (double) this.GetValue(Pie.SliceProperty);
    set => this.SetValue(Pie.SliceProperty, (object) value);
  }

  private static void OnSliceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Pie) d).OnSliceChanged(e);
  }

  private void OnSliceChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.IsUpdatingSlice)
      return;
    if (!this.IsUpdatingStartAngle && !this.IsUpdatingEndAngle && !this.IsUpdatingSweepDirection && this.Mode == PieMode.EndAngle)
      throw new InvalidOperationException(ErrorMessages.GetMessage("SliceCannotBeSetDirectlyInEndAngle"));
    this.IsUpdatingSlice = true;
    try
    {
      if (this.IsUpdatingStartAngle || this.IsUpdatingEndAngle || this.Mode == PieMode.Manual && this.IsUpdatingSweepDirection)
        return;
      this.CoerceValue(Pie.EndAngleProperty);
    }
    finally
    {
      this.IsUpdatingSlice = false;
    }
  }

  private static object CoerceSliceValue(DependencyObject d, object value)
  {
    Pie pie = (Pie) d;
    if (pie.IsUpdatingEndAngle || pie.IsUpdatingStartAngle || pie.IsUpdatingSweepDirection)
    {
      double d1 = Math.Max(-360.0, Math.Min(360.0, pie.EndAngle - pie.StartAngle)) / (pie.SweepDirection == SweepDirection.Clockwise ? 360.0 : -360.0);
      double d2 = DoubleHelper.AreVirtuallyEqual(d1, 0.0) ? 0.0 : (d1 < 0.0 ? d1 + 1.0 : d1);
      if (!DoubleHelper.AreVirtuallyEqual((double) value, d2))
        value = (object) d2;
    }
    return value;
  }

  private static bool ValidateSlice(object value)
  {
    double num = (double) value;
    if (num < 0.0 || num > 1.0 || DoubleHelper.IsNaN(num))
      throw new ArgumentException(ErrorMessages.GetMessage("SliceOOR"));
    return true;
  }

  public double StartAngle
  {
    get => (double) this.GetValue(Pie.StartAngleProperty);
    set => this.SetValue(Pie.StartAngleProperty, (object) value);
  }

  private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Pie) d).OnStartAngleChanged(e);
  }

  private void OnStartAngleChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.IsUpdatingStartAngle)
      return;
    this.IsUpdatingStartAngle = true;
    try
    {
      switch (this.Mode)
      {
        case PieMode.Manual:
          this.CoerceValue(Pie.SliceProperty);
          break;
        case PieMode.EndAngle:
          this.CoerceValue(Pie.SweepDirectionProperty);
          this.CoerceValue(Pie.SliceProperty);
          break;
        case PieMode.Slice:
          this.CoerceValue(Pie.EndAngleProperty);
          break;
      }
    }
    finally
    {
      this.IsUpdatingStartAngle = false;
    }
  }

  public SweepDirection SweepDirection
  {
    get => (SweepDirection) this.GetValue(Pie.SweepDirectionProperty);
    set => this.SetValue(Pie.SweepDirectionProperty, (object) value);
  }

  private static void OnSweepDirectionChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Pie) d).OnSweepDirectionChanged(e);
  }

  private void OnSweepDirectionChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.IsUpdatingSweepDirection)
      return;
    this.IsUpdatingSweepDirection = true;
    try
    {
      if (this.Mode == PieMode.Slice)
        this.CoerceValue(Pie.EndAngleProperty);
      else
        this.CoerceValue(Pie.SliceProperty);
    }
    finally
    {
      this.IsUpdatingSweepDirection = false;
    }
  }

  private static object CoerceSweepDirectionValue(DependencyObject d, object value)
  {
    Pie pie = (Pie) d;
    if (pie.IsUpdatingEndAngle || pie.IsUpdatingStartAngle || pie.IsUpdatingMode)
      value = !DoubleHelper.AreVirtuallyEqual(pie.StartAngle, pie.EndAngle) ? (object) (SweepDirection) (pie.EndAngle < pie.StartAngle ? 0 : 1) : (object) pie.SweepDirection;
    return value;
  }

  public override Transform GeometryTransform => Transform.Identity;

  public override Geometry RenderedGeometry => this.DefiningGeometry;

  protected override Geometry DefiningGeometry
  {
    get
    {
      double slice = this.Slice;
      if (this._rect.IsEmpty || slice <= 0.0)
        return Geometry.Empty;
      if (slice >= 1.0)
        return (Geometry) new EllipseGeometry(this._rect);
      double num = this.SweepDirection == SweepDirection.Clockwise ? 1.0 : -1.0;
      double startAngle = this.StartAngle;
      Point point1 = EllipseHelper.PointOfRadialIntersection(this._rect, startAngle);
      Point point2 = EllipseHelper.PointOfRadialIntersection(this._rect, startAngle + num * slice * 360.0);
      return (Geometry) new PathGeometry((IEnumerable<PathFigure>) new PathFigureCollection()
      {
        new PathFigure(RectHelper.Center(this._rect), (IEnumerable<PathSegment>) new PathSegmentCollection()
        {
          (PathSegment) new LineSegment(point1, true),
          (PathSegment) new ArcSegment()
          {
            Point = point2,
            Size = new Size(this._rect.Width / 2.0, this._rect.Height / 2.0),
            IsLargeArc = (slice > 0.5),
            SweepDirection = this.SweepDirection
          }
        }, true)
      });
    }
  }

  private bool IsUpdatingEndAngle
  {
    get => this._cacheBits[1];
    set => this._cacheBits[1] = value;
  }

  private bool IsUpdatingMode
  {
    get => this._cacheBits[2];
    set => this._cacheBits[2] = value;
  }

  private bool IsUpdatingSlice
  {
    get => this._cacheBits[4];
    set => this._cacheBits[4] = value;
  }

  private bool IsUpdatingStartAngle
  {
    get => this._cacheBits[8];
    set => this._cacheBits[8] = value;
  }

  private bool IsUpdatingSweepDirection
  {
    get => this._cacheBits[16 /*0x10*/];
    set => this._cacheBits[16 /*0x10*/] = value;
  }

  internal override Size GetNaturalSize()
  {
    double strokeThickness = this.GetStrokeThickness();
    return new Size(strokeThickness, strokeThickness);
  }

  internal override Rect GetDefiningGeometryBounds() => this._rect;

  protected override Size ArrangeOverride(Size finalSize)
  {
    double strokeThickness = this.GetStrokeThickness();
    double num = strokeThickness / 2.0;
    this._rect = new Rect(num, num, Math.Max(0.0, finalSize.Width - strokeThickness), Math.Max(0.0, finalSize.Height - strokeThickness));
    switch (this.Stretch)
    {
      case Stretch.None:
        this._rect.Width = this._rect.Height = 0.0;
        break;
      case Stretch.Uniform:
        if (this._rect.Width > this._rect.Height)
        {
          this._rect.Width = this._rect.Height;
          break;
        }
        this._rect.Height = this._rect.Width;
        break;
      case Stretch.UniformToFill:
        if (this._rect.Width < this._rect.Height)
        {
          this._rect.Width = this._rect.Height;
          break;
        }
        this._rect.Height = this._rect.Width;
        break;
    }
    return finalSize;
  }

  protected override Size MeasureOverride(Size constraint)
  {
    if (this.Stretch != Stretch.UniformToFill)
      return this.GetNaturalSize();
    double width = constraint.Width;
    double height = constraint.Height;
    if (double.IsInfinity(width) && double.IsInfinity(height))
      return this.GetNaturalSize();
    double num = double.IsInfinity(width) || double.IsInfinity(height) ? Math.Min(width, height) : Math.Max(width, height);
    return new Size(num, num);
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    if (this._rect.IsEmpty)
      return;
    Pen pen = this.GetPen();
    drawingContext.DrawGeometry(this.Fill, pen, this.RenderedGeometry);
  }

  private enum CacheBits
  {
    IsUpdatingEndAngle = 1,
    IsUpdatingMode = 2,
    IsUpdatingSlice = 4,
    IsUpdatingStartAngle = 8,
    IsUpdatingSweepDirection = 16, // 0x00000010
  }
}
