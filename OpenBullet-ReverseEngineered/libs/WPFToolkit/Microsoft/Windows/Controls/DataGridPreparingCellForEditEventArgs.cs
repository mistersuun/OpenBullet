// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridPreparingCellForEditEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridPreparingCellForEditEventArgs : EventArgs
{
  private DataGridColumn _dataGridColumn;
  private DataGridRow _dataGridRow;
  private RoutedEventArgs _editingEventArgs;
  private FrameworkElement _editingElement;

  public DataGridPreparingCellForEditEventArgs(
    DataGridColumn column,
    DataGridRow row,
    RoutedEventArgs editingEventArgs,
    FrameworkElement editingElement)
  {
    this._dataGridColumn = column;
    this._dataGridRow = row;
    this._editingEventArgs = editingEventArgs;
    this._editingElement = editingElement;
  }

  public DataGridColumn Column => this._dataGridColumn;

  public DataGridRow Row => this._dataGridRow;

  public RoutedEventArgs EditingEventArgs => this._editingEventArgs;

  public FrameworkElement EditingElement => this._editingElement;
}
