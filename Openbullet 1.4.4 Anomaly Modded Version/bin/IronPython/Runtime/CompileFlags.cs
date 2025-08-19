// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.CompileFlags
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime;

[Flags]
public enum CompileFlags
{
  CO_NESTED = 16, // 0x00000010
  CO_DONT_IMPLY_DEDENT = 512, // 0x00000200
  CO_GENERATOR_ALLOWED = 4096, // 0x00001000
  CO_FUTURE_DIVISION = 8192, // 0x00002000
  CO_FUTURE_ABSOLUTE_IMPORT = 16384, // 0x00004000
  CO_FUTURE_WITH_STATEMENT = 32768, // 0x00008000
  CO_FUTURE_PRINT_FUNCTION = 65536, // 0x00010000
  CO_FUTURE_UNICODE_LITERALS = 131072, // 0x00020000
}
