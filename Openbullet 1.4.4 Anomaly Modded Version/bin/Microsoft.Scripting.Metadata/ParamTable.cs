// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ParamTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class ParamTable
{
  internal const int TableIndex = 8;
  internal readonly int NumberOfRows;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly int FlagsOffset;
  private readonly int SequenceOffset;
  private readonly int NameOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal ParamTable(int numberOfRows, int stringHeapRefSize, int start, MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.FlagsOffset = 0;
    this.SequenceOffset = this.FlagsOffset + 2;
    this.NameOffset = this.SequenceOffset + 2;
    this.RowSize = this.NameOffset + stringHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal ParameterAttributes GetFlags(int rowId)
  {
    return (ParameterAttributes) this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal ushort GetSequence(int rowId)
  {
    return this.Table.ReadUInt16((rowId - 1) * this.RowSize + this.SequenceOffset);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }
}
