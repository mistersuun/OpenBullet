// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Parser.CssCombinator
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css.Parser;

internal abstract class CssCombinator
{
  public static readonly CssCombinator Child = (CssCombinator) new CssCombinator.ChildCombinator();
  public static readonly CssCombinator Deep = (CssCombinator) new CssCombinator.DeepCombinator();
  public static readonly CssCombinator Descendent = (CssCombinator) new CssCombinator.DescendentCombinator();
  public static readonly CssCombinator AdjacentSibling = (CssCombinator) new CssCombinator.AdjacentSiblingCombinator();
  public static readonly CssCombinator Sibling = (CssCombinator) new CssCombinator.SiblingCombinator();
  public static readonly CssCombinator Namespace = (CssCombinator) new CssCombinator.NamespaceCombinator();
  public static readonly CssCombinator Column = (CssCombinator) new CssCombinator.ColumnCombinator();

  public Func<IElement, IEnumerable<IElement>> Transform { get; protected set; }

  public string Delimiter { get; protected set; }

  public virtual ISelector Change(ISelector selector) => selector;

  protected static IEnumerable<IElement> Single(IElement element)
  {
    if (element != null)
      yield return element;
  }

  private sealed class ChildCombinator : CssCombinator
  {
    public ChildCombinator()
    {
      this.Delimiter = CombinatorSymbols.Child;
      this.Transform = (Func<IElement, IEnumerable<IElement>>) (el => CssCombinator.Single(el.ParentElement));
    }
  }

  private sealed class DeepCombinator : CssCombinator
  {
    public DeepCombinator()
    {
      this.Delimiter = CombinatorSymbols.Deep;
      this.Transform = (Func<IElement, IEnumerable<IElement>>) (el => CssCombinator.Single(el.Parent is IShadowRoot ? ((IShadowRoot) el.Parent).Host : (IElement) null));
    }
  }

  private sealed class DescendentCombinator : CssCombinator
  {
    public DescendentCombinator()
    {
      this.Delimiter = CombinatorSymbols.Descendent;
      this.Transform = (Func<IElement, IEnumerable<IElement>>) (el =>
      {
        List<IElement> elementList = new List<IElement>();
        for (IElement parentElement = el.ParentElement; parentElement != null; parentElement = parentElement.ParentElement)
          elementList.Add(parentElement);
        return (IEnumerable<IElement>) elementList;
      });
    }
  }

  private sealed class AdjacentSiblingCombinator : CssCombinator
  {
    public AdjacentSiblingCombinator()
    {
      this.Delimiter = CombinatorSymbols.Adjacent;
      this.Transform = (Func<IElement, IEnumerable<IElement>>) (el => CssCombinator.Single(el.PreviousElementSibling));
    }
  }

  private sealed class SiblingCombinator : CssCombinator
  {
    public SiblingCombinator()
    {
      this.Delimiter = CombinatorSymbols.Sibling;
      this.Transform = (Func<IElement, IEnumerable<IElement>>) (el =>
      {
        IElement parentElement = el.ParentElement;
        if (parentElement == null)
          return (IEnumerable<IElement>) new IElement[0];
        List<IElement> elementList = new List<IElement>();
        foreach (INode childNode in (IEnumerable<INode>) parentElement.ChildNodes)
        {
          if (childNode is IElement element2)
          {
            if (element2 != el)
              elementList.Add(element2);
            else
              break;
          }
        }
        return (IEnumerable<IElement>) elementList;
      });
    }
  }

  private sealed class NamespaceCombinator : CssCombinator
  {
    public NamespaceCombinator()
    {
      this.Delimiter = CombinatorSymbols.Pipe;
      this.Transform = (Func<IElement, IEnumerable<IElement>>) (el => CssCombinator.Single(el));
    }

    public override ISelector Change(ISelector selector)
    {
      return (ISelector) new NamespaceSelector(selector.Text);
    }
  }

  private sealed class ColumnCombinator : CssCombinator
  {
    public ColumnCombinator() => this.Delimiter = CombinatorSymbols.Column;
  }
}
