// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.TimePicker
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_TimeListItems", Type = typeof (ListBox))]
public class TimePicker : DateTimePickerBase
{
  private const string PART_TimeListItems = "PART_TimeListItems";
  private ListBox _timeListBox;
  private bool _isListBoxInvalid = true;
  internal static readonly TimeSpan EndTimeDefaultValue = new TimeSpan(23, 59, 0);
  internal static readonly TimeSpan StartTimeDefaultValue = new TimeSpan(0, 0, 0);
  internal static readonly TimeSpan TimeIntervalDefaultValue = new TimeSpan(1, 0, 0);
  public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register(nameof (EndTime), typeof (TimeSpan), typeof (TimePicker), (PropertyMetadata) new UIPropertyMetadata((object) TimePicker.EndTimeDefaultValue, new PropertyChangedCallback(TimePicker.OnEndTimeChanged), new CoerceValueCallback(TimePicker.OnCoerceEndTime)));
  public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof (MaxDropDownHeight), typeof (double), typeof (TimePicker), (PropertyMetadata) new UIPropertyMetadata((object) 130.0, new PropertyChangedCallback(TimePicker.OnMaxDropDownHeightChanged)));
  public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register(nameof (StartTime), typeof (TimeSpan), typeof (TimePicker), (PropertyMetadata) new UIPropertyMetadata((object) TimePicker.StartTimeDefaultValue, new PropertyChangedCallback(TimePicker.OnStartTimeChanged), new CoerceValueCallback(TimePicker.OnCoerceStartTime)));
  public static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register(nameof (TimeInterval), typeof (TimeSpan), typeof (TimePicker), (PropertyMetadata) new UIPropertyMetadata((object) TimePicker.TimeIntervalDefaultValue, new PropertyChangedCallback(TimePicker.OnTimeIntervalChanged)));

  private static object OnCoerceEndTime(DependencyObject o, object value)
  {
    return o is TimePicker timePicker ? (object) timePicker.OnCoerceEndTime((TimeSpan) value) : value;
  }

  private static void OnEndTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is TimePicker timePicker))
      return;
    timePicker.OnEndTimeChanged((TimeSpan) e.OldValue, (TimeSpan) e.NewValue);
  }

  protected virtual TimeSpan OnCoerceEndTime(TimeSpan value)
  {
    this.ValidateTime(value);
    return value;
  }

  protected virtual void OnEndTimeChanged(TimeSpan oldValue, TimeSpan newValue)
  {
    this.InvalidateListBoxItems();
  }

  public TimeSpan EndTime
  {
    get => (TimeSpan) this.GetValue(TimePicker.EndTimeProperty);
    set => this.SetValue(TimePicker.EndTimeProperty, (object) value);
  }

  protected override void OnFormatChanged(DateTimeFormat oldValue, DateTimeFormat newValue)
  {
    base.OnFormatChanged(oldValue, newValue);
    this.InvalidateListBoxItems();
  }

  public double MaxDropDownHeight
  {
    get => (double) this.GetValue(TimePicker.MaxDropDownHeightProperty);
    set => this.SetValue(TimePicker.MaxDropDownHeightProperty, (object) value);
  }

  private static void OnMaxDropDownHeightChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is TimePicker timePicker))
      return;
    timePicker.OnMaxDropDownHeightChanged((double) e.OldValue, (double) e.NewValue);
  }

  protected virtual void OnMaxDropDownHeightChanged(double oldValue, double newValue)
  {
  }

  private static object OnCoerceStartTime(DependencyObject o, object value)
  {
    return o is TimePicker timePicker ? (object) timePicker.OnCoerceStartTime((TimeSpan) value) : value;
  }

  private static void OnStartTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is TimePicker timePicker))
      return;
    timePicker.OnStartTimeChanged((TimeSpan) e.OldValue, (TimeSpan) e.NewValue);
  }

  protected virtual TimeSpan OnCoerceStartTime(TimeSpan value)
  {
    this.ValidateTime(value);
    return value;
  }

  protected virtual void OnStartTimeChanged(TimeSpan oldValue, TimeSpan newValue)
  {
    this.InvalidateListBoxItems();
  }

  public TimeSpan StartTime
  {
    get => (TimeSpan) this.GetValue(TimePicker.StartTimeProperty);
    set => this.SetValue(TimePicker.StartTimeProperty, (object) value);
  }

  public TimeSpan TimeInterval
  {
    get => (TimeSpan) this.GetValue(TimePicker.TimeIntervalProperty);
    set => this.SetValue(TimePicker.TimeIntervalProperty, (object) value);
  }

  private static object OnCoerceTimeInterval(DependencyObject o, object value)
  {
    return o is TimePicker timePicker ? (object) timePicker.OnCoerceTimeInterval((TimeSpan) value) : value;
  }

  private static void OnTimeIntervalChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is TimePicker timePicker))
      return;
    timePicker.OnTimeIntervalChanged((TimeSpan) e.OldValue, (TimeSpan) e.NewValue);
  }

  protected virtual TimeSpan OnCoerceTimeInterval(TimeSpan value)
  {
    this.ValidateTime(value);
    return value.Ticks != 0L ? value : throw new ArgumentException("TimeInterval must be greater than zero");
  }

  protected virtual void OnTimeIntervalChanged(TimeSpan oldValue, TimeSpan newValue)
  {
    this.InvalidateListBoxItems();
  }

  static TimePicker()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TimePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (TimePicker)));
    DateTimeUpDown.FormatProperty.OverrideMetadata(typeof (TimePicker), (PropertyMetadata) new UIPropertyMetadata((object) DateTimeFormat.ShortTime));
    UpDownBase<DateTime?>.UpdateValueOnEnterKeyProperty.OverrideMetadata(typeof (TimePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  }

  protected override void OnFormatStringChanged(string oldValue, string newValue)
  {
    if (this.Format == DateTimeFormat.Custom)
      this.InvalidateListBoxItems();
    base.OnFormatStringChanged(oldValue, newValue);
  }

  protected override void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
  {
    base.OnMaximumChanged(oldValue, newValue);
    this.InvalidateListBoxItems();
  }

  protected override void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
  {
    base.OnMinimumChanged(oldValue, newValue);
    this.InvalidateListBoxItems();
  }

  protected override void OnValueChanged(DateTime? oldValue, DateTime? newValue)
  {
    base.OnValueChanged(oldValue, newValue);
    bool flag = false;
    if (DateTimeUtilities.IsSameDate(this.Minimum, oldValue) != DateTimeUtilities.IsSameDate(this.Minimum, newValue))
      flag = true;
    if (DateTimeUtilities.IsSameDate(this.Maximum, oldValue) != DateTimeUtilities.IsSameDate(this.Maximum, newValue))
      flag = true;
    DateTime valueOrDefault = oldValue.GetValueOrDefault();
    DateTime date1 = valueOrDefault.Date;
    valueOrDefault = newValue.GetValueOrDefault();
    DateTime date2 = valueOrDefault.Date;
    if (date1 != date2)
      flag = true;
    if (flag)
      this.InvalidateListBoxItems();
    else
      this.UpdateListBoxSelectedItem();
  }

  protected override void Popup_Opened(object sender, EventArgs e)
  {
    base.Popup_Opened(sender, e);
    if (this._timeListBox == null)
      return;
    this.UpdateListBoxItems();
    TimeItem nearestTimeItem = this.GetNearestTimeItem(this.Value.HasValue ? this.Value.Value.TimeOfDay : TimePicker.StartTimeDefaultValue);
    if (nearestTimeItem != null)
    {
      this._timeListBox.ScrollIntoView((object) nearestTimeItem);
      this.UpdateListBoxSelectedItem();
    }
    this._timeListBox.Focus();
  }

  public override void OnApplyTemplate()
  {
    if (this.TextBox != null)
      this.TextBox.GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.TextBoxSpinner_GotKeyboardFocus);
    if (this.Spinner != null)
      this.Spinner.GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.TextBoxSpinner_GotKeyboardFocus);
    base.OnApplyTemplate();
    if (this.TextBox != null)
      this.TextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TextBoxSpinner_GotKeyboardFocus);
    if (this.Spinner != null)
      this.Spinner.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TextBoxSpinner_GotKeyboardFocus);
    if (this._timeListBox != null)
    {
      this._timeListBox.SelectionChanged -= new SelectionChangedEventHandler(this.TimeListBox_SelectionChanged);
      this._timeListBox.MouseUp -= new MouseButtonEventHandler(this.TimeListBox_MouseUp);
    }
    this._timeListBox = this.GetTemplateChild("PART_TimeListItems") as ListBox;
    if (this._timeListBox == null)
      return;
    this._timeListBox.SelectionChanged += new SelectionChangedEventHandler(this.TimeListBox_SelectionChanged);
    this._timeListBox.MouseUp += new MouseButtonEventHandler(this.TimeListBox_MouseUp);
    this.InvalidateListBoxItems();
  }

  private void TimeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count <= 0)
      return;
    TimeSpan time = ((TimeItem) e.AddedItems[0]).Time;
    if (this.UpdateValueOnEnterKey)
    {
      DateTime dateTime = this.ConvertTextToValue(this.TextBox.Text) ?? this.ContextNow;
      this.TextBox.Text = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, time.Hours, time.Minutes, time.Seconds, time.Milliseconds, dateTime.Kind).ToString(this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo);
    }
    else
    {
      DateTime dateTime = this.Value ?? this.ContextNow;
      this.Value = new DateTime?(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, time.Hours, time.Minutes, time.Seconds, time.Milliseconds, dateTime.Kind));
    }
  }

  private void TextBoxSpinner_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
  {
    this.ClosePopup(true);
  }

  private void TimeListBox_MouseUp(object sender, MouseButtonEventArgs e) => this.ClosePopup(true);

  private void ValidateTime(TimeSpan time)
  {
    if (time.TotalHours >= 24.0)
      throw new ArgumentException("Time value cannot be greater than or equal to 24 hours.");
  }

  public IEnumerable GenerateTimeListItemsSource()
  {
    TimeSpan time = this.StartTime;
    TimeSpan timeSpan = this.EndTime;
    if (timeSpan <= time)
    {
      timeSpan = TimePicker.EndTimeDefaultValue;
      time = TimePicker.StartTimeDefaultValue;
    }
    if (this.Value.HasValue)
    {
      DateTime dateTime1 = this.Value.Value;
      DateTime? nullable = this.Minimum;
      DateTime dateTime2 = nullable ?? DateTime.MinValue;
      nullable = this.Maximum;
      DateTime dateTime3 = nullable ?? DateTime.MaxValue;
      TimeSpan timeOfDay1 = dateTime2.TimeOfDay;
      TimeSpan timeOfDay2 = dateTime3.TimeOfDay;
      if (dateTime1.Date == dateTime2.Date && time.Ticks < timeOfDay1.Ticks)
        time = timeOfDay1;
      if (dateTime1.Date == dateTime3.Date && timeSpan.Ticks > timeOfDay2.Ticks)
        timeSpan = timeOfDay2;
      if (timeSpan < time)
        time = timeSpan;
    }
    TimeSpan timeInterval = this.TimeInterval;
    List<TimeItem> timeListItemsSource = new List<TimeItem>();
    if (timeInterval.Ticks > 0L)
    {
      for (; time <= timeSpan; time = time.Add(timeInterval))
        timeListItemsSource.Add(this.CreateTimeItem(time));
    }
    return (IEnumerable) timeListItemsSource;
  }

  protected virtual TimeItem CreateTimeItem(TimeSpan time)
  {
    DateTime dateTime = this.Value ?? this.ContextNow;
    string formatString = this.GetFormatString(this.Format);
    return new TimeItem(dateTime.Date.Add(time).ToString(formatString, (IFormatProvider) this.CultureInfo), time);
  }

  private void UpdateListBoxSelectedItem()
  {
    if (this._timeListBox == null)
      return;
    TimeItem timeItem = (TimeItem) null;
    if (this.Value.HasValue)
    {
      timeItem = this.CreateTimeItem(this.Value.Value.TimeOfDay);
      if (!this._timeListBox.Items.Contains((object) timeItem))
        timeItem = (TimeItem) null;
    }
    this._timeListBox.SelectedItem = (object) timeItem;
  }

  private void InvalidateListBoxItems()
  {
    this._isListBoxInvalid = true;
    if (!this.IsOpen)
      return;
    this.UpdateListBoxItems();
  }

  private void UpdateListBoxItems()
  {
    if (this._timeListBox == null || !this._isListBoxInvalid)
      return;
    this._timeListBox.ItemsSource = this.GenerateTimeListItemsSource();
    this.UpdateListBoxSelectedItem();
    this._isListBoxInvalid = false;
  }

  private TimeItem GetNearestTimeItem(TimeSpan time)
  {
    if (this._timeListBox != null)
    {
      int count = this._timeListBox.Items.Count;
      for (int index = 0; index < count; ++index)
      {
        if (this._timeListBox.Items[index] is TimeItem nearestTimeItem && nearestTimeItem.Time >= time)
          return nearestTimeItem;
      }
      if (count > 0)
        return this._timeListBox.Items[count - 1] as TimeItem;
    }
    return (TimeItem) null;
  }
}
