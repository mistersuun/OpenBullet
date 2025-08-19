// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.ByIdOrName
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

public class ByIdOrName : By
{
  private string elementIdentifier = string.Empty;
  private By idFinder;
  private By nameFinder;

  public ByIdOrName(string elementIdentifier)
  {
    this.elementIdentifier = !string.IsNullOrEmpty(elementIdentifier) ? elementIdentifier : throw new ArgumentException("element identifier cannot be null or the empty string", nameof (elementIdentifier));
    this.idFinder = By.Id(this.elementIdentifier);
    this.nameFinder = By.Name(this.elementIdentifier);
  }

  public override IWebElement FindElement(ISearchContext context)
  {
    try
    {
      return this.idFinder.FindElement(context);
    }
    catch (NoSuchElementException ex)
    {
      return this.nameFinder.FindElement(context);
    }
  }

  public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
  {
    List<IWebElement> webElementList = new List<IWebElement>();
    webElementList.AddRange((IEnumerable<IWebElement>) this.idFinder.FindElements(context));
    webElementList.AddRange((IEnumerable<IWebElement>) this.nameFinder.FindElements(context));
    return webElementList.AsReadOnly();
  }

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ByIdOrName([{0}])", (object) this.elementIdentifier);
  }
}
