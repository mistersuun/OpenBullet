// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteAlert
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteAlert : IAlert
{
  private RemoteWebDriver driver;

  public RemoteAlert(RemoteWebDriver driver) => this.driver = driver;

  public string Text
  {
    get
    {
      return this.driver.InternalExecute(DriverCommand.GetAlertText, (Dictionary<string, object>) null).Value.ToString();
    }
  }

  public void Dismiss()
  {
    this.driver.InternalExecute(DriverCommand.DismissAlert, (Dictionary<string, object>) null);
  }

  public void Accept()
  {
    this.driver.InternalExecute(DriverCommand.AcceptAlert, (Dictionary<string, object>) null);
  }

  public void SendKeys(string keysToSend)
  {
    if (keysToSend == null)
      throw new ArgumentNullException(nameof (keysToSend), "Keys to send must not be null.");
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    if (this.driver.IsSpecificationCompliant)
    {
      parameters.Add("value", (object) keysToSend.ToCharArray());
      parameters.Add("text", (object) keysToSend);
    }
    else
      parameters.Add("text", (object) keysToSend);
    this.driver.InternalExecute(DriverCommand.SetAlertValue, parameters);
  }

  public void SetAuthenticationCredentials(string userName, string password)
  {
    this.driver.InternalExecute(DriverCommand.SetAlertCredentials, new Dictionary<string, object>()
    {
      {
        "username",
        (object) userName
      },
      {
        nameof (password),
        (object) password
      }
    });
  }
}
