// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public abstract class XshdElement
{
  public int LineNumber { get; set; }

  public int ColumnNumber { get; set; }

  public abstract object AcceptVisitor(IXshdVisitor visitor);
}
