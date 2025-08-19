// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ResolutionScopeTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class ResolutionScopeTag
{
  internal const int NumberOfBits = 2;
  internal const int LargeRowSize = 16384 /*0x4000*/;
  internal const uint Module = 0;
  internal const uint ModuleRef = 1;
  internal const uint AssemblyRef = 2;
  internal const uint TypeRef = 3;
  internal const uint TagMask = 3;
  internal const TableMask TablesReferenced = TableMask.Module | TableMask.TypeRef | TableMask.ModuleRef | TableMask.AssemblyRef;

  internal static MetadataToken ConvertToToken(uint resolutionScope)
  {
    MetadataTokenType type;
    switch (resolutionScope & 3U)
    {
      case 0:
        type = MetadataTokenType.Module;
        break;
      case 1:
        type = MetadataTokenType.ModuleRef;
        break;
      case 2:
        type = MetadataTokenType.AssemblyRef;
        break;
      case 3:
        type = MetadataTokenType.TypeRef;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, resolutionScope >> 2);
  }
}
