// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.SwitchPresenter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public class SwitchPresenter : FrameworkElement
{
  public static readonly DependencyProperty DelaySwitchProperty = DependencyProperty.Register(nameof (DelaySwitch), typeof (bool), typeof (SwitchPresenter), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty DelayPriorityProperty = DependencyProperty.Register(nameof (DelayPriority), typeof (DispatcherPriority), typeof (SwitchPresenter), (PropertyMetadata) new UIPropertyMetadata((object) DispatcherPriority.Background));
  internal static readonly DependencyProperty SwitchParentProperty = DependencyProperty.Register(nameof (SwitchParent), typeof (SwitchPanel), typeof (SwitchPresenter), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(SwitchPresenter.OnSwitchParentChanged)));
  internal UIElement _switchRoot;
  internal Dictionary<string, FrameworkElement> _knownIDs = new Dictionary<string, FrameworkElement>();
  private ContentPresenter _contentPresenter = new ContentPresenter();
  private bool _isMeasured;
  private DataTemplate _currentTemplate;

  public SwitchPresenter()
  {
    this.AddVisualChild((Visual) this._contentPresenter);
    this.Loaded += new RoutedEventHandler(this.SwitchPresenter_Loaded);
    this.Unloaded += new RoutedEventHandler(this.SwitchPresenter_Unloaded);
  }

  public bool DelaySwitch
  {
    get => (bool) this.GetValue(SwitchPresenter.DelaySwitchProperty);
    set => this.SetValue(SwitchPresenter.DelaySwitchProperty, (object) value);
  }

  public DispatcherPriority DelayPriority
  {
    get => (DispatcherPriority) this.GetValue(SwitchPresenter.DelayPriorityProperty);
    set => this.SetValue(SwitchPresenter.DelayPriorityProperty, (object) value);
  }

  internal SwitchPanel SwitchParent
  {
    get => (SwitchPanel) this.GetValue(SwitchPresenter.SwitchParentProperty);
    set => this.SetValue(SwitchPresenter.SwitchParentProperty, (object) value);
  }

  private static void OnSwitchParentChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((SwitchPresenter) d).OnSwitchParentChanged(e);
  }

  protected virtual void OnSwitchParentChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.OldValue != null)
    {
      (e.OldValue as SwitchPanel).UnregisterPresenter(this, (DependencyObject) this._switchRoot);
      this._switchRoot = (UIElement) null;
      BindingOperations.ClearAllBindings((DependencyObject) this._contentPresenter);
    }
    if (e.NewValue == null)
      return;
    this._contentPresenter.SetBinding(ContentPresenter.ContentProperty, (BindingBase) new Binding());
    this._switchRoot = (e.NewValue as SwitchPanel).RegisterPresenter(this);
  }

  private static void OnLoaded(object sender, RoutedEventArgs e)
  {
    SwitchPresenter element = sender as SwitchPresenter;
    if (element._switchRoot != null)
      return;
    element.SwitchParent = VisualTreeHelperEx.FindAncestorByType((DependencyObject) element, typeof (SwitchPanel), false) as SwitchPanel;
  }

  private static void OnUnloaded(object sender, RoutedEventArgs e)
  {
    (sender as SwitchPresenter).SwitchParent = (SwitchPanel) null;
  }

  protected override int VisualChildrenCount => 1;

  internal void RegisterID(string id, FrameworkElement element)
  {
    if (element == null)
      return;
    this._knownIDs[id] = element;
  }

  internal void SwapTheTemplate(DataTemplate template, bool beginAnimation)
  {
    if (this.DelaySwitch)
    {
      this._currentTemplate = template;
      this.Dispatcher.BeginInvoke((Delegate) new Action<SwitchPresenter.DelaySwitchParams>(this.OnSwapTemplate), this.DelayPriority, (object) new SwitchPresenter.DelaySwitchParams()
      {
        Template = template,
        BeginAnimation = beginAnimation
      });
    }
    else
      this.DoSwapTemplate(template, beginAnimation);
  }

  protected override Size MeasureOverride(Size constraint)
  {
    if (!this._isMeasured && this._switchRoot == null)
    {
      SwitchPresenter.OnLoaded((object) this, (RoutedEventArgs) null);
      this._isMeasured = true;
    }
    this._contentPresenter.Measure(constraint);
    return this._contentPresenter.DesiredSize;
  }

  protected override Size ArrangeOverride(Size arrangeBounds)
  {
    this._contentPresenter.Arrange(new Rect(arrangeBounds));
    return arrangeBounds;
  }

  protected override Visual GetVisualChild(int index)
  {
    if (index != 0)
      throw new ArgumentOutOfRangeException(nameof (index), (object) index, "");
    return (Visual) this._contentPresenter;
  }

  private void OnSwapTemplate(SwitchPresenter.DelaySwitchParams data)
  {
    if (data.Template != this._currentTemplate)
      return;
    this.DoSwapTemplate(data.Template, data.BeginAnimation);
    this._currentTemplate = (DataTemplate) null;
  }

  private void DoSwapTemplate(DataTemplate template, bool beginAnimation)
  {
    Dictionary<string, Rect> knownLocations = (Dictionary<string, Rect>) null;
    if (beginAnimation && this._knownIDs.Count > 0)
    {
      knownLocations = new Dictionary<string, Rect>();
      foreach (KeyValuePair<string, FrameworkElement> knownId in this._knownIDs)
      {
        Size renderSize = knownId.Value.RenderSize;
        Matrix matrix = (knownId.Value.TransformToAncestor((Visual) this.SwitchParent) as MatrixTransform).Matrix;
        Point[] pointArray = new Point[2];
        pointArray[1] = new Point(renderSize.Width, renderSize.Height);
        Point[] points = pointArray;
        matrix.Transform(points);
        knownLocations[knownId.Key] = new Rect(points[0], points[1]);
      }
    }
    this._knownIDs.Clear();
    this._contentPresenter.ContentTemplate = template;
    if (template != null)
      this._contentPresenter.ApplyTemplate();
    if (knownLocations == null || this._knownIDs.Count <= 0)
      return;
    Dictionary<string, Rect> newLocations = (Dictionary<string, Rect>) null;
    RoutedEventHandler onLoaded = (RoutedEventHandler) null;
    onLoaded = (RoutedEventHandler) ((sender, e) =>
    {
      FrameworkElement frameworkElement = sender as FrameworkElement;
      frameworkElement.Loaded -= onLoaded;
      string id = SwitchTemplate.GetID((DependencyObject) frameworkElement);
      if (!knownLocations.ContainsKey(id))
        return;
      if (newLocations == null)
        newLocations = this.SwitchParent.ActiveLayout.GetNewLocationsBasedOnTargetPlacement(this, this._switchRoot);
      if (!(VisualTreeHelper.GetParent((DependencyObject) frameworkElement) is UIElement parent2))
        return;
      Rect rect = knownLocations[id];
      Point[] points = new Point[2]
      {
        rect.TopLeft,
        rect.BottomRight
      };
      (this.SwitchParent.TransformToDescendant((Visual) parent2) as MatrixTransform).Matrix.Transform(points);
      Rect currentRect = new Rect(points[0], points[1]);
      Rect placementRect = newLocations[id];
      this.SwitchParent.ActiveLayout.BeginGrandchildAnimation(frameworkElement, currentRect, placementRect);
    });
    foreach (KeyValuePair<string, FrameworkElement> knownId in this._knownIDs)
      knownId.Value.Loaded += onLoaded;
  }

  private void SwitchPresenter_Unloaded(object sender, RoutedEventArgs e)
  {
    this.SwitchParent = (SwitchPanel) null;
  }

  private void SwitchPresenter_Loaded(object sender, RoutedEventArgs e)
  {
    if (this._switchRoot != null)
      return;
    this.SwitchParent = VisualTreeHelperEx.FindAncestorByType((DependencyObject) this, typeof (SwitchPanel), false) as SwitchPanel;
  }

  private struct DelaySwitchParams
  {
    public DataTemplate Template;
    public bool BeginAnimation;
  }
}
