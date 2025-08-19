// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.TimelinePanel
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class TimelinePanel : Panel, IScrollInfo
{
  private List<DateElement> _visibleElements;
  public static readonly DependencyProperty BeginDateProperty = DependencyProperty.Register(nameof (BeginDate), typeof (DateTime), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.MinValue, FrameworkPropertyMetadataOptions.AffectsMeasure));
  public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(nameof (EndDate), typeof (DateTime), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.MinValue, FrameworkPropertyMetadataOptions.AffectsMeasure));
  public static readonly DependencyProperty OverlapBehaviorProperty = DependencyProperty.Register(nameof (OverlapBehavior), typeof (OverlapBehavior), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) OverlapBehavior.Stack, FrameworkPropertyMetadataOptions.AffectsMeasure));
  public static readonly DependencyProperty KeepOriginalOrderForOverlapProperty = DependencyProperty.Register(nameof (KeepOriginalOrderForOverlap), typeof (bool), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.AffectsMeasure));
  public static readonly DependencyProperty OrientationProperty = StackPanel.OrientationProperty.AddOwner(typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));
  public static readonly DependencyProperty UnitTimeSpanProperty = DependencyProperty.Register(nameof (UnitTimeSpan), typeof (TimeSpan), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) TimeSpan.Zero, FrameworkPropertyMetadataOptions.AffectsMeasure));
  public static readonly DependencyProperty UnitSizeProperty = DependencyProperty.Register(nameof (UnitSize), typeof (double), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
  public static readonly DependencyProperty DateProperty = DependencyProperty.RegisterAttached("Date", typeof (DateTime), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.MinValue, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
  public static readonly DependencyProperty DateEndProperty = DependencyProperty.RegisterAttached("DateEnd", typeof (DateTime), typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.MinValue, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
  private bool _allowHorizontal;
  private bool _allowVertical;
  private Vector _computedOffset = new Vector(0.0, 0.0);
  private Size _extent = new Size(0.0, 0.0);
  private Vector _offset = new Vector(0.0, 0.0);
  private ScrollViewer _scrollOwner;
  private Size _viewport;
  private Size _physicalViewport;

  public DateTime BeginDate
  {
    get => (DateTime) this.GetValue(TimelinePanel.BeginDateProperty);
    set => this.SetValue(TimelinePanel.BeginDateProperty, (object) value);
  }

  public DateTime EndDate
  {
    get => (DateTime) this.GetValue(TimelinePanel.EndDateProperty);
    set => this.SetValue(TimelinePanel.EndDateProperty, (object) value);
  }

  public OverlapBehavior OverlapBehavior
  {
    get => (OverlapBehavior) this.GetValue(TimelinePanel.OverlapBehaviorProperty);
    set => this.SetValue(TimelinePanel.OverlapBehaviorProperty, (object) value);
  }

  public bool KeepOriginalOrderForOverlap
  {
    get => (bool) this.GetValue(TimelinePanel.KeepOriginalOrderForOverlapProperty);
    set => this.SetValue(TimelinePanel.KeepOriginalOrderForOverlapProperty, (object) value);
  }

  public Orientation Orientation
  {
    get => (Orientation) this.GetValue(TimelinePanel.OrientationProperty);
    set => this.SetValue(TimelinePanel.OrientationProperty, (object) value);
  }

  public TimeSpan UnitTimeSpan
  {
    get => (TimeSpan) this.GetValue(TimelinePanel.UnitTimeSpanProperty);
    set => this.SetValue(TimelinePanel.UnitTimeSpanProperty, (object) value);
  }

  public double UnitSize
  {
    get => (double) this.GetValue(TimelinePanel.UnitSizeProperty);
    set => this.SetValue(TimelinePanel.UnitSizeProperty, (object) value);
  }

  public static DateTime GetDate(DependencyObject obj)
  {
    return (DateTime) obj.GetValue(TimelinePanel.DateProperty);
  }

  public static void SetDate(DependencyObject obj, DateTime value)
  {
    obj.SetValue(TimelinePanel.DateProperty, (object) value);
  }

  public static DateTime GetDateEnd(DependencyObject obj)
  {
    return (DateTime) obj.GetValue(TimelinePanel.DateEndProperty);
  }

  public static void SetDateEnd(DependencyObject obj, DateTime value)
  {
    obj.SetValue(TimelinePanel.DateEndProperty, (object) value);
  }

  public List<DateElement> VisibleElements => this._visibleElements;

  static TimelinePanel()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TimelinePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (TimelinePanel)));
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    DateTime dateTime1 = DateTime.MaxValue;
    DateTime dateTime2 = DateTime.MinValue;
    foreach (UIElement internalChild in this.InternalChildren)
    {
      DateTime date = TimelinePanel.GetDate((DependencyObject) internalChild);
      DateTime dateEnd = TimelinePanel.GetDateEnd((DependencyObject) internalChild);
      if (date < dateTime1)
        dateTime1 = date;
      if (date > dateTime2)
        dateTime2 = date;
      if (dateEnd > dateTime2)
        dateTime2 = dateEnd;
    }
    if (this.BeginDate == DateTime.MinValue)
      this.BeginDate = dateTime1;
    if (this.EndDate == DateTime.MinValue)
      this.EndDate = dateTime2;
    foreach (UIElement internalChild in this.InternalChildren)
    {
      DateTime date = TimelinePanel.GetDate((DependencyObject) internalChild);
      DateTime dateEnd = TimelinePanel.GetDateEnd((DependencyObject) internalChild);
      Size availableSize1 = availableSize;
      if (dateEnd > DateTime.MinValue && dateEnd > date)
      {
        if (this.Orientation == Orientation.Horizontal)
        {
          if (this.UnitTimeSpan != TimeSpan.Zero && this.UnitSize > 0.0)
          {
            double num = (double) (dateEnd.Ticks - date.Ticks) / (double) this.UnitTimeSpan.Ticks;
            availableSize1.Width = num * this.UnitSize;
          }
          else if (!double.IsPositiveInfinity(availableSize.Width))
            availableSize1.Width = this.CalculateTimelineOffset(dateEnd, availableSize.Width) - this.CalculateTimelineOffset(date, availableSize.Width);
        }
        else if (this.UnitTimeSpan != TimeSpan.Zero && this.UnitSize > 0.0)
        {
          double num = (double) (dateEnd.Ticks - date.Ticks) / (double) this.UnitTimeSpan.Ticks;
          availableSize1.Height = num * this.UnitSize;
        }
        else if (!double.IsPositiveInfinity(availableSize.Height))
          availableSize1.Height = this.CalculateTimelineOffset(dateEnd, availableSize.Height) - this.CalculateTimelineOffset(date, availableSize.Height);
      }
      internalChild.Measure(availableSize1);
    }
    Size availableSize2 = new Size(availableSize.Width, availableSize.Height);
    if (this.UnitTimeSpan != TimeSpan.Zero && this.UnitSize > 0.0)
    {
      DateTime dateTime3 = this.EndDate;
      long ticks1 = dateTime3.Ticks;
      dateTime3 = this.BeginDate;
      long ticks2 = dateTime3.Ticks;
      double num = (double) (ticks1 - ticks2) / (double) this.UnitTimeSpan.Ticks;
      if (this.Orientation == Orientation.Horizontal)
        availableSize2.Width = num * this.UnitSize;
      else
        availableSize2.Height = num * this.UnitSize;
    }
    Size size = new Size();
    if (this.Orientation == Orientation.Vertical && double.IsPositiveInfinity(availableSize2.Height) || this.Orientation == Orientation.Horizontal && double.IsPositiveInfinity(availableSize2.Width))
    {
      this._visibleElements = (List<DateElement>) null;
    }
    else
    {
      this.LayoutItems(this.InternalChildren, availableSize2);
      Rect rect = new Rect();
      foreach (DateElement visibleElement in this._visibleElements)
        rect.Union(visibleElement.PlacementRectangle);
      if (this.Orientation == Orientation.Horizontal)
      {
        size.Width = availableSize2.Width;
        size.Height = rect.Size.Height;
      }
      else
      {
        size.Width = rect.Size.Width;
        size.Height = availableSize2.Height;
      }
    }
    if (this.IsScrolling)
    {
      Size viewport = new Size(availableSize.Width, availableSize.Height);
      Size extent = new Size(size.Width, size.Height);
      Vector offset = new Vector(Math.Max(0.0, Math.Min(this._offset.X, extent.Width - viewport.Width)), Math.Max(0.0, Math.Min(this._offset.Y, extent.Height - viewport.Height)));
      this.SetScrollingData(viewport, extent, offset);
      size.Width = Math.Min(size.Width, availableSize.Width);
      size.Height = Math.Min(size.Height, availableSize.Height);
      this._physicalViewport = availableSize;
    }
    return size;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    Rect rect = new Rect();
    if (this._visibleElements == null)
      this.LayoutItems(this.InternalChildren, finalSize);
    foreach (DateElement visibleElement in this._visibleElements)
    {
      if (this.IsScrolling)
      {
        Rect finalRect = new Rect(visibleElement.PlacementRectangle.Location, visibleElement.PlacementRectangle.Size);
        finalRect.Offset(-this._offset);
        visibleElement.Element.Arrange(finalRect);
      }
      else
        visibleElement.Element.Arrange(visibleElement.PlacementRectangle);
      rect.Union(visibleElement.PlacementRectangle);
    }
    return this.Orientation != Orientation.Horizontal ? new Size(rect.Size.Width, finalSize.Height) : new Size(finalSize.Width, rect.Size.Height);
  }

  private void ResetScrollInfo()
  {
    this._offset = new Vector();
    this._physicalViewport = this._viewport = this._extent = new Size(0.0, 0.0);
  }

  private void SetScrollingData(Size viewport, Size extent, Vector offset)
  {
    this._offset = offset;
    if (TimelinePanel.AreVirtuallyEqual(viewport, this._viewport) && TimelinePanel.AreVirtuallyEqual(extent, this._extent) && TimelinePanel.AreVirtuallyEqual(offset, this._computedOffset))
      return;
    this._viewport = viewport;
    this._extent = extent;
    this._offset = offset;
    this.OnScrollChange();
  }

  private double ValidateInputOffset(double offset, string parameterName)
  {
    return !double.IsNaN(offset) ? Math.Max(0.0, offset) : throw new ArgumentOutOfRangeException(parameterName);
  }

  private void OnScrollChange()
  {
    if (this.ScrollOwner == null)
      return;
    this.ScrollOwner.InvalidateScrollInfo();
  }

  private double CalculateTimelineOffset(DateTime d, double finalWidth)
  {
    DateTime dateTime = this.EndDate;
    long ticks1 = dateTime.Ticks;
    dateTime = this.BeginDate;
    long ticks2 = dateTime.Ticks;
    long num1 = ticks1 - ticks2;
    long num2 = d.Ticks - this.BeginDate.Ticks;
    return !(this.UnitTimeSpan != TimeSpan.Zero) || this.UnitSize <= 0.0 ? (num1 <= 0L ? 0.0 : (double) num2 / (double) num1 * finalWidth) : (double) num2 / (double) this.UnitTimeSpan.Ticks * this.UnitSize;
  }

  private static int CompareElementsByLeft(DateElement a, DateElement b)
  {
    return a.PlacementRectangle.Left.CompareTo(b.PlacementRectangle.Left);
  }

  private static int CompareElementsByTop(DateElement a, DateElement b)
  {
    return a.PlacementRectangle.Top.CompareTo(b.PlacementRectangle.Top);
  }

  private void LayoutItems(UIElementCollection children, Size availableSize)
  {
    this._visibleElements = new List<DateElement>();
    List<DateElement> dateElementList = new List<DateElement>();
    int originalIndex = 0;
    foreach (UIElement child in children)
    {
      if (child != null)
      {
        DateTime date = TimelinePanel.GetDate((DependencyObject) child);
        DateTime dateEnd = TimelinePanel.GetDateEnd((DependencyObject) child);
        if (child.Visibility != Visibility.Collapsed)
        {
          if (this.KeepOriginalOrderForOverlap)
            this._visibleElements.Add(new DateElement(child, date, dateEnd, originalIndex));
          else
            this._visibleElements.Add(new DateElement(child, date, dateEnd));
        }
        ++originalIndex;
      }
    }
    this._visibleElements.Sort();
    foreach (DateElement visibleElement1 in this._visibleElements)
    {
      DateTime date = TimelinePanel.GetDate((DependencyObject) visibleElement1.Element);
      DateTime dateEnd = TimelinePanel.GetDateEnd((DependencyObject) visibleElement1.Element);
      Size desiredSize;
      if (this.Orientation == Orientation.Vertical)
      {
        visibleElement1.PlacementRectangle.Y = this.CalculateTimelineOffset(date, availableSize.Height);
        if (dateEnd > DateTime.MinValue && dateEnd > date)
        {
          visibleElement1.PlacementRectangle.Height = this.CalculateTimelineOffset(dateEnd, availableSize.Height) - this.CalculateTimelineOffset(date, availableSize.Height);
        }
        else
        {
          ref Rect local = ref visibleElement1.PlacementRectangle;
          desiredSize = visibleElement1.Element.DesiredSize;
          double height = desiredSize.Height;
          local.Height = height;
        }
        switch (this.OverlapBehavior)
        {
          case OverlapBehavior.Stack:
            dateElementList.Clear();
            foreach (DateElement visibleElement2 in this._visibleElements)
            {
              if (visibleElement1 != visibleElement2)
              {
                Rect placementRectangle1 = visibleElement1.PlacementRectangle;
                Rect placementRectangle2 = visibleElement2.PlacementRectangle;
                if (placementRectangle1.Top >= placementRectangle2.Top && placementRectangle1.Top < placementRectangle2.Bottom)
                  dateElementList.Add(visibleElement2);
              }
              else
                break;
            }
            dateElementList.Sort(new Comparison<DateElement>(TimelinePanel.CompareElementsByLeft));
            double num1 = 0.0;
            visibleElement1.PlacementRectangle.Width = visibleElement1.Element.DesiredSize.Width;
            for (int index = 0; index < dateElementList.Count; ++index)
            {
              Rect placementRectangle = dateElementList[index].PlacementRectangle;
              if (index == 0 && placementRectangle.Left >= visibleElement1.PlacementRectangle.Width)
              {
                num1 = 0.0;
                break;
              }
              if (index == dateElementList.Count - 1)
              {
                num1 = placementRectangle.Right;
                break;
              }
              if (dateElementList[index + 1].PlacementRectangle.Left - placementRectangle.Right >= visibleElement1.PlacementRectangle.Width)
              {
                num1 = placementRectangle.Right;
                break;
              }
            }
            visibleElement1.PlacementRectangle.X = num1;
            continue;
          case OverlapBehavior.Stretch:
            dateElementList.Clear();
            foreach (DateElement visibleElement3 in this._visibleElements)
            {
              if (visibleElement1 != visibleElement3)
              {
                Rect placementRectangle3 = visibleElement1.PlacementRectangle;
                Rect placementRectangle4 = visibleElement3.PlacementRectangle;
                if (placementRectangle3.Top >= placementRectangle4.Top && placementRectangle3.Top < placementRectangle4.Bottom)
                  dateElementList.Add(visibleElement3);
              }
              else
                break;
            }
            dateElementList.Sort(new Comparison<DateElement>(TimelinePanel.CompareElementsByLeft));
            double num2 = 0.0;
            double d = availableSize.Width;
            if (dateElementList.Count > 0)
            {
              bool flag = false;
              for (int index = 0; index < dateElementList.Count; ++index)
              {
                Rect placementRectangle5 = dateElementList[index].PlacementRectangle;
                if (index == 0 && placementRectangle5.Left > 0.0)
                {
                  num2 = 0.0;
                  d = placementRectangle5.Left;
                  flag = true;
                  break;
                }
                if (index != dateElementList.Count - 1)
                {
                  Rect placementRectangle6 = dateElementList[index + 1].PlacementRectangle;
                  if (placementRectangle6.Left - placementRectangle5.Right > 0.0)
                  {
                    num2 = placementRectangle5.Right;
                    d = placementRectangle6.Left - placementRectangle5.Right;
                    flag = true;
                    break;
                  }
                }
                else
                  break;
              }
              if (!flag)
              {
                d = Math.Min(availableSize.Width / (double) (dateElementList.Count + 1), dateElementList[0].PlacementRectangle.Width);
                num2 = 0.0;
                foreach (DateElement dateElement in dateElementList)
                {
                  dateElement.PlacementRectangle.Width = d;
                  dateElement.PlacementRectangle.X = num2;
                  num2 += d;
                }
              }
            }
            visibleElement1.PlacementRectangle.X = num2;
            if (double.IsPositiveInfinity(d))
            {
              ref Rect local = ref visibleElement1.PlacementRectangle;
              desiredSize = visibleElement1.Element.DesiredSize;
              double width = desiredSize.Width;
              local.Width = width;
              continue;
            }
            visibleElement1.PlacementRectangle.Width = d;
            continue;
          case OverlapBehavior.Hide:
            dateElementList.Clear();
            foreach (DateElement visibleElement4 in this._visibleElements)
            {
              if (visibleElement1 != visibleElement4)
              {
                Rect placementRectangle7 = visibleElement1.PlacementRectangle;
                Rect placementRectangle8 = visibleElement4.PlacementRectangle;
                if (placementRectangle7.Top >= placementRectangle8.Top && placementRectangle7.Top < placementRectangle8.Bottom)
                  dateElementList.Add(visibleElement4);
              }
              else
                break;
            }
            if (dateElementList.Count > 0)
            {
              visibleElement1.PlacementRectangle.X = 0.0;
              visibleElement1.PlacementRectangle.Y = 0.0;
              visibleElement1.PlacementRectangle.Width = 0.0;
              visibleElement1.PlacementRectangle.Height = 0.0;
              continue;
            }
            visibleElement1.PlacementRectangle.X = 0.0;
            ref Rect local1 = ref visibleElement1.PlacementRectangle;
            desiredSize = visibleElement1.Element.DesiredSize;
            double width1 = desiredSize.Width;
            local1.Width = width1;
            continue;
          case OverlapBehavior.None:
            visibleElement1.PlacementRectangle.X = 0.0;
            ref Rect local2 = ref visibleElement1.PlacementRectangle;
            desiredSize = visibleElement1.Element.DesiredSize;
            double width2 = desiredSize.Width;
            local2.Width = width2;
            continue;
          default:
            continue;
        }
      }
      else
      {
        visibleElement1.PlacementRectangle.X = this.CalculateTimelineOffset(date, availableSize.Width);
        if (dateEnd > DateTime.MinValue && dateEnd > date)
        {
          visibleElement1.PlacementRectangle.Width = this.CalculateTimelineOffset(dateEnd, availableSize.Width) - this.CalculateTimelineOffset(date, availableSize.Width);
        }
        else
        {
          ref Rect local = ref visibleElement1.PlacementRectangle;
          desiredSize = visibleElement1.Element.DesiredSize;
          double width = desiredSize.Width;
          local.Width = width;
        }
        switch (this.OverlapBehavior)
        {
          case OverlapBehavior.Stack:
            dateElementList.Clear();
            foreach (DateElement visibleElement5 in this._visibleElements)
            {
              if (visibleElement1 != visibleElement5)
              {
                Rect placementRectangle9 = visibleElement1.PlacementRectangle;
                Rect placementRectangle10 = visibleElement5.PlacementRectangle;
                if (placementRectangle9.Left >= placementRectangle10.Left && placementRectangle9.Left < placementRectangle10.Right)
                  dateElementList.Add(visibleElement5);
              }
              else
                break;
            }
            dateElementList.Sort(new Comparison<DateElement>(TimelinePanel.CompareElementsByTop));
            double num3 = 0.0;
            visibleElement1.PlacementRectangle.Height = visibleElement1.Element.DesiredSize.Height;
            for (int index = 0; index < dateElementList.Count; ++index)
            {
              Rect placementRectangle = dateElementList[index].PlacementRectangle;
              if (index == 0 && placementRectangle.Top >= visibleElement1.PlacementRectangle.Height)
              {
                num3 = 0.0;
                break;
              }
              if (index == dateElementList.Count - 1)
              {
                num3 = placementRectangle.Bottom;
                break;
              }
              if (dateElementList[index + 1].PlacementRectangle.Top - placementRectangle.Bottom >= visibleElement1.PlacementRectangle.Height)
              {
                num3 = placementRectangle.Bottom;
                break;
              }
            }
            visibleElement1.PlacementRectangle.Y = num3;
            continue;
          case OverlapBehavior.Stretch:
            dateElementList.Clear();
            foreach (DateElement visibleElement6 in this._visibleElements)
            {
              if (visibleElement1 != visibleElement6)
              {
                Rect placementRectangle11 = visibleElement1.PlacementRectangle;
                Rect placementRectangle12 = visibleElement6.PlacementRectangle;
                if (placementRectangle11.Left >= placementRectangle12.Left && placementRectangle11.Left < placementRectangle12.Right)
                  dateElementList.Add(visibleElement6);
              }
              else
                break;
            }
            dateElementList.Sort(new Comparison<DateElement>(TimelinePanel.CompareElementsByTop));
            double num4 = 0.0;
            double d = availableSize.Height;
            if (dateElementList.Count > 0)
            {
              bool flag = false;
              for (int index = 0; index < dateElementList.Count; ++index)
              {
                Rect placementRectangle13 = dateElementList[index].PlacementRectangle;
                if (index == 0 && placementRectangle13.Top > 0.0)
                {
                  num4 = 0.0;
                  d = placementRectangle13.Top;
                  flag = true;
                  break;
                }
                if (index != dateElementList.Count - 1)
                {
                  Rect placementRectangle14 = dateElementList[index + 1].PlacementRectangle;
                  if (placementRectangle14.Top - placementRectangle13.Bottom > 0.0)
                  {
                    num4 = placementRectangle13.Bottom;
                    d = placementRectangle14.Top - placementRectangle13.Bottom;
                    flag = true;
                    break;
                  }
                }
                else
                  break;
              }
              if (!flag)
              {
                d = Math.Min(availableSize.Height / (double) (dateElementList.Count + 1), dateElementList[0].PlacementRectangle.Height);
                num4 = 0.0;
                foreach (DateElement dateElement in dateElementList)
                {
                  dateElement.PlacementRectangle.Height = d;
                  dateElement.PlacementRectangle.Y = num4;
                  num4 += d;
                }
              }
            }
            visibleElement1.PlacementRectangle.Y = num4;
            if (double.IsPositiveInfinity(d))
            {
              ref Rect local = ref visibleElement1.PlacementRectangle;
              desiredSize = visibleElement1.Element.DesiredSize;
              double height = desiredSize.Height;
              local.Height = height;
              continue;
            }
            visibleElement1.PlacementRectangle.Height = d;
            continue;
          case OverlapBehavior.Hide:
            dateElementList.Clear();
            foreach (DateElement visibleElement7 in this._visibleElements)
            {
              if (visibleElement1 != visibleElement7)
              {
                Rect placementRectangle15 = visibleElement1.PlacementRectangle;
                Rect placementRectangle16 = visibleElement7.PlacementRectangle;
                if (placementRectangle15.Left >= placementRectangle16.Left && placementRectangle15.Left < placementRectangle16.Right)
                  dateElementList.Add(visibleElement7);
              }
              else
                break;
            }
            if (dateElementList.Count > 0)
            {
              visibleElement1.PlacementRectangle.X = 0.0;
              visibleElement1.PlacementRectangle.Y = 0.0;
              visibleElement1.PlacementRectangle.Width = 0.0;
              visibleElement1.PlacementRectangle.Height = 0.0;
              continue;
            }
            visibleElement1.PlacementRectangle.Y = 0.0;
            ref Rect local3 = ref visibleElement1.PlacementRectangle;
            desiredSize = visibleElement1.Element.DesiredSize;
            double height1 = desiredSize.Height;
            local3.Height = height1;
            continue;
          case OverlapBehavior.None:
            visibleElement1.PlacementRectangle.Y = 0.0;
            ref Rect local4 = ref visibleElement1.PlacementRectangle;
            desiredSize = visibleElement1.Element.DesiredSize;
            double height2 = desiredSize.Height;
            local4.Height = height2;
            continue;
          default:
            continue;
        }
      }
    }
  }

  private static bool AreVirtuallyEqual(double d1, double d2)
  {
    if (double.IsPositiveInfinity(d1))
      return double.IsPositiveInfinity(d2);
    if (double.IsNegativeInfinity(d1))
      return double.IsNegativeInfinity(d2);
    if (double.IsNaN(d1))
      return double.IsNaN(d2);
    double num1 = d1 - d2;
    double num2 = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * 1E-15;
    return -num2 < num1 && num2 > num1;
  }

  private static bool AreVirtuallyEqual(Size s1, Size s2)
  {
    return TimelinePanel.AreVirtuallyEqual(s1.Width, s2.Width) && TimelinePanel.AreVirtuallyEqual(s1.Height, s2.Height);
  }

  private static bool AreVirtuallyEqual(Vector v1, Vector v2)
  {
    return TimelinePanel.AreVirtuallyEqual(v1.X, v2.X) && TimelinePanel.AreVirtuallyEqual(v1.Y, v2.Y);
  }

  public bool CanHorizontallyScroll
  {
    get => this._allowHorizontal;
    set => this._allowHorizontal = value;
  }

  public bool CanVerticallyScroll
  {
    get => this._allowVertical;
    set => this._allowVertical = value;
  }

  public double ExtentHeight => this._extent.Height;

  public double ExtentWidth => this._extent.Width;

  public double HorizontalOffset => this._offset.X;

  public void LineDown()
  {
    this.SetVerticalOffset(this.VerticalOffset + (this.Orientation == Orientation.Vertical ? 1.0 : 16.0));
  }

  public void LineLeft()
  {
    this.SetHorizontalOffset(this.HorizontalOffset - (this.Orientation == Orientation.Horizontal ? 1.0 : 16.0));
  }

  public void LineRight()
  {
    this.SetHorizontalOffset(this.HorizontalOffset + (this.Orientation == Orientation.Horizontal ? 1.0 : 16.0));
  }

  public void LineUp()
  {
    this.SetVerticalOffset(this.VerticalOffset - (this.Orientation == Orientation.Vertical ? 1.0 : 16.0));
  }

  public Rect MakeVisible(Visual visual, Rect rectangle) => rectangle;

  public void MouseWheelDown()
  {
    this.SetVerticalOffset(this.VerticalOffset + (double) SystemParameters.WheelScrollLines * (this.Orientation == Orientation.Vertical ? 1.0 : 16.0));
  }

  public void MouseWheelLeft()
  {
    this.SetHorizontalOffset(this.HorizontalOffset - 3.0 * (this.Orientation == Orientation.Horizontal ? 1.0 : 16.0));
  }

  public void MouseWheelRight()
  {
    this.SetHorizontalOffset(this.HorizontalOffset + 3.0 * (this.Orientation == Orientation.Horizontal ? 1.0 : 16.0));
  }

  public void MouseWheelUp()
  {
    this.SetVerticalOffset(this.VerticalOffset - (double) SystemParameters.WheelScrollLines * (this.Orientation == Orientation.Vertical ? 1.0 : 16.0));
  }

  public void PageDown() => this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);

  public void PageLeft() => this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);

  public void PageRight() => this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);

  public void PageUp() => this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);

  public ScrollViewer ScrollOwner
  {
    get => this._scrollOwner;
    set
    {
      if (this._scrollOwner == value)
        return;
      this._scrollOwner = value;
      this.ResetScrollInfo();
    }
  }

  public void SetHorizontalOffset(double offset)
  {
    offset = this.ValidateInputOffset(offset, "HorizontalOffset");
    if (TimelinePanel.AreVirtuallyEqual(offset, this._offset.X))
      return;
    this._offset.X = offset;
    this.InvalidateMeasure();
  }

  public void SetVerticalOffset(double offset)
  {
    offset = this.ValidateInputOffset(offset, "VerticalOffset");
    if (TimelinePanel.AreVirtuallyEqual(offset, this._offset.Y))
      return;
    this._offset.Y = offset;
    this.InvalidateMeasure();
  }

  public double VerticalOffset => this._offset.Y;

  public double ViewportHeight => this._viewport.Height;

  public double ViewportWidth => this._viewport.Width;

  private bool IsScrolling => this._scrollOwner != null;
}
