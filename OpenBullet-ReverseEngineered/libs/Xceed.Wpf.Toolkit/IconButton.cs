// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.IconButton
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class IconButton : Button
{
  public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof (Icon), typeof (Image), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty IconLocationProperty = DependencyProperty.Register(nameof (IconLocation), typeof (Location), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) Location.Left));
  public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register(nameof (MouseOverBackground), typeof (Brush), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty MouseOverBorderBrushProperty = DependencyProperty.Register(nameof (MouseOverBorderBrush), typeof (Brush), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register(nameof (MouseOverForeground), typeof (Brush), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty MousePressedBackgroundProperty = DependencyProperty.Register(nameof (MousePressedBackground), typeof (Brush), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty MousePressedBorderBrushProperty = DependencyProperty.Register(nameof (MousePressedBorderBrush), typeof (Brush), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty MousePressedForegroundProperty = DependencyProperty.Register(nameof (MousePressedForeground), typeof (Brush), typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));

  static IconButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (IconButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (IconButton)));
  }

  public Image Icon
  {
    get => (Image) this.GetValue(IconButton.IconProperty);
    set => this.SetValue(IconButton.IconProperty, (object) value);
  }

  public Location IconLocation
  {
    get => (Location) this.GetValue(IconButton.IconLocationProperty);
    set => this.SetValue(IconButton.IconLocationProperty, (object) value);
  }

  public Brush MouseOverBackground
  {
    get => (Brush) this.GetValue(IconButton.MouseOverBackgroundProperty);
    set => this.SetValue(IconButton.MouseOverBackgroundProperty, (object) value);
  }

  public Brush MouseOverBorderBrush
  {
    get => (Brush) this.GetValue(IconButton.MouseOverBorderBrushProperty);
    set => this.SetValue(IconButton.MouseOverBorderBrushProperty, (object) value);
  }

  public Brush MouseOverForeground
  {
    get => (Brush) this.GetValue(IconButton.MouseOverForegroundProperty);
    set => this.SetValue(IconButton.MouseOverForegroundProperty, (object) value);
  }

  public Brush MousePressedBackground
  {
    get => (Brush) this.GetValue(IconButton.MousePressedBackgroundProperty);
    set => this.SetValue(IconButton.MousePressedBackgroundProperty, (object) value);
  }

  public Brush MousePressedBorderBrush
  {
    get => (Brush) this.GetValue(IconButton.MousePressedBorderBrushProperty);
    set => this.SetValue(IconButton.MousePressedBorderBrushProperty, (object) value);
  }

  public Brush MousePressedForeground
  {
    get => (Brush) this.GetValue(IconButton.MousePressedForegroundProperty);
    set => this.SetValue(IconButton.MousePressedForegroundProperty, (object) value);
  }
}
