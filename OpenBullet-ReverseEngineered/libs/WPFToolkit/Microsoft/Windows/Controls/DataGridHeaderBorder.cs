// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridHeaderBorder
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridHeaderBorder : Border
{
  private const string ClassicThemeName = "Classic";
  private const string AeroNormalColorName = "Aero.NormalColor";
  private const string LunaNormalColorName = "Luna.NormalColor";
  private const string LunaHomeSteadName = "Luna.HomeStead";
  private const string LunaMetallicName = "Luna.Metallic";
  private const string RoyaleNormalColorName = "Royale.NormalColor";
  public static readonly DependencyProperty IsHoveredProperty = DependencyProperty.Register(nameof (IsHovered), typeof (bool), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register(nameof (IsPressed), typeof (bool), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty IsClickableProperty = DependencyProperty.Register(nameof (IsClickable), typeof (bool), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(nameof (SortDirection), typeof (ListSortDirection?), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof (IsSelected), typeof (bool), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty SeparatorBrushProperty = DependencyProperty.Register(nameof (SeparatorBrush), typeof (Brush), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty SeparatorVisibilityProperty = DependencyProperty.Register(nameof (SeparatorVisibility), typeof (Visibility), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Visible));
  private static readonly DependencyProperty ControlBrushProperty = DependencyProperty.Register("ControlBrush", typeof (Brush), typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
  private static List<Freezable> _freezableCache;
  private static object _cacheAccess = new object();

  static DataGridHeaderBorder()
  {
    DataGridHelper.HookThemeChange(typeof (DataGridHeaderBorder), new PropertyChangedCallback(DataGridHeaderBorder.OnThemeChange));
    UIElement.SnapsToDevicePixelsProperty.OverrideMetadata(typeof (DataGridHeaderBorder), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  }

  public bool IsHovered
  {
    get => (bool) this.GetValue(DataGridHeaderBorder.IsHoveredProperty);
    set => this.SetValue(DataGridHeaderBorder.IsHoveredProperty, (object) value);
  }

  public bool IsPressed
  {
    get => (bool) this.GetValue(DataGridHeaderBorder.IsPressedProperty);
    set => this.SetValue(DataGridHeaderBorder.IsPressedProperty, (object) value);
  }

  public bool IsClickable
  {
    get => (bool) this.GetValue(DataGridHeaderBorder.IsClickableProperty);
    set => this.SetValue(DataGridHeaderBorder.IsClickableProperty, (object) value);
  }

  public ListSortDirection? SortDirection
  {
    get => (ListSortDirection?) this.GetValue(DataGridHeaderBorder.SortDirectionProperty);
    set => this.SetValue(DataGridHeaderBorder.SortDirectionProperty, (object) value);
  }

  public bool IsSelected
  {
    get => (bool) this.GetValue(DataGridHeaderBorder.IsSelectedProperty);
    set => this.SetValue(DataGridHeaderBorder.IsSelectedProperty, (object) value);
  }

  public Orientation Orientation
  {
    get => (Orientation) this.GetValue(DataGridHeaderBorder.OrientationProperty);
    set => this.SetValue(DataGridHeaderBorder.OrientationProperty, (object) value);
  }

  private bool UsingBorderImplementation => this.Background != null || this.BorderBrush != null;

  public Brush SeparatorBrush
  {
    get => (Brush) this.GetValue(DataGridHeaderBorder.SeparatorBrushProperty);
    set => this.SetValue(DataGridHeaderBorder.SeparatorBrushProperty, (object) value);
  }

  public Visibility SeparatorVisibility
  {
    get => (Visibility) this.GetValue(DataGridHeaderBorder.SeparatorVisibilityProperty);
    set => this.SetValue(DataGridHeaderBorder.SeparatorVisibilityProperty, (object) value);
  }

  private string Theme
  {
    get
    {
      string theme = DataGridHelper.GetTheme((FrameworkElement) this);
      if (string.IsNullOrEmpty(theme))
        theme = "Classic";
      return theme;
    }
  }

  private static void OnThemeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    DataGridHeaderBorder.ReleaseCache();
    DataGridHeaderBorder gridHeaderBorder = (DataGridHeaderBorder) d;
    gridHeaderBorder.InvalidateMeasure();
    gridHeaderBorder.InvalidateArrange();
    gridHeaderBorder.InvalidateVisual();
  }

  protected override Size MeasureOverride(Size constraint)
  {
    if (this.UsingBorderImplementation)
      return base.MeasureOverride(constraint);
    UIElement child = this.Child;
    if (child == null)
      return new Size();
    Thickness thickness = this.Padding;
    if (thickness.Equals(new Thickness()))
      thickness = this.DefaultPadding;
    double num1 = constraint.Width;
    double num2 = constraint.Height;
    if (!double.IsInfinity(num1))
      num1 = Math.Max(0.0, num1 - thickness.Left - thickness.Right);
    if (!double.IsInfinity(num2))
      num2 = Math.Max(0.0, num2 - thickness.Top - thickness.Bottom);
    child.Measure(new Size(num1, num2));
    Size desiredSize = child.DesiredSize;
    return new Size(desiredSize.Width + thickness.Left + thickness.Right, desiredSize.Height + thickness.Top + thickness.Bottom);
  }

  protected override Size ArrangeOverride(Size arrangeSize)
  {
    if (this.UsingBorderImplementation)
      return base.ArrangeOverride(arrangeSize);
    UIElement child = this.Child;
    if (child != null)
    {
      Thickness thickness = this.Padding;
      if (thickness.Equals(new Thickness()))
        thickness = this.DefaultPadding;
      double width = Math.Max(0.0, arrangeSize.Width - thickness.Left - thickness.Right);
      double height = Math.Max(0.0, arrangeSize.Height - thickness.Top - thickness.Bottom);
      child.Arrange(new Rect(thickness.Left, thickness.Top, width, height));
    }
    return arrangeSize;
  }

  protected override void OnRender(DrawingContext dc)
  {
    if (this.UsingBorderImplementation)
    {
      base.OnRender(dc);
    }
    else
    {
      switch (this.Theme)
      {
        case "Classic":
          this.RenderClassic(dc);
          break;
        case "Luna.NormalColor":
          this.RenderLuna(dc, DataGridHeaderBorder.Luna.NormalColor);
          break;
        case "Luna.HomeStead":
          this.RenderLuna(dc, DataGridHeaderBorder.Luna.HomeStead);
          break;
        case "Luna.Metallic":
          this.RenderLuna(dc, DataGridHeaderBorder.Luna.Metallic);
          break;
        case "Royale.NormalColor":
          this.RenderLuna(dc, DataGridHeaderBorder.Luna.Metallic);
          break;
        case "Aero.NormalColor":
          this.RenderAeroNormalColor(dc);
          break;
      }
    }
  }

  private Thickness DefaultPadding
  {
    get
    {
      Thickness defaultPadding = new Thickness(3.0);
      if (this.Orientation == Orientation.Vertical)
      {
        if (this.Theme == "Aero.NormalColor")
          defaultPadding = new Thickness(5.0, 4.0, 5.0, 4.0);
        else
          defaultPadding.Right = 15.0;
      }
      if (this.IsPressed && this.IsClickable)
      {
        ++defaultPadding.Left;
        ++defaultPadding.Top;
        --defaultPadding.Right;
        --defaultPadding.Bottom;
      }
      return defaultPadding;
    }
  }

  private static double Max0(double d) => Math.Max(0.0, d);

  private void RenderAeroNormalColor(DrawingContext dc)
  {
    Size renderSize = this.RenderSize;
    bool flag1 = this.Orientation == Orientation.Horizontal;
    bool flag2 = this.IsClickable && this.IsEnabled;
    bool flag3 = flag2 && this.IsHovered;
    bool flag4 = flag2 && this.IsPressed;
    ListSortDirection? sortDirection = this.SortDirection;
    bool hasValue = sortDirection.HasValue;
    bool isSelected = this.IsSelected;
    bool flag5 = !flag3 && !flag4 && !hasValue && !isSelected;
    DataGridHeaderBorder.EnsureCache(19);
    if (flag1)
    {
      Matrix matrix1 = new Matrix();
      matrix1.RotateAt(-90.0, 0.0, 0.0);
      Matrix matrix2 = new Matrix();
      matrix2.Translate(0.0, renderSize.Height);
      MatrixTransform matrixTransform = new MatrixTransform(matrix1 * matrix2);
      matrixTransform.Freeze();
      dc.PushTransform((Transform) matrixTransform);
      double width = renderSize.Width;
      renderSize.Width = renderSize.Height;
      renderSize.Height = width;
    }
    if (flag5)
    {
      LinearGradientBrush linearGradientBrush = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable(0);
      if (linearGradientBrush == null)
      {
        linearGradientBrush = new LinearGradientBrush();
        linearGradientBrush.StartPoint = new Point();
        linearGradientBrush.EndPoint = new Point(0.0, 1.0);
        linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), 0.0));
        linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), 0.4));
        linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 252, (byte) 252, (byte) 253), 0.4));
        linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 251, (byte) 252, (byte) 252), 1.0));
        linearGradientBrush.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush, 0);
      }
      dc.DrawRectangle((Brush) linearGradientBrush, (Pen) null, new Rect(0.0, 0.0, renderSize.Width, renderSize.Height));
    }
    DataGridHeaderBorder.AeroFreezables index1 = DataGridHeaderBorder.AeroFreezables.NormalBackground;
    if (flag4)
      index1 = DataGridHeaderBorder.AeroFreezables.PressedBackground;
    else if (flag3)
      index1 = DataGridHeaderBorder.AeroFreezables.HoveredBackground;
    else if (hasValue || isSelected)
      index1 = DataGridHeaderBorder.AeroFreezables.SortedBackground;
    LinearGradientBrush linearGradientBrush1 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable((int) index1);
    if (linearGradientBrush1 == null)
    {
      linearGradientBrush1 = new LinearGradientBrush();
      linearGradientBrush1.StartPoint = new Point();
      linearGradientBrush1.EndPoint = new Point(0.0, 1.0);
      switch (index1)
      {
        case DataGridHeaderBorder.AeroFreezables.NormalBackground:
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 247, (byte) 248, (byte) 250), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 241, (byte) 242, (byte) 244), 1.0));
          break;
        case DataGridHeaderBorder.AeroFreezables.PressedBackground:
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 188, (byte) 228, (byte) 249), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 188, (byte) 228, (byte) 249), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 141, (byte) 214, (byte) 247), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 138, (byte) 209, (byte) 245), 1.0));
          break;
        case DataGridHeaderBorder.AeroFreezables.HoveredBackground:
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 227, (byte) 247, byte.MaxValue), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 227, (byte) 247, byte.MaxValue), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 189, (byte) 237, byte.MaxValue), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 183, (byte) 231, (byte) 251), 1.0));
          break;
        case DataGridHeaderBorder.AeroFreezables.SortedBackground:
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 242, (byte) 249, (byte) 252), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 242, (byte) 249, (byte) 252), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 225, (byte) 241, (byte) 249), 0.4));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 216, (byte) 236, (byte) 246), 1.0));
          break;
      }
      linearGradientBrush1.Freeze();
      DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush1, (int) index1);
    }
    dc.DrawRectangle((Brush) linearGradientBrush1, (Pen) null, new Rect(0.0, 0.0, renderSize.Width, renderSize.Height));
    if (renderSize.Width >= 2.0)
    {
      DataGridHeaderBorder.AeroFreezables index2 = DataGridHeaderBorder.AeroFreezables.NormalSides;
      if (flag4)
        index2 = DataGridHeaderBorder.AeroFreezables.PressedSides;
      else if (flag3)
        index2 = DataGridHeaderBorder.AeroFreezables.HoveredSides;
      else if (hasValue || isSelected)
        index2 = DataGridHeaderBorder.AeroFreezables.SortedSides;
      if (this.SeparatorVisibility == Visibility.Visible)
      {
        Brush brush;
        if (this.SeparatorBrush != null)
        {
          brush = this.SeparatorBrush;
        }
        else
        {
          brush = (Brush) DataGridHeaderBorder.GetCachedFreezable((int) index2);
          if (brush == null)
          {
            LinearGradientBrush linearGradientBrush2 = (LinearGradientBrush) null;
            if (index2 != DataGridHeaderBorder.AeroFreezables.SortedSides)
            {
              linearGradientBrush2 = new LinearGradientBrush();
              linearGradientBrush2.StartPoint = new Point();
              linearGradientBrush2.EndPoint = new Point(0.0, 1.0);
              brush = (Brush) linearGradientBrush2;
            }
            switch (index2 - 6)
            {
              case DataGridHeaderBorder.AeroFreezables.NormalBevel:
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 242, (byte) 242, (byte) 242), 0.0));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 239, (byte) 239, (byte) 239), 0.4));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 231, (byte) 232, (byte) 234), 0.4));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 222, (byte) 223, (byte) 225), 1.0));
                break;
              case DataGridHeaderBorder.AeroFreezables.NormalBackground:
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 122, (byte) 158, (byte) 177), 0.0));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 122, (byte) 158, (byte) 177), 0.4));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 80 /*0x50*/, (byte) 145, (byte) 175), 0.4));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 77, (byte) 141, (byte) 173), 1.0));
                break;
              case DataGridHeaderBorder.AeroFreezables.PressedBackground:
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 136, (byte) 203, (byte) 235), 0.0));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 136, (byte) 203, (byte) 235), 0.4));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 105, (byte) 187, (byte) 227), 0.4));
                linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 105, (byte) 187, (byte) 227), 1.0));
                break;
              case DataGridHeaderBorder.AeroFreezables.HoveredBackground:
                brush = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 150, (byte) 217, (byte) 249));
                break;
            }
            brush.Freeze();
            DataGridHeaderBorder.CacheFreezable((Freezable) brush, (int) index2);
          }
        }
        dc.DrawRectangle(brush, (Pen) null, new Rect(0.0, 0.0, 1.0, DataGridHeaderBorder.Max0(renderSize.Height - 0.95)));
        dc.DrawRectangle(brush, (Pen) null, new Rect(renderSize.Width - 1.0, 0.0, 1.0, DataGridHeaderBorder.Max0(renderSize.Height - 0.95)));
      }
    }
    if (flag4 && renderSize.Width >= 4.0 && renderSize.Height >= 4.0)
    {
      LinearGradientBrush linearGradientBrush3 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable(5);
      if (linearGradientBrush3 == null)
      {
        linearGradientBrush3 = new LinearGradientBrush();
        linearGradientBrush3.StartPoint = new Point();
        linearGradientBrush3.EndPoint = new Point(0.0, 1.0);
        linearGradientBrush3.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 134, (byte) 163, (byte) 178), 0.0));
        linearGradientBrush3.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 134, (byte) 163, (byte) 178), 0.1));
        linearGradientBrush3.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 170, (byte) 206, (byte) 225), 0.9));
        linearGradientBrush3.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 170, (byte) 206, (byte) 225), 1.0));
        linearGradientBrush3.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush3, 5);
      }
      dc.DrawRectangle((Brush) linearGradientBrush3, (Pen) null, new Rect(0.0, 0.0, renderSize.Width, 2.0));
      LinearGradientBrush linearGradientBrush4 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable(10);
      if (linearGradientBrush4 == null)
      {
        linearGradientBrush4 = new LinearGradientBrush();
        linearGradientBrush4.StartPoint = new Point();
        linearGradientBrush4.EndPoint = new Point(0.0, 1.0);
        linearGradientBrush4.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 162, (byte) 203, (byte) 224 /*0xE0*/), 0.0));
        linearGradientBrush4.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 162, (byte) 203, (byte) 224 /*0xE0*/), 0.4));
        linearGradientBrush4.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 114, (byte) 188, (byte) 223), 0.4));
        linearGradientBrush4.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 110, (byte) 184, (byte) 220), 1.0));
        linearGradientBrush4.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush4, 10);
      }
      dc.DrawRectangle((Brush) linearGradientBrush4, (Pen) null, new Rect(1.0, 0.0, 1.0, renderSize.Height - 0.95));
      dc.DrawRectangle((Brush) linearGradientBrush4, (Pen) null, new Rect(renderSize.Width - 2.0, 0.0, 1.0, renderSize.Height - 0.95));
    }
    if (renderSize.Height >= 2.0)
    {
      DataGridHeaderBorder.AeroFreezables index3 = DataGridHeaderBorder.AeroFreezables.NormalBottom;
      if (flag4)
        index3 = DataGridHeaderBorder.AeroFreezables.PressedOrHoveredBottom;
      else if (flag3)
        index3 = DataGridHeaderBorder.AeroFreezables.PressedOrHoveredBottom;
      else if (hasValue || isSelected)
        index3 = DataGridHeaderBorder.AeroFreezables.SortedBottom;
      SolidColorBrush solidColorBrush = (SolidColorBrush) DataGridHeaderBorder.GetCachedFreezable((int) index3);
      if (solidColorBrush == null)
      {
        switch (index3)
        {
          case DataGridHeaderBorder.AeroFreezables.NormalBottom:
            solidColorBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 213, (byte) 213, (byte) 213));
            break;
          case DataGridHeaderBorder.AeroFreezables.PressedOrHoveredBottom:
            solidColorBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 147, (byte) 201, (byte) 227));
            break;
          case DataGridHeaderBorder.AeroFreezables.SortedBottom:
            solidColorBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 150, (byte) 217, (byte) 249));
            break;
        }
        solidColorBrush.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) solidColorBrush, (int) index3);
      }
      dc.DrawRectangle((Brush) solidColorBrush, (Pen) null, new Rect(0.0, renderSize.Height - 1.0, renderSize.Width, 1.0));
    }
    if (hasValue && renderSize.Width > 14.0 && renderSize.Height > 10.0)
    {
      TranslateTransform translateTransform = new TranslateTransform((renderSize.Width - 8.0) * 0.5, 1.0);
      translateTransform.Freeze();
      dc.PushTransform((Transform) translateTransform);
      ListSortDirection? nullable = sortDirection;
      bool flag6 = nullable.GetValueOrDefault() == ListSortDirection.Ascending && nullable.HasValue;
      PathGeometry pathGeometry = (PathGeometry) DataGridHeaderBorder.GetCachedFreezable(flag6 ? 17 : 18);
      if (pathGeometry == null)
      {
        pathGeometry = new PathGeometry();
        PathFigure pathFigure = new PathFigure();
        if (flag6)
        {
          pathFigure.StartPoint = new Point(0.0, 4.0);
          LineSegment lineSegment1 = new LineSegment(new Point(4.0, 0.0), false);
          lineSegment1.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment1);
          LineSegment lineSegment2 = new LineSegment(new Point(8.0, 4.0), false);
          lineSegment2.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment2);
        }
        else
        {
          pathFigure.StartPoint = new Point(0.0, 0.0);
          LineSegment lineSegment3 = new LineSegment(new Point(8.0, 0.0), false);
          lineSegment3.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment3);
          LineSegment lineSegment4 = new LineSegment(new Point(4.0, 4.0), false);
          lineSegment4.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment4);
        }
        pathFigure.IsClosed = true;
        pathFigure.Freeze();
        pathGeometry.Figures.Add(pathFigure);
        pathGeometry.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) pathGeometry, flag6 ? 17 : 18);
      }
      LinearGradientBrush linearGradientBrush5 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable(14);
      if (linearGradientBrush5 == null)
      {
        linearGradientBrush5 = new LinearGradientBrush();
        linearGradientBrush5.StartPoint = new Point();
        linearGradientBrush5.EndPoint = new Point(1.0, 1.0);
        linearGradientBrush5.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 60, (byte) 94, (byte) 114), 0.0));
        linearGradientBrush5.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 60, (byte) 94, (byte) 114), 0.1));
        linearGradientBrush5.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 195, (byte) 228, (byte) 245), 1.0));
        linearGradientBrush5.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush5, 14);
      }
      dc.DrawGeometry((Brush) linearGradientBrush5, (Pen) null, (Geometry) pathGeometry);
      LinearGradientBrush linearGradientBrush6 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable(15);
      if (linearGradientBrush6 == null)
      {
        linearGradientBrush6 = new LinearGradientBrush();
        linearGradientBrush6.StartPoint = new Point();
        linearGradientBrush6.EndPoint = new Point(1.0, 1.0);
        linearGradientBrush6.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 97, (byte) 150, (byte) 182), 0.0));
        linearGradientBrush6.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 97, (byte) 150, (byte) 182), 0.1));
        linearGradientBrush6.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 202, (byte) 230, (byte) 245), 1.0));
        linearGradientBrush6.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush6, 15);
      }
      ScaleTransform scaleTransform = (ScaleTransform) DataGridHeaderBorder.GetCachedFreezable(16 /*0x10*/);
      if (scaleTransform == null)
      {
        scaleTransform = new ScaleTransform(0.75, 0.75, 3.5, 4.0);
        scaleTransform.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) scaleTransform, 16 /*0x10*/);
      }
      dc.PushTransform((Transform) scaleTransform);
      dc.DrawGeometry((Brush) linearGradientBrush6, (Pen) null, (Geometry) pathGeometry);
      dc.Pop();
      dc.Pop();
    }
    if (!flag1)
      return;
    dc.Pop();
  }

  private void RenderLuna(DrawingContext dc, DataGridHeaderBorder.Luna colorVariant)
  {
    Size renderSize = this.RenderSize;
    bool flag1 = this.Orientation == Orientation.Horizontal;
    bool flag2 = this.IsClickable && this.IsEnabled;
    bool flag3 = flag2 && this.IsHovered;
    bool flag4 = flag2 && this.IsPressed;
    ListSortDirection? sortDirection = this.SortDirection;
    bool hasValue = sortDirection.HasValue;
    bool isSelected = this.IsSelected;
    DataGridHeaderBorder.EnsureCache(12);
    if (flag1)
    {
      Matrix matrix1 = new Matrix();
      matrix1.RotateAt(-90.0, 0.0, 0.0);
      Matrix matrix2 = new Matrix();
      matrix2.Translate(0.0, renderSize.Height);
      MatrixTransform matrixTransform = new MatrixTransform(matrix1 * matrix2);
      matrixTransform.Freeze();
      dc.PushTransform((Transform) matrixTransform);
      double width = renderSize.Width;
      renderSize.Width = renderSize.Height;
      renderSize.Height = width;
    }
    DataGridHeaderBorder.LunaFreezables index = flag4 ? DataGridHeaderBorder.LunaFreezables.PressedBackground : (flag3 ? DataGridHeaderBorder.LunaFreezables.HoveredBackground : DataGridHeaderBorder.LunaFreezables.NormalBackground);
    LinearGradientBrush linearGradientBrush1 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable((int) index);
    if (linearGradientBrush1 == null)
    {
      linearGradientBrush1 = new LinearGradientBrush();
      linearGradientBrush1.StartPoint = new Point();
      linearGradientBrush1.EndPoint = new Point(0.0, 1.0);
      if (flag4)
      {
        if (colorVariant == DataGridHeaderBorder.Luna.Metallic)
        {
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 185, (byte) 185, (byte) 200), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 236, (byte) 236, (byte) 243), 0.1));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 236, (byte) 236, (byte) 243), 1.0));
        }
        else
        {
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 193, (byte) 194, (byte) 184), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 222, (byte) 223, (byte) 216), 0.1));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 222, (byte) 223, (byte) 216), 1.0));
        }
      }
      else if (flag3 || isSelected)
      {
        if (colorVariant == DataGridHeaderBorder.Luna.Metallic)
        {
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 254, (byte) 254, (byte) 254), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 254, (byte) 254, (byte) 254), 0.85));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 189, (byte) 190, (byte) 206), 1.0));
        }
        else
        {
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 250, (byte) 249, (byte) 244), 0.0));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 250, (byte) 249, (byte) 244), 0.85));
          linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 236, (byte) 233, (byte) 216), 1.0));
        }
      }
      else if (colorVariant == DataGridHeaderBorder.Luna.Metallic)
      {
        linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 249, (byte) 250, (byte) 253), 0.0));
        linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 249, (byte) 250, (byte) 253), 0.85));
        linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 189, (byte) 190, (byte) 206), 1.0));
      }
      else
      {
        linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 235, (byte) 234, (byte) 219), 0.0));
        linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 235, (byte) 234, (byte) 219), 0.85));
        linearGradientBrush1.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 203, (byte) 199, (byte) 184), 1.0));
      }
      linearGradientBrush1.Freeze();
      DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush1, (int) index);
    }
    dc.DrawRectangle((Brush) linearGradientBrush1, (Pen) null, new Rect(0.0, 0.0, renderSize.Width, renderSize.Height));
    if (flag3 && !flag4 && renderSize.Width >= 6.0 && renderSize.Height >= 4.0)
    {
      TranslateTransform translateTransform = new TranslateTransform(0.0, renderSize.Height - 3.0);
      translateTransform.Freeze();
      dc.PushTransform((Transform) translateTransform);
      PathGeometry pathGeometry = new PathGeometry();
      PathFigure pathFigure = new PathFigure();
      pathFigure.StartPoint = new Point(0.5, 0.5);
      LineSegment lineSegment1 = new LineSegment(new Point(renderSize.Width - 0.5, 0.5), true);
      lineSegment1.Freeze();
      pathFigure.Segments.Add((PathSegment) lineSegment1);
      ArcSegment arcSegment1 = new ArcSegment(new Point(renderSize.Width - 2.5, 2.5), new Size(2.0, 2.0), 90.0, false, SweepDirection.Clockwise, true);
      arcSegment1.Freeze();
      pathFigure.Segments.Add((PathSegment) arcSegment1);
      LineSegment lineSegment2 = new LineSegment(new Point(2.5, 2.5), true);
      lineSegment2.Freeze();
      pathFigure.Segments.Add((PathSegment) lineSegment2);
      ArcSegment arcSegment2 = new ArcSegment(new Point(0.5, 0.5), new Size(2.0, 2.0), 90.0, false, SweepDirection.Clockwise, true);
      arcSegment2.Freeze();
      pathFigure.Segments.Add((PathSegment) arcSegment2);
      pathFigure.IsClosed = true;
      pathFigure.Freeze();
      pathGeometry.Figures.Add(pathFigure);
      pathGeometry.Freeze();
      Pen pen = (Pen) DataGridHeaderBorder.GetCachedFreezable(7);
      if (pen == null)
      {
        SolidColorBrush solidColorBrush = new SolidColorBrush(colorVariant == DataGridHeaderBorder.Luna.HomeStead ? Color.FromArgb(byte.MaxValue, (byte) 207, (byte) 114, (byte) 37) : Color.FromArgb(byte.MaxValue, (byte) 248, (byte) 169, (byte) 0));
        solidColorBrush.Freeze();
        pen = new Pen((Brush) solidColorBrush, 1.0);
        pen.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) pen, 7);
      }
      LinearGradientBrush linearGradientBrush2 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable(8);
      if (linearGradientBrush2 == null)
      {
        linearGradientBrush2 = new LinearGradientBrush();
        linearGradientBrush2.StartPoint = new Point();
        linearGradientBrush2.EndPoint = new Point(1.0, 0.0);
        if (colorVariant == DataGridHeaderBorder.Luna.HomeStead)
        {
          linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 227, (byte) 145, (byte) 79), 0.0));
          linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 227, (byte) 145, (byte) 79), 1.0));
        }
        else
        {
          linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 252, (byte) 224 /*0xE0*/, (byte) 166), 0.0));
          linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 246, (byte) 196, (byte) 86), 0.1));
          linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 246, (byte) 196, (byte) 86), 0.9));
          linearGradientBrush2.GradientStops.Add(new GradientStop(Color.FromArgb(byte.MaxValue, (byte) 223, (byte) 151, (byte) 0), 1.0));
        }
        linearGradientBrush2.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush2, 8);
      }
      dc.DrawGeometry((Brush) linearGradientBrush2, pen, (Geometry) pathGeometry);
      dc.Pop();
    }
    if (flag4 && renderSize.Width >= 2.0 && renderSize.Height >= 2.0)
    {
      SolidColorBrush solidColorBrush = (SolidColorBrush) DataGridHeaderBorder.GetCachedFreezable(5);
      if (solidColorBrush == null)
      {
        solidColorBrush = new SolidColorBrush(colorVariant == DataGridHeaderBorder.Luna.Metallic ? Color.FromArgb(byte.MaxValue, (byte) 128 /*0x80*/, (byte) 128 /*0x80*/, (byte) 153) : Color.FromArgb(byte.MaxValue, (byte) 165, (byte) 165, (byte) 151));
        solidColorBrush.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) solidColorBrush, 5);
      }
      dc.DrawRectangle((Brush) solidColorBrush, (Pen) null, new Rect(0.0, 0.0, 1.0, renderSize.Height));
      dc.DrawRectangle((Brush) solidColorBrush, (Pen) null, new Rect(0.0, DataGridHeaderBorder.Max0(renderSize.Height - 1.0), renderSize.Width, 1.0));
    }
    if (!flag4 && !flag3 && renderSize.Width >= 4.0 && this.SeparatorVisibility == Visibility.Visible)
    {
      Brush brush;
      if (this.SeparatorBrush != null)
      {
        brush = this.SeparatorBrush;
      }
      else
      {
        LinearGradientBrush linearGradientBrush3 = (LinearGradientBrush) DataGridHeaderBorder.GetCachedFreezable(flag1 ? 3 : 4);
        if (linearGradientBrush3 == null)
        {
          linearGradientBrush3 = new LinearGradientBrush();
          linearGradientBrush3.StartPoint = new Point();
          linearGradientBrush3.EndPoint = new Point(1.0, 0.0);
          Color color1 = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
          Color color2 = Color.FromArgb(byte.MaxValue, (byte) 199, (byte) 197, (byte) 178);
          if (flag1)
          {
            linearGradientBrush3.GradientStops.Add(new GradientStop(color1, 0.0));
            linearGradientBrush3.GradientStops.Add(new GradientStop(color1, 0.25));
            linearGradientBrush3.GradientStops.Add(new GradientStop(color2, 0.75));
            linearGradientBrush3.GradientStops.Add(new GradientStop(color2, 1.0));
          }
          else
          {
            linearGradientBrush3.GradientStops.Add(new GradientStop(color2, 0.0));
            linearGradientBrush3.GradientStops.Add(new GradientStop(color2, 0.25));
            linearGradientBrush3.GradientStops.Add(new GradientStop(color1, 0.75));
            linearGradientBrush3.GradientStops.Add(new GradientStop(color1, 1.0));
          }
          linearGradientBrush3.Freeze();
          DataGridHeaderBorder.CacheFreezable((Freezable) linearGradientBrush3, flag1 ? 3 : 4);
        }
        brush = (Brush) linearGradientBrush3;
      }
      dc.DrawRectangle(brush, (Pen) null, new Rect(flag1 ? 0.0 : DataGridHeaderBorder.Max0(renderSize.Width - 2.0), 4.0, 2.0, DataGridHeaderBorder.Max0(renderSize.Height - 8.0)));
    }
    if (hasValue && renderSize.Width > 14.0 && renderSize.Height > 10.0)
    {
      TranslateTransform translateTransform = new TranslateTransform(renderSize.Width - 15.0, (renderSize.Height - 5.0) * 0.5);
      translateTransform.Freeze();
      dc.PushTransform((Transform) translateTransform);
      ListSortDirection? nullable = sortDirection;
      bool flag5 = nullable.GetValueOrDefault() == ListSortDirection.Ascending && nullable.HasValue;
      PathGeometry pathGeometry = (PathGeometry) DataGridHeaderBorder.GetCachedFreezable(flag5 ? 10 : 11);
      if (pathGeometry == null)
      {
        pathGeometry = new PathGeometry();
        PathFigure pathFigure = new PathFigure();
        if (flag5)
        {
          pathFigure.StartPoint = new Point(0.0, 5.0);
          LineSegment lineSegment3 = new LineSegment(new Point(5.0, 0.0), false);
          lineSegment3.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment3);
          LineSegment lineSegment4 = new LineSegment(new Point(10.0, 5.0), false);
          lineSegment4.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment4);
        }
        else
        {
          pathFigure.StartPoint = new Point(0.0, 0.0);
          LineSegment lineSegment5 = new LineSegment(new Point(10.0, 0.0), false);
          lineSegment5.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment5);
          LineSegment lineSegment6 = new LineSegment(new Point(5.0, 5.0), false);
          lineSegment6.Freeze();
          pathFigure.Segments.Add((PathSegment) lineSegment6);
        }
        pathFigure.IsClosed = true;
        pathFigure.Freeze();
        pathGeometry.Figures.Add(pathFigure);
        pathGeometry.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) pathGeometry, flag5 ? 10 : 11);
      }
      SolidColorBrush solidColorBrush = (SolidColorBrush) DataGridHeaderBorder.GetCachedFreezable(9);
      if (solidColorBrush == null)
      {
        solidColorBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 172, (byte) 168, (byte) 153));
        solidColorBrush.Freeze();
        DataGridHeaderBorder.CacheFreezable((Freezable) solidColorBrush, 9);
      }
      dc.DrawGeometry((Brush) solidColorBrush, (Pen) null, (Geometry) pathGeometry);
      dc.Pop();
    }
    if (!flag1)
      return;
    dc.Pop();
  }

  private Brush EnsureControlBrush()
  {
    if (this.ReadLocalValue(DataGridHeaderBorder.ControlBrushProperty) == DependencyProperty.UnsetValue)
      this.SetResourceReference(DataGridHeaderBorder.ControlBrushProperty, (object) SystemColors.ControlBrushKey);
    return (Brush) this.GetValue(DataGridHeaderBorder.ControlBrushProperty);
  }

  private void RenderClassic(DrawingContext dc)
  {
    Size renderSize = this.RenderSize;
    bool flag1 = this.IsClickable && this.IsEnabled && this.IsPressed;
    ListSortDirection? sortDirection = this.SortDirection;
    bool hasValue = sortDirection.HasValue;
    bool flag2 = this.Orientation == Orientation.Horizontal;
    Brush brush1 = this.EnsureControlBrush();
    Brush controlLightBrush = (Brush) SystemColors.ControlLightBrush;
    Brush controlDarkBrush = (Brush) SystemColors.ControlDarkBrush;
    bool flag3 = true;
    bool flag4 = true;
    bool flag5 = false;
    Brush brush2 = (Brush) null;
    if (!flag2)
    {
      if (this.SeparatorVisibility == Visibility.Visible && this.SeparatorBrush != null)
      {
        brush2 = this.SeparatorBrush;
        flag5 = true;
      }
      else
        flag3 = false;
    }
    else
      brush2 = (Brush) SystemColors.ControlDarkDarkBrush;
    Brush brush3 = (Brush) null;
    if (flag2)
    {
      if (this.SeparatorVisibility == Visibility.Visible && this.SeparatorBrush != null)
      {
        brush3 = this.SeparatorBrush;
        flag5 = true;
      }
      else
        flag4 = false;
    }
    else
      brush3 = (Brush) SystemColors.ControlDarkDarkBrush;
    DataGridHeaderBorder.EnsureCache(2);
    dc.DrawRectangle(brush1, (Pen) null, new Rect(0.0, 0.0, renderSize.Width, renderSize.Height));
    if (renderSize.Width > 3.0 && renderSize.Height > 3.0)
    {
      if (flag1)
      {
        dc.DrawRectangle(controlDarkBrush, (Pen) null, new Rect(0.0, 0.0, renderSize.Width, 1.0));
        dc.DrawRectangle(controlDarkBrush, (Pen) null, new Rect(0.0, 0.0, 1.0, renderSize.Height));
        dc.DrawRectangle(controlDarkBrush, (Pen) null, new Rect(0.0, DataGridHeaderBorder.Max0(renderSize.Height - 1.0), renderSize.Width, 1.0));
        dc.DrawRectangle(controlDarkBrush, (Pen) null, new Rect(DataGridHeaderBorder.Max0(renderSize.Width - 1.0), 0.0, 1.0, renderSize.Height));
      }
      else
      {
        dc.DrawRectangle(controlLightBrush, (Pen) null, new Rect(0.0, 0.0, 1.0, DataGridHeaderBorder.Max0(renderSize.Height - 1.0)));
        dc.DrawRectangle(controlLightBrush, (Pen) null, new Rect(0.0, 0.0, DataGridHeaderBorder.Max0(renderSize.Width - 1.0), 1.0));
        if (flag3)
        {
          if (!flag5)
            dc.DrawRectangle(controlDarkBrush, (Pen) null, new Rect(DataGridHeaderBorder.Max0(renderSize.Width - 2.0), 1.0, 1.0, DataGridHeaderBorder.Max0(renderSize.Height - 2.0)));
          dc.DrawRectangle(brush2, (Pen) null, new Rect(DataGridHeaderBorder.Max0(renderSize.Width - 1.0), 0.0, 1.0, renderSize.Height));
        }
        if (flag4)
        {
          if (!flag5)
            dc.DrawRectangle(controlDarkBrush, (Pen) null, new Rect(1.0, DataGridHeaderBorder.Max0(renderSize.Height - 2.0), DataGridHeaderBorder.Max0(renderSize.Width - 2.0), 1.0));
          dc.DrawRectangle(brush3, (Pen) null, new Rect(0.0, DataGridHeaderBorder.Max0(renderSize.Height - 1.0), renderSize.Width, 1.0));
        }
      }
    }
    if (!hasValue || renderSize.Width <= 14.0 || renderSize.Height <= 10.0)
      return;
    TranslateTransform translateTransform = new TranslateTransform(renderSize.Width - 15.0, (renderSize.Height - 5.0) * 0.5);
    translateTransform.Freeze();
    dc.PushTransform((Transform) translateTransform);
    ListSortDirection? nullable = sortDirection;
    bool flag6 = nullable.GetValueOrDefault() == ListSortDirection.Ascending && nullable.HasValue;
    PathGeometry pathGeometry = (PathGeometry) DataGridHeaderBorder.GetCachedFreezable(flag6 ? 0 : 1);
    if (pathGeometry == null)
    {
      pathGeometry = new PathGeometry();
      PathFigure pathFigure = new PathFigure();
      if (flag6)
      {
        pathFigure.StartPoint = new Point(0.0, 5.0);
        LineSegment lineSegment1 = new LineSegment(new Point(5.0, 0.0), false);
        lineSegment1.Freeze();
        pathFigure.Segments.Add((PathSegment) lineSegment1);
        LineSegment lineSegment2 = new LineSegment(new Point(10.0, 5.0), false);
        lineSegment2.Freeze();
        pathFigure.Segments.Add((PathSegment) lineSegment2);
      }
      else
      {
        pathFigure.StartPoint = new Point(0.0, 0.0);
        LineSegment lineSegment3 = new LineSegment(new Point(10.0, 0.0), false);
        lineSegment3.Freeze();
        pathFigure.Segments.Add((PathSegment) lineSegment3);
        LineSegment lineSegment4 = new LineSegment(new Point(5.0, 5.0), false);
        lineSegment4.Freeze();
        pathFigure.Segments.Add((PathSegment) lineSegment4);
      }
      pathFigure.IsClosed = true;
      pathFigure.Freeze();
      pathGeometry.Figures.Add(pathFigure);
      pathGeometry.Freeze();
      DataGridHeaderBorder.CacheFreezable((Freezable) pathGeometry, flag6 ? 0 : 1);
    }
    dc.DrawGeometry((Brush) SystemColors.GrayTextBrush, (Pen) null, (Geometry) pathGeometry);
    dc.Pop();
  }

  private static void EnsureCache(int size)
  {
    if (DataGridHeaderBorder._freezableCache != null)
      return;
    lock (DataGridHeaderBorder._cacheAccess)
    {
      if (DataGridHeaderBorder._freezableCache != null)
        return;
      DataGridHeaderBorder._freezableCache = new List<Freezable>(size);
      for (int index = 0; index < size; ++index)
        DataGridHeaderBorder._freezableCache.Add((Freezable) null);
    }
  }

  private static void ReleaseCache()
  {
    if (DataGridHeaderBorder._freezableCache == null)
      return;
    lock (DataGridHeaderBorder._cacheAccess)
      DataGridHeaderBorder._freezableCache = (List<Freezable>) null;
  }

  private static Freezable GetCachedFreezable(int index)
  {
    lock (DataGridHeaderBorder._cacheAccess)
      return DataGridHeaderBorder._freezableCache[index];
  }

  private static void CacheFreezable(Freezable freezable, int index)
  {
    lock (DataGridHeaderBorder._cacheAccess)
    {
      if (DataGridHeaderBorder._freezableCache[index] == null)
        return;
      DataGridHeaderBorder._freezableCache[index] = freezable;
    }
  }

  private enum AeroFreezables
  {
    NormalBevel,
    NormalBackground,
    PressedBackground,
    HoveredBackground,
    SortedBackground,
    PressedTop,
    NormalSides,
    PressedSides,
    HoveredSides,
    SortedSides,
    PressedBevel,
    NormalBottom,
    PressedOrHoveredBottom,
    SortedBottom,
    ArrowBorder,
    ArrowFill,
    ArrowFillScale,
    ArrowUpGeometry,
    ArrowDownGeometry,
    NumFreezables,
  }

  private enum Luna
  {
    NormalColor,
    HomeStead,
    Metallic,
  }

  private enum LunaFreezables
  {
    NormalBackground,
    HoveredBackground,
    PressedBackground,
    HorizontalGripper,
    VerticalGripper,
    PressedBorder,
    TabGeometry,
    TabStroke,
    TabFill,
    ArrowFill,
    ArrowUpGeometry,
    ArrowDownGeometry,
    NumFreezables,
  }

  private enum ClassicFreezables
  {
    ArrowUpGeometry,
    ArrowDownGeometry,
    NumFreezables,
  }
}
