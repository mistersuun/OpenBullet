// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.DateTimePickerBase
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

[TemplatePart(Name = "PART_Popup", Type = typeof (Popup))]
public class DateTimePickerBase : DateTimeUpDown
{
  private const string PART_Popup = "PART_Popup";
  private Popup _popup;
  private DateTime? _initialValue;
  public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (DateTimePickerBase), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(DateTimePickerBase.OnIsOpenChanged)));
  public static readonly DependencyProperty ShowDropDownButtonProperty = DependencyProperty.Register(nameof (ShowDropDownButton), typeof (bool), typeof (DateTimePickerBase), (PropertyMetadata) new UIPropertyMetadata((object) true));

  public bool IsOpen
  {
    get => (bool) this.GetValue(DateTimePickerBase.IsOpenProperty);
    set => this.SetValue(DateTimePickerBase.IsOpenProperty, (object) value);
  }

  private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((DateTimePickerBase) d)?.OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
  {
    if (!newValue)
      return;
    this._initialValue = this.Value;
  }

  public bool ShowDropDownButton
  {
    get => (bool) this.GetValue(DateTimePickerBase.ShowDropDownButtonProperty);
    set => this.SetValue(DateTimePickerBase.ShowDropDownButtonProperty, (object) value);
  }

  public DateTimePickerBase()
  {
    this.AddHandler(UIElement.KeyDownEvent, (Delegate) new KeyEventHandler(this.HandleKeyDown), true);
    Mouse.AddPreviewMouseDownOutsideCapturedElementHandler((DependencyObject) this, new MouseButtonEventHandler(this.OnMouseDownOutsideCapturedElement));
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

  protected virtual void HandleKeyDown(object sender, KeyEventArgs e)
  {
    if (!this.IsOpen)
    {
      if (!KeyboardUtilities.IsKeyModifyingPopupState(e))
        return;
      this.IsOpen = true;
      e.Handled = true;
    }
    else if (KeyboardUtilities.IsKeyModifyingPopupState(e))
    {
      this.ClosePopup(true);
      e.Handled = true;
    }
    else if (e.Key == Key.Return)
    {
      this.ClosePopup(true);
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Escape)
        return;
      if (!object.Equals((object) this.Value, (object) this._initialValue))
        this.Value = this._initialValue;
      this.ClosePopup(true);
      e.Handled = true;
    }
  }

  private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
  {
    this.ClosePopup(true);
  }

  protected virtual void Popup_Opened(object sender, EventArgs e)
  {
  }

  protected void ClosePopup(bool isFocusOnTextBox)
  {
    if (this.IsOpen)
      this.IsOpen = false;
    this.ReleaseMouseCapture();
    if (!isFocusOnTextBox || this.TextBox == null)
      return;
    this.TextBox.Focus();
  }
}
