// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.SelectedCellsCollection
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Collections.Specialized;

#nullable disable
namespace Microsoft.Windows.Controls;

internal sealed class SelectedCellsCollection : VirtualizedCellInfoCollection
{
  internal SelectedCellsCollection(DataGrid owner)
    : base(owner)
  {
  }

  internal bool GetSelectionRange(
    out int minColumnDisplayIndex,
    out int maxColumnDisplayIndex,
    out int minRowIndex,
    out int maxRowIndex)
  {
    if (this.IsEmpty)
    {
      minColumnDisplayIndex = -1;
      maxColumnDisplayIndex = -1;
      minRowIndex = -1;
      maxRowIndex = -1;
      return false;
    }
    this.GetBoundingRegion(out minColumnDisplayIndex, out minRowIndex, out maxColumnDisplayIndex, out maxRowIndex);
    return true;
  }

  protected override void OnCollectionChanged(
    NotifyCollectionChangedAction action,
    VirtualizedCellInfoCollection oldItems,
    VirtualizedCellInfoCollection newItems)
  {
    this.Owner.OnSelectedCellsChanged(action, oldItems, newItems);
  }
}
