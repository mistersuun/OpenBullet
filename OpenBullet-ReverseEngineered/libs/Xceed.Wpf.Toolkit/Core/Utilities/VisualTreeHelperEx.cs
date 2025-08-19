// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.VisualTreeHelperEx
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

public static class VisualTreeHelperEx
{
  public static DependencyObject FindAncestorByType(
    DependencyObject element,
    Type type,
    bool specificTypeOnly)
  {
    if (element == null)
      return (DependencyObject) null;
    return (specificTypeOnly ? (element.GetType() == type ? 1 : 0) : (element.GetType() == type ? 1 : (element.GetType().IsSubclassOf(type) ? 1 : 0))) != 0 ? element : VisualTreeHelperEx.FindAncestorByType(VisualTreeHelper.GetParent(element), type, specificTypeOnly);
  }

  public static T FindAncestorByType<T>(DependencyObject depObj) where T : DependencyObject
  {
    if (depObj == null)
      return default (T);
    return depObj is T obj ? obj : VisualTreeHelperEx.FindAncestorByType<T>(VisualTreeHelper.GetParent(depObj));
  }

  public static Visual FindDescendantByName(Visual element, string name)
  {
    if (element != null && element is FrameworkElement && (element as FrameworkElement).Name == name)
      return element;
    Visual descendantByName = (Visual) null;
    if (element is FrameworkElement)
      (element as FrameworkElement).ApplyTemplate();
    for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) element); ++childIndex)
    {
      descendantByName = VisualTreeHelperEx.FindDescendantByName(VisualTreeHelper.GetChild((DependencyObject) element, childIndex) as Visual, name);
      if (descendantByName != null)
        break;
    }
    return descendantByName;
  }

  public static Visual FindDescendantByType(Visual element, Type type)
  {
    return VisualTreeHelperEx.FindDescendantByType(element, type, true);
  }

  public static Visual FindDescendantByType(Visual element, Type type, bool specificTypeOnly)
  {
    if (element == null)
      return (Visual) null;
    if ((specificTypeOnly ? (element.GetType() == type ? 1 : 0) : (element.GetType() == type ? 1 : (element.GetType().IsSubclassOf(type) ? 1 : 0))) != 0)
      return element;
    Visual descendantByType = (Visual) null;
    if (element is FrameworkElement)
      (element as FrameworkElement).ApplyTemplate();
    for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) element); ++childIndex)
    {
      descendantByType = VisualTreeHelperEx.FindDescendantByType(VisualTreeHelper.GetChild((DependencyObject) element, childIndex) as Visual, type, specificTypeOnly);
      if (descendantByType != null)
        break;
    }
    return descendantByType;
  }

  public static T FindDescendantByType<T>(Visual element) where T : Visual
  {
    return (T) VisualTreeHelperEx.FindDescendantByType(element, typeof (T));
  }

  public static Visual FindDescendantWithPropertyValue(
    Visual element,
    DependencyProperty dp,
    object value)
  {
    if (element == null)
      return (Visual) null;
    if (element.GetValue(dp).Equals(value))
      return element;
    Visual withPropertyValue = (Visual) null;
    if (element is FrameworkElement)
      (element as FrameworkElement).ApplyTemplate();
    for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject) element); ++childIndex)
    {
      withPropertyValue = VisualTreeHelperEx.FindDescendantWithPropertyValue(VisualTreeHelper.GetChild((DependencyObject) element, childIndex) as Visual, dp, value);
      if (withPropertyValue != null)
        break;
    }
    return withPropertyValue;
  }
}
