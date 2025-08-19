// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Calendar
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls.Primitives;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls;

[TemplatePart(Name = "PART_CalendarItem", Type = typeof (CalendarItem))]
[TemplatePart(Name = "PART_Root", Type = typeof (Panel))]
public class Calendar : Control
{
  private const string ElementRoot = "PART_Root";
  private const string ElementMonth = "PART_CalendarItem";
  private const int COLS = 7;
  private const int ROWS = 7;
  private const int YEAR_ROWS = 3;
  private const int YEAR_COLS = 4;
  private const int YEARS_PER_DECADE = 10;
  private DateTime? _hoverStart;
  private DateTime? _hoverEnd;
  private bool _isShiftPressed;
  private DateTime? _currentDate;
  private CalendarItem _monthControl;
  private CalendarBlackoutDatesCollection _blackoutDates;
  private SelectedDatesCollection _selectedDates;
  public static readonly RoutedEvent SelectedDatesChangedEvent = EventManager.RegisterRoutedEvent("SelectedDatesChanged", RoutingStrategy.Direct, typeof (EventHandler<SelectionChangedEventArgs>), typeof (Calendar));
  public static readonly DependencyProperty CalendarButtonStyleProperty = DependencyProperty.Register(nameof (CalendarButtonStyle), typeof (Style), typeof (Calendar));
  public static readonly DependencyProperty CalendarDayButtonStyleProperty = DependencyProperty.Register(nameof (CalendarDayButtonStyle), typeof (Style), typeof (Calendar));
  public static readonly DependencyProperty CalendarItemStyleProperty = DependencyProperty.Register(nameof (CalendarItemStyle), typeof (Style), typeof (Calendar));
  public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof (DisplayDate), typeof (DateTime), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Calendar.OnDisplayDateChanged), new CoerceValueCallback(Calendar.CoerceDisplayDate)));
  public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(nameof (DisplayDateEnd), typeof (DateTime?), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Calendar.OnDisplayDateEndChanged), new CoerceValueCallback(Calendar.CoerceDisplayDateEnd)));
  public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(nameof (DisplayDateStart), typeof (DateTime?), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Calendar.OnDisplayDateStartChanged), new CoerceValueCallback(Calendar.CoerceDisplayDateStart)));
  public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof (DisplayMode), typeof (CalendarMode), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) CalendarMode.Month, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Calendar.OnDisplayModePropertyChanged)), new ValidateValueCallback(Calendar.IsValidDisplayMode));
  public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(nameof (FirstDayOfWeek), typeof (DayOfWeek), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek, new PropertyChangedCallback(Calendar.OnFirstDayOfWeekChanged)), new ValidateValueCallback(Calendar.IsValidFirstDayOfWeek));
  public static readonly DependencyProperty IsTodayHighlightedProperty = DependencyProperty.Register(nameof (IsTodayHighlighted), typeof (bool), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, new PropertyChangedCallback(Calendar.OnIsTodayHighlightedChanged)));
  public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof (SelectedDate), typeof (DateTime?), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(Calendar.OnSelectedDateChanged)));
  public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof (SelectionMode), typeof (CalendarSelectionMode), typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) CalendarSelectionMode.SingleDate, new PropertyChangedCallback(Calendar.OnSelectionModeChanged)), new ValidateValueCallback(Calendar.IsValidSelectionMode));

  public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged
  {
    add => this.AddHandler(Calendar.SelectedDatesChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(Calendar.SelectedDatesChangedEvent, (Delegate) value);
  }

  public event EventHandler<CalendarDateChangedEventArgs> DisplayDateChanged;

  public event EventHandler<CalendarModeChangedEventArgs> DisplayModeChanged;

  public event EventHandler<EventArgs> SelectionModeChanged;

  static Calendar()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (Calendar)));
    KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Once));
    KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Contained));
    EventManager.RegisterClassHandler(typeof (Calendar), UIElement.GotFocusEvent, (Delegate) new RoutedEventHandler(Calendar.OnGotFocus));
    FrameworkElement.LanguageProperty.OverrideMetadata(typeof (Calendar), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(Calendar.OnLanguageChanged)));
  }

  public Calendar()
  {
    this._blackoutDates = new CalendarBlackoutDatesCollection(this);
    this._selectedDates = new SelectedDatesCollection(this);
    this.DisplayDate = DateTime.Today;
  }

  public CalendarBlackoutDatesCollection BlackoutDates => this._blackoutDates;

  public Style CalendarButtonStyle
  {
    get => (Style) this.GetValue(Calendar.CalendarButtonStyleProperty);
    set => this.SetValue(Calendar.CalendarButtonStyleProperty, (object) value);
  }

  public Style CalendarDayButtonStyle
  {
    get => (Style) this.GetValue(Calendar.CalendarDayButtonStyleProperty);
    set => this.SetValue(Calendar.CalendarDayButtonStyleProperty, (object) value);
  }

  public Style CalendarItemStyle
  {
    get => (Style) this.GetValue(Calendar.CalendarItemStyleProperty);
    set => this.SetValue(Calendar.CalendarItemStyleProperty, (object) value);
  }

  public DateTime DisplayDate
  {
    get => (DateTime) this.GetValue(Calendar.DisplayDateProperty);
    set => this.SetValue(Calendar.DisplayDateProperty, (object) value);
  }

  private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    Calendar calendar = d as Calendar;
    calendar.DisplayDateInternal = DateTimeHelper.DiscardDayTime((DateTime) e.NewValue);
    calendar.UpdateCellItems();
    calendar.OnDisplayDateChanged(new CalendarDateChangedEventArgs(new DateTime?((DateTime) e.OldValue), new DateTime?((DateTime) e.NewValue)));
  }

  private static object CoerceDisplayDate(DependencyObject d, object value)
  {
    Calendar calendar = d as Calendar;
    DateTime dateTime = (DateTime) value;
    if (calendar.DisplayDateStart.HasValue && dateTime < calendar.DisplayDateStart.Value)
      value = (object) calendar.DisplayDateStart.Value;
    else if (calendar.DisplayDateEnd.HasValue && dateTime > calendar.DisplayDateEnd.Value)
      value = (object) calendar.DisplayDateEnd.Value;
    return value;
  }

  public DateTime? DisplayDateEnd
  {
    get => (DateTime?) this.GetValue(Calendar.DisplayDateEndProperty);
    set => this.SetValue(Calendar.DisplayDateEndProperty, (object) value);
  }

  private static void OnDisplayDateEndChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Calendar calendar = d as Calendar;
    calendar.CoerceValue(Calendar.DisplayDateProperty);
    calendar.UpdateCellItems();
  }

  private static object CoerceDisplayDateEnd(DependencyObject d, object value)
  {
    Calendar calendar = d as Calendar;
    DateTime? nullable = (DateTime?) value;
    if (nullable.HasValue)
    {
      if (calendar.DisplayDateStart.HasValue && nullable.Value < calendar.DisplayDateStart.Value)
        value = (object) calendar.DisplayDateStart;
      DateTime? maximumDate = calendar.SelectedDates.MaximumDate;
      if (maximumDate.HasValue && nullable.Value < maximumDate.Value)
        value = (object) maximumDate;
    }
    return value;
  }

  public DateTime? DisplayDateStart
  {
    get => (DateTime?) this.GetValue(Calendar.DisplayDateStartProperty);
    set => this.SetValue(Calendar.DisplayDateStartProperty, (object) value);
  }

  private static void OnDisplayDateStartChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Calendar calendar = d as Calendar;
    calendar.CoerceValue(Calendar.DisplayDateEndProperty);
    calendar.CoerceValue(Calendar.DisplayDateProperty);
    calendar.UpdateCellItems();
  }

  private static object CoerceDisplayDateStart(DependencyObject d, object value)
  {
    Calendar calendar = d as Calendar;
    DateTime? nullable = (DateTime?) value;
    if (nullable.HasValue)
    {
      DateTime? minimumDate = calendar.SelectedDates.MinimumDate;
      if (minimumDate.HasValue && nullable.Value > minimumDate.Value)
        value = (object) minimumDate;
    }
    return value;
  }

  public CalendarMode DisplayMode
  {
    get => (CalendarMode) this.GetValue(Calendar.DisplayModeProperty);
    set => this.SetValue(Calendar.DisplayModeProperty, (object) value);
  }

  private static void OnDisplayModePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Calendar calendar1 = d as Calendar;
    CalendarMode newValue = (CalendarMode) e.NewValue;
    CalendarMode oldValue = (CalendarMode) e.OldValue;
    CalendarItem monthControl = calendar1.MonthControl;
    switch (newValue)
    {
      case CalendarMode.Month:
        if (oldValue == CalendarMode.Year || oldValue == CalendarMode.Decade)
        {
          Calendar calendar2 = calendar1;
          Calendar calendar3 = calendar1;
          DateTime? nullable1 = new DateTime?();
          DateTime? nullable2;
          DateTime? nullable3 = nullable2 = nullable1;
          calendar3.HoverEnd = nullable2;
          DateTime? nullable4 = nullable3;
          calendar2.HoverStart = nullable4;
          calendar1.CurrentDate = calendar1.DisplayDate;
        }
        calendar1.UpdateCellItems();
        break;
      case CalendarMode.Year:
      case CalendarMode.Decade:
        if (oldValue == CalendarMode.Month)
          calendar1.DisplayDate = calendar1.CurrentDate;
        calendar1.UpdateCellItems();
        break;
    }
    calendar1.OnDisplayModeChanged(new CalendarModeChangedEventArgs((CalendarMode) e.OldValue, newValue));
  }

  public DayOfWeek FirstDayOfWeek
  {
    get => (DayOfWeek) this.GetValue(Calendar.FirstDayOfWeekProperty);
    set => this.SetValue(Calendar.FirstDayOfWeekProperty, (object) value);
  }

  private static void OnFirstDayOfWeekChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    (d as Calendar).UpdateCellItems();
  }

  public bool IsTodayHighlighted
  {
    get => (bool) this.GetValue(Calendar.IsTodayHighlightedProperty);
    set => this.SetValue(Calendar.IsTodayHighlightedProperty, (object) value);
  }

  private static void OnIsTodayHighlightedChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Calendar calendar = d as Calendar;
    int num = DateTimeHelper.CompareYearMonth(calendar.DisplayDateInternal, DateTime.Today);
    if (num <= -2 || num >= 2)
      return;
    calendar.UpdateCellItems();
  }

  private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    Calendar calendar = d as Calendar;
    if (DependencyPropertyHelper.GetValueSource(d, Calendar.FirstDayOfWeekProperty).BaseValueSource != BaseValueSource.Default)
      return;
    calendar.CoerceValue(Calendar.FirstDayOfWeekProperty);
    calendar.UpdateCellItems();
  }

  public DateTime? SelectedDate
  {
    get => (DateTime?) this.GetValue(Calendar.SelectedDateProperty);
    set => this.SetValue(Calendar.SelectedDateProperty, (object) value);
  }

  private static void OnSelectedDateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Calendar cal = d as Calendar;
    if (cal.SelectionMode == CalendarSelectionMode.None && e.NewValue != null)
      throw new InvalidOperationException(SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidOperation));
    DateTime? newValue = (DateTime?) e.NewValue;
    if (!Calendar.IsValidDateSelection(cal, (object) newValue))
      throw new ArgumentOutOfRangeException(nameof (d), SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidValue));
    if (!newValue.HasValue)
      cal.SelectedDates.ClearInternal(true);
    else if (newValue.HasValue && (cal.SelectedDates.Count <= 0 || !(cal.SelectedDates[0] == newValue.Value)))
    {
      cal.SelectedDates.ClearInternal();
      cal.SelectedDates.Add(newValue.Value);
    }
    if (cal.SelectionMode != CalendarSelectionMode.SingleDate)
      return;
    if (newValue.HasValue)
      cal.CurrentDate = newValue.Value;
    cal.UpdateCellItems();
  }

  public SelectedDatesCollection SelectedDates => this._selectedDates;

  public CalendarSelectionMode SelectionMode
  {
    get => (CalendarSelectionMode) this.GetValue(Calendar.SelectionModeProperty);
    set => this.SetValue(Calendar.SelectionModeProperty, (object) value);
  }

  private static void OnSelectionModeChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    Calendar calendar1 = d as Calendar;
    Calendar calendar2 = calendar1;
    Calendar calendar3 = calendar1;
    DateTime? nullable1 = new DateTime?();
    DateTime? nullable2;
    DateTime? nullable3 = nullable2 = nullable1;
    calendar3.HoverEnd = nullable2;
    DateTime? nullable4 = nullable3;
    calendar2.HoverStart = nullable4;
    calendar1.SelectedDates.ClearInternal(true);
    calendar1.OnSelectionModeChanged(EventArgs.Empty);
  }

  internal event MouseButtonEventHandler DayButtonMouseUp;

  internal event RoutedEventHandler DayOrMonthPreviewKeyDown;

  internal bool DatePickerDisplayDateFlag { get; set; }

  internal DateTime DisplayDateInternal { get; private set; }

  internal DateTime DisplayDateEndInternal => this.DisplayDateEnd ?? DateTime.MaxValue;

  internal DateTime DisplayDateStartInternal => this.DisplayDateStart ?? DateTime.MinValue;

  internal DateTime CurrentDate
  {
    get => this._currentDate ?? this.DisplayDateInternal;
    set => this._currentDate = new DateTime?(value);
  }

  internal DateTime? HoverStart
  {
    get => this.SelectionMode != CalendarSelectionMode.None ? this._hoverStart : new DateTime?();
    set => this._hoverStart = value;
  }

  internal DateTime? HoverEnd
  {
    get => this.SelectionMode != CalendarSelectionMode.None ? this._hoverEnd : new DateTime?();
    set => this._hoverEnd = value;
  }

  internal CalendarItem MonthControl => this._monthControl;

  internal DateTime DisplayMonth => DateTimeHelper.DiscardDayTime(this.DisplayDate);

  internal DateTime DisplayYear => new DateTime(this.DisplayDate.Year, 1, 1);

  private new bool IsRightToLeft => this.FlowDirection == FlowDirection.RightToLeft;

  public override void OnApplyTemplate()
  {
    if (this._monthControl != null)
      this._monthControl.Owner = (Calendar) null;
    base.OnApplyTemplate();
    this._monthControl = this.GetTemplateChild("PART_CalendarItem") as CalendarItem;
    if (this._monthControl != null)
      this._monthControl.Owner = this;
    this.CurrentDate = this.DisplayDate;
    this.UpdateCellItems();
  }

  public override string ToString()
  {
    return this.SelectedDate.HasValue ? this.SelectedDate.Value.ToString((IFormatProvider) DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture((FrameworkElement) this))) : string.Empty;
  }

  protected virtual void OnSelectedDatesChanged(SelectionChangedEventArgs e)
  {
    this.RaiseEvent((RoutedEventArgs) e);
  }

  protected virtual void OnDisplayDateChanged(CalendarDateChangedEventArgs e)
  {
    EventHandler<CalendarDateChangedEventArgs> displayDateChanged = this.DisplayDateChanged;
    if (displayDateChanged == null)
      return;
    displayDateChanged((object) this, e);
  }

  protected virtual void OnDisplayModeChanged(CalendarModeChangedEventArgs e)
  {
    EventHandler<CalendarModeChangedEventArgs> displayModeChanged = this.DisplayModeChanged;
    if (displayModeChanged == null)
      return;
    displayModeChanged((object) this, e);
  }

  protected virtual void OnSelectionModeChanged(EventArgs e)
  {
    EventHandler<EventArgs> selectionModeChanged = this.SelectionModeChanged;
    if (selectionModeChanged == null)
      return;
    selectionModeChanged((object) this, e);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.CalendarAutomationPeer(this);
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    if (e.Handled)
      return;
    e.Handled = this.ProcessCalendarKey(e);
  }

  protected override void OnKeyUp(KeyEventArgs e)
  {
    if (e.Handled || e.Key != Key.LeftShift && e.Key != Key.RightShift)
      return;
    this.ProcessShiftKeyUp();
  }

  internal CalendarDayButton FindDayButtonFromDay(DateTime day)
  {
    if (this.MonthControl != null)
    {
      foreach (CalendarDayButton calendarDayButton in this.MonthControl.GetCalendarDayButtons())
      {
        if (calendarDayButton.DataContext is DateTime && DateTimeHelper.CompareDays((DateTime) calendarDayButton.DataContext, day) == 0)
          return calendarDayButton;
      }
    }
    return (CalendarDayButton) null;
  }

  internal static bool IsValidDateSelection(Calendar cal, object value)
  {
    return value == null || !cal.BlackoutDates.Contains((DateTime) value);
  }

  internal void OnDayButtonMouseUp(MouseButtonEventArgs e)
  {
    MouseButtonEventHandler dayButtonMouseUp = this.DayButtonMouseUp;
    if (dayButtonMouseUp == null)
      return;
    dayButtonMouseUp((object) this, e);
  }

  internal void OnDayOrMonthPreviewKeyDown(RoutedEventArgs e)
  {
    RoutedEventHandler monthPreviewKeyDown = this.DayOrMonthPreviewKeyDown;
    if (monthPreviewKeyDown == null)
      return;
    monthPreviewKeyDown((object) this, e);
  }

  internal void OnDayClick(DateTime selectedDate)
  {
    if (this.SelectionMode == CalendarSelectionMode.None)
      this.CurrentDate = selectedDate;
    if (DateTimeHelper.CompareYearMonth(selectedDate, this.DisplayDateInternal) != 0)
    {
      this.MoveDisplayTo(new DateTime?(selectedDate));
    }
    else
    {
      this.UpdateCellItems();
      this.FocusDate(selectedDate);
    }
  }

  internal void OnCalendarButtonPressed(CalendarButton b, bool switchDisplayMode)
  {
    if (!(b.DataContext is DateTime))
      return;
    DateTime dataContext = (DateTime) b.DataContext;
    DateTime? nullable = new DateTime?();
    CalendarMode calendarMode = CalendarMode.Month;
    switch (this.DisplayMode)
    {
      case CalendarMode.Year:
        nullable = DateTimeHelper.SetYearMonth(this.DisplayDate, dataContext);
        calendarMode = CalendarMode.Month;
        break;
      case CalendarMode.Decade:
        nullable = DateTimeHelper.SetYear(this.DisplayDate, dataContext.Year);
        calendarMode = CalendarMode.Year;
        break;
    }
    if (!nullable.HasValue)
      return;
    this.DisplayDate = nullable.Value;
    if (!switchDisplayMode)
      return;
    this.DisplayMode = calendarMode;
    this.FocusDate(this.DisplayMode == CalendarMode.Month ? this.CurrentDate : this.DisplayDate);
  }

  private DateTime? GetDateOffset(DateTime date, int offset, CalendarMode displayMode)
  {
    DateTime? dateOffset = new DateTime?();
    switch (displayMode)
    {
      case CalendarMode.Month:
        dateOffset = DateTimeHelper.AddMonths(date, offset);
        break;
      case CalendarMode.Year:
        dateOffset = DateTimeHelper.AddYears(date, offset);
        break;
      case CalendarMode.Decade:
        dateOffset = DateTimeHelper.AddYears(this.DisplayDate, offset * 10);
        break;
    }
    return dateOffset;
  }

  private void MoveDisplayTo(DateTime? date)
  {
    if (!date.HasValue)
      return;
    DateTime date1 = date.Value.Date;
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        this.DisplayDate = DateTimeHelper.DiscardDayTime(date1);
        this.CurrentDate = date1;
        this.UpdateCellItems();
        break;
      case CalendarMode.Year:
      case CalendarMode.Decade:
        this.DisplayDate = date1;
        this.UpdateCellItems();
        break;
    }
    this.FocusDate(date1);
  }

  internal void OnNextClick()
  {
    DateTime? dateOffset = this.GetDateOffset(this.DisplayDate, 1, this.DisplayMode);
    if (!dateOffset.HasValue)
      return;
    this.MoveDisplayTo(new DateTime?(DateTimeHelper.DiscardDayTime(dateOffset.Value)));
  }

  internal void OnPreviousClick()
  {
    DateTime? dateOffset = this.GetDateOffset(this.DisplayDate, -1, this.DisplayMode);
    if (!dateOffset.HasValue)
      return;
    this.MoveDisplayTo(new DateTime?(DateTimeHelper.DiscardDayTime(dateOffset.Value)));
  }

  internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
  {
    if (!Calendar.IsSelectionChanged(e))
      return;
    if ((AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection)) && UIElementAutomationPeer.FromElement((UIElement) this) is Microsoft.Windows.Automation.Peers.CalendarAutomationPeer calendarAutomationPeer)
      calendarAutomationPeer.RaiseSelectionEvents(e);
    this.CoerceFromSelection();
    this.OnSelectedDatesChanged(e);
  }

  internal void UpdateCellItems()
  {
    CalendarItem monthControl = this.MonthControl;
    if (monthControl == null)
      return;
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        monthControl.UpdateMonthMode();
        break;
      case CalendarMode.Year:
        monthControl.UpdateYearMode();
        break;
      case CalendarMode.Decade:
        monthControl.UpdateDecadeMode();
        break;
    }
  }

  private void CoerceFromSelection()
  {
    this.CoerceValue(Calendar.DisplayDateStartProperty);
    this.CoerceValue(Calendar.DisplayDateEndProperty);
    this.CoerceValue(Calendar.DisplayDateProperty);
  }

  private void AddKeyboardSelection()
  {
    if (!this.HoverStart.HasValue)
      return;
    this.SelectedDates.ClearInternal();
    this.SelectedDates.AddRange(this.HoverStart.Value, this.CurrentDate);
  }

  private static bool IsSelectionChanged(SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count != e.RemovedItems.Count)
      return true;
    foreach (DateTime addedItem in (IEnumerable) e.AddedItems)
    {
      if (!e.RemovedItems.Contains((object) addedItem))
        return true;
    }
    return false;
  }

  private static bool IsValidDisplayMode(object value)
  {
    CalendarMode calendarMode = (CalendarMode) value;
    switch (calendarMode)
    {
      case CalendarMode.Month:
      case CalendarMode.Year:
        return true;
      default:
        return calendarMode == CalendarMode.Decade;
    }
  }

  internal static bool IsValidFirstDayOfWeek(object value)
  {
    DayOfWeek dayOfWeek = (DayOfWeek) value;
    switch (dayOfWeek)
    {
      case DayOfWeek.Sunday:
      case DayOfWeek.Monday:
      case DayOfWeek.Tuesday:
      case DayOfWeek.Wednesday:
      case DayOfWeek.Thursday:
      case DayOfWeek.Friday:
        return true;
      default:
        return dayOfWeek == DayOfWeek.Saturday;
    }
  }

  private static bool IsValidKeyboardSelection(Calendar cal, object value)
  {
    if (value == null)
      return true;
    return !cal.BlackoutDates.Contains((DateTime) value) && DateTime.Compare((DateTime) value, cal.DisplayDateStartInternal) >= 0 && DateTime.Compare((DateTime) value, cal.DisplayDateEndInternal) <= 0;
  }

  private static bool IsValidSelectionMode(object value)
  {
    CalendarSelectionMode calendarSelectionMode = (CalendarSelectionMode) value;
    switch (calendarSelectionMode)
    {
      case CalendarSelectionMode.SingleDate:
      case CalendarSelectionMode.SingleRange:
      case CalendarSelectionMode.MultipleRange:
        return true;
      default:
        return calendarSelectionMode == CalendarSelectionMode.None;
    }
  }

  private void OnSelectedMonthChanged(DateTime? selectedMonth)
  {
    if (!selectedMonth.HasValue)
      return;
    this.DisplayDate = selectedMonth.Value;
    this.UpdateCellItems();
    this.FocusDate(selectedMonth.Value);
  }

  private void OnSelectedYearChanged(DateTime? selectedYear)
  {
    if (!selectedYear.HasValue)
      return;
    this.DisplayDate = selectedYear.Value;
    this.UpdateCellItems();
    this.FocusDate(selectedYear.Value);
  }

  internal void FocusDate(DateTime date)
  {
    if (this.MonthControl == null)
      return;
    this.MonthControl.FocusDate(date);
  }

  private static void OnGotFocus(object sender, RoutedEventArgs e)
  {
    Calendar calendar = (Calendar) sender;
    if (e.Handled || e.OriginalSource != calendar)
      return;
    if (calendar.SelectedDate.HasValue && DateTimeHelper.CompareYearMonth(calendar.SelectedDate.Value, calendar.DisplayDateInternal) == 0)
      calendar.FocusDate(calendar.SelectedDate.Value);
    else
      calendar.FocusDate(calendar.DisplayDate);
    e.Handled = true;
  }

  private bool ProcessCalendarKey(KeyEventArgs e)
  {
    if (this.DisplayMode == CalendarMode.Month)
    {
      CalendarDayButton calendarDayButton = this.MonthControl != null ? this.MonthControl.GetCalendarDayButton(this.CurrentDate) : (CalendarDayButton) null;
      if (DateTimeHelper.CompareYearMonth(this.CurrentDate, this.DisplayDateInternal) != 0 && calendarDayButton != null && !calendarDayButton.IsInactive)
        return false;
    }
    bool ctrl;
    bool shift;
    KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
    switch (e.Key)
    {
      case Key.Return:
      case Key.Space:
        return this.ProcessEnterKey();
      case Key.Prior:
        this.ProcessPageUpKey(shift);
        return true;
      case Key.Next:
        this.ProcessPageDownKey(shift);
        return true;
      case Key.End:
        this.ProcessEndKey(shift);
        return true;
      case Key.Home:
        this.ProcessHomeKey(shift);
        return true;
      case Key.Left:
        this.ProcessLeftKey(shift);
        return true;
      case Key.Up:
        this.ProcessUpKey(ctrl, shift);
        return true;
      case Key.Right:
        this.ProcessRightKey(shift);
        return true;
      case Key.Down:
        this.ProcessDownKey(ctrl, shift);
        return true;
      default:
        return false;
    }
  }

  private void ProcessDownKey(bool ctrl, bool shift)
  {
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        if (ctrl && !shift)
          break;
        DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, 7), 1);
        this.ProcessSelection(shift, nonBlackoutDate);
        break;
      case CalendarMode.Year:
        if (ctrl)
        {
          this.DisplayMode = CalendarMode.Month;
          this.FocusDate(this.DisplayDate);
          break;
        }
        this.OnSelectedMonthChanged(DateTimeHelper.AddMonths(this.DisplayDate, 4));
        break;
      case CalendarMode.Decade:
        if (ctrl)
        {
          this.DisplayMode = CalendarMode.Year;
          this.FocusDate(this.DisplayDate);
          break;
        }
        this.OnSelectedYearChanged(DateTimeHelper.AddYears(this.DisplayDate, 4));
        break;
    }
  }

  private void ProcessEndKey(bool shift)
  {
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        DateTime displayDate = this.DisplayDate;
        DateTime? lastSelectedDate = new DateTime?(new DateTime(this.DisplayDateInternal.Year, this.DisplayDateInternal.Month, 1));
        if (DateTimeHelper.CompareYearMonth(DateTime.MaxValue, lastSelectedDate.Value) > 0)
        {
          lastSelectedDate = new DateTime?(DateTimeHelper.AddMonths(lastSelectedDate.Value, 1).Value);
          lastSelectedDate = new DateTime?(DateTimeHelper.AddDays(lastSelectedDate.Value, -1).Value);
        }
        else
          lastSelectedDate = new DateTime?(DateTime.MaxValue);
        this.ProcessSelection(shift, lastSelectedDate);
        break;
      case CalendarMode.Year:
        this.OnSelectedMonthChanged(new DateTime?(new DateTime(this.DisplayDate.Year, 12, 1)));
        break;
      case CalendarMode.Decade:
        this.OnSelectedYearChanged(new DateTime?(new DateTime(DateTimeHelper.EndOfDecade(this.DisplayDate), 1, 1)));
        break;
    }
  }

  private bool ProcessEnterKey()
  {
    switch (this.DisplayMode)
    {
      case CalendarMode.Year:
        this.DisplayMode = CalendarMode.Month;
        this.FocusDate(this.DisplayDate);
        return true;
      case CalendarMode.Decade:
        this.DisplayMode = CalendarMode.Year;
        this.FocusDate(this.DisplayDate);
        return true;
      default:
        return false;
    }
  }

  private void ProcessHomeKey(bool shift)
  {
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        DateTime? lastSelectedDate = new DateTime?(new DateTime(this.DisplayDateInternal.Year, this.DisplayDateInternal.Month, 1));
        this.ProcessSelection(shift, lastSelectedDate);
        break;
      case CalendarMode.Year:
        this.OnSelectedMonthChanged(new DateTime?(new DateTime(this.DisplayDate.Year, 1, 1)));
        break;
      case CalendarMode.Decade:
        this.OnSelectedYearChanged(new DateTime?(new DateTime(DateTimeHelper.DecadeOfDate(this.DisplayDate), 1, 1)));
        break;
    }
  }

  private void ProcessLeftKey(bool shift)
  {
    int num = !this.IsRightToLeft ? -1 : 1;
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, num), num);
        this.ProcessSelection(shift, nonBlackoutDate);
        break;
      case CalendarMode.Year:
        this.OnSelectedMonthChanged(DateTimeHelper.AddMonths(this.DisplayDate, num));
        break;
      case CalendarMode.Decade:
        this.OnSelectedYearChanged(DateTimeHelper.AddYears(this.DisplayDate, num));
        break;
    }
  }

  private void ProcessPageDownKey(bool shift)
  {
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddMonths(this.CurrentDate, 1), 1);
        this.ProcessSelection(shift, nonBlackoutDate);
        break;
      case CalendarMode.Year:
        this.OnSelectedMonthChanged(DateTimeHelper.AddYears(this.DisplayDate, 1));
        break;
      case CalendarMode.Decade:
        this.OnSelectedYearChanged(DateTimeHelper.AddYears(this.DisplayDate, 10));
        break;
    }
  }

  private void ProcessPageUpKey(bool shift)
  {
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddMonths(this.CurrentDate, -1), -1);
        this.ProcessSelection(shift, nonBlackoutDate);
        break;
      case CalendarMode.Year:
        this.OnSelectedMonthChanged(DateTimeHelper.AddYears(this.DisplayDate, -1));
        break;
      case CalendarMode.Decade:
        this.OnSelectedYearChanged(DateTimeHelper.AddYears(this.DisplayDate, -10));
        break;
    }
  }

  private void ProcessRightKey(bool shift)
  {
    int num = !this.IsRightToLeft ? 1 : -1;
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, num), num);
        this.ProcessSelection(shift, nonBlackoutDate);
        break;
      case CalendarMode.Year:
        this.OnSelectedMonthChanged(DateTimeHelper.AddMonths(this.DisplayDate, num));
        break;
      case CalendarMode.Decade:
        this.OnSelectedYearChanged(DateTimeHelper.AddYears(this.DisplayDate, num));
        break;
    }
  }

  private void ProcessSelection(bool shift, DateTime? lastSelectedDate)
  {
    if (this.SelectionMode == CalendarSelectionMode.None && lastSelectedDate.HasValue)
    {
      this.OnDayClick(lastSelectedDate.Value);
    }
    else
    {
      if (!lastSelectedDate.HasValue || !Calendar.IsValidKeyboardSelection(this, (object) lastSelectedDate.Value))
        return;
      if (this.SelectionMode == CalendarSelectionMode.SingleRange || this.SelectionMode == CalendarSelectionMode.MultipleRange)
      {
        this.SelectedDates.ClearInternal();
        if (shift)
        {
          this._isShiftPressed = true;
          if (!this.HoverStart.HasValue)
            this.HoverStart = this.HoverEnd = new DateTime?(this.CurrentDate);
          if (!this.BlackoutDates.ContainsAny(DateTime.Compare(this.HoverStart.Value, lastSelectedDate.Value) >= 0 ? new CalendarDateRange(lastSelectedDate.Value, this.HoverStart.Value) : new CalendarDateRange(this.HoverStart.Value, lastSelectedDate.Value)))
          {
            this._currentDate = lastSelectedDate;
            this.HoverEnd = lastSelectedDate;
          }
          this.OnDayClick(this.CurrentDate);
        }
        else
        {
          this.HoverStart = this.HoverEnd = new DateTime?(this.CurrentDate = lastSelectedDate.Value);
          this.AddKeyboardSelection();
          this.OnDayClick(lastSelectedDate.Value);
        }
      }
      else
      {
        this.CurrentDate = lastSelectedDate.Value;
        this.HoverStart = this.HoverEnd = new DateTime?();
        if (this.SelectedDates.Count > 0)
          this.SelectedDates[0] = lastSelectedDate.Value;
        else
          this.SelectedDates.Add(lastSelectedDate.Value);
        this.OnDayClick(lastSelectedDate.Value);
      }
      this.UpdateCellItems();
    }
  }

  private void ProcessShiftKeyUp()
  {
    if (!this._isShiftPressed || this.SelectionMode != CalendarSelectionMode.SingleRange && this.SelectionMode != CalendarSelectionMode.MultipleRange)
      return;
    this.AddKeyboardSelection();
    this._isShiftPressed = false;
    this.HoverStart = this.HoverEnd = new DateTime?();
  }

  private void ProcessUpKey(bool ctrl, bool shift)
  {
    switch (this.DisplayMode)
    {
      case CalendarMode.Month:
        if (ctrl)
        {
          this.DisplayMode = CalendarMode.Year;
          this.FocusDate(this.DisplayDate);
          break;
        }
        DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, -7), -1);
        this.ProcessSelection(shift, nonBlackoutDate);
        break;
      case CalendarMode.Year:
        if (ctrl)
        {
          this.DisplayMode = CalendarMode.Decade;
          this.FocusDate(this.DisplayDate);
          break;
        }
        this.OnSelectedMonthChanged(DateTimeHelper.AddMonths(this.DisplayDate, -4));
        break;
      case CalendarMode.Decade:
        if (ctrl)
          break;
        this.OnSelectedYearChanged(DateTimeHelper.AddYears(this.DisplayDate, -4));
        break;
    }
  }
}
