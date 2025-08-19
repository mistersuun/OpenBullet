// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridColumnCollection
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class DataGridColumnCollection : ObservableCollection<DataGridColumn>
{
  private DataGrid _dataGridOwner;
  private bool _isUpdatingDisplayIndex;
  private List<int> _displayIndexMap;
  private bool _displayIndexMapInitialized;
  private bool _isClearingDisplayIndex;
  private bool _columnWidthsComputationPending;
  private Dictionary<DataGridColumn, DataGridLength> _originalWidthsForResize;
  private double? _averageColumnWidth = new double?();
  private List<RealizedColumnsBlock> _realizedColumnsBlockListForNonVirtualizedRows;
  private List<RealizedColumnsBlock> _realizedColumnsBlockListForVirtualizedRows;

  internal DataGridColumnCollection(DataGrid dataGridOwner)
  {
    this.DisplayIndexMap = new List<int>(5);
    this._dataGridOwner = dataGridOwner;
    this.RealizedColumnsBlockListForNonVirtualizedRows = (List<RealizedColumnsBlock>) null;
    this.RealizedColumnsDisplayIndexBlockListForNonVirtualizedRows = (List<RealizedColumnsBlock>) null;
    this.RebuildRealizedColumnsBlockListForNonVirtualizedRows = true;
    this.RealizedColumnsBlockListForVirtualizedRows = (List<RealizedColumnsBlock>) null;
    this.RealizedColumnsDisplayIndexBlockListForVirtualizedRows = (List<RealizedColumnsBlock>) null;
    this.RebuildRealizedColumnsBlockListForVirtualizedRows = true;
  }

  protected override void InsertItem(int index, DataGridColumn item)
  {
    if (item == null)
      throw new ArgumentNullException(nameof (item), SR.Get(SRID.DataGrid_NullColumn));
    if (item.DataGridOwner != null)
      throw new ArgumentException(SR.Get(SRID.DataGrid_InvalidColumnReuse, item.Header), nameof (item));
    if (this.DisplayIndexMapInitialized)
      this.ValidateDisplayIndex(item, item.DisplayIndex, true);
    base.InsertItem(index, item);
    item.CoerceValue(DataGridColumn.IsFrozenProperty);
  }

  protected override void SetItem(int index, DataGridColumn item)
  {
    if (item == null)
      throw new ArgumentNullException(nameof (item), SR.Get(SRID.DataGrid_NullColumn));
    if (index >= this.Count || index < 0)
      throw new ArgumentOutOfRangeException(nameof (index), SR.Get(SRID.DataGrid_ColumnIndexOutOfRange, item.Header));
    if (item.DataGridOwner != null && this[index] != item)
      throw new ArgumentException(SR.Get(SRID.DataGrid_InvalidColumnReuse, item.Header), nameof (item));
    if (this.DisplayIndexMapInitialized)
      this.ValidateDisplayIndex(item, item.DisplayIndex);
    base.SetItem(index, item);
    item.CoerceValue(DataGridColumn.IsFrozenProperty);
  }

  protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case NotifyCollectionChangedAction.Add:
        if (this.DisplayIndexMapInitialized)
          this.UpdateDisplayIndexForNewColumns(e.NewItems, e.NewStartingIndex);
        this.InvalidateHasVisibleStarColumns();
        break;
      case NotifyCollectionChangedAction.Remove:
        if (this.DisplayIndexMapInitialized)
          this.UpdateDisplayIndexForRemovedColumns(e.OldItems, e.OldStartingIndex);
        this.ClearDisplayIndex(e.OldItems, e.NewItems);
        this.InvalidateHasVisibleStarColumns();
        break;
      case NotifyCollectionChangedAction.Replace:
        if (this.DisplayIndexMapInitialized)
          this.UpdateDisplayIndexForReplacedColumn(e.OldItems, e.NewItems);
        this.ClearDisplayIndex(e.OldItems, e.NewItems);
        this.InvalidateHasVisibleStarColumns();
        break;
      case NotifyCollectionChangedAction.Move:
        if (this.DisplayIndexMapInitialized)
        {
          this.UpdateDisplayIndexForMovedColumn(e.OldStartingIndex, e.NewStartingIndex);
          break;
        }
        break;
      case NotifyCollectionChangedAction.Reset:
        if (this.DisplayIndexMapInitialized)
        {
          this.DisplayIndexMap.Clear();
          this.DataGridOwner.UpdateColumnsOnVirtualizedCellInfoCollections(NotifyCollectionChangedAction.Reset, -1, (DataGridColumn) null, -1);
        }
        this.HasVisibleStarColumns = false;
        break;
    }
    base.OnCollectionChanged(e);
  }

  protected override void ClearItems()
  {
    this.ClearDisplayIndex((IList) this, (IList) null);
    this.DataGridOwner.UpdateDataGridReference((IList) this, true);
    base.ClearItems();
  }

  internal void NotifyPropertyChanged(
    DependencyObject d,
    string propertyName,
    DependencyPropertyChangedEventArgs e,
    NotificationTarget target)
  {
    if (DataGridHelper.ShouldNotifyColumnCollection(target))
    {
      if (e.Property == DataGridColumn.DisplayIndexProperty)
      {
        this.OnColumnDisplayIndexChanged((DataGridColumn) d, (int) e.OldValue, (int) e.NewValue);
        if (((DataGridColumn) d).IsVisible)
          this.InvalidateColumnRealization(true);
      }
      else if (e.Property == DataGridColumn.WidthProperty)
      {
        if (((DataGridColumn) d).IsVisible)
          this.InvalidateColumnRealization(false);
      }
      else if (e.Property == DataGrid.FrozenColumnCountProperty)
      {
        this.InvalidateColumnRealization(false);
        this.OnDataGridFrozenColumnCountChanged((int) e.OldValue, (int) e.NewValue);
      }
      else if (e.Property == DataGridColumn.VisibilityProperty)
      {
        this.InvalidateHasVisibleStarColumns();
        this.InvalidateColumnWidthsComputation();
        this.InvalidateColumnRealization(true);
      }
      else if (e.Property == DataGrid.EnableColumnVirtualizationProperty)
        this.InvalidateColumnRealization(true);
      else if (e.Property == DataGrid.CellsPanelHorizontalOffsetProperty)
        this.OnCellsPanelHorizontalOffsetChanged(e);
      else if (e.Property == DataGrid.HorizontalScrollOffsetProperty || string.Compare(propertyName, "ViewportWidth", StringComparison.Ordinal) == 0)
        this.InvalidateColumnRealization(false);
    }
    if (!DataGridHelper.ShouldNotifyColumns(target))
      return;
    int count = this.Count;
    for (int index = 0; index < count; ++index)
      this[index].NotifyPropertyChanged(d, e, NotificationTarget.Columns);
  }

  internal DataGridColumn ColumnFromDisplayIndex(int displayIndex)
  {
    return this[this.DisplayIndexMap[displayIndex]];
  }

  internal List<int> DisplayIndexMap
  {
    get
    {
      if (!this.DisplayIndexMapInitialized)
        this.InitializeDisplayIndexMap();
      return this._displayIndexMap;
    }
    private set => this._displayIndexMap = value;
  }

  private bool IsUpdatingDisplayIndex
  {
    get => this._isUpdatingDisplayIndex;
    set => this._isUpdatingDisplayIndex = value;
  }

  private int CoerceDefaultDisplayIndex(DataGridColumn column)
  {
    return this.CoerceDefaultDisplayIndex(column, this.IndexOf(column));
  }

  private int CoerceDefaultDisplayIndex(DataGridColumn column, int newDisplayIndex)
  {
    if (!DataGridHelper.IsDefaultValue((DependencyObject) column, DataGridColumn.DisplayIndexProperty))
      return column.DisplayIndex;
    bool updatingDisplayIndex = this.IsUpdatingDisplayIndex;
    try
    {
      this.IsUpdatingDisplayIndex = true;
      column.DisplayIndex = newDisplayIndex;
    }
    finally
    {
      this.IsUpdatingDisplayIndex = updatingDisplayIndex;
    }
    return newDisplayIndex;
  }

  private void OnColumnDisplayIndexChanged(
    DataGridColumn column,
    int oldDisplayIndex,
    int newDisplayIndex)
  {
    int num = oldDisplayIndex;
    if (!this._displayIndexMapInitialized)
      this.InitializeDisplayIndexMap(column, oldDisplayIndex, out oldDisplayIndex);
    if (this._isClearingDisplayIndex)
      return;
    newDisplayIndex = this.CoerceDefaultDisplayIndex(column);
    if (newDisplayIndex == oldDisplayIndex)
      return;
    if (num != -1)
      this.DataGridOwner.OnColumnDisplayIndexChanged(new DataGridColumnEventArgs(column));
    this.UpdateDisplayIndexForChangedColumn(oldDisplayIndex, newDisplayIndex);
  }

  private void UpdateDisplayIndexForChangedColumn(int oldDisplayIndex, int newDisplayIndex)
  {
    if (this.IsUpdatingDisplayIndex)
      return;
    try
    {
      this.IsUpdatingDisplayIndex = true;
      int displayIndex1 = this.DisplayIndexMap[oldDisplayIndex];
      this.DisplayIndexMap.RemoveAt(oldDisplayIndex);
      this.DisplayIndexMap.Insert(newDisplayIndex, displayIndex1);
      if (newDisplayIndex < oldDisplayIndex)
      {
        for (int displayIndex2 = newDisplayIndex + 1; displayIndex2 <= oldDisplayIndex; ++displayIndex2)
          ++this.ColumnFromDisplayIndex(displayIndex2).DisplayIndex;
      }
      else
      {
        for (int displayIndex3 = oldDisplayIndex; displayIndex3 < newDisplayIndex; ++displayIndex3)
          --this.ColumnFromDisplayIndex(displayIndex3).DisplayIndex;
      }
      this.DataGridOwner.UpdateColumnsOnVirtualizedCellInfoCollections(NotifyCollectionChangedAction.Move, oldDisplayIndex, (DataGridColumn) null, newDisplayIndex);
    }
    finally
    {
      this.IsUpdatingDisplayIndex = false;
    }
  }

  private void UpdateDisplayIndexForMovedColumn(int oldColumnIndex, int newColumnIndex)
  {
    this.InsertInDisplayIndexMap(this.RemoveFromDisplayIndexMap(oldColumnIndex), newColumnIndex);
    this.DataGridOwner.UpdateColumnsOnVirtualizedCellInfoCollections(NotifyCollectionChangedAction.Move, oldColumnIndex, (DataGridColumn) null, newColumnIndex);
  }

  private void UpdateDisplayIndexForNewColumns(IList newColumns, int startingIndex)
  {
    try
    {
      this.IsUpdatingDisplayIndex = true;
      DataGridColumn newColumn = (DataGridColumn) newColumns[0];
      int num = startingIndex;
      int newDisplayIndex = this.CoerceDefaultDisplayIndex(newColumn, num);
      this.InsertInDisplayIndexMap(newDisplayIndex, num);
      for (int displayIndex = 0; displayIndex < this.DisplayIndexMap.Count; ++displayIndex)
      {
        if (displayIndex > newDisplayIndex)
          ++this.ColumnFromDisplayIndex(displayIndex).DisplayIndex;
      }
      this.DataGridOwner.UpdateColumnsOnVirtualizedCellInfoCollections(NotifyCollectionChangedAction.Add, -1, (DataGridColumn) null, newDisplayIndex);
    }
    finally
    {
      this.IsUpdatingDisplayIndex = false;
    }
  }

  internal void InitializeDisplayIndexMap()
  {
    int resultDisplayIndex = -1;
    this.InitializeDisplayIndexMap((DataGridColumn) null, -1, out resultDisplayIndex);
  }

  private void InitializeDisplayIndexMap(
    DataGridColumn changingColumn,
    int oldDisplayIndex,
    out int resultDisplayIndex)
  {
    resultDisplayIndex = oldDisplayIndex;
    if (this._displayIndexMapInitialized)
      return;
    this._displayIndexMapInitialized = true;
    int count = this.Count;
    Dictionary<int, int> dictionary = new Dictionary<int, int>();
    if (changingColumn != null && oldDisplayIndex >= count)
      throw new ArgumentOutOfRangeException("displayIndex", (object) oldDisplayIndex, SR.Get(SRID.DataGrid_ColumnDisplayIndexOutOfRange, changingColumn.Header));
    for (int index = 0; index < count; ++index)
    {
      DataGridColumn column = this[index];
      int num = column.DisplayIndex;
      this.ValidateDisplayIndex(column, num);
      if (column == changingColumn)
        num = oldDisplayIndex;
      if (num >= 0)
      {
        if (dictionary.ContainsKey(num))
          throw new ArgumentException(SR.Get(SRID.DataGrid_DuplicateDisplayIndex));
        dictionary.Add(num, index);
      }
    }
    int num1 = 0;
    for (int index = 0; index < count; ++index)
    {
      DataGridColumn dataGridColumn = this[index];
      int displayIndex = dataGridColumn.DisplayIndex;
      bool flag = DataGridHelper.IsDefaultValue((DependencyObject) dataGridColumn, DataGridColumn.DisplayIndexProperty);
      if (dataGridColumn == changingColumn && oldDisplayIndex == -1)
        flag = true;
      if (flag)
      {
        while (dictionary.ContainsKey(num1))
          ++num1;
        this.CoerceDefaultDisplayIndex(dataGridColumn, num1);
        dictionary.Add(num1, index);
        if (dataGridColumn == changingColumn)
          resultDisplayIndex = num1;
        ++num1;
      }
    }
    for (int key = 0; key < count; ++key)
      this.DisplayIndexMap.Add(dictionary[key]);
  }

  private void UpdateDisplayIndexForRemovedColumns(IList oldColumns, int startingIndex)
  {
    try
    {
      this.IsUpdatingDisplayIndex = true;
      int oldDisplayIndex = this.RemoveFromDisplayIndexMap(startingIndex);
      for (int displayIndex = 0; displayIndex < this.DisplayIndexMap.Count; ++displayIndex)
      {
        if (displayIndex >= oldDisplayIndex)
          --this.ColumnFromDisplayIndex(displayIndex).DisplayIndex;
      }
      this.DataGridOwner.UpdateColumnsOnVirtualizedCellInfoCollections(NotifyCollectionChangedAction.Remove, oldDisplayIndex, (DataGridColumn) oldColumns[0], -1);
    }
    finally
    {
      this.IsUpdatingDisplayIndex = false;
    }
  }

  private void UpdateDisplayIndexForReplacedColumn(IList oldColumns, IList newColumns)
  {
    if (oldColumns == null || oldColumns.Count <= 0 || newColumns == null || newColumns.Count <= 0)
      return;
    DataGridColumn oldColumn = (DataGridColumn) oldColumns[0];
    DataGridColumn newColumn = (DataGridColumn) newColumns[0];
    if (oldColumn == null || newColumn == null)
      return;
    int num = this.CoerceDefaultDisplayIndex(newColumn);
    if (oldColumn.DisplayIndex != num)
      this.UpdateDisplayIndexForChangedColumn(oldColumn.DisplayIndex, num);
    this.DataGridOwner.UpdateColumnsOnVirtualizedCellInfoCollections(NotifyCollectionChangedAction.Replace, num, oldColumn, num);
  }

  private void ClearDisplayIndex(IList oldColumns, IList newColumns)
  {
    if (oldColumns == null)
      return;
    try
    {
      this._isClearingDisplayIndex = true;
      int count = oldColumns.Count;
      for (int index = 0; index < count; ++index)
      {
        DataGridColumn oldColumn = (DataGridColumn) oldColumns[index];
        if (newColumns == null || !newColumns.Contains((object) oldColumn))
          oldColumn.ClearValue(DataGridColumn.DisplayIndexProperty);
      }
    }
    finally
    {
      this._isClearingDisplayIndex = false;
    }
  }

  private bool IsDisplayIndexValid(DataGridColumn column, int displayIndex, bool isAdding)
  {
    if (displayIndex == -1 && DataGridHelper.IsDefaultValue((DependencyObject) column, DataGridColumn.DisplayIndexProperty))
      return true;
    if (displayIndex < 0)
      return false;
    return !isAdding ? displayIndex < this.Count : displayIndex <= this.Count;
  }

  private void InsertInDisplayIndexMap(int newDisplayIndex, int columnIndex)
  {
    this.DisplayIndexMap.Insert(newDisplayIndex, columnIndex);
    for (int index1 = 0; index1 < this.DisplayIndexMap.Count; ++index1)
    {
      if (this.DisplayIndexMap[index1] >= columnIndex && index1 != newDisplayIndex)
      {
        List<int> displayIndexMap;
        int index2;
        (displayIndexMap = this.DisplayIndexMap)[index2 = index1] = displayIndexMap[index2] + 1;
      }
    }
  }

  private int RemoveFromDisplayIndexMap(int columnIndex)
  {
    int index1 = this.DisplayIndexMap.IndexOf(columnIndex);
    this.DisplayIndexMap.RemoveAt(index1);
    for (int index2 = 0; index2 < this.DisplayIndexMap.Count; ++index2)
    {
      if (this.DisplayIndexMap[index2] >= columnIndex)
      {
        List<int> displayIndexMap;
        int index3;
        (displayIndexMap = this.DisplayIndexMap)[index3 = index2] = displayIndexMap[index3] - 1;
      }
    }
    return index1;
  }

  internal void ValidateDisplayIndex(DataGridColumn column, int displayIndex)
  {
    this.ValidateDisplayIndex(column, displayIndex, false);
  }

  internal void ValidateDisplayIndex(DataGridColumn column, int displayIndex, bool isAdding)
  {
    if (!this.IsDisplayIndexValid(column, displayIndex, isAdding))
      throw new ArgumentOutOfRangeException(nameof (displayIndex), (object) displayIndex, SR.Get(SRID.DataGrid_ColumnDisplayIndexOutOfRange, column.Header));
  }

  [Conditional("DEBUG")]
  private void Debug_VerifyDisplayIndexMap()
  {
    int num = 0;
    while (num < this.DisplayIndexMap.Count)
      ++num;
  }

  private void OnDataGridFrozenColumnCountChanged(int oldFrozenCount, int newFrozenCount)
  {
    if (newFrozenCount > oldFrozenCount)
    {
      int num = Math.Min(newFrozenCount, this.Count);
      for (int displayIndex = oldFrozenCount; displayIndex < num; ++displayIndex)
        this.ColumnFromDisplayIndex(displayIndex).IsFrozen = true;
    }
    else
    {
      int num = Math.Min(oldFrozenCount, this.Count);
      for (int displayIndex = newFrozenCount; displayIndex < num; ++displayIndex)
        this.ColumnFromDisplayIndex(displayIndex).IsFrozen = false;
    }
  }

  private DataGrid DataGridOwner => this._dataGridOwner;

  internal bool DisplayIndexMapInitialized => this._displayIndexMapInitialized;

  private bool HasVisibleStarColumnsInternal(DataGridColumn ignoredColumn, out double perStarWidth)
  {
    bool flag = false;
    perStarWidth = 0.0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (dataGridColumn != ignoredColumn && dataGridColumn.IsVisible)
      {
        DataGridLength width = dataGridColumn.Width;
        if (width.IsStar)
        {
          flag = true;
          if (!DoubleUtil.AreClose(width.Value, 0.0) && !DoubleUtil.AreClose(width.DesiredValue, 0.0))
          {
            perStarWidth = width.DesiredValue / width.Value;
            break;
          }
        }
      }
    }
    return flag;
  }

  private bool HasVisibleStarColumnsInternal(out double perStarWidth)
  {
    return this.HasVisibleStarColumnsInternal((DataGridColumn) null, out perStarWidth);
  }

  private bool HasVisibleStarColumnsInternal(DataGridColumn ignoredColumn)
  {
    return this.HasVisibleStarColumnsInternal(ignoredColumn, out double _);
  }

  internal bool HasVisibleStarColumns { get; private set; }

  internal void InvalidateHasVisibleStarColumns()
  {
    this.HasVisibleStarColumns = this.HasVisibleStarColumnsInternal((DataGridColumn) null);
  }

  private void RecomputeStarColumnWidths()
  {
    double viewportWidthForColumns = this.DataGridOwner.GetViewportWidthForColumns();
    double num = 0.0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      DataGridLength width = dataGridColumn.Width;
      if (dataGridColumn.IsVisible && !width.IsStar)
        num += width.DisplayValue;
    }
    if (DoubleUtil.IsNaN(num))
      return;
    this.ComputeStarColumnWidths(viewportWidthForColumns - num);
  }

  private double ComputeStarColumnWidths(double availableStarSpace)
  {
    List<DataGridColumn> dataGridColumnList1 = new List<DataGridColumn>();
    List<DataGridColumn> dataGridColumnList2 = new List<DataGridColumn>();
    double num1 = 0.0;
    double num2 = 0.0;
    double num3 = 0.0;
    double starColumnWidths = 0.0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      DataGridLength width = dataGridColumn.Width;
      if (dataGridColumn.IsVisible && width.IsStar)
      {
        dataGridColumnList1.Add(dataGridColumn);
        num1 += width.Value;
        num2 += dataGridColumn.MinWidth;
        num3 += dataGridColumn.MaxWidth;
      }
    }
    if (DoubleUtil.LessThan(availableStarSpace, num2))
      availableStarSpace = num2;
    if (DoubleUtil.GreaterThan(availableStarSpace, num3))
      availableStarSpace = num3;
    while (dataGridColumnList1.Count > 0)
    {
      double num4 = availableStarSpace / num1;
      int index1 = 0;
      for (int count = dataGridColumnList1.Count; index1 < count; ++index1)
      {
        DataGridColumn dataGridColumn = dataGridColumnList1[index1];
        DataGridLength width = dataGridColumn.Width;
        double minWidth = dataGridColumn.MinWidth;
        double num5 = availableStarSpace * width.Value / num1;
        if (DoubleUtil.GreaterThan(minWidth, num5))
        {
          availableStarSpace = Math.Max(0.0, availableStarSpace - minWidth);
          num1 -= width.Value;
          dataGridColumnList1.RemoveAt(index1);
          --index1;
          --count;
          dataGridColumnList2.Add(dataGridColumn);
        }
      }
      bool flag = false;
      int index2 = 0;
      for (int count = dataGridColumnList1.Count; index2 < count; ++index2)
      {
        DataGridColumn dataGridColumn = dataGridColumnList1[index2];
        DataGridLength width = dataGridColumn.Width;
        double maxWidth = dataGridColumn.MaxWidth;
        double num6 = availableStarSpace * width.Value / num1;
        if (DoubleUtil.LessThan(maxWidth, num6))
        {
          flag = true;
          dataGridColumnList1.RemoveAt(index2);
          availableStarSpace -= maxWidth;
          starColumnWidths += maxWidth;
          num1 -= width.Value;
          dataGridColumn.UpdateWidthForStarColumn(maxWidth, num4 * width.Value, width.Value);
          break;
        }
      }
      if (flag)
      {
        int index3 = 0;
        for (int count = dataGridColumnList2.Count; index3 < count; ++index3)
        {
          DataGridColumn dataGridColumn = dataGridColumnList2[index3];
          dataGridColumnList1.Add(dataGridColumn);
          availableStarSpace += dataGridColumn.MinWidth;
          num1 += dataGridColumn.Width.Value;
        }
        dataGridColumnList2.Clear();
      }
      else
      {
        int index4 = 0;
        for (int count = dataGridColumnList2.Count; index4 < count; ++index4)
        {
          DataGridColumn dataGridColumn = dataGridColumnList2[index4];
          DataGridLength width = dataGridColumn.Width;
          double minWidth = dataGridColumn.MinWidth;
          dataGridColumn.UpdateWidthForStarColumn(minWidth, width.Value * num4, width.Value);
          starColumnWidths += minWidth;
        }
        dataGridColumnList2.Clear();
        int index5 = 0;
        for (int count = dataGridColumnList1.Count; index5 < count; ++index5)
        {
          DataGridColumn dataGridColumn = dataGridColumnList1[index5];
          DataGridLength width = dataGridColumn.Width;
          double displayWidth = availableStarSpace * width.Value / num1;
          dataGridColumn.UpdateWidthForStarColumn(displayWidth, width.Value * num4, width.Value);
          starColumnWidths += displayWidth;
        }
        dataGridColumnList1.Clear();
      }
    }
    return starColumnWidths;
  }

  private void OnCellsPanelHorizontalOffsetChanged(DependencyPropertyChangedEventArgs e)
  {
    this.InvalidateColumnRealization(false);
    double viewportWidthForColumns = this.DataGridOwner.GetViewportWidthForColumns();
    this.RedistributeColumnWidthsOnAvailableSpaceChange((double) e.OldValue - (double) e.NewValue, viewportWidthForColumns);
  }

  internal void InvalidateAverageColumnWidth() => this._averageColumnWidth = new double?();

  internal double AverageColumnWidth
  {
    get
    {
      if (!this._averageColumnWidth.HasValue)
        this._averageColumnWidth = new double?(this.ComputeAverageColumnWidth());
      return this._averageColumnWidth.Value;
    }
  }

  private double ComputeAverageColumnWidth()
  {
    double num1 = 0.0;
    int num2 = 0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      DataGridLength width = dataGridColumn.Width;
      if (dataGridColumn.IsVisible && !DoubleUtil.IsNaN(width.DisplayValue))
      {
        num1 += width.DisplayValue;
        ++num2;
      }
    }
    return num2 != 0 ? num1 / (double) num2 : 0.0;
  }

  internal bool ColumnWidthsComputationPending => this._columnWidthsComputationPending;

  internal void InvalidateColumnWidthsComputation()
  {
    if (this._columnWidthsComputationPending)
      return;
    this.DataGridOwner.Dispatcher.BeginInvoke((Delegate) new DispatcherOperationCallback(this.ComputeColumnWidths), DispatcherPriority.Render, (object) this);
    this._columnWidthsComputationPending = true;
  }

  private object ComputeColumnWidths(object arg)
  {
    this.ComputeColumnWidths();
    this.DataGridOwner.NotifyPropertyChanged((DependencyObject) this.DataGridOwner, "DelayedColumnWidthComputation", new DependencyPropertyChangedEventArgs(), NotificationTarget.CellsPresenter | NotificationTarget.ColumnHeadersPresenter);
    return (object) null;
  }

  private void ComputeColumnWidths()
  {
    if (this.HasVisibleStarColumns)
    {
      this.InitializeColumnDisplayValues();
      this.DistributeSpaceAmongColumns(this.DataGridOwner.GetViewportWidthForColumns());
    }
    else
      this.ExpandAllColumnWidthsToDesiredValue();
    if (this.RefreshAutoWidthColumns)
    {
      foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
      {
        if (dataGridColumn.Width.IsAuto)
          dataGridColumn.Width = DataGridLength.Auto;
      }
      this.RefreshAutoWidthColumns = false;
    }
    this._columnWidthsComputationPending = false;
  }

  private void InitializeColumnDisplayValues()
  {
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (dataGridColumn.IsVisible)
      {
        DataGridLength width = dataGridColumn.Width;
        if (!width.IsStar)
        {
          double minWidth = dataGridColumn.MinWidth;
          double minMax = DataGridHelper.CoerceToMinMax(DoubleUtil.IsNaN(width.DesiredValue) ? minWidth : width.DesiredValue, minWidth, dataGridColumn.MaxWidth);
          if (!DoubleUtil.AreClose(width.DisplayValue, minMax))
            dataGridColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, minMax));
        }
      }
    }
  }

  internal void RedistributeColumnWidthsOnMinWidthChangeOfColumn(
    DataGridColumn changedColumn,
    double oldMinWidth)
  {
    DataGridLength width = changedColumn.Width;
    double minWidth = changedColumn.MinWidth;
    if (DoubleUtil.GreaterThan(minWidth, width.DisplayValue))
    {
      if (this.HasVisibleStarColumns)
        this.TakeAwayWidthFromColumns(changedColumn, minWidth - width.DisplayValue, false);
      changedColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, minWidth));
    }
    else
    {
      if (!DoubleUtil.LessThan(minWidth, oldMinWidth))
        return;
      if (width.IsStar)
      {
        if (!DoubleUtil.AreClose(width.DisplayValue, oldMinWidth))
          return;
        this.GiveAwayWidthToColumns(changedColumn, oldMinWidth - minWidth, true);
      }
      else
      {
        if (!DoubleUtil.GreaterThan(oldMinWidth, width.DesiredValue))
          return;
        double displayValue = Math.Max(width.DesiredValue, minWidth);
        if (this.HasVisibleStarColumns)
          this.GiveAwayWidthToColumns(changedColumn, oldMinWidth - displayValue);
        changedColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, displayValue));
      }
    }
  }

  internal void RedistributeColumnWidthsOnMaxWidthChangeOfColumn(
    DataGridColumn changedColumn,
    double oldMaxWidth)
  {
    DataGridLength width = changedColumn.Width;
    double maxWidth = changedColumn.MaxWidth;
    if (DoubleUtil.LessThan(maxWidth, width.DisplayValue))
    {
      if (this.HasVisibleStarColumns)
        this.GiveAwayWidthToColumns(changedColumn, width.DisplayValue - maxWidth);
      changedColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, maxWidth));
    }
    else
    {
      if (!DoubleUtil.GreaterThan(maxWidth, oldMaxWidth))
        return;
      if (width.IsStar)
      {
        this.RecomputeStarColumnWidths();
      }
      else
      {
        if (!DoubleUtil.LessThan(oldMaxWidth, width.DesiredValue))
          return;
        double displayValue = Math.Min(width.DesiredValue, maxWidth);
        if (this.HasVisibleStarColumns)
        {
          double widthFromUnusedSpace = this.TakeAwayWidthFromUnusedSpace(false, displayValue - oldMaxWidth);
          double widthFromStarColumns = this.TakeAwayWidthFromStarColumns(changedColumn, widthFromUnusedSpace);
          displayValue -= widthFromStarColumns;
        }
        changedColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, displayValue));
      }
    }
  }

  internal void RedistributeColumnWidthsOnWidthChangeOfColumn(
    DataGridColumn changedColumn,
    DataGridLength oldWidth)
  {
    DataGridLength width = changedColumn.Width;
    bool visibleStarColumns = this.HasVisibleStarColumns;
    if (oldWidth.IsStar && !width.IsStar && !visibleStarColumns)
      this.ExpandAllColumnWidthsToDesiredValue();
    else if (width.IsStar && !oldWidth.IsStar)
    {
      if (!this.HasVisibleStarColumnsInternal(changedColumn))
      {
        this.ComputeColumnWidths();
      }
      else
      {
        double minWidth = changedColumn.MinWidth;
        double nonStarColumns = this.GiveAwayWidthToNonStarColumns((DataGridColumn) null, oldWidth.DisplayValue - minWidth);
        changedColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, minWidth + nonStarColumns));
        this.RecomputeStarColumnWidths();
      }
    }
    else if (width.IsStar && oldWidth.IsStar)
    {
      this.RecomputeStarColumnWidths();
    }
    else
    {
      if (!visibleStarColumns)
        return;
      this.RedistributeColumnWidthsOnNonStarWidthChange(changedColumn, oldWidth);
    }
  }

  internal void RedistributeColumnWidthsOnAvailableSpaceChange(
    double availableSpaceChange,
    double newTotalAvailableSpace)
  {
    if (this.ColumnWidthsComputationPending || !this.HasVisibleStarColumns)
      return;
    if (DoubleUtil.GreaterThan(availableSpaceChange, 0.0))
    {
      this.GiveAwayWidthToColumns((DataGridColumn) null, availableSpaceChange);
    }
    else
    {
      if (!DoubleUtil.LessThan(availableSpaceChange, 0.0))
        return;
      this.TakeAwayWidthFromColumns((DataGridColumn) null, Math.Abs(availableSpaceChange), false, newTotalAvailableSpace);
    }
  }

  private void ExpandAllColumnWidthsToDesiredValue()
  {
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (dataGridColumn.IsVisible)
      {
        DataGridLength width = dataGridColumn.Width;
        double maxWidth = dataGridColumn.MaxWidth;
        if (DoubleUtil.GreaterThan(width.DesiredValue, width.DisplayValue) && !DoubleUtil.AreClose(width.DisplayValue, maxWidth))
          dataGridColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, Math.Min(width.DesiredValue, maxWidth)));
      }
    }
  }

  private void RedistributeColumnWidthsOnNonStarWidthChange(
    DataGridColumn changedColumn,
    DataGridLength oldWidth)
  {
    DataGridLength width = changedColumn.Width;
    if (DoubleUtil.GreaterThan(width.DesiredValue, oldWidth.DisplayValue))
    {
      double widthFromColumns = this.TakeAwayWidthFromColumns(changedColumn, width.DesiredValue - oldWidth.DisplayValue, changedColumn != null);
      if (!DoubleUtil.GreaterThan(widthFromColumns, 0.0))
        return;
      changedColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, Math.Max(width.DisplayValue - widthFromColumns, changedColumn.MinWidth)));
    }
    else
    {
      if (!DoubleUtil.LessThan(width.DesiredValue, oldWidth.DisplayValue))
        return;
      double minMax = DataGridHelper.CoerceToMinMax(width.DesiredValue, changedColumn.MinWidth, changedColumn.MaxWidth);
      this.GiveAwayWidthToColumns(changedColumn, oldWidth.DisplayValue - minMax);
    }
  }

  private void DistributeSpaceAmongColumns(double availableSpace)
  {
    double num1 = 0.0;
    double num2 = 0.0;
    double num3 = 0.0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (dataGridColumn.IsVisible)
      {
        num1 += dataGridColumn.MinWidth;
        num2 += dataGridColumn.MaxWidth;
        if (dataGridColumn.Width.IsStar)
          num3 += dataGridColumn.MinWidth;
      }
    }
    if (DoubleUtil.LessThan(availableSpace, num1))
      availableSpace = num1;
    if (DoubleUtil.GreaterThan(availableSpace, num2))
      availableSpace = num2;
    double num4 = this.DistributeSpaceAmongNonStarColumns(availableSpace - num3);
    this.ComputeStarColumnWidths(num3 + num4);
  }

  private double DistributeSpaceAmongNonStarColumns(double availableSpace)
  {
    double num = 0.0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      DataGridLength width = dataGridColumn.Width;
      if (dataGridColumn.IsVisible && !width.IsStar)
        num += width.DisplayValue;
    }
    if (DoubleUtil.LessThan(availableSpace, num))
      this.TakeAwayWidthFromNonStarColumns((DataGridColumn) null, num - availableSpace);
    return Math.Max(availableSpace - num, 0.0);
  }

  internal void OnColumnResizeStarted()
  {
    this._originalWidthsForResize = new Dictionary<DataGridColumn, DataGridLength>();
    foreach (DataGridColumn key in (Collection<DataGridColumn>) this)
      this._originalWidthsForResize[key] = key.Width;
  }

  internal void OnColumnResizeCompleted(bool cancel)
  {
    if (cancel && this._originalWidthsForResize != null)
    {
      foreach (DataGridColumn key in (Collection<DataGridColumn>) this)
      {
        if (this._originalWidthsForResize.ContainsKey(key))
          key.Width = this._originalWidthsForResize[key];
      }
    }
    this._originalWidthsForResize = (Dictionary<DataGridColumn, DataGridLength>) null;
  }

  internal void RecomputeColumnWidthsOnColumnResize(
    DataGridColumn resizingColumn,
    double horizontalChange,
    bool retainAuto)
  {
    DataGridLength width = resizingColumn.Width;
    double num = width.DisplayValue + horizontalChange;
    if (DoubleUtil.LessThan(num, resizingColumn.MinWidth))
      horizontalChange = resizingColumn.MinWidth - width.DisplayValue;
    else if (DoubleUtil.GreaterThan(num, resizingColumn.MaxWidth))
      horizontalChange = resizingColumn.MaxWidth - width.DisplayValue;
    int displayIndex = resizingColumn.DisplayIndex;
    if (DoubleUtil.GreaterThan(horizontalChange, 0.0))
    {
      this.RecomputeColumnWidthsOnColumnPositiveResize(horizontalChange, displayIndex, retainAuto);
    }
    else
    {
      if (!DoubleUtil.LessThan(horizontalChange, 0.0))
        return;
      this.RecomputeColumnWidthsOnColumnNegativeResize(-horizontalChange, displayIndex, retainAuto);
    }
  }

  private void RecomputeColumnWidthsOnColumnPositiveResize(
    double horizontalChange,
    int resizingColumnIndex,
    bool retainAuto)
  {
    double perStarWidth = 0.0;
    if (this.HasVisibleStarColumnsInternal(out perStarWidth))
    {
      horizontalChange = this.TakeAwayUnusedSpaceOnColumnPositiveResize(horizontalChange, resizingColumnIndex, retainAuto);
      horizontalChange = this.RecomputeNonStarColumnWidthsOnColumnPositiveResize(horizontalChange, resizingColumnIndex, retainAuto, true);
      horizontalChange = this.RecomputeStarColumnWidthsOnColumnPositiveResize(horizontalChange, resizingColumnIndex, perStarWidth, retainAuto);
      horizontalChange = this.RecomputeNonStarColumnWidthsOnColumnPositiveResize(horizontalChange, resizingColumnIndex, retainAuto, false);
    }
    else
      DataGridColumnCollection.SetResizedColumnWidth(this.ColumnFromDisplayIndex(resizingColumnIndex), horizontalChange, retainAuto);
  }

  private double RecomputeStarColumnWidthsOnColumnPositiveResize(
    double horizontalChange,
    int resizingColumnIndex,
    double perStarWidth,
    bool retainAuto)
  {
    while (DoubleUtil.GreaterThan(horizontalChange, 0.0))
    {
      double minPerStarExcessRatio = double.PositiveInfinity;
      double forPositiveResize = this.GetStarFactorsForPositiveResize(resizingColumnIndex + 1, out minPerStarExcessRatio);
      if (DoubleUtil.GreaterThan(forPositiveResize, 0.0))
      {
        horizontalChange = this.ReallocateStarValuesForPositiveResize(resizingColumnIndex, horizontalChange, minPerStarExcessRatio, forPositiveResize, perStarWidth, retainAuto);
        if (DoubleUtil.AreClose(horizontalChange, 0.0))
          break;
      }
      else
        break;
    }
    return horizontalChange;
  }

  private static bool CanColumnParticipateInResize(DataGridColumn column)
  {
    return column.IsVisible && column.CanUserResize;
  }

  private double GetStarFactorsForPositiveResize(int startIndex, out double minPerStarExcessRatio)
  {
    minPerStarExcessRatio = double.PositiveInfinity;
    double forPositiveResize = 0.0;
    int displayIndex = startIndex;
    for (int count = this.Count; displayIndex < count; ++displayIndex)
    {
      DataGridColumn column = this.ColumnFromDisplayIndex(displayIndex);
      if (DataGridColumnCollection.CanColumnParticipateInResize(column))
      {
        DataGridLength width = column.Width;
        if (width.IsStar && !DoubleUtil.AreClose(width.Value, 0.0) && DoubleUtil.GreaterThan(width.DisplayValue, column.MinWidth))
        {
          forPositiveResize += width.Value;
          double num = (width.DisplayValue - column.MinWidth) / width.Value;
          if (DoubleUtil.LessThan(num, minPerStarExcessRatio))
            minPerStarExcessRatio = num;
        }
      }
    }
    return forPositiveResize;
  }

  private double ReallocateStarValuesForPositiveResize(
    int startIndex,
    double horizontalChange,
    double perStarExcessRatio,
    double totalStarFactors,
    double perStarWidth,
    bool retainAuto)
  {
    double num1;
    double widthDelta;
    if (DoubleUtil.LessThan(horizontalChange, perStarExcessRatio * totalStarFactors))
    {
      num1 = horizontalChange / totalStarFactors;
      widthDelta = horizontalChange;
      horizontalChange = 0.0;
    }
    else
    {
      num1 = perStarExcessRatio;
      widthDelta = num1 * totalStarFactors;
      horizontalChange -= widthDelta;
    }
    int displayIndex = startIndex;
    for (int count = this.Count; displayIndex < count; ++displayIndex)
    {
      DataGridColumn column = this.ColumnFromDisplayIndex(displayIndex);
      DataGridLength width = column.Width;
      if (displayIndex == startIndex)
        DataGridColumnCollection.SetResizedColumnWidth(column, widthDelta, retainAuto);
      else if (column.Width.IsStar && DataGridColumnCollection.CanColumnParticipateInResize(column) && DoubleUtil.GreaterThan(width.DisplayValue, column.MinWidth))
      {
        double num2 = width.DisplayValue - width.Value * num1;
        column.UpdateWidthForStarColumn(Math.Max(num2, column.MinWidth), num2, num2 / perStarWidth);
      }
    }
    return horizontalChange;
  }

  private double RecomputeNonStarColumnWidthsOnColumnPositiveResize(
    double horizontalChange,
    int resizingColumnIndex,
    bool retainAuto,
    bool onlyShrinkToDesiredWidth)
  {
    if (DoubleUtil.GreaterThan(horizontalChange, 0.0))
    {
      double widthDelta = 0.0;
      bool flag = true;
      for (int displayIndex = this.Count - 1; flag && displayIndex > resizingColumnIndex; --displayIndex)
      {
        DataGridColumn column = this.ColumnFromDisplayIndex(displayIndex);
        if (DataGridColumnCollection.CanColumnParticipateInResize(column))
        {
          DataGridLength width = column.Width;
          double minWidth = column.MinWidth;
          double num = onlyShrinkToDesiredWidth ? width.DisplayValue - Math.Max(width.DesiredValue, column.MinWidth) : width.DisplayValue - column.MinWidth;
          if (!width.IsStar && DoubleUtil.GreaterThan(num, 0.0))
          {
            if (DoubleUtil.GreaterThanOrClose(widthDelta + num, horizontalChange))
            {
              num = horizontalChange - widthDelta;
              flag = false;
            }
            column.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, width.DisplayValue - num));
            widthDelta += num;
          }
        }
      }
      if (DoubleUtil.GreaterThan(widthDelta, 0.0))
      {
        DataGridColumnCollection.SetResizedColumnWidth(this.ColumnFromDisplayIndex(resizingColumnIndex), widthDelta, retainAuto);
        horizontalChange -= widthDelta;
      }
    }
    return horizontalChange;
  }

  private void RecomputeColumnWidthsOnColumnNegativeResize(
    double horizontalChange,
    int resizingColumnIndex,
    bool retainAuto)
  {
    double perStarWidth = 0.0;
    if (this.HasVisibleStarColumnsInternal(out perStarWidth))
    {
      horizontalChange = this.RecomputeNonStarColumnWidthsOnColumnNegativeResize(horizontalChange, resizingColumnIndex, retainAuto, false);
      horizontalChange = this.RecomputeStarColumnWidthsOnColumnNegativeResize(horizontalChange, resizingColumnIndex, perStarWidth, retainAuto);
      horizontalChange = this.RecomputeNonStarColumnWidthsOnColumnNegativeResize(horizontalChange, resizingColumnIndex, retainAuto, true);
      if (!DoubleUtil.GreaterThan(horizontalChange, 0.0))
        return;
      DataGridColumn column = this.ColumnFromDisplayIndex(resizingColumnIndex);
      if (column.Width.IsStar)
        return;
      DataGridColumnCollection.SetResizedColumnWidth(column, -horizontalChange, retainAuto);
    }
    else
      DataGridColumnCollection.SetResizedColumnWidth(this.ColumnFromDisplayIndex(resizingColumnIndex), -horizontalChange, retainAuto);
  }

  private double RecomputeNonStarColumnWidthsOnColumnNegativeResize(
    double horizontalChange,
    int resizingColumnIndex,
    bool retainAuto,
    bool expandBeyondDesiredWidth)
  {
    if (DoubleUtil.GreaterThan(horizontalChange, 0.0))
    {
      double num1 = 0.0;
      bool flag = true;
      int displayIndex = resizingColumnIndex + 1;
      for (int count = this.Count; flag && displayIndex < count; ++displayIndex)
      {
        DataGridColumn column = this.ColumnFromDisplayIndex(displayIndex);
        if (DataGridColumnCollection.CanColumnParticipateInResize(column))
        {
          DataGridLength width = column.Width;
          double num2 = expandBeyondDesiredWidth ? column.MaxWidth : Math.Min(width.DesiredValue, column.MaxWidth);
          if (!width.IsStar && DoubleUtil.LessThan(width.DisplayValue, num2))
          {
            double num3 = num2 - width.DisplayValue;
            if (DoubleUtil.GreaterThanOrClose(num1 + num3, horizontalChange))
            {
              num3 = horizontalChange - num1;
              flag = false;
            }
            column.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, width.DisplayValue + num3));
            num1 += num3;
          }
        }
      }
      if (DoubleUtil.GreaterThan(num1, 0.0))
      {
        DataGridColumnCollection.SetResizedColumnWidth(this.ColumnFromDisplayIndex(resizingColumnIndex), -num1, retainAuto);
        horizontalChange -= num1;
      }
    }
    return horizontalChange;
  }

  private double RecomputeStarColumnWidthsOnColumnNegativeResize(
    double horizontalChange,
    int resizingColumnIndex,
    double perStarWidth,
    bool retainAuto)
  {
    while (DoubleUtil.GreaterThan(horizontalChange, 0.0))
    {
      double minPerStarLagRatio = double.PositiveInfinity;
      double forNegativeResize = this.GetStarFactorsForNegativeResize(resizingColumnIndex + 1, out minPerStarLagRatio);
      if (DoubleUtil.GreaterThan(forNegativeResize, 0.0))
      {
        horizontalChange = this.ReallocateStarValuesForNegativeResize(resizingColumnIndex, horizontalChange, minPerStarLagRatio, forNegativeResize, perStarWidth, retainAuto);
        if (DoubleUtil.AreClose(horizontalChange, 0.0))
          break;
      }
      else
        break;
    }
    return horizontalChange;
  }

  private double GetStarFactorsForNegativeResize(int startIndex, out double minPerStarLagRatio)
  {
    minPerStarLagRatio = double.PositiveInfinity;
    double forNegativeResize = 0.0;
    int displayIndex = startIndex;
    for (int count = this.Count; displayIndex < count; ++displayIndex)
    {
      DataGridColumn column = this.ColumnFromDisplayIndex(displayIndex);
      if (DataGridColumnCollection.CanColumnParticipateInResize(column))
      {
        DataGridLength width = column.Width;
        if (width.IsStar && !DoubleUtil.AreClose(width.Value, 0.0) && DoubleUtil.LessThan(width.DisplayValue, column.MaxWidth))
        {
          forNegativeResize += width.Value;
          double num = (column.MaxWidth - width.DisplayValue) / width.Value;
          if (DoubleUtil.LessThan(num, minPerStarLagRatio))
            minPerStarLagRatio = num;
        }
      }
    }
    return forNegativeResize;
  }

  private double ReallocateStarValuesForNegativeResize(
    int startIndex,
    double horizontalChange,
    double perStarLagRatio,
    double totalStarFactors,
    double perStarWidth,
    bool retainAuto)
  {
    double num1;
    double num2;
    if (DoubleUtil.LessThan(horizontalChange, perStarLagRatio * totalStarFactors))
    {
      num1 = horizontalChange / totalStarFactors;
      num2 = horizontalChange;
      horizontalChange = 0.0;
    }
    else
    {
      num1 = perStarLagRatio;
      num2 = num1 * totalStarFactors;
      horizontalChange -= num2;
    }
    int displayIndex = startIndex;
    for (int count = this.Count; displayIndex < count; ++displayIndex)
    {
      DataGridColumn column = this.ColumnFromDisplayIndex(displayIndex);
      DataGridLength width = column.Width;
      if (displayIndex == startIndex)
        DataGridColumnCollection.SetResizedColumnWidth(column, -num2, retainAuto);
      else if (column.Width.IsStar && DataGridColumnCollection.CanColumnParticipateInResize(column) && DoubleUtil.LessThan(width.DisplayValue, column.MaxWidth))
      {
        double num3 = width.DisplayValue + width.Value * num1;
        column.UpdateWidthForStarColumn(Math.Min(num3, column.MaxWidth), num3, num3 / perStarWidth);
      }
    }
    return horizontalChange;
  }

  private static void SetResizedColumnWidth(
    DataGridColumn column,
    double widthDelta,
    bool retainAuto)
  {
    DataGridLength width = column.Width;
    double minMax = DataGridHelper.CoerceToMinMax(width.DisplayValue + widthDelta, column.MinWidth, column.MaxWidth);
    if (width.IsStar)
    {
      double num = width.DesiredValue / width.Value;
      column.UpdateWidthForStarColumn(minMax, minMax, minMax / num);
    }
    else if (!width.IsAbsolute && retainAuto)
      column.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, minMax));
    else
      column.SetWidthInternal(new DataGridLength(minMax, DataGridLengthUnitType.Pixel, minMax, minMax));
  }

  private double GiveAwayWidthToColumns(DataGridColumn ignoredColumn, double giveAwayWidth)
  {
    return this.GiveAwayWidthToColumns(ignoredColumn, giveAwayWidth, false);
  }

  private double GiveAwayWidthToColumns(
    DataGridColumn ignoredColumn,
    double giveAwayWidth,
    bool recomputeStars)
  {
    double num1 = giveAwayWidth;
    giveAwayWidth = this.GiveAwayWidthToScrollViewerExcess(giveAwayWidth, ignoredColumn != null);
    giveAwayWidth = this.GiveAwayWidthToNonStarColumns(ignoredColumn, giveAwayWidth);
    if (DoubleUtil.GreaterThan(giveAwayWidth, 0.0) || recomputeStars)
    {
      double num2 = 0.0;
      double val2 = 0.0;
      bool flag = false;
      foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
      {
        DataGridLength width = dataGridColumn.Width;
        if (width.IsStar && dataGridColumn.IsVisible)
        {
          if (dataGridColumn == ignoredColumn)
            flag = true;
          num2 += width.DisplayValue;
          val2 += dataGridColumn.MaxWidth;
        }
      }
      double val1 = num2;
      if (!flag)
        val1 += giveAwayWidth;
      else if (!DoubleUtil.AreClose(num1, giveAwayWidth))
        val1 -= num1 - giveAwayWidth;
      giveAwayWidth = Math.Max(this.ComputeStarColumnWidths(Math.Min(val1, val2)) - val1, 0.0);
    }
    return giveAwayWidth;
  }

  private double GiveAwayWidthToNonStarColumns(DataGridColumn ignoredColumn, double giveAwayWidth)
  {
    while (DoubleUtil.GreaterThan(giveAwayWidth, 0.0))
    {
      int countOfParticipatingColumns = 0;
      double perColumnGiveAwayWidth = this.FindMinimumLaggingWidthOfNonStarColumns(ignoredColumn, out countOfParticipatingColumns);
      if (countOfParticipatingColumns != 0)
      {
        double num = perColumnGiveAwayWidth * (double) countOfParticipatingColumns;
        if (DoubleUtil.GreaterThanOrClose(num, giveAwayWidth))
        {
          perColumnGiveAwayWidth = giveAwayWidth / (double) countOfParticipatingColumns;
          giveAwayWidth = 0.0;
        }
        else
          giveAwayWidth -= num;
        this.GiveAwayWidthToEveryNonStarColumn(ignoredColumn, perColumnGiveAwayWidth);
      }
      else
        break;
    }
    return giveAwayWidth;
  }

  private double FindMinimumLaggingWidthOfNonStarColumns(
    DataGridColumn ignoredColumn,
    out int countOfParticipatingColumns)
  {
    double ofNonStarColumns = double.PositiveInfinity;
    countOfParticipatingColumns = 0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (ignoredColumn != dataGridColumn && dataGridColumn.IsVisible)
      {
        DataGridLength width = dataGridColumn.Width;
        if (!width.IsStar)
        {
          double maxWidth = dataGridColumn.MaxWidth;
          if (DoubleUtil.LessThan(width.DisplayValue, width.DesiredValue) && !DoubleUtil.AreClose(width.DisplayValue, maxWidth))
          {
            ++countOfParticipatingColumns;
            double num = Math.Min(width.DesiredValue, maxWidth) - width.DisplayValue;
            if (DoubleUtil.LessThan(num, ofNonStarColumns))
              ofNonStarColumns = num;
          }
        }
      }
    }
    return ofNonStarColumns;
  }

  private void GiveAwayWidthToEveryNonStarColumn(
    DataGridColumn ignoredColumn,
    double perColumnGiveAwayWidth)
  {
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (ignoredColumn != dataGridColumn && dataGridColumn.IsVisible)
      {
        DataGridLength width = dataGridColumn.Width;
        if (!width.IsStar && DoubleUtil.LessThan(width.DisplayValue, Math.Min(width.DesiredValue, dataGridColumn.MaxWidth)))
          dataGridColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, width.DisplayValue + perColumnGiveAwayWidth));
      }
    }
  }

  private double GiveAwayWidthToScrollViewerExcess(
    double giveAwayWidth,
    bool includedInColumnsWidth)
  {
    double viewportWidthForColumns = this.DataGridOwner.GetViewportWidthForColumns();
    double num = 0.0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (dataGridColumn.IsVisible)
        num += dataGridColumn.Width.DisplayValue;
    }
    if (includedInColumnsWidth)
    {
      if (DoubleUtil.GreaterThan(num, viewportWidthForColumns))
      {
        double val1 = num - viewportWidthForColumns;
        giveAwayWidth -= Math.Min(val1, giveAwayWidth);
      }
    }
    else
      giveAwayWidth = Math.Min(giveAwayWidth, Math.Max(0.0, viewportWidthForColumns - num));
    return giveAwayWidth;
  }

  private double TakeAwayUnusedSpaceOnColumnPositiveResize(
    double horizontalChange,
    int resizingColumnIndex,
    bool retainAuto)
  {
    double widthFromUnusedSpace = this.TakeAwayWidthFromUnusedSpace(false, horizontalChange);
    if (DoubleUtil.LessThan(widthFromUnusedSpace, horizontalChange))
      DataGridColumnCollection.SetResizedColumnWidth(this.ColumnFromDisplayIndex(resizingColumnIndex), horizontalChange - widthFromUnusedSpace, retainAuto);
    return widthFromUnusedSpace;
  }

  private double TakeAwayWidthFromUnusedSpace(
    bool spaceAlreadyUtilized,
    double takeAwayWidth,
    double totalAvailableWidth)
  {
    double num1 = 0.0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (dataGridColumn.IsVisible)
        num1 += dataGridColumn.Width.DisplayValue;
    }
    if (spaceAlreadyUtilized)
      return DoubleUtil.GreaterThanOrClose(totalAvailableWidth, num1) ? 0.0 : Math.Min(num1 - totalAvailableWidth, takeAwayWidth);
    double num2 = totalAvailableWidth - num1;
    if (DoubleUtil.GreaterThan(num2, 0.0))
      takeAwayWidth = Math.Max(0.0, takeAwayWidth - num2);
    return takeAwayWidth;
  }

  private double TakeAwayWidthFromUnusedSpace(bool spaceAlreadyUtilized, double takeAwayWidth)
  {
    double viewportWidthForColumns = this.DataGridOwner.GetViewportWidthForColumns();
    return DoubleUtil.GreaterThan(viewportWidthForColumns, 0.0) ? this.TakeAwayWidthFromUnusedSpace(spaceAlreadyUtilized, takeAwayWidth, viewportWidthForColumns) : takeAwayWidth;
  }

  private double TakeAwayWidthFromColumns(
    DataGridColumn ignoredColumn,
    double takeAwayWidth,
    bool widthAlreadyUtilized)
  {
    double viewportWidthForColumns = this.DataGridOwner.GetViewportWidthForColumns();
    return this.TakeAwayWidthFromColumns(ignoredColumn, takeAwayWidth, widthAlreadyUtilized, viewportWidthForColumns);
  }

  private double TakeAwayWidthFromColumns(
    DataGridColumn ignoredColumn,
    double takeAwayWidth,
    bool widthAlreadyUtilized,
    double totalAvailableWidth)
  {
    takeAwayWidth = this.TakeAwayWidthFromUnusedSpace(widthAlreadyUtilized, takeAwayWidth, totalAvailableWidth);
    takeAwayWidth = this.TakeAwayWidthFromStarColumns(ignoredColumn, takeAwayWidth);
    takeAwayWidth = this.TakeAwayWidthFromNonStarColumns(ignoredColumn, takeAwayWidth);
    return takeAwayWidth;
  }

  private double TakeAwayWidthFromStarColumns(DataGridColumn ignoredColumn, double takeAwayWidth)
  {
    if (DoubleUtil.GreaterThan(takeAwayWidth, 0.0))
    {
      double num = 0.0;
      double val2 = 0.0;
      foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
      {
        DataGridLength width = dataGridColumn.Width;
        if (width.IsStar && dataGridColumn.IsVisible)
        {
          if (dataGridColumn == ignoredColumn)
            num += takeAwayWidth;
          num += width.DisplayValue;
          val2 += dataGridColumn.MinWidth;
        }
      }
      double val1 = num - takeAwayWidth;
      takeAwayWidth = Math.Max(this.ComputeStarColumnWidths(Math.Max(val1, val2)) - val1, 0.0);
    }
    return takeAwayWidth;
  }

  private double TakeAwayWidthFromNonStarColumns(DataGridColumn ignoredColumn, double takeAwayWidth)
  {
    while (DoubleUtil.GreaterThan(takeAwayWidth, 0.0))
    {
      int countOfParticipatingColumns = 0;
      double perColumnTakeAwayWidth = this.FindMinimumExcessWidthOfNonStarColumns(ignoredColumn, out countOfParticipatingColumns);
      if (countOfParticipatingColumns != 0)
      {
        double num = perColumnTakeAwayWidth * (double) countOfParticipatingColumns;
        if (DoubleUtil.GreaterThanOrClose(num, takeAwayWidth))
        {
          perColumnTakeAwayWidth = takeAwayWidth / (double) countOfParticipatingColumns;
          takeAwayWidth = 0.0;
        }
        else
          takeAwayWidth -= num;
        this.TakeAwayWidthFromEveryNonStarColumn(ignoredColumn, perColumnTakeAwayWidth);
      }
      else
        break;
    }
    return takeAwayWidth;
  }

  private double FindMinimumExcessWidthOfNonStarColumns(
    DataGridColumn ignoredColumn,
    out int countOfParticipatingColumns)
  {
    double ofNonStarColumns = double.PositiveInfinity;
    countOfParticipatingColumns = 0;
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (ignoredColumn != dataGridColumn && dataGridColumn.IsVisible)
      {
        DataGridLength width = dataGridColumn.Width;
        if (!width.IsStar)
        {
          double minWidth = dataGridColumn.MinWidth;
          if (DoubleUtil.GreaterThan(width.DisplayValue, minWidth))
          {
            ++countOfParticipatingColumns;
            double num = width.DisplayValue - minWidth;
            if (DoubleUtil.LessThan(num, ofNonStarColumns))
              ofNonStarColumns = num;
          }
        }
      }
    }
    return ofNonStarColumns;
  }

  private void TakeAwayWidthFromEveryNonStarColumn(
    DataGridColumn ignoredColumn,
    double perColumnTakeAwayWidth)
  {
    foreach (DataGridColumn dataGridColumn in (Collection<DataGridColumn>) this)
    {
      if (ignoredColumn != dataGridColumn && dataGridColumn.IsVisible)
      {
        DataGridLength width = dataGridColumn.Width;
        if (!width.IsStar && DoubleUtil.GreaterThan(width.DisplayValue, dataGridColumn.MinWidth))
          dataGridColumn.SetWidthInternal(new DataGridLength(width.Value, width.UnitType, width.DesiredValue, width.DisplayValue - perColumnTakeAwayWidth));
      }
    }
  }

  internal bool RebuildRealizedColumnsBlockListForNonVirtualizedRows { get; set; }

  internal List<RealizedColumnsBlock> RealizedColumnsBlockListForNonVirtualizedRows
  {
    get => this._realizedColumnsBlockListForNonVirtualizedRows;
    set
    {
      this._realizedColumnsBlockListForNonVirtualizedRows = value;
      DataGrid dataGridOwner = this.DataGridOwner;
      dataGridOwner.NotifyPropertyChanged((DependencyObject) dataGridOwner, nameof (RealizedColumnsBlockListForNonVirtualizedRows), new DependencyPropertyChangedEventArgs(), NotificationTarget.CellsPresenter | NotificationTarget.ColumnHeadersPresenter);
    }
  }

  internal List<RealizedColumnsBlock> RealizedColumnsDisplayIndexBlockListForNonVirtualizedRows { get; set; }

  internal bool RebuildRealizedColumnsBlockListForVirtualizedRows { get; set; }

  internal List<RealizedColumnsBlock> RealizedColumnsBlockListForVirtualizedRows
  {
    get => this._realizedColumnsBlockListForVirtualizedRows;
    set
    {
      this._realizedColumnsBlockListForVirtualizedRows = value;
      DataGrid dataGridOwner = this.DataGridOwner;
      dataGridOwner.NotifyPropertyChanged((DependencyObject) dataGridOwner, nameof (RealizedColumnsBlockListForVirtualizedRows), new DependencyPropertyChangedEventArgs(), NotificationTarget.CellsPresenter | NotificationTarget.ColumnHeadersPresenter);
    }
  }

  internal List<RealizedColumnsBlock> RealizedColumnsDisplayIndexBlockListForVirtualizedRows { get; set; }

  internal void InvalidateColumnRealization(bool invalidateForNonVirtualizedRows)
  {
    this.RebuildRealizedColumnsBlockListForVirtualizedRows = true;
    if (!invalidateForNonVirtualizedRows)
      return;
    this.RebuildRealizedColumnsBlockListForNonVirtualizedRows = true;
  }

  internal int FirstVisibleDisplayIndex
  {
    get
    {
      int displayIndex = 0;
      for (int count = this.Count; displayIndex < count; ++displayIndex)
      {
        if (this.ColumnFromDisplayIndex(displayIndex).IsVisible)
          return displayIndex;
      }
      return -1;
    }
  }

  internal int LastVisibleDisplayIndex
  {
    get
    {
      for (int displayIndex = this.Count - 1; displayIndex >= 0; --displayIndex)
      {
        if (this.ColumnFromDisplayIndex(displayIndex).IsVisible)
          return displayIndex;
      }
      return -1;
    }
  }

  internal bool RefreshAutoWidthColumns { get; set; }
}
