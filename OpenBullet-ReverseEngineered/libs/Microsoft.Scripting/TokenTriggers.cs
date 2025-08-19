// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.TokenTriggers
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;

#nullable disable
namespace Microsoft.Scripting;

[Flags]
public enum TokenTriggers
{
  None = 0,
  MemberSelect = 1,
  MatchBraces = 2,
  ParameterStart = 16, // 0x00000010
  ParameterNext = 32, // 0x00000020
  ParameterEnd = 64, // 0x00000040
  Parameter = 128, // 0x00000080
  MethodTip = Parameter | ParameterEnd | ParameterNext | ParameterStart, // 0x000000F0
}
