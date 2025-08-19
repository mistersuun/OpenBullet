// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.VisualTreeExtensions
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

#nullable disable
namespace System.Windows.Controls;

internal static class VisualTreeExtensions
{
  internal static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
  {
    int childCount = VisualTreeHelper.GetChildrenCount(parent);
    for (int counter = 0; counter < childCount; ++counter)
      yield return VisualTreeHelper.GetChild(parent, counter);
  }

  internal static IEnumerable<FrameworkElement> GetLogicalChildrenBreadthFirst(
    this FrameworkElement parent)
  {
    Queue<FrameworkElement> queue = new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());
    while (queue.Count > 0)
    {
      FrameworkElement element = queue.Dequeue();
      yield return element;
      foreach (FrameworkElement frameworkElement in element.GetVisualChildren().OfType<FrameworkElement>())
        queue.Enqueue(frameworkElement);
    }
  }
}
