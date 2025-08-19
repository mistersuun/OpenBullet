// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.AssemblyRefTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class AssemblyRefTable
{
  internal const int TableIndex = 35;
  internal readonly int NumberOfRows;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int MajorVersionOffset;
  private readonly int MinorVersionOffset;
  private readonly int BuildNumberOffset;
  private readonly int RevisionNumberOffset;
  private readonly int FlagsOffset;
  private readonly int PublicKeyOrTokenOffset;
  private readonly int NameOffset;
  private readonly int CultureOffset;
  private readonly int HashValueOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal AssemblyRefTable(
    int numberOfRows,
    int stringHeapRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.MajorVersionOffset = 0;
    this.MinorVersionOffset = this.MajorVersionOffset + 2;
    this.BuildNumberOffset = this.MinorVersionOffset + 2;
    this.RevisionNumberOffset = this.BuildNumberOffset + 2;
    this.FlagsOffset = this.RevisionNumberOffset + 2;
    this.PublicKeyOrTokenOffset = this.FlagsOffset + 4;
    this.NameOffset = this.PublicKeyOrTokenOffset + blobHeapRefSize;
    this.CultureOffset = this.NameOffset + stringHeapRefSize;
    this.HashValueOffset = this.CultureOffset + stringHeapRefSize;
    this.RowSize = this.HashValueOffset + blobHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal Version GetVersion(int rowId)
  {
    int num = (rowId - 1) * this.RowSize;
    return new Version((int) this.Table.ReadUInt16(num + this.MajorVersionOffset), (int) this.Table.ReadUInt16(num + this.MinorVersionOffset), (int) this.Table.ReadUInt16(num + this.BuildNumberOffset), (int) this.Table.ReadUInt16(num + this.RevisionNumberOffset));
  }

  internal AssemblyNameFlags GetFlags(int rowId)
  {
    return (AssemblyNameFlags) this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetCulture(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.CultureOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetPublicKeyOrToken(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.PublicKeyOrTokenOffset, this.IsBlobHeapRefSizeSmall);
  }

  internal uint GetHashValue(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.HashValueOffset, this.IsBlobHeapRefSizeSmall);
  }
}
