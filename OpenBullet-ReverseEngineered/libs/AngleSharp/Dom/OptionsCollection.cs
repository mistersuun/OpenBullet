// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.OptionsCollection
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class OptionsCollection : 
  IHtmlOptionsCollection,
  IHtmlCollection<IHtmlOptionElement>,
  IEnumerable<IHtmlOptionElement>,
  IEnumerable
{
  private readonly IElement _parent;
  private readonly IEnumerable<IHtmlOptionElement> _options;

  public OptionsCollection(IElement parent)
  {
    this._parent = parent;
    this._options = this.GetOptions();
  }

  public IHtmlOptionElement this[int index] => this.GetOptionAt(index);

  public IHtmlOptionElement this[string name]
  {
    get
    {
      if (string.IsNullOrEmpty(name))
        return (IHtmlOptionElement) null;
      foreach (IHtmlOptionElement option in this._options)
      {
        if (option.Id.Is(name))
          return option;
      }
      return this._parent.Children[name] as IHtmlOptionElement;
    }
  }

  public int SelectedIndex
  {
    get
    {
      int selectedIndex = 0;
      foreach (IHtmlOptionElement option in this._options)
      {
        if (option.IsSelected)
          return selectedIndex;
        ++selectedIndex;
      }
      return -1;
    }
    set
    {
      int num = 0;
      foreach (IHtmlOptionElement option in this._options)
        option.IsSelected = num++ == value;
    }
  }

  public int Length => this._options.Count<IHtmlOptionElement>();

  public IHtmlOptionElement GetOptionAt(int index)
  {
    return this._options.GetItemByIndex<IHtmlOptionElement>(index);
  }

  public void SetOptionAt(int index, IHtmlOptionElement value)
  {
    IHtmlOptionElement optionAt = this.GetOptionAt(index);
    if (optionAt != null)
      this._parent.ReplaceChild((INode) value, (INode) optionAt);
    else
      this._parent.AppendChild((INode) value);
  }

  public void Add(IHtmlOptionElement element, IHtmlElement before = null)
  {
    this._parent.InsertBefore((INode) element, (INode) before);
  }

  public void Add(IHtmlOptionsGroupElement element, IHtmlElement before = null)
  {
    this._parent.InsertBefore((INode) element, (INode) before);
  }

  public void Remove(int index)
  {
    if (index < 0 || index >= this.Length)
      return;
    this._parent.RemoveChild((INode) this.GetOptionAt(index));
  }

  private IEnumerable<IHtmlOptionElement> GetOptions()
  {
    foreach (INode childNode1 in (IEnumerable<INode>) this._parent.ChildNodes)
    {
      if (childNode1 is IHtmlOptionsGroupElement optionsGroupElement)
      {
        foreach (INode childNode2 in (IEnumerable<INode>) optionsGroupElement.ChildNodes)
        {
          if (childNode2 is IHtmlOptionElement option)
            yield return option;
        }
      }
      else if (childNode1 is IHtmlOptionElement)
        yield return (IHtmlOptionElement) childNode1;
    }
  }

  public IEnumerator<IHtmlOptionElement> GetEnumerator() => this._options.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
