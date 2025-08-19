// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.BindingEvaluator`1
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Windows.Data;

#nullable disable
namespace System.Windows.Controls;

internal class BindingEvaluator<T> : FrameworkElement
{
  private Binding _binding;
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (T), typeof (BindingEvaluator<T>), new PropertyMetadata((object) default (T)));

  public T Value
  {
    get => (T) this.GetValue(BindingEvaluator<T>.ValueProperty);
    set => this.SetValue(BindingEvaluator<T>.ValueProperty, (object) value);
  }

  public Binding ValueBinding
  {
    get => this._binding;
    set
    {
      this._binding = value;
      this.SetBinding(BindingEvaluator<T>.ValueProperty, (BindingBase) this._binding);
    }
  }

  public BindingEvaluator()
  {
  }

  public BindingEvaluator(Binding binding)
  {
    this.SetBinding(BindingEvaluator<T>.ValueProperty, (BindingBase) binding);
  }

  public void ClearDataContext() => this.DataContext = (object) null;

  public T GetDynamicValue(object o, bool clearDataContext)
  {
    this.DataContext = o;
    T dynamicValue = this.Value;
    if (clearDataContext)
      this.DataContext = (object) null;
    return dynamicValue;
  }

  public T GetDynamicValue(object o)
  {
    this.DataContext = o;
    return this.Value;
  }
}
