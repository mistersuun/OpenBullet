// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.EnCMapTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class EnCMapTable
{
  internal const int TableIndex = 31 /*0x1F*/;
  internal readonly int NumberOfRows;
  private readonly int TokenOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal EnCMapTable(int numberOfRows, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.TokenOffset = 0;
    this.RowSize = this.TokenOffset + 4;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
