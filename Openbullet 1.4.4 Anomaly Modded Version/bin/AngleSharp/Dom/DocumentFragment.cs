// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DocumentFragment
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System.Linq;
using System.Text;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class DocumentFragment : 
  Node,
  IDocumentFragment,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  INonElementParentNode
{
  private HtmlCollection<IElement> _elements;

  internal DocumentFragment(Document owner)
    : base(owner, "#document-fragment", NodeType.DocumentFragment)
  {
  }

  internal DocumentFragment(Element contextElement, string html)
    : this(contextElement.Owner)
  {
    IElement subtree = contextElement.ParseSubtree(html);
    while (subtree.HasChildNodes)
    {
      INode firstChild = subtree.FirstChild;
      subtree.RemoveChild(firstChild);
      if (firstChild is Node)
      {
        this.Owner.AdoptNode(firstChild);
        this.InsertBefore((Node) firstChild, (Node) null, false);
      }
    }
  }

  public int ChildElementCount => this.ChildNodes.OfType<Element>().Count<Element>();

  public IHtmlCollection<IElement> Children
  {
    get
    {
      return (IHtmlCollection<IElement>) this._elements ?? (IHtmlCollection<IElement>) (this._elements = new HtmlCollection<IElement>((INode) this, false));
    }
  }

  public IElement FirstElementChild
  {
    get
    {
      NodeList childNodes = this.ChildNodes;
      int length = childNodes.Length;
      for (int index = 0; index < length; ++index)
      {
        if (childNodes[index] is IElement firstElementChild)
          return firstElementChild;
      }
      return (IElement) null;
    }
  }

  public IElement LastElementChild
  {
    get
    {
      NodeList childNodes = this.ChildNodes;
      for (int index = childNodes.Length - 1; index >= 0; --index)
      {
        if (childNodes[index] is IElement lastElementChild)
          return lastElementChild;
      }
      return (IElement) null;
    }
  }

  public override string TextContent
  {
    get
    {
      StringBuilder sb = StringBuilderPool.Obtain();
      foreach (IText text in this.GetDescendants().OfType<IText>())
        sb.Append(text.Data);
      return sb.ToPool();
    }
    set
    {
      this.ReplaceAll(!string.IsNullOrEmpty(value) ? (Node) new TextNode(this.Owner, value) : (Node) null, false);
    }
  }

  public void Prepend(params INode[] nodes) => this.PrependNodes(nodes);

  public void Append(params INode[] nodes) => this.AppendNodes(nodes);

  public IElement QuerySelector(string selectors) => this.ChildNodes.QuerySelector(selectors);

  public IHtmlCollection<IElement> QuerySelectorAll(string selectors)
  {
    return this.ChildNodes.QuerySelectorAll(selectors);
  }

  public IHtmlCollection<IElement> GetElementsByClassName(string classNames)
  {
    return this.ChildNodes.GetElementsByClassName(classNames);
  }

  public IHtmlCollection<IElement> GetElementsByTagName(string tagName)
  {
    return this.ChildNodes.GetElementsByTagName(tagName);
  }

  public IHtmlCollection<IElement> GetElementsByTagNameNS(string namespaceURI, string tagName)
  {
    return this.ChildNodes.GetElementsByTagName(namespaceURI, tagName);
  }

  public IElement GetElementById(string elementId) => this.ChildNodes.GetElementById(elementId);

  public override Node Clone(Document owner, bool deep)
  {
    DocumentFragment target = new DocumentFragment(owner);
    this.CloneNode((Node) target, owner, deep);
    return (Node) target;
  }
}
