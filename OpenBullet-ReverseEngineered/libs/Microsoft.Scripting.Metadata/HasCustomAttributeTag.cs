// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.HasCustomAttributeTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class HasCustomAttributeTag
{
  internal const int NumberOfBits = 5;
  internal const int LargeRowSize = 2048 /*0x0800*/;
  internal const uint Method = 0;
  internal const uint Field = 1;
  internal const uint TypeRef = 2;
  internal const uint TypeDef = 3;
  internal const uint Param = 4;
  internal const uint InterfaceImpl = 5;
  internal const uint MemberRef = 6;
  internal const uint Module = 7;
  internal const uint DeclSecurity = 8;
  internal const uint Property = 9;
  internal const uint Event = 10;
  internal const uint StandAloneSig = 11;
  internal const uint ModuleRef = 12;
  internal const uint TypeSpec = 13;
  internal const uint Assembly = 14;
  internal const uint AssemblyRef = 15;
  internal const uint File = 16 /*0x10*/;
  internal const uint ExportedType = 17;
  internal const uint ManifestResource = 18;
  internal const uint GenericParameter = 19;
  internal const uint TagMask = 31 /*0x1F*/;
  internal const TableMask TablesReferenced = TableMask.Module | TableMask.TypeRef | TableMask.TypeDef | TableMask.Field | TableMask.Method | TableMask.Param | TableMask.InterfaceImpl | TableMask.MemberRef | TableMask.DeclSecurity | TableMask.StandAloneSig | TableMask.Event | TableMask.Property | TableMask.ModuleRef | TableMask.TypeSpec | TableMask.Assembly | TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType | TableMask.ManifestResource | TableMask.GenericParam;

  internal static MetadataToken ConvertToToken(uint hasCustomAttribute)
  {
    MetadataTokenType type;
    switch (hasCustomAttribute & 31U /*0x1F*/)
    {
      case 0:
        type = MetadataTokenType.MethodDef;
        break;
      case 1:
        type = MetadataTokenType.FieldDef;
        break;
      case 2:
        type = MetadataTokenType.TypeRef;
        break;
      case 3:
        type = MetadataTokenType.TypeDef;
        break;
      case 4:
        type = MetadataTokenType.ParamDef;
        break;
      case 5:
        type = MetadataTokenType.InterfaceImpl;
        break;
      case 6:
        type = MetadataTokenType.MemberRef;
        break;
      case 7:
        type = MetadataTokenType.Module;
        break;
      case 8:
        type = MetadataTokenType.Permission;
        break;
      case 9:
        type = MetadataTokenType.Property;
        break;
      case 10:
        type = MetadataTokenType.Event;
        break;
      case 11:
        type = MetadataTokenType.Signature;
        break;
      case 12:
        type = MetadataTokenType.ModuleRef;
        break;
      case 13:
        type = MetadataTokenType.TypeSpec;
        break;
      case 14:
        type = MetadataTokenType.Assembly;
        break;
      case 15:
        type = MetadataTokenType.AssemblyRef;
        break;
      case 16 /*0x10*/:
        type = MetadataTokenType.File;
        break;
      case 17:
        type = MetadataTokenType.ExportedType;
        break;
      case 18:
        type = MetadataTokenType.ManifestResource;
        break;
      case 19:
        type = MetadataTokenType.GenericPar;
        break;
      default:
        throw new BadImageFormatException();
    }
    return new MetadataToken(type, hasCustomAttribute >> 5);
  }

  internal static uint ConvertToTag(MetadataToken token)
  {
    uint rid = (uint) token.Rid;
    switch (token.TokenType)
    {
      case MetadataTokenType.Module:
        return (uint) ((int) rid << 5 | 7);
      case MetadataTokenType.TypeRef:
        return (uint) ((int) rid << 5 | 2);
      case MetadataTokenType.TypeDef:
        return (uint) ((int) rid << 5 | 3);
      case MetadataTokenType.FieldDef:
        return (uint) ((int) rid << 5 | 1);
      case MetadataTokenType.MethodDef:
        return (uint) ((int) rid << 5 | 0);
      case MetadataTokenType.ParamDef:
        return (uint) ((int) rid << 5 | 4);
      case MetadataTokenType.InterfaceImpl:
        return (uint) ((int) rid << 5 | 5);
      case MetadataTokenType.MemberRef:
        return (uint) ((int) rid << 5 | 6);
      case MetadataTokenType.Permission:
        return (uint) ((int) rid << 5 | 8);
      case MetadataTokenType.Signature:
        return (uint) ((int) rid << 5 | 11);
      case MetadataTokenType.Event:
        return (uint) ((int) rid << 5 | 10);
      case MetadataTokenType.Property:
        return (uint) ((int) rid << 5 | 9);
      case MetadataTokenType.ModuleRef:
        return (uint) ((int) rid << 5 | 12);
      case MetadataTokenType.TypeSpec:
        return (uint) ((int) rid << 5 | 13);
      case MetadataTokenType.Assembly:
        return (uint) ((int) rid << 5 | 14);
      case MetadataTokenType.AssemblyRef:
        return (uint) ((int) rid << 5 | 15);
      case MetadataTokenType.File:
        return (uint) ((int) rid << 5 | 16 /*0x10*/);
      case MetadataTokenType.ExportedType:
        return (uint) ((int) rid << 5 | 17);
      case MetadataTokenType.ManifestResource:
        return (uint) ((int) rid << 5 | 18);
      case MetadataTokenType.GenericPar:
        return (uint) ((int) rid << 5 | 19);
      default:
        return 0;
    }
  }
}
