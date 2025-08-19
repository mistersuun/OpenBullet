// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.PropertyPtrTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class PropertyPtrTable
{
  internal const int TableIndex = 22;
  internal readonly int NumberOfRows;
  private readonly bool IsPropertyTableRowRefSizeSmall;
  private readonly int PropertyOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal PropertyPtrTable(
    int numberOfRows,
    int propertyTableRowRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsPropertyTableRowRefSizeSmall = propertyTableRowRefSize == 2;
    this.PropertyOffset = 0;
    this.RowSize = this.PropertyOffset + propertyTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetPropertyFor(int rowId)
  {
    return new MetadataToken(MetadataTokenType.Property, this.Table.ReadReference((rowId - 1) * this.RowSize + this.PropertyOffset, this.IsPropertyTableRowRefSizeSmall));
  }
}
