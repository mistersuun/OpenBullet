// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingSpan
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public class HighlightingSpan
{
  public Regex StartExpression { get; set; }

  public Regex EndExpression { get; set; }

  public HighlightingRuleSet RuleSet { get; set; }

  public HighlightingColor StartColor { get; set; }

  public HighlightingColor SpanColor { get; set; }

  public HighlightingColor EndColor { get; set; }

  public bool SpanColorIncludesStart { get; set; }

  public bool SpanColorIncludesEnd { get; set; }

  public override string ToString()
  {
    return $"[{this.GetType().Name} Start={(object) this.StartExpression}, End={(object) this.EndExpression}]";
  }
}
