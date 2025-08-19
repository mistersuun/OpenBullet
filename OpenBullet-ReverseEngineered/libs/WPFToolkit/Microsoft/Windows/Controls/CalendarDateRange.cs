﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.CalendarDateRange
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Windows.Controls;

public sealed class CalendarDateRange : INotifyPropertyChanged
{
  private DateTime _end;
  private DateTime _start;

  public CalendarDateRange()
    : this(DateTime.MinValue, DateTime.MaxValue)
  {
  }

  public CalendarDateRange(DateTime day)
    : this(day, day)
  {
  }

  public CalendarDateRange(DateTime start, DateTime end)
  {
    this._start = start;
    this._end = end;
  }

  public event PropertyChangedEventHandler PropertyChanged;

  public DateTime End
  {
    get => CalendarDateRange.CoerceEnd(this._start, this._end);
    set
    {
      DateTime end = CalendarDateRange.CoerceEnd(this._start, value);
      if (!(end != this.End))
        return;
      this.OnChanging(new CalendarDateRangeChangingEventArgs(this._start, end));
      this._end = value;
      this.OnPropertyChanged(new PropertyChangedEventArgs(nameof (End)));
    }
  }

  public DateTime Start
  {
    get => this._start;
    set
    {
      if (!(this._start != value))
        return;
      DateTime end1 = this.End;
      DateTime end2 = CalendarDateRange.CoerceEnd(value, this._end);
      this.OnChanging(new CalendarDateRangeChangingEventArgs(value, end2));
      this._start = value;
      this.OnPropertyChanged(new PropertyChangedEventArgs(nameof (Start)));
      if (!(end2 != end1))
        return;
      this.OnPropertyChanged(new PropertyChangedEventArgs("End"));
    }
  }

  internal event EventHandler<CalendarDateRangeChangingEventArgs> Changing;

  internal bool ContainsAny(CalendarDateRange range)
  {
    return range.End >= this.Start && this.End >= range.Start;
  }

  private void OnChanging(CalendarDateRangeChangingEventArgs e)
  {
    EventHandler<CalendarDateRangeChangingEventArgs> changing = this.Changing;
    if (changing == null)
      return;
    changing((object) this, e);
  }

  private void OnPropertyChanged(PropertyChangedEventArgs e)
  {
    PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    if (propertyChanged == null)
      return;
    propertyChanged((object) this, e);
  }

  private static DateTime CoerceEnd(DateTime start, DateTime end)
  {
    return DateTime.Compare(start, end) > 0 ? start : end;
  }
}
