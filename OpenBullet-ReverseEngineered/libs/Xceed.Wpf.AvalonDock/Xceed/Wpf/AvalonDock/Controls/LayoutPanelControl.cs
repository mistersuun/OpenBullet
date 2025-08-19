// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutPanelControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutPanelControl : LayoutGridControl<ILayoutPanelElement>, ILayoutControl
{
  private LayoutPanel _model;

  internal LayoutPanelControl(LayoutPanel model)
    : base((LayoutPositionableGroup<ILayoutPanelElement>) model, model.Orientation)
  {
    this._model = model;
  }

  protected override void OnFixChildrenDockLengths()
  {
    if (this.ActualWidth == 0.0 || this.ActualHeight == 0.0)
      return;
    LayoutPanel model = this._model;
    if (this._model.Orientation == Orientation.Horizontal)
    {
      if (this._model.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>())
      {
        for (int index = 0; index < this._model.Children.Count; ++index)
        {
          ILayoutContainer child1 = this._model.Children[index] as ILayoutContainer;
          ILayoutPositionableElement child2 = this._model.Children[index] as ILayoutPositionableElement;
          if (child1 != null && (child1.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() || child1.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>()))
            child2.DockWidth = new GridLength(1.0, GridUnitType.Star);
          else if (child2 != null && child2.DockWidth.IsStar)
          {
            double num = Math.Max(Math.Min(Math.Max((child2 as ILayoutPositionableElementWithActualSize).ActualWidth, child2.DockMinWidth), this.ActualWidth / 2.0), child2.DockMinWidth);
            child2.DockWidth = new GridLength(num, GridUnitType.Pixel);
          }
        }
      }
      else
      {
        for (int index = 0; index < this._model.Children.Count; ++index)
        {
          ILayoutPositionableElement child = this._model.Children[index] as ILayoutPositionableElement;
          if (!child.DockWidth.IsStar)
            child.DockWidth = new GridLength(1.0, GridUnitType.Star);
        }
      }
    }
    else if (this._model.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>())
    {
      for (int index = 0; index < this._model.Children.Count; ++index)
      {
        ILayoutContainer child3 = this._model.Children[index] as ILayoutContainer;
        ILayoutPositionableElement child4 = this._model.Children[index] as ILayoutPositionableElement;
        if (child3 != null && (child3.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() || child3.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>()))
          child4.DockHeight = new GridLength(1.0, GridUnitType.Star);
        else if (child4 != null && child4.DockHeight.IsStar)
        {
          double num = Math.Max(Math.Min(Math.Max((child4 as ILayoutPositionableElementWithActualSize).ActualHeight, child4.DockMinHeight), this.ActualHeight / 2.0), child4.DockMinHeight);
          child4.DockHeight = new GridLength(num, GridUnitType.Pixel);
        }
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
