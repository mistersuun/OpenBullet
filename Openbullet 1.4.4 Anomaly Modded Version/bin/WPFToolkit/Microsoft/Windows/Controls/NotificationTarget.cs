// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.NotificationTarget
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;

#nullable disable
namespace Microsoft.Windows.Controls;

[Flags]
internal enum NotificationTarget
{
  None = 0,
  Cells = 1,
  CellsPresenter = 2,
  Columns = 4,
  ColumnCollection = 8,
  ColumnHeaders = 16, // 0x00000010
  ColumnHeadersPresenter = 32, // 0x00000020
  DataGrid = 64, // 0x00000040
  DetailsPresenter = 128, // 0x00000080
  RefreshCellContent = 256, // 0x00000100
  RowHeaders = 512, // 0x00000200
  Rows = 1024, // 0x00000400
  All = 4095, // 0x00000FFF
}
