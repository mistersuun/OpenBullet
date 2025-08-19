// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.TreeHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class TreeHelper
{
  public static DependencyObject GetParent(DependencyObject element)
  {
    return TreeHelper.GetParent(element, true);
  }

  private static DependencyObject GetParent(DependencyObject element, bool recurseIntoPopup)
  {
    if (recurseIntoPopup && element is Popup popup && popup.PlacementTarget != null)
      return (DependencyObject) popup.PlacementTarget;
    DependencyObject parent = !(element is Visual reference) ? (DependencyObject) null : VisualTreeHelper.GetParent((DependencyObject) reference);
    if (parent == null)
    {
      switch (element)
      {
        case FrameworkElement frameworkElement:
          parent = frameworkElement.Parent ?? frameworkElement.TemplatedParent;
          break;
        case FrameworkContentElement frameworkContentElement:
          parent = frameworkContentElement.Parent ?? frameworkContentElement.TemplatedParent;
          break;
      }
    }
    return parent;
  }

  public static T FindParent<T>(DependencyObject startingObject) where T : DependencyObject
  {
    return TreeHelper.FindParent<T>(startingObject, false, (Func<T, bool>) null);
  }

  public static T FindParent<T>(DependencyObject startingObject, bool checkStartingObject) where T : DependencyObject
  {
    return TreeHelper.FindParent<T>(startingObject, checkStartingObject, (Func<T, bool>) null);
  }

  public static T FindParent<T>(
    DependencyObject startingObject,
    bool checkStartingObject,
    Func<T, bool> additionalCheck)
    where T : DependencyObject
  {
    for (DependencyObject element = checkStartingObject ? startingObject : TreeHelper.GetParent(startingObject, true); element != null; element = TreeHelper.GetParent(element, true))
    {
      if (element is T parent && (additionalCheck == null || additionalCheck(parent)))
        return parent;
    }
    return default (T);
  }

  public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
  {
    return TreeHelper.FindChild<T>(parent, (Func<T, bool>) null);
  }

  public static T FindChild<T>(DependencyObject parent, Func<T, bool> additionalCheck) where T : DependencyObject
  {
    int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
    for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
    {
      if (VisualTreeHelper.GetChild(parent, childIndex) is T child && (additionalCheck == null || additionalCheck(child)))
        return child;
    }
    for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
    {
      T child = TreeHelper.FindChild<T>(VisualTreeHelper.GetChild(parent, childIndex), additionalCheck);
      if ((object) child != null)
        return child;
    }
    return default (T);
  }

  public static bool IsDescendantOf(DependencyObject element, DependencyObject parent)
  {
    return TreeHelper.IsDescendantOf(element, parent, true);
  }

  public static bool IsDescendantOf(
    DependencyObject element,
    DependencyObject parent,
    bool recurseIntoPopup)
  {
    for (; element != null; element = TreeHelper.GetParent(element, recurseIntoPopup))
    {
      if (element == parent)
        return true;
    }
    return false;
  }
}
