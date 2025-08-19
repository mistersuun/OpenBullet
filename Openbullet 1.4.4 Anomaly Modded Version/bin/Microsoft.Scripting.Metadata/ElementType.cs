// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ElementType
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

public enum ElementType : byte
{
  End = 0,
  Void = 1,
  Boolean = 2,
  Char = 3,
  Int8 = 4,
  UInt8 = 5,
  Int16 = 6,
  UInt16 = 7,
  Int32 = 8,
  UInt32 = 9,
  Int64 = 10, // 0x0A
  UInt64 = 11, // 0x0B
  Single = 12, // 0x0C
  Double = 13, // 0x0D
  String = 14, // 0x0E
  Pointer = 15, // 0x0F
  ByReference = 16, // 0x10
  ValueType = 17, // 0x11
  Class = 18, // 0x12
  GenericTypeParameter = 19, // 0x13
  Array = 20, // 0x14
  GenericTypeInstance = 21, // 0x15
  TypedReference = 22, // 0x16
  IntPtr = 24, // 0x18
  UIntPtr = 25, // 0x19
  FunctionPointer = 27, // 0x1B
  Object = 28, // 0x1C
  Vector = 29, // 0x1D
  GenericMethodParameter = 30, // 0x1E
  RequiredModifier = 31, // 0x1F
  OptionalModifier = 32, // 0x20
  Internal = 33, // 0x21
  Max = 34, // 0x22
  Modifier = 64, // 0x40
  Sentinel = 65, // 0x41
  Pinned = 69, // 0x45
}
