// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.FieldMarshalTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class FieldMarshalTable
{
  internal const int TableIndex = 13;
  internal readonly int NumberOfRows;
  private readonly int ParentOffset;
  private readonly int NativeTypeOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal FieldMarshalTable(
    int numberOfRows,
    int hasFieldMarshalRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.ParentOffset = 0;
    this.NativeTypeOffset = this.ParentOffset + hasFieldMarshalRefSize;
    this.RowSize = this.NativeTypeOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
