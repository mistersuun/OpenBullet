// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.NodeList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class NodeList : INodeList, IEnumerable<INode>, IEnumerable, IMarkupFormattable
{
  private readonly List<Node> _entries;
  internal static readonly NodeList Empty = new NodeList();

  internal NodeList() => this._entries = new List<Node>();

  public Node this[int index]
  {
    get => this._entries[index];
    set => this._entries[index] = value;
  }

  INode INodeList.this[int index] => (INode) this[index];

  public int Length => this._entries.Count;

  internal void Add(Node node) => this._entries.Add(node);

  internal void AddRange(NodeList nodeList)
  {
    this._entries.AddRange((IEnumerable<Node>) nodeList._entries);
  }

  internal void Insert(int index, Node node) => this._entries.Insert(index, node);

  internal void Remove(Node node) => this._entries.Remove(node);

  internal void RemoveAt(int index) => this._entries.RemoveAt(index);

  internal bool Contains(Node node) => this._entries.Contains(node);

  public void ToHtml(TextWriter writer, IMarkupFormatter formatter)
  {
    for (int index = 0; index < this._entries.Count; ++index)
      this._entries[index].ToHtml(writer, formatter);
  }

  public IEnumerator<INode> GetEnumerator() => (IEnumerator<INode>) this._entries.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._entries.GetEnumerator();
}
