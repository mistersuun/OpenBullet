// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ImplementationTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class ImplementationTag
{
  internal const int NumberOfBits = 2;
  internal const int LargeRowSize = 16384 /*0x4000*/;
  internal const uint File = 0;
  internal const uint AssemblyRef = 1;
  internal const uint ExportedType = 2;
  internal const uint TagMask = 3;
  internal const TableMask TablesReferenced = TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType;

  internal static MetadataToken ConvertToToken(uint implementation)
  {
    if (implementation == 0U)
      return new MetadataToken();
    MetadataTokenType type;
    switch (implementation & 3U)
    {
      case 0:
        type = MetadataTokenType.File;
        break;
      case 1:
        type = MetadataTokenType.AssemblyRef;
        break;
      case 2:
        type = MetadataTokenType.ExportedType;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, implementation >> 2);
  }
}
