// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Spinner
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public abstract class Spinner : Control
{
  public static readonly DependencyProperty ValidSpinDirectionProperty = DependencyProperty.Register(nameof (ValidSpinDirection), typeof (ValidSpinDirections), typeof (Spinner), new PropertyMetadata((object) (ValidSpinDirections.Increase | ValidSpinDirections.Decrease), new PropertyChangedCallback(Spinner.OnValidSpinDirectionPropertyChanged)));
  public static readonly RoutedEvent SpinnerSpinEvent = EventManager.RegisterRoutedEvent("SpinnerSpin", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (Spinner));

  public ValidSpinDirections ValidSpinDirection
  {
    get => (ValidSpinDirections) this.GetValue(Spinner.ValidSpinDirectionProperty);
    set => this.SetValue(Spinner.ValidSpinDirectionProperty, (object) value);
  }

  private static void OnValidSpinDirectionPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Spinner spinner = (Spinner) d;
    ValidSpinDirections oldValue1 = (ValidSpinDirections) e.OldValue;
    ValidSpinDirections newValue1 = (ValidSpinDirections) e.NewValue;
    int oldValue2 = (int) oldValue1;
    int newValue2 = (int) newValue1;
    spinner.OnValidSpinDirectionChanged((ValidSpinDirections) oldValue2, (ValidSpinDirections) newValue2);
  }

  public event EventHandler<SpinEventArgs> Spin;

  public event RoutedEventHandler SpinnerSpin
  {
    add => this.AddHandler(Spinner.SpinnerSpinEvent, (Delegate) value);
    remove => this.RemoveHandler(Spinner.SpinnerSpinEvent, (Delegate) value);
  }

  protected virtual void OnSpin(SpinEventArgs e)
  {
    ValidSpinDirections validSpinDirections = e.Direction == SpinDirection.Increase ? ValidSpinDirections.Increase : ValidSpinDirections.Decrease;
    if ((this.ValidSpinDirection & validSpinDirections) != validSpinDirections)
      return;
    EventHandler<SpinEventArgs> spin = this.Spin;
    if (spin == null)
      return;
    spin((object) this, e);
  }

  protected virtual void OnValidSpinDirectionChanged(
    ValidSpinDirections oldValue,
    ValidSpinDirections newValue)
  {
  }
}
