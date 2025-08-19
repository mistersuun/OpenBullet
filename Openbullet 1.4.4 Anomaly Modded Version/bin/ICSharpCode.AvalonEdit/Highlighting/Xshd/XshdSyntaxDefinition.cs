// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdSyntaxDefinition
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdSyntaxDefinition
{
  public XshdSyntaxDefinition()
  {
    this.Elements = (IList<XshdElement>) new NullSafeCollection<XshdElement>();
    this.Extensions = (IList<string>) new NullSafeCollection<string>();
  }

  public string Name { get; set; }

  public IList<string> Extensions { get; private set; }

  public IList<XshdElement> Elements { get; private set; }

  public void AcceptElements(IXshdVisitor visitor)
  {
    foreach (XshdElement element in (IEnumerable<XshdElement>) this.Elements)
      element.AcceptVisitor(visitor);
  }
}
