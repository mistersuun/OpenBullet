// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ClassLayoutTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ClassLayoutTable
{
  internal const int TableIndex = 15;
  internal int NumberOfRows;
  private readonly int PackagingSizeOffset;
  private readonly int ClassSizeOffset;
  private readonly int ParentOffset;
  private int RowSize;
  internal MemoryBlock Table;

  internal ClassLayoutTable(
    int numberOfRows,
    int typeDefTableRowRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.PackagingSizeOffset = 0;
    this.ClassSizeOffset = this.PackagingSizeOffset + 2;
    this.ParentOffset = this.ClassSizeOffset + 4;
    this.RowSize = this.ParentOffset + typeDefTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
