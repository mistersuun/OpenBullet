// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteOptions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteOptions : IOptions
{
  private RemoteWebDriver driver;

  public RemoteOptions(RemoteWebDriver driver) => this.driver = driver;

  public ICookieJar Cookies => (ICookieJar) new RemoteCookieJar(this.driver);

  public IWindow Window => (IWindow) new RemoteWindow(this.driver);

  public ILogs Logs => (ILogs) new RemoteLogs(this.driver);

  public ITimeouts Timeouts() => (ITimeouts) new RemoteTimeouts(this.driver);
}
