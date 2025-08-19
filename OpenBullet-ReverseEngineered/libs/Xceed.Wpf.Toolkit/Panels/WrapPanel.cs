// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.WrapPanel
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public class WrapPanel : AnimationPanel
{
  public static readonly DependencyProperty OrientationProperty = StackPanel.OrientationProperty.AddOwner(typeof (WrapPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Horizontal, new PropertyChangedCallback(WrapPanel.OnOrientationChanged)));
  private Orientation _orientation;
  public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(nameof (ItemWidth), typeof (double), typeof (WrapPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(WrapPanel.OnInvalidateMeasure)), new ValidateValueCallback(WrapPanel.IsWidthHeightValid));
  public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(nameof (ItemHeight), typeof (double), typeof (WrapPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(WrapPanel.OnInvalidateMeasure)), new ValidateValueCallback(WrapPanel.IsWidthHeightValid));
  public static readonly DependencyProperty IsStackReversedProperty = DependencyProperty.Register(nameof (IsChildOrderReversed), typeof (bool), typeof (WrapPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(WrapPanel.OnInvalidateMeasure)));

  public Orientation Orientation
  {
    get => this._orientation;
    set => this.SetValue(WrapPanel.OrientationProperty, (object) value);
  }

  private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    WrapPanel wrapPanel = (WrapPanel) d;
    wrapPanel._orientation = (Orientation) e.NewValue;
    wrapPanel.InvalidateMeasure();
  }

  [TypeConverter(typeof (LengthConverter))]
  public double ItemWidth
  {
    get => (double) this.GetValue(WrapPanel.ItemWidthProperty);
    set => this.SetValue(WrapPanel.ItemWidthProperty, (object) value);
  }

  [TypeConverter(typeof (LengthConverter))]
  public double ItemHeight
  {
    get => (double) this.GetValue(WrapPanel.ItemHeightProperty);
    set => this.SetValue(WrapPanel.ItemHeightProperty, (object) value);
  }

  public bool IsChildOrderReversed
  {
    get => (bool) this.GetValue(WrapPanel.IsStackReversedProperty);
    set => this.SetValue(WrapPanel.IsStackReversedProperty, (object) value);
  }

  protected override Size MeasureChildrenOverride(UIElementCollection children, Size constraint)
  {
    double val2_1 = 0.0;
    double num1 = 0.0;
    bool flag1 = this.Orientation == Orientation.Horizontal;
    double num2 = flag1 ? constraint.Width : constraint.Height;
    double itemWidth = this.ItemWidth;
    double itemHeight = this.ItemHeight;
    int num3 = flag1 ? 1 : 0;
    bool flag2 = !double.IsNaN(itemWidth);
    bool flag3 = !double.IsNaN(itemHeight);
    int num4 = flag1 ? 1 : 0;
    double val1_1 = 0.0;
    double val2_2 = 0.0;
    Size availableSize = new Size(flag2 ? itemWidth : constraint.Width, flag3 ? itemHeight : constraint.Height);
    int num5 = this.IsChildOrderReversed ? 1 : 0;
    int num6 = num5 != 0 ? children.Count - 1 : 0;
    if (num5 == 0)
    {
      int count = children.Count;
    }
    int num7 = num5 != 0 ? -1 : 1;
    int index1 = num6;
    for (int index2 = 0; index2 < children.Count; ++index2)
    {
      UIElement child = children[index1];
      child.Measure(availableSize);
      Size desiredSize;
      double num8;
      if (!flag1)
      {
        if (!flag3)
        {
          desiredSize = child.DesiredSize;
          num8 = desiredSize.Height;
        }
        else
          num8 = itemHeight;
      }
      else if (!flag2)
      {
        desiredSize = child.DesiredSize;
        num8 = desiredSize.Width;
      }
      else
        num8 = itemWidth;
      double val1_2 = num8;
      double num9;
      if (!flag1)
      {
        if (!flag2)
        {
          desiredSize = child.DesiredSize;
          num9 = desiredSize.Width;
        }
        else
          num9 = itemWidth;
      }
      else if (!flag3)
      {
        desiredSize = child.DesiredSize;
        num9 = desiredSize.Height;
      }
      else
        num9 = itemHeight;
      double val1_3 = num9;
      if (val1_1 + val1_2 > num2)
      {
        val2_1 = Math.Max(val1_1, val2_1);
        num1 += val2_2;
        val1_1 = val1_2;
        val2_2 = val1_3;
        if (val1_2 > num2)
        {
          val2_1 = Math.Max(val1_2, val2_1);
          num1 += val1_3;
          val1_1 = 0.0;
          val2_2 = 0.0;
        }
      }
      else
      {
        val1_1 += val1_2;
        val2_2 = Math.Max(val1_3, val2_2);
      }
      index1 += num7;
    }
    double num10 = Math.Max(val1_1, val2_1);
    double num11 = num1 + val2_2;
    return !flag1 ? new Size(num11, num10) : new Size(num10, num11);
  }

  protected override Size ArrangeChildrenOverride(UIElementCollection children, Size finalSize)
  {
    bool isHorizontal = this.Orientation == Orientation.Horizontal;
    double num1 = isHorizontal ? finalSize.Width : finalSize.Height;
    double itemWidth = this.ItemWidth;
    double itemHeight = this.ItemHeight;
    double itemExtent = isHorizontal ? itemWidth : itemHeight;
    bool flag1 = !double.IsNaN(itemWidth);
    bool flag2 = !double.IsNaN(itemHeight);
    bool useItemExtent = isHorizontal ? flag1 : flag2;
    double num2 = 0.0;
    double num3 = 0.0;
    double lineStackSum = 0.0;
    int num4 = this.IsChildOrderReversed ? children.Count - 1 : 0;
    if (!this.IsChildOrderReversed)
    {
      int count = children.Count;
    }
    int num5 = this.IsChildOrderReversed ? -1 : 1;
    Collection<UIElement> children1 = new Collection<UIElement>();
    int index1 = num4;
    for (int index2 = 0; index2 < children.Count; ++index2)
    {
      UIElement child = children[index1];
      double num6 = isHorizontal ? (flag1 ? itemWidth : child.DesiredSize.Width) : (flag2 ? itemHeight : child.DesiredSize.Height);
      double num7 = isHorizontal ? (flag2 ? itemHeight : child.DesiredSize.Height) : (flag1 ? itemWidth : child.DesiredSize.Width);
      if (num2 + num6 > num1)
      {
        this.ArrangeLineOfChildren(children1, isHorizontal, num3, lineStackSum, itemExtent, useItemExtent);
        lineStackSum += num3;
        num2 = num6;
        if (num6 > num1)
        {
          children1.Add(child);
          this.ArrangeLineOfChildren(children1, isHorizontal, num7, lineStackSum, itemExtent, useItemExtent);
          lineStackSum += num7;
          num2 = 0.0;
        }
        children1.Add(child);
      }
      else
      {
        children1.Add(child);
        num2 += num6;
        num3 = Math.Max(num7, num3);
      }
      index1 += num5;
    }
    if (children1.Count > 0)
      this.ArrangeLineOfChildren(children1, isHorizontal, num3, lineStackSum, itemExtent, useItemExtent);
    return finalSize;
  }

  private void ArrangeLineOfChildren(
    Collection<UIElement> children,
    bool isHorizontal,
    double lineStack,
    double lineStackSum,
    double itemExtent,
    bool useItemExtent)
  {
    double num1 = 0.0;
    foreach (UIElement child in children)
    {
      double num2 = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
      double num3 = useItemExtent ? itemExtent : num2;
      this.ArrangeChild(child, isHorizontal ? new Rect(num1, lineStackSum, num3, lineStack) : new Rect(lineStackSum, num1, lineStack, num3));
      num1 += num3;
    }
    children.Clear();
  }

  private static void OnInvalidateMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((AnimationPanel) d).InvalidateMeasure();
  }

  private static bool IsWidthHeightValid(object value)
  {
    double d = (double) value;
    if (DoubleHelper.IsNaN(d))
      return true;
    return d >= 0.0 && !double.IsPositiveInfinity(d);
  }
}
