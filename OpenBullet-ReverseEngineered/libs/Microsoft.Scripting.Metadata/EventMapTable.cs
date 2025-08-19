// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.EventMapTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class EventMapTable
{
  internal const int TableIndex = 18;
  internal readonly int NumberOfRows;
  private readonly bool IsTypeDefTableRowRefSizeSmall;
  private readonly bool IsEventRefSizeSmall;
  private readonly int ParentOffset;
  private readonly int EventListOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal EventMapTable(
    int numberOfRows,
    int typeDefTableRowRefSize,
    int eventRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
    this.IsEventRefSizeSmall = eventRefSize == 2;
    this.ParentOffset = 0;
    this.EventListOffset = this.ParentOffset + typeDefTableRowRefSize;
    this.RowSize = this.EventListOffset + eventRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal int FindEventMapRowIdFor(int typeDefRowId)
  {
    return this.Table.LinearSearchReference(this.RowSize, this.ParentOffset, (uint) typeDefRowId, this.IsTypeDefTableRowRefSizeSmall) + 1;
  }

  internal uint GetEventListStartFor(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.EventListOffset, this.IsEventRefSizeSmall);
  }

  internal uint GetParent(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.ParentOffset, this.IsTypeDefTableRowRefSizeSmall);
  }

  internal uint FindTypeContainingEvent(int eventDefOrPtrRowId, int eventTableRowCount)
  {
    int rowId = 1 + this.Table.BinarySearchForSlot(this.NumberOfRows, eventTableRowCount, this.RowSize, this.EventListOffset, (uint) eventDefOrPtrRowId, this.IsEventRefSizeSmall);
    return rowId != 0 ? this.GetParent(rowId) : throw new BadImageFormatException();
  }
}
