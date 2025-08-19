// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.ValidationErrors
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Html;

[Flags]
public enum ValidationErrors : ushort
{
  None = 0,
  ValueMissing = 1,
  TypeMismatch = 2,
  PatternMismatch = 4,
  TooLong = 8,
  TooShort = 16, // 0x0010
  RangeUnderflow = 32, // 0x0020
  RangeOverflow = 64, // 0x0040
  StepMismatch = 128, // 0x0080
  BadInput = 256, // 0x0100
  Custom = 512, // 0x0200
}
