// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.IOverlayWindow
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Collections.Generic;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal interface IOverlayWindow
{
  IEnumerable<IDropTarget> GetTargets();

  void DragEnter(LayoutFloatingWindowControl floatingWindow);

  void DragLeave(LayoutFloatingWindowControl floatingWindow);

  void DragEnter(IDropArea area);

  void DragLeave(IDropArea area);

  void DragEnter(IDropTarget target);

  void DragLeave(IDropTarget target);

  void DragDrop(IDropTarget target);
}
