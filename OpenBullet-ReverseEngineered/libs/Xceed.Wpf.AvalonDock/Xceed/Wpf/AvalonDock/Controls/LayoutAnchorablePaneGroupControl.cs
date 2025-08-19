// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorablePaneGroupControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAnchorablePaneGroupControl : 
  LayoutGridControl<ILayoutAnchorablePane>,
  ILayoutControl
{
  private LayoutAnchorablePaneGroup _model;

  internal LayoutAnchorablePaneGroupControl(LayoutAnchorablePaneGroup model)
    : base((LayoutPositionableGroup<ILayoutAnchorablePane>) model, model.Orientation)
  {
    this._model = model;
  }

  protected override void OnFixChildrenDockLengths()
  {
    if (this._model.Orientation == Orientation.Horizontal)
    {
      for (int index = 0; index < this._model.Children.Count; ++index)
      {
        ILayoutPositionableElement child = this._model.Children[index] as ILayoutPositionableElement;
        if (!child.DockWidth.IsStar)
          child.DockWidth = new GridLength(1.0, GridUnitType.Star);
      }
    }
    else
    {
      for (int index = 0; index < this._model.Children.Count; ++index)
      {
        ILayoutPositionableElement child = this._model.Children[index] as ILayoutPositionableElement;
        if (!child.DockHeight.IsStar)
          child.DockHeight = new GridLength(1.0, GridUnitType.Star);
      }
    }
  }
}
