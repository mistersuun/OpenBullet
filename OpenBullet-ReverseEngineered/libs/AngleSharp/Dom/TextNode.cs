// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.TextNode
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Text;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class TextNode : 
  CharacterData,
  IText,
  ICharacterData,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IChildNode,
  INonDocumentTypeChildNode
{
  internal TextNode(Document owner)
    : this(owner, string.Empty)
  {
  }

  internal TextNode(Document owner, string text)
    : base(owner, "#text", NodeType.Text, text)
  {
  }

  internal bool IsEmpty
  {
    get
    {
      for (int index = 0; index < this.Length; ++index)
      {
        if (!this[index].IsSpaceCharacter())
          return false;
      }
      return true;
    }
  }

  public string Text
  {
    get
    {
      Node previousSibling = this.PreviousSibling;
      textNode = this;
      StringBuilder sb = StringBuilderPool.Obtain();
      for (; previousSibling is TextNode; previousSibling = textNode.PreviousSibling)
        textNode = (TextNode) previousSibling;
      do
      {
        sb.Append(textNode.Data);
      }
      while (textNode.NextSibling is TextNode textNode);
      return sb.ToPool();
    }
  }

  public IElement AssignedSlot
  {
    get
    {
      IElement parentElement = this.ParentElement;
      return parentElement.IsShadow() ? parentElement.ShadowRoot.GetAssignedSlot((string) null) : (IElement) null;
    }
  }

  public IText Split(int offset)
  {
    int length = this.Length;
    if (offset > length)
      throw new DomException(DomError.IndexSizeError);
    int count = length - offset;
    TextNode newNode = new TextNode(this.Owner, this.Substring(offset, count));
    Node parent = this.Parent;
    Document owner = this.Owner;
    if (parent != null)
    {
      int index = this.Index();
      parent.InsertBefore((INode) newNode, (INode) this.NextSibling);
      owner.ForEachRange((Predicate<Range>) (m => m.Head == this && m.Start > offset), (Action<Range>) (m => m.StartWith((INode) newNode, m.Start - offset)));
      owner.ForEachRange((Predicate<Range>) (m => m.Tail == this && m.End > offset), (Action<Range>) (m => m.EndWith((INode) newNode, m.End - offset)));
      owner.ForEachRange((Predicate<Range>) (m => m.Head == parent && m.Start == index + 1), (Action<Range>) (m => m.StartWith((INode) parent, m.Start + 1)));
      owner.ForEachRange((Predicate<Range>) (m => m.Tail == parent && m.End == index + 1), (Action<Range>) (m => m.StartWith((INode) parent, m.End + 1)));
    }
    this.Replace(offset, count, string.Empty);
    if (parent != null)
    {
      owner.ForEachRange((Predicate<Range>) (m => m.Head == this && m.Start > offset), (Action<Range>) (m => m.StartWith((INode) this, offset)));
      owner.ForEachRange((Predicate<Range>) (m => m.Tail == this && m.End > offset), (Action<Range>) (m => m.EndWith((INode) this, offset)));
    }
    return (IText) newNode;
  }

  public override Node Clone(Document owner, bool deep)
  {
    TextNode target = new TextNode(owner, this.Data);
    this.CloneNode((Node) target, owner, deep);
    return (Node) target;
  }
}
