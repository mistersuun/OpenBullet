// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ConstantTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ConstantTable
{
  internal const int TableIndex = 11;
  internal readonly int NumberOfRows;
  private readonly bool IsHasConstantRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int TypeOffset;
  private readonly int ParentOffset;
  private readonly int ValueOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal ConstantTable(
    int numberOfRows,
    int hasConstantRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsHasConstantRefSizeSmall = hasConstantRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.TypeOffset = 0;
    this.ParentOffset = this.TypeOffset + 1 + 1;
    this.ValueOffset = this.ParentOffset + hasConstantRefSize;
    this.RowSize = this.ValueOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal int GetConstantRowId(MetadataToken parentToken)
  {
    return this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.ParentOffset, HasConstantTag.ConvertToTag(parentToken), this.IsHasConstantRefSizeSmall) + 1;
  }

  internal uint GetValue(int rowId, out ElementType type)
  {
    int num = (rowId - 1) * this.RowSize;
    type = (ElementType) this.Table.ReadByte(num + this.TypeOffset);
    return this.Table.ReadReference(num + this.ValueOffset, this.IsBlobHeapRefSizeSmall);
  }
}
