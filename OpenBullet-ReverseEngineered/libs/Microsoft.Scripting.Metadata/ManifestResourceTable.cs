// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ManifestResourceTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ManifestResourceTable
{
  internal const int TableIndex = 40;
  internal readonly int NumberOfRows;
  private readonly bool IsImplementationRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly int OffsetOffset;
  private readonly int FlagsOffset;
  private readonly int NameOffset;
  private readonly int ImplementationOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal ManifestResourceTable(
    int numberOfRows,
    int implementationRefSize,
    int stringHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsImplementationRefSizeSmall = implementationRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.OffsetOffset = 0;
    this.FlagsOffset = this.OffsetOffset + 4;
    this.NameOffset = this.FlagsOffset + 4;
    this.ImplementationOffset = this.NameOffset + stringHeapRefSize;
    this.RowSize = this.ImplementationOffset + implementationRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetImplementation(int rowId)
  {
    return ImplementationTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.ImplementationOffset, this.IsImplementationRefSizeSmall));
  }

  internal ManifestResourceAttributes GetFlags(int rowId)
  {
    return (ManifestResourceAttributes) this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal uint GetOffset(int rowId)
  {
    return this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.OffsetOffset);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }
}
