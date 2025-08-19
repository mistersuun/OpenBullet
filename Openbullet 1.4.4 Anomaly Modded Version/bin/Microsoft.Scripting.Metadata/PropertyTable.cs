// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.PropertyTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class PropertyTable
{
  internal const int TableIndex = 23;
  internal readonly int NumberOfRows;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int FlagsOffset;
  private readonly int NameOffset;
  private readonly int SignatureOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal PropertyTable(
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
    this.NameOffset = this.FlagsOffset + 2;
    this.SignatureOffset = this.NameOffset + stringHeapRefSize;
    this.RowSize = this.SignatureOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetSignature(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.SignatureOffset, this.IsBlobHeapRefSizeSmall);
  }

  internal PropertyAttributes GetFlags(int rowId)
  {
    return (PropertyAttributes) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }
}
