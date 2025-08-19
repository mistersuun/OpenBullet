// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FunctionAttributes
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime;

[Flags]
public enum FunctionAttributes
{
  None = 0,
  ArgumentList = 4,
  KeywordDictionary = 8,
  Generator = 32, // 0x00000020
  FutureDivision = 8192, // 0x00002000
  CanSetSysExcInfo = 16384, // 0x00004000
  ContainsTryFinally = 32768, // 0x00008000
}
