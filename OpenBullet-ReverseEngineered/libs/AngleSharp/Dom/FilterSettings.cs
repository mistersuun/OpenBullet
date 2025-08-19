// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.FilterSettings
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System;

#nullable disable
namespace AngleSharp.Dom;

[Flags]
[DomName("NodeFilter")]
public enum FilterSettings : ulong
{
  [DomName("SHOW_ALL")] All = 4294967295, // 0x00000000FFFFFFFF
  [DomName("SHOW_ELEMENT")] Element = 1,
  [DomName("SHOW_ATTRIBUTE"), DomHistorical] Attribute = 2,
  [DomName("SHOW_TEXT")] Text = 4,
  [DomName("SHOW_CDATA_SECTION"), DomHistorical] CharacterData = 8,
  [DomName("SHOW_ENTITY_REFERENCE"), DomHistorical] EntityReference = 16, // 0x0000000000000010
  [DomName("SHOW_ENTITY"), DomHistorical] Entity = 32, // 0x0000000000000020
  [DomName("SHOW_PROCESSING_INSTRUCTION")] ProcessingInstruction = 64, // 0x0000000000000040
  [DomName("SHOW_COMMENT")] Comment = 128, // 0x0000000000000080
  [DomName("SHOW_DOCUMENT")] Document = 256, // 0x0000000000000100
  [DomName("SHOW_DOCUMENT_TYPE")] DocumentType = 512, // 0x0000000000000200
  [DomName("SHOW_DOCUMENT_FRAGMENT")] DocumentFragment = 1024, // 0x0000000000000400
  [DomName("SHOW_NOTATION"), DomHistorical] Notation = 2048, // 0x0000000000000800
}
