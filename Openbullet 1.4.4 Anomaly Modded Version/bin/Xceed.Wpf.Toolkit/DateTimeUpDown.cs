// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.DateTimeUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class DateTimeUpDown : DateTimeUpDownBase<DateTime?>
{
  private DateTime? _lastValidDate;
  private bool _setKindInternal;
  public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(nameof (Format), typeof (DateTimeFormat), typeof (DateTimeUpDown), (PropertyMetadata) new UIPropertyMetadata((object) DateTimeFormat.FullDateTime, new PropertyChangedCallback(DateTimeUpDown.OnFormatChanged)));
  public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register(nameof (FormatString), typeof (string), typeof (DateTimeUpDown), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(DateTimeUpDown.OnFormatStringChanged)), new ValidateValueCallback(DateTimeUpDown.IsFormatStringValid));
  public static readonly DependencyProperty KindProperty = DependencyProperty.Register(nameof (Kind), typeof (DateTimeKind), typeof (DateTimeUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTimeKind.Unspecified, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DateTimeUpDown.OnKindChanged)));

  public DateTimeFormat Format
  {
    get => (DateTimeFormat) this.GetValue(DateTimeUpDown.FormatProperty);
    set => this.SetValue(DateTimeUpDown.FormatProperty, (object) value);
  }

  private static void OnFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DateTimeUpDown dateTimeUpDown))
      return;
    dateTimeUpDown.OnFormatChanged((DateTimeFormat) e.OldValue, (DateTimeFormat) e.NewValue);
  }

  protected virtual void OnFormatChanged(DateTimeFormat oldValue, DateTimeFormat newValue)
  {
    this.FormatUpdated();
  }

  public string FormatString
  {
    get => (string) this.GetValue(DateTimeUpDown.FormatStringProperty);
    set => this.SetValue(DateTimeUpDown.FormatStringProperty, (object) value);
  }

  internal static bool IsFormatStringValid(object value)
  {
    try
    {
      DateTime.MinValue.ToString((string) value, (IFormatProvider) CultureInfo.CurrentCulture);
    }
    catch
    {
      return false;
    }
    return true;
  }

  private static void OnFormatStringChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DateTimeUpDown dateTimeUpDown))
      return;
    dateTimeUpDown.OnFormatStringChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnFormatStringChanged(string oldValue, string newValue)
  {
    this.FormatUpdated();
  }

  public DateTimeKind Kind
  {
    get => (DateTimeKind) this.GetValue(DateTimeUpDown.KindProperty);
    set => this.SetValue(DateTimeUpDown.KindProperty, (object) value);
  }

  private static void OnKindChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DateTimeUpDown dateTimeUpDown))
      return;
    dateTimeUpDown.OnKindChanged((DateTimeKind) e.OldValue, (DateTimeKind) e.NewValue);
  }

  protected virtual void OnKindChanged(DateTimeKind oldValue, DateTimeKind newValue)
  {
    if (this._setKindInternal || !this.Value.HasValue || !this.IsInitialized)
      return;
    this.Value = new DateTime?(this.ConvertToKind(this.Value.Value, newValue));
  }

  private void SetKindInternal(DateTimeKind kind)
  {
    this._setKindInternal = true;
    try
    {
      this.SetCurrentValue(DateTimeUpDown.KindProperty, (object) kind);
    }
    finally
    {
      this._setKindInternal = false;
    }
  }

  internal DateTime ContextNow => DateTimeUtilities.GetContextNow(this.Kind);

  static DateTimeUpDown()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DateTimeUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DateTimeUpDown)));
    UpDownBase<DateTime?>.MaximumProperty.OverrideMetadata(typeof (DateTimeUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.MaxValue));
    UpDownBase<DateTime?>.MinimumProperty.OverrideMetadata(typeof (DateTimeUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) DateTime.MinValue));
    UpDownBase<DateTime?>.UpdateValueOnEnterKeyProperty.OverrideMetadata(typeof (DateTimeUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  }

  public DateTimeUpDown() => this.Loaded += new RoutedEventHandler(this.DateTimeUpDown_Loaded);

  public override bool CommitInput()
  {
    int num = this.SyncTextAndValueProperties(true, this.Text) ? 1 : 0;
    this._lastValidDate = this.Value;
    return num != 0;
  }

  protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
  {
    this.FormatUpdated();
  }

  protected override void OnIncrement()
  {
    if (!this.IsCurrentValueValid())
      return;
    this.Increment(this.Step);
  }

  protected override void OnDecrement()
  {
    if (!this.IsCurrentValueValid())
      return;
    this.Increment(-this.Step);
  }

  protected override void OnTextChanged(string previousValue, string currentValue)
  {
    if (!this._processTextChanged)
      return;
    base.OnTextChanged(previousValue, currentValue);
  }

  protected override DateTime? ConvertTextToValue(string text)
  {
    if (string.IsNullOrEmpty(text))
      return new DateTime?();
    DateTime result;
    this.TryParseDateTime(text, out result);
    if (this.Kind != DateTimeKind.Unspecified)
      result = this.ConvertToKind(result, this.Kind);
    if (this.ClipValueToMinMax)
      return this.GetClippedMinMaxValue(new DateTime?(result));
    this.ValidateDefaultMinMax(new DateTime?(result));
    return new DateTime?(result);
  }

  protected override string ConvertValueToText()
  {
    return !this.Value.HasValue ? string.Empty : this.Value.Value.ToString(this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo);
  }

  protected override void SetValidSpinDirection()
  {
    ValidSpinDirections validSpinDirections = ValidSpinDirections.None;
    if (!this.IsReadOnly)
    {
      if (this.IsLowerThan(this.Value, this.Maximum) || !this.Value.HasValue || !this.Maximum.HasValue)
        validSpinDirections |= ValidSpinDirections.Increase;
      if (this.IsGreaterThan(this.Value, this.Minimum) || !this.Value.HasValue || !this.Minimum.HasValue)
        validSpinDirections |= ValidSpinDirections.Decrease;
    }
    if (this.Spinner == null)
      return;
    this.Spinner.ValidSpinDirection = validSpinDirections;
  }

  protected override object OnCoerceValue(object newValue)
  {
    DateTime? nullable = (DateTime?) base.OnCoerceValue(newValue);
    if (nullable.HasValue && this.IsInitialized)
      this.SetKindInternal(nullable.Value.Kind);
    return (object) nullable;
  }

  protected override void OnValueChanged(DateTime? oldValue, DateTime? newValue)
  {
    DateTimeInfo dateTimeInfo = this._selectedDateTimeInfo ?? (this.CurrentDateTimePart != DateTimePart.Other ? this.GetDateTimeInfo(this.CurrentDateTimePart) : this._dateTimeInfoList[0]);
    if (newValue.HasValue)
      this.ParseValueIntoDateTimeInfo(this.Value);
    base.OnValueChanged(oldValue, newValue);
    if (!this._isTextChangedFromUI)
      this._lastValidDate = newValue;
    if (this.TextBox == null)
      return;
    this._fireSelectionChangedEvent = false;
    this.TextBox.Select(dateTimeInfo.StartPosition, dateTimeInfo.Length);
    this._fireSelectionChangedEvent = true;
  }

  protected override bool IsCurrentValueValid()
  {
    return string.IsNullOrEmpty(this.TextBox.Text) || this.TryParseDateTime(this.TextBox.Text, out DateTime _);
  }

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    if (!this.Value.HasValue)
      return;
    DateTimeKind kind = this.Value.Value.Kind;
    if (kind == this.Kind)
      return;
    if (this.Kind == DateTimeKind.Unspecified)
      this.SetKindInternal(kind);
    else
      this.Value = new DateTime?(this.ConvertToKind(this.Value.Value, this.Kind));
  }

  protected override void PerformMouseSelection()
  {
    if (this.UpdateValueOnEnterKey)
      this.ParseValueIntoDateTimeInfo(this.ConvertTextToValue(this.TextBox.Text));
    base.PerformMouseSelection();
  }

  protected internal override void PerformKeyboardSelection(int nextSelectionStart)
  {
    if (this.UpdateValueOnEnterKey)
      this.ParseValueIntoDateTimeInfo(this.ConvertTextToValue(this.TextBox.Text));
    base.PerformKeyboardSelection(nextSelectionStart);
  }

  protected override void InitializeDateTimeInfoList(DateTime? value)
  {
    this._dateTimeInfoList.Clear();
    this._selectedDateTimeInfo = (DateTimeInfo) null;
    string format = this.GetFormatString(this.Format);
    if (string.IsNullOrEmpty(format))
      return;
    int num1;
    for (; format.Length > 0; format = format.Substring(num1))
    {
      num1 = DateTimeUpDown.GetElementLengthByFormat(format);
      DateTimeInfo dateTimeInfo1 = (DateTimeInfo) null;
      char ch = format[0];
      switch (ch)
      {
        case '"':
        case '\'':
          int num2 = format.IndexOf(format[0], 1);
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = true,
            Type = DateTimePart.Other,
            Length = 1,
            Content = format.Substring(1, Math.Max(1, num2 - 1))
          };
          num1 = Math.Max(1, num2 + 1);
          break;
        case 'D':
        case 'd':
          string str1 = format.Substring(0, num1);
          if (num1 == 1)
            str1 = "%" + str1;
          if (num1 > 2)
          {
            dateTimeInfo1 = new DateTimeInfo()
            {
              IsReadOnly = true,
              Type = DateTimePart.DayName,
              Length = num1,
              Format = str1
            };
            break;
          }
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Day,
            Length = num1,
            Format = str1
          };
          break;
        case 'F':
        case 'f':
          string str2 = format.Substring(0, num1);
          if (num1 == 1)
            str2 = "%" + str2;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Millisecond,
            Length = num1,
            Format = str2
          };
          break;
        case 'H':
          string str3 = format.Substring(0, num1);
          if (num1 == 1)
            str3 = "%" + str3;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Hour24,
            Length = num1,
            Format = str3
          };
          break;
        case 'M':
          string str4 = format.Substring(0, num1);
          if (num1 == 1)
            str4 = "%" + str4;
          if (num1 >= 3)
          {
            dateTimeInfo1 = new DateTimeInfo()
            {
              IsReadOnly = false,
              Type = DateTimePart.MonthName,
              Length = num1,
              Format = str4
            };
            break;
          }
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Month,
            Length = num1,
            Format = str4
          };
          break;
        case 'S':
        case 's':
          string str5 = format.Substring(0, num1);
          if (num1 == 1)
            str5 = "%" + str5;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Second,
            Length = num1,
            Format = str5
          };
          break;
        case 'T':
        case 't':
          string str6 = format.Substring(0, num1);
          if (num1 == 1)
            str6 = "%" + str6;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.AmPmDesignator,
            Length = num1,
            Format = str6
          };
          break;
        case 'Y':
        case 'y':
          string str7 = format.Substring(0, num1);
          if (num1 == 1)
            str7 = "%" + str7;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Year,
            Length = num1,
            Format = str7
          };
          break;
        case '\\':
          if (format.Length >= 2)
          {
            dateTimeInfo1 = new DateTimeInfo()
            {
              IsReadOnly = true,
              Content = format.Substring(1, 1),
              Length = 1,
              Type = DateTimePart.Other
            };
            num1 = 2;
            break;
          }
          break;
        case 'g':
          string str8 = format.Substring(0, num1);
          if (num1 == 1)
          {
            string str9 = "%" + str8;
          }
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = true,
            Type = DateTimePart.Period,
            Length = num1,
            Format = format.Substring(0, num1)
          };
          break;
        case 'h':
          string str10 = format.Substring(0, num1);
          if (num1 == 1)
            str10 = "%" + str10;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Hour12,
            Length = num1,
            Format = str10
          };
          break;
        case 'm':
          string str11 = format.Substring(0, num1);
          if (num1 == 1)
            str11 = "%" + str11;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = false,
            Type = DateTimePart.Minute,
            Length = num1,
            Format = str11
          };
          break;
        case 'z':
          string str12 = format.Substring(0, num1);
          if (num1 == 1)
            str12 = "%" + str12;
          dateTimeInfo1 = new DateTimeInfo()
          {
            IsReadOnly = true,
            Type = DateTimePart.TimeZone,
            Length = num1,
            Format = str12
          };
          break;
        default:
          num1 = 1;
          DateTimeInfo dateTimeInfo2 = new DateTimeInfo();
          dateTimeInfo2.IsReadOnly = true;
          dateTimeInfo2.Length = 1;
          ch = format[0];
          dateTimeInfo2.Content = ch.ToString();
          dateTimeInfo2.Type = DateTimePart.Other;
          dateTimeInfo1 = dateTimeInfo2;
          break;
      }
      this._dateTimeInfoList.Add(dateTimeInfo1);
    }
  }

  protected override bool IsLowerThan(DateTime? value1, DateTime? value2)
  {
    return value1.HasValue && value2.HasValue && value1.Value < value2.Value;
  }

  protected override bool IsGreaterThan(DateTime? value1, DateTime? value2)
  {
    return value1.HasValue && value2.HasValue && value1.Value > value2.Value;
  }

  protected override void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue)
  {
    throw new NotSupportedException("DateTimeUpDown controls do not support modifying UpdateValueOnEnterKey property.");
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    if (e.Key == Key.Escape)
    {
      this.SyncTextAndValueProperties(false, (string) null);
      e.Handled = true;
    }
    base.OnKeyDown(e);
  }

  public void SelectAll()
  {
    this._fireSelectionChangedEvent = false;
    this.TextBox.SelectAll();
    this._fireSelectionChangedEvent = true;
  }

  private void FormatUpdated()
  {
    this.InitializeDateTimeInfoList(this.Value);
    if (this.Value.HasValue)
      this.ParseValueIntoDateTimeInfo(this.Value);
    this._processTextChanged = false;
    this.SyncTextAndValueProperties(false, (string) null);
    this._processTextChanged = true;
  }

  private static int GetElementLengthByFormat(string format)
  {
    for (int index = 1; index < format.Length; ++index)
    {
      if (string.Compare(format[index].ToString(), format[0].ToString(), false) != 0)
        return index;
    }
    return format.Length;
  }

  private void Increment(int step)
  {
    this._fireSelectionChangedEvent = false;
    DateTime? currentDateTime = this.ConvertTextToValue(this.TextBox.Text);
    if (currentDateTime.HasValue)
    {
      DateTime? nullable = this.UpdateDateTime(currentDateTime, step);
      if (!nullable.HasValue)
        return;
      this.TextBox.Text = nullable.Value.ToString(this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo);
    }
    else
      this.TextBox.Text = this.DefaultValue.HasValue ? this.DefaultValue.Value.ToString(this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo) : this.ContextNow.ToString(this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo);
    if (this.TextBox != null)
    {
      DateTimeInfo dateTimeInfo = this._selectedDateTimeInfo ?? (this.CurrentDateTimePart != DateTimePart.Other ? this.GetDateTimeInfo(this.CurrentDateTimePart) : this._dateTimeInfoList[0]);
      this.ParseValueIntoDateTimeInfo(this.ConvertTextToValue(this.TextBox.Text));
      this.TextBox.Select(dateTimeInfo.StartPosition, dateTimeInfo.Length);
    }
    this._fireSelectionChangedEvent = true;
    this.SyncTextAndValueProperties(true, this.Text);
  }

  private void ParseValueIntoDateTimeInfo(DateTime? newDate)
  {
    string text = string.Empty;
    this._dateTimeInfoList.ForEach((Action<DateTimeInfo>) (info =>
    {
      if (info.Format == null)
      {
        info.StartPosition = text.Length;
        info.Length = info.Content.Length;
        text += info.Content;
      }
      else
      {
        if (!newDate.HasValue)
          return;
        DateTime dateTime = newDate.Value;
        info.StartPosition = text.Length;
        info.Content = dateTime.ToString(info.Format, (IFormatProvider) this.CultureInfo.DateTimeFormat);
        info.Length = info.Content.Length;
        text += info.Content;
      }
    }));
  }

  internal string GetFormatString(DateTimeFormat dateTimeFormat)
  {
    switch (dateTimeFormat)
    {
      case DateTimeFormat.Custom:
        switch (this.FormatString)
        {
          case "D":
            return this.CultureInfo.DateTimeFormat.LongDatePattern;
          case "F":
            return this.CultureInfo.DateTimeFormat.FullDateTimePattern;
          case "G":
            return $"{this.CultureInfo.DateTimeFormat.ShortDatePattern} {this.CultureInfo.DateTimeFormat.LongTimePattern}";
          case "T":
            return this.CultureInfo.DateTimeFormat.LongTimePattern;
          case "d":
            return this.CultureInfo.DateTimeFormat.ShortDatePattern;
          case "f":
            return $"{this.CultureInfo.DateTimeFormat.LongDatePattern} {this.CultureInfo.DateTimeFormat.ShortTimePattern}";
          case "g":
            return $"{this.CultureInfo.DateTimeFormat.ShortDatePattern} {this.CultureInfo.DateTimeFormat.ShortTimePattern}";
          case "m":
            return this.CultureInfo.DateTimeFormat.MonthDayPattern;
          case "r":
            return this.CultureInfo.DateTimeFormat.RFC1123Pattern;
          case "s":
            return this.CultureInfo.DateTimeFormat.SortableDateTimePattern;
          case "t":
            return this.CultureInfo.DateTimeFormat.ShortTimePattern;
          case "u":
            return this.CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
          case "y":
            return this.CultureInfo.DateTimeFormat.YearMonthPattern;
          default:
            return this.FormatString;
        }
      case DateTimeFormat.FullDateTime:
        return this.CultureInfo.DateTimeFormat.FullDateTimePattern;
      case DateTimeFormat.LongDate:
        return this.CultureInfo.DateTimeFormat.LongDatePattern;
      case DateTimeFormat.LongTime:
        return this.CultureInfo.DateTimeFormat.LongTimePattern;
      case DateTimeFormat.MonthDay:
        return this.CultureInfo.DateTimeFormat.MonthDayPattern;
      case DateTimeFormat.RFC1123:
        return this.CultureInfo.DateTimeFormat.RFC1123Pattern;
      case DateTimeFormat.ShortDate:
        return this.CultureInfo.DateTimeFormat.ShortDatePattern;
      case DateTimeFormat.ShortTime:
        return this.CultureInfo.DateTimeFormat.ShortTimePattern;
      case DateTimeFormat.SortableDateTime:
        return this.CultureInfo.DateTimeFormat.SortableDateTimePattern;
      case DateTimeFormat.UniversalSortableDateTime:
        return this.CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
      case DateTimeFormat.YearMonth:
        return this.CultureInfo.DateTimeFormat.YearMonthPattern;
      default:
        throw new ArgumentException("Not a supported format");
    }
  }

  private DateTime? UpdateDateTime(DateTime? currentDateTime, int value)
  {
    DateTimeInfo dateTimeInfo = this._selectedDateTimeInfo ?? (this.CurrentDateTimePart != DateTimePart.Other ? this.GetDateTimeInfo(this.CurrentDateTimePart) : this._dateTimeInfoList[0]);
    DateTime? nullable = new DateTime?();
    try
    {
      switch (dateTimeInfo.Type)
      {
        case DateTimePart.Day:
        case DateTimePart.DayName:
          nullable = new DateTime?(currentDateTime.Value.AddDays((double) value));
          break;
        case DateTimePart.AmPmDesignator:
          nullable = new DateTime?(currentDateTime.Value.AddHours((double) (value * 12)));
          break;
        case DateTimePart.Millisecond:
          nullable = new DateTime?(currentDateTime.Value.AddMilliseconds((double) value));
          break;
        case DateTimePart.Hour12:
        case DateTimePart.Hour24:
          nullable = new DateTime?(currentDateTime.Value.AddHours((double) value));
          break;
        case DateTimePart.Minute:
          nullable = new DateTime?(currentDateTime.Value.AddMinutes((double) value));
          break;
        case DateTimePart.Month:
        case DateTimePart.MonthName:
          nullable = new DateTime?(currentDateTime.Value.AddMonths(value));
          break;
        case DateTimePart.Second:
          nullable = new DateTime?(currentDateTime.Value.AddSeconds((double) value));
          break;
        case DateTimePart.Year:
          nullable = new DateTime?(currentDateTime.Value.AddYears(value));
          break;
      }
    }
    catch
    {
    }
    return this.CoerceValueMinMax(nullable);
  }

  private bool TryParseDateTime(string text, out DateTime result)
  {
    result = this.ContextNow;
    DateTime currentDate = this.ContextNow;
    bool dateTime;
    try
    {
      currentDate = this.Value.HasValue ? this.Value.Value : DateTime.Parse(this.ContextNow.ToString(), (IFormatProvider) this.CultureInfo.DateTimeFormat);
      dateTime = DateTimeParser.TryParse(text, this.GetFormatString(this.Format), currentDate, this.CultureInfo, out result);
    }
    catch (FormatException ex)
    {
      dateTime = false;
    }
    if (!dateTime)
      dateTime = DateTime.TryParseExact(text, this.GetFormatString(this.Format), (IFormatProvider) this.CultureInfo, DateTimeStyles.None, out result);
    if (!dateTime)
      result = this._lastValidDate.HasValue ? this._lastValidDate.Value : currentDate;
    return dateTime;
  }

  private DateTime ConvertToKind(DateTime dateTime, DateTimeKind kind)
  {
    if (kind == dateTime.Kind)
      return dateTime;
    if (dateTime.Kind == DateTimeKind.Unspecified || kind == DateTimeKind.Unspecified)
      return DateTime.SpecifyKind(dateTime, kind);
    return kind != DateTimeKind.Local ? dateTime.ToUniversalTime() : dateTime.ToLocalTime();
  }

  private void DateTimeUpDown_Loaded(object sender, RoutedEventArgs e)
  {
    if (this.Format == DateTimeFormat.Custom && string.IsNullOrEmpty(this.FormatString))
      throw new InvalidOperationException("A FormatString is necessary when Format is set to Custom.");
  }
}
