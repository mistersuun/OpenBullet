// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.CalendarBlackoutDatesCollection
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;

#nullable disable
namespace Microsoft.Windows.Controls;

public sealed class CalendarBlackoutDatesCollection : ObservableCollection<CalendarDateRange>
{
  private Thread _dispatcherThread;
  private Calendar _owner;

  public CalendarBlackoutDatesCollection(Calendar owner)
  {
    this._owner = owner;
    this._dispatcherThread = Thread.CurrentThread;
  }

  public void AddDatesInPast()
  {
    this.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1.0)));
  }

  public bool Contains(DateTime date) => null != this.GetContainingDateRange(date);

  public bool Contains(DateTime start, DateTime end)
  {
    int count = this.Count;
    DateTime t2_1;
    DateTime t2_2;
    if (DateTime.Compare(end, start) > -1)
    {
      t2_1 = DateTimeHelper.DiscardTime(new DateTime?(start)).Value;
      t2_2 = DateTimeHelper.DiscardTime(new DateTime?(end)).Value;
    }
    else
    {
      t2_1 = DateTimeHelper.DiscardTime(new DateTime?(end)).Value;
      t2_2 = DateTimeHelper.DiscardTime(new DateTime?(start)).Value;
    }
    for (int index = 0; index < count; ++index)
    {
      if (DateTime.Compare(this[index].Start, t2_1) == 0 && DateTime.Compare(this[index].End, t2_2) == 0)
        return true;
    }
    return false;
  }

  public bool ContainsAny(CalendarDateRange range)
  {
    foreach (CalendarDateRange calendarDateRange in (Collection<CalendarDateRange>) this)
    {
      if (calendarDateRange.ContainsAny(range))
        return true;
    }
    return false;
  }

  internal DateTime? GetNonBlackoutDate(DateTime? requestedDate, int dayInterval)
  {
    DateTime? nonBlackoutDate = requestedDate;
    if (!requestedDate.HasValue)
      return new DateTime?();
    CalendarDateRange containingDateRange;
    if ((containingDateRange = this.GetContainingDateRange(nonBlackoutDate.Value)) == null)
      return requestedDate;
    do
    {
      nonBlackoutDate = dayInterval <= 0 ? DateTimeHelper.AddDays(containingDateRange.Start, dayInterval) : DateTimeHelper.AddDays(containingDateRange.End, dayInterval);
    }
    while (nonBlackoutDate.HasValue && (containingDateRange = this.GetContainingDateRange(nonBlackoutDate.Value)) != null);
    return nonBlackoutDate;
  }

  protected override void ClearItems()
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    foreach (CalendarDateRange calendarDateRange in (IEnumerable<CalendarDateRange>) this.Items)
      this.UnRegisterItem(calendarDateRange);
    base.ClearItems();
    this._owner.UpdateCellItems();
  }

  protected override void InsertItem(int index, CalendarDateRange item)
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    if (!this.IsValid(item))
      throw new ArgumentOutOfRangeException(SR.Get(SRID.Calendar_UnSelectableDates));
    this.RegisterItem(item);
    base.InsertItem(index, item);
    this._owner.UpdateCellItems();
  }

  protected override void RemoveItem(int index)
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    if (index >= 0 && index < this.Count)
      this.UnRegisterItem(this.Items[index]);
    base.RemoveItem(index);
    this._owner.UpdateCellItems();
  }

  protected override void SetItem(int index, CalendarDateRange item)
  {
    if (!this.IsValidThread())
      throw new NotSupportedException(SR.Get(SRID.CalendarCollection_MultiThreadedCollectionChangeNotSupported));
    if (!this.IsValid(item))
      throw new ArgumentOutOfRangeException(SR.Get(SRID.Calendar_UnSelectableDates));
    CalendarDateRange calendarDateRange = (CalendarDateRange) null;
    if (index >= 0 && index < this.Count)
      calendarDateRange = this.Items[index];
    base.SetItem(index, item);
    this.UnRegisterItem(calendarDateRange);
    this.RegisterItem(this.Items[index]);
    this._owner.UpdateCellItems();
  }

  private void RegisterItem(CalendarDateRange item)
  {
    if (item == null)
      return;
    item.Changing += new EventHandler<CalendarDateRangeChangingEventArgs>(this.Item_Changing);
    item.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
  }

  private void UnRegisterItem(CalendarDateRange item)
  {
    if (item == null)
      return;
    item.Changing -= new EventHandler<CalendarDateRangeChangingEventArgs>(this.Item_Changing);
    item.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
  }

  private void Item_Changing(object sender, CalendarDateRangeChangingEventArgs e)
  {
    if (sender is CalendarDateRange && !this.IsValid(e.Start, e.End))
      throw new ArgumentOutOfRangeException(SR.Get(SRID.Calendar_UnSelectableDates));
  }

  private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (!(sender is CalendarDateRange))
      return;
    this._owner.UpdateCellItems();
  }

  private bool IsValid(CalendarDateRange item) => this.IsValid(item.Start, item.End);

  private bool IsValid(DateTime start, DateTime end)
  {
    foreach (DateTime selectedDate in (Collection<DateTime>) this._owner.SelectedDates)
    {
      if (DateTimeHelper.InRange(((object) selectedDate as DateTime?).Value, start, end))
        return false;
    }
    return true;
  }

  private bool IsValidThread() => Thread.CurrentThread == this._dispatcherThread;

  private CalendarDateRange GetContainingDateRange(DateTime date)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      if (DateTimeHelper.InRange(date, this[index]))
        return this[index];
    }
    return (CalendarDateRange) null;
  }
}
