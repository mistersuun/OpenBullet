// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.NodeIterator
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class NodeIterator : INodeIterator
{
  private readonly INode _root;
  private readonly FilterSettings _settings;
  private readonly NodeFilter _filter;
  private readonly IEnumerable<INode> _iterator;
  private INode _reference;
  private bool _beforeNode;

  public NodeIterator(INode root, FilterSettings settings, NodeFilter filter)
  {
    this._root = root;
    this._settings = settings;
    this._filter = filter ?? (NodeFilter) (m => FilterResult.Accept);
    this._beforeNode = true;
    this._iterator = NodeIterator.GetNodes(root);
    this._reference = root;
  }

  public INode Root => this._root;

  public FilterSettings Settings => this._settings;

  public NodeFilter Filter => this._filter;

  public INode Reference => this._reference;

  public bool IsBeforeReference => this._beforeNode;

  public INode Next()
  {
    INode node = this._reference;
    bool flag = this._beforeNode;
    do
    {
      if (!flag)
        node = this._iterator.SkipWhile<INode>((Func<INode, bool>) (m => m != node)).Skip<INode>(1).FirstOrDefault<INode>();
      if (node == null)
        return (INode) null;
      flag = false;
    }
    while (!this._settings.Accepts(node) || this._filter(node) != FilterResult.Accept);
    this._beforeNode = false;
    this._reference = node;
    return node;
  }

  public INode Previous()
  {
    INode node = this._reference;
    bool flag = this._beforeNode;
    do
    {
      if (flag)
        node = this._iterator.TakeWhile<INode>((Func<INode, bool>) (m => m != node)).LastOrDefault<INode>();
      if (node == null)
        return (INode) null;
      flag = true;
    }
    while (!this._settings.Accepts(node) || this._filter(node) != FilterResult.Accept);
    this._beforeNode = true;
    this._reference = node;
    return node;
  }

  private static IEnumerable<INode> GetNodes(INode root)
  {
    yield return root;
    foreach (INode node in root.GetNodes<INode>())
      yield return node;
  }
}
