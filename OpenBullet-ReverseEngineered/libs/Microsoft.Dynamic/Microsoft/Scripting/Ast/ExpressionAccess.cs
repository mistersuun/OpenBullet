// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.ExpressionAccess
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Ast;

[Flags]
public enum ExpressionAccess
{
  None = 0,
  Read = 1,
  Write = 2,
  ReadWrite = Write | Read, // 0x00000003
}
