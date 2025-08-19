// Decompiled with JetBrains decompiler
// Type: AngleSharp.Xhtml.XhtmlMarkupFormatter
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Xhtml;

public sealed class XhtmlMarkupFormatter : IMarkupFormatter
{
  public static readonly IMarkupFormatter Instance = (IMarkupFormatter) new XhtmlMarkupFormatter();

  string IMarkupFormatter.CloseTag(IElement element, bool selfClosing)
  {
    string prefix = element.Prefix;
    string localName = element.LocalName;
    string str = !string.IsNullOrEmpty(prefix) ? $"{prefix}:{localName}" : localName;
    return !selfClosing && element.HasChildNodes ? $"</{str}>" : string.Empty;
  }

  string IMarkupFormatter.Comment(IComment comment) => $"<!--{comment.Data}-->";

  string IMarkupFormatter.Doctype(IDocumentType doctype)
  {
    string publicIdentifier = doctype.PublicIdentifier;
    string systemIdentifier = doctype.SystemIdentifier;
    string str1;
    if ((!string.IsNullOrEmpty(publicIdentifier) ? 0 : (string.IsNullOrEmpty(systemIdentifier) ? 1 : 0)) == 0)
    {
      string str2;
      if (!string.IsNullOrEmpty(publicIdentifier))
        str2 = $"PUBLIC \"{publicIdentifier}\" \"{systemIdentifier}\"";
      else
        str2 = $"SYSTEM \"{systemIdentifier}\"";
      str1 = " " + str2;
    }
    else
      str1 = string.Empty;
    string str3 = str1;
    return $"<!DOCTYPE {doctype.Name}{str3}>";
  }

  string IMarkupFormatter.OpenTag(IElement element, bool selfClosing)
  {
    string prefix = element.Prefix;
    StringBuilder sb = StringBuilderPool.Obtain();
    sb.Append('<');
    if (!string.IsNullOrEmpty(prefix))
      sb.Append(prefix).Append(':');
    sb.Append(element.LocalName);
    foreach (IAttr attribute in (IEnumerable<IAttr>) element.Attributes)
      sb.Append(" ").Append(XhtmlMarkupFormatter.Instance.Attribute(attribute));
    if (selfClosing || !element.HasChildNodes)
      sb.Append(" /");
    sb.Append('>');
    return sb.ToPool();
  }

  string IMarkupFormatter.Processing(IProcessingInstruction processing)
  {
    return $"<?{$"{processing.Target} {processing.Data}"}?>";
  }

  string IMarkupFormatter.Text(ICharacterData text) => XhtmlMarkupFormatter.EscapeText(text.Data);

  string IMarkupFormatter.Attribute(IAttr attribute)
  {
    string namespaceUri = attribute.NamespaceUri;
    string localName = attribute.LocalName;
    string str = attribute.Value;
    StringBuilder stringBuilder = StringBuilderPool.Obtain();
    if (string.IsNullOrEmpty(namespaceUri))
      stringBuilder.Append(localName);
    else if (namespaceUri.Is(NamespaceNames.XmlUri))
      stringBuilder.Append(NamespaceNames.XmlPrefix).Append(':').Append(localName);
    else if (namespaceUri.Is(NamespaceNames.XLinkUri))
      stringBuilder.Append(NamespaceNames.XLinkPrefix).Append(':').Append(localName);
    else if (namespaceUri.Is(NamespaceNames.XmlNsUri))
      stringBuilder.Append(XhtmlMarkupFormatter.XmlNamespaceLocalName(localName));
    else
      stringBuilder.Append(attribute.Name);
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
        case '<':
          stringBuilder.Append("&lt;");
          break;
        case ' ':
          stringBuilder.Append("&#160;");
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
          sb.Append("&#160;");
          break;
        default:
          sb.Append(content[index]);
          break;
      }
    }
    return sb.ToPool();
  }

  private static string XmlNamespaceLocalName(string name)
  {
    return !(name != NamespaceNames.XmlNsPrefix) ? name : NamespaceNames.XmlNsPrefix + ":";
  }
}
