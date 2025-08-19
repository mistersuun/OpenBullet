// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.CharacterData
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Dom;

internal abstract class CharacterData : 
  Node,
  ICharacterData,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IChildNode,
  INonDocumentTypeChildNode
{
  private string _content;

  internal CharacterData(Document owner, string name, NodeType type)
    : this(owner, name, type, string.Empty)
  {
  }

  internal CharacterData(Document owner, string name, NodeType type, string content)
    : base(owner, name, type)
  {
    this._content = content;
  }

  public IElement PreviousElementSibling
  {
    get
    {
      Node parent = this.Parent;
      if (parent != null)
      {
        bool flag = false;
        for (int index = parent.ChildNodes.Length - 1; index >= 0; --index)
        {
          if (parent.ChildNodes[index] == this)
            flag = true;
          else if (flag && parent.ChildNodes[index] is IElement)
            return (IElement) parent.ChildNodes[index];
        }
      }
      return (IElement) null;
    }
  }

  public IElement NextElementSibling
  {
    get
    {
      Node parent = this.Parent;
      if (parent != null)
      {
        int length = parent.ChildNodes.Length;
        bool flag = false;
        for (int index = 0; index < length; ++index)
        {
          if (parent.ChildNodes[index] == this)
            flag = true;
          else if (flag && parent.ChildNodes[index] is IElement)
            return (IElement) parent.ChildNodes[index];
        }
      }
      return (IElement) null;
    }
  }

  internal char this[int index]
  {
    get => this._content[index];
    set
    {
      if (index < 0)
        return;
      if (index >= this.Length)
      {
        this._content = this._content.PadRight(index) + value.ToString();
      }
      else
      {
        char[] charArray = this._content.ToCharArray();
        charArray[index] = value;
        this._content = new string(charArray);
      }
    }
  }

  public int Length => this._content.Length;

  public sealed override string NodeValue
  {
    get => this.Data;
    set => this.Data = value;
  }

  public sealed override string TextContent
  {
    get => this.Data;
    set => this.Data = value;
  }

  public string Data
  {
    get => this._content;
    set => this.Replace(0, this.Length, value);
  }

  public string Substring(int offset, int count)
  {
    int length = this._content.Length;
    if (offset > length)
      throw new DomException(DomError.IndexSizeError);
    return offset + count > length ? this._content.Substring(offset) : this._content.Substring(offset, count);
  }

  public void Append(string value) => this.Replace(this._content.Length, 0, value);

  public void Insert(int offset, string data) => this.Replace(offset, 0, data);

  public void Delete(int offset, int count) => this.Replace(offset, count, string.Empty);

  public void Replace(int offset, int count, string data)
  {
    Document owner = this.Owner;
    int length = this._content.Length;
    if (offset > length)
      throw new DomException(DomError.IndexSizeError);
    if (offset + count > length)
      count = length - offset;
    string content = this._content;
    int startIndex = offset + data.Length;
    this._content = this._content.Insert(offset, data);
    if (count > 0)
      this._content = this._content.Remove(startIndex, count);
    owner.QueueMutation(MutationRecord.CharacterData((INode) this, content));
    owner.ForEachRange((Predicate<Range>) (m => m.Head == this && m.Start > offset && m.Start <= offset + count), (Action<Range>) (m => m.StartWith((INode) this, offset)));
    owner.ForEachRange((Predicate<Range>) (m => m.Tail == this && m.End > offset && m.End <= offset + count), (Action<Range>) (m => m.EndWith((INode) this, offset)));
    owner.ForEachRange((Predicate<Range>) (m => m.Head == this && m.Start > offset + count), (Action<Range>) (m => m.StartWith((INode) this, m.Start + data.Length - count)));
    owner.ForEachRange((Predicate<Range>) (m => m.Tail == this && m.End > offset + count), (Action<Range>) (m => m.EndWith((INode) this, m.End + data.Length - count)));
  }

  public void Before(params INode[] nodes) => this.InsertBefore(nodes);

  public void After(params INode[] nodes) => this.InsertAfter(nodes);

  public void Replace(params INode[] nodes) => this.ReplaceWith(nodes);

  public void Remove() => this.RemoveFromParent();
}
