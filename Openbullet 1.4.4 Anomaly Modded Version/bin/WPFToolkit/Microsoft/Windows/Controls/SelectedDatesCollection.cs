﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.SelectedDatesCollection
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

public sealed class SelectedDatesCollection : ObservableCollection<DateTime>
{
  private Collection<DateTime> _addedItems;
  private Collection<DateTime> _removedItems;
  private Thread _dispatcherThread;
  private bool _isAddingRange;
  private Calendar _owner;
  private DateTime? _maximumDate;
  private DateTime? _minimumDate;

  public SelectedDatesCollection(Calendar owner)
  {
    this._dispatcherThread = Thread.CurrentThread;
    this._owner = owner;
    this._addedItems = new Collection<DateTime>();
    this._removedItems = new Collection<DateTime>();
  }

  internal DateTime? MinimumDate
  {
    get
    {
      if (this.Count < 1)
        return new DateTime?();
      if (!this._minimumDate.HasValue)
      {
        DateTime t2 = this[0];
        foreach (DateTime t1 in (Collection<DateTime>) this)
        {
          if (DateTime.Compare(t1, t2) < 0)
            t2 = t1;
        }
        this._maximumDate = new DateTime?(t2);
      }
      return this._minimumDate;
    }
  }

  internal DateTime? MaximumDate
  {
    get
    {
      if (this.Count < 1)
        return new DateTime?();
      if (!this._maximumDate.HasValue)
      {
        DateTime t2 = this[0];
        foreach (DateTime t1 in (Collection<DateTime>) this)
        {
          if (DateTime.Compare(t1, t2) > 0)
            t2 = t1;
        }
        this._maximumDate = new DateTime?(t2);
      }
      return this._maximumDate;
    }
  }

  public void AddRange(DateTime start, DateTime end)
  {
    this.BeginAddRange();
    if (this._owner.SelectionMode == CalendarSelectionMode.SingleRange && this.Count > 0)
      this.ClearInternal();
    foreach (DateTime dateTime in SelectedDatesCollection.GetDaysInRange(start, end))
      this.Add(dateTime);
    this.EndAddRange();
  }

  protected override void ClearItems()
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    this._owner.HoverStart = new DateTime?();
    this.ClearInternal(true);
  }

  protected override void InsertItem(int index, DateTime item)
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    if (this.Contains(item))
      return;
    Collection<DateTime> addedItems = new Collection<DateTime>();
    bool flag = this.CheckSelectionMode();
    if (!Calendar.IsValidDateSelection(this._owner, (object) item))
      throw new ArgumentOutOfRangeException(SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidValue));
    if (flag)
      index = 0;
    base.InsertItem(index, item);
    this.UpdateMinMax(item);
    if (index == 0 && (!this._owner.SelectedDate.HasValue || DateTime.Compare(this._owner.SelectedDate.Value, item) != 0))
      this._owner.SelectedDate = new DateTime?(item);
    if (!this._isAddingRange)
    {
      addedItems.Add(item);
      this.RaiseSelectionChanged((IList) this._removedItems, (IList) addedItems);
      this._removedItems.Clear();
      int num = DateTimeHelper.CompareYearMonth(item, this._owner.DisplayDateInternal);
      if (num >= 2 || num <= -2)
        return;
      this._owner.UpdateCellItems();
    }
    else
      this._addedItems.Add(item);
  }

  protected override void RemoveItem(int index)
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    if (index >= this.Count)
    {
      base.RemoveItem(index);
      this.ClearMinMax();
    }
    else
    {
      Collection<DateTime> addedItems = new Collection<DateTime>();
      Collection<DateTime> removedItems = new Collection<DateTime>();
      int num = DateTimeHelper.CompareYearMonth(this[index], this._owner.DisplayDateInternal);
      removedItems.Add(this[index]);
      base.RemoveItem(index);
      this.ClearMinMax();
      if (index == 0)
        this._owner.SelectedDate = this.Count <= 0 ? new DateTime?() : new DateTime?(this[0]);
      this.RaiseSelectionChanged((IList) removedItems, (IList) addedItems);
      if (num >= 2 || num <= -2)
        return;
      this._owner.UpdateCellItems();
    }
  }

  protected override void SetItem(int index, DateTime item)
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    if (this.Contains(item))
      return;
    Collection<DateTime> addedItems = new Collection<DateTime>();
    Collection<DateTime> removedItems = new Collection<DateTime>();
    if (index >= this.Count)
    {
      base.SetItem(index, item);
      this.UpdateMinMax(item);
    }
    else
    {
      if (DateTime.Compare(this[index], item) == 0 || !Calendar.IsValidDateSelection(this._owner, (object) item))
        return;
      removedItems.Add(this[index]);
      base.SetItem(index, item);
      this.UpdateMinMax(item);
      addedItems.Add(item);
      if (index == 0 && (!this._owner.SelectedDate.HasValue || DateTime.Compare(this._owner.SelectedDate.Value, item) != 0))
        this._owner.SelectedDate = new DateTime?(item);
      this.RaiseSelectionChanged((IList) removedItems, (IList) addedItems);
      int num = DateTimeHelper.CompareYearMonth(item, this._owner.DisplayDateInternal);
      if (num >= 2 || num <= -2)
        return;
      this._owner.UpdateCellItems();
    }
  }

  internal void AddRangeInternal(DateTime start, DateTime end)
  {
    this.BeginAddRange();
    DateTime dateTime1 = start;
    foreach (DateTime dateTime2 in SelectedDatesCollection.GetDaysInRange(start, end))
    {
      if (Calendar.IsValidDateSelection(this._owner, (object) dateTime2))
      {
        this.Add(dateTime2);
        dateTime1 = dateTime2;
      }
      else if (this._owner.SelectionMode == CalendarSelectionMode.SingleRange)
      {
        this._owner.CurrentDate = dateTime1;
        break;
      }
    }
    this.EndAddRange();
  }

  internal void ClearInternal() => this.ClearInternal(false);

  internal void ClearInternal(bool fireChangeNotification)
  {
    if (this.Count <= 0)
      return;
    foreach (DateTime dateTime in (Collection<DateTime>) this)
      this._removedItems.Add(dateTime);
    base.ClearItems();
    this.ClearMinMax();
    if (!fireChangeNotification)
      return;
    if (this._owner.SelectedDate.HasValue)
      this._owner.SelectedDate = new DateTime?();
    if (this._removedItems.Count > 0)
    {
      this.RaiseSelectionChanged((IList) this._removedItems, (IList) new Collection<DateTime>());
      this._removedItems.Clear();
    }
    this._owner.UpdateCellItems();
  }

  internal void Toggle(DateTime date)
  {
    if (!Calendar.IsValidDateSelection(this._owner, (object) date))
      return;
    switch (this._owner.SelectionMode)
    {
      case CalendarSelectionMode.SingleDate:
        if (!this._owner.SelectedDate.HasValue || DateTimeHelper.CompareDays(this._owner.SelectedDate.Value, date) != 0)
        {
          this._owner.SelectedDate = new DateTime?(date);
          break;
        }
        this._owner.SelectedDate = new DateTime?();
        break;
      case CalendarSelectionMode.MultipleRange:
        if (this.Remove(date))
          break;
        this.Add(date);
        break;
    }
  }

  private void RaiseSelectionChanged(IList removedItems, IList addedItems)
  {
    this._owner.OnSelectedDatesCollectionChanged((SelectionChangedEventArgs) new CalendarSelectionChangedEventArgs(Calendar.SelectedDatesChangedEvent, removedItems, addedItems));
  }

  private void BeginAddRange() => this._isAddingRange = true;

  private void EndAddRange()
  {
    this._isAddingRange = false;
    this.RaiseSelectionChanged((IList) this._removedItems, (IList) this._addedItems);
    this._removedItems.Clear();
    this._addedItems.Clear();
    this._owner.UpdateCellItems();
  }

  private bool CheckSelectionMode()
  {
    if (this._owner.SelectionMode == CalendarSelectionMode.None)
      throw new InvalidOperationException(SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidOperation));
    if (this._owner.SelectionMode == CalendarSelectionMode.SingleDate && this.Count > 0)
      throw new InvalidOperationException(SR.Get(SRID.Calendar_CheckSelectionMode_InvalidOperation));
    if (this._owner.SelectionMode != CalendarSelectionMode.SingleRange || this._isAddingRange || this.Count <= 0)
      return false;
    this.ClearInternal();
    return true;
  }

  private bool IsValidThread() => Thread.CurrentThread == this._dispatcherThread;

  private void UpdateMinMax(DateTime date)
  {
    if (!this._maximumDate.HasValue || date > this._maximumDate.Value)
      this._maximumDate = new DateTime?(date);
    if (this._minimumDate.HasValue && !(date < this._minimumDate.Value))
      return;
    this._minimumDate = new DateTime?(date);
  }

  private void ClearMinMax()
  {
    this._maximumDate = new DateTime?();
    this._minimumDate = new DateTime?();
  }

  private static IEnumerable<DateTime> GetDaysInRange(DateTime start, DateTime end)
  {
    int increment = SelectedDatesCollection.GetDirection(start, end);
    DateTime? rangeStart = new DateTime?(start);
    do
    {
      yield return rangeStart.Value;
      rangeStart = DateTimeHelper.AddDays(rangeStart.Value, increment);
    }
    while (rangeStart.HasValue && DateTime.Compare(end, rangeStart.Value) != -increment);
  }

  private static int GetDirection(DateTime start, DateTime end)
  {
    return DateTime.Compare(end, start) < 0 ? -1 : 1;
  }
}
