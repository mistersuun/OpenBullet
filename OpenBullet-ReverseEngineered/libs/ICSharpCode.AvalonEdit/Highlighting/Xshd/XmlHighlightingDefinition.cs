// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XmlHighlightingDefinition
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
internal sealed class XmlHighlightingDefinition : IHighlightingDefinition
{
  private Dictionary<string, HighlightingRuleSet> ruleSetDict = new Dictionary<string, HighlightingRuleSet>();
  private Dictionary<string, HighlightingColor> colorDict = new Dictionary<string, HighlightingColor>();
  [OptionalField]
  private Dictionary<string, string> propDict = new Dictionary<string, string>();

  public string Name { get; private set; }

  public XmlHighlightingDefinition(
    XshdSyntaxDefinition xshd,
    IHighlightingDefinitionReferenceResolver resolver)
  {
    this.Name = xshd.Name;
    XmlHighlightingDefinition.RegisterNamedElementsVisitor visitor = new XmlHighlightingDefinition.RegisterNamedElementsVisitor(this);
    xshd.AcceptElements((IXshdVisitor) visitor);
    foreach (XshdElement element in (IEnumerable<XshdElement>) xshd.Elements)
    {
      if (element is XshdRuleSet key && key.Name == null)
      {
        if (this.MainRuleSet != null)
          throw XmlHighlightingDefinition.Error(element, "Duplicate main RuleSet. There must be only one nameless RuleSet!");
        this.MainRuleSet = visitor.ruleSets[key];
      }
    }
    if (this.MainRuleSet == null)
      throw new HighlightingDefinitionInvalidException("Could not find main RuleSet.");
    xshd.AcceptElements((IXshdVisitor) new XmlHighlightingDefinition.TranslateElementVisitor(this, visitor.ruleSets, resolver));
    foreach (XshdProperty xshdProperty in xshd.Elements.OfType<XshdProperty>())
      this.propDict.Add(xshdProperty.Name, xshdProperty.Value);
  }

  private static Exception Error(XshdElement element, string message)
  {
    if (element.LineNumber <= 0)
      return (Exception) new HighlightingDefinitionInvalidException(message);
    return (Exception) new HighlightingDefinitionInvalidException($"Error at line {(object) element.LineNumber}:\n{message}");
  }

  public HighlightingRuleSet MainRuleSet { get; private set; }

  public HighlightingRuleSet GetNamedRuleSet(string name)
  {
    if (string.IsNullOrEmpty(name))
      return this.MainRuleSet;
    HighlightingRuleSet highlightingRuleSet;
    return this.ruleSetDict.TryGetValue(name, out highlightingRuleSet) ? highlightingRuleSet : (HighlightingRuleSet) null;
  }

  public HighlightingColor GetNamedColor(string name)
  {
    HighlightingColor highlightingColor;
    return this.colorDict.TryGetValue(name, out highlightingColor) ? highlightingColor : (HighlightingColor) null;
  }

  public IEnumerable<HighlightingColor> NamedHighlightingColors
  {
    get => (IEnumerable<HighlightingColor>) this.colorDict.Values;
  }

  public override string ToString() => this.Name;

  public IDictionary<string, string> Properties => (IDictionary<string, string>) this.propDict;

  private sealed class RegisterNamedElementsVisitor : IXshdVisitor
  {
    private XmlHighlightingDefinition def;
    internal readonly Dictionary<XshdRuleSet, HighlightingRuleSet> ruleSets = new Dictionary<XshdRuleSet, HighlightingRuleSet>();

    public RegisterNamedElementsVisitor(XmlHighlightingDefinition def) => this.def = def;

    public object VisitRuleSet(XshdRuleSet ruleSet)
    {
      HighlightingRuleSet highlightingRuleSet = new HighlightingRuleSet();
      this.ruleSets.Add(ruleSet, highlightingRuleSet);
      if (ruleSet.Name != null)
      {
        if (ruleSet.Name.Length == 0)
          throw XmlHighlightingDefinition.Error((XshdElement) ruleSet, "Name must not be the empty string");
        if (this.def.ruleSetDict.ContainsKey(ruleSet.Name))
          throw XmlHighlightingDefinition.Error((XshdElement) ruleSet, $"Duplicate rule set name '{ruleSet.Name}'.");
        this.def.ruleSetDict.Add(ruleSet.Name, highlightingRuleSet);
      }
      ruleSet.AcceptElements((IXshdVisitor) this);
      return (object) null;
    }

    public object VisitColor(XshdColor color)
    {
      if (color.Name != null)
      {
        if (color.Name.Length == 0)
          throw XmlHighlightingDefinition.Error((XshdElement) color, "Name must not be the empty string");
        if (this.def.colorDict.ContainsKey(color.Name))
          throw XmlHighlightingDefinition.Error((XshdElement) color, $"Duplicate color name '{color.Name}'.");
        this.def.colorDict.Add(color.Name, new HighlightingColor());
      }
      return (object) null;
    }

    public object VisitKeywords(XshdKeywords keywords)
    {
      return keywords.ColorReference.AcceptVisitor((IXshdVisitor) this);
    }

    public object VisitSpan(XshdSpan span)
    {
      span.BeginColorReference.AcceptVisitor((IXshdVisitor) this);
      span.SpanColorReference.AcceptVisitor((IXshdVisitor) this);
      span.EndColorReference.AcceptVisitor((IXshdVisitor) this);
      return span.RuleSetReference.AcceptVisitor((IXshdVisitor) this);
    }

    public object VisitImport(XshdImport import)
    {
      return import.RuleSetReference.AcceptVisitor((IXshdVisitor) this);
    }

    public object VisitRule(XshdRule rule)
    {
      return rule.ColorReference.AcceptVisitor((IXshdVisitor) this);
    }
  }

  private sealed class TranslateElementVisitor : IXshdVisitor
  {
    private readonly XmlHighlightingDefinition def;
    private readonly Dictionary<XshdRuleSet, HighlightingRuleSet> ruleSetDict;
    private readonly Dictionary<HighlightingRuleSet, XshdRuleSet> reverseRuleSetDict;
    private readonly IHighlightingDefinitionReferenceResolver resolver;
    private HashSet<XshdRuleSet> processingStartedRuleSets = new HashSet<XshdRuleSet>();
    private HashSet<XshdRuleSet> processedRuleSets = new HashSet<XshdRuleSet>();
    private bool ignoreCase;

    public TranslateElementVisitor(
      XmlHighlightingDefinition def,
      Dictionary<XshdRuleSet, HighlightingRuleSet> ruleSetDict,
      IHighlightingDefinitionReferenceResolver resolver)
    {
      this.def = def;
      this.ruleSetDict = ruleSetDict;
      this.resolver = resolver;
      this.reverseRuleSetDict = new Dictionary<HighlightingRuleSet, XshdRuleSet>();
      foreach (KeyValuePair<XshdRuleSet, HighlightingRuleSet> keyValuePair in ruleSetDict)
        this.reverseRuleSetDict.Add(keyValuePair.Value, keyValuePair.Key);
    }

    public object VisitRuleSet(XshdRuleSet ruleSet)
    {
      HighlightingRuleSet target = this.ruleSetDict[ruleSet];
      if (this.processedRuleSets.Contains(ruleSet))
        return (object) target;
      if (!this.processingStartedRuleSets.Add(ruleSet))
        throw XmlHighlightingDefinition.Error((XshdElement) ruleSet, "RuleSet cannot be processed because it contains cyclic <Import>");
      bool ignoreCase = this.ignoreCase;
      if (ruleSet.IgnoreCase.HasValue)
        this.ignoreCase = ruleSet.IgnoreCase.Value;
      target.Name = ruleSet.Name;
      foreach (XshdElement element in (IEnumerable<XshdElement>) ruleSet.Elements)
      {
        switch (element.AcceptVisitor((IXshdVisitor) this))
        {
          case HighlightingRuleSet source:
            XmlHighlightingDefinition.TranslateElementVisitor.Merge(target, source);
            continue;
          case HighlightingSpan highlightingSpan:
            target.Spans.Add(highlightingSpan);
            continue;
          case HighlightingRule highlightingRule:
            target.Rules.Add(highlightingRule);
            continue;
          default:
            continue;
        }
      }
      this.ignoreCase = ignoreCase;
      this.processedRuleSets.Add(ruleSet);
      return (object) target;
    }

    private static void Merge(HighlightingRuleSet target, HighlightingRuleSet source)
    {
      target.Rules.AddRange<HighlightingRule>((IEnumerable<HighlightingRule>) source.Rules);
      target.Spans.AddRange<HighlightingSpan>((IEnumerable<HighlightingSpan>) source.Spans);
    }

    public object VisitColor(XshdColor color)
    {
      HighlightingColor highlightingColor;
      if (color.Name != null)
      {
        highlightingColor = this.def.colorDict[color.Name];
      }
      else
      {
        if (color.Foreground == null && !color.FontStyle.HasValue && !color.FontWeight.HasValue)
          return (object) null;
        highlightingColor = new HighlightingColor();
      }
      highlightingColor.Name = color.Name;
      highlightingColor.Foreground = color.Foreground;
      highlightingColor.Background = color.Background;
      highlightingColor.Underline = color.Underline;
      highlightingColor.FontStyle = color.FontStyle;
      highlightingColor.FontWeight = color.FontWeight;
      return (object) highlightingColor;
    }

    public object VisitKeywords(XshdKeywords keywords)
    {
      if (keywords.Words.Count == 0)
        return (object) XmlHighlightingDefinition.Error((XshdElement) keywords, "Keyword group must not be empty.");
      foreach (string word in (IEnumerable<string>) keywords.Words)
      {
        if (string.IsNullOrEmpty(word))
          throw XmlHighlightingDefinition.Error((XshdElement) keywords, "Cannot use empty string as keyword");
      }
      StringBuilder stringBuilder = new StringBuilder();
      if (keywords.Words.All<string>(new Func<string, bool>(XmlHighlightingDefinition.TranslateElementVisitor.IsSimpleWord)))
      {
        stringBuilder.Append("\\b(?>");
        int num = 0;
        foreach (string str in (IEnumerable<string>) keywords.Words.OrderByDescending<string, int>((Func<string, int>) (w => w.Length)))
        {
          if (num++ > 0)
            stringBuilder.Append('|');
          stringBuilder.Append(Regex.Escape(str));
        }
        stringBuilder.Append(")\\b");
      }
      else
      {
        stringBuilder.Append('(');
        int num = 0;
        foreach (string word in (IEnumerable<string>) keywords.Words)
        {
          if (num++ > 0)
            stringBuilder.Append('|');
          if (char.IsLetterOrDigit(word[0]))
            stringBuilder.Append("\\b");
          stringBuilder.Append(Regex.Escape(word));
          if (char.IsLetterOrDigit(word[word.Length - 1]))
            stringBuilder.Append("\\b");
        }
        stringBuilder.Append(')');
      }
      return (object) new HighlightingRule()
      {
        Color = this.GetColor((XshdElement) keywords, keywords.ColorReference),
        Regex = this.CreateRegex((XshdElement) keywords, stringBuilder.ToString(), XshdRegexType.Default)
      };
    }

    private static bool IsSimpleWord(string word)
    {
      return char.IsLetterOrDigit(word[0]) && char.IsLetterOrDigit(word, word.Length - 1);
    }

    private Regex CreateRegex(XshdElement position, string regex, XshdRegexType regexType)
    {
      if (regex == null)
        throw XmlHighlightingDefinition.Error(position, "Regex missing");
      RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;
      if (regexType == XshdRegexType.IgnorePatternWhitespace)
        options |= RegexOptions.IgnorePatternWhitespace;
      if (this.ignoreCase)
        options |= RegexOptions.IgnoreCase;
      try
      {
        return new Regex(regex, options);
      }
      catch (ArgumentException ex)
      {
        throw XmlHighlightingDefinition.Error(position, ex.Message);
      }
    }

    private HighlightingColor GetColor(
      XshdElement position,
      XshdReference<XshdColor> colorReference)
    {
      if (colorReference.InlineElement != null)
        return (HighlightingColor) colorReference.InlineElement.AcceptVisitor((IXshdVisitor) this);
      if (colorReference.ReferencedElement == null)
        return (HighlightingColor) null;
      return this.GetDefinition(position, colorReference.ReferencedDefinition).GetNamedColor(colorReference.ReferencedElement) ?? throw XmlHighlightingDefinition.Error(position, $"Could not find color named '{colorReference.ReferencedElement}'.");
    }

    private IHighlightingDefinition GetDefinition(XshdElement position, string definitionName)
    {
      if (definitionName == null)
        return (IHighlightingDefinition) this.def;
      return (this.resolver != null ? this.resolver.GetDefinition(definitionName) : throw XmlHighlightingDefinition.Error(position, "Resolving references to other syntax definitions is not possible because the IHighlightingDefinitionReferenceResolver is null.")) ?? throw XmlHighlightingDefinition.Error(position, $"Could not find definition with name '{definitionName}'.");
    }

    private HighlightingRuleSet GetRuleSet(
      XshdElement position,
      XshdReference<XshdRuleSet> ruleSetReference)
    {
      if (ruleSetReference.InlineElement != null)
        return (HighlightingRuleSet) ruleSetReference.InlineElement.AcceptVisitor((IXshdVisitor) this);
      if (ruleSetReference.ReferencedElement == null)
        return (HighlightingRuleSet) null;
      return this.GetDefinition(position, ruleSetReference.ReferencedDefinition).GetNamedRuleSet(ruleSetReference.ReferencedElement) ?? throw XmlHighlightingDefinition.Error(position, $"Could not find rule set named '{ruleSetReference.ReferencedElement}'.");
    }

    public object VisitSpan(XshdSpan span)
    {
      string regex = span.EndRegex;
      if (string.IsNullOrEmpty(span.BeginRegex) && string.IsNullOrEmpty(span.EndRegex))
        throw XmlHighlightingDefinition.Error((XshdElement) span, "Span has no start/end regex.");
      if (!span.Multiline)
        regex = regex != null ? (span.EndRegexType != XshdRegexType.IgnorePatternWhitespace ? $"($|{regex})" : $"($|{regex}\n)") : "$";
      HighlightingColor color = this.GetColor((XshdElement) span, span.SpanColorReference);
      return (object) new HighlightingSpan()
      {
        StartExpression = this.CreateRegex((XshdElement) span, span.BeginRegex, span.BeginRegexType),
        EndExpression = this.CreateRegex((XshdElement) span, regex, span.EndRegexType),
        RuleSet = this.GetRuleSet((XshdElement) span, span.RuleSetReference),
        StartColor = this.GetColor((XshdElement) span, span.BeginColorReference),
        SpanColor = color,
        EndColor = this.GetColor((XshdElement) span, span.EndColorReference),
        SpanColorIncludesStart = true,
        SpanColorIncludesEnd = true
      };
    }

    public object VisitImport(XshdImport import)
    {
      HighlightingRuleSet ruleSet1 = this.GetRuleSet((XshdElement) import, import.RuleSetReference);
      XshdRuleSet ruleSet2;
      if (this.reverseRuleSetDict.TryGetValue(ruleSet1, out ruleSet2))
        this.VisitRuleSet(ruleSet2);
      return (object) ruleSet1;
    }

    public object VisitRule(XshdRule rule)
    {
      return (object) new HighlightingRule()
      {
        Color = this.GetColor((XshdElement) rule, rule.ColorReference),
        Regex = this.CreateRegex((XshdElement) rule, rule.Regex, rule.RegexType)
      };
    }
  }
}
