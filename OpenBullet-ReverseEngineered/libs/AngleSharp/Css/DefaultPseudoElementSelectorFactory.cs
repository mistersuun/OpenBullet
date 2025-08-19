// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.DefaultPseudoElementSelectorFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css;

public class DefaultPseudoElementSelectorFactory : IPseudoElementSelectorFactory
{
  private readonly Dictionary<string, ISelector> _selectors = new Dictionary<string, ISelector>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    {
      PseudoElementNames.Before,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => el.IsPseudo(PseudoElementNames.Before)), PseudoElementNames.Before)
    },
    {
      PseudoElementNames.After,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => el.IsPseudo(PseudoElementNames.After)), PseudoElementNames.After)
    },
    {
      PseudoElementNames.Selection,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => false), PseudoElementNames.Selection)
    },
    {
      PseudoElementNames.FootnoteCall,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => false), PseudoElementNames.FootnoteCall)
    },
    {
      PseudoElementNames.FootnoteMarker,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => false), PseudoElementNames.FootnoteMarker)
    },
    {
      PseudoElementNames.FirstLine,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => el.HasChildNodes && el.ChildNodes[0].NodeType == NodeType.Text), PseudoElementNames.FirstLine)
    },
    {
      PseudoElementNames.FirstLetter,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => el.HasChildNodes && el.ChildNodes[0].NodeType == NodeType.Text && el.ChildNodes[0].TextContent.Length > 0), PseudoElementNames.FirstLetter)
    },
    {
      PseudoElementNames.Content,
      (ISelector) new PseudoElementSelector((Predicate<IElement>) (el => false), PseudoElementNames.Content)
    }
  };

  public void Register(string name, ISelector selector) => this._selectors.Add(name, selector);

  public ISelector Unregister(string name)
  {
    ISelector selector;
    if (this._selectors.TryGetValue(name, out selector))
      this._selectors.Remove(name);
    return selector;
  }

  protected virtual ISelector CreateDefault(string name) => (ISelector) null;

  public ISelector Create(string name)
  {
    ISelector selector;
    return this._selectors.TryGetValue(name, out selector) ? selector : this.CreateDefault(name);
  }
}
