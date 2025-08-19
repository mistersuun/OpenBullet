// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DateTimeHelper
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

#nullable disable
namespace Microsoft.Windows.Controls;

internal static class DateTimeHelper
{
  private static System.Globalization.Calendar cal = (System.Globalization.Calendar) new GregorianCalendar();

  public static DateTime? AddDays(DateTime time, int days)
  {
    try
    {
      return new DateTime?(DateTimeHelper.cal.AddDays(time, days));
    }
    catch (ArgumentException ex)
    {
      return new DateTime?();
    }
  }

  public static DateTime? AddMonths(DateTime time, int months)
  {
    try
    {
      return new DateTime?(DateTimeHelper.cal.AddMonths(time, months));
    }
    catch (ArgumentException ex)
    {
      return new DateTime?();
    }
  }

  public static DateTime? AddYears(DateTime time, int years)
  {
    try
    {
      return new DateTime?(DateTimeHelper.cal.AddYears(time, years));
    }
    catch (ArgumentException ex)
    {
      return new DateTime?();
    }
  }

  public static DateTime? SetYear(DateTime date, int year)
  {
    return DateTimeHelper.AddYears(date, year - date.Year);
  }

  public static DateTime? SetYearMonth(DateTime date, DateTime yearMonth)
  {
    DateTime? nullable = DateTimeHelper.SetYear(date, yearMonth.Year);
    if (nullable.HasValue)
      nullable = DateTimeHelper.AddMonths(nullable.Value, yearMonth.Month - date.Month);
    return nullable;
  }

  public static int CompareDays(DateTime dt1, DateTime dt2)
  {
    return DateTime.Compare(DateTimeHelper.DiscardTime(new DateTime?(dt1)).Value, DateTimeHelper.DiscardTime(new DateTime?(dt2)).Value);
  }

  public static int CompareYearMonth(DateTime dt1, DateTime dt2)
  {
    return (dt1.Year - dt2.Year) * 12 + (dt1.Month - dt2.Month);
  }

  public static int DecadeOfDate(DateTime date) => date.Year - date.Year % 10;

  public static DateTime DiscardDayTime(DateTime d) => new DateTime(d.Year, d.Month, 1, 0, 0, 0);

  public static DateTime? DiscardTime(DateTime? d)
  {
    return !d.HasValue ? new DateTime?() : new DateTime?(d.Value.Date);
  }

  public static int EndOfDecade(DateTime date) => DateTimeHelper.DecadeOfDate(date) + 9;

  public static DateTimeFormatInfo GetCurrentDateFormat()
  {
    return DateTimeHelper.GetDateFormat(CultureInfo.CurrentCulture);
  }

  internal static CultureInfo GetCulture(FrameworkElement element)
  {
    return DependencyPropertyHelper.GetValueSource((DependencyObject) element, FrameworkElement.LanguageProperty).BaseValueSource == BaseValueSource.Default ? CultureInfo.CurrentCulture : DateTimeHelper.GetCultureInfo((DependencyObject) element);
  }

  internal static CultureInfo GetCultureInfo(DependencyObject element)
  {
    XmlLanguage xmlLanguage = (XmlLanguage) element.GetValue(FrameworkElement.LanguageProperty);
    try
    {
      return xmlLanguage.GetSpecificCulture();
    }
    catch (InvalidOperationException ex)
    {
      return CultureInfo.ReadOnly(new CultureInfo("en-us", false));
    }
  }

  internal static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
  {
    if (culture.Calendar is GregorianCalendar)
      return culture.DateTimeFormat;
    GregorianCalendar gregorianCalendar = (GregorianCalendar) null;
    foreach (System.Globalization.Calendar optionalCalendar in culture.OptionalCalendars)
    {
      if (optionalCalendar is GregorianCalendar)
      {
        if (gregorianCalendar == null)
          gregorianCalendar = optionalCalendar as GregorianCalendar;
        if (((GregorianCalendar) optionalCalendar).CalendarType == GregorianCalendarTypes.Localized)
        {
          gregorianCalendar = optionalCalendar as GregorianCalendar;
          break;
        }
      }
    }
    DateTimeFormatInfo dateTimeFormat;
    if (gregorianCalendar == null)
    {
      dateTimeFormat = ((CultureInfo) CultureInfo.InvariantCulture.Clone()).DateTimeFormat;
      dateTimeFormat.Calendar = (System.Globalization.Calendar) new GregorianCalendar();
    }
    else
    {
      dateTimeFormat = ((CultureInfo) culture.Clone()).DateTimeFormat;
      dateTimeFormat.Calendar = (System.Globalization.Calendar) gregorianCalendar;
    }
    return dateTimeFormat;
  }

  public static bool InRange(DateTime date, CalendarDateRange range)
  {
    return DateTimeHelper.InRange(date, range.Start, range.End);
  }

  public static bool InRange(DateTime date, DateTime start, DateTime end)
  {
    return DateTimeHelper.CompareDays(date, start) > -1 && DateTimeHelper.CompareDays(date, end) < 1;
  }

  public static string ToDayString(DateTime? date, CultureInfo culture)
  {
    string empty = string.Empty;
    DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
    if (date.HasValue && dateFormat != null)
      empty = date.Value.Day.ToString((IFormatProvider) dateFormat);
    return empty;
  }

  public static string ToDecadeRangeString(int decade, CultureInfo culture)
  {
    string decadeRangeString = string.Empty;
    DateTimeFormatInfo dateTimeFormat = culture.DateTimeFormat;
    if (dateTimeFormat != null)
    {
      int num = decade + 9;
      decadeRangeString = $"{decade.ToString((IFormatProvider) dateTimeFormat)}-{num.ToString((IFormatProvider) dateTimeFormat)}";
    }
    return decadeRangeString;
  }

  public static string ToYearMonthPatternString(DateTime? date, CultureInfo culture)
  {
    string empty = string.Empty;
    DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
    if (date.HasValue && dateFormat != null)
      empty = date.Value.ToString(dateFormat.YearMonthPattern, (IFormatProvider) dateFormat);
    return empty;
  }

  public static string ToYearString(DateTime? date, CultureInfo culture)
  {
    string empty = string.Empty;
    DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
    if (date.HasValue && dateFormat != null)
      empty = date.Value.Year.ToString((IFormatProvider) dateFormat);
    return empty;
  }

  public static string ToAbbreviatedMonthString(DateTime? date, CultureInfo culture)
  {
    string empty = string.Empty;
    DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
    if (date.HasValue && dateFormat != null)
    {
      string[] abbreviatedMonthNames = dateFormat.AbbreviatedMonthNames;
      if (abbreviatedMonthNames != null && abbreviatedMonthNames.Length > 0)
        empty = abbreviatedMonthNames[(date.Value.Month - 1) % abbreviatedMonthNames.Length];
    }
    return empty;
  }

  public static string ToLongDateString(DateTime? date, CultureInfo culture)
  {
    string empty = string.Empty;
    DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
    if (date.HasValue && dateFormat != null)
      empty = date.Value.Date.ToString(dateFormat.LongDatePattern, (IFormatProvider) dateFormat);
    return empty;
  }
}
