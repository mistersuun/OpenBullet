// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.DateTimePicker
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_Calendar", Type = typeof (Calendar))]
[TemplatePart(Name = "PART_TimeUpDown", Type = typeof (TimePicker))]
public class DateTimePicker : DateTimePickerBase
{
  private const string PART_Calendar = "PART_Calendar";
  private const string PART_TimeUpDown = "PART_TimeUpDown";
  private Calendar _calendar;
  private TimePicker _timePicker;
  private DateTime? _calendarTemporaryDateTime;
  private DateTime? _calendarIntendedDateTime;
  public static readonly DependencyProperty AutoCloseCalendarProperty = DependencyProperty.Register(nameof (AutoCloseCalendar), typeof (bool), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty CalendarDisplayModeProperty = DependencyProperty.Register(nameof (CalendarDisplayMode), typeof (CalendarMode), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) CalendarMode.Month));
  public static readonly DependencyProperty CalendarWidthProperty = DependencyProperty.Register(nameof (CalendarWidth), typeof (double), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) 178.0));
  public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register(nameof (TimeFormat), typeof (DateTimeFormat), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) DateTimeFormat.ShortTime));
  public static readonly DependencyProperty TimeFormatStringProperty = DependencyProperty.Register(nameof (TimeFormatString), typeof (string), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) null), new ValidateValueCallback(DateTimePicker.IsTimeFormatStringValid));
  public static readonly DependencyProperty TimePickerAllowSpinProperty = DependencyProperty.Register(nameof (TimePickerAllowSpin), typeof (bool), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty TimePickerShowButtonSpinnerProperty = DependencyProperty.Register(nameof (TimePickerShowButtonSpinner), typeof (bool), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty TimePickerVisibilityProperty = DependencyProperty.Register(nameof (TimePickerVisibility), typeof (Visibility), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((object) Visibility.Visible));
  public static readonly DependencyProperty TimeWatermarkProperty = DependencyProperty.Register(nameof (TimeWatermark), typeof (object), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty TimeWatermarkTemplateProperty = DependencyProperty.Register(nameof (TimeWatermarkTemplate), typeof (DataTemplate), typeof (DateTimePicker), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

  public bool AutoCloseCalendar
  {
    get => (bool) this.GetValue(DateTimePicker.AutoCloseCalendarProperty);
    set => this.SetValue(DateTimePicker.AutoCloseCalendarProperty, (object) value);
  }

  public CalendarMode CalendarDisplayMode
  {
    get => (CalendarMode) this.GetValue(DateTimePicker.CalendarDisplayModeProperty);
    set => this.SetValue(DateTimePicker.CalendarDisplayModeProperty, (object) value);
  }

  public double CalendarWidth
  {
    get => (double) this.GetValue(DateTimePicker.CalendarWidthProperty);
    set => this.SetValue(DateTimePicker.CalendarWidthProperty, (object) value);
  }

  public DateTimeFormat TimeFormat
  {
    get => (DateTimeFormat) this.GetValue(DateTimePicker.TimeFormatProperty);
    set => this.SetValue(DateTimePicker.TimeFormatProperty, (object) value);
  }

  public string TimeFormatString
  {
    get => (string) this.GetValue(DateTimePicker.TimeFormatStringProperty);
    set => this.SetValue(DateTimePicker.TimeFormatStringProperty, (object) value);
  }

  private static bool IsTimeFormatStringValid(object value)
  {
    return DateTimeUpDown.IsFormatStringValid(value);
  }

  public bool TimePickerAllowSpin
  {
    get => (bool) this.GetValue(DateTimePicker.TimePickerAllowSpinProperty);
    set => this.SetValue(DateTimePicker.TimePickerAllowSpinProperty, (object) value);
  }

  public bool TimePickerShowButtonSpinner
  {
    get => (bool) this.GetValue(DateTimePicker.TimePickerShowButtonSpinnerProperty);
    set => this.SetValue(DateTimePicker.TimePickerShowButtonSpinnerProperty, (object) value);
  }

  public Visibility TimePickerVisibility
  {
    get => (Visibility) this.GetValue(DateTimePicker.TimePickerVisibilityProperty);
    set => this.SetValue(DateTimePicker.TimePickerVisibilityProperty, (object) value);
  }

  public object TimeWatermark
  {
    get => this.GetValue(DateTimePicker.TimeWatermarkProperty);
    set => this.SetValue(DateTimePicker.TimeWatermarkProperty, value);
  }

  public DataTemplate TimeWatermarkTemplate
  {
    get => (DataTemplate) this.GetValue(DateTimePicker.TimeWatermarkTemplateProperty);
    set => this.SetValue(DateTimePicker.TimeWatermarkTemplateProperty, (object) value);
  }

  static DateTimePicker()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DateTimePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DateTimePicker)));
    UpDownBase<DateTime?>.UpdateValueOnEnterKeyProperty.OverrideMetadata(typeof (DateTimePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._calendar != null)
      this._calendar.SelectedDatesChanged -= new EventHandler<SelectionChangedEventArgs>(this.Calendar_SelectedDatesChanged);
    this._calendar = this.GetTemplateChild("PART_Calendar") as Calendar;
    if (this._calendar != null)
    {
      this._calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(this.Calendar_SelectedDatesChanged);
      Calendar calendar1 = this._calendar;
      DateTime? nullable1 = this.Value;
      DateTime? nullable2 = nullable1 ?? new DateTime?();
      calendar1.SelectedDate = nullable2;
      Calendar calendar2 = this._calendar;
      nullable1 = this.Value;
      DateTime dateTime = nullable1 ?? this.ContextNow;
      calendar2.DisplayDate = dateTime;
      this.SetBlackOutDates();
    }
    if (this._timePicker != null)
      this._timePicker.ValueChanged -= new RoutedPropertyChangedEventHandler<object>(this.TimePicker_ValueChanged);
    this._timePicker = this.GetTemplateChild("PART_TimeUpDown") as TimePicker;
    if (this._timePicker == null)
      return;
    this._timePicker.ValueChanged += new RoutedPropertyChangedEventHandler<object>(this.TimePicker_ValueChanged);
  }

  protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
  {
    if (Mouse.Captured is CalendarItem)
    {
      Mouse.Capture((IInputElement) null);
      if (this.AutoCloseCalendar && this._calendar != null && this._calendar.DisplayMode == CalendarMode.Month)
        this.ClosePopup(true);
    }
    base.OnPreviewMouseUp(e);
  }

  protected override void OnValueChanged(DateTime? oldValue, DateTime? newValue)
  {
    DateTime? nullable1 = newValue.HasValue ? new DateTime?(newValue.Value.Date) : new DateTime?();
    DateTime? nullable2;
    DateTime? nullable3;
    if (this._calendar != null)
    {
      nullable2 = this._calendar.SelectedDate;
      nullable3 = nullable1;
      if ((nullable2.HasValue == nullable3.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable3.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        this._calendar.SelectedDate = nullable1;
        this._calendar.DisplayDate = newValue ?? this.ContextNow;
      }
    }
    if (this._calendar != null && this._calendarTemporaryDateTime.HasValue)
    {
      nullable3 = newValue;
      nullable2 = this._calendarTemporaryDateTime;
      if ((nullable3.HasValue == nullable2.HasValue ? (nullable3.HasValue ? (nullable3.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        this._calendarTemporaryDateTime = new DateTime?();
        this._calendarIntendedDateTime = new DateTime?();
      }
    }
    base.OnValueChanged(oldValue, newValue);
  }

  protected override void OnIsOpenChanged(bool oldValue, bool newValue)
  {
    base.OnIsOpenChanged(oldValue, newValue);
    if (newValue)
      return;
    this._calendarTemporaryDateTime = new DateTime?();
    this._calendarIntendedDateTime = new DateTime?();
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if (this.IsOpen)
      return;
    base.OnPreviewKeyDown(e);
  }

  protected override void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
  {
    base.OnMaximumChanged(oldValue, newValue);
    this.SetBlackOutDates();
  }

  protected override void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
  {
    base.OnMinimumChanged(oldValue, newValue);
    this.SetBlackOutDates();
  }

  protected override void HandleKeyDown(object sender, KeyEventArgs e)
  {
    if (this.IsOpen && this._timePicker != null && this._timePicker.IsKeyboardFocusWithin && (this._timePicker.IsOpen || e.Handled))
      return;
    base.HandleKeyDown(sender, e);
  }

  private void TimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
  {
    e.Handled = true;
    if (!this.UpdateValueOnEnterKey)
      return;
    DateTime? newValue = e.NewValue as DateTime?;
    if (!newValue.HasValue)
      return;
    this._fireSelectionChangedEvent = false;
    DateTime dateTime1 = this.ConvertTextToValue(this.TextBox.Text) ?? this.ContextNow;
    DateTime dateTime2;
    ref DateTime local = ref dateTime2;
    int year = dateTime1.Year;
    int month = dateTime1.Month;
    int day = dateTime1.Day;
    int hour = newValue.Value.Hour;
    DateTime dateTime3 = newValue.Value;
    int minute = dateTime3.Minute;
    dateTime3 = newValue.Value;
    int second = dateTime3.Second;
    dateTime3 = newValue.Value;
    int millisecond = dateTime3.Millisecond;
    int kind = (int) dateTime1.Kind;
    local = new DateTime(year, month, day, hour, minute, second, millisecond, (DateTimeKind) kind);
    this.TextBox.Text = dateTime2.ToString(this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo);
    this._fireSelectionChangedEvent = true;
  }

  private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count <= 0)
      return;
    DateTime? objA = (DateTime?) e.AddedItems[0];
    DateTime dateTime1;
    if (objA.HasValue)
    {
      objA = new DateTime?(DateTime.SpecifyKind(objA.Value, this.Kind));
      if (this._calendarIntendedDateTime.HasValue)
      {
        ref DateTime? local = ref objA;
        dateTime1 = objA.Value;
        DateTime date = dateTime1.Date;
        dateTime1 = this._calendarIntendedDateTime.Value;
        TimeSpan timeOfDay = dateTime1.TimeOfDay;
        DateTime dateTime2 = date + timeOfDay;
        local = new DateTime?(dateTime2);
        this._calendarTemporaryDateTime = new DateTime?();
        this._calendarIntendedDateTime = new DateTime?();
      }
      else if (this.Value.HasValue)
      {
        ref DateTime? local = ref objA;
        dateTime1 = objA.Value;
        DateTime date = dateTime1.Date;
        dateTime1 = this.Value.Value;
        TimeSpan timeOfDay = dateTime1.TimeOfDay;
        DateTime dateTime3 = date + timeOfDay;
        local = new DateTime?(dateTime3);
      }
      DateTime? clippedMinMaxValue = this.GetClippedMinMaxValue(objA);
      if (clippedMinMaxValue.Value != objA.Value)
      {
        this._calendarTemporaryDateTime = clippedMinMaxValue;
        this._calendarIntendedDateTime = objA;
        objA = clippedMinMaxValue;
      }
    }
    if (this.UpdateValueOnEnterKey)
    {
      this._fireSelectionChangedEvent = false;
      TextBox textBox = this.TextBox;
      dateTime1 = objA.Value;
      string str = dateTime1.ToString(this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo);
      textBox.Text = str;
      this._fireSelectionChangedEvent = true;
    }
    else
    {
      if (object.Equals((object) objA, (object) this.Value))
        return;
      this.Value = objA;
    }
  }

  protected override void Popup_Opened(object sender, EventArgs e)
  {
    base.Popup_Opened(sender, e);
    if (this._calendar == null)
      return;
    this._calendar.Focus();
  }

  private void SetBlackOutDates()
  {
    if (this._calendar == null)
      return;
    this._calendar.BlackoutDates.Clear();
    DateTime? nullable;
    if (this.Minimum.HasValue)
    {
      nullable = this.Minimum;
      if (nullable.HasValue)
      {
        nullable = this.Minimum;
        if (nullable.Value != DateTime.MinValue)
        {
          nullable = this.Minimum;
          DateTime dateTime = nullable.Value;
          this._calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, dateTime.AddDays(-1.0)));
        }
      }
    }
    nullable = this.Maximum;
    if (!nullable.HasValue)
      return;
    nullable = this.Maximum;
    if (!nullable.HasValue)
      return;
    nullable = this.Maximum;
    if (!(nullable.Value != DateTime.MaxValue))
      return;
    nullable = this.Maximum;
    this._calendar.BlackoutDates.Add(new CalendarDateRange(nullable.Value.AddDays(1.0), DateTime.MaxValue));
  }
}
