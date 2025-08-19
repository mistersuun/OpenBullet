// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.DefaultElementLocator
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

public class DefaultElementLocator : IElementLocator
{
  private ISearchContext searchContext;

  public DefaultElementLocator(ISearchContext searchContext) => this.searchContext = searchContext;

  public ISearchContext SearchContext => this.searchContext;

  public IWebElement LocateElement(IEnumerable<By> bys)
  {
    if (bys == null)
      throw new ArgumentNullException(nameof (bys), "List of criteria may not be null");
    string message = (string) null;
    foreach (By by in bys)
    {
      try
      {
        return this.searchContext.FindElement(by);
      }
      catch (NoSuchElementException ex)
      {
        message = (message == null ? (object) "Could not find element by: " : (object) (message + ", or: ")).ToString() + (object) by;
      }
    }
    throw new NoSuchElementException(message);
  }

  public ReadOnlyCollection<IWebElement> LocateElements(IEnumerable<By> bys)
  {
    if (bys == null)
      throw new ArgumentNullException(nameof (bys), "List of criteria may not be null");
    List<IWebElement> webElementList = new List<IWebElement>();
    foreach (By by in bys)
    {
      ReadOnlyCollection<IWebElement> elements = this.searchContext.FindElements(by);
      webElementList.AddRange((IEnumerable<IWebElement>) elements);
    }
    return webElementList.AsReadOnly();
  }
}
