// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridRowClipboardEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridRowClipboardEventArgs : EventArgs
{
  private int _startColumnDisplayIndex;
  private int _endColumnDisplayIndex;
  private object _item;
  private bool _isColumnHeadersRow;
  private List<DataGridClipboardCellContent> _clipboardRowContent;
  private int _rowIndexHint = -1;

  public DataGridRowClipboardEventArgs(
    object item,
    int startColumnDisplayIndex,
    int endColumnDisplayIndex,
    bool isColumnHeadersRow)
  {
    this._item = item;
    this._startColumnDisplayIndex = startColumnDisplayIndex;
    this._endColumnDisplayIndex = endColumnDisplayIndex;
    this._isColumnHeadersRow = isColumnHeadersRow;
  }

  internal DataGridRowClipboardEventArgs(
    object item,
    int startColumnDisplayIndex,
    int endColumnDisplayIndex,
    bool isColumnHeadersRow,
    int rowIndexHint)
    : this(item, startColumnDisplayIndex, endColumnDisplayIndex, isColumnHeadersRow)
  {
    this._rowIndexHint = rowIndexHint;
  }

  public object Item => this._item;

  public List<DataGridClipboardCellContent> ClipboardRowContent
  {
    get
    {
      if (this._clipboardRowContent == null)
        this._clipboardRowContent = new List<DataGridClipboardCellContent>();
      return this._clipboardRowContent;
    }
  }

  public string FormatClipboardCellValues(string format)
  {
    StringBuilder sb = new StringBuilder();
    int count = this.ClipboardRowContent.Count;
    for (int index = 0; index < count; ++index)
      ClipboardHelper.FormatCell(this.ClipboardRowContent[index].Content, index == 0, index == count - 1, sb, format);
    return sb.ToString();
  }

  public int StartColumnDisplayIndex => this._startColumnDisplayIndex;

  public int EndColumnDisplayIndex => this._endColumnDisplayIndex;

  public bool IsColumnHeadersRow => this._isColumnHeadersRow;

  internal int RowIndexHint => this._rowIndexHint;
}
