// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonOperationKind
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Runtime.Binding;

internal enum PythonOperationKind
{
  None = 0,
  Documentation = 1,
  CallSignatures = 2,
  IsCallable = 3,
  Hash = 4,
  Contains = 5,
  Length = 6,
  Compare = 7,
  DivMod = 8,
  AbsoluteValue = 9,
  Positive = 10, // 0x0000000A
  Negate = 11, // 0x0000000B
  OnesComplement = 12, // 0x0000000C
  GetItem = 13, // 0x0000000D
  SetItem = 14, // 0x0000000E
  DeleteItem = 15, // 0x0000000F
  IsFalse = 16, // 0x00000010
  Not = 17, // 0x00000011
  GetEnumeratorForIteration = 18, // 0x00000012
  Add = 19, // 0x00000013
  Subtract = 20, // 0x00000014
  Power = 21, // 0x00000015
  Multiply = 22, // 0x00000016
  FloorDivide = 23, // 0x00000017
  Divide = 24, // 0x00000018
  TrueDivide = 25, // 0x00000019
  Mod = 26, // 0x0000001A
  LeftShift = 27, // 0x0000001B
  RightShift = 28, // 0x0000001C
  BitwiseAnd = 29, // 0x0000001D
  BitwiseOr = 30, // 0x0000001E
  ExclusiveOr = 31, // 0x0000001F
  Comparison = 134217728, // 0x08000000
  LessThan = 134217760, // 0x08000020
  GreaterThan = 134217761, // 0x08000021
  LessThanOrEqual = 134217762, // 0x08000022
  GreaterThanOrEqual = 134217763, // 0x08000023
  Equal = 134217764, // 0x08000024
  NotEqual = 134217765, // 0x08000025
  LessThanGreaterThan = 134217766, // 0x08000026
  Reversed = 268435456, // 0x10000000
  ReverseDivMod = 268435464, // 0x10000008
  ReverseAdd = 268435475, // 0x10000013
  ReverseSubtract = 268435476, // 0x10000014
  ReversePower = 268435477, // 0x10000015
  ReverseMultiply = 268435478, // 0x10000016
  ReverseFloorDivide = 268435479, // 0x10000017
  ReverseDivide = 268435480, // 0x10000018
  ReverseTrueDivide = 268435481, // 0x10000019
  ReverseMod = 268435482, // 0x1000001A
  ReverseLeftShift = 268435483, // 0x1000001B
  ReverseRightShift = 268435484, // 0x1000001C
  ReverseBitwiseAnd = 268435485, // 0x1000001D
  ReverseBitwiseOr = 268435486, // 0x1000001E
  ReverseExclusiveOr = 268435487, // 0x1000001F
  InPlace = 536870912, // 0x20000000
  InPlaceAdd = 536870931, // 0x20000013
  InPlaceSubtract = 536870932, // 0x20000014
  InPlacePower = 536870933, // 0x20000015
  InPlaceMultiply = 536870934, // 0x20000016
  InPlaceFloorDivide = 536870935, // 0x20000017
  InPlaceDivide = 536870936, // 0x20000018
  InPlaceTrueDivide = 536870937, // 0x20000019
  InPlaceMod = 536870938, // 0x2000001A
  InPlaceLeftShift = 536870939, // 0x2000001B
  InPlaceRightShift = 536870940, // 0x2000001C
  InPlaceBitwiseAnd = 536870941, // 0x2000001D
  InPlaceBitwiseOr = 536870942, // 0x2000001E
  InPlaceExclusiveOr = 536870943, // 0x2000001F
  DisableCoerce = 1073741824, // 0x40000000
}
