// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.ILayoutPositionableElement
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.ComponentModel;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

internal interface ILayoutPositionableElement : 
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging,
  ILayoutElementForFloatingWindow
{
  GridLength DockWidth { get; set; }

  GridLength DockHeight { get; set; }

  double DockMinWidth { get; set; }

  double DockMinHeight { get; set; }

  bool AllowDuplicateContent { get; set; }

  bool IsVisible { get; }
}
