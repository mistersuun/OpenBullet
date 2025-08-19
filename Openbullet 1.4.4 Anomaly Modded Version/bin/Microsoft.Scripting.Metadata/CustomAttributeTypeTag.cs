// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.CustomAttributeTypeTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class CustomAttributeTypeTag
{
  internal const int NumberOfBits = 3;
  internal const int LargeRowSize = 8192 /*0x2000*/;
  internal const uint Method = 2;
  internal const uint MemberRef = 3;
  internal const uint TagMask = 7;
  internal const TableMask TablesReferenced = TableMask.Method | TableMask.MemberRef;

  internal static MetadataToken ConvertToToken(uint customAttributeType)
  {
    MetadataTokenType type;
    switch (customAttributeType & 7U)
    {
      case 2:
        type = MetadataTokenType.MethodDef;
        break;
      case 3:
        type = MetadataTokenType.MemberRef;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, customAttributeType >> 3);
  }
}
