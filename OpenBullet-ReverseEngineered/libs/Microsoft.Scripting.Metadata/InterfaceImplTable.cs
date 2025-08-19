// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.InterfaceImplTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class InterfaceImplTable
{
  internal const int TableIndex = 9;
  internal readonly int NumberOfRows;
  private readonly bool IsTypeDefTableRowRefSizeSmall;
  private readonly bool IsTypeDefOrRefRefSizeSmall;
  private readonly int ClassOffset;
  private readonly int InterfaceOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal InterfaceImplTable(
    int numberOfRows,
    int typeDefTableRowRefSize,
    int typeDefOrRefRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
    this.IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
    this.ClassOffset = 0;
    this.InterfaceOffset = this.ClassOffset + typeDefTableRowRefSize;
    this.RowSize = this.InterfaceOffset + typeDefOrRefRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal int FindInterfaceImplForType(int typeDefRowId, out int interfaceCount)
  {
    int num1 = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.ClassOffset, (uint) typeDefRowId, this.IsTypeDefOrRefRefSizeSmall);
    if (num1 == -1)
    {
      interfaceCount = 0;
      return 0;
    }
    int num2 = num1;
    while (num2 > 0 && (long) this.Table.ReadReference((num2 - 1) * this.RowSize + this.ClassOffset, this.IsTypeDefOrRefRefSizeSmall) == (long) typeDefRowId)
      --num2;
    int num3 = num1;
    while (num3 + 1 < this.NumberOfRows && (long) this.Table.ReadReference((num3 + 1) * this.RowSize + this.ClassOffset, this.IsTypeDefOrRefRefSizeSmall) == (long) typeDefRowId)
      ++num3;
    interfaceCount = num3 - num2 + 1;
    return num2 + 1;
  }

  internal uint GetClass(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.ClassOffset, this.IsTypeDefTableRowRefSizeSmall);
  }

  internal MetadataToken GetInterface(int rowId)
  {
    return TypeDefOrRefTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.InterfaceOffset, this.IsTypeDefOrRefRefSizeSmall));
  }
}
