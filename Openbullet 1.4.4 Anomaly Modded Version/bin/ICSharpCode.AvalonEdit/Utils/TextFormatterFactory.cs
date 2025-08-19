// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.TextFormatterFactory
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal static class TextFormatterFactory
{
  public static TextFormatter Create(DependencyObject owner)
  {
    return owner != null ? TextFormatter.Create(TextOptions.GetTextFormattingMode(owner)) : throw new ArgumentNullException(nameof (owner));
  }

  public static bool PropertyChangeAffectsTextFormatter(DependencyProperty dp)
  {
    return dp == TextOptions.TextFormattingModeProperty;
  }

  public static FormattedText CreateFormattedText(
    FrameworkElement element,
    string text,
    Typeface typeface,
    double? emSize,
    Brush foreground)
  {
    if (element == null)
      throw new ArgumentNullException(nameof (element));
    if (text == null)
      throw new ArgumentNullException(nameof (text));
    if (typeface == null)
      typeface = element.CreateTypeface();
    if (!emSize.HasValue)
      emSize = new double?(TextBlock.GetFontSize((DependencyObject) element));
    if (foreground == null)
      foreground = TextBlock.GetForeground((DependencyObject) element);
    return new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, emSize.Value, foreground, (NumberSubstitution) null, TextOptions.GetTextFormattingMode((DependencyObject) element));
  }
}
