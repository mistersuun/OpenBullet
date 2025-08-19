// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PrimitiveTypeCollectionControl
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class PrimitiveTypeCollectionControl : ContentControl
{
  private bool _surpressTextChanged;
  private bool _conversionFailed;
  public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (PrimitiveTypeCollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(PrimitiveTypeCollectionControl.OnIsOpenChanged)));
  public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof (ItemsSource), typeof (IList), typeof (PrimitiveTypeCollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(PrimitiveTypeCollectionControl.OnItemsSourceChanged)));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (PrimitiveTypeCollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ItemsSourceTypeProperty = DependencyProperty.Register(nameof (ItemsSourceType), typeof (Type), typeof (PrimitiveTypeCollectionControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.Register(nameof (ItemType), typeof (Type), typeof (PrimitiveTypeCollectionControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (PrimitiveTypeCollectionControl), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(PrimitiveTypeCollectionControl.OnTextChanged)));

  public bool IsOpen
  {
    get => (bool) this.GetValue(PrimitiveTypeCollectionControl.IsOpenProperty);
    set => this.SetValue(PrimitiveTypeCollectionControl.IsOpenProperty, (object) value);
  }

  private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is PrimitiveTypeCollectionControl collectionControl))
      return;
    collectionControl.OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
  {
  }

  public IList ItemsSource
  {
    get => (IList) this.GetValue(PrimitiveTypeCollectionControl.ItemsSourceProperty);
    set => this.SetValue(PrimitiveTypeCollectionControl.ItemsSourceProperty, (object) value);
  }

  private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is PrimitiveTypeCollectionControl collectionControl))
      return;
    collectionControl.OnItemsSourceChanged((IList) e.OldValue, (IList) e.NewValue);
  }

  protected virtual void OnItemsSourceChanged(IList oldValue, IList newValue)
  {
    if (newValue == null)
      return;
    if (this.ItemsSourceType == (Type) null)
      this.ItemsSourceType = newValue.GetType();
    if (this.ItemType == (Type) null && newValue.GetType().ContainsGenericParameters)
      this.ItemType = newValue.GetType().GetGenericArguments()[0];
    this.SetText((IEnumerable) newValue);
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(PrimitiveTypeCollectionControl.IsReadOnlyProperty);
    set => this.SetValue(PrimitiveTypeCollectionControl.IsReadOnlyProperty, (object) value);
  }

  public Type ItemsSourceType
  {
    get => (Type) this.GetValue(PrimitiveTypeCollectionControl.ItemsSourceTypeProperty);
    set => this.SetValue(PrimitiveTypeCollectionControl.ItemsSourceTypeProperty, (object) value);
  }

  public Type ItemType
  {
    get => (Type) this.GetValue(PrimitiveTypeCollectionControl.ItemTypeProperty);
    set => this.SetValue(PrimitiveTypeCollectionControl.ItemTypeProperty, (object) value);
  }

  public string Text
  {
    get => (string) this.GetValue(PrimitiveTypeCollectionControl.TextProperty);
    set => this.SetValue(PrimitiveTypeCollectionControl.TextProperty, (object) value);
  }

  private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is PrimitiveTypeCollectionControl collectionControl))
      return;
    collectionControl.OnTextChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnTextChanged(string oldValue, string newValue)
  {
    if (this._surpressTextChanged)
      return;
    this.PersistChanges();
  }

  static PrimitiveTypeCollectionControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (PrimitiveTypeCollectionControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (PrimitiveTypeCollectionControl)));
  }

  private void PersistChanges()
  {
    IList itemsSource = this.ComputeItemsSource();
    if (itemsSource == null)
      return;
    IList items = this.ComputeItems();
    itemsSource.Clear();
    int num = 0;
    foreach (object obj in (IEnumerable) items)
    {
      if (itemsSource is Array)
        ((Array) itemsSource).SetValue(obj, num++);
      else
        itemsSource.Add(obj);
    }
    if (!this._conversionFailed)
      return;
    this.SetText((IEnumerable) itemsSource);
  }

  private IList ComputeItems()
  {
    IList items = (IList) new List<object>();
    if (this.ItemType == (Type) null)
      return items;
    string text = this.Text;
    char[] chArray1 = new char[1]{ '\n' };
    foreach (string str1 in text.Split(chArray1))
    {
      char[] chArray2 = new char[1]{ '\r' };
      string str2 = str1.TrimEnd(chArray2);
      if (!string.IsNullOrEmpty(str2))
      {
        object obj = (object) null;
        try
        {
          obj = !this.ItemType.IsEnum ? Convert.ChangeType((object) str2, this.ItemType) : Enum.Parse(this.ItemType, str2);
        }
        catch
        {
          this._conversionFailed = true;
        }
        if (obj != null)
          items.Add(obj);
      }
    }
    return items;
  }

  private IList ComputeItemsSource()
  {
    if (this.ItemsSource == null)
    {
      string text = this.Text;
      this.ItemsSource = this.CreateItemsSource();
      this.Text = text;
    }
    return this.ItemsSource;
  }

  private IList CreateItemsSource()
  {
    IList itemsSource = (IList) null;
    if (this.ItemsSourceType != (Type) null)
      itemsSource = (IList) this.ItemsSourceType.GetConstructor(Type.EmptyTypes).Invoke((object[]) null);
    return itemsSource;
  }

  private void SetText(IEnumerable collection)
  {
    this._surpressTextChanged = true;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (object obj in collection)
    {
      stringBuilder.Append(obj.ToString());
      stringBuilder.AppendLine();
    }
    this.Text = stringBuilder.ToString().Trim();
    this._surpressTextChanged = false;
  }
}
