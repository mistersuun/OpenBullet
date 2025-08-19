// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.BindingHelper
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class BindingHelper
{
  public static void RebindInactiveBindings(DependencyObject dependencyObject)
  {
    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(dependencyObject.GetType()))
    {
      DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(property);
      if (dpd != null)
      {
        BindingExpressionBase binding = BindingOperations.GetBindingExpressionBase(dependencyObject, dpd.DependencyProperty);
        if (binding != null)
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Delegate) (() =>
          {
            dependencyObject.ClearValue(dpd.DependencyProperty);
            BindingOperations.SetBinding(dependencyObject, dpd.DependencyProperty, binding.ParentBindingBase);
          }));
      }
    }
  }
}
