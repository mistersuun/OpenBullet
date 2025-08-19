// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.IXshdVisitor
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

public interface IXshdVisitor
{
  object VisitRuleSet(XshdRuleSet ruleSet);

  object VisitColor(XshdColor color);

  object VisitKeywords(XshdKeywords keywords);

  object VisitSpan(XshdSpan span);

  object VisitImport(XshdImport import);

  object VisitRule(XshdRule rule);
}
