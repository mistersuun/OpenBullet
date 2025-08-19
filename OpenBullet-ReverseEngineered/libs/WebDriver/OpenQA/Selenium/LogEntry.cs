// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.LogEntry
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium;

public class LogEntry
{
  private LogLevel level;
  private DateTime timestamp = DateTime.MinValue;
  private string message = string.Empty;

  private LogEntry()
  {
  }

  public DateTime Timestamp => this.timestamp;

  public LogLevel Level => this.level;

  public string Message => this.message;

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0:yyyy-MM-ddTHH:mm:ssZ}] [{1}] {2}", (object) this.timestamp, (object) this.level, (object) this.message);
  }

  internal static LogEntry FromDictionary(Dictionary<string, object> entryDictionary)
  {
    LogEntry logEntry = new LogEntry();
    if (entryDictionary.ContainsKey("message"))
      logEntry.message = entryDictionary["message"].ToString();
    if (entryDictionary.ContainsKey("timestamp"))
    {
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      double num = Convert.ToDouble(entryDictionary["timestamp"], (IFormatProvider) CultureInfo.InvariantCulture);
      logEntry.timestamp = dateTime.AddMilliseconds(num);
    }
    if (entryDictionary.ContainsKey("level"))
    {
      string str = entryDictionary["level"].ToString();
      try
      {
        logEntry.level = (LogLevel) Enum.Parse(typeof (LogLevel), str, true);
      }
      catch (ArgumentException ex)
      {
      }
    }
    return logEntry;
  }
}
