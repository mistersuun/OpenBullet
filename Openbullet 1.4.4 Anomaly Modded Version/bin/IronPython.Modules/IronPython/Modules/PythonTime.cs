// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonTime
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class PythonTime
{
  private const int YearIndex = 0;
  private const int MonthIndex = 1;
  private const int DayIndex = 2;
  private const int HourIndex = 3;
  private const int MinuteIndex = 4;
  private const int SecondIndex = 5;
  private const int WeekdayIndex = 6;
  private const int DayOfYearIndex = 7;
  private const int IsDaylightSavingsIndex = 8;
  private const int MaxIndex = 9;
  private const int minYear = 1900;
  private const double epochDifferenceDouble = 62135596800.0;
  private const long epochDifferenceLong = 62135596800;
  private const double ticksPerSecond = 10000000.0;
  public static readonly int altzone;
  public static readonly int daylight = TimeZoneInfo.Local.SupportsDaylightSavingTime ? 1 : 0;
  public static readonly int timezone;
  public static readonly PythonTuple tzname = PythonTuple.MakeTuple((object) TimeZoneInfo.Local.StandardName, (object) TimeZoneInfo.Local.DaylightName);
  public const bool accept2dyear = true;
  private static Stopwatch sw;
  public const string __doc__ = "This module provides various functions to manipulate time values.";

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    PythonLocale.EnsureLocaleInitialized(context);
  }

  static PythonTime()
  {
    PythonTime.timezone = (int) -TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds;
    PythonTime.altzone = (int) -TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalSeconds;
  }

  internal static long TimestampToTicks(double seconds)
  {
    return ((long) seconds + 62135596800L) * 10000000L + (long) (Math.Round(seconds % 1.0, 6) * 10000000.0);
  }

  internal static double TicksToTimestamp(long ticks)
  {
    return (double) ticks / 10000000.0 - 62135596800.0;
  }

  public static string asctime(CodeContext context) => PythonTime.asctime(context, (object) null);

  public static string asctime(CodeContext context, object time)
  {
    DateTime dateTime;
    if (time is PythonTuple)
    {
      dateTime = PythonTime.GetDateTimeFromTupleNoDst(context, (PythonTuple) time);
    }
    else
    {
      if (time != null)
        throw PythonOps.TypeError("expected struct_time or None");
      dateTime = DateTime.Now;
    }
    return dateTime.ToString("ddd MMM dd HH:mm:ss yyyy", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static double clock()
  {
    PythonTime.InitStopWatch();
    return (double) PythonTime.sw.ElapsedTicks / (double) Stopwatch.Frequency;
  }

  public static string ctime(CodeContext context)
  {
    return PythonTime.asctime(context, (object) PythonTime.localtime());
  }

  public static string ctime(CodeContext context, object seconds)
  {
    return seconds == null ? PythonTime.ctime(context) : PythonTime.asctime(context, (object) PythonTime.localtime(seconds));
  }

  public static void sleep(double tm) => Thread.Sleep((int) (tm * 1000.0));

  public static double time()
  {
    DateTime dateTime = DateTime.Now;
    dateTime = dateTime.ToUniversalTime();
    return PythonTime.TicksToTimestamp(dateTime.Ticks);
  }

  public static PythonTuple localtime()
  {
    return (PythonTuple) PythonTime.GetDateTimeTuple(DateTime.Now, DateTime.Now.IsDaylightSavingTime());
  }

  public static PythonTuple localtime(object seconds)
  {
    if (seconds == null)
      return PythonTime.localtime();
    DateTime dt = PythonTime.TimestampToDateTime(PythonTime.GetTimestampFromObject(seconds)).AddSeconds((double) -PythonTime.timezone);
    return (PythonTuple) PythonTime.GetDateTimeTuple(dt, dt.IsDaylightSavingTime());
  }

  public static PythonTuple gmtime()
  {
    return (PythonTuple) PythonTime.GetDateTimeTuple(DateTime.Now.ToUniversalTime(), false);
  }

  public static PythonTuple gmtime(object seconds)
  {
    return seconds == null ? PythonTime.gmtime() : (PythonTuple) PythonTime.GetDateTimeTuple(new DateTime(PythonTime.TimestampToTicks(PythonTime.GetTimestampFromObject(seconds)), DateTimeKind.Unspecified), false);
  }

  public static double mktime(CodeContext context, PythonTuple localTime)
  {
    DateTime dateTime = PythonTime.GetDateTimeFromTuple(context, localTime);
    dateTime = dateTime.AddSeconds((double) PythonTime.timezone);
    return PythonTime.TicksToTimestamp(dateTime.Ticks);
  }

  public static string strftime(CodeContext context, string format)
  {
    return PythonTime.strftime(context, format, DateTime.Now, new int?());
  }

  public static string strftime(CodeContext context, string format, PythonTuple dateTime)
  {
    return PythonTime.strftime(context, format, PythonTime.GetDateTimeFromTupleNoDst(context, dateTime), new int?());
  }

  public static object strptime(CodeContext context, string @string)
  {
    return (object) DateTime.Parse(@string, (IFormatProvider) PythonLocale.GetLocaleInfo(context).Time.DateTimeFormat);
  }

  public static object strptime(CodeContext context, string @string, string format)
  {
    object[] objArray = PythonTime._strptime(context, @string, format);
    return (object) PythonTime.GetDateTimeTuple((DateTime) objArray[0], (DayOfWeek?) objArray[1]);
  }

  internal static object[] _strptime(CodeContext context, string @string, string format)
  {
    bool postProcess;
    PythonTime.FoundDateComponents found;
    List<PythonTime.FormatInfo> cliFormat = PythonTime.PythonFormatToCLIFormat(format, true, out postProcess, out found);
    DateTime result;
    if (postProcess)
    {
      int format1 = PythonTime.FindFormat(cliFormat, "\\%j");
      int format2 = PythonTime.FindFormat(cliFormat, "\\%W");
      int format3 = PythonTime.FindFormat(cliFormat, "\\%U");
      if (format1 != -1 && format2 == -1 && format3 == -1)
      {
        result = new DateTime(1900, 1, 1);
        result = result.AddDays((double) int.Parse(@string));
      }
      else if (format2 != -1 && format1 == -1 && format3 == -1)
      {
        result = new DateTime(1900, 1, 1);
        result = result.AddDays((double) (int.Parse(@string) * 7));
      }
      else
      {
        if (format3 == -1 || format1 != -1 || format2 != -1)
          throw PythonOps.ValueError("cannot parse %j, %W, or %U w/ other values");
        result = new DateTime(1900, 1, 1);
        result = result.AddDays((double) (int.Parse(@string) * 7));
      }
    }
    else
    {
      int fIdx = -1;
      string[] formatParts = new string[cliFormat.Count];
      for (int index = 0; index < cliFormat.Count; ++index)
      {
        switch (cliFormat[index].Type)
        {
          case PythonTime.FormatInfoType.UserText:
            formatParts[index] = $"'{cliFormat[index].Text}'";
            break;
          case PythonTime.FormatInfoType.SimpleFormat:
            formatParts[index] = cliFormat[index].Text;
            break;
          case PythonTime.FormatInfoType.CustomFormat:
            if (cliFormat[index].Text == "f")
              fIdx = index;
            formatParts[index] = cliFormat.Count != 1 || cliFormat[index].Text.Length != 1 ? cliFormat[index].Text : "%" + cliFormat[index].Text;
            break;
        }
      }
      string[] strArray;
      if (fIdx != -1)
        strArray = PythonTime.ExpandMicrosecondFormat(fIdx, formatParts);
      else
        strArray = new string[1]
        {
          string.Join("", formatParts)
        };
      string[] formats = strArray;
      try
      {
        if (!StringUtils.TryParseDateTimeExact(@string, formats, (IFormatProvider) PythonLocale.GetLocaleInfo(context).Time.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out result))
          throw PythonOps.ValueError($"time data does not match format{Environment.NewLine}data={@string}, fmt={format}, to: {formats[0]}");
      }
      catch (FormatException ex)
      {
        throw PythonOps.ValueError($"{ex.Message}{Environment.NewLine}data={@string}, fmt={format}, to: {formats[0]}");
      }
    }
    DayOfWeek? nullable = new DayOfWeek?();
    if ((found & PythonTime.FoundDateComponents.DayOfWeek) != PythonTime.FoundDateComponents.None)
      nullable = new DayOfWeek?(result.DayOfWeek);
    if ((found & PythonTime.FoundDateComponents.Year) == PythonTime.FoundDateComponents.None)
      result = new DateTime(1900, result.Month, result.Day, result.Hour, result.Minute, result.Second, result.Millisecond, result.Kind);
    return new object[2]
    {
      (object) result,
      (object) nullable
    };
  }

  private static string[] ExpandMicrosecondFormat(int fIdx, string[] formatParts)
  {
    string[] strArray = new string[6];
    strArray[0] = string.Join("", formatParts);
    for (int index = 1; index < 6; ++index)
    {
      formatParts[fIdx] = new string('f', index + 1);
      strArray[index] = string.Join("", formatParts);
    }
    return strArray;
  }

  internal static string strftime(
    CodeContext context,
    string format,
    DateTime dt,
    int? microseconds)
  {
    bool postProcess;
    List<PythonTime.FormatInfo> cliFormat = PythonTime.PythonFormatToCLIFormat(format, false, out postProcess, out PythonTime.FoundDateComponents _);
    StringBuilder stringBuilder1 = new StringBuilder();
    for (int index = 0; index < cliFormat.Count; ++index)
    {
      switch (cliFormat[index].Type)
      {
        case PythonTime.FormatInfoType.UserText:
          stringBuilder1.Append(cliFormat[index].Text);
          break;
        case PythonTime.FormatInfoType.SimpleFormat:
          stringBuilder1.Append(dt.ToString(cliFormat[index].Text, (IFormatProvider) PythonLocale.GetLocaleInfo(context).Time.DateTimeFormat));
          break;
        case PythonTime.FormatInfoType.CustomFormat:
          stringBuilder1.Append(dt.ToString("%" + cliFormat[index].Text, (IFormatProvider) PythonLocale.GetLocaleInfo(context).Time.DateTimeFormat));
          break;
      }
    }
    if (postProcess)
    {
      StringBuilder stringBuilder2 = stringBuilder1.Replace("%f", microseconds.HasValue ? $"{microseconds:D6}" : "");
      int num1 = dt.DayOfYear;
      string newValue1 = num1.ToString("D03");
      StringBuilder stringBuilder3 = stringBuilder2.Replace("%j", newValue1);
      DateTime dateTime = new DateTime(dt.Year, 1, 1);
      int num2 = (int) (7 - dateTime.DayOfWeek) % 7;
      int num3 = (int) (8 - dateTime.DayOfWeek) % 7;
      StringBuilder stringBuilder4 = stringBuilder3;
      num1 = (dt.DayOfYear + 6 - num2) / 7;
      string newValue2 = num1.ToString();
      StringBuilder stringBuilder5 = stringBuilder4.Replace("%U", newValue2);
      num1 = (dt.DayOfYear + 6 - num3) / 7;
      string newValue3 = num1.ToString();
      StringBuilder stringBuilder6 = stringBuilder5.Replace("%W", newValue3);
      num1 = (int) dt.DayOfWeek;
      string newValue4 = num1.ToString();
      stringBuilder1 = stringBuilder6.Replace("%w", newValue4);
    }
    return stringBuilder1.ToString();
  }

  internal static double DateTimeToTimestamp(DateTime dateTime)
  {
    return PythonTime.TicksToTimestamp(PythonTime.RemoveDst(dateTime).Ticks);
  }

  internal static DateTime TimestampToDateTime(double timeStamp)
  {
    return PythonTime.AddDst(new DateTime(PythonTime.TimestampToTicks(timeStamp)));
  }

  private static DateTime RemoveDst(DateTime dt) => PythonTime.RemoveDst(dt, false);

  private static DateTime RemoveDst(DateTime dt, bool always)
  {
    if (always || TimeZoneInfo.Local.IsDaylightSavingTime(dt))
      dt -= TimeZoneInfo.Local.GetUtcOffset(dt) - TimeZoneInfo.Local.BaseUtcOffset;
    return dt;
  }

  private static DateTime AddDst(DateTime dt)
  {
    if (TimeZoneInfo.Local.IsDaylightSavingTime(dt))
      dt += TimeZoneInfo.Local.GetUtcOffset(dt) - TimeZoneInfo.Local.BaseUtcOffset;
    return dt;
  }

  private static double GetTimestampFromObject(object seconds)
  {
    int result1;
    if (Converter.TryConvertToInt32(seconds, out result1))
      return (double) result1;
    double result2;
    if (Converter.TryConvertToDouble(seconds, out result2))
      return result2 <= (double) long.MaxValue && result2 >= (double) long.MinValue ? result2 : throw PythonOps.ValueError("unreasonable date/time");
    throw PythonOps.TypeError("expected int, got {0}", (object) DynamicHelpers.GetPythonType(seconds));
  }

  private static void AddTime(List<PythonTime.FormatInfo> newFormat)
  {
    newFormat.Add(new PythonTime.FormatInfo("HH"));
    newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, ":"));
    newFormat.Add(new PythonTime.FormatInfo("mm"));
    newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, ":"));
    newFormat.Add(new PythonTime.FormatInfo("ss"));
  }

  private static void AddDate(List<PythonTime.FormatInfo> newFormat)
  {
    newFormat.Add(new PythonTime.FormatInfo("MM"));
    newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, "/"));
    newFormat.Add(new PythonTime.FormatInfo("dd"));
    newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, "/"));
    newFormat.Add(new PythonTime.FormatInfo("yy"));
  }

  private static List<PythonTime.FormatInfo> PythonFormatToCLIFormat(
    string format,
    bool forParse,
    out bool postProcess,
    out PythonTime.FoundDateComponents found)
  {
    postProcess = false;
    found = PythonTime.FoundDateComponents.None;
    List<PythonTime.FormatInfo> newFormat = new List<PythonTime.FormatInfo>();
    for (int index = 0; index < format.Length; ++index)
    {
      if (format[index] == '%')
      {
        if (index + 1 == format.Length)
          throw PythonOps.ValueError("badly formatted string");
        switch (format[++index])
        {
          case '%':
            newFormat.Add(new PythonTime.FormatInfo("\\%"));
            continue;
          case 'A':
            found |= PythonTime.FoundDateComponents.DayOfWeek;
            newFormat.Add(new PythonTime.FormatInfo("dddd"));
            continue;
          case 'B':
            newFormat.Add(new PythonTime.FormatInfo("MMMM"));
            continue;
          case 'H':
            newFormat.Add(new PythonTime.FormatInfo(forParse ? "H" : "HH"));
            continue;
          case 'I':
            newFormat.Add(new PythonTime.FormatInfo(forParse ? "h" : "hh"));
            continue;
          case 'M':
            newFormat.Add(new PythonTime.FormatInfo(forParse ? "m" : "mm"));
            continue;
          case 'S':
            newFormat.Add(new PythonTime.FormatInfo("ss"));
            continue;
          case 'U':
            newFormat.Add(new PythonTime.FormatInfo("\\%U"));
            postProcess = true;
            continue;
          case 'W':
            newFormat.Add(new PythonTime.FormatInfo("\\%W"));
            postProcess = true;
            continue;
          case 'X':
            PythonTime.AddTime(newFormat);
            continue;
          case 'Y':
            found |= PythonTime.FoundDateComponents.Year;
            newFormat.Add(new PythonTime.FormatInfo("yyyy"));
            continue;
          case 'Z':
          case 'z':
            newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, ""));
            continue;
          case 'a':
            found |= PythonTime.FoundDateComponents.DayOfWeek;
            newFormat.Add(new PythonTime.FormatInfo("ddd"));
            continue;
          case 'b':
            newFormat.Add(new PythonTime.FormatInfo("MMM"));
            continue;
          case 'c':
            found |= PythonTime.FoundDateComponents.Year;
            PythonTime.AddDate(newFormat);
            newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, " "));
            PythonTime.AddTime(newFormat);
            continue;
          case 'd':
            if (forParse)
            {
              newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.CustomFormat, "d"));
              continue;
            }
            newFormat.Add(new PythonTime.FormatInfo("dd"));
            continue;
          case 'f':
            if (forParse)
            {
              newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.CustomFormat, "f"));
              continue;
            }
            postProcess = true;
            newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, "%f"));
            continue;
          case 'j':
            newFormat.Add(new PythonTime.FormatInfo("\\%j"));
            postProcess = true;
            continue;
          case 'm':
            newFormat.Add(new PythonTime.FormatInfo(forParse ? "M" : "MM"));
            continue;
          case 'p':
            newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.CustomFormat, "t"));
            newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, "M"));
            continue;
          case 'w':
            newFormat.Add(new PythonTime.FormatInfo("\\%w"));
            postProcess = true;
            continue;
          case 'x':
            found |= PythonTime.FoundDateComponents.Year;
            PythonTime.AddDate(newFormat);
            continue;
          case 'y':
            found |= PythonTime.FoundDateComponents.Year;
            newFormat.Add(new PythonTime.FormatInfo("yy"));
            continue;
          default:
            newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, ""));
            continue;
        }
      }
      else if (newFormat.Count == 0 || newFormat[newFormat.Count - 1].Type != PythonTime.FormatInfoType.UserText)
        newFormat.Add(new PythonTime.FormatInfo(PythonTime.FormatInfoType.UserText, format[index].ToString()));
      else
        newFormat[newFormat.Count - 1].Text += format[index].ToString();
    }
    return newFormat;
  }

  internal static int Weekday(DateTime dt) => PythonTime.Weekday(dt.DayOfWeek);

  internal static int Weekday(DayOfWeek dayOfWeek)
  {
    return dayOfWeek == DayOfWeek.Sunday ? 6 : (int) (dayOfWeek - 1);
  }

  internal static int IsoWeekday(DateTime dt)
  {
    return dt.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) dt.DayOfWeek;
  }

  internal static PythonTuple GetDateTimeTuple(DateTime dt)
  {
    return PythonTime.GetDateTimeTuple(dt, new DayOfWeek?());
  }

  internal static PythonTuple GetDateTimeTuple(DateTime dt, DayOfWeek? dayOfWeek)
  {
    return PythonTime.GetDateTimeTuple(dt, dayOfWeek, (PythonDateTime.tzinfo) null);
  }

  internal static PythonTuple GetDateTimeTuple(
    DateTime dt,
    DayOfWeek? dayOfWeek,
    PythonDateTime.tzinfo tz)
  {
    int isDst = -1;
    if (tz != null)
    {
      PythonDateTime.timedelta delta = tz.dst((object) dt);
      PythonDateTime.ThrowIfInvalid(delta, "dst");
      isDst = delta != null ? (delta.__nonzero__() ? 1 : 0) : -1;
    }
    return (PythonTuple) new PythonTime.struct_time(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, PythonTime.Weekday((DayOfWeek) ((int) dayOfWeek ?? (int) dt.DayOfWeek)), dt.DayOfYear, isDst);
  }

  internal static PythonTime.struct_time GetDateTimeTuple(DateTime dt, bool dstMode)
  {
    return new PythonTime.struct_time(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, PythonTime.Weekday(dt), dt.DayOfYear, dstMode ? 1 : 0);
  }

  private static DateTime GetDateTimeFromTuple(CodeContext context, PythonTuple t)
  {
    int[] ints;
    DateTime dt = PythonTime.GetDateTimeFromTupleNoDst(context, t, out ints);
    if (ints != null)
    {
      switch (ints[8])
      {
        case -1:
          dt = PythonTime.RemoveDst(dt);
          break;
        case 1:
          dt = PythonTime.RemoveDst(dt, true);
          break;
      }
    }
    return dt;
  }

  private static DateTime GetDateTimeFromTupleNoDst(CodeContext context, PythonTuple t)
  {
    return PythonTime.GetDateTimeFromTupleNoDst(context, t, out int[] _);
  }

  private static DateTime GetDateTimeFromTupleNoDst(
    CodeContext context,
    PythonTuple t,
    out int[] ints)
  {
    if (t == null)
    {
      ints = (int[]) null;
      return DateTime.Now;
    }
    ints = PythonTime.ValidateDateTimeTuple(context, t);
    return new DateTime(ints[0], ints[1], ints[2], ints[3], ints[4], ints[5]);
  }

  private static int[] ValidateDateTimeTuple(CodeContext context, PythonTuple t)
  {
    if (t.__len__() != 9)
      throw PythonOps.TypeError("expected tuple of length {0}", (object) 9);
    int[] numArray = new int[9];
    for (int index = 0; index < 9; ++index)
      numArray[index] = context.LanguageContext.ConvertToInt32(t[index]);
    int num1 = numArray[0];
    if (num1 >= 0 && num1 <= 99)
    {
      if (num1 > 68)
        num1 += 1900;
      else
        num1 += 2000;
    }
    int num2 = num1;
    DateTime dateTime = DateTime.MinValue;
    int year1 = dateTime.Year;
    if (num2 < year1 || num1 <= 1900)
      throw PythonOps.ValueError("year is too low");
    int num3 = num1;
    dateTime = DateTime.MaxValue;
    int year2 = dateTime.Year;
    if (num3 > year2)
      throw PythonOps.ValueError("year is too high");
    return numArray[6] >= 0 && numArray[6] < 7 ? numArray : throw PythonOps.ValueError("day of week is outside of 0-6 range");
  }

  private static int FindFormat(List<PythonTime.FormatInfo> formatInfo, string format)
  {
    for (int index = 0; index < formatInfo.Count; ++index)
    {
      if (formatInfo[index].Text == format)
        return index;
    }
    return -1;
  }

  private static void InitStopWatch()
  {
    if (PythonTime.sw != null)
      return;
    PythonTime.sw = new Stopwatch();
    PythonTime.sw.Start();
  }

  private enum FormatInfoType
  {
    UserText,
    SimpleFormat,
    CustomFormat,
  }

  private class FormatInfo
  {
    public PythonTime.FormatInfoType Type;
    public string Text;

    public FormatInfo(string text)
    {
      this.Type = PythonTime.FormatInfoType.SimpleFormat;
      this.Text = text;
    }

    public FormatInfo(PythonTime.FormatInfoType type, string text)
    {
      this.Type = type;
      this.Text = text;
    }

    public override string ToString() => $"{this.Type}:{this.Text}";
  }

  [Flags]
  private enum FoundDateComponents
  {
    None = 0,
    Year = 1,
    Date = Year, // 0x00000001
    DayOfWeek = 2,
  }

  [PythonType]
  public class struct_time : PythonTuple
  {
    private static PythonType _StructTimeType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonTime.struct_time));

    public object tm_year => this._data[0];

    public object tm_mon => this._data[1];

    public object tm_mday => this._data[2];

    public object tm_hour => this._data[3];

    public object tm_min => this._data[4];

    public object tm_sec => this._data[5];

    public object tm_wday => this._data[6];

    public object tm_yday => this._data[7];

    public object tm_isdst => this._data[8];

    public int n_fields => this._data.Length;

    public int n_sequence_fields => this._data.Length;

    public int n_unnamed_fields => 0;

    internal struct_time(
      int year,
      int month,
      int day,
      int hour,
      int minute,
      int second,
      int dayOfWeek,
      int dayOfYear,
      int isDst)
      : base(new object[9]
      {
        (object) year,
        (object) month,
        (object) day,
        (object) hour,
        (object) minute,
        (object) second,
        (object) dayOfWeek,
        (object) dayOfYear,
        (object) isDst
      })
    {
    }

    internal struct_time(PythonTuple sequence)
      : base((object) sequence)
    {
    }

    public static PythonTime.struct_time __new__(
      CodeContext context,
      PythonType cls,
      int year,
      int month,
      int day,
      int hour,
      int minute,
      int second,
      int dayOfWeek,
      int dayOfYear,
      int isDst)
    {
      if (cls == PythonTime.struct_time._StructTimeType)
        return new PythonTime.struct_time(year, month, day, hour, minute, second, dayOfWeek, dayOfYear, isDst);
      if (!(cls.CreateInstance(context, (object) year, (object) month, (object) day, (object) hour, (object) minute, (object) second, (object) dayOfWeek, (object) dayOfYear, (object) isDst) is PythonTime.struct_time instance))
        throw PythonOps.TypeError("{0} is not a subclass of time.struct_time", (object) cls);
      return instance;
    }

    public static PythonTime.struct_time __new__(
      CodeContext context,
      PythonType cls,
      [NotNull] PythonTuple sequence)
    {
      if (sequence.__len__() != 9)
        throw PythonOps.TypeError("time.struct_time() takes a 9-sequence ({0}-sequence given)", (object) sequence.__len__());
      if (cls == PythonTime.struct_time._StructTimeType)
        return new PythonTime.struct_time(sequence);
      if (!(cls.CreateInstance(context, (object) sequence) is PythonTime.struct_time instance))
        throw PythonOps.TypeError("{0} is not a subclass of time.struct_time", (object) cls);
      return instance;
    }

    public static PythonTime.struct_time __new__(
      CodeContext context,
      PythonType cls,
      [NotNull] IEnumerable sequence)
    {
      return PythonTime.struct_time.__new__(context, cls, PythonTuple.Make((object) sequence));
    }

    public PythonTuple __reduce__()
    {
      return PythonTuple.MakeTuple((object) PythonTime.struct_time._StructTimeType, (object) PythonTuple.MakeTuple(this.tm_year, this.tm_mon, this.tm_mday, this.tm_hour, this.tm_min, this.tm_sec, this.tm_wday, this.tm_yday, this.tm_isdst));
    }

    public static object __getnewargs__(
      CodeContext context,
      int year,
      int month,
      int day,
      int hour,
      int minute,
      int second,
      int dayOfWeek,
      int dayOfYear,
      int isDst)
    {
      return (object) PythonTuple.MakeTuple((object) PythonTime.struct_time.__new__(context, PythonTime.struct_time._StructTimeType, year, month, day, hour, minute, second, dayOfWeek, dayOfYear, isDst));
    }

    public override string ToString()
    {
      return string.Format("time.struct_time(tm_year={0}, tm_mon={1}, tm_mday={2}, tm_hour={3}, tm_min={4}, tm_sec={5}, tm_wday={6}, tm_yday={7}, tm_isdst={8})", this._data);
    }

    public override string __repr__(CodeContext context) => this.ToString();
  }
}
