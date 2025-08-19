// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Primitives.ExpandableContentControl
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Globalization;
using System.Windows.Media;

#nullable disable
namespace System.Windows.Controls.Primitives;

[TemplatePart(Name = "ContentSite", Type = typeof (ContentPresenter))]
public class ExpandableContentControl : ContentControl
{
  private const string ElementContentSiteName = "ContentSite";
  private readonly RectangleGeometry _clippingRectangle;
  private ContentPresenter _contentSite;
  public static readonly DependencyProperty RevealModeProperty = DependencyProperty.Register(nameof (RevealMode), typeof (ExpandDirection), typeof (ExpandableContentControl), new PropertyMetadata((object) ExpandDirection.Down, new PropertyChangedCallback(ExpandableContentControl.OnRevealModePropertyChanged)));
  private double? calculatePercentage;
  public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register(nameof (Percentage), typeof (double), typeof (ExpandableContentControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, new PropertyChangedCallback(ExpandableContentControl.OnPercentagePropertyChanged), new CoerceValueCallback(ExpandableContentControl.CoercePercentageProperty)));
  public static readonly DependencyProperty TargetSizeProperty = DependencyProperty.Register(nameof (TargetSize), typeof (Size), typeof (ExpandableContentControl), new PropertyMetadata((object) new Size(double.NaN, double.NaN), new PropertyChangedCallback(ExpandableContentControl.OnTargetSizePropertyChanged)));

  private ContentPresenter ContentSite
  {
    get => this._contentSite;
    set
    {
      if (this._contentSite != null)
        this._contentSite.SizeChanged -= new SizeChangedEventHandler(this.OnContentSiteSizeChanged);
      this._contentSite = value;
      if (this._contentSite == null)
        return;
      this._contentSite.SizeChanged += new SizeChangedEventHandler(this.OnContentSiteSizeChanged);
    }
  }

  public ExpandDirection RevealMode
  {
    get => (ExpandDirection) this.GetValue(ExpandableContentControl.RevealModeProperty);
    set => this.SetValue(ExpandableContentControl.RevealModeProperty, (object) value);
  }

  private static void OnRevealModePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ExpandableContentControl expandableContentControl = (ExpandableContentControl) d;
    ExpandDirection newValue = (ExpandDirection) e.NewValue;
    switch (newValue)
    {
      case ExpandDirection.Down:
      case ExpandDirection.Up:
      case ExpandDirection.Left:
      case ExpandDirection.Right:
        expandableContentControl.SetNonRevealDimension();
        expandableContentControl.SetRevealDimension();
        break;
      default:
        expandableContentControl.RevealMode = (ExpandDirection) e.OldValue;
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.Windows.Controls.Properties.Resources.Expander_OnExpandDirectionPropertyChanged_InvalidValue, (object) newValue), nameof (e));
    }
  }

  private bool IsVerticalRevealMode
  {
    get => this.RevealMode == ExpandDirection.Down || this.RevealMode == ExpandDirection.Up;
  }

  private bool IsHorizontalRevealMode
  {
    get => this.RevealMode == ExpandDirection.Left || this.RevealMode == ExpandDirection.Right;
  }

  public double Percentage
  {
    get => (double) this.GetValue(ExpandableContentControl.PercentageProperty);
    set => this.SetValue(ExpandableContentControl.PercentageProperty, (object) value);
  }

  private static object CoercePercentageProperty(DependencyObject d, object baseValue)
  {
    object obj = baseValue;
    if (d is ExpandableContentControl expandableContentControl && expandableContentControl.calculatePercentage.HasValue)
    {
      obj = !double.IsNaN(expandableContentControl.calculatePercentage.Value) ? (object) expandableContentControl.calculatePercentage.Value : (object) ExpandableContentControl.CalculatePercentage(expandableContentControl, expandableContentControl.TargetSize);
      expandableContentControl.calculatePercentage = new double?();
    }
    return obj;
  }

  private static void OnPercentagePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((ExpandableContentControl) d).SetRevealDimension();
  }

  public Size TargetSize
  {
    get => (Size) this.GetValue(ExpandableContentControl.TargetSizeProperty);
    set => this.SetValue(ExpandableContentControl.TargetSizeProperty, (object) value);
  }

  private static void OnTargetSizePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ExpandableContentControl expandableContentControl = (ExpandableContentControl) d;
    Size newValue = (Size) e.NewValue;
    expandableContentControl.RecalculatePercentage(double.NaN);
  }

  public event SizeChangedEventHandler ContentSizeChanged;

  protected override Size MeasureOverride(Size availableSize)
  {
    if (this.ContentSite == null)
      return new Size(0.0, 0.0);
    Size desiredSize = availableSize;
    if (this.Percentage != 1.0)
      desiredSize = this.CalculateDesiredContentSize();
    this.MeasureContent(desiredSize);
    return this.ContentSite.DesiredSize;
  }

  internal void MeasureContent(Size desiredSize)
  {
    if (this.ContentSite == null)
      return;
    this.ContentSite.Measure(desiredSize);
  }

  internal Size CalculateDesiredContentSize()
  {
    Size targetSize = this.TargetSize;
    if (targetSize.Width.Equals(double.NaN))
      targetSize.Width = double.PositiveInfinity;
    if (targetSize.Height.Equals(double.NaN))
      targetSize.Height = double.PositiveInfinity;
    return targetSize;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    if (this.ContentSite == null)
      return finalSize;
    Size targetSize = this.TargetSize;
    if (targetSize.Width.Equals(double.NaN))
      targetSize.Width = finalSize.Width;
    if (targetSize.Height.Equals(double.NaN))
      targetSize.Height = finalSize.Height;
    this.ContentSite.Arrange(new Rect(new Point(0.0, 0.0), targetSize));
    Size arrangeSize = new Size(this.IsHorizontalRevealMode ? this.Width : finalSize.Width, this.IsVerticalRevealMode ? this.Height : finalSize.Height);
    if (double.IsNaN(arrangeSize.Width))
      arrangeSize.Width = finalSize.Width;
    if (double.IsNaN(arrangeSize.Height))
      arrangeSize.Height = finalSize.Height;
    this.UpdateClip(arrangeSize);
    return arrangeSize;
  }

  private void UpdateClip(Size arrangeSize)
  {
    if (this.Clip != this._clippingRectangle)
      this.Clip = (Geometry) this._clippingRectangle;
    this._clippingRectangle.Rect = new Rect(0.0, 0.0, arrangeSize.Width, arrangeSize.Height);
  }

  internal void RecalculatePercentage(Size value)
  {
    this.Percentage = ExpandableContentControl.CalculatePercentage(this, value);
  }

  internal void RecalculatePercentage(double percentage)
  {
    this.calculatePercentage = new double?(percentage);
    this.CoerceValue(ExpandableContentControl.PercentageProperty);
  }

  private static double CalculatePercentage(
    ExpandableContentControl expandableContentControl,
    Size value)
  {
    double percentage = 0.0;
    if (expandableContentControl.ContentSite != null)
    {
      if (expandableContentControl.IsVerticalRevealMode)
        percentage = expandableContentControl.ActualHeight / (double.IsNaN(value.Height) ? expandableContentControl.ContentSite.DesiredSize.Height : value.Height);
      else if (expandableContentControl.IsHorizontalRevealMode)
        percentage = expandableContentControl.ActualWidth / (double.IsNaN(value.Width) ? expandableContentControl.ContentSite.DesiredSize.Width : value.Width);
    }
    return percentage;
  }

  private void SetRevealDimension()
  {
    if (this.ContentSite == null)
      return;
    if (this.IsHorizontalRevealMode)
    {
      double width = this.TargetSize.Width;
      if (double.IsNaN(width))
        width = this.ContentSite.DesiredSize.Width;
      this.Width = this.Percentage * width;
    }
    if (!this.IsVerticalRevealMode)
      return;
    double height = this.TargetSize.Height;
    if (double.IsNaN(height))
      height = this.ContentSite.DesiredSize.Height;
    this.Height = this.Percentage * height;
  }

  private void SetNonRevealDimension()
  {
    if (this.IsHorizontalRevealMode)
      this.Height = this.TargetSize.Height;
    if (!this.IsVerticalRevealMode)
      return;
    this.Width = this.TargetSize.Width;
  }

  internal Size RelevantContentSize
  {
    get
    {
      return new Size(this.IsHorizontalRevealMode ? this.Width : 0.0, this.IsVerticalRevealMode ? this.Height : 0.0);
    }
  }

  static ExpandableContentControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ExpandableContentControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ExpandableContentControl)));
  }

  public ExpandableContentControl()
  {
    this._clippingRectangle = new RectangleGeometry();
    this.Clip = (Geometry) this._clippingRectangle;
  }

  public override void OnApplyTemplate()
  {
    this.ContentSite = this.GetTemplateChild("ContentSite") as ContentPresenter;
    this.SetRevealDimension();
    this.SetNonRevealDimension();
  }

  private void OnContentSiteSizeChanged(object sender, SizeChangedEventArgs e)
  {
    SizeChangedEventHandler contentSizeChanged = this.ContentSizeChanged;
    if (contentSizeChanged == null)
      return;
    contentSizeChanged((object) this, e);
  }
}
