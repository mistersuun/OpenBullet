// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.ByChained
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

public class ByChained : By
{
  private readonly By[] bys;

  public ByChained(params By[] bys) => this.bys = bys;

  public override IWebElement FindElement(ISearchContext context)
  {
    ReadOnlyCollection<IWebElement> elements = this.FindElements(context);
    return elements.Count != 0 ? elements[0] : throw new NoSuchElementException("Cannot locate an element using " + this.ToString());
  }

  public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
  {
    if (this.bys.Length == 0)
      return new List<IWebElement>().AsReadOnly();
    List<IWebElement> webElementList1 = (List<IWebElement>) null;
    foreach (By by in this.bys)
    {
      List<IWebElement> webElementList2 = new List<IWebElement>();
      if (webElementList1 == null)
      {
        webElementList2.AddRange((IEnumerable<IWebElement>) by.FindElements(context));
      }
      else
      {
        foreach (IWebElement webElement in webElementList1)
          webElementList2.AddRange((IEnumerable<IWebElement>) webElement.FindElements(by));
      }
      webElementList1 = webElementList2;
    }
    return webElementList1.AsReadOnly();
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (By by in this.bys)
    {
      if (stringBuilder.Length > 0)
        stringBuilder.Append(",");
      stringBuilder.Append((object) by);
    }
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "By.Chained([{0}])", (object) stringBuilder.ToString());
  }
}
