// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DocumentPositions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;

#nullable disable
namespace AngleSharp.Dom;

[Flags]
[DomName("Document")]
public enum DocumentPositions : byte
{
  Same = 0,
  [DomName("DOCUMENT_POSITION_DISCONNECTED")] Disconnected = 1,
  [DomName("DOCUMENT_POSITION_PRECEDING")] Preceding = 2,
  [DomName("DOCUMENT_POSITION_FOLLOWING")] Following = 4,
  [DomName("DOCUMENT_POSITION_CONTAINS")] Contains = 8,
  [DomName("DOCUMENT_POSITION_CONTAINED_BY")] ContainedBy = 16, // 0x10
  [DomName("DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC")] ImplementationSpecific = 32, // 0x20
}
