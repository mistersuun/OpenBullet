// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.HtmlAllCollection
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class HtmlAllCollection : 
  IHtmlAllCollection,
  IHtmlCollection<IElement>,
  IEnumerable<IElement>,
  IEnumerable
{
  private readonly IEnumerable<IElement> _elements;

  public HtmlAllCollection(IDocument document) => this._elements = document.GetNodes<IElement>();

  public IElement this[int index] => this._elements.GetItemByIndex<IElement>(index);

  public IElement this[string id] => this._elements.GetElementById<IElement>(id);

  public int Length => this._elements.Count<IElement>();

  public IEnumerator<IElement> GetEnumerator() => this._elements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._elements.GetEnumerator();
}
