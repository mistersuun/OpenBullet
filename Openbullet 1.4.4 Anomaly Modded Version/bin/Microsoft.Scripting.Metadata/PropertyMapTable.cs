// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.PropertyMapTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class PropertyMapTable
{
  internal const int TableIndex = 21;
  internal readonly int NumberOfRows;
  private readonly bool IsTypeDefTableRowRefSizeSmall;
  private readonly bool IsPropertyRefSizeSmall;
  private readonly int ParentOffset;
  private readonly int PropertyListOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal PropertyMapTable(
    int numberOfRows,
    int typeDefTableRowRefSize,
    int propertyRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
    this.IsPropertyRefSizeSmall = propertyRefSize == 2;
    this.ParentOffset = 0;
    this.PropertyListOffset = this.ParentOffset + typeDefTableRowRefSize;
    this.RowSize = this.PropertyListOffset + propertyRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal int FindPropertyMapRowIdFor(int typeDefRowId)
  {
    return this.Table.LinearSearchReference(this.RowSize, this.ParentOffset, (uint) typeDefRowId, this.IsTypeDefTableRowRefSizeSmall) + 1;
  }

  internal uint GetFirstPropertyRid(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.PropertyListOffset, this.IsPropertyRefSizeSmall);
  }

  internal uint GetParent(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.ParentOffset, this.IsTypeDefTableRowRefSizeSmall);
  }

  internal uint FindTypeContainingProperty(int propertyDefOrPtrRowId, int propertyTableRowCount)
  {
    int rowId = 1 + this.Table.BinarySearchForSlot(this.NumberOfRows, propertyTableRowCount, this.RowSize, this.PropertyListOffset, (uint) propertyDefOrPtrRowId, this.IsPropertyRefSizeSmall);
    return rowId != 0 ? this.GetParent(rowId) : throw new BadImageFormatException();
  }
}
