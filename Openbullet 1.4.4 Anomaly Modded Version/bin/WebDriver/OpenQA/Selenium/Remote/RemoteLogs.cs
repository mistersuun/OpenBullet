// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteLogs
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteLogs : ILogs
{
  private RemoteWebDriver driver;

  public RemoteLogs(RemoteWebDriver driver) => this.driver = driver;

  public ReadOnlyCollection<string> AvailableLogTypes
  {
    get
    {
      List<string> stringList = new List<string>();
      if (this.driver.InternalExecute(DriverCommand.GetAvailableLogTypes, (Dictionary<string, object>) null).Value is object[] objArray)
      {
        for (int index = 0; index < objArray.Length; ++index)
        {
          object obj = objArray[index];
          stringList.Add(obj.ToString());
        }
      }
      return stringList.AsReadOnly();
    }
  }

  public ReadOnlyCollection<LogEntry> GetLog(string logKind)
  {
    List<LogEntry> logEntryList = new List<LogEntry>();
    if (this.driver.InternalExecute(DriverCommand.GetLog, new Dictionary<string, object>()
    {
      {
        "type",
        (object) logKind
      }
    }).Value is object[] objArray)
    {
      for (int index = 0; index < objArray.Length; ++index)
      {
        if (objArray[index] is Dictionary<string, object> entryDictionary)
          logEntryList.Add(LogEntry.FromDictionary(entryDictionary));
      }
    }
    return logEntryList.AsReadOnly();
  }
}
