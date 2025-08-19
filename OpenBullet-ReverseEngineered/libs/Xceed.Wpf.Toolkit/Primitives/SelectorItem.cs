// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.SelectorItem
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public class SelectorItem : ContentControl
{
  public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool?), typeof (SelectorItem), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(SelectorItem.OnIsSelectedChanged)));
  public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof (SelectorItem));
  public static readonly RoutedEvent UnselectedEvent = Selector.UnSelectedEvent.AddOwner(typeof (SelectorItem));

  static SelectorItem()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SelectorItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SelectorItem)));
  }

  public bool? IsSelected
  {
    get => (bool?) this.GetValue(SelectorItem.IsSelectedProperty);
    set => this.SetValue(SelectorItem.IsSelectedProperty, (object) value);
  }

  private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is SelectorItem selectorItem))
      return;
    selectorItem.OnIsSelectedChanged((bool?) e.OldValue, (bool?) e.NewValue);
  }

  protected virtual void OnIsSelectedChanged(bool? oldValue, bool? newValue)
  {
    if (!newValue.HasValue)
      return;
    if (newValue.Value)
      this.RaiseEvent(new RoutedEventArgs(Selector.SelectedEvent, (object) this));
    else
      this.RaiseEvent(new RoutedEventArgs(Selector.UnSelectedEvent, (object) this));
  }

  internal Selector ParentSelector
  {
    get => ItemsControl.ItemsControlFromItemContainer((DependencyObject) this) as Selector;
  }
}
