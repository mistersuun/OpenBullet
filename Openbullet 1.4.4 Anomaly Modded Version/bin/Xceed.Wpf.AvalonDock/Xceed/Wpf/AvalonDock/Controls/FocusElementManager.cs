// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.FocusElementManager
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal static class FocusElementManager
{
  private static List<DockingManager> _managers = new List<DockingManager>();
  private static FullWeakDictionary<ILayoutElement, IInputElement> _modelFocusedElement = new FullWeakDictionary<ILayoutElement, IInputElement>();
  private static WeakDictionary<ILayoutElement, IntPtr> _modelFocusedWindowHandle = new WeakDictionary<ILayoutElement, IntPtr>();
  private static WeakReference _lastFocusedElement;
  private static WindowHookHandler _windowHandler = (WindowHookHandler) null;
  private static DispatcherOperation _setFocusAsyncOperation;
  private static WeakReference _lastFocusedElementBeforeEnterMenuMode = (WeakReference) null;

  internal static void SetupFocusManagement(DockingManager manager)
  {
    if (FocusElementManager._managers.Count == 0)
    {
      FocusElementManager._windowHandler = new WindowHookHandler();
      FocusElementManager._windowHandler.FocusChanged += new EventHandler<FocusChangeEventArgs>(FocusElementManager.WindowFocusChanging);
      FocusElementManager._windowHandler.Attach();
      if (Application.Current != null)
        Application.Current.Exit += new ExitEventHandler(FocusElementManager.Current_Exit);
    }
    manager.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(FocusElementManager.manager_PreviewGotKeyboardFocus);
    FocusElementManager._managers.Add(manager);
  }

  internal static void FinalizeFocusManagement(DockingManager manager)
  {
    manager.PreviewGotKeyboardFocus -= new KeyboardFocusChangedEventHandler(FocusElementManager.manager_PreviewGotKeyboardFocus);
    FocusElementManager._managers.Remove(manager);
    if (FocusElementManager._managers.Count != 0 || FocusElementManager._windowHandler == null)
      return;
    FocusElementManager._windowHandler.FocusChanged -= new EventHandler<FocusChangeEventArgs>(FocusElementManager.WindowFocusChanging);
    FocusElementManager._windowHandler.Detach();
    FocusElementManager._windowHandler = (WindowHookHandler) null;
  }

  internal static IInputElement GetLastFocusedElement(ILayoutElement model)
  {
    IInputElement inputElement;
    return FocusElementManager._modelFocusedElement.GetValue(model, out inputElement) ? inputElement : (IInputElement) null;
  }

  internal static IntPtr GetLastWindowHandle(ILayoutElement model)
  {
    IntPtr num;
    return FocusElementManager._modelFocusedWindowHandle.GetValue(model, out num) ? num : IntPtr.Zero;
  }

  internal static void SetFocusOnLastElement(ILayoutElement model)
  {
    bool flag = false;
    IInputElement element;
    if (FocusElementManager._modelFocusedElement.GetValue(model, out element))
      flag = element == Keyboard.Focus(element);
    IntPtr hWnd;
    if (FocusElementManager._modelFocusedWindowHandle.GetValue(model, out hWnd))
      flag = IntPtr.Zero != Win32Helper.SetFocus(hWnd);
    if (!flag)
      return;
    FocusElementManager._lastFocusedElement = new WeakReference((object) model);
  }

  private static void Current_Exit(object sender, ExitEventArgs e)
  {
    Application.Current.Exit -= new ExitEventHandler(FocusElementManager.Current_Exit);
    if (FocusElementManager._windowHandler == null)
      return;
    FocusElementManager._windowHandler.FocusChanged -= new EventHandler<FocusChangeEventArgs>(FocusElementManager.WindowFocusChanging);
    FocusElementManager._windowHandler.Detach();
    FocusElementManager._windowHandler = (WindowHookHandler) null;
  }

  private static void manager_PreviewGotKeyboardFocus(
    object sender,
    KeyboardFocusChangedEventArgs e)
  {
    if (!(e.NewFocus is Visual newFocus))
      return;
    switch (newFocus)
    {
      case LayoutAnchorableTabItem _:
        break;
      case LayoutDocumentTabItem _:
        break;
      default:
        LayoutAnchorableControl visualAncestor1 = newFocus.FindVisualAncestor<LayoutAnchorableControl>();
        if (visualAncestor1 != null)
        {
          FocusElementManager._modelFocusedElement[(ILayoutElement) visualAncestor1.Model] = e.NewFocus;
          break;
        }
        LayoutDocumentControl visualAncestor2 = newFocus.FindVisualAncestor<LayoutDocumentControl>();
        if (visualAncestor2 == null)
          break;
        FocusElementManager._modelFocusedElement[(ILayoutElement) visualAncestor2.Model] = e.NewFocus;
        break;
    }
  }

  private static void WindowFocusChanging(object sender, FocusChangeEventArgs e)
  {
    foreach (DependencyObject manager in FocusElementManager._managers)
    {
      HwndHost dependencyObject = manager.FindLogicalChildren<HwndHost>().FirstOrDefault<HwndHost>((Func<HwndHost, bool>) (hw => Win32Helper.IsChild(hw.Handle, e.GotFocusWinHandle)));
      if (dependencyObject != null)
      {
        LayoutAnchorableControl visualAncestor1 = dependencyObject.FindVisualAncestor<LayoutAnchorableControl>();
        if (visualAncestor1 != null)
        {
          FocusElementManager._modelFocusedWindowHandle[(ILayoutElement) visualAncestor1.Model] = e.GotFocusWinHandle;
          if (visualAncestor1.Model != null)
            visualAncestor1.Model.IsActive = true;
        }
        else
        {
          LayoutDocumentControl visualAncestor2 = dependencyObject.FindVisualAncestor<LayoutDocumentControl>();
          if (visualAncestor2 != null)
          {
            FocusElementManager._modelFocusedWindowHandle[(ILayoutElement) visualAncestor2.Model] = e.GotFocusWinHandle;
            if (visualAncestor2.Model != null)
              visualAncestor2.Model.IsActive = true;
          }
        }
      }
    }
  }

  private static void WindowActivating(object sender, WindowActivateEventArgs e)
  {
    if (Keyboard.FocusedElement != null || FocusElementManager._lastFocusedElement == null || !FocusElementManager._lastFocusedElement.IsAlive)
      return;
    ILayoutElement elementToSetFocus = FocusElementManager._lastFocusedElement.Target as ILayoutElement;
    if (elementToSetFocus == null)
      return;
    DockingManager manager = elementToSetFocus.Root.Manager;
    IntPtr hwnd;
    if (manager == null || !manager.GetParentWindowHandle(out hwnd) || e.HwndActivating != hwnd)
      return;
    FocusElementManager._setFocusAsyncOperation = Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) (() =>
    {
      try
      {
        FocusElementManager.SetFocusOnLastElement(elementToSetFocus);
      }
      finally
      {
        FocusElementManager._setFocusAsyncOperation = (DispatcherOperation) null;
      }
    }), DispatcherPriority.Input);
  }

  private static void InputManager_EnterMenuMode(object sender, EventArgs e)
  {
    if (Keyboard.FocusedElement == null)
      return;
    if ((Keyboard.FocusedElement as DependencyObject).FindLogicalAncestor<DockingManager>() == null)
      FocusElementManager._lastFocusedElementBeforeEnterMenuMode = (WeakReference) null;
    else
      FocusElementManager._lastFocusedElementBeforeEnterMenuMode = new WeakReference((object) Keyboard.FocusedElement);
  }

  private static void InputManager_LeaveMenuMode(object sender, EventArgs e)
  {
    if (FocusElementManager._lastFocusedElementBeforeEnterMenuMode == null || !FocusElementManager._lastFocusedElementBeforeEnterMenuMode.IsAlive)
      return;
    UIElement valueOrDefault = FocusElementManager._lastFocusedElementBeforeEnterMenuMode.GetValueOrDefault<UIElement>();
    if (valueOrDefault == null)
      return;
    Keyboard.Focus((IInputElement) valueOrDefault);
  }
}
