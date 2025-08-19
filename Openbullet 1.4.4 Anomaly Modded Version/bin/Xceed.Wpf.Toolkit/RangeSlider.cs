// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.RangeSlider
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_LowerRange", Type = typeof (RepeatButton))]
[TemplatePart(Name = "PART_HigherRange", Type = typeof (RepeatButton))]
[TemplatePart(Name = "PART_HigherSlider", Type = typeof (Slider))]
[TemplatePart(Name = "PART_LowerSlider", Type = typeof (Slider))]
[TemplatePart(Name = "PART_Track", Type = typeof (Track))]
public class RangeSlider : Control
{
  private const string PART_LowerRange = "PART_LowerRange";
  private const string PART_Range = "PART_Range";
  private const string PART_HigherRange = "PART_HigherRange";
  private const string PART_HigherSlider = "PART_HigherSlider";
  private const string PART_LowerSlider = "PART_LowerSlider";
  private const string PART_Track = "PART_Track";
  private RepeatButton _lowerRange;
  private RepeatButton _higherRange;
  private Slider _lowerSlider;
  private Slider _higherSlider;
  private Track _lowerTrack;
  private Track _higherTrack;
  private double _deferredUpdateValue;
  public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register(nameof (AutoToolTipPlacement), typeof (AutoToolTipPlacement), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) AutoToolTipPlacement.None, new PropertyChangedCallback(RangeSlider.OnAutoToolTipPlacementChanged)));
  public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(nameof (AutoToolTipPrecision), typeof (int), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0));
  public static readonly DependencyProperty HigherRangeBackgroundProperty = DependencyProperty.Register(nameof (HigherRangeBackground), typeof (Brush), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Transparent));
  public static readonly DependencyProperty HigherRangeStyleProperty = DependencyProperty.Register(nameof (HigherRangeStyle), typeof (Style), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  private static readonly DependencyPropertyKey HigherRangeWidthPropertyKey = DependencyProperty.RegisterAttachedReadOnly(nameof (HigherRangeWidth), typeof (double), typeof (RangeSlider), new PropertyMetadata((object) 0.0));
  public static readonly DependencyProperty HigherRangeWidthProperty = RangeSlider.HigherRangeWidthPropertyKey.DependencyProperty;
  public static readonly DependencyProperty HigherThumbBackgroundProperty = DependencyProperty.Register(nameof (HigherThumbBackground), typeof (Brush), typeof (RangeSlider));
  public static readonly DependencyProperty HigherValueProperty = DependencyProperty.Register(nameof (HigherValue), typeof (double), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(RangeSlider.OnHigherValueChanged), new CoerceValueCallback(RangeSlider.OnCoerceHigherValueChanged)));
  public static readonly DependencyProperty IsDeferredUpdateValuesProperty = DependencyProperty.Register(nameof (IsDeferredUpdateValues), typeof (bool), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register(nameof (IsSnapToTickEnabled), typeof (bool), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty LowerRangeBackgroundProperty = DependencyProperty.Register(nameof (LowerRangeBackground), typeof (Brush), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Transparent));
  public static readonly DependencyProperty LowerRangeStyleProperty = DependencyProperty.Register(nameof (LowerRangeStyle), typeof (Style), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  private static DependencyPropertyKey LowerRangeWidthPropertyKey = DependencyProperty.RegisterAttachedReadOnly(nameof (LowerRangeWidth), typeof (double), typeof (RangeSlider), new PropertyMetadata((object) 0.0));
  public static readonly DependencyProperty LowerRangeWidthProperty = RangeSlider.LowerRangeWidthPropertyKey.DependencyProperty;
  public static readonly DependencyProperty LowerThumbBackgroundProperty = DependencyProperty.Register(nameof (LowerThumbBackground), typeof (Brush), typeof (RangeSlider));
  public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register(nameof (LowerValue), typeof (double), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(RangeSlider.OnLowerValueChanged), new CoerceValueCallback(RangeSlider.OnCoerceLowerValueChanged)));
  public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof (Maximum), typeof (double), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(RangeSlider.OnMaximumChanged)));
  public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof (Minimum), typeof (double), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(RangeSlider.OnMinimumChanged)));
  public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Horizontal, new PropertyChangedCallback(RangeSlider.OnOrientationChanged)));
  public static readonly DependencyProperty RangeBackgroundProperty = DependencyProperty.Register(nameof (RangeBackground), typeof (Brush), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Transparent));
  public static readonly DependencyProperty RangeStyleProperty = DependencyProperty.Register(nameof (RangeStyle), typeof (Style), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  private static readonly DependencyPropertyKey RangeWidthPropertyKey = DependencyProperty.RegisterAttachedReadOnly(nameof (RangeWidth), typeof (double), typeof (RangeSlider), new PropertyMetadata((object) 0.0));
  public static readonly DependencyProperty RangeWidthProperty = RangeSlider.RangeWidthPropertyKey.DependencyProperty;
  private static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof (Step), typeof (double), typeof (RangeSlider), new PropertyMetadata((object) 1.0, (PropertyChangedCallback) null, new CoerceValueCallback(RangeSlider.CoerceStep)));
  public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(nameof (TickFrequency), typeof (double), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) 1.0, new PropertyChangedCallback(RangeSlider.OnTickFrequencyChanged)));
  public static readonly DependencyProperty TickPlacementProperty = DependencyProperty.Register(nameof (TickPlacement), typeof (TickPlacement), typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) TickPlacement.None, new PropertyChangedCallback(RangeSlider.OnTickPlacementChanged)));
  public static readonly RoutedEvent LowerValueChangedEvent = EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (RangeSlider));
  public static readonly RoutedEvent HigherValueChangedEvent = EventManager.RegisterRoutedEvent("HigherValueChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (RangeSlider));

  static RangeSlider()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (RangeSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (RangeSlider)));
  }

  public RangeSlider()
  {
    this.SizeChanged += new SizeChangedEventHandler(this.RangeSlider_SizeChanged);
  }

  public AutoToolTipPlacement AutoToolTipPlacement
  {
    get => (AutoToolTipPlacement) this.GetValue(RangeSlider.AutoToolTipPlacementProperty);
    set => this.SetValue(RangeSlider.AutoToolTipPlacementProperty, (object) value);
  }

  private static void OnAutoToolTipPlacementChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnAutoToolTipPlacementChanged((AutoToolTipPlacement) e.OldValue, (AutoToolTipPlacement) e.NewValue);
  }

  protected virtual void OnAutoToolTipPlacementChanged(
    AutoToolTipPlacement oldValue,
    AutoToolTipPlacement newValue)
  {
  }

  public int AutoToolTipPrecision
  {
    get => (int) this.GetValue(RangeSlider.AutoToolTipPrecisionProperty);
    set => this.SetValue(RangeSlider.AutoToolTipPrecisionProperty, (object) value);
  }

  public Brush HigherRangeBackground
  {
    get => (Brush) this.GetValue(RangeSlider.HigherRangeBackgroundProperty);
    set => this.SetValue(RangeSlider.HigherRangeBackgroundProperty, (object) value);
  }

  public Style HigherRangeStyle
  {
    get => (Style) this.GetValue(RangeSlider.HigherRangeStyleProperty);
    set => this.SetValue(RangeSlider.HigherRangeStyleProperty, (object) value);
  }

  public double HigherRangeWidth
  {
    get => (double) this.GetValue(RangeSlider.HigherRangeWidthProperty);
    private set => this.SetValue(RangeSlider.HigherRangeWidthPropertyKey, (object) value);
  }

  public Brush HigherThumbBackground
  {
    get => (Brush) this.GetValue(RangeSlider.HigherThumbBackgroundProperty);
    set => this.SetValue(RangeSlider.HigherThumbBackgroundProperty, (object) value);
  }

  public double HigherValue
  {
    get => (double) this.GetValue(RangeSlider.HigherValueProperty);
    set => this.SetValue(RangeSlider.HigherValueProperty, (object) value);
  }

  private static object OnCoerceHigherValueChanged(DependencyObject d, object basevalue)
  {
    RangeSlider rangeSlider = (RangeSlider) d;
    if (rangeSlider == null || !rangeSlider.IsLoaded)
      return basevalue;
    Math.Min(rangeSlider.Minimum, rangeSlider.Maximum);
    Math.Max(rangeSlider.Minimum, rangeSlider.Maximum);
    Math.Max(rangeSlider.Minimum, Math.Min(rangeSlider.Maximum, (double) basevalue));
    return (object) Math.Max(rangeSlider.LowerValue, (double) basevalue);
  }

  private static void OnHigherValueChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnHigherValueChanged((double) args.OldValue, (double) args.NewValue);
  }

  protected virtual void OnHigherValueChanged(double oldValue, double newValue)
  {
    this.AdjustView();
    this.RaiseEvent(new RoutedEventArgs()
    {
      RoutedEvent = RangeSlider.HigherValueChangedEvent
    });
  }

  public bool IsDeferredUpdateValues
  {
    get => (bool) this.GetValue(RangeSlider.IsDeferredUpdateValuesProperty);
    set => this.SetValue(RangeSlider.IsDeferredUpdateValuesProperty, (object) value);
  }

  public bool IsSnapToTickEnabled
  {
    get => (bool) this.GetValue(RangeSlider.IsSnapToTickEnabledProperty);
    set => this.SetValue(RangeSlider.IsSnapToTickEnabledProperty, (object) value);
  }

  public Brush LowerRangeBackground
  {
    get => (Brush) this.GetValue(RangeSlider.LowerRangeBackgroundProperty);
    set => this.SetValue(RangeSlider.LowerRangeBackgroundProperty, (object) value);
  }

  public Style LowerRangeStyle
  {
    get => (Style) this.GetValue(RangeSlider.LowerRangeStyleProperty);
    set => this.SetValue(RangeSlider.LowerRangeStyleProperty, (object) value);
  }

  public double LowerRangeWidth
  {
    get => (double) this.GetValue(RangeSlider.LowerRangeWidthProperty);
    private set => this.SetValue(RangeSlider.LowerRangeWidthPropertyKey, (object) value);
  }

  public Brush LowerThumbBackground
  {
    get => (Brush) this.GetValue(RangeSlider.LowerThumbBackgroundProperty);
    set => this.SetValue(RangeSlider.LowerThumbBackgroundProperty, (object) value);
  }

  public double LowerValue
  {
    get => (double) this.GetValue(RangeSlider.LowerValueProperty);
    set => this.SetValue(RangeSlider.LowerValueProperty, (object) value);
  }

  private static object OnCoerceLowerValueChanged(DependencyObject d, object basevalue)
  {
    RangeSlider rangeSlider = (RangeSlider) d;
    if (rangeSlider == null || !rangeSlider.IsLoaded)
      return basevalue;
    Math.Min(rangeSlider.Minimum, rangeSlider.Maximum);
    Math.Max(rangeSlider.Minimum, rangeSlider.Maximum);
    Math.Max(rangeSlider.Minimum, Math.Min(rangeSlider.Maximum, (double) basevalue));
    return (object) Math.Min((double) basevalue, rangeSlider.HigherValue);
  }

  private static void OnLowerValueChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnLowerValueChanged((double) args.OldValue, (double) args.NewValue);
  }

  protected virtual void OnLowerValueChanged(double oldValue, double newValue)
  {
    this.AdjustView();
    this.RaiseEvent(new RoutedEventArgs()
    {
      RoutedEvent = RangeSlider.LowerValueChangedEvent
    });
  }

  public double Maximum
  {
    get => (double) this.GetValue(RangeSlider.MaximumProperty);
    set => this.SetValue(RangeSlider.MaximumProperty, (object) value);
  }

  private static void OnMaximumChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnMaximumChanged((double) args.OldValue, (double) args.NewValue);
  }

  protected virtual void OnMaximumChanged(double oldValue, double newValue) => this.AdjustView();

  public double Minimum
  {
    get => (double) this.GetValue(RangeSlider.MinimumProperty);
    set => this.SetValue(RangeSlider.MinimumProperty, (object) value);
  }

  private static void OnMinimumChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnMinimumChanged((double) args.OldValue, (double) args.NewValue);
  }

  protected virtual void OnMinimumChanged(double oldValue, double newValue) => this.AdjustView();

  public Orientation Orientation
  {
    get => (Orientation) this.GetValue(RangeSlider.OrientationProperty);
    set => this.SetValue(RangeSlider.OrientationProperty, (object) value);
  }

  private static void OnOrientationChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnOrientationChanged((Orientation) e.OldValue, (Orientation) e.NewValue);
  }

  protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue)
  {
  }

  public Brush RangeBackground
  {
    get => (Brush) this.GetValue(RangeSlider.RangeBackgroundProperty);
    set => this.SetValue(RangeSlider.RangeBackgroundProperty, (object) value);
  }

  public Style RangeStyle
  {
    get => (Style) this.GetValue(RangeSlider.RangeStyleProperty);
    set => this.SetValue(RangeSlider.RangeStyleProperty, (object) value);
  }

  public double RangeWidth
  {
    get => (double) this.GetValue(RangeSlider.RangeWidthProperty);
    private set => this.SetValue(RangeSlider.RangeWidthPropertyKey, (object) value);
  }

  public double Step
  {
    get => (double) this.GetValue(RangeSlider.StepProperty);
    set => this.SetValue(RangeSlider.StepProperty, (object) value);
  }

  private static object CoerceStep(DependencyObject sender, object value)
  {
    return (object) Math.Max(0.01, (double) value);
  }

  public double TickFrequency
  {
    get => (double) this.GetValue(RangeSlider.TickFrequencyProperty);
    set => this.SetValue(RangeSlider.TickFrequencyProperty, (object) value);
  }

  private static void OnTickFrequencyChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnTickFrequencyChanged((double) args.OldValue, (double) args.NewValue);
  }

  protected virtual void OnTickFrequencyChanged(double oldValue, double newValue)
  {
  }

  public TickPlacement TickPlacement
  {
    get => (TickPlacement) this.GetValue(RangeSlider.TickPlacementProperty);
    set => this.SetValue(RangeSlider.TickPlacementProperty, (object) value);
  }

  private static void OnTickPlacementChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is RangeSlider rangeSlider))
      return;
    rangeSlider.OnTickPlacementChanged((TickPlacement) e.OldValue, (TickPlacement) e.NewValue);
  }

  protected virtual void OnTickPlacementChanged(TickPlacement oldValue, TickPlacement newValue)
  {
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._lowerRange != null)
      this._lowerRange.Click -= new RoutedEventHandler(this.LowerRange_Click);
    this._lowerRange = this.Template.FindName("PART_LowerRange", (FrameworkElement) this) as RepeatButton;
    if (this._lowerRange != null)
      this._lowerRange.Click += new RoutedEventHandler(this.LowerRange_Click);
    if (this._higherRange != null)
      this._higherRange.Click -= new RoutedEventHandler(this.HigherRange_Click);
    this._higherRange = this.Template.FindName("PART_HigherRange", (FrameworkElement) this) as RepeatButton;
    if (this._higherRange != null)
      this._higherRange.Click += new RoutedEventHandler(this.HigherRange_Click);
    if (this._lowerSlider != null)
    {
      this._lowerSlider.Loaded -= new RoutedEventHandler(this.Slider_Loaded);
      this._lowerSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.LowerSlider_ValueChanged);
      if (this._lowerTrack != null)
        this._lowerTrack.Thumb.DragCompleted -= new DragCompletedEventHandler(this.LowerSlider_DragCompleted);
    }
    this._lowerSlider = this.Template.FindName("PART_LowerSlider", (FrameworkElement) this) as Slider;
    if (this._lowerSlider != null)
    {
      this._lowerSlider.Loaded += new RoutedEventHandler(this.Slider_Loaded);
      this._lowerSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.LowerSlider_ValueChanged);
      this._lowerSlider.ApplyTemplate();
      this._lowerTrack = this._lowerSlider.Template.FindName("PART_Track", (FrameworkElement) this._lowerSlider) as Track;
      if (this._lowerTrack != null)
        this._lowerTrack.Thumb.DragCompleted += new DragCompletedEventHandler(this.LowerSlider_DragCompleted);
    }
    if (this._higherSlider != null)
    {
      this._higherSlider.Loaded -= new RoutedEventHandler(this.Slider_Loaded);
      this._higherSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.HigherSlider_ValueChanged);
      if (this._higherTrack != null)
        this._higherTrack.Thumb.DragCompleted -= new DragCompletedEventHandler(this.HigherSlider_DragCompleted);
    }
    this._higherSlider = this.Template.FindName("PART_HigherSlider", (FrameworkElement) this) as Slider;
    if (this._higherSlider == null)
      return;
    this._higherSlider.Loaded += new RoutedEventHandler(this.Slider_Loaded);
    this._higherSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.HigherSlider_ValueChanged);
    this._higherSlider.ApplyTemplate();
    this._higherTrack = this._higherSlider.Template.FindName("PART_Track", (FrameworkElement) this._higherSlider) as Track;
    if (this._higherTrack == null)
      return;
    this._higherTrack.Thumb.DragCompleted += new DragCompletedEventHandler(this.HigherSlider_DragCompleted);
  }

  public override string ToString()
  {
    double num = this.LowerValue;
    string str1 = num.ToString();
    num = this.HigherValue;
    string str2 = num.ToString();
    return $"{str1}-{str2}";
  }

  internal static double GetThumbWidth(Slider slider)
  {
    if (slider != null)
    {
      Track name = (Track) slider.Template.FindName("PART_Track", (FrameworkElement) slider);
      if (name != null)
        return name.Thumb.ActualWidth;
    }
    return 0.0;
  }

  internal static double GetThumbHeight(Slider slider)
  {
    if (slider != null)
    {
      Track name = (Track) slider.Template.FindName("PART_Track", (FrameworkElement) slider);
      if (name != null)
        return name.Thumb.ActualHeight;
    }
    return 0.0;
  }

  private void AdjustView()
  {
    RangeSlider.CoercedValues coercedValues = this.GetCoercedValues();
    double num1 = 0.0;
    double num2 = 0.0;
    double num3 = 0.0;
    if (this.Orientation == Orientation.Horizontal)
    {
      num1 = this.ActualWidth;
      num2 = RangeSlider.GetThumbWidth(this._lowerSlider);
      num3 = RangeSlider.GetThumbWidth(this._higherSlider);
    }
    else if (this.Orientation == Orientation.Vertical)
    {
      num1 = this.ActualHeight;
      num2 = RangeSlider.GetThumbHeight(this._lowerSlider);
      num3 = RangeSlider.GetThumbHeight(this._higherSlider);
    }
    double num4 = num1 - (num2 + num3);
    this.SetLowerSliderValues(coercedValues.LowerValue, new double?(coercedValues.Minimum), new double?(coercedValues.Maximum));
    this.SetHigherSliderValues(coercedValues.HigherValue, new double?(coercedValues.Minimum), new double?(coercedValues.Maximum));
    double num5 = coercedValues.Maximum - coercedValues.Minimum;
    if (num5 > 0.0)
    {
      this.HigherRangeWidth = num4 * (coercedValues.Maximum - coercedValues.HigherValue) / num5;
      this.RangeWidth = num4 * (coercedValues.HigherValue - coercedValues.LowerValue) / num5;
      this.LowerRangeWidth = num4 * (coercedValues.LowerValue - coercedValues.Minimum) / num5;
    }
    else
    {
      this.HigherRangeWidth = 0.0;
      this.RangeWidth = 0.0;
      this.LowerRangeWidth = num4;
    }
  }

  private void SetSlidersMargins()
  {
    if (this._lowerSlider == null || this._higherSlider == null)
      return;
    if (this.Orientation == Orientation.Horizontal)
    {
      double thumbWidth1 = RangeSlider.GetThumbWidth(this._lowerSlider);
      double thumbWidth2 = RangeSlider.GetThumbWidth(this._higherSlider);
      this._higherSlider.Margin = new Thickness(thumbWidth1, 0.0, 0.0, 0.0);
      this._lowerSlider.Margin = new Thickness(0.0, 0.0, thumbWidth2, 0.0);
    }
    else
    {
      double thumbHeight1 = RangeSlider.GetThumbHeight(this._lowerSlider);
      double thumbHeight2 = RangeSlider.GetThumbHeight(this._higherSlider);
      this._higherSlider.Margin = new Thickness(0.0, 0.0, 0.0, thumbHeight1);
      this._lowerSlider.Margin = new Thickness(0.0, thumbHeight2, 0.0, 0.0);
    }
  }

  private RangeSlider.CoercedValues GetCoercedValues()
  {
    RangeSlider.CoercedValues coercedValues = new RangeSlider.CoercedValues()
    {
      Minimum = Math.Min(this.Minimum, this.Maximum)
    };
    coercedValues.Maximum = Math.Max(coercedValues.Minimum, this.Maximum);
    coercedValues.LowerValue = Math.Max(coercedValues.Minimum, Math.Min(coercedValues.Maximum, this.LowerValue));
    coercedValues.HigherValue = Math.Max(coercedValues.Minimum, Math.Min(coercedValues.Maximum, this.HigherValue));
    coercedValues.HigherValue = Math.Max(coercedValues.LowerValue, coercedValues.HigherValue);
    return coercedValues;
  }

  private void SetLowerSliderValues(double value, double? minimum, double? maximum)
  {
    this.SetSliderValues(this._lowerSlider, new RoutedPropertyChangedEventHandler<double>(this.LowerSlider_ValueChanged), value, minimum, maximum);
  }

  private void SetHigherSliderValues(double value, double? minimum, double? maximum)
  {
    this.SetSliderValues(this._higherSlider, new RoutedPropertyChangedEventHandler<double>(this.HigherSlider_ValueChanged), value, minimum, maximum);
  }

  private void SetSliderValues(
    Slider slider,
    RoutedPropertyChangedEventHandler<double> handler,
    double value,
    double? minimum,
    double? maximum)
  {
    if (slider == null)
      return;
    slider.ValueChanged -= handler;
    slider.Value = value;
    if (minimum.HasValue)
      slider.Minimum = minimum.Value;
    if (maximum.HasValue)
      slider.Maximum = maximum.Value;
    slider.ValueChanged += handler;
  }

  private void UpdateHigherValue(double value)
  {
    RangeSlider.CoercedValues coercedValues = this.GetCoercedValues();
    double num = Math.Max(Math.Max(coercedValues.Minimum, Math.Min(coercedValues.Maximum, value)), coercedValues.LowerValue);
    this.SetHigherSliderValues(num, new double?(), new double?());
    this.HigherValue = num;
  }

  private void UpdateLowerValue(double value)
  {
    RangeSlider.CoercedValues coercedValues = this.GetCoercedValues();
    double num = Math.Min(Math.Max(coercedValues.Minimum, Math.Min(coercedValues.Maximum, value)), coercedValues.HigherValue);
    this.SetLowerSliderValues(num, new double?(), new double?());
    this.LowerValue = num;
  }

  public event RoutedEventHandler LowerValueChanged
  {
    add => this.AddHandler(RangeSlider.LowerValueChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(RangeSlider.LowerValueChangedEvent, (Delegate) value);
  }

  public event RoutedEventHandler HigherValueChanged
  {
    add => this.AddHandler(RangeSlider.HigherValueChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(RangeSlider.HigherValueChangedEvent, (Delegate) value);
  }

  private void LowerRange_Click(object sender, RoutedEventArgs e)
  {
    RangeSlider.CoercedValues coercedValues = this.GetCoercedValues();
    if (coercedValues.Minimum >= coercedValues.Maximum)
      return;
    double val2 = coercedValues.LowerValue - this.Step;
    this.LowerValue = Math.Min(coercedValues.Maximum, Math.Max(coercedValues.Minimum, val2));
  }

  private void HigherRange_Click(object sender, RoutedEventArgs e)
  {
    RangeSlider.CoercedValues coercedValues = this.GetCoercedValues();
    if (coercedValues.Minimum >= coercedValues.Maximum)
      return;
    double val2 = coercedValues.HigherValue + this.Step;
    this.HigherValue = Math.Min(coercedValues.Maximum, Math.Max(coercedValues.Minimum, val2));
  }

  private void RangeSlider_SizeChanged(object sender, SizeChangedEventArgs e) => this.AdjustView();

  private void Slider_Loaded(object sender, RoutedEventArgs e)
  {
    this.SetSlidersMargins();
    this.AdjustView();
  }

  private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
  {
    if (this._lowerSlider == null || !this._lowerSlider.IsLoaded)
      return;
    if (!this.IsDeferredUpdateValues)
      this.UpdateLowerValue(e.NewValue);
    else
      this._deferredUpdateValue = e.NewValue;
  }

  private void HigherSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
  {
    if (this._higherSlider == null || !this._higherSlider.IsLoaded)
      return;
    if (!this.IsDeferredUpdateValues)
      this.UpdateHigherValue(e.NewValue);
    else
      this._deferredUpdateValue = e.NewValue;
  }

  private void HigherSlider_DragCompleted(object sender, DragCompletedEventArgs e)
  {
    if (!this.IsDeferredUpdateValues)
      return;
    this.UpdateHigherValue(this._deferredUpdateValue);
  }

  private void LowerSlider_DragCompleted(object sender, DragCompletedEventArgs e)
  {
    if (!this.IsDeferredUpdateValues)
      return;
    this.UpdateLowerValue(this._deferredUpdateValue);
  }

  private struct CoercedValues
  {
    public double Minimum;
    public double Maximum;
    public double LowerValue;
    public double HigherValue;
  }
}
