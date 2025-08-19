// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Automation.Peers.DataGridCellItemAutomationPeer
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Windows.Automation.Peers;

public sealed class DataGridCellItemAutomationPeer : 
  AutomationPeer,
  ITableItemProvider,
  IGridItemProvider,
  IInvokeProvider,
  IScrollItemProvider,
  ISelectionItemProvider
{
  private object _item;
  private DataGridColumn _column;

  public DataGridCellItemAutomationPeer(object item, DataGridColumn dataGridColumn)
  {
    if (item == null)
      throw new ArgumentNullException(nameof (item));
    if (dataGridColumn == null)
      throw new ArgumentNullException(nameof (dataGridColumn));
    this._item = item;
    this._column = dataGridColumn;
  }

  protected override string GetAcceleratorKeyCore()
  {
    return this.OwningCellPeer == null ? string.Empty : this.OwningCellPeer.GetAcceleratorKey();
  }

  protected override string GetAccessKeyCore()
  {
    return this.OwningCellPeer == null ? string.Empty : this.OwningCellPeer.GetAccessKey();
  }

  protected override AutomationControlType GetAutomationControlTypeCore()
  {
    return AutomationControlType.Custom;
  }

  protected override string GetAutomationIdCore()
  {
    return this.OwningCellPeer == null ? string.Empty : this.OwningCellPeer.GetAutomationId();
  }

  protected override Rect GetBoundingRectangleCore()
  {
    return this.OwningCellPeer == null ? new Rect() : this.OwningCellPeer.GetBoundingRectangle();
  }

  protected override List<AutomationPeer> GetChildrenCore()
  {
    AutomationPeer owningCellPeer = (AutomationPeer) this.OwningCellPeer;
    if (owningCellPeer == null)
      return (List<AutomationPeer>) null;
    owningCellPeer.ResetChildrenCache();
    return owningCellPeer.GetChildren();
  }

  protected override string GetClassNameCore()
  {
    return this.OwningCellPeer == null ? string.Empty : this.OwningCellPeer.GetClassName();
  }

  protected override Point GetClickablePointCore()
  {
    return this.OwningCellPeer == null ? new Point(double.NaN, double.NaN) : this.OwningCellPeer.GetClickablePoint();
  }

  protected override string GetHelpTextCore()
  {
    return this.OwningCellPeer == null ? string.Empty : this.OwningCellPeer.GetHelpText();
  }

  protected override string GetItemStatusCore()
  {
    return this.OwningCellPeer == null ? string.Empty : this.OwningCellPeer.GetItemStatus();
  }

  protected override string GetItemTypeCore()
  {
    return this.OwningCellPeer == null ? string.Empty : this.OwningCellPeer.GetItemType();
  }

  protected override AutomationPeer GetLabeledByCore()
  {
    return this.OwningCellPeer == null ? (AutomationPeer) null : this.OwningCellPeer.GetLabeledBy();
  }

  protected override string GetLocalizedControlTypeCore()
  {
    return this.OwningCellPeer == null ? base.GetLocalizedControlTypeCore() : this.OwningCellPeer.GetLocalizedControlType();
  }

  protected override string GetNameCore()
  {
    return Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGridCellItemAutomationPeer_NameCoreFormat, this._item, (object) this._column.DisplayIndex);
  }

  protected override AutomationOrientation GetOrientationCore()
  {
    return this.OwningCellPeer == null ? AutomationOrientation.None : this.OwningCellPeer.GetOrientation();
  }

  public override object GetPattern(PatternInterface patternInterface)
  {
    switch (patternInterface)
    {
      case PatternInterface.Invoke:
        if (!this.OwningDataGrid.IsReadOnly && !this._column.IsReadOnly)
          return (object) this;
        break;
      case PatternInterface.ScrollItem:
      case PatternInterface.GridItem:
      case PatternInterface.TableItem:
        return (object) this;
      case PatternInterface.SelectionItem:
        if (this.IsCellSelectionUnit)
          return (object) this;
        break;
    }
    return (object) null;
  }

  protected override bool HasKeyboardFocusCore()
  {
    return this.OwningCellPeer != null && this.OwningCellPeer.HasKeyboardFocus();
  }

  protected override bool IsContentElementCore()
  {
    return this.OwningCellPeer == null || this.OwningCellPeer.IsContentElement();
  }

  protected override bool IsControlElementCore()
  {
    return this.OwningCellPeer == null || this.OwningCellPeer.IsControlElement();
  }

  protected override bool IsEnabledCore()
  {
    return this.OwningCellPeer == null || this.OwningCellPeer.IsEnabled();
  }

  protected override bool IsKeyboardFocusableCore()
  {
    return this.OwningCellPeer != null && this.OwningCellPeer.IsKeyboardFocusable();
  }

  protected override bool IsOffscreenCore()
  {
    return this.OwningCellPeer == null || this.OwningCellPeer.IsOffscreen();
  }

  protected override bool IsPasswordCore()
  {
    return this.OwningCellPeer != null && this.OwningCellPeer.IsPassword();
  }

  protected override bool IsRequiredForFormCore()
  {
    return this.OwningCellPeer != null && this.OwningCellPeer.IsRequiredForForm();
  }

  protected override void SetFocusCore()
  {
    if (this.OwningCellPeer == null || !this.OwningCellPeer.Owner.Focusable)
      return;
    this.OwningCellPeer.SetFocus();
  }

  int IGridItemProvider.Column => this.OwningDataGrid.Columns.IndexOf(this._column);

  int IGridItemProvider.ColumnSpan => 1;

  IRawElementProviderSimple IGridItemProvider.ContainingGrid => this.ContainingGrid;

  int IGridItemProvider.Row => this.OwningDataGrid.Items.IndexOf(this._item);

  int IGridItemProvider.RowSpan => 1;

  IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
  {
    if (this.OwningDataGrid != null && (this.OwningDataGrid.HeadersVisibility & DataGridHeadersVisibility.Column) == DataGridHeadersVisibility.Column && this.OwningDataGrid.ColumnHeadersPresenter != null && this.OwningDataGrid.ColumnHeadersPresenter.ItemContainerGenerator.ContainerFromIndex(this.OwningDataGrid.Columns.IndexOf(this._column)) is DataGridColumnHeader element)
    {
      AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) element);
      if (peerForElement != null)
        return new List<IRawElementProviderSimple>(1)
        {
          this.ProviderFromPeer(peerForElement)
        }.ToArray();
    }
    return (IRawElementProviderSimple[]) null;
  }

  IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
  {
    if (this.OwningDataGrid != null && (this.OwningDataGrid.HeadersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row)
    {
      DataGridItemAutomationPeer itemPeer = (UIElementAutomationPeer.CreatePeerForElement((UIElement) this.OwningDataGrid) as DataGridAutomationPeer).GetOrCreateItemPeer(this._item);
      if (itemPeer != null)
      {
        AutomationPeer headerAutomationPeer = itemPeer.RowHeaderAutomationPeer;
        if (headerAutomationPeer != null)
          return new List<IRawElementProviderSimple>(1)
          {
            this.ProviderFromPeer(headerAutomationPeer)
          }.ToArray();
      }
    }
    return (IRawElementProviderSimple[]) null;
  }

  void IInvokeProvider.Invoke()
  {
    if (this.OwningDataGrid.IsReadOnly || this._column.IsReadOnly)
      return;
    this.EnsureEnabled();
    bool flag = false;
    if (this.OwningCell == null)
      this.OwningDataGrid.ScrollIntoView(this._item, this._column);
    DataGridCell owningCell = this.OwningCell;
    if (owningCell != null)
    {
      if (!owningCell.IsEditing)
      {
        if (!owningCell.IsKeyboardFocusWithin)
          owningCell.Focus();
        this.OwningDataGrid.HandleSelectionForCellInput(owningCell, false, false, false);
        flag = this.OwningDataGrid.BeginEdit();
      }
      else
        flag = true;
    }
    if (!flag && !this.IsNewItemPlaceholder)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGrid_AutomationInvokeFailed));
  }

  void IScrollItemProvider.ScrollIntoView()
  {
    this.OwningDataGrid.ScrollIntoView(this._item, this._column);
  }

  bool ISelectionItemProvider.IsSelected
  {
    get
    {
      return this.OwningDataGrid.SelectedCellsInternal.Contains(new DataGridCellInfo(this._item, this._column));
    }
  }

  IRawElementProviderSimple ISelectionItemProvider.SelectionContainer => this.ContainingGrid;

  void ISelectionItemProvider.AddToSelection()
  {
    if (!this.IsCellSelectionUnit)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGrid_CannotSelectCell));
    DataGridCellInfo cell = new DataGridCellInfo(this._item, this._column);
    if (this.OwningDataGrid.SelectedCellsInternal.Contains(cell))
      return;
    this.EnsureEnabled();
    if (this.OwningDataGrid.SelectionMode == DataGridSelectionMode.Single && this.OwningDataGrid.SelectedCells.Count > 0)
      throw new InvalidOperationException();
    this.OwningDataGrid.SelectedCellsInternal.Add(cell);
  }

  void ISelectionItemProvider.RemoveFromSelection()
  {
    if (!this.IsCellSelectionUnit)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGrid_CannotSelectCell));
    this.EnsureEnabled();
    DataGridCellInfo cell = new DataGridCellInfo(this._item, this._column);
    if (!this.OwningDataGrid.SelectedCellsInternal.Contains(cell))
      return;
    this.OwningDataGrid.SelectedCellsInternal.Remove(cell);
  }

  void ISelectionItemProvider.Select()
  {
    if (!this.IsCellSelectionUnit)
      throw new InvalidOperationException(Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DataGrid_CannotSelectCell));
    this.EnsureEnabled();
    this.OwningDataGrid.SelectOnlyThisCell(new DataGridCellInfo(this._item, this._column));
  }

  private void EnsureEnabled()
  {
    if (!this.OwningDataGrid.IsEnabled)
      throw new ElementNotEnabledException();
  }

  private bool IsCellSelectionUnit
  {
    get
    {
      if (this.OwningDataGrid == null)
        return false;
      return this.OwningDataGrid.SelectionUnit == DataGridSelectionUnit.Cell || this.OwningDataGrid.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader;
    }
  }

  private bool IsNewItemPlaceholder
  {
    get
    {
      return this._item == CollectionView.NewItemPlaceholder || this._item == DataGrid.NewItemPlaceholder;
    }
  }

  private DataGrid OwningDataGrid => this._column.DataGridOwner;

  private DataGridCell OwningCell => this.OwningDataGrid.TryFindCell(this._item, this._column);

  internal DataGridCellAutomationPeer OwningCellPeer
  {
    get
    {
      DataGridCellAutomationPeer owningCellPeer = (DataGridCellAutomationPeer) null;
      DataGridCell owningCell = this.OwningCell;
      if (owningCell != null)
      {
        owningCellPeer = UIElementAutomationPeer.CreatePeerForElement((UIElement) owningCell) as DataGridCellAutomationPeer;
        owningCellPeer.EventsSource = (AutomationPeer) this;
      }
      return owningCellPeer;
    }
  }

  private IRawElementProviderSimple ContainingGrid
  {
    get
    {
      AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) this.OwningDataGrid);
      return peerForElement != null ? this.ProviderFromPeer(peerForElement) : (IRawElementProviderSimple) null;
    }
  }
}
