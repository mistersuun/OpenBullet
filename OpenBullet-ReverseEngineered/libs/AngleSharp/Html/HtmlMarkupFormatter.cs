// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.HtmlMarkupFormatter
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Html;

public sealed class HtmlMarkupFormatter : IMarkupFormatter
{
  public static readonly IMarkupFormatter Instance = (IMarkupFormatter) new HtmlMarkupFormatter();

  string IMarkupFormatter.Comment(IComment comment) => $"<!--{comment.Data}-->";

  string IMarkupFormatter.Doctype(IDocumentType doctype)
  {
    string ids = HtmlMarkupFormatter.GetIds(doctype.PublicIdentifier, doctype.SystemIdentifier);
    return $"<!DOCTYPE {doctype.Name}{ids}>";
  }

  string IMarkupFormatter.Processing(IProcessingInstruction processing)
  {
    return $"<?{$"{processing.Target} {processing.Data}"}>";
  }

  string IMarkupFormatter.Text(ICharacterData text) => HtmlMarkupFormatter.EscapeText(text.Data);

  string IMarkupFormatter.OpenTag(IElement element, bool selfClosing)
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    sb.Append('<');
    if (!string.IsNullOrEmpty(element.Prefix))
      sb.Append(element.Prefix).Append(':');
    sb.Append(element.LocalName);
    foreach (IAttr attribute in (IEnumerable<IAttr>) element.Attributes)
      sb.Append(' ').Append(HtmlMarkupFormatter.Instance.Attribute(attribute));
    sb.Append('>');
    return sb.ToPool();
  }

  string IMarkupFormatter.CloseTag(IElement element, bool selfClosing)
  {
    string prefix = element.Prefix;
    string localName = element.LocalName;
    string str = !string.IsNullOrEmpty(prefix) ? $"{prefix}:{localName}" : localName;
    return !selfClosing ? $"</{str}>" : string.Empty;
  }

  string IMarkupFormatter.Attribute(IAttr attr)
  {
    string namespaceUri = attr.NamespaceUri;
    string localName = attr.LocalName;
    string str = attr.Value;
    StringBuilder stringBuilder = StringBuilderPool.Obtain();
    if (string.IsNullOrEmpty(namespaceUri))
      stringBuilder.Append(localName);
    else if (namespaceUri.Is(NamespaceNames.XmlUri))
      stringBuilder.Append(NamespaceNames.XmlPrefix).Append(':').Append(localName);
    else if (namespaceUri.Is(NamespaceNames.XLinkUri))
      stringBuilder.Append(NamespaceNames.XLinkPrefix).Append(':').Append(localName);
    else if (namespaceUri.Is(NamespaceNames.XmlNsUri))
      stringBuilder.Append(HtmlMarkupFormatter.XmlNamespaceLocalName(localName));
    else
      stringBuilder.Append(attr.Name);
    stringBuilder.Append('=').Append('"');
    for (int index = 0; index < str.Length; ++index)
    {
      switch (str[index])
      {
        case '"':
          stringBuilder.Append("&quot;");
          break;
        case '&':
          stringBuilder.Append("&amp;");
          break;
        case ' ':
          stringBuilder.Append("&nbsp;");
          break;
        default:
          stringBuilder.Append(str[index]);
          break;
      }
    }
    return stringBuilder.Append('"').ToPool();
  }

  public static string EscapeText(string content)
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    for (int index = 0; index < content.Length; ++index)
    {
      switch (content[index])
      {
        case '&':
          sb.Append("&amp;");
          break;
        case '<':
          sb.Append("&lt;");
          break;
        case '>':
          sb.Append("&gt;");
          break;
        case ' ':
          sb.Append("&nbsp;");
          break;
        default:
          sb.Append(content[index]);
          break;
      }
    }
    return sb.ToPool();
  }

  private static string GetIds(string publicId, string systemId)
  {
    if (string.IsNullOrEmpty(publicId) && string.IsNullOrEmpty(systemId))
      return string.Empty;
    if (string.IsNullOrEmpty(systemId))
      return $" PUBLIC \"{publicId}\"";
    if (string.IsNullOrEmpty(publicId))
      return $" SYSTEM \"{systemId}\"";
    return $" PUBLIC \"{publicId}\" \"{systemId}\"";
  }

  private static string XmlNamespaceLocalName(string name)
  {
    return !(name != NamespaceNames.XmlNsPrefix) ? name : $"{NamespaceNames.XmlNsPrefix}:{name}";
  }
}
