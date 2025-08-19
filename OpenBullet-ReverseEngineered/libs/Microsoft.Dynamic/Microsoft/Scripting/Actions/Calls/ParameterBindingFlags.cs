// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ParameterBindingFlags
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

[Flags]
public enum ParameterBindingFlags
{
  None = 0,
  ProhibitNull = 1,
  ProhibitNullItems = 2,
  IsParamArray = 4,
  IsParamDictionary = 8,
  IsHidden = 16, // 0x00000010
}
