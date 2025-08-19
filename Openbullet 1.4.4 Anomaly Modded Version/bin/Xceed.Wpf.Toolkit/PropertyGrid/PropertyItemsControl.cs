// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.PropertyItemsControl
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class PropertyItemsControl : ItemsControl
{
  internal static readonly RoutedEvent PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof (PropertyItemEventHandler), typeof (PropertyItemsControl));
  internal static readonly RoutedEvent ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof (PropertyItemEventHandler), typeof (PropertyItemsControl));

  public PropertyItemsControl()
  {
    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties((object) this, new Attribute[1]
    {
      (Attribute) new PropertyFilterAttribute(PropertyFilterOptions.All)
    });
    properties.Find("VirtualizingPanel.IsVirtualizingWhenGrouping", false)?.SetValue((object) this, (object) true);
    PropertyDescriptor propertyDescriptor = properties.Find("VirtualizingPanel.CacheLengthUnit", false);
    propertyDescriptor?.SetValue((object) this, Enum.ToObject(propertyDescriptor.PropertyType, 1));
  }

  internal event PropertyItemEventHandler PreparePropertyItem
  {
    add => this.AddHandler(PropertyItemsControl.PreparePropertyItemEvent, (Delegate) value);
    remove => this.RemoveHandler(PropertyItemsControl.PreparePropertyItemEvent, (Delegate) value);
  }

  private void RaisePreparePropertyItemEvent(PropertyItemBase propertyItem, object item)
  {
    this.RaiseEvent((RoutedEventArgs) new PropertyItemEventArgs(PropertyItemsControl.PreparePropertyItemEvent, (object) this, propertyItem, item));
  }

  internal event PropertyItemEventHandler ClearPropertyItem
  {
    add => this.AddHandler(PropertyItemsControl.ClearPropertyItemEvent, (Delegate) value);
    remove => this.RemoveHandler(PropertyItemsControl.ClearPropertyItemEvent, (Delegate) value);
  }

  private void RaiseClearPropertyItemEvent(PropertyItemBase propertyItem, object item)
  {
    this.RaiseEvent((RoutedEventArgs) new PropertyItemEventArgs(PropertyItemsControl.ClearPropertyItemEvent, (object) this, propertyItem, item));
  }

  protected override bool IsItemItsOwnContainerOverride(object item) => item is PropertyItemBase;

  protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
  {
    base.PrepareContainerForItemOverride(element, item);
    this.RaisePreparePropertyItemEvent((PropertyItemBase) element, item);
  }

  protected override void ClearContainerForItemOverride(DependencyObject element, object item)
  {
    this.RaiseClearPropertyItemEvent((PropertyItemBase) element, item);
    base.ClearContainerForItemOverride(element, item);
  }
}
