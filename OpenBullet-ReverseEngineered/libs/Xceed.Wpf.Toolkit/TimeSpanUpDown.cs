// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.TimeSpanUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class TimeSpanUpDown : DateTimeUpDownBase<TimeSpan?>
{
  public static readonly DependencyProperty FractionalSecondsDigitsCountProperty = DependencyProperty.Register(nameof (FractionalSecondsDigitsCount), typeof (int), typeof (TimeSpanUpDown), (PropertyMetadata) new UIPropertyMetadata((object) 0, new PropertyChangedCallback(TimeSpanUpDown.OnFractionalSecondsDigitsCountChanged), new CoerceValueCallback(TimeSpanUpDown.OnCoerceFractionalSecondsDigitsCount)));
  public static readonly DependencyProperty ShowDaysProperty = DependencyProperty.Register(nameof (ShowDays), typeof (bool), typeof (TimeSpanUpDown), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(TimeSpanUpDown.OnShowDaysChanged)));
  public static readonly DependencyProperty ShowSecondsProperty = DependencyProperty.Register(nameof (ShowSeconds), typeof (bool), typeof (TimeSpanUpDown), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(TimeSpanUpDown.OnShowSecondsChanged)));

  static TimeSpanUpDown()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TimeSpanUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (TimeSpanUpDown)));
    UpDownBase<TimeSpan?>.MaximumProperty.OverrideMetadata(typeof (TimeSpanUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) TimeSpan.MaxValue));
    UpDownBase<TimeSpan?>.MinimumProperty.OverrideMetadata(typeof (TimeSpanUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) TimeSpan.MinValue));
    UpDownBase<TimeSpan?>.DefaultValueProperty.OverrideMetadata(typeof (TimeSpanUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) TimeSpan.Zero));
  }

  public TimeSpanUpDown()
  {
    DataObject.AddPastingHandler((DependencyObject) this, new DataObjectPastingEventHandler(this.OnPasting));
  }

  public int FractionalSecondsDigitsCount
  {
    get => (int) this.GetValue(TimeSpanUpDown.FractionalSecondsDigitsCountProperty);
    set => this.SetValue(TimeSpanUpDown.FractionalSecondsDigitsCountProperty, (object) value);
  }

  private static object OnCoerceFractionalSecondsDigitsCount(DependencyObject o, object value)
  {
    if (o is TimeSpanUpDown)
    {
      int num = (int) value;
      if (num < 0 || num > 3)
        throw new ArgumentException("Fractional seconds digits count must be between 0 and 3.");
    }
    return value;
  }

  private static void OnFractionalSecondsDigitsCountChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is TimeSpanUpDown timeSpanUpDown))
      return;
    timeSpanUpDown.OnFractionalSecondsDigitsCountChanged((int) e.OldValue, (int) e.NewValue);
  }

  protected virtual void OnFractionalSecondsDigitsCountChanged(int oldValue, int newValue)
  {
    this.UpdateValue();
  }

  public bool ShowDays
  {
    get => (bool) this.GetValue(TimeSpanUpDown.ShowDaysProperty);
    set => this.SetValue(TimeSpanUpDown.ShowDaysProperty, (object) value);
  }

  private static void OnShowDaysChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is TimeSpanUpDown timeSpanUpDown))
      return;
    timeSpanUpDown.OnShowDaysChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnShowDaysChanged(bool oldValue, bool newValue) => this.UpdateValue();

  public bool ShowSeconds
  {
    get => (bool) this.GetValue(TimeSpanUpDown.ShowSecondsProperty);
    set => this.SetValue(TimeSpanUpDown.ShowSecondsProperty, (object) value);
  }

  private static void OnShowSecondsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is TimeSpanUpDown timeSpanUpDown))
      return;
    timeSpanUpDown.OnShowSecondsChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnShowSecondsChanged(bool oldValue, bool newValue) => this.UpdateValue();

  protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
  {
    this.InitializeDateTimeInfoList(this.UpdateValueOnEnterKey ? (this.TextBox != null ? this.ConvertTextToValue(this.TextBox.Text) : new TimeSpan?()) : this.Value);
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

  protected override void OnIncrement() => this.Increment(this.Step);

  protected override void OnDecrement() => this.Increment(-this.Step);

  protected override string ConvertValueToText()
  {
    return !this.Value.HasValue ? string.Empty : this.ParseValueIntoTimeSpanInfo(this.Value, true);
  }

  protected override TimeSpan? ConvertTextToValue(string text)
  {
    if (string.IsNullOrEmpty(text))
      return new TimeSpan?();
    TimeSpan timeSpan = TimeSpan.MinValue;
    if (this.ShowDays)
    {
      timeSpan = TimeSpan.Parse(text);
    }
    else
    {
      List<char> list = text.Where<char>((Func<char, bool>) (x => x == ':' || x == '.')).ToList<char>();
      string[] source = text.Split(':', '.');
      if (((IEnumerable<string>) source).Count<string>() >= 2 && !((IEnumerable<string>) source).Any<string>((Func<string, bool>) (x => string.IsNullOrEmpty(x))))
      {
        bool flag = list.Count<char>() > 1 && list.Last<char>() == '.';
        timeSpan = new TimeSpan(0, int.Parse(source[0].Replace("-", "")), int.Parse(source[1].Replace("-", "")), this.ShowSeconds ? int.Parse(source[2].Replace("-", "")) : 0, flag ? int.Parse(((IEnumerable<string>) source).Last<string>().Replace("-", "")) : 0);
        if (text.StartsWith("-"))
          timeSpan = timeSpan.Negate();
      }
    }
    if (this.ClipValueToMinMax)
      return this.GetClippedMinMaxValue(new TimeSpan?(timeSpan));
    this.ValidateDefaultMinMax(new TimeSpan?(timeSpan));
    return new TimeSpan?(timeSpan);
  }

  protected override void OnPreviewTextInput(TextCompositionEventArgs e)
  {
    e.Handled = !this.IsNumber(e.Text);
    base.OnPreviewTextInput(e);
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if (e.Key == Key.Space)
      e.Handled = true;
    base.OnPreviewKeyDown(e);
  }

  protected override void OnTextChanged(string previousValue, string currentValue)
  {
    if (!this._processTextChanged)
      return;
    if (string.IsNullOrEmpty(currentValue))
    {
      if (this.UpdateValueOnEnterKey)
        return;
      this.Value = new TimeSpan?();
    }
    else
    {
      TimeSpan? nullable = this.Value;
      if (nullable.HasValue)
      {
        nullable = this.Value;
        TimeSpan timeSpan = nullable.Value;
      }
      TimeSpan result;
      if (!TimeSpan.TryParse(currentValue, out result))
      {
        List<char> list = currentValue.Where<char>((Func<char, bool>) (x => x == ':' || x == '.')).ToList<char>();
        string[] source = currentValue.Split(':', '.');
        if (((IEnumerable<string>) source).Count<string>() >= 2 && !((IEnumerable<string>) source).Any<string>((Func<string, bool>) (x => string.IsNullOrEmpty(x))))
        {
          bool flag1 = list.First<char>() == '.';
          bool flag2 = list.Count<char>() > 1 && list.Last<char>() == '.';
          result = new TimeSpan(flag1 ? int.Parse(source[0]) : 0, flag1 ? int.Parse(source[1]) : int.Parse(source[0]), flag1 ? int.Parse(source[2]) : int.Parse(source[1]), !flag1 || !this.ShowSeconds ? (this.ShowSeconds ? int.Parse(source[2]) : 0) : int.Parse(source[3]), flag2 ? int.Parse(((IEnumerable<string>) source).Last<string>()) : 0);
        }
      }
      currentValue = result.ToString();
      if ((!this._isTextChangedFromUI || this.UpdateValueOnEnterKey) && this._isTextChangedFromUI)
        return;
      this.SyncTextAndValueProperties(true, currentValue);
    }
  }

  protected override void OnValueChanged(TimeSpan? oldValue, TimeSpan? newValue)
  {
    if (newValue.HasValue)
      this.InitializeDateTimeInfoList(this.UpdateValueOnEnterKey ? (this.TextBox != null ? this.ConvertTextToValue(this.TextBox.Text) : new TimeSpan?()) : this.Value);
    base.OnValueChanged(oldValue, newValue);
  }

  protected override void PerformMouseSelection()
  {
    this.InitializeDateTimeInfoList(this.UpdateValueOnEnterKey ? (this.TextBox != null ? this.ConvertTextToValue(this.TextBox.Text) : new TimeSpan?()) : this.Value);
    base.PerformMouseSelection();
  }

  protected override void InitializeDateTimeInfoList(TimeSpan? value)
  {
    DateTimeInfo dateTimeInfo1 = this._dateTimeInfoList.FirstOrDefault<DateTimeInfo>((Func<DateTimeInfo, bool>) (x => x.Type == DateTimePart.Day));
    bool flag1 = dateTimeInfo1 != null;
    DateTimeInfo dateTimeInfo2 = this._dateTimeInfoList.FirstOrDefault<DateTimeInfo>((Func<DateTimeInfo, bool>) (x => x.Type == DateTimePart.Other));
    bool flag2 = dateTimeInfo2 != null && dateTimeInfo2.Content == "-";
    this._dateTimeInfoList.Clear();
    if (value.HasValue && value.Value.TotalMilliseconds < 0.0)
    {
      this._dateTimeInfoList.Add(new DateTimeInfo()
      {
        Type = DateTimePart.Other,
        Length = 1,
        Content = "-",
        IsReadOnly = true
      });
      if (!flag2 && this.TextBox != null)
      {
        this._fireSelectionChangedEvent = false;
        ++this.TextBox.SelectionStart;
        this._fireSelectionChangedEvent = true;
      }
    }
    if (this.ShowDays)
    {
      if (value.HasValue && value.Value.Days != 0)
      {
        int length = Math.Abs(value.Value.Days).ToString().Length;
        this._dateTimeInfoList.Add(new DateTimeInfo()
        {
          Type = DateTimePart.Day,
          Length = length,
          Format = "dd"
        });
        this._dateTimeInfoList.Add(new DateTimeInfo()
        {
          Type = DateTimePart.Other,
          Length = 1,
          Content = ".",
          IsReadOnly = true
        });
        if (this.TextBox != null)
        {
          if (flag1 && length != dateTimeInfo1.Length && this._selectedDateTimeInfo.Type != DateTimePart.Day)
          {
            this._fireSelectionChangedEvent = false;
            this.TextBox.SelectionStart = Math.Max(0, this.TextBox.SelectionStart + (length - dateTimeInfo1.Length));
            this._fireSelectionChangedEvent = true;
          }
          else if (!flag1)
          {
            this._fireSelectionChangedEvent = false;
            this.TextBox.SelectionStart += length + 1;
            this._fireSelectionChangedEvent = true;
          }
        }
      }
      else if (flag1)
      {
        this._fireSelectionChangedEvent = false;
        this.TextBox.SelectionStart = Math.Max(flag2 ? 1 : 0, this.TextBox.SelectionStart - (dateTimeInfo1.Length + 1));
        this._fireSelectionChangedEvent = true;
      }
    }
    this._dateTimeInfoList.Add(new DateTimeInfo()
    {
      Type = DateTimePart.Hour24,
      Length = 2,
      Format = "hh"
    });
    this._dateTimeInfoList.Add(new DateTimeInfo()
    {
      Type = DateTimePart.Other,
      Length = 1,
      Content = ":",
      IsReadOnly = true
    });
    this._dateTimeInfoList.Add(new DateTimeInfo()
    {
      Type = DateTimePart.Minute,
      Length = 2,
      Format = "mm"
    });
    if (this.ShowSeconds)
    {
      this._dateTimeInfoList.Add(new DateTimeInfo()
      {
        Type = DateTimePart.Other,
        Length = 1,
        Content = ":",
        IsReadOnly = true
      });
      this._dateTimeInfoList.Add(new DateTimeInfo()
      {
        Type = DateTimePart.Second,
        Length = 2,
        Format = "ss"
      });
    }
    if (this.FractionalSecondsDigitsCount > 0)
    {
      this._dateTimeInfoList.Add(new DateTimeInfo()
      {
        Type = DateTimePart.Other,
        Length = 1,
        Content = ".",
        IsReadOnly = true
      });
      string str = new string('f', this.FractionalSecondsDigitsCount);
      if (str.Length == 1)
        str = "%" + str;
      this._dateTimeInfoList.Add(new DateTimeInfo()
      {
        Type = DateTimePart.Millisecond,
        Length = this.FractionalSecondsDigitsCount,
        Format = str
      });
    }
    if (!value.HasValue)
      return;
    this.ParseValueIntoTimeSpanInfo(value, true);
  }

  protected override bool IsLowerThan(TimeSpan? value1, TimeSpan? value2)
  {
    return value1.HasValue && value2.HasValue && value1.Value < value2.Value;
  }

  protected override bool IsGreaterThan(TimeSpan? value1, TimeSpan? value2)
  {
    return value1.HasValue && value2.HasValue && value1.Value > value2.Value;
  }

  private string ParseValueIntoTimeSpanInfo(TimeSpan? value, bool modifyInfo)
  {
    string text = string.Empty;
    this._dateTimeInfoList.ForEach((Action<DateTimeInfo>) (info =>
    {
      if (info.Format == null)
      {
        if (modifyInfo)
        {
          info.StartPosition = text.Length;
          info.Length = info.Content.Length;
        }
        text += info.Content;
      }
      else
      {
        TimeSpan timeSpan = TimeSpan.Parse(value.ToString());
        if (modifyInfo)
          info.StartPosition = text.Length;
        string str = this.ShowDays || timeSpan.Days == 0 || !(info.Format == "hh") ? timeSpan.ToString(info.Format, (IFormatProvider) this.CultureInfo.DateTimeFormat) : Math.Truncate(Math.Abs(timeSpan.TotalHours)).ToString();
        if (modifyInfo)
        {
          if (info.Format == "dd")
            str = Convert.ToInt32(str).ToString();
          info.Content = str;
          info.Length = info.Content.Length;
        }
        text += str;
      }
    }));
    return text;
  }

  private TimeSpan? UpdateTimeSpan(TimeSpan? currentValue, int value)
  {
    DateTimeInfo dateTimeInfo = this._selectedDateTimeInfo ?? (this.CurrentDateTimePart != DateTimePart.Other ? this.GetDateTimeInfo(this.CurrentDateTimePart) : (this._dateTimeInfoList[0].Content != "-" ? this._dateTimeInfoList[0] : this._dateTimeInfoList[1]));
    TimeSpan? nullable = new TimeSpan?();
    try
    {
      switch (dateTimeInfo.Type)
      {
        case DateTimePart.Day:
          nullable = new TimeSpan?(currentValue.Value.Add(new TimeSpan(value, 0, 0, 0, 0)));
          break;
        case DateTimePart.Millisecond:
          switch (this.FractionalSecondsDigitsCount)
          {
            case 1:
              value *= 100;
              break;
            case 2:
              value *= 10;
              break;
            default:
              value = value;
              break;
          }
          nullable = new TimeSpan?(currentValue.Value.Add(new TimeSpan(0, 0, 0, 0, value)));
          break;
        case DateTimePart.Hour24:
          nullable = new TimeSpan?(currentValue.Value.Add(new TimeSpan(0, value, 0, 0, 0)));
          break;
        case DateTimePart.Minute:
          nullable = new TimeSpan?(currentValue.Value.Add(new TimeSpan(0, 0, value, 0, 0)));
          break;
        case DateTimePart.Second:
          nullable = new TimeSpan?(currentValue.Value.Add(new TimeSpan(0, 0, 0, value, 0)));
          break;
      }
    }
    catch
    {
    }
    return this.CoerceValueMinMax(!nullable.HasValue || !nullable.HasValue ? nullable : new TimeSpan?(nullable.Value));
  }

  private void Increment(int step)
  {
    if (this.UpdateValueOnEnterKey)
    {
      TimeSpan? currentValue = this.ConvertTextToValue(this.TextBox.Text);
      TimeSpan? nullable = currentValue.HasValue ? this.UpdateTimeSpan(currentValue, step) : new TimeSpan?(this.DefaultValue ?? TimeSpan.Zero);
      if (!nullable.HasValue)
        return;
      this.InitializeDateTimeInfoList(nullable);
      int selectionStart = this.TextBox.SelectionStart;
      int selectionLength = this.TextBox.SelectionLength;
      this.TextBox.Text = this.ParseValueIntoTimeSpanInfo(nullable, false);
      this.TextBox.Select(selectionStart, selectionLength);
    }
    else
    {
      TimeSpan? defaultValue = this.Value;
      if (defaultValue.HasValue)
      {
        TimeSpan? nullable = this.UpdateTimeSpan(this.Value, step);
        if (!nullable.HasValue)
          return;
        this.InitializeDateTimeInfoList(nullable);
        int selectionStart = this.TextBox.SelectionStart;
        int selectionLength = this.TextBox.SelectionLength;
        this.Value = nullable;
        this.TextBox.Select(selectionStart, selectionLength);
      }
      else
      {
        defaultValue = this.DefaultValue;
        this.Value = new TimeSpan?(defaultValue ?? TimeSpan.Zero);
      }
    }
  }

  private bool IsNumber(string str)
  {
    foreach (char c in str)
    {
      if (!char.IsNumber(c))
        return false;
    }
    return true;
  }

  private void UpdateValue()
  {
    this.InitializeDateTimeInfoList(this.UpdateValueOnEnterKey ? (this.TextBox != null ? this.ConvertTextToValue(this.TextBox.Text) : new TimeSpan?()) : this.Value);
    this.SyncTextAndValueProperties(false, this.Text);
  }

  private void OnPasting(object sender, DataObjectPastingEventArgs e)
  {
    if (!e.DataObject.GetDataPresent(typeof (string)) || TimeSpan.TryParse(e.DataObject.GetData(typeof (string)) as string, out TimeSpan _))
      return;
    e.CancelCommand();
  }
}
