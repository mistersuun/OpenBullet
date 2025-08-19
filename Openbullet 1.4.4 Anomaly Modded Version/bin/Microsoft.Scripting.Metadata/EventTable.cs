// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.EventTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class EventTable
{
  internal const int TableIndex = 20;
  internal int NumberOfRows;
  private readonly bool IsTypeDefOrRefRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly int FlagsOffset;
  private readonly int NameOffset;
  private readonly int EventTypeOffset;
  private int RowSize;
  internal MemoryBlock Table;

  internal EventTable(
    int numberOfRows,
    int typeDefOrRefRefSize,
    int stringHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.FlagsOffset = 0;
    this.NameOffset = this.FlagsOffset + 2;
    this.EventTypeOffset = this.NameOffset + stringHeapRefSize;
    this.RowSize = this.EventTypeOffset + typeDefOrRefRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetEventType(int rowId)
  {
    return TypeDefOrRefTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.EventTypeOffset, this.IsTypeDefOrRefRefSizeSmall));
  }

  internal EventAttributes GetFlags(int rowId)
  {
    return (EventAttributes) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }
}
