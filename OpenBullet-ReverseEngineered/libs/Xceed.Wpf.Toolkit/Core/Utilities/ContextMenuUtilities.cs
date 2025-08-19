// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.ContextMenuUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

public class ContextMenuUtilities
{
  public static readonly DependencyProperty OpenOnMouseLeftButtonClickProperty = DependencyProperty.RegisterAttached("OpenOnMouseLeftButtonClick", typeof (bool), typeof (ContextMenuUtilities), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(ContextMenuUtilities.OpenOnMouseLeftButtonClickChanged)));

  public static void SetOpenOnMouseLeftButtonClick(FrameworkElement element, bool value)
  {
    element.SetValue(ContextMenuUtilities.OpenOnMouseLeftButtonClickProperty, (object) value);
  }

  public static bool GetOpenOnMouseLeftButtonClick(FrameworkElement element)
  {
    return (bool) element.GetValue(ContextMenuUtilities.OpenOnMouseLeftButtonClickProperty);
  }

  public static void OpenOnMouseLeftButtonClickChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is FrameworkElement frameworkElement))
      return;
    if ((bool) e.NewValue)
      frameworkElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ContextMenuUtilities.Control_PreviewMouseLeftButtonDown);
    else
      frameworkElement.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(ContextMenuUtilities.Control_PreviewMouseLeftButtonDown);
  }

  private static void Control_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (!(sender is FrameworkElement reference) || reference.ContextMenu == null)
      return;
    for (DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject) reference); parent != null; parent = VisualTreeHelper.GetParent(parent))
    {
      if (parent is PropertyItemBase propertyItemBase)
      {
        reference.ContextMenu.DataContext = (object) propertyItemBase;
        break;
      }
    }
    reference.ContextMenu.PlacementTarget = (UIElement) reference;
    reference.ContextMenu.IsOpen = true;
  }
}
