// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridColumnHeadersPresenterAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Automation.Peers;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridColumnHeadersPresenterAutomationPeer(
  DataGridColumnHeadersPresenter owner) : FrameworkElementAutomationPeer((FrameworkElement) owner)
{
  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Header;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  protected override bool IsContentElementCore() => false;
}
