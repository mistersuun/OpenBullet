// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ExportedTypeTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ExportedTypeTable
{
  internal const int TableIndex = 39;
  internal readonly int NumberOfRows;
  private readonly bool IsImplementationRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly int FlagsOffset;
  private readonly int TypeDefIdOffset;
  private readonly int TypeNameOffset;
  private readonly int TypeNamespaceOffset;
  private readonly int ImplementationOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal ExportedTypeTable(
    int numberOfRows,
    int implementationRefSize,
    int stringHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsImplementationRefSizeSmall = implementationRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.FlagsOffset = 0;
    this.TypeDefIdOffset = this.FlagsOffset + 4;
    this.TypeNameOffset = this.TypeDefIdOffset + 4;
    this.TypeNamespaceOffset = this.TypeNameOffset + stringHeapRefSize;
    this.ImplementationOffset = this.TypeNamespaceOffset + stringHeapRefSize;
    this.RowSize = this.ImplementationOffset + implementationRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetNamespace(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.TypeNamespaceOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.TypeNameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal TypeAttributes GetFlags(int rowId)
  {
    return (TypeAttributes) this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal MetadataToken GetImplementation(int rowId)
  {
    return ImplementationTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.ImplementationOffset, this.IsImplementationRefSizeSmall));
  }
}
