// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.FieldLayoutTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class FieldLayoutTable
{
  internal const int TableIndex = 16 /*0x10*/;
  internal readonly int NumberOfRows;
  private readonly int OffsetOffset;
  private readonly int FieldOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal FieldLayoutTable(
    int numberOfRows,
    int fieldTableRowRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.OffsetOffset = 0;
    this.FieldOffset = this.OffsetOffset + 4;
    this.RowSize = this.FieldOffset + fieldTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
