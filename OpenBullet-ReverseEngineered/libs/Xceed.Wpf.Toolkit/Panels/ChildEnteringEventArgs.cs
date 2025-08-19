// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.ChildEnteringEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public class ChildEnteringEventArgs : RoutedEventArgs
{
  private readonly Rect _arrangeRect;
  private readonly UIElement _child;
  private Rect? _enterFrom;

  public ChildEnteringEventArgs(UIElement child, Rect? enterFrom, Rect arrangeRect)
  {
    this._child = child;
    this._enterFrom = enterFrom;
    this._arrangeRect = arrangeRect;
  }

  public Rect ArrangeRect => this._arrangeRect;

  public UIElement Child => this._child;

  public Rect? EnterFrom
  {
    get => this._enterFrom;
    set => this._enterFrom = value;
  }
}
