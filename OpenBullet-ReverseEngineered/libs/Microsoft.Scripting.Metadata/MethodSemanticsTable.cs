// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodSemanticsTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class MethodSemanticsTable
{
  internal const int TableIndex = 24;
  internal readonly int NumberOfRows;
  private readonly bool IsMethodTableRowRefSizeSmall;
  private readonly bool IsHasSemanticRefSizeSmall;
  private readonly int SemanticsFlagOffset;
  private readonly int MethodOffset;
  private readonly int AssociationOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal MethodSemanticsTable(
    int numberOfRows,
    int methodTableRowRefSize,
    int hasSemanticRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsMethodTableRowRefSizeSmall = methodTableRowRefSize == 2;
    this.IsHasSemanticRefSizeSmall = hasSemanticRefSize == 2;
    this.SemanticsFlagOffset = 0;
    this.MethodOffset = this.SemanticsFlagOffset + 2;
    this.AssociationOffset = this.MethodOffset + methodTableRowRefSize;
    this.RowSize = this.AssociationOffset + hasSemanticRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MethodSemanticsFlags GetFlags(int rowId)
  {
    return (MethodSemanticsFlags) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.SemanticsFlagOffset);
  }

  internal uint GetMethodRid(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.MethodOffset, this.IsMethodTableRowRefSizeSmall);
  }

  internal MetadataToken GetAssociation(int rowId)
  {
    return HasSemanticsTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.AssociationOffset, this.IsHasSemanticRefSizeSmall));
  }

  internal int FindSemanticMethodsForEvent(int eventRowId, out int methodCount)
  {
    return this.BinarySearchTag(HasSemanticsTag.ConvertEventRowIdToTag(eventRowId), out methodCount);
  }

  internal int FindSemanticMethodsForProperty(int propertyRowId, out int methodCount)
  {
    return this.BinarySearchTag(HasSemanticsTag.ConvertPropertyRowIdToTag(propertyRowId), out methodCount);
  }

  private int BinarySearchTag(uint searchCodedTag, out int methodCount)
  {
    int num1 = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.AssociationOffset, searchCodedTag, this.IsHasSemanticRefSizeSmall);
    if (num1 == -1)
    {
      methodCount = 0;
      return 0;
    }
    int num2 = num1;
    while (num2 > 0 && (int) this.Table.ReadReference((num2 - 1) * this.RowSize + this.AssociationOffset, this.IsHasSemanticRefSizeSmall) == (int) searchCodedTag)
      --num2;
    int num3 = num1;
    while (num3 + 1 < this.NumberOfRows && (int) this.Table.ReadReference((num3 + 1) * this.RowSize + this.AssociationOffset, this.IsHasSemanticRefSizeSmall) == (int) searchCodedTag)
      ++num3;
    methodCount = (int) (ushort) (num3 - num2 + 1);
    return num2 + 1;
  }
}
