// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ModuleOptions
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime;

[Flags]
public enum ModuleOptions
{
  None = 0,
  TrueDivision = 1,
  ShowClsMethods = 2,
  Optimized = 4,
  Initialize = 8,
  WithStatement = 16, // 0x00000010
  AbsoluteImports = 32, // 0x00000020
  NoBuiltins = 64, // 0x00000040
  ModuleBuiltins = 128, // 0x00000080
  ExecOrEvalCode = 256, // 0x00000100
  SkipFirstLine = 512, // 0x00000200
  PrintFunction = 1024, // 0x00000400
  Interpret = 4096, // 0x00001000
  UnicodeLiterals = 8192, // 0x00002000
  Verbatim = 16384, // 0x00004000
  LightThrow = 32768, // 0x00008000
}
