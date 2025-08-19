// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.NestedClassTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class NestedClassTable
{
  internal const int TableIndex = 41;
  internal readonly int NumberOfRows;
  private readonly bool IsTypeDefTableRowRefSizeSmall;
  private readonly int NestedClassOffset;
  private readonly int EnclosingClassOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal NestedClassTable(
    int numberOfRows,
    int typeDefTableRowRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
    this.NestedClassOffset = 0;
    this.EnclosingClassOffset = this.NestedClassOffset + typeDefTableRowRefSize;
    this.RowSize = this.EnclosingClassOffset + typeDefTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetNestedType(int rowId)
  {
    return new MetadataToken(MetadataTokenType.TypeDef, this.Table.ReadReference((rowId - 1) * this.RowSize + this.NestedClassOffset, this.IsTypeDefTableRowRefSizeSmall));
  }

  internal MetadataToken GetEnclosingType(int rowId)
  {
    return new MetadataToken(MetadataTokenType.TypeDef, this.Table.ReadReference((rowId - 1) * this.RowSize + this.EnclosingClassOffset, this.IsTypeDefTableRowRefSizeSmall));
  }

  internal uint FindParentTypeDefRowId(int nestedTypeRowId)
  {
    int num = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.NestedClassOffset, (uint) nestedTypeRowId, this.IsTypeDefTableRowRefSizeSmall);
    return num == -1 ? 0U : this.Table.ReadReference(num * this.RowSize + this.EnclosingClassOffset, this.IsTypeDefTableRowRefSizeSmall);
  }
}
