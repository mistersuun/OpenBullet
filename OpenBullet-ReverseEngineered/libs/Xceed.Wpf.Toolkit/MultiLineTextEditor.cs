// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.MultiLineTextEditor
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_ResizeThumb", Type = typeof (Thumb))]
[TemplatePart(Name = "PART_TextBox", Type = typeof (TextBox))]
[TemplatePart(Name = "PART_DropDownButton", Type = typeof (ToggleButton))]
public class MultiLineTextEditor : ContentControl
{
  private const string PART_ResizeThumb = "PART_ResizeThumb";
  private const string PART_TextBox = "PART_TextBox";
  private const string PART_DropDownButton = "PART_DropDownButton";
  private Thumb _resizeThumb;
  private TextBox _textBox;
  private ToggleButton _toggleButton;
  public static readonly DependencyProperty DropDownHeightProperty = DependencyProperty.Register(nameof (DropDownHeight), typeof (double), typeof (MultiLineTextEditor), (PropertyMetadata) new UIPropertyMetadata((object) 150.0));
  public static readonly DependencyProperty DropDownWidthProperty = DependencyProperty.Register(nameof (DropDownWidth), typeof (double), typeof (MultiLineTextEditor), (PropertyMetadata) new UIPropertyMetadata((object) 200.0));
  public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (MultiLineTextEditor), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(MultiLineTextEditor.OnIsOpenChanged)));
  public static readonly DependencyProperty IsSpellCheckEnabledProperty = DependencyProperty.Register(nameof (IsSpellCheckEnabled), typeof (bool), typeof (MultiLineTextEditor), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (MultiLineTextEditor), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (MultiLineTextEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(MultiLineTextEditor.OnTextChanged)));
  public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(nameof (TextAlignment), typeof (TextAlignment), typeof (MultiLineTextEditor), (PropertyMetadata) new UIPropertyMetadata((object) TextAlignment.Left));
  public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(nameof (TextWrapping), typeof (TextWrapping), typeof (MultiLineTextEditor), (PropertyMetadata) new UIPropertyMetadata((object) TextWrapping.NoWrap));

  public double DropDownHeight
  {
    get => (double) this.GetValue(MultiLineTextEditor.DropDownHeightProperty);
    set => this.SetValue(MultiLineTextEditor.DropDownHeightProperty, (object) value);
  }

  public double DropDownWidth
  {
    get => (double) this.GetValue(MultiLineTextEditor.DropDownWidthProperty);
    set => this.SetValue(MultiLineTextEditor.DropDownWidthProperty, (object) value);
  }

  public bool IsOpen
  {
    get => (bool) this.GetValue(MultiLineTextEditor.IsOpenProperty);
    set => this.SetValue(MultiLineTextEditor.IsOpenProperty, (object) value);
  }

  private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MultiLineTextEditor multiLineTextEditor))
      return;
    multiLineTextEditor.OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
  {
    if (this._textBox == null)
      return;
    this.Dispatcher.BeginInvoke((Delegate) (() => this._textBox.Focus()), DispatcherPriority.Background);
  }

  public bool IsSpellCheckEnabled
  {
    get => (bool) this.GetValue(MultiLineTextEditor.IsSpellCheckEnabledProperty);
    set => this.SetValue(MultiLineTextEditor.IsSpellCheckEnabledProperty, (object) value);
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(MultiLineTextEditor.IsReadOnlyProperty);
    set => this.SetValue(MultiLineTextEditor.IsReadOnlyProperty, (object) value);
  }

  public string Text
  {
    get => (string) this.GetValue(MultiLineTextEditor.TextProperty);
    set => this.SetValue(MultiLineTextEditor.TextProperty, (object) value);
  }

  private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is MultiLineTextEditor multiLineTextEditor))
      return;
    multiLineTextEditor.OnTextChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnTextChanged(string oldValue, string newValue)
  {
  }

  public TextAlignment TextAlignment
  {
    get => (TextAlignment) this.GetValue(MultiLineTextEditor.TextAlignmentProperty);
    set => this.SetValue(MultiLineTextEditor.TextAlignmentProperty, (object) value);
  }

  public TextWrapping TextWrapping
  {
    get => (TextWrapping) this.GetValue(MultiLineTextEditor.TextWrappingProperty);
    set => this.SetValue(MultiLineTextEditor.TextWrappingProperty, (object) value);
  }

  static MultiLineTextEditor()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (MultiLineTextEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (MultiLineTextEditor)));
  }

  public MultiLineTextEditor()
  {
    Keyboard.AddKeyDownHandler((DependencyObject) this, new KeyEventHandler(this.OnKeyDown));
    Mouse.AddPreviewMouseDownOutsideCapturedElementHandler((DependencyObject) this, new MouseButtonEventHandler(this.OnMouseDownOutsideCapturedElement));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._resizeThumb != null)
      this._resizeThumb.DragDelta -= new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
    this._resizeThumb = this.GetTemplateChild("PART_ResizeThumb") as Thumb;
    if (this._resizeThumb != null)
      this._resizeThumb.DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
    this._textBox = this.GetTemplateChild("PART_TextBox") as TextBox;
    this._toggleButton = this.GetTemplateChild("PART_DropDownButton") as ToggleButton;
  }

  private void OnKeyDown(object sender, KeyEventArgs e)
  {
    if (!this.IsOpen)
    {
      if (!KeyboardUtilities.IsKeyModifyingPopupState(e))
        return;
      this.IsOpen = true;
      e.Handled = true;
    }
    else
    {
      if (!KeyboardUtilities.IsKeyModifyingPopupState(e) && e.Key != Key.Escape && e.Key != Key.Tab)
        return;
      this.CloseEditor();
      e.Handled = true;
    }
  }

  private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
  {
    this.CloseEditor();
  }

  private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
  {
    double num1 = this.DropDownHeight + e.VerticalChange;
    double num2 = this.DropDownWidth + e.HorizontalChange;
    if (num2 < 0.0 || num1 < 0.0)
      return;
    this.DropDownWidth = num2;
    this.DropDownHeight = num1;
  }

  private void CloseEditor()
  {
    if (this.IsOpen)
      this.IsOpen = false;
    this.ReleaseMouseCapture();
    if (this._toggleButton == null)
      return;
    this._toggleButton.Focus();
  }
}
