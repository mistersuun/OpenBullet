// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyItemEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class PropertyItemEventArgs : RoutedEventArgs
{
  public PropertyItemBase PropertyItem { get; private set; }

  public object Item { get; private set; }

  public PropertyItemEventArgs(
    RoutedEvent routedEvent,
    object source,
    PropertyItemBase propertyItem,
    object item)
    : base(routedEvent, source)
  {
    this.PropertyItem = propertyItem;
    this.Item = item;
  }
}
