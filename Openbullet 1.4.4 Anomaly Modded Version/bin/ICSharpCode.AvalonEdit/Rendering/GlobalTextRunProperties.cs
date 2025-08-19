// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.GlobalTextRunProperties
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class GlobalTextRunProperties : TextRunProperties
{
  internal Typeface typeface;
  internal double fontRenderingEmSize;
  internal Brush foregroundBrush;
  internal Brush backgroundBrush;
  internal CultureInfo cultureInfo;

  public override Typeface Typeface => this.typeface;

  public override double FontRenderingEmSize => this.fontRenderingEmSize;

  public override double FontHintingEmSize => this.fontRenderingEmSize;

  public override TextDecorationCollection TextDecorations => (TextDecorationCollection) null;

  public override Brush ForegroundBrush => this.foregroundBrush;

  public override Brush BackgroundBrush => this.backgroundBrush;

  public override CultureInfo CultureInfo => this.cultureInfo;

  public override TextEffectCollection TextEffects => (TextEffectCollection) null;
}
