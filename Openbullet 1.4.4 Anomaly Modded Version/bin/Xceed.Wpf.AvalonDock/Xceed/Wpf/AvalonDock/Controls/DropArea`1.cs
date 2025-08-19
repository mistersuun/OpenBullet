// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DropArea`1
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class DropArea<T> : IDropArea where T : FrameworkElement
{
  private Rect _detectionRect;
  private DropAreaType _type;
  private T _element;

  internal DropArea(T areaElement, DropAreaType type)
  {
    this._element = areaElement;
    this._detectionRect = areaElement.GetScreenArea();
    this._type = type;
  }

  public Rect DetectionRect => this._detectionRect;

  public DropAreaType Type => this._type;

  public T AreaElement => this._element;
}
