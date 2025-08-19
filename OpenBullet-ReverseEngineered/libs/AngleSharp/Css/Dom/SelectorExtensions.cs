// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.SelectorExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Css.Dom;

public static class SelectorExtensions
{
  public static IElement MatchAny(
    this ISelector selector,
    IEnumerable<IElement> elements,
    IElement scope)
  {
    foreach (INode element1 in elements)
    {
      foreach (IElement element2 in element1.DescendentsAndSelf<IElement>())
      {
        if (selector.Match(element2, scope))
          return element2;
      }
    }
    return (IElement) null;
  }

  public static IHtmlCollection<IElement> MatchAll(
    this ISelector selector,
    IEnumerable<IElement> elements,
    IElement scope)
  {
    List<IElement> result = new List<IElement>();
    selector.MatchAll(elements, scope, result);
    return (IHtmlCollection<IElement>) new HtmlCollection<IElement>((IEnumerable<IElement>) result);
  }

  public static bool Match(this ISelector selector, IElement element)
  {
    return selector.Match(element, element?.Owner.DocumentElement);
  }

  private static void MatchAll(
    this ISelector selector,
    IEnumerable<IElement> elements,
    IElement scope,
    List<IElement> result)
  {
    foreach (INode element1 in elements)
    {
      foreach (IElement element2 in element1.DescendentsAndSelf<IElement>())
      {
        if (selector.Match(element2, scope))
          result.Add(element2);
      }
    }
  }
}
