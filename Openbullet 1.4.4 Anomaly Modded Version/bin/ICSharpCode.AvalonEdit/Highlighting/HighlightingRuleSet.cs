// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingRuleSet
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public class HighlightingRuleSet
{
  public HighlightingRuleSet()
  {
    this.Spans = (IList<HighlightingSpan>) new NullSafeCollection<HighlightingSpan>();
    this.Rules = (IList<HighlightingRule>) new NullSafeCollection<HighlightingRule>();
  }

  public string Name { get; set; }

  public IList<HighlightingSpan> Spans { get; private set; }

  public IList<HighlightingRule> Rules { get; private set; }

  public override string ToString() => $"[{this.GetType().Name} {this.Name}]";
}
