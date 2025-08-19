// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridCellEditEndingEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridCellEditEndingEventArgs : EventArgs
{
  private bool _cancel;
  private DataGridColumn _dataGridColumn;
  private DataGridRow _dataGridRow;
  private FrameworkElement _editingElement;
  private DataGridEditAction _editAction;

  public DataGridCellEditEndingEventArgs(
    DataGridColumn column,
    DataGridRow row,
    FrameworkElement editingElement,
    DataGridEditAction editAction)
  {
    this._dataGridColumn = column;
    this._dataGridRow = row;
    this._editingElement = editingElement;
    this._editAction = editAction;
  }

  public bool Cancel
  {
    get => this._cancel;
    set => this._cancel = value;
  }

  public DataGridColumn Column => this._dataGridColumn;

  public DataGridRow Row => this._dataGridRow;

  public FrameworkElement EditingElement => this._editingElement;

  public DataGridEditAction EditAction => this._editAction;
}
