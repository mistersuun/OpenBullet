// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.RoutedPropertyChangingEventArgs`1
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Windows.Controls.Properties;

#nullable disable
namespace System.Windows.Controls;

public class RoutedPropertyChangingEventArgs<T> : RoutedEventArgs
{
  private bool _cancel;

  public DependencyProperty Property { get; private set; }

  public T OldValue { get; private set; }

  public T NewValue { get; set; }

  public bool IsCancelable { get; private set; }

  public bool Cancel
  {
    get => this._cancel;
    set
    {
      if (this.IsCancelable)
        this._cancel = value;
      else if (value)
        throw new InvalidOperationException(Resources.RoutedPropertyChangingEventArgs_CancelSet_InvalidOperation);
    }
  }

  public bool InCoercion { get; set; }

  public RoutedPropertyChangingEventArgs(
    DependencyProperty property,
    T oldValue,
    T newValue,
    bool isCancelable)
  {
    this.Property = property;
    this.OldValue = oldValue;
    this.NewValue = newValue;
    this.IsCancelable = isCancelable;
    this.Cancel = false;
  }

  public RoutedPropertyChangingEventArgs(
    DependencyProperty property,
    T oldValue,
    T newValue,
    bool isCancelable,
    RoutedEvent routedEvent)
    : base(routedEvent)
  {
    this.Property = property;
    this.OldValue = oldValue;
    this.NewValue = newValue;
    this.IsCancelable = isCancelable;
    this.Cancel = false;
  }
}
