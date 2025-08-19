// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdSpan
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdSpan : XshdElement
{
  public string BeginRegex { get; set; }

  public XshdRegexType BeginRegexType { get; set; }

  public string EndRegex { get; set; }

  public XshdRegexType EndRegexType { get; set; }

  public bool Multiline { get; set; }

  public XshdReference<XshdRuleSet> RuleSetReference { get; set; }

  public XshdReference<XshdColor> SpanColorReference { get; set; }

  public XshdReference<XshdColor> BeginColorReference { get; set; }

  public XshdReference<XshdColor> EndColorReference { get; set; }

  public override object AcceptVisitor(IXshdVisitor visitor) => visitor.VisitSpan(this);
}
