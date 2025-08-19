// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ParamPtrTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ParamPtrTable
{
  internal const int TableIndex = 7;
  internal readonly int NumberOfRows;
  private readonly bool IsParamTableRowRefSizeSmall;
  private readonly int ParamOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal ParamPtrTable(int numberOfRows, int paramTableRowRefSize, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsParamTableRowRefSizeSmall = paramTableRowRefSize == 2;
    this.ParamOffset = 0;
    this.RowSize = this.ParamOffset + paramTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetParamFor(int rowId)
  {
    return new MetadataToken(MetadataTokenType.ParamDef, this.Table.ReadReference((rowId - 1) * this.RowSize + this.ParamOffset, this.IsParamTableRowRefSizeSmall));
  }
}
