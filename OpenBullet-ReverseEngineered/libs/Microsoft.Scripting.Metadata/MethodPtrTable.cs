// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodPtrTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class MethodPtrTable
{
  internal const int TableIndex = 5;
  internal readonly int NumberOfRows;
  private readonly bool IsMethodTableRowRefSizeSmall;
  private readonly int MethodOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal MethodPtrTable(
    int numberOfRows,
    int methodTableRowRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsMethodTableRowRefSizeSmall = methodTableRowRefSize == 2;
    this.MethodOffset = 0;
    this.RowSize = this.MethodOffset + methodTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetMethodFor(int rowId)
  {
    return new MetadataToken(MetadataTokenType.MethodDef, this.Table.ReadReference((rowId - 1) * this.RowSize + this.MethodOffset, this.IsMethodTableRowRefSizeSmall));
  }
}
