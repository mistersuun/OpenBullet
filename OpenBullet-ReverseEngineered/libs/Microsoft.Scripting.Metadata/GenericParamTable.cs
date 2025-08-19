// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.GenericParamTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class GenericParamTable
{
  internal const int TableIndex = 42;
  internal readonly int NumberOfRows;
  private readonly bool IsTypeOrMethodDefRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly int NumberOffset;
  private readonly int FlagsOffset;
  private readonly int OwnerOffset;
  private readonly int NameOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal GenericParamTable(
    int numberOfRows,
    int typeOrMethodDefRefSize,
    int stringHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsTypeOrMethodDefRefSizeSmall = typeOrMethodDefRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.NumberOffset = 0;
    this.FlagsOffset = this.NumberOffset + 2;
    this.OwnerOffset = this.FlagsOffset + 2;
    this.NameOffset = this.OwnerOffset + typeOrMethodDefRefSize;
    this.RowSize = this.NameOffset + stringHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal int GetIndex(int rowId)
  {
    return (int) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.NumberOffset);
  }

  internal GenericParameterAttributes GetFlags(int rowId)
  {
    return (GenericParameterAttributes) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal MetadataToken GetOwner(int rowId)
  {
    return TypeOrMethodDefTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.OwnerOffset, this.IsTypeOrMethodDefRefSizeSmall));
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal int FindGenericParametersForType(int typeDefRowId, out int genericParamCount)
  {
    return this.BinarySearchTag(TypeOrMethodDefTag.ConvertTypeDefRowIdToTag(typeDefRowId), out genericParamCount);
  }

  internal int FindGenericParametersForMethod(int typeDefRowId, out int genericParamCount)
  {
    return this.BinarySearchTag(TypeOrMethodDefTag.ConvertMethodDefRowIdToTag(typeDefRowId), out genericParamCount);
  }

  private int BinarySearchTag(uint searchCodedTag, out int genericParamCount)
  {
    int num1 = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.OwnerOffset, searchCodedTag, this.IsTypeOrMethodDefRefSizeSmall);
    if (num1 == -1)
    {
      genericParamCount = 0;
      return 0;
    }
    int num2 = num1;
    while (num2 > 0 && (int) this.Table.ReadReference((num2 - 1) * this.RowSize + this.OwnerOffset, this.IsTypeOrMethodDefRefSizeSmall) == (int) searchCodedTag)
      --num2;
    int num3 = num1;
    while (num3 + 1 < this.NumberOfRows && (int) this.Table.ReadReference((num3 + 1) * this.RowSize + this.OwnerOffset, this.IsTypeOrMethodDefRefSizeSmall) == (int) searchCodedTag)
      ++num3;
    genericParamCount = num3 - num2 + 1;
    return num2 + 1;
  }
}
