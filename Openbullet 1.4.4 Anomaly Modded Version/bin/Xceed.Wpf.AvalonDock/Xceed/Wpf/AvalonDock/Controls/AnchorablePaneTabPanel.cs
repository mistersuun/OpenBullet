// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.AnchorablePaneTabPanel
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class AnchorablePaneTabPanel : Panel
{
  public AnchorablePaneTabPanel() => this.FlowDirection = FlowDirection.LeftToRight;

  protected override Size MeasureOverride(Size availableSize)
  {
    double val2 = 0.0;
    double num = 0.0;
    IEnumerable<UIElement> source = this.Children.Cast<UIElement>().Where<UIElement>((Func<UIElement, bool>) (ch => ch.Visibility != Visibility.Collapsed));
    foreach (FrameworkElement frameworkElement in source)
    {
      frameworkElement.Measure(new Size(double.PositiveInfinity, availableSize.Height));
      val2 += frameworkElement.DesiredSize.Width;
      num = Math.Max(num, frameworkElement.DesiredSize.Height);
    }
    if (val2 > availableSize.Width)
    {
      double width = availableSize.Width / (double) source.Count<UIElement>();
      foreach (UIElement uiElement in source)
        uiElement.Measure(new Size(width, availableSize.Height));
    }
    return new Size(Math.Min(availableSize.Width, val2), num);
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    IEnumerable<UIElement> source = this.Children.Cast<UIElement>().Where<UIElement>((Func<UIElement, bool>) (ch => ch.Visibility != Visibility.Collapsed));
    double width1 = finalSize.Width;
    double num = source.Sum<UIElement>((Func<UIElement, double>) (ch => ch.DesiredSize.Width));
    double x = 0.0;
    if (width1 > num)
    {
      foreach (FrameworkElement frameworkElement in source)
      {
        double width2 = frameworkElement.DesiredSize.Width;
        frameworkElement.Arrange(new Rect(x, 0.0, width2, finalSize.Height));
        x += width2;
      }
    }
    else
    {
      double width3 = width1 / (double) source.Count<UIElement>();
      foreach (UIElement uiElement in source)
      {
        uiElement.Arrange(new Rect(x, 0.0, width3, finalSize.Height));
        x += width3;
      }
    }
    return finalSize;
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    if (e.LeftButton == MouseButtonState.Pressed && LayoutAnchorableTabItem.IsDraggingItem())
    {
      LayoutAnchorable model = LayoutAnchorableTabItem.GetDraggingItem().Model as LayoutAnchorable;
      DockingManager manager = model.Root.Manager;
      LayoutAnchorableTabItem.ResetDraggingItem();
      LayoutAnchorable contentModel = model;
      manager.StartDraggingFloatingWindowForContent((LayoutContent) contentModel);
    }
    base.OnMouseLeave(e);
  }
}
