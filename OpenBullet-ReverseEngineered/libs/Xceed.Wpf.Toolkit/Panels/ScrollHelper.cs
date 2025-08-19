// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.ScrollHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

internal static class ScrollHelper
{
  public static bool ScrollLeastAmount(Rect physViewRect, Rect itemRect, out Vector newPhysOffset)
  {
    bool flag = false;
    newPhysOffset = new Vector();
    if (!physViewRect.Contains(itemRect))
    {
      newPhysOffset.X = itemRect.Left > physViewRect.Left && itemRect.Right < physViewRect.Right || DoubleHelper.AreVirtuallyEqual(itemRect.Left, physViewRect.Left) ? itemRect.Left : (itemRect.Left < physViewRect.Left || itemRect.Width > physViewRect.Width ? itemRect.Left : Math.Max(0.0, physViewRect.Left + (itemRect.Right - physViewRect.Right)));
      newPhysOffset.Y = itemRect.Top > physViewRect.Top && itemRect.Bottom < physViewRect.Bottom || DoubleHelper.AreVirtuallyEqual(itemRect.Top, physViewRect.Top) ? itemRect.Top : (itemRect.Top < physViewRect.Top || itemRect.Height > physViewRect.Height ? itemRect.Top : Math.Max(0.0, physViewRect.Top + (itemRect.Bottom - physViewRect.Bottom)));
      flag = true;
    }
    return flag;
  }
}
