// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodSpecTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class MethodSpecTable
{
  internal const int TableIndex = 43;
  internal readonly int NumberOfRows;
  private readonly bool IsMethodDefOrRefRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int MethodOffset;
  private readonly int InstantiationOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal MethodSpecTable(
    int numberOfRows,
    int methodDefOrRefRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsMethodDefOrRefRefSizeSmall = methodDefOrRefRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.MethodOffset = 0;
    this.InstantiationOffset = this.MethodOffset + methodDefOrRefRefSize;
    this.RowSize = this.InstantiationOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetGenericMethod(int rowId)
  {
    return MethodDefOrRefTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.MethodOffset, this.IsMethodDefOrRefRefSizeSmall));
  }

  internal uint GetSignature(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.InstantiationOffset, this.IsBlobHeapRefSizeSmall);
  }
}
