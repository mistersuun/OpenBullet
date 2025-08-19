// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.CustomPropertyItem
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Linq.Expressions;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class CustomPropertyItem : PropertyItemBase
{
  public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(nameof (Category), typeof (string), typeof (CustomPropertyItem), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  private int _categoryOrder;
  public static readonly DependencyProperty PropertyOrderProperty = DependencyProperty.Register(nameof (PropertyOrder), typeof (int), typeof (CustomPropertyItem), (PropertyMetadata) new UIPropertyMetadata((object) 0));
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (object), typeof (CustomPropertyItem), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(CustomPropertyItem.OnValueChanged), new CoerceValueCallback(CustomPropertyItem.OnCoerceValueChanged)));

  internal CustomPropertyItem()
  {
  }

  internal CustomPropertyItem(bool isPropertyGridCategorized, bool isSortedAlphabetically)
  {
    this._isPropertyGridCategorized = isPropertyGridCategorized;
    this._isSortedAlphabetically = isSortedAlphabetically;
  }

  public string Category
  {
    get => (string) this.GetValue(CustomPropertyItem.CategoryProperty);
    set => this.SetValue(CustomPropertyItem.CategoryProperty, (object) value);
  }

  public int CategoryOrder
  {
    get => this._categoryOrder;
    set
    {
      if (this._categoryOrder == value)
        return;
      this._categoryOrder = value;
      this.RaisePropertyChanged<int>((Expression<Func<int>>) (() => this.CategoryOrder));
    }
  }

  public int PropertyOrder
  {
    get => (int) this.GetValue(CustomPropertyItem.PropertyOrderProperty);
    set => this.SetValue(CustomPropertyItem.PropertyOrderProperty, (object) value);
  }

  public object Value
  {
    get => this.GetValue(CustomPropertyItem.ValueProperty);
    set => this.SetValue(CustomPropertyItem.ValueProperty, value);
  }

  private static object OnCoerceValueChanged(DependencyObject o, object baseValue)
  {
    return o is CustomPropertyItem customPropertyItem ? customPropertyItem.OnCoerceValueChanged(baseValue) : baseValue;
  }

  protected virtual object OnCoerceValueChanged(object baseValue) => baseValue;

  private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is CustomPropertyItem customPropertyItem))
      return;
    customPropertyItem.OnValueChanged(e.OldValue, e.NewValue);
  }

  protected virtual void OnValueChanged(object oldValue, object newValue)
  {
    if (!this.IsInitialized)
      return;
    this.RaiseEvent((RoutedEventArgs) new PropertyValueChangedEventArgs(Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid.PropertyValueChangedEvent, (object) this, oldValue, newValue));
  }

  protected override Type GetPropertyItemType() => this.Value.GetType();

  protected override void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
  {
    if (oldValue != null)
      oldValue.DataContext = (object) null;
    if (newValue == null || newValue.DataContext != null)
      return;
    newValue.DataContext = (object) this;
  }
}
