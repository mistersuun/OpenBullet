// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Chromes.ButtonChrome
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit.Chromes;

public class ButtonChrome : ContentControl
{
  public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof (CornerRadius), typeof (CornerRadius), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) new CornerRadius(), new PropertyChangedCallback(ButtonChrome.OnCornerRadiusChanged)));
  public static readonly DependencyProperty InnerCornerRadiusProperty = DependencyProperty.Register(nameof (InnerCornerRadius), typeof (CornerRadius), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) new CornerRadius(), new PropertyChangedCallback(ButtonChrome.OnInnerCornerRadiusChanged)));
  public static readonly DependencyProperty RenderCheckedProperty = DependencyProperty.Register(nameof (RenderChecked), typeof (bool), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(ButtonChrome.OnRenderCheckedChanged)));
  public static readonly DependencyProperty RenderEnabledProperty = DependencyProperty.Register(nameof (RenderEnabled), typeof (bool), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(ButtonChrome.OnRenderEnabledChanged)));
  public static readonly DependencyProperty RenderFocusedProperty = DependencyProperty.Register(nameof (RenderFocused), typeof (bool), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(ButtonChrome.OnRenderFocusedChanged)));
  public static readonly DependencyProperty RenderMouseOverProperty = DependencyProperty.Register(nameof (RenderMouseOver), typeof (bool), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(ButtonChrome.OnRenderMouseOverChanged)));
  public static readonly DependencyProperty RenderNormalProperty = DependencyProperty.Register(nameof (RenderNormal), typeof (bool), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(ButtonChrome.OnRenderNormalChanged)));
  public static readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register(nameof (RenderPressed), typeof (bool), typeof (ButtonChrome), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(ButtonChrome.OnRenderPressedChanged)));

  public CornerRadius CornerRadius
  {
    get => (CornerRadius) this.GetValue(ButtonChrome.CornerRadiusProperty);
    set => this.SetValue(ButtonChrome.CornerRadiusProperty, (object) value);
  }

  private static void OnCornerRadiusChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnCornerRadiusChanged((CornerRadius) e.OldValue, (CornerRadius) e.NewValue);
  }

  protected virtual void OnCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue)
  {
    this.InnerCornerRadius = new CornerRadius(Math.Max(0.0, newValue.TopLeft - 1.0), Math.Max(0.0, newValue.TopRight - 1.0), Math.Max(0.0, newValue.BottomRight - 1.0), Math.Max(0.0, newValue.BottomLeft - 1.0));
  }

  public CornerRadius InnerCornerRadius
  {
    get => (CornerRadius) this.GetValue(ButtonChrome.InnerCornerRadiusProperty);
    set => this.SetValue(ButtonChrome.InnerCornerRadiusProperty, (object) value);
  }

  private static void OnInnerCornerRadiusChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnInnerCornerRadiusChanged((CornerRadius) e.OldValue, (CornerRadius) e.NewValue);
  }

  protected virtual void OnInnerCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue)
  {
  }

  public bool RenderChecked
  {
    get => (bool) this.GetValue(ButtonChrome.RenderCheckedProperty);
    set => this.SetValue(ButtonChrome.RenderCheckedProperty, (object) value);
  }

  private static void OnRenderCheckedChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnRenderCheckedChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnRenderCheckedChanged(bool oldValue, bool newValue)
  {
  }

  public bool RenderEnabled
  {
    get => (bool) this.GetValue(ButtonChrome.RenderEnabledProperty);
    set => this.SetValue(ButtonChrome.RenderEnabledProperty, (object) value);
  }

  private static void OnRenderEnabledChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnRenderEnabledChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnRenderEnabledChanged(bool oldValue, bool newValue)
  {
  }

  public bool RenderFocused
  {
    get => (bool) this.GetValue(ButtonChrome.RenderFocusedProperty);
    set => this.SetValue(ButtonChrome.RenderFocusedProperty, (object) value);
  }

  private static void OnRenderFocusedChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnRenderFocusedChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnRenderFocusedChanged(bool oldValue, bool newValue)
  {
  }

  public bool RenderMouseOver
  {
    get => (bool) this.GetValue(ButtonChrome.RenderMouseOverProperty);
    set => this.SetValue(ButtonChrome.RenderMouseOverProperty, (object) value);
  }

  private static void OnRenderMouseOverChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnRenderMouseOverChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnRenderMouseOverChanged(bool oldValue, bool newValue)
  {
  }

  public bool RenderNormal
  {
    get => (bool) this.GetValue(ButtonChrome.RenderNormalProperty);
    set => this.SetValue(ButtonChrome.RenderNormalProperty, (object) value);
  }

  private static void OnRenderNormalChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnRenderNormalChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnRenderNormalChanged(bool oldValue, bool newValue)
  {
  }

  public bool RenderPressed
  {
    get => (bool) this.GetValue(ButtonChrome.RenderPressedProperty);
    set => this.SetValue(ButtonChrome.RenderPressedProperty, (object) value);
  }

  private static void OnRenderPressedChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ButtonChrome buttonChrome))
      return;
    buttonChrome.OnRenderPressedChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnRenderPressedChanged(bool oldValue, bool newValue)
  {
  }

  static ButtonChrome()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ButtonChrome), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ButtonChrome)));
  }
}
