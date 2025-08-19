// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonOperator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler;

public enum PythonOperator
{
  None = 0,
  Not = 1,
  Pos = 2,
  Invert = 3,
  Negate = 4,
  Add = 5,
  Subtract = 6,
  Multiply = 7,
  Divide = 8,
  TrueDivide = 9,
  Mod = 10, // 0x0000000A
  BitwiseAnd = 11, // 0x0000000B
  BitwiseOr = 12, // 0x0000000C
  ExclusiveOr = 13, // 0x0000000D
  Xor = 13, // 0x0000000D
  LeftShift = 14, // 0x0000000E
  RightShift = 15, // 0x0000000F
  Power = 16, // 0x00000010
  FloorDivide = 17, // 0x00000011
  LessThan = 18, // 0x00000012
  LessThanOrEqual = 19, // 0x00000013
  GreaterThan = 20, // 0x00000014
  GreaterThanOrEqual = 21, // 0x00000015
  Equal = 22, // 0x00000016
  Equals = 22, // 0x00000016
  NotEqual = 23, // 0x00000017
  NotEquals = 23, // 0x00000017
  In = 24, // 0x00000018
  NotIn = 25, // 0x00000019
  IsNot = 26, // 0x0000001A
  Is = 27, // 0x0000001B
}
