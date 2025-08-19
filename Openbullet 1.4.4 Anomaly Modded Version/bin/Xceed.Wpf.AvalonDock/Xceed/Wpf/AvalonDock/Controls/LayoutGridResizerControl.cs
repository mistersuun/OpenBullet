// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutGridResizerControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutGridResizerControl : Thumb
{
  public static readonly DependencyProperty BackgroundWhileDraggingProperty = DependencyProperty.Register(nameof (BackgroundWhileDragging), typeof (Brush), typeof (LayoutGridResizerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Black));
  public static readonly DependencyProperty OpacityWhileDraggingProperty = DependencyProperty.Register(nameof (OpacityWhileDragging), typeof (double), typeof (LayoutGridResizerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.5));

  static LayoutGridResizerControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutGridResizerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutGridResizerControl)));
    FrameworkElement.HorizontalAlignmentProperty.OverrideMetadata(typeof (LayoutGridResizerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    FrameworkElement.VerticalAlignmentProperty.OverrideMetadata(typeof (LayoutGridResizerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    Control.BackgroundProperty.OverrideMetadata(typeof (LayoutGridResizerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Transparent));
    UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof (LayoutGridResizerControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, (PropertyChangedCallback) null));
  }

  public Brush BackgroundWhileDragging
  {
    get => (Brush) this.GetValue(LayoutGridResizerControl.BackgroundWhileDraggingProperty);
    set => this.SetValue(LayoutGridResizerControl.BackgroundWhileDraggingProperty, (object) value);
  }

  public double OpacityWhileDragging
  {
    get => (double) this.GetValue(LayoutGridResizerControl.OpacityWhileDraggingProperty);
    set => this.SetValue(LayoutGridResizerControl.OpacityWhileDraggingProperty, (object) value);
  }
}
