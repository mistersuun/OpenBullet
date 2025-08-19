// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.CalendarItem
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = "PART_Root", Type = typeof (FrameworkElement))]
[TemplatePart(Name = "PART_HeaderButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_PreviousButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_NextButton", Type = typeof (Button))]
[TemplatePart(Name = "DayTitleTemplate", Type = typeof (DataTemplate))]
[TemplatePart(Name = "PART_MonthView", Type = typeof (Grid))]
[TemplatePart(Name = "PART_YearView", Type = typeof (Grid))]
[TemplatePart(Name = "PART_DisabledVisual", Type = typeof (FrameworkElement))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
public sealed class CalendarItem : Control
{
  private const string ElementRoot = "PART_Root";
  private const string ElementHeaderButton = "PART_HeaderButton";
  private const string ElementPreviousButton = "PART_PreviousButton";
  private const string ElementNextButton = "PART_NextButton";
  private const string ElementDayTitleTemplate = "DayTitleTemplate";
  private const string ElementMonthView = "PART_MonthView";
  private const string ElementYearView = "PART_YearView";
  private const string ElementDisabledVisual = "PART_DisabledVisual";
  private const int COLS = 7;
  private const int ROWS = 7;
  private const int YEAR_COLS = 4;
  private const int YEAR_ROWS = 3;
  private const int NUMBER_OF_DAYS_IN_WEEK = 7;
  private System.Globalization.Calendar _calendar = (System.Globalization.Calendar) new GregorianCalendar();
  private DataTemplate _dayTitleTemplate;
  private FrameworkElement _disabledVisual;
  private Button _headerButton;
  private Grid _monthView;
  private Button _nextButton;
  private Button _previousButton;
  private Grid _yearView;
  private bool _isMonthPressed;
  private bool _isDayPressed;

  static CalendarItem()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CalendarItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CalendarItem)));
    UIElement.FocusableProperty.OverrideMetadata(typeof (CalendarItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (CalendarItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Once));
    KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof (CalendarItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.Contained));
  }

  internal Grid MonthView => this._monthView;

  internal Microsoft.Windows.Controls.Calendar Owner { get; set; }

  internal Grid YearView => this._yearView;

  private Microsoft.Windows.Controls.CalendarMode DisplayMode
  {
    get => this.Owner == null ? Microsoft.Windows.Controls.CalendarMode.Month : this.Owner.DisplayMode;
  }

  private Button HeaderButton => this._headerButton;

  private Button NextButton => this._nextButton;

  private Button PreviousButton => this._previousButton;

  private DateTime DisplayDate => this.Owner == null ? DateTime.Today : this.Owner.DisplayDate;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._previousButton != null)
      this._previousButton.Click -= new RoutedEventHandler(this.PreviousButton_Click);
    if (this._nextButton != null)
      this._nextButton.Click -= new RoutedEventHandler(this.NextButton_Click);
    if (this._headerButton != null)
      this._headerButton.Click -= new RoutedEventHandler(this.HeaderButton_Click);
    this._monthView = this.GetTemplateChild("PART_MonthView") as Grid;
    this._yearView = this.GetTemplateChild("PART_YearView") as Grid;
    this._previousButton = this.GetTemplateChild("PART_PreviousButton") as Button;
    this._nextButton = this.GetTemplateChild("PART_NextButton") as Button;
    this._headerButton = this.GetTemplateChild("PART_HeaderButton") as Button;
    this._disabledVisual = this.GetTemplateChild("PART_DisabledVisual") as FrameworkElement;
    this._dayTitleTemplate = (DataTemplate) null;
    if (this.Template != null && this.Template.Resources.Contains((object) "DayTitleTemplate"))
      this._dayTitleTemplate = this.Template.Resources[(object) "DayTitleTemplate"] as DataTemplate;
    if (this._previousButton != null)
    {
      if (this._previousButton.Content == null)
        this._previousButton.Content = (object) Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.Calendar_PreviousButtonName);
      this._previousButton.Click += new RoutedEventHandler(this.PreviousButton_Click);
    }
    if (this._nextButton != null)
    {
      if (this._nextButton.Content == null)
        this._nextButton.Content = (object) Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.Calendar_NextButtonName);
      this._nextButton.Click += new RoutedEventHandler(this.NextButton_Click);
    }
    if (this._headerButton != null)
      this._headerButton.Click += new RoutedEventHandler(this.HeaderButton_Click);
    this.PopulateGrids();
    if (this.Owner != null)
    {
      switch (this.Owner.DisplayMode)
      {
        case Microsoft.Windows.Controls.CalendarMode.Month:
          this.UpdateMonthMode();
          break;
        case Microsoft.Windows.Controls.CalendarMode.Year:
          this.UpdateYearMode();
          break;
        case Microsoft.Windows.Controls.CalendarMode.Decade:
          this.UpdateDecadeMode();
          break;
      }
    }
    else
      this.UpdateMonthMode();
  }

  protected override void OnMouseUp(MouseButtonEventArgs e)
  {
    base.OnMouseUp(e);
    if (this.IsMouseCaptured)
      this.ReleaseMouseCapture();
    this._isMonthPressed = false;
    this._isDayPressed = false;
    if (e.Handled || this.Owner.DisplayMode != Microsoft.Windows.Controls.CalendarMode.Month || !this.Owner.HoverEnd.HasValue)
      return;
    this.FinishSelection(this.Owner.HoverEnd.Value);
  }

  protected override void OnLostMouseCapture(MouseEventArgs e)
  {
    base.OnLostMouseCapture(e);
    if (this.IsMouseCaptured)
      return;
    this._isDayPressed = false;
    this._isMonthPressed = false;
  }

  internal void UpdateDecadeMode()
  {
    int decadeForDecadeMode = this.GetDecadeForDecadeMode(this.Owner == null ? DateTime.Today : this.Owner.DisplayYear);
    int decadeEnd = decadeForDecadeMode + 9;
    this.SetDecadeModeHeaderButton(decadeForDecadeMode);
    this.SetDecadeModePreviousButton(decadeForDecadeMode);
    this.SetDecadeModeNextButton(decadeEnd);
    if (this._yearView == null)
      return;
    this.SetYearButtons(decadeForDecadeMode, decadeEnd);
  }

  internal void UpdateMonthMode()
  {
    this.SetMonthModeHeaderButton();
    this.SetMonthModePreviousButton();
    this.SetMonthModeNextButton();
    if (this._monthView == null)
      return;
    this.SetMonthModeDayTitles();
    this.SetMonthModeCalendarDayButtons();
    this.AddMonthModeHighlight();
  }

  internal void UpdateYearMode()
  {
    this.SetYearModeHeaderButton();
    this.SetYearModePreviousButton();
    this.SetYearModeNextButton();
    if (this._yearView == null)
      return;
    this.SetYearModeMonthButtons();
  }

  internal IEnumerable<CalendarDayButton> GetCalendarDayButtons()
  {
    int count = 49;
    if (this.MonthView != null)
    {
      UIElementCollection dayButtonsHost = this.MonthView.Children;
      for (int childIndex = 7; childIndex < count; ++childIndex)
      {
        if (dayButtonsHost[childIndex] is CalendarDayButton b)
          yield return b;
      }
    }
  }

  internal CalendarDayButton GetFocusedCalendarDayButton()
  {
    foreach (CalendarDayButton calendarDayButton in this.GetCalendarDayButtons())
    {
      if (calendarDayButton != null && calendarDayButton.IsFocused)
        return calendarDayButton;
    }
    return (CalendarDayButton) null;
  }

  internal CalendarDayButton GetCalendarDayButton(DateTime date)
  {
    foreach (CalendarDayButton calendarDayButton in this.GetCalendarDayButtons())
    {
      if (calendarDayButton != null && calendarDayButton.DataContext is DateTime && Microsoft.Windows.Controls.DateTimeHelper.CompareDays(date, (DateTime) calendarDayButton.DataContext) == 0)
        return calendarDayButton;
    }
    return (CalendarDayButton) null;
  }

  internal CalendarButton GetCalendarButton(DateTime date, Microsoft.Windows.Controls.CalendarMode mode)
  {
    foreach (CalendarButton calendarButton in this.GetCalendarButtons())
    {
      if (calendarButton != null && calendarButton.DataContext is DateTime)
      {
        if (mode == Microsoft.Windows.Controls.CalendarMode.Year)
        {
          if (Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(date, (DateTime) calendarButton.DataContext) == 0)
            return calendarButton;
        }
        else if (date.Year == ((DateTime) calendarButton.DataContext).Year)
          return calendarButton;
      }
    }
    return (CalendarButton) null;
  }

  internal CalendarButton GetFocusedCalendarButton()
  {
    foreach (CalendarButton calendarButton in this.GetCalendarButtons())
    {
      if (calendarButton != null && calendarButton.IsFocused)
        return calendarButton;
    }
    return (CalendarButton) null;
  }

  private IEnumerable<CalendarButton> GetCalendarButtons()
  {
    foreach (UIElement element in this.YearView.Children)
    {
      if (element is CalendarButton b)
        yield return b;
    }
  }

  internal void FocusDate(DateTime date)
  {
    FrameworkElement frameworkElement = (FrameworkElement) null;
    switch (this.DisplayMode)
    {
      case Microsoft.Windows.Controls.CalendarMode.Month:
        frameworkElement = (FrameworkElement) this.GetCalendarDayButton(date);
        break;
      case Microsoft.Windows.Controls.CalendarMode.Year:
      case Microsoft.Windows.Controls.CalendarMode.Decade:
        frameworkElement = (FrameworkElement) this.GetCalendarButton(date, this.DisplayMode);
        break;
    }
    if (frameworkElement == null || frameworkElement.IsFocused)
      return;
    frameworkElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
  }

  private int GetDecadeForDecadeMode(DateTime selectedYear)
  {
    int decadeForDecadeMode = Microsoft.Windows.Controls.DateTimeHelper.DecadeOfDate(selectedYear);
    if (this._isMonthPressed && this._yearView != null)
    {
      UIElementCollection children = this._yearView.Children;
      int count = children.Count;
      if (count > 0 && children[0] is CalendarButton calendarButton1 && calendarButton1.DataContext is DateTime && ((DateTime) calendarButton1.DataContext).Year == selectedYear.Year)
        return decadeForDecadeMode + 10;
      if (count > 1 && children[count - 1] is CalendarButton calendarButton2 && calendarButton2.DataContext is DateTime && ((DateTime) calendarButton2.DataContext).Year == selectedYear.Year)
        return decadeForDecadeMode - 10;
    }
    return decadeForDecadeMode;
  }

  private void EndDrag(bool ctrl, DateTime selectedDate)
  {
    if (this.Owner == null)
      return;
    this.Owner.CurrentDate = selectedDate;
    if (!this.Owner.HoverStart.HasValue)
      return;
    if (ctrl && DateTime.Compare(this.Owner.HoverStart.Value, selectedDate) == 0 && (this.Owner.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.SingleDate || this.Owner.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.MultipleRange))
      this.Owner.SelectedDates.Toggle(selectedDate);
    else
      this.Owner.SelectedDates.AddRangeInternal(this.Owner.HoverStart.Value, selectedDate);
    this.Owner.OnDayClick(selectedDate);
  }

  private void CellOrMonth_PreviewKeyDown(object sender, RoutedEventArgs e)
  {
    if (this.Owner == null)
      return;
    this.Owner.OnDayOrMonthPreviewKeyDown(e);
  }

  private void Cell_Clicked(object sender, RoutedEventArgs e)
  {
    if (this.Owner == null)
      return;
    CalendarDayButton calendarDayButton = sender as CalendarDayButton;
    if (!(calendarDayButton.DataContext is DateTime) || calendarDayButton.IsBlackedOut)
      return;
    DateTime dataContext = (DateTime) calendarDayButton.DataContext;
    bool ctrl;
    bool shift;
    KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
    switch (this.Owner.SelectionMode)
    {
      case Microsoft.Windows.Controls.CalendarSelectionMode.SingleDate:
        if (!ctrl)
        {
          this.Owner.SelectedDate = new DateTime?(dataContext);
          break;
        }
        this.Owner.SelectedDates.Toggle(dataContext);
        break;
      case Microsoft.Windows.Controls.CalendarSelectionMode.SingleRange:
        DateTime? nullable = new DateTime?(this.Owner.CurrentDate);
        this.Owner.SelectedDates.ClearInternal(true);
        if (shift && nullable.HasValue)
        {
          this.Owner.SelectedDates.AddRangeInternal(nullable.Value, dataContext);
          break;
        }
        this.Owner.SelectedDate = new DateTime?(dataContext);
        this.Owner.HoverStart = new DateTime?();
        this.Owner.HoverEnd = new DateTime?();
        break;
      case Microsoft.Windows.Controls.CalendarSelectionMode.MultipleRange:
        if (!ctrl)
          this.Owner.SelectedDates.ClearInternal(true);
        if (shift)
        {
          this.Owner.SelectedDates.AddRangeInternal(this.Owner.CurrentDate, dataContext);
          break;
        }
        if (!ctrl)
        {
          this.Owner.SelectedDate = new DateTime?(dataContext);
          break;
        }
        this.Owner.SelectedDates.Toggle(dataContext);
        this.Owner.HoverStart = new DateTime?();
        this.Owner.HoverEnd = new DateTime?();
        break;
    }
    this.Owner.OnDayClick(dataContext);
  }

  private void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (!(sender is CalendarDayButton calendarDayButton) || this.Owner == null || !(calendarDayButton.DataContext is DateTime))
      return;
    if (calendarDayButton.IsBlackedOut)
    {
      this.Owner.HoverStart = new DateTime?();
    }
    else
    {
      this._isDayPressed = true;
      Mouse.Capture((IInputElement) this, CaptureMode.SubTree);
      calendarDayButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
      bool ctrl;
      bool shift;
      KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
      DateTime dataContext = (DateTime) calendarDayButton.DataContext;
      switch (this.Owner.SelectionMode)
      {
        case Microsoft.Windows.Controls.CalendarSelectionMode.SingleDate:
          this.Owner.DatePickerDisplayDateFlag = true;
          if (!ctrl)
          {
            this.Owner.SelectedDate = new DateTime?(dataContext);
            break;
          }
          this.Owner.SelectedDates.Toggle(dataContext);
          break;
        case Microsoft.Windows.Controls.CalendarSelectionMode.SingleRange:
          this.Owner.SelectedDates.ClearInternal();
          if (shift)
          {
            if (!this.Owner.HoverStart.HasValue)
            {
              this.Owner.HoverStart = this.Owner.HoverEnd = new DateTime?(this.Owner.CurrentDate);
              break;
            }
            break;
          }
          this.Owner.HoverStart = this.Owner.HoverEnd = new DateTime?(dataContext);
          break;
        case Microsoft.Windows.Controls.CalendarSelectionMode.MultipleRange:
          if (!ctrl)
            this.Owner.SelectedDates.ClearInternal();
          if (shift)
          {
            if (!this.Owner.HoverStart.HasValue)
            {
              this.Owner.HoverStart = this.Owner.HoverEnd = new DateTime?(this.Owner.CurrentDate);
              break;
            }
            break;
          }
          this.Owner.HoverStart = this.Owner.HoverEnd = new DateTime?(dataContext);
          break;
      }
      this.Owner.CurrentDate = dataContext;
      this.Owner.UpdateCellItems();
    }
  }

  private void Cell_MouseEnter(object sender, MouseEventArgs e)
  {
    if (!(sender is CalendarDayButton calendarDayButton) || calendarDayButton.IsBlackedOut || e.LeftButton != MouseButtonState.Pressed || !this._isDayPressed)
      return;
    calendarDayButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    if (this.Owner == null || !(calendarDayButton.DataContext is DateTime))
      return;
    DateTime dataContext = (DateTime) calendarDayButton.DataContext;
    if (this.Owner.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.SingleDate)
    {
      this.Owner.DatePickerDisplayDateFlag = true;
      Microsoft.Windows.Controls.Calendar owner1 = this.Owner;
      Microsoft.Windows.Controls.Calendar owner2 = this.Owner;
      DateTime? nullable1 = new DateTime?();
      DateTime? nullable2;
      DateTime? nullable3 = nullable2 = nullable1;
      owner2.HoverEnd = nullable2;
      DateTime? nullable4 = nullable3;
      owner1.HoverStart = nullable4;
      if (this.Owner.SelectedDates.Count == 0)
        this.Owner.SelectedDates.Add(dataContext);
      else
        this.Owner.SelectedDates[0] = dataContext;
    }
    else
    {
      this.Owner.HoverEnd = new DateTime?(dataContext);
      this.Owner.CurrentDate = dataContext;
      this.Owner.UpdateCellItems();
    }
  }

  private void Cell_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (!(sender is CalendarDayButton calendarDayButton) || this.Owner == null)
      return;
    if (!calendarDayButton.IsBlackedOut)
      this.Owner.OnDayButtonMouseUp(e);
    if (!(calendarDayButton.DataContext is DateTime))
      return;
    this.FinishSelection((DateTime) calendarDayButton.DataContext);
    e.Handled = true;
  }

  private void FinishSelection(DateTime selectedDate)
  {
    bool ctrl;
    KeyboardHelper.GetMetaKeyState(out ctrl, out bool _);
    if (this.Owner.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.None || this.Owner.SelectionMode == Microsoft.Windows.Controls.CalendarSelectionMode.SingleDate)
      this.Owner.OnDayClick(selectedDate);
    else if (this.Owner.HoverStart.HasValue)
    {
      switch (this.Owner.SelectionMode)
      {
        case Microsoft.Windows.Controls.CalendarSelectionMode.SingleRange:
          this.Owner.SelectedDates.ClearInternal();
          this.EndDrag(ctrl, selectedDate);
          break;
        case Microsoft.Windows.Controls.CalendarSelectionMode.MultipleRange:
          this.EndDrag(ctrl, selectedDate);
          break;
      }
    }
    else
    {
      CalendarDayButton calendarDayButton = this.GetCalendarDayButton(selectedDate);
      if (calendarDayButton == null || !calendarDayButton.IsInactive || !calendarDayButton.IsBlackedOut)
        return;
      this.Owner.OnDayClick(selectedDate);
    }
  }

  private void Month_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (!(sender is CalendarButton b))
      return;
    this._isMonthPressed = true;
    Mouse.Capture((IInputElement) this, CaptureMode.SubTree);
    if (this.Owner == null)
      return;
    this.Owner.OnCalendarButtonPressed(b, false);
  }

  private void Month_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (!(sender is CalendarButton b) || this.Owner == null)
      return;
    this.Owner.OnCalendarButtonPressed(b, true);
  }

  private void Month_MouseEnter(object sender, MouseEventArgs e)
  {
    if (!(sender is CalendarButton b) || !this._isMonthPressed || this.Owner == null)
      return;
    this.Owner.OnCalendarButtonPressed(b, false);
  }

  private void Month_Clicked(object sender, RoutedEventArgs e)
  {
    if (!(sender is CalendarButton b))
      return;
    this.Owner.OnCalendarButtonPressed(b, true);
  }

  private void HeaderButton_Click(object sender, RoutedEventArgs e)
  {
    if (this.Owner == null)
      return;
    this.Owner.DisplayMode = this.Owner.DisplayMode != Microsoft.Windows.Controls.CalendarMode.Month ? Microsoft.Windows.Controls.CalendarMode.Decade : Microsoft.Windows.Controls.CalendarMode.Year;
    this.FocusDate(this.DisplayDate);
  }

  private void PreviousButton_Click(object sender, RoutedEventArgs e)
  {
    if (this.Owner == null)
      return;
    this.Owner.OnPreviousClick();
  }

  private void NextButton_Click(object sender, RoutedEventArgs e)
  {
    if (this.Owner == null)
      return;
    this.Owner.OnNextClick();
  }

  private void PopulateGrids()
  {
    if (this._monthView != null)
    {
      if (this._dayTitleTemplate != null)
      {
        for (int index = 0; index < 7; ++index)
        {
          FrameworkElement element = (FrameworkElement) this._dayTitleTemplate.LoadContent();
          element.SetValue(Grid.RowProperty, (object) 0);
          element.SetValue(Grid.ColumnProperty, (object) index);
          this._monthView.Children.Add((UIElement) element);
        }
      }
      for (int index1 = 1; index1 < 7; ++index1)
      {
        for (int index2 = 0; index2 < 7; ++index2)
        {
          CalendarDayButton element = new CalendarDayButton();
          element.Owner = this.Owner;
          element.SetValue(Grid.RowProperty, (object) index1);
          element.SetValue(Grid.ColumnProperty, (object) index2);
          element.SetBinding(FrameworkElement.StyleProperty, this.GetOwnerBinding("CalendarDayButtonStyle"));
          element.AddHandler(UIElement.MouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.Cell_MouseLeftButtonDown), true);
          element.AddHandler(UIElement.MouseLeftButtonUpEvent, (Delegate) new MouseButtonEventHandler(this.Cell_MouseLeftButtonUp), true);
          element.AddHandler(UIElement.MouseEnterEvent, (Delegate) new MouseEventHandler(this.Cell_MouseEnter), true);
          element.Click += new RoutedEventHandler(this.Cell_Clicked);
          element.AddHandler(UIElement.PreviewKeyDownEvent, (Delegate) new RoutedEventHandler(this.CellOrMonth_PreviewKeyDown), true);
          this._monthView.Children.Add((UIElement) element);
        }
      }
    }
    if (this._yearView == null)
      return;
    int num = 0;
    for (int index3 = 0; index3 < 3; ++index3)
    {
      for (int index4 = 0; index4 < 4; ++index4)
      {
        CalendarButton element = new CalendarButton();
        element.Owner = this.Owner;
        element.SetValue(Grid.RowProperty, (object) index3);
        element.SetValue(Grid.ColumnProperty, (object) index4);
        element.SetBinding(FrameworkElement.StyleProperty, this.GetOwnerBinding("CalendarButtonStyle"));
        element.AddHandler(UIElement.MouseLeftButtonDownEvent, (Delegate) new MouseButtonEventHandler(this.Month_MouseLeftButtonDown), true);
        element.AddHandler(UIElement.MouseLeftButtonUpEvent, (Delegate) new MouseButtonEventHandler(this.Month_MouseLeftButtonUp), true);
        element.AddHandler(UIElement.MouseEnterEvent, (Delegate) new MouseEventHandler(this.Month_MouseEnter), true);
        element.AddHandler(UIElement.PreviewKeyDownEvent, (Delegate) new RoutedEventHandler(this.CellOrMonth_PreviewKeyDown), true);
        element.Click += new RoutedEventHandler(this.Month_Clicked);
        this._yearView.Children.Add((UIElement) element);
        ++num;
      }
    }
  }

  private void SetMonthModeDayTitles()
  {
    if (this._monthView == null)
      return;
    string[] shortestDayNames = Microsoft.Windows.Controls.DateTimeHelper.GetDateFormat(Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this)).ShortestDayNames;
    for (int index = 0; index < 7; ++index)
    {
      if (this._monthView.Children[index] is FrameworkElement child && shortestDayNames != null && shortestDayNames.Length > 0)
        child.DataContext = this.Owner == null ? (object) shortestDayNames[(int) (index + Microsoft.Windows.Controls.DateTimeHelper.GetDateFormat(Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this)).FirstDayOfWeek) % shortestDayNames.Length] : (object) shortestDayNames[(int) (index + this.Owner.FirstDayOfWeek) % shortestDayNames.Length];
    }
  }

  private void SetMonthModeCalendarDayButtons()
  {
    DateTime dateTime1 = Microsoft.Windows.Controls.DateTimeHelper.DiscardDayTime(this.DisplayDate);
    int fromPreviousMonth = this.GetNumberOfDisplayedDaysFromPreviousMonth(dateTime1);
    bool flag1 = Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(dateTime1, DateTime.MinValue) <= 0;
    bool flag2 = Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(dateTime1, DateTime.MaxValue) >= 0;
    int daysInMonth = this._calendar.GetDaysInMonth(dateTime1.Year, dateTime1.Month);
    CultureInfo culture = Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this);
    int num = 49;
    for (int index = 7; index < num; ++index)
    {
      CalendarDayButton child = this._monthView.Children[index] as CalendarDayButton;
      int days = index - fromPreviousMonth - 7;
      if ((!flag1 || days >= 0) && (!flag2 || days < daysInMonth))
      {
        DateTime dateTime2 = this._calendar.AddDays(dateTime1, days);
        this.SetMonthModeDayButtonState(child, new DateTime?(dateTime2));
        child.DataContext = (object) dateTime2;
        child.SetContentInternal(Microsoft.Windows.Controls.DateTimeHelper.ToDayString(new DateTime?(dateTime2), culture));
      }
      else
      {
        this.SetMonthModeDayButtonState(child, new DateTime?());
        child.DataContext = (object) null;
        child.SetContentInternal(Microsoft.Windows.Controls.DateTimeHelper.ToDayString(new DateTime?(), culture));
      }
    }
  }

  private void SetMonthModeDayButtonState(CalendarDayButton childButton, DateTime? dateToAdd)
  {
    if (this.Owner == null)
      return;
    if (dateToAdd.HasValue)
    {
      childButton.Visibility = Visibility.Visible;
      if (Microsoft.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, this.Owner.DisplayDateStartInternal) < 0 || Microsoft.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, this.Owner.DisplayDateEndInternal) > 0)
      {
        childButton.IsEnabled = false;
        childButton.Visibility = Visibility.Hidden;
      }
      else
      {
        childButton.IsEnabled = true;
        childButton.SetValue(CalendarDayButton.IsBlackedOutPropertyKey, (object) this.Owner.BlackoutDates.Contains(dateToAdd.Value));
        childButton.SetValue(CalendarDayButton.IsInactivePropertyKey, (object) (Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(dateToAdd.Value, this.Owner.DisplayDateInternal) != 0));
        if (Microsoft.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, DateTime.Today) == 0)
        {
          childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, (object) true);
          childButton.ChangeVisualState(true);
        }
        else
          childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, (object) false);
        bool flag = false;
        foreach (DateTime selectedDate in (Collection<DateTime>) this.Owner.SelectedDates)
          flag |= Microsoft.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, selectedDate) == 0;
        childButton.SetValue(CalendarDayButton.IsSelectedPropertyKey, (object) flag);
      }
    }
    else
    {
      childButton.Visibility = Visibility.Hidden;
      childButton.IsEnabled = false;
      childButton.SetValue(CalendarDayButton.IsBlackedOutPropertyKey, (object) false);
      childButton.SetValue(CalendarDayButton.IsInactivePropertyKey, (object) true);
      childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, (object) false);
      childButton.SetValue(CalendarDayButton.IsSelectedPropertyKey, (object) false);
    }
  }

  private void AddMonthModeHighlight()
  {
    Microsoft.Windows.Controls.Calendar owner = this.Owner;
    if (owner == null)
      return;
    if (owner.HoverStart.HasValue && owner.HoverEnd.HasValue)
    {
      DateTime start = owner.HoverEnd.Value;
      DateTime end = owner.HoverEnd.Value;
      int num1 = Microsoft.Windows.Controls.DateTimeHelper.CompareDays(owner.HoverEnd.Value, owner.HoverStart.Value);
      if (num1 < 0)
        end = this.Owner.HoverStart.Value;
      else
        start = this.Owner.HoverStart.Value;
      int num2 = 49;
      for (int index = 7; index < num2; ++index)
      {
        CalendarDayButton child = this._monthView.Children[index] as CalendarDayButton;
        if (child.DataContext is DateTime)
        {
          DateTime dataContext = (DateTime) child.DataContext;
          child.SetValue(CalendarDayButton.IsHighlightedPropertyKey, (object) (bool) (num1 == 0 ? 0 : (Microsoft.Windows.Controls.DateTimeHelper.InRange(dataContext, start, end) ? 1 : 0)));
        }
        else
          child.SetValue(CalendarDayButton.IsHighlightedPropertyKey, (object) false);
      }
    }
    else
    {
      int num = 49;
      for (int index = 7; index < num; ++index)
        (this._monthView.Children[index] as CalendarDayButton).SetValue(CalendarDayButton.IsHighlightedPropertyKey, (object) false);
    }
  }

  private void SetMonthModeHeaderButton()
  {
    if (this._headerButton == null)
      return;
    this._headerButton.Content = (object) Microsoft.Windows.Controls.DateTimeHelper.ToYearMonthPatternString(new DateTime?(this.DisplayDate), Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this));
    if (this.Owner == null)
      return;
    this._headerButton.IsEnabled = true;
  }

  private void SetMonthModeNextButton()
  {
    if (this.Owner == null || this._nextButton == null)
      return;
    DateTime dateTime = Microsoft.Windows.Controls.DateTimeHelper.DiscardDayTime(this.DisplayDate);
    if (Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(dateTime, DateTime.MaxValue) == 0)
      this._nextButton.IsEnabled = false;
    else
      this._nextButton.IsEnabled = Microsoft.Windows.Controls.DateTimeHelper.CompareDays(this.Owner.DisplayDateEndInternal, this._calendar.AddMonths(dateTime, 1)) > -1;
  }

  private void SetMonthModePreviousButton()
  {
    if (this.Owner == null || this._previousButton == null)
      return;
    this._previousButton.IsEnabled = Microsoft.Windows.Controls.DateTimeHelper.CompareDays(this.Owner.DisplayDateStartInternal, Microsoft.Windows.Controls.DateTimeHelper.DiscardDayTime(this.DisplayDate)) < 0;
  }

  private void SetYearButtons(int decade, int decadeEnd)
  {
    int num = -1;
    foreach (object child in this._yearView.Children)
    {
      CalendarButton calendarButton = child as CalendarButton;
      int year = decade + num;
      if (year <= DateTime.MaxValue.Year && year >= DateTime.MinValue.Year)
      {
        DateTime dateTime = new DateTime(year, 1, 1);
        calendarButton.DataContext = (object) dateTime;
        calendarButton.SetContentInternal(Microsoft.Windows.Controls.DateTimeHelper.ToYearString(new DateTime?(dateTime), Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this)));
        calendarButton.Visibility = Visibility.Visible;
        if (this.Owner != null)
        {
          calendarButton.HasSelectedDays = this.Owner.DisplayDate.Year == year;
          if (year < this.Owner.DisplayDateStartInternal.Year || year > this.Owner.DisplayDateEndInternal.Year)
          {
            calendarButton.IsEnabled = false;
            calendarButton.Opacity = 0.0;
          }
          else
          {
            calendarButton.IsEnabled = true;
            calendarButton.Opacity = 1.0;
          }
        }
        calendarButton.IsInactive = year < decade || year > decadeEnd;
      }
      else
      {
        calendarButton.DataContext = (object) null;
        calendarButton.IsEnabled = false;
        calendarButton.Opacity = 0.0;
      }
      ++num;
    }
  }

  private void SetYearModeMonthButtons()
  {
    int num = 0;
    foreach (object child in this._yearView.Children)
    {
      CalendarButton calendarButton = child as CalendarButton;
      DateTime dt1 = new DateTime(this.DisplayDate.Year, num + 1, 1);
      calendarButton.DataContext = (object) dt1;
      calendarButton.SetContentInternal(Microsoft.Windows.Controls.DateTimeHelper.ToAbbreviatedMonthString(new DateTime?(dt1), Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this)));
      calendarButton.Visibility = Visibility.Visible;
      if (this.Owner != null)
      {
        calendarButton.HasSelectedDays = Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(dt1, this.Owner.DisplayDateInternal) == 0;
        if (Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(dt1, this.Owner.DisplayDateStartInternal) < 0 || Microsoft.Windows.Controls.DateTimeHelper.CompareYearMonth(dt1, this.Owner.DisplayDateEndInternal) > 0)
        {
          calendarButton.IsEnabled = false;
          calendarButton.Opacity = 0.0;
        }
        else
        {
          calendarButton.IsEnabled = true;
          calendarButton.Opacity = 1.0;
        }
      }
      calendarButton.IsInactive = false;
      ++num;
    }
  }

  private void SetYearModeHeaderButton()
  {
    if (this._headerButton == null)
      return;
    this._headerButton.IsEnabled = true;
    this._headerButton.Content = (object) Microsoft.Windows.Controls.DateTimeHelper.ToYearString(new DateTime?(this.DisplayDate), Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this));
  }

  private void SetYearModeNextButton()
  {
    if (this.Owner == null || this._nextButton == null)
      return;
    this._nextButton.IsEnabled = this.Owner.DisplayDateEndInternal.Year != this.DisplayDate.Year;
  }

  private void SetYearModePreviousButton()
  {
    if (this.Owner == null || this._previousButton == null)
      return;
    this._previousButton.IsEnabled = this.Owner.DisplayDateStartInternal.Year != this.DisplayDate.Year;
  }

  private void SetDecadeModeHeaderButton(int decade)
  {
    if (this._headerButton == null)
      return;
    this._headerButton.Content = (object) Microsoft.Windows.Controls.DateTimeHelper.ToDecadeRangeString(decade, Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this));
    this._headerButton.IsEnabled = false;
  }

  private void SetDecadeModeNextButton(int decadeEnd)
  {
    if (this.Owner == null || this._nextButton == null)
      return;
    this._nextButton.IsEnabled = this.Owner.DisplayDateEndInternal.Year > decadeEnd;
  }

  private void SetDecadeModePreviousButton(int decade)
  {
    if (this.Owner == null || this._previousButton == null)
      return;
    this._previousButton.IsEnabled = decade > this.Owner.DisplayDateStartInternal.Year;
  }

  private int GetNumberOfDisplayedDaysFromPreviousMonth(DateTime firstOfMonth)
  {
    DayOfWeek dayOfWeek = this._calendar.GetDayOfWeek(firstOfMonth);
    int num = this.Owner == null ? (dayOfWeek - Microsoft.Windows.Controls.DateTimeHelper.GetDateFormat(Microsoft.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement) this)).FirstDayOfWeek + 7) % 7 : (dayOfWeek - this.Owner.FirstDayOfWeek + 7) % 7;
    return num == 0 ? 7 : num;
  }

  private BindingBase GetOwnerBinding(string propertyName)
  {
    return (BindingBase) new Binding(propertyName)
    {
      Source = (object) this.Owner
    };
  }
}
