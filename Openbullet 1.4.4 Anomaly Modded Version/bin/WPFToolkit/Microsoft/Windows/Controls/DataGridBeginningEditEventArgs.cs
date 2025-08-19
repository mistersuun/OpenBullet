// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridBeginningEditEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridBeginningEditEventArgs : EventArgs
{
  private bool _cancel;
  private DataGridColumn _dataGridColumn;
  private DataGridRow _dataGridRow;
  private RoutedEventArgs _editingEventArgs;

  public DataGridBeginningEditEventArgs(
    DataGridColumn column,
    DataGridRow row,
    RoutedEventArgs editingEventArgs)
  {
    this._dataGridColumn = column;
    this._dataGridRow = row;
    this._editingEventArgs = editingEventArgs;
  }

  public bool Cancel
  {
    get => this._cancel;
    set => this._cancel = value;
  }

  public DataGridColumn Column => this._dataGridColumn;

  public DataGridRow Row => this._dataGridRow;

  public RoutedEventArgs EditingEventArgs => this._editingEventArgs;
}
