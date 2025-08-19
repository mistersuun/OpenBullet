// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.HtmlCollection`1
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class HtmlCollection<T> : IHtmlCollection<T>, IEnumerable<T>, IEnumerable where T : class, IElement
{
  private readonly IEnumerable<T> _elements;

  public HtmlCollection(IEnumerable<T> elements) => this._elements = elements;

  public HtmlCollection(INode parent, bool deep = true, Func<T, bool> predicate = null)
  {
    this._elements = parent.GetNodes<T>(deep, predicate);
  }

  public T this[int index] => this._elements.GetItemByIndex<T>(index);

  public T this[string id] => this._elements.GetElementById<T>(id);

  public int Length => this._elements.Count<T>();

  public IEnumerator<T> GetEnumerator() => this._elements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._elements.GetEnumerator();
}
