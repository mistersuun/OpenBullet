// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.CheckComboBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_Popup", Type = typeof (Popup))]
public class CheckComboBox : SelectAllSelector
{
  private const string PART_Popup = "PART_Popup";
  private ValueChangeHelper _displayMemberPathValuesChangeHelper;
  private bool _ignoreTextValueChanged;
  private Popup _popup;
  private List<object> _initialValue = new List<object>();
  public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(nameof (IsEditable), typeof (bool), typeof (CheckComboBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (CheckComboBox), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(CheckComboBox.OnTextChanged)));
  public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof (IsDropDownOpen), typeof (bool), typeof (CheckComboBox), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(CheckComboBox.OnIsDropDownOpenChanged)));
  public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof (MaxDropDownHeight), typeof (double), typeof (CheckComboBox), (PropertyMetadata) new UIPropertyMetadata((object) (SystemParameters.PrimaryScreenHeight / 3.0), new PropertyChangedCallback(CheckComboBox.OnMaxDropDownHeightChanged)));
  public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof (EventHandler), typeof (CheckComboBox));
  public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof (EventHandler), typeof (CheckComboBox));

  static CheckComboBox()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CheckComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CheckComboBox)));
  }

  public CheckComboBox()
  {
    Keyboard.AddKeyDownHandler((DependencyObject) this, new KeyEventHandler(this.OnKeyDown));
    Mouse.AddPreviewMouseDownOutsideCapturedElementHandler((DependencyObject) this, new MouseButtonEventHandler(this.OnMouseDownOutsideCapturedElement));
    this._displayMemberPathValuesChangeHelper = new ValueChangeHelper(new Action(this.OnDisplayMemberPathValuesChanged));
  }

  public bool IsEditable
  {
    get => (bool) this.GetValue(CheckComboBox.IsEditableProperty);
    set => this.SetValue(CheckComboBox.IsEditableProperty, (object) value);
  }

  public string Text
  {
    get => (string) this.GetValue(CheckComboBox.TextProperty);
    set => this.SetValue(CheckComboBox.TextProperty, (object) value);
  }

  private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is CheckComboBox checkComboBox))
      return;
    checkComboBox.OnTextChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnTextChanged(string oldValue, string newValue)
  {
    if (!this.IsInitialized || this._ignoreTextValueChanged || !this.IsEditable)
      return;
    this.UpdateFromText();
  }

  public bool IsDropDownOpen
  {
    get => (bool) this.GetValue(CheckComboBox.IsDropDownOpenProperty);
    set => this.SetValue(CheckComboBox.IsDropDownOpenProperty, (object) value);
  }

  private static void OnIsDropDownOpenChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is CheckComboBox checkComboBox))
      return;
    checkComboBox.OnIsDropDownOpenChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsDropDownOpenChanged(bool oldValue, bool newValue)
  {
    if (newValue)
    {
      this._initialValue.Clear();
      foreach (object selectedItem in (IEnumerable) this.SelectedItems)
        this._initialValue.Add(selectedItem);
      this.RaiseEvent(new RoutedEventArgs(CheckComboBox.OpenedEvent, (object) this));
    }
    else
    {
      this._initialValue.Clear();
      this.RaiseEvent(new RoutedEventArgs(CheckComboBox.ClosedEvent, (object) this));
    }
  }

  public double MaxDropDownHeight
  {
    get => (double) this.GetValue(CheckComboBox.MaxDropDownHeightProperty);
    set => this.SetValue(CheckComboBox.MaxDropDownHeightProperty, (object) value);
  }

  private static void OnMaxDropDownHeightChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is CheckComboBox checkComboBox))
      return;
    checkComboBox.OnMaxDropDownHeightChanged((double) e.OldValue, (double) e.NewValue);
  }

  protected virtual void OnMaxDropDownHeightChanged(double oldValue, double newValue)
  {
  }

  protected override void OnSelectedValueChanged(string oldValue, string newValue)
  {
    base.OnSelectedValueChanged(oldValue, newValue);
    this.UpdateText();
  }

  protected override void OnDisplayMemberPathChanged(
    string oldDisplayMemberPath,
    string newDisplayMemberPath)
  {
    base.OnDisplayMemberPathChanged(oldDisplayMemberPath, newDisplayMemberPath);
    this.UpdateDisplayMemberPathValuesBindings();
  }

  protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
  {
    base.OnItemsSourceChanged(oldValue, newValue);
    this.UpdateDisplayMemberPathValuesBindings();
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._popup != null)
      this._popup.Opened -= new EventHandler(this.Popup_Opened);
    this._popup = this.GetTemplateChild("PART_Popup") as Popup;
    if (this._popup == null)
      return;
    this._popup.Opened += new EventHandler(this.Popup_Opened);
  }

  private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
  {
    this.CloseDropDown(true);
  }

  private void OnKeyDown(object sender, KeyEventArgs e)
  {
    if (!this.IsDropDownOpen)
    {
      if (!KeyboardUtilities.IsKeyModifyingPopupState(e))
        return;
      this.IsDropDownOpen = true;
      e.Handled = true;
    }
    else if (KeyboardUtilities.IsKeyModifyingPopupState(e))
    {
      this.CloseDropDown(true);
      e.Handled = true;
    }
    else if (e.Key == Key.Return)
    {
      this.CloseDropDown(true);
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Escape)
        return;
      this.SelectedItems.Clear();
      foreach (object obj in this._initialValue)
        this.SelectedItems.Add(obj);
      this.CloseDropDown(true);
      e.Handled = true;
    }
  }

  private void Popup_Opened(object sender, EventArgs e)
  {
    if (!(this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) is UIElement uiElement) && this.Items.Count > 0)
      uiElement = this.ItemContainerGenerator.ContainerFromItem(this.Items[0]) as UIElement;
    uiElement?.Focus();
  }

  public event RoutedEventHandler Closed
  {
    add => this.AddHandler(CheckComboBox.ClosedEvent, (Delegate) value);
    remove => this.RemoveHandler(CheckComboBox.ClosedEvent, (Delegate) value);
  }

  public event RoutedEventHandler Opened
  {
    add => this.AddHandler(CheckComboBox.OpenedEvent, (Delegate) value);
    remove => this.RemoveHandler(CheckComboBox.OpenedEvent, (Delegate) value);
  }

  protected virtual void UpdateText()
  {
    string str = string.Join<object>(this.Delimiter, this.SelectedItems.Cast<object>().Select<object, object>((Func<object, object>) (x => this.GetItemDisplayValue(x))));
    if (!string.IsNullOrEmpty(this.Text) && this.Text.Equals(str))
      return;
    this._ignoreTextValueChanged = true;
    this.SetCurrentValue(CheckComboBox.TextProperty, (object) str);
    this._ignoreTextValueChanged = false;
  }

  private void UpdateDisplayMemberPathValuesBindings()
  {
    this._displayMemberPathValuesChangeHelper.UpdateValueSource(this.ItemsCollection, this.DisplayMemberPath);
  }

  private void OnDisplayMemberPathValuesChanged() => this.UpdateText();

  private void UpdateFromText()
  {
    List<string> selectedValues = (List<string>) null;
    if (!string.IsNullOrEmpty(this.Text))
      selectedValues = ((IEnumerable<string>) this.Text.Replace(" ", string.Empty).Split(new string[1]
      {
        this.Delimiter
      }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
    this.UpdateFromList(selectedValues, new Func<object, object>(this.GetItemDisplayValue));
  }

  protected object GetItemDisplayValue(object item)
  {
    if (string.IsNullOrEmpty(this.DisplayMemberPath))
      return item;
    string[] source = this.DisplayMemberPath.Split('.');
    if (source.Length == 1)
    {
      PropertyInfo property = item.GetType().GetProperty(this.DisplayMemberPath);
      return property != (PropertyInfo) null ? property.GetValue(item, (object[]) null) : item;
    }
    for (int index = 0; index < ((IEnumerable<string>) source).Count<string>(); ++index)
    {
      PropertyInfo property = item.GetType().GetProperty(source[index]);
      if (property == (PropertyInfo) null)
        return item;
      if (index == ((IEnumerable<string>) source).Count<string>() - 1)
        return property.GetValue(item, (object[]) null);
      item = property.GetValue(item, (object[]) null);
    }
    return item;
  }

  private void CloseDropDown(bool isFocusOnComboBox)
  {
    if (this.IsDropDownOpen)
      this.IsDropDownOpen = false;
    this.ReleaseMouseCapture();
    if (!isFocusOnComboBox)
      return;
    this.Focus();
  }
}
