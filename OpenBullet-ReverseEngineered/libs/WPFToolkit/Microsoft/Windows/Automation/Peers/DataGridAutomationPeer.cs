// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridAutomationPeer : 
  FrameworkElementAutomationPeer,
  ISelectionProvider,
  ITableProvider,
  IGridProvider
{
  private Dictionary<object, DataGridItemAutomationPeer> _itemPeers = new Dictionary<object, DataGridItemAutomationPeer>();

  public DataGridAutomationPeer(Microsoft.Windows.Controls.DataGrid owner)
    : base((FrameworkElement) owner)
  {
    if (owner == null)
      throw new ArgumentNullException(nameof (owner));
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.DataGrid;
  }

  protected override List<AutomationPeer> GetChildrenCore()
  {
    List<AutomationPeer> childrenCore = this.GetItemPeers();
    DataGridColumnHeadersPresenter headersPresenter = this.OwningDataGrid.ColumnHeadersPresenter;
    if (headersPresenter != null && headersPresenter.IsVisible)
    {
      AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) headersPresenter);
      if (peerForElement != null)
      {
        if (childrenCore == null)
          childrenCore = new List<AutomationPeer>(1);
        childrenCore.Insert(0, peerForElement);
      }
    }
    return childrenCore;
  }

  protected override string GetClassNameCore() => this.Owner.GetType().Name;

  public override object GetPattern(PatternInterface patternInterface)
  {
    switch (patternInterface)
    {
      case PatternInterface.Selection:
      case PatternInterface.Grid:
      case PatternInterface.Table:
        return (object) this;
      case PatternInterface.Scroll:
        ScrollViewer internalScrollHost = this.OwningDataGrid.InternalScrollHost;
        if (internalScrollHost != null)
        {
          AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) internalScrollHost);
          IScrollProvider pattern = peerForElement as IScrollProvider;
          if (peerForElement != null && pattern != null)
          {
            peerForElement.EventsSource = (AutomationPeer) this;
            return (object) pattern;
          }
          break;
        }
        break;
    }
    return base.GetPattern(patternInterface);
  }

  int IGridProvider.ColumnCount => this.OwningDataGrid.Columns.Count;

  int IGridProvider.RowCount => this.OwningDataGrid.Items.Count;

  IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
  {
    if (row >= 0 && row < this.OwningDataGrid.Items.Count && column >= 0 && column < this.OwningDataGrid.Columns.Count)
    {
      object obj = this.OwningDataGrid.Items[row];
      Microsoft.Windows.Controls.DataGridColumn column1 = this.OwningDataGrid.Columns[column];
      this.OwningDataGrid.ScrollIntoView(obj, column1);
      this.OwningDataGrid.UpdateLayout();
      DataGridItemAutomationPeer itemPeer = this.GetOrCreateItemPeer(obj);
      if (itemPeer != null)
      {
        DataGridCellItemAutomationPeer cellItemPeer = itemPeer.GetOrCreateCellItemPeer(column1);
        if (cellItemPeer != null)
          return this.ProviderFromPeer((AutomationPeer) cellItemPeer);
      }
    }
    return (IRawElementProviderSimple) null;
  }

  IRawElementProviderSimple[] ISelectionProvider.GetSelection()
  {
    List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
    switch (this.OwningDataGrid.SelectionUnit)
    {
      case Microsoft.Windows.Controls.DataGridSelectionUnit.Cell:
        this.AddSelectedCells(elementProviderSimpleList);
        break;
      case Microsoft.Windows.Controls.DataGridSelectionUnit.FullRow:
        this.AddSelectedRows(elementProviderSimpleList);
        break;
      case Microsoft.Windows.Controls.DataGridSelectionUnit.CellOrRowHeader:
        this.AddSelectedRows(elementProviderSimpleList);
        this.AddSelectedCells(elementProviderSimpleList);
        break;
    }
    return elementProviderSimpleList.ToArray();
  }

  bool ISelectionProvider.CanSelectMultiple
  {
    get => this.OwningDataGrid.SelectionMode == Microsoft.Windows.Controls.DataGridSelectionMode.Extended;
  }

  bool ISelectionProvider.IsSelectionRequired => false;

  RowOrColumnMajor ITableProvider.RowOrColumnMajor => RowOrColumnMajor.RowMajor;

  IRawElementProviderSimple[] ITableProvider.GetColumnHeaders()
  {
    if ((this.OwningDataGrid.HeadersVisibility & Microsoft.Windows.Controls.DataGridHeadersVisibility.Column) == Microsoft.Windows.Controls.DataGridHeadersVisibility.Column)
    {
      List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
      DataGridColumnHeadersPresenter headersPresenter = this.OwningDataGrid.ColumnHeadersPresenter;
      for (int index = 0; index < this.OwningDataGrid.Columns.Count; ++index)
      {
        if (headersPresenter.ItemContainerGenerator.ContainerFromIndex(index) is DataGridColumnHeader element)
        {
          AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) element);
          if (peerForElement != null)
            elementProviderSimpleList.Add(this.ProviderFromPeer(peerForElement));
        }
      }
      if (elementProviderSimpleList.Count > 0)
        return elementProviderSimpleList.ToArray();
    }
    return (IRawElementProviderSimple[]) null;
  }

  IRawElementProviderSimple[] ITableProvider.GetRowHeaders()
  {
    if ((this.OwningDataGrid.HeadersVisibility & Microsoft.Windows.Controls.DataGridHeadersVisibility.Row) == Microsoft.Windows.Controls.DataGridHeadersVisibility.Row)
    {
      List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
      foreach (object obj in (IEnumerable) this.OwningDataGrid.Items)
      {
        AutomationPeer headerAutomationPeer = this.GetOrCreateItemPeer(obj).RowHeaderAutomationPeer;
        if (headerAutomationPeer != null)
          elementProviderSimpleList.Add(this.ProviderFromPeer(headerAutomationPeer));
      }
      if (elementProviderSimpleList.Count > 0)
        return elementProviderSimpleList.ToArray();
    }
    return (IRawElementProviderSimple[]) null;
  }

  private Microsoft.Windows.Controls.DataGrid OwningDataGrid => (Microsoft.Windows.Controls.DataGrid) this.Owner;

  private List<AutomationPeer> GetItemPeers()
  {
    List<AutomationPeer> itemPeers = new List<AutomationPeer>();
    Dictionary<object, DataGridItemAutomationPeer> dictionary = new Dictionary<object, DataGridItemAutomationPeer>((IDictionary<object, DataGridItemAutomationPeer>) this._itemPeers);
    this._itemPeers.Clear();
    foreach (object key in (IEnumerable) this.OwningDataGrid.Items)
    {
      DataGridItemAutomationPeer itemAutomationPeer = (DataGridItemAutomationPeer) null;
      if (!dictionary.TryGetValue(key, out itemAutomationPeer) || itemAutomationPeer == null)
        itemAutomationPeer = new DataGridItemAutomationPeer(key, this.OwningDataGrid);
      itemPeers.Add((AutomationPeer) itemAutomationPeer);
      this._itemPeers.Add(key, itemAutomationPeer);
    }
    return itemPeers;
  }

  internal DataGridItemAutomationPeer GetOrCreateItemPeer(object item)
  {
    DataGridItemAutomationPeer itemPeer = (DataGridItemAutomationPeer) null;
    if (!this._itemPeers.TryGetValue(item, out itemPeer) || itemPeer == null)
    {
      itemPeer = new DataGridItemAutomationPeer(item, this.OwningDataGrid);
      this._itemPeers.Add(item, itemPeer);
    }
    return itemPeer;
  }

  private DataGridCellItemAutomationPeer GetCellItemPeer(Microsoft.Windows.Controls.DataGridCellInfo cellInfo)
  {
    if (cellInfo.IsValid)
    {
      DataGridItemAutomationPeer itemPeer = this.GetOrCreateItemPeer(cellInfo.Item);
      if (itemPeer != null)
        return itemPeer.GetOrCreateCellItemPeer(cellInfo.Column);
    }
    return (DataGridCellItemAutomationPeer) null;
  }

  internal void RaiseAutomationCellSelectedEvent(Microsoft.Windows.Controls.SelectedCellsChangedEventArgs e)
  {
    if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && this.OwningDataGrid.SelectedCells.Count == 1 && e.AddedCells.Count == 1)
    {
      this.GetCellItemPeer(e.AddedCells[0])?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
    }
    else
    {
      if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
      {
        for (int index = 0; index < e.AddedCells.Count; ++index)
          this.GetCellItemPeer(e.AddedCells[index])?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
      }
      if (!AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
        return;
      for (int index = 0; index < e.RemovedCells.Count; ++index)
        this.GetCellItemPeer(e.RemovedCells[index])?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
    }
  }

  internal void RaiseAutomationRowInvokeEvents(Microsoft.Windows.Controls.DataGridRow row)
  {
    this.GetOrCreateItemPeer(row.Item)?.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
  }

  internal void RaiseAutomationCellInvokeEvents(Microsoft.Windows.Controls.DataGridColumn column, Microsoft.Windows.Controls.DataGridRow row)
  {
    this.GetOrCreateItemPeer(row.Item)?.GetOrCreateCellItemPeer(column)?.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
  }

  internal void RaiseAutomationSelectionEvents(SelectionChangedEventArgs e)
  {
    int count1 = this.OwningDataGrid.SelectedItems.Count;
    int count2 = e.AddedItems.Count;
    if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && count1 == 1 && count2 == 1)
    {
      this.GetOrCreateItemPeer(this.OwningDataGrid.SelectedItem)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
    }
    else
    {
      if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
      {
        for (int index = 0; index < e.AddedItems.Count; ++index)
          this.GetOrCreateItemPeer(e.AddedItems[index])?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
      }
      if (!AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
        return;
      for (int index = 0; index < e.RemovedItems.Count; ++index)
        this.GetOrCreateItemPeer(e.RemovedItems[index])?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
    }
  }

  private void AddSelectedCells(List<IRawElementProviderSimple> cellProviders)
  {
    if (cellProviders == null)
      throw new ArgumentNullException(nameof (cellProviders));
    if (this.OwningDataGrid.SelectedCells == null)
      return;
    foreach (Microsoft.Windows.Controls.DataGridCellInfo selectedCell in (IEnumerable<Microsoft.Windows.Controls.DataGridCellInfo>) this.OwningDataGrid.SelectedCells)
    {
      DataGridItemAutomationPeer itemPeer = this.GetOrCreateItemPeer(selectedCell.Item);
      if (itemPeer != null)
      {
        IRawElementProviderSimple elementProviderSimple = this.ProviderFromPeer((AutomationPeer) itemPeer.GetOrCreateCellItemPeer(selectedCell.Column));
        if (elementProviderSimple != null)
          cellProviders.Add(elementProviderSimple);
      }
    }
  }

  private void AddSelectedRows(List<IRawElementProviderSimple> itemProviders)
  {
    if (itemProviders == null)
      throw new ArgumentNullException(nameof (itemProviders));
    if (this.OwningDataGrid.SelectedItems == null)
      return;
    foreach (object selectedItem in (IEnumerable) this.OwningDataGrid.SelectedItems)
    {
      IRawElementProviderSimple elementProviderSimple = this.ProviderFromPeer((AutomationPeer) this.GetOrCreateItemPeer(selectedItem));
      if (elementProviderSimple != null)
        itemProviders.Add(elementProviderSimple);
    }
  }

  internal static Rect CalculateVisibleBoundingRect(UIElement uiElement)
  {
    Rect visibleBoundingRect = Rect.Empty;
    visibleBoundingRect = new Rect(uiElement.RenderSize);
    for (Visual parent = VisualTreeHelper.GetParent((DependencyObject) uiElement) as Visual; parent != null && visibleBoundingRect != Rect.Empty && visibleBoundingRect.Height != 0.0 && visibleBoundingRect.Width != 0.0; parent = VisualTreeHelper.GetParent((DependencyObject) parent) as Visual)
    {
      Geometry clip = VisualTreeHelper.GetClip(parent);
      if (clip != null)
      {
        GeneralTransform inverse = uiElement.TransformToAncestor(parent).Inverse;
        if (inverse != null)
        {
          Rect bounds = clip.Bounds;
          Rect rect = inverse.TransformBounds(bounds);
          visibleBoundingRect.Intersect(rect);
        }
        else
          visibleBoundingRect = Rect.Empty;
      }
    }
    return visibleBoundingRect;
  }
}
