// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ModuleTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ModuleTable
{
  internal const int TableIndex = 0;
  internal readonly int NumberOfRows;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly bool IsGUIDHeapRefSizeSmall;
  private readonly int GenerationOffset;
  private readonly int NameOffset;
  private readonly int MVIdOffset;
  private readonly int EnCIdOffset;
  private readonly int EnCBaseIdOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal ModuleTable(
    int numberOfRows,
    int stringHeapRefSize,
    int guidHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.IsGUIDHeapRefSizeSmall = guidHeapRefSize == 2;
    this.GenerationOffset = 0;
    this.NameOffset = this.GenerationOffset + 2;
    this.MVIdOffset = this.NameOffset + stringHeapRefSize;
    this.EnCIdOffset = this.MVIdOffset + guidHeapRefSize;
    this.EnCBaseIdOffset = this.EnCIdOffset + guidHeapRefSize;
    this.RowSize = this.EnCBaseIdOffset + guidHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetMVId(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.MVIdOffset, this.IsGUIDHeapRefSizeSmall);
  }
}
