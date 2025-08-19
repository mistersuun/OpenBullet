// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.HasDeclSecurityTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class HasDeclSecurityTag
{
  internal const int NumberOfBits = 2;
  internal const int LargeRowSize = 16384 /*0x4000*/;
  internal const uint TypeDef = 0;
  internal const uint Method = 1;
  internal const uint Assembly = 2;
  internal const uint TagMask = 3;
  internal const TableMask TablesReferenced = TableMask.TypeDef | TableMask.Method | TableMask.Assembly;

  internal static MetadataToken ConvertToToken(uint hasDeclSecurity)
  {
    MetadataTokenType type;
    switch (hasDeclSecurity & 3U)
    {
      case 0:
        type = MetadataTokenType.TypeDef;
        break;
      case 1:
        type = MetadataTokenType.MethodDef;
        break;
      case 2:
        type = MetadataTokenType.Assembly;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, hasDeclSecurity >> 2);
  }

  internal static uint ConvertToTag(MetadataToken token)
  {
    uint rid = (uint) token.Rid;
    switch (token.TokenType)
    {
      case MetadataTokenType.TypeDef:
        return (uint) ((int) rid << 2 | 0);
      case MetadataTokenType.MethodDef:
        return (uint) ((int) rid << 2 | 1);
      case MetadataTokenType.Assembly:
        return (uint) ((int) rid << 2 | 2);
      default:
        return 0;
    }
  }
}
