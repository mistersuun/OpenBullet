// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.RichTextBoxFormatBarManager
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class RichTextBoxFormatBarManager : DependencyObject
{
  private System.Windows.Controls.RichTextBox _richTextBox;
  private UIElementAdorner<Control> _adorner;
  private IRichTextBoxFormatBar _toolbar;
  private Window _parentWindow;
  private const double _hideAdornerDistance = 150.0;
  public static readonly DependencyProperty FormatBarProperty = DependencyProperty.RegisterAttached("FormatBar", typeof (IRichTextBoxFormatBar), typeof (RichTextBox), new PropertyMetadata((object) null, new PropertyChangedCallback(RichTextBoxFormatBarManager.OnFormatBarPropertyChanged)));

  public static void SetFormatBar(UIElement element, IRichTextBoxFormatBar value)
  {
    element.SetValue(RichTextBoxFormatBarManager.FormatBarProperty, (object) value);
  }

  public static IRichTextBoxFormatBar GetFormatBar(UIElement element)
  {
    return (IRichTextBoxFormatBar) element.GetValue(RichTextBoxFormatBarManager.FormatBarProperty);
  }

  private static void OnFormatBarPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is System.Windows.Controls.RichTextBox richTextBox))
      throw new Exception("A FormatBar can only be applied to a RichTextBox.");
    new RichTextBoxFormatBarManager().AttachFormatBarToRichtextBox(richTextBox, e.NewValue as IRichTextBoxFormatBar);
  }

  public bool IsAdornerVisible => this._adorner.Visibility == Visibility.Visible;

  private void RichTextBox_MouseButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
    {
      if (this._richTextBox.IsReadOnly)
        return;
      TextRange textRange = new TextRange(this._richTextBox.Selection.Start, this._richTextBox.Selection.End);
      if (textRange.Text.Length > 0 && !string.IsNullOrWhiteSpace(textRange.Text))
        this.ShowAdorner();
      else
        this.HideAdorner();
      e.Handled = true;
    }
    else
      this.HideAdorner();
  }

  private void OnPreviewMouseMoveParentWindow(object sender, MouseEventArgs e)
  {
    Point position = e.GetPosition((IInputElement) this._adorner);
    double val1 = 0.0;
    if ((this._adorner.Child == null || !(this._adorner.Child is IRichTextBoxFormatBar) ? 0 : (((IRichTextBoxFormatBar) this._adorner.Child).PreventDisplayFadeOut ? 1 : 0)) != 0 || position.X >= 0.0 && position.X <= this._adorner.ActualWidth && position.Y >= 0.0 && position.Y <= this._adorner.ActualHeight)
      return;
    if (position.X < -150.0 || position.X > this._adorner.ActualWidth + 150.0 || position.Y < -150.0 || position.Y > this._adorner.ActualHeight + 150.0)
    {
      this.HideAdorner();
    }
    else
    {
      if (position.X < 0.0)
        val1 = -position.X;
      else if (position.X > this._adorner.ActualWidth)
        val1 = position.X - this._adorner.ActualWidth;
      if (position.Y < 0.0)
        val1 = Math.Max(val1, -position.Y);
      else if (position.Y > this._adorner.ActualHeight)
        val1 = Math.Max(val1, position.Y - this._adorner.ActualHeight);
      this._adorner.Opacity = 1.0 - Math.Min(val1, 100.0) / 100.0;
    }
  }

  private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
  {
    if (this._richTextBox.IsFocused || this._richTextBox.Selection.IsEmpty)
      return;
    this._richTextBox.Focus();
  }

  private void AttachFormatBarToRichtextBox(
    System.Windows.Controls.RichTextBox richTextBox,
    IRichTextBoxFormatBar formatBar)
  {
    this._richTextBox = richTextBox;
    this._richTextBox.AddHandler(Mouse.MouseUpEvent, (Delegate) new MouseButtonEventHandler(this.RichTextBox_MouseButtonUp), true);
    this._richTextBox.TextChanged += new TextChangedEventHandler(this.RichTextBox_TextChanged);
    this._adorner = new UIElementAdorner<Control>((UIElement) this._richTextBox);
    formatBar.Target = this._richTextBox;
    this._toolbar = formatBar;
  }

  private void ShowAdorner()
  {
    if (this._adorner.Visibility == Visibility.Visible)
      return;
    this.VerifyAdornerLayer();
    Control toolbar = this._toolbar as Control;
    if (this._adorner.Child == null)
      this._adorner.Child = toolbar;
    toolbar.ApplyTemplate();
    this._toolbar.Update();
    this._adorner.Visibility = Visibility.Visible;
    this.PositionFormatBar(toolbar);
    this._parentWindow = TreeHelper.FindParent<Window>((DependencyObject) this._adorner);
    if (this._parentWindow == null)
      return;
    Mouse.AddMouseMoveHandler((DependencyObject) this._parentWindow, new MouseEventHandler(this.OnPreviewMouseMoveParentWindow));
  }

  private void PositionFormatBar(Control adorningEditor)
  {
    Point position = Mouse.GetPosition((IInputElement) this._richTextBox);
    double x = position.X;
    double top = position.Y - 15.0 - adorningEditor.ActualHeight;
    if (top < 0.0)
      top = position.Y + 10.0;
    if (x + adorningEditor.ActualWidth > this._richTextBox.ActualWidth - 20.0)
      x -= adorningEditor.ActualWidth - (this._richTextBox.ActualWidth - x);
    this._adorner.SetOffsets(x, top);
  }

  private bool VerifyAdornerLayer()
  {
    if (this._adorner.Parent != null)
      return true;
    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual) this._richTextBox);
    if (adornerLayer == null)
      return false;
    adornerLayer.Add((Adorner) this._adorner);
    return true;
  }

  private void HideAdorner()
  {
    if (!this.IsAdornerVisible)
      return;
    this._adorner.Visibility = Visibility.Collapsed;
    if (this._parentWindow == null)
      return;
    Mouse.RemoveMouseMoveHandler((DependencyObject) this._parentWindow, new MouseEventHandler(this.OnPreviewMouseMoveParentWindow));
  }
}
