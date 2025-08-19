// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.IElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom;

[DomName("Element")]
public interface IElement : 
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode
{
  [DomName("prefix")]
  string Prefix { get; }

  [DomName("localName")]
  string LocalName { get; }

  [DomName("namespaceURI")]
  string NamespaceUri { get; }

  [DomName("attributes")]
  INamedNodeMap Attributes { get; }

  [DomName("classList")]
  ITokenList ClassList { get; }

  [DomName("className")]
  string ClassName { get; set; }

  [DomName("id")]
  string Id { get; set; }

  [DomName("insertAdjacentHTML")]
  void Insert(AdjacentPosition position, string html);

  [DomName("hasAttribute")]
  bool HasAttribute(string name);

  [DomName("hasAttributeNS")]
  bool HasAttribute(string namespaceUri, string localName);

  [DomName("getAttribute")]
  string GetAttribute(string name);

  [DomName("getAttributeNS")]
  string GetAttribute(string namespaceUri, string localName);

  [DomName("setAttribute")]
  void SetAttribute(string name, string value);

  [DomName("setAttributeNS")]
  void SetAttribute(string namespaceUri, string name, string value);

  [DomName("removeAttribute")]
  bool RemoveAttribute(string name);

  [DomName("removeAttributeNS")]
  bool RemoveAttribute(string namespaceUri, string localName);

  [DomName("getElementsByClassName")]
  IHtmlCollection<IElement> GetElementsByClassName(string classNames);

  [DomName("getElementsByTagName")]
  IHtmlCollection<IElement> GetElementsByTagName(string tagName);

  [DomName("getElementsByTagNameNS")]
  IHtmlCollection<IElement> GetElementsByTagNameNS(string namespaceUri, string tagName);

  [DomName("matches")]
  bool Matches(string selectors);

  [DomName("closest")]
  IElement Closest(string selectors);

  [DomName("innerHTML")]
  string InnerHtml { get; set; }

  [DomName("outerHTML")]
  string OuterHtml { get; set; }

  [DomName("tagName")]
  string TagName { get; }

  [DomName("attachShadow")]
  [DomInitDict(0, false)]
  IShadowRoot AttachShadow(ShadowRootMode mode = ShadowRootMode.Open);

  [DomName("assignedSlot")]
  IElement AssignedSlot { get; }

  [DomName("slot")]
  string Slot { get; set; }

  [DomName("shadowRoot")]
  IShadowRoot ShadowRoot { get; }

  bool IsFocused { get; }

  ISourceReference SourceReference { get; }
}
