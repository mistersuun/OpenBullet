// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingRule
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public class HighlightingRule
{
  public Regex Regex { get; set; }

  public HighlightingColor Color { get; set; }

  public override string ToString() => $"[{this.GetType().Name} {(object) this.Regex}]";
}
