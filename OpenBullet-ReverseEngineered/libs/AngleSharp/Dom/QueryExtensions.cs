// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.QueryExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Text;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom;

public static class QueryExtensions
{
  public static IElement QuerySelector(this INodeList nodes, string selectorText, INode scopeNode = null)
  {
    IElement scope = QueryExtensions.GetScope(scopeNode);
    ISelector selector = QueryExtensions.CreateSelector(nodes, (INode) scope, selectorText);
    return selector != null ? selector.MatchAny(nodes.OfType<IElement>(), scope) : (IElement) null;
  }

  public static IHtmlCollection<IElement> QuerySelectorAll(
    this INodeList nodes,
    string selectorText,
    INode scopeNode = null)
  {
    IElement scope = QueryExtensions.GetScope(scopeNode);
    ISelector selector = QueryExtensions.CreateSelector(nodes, (INode) scope, selectorText);
    return selector != null ? selector.MatchAll(nodes.OfType<IElement>(), scope) : (IHtmlCollection<IElement>) new HtmlCollection<IElement>(Enumerable.Empty<IElement>());
  }

  public static IHtmlCollection<IElement> GetElementsByClassName(
    this INodeList elements,
    string classNames)
  {
    List<IElement> result = new List<IElement>();
    string[] classNames1 = classNames.SplitSpaces();
    if (classNames1.Length != 0)
      elements.GetElementsByClassName(classNames1, result);
    return (IHtmlCollection<IElement>) new HtmlCollection<IElement>((IEnumerable<IElement>) result);
  }

  public static IHtmlCollection<IElement> GetElementsByTagName(
    this INodeList elements,
    string tagName)
  {
    List<IElement> result = new List<IElement>();
    elements.GetElementsByTagName(tagName.Is("*") ? (string) null : tagName, result);
    return (IHtmlCollection<IElement>) new HtmlCollection<IElement>((IEnumerable<IElement>) result);
  }

  public static IHtmlCollection<IElement> GetElementsByTagName(
    this INodeList elements,
    string namespaceUri,
    string localName)
  {
    List<IElement> result = new List<IElement>();
    elements.GetElementsByTagName(namespaceUri, localName.Is("*") ? (string) null : localName, result);
    return (IHtmlCollection<IElement>) new HtmlCollection<IElement>((IEnumerable<IElement>) result);
  }

  public static T QuerySelector<T>(this INodeList elements, ISelector selectors) where T : class, IElement
  {
    return elements.QuerySelector(selectors) as T;
  }

  public static IElement QuerySelector(this INodeList elements, ISelector selector)
  {
    for (int index = 0; index < elements.Length; ++index)
    {
      if (elements[index] is IElement element1)
      {
        if (selector.Match(element1))
          return element1;
        if (element1.HasChildNodes)
        {
          IElement element = element1.ChildNodes.QuerySelector(selector);
          if (element != null)
            return element;
        }
      }
    }
    return (IElement) null;
  }

  public static IHtmlCollection<IElement> QuerySelectorAll(
    this INodeList elements,
    ISelector selector)
  {
    List<IElement> result = new List<IElement>();
    elements.QuerySelectorAll(selector, result);
    return (IHtmlCollection<IElement>) new HtmlCollection<IElement>((IEnumerable<IElement>) result);
  }

  public static void QuerySelectorAll(
    this INodeList elements,
    ISelector selector,
    List<IElement> result)
  {
    for (int index = 0; index < elements.Length; ++index)
    {
      if (elements[index] is IElement element1)
      {
        foreach (IElement element in element1.DescendentsAndSelf<IElement>())
        {
          if (selector.Match(element))
            result.Add(element);
        }
      }
    }
  }

  public static bool Contains(this ITokenList list, string[] tokens)
  {
    for (int index = 0; index < tokens.Length; ++index)
    {
      if (!list.Contains(tokens[index]))
        return false;
    }
    return true;
  }

  private static void GetElementsByClassName(
    this INodeList elements,
    string[] classNames,
    List<IElement> result)
  {
    for (int index = 0; index < elements.Length; ++index)
    {
      if (elements[index] is IElement element)
      {
        if (element.ClassList.Contains(classNames))
          result.Add(element);
        if (element.ChildElementCount != 0)
          element.ChildNodes.GetElementsByClassName(classNames, result);
      }
    }
  }

  private static void GetElementsByTagName(
    this INodeList elements,
    string tagName,
    List<IElement> result)
  {
    for (int index = 0; index < elements.Length; ++index)
    {
      if (elements[index] is IElement element)
      {
        if (tagName == null || tagName.Isi(element.LocalName))
          result.Add(element);
        if (element.ChildElementCount != 0)
          element.ChildNodes.GetElementsByTagName(tagName, result);
      }
    }
  }

  private static void GetElementsByTagName(
    this INodeList elements,
    string namespaceUri,
    string localName,
    List<IElement> result)
  {
    for (int index = 0; index < elements.Length; ++index)
    {
      if (elements[index] is IElement element)
      {
        if (element.NamespaceUri.Is(namespaceUri) && (localName == null || localName.Isi(element.LocalName)))
          result.Add(element);
        if (element.ChildElementCount != 0)
          element.ChildNodes.GetElementsByTagName(namespaceUri, localName, result);
      }
    }
  }

  private static IElement GetScope(INode scopeNode)
  {
    IElement scope1;
    switch (scopeNode)
    {
      case IElement scope2:
        return scope2;
      case IDocument document:
        scope1 = document.DocumentElement;
        break;
      default:
        scope1 = (IElement) null;
        break;
    }
    if (scope1 != null)
      return scope1;
    return !(scopeNode is IShadowRoot shadowRoot) ? (IElement) null : shadowRoot.Host;
  }

  private static ISelector CreateSelector(INodeList nodes, INode scope, string selectorText)
  {
    INode node = nodes.Length > 0 ? nodes[0] : scope;
    ISelector selector = (ISelector) null;
    if (node != null)
      selector = node.Owner.Context.GetService<ICssSelectorParser>().ParseSelector(selectorText) ?? throw new DomException(DomError.Syntax);
    return selector;
  }
}
