// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.CategoryGroupStyleSelector
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class CategoryGroupStyleSelector : StyleSelector
{
  public Style SingleDefaultCategoryItemGroupStyle { get; set; }

  public Style ItemGroupStyle { get; set; }

  public override Style SelectStyle(object item, DependencyObject container)
  {
    CollectionViewGroup collectionViewGroup = item as CollectionViewGroup;
    if (collectionViewGroup.Name != null && !collectionViewGroup.Name.Equals((object) CategoryAttribute.Default.Category))
      return this.ItemGroupStyle;
    while (container != null)
    {
      container = VisualTreeHelper.GetParent(container);
      if (container is ItemsControl)
        break;
    }
    return container is ItemsControl itemsControl && itemsControl.Items.Count > 0 && itemsControl.Items.Groups.Count == 1 ? this.SingleDefaultCategoryItemGroupStyle : this.ItemGroupStyle;
  }
}
