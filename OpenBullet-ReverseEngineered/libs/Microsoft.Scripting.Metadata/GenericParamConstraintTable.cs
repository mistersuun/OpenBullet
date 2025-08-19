// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.GenericParamConstraintTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class GenericParamConstraintTable
{
  internal const int TableIndex = 44;
  internal readonly int NumberOfRows;
  private readonly bool IsGenericParamTableRowRefSizeSmall;
  private readonly bool IsTypeDefOrRefRefSizeSmall;
  private readonly int OwnerOffset;
  private readonly int ConstraintOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal GenericParamConstraintTable(
    int numberOfRows,
    int genericParamTableRowRefSize,
    int typeDefOrRefRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsGenericParamTableRowRefSizeSmall = genericParamTableRowRefSize == 2;
    this.IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
    this.OwnerOffset = 0;
    this.ConstraintOffset = this.OwnerOffset + genericParamTableRowRefSize;
    this.RowSize = this.ConstraintOffset + typeDefOrRefRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal MetadataToken GetConstraint(int rowId)
  {
    return TypeDefOrRefTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.ConstraintOffset, this.IsTypeDefOrRefRefSizeSmall));
  }

  internal MetadataToken GetOwner(int rowId)
  {
    return new MetadataToken(MetadataTokenType.GenericPar, this.Table.ReadReference((rowId - 1) * this.RowSize + this.OwnerOffset, this.IsGenericParamTableRowRefSizeSmall));
  }

  internal int FindConstraintForGenericParam(
    int genericParamRowId,
    out int genericParamConstraintCount)
  {
    int num1 = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.OwnerOffset, (uint) genericParamRowId, this.IsGenericParamTableRowRefSizeSmall);
    if (num1 == -1)
    {
      genericParamConstraintCount = 0;
      return 0;
    }
    int num2 = num1;
    while (num2 > 0 && (long) this.Table.ReadReference((num2 - 1) * this.RowSize + this.OwnerOffset, this.IsGenericParamTableRowRefSizeSmall) == (long) genericParamRowId)
      --num2;
    int num3 = num1;
    while (num3 + 1 < this.NumberOfRows && (long) this.Table.ReadReference((num3 + 1) * this.RowSize + this.OwnerOffset, this.IsGenericParamTableRowRefSizeSmall) == (long) genericParamRowId)
      ++num3;
    genericParamConstraintCount = num3 - num2 + 1;
    return num2 + 1;
  }
}
