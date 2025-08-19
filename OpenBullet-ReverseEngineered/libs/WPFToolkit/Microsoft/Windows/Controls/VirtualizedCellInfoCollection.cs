// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.VirtualizedCellInfoCollection
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class VirtualizedCellInfoCollection : 
  IList<DataGridCellInfo>,
  ICollection<DataGridCellInfo>,
  IEnumerable<DataGridCellInfo>,
  IEnumerable
{
  private bool _isReadOnly;
  private DataGrid _owner;
  private List<VirtualizedCellInfoCollection.CellRegion> _regions;

  internal VirtualizedCellInfoCollection(DataGrid owner)
  {
    this._owner = owner;
    this._regions = new List<VirtualizedCellInfoCollection.CellRegion>();
  }

  private VirtualizedCellInfoCollection(
    DataGrid owner,
    List<VirtualizedCellInfoCollection.CellRegion> regions)
  {
    this._owner = owner;
    this._regions = regions != null ? new List<VirtualizedCellInfoCollection.CellRegion>((IEnumerable<VirtualizedCellInfoCollection.CellRegion>) regions) : new List<VirtualizedCellInfoCollection.CellRegion>();
    this.IsReadOnly = true;
  }

  internal static VirtualizedCellInfoCollection MakeEmptyCollection(DataGrid owner)
  {
    return new VirtualizedCellInfoCollection(owner, (List<VirtualizedCellInfoCollection.CellRegion>) null);
  }

  public void Add(DataGridCellInfo cell)
  {
    this._owner.Dispatcher.VerifyAccess();
    this.ValidateIsReadOnly();
    if (!this.IsValidPublicCell(cell))
      throw new ArgumentException(SR.Get(SRID.SelectedCellsCollection_InvalidItem), nameof (cell));
    if (this.Contains(cell))
      throw new ArgumentException(SR.Get(SRID.SelectedCellsCollection_DuplicateItem), nameof (cell));
    this.AddValidatedCell(cell);
  }

  internal void AddValidatedCell(DataGridCellInfo cell)
  {
    int rowIndex;
    int columnIndex;
    this.ConvertCellInfoToIndexes(cell, out rowIndex, out columnIndex);
    this.AddRegion(rowIndex, columnIndex, 1, 1);
  }

  public void Clear()
  {
    this._owner.Dispatcher.VerifyAccess();
    this.ValidateIsReadOnly();
    if (this.IsEmpty)
      return;
    VirtualizedCellInfoCollection oldItems = new VirtualizedCellInfoCollection(this._owner, this._regions);
    this._regions.Clear();
    this.OnRemove(oldItems);
  }

  public bool Contains(DataGridCellInfo cell)
  {
    if (!this.IsValidPublicCell(cell))
      throw new ArgumentException(SR.Get(SRID.SelectedCellsCollection_InvalidItem), nameof (cell));
    if (this.IsEmpty)
      return false;
    int rowIndex;
    int columnIndex;
    this.ConvertCellInfoToIndexes(cell, out rowIndex, out columnIndex);
    return this.Contains(rowIndex, columnIndex);
  }

  internal bool Contains(DataGridCell cell)
  {
    if (!this.IsEmpty)
    {
      object rowDataItem = cell.RowDataItem;
      int displayIndex = cell.Column.DisplayIndex;
      ItemCollection items = this._owner.Items;
      int count1 = items.Count;
      int count2 = this._regions.Count;
      for (int index = 0; index < count2; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion region = this._regions[index];
        if (region.Left <= displayIndex && displayIndex <= region.Right)
        {
          int bottom = region.Bottom;
          for (int top = region.Top; top <= bottom; ++top)
          {
            if (top < count1 && items[top] == rowDataItem)
              return true;
          }
        }
      }
    }
    return false;
  }

  internal bool Contains(int rowIndex, int columnIndex)
  {
    int count = this._regions.Count;
    for (int index = 0; index < count; ++index)
    {
      if (this._regions[index].Contains(columnIndex, rowIndex))
        return true;
    }
    return false;
  }

  public void CopyTo(DataGridCellInfo[] array, int arrayIndex)
  {
    List<DataGridCellInfo> list = new List<DataGridCellInfo>();
    int count = this._regions.Count;
    for (int index = 0; index < count; ++index)
      this.AddRegionToList(this._regions[index], list);
    list.CopyTo(array, arrayIndex);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) new VirtualizedCellInfoCollection.VirtualizedCellInfoCollectionEnumerator(this._owner, this._regions, this);
  }

  public IEnumerator<DataGridCellInfo> GetEnumerator()
  {
    return (IEnumerator<DataGridCellInfo>) new VirtualizedCellInfoCollection.VirtualizedCellInfoCollectionEnumerator(this._owner, this._regions, this);
  }

  public int IndexOf(DataGridCellInfo cell)
  {
    int rowIndex;
    int columnIndex;
    this.ConvertCellInfoToIndexes(cell, out rowIndex, out columnIndex);
    int count = this._regions.Count;
    int num = 0;
    for (int index = 0; index < count; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion region = this._regions[index];
      if (region.Contains(columnIndex, rowIndex))
        return num + ((rowIndex - region.Top) * region.Width + columnIndex - region.Left);
      num += region.Size;
    }
    return -1;
  }

  public void Insert(int index, DataGridCellInfo cell)
  {
    throw new NotSupportedException(SR.Get(SRID.VirtualizedCellInfoCollection_DoesNotSupportIndexChanges));
  }

  public bool Remove(DataGridCellInfo cell)
  {
    this._owner.Dispatcher.VerifyAccess();
    this.ValidateIsReadOnly();
    if (!this.IsEmpty)
    {
      int rowIndex;
      int columnIndex;
      this.ConvertCellInfoToIndexes(cell, out rowIndex, out columnIndex);
      if (this.Contains(rowIndex, columnIndex))
      {
        this.RemoveRegion(rowIndex, columnIndex, 1, 1);
        return true;
      }
    }
    return false;
  }

  public void RemoveAt(int index)
  {
    throw new NotSupportedException(SR.Get(SRID.VirtualizedCellInfoCollection_DoesNotSupportIndexChanges));
  }

  public DataGridCellInfo this[int index]
  {
    get
    {
      if (index >= 0 && index < this.Count)
        return this.GetCellInfoFromIndex(this._owner, this._regions, index);
      throw new ArgumentOutOfRangeException(nameof (index));
    }
    set
    {
      throw new NotSupportedException(SR.Get(SRID.VirtualizedCellInfoCollection_DoesNotSupportIndexChanges));
    }
  }

  public int Count
  {
    get
    {
      int count1 = 0;
      int count2 = this._regions.Count;
      for (int index = 0; index < count2; ++index)
        count1 += this._regions[index].Size;
      return count1;
    }
  }

  public bool IsReadOnly
  {
    get => this._isReadOnly;
    private set => this._isReadOnly = value;
  }

  private void OnAdd(VirtualizedCellInfoCollection newItems)
  {
    this.OnCollectionChanged(NotifyCollectionChangedAction.Add, (VirtualizedCellInfoCollection) null, newItems);
  }

  private void OnRemove(VirtualizedCellInfoCollection oldItems)
  {
    this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, oldItems, (VirtualizedCellInfoCollection) null);
  }

  protected virtual void OnCollectionChanged(
    NotifyCollectionChangedAction action,
    VirtualizedCellInfoCollection oldItems,
    VirtualizedCellInfoCollection newItems)
  {
  }

  private bool IsValidCell(DataGridCellInfo cell) => cell.IsValidForDataGrid(this._owner);

  private bool IsValidPublicCell(DataGridCellInfo cell)
  {
    return cell.IsValidForDataGrid(this._owner) && cell.IsValid;
  }

  protected bool IsEmpty
  {
    get
    {
      int count = this._regions.Count;
      for (int index = 0; index < count; ++index)
      {
        if (!this._regions[index].IsEmpty)
          return false;
      }
      return true;
    }
  }

  protected void GetBoundingRegion(out int left, out int top, out int right, out int bottom)
  {
    left = int.MaxValue;
    top = int.MaxValue;
    right = 0;
    bottom = 0;
    int count = this._regions.Count;
    for (int index = 0; index < count; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion region = this._regions[index];
      if (region.Left < left)
        left = region.Left;
      if (region.Top < top)
        top = region.Top;
      if (region.Right > right)
        right = region.Right;
      if (region.Bottom > bottom)
        bottom = region.Bottom;
    }
  }

  internal void AddRegion(int rowIndex, int columnIndex, int rowCount, int columnCount)
  {
    this.AddRegion(rowIndex, columnIndex, rowCount, columnCount, true);
  }

  private void AddRegion(
    int rowIndex,
    int columnIndex,
    int rowCount,
    int columnCount,
    bool notify)
  {
    List<VirtualizedCellInfoCollection.CellRegion> regions = new List<VirtualizedCellInfoCollection.CellRegion>();
    regions.Add(new VirtualizedCellInfoCollection.CellRegion(columnIndex, rowIndex, columnCount, rowCount));
    int count1 = this._regions.Count;
    for (int index1 = 0; index1 < count1; ++index1)
    {
      VirtualizedCellInfoCollection.CellRegion region = this._regions[index1];
      for (int index2 = 0; index2 < regions.Count; ++index2)
      {
        List<VirtualizedCellInfoCollection.CellRegion> remainder;
        if (regions[index2].Remainder(region, out remainder))
        {
          regions.RemoveAt(index2);
          --index2;
          if (remainder != null)
            regions.AddRange((IEnumerable<VirtualizedCellInfoCollection.CellRegion>) remainder);
        }
      }
    }
    if (regions.Count <= 0)
      return;
    VirtualizedCellInfoCollection newItems = new VirtualizedCellInfoCollection(this._owner, regions);
    for (int index3 = 0; index3 < count1; ++index3)
    {
      for (int index4 = 0; index4 < regions.Count; ++index4)
      {
        VirtualizedCellInfoCollection.CellRegion region = this._regions[index3];
        if (region.Union(regions[index4]))
        {
          this._regions[index3] = region;
          regions.RemoveAt(index4);
          --index4;
        }
      }
    }
    int count2 = regions.Count;
    for (int index = 0; index < count2; ++index)
      this._regions.Add(regions[index]);
    if (!notify)
      return;
    this.OnAdd(newItems);
  }

  internal void RemoveRegion(int rowIndex, int columnIndex, int rowCount, int columnCount)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    this.RemoveRegion(rowIndex, columnIndex, rowCount, columnCount, ref removeList);
    if (removeList == null || removeList.Count <= 0)
      return;
    this.OnRemove(new VirtualizedCellInfoCollection(this._owner, removeList));
  }

  private void RemoveRegion(
    int rowIndex,
    int columnIndex,
    int rowCount,
    int columnCount,
    ref List<VirtualizedCellInfoCollection.CellRegion> removeList)
  {
    if (this.IsEmpty)
      return;
    VirtualizedCellInfoCollection.CellRegion region1 = new VirtualizedCellInfoCollection.CellRegion(columnIndex, rowIndex, columnCount, rowCount);
    for (int index = 0; index < this._regions.Count; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion region2 = this._regions[index];
      VirtualizedCellInfoCollection.CellRegion region3 = region2.Intersection(region1);
      if (!region3.IsEmpty)
      {
        if (removeList == null)
          removeList = new List<VirtualizedCellInfoCollection.CellRegion>();
        removeList.Add(region3);
        this._regions.RemoveAt(index);
        List<VirtualizedCellInfoCollection.CellRegion> remainder;
        region2.Remainder(region3, out remainder);
        if (remainder != null)
        {
          this._regions.InsertRange(index, (IEnumerable<VirtualizedCellInfoCollection.CellRegion>) remainder);
          index += remainder.Count;
        }
        --index;
      }
    }
  }

  internal void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e, IList selectedRows)
  {
    if (this.IsEmpty)
      return;
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        this.OnAddRow(e.NewStartingIndex);
        break;
      case NotifyCollectionChangedAction.Remove:
        this.OnRemoveRow(e.OldStartingIndex, e.OldItems[0]);
        break;
      case NotifyCollectionChangedAction.Replace:
        this.OnReplaceRow(e.OldStartingIndex, e.OldItems[0]);
        break;
      case NotifyCollectionChangedAction.Move:
        this.OnMoveRow(e.OldStartingIndex, e.NewStartingIndex);
        break;
      case NotifyCollectionChangedAction.Reset:
        this.RestoreOnlyFullRows(selectedRows);
        break;
    }
  }

  private void OnAddRow(int rowIndex)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    int count1 = this._owner.Items.Count;
    int count2 = this._owner.Columns.Count;
    if (count2 <= 0)
      return;
    this.RemoveRegion(rowIndex, 0, count1 - 1 - rowIndex, count2, ref removeList);
    if (removeList == null)
      return;
    int count3 = removeList.Count;
    for (int index = 0; index < count3; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion cellRegion = removeList[index];
      this.AddRegion(cellRegion.Top + 1, cellRegion.Left, cellRegion.Height, cellRegion.Width, false);
    }
  }

  private void OnRemoveRow(int rowIndex, object item)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList1 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    List<VirtualizedCellInfoCollection.CellRegion> removeList2 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    int count1 = this._owner.Items.Count;
    int count2 = this._owner.Columns.Count;
    if (count2 <= 0)
      return;
    this.RemoveRegion(rowIndex + 1, 0, count1 - rowIndex, count2, ref removeList1);
    this.RemoveRegion(rowIndex, 0, 1, count2, ref removeList2);
    if (removeList1 != null)
    {
      int count3 = removeList1.Count;
      for (int index = 0; index < count3; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion cellRegion = removeList1[index];
        this.AddRegion(cellRegion.Top - 1, cellRegion.Left, cellRegion.Height, cellRegion.Width, false);
      }
    }
    if (removeList2 == null)
      return;
    this.OnRemove((VirtualizedCellInfoCollection) new VirtualizedCellInfoCollection.RemovedCellInfoCollection(this._owner, removeList2, item));
  }

  private void OnReplaceRow(int rowIndex, object item)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    this.RemoveRegion(rowIndex, 0, 1, this._owner.Columns.Count, ref removeList);
    if (removeList == null)
      return;
    this.OnRemove((VirtualizedCellInfoCollection) new VirtualizedCellInfoCollection.RemovedCellInfoCollection(this._owner, removeList, item));
  }

  private void OnMoveRow(int oldIndex, int newIndex)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList1 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    List<VirtualizedCellInfoCollection.CellRegion> removeList2 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    int count1 = this._owner.Items.Count;
    int count2 = this._owner.Columns.Count;
    if (count2 <= 0)
      return;
    this.RemoveRegion(oldIndex + 1, 0, count1 - oldIndex - 1, count2, ref removeList1);
    this.RemoveRegion(oldIndex, 0, 1, count2, ref removeList2);
    if (removeList1 != null)
    {
      int count3 = removeList1.Count;
      for (int index = 0; index < count3; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion cellRegion = removeList1[index];
        this.AddRegion(cellRegion.Top - 1, cellRegion.Left, cellRegion.Height, cellRegion.Width, false);
      }
    }
    List<VirtualizedCellInfoCollection.CellRegion> removeList3 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    this.RemoveRegion(newIndex, 0, count1 - newIndex, count2, ref removeList3);
    if (removeList2 != null)
    {
      int count4 = removeList2.Count;
      for (int index = 0; index < count4; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion cellRegion = removeList2[index];
        this.AddRegion(newIndex, cellRegion.Left, cellRegion.Height, cellRegion.Width, false);
      }
    }
    if (removeList3 == null)
      return;
    int count5 = removeList3.Count;
    for (int index = 0; index < count5; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion cellRegion = removeList3[index];
      this.AddRegion(cellRegion.Top + 1, cellRegion.Left, cellRegion.Height, cellRegion.Width, false);
    }
  }

  internal void OnColumnsChanged(
    NotifyCollectionChangedAction action,
    int oldDisplayIndex,
    DataGridColumn oldColumn,
    int newDisplayIndex,
    IList selectedRows)
  {
    if (this.IsEmpty)
      return;
    switch (action)
    {
      case NotifyCollectionChangedAction.Add:
        this.OnAddColumn(newDisplayIndex, selectedRows);
        break;
      case NotifyCollectionChangedAction.Remove:
        this.OnRemoveColumn(oldDisplayIndex, oldColumn);
        break;
      case NotifyCollectionChangedAction.Replace:
        this.OnReplaceColumn(oldDisplayIndex, oldColumn, selectedRows);
        break;
      case NotifyCollectionChangedAction.Move:
        this.OnMoveColumn(oldDisplayIndex, newDisplayIndex);
        break;
      case NotifyCollectionChangedAction.Reset:
        this._regions.Clear();
        break;
    }
  }

  private void OnAddColumn(int columnIndex, IList selectedRows)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    int count1 = this._owner.Items.Count;
    int count2 = this._owner.Columns.Count;
    if (count1 <= 0)
      return;
    this.RemoveRegion(0, columnIndex, count1, count2 - 1 - columnIndex, ref removeList);
    if (removeList != null)
    {
      int count3 = removeList.Count;
      for (int index = 0; index < count3; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion cellRegion = removeList[index];
        this.AddRegion(cellRegion.Top, cellRegion.Left + 1, cellRegion.Height, cellRegion.Width, false);
      }
    }
    this.FillInFullRowRegions(selectedRows, columnIndex, true);
  }

  private void FillInFullRowRegions(IList rows, int columnIndex, bool notify)
  {
    int count = rows.Count;
    for (int index = 0; index < count; ++index)
    {
      int rowIndex = this._owner.Items.IndexOf(rows[index]);
      if (rowIndex >= 0)
        this.AddRegion(rowIndex, columnIndex, 1, 1, notify);
    }
  }

  private void OnRemoveColumn(int columnIndex, DataGridColumn oldColumn)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList1 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    List<VirtualizedCellInfoCollection.CellRegion> removeList2 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    int count1 = this._owner.Items.Count;
    int count2 = this._owner.Columns.Count;
    if (count1 <= 0)
      return;
    this.RemoveRegion(0, columnIndex + 1, count1, count2 - columnIndex, ref removeList1);
    this.RemoveRegion(0, columnIndex, count1, 1, ref removeList2);
    if (removeList1 != null)
    {
      int count3 = removeList1.Count;
      for (int index = 0; index < count3; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion cellRegion = removeList1[index];
        this.AddRegion(cellRegion.Top, cellRegion.Left - 1, cellRegion.Height, cellRegion.Width, false);
      }
    }
    if (removeList2 == null)
      return;
    this.OnRemove((VirtualizedCellInfoCollection) new VirtualizedCellInfoCollection.RemovedCellInfoCollection(this._owner, removeList2, oldColumn));
  }

  private void OnReplaceColumn(int columnIndex, DataGridColumn oldColumn, IList selectedRows)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    this.RemoveRegion(0, columnIndex, this._owner.Items.Count, 1, ref removeList);
    if (removeList != null)
      this.OnRemove((VirtualizedCellInfoCollection) new VirtualizedCellInfoCollection.RemovedCellInfoCollection(this._owner, removeList, oldColumn));
    this.FillInFullRowRegions(selectedRows, columnIndex, true);
  }

  private void OnMoveColumn(int oldIndex, int newIndex)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList1 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    List<VirtualizedCellInfoCollection.CellRegion> removeList2 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    int count1 = this._owner.Items.Count;
    int count2 = this._owner.Columns.Count;
    if (count1 <= 0)
      return;
    this.RemoveRegion(0, oldIndex + 1, count1, count2 - oldIndex - 1, ref removeList1);
    this.RemoveRegion(0, oldIndex, count1, 1, ref removeList2);
    if (removeList1 != null)
    {
      int count3 = removeList1.Count;
      for (int index = 0; index < count3; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion cellRegion = removeList1[index];
        this.AddRegion(cellRegion.Top, cellRegion.Left - 1, cellRegion.Height, cellRegion.Width, false);
      }
    }
    List<VirtualizedCellInfoCollection.CellRegion> removeList3 = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    this.RemoveRegion(0, newIndex, count1, count2 - newIndex, ref removeList3);
    if (removeList2 != null)
    {
      int count4 = removeList2.Count;
      for (int index = 0; index < count4; ++index)
      {
        VirtualizedCellInfoCollection.CellRegion cellRegion = removeList2[index];
        this.AddRegion(cellRegion.Top, newIndex, cellRegion.Height, cellRegion.Width, false);
      }
    }
    if (removeList3 == null)
      return;
    int count5 = removeList3.Count;
    for (int index = 0; index < count5; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion cellRegion = removeList3[index];
      this.AddRegion(cellRegion.Top, cellRegion.Left + 1, cellRegion.Height, cellRegion.Width, false);
    }
  }

  internal void Union(VirtualizedCellInfoCollection collection)
  {
    int count = collection._regions.Count;
    for (int index = 0; index < count; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion region = collection._regions[index];
      this.AddRegion(region.Top, region.Left, region.Height, region.Width);
    }
  }

  internal static void Xor(VirtualizedCellInfoCollection c1, VirtualizedCellInfoCollection c2)
  {
    VirtualizedCellInfoCollection cellInfoCollection = new VirtualizedCellInfoCollection(c2._owner, c2._regions);
    int count1 = c1._regions.Count;
    for (int index = 0; index < count1; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion region = c1._regions[index];
      c2.RemoveRegion(region.Top, region.Left, region.Height, region.Width);
    }
    int count2 = cellInfoCollection._regions.Count;
    for (int index = 0; index < count2; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion region = cellInfoCollection._regions[index];
      c1.RemoveRegion(region.Top, region.Left, region.Height, region.Width);
    }
  }

  internal void ClearFullRows(IList rows)
  {
    if (this.IsEmpty)
      return;
    int count1 = this._owner.Columns.Count;
    if (this._regions.Count == 1)
    {
      VirtualizedCellInfoCollection.CellRegion region = this._regions[0];
      if (region.Width == count1 && region.Height == rows.Count)
      {
        this.Clear();
        return;
      }
    }
    List<VirtualizedCellInfoCollection.CellRegion> removeList = new List<VirtualizedCellInfoCollection.CellRegion>();
    int count2 = rows.Count;
    for (int index = 0; index < count2; ++index)
    {
      int rowIndex = this._owner.Items.IndexOf(rows[index]);
      if (rowIndex >= 0)
        this.RemoveRegion(rowIndex, 0, 1, count1, ref removeList);
    }
    if (removeList.Count <= 0)
      return;
    this.OnRemove(new VirtualizedCellInfoCollection(this._owner, removeList));
  }

  internal void RestoreOnlyFullRows(IList rows)
  {
    this.Clear();
    int count1 = this._owner.Columns.Count;
    if (count1 <= 0)
      return;
    int count2 = rows.Count;
    for (int index = 0; index < count2; ++index)
    {
      int rowIndex = this._owner.Items.IndexOf(rows[index]);
      if (rowIndex >= 0)
        this.AddRegion(rowIndex, 0, 1, count1);
    }
  }

  internal void RemoveAllButOne(DataGridCellInfo cellInfo)
  {
    if (this.IsEmpty)
      return;
    int rowIndex;
    int columnIndex;
    this.ConvertCellInfoToIndexes(cellInfo, out rowIndex, out columnIndex);
    this.RemoveAllButRegion(rowIndex, columnIndex, 1, 1);
  }

  internal void RemoveAllButOne()
  {
    if (this.IsEmpty)
      return;
    VirtualizedCellInfoCollection.CellRegion region = this._regions[0];
    this.RemoveAllButRegion(region.Top, region.Left, 1, 1);
  }

  internal void RemoveAllButOneRow(int rowIndex)
  {
    if (this.IsEmpty || rowIndex < 0)
      return;
    this.RemoveAllButRegion(rowIndex, 0, 1, this._owner.Columns.Count);
  }

  private void RemoveAllButRegion(int rowIndex, int columnIndex, int rowCount, int columnCount)
  {
    List<VirtualizedCellInfoCollection.CellRegion> removeList = (List<VirtualizedCellInfoCollection.CellRegion>) null;
    this.RemoveRegion(rowIndex, columnIndex, rowCount, columnCount, ref removeList);
    VirtualizedCellInfoCollection oldItems = new VirtualizedCellInfoCollection(this._owner, this._regions);
    this._regions.Clear();
    this._regions.Add(new VirtualizedCellInfoCollection.CellRegion(columnIndex, rowIndex, columnCount, rowCount));
    this.OnRemove(oldItems);
  }

  internal bool Intersects(int rowIndex)
  {
    VirtualizedCellInfoCollection.CellRegion region = new VirtualizedCellInfoCollection.CellRegion(0, rowIndex, this._owner.Columns.Count, 1);
    int count = this._regions.Count;
    for (int index = 0; index < count; ++index)
    {
      if (this._regions[index].Intersects(region))
        return true;
    }
    return false;
  }

  internal bool Intersects(int rowIndex, out List<int> columnIndexRanges)
  {
    VirtualizedCellInfoCollection.CellRegion region1 = new VirtualizedCellInfoCollection.CellRegion(0, rowIndex, this._owner.Columns.Count, 1);
    columnIndexRanges = (List<int>) null;
    int count = this._regions.Count;
    for (int index = 0; index < count; ++index)
    {
      VirtualizedCellInfoCollection.CellRegion region2 = this._regions[index];
      if (region2.Intersects(region1))
      {
        if (columnIndexRanges == null)
          columnIndexRanges = new List<int>();
        columnIndexRanges.Add(region2.Left);
        columnIndexRanges.Add(region2.Width);
      }
    }
    return columnIndexRanges != null;
  }

  protected DataGrid Owner => this._owner;

  private void ConvertCellInfoToIndexes(
    DataGridCellInfo cell,
    out int rowIndex,
    out int columnIndex)
  {
    columnIndex = cell.Column.DisplayIndex;
    rowIndex = this._owner.Items.IndexOf(cell.Item);
  }

  private static void ConvertIndexToIndexes(
    List<VirtualizedCellInfoCollection.CellRegion> regions,
    int index,
    out int rowIndex,
    out int columnIndex)
  {
    columnIndex = -1;
    rowIndex = -1;
    int count = regions.Count;
    for (int index1 = 0; index1 < count; ++index1)
    {
      VirtualizedCellInfoCollection.CellRegion region = regions[index1];
      int size = region.Size;
      if (index < size)
      {
        columnIndex = region.Left + index % region.Width;
        rowIndex = region.Top + index / region.Width;
        break;
      }
      index -= size;
    }
  }

  private DataGridCellInfo GetCellInfoFromIndex(
    DataGrid owner,
    List<VirtualizedCellInfoCollection.CellRegion> regions,
    int index)
  {
    int rowIndex;
    int columnIndex;
    VirtualizedCellInfoCollection.ConvertIndexToIndexes(regions, index, out rowIndex, out columnIndex);
    if (rowIndex < 0 || columnIndex < 0 || rowIndex >= owner.Items.Count || columnIndex >= owner.Columns.Count)
      return DataGridCellInfo.Unset;
    DataGridColumn column = owner.ColumnFromDisplayIndex(columnIndex);
    return this.CreateCellInfo(owner.Items[rowIndex], column, owner);
  }

  private void ValidateIsReadOnly()
  {
    if (this.IsReadOnly)
      throw new NotSupportedException(SR.Get(SRID.VirtualizedCellInfoCollection_IsReadOnly));
  }

  private void AddRegionToList(
    VirtualizedCellInfoCollection.CellRegion region,
    List<DataGridCellInfo> list)
  {
    for (int top = region.Top; top <= region.Bottom; ++top)
    {
      object rowItem = this._owner.Items[top];
      for (int left = region.Left; left <= region.Right; ++left)
      {
        DataGridColumn column = this._owner.ColumnFromDisplayIndex(left);
        DataGridCellInfo cellInfo = this.CreateCellInfo(rowItem, column, this._owner);
        list.Add(cellInfo);
      }
    }
  }

  protected virtual DataGridCellInfo CreateCellInfo(
    object rowItem,
    DataGridColumn column,
    DataGrid owner)
  {
    return new DataGridCellInfo(rowItem, column, owner);
  }

  private class VirtualizedCellInfoCollectionEnumerator : 
    IEnumerator<DataGridCellInfo>,
    IDisposable,
    IEnumerator
  {
    private DataGrid _owner;
    private List<VirtualizedCellInfoCollection.CellRegion> _regions;
    private int _current;
    private int _count;
    private VirtualizedCellInfoCollection _collection;

    public VirtualizedCellInfoCollectionEnumerator(
      DataGrid owner,
      List<VirtualizedCellInfoCollection.CellRegion> regions,
      VirtualizedCellInfoCollection collection)
    {
      this._owner = owner;
      this._regions = new List<VirtualizedCellInfoCollection.CellRegion>((IEnumerable<VirtualizedCellInfoCollection.CellRegion>) regions);
      this._current = -1;
      this._collection = collection;
      if (this._regions == null)
        return;
      int count = this._regions.Count;
      for (int index = 0; index < count; ++index)
        this._count += this._regions[index].Size;
    }

    public void Dispose() => GC.SuppressFinalize((object) this);

    public bool MoveNext()
    {
      if (this._current < this._count)
        ++this._current;
      return this.CurrentWithinBounds;
    }

    public void Reset() => this._current = -1;

    public DataGridCellInfo Current
    {
      get
      {
        return this.CurrentWithinBounds ? this._collection.GetCellInfoFromIndex(this._owner, this._regions, this._current) : DataGridCellInfo.Unset;
      }
    }

    private bool CurrentWithinBounds => this._current >= 0 && this._current < this._count;

    object IEnumerator.Current => (object) this.Current;
  }

  private struct CellRegion(int left, int top, int width, int height)
  {
    private int _left = left;
    private int _top = top;
    private int _width = width;
    private int _height = height;

    public int Left
    {
      get => this._left;
      set => this._left = value;
    }

    public int Top
    {
      get => this._top;
      set => this._top = value;
    }

    public int Right
    {
      get => this._left + this._width - 1;
      set => this._width = value - this._left + 1;
    }

    public int Bottom
    {
      get => this._top + this._height - 1;
      set => this._height = value - this._top + 1;
    }

    public int Width
    {
      get => this._width;
      set => this._width = value;
    }

    public int Height
    {
      get => this._height;
      set => this._height = value;
    }

    public bool IsEmpty => this._width == 0 || this._height == 0;

    public int Size => this._width * this._height;

    public bool Contains(int x, int y)
    {
      return !this.IsEmpty && x >= this.Left && y >= this.Top && x <= this.Right && y <= this.Bottom;
    }

    public bool Contains(VirtualizedCellInfoCollection.CellRegion region)
    {
      return this.Left <= region.Left && this.Top <= region.Top && this.Right >= region.Right && this.Bottom >= region.Bottom;
    }

    public bool Intersects(VirtualizedCellInfoCollection.CellRegion region)
    {
      return VirtualizedCellInfoCollection.CellRegion.Intersects(this.Left, this.Right, region.Left, region.Right) && VirtualizedCellInfoCollection.CellRegion.Intersects(this.Top, this.Bottom, region.Top, region.Bottom);
    }

    private static bool Intersects(int start1, int end1, int start2, int end2)
    {
      return start1 <= end2 && end1 >= start2;
    }

    public VirtualizedCellInfoCollection.CellRegion Intersection(
      VirtualizedCellInfoCollection.CellRegion region)
    {
      if (!this.Intersects(region))
        return VirtualizedCellInfoCollection.CellRegion.Empty;
      int left = Math.Max(this.Left, region.Left);
      int top = Math.Max(this.Top, region.Top);
      int num1 = Math.Min(this.Right, region.Right);
      int num2 = Math.Min(this.Bottom, region.Bottom);
      return new VirtualizedCellInfoCollection.CellRegion(left, top, num1 - left + 1, num2 - top + 1);
    }

    public bool Union(VirtualizedCellInfoCollection.CellRegion region)
    {
      if (this.Contains(region))
        return true;
      if (region.Contains(this))
      {
        this.Left = region.Left;
        this.Top = region.Top;
        this.Width = region.Width;
        this.Height = region.Height;
        return true;
      }
      bool flag1 = region.Left == this.Left && region.Width == this.Width;
      bool flag2 = region.Top == this.Top && region.Height == this.Height;
      if (flag1 || flag2)
      {
        int num1 = flag1 ? this.Top : this.Left;
        int num2 = flag1 ? this.Bottom : this.Right;
        int num3 = flag1 ? region.Top : region.Left;
        int num4 = flag1 ? region.Bottom : region.Right;
        bool flag3 = false;
        if (num4 <= num2)
          flag3 = num1 - num4 <= 1;
        else if (num1 <= num3)
          flag3 = num3 - num2 <= 1;
        if (flag3)
        {
          int right = this.Right;
          int bottom = this.Bottom;
          this.Left = Math.Min(this.Left, region.Left);
          this.Top = Math.Min(this.Top, region.Top);
          this.Right = Math.Max(right, region.Right);
          this.Bottom = Math.Max(bottom, region.Bottom);
          return true;
        }
      }
      return false;
    }

    public bool Remainder(
      VirtualizedCellInfoCollection.CellRegion region,
      out List<VirtualizedCellInfoCollection.CellRegion> remainder)
    {
      if (this.Intersects(region))
      {
        if (region.Contains(this))
        {
          remainder = (List<VirtualizedCellInfoCollection.CellRegion>) null;
        }
        else
        {
          remainder = new List<VirtualizedCellInfoCollection.CellRegion>();
          if (this.Top < region.Top)
            remainder.Add(new VirtualizedCellInfoCollection.CellRegion(this.Left, this.Top, this.Width, region.Top - this.Top));
          if (this.Left < region.Left)
          {
            int top = Math.Max(this.Top, region.Top);
            int num = Math.Min(this.Bottom, region.Bottom);
            remainder.Add(new VirtualizedCellInfoCollection.CellRegion(this.Left, top, region.Left - this.Left, num - top + 1));
          }
          if (this.Right > region.Right)
          {
            int top = Math.Max(this.Top, region.Top);
            int num = Math.Min(this.Bottom, region.Bottom);
            remainder.Add(new VirtualizedCellInfoCollection.CellRegion(region.Right + 1, top, this.Right - region.Right, num - top + 1));
          }
          if (this.Bottom > region.Bottom)
            remainder.Add(new VirtualizedCellInfoCollection.CellRegion(this.Left, region.Bottom + 1, this.Width, this.Bottom - region.Bottom));
        }
        return true;
      }
      remainder = (List<VirtualizedCellInfoCollection.CellRegion>) null;
      return false;
    }

    public static VirtualizedCellInfoCollection.CellRegion Empty
    {
      get => new VirtualizedCellInfoCollection.CellRegion(0, 0, 0, 0);
    }
  }

  private class RemovedCellInfoCollection : VirtualizedCellInfoCollection
  {
    private DataGridColumn _removedColumn;
    private object _removedItem;

    internal RemovedCellInfoCollection(
      DataGrid owner,
      List<VirtualizedCellInfoCollection.CellRegion> regions,
      DataGridColumn column)
      : base(owner, regions)
    {
      this._removedColumn = column;
    }

    internal RemovedCellInfoCollection(
      DataGrid owner,
      List<VirtualizedCellInfoCollection.CellRegion> regions,
      object item)
      : base(owner, regions)
    {
      this._removedItem = item;
    }

    protected override DataGridCellInfo CreateCellInfo(
      object rowItem,
      DataGridColumn column,
      DataGrid owner)
    {
      return this._removedColumn != null ? new DataGridCellInfo(rowItem, this._removedColumn, owner) : new DataGridCellInfo(this._removedItem, column, owner);
    }
  }
}
