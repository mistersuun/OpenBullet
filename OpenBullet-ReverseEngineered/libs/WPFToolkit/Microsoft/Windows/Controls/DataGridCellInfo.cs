// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridCellInfo
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Windows.Controls;

public struct DataGridCellInfo
{
  private object _item;
  private DataGridColumn _column;
  private WeakReference _owner;

  public DataGridCellInfo(object item, DataGridColumn column)
  {
    if (column == null)
      throw new ArgumentNullException(nameof (column));
    this._item = item;
    this._column = column;
    this._owner = (WeakReference) null;
  }

  public DataGridCellInfo(DataGridCell cell)
  {
    this._item = cell != null ? cell.RowDataItem : throw new ArgumentNullException(nameof (cell));
    this._column = cell.Column;
    this._owner = new WeakReference((object) cell.DataGridOwner);
  }

  internal DataGridCellInfo(object item, DataGridColumn column, DataGrid owner)
  {
    this._item = item;
    this._column = column;
    this._owner = new WeakReference((object) owner);
  }

  internal DataGridCellInfo(object item)
  {
    this._item = item;
    this._column = (DataGridColumn) null;
    this._owner = (WeakReference) null;
  }

  private DataGridCellInfo(DataGrid owner, DataGridColumn column, object item)
  {
    this._item = item;
    this._column = column;
    this._owner = new WeakReference((object) owner);
  }

  internal static DataGridCellInfo CreatePossiblyPartialCellInfo(
    object item,
    DataGridColumn column,
    DataGrid owner)
  {
    return item == null && column == null ? DataGridCellInfo.Unset : new DataGridCellInfo(owner, column, item == null ? DependencyProperty.UnsetValue : item);
  }

  public object Item => this._item;

  public DataGridColumn Column => this._column;

  public override bool Equals(object obj) => obj is DataGridCellInfo cell && this.EqualsImpl(cell);

  public static bool operator ==(DataGridCellInfo cell1, DataGridCellInfo cell2)
  {
    return cell1.EqualsImpl(cell2);
  }

  public static bool operator !=(DataGridCellInfo cell1, DataGridCellInfo cell2)
  {
    return !cell1.EqualsImpl(cell2);
  }

  internal bool EqualsImpl(DataGridCellInfo cell)
  {
    return cell._item == this._item && cell._column == this._column && cell.Owner == this.Owner;
  }

  public override int GetHashCode()
  {
    return (this._item == null ? 0 : this._item.GetHashCode()) ^ (this._column == null ? 0 : this._column.GetHashCode());
  }

  public bool IsValid => this.ArePropertyValuesValid;

  internal bool IsValidForDataGrid(DataGrid dataGrid)
  {
    DataGrid owner = this.Owner;
    return this.ArePropertyValuesValid && owner == dataGrid || owner == null;
  }

  private bool ArePropertyValuesValid
  {
    get => this._item != DependencyProperty.UnsetValue && this._column != null;
  }

  internal static DataGridCellInfo Unset => new DataGridCellInfo(DependencyProperty.UnsetValue);

  private DataGrid Owner => this._owner != null ? (DataGrid) this._owner.Target : (DataGrid) null;
}
