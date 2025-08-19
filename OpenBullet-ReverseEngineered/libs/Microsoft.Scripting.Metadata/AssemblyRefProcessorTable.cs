// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.AssemblyRefProcessorTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class AssemblyRefProcessorTable
{
  internal const int TableIndex = 36;
  internal readonly int NumberOfRows;
  private readonly int ProcessorOffset;
  private readonly int AssemblyRefOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal AssemblyRefProcessorTable(
    int numberOfRows,
    int assembyRefTableRowRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.ProcessorOffset = 0;
    this.AssemblyRefOffset = this.ProcessorOffset + 4;
    this.RowSize = this.AssemblyRefOffset + assembyRefTableRowRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
