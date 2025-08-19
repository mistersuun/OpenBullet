// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridCellClipboardEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridCellClipboardEventArgs : EventArgs
{
  private object _content;
  private object _item;
  private DataGridColumn _column;

  public DataGridCellClipboardEventArgs(object item, DataGridColumn column, object content)
  {
    this._item = item;
    this._column = column;
    this._content = content;
  }

  public object Content
  {
    get => this._content;
    set => this._content = value;
  }

  public object Item => this._item;

  public DataGridColumn Column => this._column;
}
