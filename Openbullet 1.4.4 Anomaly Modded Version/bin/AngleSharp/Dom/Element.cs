// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Element
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Dom.Events;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace AngleSharp.Dom;

public abstract class Element : 
  Node,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode
{
  private static readonly AttachedProperty<Element, IShadowRoot> ShadowRootProperty = new AttachedProperty<Element, IShadowRoot>();
  private readonly NamedNodeMap _attributes;
  private readonly string _namespace;
  private readonly string _prefix;
  private readonly string _localName;
  private HtmlCollection<IElement> _elements;
  private TokenList _classList;

  public Element(
    Document owner,
    string localName,
    string prefix,
    string namespaceUri,
    NodeFlags flags = NodeFlags.None)
    : this(owner, prefix != null ? $"{prefix}:{localName}" : localName, localName, prefix, namespaceUri, flags)
  {
  }

  public Element(
    Document owner,
    string name,
    string localName,
    string prefix,
    string namespaceUri,
    NodeFlags flags = NodeFlags.None)
    : base(owner, name, flags: flags)
  {
    this._localName = localName;
    this._prefix = prefix;
    this._namespace = namespaceUri;
    this._attributes = new NamedNodeMap(this);
  }

  internal IBrowsingContext Context => this.Owner?.Context;

  internal NamedNodeMap Attributes => this._attributes;

  public IElement AssignedSlot
  {
    get
    {
      IElement parentElement = this.ParentElement;
      if (parentElement == null)
        return (IElement) null;
      IShadowRoot shadowRoot = parentElement.ShadowRoot;
      return shadowRoot == null ? (IElement) null : shadowRoot.GetAssignedSlot(this.Slot);
    }
  }

  public string Slot
  {
    get => this.GetOwnAttribute(AttributeNames.Slot);
    set => this.SetOwnAttribute(AttributeNames.Slot, value);
  }

  public IShadowRoot ShadowRoot => Element.ShadowRootProperty.Get(this);

  public string Prefix => this._prefix;

  public string LocalName => this._localName;

  public string NamespaceUri => this._namespace ?? this.GetNamespaceUri();

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

  public ITokenList ClassList
  {
    get
    {
      if (this._classList == null)
      {
        this._classList = new TokenList(this.GetOwnAttribute(AttributeNames.Class));
        this._classList.Changed += (Action<string>) (value => this.UpdateAttribute(AttributeNames.Class, value));
      }
      return (ITokenList) this._classList;
    }
  }

  public string ClassName
  {
    get => this.GetOwnAttribute(AttributeNames.Class);
    set => this.SetOwnAttribute(AttributeNames.Class, value);
  }

  public string Id
  {
    get => this.GetOwnAttribute(AttributeNames.Id);
    set => this.SetOwnAttribute(AttributeNames.Id, value);
  }

  public string TagName => this.NodeName;

  public ISourceReference SourceReference { get; set; }

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

  public int ChildElementCount
  {
    get
    {
      NodeList childNodes = this.ChildNodes;
      int length = childNodes.Length;
      int childElementCount = 0;
      for (int index = 0; index < length; ++index)
      {
        if (childNodes[index].NodeType == NodeType.Element)
          ++childElementCount;
      }
      return childElementCount;
    }
  }

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

  public string InnerHtml
  {
    get => this.ChildNodes.ToHtml();
    set => this.ReplaceAll((Node) new DocumentFragment(this, value), false);
  }

  public string OuterHtml
  {
    get => this.ToHtml();
    set
    {
      Node node = this.Parent;
      if (node != null)
      {
        switch (node.NodeType)
        {
          case NodeType.Document:
            throw new DomException(DomError.NoModificationAllowed);
          case NodeType.DocumentFragment:
            node = (Node) new HtmlBodyElement(this.Owner);
            break;
        }
      }
      if (!(node is Element element))
        throw new DomException(DomError.NotSupported);
      element.InsertChild(element.IndexOf((INode) this), (INode) new DocumentFragment(element, value));
      element.RemoveChild((INode) this);
    }
  }

  INamedNodeMap IElement.Attributes => (INamedNodeMap) this._attributes;

  public bool IsFocused
  {
    get => this.Owner?.FocusElement == this;
    protected set
    {
      Document document = this.Owner;
      Document document1 = document;
      if (document1 == null)
        return;
      document1.QueueTask((Action) (() =>
      {
        if (value)
        {
          document.SetFocus((IElement) this);
          this.Fire<FocusEvent>((Action<FocusEvent>) (m => m.Init(EventNames.Focus, false, false)));
        }
        else
        {
          document.SetFocus((IElement) null);
          this.Fire<FocusEvent>((Action<FocusEvent>) (m => m.Init(EventNames.Blur, false, false)));
        }
      }));
    }
  }

  public abstract IElement ParseSubtree(string source);

  public IShadowRoot AttachShadow(ShadowRootMode mode = ShadowRootMode.Open)
  {
    if (TagNames.AllNoShadowRoot.Contains(this._localName))
      throw new DomException(DomError.NotSupported);
    if (this.ShadowRoot != null)
      throw new DomException(DomError.InvalidState);
    AngleSharp.Dom.ShadowRoot shadowRoot = new AngleSharp.Dom.ShadowRoot(this, mode);
    Element.ShadowRootProperty.Set(this, (IShadowRoot) shadowRoot);
    return (IShadowRoot) shadowRoot;
  }

  public IElement QuerySelector(string selectors)
  {
    return this.ChildNodes.QuerySelector(selectors, (INode) this);
  }

  public IHtmlCollection<IElement> QuerySelectorAll(string selectors)
  {
    return this.ChildNodes.QuerySelectorAll(selectors, (INode) this);
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

  public bool Matches(string selectorText)
  {
    return (this.Context.GetService<ICssSelectorParser>().ParseSelector(selectorText) ?? throw new DomException(DomError.Syntax)).Match((IElement) this, (IElement) this);
  }

  public IElement Closest(string selectorText)
  {
    ISelector selector = this.Context.GetService<ICssSelectorParser>().ParseSelector(selectorText) ?? throw new DomException(DomError.Syntax);
    for (IElement element = (IElement) this; element != null; element = element.ParentElement)
    {
      if (selector.Match(element, element))
        return element;
    }
    return (IElement) null;
  }

  public bool HasAttribute(string name)
  {
    if (this._namespace.Is(NamespaceNames.HtmlUri))
      name = name.HtmlLower();
    return this._attributes.GetNamedItem(name) != null;
  }

  public bool HasAttribute(string namespaceUri, string localName)
  {
    if (string.IsNullOrEmpty(namespaceUri))
      namespaceUri = (string) null;
    return this._attributes.GetNamedItem(namespaceUri, localName) != null;
  }

  public string GetAttribute(string name)
  {
    if (this._namespace.Is(NamespaceNames.HtmlUri))
      name = name.HtmlLower();
    return this._attributes.GetNamedItem(name)?.Value;
  }

  public string GetAttribute(string namespaceUri, string localName)
  {
    if (string.IsNullOrEmpty(namespaceUri))
      namespaceUri = (string) null;
    return this._attributes.GetNamedItem(namespaceUri, localName)?.Value;
  }

  public void SetAttribute(string name, string value)
  {
    if (value != null)
    {
      if (!name.IsXmlName())
        throw new DomException(DomError.InvalidCharacter);
      if (this._namespace.Is(NamespaceNames.HtmlUri))
        name = name.HtmlLower();
      this.SetOwnAttribute(name, value);
    }
    else
      this.RemoveAttribute(name);
  }

  public void SetAttribute(string namespaceUri, string name, string value)
  {
    if (value != null)
    {
      string prefix;
      string localName;
      Node.GetPrefixAndLocalName(name, ref namespaceUri, out prefix, out localName);
      this._attributes.SetNamedItem((IAttr) new Attr(prefix, localName, value, namespaceUri));
    }
    else
      this.RemoveAttribute(namespaceUri, name);
  }

  public void AddAttribute(Attr attr) => this._attributes.FastAddItem(attr);

  public bool RemoveAttribute(string name)
  {
    if (this._namespace.Is(NamespaceNames.HtmlUri))
      name = name.HtmlLower();
    return this._attributes.RemoveNamedItemOrDefault(name) != null;
  }

  public bool RemoveAttribute(string namespaceUri, string localName)
  {
    if (string.IsNullOrEmpty(namespaceUri))
      namespaceUri = (string) null;
    return this._attributes.RemoveNamedItemOrDefault(namespaceUri, localName) != null;
  }

  public void Prepend(params INode[] nodes) => this.PrependNodes(nodes);

  public void Append(params INode[] nodes) => this.AppendNodes(nodes);

  public override bool Equals(INode otherNode)
  {
    return otherNode is IElement element && this.NamespaceUri.Is(element.NamespaceUri) && this._attributes.SameAs(element.Attributes) && base.Equals(otherNode);
  }

  public void Before(params INode[] nodes) => this.InsertBefore(nodes);

  public void After(params INode[] nodes) => this.InsertAfter(nodes);

  public void Replace(params INode[] nodes) => this.ReplaceWith(nodes);

  public void Remove() => this.RemoveFromParent();

  public void Insert(AdjacentPosition position, string html)
  {
    Element contextElement;
    if ((position == AdjacentPosition.AfterBegin ? 1 : (position == AdjacentPosition.BeforeEnd ? 1 : 0)) == 0)
      contextElement = this.Parent is Element parent ? parent : throw new DomException("The element has no parent.");
    else
      contextElement = this;
    string html1 = html;
    DocumentFragment documentFragment = new DocumentFragment(contextElement, html1);
    switch (position)
    {
      case AdjacentPosition.BeforeBegin:
        this.Parent.InsertBefore((INode) documentFragment, (INode) this);
        break;
      case AdjacentPosition.AfterBegin:
        this.InsertChild(0, (INode) documentFragment);
        break;
      case AdjacentPosition.BeforeEnd:
        this.AppendChild((INode) documentFragment);
        break;
      case AdjacentPosition.AfterEnd:
        this.Parent.InsertChild(this.Parent.IndexOf((INode) this) + 1, (INode) documentFragment);
        break;
    }
  }

  public override Node Clone(Document owner, bool deep)
  {
    AnyElement anyElement = new AnyElement(owner, this.LocalName, this._prefix, this._namespace, this.Flags);
    this.CloneElement((Element) anyElement, owner, deep);
    return (Node) anyElement;
  }

  internal virtual void SetupElement()
  {
    NamedNodeMap attributes = this._attributes;
    if (attributes.Length <= 0)
      return;
    IEnumerable<IAttributeObserver> services = this.Context.GetServices<IAttributeObserver>();
    foreach (IAttr attr in attributes)
    {
      string localName = attr.LocalName;
      string str = attr.Value;
      foreach (IAttributeObserver attributeObserver in services)
        attributeObserver.NotifyChange((IElement) this, localName, str);
    }
  }

  internal void AttributeChanged(
    string localName,
    string namespaceUri,
    string oldValue,
    string newValue)
  {
    if (namespaceUri == null)
    {
      foreach (IAttributeObserver service in this.Context.GetServices<IAttributeObserver>())
        service.NotifyChange((IElement) this, localName, newValue);
    }
    this.Owner.QueueMutation(MutationRecord.Attributes((INode) this, localName, namespaceUri, oldValue));
  }

  internal void UpdateClassList(string value) => this._classList?.Update(value);

  protected void UpdateAttribute(string name, string value)
  {
    this.SetOwnAttribute(name, value, true);
  }

  protected sealed override string LocateNamespace(string prefix)
  {
    return this.LocateNamespaceFor(prefix);
  }

  protected sealed override string LocatePrefix(string namespaceUri)
  {
    return this.LocatePrefixFor(namespaceUri);
  }

  protected void CloneElement(Element element, Document owner, bool deep)
  {
    this.CloneNode((Node) element, owner, deep);
    foreach (IAttr attribute in this._attributes)
    {
      Attr attr = new Attr(attribute.Prefix, attribute.LocalName, attribute.Value, attribute.NamespaceUri);
      element._attributes.FastAddItem(attr);
    }
    element.SetupElement();
  }
}
