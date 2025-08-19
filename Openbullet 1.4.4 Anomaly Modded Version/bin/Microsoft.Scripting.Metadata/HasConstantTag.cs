// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.HasConstantTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class HasConstantTag
{
  internal const int NumberOfBits = 2;
  internal const int LargeRowSize = 16384 /*0x4000*/;
  internal const uint Field = 0;
  internal const uint Param = 1;
  internal const uint Property = 2;
  internal const uint TagMask = 3;
  internal const TableMask TablesReferenced = TableMask.Field | TableMask.Param | TableMask.Property;

  internal static MetadataToken ConvertToToken(uint hasConstant)
  {
    MetadataTokenType type;
    switch (hasConstant & 3U)
    {
      case 0:
        type = MetadataTokenType.FieldDef;
        break;
      case 1:
        type = MetadataTokenType.ParamDef;
        break;
      case 2:
        type = MetadataTokenType.Property;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, hasConstant >> 2);
  }

  internal static uint ConvertToTag(MetadataToken token)
  {
    uint rid = (uint) token.Rid;
    switch (token.TokenType)
    {
      case MetadataTokenType.FieldDef:
        return (uint) ((int) rid << 2 | 0);
      case MetadataTokenType.ParamDef:
        return (uint) ((int) rid << 2 | 1);
      case MetadataTokenType.Property:
        return (uint) ((int) rid << 2 | 2);
      default:
        return 0;
    }
  }
}
