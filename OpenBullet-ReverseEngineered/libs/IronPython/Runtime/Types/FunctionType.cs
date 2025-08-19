// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.FunctionType
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime.Types;

[Flags]
public enum FunctionType
{
  None = 0,
  Function = 1,
  Method = 2,
  FunctionMethodMask = Method | Function, // 0x00000003
  AlwaysVisible = 4,
  ReversedOperator = 32, // 0x00000020
  BinaryOperator = 64, // 0x00000040
  ModuleMethod = 128, // 0x00000080
}
