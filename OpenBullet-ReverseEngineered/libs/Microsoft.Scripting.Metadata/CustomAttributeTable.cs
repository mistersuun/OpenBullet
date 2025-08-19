// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.CustomAttributeTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class CustomAttributeTable
{
  internal const int TableIndex = 12;
  internal readonly int NumberOfRows;
  private readonly bool IsHasCustomAttributeRefSizeSmall;
  private readonly bool IsCustomAttriubuteTypeRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int ParentOffset;
  private readonly int TypeOffset;
  private readonly int ValueOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal CustomAttributeTable(
    int numberOfRows,
    int hasCustomAttributeRefSize,
    int customAttributeTypeRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsHasCustomAttributeRefSizeSmall = hasCustomAttributeRefSize == 2;
    this.IsCustomAttriubuteTypeRefSizeSmall = customAttributeTypeRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.ParentOffset = 0;
    this.TypeOffset = this.ParentOffset + hasCustomAttributeRefSize;
    this.ValueOffset = this.TypeOffset + customAttributeTypeRefSize;
    this.RowSize = this.ValueOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetParent(int rowId)
  {
    return HasCustomAttributeTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.ParentOffset, this.IsHasCustomAttributeRefSizeSmall));
  }

  internal MetadataToken GetConstructor(int rowId)
  {
    return CustomAttributeTypeTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.TypeOffset, this.IsCustomAttriubuteTypeRefSizeSmall));
  }

  internal uint GetValue(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.ValueOffset, this.IsBlobHeapRefSizeSmall);
  }

  internal int FindCustomAttributesForToken(MetadataToken token, out int customAttributeCount)
  {
    return this.BinarySearchTag(HasCustomAttributeTag.ConvertToTag(token), out customAttributeCount);
  }

  private int BinarySearchTag(uint searchCodedTag, out int customAttributeCount)
  {
    int num1 = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.ParentOffset, searchCodedTag, this.IsHasCustomAttributeRefSizeSmall);
    if (num1 == -1)
    {
      customAttributeCount = 0;
      return 0;
    }
    int num2 = num1;
    while (num2 > 0 && (int) this.Table.ReadReference((num2 - 1) * this.RowSize + this.ParentOffset, this.IsHasCustomAttributeRefSizeSmall) == (int) searchCodedTag)
      --num2;
    int num3 = num1;
    while (num3 + 1 < this.NumberOfRows && (int) this.Table.ReadReference((num3 + 1) * this.RowSize + this.ParentOffset, this.IsHasCustomAttributeRefSizeSmall) == (int) searchCodedTag)
      ++num3;
    customAttributeCount = num3 - num2 + 1;
    return num2 + 1;
  }
}
