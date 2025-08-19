// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.RoutedEventHelper
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class RoutedEventHelper
{
  internal static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
  {
    switch (target)
    {
      case UIElement _:
        (target as UIElement).RaiseEvent(args);
        break;
      case ContentElement _:
        (target as ContentElement).RaiseEvent(args);
        break;
    }
  }

  internal static void AddHandler(
    DependencyObject element,
    RoutedEvent routedEvent,
    Delegate handler)
  {
    switch (element)
    {
      case UIElement uiElement:
        uiElement.AddHandler(routedEvent, handler);
        break;
      case ContentElement contentElement:
        contentElement.AddHandler(routedEvent, handler);
        break;
    }
  }

  internal static void RemoveHandler(
    DependencyObject element,
    RoutedEvent routedEvent,
    Delegate handler)
  {
    switch (element)
    {
      case UIElement uiElement:
        uiElement.RemoveHandler(routedEvent, handler);
        break;
      case ContentElement contentElement:
        contentElement.RemoveHandler(routedEvent, handler);
        break;
    }
  }
}
