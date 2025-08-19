// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.HasSemanticsTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class HasSemanticsTag
{
  internal const int NumberOfBits = 1;
  internal const int LargeRowSize = 32768 /*0x8000*/;
  internal const uint Event = 0;
  internal const uint Property = 1;
  internal const uint TagMask = 1;
  internal const TableMask TablesReferenced = TableMask.Event | TableMask.Property;

  internal static MetadataToken ConvertToToken(uint hasSemantic)
  {
    return new MetadataToken(((int) hasSemantic & 1) == 0 ? MetadataTokenType.Event : MetadataTokenType.Property, hasSemantic >> 1);
  }

  internal static uint ConvertEventRowIdToTag(int eventRowId) => (uint) (eventRowId << 1 | 0);

  internal static uint ConvertPropertyRowIdToTag(int propertyRowId)
  {
    return (uint) (propertyRowId << 1 | 1);
  }
}
