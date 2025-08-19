// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.SelectorExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

public static class SelectorExtensions
{
  public static T Eq<T>(this IEnumerable<T> elements, int index) where T : IElement
  {
    return elements != null ? elements.Skip<T>(index).FirstOrDefault<T>() : throw new ArgumentNullException(nameof (elements));
  }

  public static IEnumerable<T> Gt<T>(this IEnumerable<T> elements, int index) where T : IElement
  {
    return elements != null ? elements.Skip<T>(index + 1) : throw new ArgumentNullException(nameof (elements));
  }

  public static IEnumerable<T> Lt<T>(this IEnumerable<T> elements, int index) where T : IElement
  {
    return elements != null ? elements.Take<T>(index) : throw new ArgumentNullException(nameof (elements));
  }

  public static IEnumerable<T> Even<T>(this IEnumerable<T> elements) where T : IElement
  {
    if (elements == null)
      throw new ArgumentNullException(nameof (elements));
    bool even = true;
    foreach (T element in elements)
    {
      if (even)
        yield return element;
      even = !even;
    }
  }

  public static IEnumerable<T> Odd<T>(this IEnumerable<T> elements) where T : IElement
  {
    if (elements == null)
      throw new ArgumentNullException(nameof (elements));
    bool odd = false;
    foreach (T element in elements)
    {
      if (odd)
        yield return element;
      odd = !odd;
    }
  }

  public static IEnumerable<T> Filter<T>(this IEnumerable<T> elements, string selectorText) where T : IElement
  {
    return elements.Filter<T>(selectorText, true);
  }

  public static IEnumerable<T> Not<T>(this IEnumerable<T> elements, string selectorText) where T : IElement
  {
    return elements.Filter<T>(selectorText, false);
  }

  public static IEnumerable<IElement> Children(
    this IEnumerable<IElement> elements,
    string selectorText = null)
  {
    return elements.GetMany((Func<IElement, IEnumerable<IElement>>) (m => (IEnumerable<IElement>) m.Children), selectorText);
  }

  public static IEnumerable<IElement> Siblings(
    this IEnumerable<IElement> elements,
    string selectorText = null)
  {
    return elements.GetMany((Func<IElement, IEnumerable<IElement>>) (m => m.Parent.ChildNodes.OfType<IElement>().Except(m)), selectorText);
  }

  public static IEnumerable<IElement> Parent(
    this IEnumerable<IElement> elements,
    string selectorText = null)
  {
    return elements.Get((Func<IElement, IElement>) (m => m.ParentElement), selectorText);
  }

  public static IEnumerable<IElement> Next(this IEnumerable<IElement> elements, string selectorText = null)
  {
    return elements.Get((Func<IElement, IElement>) (m => m.NextElementSibling), selectorText);
  }

  public static IEnumerable<IElement> Previous(
    this IEnumerable<IElement> elements,
    string selectorText = null)
  {
    return elements.Get((Func<IElement, IElement>) (m => m.PreviousElementSibling), selectorText);
  }

  public static IEnumerable<T> Is<T>(this IEnumerable<T> elements, ISelector selector) where T : IElement
  {
    return elements.Filter<T>(selector, true);
  }

  public static IEnumerable<T> Not<T>(this IEnumerable<T> elements, ISelector selector) where T : IElement
  {
    return elements.Filter<T>(selector, false);
  }

  public static IEnumerable<IElement> Children(
    this IEnumerable<IElement> elements,
    ISelector selector = null)
  {
    return elements.GetMany((Func<IElement, IEnumerable<IElement>>) (m => (IEnumerable<IElement>) m.Children), selector);
  }

  public static IEnumerable<IElement> Siblings(
    this IEnumerable<IElement> elements,
    ISelector selector = null)
  {
    return elements.GetMany((Func<IElement, IEnumerable<IElement>>) (m => m.Parent.ChildNodes.OfType<IElement>().Except(m)), selector);
  }

  public static IEnumerable<IElement> Parent(
    this IEnumerable<IElement> elements,
    ISelector selector = null)
  {
    return elements.Get((Func<IElement, IElement>) (m => m.ParentElement), selector);
  }

  public static IEnumerable<IElement> Next(this IEnumerable<IElement> elements, ISelector selector = null)
  {
    return elements.Get((Func<IElement, IElement>) (m => m.NextElementSibling), selector);
  }

  public static IEnumerable<IElement> Previous(
    this IEnumerable<IElement> elements,
    ISelector selector = null)
  {
    return elements.Get((Func<IElement, IElement>) (m => m.PreviousElementSibling), selector);
  }

  private static IEnumerable<IElement> GetMany(
    this IEnumerable<IElement> elements,
    Func<IElement, IEnumerable<IElement>> getter,
    ISelector selector)
  {
    if (selector == null)
      selector = AllSelector.Instance;
    foreach (IElement element1 in elements)
    {
      foreach (IElement element2 in getter(element1))
      {
        if (selector.Match(element2))
          yield return element2;
      }
    }
  }

  private static IEnumerable<IElement> GetMany(
    this IEnumerable<IElement> elements,
    Func<IElement, IEnumerable<IElement>> getter,
    string selectorText)
  {
    ISelector selector = SelectorExtensions.CreateSelector<IElement>(elements, selectorText);
    return elements.GetMany(getter, selector);
  }

  private static IEnumerable<IElement> Get(
    this IEnumerable<IElement> elements,
    Func<IElement, IElement> getter,
    ISelector selector)
  {
    if (selector == null)
      selector = AllSelector.Instance;
    foreach (IElement element1 in elements)
    {
      for (IElement element2 = getter(element1); element2 != null; element2 = getter(element2))
      {
        if (selector.Match(element2))
        {
          yield return element2;
          break;
        }
      }
    }
  }

  private static IEnumerable<IElement> Get(
    this IEnumerable<IElement> elements,
    Func<IElement, IElement> getter,
    string selectorText)
  {
    ISelector selector = SelectorExtensions.CreateSelector<IElement>(elements, selectorText);
    return elements.Get(getter, selector);
  }

  private static IEnumerable<IElement> Except(
    this IEnumerable<IElement> elements,
    IElement excluded)
  {
    foreach (IElement element in elements)
    {
      if (element != excluded)
        yield return element;
    }
  }

  private static IEnumerable<T> Filter<T>(
    this IEnumerable<T> elements,
    ISelector selector,
    bool result)
    where T : IElement
  {
    if (selector == null)
      selector = AllSelector.Instance;
    foreach (T element in elements)
    {
      if (selector.Match((IElement) element) == result)
        yield return element;
    }
  }

  private static IEnumerable<T> Filter<T>(
    this IEnumerable<T> elements,
    string selectorText,
    bool result)
    where T : IElement
  {
    ISelector selector = SelectorExtensions.CreateSelector<T>(elements, selectorText);
    return elements.Filter<T>(selector, result);
  }

  private static ISelector CreateSelector<T>(IEnumerable<T> elements, string selector) where T : IElement
  {
    if (selector != null)
    {
      T obj = elements.FirstOrDefault<T>();
      if ((object) obj != null)
        return obj.Owner.Context.GetService<ICssSelectorParser>().ParseSelector(selector);
    }
    return AllSelector.Instance;
  }
}
