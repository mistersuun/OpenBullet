// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.INode
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("Node")]
public interface INode : IEventTarget, IMarkupFormattable
{
  [DomName("baseURI")]
  string BaseUri { get; }

  Url BaseUrl { get; }

  [DomName("nodeName")]
  string NodeName { get; }

  [DomName("childNodes")]
  INodeList ChildNodes { get; }

  [DomName("cloneNode")]
  INode Clone(bool deep = true);

  [DomName("isEqualNode")]
  bool Equals(INode otherNode);

  [DomName("compareDocumentPosition")]
  DocumentPositions CompareDocumentPosition(INode otherNode);

  [DomName("normalize")]
  void Normalize();

  [DomName("ownerDocument")]
  IDocument Owner { get; }

  [DomName("parentElement")]
  IElement ParentElement { get; }

  [DomName("parentNode")]
  INode Parent { get; }

  [DomName("contains")]
  bool Contains(INode otherNode);

  [DomName("firstChild")]
  INode FirstChild { get; }

  [DomName("lastChild")]
  INode LastChild { get; }

  [DomName("nextSibling")]
  INode NextSibling { get; }

  [DomName("previousSibling")]
  INode PreviousSibling { get; }

  [DomName("isDefaultNamespace")]
  bool IsDefaultNamespace(string namespaceUri);

  [DomName("lookupNamespaceURI")]
  string LookupNamespaceUri(string prefix);

  [DomName("lookupPrefix")]
  string LookupPrefix(string namespaceUri);

  [DomName("nodeType")]
  NodeType NodeType { get; }

  [DomName("nodeValue")]
  string NodeValue { get; set; }

  [DomName("textContent")]
  string TextContent { get; set; }

  [DomName("hasChildNodes")]
  bool HasChildNodes { get; }

  [DomName("appendChild")]
  INode AppendChild(INode child);

  [DomName("insertBefore")]
  INode InsertBefore(INode newElement, INode referenceElement);

  [DomName("removeChild")]
  INode RemoveChild(INode child);

  [DomName("replaceChild")]
  INode ReplaceChild(INode newChild, INode oldChild);

  NodeFlags Flags { get; }
}
