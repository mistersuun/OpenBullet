// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.WindowHookHandler
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class WindowHookHandler
{
  private IntPtr _windowHook;
  private Win32Helper.HookProc _hookProc;
  private ReentrantFlag _insideActivateEvent = new ReentrantFlag();

  public void Attach()
  {
    this._hookProc = new Win32Helper.HookProc(this.HookProc);
    this._windowHook = Win32Helper.SetWindowsHookEx(Win32Helper.HookType.WH_CBT, this._hookProc, IntPtr.Zero, (int) Win32Helper.GetCurrentThreadId());
  }

  public void Detach() => Win32Helper.UnhookWindowsHookEx(this._windowHook);

  public int HookProc(int code, IntPtr wParam, IntPtr lParam)
  {
    switch (code)
    {
      case 5:
        if (this._insideActivateEvent.CanEnter)
        {
          using (this._insideActivateEvent.Enter())
            break;
        }
        break;
      case 9:
        if (this.FocusChanged != null)
        {
          this.FocusChanged((object) this, new FocusChangeEventArgs(wParam, lParam));
          break;
        }
        break;
    }
    return Win32Helper.CallNextHookEx(this._windowHook, code, wParam, lParam);
  }

  public event EventHandler<FocusChangeEventArgs> FocusChanged;
}
