// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.RandomPanel
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public class RandomPanel : AnimationPanel
{
  public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.Register(nameof (MinimumWidth), typeof (double), typeof (RandomPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 10.0, new PropertyChangedCallback(RandomPanel.OnMinimumWidthChanged), new CoerceValueCallback(RandomPanel.CoerceMinimumWidth)));
  public static readonly DependencyProperty MinimumHeightProperty = DependencyProperty.Register(nameof (MinimumHeight), typeof (double), typeof (RandomPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 10.0, new PropertyChangedCallback(RandomPanel.OnMinimumHeightChanged), new CoerceValueCallback(RandomPanel.CoerceMinimumHeight)));
  public static readonly DependencyProperty MaximumWidthProperty = DependencyProperty.Register(nameof (MaximumWidth), typeof (double), typeof (RandomPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 100.0, new PropertyChangedCallback(RandomPanel.OnMaximumWidthChanged), new CoerceValueCallback(RandomPanel.CoerceMaximumWidth)));
  public static readonly DependencyProperty MaximumHeightProperty = DependencyProperty.Register(nameof (MaximumHeight), typeof (double), typeof (RandomPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 100.0, new PropertyChangedCallback(RandomPanel.OnMaximumHeightChanged), new CoerceValueCallback(RandomPanel.CoerceMaximumHeight)));
  public static readonly DependencyProperty SeedProperty = DependencyProperty.Register(nameof (Seed), typeof (int), typeof (RandomPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0, new PropertyChangedCallback(RandomPanel.SeedChanged)));
  private static readonly DependencyProperty ActualSizeProperty = DependencyProperty.RegisterAttached("ActualSize", typeof (Size), typeof (RandomPanel), (PropertyMetadata) new UIPropertyMetadata((object) new Size()));
  private Random _random = new Random();

  public double MinimumWidth
  {
    get => (double) this.GetValue(RandomPanel.MinimumWidthProperty);
    set => this.SetValue(RandomPanel.MinimumWidthProperty, (object) value);
  }

  private static void OnMinimumWidthChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    randomPanel.CoerceValue(RandomPanel.MaximumWidthProperty);
    randomPanel.InvalidateMeasure();
  }

  private static object CoerceMinimumWidth(DependencyObject d, object baseValue)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    double d1 = (double) baseValue;
    if (double.IsNaN(d1) || double.IsInfinity(d1) || d1 < 0.0)
      return DependencyProperty.UnsetValue;
    double maximumWidth = randomPanel.MaximumWidth;
    return d1 > maximumWidth ? (object) maximumWidth : baseValue;
  }

  public double MinimumHeight
  {
    get => (double) this.GetValue(RandomPanel.MinimumHeightProperty);
    set => this.SetValue(RandomPanel.MinimumHeightProperty, (object) value);
  }

  private static void OnMinimumHeightChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    randomPanel.CoerceValue(RandomPanel.MaximumHeightProperty);
    randomPanel.InvalidateMeasure();
  }

  private static object CoerceMinimumHeight(DependencyObject d, object baseValue)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    double d1 = (double) baseValue;
    if (double.IsNaN(d1) || double.IsInfinity(d1) || d1 < 0.0)
      return DependencyProperty.UnsetValue;
    double maximumHeight = randomPanel.MaximumHeight;
    return d1 > maximumHeight ? (object) maximumHeight : baseValue;
  }

  public double MaximumWidth
  {
    get => (double) this.GetValue(RandomPanel.MaximumWidthProperty);
    set => this.SetValue(RandomPanel.MaximumWidthProperty, (object) value);
  }

  private static void OnMaximumWidthChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    randomPanel.CoerceValue(RandomPanel.MinimumWidthProperty);
    randomPanel.InvalidateMeasure();
  }

  private static object CoerceMaximumWidth(DependencyObject d, object baseValue)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    double d1 = (double) baseValue;
    if (double.IsNaN(d1) || double.IsInfinity(d1) || d1 < 0.0)
      return DependencyProperty.UnsetValue;
    double minimumWidth = randomPanel.MinimumWidth;
    return d1 < minimumWidth ? (object) minimumWidth : baseValue;
  }

  public double MaximumHeight
  {
    get => (double) this.GetValue(RandomPanel.MaximumHeightProperty);
    set => this.SetValue(RandomPanel.MaximumHeightProperty, (object) value);
  }

  private static void OnMaximumHeightChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    randomPanel.CoerceValue(RandomPanel.MinimumHeightProperty);
    randomPanel.InvalidateMeasure();
  }

  private static object CoerceMaximumHeight(DependencyObject d, object baseValue)
  {
    RandomPanel randomPanel = (RandomPanel) d;
    double d1 = (double) baseValue;
    if (double.IsNaN(d1) || double.IsInfinity(d1) || d1 < 0.0)
      return DependencyProperty.UnsetValue;
    double minimumHeight = randomPanel.MinimumHeight;
    return d1 < minimumHeight ? (object) minimumHeight : baseValue;
  }

  public int Seed
  {
    get => (int) this.GetValue(RandomPanel.SeedProperty);
    set => this.SetValue(RandomPanel.SeedProperty, (object) value);
  }

  private static void SeedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    if (!(obj is RandomPanel))
      return;
    RandomPanel randomPanel = (RandomPanel) obj;
    randomPanel._random = new Random((int) args.NewValue);
    randomPanel.InvalidateArrange();
  }

  private static Size GetActualSize(DependencyObject obj)
  {
    return (Size) obj.GetValue(RandomPanel.ActualSizeProperty);
  }

  private static void SetActualSize(DependencyObject obj, Size value)
  {
    obj.SetValue(RandomPanel.ActualSizeProperty, (object) value);
  }

  protected override Size MeasureChildrenOverride(UIElementCollection children, Size constraint)
  {
    Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
    foreach (UIElement child in children)
    {
      if (child != null)
      {
        Size availableSize = new Size(1.0 * (double) this._random.Next(Convert.ToInt32(this.MinimumWidth), Convert.ToInt32(this.MaximumWidth)), 1.0 * (double) this._random.Next(Convert.ToInt32(this.MinimumHeight), Convert.ToInt32(this.MaximumHeight)));
        child.Measure(availableSize);
        RandomPanel.SetActualSize((DependencyObject) child, availableSize);
      }
    }
    return new Size();
  }

  protected override Size ArrangeChildrenOverride(UIElementCollection children, Size finalSize)
  {
    foreach (UIElement child in children)
    {
      if (child != null)
      {
        Size actualSize = RandomPanel.GetActualSize((DependencyObject) child);
        double x = (double) this._random.Next(0, (int) Math.Max(finalSize.Width - actualSize.Width, 0.0));
        double y = (double) this._random.Next(0, (int) Math.Max(finalSize.Height - actualSize.Height, 0.0));
        double width = Math.Min(finalSize.Width, actualSize.Width);
        double height = Math.Min(finalSize.Height, actualSize.Height);
        this.ArrangeChild(child, new Rect(new Point(x, y), new Size(width, height)));
      }
    }
    return finalSize;
  }
}
