// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.EventPtrTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class EventPtrTable
{
  internal const int TableIndex = 19;
  internal readonly int NumberOfRows;
  private readonly bool IsEventTableRowRefSizeSmall;
  private readonly int EventOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal EventPtrTable(int numberOfRows, int eventTableRowRefSize, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsEventTableRowRefSizeSmall = eventTableRowRefSize == 2;
    this.EventOffset = 0;
    this.RowSize = this.EventOffset + eventTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetEventFor(int rowId)
  {
    return new MetadataToken(MetadataTokenType.Event, this.Table.ReadReference((rowId - 1) * this.RowSize + this.EventOffset, this.IsEventTableRowRefSizeSmall));
  }
}
