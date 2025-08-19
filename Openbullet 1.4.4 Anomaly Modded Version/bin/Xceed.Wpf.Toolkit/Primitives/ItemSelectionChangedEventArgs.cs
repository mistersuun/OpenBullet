// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public class ItemSelectionChangedEventArgs : RoutedEventArgs
{
  public bool IsSelected { get; private set; }

  public object Item { get; private set; }

  public ItemSelectionChangedEventArgs(
    RoutedEvent routedEvent,
    object source,
    object item,
    bool isSelected)
    : base(routedEvent, source)
  {
    this.Item = item;
    this.IsSelected = isSelected;
  }
}
