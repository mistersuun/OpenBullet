// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DropTargetBase
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal abstract class DropTargetBase : DependencyObject
{
  public static readonly DependencyProperty IsDraggingOverProperty = DependencyProperty.RegisterAttached("IsDraggingOver", typeof (bool), typeof (DropTargetBase), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));

  public static bool GetIsDraggingOver(DependencyObject d)
  {
    return (bool) d.GetValue(DropTargetBase.IsDraggingOverProperty);
  }

  public static void SetIsDraggingOver(DependencyObject d, bool value)
  {
    d.SetValue(DropTargetBase.IsDraggingOverProperty, (object) value);
  }
}
