// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.HasFieldMarshalTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class HasFieldMarshalTag
{
  internal const int NumberOfBits = 1;
  internal const int LargeRowSize = 32768 /*0x8000*/;
  internal const uint Field = 0;
  internal const uint Param = 1;
  internal const uint TagMask = 1;
  internal const TableMask TablesReferenced = TableMask.Field | TableMask.Param;

  internal static MetadataToken ConvertToToken(uint hasFieldMarshal)
  {
    return new MetadataToken(((int) hasFieldMarshal & 1) == 0 ? MetadataTokenType.FieldDef : MetadataTokenType.ParamDef, hasFieldMarshal >> 1);
  }

  internal static uint ConvertToTag(MetadataToken token)
  {
    uint rid = (uint) token.Rid;
    switch (token.TokenType)
    {
      case MetadataTokenType.FieldDef:
        return (uint) ((int) rid << 1 | 0);
      case MetadataTokenType.ParamDef:
        return (uint) ((int) rid << 1 | 1);
      default:
        return 0;
    }
  }
}
