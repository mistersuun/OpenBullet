// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.V2Loader
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

internal static class V2Loader
{
  public const string Namespace = "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008";
  private static XmlSchemaSet schemaSet;
  internal static readonly ColorConverter ColorConverter = new ColorConverter();
  internal static readonly FontWeightConverter FontWeightConverter = new FontWeightConverter();
  internal static readonly FontStyleConverter FontStyleConverter = new FontStyleConverter();

  private static XmlSchemaSet SchemaSet
  {
    get
    {
      if (V2Loader.schemaSet == null)
        V2Loader.schemaSet = HighlightingLoader.LoadSchemaSet((XmlReader) new XmlTextReader(Resources.OpenStream("ModeV2.xsd")));
      return V2Loader.schemaSet;
    }
  }

  public static XshdSyntaxDefinition LoadDefinition(XmlReader reader, bool skipValidation)
  {
    reader = HighlightingLoader.GetValidatingReader(reader, true, skipValidation ? (XmlSchemaSet) null : V2Loader.SchemaSet);
    reader.Read();
    return V2Loader.ParseDefinition(reader);
  }

  private static XshdSyntaxDefinition ParseDefinition(XmlReader reader)
  {
    XshdSyntaxDefinition definition = new XshdSyntaxDefinition();
    definition.Name = reader.GetAttribute("name");
    string attribute = reader.GetAttribute("extensions");
    if (attribute != null)
      definition.Extensions.AddRange<string>((IEnumerable<string>) attribute.Split(';'));
    V2Loader.ParseElements((ICollection<XshdElement>) definition.Elements, reader);
    return definition;
  }

  private static void ParseElements(ICollection<XshdElement> c, XmlReader reader)
  {
    if (reader.IsEmptyElement)
      return;
    while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
    {
      if (reader.NamespaceURI != "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008")
      {
        if (!reader.IsEmptyElement)
          reader.Skip();
      }
      else
      {
        switch (reader.Name)
        {
          case "RuleSet":
            c.Add((XshdElement) V2Loader.ParseRuleSet(reader));
            continue;
          case "Property":
            c.Add(V2Loader.ParseProperty(reader));
            continue;
          case "Color":
            c.Add((XshdElement) V2Loader.ParseNamedColor(reader));
            continue;
          case "Keywords":
            c.Add((XshdElement) V2Loader.ParseKeywords(reader));
            continue;
          case "Span":
            c.Add((XshdElement) V2Loader.ParseSpan(reader));
            continue;
          case "Import":
            c.Add((XshdElement) V2Loader.ParseImport(reader));
            continue;
          case "Rule":
            c.Add((XshdElement) V2Loader.ParseRule(reader));
            continue;
          default:
            throw new NotSupportedException("Unknown element " + reader.Name);
        }
      }
    }
  }

  private static XshdElement ParseProperty(XmlReader reader)
  {
    XshdProperty element = new XshdProperty();
    V2Loader.SetPosition((XshdElement) element, reader);
    element.Name = reader.GetAttribute("name");
    element.Value = reader.GetAttribute("value");
    return (XshdElement) element;
  }

  private static XshdRuleSet ParseRuleSet(XmlReader reader)
  {
    XshdRuleSet element = new XshdRuleSet();
    V2Loader.SetPosition((XshdElement) element, reader);
    element.Name = reader.GetAttribute("name");
    element.IgnoreCase = reader.GetBoolAttribute("ignoreCase");
    V2Loader.CheckElementName(reader, element.Name);
    V2Loader.ParseElements((ICollection<XshdElement>) element.Elements, reader);
    return element;
  }

  private static XshdRule ParseRule(XmlReader reader)
  {
    XshdRule element = new XshdRule();
    V2Loader.SetPosition((XshdElement) element, reader);
    element.ColorReference = V2Loader.ParseColorReference(reader);
    if (!reader.IsEmptyElement)
    {
      reader.Read();
      if (reader.NodeType == XmlNodeType.Text)
      {
        element.Regex = reader.ReadContentAsString();
        element.RegexType = XshdRegexType.IgnorePatternWhitespace;
      }
    }
    return element;
  }

  private static XshdKeywords ParseKeywords(XmlReader reader)
  {
    XshdKeywords element = new XshdKeywords();
    V2Loader.SetPosition((XshdElement) element, reader);
    element.ColorReference = V2Loader.ParseColorReference(reader);
    reader.Read();
    while (reader.NodeType != XmlNodeType.EndElement)
      element.Words.Add(reader.ReadElementString());
    return element;
  }

  private static XshdImport ParseImport(XmlReader reader)
  {
    XshdImport element = new XshdImport();
    V2Loader.SetPosition((XshdElement) element, reader);
    element.RuleSetReference = V2Loader.ParseRuleSetReference(reader);
    if (!reader.IsEmptyElement)
      reader.Skip();
    return element;
  }

  private static XshdSpan ParseSpan(XmlReader reader)
  {
    XshdSpan element = new XshdSpan();
    V2Loader.SetPosition((XshdElement) element, reader);
    element.BeginRegex = reader.GetAttribute("begin");
    element.EndRegex = reader.GetAttribute("end");
    element.Multiline = ((int) reader.GetBoolAttribute("multiline") ?? 0) != 0;
    element.SpanColorReference = V2Loader.ParseColorReference(reader);
    element.RuleSetReference = V2Loader.ParseRuleSetReference(reader);
    if (!reader.IsEmptyElement)
    {
      reader.Read();
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        switch (reader.Name)
        {
          case "Begin":
            if (element.BeginRegex != null)
              throw V2Loader.Error(reader, "Duplicate Begin regex");
            element.BeginColorReference = V2Loader.ParseColorReference(reader);
            element.BeginRegex = reader.ReadElementString();
            element.BeginRegexType = XshdRegexType.IgnorePatternWhitespace;
            continue;
          case "End":
            if (element.EndRegex != null)
              throw V2Loader.Error(reader, "Duplicate End regex");
            element.EndColorReference = V2Loader.ParseColorReference(reader);
            element.EndRegex = reader.ReadElementString();
            element.EndRegexType = XshdRegexType.IgnorePatternWhitespace;
            continue;
          case "RuleSet":
            if (element.RuleSetReference.ReferencedElement != null)
              throw V2Loader.Error(reader, "Cannot specify both inline RuleSet and RuleSet reference");
            element.RuleSetReference = new XshdReference<XshdRuleSet>(V2Loader.ParseRuleSet(reader));
            reader.Read();
            continue;
          default:
            throw new NotSupportedException("Unknown element " + reader.Name);
        }
      }
    }
    return element;
  }

  private static Exception Error(XmlReader reader, string message)
  {
    return V2Loader.Error(reader as IXmlLineInfo, message);
  }

  private static Exception Error(IXmlLineInfo lineInfo, string message)
  {
    return lineInfo != null ? (Exception) new HighlightingDefinitionInvalidException(HighlightingLoader.FormatExceptionMessage(message, lineInfo.LineNumber, lineInfo.LinePosition)) : (Exception) new HighlightingDefinitionInvalidException(message);
  }

  private static void SetPosition(XshdElement element, XmlReader reader)
  {
    if (!(reader is IXmlLineInfo xmlLineInfo))
      return;
    element.LineNumber = xmlLineInfo.LineNumber;
    element.ColumnNumber = xmlLineInfo.LinePosition;
  }

  private static XshdReference<XshdRuleSet> ParseRuleSetReference(XmlReader reader)
  {
    string attribute = reader.GetAttribute("ruleSet");
    if (attribute == null)
      return new XshdReference<XshdRuleSet>();
    int length = attribute.LastIndexOf('/');
    return length >= 0 ? new XshdReference<XshdRuleSet>(attribute.Substring(0, length), attribute.Substring(length + 1)) : new XshdReference<XshdRuleSet>((string) null, attribute);
  }

  private static void CheckElementName(XmlReader reader, string name)
  {
    switch (name)
    {
      case null:
        break;
      case "":
        throw V2Loader.Error(reader, "The empty string is not a valid name.");
      default:
        if (name.IndexOf('/') < 0)
          break;
        throw V2Loader.Error(reader, "Element names must not contain a slash.");
    }
  }

  private static XshdColor ParseNamedColor(XmlReader reader)
  {
    XshdColor colorAttributes = V2Loader.ParseColorAttributes(reader);
    colorAttributes.Name = reader.GetAttribute("name");
    V2Loader.CheckElementName(reader, colorAttributes.Name);
    colorAttributes.ExampleText = reader.GetAttribute("exampleText");
    return colorAttributes;
  }

  private static XshdReference<XshdColor> ParseColorReference(XmlReader reader)
  {
    string attribute = reader.GetAttribute("color");
    if (attribute == null)
      return new XshdReference<XshdColor>(V2Loader.ParseColorAttributes(reader));
    int length = attribute.LastIndexOf('/');
    return length >= 0 ? new XshdReference<XshdColor>(attribute.Substring(0, length), attribute.Substring(length + 1)) : new XshdReference<XshdColor>((string) null, attribute);
  }

  private static XshdColor ParseColorAttributes(XmlReader reader)
  {
    XshdColor element = new XshdColor();
    V2Loader.SetPosition((XshdElement) element, reader);
    IXmlLineInfo lineInfo = reader as IXmlLineInfo;
    element.Foreground = V2Loader.ParseColor(lineInfo, reader.GetAttribute("foreground"));
    element.Background = V2Loader.ParseColor(lineInfo, reader.GetAttribute("background"));
    element.FontWeight = V2Loader.ParseFontWeight(reader.GetAttribute("fontWeight"));
    element.FontStyle = V2Loader.ParseFontStyle(reader.GetAttribute("fontStyle"));
    element.Underline = reader.GetBoolAttribute("underline");
    return element;
  }

  private static HighlightingBrush ParseColor(IXmlLineInfo lineInfo, string color)
  {
    if (string.IsNullOrEmpty(color))
      return (HighlightingBrush) null;
    return color.StartsWith("SystemColors.", StringComparison.Ordinal) ? (HighlightingBrush) V2Loader.GetSystemColorBrush(lineInfo, color) : V2Loader.FixedColorHighlightingBrush((Color?) V2Loader.ColorConverter.ConvertFromInvariantString(color));
  }

  internal static SystemColorHighlightingBrush GetSystemColorBrush(
    IXmlLineInfo lineInfo,
    string name)
  {
    PropertyInfo property = typeof (SystemColors).GetProperty(name.Substring(13) + "Brush");
    return !(property == (PropertyInfo) null) ? new SystemColorHighlightingBrush(property) : throw V2Loader.Error(lineInfo, $"Cannot find '{name}'.");
  }

  private static HighlightingBrush FixedColorHighlightingBrush(Color? color)
  {
    return !color.HasValue ? (HighlightingBrush) null : (HighlightingBrush) new SimpleHighlightingBrush(color.Value);
  }

  private static FontWeight? ParseFontWeight(string fontWeight)
  {
    return string.IsNullOrEmpty(fontWeight) ? new FontWeight?() : (FontWeight?) V2Loader.FontWeightConverter.ConvertFromInvariantString(fontWeight);
  }

  private static FontStyle? ParseFontStyle(string fontStyle)
  {
    return string.IsNullOrEmpty(fontStyle) ? new FontStyle?() : (FontStyle?) V2Loader.FontStyleConverter.ConvertFromInvariantString(fontStyle);
  }
}
