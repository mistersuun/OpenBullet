// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.DateTimeParser
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace Xceed.Wpf.Toolkit;

internal class DateTimeParser
{
  public static bool TryParse(
    string value,
    string format,
    DateTime currentDate,
    CultureInfo cultureInfo,
    out DateTime result)
  {
    bool flag = false;
    result = currentDate;
    if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(format))
      return false;
    string s = DateTimeParser.ComputeDateTimeString(value, format, currentDate, cultureInfo).Trim();
    if (!string.IsNullOrEmpty(s))
      flag = DateTime.TryParse(s, (IFormatProvider) cultureInfo.DateTimeFormat, DateTimeStyles.None, out result);
    if (!flag)
      result = currentDate;
    return flag;
  }

  private static string ComputeDateTimeString(
    string dateTime,
    string format,
    DateTime currentDate,
    CultureInfo cultureInfo)
  {
    Dictionary<string, string> dateParts = DateTimeParser.GetDateParts(currentDate, cultureInfo);
    string[] strArray1 = new string[3]
    {
      currentDate.Hour.ToString(),
      currentDate.Minute.ToString(),
      currentDate.Second.ToString()
    };
    string str1 = currentDate.Millisecond.ToString();
    string str2 = "";
    string[] strArray2 = new string[7]
    {
      ",",
      " ",
      "-",
      ".",
      "/",
      cultureInfo.DateTimeFormat.DateSeparator,
      cultureInfo.DateTimeFormat.TimeSeparator
    };
    DateTimeParser.UpdateSortableDateTimeString(ref dateTime, ref format, cultureInfo);
    List<string> stringList1 = new List<string>();
    List<string> stringList2 = new List<string>();
    if (((IEnumerable<string>) strArray2).Any<string>((Func<string, bool>) (s => dateTime.Contains(s))))
    {
      stringList1 = ((IEnumerable<string>) dateTime.Split(strArray2, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      stringList2 = ((IEnumerable<string>) format.Split(strArray2, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
    }
    else
    {
      string source = "";
      string str3 = "";
      char[] charArray = format.ToCharArray();
      for (int index = 0; index < ((IEnumerable<char>) charArray).Count<char>(); ++index)
      {
        char ch = charArray[index];
        if (!source.Contains<char>(ch))
        {
          if (!string.IsNullOrEmpty(source))
          {
            stringList2.Add(source);
            stringList1.Add(str3);
          }
          source = ch.ToString();
          str3 = index < dateTime.Length ? dateTime[index].ToString() : "";
        }
        else
        {
          source += (string) (object) ch;
          str3 += (string) (object) (char) (index < dateTime.Length ? (int) dateTime[index] : 0);
        }
      }
      if (!string.IsNullOrEmpty(source))
      {
        stringList2.Add(source);
        stringList1.Add(str3);
      }
    }
    if (stringList1.Count < stringList2.Count)
    {
      while (stringList1.Count != stringList2.Count)
        stringList1.Add("0");
    }
    if (stringList1.Count != stringList2.Count)
      return string.Empty;
    for (int index = 0; index < stringList2.Count; ++index)
    {
      string str4 = stringList2[index];
      if (!str4.Contains("ddd") && !str4.Contains("GMT"))
      {
        if (str4.Contains("M"))
          dateParts["Month"] = stringList1[index];
        else if (str4.Contains("d"))
          dateParts["Day"] = stringList1[index];
        else if (str4.Contains("y"))
        {
          dateParts["Year"] = stringList1[index] != "0" ? stringList1[index] : "0000";
          if (dateParts["Year"].Length == 2)
            dateParts["Year"] = $"{currentDate.Year / 100}{dateParts["Year"]}";
        }
        else if (str4.Contains("h") || str4.Contains("H"))
          strArray1[0] = stringList1[index];
        else if (str4.Contains("m"))
          strArray1[1] = stringList1[index];
        else if (str4.Contains("s"))
          strArray1[2] = stringList1[index];
        else if (str4.Contains("f"))
          str1 = stringList1[index];
        else if (str4.Contains("t"))
          str2 = stringList1[index];
      }
    }
    return $"{string.Join(cultureInfo.DateTimeFormat.DateSeparator, dateParts.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value)).ToArray<string>())} {$"{string.Join(cultureInfo.DateTimeFormat.TimeSeparator, strArray1)}.{str1}"} {str2}";
  }

  private static void UpdateSortableDateTimeString(
    ref string dateTime,
    ref string format,
    CultureInfo cultureInfo)
  {
    if (format == cultureInfo.DateTimeFormat.SortableDateTimePattern)
    {
      format = format.Replace("'", "").Replace("T", " ");
      dateTime = dateTime.Replace("'", "").Replace("T", " ");
    }
    else
    {
      if (!(format == cultureInfo.DateTimeFormat.UniversalSortableDateTimePattern))
        return;
      format = format.Replace("'", "").Replace("Z", "");
      dateTime = dateTime.Replace("'", "").Replace("Z", "");
    }
  }

  private static Dictionary<string, string> GetDateParts(
    DateTime currentDate,
    CultureInfo cultureInfo)
  {
    Dictionary<string, string> dateParts = new Dictionary<string, string>();
    string[] separator = new string[7]
    {
      ",",
      " ",
      "-",
      ".",
      "/",
      cultureInfo.DateTimeFormat.DateSeparator,
      cultureInfo.DateTimeFormat.TimeSeparator
    };
    ((IEnumerable<string>) cultureInfo.DateTimeFormat.ShortDatePattern.Split(separator, StringSplitOptions.RemoveEmptyEntries)).ToList<string>().ForEach((Action<string>) (item =>
    {
      string key = string.Empty;
      string empty = string.Empty;
      if (item.Contains("M"))
      {
        key = "Month";
        empty = currentDate.Month.ToString();
      }
      else if (item.Contains("d"))
      {
        key = "Day";
        empty = currentDate.Day.ToString();
      }
      else if (item.Contains("y"))
      {
        key = "Year";
        empty = currentDate.Year.ToString("D4");
      }
      if (dateParts.ContainsKey(key))
        return;
      dateParts.Add(key, empty);
    }));
    return dateParts;
  }
}
