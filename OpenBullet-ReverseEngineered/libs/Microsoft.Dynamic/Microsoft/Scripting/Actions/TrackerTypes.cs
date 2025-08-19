// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.TrackerTypes
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Actions;

[Flags]
public enum TrackerTypes
{
  None = 0,
  Constructor = 1,
  Event = 2,
  Field = 4,
  Method = 8,
  Property = 16, // 0x00000010
  Type = 32, // 0x00000020
  Namespace = 64, // 0x00000040
  MethodGroup = 128, // 0x00000080
  TypeGroup = 256, // 0x00000100
  Custom = 512, // 0x00000200
  Bound = 1024, // 0x00000400
  All = Bound | TypeGroup | MethodGroup | Namespace | Type | Property | Method | Field | Event | Constructor, // 0x000005FF
}
