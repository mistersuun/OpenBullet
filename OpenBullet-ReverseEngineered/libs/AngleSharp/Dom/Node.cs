// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Node
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace AngleSharp.Dom;

public abstract class Node : EventTarget, INode, IEventTarget, IMarkupFormattable, IEquatable<INode>
{
  private readonly NodeType _type;
  private readonly string _name;
  private readonly NodeFlags _flags;
  private Url _baseUri;
  private Node _parent;
  private NodeList _children;
  private Document _owner;

  public Node(Document owner, string name, NodeType type = NodeType.Element, NodeFlags flags = NodeFlags.None)
  {
    this._owner = owner;
    this._name = name ?? string.Empty;
    this._type = type;
    this._children = this.IsEndPoint() ? NodeList.Empty : new NodeList();
    this._flags = flags;
  }

  public NodeFlags Flags => this._flags;

  public bool HasChildNodes => this._children.Length != 0;

  public string BaseUri => this.BaseUrl?.Href ?? string.Empty;

  public Url BaseUrl
  {
    get
    {
      if (this._baseUri != null)
        return this._baseUri;
      if (this._parent != null)
      {
        foreach (Node ancestor in this.Ancestors<Node>())
        {
          if (ancestor._baseUri != null)
            return ancestor._baseUri;
        }
      }
      Document owner = this.Owner;
      if (owner != null)
        return owner._baseUri ?? owner.DocumentUrl;
      return this._type == NodeType.Document ? ((Document) this).DocumentUrl : (Url) null;
    }
    set => this._baseUri = value;
  }

  public NodeType NodeType => this._type;

  public virtual string NodeValue
  {
    get => (string) null;
    set
    {
    }
  }

  public virtual string TextContent
  {
    get => (string) null;
    set
    {
    }
  }

  INode INode.PreviousSibling => (INode) this.PreviousSibling;

  INode INode.NextSibling => (INode) this.NextSibling;

  INode INode.FirstChild => (INode) this.FirstChild;

  INode INode.LastChild => (INode) this.LastChild;

  IDocument INode.Owner => (IDocument) this.Owner;

  INode INode.Parent => (INode) this._parent;

  public IElement ParentElement => this._parent as IElement;

  INodeList INode.ChildNodes => (INodeList) this._children;

  public string NodeName => this._name;

  internal Node PreviousSibling
  {
    get
    {
      if (this._parent != null)
      {
        int length = this._parent._children.Length;
        for (int index = 1; index < length; ++index)
        {
          if (this._parent._children[index] == this)
            return this._parent._children[index - 1];
        }
      }
      return (Node) null;
    }
  }

  internal Node NextSibling
  {
    get
    {
      if (this._parent != null)
      {
        int num = this._parent._children.Length - 1;
        for (int index = 0; index < num; ++index)
        {
          if (this._parent._children[index] == this)
            return this._parent._children[index + 1];
        }
      }
      return (Node) null;
    }
  }

  internal Node FirstChild => this._children.Length <= 0 ? (Node) null : this._children[0];

  internal Node LastChild
  {
    get => this._children.Length <= 0 ? (Node) null : this._children[this._children.Length - 1];
  }

  internal NodeList ChildNodes
  {
    get => this._children;
    set => this._children = value;
  }

  internal Node Parent
  {
    get => this._parent;
    set => this._parent = value;
  }

  internal Document Owner
  {
    get => this._type == NodeType.Document ? (Document) null : this._owner;
    set
    {
      foreach (Node node in this.DescendentsAndSelf<Node>())
      {
        Document owner = node.Owner;
        if (owner != value)
        {
          node._owner = value;
          if (owner != null)
            this.NodeIsAdopted(owner);
        }
      }
    }
  }

  internal void ReplaceAll(Node node, bool suppressObservers)
  {
    Document owner = this.Owner;
    if (node != null)
      owner.AdoptNode((INode) node);
    NodeList removedNodes = new NodeList();
    NodeList addedNodes = new NodeList();
    removedNodes.AddRange(this._children);
    if (node != null)
    {
      if (node.NodeType == NodeType.DocumentFragment)
        addedNodes.AddRange(node._children);
      else
        addedNodes.Add(node);
    }
    for (int index = 0; index < removedNodes.Length; ++index)
      this.RemoveChild(removedNodes[index], true);
    for (int index = 0; index < addedNodes.Length; ++index)
      this.InsertBefore(addedNodes[index], (Node) null, true);
    if (suppressObservers)
      return;
    owner.QueueMutation(MutationRecord.ChildList((INode) this, (INodeList) addedNodes, (INodeList) removedNodes));
  }

  internal INode InsertBefore(Node newElement, Node referenceElement, bool suppressObservers)
  {
    Document owner = this.Owner;
    int count = newElement.NodeType == NodeType.DocumentFragment ? newElement.ChildNodes.Length : 1;
    if (referenceElement != null && owner != null)
    {
      int childIndex = referenceElement.Index();
      owner.ForEachRange((Predicate<Range>) (m => m.Head == this && m.Start > childIndex), (Action<Range>) (m => m.StartWith((INode) this, m.Start + count)));
      owner.ForEachRange((Predicate<Range>) (m => m.Tail == this && m.End > childIndex), (Action<Range>) (m => m.EndWith((INode) this, m.End + count)));
    }
    if (newElement.NodeType == NodeType.Document || newElement.Contains((INode) this))
      throw new DomException(DomError.HierarchyRequest);
    NodeList addedNodes = new NodeList();
    int index1 = this._children.Index((INode) referenceElement);
    if (index1 == -1)
      index1 = this._children.Length;
    if (newElement._type == NodeType.DocumentFragment)
    {
      int index2 = index1;
      int index3 = index1;
      while (newElement.HasChildNodes)
      {
        Node childNode = newElement.ChildNodes[0];
        newElement.RemoveChild(childNode, true);
        this.InsertNode(index2, childNode);
        ++index2;
      }
      for (; index3 < index2; ++index3)
      {
        Node child = this._children[index3];
        addedNodes.Add(child);
        this.NodeIsInserted(child);
      }
    }
    else
    {
      addedNodes.Add(newElement);
      this.InsertNode(index1, newElement);
      this.NodeIsInserted(newElement);
    }
    if (!suppressObservers && owner != null)
      owner.QueueMutation(MutationRecord.ChildList((INode) this, (INodeList) addedNodes, previousSibling: index1 > 0 ? (INode) this._children[index1 - 1] : (INode) null, nextSibling: (INode) referenceElement));
    return (INode) newElement;
  }

  internal void RemoveChild(Node node, bool suppressObservers)
  {
    Document owner = this.Owner;
    int index = this._children.Index((INode) node);
    if (owner != null)
    {
      owner.ForEachRange((Predicate<Range>) (m => m.Head.IsInclusiveDescendantOf((INode) node)), (Action<Range>) (m => m.StartWith((INode) this, index)));
      owner.ForEachRange((Predicate<Range>) (m => m.Tail.IsInclusiveDescendantOf((INode) node)), (Action<Range>) (m => m.EndWith((INode) this, index)));
      owner.ForEachRange((Predicate<Range>) (m => m.Head == this && m.Start > index), (Action<Range>) (m => m.StartWith((INode) this, m.Start - 1)));
      owner.ForEachRange((Predicate<Range>) (m => m.Tail == this && m.End > index), (Action<Range>) (m => m.EndWith((INode) this, m.End - 1)));
    }
    Node child = index > 0 ? this._children[index - 1] : (Node) null;
    if (!suppressObservers && owner != null)
    {
      NodeList removedNodes = new NodeList() { node };
      owner.QueueMutation(MutationRecord.ChildList((INode) this, removedNodes: (INodeList) removedNodes, previousSibling: (INode) child, nextSibling: (INode) node.NextSibling));
      owner.AddTransientObserver((INode) node);
    }
    this.RemoveNode(index, node);
    this.NodeIsRemoved(node, child);
  }

  internal INode ReplaceChild(Node node, Node child, bool suppressObservers)
  {
    if (this.IsEndPoint() || node.IsHostIncludingInclusiveAncestor((INode) this))
      throw new DomException(DomError.HierarchyRequest);
    if (child.Parent != this)
      throw new DomException(DomError.NotFound);
    if (!node.IsInsertable())
      throw new DomException(DomError.HierarchyRequest);
    Node nextSibling = child.NextSibling;
    Document owner = this.Owner;
    NodeList addedNodes = new NodeList();
    NodeList removedNodes = new NodeList();
    if (this is IDocument parent && Node.IsChangeForbidden(node, parent, (INode) child))
      throw new DomException(DomError.HierarchyRequest);
    if (nextSibling == node)
      nextSibling = node.NextSibling;
    if (owner != null)
      owner.AdoptNode((INode) node);
    this.RemoveChild(child, true);
    this.InsertBefore(node, nextSibling, true);
    removedNodes.Add(child);
    if (node._type == NodeType.DocumentFragment)
      addedNodes.AddRange(node._children);
    else
      addedNodes.Add(node);
    if (!suppressObservers && owner != null)
      owner.QueueMutation(MutationRecord.ChildList((INode) this, (INodeList) addedNodes, (INodeList) removedNodes, (INode) child.PreviousSibling, (INode) nextSibling));
    return (INode) child;
  }

  public abstract Node Clone(Document newOwner, bool deep);

  public void AppendText(string s)
  {
    if (this.LastChild is TextNode lastChild)
      lastChild.Append(s);
    else
      this.AddNode((Node) new TextNode(this.Owner, s));
  }

  public void InsertText(int index, string s)
  {
    if (index > 0 && index <= this._children.Length && this._children[index - 1] is IText child2)
      child2.Append(s);
    else if (index >= 0 && index < this._children.Length && this._children[index] is IText child1)
      child1.Insert(0, s);
    else
      this.InsertNode(index, (Node) new TextNode(this.Owner, s));
  }

  public void InsertNode(int index, Node node)
  {
    node.Parent = this;
    this._children.Insert(index, node);
  }

  public void AddNode(Node node)
  {
    node.Parent = this;
    this._children.Add(node);
  }

  public void RemoveNode(int index, Node node)
  {
    node.Parent = (Node) null;
    this._children.RemoveAt(index);
  }

  public void ToHtml(TextWriter writer, IMarkupFormatter formatter)
  {
    IElement parentElement = this.ParentElement;
    foreach (INode node in this.GetDescendantsAndSelf())
    {
      switch (node)
      {
        case IComment comment:
          writer.Write(formatter.Comment(comment));
          break;
        case ICharacterData text:
          INode parent = text.Parent;
          if (parent != null && parent.Flags.HasFlag((Enum) NodeFlags.LiteralText))
          {
            writer.Write(text.Data);
            break;
          }
          writer.Write(formatter.Text(text));
          break;
        case IDocumentType doctype:
          writer.Write(formatter.Doctype(doctype));
          break;
        case IProcessingInstruction processing:
          writer.Write(formatter.Processing(processing));
          break;
        case IElement element1:
          NodeFlags flags = element1.Flags;
          bool selfClosing = flags.HasFlag((Enum) NodeFlags.SelfClosing);
          writer.Write(formatter.OpenTag(element1, selfClosing));
          if (!selfClosing && flags.HasFlag((Enum) NodeFlags.LineTolerance) && element1.FirstChild is IText firstChild && firstChild.Data.Has('\n'))
            writer.Write('\n');
          if (element1 is IHtmlTemplateElement htmlTemplateElement)
            htmlTemplateElement.Content.ToHtml(writer, formatter);
          if (!node.HasChildNodes)
          {
            writer.Write(formatter.CloseTag(element1, selfClosing));
            break;
          }
          break;
      }
      if (!node.HasChildNodes && node.NextSibling == null)
      {
        for (IElement element2 = node.ParentElement; element2 != parentElement; element2 = element2.NextSibling == null ? element2.ParentElement : parentElement)
          writer.Write(formatter.CloseTag(element2, element2.Flags.HasFlag((Enum) NodeFlags.SelfClosing)));
      }
    }
  }

  public INode AppendChild(INode child) => this.PreInsert(child, (INode) null);

  public INode InsertChild(int index, INode child)
  {
    Node child1 = index < this._children.Length ? this._children[index] : (Node) null;
    return this.PreInsert(child, (INode) child1);
  }

  public INode InsertBefore(INode newElement, INode referenceElement)
  {
    return this.PreInsert(newElement, referenceElement);
  }

  public INode ReplaceChild(INode newChild, INode oldChild)
  {
    return this.ReplaceChild(newChild as Node, oldChild as Node, false);
  }

  public INode RemoveChild(INode child) => this.PreRemove(child);

  public INode Clone(bool deep = true) => (INode) this.Clone(this.Owner, deep);

  public DocumentPositions CompareDocumentPosition(INode otherNode)
  {
    if (this == otherNode)
      return DocumentPositions.Same;
    if (this.Owner != otherNode.Owner)
      return DocumentPositions.Disconnected | DocumentPositions.ImplementationSpecific | (otherNode.GetHashCode() > this.GetHashCode() ? DocumentPositions.Following : DocumentPositions.Preceding);
    if (otherNode.IsAncestorOf((INode) this))
      return DocumentPositions.Preceding | DocumentPositions.Contains;
    if (otherNode.IsDescendantOf((INode) this))
      return DocumentPositions.Following | DocumentPositions.ContainedBy;
    return otherNode.IsPreceding((INode) this) ? DocumentPositions.Preceding : DocumentPositions.Following;
  }

  public bool Contains(INode otherNode) => this.IsInclusiveAncestorOf(otherNode);

  public void Normalize()
  {
    for (int index1 = 0; index1 < this._children.Length; ++index1)
    {
      TextNode text = this._children[index1] as TextNode;
      if (text != null)
      {
        int length = text.Length;
        if (length == 0)
        {
          this.RemoveChild((Node) text, false);
          --index1;
        }
        else
        {
          StringBuilder sb = StringBuilderPool.Obtain();
          TextNode sibling = text;
          int end = index1;
          Document owner = this.Owner;
          while ((sibling = sibling.NextSibling as TextNode) != null)
          {
            sb.Append(sibling.Data);
            end++;
            owner.ForEachRange((Predicate<Range>) (m => m.Head == sibling), (Action<Range>) (m => m.StartWith((INode) text, length)));
            owner.ForEachRange((Predicate<Range>) (m => m.Tail == sibling), (Action<Range>) (m => m.EndWith((INode) text, length)));
            owner.ForEachRange((Predicate<Range>) (m => m.Head == sibling.Parent && m.Start == end), (Action<Range>) (m => m.StartWith((INode) text, length)));
            owner.ForEachRange((Predicate<Range>) (m => m.Tail == sibling.Parent && m.End == end), (Action<Range>) (m => m.EndWith((INode) text, length)));
            length += sibling.Length;
          }
          text.Replace(text.Length, 0, sb.ToPool());
          for (int index2 = end; index2 > index1; --index2)
            this.RemoveChild(this._children[index2], false);
        }
      }
      else if (this._children[index1].HasChildNodes)
        this._children[index1].Normalize();
    }
  }

  public string LookupNamespaceUri(string prefix)
  {
    if (string.IsNullOrEmpty(prefix))
      prefix = (string) null;
    return this.LocateNamespace(prefix);
  }

  public string LookupPrefix(string namespaceUri)
  {
    return string.IsNullOrEmpty(namespaceUri) ? (string) null : this.LocatePrefix(namespaceUri);
  }

  public bool IsDefaultNamespace(string namespaceUri)
  {
    if (string.IsNullOrEmpty(namespaceUri))
      namespaceUri = (string) null;
    string other = this.LocateNamespace((string) null);
    return namespaceUri.Is(other);
  }

  public virtual bool Equals(INode otherNode)
  {
    if (!this.BaseUri.Is(otherNode.BaseUri) || !this.NodeName.Is(otherNode.NodeName) || this.ChildNodes.Length != otherNode.ChildNodes.Length)
      return false;
    for (int index = 0; index < this._children.Length; ++index)
    {
      if (!this._children[index].Equals(otherNode.ChildNodes[index]))
        return false;
    }
    return true;
  }

  private static bool IsChangeForbidden(Node node, IDocument parent, INode child)
  {
    switch (node._type)
    {
      case NodeType.Element:
        return parent.DocumentElement != child || child.IsFollowedByDoctype();
      case NodeType.DocumentType:
        return parent.Doctype != child || child.IsPrecededByElement();
      case NodeType.DocumentFragment:
        int elementCount = node.GetElementCount();
        if (elementCount > 1 || node.HasTextNodes())
          return true;
        if (elementCount != 1)
          return false;
        return parent.DocumentElement != child || child.IsFollowedByDoctype();
      default:
        return false;
    }
  }

  protected static void GetPrefixAndLocalName(
    string qualifiedName,
    ref string namespaceUri,
    out string prefix,
    out string localName)
  {
    if (!qualifiedName.IsXmlName())
      throw new DomException(DomError.InvalidCharacter);
    if (!qualifiedName.IsQualifiedName())
      throw new DomException(DomError.Namespace);
    if (string.IsNullOrEmpty(namespaceUri))
      namespaceUri = (string) null;
    int length = qualifiedName.IndexOf(':');
    if (length > 0)
    {
      prefix = qualifiedName.Substring(0, length);
      localName = qualifiedName.Substring(length + 1);
    }
    else
    {
      prefix = (string) null;
      localName = qualifiedName;
    }
    if (Node.IsNamespaceError(prefix, namespaceUri, qualifiedName))
      throw new DomException(DomError.Namespace);
  }

  protected static bool IsNamespaceError(string prefix, string namespaceUri, string qualifiedName)
  {
    if (prefix != null && namespaceUri == null || prefix.Is(NamespaceNames.XmlPrefix) && !namespaceUri.Is(NamespaceNames.XmlUri) || (qualifiedName.Is(NamespaceNames.XmlNsPrefix) || prefix.Is(NamespaceNames.XmlNsPrefix)) && !namespaceUri.Is(NamespaceNames.XmlNsUri))
      return true;
    return namespaceUri.Is(NamespaceNames.XmlNsUri) && !qualifiedName.Is(NamespaceNames.XmlNsPrefix) && !prefix.Is(NamespaceNames.XmlNsPrefix);
  }

  protected virtual string LocateNamespace(string prefix) => this._parent?.LocateNamespace(prefix);

  protected virtual string LocatePrefix(string namespaceUri)
  {
    return this._parent?.LocatePrefix(namespaceUri);
  }

  protected virtual void NodeIsAdopted(Document oldDocument)
  {
  }

  protected virtual void NodeIsInserted(Node newNode) => newNode.OnParentChanged();

  protected virtual void NodeIsRemoved(Node removedNode, Node oldPreviousSibling)
  {
    removedNode.OnParentChanged();
  }

  protected virtual void OnParentChanged()
  {
  }

  protected void CloneNode(Node target, Document owner, bool deep)
  {
    target._baseUri = this._baseUri;
    if (!deep)
      return;
    foreach (INode child in this._children)
    {
      if (child is Node node1)
      {
        Node node = node1.Clone(owner, true);
        target.AddNode(node);
      }
    }
  }
}
