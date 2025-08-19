// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridRowAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using MS.Internal;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridRowAutomationPeer : FrameworkElementAutomationPeer
{
  public DataGridRowAutomationPeer(DataGridRow owner)
    : base((FrameworkElement) owner)
  {
    if (owner == null)
      throw new ArgumentNullException(nameof (owner));
    this.UpdateEventSource();
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.DataItem;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  protected override List<AutomationPeer> GetChildrenCore()
  {
    List<AutomationPeer> childrenCore = new List<AutomationPeer>(3);
    AutomationPeer headerAutomationPeer = this.RowHeaderAutomationPeer;
    if (headerAutomationPeer != null)
      childrenCore.Add(headerAutomationPeer);
    if (this.EventsSource is DataGridItemAutomationPeer eventsSource)
      childrenCore.AddRange((IEnumerable<AutomationPeer>) eventsSource.GetCellItemPeers());
    AutomationPeer presenterAutomationPeer = this.DetailsPresenterAutomationPeer;
    if (presenterAutomationPeer != null)
      childrenCore.Add(presenterAutomationPeer);
    return childrenCore;
  }

  protected override bool IsOffscreenCore()
  {
    if (!this.Owner.IsVisible)
      return true;
    Rect visibleBoundingRect = DataGridAutomationPeer.CalculateVisibleBoundingRect(this.Owner);
    return DoubleUtil.AreClose(visibleBoundingRect, Rect.Empty) || DoubleUtil.AreClose(visibleBoundingRect.Height, 0.0) || DoubleUtil.AreClose(visibleBoundingRect.Width, 0.0);
  }

  internal AutomationPeer RowHeaderAutomationPeer
  {
    get
    {
      DataGridRowHeader rowHeader = this.OwningDataGridRow.RowHeader;
      return rowHeader != null ? UIElementAutomationPeer.CreatePeerForElement((UIElement) rowHeader) : (AutomationPeer) null;
    }
  }

  private AutomationPeer DetailsPresenterAutomationPeer
  {
    get
    {
      DataGridDetailsPresenter detailsPresenter = this.OwningDataGridRow.DetailsPresenter;
      return detailsPresenter != null ? UIElementAutomationPeer.CreatePeerForElement((UIElement) detailsPresenter) : (AutomationPeer) null;
    }
  }

  internal void UpdateEventSource()
  {
    DataGrid dataGridOwner = this.OwningDataGridRow.DataGridOwner;
    if (dataGridOwner == null || !(UIElementAutomationPeer.CreatePeerForElement((UIElement) dataGridOwner) is DataGridAutomationPeer peerForElement))
      return;
    AutomationPeer itemPeer = (AutomationPeer) peerForElement.GetOrCreateItemPeer(this.OwningDataGridRow.Item);
    if (itemPeer == null)
      return;
    this.EventsSource = itemPeer;
  }

  private DataGridRow OwningDataGridRow => (DataGridRow) this.Owner;
}
