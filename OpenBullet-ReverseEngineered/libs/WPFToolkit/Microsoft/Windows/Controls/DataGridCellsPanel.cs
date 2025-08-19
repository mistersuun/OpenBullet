// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridCellsPanel
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridCellsPanel : VirtualizingPanel
{
  private DataGrid _parentDataGrid;
  private UIElement _clippedChildForFrozenBehaviour;
  private RectangleGeometry _childClipForFrozenBehavior = new RectangleGeometry();
  private List<UIElement> _realizedChildren;

  static DataGridCellsPanel()
  {
    KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (DataGridCellsPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Local));
  }

  public DataGridCellsPanel()
  {
    this.IsVirtualizing = false;
    this.InRecyclingMode = false;
  }

  protected override Size MeasureOverride(Size constraint)
  {
    Size size1 = new Size();
    this.DetermineVirtualizationState();
    this.EnsureRealizedChildren();
    IList realizedChildren = this.RealizedChildren;
    Size size2 = !this.RebuildRealizedColumnsBlockList ? this.GenerateAndMeasureChildrenForRealizedColumns(constraint) : this.DetermineRealizedColumnsBlockList(constraint);
    if (this.IsVirtualizing && this.InRecyclingMode)
      this.DisconnectRecycledContainers();
    return size2;
  }

  private static void MeasureChild(UIElement child, Size constraint)
  {
    IProvideDataGridColumn provideDataGridColumn = child as IProvideDataGridColumn;
    bool isHeader = child is Microsoft.Windows.Controls.Primitives.DataGridColumnHeader;
    Size availableSize = new Size(double.PositiveInfinity, constraint.Height);
    double num = 0.0;
    bool flag = false;
    if (provideDataGridColumn != null)
    {
      DataGridColumn column = provideDataGridColumn.Column;
      DataGridLength width = column.Width;
      if (width.IsAuto || width.IsSizeToHeader && isHeader || width.IsSizeToCells && !isHeader)
      {
        child.Measure(availableSize);
        num = child.DesiredSize.Width;
        flag = true;
      }
      availableSize.Width = column.GetConstraintWidth(isHeader);
    }
    if (DoubleUtil.AreClose(num, 0.0))
      child.Measure(availableSize);
    Size desiredSize = child.DesiredSize;
    if (provideDataGridColumn == null)
      return;
    DataGridColumn column1 = provideDataGridColumn.Column;
    column1.UpdateDesiredWidthForAutoColumn(isHeader, DoubleUtil.AreClose(num, 0.0) ? desiredSize.Width : num);
    DataGridLength width1 = column1.Width;
    if (!flag || DoubleUtil.IsNaN(width1.DisplayValue) || !DoubleUtil.GreaterThan(num, width1.DisplayValue))
      return;
    availableSize.Width = width1.DisplayValue;
    child.Measure(availableSize);
  }

  private Size GenerateAndMeasureChildrenForRealizedColumns(Size constraint)
  {
    double num1 = 0.0;
    double num2 = 0.0;
    DataGrid parentDataGrid = this.ParentDataGrid;
    double averageColumnWidth = parentDataGrid.InternalColumns.AverageColumnWidth;
    IItemContainerGenerator containerGenerator = this.ItemContainerGenerator;
    List<RealizedColumnsBlock> columnsBlockList = this.RealizedColumnsBlockList;
    this.VirtualizeChildren(columnsBlockList, containerGenerator);
    double width;
    if (columnsBlockList.Count > 0)
    {
      int index = 0;
      for (int count = columnsBlockList.Count; index < count; ++index)
      {
        RealizedColumnsBlock realizedColumnsBlock1 = columnsBlockList[index];
        Size children = this.GenerateChildren(containerGenerator, realizedColumnsBlock1.StartIndex, realizedColumnsBlock1.EndIndex, constraint);
        num1 += children.Width;
        num2 = Math.Max(num2, children.Height);
        if (index != count - 1)
        {
          RealizedColumnsBlock realizedColumnsBlock2 = columnsBlockList[index + 1];
          num1 += this.GetColumnEstimatedMeasureWidthSum(realizedColumnsBlock1.EndIndex + 1, realizedColumnsBlock2.StartIndex - 1, averageColumnWidth);
        }
      }
      width = num1 + this.GetColumnEstimatedMeasureWidthSum(0, columnsBlockList[0].StartIndex - 1, averageColumnWidth) + this.GetColumnEstimatedMeasureWidthSum(columnsBlockList[columnsBlockList.Count - 1].EndIndex + 1, parentDataGrid.Columns.Count - 1, averageColumnWidth);
    }
    else
      width = 0.0;
    return new Size(width, num2);
  }

  private Size DetermineRealizedColumnsBlockList(Size constraint)
  {
    List<int> realizedColumnIndices = new List<int>();
    List<int> realizedColumnDisplayIndices = new List<int>();
    Size columnsBlockList = new Size();
    DataGrid parentDataGrid = this.ParentDataGrid;
    if (parentDataGrid == null)
      return columnsBlockList;
    double horizontalScrollOffset = parentDataGrid.HorizontalScrollOffset;
    double horizontalOffset = parentDataGrid.CellsPanelHorizontalOffset;
    double num1 = horizontalScrollOffset;
    double num2 = -horizontalOffset;
    double num3 = horizontalScrollOffset - horizontalOffset;
    int firstVisibleNonFrozenDisplayIndex = -1;
    int lastVisibleNonFrozenDisplayIndex = -1;
    double num4 = this.GetViewportWidth() - horizontalOffset;
    double num5 = 0.0;
    if (DoubleUtil.LessThan(num4, 0.0))
      return columnsBlockList;
    bool visibleStarColumns = parentDataGrid.InternalColumns.HasVisibleStarColumns;
    double averageColumnWidth = parentDataGrid.InternalColumns.AverageColumnWidth;
    bool flag1 = DoubleUtil.AreClose(averageColumnWidth, 0.0);
    bool flag2 = !this.IsVirtualizing;
    bool flag3 = flag1 || visibleStarColumns || flag2;
    int frozenColumnCount = parentDataGrid.FrozenColumnCount;
    int num6 = -1;
    bool redeterminationNeeded = false;
    IItemContainerGenerator containerGenerator = this.ItemContainerGenerator;
    IDisposable generatorState = (IDisposable) null;
    int childIndex = 0;
    try
    {
      int displayIndex = 0;
      for (int count = parentDataGrid.Columns.Count; displayIndex < count; ++displayIndex)
      {
        DataGridColumn column = parentDataGrid.ColumnFromDisplayIndex(displayIndex);
        if (column.IsVisible)
        {
          int num7 = parentDataGrid.ColumnIndexFromDisplayIndex(displayIndex);
          if (num7 != childIndex || num6 != num7 - 1)
          {
            childIndex = num7;
            if (generatorState != null)
            {
              generatorState.Dispose();
              generatorState = (IDisposable) null;
            }
          }
          num6 = num7;
          Size childSize;
          if (flag3)
          {
            if (this.GenerateChild(containerGenerator, constraint, column, ref generatorState, ref childIndex, out childSize) == null)
              break;
          }
          else
            childSize = new Size(DataGridCellsPanel.GetColumnEstimatedMeasureWidth(column, averageColumnWidth), 0.0);
          if (flag2 || visibleStarColumns || DoubleUtil.LessThan(num5, num4))
          {
            if (displayIndex < frozenColumnCount)
            {
              if (!flag3)
              {
                if (this.GenerateChild(containerGenerator, constraint, column, ref generatorState, ref childIndex, out childSize) == null)
                  break;
              }
              realizedColumnIndices.Add(num7);
              realizedColumnDisplayIndices.Add(displayIndex);
              num5 += childSize.Width;
              num1 += childSize.Width;
            }
            else if (DoubleUtil.LessThanOrClose(num2, num3))
            {
              if (DoubleUtil.LessThanOrClose(num2 + childSize.Width, num3))
              {
                if (flag3)
                {
                  if (flag2 || visibleStarColumns)
                  {
                    realizedColumnIndices.Add(num7);
                    realizedColumnDisplayIndices.Add(displayIndex);
                  }
                  else if (flag1)
                    redeterminationNeeded = true;
                }
                else if (generatorState != null)
                {
                  generatorState.Dispose();
                  generatorState = (IDisposable) null;
                }
                num2 += childSize.Width;
              }
              else
              {
                if (!flag3)
                {
                  if (this.GenerateChild(containerGenerator, constraint, column, ref generatorState, ref childIndex, out childSize) == null)
                    break;
                }
                double num8 = num3 - num2;
                if (DoubleUtil.AreClose(num8, 0.0))
                {
                  num2 = num1 + childSize.Width;
                  num5 += childSize.Width;
                }
                else
                {
                  double num9 = childSize.Width - num8;
                  num2 = num1 + num9;
                  num5 += num9;
                }
                realizedColumnIndices.Add(num7);
                realizedColumnDisplayIndices.Add(displayIndex);
                firstVisibleNonFrozenDisplayIndex = displayIndex;
                lastVisibleNonFrozenDisplayIndex = displayIndex;
              }
            }
            else
            {
              if (!flag3)
              {
                if (this.GenerateChild(containerGenerator, constraint, column, ref generatorState, ref childIndex, out childSize) == null)
                  break;
              }
              if (firstVisibleNonFrozenDisplayIndex < 0)
                firstVisibleNonFrozenDisplayIndex = displayIndex;
              lastVisibleNonFrozenDisplayIndex = displayIndex;
              num2 += childSize.Width;
              num5 += childSize.Width;
              realizedColumnIndices.Add(num7);
              realizedColumnDisplayIndices.Add(displayIndex);
            }
          }
          columnsBlockList.Width += childSize.Width;
          columnsBlockList.Height = Math.Max(columnsBlockList.Height, childSize.Height);
        }
      }
    }
    finally
    {
      generatorState?.Dispose();
    }
    if (!visibleStarColumns && !flag2)
    {
      if (this.ParentPresenter is Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter)
      {
        Size size = this.EnsureAtleastOneHeader(containerGenerator, constraint, realizedColumnIndices, realizedColumnDisplayIndices);
        columnsBlockList.Height = Math.Max(columnsBlockList.Height, size.Height);
        redeterminationNeeded = true;
      }
      else
        this.EnsureFocusTrail(realizedColumnIndices, realizedColumnDisplayIndices, firstVisibleNonFrozenDisplayIndex, lastVisibleNonFrozenDisplayIndex, constraint);
    }
    this.UpdateRealizedBlockLists(realizedColumnIndices, realizedColumnDisplayIndices, redeterminationNeeded);
    this.VirtualizeChildren(this.RealizedColumnsBlockList, containerGenerator);
    return columnsBlockList;
  }

  private void UpdateRealizedBlockLists(
    List<int> realizedColumnIndices,
    List<int> realizedColumnDisplayIndices,
    bool redeterminationNeeded)
  {
    realizedColumnIndices.Sort();
    this.RealizedColumnsBlockList = DataGridCellsPanel.BuildRealizedColumnsBlockList(realizedColumnIndices);
    this.RealizedColumnsDisplayIndexBlockList = DataGridCellsPanel.BuildRealizedColumnsBlockList(realizedColumnDisplayIndices);
    if (redeterminationNeeded)
      return;
    this.RebuildRealizedColumnsBlockList = false;
  }

  private static List<RealizedColumnsBlock> BuildRealizedColumnsBlockList(List<int> indexList)
  {
    List<RealizedColumnsBlock> realizedColumnsBlockList = new List<RealizedColumnsBlock>();
    if (indexList.Count == 1)
      realizedColumnsBlockList.Add(new RealizedColumnsBlock(indexList[0], indexList[0], 0));
    else if (indexList.Count > 0)
    {
      int index1 = indexList[0];
      int index2 = 1;
      for (int count = indexList.Count; index2 < count; ++index2)
      {
        if (indexList[index2] != indexList[index2 - 1] + 1)
        {
          if (realizedColumnsBlockList.Count == 0)
          {
            realizedColumnsBlockList.Add(new RealizedColumnsBlock(index1, indexList[index2 - 1], 0));
          }
          else
          {
            RealizedColumnsBlock realizedColumnsBlock = realizedColumnsBlockList[realizedColumnsBlockList.Count - 1];
            int startIndexOffset = realizedColumnsBlock.StartIndexOffset + realizedColumnsBlock.EndIndex - realizedColumnsBlock.StartIndex + 1;
            realizedColumnsBlockList.Add(new RealizedColumnsBlock(index1, indexList[index2 - 1], startIndexOffset));
          }
          index1 = indexList[index2];
        }
        if (index2 == count - 1)
        {
          if (realizedColumnsBlockList.Count == 0)
          {
            realizedColumnsBlockList.Add(new RealizedColumnsBlock(index1, indexList[index2], 0));
          }
          else
          {
            RealizedColumnsBlock realizedColumnsBlock = realizedColumnsBlockList[realizedColumnsBlockList.Count - 1];
            int startIndexOffset = realizedColumnsBlock.StartIndexOffset + realizedColumnsBlock.EndIndex - realizedColumnsBlock.StartIndex + 1;
            realizedColumnsBlockList.Add(new RealizedColumnsBlock(index1, indexList[index2], startIndexOffset));
          }
        }
      }
    }
    return realizedColumnsBlockList;
  }

  private static GeneratorPosition IndexToGeneratorPositionForStart(
    IItemContainerGenerator generator,
    int index,
    out int childIndex)
  {
    GeneratorPosition positionForStart = generator != null ? generator.GeneratorPositionFromIndex(index) : new GeneratorPosition(-1, index + 1);
    childIndex = positionForStart.Offset == 0 ? positionForStart.Index : positionForStart.Index + 1;
    return positionForStart;
  }

  private UIElement GenerateChild(
    IItemContainerGenerator generator,
    Size constraint,
    DataGridColumn column,
    ref IDisposable generatorState,
    ref int childIndex,
    out Size childSize)
  {
    if (generatorState == null)
      generatorState = generator.StartAt(DataGridCellsPanel.IndexToGeneratorPositionForStart(generator, childIndex, out childIndex), GeneratorDirection.Forward, true);
    return this.GenerateChild(generator, constraint, column, ref childIndex, out childSize);
  }

  private UIElement GenerateChild(
    IItemContainerGenerator generator,
    Size constraint,
    DataGridColumn column,
    ref int childIndex,
    out Size childSize)
  {
    bool isNewlyRealized;
    if (!(generator.GenerateNext(out isNewlyRealized) is UIElement next))
    {
      childSize = new Size();
      return (UIElement) null;
    }
    this.AddContainerFromGenerator(childIndex, next, isNewlyRealized);
    ++childIndex;
    DataGridCellsPanel.MeasureChild(next, constraint);
    DataGridLength width = column.Width;
    childSize = next.DesiredSize;
    if (!DoubleUtil.IsNaN(width.DisplayValue))
      childSize = new Size(width.DisplayValue, childSize.Height);
    return next;
  }

  private Size GenerateChildren(
    IItemContainerGenerator generator,
    int startIndex,
    int endIndex,
    Size constraint)
  {
    double width = 0.0;
    double num = 0.0;
    int childIndex;
    GeneratorPosition positionForStart = DataGridCellsPanel.IndexToGeneratorPositionForStart(generator, startIndex, out childIndex);
    DataGrid parentDataGrid = this.ParentDataGrid;
    using (generator.StartAt(positionForStart, GeneratorDirection.Forward, true))
    {
      for (int index = startIndex; index <= endIndex; ++index)
      {
        if (parentDataGrid.Columns[index].IsVisible)
        {
          Size childSize;
          if (this.GenerateChild(generator, constraint, parentDataGrid.Columns[index], ref childIndex, out childSize) == null)
            return new Size(width, num);
          width += childSize.Width;
          num = Math.Max(num, childSize.Height);
        }
      }
    }
    return new Size(width, num);
  }

  private void AddContainerFromGenerator(int childIndex, UIElement child, bool newlyRealized)
  {
    if (!newlyRealized)
    {
      if (!this.InRecyclingMode)
        return;
      IList realizedChildren = this.RealizedChildren;
      if (childIndex < realizedChildren.Count && realizedChildren[childIndex] == child)
        return;
      this.InsertRecycledContainer(childIndex, child);
      child.Measure(new Size());
    }
    else
      this.InsertNewContainer(childIndex, child);
  }

  private void InsertRecycledContainer(int childIndex, UIElement container)
  {
    this.InsertContainer(childIndex, container, true);
  }

  private void InsertNewContainer(int childIndex, UIElement container)
  {
    this.InsertContainer(childIndex, container, false);
  }

  private void InsertContainer(int childIndex, UIElement container, bool isRecycled)
  {
    UIElementCollection internalChildren = this.InternalChildren;
    int index1 = 0;
    if (childIndex > 0)
      index1 = this.ChildIndexFromRealizedIndex(childIndex - 1) + 1;
    if (!isRecycled || index1 >= internalChildren.Count || internalChildren[index1] != container)
    {
      if (index1 < internalChildren.Count)
      {
        int index2 = index1;
        if (isRecycled && VisualTreeHelper.GetParent((DependencyObject) container) != null)
        {
          int index3 = internalChildren.IndexOf(container);
          this.RemoveInternalChildRange(index3, 1);
          if (index3 < index2)
            --index2;
          this.InsertInternalChild(index2, container);
        }
        else
          this.InsertInternalChild(index2, container);
      }
      else if (isRecycled && VisualTreeHelper.GetParent((DependencyObject) container) != null)
      {
        this.RemoveInternalChildRange(internalChildren.IndexOf(container), 1);
        this.AddInternalChild(container);
      }
      else
        this.AddInternalChild(container);
    }
    if (this.IsVirtualizing && this.InRecyclingMode)
      this._realizedChildren.Insert(childIndex, container);
    this.ItemContainerGenerator.PrepareItemContainer((DependencyObject) container);
  }

  private int ChildIndexFromRealizedIndex(int realizedChildIndex)
  {
    if (this.IsVirtualizing && this.InRecyclingMode && realizedChildIndex < this._realizedChildren.Count)
    {
      UIElement realizedChild = this._realizedChildren[realizedChildIndex];
      UIElementCollection internalChildren = this.InternalChildren;
      for (int index = realizedChildIndex; index < internalChildren.Count; ++index)
      {
        if (internalChildren[index] == realizedChild)
          return index;
      }
    }
    return realizedChildIndex;
  }

  private static bool InBlockOrNextBlock(
    List<RealizedColumnsBlock> blockList,
    int index,
    ref int blockIndex,
    ref RealizedColumnsBlock block,
    out bool pastLastBlock)
  {
    pastLastBlock = false;
    bool flag = true;
    if (index < block.StartIndex)
      flag = false;
    else if (index > block.EndIndex)
    {
      if (blockIndex == blockList.Count - 1)
      {
        ++blockIndex;
        pastLastBlock = true;
        flag = false;
      }
      else
      {
        block = blockList[++blockIndex];
        if (index < block.StartIndex || index > block.EndIndex)
          flag = false;
      }
    }
    return flag;
  }

  private Size EnsureAtleastOneHeader(
    IItemContainerGenerator generator,
    Size constraint,
    List<int> realizedColumnIndices,
    List<int> realizedColumnDisplayIndices)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    int count = parentDataGrid.Columns.Count;
    Size childSize = new Size();
    if (this.RealizedChildren.Count == 0 && count > 0)
    {
      for (int index = 0; index < count; ++index)
      {
        DataGridColumn column = parentDataGrid.Columns[index];
        if (column.IsVisible)
        {
          int childIndex = index;
          using (generator.StartAt(DataGridCellsPanel.IndexToGeneratorPositionForStart(generator, childIndex, out childIndex), GeneratorDirection.Forward, true))
          {
            if (this.GenerateChild(generator, constraint, column, ref childIndex, out childSize) != null)
            {
              int displayIndexListIterator = 0;
              DataGridCellsPanel.AddToIndicesListIfNeeded(realizedColumnIndices, realizedColumnDisplayIndices, index, column.DisplayIndex, ref displayIndexListIterator);
              return childSize;
            }
          }
        }
      }
    }
    return childSize;
  }

  private void EnsureFocusTrail(
    List<int> realizedColumnIndices,
    List<int> realizedColumnDisplayIndices,
    int firstVisibleNonFrozenDisplayIndex,
    int lastVisibleNonFrozenDisplayIndex,
    Size constraint)
  {
    if (firstVisibleNonFrozenDisplayIndex < 0)
      return;
    int frozenColumnCount = this.ParentDataGrid.FrozenColumnCount;
    int count = this.Columns.Count;
    ItemsControl parentPresenter = this.ParentPresenter;
    if (parentPresenter == null)
      return;
    System.Windows.Controls.ItemContainerGenerator containerGenerator = parentPresenter.ItemContainerGenerator;
    int displayIndexListIterator = 0;
    int num = -1;
    for (int displayIndex = 0; displayIndex < firstVisibleNonFrozenDisplayIndex; ++displayIndex)
    {
      if (this.GenerateChildForFocusTrail(containerGenerator, realizedColumnIndices, realizedColumnDisplayIndices, constraint, displayIndex, ref displayIndexListIterator))
      {
        num = displayIndex;
        break;
      }
    }
    if (num < frozenColumnCount)
    {
      for (int displayIndex = frozenColumnCount; displayIndex < count; ++displayIndex)
      {
        if (this.GenerateChildForFocusTrail(containerGenerator, realizedColumnIndices, realizedColumnDisplayIndices, constraint, displayIndex, ref displayIndexListIterator))
        {
          num = displayIndex;
          break;
        }
      }
    }
    for (int displayIndex = firstVisibleNonFrozenDisplayIndex - 1; displayIndex > num; --displayIndex)
    {
      if (this.GenerateChildForFocusTrail(containerGenerator, realizedColumnIndices, realizedColumnDisplayIndices, constraint, displayIndex, ref displayIndexListIterator))
      {
        num = displayIndex;
        break;
      }
    }
    for (int displayIndex = lastVisibleNonFrozenDisplayIndex + 1; displayIndex < count; ++displayIndex)
    {
      if (this.GenerateChildForFocusTrail(containerGenerator, realizedColumnIndices, realizedColumnDisplayIndices, constraint, displayIndex, ref displayIndexListIterator))
      {
        num = displayIndex;
        break;
      }
    }
    int displayIndex1 = count - 1;
    while (displayIndex1 > num && !this.GenerateChildForFocusTrail(containerGenerator, realizedColumnIndices, realizedColumnDisplayIndices, constraint, displayIndex1, ref displayIndexListIterator))
      --displayIndex1;
  }

  private bool GenerateChildForFocusTrail(
    System.Windows.Controls.ItemContainerGenerator generator,
    List<int> realizedColumnIndices,
    List<int> realizedColumnDisplayIndices,
    Size constraint,
    int displayIndex,
    ref int displayIndexListIterator)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    DataGridColumn column = parentDataGrid.ColumnFromDisplayIndex(displayIndex);
    if (column.IsVisible)
    {
      int num = parentDataGrid.ColumnIndexFromDisplayIndex(displayIndex);
      if (!(generator.ContainerFromIndex(num) is UIElement element))
      {
        int childIndex = num;
        using (((IItemContainerGenerator) generator).StartAt(DataGridCellsPanel.IndexToGeneratorPositionForStart((IItemContainerGenerator) generator, childIndex, out childIndex), GeneratorDirection.Forward, true))
          element = this.GenerateChild((IItemContainerGenerator) generator, constraint, column, ref childIndex, out Size _);
      }
      if (element != null && DataGridHelper.TreeHasFocusAndTabStop((DependencyObject) element))
      {
        DataGridCellsPanel.AddToIndicesListIfNeeded(realizedColumnIndices, realizedColumnDisplayIndices, num, displayIndex, ref displayIndexListIterator);
        return true;
      }
    }
    return false;
  }

  private static void AddToIndicesListIfNeeded(
    List<int> realizedColumnIndices,
    List<int> realizedColumnDisplayIndices,
    int columnIndex,
    int displayIndex,
    ref int displayIndexListIterator)
  {
    int count = realizedColumnDisplayIndices.Count;
    while (displayIndexListIterator < count)
    {
      if (realizedColumnDisplayIndices[displayIndexListIterator] == displayIndex)
        return;
      if (realizedColumnDisplayIndices[displayIndexListIterator] > displayIndex)
      {
        realizedColumnDisplayIndices.Insert(displayIndexListIterator, displayIndex);
        realizedColumnIndices.Add(columnIndex);
        return;
      }
      ++displayIndexListIterator;
    }
    realizedColumnIndices.Add(columnIndex);
    realizedColumnDisplayIndices.Add(displayIndex);
  }

  private void VirtualizeChildren(
    List<RealizedColumnsBlock> blockList,
    IItemContainerGenerator generator)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    ObservableCollection<DataGridColumn> columns = parentDataGrid.Columns;
    int count1 = columns.Count;
    int index1 = 0;
    IList realizedChildren = this.RealizedChildren;
    int count2 = realizedChildren.Count;
    if (count2 == 0)
      return;
    int blockIndex = 0;
    int count3 = blockList.Count;
    RealizedColumnsBlock block = count3 > 0 ? blockList[blockIndex] : new RealizedColumnsBlock(-1, -1, -1);
    bool pastLastBlock = count3 <= 0;
    int startIndex = -1;
    int count4 = 0;
    int num = -1;
    ItemsControl parentPresenter = this.ParentPresenter;
    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter gridCellsPresenter = parentPresenter as Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter;
    Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter headersPresenter = parentPresenter as Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter;
    for (int index2 = 0; index2 < count2; ++index2)
    {
      int index3 = index2;
      UIElement uiElement = realizedChildren[index2] as UIElement;
      if (uiElement is IProvideDataGridColumn provideDataGridColumn)
      {
        DataGridColumn column = provideDataGridColumn.Column;
        while (index1 < count1 && column != columns[index1])
          ++index1;
        index3 = index1++;
      }
      bool flag = pastLastBlock || !DataGridCellsPanel.InBlockOrNextBlock(blockList, index3, ref blockIndex, ref block, out pastLastBlock);
      if (uiElement is DataGridCell dataGridCell && (dataGridCell.IsEditing || dataGridCell.IsKeyboardFocusWithin || dataGridCell == parentDataGrid.FocusedCell) || gridCellsPresenter != null && gridCellsPresenter.IsItemItsOwnContainerInternal(gridCellsPresenter.Items[index3]) || headersPresenter != null && headersPresenter.IsItemItsOwnContainerInternal(headersPresenter.Items[index3]))
        flag = false;
      if (!columns[index3].IsVisible)
        flag = true;
      if (flag)
      {
        if (startIndex == -1)
        {
          startIndex = index2;
          count4 = 1;
        }
        else if (num == index3 - 1)
        {
          ++count4;
        }
        else
        {
          this.CleanupRange(realizedChildren, generator, startIndex, count4);
          count2 -= count4;
          index2 -= count4;
          count4 = 1;
          startIndex = index2;
        }
        num = index3;
      }
      else if (count4 > 0)
      {
        this.CleanupRange(realizedChildren, generator, startIndex, count4);
        count2 -= count4;
        index2 -= count4;
        count4 = 0;
        startIndex = -1;
      }
    }
    if (count4 <= 0)
      return;
    this.CleanupRange(realizedChildren, generator, startIndex, count4);
  }

  private void CleanupRange(
    IList children,
    IItemContainerGenerator generator,
    int startIndex,
    int count)
  {
    if (count <= 0)
      return;
    if (this.IsVirtualizing && this.InRecyclingMode)
    {
      GeneratorPosition position = new GeneratorPosition(startIndex, 0);
      ((IRecyclingItemContainerGenerator) generator).Recycle(position, count);
      this._realizedChildren.RemoveRange(startIndex, count);
    }
    else
    {
      this.RemoveInternalChildRange(startIndex, count);
      generator.Remove(new GeneratorPosition(startIndex, 0), count);
    }
  }

  private void DisconnectRecycledContainers()
  {
    int index1 = 0;
    UIElement realizedChild = this._realizedChildren.Count > 0 ? this._realizedChildren[0] : (UIElement) null;
    UIElementCollection internalChildren = this.InternalChildren;
    int index2 = -1;
    int range = 0;
    for (int index3 = 0; index3 < internalChildren.Count; ++index3)
    {
      if (internalChildren[index3] == realizedChild)
      {
        if (range > 0)
        {
          this.RemoveInternalChildRange(index2, range);
          index3 -= range;
          range = 0;
          index2 = -1;
        }
        ++index1;
        realizedChild = index1 >= this._realizedChildren.Count ? (UIElement) null : this._realizedChildren[index1];
      }
      else
      {
        if (index2 == -1)
          index2 = index3;
        ++range;
      }
    }
    if (range <= 0)
      return;
    this.RemoveInternalChildRange(index2, range);
  }

  private void InitializeArrangeState(DataGridCellsPanel.ArrangeState arrangeState)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    double horizontalScrollOffset = parentDataGrid.HorizontalScrollOffset;
    double horizontalOffset = parentDataGrid.CellsPanelHorizontalOffset;
    arrangeState.NextFrozenCellStart = horizontalScrollOffset;
    arrangeState.NextNonFrozenCellStart -= horizontalOffset;
    arrangeState.ViewportStartX = horizontalScrollOffset - horizontalOffset;
    arrangeState.FrozenColumnCount = parentDataGrid.FrozenColumnCount;
  }

  private void FinishArrange(DataGridCellsPanel.ArrangeState arrangeState)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    if (parentDataGrid != null)
      parentDataGrid.NonFrozenColumnsViewportHorizontalOffset = arrangeState.DataGridHorizontalScrollStartX;
    if (arrangeState.OldClippedChild != null)
      arrangeState.OldClippedChild.CoerceValue(UIElement.ClipProperty);
    this._clippedChildForFrozenBehaviour = arrangeState.NewClippedChild;
    if (this._clippedChildForFrozenBehaviour == null)
      return;
    this._clippedChildForFrozenBehaviour.CoerceValue(UIElement.ClipProperty);
  }

  private void SetDataGridCellPanelWidth(IList children, double newWidth)
  {
    if (children.Count == 0 || !(children[0] is Microsoft.Windows.Controls.Primitives.DataGridColumnHeader) || DoubleUtil.AreClose(this.ParentDataGrid.CellsPanelActualWidth, newWidth))
      return;
    this.ParentDataGrid.CellsPanelActualWidth = newWidth;
  }

  [Conditional("DEBUG")]
  private static void Debug_VerifyRealizedIndexCountVsDisplayIndexCount(
    List<RealizedColumnsBlock> blockList,
    List<RealizedColumnsBlock> displayIndexBlockList)
  {
    RealizedColumnsBlock block = blockList[blockList.Count - 1];
    RealizedColumnsBlock displayIndexBlock = displayIndexBlockList[displayIndexBlockList.Count - 1];
  }

  protected override Size ArrangeOverride(Size arrangeSize)
  {
    IList realizedChildren = this.RealizedChildren;
    DataGridCellsPanel.ArrangeState arrangeState = new DataGridCellsPanel.ArrangeState();
    arrangeState.ChildHeight = arrangeSize.Height;
    DataGrid parentDataGrid = this.ParentDataGrid;
    if (parentDataGrid != null)
    {
      parentDataGrid.QueueInvalidateCellsPanelHorizontalOffset();
      this.SetDataGridCellPanelWidth(realizedChildren, arrangeSize.Width);
      this.InitializeArrangeState(arrangeState);
    }
    List<RealizedColumnsBlock> displayIndexBlockList = this.RealizedColumnsDisplayIndexBlockList;
    if (displayIndexBlockList != null && displayIndexBlockList.Count > 0)
    {
      double averageColumnWidth = parentDataGrid.InternalColumns.AverageColumnWidth;
      List<RealizedColumnsBlock> columnsBlockList = this.RealizedColumnsBlockList;
      List<int> childrenNotInBlockList = this.GetRealizedChildrenNotInBlockList(columnsBlockList, realizedChildren);
      int num1 = -1;
      int blockIndex;
      RealizedColumnsBlock block = displayIndexBlockList[blockIndex = num1 + 1];
      bool pastLastBlock = false;
      int num2 = 0;
      for (int count1 = parentDataGrid.Columns.Count; num2 < count1; ++num2)
      {
        bool flag = DataGridCellsPanel.InBlockOrNextBlock(displayIndexBlockList, num2, ref blockIndex, ref block, out pastLastBlock);
        if (!pastLastBlock)
        {
          if (flag)
          {
            int columnIndex = parentDataGrid.ColumnIndexFromDisplayIndex(num2);
            RealizedColumnsBlock realizedBlockForColumn = DataGridCellsPanel.GetRealizedBlockForColumn(columnsBlockList, columnIndex);
            int index1 = realizedBlockForColumn.StartIndexOffset + columnIndex - realizedBlockForColumn.StartIndex;
            if (childrenNotInBlockList != null)
            {
              int index2 = 0;
              for (int count2 = childrenNotInBlockList.Count; index2 < count2 && childrenNotInBlockList[index2] <= index1; ++index2)
                ++index1;
            }
            this.ArrangeChild(realizedChildren[index1] as UIElement, num2, arrangeState);
          }
          else
          {
            DataGridColumn column = parentDataGrid.ColumnFromDisplayIndex(num2);
            if (column.IsVisible)
            {
              double estimatedMeasureWidth = DataGridCellsPanel.GetColumnEstimatedMeasureWidth(column, averageColumnWidth);
              arrangeState.NextNonFrozenCellStart += estimatedMeasureWidth;
            }
          }
        }
        else
          break;
      }
      if (childrenNotInBlockList != null)
      {
        int index = 0;
        for (int count = childrenNotInBlockList.Count; index < count; ++index)
          (realizedChildren[childrenNotInBlockList[index]] as UIElement).Arrange(new Rect());
      }
    }
    this.FinishArrange(arrangeState);
    return arrangeSize;
  }

  private void ArrangeChild(
    UIElement child,
    int displayIndex,
    DataGridCellsPanel.ArrangeState arrangeState)
  {
    IProvideDataGridColumn provideDataGridColumn = child as IProvideDataGridColumn;
    if (child == this._clippedChildForFrozenBehaviour)
    {
      arrangeState.OldClippedChild = child;
      this._clippedChildForFrozenBehaviour = (UIElement) null;
    }
    double width1;
    if (provideDataGridColumn != null)
    {
      width1 = provideDataGridColumn.Column.Width.DisplayValue;
      if (DoubleUtil.IsNaN(width1))
        width1 = provideDataGridColumn.Column.ActualWidth;
    }
    else
      width1 = child.DesiredSize.Width;
    Rect finalRect = new Rect(new Size(width1, arrangeState.ChildHeight));
    if (displayIndex < arrangeState.FrozenColumnCount)
    {
      finalRect.X = arrangeState.NextFrozenCellStart;
      arrangeState.NextFrozenCellStart += width1;
      arrangeState.DataGridHorizontalScrollStartX += width1;
    }
    else if (DoubleUtil.LessThanOrClose(arrangeState.NextNonFrozenCellStart, arrangeState.ViewportStartX))
    {
      if (DoubleUtil.LessThanOrClose(arrangeState.NextNonFrozenCellStart + width1, arrangeState.ViewportStartX))
      {
        finalRect.X = arrangeState.NextNonFrozenCellStart;
        arrangeState.NextNonFrozenCellStart += width1;
      }
      else
      {
        double x = arrangeState.ViewportStartX - arrangeState.NextNonFrozenCellStart;
        if (DoubleUtil.AreClose(x, 0.0))
        {
          finalRect.X = arrangeState.NextFrozenCellStart;
          arrangeState.NextNonFrozenCellStart = arrangeState.NextFrozenCellStart + width1;
        }
        else
        {
          finalRect.X = arrangeState.NextFrozenCellStart - x;
          double width2 = width1 - x;
          arrangeState.NewClippedChild = child;
          this._childClipForFrozenBehavior.Rect = new Rect(x, 0.0, width2, finalRect.Height);
          arrangeState.NextNonFrozenCellStart = arrangeState.NextFrozenCellStart + width2;
        }
      }
    }
    else
    {
      finalRect.X = arrangeState.NextNonFrozenCellStart;
      arrangeState.NextNonFrozenCellStart += width1;
    }
    child.Arrange(finalRect);
  }

  private static RealizedColumnsBlock GetRealizedBlockForColumn(
    List<RealizedColumnsBlock> blockList,
    int columnIndex)
  {
    int index = 0;
    for (int count = blockList.Count; index < count; ++index)
    {
      RealizedColumnsBlock block = blockList[index];
      if (columnIndex >= block.StartIndex && columnIndex <= block.EndIndex)
        return block;
    }
    return new RealizedColumnsBlock(-1, -1, -1);
  }

  private List<int> GetRealizedChildrenNotInBlockList(
    List<RealizedColumnsBlock> blockList,
    IList children)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    RealizedColumnsBlock block1 = blockList[blockList.Count - 1];
    int num1 = block1.StartIndexOffset + block1.EndIndex - block1.StartIndex + 1;
    if (children.Count == num1)
      return (List<int>) null;
    List<int> childrenNotInBlockList = new List<int>();
    if (blockList.Count == 0)
    {
      int num2 = 0;
      for (int count = children.Count; num2 < count; ++num2)
        childrenNotInBlockList.Add(num2);
    }
    else
    {
      int num3 = 0;
      List<RealizedColumnsBlock> realizedColumnsBlockList = blockList;
      int index1 = num3;
      int num4 = index1 + 1;
      RealizedColumnsBlock block2 = realizedColumnsBlockList[index1];
      int index2 = 0;
      for (int count = children.Count; index2 < count; ++index2)
      {
        IProvideDataGridColumn child = children[index2] as IProvideDataGridColumn;
        int num5 = index2;
        if (child != null)
          num5 = parentDataGrid.Columns.IndexOf(child.Column);
        if (num5 < block2.StartIndex)
          childrenNotInBlockList.Add(index2);
        else if (num5 > block2.EndIndex)
        {
          if (num4 >= blockList.Count)
          {
            for (int index3 = index2; index3 < count; ++index3)
              childrenNotInBlockList.Add(index3);
            break;
          }
          block2 = blockList[num4++];
          if (num5 < block2.StartIndex)
            childrenNotInBlockList.Add(index2);
        }
      }
    }
    return childrenNotInBlockList;
  }

  private bool RebuildRealizedColumnsBlockList
  {
    get
    {
      DataGrid parentDataGrid = this.ParentDataGrid;
      if (parentDataGrid == null)
        return true;
      DataGridColumnCollection internalColumns = parentDataGrid.InternalColumns;
      return !this.IsVirtualizing ? internalColumns.RebuildRealizedColumnsBlockListForNonVirtualizedRows : internalColumns.RebuildRealizedColumnsBlockListForVirtualizedRows;
    }
    set
    {
      DataGrid parentDataGrid = this.ParentDataGrid;
      if (parentDataGrid == null)
        return;
      if (this.IsVirtualizing)
        parentDataGrid.InternalColumns.RebuildRealizedColumnsBlockListForVirtualizedRows = value;
      else
        parentDataGrid.InternalColumns.RebuildRealizedColumnsBlockListForNonVirtualizedRows = value;
    }
  }

  private List<RealizedColumnsBlock> RealizedColumnsBlockList
  {
    get
    {
      DataGrid parentDataGrid = this.ParentDataGrid;
      if (parentDataGrid == null)
        return (List<RealizedColumnsBlock>) null;
      DataGridColumnCollection internalColumns = parentDataGrid.InternalColumns;
      return !this.IsVirtualizing ? internalColumns.RealizedColumnsBlockListForNonVirtualizedRows : internalColumns.RealizedColumnsBlockListForVirtualizedRows;
    }
    set
    {
      DataGrid parentDataGrid = this.ParentDataGrid;
      if (parentDataGrid == null)
        return;
      if (this.IsVirtualizing)
        parentDataGrid.InternalColumns.RealizedColumnsBlockListForVirtualizedRows = value;
      else
        parentDataGrid.InternalColumns.RealizedColumnsBlockListForNonVirtualizedRows = value;
    }
  }

  private List<RealizedColumnsBlock> RealizedColumnsDisplayIndexBlockList
  {
    get
    {
      DataGrid parentDataGrid = this.ParentDataGrid;
      if (parentDataGrid == null)
        return (List<RealizedColumnsBlock>) null;
      DataGridColumnCollection internalColumns = parentDataGrid.InternalColumns;
      return !this.IsVirtualizing ? internalColumns.RealizedColumnsDisplayIndexBlockListForNonVirtualizedRows : internalColumns.RealizedColumnsDisplayIndexBlockListForVirtualizedRows;
    }
    set
    {
      DataGrid parentDataGrid = this.ParentDataGrid;
      if (parentDataGrid == null)
        return;
      if (this.IsVirtualizing)
        parentDataGrid.InternalColumns.RealizedColumnsDisplayIndexBlockListForVirtualizedRows = value;
      else
        parentDataGrid.InternalColumns.RealizedColumnsDisplayIndexBlockListForNonVirtualizedRows = value;
    }
  }

  protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost)
  {
    base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
    if (newIsItemsHost)
    {
      ItemsControl parentPresenter = this.ParentPresenter;
      if (parentPresenter == null)
        return;
      IItemContainerGenerator containerGenerator = (IItemContainerGenerator) parentPresenter.ItemContainerGenerator;
      if (containerGenerator == null || containerGenerator != containerGenerator.GetItemContainerGeneratorForPanel((Panel) this))
        return;
      switch (parentPresenter)
      {
        case Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter gridCellsPresenter:
          gridCellsPresenter.InternalItemsHost = (Panel) this;
          break;
        case Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter headersPresenter:
          headersPresenter.InternalItemsHost = (Panel) this;
          break;
      }
    }
    else
    {
      switch (this.ParentPresenter)
      {
        case Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter gridCellsPresenter1:
          if (gridCellsPresenter1.InternalItemsHost != this)
            break;
          gridCellsPresenter1.InternalItemsHost = (Panel) null;
          break;
        case Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter headersPresenter1:
          if (headersPresenter1.InternalItemsHost != this)
            break;
          headersPresenter1.InternalItemsHost = (Panel) null;
          break;
      }
    }
  }

  private Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter ParentRowsPresenter
  {
    get
    {
      DataGrid parentDataGrid = this.ParentDataGrid;
      return parentDataGrid == null ? (Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter) null : parentDataGrid.InternalItemsHost as Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter;
    }
  }

  private void DetermineVirtualizationState()
  {
    ItemsControl parentPresenter = this.ParentPresenter;
    if (parentPresenter == null)
      return;
    this.IsVirtualizing = VirtualizingStackPanel.GetIsVirtualizing((DependencyObject) parentPresenter);
    this.InRecyclingMode = VirtualizingStackPanel.GetVirtualizationMode((DependencyObject) parentPresenter) == VirtualizationMode.Recycling;
  }

  private bool IsVirtualizing { get; set; }

  private bool InRecyclingMode { get; set; }

  private static double GetColumnEstimatedMeasureWidth(
    DataGridColumn column,
    double averageColumnWidth)
  {
    if (!column.IsVisible)
      return 0.0;
    double estimatedMeasureWidth = column.Width.DisplayValue;
    if (DoubleUtil.IsNaN(estimatedMeasureWidth))
      estimatedMeasureWidth = Math.Min(Math.Max(averageColumnWidth, column.MinWidth), column.MaxWidth);
    return estimatedMeasureWidth;
  }

  private double GetColumnEstimatedMeasureWidthSum(
    int startIndex,
    int endIndex,
    double averageColumnWidth)
  {
    double estimatedMeasureWidthSum = 0.0;
    DataGrid parentDataGrid = this.ParentDataGrid;
    for (int index = startIndex; index <= endIndex; ++index)
      estimatedMeasureWidthSum += DataGridCellsPanel.GetColumnEstimatedMeasureWidth(parentDataGrid.Columns[index], averageColumnWidth);
    return estimatedMeasureWidthSum;
  }

  private IList RealizedChildren
  {
    get
    {
      return this.IsVirtualizing && this.InRecyclingMode ? (IList) this._realizedChildren : (IList) this.InternalChildren;
    }
  }

  private void EnsureRealizedChildren()
  {
    if (this.IsVirtualizing && this.InRecyclingMode)
    {
      if (this._realizedChildren != null)
        return;
      UIElementCollection internalChildren = this.InternalChildren;
      this._realizedChildren = new List<UIElement>(internalChildren.Count);
      for (int index = 0; index < internalChildren.Count; ++index)
        this._realizedChildren.Add(internalChildren[index]);
    }
    else
      this._realizedChildren = (List<UIElement>) null;
  }

  internal double ComputeCellsPanelHorizontalOffset()
  {
    double horizontalOffset = 0.0;
    DataGrid parentDataGrid = this.ParentDataGrid;
    double horizontalScrollOffset = parentDataGrid.HorizontalScrollOffset;
    ScrollViewer internalScrollHost = parentDataGrid.InternalScrollHost;
    if (internalScrollHost != null)
      horizontalOffset = horizontalScrollOffset + this.TransformToAncestor((Visual) internalScrollHost).Transform(new Point()).X;
    return horizontalOffset;
  }

  private double GetViewportWidth()
  {
    double viewportWidth = 0.0;
    DataGrid parentDataGrid = this.ParentDataGrid;
    if (parentDataGrid != null)
    {
      ScrollContentPresenter contentPresenter = parentDataGrid.InternalScrollContentPresenter;
      if (contentPresenter != null && !contentPresenter.CanContentScroll)
        viewportWidth = contentPresenter.ViewportWidth;
      else if (parentDataGrid.InternalItemsHost is IScrollInfo internalItemsHost)
        viewportWidth = internalItemsHost.ViewportWidth;
    }
    Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter parentRowsPresenter = this.ParentRowsPresenter;
    if (DoubleUtil.AreClose(viewportWidth, 0.0) && parentRowsPresenter != null)
    {
      Size availableSize = parentRowsPresenter.AvailableSize;
      if (!DoubleUtil.IsNaN(availableSize.Width) && !double.IsInfinity(availableSize.Width))
        viewportWidth = availableSize.Width;
    }
    return viewportWidth;
  }

  protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
  {
    base.OnItemsChanged(sender, args);
    switch (args.Action)
    {
      case NotifyCollectionChangedAction.Remove:
        this.OnItemsRemove(args);
        break;
      case NotifyCollectionChangedAction.Replace:
        this.OnItemsReplace(args);
        break;
      case NotifyCollectionChangedAction.Move:
        this.OnItemsMove(args);
        break;
    }
  }

  private void OnItemsRemove(ItemsChangedEventArgs args)
  {
    this.RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
  }

  private void OnItemsReplace(ItemsChangedEventArgs args)
  {
    this.RemoveChildRange(args.Position, args.ItemCount, args.ItemUICount);
  }

  private void OnItemsMove(ItemsChangedEventArgs args)
  {
    this.RemoveChildRange(args.OldPosition, args.ItemCount, args.ItemUICount);
  }

  private void RemoveChildRange(GeneratorPosition position, int itemCount, int itemUICount)
  {
    if (!this.IsItemsHost)
      return;
    UIElementCollection internalChildren = this.InternalChildren;
    int index = position.Index;
    if (position.Offset > 0)
      ++index;
    if (index >= internalChildren.Count || itemUICount <= 0)
      return;
    this.RemoveInternalChildRange(index, itemUICount);
    if (!this.IsVirtualizing || !this.InRecyclingMode)
      return;
    this._realizedChildren.RemoveRange(index, itemUICount);
  }

  protected override void OnClearChildren()
  {
    base.OnClearChildren();
    this._realizedChildren = (List<UIElement>) null;
  }

  internal void InternalBringIndexIntoView(int index) => this.BringIndexIntoView(index);

  protected override void BringIndexIntoView(int index)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    if (parentDataGrid == null)
    {
      base.BringIndexIntoView(index);
    }
    else
    {
      if (index < 0 || index >= parentDataGrid.Columns.Count)
        throw new ArgumentOutOfRangeException(nameof (index));
      ScrollContentPresenter contentPresenter = parentDataGrid.InternalScrollContentPresenter;
      IScrollInfo scrollInfo = (IScrollInfo) null;
      if (contentPresenter != null && !contentPresenter.CanContentScroll)
      {
        scrollInfo = (IScrollInfo) contentPresenter;
      }
      else
      {
        Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter parentRowsPresenter = this.ParentRowsPresenter;
        if (parentRowsPresenter != null)
          scrollInfo = (IScrollInfo) parentRowsPresenter;
      }
      if (scrollInfo == null)
      {
        base.BringIndexIntoView(index);
      }
      else
      {
        double newHorizontalOffset = 0.0;
        for (double num = parentDataGrid.HorizontalScrollOffset; !this.IsChildInView(index, out newHorizontalOffset) && !DoubleUtil.AreClose(num, newHorizontalOffset); num = newHorizontalOffset)
        {
          scrollInfo.SetHorizontalOffset(newHorizontalOffset);
          this.UpdateLayout();
        }
      }
    }
  }

  private bool IsChildInView(int index, out double newHorizontalOffset)
  {
    DataGrid parentDataGrid = this.ParentDataGrid;
    double horizontalScrollOffset = parentDataGrid.HorizontalScrollOffset;
    newHorizontalOffset = horizontalScrollOffset;
    double averageColumnWidth = parentDataGrid.InternalColumns.AverageColumnWidth;
    int frozenColumnCount = parentDataGrid.FrozenColumnCount;
    double horizontalOffset = parentDataGrid.CellsPanelHorizontalOffset;
    double viewportWidth = this.GetViewportWidth();
    double num1 = horizontalScrollOffset;
    double num2 = -horizontalOffset;
    double num3 = horizontalScrollOffset - horizontalOffset;
    int displayIndex1 = this.Columns[index].DisplayIndex;
    double num4 = 0.0;
    double num5 = 0.0;
    for (int displayIndex2 = 0; displayIndex2 <= displayIndex1; ++displayIndex2)
    {
      DataGridColumn column = parentDataGrid.ColumnFromDisplayIndex(displayIndex2);
      if (column.IsVisible)
      {
        double estimatedMeasureWidth = DataGridCellsPanel.GetColumnEstimatedMeasureWidth(column, averageColumnWidth);
        if (displayIndex2 < frozenColumnCount)
        {
          num4 = num1;
          num5 = num4 + estimatedMeasureWidth;
          num1 += estimatedMeasureWidth;
        }
        else if (DoubleUtil.LessThanOrClose(num2, num3))
        {
          if (DoubleUtil.LessThanOrClose(num2 + estimatedMeasureWidth, num3))
          {
            num4 = num2;
            num5 = num4 + estimatedMeasureWidth;
            num2 += estimatedMeasureWidth;
          }
          else
          {
            num4 = num1;
            double num6 = num3 - num2;
            if (DoubleUtil.AreClose(num6, 0.0))
            {
              num5 = num4 + estimatedMeasureWidth;
              num2 = num1 + estimatedMeasureWidth;
            }
            else
            {
              double num7 = estimatedMeasureWidth - num6;
              num5 = num4 + num7;
              num2 = num1 + num7;
              if (displayIndex2 == displayIndex1)
              {
                newHorizontalOffset = horizontalScrollOffset - num6;
                return false;
              }
            }
          }
        }
        else
        {
          num4 = num2;
          num5 = num4 + estimatedMeasureWidth;
          num2 += estimatedMeasureWidth;
        }
      }
    }
    double num8 = num3 + viewportWidth;
    if (DoubleUtil.LessThan(num4, num3))
    {
      newHorizontalOffset = num4 + horizontalOffset;
    }
    else
    {
      if (!DoubleUtil.GreaterThan(num5, num8))
        return true;
      double num9 = num5 - num8;
      if (displayIndex1 < frozenColumnCount)
        num1 -= num5 - num4;
      if (DoubleUtil.LessThan(num4 - num9, num1))
        num9 = num4 - num1;
      if (DoubleUtil.AreClose(num9, 0.0))
        return true;
      newHorizontalOffset = horizontalScrollOffset + num9;
    }
    return false;
  }

  internal Geometry GetFrozenClipForChild(UIElement child)
  {
    return child == this._clippedChildForFrozenBehaviour ? (Geometry) this._childClipForFrozenBehavior : (Geometry) null;
  }

  private ObservableCollection<DataGridColumn> Columns => this.ParentDataGrid?.Columns;

  private DataGrid ParentDataGrid
  {
    get
    {
      if (this._parentDataGrid == null)
      {
        if (this.ParentPresenter is Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter parentPresenter2)
        {
          DataGridRow dataGridRowOwner = parentPresenter2.DataGridRowOwner;
          if (dataGridRowOwner != null)
            this._parentDataGrid = dataGridRowOwner.DataGridOwner;
        }
        else if (this.ParentPresenter is Microsoft.Windows.Controls.Primitives.DataGridColumnHeadersPresenter parentPresenter1)
          this._parentDataGrid = parentPresenter1.ParentDataGrid;
      }
      return this._parentDataGrid;
    }
  }

  private ItemsControl ParentPresenter
  {
    get
    {
      return this.TemplatedParent is FrameworkElement templatedParent ? templatedParent.TemplatedParent as ItemsControl : (ItemsControl) null;
    }
  }

  private class ArrangeState
  {
    public ArrangeState()
    {
      this.FrozenColumnCount = 0;
      this.ChildHeight = 0.0;
      this.NextFrozenCellStart = 0.0;
      this.NextNonFrozenCellStart = 0.0;
      this.ViewportStartX = 0.0;
      this.DataGridHorizontalScrollStartX = 0.0;
      this.OldClippedChild = (UIElement) null;
      this.NewClippedChild = (UIElement) null;
    }

    public int FrozenColumnCount { get; set; }

    public double ChildHeight { get; set; }

    public double NextFrozenCellStart { get; set; }

    public double NextNonFrozenCellStart { get; set; }

    public double ViewportStartX { get; set; }

    public double DataGridHorizontalScrollStartX { get; set; }

    public UIElement OldClippedChild { get; set; }

    public UIElement NewClippedChild { get; set; }
  }
}
