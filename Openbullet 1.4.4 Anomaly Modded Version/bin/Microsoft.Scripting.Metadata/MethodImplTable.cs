// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodImplTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class MethodImplTable
{
  internal const int TableIndex = 25;
  internal readonly int NumberOfRows;
  private readonly bool IsTypeDefTableRowRefSizeSmall;
  private readonly int ClassOffset;
  private readonly int MethodBodyOffset;
  private readonly int MethodDeclarationOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal MethodImplTable(
    int numberOfRows,
    int typeDefTableRowRefSize,
    int methodDefOrRefRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
    this.ClassOffset = 0;
    this.MethodBodyOffset = this.ClassOffset + typeDefTableRowRefSize;
    this.MethodDeclarationOffset = this.MethodBodyOffset + methodDefOrRefRefSize;
    this.RowSize = this.MethodDeclarationOffset + methodDefOrRefRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal int FindMethodsImplForClass(int typeDefRowId, out ushort methodImplCount)
  {
    methodImplCount = (ushort) 0;
    int num1 = this.Table.BinarySearchReference(this.NumberOfRows, this.RowSize, this.ClassOffset, (uint) typeDefRowId, this.IsTypeDefTableRowRefSizeSmall);
    if (num1 == -1)
      return 0;
    int num2 = num1;
    while (num2 > 0 && (long) this.Table.ReadReference((num2 - 1) * this.RowSize + this.ClassOffset, this.IsTypeDefTableRowRefSizeSmall) == (long) typeDefRowId)
      --num2;
    int num3 = num1;
    while (num3 + 1 < this.NumberOfRows && (long) this.Table.ReadReference((num3 + 1) * this.RowSize + this.ClassOffset, this.IsTypeDefTableRowRefSizeSmall) == (long) typeDefRowId)
      ++num3;
    methodImplCount = (ushort) (num3 - num2 + 1);
    return num2 + 1;
  }
}
