// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataRecordType
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

[Serializable]
public enum MetadataRecordType
{
  ModuleDef = 0,
  TypeRef = 1,
  TypeDef = 2,
  FieldDef = 4,
  MethodDef = 6,
  ParamDef = 8,
  InterfaceImpl = 9,
  MemberRef = 10, // 0x0000000A
  CustomAttributeDef = 12, // 0x0000000C
  Permission = 14, // 0x0000000E
  SignatureDef = 17, // 0x00000011
  EventDef = 20, // 0x00000014
  PropertyDef = 23, // 0x00000017
  ModuleRef = 26, // 0x0000001A
  TypeSpec = 27, // 0x0000001B
  AssemblyDef = 32, // 0x00000020
  AssemblyRef = 35, // 0x00000023
  FileDef = 38, // 0x00000026
  TypeExport = 39, // 0x00000027
  ManifestResourceDef = 40, // 0x00000028
  TypeNesting = 41, // 0x00000029
  GenericParamDef = 42, // 0x0000002A
  MethodSpec = 43, // 0x0000002B
  GenericParamConstraint = 44, // 0x0000002C
}
