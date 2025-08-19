// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.MagnifierManager
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class MagnifierManager : DependencyObject
{
  private MagnifierAdorner _adorner;
  private UIElement _element;
  public static readonly DependencyProperty CurrentProperty = DependencyProperty.RegisterAttached("Magnifier", typeof (Magnifier), typeof (UIElement), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(MagnifierManager.OnMagnifierChanged)));

  public static void SetMagnifier(UIElement element, Magnifier value)
  {
    element.SetValue(MagnifierManager.CurrentProperty, (object) value);
  }

  public static Magnifier GetMagnifier(UIElement element)
  {
    return (Magnifier) element.GetValue(MagnifierManager.CurrentProperty);
  }

  private static void OnMagnifierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (!(d is UIElement element))
      throw new ArgumentException("Magnifier can only be attached to a UIElement.");
    new MagnifierManager().AttachToMagnifier(element, e.NewValue as Magnifier);
  }

  private void Element_MouseLeave(object sender, MouseEventArgs e)
  {
    Magnifier magnifier = MagnifierManager.GetMagnifier(this._element);
    if (magnifier != null && magnifier.IsFrozen)
      return;
    this.HideAdorner();
  }

  private void Element_MouseEnter(object sender, MouseEventArgs e) => this.ShowAdorner();

  private void Element_MouseWheel(object sender, MouseWheelEventArgs e)
  {
    Magnifier magnifier = MagnifierManager.GetMagnifier(this._element);
    if (magnifier == null || !magnifier.IsUsingZoomOnMouseWheel)
      return;
    if (e.Delta < 0)
    {
      double num = magnifier.ZoomFactor + magnifier.ZoomFactorOnMouseWheel;
      magnifier.SetCurrentValue(Magnifier.ZoomFactorProperty, (object) num);
    }
    else if (e.Delta > 0)
    {
      double num = magnifier.ZoomFactor >= magnifier.ZoomFactorOnMouseWheel ? magnifier.ZoomFactor - magnifier.ZoomFactorOnMouseWheel : 0.0;
      magnifier.SetCurrentValue(Magnifier.ZoomFactorProperty, (object) num);
    }
    this._adorner.UpdateViewBox();
  }

  private void AttachToMagnifier(UIElement element, Magnifier magnifier)
  {
    this._element = element;
    this._element.MouseEnter += new MouseEventHandler(this.Element_MouseEnter);
    this._element.MouseLeave += new MouseEventHandler(this.Element_MouseLeave);
    this._element.MouseWheel += new MouseWheelEventHandler(this.Element_MouseWheel);
    magnifier.Target = this._element;
    this._adorner = new MagnifierAdorner(this._element, magnifier);
  }

  private void ShowAdorner()
  {
    this.VerifyAdornerLayer();
    this._adorner.Visibility = Visibility.Visible;
  }

  private bool VerifyAdornerLayer()
  {
    if (this._adorner.Parent != null)
      return true;
    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual) this._element);
    if (adornerLayer == null)
      return false;
    adornerLayer.Add((Adorner) this._adorner);
    return true;
  }

  private void HideAdorner()
  {
    if (this._adorner.Visibility != Visibility.Visible)
      return;
    this._adorner.Visibility = Visibility.Collapsed;
  }
}
