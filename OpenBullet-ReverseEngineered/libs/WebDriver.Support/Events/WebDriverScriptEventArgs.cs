// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.Events.WebDriverScriptEventArgs
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;

#nullable disable
namespace OpenQA.Selenium.Support.Events;

public class WebDriverScriptEventArgs : EventArgs
{
  private IWebDriver driver;
  private string script;

  public WebDriverScriptEventArgs(IWebDriver driver, string script)
  {
    this.driver = driver;
    this.script = script;
  }

  public IWebDriver Driver => this.driver;

  public string Script => this.script;
}
