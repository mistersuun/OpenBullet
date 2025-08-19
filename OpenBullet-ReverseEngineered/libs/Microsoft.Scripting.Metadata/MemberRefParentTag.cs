// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MemberRefParentTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class MemberRefParentTag
{
  internal const int NumberOfBits = 3;
  internal const int LargeRowSize = 8192 /*0x2000*/;
  internal const uint TypeDef = 0;
  internal const uint TypeRef = 1;
  internal const uint ModuleRef = 2;
  internal const uint Method = 3;
  internal const uint TypeSpec = 4;
  internal const uint TagMask = 7;
  internal const TableMask TablesReferenced = TableMask.TypeRef | TableMask.TypeDef | TableMask.Method | TableMask.ModuleRef | TableMask.TypeSpec;

  internal static MetadataToken ConvertToToken(uint memberRef)
  {
    MetadataTokenType type;
    switch (memberRef & 7U)
    {
      case 0:
        type = MetadataTokenType.TypeDef;
        break;
      case 1:
        type = MetadataTokenType.TypeRef;
        break;
      case 2:
        type = MetadataTokenType.ModuleRef;
        break;
      case 3:
        type = MetadataTokenType.MethodDef;
        break;
      case 4:
        type = MetadataTokenType.TypeSpec;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, memberRef >> 3);
  }
}
