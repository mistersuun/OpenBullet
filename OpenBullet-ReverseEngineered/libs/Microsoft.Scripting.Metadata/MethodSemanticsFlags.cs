// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MethodSemanticsFlags
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal enum MethodSemanticsFlags : ushort
{
  Setter = 1,
  Getter = 2,
  Other = 4,
  AddOn = 8,
  RemoveOn = 16, // 0x0010
  Fire = 32, // 0x0020
}
