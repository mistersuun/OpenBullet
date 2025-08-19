// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IText
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("Text")]
public interface IText : 
  ICharacterData,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IChildNode,
  INonDocumentTypeChildNode
{
  [DomName("splitText")]
  IText Split(int offset);

  [DomName("wholeText")]
  string Text { get; }

  [DomName("assignedSlot")]
  IElement AssignedSlot { get; }
}
