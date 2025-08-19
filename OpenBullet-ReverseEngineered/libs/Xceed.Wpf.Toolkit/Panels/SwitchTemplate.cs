// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.SwitchTemplate
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public static class SwitchTemplate
{
  public static readonly DependencyProperty IDProperty = DependencyProperty.RegisterAttached("ID", typeof (string), typeof (SwitchTemplate), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(SwitchTemplate.OnIDChanged)));

  public static string GetID(DependencyObject d) => (string) d.GetValue(SwitchTemplate.IDProperty);

  public static void SetID(DependencyObject d, string value)
  {
    d.SetValue(SwitchTemplate.IDProperty, (object) value);
  }

  private static void OnIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue == null || !(d is UIElement))
      return;
    SwitchPresenter parentPresenter = VisualTreeHelperEx.FindAncestorByType<SwitchPresenter>(d);
    if (parentPresenter != null)
      parentPresenter.RegisterID(e.NewValue as string, d as FrameworkElement);
    else
      d.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Delegate) (() =>
      {
        parentPresenter = VisualTreeHelperEx.FindAncestorByType<SwitchPresenter>(d);
        parentPresenter?.RegisterID(e.NewValue as string, d as FrameworkElement);
      }));
  }
}
