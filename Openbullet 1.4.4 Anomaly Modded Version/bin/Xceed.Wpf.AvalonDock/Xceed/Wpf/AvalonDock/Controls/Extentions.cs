// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.Extentions
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public static class Extentions
{
  public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
  {
    if (depObj != null)
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
      {
        DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
        if (child != null && child is T)
          yield return (T) child;
        foreach (T visualChild in child.FindVisualChildren<T>())
          yield return visualChild;
        child = (DependencyObject) null;
      }
    }
  }

  public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
  {
    if (depObj != null)
    {
      foreach (DependencyObject child in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>())
      {
        if (child != null && child is T)
          yield return (T) child;
        foreach (T logicalChild in child.FindLogicalChildren<T>())
          yield return logicalChild;
      }
    }
  }

  public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
  {
    DependencyObject dependencyObject = initial;
    DependencyObject visualTreeRoot = initial;
    for (; dependencyObject != null; dependencyObject = dependencyObject is Visual || dependencyObject is Visual3D ? VisualTreeHelper.GetParent(dependencyObject) : LogicalTreeHelper.GetParent(dependencyObject))
      visualTreeRoot = dependencyObject;
    return visualTreeRoot;
  }

  public static T FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : class
  {
    DependencyObject reference = dependencyObject;
    do
    {
      reference = VisualTreeHelper.GetParent(reference);
    }
    while (reference != null && !(reference is T));
    return reference as T;
  }

  public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject) where T : class
  {
    DependencyObject current = dependencyObject;
    do
    {
      DependencyObject reference = current;
      current = LogicalTreeHelper.GetParent(current) ?? VisualTreeHelper.GetParent(reference);
    }
    while (current != null && !(current is T));
    return current as T;
  }
}
