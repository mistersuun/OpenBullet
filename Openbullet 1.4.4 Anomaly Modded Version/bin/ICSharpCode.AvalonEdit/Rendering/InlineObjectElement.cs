// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.InlineObjectElement
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class InlineObjectElement : VisualLineElement
{
  public UIElement Element { get; private set; }

  public InlineObjectElement(int documentLength, UIElement element)
    : base(1, documentLength)
  {
    this.Element = element != null ? element : throw new ArgumentNullException(nameof (element));
  }

  public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
  {
    if (context == null)
      throw new ArgumentNullException(nameof (context));
    return (TextRun) new InlineObjectRun(1, (System.Windows.Media.TextFormatting.TextRunProperties) this.TextRunProperties, this.Element);
  }
}
