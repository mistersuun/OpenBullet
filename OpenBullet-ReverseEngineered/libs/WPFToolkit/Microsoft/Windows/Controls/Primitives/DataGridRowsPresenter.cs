// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

public class DataGridRowsPresenter : VirtualizingStackPanel
{
  private Microsoft.Windows.Controls.DataGrid _owner;
  private Size _availableSize;

  internal void InternalBringIndexIntoView(int index) => this.BringIndexIntoView(index);

  protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost)
  {
    base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
    if (newIsItemsHost)
    {
      Microsoft.Windows.Controls.DataGrid owner = this.Owner;
      if (owner == null)
        return;
      IItemContainerGenerator containerGenerator = (IItemContainerGenerator) owner.ItemContainerGenerator;
      if (containerGenerator == null || containerGenerator != containerGenerator.GetItemContainerGeneratorForPanel((Panel) this))
        return;
      owner.InternalItemsHost = (Panel) this;
    }
    else
    {
      if (this._owner != null && this._owner.InternalItemsHost == this)
        this._owner.InternalItemsHost = (Panel) null;
      this._owner = (Microsoft.Windows.Controls.DataGrid) null;
    }
  }

  protected override void OnViewportSizeChanged(Size oldViewportSize, Size newViewportSize)
  {
    Microsoft.Windows.Controls.DataGrid owner = this.Owner;
    if (owner == null)
      return;
    ScrollContentPresenter contentPresenter = owner.InternalScrollContentPresenter;
    if (contentPresenter != null && !contentPresenter.CanContentScroll)
      return;
    owner.OnViewportSizeChanged(oldViewportSize, newViewportSize);
  }

  protected override Size MeasureOverride(Size constraint)
  {
    this._availableSize = constraint;
    return base.MeasureOverride(constraint);
  }

  internal Size AvailableSize => this._availableSize;

  protected override void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
  {
    base.OnCleanUpVirtualizedItem(e);
    if (e.UIElement == null || !Validation.GetHasError((DependencyObject) e.UIElement))
      return;
    e.Cancel = true;
  }

  internal Microsoft.Windows.Controls.DataGrid Owner
  {
    get
    {
      if (this._owner == null)
        this._owner = ItemsControl.GetItemsOwner((DependencyObject) this) as Microsoft.Windows.Controls.DataGrid;
      return this._owner;
    }
  }
}
