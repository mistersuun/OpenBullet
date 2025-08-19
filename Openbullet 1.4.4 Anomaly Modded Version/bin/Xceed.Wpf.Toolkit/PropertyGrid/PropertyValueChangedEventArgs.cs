// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class PropertyValueChangedEventArgs : RoutedEventArgs
{
  public object NewValue { get; set; }

  public object OldValue { get; set; }

  public PropertyValueChangedEventArgs(
    RoutedEvent routedEvent,
    object source,
    object oldValue,
    object newValue)
    : base(routedEvent, source)
  {
    this.NewValue = newValue;
    this.OldValue = oldValue;
  }
}
