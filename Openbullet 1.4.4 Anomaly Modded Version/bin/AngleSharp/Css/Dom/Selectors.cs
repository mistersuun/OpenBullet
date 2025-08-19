// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.Selectors
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css.Dom;

internal abstract class Selectors : IEnumerable<ISelector>, IEnumerable
{
  protected readonly List<ISelector> _selectors;

  public Selectors() => this._selectors = new List<ISelector>();

  public Priority Specificity
  {
    get
    {
      Priority specificity = new Priority();
      for (int index = 0; index < this._selectors.Count; ++index)
        specificity += this._selectors[index].Specificity;
      return specificity;
    }
  }

  public string Text => this.Stringify();

  public int Length => this._selectors.Count;

  public ISelector this[int index]
  {
    get => this._selectors[index];
    set => this._selectors[index] = value;
  }

  protected abstract string Stringify();

  public void Add(ISelector selector) => this._selectors.Add(selector);

  public void Remove(ISelector selector) => this._selectors.Remove(selector);

  public IEnumerator<ISelector> GetEnumerator()
  {
    return (IEnumerator<ISelector>) this._selectors.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
