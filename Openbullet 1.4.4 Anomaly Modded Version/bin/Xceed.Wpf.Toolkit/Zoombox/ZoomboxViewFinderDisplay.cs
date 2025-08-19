// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Zoombox.ZoomboxViewFinderDisplay
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Zoombox;

public class ZoomboxViewFinderDisplay : FrameworkElement
{
  public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof (Background), typeof (Brush), typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) new SolidColorBrush(Color.FromArgb((byte) 192 /*0xC0*/, byte.MaxValue, byte.MaxValue, byte.MaxValue)), FrameworkPropertyMetadataOptions.AffectsRender));
  private static readonly DependencyPropertyKey ContentBoundsPropertyKey = DependencyProperty.RegisterReadOnly(nameof (ContentBounds), typeof (Rect), typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) Rect.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty ContentBoundsProperty = ZoomboxViewFinderDisplay.ContentBoundsPropertyKey.DependencyProperty;
  public static readonly DependencyProperty ShadowBrushProperty = DependencyProperty.Register(nameof (ShadowBrush), typeof (Brush), typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) new SolidColorBrush(Color.FromArgb((byte) 128 /*0x80*/, byte.MaxValue, byte.MaxValue, byte.MaxValue)), FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty ViewportBrushProperty = DependencyProperty.Register(nameof (ViewportBrush), typeof (Brush), typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty ViewportPenProperty = DependencyProperty.Register(nameof (ViewportPen), typeof (Pen), typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Pen((Brush) new SolidColorBrush(Color.FromArgb((byte) 128 /*0x80*/, (byte) 0, (byte) 0, (byte) 0)), 1.0), FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty ViewportRectProperty = DependencyProperty.Register(nameof (ViewportRect), typeof (Rect), typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) Rect.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
  private static readonly DependencyPropertyKey VisualBrushPropertyKey = DependencyProperty.RegisterReadOnly(nameof (VisualBrush), typeof (VisualBrush), typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty VisualBrushProperty = ZoomboxViewFinderDisplay.VisualBrushPropertyKey.DependencyProperty;
  private Size _availableSize = Size.Empty;
  private double _scale = 1.0;

  static ZoomboxViewFinderDisplay()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ZoomboxViewFinderDisplay), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ZoomboxViewFinderDisplay)));
  }

  public Brush Background
  {
    get => (Brush) this.GetValue(ZoomboxViewFinderDisplay.BackgroundProperty);
    set => this.SetValue(ZoomboxViewFinderDisplay.BackgroundProperty, (object) value);
  }

  internal Rect ContentBounds
  {
    get => (Rect) this.GetValue(ZoomboxViewFinderDisplay.ContentBoundsProperty);
    set => this.SetValue(ZoomboxViewFinderDisplay.ContentBoundsPropertyKey, (object) value);
  }

  public Brush ShadowBrush
  {
    get => (Brush) this.GetValue(ZoomboxViewFinderDisplay.ShadowBrushProperty);
    set => this.SetValue(ZoomboxViewFinderDisplay.ShadowBrushProperty, (object) value);
  }

  public Brush ViewportBrush
  {
    get => (Brush) this.GetValue(ZoomboxViewFinderDisplay.ViewportBrushProperty);
    set => this.SetValue(ZoomboxViewFinderDisplay.ViewportBrushProperty, (object) value);
  }

  public Pen ViewportPen
  {
    get => (Pen) this.GetValue(ZoomboxViewFinderDisplay.ViewportPenProperty);
    set => this.SetValue(ZoomboxViewFinderDisplay.ViewportPenProperty, (object) value);
  }

  public Rect ViewportRect
  {
    get => (Rect) this.GetValue(ZoomboxViewFinderDisplay.ViewportRectProperty);
    set => this.SetValue(ZoomboxViewFinderDisplay.ViewportRectProperty, (object) value);
  }

  internal VisualBrush VisualBrush
  {
    get => (VisualBrush) this.GetValue(ZoomboxViewFinderDisplay.VisualBrushProperty);
    set => this.SetValue(ZoomboxViewFinderDisplay.VisualBrushPropertyKey, (object) value);
  }

  internal Size AvailableSize => this._availableSize;

  internal double Scale
  {
    get => this._scale;
    set => this._scale = value;
  }

  protected override Size ArrangeOverride(Size finalSize) => this.DesiredSize;

  protected override Size MeasureOverride(Size availableSize)
  {
    this._availableSize = availableSize;
    Size size = new Size(DoubleHelper.IsNaN(this.ContentBounds.Width) ? 0.0 : Math.Max(0.0, this.ContentBounds.Width), DoubleHelper.IsNaN(this.ContentBounds.Height) ? 0.0 : Math.Max(0.0, this.ContentBounds.Height));
    if (size.Width > availableSize.Width || size.Height > availableSize.Height)
    {
      double num1 = availableSize.Width / size.Width;
      double num2 = availableSize.Height / size.Height;
      double num3 = num1 < num2 ? num1 : num2;
      size = new Size(size.Width * num3, size.Height * num3);
    }
    return size;
  }

  protected override void OnRender(DrawingContext dc)
  {
    base.OnRender(dc);
    dc.DrawRectangle(this.Background, (Pen) null, this.ContentBounds);
    dc.DrawRectangle((Brush) this.VisualBrush, (Pen) null, this.ContentBounds);
    if (this.ViewportRect.IntersectsWith(new Rect(this.RenderSize)))
    {
      Rect rectangle1 = new Rect(new Point(0.0, 0.0), new Size(this.RenderSize.Width, Math.Max(0.0, this.ViewportRect.Top)));
      Rect rectangle2 = new Rect(new Point(0.0, this.ViewportRect.Top), new Size(Math.Max(0.0, this.ViewportRect.Left), this.ViewportRect.Height));
      Rect rectangle3;
      ref Rect local1 = ref rectangle3;
      Rect viewportRect1 = this.ViewportRect;
      double right1 = viewportRect1.Right;
      viewportRect1 = this.ViewportRect;
      double top = viewportRect1.Top;
      Point location1 = new Point(right1, top);
      double width1 = this.RenderSize.Width;
      viewportRect1 = this.ViewportRect;
      double right2 = viewportRect1.Right;
      Size size1 = new Size(Math.Max(0.0, width1 - right2), this.ViewportRect.Height);
      local1 = new Rect(location1, size1);
      Rect rectangle4;
      ref Rect local2 = ref rectangle4;
      Rect viewportRect2 = this.ViewportRect;
      Point location2 = new Point(0.0, viewportRect2.Bottom);
      Size renderSize = this.RenderSize;
      double width2 = renderSize.Width;
      renderSize = this.RenderSize;
      double height1 = renderSize.Height;
      viewportRect2 = this.ViewportRect;
      double bottom = viewportRect2.Bottom;
      double height2 = Math.Max(0.0, height1 - bottom);
      Size size2 = new Size(width2, height2);
      local2 = new Rect(location2, size2);
      dc.DrawRectangle(this.ShadowBrush, (Pen) null, rectangle1);
      dc.DrawRectangle(this.ShadowBrush, (Pen) null, rectangle2);
      dc.DrawRectangle(this.ShadowBrush, (Pen) null, rectangle3);
      dc.DrawRectangle(this.ShadowBrush, (Pen) null, rectangle4);
      dc.DrawRectangle(this.ViewportBrush, this.ViewportPen, this.ViewportRect);
    }
    else
      dc.DrawRectangle(this.ShadowBrush, (Pen) null, new Rect(this.RenderSize));
  }
}
