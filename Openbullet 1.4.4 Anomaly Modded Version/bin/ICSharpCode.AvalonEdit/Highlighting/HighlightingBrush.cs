// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingBrush
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public abstract class HighlightingBrush
{
  public abstract Brush GetBrush(ITextRunConstructionContext context);

  public virtual Color? GetColor(ITextRunConstructionContext context)
  {
    return this.GetBrush(context) is SolidColorBrush brush ? new Color?(brush.Color) : new Color?();
  }
}
