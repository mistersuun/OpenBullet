// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeRefTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class TypeRefTable
{
  internal const int TableIndex = 1;
  internal readonly int NumberOfRows;
  private readonly bool IsResolutionScopeRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly int ResolutionScopeOffset;
  private readonly int NameOffset;
  private readonly int NamespaceOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal TypeRefTable(
    int numberOfRows,
    int resolutionScopeRefSize,
    int stringHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsResolutionScopeRefSizeSmall = resolutionScopeRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.ResolutionScopeOffset = 0;
    this.NameOffset = this.ResolutionScopeOffset + resolutionScopeRefSize;
    this.NamespaceOffset = this.NameOffset + stringHeapRefSize;
    this.RowSize = this.NamespaceOffset + stringHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetNamespace(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NamespaceOffset, this.IsStringHeapRefSizeSmall);
  }

  internal MetadataToken GetResolutionScope(int rowId)
  {
    return ResolutionScopeTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.ResolutionScopeOffset, this.IsResolutionScopeRefSizeSmall));
  }
}
