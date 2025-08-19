// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.BusyIndicator
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplateVisualState(Name = "Idle", GroupName = "BusyStatusStates")]
[TemplateVisualState(Name = "Busy", GroupName = "BusyStatusStates")]
[TemplateVisualState(Name = "Visible", GroupName = "VisibilityStates")]
[TemplateVisualState(Name = "Hidden", GroupName = "VisibilityStates")]
[StyleTypedProperty(Property = "OverlayStyle", StyleTargetType = typeof (Rectangle))]
[StyleTypedProperty(Property = "ProgressBarStyle", StyleTargetType = typeof (ProgressBar))]
public class BusyIndicator : ContentControl
{
  private DispatcherTimer _displayAfterTimer = new DispatcherTimer();
  public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(nameof (IsBusy), typeof (bool), typeof (BusyIndicator), new PropertyMetadata((object) false, new PropertyChangedCallback(BusyIndicator.OnIsBusyChanged)));
  public static readonly DependencyProperty BusyContentProperty = DependencyProperty.Register(nameof (BusyContent), typeof (object), typeof (BusyIndicator), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty BusyContentTemplateProperty = DependencyProperty.Register(nameof (BusyContentTemplate), typeof (DataTemplate), typeof (BusyIndicator), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DisplayAfterProperty = DependencyProperty.Register(nameof (DisplayAfter), typeof (TimeSpan), typeof (BusyIndicator), new PropertyMetadata((object) TimeSpan.FromSeconds(0.1)));
  public static readonly DependencyProperty FocusAfterBusyProperty = DependencyProperty.Register(nameof (FocusAfterBusy), typeof (Control), typeof (BusyIndicator), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty OverlayStyleProperty = DependencyProperty.Register(nameof (OverlayStyle), typeof (Style), typeof (BusyIndicator), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ProgressBarStyleProperty = DependencyProperty.Register(nameof (ProgressBarStyle), typeof (Style), typeof (BusyIndicator), new PropertyMetadata((PropertyChangedCallback) null));

  static BusyIndicator()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (BusyIndicator), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (BusyIndicator)));
  }

  public BusyIndicator()
  {
    this._displayAfterTimer.Tick += new EventHandler(this.DisplayAfterTimerElapsed);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.ChangeVisualState(false);
  }

  protected bool IsContentVisible { get; set; }

  public bool IsBusy
  {
    get => (bool) this.GetValue(BusyIndicator.IsBusyProperty);
    set => this.SetValue(BusyIndicator.IsBusyProperty, (object) value);
  }

  private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((BusyIndicator) d).OnIsBusyChanged(e);
  }

  protected virtual void OnIsBusyChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.IsBusy)
    {
      if (this.DisplayAfter.Equals(TimeSpan.Zero))
      {
        this.IsContentVisible = true;
      }
      else
      {
        this._displayAfterTimer.Interval = this.DisplayAfter;
        this._displayAfterTimer.Start();
      }
    }
    else
    {
      this._displayAfterTimer.Stop();
      this.IsContentVisible = false;
      if (this.FocusAfterBusy != null)
        this.FocusAfterBusy.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) (() => this.FocusAfterBusy.Focus()));
    }
    this.ChangeVisualState(true);
  }

  public object BusyContent
  {
    get => this.GetValue(BusyIndicator.BusyContentProperty);
    set => this.SetValue(BusyIndicator.BusyContentProperty, value);
  }

  public DataTemplate BusyContentTemplate
  {
    get => (DataTemplate) this.GetValue(BusyIndicator.BusyContentTemplateProperty);
    set => this.SetValue(BusyIndicator.BusyContentTemplateProperty, (object) value);
  }

  public TimeSpan DisplayAfter
  {
    get => (TimeSpan) this.GetValue(BusyIndicator.DisplayAfterProperty);
    set => this.SetValue(BusyIndicator.DisplayAfterProperty, (object) value);
  }

  public Control FocusAfterBusy
  {
    get => (Control) this.GetValue(BusyIndicator.FocusAfterBusyProperty);
    set => this.SetValue(BusyIndicator.FocusAfterBusyProperty, (object) value);
  }

  public Style OverlayStyle
  {
    get => (Style) this.GetValue(BusyIndicator.OverlayStyleProperty);
    set => this.SetValue(BusyIndicator.OverlayStyleProperty, (object) value);
  }

  public Style ProgressBarStyle
  {
    get => (Style) this.GetValue(BusyIndicator.ProgressBarStyleProperty);
    set => this.SetValue(BusyIndicator.ProgressBarStyleProperty, (object) value);
  }

  private void DisplayAfterTimerElapsed(object sender, EventArgs e)
  {
    this._displayAfterTimer.Stop();
    this.IsContentVisible = true;
    this.ChangeVisualState(true);
  }

  protected new virtual void ChangeVisualState(bool useTransitions)
  {
    VisualStateManager.GoToState((FrameworkElement) this, this.IsBusy ? "Busy" : "Idle", useTransitions);
    VisualStateManager.GoToState((FrameworkElement) this, this.IsContentVisible ? "Visible" : "Hidden", useTransitions);
  }
}
