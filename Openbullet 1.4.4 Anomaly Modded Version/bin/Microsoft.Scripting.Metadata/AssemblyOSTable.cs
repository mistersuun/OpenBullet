// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.AssemblyOSTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class AssemblyOSTable
{
  internal const int TableIndex = 34;
  internal readonly int NumberOfRows;
  private readonly int OSPlatformIdOffset;
  private readonly int OSMajorVersionIdOffset;
  private readonly int OSMinorVersionIdOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal AssemblyOSTable(int numberOfRows, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.OSPlatformIdOffset = 0;
    this.OSMajorVersionIdOffset = this.OSPlatformIdOffset + 4;
    this.OSMinorVersionIdOffset = this.OSMajorVersionIdOffset + 4;
    this.RowSize = this.OSMinorVersionIdOffset + 4;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
