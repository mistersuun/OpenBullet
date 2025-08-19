// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonDateTime
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public class PythonDateTime
{
  public static readonly int MAXYEAR = DateTime.MaxValue.Year;
  public static readonly int MINYEAR = DateTime.MinValue.Year;
  public const string __doc__ = "Provides functions and types for working with dates and times.";

  internal static void ThrowIfInvalid(PythonDateTime.timedelta delta, string funcname)
  {
    if (delta == null)
      return;
    if (delta._microseconds != 0 || delta._seconds % 60 != 0)
      throw PythonOps.ValueError("tzinfo.{0}() must return a whole number of minutes", (object) funcname);
    int num = (int) (delta.TimeSpanWithDaysAndSeconds.TotalSeconds / 60.0);
    if (Math.Abs(num) >= 1440)
      throw PythonOps.ValueError("tzinfo.{0}() returned {1}; must be in -1439 .. 1439", (object) funcname, (object) num);
  }

  internal static void ValidateInput(PythonDateTime.InputKind kind, int value)
  {
    switch (kind)
    {
      case PythonDateTime.InputKind.Year:
        int num1 = value;
        DateTime dateTime = DateTime.MaxValue;
        int year1 = dateTime.Year;
        if (num1 <= year1)
        {
          int num2 = value;
          dateTime = DateTime.MinValue;
          int year2 = dateTime.Year;
          if (num2 >= year2)
            break;
        }
        throw PythonOps.ValueError("year is out of range");
      case PythonDateTime.InputKind.Month:
        if (value <= 12 && value >= 1)
          break;
        throw PythonOps.ValueError("month must be in 1..12");
      case PythonDateTime.InputKind.Day:
        if (value <= 31 /*0x1F*/ && value >= 1)
          break;
        throw PythonOps.ValueError("day is out of range for month");
      case PythonDateTime.InputKind.Hour:
        if (value <= 23 && value >= 0)
          break;
        throw PythonOps.ValueError("hour must be in 0..23");
      case PythonDateTime.InputKind.Minute:
        if (value <= 59 && value >= 0)
          break;
        throw PythonOps.ValueError("minute must be in 0..59");
      case PythonDateTime.InputKind.Second:
        if (value <= 59 && value >= 0)
          break;
        throw PythonOps.ValueError("second must be in 0..59");
      case PythonDateTime.InputKind.Microsecond:
        if (value <= 999999 && value >= 0)
          break;
        throw PythonOps.ValueError("microsecond must be in 0..999999");
    }
  }

  internal static bool IsNaiveTimeZone(PythonDateTime.tzinfo tz)
  {
    return tz == null || tz.utcoffset((object) null) == null;
  }

  internal static int CastToInt(object o)
  {
    return !(o is BigInteger bigInteger) ? (int) o : (int) bigInteger;
  }

  [PythonType]
  public class timedelta : ICodeFormattable
  {
    internal int _days;
    internal int _seconds;
    internal int _microseconds;
    private TimeSpan _tsWithDaysAndSeconds;
    private TimeSpan _tsWithSeconds;
    private bool _fWithDaysAndSeconds;
    private bool _fWithSeconds;
    internal static readonly PythonDateTime.timedelta _DayResolution = new PythonDateTime.timedelta(1.0, 0.0, 0.0);
    public static readonly PythonDateTime.timedelta resolution = new PythonDateTime.timedelta(0.0, 0.0, 1.0);
    public static readonly PythonDateTime.timedelta min = new PythonDateTime.timedelta(-999999999.0, 0.0, 0.0);
    public static readonly PythonDateTime.timedelta max = new PythonDateTime.timedelta(999999999.0, 86399.0, 999999.0);
    private const int MAXDAYS = 999999999;
    private const double SECONDSPERDAY = 86400.0;

    internal timedelta(double days, double seconds, double microsecond)
      : this(days, seconds, microsecond, 0.0, 0.0, 0.0, 0.0)
    {
    }

    internal timedelta(TimeSpan ts, double microsecond)
      : this((double) ts.Days, (double) ts.Seconds, microsecond, (double) ts.Milliseconds, (double) ts.Minutes, (double) ts.Hours, 0.0)
    {
    }

    public timedelta(
      double days,
      double seconds,
      double microseconds,
      double milliseconds,
      double minutes,
      double hours,
      double weeks)
    {
      double d = (((weeks * 7.0 + days) * 24.0 + hours) * 60.0 + minutes) * 60.0 + seconds;
      double num1 = Math.Floor(d);
      double num2 = Math.Round((d - num1) * 1000000.0 + milliseconds * 1000.0 + microseconds);
      double num3 = Math.Floor(num2 / 1000000.0);
      double num4 = num1 + num3;
      double num5 = num2 - num3 * 1000000.0;
      if (num4 > 0.0 && num5 < 0.0)
      {
        --num4;
        num5 += 1000000.0;
      }
      this._days = (int) (num4 / 86400.0);
      this._seconds = (int) (num4 - (double) this._days * 86400.0);
      if (this._seconds < 0)
      {
        --this._days;
        this._seconds += 86400;
      }
      this._microseconds = (int) num5;
      if (Math.Abs(this._days) > 999999999)
        throw PythonOps.OverflowError("days={0}; must have magnitude <= 999999999", (object) this._days);
    }

    public static PythonDateTime.timedelta __new__(
      CodeContext context,
      PythonType cls,
      double days = 0.0,
      double seconds = 0.0,
      double microseconds = 0.0,
      double milliseconds = 0.0,
      double minutes = 0.0,
      double hours = 0.0,
      double weeks = 0.0)
    {
      if (cls == DynamicHelpers.GetPythonTypeFromType(typeof (PythonDateTime.timedelta)))
        return new PythonDateTime.timedelta(days, seconds, microseconds, milliseconds, minutes, hours, weeks);
      if (!(cls.CreateInstance(context, (object) days, (object) seconds, (object) microseconds, (object) milliseconds, (object) minutes, (object) hours, (object) weeks) is PythonDateTime.timedelta instance))
        throw PythonOps.TypeError("{0} is not a subclass of datetime.timedelta", (object) cls);
      return instance;
    }

    public int days => this._days;

    public int seconds => this._seconds;

    public int microseconds => this._microseconds;

    internal TimeSpan TimeSpanWithDaysAndSeconds
    {
      get
      {
        if (!this._fWithDaysAndSeconds)
        {
          this._tsWithDaysAndSeconds = new TimeSpan(this._days, 0, 0, this._seconds);
          this._fWithDaysAndSeconds = true;
        }
        return this._tsWithDaysAndSeconds;
      }
    }

    internal TimeSpan TimeSpanWithSeconds
    {
      get
      {
        if (!this._fWithSeconds)
        {
          this._tsWithSeconds = TimeSpan.FromSeconds((double) this._seconds);
          this._fWithSeconds = true;
        }
        return this._tsWithSeconds;
      }
    }

    public static PythonDateTime.timedelta operator +(
      PythonDateTime.timedelta self,
      PythonDateTime.timedelta other)
    {
      return new PythonDateTime.timedelta((double) (self._days + other._days), (double) (self._seconds + other._seconds), (double) (self._microseconds + other._microseconds));
    }

    public static PythonDateTime.timedelta operator -(
      PythonDateTime.timedelta self,
      PythonDateTime.timedelta other)
    {
      return new PythonDateTime.timedelta((double) (self._days - other._days), (double) (self._seconds - other._seconds), (double) (self._microseconds - other._microseconds));
    }

    public static PythonDateTime.timedelta operator -(PythonDateTime.timedelta self)
    {
      return new PythonDateTime.timedelta((double) -self._days, (double) -self._seconds, (double) -self._microseconds);
    }

    public static PythonDateTime.timedelta operator +(PythonDateTime.timedelta self)
    {
      return new PythonDateTime.timedelta((double) self._days, (double) self._seconds, (double) self._microseconds);
    }

    public static PythonDateTime.timedelta operator *(PythonDateTime.timedelta self, int other)
    {
      return new PythonDateTime.timedelta((double) (self._days * other), (double) (self._seconds * other), (double) (self._microseconds * other));
    }

    public static PythonDateTime.timedelta operator *(int other, PythonDateTime.timedelta self)
    {
      return new PythonDateTime.timedelta((double) (self._days * other), (double) (self._seconds * other), (double) (self._microseconds * other));
    }

    public static PythonDateTime.timedelta operator /(PythonDateTime.timedelta self, int other)
    {
      return new PythonDateTime.timedelta((double) self._days / (double) other, (double) self._seconds / (double) other, (double) self._microseconds / (double) other);
    }

    public static PythonDateTime.timedelta operator *(
      PythonDateTime.timedelta self,
      BigInteger other)
    {
      return self * (int) other;
    }

    public static PythonDateTime.timedelta operator *(
      BigInteger other,
      PythonDateTime.timedelta self)
    {
      return (int) other * self;
    }

    public static PythonDateTime.timedelta operator /(
      PythonDateTime.timedelta self,
      BigInteger other)
    {
      return self / (int) other;
    }

    public PythonDateTime.timedelta __pos__() => +this;

    public PythonDateTime.timedelta __neg__() => -this;

    public PythonDateTime.timedelta __abs__() => this._days <= 0 ? -this : this;

    [SpecialName]
    public PythonDateTime.timedelta FloorDivide(int y) => this / y;

    [SpecialName]
    public PythonDateTime.timedelta ReverseFloorDivide(int y) => this / y;

    public double total_seconds()
    {
      return ((double) this.microseconds + ((double) this.seconds + (double) this.days * 24.0 * 3600.0) * 1000000.0) / 1000000.0;
    }

    public bool __nonzero__() => this._days != 0 || this._seconds != 0 || this._microseconds != 0;

    public PythonTuple __reduce__()
    {
      return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple((object) this._days, (object) this._seconds, (object) this._microseconds));
    }

    public static object __getnewargs__(int days, int seconds, int microseconds)
    {
      return (object) PythonTuple.MakeTuple((object) new PythonDateTime.timedelta((double) days, (double) seconds, (double) microseconds, 0.0, 0.0, 0.0, 0.0));
    }

    public override bool Equals(object obj)
    {
      return obj is PythonDateTime.timedelta timedelta && this._days == timedelta._days && this._seconds == timedelta._seconds && this._microseconds == timedelta._microseconds;
    }

    public override int GetHashCode() => this._days ^ this._seconds ^ this._microseconds;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this._days != 0)
      {
        stringBuilder.Append(this._days);
        if (Math.Abs(this._days) == 1)
          stringBuilder.Append(" day, ");
        else
          stringBuilder.Append(" days, ");
      }
      stringBuilder.AppendFormat("{0}:{1:d2}:{2:d2}", (object) this.TimeSpanWithSeconds.Hours, (object) this.TimeSpanWithSeconds.Minutes, (object) this.TimeSpanWithSeconds.Seconds);
      if (this._microseconds != 0)
        stringBuilder.AppendFormat(".{0:d6}", (object) this._microseconds);
      return stringBuilder.ToString();
    }

    private int CompareTo(object other)
    {
      if (!(other is PythonDateTime.timedelta timedelta))
        throw PythonOps.TypeError("can't compare datetime.timedelta to {0}", (object) PythonTypeOps.GetName(other));
      int num1 = this._days - timedelta._days;
      if (num1 != 0)
        return num1;
      int num2 = this._seconds - timedelta._seconds;
      return num2 != 0 ? num2 : this._microseconds - timedelta._microseconds;
    }

    public static bool operator >(PythonDateTime.timedelta self, object other)
    {
      return self.CompareTo(other) > 0;
    }

    public static bool operator <(PythonDateTime.timedelta self, object other)
    {
      return self.CompareTo(other) < 0;
    }

    public static bool operator >=(PythonDateTime.timedelta self, object other)
    {
      return self.CompareTo(other) >= 0;
    }

    public static bool operator <=(PythonDateTime.timedelta self, object other)
    {
      return self.CompareTo(other) <= 0;
    }

    public virtual string __repr__(CodeContext context)
    {
      if (this._seconds == 0 && this._microseconds == 0)
        return $"datetime.timedelta({this._days})";
      return this._microseconds == 0 ? $"datetime.timedelta({this._days}, {this._seconds})" : $"datetime.timedelta({this._days}, {this._seconds}, {this._microseconds})";
    }
  }

  internal enum InputKind
  {
    Year,
    Month,
    Day,
    Hour,
    Minute,
    Second,
    Microsecond,
  }

  [PythonType]
  public class date : ICodeFormattable
  {
    internal DateTime _dateTime;
    public static readonly PythonDateTime.date min = new PythonDateTime.date(new DateTime(1, 1, 1));
    public static readonly PythonDateTime.date max = new PythonDateTime.date(new DateTime(9999, 12, 31 /*0x1F*/));
    public static readonly PythonDateTime.timedelta resolution = PythonDateTime.timedelta._DayResolution;

    internal date()
    {
    }

    public date(int year, int month, int day)
    {
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Year, year);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Month, month);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Day, day);
      this._dateTime = new DateTime(year, month, day);
    }

    internal date(DateTime value) => this._dateTime = value.Date;

    public static object today() => (object) new PythonDateTime.date(DateTime.Today);

    public static PythonDateTime.date fromordinal(int d)
    {
      if (d < 1)
        throw PythonOps.ValueError("ordinal must be >= 1");
      return new PythonDateTime.date(PythonDateTime.date.min._dateTime.AddDays((double) (d - 1)));
    }

    public static PythonDateTime.date fromtimestamp(double timestamp)
    {
      DateTime dateTime = PythonTime.TimestampToDateTime(timestamp);
      dateTime = dateTime.AddSeconds((double) -PythonTime.timezone);
      return new PythonDateTime.date(dateTime.Year, dateTime.Month, dateTime.Day);
    }

    public int year => this._dateTime.Year;

    public int month => this._dateTime.Month;

    public int day => this._dateTime.Day;

    internal DateTime InternalDateTime
    {
      get => this._dateTime;
      set => this._dateTime = value;
    }

    public static implicit operator DateTime(PythonDateTime.date self) => self._dateTime;

    public static PythonDateTime.date operator +(
      [NotNull] PythonDateTime.date self,
      [NotNull] PythonDateTime.timedelta other)
    {
      try
      {
        return new PythonDateTime.date(self._dateTime.AddDays((double) other.days));
      }
      catch
      {
        throw PythonOps.OverflowError("date value out of range");
      }
    }

    public static PythonDateTime.date operator +(
      [NotNull] PythonDateTime.timedelta other,
      [NotNull] PythonDateTime.date self)
    {
      try
      {
        return new PythonDateTime.date(self._dateTime.AddDays((double) other.days));
      }
      catch
      {
        throw PythonOps.OverflowError("date value out of range");
      }
    }

    public static PythonDateTime.date operator -(
      PythonDateTime.date self,
      PythonDateTime.timedelta delta)
    {
      try
      {
        return new PythonDateTime.date(self._dateTime.AddDays((double) (-1 * delta.days)));
      }
      catch
      {
        throw PythonOps.OverflowError("date value out of range");
      }
    }

    public static PythonDateTime.timedelta operator -(
      PythonDateTime.date self,
      PythonDateTime.date other)
    {
      TimeSpan timeSpan = self._dateTime - other._dateTime;
      return new PythonDateTime.timedelta(0.0, timeSpan.TotalSeconds, (double) (timeSpan.Milliseconds * 1000));
    }

    public virtual PythonTuple __reduce__()
    {
      return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple((object) this._dateTime.Year, (object) this._dateTime.Month, (object) this._dateTime.Day));
    }

    public static object __getnewargs__(CodeContext context, int year, int month, int day)
    {
      return (object) PythonTuple.MakeTuple((object) new PythonDateTime.date(year, month, day));
    }

    public object replace() => (object) this;

    public virtual PythonDateTime.date replace(
      CodeContext context,
      [ParamDictionary] IDictionary<object, object> dict)
    {
      int year = this._dateTime.Year;
      int month = this._dateTime.Month;
      int day = this._dateTime.Day;
      foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) dict)
      {
        if (keyValuePair.Key is string key)
        {
          switch (key)
          {
            case "year":
              year = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "month":
              month = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "day":
              day = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            default:
              throw PythonOps.TypeError("{0} is an invalid keyword argument for this function", keyValuePair.Key);
          }
        }
      }
      return new PythonDateTime.date(year, month, day);
    }

    public virtual object timetuple() => (object) PythonTime.GetDateTimeTuple(this._dateTime);

    public int toordinal() => (this._dateTime - PythonDateTime.date.min._dateTime).Days + 1;

    public int weekday() => PythonTime.Weekday(this._dateTime);

    public int isoweekday() => PythonTime.IsoWeekday(this._dateTime);

    private DateTime FirstDayOfIsoYear(int year)
    {
      DateTime dateTime1 = new DateTime(year, 1, 1);
      DateTime dateTime2 = dateTime1;
      switch (dateTime1.DayOfWeek)
      {
        case DayOfWeek.Sunday:
          dateTime2 = dateTime1.AddDays(1.0);
          break;
        case DayOfWeek.Monday:
        case DayOfWeek.Tuesday:
        case DayOfWeek.Wednesday:
        case DayOfWeek.Thursday:
          dateTime2 = dateTime1.AddDays((double) (-1 * (int) (dateTime1.DayOfWeek - 1)));
          break;
        case DayOfWeek.Friday:
          dateTime2 = dateTime1.AddDays(3.0);
          break;
        case DayOfWeek.Saturday:
          dateTime2 = dateTime1.AddDays(2.0);
          break;
      }
      return dateTime2;
    }

    public PythonTuple isocalendar()
    {
      DateTime dateTime1 = this.FirstDayOfIsoYear(this._dateTime.Year - 1);
      DateTime dateTime2 = this.FirstDayOfIsoYear(this._dateTime.Year);
      DateTime dateTime3 = this.FirstDayOfIsoYear(this._dateTime.Year + 1);
      int num;
      int days;
      if (dateTime2 <= this._dateTime && this._dateTime < dateTime3)
      {
        num = this._dateTime.Year;
        days = (this._dateTime - dateTime2).Days;
      }
      else if (this._dateTime < dateTime2)
      {
        num = this._dateTime.Year - 1;
        days = (this._dateTime - dateTime1).Days;
      }
      else
      {
        num = this._dateTime.Year + 1;
        days = (this._dateTime - dateTime3).Days;
      }
      return PythonTuple.MakeTuple((object) num, (object) (days / 7 + 1), (object) (days % 7 + 1));
    }

    public string isoformat() => this._dateTime.ToString("yyyy-MM-dd");

    public override string ToString() => this.isoformat();

    public string ctime()
    {
      return this._dateTime.ToString("ddd MMM ", (IFormatProvider) CultureInfo.InvariantCulture) + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0,2}", (object) this._dateTime.Day) + this._dateTime.ToString(" HH:mm:ss yyyy", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public virtual string strftime(CodeContext context, string dateFormat)
    {
      return PythonTime.strftime(context, dateFormat, this._dateTime, new int?());
    }

    public override bool Equals(object obj)
    {
      return obj != null && obj is PythonDateTime.date date && !(obj is PythonDateTime.datetime) && this._dateTime == date._dateTime;
    }

    public override int GetHashCode() => this._dateTime.GetHashCode();

    internal virtual int CompareTo(object other)
    {
      return this._dateTime.CompareTo((other as PythonDateTime.date)._dateTime);
    }

    internal bool CheckType(object other) => this.CheckType(other, true);

    internal bool CheckType(object other, bool shouldThrow)
    {
      if (other == null)
        return PythonDateTime.date.CheckTypeError(other, shouldThrow);
      if (!(other.GetType() != this.GetType()))
        return true;
      return (this.GetType() == typeof (PythonDateTime.date) && other.GetType() == typeof (PythonDateTime.datetime) || this.GetType() == typeof (PythonDateTime.datetime) & other.GetType() == typeof (PythonDateTime.date) || !PythonOps.HasAttr(DefaultContext.Default, other, "timetuple")) && PythonDateTime.date.CheckTypeError(other, shouldThrow);
    }

    private static bool CheckTypeError(object other, bool shouldThrow)
    {
      if (shouldThrow)
        throw PythonOps.TypeError("can't compare datetime.date to {0}", (object) PythonTypeOps.GetName(other));
      return true;
    }

    public static object operator >(PythonDateTime.date self, object other)
    {
      return !self.CheckType(other) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareTo(other) > 0);
    }

    public static object operator <(PythonDateTime.date self, object other)
    {
      return !self.CheckType(other) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareTo(other) < 0);
    }

    public static object operator >=(PythonDateTime.date self, object other)
    {
      return !self.CheckType(other) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareTo(other) >= 0);
    }

    public static object operator <=(PythonDateTime.date self, object other)
    {
      return !self.CheckType(other) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareTo(other) <= 0);
    }

    public object __eq__(object other)
    {
      return !this.CheckType(other, false) ? (object) NotImplementedType.Value : (object) this.Equals(other);
    }

    public object __ne__(object other)
    {
      return !this.CheckType(other, false) ? (object) NotImplementedType.Value : (object) !this.Equals(other);
    }

    public virtual string __repr__(CodeContext context)
    {
      return $"datetime.date({this._dateTime.Year}, {this._dateTime.Month}, {this._dateTime.Day})";
    }

    public virtual string __format__(CodeContext context, string dateFormat)
    {
      return string.IsNullOrEmpty(dateFormat) ? PythonOps.ToString(context, (object) this) : this.strftime(context, dateFormat);
    }
  }

  [PythonType]
  public class datetime : PythonDateTime.date, ICodeFormattable
  {
    internal int _lostMicroseconds;
    internal PythonDateTime.tzinfo _tz;
    public static readonly PythonDateTime.datetime max = new PythonDateTime.datetime(DateTime.MaxValue, 999, (PythonDateTime.tzinfo) null);
    public static readonly PythonDateTime.datetime min = new PythonDateTime.datetime(DateTime.MinValue, 0, (PythonDateTime.tzinfo) null);
    public new static readonly PythonDateTime.timedelta resolution = PythonDateTime.timedelta.resolution;
    private PythonDateTime.datetime.UnifiedDateTime _utcDateTime;
    private const long TicksPerMicrosecond = 10;

    public datetime(
      int year,
      int month,
      int day,
      int hour = 0,
      int minute = 0,
      int second = 0,
      int microsecond = 0,
      PythonDateTime.tzinfo tzinfo = null)
    {
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Year, year);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Month, month);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Day, day);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Hour, hour);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Minute, minute);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Second, second);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Microsecond, microsecond);
      this.InternalDateTime = new DateTime(year, month, day, hour, minute, second, microsecond / 1000);
      this._lostMicroseconds = microsecond % 1000;
      this._tz = tzinfo;
    }

    public datetime([NotNull] string str)
    {
      if (str.Length != 10)
        throw PythonOps.TypeError("an integer is required");
      int num = (int) str[7] << 16 /*0x10*/ | (int) str[8] << 8 | (int) str[9];
      int month = (int) str[2];
      if (month == 0 || month > 12)
        throw PythonOps.TypeError("invalid month");
      this.InternalDateTime = new DateTime((int) str[0] << 8 | (int) str[1], month, (int) str[3], (int) str[4], (int) str[5], (int) str[6], num / 1000);
      this._lostMicroseconds = this.microsecond % 1000;
    }

    public datetime([NotNull] string str, [NotNull] PythonDateTime.tzinfo tzinfo)
      : this(str)
    {
      this._tz = tzinfo;
    }

    private void Initialize(
      int year,
      int month,
      int day,
      int hour,
      int minute,
      int second,
      int microsecond,
      PythonDateTime.tzinfo tzinfo)
    {
    }

    public datetime(DateTime dt)
      : this(dt, (PythonDateTime.tzinfo) null)
    {
    }

    public datetime(DateTime dt, PythonDateTime.tzinfo tzinfo)
      : this(dt, (int) (dt.Ticks / 10L % 1000L), tzinfo)
    {
    }

    public datetime(params object[] args)
    {
      if (args.Length < 3)
        throw PythonOps.TypeError("function takes at least 3 arguments ({0} given)", (object) args.Length);
      if (args.Length > 8)
        throw PythonOps.TypeError("function takes at most 8 arguments ({0} given)", (object) args.Length);
      for (int index = 0; index < args.Length && index < 7; ++index)
      {
        if (!(args[index] is int))
          throw PythonOps.TypeError("an integer is required");
      }
      if (args.Length > 7 && !(args[7] is PythonDateTime.tzinfo) && args[7] != null)
        throw PythonOps.TypeError("tzinfo argument must be None or of a tzinfo subclass, not type '{0}'", (object) PythonTypeOps.GetName(args[7]));
      throw new InvalidOperationException();
    }

    internal datetime(DateTime dt, int lostMicroseconds, PythonDateTime.tzinfo tzinfo)
    {
      this.InternalDateTime = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
      this._lostMicroseconds = dt.Millisecond * 1000 + lostMicroseconds;
      this._tz = tzinfo;
      DateTime internalDateTime;
      if (this._lostMicroseconds < 0)
      {
        try
        {
          internalDateTime = this.InternalDateTime;
          this.InternalDateTime = internalDateTime.AddMilliseconds((double) (this._lostMicroseconds / 1000 - 1));
        }
        catch
        {
          throw PythonOps.OverflowError("date value out of range");
        }
        this._lostMicroseconds = this._lostMicroseconds % 1000 + 1000;
      }
      if (this._lostMicroseconds <= 999)
        return;
      try
      {
        internalDateTime = this.InternalDateTime;
        this.InternalDateTime = internalDateTime.AddMilliseconds((double) (this._lostMicroseconds / 1000));
      }
      catch
      {
        throw PythonOps.OverflowError("date value out of range");
      }
      this._lostMicroseconds %= 1000;
    }

    public static object now(PythonDateTime.tzinfo tz = null)
    {
      return tz != null ? tz.fromutc(new PythonDateTime.datetime(DateTime.UtcNow, 0, tz)) : (object) new PythonDateTime.datetime(DateTime.Now, 0, (PythonDateTime.tzinfo) null);
    }

    public static object utcnow()
    {
      return (object) new PythonDateTime.datetime(DateTime.UtcNow, 0, (PythonDateTime.tzinfo) null);
    }

    public new static object today()
    {
      return (object) new PythonDateTime.datetime(DateTime.Now, 0, (PythonDateTime.tzinfo) null);
    }

    public static object fromtimestamp(double timestamp, PythonDateTime.tzinfo tz = null)
    {
      DateTime dt1 = PythonTime.TimestampToDateTime(timestamp);
      dt1 = dt1.AddSeconds((double) -PythonTime.timezone);
      if (tz == null)
        return (object) new PythonDateTime.datetime(dt1);
      dt1 = dt1.ToUniversalTime();
      PythonDateTime.datetime dt2 = new PythonDateTime.datetime(dt1, tz);
      return tz.fromutc(dt2);
    }

    public static PythonDateTime.datetime utcfromtimestamp(double timestamp)
    {
      return new PythonDateTime.datetime(new DateTime(PythonTime.TimestampToTicks(timestamp), DateTimeKind.Utc), 0, (PythonDateTime.tzinfo) null);
    }

    public static PythonDateTime.datetime fromordinal(int d)
    {
      return d >= 1 ? new PythonDateTime.datetime(DateTime.MinValue + new TimeSpan(d - 1, 0, 0, 0), 0, (PythonDateTime.tzinfo) null) : throw PythonOps.ValueError("ordinal must be >= 1");
    }

    public static object combine(PythonDateTime.date date, PythonDateTime.time time)
    {
      return (object) new PythonDateTime.datetime(date.year, date.month, date.day, time.hour, time.minute, time.second, time.microsecond, time.tzinfo);
    }

    public int hour => this.InternalDateTime.Hour;

    public int minute => this.InternalDateTime.Minute;

    public int second => this.InternalDateTime.Second;

    public int microsecond => this.InternalDateTime.Millisecond * 1000 + this._lostMicroseconds;

    public object tzinfo => (object) this._tz;

    private PythonDateTime.datetime.UnifiedDateTime UtcDateTime
    {
      get
      {
        if (this._utcDateTime == null)
        {
          this._utcDateTime = new PythonDateTime.datetime.UnifiedDateTime();
          this._utcDateTime.DateTime = this.InternalDateTime;
          this._utcDateTime.LostMicroseconds = this._lostMicroseconds;
          PythonDateTime.timedelta timedelta = this.utcoffset();
          if (timedelta != null)
          {
            PythonDateTime.datetime datetime = this - timedelta;
            this._utcDateTime.DateTime = datetime.InternalDateTime;
            this._utcDateTime.LostMicroseconds = datetime._lostMicroseconds;
          }
        }
        return this._utcDateTime;
      }
    }

    public static PythonDateTime.datetime operator +(
      [NotNull] PythonDateTime.datetime date,
      [NotNull] PythonDateTime.timedelta delta)
    {
      try
      {
        return new PythonDateTime.datetime(date.InternalDateTime.Add(delta.TimeSpanWithDaysAndSeconds), delta._microseconds + date._lostMicroseconds, date._tz);
      }
      catch (ArgumentException ex)
      {
        throw new OverflowException("date value out of range");
      }
    }

    public static PythonDateTime.datetime operator +(
      [NotNull] PythonDateTime.timedelta delta,
      [NotNull] PythonDateTime.datetime date)
    {
      try
      {
        return new PythonDateTime.datetime(date.InternalDateTime.Add(delta.TimeSpanWithDaysAndSeconds), delta._microseconds + date._lostMicroseconds, date._tz);
      }
      catch (ArgumentException ex)
      {
        throw new OverflowException("date value out of range");
      }
    }

    public static PythonDateTime.datetime operator -(
      PythonDateTime.datetime date,
      PythonDateTime.timedelta delta)
    {
      return new PythonDateTime.datetime(date.InternalDateTime.Subtract(delta.TimeSpanWithDaysAndSeconds), date._lostMicroseconds - delta._microseconds, date._tz);
    }

    public static PythonDateTime.timedelta operator -(
      PythonDateTime.datetime date,
      PythonDateTime.datetime other)
    {
      return PythonDateTime.datetime.CheckTzInfoBeforeCompare(date, other) ? new PythonDateTime.timedelta(date.InternalDateTime - other.InternalDateTime, (double) (date._lostMicroseconds - other._lostMicroseconds)) : new PythonDateTime.timedelta(date.UtcDateTime.DateTime - other.UtcDateTime.DateTime, (double) (date.UtcDateTime.LostMicroseconds - other.UtcDateTime.LostMicroseconds));
    }

    public PythonDateTime.date date() => new PythonDateTime.date(this.year, this.month, this.day);

    [Documentation("gets the datetime w/o the time zone component")]
    public PythonDateTime.time time()
    {
      return new PythonDateTime.time(this.hour, this.minute, this.second, this.microsecond, (PythonDateTime.tzinfo) null);
    }

    public object timetz()
    {
      return (object) new PythonDateTime.time(this.hour, this.minute, this.second, this.microsecond, this._tz);
    }

    [Documentation("gets a new datetime object with the fields provided as keyword arguments replaced.")]
    public override PythonDateTime.date replace(
      CodeContext context,
      [ParamDictionary] IDictionary<object, object> dict)
    {
      int year = this.year;
      int month = this.month;
      int day = this.day;
      int hour = this.hour;
      int minute = this.minute;
      int second = this.second;
      int microsecond = this.microsecond;
      PythonDateTime.tzinfo tz = this._tz;
      foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) dict)
      {
        if (keyValuePair.Key is string key)
        {
          switch (key)
          {
            case "day":
              day = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "hour":
              hour = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "microsecond":
              microsecond = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "minute":
              minute = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "month":
              month = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "second":
              second = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "tzinfo":
              tz = keyValuePair.Value as PythonDateTime.tzinfo;
              continue;
            case "year":
              year = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            default:
              throw PythonOps.TypeError("{0} is an invalid keyword argument for this function", keyValuePair.Key);
          }
        }
      }
      return (PythonDateTime.date) new PythonDateTime.datetime(year, month, day, hour, minute, second, microsecond, tz);
    }

    public object astimezone(PythonDateTime.tzinfo tz)
    {
      if (tz == null)
        throw PythonOps.TypeError("astimezone() argument 1 must be datetime.tzinfo, not None");
      if (this._tz == null)
        throw PythonOps.ValueError("astimezone() cannot be applied to a naive datetime");
      if (tz == this._tz)
        return (object) this;
      PythonDateTime.datetime dt = this - this.utcoffset();
      dt._tz = tz;
      return tz.fromutc(dt);
    }

    public PythonDateTime.timedelta utcoffset()
    {
      if (this._tz == null)
        return (PythonDateTime.timedelta) null;
      PythonDateTime.timedelta delta = this._tz.utcoffset((object) this);
      PythonDateTime.ThrowIfInvalid(delta, nameof (utcoffset));
      return delta;
    }

    public PythonDateTime.timedelta dst()
    {
      if (this._tz == null)
        return (PythonDateTime.timedelta) null;
      PythonDateTime.timedelta delta = this._tz.dst((object) this);
      PythonDateTime.ThrowIfInvalid(delta, nameof (dst));
      return delta;
    }

    public object tzname()
    {
      return this._tz == null ? (object) null : (object) this._tz.tzname((object) this);
    }

    public override object timetuple()
    {
      return (object) PythonTime.GetDateTimeTuple(this.InternalDateTime, new DayOfWeek?(), this._tz);
    }

    public object utctimetuple()
    {
      return this._tz == null ? (object) PythonTime.GetDateTimeTuple(this.InternalDateTime, false) : (object) PythonTime.GetDateTimeTuple((this - this.utcoffset()).InternalDateTime, false);
    }

    public string isoformat(char sep = 'T')
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendFormat("{0:d4}-{1:d2}-{2:d2}{3}{4:d2}:{5:d2}:{6:d2}", (object) this.year, (object) this.month, (object) this.day, (object) sep, (object) this.hour, (object) this.minute, (object) this.second);
      if (this.microsecond != 0)
        stringBuilder1.AppendFormat(".{0:d6}", (object) this.microsecond);
      PythonDateTime.timedelta timedelta = this.utcoffset();
      if (timedelta != null)
      {
        if (timedelta.TimeSpanWithDaysAndSeconds >= TimeSpan.Zero)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          TimeSpan withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> hours = (ValueType) withDaysAndSeconds.Hours;
          withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> minutes = (ValueType) withDaysAndSeconds.Minutes;
          stringBuilder2.AppendFormat("+{0:d2}:{1:d2}", (object) hours, (object) minutes);
        }
        else
        {
          StringBuilder stringBuilder3 = stringBuilder1;
          TimeSpan withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> local1 = (ValueType) -withDaysAndSeconds.Hours;
          withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> local2 = (ValueType) -withDaysAndSeconds.Minutes;
          stringBuilder3.AppendFormat("-{0:d2}:{1:d2}", (object) local1, (object) local2);
        }
      }
      return stringBuilder1.ToString();
    }

    internal static bool CheckTzInfoBeforeCompare(
      PythonDateTime.datetime self,
      PythonDateTime.datetime other)
    {
      if (self._tz == other._tz)
        return true;
      PythonDateTime.timedelta timedelta1 = self.utcoffset();
      PythonDateTime.timedelta timedelta2 = other.utcoffset();
      if (timedelta1 == null && timedelta2 != null || timedelta1 != null && timedelta2 == null)
        throw PythonOps.TypeError("can't compare offset-naive and offset-aware times");
      return false;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is PythonDateTime.datetime other))
        return false;
      return PythonDateTime.datetime.CheckTzInfoBeforeCompare(this, other) ? this.InternalDateTime.Equals(other.InternalDateTime) && this._lostMicroseconds == other._lostMicroseconds : Math.Abs((this.InternalDateTime - other.InternalDateTime).TotalHours) <= 48.0 && this.UtcDateTime.Equals((object) other.UtcDateTime);
    }

    public override int GetHashCode()
    {
      return this.UtcDateTime.DateTime.GetHashCode() ^ this.UtcDateTime.LostMicroseconds;
    }

    public override string ToString() => this.isoformat(' ');

    public override PythonTuple __reduce__()
    {
      return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple((object) this.InternalDateTime.Year, (object) this.InternalDateTime.Month, (object) this.InternalDateTime.Day, (object) this.InternalDateTime.Hour, (object) this.InternalDateTime.Minute, (object) this.InternalDateTime.Second, (object) this.microsecond, this.tzinfo));
    }

    public override string strftime(CodeContext context, string dateFormat)
    {
      return PythonTime.strftime(context, dateFormat, this._dateTime, new int?(this.microsecond));
    }

    public static PythonDateTime.datetime strptime(
      CodeContext context,
      string date_string,
      string format)
    {
      return new PythonDateTime.datetime((DateTime) PythonTime._strptime(context, date_string, format)[0]);
    }

    internal override int CompareTo(object other)
    {
      if (other == null)
        throw PythonOps.TypeError("can't compare datetime.datetime to NoneType");
      if (!(other is PythonDateTime.datetime other1))
        throw PythonOps.TypeError("can't compare datetime.datetime to {0}", (object) PythonTypeOps.GetName(other));
      if (PythonDateTime.datetime.CheckTzInfoBeforeCompare(this, other1))
      {
        int num = this.InternalDateTime.CompareTo(other1.InternalDateTime);
        return num != 0 ? num : this._lostMicroseconds - other1._lostMicroseconds;
      }
      TimeSpan timeSpan = this.InternalDateTime - other1.InternalDateTime;
      if (Math.Abs(timeSpan.TotalHours) <= 48.0)
        return this.UtcDateTime.CompareTo(other1.UtcDateTime);
      return !(timeSpan > TimeSpan.Zero) ? -1 : 1;
    }

    public override string __repr__(CodeContext context)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("datetime.datetime({0}, {1}, {2}, {3}, {4}", (object) this.InternalDateTime.Year, (object) this.InternalDateTime.Month, (object) this.InternalDateTime.Day, (object) this.InternalDateTime.Hour, (object) this.InternalDateTime.Minute);
      if (this.microsecond != 0)
        stringBuilder.AppendFormat(", {0}, {1}", (object) this.second, (object) this.microsecond);
      else if (this.second != 0)
        stringBuilder.AppendFormat(", {0}", (object) this.second);
      if (this._tz != null)
        stringBuilder.AppendFormat(", tzinfo={0}", (object) PythonOps.Repr(context, (object) this._tz));
      stringBuilder.AppendFormat(")");
      return stringBuilder.ToString();
    }

    private class UnifiedDateTime
    {
      public DateTime DateTime;
      public int LostMicroseconds;

      public override bool Equals(object obj)
      {
        return obj is PythonDateTime.datetime.UnifiedDateTime unifiedDateTime && this.DateTime == unifiedDateTime.DateTime && this.LostMicroseconds == unifiedDateTime.LostMicroseconds;
      }

      public override int GetHashCode() => this.DateTime.GetHashCode() ^ this.LostMicroseconds;

      public int CompareTo(PythonDateTime.datetime.UnifiedDateTime other)
      {
        int num = this.DateTime.CompareTo(other.DateTime);
        return num != 0 ? num : this.LostMicroseconds - other.LostMicroseconds;
      }
    }
  }

  [PythonType]
  public class time : ICodeFormattable
  {
    internal TimeSpan _timeSpan;
    internal int _lostMicroseconds;
    internal PythonDateTime.tzinfo _tz;
    private PythonDateTime.time.UnifiedTime _utcTime;
    public static readonly PythonDateTime.time max = new PythonDateTime.time(23, 59, 59, 999999, (PythonDateTime.tzinfo) null);
    public static readonly PythonDateTime.time min = new PythonDateTime.time(0, 0, 0, 0, (PythonDateTime.tzinfo) null);
    public static readonly PythonDateTime.timedelta resolution = PythonDateTime.timedelta.resolution;

    public time(int hour = 0, int minute = 0, int second = 0, int microsecond = 0, PythonDateTime.tzinfo tzinfo = null)
    {
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Hour, hour);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Minute, minute);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Second, second);
      PythonDateTime.ValidateInput(PythonDateTime.InputKind.Microsecond, microsecond);
      this._timeSpan = new TimeSpan(0, hour, minute, second, microsecond / 1000);
      this._lostMicroseconds = microsecond % 1000;
      this._tz = tzinfo;
    }

    internal time(TimeSpan timeSpan, int lostMicroseconds, PythonDateTime.tzinfo tzinfo)
    {
      this._timeSpan = timeSpan;
      this._lostMicroseconds = lostMicroseconds;
      this._tz = tzinfo;
    }

    public int hour => this._timeSpan.Hours;

    public int minute => this._timeSpan.Minutes;

    public int second => this._timeSpan.Seconds;

    public int microsecond => this._timeSpan.Milliseconds * 1000 + this._lostMicroseconds;

    public PythonDateTime.tzinfo tzinfo => this._tz;

    private PythonDateTime.time.UnifiedTime UtcTime
    {
      get
      {
        if (this._utcTime == null)
        {
          this._utcTime = new PythonDateTime.time.UnifiedTime();
          this._utcTime.TimeSpan = this._timeSpan;
          this._utcTime.LostMicroseconds = this._lostMicroseconds;
          PythonDateTime.timedelta timedelta = this.utcoffset();
          if (timedelta != null)
          {
            PythonDateTime.time time = PythonDateTime.time.Add(this, -timedelta);
            this._utcTime.TimeSpan = time._timeSpan;
            this._utcTime.LostMicroseconds = time._lostMicroseconds;
          }
        }
        return this._utcTime;
      }
    }

    private static PythonDateTime.time Add(PythonDateTime.time date, PythonDateTime.timedelta delta)
    {
      return new PythonDateTime.time(date._timeSpan.Add(delta.TimeSpanWithDaysAndSeconds), delta._microseconds + date._lostMicroseconds, date._tz);
    }

    public PythonTuple __reduce__()
    {
      return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple((object) this.hour, (object) this.minute, (object) this.second, (object) this.microsecond, (object) this.tzinfo));
    }

    public bool __nonzero__()
    {
      return this.UtcTime.TimeSpan.Ticks != 0L || this.UtcTime.LostMicroseconds != 0;
    }

    public static explicit operator bool(PythonDateTime.time time) => time.__nonzero__();

    public object replace() => (object) this;

    public object replace([ParamDictionary] IDictionary<object, object> dict)
    {
      int hour = this.hour;
      int minute = this.minute;
      int second = this.second;
      int microsecond = this.microsecond;
      PythonDateTime.tzinfo tzinfo = this.tzinfo;
      foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) dict)
      {
        if (keyValuePair.Key is string key)
        {
          switch (key)
          {
            case "hour":
              hour = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "minute":
              minute = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "second":
              second = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "microsecond":
              microsecond = PythonDateTime.CastToInt(keyValuePair.Value);
              continue;
            case "tzinfo":
              tzinfo = keyValuePair.Value as PythonDateTime.tzinfo;
              continue;
            default:
              continue;
          }
        }
      }
      return (object) new PythonDateTime.time(hour, minute, second, microsecond, tzinfo);
    }

    public object isoformat() => (object) this.ToString();

    public override string ToString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendFormat("{0:d2}:{1:d2}:{2:d2}", (object) this.hour, (object) this.minute, (object) this.second);
      if (this.microsecond != 0)
        stringBuilder1.AppendFormat(".{0:d6}", (object) this.microsecond);
      PythonDateTime.timedelta timedelta = this.utcoffset();
      if (timedelta != null)
      {
        if (timedelta.TimeSpanWithDaysAndSeconds >= TimeSpan.Zero)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          TimeSpan withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> hours = (ValueType) withDaysAndSeconds.Hours;
          withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> minutes = (ValueType) withDaysAndSeconds.Minutes;
          stringBuilder2.AppendFormat("+{0:d2}:{1:d2}", (object) hours, (object) minutes);
        }
        else
        {
          StringBuilder stringBuilder3 = stringBuilder1;
          TimeSpan withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> local1 = (ValueType) -withDaysAndSeconds.Hours;
          withDaysAndSeconds = timedelta.TimeSpanWithDaysAndSeconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> local2 = (ValueType) -withDaysAndSeconds.Minutes;
          stringBuilder3.AppendFormat("-{0:d2}:{1:d2}", (object) local1, (object) local2);
        }
      }
      return stringBuilder1.ToString();
    }

    public string strftime(CodeContext context, string format)
    {
      return PythonTime.strftime(context, format, new DateTime(1900, 1, 1, this._timeSpan.Hours, this._timeSpan.Minutes, this._timeSpan.Seconds, this._timeSpan.Milliseconds), new int?(this._lostMicroseconds));
    }

    public PythonDateTime.timedelta utcoffset()
    {
      if (this._tz == null)
        return (PythonDateTime.timedelta) null;
      PythonDateTime.timedelta delta = this._tz.utcoffset((object) null);
      PythonDateTime.ThrowIfInvalid(delta, nameof (utcoffset));
      return delta;
    }

    public object dst()
    {
      if (this._tz == null)
        return (object) null;
      PythonDateTime.timedelta delta = this._tz.dst((object) null);
      PythonDateTime.ThrowIfInvalid(delta, nameof (dst));
      return (object) delta;
    }

    public object tzname()
    {
      return this._tz == null ? (object) null : (object) this._tz.tzname((object) null);
    }

    public override int GetHashCode() => this.UtcTime.GetHashCode();

    internal static bool CheckTzInfoBeforeCompare(
      PythonDateTime.time self,
      PythonDateTime.time other)
    {
      if (self._tz == other._tz)
        return true;
      PythonDateTime.timedelta timedelta1 = self.utcoffset();
      PythonDateTime.timedelta timedelta2 = other.utcoffset();
      if (timedelta1 == null && timedelta2 != null || timedelta1 != null && timedelta2 == null)
        throw PythonOps.TypeError("can't compare offset-naive and offset-aware times");
      return false;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is PythonDateTime.time other))
        return false;
      if (!PythonDateTime.time.CheckTzInfoBeforeCompare(this, other))
        return this.UtcTime.Equals((object) other.UtcTime);
      return this._timeSpan == other._timeSpan && this._lostMicroseconds == other._lostMicroseconds;
    }

    private int CompareTo(object other)
    {
      if (!(other is PythonDateTime.time other1))
        throw PythonOps.TypeError("can't compare datetime.time to {0}", (object) PythonTypeOps.GetName(other));
      if (!PythonDateTime.time.CheckTzInfoBeforeCompare(this, other1))
        return this.UtcTime.CompareTo(other1.UtcTime);
      int num = this._timeSpan.CompareTo(other1._timeSpan);
      return num != 0 ? num : this._lostMicroseconds - other1._lostMicroseconds;
    }

    public static bool operator >(PythonDateTime.time self, object other)
    {
      return self.CompareTo(other) > 0;
    }

    public static bool operator <(PythonDateTime.time self, object other)
    {
      return self.CompareTo(other) < 0;
    }

    public static bool operator >=(PythonDateTime.time self, object other)
    {
      return self.CompareTo(other) >= 0;
    }

    public static bool operator <=(PythonDateTime.time self, object other)
    {
      return self.CompareTo(other) <= 0;
    }

    public virtual string __repr__(CodeContext context)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.microsecond != 0)
        stringBuilder.AppendFormat("datetime.time({0}, {1}, {2}, {3}", (object) this.hour, (object) this.minute, (object) this.second, (object) this.microsecond);
      else if (this.second != 0)
        stringBuilder.AppendFormat("datetime.time({0}, {1}, {2}", (object) this.hour, (object) this.minute, (object) this.second);
      else
        stringBuilder.AppendFormat("datetime.time({0}, {1}", (object) this.hour, (object) this.minute);
      if (this.tzname() is string str)
        stringBuilder.AppendFormat(", tzinfo={0}", (object) str.ToLower());
      stringBuilder.AppendFormat(")");
      return stringBuilder.ToString();
    }

    public object __format__(CodeContext context, string dateFormat)
    {
      if (string.IsNullOrEmpty(dateFormat))
        return (object) PythonOps.ToString(context, (object) this);
      if (this.GetType() == typeof (PythonDateTime.time))
        return (object) this.strftime(context, dateFormat);
      return PythonOps.Invoke(context, (object) this, "strftime", (object) dateFormat);
    }

    private class UnifiedTime
    {
      public TimeSpan TimeSpan;
      public int LostMicroseconds;

      public override bool Equals(object obj)
      {
        return obj is PythonDateTime.time.UnifiedTime unifiedTime && this.TimeSpan == unifiedTime.TimeSpan && this.LostMicroseconds == unifiedTime.LostMicroseconds;
      }

      public override int GetHashCode() => this.TimeSpan.GetHashCode() ^ this.LostMicroseconds;

      public int CompareTo(PythonDateTime.time.UnifiedTime other)
      {
        int num = this.TimeSpan.CompareTo(other.TimeSpan);
        return num != 0 ? num : this.LostMicroseconds - other.LostMicroseconds;
      }
    }
  }

  [PythonType]
  public class tzinfo
  {
    public tzinfo()
    {
    }

    public tzinfo(params object[] args)
    {
    }

    public tzinfo([ParamDictionary] PythonDictionary dict, params object[] args)
    {
    }

    public virtual object fromutc(PythonDateTime.datetime dt)
    {
      PythonDateTime.timedelta timedelta1 = this.utcoffset((object) dt);
      if (timedelta1 == null)
        throw PythonOps.ValueError("fromutc: non-None utcoffset() result required");
      PythonDateTime.timedelta timedelta2 = this.dst((object) dt);
      if (timedelta2 == null)
        throw PythonOps.ValueError("fromutc: non-None dst() result required");
      PythonDateTime.timedelta timedelta3 = timedelta1 - timedelta2;
      dt += timedelta3;
      PythonDateTime.timedelta timedelta4 = dt.dst();
      return (object) (dt + timedelta4);
    }

    public virtual PythonDateTime.timedelta dst(object dt) => throw new NotImplementedException();

    public virtual string tzname(object dt)
    {
      throw new NotImplementedException("a tzinfo subclass must implement tzname()");
    }

    public virtual PythonDateTime.timedelta utcoffset(object dt)
    {
      throw new NotImplementedException();
    }

    public PythonTuple __reduce__(CodeContext context)
    {
      object ret;
      return this.GetType() == typeof (PythonDateTime.tzinfo) || !PythonOps.TryGetBoundAttr(context, (object) this, "__dict__", out ret) ? PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.EMPTY) : PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.EMPTY, ret);
    }
  }
}
