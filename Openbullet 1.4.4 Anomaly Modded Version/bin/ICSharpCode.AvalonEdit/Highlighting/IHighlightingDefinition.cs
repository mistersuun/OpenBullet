// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[TypeConverter(typeof (HighlightingDefinitionTypeConverter))]
public interface IHighlightingDefinition
{
  string Name { get; }

  HighlightingRuleSet MainRuleSet { get; }

  HighlightingRuleSet GetNamedRuleSet(string name);

  HighlightingColor GetNamedColor(string name);

  IEnumerable<HighlightingColor> NamedHighlightingColors { get; }

  IDictionary<string, string> Properties { get; }
}
