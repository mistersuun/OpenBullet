// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Zoombox.Zoombox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Zoombox;

[TemplatePart(Name = "PART_VerticalScrollBar", Type = typeof (ScrollBar))]
[TemplatePart(Name = "PART_HorizontalScrollBar", Type = typeof (ScrollBar))]
public sealed class Zoombox : ContentControl
{
  private const string PART_VerticalScrollBar = "PART_VerticalScrollBar";
  private const string PART_HorizontalScrollBar = "PART_HorizontalScrollBar";
  private bool _isUpdatingVisualTree;
  public static readonly DependencyProperty AnimationAccelerationRatioProperty = DependencyProperty.Register(nameof (AnimationAccelerationRatio), typeof (double), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0), new ValidateValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ValidateAccelerationRatio));
  public static readonly DependencyProperty AnimationDecelerationRatioProperty = DependencyProperty.Register(nameof (AnimationDecelerationRatio), typeof (double), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0), new ValidateValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ValidateDecelerationRatio));
  public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof (AnimationDuration), typeof (Duration), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Duration(TimeSpan.FromMilliseconds(300.0))));
  private static readonly DependencyPropertyKey AreDragModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof (AreDragModifiersActive), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty AreDragModifiersActiveProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreDragModifiersActivePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey AreRelativeZoomModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof (AreRelativeZoomModifiersActive), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty AreRelativeZoomModifiersActiveProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreRelativeZoomModifiersActivePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey AreZoomModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof (AreZoomModifiersActive), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty AreZoomModifiersActiveProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreZoomModifiersActivePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey AreZoomToSelectionModifiersActivePropertyKey = DependencyProperty.RegisterReadOnly(nameof (AreZoomToSelectionModifiersActive), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty AreZoomToSelectionModifiersActiveProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreZoomToSelectionModifiersActivePropertyKey.DependencyProperty;
  public static readonly DependencyProperty AutoWrapContentWithViewboxProperty = DependencyProperty.Register(nameof (AutoWrapContentWithViewbox), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnAutoWrapContentWithViewboxChanged)));
  private UIElement _trueContent;
  private static readonly DependencyPropertyKey CurrentViewPropertyKey = DependencyProperty.RegisterReadOnly(nameof (CurrentView), typeof (ZoomboxView), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) ZoomboxView.Empty, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnCurrentViewChanged)));
  public static readonly DependencyProperty CurrentViewProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey CurrentViewIndexPropertyKey = DependencyProperty.RegisterReadOnly(nameof (CurrentViewIndex), typeof (int), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1));
  public static readonly DependencyProperty CurrentViewIndexProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewIndexPropertyKey.DependencyProperty;
  public static readonly DependencyProperty DragModifiersProperty = DependencyProperty.Register(nameof (DragModifiers), typeof (KeyModifierCollection), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.GetDefaultDragModifiers()));
  public static readonly DependencyProperty DragOnPreviewProperty = DependencyProperty.Register(nameof (DragOnPreview), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  private static readonly DependencyPropertyKey EffectiveViewStackModePropertyKey = DependencyProperty.RegisterReadOnly(nameof (EffectiveViewStackMode), typeof (ZoomboxViewStackMode), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) ZoomboxViewStackMode.Auto));
  public static readonly DependencyProperty EffectiveViewStackModeProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.EffectiveViewStackModePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey HasBackStackPropertyKey = DependencyProperty.RegisterReadOnly(nameof (HasBackStack), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty HasBackStackProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.HasBackStackPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey HasForwardStackPropertyKey = DependencyProperty.RegisterReadOnly(nameof (HasForwardStack), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty HasForwardStackProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.HasForwardStackPropertyKey.DependencyProperty;
  public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register(nameof (IsAnimated), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, (PropertyChangedCallback) null, new CoerceValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CoerceIsAnimatedValue)));
  private static readonly DependencyPropertyKey IsDraggingContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsDraggingContent), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsDraggingContentProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsDraggingContentPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsSelectingRegionPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsSelectingRegion), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsSelectingRegionProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsSelectingRegionPropertyKey.DependencyProperty;
  public static readonly DependencyProperty IsUsingScrollBarsProperty = DependencyProperty.Register(nameof (IsUsingScrollBars), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, (PropertyChangedCallback) null));
  public static readonly DependencyProperty MaxScaleProperty = DependencyProperty.Register(nameof (MaxScale), typeof (double), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 100.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnMaxScaleChanged), new CoerceValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CoerceMaxScaleValue)));
  public static readonly DependencyProperty MinScaleProperty = DependencyProperty.Register(nameof (MinScale), typeof (double), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.01, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnMinScaleChanged), new CoerceValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CoerceMinScaleValue)));
  public static readonly DependencyProperty NavigateOnPreviewProperty = DependencyProperty.Register(nameof (NavigateOnPreview), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty PanDistanceProperty = DependencyProperty.Register(nameof (PanDistance), typeof (double), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 5.0));
  public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof (Position), typeof (Point), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) PointHelper.Empty, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnPositionChanged)));
  public static readonly DependencyProperty RelativeZoomModifiersProperty = DependencyProperty.Register(nameof (RelativeZoomModifiers), typeof (KeyModifierCollection), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.GetDefaultRelativeZoomModifiers()));
  public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof (Scale), typeof (double), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnScaleChanged), new CoerceValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CoerceScaleValue)));
  private static readonly DependencyPropertyKey ViewFinderPropertyKey = DependencyProperty.RegisterReadOnly(nameof (ViewFinder), typeof (FrameworkElement), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnViewFinderChanged)));
  public static readonly DependencyProperty ViewFinderProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderPropertyKey.DependencyProperty;
  public static readonly DependencyProperty ViewFinderVisibilityProperty = DependencyProperty.RegisterAttached("ViewFinderVisibility", typeof (Visibility), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Visible));
  private static readonly DependencyPropertyKey ViewportPropertyKey = DependencyProperty.RegisterReadOnly(nameof (Viewport), typeof (Rect), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) Rect.Empty, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnViewportChanged)));
  public static readonly DependencyProperty ViewportProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewportPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey ViewStackCountPropertyKey = DependencyProperty.RegisterReadOnly(nameof (ViewStackCount), typeof (int), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnViewStackCountChanged)));
  public static readonly DependencyProperty ViewStackCountProperty = Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackCountPropertyKey.DependencyProperty;
  public static readonly DependencyProperty ViewStackIndexProperty = DependencyProperty.Register(nameof (ViewStackIndex), typeof (int), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnViewStackIndexChanged), new CoerceValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CoerceViewStackIndexValue)));
  public static readonly DependencyProperty ViewStackModeProperty = DependencyProperty.Register(nameof (ViewStackMode), typeof (ZoomboxViewStackMode), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) ZoomboxViewStackMode.Default, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnViewStackModeChanged), new CoerceValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CoerceViewStackModeValue)));
  public static readonly DependencyProperty ViewStackSourceProperty = DependencyProperty.Register(nameof (ViewStackSource), typeof (IEnumerable), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnViewStackSourceChanged)));
  public static readonly DependencyProperty ZoomModifiersProperty = DependencyProperty.Register(nameof (ZoomModifiers), typeof (KeyModifierCollection), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.GetDefaultZoomModifiers()));
  public static readonly DependencyProperty ZoomOnPreviewProperty = DependencyProperty.Register(nameof (ZoomOnPreview), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  public static readonly DependencyProperty ZoomOriginProperty = DependencyProperty.Register(nameof (ZoomOrigin), typeof (Point), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Point(0.5, 0.5)));
  public static readonly DependencyProperty ZoomPercentageProperty = DependencyProperty.Register(nameof (ZoomPercentage), typeof (double), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) 5.0));
  public static readonly DependencyProperty ZoomOnProperty = DependencyProperty.Register(nameof (ZoomOn), typeof (ZoomboxZoomOn), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) ZoomboxZoomOn.Content));
  public static readonly DependencyProperty ZoomToSelectionModifiersProperty = DependencyProperty.Register(nameof (ZoomToSelectionModifiers), typeof (KeyModifierCollection), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.GetDefaultZoomToSelectionModifiers()));
  public static readonly DependencyProperty KeepContentInBoundsProperty = DependencyProperty.Register(nameof (KeepContentInBounds), typeof (bool), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.OnKeepContentInBoundsChanged)));
  public static readonly RoutedEvent AnimationBeginningEvent = EventManager.RegisterRoutedEvent("AnimationBeginning", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static readonly RoutedEvent AnimationCompletedEvent = EventManager.RegisterRoutedEvent("AnimationCompleted", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static readonly RoutedEvent CurrentViewChangedEvent = EventManager.RegisterRoutedEvent("CurrentViewChanged", RoutingStrategy.Bubble, typeof (ZoomboxViewChangedEventHandler), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static readonly RoutedEvent ViewStackIndexChangedEvent = EventManager.RegisterRoutedEvent("ViewStackIndexChanged", RoutingStrategy.Bubble, typeof (IndexChangedEventHandler), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand Back = new RoutedUICommand("Go Back", "GoBack", typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand Center = new RoutedUICommand("Center Content", nameof (Center), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand Fill = new RoutedUICommand("Fill Bounds with Content", "FillToBounds", typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand Fit = new RoutedUICommand("Fit Content within Bounds", "FitToBounds", typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand Forward = new RoutedUICommand("Go Forward", "GoForward", typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand Home = new RoutedUICommand("Go Home", "GoHome", typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand PanDown = new RoutedUICommand("Pan Down", nameof (PanDown), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand PanLeft = new RoutedUICommand("Pan Left", nameof (PanLeft), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand PanRight = new RoutedUICommand("Pan Right", nameof (PanRight), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand PanUp = new RoutedUICommand("Pan Up", nameof (PanUp), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand Refocus = new RoutedUICommand("Refocus View", nameof (Refocus), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand ZoomIn = new RoutedUICommand("Zoom In", nameof (ZoomIn), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  public static RoutedUICommand ZoomOut = new RoutedUICommand("Zoom Out", nameof (ZoomOut), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox));
  private static int MOUSE_WHEEL_DELTA = 28;
  private ContentPresenter _contentPresenter;
  private ScrollBar _verticalScrollBar;
  private ScrollBar _horizontalScrollBar;
  private UIElement _content;
  private Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner _dragAdorner;
  private ZoomboxViewStack _viewStack;
  private ZoomboxViewFinderDisplay _viewFinderDisplay;
  private Rect _resizeViewportBounds = Rect.Empty;
  private Point _resizeAnchorPoint = new Point(0.0, 0.0);
  private Point _resizeDraggingPoint = new Point(0.0, 0.0);
  private Point _originPoint = new Point(0.0, 0.0);
  private double _viewboxFactor = 1.0;
  private double _relativeScale = 1.0;
  private Point _relativePosition;
  private Point _basePosition;
  private DateTime _lastStackAddition;
  private int _lastViewIndex = -1;
  private BitVector32 _cacheBits = new BitVector32(0);

  static Zoombox()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox)));
    UIElement.ClipToBoundsProperty.OverrideMetadata(typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    UIElement.FocusableProperty.OverrideMetadata(typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    Control.HorizontalContentAlignmentProperty.OverrideMetadata(typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) HorizontalAlignment.Center, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.RefocusView)));
    Control.VerticalContentAlignmentProperty.OverrideMetadata(typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((object) VerticalAlignment.Center, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.RefocusView)));
    ContentControl.ContentProperty.OverrideMetadata(typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CoerceContentValue)));
  }

  public Zoombox()
  {
    try
    {
      new UIPermission(PermissionState.Unrestricted).Demand();
      this._cacheBits[512 /*0x0200*/] = true;
    }
    catch (SecurityException ex)
    {
    }
    this.InitCommands();
    this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
    this.AddHandler(FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnSizeChanged), true);
    this.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackModeProperty);
    this.Loaded += new RoutedEventHandler(this.Zoombox_Loaded);
  }

  public double AnimationAccelerationRatio
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationAccelerationRatioProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationAccelerationRatioProperty, (object) value);
  }

  private static bool ValidateAccelerationRatio(object value)
  {
    double num = (double) value;
    if (num < 0.0 || num > 1.0 || DoubleHelper.IsNaN(num))
      throw new ArgumentException(ErrorMessages.GetMessage("AnimationAccelerationRatioOOR"));
    return true;
  }

  public double AnimationDecelerationRatio
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationDecelerationRatioProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationDecelerationRatioProperty, (object) value);
  }

  private static bool ValidateDecelerationRatio(object value)
  {
    double num = (double) value;
    if (num < 0.0 || num > 1.0 || DoubleHelper.IsNaN(num))
      throw new ArgumentException(ErrorMessages.GetMessage("AnimationDecelerationRatioOOR"));
    return true;
  }

  public Duration AnimationDuration
  {
    get => (Duration) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationDurationProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationDurationProperty, (object) value);
  }

  public bool AreDragModifiersActive
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreDragModifiersActiveProperty);
  }

  private void SetAreDragModifiersActive(bool value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreDragModifiersActivePropertyKey, (object) value);
  }

  public bool AreRelativeZoomModifiersActive
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreRelativeZoomModifiersActiveProperty);
  }

  private void SetAreRelativeZoomModifiersActive(bool value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreRelativeZoomModifiersActivePropertyKey, (object) value);
  }

  public bool AreZoomModifiersActive
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreZoomModifiersActiveProperty);
  }

  private void SetAreZoomModifiersActive(bool value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreZoomModifiersActivePropertyKey, (object) value);
  }

  public bool AreZoomToSelectionModifiersActive
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreZoomToSelectionModifiersActiveProperty);
  }

  private void SetAreZoomToSelectionModifiersActive(bool value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AreZoomToSelectionModifiersActivePropertyKey, (object) value);
  }

  public bool AutoWrapContentWithViewbox
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AutoWrapContentWithViewboxProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AutoWrapContentWithViewboxProperty, (object) value);
  }

  private static void OnAutoWrapContentWithViewboxChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    o.CoerceValue(ContentControl.ContentProperty);
  }

  private static object CoerceContentValue(DependencyObject d, object value)
  {
    return ((Xceed.Wpf.Toolkit.Zoombox.Zoombox) d).CoerceContentValue(value);
  }

  private object CoerceContentValue(object value)
  {
    if (value != null && !(value is UIElement) && !(bool) this.GetValue(DesignerProperties.IsInDesignModeProperty))
      throw new InvalidContentException(ErrorMessages.GetMessage("ZoomboxContentMustBeUIElement"));
    UIElement content1 = this._content;
    if (value != this._trueContent || this.IsContentWrapped != this.AutoWrapContentWithViewbox)
    {
      if (this.IsContentWrapped && this._content is Viewbox && this._content != this._trueContent)
      {
        Viewbox content2 = (Viewbox) this._content;
        BindingOperations.ClearAllBindings((DependencyObject) content2);
        if (content2.Child is FrameworkElement)
          (content2.Child as FrameworkElement).RemoveHandler(FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnContentSizeChanged));
        content2.Child = (UIElement) null;
        this.RemoveLogicalChild((object) content2);
      }
      if (this._viewFinderDisplay != null && this._viewFinderDisplay.VisualBrush != null)
      {
        this._viewFinderDisplay.VisualBrush.Visual = (Visual) null;
        this._viewFinderDisplay.VisualBrush = (VisualBrush) null;
      }
      this._content = value as UIElement;
      this._trueContent = value as UIElement;
      if (this._contentPresenter != null && this._contentPresenter.Content != null)
        this._contentPresenter.Content = (object) null;
      this.IsContentWrapped = false;
      if (this.AutoWrapContentWithViewbox)
      {
        Viewbox child = new Viewbox();
        this.AddLogicalChild((object) child);
        child.Child = value as UIElement;
        this._content = (UIElement) child;
        child.HorizontalAlignment = HorizontalAlignment.Left;
        child.VerticalAlignment = VerticalAlignment.Top;
        this.IsContentWrapped = true;
      }
      if (this._content is Viewbox && this.IsContentWrapped && this._trueContent is FrameworkElement)
        (this._trueContent as FrameworkElement).AddHandler(FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnContentSizeChanged), true);
      if (this._contentPresenter != null)
        this._contentPresenter.Content = (object) this._content;
      if (this._viewFinderDisplay != null)
        this.CreateVisualBrushForViewFinder((Visual) this._content);
      this.UpdateViewFinderDisplayContentBounds();
    }
    UIElement content3 = this._content;
    if (content1 != content3 && this.HasArrangedContentPresenter && this.HasRenderedFirstView)
    {
      this.HasArrangedContentPresenter = false;
      this.HasRenderedFirstView = false;
      this.RefocusViewOnFirstRender = true;
      this._contentPresenter.LayoutUpdated += new EventHandler(this.ContentPresenterFirstArranged);
    }
    return (object) this._content;
  }

  public ZoomboxView CurrentView => (ZoomboxView) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewProperty);

  private void SetCurrentView(ZoomboxView value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewPropertyKey, (object) value);
  }

  private static void OnCurrentViewChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) o;
    if (!zoombox.IsUpdatingView)
      zoombox.ZoomTo(zoombox.CurrentView);
    zoombox.RaiseEvent((RoutedEventArgs) new ZoomboxViewChangedEventArgs(e.OldValue as ZoomboxView, e.NewValue as ZoomboxView, zoombox._lastViewIndex, zoombox.CurrentViewIndex));
  }

  public int CurrentViewIndex => (int) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewIndexProperty);

  internal void SetCurrentViewIndex(int value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewIndexPropertyKey, (object) value);
  }

  [TypeConverter(typeof (KeyModifierCollectionConverter))]
  public KeyModifierCollection DragModifiers
  {
    get => (KeyModifierCollection) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragModifiersProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragModifiersProperty, (object) value);
  }

  private static KeyModifierCollection GetDefaultDragModifiers()
  {
    KeyModifierCollection defaultDragModifiers = new KeyModifierCollection();
    defaultDragModifiers.Add(KeyModifier.Ctrl);
    defaultDragModifiers.Add(KeyModifier.Exact);
    return defaultDragModifiers;
  }

  public bool DragOnPreview
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragOnPreviewProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragOnPreviewProperty, (object) value);
  }

  public ZoomboxViewStackMode EffectiveViewStackMode
  {
    get => (ZoomboxViewStackMode) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.EffectiveViewStackModeProperty);
  }

  private void SetEffectiveViewStackMode(ZoomboxViewStackMode value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.EffectiveViewStackModePropertyKey, (object) value);
  }

  public bool HasBackStack => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.HasBackStackProperty);

  public bool HasForwardStack => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.HasForwardStackProperty);

  public bool IsAnimated
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsAnimatedProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsAnimatedProperty, (object) value);
  }

  private static object CoerceIsAnimatedValue(DependencyObject d, object value)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) d;
    bool flag = (bool) value;
    if (!zoombox.IsInitialized)
      flag = false;
    return (object) flag;
  }

  public bool IsDraggingContent => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsDraggingContentProperty);

  private void SetIsDraggingContent(bool value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsDraggingContentPropertyKey, (object) value);
  }

  public bool IsSelectingRegion => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsSelectingRegionProperty);

  private void SetIsSelectingRegion(bool value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsSelectingRegionPropertyKey, (object) value);
  }

  public bool IsUsingScrollBars
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsUsingScrollBarsProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsUsingScrollBarsProperty, (object) value);
  }

  public double MaxScale
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.MaxScaleProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.MaxScaleProperty, (object) value);
  }

  private static void OnMaxScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) o;
    zoombox.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.MinScaleProperty);
    zoombox.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ScaleProperty);
  }

  private static object CoerceMaxScaleValue(DependencyObject d, object value)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) d;
    double num = (double) value;
    if (num < zoombox.MinScale)
      num = zoombox.MinScale;
    return (object) num;
  }

  public double MinScale
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.MinScaleProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.MinScaleProperty, (object) value);
  }

  private static void OnMinScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) o;
    zoombox.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.MinScaleProperty);
    zoombox.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ScaleProperty);
  }

  private static object CoerceMinScaleValue(DependencyObject d, object value)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) d;
    double num = (double) value;
    if (num > zoombox.MaxScale)
      num = zoombox.MaxScale;
    return (object) num;
  }

  public bool NavigateOnPreview
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.NavigateOnPreviewProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.NavigateOnPreviewProperty, (object) value);
  }

  public double PanDistance
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.PanDistanceProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.PanDistanceProperty, (object) value);
  }

  public Point Position
  {
    get => (Point) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.PositionProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.PositionProperty, (object) value);
  }

  private static void OnPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) o;
    if (zoombox.IsUpdatingViewport)
      return;
    Point newValue = (Point) e.NewValue;
    if (zoombox.Scale <= 0.0)
      return;
    zoombox.ZoomTo(new Point(-newValue.X, -newValue.Y));
  }

  [TypeConverter(typeof (KeyModifierCollectionConverter))]
  public KeyModifierCollection RelativeZoomModifiers
  {
    get => (KeyModifierCollection) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.RelativeZoomModifiersProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.RelativeZoomModifiersProperty, (object) value);
  }

  private static KeyModifierCollection GetDefaultRelativeZoomModifiers()
  {
    KeyModifierCollection relativeZoomModifiers = new KeyModifierCollection();
    relativeZoomModifiers.Add(KeyModifier.Ctrl);
    relativeZoomModifiers.Add(KeyModifier.Alt);
    relativeZoomModifiers.Add(KeyModifier.Exact);
    return relativeZoomModifiers;
  }

  public double Scale
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ScaleProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ScaleProperty, (object) value);
  }

  private static void OnScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) o;
    if (zoombox.IsUpdatingView)
      return;
    double newValue = (double) e.NewValue;
    zoombox.ZoomTo(newValue);
  }

  private static object CoerceScaleValue(DependencyObject d, object value)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) d;
    double num = (double) value;
    if (num < zoombox.MinScale)
      num = zoombox.MinScale;
    if (num > zoombox.MaxScale)
      num = zoombox.MaxScale;
    return (object) num;
  }

  public FrameworkElement ViewFinder
  {
    get => (FrameworkElement) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderPropertyKey, (object) value);
  }

  private static void OnViewFinderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Xceed.Wpf.Toolkit.Zoombox.Zoombox) d).OnViewFinderChanged(e);
  }

  private void OnViewFinderChanged(DependencyPropertyChangedEventArgs e)
  {
    this.AttachToVisualTree();
  }

  public static Visibility GetViewFinderVisibility(DependencyObject d)
  {
    return (Visibility) d.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderVisibilityProperty);
  }

  public static void SetViewFinderVisibility(DependencyObject d, Visibility value)
  {
    d.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderVisibilityProperty, (object) value);
  }

  public Rect Viewport => (Rect) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewportProperty);

  private static void OnViewportChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox1 = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) o;
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox2 = zoombox1;
    Rect viewport = zoombox1.Viewport;
    double x = -viewport.Left * zoombox1.Scale / zoombox1._viewboxFactor;
    viewport = zoombox1.Viewport;
    double y = -viewport.Top * zoombox1.Scale / zoombox1._viewboxFactor;
    Point point = new Point(x, y);
    zoombox2.Position = point;
  }

  public int ViewStackCount => (int) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackCountProperty);

  internal void SetViewStackCount(int value)
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackCountPropertyKey, (object) value);
  }

  private static void OnViewStackCountChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Xceed.Wpf.Toolkit.Zoombox.Zoombox) d).OnViewStackCountChanged(e);
  }

  private void OnViewStackCountChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled)
      return;
    this.UpdateStackProperties();
  }

  public int ViewStackIndex
  {
    get => (int) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackIndexProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackIndexProperty, (object) value);
  }

  private static void OnViewStackIndexChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Xceed.Wpf.Toolkit.Zoombox.Zoombox) d).OnViewStackIndexChanged(e);
  }

  private void OnViewStackIndexChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled)
      return;
    if (!this.IsUpdatingView)
    {
      int viewStackIndex = this.ViewStackIndex;
      if (viewStackIndex >= 0 && viewStackIndex < this.ViewStack.Count)
        this.UpdateView(this.ViewStack[viewStackIndex], true, false, viewStackIndex);
    }
    this.UpdateStackProperties();
    this.RaiseEvent((RoutedEventArgs) new IndexChangedEventArgs(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackIndexChangedEvent, (int) e.OldValue, (int) e.NewValue));
  }

  private static object CoerceViewStackIndexValue(DependencyObject d, object value)
  {
    return (d as Xceed.Wpf.Toolkit.Zoombox.Zoombox).EffectiveViewStackMode != ZoomboxViewStackMode.Disabled ? value : (object) -1;
  }

  public ZoomboxViewStackMode ViewStackMode
  {
    get => (ZoomboxViewStackMode) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackModeProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackModeProperty, (object) value);
  }

  private static void OnViewStackModeChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Xceed.Wpf.Toolkit.Zoombox.Zoombox) d).OnViewStackModeChanged(e);
  }

  private void OnViewStackModeChanged(DependencyPropertyChangedEventArgs e)
  {
    if ((ZoomboxViewStackMode) e.NewValue != ZoomboxViewStackMode.Disabled || this._viewStack == null)
      return;
    this._viewStack.ClearViewStackSource();
    this._viewStack = (ZoomboxViewStack) null;
  }

  private static object CoerceViewStackModeValue(DependencyObject d, object value)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = d as Xceed.Wpf.Toolkit.Zoombox.Zoombox;
    ZoomboxViewStackMode zoomboxViewStackMode = (ZoomboxViewStackMode) value;
    if (zoombox.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled)
      zoombox.SetEffectiveViewStackMode(zoomboxViewStackMode);
    switch (zoomboxViewStackMode)
    {
      case ZoomboxViewStackMode.Default:
        zoomboxViewStackMode = zoombox.ViewStack.AreViewsFromSource ? ZoomboxViewStackMode.Manual : ZoomboxViewStackMode.Auto;
        goto default;
      case ZoomboxViewStackMode.Disabled:
        zoombox.SetEffectiveViewStackMode(zoomboxViewStackMode);
        return value;
      default:
        if (zoombox.ViewStack.AreViewsFromSource && zoomboxViewStackMode != ZoomboxViewStackMode.Manual)
          throw new InvalidOperationException(ErrorMessages.GetMessage("ViewModeInvalidForSource"));
        goto case ZoomboxViewStackMode.Disabled;
    }
  }

  [Bindable(true)]
  public IEnumerable ViewStackSource
  {
    get => this._viewStack != null ? this.ViewStack.Source : (IEnumerable) null;
    set
    {
      if (value == null)
        this.ClearValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackSourceProperty);
      else
        this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackSourceProperty, (object) value);
    }
  }

  private static void OnViewStackSourceChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = (Xceed.Wpf.Toolkit.Zoombox.Zoombox) d;
    IEnumerable oldValue = (IEnumerable) e.OldValue;
    IEnumerable newValue = (IEnumerable) e.NewValue;
    if (e.NewValue == null && !BindingOperations.IsDataBound(d, Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackSourceProperty))
    {
      if (zoombox.ViewStack != null)
        zoombox.ViewStack.ClearViewStackSource();
    }
    else
      zoombox.ViewStack.SetViewStackSource(newValue);
    zoombox.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackModeProperty);
  }

  [TypeConverter(typeof (KeyModifierCollectionConverter))]
  public KeyModifierCollection ZoomModifiers
  {
    get => (KeyModifierCollection) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomModifiersProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomModifiersProperty, (object) value);
  }

  private static KeyModifierCollection GetDefaultZoomModifiers()
  {
    KeyModifierCollection defaultZoomModifiers = new KeyModifierCollection();
    defaultZoomModifiers.Add(KeyModifier.Shift);
    defaultZoomModifiers.Add(KeyModifier.Exact);
    return defaultZoomModifiers;
  }

  public bool ZoomOnPreview
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomOnPreviewProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomOnPreviewProperty, (object) value);
  }

  public Point ZoomOrigin
  {
    get => (Point) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomOriginProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomOriginProperty, (object) value);
  }

  public double ZoomPercentage
  {
    get => (double) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomPercentageProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomPercentageProperty, (object) value);
  }

  public ZoomboxZoomOn ZoomOn
  {
    get => (ZoomboxZoomOn) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomOnProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomOnProperty, (object) value);
  }

  [TypeConverter(typeof (KeyModifierCollectionConverter))]
  public KeyModifierCollection ZoomToSelectionModifiers
  {
    get => (KeyModifierCollection) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomToSelectionModifiersProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomToSelectionModifiersProperty, (object) value);
  }

  private static KeyModifierCollection GetDefaultZoomToSelectionModifiers()
  {
    KeyModifierCollection selectionModifiers = new KeyModifierCollection();
    selectionModifiers.Add(KeyModifier.Alt);
    selectionModifiers.Add(KeyModifier.Exact);
    return selectionModifiers;
  }

  public bool KeepContentInBounds
  {
    get => (bool) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.KeepContentInBoundsProperty);
    set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.KeepContentInBoundsProperty, (object) value);
  }

  private static void OnKeepContentInBoundsChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((Xceed.Wpf.Toolkit.Zoombox.Zoombox) d).OnKeepContentInBoundsChanged(e);
  }

  private void OnKeepContentInBoundsChanged(DependencyPropertyChangedEventArgs e)
  {
    bool isAnimated = this.IsAnimated;
    this.IsAnimated = false;
    try
    {
      this.UpdateView(this.CurrentView, false, false, this.ViewStackIndex);
    }
    finally
    {
      this.IsAnimated = isAnimated;
    }
  }

  public ZoomboxViewStack ViewStack
  {
    get
    {
      if (this._viewStack == null && this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled)
        this._viewStack = new ZoomboxViewStack(this);
      return this._viewStack;
    }
  }

  internal bool HasArrangedContentPresenter
  {
    get => this._cacheBits[64 /*0x40*/];
    set => this._cacheBits[64 /*0x40*/] = value;
  }

  internal bool IsUpdatingView
  {
    get => this._cacheBits[1];
    set => this._cacheBits[1] = value;
  }

  private Vector ContentOffset
  {
    get
    {
      if (this.IsContentWrapped || this._content == null || !(this._content is FrameworkElement))
        return new Vector(0.0, 0.0);
      double x = 0.0;
      double y = 0.0;
      Size size = this.ContentRect.Size;
      switch ((this._content as FrameworkElement).HorizontalAlignment)
      {
        case HorizontalAlignment.Center:
        case HorizontalAlignment.Stretch:
          x = (this.RenderSize.Width - size.Width) / 2.0;
          break;
        case HorizontalAlignment.Right:
          x = this.RenderSize.Width - size.Width;
          break;
      }
      switch ((this._content as FrameworkElement).VerticalAlignment)
      {
        case VerticalAlignment.Center:
        case VerticalAlignment.Stretch:
          y = (this.RenderSize.Height - size.Height) / 2.0;
          break;
        case VerticalAlignment.Bottom:
          y = this.RenderSize.Height - size.Height;
          break;
      }
      return new Vector(x, y);
    }
  }

  private Rect ContentRect
  {
    get
    {
      return this._content != null ? new Rect(new Size(this._content.RenderSize.Width / this._viewboxFactor, this._content.RenderSize.Height / this._viewboxFactor)) : Rect.Empty;
    }
  }

  private bool HasRenderedFirstView
  {
    get => this._cacheBits[128 /*0x80*/];
    set => this._cacheBits[128 /*0x80*/] = value;
  }

  private bool HasUIPermission => this._cacheBits[512 /*0x0200*/];

  private bool IsContentWrapped
  {
    get => this._cacheBits[32 /*0x20*/];
    set => this._cacheBits[32 /*0x20*/] = value;
  }

  private bool IsDraggingViewport
  {
    get => this._cacheBits[4];
    set => this._cacheBits[4] = value;
  }

  private bool IsMonitoringInput
  {
    get => this._cacheBits[16 /*0x10*/];
    set => this._cacheBits[16 /*0x10*/] = value;
  }

  private bool IsResizingViewport
  {
    get => this._cacheBits[8];
    set => this._cacheBits[8] = value;
  }

  private bool IsUpdatingViewport
  {
    get => this._cacheBits[2];
    set => this._cacheBits[2] = value;
  }

  private bool RefocusViewOnFirstRender
  {
    get => this._cacheBits[256 /*0x0100*/];
    set => this._cacheBits[256 /*0x0100*/] = value;
  }

  private Rect ViewFinderDisplayRect
  {
    get
    {
      return this._viewFinderDisplay != null ? new Rect(new Point(0.0, 0.0), new Point(this._viewFinderDisplay.RenderSize.Width, this._viewFinderDisplay.RenderSize.Height)) : Rect.Empty;
    }
  }

  public event RoutedEventHandler AnimationBeginning
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationBeginningEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationBeginningEvent, (Delegate) value);
  }

  public event RoutedEventHandler AnimationCompleted
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationCompletedEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationCompletedEvent, (Delegate) value);
  }

  public event ZoomboxViewChangedEventHandler CurrentViewChanged
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewChangedEvent, (Delegate) value);
  }

  public event EventHandler<ScrollEventArgs> Scroll;

  public event IndexChangedEventHandler ViewStackIndexChanged
  {
    add => this.AddHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackIndexChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewStackIndexChangedEvent, (Delegate) value);
  }

  private void CanGoBack(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled && this.ViewStackIndex > 0;
  }

  private void GoBack(object sender, ExecutedRoutedEventArgs e) => this.GoBack();

  private void CenterContent(object sender, ExecutedRoutedEventArgs e) => this.CenterContent();

  private void FillToBounds(object sender, ExecutedRoutedEventArgs e) => this.FillToBounds();

  private void FitToBounds(object sender, ExecutedRoutedEventArgs e) => this.FitToBounds();

  private void CanGoForward(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled && this.ViewStackIndex < this.ViewStack.Count - 1;
  }

  private void GoForward(object sender, ExecutedRoutedEventArgs e) => this.GoForward();

  private void CanGoHome(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled && this.ViewStack.Count > 0 && this.ViewStackIndex != 0;
  }

  private void GoHome(object sender, ExecutedRoutedEventArgs e) => this.GoHome();

  private void PanDownExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    this.Position = new Point(this._basePosition.X, this._basePosition.Y + this.PanDistance);
  }

  private void PanLeftExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    this.Position = new Point(this._basePosition.X - this.PanDistance, this._basePosition.Y);
  }

  private void PanRightExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    this.Position = new Point(this._basePosition.X + this.PanDistance, this._basePosition.Y);
  }

  private void PanUpExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    this.Position = new Point(this._basePosition.X, this._basePosition.Y - this.PanDistance);
  }

  private void CanRefocusView(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = this.EffectiveViewStackMode == ZoomboxViewStackMode.Manual && this.ViewStackIndex >= 0 && this.ViewStackIndex < this.ViewStack.Count && this.CurrentView != this.ViewStack[this.ViewStackIndex];
  }

  private void RefocusView(object sender, ExecutedRoutedEventArgs e) => this.RefocusView();

  private void ZoomInExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    this.Zoom(this.ZoomPercentage / 100.0);
  }

  private void ZoomOutExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    this.Zoom(-this.ZoomPercentage / 100.0);
  }

  public void CenterContent()
  {
    if (this._content == null)
      return;
    this.SetScrollBars();
    this.ZoomTo(ZoomboxView.Center);
  }

  public void FillToBounds()
  {
    if (this._content == null)
      return;
    this.SetScrollBars();
    this.ZoomTo(ZoomboxView.Fill);
  }

  public void FitToBounds()
  {
    if (this._content == null)
      return;
    this.SetScrollBars();
    this.ZoomTo(ZoomboxView.Fit);
  }

  public void GoBack()
  {
    if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex <= 0)
      return;
    --this.ViewStackIndex;
  }

  public void GoForward()
  {
    if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex >= this.ViewStack.Count - 1)
      return;
    ++this.ViewStackIndex;
  }

  public void GoHome()
  {
    if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex <= 0)
      return;
    this.ViewStackIndex = 0;
  }

  public override void OnApplyTemplate()
  {
    this.AttachToVisualTree();
    base.OnApplyTemplate();
  }

  public void RefocusView()
  {
    if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Disabled || this.ViewStackIndex < 0 || this.ViewStackIndex >= this.ViewStack.Count || !(this.CurrentView != this.ViewStack[this.ViewStackIndex]))
      return;
    this.UpdateView(this.ViewStack[this.ViewStackIndex], true, false, this.ViewStackIndex);
  }

  public void Zoom(double percentage)
  {
    if (this._content == null)
      return;
    this.Zoom(percentage, this.GetZoomRelativePoint());
  }

  public void Zoom(double percentage, Point relativeTo)
  {
    if (this._content == null)
      return;
    this.ZoomTo(this.Scale * (1.0 + percentage), relativeTo);
  }

  public void ZoomTo(Point position)
  {
    if (this._content == null)
      return;
    this.ZoomTo(new ZoomboxView(new Point(-position.X, -position.Y)));
  }

  public void ZoomTo(Rect region)
  {
    if (this._content == null)
      return;
    this.UpdateView(new ZoomboxView(region), true, true);
  }

  public void ZoomTo(double scale)
  {
    if (this._content == null)
      return;
    this.ZoomTo(scale, true);
  }

  public void ZoomTo(double scale, Point relativeTo) => this.ZoomTo(scale, relativeTo, true, true);

  public void ZoomTo(ZoomboxView view) => this.UpdateView(view, true, true);

  internal void UpdateStackProperties()
  {
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.HasBackStackPropertyKey, (object) (this.ViewStackIndex > 0));
    this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.HasForwardStackPropertyKey, (object) (this.ViewStack.Count > this.ViewStackIndex + 1));
    CommandManager.InvalidateRequerySuggested();
  }

  protected override Size MeasureOverride(Size constraint)
  {
    if (this._content != null)
    {
      Size size = base.MeasureOverride(constraint);
      this._content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      return size;
    }
    if (double.IsInfinity(constraint.Height))
      constraint.Height = 0.0;
    if (double.IsInfinity(constraint.Width))
      constraint.Width = 0.0;
    return constraint;
  }

  protected override void OnContentChanged(object oldContent, object newContent)
  {
    if (oldContent is FrameworkElement)
      (oldContent as FrameworkElement).RemoveHandler(FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnContentSizeChanged));
    else
      this.RemoveHandler(FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnContentSizeChanged));
    if (this._content is FrameworkElement)
      (this._content as FrameworkElement).AddHandler(FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnContentSizeChanged), true);
    else
      this.AddHandler(FrameworkElement.SizeChangedEvent, (Delegate) new SizeChangedEventHandler(this.OnContentSizeChanged), true);
    if (this._viewFinderDisplay == null || this._viewFinderDisplay.VisualBrush == null)
      return;
    this._viewFinderDisplay.VisualBrush.Visual = (Visual) this._content;
  }

  protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    this.MonitorInput();
    base.OnGotKeyboardFocus(e);
  }

  protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    this.MonitorInput();
    base.OnLostKeyboardFocus(e);
  }

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    this.CoerceValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.IsAnimatedProperty);
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    if (this.HasArrangedContentPresenter && !this.HasRenderedFirstView)
    {
      this.HasRenderedFirstView = true;
      if (this.RefocusViewOnFirstRender)
      {
        this.RefocusViewOnFirstRender = false;
        bool isAnimated = this.IsAnimated;
        this.IsAnimated = false;
        try
        {
          this.RefocusView();
        }
        finally
        {
          this.IsAnimated = isAnimated;
        }
      }
    }
    base.OnRender(drawingContext);
  }

  private static void RefocusView(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox = o as Xceed.Wpf.Toolkit.Zoombox.Zoombox;
    zoombox.UpdateView(zoombox.CurrentView, true, false, zoombox.ViewStackIndex);
  }

  private void AttachToVisualTree()
  {
    if (this._isUpdatingVisualTree)
      return;
    this._isUpdatingVisualTree = true;
    this.DetachFromVisualTree();
    this._dragAdorner = new Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner((UIElement) this);
    if (this.Template.Resources.Contains((object) "SelectionBrush"))
      this._dragAdorner.Brush = this.Template.Resources[(object) "SelectionBrush"] as Brush;
    if (this.Template.Resources.Contains((object) "SelectionPen"))
      this._dragAdorner.Pen = this.Template.Resources[(object) "SelectionPen"] as Pen;
    if (this.Template.Resources.Contains((object) "InputBindings") && this.Template.Resources[(object) "InputBindings"] is InputBindingCollection resource)
      this.InputBindings.AddRange((ICollection) resource);
    this._contentPresenter = VisualTreeHelperEx.FindDescendantByType((Visual) this, typeof (ContentPresenter)) as ContentPresenter;
    if (this._contentPresenter == null)
      throw new InvalidTemplateException(ErrorMessages.GetMessage("ZoomboxTemplateNeedsContent"));
    this._verticalScrollBar = this.GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
    if (this._verticalScrollBar == null)
      throw new InvalidTemplateException(ErrorMessages.GetMessage("Zoombox vertical scrollBar not found."));
    this._verticalScrollBar.Scroll += new ScrollEventHandler(this.VerticalScrollBar_Scroll);
    this._horizontalScrollBar = this.GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
    if (this._horizontalScrollBar == null)
      throw new InvalidTemplateException(ErrorMessages.GetMessage("Zoombox horizontal scrollBar not found."));
    this._horizontalScrollBar.Scroll += new ScrollEventHandler(this.HorizontalScrollBar_Scroll);
    AdornerLayer adornerLayer = (AdornerLayer) null;
    if (VisualTreeHelperEx.FindDescendantByType((Visual) this, typeof (AdornerDecorator)) is AdornerDecorator descendantByType)
    {
      adornerLayer = descendantByType.AdornerLayer;
    }
    else
    {
      try
      {
        adornerLayer = AdornerLayer.GetAdornerLayer((Visual) this);
      }
      catch (Exception ex)
      {
      }
    }
    adornerLayer?.Add((Adorner) this._dragAdorner);
    VisualTreeHelperEx.FindDescendantWithPropertyValue((Visual) this, ButtonBase.IsPressedProperty, (object) true);
    if (this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderPropertyKey.DependencyProperty) == null)
    {
      this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderPropertyKey, (object) (this.Template.FindName("ViewFinder", (FrameworkElement) this) as FrameworkElement));
      Xceed.Wpf.Toolkit.Zoombox.Zoombox.SetViewFinderVisibility((DependencyObject) this, Visibility.Collapsed);
    }
    else
      Xceed.Wpf.Toolkit.Zoombox.Zoombox.SetViewFinderVisibility((DependencyObject) this, Visibility.Hidden);
    if (this.ViewFinder != null)
      this._viewFinderDisplay = VisualTreeHelperEx.FindDescendantByType((Visual) this.ViewFinder, typeof (ZoomboxViewFinderDisplay)) as ZoomboxViewFinderDisplay;
    if (this.ViewFinder != null && this._viewFinderDisplay == null)
      throw new InvalidTemplateException(ErrorMessages.GetMessage("ZoomboxHasViewFinderButNotDisplay"));
    if (this._viewFinderDisplay != null)
    {
      this.CreateVisualBrushForViewFinder((Visual) this._content);
      this._viewFinderDisplay.MouseMove += new MouseEventHandler(this.ViewFinderDisplayMouseMove);
      this._viewFinderDisplay.MouseLeftButtonDown += new MouseButtonEventHandler(this.ViewFinderDisplayBeginCapture);
      this._viewFinderDisplay.MouseLeftButtonUp += new MouseButtonEventHandler(this.ViewFinderDisplayEndCapture);
      this._viewFinderDisplay.SetBinding(ZoomboxViewFinderDisplay.ViewportRectProperty, (BindingBase) new Binding("Viewport")
      {
        Mode = BindingMode.OneWay,
        Converter = (IValueConverter) new Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewFinderSelectionConverter(this),
        Source = (object) this
      });
    }
    this.UpdateViewFinderDisplayContentBounds();
    this._contentPresenter.LayoutUpdated += new EventHandler(this.ContentPresenterFirstArranged);
    this._isUpdatingVisualTree = false;
  }

  private void CreateVisualBrushForViewFinder(Visual visual)
  {
    this._viewFinderDisplay.VisualBrush = new VisualBrush(visual);
    this._viewFinderDisplay.VisualBrush.Stretch = Stretch.Uniform;
    this._viewFinderDisplay.VisualBrush.AlignmentX = AlignmentX.Left;
    this._viewFinderDisplay.VisualBrush.AlignmentY = AlignmentY.Top;
  }

  private void ContentPresenterFirstArranged(object sender, EventArgs e)
  {
    this._contentPresenter.LayoutUpdated -= new EventHandler(this.ContentPresenterFirstArranged);
    this.HasArrangedContentPresenter = true;
    this.InvalidateVisual();
    bool isAnimated = this.IsAnimated;
    this.IsAnimated = false;
    try
    {
      double scale = this.Scale;
      Point position = this.Position;
      if (this.EffectiveViewStackMode != ZoomboxViewStackMode.Disabled)
      {
        bool flag = false;
        if (this.ViewStack.Count > 0)
        {
          if (this.ViewStackIndex >= 0)
          {
            if (this.ViewStackIndex > this.ViewStack.Count - 1)
              this.ViewStackIndex = this.ViewStack.Count - 1;
            else
              this.UpdateView(this.ViewStack[this.ViewStackIndex], false, false, this.ViewStackIndex);
          }
          else if (this.EffectiveViewStackMode != ZoomboxViewStackMode.Auto && this.ViewStackIndex < 0)
            this.ViewStackIndex = 0;
          if (this.ViewStackIndex >= 0)
          {
            flag = true;
            if (!DoubleHelper.IsNaN(scale) || !PointHelper.IsEmpty(position))
              this.UpdateView(new ZoomboxView(scale, position), false, false);
          }
        }
        if (!flag)
        {
          ZoomboxView view = new ZoomboxView(DoubleHelper.IsNaN(this.Scale) ? 1.0 : this.Scale, PointHelper.IsEmpty(position) ? new Point() : position);
          if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Auto)
          {
            this.ViewStack.PushView(view);
            this.ViewStackIndex = 0;
          }
          else
            this.UpdateView(view, false, false);
        }
      }
      else
        this.UpdateView(new ZoomboxView(DoubleHelper.IsNaN(this.Scale) ? 1.0 : this.Scale, position), false, false);
    }
    finally
    {
      this.IsAnimated = isAnimated;
    }
    this.ZoomTo(this.Scale);
  }

  private void DetachFromVisualTree()
  {
    if (this._dragAdorner != null && AdornerLayer.GetAdornerLayer((Visual) this) != null)
      AdornerLayer.GetAdornerLayer((Visual) this).Remove((Adorner) this._dragAdorner);
    if (this._contentPresenter != null)
      this._contentPresenter.LayoutUpdated -= new EventHandler(this.ContentPresenterFirstArranged);
    if (this._verticalScrollBar != null)
      this._verticalScrollBar.Scroll -= new ScrollEventHandler(this.VerticalScrollBar_Scroll);
    if (this._horizontalScrollBar != null)
      this._horizontalScrollBar.Scroll -= new ScrollEventHandler(this.HorizontalScrollBar_Scroll);
    if (this._viewFinderDisplay != null)
    {
      this._viewFinderDisplay.MouseMove -= new MouseEventHandler(this.ViewFinderDisplayMouseMove);
      this._viewFinderDisplay.MouseLeftButtonDown -= new MouseButtonEventHandler(this.ViewFinderDisplayBeginCapture);
      this._viewFinderDisplay.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ViewFinderDisplayEndCapture);
      BindingOperations.ClearBinding((DependencyObject) this._viewFinderDisplay, ZoomboxViewFinderDisplay.ViewportRectProperty);
      this._viewFinderDisplay = (ZoomboxViewFinderDisplay) null;
    }
    this._contentPresenter = (ContentPresenter) null;
  }

  private void Zoombox_Loaded(object sender, RoutedEventArgs e) => this.SetScrollBars();

  private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
  {
    double num = -(e.NewValue + this._relativePosition.Y);
    if (e.ScrollEventType == ScrollEventType.LargeIncrement)
      num = -this._verticalScrollBar.ViewportSize;
    else if (e.ScrollEventType == ScrollEventType.LargeDecrement)
      num = this._verticalScrollBar.ViewportSize;
    this.OnDrag(new DragDeltaEventArgs(0.0, num / this.Scale), false);
    EventHandler<ScrollEventArgs> scroll = this.Scroll;
    if (scroll == null)
      return;
    scroll((object) this, e);
  }

  private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
  {
    double num = -(e.NewValue + this._relativePosition.X);
    if (e.ScrollEventType == ScrollEventType.LargeIncrement)
      num = -this._horizontalScrollBar.ViewportSize;
    else if (e.ScrollEventType == ScrollEventType.LargeDecrement)
      num = this._horizontalScrollBar.ViewportSize;
    this.OnDrag(new DragDeltaEventArgs(num / this.Scale, 0.0), false);
    EventHandler<ScrollEventArgs> scroll = this.Scroll;
    if (scroll == null)
      return;
    scroll((object) this, e);
  }

  private void DragDisplayViewport(DragDeltaEventArgs e, bool end)
  {
    double scale = this._viewFinderDisplay.Scale;
    Rect viewportRect = this._viewFinderDisplay.ViewportRect;
    Rect contentBounds = this._viewFinderDisplay.ContentBounds;
    if (viewportRect.Contains(contentBounds))
      return;
    double num1 = e.HorizontalChange;
    double num2 = e.VerticalChange;
    if (viewportRect.Left < contentBounds.Left)
      num1 = Math.Max(0.0, num1);
    else if (viewportRect.Left + num1 < contentBounds.Left)
      num1 = contentBounds.Left - viewportRect.Left;
    if (viewportRect.Right > contentBounds.Right)
      num1 = Math.Min(0.0, num1);
    else if (viewportRect.Right + num1 > contentBounds.Left + contentBounds.Width)
      num1 = contentBounds.Left + contentBounds.Width - viewportRect.Right;
    if (viewportRect.Top < contentBounds.Top)
      num2 = Math.Max(0.0, num2);
    else if (viewportRect.Top + num2 < contentBounds.Top)
      num2 = contentBounds.Top - viewportRect.Top;
    if (viewportRect.Bottom > contentBounds.Bottom)
      num2 = Math.Min(0.0, num2);
    else if (viewportRect.Bottom + num2 > contentBounds.Top + contentBounds.Height)
      num2 = contentBounds.Top + contentBounds.Height - viewportRect.Bottom;
    this.OnDrag(new DragDeltaEventArgs(-num1 / scale / this._viewboxFactor, -num2 / scale / this._viewboxFactor), end);
    this._originPoint += new Vector(num1, num2);
  }

  private void InitCommands()
  {
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.Back, new ExecutedRoutedEventHandler(this.GoBack), new CanExecuteRoutedEventHandler(this.CanGoBack)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.Center, new ExecutedRoutedEventHandler(this.CenterContent)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.Fill, new ExecutedRoutedEventHandler(this.FillToBounds)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.Fit, new ExecutedRoutedEventHandler(this.FitToBounds)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.Forward, new ExecutedRoutedEventHandler(this.GoForward), new CanExecuteRoutedEventHandler(this.CanGoForward)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.Home, new ExecutedRoutedEventHandler(this.GoHome), new CanExecuteRoutedEventHandler(this.CanGoHome)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.PanDown, new ExecutedRoutedEventHandler(this.PanDownExecuted)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.PanLeft, new ExecutedRoutedEventHandler(this.PanLeftExecuted)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.PanRight, new ExecutedRoutedEventHandler(this.PanRightExecuted)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.PanUp, new ExecutedRoutedEventHandler(this.PanUpExecuted)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.Refocus, new ExecutedRoutedEventHandler(this.RefocusView), new CanExecuteRoutedEventHandler(this.CanRefocusView)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomIn, new ExecutedRoutedEventHandler(this.ZoomInExecuted)));
    this.CommandBindings.Add(new CommandBinding((ICommand) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ZoomOut, new ExecutedRoutedEventHandler(this.ZoomOutExecuted)));
  }

  private void MonitorInput()
  {
    if (!this.HasUIPermission)
      return;
    this.PreProcessInput();
  }

  private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
  {
    this.UpdateViewFinderDisplayContentBounds();
    if (!this.HasArrangedContentPresenter)
      return;
    if (this.HasRenderedFirstView)
    {
      this.SetScrollBars();
      this.UpdateView(this.CurrentView, true, false, this.CurrentViewIndex);
    }
    else
    {
      this.RefocusViewOnFirstRender = true;
      this.InvalidateVisual();
    }
  }

  private void OnDrag(DragDeltaEventArgs e, bool end)
  {
    Point relativePosition = this._relativePosition;
    double scale = this.Scale;
    Vector vector = this.ContentOffset * scale;
    Point position = relativePosition + vector + new Vector(e.HorizontalChange * scale, e.VerticalChange * scale);
    if (this.IsUsingScrollBars)
    {
      position.X = Math.Max(Math.Min(position.X, 0.0), -this._horizontalScrollBar.Maximum);
      position.Y = Math.Max(Math.Min(position.Y, 0.0), -this._verticalScrollBar.Maximum);
    }
    this.UpdateView(new ZoomboxView(scale, position), false, end);
  }

  private void OnLayoutUpdated(object sender, EventArgs e) => this.UpdateViewport();

  private void OnSelectRegion(DragDeltaEventArgs e, bool end)
  {
    if (end)
    {
      this._dragAdorner.Rect = Rect.Empty;
      if (this._trueContent == null)
        return;
      Rect region;
      ref Rect local = ref region;
      Point point1 = this.TranslatePoint(this._dragAdorner.LastPosition, this._trueContent);
      Point lastPosition = this._dragAdorner.LastPosition;
      Size lastSize = this._dragAdorner.LastSize;
      double width = lastSize.Width;
      lastSize = this._dragAdorner.LastSize;
      double height = lastSize.Height;
      Vector vector = new Vector(width, height);
      Point point2 = this.TranslatePoint(lastPosition + vector, this._trueContent);
      local = new Rect(point1, point2);
      this.ZoomTo(region);
    }
    else
    {
      Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner dragAdorner = this._dragAdorner;
      Rect rect1 = new Rect(this._originPoint, new Vector(e.HorizontalChange, e.VerticalChange));
      Point point1 = new Point(0.0, 0.0);
      Size renderSize = this.RenderSize;
      double width = renderSize.Width;
      renderSize = this.RenderSize;
      double height = renderSize.Height;
      Point point2 = new Point(width, height);
      Rect rect2 = new Rect(point1, point2);
      Rect rect = Rect.Intersect(rect1, rect2);
      dragAdorner.Rect = rect;
    }
  }

  private void OnSizeChanged(object sender, SizeChangedEventArgs e)
  {
    if (!this.HasArrangedContentPresenter)
      return;
    this.SetScrollBars();
    this.UpdateViewboxFactor();
    bool isAnimated = this.IsAnimated;
    this.IsAnimated = false;
    try
    {
      this.UpdateView(this.CurrentView, false, false, this.ViewStackIndex);
    }
    finally
    {
      this.IsAnimated = isAnimated;
    }
  }

  private void SetScrollBars()
  {
    if (this._content == null || this._verticalScrollBar == null || this._horizontalScrollBar == null)
      return;
    Size size = this._content is Viewbox ? ((Decorator) this._content).Child.DesiredSize : this.RenderSize;
    this._verticalScrollBar.SmallChange = 10.0;
    this._verticalScrollBar.LargeChange = 10.0;
    this._verticalScrollBar.Minimum = 0.0;
    this._verticalScrollBar.ViewportSize = this.RenderSize.Height;
    this._verticalScrollBar.Maximum = size.Height - this._verticalScrollBar.ViewportSize;
    this._horizontalScrollBar.SmallChange = 10.0;
    this._horizontalScrollBar.LargeChange = 10.0;
    this._horizontalScrollBar.Minimum = 0.0;
    this._horizontalScrollBar.ViewportSize = this.RenderSize.Width;
    this._horizontalScrollBar.Maximum = size.Width - this._horizontalScrollBar.ViewportSize;
  }

  private void PreProcessInput()
  {
    if (this.IsMouseOver || this.IsKeyboardFocusWithin)
    {
      if (this.IsMonitoringInput)
        return;
      this.IsMonitoringInput = true;
      InputManager.Current.PreNotifyInput += new NotifyInputEventHandler(this.PreProcessInput);
      this.UpdateKeyModifierTriggerProperties();
    }
    else
    {
      if (!this.IsMonitoringInput)
        return;
      this.IsMonitoringInput = false;
      InputManager.Current.PreNotifyInput -= new NotifyInputEventHandler(this.PreProcessInput);
      this.SetAreDragModifiersActive(false);
      this.SetAreRelativeZoomModifiersActive(false);
      this.SetAreZoomModifiersActive(false);
      this.SetAreZoomToSelectionModifiersActive(false);
    }
  }

  private void PreProcessInput(object sender, NotifyInputEventArgs e)
  {
    if (!(e.StagingItem.Input is KeyEventArgs))
      return;
    this.UpdateKeyModifierTriggerProperties();
  }

  private void ProcessMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (this.ZoomToSelectionModifiers.AreActive)
    {
      this.SetIsDraggingContent(false);
      this.SetIsSelectingRegion(true);
    }
    else if (this.DragModifiers.AreActive)
    {
      this.SetIsSelectingRegion(false);
      this.SetIsDraggingContent(true);
    }
    else
    {
      this.SetIsSelectingRegion(false);
      this.SetIsDraggingContent(false);
    }
    if (!this.IsSelectingRegion && !this.IsDraggingContent)
      return;
    this._originPoint = e.GetPosition((IInputElement) this);
    this._contentPresenter.CaptureMouse();
    e.Handled = true;
    if (this.IsDraggingContent)
    {
      this.OnDrag(new DragDeltaEventArgs(0.0, 0.0), false);
    }
    else
    {
      if (!this.IsSelectingRegion)
        return;
      this.OnSelectRegion(new DragDeltaEventArgs(0.0, 0.0), false);
    }
  }

  private void ProcessMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (!this.IsDraggingContent && !this.IsSelectingRegion)
      return;
    int num = this.IsDraggingContent ? 1 : 0;
    this.SetIsDraggingContent(false);
    this.SetIsSelectingRegion(false);
    this._originPoint = new Point();
    this._contentPresenter.ReleaseMouseCapture();
    e.Handled = true;
    if (num != 0)
      this.OnDrag(new DragDeltaEventArgs(0.0, 0.0), true);
    else
      this.OnSelectRegion(new DragDeltaEventArgs(0.0, 0.0), true);
  }

  private void ProcessMouseMove(MouseEventArgs e)
  {
    if (e.MouseDevice.LeftButton != MouseButtonState.Pressed || !this.IsDraggingContent && !this.IsSelectingRegion)
      return;
    Point position = e.GetPosition((IInputElement) this);
    e.Handled = true;
    if (this.IsDraggingContent)
    {
      Vector vector = (position - this._originPoint) / this.Scale;
      this.OnDrag(new DragDeltaEventArgs(vector.X, vector.Y), false);
      this._originPoint = position;
    }
    else
    {
      if (!this.IsSelectingRegion)
        return;
      Vector vector = position - this._originPoint;
      this.OnSelectRegion(new DragDeltaEventArgs(vector.X, vector.Y), false);
    }
  }

  private void ProcessMouseWheelZoom(MouseWheelEventArgs e)
  {
    if (this._content == null)
      return;
    bool flag = this.ZoomModifiers.AreActive;
    bool areActive = this.RelativeZoomModifiers.AreActive;
    if (flag & areActive)
      flag = false;
    if (!(flag | areActive))
      return;
    e.Handled = true;
    double percentage = (double) (e.Delta / Xceed.Wpf.Toolkit.Zoombox.Zoombox.MOUSE_WHEEL_DELTA) * this.ZoomPercentage / 100.0;
    if (areActive)
      this.Zoom(percentage, Mouse.GetPosition((IInputElement) this._content));
    else
      this.Zoom(percentage);
  }

  private void ProcessNavigationButton(RoutedEventArgs e)
  {
    switch (e)
    {
      case MouseButtonEventArgs _:
        MouseButtonEventArgs mouseButtonEventArgs = e as MouseButtonEventArgs;
        if (mouseButtonEventArgs.ChangedButton != MouseButton.XButton1 && mouseButtonEventArgs.ChangedButton != MouseButton.XButton2)
          break;
        if (mouseButtonEventArgs.ChangedButton == MouseButton.XButton2)
          this.GoForward();
        else
          this.GoBack();
        mouseButtonEventArgs.Handled = true;
        break;
      case KeyEventArgs _:
        KeyEventArgs keyEventArgs = e as KeyEventArgs;
        if (keyEventArgs.Key != Key.Back && keyEventArgs.Key != Key.BrowserBack && keyEventArgs.Key != Key.BrowserForward)
          break;
        if (keyEventArgs.Key == Key.BrowserForward)
          this.GoForward();
        else
          this.GoBack();
        keyEventArgs.Handled = true;
        break;
    }
  }

  private void ResizeDisplayViewport(DragDeltaEventArgs e, Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge relativeTo)
  {
    Rect viewportRect = this._viewFinderDisplay.ViewportRect;
    double scale = this._viewFinderDisplay.Scale;
    double num1 = Math.Max(this._resizeViewportBounds.Left, Math.Min(this._resizeDraggingPoint.X + e.HorizontalChange, this._resizeViewportBounds.Right));
    double num2 = Math.Max(this._resizeViewportBounds.Top, Math.Min(this._resizeDraggingPoint.Y + e.VerticalChange, this._resizeViewportBounds.Bottom));
    Point point = new Point(this._resizeAnchorPoint.X / scale, this._resizeAnchorPoint.Y / scale);
    Vector vector = new Vector((num1 - this._resizeAnchorPoint.X) / scale / this._viewboxFactor, (num2 - this._resizeAnchorPoint.Y) / scale / this._viewboxFactor);
    Rect rect = new Rect(point, vector);
    rect = new Rect(this._content.TranslatePoint(rect.TopLeft, (UIElement) this._contentPresenter), this._content.TranslatePoint(rect.BottomRight, (UIElement) this._contentPresenter));
    double num3 = this.RenderSize.Width / rect.Width;
    double num4 = this.RenderSize.Height / rect.Height;
    this.ZoomTo(num3 < num4 ? num3 : num4, point, false, false);
  }

  private void UpdateKeyModifierTriggerProperties()
  {
    this.SetAreDragModifiersActive(this.DragModifiers.AreActive);
    this.SetAreRelativeZoomModifiersActive(this.RelativeZoomModifiers.AreActive);
    this.SetAreZoomModifiersActive(this.ZoomModifiers.AreActive);
    this.SetAreZoomToSelectionModifiersActive(this.ZoomToSelectionModifiers.AreActive);
  }

  private void UpdateView(ZoomboxView view, bool allowAnimation, bool allowStackAddition)
  {
    this.UpdateView(view, allowAnimation, allowStackAddition, -1);
  }

  private void UpdateView(
    ZoomboxView view,
    bool allowAnimation,
    bool allowStackAddition,
    int stackIndex)
  {
    if (this._contentPresenter == null || this._content == null || !this.HasArrangedContentPresenter)
      return;
    if (view.ViewKind == ZoomboxViewKind.Absolute && PointHelper.IsEmpty(view.Position))
    {
      this.ZoomTo(view.Scale, allowStackAddition);
    }
    else
    {
      if (this.IsUpdatingView)
        return;
      this.IsUpdatingView = true;
      try
      {
        double newRelativeScale = this._viewboxFactor;
        Point newRelativePosition = new Point();
        Rect region = Rect.Empty;
        switch (view.ViewKind)
        {
          case ZoomboxViewKind.Absolute:
            newRelativeScale = DoubleHelper.IsNaN(view.Scale) ? this._relativeScale : view.Scale;
            newRelativePosition = PointHelper.IsEmpty(view.Position) ? this._relativePosition : new Point(view.Position.X, view.Position.Y) - this.ContentOffset * newRelativeScale;
            break;
          case ZoomboxViewKind.Fit:
            region = this.ContentRect;
            break;
          case ZoomboxViewKind.Fill:
            region = this.CalculateFillRect();
            break;
          case ZoomboxViewKind.Center:
            Rect rect1 = new Rect(this._content.TranslatePoint(this.ContentRect.TopLeft, (UIElement) this), this._content.TranslatePoint(this.ContentRect.BottomRight, (UIElement) this));
            region = Rect.Inflate(rect1, (this.RenderSize.Width / this._viewboxFactor - rect1.Width) / 2.0, (this.RenderSize.Height / this._viewboxFactor - rect1.Height) / 2.0);
            region = new Rect(this.TranslatePoint(region.TopLeft, this._content), this.TranslatePoint(region.BottomRight, this._content));
            break;
          case ZoomboxViewKind.Region:
            region = view.Region;
            break;
        }
        if (view.ViewKind != ZoomboxViewKind.Empty)
        {
          if (!region.IsEmpty)
            this.CalculatePositionAndScale(region, ref newRelativePosition, ref newRelativeScale);
          else if (view != ZoomboxView.Empty)
          {
            if (newRelativeScale > this.MaxScale)
              newRelativeScale = this.MaxScale;
            else if (newRelativeScale < this.MinScale)
              newRelativeScale = this.MinScale;
          }
          double fromValue = this._relativeScale;
          double x = this._relativePosition.X;
          double y = this._relativePosition.Y;
          if (this._contentPresenter.RenderTransform != Transform.Identity)
          {
            TransformGroup renderTransform = this._contentPresenter.RenderTransform as TransformGroup;
            ScaleTransform child1 = renderTransform.Children[0] as ScaleTransform;
            TranslateTransform child2 = renderTransform.Children[1] as TranslateTransform;
            fromValue = child1.ScaleX;
            x = child2.X;
            y = child2.Y;
          }
          if (this.KeepContentInBounds)
          {
            Rect rect2 = new Rect(new Size(this.ContentRect.Width * newRelativeScale, this.ContentRect.Height * newRelativeScale));
            Rect rect3 = new Rect(new Point(-newRelativePosition.X, -newRelativePosition.Y), this._contentPresenter.RenderSize);
            if (DoubleHelper.AreVirtuallyEqual(this._relativeScale, newRelativeScale))
            {
              if (this.IsGreaterThanOrClose(rect2.Width, rect3.Width))
              {
                if (rect2.Right < rect3.Right)
                  newRelativePosition.X = -(rect2.Width - rect3.Width);
                if (rect2.Left > rect3.Left)
                  newRelativePosition.X = 0.0;
              }
              else if (this.IsGreaterThanOrClose(rect3.Width, rect2.Width))
              {
                if (rect3.Right < rect2.Right)
                  newRelativePosition.X = rect3.Width - rect2.Width;
                if (rect3.Left > rect2.Left)
                  newRelativePosition.X = 0.0;
              }
              if (this.IsGreaterThanOrClose(rect2.Height, rect3.Height))
              {
                if (rect2.Bottom < rect3.Bottom)
                  newRelativePosition.Y = -(rect2.Height - rect3.Height);
                if (rect2.Top > rect3.Top)
                  newRelativePosition.Y = 0.0;
              }
              else if (this.IsGreaterThanOrClose(rect3.Height, rect2.Height))
              {
                if (rect3.Bottom < rect2.Bottom)
                  newRelativePosition.Y = rect3.Height - rect2.Height;
                if (rect3.Top > rect2.Top)
                  newRelativePosition.Y = 0.0;
              }
            }
          }
          ScaleTransform scaleTransform = new ScaleTransform(newRelativeScale / this._viewboxFactor, newRelativeScale / this._viewboxFactor);
          TranslateTransform translateTransform = new TranslateTransform(newRelativePosition.X, newRelativePosition.Y);
          this._contentPresenter.RenderTransform = (Transform) new TransformGroup()
          {
            Children = {
              (Transform) scaleTransform,
              (Transform) translateTransform
            }
          };
          Size size1 = this._content is Viewbox ? ((Decorator) this._content).Child.DesiredSize : this.RenderSize;
          Size size2 = new Size(size1.Width * newRelativeScale, size1.Height * newRelativeScale);
          if (allowAnimation && this.IsAnimated)
          {
            DoubleAnimation animation1 = new DoubleAnimation(fromValue, newRelativeScale / this._viewboxFactor, this.AnimationDuration);
            animation1.AccelerationRatio = this.AnimationAccelerationRatio;
            animation1.DecelerationRatio = this.AnimationDecelerationRatio;
            DoubleAnimation animation2 = new DoubleAnimation(x, newRelativePosition.X, this.AnimationDuration);
            animation2.AccelerationRatio = this.AnimationAccelerationRatio;
            animation2.DecelerationRatio = this.AnimationDecelerationRatio;
            DoubleAnimation animation3 = new DoubleAnimation(y, newRelativePosition.Y, this.AnimationDuration);
            animation3.AccelerationRatio = this.AnimationAccelerationRatio;
            animation3.DecelerationRatio = this.AnimationDecelerationRatio;
            animation3.CurrentTimeInvalidated += new EventHandler(this.UpdateViewport);
            animation3.CurrentStateInvalidated += new EventHandler(this.ZoomAnimationCompleted);
            this.RaiseEvent(new RoutedEventArgs(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationBeginningEvent, (object) this));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, (AnimationTimeline) animation1);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, (AnimationTimeline) animation1);
            translateTransform.BeginAnimation(TranslateTransform.XProperty, (AnimationTimeline) animation2);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline) animation3);
            if (this.IsUsingScrollBars)
            {
              DoubleAnimation animation4 = new DoubleAnimation();
              animation4.From = new double?(this._verticalScrollBar.Maximum);
              animation4.To = new double?(size2.Height - this._verticalScrollBar.ViewportSize);
              animation4.Duration = this.AnimationDuration;
              this._verticalScrollBar.BeginAnimation(RangeBase.MaximumProperty, (AnimationTimeline) animation4);
              DoubleAnimation animation5 = new DoubleAnimation();
              animation5.From = new double?(this._verticalScrollBar.Value);
              animation5.To = new double?(-newRelativePosition.Y);
              animation5.Duration = this.AnimationDuration;
              animation5.Completed += new EventHandler(this.VerticalValueAnimation_Completed);
              this._verticalScrollBar.BeginAnimation(RangeBase.ValueProperty, (AnimationTimeline) animation5);
              DoubleAnimation animation6 = new DoubleAnimation();
              animation6.From = new double?(this._horizontalScrollBar.Maximum);
              animation6.To = new double?(size2.Width - this._horizontalScrollBar.ViewportSize);
              animation6.Duration = this.AnimationDuration;
              this._horizontalScrollBar.BeginAnimation(RangeBase.MaximumProperty, (AnimationTimeline) animation6);
              DoubleAnimation animation7 = new DoubleAnimation();
              animation7.From = new double?(this._horizontalScrollBar.Value);
              animation7.To = new double?(-newRelativePosition.X);
              animation7.Duration = this.AnimationDuration;
              animation7.Completed += new EventHandler(this.HorizontalValueAnimation_Completed);
              this._horizontalScrollBar.BeginAnimation(RangeBase.ValueProperty, (AnimationTimeline) animation7);
            }
          }
          else if (this.IsUsingScrollBars)
          {
            this._verticalScrollBar.Maximum = size2.Height - this._verticalScrollBar.ViewportSize;
            this._verticalScrollBar.Value = -newRelativePosition.Y;
            this._horizontalScrollBar.Maximum = size2.Width - this._horizontalScrollBar.ViewportSize;
            this._horizontalScrollBar.Value = -newRelativePosition.X;
          }
          this._relativePosition = newRelativePosition;
          this._relativeScale = newRelativeScale;
          this.Scale = newRelativeScale;
          this._basePosition = newRelativePosition + this.ContentOffset * newRelativeScale;
          this.UpdateViewport();
        }
        if (this.EffectiveViewStackMode == ZoomboxViewStackMode.Auto & allowStackAddition)
        {
          if (this.ViewStack.Count > 1 && Math.Abs(DateTime.Now.Ticks - this._lastStackAddition.Ticks) < TimeSpan.FromMilliseconds(300.0).Ticks)
          {
            this.ViewStack.RemoveAt(this.ViewStack.Count - 1);
            this._lastStackAddition = DateTime.Now - TimeSpan.FromMilliseconds(300.0);
          }
          if (this.ViewStack.Count <= 0 || !(view == this.ViewStack.SelectedView))
          {
            this.ViewStack.PushView(view);
            ++this.ViewStackIndex;
            stackIndex = this.ViewStackIndex;
            this._lastStackAddition = DateTime.Now;
          }
        }
        this._lastViewIndex = this.CurrentViewIndex;
        this.SetCurrentViewIndex(stackIndex);
        this.SetCurrentView(view);
      }
      finally
      {
        this.IsUpdatingView = false;
      }
    }
  }

  private bool IsGreaterThanOrClose(double value1, double value2)
  {
    return value1 > value2 || DoubleHelper.AreVirtuallyEqual(value1, value2);
  }

  private Rect CalculateFillRect()
  {
    Size renderSize = this.RenderSize;
    double width1 = renderSize.Width;
    renderSize = this.RenderSize;
    double height1 = renderSize.Height;
    double num = width1 / height1;
    double x = 0.0;
    double y = 0.0;
    double width2 = this.ContentRect.Width;
    double height2 = this.ContentRect.Height;
    if (num > width2 / height2)
    {
      height2 = width2 / num;
      y = (this.ContentRect.Height - height2) / 2.0;
    }
    else
    {
      width2 = height2 * num;
      x = (this.ContentRect.Width - width2) / 2.0;
    }
    return new Rect(x, y, width2, height2);
  }

  private void CalculatePositionAndScale(
    Rect region,
    ref Point newRelativePosition,
    ref double newRelativeScale)
  {
    if (region.Width == 0.0 || region.Height == 0.0 || !this.ContentRect.IntersectsWith(region))
      return;
    region = new Rect(this._content.TranslatePoint(region.TopLeft, (UIElement) this._contentPresenter), this._content.TranslatePoint(region.BottomRight, (UIElement) this._contentPresenter));
    Size renderSize1 = this.RenderSize;
    double num1 = renderSize1.Width / region.Width;
    renderSize1 = this.RenderSize;
    double num2 = renderSize1.Height / region.Height;
    newRelativeScale = num1 < num2 ? num1 : num2;
    if (newRelativeScale > this.MaxScale)
      newRelativeScale = this.MaxScale;
    else if (newRelativeScale < this.MinScale)
      newRelativeScale = this.MinScale;
    double x = 0.0;
    double y = 0.0;
    Size renderSize2;
    switch (this.HorizontalContentAlignment)
    {
      case HorizontalAlignment.Center:
      case HorizontalAlignment.Stretch:
        renderSize2 = this.RenderSize;
        x = (renderSize2.Width - region.Width * newRelativeScale) / 2.0;
        break;
      case HorizontalAlignment.Right:
        renderSize2 = this.RenderSize;
        x = renderSize2.Width - region.Width * newRelativeScale;
        break;
    }
    switch (this.VerticalContentAlignment)
    {
      case VerticalAlignment.Center:
      case VerticalAlignment.Stretch:
        renderSize2 = this.RenderSize;
        y = (renderSize2.Height - region.Height * newRelativeScale) / 2.0;
        break;
      case VerticalAlignment.Bottom:
        renderSize2 = this.RenderSize;
        y = renderSize2.Height - region.Height * newRelativeScale;
        break;
    }
    newRelativePosition = new Point(-region.TopLeft.X * newRelativeScale, -region.TopLeft.Y * newRelativeScale) + new Vector(x, y);
  }

  private void UpdateViewFinderDisplayContentBounds()
  {
    if (this._content == null || this._trueContent == null || this._viewFinderDisplay == null)
      return;
    this.UpdateViewboxFactor();
    Size renderSize = this._content.RenderSize;
    Size size = this._viewFinderDisplay.AvailableSize;
    if (size.Width > 0.0 && DoubleHelper.AreVirtuallyEqual(size.Height, 0.0))
      size = new Size(size.Width, renderSize.Height * size.Width / renderSize.Width);
    else if (size.Height > 0.0 && DoubleHelper.AreVirtuallyEqual(size.Width, 0.0))
      size = new Size(renderSize.Width * size.Height / renderSize.Height, size.Width);
    double num1 = size.Width / renderSize.Width;
    double num2 = size.Height / renderSize.Height;
    double num3 = num1 < num2 ? num1 : num2;
    double width = renderSize.Width * num3;
    double height = renderSize.Height * num3;
    this._viewFinderDisplay.Scale = num3;
    this._viewFinderDisplay.ContentBounds = new Rect(new Size(width, height));
  }

  private void UpdateViewboxFactor()
  {
    if (this._content == null || this._trueContent == null)
      return;
    double width1 = this._content.RenderSize.Width;
    double width2 = this._trueContent.RenderSize.Width;
    if (DoubleHelper.AreVirtuallyEqual(width1, 0.0) || DoubleHelper.AreVirtuallyEqual(width2, 0.0))
      this._viewboxFactor = 1.0;
    else
      this._viewboxFactor = width1 / width2;
  }

  private void UpdateViewport()
  {
    if (this._contentPresenter == null || this._trueContent == null)
      return;
    this.IsUpdatingViewport = true;
    try
    {
      Rect r1;
      ref Rect local = ref r1;
      Point point1 = this.TranslatePoint(new Point(0.0, 0.0), this._trueContent);
      Size renderSize = this.RenderSize;
      double width = renderSize.Width;
      renderSize = this.RenderSize;
      double height = renderSize.Height;
      Point point2 = this.TranslatePoint(new Point(width, height), this._trueContent);
      local = new Rect(point1, point2);
      if (DoubleHelper.AreVirtuallyEqual(r1, this.Viewport))
        return;
      this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.ViewportPropertyKey, (object) r1);
    }
    finally
    {
      this.IsUpdatingViewport = false;
    }
  }

  private void UpdateViewport(object sender, EventArgs e) => this.UpdateViewport();

  private void ViewFinderDisplayBeginCapture(object sender, MouseButtonEventArgs e)
  {
    if (!(this._viewFinderDisplay.Tag is Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge))
      return;
    if ((Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge) this._viewFinderDisplay.Tag == Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.None)
    {
      this.IsDraggingViewport = true;
    }
    else
    {
      this.IsResizingViewport = true;
      Vector vector = new Vector();
      switch ((Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge) this._viewFinderDisplay.Tag)
      {
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.TopLeft:
          this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.TopLeft;
          this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.BottomRight;
          vector = new Vector(-1.0, -1.0);
          break;
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.TopRight:
          this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.TopRight;
          this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.BottomLeft;
          vector = new Vector(1.0, -1.0);
          break;
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.BottomLeft:
          this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.BottomLeft;
          this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.TopRight;
          vector = new Vector(-1.0, 1.0);
          break;
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.BottomRight:
          this._resizeDraggingPoint = this._viewFinderDisplay.ViewportRect.BottomRight;
          this._resizeAnchorPoint = this._viewFinderDisplay.ViewportRect.TopLeft;
          vector = new Vector(1.0, 1.0);
          break;
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Left:
          this._resizeDraggingPoint = new Point(this._viewFinderDisplay.ViewportRect.Left, this._viewFinderDisplay.ViewportRect.Top + this._viewFinderDisplay.ViewportRect.Height / 2.0);
          Rect viewportRect1 = this._viewFinderDisplay.ViewportRect;
          double right = viewportRect1.Right;
          viewportRect1 = this._viewFinderDisplay.ViewportRect;
          double y1 = viewportRect1.Top + this._viewFinderDisplay.ViewportRect.Height / 2.0;
          this._resizeAnchorPoint = new Point(right, y1);
          vector = new Vector(-1.0, 0.0);
          break;
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Top:
          this._resizeDraggingPoint = new Point(this._viewFinderDisplay.ViewportRect.Left + this._viewFinderDisplay.ViewportRect.Width / 2.0, this._viewFinderDisplay.ViewportRect.Top);
          Rect viewportRect2 = this._viewFinderDisplay.ViewportRect;
          double left1 = viewportRect2.Left;
          viewportRect2 = this._viewFinderDisplay.ViewportRect;
          double num1 = viewportRect2.Width / 2.0;
          this._resizeAnchorPoint = new Point(left1 + num1, this._viewFinderDisplay.ViewportRect.Bottom);
          vector = new Vector(0.0, -1.0);
          break;
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Right:
          this._resizeDraggingPoint = new Point(this._viewFinderDisplay.ViewportRect.Right, this._viewFinderDisplay.ViewportRect.Top + this._viewFinderDisplay.ViewportRect.Height / 2.0);
          Rect viewportRect3 = this._viewFinderDisplay.ViewportRect;
          double left2 = viewportRect3.Left;
          viewportRect3 = this._viewFinderDisplay.ViewportRect;
          double y2 = viewportRect3.Top + this._viewFinderDisplay.ViewportRect.Height / 2.0;
          this._resizeAnchorPoint = new Point(left2, y2);
          vector = new Vector(1.0, 0.0);
          break;
        case Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Bottom:
          this._resizeDraggingPoint = new Point(this._viewFinderDisplay.ViewportRect.Left + this._viewFinderDisplay.ViewportRect.Width / 2.0, this._viewFinderDisplay.ViewportRect.Bottom);
          Rect viewportRect4 = this._viewFinderDisplay.ViewportRect;
          double left3 = viewportRect4.Left;
          viewportRect4 = this._viewFinderDisplay.ViewportRect;
          double num2 = viewportRect4.Width / 2.0;
          this._resizeAnchorPoint = new Point(left3 + num2, this._viewFinderDisplay.ViewportRect.Top);
          vector = new Vector(0.0, 1.0);
          break;
      }
      double scale = this._viewFinderDisplay.Scale;
      Rect contentBounds = this._viewFinderDisplay.ContentBounds;
      this._resizeViewportBounds = new Rect(this._resizeAnchorPoint + new Vector(vector.X * 10000000000.0, vector.Y * 10000000000.0), this._resizeAnchorPoint + new Vector(vector.X * contentBounds.Width / this.MaxScale, vector.Y * contentBounds.Height / this.MaxScale));
    }
    this._originPoint = e.GetPosition((IInputElement) this._viewFinderDisplay);
    this._viewFinderDisplay.CaptureMouse();
    e.Handled = true;
  }

  private void ViewFinderDisplayEndCapture(object sender, MouseButtonEventArgs e)
  {
    if (!this.IsDraggingViewport && !this.IsResizingViewport)
      return;
    this.DragDisplayViewport(new DragDeltaEventArgs(0.0, 0.0), true);
    this.IsDraggingViewport = false;
    this.IsResizingViewport = false;
    this._originPoint = new Point();
    this._viewFinderDisplay.ReleaseMouseCapture();
    e.Handled = true;
  }

  private void ViewFinderDisplayMouseMove(object sender, MouseEventArgs e)
  {
    if (e.MouseDevice.LeftButton == MouseButtonState.Pressed && (this.IsDraggingViewport || this.IsResizingViewport))
    {
      Vector vector = e.GetPosition((IInputElement) this._viewFinderDisplay) - this._originPoint;
      if (this.IsDraggingViewport)
        this.DragDisplayViewport(new DragDeltaEventArgs(vector.X, vector.Y), false);
      else
        this.ResizeDisplayViewport(new DragDeltaEventArgs(vector.X, vector.Y), (Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge) this._viewFinderDisplay.Tag);
      e.Handled = true;
    }
    else
    {
      Point position = e.GetPosition((IInputElement) this._viewFinderDisplay);
      Rect viewportRect = this._viewFinderDisplay.ViewportRect;
      double num = viewportRect.Width * viewportRect.Height > 100.0 ? 5.0 : Math.Sqrt(viewportRect.Width * viewportRect.Height) / 2.0;
      if (viewportRect.Contains(position) && !DoubleHelper.AreVirtuallyEqual(Rect.Intersect(viewportRect, this._viewFinderDisplay.ContentBounds), this._viewFinderDisplay.ContentBounds))
      {
        if (PointHelper.DistanceBetween(position, viewportRect.TopLeft) < num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.TopLeft;
          this._viewFinderDisplay.Cursor = Cursors.SizeNWSE;
        }
        else if (PointHelper.DistanceBetween(position, viewportRect.BottomRight) < num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.BottomRight;
          this._viewFinderDisplay.Cursor = Cursors.SizeNWSE;
        }
        else if (PointHelper.DistanceBetween(position, viewportRect.TopRight) < num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.TopRight;
          this._viewFinderDisplay.Cursor = Cursors.SizeNESW;
        }
        else if (PointHelper.DistanceBetween(position, viewportRect.BottomLeft) < num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.BottomLeft;
          this._viewFinderDisplay.Cursor = Cursors.SizeNESW;
        }
        else if (position.X <= viewportRect.Left + num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Left;
          this._viewFinderDisplay.Cursor = Cursors.SizeWE;
        }
        else if (position.Y <= viewportRect.Top + num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Top;
          this._viewFinderDisplay.Cursor = Cursors.SizeNS;
        }
        else if (position.X >= viewportRect.Right - num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Right;
          this._viewFinderDisplay.Cursor = Cursors.SizeWE;
        }
        else if (position.Y >= viewportRect.Bottom - num)
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.Bottom;
          this._viewFinderDisplay.Cursor = Cursors.SizeNS;
        }
        else
        {
          this._viewFinderDisplay.Tag = (object) Xceed.Wpf.Toolkit.Zoombox.Zoombox.ResizeEdge.None;
          this._viewFinderDisplay.Cursor = Cursors.SizeAll;
        }
      }
      else
      {
        this._viewFinderDisplay.Tag = (object) null;
        this._viewFinderDisplay.Cursor = Cursors.Arrow;
      }
    }
  }

  private void ZoomAnimationCompleted(object sender, EventArgs e)
  {
    if ((sender as AnimationClock).CurrentState == ClockState.Active)
      return;
    (sender as AnimationClock).CurrentStateInvalidated -= new EventHandler(this.ZoomAnimationCompleted);
    (sender as AnimationClock).CurrentTimeInvalidated -= new EventHandler(this.UpdateViewport);
    this.RaiseEvent(new RoutedEventArgs(Xceed.Wpf.Toolkit.Zoombox.Zoombox.AnimationCompletedEvent, (object) this));
  }

  private void VerticalValueAnimation_Completed(object sender, EventArgs e)
  {
    if (this._verticalScrollBar.Value != -this._relativePosition.Y && this._verticalScrollBar.Value != this._verticalScrollBar.Maximum && this._verticalScrollBar.Value != this._verticalScrollBar.Minimum)
      return;
    double num = this._verticalScrollBar.Value;
    this._verticalScrollBar.BeginAnimation(RangeBase.ValueProperty, (AnimationTimeline) null);
    this._verticalScrollBar.Value = num;
  }

  private void HorizontalValueAnimation_Completed(object sender, EventArgs e)
  {
    if (this._horizontalScrollBar.Value != -this._relativePosition.X && this._horizontalScrollBar.Value != this._horizontalScrollBar.Maximum && this._horizontalScrollBar.Value != this._horizontalScrollBar.Minimum)
      return;
    double num = this._horizontalScrollBar.Value;
    this._horizontalScrollBar.BeginAnimation(RangeBase.ValueProperty, (AnimationTimeline) null);
    this._horizontalScrollBar.Value = num;
  }

  private void ZoomTo(double scale, bool allowStackAddition)
  {
    if (this._content == null)
      return;
    this.ZoomTo(scale, this.GetZoomRelativePoint(), true, allowStackAddition);
  }

  private void ZoomTo(
    double scale,
    Point relativeTo,
    bool restrictRelativePointToContent,
    bool allowStackAddition)
  {
    if (this._content == null || double.IsNaN(scale) || restrictRelativePointToContent && !new Rect(this._content.RenderSize).Contains(relativeTo))
      return;
    if (scale > this.MaxScale)
      scale = this.MaxScale;
    else if (scale < this.MinScale)
      scale = this.MinScale;
    Point point = relativeTo;
    if (this.HasRenderedFirstView)
    {
      relativeTo = this._content.TranslatePoint(relativeTo, (UIElement) this);
      point = this.TranslatePoint(relativeTo, (UIElement) this._contentPresenter);
    }
    else if (this._contentPresenter != null)
    {
      if (this._contentPresenter.RenderTransform == Transform.Identity)
        this.UpdateView(new ZoomboxView(1.0, new Point(0.0, 0.0)), false, false);
      relativeTo = this._contentPresenter.RenderTransform.Transform(relativeTo);
    }
    Point position = new Point(relativeTo.X - point.X * scale / this._viewboxFactor, relativeTo.Y - point.Y * scale / this._viewboxFactor) + this.ContentOffset * scale / this._viewboxFactor;
    this.UpdateView(new ZoomboxView(scale, position), !this.IsResizingViewport, allowStackAddition);
  }

  private Point GetZoomRelativePoint()
  {
    Point zoomRelativePoint;
    if (this.ZoomOn == ZoomboxZoomOn.View)
    {
      Point point1 = new Point();
      ref Point local1 = ref point1;
      Rect viewport = this.Viewport;
      double x1 = viewport.X;
      viewport = this.Viewport;
      double num1 = viewport.Width * this.ZoomOrigin.X;
      double num2 = x1 + num1;
      local1.X = num2;
      ref Point local2 = ref point1;
      viewport = this.Viewport;
      double y1 = viewport.Y;
      viewport = this.Viewport;
      double num3 = viewport.Height * this.ZoomOrigin.Y;
      double num4 = y1 + num3;
      local2.Y = num4;
      Point point2 = this._trueContent.TranslatePoint(point1, this._content);
      Size renderSize;
      if (point2.X < 0.0)
      {
        point2.X = 0.0;
      }
      else
      {
        double x2 = point2.X;
        renderSize = this._content.RenderSize;
        double width1 = renderSize.Width;
        if (x2 > width1)
        {
          ref Point local3 = ref point2;
          renderSize = this._content.RenderSize;
          double width2 = renderSize.Width;
          local3.X = width2;
        }
      }
      if (point2.Y < 0.0)
      {
        point2.Y = 0.0;
      }
      else
      {
        double y2 = point2.Y;
        renderSize = this._content.RenderSize;
        double height1 = renderSize.Height;
        if (y2 > height1)
        {
          ref Point local4 = ref point2;
          renderSize = this._content.RenderSize;
          double height2 = renderSize.Height;
          local4.Y = height2;
        }
      }
      zoomRelativePoint = point2;
    }
    else
    {
      ref Point local = ref zoomRelativePoint;
      Size renderSize = this._content.RenderSize;
      double width = renderSize.Width;
      Point zoomOrigin = this.ZoomOrigin;
      double x3 = zoomOrigin.X;
      double x4 = width * x3;
      renderSize = this._content.RenderSize;
      double height = renderSize.Height;
      zoomOrigin = this.ZoomOrigin;
      double y3 = zoomOrigin.Y;
      double y4 = height * y3;
      local = new Point(x4, y4);
    }
    return zoomRelativePoint;
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if (this.NavigateOnPreview && !e.Handled)
      this.ProcessNavigationButton((RoutedEventArgs) e);
    base.OnPreviewKeyDown(e);
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    if (!this.NavigateOnPreview && !e.Handled)
      this.ProcessNavigationButton((RoutedEventArgs) e);
    base.OnKeyDown(e);
  }

  protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
  {
    if (this.NavigateOnPreview && !e.Handled)
      this.ProcessNavigationButton((RoutedEventArgs) e);
    base.OnPreviewMouseDown(e);
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    if (!this.NavigateOnPreview && !e.Handled)
      this.ProcessNavigationButton((RoutedEventArgs) e);
    base.OnMouseDown(e);
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    this.MonitorInput();
    base.OnMouseEnter(e);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    this.MonitorInput();
    base.OnMouseLeave(e);
  }

  protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (this.DragOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseLeftButtonDown(e);
    base.OnPreviewMouseLeftButtonDown(e);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (!this.DragOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseLeftButtonDown(e);
    base.OnMouseLeftButtonDown(e);
  }

  protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (this.DragOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseLeftButtonUp(e);
    base.OnPreviewMouseLeftButtonUp(e);
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (!this.DragOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseLeftButtonUp(e);
    base.OnMouseLeftButtonUp(e);
  }

  protected override void OnPreviewMouseMove(MouseEventArgs e)
  {
    if (this.DragOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseMove(e);
    base.OnPreviewMouseMove(e);
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    if (!this.DragOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseMove(e);
    base.OnMouseMove(e);
  }

  protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
  {
    if (this.ZoomOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseWheelZoom(e);
    base.OnPreviewMouseWheel(e);
  }

  protected override void OnMouseWheel(MouseWheelEventArgs e)
  {
    if (!this.ZoomOnPreview && !e.Handled && this._contentPresenter != null)
      this.ProcessMouseWheelZoom(e);
    base.OnMouseWheel(e);
  }

  private sealed class ViewFinderSelectionConverter : IValueConverter
  {
    private readonly Xceed.Wpf.Toolkit.Zoombox.Zoombox _zoombox;

    public ViewFinderSelectionConverter(Xceed.Wpf.Toolkit.Zoombox.Zoombox zoombox)
    {
      this._zoombox = zoombox;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Rect rect1 = (Rect) value;
      if (rect1.IsEmpty)
        return (object) rect1;
      double num = this._zoombox._viewFinderDisplay.Scale * this._zoombox._viewboxFactor;
      Rect rect2 = new Rect(rect1.Left * num, rect1.Top * num, rect1.Width * num, rect1.Height * num);
      rect2.Offset(this._zoombox._viewFinderDisplay.ContentBounds.Left, this._zoombox._viewFinderDisplay.ContentBounds.Top);
      return (object) rect2;
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return (object) null;
    }
  }

  internal sealed class DragAdorner : Adorner
  {
    public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(nameof (Brush), typeof (Brush), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty PenProperty = DependencyProperty.Register(nameof (Pen), typeof (Pen), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Pen((Brush) new SolidColorBrush(Color.FromArgb((byte) 127 /*0x7F*/, (byte) 63 /*0x3F*/, (byte) 63 /*0x3F*/, (byte) 63 /*0x3F*/)), 2.0), FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty RectProperty = DependencyProperty.Register(nameof (Rect), typeof (Rect), typeof (Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) Rect.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner.OnRectChanged)));
    private Point _cachedPosition;
    private Size _cachedSize;

    public DragAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
      this.ClipToBounds = true;
    }

    public Brush Brush
    {
      get => (Brush) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner.BrushProperty);
      set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner.BrushProperty, (object) value);
    }

    public Pen Pen
    {
      get => (Pen) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner.PenProperty);
      set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner.PenProperty, (object) value);
    }

    public Rect Rect
    {
      get => (Rect) this.GetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner.RectProperty);
      set => this.SetValue(Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner.RectProperty, (object) value);
    }

    private static void OnRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner dragAdorner = (Xceed.Wpf.Toolkit.Zoombox.Zoombox.DragAdorner) d;
      if (((Rect) e.NewValue).IsEmpty)
        return;
      dragAdorner._cachedPosition = ((Rect) e.NewValue).TopLeft;
      dragAdorner._cachedSize = ((Rect) e.NewValue).Size;
    }

    public Point LastPosition => this._cachedPosition;

    public Size LastSize => this._cachedSize;

    protected override void OnRender(DrawingContext drawingContext)
    {
      drawingContext.DrawRectangle(this.Brush, this.Pen, this.Rect);
    }
  }

  private enum CacheBits
  {
    IsUpdatingView = 1,
    IsUpdatingViewport = 2,
    IsDraggingViewport = 4,
    IsResizingViewport = 8,
    IsMonitoringInput = 16, // 0x00000010
    IsContentWrapped = 32, // 0x00000020
    HasArrangedContentPresenter = 64, // 0x00000040
    HasRenderedFirstView = 128, // 0x00000080
    RefocusViewOnFirstRender = 256, // 0x00000100
    HasUIPermission = 512, // 0x00000200
  }

  private enum ResizeEdge
  {
    None,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Left,
    Top,
    Right,
    Bottom,
  }
}
