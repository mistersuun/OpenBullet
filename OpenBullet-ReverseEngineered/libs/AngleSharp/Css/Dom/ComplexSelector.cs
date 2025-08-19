// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.ComplexSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class ComplexSelector : ISelector
{
  private readonly List<ComplexSelector.CombinatorSelector> _combinators;

  public ComplexSelector() => this._combinators = new List<ComplexSelector.CombinatorSelector>();

  public Priority Specificity
  {
    get
    {
      Priority specificity = new Priority();
      int count = this._combinators.Count;
      for (int index = 0; index < count; ++index)
        specificity += this._combinators[index].Selector.Specificity;
      return specificity;
    }
  }

  public string Text
  {
    get
    {
      string[] strArray1 = new string[2 * this._combinators.Count + 1];
      if (this._combinators.Count > 0)
      {
        int index1 = 0;
        int index2 = this._combinators.Count - 1;
        for (int index3 = 0; index3 < index2; ++index3)
        {
          string[] strArray2 = strArray1;
          int index4 = index1;
          int num = index4 + 1;
          string text = this._combinators[index3].Selector.Text;
          strArray2[index4] = text;
          string[] strArray3 = strArray1;
          int index5 = num;
          index1 = index5 + 1;
          string delimiter = this._combinators[index3].Delimiter;
          strArray3[index5] = delimiter;
        }
        strArray1[index1] = this._combinators[index2].Selector.Text;
      }
      return string.Concat(strArray1);
    }
  }

  public int Length => this._combinators.Count;

  public bool IsReady { get; private set; }

  public void Accept(ISelectorVisitor visitor)
  {
    IEnumerable<ISelector> selectors = this._combinators.Select<ComplexSelector.CombinatorSelector, ISelector>((Func<ComplexSelector.CombinatorSelector, ISelector>) (m => m.Selector));
    IEnumerable<string> symbols = this._combinators.Take<ComplexSelector.CombinatorSelector>(this._combinators.Count - 1).Select<ComplexSelector.CombinatorSelector, string>((Func<ComplexSelector.CombinatorSelector, string>) (m => m.Delimiter));
    visitor.Combinator(selectors, symbols);
  }

  public bool Match(IElement element, IElement scope)
  {
    int index = this._combinators.Count - 1;
    if (!this._combinators[index].Selector.Match(element, scope))
      return false;
    return index <= 0 || this.MatchCascade(index - 1, element, scope);
  }

  public void ConcludeSelector(ISelector selector)
  {
    if (this.IsReady)
      return;
    this._combinators.Add(new ComplexSelector.CombinatorSelector()
    {
      Selector = selector,
      Transform = (Func<IElement, IEnumerable<IElement>>) null,
      Delimiter = (string) null
    });
    this.IsReady = true;
  }

  public void AppendSelector(ISelector selector, CssCombinator combinator)
  {
    if (this.IsReady)
      return;
    this._combinators.Add(new ComplexSelector.CombinatorSelector()
    {
      Selector = combinator.Change(selector),
      Transform = combinator.Transform,
      Delimiter = combinator.Delimiter
    });
  }

  private bool MatchCascade(int pos, IElement element, IElement scope)
  {
    foreach (IElement element1 in this._combinators[pos].Transform(element))
    {
      if (this._combinators[pos].Selector.Match(element1, scope) && (pos == 0 || this.MatchCascade(pos - 1, element1, scope)))
        return true;
    }
    return false;
  }

  private struct CombinatorSelector
  {
    public string Delimiter;
    public Func<IElement, IEnumerable<IElement>> Transform;
    public ISelector Selector;
  }
}
