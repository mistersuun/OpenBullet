// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlDomBuilderExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Parser.Tokens;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace AngleSharp.Html.Parser;

internal static class HtmlDomBuilderExtensions
{
  public static void SetAttributes(this Element element, List<HtmlAttributeToken> attributes)
  {
    NamedNodeMap attributes1 = element.Attributes;
    for (int index = 0; index < attributes.Count; ++index)
    {
      HtmlAttributeToken attribute = attributes[index];
      Attr attr = new Attr(attribute.Name, attribute.Value);
      attributes1.FastAddItem(attr);
    }
  }

  public static HtmlTreeMode? SelectMode(
    this Element element,
    bool isLast,
    Stack<HtmlTreeMode> templateModes)
  {
    if (element.Flags.HasFlag((Enum) NodeFlags.HtmlMember))
    {
      string localName = element.LocalName;
      if (localName.Is(TagNames.Select))
        return new HtmlTreeMode?(HtmlTreeMode.InSelect);
      if (TagNames.AllTableCells.Contains(localName))
        return new HtmlTreeMode?(isLast ? HtmlTreeMode.InBody : HtmlTreeMode.InCell);
      if (localName.Is(TagNames.Tr))
        return new HtmlTreeMode?(HtmlTreeMode.InRow);
      if (TagNames.AllTableSections.Contains(localName))
        return new HtmlTreeMode?(HtmlTreeMode.InTableBody);
      if (localName.Is(TagNames.Body))
        return new HtmlTreeMode?(HtmlTreeMode.InBody);
      if (localName.Is(TagNames.Table))
        return new HtmlTreeMode?(HtmlTreeMode.InTable);
      if (localName.Is(TagNames.Caption))
        return new HtmlTreeMode?(HtmlTreeMode.InCaption);
      if (localName.Is(TagNames.Colgroup))
        return new HtmlTreeMode?(HtmlTreeMode.InColumnGroup);
      if (localName.Is(TagNames.Template) && templateModes.Count > 0)
        return new HtmlTreeMode?(templateModes.Peek());
      if (localName.Is(TagNames.Html))
        return new HtmlTreeMode?(HtmlTreeMode.BeforeHead);
      if (localName.Is(TagNames.Head))
        return new HtmlTreeMode?(isLast ? HtmlTreeMode.InBody : HtmlTreeMode.InHead);
      if (localName.Is(TagNames.Frameset))
        return new HtmlTreeMode?(HtmlTreeMode.InFrameset);
    }
    return isLast ? new HtmlTreeMode?(HtmlTreeMode.InBody) : new HtmlTreeMode?();
  }

  public static int GetCode(this HtmlParseError code) => (int) code;

  public static void SetUniqueAttributes(this Element element, List<HtmlAttributeToken> attributes)
  {
    for (int index = attributes.Count - 1; index >= 0; --index)
    {
      if (element.HasAttribute(attributes[index].Name))
        attributes.RemoveAt(index);
    }
    element.SetAttributes(attributes);
  }

  public static void AddFormatting(this List<Element> formatting, Element element)
  {
    int num = 0;
    for (int index = formatting.Count - 1; index >= 0; --index)
    {
      Element element1 = formatting[index];
      if (element1 != null)
      {
        if (element1.NodeName.Is(element.NodeName) && element1.NamespaceUri.Is(element.NamespaceUri) && element1.Attributes.SameAs((INamedNodeMap) element.Attributes) && ++num == 3)
        {
          formatting.RemoveAt(index);
          break;
        }
      }
      else
        break;
    }
    formatting.Add(element);
  }

  public static void ClearFormatting(this List<Element> formatting)
  {
    while (formatting.Count != 0)
    {
      int index = formatting.Count - 1;
      Element element = formatting[index];
      formatting.RemoveAt(index);
      if (element == null)
        break;
    }
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static void AddScopeMarker(this List<Element> formatting)
  {
    formatting.Add((Element) null);
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static void AddComment(this Element parent, HtmlToken token)
  {
    parent.AddNode(token.IsProcessingInstruction ? (AngleSharp.Dom.Node) ProcessingInstruction.Create(parent.Owner, token.Data) : (AngleSharp.Dom.Node) new Comment(parent.Owner, token.Data));
  }

  [MethodImpl((MethodImplOptions) 256 /*0x0100*/)]
  public static void AddComment(this Document parent, HtmlToken token)
  {
    parent.AddNode(token.IsProcessingInstruction ? (AngleSharp.Dom.Node) ProcessingInstruction.Create(parent, token.Data) : (AngleSharp.Dom.Node) new Comment(parent, token.Data));
  }

  public static QuirksMode GetQuirksMode(this HtmlDoctypeToken doctype)
  {
    if (doctype.IsFullQuirks)
      return QuirksMode.On;
    return doctype.IsLimitedQuirks ? QuirksMode.Limited : QuirksMode.Off;
  }
}
