// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineElementTextRunProperties
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class VisualLineElementTextRunProperties : TextRunProperties, ICloneable
{
  private Brush backgroundBrush;
  private BaselineAlignment baselineAlignment;
  private CultureInfo cultureInfo;
  private double fontHintingEmSize;
  private double fontRenderingEmSize;
  private Brush foregroundBrush;
  private Typeface typeface;
  private TextDecorationCollection textDecorations;
  private TextEffectCollection textEffects;
  private TextRunTypographyProperties typographyProperties;
  private NumberSubstitution numberSubstitution;

  public VisualLineElementTextRunProperties(TextRunProperties textRunProperties)
  {
    this.backgroundBrush = textRunProperties != null ? textRunProperties.BackgroundBrush : throw new ArgumentNullException(nameof (textRunProperties));
    this.baselineAlignment = textRunProperties.BaselineAlignment;
    this.cultureInfo = textRunProperties.CultureInfo;
    this.fontHintingEmSize = textRunProperties.FontHintingEmSize;
    this.fontRenderingEmSize = textRunProperties.FontRenderingEmSize;
    this.foregroundBrush = textRunProperties.ForegroundBrush;
    this.typeface = textRunProperties.Typeface;
    this.textDecorations = textRunProperties.TextDecorations;
    if (this.textDecorations != null && !this.textDecorations.IsFrozen)
      this.textDecorations = this.textDecorations.Clone();
    this.textEffects = textRunProperties.TextEffects;
    if (this.textEffects != null && !this.textEffects.IsFrozen)
      this.textEffects = this.textEffects.Clone();
    this.typographyProperties = textRunProperties.TypographyProperties;
    this.numberSubstitution = textRunProperties.NumberSubstitution;
  }

  public virtual VisualLineElementTextRunProperties Clone()
  {
    return new VisualLineElementTextRunProperties((TextRunProperties) this);
  }

  object ICloneable.Clone() => (object) this.Clone();

  public override Brush BackgroundBrush => this.backgroundBrush;

  public void SetBackgroundBrush(Brush value) => this.backgroundBrush = value;

  public override BaselineAlignment BaselineAlignment => this.baselineAlignment;

  public void SetBaselineAlignment(BaselineAlignment value) => this.baselineAlignment = value;

  public override CultureInfo CultureInfo => this.cultureInfo;

  public void SetCultureInfo(CultureInfo value)
  {
    this.cultureInfo = value != null ? value : throw new ArgumentNullException(nameof (value));
  }

  public override double FontHintingEmSize => this.fontHintingEmSize;

  public void SetFontHintingEmSize(double value) => this.fontHintingEmSize = value;

  public override double FontRenderingEmSize => this.fontRenderingEmSize;

  public void SetFontRenderingEmSize(double value) => this.fontRenderingEmSize = value;

  public override Brush ForegroundBrush => this.foregroundBrush;

  public void SetForegroundBrush(Brush value) => this.foregroundBrush = value;

  public override Typeface Typeface => this.typeface;

  public void SetTypeface(Typeface value)
  {
    this.typeface = value != null ? value : throw new ArgumentNullException(nameof (value));
  }

  public override TextDecorationCollection TextDecorations => this.textDecorations;

  public void SetTextDecorations(TextDecorationCollection value) => this.textDecorations = value;

  public override TextEffectCollection TextEffects => this.textEffects;

  public void SetTextEffects(TextEffectCollection value) => this.textEffects = value;

  public override TextRunTypographyProperties TypographyProperties => this.typographyProperties;

  public void SetTypographyProperties(TextRunTypographyProperties value)
  {
    this.typographyProperties = value;
  }

  public override NumberSubstitution NumberSubstitution => this.numberSubstitution;

  public void SetNumberSubstitution(NumberSubstitution value) => this.numberSubstitution = value;
}
