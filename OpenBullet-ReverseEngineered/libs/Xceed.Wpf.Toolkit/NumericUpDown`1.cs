// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.NumericUpDown`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public abstract class NumericUpDown<T> : UpDownBase<T>
{
  public static readonly DependencyProperty AutoMoveFocusProperty = DependencyProperty.Register(nameof (AutoMoveFocus), typeof (bool), typeof (NumericUpDown<T>), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty AutoSelectBehaviorProperty = DependencyProperty.Register(nameof (AutoSelectBehavior), typeof (AutoSelectBehavior), typeof (NumericUpDown<T>), (PropertyMetadata) new UIPropertyMetadata((object) AutoSelectBehavior.OnFocus));
  public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register(nameof (FormatString), typeof (string), typeof (NumericUpDown<T>), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty, new PropertyChangedCallback(NumericUpDown<T>.OnFormatStringChanged)));
  public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(nameof (Increment), typeof (T), typeof (NumericUpDown<T>), new PropertyMetadata((object) default (T), new PropertyChangedCallback(NumericUpDown<T>.OnIncrementChanged), new CoerceValueCallback(NumericUpDown<T>.OnCoerceIncrement)));
  public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(nameof (MaxLength), typeof (int), typeof (NumericUpDown<T>), (PropertyMetadata) new UIPropertyMetadata((object) 0));

  public bool AutoMoveFocus
  {
    get => (bool) this.GetValue(NumericUpDown<T>.AutoMoveFocusProperty);
    set => this.SetValue(NumericUpDown<T>.AutoMoveFocusProperty, (object) value);
  }

  public AutoSelectBehavior AutoSelectBehavior
  {
    get => (AutoSelectBehavior) this.GetValue(NumericUpDown<T>.AutoSelectBehaviorProperty);
    set => this.SetValue(NumericUpDown<T>.AutoSelectBehaviorProperty, (object) value);
  }

  public string FormatString
  {
    get => (string) this.GetValue(NumericUpDown<T>.FormatStringProperty);
    set => this.SetValue(NumericUpDown<T>.FormatStringProperty, (object) value);
  }

  private static void OnFormatStringChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is NumericUpDown<T> numericUpDown))
      return;
    numericUpDown.OnFormatStringChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnFormatStringChanged(string oldValue, string newValue)
  {
    if (!this.IsInitialized)
      return;
    this.SyncTextAndValueProperties(false, (string) null);
  }

  public T Increment
  {
    get => (T) this.GetValue(NumericUpDown<T>.IncrementProperty);
    set => this.SetValue(NumericUpDown<T>.IncrementProperty, (object) value);
  }

  private static void OnIncrementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is NumericUpDown<T> numericUpDown))
      return;
    numericUpDown.OnIncrementChanged((T) e.OldValue, (T) e.NewValue);
  }

  protected virtual void OnIncrementChanged(T oldValue, T newValue)
  {
    if (!this.IsInitialized)
      return;
    this.SetValidSpinDirection();
  }

  private static object OnCoerceIncrement(DependencyObject d, object baseValue)
  {
    return d is NumericUpDown<T> numericUpDown ? (object) numericUpDown.OnCoerceIncrement((T) baseValue) : baseValue;
  }

  protected virtual T OnCoerceIncrement(T baseValue) => baseValue;

  public int MaxLength
  {
    get => (int) this.GetValue(NumericUpDown<T>.MaxLengthProperty);
    set => this.SetValue(NumericUpDown<T>.MaxLengthProperty, (object) value);
  }

  protected static Decimal ParsePercent(string text, IFormatProvider cultureInfo)
  {
    NumberFormatInfo instance = NumberFormatInfo.GetInstance(cultureInfo);
    text = text.Replace(instance.PercentSymbol, (string) null);
    return Decimal.Parse(text, NumberStyles.Any, (IFormatProvider) instance) / 100M;
  }
}
