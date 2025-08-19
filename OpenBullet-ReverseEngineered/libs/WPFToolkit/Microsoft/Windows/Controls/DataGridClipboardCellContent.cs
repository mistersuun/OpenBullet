// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridClipboardCellContent
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

#nullable disable
namespace Microsoft.Windows.Controls;

public struct DataGridClipboardCellContent(object item, DataGridColumn column, object content)
{
  private object _item = item;
  private DataGridColumn _column = column;
  private object _content = content;

  public object Item => this._item;

  public DataGridColumn Column => this._column;

  public object Content => this._content;

  public override bool Equals(object data)
  {
    return data is DataGridClipboardCellContent clipboardCellContent && this._column == clipboardCellContent._column && this._content == clipboardCellContent._content && this._item == clipboardCellContent._item;
  }

  public override int GetHashCode()
  {
    return (this._column == null ? 0 : this._column.GetHashCode()) ^ (this._content == null ? 0 : this._content.GetHashCode()) ^ (this._item == null ? 0 : this._item.GetHashCode());
  }

  public static bool operator ==(
    DataGridClipboardCellContent clipboardCellContent1,
    DataGridClipboardCellContent clipboardCellContent2)
  {
    return clipboardCellContent1._column == clipboardCellContent2._column && clipboardCellContent1._content == clipboardCellContent2._content && clipboardCellContent1._item == clipboardCellContent2._item;
  }

  public static bool operator !=(
    DataGridClipboardCellContent clipboardCellContent1,
    DataGridClipboardCellContent clipboardCellContent2)
  {
    return clipboardCellContent1._column != clipboardCellContent2._column || clipboardCellContent1._content != clipboardCellContent2._content || clipboardCellContent1._item != clipboardCellContent2._item;
  }
}
