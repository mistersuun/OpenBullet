// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.IWebDriver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium;

public interface IWebDriver : ISearchContext, IDisposable
{
  string Url { get; set; }

  string Title { get; }

  string PageSource { get; }

  string CurrentWindowHandle { get; }

  ReadOnlyCollection<string> WindowHandles { get; }

  void Close();

  void Quit();

  IOptions Manage();

  INavigation Navigate();

  ITargetLocator SwitchTo();
}
