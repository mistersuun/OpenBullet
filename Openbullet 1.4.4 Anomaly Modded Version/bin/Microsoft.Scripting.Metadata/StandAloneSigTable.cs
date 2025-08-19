// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.StandAloneSigTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class StandAloneSigTable
{
  internal const int TableIndex = 17;
  internal readonly int NumberOfRows;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int SignatureOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal StandAloneSigTable(int numberOfRows, int blobHeapRefSize, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.SignatureOffset = 0;
    this.RowSize = this.SignatureOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetSignature(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.SignatureOffset, this.IsBlobHeapRefSizeSmall);
  }
}
