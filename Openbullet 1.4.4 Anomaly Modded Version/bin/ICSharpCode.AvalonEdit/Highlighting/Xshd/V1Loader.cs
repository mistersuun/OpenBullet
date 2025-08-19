// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.V1Loader
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

internal sealed class V1Loader
{
  private static XmlSchemaSet schemaSet;
  private char ruleSetEscapeCharacter;

  private static XmlSchemaSet SchemaSet
  {
    get
    {
      if (V1Loader.schemaSet == null)
        V1Loader.schemaSet = HighlightingLoader.LoadSchemaSet((XmlReader) new XmlTextReader(Resources.OpenStream("ModeV1.xsd")));
      return V1Loader.schemaSet;
    }
  }

  public static XshdSyntaxDefinition LoadDefinition(XmlReader reader, bool skipValidation)
  {
    reader = HighlightingLoader.GetValidatingReader(reader, false, skipValidation ? (XmlSchemaSet) null : V1Loader.SchemaSet);
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load(reader);
    return new V1Loader().ParseDefinition(xmlDocument.DocumentElement);
  }

  private XshdSyntaxDefinition ParseDefinition(XmlElement syntaxDefinition)
  {
    XshdSyntaxDefinition definition = new XshdSyntaxDefinition();
    definition.Name = syntaxDefinition.GetAttributeOrNull("name");
    if (syntaxDefinition.HasAttribute("extensions"))
      definition.Extensions.AddRange<string>((IEnumerable<string>) syntaxDefinition.GetAttribute("extensions").Split(';', '|'));
    XshdRuleSet xshdRuleSet1 = (XshdRuleSet) null;
    foreach (XmlElement element in syntaxDefinition.GetElementsByTagName("RuleSet"))
    {
      XshdRuleSet xshdRuleSet2 = this.ImportRuleSet(element);
      definition.Elements.Add((XshdElement) xshdRuleSet2);
      if (xshdRuleSet2.Name == null)
        xshdRuleSet1 = xshdRuleSet2;
      if (syntaxDefinition["Digits"] != null)
        xshdRuleSet2.Elements.Add((XshdElement) new XshdRule()
        {
          ColorReference = V1Loader.GetColorReference(syntaxDefinition["Digits"]),
          RegexType = XshdRegexType.IgnorePatternWhitespace,
          Regex = "\\b0[xX][0-9a-fA-F]+|(\\b\\d+(\\.[0-9]+)?|\\.[0-9]+)([eE][+-]?[0-9]+)?"
        });
    }
    if (syntaxDefinition.HasAttribute("extends") && xshdRuleSet1 != null)
      xshdRuleSet1.Elements.Add((XshdElement) new XshdImport()
      {
        RuleSetReference = new XshdReference<XshdRuleSet>(syntaxDefinition.GetAttribute("extends"), string.Empty)
      });
    return definition;
  }

  private static XshdColor GetColorFromElement(XmlElement element)
  {
    if (!element.HasAttribute("bold") && !element.HasAttribute("italic") && !element.HasAttribute("color") && !element.HasAttribute("bgcolor"))
      return (XshdColor) null;
    XshdColor colorFromElement = new XshdColor();
    if (element.HasAttribute("bold"))
      colorFromElement.FontWeight = new FontWeight?(XmlConvert.ToBoolean(element.GetAttribute("bold")) ? FontWeights.Bold : FontWeights.Normal);
    if (element.HasAttribute("italic"))
      colorFromElement.FontStyle = new FontStyle?(XmlConvert.ToBoolean(element.GetAttribute("italic")) ? FontStyles.Italic : FontStyles.Normal);
    if (element.HasAttribute("color"))
      colorFromElement.Foreground = V1Loader.ParseColor(element.GetAttribute("color"));
    if (element.HasAttribute("bgcolor"))
      colorFromElement.Background = V1Loader.ParseColor(element.GetAttribute("bgcolor"));
    return colorFromElement;
  }

  private static XshdReference<XshdColor> GetColorReference(XmlElement element)
  {
    XshdColor colorFromElement = V1Loader.GetColorFromElement(element);
    return colorFromElement != null ? new XshdReference<XshdColor>(colorFromElement) : new XshdReference<XshdColor>();
  }

  private static HighlightingBrush ParseColor(string c)
  {
    if (c.StartsWith("#", StringComparison.Ordinal))
    {
      int maxValue = (int) byte.MaxValue;
      int num = 0;
      if (c.Length > 7)
      {
        num = 2;
        maxValue = int.Parse(c.Substring(1, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      int r = int.Parse(c.Substring(1 + num, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      int g = int.Parse(c.Substring(3 + num, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      int b = int.Parse(c.Substring(5 + num, 2), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
      return (HighlightingBrush) new SimpleHighlightingBrush(Color.FromArgb((byte) maxValue, (byte) r, (byte) g, (byte) b));
    }
    return c.StartsWith("SystemColors.", StringComparison.Ordinal) ? (HighlightingBrush) V2Loader.GetSystemColorBrush((IXmlLineInfo) null, c) : (HighlightingBrush) new SimpleHighlightingBrush((Color) V2Loader.ColorConverter.ConvertFromInvariantString(c));
  }

  private XshdRuleSet ImportRuleSet(XmlElement element)
  {
    XshdRuleSet xshdRuleSet = new XshdRuleSet();
    xshdRuleSet.Name = element.GetAttributeOrNull("name");
    this.ruleSetEscapeCharacter = !element.HasAttribute("escapecharacter") ? char.MinValue : element.GetAttribute("escapecharacter")[0];
    if (element.HasAttribute("reference"))
      xshdRuleSet.Elements.Add((XshdElement) new XshdImport()
      {
        RuleSetReference = new XshdReference<XshdRuleSet>(element.GetAttribute("reference"), string.Empty)
      });
    xshdRuleSet.IgnoreCase = element.GetBoolAttribute("ignorecase");
    foreach (XmlElement element1 in element.GetElementsByTagName("KeyWords"))
    {
      XshdKeywords xshdKeywords = new XshdKeywords();
      xshdKeywords.ColorReference = V1Loader.GetColorReference(element1);
      foreach (XmlElement xmlElement in element1.GetElementsByTagName("Key"))
      {
        string attribute = xmlElement.GetAttribute("word");
        if (!string.IsNullOrEmpty(attribute))
          xshdKeywords.Words.Add(attribute);
      }
      if (xshdKeywords.Words.Count > 0)
        xshdRuleSet.Elements.Add((XshdElement) xshdKeywords);
    }
    foreach (XmlElement element2 in element.GetElementsByTagName("Span"))
      xshdRuleSet.Elements.Add((XshdElement) this.ImportSpan(element2));
    foreach (XmlElement el in element.GetElementsByTagName("MarkPrevious"))
      xshdRuleSet.Elements.Add((XshdElement) V1Loader.ImportMarkPrevNext(el, false));
    foreach (XmlElement el in element.GetElementsByTagName("MarkFollowing"))
      xshdRuleSet.Elements.Add((XshdElement) V1Loader.ImportMarkPrevNext(el, true));
    return xshdRuleSet;
  }

  private static XshdRule ImportMarkPrevNext(XmlElement el, bool markFollowing)
  {
    bool flag = ((int) el.GetBoolAttribute("markmarker") ?? 0) != 0;
    string str1 = Regex.Escape(el.InnerText);
    string str2 = !markFollowing ? (!flag ? $"[\\d\\w_]+(?=(\\s*{str1}))" : "[\\d\\w_]+\\s*" + str1) : (!flag ? $"(?<=({str1}\\s*))[\\d\\w_]+" : str1 + "\\s*[\\d\\w_]+");
    return new XshdRule()
    {
      ColorReference = V1Loader.GetColorReference(el),
      Regex = str2,
      RegexType = XshdRegexType.IgnorePatternWhitespace
    };
  }

  private XshdSpan ImportSpan(XmlElement element)
  {
    XshdSpan xshdSpan = new XshdSpan();
    if (element.HasAttribute("rule"))
      xshdSpan.RuleSetReference = new XshdReference<XshdRuleSet>((string) null, element.GetAttribute("rule"));
    char setEscapeCharacter = this.ruleSetEscapeCharacter;
    if (element.HasAttribute("escapecharacter"))
      setEscapeCharacter = element.GetAttribute("escapecharacter")[0];
    xshdSpan.Multiline = ((int) element.GetBoolAttribute("stopateol") ?? 0) == 0;
    xshdSpan.SpanColorReference = V1Loader.GetColorReference(element);
    xshdSpan.BeginRegexType = XshdRegexType.IgnorePatternWhitespace;
    xshdSpan.BeginRegex = V1Loader.ImportRegex(element["Begin"].InnerText, ((int) element["Begin"].GetBoolAttribute("singleword") ?? 0) != 0, element["Begin"].GetBoolAttribute("startofline"));
    xshdSpan.BeginColorReference = V1Loader.GetColorReference(element["Begin"]);
    string expr = string.Empty;
    if (element["End"] != null)
    {
      xshdSpan.EndRegexType = XshdRegexType.IgnorePatternWhitespace;
      expr = element["End"].InnerText;
      xshdSpan.EndRegex = V1Loader.ImportRegex(expr, ((int) element["End"].GetBoolAttribute("singleword") ?? 0) != 0, new bool?());
      xshdSpan.EndColorReference = V1Loader.GetColorReference(element["End"]);
    }
    if (setEscapeCharacter != char.MinValue)
    {
      XshdRuleSet inlineElement = new XshdRuleSet();
      if (expr.Length == 1 && (int) expr[0] == (int) setEscapeCharacter)
        inlineElement.Elements.Add((XshdElement) new XshdSpan()
        {
          BeginRegex = Regex.Escape(expr + expr),
          EndRegex = ""
        });
      else
        inlineElement.Elements.Add((XshdElement) new XshdSpan()
        {
          BeginRegex = Regex.Escape(setEscapeCharacter.ToString()),
          EndRegex = "."
        });
      if (xshdSpan.RuleSetReference.ReferencedElement != null)
        inlineElement.Elements.Add((XshdElement) new XshdImport()
        {
          RuleSetReference = xshdSpan.RuleSetReference
        });
      xshdSpan.RuleSetReference = new XshdReference<XshdRuleSet>(inlineElement);
    }
    return xshdSpan;
  }

  private static string ImportRegex(string expr, bool singleWord, bool? startOfLine)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    if (startOfLine.HasValue)
    {
      if (startOfLine.Value)
        stringBuilder1.Append("(?<=(^\\s*))");
      else
        stringBuilder1.Append("(?<!(^\\s*))");
    }
    else if (singleWord)
      stringBuilder1.Append("\\b");
    for (int index = 0; index < expr.Length; ++index)
    {
      char ch = expr[index];
      if (ch == '@')
      {
        ++index;
        if (index == expr.Length)
          throw new HighlightingDefinitionInvalidException("Unexpected end of @ sequence, use @@ to look for a single @.");
        switch (expr[index])
        {
          case '!':
            StringBuilder stringBuilder2 = new StringBuilder();
            ++index;
            while (index < expr.Length && expr[index] != '@')
              stringBuilder2.Append(expr[index++]);
            stringBuilder1.Append("(?!(");
            stringBuilder1.Append(Regex.Escape(stringBuilder2.ToString()));
            stringBuilder1.Append("))");
            continue;
          case '-':
            StringBuilder stringBuilder3 = new StringBuilder();
            ++index;
            while (index < expr.Length && expr[index] != '@')
              stringBuilder3.Append(expr[index++]);
            stringBuilder1.Append("(?<!(");
            stringBuilder1.Append(Regex.Escape(stringBuilder3.ToString()));
            stringBuilder1.Append("))");
            continue;
          case '@':
            stringBuilder1.Append("@");
            continue;
          case 'C':
            stringBuilder1.Append("[^\\w\\d_]");
            continue;
          default:
            throw new HighlightingDefinitionInvalidException("Unknown character in @ sequence.");
        }
      }
      else
        stringBuilder1.Append(Regex.Escape(ch.ToString()));
    }
    if (singleWord)
      stringBuilder1.Append("\\b");
    return stringBuilder1.ToString();
  }
}
