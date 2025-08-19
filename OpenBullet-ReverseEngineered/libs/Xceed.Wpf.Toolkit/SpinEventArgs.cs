// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.SpinEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class SpinEventArgs : RoutedEventArgs
{
  public SpinDirection Direction { get; private set; }

  public bool UsingMouseWheel { get; private set; }

  public SpinEventArgs(SpinDirection direction) => this.Direction = direction;

  public SpinEventArgs(RoutedEvent routedEvent, SpinDirection direction)
    : base(routedEvent)
  {
    this.Direction = direction;
  }

  public SpinEventArgs(SpinDirection direction, bool usingMouseWheel)
  {
    this.Direction = direction;
    this.UsingMouseWheel = usingMouseWheel;
  }

  public SpinEventArgs(RoutedEvent routedEvent, SpinDirection direction, bool usingMouseWheel)
    : base(routedEvent)
  {
    this.Direction = direction;
    this.UsingMouseWheel = usingMouseWheel;
  }
}
