// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.ElementExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Css.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using AngleSharp.Io.Processors;
using AngleSharp.Media;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Dom;

public static class ElementExtensions
{
  public static string LocatePrefixFor(this IElement element, string namespaceUri)
  {
    if (element.NamespaceUri.Is(namespaceUri) && element.Prefix != null)
      return element.Prefix;
    foreach (IAttr attribute in (IEnumerable<IAttr>) element.Attributes)
    {
      if (attribute.Prefix.Is(NamespaceNames.XmlNsPrefix) && attribute.Value.Is(namespaceUri))
        return attribute.LocalName;
    }
    IElement parentElement = element.ParentElement;
    return parentElement == null ? (string) null : parentElement.LocatePrefixFor(namespaceUri);
  }

  public static string LocateNamespaceFor(this IElement element, string prefix)
  {
    string namespaceUri = element.NamespaceUri;
    string prefix1 = element.Prefix;
    if (string.IsNullOrEmpty(namespaceUri) || !prefix1.Is(prefix))
    {
      if (!(prefix != null ? element.TryLocateCustomNamespace(prefix, out namespaceUri) : element.TryLocateStandardNamespace(out namespaceUri)))
      {
        IElement parentElement = element.ParentElement;
        namespaceUri = parentElement != null ? parentElement.LocateNamespaceFor(prefix) : (string) null;
      }
    }
    return namespaceUri;
  }

  public static string GetNamespaceUri(this IElement element)
  {
    string prefix = element.Prefix;
    string namespaceUri = string.Empty;
    if (!(prefix != null ? element.TryLocateCustomNamespace(prefix, out namespaceUri) : element.TryLocateStandardNamespace(out namespaceUri)))
    {
      IElement parentElement = element.ParentElement;
      namespaceUri = parentElement != null ? parentElement.LocateNamespaceFor(prefix) : (string) null;
    }
    return namespaceUri;
  }

  public static bool TryLocateCustomNamespace(
    this IElement element,
    string prefix,
    out string namespaceUri)
  {
    foreach (IAttr attribute in (IEnumerable<IAttr>) element.Attributes)
    {
      if (attribute.NamespaceUri.Is(NamespaceNames.XmlNsUri) && attribute.Prefix.Is(NamespaceNames.XmlNsPrefix) && attribute.LocalName.Is(prefix))
      {
        string str = attribute.Value;
        if (string.IsNullOrEmpty(str))
          str = (string) null;
        namespaceUri = str;
        return true;
      }
    }
    namespaceUri = (string) null;
    return false;
  }

  public static bool TryLocateStandardNamespace(this IElement element, out string namespaceUri)
  {
    foreach (IAttr attribute in (IEnumerable<IAttr>) element.Attributes)
    {
      if (attribute.Prefix == null && attribute.LocalName.Is(NamespaceNames.XmlNsPrefix))
      {
        string str = attribute.Value;
        if (string.IsNullOrEmpty(str))
          str = (string) null;
        namespaceUri = str;
        return true;
      }
    }
    namespaceUri = (string) null;
    return false;
  }

  public static ResourceRequest CreateRequestFor(this IElement element, Url url)
  {
    return new ResourceRequest(element, url);
  }

  public static bool MatchesCssNamespace(this IElement el, string prefix)
  {
    if (prefix.Is("*"))
      return true;
    string current = el.GetAttribute(NamespaceNames.XmlNsPrefix) ?? el.NamespaceUri;
    return prefix.Is(string.Empty) ? current.Is(string.Empty) : current.Is(el.GetCssNamespace(prefix));
  }

  public static string GetCssNamespace(this IElement el, string prefix)
  {
    IDocument owner = el.Owner;
    return (owner != null ? owner.StyleSheets.LocateNamespace(prefix) : (string) null) ?? el.LocateNamespaceFor(prefix);
  }

  public static bool IsHovered(this IElement element) => false;

  public static bool IsOnlyOfType(this IElement element)
  {
    IElement parentElement = element.ParentElement;
    if (parentElement == null)
      return false;
    for (int index = 0; index < parentElement.ChildNodes.Length; ++index)
    {
      if (parentElement.ChildNodes[index].NodeName.Is(element.NodeName) && parentElement.ChildNodes[index] != element)
        return false;
    }
    return true;
  }

  public static bool IsFirstOfType(this IElement element)
  {
    IElement parentElement = element.ParentElement;
    if (parentElement != null)
    {
      for (int index = 0; index < parentElement.ChildNodes.Length; ++index)
      {
        if (parentElement.ChildNodes[index].NodeName.Is(element.NodeName))
          return parentElement.ChildNodes[index] == element;
      }
    }
    return false;
  }

  public static bool IsLastOfType(this IElement element)
  {
    IElement parentElement = element.ParentElement;
    if (parentElement != null)
    {
      for (int index = parentElement.ChildNodes.Length - 1; index >= 0; --index)
      {
        if (parentElement.ChildNodes[index].NodeName.Is(element.NodeName))
          return parentElement.ChildNodes[index] == element;
      }
    }
    return false;
  }

  public static bool IsTarget(this IElement element)
  {
    string id = element.Id;
    string hash = element.Owner?.Location.Hash;
    return id != null && hash != null && string.Compare(id, 0, hash, hash.Length > 0 ? 1 : 0, int.MaxValue) == 0;
  }

  public static bool IsEnabled(this IElement element)
  {
    switch (element)
    {
      case HtmlAnchorElement _:
      case HtmlAreaElement _:
      case HtmlLinkElement _:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href));
      case HtmlButtonElement htmlButtonElement:
        return !htmlButtonElement.IsDisabled;
      case HtmlInputElement htmlInputElement:
        return !htmlInputElement.IsDisabled;
      case HtmlSelectElement htmlSelectElement:
        return !htmlSelectElement.IsDisabled;
      case HtmlTextAreaElement htmlTextAreaElement:
        return !htmlTextAreaElement.IsDisabled;
      case HtmlOptionElement htmlOptionElement:
        return !htmlOptionElement.IsDisabled;
      case HtmlOptionsGroupElement _:
      case HtmlMenuItemElement _:
      case HtmlFieldSetElement _:
        return string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Disabled));
      default:
        return false;
    }
  }

  public static bool IsDisabled(this IElement element)
  {
    switch (element)
    {
      case HtmlButtonElement htmlButtonElement:
        return htmlButtonElement.IsDisabled;
      case HtmlInputElement htmlInputElement:
        return htmlInputElement.IsDisabled;
      case HtmlSelectElement htmlSelectElement:
        return htmlSelectElement.IsDisabled;
      case HtmlTextAreaElement htmlTextAreaElement:
        return htmlTextAreaElement.IsDisabled;
      case HtmlOptionElement htmlOptionElement:
        return htmlOptionElement.IsDisabled;
      case HtmlOptionsGroupElement _:
      case HtmlMenuItemElement _:
      case HtmlFieldSetElement _:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Disabled));
      default:
        return false;
    }
  }

  public static bool IsDefault(this IElement element)
  {
    switch (element)
    {
      case HtmlButtonElement htmlButtonElement:
        if (htmlButtonElement.Form != null)
          return true;
        break;
      case HtmlInputElement htmlInputElement:
        string type = htmlInputElement.Type;
        if ((type == InputTypeNames.Submit || type == InputTypeNames.Image) && htmlInputElement.Form != null)
          return true;
        break;
      case HtmlOptionElement _:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Selected));
    }
    return false;
  }

  public static bool IsPseudo(this IElement element, string name)
  {
    return element is IPseudoElement pseudoElement && pseudoElement.PseudoName.Is(name);
  }

  public static bool IsChecked(this IElement element)
  {
    switch (element)
    {
      case HtmlInputElement htmlInputElement:
        return htmlInputElement.Type.IsOneOf(InputTypeNames.Checkbox, InputTypeNames.Radio) && htmlInputElement.IsChecked;
      case HtmlMenuItemElement htmlMenuItemElement:
        return htmlMenuItemElement.Type.IsOneOf(InputTypeNames.Checkbox, InputTypeNames.Radio) && htmlMenuItemElement.IsChecked;
      case HtmlOptionElement htmlOptionElement:
        return htmlOptionElement.IsSelected;
      default:
        return false;
    }
  }

  public static bool IsIndeterminate(this IElement element)
  {
    switch (element)
    {
      case HtmlInputElement htmlInputElement:
        return htmlInputElement.Type.Is(InputTypeNames.Checkbox) && htmlInputElement.IsIndeterminate;
      case HtmlProgressElement _:
        return string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Value));
      default:
        return false;
    }
  }

  public static bool IsPlaceholderShown(this IElement element)
  {
    return element is HtmlInputElement htmlInputElement && !string.IsNullOrEmpty(htmlInputElement.Placeholder) & string.IsNullOrEmpty(htmlInputElement.Value);
  }

  public static bool IsUnchecked(this IElement element)
  {
    switch (element)
    {
      case HtmlInputElement htmlInputElement:
        return htmlInputElement.Type.IsOneOf(InputTypeNames.Checkbox, InputTypeNames.Radio) && !htmlInputElement.IsChecked;
      case HtmlMenuItemElement htmlMenuItemElement:
        return htmlMenuItemElement.Type.IsOneOf(InputTypeNames.Checkbox, InputTypeNames.Radio) && !htmlMenuItemElement.IsChecked;
      case HtmlOptionElement htmlOptionElement:
        return !htmlOptionElement.IsSelected;
      default:
        return false;
    }
  }

  public static bool IsActive(this IElement element)
  {
    switch (element)
    {
      case HtmlAnchorElement htmlAnchorElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && htmlAnchorElement.IsActive;
      case HtmlAreaElement htmlAreaElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && htmlAreaElement.IsActive;
      case HtmlLinkElement htmlLinkElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && htmlLinkElement.IsActive;
      case HtmlButtonElement htmlButtonElement:
        return !htmlButtonElement.IsDisabled && htmlButtonElement.IsActive;
      case HtmlInputElement htmlInputElement:
        return htmlInputElement.Type.IsOneOf(InputTypeNames.Submit, InputTypeNames.Image, InputTypeNames.Reset, InputTypeNames.Button) && htmlInputElement.IsActive;
      case HtmlMenuItemElement htmlMenuItemElement:
        return !htmlMenuItemElement.IsDisabled && htmlMenuItemElement.IsActive;
      default:
        return false;
    }
  }

  public static bool IsVisited(this IElement element)
  {
    switch (element)
    {
      case HtmlAnchorElement htmlAnchorElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && htmlAnchorElement.IsVisited;
      case HtmlAreaElement htmlAreaElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && htmlAreaElement.IsVisited;
      case HtmlLinkElement htmlLinkElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && htmlLinkElement.IsVisited;
      default:
        return false;
    }
  }

  public static bool IsLink(this IElement element)
  {
    switch (element)
    {
      case HtmlAnchorElement htmlAnchorElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && !htmlAnchorElement.IsVisited;
      case HtmlAreaElement htmlAreaElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && !htmlAreaElement.IsVisited;
      case HtmlLinkElement htmlLinkElement:
        return !string.IsNullOrEmpty(element.GetAttribute((string) null, AttributeNames.Href)) && !htmlLinkElement.IsVisited;
      default:
        return false;
    }
  }

  public static bool IsShadow(this IElement element) => element?.ShadowRoot != null;

  public static bool IsOptional(this IElement element)
  {
    switch (element)
    {
      case HtmlInputElement htmlInputElement:
        return !htmlInputElement.IsRequired;
      case HtmlSelectElement htmlSelectElement:
        return !htmlSelectElement.IsRequired;
      case HtmlTextAreaElement htmlTextAreaElement:
        return !htmlTextAreaElement.IsRequired;
      default:
        return false;
    }
  }

  public static bool IsRequired(this IElement element)
  {
    switch (element)
    {
      case HtmlInputElement htmlInputElement:
        return htmlInputElement.IsRequired;
      case HtmlSelectElement htmlSelectElement:
        return htmlSelectElement.IsRequired;
      case HtmlTextAreaElement htmlTextAreaElement:
        return htmlTextAreaElement.IsRequired;
      default:
        return false;
    }
  }

  public static bool IsInvalid(this IElement element)
  {
    switch (element)
    {
      case IValidation validation:
        return !validation.CheckValidity();
      case HtmlFormElement htmlFormElement:
        return !htmlFormElement.CheckValidity();
      default:
        return false;
    }
  }

  public static bool IsValid(this IElement element)
  {
    switch (element)
    {
      case IValidation validation:
        return validation.CheckValidity();
      case HtmlFormElement htmlFormElement:
        return htmlFormElement.CheckValidity();
      default:
        return false;
    }
  }

  public static bool IsReadOnly(this IElement element)
  {
    switch (element)
    {
      case HtmlInputElement htmlInputElement:
        return !htmlInputElement.IsMutable;
      case HtmlTextAreaElement htmlTextAreaElement:
        return !htmlTextAreaElement.IsMutable;
      case IHtmlElement htmlElement:
        return !htmlElement.IsContentEditable;
      default:
        return true;
    }
  }

  public static bool IsEditable(this IElement element)
  {
    switch (element)
    {
      case HtmlInputElement htmlInputElement:
        return htmlInputElement.IsMutable;
      case HtmlTextAreaElement htmlTextAreaElement:
        return htmlTextAreaElement.IsMutable;
      case IHtmlElement htmlElement:
        return htmlElement.IsContentEditable;
      default:
        return false;
    }
  }

  public static bool IsOutOfRange(this IElement element)
  {
    if (!(element is IValidation validation))
      return false;
    IValidityState validity = validation.Validity;
    return validity.IsRangeOverflow || validity.IsRangeUnderflow;
  }

  public static bool IsInRange(this IElement element)
  {
    if (!(element is IValidation validation))
      return false;
    IValidityState validity = validation.Validity;
    return !validity.IsRangeOverflow && !validity.IsRangeUnderflow;
  }

  public static bool IsOnlyChild(this IElement element)
  {
    IElement parentElement = element.ParentElement;
    return parentElement != null && parentElement.ChildElementCount == 1 && parentElement.FirstElementChild == element;
  }

  public static bool IsFirstChild(this IElement element)
  {
    return element.ParentElement?.FirstElementChild == element;
  }

  public static bool IsLastChild(this IElement element)
  {
    return element.ParentElement?.LastElementChild == element;
  }

  public static T Attr<T>(this T elements, string attributeName, string attributeValue) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    attributeName = attributeName ?? throw new ArgumentNullException(nameof (attributeName));
    foreach (IElement element in (IEnumerable<IElement>) elements)
      element.SetAttribute(attributeName, attributeValue);
    return elements;
  }

  public static T Attr<T>(
    this T elements,
    IEnumerable<KeyValuePair<string, string>> attributes)
    where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    attributes = attributes ?? throw new ArgumentNullException(nameof (attributes));
    foreach (IElement element in (IEnumerable<IElement>) elements)
    {
      foreach (KeyValuePair<string, string> attribute in attributes)
        element.SetAttribute(attribute.Key, attribute.Value);
    }
    return elements;
  }

  public static T Attr<T>(this T elements, object attributes) where T : class, IEnumerable<IElement>
  {
    Dictionary<string, string> dictionary = attributes.ToDictionary();
    return ElementExtensions.Attr<T>(elements, (IEnumerable<KeyValuePair<string, string>>) dictionary);
  }

  public static IEnumerable<string> Attr<T>(this T elements, string attributeName) where T : IEnumerable<IElement>
  {
    return elements.Select<IElement, string>((Func<IElement, string>) (m => m.GetAttribute(attributeName)));
  }

  public static IElement ClearAttr(this IElement element)
  {
    element = element ?? throw new ArgumentNullException(nameof (element));
    element.Attributes.Clear();
    return element;
  }

  public static T ClearAttr<T>(this T elements) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
      element.ClearAttr();
    return elements;
  }

  public static IElement Empty(this IElement element)
  {
    element = element ?? throw new ArgumentNullException(nameof (element));
    element.InnerHtml = string.Empty;
    return element;
  }

  public static T Empty<T>(this T elements) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
      element.Empty();
    return elements;
  }

  public static string Html(this IElement element)
  {
    element = element ?? throw new ArgumentNullException(nameof (element));
    return element.InnerHtml;
  }

  public static T Html<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
      element.InnerHtml = html;
    return elements;
  }

  public static T AddClass<T>(this T elements, string className) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    className = className ?? throw new ArgumentNullException(nameof (className));
    string[] strArray = className.SplitSpaces();
    foreach (IElement element in (IEnumerable<IElement>) elements)
      element.ClassList.Add(strArray);
    return elements;
  }

  public static T RemoveClass<T>(this T elements, string className) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    className = className ?? throw new ArgumentNullException(nameof (className));
    string[] strArray = className.SplitSpaces();
    foreach (IElement element in (IEnumerable<IElement>) elements)
      element.ClassList.Remove(strArray);
    return elements;
  }

  public static T ToggleClass<T>(this T elements, string className) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    className = className ?? throw new ArgumentNullException(nameof (className));
    string[] strArray = className.SplitSpaces();
    foreach (IElement element in (IEnumerable<IElement>) elements)
    {
      foreach (string token in strArray)
        element.ClassList.Toggle(token);
    }
    return elements;
  }

  public static bool HasClass(this IEnumerable<IElement> elements, string className)
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    className = className ?? throw new ArgumentNullException(nameof (className));
    string[] strArray = className.SplitSpaces();
    foreach (IElement element in elements)
    {
      bool flag = true;
      foreach (string token in strArray)
      {
        if (!element.ClassList.Contains(token))
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return true;
    }
    return false;
  }

  public static T Before<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
    {
      IElement parentElement = element.ParentElement;
      if (parentElement != null)
      {
        IDocumentFragment fragment = parentElement.CreateFragment(html);
        parentElement.InsertBefore((INode) fragment, (INode) element);
      }
    }
    return elements;
  }

  public static T After<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
    {
      IElement parentElement = element.ParentElement;
      if (parentElement != null)
      {
        IDocumentFragment fragment = parentElement.CreateFragment(html);
        parentElement.InsertBefore((INode) fragment, element.NextSibling);
      }
    }
    return elements;
  }

  public static T Append<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
      element.Append((INode) element.CreateFragment(html));
    return elements;
  }

  public static T Prepend<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
    {
      IDocumentFragment fragment = element.CreateFragment(html);
      element.InsertBefore((INode) fragment, element.FirstChild);
    }
    return elements;
  }

  public static T Wrap<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
    {
      IDocumentFragment fragment = element.CreateFragment(html);
      IElement innerMostElement = fragment.GetInnerMostElement();
      element.Parent?.InsertBefore((INode) fragment, (INode) element);
      IElement child = element;
      innerMostElement.AppendChild((INode) child);
    }
    return elements;
  }

  public static T WrapInner<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    foreach (IElement element in (IEnumerable<IElement>) elements)
    {
      IDocumentFragment fragment = element.CreateFragment(html);
      IElement innerMostElement = fragment.GetInnerMostElement();
      while (element.ChildNodes.Length > 0)
      {
        INode childNode = element.ChildNodes[0];
        innerMostElement.AppendChild(childNode);
      }
      element.AppendChild((INode) fragment);
    }
    return elements;
  }

  public static T WrapAll<T>(this T elements, string html) where T : class, IEnumerable<IElement>
  {
    elements = elements ?? throw new ArgumentNullException(nameof (elements));
    IElement element1 = elements.FirstOrDefault<IElement>();
    if (element1 != null)
    {
      IDocumentFragment fragment = element1.CreateFragment(html);
      IElement innerMostElement = fragment.GetInnerMostElement();
      element1.Parent?.InsertBefore((INode) fragment, (INode) element1);
      foreach (IElement element2 in (IEnumerable<IElement>) elements)
        innerMostElement.AppendChild((INode) element2);
    }
    return elements;
  }

  public static IHtmlCollection<TElement> ToCollection<TElement>(this IEnumerable<TElement> elements) where TElement : class, IElement
  {
    return (IHtmlCollection<TElement>) new HtmlCollection<TElement>(elements);
  }

  public static Task<IDocument> NavigateAsync<TElement>(this TElement element) where TElement : class, IUrlUtilities, IElement
  {
    return element.NavigateAsync<TElement>(CancellationToken.None);
  }

  public static Task<IDocument> NavigateAsync<TElement>(
    this TElement element,
    CancellationToken cancel)
    where TElement : class, IUrlUtilities, IElement
  {
    element = element ?? throw new ArgumentNullException(nameof (element));
    IDocument owner = element.Owner;
    DocumentRequest request = DocumentRequest.Get(Url.Create(element.Href), (INode) element, owner.DocumentUri);
    return owner.Context.NavigateToAsync(request);
  }

  internal static void Process(this Element element, IRequestProcessor processor, Url url)
  {
    ResourceRequest requestFor = element.CreateRequestFor(url);
    Task task = processor?.ProcessAsync(requestFor);
    element.Owner?.DelayLoad(task);
  }

  internal static Url GetImageCandidate(this HtmlImageElement img)
  {
    SourceSet sourceSet = new SourceSet();
    IBrowsingContext context = img.Context;
    Stack<IHtmlSourceElement> sources = img.GetSources();
    while (sources.Count > 0)
    {
      IHtmlSourceElement htmlSourceElement = sources.Pop();
      string type = htmlSourceElement.Type;
      if (string.IsNullOrEmpty(type) || context.GetResourceService<IImageInfo>(type) != null)
      {
        using (IEnumerator<string> enumerator = sourceSet.GetCandidates(htmlSourceElement.SourceSet, htmlSourceElement.Sizes).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            string current = enumerator.Current;
            return new Url(img.BaseUrl, current);
          }
        }
      }
    }
    using (IEnumerator<string> enumerator = sourceSet.GetCandidates(img.SourceSet, img.Sizes).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        string current = enumerator.Current;
        return new Url(img.BaseUrl, current);
      }
    }
    return !string.IsNullOrEmpty(img.Source) ? Url.Create(img.Source) : (Url) null;
  }

  internal static string GetOwnAttribute(this Element element, string name)
  {
    return element.Attributes.GetNamedItem((string) null, name)?.Value;
  }

  internal static bool HasOwnAttribute(this Element element, string name)
  {
    return element.Attributes.GetNamedItem((string) null, name) != null;
  }

  internal static string GetUrlAttribute(this Element element, string name)
  {
    string ownAttribute = element.GetOwnAttribute(name);
    Url url = ownAttribute != null ? new Url(element.BaseUrl, ownAttribute) : (Url) null;
    return url == null || url.IsInvalid ? string.Empty : url.Href;
  }

  internal static bool IsBooleanAttribute(this IElement element, string name)
  {
    switch (element)
    {
      case HtmlDetailsElement _ when name.Is(AttributeNames.Open):
      case HtmlDialogElement _ when name.Is(AttributeNames.Open):
      case HtmlElement _ when name.Is(AttributeNames.Hidden):
      case HtmlFormControlElement _ when name.Is(AttributeNames.AutoFocus):
      case HtmlFormControlElement _ when name.Is(AttributeNames.Disabled):
      case HtmlLinkElement _ when name.Is(AttributeNames.Disabled):
      case HtmlIFrameElement _ when name.Is(AttributeNames.SrcDoc):
      case HtmlIFrameElement _ when name.Is(AttributeNames.AllowFullscreen):
      case HtmlIFrameElement _ when name.Is(AttributeNames.AllowPaymentRequest):
      case HtmlImageElement _ when name.Is(AttributeNames.IsMap):
      case HtmlInputElement _ when name.Is(AttributeNames.Checked):
      case HtmlInputElement _ when name.Is(AttributeNames.Multiple):
      case HtmlTrackElement _ when name.Is(AttributeNames.Default):
      case HtmlTextFormControlElement _ when name.Is(AttributeNames.Required):
      case HtmlTextFormControlElement _ when name.Is(AttributeNames.Readonly):
      case HtmlStyleElement _ when name.Is(AttributeNames.Scoped):
      case HtmlStyleElement _ when name.Is(AttributeNames.Disabled):
      case HtmlSelectElement _ when name.Is(AttributeNames.Required):
      case HtmlSelectElement _ when name.Is(AttributeNames.Multiple):
      case HtmlScriptElement _ when name.Is(AttributeNames.Defer):
      case HtmlScriptElement _ when name.Is(AttributeNames.Async):
      case HtmlOrderedListElement _ when name.Is(AttributeNames.Reversed):
      case HtmlOptionsGroupElement _ when name.Is(AttributeNames.Disabled):
      case HtmlOptionElement _ when name.Is(AttributeNames.Disabled):
      case HtmlOptionElement _ when name.Is(AttributeNames.Selected):
      case HtmlObjectElement _ when name.Is(AttributeNames.TypeMustMatch):
      case HtmlMenuItemElement _ when name.Is(AttributeNames.Disabled):
      case HtmlMenuItemElement _ when name.Is(AttributeNames.Checked):
      case HtmlMenuItemElement _ when name.Is(AttributeNames.Default):
      case IHtmlMediaElement _ when name.Is(AttributeNames.Autoplay):
      case IHtmlMediaElement _ when name.Is(AttributeNames.Loop):
      case IHtmlMediaElement _ when name.Is(AttributeNames.Muted):
        return true;
      case IHtmlMediaElement _:
        return name.Is(AttributeNames.Controls);
      default:
        return false;
    }
  }

  internal static bool GetBoolAttribute(this Element element, string name)
  {
    return element.GetOwnAttribute(name) != null;
  }

  internal static void SetBoolAttribute(this Element element, string name, bool value)
  {
    if (value)
      element.SetOwnAttribute(name, string.Empty);
    else
      element.Attributes.RemoveNamedItemOrDefault(name, true);
  }

  internal static void SetOwnAttribute(
    this Element element,
    string name,
    string value,
    bool suppressCallbacks = false)
  {
    element.Attributes.SetNamedItemWithNamespaceUri((IAttr) new AngleSharp.Dom.Attr(name, value), suppressCallbacks);
  }

  private static IDocumentFragment CreateFragment(this IElement context, string html)
  {
    return (IDocumentFragment) new DocumentFragment(context as Element, html ?? string.Empty);
  }

  private static IElement GetInnerMostElement(this IDocumentFragment fragment)
  {
    IElement element = fragment.ChildElementCount == 1 ? fragment.FirstElementChild : throw new InvalidOperationException("The provided HTML code did not result in any element.");
    IElement innerMostElement;
    do
    {
      innerMostElement = element;
      element = innerMostElement.FirstElementChild;
    }
    while (element != null);
    return innerMostElement;
  }

  public static string GetSelector(this IElement element)
  {
    string selector = string.Empty;
    bool flag;
    do
    {
      flag = !string.IsNullOrEmpty(element.Id);
      IElement parentElement = element.ParentElement;
      string str = element.LocalName;
      if (flag)
        str = "#" + element.Id;
      else if (parentElement != null && !element.IsOnlyOfType())
      {
        int num = ((IEnumerable<INode>) parentElement.Children.Where<IElement>((Func<IElement, bool>) (_ => _.GetType() == element.GetType()))).Index((INode) element);
        str += $":nth-child({num + 1})";
      }
      selector = string.IsNullOrEmpty(selector) ? str : $"{str}>{selector}";
      element = parentElement;
    }
    while (element?.ParentElement != null && !flag);
    return selector;
  }

  internal static IElement ParseHtmlSubtree(this Element element, string html)
  {
    IBrowsingContext context = element.Context;
    TextSource source = new TextSource(html);
    return new HtmlDomBuilder(new HtmlDocument(context, source)).ParseFragment(new HtmlParserOptions()
    {
      IsEmbedded = false,
      IsStrictMode = false,
      IsScripting = context.IsScripting()
    }, element).DocumentElement;
  }
}
