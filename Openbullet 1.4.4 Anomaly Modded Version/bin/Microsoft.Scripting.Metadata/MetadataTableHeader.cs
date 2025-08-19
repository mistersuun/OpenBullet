// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataTableHeader
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal struct MetadataTableHeader
{
  internal byte MajorVersion;
  internal byte MinorVersion;
  internal HeapSizeFlag HeapSizeFlags;
  internal TableMask ValidTables;
  internal TableMask SortedTables;
  internal int[] CompressedMetadataTableRowCount;

  internal int GetNumberOfTablesPresent()
  {
    ulong validTables = (ulong) this.ValidTables;
    ulong num1 = (ulong) (((long) validTables & 6148914691236517205L /*0x5555555555555555*/) + ((long) (validTables >> 1) & 6148914691236517205L /*0x5555555555555555*/));
    ulong num2 = (ulong) (((long) num1 & 3689348814741910323L /*0x3333333333333333*/) + ((long) (num1 >> 2) & 3689348814741910323L /*0x3333333333333333*/));
    ulong num3 = (ulong) (((long) num2 & 1085102592571150095L) + ((long) (num2 >> 4) & 1085102592571150095L));
    ulong num4 = (ulong) (((long) num3 & 71777214294589695L) + ((long) (num3 >> 8) & 71777214294589695L));
    ulong num5 = (ulong) (((long) num4 & 281470681808895L) + ((long) (num4 >> 16 /*0x10*/) & 281470681808895L));
    return (int) (((long) num5 & (long) uint.MaxValue) + ((long) (num5 >> 32 /*0x20*/) & (long) uint.MaxValue));
  }
}
