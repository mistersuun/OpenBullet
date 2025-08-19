// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridColumnReorderingEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridColumnReorderingEventArgs(DataGridColumn dataGridColumn) : 
  DataGridColumnEventArgs(dataGridColumn)
{
  private bool _cancel;
  private Control _dropLocationIndicator;
  private Control _dragIndicator;

  public bool Cancel
  {
    get => this._cancel;
    set => this._cancel = value;
  }

  public Control DropLocationIndicator
  {
    get => this._dropLocationIndicator;
    set => this._dropLocationIndicator = value;
  }

  public Control DragIndicator
  {
    get => this._dragIndicator;
    set => this._dragIndicator = value;
  }
}
