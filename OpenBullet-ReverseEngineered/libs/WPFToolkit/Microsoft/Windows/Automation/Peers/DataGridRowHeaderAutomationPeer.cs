// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridRowHeaderAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridRowHeaderAutomationPeer(Microsoft.Windows.Controls.Primitives.DataGridRowHeader owner) : 
  ButtonBaseAutomationPeer((ButtonBase) owner)
{
  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.HeaderItem;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  protected override bool IsContentElementCore() => false;

  protected override bool IsOffscreenCore()
  {
    if (!this.Owner.IsVisible)
      return true;
    Rect visibleBoundingRect = DataGridAutomationPeer.CalculateVisibleBoundingRect(this.Owner);
    return DoubleUtil.AreClose(visibleBoundingRect, Rect.Empty) || DoubleUtil.AreClose(visibleBoundingRect.Height, 0.0) || DoubleUtil.AreClose(visibleBoundingRect.Width, 0.0);
  }
}
