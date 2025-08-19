// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.NamedNodeMap
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class NamedNodeMap : INamedNodeMap, IEnumerable<IAttr>, IEnumerable
{
  private readonly List<Attr> _items;
  private readonly WeakReference<Element> _owner;

  public NamedNodeMap(Element owner)
  {
    this._items = new List<Attr>();
    this._owner = new WeakReference<Element>(owner);
  }

  public IAttr this[string name] => this.GetNamedItem(name);

  public IAttr this[int index]
  {
    get => index < 0 || index >= this._items.Count ? (IAttr) null : (IAttr) this._items[index];
  }

  public int Length => this._items.Count;

  internal void FastAddItem(Attr attr) => this._items.Add(attr);

  internal void RaiseChangedEvent(Attr attr, string newValue, string oldValue)
  {
    Element element = (Element) null;
    if (!this._owner.TryGetTarget(ref element))
      return;
    element.AttributeChanged(attr.LocalName, attr.NamespaceUri, oldValue, newValue);
  }

  internal IAttr RemoveNamedItemOrDefault(string name, bool suppressMutationObservers)
  {
    for (int index = 0; index < this._items.Count; ++index)
    {
      if (name.Is(this._items[index].Name))
      {
        Attr attr = this._items[index];
        this._items.RemoveAt(index);
        attr.Container = (NamedNodeMap) null;
        if (!suppressMutationObservers)
          this.RaiseChangedEvent(attr, (string) null, attr.Value);
        return (IAttr) attr;
      }
    }
    return (IAttr) null;
  }

  internal IAttr RemoveNamedItemOrDefault(string name)
  {
    return this.RemoveNamedItemOrDefault(name, false);
  }

  internal IAttr RemoveNamedItemOrDefault(
    string namespaceUri,
    string localName,
    bool suppressMutationObservers)
  {
    for (int index = 0; index < this._items.Count; ++index)
    {
      if (localName.Is(this._items[index].LocalName) && namespaceUri.Is(this._items[index].NamespaceUri))
      {
        Attr attr = this._items[index];
        this._items.RemoveAt(index);
        attr.Container = (NamedNodeMap) null;
        if (!suppressMutationObservers)
          this.RaiseChangedEvent(attr, (string) null, attr.Value);
        return (IAttr) attr;
      }
    }
    return (IAttr) null;
  }

  internal IAttr RemoveNamedItemOrDefault(string namespaceUri, string localName)
  {
    return this.RemoveNamedItemOrDefault(namespaceUri, localName, false);
  }

  public IAttr GetNamedItem(string name)
  {
    for (int index = 0; index < this._items.Count; ++index)
    {
      if (name.Is(this._items[index].Name))
        return (IAttr) this._items[index];
    }
    return (IAttr) null;
  }

  public IAttr GetNamedItem(string namespaceUri, string localName)
  {
    for (int index = 0; index < this._items.Count; ++index)
    {
      if (localName.Is(this._items[index].LocalName) && namespaceUri.Is(this._items[index].NamespaceUri))
        return (IAttr) this._items[index];
    }
    return (IAttr) null;
  }

  public IAttr SetNamedItem(IAttr item)
  {
    Attr attr1 = this.Prepare(item);
    if (attr1 != null)
    {
      string name = item.Name;
      for (int index = 0; index < this._items.Count; ++index)
      {
        if (name.Is(this._items[index].Name))
        {
          Attr attr2 = this._items[index];
          this._items[index] = attr1;
          this.RaiseChangedEvent(attr1, attr1.Value, attr2.Value);
          return (IAttr) attr2;
        }
      }
      this._items.Add(attr1);
      this.RaiseChangedEvent(attr1, attr1.Value, (string) null);
    }
    return (IAttr) null;
  }

  public IAttr SetNamedItemWithNamespaceUri(IAttr item, bool suppressMutationObservers)
  {
    Attr attr1 = this.Prepare(item);
    if (attr1 != null)
    {
      string localName = item.LocalName;
      string namespaceUri = item.NamespaceUri;
      for (int index = 0; index < this._items.Count; ++index)
      {
        if (localName.Is(this._items[index].LocalName) && namespaceUri.Is(this._items[index].NamespaceUri))
        {
          Attr attr2 = this._items[index];
          this._items[index] = attr1;
          if (!suppressMutationObservers)
            this.RaiseChangedEvent(attr1, attr1.Value, attr2.Value);
          return (IAttr) attr2;
        }
      }
      this._items.Add(attr1);
      if (!suppressMutationObservers)
        this.RaiseChangedEvent(attr1, attr1.Value, (string) null);
    }
    return (IAttr) null;
  }

  public IAttr SetNamedItemWithNamespaceUri(IAttr item)
  {
    return this.SetNamedItemWithNamespaceUri(item, false);
  }

  public IAttr RemoveNamedItem(string name)
  {
    return this.RemoveNamedItemOrDefault(name) ?? throw new DomException(DomError.NotFound);
  }

  public IAttr RemoveNamedItem(string namespaceUri, string localName)
  {
    return this.RemoveNamedItemOrDefault(namespaceUri, localName) ?? throw new DomException(DomError.NotFound);
  }

  public IEnumerator<IAttr> GetEnumerator() => (IEnumerator<IAttr>) this._items.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._items.GetEnumerator();

  private Attr Prepare(IAttr item)
  {
    if (item is Attr attr)
    {
      if (attr.Container == this)
        return (Attr) null;
      attr.Container = attr.Container == null ? this : throw new DomException(DomError.InUse);
    }
    return attr;
  }
}
