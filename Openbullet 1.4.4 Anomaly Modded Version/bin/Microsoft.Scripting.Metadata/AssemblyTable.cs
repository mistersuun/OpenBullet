// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.AssemblyTable
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Configuration.Assemblies;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class AssemblyTable
{
  internal const int TableIndex = 32 /*0x20*/;
  internal readonly int NumberOfRows;
  private readonly bool IsStringHeapRefSizeSmall;
  private readonly bool IsBlobHeapRefSizeSmall;
  private readonly int HashAlgIdOffset;
  private readonly int MajorVersionOffset;
  private readonly int MinorVersionOffset;
  private readonly int BuildNumberOffset;
  private readonly int RevisionNumberOffset;
  private readonly int FlagsOffset;
  private readonly int PublicKeyOffset;
  private readonly int NameOffset;
  private readonly int CultureOffset;
  private readonly int RowSize;
  internal readonly MemoryBlock Table;

  internal AssemblyTable(
    int numberOfRows,
    int stringHeapRefSize,
    int blobHeapRefSize,
    int start,
    MemoryBlock block)
  {
    this.NumberOfRows = numberOfRows;
    this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
    this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
    this.HashAlgIdOffset = 0;
    this.MajorVersionOffset = this.HashAlgIdOffset + 4;
    this.MinorVersionOffset = this.MajorVersionOffset + 2;
    this.BuildNumberOffset = this.MinorVersionOffset + 2;
    this.RevisionNumberOffset = this.BuildNumberOffset + 2;
    this.FlagsOffset = this.RevisionNumberOffset + 2;
    this.PublicKeyOffset = this.FlagsOffset + 4;
    this.NameOffset = this.PublicKeyOffset + blobHeapRefSize;
    this.CultureOffset = this.NameOffset + stringHeapRefSize;
    this.RowSize = this.CultureOffset + stringHeapRefSize;
    this.Table = block.GetRange(start, this.RowSize * numberOfRows);
  }

  internal AssemblyHashAlgorithm GetHashAlgorithm(int rowId)
  {
    return (AssemblyHashAlgorithm) this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.HashAlgIdOffset);
  }

  internal AssemblyNameFlags GetFlags(int rowId)
  {
    return (AssemblyNameFlags) this.Table.ReadUInt32((rowId - 1) * this.RowSize + this.FlagsOffset);
  }

  internal Version GetVersion(int rowId)
  {
    int num = (rowId - 1) * this.RowSize;
    return new Version((int) this.Table.ReadUInt16(num + this.MajorVersionOffset), (int) this.Table.ReadUInt16(num + this.MinorVersionOffset), (int) this.Table.ReadUInt16(num + this.BuildNumberOffset), (int) this.Table.ReadUInt16(num + this.RevisionNumberOffset));
  }

  internal uint GetName(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.NameOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetCulture(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.CultureOffset, this.IsStringHeapRefSizeSmall);
  }

  internal uint GetPublicKey(int rowId)
  {
    return this.Table.ReadReference((rowId - 1) * this.RowSize + this.PublicKeyOffset, this.IsBlobHeapRefSizeSmall);
  }
}
