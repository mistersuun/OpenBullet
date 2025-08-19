// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.ParentNodeExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

public static class ParentNodeExtensions
{
  internal static INode MutationMacro(this INode parent, INode[] nodes)
  {
    if (nodes.Length <= 1)
      return nodes[0];
    IDocumentFragment documentFragment = parent.Owner.CreateDocumentFragment();
    for (int index = 0; index < nodes.Length; ++index)
      documentFragment.AppendChild(nodes[index]);
    return (INode) documentFragment;
  }

  public static void PrependNodes(this INode parent, params INode[] nodes)
  {
    if (nodes.Length == 0)
      return;
    INode node = parent.MutationMacro(nodes);
    parent.PreInsert(node, parent.FirstChild);
  }

  public static void AppendNodes(this INode parent, params INode[] nodes)
  {
    if (nodes.Length == 0)
      return;
    INode node = parent.MutationMacro(nodes);
    parent.PreInsert(node, (INode) null);
  }

  public static void InsertBefore(this INode child, params INode[] nodes)
  {
    INode parent = child.Parent;
    if (parent == null || nodes.Length == 0)
      return;
    INode node = parent.MutationMacro(nodes);
    parent.PreInsert(node, child);
  }

  public static void InsertAfter(this INode child, params INode[] nodes)
  {
    INode parent = child.Parent;
    if (parent == null || nodes.Length == 0)
      return;
    INode node = parent.MutationMacro(nodes);
    parent.PreInsert(node, child.NextSibling);
  }

  public static void ReplaceWith(this INode child, params INode[] nodes)
  {
    INode parent = child.Parent;
    if (parent == null)
      return;
    if (nodes.Length != 0)
    {
      INode newChild = parent.MutationMacro(nodes);
      parent.ReplaceChild(newChild, child);
    }
    else
      parent.RemoveChild(child);
  }

  public static void RemoveFromParent(this INode child)
  {
    INode parent = child.Parent;
    if (parent == null)
      return;
    parent.PreRemove(child);
  }

  public static TElement AppendElement<TElement>(this INode parent, TElement element) where TElement : class, IElement
  {
    return parent != null ? parent.AppendChild((INode) element) as TElement : throw new ArgumentNullException(nameof (parent));
  }

  public static TElement InsertElement<TElement>(
    this INode parent,
    TElement newElement,
    INode referenceElement)
    where TElement : class, IElement
  {
    if (parent == null)
      throw new ArgumentNullException(nameof (parent));
    return parent.InsertBefore((INode) newElement, referenceElement) as TElement;
  }

  public static TElement RemoveElement<TElement>(this INode parent, TElement element) where TElement : class, IElement
  {
    return parent != null ? parent.RemoveChild((INode) element) as TElement : throw new ArgumentNullException(nameof (parent));
  }

  public static TElement QuerySelector<TElement>(this IParentNode parent, string selectors) where TElement : class, IElement
  {
    if (parent == null)
      throw new ArgumentNullException(nameof (parent));
    return selectors != null ? parent.QuerySelector(selectors) as TElement : throw new ArgumentNullException(nameof (selectors));
  }

  public static IEnumerable<TElement> QuerySelectorAll<TElement>(
    this IParentNode parent,
    string selectors)
    where TElement : IElement
  {
    if (parent == null)
      throw new ArgumentNullException(nameof (parent));
    return selectors != null ? parent.QuerySelectorAll(selectors).OfType<TElement>() : throw new ArgumentNullException(nameof (selectors));
  }

  public static IEnumerable<TNode> Descendents<TNode>(this INode parent)
  {
    return parent.Descendents().OfType<TNode>();
  }

  public static IEnumerable<INode> Descendents(this INode parent)
  {
    return parent != null ? parent.GetDescendants() : throw new ArgumentNullException(nameof (parent));
  }

  public static IEnumerable<TNode> DescendentsAndSelf<TNode>(this INode parent)
  {
    return parent.DescendentsAndSelf().OfType<TNode>();
  }

  public static IEnumerable<INode> DescendentsAndSelf(this INode parent)
  {
    return parent != null ? parent.GetDescendantsAndSelf() : throw new ArgumentNullException(nameof (parent));
  }

  public static IEnumerable<TNode> Ancestors<TNode>(this INode child)
  {
    return child.Ancestors().OfType<TNode>();
  }

  public static IEnumerable<INode> Ancestors(this INode child)
  {
    return child != null ? child.GetAncestors() : throw new ArgumentNullException(nameof (child));
  }
}
