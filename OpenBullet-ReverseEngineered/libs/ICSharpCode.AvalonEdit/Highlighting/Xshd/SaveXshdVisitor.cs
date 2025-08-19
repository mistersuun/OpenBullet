// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.SaveXshdVisitor
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

public sealed class SaveXshdVisitor : IXshdVisitor
{
  public const string Namespace = "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008";
  private XmlWriter writer;

  public SaveXshdVisitor(XmlWriter writer)
  {
    this.writer = writer != null ? writer : throw new ArgumentNullException(nameof (writer));
  }

  public void WriteDefinition(XshdSyntaxDefinition definition)
  {
    if (definition == null)
      throw new ArgumentNullException(nameof (definition));
    this.writer.WriteStartElement("SyntaxDefinition", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    if (definition.Name != null)
      this.writer.WriteAttributeString("name", definition.Name);
    if (definition.Extensions != null)
      this.writer.WriteAttributeString("extensions", string.Join(";", definition.Extensions.ToArray<string>()));
    definition.AcceptElements((IXshdVisitor) this);
    this.writer.WriteEndElement();
  }

  object IXshdVisitor.VisitRuleSet(XshdRuleSet ruleSet)
  {
    this.writer.WriteStartElement("RuleSet", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    if (ruleSet.Name != null)
      this.writer.WriteAttributeString("name", ruleSet.Name);
    this.WriteBoolAttribute("ignoreCase", ruleSet.IgnoreCase);
    ruleSet.AcceptElements((IXshdVisitor) this);
    this.writer.WriteEndElement();
    return (object) null;
  }

  private void WriteBoolAttribute(string attributeName, bool? value)
  {
    if (!value.HasValue)
      return;
    this.writer.WriteAttributeString(attributeName, value.Value ? "true" : "false");
  }

  private void WriteRuleSetReference(XshdReference<XshdRuleSet> ruleSetReference)
  {
    if (ruleSetReference.ReferencedElement == null)
      return;
    if (ruleSetReference.ReferencedDefinition != null)
      this.writer.WriteAttributeString("ruleSet", $"{ruleSetReference.ReferencedDefinition}/{ruleSetReference.ReferencedElement}");
    else
      this.writer.WriteAttributeString("ruleSet", ruleSetReference.ReferencedElement);
  }

  private void WriteColorReference(XshdReference<XshdColor> color)
  {
    if (color.InlineElement != null)
    {
      this.WriteColorAttributes(color.InlineElement);
    }
    else
    {
      if (color.ReferencedElement == null)
        return;
      if (color.ReferencedDefinition != null)
        this.writer.WriteAttributeString(nameof (color), $"{color.ReferencedDefinition}/{color.ReferencedElement}");
      else
        this.writer.WriteAttributeString(nameof (color), color.ReferencedElement);
    }
  }

  private void WriteColorAttributes(XshdColor color)
  {
    if (color.Foreground != null)
      this.writer.WriteAttributeString("foreground", color.Foreground.ToString());
    if (color.Background != null)
      this.writer.WriteAttributeString("background", color.Background.ToString());
    if (color.FontWeight.HasValue)
      this.writer.WriteAttributeString("fontWeight", V2Loader.FontWeightConverter.ConvertToInvariantString((object) color.FontWeight.Value).ToLowerInvariant());
    if (!color.FontStyle.HasValue)
      return;
    this.writer.WriteAttributeString("fontStyle", V2Loader.FontStyleConverter.ConvertToInvariantString((object) color.FontStyle.Value).ToLowerInvariant());
  }

  object IXshdVisitor.VisitColor(XshdColor color)
  {
    this.writer.WriteStartElement("Color", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    if (color.Name != null)
      this.writer.WriteAttributeString("name", color.Name);
    this.WriteColorAttributes(color);
    if (color.ExampleText != null)
      this.writer.WriteAttributeString("exampleText", color.ExampleText);
    this.writer.WriteEndElement();
    return (object) null;
  }

  object IXshdVisitor.VisitKeywords(XshdKeywords keywords)
  {
    this.writer.WriteStartElement("Keywords", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    this.WriteColorReference(keywords.ColorReference);
    foreach (string word in (IEnumerable<string>) keywords.Words)
      this.writer.WriteElementString("Word", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008", word);
    this.writer.WriteEndElement();
    return (object) null;
  }

  object IXshdVisitor.VisitSpan(XshdSpan span)
  {
    this.writer.WriteStartElement("Span", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    this.WriteColorReference(span.SpanColorReference);
    if (span.BeginRegexType == XshdRegexType.Default && span.BeginRegex != null)
      this.writer.WriteAttributeString("begin", span.BeginRegex);
    if (span.EndRegexType == XshdRegexType.Default && span.EndRegex != null)
      this.writer.WriteAttributeString("end", span.EndRegex);
    this.WriteRuleSetReference(span.RuleSetReference);
    if (span.Multiline)
      this.writer.WriteAttributeString("multiline", "true");
    if (span.BeginRegexType == XshdRegexType.IgnorePatternWhitespace)
      this.WriteBeginEndElement("Begin", span.BeginRegex, span.BeginColorReference);
    if (span.EndRegexType == XshdRegexType.IgnorePatternWhitespace)
      this.WriteBeginEndElement("End", span.EndRegex, span.EndColorReference);
    if (span.RuleSetReference.InlineElement != null)
      span.RuleSetReference.InlineElement.AcceptVisitor((IXshdVisitor) this);
    this.writer.WriteEndElement();
    return (object) null;
  }

  private void WriteBeginEndElement(
    string elementName,
    string regex,
    XshdReference<XshdColor> colorReference)
  {
    if (regex == null)
      return;
    this.writer.WriteStartElement(elementName, "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    this.WriteColorReference(colorReference);
    this.writer.WriteString(regex);
    this.writer.WriteEndElement();
  }

  object IXshdVisitor.VisitImport(XshdImport import)
  {
    this.writer.WriteStartElement("Import", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    this.WriteRuleSetReference(import.RuleSetReference);
    this.writer.WriteEndElement();
    return (object) null;
  }

  object IXshdVisitor.VisitRule(XshdRule rule)
  {
    this.writer.WriteStartElement("Rule", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
    this.WriteColorReference(rule.ColorReference);
    this.writer.WriteString(rule.Regex);
    this.writer.WriteEndElement();
    return (object) null;
  }
}
