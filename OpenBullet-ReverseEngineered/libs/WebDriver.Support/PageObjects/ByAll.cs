// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.ByAll
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

public class ByAll : By
{
  private readonly By[] bys;

  public ByAll(params By[] bys) => this.bys = bys;

  public override IWebElement FindElement(ISearchContext context)
  {
    ReadOnlyCollection<IWebElement> elements = this.FindElements(context);
    return elements.Count != 0 ? elements[0] : throw new NoSuchElementException("Cannot locate an element using " + this.ToString());
  }

  public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
  {
    if (this.bys.Length == 0)
      return new List<IWebElement>().AsReadOnly();
    IEnumerable<IWebElement> webElements = (IEnumerable<IWebElement>) null;
    foreach (By by in this.bys)
    {
      ReadOnlyCollection<IWebElement> elements = by.FindElements(context);
      if (elements.Count == 0)
        return new List<IWebElement>().AsReadOnly();
      webElements = webElements != null ? webElements.Intersect<IWebElement>((IEnumerable<IWebElement>) by.FindElements(context)) : (IEnumerable<IWebElement>) elements;
    }
    return webElements.ToList<IWebElement>().AsReadOnly();
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
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "By.All([{0}])", (object) stringBuilder.ToString());
  }
}
