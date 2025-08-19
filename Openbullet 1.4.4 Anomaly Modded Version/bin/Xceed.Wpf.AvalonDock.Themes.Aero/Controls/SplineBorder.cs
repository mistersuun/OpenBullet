// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Themes.Controls.SplineBorder
// Assembly: Xceed.Wpf.AvalonDock.Themes.Aero, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F3B428EA-E0EF-4865-A985-6BA3455007AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.Themes.Aero.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Themes.Controls;

public class SplineBorder : Control
{
  public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof (Thickness), typeof (double), typeof (SplineBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty FillProperty = DependencyProperty.Register(nameof (Fill), typeof (Brush), typeof (SplineBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof (Stroke), typeof (Brush), typeof (SplineBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty BottomBorderMarginProperty = DependencyProperty.Register(nameof (BottomBorderMargin), typeof (double), typeof (SplineBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsRender));

  public double Thickness
  {
    get => (double) this.GetValue(SplineBorder.ThicknessProperty);
    set => this.SetValue(SplineBorder.ThicknessProperty, (object) value);
  }

  public Brush Fill
  {
    get => (Brush) this.GetValue(SplineBorder.FillProperty);
    set => this.SetValue(SplineBorder.FillProperty, (object) value);
  }

  public Brush Stroke
  {
    get => (Brush) this.GetValue(SplineBorder.StrokeProperty);
    set => this.SetValue(SplineBorder.StrokeProperty, (object) value);
  }

  public double BottomBorderMargin
  {
    get => (double) this.GetValue(SplineBorder.BottomBorderMarginProperty);
    set => this.SetValue(SplineBorder.BottomBorderMarginProperty, (object) value);
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    PathGeometry pathGeometry1 = new PathGeometry();
    PathFigure pathFigure1 = new PathFigure()
    {
      IsFilled = true,
      IsClosed = true
    };
    pathFigure1.StartPoint = new Point(this.ActualWidth, 0.0);
    QuadraticBezierSegment quadraticBezierSegment1 = new QuadraticBezierSegment();
    quadraticBezierSegment1.Point1 = new Point(this.ActualWidth * 2.0 / 3.0, 0.0);
    quadraticBezierSegment1.Point2 = new Point(this.ActualWidth / 2.0, this.ActualHeight / 2.0);
    quadraticBezierSegment1.IsStroked = false;
    QuadraticBezierSegment quadraticBezierSegment2 = quadraticBezierSegment1;
    pathFigure1.Segments.Add((PathSegment) quadraticBezierSegment2);
    QuadraticBezierSegment quadraticBezierSegment3 = new QuadraticBezierSegment();
    quadraticBezierSegment3.Point1 = new Point(this.ActualWidth / 3.0, this.ActualHeight);
    quadraticBezierSegment3.Point2 = new Point(0.0, this.ActualHeight);
    quadraticBezierSegment3.IsStroked = false;
    QuadraticBezierSegment quadraticBezierSegment4 = quadraticBezierSegment3;
    pathFigure1.Segments.Add((PathSegment) quadraticBezierSegment4);
    PathSegmentCollection segments = pathFigure1.Segments;
    LineSegment lineSegment = new LineSegment();
    lineSegment.Point = new Point(this.ActualWidth, this.ActualHeight);
    lineSegment.IsStroked = false;
    segments.Add((PathSegment) lineSegment);
    pathGeometry1.Figures.Add(pathFigure1);
    drawingContext.DrawGeometry(this.Fill, (Pen) null, (Geometry) pathGeometry1);
    PathGeometry pathGeometry2 = new PathGeometry();
    PathFigure pathFigure2 = new PathFigure()
    {
      IsFilled = false,
      IsClosed = false
    };
    pathFigure2.StartPoint = new Point(this.ActualWidth, this.Thickness / 2.0);
    QuadraticBezierSegment quadraticBezierSegment5 = new QuadraticBezierSegment()
    {
      Point1 = new Point(this.ActualWidth * 2.0 / 3.0, 0.0),
      Point2 = new Point(this.ActualWidth / 2.0, this.ActualHeight / 2.0)
    };
    pathFigure2.Segments.Add((PathSegment) quadraticBezierSegment5);
    QuadraticBezierSegment quadraticBezierSegment6 = new QuadraticBezierSegment()
    {
      Point1 = new Point(this.ActualWidth / 3.0, this.ActualHeight),
      Point2 = new Point(0.0, this.ActualHeight - this.BottomBorderMargin)
    };
    pathFigure2.Segments.Add((PathSegment) quadraticBezierSegment6);
    pathGeometry2.Figures.Add(pathFigure2);
    drawingContext.DrawGeometry((Brush) null, new Pen(this.Stroke, this.Thickness), (Geometry) pathGeometry2);
    base.OnRender(drawingContext);
  }
}
