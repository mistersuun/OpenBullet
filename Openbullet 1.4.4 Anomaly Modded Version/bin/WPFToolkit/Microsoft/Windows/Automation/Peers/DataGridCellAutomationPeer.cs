// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridCellAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls;
using MS.Internal;
using System;
using System.Windows;
using System.Windows.Automation.Peers;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridCellAutomationPeer : FrameworkElementAutomationPeer
{
  public DataGridCellAutomationPeer(DataGridCell owner)
    : base((FrameworkElement) owner)
  {
    if (owner == null)
      throw new ArgumentNullException(nameof (owner));
    this.UpdateEventSource();
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Custom;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  protected override bool IsOffscreenCore()
  {
    if (!this.Owner.IsVisible)
      return true;
    Rect visibleBoundingRect = DataGridAutomationPeer.CalculateVisibleBoundingRect(this.Owner);
    return DoubleUtil.AreClose(visibleBoundingRect, Rect.Empty) || DoubleUtil.AreClose(visibleBoundingRect.Height, 0.0) || DoubleUtil.AreClose(visibleBoundingRect.Width, 0.0);
  }

  private void UpdateEventSource()
  {
    DataGridCell owner = (DataGridCell) this.Owner;
    DataGrid dataGridOwner = owner.DataGridOwner;
    if (dataGridOwner == null || !(UIElementAutomationPeer.CreatePeerForElement((UIElement) dataGridOwner) is DataGridAutomationPeer peerForElement))
      return;
    DataGridItemAutomationPeer itemPeer = peerForElement.GetOrCreateItemPeer(owner.DataContext);
    if (itemPeer == null)
      return;
    this.EventsSource = (AutomationPeer) itemPeer.GetOrCreateCellItemPeer(owner.Column);
  }
}
