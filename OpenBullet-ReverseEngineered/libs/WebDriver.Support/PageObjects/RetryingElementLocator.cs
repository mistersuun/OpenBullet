// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.RetryingElementLocator
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

public class RetryingElementLocator : IElementLocator
{
  private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5.0);
  private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(500.0);
  private ISearchContext searchContext;
  private TimeSpan timeout;
  private TimeSpan pollingInterval;

  public RetryingElementLocator(ISearchContext searchContext)
    : this(searchContext, RetryingElementLocator.DefaultTimeout)
  {
  }

  public RetryingElementLocator(ISearchContext searchContext, TimeSpan timeout)
    : this(searchContext, timeout, RetryingElementLocator.DefaultPollingInterval)
  {
  }

  public RetryingElementLocator(
    ISearchContext searchContext,
    TimeSpan timeout,
    TimeSpan pollingInterval)
  {
    this.searchContext = searchContext;
    this.timeout = timeout;
    this.pollingInterval = pollingInterval;
  }

  public ISearchContext SearchContext => this.searchContext;

  public IWebElement LocateElement(IEnumerable<By> bys)
  {
    if (bys == null)
      throw new ArgumentNullException(nameof (bys), "List of criteria may not be null");
    string message = (string) null;
    DateTime dateTime = DateTime.Now.Add(this.timeout);
    bool flag = DateTime.Now > dateTime;
    while (!flag)
    {
      foreach (By by in bys)
      {
        try
        {
          return this.SearchContext.FindElement(by);
        }
        catch (NoSuchElementException ex)
        {
          message = (message == null ? (object) "Could not find element by: " : (object) (message + ", or: ")).ToString() + (object) by;
        }
      }
      flag = DateTime.Now > dateTime;
      if (!flag)
        Thread.Sleep(this.pollingInterval);
    }
    throw new NoSuchElementException(message);
  }

  public ReadOnlyCollection<IWebElement> LocateElements(IEnumerable<By> bys)
  {
    if (bys == null)
      throw new ArgumentNullException(nameof (bys), "List of criteria may not be null");
    List<IWebElement> webElementList = new List<IWebElement>();
    DateTime dateTime = DateTime.Now.Add(this.timeout);
    bool flag = DateTime.Now > dateTime;
    while (!flag)
    {
      foreach (By by in bys)
      {
        ReadOnlyCollection<IWebElement> elements = this.SearchContext.FindElements(by);
        webElementList.AddRange((IEnumerable<IWebElement>) elements);
      }
      flag = webElementList.Count != 0 || DateTime.Now > dateTime;
      if (!flag)
        Thread.Sleep(this.pollingInterval);
    }
    return webElementList.AsReadOnly();
  }
}
