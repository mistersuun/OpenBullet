// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.AssemblyProcessorTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class AssemblyProcessorTable
{
  internal const int TableIndex = 33;
  internal readonly int NumberOfRows;
  private readonly int ProcessorOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal AssemblyProcessorTable(int numberOfRows, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.ProcessorOffset = 0;
    this.RowSize = this.ProcessorOffset + 4;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
