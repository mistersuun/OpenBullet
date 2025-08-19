// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DocumentPaneTabPanel
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

public class DocumentPaneTabPanel : Panel
{
  public DocumentPaneTabPanel() => this.FlowDirection = FlowDirection.LeftToRight;

  protected override Size MeasureOverride(Size availableSize)
  {
    this.Children.Cast<UIElement>().Where<UIElement>((Func<UIElement, bool>) (ch => ch.Visibility != Visibility.Collapsed));
    Size size = new Size();
    foreach (FrameworkElement child in this.Children)
    {
      child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      ref Size local1 = ref size;
      double width1 = local1.Width;
      Size desiredSize = child.DesiredSize;
      double width2 = desiredSize.Width;
      local1.Width = width1 + width2;
      ref Size local2 = ref size;
      double height1 = size.Height;
      desiredSize = child.DesiredSize;
      double height2 = desiredSize.Height;
      double num = Math.Max(height1, height2);
      local2.Height = num;
    }
    return new Size(Math.Min(size.Width, availableSize.Width), size.Height);
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    IEnumerable<UIElement> uiElements = this.Children.Cast<UIElement>().Where<UIElement>((Func<UIElement, bool>) (ch => ch.Visibility != Visibility.Collapsed));
    double x = 0.0;
    bool flag = false;
    foreach (TabItem tabItem in uiElements)
    {
      LayoutContent content = tabItem.Content as LayoutContent;
      if (flag || x + tabItem.DesiredSize.Width > finalSize.Width)
      {
        if (content.IsSelected && !tabItem.IsVisible)
        {
          ILayoutContainer parent1 = content.Parent;
          ILayoutContentSelector parent2 = content.Parent as ILayoutContentSelector;
          ILayoutPane parent3 = content.Parent as ILayoutPane;
          int oldIndex = parent2.IndexOf(content);
          if (oldIndex > 0 && parent1.ChildrenCount > 1)
          {
            parent3.MoveChild(oldIndex, 0);
            parent2.SelectedContentIndex = 0;
            return this.ArrangeOverride(finalSize);
          }
        }
        tabItem.Visibility = Visibility.Hidden;
        flag = true;
      }
      else
      {
        tabItem.Visibility = Visibility.Visible;
        tabItem.Arrange(new Rect(x, 0.0, tabItem.DesiredSize.Width, finalSize.Height));
        double num1 = x;
        double actualWidth = tabItem.ActualWidth;
        Thickness margin = tabItem.Margin;
        double left = margin.Left;
        double num2 = actualWidth + left;
        margin = tabItem.Margin;
        double right = margin.Right;
        double num3 = num2 + right;
        x = num1 + num3;
      }
    }
    return finalSize;
  }

  protected override void OnMouseLeave(MouseEventArgs e) => base.OnMouseLeave(e);
}
