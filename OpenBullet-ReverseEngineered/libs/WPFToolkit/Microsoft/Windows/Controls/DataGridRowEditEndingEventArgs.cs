// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridRowEditEndingEventArgs : EventArgs
{
  private bool _cancel;
  private DataGridRow _dataGridRow;
  private DataGridEditAction _editAction;

  public DataGridRowEditEndingEventArgs(DataGridRow row, DataGridEditAction editAction)
  {
    this._dataGridRow = row;
    this._editAction = editAction;
  }

  public bool Cancel
  {
    get => this._cancel;
    set => this._cancel = value;
  }

  public DataGridRow Row => this._dataGridRow;

  public DataGridEditAction EditAction => this._editAction;
}
