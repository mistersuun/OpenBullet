// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteSessionStorage
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Html5;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteSessionStorage : ISessionStorage
{
  private RemoteWebDriver driver;

  public RemoteSessionStorage(RemoteWebDriver driver) => this.driver = driver;

  public int Count
  {
    get
    {
      return Convert.ToInt32(this.driver.InternalExecute(DriverCommand.GetSessionStorageSize, (Dictionary<string, object>) null).Value, (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }

  public string GetItem(string key)
  {
    Response response = this.driver.InternalExecute(DriverCommand.GetSessionStorageItem, new Dictionary<string, object>()
    {
      {
        nameof (key),
        (object) key
      }
    });
    return response.Value == null ? (string) null : response.Value.ToString();
  }

  public ReadOnlyCollection<string> KeySet()
  {
    List<string> stringList = new List<string>();
    foreach (string str in this.driver.InternalExecute(DriverCommand.GetSessionStorageKeys, (Dictionary<string, object>) null).Value as object[])
      stringList.Add(str);
    return stringList.AsReadOnly();
  }

  public void SetItem(string key, string value)
  {
    this.driver.InternalExecute(DriverCommand.SetSessionStorageItem, new Dictionary<string, object>()
    {
      {
        nameof (key),
        (object) key
      },
      {
        nameof (value),
        (object) value
      }
    });
  }

  public string RemoveItem(string key)
  {
    Response response = this.driver.InternalExecute(DriverCommand.RemoveSessionStorageItem, new Dictionary<string, object>()
    {
      {
        nameof (key),
        (object) key
      }
    });
    return response.Value == null ? (string) null : response.Value.ToString();
  }

  public void Clear()
  {
    this.driver.InternalExecute(DriverCommand.ClearSessionStorage, (Dictionary<string, object>) null);
  }
}
