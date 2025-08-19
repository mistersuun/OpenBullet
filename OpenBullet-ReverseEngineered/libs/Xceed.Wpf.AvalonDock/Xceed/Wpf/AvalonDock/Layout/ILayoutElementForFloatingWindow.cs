// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.ILayoutElementForFloatingWindow
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

internal interface ILayoutElementForFloatingWindow
{
  double FloatingWidth { get; set; }

  double FloatingHeight { get; set; }

  double FloatingLeft { get; set; }

  double FloatingTop { get; set; }

  bool IsMaximized { get; set; }
}
