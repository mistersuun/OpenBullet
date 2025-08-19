// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteCookieJar
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteCookieJar : ICookieJar
{
  private RemoteWebDriver driver;

  public RemoteCookieJar(RemoteWebDriver driver) => this.driver = driver;

  public ReadOnlyCollection<Cookie> AllCookies => this.GetAllCookies();

  public void AddCookie(Cookie cookie)
  {
    this.driver.InternalExecute(DriverCommand.AddCookie, new Dictionary<string, object>()
    {
      {
        nameof (cookie),
        (object) cookie
      }
    });
  }

  public void DeleteCookieNamed(string name)
  {
    this.driver.InternalExecute(DriverCommand.DeleteCookie, new Dictionary<string, object>()
    {
      {
        nameof (name),
        (object) name
      }
    });
  }

  public void DeleteCookie(Cookie cookie)
  {
    if (cookie == null)
      return;
    this.DeleteCookieNamed(cookie.Name);
  }

  public void DeleteAllCookies()
  {
    this.driver.InternalExecute(DriverCommand.DeleteAllCookies, (Dictionary<string, object>) null);
  }

  public Cookie GetCookieNamed(string name)
  {
    Cookie cookieNamed = (Cookie) null;
    if (name != null)
    {
      foreach (Cookie allCookie in this.AllCookies)
      {
        if (name.Equals(allCookie.Name))
        {
          cookieNamed = allCookie;
          break;
        }
      }
    }
    return cookieNamed;
  }

  private ReadOnlyCollection<Cookie> GetAllCookies()
  {
    List<Cookie> list = new List<Cookie>();
    object obj1 = this.driver.InternalExecute(DriverCommand.GetAllCookies, new Dictionary<string, object>()).Value;
    try
    {
      if (obj1 is object[] objArray)
      {
        for (int index = 0; index < objArray.Length; ++index)
        {
          object obj2 = objArray[index];
          Dictionary<string, object> rawCookie = obj2 as Dictionary<string, object>;
          if (obj2 != null)
            list.Add(Cookie.FromDictionary(rawCookie));
        }
      }
      return new ReadOnlyCollection<Cookie>((IList<Cookie>) list);
    }
    catch (Exception ex)
    {
      throw new WebDriverException("Unexpected problem getting cookies", ex);
    }
  }
}
