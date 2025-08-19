// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeDefTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class TypeDefTable
{
  internal const int TableIndex = 2;
  internal readonly int NumberOfRows;
  private readonly bool IsFieldRefSizeSmall;
  private readonly bool IsMethodRefSizeSmall;
  private readonly bool IsTypeDefOrRefRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly int FlagsOffset;
  private readonly int NameOffset;
  private readonly int NamespaceOffset;
  private readonly int ExtendsOffset;
  private readonly int FieldListOffset;
  private readonly int MethodListOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal TypeDefTable(
    int numberOfRows,
    int fieldRefSize,
    int methodRefSize,
    int typeDefOrRefRefSize,
    int stringHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsFieldRefSizeSmall = fieldRefSize == 2;
    this.IsMethodRefSizeSmall = methodRefSize == 2;
    this.IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.FlagsOffset = 0;
    this.NameOffset = this.FlagsOffset + 4;
    this.NamespaceOffset = this.NameOffset + stringHeapRefSize;
    this.ExtendsOffset = this.NamespaceOffset + stringHeapRefSize;
    this.FieldListOffset = this.ExtendsOffset + typeDefOrRefRefSize;
    this.MethodListOffset = this.FieldListOffset + fieldRefSize;
    this.RowSize = this.MethodListOffset + methodRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal TypeAttributes GetFlags(int rowId)
  {
    return (TypeAttributes) this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal uint GetNamespace(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NamespaceOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal MetadataToken GetExtends(int rowId)
  {
    return TypeDefOrRefTag.ConvertToToken(this.Table.ReadReference((rowId - 1) * this.RowSize + this.ExtendsOffset, this.IsTypeDefOrRefRefSizeSmall));
  }

  internal uint GetFirstFieldRid(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.FieldListOffset, this.IsFieldRefSizeSmall);
  }

  internal uint GetFirstMethodRid(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.MethodListOffset, this.IsMethodRefSizeSmall);
  }

  internal int FindTypeContainingMethod(int methodDefOrPtrRowId, int methodTableRowCount)
  {
    int num = 1 + this.Table.BinarySearchForSlot(this.NumberOfRows, methodTableRowCount, this.RowSize, this.MethodListOffset, (uint) methodDefOrPtrRowId, this.IsMethodRefSizeSmall);
    return num != 0 ? num : throw new BadImageFormatException();
  }

  internal int FindTypeContainingField(int fieldDefOrPtrRowId, int fieldTableRowCount)
  {
    int num = 1 + this.Table.BinarySearchForSlot(this.NumberOfRows, fieldTableRowCount, this.RowSize, this.FieldListOffset, (uint) fieldDefOrPtrRowId, this.IsFieldRefSizeSmall);
    return num != 0 ? num : throw new BadImageFormatException();
  }
}
