// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.ValueChangeHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal class ValueChangeHelper : DependencyObject
{
  private static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (object), typeof (ValueChangeHelper), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(ValueChangeHelper.OnValueChanged)));

  private object Value
  {
    get => this.GetValue(ValueChangeHelper.ValueProperty);
    set => this.SetValue(ValueChangeHelper.ValueProperty, value);
  }

  private static void OnValueChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    ((ValueChangeHelper) sender).RaiseValueChanged();
  }

  public event EventHandler ValueChanged;

  public ValueChangeHelper(Action changeCallback)
  {
    if (changeCallback == null)
      throw new ArgumentNullException(nameof (changeCallback));
    this.ValueChanged += (EventHandler) ((s, args) => changeCallback());
  }

  public void UpdateValueSource(object sourceItem, string path)
  {
    BindingBase binding = (BindingBase) null;
    if (sourceItem != null && path != null)
      binding = (BindingBase) new Binding(path)
      {
        Source = sourceItem
      };
    this.UpdateBinding(binding);
  }

  public void UpdateValueSource(IEnumerable sourceItems, string path)
  {
    BindingBase binding = (BindingBase) null;
    if (sourceItems != null && path != null)
    {
      MultiBinding multiBinding = new MultiBinding();
      multiBinding.Converter = (IMultiValueConverter) new ValueChangeHelper.BlankMultiValueConverter();
      foreach (object sourceItem in sourceItems)
        multiBinding.Bindings.Add((BindingBase) new Binding(path)
        {
          Source = sourceItem
        });
      binding = (BindingBase) multiBinding;
    }
    this.UpdateBinding(binding);
  }

  private void UpdateBinding(BindingBase binding)
  {
    if (binding != null)
      BindingOperations.SetBinding((DependencyObject) this, ValueChangeHelper.ValueProperty, binding);
    else
      this.ClearBinding();
  }

  private void ClearBinding()
  {
    BindingOperations.ClearBinding((DependencyObject) this, ValueChangeHelper.ValueProperty);
  }

  private void RaiseValueChanged()
  {
    if (this.ValueChanged == null)
      return;
    this.ValueChanged((object) this, EventArgs.Empty);
  }

  private class BlankMultiValueConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return new object();
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      throw new InvalidOperationException();
    }
  }
}
