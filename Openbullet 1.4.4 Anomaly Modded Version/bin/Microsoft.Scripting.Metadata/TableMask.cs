// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TableMask
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal enum TableMask : ulong
{
  Module = 1,
  TypeRef = 2,
  TypeDef = 4,
  FieldPtr = 8,
  Field = 16, // 0x0000000000000010
  MethodPtr = 32, // 0x0000000000000020
  Method = 64, // 0x0000000000000040
  ParamPtr = 128, // 0x0000000000000080
  Param = 256, // 0x0000000000000100
  InterfaceImpl = 512, // 0x0000000000000200
  MemberRef = 1024, // 0x0000000000000400
  Constant = 2048, // 0x0000000000000800
  CustomAttribute = 4096, // 0x0000000000001000
  FieldMarshal = 8192, // 0x0000000000002000
  DeclSecurity = 16384, // 0x0000000000004000
  ClassLayout = 32768, // 0x0000000000008000
  FieldLayout = 65536, // 0x0000000000010000
  StandAloneSig = 131072, // 0x0000000000020000
  EventMap = 262144, // 0x0000000000040000
  EventPtr = 524288, // 0x0000000000080000
  Event = 1048576, // 0x0000000000100000
  PropertyMap = 2097152, // 0x0000000000200000
  PropertyPtr = 4194304, // 0x0000000000400000
  Property = 8388608, // 0x0000000000800000
  MethodSemantics = 16777216, // 0x0000000001000000
  MethodImpl = 33554432, // 0x0000000002000000
  ModuleRef = 67108864, // 0x0000000004000000
  TypeSpec = 134217728, // 0x0000000008000000
  ImplMap = 268435456, // 0x0000000010000000
  FieldRva = 536870912, // 0x0000000020000000
  EnCLog = 1073741824, // 0x0000000040000000
  EnCMap = 2147483648, // 0x0000000080000000
  CompressedStreamNotAllowedMask = 3225944232, // 0x00000000C04800A8
  Assembly = 4294967296, // 0x0000000100000000
  AssemblyProcessor = 8589934592, // 0x0000000200000000
  AssemblyOS = 17179869184, // 0x0000000400000000
  AssemblyRef = 34359738368, // 0x0000000800000000
  AssemblyRefProcessor = 68719476736, // 0x0000001000000000
  AssemblyRefOS = 137438953472, // 0x0000002000000000
  File = 274877906944, // 0x0000004000000000
  ExportedType = 549755813888, // 0x0000008000000000
  ManifestResource = 1099511627776, // 0x0000010000000000
  NestedClass = 2199023255552, // 0x0000020000000000
  V1_0_TablesMask = 4166118277119, // 0x000003C9FFFFFFFF
  V1_1_TablesMask = 4166118277119, // 0x000003C9FFFFFFFF
  GenericParam = 4398046511104, // 0x0000040000000000
  MethodSpec = 8796093022208, // 0x0000080000000000
  GenericParamConstraint = 17592186044416, // 0x0000100000000000
  SortedTablesMask = 24190111578624, // 0x000016003301FA00
  V2_0_TablesMask = 34952443854847, // 0x00001FC9FFFFFFFF
}
