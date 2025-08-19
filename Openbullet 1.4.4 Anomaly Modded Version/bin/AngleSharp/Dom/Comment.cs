// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Comment
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Dom;

internal sealed class Comment : 
  CharacterData,
  IComment,
  ICharacterData,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IChildNode,
  INonDocumentTypeChildNode
{
  internal Comment(Document owner)
    : this(owner, string.Empty)
  {
  }

  internal Comment(Document owner, string data)
    : base(owner, "#comment", NodeType.Comment, data)
  {
  }

  public override Node Clone(Document owner, bool deep)
  {
    Comment target = new Comment(owner, this.Data);
    this.CloneNode((Node) target, owner, deep);
    return (Node) target;
  }
}
