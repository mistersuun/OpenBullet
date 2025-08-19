// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ImplMapTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ImplMapTable
{
  internal const int TableIndex = 28;
  internal readonly int NumberOfRows;
  private readonly bool IsMemberForwardRowRefSizeSmall;
  private readonly int FlagsOffset;
  private readonly int MemberForwardedOffset;
  private readonly int ImportNameOffset;
  private readonly int ImportScopeOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal ImplMapTable(
    int numberOfRows,
    int moduleRefTableRowRefSize,
    int memberForwardedRefSize,
    int stringHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsMemberForwardRowRefSizeSmall = memberForwardedRefSize == 2;
    this.FlagsOffset = 0;
    this.MemberForwardedOffset = this.FlagsOffset + 2;
    this.ImportNameOffset = this.MemberForwardedOffset + memberForwardedRefSize;
    this.ImportScopeOffset = this.ImportNameOffset + stringHeapRefSize;
    this.RowSize = this.ImportScopeOffset + moduleRefTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal int FindImplForMethod(int methodRowId)
  {
    return this.BinarySearchTag(MemberForwardedTag.ConvertMethodDefRowIdToTag(methodRowId));
  }

  private int BinarySearchTag(uint searchCodedTag)
  {
    return this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.MemberForwardedOffset, searchCodedTag, this.IsMemberForwardRowRefSizeSmall) + 1;
  }
}
