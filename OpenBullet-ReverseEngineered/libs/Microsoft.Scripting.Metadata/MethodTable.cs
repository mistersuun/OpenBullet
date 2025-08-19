// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class MethodTable
{
  internal const int TableIndex = 6;
  internal readonly int NumberOfRows;
  private readonly bool IsParamRefSizeSmall;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int RVAOffset;
  private readonly int ImplFlagsOffset;
  private readonly int FlagsOffset;
  private readonly int NameOffset;
  private readonly int SignatureOffset;
  private readonly int ParamListOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal MethodTable(
    int numberOfRows,
    int paramRefSize,
    int stringHeapRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsParamRefSizeSmall = paramRefSize == 2;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.RVAOffset = 0;
    this.ImplFlagsOffset = this.RVAOffset + 4;
    this.FlagsOffset = this.ImplFlagsOffset + 2;
    this.NameOffset = this.FlagsOffset + 2;
    this.SignatureOffset = this.NameOffset + stringHeapRefSize;
    this.ParamListOffset = this.SignatureOffset + blobHeapRefSize;
    this.RowSize = this.ParamListOffset + paramRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal uint GetFirstParamRid(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.ParamListOffset, this.IsParamRefSizeSmall);
  }

  internal uint GetSignature(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.SignatureOffset, this.IsBlobHeapRefSizeSmall);
  }

  internal uint GetRVA(int rowId)
  {
    return this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.RVAOffset);
  }

  internal MethodAttributes GetFlags(int rowId)
  {
    return (MethodAttributes) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal MethodImplAttributes GetImplFlags(int rowId)
  {
    return (MethodImplAttributes) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.ImplFlagsOffset);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal int GetNextRVA(int rva)
  {
    int num1 = int.MaxValue;
    int num2 = this.NumberOfRows * this.RowSize;
    for (int rvaOffset = this.RVAOffset; rvaOffset < num2; rvaOffset += this.RowSize)
    {
      int num3 = this.Table.ReadInt32(rvaOffset);
      if (num3 > rva && num3 < num1)
        num1 = num3;
    }
    return num1 != int.MaxValue ? num1 : -1;
  }

  internal int FindMethodContainingParam(int paramDefOrPtrRowId, int paramTableRowCount)
  {
    int num = 1 + this.Table.BinarySearchForSlot(this.NumberOfRows, paramTableRowCount, this.RowSize, this.ParamListOffset, (uint) paramDefOrPtrRowId, this.IsParamRefSizeSmall);
    return num != 0 ? num : throw new BadImageFormatException();
  }
}
