// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteWebStorage
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Html5;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteWebStorage : IWebStorage
{
  private RemoteWebDriver driver;

  public RemoteWebStorage(RemoteWebDriver driver) => this.driver = driver;

  public ILocalStorage LocalStorage => (ILocalStorage) new RemoteLocalStorage(this.driver);

  public ISessionStorage SessionStorage => (ISessionStorage) new RemoteSessionStorage(this.driver);
}
