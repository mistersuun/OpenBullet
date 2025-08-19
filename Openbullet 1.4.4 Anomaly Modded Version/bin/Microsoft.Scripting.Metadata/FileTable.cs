// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.FileTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class FileTable
{
  internal const int TableIndex = 38;
  internal readonly int NumberOfRows;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int FlagsOffset;
  private readonly int NameOffset;
  private readonly int HashValueOffset;
  private readonly int RowSize;
  public readonly MemoryBlock Table;

  internal FileTable(
    int numberOfRows,
    int stringHeapRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.FlagsOffset = 0;
    this.NameOffset = this.FlagsOffset + 4;
    this.HashValueOffset = this.NameOffset + stringHeapRefSize;
    this.RowSize = this.HashValueOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetHashValue(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.HashValueOffset, this.IsBlobHeapRefSizeSmall);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal AssemblyFileAttributes GetFlags(int rowId)
  {
    return (AssemblyFileAttributes) this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.FlagsOffset);
  }
}
