// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.FieldPtrTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class FieldPtrTable
{
  internal const int TableIndex = 3;
  internal readonly int NumberOfRows;
  private readonly bool IsFieldTableRowRefSizeSmall;
  private readonly int FieldOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal FieldPtrTable(int numberOfRows, int fieldTableRowRefSize, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsFieldTableRowRefSizeSmall = fieldTableRowRefSize == 2;
    this.FieldOffset = 0;
    this.RowSize = this.FieldOffset + fieldTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetFieldFor(int rowId)
  {
    return new MetadataToken(MetadataTokenType.FieldDef, this.Table.ReadReference((rowId - 1) * this.RowSize + this.FieldOffset, this.IsFieldTableRowRefSizeSmall));
  }
}
