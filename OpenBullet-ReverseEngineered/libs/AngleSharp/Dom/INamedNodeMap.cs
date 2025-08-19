// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.INamedNodeMap
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

[DomName("NamedNodeMap")]
public interface INamedNodeMap : IEnumerable<IAttr>, IEnumerable
{
  [DomName("item")]
  [DomAccessor(Accessors.Getter)]
  IAttr this[int index] { get; }

  [DomAccessor(Accessors.Getter)]
  IAttr this[string name] { get; }

  [DomName("length")]
  int Length { get; }

  [DomName("getNamedItem")]
  IAttr GetNamedItem(string name);

  [DomName("setNamedItem")]
  IAttr SetNamedItem(IAttr item);

  [DomName("removeNamedItem")]
  IAttr RemoveNamedItem(string name);

  [DomName("getNamedItemNS")]
  IAttr GetNamedItem(string namespaceUri, string localName);

  [DomName("setNamedItemNS")]
  IAttr SetNamedItemWithNamespaceUri(IAttr item);

  [DomName("removeNamedItemNS")]
  IAttr RemoveNamedItem(string namespaceUri, string localName);
}
