// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.ILayoutRoot
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Collections.ObjectModel;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

public interface ILayoutRoot
{
  DockingManager Manager { get; }

  LayoutPanel RootPanel { get; }

  LayoutAnchorSide TopSide { get; }

  LayoutAnchorSide LeftSide { get; }

  LayoutAnchorSide RightSide { get; }

  LayoutAnchorSide BottomSide { get; }

  LayoutContent ActiveContent { get; set; }

  ObservableCollection<LayoutFloatingWindow> FloatingWindows { get; }

  ObservableCollection<LayoutAnchorable> Hidden { get; }

  void CollectGarbage();
}
