// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.SelectedCellsChangedEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Windows.Controls;

public class SelectedCellsChangedEventArgs : EventArgs
{
  private IList<DataGridCellInfo> _addedCells;
  private IList<DataGridCellInfo> _removedCells;

  public SelectedCellsChangedEventArgs(
    List<DataGridCellInfo> addedCells,
    List<DataGridCellInfo> removedCells)
  {
    if (addedCells == null)
      throw new ArgumentNullException(nameof (addedCells));
    if (removedCells == null)
      throw new ArgumentNullException(nameof (removedCells));
    this._addedCells = (IList<DataGridCellInfo>) addedCells.AsReadOnly();
    this._removedCells = (IList<DataGridCellInfo>) removedCells.AsReadOnly();
  }

  public SelectedCellsChangedEventArgs(
    ReadOnlyCollection<DataGridCellInfo> addedCells,
    ReadOnlyCollection<DataGridCellInfo> removedCells)
  {
    if (addedCells == null)
      throw new ArgumentNullException(nameof (addedCells));
    if (removedCells == null)
      throw new ArgumentNullException(nameof (removedCells));
    this._addedCells = (IList<DataGridCellInfo>) addedCells;
    this._removedCells = (IList<DataGridCellInfo>) removedCells;
  }

  internal SelectedCellsChangedEventArgs(
    DataGrid owner,
    VirtualizedCellInfoCollection addedCells,
    VirtualizedCellInfoCollection removedCells)
  {
    this._addedCells = addedCells != null ? (IList<DataGridCellInfo>) addedCells : (IList<DataGridCellInfo>) VirtualizedCellInfoCollection.MakeEmptyCollection(owner);
    this._removedCells = removedCells != null ? (IList<DataGridCellInfo>) removedCells : (IList<DataGridCellInfo>) VirtualizedCellInfoCollection.MakeEmptyCollection(owner);
  }

  public IList<DataGridCellInfo> AddedCells => this._addedCells;

  public IList<DataGridCellInfo> RemovedCells => this._removedCells;
}
