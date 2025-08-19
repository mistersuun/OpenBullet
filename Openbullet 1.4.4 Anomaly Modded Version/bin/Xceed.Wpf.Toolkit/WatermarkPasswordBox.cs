// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.WatermarkPasswordBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class WatermarkPasswordBox : WatermarkTextBox
{
  private int _newCaretIndex = -1;
  public static readonly DependencyProperty PasswordCharProperty = DependencyProperty.Register(nameof (PasswordChar), typeof (char), typeof (WatermarkPasswordBox), (PropertyMetadata) new UIPropertyMetadata((object) '●', new PropertyChangedCallback(WatermarkPasswordBox.OnPasswordCharChanged)));
  public static readonly RoutedEvent PasswordChangedEvent = EventManager.RegisterRoutedEvent("PasswordChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (WatermarkPasswordBox));

  public string Password
  {
    [SecuritySafeCritical] get
    {
      IntPtr bstr = Marshal.SecureStringToBSTR(this.SecurePassword);
      try
      {
        return Marshal.PtrToStringUni(bstr);
      }
      finally
      {
        Marshal.ZeroFreeBSTR(bstr);
      }
    }
    set
    {
      if (value == null)
        value = string.Empty;
      this.SecurePassword = new SecureString();
      for (int index = 0; index < value.Length; ++index)
        this.SecurePassword.AppendChar(value[index]);
      this.SyncTextPassword(this._newCaretIndex);
      this.RaiseEvent(new RoutedEventArgs(WatermarkPasswordBox.PasswordChangedEvent, (object) this));
    }
  }

  public char PasswordChar
  {
    get => (char) this.GetValue(WatermarkPasswordBox.PasswordCharProperty);
    set => this.SetValue(WatermarkPasswordBox.PasswordCharProperty, (object) value);
  }

  private static void OnPasswordCharChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is WatermarkPasswordBox watermarkPasswordBox))
      return;
    watermarkPasswordBox.OnPasswordCharChanged((char) e.OldValue, (char) e.NewValue);
  }

  protected virtual void OnPasswordCharChanged(char oldValue, char newValue)
  {
    this.SyncTextPassword(this.CaretIndex);
  }

  public SecureString SecurePassword { get; private set; }

  public WatermarkPasswordBox()
  {
    this.Password = string.Empty;
    this.IsUndoEnabled = false;
    this.UndoLimit = 0;
    CommandManager.AddPreviewCanExecuteHandler((UIElement) this, new CanExecuteRoutedEventHandler(this.OnPreviewCanExecuteCommand));
    DataObject.AddPastingHandler((DependencyObject) this, new DataObjectPastingEventHandler(this.OnPaste));
  }

  [SecuritySafeCritical]
  protected override void OnPreviewTextInput(TextCompositionEventArgs e)
  {
    this.PasswordInsert(e.Text, this.CaretIndex);
    e.Handled = true;
    base.OnPreviewTextInput(e);
  }

  [SecuritySafeCritical]
  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    switch (e.Key)
    {
      case Key.Back:
        this.PasswordRemove(this.SelectedText.Length > 0 ? this.CaretIndex : this.CaretIndex - 1);
        e.Handled = true;
        break;
      case Key.Space:
        this.PasswordInsert(" ", this.CaretIndex);
        e.Handled = true;
        break;
      case Key.Delete:
        this.PasswordRemove(this.CaretIndex);
        e.Handled = true;
        break;
      case Key.V:
        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Clipboard.ContainsText())
        {
          this.PasswordInsert(Clipboard.GetText(), this.CaretIndex);
          e.Handled = true;
          break;
        }
        break;
    }
    base.OnPreviewKeyDown(e);
  }

  protected override void OnTextChanged(TextChangedEventArgs e)
  {
    base.OnTextChanged(e);
    if (this.Text.Length == this.Password.Length)
      return;
    if (this.Text == "")
      this.SetPassword("", 0);
    else
      this.SyncTextPassword(this.Password.Length);
  }

  public event RoutedEventHandler PasswordChanged
  {
    add => this.AddHandler(WatermarkPasswordBox.PasswordChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(WatermarkPasswordBox.PasswordChangedEvent, (Delegate) value);
  }

  [SecuritySafeCritical]
  private void OnPaste(object sender, DataObjectPastingEventArgs e)
  {
    if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
      return;
    if (e.SourceDataObject.GetData(DataFormats.UnicodeText) is string data)
      this.PasswordInsert(data, this.CaretIndex);
    e.CancelCommand();
  }

  private void OnPreviewCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
  {
    if (e.Command != ApplicationCommands.Copy && e.Command != ApplicationCommands.Cut && e.Command != ApplicationCommands.Undo)
      return;
    e.CanExecute = false;
    e.Handled = true;
  }

  [SecurityCritical]
  private void PasswordInsert(string text, int index)
  {
    if (text == null || index < 0 || index > this.Password.Length)
      return;
    if (this.SelectedText.Length > 0)
      this.PasswordRemove(index);
    string password = this.Password;
    for (int index1 = 0; index1 < text.Length; ++index1)
    {
      if (this.MaxLength == 0 || password.Length < this.MaxLength)
        password = password.Insert(index++, text[index1].ToString());
    }
    this.SetPassword(password, index);
  }

  [SecurityCritical]
  private void PasswordRemove(int index)
  {
    if (index < 0 || index >= this.Password.Length)
      return;
    if (this.SelectedText.Length > 0)
    {
      string password = this.Password;
      for (int index1 = 0; index1 < this.SelectedText.Length; ++index1)
        password = password.Remove(index, 1);
      this.SetPassword(password, index);
    }
    else
      this.SetPassword(this.Password.Remove(index, 1), index);
  }

  private void SetPassword(string password, int caretIndex)
  {
    this._newCaretIndex = caretIndex;
    this.Password = password;
    this._newCaretIndex = -1;
  }

  private void SyncTextPassword(int nextCarretIndex)
  {
    this.Text = new StringBuilder().Append(Enumerable.Repeat<char>(this.PasswordChar, this.Password.Length).ToArray<char>()).ToString();
    this.CaretIndex = Math.Max(nextCarretIndex, 0);
  }
}
