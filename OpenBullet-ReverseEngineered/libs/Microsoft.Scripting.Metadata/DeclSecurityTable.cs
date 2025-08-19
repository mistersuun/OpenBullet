// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.DeclSecurityTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class DeclSecurityTable
{
  internal const int TableIndex = 14;
  internal readonly int NumberOfRows;
  private readonly int ActionOffset;
  private readonly int ParentOffset;
  private readonly int PermissionSetOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal DeclSecurityTable(
    int numberOfRows,
    int hasDeclSecurityRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.ActionOffset = 0;
    this.ParentOffset = this.ActionOffset + 2;
    this.PermissionSetOffset = this.ParentOffset + hasDeclSecurityRefSize;
    this.RowSize = this.PermissionSetOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }
}
