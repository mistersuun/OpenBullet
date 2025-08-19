// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.ISelectionAdapter
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Collections;
using System.Windows.Automation.Peers;
using System.Windows.Input;

#nullable disable
namespace System.Windows.Controls;

public interface ISelectionAdapter
{
  object SelectedItem { get; set; }

  event SelectionChangedEventHandler SelectionChanged;

  IEnumerable ItemsSource { get; set; }

  event RoutedEventHandler Commit;

  event RoutedEventHandler Cancel;

  void HandleKeyDown(KeyEventArgs e);

  AutomationPeer CreateAutomationPeer();
}
