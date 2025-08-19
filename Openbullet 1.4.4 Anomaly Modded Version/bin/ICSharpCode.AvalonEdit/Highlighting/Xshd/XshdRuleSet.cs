// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdRuleSet
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdRuleSet : XshdElement
{
  private readonly NullSafeCollection<XshdElement> elements = new NullSafeCollection<XshdElement>();

  public string Name { get; set; }

  public bool? IgnoreCase { get; set; }

  public IList<XshdElement> Elements => (IList<XshdElement>) this.elements;

  public void AcceptElements(IXshdVisitor visitor)
  {
    foreach (XshdElement element in (IEnumerable<XshdElement>) this.Elements)
      element.AcceptVisitor(visitor);
  }

  public override object AcceptVisitor(IXshdVisitor visitor) => visitor.VisitRuleSet(this);
}
