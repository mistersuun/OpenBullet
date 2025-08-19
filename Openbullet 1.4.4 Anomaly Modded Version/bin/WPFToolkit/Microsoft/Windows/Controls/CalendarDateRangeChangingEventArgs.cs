// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.CalendarDateRangeChangingEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class CalendarDateRangeChangingEventArgs : EventArgs
{
  private DateTime _start;
  private DateTime _end;

  public CalendarDateRangeChangingEventArgs(DateTime start, DateTime end)
  {
    this._start = start;
    this._end = end;
  }

  public DateTime Start => this._start;

  public DateTime End => this._end;
}
