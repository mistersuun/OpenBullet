// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdKeywords
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdKeywords : XshdElement
{
  private readonly NullSafeCollection<string> words = new NullSafeCollection<string>();

  public XshdReference<XshdColor> ColorReference { get; set; }

  public IList<string> Words => (IList<string>) this.words;

  public override object AcceptVisitor(IXshdVisitor visitor) => visitor.VisitKeywords(this);
}
