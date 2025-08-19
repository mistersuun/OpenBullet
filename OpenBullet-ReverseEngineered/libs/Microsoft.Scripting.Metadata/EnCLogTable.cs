// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.EnCLogTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class EnCLogTable
{
  internal const int TableIndex = 30;
  internal readonly int NumberOfRows;
  private readonly int TokenOffset;
  private readonly int FuncCodeOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal EnCLogTable(int numberOfRows, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.TokenOffset = 0;
    this.FuncCodeOffset = this.TokenOffset + 4;
    this.RowSize = this.FuncCodeOffset + 4;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
