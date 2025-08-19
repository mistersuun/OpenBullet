// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.ImeSupport
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal class ImeSupport
{
  private readonly TextArea textArea;
  private IntPtr currentContext;
  private IntPtr previousContext;
  private IntPtr defaultImeWnd;
  private HwndSource hwndSource;
  private EventHandler requerySuggestedHandler;
  private bool isReadOnly;

  public ImeSupport(TextArea textArea)
  {
    this.textArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
    InputMethod.SetIsInputMethodSuspended((DependencyObject) this.textArea, textArea.Options.EnableImeSupport);
    this.requerySuggestedHandler = new EventHandler(this.OnRequerySuggested);
    CommandManager.RequerySuggested += this.requerySuggestedHandler;
    textArea.OptionChanged += new PropertyChangedEventHandler(this.TextAreaOptionChanged);
  }

  private void OnRequerySuggested(object sender, EventArgs e) => this.UpdateImeEnabled();

  private void TextAreaOptionChanged(object sender, PropertyChangedEventArgs e)
  {
    if (!(e.PropertyName == "EnableImeSupport"))
      return;
    InputMethod.SetIsInputMethodSuspended((DependencyObject) this.textArea, this.textArea.Options.EnableImeSupport);
    this.UpdateImeEnabled();
  }

  public void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) => this.UpdateImeEnabled();

  public void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    if (e.OldFocus == this.textArea && this.currentContext != IntPtr.Zero)
      ImeNativeWrapper.NotifyIme(this.currentContext);
    this.ClearContext();
  }

  private void UpdateImeEnabled()
  {
    if (this.textArea.Options.EnableImeSupport && this.textArea.IsKeyboardFocused)
    {
      bool flag = !this.textArea.ReadOnlySectionProvider.CanInsert(this.textArea.Caret.Offset);
      if (this.hwndSource != null && this.isReadOnly == flag)
        return;
      this.ClearContext();
      this.isReadOnly = flag;
      this.CreateContext();
    }
    else
      this.ClearContext();
  }

  private void ClearContext()
  {
    if (this.hwndSource == null)
      return;
    ImeNativeWrapper.ImmAssociateContext(this.hwndSource.Handle, this.previousContext);
    ImeNativeWrapper.ImmReleaseContext(this.defaultImeWnd, this.currentContext);
    this.currentContext = IntPtr.Zero;
    this.defaultImeWnd = IntPtr.Zero;
    this.hwndSource.RemoveHook(new HwndSourceHook(this.WndProc));
    this.hwndSource = (HwndSource) null;
  }

  private void CreateContext()
  {
    this.hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) this.textArea);
    if (this.hwndSource == null)
      return;
    if (this.isReadOnly)
    {
      this.defaultImeWnd = IntPtr.Zero;
      this.currentContext = IntPtr.Zero;
    }
    else
    {
      this.defaultImeWnd = ImeNativeWrapper.ImmGetDefaultIMEWnd(IntPtr.Zero);
      this.currentContext = ImeNativeWrapper.ImmGetContext(this.defaultImeWnd);
    }
    this.previousContext = ImeNativeWrapper.ImmAssociateContext(this.hwndSource.Handle, this.currentContext);
    this.hwndSource.AddHook(new HwndSourceHook(this.WndProc));
    ImeNativeWrapper.GetTextFrameworkThreadManager()?.SetFocus(IntPtr.Zero);
  }

  private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
  {
    switch (msg)
    {
      case 81:
        if (this.hwndSource != null)
        {
          this.ClearContext();
          this.CreateContext();
          break;
        }
        break;
      case 271:
        this.UpdateCompositionWindow();
        break;
    }
    return IntPtr.Zero;
  }

  public void UpdateCompositionWindow()
  {
    if (!(this.currentContext != IntPtr.Zero))
      return;
    ImeNativeWrapper.SetCompositionFont(this.hwndSource, this.currentContext, this.textArea);
    ImeNativeWrapper.SetCompositionWindow(this.hwndSource, this.currentContext, this.textArea);
  }
}
