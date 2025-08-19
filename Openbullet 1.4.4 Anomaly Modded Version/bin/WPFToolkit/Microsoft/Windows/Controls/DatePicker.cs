// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DatePicker
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Windows.Controls;

[TemplatePart(Name = "PART_Root", Type = typeof (Grid))]
[TemplatePart(Name = "PART_Popup", Type = typeof (Popup))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = "PART_TextBox", Type = typeof (Microsoft.Windows.Controls.Primitives.DatePickerTextBox))]
[TemplatePart(Name = "PART_Button", Type = typeof (Button))]
public class DatePicker : Control
{
  private const string ElementRoot = "PART_Root";
  private const string ElementTextBox = "PART_TextBox";
  private const string ElementButton = "PART_Button";
  private const string ElementPopup = "PART_Popup";
  private Calendar _calendar;
  private string _defaultText;
  private ButtonBase _dropDownButton;
  private Popup _popUp;
  private bool _disablePopupReopen;
  private bool _shouldCoerceText;
  private string _coercedTextValue;
  private Microsoft.Windows.Controls.Primitives.DatePickerTextBox _textBox;
  private IDictionary<DependencyProperty, bool> _isHandlerSuspended;
  private DateTime? _originalSelectedDate;
  public static readonly RoutedEvent SelectedDateChangedEvent = EventManager.RegisterRoutedEvent("SelectedDateChanged", RoutingStrategy.Direct, typeof (EventHandler<SelectionChangedEventArgs>), typeof (DatePicker));
  public static readonly DependencyProperty CalendarStyleProperty = DependencyProperty.Register(nameof (CalendarStyle), typeof (Style), typeof (DatePicker));
  public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof (DisplayDate), typeof (DateTime), typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (PropertyChangedCallback) null, new CoerceValueCallback(DatePicker.CoerceDisplayDate)));
  public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(nameof (DisplayDateEnd), typeof (DateTime?), typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DatePicker.OnDisplayDateEndChanged), new CoerceValueCallback(DatePicker.CoerceDisplayDateEnd)));
  public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(nameof (DisplayDateStart), typeof (DateTime?), typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DatePicker.OnDisplayDateStartChanged), new CoerceValueCallback(DatePicker.CoerceDisplayDateStart)));
  public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(nameof (FirstDayOfWeek), typeof (DayOfWeek), typeof (DatePicker), (PropertyMetadata) null, new ValidateValueCallback(Calendar.IsValidFirstDayOfWeek));
  public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof (IsDropDownOpen), typeof (bool), typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DatePicker.OnIsDropDownOpenChanged), new CoerceValueCallback(DatePicker.OnCoerceIsDropDownOpen)));
  public static readonly DependencyProperty IsTodayHighlightedProperty = DependencyProperty.Register(nameof (IsTodayHighlighted), typeof (bool), typeof (DatePicker));
  public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof (SelectedDate), typeof (DateTime?), typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DatePicker.OnSelectedDateChanged), new CoerceValueCallback(DatePicker.CoerceSelectedDate)));
  public static readonly DependencyProperty SelectedDateFormatProperty = DependencyProperty.Register(nameof (SelectedDateFormat), typeof (DatePickerFormat), typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) DatePickerFormat.Long, new PropertyChangedCallback(DatePicker.OnSelectedDateFormatChanged)), new ValidateValueCallback(DatePicker.IsValidSelectedDateFormat));
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty, new PropertyChangedCallback(DatePicker.OnTextChanged), new CoerceValueCallback(DatePicker.OnCoerceText)));

  public event RoutedEventHandler CalendarClosed;

  public event RoutedEventHandler CalendarOpened;

  public event EventHandler<DatePickerDateValidationErrorEventArgs> DateValidationError;

  public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged
  {
    add => this.AddHandler(DatePicker.SelectedDateChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(DatePicker.SelectedDateChangedEvent, (Delegate) value);
  }

  static DatePicker()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DatePicker)));
    EventManager.RegisterClassHandler(typeof (DatePicker), UIElement.GotFocusEvent, (Delegate) new RoutedEventHandler(DatePicker.OnGotFocus));
    KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Once));
    KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof (DatePicker), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    UIElement.IsEnabledProperty.OverrideMetadata(typeof (DatePicker), (PropertyMetadata) new UIPropertyMetadata(new PropertyChangedCallback(DatePicker.OnIsEnabledChanged)));
  }

  public DatePicker()
  {
    this.InitializeCalendar();
    this._defaultText = string.Empty;
    this.FirstDayOfWeek = DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek;
    this.DisplayDate = DateTime.Today;
  }

  public CalendarBlackoutDatesCollection BlackoutDates => this._calendar.BlackoutDates;

  public Style CalendarStyle
  {
    get => (Style) this.GetValue(DatePicker.CalendarStyleProperty);
    set => this.SetValue(DatePicker.CalendarStyleProperty, (object) value);
  }

  public DateTime DisplayDate
  {
    get => (DateTime) this.GetValue(DatePicker.DisplayDateProperty);
    set => this.SetValue(DatePicker.DisplayDateProperty, (object) value);
  }

  private static object CoerceDisplayDate(DependencyObject d, object value)
  {
    DatePicker datePicker = d as DatePicker;
    datePicker._calendar.DisplayDate = (DateTime) value;
    return (object) datePicker._calendar.DisplayDate;
  }

  public DateTime? DisplayDateEnd
  {
    get => (DateTime?) this.GetValue(DatePicker.DisplayDateEndProperty);
    set => this.SetValue(DatePicker.DisplayDateEndProperty, (object) value);
  }

  private static void OnDisplayDateEndChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    (d as DatePicker).CoerceValue(DatePicker.DisplayDateProperty);
  }

  private static object CoerceDisplayDateEnd(DependencyObject d, object value)
  {
    DatePicker datePicker = d as DatePicker;
    datePicker._calendar.DisplayDateEnd = (DateTime?) value;
    return (object) datePicker._calendar.DisplayDateEnd;
  }

  public DateTime? DisplayDateStart
  {
    get => (DateTime?) this.GetValue(DatePicker.DisplayDateStartProperty);
    set => this.SetValue(DatePicker.DisplayDateStartProperty, (object) value);
  }

  private static void OnDisplayDateStartChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DatePicker datePicker = d as DatePicker;
    datePicker.CoerceValue(DatePicker.DisplayDateEndProperty);
    datePicker.CoerceValue(DatePicker.DisplayDateProperty);
  }

  private static object CoerceDisplayDateStart(DependencyObject d, object value)
  {
    DatePicker datePicker = d as DatePicker;
    datePicker._calendar.DisplayDateStart = (DateTime?) value;
    return (object) datePicker._calendar.DisplayDateStart;
  }

  public DayOfWeek FirstDayOfWeek
  {
    get => (DayOfWeek) this.GetValue(DatePicker.FirstDayOfWeekProperty);
    set => this.SetValue(DatePicker.FirstDayOfWeekProperty, (object) value);
  }

  public bool IsDropDownOpen
  {
    get => (bool) this.GetValue(DatePicker.IsDropDownOpenProperty);
    set => this.SetValue(DatePicker.IsDropDownOpenProperty, (object) value);
  }

  private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
  {
    return !(d as DatePicker).IsEnabled ? (object) false : baseValue;
  }

  private static void OnIsDropDownOpenChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DatePicker dp = d as DatePicker;
    bool newValue = (bool) e.NewValue;
    if (dp._popUp == null || dp._popUp.IsOpen == newValue)
      return;
    dp._popUp.IsOpen = newValue;
    if (!newValue)
      return;
    dp._originalSelectedDate = dp.SelectedDate;
    dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) (() => dp._calendar.Focus()));
  }

  private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    DatePicker dp = d as DatePicker;
    dp.CoerceValue(DatePicker.IsDropDownOpenProperty);
    DatePicker.OnVisualStatePropertyChanged(dp);
  }

  private static void OnVisualStatePropertyChanged(DatePicker dp)
  {
    if (Validation.GetHasError((DependencyObject) dp))
    {
      if (dp.IsKeyboardFocused)
        VisualStateManager.GoToState((Control) dp, "InvalidFocused", true);
      else
        VisualStateManager.GoToState((Control) dp, "InvalidUnfocused", true);
    }
    else
      VisualStateManager.GoToState((Control) dp, "Valid", true);
  }

  public bool IsTodayHighlighted
  {
    get => (bool) this.GetValue(DatePicker.IsTodayHighlightedProperty);
    set => this.SetValue(DatePicker.IsTodayHighlightedProperty, (object) value);
  }

  public DateTime? SelectedDate
  {
    get => (DateTime?) this.GetValue(DatePicker.SelectedDateProperty);
    set => this.SetValue(DatePicker.SelectedDateProperty, (object) value);
  }

  private static void OnSelectedDateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DatePicker element = d as DatePicker;
    Collection<DateTime> addedItems = new Collection<DateTime>();
    Collection<DateTime> removedItems = new Collection<DateTime>();
    element.CoerceValue(DatePicker.DisplayDateStartProperty);
    element.CoerceValue(DatePicker.DisplayDateEndProperty);
    element.CoerceValue(DatePicker.DisplayDateProperty);
    DateTime? newValue1 = (DateTime?) e.NewValue;
    DateTime? oldValue1 = (DateTime?) e.OldValue;
    if (element.SelectedDate.HasValue)
    {
      DateTime d1 = element.SelectedDate.Value;
      element.SetTextInternal(element.DateTimeToString(d1));
      if ((d1.Month != element.DisplayDate.Month || d1.Year != element.DisplayDate.Year) && !element._calendar.DatePickerDisplayDateFlag)
        element.DisplayDate = d1;
      element._calendar.DatePickerDisplayDateFlag = false;
    }
    else
      element.SetWaterMarkText();
    if (newValue1.HasValue)
      addedItems.Add(newValue1.Value);
    if (oldValue1.HasValue)
      removedItems.Add(oldValue1.Value);
    element.OnSelectedDateChanged((SelectionChangedEventArgs) new CalendarSelectionChangedEventArgs(DatePicker.SelectedDateChangedEvent, (IList) removedItems, (IList) addedItems));
    if (!(UIElementAutomationPeer.FromElement((UIElement) element) is Microsoft.Windows.Automation.Peers.DatePickerAutomationPeer pickerAutomationPeer))
      return;
    string newValue2 = newValue1.HasValue ? element.DateTimeToString(newValue1.Value) : "";
    string oldValue2 = oldValue1.HasValue ? element.DateTimeToString(oldValue1.Value) : "";
    pickerAutomationPeer.RaiseValuePropertyChangedEvent(oldValue2, newValue2);
  }

  private static object CoerceSelectedDate(DependencyObject d, object value)
  {
    DatePicker datePicker = d as DatePicker;
    datePicker._calendar.SelectedDate = (DateTime?) value;
    return (object) datePicker._calendar.SelectedDate;
  }

  public DatePickerFormat SelectedDateFormat
  {
    get => (DatePickerFormat) this.GetValue(DatePicker.SelectedDateFormatProperty);
    set => this.SetValue(DatePicker.SelectedDateFormatProperty, (object) value);
  }

  private static void OnSelectedDateFormatChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    DatePicker datePicker = d as DatePicker;
    if (datePicker._textBox == null)
      return;
    if (string.IsNullOrEmpty(datePicker._textBox.Text))
    {
      datePicker.SetWaterMarkText();
    }
    else
    {
      DateTime? text = datePicker.ParseText(datePicker._textBox.Text);
      if (!text.HasValue)
        return;
      datePicker.SetTextInternal(datePicker.DateTimeToString(text.Value));
    }
  }

  public string Text
  {
    get => (string) this.GetValue(DatePicker.TextProperty);
    set => this.SetValue(DatePicker.TextProperty, (object) value);
  }

  private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    DatePicker datePicker = d as DatePicker;
    if (datePicker.IsHandlerSuspended(DatePicker.TextProperty))
      return;
    if (e.NewValue is string newValue)
    {
      if (datePicker._textBox != null)
        datePicker._textBox.Text = newValue;
      else
        datePicker._defaultText = newValue;
      datePicker.SetSelectedDate();
    }
    else
      datePicker.SetValueNoCallback(DatePicker.SelectedDateProperty, (object) null);
  }

  private static object OnCoerceText(DependencyObject dObject, object baseValue)
  {
    DatePicker datePicker = (DatePicker) dObject;
    if (!datePicker._shouldCoerceText)
      return baseValue;
    datePicker._shouldCoerceText = false;
    return (object) datePicker._coercedTextValue;
  }

  private void SetTextInternal(string value)
  {
    if (BindingOperations.GetBindingExpressionBase((DependencyObject) this, DatePicker.TextProperty) != null)
    {
      this.Text = value;
    }
    else
    {
      this._shouldCoerceText = true;
      this._coercedTextValue = value;
      this.CoerceValue(DatePicker.TextProperty);
    }
  }

  public override void OnApplyTemplate()
  {
    if (this._popUp != null)
    {
      this._popUp.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.PopUp_PreviewMouseLeftButtonDown));
      this._popUp.Opened -= new EventHandler(this.PopUp_Opened);
      this._popUp.Closed -= new EventHandler(this.PopUp_Closed);
      this._popUp.Child = (UIElement) null;
    }
    if (this._dropDownButton != null)
    {
      this._dropDownButton.Click -= new RoutedEventHandler(this.DropDownButton_Click);
      this._dropDownButton.RemoveHandler(UIElement.MouseLeaveEvent, (Delegate) new MouseEventHandler(this.DropDownButton_MouseLeave));
    }
    if (this._textBox != null)
    {
      this._textBox.RemoveHandler(UIElement.KeyDownEvent, (Delegate) new KeyEventHandler(this.TextBox_KeyDown));
      this._textBox.RemoveHandler(TextBoxBase.TextChangedEvent, (Delegate) new TextChangedEventHandler(this.TextBox_TextChanged));
      this._textBox.RemoveHandler(UIElement.LostFocusEvent, (Delegate) new RoutedEventHandler(this.TextBox_LostFocus));
    }
    base.OnApplyTemplate();
    this._popUp = this.GetTemplateChild("PART_Popup") as Popup;
    if (this._popUp != null)
    {
      this._popUp.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.PopUp_PreviewMouseLeftButtonDown));
      this._popUp.Opened += new EventHandler(this.PopUp_Opened);
      this._popUp.Closed += new EventHandler(this.PopUp_Closed);
      this._popUp.Child = (UIElement) this._calendar;
      if (this.IsDropDownOpen)
        this._popUp.IsOpen = true;
    }
    this._dropDownButton = (ButtonBase) (this.GetTemplateChild("PART_Button") as Button);
    if (this._dropDownButton != null)
    {
      this._dropDownButton.Click += new RoutedEventHandler(this.DropDownButton_Click);
      this._dropDownButton.AddHandler(UIElement.MouseLeaveEvent, (Delegate) new MouseEventHandler(this.DropDownButton_MouseLeave), true);
      if (this._dropDownButton.Content == null)
        this._dropDownButton.Content = (object) SR.Get(SRID.DatePicker_DropDownButtonName);
    }
    this._textBox = this.GetTemplateChild("PART_TextBox") as Microsoft.Windows.Controls.Primitives.DatePickerTextBox;
    this.UpdateDisabledVisual();
    if (!this.SelectedDate.HasValue)
      this.SetWaterMarkText();
    if (this._textBox == null)
      return;
    this._textBox.AddHandler(UIElement.KeyDownEvent, (Delegate) new KeyEventHandler(this.TextBox_KeyDown), true);
    this._textBox.AddHandler(TextBoxBase.TextChangedEvent, (Delegate) new TextChangedEventHandler(this.TextBox_TextChanged), true);
    this._textBox.AddHandler(UIElement.LostFocusEvent, (Delegate) new RoutedEventHandler(this.TextBox_LostFocus), true);
    if (!this.SelectedDate.HasValue)
    {
      if (string.IsNullOrEmpty(this._defaultText))
        return;
      this._textBox.Text = this._defaultText;
      this.SetSelectedDate();
    }
    else
      this._textBox.Text = this.DateTimeToString(this.SelectedDate.Value);
  }

  public override string ToString()
  {
    return this.SelectedDate.HasValue ? this.SelectedDate.Value.ToString((IFormatProvider) DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture((FrameworkElement) this))) : string.Empty;
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new Microsoft.Windows.Automation.Peers.DatePickerAutomationPeer(this);
  }

  protected virtual void OnCalendarClosed(RoutedEventArgs e)
  {
    RoutedEventHandler calendarClosed = this.CalendarClosed;
    if (calendarClosed == null)
      return;
    calendarClosed((object) this, e);
  }

  protected virtual void OnCalendarOpened(RoutedEventArgs e)
  {
    RoutedEventHandler calendarOpened = this.CalendarOpened;
    if (calendarOpened == null)
      return;
    calendarOpened((object) this, e);
  }

  protected virtual void OnSelectedDateChanged(SelectionChangedEventArgs e)
  {
    this.RaiseEvent((RoutedEventArgs) e);
  }

  protected virtual void OnDateValidationError(DatePickerDateValidationErrorEventArgs e)
  {
    EventHandler<DatePickerDateValidationErrorEventArgs> dateValidationError = this.DateValidationError;
    if (dateValidationError == null)
      return;
    dateValidationError((object) this, e);
  }

  private static void OnGotFocus(object sender, RoutedEventArgs e)
  {
    DatePicker datePicker = (DatePicker) sender;
    if (e.Handled || datePicker._textBox == null)
      return;
    if (e.OriginalSource == datePicker)
    {
      datePicker._textBox.Focus();
      e.Handled = true;
    }
    else
    {
      if (e.OriginalSource != datePicker._textBox)
        return;
      datePicker._textBox.SelectAll();
      e.Handled = true;
    }
  }

  private void SetValueNoCallback(DependencyProperty property, object value)
  {
    this.SetIsHandlerSuspended(property, true);
    try
    {
      this.SetValue(property, value);
    }
    finally
    {
      this.SetIsHandlerSuspended(property, false);
    }
  }

  private bool IsHandlerSuspended(DependencyProperty property)
  {
    return this._isHandlerSuspended != null && this._isHandlerSuspended.ContainsKey(property);
  }

  private void SetIsHandlerSuspended(DependencyProperty property, bool value)
  {
    if (value)
    {
      if (this._isHandlerSuspended == null)
        this._isHandlerSuspended = (IDictionary<DependencyProperty, bool>) new Dictionary<DependencyProperty, bool>(2);
      this._isHandlerSuspended[property] = true;
    }
    else
    {
      if (this._isHandlerSuspended == null)
        return;
      this._isHandlerSuspended.Remove(property);
    }
  }

  private void PopUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (!(sender is Popup popup) || popup.StaysOpen || this._dropDownButton == null || this._dropDownButton.InputHitTest(e.GetPosition((IInputElement) this._dropDownButton)) == null)
      return;
    this._disablePopupReopen = true;
  }

  private void PopUp_Opened(object sender, EventArgs e)
  {
    if (!this.IsDropDownOpen)
      this.IsDropDownOpen = true;
    if (this._calendar != null)
    {
      this._calendar.DisplayMode = CalendarMode.Month;
      this._calendar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    }
    this.OnCalendarOpened(new RoutedEventArgs());
  }

  private void PopUp_Closed(object sender, EventArgs e)
  {
    if (this.IsDropDownOpen)
      this.IsDropDownOpen = false;
    if (this._calendar.IsKeyboardFocusWithin)
      this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    this.OnCalendarClosed(new RoutedEventArgs());
  }

  private void Calendar_DayButtonMouseUp(object sender, MouseButtonEventArgs e)
  {
    this.IsDropDownOpen = false;
  }

  private void Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
  {
    DateTime? addedDate = e.AddedDate;
    DateTime displayDate = this.DisplayDate;
    if ((!addedDate.HasValue ? 1 : (addedDate.GetValueOrDefault() != displayDate ? 1 : 0)) == 0)
      return;
    this.SetValue(DatePicker.DisplayDateProperty, (object) e.AddedDate.Value);
  }

  private void CalendarDayOrMonthButton_PreviewKeyDown(object sender, RoutedEventArgs e)
  {
    Calendar calendar = sender as Calendar;
    KeyEventArgs keyEventArgs = (KeyEventArgs) e;
    if (keyEventArgs.Key != Key.Escape && (keyEventArgs.Key != Key.Return && keyEventArgs.Key != Key.Space || calendar.DisplayMode != CalendarMode.Month))
      return;
    this.IsDropDownOpen = false;
    if (keyEventArgs.Key != Key.Escape)
      return;
    this.SelectedDate = this._originalSelectedDate;
  }

  private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count > 0 && this.SelectedDate.HasValue && DateTime.Compare((DateTime) e.AddedItems[0], this.SelectedDate.Value) != 0)
      this.SelectedDate = (DateTime?) e.AddedItems[0];
    else if (e.AddedItems.Count == 0)
    {
      this.SelectedDate = new DateTime?();
    }
    else
    {
      if (this.SelectedDate.HasValue || e.AddedItems.Count <= 0)
        return;
      this.SelectedDate = (DateTime?) e.AddedItems[0];
    }
  }

  private string DateTimeToString(DateTime d)
  {
    DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture((FrameworkElement) this));
    switch (this.SelectedDateFormat)
    {
      case DatePickerFormat.Long:
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, d.ToString(dateFormat.LongDatePattern, (IFormatProvider) dateFormat));
      case DatePickerFormat.Short:
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, d.ToString(dateFormat.ShortDatePattern, (IFormatProvider) dateFormat));
      default:
        return (string) null;
    }
  }

  private static DateTime DiscardDayTime(DateTime d) => new DateTime(d.Year, d.Month, 1, 0, 0, 0);

  private static DateTime? DiscardTime(DateTime? d)
  {
    if (!d.HasValue)
      return new DateTime?();
    DateTime dateTime = d.Value;
    return new DateTime?(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0));
  }

  private void DropDownButton_Click(object sender, RoutedEventArgs e) => this.TogglePopUp();

  private void DropDownButton_MouseLeave(object sender, MouseEventArgs e)
  {
    this._disablePopupReopen = false;
  }

  private void TogglePopUp()
  {
    if (this.IsDropDownOpen)
      this.IsDropDownOpen = false;
    else if (this._disablePopupReopen)
    {
      this._disablePopupReopen = false;
    }
    else
    {
      this.SetSelectedDate();
      this.IsDropDownOpen = true;
    }
  }

  private void InitializeCalendar()
  {
    this._calendar = new Calendar();
    this._calendar.DayButtonMouseUp += new MouseButtonEventHandler(this.Calendar_DayButtonMouseUp);
    this._calendar.DisplayDateChanged += new EventHandler<CalendarDateChangedEventArgs>(this.Calendar_DisplayDateChanged);
    this._calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(this.Calendar_SelectedDatesChanged);
    this._calendar.DayOrMonthPreviewKeyDown += new RoutedEventHandler(this.CalendarDayOrMonthButton_PreviewKeyDown);
    this._calendar.HorizontalAlignment = HorizontalAlignment.Left;
    this._calendar.VerticalAlignment = VerticalAlignment.Top;
    this._calendar.SelectionMode = CalendarSelectionMode.SingleDate;
    this._calendar.SetBinding(Control.ForegroundProperty, this.GetDatePickerBinding(Control.ForegroundProperty));
    this._calendar.SetBinding(FrameworkElement.StyleProperty, this.GetDatePickerBinding(DatePicker.CalendarStyleProperty));
    this._calendar.SetBinding(Calendar.IsTodayHighlightedProperty, this.GetDatePickerBinding(DatePicker.IsTodayHighlightedProperty));
    this._calendar.SetBinding(Calendar.FirstDayOfWeekProperty, this.GetDatePickerBinding(DatePicker.FirstDayOfWeekProperty));
  }

  private BindingBase GetDatePickerBinding(DependencyProperty property)
  {
    return (BindingBase) new Binding(property.Name)
    {
      Source = (object) this
    };
  }

  private static bool IsValidSelectedDateFormat(object value)
  {
    DatePickerFormat datePickerFormat = (DatePickerFormat) value;
    return datePickerFormat == DatePickerFormat.Long || datePickerFormat == DatePickerFormat.Short;
  }

  private DateTime? ParseText(string text)
  {
    try
    {
      DateTime dateTime = DateTime.Parse(text, (IFormatProvider) DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture((FrameworkElement) this)));
      if (Calendar.IsValidDateSelection(this._calendar, (object) dateTime))
        return new DateTime?(dateTime);
      DatePickerDateValidationErrorEventArgs e = new DatePickerDateValidationErrorEventArgs((Exception) new ArgumentOutOfRangeException(nameof (text), SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidValue)), text);
      this.OnDateValidationError(e);
      if (e.ThrowException)
        throw e.Exception;
    }
    catch (FormatException ex)
    {
      DatePickerDateValidationErrorEventArgs e = new DatePickerDateValidationErrorEventArgs((Exception) ex, text);
      this.OnDateValidationError(e);
      if (e.ThrowException)
      {
        if (e.Exception != null)
          throw e.Exception;
      }
    }
    return new DateTime?();
  }

  private bool ProcessDatePickerKey(KeyEventArgs e)
  {
    switch (e.Key)
    {
      case Key.Return:
        this.SetSelectedDate();
        return true;
      case Key.System:
        if (e.SystemKey == Key.Down && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        {
          this.TogglePopUp();
          return true;
        }
        break;
    }
    return false;
  }

  private void SetSelectedDate()
  {
    if (this._textBox != null)
    {
      if (!string.IsNullOrEmpty(this._textBox.Text))
      {
        string text = this._textBox.Text;
        if (this.SelectedDate.HasValue && this.DateTimeToString(this.SelectedDate.Value) == text)
          return;
        DateTime? nullable = this.SetTextBoxValue(text);
        if (this.SelectedDate.Equals((object) nullable))
          return;
        this.SelectedDate = nullable;
        this.DisplayDate = nullable.Value;
      }
      else
      {
        if (!this.SelectedDate.HasValue)
          return;
        this.SelectedDate = new DateTime?();
      }
    }
    else
    {
      DateTime? nullable = this.SetTextBoxValue(this._defaultText);
      if (this.SelectedDate.Equals((object) nullable))
        return;
      this.SelectedDate = nullable;
    }
  }

  private DateTime? SetTextBoxValue(string s)
  {
    if (string.IsNullOrEmpty(s))
    {
      this.SetValue(DatePicker.TextProperty, (object) s);
      return this.SelectedDate;
    }
    DateTime? text = this.ParseText(s);
    if (text.HasValue)
    {
      this.SetValue(DatePicker.TextProperty, (object) this.DateTimeToString(text.Value));
      return text;
    }
    if (this.SelectedDate.HasValue)
    {
      string str = this.DateTimeToString(this.SelectedDate.Value);
      this.SetValue(DatePicker.TextProperty, (object) str);
      return this.SelectedDate;
    }
    this.SetWaterMarkText();
    return new DateTime?();
  }

  private void SetWaterMarkText()
  {
    if (this._textBox == null)
      return;
    DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture((FrameworkElement) this));
    this.SetTextInternal(string.Empty);
    this._defaultText = string.Empty;
    switch (this.SelectedDateFormat)
    {
      case DatePickerFormat.Long:
        this._textBox.Watermark = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, SR.Get(SRID.DatePicker_WatermarkText), (object) dateFormat.LongDatePattern.ToString());
        break;
      case DatePickerFormat.Short:
        this._textBox.Watermark = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, SR.Get(SRID.DatePicker_WatermarkText), (object) dateFormat.ShortDatePattern.ToString());
        break;
    }
  }

  private void TextBox_LostFocus(object sender, RoutedEventArgs e) => this.SetSelectedDate();

  private void TextBox_KeyDown(object sender, KeyEventArgs e)
  {
    e.Handled = this.ProcessDatePickerKey(e) || e.Handled;
  }

  private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
  {
    this.SetValueNoCallback(DatePicker.TextProperty, (object) this._textBox.Text);
  }

  private void UpdateDisabledVisual()
  {
    if (!this.IsEnabled)
      VisualStates.GoToState((Control) this, true, "Disabled", "Normal");
    else
      VisualStates.GoToState((Control) this, true, "Normal");
  }
}
