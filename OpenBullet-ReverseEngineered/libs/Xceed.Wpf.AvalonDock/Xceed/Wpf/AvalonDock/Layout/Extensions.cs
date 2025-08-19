// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.Extensions
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

public static class Extensions
{
  public static IEnumerable<ILayoutElement> Descendents(this ILayoutElement element)
  {
    if (element is ILayoutContainer layoutContainer)
    {
      foreach (ILayoutElement childElement in layoutContainer.Children)
      {
        yield return childElement;
        foreach (ILayoutElement descendent in childElement.Descendents())
          yield return descendent;
      }
    }
  }

  public static T FindParent<T>(this ILayoutElement element)
  {
    ILayoutContainer parent = element.Parent;
    while (true)
    {
      switch (parent)
      {
        case null:
        case T _:
          goto label_3;
        default:
          parent = parent.Parent;
          continue;
      }
    }
label_3:
    return (T) parent;
  }

  public static ILayoutRoot GetRoot(this ILayoutElement element)
  {
    if (element is ILayoutRoot)
      return element as ILayoutRoot;
    ILayoutContainer parent = element.Parent;
    while (true)
    {
      switch (parent)
      {
        case null:
        case ILayoutRoot _:
          goto label_5;
        default:
          parent = parent.Parent;
          continue;
      }
    }
label_5:
    return (ILayoutRoot) parent;
  }

  public static bool ContainsChildOfType<T>(this ILayoutContainer element)
  {
    foreach (ILayoutElement descendent in element.Descendents())
    {
      if (descendent is T)
        return true;
    }
    return false;
  }

  public static bool ContainsChildOfType<T, S>(this ILayoutContainer container)
  {
    foreach (ILayoutElement descendent in container.Descendents())
    {
      if (descendent is T || descendent is S)
        return true;
    }
    return false;
  }

  public static bool IsOfType<T, S>(this ILayoutContainer container)
  {
    return container is T || container is S;
  }

  public static AnchorSide GetSide(this ILayoutElement element)
  {
    if (element.Parent is ILayoutOrientableGroup parent)
    {
      if (!(parent is LayoutPanel layoutPanel))
        layoutPanel = parent.FindParent<LayoutPanel>();
      if (layoutPanel != null && layoutPanel.Children.Count > 0)
        return layoutPanel.Orientation == Orientation.Horizontal ? (!layoutPanel.Children[0].Equals((object) element) && !layoutPanel.Children[0].Descendents().Contains((object) element) ? AnchorSide.Right : AnchorSide.Left) : (!layoutPanel.Children[0].Equals((object) element) && !layoutPanel.Children[0].Descendents().Contains((object) element) ? AnchorSide.Bottom : AnchorSide.Top);
    }
    return AnchorSide.Right;
  }

  internal static void KeepInsideNearestMonitor(
    this ILayoutElementForFloatingWindow paneInsideFloatingWindow)
  {
    Win32Helper.RECT lprc = new Win32Helper.RECT()
    {
      Left = (int) paneInsideFloatingWindow.FloatingLeft,
      Top = (int) paneInsideFloatingWindow.FloatingTop
    };
    lprc.Bottom = lprc.Top + (int) paneInsideFloatingWindow.FloatingHeight;
    lprc.Right = lprc.Left + (int) paneInsideFloatingWindow.FloatingWidth;
    uint dwFlags1 = 2;
    uint dwFlags2 = 0;
    if (!(Win32Helper.MonitorFromRect(ref lprc, dwFlags2) == IntPtr.Zero))
      return;
    IntPtr hMonitor = Win32Helper.MonitorFromRect(ref lprc, dwFlags1);
    if (!(hMonitor != IntPtr.Zero))
      return;
    Win32Helper.MonitorInfo monitorInfo = new Win32Helper.MonitorInfo();
    monitorInfo.Size = Marshal.SizeOf((object) monitorInfo);
    Win32Helper.GetMonitorInfo(hMonitor, monitorInfo);
    if (paneInsideFloatingWindow.FloatingLeft < (double) monitorInfo.Work.Left)
      paneInsideFloatingWindow.FloatingLeft = (double) (monitorInfo.Work.Left + 10);
    if (paneInsideFloatingWindow.FloatingLeft + paneInsideFloatingWindow.FloatingWidth > (double) monitorInfo.Work.Right)
      paneInsideFloatingWindow.FloatingLeft = (double) monitorInfo.Work.Right - (paneInsideFloatingWindow.FloatingWidth + 10.0);
    if (paneInsideFloatingWindow.FloatingTop < (double) monitorInfo.Work.Top)
      paneInsideFloatingWindow.FloatingTop = (double) (monitorInfo.Work.Top + 10);
    if (paneInsideFloatingWindow.FloatingTop + paneInsideFloatingWindow.FloatingHeight <= (double) monitorInfo.Work.Bottom)
      return;
    paneInsideFloatingWindow.FloatingTop = (double) monitorInfo.Work.Bottom - (paneInsideFloatingWindow.FloatingHeight + 10.0);
  }
}
