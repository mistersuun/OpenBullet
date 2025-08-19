// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.ContextMenuEx
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class ContextMenuEx : ContextMenu
{
  protected override DependencyObject GetContainerForItemOverride()
  {
    return (DependencyObject) new MenuItemEx();
  }

  protected override void OnOpened(RoutedEventArgs e)
  {
    BindingOperations.GetBindingExpression((DependencyObject) this, ItemsControl.ItemsSourceProperty).UpdateTarget();
    base.OnOpened(e);
  }
}
