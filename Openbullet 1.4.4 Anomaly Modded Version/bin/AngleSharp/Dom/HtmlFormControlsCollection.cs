// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.HtmlFormControlsCollection
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class HtmlFormControlsCollection : 
  IHtmlFormControlsCollection,
  IHtmlCollection<IHtmlElement>,
  IEnumerable<IHtmlElement>,
  IEnumerable
{
  private readonly IEnumerable<HtmlFormControlElement> _elements;

  public HtmlFormControlsCollection(IElement form, IElement root = null)
  {
    if (root == null)
      root = form.Owner.DocumentElement;
    this._elements = root.GetNodes<HtmlFormControlElement>().Where<HtmlFormControlElement>((Func<HtmlFormControlElement, bool>) (m => m.Form == form && (!(m is IHtmlInputElement htmlInputElement) || !htmlInputElement.Type.Is(InputTypeNames.Image))));
  }

  public int Length => this._elements.Count<HtmlFormControlElement>();

  public HtmlFormControlElement this[int index]
  {
    get => this._elements.GetItemByIndex<HtmlFormControlElement>(index);
  }

  public HtmlFormControlElement this[string id]
  {
    get => this._elements.GetElementById<HtmlFormControlElement>(id);
  }

  public IEnumerator<HtmlFormControlElement> GetEnumerator() => this._elements.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._elements.GetEnumerator();

  IHtmlElement IHtmlCollection<IHtmlElement>.this[int index] => (IHtmlElement) this[index];

  IHtmlElement IHtmlCollection<IHtmlElement>.this[string id] => (IHtmlElement) this[id];

  IEnumerator<IHtmlElement> IEnumerable<IHtmlElement>.GetEnumerator()
  {
    return (IEnumerator<IHtmlElement>) this._elements.GetEnumerator();
  }
}
