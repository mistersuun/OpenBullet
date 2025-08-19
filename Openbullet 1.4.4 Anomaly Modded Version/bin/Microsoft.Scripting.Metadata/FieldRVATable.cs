// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.FieldRVATable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class FieldRVATable
{
  internal const int TableIndex = 29;
  internal readonly int NumberOfRows;
  private readonly bool IsFieldTableRowRefSizeSmall;
  private readonly int RVAOffset;
  private readonly int FieldOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal FieldRVATable(int numberOfRows, int fieldTableRowRefSize, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsFieldTableRowRefSizeSmall = fieldTableRowRefSize == 2;
    this.RVAOffset = 0;
    this.FieldOffset = this.RVAOffset + 4;
    this.RowSize = this.FieldOffset + fieldTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetFieldRVA(int fieldDefRowId)
  {
    int num = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.FieldOffset, (uint) fieldDefRowId, this.IsFieldTableRowRefSizeSmall);
    return num == -1 ? 0U : this.Table.ReadUInt32(num * this.RowSize + this.RVAOffset);
  }
}
