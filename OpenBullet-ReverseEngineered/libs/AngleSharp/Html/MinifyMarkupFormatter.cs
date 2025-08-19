// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.MinifyMarkupFormatter
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace AngleSharp.Html;

public class MinifyMarkupFormatter : IMarkupFormatter
{
  private IEnumerable<string> _preservedTags = (IEnumerable<string>) new string[2]
  {
    TagNames.Pre,
    TagNames.Textarea
  };

  public IEnumerable<string> PreservedTags
  {
    get => this._preservedTags ?? Enumerable.Empty<string>();
    set => this._preservedTags = value;
  }

  public bool ShouldKeepStandardElements { get; set; }

  public bool ShouldKeepComments { get; set; }

  public bool ShouldKeepAttributeQuotes { get; set; }

  public bool ShouldKeepEmptyAttributes { get; set; }

  public bool ShouldKeepImpliedEndTag { get; set; }

  string IMarkupFormatter.Comment(IComment comment)
  {
    return !this.ShouldKeepComments ? string.Empty : HtmlMarkupFormatter.Instance.Comment(comment);
  }

  string IMarkupFormatter.Doctype(IDocumentType doctype)
  {
    return HtmlMarkupFormatter.Instance.Doctype(doctype);
  }

  string IMarkupFormatter.Processing(IProcessingInstruction processing)
  {
    return HtmlMarkupFormatter.Instance.Processing(processing);
  }

  string IMarkupFormatter.Text(ICharacterData text)
  {
    if (text.Parent is IHtmlHeadElement || text.Parent is IHtmlHtmlElement)
      return string.Empty;
    string str = HtmlMarkupFormatter.Instance.Text(text);
    if (this.PreservedTags.Contains<string>(text.ParentElement?.LocalName))
      return str;
    StringBuilder sb = StringBuilderPool.Obtain();
    bool flag1 = false;
    bool flag2 = true;
    for (int index = 0; index < str.Length; ++index)
    {
      char c = str[index];
      if (c.IsWhiteSpaceCharacter())
      {
        if (!flag1)
        {
          sb.Append(' ');
          flag1 = true;
        }
      }
      else
      {
        sb.Append(c);
        flag2 = false;
        flag1 = false;
      }
    }
    return !flag2 || MinifyMarkupFormatter.ShouldOutput(text) ? sb.ToPool() : string.Empty;
  }

  string IMarkupFormatter.OpenTag(IElement element, bool selfClosing)
  {
    if (this.CanBeRemoved(element))
      return string.Empty;
    StringBuilder sb = StringBuilderPool.Obtain();
    sb.Append('<');
    if (!string.IsNullOrEmpty(element.Prefix))
      sb.Append(element.Prefix).Append(':');
    sb.Append(element.LocalName);
    foreach (IAttr attribute in (IEnumerable<IAttr>) element.Attributes)
    {
      if (MinifyMarkupFormatter.ShouldKeep(element, attribute))
      {
        if (!element.IsBooleanAttribute(attribute.Name))
        {
          string str = this.Serialize(attribute);
          if (!string.IsNullOrEmpty(str))
            sb.Append(' ').Append(str);
        }
        else
          sb.Append(' ').Append(attribute.Name);
      }
    }
    sb.Append('>');
    return sb.ToPool();
  }

  string IMarkupFormatter.CloseTag(IElement element, bool selfClosing)
  {
    return !this.CanBeRemoved(element) && !this.CanBeSkipped(element) ? HtmlMarkupFormatter.Instance.CloseTag(element, selfClosing) : string.Empty;
  }

  string IMarkupFormatter.Attribute(IAttr attribute) => this.Serialize(attribute);

  private bool CanBeRemoved(IElement element)
  {
    return !this.ShouldKeepStandardElements && element.Attributes.Length == 0 && element.LocalName.IsOneOf(TagNames.Html, TagNames.Head, TagNames.Body);
  }

  private bool CanBeSkipped(IElement element)
  {
    if (this.ShouldKeepImpliedEndTag || !element.Flags.HasFlag((Enum) NodeFlags.ImpliedEnd))
      return false;
    return element.NextElementSibling == null || element.NextElementSibling.LocalName == element.LocalName;
  }

  private static bool ShouldOutput(ICharacterData text)
  {
    if (!(text.Parent is HtmlBodyElement))
      return true;
    return text.NextSibling != null && text.PreviousSibling != null;
  }

  private static bool ShouldKeep(IElement element, IAttr attribute)
  {
    return !MinifyMarkupFormatter.IsStandardScript(element, attribute) && !MinifyMarkupFormatter.IsStandardStyle(element, attribute);
  }

  private static bool IsStandardScript(IElement element, IAttr attr)
  {
    return element is HtmlScriptElement && attr.Name.Is(AttributeNames.Type) && attr.Value.Is(MimeTypeNames.DefaultJavaScript);
  }

  private static bool IsStandardStyle(IElement element, IAttr attr)
  {
    return element is HtmlStyleElement && attr.Name.Is(AttributeNames.Type) && attr.Value.Is(MimeTypeNames.Css);
  }

  private string Serialize(IAttr attribute)
  {
    if (!this.ShouldKeepEmptyAttributes && string.IsNullOrEmpty(attribute.Value))
      return string.Empty;
    string source = HtmlMarkupFormatter.Instance.Attribute(attribute);
    return this.ShouldKeepAttributeQuotes || source.Any<char>(new Func<char, bool>(CharExtensions.IsWhiteSpaceCharacter)) ? source : source.Replace("\"", "");
  }
}
