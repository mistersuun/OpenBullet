// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridItemAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridItemAutomationPeer : 
  AutomationPeer,
  IInvokeProvider,
  IScrollItemProvider,
  ISelectionItemProvider,
  ISelectionProvider
{
  private object _item;
  private AutomationPeer _dataGridAutomationPeer;
  private Dictionary<DataGridColumn, DataGridCellItemAutomationPeer> _itemPeers = new Dictionary<DataGridColumn, DataGridCellItemAutomationPeer>();

  public DataGridItemAutomationPeer(object item, DataGrid dataGrid)
  {
    if (item == null)
      throw new ArgumentNullException(nameof (item));
    if (dataGrid == null)
      throw new ArgumentNullException(nameof (dataGrid));
    this._item = item;
    this._dataGridAutomationPeer = UIElementAutomationPeer.CreatePeerForElement((UIElement) dataGrid);
  }

  protected override string GetAcceleratorKeyCore()
  {
    return this.OwningRowPeer == null ? string.Empty : this.OwningRowPeer.GetAcceleratorKey();
  }

  protected override string GetAccessKeyCore()
  {
    return this.OwningRowPeer == null ? string.Empty : this.OwningRowPeer.GetAccessKey();
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.DataItem;
  }

  protected override string GetAutomationIdCore()
  {
    return this.OwningRowPeer == null ? string.Empty : this.OwningRowPeer.GetAutomationId();
  }

  protected override Rect GetBoundingRectangleCore()
  {
    return this.OwningRowPeer == null ? new Rect() : this.OwningRowPeer.GetBoundingRectangle();
  }

  protected override List<AutomationPeer> GetChildrenCore()
  {
    AutomationPeer owningRowPeer = (AutomationPeer) this.OwningRowPeer;
    if (owningRowPeer == null)
      return this.GetCellItemPeers();
    owningRowPeer.ResetChildrenCache();
    return owningRowPeer.GetChildren();
  }

  protected override string GetClassNameCore()
  {
    return this.OwningRowPeer == null ? string.Empty : this.OwningRowPeer.GetClassName();
  }

  protected override Point GetClickablePointCore()
  {
    return this.OwningRowPeer == null ? new Point(double.NaN, double.NaN) : this.OwningRowPeer.GetClickablePoint();
  }

  protected override string GetHelpTextCore()
  {
    return this.OwningRowPeer == null ? string.Empty : this.OwningRowPeer.GetHelpText();
  }

  protected override string GetItemStatusCore()
  {
    return this.OwningRowPeer == null ? string.Empty : this.OwningRowPeer.GetItemStatus();
  }

  protected override string GetItemTypeCore()
  {
    return this.OwningRowPeer == null ? string.Empty : this.OwningRowPeer.GetItemType();
  }

  protected override AutomationPeer GetLabeledByCore()
  {
    return this.OwningRowPeer == null ? (AutomationPeer) null : this.OwningRowPeer.GetLabeledBy();
  }

  protected override string GetLocalizedControlTypeCore()
  {
    return this.OwningRowPeer == null ? base.GetLocalizedControlTypeCore() : this.OwningRowPeer.GetLocalizedControlType();
  }

  protected override string GetNameCore() => this._item.ToString();

  protected override AutomationOrientation GetOrientationCore()
  {
    return this.OwningRowPeer == null ? AutomationOrientation.None : this.OwningRowPeer.GetOrientation();
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    switch (patternInterface)
    {
      case PatternInterface.Invoke:
        if (!this.OwningDataGrid.IsReadOnly)
          return (object) this;
        break;
      case PatternInterface.Selection:
      case PatternInterface.ScrollItem:
        return (object) this;
      case PatternInterface.SelectionItem:
        if (this.IsRowSelectionUnit)
          return (object) this;
        break;
    }
    return (object) null;
  }

  protected override bool HasKeyboardFocusCore()
  {
    return this.OwningRowPeer != null && this.OwningRowPeer.HasKeyboardFocus();
  }

  protected override bool IsContentElementCore()
  {
    return this.OwningRowPeer == null || this.OwningRowPeer.IsContentElement();
  }

  protected override bool IsControlElementCore()
  {
    return this.OwningRowPeer == null || this.OwningRowPeer.IsControlElement();
  }

  protected override bool IsEnabledCore()
  {
    return this.OwningRowPeer == null || this.OwningRowPeer.IsEnabled();
  }

  protected override bool IsKeyboardFocusableCore()
  {
    return this.OwningRowPeer != null && this.OwningRowPeer.IsKeyboardFocusable();
  }

  protected override bool IsOffscreenCore()
  {
    return this.OwningRowPeer == null || this.OwningRowPeer.IsOffscreen();
  }

  protected override bool IsPasswordCore()
  {
    return this.OwningRowPeer != null && this.OwningRowPeer.IsPassword();
  }

  protected override bool IsRequiredForFormCore()
  {
    return this.OwningRowPeer != null && this.OwningRowPeer.IsRequiredForForm();
  }

  protected override void SetFocusCore()
  {
    if (this.OwningRowPeer == null || !this.OwningRowPeer.Owner.Focusable)
      return;
    this.OwningRowPeer.SetFocus();
  }

  void IInvokeProvider.Invoke()
  {
    this.EnsureEnabled();
    if (this.OwningRowPeer == null)
      this.OwningDataGrid.ScrollIntoView(this._item);
    bool flag = false;
    if (this.OwningRow != null)
    {
      if (((IEditableCollectionView) this.OwningDataGrid.Items).CurrentEditItem == this._item)
        flag = this.OwningDataGrid.CommitEdit();
      else if (this.OwningDataGrid.Columns.Count > 0)
      {
        DataGridCell cell = this.OwningDataGrid.TryFindCell(this._item, this.OwningDataGrid.Columns[0]);
        if (cell != null)
        {
          this.OwningDataGrid.UnselectAll();
          cell.Focus();
          flag = this.OwningDataGrid.BeginEdit();
        }
      }
    }
    if (!flag && !this.IsNewItemPlaceholder)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGrid_AutomationInvokeFailed));
  }

  void IScrollItemProvider.ScrollIntoView() => this.OwningDataGrid.ScrollIntoView(this._item);

  bool ISelectionItemProvider.IsSelected => this.OwningDataGrid.SelectedItems.Contains(this._item);

  IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
  {
    get => this.ProviderFromPeer(this._dataGridAutomationPeer);
  }

  void ISelectionItemProvider.AddToSelection()
  {
    if (!this.IsRowSelectionUnit)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGridRow_CannotSelectRowWhenCells));
    if (this.OwningDataGrid.SelectedItems.Contains(this._item))
      return;
    this.EnsureEnabled();
    if (this.OwningDataGrid.SelectionMode == DataGridSelectionMode.Single && this.OwningDataGrid.SelectedItems.Count > 0)
      throw new InvalidOperationException();
    if (!this.OwningDataGrid.Items.Contains(this._item))
      return;
    this.OwningDataGrid.SelectedItems.Add(this._item);
  }

  void ISelectionItemProvider.RemoveFromSelection()
  {
    if (!this.IsRowSelectionUnit)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGridRow_CannotSelectRowWhenCells));
    this.EnsureEnabled();
    if (!this.OwningDataGrid.SelectedItems.Contains(this._item))
      return;
    this.OwningDataGrid.SelectedItems.Remove(this._item);
  }

  void ISelectionItemProvider.Select()
  {
    if (!this.IsRowSelectionUnit)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGridRow_CannotSelectRowWhenCells));
    this.EnsureEnabled();
    this.OwningDataGrid.SelectedItem = this._item;
  }

  bool ISelectionProvider.CanSelectMultiple
  {
    get => this.OwningDataGrid.SelectionMode == DataGridSelectionMode.Extended;
  }

  bool ISelectionProvider.IsSelectionRequired => false;

  IRawElementProviderSimple[] ISelectionProvider.GetSelection()
  {
    DataGrid owningDataGrid = this.OwningDataGrid;
    if (owningDataGrid == null)
      return (IRawElementProviderSimple[]) null;
    int rowIndex = owningDataGrid.Items.IndexOf(this._item);
    if (rowIndex > -1 && owningDataGrid.SelectedCellsInternal.Intersects(rowIndex))
    {
      List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
      for (int index = 0; index < this.OwningDataGrid.Columns.Count; ++index)
      {
        if (owningDataGrid.SelectedCellsInternal.Contains(rowIndex, index))
        {
          DataGridCellItemAutomationPeer cellItemPeer = this.GetOrCreateCellItemPeer(owningDataGrid.ColumnFromDisplayIndex(index));
          if (cellItemPeer != null)
            elementProviderSimpleList.Add(this.ProviderFromPeer((AutomationPeer) cellItemPeer));
        }
      }
      if (elementProviderSimpleList.Count > 0)
        return elementProviderSimpleList.ToArray();
    }
    return (IRawElementProviderSimple[]) null;
  }

  internal List<AutomationPeer> GetCellItemPeers()
  {
    List<AutomationPeer> cellItemPeers = new List<AutomationPeer>();
    Dictionary<DataGridColumn, DataGridCellItemAutomationPeer> dictionary = new Dictionary<DataGridColumn, DataGridCellItemAutomationPeer>((IDictionary<DataGridColumn, DataGridCellItemAutomationPeer>) this._itemPeers);
    this._itemPeers.Clear();
    foreach (DataGridColumn column in (Collection<DataGridColumn>) this.OwningDataGrid.Columns)
    {
      DataGridCellItemAutomationPeer itemAutomationPeer = (DataGridCellItemAutomationPeer) null;
      if (!dictionary.TryGetValue(column, out itemAutomationPeer) || itemAutomationPeer == null)
        itemAutomationPeer = new DataGridCellItemAutomationPeer(this._item, column);
      cellItemPeers.Add((AutomationPeer) itemAutomationPeer);
      this._itemPeers.Add(column, itemAutomationPeer);
    }
    return cellItemPeers;
  }

  internal DataGridCellItemAutomationPeer GetOrCreateCellItemPeer(DataGridColumn column)
  {
    DataGridCellItemAutomationPeer cellItemPeer = (DataGridCellItemAutomationPeer) null;
    if (!this._itemPeers.TryGetValue(column, out cellItemPeer) || cellItemPeer == null)
    {
      cellItemPeer = new DataGridCellItemAutomationPeer(this._item, column);
      this._itemPeers.Add(column, cellItemPeer);
    }
    return cellItemPeer;
  }

  internal AutomationPeer RowHeaderAutomationPeer
  {
    get
    {
      return this.OwningRowPeer == null ? (AutomationPeer) null : this.OwningRowPeer.RowHeaderAutomationPeer;
    }
  }

  private void EnsureEnabled()
  {
    if (!this._dataGridAutomationPeer.IsEnabled())
      throw new ElementNotEnabledException();
  }

  private bool IsRowSelectionUnit
  {
    get
    {
      if (this.OwningDataGrid == null)
        return false;
      return this.OwningDataGrid.SelectionUnit == DataGridSelectionUnit.FullRow || this.OwningDataGrid.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader;
    }
  }

  private bool IsNewItemPlaceholder
  {
    get
    {
      return this._item == CollectionView.NewItemPlaceholder || this._item == DataGrid.NewItemPlaceholder;
    }
  }

  private DataGrid OwningDataGrid
  {
    get => (DataGrid) (this._dataGridAutomationPeer as DataGridAutomationPeer).Owner;
  }

  private DataGridRow OwningRow
  {
    get => this.OwningDataGrid.ItemContainerGenerator.ContainerFromItem(this._item) as DataGridRow;
  }

  internal DataGridRowAutomationPeer OwningRowPeer
  {
    get
    {
      DataGridRowAutomationPeer owningRowPeer = (DataGridRowAutomationPeer) null;
      DataGridRow owningRow = this.OwningRow;
      if (owningRow != null)
        owningRowPeer = UIElementAutomationPeer.CreatePeerForElement((UIElement) owningRow) as DataGridRowAutomationPeer;
      return owningRowPeer;
    }
  }
}
