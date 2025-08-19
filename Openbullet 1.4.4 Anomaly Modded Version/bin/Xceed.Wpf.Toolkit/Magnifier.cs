// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Magnifier
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_VisualBrush", Type = typeof (VisualBrush))]
public class Magnifier : Control
{
  private const double DEFAULT_SIZE = 100.0;
  private const string PART_VisualBrush = "PART_VisualBrush";
  private VisualBrush _visualBrush = new VisualBrush();
  public static readonly DependencyProperty FrameTypeProperty = DependencyProperty.Register(nameof (FrameType), typeof (FrameType), typeof (Magnifier), (PropertyMetadata) new UIPropertyMetadata((object) FrameType.Circle, new PropertyChangedCallback(Magnifier.OnFrameTypeChanged)));
  public static readonly DependencyProperty IsUsingZoomOnMouseWheelProperty = DependencyProperty.Register(nameof (IsUsingZoomOnMouseWheel), typeof (bool), typeof (Magnifier), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(nameof (Radius), typeof (double), typeof (Magnifier), (PropertyMetadata) new FrameworkPropertyMetadata((object) 50.0, new PropertyChangedCallback(Magnifier.OnRadiusPropertyChanged)));
  public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof (Target), typeof (UIElement), typeof (Magnifier));
  public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(nameof (ZoomFactor), typeof (double), typeof (Magnifier), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.5, new PropertyChangedCallback(Magnifier.OnZoomFactorPropertyChanged)), new ValidateValueCallback(Magnifier.OnValidationCallback));
  public static readonly DependencyProperty ZoomFactorOnMouseWheelProperty = DependencyProperty.Register(nameof (ZoomFactorOnMouseWheel), typeof (double), typeof (Magnifier), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.1, new PropertyChangedCallback(Magnifier.OnZoomFactorOnMouseWheelPropertyChanged)), new ValidateValueCallback(Magnifier.OnZoomFactorOnMouseWheelValidationCallback));

  public FrameType FrameType
  {
    get => (FrameType) this.GetValue(Magnifier.FrameTypeProperty);
    set => this.SetValue(Magnifier.FrameTypeProperty, (object) value);
  }

  private static void OnFrameTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Magnifier) d).OnFrameTypeChanged((FrameType) e.OldValue, (FrameType) e.NewValue);
  }

  protected virtual void OnFrameTypeChanged(FrameType oldValue, FrameType newValue)
  {
    this.UpdateSizeFromRadius();
  }

  public bool IsUsingZoomOnMouseWheel
  {
    get => (bool) this.GetValue(Magnifier.IsUsingZoomOnMouseWheelProperty);
    set => this.SetValue(Magnifier.IsUsingZoomOnMouseWheelProperty, (object) value);
  }

  public bool IsFrozen { get; private set; }

  public double Radius
  {
    get => (double) this.GetValue(Magnifier.RadiusProperty);
    set => this.SetValue(Magnifier.RadiusProperty, (object) value);
  }

  private static void OnRadiusPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Magnifier) d).OnRadiusChanged(e);
  }

  protected virtual void OnRadiusChanged(DependencyPropertyChangedEventArgs e)
  {
    this.UpdateSizeFromRadius();
  }

  public UIElement Target
  {
    get => (UIElement) this.GetValue(Magnifier.TargetProperty);
    set => this.SetValue(Magnifier.TargetProperty, (object) value);
  }

  internal Rect ViewBox
  {
    get => this._visualBrush.Viewbox;
    set => this._visualBrush.Viewbox = value;
  }

  public double ZoomFactor
  {
    get => (double) this.GetValue(Magnifier.ZoomFactorProperty);
    set => this.SetValue(Magnifier.ZoomFactorProperty, (object) value);
  }

  private static bool OnValidationCallback(object baseValue) => (double) baseValue >= 0.0;

  private static void OnZoomFactorPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Magnifier) d).OnZoomFactorChanged(e);
  }

  protected virtual void OnZoomFactorChanged(DependencyPropertyChangedEventArgs e)
  {
    this.UpdateViewBox();
  }

  public double ZoomFactorOnMouseWheel
  {
    get => (double) this.GetValue(Magnifier.ZoomFactorOnMouseWheelProperty);
    set => this.SetValue(Magnifier.ZoomFactorOnMouseWheelProperty, (object) value);
  }

  private static bool OnZoomFactorOnMouseWheelValidationCallback(object baseValue)
  {
    return (double) baseValue >= 0.0;
  }

  private static void OnZoomFactorOnMouseWheelPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Magnifier) d).OnZoomFactorOnMouseWheelChanged(e);
  }

  protected virtual void OnZoomFactorOnMouseWheelChanged(DependencyPropertyChangedEventArgs e)
  {
  }

  static Magnifier()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Magnifier), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Magnifier)));
    FrameworkElement.HeightProperty.OverrideMetadata(typeof (Magnifier), (PropertyMetadata) new FrameworkPropertyMetadata((object) 100.0));
    FrameworkElement.WidthProperty.OverrideMetadata(typeof (Magnifier), (PropertyMetadata) new FrameworkPropertyMetadata((object) 100.0));
  }

  public Magnifier() => this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChangedEvent);

  private void OnSizeChangedEvent(object sender, SizeChangedEventArgs e) => this.UpdateViewBox();

  private void UpdateSizeFromRadius()
  {
    if (this.FrameType != FrameType.Circle)
      return;
    double d2 = this.Radius * 2.0;
    if (!DoubleHelper.AreVirtuallyEqual(this.Width, d2))
      this.Width = d2;
    if (DoubleHelper.AreVirtuallyEqual(this.Height, d2))
      return;
    this.Height = d2;
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (!(this.GetTemplateChild("PART_VisualBrush") is VisualBrush visualBrush))
      visualBrush = new VisualBrush();
    visualBrush.Viewbox = this._visualBrush.Viewbox;
    this._visualBrush = visualBrush;
  }

  public void Freeze(bool freeze) => this.IsFrozen = freeze;

  private void UpdateViewBox()
  {
    if (!this.IsInitialized)
      return;
    this.ViewBox = new Rect(this.ViewBox.Location, new Size(this.ActualWidth * this.ZoomFactor, this.ActualHeight * this.ZoomFactor));
  }
}
