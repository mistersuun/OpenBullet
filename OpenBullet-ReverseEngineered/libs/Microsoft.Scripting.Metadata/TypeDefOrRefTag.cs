// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeDefOrRefTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class TypeDefOrRefTag
{
  internal const int NumberOfBits = 2;
  internal const uint TypeDef = 0;
  internal const uint TypeRef = 1;
  internal const uint TypeSpec = 2;
  internal const uint TagMask = 3;
  internal const int LargeRowSize = 16384 /*0x4000*/;
  internal const TableMask TablesReferenced = TableMask.TypeRef | TableMask.TypeDef | TableMask.TypeSpec;

  internal static MetadataToken ConvertToToken(uint typeDefOrRefTag)
  {
    MetadataTokenType type;
    switch (typeDefOrRefTag & 3U)
    {
      case 0:
        type = MetadataTokenType.TypeDef;
        break;
      case 1:
        type = MetadataTokenType.TypeRef;
        break;
      case 2:
        type = MetadataTokenType.TypeSpec;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, typeDefOrRefTag >> 2);
  }
}
