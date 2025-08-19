// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MemberRefTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class MemberRefTable
{
  internal const int TableIndex = 10;
  internal readonly int NumberOfRows;
  private readonly bool IsMemberRefParentRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int ClassOffset;
  private readonly int NameOffset;
  private readonly int SignatureOffset;
  private readonly int RowSize;
  internal MemoryBlock Table;

  internal MemberRefTable(
    int numberOfRows,
    int memberRefParentRefSize,
    int stringHeapRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsMemberRefParentRefSizeSmall = memberRefParentRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.ClassOffset = 0;
    this.NameOffset = this.ClassOffset + memberRefParentRefSize;
    this.SignatureOffset = this.NameOffset + stringHeapRefSize;
    this.RowSize = this.SignatureOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetClass(int rowId)
  {
    return MemberRefParentTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.ClassOffset, this.IsMemberRefParentRefSizeSmall));
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetSignature(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.SignatureOffset, this.IsBlobHeapRefSizeSmall);
  }
}
