// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.ChildrenTreeChangedEventArgs
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

public class ChildrenTreeChangedEventArgs : EventArgs
{
  public ChildrenTreeChangedEventArgs(ChildrenTreeChange change) => this.Change = change;

  public ChildrenTreeChange Change { get; private set; }
}
