// Decompiled with JetBrains decompiler
// Type: AngleSharp.Attributes.Accessors
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Attributes;

[Flags]
public enum Accessors : byte
{
  None = 0,
  Getter = 1,
  Setter = 2,
  Deleter = 4,
  Adder = 8,
  Remover = 16, // 0x10
}
